using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.Patterns.Generic
{
    /// <summary>
    /// A pattern that generates <see cref="IndexerEndpoint"/>s.
    /// </summary>
    public class IndexerPattern : IPattern
    {
        public IEndpoint? TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
        {
            var operations = tree.Item?.Operations.Keys ?? new OperationType[0];
            if (!RequiredOperations.All(operations.Contains))
                return null;

            var childEndpoints = patternMatcher.GetEndpoints(tree);
            var elementEndpoint = ExtractElementEndpoint(childEndpoints);
            if (elementEndpoint == null) return null;

            var endpoint = BuildEndpoint(tree.Item, elementEndpoint);
            endpoint?.Children.AddRange(childEndpoints);

            return endpoint;
        }

        private static IEndpoint? ExtractElementEndpoint(IDictionary<string, IEndpoint> childEndpoints)
        {
            (string key, var endpoint) = childEndpoints.FirstOrDefault(x => x.Key.StartsWith("{") && x.Key.EndsWith("}"));
            if (endpoint == null)
                return null;
            childEndpoints.Remove(key);

            endpoint.Uri = null;
            return endpoint;
        }

        /// <summary>
        /// A list of <see cref="OperationType"/>s the root of the path tree must support in order for this pattern to be applicable.
        /// </summary>
        protected virtual OperationType[] RequiredOperations
            => new OperationType[0];

        /// <summary>
        /// Builds the endpoint using information from the <paramref name="item"/> and <paramref name="elementEndpoint"/>. <c>null</c> if the pattern does not match.
        /// </summary>
        protected virtual IndexerEndpoint? BuildEndpoint(OpenApiPathItem? item, IEndpoint elementEndpoint)
            => new IndexerEndpoint {Element = elementEndpoint};
    }
}
