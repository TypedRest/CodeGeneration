using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Dtos;

/// <summary>
/// Builds a TypeScript interface for an object schema.
/// </summary>
/// <inheritdoc cref="DtoBuilder"/>
public class DtoInterfaceBuilder(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
    : DtoBuilder(key, schema, naming, typeNames)
{
    /// <summary>
    /// The properties of this type, including any merged in from inline <c>allOf</c> schemas.
    /// </summary>
    protected readonly IReadOnlyDictionary<string, OpenApiSchema> Properties = GetProperties(schema);

    /// <summary>
    /// The keys of the required properties, including any from inline <c>allOf</c> schemas.
    /// </summary>
    protected readonly ICollection<string> RequiredProperties = GetRequiredProperties(schema);

    /// <inheritdoc/>
    protected override ITsType BuildTypeInner()
    {
        var type = new TsInterface(Identifier);

        // Unlike C# classes, TypeScript interfaces can extend more than one type
        foreach (var baseSchema in GetBaseSchemas(Schema))
            type.BaseTypes.Add(Naming.DtoType(baseSchema.Reference.Id));

        foreach ((string propKey, var propSchema) in Properties)
            type.Properties.Add(BuildProperty(propKey, propSchema));

        return type;
    }

    /// <summary>
    /// Returns the <c>allOf</c> entries that reference another schema and therefore become base types.
    /// </summary>
    private static IEnumerable<OpenApiSchema> GetBaseSchemas(OpenApiSchema schema)
        => schema.AllOf.Where(x => x.Reference?.Id is {Length: > 0});

    /// <summary>
    /// Returns the schemas contributing properties to this type: the schema itself plus every inline
    /// <c>allOf</c> entry. Referenced entries contribute through <c>extends</c> instead.
    /// </summary>
    private static IEnumerable<OpenApiSchema> GetSources(OpenApiSchema schema)
    {
        yield return schema;

        foreach (var source in schema.AllOf)
        {
            if (source.Reference?.Id is not {Length: > 0}) yield return source;
        }
    }

    private static IReadOnlyDictionary<string, OpenApiSchema> GetProperties(OpenApiSchema schema)
    {
        var result = new Dictionary<string, OpenApiSchema>();
        foreach (var source in GetSources(schema))
        {
            foreach ((string key, var value) in source.Properties)
                result[key] = value;
        }
        return result;
    }

    private static ICollection<string> GetRequiredProperties(OpenApiSchema schema)
        => new HashSet<string>(GetSources(schema).SelectMany(x => x.Required));

    /// <summary>
    /// Builds a property, keeping the exact key used on the wire as its name.
    /// </summary>
    protected virtual TsProperty BuildProperty(string key, OpenApiSchema? schema)
    {
        var type = GetPropertyType(Words.ToPascalCase(key), schema);
        if (schema is {Nullable: true}) type = type.ToNullable();

        return new TsProperty(key, type)
        {
            // Absent and null are distinct in TypeScript, so "not required" and "nullable" stay separate
            Optional = !RequiredProperties.Contains(key),
            Summary = schema?.Description,
            Deprecated = schema is {Deprecated: true}
        };
    }

    private TsIdentifier GetPropertyType(string nameHint, OpenApiSchema? schema)
        => schema switch
        {
            // Inline enum
            {Reference: null, Type: "string" or "integer", Enum.Count: > 0} =>
                AddChildType(new DtoEnumBuilder(ChildKey(nameHint), schema, Naming, TypeNames)),

            // Inline object
            {Reference: null, Properties.Count: > 0} =>
                AddChildType(new DtoInterfaceBuilder(ChildKey(nameHint), schema, Naming, TypeNames)),

            // Array of inline enums/objects
            {Type: "array", Items: {} items} when NeedsChildType(items) =>
                TsIdentifier.ArrayOf(GetPropertyType(nameHint.Depluralize(), items)),

            // Record of inline enums/objects
            {AdditionalProperties: {} values} when NeedsChildType(values) =>
                TsIdentifier.RecordOf(TsIdentifier.String, GetPropertyType(nameHint, values)),

            _ => Naming.TypeFor(schema)
        };

    /// <summary>
    /// Indicates whether a schema is inlined rather than referenced and therefore needs a type generated for it.
    /// </summary>
    private static bool NeedsChildType(OpenApiSchema schema)
        => schema is {Reference: null, Type: "string" or "integer", Enum.Count: > 0}
                  or {Reference: null, Properties.Count: > 0};

    /// <summary>
    /// Prefixes a child type's key with this type's name, so that e.g. two interfaces with an inline
    /// <c>status</c> enum do not both generate a type called <c>Status</c>.
    /// </summary>
    private string ChildKey(string nameHint)
        => Identifier.Name + nameHint;

    private TsIdentifier AddChildType(DtoBuilder builder)
    {
        var types = builder.BuildTypes().ToList();
        ChildTypes.AddRange(types);
        return types[0].Identifier;
    }
}
