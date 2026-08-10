namespace TypedRest.CodeGeneration.Jvm.Model;

/// <summary>
/// A reference to a JVM type.
/// </summary>
public sealed class JvmIdentifier
{
    /// <summary>
    /// Creates a new type reference.
    /// </summary>
    /// <param name="package">The package the type lives in, or <c>null</c> for a primitive/built-in.</param>
    /// <param name="name">The simple name of the type.</param>
    /// <param name="nullable">Indicates whether the type can have the value <c>null</c>.</param>
    public JvmIdentifier(JvmPackage? package, string name, bool nullable = false)
    {
        Package = package;
        Name = name;
        Nullable = nullable;
    }

    private JvmIdentifier(JvmIdentifier other, bool nullable)
        : this(other.Package, other.Name, nullable)
    {
        Kind = other.Kind;
        TypeArguments.AddRange(other.TypeArguments);
    }

    /// <summary>
    /// The package the type lives in, or <c>null</c> for a primitive/built-in.
    /// </summary>
    public JvmPackage? Package { get; }

    /// <summary>
    /// The simple name of the type, without the package.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Indicates whether the type can have the value <c>null</c>.
    /// </summary>
    public bool Nullable { get; }

    /// <summary>
    /// What kind of type this is, for the cases the writers have to treat specially.
    /// </summary>
    public JvmTypeKind Kind { get; private set; } = JvmTypeKind.Class;

    /// <summary>
    /// Generic type arguments for the type.
    /// </summary>
    public List<JvmIdentifier> TypeArguments { get; } = [];

    /// <summary>
    /// The fully qualified name, e.g. <c>java.util.List</c>.
    /// </summary>
    public string QualifiedName
        => Package is null or {Name.Length: 0} ? Name : Package.Name + "." + Name;

    /// <summary>
    /// Returns a copy of the type reference that can have the value <c>null</c>.
    /// </summary>
    public JvmIdentifier ToNullable()
        => Nullable ? this : new JvmIdentifier(this, nullable: true);

    /// <summary>
    /// Returns a copy of the type reference that cannot have the value <c>null</c>.
    /// </summary>
    public JvmIdentifier ToNonNullable()
        => Nullable ? new JvmIdentifier(this, nullable: false) : this;

    /// <summary>
    /// Returns a copy of the type reference with <paramref name="typeArguments"/> applied.
    /// </summary>
    public JvmIdentifier WithTypeArguments(params JvmIdentifier[] typeArguments)
    {
        var result = new JvmIdentifier(this, Nullable);
        result.TypeArguments.Clear();
        result.TypeArguments.AddRange(typeArguments);
        return result;
    }

    /// <summary>The <c>java.lang.String</c> type.</summary>
    public static JvmIdentifier String => new(Packages.JavaLang, "String");

    /// <summary>The boxed <c>java.lang.Integer</c> type.</summary>
    public static JvmIdentifier Int => new(null, "Int") {Kind = JvmTypeKind.Int};

    /// <summary>The boxed <c>java.lang.Long</c> type.</summary>
    public static JvmIdentifier Long => new(null, "Long") {Kind = JvmTypeKind.Long};

    /// <summary>The boxed <c>java.lang.Double</c> type.</summary>
    public static JvmIdentifier Double => new(null, "Double") {Kind = JvmTypeKind.Double};

    /// <summary>The boxed <c>java.lang.Boolean</c> type.</summary>
    public static JvmIdentifier Boolean => new(null, "Boolean") {Kind = JvmTypeKind.Boolean};

    /// <summary>The <c>java.net.URI</c> type, which every endpoint constructor takes.</summary>
    public static JvmIdentifier Uri => new(Packages.JavaNet, "URI");

    /// <summary>The <c>java.io.InputStream</c> type, used for blob and upload endpoints.</summary>
    public static JvmIdentifier InputStream => new(Packages.JavaIo, "InputStream");

    /// <summary>The <c>java.time.OffsetDateTime</c> type, used for <c>date-time</c> formats.</summary>
    public static JvmIdentifier OffsetDateTime => new(Packages.JavaTime, "OffsetDateTime");

    /// <summary>The <c>java.time.LocalDate</c> type, used for <c>date</c> formats.</summary>
    public static JvmIdentifier LocalDate => new(Packages.JavaTime, "LocalDate");

    /// <summary>The <c>java.util.UUID</c> type, used for <c>uuid</c> formats.</summary>
    public static JvmIdentifier Uuid => new(Packages.JavaUtil, "UUID");

    /// <summary>The fallback for schemas that carry no usable type information.</summary>
    public static JvmIdentifier Object => new(Packages.JavaLang, "Object") {Kind = JvmTypeKind.Object};

    /// <summary>
    /// A <c>java.util.List</c> of <paramref name="item"/>.
    /// </summary>
    public static JvmIdentifier ListOf(JvmIdentifier item)
        => new JvmIdentifier(Packages.JavaUtil, "List") {Kind = JvmTypeKind.List}.WithTypeArguments(item);

    /// <summary>
    /// A <c>java.util.Map</c> from <c>String</c> to <paramref name="value"/>.
    /// </summary>
    public static JvmIdentifier MapOf(JvmIdentifier value)
        => new JvmIdentifier(Packages.JavaUtil, "Map") {Kind = JvmTypeKind.Map}.WithTypeArguments(String, value);

    /// <summary>
    /// Returns every type that has to be imported to reference this type.
    /// </summary>
    public IEnumerable<JvmIdentifier> GetImports()
    {
        if (Package != null) yield return this;

        foreach (var import in TypeArguments.SelectMany(x => x.GetImports()))
            yield return import;
    }

    /// <inheritdoc/>
    public override string ToString()
        => TypeArguments.Count == 0
            ? QualifiedName
            : $"{QualifiedName}<{string.Join(", ", TypeArguments)}>";
}

/// <summary>
/// The kinds of type a <see cref="JvmIdentifier"/> can refer to, limited to the distinctions the writers act on.
/// </summary>
public enum JvmTypeKind
{
    /// <summary>An ordinary class or interface.</summary>
    Class,

    /// <summary>A 32-bit integer, written <c>Int</c> in Kotlin and <c>Integer</c>/<c>int</c> in Java.</summary>
    Int,

    /// <summary>A 64-bit integer, written <c>Long</c> in both but boxed differently in Java.</summary>
    Long,

    /// <summary>A double-precision float.</summary>
    Double,

    /// <summary>A boolean.</summary>
    Boolean,

    /// <summary>The root type, written <c>Any</c> in Kotlin and <c>Object</c> in Java.</summary>
    Object,

    /// <summary>A <c>java.util.List</c>.</summary>
    List,

    /// <summary>A <c>java.util.Map</c>.</summary>
    Map
}
