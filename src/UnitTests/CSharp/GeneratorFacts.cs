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
                        new CSharpParameter(CSharpIdentifier.String, "relativeUri")
                    }
                },
                Properties =
                {
                    Property(noteEndpoint, "Note", "./note"),
                    Property(ActionEndpoint, "Poke", "./poke"),
                    Property(BlobEndpoint, "Picture", "./picture")
                }
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
                    Property(collectionEndpoint, "Contacts", "./contacts")
                }
            };

            generated.Should().BeEquivalentTo(entryEndpoint, contactEndpoint);
        }

        private static CSharpParameter Referrer
            => new CSharpParameter(new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint"), "referrer")
            {
                ThisReference = true
            };

        private static CSharpProperty Property(CSharpIdentifier endpoint, string name, string relativeUri)
            => new CSharpProperty(endpoint.ToInterface(), name)
            {
                GetterExpression = new CSharpClassConstruction(endpoint)
                {
                    Parameters =
                    {
                        Referrer,
                        new CSharpParameter(CSharpIdentifier.String, "relativeUri") {Value = relativeUri}
                    }
                }
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
