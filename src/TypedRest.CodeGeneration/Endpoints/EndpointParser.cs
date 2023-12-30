namespace TypedRest.CodeGeneration.Endpoints;

/// <summary>
/// Parses <see cref="IEndpoint"/>s in <see cref="OpenApiDocument"/>s.
/// </summary>
/// <param name="endpointRegistry">A list of all known <see cref="IEndpoint"/> kinds.</param>
public class EndpointParser(EndpointRegistry endpointRegistry) : IEndpointParser
{
    public IEndpoint Parse(OpenApiObject data, string defaultKind = "")
    {
        var endpoint = endpointRegistry.OfKind(data.GetString("kind") ?? defaultKind);
        endpoint.Parse(data, this);
        return endpoint;
    }
}
