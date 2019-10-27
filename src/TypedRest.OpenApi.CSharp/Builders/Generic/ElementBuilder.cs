using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders.Generic
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ElementEndpoint"/>s.
    /// </summary>
    public class ElementBuilder : BuilderBase<ElementEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(ElementEndpoint endpoint, ITypeLookup typeLookup)
            => new CSharpIdentifier(Namespace.Name, "ElementEndpoint")
            {
                TypeArguments = {typeLookup[endpoint.Schema]}
            };
    }
}
