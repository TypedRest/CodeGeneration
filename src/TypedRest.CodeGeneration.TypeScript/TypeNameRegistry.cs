using TypedRest.CodeGeneration.Generation;
using TypedRest.CodeGeneration.TypeScript.Model;

namespace TypedRest.CodeGeneration.TypeScript;

/// <summary>
/// Keeps track of the type names handed out during a generation run, so that no two generated types end up in the
/// same file.
/// </summary>
public class TypeNameRegistry
{
    private readonly NameRegistry<TsIdentifier> _names = new(
        // Every generated type gets its own module, so the module specifier is the identity
        getKey: identifier => identifier.Module?.Specifier ?? identifier.Name,
        withNumber: (identifier, number) => Renamed(identifier, identifier.Name + number));

    /// <summary>
    /// Registers a name for a type, appending a number if it is already taken.
    /// </summary>
    public TsIdentifier Register(TsIdentifier candidate)
        => _names.Register(candidate);

    /// <summary>
    /// Registers the first of the <paramref name="candidates"/> that is still free.
    /// Falls back to appending a number to the last candidate if all of them are taken.
    /// </summary>
    public TsIdentifier Register(IEnumerable<TsIdentifier> candidates)
        => _names.Register(candidates);

    private static TsIdentifier Renamed(TsIdentifier identifier, string name)
        => new(identifier.Module?.WithName(name), name);
}
