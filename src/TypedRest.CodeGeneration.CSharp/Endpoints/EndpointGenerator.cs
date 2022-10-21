using System.Collections.Generic;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

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
