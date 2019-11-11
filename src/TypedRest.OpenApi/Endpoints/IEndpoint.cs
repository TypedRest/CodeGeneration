using Microsoft.OpenApi.Interfaces;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Represents a TypedRest endpoint.
    /// </summary>
    public interface IEndpoint : IOpenApiSerializable, IOpenApiExtension, IParsable
    {
        /// <summary>
        /// The type of endpoint.
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
        EndpointList Children { get; }
    }
}
