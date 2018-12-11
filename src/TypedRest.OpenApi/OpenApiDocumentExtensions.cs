using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi
{
    /// <summary>
    /// Provides extension methods for <see cref="OpenApiDocument"/>s.
    /// </summary>
    public static class OpenApiDocumentExtensions
    {
        /// <summary>
        /// Gets the TypedRest extension from the OpenAPI Spec <paramref name="document"/>.
        /// </summary>
        /// <seealso cref="OpenApiReaderSettingsExtensions.AddTypedRest"/>
        [PublicAPI]
        public static EndpointList GetTypedRestEndpoints([NotNull] this OpenApiDocument document)
        {
            document.Extensions.TryGetValue(EndpointList.ExtensionKey, out var list);
            return list as EndpointList;
        }

        /// <summary>
        /// Adds the <paramref name="list"/> as a TypedRest extension to the OpenAPI Spec <paramref name="document"/>.
        /// </summary>
        [PublicAPI]
        public static OpenApiDocument SetTypedRestEndpoints([NotNull] this OpenApiDocument document, [NotNull] EndpointList list)
        {
            document.Extensions[EndpointList.ExtensionKey] = list;
            return document;
        }
    }
}
