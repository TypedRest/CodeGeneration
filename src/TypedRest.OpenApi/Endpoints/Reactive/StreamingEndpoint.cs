using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
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

            Separator = data.GetString("separator");
            BufferSize = data.GetInt("buffer-size");
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteProperty("separator", Separator);
            writer.WriteProperty("buffer-size", BufferSize);
        }
    }
}
