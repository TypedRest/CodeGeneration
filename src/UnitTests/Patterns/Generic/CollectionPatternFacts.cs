using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic;

public class CollectionPatternFacts : PatternFactsBase<CollectionPattern>
{
    [Fact]
    public void GetsEndpoint()
    {
        var mockChildMatches = new Dictionary<string, IEndpoint>
        {
            ["{}"] = new ElementEndpoint
            {
                Schema = Sample.ContactSchema,
                Children = {["sub"] = new Endpoint {Description = "sub"}}
            },
            ["other"] = new Endpoint {Description = "other"}
        };

        TryGetEndpoint(CollectionTree, mockChildMatches).Should().BeEquivalentTo(new CollectionEndpoint
        {
            Schema = Sample.ContactSchema,
            Element = new ElementEndpoint
            {
                Children = {["sub"] = new Endpoint {Description = "sub"}}
            },
            Children =
            {
                ["other"] = new Endpoint {Description = "other"}
            },
            Description = "Collection of contacts."
        }, options => options.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void TrimsTrivialElementEndpoint()
    {
        var mockChildMatches = new Dictionary<string, IEndpoint>
        {
            ["{}"] = new ElementEndpoint {Schema = Sample.ContactSchema},
            ["other"] = new Endpoint {Description = "other"}
        };

        TryGetEndpoint(CollectionTree, mockChildMatches).Should().BeEquivalentTo(new CollectionEndpoint
        {
            Schema = Sample.ContactSchema,
            Children =
            {
                ["other"] = new Endpoint {Description = "other"}
            },
            Description = "Collection of contacts."
        }, options => options.IncludingAllRuntimeProperties());
    }

    [Fact]
    public void RejectsEndpointWithoutChild()
    {
        TryGetEndpoint(CollectionTree).Should().BeNull();
    }

    private static PathTree CollectionTree
        => new()
        {
            Item = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Sample.Operation(response: new OpenApiSchema {Type = "array", Items = Sample.ContactSchema}, description: "Collection of contacts."),
                    [OperationType.Post] = Sample.Operation(request: Sample.ContactSchema)
                }
            }
        };
}
