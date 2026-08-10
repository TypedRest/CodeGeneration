using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// Decides what the types and members of a generated TypeScript client are called.
/// </summary>
public interface INamingStrategy
{
    /// <summary>
    /// The name of the getter exposing a child endpoint.
    /// </summary>
    string Property(string key);

    /// <summary>
    /// The name and module of a generated endpoint class.
    /// </summary>
    TsIdentifier EndpointType(string key, IEndpoint endpoint, string? prefix = null);

    /// <summary>
    /// The name and module of a generated DTO type.
    /// </summary>
    TsIdentifier DtoType(string key);

    /// <summary>
    /// The TypeScript type a schema maps to.
    /// </summary>
    TsIdentifier TypeFor(OpenApiSchema? schema);
}
