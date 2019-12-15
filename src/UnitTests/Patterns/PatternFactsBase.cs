using System.Collections.Generic;
using Moq;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
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
}
