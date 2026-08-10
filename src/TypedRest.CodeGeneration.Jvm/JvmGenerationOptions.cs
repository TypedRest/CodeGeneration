using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm;

/// <summary>
/// Options controlling the generation of a JVM TypedRest client, shared by the Java and Kotlin generators.
/// </summary>
public abstract class JvmGenerationOptions : ClientGenerationOptions
{
    /// <summary>
    /// Creates new generation options.
    /// </summary>
    /// <param name="serviceName">The service name to use for the entry endpoint.</param>
    protected JvmGenerationOptions(string serviceName)
        : base(serviceName)
    {}

    /// <summary>
    /// Creates new generation options, copying the common options from <paramref name="other"/>.
    /// </summary>
    protected JvmGenerationOptions(ClientGenerationOptions other)
        : base(other)
    {}

    /// <summary>
    /// The package name DTOs go into when <see cref="ClientGenerationOptions.DtoNamespace"/> is not set, relative
    /// to the endpoint package.
    /// </summary>
    public const string DefaultDtoSubPackage = "dtos";

    /// <summary>
    /// The type to use for schemas that carry no usable type information.
    /// </summary>
    public JvmIdentifier UntypedFallback { get; set; } = JvmIdentifier.Object;

    /// <summary>
    /// The name of the serializer this target language uses when none is chosen.
    /// </summary>
    protected abstract string DefaultSerializerName { get; }

    /// <summary>
    /// Resolves <see cref="ClientGenerationOptions.Serializer"/> to the serializer to generate for.
    /// </summary>
    /// <exception cref="ArgumentException">The serializer is not one of <see cref="ClientGenerationOptions.SupportedSerializers"/>.</exception>
    public JvmSerializer ResolveSerializer()
        => JvmSerializer.For(Serializer ?? DefaultSerializerName);

    /// <summary>
    /// The package the endpoints are generated into.
    /// </summary>
    public string EndpointPackage
        => JvmPackage.Sanitize(Namespace ?? ServiceName);

    /// <summary>
    /// The package the DTOs are generated into.
    /// </summary>
    /// <remarks>
    /// DTOs default to a subpackage of the endpoints rather than sharing their package, because a DTO and an endpoint generated from the same key would otherwise be able to collide.
    /// </remarks>
    public string DtoPackage
        => DtoNamespace is {Length: > 0} dtoNamespace
            ? JvmPackage.Sanitize(dtoNamespace)
            : Combine(EndpointPackage, DefaultDtoSubPackage);

    /// <summary>
    /// Builds a <see cref="Jvm.NamingStrategy"/> applying the package fallbacks.
    /// </summary>
    public NamingStrategy NamingStrategy()
        => new(ServiceName, EndpointPackage, DtoPackage, UntypedFallback);

    private static string Combine(string package, string subPackage)
        => package.Length == 0 ? subPackage : package + "." + subPackage;
}
