using JetBrains.Annotations;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints
{
    public class FunctionEndpoint : Endpoint
    {
        public override string Type => "rpc";

        [CanBeNull]
        public string RequestSchema { get; set; }

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
