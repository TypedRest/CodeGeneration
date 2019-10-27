using FluentAssertions;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using Xunit;

namespace TypedRest.OpenApi.CSharp
{
    public class GeneratorFacts
    {
        [Fact]
        public void GeneratesCorrectDom()
        {
            var endpoints = new EndpointList
            {
                ["contacts"] = new CollectionEndpoint
                {
                    Description = "Collection of contacts.",
                    Uri = "./contacts",
                    Schema = Sample.ContactSchema
                }
            };
            var generated = new Generator().Generate(endpoints);

            var interfaceType = new CSharpIdentifier("TypedRest.Endpoints.Generic", "ICollectionEndpoint")
            {
                TypeArguments = {new CSharpIdentifier("Schemas", "Contact")}
            };
            var implementationType = new CSharpIdentifier("TypedRest.Endpoints.Generic", "CollectionEndpoint")
            {
                TypeArguments = {new CSharpIdentifier("Schemas", "Contact")}
            };

            generated.Should().BeEquivalentTo(
                new CSharpClass(new CSharpIdentifier("MyNamespace", "MyClient"))
                {
                    BaseClass = new CSharpClassConstruction(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                    {
                        Parameters = {new CSharpParameter(new CSharpIdentifier("System", "Uri"), "uri")}
                    },
                    Properties =
                    {
                        new CSharpProperty(interfaceType, "contacts")
                        {
                            GetterExpression = new CSharpClassConstruction(implementationType)
                            {
                                Parameters =
                                {
                                    new CSharpParameter(CSharpIdentifier.String, "relativeUri", "./contacts")
                                }
                            }
                        }
                    }
                });
        }
    }
}
