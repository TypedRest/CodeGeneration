using TypedRest.OpenApi.CSharp.Dom;
using TypedRest.OpenApi.Endpoints;

namespace TypedRest.OpenApi.CSharp.Builders
{
    /// <summary>
    /// Builds C# code snippets for a specific type of <see cref="IEndpoint"/>.
    /// </summary>
    public interface IBuilder
    {
        CSharpClassConstruction GetConstruction(IEndpoint endpoint, ITypeList typeList);

        CSharpIdentifier GetInterface(IEndpoint endpoint, ITypeList typeList);
    }

    /// <summary>
    /// Builds C# code snippets for <typeparamref name="TEndpoint"/>s.
    /// </summary>
    /// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
    public interface IBuilder<in TEndpoint> : IBuilder
        where TEndpoint : IEndpoint
    {
        CSharpClassConstruction GetConstruction(TEndpoint endpoint, ITypeList typeList);

        CSharpIdentifier GetInterface(TEndpoint endpoint, ITypeList typeList);
    }
}
