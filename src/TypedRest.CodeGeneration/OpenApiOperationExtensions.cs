using System.Net;

namespace TypedRest.CodeGeneration;

public static class OpenApiOperationExtensions
{
    /// <summary>
    /// Gets the HTTP 200 response, if any.
    /// </summary>
    public static OpenApiResponse? Get200Response(this OpenApiOperation operation)
        => operation.GetResponse(HttpStatusCode.OK);

    /// <summary>
    /// Gets the HTTP 200, 201, 202 or 204 response, if any.
    /// </summary>
    public static OpenApiResponse? Get20XResponse(this OpenApiOperation operation)
        => operation.GetResponse(HttpStatusCode.OK)
        ?? operation.GetResponse(HttpStatusCode.Created)
        ?? operation.GetResponse(HttpStatusCode.Accepted)
        ?? operation.GetResponse(HttpStatusCode.NoContent);

    /// <summary>
    /// Gets the response for a specific HTTP <paramref name="statusCode"/>, if any.
    /// </summary>
    public static OpenApiResponse? GetResponse(this OpenApiOperation operation, HttpStatusCode statusCode)
        => operation.Responses.TryGetValue(((int)statusCode).ToString(), out var response) ? response : null;

    /// <summary>
    /// Gets the schema for the JSON media type, if any.
    /// </summary>
    public static OpenApiSchema? GetJsonSchema(this OpenApiRequestBody request)
        => request.Content.GetJsonSchema();

    /// <summary>
    /// Gets the schema for the JSON media type, if any.
    /// </summary>
    public static OpenApiSchema? GetJsonSchema(this OpenApiResponse response)
        => response.Content.GetJsonSchema();

    private static OpenApiSchema? GetJsonSchema(this IDictionary<string, OpenApiMediaType> content)
        // ReSharper disable once ConstantConditionalAccessQualifier
        => content.FirstOrDefault(x => x.Key.Contains("/json")).Value?.Schema;

    /// <summary>
    /// Indicates whether the response carries a JSON payload, even one without a schema.
    /// </summary>
    public static bool HasJsonContent(this OpenApiResponse response)
        => response.Content.Keys.Any(x => x.Contains("/json"));

    /// <summary>
    /// Gets the JSON request body schema of an operation on the <paramref name="item"/>, if the operation is present and takes one.
    /// </summary>
    public static OpenApiSchema? GetJsonRequestSchema(this OpenApiPathItem item, OperationType type)
        => item.Operations.TryGetValue(type, out var operation)
            ? operation.RequestBody?.GetJsonSchema()
            : null;

    /// <summary>
    /// Indicates whether two schemas describe the same type, i.e. whether they map to the same generated DTO.
    /// </summary>
    /// <remarks>
    /// Schemas pointing at a component match exactly when they point at the same one. Inline schemas match when their
    /// <c>type</c> and <c>format</c> agree, because that is the granularity at which they are mapped to a target type.
    /// </remarks>
    public static bool DescribesSameTypeAs(this OpenApiSchema? schema, OpenApiSchema? other)
    {
        if (ReferenceEquals(schema, other)) return true;
        if (schema == null || other == null) return false;

        if (schema.Reference != null || other.Reference != null)
            return schema.Reference?.Id != null && schema.Reference.Id == other.Reference?.Id;

        return schema.Type == other.Type
            && schema.Format == other.Format
            && (schema.Type != "array" || schema.Items.DescribesSameTypeAs(other.Items));
    }
}
