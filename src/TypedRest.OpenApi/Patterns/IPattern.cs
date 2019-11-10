using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// Generates <see cref="IEndpoint"/>s from <see cref="PathTree"/>s if they match a specific pattern.
    /// </summary>
    public interface IPattern
    {
        /// <summary>
        /// Generates an endpoint for the specified path tree, if it matches the pattern.
        /// </summary>
        /// <param name="tree">The path tree to try to match against the pattern.</param>
        /// <param name="patternMatcher">The pattern matcher to use for matching child trees.</param>
        /// <returns>The endpoint if the pattern matched; <c>null</c> otherwise.</returns>
        IEndpoint? TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher);
    }
}
