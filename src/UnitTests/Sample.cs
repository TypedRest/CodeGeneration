using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi
{
    public static class Sample
    {
        public static readonly string YamlV2 = ReadYamlFile("sample-v2.yml");

        public static readonly string YamlV3 = ReadYamlFile("sample-v3.yml");

        private static string ReadYamlFile(string name)
        {
            var type = typeof(Sample);
            using (var reader = new StreamReader(Assembly.GetAssembly(type).GetManifestResourceStream(type, name)))
            {
                var lines = reader.ReadToEnd().Split('\n');
                return string.Join("\n", lines.Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#")).ToArray());
            }
        }

        public static readonly OpenApiDocument Doc = new OpenApiDocument
        {
            Info = new OpenApiInfo {Title = "My Service"},
            Paths = new OpenApiPaths
            {
                //["/"] = new OpenApiPathItem(),
                //["/contacts"] = new OpenApiPathItem(),
                //["/contacts/{id}"] = new OpenApiPathItem(),
                //["/contacts/{id}/note"] = new OpenApiPathItem(),
                //["/contacts/{id}/poke"] = new OpenApiPathItem()
            },
            //Components = new OpenApiComponents
            //{
            //    Schemas =
            //    {
            //        ["Contact"] = new OpenApiSchema(),
            //        ["Note"] = new OpenApiSchema()
            //    }
            //}
        }.SetTypedRestEndpoints(new EndpointList
        {
            ["contacts"] = new CollectionEndpoint
            {
                Description = "a collection of contacts",
                Uri = "./contacts",
                Schema = SchemaReference("Contact"),
                Element = new ElementEndpoint
                {
                    Schema = SchemaReference("Contact"),
                    Children =
                    {
                        ["note"] = new ElementEndpoint
                        {
                            Uri = "./note",
                            Schema = SchemaReference("Note")
                        },
                        ["poke"] = new ActionEndpoint
                        {
                            Uri = "./poke"
                        }
                    }
                }
            }
        });

        private static OpenApiSchema SchemaReference(string id)
            => new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Id = id,
                    Type = ReferenceType.Schema
                }
            };
    }
}
