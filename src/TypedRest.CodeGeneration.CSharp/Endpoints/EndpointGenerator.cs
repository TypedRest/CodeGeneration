using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

public class EndpointGenerator(INamingStrategy namingStrategy, BuilderRegistry builders) : IEndpointGenerator
{
    public INamingStrategy Naming { get; } = namingStrategy;

    public bool WithInterfaces { get; set; } = true;

    public IEnumerable<ICSharpType> Generate(EntryEndpoint endpoint)
    {
        var types = new List<ICSharpType>();
        types.AddRange(Generate("entry", endpoint).types);
        return types;
    }

    public (CSharpProperty property, IEnumerable<ICSharpType> types) Generate(string key, IEndpoint endpoint)
        => builders.For(endpoint).Build(key, endpoint, this);
}
