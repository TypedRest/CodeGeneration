using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm.Endpoints;

/// <summary>
/// Common base class for <see cref="IBuilder{TEndpoint}"/>s.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
public abstract class BuilderBase<TEndpoint> : IBuilder<TEndpoint>
    where TEndpoint : IEndpoint
{
    /// <inheritdoc/>
    public (JvmChildEndpoint child, IEnumerable<IJvmType> types) Build(string key, IEndpoint endpoint, IEndpointGenerator generator)
        => Build(key, (TEndpoint)endpoint, generator);

    /// <inheritdoc/>
    public (JvmChildEndpoint child, IEnumerable<IJvmType> types) Build(string key, TEndpoint endpoint, IEndpointGenerator generator)
    {
        var types = new List<IJvmType>();

        var (baseType, additionalTypes, extraArguments) = GetBase(key, endpoint, generator);
        types.AddRange(additionalTypes);

        var extras = extraArguments.ToList();

        var inlineCreation = new JvmNew(baseType) {Arguments = {JvmThis.Instance, RelativeUri(endpoint)}};
        inlineCreation.Arguments.AddRange(extras);

        var memberType = generator.WithInterfaces ? baseType.Interface ?? baseType : baseType;
        var implementationType = baseType;
        JvmExpression value = inlineCreation;

        if (endpoint.Children.Count > 0 || RequiresGeneratedClass)
        {
            var implementation = CustomImplementation(key, endpoint, baseType, extras, types, generator);
            types.Add(implementation);

            if (implementation.Interface is {} generatedInterface)
            {
                types.Add(BuildInterface(implementation, endpoint, baseType));
                memberType = generatedInterface;
            }
            else
                memberType = implementation.Identifier;

            implementationType = implementation.Identifier;

            // A generated class bakes its own relative URI in, unless it sits in an element position
            var creation = new JvmNew(implementationType) {Arguments = {JvmThis.Instance}};
            if (endpoint.Uri == null) creation.Arguments.Add(RelativeUri(endpoint));
            value = creation;
        }

        return (
            new JvmChildEndpoint(generator.Naming.Property(key), memberType, value)
            {
                ImplementationType = implementationType,
                Summary = endpoint.Description
            },
            types);
    }

    private JvmEndpointClass CustomImplementation(string key, TEndpoint endpoint, JvmIdentifier baseType, List<JvmExpression> extraArguments, List<IJvmType> types, IEndpointGenerator generator)
    {
        var name = generator.EndpointType(key, endpoint);

        var implementation = new JvmEndpointClass(generator.WithInterfaces ? name.WithName(name.Name + "Impl") : name)
        {
            Summary = endpoint.Description,
            BaseType = baseType,
            Interface = generator.WithInterfaces ? name : null,
            Constructor = BuildConstructor(endpoint, extraArguments, generator)
        };

        generator.PushParent(key);
        try
        {
            foreach ((string childKey, var childEndpoint) in endpoint.Children)
            {
                var (child, additionalTypes) = generator.Generate(childKey, childEndpoint);
                implementation.Children.Add(child);
                types.AddRange(additionalTypes);
            }
        }
        finally
        {
            generator.PopParent();
        }

        return implementation;
    }

    /// <summary>
    /// Builds the interface a generated endpoint class implements.
    /// </summary>
    /// <remarks>Extends the TypedRest interface behind the class's base type and redeclares the same children.</remarks>
    private static JvmEndpointInterface BuildInterface(JvmEndpointClass implementation, TEndpoint endpoint, JvmIdentifier baseType)
    {
        var result = new JvmEndpointInterface(implementation.Interface!)
        {
            Summary = endpoint.Description,
            BaseType = baseType.Interface
        };
        result.Children.AddRange(implementation.Children);
        return result;
    }

    /// <summary>
    /// Builds the constructor of a generated endpoint class.
    /// </summary>
    protected virtual JvmConstructor BuildConstructor(TEndpoint endpoint, List<JvmExpression> extraArguments, IEndpointGenerator generator)
    {
        bool hasUri = endpoint.Uri != null;

        var constructor = new JvmConstructor
        {
            Parameters = {new JvmParameter("referrer", Packages.Endpoint)},
            BaseArguments = {new JvmName("referrer"), RelativeUri(endpoint)}
        };
        if (!hasUri) constructor.Parameters.Add(new JvmParameter("relativeUri", JvmIdentifier.Uri));
        constructor.BaseArguments.AddRange(extraArguments);

        return constructor;
    }

    /// <summary>
    /// The relative URI of the endpoint, either as a literal or as the constructor parameter it is handed in.
    /// </summary>
    protected JvmExpression RelativeUri(TEndpoint endpoint)
        => endpoint.Uri switch
        {
            null => new JvmName("relativeUri"),
            {} uri when RequiresUriObject => new JvmUriLiteral(uri),
            {} uri => new JvmLiteral(uri)
        };

    /// <summary>
    /// Returns the TypedRest type the endpoint derives from, any additional types generated along the way, and
    /// any constructor arguments beyond the referrer and the relative URI.
    /// </summary>
    protected virtual (JvmIdentifier baseType, IEnumerable<IJvmType> types, IEnumerable<JvmExpression> extraArguments) GetBase(string key, TEndpoint endpoint, IEndpointGenerator generator)
        => (GetBaseType(endpoint, generator), [], ExtraArguments(endpoint, generator));

    /// <summary>
    /// Returns the TypedRest implementation class the endpoint derives from.
    /// </summary>
    protected abstract JvmIdentifier GetBaseType(TEndpoint endpoint, IEndpointGenerator generator);

    /// <summary>
    /// Indicates whether this kind of endpoint needs a generated class even when it has no children.
    /// </summary>
    /// <remarks>
    /// Most kinds map to a concrete TypedRest class, which is simply constructed inline where there are no children
    /// to hold. Override this where the base type cannot be instantiated.
    /// </remarks>
    protected virtual bool RequiresGeneratedClass => false;

    /// <summary>
    /// Indicates whether the base type takes the relative URI as a <c>URI</c> rather than a <c>String</c>.
    /// </summary>
    /// <remarks>
    /// Every <c>Impl</c> class has a secondary constructor taking a <c>String</c>. Override this where the base
    /// type has only the <c>URI</c> one, so that the literal gets wrapped.
    /// </remarks>
    protected virtual bool RequiresUriObject => false;

    /// <summary>
    /// Returns the constructor arguments beyond the referrer and the relative URI.
    /// </summary>
    /// <remarks>Most JVM endpoints take a <c>Class&lt;T&gt;</c> for the entity they deserialize.</remarks>
    protected virtual IEnumerable<JvmExpression> ExtraArguments(TEndpoint endpoint, IEndpointGenerator generator)
        => [];
}
