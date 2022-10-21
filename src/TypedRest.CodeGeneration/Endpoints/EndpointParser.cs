namespace TypedRest.CodeGeneration.Endpoints;

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
    public EndpointParser(EndpointRegistry endpointRegistry)
    {
        _endpointRegistry = endpointRegistry;
    }

    public IEndpoint Parse(OpenApiObject data, string defaultKind = "")
    {
        var endpoint = _endpointRegistry.OfKind(data.GetString("kind") ?? defaultKind);
        endpoint.Parse(data, this);
        return endpoint;
    }
}
