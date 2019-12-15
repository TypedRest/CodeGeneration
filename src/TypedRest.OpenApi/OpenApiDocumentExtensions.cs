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
        /// The property name used to add TypedRest as an extension to an <see cref="OpenApiDocument"/>.
        /// </summary>
        public const string TypedRestKey = "x-endpoints";

        /// <summary>
        /// Gets the TypedRest extension from the OpenAPI Spec <paramref name="document"/>.
        /// </summary>
        /// <seealso cref="OpenApiReaderSettingsExtensions.AddTypedRest"/>
        public static EntryEndpoint? GetTypedRest(this OpenApiDocument document)
        {
            document.Extensions.TryGetValue(TypedRestKey, out var endpoint);
            return endpoint as EntryEndpoint;
        }

        /// <summary>
        /// Adds the <paramref name="endpoint"/> as a TypedRest extension to the OpenAPI Spec <paramref name="document"/>.
        /// </summary>
        public static OpenApiDocument SetTypedRest(this OpenApiDocument document, EntryEndpoint endpoint)
        {
            document.Extensions[TypedRestKey] = endpoint;
            return document;
        }

        /// <summary>
        /// Resolves <see cref="OpenApiReference"/>s in TypedRest endpoints.
        /// </summary>
        public static OpenApiDocument ResolveTypedRestReferences(this OpenApiDocument doc)
        {
            doc.GetTypedRest()?.ResolveReferences(doc.Components);
            return doc;
        }
    }
}
