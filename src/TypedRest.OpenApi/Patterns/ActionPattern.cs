using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class ActionPattern : RpcPattern
    {
        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new ActionEndpoint();
    }
}
