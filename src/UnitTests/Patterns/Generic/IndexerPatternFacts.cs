using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using Xunit;

namespace TypedRest.OpenApi.Patterns.Generic
{
    public class IndexerPatternFacts : PatternFactsBase<IndexerPattern>
    {
        [Fact]
        public void GetsEndpoint()
        {
            var tree = new PathTree
            {
                Children =
                {
                    ["{id}"] = new PathTree {Item = new OpenApiPathItem()}
                }
            };
            var childMatches = new EndpointList
            {
                ["{id}"] = new ElementEndpoint(),
                ["other"] = new Endpoint {Description = "other"}
            };
            var endpoint = new IndexerEndpoint
            {
                Element = new ElementEndpoint(),
                Children =
                {
                    ["other"] = new Endpoint {Description = "other"}
                }
            };

            TryGetEndpoint(tree, childMatches).Should().BeEquivalentTo(endpoint);
        }

        [Fact]
        public void RejectsEndpointWithoutChild()
        {
            var tree = new PathTree();

            TryGetEndpoint(tree).Should().BeNull();
        }
    }
}
