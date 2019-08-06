using JetBrains.Annotations;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Rpc
{
    /// <summary>
    /// RPC endpoint that returns an entity as output when invoked.
    /// </summary>
    public class ProducerEndpoint : Endpoint
    {
        public override string Type => "producer";

        /// <summary>
        /// A reference to the <see cref="OpenApiSchema"/> describing the entity provided as output.
        /// </summary>
        [CanBeNull]
        public string Schema { get; set; }

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            Schema = data.GetString("schema");
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteProperty("schema", Schema);
        }
    }
}
