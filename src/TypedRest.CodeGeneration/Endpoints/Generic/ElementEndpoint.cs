namespace TypedRest.CodeGeneration.Endpoints.Generic;

/// <summary>
/// Endpoint for an individual resource.
/// </summary>
public class ElementEndpoint : Endpoint
{
    public override string Kind => "element";

    /// <summary>
    /// Schema describing the representation of this resource. Inherited from the containing <see cref="CollectionEndpoint"/> when unset.
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
