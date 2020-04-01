using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic
{
    /// <summary>
    /// A pattern that generates <see cref="IndexerEndpoint"/>s.
    /// </summary>
    public class IndexerPattern : IPattern
    {
        public virtual IEndpoint? TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
        {
            OpenApiOperation? operation = null;
            tree.Item?.Operations.TryGetValue(OperationType.Get, out operation);

            var children = patternMatcher.GetEndpoints(tree);
            var element = ExtractElement<IEndpoint>(children);
            if (element == null) return null;

            var endpoint = new IndexerEndpoint
            {
                Element = element,
                Description = operation?.Description ?? operation?.Summary
            };
            endpoint.Children.AddRange(children);
            return endpoint;
        }

        /// <summary>
        /// Extracts an element endpoint from a set of child endpoints.
        /// </summary>
        /// <param name="children">The child endpoints to search for. A potential match will be removed from this dictionary.</param>
        /// <typeparam name="T">The type of endpoint to search for.</typeparam>
        /// <returns>The element endpoint; <c>null</c> if none was found.</returns>
        protected static T? ExtractElement<T>(IDictionary<string, IEndpoint> children)
            where T : class, IEndpoint
        {
            (string key, var element) = children.FirstOrDefault(x => x.Key == "{}" && x.Value is T);
            if (element != null)
            {
                children.Remove(key);
                element.Uri = null;
            }

            return (T?)element;
        }
    }
}
