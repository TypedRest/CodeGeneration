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
            => new[] {OperationType.Get /*, OperationType.Put*/};

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
        {
            var operation = item.Operations[OperationType.Get];

            var schema = operation.GetResponseSchema();
            if (schema == null) return null;

            return new ElementEndpoint
            {
                Schema = schema,
                Description = operation.Description ?? operation.Summary
            };
        }
    }
}
