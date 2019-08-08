using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi
{
    public static class Sample
    {
        public static string YamlV2 => ReadYamlFile("sample-v2.yml");

        public static string YamlV3 => ReadYamlFile("sample-v3.yml");

        public static OpenApiDocument Doc => new OpenApiDocument
        {
            Info = new OpenApiInfo {Title = "My Service"},
            Paths = Paths,
            Components = new OpenApiComponents
            {
                Schemas =
                {
                    ["Contact"] = ContactSchema,
                    ["Note"] = NoteSchema
                }
            }
        }.SetTypedRestEndpoints(Endpoints);

        public static OpenApiPaths Paths => new OpenApiPaths
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
                    [OperationType.Get] = Operation(response: new OpenApiSchema {Type = "array", Items = ContactSchema}, summary: "Collection of contacts."),
                    [OperationType.Post] = Operation(statusCode: HttpStatusCode.Created, request: ContactSchema, response: ContactSchema)
                }
            },
            ["/contacts/{id}"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(parameter: "id", response: ContactSchema, summary: "A specific contact."),
                    [OperationType.Put] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, request: ContactSchema),
                    [OperationType.Delete] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent)
                }
            },
            ["/contacts/{id}/note"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(parameter: "id", response: NoteSchema, summary: "The note for a specific contact."),
                    [OperationType.Put] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, request: NoteSchema)
                }
            },
            ["/contacts/{id}/poke"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Post] = Operation(parameter: "id", statusCode: HttpStatusCode.NoContent, summary: "Pokes a contact.")
                }
            },
        };

        public static EndpointList Endpoints => new EndpointList
        {
            ["contacts"] = new CollectionEndpoint
            {
                Description = "Collection of contacts.",
                Uri = "./contacts",
                Schema = ReferenceTo(ContactSchema),
                Element = new ElementEndpoint
                {
                    Description = "A specific contact.",
                    Children =
                    {
                        ["note"] = new ElementEndpoint
                        {
                            Description = "The note for a specific contact.",
                            Uri = "./note",
                            Schema = ReferenceTo(NoteSchema)
                        },
                        ["poke"] = new ActionEndpoint
                        {
                            Description = "Pokes a contact.",
                            Uri = "./poke"
                        }
                    }
                }
            }
        };

        public static OpenApiSchema ContactSchema => new OpenApiSchema
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

        public static OpenApiSchema NoteSchema => new OpenApiSchema
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

        public static OpenApiOperation Operation(HttpStatusCode statusCode = HttpStatusCode.OK, string parameter = null, string mimeType = "application/json", OpenApiSchema request = null, OpenApiSchema response = null, string summary = null)
        {
            var responseObj = new OpenApiResponse();
            if (response != null)
                responseObj.Content[mimeType] = new OpenApiMediaType {Schema = response};

            var operation = new OpenApiOperation
            {
                Summary = summary,
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
            using (var reader = new StreamReader(Assembly.GetAssembly(type).GetManifestResourceStream(type, name)))
                return reader.ReadToEnd().TrimEnd();
        }

        private static OpenApiSchema StringProperty(string description = null)
            => new OpenApiSchema {Type = "string", Description = description};

        private static OpenApiSchema ReferenceTo(OpenApiSchema schema)
            => new OpenApiSchema {Reference = SchemaReference(schema.Reference.Id)};

        private static OpenApiReference SchemaReference(string id)
            => new OpenApiReference {Id = id, Type = ReferenceType.Schema};
    }
}
