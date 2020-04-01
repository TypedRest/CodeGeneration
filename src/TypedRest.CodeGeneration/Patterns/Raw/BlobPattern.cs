using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Raw;

namespace TypedRest.CodeGeneration.Patterns.Raw
{
    /// <summary>
    /// A pattern that generates <see cref="BlobEndpoint"/>s.
    /// </summary>
    public class BlobPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Get /*, OperationType.Put*/};

        protected override IEndpoint? BuildEndpoint(OpenApiPathItem item)
        {
            var operation = item.Operations[OperationType.Get];

            if (operation.Get200Response() == null) return null;

            return new BlobEndpoint
            {
                Description = operation.Description ?? operation.Summary
            };
        }
    }
}