namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Helpers for transforming OpenAPI path/property keys before they become C# identifiers.
/// </summary>
public static class KeyExtensions
{
    /// <summary>
    /// Returns the singular form of <paramref name="key"/> using a small set of English depluralization rules.
    /// Falls back to <paramref name="key"/> unchanged if no rule applies (e.g. "address" stays "address").
    /// </summary>
    public static string Depluralize(this string key)
    {
        if (key.Length < 2) return key;

        if (key.EndsWith("ies", StringComparison.OrdinalIgnoreCase) && key.Length > 3)
            return key.Substring(0, key.Length - 3) + "y";

        if (key.EndsWith("ses", StringComparison.OrdinalIgnoreCase)
         || key.EndsWith("xes", StringComparison.OrdinalIgnoreCase)
         || key.EndsWith("zes", StringComparison.OrdinalIgnoreCase)
         || key.EndsWith("ches", StringComparison.OrdinalIgnoreCase)
         || key.EndsWith("shes", StringComparison.OrdinalIgnoreCase))
            return key.Substring(0, key.Length - 2);

        if (key.EndsWith("s", StringComparison.OrdinalIgnoreCase)
         && !key.EndsWith("ss", StringComparison.OrdinalIgnoreCase)
         && !key.EndsWith("us", StringComparison.OrdinalIgnoreCase))
            return key.Substring(0, key.Length - 1);

        return key;
    }
}
