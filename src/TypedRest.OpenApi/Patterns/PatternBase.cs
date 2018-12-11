using System.Linq;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// Common base class for <see cref="IPattern"/>s.
    /// </summary>
    [PublicAPI]
    public abstract class PatternBase : IPattern
    {
        public IEndpoint TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
        {
            var operations = tree.Item?.Operations.Keys ?? new OperationType[0];
            if (!RequiredOperations.All(operations.Contains))
                return null;

            var endpoint = BuildEndpoint(tree.Item);
            endpoint.Children.AddRange(patternMatcher.GetEndpoints(tree.Children));
            return endpoint;
        }

        /// <summary>
        /// A list of <see cref="OperationType"/>s the root of the path tree must support in order for this pattern to be applicable.
        /// </summary>
        protected abstract OperationType[] RequiredOperations { get; }

        /// <summary>
        /// Builds the endpoint using information from the <paramref name="item"/>.
        /// </summary>
        protected abstract IEndpoint BuildEndpoint(OpenApiPathItem item);
    }
}
