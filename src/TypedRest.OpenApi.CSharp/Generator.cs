using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using TypedRest.OpenApi.CSharp.Builders;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp
{
    public class Generator
    {
        private readonly INamingConvention _naming;
        private readonly BuilderRegistry _builders;

        public Generator(INamingConvention naming, BuilderRegistry? builders = null)
        {
            _naming = naming;
            _builders = builders ?? BuilderRegistry.Default;
        }

        public bool GenerateInterfaces { get; set; } = true;

        private TypeList _typeList = default!;
        private EndpointList _endpoints = default!;
        private IDictionary<string, OpenApiSchema> _schemas = default!;

        public ITypeList Generate(EntryEndpoint endpoint, IDictionary<string, OpenApiSchema> schemas)
        {
            _typeList = new TypeList();
            _endpoints = endpoint.Children;
            _schemas = schemas;

            GenerateDtos();
            GenerateEntryEndpoint();

            return _typeList;
        }

        private void GenerateDtos()
        {
            foreach (var pair in _schemas)
            {
                var type = GenerateDto(_naming.DtoType(pair.Key));
                _typeList.Add(pair.Value, type);
            }
        }

        private CSharpClass GenerateDto(CSharpIdentifier identifier)
        {
            // TODO: Proper implementation
            return new CSharpClass(identifier);
        }

        private void GenerateEntryEndpoint()
        {
            var endpoint = new CSharpClass(_naming.EntryEndpointType())
            {
                BaseClass = new CSharpClassConstruction(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                {
                    Parameters = {new CSharpParameter(CSharpIdentifier.Uri, "uri")}
                }
            };

            endpoint.Properties.AddRange(GenerateEndpoints(_endpoints));
            _typeList.Add(endpoint);
        }

        private IEnumerable<CSharpProperty> GenerateEndpoints(EndpointList endpoints)
            => endpoints.Select(x => GenerateEndpoint(x.Key, x.Value));

        private CSharpProperty GenerateEndpoint(string key, IEndpoint endpoint)
        {
            GenerateElementEndpoint(key, endpoint);
            var children = GenerateEndpoints(endpoint.Children).ToList();

            return children.Count == 0
                ? GenerateEndpointWithoutChildren(key, endpoint)
                : GenerateEndpointWithChildren(key, endpoint, children);
        }

        private CSharpProperty GenerateEndpointWithoutChildren(string key, IEndpoint endpoint)
        {
            var builder = _builders.For(endpoint);

            return new CSharpProperty(builder.GetInterface(endpoint, _typeList), _naming.Property(key))
            {
                GetterExpression = builder.GetConstruction(endpoint, _typeList),
                Description = endpoint.Description
            };
        }

        private CSharpProperty GenerateEndpointWithChildren(string key, IEndpoint endpoint, IList<CSharpProperty> children)
        {
            var endpointImplementation = GenerateEndpointImplementation(key, endpoint, children);

            var identifier = GenerateInterfaces
                ? ExtractInterface(endpoint, endpointImplementation)
                : endpointImplementation.Identifier;

            return new CSharpProperty(identifier, key)
            {
                GetterExpression = endpointImplementation.GetConstruction(),
                Description = endpoint.Description
            };
        }

        private CSharpClass GenerateEndpointImplementation(string key, IEndpoint endpoint, IList<CSharpProperty> children)
        {
            var builder = _builders.For(endpoint);

            var endpointImplementation = new CSharpClass(_naming.EndpointType(key, endpoint))
            {
                BaseClass = builder.GetConstruction(endpoint, _typeList),
                Description = endpoint.Description
            };
            endpointImplementation.Properties.AddRange(children);

            _typeList.Add(endpoint, endpointImplementation);
            return endpointImplementation;
        }

        private CSharpIdentifier ExtractInterface(IEndpoint endpoint, CSharpClass implementation)
        {
            var builder = _builders.For(endpoint);

            var endpointInterface = new CSharpInterface(implementation.Identifier.ToInterface())
            {
                Interfaces = {builder.GetInterface(endpoint, _typeList)},
                Description = endpoint.Description
            };
            foreach (var property in implementation.Properties)
                endpointInterface.Properties.Add(new CSharpProperty(property.Type, property.Name) {Description = property.Description});

            implementation.Interfaces.Add(endpointInterface.Identifier);

            _typeList.Add(endpoint, endpointInterface);
            return endpointInterface.Identifier;
        }

        private void GenerateElementEndpoint(string key, IEndpoint endpoint)
        {
            if (endpoint is IndexerEndpoint indexerEndpoint && indexerEndpoint.Element != null)
            {
                if (indexerEndpoint is CollectionEndpoint collectionEndpoint && collectionEndpoint.Element is ElementEndpoint elementEndpoint && elementEndpoint.Schema == null)
                    elementEndpoint.Schema = collectionEndpoint.Schema;

                GenerateEndpoint(key.TrimEnd('s'), indexerEndpoint.Element);
            }
        }
    }
}
