using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints.Rpc;
using Xunit;

namespace TypedRest.OpenApi.Patterns.Rpc
{
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
                        [OperationType.Post] = Sample.Operation(response: Sample.ContactSchema, summary: "A producer.")
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
}
