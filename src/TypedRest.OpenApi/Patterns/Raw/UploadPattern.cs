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

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
        {
            // TODO: Check operation.RequestBody

            return new BlobEndpoint();
        }
    }
}
