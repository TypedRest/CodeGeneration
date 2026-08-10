namespace TypedRest.CodeGeneration.Jvm.Model;

/// <summary>
/// An expression in generated code.
/// </summary>
public abstract class JvmExpression
{
    /// <summary>
    /// Returns every type that has to be imported to write this expression.
    /// </summary>
    public abstract IEnumerable<JvmIdentifier> GetImports();
}

/// <summary>
/// A reference to a local variable or parameter, e.g. <c>referrer</c>.
/// </summary>
/// <param name="name">The name of the variable.</param>
public sealed class JvmName(string name) : JvmExpression
{
    /// <summary>
    /// The name of the variable.
    /// </summary>
    public string Name { get; } = name;

    /// <inheritdoc/>
    public override IEnumerable<JvmIdentifier> GetImports() => [];
}

/// <summary>
/// The <c>this</c> reference, which every child endpoint passes as its referrer.
/// </summary>
public sealed class JvmThis : JvmExpression
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static readonly JvmThis Instance = new();

    private JvmThis() {}

    /// <inheritdoc/>
    public override IEnumerable<JvmIdentifier> GetImports() => [];
}

/// <summary>
/// A string literal, e.g. the relative URI of an endpoint.
/// </summary>
/// <param name="value">The value of the literal, unescaped.</param>
public sealed class JvmLiteral(string value) : JvmExpression
{
    /// <summary>
    /// The value of the literal, unescaped.
    /// </summary>
    public string Value { get; } = value;

    /// <inheritdoc/>
    public override IEnumerable<JvmIdentifier> GetImports() => [];
}

/// <summary>
/// A <c>java.net.URI</c> built from a string literal.
/// </summary>
/// <param name="value">The value of the URI, unescaped.</param>
public sealed class JvmUriLiteral(string value) : JvmExpression
{
    /// <summary>
    /// The value of the URI, unescaped.
    /// </summary>
    public string Value { get; } = value;

    /// <inheritdoc/>
    public override IEnumerable<JvmIdentifier> GetImports() => JvmIdentifier.Uri.GetImports();
}

/// <summary>
/// A class literal, which the endpoints need to deserialize their entities at runtime.
/// </summary>
/// <param name="type">The type to take the class literal of.</param>
public sealed class JvmClassLiteral(JvmIdentifier type) : JvmExpression
{
    /// <summary>
    /// The type to take the class literal of.
    /// </summary>
    public JvmIdentifier Type { get; } = type;

    /// <inheritdoc/>
    public override IEnumerable<JvmIdentifier> GetImports() => Type.GetImports();
}

/// <summary>
/// The construction of an object, e.g. <c>ContactElementEndpoint(this, "contacts")</c>.
/// </summary>
/// <param name="type">The type to construct.</param>
public sealed class JvmNew(JvmIdentifier type) : JvmExpression
{
    /// <summary>
    /// The type to construct.
    /// </summary>
    public JvmIdentifier Type { get; } = type;

    /// <summary>
    /// The arguments to pass to the constructor.
    /// </summary>
    public List<JvmExpression> Arguments { get; } = [];

    /// <inheritdoc/>
    public override IEnumerable<JvmIdentifier> GetImports()
        => Type.GetImports().Concat(Arguments.SelectMany(x => x.GetImports()));
}

/// <summary>
/// A lambda taking a referrer and a relative URI and returning an endpoint.
/// </summary>
/// <param name="body">The expression the lambda returns.</param>
public sealed class JvmElementFactory(JvmExpression body) : JvmExpression
{
    /// <summary>
    /// The name of the referrer parameter.
    /// </summary>
    public const string ReferrerParameter = "elementReferrer";

    /// <summary>
    /// The name of the relative URI parameter.
    /// </summary>
    public const string RelativeUriParameter = "elementUri";

    /// <summary>
    /// The expression the lambda returns, in terms of <see cref="ReferrerParameter"/> and <see cref="RelativeUriParameter"/>.
    /// </summary>
    public JvmExpression Body { get; } = body;

    /// <inheritdoc/>
    public override IEnumerable<JvmIdentifier> GetImports() => Body.GetImports();
}
