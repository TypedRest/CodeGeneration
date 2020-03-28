using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Builders;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Patterns;

namespace TypedRest.OpenApi.CSharp
{
    public static class OpenApiDocumentExtensions
    {
        public static IEnumerable<ICSharpType> GenerateTypedRestEndpoints(this OpenApiDocument doc, INamingStrategy naming, bool withInterfaces = true, BuilderRegistry? builders = null)
        {
            var generator = new EndpointGenerator(naming, builders ?? BuilderRegistry.Default)
            {
                WithInterfaces = withInterfaces
            };
            var entryEndpoint = doc.GetTypedRest() ?? new PatternMatcher().GetEntryEndpoint(doc);
            return generator.Generate(entryEndpoint);
        }

        public static IEnumerable<ICSharpType> GenerateDtos(this OpenApiDocument doc, INamingStrategy naming)
        {
            var generator = new DtoGenerator(naming);
            return generator.Generate(doc.Components.Schemas);
        }
    }
}
