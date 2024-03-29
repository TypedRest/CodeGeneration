﻿using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

/// <summary>
/// Builds C# code snippets for <see cref="Endpoint"/>s.
/// </summary>
public class DefaultBuilder : BuilderBase<Endpoint>
{
    protected override CSharpIdentifier GetImplementationType(Endpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "EndpointBase");

    protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
        => new(Namespace.Name, "IEndpoint");
}
