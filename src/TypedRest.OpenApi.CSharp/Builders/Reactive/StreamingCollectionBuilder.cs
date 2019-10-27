using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Reactive;

namespace TypedRest.OpenApi.CSharp.Builders.Reactive
{
    /// <summary>
    /// Builds C# code snippets for <see cref="StreamingCollectionEndpoint"/>s.
    /// </summary>
    public class StreamingCollectionBuilder : BuilderBase<StreamingCollectionEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(StreamingCollectionEndpoint endpoint, ITypeLookup typeLookup)
        {
            var identifier = new CSharpIdentifier(Namespace.Name, "StreamingCollectionEndpoint")
            {
                TypeArguments = {typeLookup[endpoint.Schema]}
            };

            if (endpoint.Element != null)
                identifier.TypeArguments.Add(typeLookup[endpoint.Element]);

            return identifier;
        }
    }
}
