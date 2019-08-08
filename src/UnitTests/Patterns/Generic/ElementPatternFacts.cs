using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints.Generic;
using Xunit;

namespace TypedRest.OpenApi.Patterns.Generic
{
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
                        [OperationType.Get] = Sample.Operation(response: Sample.ContactSchema, summary: "A specific contact."),
                        [OperationType.Put] = Sample.Operation(statusCode: 204, request: Sample.ContactSchema),
                        [OperationType.Delete] = Sample.Operation(statusCode: 204)
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
}
