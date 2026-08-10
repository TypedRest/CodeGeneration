using System.Text;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Exposes a generated C# type as an <see cref="IGeneratedFile"/>.
/// </summary>
/// <param name="type">The type to write.</param>
public class CSharpGeneratedFile(ICSharpType type) : IGeneratedFile
{
    /// <summary>
    /// The type written by this file.
    /// </summary>
    public ICSharpType Type { get; } = type;

    /// <inheritdoc/>
    public string Path { get; } = type.Identifier.Name + ".cs";

    /// <inheritdoc/>
    /// <remarks>UTF-8 with a byte order mark, matching <see cref="CSharpTypeExtensions.WriteToFile"/>.</remarks>
    public Encoding Encoding { get; } = Encoding.UTF8;

    /// <inheritdoc/>
    public void WriteTo(TextWriter writer)
        => writer.Write(Type.ToSyntax().ToFullString());
}
