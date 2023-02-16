using System;
using System.Collections.Generic;

namespace PinInCSharp.Utils {
    public class Cache<K, V> {
        private Dictionary<K, V> data = new();
        private Func<K, V> generator;

        public Cache(Func<K, V> generator) {
            this.generator = generator;
        }

        public V Get(K key) {
            data.TryGetValue(key, out var ret);
            if (ret == null) {
                ret = generator.Invoke(key);
                if (ret != null) data.Add(key, ret);
            }

            return ret;
        }

        public void Foreach(Action<K, V> c) {
            foreach (var item in data) {
                c.Invoke(item.Key, item.Value);
            }
        }

        public void Clear() {
            data.Clear();
        }
    }
}