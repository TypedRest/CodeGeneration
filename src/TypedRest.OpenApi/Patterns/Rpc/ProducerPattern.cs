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
        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new ProducerEndpoint();
    }
}
