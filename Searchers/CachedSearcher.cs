using System;
using System.Collections.Generic;
using System.Linq;

namespace PinInCSharp.Searchers {
    public class CachedSearcher<T> : SimpleSearcher<T> {
        private List<int> all = new();
        private float scale;
        private int lenCached = 0; // longest string with cached result
        private int maxCached = 0; // maximum amount of cached results
        private int total = 0; // total characters of all strings
        private Stats<String> stats = new();

        private Dictionary<String, List<int>> cache = new();

        public CachedSearcher(SearcherLogic logic, PinIn context, float scale = 1f) : base(logic, context) {
            this.scale = scale;
        }

        public override void Put(String name, T identifier) {
            Reset();
            for (int i = 0; i < name.Length; i++)
                context.GetChar(name[i]);
            total += name.Length;
            all.Add(all.Count);
            lenCached = 0;
            maxCached = 0;
            base.Put(name, identifier);
        }

        public override List<T> Search(String name) {
            ticket.Renew();
            if (all.Count == 0) return new();

            if (maxCached == 0) {
                float totalSearch = logic == SearcherLogic.CONTAIN ? total : all.Count;
                maxCached = (int)(scale * Math.Ceiling(2 * Math.Log(totalSearch) / Math.Log(2) + 16));
            }

            if (lenCached == 0) lenCached = (int)Math.Ceiling(Math.Log(maxCached) / Math.Log(8));

            return Test(name).Select(i => objs[i]).ToList();
        }

        public override void Reset() {
            base.Reset();
            stats.Reset();
            lenCached = 0;
            maxCached = 0;
        }

        private List<int> Filter(String name) {
            List<int> ret;
            if (string.IsNullOrEmpty(name)) return all;

            ret = cache[name];
            stats.Count(name);

            if (ret == null) {
                List<int> @base = Filter(name.SubstringWithIndex(0, name.Length - 1));
                if (cache.Count >= maxCached) {
                    String least = stats.Least(cache.Keys, name);
                    if (!least.Equals(name)) cache.Remove(least);
                    else return @base;
                }

                acc.Search(name);
                List<int> tmp = new();
                SearcherLogic filter = logic == SearcherLogic.EQUAL ? SearcherLogic.BEGIN : logic;
                foreach (int i in @base) {
                    if (filter.Test(acc, 0, strs.offsets[i])) tmp.Add(i);
                }

                if (tmp.Count == @base.Count) {
                    ret = @base;
                }
                else {
                    tmp.TrimExcess();
                    ret = tmp;
                }

                cache[name] = ret;
            }

            return ret;
        }

        private List<int> Test(String name) {
            List<int> intList = Filter(name.SubstringWithIndex(0, Math.Min(name.Length, lenCached)));
            if (logic == SearcherLogic.EQUAL || name.Length > lenCached) {
                List<int> ret = new();
                acc.Search(name);
                foreach (int i in intList) {
                    if (logic.Test(acc, 0, strs.offsets[i])) ret.Add(i);
                }

                return ret;
            }

            return intList;
        }

        private class Stats<T> {
            Dictionary<T, int> data = new();

            public void Count(T key) {
                int cnt = data[key] + 1;
                data[key] = cnt;
                if (cnt == int.MaxValue) {
                    foreach (var (k, v) in data) {
                        data[k] = v / 2;
                    }
                }
            }

            public T Least(ICollection<T> keys, T extra) {
                T ret = extra;
                int cnt = data[extra];
                foreach (T i in keys) {
                    int value = data[i];
                    if (value < cnt) {
                        ret = i;
                        cnt = value;
                    }
                }

                return ret;
            }

            public void Reset() {
                data.Clear();
            }
        }
    }
}