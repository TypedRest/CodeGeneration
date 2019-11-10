using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi
{
    public static class OpenApiOperationExtensions
    {
        /// <summary>
        /// Gets the media types for requests.
        /// </summary>
        public static IDictionary<string, OpenApiMediaType>? GetRequest(this OpenApiOperation operation)
            => operation.RequestBody?.Content;

        /// <summary>
        /// Gets the media types for HTTP 200 responses.
        /// </summary>
        public static IDictionary<string, OpenApiMediaType>? Get200Response(this OpenApiOperation operation)
            => operation.GetResponse(HttpStatusCode.OK);

        /// <summary>
        /// Gets the media types for HTTP 200, 201 and 202 responses.
        /// </summary>
        public static IDictionary<string, OpenApiMediaType>? Get20XResponse(this OpenApiOperation operation)
            => operation.GetResponse(HttpStatusCode.OK)
            ?? operation.GetResponse(HttpStatusCode.Created)
            ?? operation.GetResponse(HttpStatusCode.Accepted);

        /// <summary>
        /// Gets the media types for responses with a specific HTTP <paramref name="statusCode"/>.
        /// </summary>
        public static IDictionary<string, OpenApiMediaType>? GetResponse(this OpenApiOperation operation, HttpStatusCode statusCode)
            => operation.Responses.TryGetValue(((int)statusCode).ToString(), out var response) ? response.Content : null;

        /// <summary>
        /// Gets the schema for a JSON media type, if any.
        /// </summary>
        public static OpenApiSchema? GetJsonSchema(this IDictionary<string, OpenApiMediaType> content)
            => content.FirstOrDefault(x => x.Key.Contains("/json")).Value?.Schema;
    }
}
