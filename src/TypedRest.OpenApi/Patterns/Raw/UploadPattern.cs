using System.Linq;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Raw;

namespace TypedRest.OpenApi.Patterns.Raw
{
    /// <summary>
    /// A pattern that generates <see cref="UploadEndpoint"/>s.
    /// </summary>
    public class UploadPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Post};

        protected override IEndpoint? BuildEndpoint(OpenApiPathItem item)
        {
            var operation = item.Operations[OperationType.Post];

            var request = operation.GetRequest();
            if (request == null) return null;

            return new UploadEndpoint
            {
                FormField = request.TryGetValue("multipart/form-data", out var mediaType)
                    ? mediaType?.Schema.Properties.FirstOrDefault(x => x.Value.Type == "binary").Key
                    : null,
                Description = operation.Description ?? operation.Summary
            };
        }
    }
}
