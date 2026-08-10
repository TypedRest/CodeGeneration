using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm;

namespace TypedRest.CodeGeneration.Java;

/// <summary>
/// Options controlling the generation of a Java TypedRest client.
/// </summary>
public class JavaGenerationOptions : JvmGenerationOptions
{
    /// <summary>
    /// Creates new generation options.
    /// </summary>
    /// <param name="serviceName">The service name to use for the entry endpoint.</param>
    public JavaGenerationOptions(string serviceName)
        : base(serviceName)
    {}

    /// <summary>
    /// Creates new generation options, copying the common options from <paramref name="other"/>.
    /// </summary>
    public JavaGenerationOptions(ClientGenerationOptions other)
        : base(other)
    {}

    /// <inheritdoc/>
    protected override string DefaultSerializerName => JvmSerializer.Jackson;

    /// <inheritdoc/>
    public override IReadOnlyCollection<string> SupportedSerializers
        => [JvmSerializer.Jackson, JvmSerializer.Moshi];

    /// <summary>
    /// Controls whether nullable properties are annotated with JSpecify's <c>@Nullable</c>.
    /// </summary>
    public bool NullableAnnotations { get; set; } = true;
}
