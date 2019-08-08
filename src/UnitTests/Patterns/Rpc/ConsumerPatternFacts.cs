using System.Net;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints.Rpc;
using Xunit;

namespace TypedRest.OpenApi.Patterns.Rpc
{
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
                        [OperationType.Post] = Sample.Operation(statusCode: HttpStatusCode.NoContent, request: Sample.ContactSchema, summary: "A consumer.")
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
}
