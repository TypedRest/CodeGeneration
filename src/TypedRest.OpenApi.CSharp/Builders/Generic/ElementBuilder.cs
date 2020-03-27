using System;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Generic;

namespace TypedRest.OpenApi.CSharp.Builders.Generic
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ElementEndpoint"/>s.
    /// </summary>
    public class ElementBuilder : BuilderBase<ElementEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(ElementEndpoint endpoint, INamingConvention naming)
            => new CSharpIdentifier(Namespace.Name, "ElementEndpoint")
            {
                TypeArguments = {naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
            };
    }
}
