using System;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints.Reactive;

namespace TypedRest.OpenApi.CSharp.Builders.Reactive
{
    /// <summary>
    /// Builds C# code snippets for <see cref="PollingEndpoint"/>s.
    /// </summary>
    public class PollingBuilder : BuilderBase<PollingEndpoint>
    {
        protected override CSharpIdentifier GetImplementationType(PollingEndpoint endpoint, INamingConvention naming)
            => new CSharpIdentifier(Namespace.Name, "PollingEndpoint")
            {
                TypeArguments = {naming.TypeFor(endpoint.Schema ?? throw new InvalidOperationException($"Missing schema for {endpoint}."))}
            };
    }
}
