using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders.Generic
{
    /// <summary>
    /// Builds C# code snippets for <see cref="IndexerEndpoint"/>s.
    /// </summary>
    public class IndexerBuilder : BuilderBase<IndexerEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(IndexerEndpoint endpoint, ITypeList typeList)
            => new CSharpIdentifier(Namespace.Name, "IndexerEndpoint")
            {
                TypeArguments = {typeList[endpoint.Element]}
            };
    }
}
