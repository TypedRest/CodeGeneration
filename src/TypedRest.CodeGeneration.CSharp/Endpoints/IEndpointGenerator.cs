using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Endpoints;

namespace TypedRest.CodeGeneration.CSharp.Endpoints;

public interface IEndpointGenerator
{
    INamingStrategy Naming { get; }

    bool WithInterfaces { get; }

    /// <summary>
    /// Controls whether the entry endpoint gets a generated constructor taking the base URI.
    /// Turn this off to supply the constructors yourself in a partial class, e.g. to pass an error handler or custom headers.
    /// </summary>
    bool GenerateEntryConstructor { get; }

    (CSharpProperty property, IEnumerable<ICSharpType> types) Generate(string key, IEndpoint endpoint);

    /// <summary>
    /// Returns the name to use for the class generated for an endpoint with the given <paramref name="key"/>.
    /// Endpoints that share a key are disambiguated with a prefix derived from their parents, or failing that a number.
    /// </summary>
    /// <remarks>Every call hands out a new name, so this must be called exactly once per generated class.</remarks>
    CSharpIdentifier EndpointType(string key, IEndpoint endpoint);

    /// <summary>
    /// Pushes a key onto the parent stack. Builders call this around recursion into child endpoints
    /// so that <see cref="EndpointType"/> can derive prefixes from the current parents.
    /// </summary>
    void PushParent(string key);

    /// <summary>
    /// Pops a key from the parent stack.
    /// </summary>
    void PopParent();
}
