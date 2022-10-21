using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc;

public class FunctionPatternFacts : PatternFactsBase<FunctionPattern>
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
                    [OperationType.Post] = Sample.Operation(request: Sample.ContactSchema, response: Sample.NoteSchema, description: "A function.")
                }
            }
        };

        TryGetEndpoint(tree).Should().BeEquivalentTo(new FunctionEndpoint
        {
            RequestSchema = Sample.ContactSchema,
            ResponseSchema = Sample.NoteSchema,
            Description = "A function."
        }, options => options.IncludingAllRuntimeProperties());
    }
}
