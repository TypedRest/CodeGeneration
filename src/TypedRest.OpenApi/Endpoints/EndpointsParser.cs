using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Parses <see cref="IEndpoint"/>s in <see cref="OpenApiDocument"/>s.
    /// </summary>
    public class EndpointsParser : IEndpointsParser, IEndpointsParserSetup
    {
        private readonly IDictionary<string, Func<IEndpoint>> _endpointFactories = new Dictionary<string, Func<IEndpoint>>();

        /// <summary>
        /// Creates an endpoint parser with the built-in default endpoint types.
        /// </summary>
        public EndpointsParser()
        {
            Add<Endpoint>();
            Add<IndexerEndpoint>();
            Add<CollectionEndpoint>();
            Add<StreamingCollectionEndpoint>();
            Add<ElementEndpoint>();
            Add<ActionEndpoint>();
            Add<ProducerEndpoint>();
            Add<ConsumerEndpoint>();
            Add<FunctionEndpoint>();
            Add<PollingEndpoint>();
            Add<StreamingEndpoint>();
            Add<UploadEndpoint>();
            Add<BlobEndpoint>();
        }

        public IEndpointsParserSetup Add<T>()
            where T : IEndpoint, new()
        {
            _endpointFactories.Add(new T().Type, () => new T());
            return this;
        }

        public IEndpoint Parse(OpenApiObject data, string defaultType = "")
        {
            string type = data.GetString("type") ?? defaultType;
            if (!_endpointFactories.TryGetValue(type, out var factory))
                throw new FormatException($"Unknown endpoint type '{type}'.");

            var endpoint = factory();
            endpoint.Parse(data, this);
            return endpoint;
        }

        /// <summary>
        /// Parses the TypedRest OpenAPI Spec Extension.
        /// </summary>
        /// <param name="data">The OpenAPI Object to parse.</param>
        /// <param name="specVersion">The OpenAPI Spec version.</param>
        [NotNull]
        public IOpenApiExtension Parse([NotNull] IOpenApiAny data, OpenApiSpecVersion specVersion)
        {
            if (!(data is OpenApiObject objData)) throw new FormatException($"{EndpointList.ExtensionKey} is not an object.");

            var result = new EndpointList();
            result.Parse(objData, this);
            return result;
        }
    }
}
