using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Patterns.Rpc
{
    /// <summary>
    /// Common base class for patterns that produce RPC endpoints.
    /// </summary>
    public abstract class RpcPatternBase : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Post};
    }
}
