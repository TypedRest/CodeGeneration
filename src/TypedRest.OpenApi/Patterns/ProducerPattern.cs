using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class ProducerPattern : RpcPattern
    {
        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new ProducerEndpoint();
    }
}
