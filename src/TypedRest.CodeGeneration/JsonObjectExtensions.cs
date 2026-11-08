namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="JsonObject"/>s, e.g. the data of an OpenAPI extension.
/// </summary>
public static class JsonObjectExtensions
{
    /// <summary>
    /// Gets a string property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
    public static string? GetString(this JsonObject obj, string name)
        => obj.TryGetPropertyValue(name, out var node) && node is JsonValue value && value.TryGetValue(out string? result) ? result : null;

    /// <summary>
    /// Gets a int property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
    public static int? GetInt(this JsonObject obj, string name)
        => obj.TryGetPropertyValue(name, out var node) && node is JsonValue value && value.TryGetValue(out int result) ? result : null;

    /// <summary>
    /// Gets a bool property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>false</c> if it was not found or had the wrong type.</returns>
    public static bool GetBool(this JsonObject obj, string name)
        => obj.TryGetPropertyValue(name, out var node) && node is JsonValue value && value.TryGetValue(out bool result) && result;

    /// <summary>
    /// Gets a schema property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
    /// <remarks>References to components are returned as unresolved <see cref="OpenApiSchemaReference"/>s. Use <see cref="Endpoints.IEndpoint.ResolveReferences"/> to resolve them.</remarks>
    public static IOpenApiSchema? GetSchema(this JsonObject obj, string name)
    {
        if (obj.TryGetObject(name) is not {} schemaObj) return null;

        if (schemaObj.GetString("$ref") is {} schemaRef
         && (OpenApiSchemaExtensions.TryGetComponentId(schemaRef) is {} id))
            return new OpenApiSchemaReference(id);

        return new OpenApiSchema
        {
            Type = schemaObj.GetString("type") is {} type ? type.ToJsonSchemaType() : null,
            Format = schemaObj.GetString("format")
        };
    }

    /// <summary>
    /// Tries to get an object property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to get the property from.</param>
    /// <param name="name">The name of the property to look for.</param>
    /// <returns>The value of the property; <c>null</c> if it was not found or it is not an object.</returns>
    public static JsonObject? TryGetObject(this JsonObject obj, string name)
        => obj.TryGetPropertyValue(name, out var node) ? node as JsonObject : null;
}
