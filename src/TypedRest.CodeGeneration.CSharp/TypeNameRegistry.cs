using NanoByte.CodeGeneration;

namespace TypedRest.CodeGeneration.CSharp;

/// <summary>
/// Keeps track of the type names handed out during a generation run, so that no two generated types in the same
/// namespace end up with the same name.
/// </summary>
public class TypeNameRegistry
{
    private readonly HashSet<string> _used = new(StringComparer.Ordinal);

    /// <summary>
    /// Registers a name for a type, appending a number if it is already taken.
    /// </summary>
    public CSharpIdentifier Register(CSharpIdentifier candidate)
        => Register([candidate]);

    /// <summary>
    /// Registers the first of the <paramref name="candidates"/> that is still free.
    /// Falls back to appending a number to the last candidate if all of them are taken.
    /// </summary>
    public CSharpIdentifier Register(IEnumerable<CSharpIdentifier> candidates)
    {
        CSharpIdentifier? lastCandidate = null;
        foreach (var candidate in candidates)
        {
            if (_used.Add(Key(candidate))) return candidate;
            lastCandidate = candidate;
        }

        if (lastCandidate == null)
            throw new ArgumentException("Must provide at least one candidate name.", nameof(candidates));

        for (int number = 2;; number++)
        {
            var numbered = new CSharpIdentifier(lastCandidate.Namespace, lastCandidate.Name + number);
            if (_used.Add(Key(numbered))) return numbered;
        }
    }

    private static string Key(CSharpIdentifier identifier)
        => $"{identifier.Namespace}.{identifier.Name}";
}
