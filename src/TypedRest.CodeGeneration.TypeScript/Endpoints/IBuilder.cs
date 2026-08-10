using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints;

/// <summary>
/// Builds TypeScript code for a specific kind of <see cref="IEndpoint"/>.
/// </summary>
public interface IBuilder
{
    /// <summary>
    /// Builds the getter exposing <paramref name="endpoint"/> on its parent, plus any types needed for it.
    /// </summary>
    (TsGetter getter, IEnumerable<ITsType> types) Build(string key, IEndpoint endpoint, IEndpointGenerator generator);
}

/// <summary>
/// Builds TypeScript code for <typeparamref name="TEndpoint"/>.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
public interface IBuilder<in TEndpoint> : IBuilder
    where TEndpoint : IEndpoint
{
    /// <summary>
    /// Builds the getter exposing <paramref name="endpoint"/> on its parent, plus any types needed for it.
    /// </summary>
    (TsGetter getter, IEnumerable<ITsType> types) Build(string key, TEndpoint endpoint, IEndpointGenerator generator);
}
