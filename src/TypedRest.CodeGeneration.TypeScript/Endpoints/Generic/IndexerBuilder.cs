using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints.Generic;

/// <summary>
/// Builds TypeScript code for <see cref="IndexerEndpoint"/>s.
/// </summary>
public class IndexerBuilder : BuilderBase<IndexerEndpoint>
{
    /// <inheritdoc/>
    /// <remarks>
    /// Unlike the C# <c>IndexerEndpoint</c>, which instantiates its element type itself, the TypeScript one takes
    /// the element endpoint class as a constructor argument.
    /// </remarks>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, IndexerEndpoint endpoint, IEndpointGenerator generator)
    {
        if (endpoint.Element == null) throw new InvalidOperationException($"Missing element for endpoint '{key}'.");

        var (getter, types) = generator.Generate(EndpointTree.ElementKey(key), endpoint.Element);
        var elementType = getter.Type;

        return (
            new TsIdentifier(generator.Modules.Generic, "IndexerEndpoint") {TypeArguments = {elementType}},
            types,
            [new TsTypeRef(elementType)]);
    }

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(IndexerEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Generic, "IndexerEndpoint");
}
