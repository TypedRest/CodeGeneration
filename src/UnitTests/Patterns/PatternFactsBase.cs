using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.Patterns;

public class PatternFactsBase<TPattern>
    where TPattern : IPattern, new()
{
    protected IEndpoint? TryGetEndpoint(PathTree tree, IDictionary<string, IEndpoint>? mockChildMatches = null)
    {
        var patternMatcherMock = new Mock<IPatternMatcher>();
        patternMatcherMock.Setup(x => x.GetEndpoints(tree))
                          .Returns(mockChildMatches ?? new Dictionary<string, IEndpoint>());

        return new TPattern().TryGetEndpoint(tree, patternMatcherMock.Object);
    }
}
