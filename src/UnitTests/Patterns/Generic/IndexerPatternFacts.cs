using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic;

public class IndexerPatternFacts : PatternFactsBase<IndexerPattern>
{
    [Fact]
    public void GetsEndpoint()
    {
        var mockChildMatches = new Dictionary<string, IEndpoint>
        {
            ["{}"] = new Endpoint {Description = "element"},
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
