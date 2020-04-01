using System.Collections.Generic;
using TypedRest.CodeGeneration.CSharp.Builders.Generic;
using TypedRest.CodeGeneration.CSharp.Builders.Raw;
using TypedRest.CodeGeneration.CSharp.Builders.Reactive;
using TypedRest.CodeGeneration.CSharp.Builders.Rpc;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Builders
{
    /// <summary>
    /// A list of all known <see cref="IBuilder"/>s.
    /// </summary>
    public class BuilderRegistry
    {
        /// <summary>
        /// Builder registry with the built-in default <see cref="IBuilder"/>s.
        /// </summary>
        public static BuilderRegistry Default
            => new BuilderRegistry()
              .Add(new DefaultBuilder())
              .Add(new ElementBuilder())
              .Add(new IndexerBuilder())
              .Add(new CollectionBuilder())
              .Add(new ActionBuilder())
              .Add(new ProducerBuilder())
              .Add(new ConsumerBuilder())
              .Add(new FunctionBuilder())
              .Add(new UploadBuilder())
              .Add(new BlobBuilder())
              .Add(new PollingBuilder())
              .Add(new StreamingBuilder())
              .Add(new StreamingCollectionBuilder());

        private readonly IDictionary<string, IBuilder> _builders = new Dictionary<string, IBuilder>();

        public BuilderRegistry()
        {
            // Must always be registered
            Add(new EntryBuilder());
        }

        /// <summary>
        /// Adds <paramref name="builder"/> to the list of known builders.
        /// </summary>
        public BuilderRegistry Add<TEndpoint>(IBuilder<TEndpoint> builder)
            where TEndpoint : IEndpoint, new()
        {
            _builders.Add(new TEndpoint().Kind, builder);
            return this;
        }

        /// <summary>
        /// Returns an <see cref="IBuilder"/> suitable for the kind of <paramref name="endpoint"/>.
        /// </summary>
        /// <exception cref="KeyNotFoundException">No builder matching the endpoint's kind found.</exception>
        public IBuilder For(IEndpoint endpoint)
        {
            if (!_builders.TryGetValue(endpoint.Kind, out var builder))
                throw new KeyNotFoundException($"No builder registered for endpoint kind '{endpoint.Kind}'.");

            return builder;
        }
    }
}
