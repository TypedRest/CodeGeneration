using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Reactive
{
    /// <summary>
    /// Endpoint for a stream of entities using a persistent HTTP connection.
    /// </summary>
    public class StreamingEndpoint : Endpoint
    {
        public override string Type => "streaming";

        /// <summary>
        /// Schema describing the representation of individual elements in the strean.
        /// </summary>
        public OpenApiSchema Schema { get; set; }

        /// <summary>
        /// The character sequence used to detect that a new element starts in an HTTP stream.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// The size of the buffer used to collect data for deserialization in bytes.
        /// </summary>
        public int? BufferSize { get; set; }

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
        {
            base.Parse(data, parser);

            Schema = data.GetSchema("schema");
            Separator = data.GetString("separator");
            BufferSize = data.GetInt("buffer-size");
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
            writer.WriteProperty("separator", Separator);
            writer.WriteProperty("buffer-size", BufferSize);
        }
    }
}
