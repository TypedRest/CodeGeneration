using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Endpoints;

/// <summary>
/// Helpers for walking a tree of <see cref="IEndpoint"/>s the way code generators do.
/// </summary>
public static class EndpointTree
{
    /// <summary>
    /// The key used for the element endpoint of a collection or indexer with the key <paramref name="key"/>.
    /// </summary>
    public static string ElementKey(string key)
        => key.Depluralize() + "_Element";

    /// <summary>
    /// Returns the element endpoint of a collection or indexer, or <c>null</c> for any other kind of endpoint.
    /// </summary>
    public static IEndpoint? GetElement(IEndpoint endpoint)
        => endpoint switch
        {
            CollectionEndpoint collection => collection.Element,
            IndexerEndpoint indexer => indexer.Element,
            _ => null
        };

    /// <summary>
    /// Walks the endpoint tree the way a generator's build pass will and returns the set of keys whose endpoints
    /// produce a custom type (i.e. have children) in more than one place.
    /// </summary>
    public static HashSet<string> FindCollidingKeys(IEndpoint root)
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

            var element = GetElement(endpoint);
            if (element != null)
                Walk(ElementKey(key), element);
        }
    }
}
