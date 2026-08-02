using Microsoft.CodeAnalysis;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// The diagnostics reported by <see cref="TypedRestSourceGenerator"/>.
/// </summary>
internal static class Diagnostics
{
    private const string Category = "TypedRest";
    private const string HelpLink = "https://code-generation.typedrest.net/";

    public static readonly DiagnosticDescriptor MissingServiceName = new(
        "TRCG001", "Missing ServiceName",
        "The TypedRestOpenApi item '{0}' has no ServiceName metadata and no $(TypedRestServiceName) fallback",
        Category, DiagnosticSeverity.Error, isEnabledByDefault: true, helpLinkUri: HelpLink);

    public static readonly DiagnosticDescriptor SpecError = new(
        "TRCG002", "OpenAPI specification error",
        "Error in OpenAPI specification '{0}': {1}",
        Category, DiagnosticSeverity.Error, isEnabledByDefault: true, helpLinkUri: HelpLink);

    public static readonly DiagnosticDescriptor SpecWarning = new(
        "TRCG003", "OpenAPI specification warning",
        "Warning in OpenAPI specification '{0}': {1}",
        Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, helpLinkUri: HelpLink);

    public static readonly DiagnosticDescriptor GenerationFailed = new(
        "TRCG004", "Code generation failed",
        "Failed to generate a TypedRest client from '{0}': {1}: {2}",
        Category, DiagnosticSeverity.Error, isEnabledByDefault: true, helpLinkUri: HelpLink);

    public static readonly DiagnosticDescriptor UnreadableSpec = new(
        "TRCG005", "Unreadable OpenAPI specification",
        "Could not read the content of '{0}'",
        Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, helpLinkUri: HelpLink);
}
