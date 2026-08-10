using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm.Dtos;

/// <summary>
/// Builds the type for one schema in an OpenAPI/Swagger document.
/// </summary>
/// <param name="key">The key of the schema in the document.</param>
/// <param name="schema">The schema to build a type for.</param>
/// <param name="naming">Decides what the generated type is called.</param>
/// <param name="typeNames">Keeps the generated names from colliding.</param>
public abstract class DtoBuilder(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
{
    /// <summary>The name and package of the generated type.</summary>
    protected readonly JvmIdentifier Identifier = typeNames?.Register(naming.DtoType(key)) ?? naming.DtoType(key);

    /// <summary>The schema being generated for.</summary>
    protected readonly OpenApiSchema Schema = schema;

    /// <summary>Decides what the generated types are called.</summary>
    protected readonly INamingStrategy Naming = naming;

    /// <summary>Keeps the names of types generated for inline schemas from colliding with other generated types.</summary>
    protected readonly TypeNameRegistry? TypeNames = typeNames;

    /// <summary>Types generated for schemas inlined into this one.</summary>
    protected readonly List<IJvmType> ChildTypes = [];

    /// <summary>
    /// Returns the builder for <paramref name="schema"/>, or <c>null</c> if it needs no type of its own.
    /// </summary>
    public static DtoBuilder? For(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
        => (schema.Type ?? "object") switch
        {
            "object" => new DtoClassBuilder(key, schema, naming, typeNames),
            "string" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, typeNames),
            "integer" when schema.Enum.Count != 0 => new DtoEnumBuilder(key, schema, naming, typeNames),
            _ => null
        };

    /// <summary>
    /// Builds the type for the schema, followed by any types generated for schemas inlined into it.
    /// </summary>
    public IEnumerable<IJvmType> BuildTypes()
    {
        ChildTypes.Clear();
        yield return BuildType();

        foreach (var type in ChildTypes)
            yield return type;
    }

    private IJvmType BuildType()
    {
        var type = BuildTypeInner();
        type.Summary = Schema.Description;
        type.Deprecated = Schema.Deprecated;
        return type;
    }

    /// <summary>
    /// Builds the type itself, without the parts every kind of DTO has in common.
    /// </summary>
    protected abstract IJvmType BuildTypeInner();
}

/// <summary>
/// Builds a DTO for an object schema.
/// </summary>
public class DtoClassBuilder(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
    : DtoBuilder(key, schema, naming, typeNames)
{
    /// <summary>
    /// The properties of this type, including any merged in from <c>allOf</c> schemas.
    /// </summary>
    protected readonly IReadOnlyDictionary<string, OpenApiSchema> Properties = GetProperties(schema);

    /// <summary>
    /// The keys of the required properties, including any from <c>allOf</c> schemas.
    /// </summary>
    protected readonly ICollection<string> RequiredProperties = GetRequiredProperties(schema);

    /// <inheritdoc/>
    protected override IJvmType BuildTypeInner()
    {
        var type = new JvmDto(Identifier);

        foreach ((string propKey, var propSchema) in Properties)
            type.Properties.Add(BuildProperty(propKey, propSchema));

        return type;
    }

    /// <summary>
    /// Returns every schema contributing properties to this type.
    /// </summary>
    private static IEnumerable<OpenApiSchema> GetSources(OpenApiSchema schema)
    {
        yield return schema;
        foreach (var source in schema.AllOf)
            yield return source;
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
    /// Builds one property of the DTO.
    /// </summary>
    protected virtual JvmDtoProperty BuildProperty(string key, OpenApiSchema? schema)
    {
        string propertyName = Naming.Property(key);
        if (propertyName == Identifier.Name)
            propertyName += "Value";

        var type = GetPropertyType(propertyName, schema);
        bool required = RequiredProperties.Contains(key);

        // A property the document does not require may simply be absent from a response, so it has to be nullable.
        // Collections are the exception: they default to an empty one rather than to null.
        if (!required && !IsCollection(schema)) type = type.ToNullable();

        return new JvmDtoProperty(propertyName, key, type)
        {
            Summary = schema?.Description,
            Deprecated = schema is {Deprecated: true},
            Required = required
        };
    }

    private JvmIdentifier GetPropertyType(string nameHint, OpenApiSchema? schema)
        => schema switch
        {
            // Inline enum
            {Reference: null, Type: "string" or "integer", Enum.Count: > 0} =>
                AddChildType(new DtoEnumBuilder(ChildKey(nameHint), schema, Naming, TypeNames)),

            // Inline object
            {Reference: null, Properties.Count: > 0} =>
                AddChildType(new DtoClassBuilder(ChildKey(nameHint), schema, Naming, TypeNames)),

            // Array of inline enums/objects
            {Type: "array", Items: {} items} when NeedsChildType(items) =>
                JvmIdentifier.ListOf(GetPropertyType(nameHint.Depluralize(), items)),

            // Map of inline enums/objects
            {AdditionalProperties: {} values} when NeedsChildType(values) =>
                JvmIdentifier.MapOf(GetPropertyType(nameHint, values)),

            _ => Naming.TypeFor(schema)
        };

    /// <summary>
    /// Indicates whether a schema is inlined rather than referenced and therefore needs a type generated for it.
    /// </summary>
    private static bool NeedsChildType(OpenApiSchema schema)
        => schema is {Reference: null, Type: "string" or "integer", Enum.Count: > 0}
                  or {Reference: null, Properties.Count: > 0};

    /// <summary>
    /// Prefixes a child type's key with this type's name, so that e.g. two DTOs with an inline <c>status</c> enum
    /// do not both generate a type called <c>Status</c>.
    /// </summary>
    private string ChildKey(string nameHint)
        => Identifier.Name + nameHint;

    private JvmIdentifier AddChildType(DtoBuilder builder)
    {
        var types = builder.BuildTypes().ToList();
        ChildTypes.AddRange(types);
        return types[0].Identifier;
    }

    private static bool IsCollection(OpenApiSchema? schema)
        => schema is {Type: "array"} or {AdditionalProperties: not null};
}

/// <summary>
/// Builds an enum for a schema with an <c>enum</c>.
/// </summary>
public class DtoEnumBuilder(string key, OpenApiSchema schema, INamingStrategy naming, TypeNameRegistry? typeNames = null)
    : DtoBuilder(key, schema, naming, typeNames)
{
    /// <inheritdoc/>
    protected override IJvmType BuildTypeInner()
    {
        var type = new JvmEnum(Identifier);
        var usedNames = new HashSet<string>();

        foreach (var value in Schema.Enum)
        {
            switch (value)
            {
                case OpenApiString str:
                    type.Values.Add(new JvmEnumValue(UniqueName(usedNames, ValueName(str.Value)), str.Value));
                    break;
                case OpenApiInteger num:
                    type.Values.Add(new JvmEnumValue(UniqueName(usedNames, NumericName(num.Value)), num.Value.ToString()));
                    break;
                case OpenApiLong num:
                    type.Values.Add(new JvmEnumValue(UniqueName(usedNames, NumericName(num.Value)), num.Value.ToString()));
                    break;
            }
        }

        return type;
    }

    /// <summary>
    /// Builds the name of an enum value.
    /// </summary>
    private static string ValueName(string value)
    {
        var words = Words.Split(value);
        return words.Count == 0
            ? ""
            : JvmSyntax.Identifier(string.Join("_", words.Select(word => word.ToUpperInvariant())));
    }

    /// <summary>
    /// Builds a name for a numeric value, avoiding the minus sign which may not appear in an identifier.
    /// </summary>
    private static string NumericName(long value)
        => value < 0
            ? "VALUE_MINUS_" + -value
            : "VALUE_" + value;

    /// <summary>
    /// Ensures the <paramref name="name"/> is usable as an identifier and unique within the enum.
    /// </summary>
    private static string UniqueName(HashSet<string> usedNames, string name)
    {
        // Schemas may contain an empty string as an enum value
        if (name.Length == 0) name = "EMPTY";

        string result = name;
        for (int i = 2; !usedNames.Add(result); i++)
            result = name + i;
        return result;
    }
}
