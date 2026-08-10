using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// Options controlling the generation of a TypeScript TypedRest client.
/// </summary>
public class TypeScriptGenerationOptions : ClientGenerationOptions
{
    /// <summary>
    /// Creates new generation options.
    /// </summary>
    /// <param name="serviceName">The service name to use for the entry endpoint.</param>
    public TypeScriptGenerationOptions(string serviceName)
        : base(serviceName)
    {}

    /// <summary>
    /// Creates new generation options, copying the common options from <paramref name="other"/>.
    /// </summary>
    public TypeScriptGenerationOptions(ClientGenerationOptions other)
        : base(other)
    {}

    /// <summary>
    /// The default directory DTO modules are written to, relative to the output directory.
    /// </summary>
    public const string DefaultDtoDirectory = "dtos";

    /// <summary>
    /// The name of the npm package providing TypedRest.
    /// </summary>
    public string TypedRestPackage { get; set; } = TypeScript.Modules.DefaultPackage;

    /// <summary>
    /// Controls whether to generate an <c>index.ts</c> re-exporting every generated module.
    /// </summary>
    public bool GenerateIndex { get; set; } = true;

    /// <summary>
    /// The type to use for schemas that carry no usable type information. Defaults to <c>unknown</c>.
    /// </summary>
    public TsIdentifier UntypedFallback { get; set; } = TsIdentifier.Unknown;

    /// <summary>
    /// The modules of the TypedRest runtime library to import from.
    /// </summary>
    public Modules Modules()
        => TypedRestPackage == TypeScript.Modules.DefaultPackage
            ? TypeScript.Modules.Default
            : new Modules(TypedRestPackage);

    /// <summary>
    /// Builds a <see cref="TypeScript.NamingStrategy"/> applying the directory fallbacks.
    /// </summary>
    /// <remarks>
    /// Unlike the C# generator, DTOs default to their own subdirectory rather than to the endpoint namespace,
    /// because every generated type gets a file of its own.
    /// </remarks>
    public NamingStrategy NamingStrategy()
        => new(ServiceName, Namespace ?? "", DtoNamespace ?? DefaultDtoDirectory, UntypedFallback);
}
