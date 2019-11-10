using System;
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
        public static OpenApiReaderSettings AddTypedRest(this OpenApiReaderSettings settings, Action<IEndpointsParserSetup>? setup = null)
        {
            var parser = new EndpointsParser();
            setup?.Invoke(parser);

            settings.ExtensionParsers.Add(EndpointList.ExtensionKey, parser.Parse);
            return settings;
        }
    }
}
