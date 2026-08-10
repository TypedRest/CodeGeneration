using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints;

/// <summary>
/// Common base class for <see cref="IBuilder{TEndpoint}"/>s.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
public abstract class BuilderBase<TEndpoint> : IBuilder<TEndpoint>
    where TEndpoint : IEndpoint
{
    /// <inheritdoc/>
    public (TsGetter getter, IEnumerable<ITsType> types) Build(string key, IEndpoint endpoint, IEndpointGenerator generator)
        => Build(key, (TEndpoint)endpoint, generator);

    /// <inheritdoc/>
    public (TsGetter getter, IEnumerable<ITsType> types) Build(string key, TEndpoint endpoint, IEndpointGenerator generator)
    {
        var types = new List<ITsType>();

        var (baseType, additionalTypes, extraArguments) = GetBase(key, endpoint, generator);
        types.AddRange(additionalTypes);

        var extras = extraArguments.ToList();

        var inlineCreation = new TsNew(baseType) {Arguments = {TsThis.Instance, RelativeUri(endpoint)}};
        inlineCreation.Arguments.AddRange(extras);

        var getterType = baseType;
        TsExpression body = inlineCreation;

        if (endpoint.Children.Count > 0)
        {
            var implementation = CustomImplementation(key, endpoint, baseType, extras, types, generator);
            types.Add(implementation);

            getterType = implementation.Identifier;

            // A generated class bakes its own relative URI in, unless it sits in an element position
            var creation = new TsNew(getterType) {Arguments = {TsThis.Instance}};
            if (endpoint.Uri == null) creation.Arguments.Add(RelativeUri(endpoint));
            body = creation;
        }

        return (new TsGetter(generator.Naming.Property(key), getterType, body) {Summary = endpoint.Description}, types);
    }

    private TsClass CustomImplementation(string key, TEndpoint endpoint, TsIdentifier baseType, List<TsExpression> extraArguments, List<ITsType> types, IEndpointGenerator generator)
    {
        var implementation = new TsClass(generator.EndpointType(key, endpoint))
        {
            Summary = endpoint.Description,
            BaseType = baseType,
            Constructor = BuildConstructor(endpoint, extraArguments, generator)
        };

        generator.PushParent(key);
        try
        {
            foreach ((string childKey, var childEndpoint) in endpoint.Children)
            {
                var (getter, additionalTypes) = generator.Generate(childKey, childEndpoint);
                implementation.Getters.Add(getter);
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
    /// Builds the constructor of a generated endpoint class, or <c>null</c> to inherit the base constructor.
    /// </summary>
    /// <remarks>
    /// An endpoint that knows its own relative URI bakes it into the <c>super()</c> call and drops the parameter.
    /// An endpoint in an element position does not know its URI - it is handed one per element by the collection
    /// or indexer that owns it - so it must keep forwarding the parameter. Hard-coding a URI there would still
    /// compile, because TypeScript accepts a constructor with fewer parameters where one with more is expected,
    /// but every element would silently resolve to the same URI at runtime.
    /// </remarks>
    protected virtual TsConstructor? BuildConstructor(TEndpoint endpoint, List<TsExpression> extraArguments, IEndpointGenerator generator)
    {
        bool hasUri = endpoint.Uri != null;

        // super(referrer, relativeUri) verbatim - just inherit it
        if (!hasUri && extraArguments.Count == 0) return null;

        var constructor = new TsConstructor
        {
            Parameters = {new TsParameter("referrer", new TsIdentifier(generator.Modules.Endpoints, "Endpoint"))},
            SuperArguments = {new TsName("referrer"), RelativeUri(endpoint)}
        };
        if (!hasUri) constructor.Parameters.Add(new TsParameter("relativeUri", TsIdentifier.UrlOrString));
        constructor.SuperArguments.AddRange(extraArguments);

        return constructor;
    }

    private static TsExpression RelativeUri(TEndpoint endpoint)
        => endpoint.Uri == null ? new TsName("relativeUri") : new TsLiteral(endpoint.Uri);

    /// <summary>
    /// Returns the TypedRest type the endpoint derives from, any additional types generated along the way, and
    /// any constructor arguments beyond the referrer and the relative URI.
    /// </summary>
    protected virtual (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, TEndpoint endpoint, IEndpointGenerator generator)
        => (GetBaseType(endpoint, generator), [], []);

    /// <summary>
    /// Returns the TypedRest type the endpoint derives from.
    /// </summary>
    protected abstract TsIdentifier GetBaseType(TEndpoint endpoint, IEndpointGenerator generator);
}
