using FluentAssertions;
using TypedRest.CodeGeneration.CSharp.Builders;
using NanoByte.CodeGeneration;
using Xunit;

namespace TypedRest.CodeGeneration.CSharp
{
    public class EndpointGeneratorFacts
    {
        [Fact]
        public void GeneratesCorrectDom()
        {
            var generator = new EndpointGenerator(
                new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
                BuilderRegistry.Default);
            var generated = generator.Generate(Sample.EntryEndpoint);

            var noteEndpoint = ElementEndpoint("Note");

            var contactEndpointInterface = new CSharpInterface(new CSharpIdentifier("MyNamespace", "IContactElementEndpoint"))
            {
                Summary = "A specific contact.",
                Attributes = {Attributes.GeneratedCode},
                Interfaces = {ElementEndpoint("Contact").ToInterface()},
                Properties =
                {
                    Property("Note", "The note for a specific contact.", noteEndpoint.ToInterface()),
                    Property("Poke", "Pokes a contact.", ActionEndpoint.ToInterface()),
                    Property("Picture", "A picture of a specific contact.", BlobEndpoint.ToInterface())
                }
            };
            var contactEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "ContactElementEndpoint"))
            {
                Summary = contactEndpointInterface.Summary,
                Attributes = {Attributes.GeneratedCode},
                BaseClass = new CSharpConstructor(ElementEndpoint("Contact"))
                {
                    Parameters =
                    {
                        Referrer,
                        new CSharpParameter(CSharpIdentifier.Uri, "relativeUri")
                    }
                },
                Interfaces = {contactEndpointInterface.Identifier},
                Properties =
                {
                    Property("Note", "The note for a specific contact.", noteEndpoint.ToInterface(), noteEndpoint, "./note"),
                    Property("Poke", "Pokes a contact.", ActionEndpoint.ToInterface(), ActionEndpoint, "./poke"),
                    Property("Picture", "A picture of a specific contact.", BlobEndpoint.ToInterface(), BlobEndpoint, "./picture")
                }
            };

            var collectionEndpoint = CollectionEndpoint("Contact", contactEndpoint.Identifier);

            var entryEndpointInterface = new CSharpInterface(new CSharpIdentifier("MyNamespace", "IMyServiceClient"))
            {
                Attributes = {Attributes.GeneratedCode},
                Interfaces = {new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint")},
                Properties =
                {
                    Property("Contacts", "Collection of contacts.", CollectionEndpoint("Contact", contactEndpointInterface.Identifier).ToInterface())
                }
            };
            var entryEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "MyServiceClient"))
            {
                Attributes = {Attributes.GeneratedCode},
                BaseClass = new CSharpConstructor(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                {
                    Parameters =
                    {
                        new CSharpParameter(CSharpIdentifier.Uri, "uri")
                    }
                },
                Interfaces = {entryEndpointInterface.Identifier},
                Properties =
                {
                    Property("Contacts", "Collection of contacts.", CollectionEndpoint("Contact", contactEndpointInterface.Identifier).ToInterface(), collectionEndpoint, "./contacts")
                }
            };

            generated.Should().BeEquivalentTo(
                entryEndpointInterface, entryEndpoint, contactEndpointInterface, contactEndpoint);
        }

        private static CSharpParameter Referrer
            => new CSharpParameter(new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint"), "referrer")
            {
                Value = new ThisReference()
            };

        private static CSharpProperty Property(string name, string description, CSharpIdentifier interfaceType, CSharpIdentifier? implementationType = null, string? relativeUri = null)
        {
            var property = new CSharpProperty(interfaceType, name)
            {
                Summary = description
            };
            if (implementationType != null)
            {
                property.GetterExpression = new CSharpConstructor(implementationType)
                {
                    Parameters =
                    {
                        Referrer,
                        new CSharpParameter(CSharpIdentifier.String, "relativeUri") {Value = relativeUri}
                    }
                };
            }
            return property;
        }

        private static CSharpIdentifier ActionEndpoint
            => new CSharpIdentifier("TypedRest.Endpoints.Rpc", "ActionEndpoint");

        private static CSharpIdentifier BlobEndpoint
            => new CSharpIdentifier("TypedRest.Endpoints.Raw", "BlobEndpoint");

        private static CSharpIdentifier ElementEndpoint(string dto)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "ElementEndpoint")
            {
                TypeArguments = {new CSharpIdentifier("MyNamespace", dto)}
            };

        private static CSharpIdentifier CollectionEndpoint(string dto, CSharpIdentifier elementEndpoint)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "CollectionEndpoint")
            {
                TypeArguments = {new CSharpIdentifier("MyNamespace", dto), elementEndpoint}
            };
    }
}
