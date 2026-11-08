using System.Globalization;
using System.Text.Json;

namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="JsonNode"/>s, e.g. <c>enum</c> values of a schema.
/// </summary>
public static class JsonNodeExtensions
{
    /// <summary>
    /// Gets the value of the <paramref name="node"/> if it is a JSON string.
    /// </summary>
    public static string? AsString(this JsonNode? node)
        => node is JsonValue value && value.GetValueKind() == JsonValueKind.String && value.TryGetValue(out string? result)
            ? result
            : null;

    /// <summary>
    /// Gets the value of the <paramref name="node"/> if it is a JSON number without a fractional part.
    /// </summary>
    public static long? AsInteger(this JsonNode? node)
    {
        if (node is not JsonValue value || value.GetValueKind() != JsonValueKind.Number) return null;
        if (value.TryGetValue(out long result)) return result;
        return long.TryParse(value.ToJsonString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out result) ? result : null;
    }
}
