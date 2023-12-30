using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoEnumBuilder(string key, OpenApiSchema schema, INamingStrategy naming)
    : DtoBuilder(key, schema, naming)
{
    protected override ICSharpType BuildTypeInner()
    {
        var type = new CSharpEnum(Identifier);
        type.Values.AddRange(Schema.Enum.OfType<OpenApiString>().Select(x => new CSharpEnumValue(Naming.Property(x.Value))
        {
            Attributes = {Attributes.EnumMember(x.Value)}
        }));
        return type;
    }
}
