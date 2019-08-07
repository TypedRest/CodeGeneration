using System.IO;
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
                    [OperationType.Get] = Operation(response: new OpenApiSchema {Type = "array", Items = ContactSchema}, summary: "All contacts."),
                    [OperationType.Post] = Operation(statusCode: 201, request: ContactSchema, response: ContactSchema)
                }
            },
            ["/contacts/{id}"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(parameter: "id", response: ContactSchema, summary: "A specific contact."),
                    [OperationType.Put] = Operation(parameter: "id", statusCode: 204, request: ContactSchema),
                    [OperationType.Delete] = Operation(parameter: "id", statusCode: 204)
                }
            },
            ["/contacts/{id}/note"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Get] = Operation(parameter: "id", response: NoteSchema, summary: "The note for a specific contact."),
                    [OperationType.Put] = Operation(parameter: "id", statusCode: 204, request: NoteSchema)
                }
            },
            ["/contacts/{id}/poke"] = new OpenApiPathItem
            {
                Operations =
                {
                    [OperationType.Post] = Operation(parameter: "id", statusCode: 204, summary: "Pokes a contact.")
                }
            },
        };

        public static EndpointList Endpoints => new EndpointList
        {
            ["contacts"] = new CollectionEndpoint
            {
                Description = "a collection of contacts",
                Uri = "./contacts",
                Schema = ReferenceTo(ContactSchema),
                Element = new ElementEndpoint
                {
                    Schema = ReferenceTo(ContactSchema),
                    Children =
                    {
                        ["note"] = new ElementEndpoint
                        {
                            Uri = "./note",
                            Schema = ReferenceTo(NoteSchema)
                        },
                        ["poke"] = new ActionEndpoint
                        {
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

        public static OpenApiOperation Operation(int statusCode = 200, string parameter = null, OpenApiSchema request = null, OpenApiSchema response = null, string summary = null)
        {
            var responseObj = new OpenApiResponse();
            if (response != null)
                responseObj.Content["application/json"] = new OpenApiMediaType {Schema = response};

            var operation = new OpenApiOperation
            {
                Summary = summary,
                Responses = new OpenApiResponses
                {
                    [statusCode.ToString()] = responseObj
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
                        ["application/json"] = new OpenApiMediaType {Schema = request}
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
