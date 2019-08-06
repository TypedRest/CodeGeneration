using FluentAssertions;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using Xunit;

namespace TypedRest.OpenApi.Patterns.Generic
{
    public class CollectionPatternFacts : PatternFactsBase<CollectionPattern>
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
                        [OperationType.Post] = new OpenApiOperation()
                    }
                },
                Children =
                {
                    ["{id}"] = new PathTree {Item = new OpenApiPathItem()}
                }
            };
            var childMatches = new EndpointList
            {
                ["{id}"] = new Endpoint(),
                ["other"] = new Endpoint {Description = "other"}
            };
            var endpoint = new CollectionEndpoint
            {
                Element = new Endpoint(),
                Children =
                {
                    ["other"] = new Endpoint {Description = "other"}
                }
            };

            TryGetEndpoint(tree, childMatches).Should().BeEquivalentTo(endpoint);
        }

        [Fact]
        public void CompactsSimpleChildElement()
        {
            var tree = new PathTree
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
                    ["{id}"] = new PathTree {Item = new OpenApiPathItem()}
                }
            };
            var childMatches = new EndpointList
            {
                ["{id}"] = new ElementEndpoint()
            };
            var endpoint = new CollectionEndpoint();

            TryGetEndpoint(tree, childMatches).Should().BeEquivalentTo(endpoint);
        }

        [Fact]
        public void RejectsEndpointWithoutChild()
        {
            var tree = new PathTree
            {
                Item = new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Get] = new OpenApiOperation(),
                        [OperationType.Post] = new OpenApiOperation()
                    }
                }
            };

            TryGetEndpoint(tree).Should().BeNull();
        }

        [Fact]
        public void RejectsEndpointWithoutPost()
        {
            var tree = new PathTree
            {
                Item = new OpenApiPathItem
                {
                    Operations =
                    {
                        [OperationType.Get] = new OpenApiOperation()
                    }
                },
                Children =
                {
                    ["{id}"] = new PathTree {Item = new OpenApiPathItem()}
                }
            };

            TryGetEndpoint(tree).Should().BeNull();
        }
    }
}
