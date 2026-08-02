using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints.Reactive;

namespace TypedRest.CodeGeneration.CSharp.Endpoints.Reactive;

/// <summary>
/// Builds C# code snippets for <see cref="SseStreamingEndpoint"/>s.
/// </summary>
public class SseStreamingBuilder : BuilderBase<SseStreamingEndpoint>
{
    protected override CSharpIdentifier GetImplementationType(SseStreamingEndpoint endpoint, INamingStrategy naming)
        => new(Namespace.Name, "SseStreamingEndpoint")
        {
            TypeArguments = {naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
        };

    protected override IEnumerable<CSharpParameter> GetParameters(SseStreamingEndpoint endpoint, IEndpointGenerator generator)
    {
        foreach (var parameter in base.GetParameters(endpoint, generator))
            yield return parameter;

        if (!string.IsNullOrEmpty(endpoint.EventType))
            yield return new CSharpParameter(CSharpIdentifier.String, "eventType") {Value = endpoint.EventType};
    }
}
