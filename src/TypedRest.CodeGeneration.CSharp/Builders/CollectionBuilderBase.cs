using System;
using System.Collections.Generic;
using System.Linq;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp.Builders
{
    /// <summary>
    /// Common base class for <see cref="IBuilder{TEndpoint}"/>s for <see cref="CollectionEndpoint"/> and derived types.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="CollectionEndpoint"/> to generate code for.</typeparam>
    public abstract class CollectionBuilderBase<TEndpoint> : BuilderBase<TEndpoint>
        where TEndpoint : CollectionEndpoint
    {
        protected abstract string TypeNamespace { get; }

        protected abstract string TypeName { get; }

        protected override CSharpIdentifier GetImplementationType(TEndpoint endpoint, INamingStrategy naming)
            => new CSharpIdentifier(TypeNamespace, TypeName)
            {
                TypeArguments =
                {
                    naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))
                }
            };

        protected override (IEnumerable<ICSharpType> types, IEnumerable<CSharpIdentifier> typeArguments) GetAdditional(string key, TEndpoint endpoint, IEndpointGenerator generator)
        {
            if (endpoint.Element == null)
                return (Enumerable.Empty<CSharpType>(), Enumerable.Empty<CSharpIdentifier>());

            if (endpoint.Element.Schema == null)
                endpoint.Element.Schema = endpoint.Schema;

            string elementKey = key.TrimEnd('s') + "Element";
            var (property, types) = generator.Generate(elementKey, endpoint.Element);
            return (types, typeArguments: new [] {property.GetterExpression!.Type});
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
