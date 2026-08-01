using System.Text;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Minimal path handling. Reimplemented here because analyzers must not reference <c>System.IO</c> (RS1035).
/// </summary>
internal static class PathHelper
{
    /// <summary>Extracts the file name (including the extension) from a path.</summary>
    public static string FileName(string path)
    {
        int index = path.LastIndexOfAny(['/', '\\']);
        return index < 0 ? path : path.Substring(index + 1);
    }

    /// <summary>Extracts the file name without its extension from a path.</summary>
    public static string FileStem(string path)
    {
        string name = FileName(path);
        int index = name.LastIndexOf('.');
        return index <= 0 ? name : name.Substring(0, index);
    }

    /// <summary>Replaces everything but letters, digits, dots, dashes and underscores with underscores.</summary>
    public static string Sanitize(string value)
    {
        var builder = new StringBuilder(value.Length);
        foreach (char character in value)
            builder.Append(char.IsLetterOrDigit(character) || character is '.' or '-' or '_' ? character : '_');
        return builder.ToString();
    }
}
