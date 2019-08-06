using JetBrains.Annotations;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// Matches a set of <see cref="IPattern"/>s against a path tree.
    /// </summary>
    [PublicAPI]
    public interface IPatternMatcher
    {
        /// <summary>
        /// Generates endpoints for the specified <paramref name="tree"/>.
        /// </summary>
        [NotNull]
        EndpointList GetEndpoints([NotNull] PathTree tree);
    }
}
