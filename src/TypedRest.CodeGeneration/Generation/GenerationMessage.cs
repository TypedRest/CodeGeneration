namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// How important a <see cref="GenerationMessage"/> is.
/// </summary>
public enum GenerationSeverity
{
    /// <summary>
    /// Describes a decision the generator made that the user may want to know about.
    /// </summary>
    Info,

    /// <summary>
    /// Describes something in the document that the target language cannot fully express.
    /// </summary>
    Warning
}

/// <summary>
/// A message reported by an <see cref="IClientGenerator"/> while generating a client.
/// </summary>
public sealed class GenerationMessage
{
    /// <summary>
    /// Creates a new generation message.
    /// </summary>
    /// <param name="severity">How important the message is.</param>
    /// <param name="code">A stable identifier for the kind of message, e.g. <c>TRCG101</c>.</param>
    /// <param name="text">A human-readable description.</param>
    /// <param name="endpointKey">The key of the endpoint the message is about, if any.</param>
    public GenerationMessage(GenerationSeverity severity, string code, string text, string? endpointKey = null)
    {
        Severity = severity;
        Code = code;
        Text = text;
        EndpointKey = endpointKey;
    }

    /// <summary>
    /// How important the message is.
    /// </summary>
    public GenerationSeverity Severity { get; }

    /// <summary>
    /// A stable identifier for the kind of message, e.g. <c>TRCG101</c>.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// A human-readable description.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// The key of the endpoint the message is about, if any.
    /// </summary>
    public string? EndpointKey { get; }

    /// <inheritdoc/>
    public override string ToString()
        => $"{Code}: {Text}";
}
