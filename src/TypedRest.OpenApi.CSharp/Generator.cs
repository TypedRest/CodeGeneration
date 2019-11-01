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
        private readonly IDictionary<Type, IBuilder> _builders = new Dictionary<Type, IBuilder>();

        public Generator()
        {
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

        [ItemNotNull]
        public IEnumerable<CSharpType> Generate([NotNull] EndpointList endpoints)
        {
            var typeList = new TypeList();

            // TODO: Make name configurable
            var entryEndpoint = new CSharpClass(new CSharpIdentifier("MyNamespace", "MyClient"))
            {
                BaseClass = new CSharpClassConstruction(new CSharpIdentifier("TypedRest.Endpoints", "EntryEndpoint"))
                {
                    Parameters = {new CSharpParameter(new CSharpIdentifier("System", "Uri"), "uri")}
                }
            };
            entryEndpoint.Properties.AddRange(Generate(endpoints, typeList));
            typeList.Add(entryEndpoint);

            return typeList.Types;
        }

        private IEnumerable<CSharpProperty> Generate(EndpointList endpoints, TypeList typeList)
            => endpoints.Select(x => Generate(x.Key, x.Value, typeList));

        private CSharpProperty Generate(string name, IEndpoint endpoint, TypeList typeList)
        {
            // TODO: Convert name to camelcase

            if (endpoint is CollectionEndpoint collectionEndpoint && collectionEndpoint.Element is ElementEndpoint elementEndpoint && elementEndpoint.Schema == null)
                elementEndpoint.Schema = collectionEndpoint.Schema;

            if (endpoint is IndexerEndpoint indexerEndpoint && indexerEndpoint.Element != null)
                Generate(name.TrimEnd('s') + "Element", indexerEndpoint.Element, typeList);

            var properties = Generate(endpoint.Children, typeList).ToList();

            var builder = _builders[endpoint.GetType()];
            var construction = builder.GetConstruction(endpoint, typeList);
            if (properties.Count == 0)
            {
                return new CSharpProperty(construction.Type.ToInterface(), name)
                {
                    GetterExpression = construction
                };
            }
            else
            {
                // TODO: Make namespace configurable
                var type = new CSharpClass(new CSharpIdentifier("MyNamespace", name + "Endpoint"))
                {
                    BaseClass = construction
                };
                type.Properties.AddRange(properties);
                typeList.Add(endpoint, type);

                return new CSharpProperty(type.Identifier, name)
                {
                    GetterExpression = new CSharpClassConstruction(type.Identifier)
                };
            }
        }
    }
}
