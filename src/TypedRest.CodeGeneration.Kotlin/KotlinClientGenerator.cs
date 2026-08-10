using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm;

namespace TypedRest.CodeGeneration.Kotlin;

/// <summary>
/// Generates the source code of a Kotlin TypedRest client.
/// </summary>
public class KotlinClientGenerator : IClientGenerator
{
    /// <summary>
    /// The name of this target language.
    /// </summary>
    public const string LanguageName = "kotlin";

    /// <inheritdoc/>
    public string Language => LanguageName;

    /// <inheritdoc/>
    public ClientGenerationOptions CreateOptions(string serviceName)
        => new KotlinGenerationOptions(serviceName);

    /// <inheritdoc/>
    public IEnumerable<IGeneratedFile> Generate(OpenApiDocument document, ClientGenerationOptions options, IGenerationLog? log = null)
    {
        if (options.GenerateInterfaces) (log ?? NullGenerationLog.Instance).Report(Messages.InterfacesNotSupported());

        return document.GenerateTypedRestKotlin(
            options as KotlinGenerationOptions ?? new KotlinGenerationOptions(options),
            log);
    }
}
