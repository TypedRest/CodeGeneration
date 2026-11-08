namespace TypedRest.CodeGeneration.Endpoints;

/// <summary>
/// Represents a TypedRest endpoint.
/// </summary>
public class Endpoint : IEndpoint
{
    public virtual string Kind => "";

    public string? Uri { get; set; }

    public string? Description { get; set; }

    public IDictionary<string, IEndpoint> Children { get; } = new Dictionary<string, IEndpoint>();

    public virtual void Parse(JsonObject data, IEndpointParser parser)
    {
        Description = data.GetString("description");
        Uri = data.GetString("uri");

        if (data.TryGetObject("children") is {} children)
            ParseChildren(children, parser);
    }

    protected void ParseChildren(JsonObject data, IEndpointParser parser)
    {
        foreach ((string key, var value) in data)
        {
            if (value is JsonObject objData)
                Children.Add(key, parser.Parse(objData));
        }
    }

    public virtual void ResolveReferences(OpenApiDocument document)
    {
        foreach (var child in Children.Values)
            child.ResolveReferences(document);
    }

    /// <summary>
    /// Writes this endpoint description to an OpenAPI document.
    /// </summary>
    /// <param name="writer">The write to write to.</param>
    /// <param name="specVersion">The OpenAPI Spec version to use.</param>
    protected virtual void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        if (!string.IsNullOrEmpty(Kind)) writer.WriteProperty("kind", Kind);
        writer.WriteProperty("uri", Uri);
        writer.WriteProperty("description", Description);
    }

    public virtual void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WriteStartObject();
        WriteBody(writer, specVersion);
        writer.WriteOptionalMap("children", Children, specVersion);
        writer.WriteEndObject();
    }

    public void SerializeAsV2(IOpenApiWriter writer)
        => Write(writer, OpenApiSpecVersion.OpenApi2_0);

    public void SerializeAsV3(IOpenApiWriter writer)
        => Write(writer, OpenApiSpecVersion.OpenApi3_0);

    public void SerializeAsV31(IOpenApiWriter writer)
        => Write(writer, OpenApiSpecVersion.OpenApi3_1);

    public void SerializeAsV32(IOpenApiWriter writer)
        => Write(writer, OpenApiSpecVersion.OpenApi3_2);

    public override string ToString()
        => $"{Kind} ({Uri})";
}
