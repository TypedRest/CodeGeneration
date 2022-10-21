namespace TypedRest.CodeGeneration.Endpoints;

/// <summary>
/// Represent the top-level URI of an API.
/// </summary>
public class EntryEndpoint : Endpoint
{
    public override string Kind => "entry";

    public override void Parse(OpenApiObject data, IEndpointParser parser)
        => ParseChildren(data, parser);

    public override void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WriteStartObject();
        foreach (var item in Children)
            writer.WriteOptionalObject(item.Key, item.Value, specVersion);
        writer.WriteEndObject();
    }
}
