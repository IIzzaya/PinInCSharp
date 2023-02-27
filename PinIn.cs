using System;
using System.Collections.Generic;
using System.Threading;
using PinInCSharp.Elements;
using PinInCSharp.Utils;

namespace PinInCSharp {
    public class PinIn {
        private int total = 0;

        private Cache<String, Phoneme> m_phonemes = null;
        private Cache<String, Phoneme> phonemes {
            get {
                if (m_phonemes == null) {
                    m_phonemes = new(s => new Phoneme(s, this));
                }
                return m_phonemes;
            }
        }

        private Cache<String, Pinyin> m_pinyins;
        private Cache<String, Pinyin> pinyins {
            get {
                if (m_pinyins == null) {
                    m_pinyins = new(s => new Pinyin(s, this, total++));
                }
                return m_pinyins;
            }
        }

        private Elements.Char[] chars = new Elements.Char[char.MaxValue];
        private Elements.Char.Dummy temp = new Elements.Char.Dummy();
        private readonly Accelerator acc;

        public Keyboard keyboard = Keyboard.QUANPIN;
        public int modification = 0;
        public bool fZh2Z = false;
        public bool fSh2S = false;
        public bool fCh2C = false;
        public bool fAng2An = false;
        public bool fIng2In = false;
        public bool fEng2En = false;
        public bool fU2V = false;
        public bool accelerate = false;

        private PinyinFormat m_format = PinyinFormat.NUMBER;
        public PinyinFormat format {
            get { return m_format; }
            private set { m_format = value; }
        }

        /// <summary>
        /// Use PinIn object to manage the context
        /// To configure it, use {@link #config()}
        /// <p/>
        /// The data must be "你: ni3\n我: wo3\n他: ta1" format
        /// </summary>
        public PinIn(string data) {
            if (string.IsNullOrEmpty(data)) {
                return;
            }
            acc = new Accelerator(this);
            var dataLineList = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in dataLineList) {
                var pair = item.TrimEnd('\r').Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (pair.Length != 2 && pair[0].Length != 1) {
                    continue;
                }
                var c = pair[0][0];
                var ss = pair[1];
                if (ss == null) {
                    chars[c] = null;
                }
                else {
                    var py = ss.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    Pinyin[] pinyins = new Pinyin[py.Length];

                    for (int i = 0; i < py.Length; i++) {
                        pinyins[i] = GetPinyin(py[i].TrimStart(' ').TrimEnd(' '));
                    }

                    chars[c] = new Elements.Char(c, pinyins);
                }
            }
        }

        public bool Contains(String s1, String s2) {
            if (accelerate) {
                Accelerator a = acc;
                a.SetProvider(s1);
                a.Search(s2);
                return a.Contains(0, 0);
            }
            return Matcher.Contains(s1, s2, this);
        }

        public bool Begins(String s1, String s2) {
            if (accelerate) {
                Accelerator a = acc;
                a.SetProvider(s1);
                a.Search(s2);
                return a.Begins(0, 0);
            }
            return Matcher.Begins(s1, s2, this);
        }

        public bool Matches(String s1, String s2) {
            if (accelerate) {
                Accelerator a = acc;
                a.SetProvider(s1);
                a.Search(s2);
                return a.Matches(0, 0);
            }
            return Matcher.Matches(s1, s2, this);
        }

        public Phoneme GetPhoneme(String s) {
            return phonemes.Get(s);
        }

        public Pinyin GetPinyin(String s) {
            return pinyins.Get(s);
        }

        public Elements.Char GetChar(char c) {
            Elements.Char ret = chars[c];
            if (ret != null) {
                return ret;
            }
            temp.Set(c);
            return temp;
        }

        public String Format(Pinyin p) {
            return format.Format(p);
        }

        public bool TryGetPinyinStringOfChar(char c, ref List<String> result) {
            var character = this.GetChar(c);
            var characterPinyins = character.GetPinyins();
            if (characterPinyins == null || characterPinyins.Length == 0) {
                return false;
            }
            result.Clear();
            foreach (var pinyin in characterPinyins) {
                result.Add(Format(pinyin));
            }
            return true;
        }

        /// <summary>
        /// Set values in returned {@link Config} object,
        /// then use {@link Config#commit()} to apply
        /// </summary>
        public Config NewConfig() {
            return new Config(this);
        }

        public Ticket NewTicket(ThreadStart r) {
            return new Ticket(this, r);
        }

