using System.Collections.Generic;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace TypedRest.OpenApi.Endpoints
{
    /// <summary>
    /// A list of named <see cref="IEndpoint"/>s.
    /// </summary>
    /// <seealso cref="OpenApiDocumentExtensions.GetTypedRestEndpoints"/>
    /// <seealso cref="OpenApiDocumentExtensions.SetTypedRestEndpoints"/>
    public class EndpointList : Dictionary<string, IEndpoint>, IOpenApiSerializable, IOpenApiExtension
    {
        /// <summary>
        /// The property name used to add this as an extension to an <see cref="OpenApiDocument"/>.
        /// </summary>
        public const string ExtensionKey = "x-endpoints";

        /// <summary>
        /// Adds <see cref="IEndpoint"/>s parsed from an OpenAPI Object.
        /// </summary>
        /// <param name="data">The OpenAPI Object to parse.</param>
        /// <param name="parser">The endpoint parser to use.</param>
        public void Parse(OpenApiObject data, IEndpointParser parser)
        {
            foreach (var property in data)
            {
                if (property.Value is OpenApiObject objData)
                    Add(property.Key, parser.Parse(objData));
            }
        }

        /// <summary>
        /// Resolves <see cref="OpenApiReference"/>s in the contained <see cref="IEndpoint"/>s.
        /// </summary>
        /// <param name="components">The components that references can point to.</param>
        public void ResolveReferences(OpenApiComponents components)
        {
            foreach (var child in Values)
                child.ResolveReferences(components);
        }

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteStartObject();
            foreach (var item in this)
                writer.WriteOptionalObject(item.Key, item.Value, specVersion);
            writer.WriteEndObject();
        }

        public void SerializeAsV2(IOpenApiWriter writer)
            => Write(writer, OpenApiSpecVersion.OpenApi2_0);

        public void SerializeAsV3(IOpenApiWriter writer)
            => Write(writer, OpenApiSpecVersion.OpenApi3_0);

        /// <summary>
        /// Adds all <paramref name="endpoints"/> to the list.
        /// </summary>
        public void AddRange(IDictionary<string, IEndpoint> endpoints)
        {
            foreach (var pair in endpoints)
                Add(pair.Key, pair.Value);
        }
    }
}
