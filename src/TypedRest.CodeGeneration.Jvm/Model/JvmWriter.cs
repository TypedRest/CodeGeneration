namespace TypedRest.CodeGeneration.Jvm.Model;

/// <summary>
/// Writes indented JVM source code.
/// </summary>
/// <param name="writer">The underlying writer.</param>
public sealed class JvmWriter(TextWriter writer)
{
    private int _level;

    /// <summary>
    /// Writes a line of code at the current indentation level.
    /// </summary>
    public void WriteLine(string text = "")
    {
        if (text.Length != 0)
        {
            for (int i = 0; i < _level; i++) writer.Write("    ");
            writer.Write(text);
        }
        writer.Write('\n');
    }

    /// <summary>
    /// Increases the indentation level until the returned value is disposed.
    /// </summary>
    public IDisposable Indent()
    {
        _level++;
        return new Dedenter(this);
    }

    /// <summary>
    /// Writes a JavaDoc/KDoc comment, if there is anything to write.
    /// </summary>
    /// <param name="summary">The description to write, if any.</param>
    /// <param name="deprecated">Adds a <c>@deprecated</c> tag.</param>
    public void WriteDocComment(string? summary, bool deprecated = false)
    {
        var lines = new List<string>();
        if (!string.IsNullOrEmpty(summary))
            lines.AddRange(summary!.Replace("\r\n", "\n").Split('\n').Select(line => line.TrimEnd()));
        if (deprecated)
        {
            if (lines.Count != 0) lines.Add("");
            lines.Add("@deprecated");
        }

        if (lines.Count == 0) return;

        // A doc comment may not contain the comment terminator
        lines = [..lines.Select(line => line.Replace("*/", "*&#47;"))];

        if (lines.Count == 1)
        {
            WriteLine($"/** {lines[0]} */");
            return;
        }

        WriteLine("/**");
        foreach (string line in lines)
            WriteLine(line.Length == 0 ? " *" : " * " + line);
        WriteLine(" */");
    }

    private sealed class Dedenter(JvmWriter writer) : IDisposable
    {
        public void Dispose() => writer._level--;
    }
}
