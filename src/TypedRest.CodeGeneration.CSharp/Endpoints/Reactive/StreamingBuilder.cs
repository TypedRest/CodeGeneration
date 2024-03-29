using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Reactive;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Reactive;

/// <summary>
/// Builds C# code snippets for <see cref="StreamingEndpoint"/>s.
/// </summary>
public class StreamingBuilder : BuilderBase<StreamingEndpoint>
{
    protected override CSharpIdentifier GetImplementationType(StreamingEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "StreamingEndpoint")
        {
            TypeArguments = {naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
        };

    protected override IEnumerable<CSharpParameter> GetParameters(StreamingEndpoint endpoint)
    {
        foreach (var parameter in base.GetParameters(endpoint))
            yield return parameter;

        if (!string.IsNullOrEmpty(endpoint.Separator))
            yield return new CSharpParameter(CSharpIdentifier.String, "separator") {Value = endpoint.Separator};
    }
}
