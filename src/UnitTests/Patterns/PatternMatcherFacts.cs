using System.Collections.Generic;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using Xunit;

namespace TypedRest.OpenApi.Patterns
{
    public class PatternMatcherFacts
    {
        [Fact]
        public void Bla()
        {
            var tree = new Dictionary<string, PathTree>
            {
                ["health"] = new PathTree {Item = new OpenApiPathItem {Summary = "Health"}},
                ["users"] = new PathTree
                {
                    Children =
                    {
                        ["a"] = new PathTree {Item = new OpenApiPathItem {Summary = "Users A"}},
                        ["b"] = new PathTree {Item = new OpenApiPathItem {Summary = "Users B"}}
                    }
                }
            };

            var matcher = new PatternMatcher();
            var endpoints = matcher.GetEndpoints(tree);

            endpoints.Should().BeEquivalentTo(new Dictionary<string, IEndpoint>
            {
                ["health"] = new Endpoint {Uri = "./health"},
                ["users"] = new Endpoint
                {
                    Uri = "./users",
                    Children =
                    {
                        ["a"] = new Endpoint {Uri = "./a"},
                        ["b"] = new Endpoint {Uri = "./b"}
                    }
                }
            });
        }
    }
}
