using System.Net;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc;

public class ConsumerPatternFacts : PatternFactsBase<ConsumerPattern>
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
                    [OperationType.Post] = Sample.Operation(statusCode: HttpStatusCode.NoContent, request: Sample.ContactSchema, description: "A consumer.")
                }
            }
        };

        TryGetEndpoint(tree).Should().BeEquivalentTo(new ConsumerEndpoint
        {
            Schema = Sample.ContactSchema,
            Description = "A consumer."
        }, options => options.IncludingAllRuntimeProperties());
    }
}
