using System.Collections.Generic;
using Moq;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class PatternFactsBase<TPattern>
        where TPattern : IPattern, new()
    {
        protected IEndpoint TryGetEndpoint(PathTree tree, IDictionary<string, IEndpoint> childMatches = null)
        {
            var patternMatcherMock = new Mock<IPatternMatcher>();
            patternMatcherMock.Setup(x => x.GetEndpoints(tree.Children))
                              .Returns(childMatches ?? new Dictionary<string, IEndpoint>());

            return new TPattern().TryGetEndpoint(tree, patternMatcherMock.Object);
        }
    }
}
