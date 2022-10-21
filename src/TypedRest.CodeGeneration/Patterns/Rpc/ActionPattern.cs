using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.Patterns.Rpc;

/// <summary>
/// A pattern that generates <see cref="ActionEndpoint"/>s.
/// </summary>
public class ActionPattern : RpcPatternBase
{
    protected override IEndpoint? BuildEndpoint(OpenApiOperation operation)
    {
        if (operation.RequestBody != null) return null;

        return new ActionEndpoint();
    }
}
