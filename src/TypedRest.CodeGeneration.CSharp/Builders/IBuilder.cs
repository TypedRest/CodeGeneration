using System.Collections.Generic;
using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Builders
{
    /// <summary>
    /// Builds C# code snippets for a specific type of <see cref="IEndpoint"/>.
    /// </summary>
    public interface IBuilder
    {
        (CSharpProperty property, IEnumerable<ICSharpType> types) Build(string key, IEndpoint endpoint, IEndpointGenerator generator);
    }

    /// <summary>
    /// Builds C# code snippets for <typeparamref name="TEndpoint"/>s.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
    public interface IBuilder<in TEndpoint> : IBuilder
        where TEndpoint : IEndpoint
    {
        (CSharpProperty property, IEnumerable<ICSharpType> types) Build(string key, TEndpoint endpoint, IEndpointGenerator generator);
    }
}
