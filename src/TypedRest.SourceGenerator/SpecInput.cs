using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// An OpenAPI spec file to generate a client from, along with its per-file configuration.
/// </summary>
/// <remarks>
/// Holds only equatable data. Neither <see cref="AdditionalText"/> nor <c>SourceText</c> provide value equality,
/// so both must be projected away before entering the incremental pipeline.
/// </remarks>
internal sealed record SpecInput(
    string Path,
    string? Content,
    string? ServiceName,
    string? Namespace,
    string? DtoNamespace,
    bool? GenerateInterfaces,
    bool? GenerateDtos,
    bool? GenerateEntryConstructor)
{
    /// <summary>
    /// Reads a spec file and its metadata. Returns <c>null</c> if the file is not marked as a TypedRest spec.
    /// </summary>
    public static SpecInput? From(AdditionalText text, AnalyzerConfigOptions options, CancellationToken cancellationToken)
    {
        // Check the marker before reading the content, so unrelated AdditionalFiles are never touched
        if (GetBool(options, ConfigKeys.Marker) != true) return null;

        return new SpecInput(
            text.Path,
            text.GetText(cancellationToken)?.ToString(),
            GetString(options, ConfigKeys.ServiceName),
            GetString(options, ConfigKeys.Namespace),
            GetString(options, ConfigKeys.DtoNamespace),
            GetBool(options, ConfigKeys.GenerateInterfaces),
            GetBool(options, ConfigKeys.GenerateDtos),
            GetBool(options, ConfigKeys.GenerateEntryConstructor));
    }

    private static string? GetString(AnalyzerConfigOptions options, string name)
        => options.TryGetValue(ConfigKeys.Metadata(name), out string? value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;

    private static bool? GetBool(AnalyzerConfigOptions options, string name)
        => GetString(options, name) is {} value && bool.TryParse(value, out bool result)
            ? result
            : null;
}
