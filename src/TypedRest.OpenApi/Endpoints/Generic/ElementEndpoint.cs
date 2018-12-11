using JetBrains.Annotations;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Generic
{
    /// <summary>
    /// Endpoint for an individual resource.
    /// </summary>
    public class ElementEndpoint : Endpoint
    {
        public override string Type => "element";

        /// <summary>
        /// Schema describing the representation of this resource. Inherited from the containing <see cref="CollectionEndpoint"/> when unset.
        /// </summary>
        [CanBeNull]
        public OpenApiSchema Schema { get; set; }

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
