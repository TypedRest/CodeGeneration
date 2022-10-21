using System;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Rpc;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Rpc;

/// <summary>
/// Builds C# code snippets for <see cref="FunctionEndpoint"/>s.
/// </summary>
public class FunctionBuilder : BuilderBase<FunctionEndpoint>
{
    protected override CSharpIdentifier GetImplementationType(FunctionEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "FunctionEndpoint")
        {
            TypeArguments =
            {
                naming.TypeFor(endpoint.RequestSchema ?? throw new InvalidOperationException($"Missing request schema for {endpoint}.")),
                naming.TypeFor(endpoint.ResponseSchema ?? throw new InvalidOperationException($"Missing response schema for {endpoint}."))
            }
        };
}
