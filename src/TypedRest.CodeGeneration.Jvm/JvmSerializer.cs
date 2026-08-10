using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm;

/// <summary>
/// Supplies the annotations that carry wire names on generated DTOs, for one specific JSON serializer.
/// </summary>
public abstract class JvmSerializer
{
    /// <summary>
    /// kotlinx.serialization, the default of TypedRest for the JVM. Kotlin only.
    /// </summary>
    public const string Kotlinx = "kotlinx";

    /// <summary>
    /// Jackson, from the <c>typedrest-serializers-jackson</c> artifact.
    /// </summary>
    public const string Jackson = "jackson";

    /// <summary>
    /// Moshi, from the <c>typedrest-serializers-moshi</c> artifact.
    /// </summary>
    public const string Moshi = "moshi";

    /// <summary>
    /// Returns the serializer with <paramref name="name"/>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="name"/> is not a known serializer.</exception>
    public static JvmSerializer For(string name)
        => name switch
        {
            Kotlinx => new KotlinxSerializer(),
            Jackson => new JacksonSerializer(),
            Moshi => new MoshiSerializer(),
            _ => throw new ArgumentException($"Unknown serializer '{name}'. Expected one of: {Kotlinx}, {Jackson}, {Moshi}.", nameof(name))
        };

    /// <summary>
    /// The name of this serializer.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Indicates whether this serializer can handle DTOs written in Java.
    /// </summary>
    public abstract bool SupportsJava { get; }

    /// <summary>
    /// The runtime <c>Serializer</c> the entry endpoint has to be constructed with, or <c>null</c> if it is
    /// already the default of <c>EntryEndpoint</c> and can be left out.
    /// </summary>
    public abstract JvmIdentifier? RuntimeSerializer { get; }

    /// <summary>
    /// The Maven coordinates of the artifact providing <see cref="RuntimeSerializer"/>, for the generated README.
    /// </summary>
    public abstract string Artifact { get; }

    /// <summary>
    /// Annotations that go on a generated DTO class itself.
    /// </summary>
    public virtual IEnumerable<JvmAnnotation> TypeAnnotations() => [];

    /// <summary>
    /// Annotations that go on a generated enum.
    /// </summary>
    public virtual IEnumerable<JvmAnnotation> EnumAnnotations() => [];

    /// <summary>
    /// The annotation carrying the wire name of a property, or <c>null</c> if the name needs none.
    /// </summary>
    public abstract JvmAnnotation? PropertyName(string wireName);

    /// <summary>
    /// The annotation carrying the wire name of an enum value, or <c>null</c> if the name needs none.
    /// </summary>
    public abstract JvmAnnotation? EnumMemberName(string wireName);
}

/// <summary>
/// Annotates DTOs for kotlinx.serialization.
/// </summary>
/// <remarks>
/// Requires the <c>kotlin-serialization</c> Gradle plugin in the consuming project.
/// </remarks>
public sealed class KotlinxSerializer : JvmSerializer
{
    private static readonly JvmPackage _package = JvmPackage.External("kotlinx.serialization");

    /// <inheritdoc/>
    public override string Name => Kotlinx;

    /// <inheritdoc/>
    public override bool SupportsJava => false;

    /// <inheritdoc/>
    public override JvmIdentifier? RuntimeSerializer => null;

    /// <inheritdoc/>
    public override string Artifact => "net.typedrest:typedrest";

    /// <inheritdoc/>
    public override IEnumerable<JvmAnnotation> TypeAnnotations()
        => [new(new JvmIdentifier(_package, "Serializable"))];

    /// <inheritdoc/>
    public override IEnumerable<JvmAnnotation> EnumAnnotations()
        => [new(new JvmIdentifier(_package, "Serializable"))];

    /// <inheritdoc/>
    public override JvmAnnotation? PropertyName(string wireName)
        => new JvmAnnotation(new JvmIdentifier(_package, "SerialName")) {Arguments = {wireName}};

    /// <inheritdoc/>
    public override JvmAnnotation? EnumMemberName(string wireName)
        => new JvmAnnotation(new JvmIdentifier(_package, "SerialName")) {Arguments = {wireName}};
}

/// <summary>
/// Annotates DTOs for Jackson.
/// </summary>
public sealed class JacksonSerializer : JvmSerializer
{
    private static readonly JvmPackage _package = JvmPackage.External("com.fasterxml.jackson.annotation");

    /// <inheritdoc/>
    public override string Name => Jackson;

    /// <inheritdoc/>
    public override bool SupportsJava => true;

    /// <inheritdoc/>
    public override JvmIdentifier? RuntimeSerializer => new(Packages.Serializers, "JacksonJsonSerializer");

    /// <inheritdoc/>
    public override string Artifact => "net.typedrest:typedrest-serializers-jackson";

    /// <inheritdoc/>
    public override JvmAnnotation? PropertyName(string wireName)
        => new JvmAnnotation(new JvmIdentifier(_package, "JsonProperty")) {Arguments = {wireName}};

    /// <inheritdoc/>
    public override JvmAnnotation? EnumMemberName(string wireName)
        => new JvmAnnotation(new JvmIdentifier(_package, "JsonProperty")) {Arguments = {wireName}};
}

/// <summary>
/// Annotates DTOs for Moshi.
/// </summary>
public sealed class MoshiSerializer : JvmSerializer
{
    private static readonly JvmPackage _package = JvmPackage.External("com.squareup.moshi");

    /// <inheritdoc/>
    public override string Name => Moshi;

    /// <inheritdoc/>
    public override bool SupportsJava => true;

    /// <inheritdoc/>
    public override JvmIdentifier? RuntimeSerializer => new(Packages.Serializers, "MoshiJsonSerializer");

    /// <inheritdoc/>
    public override string Artifact => "net.typedrest:typedrest-serializers-moshi";

    /// <inheritdoc/>
    public override IEnumerable<JvmAnnotation> TypeAnnotations()
        => [new JvmAnnotation(new JvmIdentifier(_package, "JsonClass")) {NamedArguments = {("generateAdapter", "true")}}];

    /// <inheritdoc/>
    public override JvmAnnotation? PropertyName(string wireName)
        => new JvmAnnotation(new JvmIdentifier(_package, "Json")) {NamedArguments = {("name", JvmSyntax.Quote(wireName))}};

    /// <inheritdoc/>
    public override JvmAnnotation? EnumMemberName(string wireName)
        => new JvmAnnotation(new JvmIdentifier(_package, "Json")) {NamedArguments = {("name", JvmSyntax.Quote(wireName))}};
}
