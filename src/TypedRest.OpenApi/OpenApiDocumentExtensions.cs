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
        public static EndpointList? GetTypedRestEndpoints(this OpenApiDocument document)
        {
            document.Extensions.TryGetValue(EndpointList.ExtensionKey, out var list);
            return list as EndpointList;
        }

        /// <summary>
        /// Adds the <paramref name="list"/> as a TypedRest extension to the OpenAPI Spec <paramref name="document"/>.
        /// </summary>
        public static OpenApiDocument SetTypedRestEndpoints(this OpenApiDocument document, EndpointList list)
        {
            document.Extensions[EndpointList.ExtensionKey] = list;
            return document;
        }

        /// <summary>
        /// Resolves <see cref="OpenApiReference"/>s in TypedRest endpoints.
        /// </summary>
        public static OpenApiDocument ResolveTypedRestReferences(this OpenApiDocument doc)
        {
            doc.GetTypedRestEndpoints()?.ResolveReferences(doc.Components);
            return doc;
        }
    }
}
