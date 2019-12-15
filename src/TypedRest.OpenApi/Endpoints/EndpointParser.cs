using System;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Parses <see cref="IEndpoint"/>s in <see cref="OpenApiDocument"/>s.
    /// </summary>
    public class EndpointParser : IEndpointParser
    {
        private readonly EndpointRegistry _endpointRegistry;

        /// <summary>
        /// Creates an endpoint parser.
        /// </summary>
        /// <param name="endpointRegistry">A list of all known <see cref="IEndpoint"/> kinds.</param>
        public EndpointParser(EndpointRegistry? endpointRegistry = null)
        {
            _endpointRegistry = endpointRegistry ?? EndpointRegistry.Default;
        }

        public IEndpoint Parse(OpenApiObject data, string defaultKind = "")
        {
            var endpoint = _endpointRegistry.OfKind(data.GetString("kind") ?? defaultKind);
            endpoint.Parse(data, this);
            return endpoint;
        }

        /// <summary>
        /// Parses the TypedRest OpenAPI Spec Extension.
        /// </summary>
        /// <param name="data">The OpenAPI Object to parse.</param>
        /// <param name="specVersion">The OpenAPI Spec version.</param>
        public IOpenApiExtension Parse(IOpenApiAny data, OpenApiSpecVersion specVersion)
        {
            if (!(data is OpenApiObject objData)) throw new FormatException($"{OpenApiDocumentExtensions.TypedRestKey} is not an object.");

            var entryEndpoint = new EntryEndpoint();
            entryEndpoint.Parse(objData, this);
            return entryEndpoint;
        }
    }
}
