using FluentAssertions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using TypedRest.OpenApi.Endpoints.Rpc;
using Xunit;

namespace TypedRest.OpenApi
{
    public class SerializationFacts
    {
        private const string Yaml = @"swagger: '2.0'
info: { }
paths: { }
x-endpoints:
  contacts:
    type: collection
    description: a collection of contacts
    uri: ./contacts
    element:
      type: element
      children:
        note:
          type: element
          uri: ./note
        poke:
          type: action
          uri: ./poke";

        private readonly OpenApiDocument _doc = new OpenApiDocument
        {
            Info = new OpenApiInfo(),
            Paths = new OpenApiPaths()
        }.SetTypedRestEndpoints(new EndpointList
        {
            ["contacts"] = new CollectionEndpoint
            {
                Description = "a collection of contacts",
                Uri = "./contacts",
                //Schema = "user",
                Element = new ElementEndpoint
                {
                    //Schema = "user",
                    Children =
                    {
                        ["note"] = new ElementEndpoint
                        {
                            Uri = "./note",
                            //Schema = "note"
                        },
                        ["poke"] = new ActionEndpoint
                        {
                            Uri = "./poke"
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
