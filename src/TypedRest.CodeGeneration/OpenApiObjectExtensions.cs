namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="OpenApiObject"/>s.
/// </summary>
public static class OpenApiObjectExtensions
{
    /// <summary>
    /// Gets a string property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
    public static string? GetString(this OpenApiObject obj, string name)
        => obj.TryGetValue(name, out var anyData) && anyData is OpenApiString stringData ? stringData.Value : null;

    /// <summary>
    /// Gets a int property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
    public static int? GetInt(this OpenApiObject obj, string name)
        => obj.TryGetValue(name, out var anyData) && anyData is OpenApiInteger intData ? intData.Value : null;

    /// <summary>
    /// Gets a bool property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>false</c> if it was not found or had the wrong type.</returns>
    public static bool GetBool(this OpenApiObject obj, string name)
        => obj.TryGetValue(name, out var anyData) && anyData is OpenApiBoolean boolData && boolData.Value;

    /// <summary>
    /// Gets a schema property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
    public static OpenApiSchema? GetSchema(this OpenApiObject obj, string name)
    {
        if (obj.TryGetObject(name) is not {} schemaObj) return null;
        string? schemaRef = schemaObj.GetString("$ref");

        OpenApiReference? Reference(string prefix)
            => schemaRef != null && schemaRef.StartsWith(prefix)
                ? new OpenApiReference
                {
                    Id = schemaRef.Substring(prefix.Length),
                    Type = ReferenceType.Schema
                }
                : null;

        return new OpenApiSchema
        {
            Type = schemaObj.GetString("type"),
            Format = schemaObj.GetString("type"),
            Reference = Reference(prefix: "#/components/schemas/")
                     ?? Reference(prefix: "#/definitions/")
        };
    }

    /// <summary>
    /// Tries to get an object property with specified <paramref name="name"/> from the <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object to get the property from.</param>
    /// <param name="name">The name of the property to look for.</param>
    /// <returns>The value of the property; <c>null</c> if it was not found or it is not an object.</returns>
    public static OpenApiObject? TryGetObject(this OpenApiObject obj, string name)
        => obj.TryGetValue(name, out var anyData) && anyData is OpenApiObject objData ? objData : null;
}
