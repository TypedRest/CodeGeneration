using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="OpenApiReaderSettings"/>s.
/// </summary>
public static class OpenApiReaderSettingsExtensions
{
    /// <summary>
    /// Registers the TypedRest OpenAPI extension parser.
    /// </summary>
    /// <param name="settings">The settings to register the extension parser in.</param>
    /// <param name="endpointRegistry">A list of all known <see cref="IEndpoint"/> kinds; leave <c>null</c> for default.</param>
    /// <seealso cref="OpenApiDocumentExtensions.GetTypedRest"/>
    public static OpenApiReaderSettings AddTypedRest(this OpenApiReaderSettings settings, EndpointRegistry? endpointRegistry = null)
    {
        var parser = new EndpointParser(endpointRegistry ?? EndpointRegistry.Default);

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
