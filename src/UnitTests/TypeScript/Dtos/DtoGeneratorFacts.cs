using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Dtos;

public class DtoGeneratorFacts
{
    private readonly DtoGenerator _generator = new(new NamingStrategy("MyService", "", "dtos"));

    private string Generate(params (string key, OpenApiSchema schema)[] schemas)
    {
        var types = _generator.Generate(schemas.Select(x => new KeyValuePair<string, OpenApiSchema>(x.key, x.schema))).ToList();

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
            Type = "object",
            Required = {"first-name"},
            Properties =
            {
                ["first-name"] = new OpenApiSchema {Type = "string"},
                ["snake_case"] = new OpenApiSchema {Type = "string"},
                ["2fa"] = new OpenApiSchema {Type = "boolean"}
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
            Type = "object",
            Required = {"required"},
            Properties =
            {
                ["required"] = new OpenApiSchema {Type = "string"},
                ["optional"] = new OpenApiSchema {Type = "string"},
                ["nullable"] = new OpenApiSchema {Type = "string", Nullable = true, Required = {}},
                ["both"] = new OpenApiSchema {Type = "string", Nullable = true}
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
            Type = "object",
            AllOf =
            {
                new OpenApiSchema {Reference = new OpenApiReference {Id = "Person", Type = ReferenceType.Schema}},
                new OpenApiSchema {Reference = new OpenApiReference {Id = "Auditable", Type = ReferenceType.Schema}},
                new OpenApiSchema {Type = "object", Required = {"salary"}, Properties = {["salary"] = new OpenApiSchema {Type = "number"}}}
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
            Type = "string",
            Description = "How urgent something is.",
            Enum = {new OpenApiString("low"), new OpenApiString("high"), new OpenApiString("")}
        })).Should().Contain(
            """
            /** How urgent something is. */
            export type Priority = "low" | "high" | "";
            """.ReplaceLineEndings("\n"));

    [Fact]
    public void GeneratesIntegerEnumAsLiteralUnion()
        => Generate(("Level", new OpenApiSchema
        {
            Type = "integer",
            Enum = {new OpenApiInteger(-1), new OpenApiInteger(0), new OpenApiInteger(1), new OpenApiInteger(1)}
        })).Should().Contain("export type Level = -1 | 0 | 1;");

    [Fact]
    public void LiftsInlineEnumsIntoNamedTypes()
    {
        string content = Generate(("Task", new OpenApiSchema
        {
            Type = "object",
            Required = {"status"},
            Properties =
            {
                ["status"] = new OpenApiSchema {Type = "string", Enum = {new OpenApiString("open"), new OpenApiString("done")}}
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
            Type = "object",
            Required = {"lines"},
            Properties =
            {
                ["lines"] = new OpenApiSchema
                {
                    Type = "array",
                    Items = new OpenApiSchema {Type = "object", Required = {"sku"}, Properties = {["sku"] = new OpenApiSchema {Type = "string"}}}
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
            new("TaskStatus", new OpenApiSchema {Type = "string", Enum = {new OpenApiString("other")}}),
            new("Task", new OpenApiSchema
            {
                Type = "object",
                Properties =
                {
                    ["status"] = new OpenApiSchema {Type = "string", Enum = {new OpenApiString("open")}}
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
            Type = "object",
            Deprecated = true,
            Properties = {["old"] = new OpenApiSchema {Type = "string", Deprecated = true}}
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
        => _generator.Generate([new("Name", new OpenApiSchema {Type = "string"})]).Should().BeEmpty();
}
