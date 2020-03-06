using System;
using System.Collections.Generic;
using System.Linq;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Common base class for <see cref="IBuilder{TEndpoint}"/>s for <see cref="CollectionEndpoint"/> and derived types.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="CollectionEndpoint"/> to generate code for.</typeparam>
    public abstract class CollectionBuilderBase<TEndpoint> : IndexerBuilderBase<TEndpoint>
        where TEndpoint : CollectionEndpoint
    {
        protected abstract string TypeNamespace { get; }

        protected abstract string TypeName { get; }

        protected override CSharpIdentifier GetImplementationType(TEndpoint endpoint, INamingConvention naming)
            => new CSharpIdentifier(TypeNamespace, TypeName)
            {
                TypeArguments =
                {
                    naming.DtoFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))
                }
            };

        protected override (IEnumerable<ICSharpType> types, IEnumerable<CSharpIdentifier> typeArguments) GetAdditional(string key, TEndpoint endpoint, IGenerator generator)
        {
            if (endpoint.Element == null)
                return (Enumerable.Empty<CSharpType>(), Enumerable.Empty<CSharpIdentifier>());

            if (endpoint.Element is ElementEndpoint elementEndpoint && elementEndpoint.Schema == null)
                elementEndpoint.Schema = endpoint.Schema;

            return base.GetAdditional(key, endpoint, generator);
        }

        protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
        {
            var identifier = implementationType.ToInterface();

            if (identifier.TypeArguments.Count == 2)
                identifier.TypeArguments[1] = identifier.TypeArguments[1].ToInterface();

            return identifier;
        }
    }
}
