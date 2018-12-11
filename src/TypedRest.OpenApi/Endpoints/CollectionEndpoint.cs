using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints
{
    public class CollectionEndpoint : IndexerEndpoint
    {
        public override string Type => "collection";

        public string Schema { get; set; }

        protected override string ElementDefaultType => "element";

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            Schema = data.GetString("schema");

            if (Element is ElementEndpoint element && element.Schema == null)
                element.Schema = Schema;
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteProperty("schema", Schema);
        }
    }
}
