using JetBrains.Annotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Parses <see cref="IEndpoint"/>s in <see cref="OpenApiDocument"/>s.
    /// </summary>
    [PublicAPI]
    public interface IEndpointsParser
    {
        /// <summary>
        /// Parses an OpenAPI Object as an <see cref="IEndpoint"/>.
        /// </summary>
        /// <param name="data">The OpenAPI Object to parse.</param>
        /// <param name="defaultType">The default value to assume for <see cref="IEndpoint.Type"/> if it is not specified in <paramref name="data"/>.</param>
        [NotNull]
        IEndpoint Parse([NotNull] OpenApiObject data, [NotNull] string defaultType = "");
    }
}
