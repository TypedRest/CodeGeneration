
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// Extension methods for <see cref="IPatternMatcher"/>
    /// </summary>
    public static class PatternMatcherExtensions
    {
        /// <summary>
        /// Generates an <see cref="EntryEndpoint"/> with children for the specified <paramref name="document"/>.
        /// </summary>
        public static EntryEndpoint GetEntryEndpoint(this IPatternMatcher patternMatcher, OpenApiDocument document)
        {
            var endpoints = patternMatcher.GetEndpoints(PathTree.From(document.Paths));

            var entryEndpoint = new EntryEndpoint();
            entryEndpoint.Children.AddRange(endpoints);

            return entryEndpoint;
        }
    }
}
