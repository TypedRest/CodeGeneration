using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.Patterns
{
    /// <summary>
    /// The default fallback pattern that is used if no other matches are found. Generates <see cref="Endpoint"/>s.
    /// </summary>
    public class DefaultPattern : PatternBase
    {
        protected override OperationType[] RequiredOperations
            => new OperationType[0];

        protected override IEndpoint? BuildEndpoint(OpenApiPathItem item)
            => new Endpoint();
    }
}
