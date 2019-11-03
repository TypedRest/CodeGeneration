using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Common base class for <see cref="IBuilder{TEndpoint}"/>s for <see cref="CollectionEndpoint"/> and derived types.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="CollectionEndpoint"/> to generate code for.</typeparam>
    public abstract class CollectionBuilderBase<TEndpoint> : BuilderBase<TEndpoint>
        where TEndpoint : CollectionEndpoint
    {
        protected override CSharpIdentifier GetImplementation(TEndpoint endpoint, ITypeList typeList)
        {
            var identifier = new CSharpIdentifier(TypeNamespace, TypeName);
            identifier.TypeArguments.Add(typeList.For(endpoint.Schema));

            if (endpoint.Element != null)
                identifier.TypeArguments.Add(typeList.ImplementationFor(endpoint.Element));

            return identifier;
        }

        public override CSharpIdentifier GetInterface(TEndpoint endpoint, ITypeList typeList)
        {
            var identifier = new CSharpIdentifier(TypeNamespace, TypeName).ToInterface();
            identifier.TypeArguments.Add(typeList.For(endpoint.Schema));

            if (endpoint.Element != null)
                identifier.TypeArguments.Add(typeList.InterfaceFor(endpoint.Element));

            return identifier;
        }

        protected abstract string TypeNamespace { get; }

        protected abstract string TypeName { get; }
    }
}
