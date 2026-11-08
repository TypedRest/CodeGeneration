using System.Net;
using System.Reflection;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.Endpoints.Raw;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration;

public static class Sample
{
    public static string YamlV2 => ReadYamlFile("sample-v2.yml");

    public static string YamlV3 => ReadYamlFile("sample-v3.yml");

    public static OpenApiDocument Doc
    {
        get
        {
            var doc = DocWithComponents();
            doc.Paths = Paths;
            return doc.SetTypedRest(EntryEndpoint);
        }
    }

    /// <summary>
    /// The document that <see cref="ContactSchema"/> and <see cref="NoteSchema"/> resolve against.
    /// </summary>
    private static readonly OpenApiDocument HostDocument = DocWithComponents();

    private static OpenApiDocument DocWithComponents()
    {
        var doc = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "My Service",
                Version = "1.0.0"
            },
            Components = new OpenApiComponents
            {
                Schemas = new Dictionary<string, IOpenApiSchema>
                {
                    ["Contact"] = ContactSchemaDefinition,
                    ["Note"] = NoteSchemaDefinition
                }
            }
        };
        doc.RegisterComponents();
        return doc;
    }

    public static OpenApiPaths Paths => new()
    {
        ["/"] = PathItem(
            (HttpMethod.Get, Operation(response: StringProperty()))),
        ["/contacts"] = PathItem(
            (HttpMethod.Get, Operation(response: new OpenApiSchema {Type = JsonSchemaType.Array, Items = ContactSchema}, description: "Collection of contacts.")),
            (HttpMethod.Post, Operation(statusCode: HttpStatusCode.Created, request: ContactSchema, response: ContactSchema))),
        ["/contacts/{id}"] = PathItem(
            (HttpMethod.Get, Operation(parameter: "id", response: ContactSchema, description: "A specific contact.")),
            (HttpMethod.Put, Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, request: ContactSchema)),
            (HttpMethod.Delete, Operation(parameter: "id", statusCode: HttpStatusCode.NoContent))),
        ["/contacts/{id}/note"] = PathItem(
            (HttpMethod.Get, Operation(parameter: "id", response: NoteSchema, description: "The note for a specific contact.")),
            (HttpMethod.Put, Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, request: NoteSchema))),
        ["/contacts/{id}/poke"] = PathItem(
            (HttpMethod.Post, Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, description: "Pokes a contact."))),
        ["/contacts/{id}/picture"] = PathItem(
            (HttpMethod.Get, Operation(parameter: "id", mimeType: "image/jpeg", response: new OpenApiSchema(), description: "A picture of a specific contact.")),
            (HttpMethod.Put, Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, mimeType: "image/jpeg", request: new OpenApiSchema())))
    };

    public static EntryEndpoint EntryEndpoint => new()
    {
        Children =
        {
            ["contacts"] = new CollectionEndpoint
            {
                Description = "Collection of contacts.",
                Uri = "./contacts",
                Schema = ContactSchema,
                Element = new ElementEndpoint
                {
                    Description = "A specific contact.",
                    Children =
                    {
                        ["note"] = new ElementEndpoint
                        {
                            Description = "The note for a specific contact.",
                            Uri = "./note",
                            Schema = NoteSchema
                        },
                        ["poke"] = new ActionEndpoint
                        {
                            Description = "Pokes a contact.",
                            Uri = "./poke"
                        },
                        ["picture"] = new BlobEndpoint
                        {
                            Description = "A picture of a specific contact.",
                            Uri = "./picture"
                        }
                    }
                }
            }
        }
    };

    /// <summary>
    /// A reference to the <c>Contact</c> schema component.
    /// </summary>
    public static IOpenApiSchema ContactSchema => new OpenApiSchemaReference("Contact", HostDocument);

    /// <summary>
    /// A reference to the <c>Note</c> schema component.
    /// </summary>
    public static IOpenApiSchema NoteSchema => new OpenApiSchemaReference("Note", HostDocument);

    public static OpenApiSchema ContactSchemaDefinition => new()
    {
        Required = new HashSet<string> {"firstName", "lastName"},
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["id"] = StringProperty("The ID of the contact."),
            ["firstName"] = StringProperty("The first name of the contact."),
            ["lastName"] = StringProperty("The last name of the contact.")
        },
        Description = "A contact in an address book."
    };

    public static OpenApiSchema NoteSchemaDefinition => new()
    {
        Required = new HashSet<string> {"content"},
        Type = JsonSchemaType.Object,
        Properties = new Dictionary<string, IOpenApiSchema>
        {
            ["content"] = StringProperty("The content of the note.")
        },
        Description = "A note about a specific contact."
    };

    public static OpenApiPathItem PathItem(params (HttpMethod method, OpenApiOperation operation)[] operations)
    {
        var item = new OpenApiPathItem {Operations = new Dictionary<HttpMethod, OpenApiOperation>()};
        foreach (var (method, operation) in operations)
            item.Operations[method] = operation;
        return item;
    }

    public static OpenApiOperation Operation(HttpStatusCode statusCode = HttpStatusCode.OK, string? parameter = null, string? mimeType = "application/json", IOpenApiSchema? request = null, IOpenApiSchema? response = null, string? description = "")
    {
        var responseObj = new OpenApiResponse {Description = description};
        if (response != null)
        {
            responseObj.Content = new Dictionary<string, IOpenApiMediaType>
            {
                [mimeType!] = new OpenApiMediaType {Schema = response}
            };
        }

        var operation = new OpenApiOperation
        {
            Responses = new OpenApiResponses
            {
                [((int)statusCode).ToString()] = responseObj
            }
        };

        if (parameter != null)
        {
            operation.Parameters = new List<IOpenApiParameter>
            {
                new OpenApiParameter
                {
                    Name = parameter,
                    In = ParameterLocation.Path,
                    Required = true,
                    Schema = StringProperty()
                }
            };
        }

        if (request != null)
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, IOpenApiMediaType>
                {
                    [mimeType!] = new OpenApiMediaType {Schema = request}
                }
            };
        }

        return operation;
    }

    private static string ReadYamlFile(string name)
    {
        var type = typeof(Sample);
        using var reader = new StreamReader(Assembly.GetAssembly(type)!.GetManifestResourceStream(type, name)!);
        return reader.ReadToEnd().TrimEnd();
    }

    private static OpenApiSchema StringProperty(string? description = null)
        => new() {Type = JsonSchemaType.String, Description = description};
}
