using System.Diagnostics.Contracts;

namespace TypedRest.CodeGeneration;

public static class KeyValuePairExtensions
{
    /// <summary>
    /// Deconstructs a <see cref="KeyValuePair{TKey,TValue}"/> like a tuple.
    /// </summary>
    /// <example>
    /// foreach (var (key, value) in dictionary)
    /// {/*...*/}
    /// </example>
    [Pure]
    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}
