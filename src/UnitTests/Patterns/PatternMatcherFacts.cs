using FluentAssertions;
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
                    Description = "All contacts.",
                    Schema = Sample.ContactSchema,
                    Element = new ElementEndpoint
                    {
                        Schema = Sample.ContactSchema,
                        Description = "A specific contact.",
                        Children =
                        {
                            ["note"] = new ElementEndpoint
                            {
                                Uri = "./note",
                                Schema = Sample.NoteSchema,
                                Description = "The note for a specific contact."
                            },
                            ["poke"] = new ActionEndpoint
                            {
                                Uri = "./poke",
                                Description = "Pokes a contact."
                            }
                        }
                    }
                }
            }, options => options.IncludingAllRuntimeProperties());
        }
    }
}
