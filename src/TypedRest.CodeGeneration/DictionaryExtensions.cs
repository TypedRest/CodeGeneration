namespace TypedRest.CodeGeneration;

/// <summary>
/// Provides extension methods for <see cref="IDictionary{TKey,TValue}"/>s.
/// </summary>
public static class DictionaryExtensions
{
    extension<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(IDictionary<TTargetKey, TTargetValue> target)
        where TSourceKey : TTargetKey where TSourceValue : TTargetValue
    {
        /// <summary>
        /// Adds multiple elements to the dictionary.
        /// </summary>
        public void AddRange(IEnumerable<KeyValuePair<TSourceKey, TSourceValue>> source)
        {
            foreach (var pair in source)
                target.Add(pair.Key, pair.Value);
        }
    }
}
