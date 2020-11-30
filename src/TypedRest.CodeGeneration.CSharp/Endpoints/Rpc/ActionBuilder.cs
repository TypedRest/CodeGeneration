using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ActionEndpoint"/>s.
    /// </summary>
    public class ActionBuilder : BuilderBase<ActionEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(ActionEndpoint endpoint, INamingStrategy naming)
            => new(Namespace.Name, "ActionEndpoint");
    }
}
