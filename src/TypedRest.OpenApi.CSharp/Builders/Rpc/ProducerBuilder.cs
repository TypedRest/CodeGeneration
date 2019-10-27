using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ProducerEndpoint"/>s.
    /// </summary>
    public class ProducerBuilder : BuilderBase<ProducerEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(ProducerEndpoint endpoint, ITypeLookup typeLookup)
            => new CSharpIdentifier(Namespace.Name, "ProducerEndpoint")
            {
                TypeArguments = {typeLookup[endpoint.Schema]}
            };
    }
}
