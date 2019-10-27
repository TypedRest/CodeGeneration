using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders.Generic
{
    /// <summary>
    /// Builds C# code snippets for <see cref="CollectionEndpoint"/>s.
    /// </summary>
    public class CollectionBuilder : BuilderBase<CollectionEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(CollectionEndpoint endpoint, ITypeLookup typeLookup)
        {
            var identifier = new CSharpIdentifier(Namespace.Name, "CollectionEndpoint")
            {
                TypeArguments = {typeLookup[endpoint.Schema]}
            };

            if (endpoint.Element != null)
                identifier.TypeArguments.Add(typeLookup[endpoint.Element]);

            return identifier;
        }
    }
}
