using System.Text;
using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Kotlin;

/// <summary>
/// A Kotlin source file holding one generated type.
/// </summary>
/// <param name="type">The type declared in the file.</param>
/// <param name="writer">Renders the type.</param>
public sealed class KotlinGeneratedFile(IJvmType type, KotlinWriter writer) : IGeneratedFile
{
    /// <summary>
    /// The type declared in the file.
    /// </summary>
    public IJvmType Type { get; } = type;

    /// <inheritdoc/>
    public string Path
        => (Type.Identifier.Package ?? JvmPackage.External("")).FilePath(Type.Identifier.Name, KotlinWriter.FileExtension);

    /// <inheritdoc/>
    public Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    /// <inheritdoc/>
    public void WriteTo(TextWriter textWriter)
        => writer.WriteFile(textWriter, Type);
}
