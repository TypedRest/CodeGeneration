using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.CodeGeneration.Endpoints.Rpc;

/// <summary>
/// RPC endpoint that takes an entity as input and returns another entity as output when invoked.
/// </summary>
public class FunctionEndpoint : Endpoint
{
    public override string Kind => "function";

    /// <summary>
    /// Schema describing the entity taken as input.
    /// </summary>
    public OpenApiSchema? RequestSchema { get; set; }

    /// <summary>
    /// Schema describing the entity provided as output.
    /// </summary>
    public OpenApiSchema? ResponseSchema { get; set; }

    public override void Parse(OpenApiObject data, IEndpointParser parser)
    {
        base.Parse(data, parser);

        RequestSchema = data.GetSchema("request-schema");
        ResponseSchema = data.GetSchema("response-schema");
    }

    public override void ResolveReferences(OpenApiComponents components)
    {
        base.ResolveReferences(components);

        RequestSchema = RequestSchema?.Resolve(components);
        ResponseSchema = ResponseSchema?.Resolve(components);
    }

    protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        base.WriteBody(writer, specVersion);

        writer.WriteOptionalObject("request-schema", RequestSchema, specVersion);
        writer.WriteOptionalObject("response-schema", ResponseSchema, specVersion);
    }
}
