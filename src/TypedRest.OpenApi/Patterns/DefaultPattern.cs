using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class DefaultPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new OperationType[0];

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new Endpoint();
    }
}
