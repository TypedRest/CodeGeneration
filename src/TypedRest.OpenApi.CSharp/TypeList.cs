using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp
{
    public class TypeList : ITypeLookup
    {
        private readonly Dictionary<IEndpoint, CSharpIdentifier> _endpointTypes = new Dictionary<IEndpoint, CSharpIdentifier>();

        public CSharpIdentifier this[IEndpoint endpoint]
            => _endpointTypes[endpoint];

        private readonly Dictionary<string, CSharpIdentifier> _schemaTypes = new Dictionary<string, CSharpIdentifier>();

        public CSharpIdentifier this[OpenApiSchema schema]
            => new CSharpIdentifier("Schemas", schema.Reference?.Id ?? schema.Type); // TODO: _schemaTypes[schema.Reference.Id];

        private readonly List<CSharpType> _types = new List<CSharpType>();

        public IEnumerable<CSharpType> Types
            => _types;

        public void Add(CSharpClass type)
        {
            _types.Add(type);
        }

        public void Add(IEndpoint endpoint, CSharpClass type)
        {
            _types.Add(type);
            _endpointTypes.Add(endpoint, type.Identifier);
        }
    }
}
