using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Patterns;

namespace TypedRest.OpenApi
{
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
        /// Reads an OpenAPI document from a string with support for the TypedRest extension.
        /// </summary>
        /// <param name="input">The JSON or YAML content to read.</param>
        /// <param name="endpointRegistry">A list of all known <see cref="IEndpoint"/> kinds; leave <c>null</c> for default.</param>
        public static OpenApiDocument ReadWithTypedRest(string input, EndpointRegistry? endpointRegistry = null)
        {
            var parser = new EndpointParser(endpointRegistry ?? EndpointRegistry.Default);
            var reader = new OpenApiStringReader(new OpenApiReaderSettings().AddTypedRest(parser));

            var doc = reader.Read(input, out _);
            doc.GetTypedRest()?.ResolveReferences(doc.Components);
            return doc;
        }

        /// <summary>
        /// Gets the TypedRest extension from the OpenAPI Spec <paramref name="document"/>, if present.
        /// </summary>
        /// <seealso cref="OpenApiReaderSettingsExtensions.AddTypedRest"/>
        public static EntryEndpoint? GetTypedRest(this OpenApiDocument document)
        {
            document.Extensions.TryGetValue(TypedRestKey, out var endpoint);
            return endpoint as EntryEndpoint;
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
            entryEndpoint.Children.AddRange(matcher.GetEndpoints(PathTree.From(document.Paths)));

            return entryEndpoint;
        }
    }
}
