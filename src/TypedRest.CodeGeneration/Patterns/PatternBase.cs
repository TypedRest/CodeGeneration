using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.Patterns;

/// <summary>
/// Common base class for <see cref="IPattern"/>s.
/// </summary>
public abstract class PatternBase : IPattern
{
    public IEndpoint? TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
    {
        var item = tree.Item;
        if (item == null || !RequiredOperations.All(item.HasOperation))
            return null;

        var endpoint = BuildEndpoint(item);
        endpoint?.Children.AddRange(patternMatcher.GetEndpoints(tree));
        return endpoint;
    }

    /// <summary>
    /// A list of <see cref="HttpMethod"/>s the root of the path tree must support in order for this pattern to be applicable.
    /// </summary>
    protected abstract HttpMethod[] RequiredOperations { get; }

    /// <summary>
    /// Builds the endpoint using information from the <paramref name="item"/>. <c>null</c> if the pattern does not match.
    /// </summary>
    protected abstract IEndpoint? BuildEndpoint(IOpenApiPathItem item);
}
