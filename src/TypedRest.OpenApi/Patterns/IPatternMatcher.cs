using System.Collections.Generic;
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
        /// Generates named endpoints for the specified <paramref name="tree"/> using the registered <see cref="IPattern"/>s.
        /// </summary>
        [NotNull]
        IDictionary<string, IEndpoint> GetEndpoints([NotNull] IDictionary<string, PathTree> tree);
    }
}
