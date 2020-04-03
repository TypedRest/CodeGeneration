using TypedRest.CodeGeneration.Endpoints.Reactive;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Reactive
{
    /// <summary>
    /// Builds C# code snippets for <see cref="StreamingCollectionEndpoint"/>s.
    /// </summary>
    public class StreamingCollectionBuilder : CollectionBuilderBase<StreamingCollectionEndpoint>
    {
        protected override string TypeNamespace => Namespace.Name;

        protected override string TypeName => "StreamingCollectionEndpoint";
    }
}
