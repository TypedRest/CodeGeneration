using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm.Endpoints;

/// <summary>
/// Builds the code for a specific kind of <see cref="IEndpoint"/>.
/// </summary>
public interface IBuilder
{
    /// <summary>
    /// Builds the member exposing <paramref name="endpoint"/> on its parent, plus any types needed for it.
    /// </summary>
    (JvmChildEndpoint child, IEnumerable<IJvmType> types) Build(string key, IEndpoint endpoint, IEndpointGenerator generator);
}

/// <summary>
/// Builds the code for <typeparamref name="TEndpoint"/>.
/// </summary>
/// <typeparam name="TEndpoint">The type of <see cref="IEndpoint"/> to generate code for.</typeparam>
public interface IBuilder<in TEndpoint> : IBuilder
    where TEndpoint : IEndpoint
{
    /// <summary>
    /// Builds the member exposing <paramref name="endpoint"/> on its parent, plus any types needed for it.
    /// </summary>
    (JvmChildEndpoint child, IEnumerable<IJvmType> types) Build(string key, TEndpoint endpoint, IEndpointGenerator generator);
}

/// <summary>
/// Drives the generation of code for a tree of <see cref="IEndpoint"/>s.
/// </summary>
public interface IEndpointGenerator
{
    /// <summary>
    /// Decides what the generated types and members are called.
    /// </summary>
    INamingStrategy Naming { get; }

    /// <summary>
    /// Collects messages about aspects of the document the target language cannot express.
    /// </summary>
    IGenerationLog Log { get; }

    /// <summary>
    /// Generates the member exposing an endpoint on its parent, plus any types needed for it.
    /// </summary>
    (JvmChildEndpoint child, IEnumerable<IJvmType> types) Generate(string key, IEndpoint endpoint);

    /// <summary>
    /// Reserves a unique name for a generated endpoint class.
    /// </summary>
    JvmIdentifier EndpointType(string key, IEndpoint endpoint);

    /// <summary>
    /// Records that generation has descended into the children of the endpoint with this key.
    /// </summary>
    void PushParent(string key);

    /// <summary>
    /// Records that generation has left the children of the most recently pushed endpoint.
    /// </summary>
    void PopParent();
}
