using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Supplies the attributes that carry wire names on generated DTOs, for one specific JSON serializer.
/// </summary>
/// <remarks>
/// TypedRest for .NET serializes with Newtonsoft.Json out of the box; System.Text.Json is available through the
/// <c>TypedRest.SystemTextJson</c> package. The two read entirely different attributes, so a DTO annotated for one
/// silently falls back to its C# member names under the other.
/// </remarks>
public abstract class JsonAttributes
{
    /// <summary>
    /// Newtonsoft.Json, used by the <c>TypedRest</c> package by default.
    /// </summary>
    public const string Newtonsoft = "newtonsoft";

    /// <summary>
    /// System.Text.Json, used through the <c>TypedRest.SystemTextJson</c> package.
    /// </summary>
    public const string SystemTextJson = "system-text-json";

    /// <summary>
    /// The serializer names <see cref="For"/> accepts, most preferred first.
    /// </summary>
    public static IReadOnlyCollection<string> Names => [Newtonsoft, SystemTextJson];

    /// <summary>
    /// Returns the attributes for <paramref name="name"/>, or the default serializer if it is <c>null</c>.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="name"/> is not one of <see cref="Names"/>.</exception>
    public static JsonAttributes For(string? name)
        => name switch
        {
            null or Newtonsoft => new NewtonsoftAttributes(),
            SystemTextJson => new SystemTextJsonAttributes(),
            _ => throw new ArgumentException($"Unknown serializer '{name}'. Expected one of: {string.Join(", ", Names)}.", nameof(name))
        };

    /// <summary>
    /// The name of the serializer these attributes are for.
    /// </summary>
    public abstract string Serializer { get; }

    /// <summary>
    /// Carries the wire name of a property whose C# name differs from it.
    /// </summary>
    public abstract CSharpAttribute PropertyName(string name);

    /// <summary>
    /// Carries the wire name of an enum value whose C# name differs from it.
    /// </summary>
    public abstract CSharpAttribute EnumMemberName(string name);
}

/// <summary>
/// Annotates DTOs for Newtonsoft.Json.
/// </summary>
public sealed class NewtonsoftAttributes : JsonAttributes
{
    /// <inheritdoc/>
    public override string Serializer => Newtonsoft;

    /// <inheritdoc/>
    public override CSharpAttribute PropertyName(string name)
        => new(new CSharpIdentifier("Newtonsoft.Json", "JsonPropertyAttribute"))
        {
            Arguments = {name}
        };

    /// <inheritdoc/>
    /// <remarks>Newtonsoft's <c>StringEnumConverter</c> honours <c>[EnumMember]</c>.</remarks>
    public override CSharpAttribute EnumMemberName(string name)
        => new(new CSharpIdentifier("System.Runtime.Serialization", "EnumMemberAttribute"))
        {
            NamedArguments =
            {
                ("Value", name)
            }
        };
}

/// <summary>
/// Annotates DTOs for System.Text.Json.
/// </summary>
public sealed class SystemTextJsonAttributes : JsonAttributes
{
    /// <inheritdoc/>
    public override string Serializer => SystemTextJson;

    /// <inheritdoc/>
    public override CSharpAttribute PropertyName(string name)
        => new(new CSharpIdentifier("System.Text.Json.Serialization", "JsonPropertyNameAttribute"))
        {
            Arguments = {name}
        };

    /// <inheritdoc/>
    /// <remarks>
    /// System.Text.Json ignores <c>[EnumMember]</c>. <c>[JsonStringEnumMemberName]</c> is the attribute its
    /// <c>JsonStringEnumConverter</c> reads instead, and it requires .NET 9 or later.
    /// </remarks>
    public override CSharpAttribute EnumMemberName(string name)
        => new(new CSharpIdentifier("System.Text.Json.Serialization", "JsonStringEnumMemberNameAttribute"))
        {
            Arguments = {name}
        };
}
