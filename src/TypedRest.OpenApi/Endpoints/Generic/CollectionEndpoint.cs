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
        /// Schema describing the representation of individual elements in the collection.
        /// </summary>
        public OpenApiSchema? Schema { get; set; }

        protected override string ElementDefaultType => "element";

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            Schema = data.GetSchema("schema");
        }

        public override void ResolveReferences(OpenApiComponents components)
        {
            base.ResolveReferences(components);

            Schema = Schema?.Resolve(components);
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteOptionalObject("schema", Schema, specVersion);
        }
    }
}
