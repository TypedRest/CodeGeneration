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
                        [OperationType.Get] = new OpenApiOperation(),
                        [OperationType.Put] = new OpenApiOperation(),
                        [OperationType.Delete] = new OpenApiOperation()
                    }
                }
            };
            var endpoint = new ElementEndpoint();

            TryGetEndpoint(tree).Should().BeEquivalentTo(endpoint);
        }
    }
}
