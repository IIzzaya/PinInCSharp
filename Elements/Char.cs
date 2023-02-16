using System;
using PinInCSharp.Utils;

namespace PinInCSharp.Elements {
    public class Char : IElement {
        private static readonly Pinyin[] NONE = new Pinyin[0];

        private char ch;
        private Pinyin[] pinyin;

        public Char(char ch, Pinyin[] pinyin) {
            this.ch = ch;
            this.pinyin = pinyin;
        }

        public IndexSet Match(String str, int start, bool partial) {
            IndexSet ret = (str[start] == ch ? IndexSet.ONE : IndexSet.NONE).Copy();
            foreach (IElement p in pinyin) ret.Merge(p.Match(str, start, partial));
            return ret;
        }

        public char GetChar() {
            return ch;
        }

        public Pinyin[] GetPinyins() {
            return pinyin;
        }

        public class Dummy : Char {
            public Dummy() : base('\0', NONE) { }
        }

        public void Set(char ch) {
            this.ch = ch;
        }
    }
}