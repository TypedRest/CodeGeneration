using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Builds C# code snippets for <see cref="Endpoint"/>s.
    /// </summary>
    public class DefaultBuilder : BuilderBase<Endpoint>
    {
        protected override CSharpIdentifier GetImplementation(Endpoint endpoint, ITypeList typeList)
            => new CSharpIdentifier("TypedRest.Endpoints", "EndpointBase");
    }
}
