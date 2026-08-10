namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A TypeScript type declaration that can be exported from a module.
/// </summary>
public interface ITsType
{
    /// <summary>
    /// The name of the type and the module it is declared in.
    /// </summary>
    TsIdentifier Identifier { get; }

    /// <summary>
    /// A description of the type for a JSDoc comment.
    /// </summary>
    string? Summary { get; set; }

    /// <summary>
    /// Marks the type as deprecated.
    /// </summary>
    bool Deprecated { get; set; }

    /// <summary>
    /// Writes the declaration of the type.
    /// </summary>
    void Write(TsWriter writer);

    /// <summary>
    /// Returns all imports needed by the declaration of the type.
    /// </summary>
    IEnumerable<TsImport> GetImports();
}
