using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;

namespace TypedRest.OpenApi.CSharp
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

        private ICSharpType GetDto(string key, OpenApiSchema schema)
        {
            var type = new CSharpClass(_naming.DtoType(key)) {Description = schema.Description};
            foreach ((string propKey, var propSchema) in schema.Properties)
            {
                type.Properties.Add(new CSharpProperty(_naming.TypeFor(propSchema), _naming.Property(propKey))
                {
                    Description = propSchema.Description,
                    HasSetter = true
                });
            }
            return type;
        }
    }
}
