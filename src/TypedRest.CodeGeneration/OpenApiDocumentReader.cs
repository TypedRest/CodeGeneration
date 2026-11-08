using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration;

/// <summary>
/// Reads OpenAPI/Swagger documents (Swagger 2.0 and OpenAPI 3.0, 3.1 and 3.2; JSON or YAML) with support for the TypedRest extension.
/// </summary>
public static class OpenApiDocumentReader
{
    /// <summary>
    /// Parses an OpenAPI/Swagger document from a string.
    /// </summary>
    /// <param name="input">The JSON or YAML content of the document.</param>
    /// <param name="endpointRegistry">A list of all known <see cref="IEndpoint"/> kinds; leave <c>null</c> for default.</param>
    /// <returns>The parsed document (<c>null</c> if it could not be parsed at all) and any errors or warnings encountered.</returns>
    public static ReadResult Parse(string input, EndpointRegistry? endpointRegistry = null)
        => OpenApiDocument.Parse(input, InspectFormat(input), Settings(endpointRegistry));

    /// <summary>
    /// Reads an OpenAPI/Swagger document from a stream.
    /// </summary>
    /// <param name="stream">The stream containing the JSON or YAML content of the document.</param>
    /// <param name="endpointRegistry">A list of all known <see cref="IEndpoint"/> kinds; leave <c>null</c> for default.</param>
    /// <returns>The parsed document (<c>null</c> if it could not be parsed at all) and any errors or warnings encountered.</returns>
    public static ReadResult Read(Stream stream, EndpointRegistry? endpointRegistry = null)
    {
        using var reader = new StreamReader(stream);
        return Parse(reader.ReadToEnd(), endpointRegistry);
    }

    private static OpenApiReaderSettings Settings(EndpointRegistry? endpointRegistry)
        => new OpenApiReaderSettings().AddTypedRest(endpointRegistry);

    private static string InspectFormat(string input)
        => input.TrimStart().StartsWith("{") ? OpenApiConstants.Json : OpenApiConstants.Yaml;
}
