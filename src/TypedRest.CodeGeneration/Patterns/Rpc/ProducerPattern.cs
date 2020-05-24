using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc
{
    /// <summary>
    /// A pattern that generates <see cref="ProducerEndpoint"/>s.
    /// </summary>
    public class ProducerPattern : RpcPatternBase
    {
        protected override IEndpoint? BuildEndpoint(OpenApiOperation operation)
        {
            var schema = operation.Get20XResponse()?.GetJsonSchema();
            if (schema == null || operation.RequestBody != null) return null;

            return new ProducerEndpoint {Schema = schema};
        }
    }
}
