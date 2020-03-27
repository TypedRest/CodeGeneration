using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Builds C# code snippets for <see cref="EntryEndpoint"/>s.
    /// </summary>
    public class EntryBuilder : BuilderBase<EntryEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(EntryEndpoint endpoint, INamingStrategy naming)
            => new CSharpIdentifier(Namespace.Name, "EntryEndpoint");

        protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
            => new CSharpIdentifier(Namespace.Name, "IEndpoint");

        protected override IEnumerable<CSharpParameter> GetParameters(EntryEndpoint endpoint)
            => new[] {new CSharpParameter(CSharpIdentifier.Uri, "uri")};
    }
}
