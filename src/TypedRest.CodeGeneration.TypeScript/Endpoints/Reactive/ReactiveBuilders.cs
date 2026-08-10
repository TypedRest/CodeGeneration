using TypedRest.CodeGeneration.Endpoints.Reactive;
using TypedRest.CodeGeneration.TypeScript.Endpoints.Generic;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints.Reactive;

// TypedRest for TypeScript has no reactive endpoints at all. Rather than refusing to generate a client for a
// document that uses them, each kind degrades to its closest non-reactive equivalent and reports what was lost.

/// <summary>
/// Builds TypeScript code for <see cref="PollingEndpoint"/>s, degrading them to plain element endpoints.
/// </summary>
public class PollingBuilder : BuilderBase<PollingEndpoint>
{
    /// <inheritdoc/>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, PollingEndpoint endpoint, IEndpointGenerator generator)
    {
        var baseType = GetBaseType(endpoint, generator);
        generator.Log.Report(Messages.PollingNotSupported(key, baseType.TypeArguments[0].ToTypeExpression()));
        return (baseType, [], []);
    }

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(PollingEndpoint endpoint, IEndpointGenerator generator)
        => ElementBuilder.ElementEndpointType(endpoint.Schema, generator);
}

/// <summary>
/// Builds TypeScript code for <see cref="StreamingEndpoint"/>s, degrading them to plain endpoints.
/// </summary>
public class StreamingBuilder : BuilderBase<StreamingEndpoint>
{
    /// <inheritdoc/>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, StreamingEndpoint endpoint, IEndpointGenerator generator)
    {
        generator.Log.Report(Messages.StreamingNotSupported(key));
        return (GetBaseType(endpoint, generator), [], []);
    }

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(StreamingEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Endpoints, "Endpoint");
}

/// <summary>
/// Builds TypeScript code for <see cref="SseStreamingEndpoint"/>s, degrading them to plain endpoints.
/// </summary>
public class SseStreamingBuilder : BuilderBase<SseStreamingEndpoint>
{
    /// <inheritdoc/>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, SseStreamingEndpoint endpoint, IEndpointGenerator generator)
    {
        generator.Log.Report(Messages.SseNotSupported(key));
        return (GetBaseType(endpoint, generator), [], []);
    }

    /// <inheritdoc/>
    protected override TsIdentifier GetBaseType(SseStreamingEndpoint endpoint, IEndpointGenerator generator)
        => new(generator.Modules.Endpoints, "Endpoint");
}

/// <summary>
/// Builds TypeScript code for <see cref="StreamingCollectionEndpoint"/>s, degrading them to plain collections.
/// </summary>
public class StreamingCollectionBuilder : CollectionBuilderBase<StreamingCollectionEndpoint>
{
    /// <inheritdoc/>
    protected override (TsIdentifier baseType, IEnumerable<ITsType> types, IEnumerable<TsExpression> extraArguments) GetBase(string key, StreamingCollectionEndpoint endpoint, IEndpointGenerator generator)
    {
        generator.Log.Report(Messages.StreamingCollectionNotSupported(key));
        return base.GetBase(key, endpoint, generator);
    }

    /// <inheritdoc/>
    protected override string TypeName => "CollectionEndpoint";

    /// <inheritdoc/>
    protected override string GenericTypeName => "GenericCollectionEndpoint";
}
