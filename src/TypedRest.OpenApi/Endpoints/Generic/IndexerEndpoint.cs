using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Generic
{
    /// <summary>
    /// Endpoint that addresses child endpoints by ID.
    /// </summary>
    public class IndexerEndpoint : Endpoint
    {
        public override string Type => "indexer";

        /// <summary>
        /// A template for child endpoints addressable by ID.
        /// </summary>
        public IEndpoint? Element { get; set; }

        /// <summary>
        /// The default value for <see cref="Element"/>.<see cref="IEndpoint.Type"/>.
        /// </summary>
        protected virtual string ElementDefaultType => "";

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            if (data.TryGetObject("element", out var element))
                Element = parser.Parse(element, ElementDefaultType);
        }

        public override void ResolveReferences(OpenApiComponents components)
        {
            base.ResolveReferences(components);

            Element?.ResolveReferences(components);
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteOptionalObject("element", Element, specVersion);
        }
    }
}
