using System.Linq;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// Matches a set of <see cref="IPattern"/>s against a path tree.
    /// </summary>
    public class PatternMatcher : IPatternMatcher
    {
        private readonly PatternRegistry _patterns;

        /// <summary>
        /// Creates a pattern matcher.
        /// </summary>
        /// <param name="patterns">An ordered list of all known <see cref="IPattern"/>s.</param>
        public PatternMatcher(PatternRegistry? patterns = null)
        {
            _patterns = patterns ?? PatternRegistry.Default;
        }

        public EndpointList GetEndpoints(PathTree tree)
        {
            var result = new EndpointList();
            foreach (var pair in tree.Children)
            {
                var endpoint = _patterns.Select(x => x.TryGetEndpoint(pair.Value, this))
                                        .First(x => x != null);
                endpoint!.Uri = "./" + pair.Key;

                result[pair.Key] = endpoint;
            }
            return result;
        }
    }
}
