using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints;

/// <summary>
/// Generates TypeScript types for a tree of <see cref="IEndpoint"/>s.
/// </summary>
/// <param name="namingStrategy">Decides what the generated types and members are called.</param>
/// <param name="builders">Decides what code is emitted for each kind of endpoint.</param>
/// <param name="typeNames">Keeps the generated names from colliding. Share this with a DTO generator writing to the same output directory.</param>
public class EndpointGenerator(INamingStrategy namingStrategy, BuilderRegistry builders, TypeNameRegistry? typeNames = null) : IEndpointGenerator
{
    /// <inheritdoc/>
    public INamingStrategy Naming { get; } = namingStrategy;

    /// <inheritdoc/>
    public Modules Modules { get; set; } = Modules.Default;

    /// <inheritdoc/>
    public IGenerationLog Log { get; set; } = NullGenerationLog.Instance;

    private HashSet<string> _collidingKeys = [];
    private readonly Stack<string> _parentKeys = new();
    private TypeNameRegistry _typeNames = typeNames ?? new TypeNameRegistry();

    /// <summary>
    /// Generates the types for an entire client.
    /// </summary>
    public IEnumerable<ITsType> Generate(EntryEndpoint endpoint)
    {
        _collidingKeys = EndpointTree.FindCollidingKeys(endpoint);
        _parentKeys.Clear();
        _typeNames = typeNames ?? new TypeNameRegistry();

        var (getter, generated) = Generate("entry", endpoint);
        var types = generated.ToList();

        // Endpoints are generated after the children they contain, but the entry endpoint is the most useful
        // thing to read first. Its getter carries the very identifier its generated class was given.
        int index = types.FindIndex(x => ReferenceEquals(x.Identifier, getter.Type));
        if (index > 0)
        {
            var entryType = types[index];
            types.RemoveAt(index);
            types.Insert(0, entryType);
        }

        return types;
    }

    /// <inheritdoc/>
    public (TsGetter getter, IEnumerable<ITsType> types) Generate(string key, IEndpoint endpoint)
        => builders.For(endpoint).Build(key, endpoint, this);

    /// <inheritdoc/>
    public TsIdentifier EndpointType(string key, IEndpoint endpoint)
        => _typeNames.Register(NameCandidates(key, endpoint));

    /// <summary>
    /// Returns increasingly qualified names for an endpoint: the bare key, then the key prefixed with its parent,
    /// its grandparent, and so on. Keys that are known to collide skip the bare name.
    /// </summary>
    private List<TsIdentifier> NameCandidates(string key, IEndpoint endpoint)
    {
        var candidates = new List<TsIdentifier>();
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

    /// <inheritdoc/>
    public void PushParent(string key) => _parentKeys.Push(key);

    /// <inheritdoc/>
    public void PopParent() => _parentKeys.Pop();
}
