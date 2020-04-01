using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Builders
{
    /// <summary>
    /// Builds C# code snippets for <see cref="Endpoint"/>s.
    /// </summary>
    public class DefaultBuilder : BuilderBase<Endpoint>
    {
        protected override CSharpIdentifier GetImplementationType(Endpoint endpoint, INamingStrategy naming)
            => new CSharpIdentifier(Namespace.Name, "EndpointBase");

        protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
            => new CSharpIdentifier(Namespace.Name, "IEndpoint");
    }
}