        private void SetConfig(Config c) {
            format = c.format;

            if (fAng2An == c.fAng2An && fEng2En == c.fEng2En && fIng2In == c.fIng2In
                && fZh2Z == c.fZh2Z && fSh2S == c.fSh2S && fCh2C == c.fCh2C
                && keyboard == c.keyboard && fU2V == c.fU2V && accelerate == c.accelerate) return;

            keyboard = c.keyboard;
            fZh2Z = c.fZh2Z;
            fSh2S = c.fSh2S;
            fCh2C = c.fCh2C;
            fAng2An = c.fAng2An;
            fIng2In = c.fIng2In;
            fEng2En = c.fEng2En;
            fU2V = c.fU2V;
            accelerate = c.accelerate;
            phonemes.Foreach((s, p) => p.Reload(s, this));
            pinyins.Foreach((s, p) => p.Reload(s, this));
            modification++;
        }

        public static class Matcher {
            public static bool Begins(String s1, String s2, PinIn p) {
                if (string.IsNullOrEmpty(s1)) return s1.StartsWith(s2);
                return Check(s1, 0, s2, 0, p, true);
            }

            public static bool Contains(String s1, String s2, PinIn p) {
                if (string.IsNullOrEmpty(s1)) return s1.Contains(s2);
                for (int i = 0; i < s1.Length; i++)
                    if (Check(s1, i, s2, 0, p, true))
                        return true;
                return false;
            }

            public static bool Matches(String s1, String s2, PinIn p) {
                if (string.IsNullOrEmpty(s1)) return s1.Equals(s2);
                return Check(s1, 0, s2, 0, p, false);
            }

            private static bool Check(String s1, int start1, String s2, int start2, PinIn p, bool partial) {
                if (start2 == s2.Length) return partial || start1 == s1.Length;

                IElement r = p.GetChar(s1[start1]);
                IndexSet s = r.Match(s2, start2, partial);

                if (start1 == s1.Length - 1) {
                    int i = s2.Length - start2;
                    return s.Get(i);
                }
                return s.Traverse(i => Check(s1, start1 + 1, s2, start2 + i, p, partial));
            }
        }

        public class Ticket {
            public int modification;
            public ThreadStart runnable;

            private PinIn m_pinIn;

            internal Ticket(PinIn pinIn, ThreadStart r) {
                m_pinIn = pinIn;
                runnable = r;
                modification = m_pinIn.modification;
            }

            public void Renew() {
                int i = m_pinIn.modification;
                if (modification != i) {
                    modification = i;
                    runnable.Invoke();
                }
            }
        }

        public class Config {
            public Keyboard keyboard;
            public bool fZh2Z;
            public bool fSh2S;
            public bool fCh2C;
            public bool fAng2An;
            public bool fIng2In;
            public bool fEng2En;
            public bool fU2V;
            public bool accelerate;
            public PinyinFormat format;

            private PinIn m_pinIn;

            internal Config(PinIn pinIn) {
                m_pinIn = pinIn;
                keyboard = pinIn.keyboard;
                fZh2Z = pinIn.fZh2Z;
                fSh2S = pinIn.fSh2S;
                fCh2C = pinIn.fCh2C;
                fAng2An = pinIn.fAng2An;
                fIng2In = pinIn.fIng2In;
                fEng2En = pinIn.fEng2En;
                fU2V = pinIn.fU2V;
                accelerate = pinIn.accelerate;
                format = PinyinFormat.NUMBER;
            }

            public Config SetKeyboard(Keyboard value) {
                this.keyboard = value;
                return this;
            }

            public Config SetFZh2Z(bool value) {
                this.fZh2Z = value;
                return this;
            }

            public Config SetFSh2S(bool value) {
                this.fSh2S = value;
                return this;
            }

            public Config SetFCh2C(bool value) {
                this.fCh2C = value;
                return this;
            }

            public Config SetFAng2An(bool value) {
                this.fAng2An = value;
                return this;
            }

            public Config SetFIng2In(bool value) {
                this.fIng2In = value;
                return this;
            }

            public Config SetFEng2En(bool value) {
                this.fEng2En = value;
                return this;
            }

            public Config SetFU2V(bool value) {
                this.fU2V = value;
                return this;
            }

            public Config SetFormat(PinyinFormat value) {
                this.format = value;
                return this;
            }

            /// <summary>
            /// Set accelerate mode of immediate matching.
            /// When working in accelerate mode, accelerator will be used.
            /// <p/>
            /// When calling immediate matching functions continuously with
            /// different {@code s1} but same {@code s2}, for, say, 100 times,
            /// they are considered stable calls.
            /// <p/>
            /// If the scenario uses mainly stable calls and most of {@code s1}
            /// contains Chinese characters, using accelerate mode provides
            /// significant speed up. Otherwise, overhead of cache management
            /// in accelerator will slow down the matching process.
            /// <p/>
            /// Accelerate mode is disabled by default for consistency in
            /// different scenarios.
            /// </summary>
            public Config SetAccelerate(bool value) {
                this.accelerate = value;
                return this;
            }

            public PinIn Commit() {
                m_pinIn.SetConfig(this);
                return m_pinIn;
            }
        }
    }
}