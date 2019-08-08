using System.Linq;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi
{
    public static class OpenApiOperationExtensions
    {
        /// <summary>
        /// Returns the schema for the first defined JSON request, if any.
        /// </summary>
        [CanBeNull]
        public static OpenApiSchema GetRequestSchema([NotNull] this OpenApiOperation operation)
            => operation.RequestBody
                       ?.Content.FirstOrDefault(x => x.Key.Contains("json")).Value
                       ?.Schema;

        /// <summary>
        /// Returns the schema for the first defined JSON response with a 2xx status code, if any.
        /// </summary>
        [CanBeNull]
        public static OpenApiSchema GetResponseSchema([NotNull] this OpenApiOperation operation)
            => operation.Responses
                        .FirstOrDefault(x => x.Key.StartsWith("2")).Value
                       ?.Content.FirstOrDefault(x => x.Key.Contains("json")).Value
                       ?.Schema;
    }
}
