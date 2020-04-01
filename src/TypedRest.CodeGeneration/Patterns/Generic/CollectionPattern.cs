using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic
{
    /// <summary>
    /// A pattern that generates <see cref="CollectionEndpoint"/>s.
    /// </summary>
    public class CollectionPattern : IndexerPattern
    {
        public override IEndpoint? TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
        {
            if (tree.Item == null || !tree.Item.Operations.TryGetValue(OperationType.Get, out var operation))
                return null;

            var children = patternMatcher.GetEndpoints(tree);
            var element = ExtractElement<ElementEndpoint>(children);
            if (element == null) return null;

            // Ensure collection and element schemas match
            var schema = operation.Get200Response()?.GetJsonSchema();
            if (schema?.Type != "array" || schema.Items?.Reference?.Id != element.Schema?.Reference?.Id) return null;
            element.Schema = null;

            var endpoint = new CollectionEndpoint
            {
                Schema = schema.Items,
                Element = (element.Children.Count == 0) ? null : element, // Trim trivial element endpoint
                Description = operation.Description ?? operation.Summary
            };
            endpoint.Children.AddRange(children);
            return endpoint;
        }
    }
}
