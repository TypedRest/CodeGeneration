using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Raw;

namespace TypedRest.CodeGeneration.CSharp.Builders.Raw
{
    /// <summary>
    /// Builds C# code snippets for <see cref="BlobEndpoint"/>s.
    /// </summary>
    public class BlobBuilder : BuilderBase<BlobEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(BlobEndpoint endpoint, INamingStrategy naming)
            => new CSharpIdentifier(Namespace.Name, "BlobEndpoint");
    }
}
