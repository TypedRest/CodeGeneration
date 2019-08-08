using System.Net;
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
            var requestSchema = operation.GetRequest()?.GetJsonSchema();
            var responseSchema = operation.Get20XResponse()?.GetJsonSchema();
            if (requestSchema == null || responseSchema == null) return null;

            return new FunctionEndpoint {RequestSchema = requestSchema, ResponseSchema = responseSchema};
        }
    }
}
