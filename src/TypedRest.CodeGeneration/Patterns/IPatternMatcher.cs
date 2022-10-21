using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.Patterns;

/// <summary>
/// Matches a set of <see cref="IPattern"/>s against a path tree.
/// </summary>
public interface IPatternMatcher
{
    /// <summary>
    /// Generates endpoints for the specified <paramref name="tree"/>.
    /// </summary>
    IDictionary<string, IEndpoint> GetEndpoints(PathTree tree);
}
