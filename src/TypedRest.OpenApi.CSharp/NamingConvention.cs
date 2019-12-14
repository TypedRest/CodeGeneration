using System;
using System.Linq;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp
{
    public class NamingConvention : INamingConvention
    {
        private readonly string _namespace;
        private readonly string _serviceName;

        public NamingConvention(string ns, string serviceName)
        {
            _namespace = ns;
            _serviceName = serviceName;
        }

        public virtual string Property(string key)
            => ToPascalCase(key);

        public CSharpIdentifier EntryEndpointType() => new CSharpIdentifier(
            _namespace,
            _serviceName + "Client");

        public virtual CSharpIdentifier EndpointType(string key, IEndpoint endpoint)
            => new CSharpIdentifier(
                _namespace,
                endpoint is IndexerEndpoint && key.EndsWith("s")
                    ? ToPascalCase(key.Substring(0, key.Length - 1)) + "CollectionEndpoint"
                    : ToPascalCase(key) + "Endpoint");

        public CSharpIdentifier DtoType(string key)
            => new CSharpIdentifier(
                _namespace,
                ToPascalCase(key));

        protected static string ToPascalCase(string key)
            => key.Contains('-') || key.Contains('_')
                // kebap-case or snake_case
                ? string.Concat(key.Split(new[] {'-', '_'}, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower()))
                // camelCase
                : key.Substring(0, 1).ToUpper() + key.Substring(1);
    }
}
