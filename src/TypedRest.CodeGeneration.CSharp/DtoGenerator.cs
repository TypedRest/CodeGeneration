using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp
{
    public class DtoGenerator
    {
        private readonly INamingStrategy _naming;

        public DtoGenerator(INamingStrategy naming)
        {
            _naming = naming;
        }

        public IEnumerable<ICSharpType> Generate(IDictionary<string, OpenApiSchema> schemas)
        {
            foreach ((string key, var schema) in schemas)
                yield return GetDto(key, schema);
        }

        protected virtual ICSharpType GetDto(string key, OpenApiSchema schema)
        {
            var type = new CSharpClass(_naming.DtoType(key))
            {
                Summary = schema.Description,
                Attributes = {Attributes.GeneratedCode}
            };

            foreach ((string propKey, var propSchema) in schema.Properties)
                type.Properties.Add(GetProperty(propKey, propSchema, schema));

            return type;
        }

        protected virtual CSharpProperty GetProperty(string key, OpenApiSchema? schema, OpenApiSchema dtoSchema)
        {
            var property = new CSharpProperty(_naming.TypeFor(schema), _naming.Property(key))
            {
                Summary = schema?.Description,
                Attributes = {Attributes.JsonProperty(key)},
                HasSetter = true
            };

            if (dtoSchema.Required.Contains(key))
                property.Attributes.Add(Attributes.Required);

            if (key.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                property.Attributes.Add(Attributes.Key);

            return property;
        }
    }
}
