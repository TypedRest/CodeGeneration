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
            var generated = generator.Generate(Sample.Doc);

            var noteDto = Dto("Note", "A note about a specific contact.",
                Property("Content", "The content of the note.", CSharpIdentifier.String));
            var noteEndpoint = ElementEndpoint(noteDto);

            var contactDto = Dto("Contact", "A contact in an address book.",
                Property("Id", "The ID of the contact.", CSharpIdentifier.String),
                Property("FirstName", "The first name of the contact.", CSharpIdentifier.String),
                Property("LastName", "The last name of the contact.", CSharpIdentifier.String));
            var contactEndpointInterface = new CSharpInterface(new CSharpIdentifier("MyNamespace", "IContactElementEndpoint"))
            {
                Interfaces = {ElementEndpoint(contactDto).ToInterface()},
                Description = "A specific contact.",
                Properties =
                {
                    Property("Note", "The note for a specific contact.", noteEndpoint.ToInterface()),
                    Property("Poke", "Pokes a contact.", ActionEndpoint.ToInterface()),
                    Property("Picture", "A picture of a specific contact.", BlobEndpoint.ToInterface())
                }
            };
            var contactEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "ContactElementEndpoint"))
            {
                BaseClass = new CSharpClassConstruction(ElementEndpoint(contactDto))
                {
                    Parameters =
                    {
                        Referrer,
                        new CSharpParameter(CSharpIdentifier.Uri, "relativeUri")
                    }
                },
                Interfaces = {contactEndpointInterface.Identifier},
                Description = contactEndpointInterface.Description,
                Properties =
                {
                    Property("Note", "The note for a specific contact.", noteEndpoint.ToInterface(), noteEndpoint, "./note"),
                    Property("Poke", "Pokes a contact.", ActionEndpoint.ToInterface(), ActionEndpoint, "./poke"),
                    Property("Picture", "A picture of a specific contact.", BlobEndpoint.ToInterface(), BlobEndpoint, "./picture")
                }
            };

            var collectionEndpoint = CollectionEndpoint(contactDto, contactEndpoint.Identifier);

            var entryEndpointInterface = new CSharpInterface(new CSharpIdentifier("MyNamespace", "IMyServiceClient"))
            {
                Interfaces = {new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint")},
                Properties =
                {
                    Property("Contacts", "Collection of contacts.", CollectionEndpoint(contactDto, contactEndpointInterface.Identifier).ToInterface())
                }
            };
            var entryEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "MyServiceClient"))
            {
                BaseClass = new CSharpClassConstruction(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                {
                    Parameters =
                    {
                        new CSharpParameter(CSharpIdentifier.Uri, "uri")
                    }
                },
                Interfaces = {entryEndpointInterface.Identifier},
                Properties =
                {
                    Property("Contacts", "Collection of contacts.", CollectionEndpoint(contactDto, contactEndpointInterface.Identifier).ToInterface(), collectionEndpoint, "./contacts")
                }
            };

            generated.Should().BeEquivalentTo(
                contactDto, noteDto,
                entryEndpointInterface, entryEndpoint, contactEndpointInterface, contactEndpoint);
        }

        private static CSharpClass Dto(string name, string description, params CSharpProperty[] properties)
        {
            var type = new CSharpClass(new CSharpIdentifier("MyNamespace", name))
            {
                Description = description
            };
            foreach (var property in properties)
                property.HasSetter = true;
            type.Properties.AddRange(properties);
            return type;
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

        private static CSharpIdentifier ElementEndpoint(ICSharpType dto)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "ElementEndpoint")
            {
                TypeArguments = {dto.Identifier}
            };

        private static CSharpIdentifier CollectionEndpoint(ICSharpType dto, CSharpIdentifier elementEndpoint)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "CollectionEndpoint")
            {
                TypeArguments = {dto.Identifier, elementEndpoint}
            };
    }
}
