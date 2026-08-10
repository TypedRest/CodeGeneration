using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// The <see cref="GenerationMessage"/>s the TypeScript generator can report.
/// </summary>
/// <remarks>
/// The <c>TRCG1xx</c> range is reserved for target language backends, so these never collide with the
/// <c>TRCG0xx</c> diagnostics of the C# source generator.
/// </remarks>
public static class Messages
{
    /// <summary>
    /// TypedRest for TypeScript has no <c>PollingEndpoint</c>.
    /// </summary>
    public static GenerationMessage PollingNotSupported(string key, string entity)
        => Warning("TRCG101", key,
            $"Endpoint '{key}' uses kind 'polling'. TypedRest for TypeScript has no polling support; generating ElementEndpoint<{entity}> instead. Change notifications are not available.");

    /// <summary>
    /// TypedRest for TypeScript has no <c>StreamingEndpoint</c>.
    /// </summary>
    public static GenerationMessage StreamingNotSupported(string key)
        => Warning("TRCG102", key,
            $"Endpoint '{key}' uses kind 'streaming'. TypedRest for TypeScript has no streaming support; generating a plain Endpoint instead. The element type and separator are not preserved.");

    /// <summary>
    /// TypedRest for TypeScript has no <c>SseStreamingEndpoint</c>.
    /// </summary>
    public static GenerationMessage SseNotSupported(string key)
        => Warning("TRCG103", key,
            $"Endpoint '{key}' uses kind 'sse'. TypedRest for TypeScript has no server-sent event support; generating a plain Endpoint instead. The element type and event type are not preserved.");

    /// <summary>
    /// TypedRest for TypeScript has no <c>StreamingCollectionEndpoint</c>.
    /// </summary>
    public static GenerationMessage StreamingCollectionNotSupported(string key)
        => Warning("TRCG104", key,
            $"Endpoint '{key}' uses kind 'streaming-collection'. TypedRest for TypeScript has no streaming support; generating a plain collection endpoint instead.");

    /// <summary>
    /// The element of a collection describes a different type than the collection itself, which TypeScript
    /// cannot express because <c>TElementEndpoint</c> is constrained to <c>ElementEndpoint&lt;TEntity&gt;</c>.
    /// </summary>
    public static GenerationMessage ElementSchemaMismatch(string key, string collectionEntity, string elementEntity)
        => Warning("TRCG105", key,
            $"The element of collection '{key}' describes {elementEntity} while the collection describes {collectionEntity}. TypeScript constrains both to the same type; using {collectionEntity}.");

    /// <summary>
    /// TypeScript is structurally typed and TypedRest for TypeScript has no endpoint interfaces.
    /// </summary>
    public static GenerationMessage InterfacesNotSupported()
        => Warning("TRCG110", null,
            "Generating interfaces has no effect for TypeScript, which is structurally typed and whose TypedRest endpoints have no interfaces.");

    /// <summary>
    /// The C# language version does not apply to TypeScript.
    /// </summary>
    public static GenerationMessage LangVersionNotSupported()
        => Warning("TRCG111", null,
            "The C# language version has no effect for TypeScript.");

    /// <summary>
    /// TypedRest for TypeScript deserializes with <c>JSON.parse()</c> and a cast, so there is no serializer to pick.
    /// </summary>
    public static GenerationMessage SerializerNotSupported()
        => Warning("TRCG112", null,
            "Choosing a serializer has no effect for TypeScript, which deserializes with JSON.parse() and a cast. Generated DTO properties always keep the exact name used on the wire.");

    private static GenerationMessage Warning(string code, string? endpointKey, string text)
        => new(GenerationSeverity.Warning, code, text, endpointKey);
}
