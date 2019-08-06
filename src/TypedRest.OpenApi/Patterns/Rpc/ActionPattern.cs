using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.Patterns.Rpc
{
    /// <summary>
    /// A pattern that generates <see cref="ActionEndpoint"/>s.
    /// </summary>
    public class ActionPattern : RpcPatternBase
    {
        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new ActionEndpoint();
    }
}
