namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// Collects <see cref="GenerationMessage"/>s reported while generating a client.
/// </summary>
public interface IGenerationLog
{
    /// <summary>
    /// Reports a message.
    /// </summary>
    void Report(GenerationMessage message);
}

/// <summary>
/// An <see cref="IGenerationLog"/> that discards all messages.
/// </summary>
public sealed class NullGenerationLog : IGenerationLog
{
    /// <summary>
    /// The singleton instance.
    /// </summary>
    public static readonly NullGenerationLog Instance = new();

    private NullGenerationLog() {}

    /// <inheritdoc/>
    public void Report(GenerationMessage message) {}
}

/// <summary>
/// An <see cref="IGenerationLog"/> that keeps all messages in memory.
/// </summary>
public sealed class CollectingGenerationLog : IGenerationLog
{
    private readonly List<GenerationMessage> _messages = [];

    /// <summary>
    /// All messages reported so far, in the order they were reported.
    /// </summary>
    public IReadOnlyList<GenerationMessage> Messages => _messages;

    /// <inheritdoc/>
    public void Report(GenerationMessage message)
        => _messages.Add(message);
}
