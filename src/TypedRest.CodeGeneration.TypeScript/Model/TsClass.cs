namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A TypeScript class declaration.
/// </summary>
/// <param name="identifier">The name of the class and the module it is declared in.</param>
public sealed class TsClass(TsIdentifier identifier) : ITsType
{
    /// <inheritdoc/>
    public TsIdentifier Identifier { get; } = identifier;

    /// <inheritdoc/>
    public string? Summary { get; set; }

    /// <inheritdoc/>
    public bool Deprecated { get; set; }

    /// <summary>
    /// The type this class derives from.
    /// </summary>
    public TsIdentifier? BaseType { get; set; }

    /// <summary>
    /// The constructor of the class. Leave this <c>null</c> to inherit the base constructor.
    /// </summary>
    public TsConstructor? Constructor { get; set; }

    /// <summary>
    /// The child endpoints exposed by the class.
    /// </summary>
    public List<TsGetter> Getters { get; } = [];

    /// <inheritdoc/>
    public IEnumerable<TsImport> GetImports()
    {
        if (BaseType != null)
        {
            foreach (var import in BaseType.GetImports()) yield return import;
        }

        if (Constructor != null)
        {
            foreach (var import in Constructor.GetImports()) yield return import;
        }

        foreach (var import in Getters.SelectMany(x => x.GetImports())) yield return import;
    }

    /// <inheritdoc/>
    public void Write(TsWriter writer)
    {
        writer.WriteDocComment(Summary, Deprecated);
        writer.WriteLine($"export class {Identifier.Name}{(BaseType == null ? "" : " extends " + BaseType.ToTypeExpression())} {{");

        using (writer.Indent())
        {
            bool first = true;

            if (Constructor != null)
            {
                Constructor.Write(writer);
                first = false;
            }

            foreach (var getter in Getters)
            {
                if (!first) writer.WriteLine();
                getter.Write(writer);
                first = false;
            }
        }

        writer.WriteLine("}");
    }
}
