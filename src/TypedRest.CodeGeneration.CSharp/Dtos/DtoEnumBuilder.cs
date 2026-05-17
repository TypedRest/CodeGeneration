using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoEnumBuilder(string key, OpenApiSchema schema, INamingStrategy naming)
    : DtoBuilder(key, schema, naming)
{
    protected override ICSharpType BuildTypeInner()
    {
        var type = new CSharpEnum(Identifier);
        foreach (var value in Schema.Enum)
        {
            switch (value)
            {
                case OpenApiString str:
                    type.Values.Add(new CSharpEnumValue(Naming.Property(str.Value))
                    {
                        Attributes = {Attributes.EnumMember(str.Value)}
                    });
                    break;
                case OpenApiInteger num:
                    type.Values.Add(new CSharpEnumValue("Value" + num.Value) {Value = num.Value});
                    break;
                case OpenApiLong num when num.Value is >= int.MinValue and <= int.MaxValue:
                    type.Values.Add(new CSharpEnumValue("Value" + num.Value) {Value = (int)num.Value});
                    break;
            }
        }
        return type;
    }
}
