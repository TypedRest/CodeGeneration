using TypedRest.CodeGeneration.Endpoints;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm;

/// <summary>
/// Decides what the types and members of a generated JVM client are called.
/// </summary>
public interface INamingStrategy
{
    /// <summary>
    /// The name of the member exposing a child endpoint.
    /// </summary>
    string Property(string key);

    /// <summary>
    /// The name and package of a generated endpoint class.
    /// </summary>
    JvmIdentifier EndpointType(string key, IEndpoint endpoint, string? prefix = null);

    /// <summary>
    /// The name and package of a generated DTO type.
    /// </summary>
    JvmIdentifier DtoType(string key);

    /// <summary>
    /// The JVM type a schema maps to.
    /// </summary>
    JvmIdentifier TypeFor(OpenApiSchema? schema);
}
