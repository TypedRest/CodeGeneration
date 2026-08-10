namespace TypedRest.CodeGeneration.Generation;

/// <summary>
/// Keeps track of the names handed out during a generation run, so that no two generated types end up with the same name.
/// </summary>
/// <typeparam name="T">The type representing a name, e.g. a language-specific type identifier.</typeparam>
/// <param name="getKey">Maps a name to the string used to detect collisions.</param>
/// <param name="withNumber">Creates a copy of a name with a number appended to it.</param>
public class NameRegistry<T>(Func<T, string> getKey, Func<T, int, T> withNumber)
{
    private readonly HashSet<string> _used = new(StringComparer.Ordinal);

    /// <summary>
    /// Registers a name, appending a number if it is already taken.
    /// </summary>
    public T Register(T candidate)
        => Register([candidate]);

    /// <summary>
    /// Registers the first of the <paramref name="candidates"/> that is still free.
    /// Falls back to appending a number to the last candidate if all of them are taken.
    /// </summary>
    public T Register(IEnumerable<T> candidates)
    {
        bool any = false;
        T lastCandidate = default!;
        foreach (var candidate in candidates)
        {
            if (_used.Add(getKey(candidate))) return candidate;
            lastCandidate = candidate;
            any = true;
        }

        if (!any)
            throw new ArgumentException("Must provide at least one candidate name.", nameof(candidates));

        for (int number = 2;; number++)
        {
            var numbered = withNumber(lastCandidate, number);
            if (_used.Add(getKey(numbered))) return numbered;
        }
    }
}
