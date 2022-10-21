using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc;

public class ProducerPatternFacts : PatternFactsBase<ProducerPattern>
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
                    [OperationType.Post] = Sample.Operation(response: Sample.ContactSchema, description: "A producer.")
                }
            }
        };

        TryGetEndpoint(tree).Should().BeEquivalentTo(new ProducerEndpoint
        {
            Schema = Sample.ContactSchema,
            Description = "A producer."
        }, options => options.IncludingAllRuntimeProperties());
    }
}
