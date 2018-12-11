using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using Xunit;

namespace TypedRest.OpenApi.Patterns
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
            var childMatches = new Dictionary<string, IEndpoint>
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
