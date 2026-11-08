using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Dtos;

public class DtoGeneratorFacts
{
    private readonly DtoGenerator _generator = new(new NamingStrategy("MyService", "", "dtos"));

    private string Generate(params (string key, IOpenApiSchema schema)[] schemas)
    {
        var types = _generator.Generate(schemas.Select(x => new KeyValuePair<string, IOpenApiSchema>(x.key, x.schema))).ToList();

        var file = new TsFile("dtos/Combined.ts");
        file.Types.AddRange(types);
        return file.GetContent();
    }

    [Fact]
    public void GeneratesInterfaceWithOptionalAndRequiredProperties()
        => Generate(("Contact", Sample.ContactSchema)).Should().Contain(
            """
            /** A contact in an address book. */
            export interface Contact {
              /** The ID of the contact. */
              id?: string;
              /** The first name of the contact. */
              firstName: string;
              /** The last name of the contact. */
              lastName: string;
            }
            """.ReplaceLineEndings("\n"));

    [Fact]
    public void KeepsWireNamesAndQuotesInvalidIdentifiers()
        => Generate(("Thing", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> {"first-name"},
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["first-name"] = new OpenApiSchema {Type = JsonSchemaType.String},
                ["snake_case"] = new OpenApiSchema {Type = JsonSchemaType.String},
                ["2fa"] = new OpenApiSchema {Type = JsonSchemaType.Boolean}
            }
        })).Should().Contain(
            """
            export interface Thing {
              "first-name": string;
              snake_case?: string;
              "2fa"?: boolean;
            }
            """.ReplaceLineEndings("\n"));

    [Fact]
    public void KeepsOptionalAndNullableDistinct()
        => Generate(("Thing", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> {"required"},
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["required"] = new OpenApiSchema {Type = JsonSchemaType.String},
                ["optional"] = new OpenApiSchema {Type = JsonSchemaType.String},
                ["nullable"] = new OpenApiSchema {Type = JsonSchemaType.String | JsonSchemaType.Null, Required = new HashSet<string>()},
                ["both"] = new OpenApiSchema {Type = JsonSchemaType.String | JsonSchemaType.Null}
            }
        })).Should().Contain(
            """
            export interface Thing {
              required: string;
              optional?: string;
              nullable?: string | null;
              both?: string | null;
            }
            """.ReplaceLineEndings("\n"));

    [Fact]
    public void TurnsEveryReferencedAllOfEntryIntoAnExtends()
        => Generate(("Employee", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            AllOf = new List<IOpenApiSchema>
            {
                new OpenApiSchemaReference("Person"),
                new OpenApiSchemaReference("Auditable"),
                new OpenApiSchema {Type = JsonSchemaType.Object, Required = new HashSet<string> {"salary"}, Properties = new Dictionary<string, IOpenApiSchema> {["salary"] = new OpenApiSchema {Type = JsonSchemaType.Number}}}
            }
        })).Should().Contain(
            """
            export interface Employee extends Person, Auditable {
              salary: number;
            }
            """.ReplaceLineEndings("\n"));

    [Fact]
    public void GeneratesStringEnumAsLiteralUnion()
        => Generate(("Priority", new OpenApiSchema
        {
            Type = JsonSchemaType.String,
            Description = "How urgent something is.",
            Enum = new List<JsonNode> {"low", "high", ""}
        })).Should().Contain(
            """
            /** How urgent something is. */
            export type Priority = "low" | "high" | "";
            """.ReplaceLineEndings("\n"));

    [Fact]
    public void GeneratesIntegerEnumAsLiteralUnion()
        => Generate(("Level", new OpenApiSchema
        {
            Type = JsonSchemaType.Integer,
            Enum = new List<JsonNode> {-1, 0, 1, 1}
        })).Should().Contain("export type Level = -1 | 0 | 1;");

    [Fact]
    public void LiftsInlineEnumsIntoNamedTypes()
    {
        string content = Generate(("Task", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> {"status"},
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["status"] = new OpenApiSchema {Type = JsonSchemaType.String, Enum = new List<JsonNode> {"open", "done"}}
            }
        }));

        content.Should().Contain("status: TaskStatus;");
        content.Should().Contain("""export type TaskStatus = "open" | "done";""");
    }

    [Fact]
    public void LiftsInlineObjectsInArraysIntoNamedTypes()
    {
        string content = Generate(("Order", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Required = new HashSet<string> {"lines"},
            Properties = new Dictionary<string, IOpenApiSchema>
            {
                ["lines"] = new OpenApiSchema
                {
                    Type = JsonSchemaType.Array,
                    Items = new OpenApiSchema {Type = JsonSchemaType.Object, Required = new HashSet<string> {"sku"}, Properties = new Dictionary<string, IOpenApiSchema> {["sku"] = new OpenApiSchema {Type = JsonSchemaType.String}}}
                }
            }
        }));

        content.Should().Contain("lines: OrderLine[];");
        content.Should().Contain(
            """
            export interface OrderLine {
              sku: string;
            }
            """.ReplaceLineEndings("\n"));
    }

    [Fact]
    public void NumbersInlineTypeNamesCollidingWithDocumentSchemas()
    {
        var types = _generator.Generate(
        [
            new("TaskStatus", new OpenApiSchema {Type = JsonSchemaType.String, Enum = new List<JsonNode> {"other"}}),
            new("Task", new OpenApiSchema
            {
                Type = JsonSchemaType.Object,
                Properties = new Dictionary<string, IOpenApiSchema>
                {
                    ["status"] = new OpenApiSchema {Type = JsonSchemaType.String, Enum = new List<JsonNode> {"open"}}
                }
            })
        ]).ToList();

        types.Select(x => x.Identifier.Name).Should().Equal("TaskStatus", "Task", "TaskStatus2");
        types.Select(x => x.Identifier.Module!.Specifier).Should().Equal("dtos/TaskStatus", "dtos/Task", "dtos/TaskStatus2");
    }

    [Fact]
    public void MarksDeprecatedSchemasAndProperties()
        => Generate(("Legacy", new OpenApiSchema
        {
            Type = JsonSchemaType.Object,
            Deprecated = true,
            Properties = new Dictionary<string, IOpenApiSchema> {["old"] = new OpenApiSchema {Type = JsonSchemaType.String, Deprecated = true}}
        })).Should().Contain(
            """
            /** @deprecated */
            export interface Legacy {
              /** @deprecated */
              old?: string;
            }
            """.ReplaceLineEndings("\n"));

    [Fact]
    public void GeneratesNothingForSchemasThatNeedNoType()
        => _generator.Generate([new("Name", new OpenApiSchema {Type = JsonSchemaType.String})]).Should().BeEmpty();
}
