using System.Collections.Generic;
using System.Linq;
using System.Net;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi
{
    public static class OpenApiOperationExtensions
    {
        /// <summary>
        /// Gets the media types for requests.
        /// </summary>
        [CanBeNull]
        public static IDictionary<string, OpenApiMediaType> GetRequest([NotNull] this OpenApiOperation operation)
            => operation.RequestBody?.Content;

        /// <summary>
        /// Gets the media types for HTTP 200 responses.
        /// </summary>
        [CanBeNull]
        public static IDictionary<string, OpenApiMediaType> Get200Response([NotNull] this OpenApiOperation operation)
            => operation.GetResponse(HttpStatusCode.OK);

        /// <summary>
        /// Gets the media types for HTTP 200, 201 and 202 responses.
        /// </summary>
        [CanBeNull]
        public static IDictionary<string, OpenApiMediaType> Get20XResponse([NotNull] this OpenApiOperation operation)
            => operation.GetResponse(HttpStatusCode.OK)
            ?? operation.GetResponse(HttpStatusCode.Created)
            ?? operation.GetResponse(HttpStatusCode.Accepted);

        /// <summary>
        /// Gets the media types for responses with a specific HTTP <paramref name="statusCode"/>.
        /// </summary>
        [CanBeNull]
        public static IDictionary<string, OpenApiMediaType> GetResponse([NotNull] this OpenApiOperation operation, HttpStatusCode statusCode)
            => operation.Responses.TryGetValue(((int)statusCode).ToString(), out var response) ? response.Content : null;

        /// <summary>
        /// Gets the schema for a JSON media type, if any.
        /// </summary>
        public static OpenApiSchema GetJsonSchema([NotNull] this IDictionary<string, OpenApiMediaType> content)
            => content.FirstOrDefault(x => x.Key.Contains("/json")).Value?.Schema;
    }
}
