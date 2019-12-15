using System;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Rpc;

namespace TypedRest.OpenApi.CSharp.Builders.Rpc
{
    /// <summary>
    /// Builds C# code snippets for <see cref="ProducerEndpoint"/>s.
    /// </summary>
    public class ProducerBuilder : BuilderBase<ProducerEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(ProducerEndpoint endpoint, INamingConvention naming)
            => new CSharpIdentifier(Namespace.Name, "ProducerEndpoint")
            {
                TypeArguments = {naming.DtoFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
            };
    }
}
