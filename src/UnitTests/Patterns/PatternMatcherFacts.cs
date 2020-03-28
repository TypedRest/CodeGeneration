using FluentAssertions;
using Xunit;

namespace TypedRest.OpenApi.Patterns
{
    public class PatternMatcherFacts
    {
        [Fact]
        public void MatchesPatterns()
        {
            var entryEndpoint = Sample.Doc.MatchTypedRestPatterns();

            entryEndpoint.Should().BeEquivalentTo(Sample.Doc.GetTypedRest(), options => options.IncludingAllRuntimeProperties());
        }
    }
}
