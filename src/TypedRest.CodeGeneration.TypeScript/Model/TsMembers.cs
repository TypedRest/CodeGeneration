namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A property of a <see cref="TsInterface"/>.
/// </summary>
/// <param name="name">The name of the property. This is the literal name used on the wire, not an identifier.</param>
/// <param name="type">The type of the property.</param>
public sealed class TsProperty(string name, TsIdentifier type)
{
    /// <summary>
    /// The name of the property. This is the literal name used on the wire and gets quoted if it is not a valid
    /// TypeScript identifier.
    /// </summary>
    /// <remarks>
    /// TypedRest for TypeScript deserializes with <c>JSON.parse()</c> and a cast, so there is no way to map a
    /// property to a differently named field.
    /// </remarks>
    public string Name { get; } = name;

    /// <summary>
    /// The type of the property.
    /// </summary>
    public TsIdentifier Type { get; } = type;

    /// <summary>
    /// Indicates whether the property may be absent.
    /// </summary>
    public bool Optional { get; set; }

    /// <summary>
    /// A description of the property for a JSDoc comment.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Marks the property as deprecated.
    /// </summary>
    public bool Deprecated { get; set; }

    internal string ToDeclaration()
        => $"{Ts.MemberName(Name)}{(Optional ? "?" : "")}: {Type.ToTypeExpression()};";
}

/// <summary>
/// A property getter of a <see cref="TsClass"/>.
/// </summary>
/// <param name="name">The name of the getter.</param>
/// <param name="type">The type returned by the getter.</param>
/// <param name="body">The expression returned by the getter.</param>
public sealed class TsGetter(string name, TsIdentifier type, TsExpression body)
{
    /// <summary>
    /// The name of the getter.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The type returned by the getter.
    /// </summary>
    public TsIdentifier Type { get; } = type;

    /// <summary>
    /// The expression returned by the getter.
    /// </summary>
    public TsExpression Body { get; } = body;

    /// <summary>
    /// A description of the getter for a JSDoc comment.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Marks the getter as deprecated.
    /// </summary>
    public bool Deprecated { get; set; }

    internal IEnumerable<TsImport> GetImports()
        => Type.GetImports().Concat(Body.GetImports());

    internal void Write(TsWriter writer)
    {
        writer.WriteDocComment(Summary, Deprecated);
        writer.WriteLine($"get {Ts.MemberName(Name)}(): {Type.ToTypeExpression()} {{");
        using (writer.Indent())
            writer.WriteLine($"return {Body.ToCode()};");
        writer.WriteLine("}");
    }
}

/// <summary>
/// A parameter of a <see cref="TsConstructor"/>.
/// </summary>
/// <param name="name">The name of the parameter.</param>
/// <param name="type">The type of the parameter.</param>
public sealed class TsParameter(string name, TsIdentifier type)
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public TsIdentifier Type { get; } = type;

    internal string ToDeclaration()
        => $"{Name}: {Type.ToTypeExpression()}";
}

/// <summary>
/// The constructor of a <see cref="TsClass"/>. Its body consists solely of a call to the base constructor.
/// </summary>
public sealed class TsConstructor
{
    /// <summary>
    /// The parameters the constructor takes.
    /// </summary>
    public List<TsParameter> Parameters { get; } = [];

    /// <summary>
    /// The arguments to pass to the base constructor.
    /// </summary>
    public List<TsExpression> SuperArguments { get; } = [];

    internal IEnumerable<TsImport> GetImports()
        => Parameters.SelectMany(x => x.Type.GetImports())
                     .Concat(SuperArguments.SelectMany(x => x.GetImports()));

    internal void Write(TsWriter writer)
    {
        writer.WriteLine($"constructor({string.Join(", ", Parameters.Select(x => x.ToDeclaration()))}) {{");
        using (writer.Indent())
            writer.WriteLine($"super({string.Join(", ", SuperArguments.Select(x => x.ToCode()))});");
        writer.WriteLine("}");
    }
}
