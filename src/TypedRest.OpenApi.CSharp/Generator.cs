using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Builders;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public class Generator : IGenerator
    {
        public INamingConvention Naming { get; }

        private readonly BuilderRegistry _builders;

        public Generator(INamingConvention namingConvention, BuilderRegistry? builders = null)
        {
            Naming = namingConvention;
            _builders = builders ?? BuilderRegistry.Default;
        }

        public bool GenerateInterfaces { get; set; } = true;

        public bool GenerateDtos { get; set; } = true;

        public List<ICSharpType> Generate(OpenApiDocument document)
        {
            var types = new List<ICSharpType>();
            var entryEndpoint = document.GetTypedRest() ?? throw new ArgumentException("No TypedRest endpoints set.", nameof(document));

            types.AddRange(GetEndpoints("entry", entryEndpoint).types);

            if (GenerateDtos)
            {
                foreach ((string key, var schema) in document.Components.Schemas)
                    types.Add(GetDto(key, schema));
            }

            return types;
        }

        public (CSharpProperty property, IEnumerable<ICSharpType> types) GetEndpoints(string key, IEndpoint endpoint)
            => _builders.For(endpoint).Build(key, endpoint, this);

        private ICSharpType GetDto(string key, OpenApiSchema schema)
        {
            var type = new CSharpClass(Naming.DtoType(key)) {Description = schema.Description};
            foreach ((string propKey, var propSchema) in schema.Properties)
            {
                type.Properties.Add(new CSharpProperty(Naming.TypeFor(propSchema), Naming.Property(propKey))
                {
                    Description = propSchema.Description,
                    HasSetter = true
                });
            }
            return type;
        }
    }
}
