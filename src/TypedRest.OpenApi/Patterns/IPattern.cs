using JetBrains.Annotations;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// A pattern that can be used to automatically generate an <see cref="IEndpoint"/> from a <see cref="PathTree"/>.
    /// </summary>
    [PublicAPI]
    public interface IPattern
    {
        /// <summary>
        /// Generates an endpoint for the specified path tree, if it matches the pattern.
        /// </summary>
        /// <param name="tree">The path tree to try to match against the pattern.</param>
        /// <param name="patternMatcher">The pattern matcher to use for matching child trees.</param>
        /// <returns>The endpoint if the pattern matched; <c>null</c> otherwise.</returns>
        [CanBeNull]
        IEndpoint TryGetEndpoint([NotNull] PathTree tree, [NotNull] IPatternMatcher patternMatcher);
    }
}
