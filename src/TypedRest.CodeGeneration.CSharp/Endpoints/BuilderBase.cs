using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

/// <summary>
/// Common base class for <see cref="IBuilder{TEndpoint}"/>s.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
public abstract class BuilderBase<TEndpoint> : IBuilder<TEndpoint>
    where TEndpoint : IEndpoint
{
    public (CSharpProperty property, IEnumerable<ICSharpType> types) Build(string key, IEndpoint endpoint, IEndpointGenerator generator)
        => Build(key, (TEndpoint)endpoint, generator);

    public (CSharpProperty property, IEnumerable<ICSharpType> types) Build(string key, TEndpoint endpoint, IEndpointGenerator generator)
    {
        var types = new List<ICSharpType>();
        var implementationType = GetImplementationType(endpoint, generator.Naming);

        var (additionalTypes, typeArguments) = GetAdditional(key, endpoint, generator);
        types.AddRange(additionalTypes);
        implementationType.TypeArguments.AddRange(typeArguments);

        var construction = new CSharpConstructor(implementationType);
        construction.Parameters.AddRange(GetParameters(endpoint));

        var interfaceType = GetInterfaceType(implementationType);

        if (endpoint.Children.Count > 0)
        {
            var customImplementation = CustomImplementation(key, endpoint, construction, types, generator);
            types.Add(customImplementation);
            construction = customImplementation.GetConstruction();

            if (generator.WithInterfaces)
            {
                var customInterface = CustomInterface(endpoint, interfaceType, customImplementation);
                types.Add(customInterface);
                interfaceType = customInterface.Identifier;
            }
            else
                interfaceType = customImplementation.Identifier;
        }

        var property = new CSharpProperty(interfaceType, generator.Naming.Property(key))
        {
            GetterExpression = construction,
            Summary = endpoint.Description
        };

        return (property, types);
    }

    private static CSharpClass CustomImplementation(string key, TEndpoint endpoint, CSharpConstructor baseClass, List<ICSharpType> types, IEndpointGenerator generator)
    {
        var customImplementation = new CSharpClass(generator.Naming.EndpointType(key, endpoint))
        {
            Summary = endpoint.Description,
            Attributes = {Attributes.GeneratedCode},
            BaseClass = baseClass
        };

        foreach ((string childKey, var childEndpoint) in endpoint.Children)
        {
            var (property, additionalTypes) = generator.Generate(childKey, childEndpoint);
            customImplementation.Properties.Add(property);
            types.AddRange(additionalTypes);
        }

        return customImplementation;
    }

    private static CSharpInterface CustomInterface(TEndpoint endpoint, CSharpIdentifier interfaceType, CSharpClass implementation)
    {
        var endpointInterface = new CSharpInterface(implementation.Identifier.ToInterface())
        {
            Summary = endpoint.Description,
            Attributes = {Attributes.GeneratedCode},
            Interfaces = {interfaceType}
        };
        foreach (var property in implementation.Properties)
            endpointInterface.Properties.Add(new CSharpProperty(property.Type, property.Name) {Summary = property.Summary});

        implementation.Interfaces.Add(endpointInterface.Identifier);

        return endpointInterface;
    }

    protected virtual (IEnumerable<ICSharpType> types, IEnumerable<CSharpIdentifier> typeArguments) GetAdditional(string key, TEndpoint endpoint, IEndpointGenerator generator)
        => (Enumerable.Empty<CSharpType>(), Enumerable.Empty<CSharpIdentifier>());

    protected abstract CSharpIdentifier GetImplementationType(TEndpoint endpoint, INamingStrategy naming);

    protected virtual CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
        => implementationType.ToInterface();

    protected virtual IEnumerable<CSharpParameter> GetParameters(TEndpoint endpoint)
        => new[]
        {
            new CSharpParameter(new CSharpIdentifier(Namespace.Name, "IEndpoint"), "referrer") {Value = new ThisReference()},
            new CSharpParameter(endpoint.Uri == null ? CSharpIdentifier.Uri : CSharpIdentifier.String, "relativeUri") {Value = endpoint.Uri}
        };
}
