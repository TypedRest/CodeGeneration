using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Generic;

/// <summary>
/// Builds C# code snippets for <see cref="CollectionEndpoint"/>s.
/// </summary>
public class CollectionBuilder : CollectionBuilderBase<CollectionEndpoint>
{
    protected override string TypeNamespace => Namespace.Name;

    protected override string TypeName => "CollectionEndpoint";
}
