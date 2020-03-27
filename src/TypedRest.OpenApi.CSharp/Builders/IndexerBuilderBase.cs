using System;
using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Common base class for <see cref="IBuilder{TEndpoint}"/>s for <see cref="IndexerEndpoint"/> and derived types.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="IndexerEndpoint"/> to generate code for.</typeparam>
    public abstract class IndexerBuilderBase<TEndpoint> : BuilderBase<TEndpoint>
        where TEndpoint : IndexerEndpoint
    {
        protected override (IEnumerable<ICSharpType> types, IEnumerable<CSharpIdentifier> typeArguments) GetAdditional(string key, TEndpoint endpoint, IGenerator generator)
        {
            if (endpoint.Element == null) throw new InvalidOperationException($"Missing element for endpoint '{key}'.");

            string elementKey = key.TrimEnd('s') + "Element";
            var (property, types) = generator.GetEndpoints(elementKey, endpoint.Element);
            return (types, typeArguments: new [] {property.GetterExpression!.Type});
        }
    }
}
