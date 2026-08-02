namespace TypedRest.CodeGeneration.Endpoints.Reactive;

public class SseStreamingEndpointFacts
{
    private static readonly EndpointParser Parser = new(EndpointRegistry.Default);

    [Fact]
    public void ParsesKind()
    {
        Parser.Parse(Data())
              .Should().BeOfType<SseStreamingEndpoint>()
              .Which.Uri.Should().Be("./events");
    }

    [Fact]
    public void ParsesSchema()
    {
        Parser.Parse(Data())
              .Should().BeOfType<SseStreamingEndpoint>()
              .Which.Schema!.Reference.Id.Should().Be("Note");
    }

    [Fact]
    public void ParsesEventType()
    {
        Parser.Parse(Data(eventType: "update"))
              .Should().BeOfType<SseStreamingEndpoint>()
              .Which.EventType.Should().Be("update");
    }

    [Fact]
    public void DefaultsToNoEventType()
    {
        Parser.Parse(Data())
              .Should().BeOfType<SseStreamingEndpoint>()
              .Which.EventType.Should().BeNull();
    }

    private static OpenApiObject Data(string? eventType = null)
    {
        var data = new OpenApiObject
        {
            ["kind"] = new OpenApiString("sse"),
            ["uri"] = new OpenApiString("./events"),
            ["schema"] = new OpenApiObject
            {
                ["$ref"] = new OpenApiString("#/components/schemas/Note")
            }
        };
        if (eventType != null) data["event-type"] = new OpenApiString(eventType);
        return data;
    }
}
