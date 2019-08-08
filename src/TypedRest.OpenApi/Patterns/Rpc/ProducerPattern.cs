using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.Patterns.Rpc
{
    /// <summary>
    /// A pattern that generates <see cref="ProducerEndpoint"/>s.
    /// </summary>
    public class ProducerPattern : RpcPatternBase
    {
        protected override IEndpoint BuildEndpoint(OpenApiOperation operation)
        {
            var schema = operation.GetResponseSchema();
            if (schema == null) return null;

            return new ProducerEndpoint {Schema = schema};
        }
    }
}
