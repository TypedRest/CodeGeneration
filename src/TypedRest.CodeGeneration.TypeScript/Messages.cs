using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// The <see cref="GenerationMessage"/>s the TypeScript generator can report.
/// </summary>
/// <remarks>
/// The <c>TRCG1xx</c> range is reserved for target language backends, so these never collide with the
/// <c>TRCG0xx</c> diagnostics of the C# source generator.
/// <c>TRCG101</c> to <c>TRCG104</c> reported reactive endpoints degrading to non-reactive ones, back when
/// TypedRest for TypeScript had none. They are retired rather than reused, so that existing suppressions keep
/// meaning what they did.
/// </remarks>
public static class Messages
{
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
