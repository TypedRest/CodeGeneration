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
    /// <remarks>
    /// Not kotlinx.serialization, which every other part of TypedRest for the JVM defaults to: it generates its
    /// serializers with a Kotlin compiler plugin and cannot handle a class written in Java. A Java client therefore
    /// has to pass its serializer to the entry endpoint explicitly.
    /// </remarks>
    protected override string DefaultSerializerName => JvmSerializer.Jackson;

    /// <inheritdoc/>
    public override IReadOnlyCollection<string> SupportedSerializers
        => [JvmSerializer.Jackson, JvmSerializer.Moshi];

    /// <summary>
    /// Controls whether nullable properties are annotated with JSpecify's <c>@Nullable</c>.
    /// </summary>
    /// <remarks>
    /// On by default. Without it Kotlin sees every generated type as a platform type and loses null safety across
    /// the whole DTO surface, which defeats much of the point of consuming a Java client from Kotlin.
    /// </remarks>
    public bool NullableAnnotations { get; set; } = true;
}
