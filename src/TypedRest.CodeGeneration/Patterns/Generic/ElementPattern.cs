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

        // ElementEndpoint reads and writes a single type, so an update taking anything else would be mistyped.
        if (!Updates(item, OperationType.Put, schema) || !Updates(item, OperationType.Patch, schema))
            return null;

        return new ElementEndpoint
        {
            Schema = schema,
            Description = item.Description ?? operation.Description ?? operation.Summary ?? response?.Description ?? schema.Description
        };
    }

    /// <summary>
    /// Checks that an update operation, if present, takes the same type that is being read.
    /// Operations without a JSON request body impose no constraint.
    /// </summary>
    private static bool Updates(OpenApiPathItem item, OperationType type, OpenApiSchema schema)
        => item.GetJsonRequestSchema(type) is not {} requestSchema || requestSchema.DescribesSameTypeAs(schema);
}
