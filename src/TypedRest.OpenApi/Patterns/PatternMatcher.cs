using System.Collections.Generic;
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
        public PatternMatcher(PatternRegistry patterns)
        {
            _patterns = patterns;
        }

        public IDictionary<string, IEndpoint> GetEndpoints(PathTree tree)
        {
            var result = new Dictionary<string, IEndpoint>();
            foreach ((string path, var subTree) in tree.Children)
            {
                var endpoint = _patterns.Select(x => x.TryGetEndpoint(subTree, this))
                                        .FirstOrDefault(x => x != null);
                if (endpoint != null)
                {
                    endpoint.Uri = "./" + path;
                    result[path] = endpoint;
                }
            }
            return result;
        }
    }
}
