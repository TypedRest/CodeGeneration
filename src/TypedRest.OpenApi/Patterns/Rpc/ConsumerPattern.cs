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
        protected override IEndpoint? BuildEndpoint(OpenApiOperation operation)
        {
            var schema = operation.GetRequest()?.GetJsonSchema();
            if (schema == null) return null;

            return new ConsumerEndpoint {Schema = schema};
        }
    }
}
