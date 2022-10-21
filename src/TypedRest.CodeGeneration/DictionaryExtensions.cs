using System.Collections.Generic;

namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="IDictionary{TKey,TValue}"/>s.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// Adds multiple elements to the dictionary.
    /// </summary>
    public static void AddRange<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(this IDictionary<TTargetKey, TTargetValue> target, IEnumerable<KeyValuePair<TSourceKey, TSourceValue>> source)
        where TSourceKey : TTargetKey
        where TSourceValue : TTargetValue
    {
        foreach (var pair in source)
            target.Add(pair.Key, pair.Value);
    }
}
