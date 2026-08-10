using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm;

namespace TypedRest.CodeGeneration.Kotlin;

/// <summary>
/// Options controlling the generation of a Kotlin TypedRest client.
/// </summary>
public class KotlinGenerationOptions : JvmGenerationOptions
{
    /// <summary>
    /// Creates new generation options.
    /// </summary>
    /// <param name="serviceName">The service name to use for the entry endpoint.</param>
    public KotlinGenerationOptions(string serviceName)
        : base(serviceName)
    {}

    /// <summary>
    /// Creates new generation options, copying the common options from <paramref name="other"/>.
    /// </summary>
    public KotlinGenerationOptions(ClientGenerationOptions other)
        : base(other)
    {}

    /// <inheritdoc/>
    /// <remarks>
    /// kotlinx.serialization is the default of <c>EntryEndpoint</c> itself, so a client generated for it needs no
    /// serializer passed at all.
    /// </remarks>
    protected override string DefaultSerializerName => JvmSerializer.Kotlinx;

    /// <inheritdoc/>
    public override IReadOnlyCollection<string> SupportedSerializers
        => [JvmSerializer.Kotlinx, JvmSerializer.Jackson, JvmSerializer.Moshi];
}
