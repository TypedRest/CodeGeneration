using TypedRest.CodeGeneration.Endpoints.Raw;

namespace TypedRest.CodeGeneration.Patterns.Raw;

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

    [Fact]
    public void IgnoresJsonPayload()
    {
        var tree = new PathTree
        {
            Item = new OpenApiPathItem
            {
                Operations = {[OperationType.Get] = Sample.Operation(response: Sample.ContactSchema)}
            }
        };

        TryGetEndpoint(tree).Should().BeNull("a JSON body belongs to one of the typed patterns");
    }

    [Fact]
    public void IgnoresEmptyPayload()
    {
        var tree = new PathTree
        {
            Item = new OpenApiPathItem
            {
                Operations = {[OperationType.Get] = Sample.Operation()}
            }
        };

        TryGetEndpoint(tree).Should().BeNull("a response with no body is not a blob");
    }
}
