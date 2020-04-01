using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.CSharp.Builders;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Patterns;

namespace TypedRest.CodeGeneration.CSharp
{
    public static class OpenApiDocumentExtensions
    {
        public static IEnumerable<ICSharpType> GenerateTypedRestEndpoints(this OpenApiDocument doc, INamingStrategy naming, bool withInterfaces = true, PatternRegistry? patterns = null, BuilderRegistry? builders = null)
        {
            var generator = new EndpointGenerator(naming, builders ?? BuilderRegistry.Default)
            {
                WithInterfaces = withInterfaces
            };
            var entryEndpoint = doc.GetTypedRest() ?? doc.MatchTypedRestPatterns(patterns);
            return generator.Generate(entryEndpoint);
        }

        public static IEnumerable<ICSharpType> GenerateDtos(this OpenApiDocument doc, INamingStrategy naming)
        {
            var generator = new DtoGenerator(naming);
            return generator.Generate(doc.Components.Schemas);
        }
    }
}
