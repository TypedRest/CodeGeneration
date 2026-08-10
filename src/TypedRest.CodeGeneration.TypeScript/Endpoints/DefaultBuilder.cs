using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints;

/// <summary>
/// Builds TypeScript code for plain <see cref="Endpoint"/>s that only hold children.
/// </summary>
public class DefaultBuilder : BuilderBase<Endpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(Endpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Endpoints, "Endpoint");
}
