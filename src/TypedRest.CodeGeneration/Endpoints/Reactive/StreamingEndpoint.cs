using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

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
    public OpenApiSchema? Schema { get; set; }

    /// <summary>
    /// The character sequence used to detect that a new element starts in an HTTP stream.
    /// </summary>
    public string? Separator { get; set; }

    public override void Parse(OpenApiObject data, IEndpointParser parser)
    {
        base.Parse(data, parser);

        Schema = data.GetSchema("schema");
        Separator = data.GetString("separator");
    }

    public override void ResolveReferences(OpenApiComponents components)
    {
        base.ResolveReferences(components);

        Schema = Schema?.Resolve(components);
    }

    protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        base.WriteBody(writer, specVersion);

        writer.WriteOptionalObject("schema", Schema, specVersion);
        writer.WriteProperty("separator", Separator);
    }
}
