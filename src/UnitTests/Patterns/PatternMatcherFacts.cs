using FluentAssertions;
using Xunit;

namespace TypedRest.OpenApi.Patterns
{
    public class PatternMatcherFacts
    {
        [Fact]
        public void MatchesPatterns()
        {
            var tree = PathTree.From(Sample.Paths);

            var endpoints = new PatternMatcher(PatternRegistry.Default).GetEndpoints(tree);

            endpoints.Should().BeEquivalentTo(Sample.Doc.GetTypedRestEndpoints(), options => options.IncludingAllRuntimeProperties());
        }
    }
}
