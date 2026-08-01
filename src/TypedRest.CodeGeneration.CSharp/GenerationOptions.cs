using Microsoft.CodeAnalysis.CSharp;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Options controlling the generation of a TypedRest client.
/// </summary>
/// <param name="serviceName">The service name to use for the entry endpoint.</param>
public class GenerationOptions(string serviceName)
{
    /// <summary>
    /// The service name to use for the entry endpoint.
    /// </summary>
    public string ServiceName { get; } = serviceName;

    /// <summary>
    /// The C# namespace for the endpoints. Uses <see cref="ServiceName"/> if not set.
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// The C# namespace for the DTOs. Uses <see cref="Namespace"/> if not set.
    /// </summary>
    public string? DtoNamespace { get; set; }

    /// <summary>
    /// Controls whether to generate interfaces for endpoints.
    /// </summary>
    public bool GenerateInterfaces { get; set; }

    /// <summary>
    /// Controls whether to generate DTOs.
    /// </summary>
    public bool GenerateDtos { get; set; }

    /// <summary>
    /// The minimum C# version the generated DTOs must compile with.
    /// </summary>
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

    /// <summary>
    /// Builds a <see cref="CSharp.NamingStrategy"/> applying the namespace fallbacks.
    /// </summary>
    public NamingStrategy NamingStrategy()
        => new(ServiceName, Namespace ?? ServiceName, DtoNamespace ?? Namespace ?? ServiceName);
}
