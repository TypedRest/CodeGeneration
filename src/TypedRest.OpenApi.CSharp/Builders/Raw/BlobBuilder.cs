using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Raw;

namespace TypedRest.OpenApi.CSharp.Builders.Raw
{
    /// <summary>
    /// Builds C# code snippets for <see cref="BlobEndpoint"/>s.
    /// </summary>
    public class BlobBuilder : BuilderBase<BlobEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(BlobEndpoint endpoint, ITypeList typeList)
            => new CSharpIdentifier(Namespace.Name, "BlobEndpoint");
    }
}