namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="IOpenApiSchema"/>s.
/// </summary>
public static class OpenApiSchemaExtensions
{
    private static readonly string[] ComponentPrefixes = ["#/components/schemas/", "#/definitions/"];

    /// <summary>
    /// Extracts the component name from a local schema <c>$ref</c> (OpenAPI 3.x or Swagger 2.0 style).
    /// </summary>
    /// <returns>The name of the component; <c>null</c> if <paramref name="reference"/> does not point to a local schema component.</returns>
    public static string? TryGetComponentId(string reference)
    {
        foreach (string prefix in ComponentPrefixes)
        {
            if (reference.StartsWith(prefix, StringComparison.Ordinal))
                return reference.Substring(prefix.Length);
        }
        return null;
    }

    /// <summary>
    /// Gets the name of the schema component the <paramref name="schema"/> refers to, if it is a reference.
    /// </summary>
    public static string? GetReferenceId(this IOpenApiSchema? schema)
        => (schema as OpenApiSchemaReference)?.Reference.Id;

    /// <summary>
    /// Re-creates a reference so that it resolves against <paramref name="document"/>. Schemas that are not references are returned as is.
    /// </summary>
    public static IOpenApiSchema Resolve(this IOpenApiSchema schema, OpenApiDocument document)
        => schema.GetReferenceId() is {} id
            ? new OpenApiSchemaReference(id, document)
            : schema;

    /// <summary>
    /// Indicates whether the <paramref name="schema"/> has the specified <paramref name="type"/>, ignoring whether <c>null</c> is also allowed.
    /// </summary>
    public static bool Is(this IOpenApiSchema? schema, JsonSchemaType type)
        => schema?.Type is {} schemaType && (schemaType & ~JsonSchemaType.Null) == type;

    /// <summary>
    /// Indicates whether the <paramref name="schema"/> allows <c>null</c> values (<c>nullable: true</c> in OpenAPI 3.0, <c>type: [..., "null"]</c> in OpenAPI 3.1+).
    /// </summary>
    public static bool IsNullable(this IOpenApiSchema? schema)
        => schema?.Type is {} schemaType && schemaType.HasFlag(JsonSchemaType.Null);

    /// <summary>
    /// Indicates whether the <paramref name="schema"/> is a string or integer enum defined inline rather than referenced from a component.
    /// </summary>
    public static bool IsInlineEnum(this IOpenApiSchema schema)
        => schema is not OpenApiSchemaReference
        && (schema.Is(JsonSchemaType.String) || schema.Is(JsonSchemaType.Integer))
        && schema.Enum is {Count: > 0};

    /// <summary>
    /// Indicates whether the <paramref name="schema"/> is an object with properties defined inline rather than referenced from a component.
    /// </summary>
    public static bool IsInlineObject(this IOpenApiSchema schema)
        => schema is not OpenApiSchemaReference && schema.Properties is {Count: > 0};

    /// <summary>
    /// Gets the <c>type</c> of the <paramref name="schema"/> as a single identifier (e.g. <c>string</c>), ignoring whether <c>null</c> is also allowed.
    /// </summary>
    /// <returns>The identifier; <c>null</c> if the type is not set or is a combination of multiple types.</returns>
    public static string? GetTypeIdentifier(this IOpenApiSchema? schema)
        => schema?.Type is {} schemaType
            ? (schemaType & ~JsonSchemaType.Null) switch
            {
                JsonSchemaType.Boolean => "boolean",
                JsonSchemaType.Integer => "integer",
                JsonSchemaType.Number => "number",
                JsonSchemaType.String => "string",
                JsonSchemaType.Object => "object",
                JsonSchemaType.Array => "array",
                _ => null
            }
            : null;
}
