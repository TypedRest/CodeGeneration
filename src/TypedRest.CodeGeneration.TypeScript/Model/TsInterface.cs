namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A TypeScript interface declaration, used for DTOs.
/// </summary>
/// <param name="identifier">The name of the interface and the module it is declared in.</param>
public sealed class TsInterface(TsIdentifier identifier) : ITsType
{
    /// <inheritdoc/>
    public TsIdentifier Identifier { get; } = identifier;

    /// <inheritdoc/>
    public string? Summary { get; set; }

    /// <inheritdoc/>
    public bool Deprecated { get; set; }

    /// <summary>
    /// The interfaces this interface derives from.
    /// </summary>
    /// <remarks>Unlike C# classes, TypeScript interfaces can extend more than one type, so every
    /// <c>allOf</c> entry referencing a schema can become an entry here.</remarks>
    public List<TsIdentifier> BaseTypes { get; } = [];

    /// <summary>
    /// The properties of the interface.
    /// </summary>
    public List<TsProperty> Properties { get; } = [];

    /// <inheritdoc/>
    public IEnumerable<TsImport> GetImports()
        => BaseTypes.SelectMany(x => x.GetImports())
                    .Concat(Properties.SelectMany(x => x.Type.GetImports()));

    /// <inheritdoc/>
    public void Write(TsWriter writer)
    {
        writer.WriteDocComment(Summary, Deprecated);

        string bases = BaseTypes.Count == 0
            ? ""
            : " extends " + string.Join(", ", BaseTypes.Select(x => x.ToTypeExpression()));

        if (Properties.Count == 0)
        {
            // An interface with no members of its own may not have an empty body if it has no base types either
            writer.WriteLine(BaseTypes.Count == 0
                ? $"export interface {Identifier.Name} {{}}"
                : $"export interface {Identifier.Name}{bases} {{}}");
            return;
        }

        writer.WriteLine($"export interface {Identifier.Name}{bases} {{");
        using (writer.Indent())
        {
            foreach (var property in Properties)
            {
                writer.WriteDocComment(property.Summary, property.Deprecated);
                writer.WriteLine(property.ToDeclaration());
            }
        }
        writer.WriteLine("}");
    }
}
