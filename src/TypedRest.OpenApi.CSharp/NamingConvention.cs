using System;
using System.Linq;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp
{
    public class NamingConvention : INamingConvention
    {
        private readonly string _serviceName;
        private readonly string _endpointNamespace;
        private readonly string _dtoNamespace;

        public NamingConvention(string serviceName, string endpointNamespace, string dtoNamespace)
        {
            _serviceName = serviceName;
            _endpointNamespace = endpointNamespace;
            _dtoNamespace = dtoNamespace;
        }

        public virtual string Property(string key)
            => ToPascalCase(key);

        public virtual CSharpIdentifier EndpointType(string key, IEndpoint endpoint)
            => new CSharpIdentifier(
                _endpointNamespace,
                endpoint switch
                {
                    EntryEndpoint _ => (_serviceName + "Client"),
                    IndexerEndpoint _ when key.EndsWith("s") => ToPascalCase(key.Substring(0, key.Length - 1)) + "CollectionEndpoint",
                    _ => ToPascalCase(key) + "Endpoint"
                });

        public CSharpIdentifier DtoType(string key)
            => new CSharpIdentifier(
                _dtoNamespace,
                ToPascalCase(key));

        public CSharpIdentifier TypeFor(OpenApiSchema schema)
            => (schema.Type, schema.Format) switch
            {
                ("array", _) => CSharpIdentifier.ListOf(TypeFor(schema.Items)),
                ("string", "uri") => CSharpIdentifier.Uri,
                ("string", _) => CSharpIdentifier.String,
                ("int", "int64") => CSharpIdentifier.Long,
                ("int", _) => CSharpIdentifier.Int,
                ("number", "float") => CSharpIdentifier.Float,
                ("number", _) => CSharpIdentifier.Double,
                ("boolean", _) => CSharpIdentifier.Bool,
                _ when !string.IsNullOrEmpty(schema.Reference?.Id) => DtoType(schema.Reference.Id),
                _ => CSharpIdentifier.Object
            };

        protected static string ToPascalCase(string key)
            => key switch
            {
                "" => "",
                null => "",
                // kebap-case or snake_case
                {} when key.Contains('-') || key.Contains('_') => string.Concat(key.Split(new[] {'-', '_'}, StringSplitOptions.RemoveEmptyEntries).Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower())),
                // CamelCase
                _ => (key.Substring(0, 1).ToUpper() + key.Substring(1))
            };
    }
}
