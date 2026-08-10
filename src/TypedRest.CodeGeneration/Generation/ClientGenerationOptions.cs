namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// Options controlling the generation of a TypedRest client, common to all target languages.
/// </summary>
/// <remarks>Target languages derive from this to add their own options.</remarks>
public class ClientGenerationOptions
{
    /// <summary>
    /// Creates new generation options.
    /// </summary>
    /// <param name="serviceName">The service name to use for the entry endpoint.</param>
    public ClientGenerationOptions(string serviceName)
    {
        ServiceName = serviceName;
    }

    /// <summary>
    /// Copies the common options from <paramref name="other"/>.
    /// </summary>
    /// <remarks>Used by target languages to accept options that were built without knowledge of the language.</remarks>
    protected ClientGenerationOptions(ClientGenerationOptions other)
    {
        ServiceName = other.ServiceName;
        Namespace = other.Namespace;
        DtoNamespace = other.DtoNamespace;
        GenerateInterfaces = other.GenerateInterfaces;
        GenerateDtos = other.GenerateDtos;
        GenerateEntryConstructor = other.GenerateEntryConstructor;
        Serializer = other.Serializer;
    }

    /// <summary>
    /// The service name to use for the entry endpoint.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// The namespace or module path for the endpoints. Uses <see cref="ServiceName"/> if not set.
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// The namespace or module path for the DTOs. Uses <see cref="Namespace"/> if not set.
    /// </summary>
    public string? DtoNamespace { get; set; }

    /// <summary>
    /// Controls whether to generate interfaces for endpoints. Not supported by all target languages.
    /// </summary>
    public bool GenerateInterfaces { get; set; }

    /// <summary>
    /// Controls whether to generate DTOs.
    /// </summary>
    public bool GenerateDtos { get; set; }

    /// <summary>
    /// Controls whether the entry endpoint gets a generated constructor taking the base URI.
    /// Turn this off to supply the constructors yourself, e.g. to pass an error handler or custom headers.
    /// Not supported by all target languages.
    /// </summary>
    public bool GenerateEntryConstructor { get; set; } = true;

    /// <summary>
    /// The JSON serializer the generated DTOs are annotated for, or <c>null</c> to use the target language's default.
    /// </summary>
    /// <remarks>
    /// The serializer decides which attributes/annotations carry the wire names of properties, so choosing one that
    /// does not match the serializer configured on the endpoint at runtime silently changes the wire format.
    /// See <see cref="SupportedSerializers"/> for the values a target language accepts.
    /// </remarks>
    public string? Serializer { get; set; }

    /// <summary>
    /// The names accepted by <see cref="Serializer"/>, most preferred first. Empty if the target language has no
    /// serializer to choose, either because it does not annotate DTOs or because it only supports one.
    /// </summary>
    /// <remarks>Target languages override this to declare what they can generate for.</remarks>
    public virtual IReadOnlyCollection<string> SupportedSerializers => [];
}
