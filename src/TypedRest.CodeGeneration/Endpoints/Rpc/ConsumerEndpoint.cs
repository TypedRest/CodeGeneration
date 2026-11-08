namespace TypedRest.CodeGeneration.Endpoints.Rpc;

/// <summary>
/// RPC endpoint that takes an entity as input when invoked.
/// </summary>
public class ConsumerEndpoint : Endpoint
{
    public override string Kind => "consumer";

    /// <summary>
    /// Schema describing the entity taken as input.
    /// </summary>
    public IOpenApiSchema? Schema { get; set; }

    public override void Parse(JsonObject data, IEndpointParser parser)
    {
        base.Parse(data, parser);

        Schema = data.GetSchema("schema");
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
    }
}
