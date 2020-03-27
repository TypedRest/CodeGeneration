using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders.Generic
{
    /// <summary>
    /// Builds C# code snippets for <see cref="IndexerEndpoint"/>s.
    /// </summary>
    public class IndexerBuilder : IndexerBuilderBase<IndexerEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(IndexerEndpoint endpoint, INamingStrategy naming)
            => new CSharpIdentifier(Namespace.Name, "IndexerEndpoint");

        protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
        {
            var identifier = implementationType.ToInterface();
            identifier.TypeArguments[0] = identifier.TypeArguments[0].ToInterface();
            return identifier;
        }
    }
}
