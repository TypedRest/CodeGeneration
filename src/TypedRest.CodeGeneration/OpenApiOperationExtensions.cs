using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.OpenApi.Models;

namespace TypedRest.CodeGeneration
{
    public static class OpenApiOperationExtensions
    {
        /// <summary>
        /// Gets the HTTP 200 response, if any.
        /// </summary>
        public static OpenApiResponse? Get200Response(this OpenApiOperation operation)
            => operation.GetResponse(HttpStatusCode.OK);

        /// <summary>
        /// Gets the HTTP 200, 201, 202 or 204 response, if any.
        /// </summary>
        public static OpenApiResponse? Get20XResponse(this OpenApiOperation operation)
            => operation.GetResponse(HttpStatusCode.OK)
            ?? operation.GetResponse(HttpStatusCode.Created)
            ?? operation.GetResponse(HttpStatusCode.Accepted)
            ?? operation.GetResponse(HttpStatusCode.NoContent);

        /// <summary>
        /// Gets the response for a specific HTTP <paramref name="statusCode"/>, if any.
        /// </summary>
        public static OpenApiResponse? GetResponse(this OpenApiOperation operation, HttpStatusCode statusCode)
            => operation.Responses.TryGetValue(((int)statusCode).ToString(), out var response) ? response : null;

        /// <summary>
        /// Gets the schema for the JSON media type, if any.
        /// </summary>
        public static OpenApiSchema? GetJsonSchema(this OpenApiRequestBody request)
            => request.Content.GetJsonSchema();

        /// <summary>
        /// Gets the schema for the JSON media type, if any.
        /// </summary>
        public static OpenApiSchema? GetJsonSchema(this OpenApiResponse response)
            => response.Content.GetJsonSchema();

        private static OpenApiSchema? GetJsonSchema(this IDictionary<string, OpenApiMediaType> content)
            => content.FirstOrDefault(x => x.Key.Contains("/json")).Value?.Schema;
    }
}
