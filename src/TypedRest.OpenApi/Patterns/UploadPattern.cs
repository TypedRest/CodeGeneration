using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class UploadPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Post};

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new BlobEndpoint();
    }
}
