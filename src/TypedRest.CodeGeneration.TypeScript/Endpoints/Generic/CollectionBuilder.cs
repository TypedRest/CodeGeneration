using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints.Generic;

/// <summary>
/// Builds TypeScript code for <see cref="CollectionEndpoint"/>s.
/// </summary>
public class CollectionBuilder : CollectionBuilderBase<CollectionEndpoint>
{
    /// <inheritdoc/>
    protected override string TypeName => "CollectionEndpoint";

    /// <inheritdoc/>
    protected override string GenericTypeName => "GenericCollectionEndpoint";
}
