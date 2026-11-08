namespace TypedRest.CodeGeneration.Endpoints.Reactive;

/// <summary>
/// Endpoint for a stream of entities using Server-Sent Events (SSE).
/// </summary>
public class SseStreamingEndpoint : Endpoint
{
    public override string Kind => "sse";

    /// <summary>
    /// Schema describing the representation of individual elements in the stream.
    /// </summary>
    public IOpenApiSchema? Schema { get; set; }

    /// <summary>
    /// If set, only events with this <c>event:</c> type are emitted; others are ignored.
    /// </summary>
    public string? EventType { get; set; }

    public override void Parse(JsonObject data, IEndpointParser parser)
    {
        base.Parse(data, parser);

        Schema = data.GetSchema("schema");
        EventType = data.GetString("event-type");
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
        writer.WriteProperty("event-type", EventType);
    }
}
