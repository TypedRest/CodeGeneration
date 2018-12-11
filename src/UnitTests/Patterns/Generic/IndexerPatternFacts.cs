using FluentAssertions;
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
            var mockChildMatches = new EndpointList
            {
                ["{id}"] = new Endpoint {Description = "element"},
                ["other"] = new Endpoint {Description = "other"}
            };

            TryGetEndpoint(new PathTree(), mockChildMatches).Should().BeEquivalentTo(new IndexerEndpoint
            {
                Element = new Endpoint {Description = "element"},
                Children =
                {
                    ["other"] = new Endpoint {Description = "other"}
                }
            }, options => options.IncludingAllRuntimeProperties());
        }

        [Fact]
        public void RejectsEndpointWithoutChild()
        {
            TryGetEndpoint(new PathTree()).Should().BeNull();
        }
    }
}
