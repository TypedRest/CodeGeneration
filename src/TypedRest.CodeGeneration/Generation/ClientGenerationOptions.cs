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
}
