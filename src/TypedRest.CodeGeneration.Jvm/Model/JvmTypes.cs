namespace TypedRest.CodeGeneration.Jvm.Model;

/// <summary>
/// A type declaration that can be written to a file of its own.
/// </summary>
public interface IJvmType
{
    /// <summary>
    /// The name of the type and the package it is declared in.
    /// </summary>
    JvmIdentifier Identifier { get; }

    /// <summary>
    /// A description of the type for a JavaDoc/KDoc comment.
    /// </summary>
    string? Summary { get; set; }

    /// <summary>
    /// Marks the type as deprecated.
    /// </summary>
    bool Deprecated { get; set; }

    /// <summary>
    /// Returns every type that has to be imported by the file declaring this type.
    /// </summary>
    IEnumerable<JvmIdentifier> GetImports();
}

/// <summary>
/// A generated endpoint class, deriving from one of the TypedRest endpoint implementations.
/// </summary>
/// <param name="identifier">The name of the class and the package it is declared in.</param>
public sealed class JvmEndpointClass(JvmIdentifier identifier) : IJvmType
{
    /// <inheritdoc/>
    public JvmIdentifier Identifier { get; } = identifier;

    /// <inheritdoc/>
    public string? Summary { get; set; }

    /// <inheritdoc/>
    public bool Deprecated { get; set; }

    /// <summary>
    /// The TypedRest endpoint implementation this class derives from.
    /// </summary>
    public JvmIdentifier BaseType { get; set; } = Packages.EntryEndpoint;

    /// <summary>
    /// The constructor of the class, or <c>null</c> to inherit the base constructor.
    /// </summary>
    public JvmConstructor? Constructor { get; set; }

    /// <summary>
    /// The child endpoints exposed by the class.
    /// </summary>
    public List<JvmChildEndpoint> Children { get; } = [];

    /// <inheritdoc/>
    public IEnumerable<JvmIdentifier> GetImports()
    {
        foreach (var package in BaseType.GetImports()) yield return package;

        if (Constructor != null)
        {
            foreach (var package in Constructor.GetImports()) yield return package;
        }

        foreach (var package in Children.SelectMany(x => x.GetImports())) yield return package;
    }
}

/// <summary>
/// A child endpoint exposed by a generated endpoint class.
/// </summary>
/// <param name="name">The name of the member.</param>
/// <param name="type">The type of the member.</param>
/// <param name="value">The expression the member is initialized with.</param>
public sealed class JvmChildEndpoint(string name, JvmIdentifier type, JvmExpression value)
{
    /// <summary>
    /// The name of the member.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The type of the member.
    /// </summary>
    public JvmIdentifier Type { get; } = type;

    /// <summary>
    /// The expression the member is initialized with.
    /// </summary>
    public JvmExpression Value { get; } = value;

    /// <summary>
    /// A description of the member for a JavaDoc/KDoc comment.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Marks the member as deprecated.
    /// </summary>
    public bool Deprecated { get; set; }

    /// <summary>
    /// Returns every type that has to be imported to declare this member.
    /// </summary>
    public IEnumerable<JvmIdentifier> GetImports()
        => Type.GetImports().Concat(Value.GetImports());
}

/// <summary>
/// The constructor of a generated endpoint class.
/// </summary>
public sealed class JvmConstructor
{
    /// <summary>
    /// The parameters of the constructor.
    /// </summary>
    public List<JvmParameter> Parameters { get; } = [];

    /// <summary>
    /// The arguments passed on to the base constructor.
    /// </summary>
    public List<JvmExpression> BaseArguments { get; } = [];

    /// <summary>
    /// Returns every type that has to be imported to declare this constructor.
    /// </summary>
    public IEnumerable<JvmIdentifier> GetImports()
        => Parameters.SelectMany(x => x.Type.GetImports())
                     .Concat(BaseArguments.SelectMany(x => x.GetImports()));
}

/// <summary>
/// A parameter of a generated constructor.
/// </summary>
/// <param name="name">The name of the parameter.</param>
/// <param name="type">The type of the parameter.</param>
public sealed class JvmParameter(string name, JvmIdentifier type)
{
    /// <summary>
    /// The name of the parameter.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The type of the parameter.
    /// </summary>
    public JvmIdentifier Type { get; } = type;
}

/// <summary>
/// A generated DTO.
/// </summary>
/// <param name="identifier">The name of the type and the package it is declared in.</param>
public sealed class JvmDto(JvmIdentifier identifier) : IJvmType
{
    /// <inheritdoc/>
    public JvmIdentifier Identifier { get; } = identifier;

    /// <inheritdoc/>
    public string? Summary { get; set; }

    /// <inheritdoc/>
    public bool Deprecated { get; set; }

    /// <summary>
    /// The properties of the DTO, in the order they were declared in the document.
    /// </summary>
    public List<JvmDtoProperty> Properties { get; } = [];

    /// <inheritdoc/>
    public IEnumerable<JvmIdentifier> GetImports()
        => Properties.SelectMany(x => x.GetImports());
}

/// <summary>
/// A property of a generated DTO.
/// </summary>
/// <param name="name">The name of the property in generated code.</param>
/// <param name="wireName">The name of the property on the wire.</param>
/// <param name="type">The type of the property.</param>
public sealed class JvmDtoProperty(string name, string wireName, JvmIdentifier type)
{
    /// <summary>
    /// The name of the property in generated code.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The name of the property on the wire, which the serializer annotation carries when it differs from <see cref="Name"/>.
    /// </summary>
    public string WireName { get; } = wireName;

    /// <summary>
    /// The type of the property.
    /// </summary>
    public JvmIdentifier Type { get; } = type;

    /// <summary>
    /// A description of the property for a JavaDoc/KDoc comment.
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// Marks the property as deprecated.
    /// </summary>
    public bool Deprecated { get; set; }

    /// <summary>
    /// Indicates whether the document marks this property as required.
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// Returns every type that has to be imported to declare this property.
    /// </summary>
    public IEnumerable<JvmIdentifier> GetImports()
        => Type.GetImports();
}

/// <summary>
/// A generated enum.
/// </summary>
/// <param name="identifier">The name of the type and the package it is declared in.</param>
public sealed class JvmEnum(JvmIdentifier identifier) : IJvmType
{
    /// <inheritdoc/>
    public JvmIdentifier Identifier { get; } = identifier;

    /// <inheritdoc/>
    public string? Summary { get; set; }

    /// <inheritdoc/>
    public bool Deprecated { get; set; }

    /// <summary>
    /// The values of the enum.
    /// </summary>
    public List<JvmEnumValue> Values { get; } = [];

    /// <inheritdoc/>
    public IEnumerable<JvmIdentifier> GetImports() => [];
}

/// <summary>
/// A value of a generated enum.
/// </summary>
/// <param name="name">The name of the value in generated code.</param>
/// <param name="wireName">The name of the value on the wire.</param>
public sealed class JvmEnumValue(string name, string wireName)
{
    /// <summary>
    /// The name of the value in generated code.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The name of the value on the wire, which the serializer annotation carries when it differs from <see cref="Name"/>.
    /// </summary>
    public string WireName { get; } = wireName;

    /// <summary>
    /// A description of the value for a JavaDoc/KDoc comment.
    /// </summary>
    public string? Summary { get; set; }
}
