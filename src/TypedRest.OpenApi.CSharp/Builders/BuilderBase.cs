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
        CSharpClassConstruction IBuilder.GetConstruction(IEndpoint endpoint, ITypeList typeList)
            => GetConstruction((TEndpoint)endpoint, typeList);

        public CSharpClassConstruction GetConstruction(TEndpoint endpoint, ITypeList typeList)
        {
            var construction = new CSharpClassConstruction(GetImplementation(endpoint, typeList));
            construction.Parameters.AddRange(GetParameters(endpoint));
            return construction;
        }

        public CSharpIdentifier GetInterface(IEndpoint endpoint, ITypeList typeList)
            => GetInterface((TEndpoint)endpoint, typeList);

        public virtual CSharpIdentifier GetInterface(TEndpoint endpoint, ITypeList typeList)
            => GetImplementation(endpoint, typeList).ToInterface();

        protected abstract CSharpIdentifier GetImplementation(TEndpoint endpoint, ITypeList typeList);

        protected virtual IEnumerable<CSharpParameter> GetParameters(TEndpoint endpoint)
        {
            yield return new CSharpParameter(new CSharpIdentifier("TypedRest.Endpoints", "IEndpoint"), "referrer") {Value = new ThisReference()};
            yield return string.IsNullOrEmpty(endpoint.Uri)
                ? new CSharpParameter(CSharpIdentifier.Uri, "relativeUri")
                : new CSharpParameter(CSharpIdentifier.String, "relativeUri") {Value = endpoint.Uri};
        }
    }
}
