using System.Collections.Generic;
using JetBrains.Annotations;
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
        public static OpenApiSchema Resolve([NotNull] this OpenApiSchema schema, [NotNull] OpenApiComponents components)
            => schema.Resolve(components.Schemas);

        /// <summary>
        /// Resolves the <see cref="OpenApiSchema.Reference"/> in the <paramref name="link"/>, if present, using <paramref name="components"/>.
        /// </summary>
        public static OpenApiLink Resolve([NotNull] this OpenApiLink link, [NotNull] OpenApiComponents components)
            => link.Resolve(components.Links);

        /// <summary>
        /// Resolves the <see cref="IOpenApiReferenceable.Reference"/> in the <paramref name="referenceable"/>, if present, using <paramref name="targets"/>.
        /// </summary>
        public static T Resolve<T>([NotNull] this T referenceable, [NotNull] IDictionary<string, T> targets)
            where T : IOpenApiReferenceable
            => referenceable.Reference?.Id != null && targets.TryGetValue(referenceable.Reference.Id, out var resolved)
                ? resolved
                : referenceable;
    }
}
