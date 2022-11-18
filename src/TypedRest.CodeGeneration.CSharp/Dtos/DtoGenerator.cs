using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoGenerator
{
    private readonly INamingStrategy _naming;

    public DtoGenerator(INamingStrategy naming)
    {
        _naming = naming;
    }

    public IEnumerable<ICSharpType> Generate(IEnumerable<KeyValuePair<string, OpenApiSchema>> schemas)
    {
        foreach ((string key, var schema) in schemas)
        {
            if (DtoBuilder.For(key, schema, _naming) is {} builder)
            {
                foreach (var type in builder.BuildTypes())
                    yield return type;
            }
        }
    }
}
