using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Reactive;

namespace TypedRest.OpenApi.CSharp.Builders.Reactive
{
    /// <summary>
    /// Builds C# code snippets for <see cref="StreamingCollectionEndpoint"/>s.
    /// </summary>
    public class StreamingCollectionBuilder : BuilderBase<StreamingCollectionEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(StreamingCollectionEndpoint endpoint, ITypeList typeList)
        {
            var identifier = new CSharpIdentifier(Namespace.Name, "StreamingCollectionEndpoint")
            {
                TypeArguments = {typeList[endpoint.Schema]}
            };

            if (endpoint.Element != null)
                identifier.TypeArguments.Add(typeList[endpoint.Element]);

            return identifier;
        }
    }
}
