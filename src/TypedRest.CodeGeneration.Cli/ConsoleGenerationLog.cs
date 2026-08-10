using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.Cli;

/// <summary>
/// Writes <see cref="GenerationMessage"/>s to standard error.
/// </summary>
public class ConsoleGenerationLog : IGenerationLog
{
    /// <inheritdoc/>
    public void Report(GenerationMessage message)
        => Console.Error.WriteLine($"{(message.Severity == GenerationSeverity.Warning ? "Warning" : "Info")}: {message.Code}: {message.Text}");
}
