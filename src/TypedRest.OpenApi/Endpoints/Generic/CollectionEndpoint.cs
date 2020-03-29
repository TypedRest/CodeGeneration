using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for a collection of entities addressable as <see cref="ElementEndpoint"/>s.
    /// </summary>
    public class CollectionEndpoint : Endpoint
    {
        public override string Kind => "collection";

        /// <summary>
        /// Schema describing the representation of individual elements in the collection.
        /// </summary>
        public OpenApiSchema? Schema { get; set; }

        /// <summary>
        /// A template for child endpoints addressable by ID.
        /// </summary>
        public ElementEndpoint? Element { get; set; }

        public override void Parse(OpenApiObject data, IEndpointParser parser)
        {
            base.Parse(data, parser);

            Schema = data.GetSchema("schema");
            if (data.TryGetObject("element", out var element))
                Element = parser.Parse(element, defaultKind: "element") as ElementEndpoint;
        }

        public override void ResolveReferences(OpenApiComponents components)
        {
            base.ResolveReferences(components);

            Schema = Schema?.Resolve(components);
            Element?.ResolveReferences(components);
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteOptionalObject("schema", Schema, specVersion);
            writer.WriteOptionalObject("element", Element, specVersion);
        }
    }
}
