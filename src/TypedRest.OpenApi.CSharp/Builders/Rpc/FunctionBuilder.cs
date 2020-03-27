using System;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="FunctionEndpoint"/>s.
    /// </summary>
    public class FunctionBuilder : BuilderBase<FunctionEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(FunctionEndpoint endpoint, INamingConvention naming)
            => new CSharpIdentifier(Namespace.Name, "FunctionEndpoint")
            {
                TypeArguments =
                {
                    naming.TypeFor(endpoint.RequestSchema ?? throw new InvalidOperationException($"Missing request schema for {endpoint}.")),
                    naming.TypeFor(endpoint.ResponseSchema ?? throw new InvalidOperationException($"Missing response schema for {endpoint}."))
                }
            };
    }
}
