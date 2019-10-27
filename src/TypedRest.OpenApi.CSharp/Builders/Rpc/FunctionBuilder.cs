using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="FunctionEndpoint"/>s.
    /// </summary>
    public class FunctionBuilder : BuilderBase<FunctionEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(FunctionEndpoint endpoint, ITypeLookup typeLookup)
            => new CSharpIdentifier(Namespace.Name, "FunctionEndpoint")
            {
                TypeArguments =
                {
                    typeLookup[endpoint.RequestSchema],
                    typeLookup[endpoint.ResponseSchema]
                }
            };
    }
}
