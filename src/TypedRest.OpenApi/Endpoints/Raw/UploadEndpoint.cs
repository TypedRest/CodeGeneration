using JetBrains.Annotations;
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
        public override string Type => "upload";

        /// <summary>
        /// The name of the form field to place the uploaded data into. Leave empty for upload of raw bodies.
        /// </summary>
        [CanBeNull]
        public string FormField { get; set; }

        public override void Parse(OpenApiObject data, IEndpointsParser parser)
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
