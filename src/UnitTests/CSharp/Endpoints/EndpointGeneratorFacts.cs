using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Rpc;
using CollectionEndpointModel = TypedRest.CodeGeneration.Endpoints.Generic.CollectionEndpoint;
using ElementEndpointModel = TypedRest.CodeGeneration.Endpoints.Generic.ElementEndpoint;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

public class EndpointGeneratorFacts
{
    private readonly EndpointGenerator _generator = new(
        new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
        BuilderRegistry.Default);

    [Fact]
    public void GeneratesCorrectSource()
    {
        var generated = _generator.Generate(Sample.EntryEndpoint);

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
            BaseConstructor = new(ElementEndpoint("Contact"))
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
            BaseConstructor = new(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
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

        generated.Should().BeEquivalentTo([entryEndpointInterface, entryEndpoint, contactEndpointInterface, contactEndpoint]);
    }

    [Fact]
    public void PrefixesCollidingKeysWithParent()
    {
        var entry = new EntryEndpoint
        {
            Children =
            {
                ["users"] = new Endpoint
                {
                    Uri = "./users",
                    Children = {["settings"] = new Endpoint {Uri = "./settings", Children = {["theme"] = new ActionEndpoint {Uri = "./theme"}}}}
                },
                ["accounts"] = new Endpoint
                {
                    Uri = "./accounts",
                    Children = {["settings"] = new Endpoint {Uri = "./settings", Children = {["limit"] = new ActionEndpoint {Uri = "./limit"}}}}
                }
            }
        };

        var generatedNames = _generator.Generate(entry).OfType<CSharpClass>().Select(x => x.Identifier.Name).ToList();

        generatedNames.Should().Contain(["UsersSettingsEndpoint", "AccountsSettingsEndpoint"]);
        generatedNames.Should().NotContain("SettingsEndpoint");
    }

    [Fact]
    public void NumbersTypeNamesThatCollideDespitePrefixes()
    {
        // Both "commits" endpoints share their key and their parent's key, so the parent prefix is not enough
        var entry = new EntryEndpoint
        {
            Children =
            {
                ["contracts"] = new Endpoint
                {
                    Uri = "./contracts",
                    Children = {["commits"] = new Endpoint {Uri = "./commits", Children = {["edit"] = new ActionEndpoint {Uri = "./edit"}}}}
                },
                ["customers"] = new Endpoint
                {
                    Uri = "./customers",
                    Children =
                    {
                        ["contracts"] = new Endpoint
                        {
                            Uri = "./contracts",
                            Children = {["commits"] = new Endpoint {Uri = "./commits", Children = {["retire"] = new ActionEndpoint {Uri = "./retire"}}}}
                        }
                    }
                }
            }
        };

        var generatedNames = _generator.Generate(entry).OfType<CSharpClass>().Select(x => x.Identifier.Name).ToList();

        generatedNames.Should().Contain(["ContractsCommitsEndpoint", "CustomersContractsCommitsEndpoint"]);
        generatedNames.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public void AddsExplicitImplementationsForElementEndpointInterfaces()
    {
        var entry = new EntryEndpoint
        {
            Children =
            {
                // The child endpoint forces a custom class and interface to be generated for the collection itself
                ["contacts"] = new CollectionEndpointModel
                {
                    Uri = "./contacts",
                    Schema = Sample.ContactSchema,
                    Element = new ElementEndpointModel {Schema = Sample.ContactSchema},
                    Children = {["poke"] = new ActionEndpoint {Uri = "./poke"}}
                }
            }
        };

        var collection = _generator.Generate(entry)
                                   .OfType<CSharpClass>()
                                   .Single(x => x.Identifier.Name == "ContactsEndpoint");

        // The base class hands out ElementEndpoint<Contact>, but the interface promises IElementEndpoint<Contact>
        collection.Indexers.Should().SatisfyRespectively(
            indexer =>
            {
                indexer.ExplicitInterface!.Name.Should().Be("IIndexerEndpoint");
                indexer.Type.Name.Should().Be("IElementEndpoint");
                indexer.Parameter.Name.Should().Be("id");
                indexer.GetterExpression.Should().Be("this[id]");
            },
            indexer =>
            {
                indexer.ExplicitInterface!.Name.Should().Be("ICollectionEndpoint");
                indexer.Type.Name.Should().Be("IElementEndpoint");
                indexer.Parameter.Type.Name.Should().Be("Contact");
                indexer.GetterExpression.Should().Be("this[entity]");
            });

        var createAsync = collection.Methods.Should().ContainSingle().Subject;
        createAsync.Name.Should().Be("CreateAsync");
        createAsync.ExplicitInterface!.Name.Should().Be("ICollectionEndpoint");
        createAsync.ReturnType.TypeArguments.Single().Nullable.Should().BeTrue();
        createAsync.BodyExpression.Should().Be("CreateAsync(entity, cancellationToken)");
        collection.NullableContext.Should().BeTrue();
    }

    [Fact]
    public void OmitsExplicitImplementationsWithoutInterfaces()
    {
        var generator = new EndpointGenerator(
            new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
            BuilderRegistry.Default)
        {
            WithInterfaces = false
        };

        var contactEndpoint = generator.Generate(Sample.EntryEndpoint)
                                       .OfType<CSharpClass>()
                                       .Single(x => x.Identifier.Name == "ContactElementEndpoint");

        contactEndpoint.Indexers.Should().BeEmpty();
        contactEndpoint.Methods.Should().BeEmpty();
    }

    [Fact]
    public void OmitsEntryConstructorWhenDisabled()
    {
        var generator = new EndpointGenerator(
            new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
            BuilderRegistry.Default)
        {
            GenerateEntryConstructor = false
        };

        var entry = generator.Generate(Sample.EntryEndpoint)
                             .OfType<CSharpClass>()
                             .Single(x => x.Identifier.Name == "MyServiceClient");

        // The base class has to survive, so the partial class the consumer writes can still call base(...)
        entry.BaseConstructor!.Type.Name.Should().Be("EntryEndpoint");
        entry.BaseConstructor.Parameters.Should().BeEmpty();
        entry.Properties.Should().NotBeEmpty();
    }

    private static CSharpParameter Referrer
        => new(new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint"), "referrer")
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
            property.GetterExpression = new(implementationType)
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
        => new("TypedRest.Endpoints.Rpc", "ActionEndpoint");

    private static CSharpIdentifier BlobEndpoint
        => new("TypedRest.Endpoints.Raw", "BlobEndpoint");

    private static CSharpIdentifier ElementEndpoint(string dto)
        => new("TypedRest.Endpoints.Generic", "ElementEndpoint")
        {
            TypeArguments = {new CSharpIdentifier("MyNamespace", dto)}
        };

    private static CSharpIdentifier CollectionEndpoint(string dto, CSharpIdentifier elementEndpoint)
        => new("TypedRest.Endpoints.Generic", "CollectionEndpoint")
        {
            TypeArguments = {new CSharpIdentifier("MyNamespace", dto), elementEndpoint}
        };
}
