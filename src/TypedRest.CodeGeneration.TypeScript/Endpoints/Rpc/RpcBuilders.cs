using TypedRest.CodeGeneration.Endpoints.Rpc;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints.Rpc;

/// <summary>
/// Builds TypeScript code for <see cref="ActionEndpoint"/>s.
/// </summary>
public class ActionBuilder : BuilderBase<ActionEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(ActionEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Rpc, "ActionEndpoint");
}

/// <summary>
/// Builds TypeScript code for <see cref="ProducerEndpoint"/>s.
/// </summary>
public class ProducerBuilder : BuilderBase<ProducerEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(ProducerEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Rpc, "ProducerEndpoint")
        {
            TypeArguments = {generator.Naming.TypeFor(endpoint.Schema)}
        };
}

/// <summary>
/// Builds TypeScript code for <see cref="ConsumerEndpoint"/>s.
/// </summary>
public class ConsumerBuilder : BuilderBase<ConsumerEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(ConsumerEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Rpc, "ConsumerEndpoint")
        {
            TypeArguments = {generator.Naming.TypeFor(endpoint.Schema)}
        };
}

/// <summary>
/// Builds TypeScript code for <see cref="FunctionEndpoint"/>s.
/// </summary>
public class FunctionBuilder : BuilderBase<FunctionEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(FunctionEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Rpc, "FunctionEndpoint")
        {
            TypeArguments =
            {
                generator.Naming.TypeFor(endpoint.RequestSchema),
                generator.Naming.TypeFor(endpoint.ResponseSchema)
            }
        };
}
