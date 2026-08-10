using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.Jvm.Model;

namespace TypedRest.CodeGeneration.Jvm;

/// <summary>
/// Keeps track of the type names handed out during a generation run, so that no two generated types end up in the
/// same file.
/// </summary>
public class TypeNameRegistry
{
    private readonly NameRegistry<JvmIdentifier> _names = new(
        getKey: identifier => identifier.QualifiedName,
        withNumber: (identifier, number) => Renamed(identifier, identifier.Name + number));

    /// <summary>
    /// Registers a name for a type, appending a number if it is already taken.
    /// </summary>
    public JvmIdentifier Register(JvmIdentifier candidate)
        => _names.Register(candidate);

    /// <summary>
    /// Registers the first of the <paramref name="candidates"/> that is still free.
    /// Falls back to appending a number to the last candidate if all of them are taken.
    /// </summary>
    public JvmIdentifier Register(IEnumerable<JvmIdentifier> candidates)
        => _names.Register(candidates);

    private static JvmIdentifier Renamed(JvmIdentifier identifier, string name)
        => new(identifier.Package, name);
}
