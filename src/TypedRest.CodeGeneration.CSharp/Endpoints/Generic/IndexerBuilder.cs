using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Generic;

/// <summary>
/// Builds C# code snippets for <see cref="IndexerEndpoint"/>s.
/// </summary>
public class IndexerBuilder : BuilderBase<IndexerEndpoint>
{
    protected override (IEnumerable<ICSharpType> types, IEnumerable<CSharpIdentifier> typeArguments) GetAdditional(string key, IndexerEndpoint endpoint, IEndpointGenerator generator)
    {
        if (endpoint.Element == null) throw new InvalidOperationException($"Missing element for endpoint '{key}'.");

        string elementKey = key.TrimEnd('s') + "_Element";
        var (property, types) = generator.Generate(elementKey, endpoint.Element);
        return (types, typeArguments: new [] {property.GetterExpression!.Type});
    }

    protected override CSharpIdentifier GetImplementationType(IndexerEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "IndexerEndpoint");

    protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
    {
        var identifier = implementationType.ToInterface();
        // TODO: Skip this if Generator.WithInterfaces is not true
        identifier.TypeArguments[0] = identifier.TypeArguments[0].ToInterface();
        return identifier;
    }
}
