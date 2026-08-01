using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoGenerator(INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest)
{
    public IEnumerable<ICSharpType> Generate(IEnumerable<KeyValuePair<string, OpenApiSchema>> schemas)
    {
        foreach ((string key, var schema) in schemas)
        {
            if (DtoBuilder.For(key, schema, naming, languageVersion) is {} builder)
            {
                foreach (var type in builder.BuildTypes())
                    yield return type;
            }
        }
    }
}
