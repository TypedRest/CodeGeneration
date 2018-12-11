using Microsoft.OpenApi.Any;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi
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
        /// <param name="parser">The endpoint parser to use for parsing child objects.</param>
        void Parse(OpenApiObject data, IEndpointsParser parser);
    }
}
