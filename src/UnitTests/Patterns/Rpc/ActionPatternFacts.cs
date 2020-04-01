using System.Net;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints.Rpc;
using Xunit;

namespace TypedRest.CodeGeneration.Patterns.Rpc
{
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
                        [OperationType.Post] = Sample.Operation(statusCode: HttpStatusCode.NoContent, summary: "An action.")
                    }
                }
            };

            TryGetEndpoint(tree).Should().BeEquivalentTo(new ActionEndpoint
            {
                Description = "An action."
            }, options => options.IncludingAllRuntimeProperties());
        }
    }
}
