using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.CSharp.Dtos;
using TypedRest.CodeGeneration.CSharp.Endpoints;
using TypedRest.CodeGeneration.Patterns;

namespace TypedRest.CodeGeneration.CSharp;

public static class OpenApiDocumentExtensions
{
    public static IEnumerable<ICSharpType> GenerateTypedRest(this OpenApiDocument doc, GenerationOptions options, PatternRegistry? patterns = null, BuilderRegistry? builders = null)
    {
        var naming = options.NamingStrategy();
        var types = doc.GenerateTypedRestEndpoints(naming, options.GenerateInterfaces, options.GenerateEntryConstructor, patterns, builders);
        return options.GenerateDtos
            ? types.Concat(doc.GenerateDtos(naming, options.LanguageVersion))
            : types;
    }

    public static IEnumerable<ICSharpType> GenerateTypedRestEndpoints(this OpenApiDocument doc, INamingStrategy naming, bool withInterfaces = true, bool generateEntryConstructor = true, PatternRegistry? patterns = null, BuilderRegistry? builders = null)
    {
        var generator = new EndpointGenerator(naming, builders ?? BuilderRegistry.Default)
        {
            WithInterfaces = withInterfaces,
            GenerateEntryConstructor = generateEntryConstructor
        };
        var entryEndpoint = doc.GetTypedRest() ?? doc.MatchTypedRestPatterns(patterns);
        return generator.Generate(entryEndpoint);
    }

    public static IEnumerable<ICSharpType> GenerateDtos(this OpenApiDocument doc, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest)
    {
        var generator = new DtoGenerator(naming, languageVersion);
        return generator.Generate(doc.Components?.Schemas ?? new Dictionary<string, OpenApiSchema>());
    }
}
