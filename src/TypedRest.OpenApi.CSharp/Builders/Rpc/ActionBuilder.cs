using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ActionEndpoint"/>s.
    /// </summary>
    public class ActionBuilder : BuilderBase<ActionEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(ActionEndpoint endpoint, INamingStrategy naming)
            => new CSharpIdentifier(Namespace.Name, "ActionEndpoint");
    }
}
