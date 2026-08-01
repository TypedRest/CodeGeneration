using Microsoft.CodeAnalysis.CSharp;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos;

public class DtoGeneratorFacts
{
    private readonly DtoGenerator _generator = new(
        new NamingStrategy("MyService", "MyNamespace", "MyNamespace"));

    [Fact]
    public void GeneratesClasses()
    {
        _generator.Generate(new Dictionary<string, OpenApiSchema>
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
        Type = "string",
        Enum = new List<IOpenApiAny>
        {
            new OpenApiString("value1"),
            new OpenApiString("value2")
        }
    };

    private static readonly CSharpEnum _dtoEnum = DtoEnum("MyEnum", "My enum",
        DtoEnumValue("Value1", "value1"),
        DtoEnumValue("Value2", "value2"));

    [Fact]
    public void GeneratesEnums()
    {
        _generator.Generate(new Dictionary<string, OpenApiSchema>
        {
            ["myEnum"] = _enumSchema
        }).Should().BeEquivalentTo([_dtoEnum]);
    }

    [Fact]
    public void GeneratesUniqueEnumValueNames()
    {
        _generator.Generate(new Dictionary<string, OpenApiSchema>
        {
            ["myEnum"] = new()
            {
                Description = "My enum",
                Type = "string",
                Enum =
                {
                    new OpenApiString(""),
                    new OpenApiString("1st"),
                    new OpenApiString("foo-bar"),
                    new OpenApiString("foo_bar")
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
        _generator.Generate(new Dictionary<string, OpenApiSchema>
        {
            ["myEnum"] = new()
            {
                Description = "My enum",
                Type = "integer",
                Enum = {new OpenApiInteger(-1), new OpenApiInteger(1)}
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
        _generator.Generate(new Dictionary<string, OpenApiSchema>
        {
            ["myType"] = new()
            {
                Description = "My type",
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["legacy"] = new() {Type = "string", Deprecated = true, Description = "My legacy value."}
                }
            }
        }).Should().BeEquivalentTo([
            DtoClass("MyType", "My type",
                new CSharpProperty(CSharpIdentifier.String.ToNullable(), "Legacy")
                {
                    Summary = "My legacy value.",
                    Attributes = {Attributes.JsonProperty("legacy"), Attributes.Obsolete},
                    HasSetter = true
                })
        ]);
    }

    [Fact]
    public void GeneratesInlineEnums()
    {
        _generator.Generate(new Dictionary<string, OpenApiSchema>
        {
            ["myType"] = new()
            {
                Description = "My type",
                Type = "object",
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["myEnum"] = _enumSchema
                }
            }
        }).Should().BeEquivalentTo(new CSharpType[]
        {
            DtoClass("MyType", "My type",
                Property("MyEnum", "myEnum", "My enum", type: _dtoEnum.Identifier)),
            _dtoEnum
        });
    }

    [Fact]
    public void GeneratesNullableProperties()
    {
        _generator.Generate(new Dictionary<string, OpenApiSchema>
        {
            ["myType"] = new()
            {
                Description = "My type",
                Type = "object",
                Required = {"name"},
                Properties = new Dictionary<string, OpenApiSchema>
                {
                    ["name"] = new() {Type = "string", Nullable = true, Description = "My name."},
                    ["age"] = new() {Type = "integer", Description = "My age."}
                }
            }
        }).Should().BeEquivalentTo([
            DtoClass("MyType", "My type",
                new CSharpProperty(CSharpIdentifier.String.ToNullable(), "Name")
                {
                    Summary = "My name.",
                    Attributes = {Attributes.JsonProperty("name"), Attributes.Required},
                    HasSetter = true
                },
                new CSharpProperty(CSharpIdentifier.Int.ToNullable(), "Age")
                {
                    Summary = "My age.",
                    Attributes = {Attributes.JsonProperty("age")},
                    HasSetter = true
                })
        ]);
    }

    private static readonly OpenApiSchema _collectionSchema = new()
    {
        Description = "My type",
        Type = "object",
        Required = {"tags"},
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["notes"] = new() {Type = "array", Items = new OpenApiSchema {Type = "string"}, Description = "My notes."},
            ["tags"] = new() {Type = "object", AdditionalProperties = new OpenApiSchema {Type = "string"}, Description = "My tags."}
        }
    };

    [Fact]
    public void GeneratesInitializedCollections()
    {
        var list = CSharpIdentifier.ListOf(CSharpIdentifier.String);
        var dictionary = CSharpIdentifier.DictionaryOf(CSharpIdentifier.String, CSharpIdentifier.String);

        _generator.Generate(new Dictionary<string, OpenApiSchema> {["myType"] = _collectionSchema})
                  .Should().BeEquivalentTo([
                       DtoClass("MyType", "My type",
                           new CSharpProperty(list, "Notes")
                           {
                               Summary = "My notes.",
                               Attributes = {Attributes.JsonProperty("notes")},
                               HasSetter = true,
                               Initializer = new CSharpObjectCreation(list)
                           },
                           new CSharpProperty(dictionary, "Tags")
                           {
                               Summary = "My tags.",
                               Attributes = {Attributes.JsonProperty("tags"), Attributes.Required},
                               HasSetter = true,
                               Initializer = new CSharpObjectCreation(dictionary)
                           })
                   ]);
    }

    private static readonly OpenApiSchema _requiredSchema = new()
    {
        Description = "My type",
        Type = "object",
        Required = {"name", "age"},
        Properties = new Dictionary<string, OpenApiSchema>
        {
            ["name"] = new() {Type = "string", Description = "My name."},
            ["age"] = new() {Type = "integer", Description = "My age."}
        }
    };

    [Fact]
    public void GeneratesRequiredMembers()
    {
        _generator.Generate(new Dictionary<string, OpenApiSchema> {["myType"] = _requiredSchema})
                  .Should().BeEquivalentTo([
                       DtoClass("MyType", "My type",
                           new CSharpProperty(CSharpIdentifier.String, "Name")
                           {
                               Summary = "My name.",
                               Attributes = {Attributes.JsonProperty("name"), Attributes.Required},
                               HasSetter = true,
                               IsRequired = true
                           },
                           new CSharpProperty(CSharpIdentifier.Int, "Age")
                           {
                               Summary = "My age.",
                               Attributes = {Attributes.JsonProperty("age"), Attributes.Required},
                               HasSetter = true,
                               IsRequired = true
                           })
                   ]);
    }

    [Fact]
    public void GeneratesNullForgivingInitializersBelowCSharp11()
    {
        Generator(LanguageVersion.CSharp10)
           .Generate(new Dictionary<string, OpenApiSchema> {["myType"] = _requiredSchema})
           .Should().BeEquivalentTo([
                DtoClass("MyType", "My type",
                    new CSharpProperty(CSharpIdentifier.String, "Name")
                    {
                        Summary = "My name.",
                        Attributes = {Attributes.JsonProperty("name"), Attributes.Required},
                        HasSetter = true,
                        InitializerExpression = "null!"
                    },
                    new CSharpProperty(CSharpIdentifier.Int, "Age")
                    {
                        Summary = "My age.",
                        Attributes = {Attributes.JsonProperty("age"), Attributes.Required},
                        HasSetter = true
                    })
            ]);
    }

    [Fact]
    public void OmitsNullableReferenceTypesBelowCSharp8()
    {
        var list = CSharpIdentifier.ListOf(CSharpIdentifier.String);

        Generator(LanguageVersion.CSharp7_3)
           .Generate(new Dictionary<string, OpenApiSchema>
            {
                ["myType"] = new()
                {
                    Description = "My type",
                    Type = "object",
                    Required = {"name"},
                    Properties = new Dictionary<string, OpenApiSchema>
                    {
                        ["name"] = new() {Type = "string", Description = "My name."},
                        ["nickname"] = new() {Type = "string", Nullable = true, Description = "My nickname."},
                        ["age"] = new() {Type = "integer", Description = "My age."},
                        ["aliases"] = new() {Type = "array", Items = new OpenApiSchema {Type = "string", Nullable = true}, Description = "My aliases."}
                    }
                }
            })
           .Should().BeEquivalentTo([
                DtoClass("MyType", "My type", nullableContext: false,
                    new CSharpProperty(CSharpIdentifier.String, "Name")
                    {
                        Summary = "My name.",
                        Attributes = {Attributes.JsonProperty("name"), Attributes.Required},
                        HasSetter = true
                    },
                    new CSharpProperty(CSharpIdentifier.String, "Nickname")
                    {
                        Summary = "My nickname.",
                        Attributes = {Attributes.JsonProperty("nickname")},
                        HasSetter = true
                    },
                    // Nullable value types predate C# 8, so they are unaffected
                    new CSharpProperty(CSharpIdentifier.Int.ToNullable(), "Age")
                    {
                        Summary = "My age.",
                        Attributes = {Attributes.JsonProperty("age")},
                        HasSetter = true
                    },
                    new CSharpProperty(list, "Aliases")
                    {
                        Summary = "My aliases.",
                        Attributes = {Attributes.JsonProperty("aliases")},
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
            Attributes = {Attributes.JsonProperty(jsonName)},
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
        => new(name) {Attributes = { Attributes.EnumMember(jsonName)}};
}
