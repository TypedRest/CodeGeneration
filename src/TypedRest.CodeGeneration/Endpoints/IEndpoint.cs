using System.Collections.Generic;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace TypedRest.CodeGeneration.Endpoints
{
    /// <summary>
    /// Represents a TypedRest endpoint.
    /// </summary>
    public interface IEndpoint : IOpenApiSerializable, IOpenApiExtension
    {
        /// <summary>
        /// Fills the endpoint with information parsed from an OpenAPI Object.
        /// </summary>
        /// <param name="data">The OpenAPI Object to parse.</param>
        /// <param name="parser">The endpoint parser to use for parsing child objects.</param>
        void Parse(OpenApiObject data, IEndpointParser parser);

        /// <summary>
        /// Resolves <see cref="OpenApiReference"/>s.
        /// </summary>
        /// <param name="components">The components that references can point to.</param>
        void ResolveReferences(OpenApiComponents components);

        /// <summary>
        /// The kind/type of endpoint.
        /// </summary>
        string Kind { get; }

        /// <summary>
        /// A short description of the endpoint.
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// The relative URI of the endpoint.
        /// </summary>
        string? Uri { get; set; }

        /// <summary>
        /// The child endpoints of this endpoint.
        /// </summary>
        IDictionary<string, IEndpoint>  Children { get; }
    }
}
