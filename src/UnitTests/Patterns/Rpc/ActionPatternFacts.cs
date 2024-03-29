using System.Net;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc;

public class ActionPatternFacts : PatternFactsBase<ActionPattern>
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
                    [OperationType.Post] = Sample.Operation(statusCode: HttpStatusCode.NoContent, description: "An action.")
                }
            }
        };

        TryGetEndpoint(tree).Should().BeEquivalentTo(new ActionEndpoint
        {
            Description = "An action."
        }, options => options.IncludingAllRuntimeProperties());
    }
}
