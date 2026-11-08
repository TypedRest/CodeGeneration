namespace TypedRest.CodeGeneration.Endpoints.Reactive;

/// <summary>
/// Endpoint for a stream of entities using a persistent HTTP connection.
/// </summary>
public class StreamingEndpoint : Endpoint
{
    public override string Kind => "streaming";

    /// <summary>
    /// Schema describing the representation of individual elements in the stream.
    /// </summary>
    public IOpenApiSchema? Schema { get; set; }

    /// <summary>
    /// The character sequence used to detect that a new element starts in an HTTP stream.
    /// </summary>
    public string? Separator { get; set; }

    public override void Parse(JsonObject data, IEndpointParser parser)
    {
        base.Parse(data, parser);

        Schema = data.GetSchema("schema");
        Separator = data.GetString("separator");
    }

    public override void ResolveReferences(OpenApiDocument document)
    {
        base.ResolveReferences(document);

        Schema = Schema?.Resolve(document);
    }

    protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        base.WriteBody(writer, specVersion);

        writer.WriteOptionalObject("schema", Schema, specVersion);
        writer.WriteProperty("separator", Separator);
    }
}
