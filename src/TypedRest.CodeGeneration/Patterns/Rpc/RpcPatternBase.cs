using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.Patterns.Rpc;

/// <summary>
/// Common base class for patterns that produce RPC endpoints.
/// </summary>
public abstract class RpcPatternBase : PatternBase
{
    protected override OperationType[] RequiredOperations
        => new[] {OperationType.Post};

    protected override IEndpoint? BuildEndpoint(OpenApiPathItem item)
    {
        var operation = item.Operations[OperationType.Post];

        var endpoint = BuildEndpoint(operation);
        if (endpoint != null)
            endpoint.Description = item.Description ?? operation.Description ?? operation.Summary ?? operation.RequestBody?.Description ?? operation.Get20XResponse()?.Description;

        return endpoint;
    }

    /// <summary>
    /// Builds the endpoint using information from the <paramref name="operation"/>. <c>null</c> if the pattern does not match.
    /// </summary>
    protected abstract IEndpoint? BuildEndpoint(OpenApiOperation operation);
}
