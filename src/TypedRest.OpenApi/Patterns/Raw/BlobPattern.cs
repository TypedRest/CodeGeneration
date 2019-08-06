using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Raw;

namespace TypedRest.OpenApi.Patterns.Raw
{
    /// <summary>
    /// A pattern that generates <see cref="BlobEndpoint"/>s.
    /// </summary>
    public class BlobPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Get, OperationType.Put};

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new BlobEndpoint();
    }
}
