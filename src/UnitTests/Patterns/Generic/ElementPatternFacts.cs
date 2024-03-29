using System.Net;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic;

public class ElementPatternFacts : PatternFactsBase<ElementPattern>
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
                    [OperationType.Get] = Sample.Operation(response: Sample.ContactSchema, description: "A specific contact."),
                    [OperationType.Put] = Sample.Operation(statusCode: HttpStatusCode.NoContent, request: Sample.ContactSchema),
                    [OperationType.Delete] = Sample.Operation(statusCode: HttpStatusCode.NoContent)
                }
            }
        };

        TryGetEndpoint(tree).Should().BeEquivalentTo(new ElementEndpoint
        {
            Description = "A specific contact.",
            Schema = Sample.ContactSchema
        }, options => options.IncludingAllRuntimeProperties());
    }
}
