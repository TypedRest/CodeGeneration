using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoClassBuilder(string key, OpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest, TypeNameRegistry? typeNames = null, JsonAttributes? jsonAttributes = null)
    : DtoBuilder(key, schema, naming, languageVersion, typeNames, jsonAttributes)
{
    /// <summary>
    /// The properties of this type, including any merged in from inline <c>allOf</c> schemas.
    /// </summary>
    protected readonly IReadOnlyDictionary<string, OpenApiSchema> Properties = GetProperties(schema);

    /// <summary>
    /// The keys of the required properties, including any from inline <c>allOf</c> schemas.
    /// </summary>
    protected readonly ICollection<string> RequiredProperties = GetRequiredProperties(schema);

    protected override ICSharpType BuildTypeInner()
    {
        var type = new CSharpClass(Identifier);

        // A single $ref in allOf becomes the base class; its properties are inherited rather than repeated
        if (GetBaseSchema(Schema)?.Reference?.Id is {} baseId)
            type.BaseConstructor = new CSharpObjectCreation(Naming.DtoType(baseId));

        foreach ((string propKey, var propSchema) in Properties)
        {
            var property = BuildProperty(propKey, propSchema);

            if (RequiredProperties.Contains(propKey))
                property.Attributes.Add(Attributes.Required);

            if (propKey.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                property.Attributes.Add(Attributes.Key);

            type.Properties.Add(property);
        }

        return type;
    }

    /// <summary>
    /// Returns the <c>allOf</c> entry to use as the base class. C# has no multiple inheritance, so any
    /// additional <c>$ref</c> entries are flattened into this type instead.
    /// </summary>
    private static OpenApiSchema? GetBaseSchema(OpenApiSchema schema)
        => schema.AllOf.FirstOrDefault(x => x.Reference?.Id is {Length: > 0});

    /// <summary>
    /// Returns the schemas contributing properties to this type: the schema itself plus every
    /// <c>allOf</c> entry except the one used as the base class.
    /// </summary>
    private static IEnumerable<OpenApiSchema> GetSources(OpenApiSchema schema)
    {
        var baseSchema = GetBaseSchema(schema);

        yield return schema;
        foreach (var source in schema.AllOf)
        {
            if (source != baseSchema) yield return source;
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

    protected virtual CSharpProperty BuildProperty(string key, OpenApiSchema? schema)
    {
        string propertyName = Naming.Property(key);
        if (propertyName == Identifier.Name)
            propertyName += "Value";

        var type = GetPropertyType(propertyName, schema);
        bool required = RequiredProperties.Contains(key);

        // Below C# 8 reference types cannot be annotated as nullable at all
        bool canBeNullable = NullableReferenceTypes || !IsReferenceType(schema);

        var property = new CSharpProperty(
            IsCollection(schema) ? type.ToNonNullable() : (required || !canBeNullable ? type : type.ToNullable()),
            propertyName)
        {
            Summary = schema?.Description,
            Attributes = {Json.PropertyName(key)},
            HasSetter = true
        };

        // Collections are always instantiated, so they neither need nor want to be required
        if (IsCollection(schema))
            property.Initializer = new CSharpObjectCreation(property.Type);
        else if (required && property.Type is {Nullable: false})
        {
            if (LanguageVersion >= LanguageVersion.CSharp11)
                property.IsRequired = true;
            else if (NullableReferenceTypes && IsReferenceType(schema))
                property.InitializerExpression = "null!";
        }

        if (schema is {Deprecated: true})
            property.Attributes.Add(Attributes.Obsolete);

        return property;
    }

    private CSharpIdentifier GetPropertyType(string nameHint, OpenApiSchema? schema)
        => schema switch
        {
            // Inline enum
            {Reference: null, Type: "string" or "integer", Enum.Count: > 0} =>
                AddChildType(new DtoEnumBuilder(ChildKey(nameHint), schema, Naming, LanguageVersion, TypeNames, Json)),

            // Inline object
            {Reference: null, Properties.Count: > 0} =>
                AddChildType(new DtoClassBuilder(ChildKey(nameHint), schema, Naming, LanguageVersion, TypeNames, Json)),

            // Array of inline enums/objects
            {Type: "array", Items: {} items} when NeedsChildType(items) =>
                CSharpIdentifier.ListOf(GetPropertyType(nameHint.Depluralize(), items)),

            // Dictionary of inline enums/objects
            {AdditionalProperties: {} values} when NeedsChildType(values) =>
                CSharpIdentifier.DictionaryOf(CSharpIdentifier.String, GetPropertyType(nameHint, values)),

            _ => Naming.TypeFor(schema, NullableReferenceTypes)
        };

    /// <summary>
    /// Indicates whether a schema is inlined rather than referenced and therefore needs a type generated for it.
    /// </summary>
    private static bool NeedsChildType(OpenApiSchema schema)
        => schema is {Reference: null, Type: "string" or "integer", Enum.Count: > 0}
                  or {Reference: null, Properties.Count: > 0};

    /// <summary>
    /// Prefixes a child type's key with this type's name, so that e.g. two classes with an inline
    /// <c>status</c> enum do not both generate a type called <c>Status</c>.
    /// </summary>
    private string ChildKey(string nameHint)
        => Identifier.Name + nameHint;

    private CSharpIdentifier AddChildType(DtoBuilder builder)
    {
        var types = builder.BuildTypes().ToList();
        ChildTypes.AddRange(types);
        return types[0].Identifier;
    }

    private static bool IsCollection(OpenApiSchema? schema)
        => schema is {Type: "array"} or {AdditionalProperties: not null};

    /// <summary>
    /// Indicates whether <see cref="INamingStrategy.TypeFor"/> maps this schema to a reference type,
    /// i.e. one that needs a <c>null!</c> initializer to satisfy nullable reference type analysis.
    /// </summary>
    private static bool IsReferenceType(OpenApiSchema? schema)
        => schema?.Type switch
        {
            null or "object" => true, // $ref to a generated class, or the JObject fallback
            "string" => schema.Enum.Count == 0 && !schema.HasValueTypeFormat(), // string/Uri, unless it is an inline enum or a format that maps to a value type (e.g. uuid)
            _ => false // integer/number/boolean map to value types
        };
}
