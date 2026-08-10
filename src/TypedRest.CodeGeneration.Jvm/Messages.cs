using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.Jvm;

/// <summary>
/// The <see cref="GenerationMessage"/>s the JVM generators can report.
/// </summary>
public static class Messages
{
    /// <summary>
    /// The element of a collection describes a different type than the collection itself, which cannot be expressed
    /// because <c>TElementEndpoint</c> is constrained to <c>ElementEndpoint&lt;TEntity&gt;</c>.
    /// </summary>
    public static GenerationMessage ElementSchemaMismatch(string key, string collectionEntity, string elementEntity)
        => Warning("TRCG120", key,
            $"The element of collection '{key}' describes {elementEntity} while the collection describes {collectionEntity}. TypedRest for the JVM constrains both to the same type; using {collectionEntity}.");

    /// <summary>
    /// TypedRest for the JVM has no endpoint interfaces to generate alongside the classes.
    /// </summary>
    public static GenerationMessage InterfacesNotSupported()
        => Warning("TRCG121", null,
            "Generating interfaces has no effect on the JVM. TypedRest for the JVM already ships an interface for every endpoint kind, and generated endpoints derive from the Impl classes behind them.");

    /// <summary>
    /// The C# language version does not apply to the JVM.
    /// </summary>
    public static GenerationMessage LangVersionNotSupported()
        => Warning("TRCG122", null,
            "The C# language version has no effect on the JVM.");

    /// <summary>
    /// kotlinx.serialization cannot serialize a class written in Java.
    /// </summary>
    public static GenerationMessage SerializerNotSupportedByJava(string serializer)
        => Warning("TRCG123", null,
            $"Serializer '{serializer}' generates its serializers with a Kotlin compiler plugin and cannot handle DTOs written in Java. Generate Kotlin, or pick a reflection-based serializer such as 'jackson' or 'moshi'.");

    private static GenerationMessage Warning(string code, string? endpointKey, string text)
        => new(GenerationSeverity.Warning, code, text, endpointKey);
}
