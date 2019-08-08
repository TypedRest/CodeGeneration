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
        public IEndpoint TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
        {
            var operations = tree.Item?.Operations.Keys ?? new OperationType[0];
            if (!RequiredOperations.All(operations.Contains))
                return null;

            string elementKey = tree.Children.Keys.FirstOrDefault(x => x.StartsWith("{") && x.EndsWith("}"));
            if (elementKey == null)
                return null;

            var endpoint = BuildEndpoint(tree.Item);

            var childEndpoints = patternMatcher.GetEndpoints(tree);
            foreach (var childEndpoint in childEndpoints)
            {
                if (childEndpoint.Key == elementKey)
                    endpoint.Element = GetElementEndpoint(childEndpoint.Value);
                else
                    endpoint.Children[childEndpoint.Key] = childEndpoint.Value;
            }

            return endpoint;
        }

        protected virtual OperationType[] RequiredOperations
            => new OperationType[0];

        protected virtual IndexerEndpoint BuildEndpoint(OpenApiPathItem item)
            => new IndexerEndpoint();

        protected virtual IEndpoint GetElementEndpoint(IEndpoint endpoint)
        {
            endpoint.Uri = null;
            return endpoint;
        }
    }
}
