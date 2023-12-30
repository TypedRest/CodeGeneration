using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public abstract class DtoBuilder(string key, OpenApiSchema schema, INamingStrategy naming)
{
    protected readonly CSharpIdentifier Identifier = naming.DtoType(key);
    protected readonly OpenApiSchema Schema = schema;
    protected readonly INamingStrategy Naming = naming;

    public static DtoBuilder? For(string key, OpenApiSchema schema, INamingStrategy naming)
        => (schema.Type ?? "object") switch
        {
            "object" => new DtoClassBuilder(key, schema, naming),
            "string" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming),
            _ => null
        };

    protected readonly List<ICSharpType> ChildTypes = new();

    public IEnumerable<ICSharpType> BuildTypes()
    {
        ChildTypes.Clear();
        yield return BuildType();

        foreach (var type in ChildTypes)
            yield return type;
    }

    internal ICSharpType BuildType()
    {
        var type = BuildTypeInner();
        type.Summary = Schema.Description;
        type.Attributes.Add(Attributes.GeneratedCode);
        return type;
    }

    protected abstract ICSharpType BuildTypeInner();
}
