namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// Extension methods for <see cref="IGeneratedFile"/>.
/// </summary>
public static class GeneratedFileExtensions
{
    /// <summary>
    /// Returns the content of the file as a string.
    /// </summary>
    public static string GetContent(this IGeneratedFile file)
    {
        var writer = new StringWriter {NewLine = "\n"};
        file.WriteTo(writer);
        return writer.ToString();
    }

    /// <summary>
    /// Writes the file to <see cref="IGeneratedFile.Path"/> below the directory at <paramref name="path"/>,
    /// creating any missing subdirectories.
    /// </summary>
    public static void WriteToDirectory(this IGeneratedFile file, string path)
    {
        string filePath = Path.Combine(path, file.Path.Replace('/', Path.DirectorySeparatorChar));

        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory!);

        using var writer = new StreamWriter(filePath, append: false, file.Encoding) {NewLine = "\n"};
        file.WriteTo(writer);
    }
}
