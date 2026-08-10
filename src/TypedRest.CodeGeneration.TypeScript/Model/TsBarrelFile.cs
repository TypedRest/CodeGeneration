using System.Text;
using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A generated TypeScript file that re-exports everything from a set of other generated modules.
/// </summary>
/// <param name="path">The path of the file relative to the output directory, e.g. <c>index.ts</c>.</param>
/// <param name="modules">The modules to re-export, in the order they should appear.</param>
public sealed class TsBarrelFile(string path, IEnumerable<TsModule> modules) : IGeneratedFile
{
    /// <inheritdoc/>
    public string Path { get; } = path;

    /// <inheritdoc/>
    public Encoding Encoding { get; } = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

    /// <summary>
    /// The modules re-exported by this file.
    /// </summary>
    public IReadOnlyList<TsModule> Modules { get; } = [..modules];

    /// <inheritdoc/>
    public void WriteTo(TextWriter textWriter)
    {
        var writer = new TsWriter(textWriter);
        writer.WriteLine(TsFile.Header);
        writer.WriteLine();

        foreach (var module in Modules)
            writer.WriteLine($"export * from {Ts.Quote(module.RelativeTo(Path))};");
    }
}
