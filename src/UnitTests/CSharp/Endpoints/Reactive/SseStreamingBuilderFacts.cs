using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Reactive;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Reactive;

public class SseStreamingBuilderFacts
{
    private readonly EndpointGenerator _generator = new(
        new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
        BuilderRegistry.Default);

    [Fact]
    public void GeneratesEndpointForSchema()
    {
        Creation().Type.Should().BeEquivalentTo(new CSharpIdentifier("TypedRest.Endpoints.Reactive", "SseStreamingEndpoint")
        {
            TypeArguments = {new CSharpIdentifier("MyNamespace", "Note")}
        });
    }

    [Fact]
    public void PassesEventType()
    {
        Creation(eventType: "update")
           .Parameters.Should().ContainSingle(x => x.Name == "eventType")
           .Which.Value.Should().Be("update");
    }

    [Fact]
    public void OmitsEventTypeWhenNotSet()
    {
        Creation().Parameters.Should().NotContain(x => x.Name == "eventType");
    }

    private CSharpObjectCreation Creation(string? eventType = null)
    {
        var (property, _) = _generator.Generate("events", new SseStreamingEndpoint
        {
            Uri = "./events",
            Schema = Sample.NoteSchema,
            EventType = eventType
        });

        return property.GetterExpression!;
    }
}
