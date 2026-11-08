using System.Net;

namespace TypedRest.CodeGeneration;

public static class OpenApiOperationExtensions
{
    /// <summary>
    /// The HTTP <c>PATCH</c> method. <see cref="HttpMethod"/> has no static property for it on .NET Standard 2.0.
    /// </summary>
    public static readonly HttpMethod Patch = new("PATCH");

    /// <summary>
    /// Gets the operation for a specific HTTP <paramref name="method"/>, if any.
    /// </summary>
    public static OpenApiOperation? GetOperation(this IOpenApiPathItem item, HttpMethod method)
        => item.Operations != null && item.Operations.TryGetValue(method, out var operation) ? operation : null;

    /// <summary>
    /// Indicates whether the <paramref name="item"/> has an operation for a specific HTTP <paramref name="method"/>.
    /// </summary>
    public static bool HasOperation(this IOpenApiPathItem item, HttpMethod method)
        => item.Operations?.ContainsKey(method) ?? false;

    /// <summary>
    /// Gets the HTTP 200 response, if any.
    /// </summary>
    public static IOpenApiResponse? Get200Response(this OpenApiOperation operation)
        => operation.GetResponse(HttpStatusCode.OK);

    /// <summary>
    /// Gets the HTTP 200, 201, 202 or 204 response, if any.
    /// </summary>
    public static IOpenApiResponse? Get20XResponse(this OpenApiOperation operation)
        => operation.GetResponse(HttpStatusCode.OK)
        ?? operation.GetResponse(HttpStatusCode.Created)
        ?? operation.GetResponse(HttpStatusCode.Accepted)
        ?? operation.GetResponse(HttpStatusCode.NoContent);

    /// <summary>
    /// Gets the response for a specific HTTP <paramref name="statusCode"/>, if any.
    /// </summary>
    public static IOpenApiResponse? GetResponse(this OpenApiOperation operation, HttpStatusCode statusCode)
        => operation.Responses != null && operation.Responses.TryGetValue(((int)statusCode).ToString(), out var response) ? response : null;

    /// <summary>
    /// Gets the schema for the JSON media type, if any.
    /// </summary>
    public static IOpenApiSchema? GetJsonSchema(this IOpenApiRequestBody request)
        => request.Content.GetJsonSchema();

    /// <summary>
    /// Gets the schema for the JSON media type, if any.
    /// </summary>
    public static IOpenApiSchema? GetJsonSchema(this IOpenApiResponse response)
        => response.Content.GetJsonSchema();

    private static IOpenApiSchema? GetJsonSchema(this IDictionary<string, IOpenApiMediaType>? content)
        => content?.FirstOrDefault(x => x.Key.Contains("/json")).Value?.Schema;

    /// <summary>
    /// Indicates whether the response carries a JSON payload, even one without a schema.
    /// </summary>
    public static bool HasJsonContent(this IOpenApiResponse response)
        => response.Content?.Keys.Any(x => x.Contains("/json")) ?? false;

    /// <summary>
    /// Gets the JSON request body schema of an operation on the <paramref name="item"/>, if the operation is present and takes one.
    /// </summary>
    public static IOpenApiSchema? GetJsonRequestSchema(this IOpenApiPathItem item, HttpMethod method)
        => item.GetOperation(method)?.RequestBody?.GetJsonSchema();

    /// <summary>
    /// Indicates whether two schemas describe the same type, i.e. whether they map to the same generated DTO.
    /// </summary>
    /// <remarks>
    /// Schemas pointing at a component match exactly when they point at the same one. Inline schemas match when their
    /// <c>type</c> and <c>format</c> agree, because that is the granularity at which they are mapped to a target type.
    /// </remarks>
    public static bool DescribesSameTypeAs(this IOpenApiSchema? schema, IOpenApiSchema? other)
    {
        if (ReferenceEquals(schema, other)) return true;
        if (schema == null || other == null) return false;

        string? schemaId = schema.GetReferenceId(), otherId = other.GetReferenceId();
        if (schemaId != null || otherId != null)
            return schemaId != null && schemaId == otherId;

        return schema.Type == other.Type
            && schema.Format == other.Format
            && (!schema.Is(JsonSchemaType.Array) || schema.Items.DescribesSameTypeAs(other.Items));
    }
}
