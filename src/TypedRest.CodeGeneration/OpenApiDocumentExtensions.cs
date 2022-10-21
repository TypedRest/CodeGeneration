using Microsoft.OpenApi.Models;
using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Patterns;

namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="OpenApiDocument"/>s.
/// </summary>
public static class OpenApiDocumentExtensions
{
    /// <summary>
    /// The property name used to add TypedRest as an extension to an <see cref="OpenApiDocument"/>.
    /// </summary>
    public const string TypedRestKey = "x-typedrest";

    /// <summary>
    /// Gets the TypedRest extension from the OpenAPI Spec <paramref name="document"/>, if present.
    /// </summary>
    /// <param name="document">The document to get TypedRest data from.</param>
    /// <param name="resolveReferences">Automatically runs <see cref="IEndpoint.ResolveReferences"/> on the endpoints before returning them.</param>
    /// <seealso cref="OpenApiReaderSettingsExtensions.AddTypedRest"/>
    public static EntryEndpoint? GetTypedRest(this OpenApiDocument document, bool resolveReferences = true)
    {
        document.Extensions.TryGetValue(TypedRestKey, out var output);
        if (output is EntryEndpoint endpoint)
        {
            if (resolveReferences)
                endpoint.ResolveReferences(document.Components);
            return endpoint;
        }
        else return null;
    }

    /// <summary>
    /// Adds the <paramref name="endpoint"/> as a TypedRest extension to the OpenAPI Spec <paramref name="document"/>.
    /// </summary>
    public static OpenApiDocument SetTypedRest(this OpenApiDocument document, EntryEndpoint endpoint)
    {
        document.Extensions[TypedRestKey] = endpoint;
        return document;
    }

    /// <summary>
    /// Generates an <see cref="EntryEndpoint"/> with children by matching patterns in the OpenAPI Spec document.
    /// </summary>
    /// <param name="document">The document to check for patterns.</param>
    /// <param name="patterns">An ordered list of all known <see cref="IPattern"/>s; leave <c>null</c> for default.</param>
    public static EntryEndpoint MatchTypedRestPatterns(this OpenApiDocument document, PatternRegistry? patterns = null)
    {
        var matcher = new PatternMatcher(patterns ?? PatternRegistry.Default);

        var entryEndpoint = new EntryEndpoint();
        entryEndpoint.Children.AddRange(matcher.GetEndpoints(PathTree.From(document.Paths ?? new OpenApiPaths())));

        return entryEndpoint;
    }
}
