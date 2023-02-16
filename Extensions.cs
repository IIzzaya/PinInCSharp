using System;
using System.Collections.Generic;
using System.Text;

namespace PinInCSharp {
    public static class Extensions {
        public static V Compute<K, V>(this Dictionary<K, V> dict, K key, Func<K, V, V> func) {
            // if no func given, throw.
            if (func == null) throw new ArgumentNullException(nameof(func));
            // if no mapping, return null.
            if (!dict.TryGetValue(key, out var value)) return default;
            // get the new value from func.
            var result = func(key, value);
            if (result == null) {
                // if the mapping exists but func => null,
                // remove the mapping and return null.
                dict.Remove(key);
                return default;
            }

            // mapping exists and func returned a non-null value.
            // set and return the new value
            dict[key] = result;
            return result;
        }

        /// <summary>Perform an action on each item.</summary>
        /// <param name="source">The source.</param>
        /// <param name="action">The action to perform.</param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action) {
            foreach (T obj in source)
                action(obj);
            return source;
        }

        public static string SubstringWithIndex(this string str, int startIndex) {
            return str.Substring(startIndex);
        }

        public static string SubstringWithIndex(this string str, int startIndex, int endIndex) {
            return str.Substring(startIndex, endIndex - startIndex);
        }

        public static void AddRange(this HashSet<String> str, params string[] items) {
            if (items != null) {
                for (int i = 0; i < items.Length; i++) {
                    str.Add(items[i]);
                }
            }
        }

        public static void AppendSafely(this StringBuilder sb, string str, int startIndex, int count) {
            if (str.Length - startIndex <= count) {
                sb.Append(str, startIndex, str.Length - startIndex);
            }
            else {
                sb.Append(str, startIndex, count);
            }
        }
    }
}