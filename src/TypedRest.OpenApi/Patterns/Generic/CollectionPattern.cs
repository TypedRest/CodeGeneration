using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.Patterns.Generic
{
    /// <summary>
    /// A pattern that generates <see cref="CollectionEndpoint"/>s.
    /// </summary>
    public class CollectionPattern : IndexerPattern
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Get, OperationType.Post};

        protected override IndexerEndpoint BuildEndpoint()
            => new CollectionEndpoint();

        protected override IEndpoint GetElementEndpoint(IEndpoint endpoint)
        {
            if (endpoint is ElementEndpoint && endpoint.Children.Count == 0)
                return null;
            else
                return base.GetElementEndpoint(endpoint);
        }
    }
}
