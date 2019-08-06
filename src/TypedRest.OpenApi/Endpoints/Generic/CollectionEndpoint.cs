using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for a collection of entities addressable as <see cref="ElementEndpoint"/>s.
    /// </summary>
    public class CollectionEndpoint : IndexerEndpoint
    {
        public override string Type => "collection";

        /// <summary>
        /// A reference to the <see cref="OpenApiSchema"/> describing the representation of individual elements in the collection.
        /// </summary>
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
