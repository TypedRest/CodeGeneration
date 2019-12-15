using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// Represent the top-level URI of an API.
    /// </summary>
    public class EntryEndpoint : Endpoint
    {
        public override string Kind => "entry";

        public override void Parse(OpenApiObject data, IEndpointParser parser)
            => Children.Parse(data, parser);

        public override void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
            => Children.Write(writer, specVersion);
    }
}
