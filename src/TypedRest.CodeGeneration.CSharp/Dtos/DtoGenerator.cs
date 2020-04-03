using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp.Dtos
{
    public class DtoGenerator
    {
        private readonly INamingStrategy _naming;

        public DtoGenerator(INamingStrategy naming)
        {
            _naming = naming;
        }

        public IEnumerable<ICSharpType> Generate(IEnumerable<KeyValuePair<string, OpenApiSchema>> schemas)
        {
            foreach ((string key, var schema) in schemas)
                yield return GetDto(key, schema);
        }

        protected virtual ICSharpType GetDto(string key, OpenApiSchema schema)
        {
            var dto = new CSharpClass(_naming.DtoType(key))
            {
                Summary = schema.Description,
                Attributes = {Attributes.GeneratedCode}
            };

            foreach ((string propKey, var propSchema) in schema.Properties)
            {
                var property = GetProperty(propKey, propSchema, dto.Identifier);

                if (schema.Required.Contains(propKey))
                    property.Attributes.Add(Attributes.Required);

                if (propKey.Equals("id", StringComparison.InvariantCultureIgnoreCase))
                    property.Attributes.Add(Attributes.Key);

                dto.Properties.Add(property);
            }

            return dto;
        }

        protected virtual CSharpProperty GetProperty(string key, OpenApiSchema? schema, CSharpIdentifier dtoIdentifier)
        {
            string propertyName = _naming.Property(key);
            if (propertyName == dtoIdentifier.Name)
                propertyName += "Value";

            return new CSharpProperty(_naming.TypeFor(schema), propertyName)
            {
                Summary = schema?.Description,
                Attributes = {Attributes.JsonProperty(key)},
                HasSetter = true
            };
        }
    }
}
