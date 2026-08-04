using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.CSharp.Endpoints.Generic;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

/// <summary>
/// Common base class for <see cref="IBuilder{TEndpoint}"/>s for <see cref="CollectionEndpoint"/> and derived types.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="CollectionEndpoint"/> to generate code for.</typeparam>
public abstract class CollectionBuilderBase<TEndpoint> : BuilderBase<TEndpoint>
    where TEndpoint : CollectionEndpoint
{
    protected abstract string TypeNamespace { get; }

    protected abstract string TypeName { get; }

    protected override CSharpIdentifier GetImplementationType(TEndpoint endpoint, INamingStrategy naming)
        => new(TypeNamespace, TypeName)
        {
            TypeArguments =
            {
                naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))
            }
        };

    protected override (IEnumerable<ICSharpType> types, IEnumerable<CSharpIdentifier> typeArguments) GetAdditional(string key, TEndpoint endpoint, IEndpointGenerator generator)
    {
        if (endpoint.Element == null)
            return (Enumerable.Empty<CSharpType>(), Enumerable.Empty<CSharpIdentifier>());

        endpoint.Element.Schema ??= endpoint.Schema;

        string elementKey = key.Depluralize() + "_Element";
        var (property, types) = generator.Generate(elementKey, endpoint.Element);
        return (types, typeArguments: new [] {property.GetterExpression!.Type});
    }

    protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType, bool withInterfaces)
    {
        var identifier = implementationType.ToInterface();

        if (withInterfaces && identifier.TypeArguments.Count == 2)
            identifier.TypeArguments[1] = identifier.TypeArguments[1].ToInterface();

        return identifier;
    }

    protected override void AddInterfaceAdapters(CSharpClass implementation, CSharpIdentifier interfaceType)
    {
        // Without a separate element endpoint the base class is the specialized CollectionEndpoint<TEntity>,
        // which already implements the interface-typed members itself
        if (interfaceType.TypeArguments.Count != 2) return;

        var entity = interfaceType.TypeArguments[0];
        var element = interfaceType.TypeArguments[1];

        // CreateAsync() returns a nullable element endpoint
        implementation.NullableContext = true;

        implementation.Indexers.Add(InterfaceAdapters.IndexerById(element));
        implementation.Indexers.Add(InterfaceAdapters.IndexerByEntity(entity, element));
        implementation.Methods.Add(InterfaceAdapters.CreateAsync(entity, element));
    }
}
