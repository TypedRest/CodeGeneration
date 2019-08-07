using System.Linq;
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

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
        {
            var operation = item.Operations[OperationType.Get];

            var mediaType = operation.Responses
                                     .FirstOrDefault(x => x.Key.StartsWith("2")).Value
                                    ?.Content.FirstOrDefault(x => x.Key.Contains("json")).Value;
            if (mediaType == null) return null;

            return new ElementEndpoint
            {
                Description = operation.Description ?? operation.Summary,
                Schema = mediaType.Schema
            };
        }
    }
}
