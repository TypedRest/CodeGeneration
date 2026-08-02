using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Raw;

namespace TypedRest.CodeGeneration.Patterns.Raw;

/// <summary>
/// A pattern that generates <see cref="BlobEndpoint"/>s.
/// </summary>
public class BlobPattern : PatternBase
{
    protected override OperationType[] RequiredOperations
        => [OperationType.Get /*, OperationType.Put*/];

    protected override IEndpoint? BuildEndpoint(OpenApiPathItem item)
    {
        var operation = item.Operations[OperationType.Get];

        var response = operation.Get200Response();
        if (response == null) return null;

        // A blob is a binary payload. Anything served as JSON, or with no body at all, belongs to another pattern.
        if (response.Content.Count == 0 || response.HasJsonContent()) return null;

        return new BlobEndpoint
        {
            Description = item.Description ?? operation.Description ?? operation.Summary ?? response.Description
        };
    }
}
