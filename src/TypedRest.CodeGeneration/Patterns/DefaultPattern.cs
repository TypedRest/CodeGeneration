using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.Patterns;

/// <summary>
/// The default fallback pattern that is used if no other matches are found. Generates <see cref="Endpoint"/>s.
/// </summary>
public class DefaultPattern : IPattern
{
    public IEndpoint? TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
    {
        var children = patternMatcher.GetEndpoints(tree);
        if (children.Count == 0) return null; // Plain endpoint with no children serves no purpose

        var endpoint = new Endpoint();
        endpoint.Children.AddRange(children);
        return endpoint;
    }
}
