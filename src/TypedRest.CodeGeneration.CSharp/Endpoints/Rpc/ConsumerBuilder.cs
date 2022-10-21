using System;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Rpc;

/// <summary>
/// Builds C# code snippets for <see cref="ConsumerEndpoint"/>s.
/// </summary>
public class ConsumerBuilder : BuilderBase<ConsumerEndpoint>
{
    protected override CSharpIdentifier GetImplementationType(ConsumerEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "ConsumerEndpoint")
        {
            TypeArguments = {naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
        };
}
