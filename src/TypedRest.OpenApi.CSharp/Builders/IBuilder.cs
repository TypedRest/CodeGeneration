using JetBrains.Annotations;
using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Builds C# code snippets for a specific type of <see cref="IEndpoint"/>.
    /// </summary>
    public interface IBuilder
    {
        [NotNull]
        CSharpClassConstruction GetConstruction([NotNull] IEndpoint endpoint, [NotNull] ITypeLookup typeLookup);
    }

    /// <summary>
    /// Builds C# code snippets for <typeparamref name="TEndpoint"/>s.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
    public interface IBuilder<in TEndpoint> : IBuilder
        where TEndpoint : IEndpoint
    {
        [NotNull]
        CSharpClassConstruction GetConstruction([NotNull] TEndpoint endpoint, [NotNull] ITypeLookup typeLookup);
    }
}
