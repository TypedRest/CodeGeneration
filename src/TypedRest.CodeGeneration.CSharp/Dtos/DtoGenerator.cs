using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoGenerator(INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest, JsonAttributes? jsonAttributes = null)
{
    public IEnumerable<ICSharpType> Generate(IEnumerable<KeyValuePair<string, OpenApiSchema>> schemas)
    {
        var typeNames = new TypeNameRegistry();

        // Create all builders first, so that types from the document claim their names before the types
        // generated for inline schemas, which get a number appended if their name is already taken
        var builders = schemas.Select(x => DtoBuilder.For(x.Key, x.Value, naming, languageVersion, typeNames, jsonAttributes))
                              .OfType<DtoBuilder>()
                              .ToList();

        foreach (var builder in builders)
        {
            foreach (var type in builder.BuildTypes())
                yield return type;
        }
    }
}
