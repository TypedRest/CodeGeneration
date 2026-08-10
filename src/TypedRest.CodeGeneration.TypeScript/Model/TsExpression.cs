namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A TypeScript expression.
/// </summary>
public abstract class TsExpression
{
    /// <summary>
    /// Returns the TypeScript code for this expression.
    /// </summary>
    public abstract string ToCode();

    /// <summary>
    /// Returns all imports needed by this expression.
    /// </summary>
    public virtual IEnumerable<TsImport> GetImports()
        => [];

    /// <inheritdoc/>
    public override string ToString()
        => ToCode();
}

/// <summary>
/// Creates an instance of a type, e.g. <c>new ElementEndpoint&lt;Contact&gt;(this, "./contacts")</c>.
/// </summary>
/// <param name="type">The type to instantiate.</param>
public sealed class TsNew(TsIdentifier type) : TsExpression
{
    /// <summary>
    /// The type to instantiate.
    /// </summary>
    public TsIdentifier Type { get; } = type;

    /// <summary>
    /// The arguments to pass to the constructor.
    /// </summary>
    public List<TsExpression> Arguments { get; } = [];

    /// <inheritdoc/>
    public override string ToCode()
        => $"new {Type.ToTypeExpression()}({string.Join(", ", Arguments.Select(x => x.ToCode()))})";

    /// <inheritdoc/>
    public override IEnumerable<TsImport> GetImports()
        => Type.GetImports().Concat(Arguments.SelectMany(x => x.GetImports()));
}

/// <summary>
/// A string literal.
/// </summary>
/// <param name="value">The value of the literal.</param>
public sealed class TsLiteral(string value) : TsExpression
{
    /// <summary>
    /// The value of the literal.
    /// </summary>
    public string Value { get; } = value;

    /// <inheritdoc/>
    public override string ToCode()
        => Ts.Quote(Value);
}

/// <summary>
/// References a type as a value, e.g. to pass a class to a constructor expecting a factory.
/// </summary>
/// <param name="type">The type to reference. Any generic type arguments are dropped.</param>
public sealed class TsTypeRef(TsIdentifier type) : TsExpression
{
    /// <summary>
    /// The type to reference.
    /// </summary>
    public TsIdentifier Type { get; } = type;

    /// <inheritdoc/>
    public override string ToCode()
        => Type.Name;

    /// <inheritdoc/>
    public override IEnumerable<TsImport> GetImports()
        => Type.Module == null ? [] : [new TsImport(Type.Module, Type.Name)];
}

/// <summary>
/// The <c>this</c> reference.
/// </summary>
public sealed class TsThis : TsExpression
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static readonly TsThis Instance = new();

    /// <inheritdoc/>
    public override string ToCode()
        => "this";
}

/// <summary>
/// References a variable or parameter by name.
/// </summary>
/// <param name="name">The name of the variable or parameter.</param>
public sealed class TsName(string name) : TsExpression
{
    /// <summary>
    /// The name of the variable or parameter.
    /// </summary>
    public string Name { get; } = name;

    /// <inheritdoc/>
    public override string ToCode()
        => Name;
}
