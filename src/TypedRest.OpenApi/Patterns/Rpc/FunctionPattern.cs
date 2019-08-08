using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.Patterns.Rpc
{
    /// <summary>
    /// A pattern that generates <see cref="FunctionEndpoint"/>s.
    /// </summary>
    public class FunctionPattern : RpcPatternBase
    {
        protected override IEndpoint BuildEndpoint(OpenApiOperation operation)
        {
            var requestSchema = operation.GetRequestSchema();
            var responseSchema = operation.GetResponseSchema();
            if (requestSchema == null || responseSchema == null) return null;

            return new FunctionEndpoint {RequestSchema = requestSchema, ResponseSchema = responseSchema};
        }
    }
}
