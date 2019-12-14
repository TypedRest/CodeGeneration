using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Can be filled with information parsed from an OpenAPI Object.
    /// </summary>
    public interface IParsable
    {
        /// <summary>
        /// Fills the object with information parsed from an OpenAPI Object.
        /// </summary>
        /// <param name="data">The OpenAPI Object to parse.</param>
        /// <param name="endpointParser">The endpoint parser to use for parsing child objects.</param>
        void Parse(OpenApiObject data, IEndpointParser endpointParser);

        /// <summary>
        /// Resolves <see cref="OpenApiReference"/>s.
        /// </summary>
        /// <param name="components">The components that references can point to.</param>
        void ResolveReferences(OpenApiComponents components);
    }
}
