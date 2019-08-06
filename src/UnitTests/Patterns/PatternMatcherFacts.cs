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
            var tree = new PathTree
            {
                Children =
                {
                    ["contacts"] = new PathTree
                    {
                        Item = new OpenApiPathItem
                        {
                            Operations =
                            {
                                [OperationType.Get] = new OpenApiOperation(),
                                [OperationType.Post] = new OpenApiOperation()
                            }
                        },
                        Children =
                        {
                            ["{id}"] = new PathTree
                            {
                                Item = new OpenApiPathItem
                                {
                                    Operations =
                                    {
                                        [OperationType.Get] = new OpenApiOperation(),
                                        [OperationType.Put] = new OpenApiOperation(),
                                        [OperationType.Delete] = new OpenApiOperation()
                                    }
                                },
                                Children =
                                {
                                    ["note"] = new PathTree
                                    {
                                        Item = new OpenApiPathItem
                                        {
                                            Operations =
                                            {
                                                [OperationType.Get] = new OpenApiOperation(),
                                                [OperationType.Put] = new OpenApiOperation()
                                            }
                                        },
                                    },
                                    ["poke"] = new PathTree
                                    {
                                        Item = new OpenApiPathItem
                                        {
                                            Operations =
                                            {
                                                [OperationType.Post] = new OpenApiOperation()
                                            }
                                        },
                                    }
                                }
                            }
                        }
                    }
                }
            };

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
