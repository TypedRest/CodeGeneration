using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// Generates the source code of a TypeScript TypedRest client.
/// </summary>
public class TypeScriptClientGenerator : IClientGenerator
{
    /// <summary>
    /// The name of this target language.
    /// </summary>
    public const string LanguageName = "typescript";

    /// <inheritdoc/>
    public string Language => LanguageName;

    /// <inheritdoc/>
    public ClientGenerationOptions CreateOptions(string serviceName)
        => new TypeScriptGenerationOptions(serviceName);

    /// <inheritdoc/>
    public IEnumerable<IGeneratedFile> Generate(OpenApiDocument document, ClientGenerationOptions options, IGenerationLog? log = null)
    {
        if (options.GenerateInterfaces) (log ?? NullGenerationLog.Instance).Report(Messages.InterfacesNotSupported());

        return document.GenerateTypedRestTypeScript(
            options as TypeScriptGenerationOptions ?? new TypeScriptGenerationOptions(options),
            log);
    }
}
