using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Builders;
using NanoByte.CodeGeneration;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public class EndpointGenerator : IEndpointGenerator
    {
        public INamingStrategy Naming { get; }

        private readonly BuilderRegistry _builders;

        public EndpointGenerator(INamingStrategy namingStrategy, BuilderRegistry builders)
        {
            Naming = namingStrategy;
            _builders = builders;
        }

        public bool WithInterfaces { get; set; } = true;

        public IEnumerable<ICSharpType> Generate(EntryEndpoint endpoint)
        {
            var types = new List<ICSharpType>();
            types.AddRange(Generate("entry", endpoint).types);
            return types;
        }

        public (CSharpProperty property, IEnumerable<ICSharpType> types) Generate(string key, IEndpoint endpoint)
            => _builders.For(endpoint).Build(key, endpoint, this);
    }
}
