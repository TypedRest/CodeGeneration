using NanoByte.CodeGeneration;
using TypedRest.CodeGeneration.Generation;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Keeps track of the type names handed out during a generation run, so that no two generated types in the same
/// namespace end up with the same name.
/// </summary>
public class TypeNameRegistry
{
    private readonly NameRegistry<CSharpIdentifier> _names = new(
        getKey: identifier => $"{identifier.Namespace}.{identifier.Name}",
        withNumber: (identifier, number) => new CSharpIdentifier(identifier.Namespace, identifier.Name + number));

    /// <summary>
    /// Registers a name for a type, appending a number if it is already taken.
    /// </summary>
    public CSharpIdentifier Register(CSharpIdentifier candidate)
        => _names.Register(candidate);

    /// <summary>
    /// Registers the first of the <paramref name="candidates"/> that is still free.
    /// Falls back to appending a number to the last candidate if all of them are taken.
    /// </summary>
    public CSharpIdentifier Register(IEnumerable<CSharpIdentifier> candidates)
        => _names.Register(candidates);
}
