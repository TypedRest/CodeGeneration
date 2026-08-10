using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// Maps each <see cref="IEndpoint.Kind"/> to the builder emitting code for it, holding at most one builder per kind.
/// </summary>
/// <typeparam name="TBuilder">The target language's builder interface.</typeparam>
public class BuilderRegistry<TBuilder>
    where TBuilder : notnull
{
    private readonly Dictionary<string, TBuilder> _builders = new();

    /// <summary>
    /// Registers <paramref name="builder"/> for the kind of <typeparamref name="TEndpoint"/>.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> the builder generates code for.</typeparam>
    /// <exception cref="ArgumentException">A builder is already registered for that kind.</exception>
    /// <remarks>Deliberately not called <c>Add</c>, so that a derived fluent <c>Add</c> taking a more specific
    /// builder type cannot accidentally bind to itself.</remarks>
    protected void Register<TEndpoint>(TBuilder builder)
        where TEndpoint : IEndpoint, new()
        => _builders.Add(new TEndpoint().Kind, builder);

    /// <summary>
    /// Returns the builder suitable for the kind of <paramref name="endpoint"/>.
    /// </summary>
    /// <exception cref="KeyNotFoundException">No builder matching the endpoint's kind found.</exception>
    public TBuilder For(IEndpoint endpoint)
    {
        if (!_builders.TryGetValue(endpoint.Kind, out var builder))
            throw new KeyNotFoundException($"No builder registered for endpoint kind '{endpoint.Kind}'.");

        return builder;
    }
}
