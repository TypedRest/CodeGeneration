namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// Turns OpenAPI keys into identifiers, using conventions that are shared across target languages.
/// </summary>
public static class Words
{
    /// <summary>
    /// Converts <paramref name="key"/> to <c>PascalCase</c>, preserving inner casing for keys that already are a single camelCase or PascalCase word.
    /// </summary>
    public static string ToPascalCase(string key)
    {
        if (string.IsNullOrEmpty(key)) return "";

        var words = Split(key);
        string result = words.Count switch
        {
            0 => "",
            // CamelCase: preserve the inner casing
            1 when words[0].Length == key.Length => Capitalize(words[0]),
            // kebap-case, snake_case or anything else separated by non-identifier characters
            _ => string.Concat(words.Select(word => Capitalize(word.ToLower())))
        };

        // Identifiers may not start with a digit
        return result.Length != 0 && char.IsDigit(result[0]) ? "_" + result : result;
    }

    /// <summary>
    /// Converts <paramref name="key"/> to <c>camelCase</c>.
    /// </summary>
    public static string ToCamelCase(string key)
    {
        string pascal = ToPascalCase(key);
        return pascal.Length == 0 ? pascal : char.ToLower(pascal[0]) + pascal.Substring(1);
    }

    /// <summary>
    /// Splits a key into words, treating every character that may not appear in an identifier as a separator.
    /// </summary>
    public static List<string> Split(string key)
    {
        var words = new List<string>();

        int start = -1;
        for (int i = 0; i < key.Length; i++)
        {
            if (char.IsLetterOrDigit(key[i]))
            {
                if (start < 0) start = i;
            }
            else if (start >= 0)
            {
                words.Add(key.Substring(start, i - start));
                start = -1;
            }
        }
        if (start >= 0) words.Add(key.Substring(start));

        return words;
    }

    /// <summary>
    /// Converts the first character of <paramref name="word"/> to upper case.
    /// </summary>
    public static string Capitalize(string word)
        => word.Length == 0 ? word : word.Substring(0, 1).ToUpper() + word.Substring(1);
}
