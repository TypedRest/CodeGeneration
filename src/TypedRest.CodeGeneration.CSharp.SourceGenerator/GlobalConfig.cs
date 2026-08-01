using Microsoft.CodeAnalysis.Diagnostics;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Project-level configuration, used as a fallback for spec files that do not specify a value themselves.
/// </summary>
internal sealed record GlobalConfig(
    string? RootNamespace,
    string? ServiceName,
    string? Namespace,
    string? DtoNamespace,
    bool? GenerateInterfaces,
    bool? GenerateDtos,
    string? LangVersion)
{
    public static GlobalConfig From(AnalyzerConfigOptions options)
        => new(
            GetString(options, ConfigKeys.RootNamespace),
            GetString(options, ConfigKeys.ServiceName),
            GetString(options, ConfigKeys.Namespace),
            GetString(options, ConfigKeys.DtoNamespace),
            GetBool(options, ConfigKeys.GenerateInterfaces),
            GetBool(options, ConfigKeys.GenerateDtos),
            GetString(options, ConfigKeys.LangVersion));

    private static string? GetString(AnalyzerConfigOptions options, string name)
        => options.TryGetValue(ConfigKeys.Property(name), out string? value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;

    private static bool? GetBool(AnalyzerConfigOptions options, string name)
        => GetString(options, name) is {} value && bool.TryParse(value, out bool result)
            ? result
            : null;
}
