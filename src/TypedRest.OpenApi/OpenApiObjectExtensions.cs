using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace TypedRest.OpenApi
{
    /// <summary>
    /// Provides extension methods for <see cref="OpenApiObject"/>s.
    /// </summary>
    public static class OpenApiObjectExtensions
    {
        /// <summary>
        /// Gets a string property with specified <paramref name="name"/> from the <paramref name="obj"/>.
        /// </summary>
        /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
        public static string GetString(this OpenApiObject obj, string name)
            => obj.TryGetValue(name, out var anyData) && anyData is OpenApiString stringData ? stringData.Value : null;

        /// <summary>
        /// Gets a int property with specified <paramref name="name"/> from the <paramref name="obj"/>.
        /// </summary>
        /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
        public static int? GetInt(this OpenApiObject obj, string name)
            => obj.TryGetValue(name, out var anyData) && anyData is OpenApiInteger intData ? intData.Value : (int?)null;

        /// <summary>
        /// Gets a schema property with specified <paramref name="name"/> from the <paramref name="obj"/>.
        /// </summary>
        /// <returns>The value of the property or <c>null</c> if it was not found or had the wrong type.</returns>
        public static OpenApiSchema GetSchema(this OpenApiObject obj, string name)
        {
            if (!obj.TryGetObject("name", out var schemaObj)) return null;
            string schemaRef = schemaObj.GetString("$ref");
            if (schemaRef == null) return null;

            OpenApiSchema FromRefPrefix(string prefix)
            {
                if (!schemaRef.StartsWith(prefix)) return null;

                return new OpenApiSchema
                {
                    Reference = new OpenApiReference
                    {
                        Id = schemaRef.Substring(prefix.Length),
                        Type = ReferenceType.Schema
                    }
                };
            }

            return FromRefPrefix("#/components/schemas/")
                ?? FromRefPrefix("#/definitions/");
        }

        /// <summary>
        /// Tries to get an object property with specified <paramref name="name"/> from the <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to get the property from.</param>
        /// <param name="name">The name of the property to look for.</param>
        /// <param name="result">The value of the property</param>
        /// <returns><c>true</c> if the property was found; <c>false</c> if not.</returns>
        public static bool TryGetObject(this OpenApiObject obj, string name, out OpenApiObject result)
        {
            if (obj.TryGetValue(name, out var anyData) && anyData is OpenApiObject objData)
            {
                result = objData;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
    }
}
