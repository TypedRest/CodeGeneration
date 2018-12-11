using JetBrains.Annotations;
using Microsoft.OpenApi.Interfaces;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Represents a TypedRest endpoint.
    /// </summary>
    [PublicAPI]
    public interface IEndpoint : IOpenApiSerializable, IOpenApiExtension, IParsable
    {
        /// <summary>
        /// The type of endpoint.
        /// </summary>
        [NotNull]
        string Type { get; }

        /// <summary>
        /// A short description of the endpoint.
        /// </summary>
        [CanBeNull]
        string Description { get; set; }

        /// <summary>
        /// The relative URI of the endpoint.
        /// </summary>
        [CanBeNull]
        string Uri { get; set; }

        /// <summary>
        /// The child endpoints of this endpoint.
        /// </summary>
        [NotNull]
        EndpointList Children { get; }
    }
}
