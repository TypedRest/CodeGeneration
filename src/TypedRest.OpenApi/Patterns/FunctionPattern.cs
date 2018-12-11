using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class FunctionPattern : RpcPattern
    {
        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new FunctionEndpoint();
    }
}
