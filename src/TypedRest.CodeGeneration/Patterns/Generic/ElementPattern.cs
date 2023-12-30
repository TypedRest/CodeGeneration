using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic;

/// <summary>
/// A pattern that generates <see cref="ElementEndpoint"/>s.
/// </summary>
public class ElementPattern : PatternBase
{
    protected override OperationType[] RequiredOperations
        => [OperationType.Get /*, OperationType.Put*/];

    protected override IEndpoint? BuildEndpoint(OpenApiPathItem item)
    {
        var operation = item.Operations[OperationType.Get];

        var response = operation.Get200Response();
        var schema = response?.GetJsonSchema();
        if (schema == null) return null;

        return new ElementEndpoint
        {
            Schema = schema,
            Description = item.Description ?? operation.Description ?? operation.Summary ?? response?.Description ?? schema.Description
        };
    }
}
