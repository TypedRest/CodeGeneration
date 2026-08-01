using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoEnumBuilder(string key, OpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest)
    : DtoBuilder(key, schema, naming, languageVersion)
{
    protected override ICSharpType BuildTypeInner()
    {
        var type = new CSharpEnum(Identifier);
        var usedNames = new HashSet<string>();

        foreach (var value in Schema.Enum)
        {
            switch (value)
            {
                case OpenApiString str:
                    type.Values.Add(new CSharpEnumValue(UniqueName(usedNames, Naming.Property(str.Value)))
                    {
                        Attributes = {Attributes.EnumMember(str.Value)}
                    });
                    break;
                case OpenApiInteger num:
                    type.Values.Add(new CSharpEnumValue(UniqueName(usedNames, NumericName(num.Value))) {Value = num.Value});
                    break;
                case OpenApiLong num when num.Value is >= int.MinValue and <= int.MaxValue:
                    type.Values.Add(new CSharpEnumValue(UniqueName(usedNames, NumericName((int)num.Value))) {Value = (int)num.Value});
                    break;
            }
        }

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
