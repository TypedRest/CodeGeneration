using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm;

namespace TypedRest.CodeGeneration.Java;

/// <summary>
/// Generates the source code of a Java TypedRest client.
/// </summary>
public class JavaClientGenerator : IClientGenerator
{
    /// <summary>
    /// The name of this target language.
    /// </summary>
    public const string LanguageName = "java";

    /// <inheritdoc/>
    public string Language => LanguageName;

    /// <inheritdoc/>
    public ClientGenerationOptions CreateOptions(string serviceName)
        => new JavaGenerationOptions(serviceName);

    /// <inheritdoc/>
    public IEnumerable<IGeneratedFile> Generate(OpenApiDocument document, ClientGenerationOptions options, IGenerationLog? log = null)
    {
        var javaOptions = options as JavaGenerationOptions ?? new JavaGenerationOptions(options);
        var generationLog = log ?? NullGenerationLog.Instance;

        if (options.GenerateInterfaces) generationLog.Report(Messages.InterfacesNotSupported());

        // kotlinx.serialization needs a Kotlin compiler plugin, so it can never serialize a Java DTO. Falling back
        // silently would generate a client that compiles and then fails to deserialize anything at runtime.
        if (javaOptions.Serializer is {} serializer && !JvmSerializer.For(serializer).SupportsJava)
            throw new ArgumentException(Messages.SerializerNotSupportedByJava(serializer).Text, nameof(options));

        return document.GenerateTypedRestJava(javaOptions, log);
    }
}
