using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

/// <summary>
/// Builds C# code snippets for <see cref="EntryEndpoint"/>s.
/// </summary>
public class EntryBuilder : BuilderBase<EntryEndpoint>
{
    protected override CSharpIdentifier GetImplementationType(EntryEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "EntryEndpoint");

    protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType)
        => new(Namespace.Name, "IEndpoint");

    protected override IEnumerable<CSharpParameter> GetParameters(EntryEndpoint endpoint)
        => [new CSharpParameter(CSharpIdentifier.Uri, "uri")];
}
