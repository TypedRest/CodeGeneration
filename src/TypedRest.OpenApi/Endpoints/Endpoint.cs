using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Represents a TypedRest endpoint.
    /// </summary>
    public class Endpoint : IEndpoint
    {
        public virtual string Kind => "";

        public string? Description { get; set; }

        public string? Uri { get; set; }

        public EndpointList Children { get; } = new EndpointList();

        public virtual void Parse(OpenApiObject data, IEndpointParser parser)
        {
            Description = data.GetString("description");
            Uri = data.GetString("uri");

            if (data.TryGetValue("children", out var anyData) && anyData is OpenApiObject objData)
                Children.Parse(objData, parser);
        }

        public virtual void ResolveReferences(OpenApiComponents components)
        {
            foreach (var child in Children.Values)
                child.ResolveReferences(components);
        }

        /// <summary>
        /// Writes this endpoint description to an OpenAPI document.
        /// </summary>
        /// <param name="writer">The write to write to.</param>
        /// <param name="specVersion">The OpenAPI Spec version to use.</param>
        protected virtual void WriteBody(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            if (!string.IsNullOrEmpty(Kind)) writer.WriteProperty("kind", Kind);
            writer.WriteProperty("description", Description);
            writer.WriteProperty("uri", Uri);
            writer.WriteOptionalMap("children", Children, specVersion);
        }

        public virtual void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();
            WriteBody(writer, specVersion);
            writer.WriteEndObject();
        }

        public void SerializeAsV2(IOpenApiWriter writer)
            => Write(writer, OpenApiSpecVersion.OpenApi2_0);

        public void SerializeAsV3(IOpenApiWriter writer)
            => Write(writer, OpenApiSpecVersion.OpenApi3_0);

        public override string ToString()
            => $"{Kind} ({Uri})";
    }
}
