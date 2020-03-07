using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Newtonsoft.Json;
using NJsonSchema;

namespace TypedRest.OpenApi.CSharp
{
    public static class OpenApiSchemaExtensions
    {
        public static JsonSchema ToNJsonSchema(this OpenApiSchema schema)
        {
            using var writer = new StringWriter();
            schema.SerializeAsV3WithoutReference(new OpenApiJsonWriter(writer));

            return JsonConvert.DeserializeObject<JsonSchema>(writer.ToString(), new JsonSerializerSettings
            {
                ContractResolver = JsonSchema.CreateJsonSerializerContractResolver(SchemaType.OpenApi3)
            });
        }

    }
}
