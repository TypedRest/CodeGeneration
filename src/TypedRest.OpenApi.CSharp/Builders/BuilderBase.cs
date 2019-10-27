using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Common base class for <see cref="IBuilder{TEndpoint}"/>s.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
    public abstract class BuilderBase<TEndpoint> : IBuilder<TEndpoint>
        where TEndpoint : IEndpoint
    {
        CSharpIdentifier IBuilder.GetInterface(IEndpoint endpoint, ITypeLookup typeLookup)
            => GetInterface((TEndpoint)endpoint, typeLookup);

        public virtual CSharpIdentifier GetInterface(TEndpoint endpoint, ITypeLookup typeLookup)
        {
            var classType = GetConstruction(endpoint, typeLookup).Type;
            var interfaceType = new CSharpIdentifier(classType.Namespace, "I" + classType.Name);
            interfaceType.TypeArguments.AddRange(classType.TypeArguments);
            return interfaceType;
        }

        CSharpClassConstruction IBuilder.GetConstruction(IEndpoint endpoint, ITypeLookup typeLookup)
            => GetConstruction((TEndpoint)endpoint, typeLookup);

        public CSharpClassConstruction GetConstruction(TEndpoint endpoint, ITypeLookup typeLookup)
        {
            var construction = new CSharpClassConstruction(GetImplementation(endpoint, typeLookup));
            construction.Parameters.AddRange(GetParameters(endpoint));
            return construction;
        }

        protected abstract CSharpIdentifier GetImplementation(TEndpoint endpoint, ITypeLookup typeLookup);

        protected virtual IEnumerable<CSharpParameter> GetParameters(TEndpoint endpoint)
        {
            yield return new CSharpParameter(CSharpIdentifier.String, "relativeUri", endpoint.Uri);
        }
    }
}
