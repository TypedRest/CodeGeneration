namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// The MSBuild property and item metadata names the generator reads.
/// </summary>
/// <remarks>
/// These must be kept in sync with <c>build/TypedRest.CodeGeneration.CSharp.SourceGenerator.props</c>,
/// which is what makes them visible to the compiler.
/// </remarks>
internal static class ConfigKeys
{
    private const string PropertyPrefix = "build_property.";
    private const string MetadataPrefix = "build_metadata.TypedRestOpenApi.";

    /// <summary>Set on every <c>TypedRestOpenApi</c> item, to tell them apart from other <c>AdditionalFiles</c>.</summary>
    public const string Marker = "IsTypedRestSpec";

    public const string ServiceName = "ServiceName";
    public const string Namespace = "Namespace";
    public const string DtoNamespace = "DtoNamespace";
    public const string GenerateInterfaces = "GenerateInterfaces";
    public const string GenerateDtos = "GenerateDtos";
    public const string GenerateEntryConstructor = "GenerateEntryConstructor";

    /// <summary>The MSBuild property the endpoint namespace falls back to.</summary>
    public const string RootNamespace = "RootNamespace";

    /// <summary>Builds the key for reading the MSBuild property fallback for <paramref name="name"/>.</summary>
    public static string Property(string name) => PropertyPrefix + (name == RootNamespace ? name : "TypedRest" + name);

    /// <summary>Builds the key for reading <paramref name="name"/> as <c>TypedRestOpenApi</c> item metadata.</summary>
    public static string Metadata(string name) => MetadataPrefix + name;
}
