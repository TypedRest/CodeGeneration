using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public abstract class DtoBuilder(string key, OpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest, TypeNameRegistry? typeNames = null)
{
    protected readonly CSharpIdentifier Identifier = typeNames?.Register(naming.DtoType(key)) ?? naming.DtoType(key);
    protected readonly OpenApiSchema Schema = schema;
    protected readonly INamingStrategy Naming = naming;

    /// <summary>
    /// Keeps the names of types generated for inline schemas from colliding with other generated types.
    /// </summary>
    protected readonly TypeNameRegistry? TypeNames = typeNames;

    /// <summary>
    /// The minimum C# version the generated code must compile with, with <see cref="LanguageVersion.Latest"/> and friends already resolved.
    /// </summary>
    protected readonly LanguageVersion LanguageVersion = languageVersion.MapSpecifiedToEffectiveVersion();

    /// <summary>
    /// Indicates whether the generated code may use nullable reference type annotations.
    /// </summary>
    protected bool NullableReferenceTypes => LanguageVersion >= LanguageVersion.CSharp8;

    public static DtoBuilder? For(string key, OpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest, TypeNameRegistry? typeNames = null)
        => (schema.Type ?? "object") switch
        {
            "object" => new DtoClassBuilder(key, schema, naming, languageVersion, typeNames),
            "string" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, languageVersion, typeNames),
            "integer" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, languageVersion, typeNames),
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
