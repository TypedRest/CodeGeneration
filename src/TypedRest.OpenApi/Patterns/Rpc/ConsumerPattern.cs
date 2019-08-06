using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.Patterns.Rpc
{
    /// <summary>
    /// A pattern that generates <see cref="ConsumerEndpoint"/>s.
    /// </summary>
    public class ConsumerPattern : RpcPatternBase
    {
        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new ConsumerEndpoint();
    }
}
