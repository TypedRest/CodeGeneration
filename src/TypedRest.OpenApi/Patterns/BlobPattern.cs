using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class BlobPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Get, OperationType.Put};

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new BlobEndpoint();
    }
}
