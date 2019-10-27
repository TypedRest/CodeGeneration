using System.Collections.Generic;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Reactive;

namespace TypedRest.OpenApi.CSharp.Builders.Reactive
{
    /// <summary>
    /// Builds C# code snippets for <see cref="StreamingEndpoint"/>s.
    /// </summary>
    public class StreamingBuilder : BuilderBase<StreamingEndpoint>
    {
        protected override CSharpIdentifier GetImplementation(StreamingEndpoint endpoint, ITypeLookup typeLookup)
            => new CSharpIdentifier(Namespace.Name, "StreamingEndpoint")
            {
                TypeArguments = {typeLookup[endpoint.Schema]}
            };

        protected override IEnumerable<CSharpParameter> GetParameters(StreamingEndpoint endpoint)
        {
            foreach (var parameter in base.GetParameters(endpoint))
                yield return parameter;

            if (!string.IsNullOrEmpty(endpoint.Separator))
                yield return new CSharpParameter(CSharpIdentifier.String, "separator", endpoint.Separator);

            if (endpoint.BufferSize.HasValue)
                yield return new CSharpParameter(CSharpIdentifier.Int, "bufferSize", endpoint.BufferSize.Value);
        }
    }
}
