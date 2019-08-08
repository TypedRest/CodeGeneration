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
            => new[] {OperationType.Get /*, OperationType.Post*/};

        protected override IndexerEndpoint BuildEndpoint(OpenApiPathItem item, IEndpoint elementEndpoint)
        {
            if (!(elementEndpoint is ElementEndpoint element)) return null;
            var operation = item.Operations[OperationType.Get];

            var schema = operation.GetResponseSchema();
            if (schema?.Type != "array" || schema.Items?.Reference?.Id != element?.Schema?.Reference?.Id) return null;

            element.Schema = null;
            if (element.Children.Count == 0) element = null;

            return new CollectionEndpoint
            {
                Schema = schema.Items,
                Element = element,
                Description = operation.Description ?? operation.Summary
            };
        }
    }
}
