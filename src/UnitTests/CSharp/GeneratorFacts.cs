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
            var generator = new Generator(new NamingConvention("MyNamespace"));
            var generated = generator.Generate(Sample.Endpoints);

            var noteSchema = new CSharpIdentifier("Schemas", "Note");
            var noteEndpoint = ElementEndpoint(noteSchema);

            var contactSchema = new CSharpIdentifier("Schemas", "Contact");
            var contactEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "ContactEndpoint"))
            {
                BaseClass = new CSharpClassConstruction(ElementEndpoint(contactSchema))
                {
                    Parameters =
                    {
                        Referrer,
                        new CSharpParameter(CSharpIdentifier.Uri, "relativeUri")
                    }
                },
                Properties =
                {
                    Property("Note", "./note", noteEndpoint, description: "The note for a specific contact."),
                    Property("Poke", "./poke", ActionEndpoint, description: "Pokes a contact."),
                    Property("Picture", "./picture", BlobEndpoint, description: "A picture of a specific contact.")
                },
                Description = "A specific contact."
            };
            var collectionEndpoint = CollectionEndpoint(contactSchema, contactEndpoint.Identifier);

            var entryEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "MyEntryEndpoint"))
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
                    Property("Contacts", "./contacts", collectionEndpoint, description: "Collection of contacts.")
                }
            };

            generated.Should().BeEquivalentTo(entryEndpoint, contactEndpoint);
        }

        private static CSharpParameter Referrer
            => new CSharpParameter(new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint"), "referrer")
            {
                ThisReference = true
            };

        private static CSharpProperty Property(string name, string relativeUri, CSharpIdentifier implementationType, CSharpIdentifier interfaceType = null, string description = null)
            => new CSharpProperty(interfaceType ?? implementationType.ToInterface(), name)
            {
                GetterExpression = new CSharpClassConstruction(implementationType)
                {
                    Parameters =
                    {
                        Referrer,
                        new CSharpParameter(CSharpIdentifier.String, "relativeUri") {Value = relativeUri}
                    }
                },
                Description = description
            };

        private static CSharpIdentifier ActionEndpoint
            => new CSharpIdentifier("TypedRest.Endpoints.Rpc", "ActionEndpoint");

        private static CSharpIdentifier BlobEndpoint
            => new CSharpIdentifier("TypedRest.Endpoints.Raw", "BlobEndpoint");

        private static CSharpIdentifier ElementEndpoint(CSharpIdentifier schema)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "ElementEndpoint")
            {
                TypeArguments = {schema}
            };

        private static CSharpIdentifier CollectionEndpoint(CSharpIdentifier schema, CSharpIdentifier elementEndpoint)
            => new CSharpIdentifier("TypedRest.Endpoints.Generic", "CollectionEndpoint")
            {
                TypeArguments = {schema, elementEndpoint}
            };
    }
}
