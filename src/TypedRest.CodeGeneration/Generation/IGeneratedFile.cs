using System.Text;

namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// A single source file produced by an <see cref="IClientGenerator"/>.
/// </summary>
public interface IGeneratedFile
{
    /// <summary>
    /// The path of the file relative to the output directory, using <c>/</c> as the separator, including the extension.
    /// </summary>
    string Path { get; }

    /// <summary>
    /// The encoding to use when writing the file to disk.
    /// </summary>
    Encoding Encoding { get; }

    /// <summary>
    /// Writes the content of the file.
    /// </summary>
    void WriteTo(TextWriter writer);
}
