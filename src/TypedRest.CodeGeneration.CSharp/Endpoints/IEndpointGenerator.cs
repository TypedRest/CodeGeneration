using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

public interface IEndpointGenerator
{
    INamingStrategy Naming { get; }

    bool WithInterfaces { get; }

    (CSharpProperty property, IEnumerable<ICSharpType> types) Generate(string key, IEndpoint endpoint);

    /// <summary>
    /// Returns a disambiguating prefix for an endpoint with the given <paramref name="key"/>, or <c>null</c> if no prefix is needed.
    /// Currently returns the parent key when the same key produces a custom endpoint class in more than one place.
    /// </summary>
    string? GetCollisionPrefix(string key);

    /// <summary>
    /// Pushes a key onto the parent stack. Builders call this around recursion into child endpoints
    /// so that <see cref="GetCollisionPrefix"/> can return the current parent.
    /// </summary>
    void PushParent(string key);

    /// <summary>
    /// Pops a key from the parent stack.
    /// </summary>
    void PopParent();
}
