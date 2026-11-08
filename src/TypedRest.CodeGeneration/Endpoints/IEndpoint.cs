namespace TypedRest.CodeGeneration.Endpoints;

/// <summary>
/// Represents a TypedRest endpoint.
/// </summary>
public interface IEndpoint : IOpenApiSerializable, IOpenApiExtension
{
    /// <summary>
    /// Fills the endpoint with information parsed from a JSON object in an OpenAPI document.
    /// </summary>
    /// <param name="data">The JSON object to parse.</param>
    /// <param name="parser">The endpoint parser to use for parsing child objects.</param>
    void Parse(JsonObject data, IEndpointParser parser);

    /// <summary>
    /// Resolves <see cref="OpenApiSchemaReference"/>s against the schema components of a document.
    /// </summary>
    /// <param name="document">The document that references can point to.</param>
    void ResolveReferences(OpenApiDocument document);

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
