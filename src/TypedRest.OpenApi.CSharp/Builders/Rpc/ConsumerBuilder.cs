using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ConsumerEndpoint"/>s.
    /// </summary>
    public class ConsumerBuilder : BuilderBase<ConsumerEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(ConsumerEndpoint endpoint, ITypeLookup typeLookup)
            => new CSharpIdentifier(Namespace.Name, "ConsumerEndpoint")
            {
                TypeArguments = {typeLookup[endpoint.Schema]}
            };
    }
}
