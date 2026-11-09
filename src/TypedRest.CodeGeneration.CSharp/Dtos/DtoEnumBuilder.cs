using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoEnumBuilder(string key, IOpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest, TypeNameRegistry? typeNames = null, JsonAttributes? jsonAttributes = null)
    : DtoBuilder(key, schema, naming, languageVersion, typeNames, jsonAttributes)
{
    protected override ICSharpType BuildTypeInner()
    {
        var type = new CSharpEnum(Identifier);
        var usedNames = new HashSet<string>();

        foreach (var value in Schema.Enum ?? [])
        {
            if (value.AsString() is {} str)
            {
                type.Values.Add(new CSharpEnumValue(UniqueName(usedNames, Naming.Property(str)))
                {
                    Attributes = {Json.EnumMemberName(str)}
                });
            }
            else if (value.AsInteger() is >= int.MinValue and <= int.MaxValue and var num)
                type.Values.Add(new CSharpEnumValue(UniqueName(usedNames, NumericName((int)num))) {Value = (int)num});
        }

        if (Schema.Enum?.Any(x => x.AsString() != null) == true && Json.StringEnumConverter(Identifier) is {} converter)
            type.Attributes.Add(converter);

        return type;
    }

    /// <summary>
    /// Builds a name for a numeric value, avoiding the minus sign which may not appear in a C# identifier.
    /// </summary>
    private static string NumericName(int value)
        => value < 0
            ? "ValueMinus" + -(long)value
            : "Value" + value;

    /// <summary>
    /// Ensures the <paramref name="name"/> is usable as a C# identifier and unique within the enum.
    /// </summary>
    private static string UniqueName(HashSet<string> usedNames, string name)
    {
        // Schemas may contain an empty string as an enum value
        if (name.Length == 0) name = "Empty";

        string result = name;
        for (int i = 2; !usedNames.Add(result); i++)
            result = name + i;
        return result;
    }
}
