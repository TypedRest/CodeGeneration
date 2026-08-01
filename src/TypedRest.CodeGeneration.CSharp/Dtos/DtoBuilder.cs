using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public abstract class DtoBuilder(string key, OpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest)
{
    protected readonly CSharpIdentifier Identifier = naming.DtoType(key);
    protected readonly OpenApiSchema Schema = schema;
    protected readonly INamingStrategy Naming = naming;

    /// <summary>
    /// The minimum C# version the generated code must compile with, with <see cref="LanguageVersion.Latest"/> and friends already resolved.
    /// </summary>
    protected readonly LanguageVersion LanguageVersion = languageVersion.MapSpecifiedToEffectiveVersion();

    /// <summary>
    /// Indicates whether the generated code may use nullable reference type annotations.
    /// </summary>
    protected bool NullableReferenceTypes => LanguageVersion >= LanguageVersion.CSharp8;

    public static DtoBuilder? For(string key, OpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest)
        => (schema.Type ?? "object") switch
        {
            "object" => new DtoClassBuilder(key, schema, naming, languageVersion),
            "string" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, languageVersion),
            "integer" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, languageVersion),
            _ => null
        };

    protected readonly List<ICSharpType> ChildTypes = [];

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
        if (Schema.Deprecated) type.Attributes.Add(Attributes.Obsolete);
        type.NullableContext = NullableReferenceTypes;
        return type;
    }

    protected abstract ICSharpType BuildTypeInner();
}
