namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A reference to a TypeScript type.
/// </summary>
public sealed class TsIdentifier
{
    /// <summary>
    /// Creates a new type reference.
    /// </summary>
    /// <param name="module">The module the type is imported from, or <c>null</c> for built-in types.</param>
    /// <param name="name">The name of the type.</param>
    /// <param name="nullable">Indicates whether the type can have the value <c>null</c>.</param>
    public TsIdentifier(TsModule? module, string name, bool nullable = false)
    {
        Module = module;
        Name = name;
        Nullable = nullable;
    }

    private TsIdentifier(TsIdentifier other, bool nullable)
        : this(other.Module, other.Name, nullable)
    {
        IsArray = other.IsArray;
        TypeArguments.AddRange(other.TypeArguments);
    }

    /// <summary>
    /// The module the type is imported from, or <c>null</c> for built-in types.
    /// </summary>
    public TsModule? Module { get; }

    /// <summary>
    /// The name of the type.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Indicates whether the type can have the value <c>null</c>.
    /// </summary>
    public bool Nullable { get; }

    /// <summary>
    /// Indicates whether this is an array of <see cref="TypeArguments"/>[0] rather than a named generic type.
    /// </summary>
    public bool IsArray { get; private set; }

    /// <summary>
    /// Generic type arguments for the type.
    /// </summary>
    public List<TsIdentifier> TypeArguments { get; } = [];

    /// <summary>
    /// Returns a copy of the type reference that can have the value <c>null</c>.
    /// </summary>
    public TsIdentifier ToNullable()
        => Nullable ? this : new TsIdentifier(this, nullable: true);

    /// <summary>
    /// An array of <paramref name="item"/>.
    /// </summary>
    public static TsIdentifier ArrayOf(TsIdentifier item)
        => new(null, "Array") {IsArray = true, TypeArguments = {item}};

    /// <summary>
    /// A <c>Record</c> mapping <paramref name="key"/> to <paramref name="value"/>.
    /// </summary>
    public static TsIdentifier RecordOf(TsIdentifier key, TsIdentifier value)
        => new(null, "Record") {TypeArguments = {key, value}};

    /// <summary>The built-in <c>string</c> type.</summary>
    public static TsIdentifier String => new(null, "string");

    /// <summary>The built-in <c>number</c> type.</summary>
    public static TsIdentifier Number => new(null, "number");

    /// <summary>The built-in <c>boolean</c> type.</summary>
    public static TsIdentifier Boolean => new(null, "boolean");

    /// <summary>The built-in <c>unknown</c> type.</summary>
    public static TsIdentifier Unknown => new(null, "unknown");

    /// <summary>The built-in <c>any</c> type.</summary>
    public static TsIdentifier Any => new(null, "any");

    /// <summary>The ambient <c>URL</c> type.</summary>
    public static TsIdentifier Url => new(null, "URL");

    /// <summary>The <c>URL | string</c> union used for relative URIs.</summary>
    public static TsIdentifier UrlOrString => new(null, "URL | string");

    /// <summary>
    /// Returns all imports needed to reference this type.
    /// </summary>
    public IEnumerable<TsImport> GetImports()
    {
        if (Module != null) yield return new TsImport(Module, Name);

        foreach (var import in TypeArguments.SelectMany(x => x.GetImports()))
            yield return import;
    }

    /// <summary>
    /// Returns the TypeScript expression for this type, e.g. <c>ElementEndpoint&lt;Contact&gt;</c>.
    /// </summary>
    public string ToTypeExpression()
    {
        string core;
        if (IsArray)
        {
            string item = TypeArguments[0].ToTypeExpression();
            // Unions and other compound expressions may not be suffixed with []
            core = item.IndexOf(' ') < 0 ? item + "[]" : $"Array<{item}>";
        }
        else
            core = TypeArguments.Count == 0
                ? Name
                : $"{Name}<{string.Join(", ", TypeArguments.Select(x => x.ToTypeExpression()))}>";

        return Nullable ? core + " | null" : core;
    }

    /// <inheritdoc/>
    public override string ToString()
        => ToTypeExpression();
}
