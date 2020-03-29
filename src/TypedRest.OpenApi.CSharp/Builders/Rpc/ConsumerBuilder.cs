using System;
using NanoByte.CodeGeneration;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ConsumerEndpoint"/>s.
    /// </summary>
    public class ConsumerBuilder : BuilderBase<ConsumerEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(ConsumerEndpoint endpoint, INamingStrategy naming)
            => new CSharpIdentifier(Namespace.Name, "ConsumerEndpoint")
            {
                TypeArguments = {naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
            };
    }
}
