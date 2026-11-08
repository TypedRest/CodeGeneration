using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.Patterns.Rpc;

/// <summary>
/// Common base class for patterns that produce RPC endpoints.
/// </summary>
public abstract class RpcPatternBase : PatternBase
{
    protected override HttpMethod[] RequiredOperations
        => [HttpMethod.Post];

    protected override IEndpoint? BuildEndpoint(IOpenApiPathItem item)
    {
        var operation = item.Operations![HttpMethod.Post];

        if (BuildEndpoint(operation) is {} endpoint)
        {
            endpoint.Description = item.Description ?? operation.Description ?? operation.Summary ?? operation.RequestBody?.Description ?? operation.Get20XResponse()?.Description;
            return endpoint;
        }
        else return null;
    }

    /// <summary>
    /// Builds the endpoint using information from the <paramref name="operation"/>. <c>null</c> if the pattern does not match.
    /// </summary>
    protected abstract IEndpoint? BuildEndpoint(OpenApiOperation operation);
}
