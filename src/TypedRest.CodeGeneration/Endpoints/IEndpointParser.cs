namespace TypedRest.CodeGeneration.Endpoints;

/// <summary>
/// Parses <see cref="IEndpoint"/>s in <see cref="OpenApiDocument"/>s.
/// </summary>
public interface IEndpointParser
{
    /// <summary>
    /// Parses a JSON object as an <see cref="IEndpoint"/>.
    /// </summary>
    /// <param name="data">The JSON object to parse.</param>
    /// <param name="defaultKind">The default value to assume for <see cref="IEndpoint.Kind"/> if it is not specified in <paramref name="data"/>.</param>
    IEndpoint Parse(JsonObject data, string defaultKind = "");
}
