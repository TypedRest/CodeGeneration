using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.Patterns.Generic
{
    /// <summary>
    /// A pattern that generates <see cref="ElementEndpoint"/>s.
    /// </summary>
    public class ElementPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Get};

        // TODO: doc.Paths.First().Value.Operations[OperationType.Post].Responses.First().Value.Content.First().Value.Schema.Reference.Id;

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new ElementEndpoint();
    }
}
