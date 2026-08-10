namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// Generates the source code of a TypedRest client in a specific target language.
/// </summary>
public interface IClientGenerator
{
    /// <summary>
    /// The canonical name of the target language, e.g. <c>csharp</c>.
    /// </summary>
    string Language { get; }

    /// <summary>
    /// Creates an options object for this target language with its defaults applied.
    /// </summary>
    /// <param name="serviceName">The service name to use for the entry endpoint.</param>
    ClientGenerationOptions CreateOptions(string serviceName);

    /// <summary>
    /// Generates the source files of a client for <paramref name="document"/>.
    /// </summary>
    /// <param name="document">The OpenAPI/Swagger document to generate a client for.</param>
    /// <param name="options">Options controlling the generation. May be an instance created by <see cref="CreateOptions"/>.</param>
    /// <param name="log">Collects messages about aspects of the document that the target language cannot express.</param>
    IEnumerable<IGeneratedFile> Generate(OpenApiDocument document, ClientGenerationOptions options, IGenerationLog? log = null);
}
