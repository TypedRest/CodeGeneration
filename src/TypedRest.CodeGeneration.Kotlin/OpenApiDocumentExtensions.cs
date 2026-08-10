using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm;
using TypedRest.CodeGeneration.Jvm.Dtos;
using TypedRest.CodeGeneration.Jvm.Endpoints;
using TypedRest.CodeGeneration.Jvm.Model;
using TypedRest.CodeGeneration.Patterns;

namespace TypedRest.CodeGeneration.Kotlin;

/// <summary>
/// Generates Kotlin TypedRest clients for OpenAPI/Swagger documents.
/// </summary>
public static class OpenApiDocumentExtensions
{
    /// <summary>
    /// Generates the source files of a Kotlin TypedRest client for <paramref name="doc"/>.
    /// </summary>
    /// <param name="doc">The document describing the service.</param>
    /// <param name="options">Options controlling the generation.</param>
    /// <param name="log">Collects messages about aspects of the document that Kotlin cannot express.</param>
    /// <param name="patterns">Controls what is inferred when the document has no <c>x-typedrest</c> extension.</param>
    /// <param name="builders">Controls what code is emitted for each kind of endpoint.</param>
    public static IEnumerable<IGeneratedFile> GenerateTypedRestKotlin(this OpenApiDocument doc, KotlinGenerationOptions options, IGenerationLog? log = null, PatternRegistry? patterns = null, BuilderRegistry? builders = null)
    {
        var naming = options.NamingStrategy();

        // Endpoints and DTOs may share a package, so they have to agree on the names they hand out
        var typeNames = new TypeNameRegistry();

        var types = doc.GenerateTypedRestKotlinEndpoints(naming, options.GenerateInterfaces, log, patterns, builders, typeNames).ToList();
        if (options.GenerateDtos)
            types.AddRange(doc.GenerateKotlinDtos(naming, typeNames));

        var writer = new KotlinWriter(options.ResolveSerializer(), options.GenerateEntryConstructor);
        return types.Select(type => (IGeneratedFile)new KotlinGeneratedFile(type, writer));
    }

    /// <summary>
    /// Generates the endpoint types of a Kotlin TypedRest client for <paramref name="doc"/>, without the DTOs.
    /// </summary>
    public static IEnumerable<IJvmType> GenerateTypedRestKotlinEndpoints(this OpenApiDocument doc, INamingStrategy naming, bool withInterfaces = false, IGenerationLog? log = null, PatternRegistry? patterns = null, BuilderRegistry? builders = null, TypeNameRegistry? typeNames = null)
    {
        var generator = new EndpointGenerator(naming, builders ?? BuilderRegistry.Default, typeNames)
        {
            Log = log ?? NullGenerationLog.Instance,
            WithInterfaces = withInterfaces
        };
        var entryEndpoint = doc.GetTypedRest() ?? doc.MatchTypedRestPatterns(patterns);
        return generator.Generate(entryEndpoint);
    }

    /// <summary>
    /// Generates Kotlin types for the schemas in <paramref name="doc"/>.
    /// </summary>
    public static IEnumerable<IJvmType> GenerateKotlinDtos(this OpenApiDocument doc, INamingStrategy naming, TypeNameRegistry? typeNames = null)
        => new DtoGenerator(naming, typeNames).Generate(doc.Components?.Schemas ?? new Dictionary<string, OpenApiSchema>());
}
