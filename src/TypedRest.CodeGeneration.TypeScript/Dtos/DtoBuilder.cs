using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Dtos;

/// <summary>
/// Builds the TypeScript type for a single schema.
/// </summary>
/// <param name="key">The key identifying the schema.</param>
/// <param name="schema">The schema to generate a type for.</param>
/// <param name="naming">Decides what the generated type is called.</param>
/// <param name="typeNames">Keeps the names of types generated for inline schemas from colliding with other generated types.</param>
public abstract class DtoBuilder(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
{
    /// <summary>The name and module of the generated type.</summary>
    protected readonly TsIdentifier Identifier = typeNames?.Register(naming.DtoType(key)) ?? naming.DtoType(key);

    /// <summary>The schema to generate a type for.</summary>
    protected readonly OpenApiSchema Schema = schema;

    /// <summary>Decides what the generated type is called.</summary>
    protected readonly INamingStrategy Naming = naming;

    /// <summary>
    /// Keeps the names of types generated for inline schemas from colliding with other generated types.
    /// </summary>
    protected readonly TypeNameRegistry? TypeNames = typeNames;

    /// <summary>
    /// Types generated for schemas inlined in <see cref="Schema"/>.
    /// </summary>
    protected readonly List<ITsType> ChildTypes = [];

    /// <summary>
    /// Returns a builder suitable for <paramref name="schema"/>, or <c>null</c> if it needs no type of its own.
    /// </summary>
    public static DtoBuilder? For(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
        => (schema.Type ?? "object") switch
        {
            "object" => new DtoInterfaceBuilder(key, schema, naming, typeNames),
            "string" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, typeNames),
            "integer" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, typeNames),
            _ => null
        };

    /// <summary>
    /// Builds the type for <see cref="Schema"/>, followed by the types for any schemas inlined in it.
    /// </summary>
    public IEnumerable<ITsType> BuildTypes()
    {
        ChildTypes.Clear();
        yield return BuildType();

        foreach (var type in ChildTypes)
            yield return type;
    }

    internal ITsType BuildType()
    {
        var type = BuildTypeInner();
        type.Summary = Schema.Description;
        type.Deprecated = Schema.Deprecated;
        return type;
    }

    /// <summary>
    /// Builds the type for <see cref="Schema"/>, without the parts common to all kinds of DTO.
    /// </summary>
    protected abstract ITsType BuildTypeInner();
}
