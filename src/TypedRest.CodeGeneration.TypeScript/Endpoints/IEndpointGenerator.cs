using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript.Endpoints;

/// <summary>
/// Drives the generation of TypeScript code for a tree of <see cref="IEndpoint"/>s.
/// </summary>
public interface IEndpointGenerator
{
    /// <summary>
    /// Decides what the generated types and members are called.
    /// </summary>
    INamingStrategy Naming { get; }

    /// <summary>
    /// The modules of the TypedRest runtime library to import from.
    /// </summary>
    Modules Modules { get; }

    /// <summary>
    /// Collects messages about aspects of the document that TypeScript cannot express.
    /// </summary>
    IGenerationLog Log { get; }

    /// <summary>
    /// Generates the getter exposing an endpoint on its parent, plus any types needed for it.
    /// </summary>
    (TsGetter getter, IEnumerable<ITsType> types) Generate(string key, IEndpoint endpoint);

    /// <summary>
    /// Reserves a unique name for a generated endpoint class.
    /// </summary>
    TsIdentifier EndpointType(string key, IEndpoint endpoint);

    /// <summary>
    /// Records that generation has descended into the children of the endpoint with this key.
    /// </summary>
    void PushParent(string key);

    /// <summary>
    /// Records that generation has left the children of the most recently pushed endpoint.
    /// </summary>
    void PopParent();
}
