using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi
{
    /// <summary>
    /// Provides extension methods for <see cref="IOpenApiWriter"/>s.
    /// </summary>
    public static class OpenApiWriterExtensions
    {
        /// <summary>
        /// Writes an optional Open API object.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="specVersion">The OpenAPI Spec version.</param>
        public static void WriteOptionalObject<T>([NotNull] this IOpenApiWriter writer, [NotNull] string name, [CanBeNull] T value, OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable, IOpenApiExtension
            => writer.WriteOptionalObject(name, value, (w, v) => v.Write(w, specVersion));

        /// <summary>
        /// Writes an optional Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type. <see cref="IOpenApiElement"/></typeparam>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="specVersion">The OpenAPI Spec version.</param>
        public static void WriteOptionalMap<T>([NotNull] this IOpenApiWriter writer, [NotNull] string name, [NotNull] IDictionary<string, T> elements, OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable, IOpenApiExtension
            => writer.WriteOptionalMap(name, elements, (w, v) => v.Write(w, specVersion));
    }
}
