using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Patterns.Generic;
using TypedRest.OpenApi.Patterns.Raw;
using TypedRest.OpenApi.Patterns.Rpc;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// Matches a set of <see cref="IPattern"/>s against a path tree.
    /// </summary>
    public class PatternMatcher : IPatternMatcher, IEnumerable<IPattern>
    {
        private readonly Stack<IPattern> _patterns = new Stack<IPattern>();

        /// <summary>
        /// Creates a pattern matcher with the built-in default patterns.
        /// </summary>
        public PatternMatcher()
        {
            // ordered from generic to specific
            Add(new DefaultPattern());
            Add(new UploadPattern());
            Add(new BlobPattern());
            Add(new ActionPattern());
            Add(new ProducerPattern());
            Add(new ConsumerPattern());
            Add(new FunctionPattern());
            Add(new ElementPattern());
            Add(new IndexerPattern());
            Add(new CollectionPattern());
        }

        /// <summary>
        /// Adds the <paramref name="pattern"/> to the list of known patterns.
        /// </summary>
        public PatternMatcher Add(IPattern pattern)
        {
            _patterns.Push(pattern);
            return this;
        }

        public IEnumerator<IPattern> GetEnumerator()
            => _patterns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

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
