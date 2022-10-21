using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Generic;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Generic;

/// <summary>
/// Builds C# code snippets for <see cref="ElementEndpoint"/>s.
/// </summary>
public class ElementBuilder : BuilderBase<ElementEndpoint>
{
    protected override CSharpIdentifier GetImplementationType(ElementEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "ElementEndpoint")
        {
            TypeArguments = {naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
        };
}
