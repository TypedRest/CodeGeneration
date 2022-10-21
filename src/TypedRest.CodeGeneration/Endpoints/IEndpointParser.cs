namespace TypedRest.CodeGeneration.Endpoints;

/// <summary>
/// Parses <see cref="IEndpoint"/>s in <see cref="OpenApiDocument"/>s.
/// </summary>
public interface IEndpointParser
{
    /// <summary>
    /// Parses an OpenAPI Object as an <see cref="IEndpoint"/>.
    /// </summary>
    /// <param name="data">The OpenAPI Object to parse.</param>
    /// <param name="defaultKind">The default value to assume for <see cref="IEndpoint.Kind"/> if it is not specified in <paramref name="data"/>.</param>
    IEndpoint Parse(OpenApiObject data, string defaultKind = "");
}
