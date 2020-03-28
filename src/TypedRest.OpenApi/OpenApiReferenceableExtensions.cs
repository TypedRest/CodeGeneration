using System.Collections.Generic;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi
{
    /// <summary>
    /// Provides extension methods for <see cref="IOpenApiReferenceable"/>s.
    /// </summary>
    public static class OpenApiReferenceableExtensions
    {
        /// <summary>
        /// Resolves the <see cref="OpenApiSchema.Reference"/> in the <paramref name="schema"/>, if present, using <paramref name="components"/>.
        /// </summary>
        public static OpenApiSchema Resolve(this OpenApiSchema schema, OpenApiComponents components)
            => schema.Resolve(components.Schemas);

        /// <summary>
        /// Resolves the <see cref="IOpenApiReferenceable.Reference"/> in the <paramref name="referenceable"/>, if present, using <paramref name="targets"/>.
        /// </summary>
        public static T Resolve<T>(this T referenceable, IDictionary<string, T> targets)
            where T : IOpenApiReferenceable
            => referenceable.Reference?.Id != null && targets.TryGetValue(referenceable.Reference.Id, out var resolved)
                ? resolved
                : referenceable;
    }
}
