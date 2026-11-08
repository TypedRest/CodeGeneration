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

    protected override CSharpIdentifier GetInterfaceType(CSharpIdentifier implementationType, bool withInterfaces)
        => new(Namespace.Name, "IEndpoint");

    protected override IEnumerable<CSharpParameter> GetParameters(EntryEndpoint endpoint, IEndpointGenerator generator)
    {
        if (!generator.GenerateEntryConstructor) yield break;

        yield return new CSharpParameter(CSharpIdentifier.Uri, "uri");

        if (generator.RuntimeSerializer is {} serializer)
            yield return new CSharpParameter(serializer, "serializer") {Value = new CSharpObjectCreation(serializer)};
    }
}
