namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A TypeScript type alias declaration, used for enums.
/// </summary>
/// <param name="identifier">The name of the alias and the module it is declared in.</param>
/// <param name="definition">The type expression the alias stands for, e.g. <c>"a" | "b"</c>.</param>
/// <remarks>
/// Enums are emitted as literal unions rather than as a TypeScript <c>enum</c>, because a string <c>enum</c>
/// refuses assignment of a bare string literal - which is exactly what <c>JSON.parse()</c> produces. Unions are
/// also fully erased at runtime, so consumers of a DTO need no import for them.
/// </remarks>
public sealed class TsTypeAlias(TsIdentifier identifier, string definition) : ITsType
{
    /// <inheritdoc/>
    public TsIdentifier Identifier { get; } = identifier;

    /// <inheritdoc/>
    public string? Summary { get; set; }

    /// <inheritdoc/>
    public bool Deprecated { get; set; }

    /// <summary>
    /// The type expression the alias stands for.
    /// </summary>
    public string Definition { get; } = definition;

    /// <inheritdoc/>
    public IEnumerable<TsImport> GetImports()
        => [];

    /// <inheritdoc/>
    public void Write(TsWriter writer)
    {
        writer.WriteDocComment(Summary, Deprecated);
        writer.WriteLine($"export type {Identifier.Name} = {Definition};");
    }
}
