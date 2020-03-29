using System;
using Microsoft.OpenApi.Any;
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
        /// Registers a TypedRest endpoint <paramref name="parser"/> as an OpenAPI extension parser.
        /// </summary>
        /// <seealso cref="OpenApiDocumentExtensions.GetTypedRest"/>
        public static OpenApiReaderSettings AddTypedRest(this OpenApiReaderSettings settings, IEndpointParser parser)
        {
            settings.ExtensionParsers.Add(OpenApiDocumentExtensions.TypedRestKey, (data, _) =>
            {
                if (!(data is OpenApiObject objData)) throw new FormatException($"{OpenApiDocumentExtensions.TypedRestKey} is not an object.");

                var entryEndpoint = new EntryEndpoint();
                entryEndpoint.Parse(objData, parser);
                return entryEndpoint;
            });
            return settings;
        }
    }
}
