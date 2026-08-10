using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Patterns;
using TypedRest.CodeGeneration.TypeScript.Dtos;
using TypedRest.CodeGeneration.TypeScript.Endpoints;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// Generates TypeScript TypedRest clients for OpenAPI/Swagger documents.
/// </summary>
public static class OpenApiDocumentExtensions
{
    /// <summary>
    /// The name of the generated file re-exporting every other generated module.
    /// </summary>
    public const string IndexFileName = "index.ts";

    /// <summary>
    /// Generates the source files of a TypeScript TypedRest client for <paramref name="doc"/>.
    /// </summary>
    /// <param name="doc">The document describing the service.</param>
    /// <param name="options">Options controlling the generation.</param>
    /// <param name="log">Collects messages about aspects of the document that TypeScript cannot express.</param>
    /// <param name="patterns">Controls what is inferred when the document has no <c>x-typedrest</c> extension.</param>
    /// <param name="builders">Controls what code is emitted for each kind of endpoint.</param>
    public static IEnumerable<IGeneratedFile> GenerateTypedRestTypeScript(this OpenApiDocument doc, TypeScriptGenerationOptions options, IGenerationLog? log = null, PatternRegistry? patterns = null, BuilderRegistry? builders = null)
    {
        var naming = options.NamingStrategy();

        // Endpoints and DTOs may share an output directory, so they have to agree on the names they hand out
        var typeNames = new TypeNameRegistry();

        var types = doc.GenerateTypedRestTypeScriptEndpoints(naming, options.Modules(), log, patterns, builders, typeNames).ToList();
        if (options.GenerateDtos)
            types.AddRange(doc.GenerateTypeScriptDtos(naming, typeNames));

        return ToFiles(types, options.GenerateIndex);
    }

    /// <summary>
    /// Generates the types of a TypeScript TypedRest client for <paramref name="doc"/>, without the DTOs.
    /// </summary>
    public static IEnumerable<ITsType> GenerateTypedRestTypeScriptEndpoints(this OpenApiDocument doc, INamingStrategy naming, Modules? modules = null, IGenerationLog? log = null, PatternRegistry? patterns = null, BuilderRegistry? builders = null, TypeNameRegistry? typeNames = null)
    {
        var generator = new EndpointGenerator(naming, builders ?? BuilderRegistry.Default, typeNames)
        {
            Modules = modules ?? Modules.Default,
            Log = log ?? NullGenerationLog.Instance
        };
        var entryEndpoint = doc.GetTypedRest() ?? doc.MatchTypedRestPatterns(patterns);
        return generator.Generate(entryEndpoint);
    }

    /// <summary>
    /// Generates TypeScript types for the schemas in <paramref name="doc"/>.
    /// </summary>
    public static IEnumerable<ITsType> GenerateTypeScriptDtos(this OpenApiDocument doc, INamingStrategy naming, TypeNameRegistry? typeNames = null)
        => new DtoGenerator(naming, typeNames).Generate(doc.Components?.Schemas ?? new Dictionary<string, OpenApiSchema>());

    /// <summary>
    /// Groups types into one file per module, optionally followed by an <c>index.ts</c> re-exporting them all.
    /// </summary>
    public static IEnumerable<IGeneratedFile> ToFiles(IEnumerable<ITsType> types, bool generateIndex = true)
    {
        var modules = new List<TsModule>();
        var files = new Dictionary<string, TsFile>(StringComparer.Ordinal);

        foreach (var type in types)
        {
            var module = type.Identifier.Module
                      ?? throw new InvalidOperationException($"Generated type '{type.Identifier.Name}' has no module.");

            if (!files.TryGetValue(module.Specifier, out var file))
            {
                files[module.Specifier] = file = new TsFile(module.FilePath);
                modules.Add(module);
            }

            file.Types.Add(type);
        }

        foreach (var module in modules)
            yield return files[module.Specifier];

        if (generateIndex && modules.Count != 0)
            yield return new TsBarrelFile(IndexFileName, modules);
    }
}
