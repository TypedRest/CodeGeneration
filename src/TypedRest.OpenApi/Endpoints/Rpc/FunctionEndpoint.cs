using JetBrains.Annotations;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that takes an entity as input and returns another entity as output when invoked.
    /// </summary>
    public class FunctionEndpoint : Endpoint
    {
        public override string Type => "rpc";

        /// <summary>
        /// Schema describing the entity taken as input.
        /// </summary>
        [CanBeNull]
        public OpenApiSchema RequestSchema { get; set; }

        /// <summary>
        /// Schema describing the entity provided as output.
        /// </summary>
        [CanBeNull]
        public OpenApiSchema ResponseSchema { get; set; }

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            RequestSchema = data.GetSchema("request-schema");
            ResponseSchema = data.GetSchema("response-schema");
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteOptionalObject("request-schema", RequestSchema, specVersion);
            writer.WriteOptionalObject("response-schema", ResponseSchema, specVersion);
        }
    }
}
