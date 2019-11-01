using System.Collections;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public class TypeList : ITypeList
    {
        private readonly List<CSharpType> _types = new List<CSharpType>();

        public IEnumerator<CSharpType> GetEnumerator()
            => _types.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _types.GetEnumerator();

        public void Add(CSharpClass type)
            => _types.Add(type);

        private readonly Dictionary<IEndpoint, CSharpIdentifier> _endpointTypes = new Dictionary<IEndpoint, CSharpIdentifier>();

        public CSharpIdentifier this[IEndpoint endpoint]
            => _endpointTypes[endpoint];

        public void Add(IEndpoint endpoint, CSharpClass type)
        {
            Add(type);

            _endpointTypes.Add(endpoint, type.Identifier);
        }

        private readonly Dictionary<string, CSharpIdentifier> _schemaTypes = new Dictionary<string, CSharpIdentifier>();

        public CSharpIdentifier this[OpenApiSchema schema]
            => new CSharpIdentifier("Schemas", schema.Reference?.Id ?? schema.Type); // TODO: _schemaTypes[schema.Reference.Id];

        public void Add(OpenApiSchema schema, CSharpClass type)
        {
            Add(type);

            string key = schema.Reference?.Id ?? schema.Type;
            if (!string.IsNullOrEmpty(key))
                _schemaTypes.Add(key, type.Identifier);
        }
    }
}
