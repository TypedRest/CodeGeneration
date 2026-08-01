using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoClassBuilder(string key, OpenApiSchema schema, INamingStrategy naming, LanguageVersion languageVersion = LanguageVersion.Latest)
    : DtoBuilder(key, schema, naming, languageVersion)
{
    protected override ICSharpType BuildTypeInner()
    {
        var type = new CSharpClass(Identifier);

        foreach ((string propKey, var propSchema) in Schema.Properties)
        {
            var property = BuildProperty(propKey, propSchema);

            if (Schema.Required.Contains(propKey))
                property.Attributes.Add(Attributes.Required);

            if (propKey.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                property.Attributes.Add(Attributes.Key);

            type.Properties.Add(property);
        }

        return type;
    }

    protected virtual CSharpProperty BuildProperty(string key, OpenApiSchema? schema)
    {
        string propertyName = Naming.Property(key);
        if (propertyName == Identifier.Name)
            propertyName += "Value";

        var type = GetPropertyType(propertyName, schema);
        bool required = Schema.Required.Contains(key);

        // Below C# 8 reference types cannot be annotated as nullable at all
        bool canBeNullable = NullableReferenceTypes || !IsReferenceType(schema);

        var property = new CSharpProperty(
            IsCollection(schema) ? type.ToNonNullable() : (required || !canBeNullable ? type : type.ToNullable()),
            propertyName)
        {
            Summary = schema?.Description,
            Attributes = {Attributes.JsonProperty(key)},
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

    private CSharpIdentifier GetPropertyType(string propertyName, OpenApiSchema? schema)
    {
        if (schema is {Type: "string" or "integer", Enum.Count: > 0})
        {
            var dtoEnum = new DtoEnumBuilder(propertyName, schema, Naming, LanguageVersion).BuildType();
            ChildTypes.Add(dtoEnum);
            return dtoEnum.Identifier;
        }

        return Naming.TypeFor(schema, NullableReferenceTypes);
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
            "string" => schema.Enum.Count == 0, // string/Uri, unless it is an inline enum
            _ => false // integer/number/boolean map to value types
        };
}
