namespace TypedRest.CodeGeneration.Endpoints.Generic;

/// <summary>
/// Endpoint that addresses child endpoints by ID.
/// </summary>
public class IndexerEndpoint : Endpoint
{
    public override string Kind => "indexer";

    /// <summary>
    /// A template for child endpoints addressable by ID.
    /// </summary>
    public IEndpoint? Element { get; set; }

    public override void Parse(OpenApiObject data, IEndpointParser parser)
    {
        base.Parse(data, parser);

        if (data.TryGetObject("element") is {} element)
            Element = parser.Parse(element);
    }

    public override void ResolveReferences(OpenApiComponents components)
    {
        base.ResolveReferences(components);

        Element?.ResolveReferences(components);
    }

    protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        base.WriteBody(writer, specVersion);

        writer.WriteOptionalObject("element", Element, specVersion);
    }
}
