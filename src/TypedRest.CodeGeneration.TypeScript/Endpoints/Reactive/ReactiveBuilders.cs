using TypedRest.CodeGeneration.Endpoints.Reactive;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints.Reactive;

/// <summary>
/// Builds TypeScript code for <see cref="PollingEndpoint"/>s.
/// </summary>
/// <remarks>The optional <c>endCondition</c> is a function, so there is nothing in the document to pass for it.</remarks>
public class PollingBuilder : BuilderBase<PollingEndpoint>
{
    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(PollingEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Reactive, "PollingEndpoint")
        {
            TypeArguments = {generator.Naming.TypeFor(endpoint.Schema)}
        };
}

/// <summary>
/// Builds TypeScript code for <see cref="StreamingEndpoint"/>s.
/// </summary>
public class StreamingBuilder : BuilderBase<StreamingEndpoint>
{
    /// <inheritdoc/>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, StreamingEndpoint endpoint, IEndpointGenerator generator)
        => (GetBaseType(endpoint, generator),
            [],
            endpoint.Separator is {Length: > 0} separator ? [new TsLiteral(separator)] : []);

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(StreamingEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Reactive, "StreamingEndpoint")
        {
            TypeArguments = {generator.Naming.TypeFor(endpoint.Schema)}
        };
}

/// <summary>
/// Builds TypeScript code for <see cref="SseStreamingEndpoint"/>s.
/// </summary>
public class SseStreamingBuilder : BuilderBase<SseStreamingEndpoint>
{
    /// <inheritdoc/>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, SseStreamingEndpoint endpoint, IEndpointGenerator generator)
        => (GetBaseType(endpoint, generator),
            [],
            endpoint.EventType is {Length: > 0} eventType ? [new TsLiteral(eventType)] : []);

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(SseStreamingEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Reactive, "SseStreamingEndpoint")
        {
            TypeArguments = {generator.Naming.TypeFor(endpoint.Schema)}
        };
}

/// <summary>
/// Builds TypeScript code for <see cref="StreamingCollectionEndpoint"/>s.
/// </summary>
public class StreamingCollectionBuilder : CollectionBuilderBase<StreamingCollectionEndpoint>
{
    /// <inheritdoc/>
    protected override TsModule TypeModule(IEndpointGenerator generator) => generator.Modules.Reactive;

    /// <inheritdoc/>
    protected override string TypeName => "StreamingCollectionEndpoint";

    /// <inheritdoc/>
    protected override string GenericTypeName => "GenericStreamingCollectionEndpoint";
}
