using System.Collections;
using System.Collections.Generic;
using TypedRest.CodeGeneration.Patterns.Generic;
using TypedRest.CodeGeneration.Patterns.Raw;
using TypedRest.CodeGeneration.Patterns.Rpc;

namespace TypedRest.CodeGeneration.Patterns
{
    /// <summary>
    /// An ordered list of all known <see cref="IPattern"/>s.
    /// </summary>
    public class PatternRegistry : IEnumerable<IPattern>
    {
        /// <summary>
        /// Pattern registry with the built-in default <see cref="IPattern"/>s.
        /// </summary>
        public static PatternRegistry Default
            => new PatternRegistry()
              .Add(new DefaultPattern())
              .Add(new UploadPattern())
              .Add(new BlobPattern())
              .Add(new ActionPattern())
              .Add(new ProducerPattern())
              .Add(new ConsumerPattern())
              .Add(new FunctionPattern())
              .Add(new ElementPattern())
              .Add(new IndexerPattern())
              .Add(new CollectionPattern());

        private readonly Stack<IPattern> _patterns = new Stack<IPattern>();

        /// <summary>
        /// Adds <paramref name="pattern"/> to the top of the list of known patterns.
        /// </summary>
        public PatternRegistry Add(IPattern pattern)
        {
            _patterns.Push(pattern);
            return this;
        }

        public IEnumerator<IPattern> GetEnumerator()
            => _patterns.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
