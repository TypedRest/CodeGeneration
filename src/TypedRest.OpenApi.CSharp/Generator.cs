using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TypedRest.OpenApi.CSharp.Builders;
using TypedRest.OpenApi.CSharp.Builders.Generic;
using TypedRest.OpenApi.CSharp.Builders.Raw;
using TypedRest.OpenApi.CSharp.Builders.Rpc;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp
{
    public class Generator
    {
        private readonly INamingConvention _naming;
        private readonly IDictionary<Type, IBuilder> _builders = new Dictionary<Type, IBuilder>();

        public Generator([NotNull] INamingConvention naming)
        {
            _naming = naming;

            Add(new DefaultBuilder());
            Add(new UploadBuilder());
            Add(new BlobBuilder());
            Add(new ActionBuilder());
            Add(new ProducerBuilder());
            Add(new ConsumerBuilder());
            Add(new FunctionBuilder());
            Add(new ElementBuilder());
            Add(new IndexerBuilder());
            Add(new CollectionBuilder());
        }

        public void Add<TEndpoint>(IBuilder<TEndpoint> builder)
            where TEndpoint : IEndpoint
            => _builders.Add(typeof(TEndpoint), builder);

        [NotNull, ItemNotNull]
        public ITypeList Generate([NotNull] EndpointList endpoints)
        {
            var typeList = new TypeList();

            var entryEndpoint = GenerateEntryEndpoint();
            entryEndpoint.Properties.AddRange(GenerateEndpoints(endpoints, typeList));
            typeList.Add(entryEndpoint);

            return typeList;
        }

        // TODO: Make entry endpoint configurable
        private CSharpClass GenerateEntryEndpoint()
            => new CSharpClass(_naming.EndpointType("myEntry", new Endpoint()))
            {
                BaseClass = new CSharpClassConstruction(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                {
                    Parameters = {new CSharpParameter(new CSharpIdentifier("System", "Uri"), "uri")}
                }
            };

        private IEnumerable<CSharpProperty> GenerateEndpoints(EndpointList endpoints, TypeList typeList)
            => endpoints.Select(x => GenerateEndpoints(x.Key, x.Value, typeList));

        private CSharpProperty GenerateEndpoints(string key, IEndpoint endpoint, TypeList typeList)
        {
            GenerateElementEndpoint(key, endpoint, typeList);
            var children = GenerateEndpoints(endpoint.Children, typeList).ToList();

            var builder = _builders[endpoint.GetType()];
            var construction = builder.GetConstruction(endpoint, typeList);
            if (children.Count == 0)
            {
                return new CSharpProperty(construction.Type.ToInterface(), _naming.Property(key))
                {
                    GetterExpression = construction
                };
            }
            else
            {
                var type = new CSharpClass(_naming.EndpointType(key, endpoint))
                {
                    BaseClass = construction
                };
                type.Properties.AddRange(children);
                typeList.Add(endpoint, type);

                return new CSharpProperty(type.Identifier, key)
                {
                    GetterExpression = new CSharpClassConstruction(type.Identifier)
                };
            }
        }

        private void GenerateElementEndpoint(string key, IEndpoint endpoint, TypeList typeList)
        {
            if (endpoint is IndexerEndpoint indexerEndpoint && indexerEndpoint.Element != null)
            {
                if (indexerEndpoint is CollectionEndpoint collectionEndpoint && collectionEndpoint.Element is ElementEndpoint elementEndpoint && elementEndpoint.Schema == null)
                    elementEndpoint.Schema = collectionEndpoint.Schema;

                GenerateEndpoints(key.TrimEnd('s'), indexerEndpoint.Element, typeList);
            }
        }
    }
}
