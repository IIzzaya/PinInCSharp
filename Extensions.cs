using System;
using System.Collections.Generic;
using System.Text;

namespace PinInCSharp {
    public static class Extensions {
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