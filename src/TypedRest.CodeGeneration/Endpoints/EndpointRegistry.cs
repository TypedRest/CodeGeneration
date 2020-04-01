using System;
using System.Collections.Generic;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.Endpoints.Raw;
using TypedRest.CodeGeneration.Endpoints.Reactive;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Endpoints
{
    /// <summary>
    /// A list of all known <see cref="IEndpoint"/> kinds.
    /// </summary>
    public class EndpointRegistry
    {
        /// <summary>
        /// Endpoint registry with the built-in default <see cref="IEndpoint"/> kinds.
        /// </summary>
        public static EndpointRegistry Default
            => new EndpointRegistry()
              .Add<Endpoint>()
              .Add<IndexerEndpoint>()
              .Add<CollectionEndpoint>()
              .Add<StreamingCollectionEndpoint>()
              .Add<ElementEndpoint>()
              .Add<ActionEndpoint>()
              .Add<ProducerEndpoint>()
              .Add<ConsumerEndpoint>()
              .Add<FunctionEndpoint>()
              .Add<PollingEndpoint>()
              .Add<StreamingEndpoint>()
              .Add<UploadEndpoint>()
              .Add<BlobEndpoint>();

        private readonly IDictionary<string, Func<IEndpoint>> _factories = new Dictionary<string, Func<IEndpoint>>();

        /// <summary>
        /// Adds <typeparamref name="T"/> to the list of known endpoint kinds.
        /// </summary>
        public EndpointRegistry Add<T>()
            where T : IEndpoint, new()
        {
            _factories.Add(new T().Kind, () => new T());
            return this;
        }

        /// <summary>
        /// Instantiates an <see cref="IEndpoint"/> of the specified <paramref name="kind"/>.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Endpoint kind not registered.</exception>
        public IEndpoint OfKind(string kind)
        {
            if (!_factories.TryGetValue(kind, out var factory))
                throw new KeyNotFoundException($"Unknown endpoint kind '{kind}'.");

            return factory();
        }
    }
}
