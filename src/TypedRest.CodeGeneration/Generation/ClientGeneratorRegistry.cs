namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// A list of all known <see cref="IClientGenerator"/>s, keyed by <see cref="IClientGenerator.Language"/>.
/// </summary>
public class ClientGeneratorRegistry
{
    private readonly Dictionary<string, IClientGenerator> _generators = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Adds <paramref name="generator"/> to the list of known generators.
    /// </summary>
    public ClientGeneratorRegistry Add(IClientGenerator generator)
    {
        _generators.Add(generator.Language, generator);
        return this;
    }

    /// <summary>
    /// The names of all known target languages.
    /// </summary>
    public IEnumerable<string> Languages => _generators.Keys;

    /// <summary>
    /// Tries to get the <see cref="IClientGenerator"/> for <paramref name="language"/>.
    /// </summary>
    public bool TryGet(string language, out IClientGenerator generator)
        => _generators.TryGetValue(language, out generator!);

    /// <summary>
    /// Returns the <see cref="IClientGenerator"/> for <paramref name="language"/>.
    /// </summary>
    /// <exception cref="KeyNotFoundException">No generator registered for the language.</exception>
    public IClientGenerator For(string language)
    {
        if (!_generators.TryGetValue(language, out var generator))
            throw new KeyNotFoundException($"No client generator registered for language '{language}'.");

        return generator;
    }
}
