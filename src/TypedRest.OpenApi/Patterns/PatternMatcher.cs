using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// Matches a set of <see cref="IPattern"/>s against a path tree.
    /// </summary>
    [PublicAPI]
    public class PatternMatcher : IPatternMatcher, IEnumerable<IPattern>
    {
        private readonly Stack<IPattern> _patterns = new Stack<IPattern>();

        /// <summary>
        /// Creates a pattern matcher with the built-in default patterns.
        /// </summary>
        public PatternMatcher()
        {
            Add(new DefaultPattern());
            Add(new IndexerPattern());
            Add(new CollectionPattern());
            Add(new ElementPattern());
            Add(new ActionPattern());
            Add(new ProducerPattern());
            Add(new ConsumerPattern());
            Add(new FunctionPattern());
            Add(new UploadPattern());
            Add(new BlobPattern());
        }

        /// <summary>
        /// Adds the <paramref name="pattern"/> to the list of known patterns.
        /// </summary>
        public PatternMatcher Add([NotNull] IPattern pattern)
        {
            _patterns.Push(pattern);
            return this;
        }

        public IEnumerator<IPattern> GetEnumerator()
            => _patterns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IDictionary<string, IEndpoint> GetEndpoints(IDictionary<string, PathTree> tree)
            => tree.ToDictionary(node => node.Key, node =>
            {
                var endpoint = _patterns.Select(x => x.TryGetEndpoint(node.Value, this))
                                        .First(x => x != null);
                endpoint.Uri = "./" + node.Key;
                return endpoint;
            });
    }
}
