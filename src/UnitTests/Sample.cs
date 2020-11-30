using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;
using TypedRest.CodeGeneration.Endpoints.Raw;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration
{
    public static class Sample
    {
        public static string YamlV2 => ReadYamlFile("sample-v2.yml");

        public static string YamlV3 => ReadYamlFile("sample-v3.yml");

        public static OpenApiDocument Doc => new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = "My Service",
                Version = "1.0.0"
            },
            Paths = Paths,
            Components = new OpenApiComponents
            {
                Schemas =
                {
                    ["Contact"] = ContactSchema,
                    ["Note"] = NoteSchema
                }
            }
        }.SetTypedRest(EntryEndpoint);

        public static OpenApiPaths Paths => new()
        {
            ["/"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(response: StringProperty())
                }
            },
            ["/contacts"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(response: new OpenApiSchema {Type = "array", Items = ContactSchema}, description: "Collection of contacts."),
                    [OperationType.Post] = Operation(statusCode: HttpStatusCode.Created, request: ContactSchema, response: ContactSchema)
                }
            },
            ["/contacts/{id}"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(parameter: "id", response: ContactSchema, description: "A specific contact."),
                    [OperationType.Put] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, request: ContactSchema),
                    [OperationType.Delete] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent)
                }
            },
            ["/contacts/{id}/note"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(parameter: "id", response: NoteSchema, description: "The note for a specific contact."),
                    [OperationType.Put] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, request: NoteSchema)
                }
            },
            ["/contacts/{id}/poke"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Post] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, description: "Pokes a contact.")
                }
            },
            ["/contacts/{id}/picture"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(parameter: "id", mimeType: "image/jpeg", response: new OpenApiSchema(), description: "A picture of a specific contact."),
                    [OperationType.Put] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, mimeType: "image/jpeg", request: new OpenApiSchema()),
                }
            }
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

        public static OpenApiSchema ContactSchema => new()
        {
            Reference = SchemaReference("Contact"),
            Required = {"firstName", "lastName"},
            Type = "object",
            Properties =
            {
                ["id"] = StringProperty("The ID of the contact."),
                ["firstName"] = StringProperty("The first name of the contact."),
                ["lastName"] = StringProperty("The last name of the contact.")
            },
            Description = "A contact in an address book."
        };

        public static OpenApiSchema NoteSchema => new()
        {
            Reference = SchemaReference("Note"),
            Required = {"content"},
            Type = "object",
            Properties =
            {
                ["content"] = StringProperty("The content of the note.")
            },
            Description = "A note about a specific contact."
        };

        public static OpenApiOperation Operation(HttpStatusCode statusCode = HttpStatusCode.OK, string? parameter = null, string? mimeType = "application/json", OpenApiSchema? request = null, OpenApiSchema? response = null, string? description = "")
        {
            var responseObj = new OpenApiResponse {Description = description};
            if (response != null)
                responseObj.Content[mimeType] = new OpenApiMediaType {Schema = response};

            var operation = new OpenApiOperation
            {
                Responses = new OpenApiResponses
                {
                    [((int)statusCode).ToString()] = responseObj
                }
            };

            if (parameter != null)
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = parameter,
                    In = ParameterLocation.Path,
                    Required = true,
                    Schema = StringProperty()
                });
            }

            if (request != null)
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content =
                    {
                        [mimeType] = new OpenApiMediaType {Schema = request}
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
            => new() {Type = "string", Description = description};

        private static OpenApiReference SchemaReference(string id)
            => new() {Id = id, Type = ReferenceType.Schema};
    }
}
