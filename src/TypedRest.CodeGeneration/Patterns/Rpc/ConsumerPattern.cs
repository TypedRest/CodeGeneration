using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc
{
    /// <summary>
    /// A pattern that generates <see cref="ConsumerEndpoint"/>s.
    /// </summary>
    public class ConsumerPattern : RpcPatternBase
    {
        protected override IEndpoint? BuildEndpoint(OpenApiOperation operation)
        {
            var schema = operation.RequestBody?.GetJsonSchema();
            if (schema == null) return null;

            return new ConsumerEndpoint {Schema = schema};
        }
    }
}
