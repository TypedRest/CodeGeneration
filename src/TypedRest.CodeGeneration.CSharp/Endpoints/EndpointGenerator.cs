using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

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
        _collidingKeys = FindCollidingKeys(endpoint);
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

    /// <summary>
    /// Walks the endpoint tree the way the build pass will and returns the set of keys whose endpoints
    /// produce a custom class (i.e. have children) in more than one place.
    /// </summary>
    private static HashSet<string> FindCollidingKeys(IEndpoint root)
    {
        var counts = new Dictionary<string, int>();
        Walk("entry", root);

        return [..counts.Where(kv => kv.Value > 1).Select(kv => kv.Key)];

        void Walk(string key, IEndpoint endpoint)
        {
            if (endpoint.Children.Count > 0)
                counts[key] = counts.TryGetValue(key, out int n) ? n + 1 : 1;

            foreach ((string childKey, var child) in endpoint.Children)
                Walk(childKey, child);

            var element = endpoint switch
            {
                CollectionEndpoint c => c.Element,
                IndexerEndpoint i => i.Element,
                _ => null
            };
            if (element != null)
                Walk(key.Depluralize() + "_Element", element);
        }
    }
}
