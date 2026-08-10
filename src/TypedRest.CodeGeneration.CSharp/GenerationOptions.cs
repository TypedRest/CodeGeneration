using Microsoft.CodeAnalysis.CSharp;
using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Options controlling the generation of a C# TypedRest client.
/// </summary>
public class GenerationOptions : ClientGenerationOptions
{
    /// <summary>
    /// Creates new generation options.
    /// </summary>
    /// <param name="serviceName">The service name to use for the entry endpoint.</param>
    public GenerationOptions(string serviceName)
        : base(serviceName)
    {}

    /// <summary>
    /// Creates new generation options, copying the common options from <paramref name="other"/>.
    /// </summary>
    public GenerationOptions(ClientGenerationOptions other)
        : base(other)
    {}

    /// <summary>
    /// The minimum C# version the generated code must compile with.
    /// </summary>
    public LanguageVersion LanguageVersion { get; set; } = LanguageVersion.Latest;

    /// <summary>
    /// Builds a <see cref="CSharp.NamingStrategy"/> applying the namespace fallbacks.
    /// </summary>
    public NamingStrategy NamingStrategy()
        => new(ServiceName, Namespace ?? ServiceName, DtoNamespace ?? Namespace ?? ServiceName);
}
