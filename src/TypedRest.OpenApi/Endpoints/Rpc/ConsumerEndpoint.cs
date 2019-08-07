using JetBrains.Annotations;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that takes an entity as input when invoked.
    /// </summary>
    public class ConsumerEndpoint : Endpoint
    {
        public override string Type => "consumer";

        /// <summary>
        /// Schema describing the entity taken as input.
        /// </summary>
        [CanBeNull]
        public OpenApiSchema Schema { get; set; }

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            Schema = data.GetSchema("schema");
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteOptionalObject("schema", Schema, specVersion);
        }
    }
}
