using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.Patterns.Generic;

/// <summary>
/// A pattern that generates <see cref="ElementEndpoint"/>s.
/// </summary>
public class ElementPattern : PatternBase
{
    protected override HttpMethod[] RequiredOperations
        => [HttpMethod.Get /*, HttpMethod.Put*/];

    protected override IEndpoint? BuildEndpoint(IOpenApiPathItem item)
    {
        var operation = item.Operations![HttpMethod.Get];

        var response = operation.Get200Response();
        var schema = response?.GetJsonSchema();
        if (schema == null) return null;

        // ElementEndpoint reads and writes a single type, so an update taking anything else would be mistyped.
        if (!Updates(item, HttpMethod.Put, schema) || !Updates(item, OpenApiOperationExtensions.Patch, schema))
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
    private static bool Updates(IOpenApiPathItem item, HttpMethod method, IOpenApiSchema schema)
        => item.GetJsonRequestSchema(method) is not {} requestSchema || requestSchema.DescribesSameTypeAs(schema);
}
