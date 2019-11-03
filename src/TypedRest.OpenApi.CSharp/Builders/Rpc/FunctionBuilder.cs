using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="FunctionEndpoint"/>s.
    /// </summary>
    public class FunctionBuilder : BuilderBase<FunctionEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(FunctionEndpoint endpoint, ITypeList typeList)
            => new CSharpIdentifier(Namespace.Name, "FunctionEndpoint")
            {
                TypeArguments =
                {
                    typeList.For(endpoint.RequestSchema),
                    typeList.For(endpoint.ResponseSchema)
                }
            };
    }
}
