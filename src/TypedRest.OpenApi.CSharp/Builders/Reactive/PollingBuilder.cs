using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Reactive;

namespace TypedRest.OpenApi.CSharp.Builders.Reactive
{
    /// <summary>
    /// Builds C# code snippets for <see cref="PollingEndpoint"/>s.
    /// </summary>
    public class PollingBuilder : BuilderBase<PollingEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(PollingEndpoint endpoint, ITypeList typeList)
            => new CSharpIdentifier(Namespace.Name, "PollingEndpoint")
            {
                TypeArguments = {typeList[endpoint.Schema]}
            };
    }
}
