using System.Text;

namespace TypedRest.CodeGeneration.Jvm.Model;

/// <summary>
/// Syntax helpers shared by the Java and Kotlin generators.
/// </summary>
public static class JvmSyntax
{
    /// <summary>
    /// The words that may not be used as an identifier in either Java or Kotlin.
    /// </summary>
    private static readonly HashSet<string> _reservedWords = new(StringComparer.Ordinal)
    {
        // Java
        "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "continue",
        "default", "do", "double", "else", "enum", "extends", "final", "finally", "float", "for", "goto", "if",
        "implements", "import", "instanceof", "int", "interface", "long", "native", "new", "package", "private",
        "protected", "public", "return", "short", "static", "strictfp", "super", "switch", "synchronized", "this",
        "throw", "throws", "transient", "try", "void", "volatile", "while",
        // Kotlin
        "as", "fun", "in", "is", "object", "typealias", "typeof", "val", "var", "when",
        // Literals, reserved in both
        "true", "false", "null", "_"
    };

    /// <summary>
    /// Indicates whether <paramref name="word"/> may not be used as an identifier in Java or Kotlin.
    /// </summary>
    public static bool IsReservedWord(string word)
        => _reservedWords.Contains(word);

    /// <summary>
    /// Returns <paramref name="name"/> as an identifier that is legal in both languages, suffixing reserved words.
    /// </summary>
    public static string Identifier(string name)
    {
        if (name.Length == 0) return "_";

        var builder = new StringBuilder(name.Length);
        foreach (char c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_') builder.Append(c);
        }

        if (builder.Length == 0) return "_";
        if (char.IsDigit(builder[0])) builder.Insert(0, '_');

        string result = builder.ToString();
        return IsReservedWord(result) ? result + "_" : result;
    }

    /// <summary>
    /// Returns <paramref name="value"/> as a double-quoted string literal, valid in both languages.
    /// </summary>
    public static string Quote(string value)
        => Quote(value, escapeDollar: false);

    /// <summary>
    /// Returns <paramref name="value"/> as a double-quoted string literal.
    /// </summary>
    /// <param name="value">The string to quote.</param>
    /// <param name="escapeDollar">
    /// Escapes <c>$</c> as <c>\$</c>, which Kotlin requires to keep it from starting a string template and Java
    /// rejects as an unknown escape sequence.
    /// </param>
    public static string Quote(string value, bool escapeDollar)
    {
        var builder = new StringBuilder(value.Length + 2);
        builder.Append('"');

        foreach (char c in value)
        {
            switch (c)
            {
                case '"':
                    builder.Append("\\\"");
                    break;
                case '\\':
                    builder.Append("\\\\");
                    break;
                case '\n':
                    builder.Append("\\n");
                    break;
                case '\r':
                    builder.Append("\\r");
                    break;
                case '\t':
                    builder.Append("\\t");
                    break;
                case '$' when escapeDollar:
                    builder.Append("\\$");
                    break;
                default:
                    builder.Append(c < ' ' ? "\\u" + ((int)c).ToString("x4") : c.ToString());
                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }
}
