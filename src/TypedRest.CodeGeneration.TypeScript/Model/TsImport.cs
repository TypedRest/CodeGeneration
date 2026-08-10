namespace TypedRest.CodeGeneration.TypeScript.Model;

/// <summary>
/// A single name imported from a <see cref="TsModule"/>.
/// </summary>
public sealed class TsImport : IEquatable<TsImport>
{
    /// <summary>
    /// Creates a new import.
    /// </summary>
    /// <param name="module">The module to import from.</param>
    /// <param name="name">The name to import.</param>
    public TsImport(TsModule module, string name)
    {
        Module = module;
        Name = name;
    }

    /// <summary>
    /// The module to import from.
    /// </summary>
    public TsModule Module { get; }

    /// <summary>
    /// The name to import.
    /// </summary>
    public string Name { get; }

    /// <inheritdoc/>
    public bool Equals(TsImport? other)
        => other != null && Module.Equals(other.Module) && Name == other.Name;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is TsImport other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        unchecked
        {
            return (Module.GetHashCode() * 397) ^ Name.GetHashCode();
        }
    }

    /// <inheritdoc/>
    public override string ToString()
        => $"{Name} from {Module}";
}
