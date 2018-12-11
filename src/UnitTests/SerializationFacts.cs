using FluentAssertions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using TypedRest.OpenApi.Endpoints;
using Xunit;

namespace TypedRest.OpenApi
{
    public class SerializationFacts
    {
        private const string Yaml = @"swagger: '2.0'
info: { }
paths: { }
x-endpoints:
  users:
    type: collection
    description: a collection of users
    uri: ./users
    element:
      type: element
      children:
        claims:
          type: collection
          uri: ./claims
          schema: claimdto
      schema: userdto
    schema: userdto";

        private readonly OpenApiDocument _doc = new OpenApiDocument
        {
            Info = new OpenApiInfo(),
            Paths = new OpenApiPaths()
        }.SetTypedRestEndpoints(new EndpointList
        {
            ["users"] = new CollectionEndpoint
            {
                Description = "a collection of users",
                Uri = "./users",
                Schema = "user",
                Element = new ElementEndpoint
                {
                    Schema = "user",
                    Children =
                    {
                        ["claims"] = new CollectionEndpoint
                        {
                            Uri = "./claims",
                            Schema = "claim"
                        }
                    }
                }
            }
        });

        [Fact]
        public void CanSerialize()
        {
            _doc.Serialize(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Yaml)
                .Should().Be(Yaml);
        }

        [Fact]
        public void CanDeserialize()
        {
            var reader = new OpenApiStringReader(new OpenApiReaderSettings().AddTypedRest());
            reader.Read(Yaml, out _)
                  .Should().BeEquivalentTo(_doc);
        }
    }
}
