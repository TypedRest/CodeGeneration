using Microsoft.OpenApi.Readers;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi
{
    /// <summary>
    /// Provides extension methods for <see cref="OpenApiReaderSettings"/>s.
    /// </summary>
    public static class OpenApiReaderSettingsExtensions
    {
        /// <summary>
        /// Adds support for the TypedRest OpenAPI Spec Extension.
        /// </summary>
        /// <seealso cref="OpenApiDocumentExtensions.GetTypedRestEndpoints"/>
        public static OpenApiReaderSettings AddTypedRest(this OpenApiReaderSettings settings, EndpointRegistry? endpointRegistry = null)
        {
            var parser = new EndpointParser(endpointRegistry);

            settings.ExtensionParsers.Add(EndpointList.ExtensionKey, parser.Parse);
            return settings;
        }
    }
}
