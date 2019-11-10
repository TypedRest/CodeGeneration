using System;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ConsumerEndpoint"/>s.
    /// </summary>
    public class ConsumerBuilder : BuilderBase<ConsumerEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(ConsumerEndpoint endpoint, ITypeList typeList)
            => new CSharpIdentifier(Namespace.Name, "ConsumerEndpoint")
            {
                TypeArguments = {typeList.For(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
            };
    }
}
