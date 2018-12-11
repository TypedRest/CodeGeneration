using System.Linq;
using JetBrains.Annotations;
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
        public IEndpoint TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
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

        [CanBeNull]
        private static IEndpoint ExtractElementEndpoint(EndpointList childEndpoints)
        {
            var match = childEndpoints.FirstOrDefault(x => x.Key.StartsWith("{") && x.Key.EndsWith("}"));
            if (match.Value == null)
                return null;
            childEndpoints.Remove(match.Key);

            match.Value.Uri = null;
            return match.Value;
        }

        /// <summary>
        /// A list of <see cref="OperationType"/>s the root of the path tree must support in order for this pattern to be applicable.
        /// </summary>
        protected virtual OperationType[] RequiredOperations
            => new OperationType[0];

        /// <summary>
        /// Builds the endpoint using information from the <paramref name="item"/> and <paramref name="elementEndpoint"/>. <c>null</c> if the pattern does not match.
        /// </summary>
        [CanBeNull]
        protected virtual IndexerEndpoint BuildEndpoint(OpenApiPathItem item, IEndpoint elementEndpoint)
            => new IndexerEndpoint {Element = elementEndpoint};
    }
}
