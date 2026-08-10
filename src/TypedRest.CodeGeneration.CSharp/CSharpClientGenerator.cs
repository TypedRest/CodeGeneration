using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Generates the source code of a C# TypedRest client.
/// </summary>
public class CSharpClientGenerator : IClientGenerator
{
    /// <summary>
    /// The name of this target language.
    /// </summary>
    public const string LanguageName = "csharp";

    /// <inheritdoc/>
    public string Language => LanguageName;

    /// <inheritdoc/>
    public ClientGenerationOptions CreateOptions(string serviceName)
        => new GenerationOptions(serviceName);

    /// <inheritdoc/>
    public IEnumerable<IGeneratedFile> Generate(OpenApiDocument document, ClientGenerationOptions options, IGenerationLog? log = null)
        => document.GenerateTypedRest(options as GenerationOptions ?? new GenerationOptions(options))
                   .Select(type => (IGeneratedFile)new CSharpGeneratedFile(type));
}
