using System;
using System.Collections.Generic;
using PinInCSharp.Utils;

namespace PinInCSharp.Searchers {
    public interface ISearcher<T> {
        public void Put(String name, T identifier);
        public List<T> Search(String name);
        public PinIn GetContext();
    }

    public enum SearcherLogic {
        BEGIN,
        CONTAIN,
        EQUAL,
        E_NUM
    }

    public static class LogicEx {
        public static bool Test(this SearcherLogic l, Accelerator a, int offset, int start) {
            switch (l) {
                case SearcherLogic.BEGIN:
                    return a.Begins(offset, start);
                case SearcherLogic.CONTAIN:
                    return a.Contains(offset, start);
                case SearcherLogic.EQUAL:
                    return a.Matches(offset, start);
            }

            return false;
        }

        public static bool Test(this SearcherLogic l, PinIn p, String s1, String s2) {
            switch (l) {
                case SearcherLogic.BEGIN:
                    return p.Begins(s1, s2);
                case SearcherLogic.CONTAIN:
                    return p.Contains(s1, s2);
                case SearcherLogic.EQUAL:
                    return p.Matches(s1, s2);
            }

            return false;
        }

        public static bool Raw(this SearcherLogic l, String s1, String s2) {
            switch (l) {
                case SearcherLogic.BEGIN:
                    return s1.StartsWith(s2);
                case SearcherLogic.CONTAIN:
                    return s1.Contains(s2);
                case SearcherLogic.EQUAL:
                    return s1.Equals(s2);
            }

            return false;
        }
    }
}