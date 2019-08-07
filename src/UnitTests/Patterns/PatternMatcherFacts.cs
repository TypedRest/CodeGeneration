using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using TypedRest.OpenApi.Endpoints.Rpc;
using Xunit;

namespace TypedRest.OpenApi.Patterns
{
    public class PatternMatcherFacts
    {
        [Fact]
        public void MatchesPatterns()
        {
            var tree = PathTree.From(Sample.Paths);

            var endpoints = new PatternMatcher().GetEndpoints(tree);

            endpoints.Should().BeEquivalentTo(new EndpointList
            {
                ["contacts"] = new CollectionEndpoint
                {
                    Uri = "./contacts",
                    Element = new ElementEndpoint
                    {
                        Children =
                        {
                            ["note"] = new ElementEndpoint
                            {
                                Uri = "./note"
                            },
                            ["poke"] = new ActionEndpoint
                            {
                                Uri = "./poke"
                            }
                        }
                    }
                }
            });
        }
    }
}
