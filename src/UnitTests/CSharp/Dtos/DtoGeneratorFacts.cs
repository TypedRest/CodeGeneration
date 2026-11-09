using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoGeneratorFacts
{
    private readonly DtoGenerator _generator = new(
        new NamingStrategy("MyService", "MyNamespace", "MyNamespace"));

    /// <summary>
    /// The attributes of the default serializer, which is what <see cref="_generator"/> annotates for.
    /// </summary>
    private static readonly JsonAttributes _json = JsonAttributes.For(null);

    [Fact]
    public void GeneratesClasses()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["contact"] = Sample.ContactSchema,
            ["note"] = Sample.NoteSchema
        }).Should().BeEquivalentTo([
            DtoClass("Contact", "A contact in an address book.",
                Property("Id", "id", "The ID of the contact.", key: true),
                Property("FirstName", "firstName", "The first name of the contact.", required: true),
                Property("LastName", "lastName", "The last name of the contact.", required: true)),
            DtoClass("Note", "A note about a specific contact.",
                Property("Content", "content", "The content of the note.", required: true))
        ]);
    }

    private static readonly OpenApiSchema _enumSchema = new()
    {
        Description = "My enum",
        Type = JsonSchemaType.String,
        Enum = new List<JsonNode>
        {
            "value1",
            "value2"
        }
    };

    private static readonly CSharpEnum _dtoEnum = DtoEnum("MyEnum", "My enum",
        DtoEnumValue("Value1", "value1"),
        DtoEnumValue("Value2", "value2"));

    [Fact]
    public void GeneratesEnums()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myEnum"] = _enumSchema
        }).Should().BeEquivalentTo([_dtoEnum]);
    }

    [Fact]
    public void NumbersInlineTypeNamesCollidingWithDocumentSchemas()
    {
        var generated = _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            // The inline enum for this property wants to be called MyEnum, just like the schema below
            ["my"] = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema> {["enum"] = _enumSchema}
            },
            ["myEnum"] = _enumSchema
        }).ToList();

        // The schema from the document keeps the name, because $refs point at it
        generated.Select(x => x.Identifier.Name).Should().Equal("My", "MyEnum2", "MyEnum");

        generated.OfType<CSharpClass>().Single()
                 .Properties.Single().Type.Name.Should().Be("MyEnum2");
    }

    [Fact]
    public void GeneratesInheritanceFromAllOf()
    {
        var baseType = new OpenApiSchemaReference("myBase");

        var derived = _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myDerived"] = new OpenApiSchema()
            {
                Description = "My derived type",
                AllOf = new List<IOpenApiSchema>
                {
                    baseType,
                    new OpenApiSchema
                    {
                        Type = JsonSchemaType.Object,
                        Required = new HashSet<string> {"name"},
                        Properties = new Dictionary<string, IOpenApiSchema> {["name"] = new OpenApiSchema() {Type = JsonSchemaType.String, Description = "My name."}}
                    }
                }
            }
        }).Should().ContainSingle().Subject.Should().BeOfType<CSharpClass>().Subject;

        // The $ref becomes the base class, the inline schema is merged in
        derived.BaseConstructor!.Type.Should().BeEquivalentTo(new CSharpIdentifier("MyNamespace", "MyBase"));
        derived.Properties.Should().BeEquivalentTo([
            new CSharpProperty(CSharpIdentifier.String, "Name")
            {
                Summary = "My name.",
                Attributes = {_json.PropertyName("name"), Attributes.Required},
                HasSetter = true,
                IsRequired = true
            }
        ]);
    }

    [Fact]
    public void GeneratesUniqueEnumValueNames()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myEnum"] = new OpenApiSchema()
            {
                Description = "My enum",
                Type = JsonSchemaType.String,
                Enum = new List<JsonNode>
                {
                    "",
                    "1st",
                    "foo-bar",
                    "foo_bar"
                }
            }
        }).Should().BeEquivalentTo([
            DtoEnum("MyEnum", "My enum",
                DtoEnumValue("Empty", ""),
                DtoEnumValue("_1st", "1st"),
                DtoEnumValue("FooBar", "foo-bar"),
                DtoEnumValue("FooBar2", "foo_bar"))
        ]);
    }

    [Fact]
    public void GeneratesNamesForNegativeEnumValues()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myEnum"] = new OpenApiSchema()
            {
                Description = "My enum",
                Type = JsonSchemaType.Integer,
                Enum = new List<JsonNode> {-1, 1}
            }
        }).Should().BeEquivalentTo([
            DtoEnum("MyEnum", "My enum",
                new CSharpEnumValue("ValueMinus1") {Value = -1},
                new CSharpEnumValue("Value1") {Value = 1})
        ]);
    }

    [Fact]
    public void MarksDeprecatedPropertiesAsObsolete()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myType"] = new OpenApiSchema()
            {
                Description = "My type",
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["legacy"] = new OpenApiSchema() {Type = JsonSchemaType.String, Deprecated = true, Description = "My legacy value."}
                }
            }
        }).Should().BeEquivalentTo([
            DtoClass("MyType", "My type",
                new CSharpProperty(CSharpIdentifier.String.ToNullable(), "Legacy")
                {
                    Summary = "My legacy value.",
                    Attributes = {_json.PropertyName("legacy"), Attributes.Obsolete},
                    HasSetter = true
                })
        ]);
    }

    [Fact]
    public void GeneratesInlineEnums()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myType"] = new OpenApiSchema()
            {
                Description = "My type",
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["myEnum"] = _enumSchema
                }
            }
        }).Should().BeEquivalentTo(new CSharpType[]
        {
            // The enum is named after the containing type as well as the property, to avoid collisions
            DtoClass("MyType", "My type",
                Property("MyEnum", "myEnum", "My enum", type: new CSharpIdentifier("MyNamespace", "MyTypeMyEnum"))),
            DtoEnum("MyTypeMyEnum", "My enum",
                DtoEnumValue("Value1", "value1"),
                DtoEnumValue("Value2", "value2"))
        });
    }

    [Fact]
    public void GeneratesClassesForInlineObjects()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myType"] = new OpenApiSchema()
            {
                Description = "My type",
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["address"] = new OpenApiSchema()
                    {
                        Description = "My address.",
                        Type = JsonSchemaType.Object,
                        Properties = new Dictionary<string, IOpenApiSchema> {["city"] = new OpenApiSchema() {Type = JsonSchemaType.String}}
                    }
                }
            }
        }).Should().BeEquivalentTo(new CSharpType[]
        {
            DtoClass("MyType", "My type",
                Property("Address", "address", "My address.", type: new CSharpIdentifier("MyNamespace", "MyTypeAddress"))),
            DtoClass("MyTypeAddress", "My address.",
                new CSharpProperty(CSharpIdentifier.String.ToNullable(), "City")
                {
                    Attributes = {_json.PropertyName("city")},
                    HasSetter = true
                })
        });
    }

    [Fact]
    public void GeneratesNullableProperties()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myType"] = new OpenApiSchema()
            {
                Description = "My type",
                Type = JsonSchemaType.Object,
                Required = new HashSet<string> {"name"},
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["name"] = new OpenApiSchema() {Type = JsonSchemaType.String | JsonSchemaType.Null, Description = "My name."},
                    ["age"] = new OpenApiSchema() {Type = JsonSchemaType.Integer, Description = "My age."}
                }
            }
        }).Should().BeEquivalentTo([
            DtoClass("MyType", "My type",
                new CSharpProperty(CSharpIdentifier.String.ToNullable(), "Name")
                {
                    Summary = "My name.",
                    Attributes = {_json.PropertyName("name"), Attributes.Required},
                    HasSetter = true
                },
                new CSharpProperty(CSharpIdentifier.Int.ToNullable(), "Age")
                {
                    Summary = "My age.",
                    Attributes = {_json.PropertyName("age")},
                    HasSetter = true
                })
        ]);
    }

    private static readonly OpenApiSchema _collectionSchema = new()
    {
        Description = "My type",
        Type = JsonSchemaType.Object,
        Required = new HashSet<string> {"tags"},
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["notes"] = new OpenApiSchema() {Type = JsonSchemaType.Array, Items = new OpenApiSchema {Type = JsonSchemaType.String}, Description = "My notes."},
            ["tags"] = new OpenApiSchema() {Type = JsonSchemaType.Object, AdditionalProperties = new OpenApiSchema {Type = JsonSchemaType.String}, Description = "My tags."}
        }
    };

    [Fact]
    public void GeneratesInitializedCollections()
    {
        var list = CSharpIdentifier.ListOf(CSharpIdentifier.String);
        var dictionary = CSharpIdentifier.DictionaryOf(CSharpIdentifier.String, CSharpIdentifier.String);

        _generator.Generate(new Dictionary<string, IOpenApiSchema> {["myType"] = _collectionSchema})
                  .Should().BeEquivalentTo([
                       DtoClass("MyType", "My type",
                           new CSharpProperty(list, "Notes")
                           {
                               Summary = "My notes.",
                               Attributes = {_json.PropertyName("notes")},
                               HasSetter = true,
                               Initializer = new CSharpObjectCreation(list)
                           },
                           new CSharpProperty(dictionary, "Tags")
                           {
                               Summary = "My tags.",
                               Attributes = {_json.PropertyName("tags"), Attributes.Required},
                               HasSetter = true,
                               Initializer = new CSharpObjectCreation(dictionary)
                           })
                   ]);
    }

    private static readonly OpenApiSchema _requiredSchema = new()
    {
        Description = "My type",
        Type = JsonSchemaType.Object,
        Required = new HashSet<string> {"name", "age"},
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["name"] = new OpenApiSchema() {Type = JsonSchemaType.String, Description = "My name."},
            ["age"] = new OpenApiSchema() {Type = JsonSchemaType.Integer, Description = "My age."}
        }
    };

    [Fact]
    public void GeneratesRequiredMembers()
    {
        _generator.Generate(new Dictionary<string, IOpenApiSchema> {["myType"] = _requiredSchema})
                  .Should().BeEquivalentTo([
                       DtoClass("MyType", "My type",
                           new CSharpProperty(CSharpIdentifier.String, "Name")
                           {
                               Summary = "My name.",
                               Attributes = {_json.PropertyName("name"), Attributes.Required},
                               HasSetter = true,
                               IsRequired = true
                           },
                           new CSharpProperty(CSharpIdentifier.Int, "Age")
                           {
                               Summary = "My age.",
                               Attributes = {_json.PropertyName("age"), Attributes.Required},
                               HasSetter = true,
                               IsRequired = true
                           })
                   ]);
    }

    [Fact]
    public void GeneratesNullForgivingInitializersBelowCSharp11()
    {
        Generator(LanguageVersion.CSharp10)
           .Generate(new Dictionary<string, IOpenApiSchema> {["myType"] = _requiredSchema})
           .Should().BeEquivalentTo([
                DtoClass("MyType", "My type",
                    new CSharpProperty(CSharpIdentifier.String, "Name")
                    {
                        Summary = "My name.",
                        Attributes = {_json.PropertyName("name"), Attributes.Required},
                        HasSetter = true,
                        InitializerExpression = "null!"
                    },
                    new CSharpProperty(CSharpIdentifier.Int, "Age")
                    {
                        Summary = "My age.",
                        Attributes = {_json.PropertyName("age"), Attributes.Required},
                        HasSetter = true
                    })
            ]);
    }

    [Fact]
    public void OmitsNullableReferenceTypesBelowCSharp8()
    {
        var list = CSharpIdentifier.ListOf(CSharpIdentifier.String);

        Generator(LanguageVersion.CSharp7_3)
           .Generate(new Dictionary<string, IOpenApiSchema>
            {
                ["myType"] = new OpenApiSchema()
                {
                    Description = "My type",
                    Type = JsonSchemaType.Object,
                    Required = new HashSet<string> {"name"},
                    Properties = new Dictionary<string, IOpenApiSchema>
                    {
                        ["name"] = new OpenApiSchema() {Type = JsonSchemaType.String, Description = "My name."},
                        ["nickname"] = new OpenApiSchema() {Type = JsonSchemaType.String | JsonSchemaType.Null, Description = "My nickname."},
                        ["age"] = new OpenApiSchema() {Type = JsonSchemaType.Integer, Description = "My age."},
                        ["aliases"] = new OpenApiSchema() {Type = JsonSchemaType.Array, Items = new OpenApiSchema {Type = JsonSchemaType.String | JsonSchemaType.Null}, Description = "My aliases."}
                    }
                }
            })
           .Should().BeEquivalentTo([
                DtoClass("MyType", "My type", nullableContext: false,
                    new CSharpProperty(CSharpIdentifier.String, "Name")
                    {
                        Summary = "My name.",
                        Attributes = {_json.PropertyName("name"), Attributes.Required},
                        HasSetter = true
                    },
                    new CSharpProperty(CSharpIdentifier.String, "Nickname")
                    {
                        Summary = "My nickname.",
                        Attributes = {_json.PropertyName("nickname")},
                        HasSetter = true
                    },
                    // Nullable value types predate C# 8, so they are unaffected
                    new CSharpProperty(CSharpIdentifier.Int.ToNullable(), "Age")
                    {
                        Summary = "My age.",
                        Attributes = {_json.PropertyName("age")},
                        HasSetter = true
                    },
                    new CSharpProperty(list, "Aliases")
                    {
                        Summary = "My aliases.",
                        Attributes = {_json.PropertyName("aliases")},
                        HasSetter = true,
                        Initializer = new CSharpObjectCreation(list)
                    })
            ]);
    }

    private static DtoGenerator Generator(LanguageVersion languageVersion)
        => new(new NamingStrategy("MyService", "MyNamespace", "MyNamespace"), languageVersion);

    private static CSharpClass DtoClass(string name, string description, params CSharpProperty[] properties)
        => DtoClass(name, description, nullableContext: true, properties);

    private static CSharpClass DtoClass(string name, string description, bool nullableContext, params CSharpProperty[] properties)
    {
        var type = new CSharpClass(new CSharpIdentifier("MyNamespace", name))
        {
            Summary = description,
            Attributes = {Attributes.GeneratedCode},
            NullableContext = nullableContext
        };
        type.Properties.AddRange(properties);
        return type;
    }

    private static CSharpProperty Property(string name, string jsonName, string description, bool required = false, bool key = false, CSharpIdentifier? type = null)
    {
        type ??= CSharpIdentifier.String;

        var property = new CSharpProperty(required ? type : type.ToNullable(), name)
        {
            Summary = description,
            Attributes = {_json.PropertyName(jsonName)},
            HasSetter = true
        };
        if (required)
        {
            property.Attributes.Add(Attributes.Required);
            property.IsRequired = true;
        }
        if (key) property.Attributes.Add(Attributes.Key);
        return property;
    }

    private static CSharpEnum DtoEnum(string name, string description, params CSharpEnumValue[] values)
    {
        var type = new CSharpEnum(new CSharpIdentifier("MyNamespace", name))
        {
            Summary = description,
            Attributes = {Attributes.GeneratedCode},
            NullableContext = true
        };
        type.Values.AddRange(values);
        return type;
    }

    private static CSharpEnumValue DtoEnumValue(string name, string jsonName)
        => new(name) {Attributes = { _json.EnumMemberName(jsonName)}};

    [Fact]
    public void AnnotatesPropertiesForTheChosenSerializer()
    {
        var generator = new DtoGenerator(
            new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
            jsonAttributes: JsonAttributes.For(JsonAttributes.SystemTextJson));

        var generated = generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myType"] = new OpenApiSchema()
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema> {["myName"] = new OpenApiSchema() {Type = JsonSchemaType.String}}
            }
        }).OfType<CSharpClass>().Single();

        generated.Properties.Single().Attributes.Should().BeEquivalentTo([
            new CSharpAttribute(new CSharpIdentifier("System.Text.Json.Serialization", "JsonPropertyNameAttribute"))
            {
                Arguments = {"myName"}
            }
        ]);
    }

    [Fact]
    public void AnnotatesEnumValuesForTheChosenSerializer()
    {
        var generator = new DtoGenerator(
            new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
            jsonAttributes: JsonAttributes.For(JsonAttributes.SystemTextJson));

        var generated = generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myEnum"] = _enumSchema
        }).OfType<CSharpEnum>().Single();

        // System.Text.Json ignores [EnumMember], so the values carry [JsonStringEnumMemberName] instead
        generated.Values.Select(x => x.Attributes.Single().Identifier.Name)
                 .Should().AllBe("JsonStringEnumMemberNameAttribute");
    }

    [Fact]
    public void AddsStringEnumConverterForSystemTextJson()
    {
        var generator = new DtoGenerator(
            new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
            jsonAttributes: JsonAttributes.For(JsonAttributes.SystemTextJson));

        var generated = generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myEnum"] = _enumSchema
        }).OfType<CSharpEnum>().Single();

        // SystemTextJsonSerializer has no string enum converter by default, so the enum brings its own
        generated.Attributes.Should().ContainEquivalentOf(
            new CSharpAttribute(new CSharpIdentifier("System.Text.Json.Serialization", "JsonConverterAttribute"))
            {
                Arguments =
                {
                    new CSharpIdentifier("System.Text.Json.Serialization", "JsonStringEnumConverter")
                    {
                        TypeArguments = {new CSharpIdentifier("MyNamespace", "MyEnum")}
                    }
                }
            });
    }

    [Fact]
    public void DoesNotAddStringEnumConverterForIntegerEnums()
    {
        var generator = new DtoGenerator(
            new NamingStrategy("MyService", "MyNamespace", "MyNamespace"),
            jsonAttributes: JsonAttributes.For(JsonAttributes.SystemTextJson));

        var generated = generator.Generate(new Dictionary<string, IOpenApiSchema>
        {
            ["myEnum"] = new OpenApiSchema
            {
                Type = JsonSchemaType.Integer,
                Enum = new List<JsonNode> {1, 2}
            }
        }).OfType<CSharpEnum>().Single();

        generated.Attributes.Should().NotContain(x => x.Identifier.Name == "JsonConverterAttribute");
    }

    [Fact]
    public void DefaultsToNewtonsoft()
        => JsonAttributes.For(null).Serializer.Should().Be(JsonAttributes.Newtonsoft);

    [Fact]
    public void RejectsUnknownSerializers()
        => new Func<JsonAttributes>(() => JsonAttributes.For("protobuf"))
          .Should().Throw<ArgumentException>().WithMessage("*protobuf*");
}
