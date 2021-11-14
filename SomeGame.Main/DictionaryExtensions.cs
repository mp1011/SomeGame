using System.Collections.Generic;

namespace SomeGame.Main.Extensions
{
    public static class DictionaryExtensions
    {
        public static V GetValueOrDefault<K,V>(this Dictionary<K,V> d, K key)
        {
            V v;
            if (d.TryGetValue(key, out v))
                return v;
            else
                return default;
        }
    }
}
