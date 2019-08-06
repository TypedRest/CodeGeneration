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
        /// A reference to the <see cref="OpenApiSchema"/> describing the entity taken as input.
        /// </summary>
        [CanBeNull]
        public string RequestSchema { get; set; }

        /// <summary>
        /// A reference to the <see cref="OpenApiSchema"/> describing the entity provided as output.
        /// </summary>
        [CanBeNull]
        public string ResponseSchema { get; set; }

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            RequestSchema = data.GetString("request-schema");
            ResponseSchema = data.GetString("response-schema");
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteProperty("request-schema", RequestSchema);
            writer.WriteProperty("response-schema", ResponseSchema);
        }
    }
}
