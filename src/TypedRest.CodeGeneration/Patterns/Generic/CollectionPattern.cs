using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic;

/// <summary>
/// A pattern that generates <see cref="CollectionEndpoint"/>s.
/// </summary>
public class CollectionPattern : IndexerPattern
{
    public override IEndpoint? TryGetEndpoint(PathTree tree, IPatternMatcher patternMatcher)
    {
        var item = tree.Item;
        if (item == null || !item.Operations.TryGetValue(OperationType.Get, out var operation))
            return null;

        var children = patternMatcher.GetEndpoints(tree);
        var element = ExtractElement<ElementEndpoint>(children);
        if (element == null) return null;

        var response = operation.Get200Response();
        var schema = response?.GetJsonSchema();

        // Ensure collection and element schemas match
        if (schema?.Type != "array" || schema.Items?.Reference?.Id != element.Schema?.Reference?.Id) return null;
        element.Schema = null;

        var endpoint = new CollectionEndpoint
        {
            Schema = schema.Items,
            Element = (element.Children.Count == 0) ? null : element, // Trim trivial element endpoint
            Description = item.Description ?? operation.Description ?? operation.Summary ?? response?.Description ?? schema.Description
        };
        endpoint.Children.AddRange(children);
        return endpoint;
    }
}
