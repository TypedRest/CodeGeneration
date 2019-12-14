using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints.Raw
{
    /// <summary>
    /// Endpoint that accepts binary uploads.
    /// </summary>
    public class UploadEndpoint : Endpoint
    {
        public override string Kind => "upload";

        /// <summary>
        /// The name of the form field to place the uploaded data into. Leave empty for upload of raw bodies.
        /// </summary>
        public string? FormField { get; set; }

        public override void Parse(OpenApiObject data, IEndpointParser parser)
        {
            base.Parse(data, parser);

            FormField = data.GetString("form-field");
        }

        protected override void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            base.WriteBody(writer, specVersion);

            writer.WriteProperty("form-field", FormField);
        }
    }
}
