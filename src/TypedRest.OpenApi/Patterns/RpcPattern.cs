using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi.Patterns
{
    public abstract class RpcPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Post};
    }
}
