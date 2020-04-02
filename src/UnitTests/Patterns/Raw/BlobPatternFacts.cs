using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints.Raw;
using Xunit;

namespace TypedRest.CodeGeneration.Patterns.Raw
{
    public class BlobPatternFacts : PatternFactsBase<BlobPattern>
    {
        [Fact]
        public void GetsEndpoint()
        {
            var tree = new PathTree
            {
                Item = new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Get] = Sample.Operation(mimeType: "application/octet-stream", response: new OpenApiSchema(), description: "A blob."),
                        [OperationType.Put] = Sample.Operation(mimeType: "application/octet-stream", request: new OpenApiSchema())
                    }
                }
            };

            TryGetEndpoint(tree).Should().BeEquivalentTo(new BlobEndpoint
            {
                Description = "A blob."
            }, options => options.IncludingAllRuntimeProperties());
        }
    }
}
