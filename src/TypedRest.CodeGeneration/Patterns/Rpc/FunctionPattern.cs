using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc
{
    /// <summary>
    /// A pattern that generates <see cref="FunctionEndpoint"/>s.
    /// </summary>
    public class FunctionPattern : RpcPatternBase
    {
        protected override IEndpoint? BuildEndpoint(OpenApiOperation operation)
        {
            var requestSchema = operation.RequestBody?.GetJsonSchema();
            var responseSchema = operation.Get20XResponse()?.GetJsonSchema();
            if (requestSchema == null || responseSchema == null) return null;

            return new FunctionEndpoint {RequestSchema = requestSchema, ResponseSchema = responseSchema};
        }
    }
}
