using System;
using System.Collections.Generic;
using System.Linq;
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

            var entryEndpoint = GenerateEndpoint("entry", document.GetTypedRest() ?? throw new ArgumentException("No TypedRest endpoints set.", nameof(document)));
            types.AddRange(entryEndpoint.types);

            if (GenerateDtos)
                types.AddRange(document.Components.Schemas.Select(x => new CSharpDto(Naming.DtoType(x.Key), x.Value)));

            return types;
        }

        public (CSharpProperty property, IEnumerable<ICSharpType> types) GenerateEndpoint(string key, IEndpoint endpoint)
            => _builders.For(endpoint).Build(key, endpoint, this);
    }
}
