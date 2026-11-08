using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Raw;

namespace TypedRest.CodeGeneration.Patterns.Raw;

/// <summary>
/// A pattern that generates <see cref="UploadEndpoint"/>s.
/// </summary>
public class UploadPattern : PatternBase
{
    protected override HttpMethod[] RequiredOperations
        => [HttpMethod.Post];

    protected override IEndpoint? BuildEndpoint(IOpenApiPathItem item)
    {
        var operation = item.Operations![HttpMethod.Post];

        var request = operation.RequestBody;
        if (request == null) return null;

        return new UploadEndpoint
        {
            FormField = request.Content != null && request.Content.TryGetValue("multipart/form-data", out var mediaType)
                ? mediaType?.Schema?.Properties?.FirstOrDefault(x => x.Value.Format == "binary").Key
                : null,
            Description = item.Description ?? operation.Description ?? operation.Summary ?? request.Description ?? operation.RequestBody?.Description ?? operation.Get20XResponse()?.Description
        };
    }
}
