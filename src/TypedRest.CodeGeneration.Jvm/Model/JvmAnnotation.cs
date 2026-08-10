namespace TypedRest.CodeGeneration.Jvm.Model;

/// <summary>
/// An annotation on a generated type or member, e.g. <c>@SerialName("first_name")</c>.
/// </summary>
/// <param name="identifier">The type of the annotation.</param>
public sealed class JvmAnnotation(JvmIdentifier identifier)
{
    /// <summary>
    /// The type of the annotation.
    /// </summary>
    public JvmIdentifier Identifier { get; } = identifier;

    /// <summary>
    /// Positional arguments, written in order before any <see cref="NamedArguments"/>.
    /// </summary>
    public List<string> Arguments { get; } = [];

    /// <summary>
    /// Named arguments. The value is written verbatim, so a string has to arrive already quoted.
    /// </summary>
    public List<(string name, string value)> NamedArguments { get; } = [];

    /// <summary>
    /// Writes the annotation, e.g. <c>@JsonClass(generateAdapter = true)</c>.
    /// </summary>
    public string Write()
    {
        var parts = Arguments.Select(JvmSyntax.Quote)
                             .Concat(NamedArguments.Select(x => $"{x.name} = {x.value}"))
                             .ToList();

        return parts.Count == 0
            ? "@" + Identifier.Name
            : $"@{Identifier.Name}({string.Join(", ", parts)})";
    }

    /// <summary>
    /// Returns every type that has to be imported to write this annotation.
    /// </summary>
    public IEnumerable<JvmIdentifier> GetImports()
        => Identifier.GetImports();
}
