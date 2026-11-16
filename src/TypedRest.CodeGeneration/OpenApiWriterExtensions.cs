namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="IOpenApiWriter"/>s.
/// </summary>
public static class OpenApiWriterExtensions
{
    /// <param name="element">The element to serialize.</param>
    extension(IOpenApiSerializable element)
    {
        /// <summary>
        /// Serializes an Open API element using a specific spec version.
        /// </summary>
        /// <param name="writer">The Open API writer.</param>
        /// <param name="specVersion">The OpenAPI Spec version.</param>
        public void Serialize(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            switch (specVersion)
            {
                case OpenApiSpecVersion.OpenApi2_0:
                    element.SerializeAsV2(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_0:
                    element.SerializeAsV3(writer);
                    break;
                case OpenApiSpecVersion.OpenApi3_1:
                    element.SerializeAsV31(writer);
                    break;
                default:
                    element.SerializeAsV32(writer);
                    break;
            }
        }
    }

    /// <param name="writer">The Open API writer.</param>
    extension(IOpenApiWriter writer)
    {
        /// <summary>
        /// Writes an optional Open API object.
        /// </summary>
        /// <typeparam name="T">The Open API element type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="value">The property value.</param>
        /// <param name="specVersion">The OpenAPI Spec version.</param>
        public void WriteOptionalObject<T>(string name, T? value, OpenApiSpecVersion specVersion)
            where T : class, IOpenApiSerializable
            => writer.WriteOptionalObject(name, value, (w, v) => v.Serialize(w, specVersion));

        /// <summary>
        /// Writes an optional Open API element map.
        /// </summary>
        /// <typeparam name="T">The Open API element type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="elements">The map values.</param>
        /// <param name="specVersion">The OpenAPI Spec version.</param>
        public void WriteOptionalMap<T>(string name, IDictionary<string, T> elements, OpenApiSpecVersion specVersion)
            where T : IOpenApiSerializable
            => writer.WriteOptionalMap(name, elements, (w, v) => v.Serialize(w, specVersion));
    }
}
