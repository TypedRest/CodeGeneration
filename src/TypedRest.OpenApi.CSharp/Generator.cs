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

        public List<ICSharpType> Generate(EntryEndpoint endpoint, IDictionary<string, OpenApiSchema> schemas)
        {
            var types = new List<ICSharpType>();

            var entryEndpoint = GenerateEndpoint("entry", endpoint);
            types.AddRange(entryEndpoint.types);

            if (GenerateDtos)
            {
                // TODO: Proper implementation
                foreach (var pair in schemas)
                    types.Add(new CSharpClass(Naming.DtoType(pair.Key)));
            }

            return types;
        }

        public (CSharpProperty property, IEnumerable<ICSharpType> types) GenerateEndpoint(string key, IEndpoint endpoint)
            => _builders.For(endpoint).Build(key, endpoint, this);
    }
}
