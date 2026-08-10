using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

public class EndpointGenerator(INamingStrategy namingStrategy, BuilderRegistry builders) : IEndpointGenerator
{
    public INamingStrategy Naming { get; } = namingStrategy;

    public bool WithInterfaces { get; set; } = true;

    public bool GenerateEntryConstructor { get; set; } = true;

    private HashSet<string> _collidingKeys = new();
    private readonly Stack<string> _parentKeys = new();
    private TypeNameRegistry _typeNames = new();

    public IEnumerable<ICSharpType> Generate(EntryEndpoint endpoint)
    {
        _collidingKeys = EndpointTree.FindCollidingKeys(endpoint);
        _parentKeys.Clear();
        _typeNames = new();

        var types = new List<ICSharpType>();
        types.AddRange(Generate("entry", endpoint).types);
        return types;
    }

    public (CSharpProperty property, IEnumerable<ICSharpType> types) Generate(string key, IEndpoint endpoint)
        => builders.For(endpoint).Build(key, endpoint, this);

    public CSharpIdentifier EndpointType(string key, IEndpoint endpoint)
        => _typeNames.Register(NameCandidates(key, endpoint));

    /// <summary>
    /// Returns increasingly qualified names for an endpoint: the bare key, then the key prefixed with its parent,
    /// its grandparent, and so on. Keys that are known to collide skip the bare name.
    /// </summary>
    private List<CSharpIdentifier> NameCandidates(string key, IEndpoint endpoint)
    {
        var candidates = new List<CSharpIdentifier>();
        if (!_collidingKeys.Contains(key))
            candidates.Add(Naming.EndpointType(key, endpoint));

        // The bottom of the stack is the entry endpoint, which would only contribute a meaningless prefix
        string prefix = "";
        foreach (string parentKey in _parentKeys.Take(Math.Max(_parentKeys.Count - 1, 0)))
        {
            prefix = parentKey + "_" + prefix;
            candidates.Add(Naming.EndpointType(key, endpoint, prefix));
        }

        if (candidates.Count == 0)
            candidates.Add(Naming.EndpointType(key, endpoint));

        return candidates;
    }

    public void PushParent(string key) => _parentKeys.Push(key);

    public void PopParent() => _parentKeys.Pop();
}
