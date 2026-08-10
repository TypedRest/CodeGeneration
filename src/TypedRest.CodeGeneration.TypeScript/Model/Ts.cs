using System.Text;

namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// TypeScript syntax helpers.
/// </summary>
public static class Ts
{
    /// <summary>
    /// Indicates whether <paramref name="name"/> can be used as a TypeScript identifier without quoting.
    /// </summary>
    public static bool IsIdentifier(string name)
    {
        if (name.Length == 0) return false;
        if (!IsStart(name[0])) return false;

        for (int i = 1; i < name.Length; i++)
        {
            if (!IsStart(name[i]) && !char.IsDigit(name[i])) return false;
        }

        return true;

        static bool IsStart(char c)
            => char.IsLetter(c) || c == '_' || c == '$';
    }

    /// <summary>
    /// Returns <paramref name="name"/> as an object/interface member name, quoting it if necessary.
    /// </summary>
    public static string MemberName(string name)
        => IsIdentifier(name) ? name : Quote(name);

    /// <summary>
    /// Returns <paramref name="value"/> as a double-quoted TypeScript string literal.
    /// </summary>
    public static string Quote(string value)
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
                default:
                    builder.Append(NeedsEscaping(c) ? Escape(c) : c.ToString());
                    break;
            }
        }

        builder.Append('"');
        return builder.ToString();
    }

    /// <summary>
    /// Control characters and the two line separators that may not appear unescaped in a JavaScript string literal.
    /// </summary>
    private static bool NeedsEscaping(char c)
        => c < ' ' || c == '\u2028' || c == '\u2029';

    private static string Escape(char c)
        => "\\u" + ((int)c).ToString("x4");
}
