using System;
using System.Linq;
using JetBrains.Annotations;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp
{
    public class NamingConvention : INamingConvention
    {
        private readonly string _namespace;

        public NamingConvention(string ns)
        {
            _namespace = ns;
        }

        public virtual string Property(string key)
            => ToPascalCase(key);

        public virtual CSharpIdentifier EndpointType(string key, IEndpoint endpoint)
            => new CSharpIdentifier(
                _namespace,
                endpoint is IndexerEndpoint && key.EndsWith("s")
                    ? ToPascalCase(key.Substring(0, key.Length - 1)) + "CollectionEndpoint"
                    : ToPascalCase(key) + "Endpoint");

        [NotNull]
        protected static string ToPascalCase([NotNull] string key)
            => key.Contains('-') || key.Contains('_')
                // kebap-case or snake_case
                ? string.Concat(key.Split(new[] {'-', '_'}, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(word => word.Substring(0, 1).ToUpper() + word.Substring(1).ToLower()))
                // camelCase
                : key.Substring(0, 1).ToUpper() + key.Substring(1);
    }
}
