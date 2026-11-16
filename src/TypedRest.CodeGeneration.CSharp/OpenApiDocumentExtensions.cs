using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.CSharp.Dtos;
using TypedRest.CodeGeneration.CSharp.Endpoints;
using TypedRest.CodeGeneration.Patterns;

namespace TypedRest.CodeGeneration.CSharp;

public static class OpenApiDocumentExtensions
{
    extension(OpenApiDocument doc)
    {
        public IEnumerable<ICSharpType> GenerateTypedRest(GenerationOptions options, PatternRegistry? patterns = null, BuilderRegistry? builders = null)
        {
            var naming = options.NamingStrategy();
            var jsonAttributes = options.JsonAttributes();
            var types = doc.GenerateTypedRestEndpoints(naming, options.GenerateInterfaces, options.GenerateEntryConstructor, patterns, builders, jsonAttributes);
            return options.GenerateDtos
                ? types.Concat(doc.GenerateDtos(naming, options.LanguageVersion, jsonAttributes))
                : types;
        }

        public IEnumerable<ICSharpType> GenerateTypedRestEndpoints(INamingStrategy naming, bool withInterfaces = true, bool generateEntryConstructor = true, PatternRegistry? patterns = null, BuilderRegistry? builders = null, JsonAttributes? jsonAttributes = null)
        {
            var generator = new EndpointGenerator(naming, builders ?? BuilderRegistry.Default)
            {
                WithInterfaces = withInterfaces,
                GenerateEntryConstructor = generateEntryConstructor,
                RuntimeSerializer = jsonAttributes?.RuntimeSerializer
            };
            var entryEndpoint = doc.GetTypedRest() ?? doc.MatchTypedRestPatterns(patterns);
            return generator.Generate(entryEndpoint);
        }

        public IEnumerable<ICSharpType> GenerateDtos(INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest, JsonAttributes? jsonAttributes = null)
        {
            var generator = new DtoGenerator(naming, languageVersion, jsonAttributes);
            return generator.Generate(doc.Components?.Schemas ?? new Dictionary<string, IOpenApiSchema>());
        }
    }
}
