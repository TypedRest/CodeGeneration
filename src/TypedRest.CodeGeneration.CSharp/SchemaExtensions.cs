namespace TypedRest.CodeGeneration.CSharp;

internal static class SchemaExtensions
{
    /// <summary>
    /// Indicates whether a <c>string</c> schema maps to a value type because of its <c>format</c>, e.g. <c>uuid</c> to <see cref="System.Guid"/>.
    /// </summary>
    public static bool HasValueTypeFormat(this OpenApiSchema? schema)
        => schema is {Type: "string"}
        && schema.Format is "uuid" or "guid" or "date-time" or "date" or "time" or "duration";
}
