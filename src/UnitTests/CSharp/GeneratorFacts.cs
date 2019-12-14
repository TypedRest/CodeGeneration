using FluentAssertions;
using TypedRest.OpenApi.CSharp.Dom;
using Xunit;

namespace TypedRest.OpenApi.CSharp
{
    public class GeneratorFacts
    {
        [Fact]
        public void GeneratesCorrectDom()
        {
            var generator = new Generator(new NamingConvention("MyNamespace", "MyService"));
            var generated = generator.Generate(Sample.Endpoints, Sample.Doc.Components.Schemas);

            var noteDto = Dto("Note");
            var noteEndpoint = ElementEndpoint(noteDto);

            var contactDto = Dto("Contact");
            var contactEndpointInterface = new CSharpInterface(new CSharpIdentifier("MyNamespace", "IContactEndpoint"))
            {
                Interfaces = {ElementEndpoint(contactDto).ToInterface()},
                Properties =
                {
                    Property("Note", "The note for a specific contact.", noteEndpoint.ToInterface()),
                    Property("Poke", "Pokes a contact.", ActionEndpoint.ToInterface()),
                    Property("Picture", "A picture of a specific contact.", BlobEndpoint.ToInterface())
                },
                Description = "A specific contact."
            };
            var contactEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "ContactEndpoint"))
            {
                BaseClass = new CSharpClassConstruction(ElementEndpoint(contactDto))
                {
                    Parameters =
                    {
                        Referrer,
                        new CSharpParameter(CSharpIdentifier.Uri, "relativeUri")
                    }
                },
                Properties =
                {
                    Property("Note", "The note for a specific contact.", noteEndpoint.ToInterface(), noteEndpoint, "./note"),
                    Property("Poke", "Pokes a contact.", ActionEndpoint.ToInterface(), ActionEndpoint, "./poke"),
                    Property("Picture", "A picture of a specific contact.", BlobEndpoint.ToInterface(), BlobEndpoint, "./picture")
                },
                Interfaces = {contactEndpointInterface.Identifier},
                Description = contactEndpointInterface.Description
            };

            var collectionEndpoint = CollectionEndpoint(contactDto, contactEndpoint.Identifier);

            var entryEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "MyServiceClient"))
            {
                BaseClass = new CSharpClassConstruction(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                {
                    Parameters =
                    {
                        new CSharpParameter(CSharpIdentifier.Uri, "uri")
                    }
                },
                Properties =
                {
                    Property("Contacts", "Collection of contacts.", CollectionEndpoint(contactDto, contactEndpointInterface.Identifier).ToInterface(), collectionEndpoint, "./contacts")
                }
            };

            generated.Should().BeEquivalentTo(
                contactDto, noteDto,
                entryEndpoint, contactEndpointInterface, contactEndpoint);
        }

        private static CSharpClass Dto(string name)
            => new CSharpClass(new CSharpIdentifier("MyNamespace", name));

        private static CSharpParameter Referrer
            => new CSharpParameter(new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint"), "referrer")
            {
                ThisReference = true
            };

        private static CSharpProperty Property(string name, string description, CSharpIdentifier interfaceType, CSharpIdentifier? implementationType = null, string? relativeUri = null)
        {
            var property = new CSharpProperty(interfaceType, name)
            {
                Description = description
            };
            if (implementationType != null)
            {
                property.GetterExpression = new CSharpClassConstruction(implementationType)
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

        private static CSharpIdentifier ElementEndpoint(CSharpClass dto)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "ElementEndpoint")
            {
                TypeArguments = {dto.Identifier}
            };

        private static CSharpIdentifier CollectionEndpoint(CSharpClass dto, CSharpIdentifier elementEndpoint)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "CollectionEndpoint")
            {
                TypeArguments = {dto.Identifier, elementEndpoint}
            };
    }
}
