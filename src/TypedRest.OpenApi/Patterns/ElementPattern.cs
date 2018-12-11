using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    public class ElementPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new[] {OperationType.Get};

        protected override IEndpoint BuildEndpoint(OpenApiPathItem item)
            => new ElementEndpoint();
    }
}
