using System;
using System.Collections.Generic;
using System.Linq;
using PinInCSharp.Elements;
using PinInCSharp.Utils;

namespace PinInCSharp.Searchers {
    public class TreeSearcher<T> : ISearcher<T> {
        private INode<T> root = new NDense<T>();

        private List<T> objects = new();
        private List<NAcc<T>> naccs = new();
        private readonly Accelerator acc;
        private readonly Compressor strs = new Compressor();
        private readonly PinIn context;
        private readonly SearcherLogic logic;
        private readonly PinIn.Ticket ticket;
        private static readonly int THRESHOLD = 128;

        public TreeSearcher(SearcherLogic logic, PinIn context) {
            this.logic = logic;
            this.context = context;
            acc = new Accelerator(context);
            acc.SetProvider(strs);
            ticket = context.NewTicket(() => {
                naccs.ForEach(i => i.Reload(this));
                acc.Reset();
            });
        }

        public void Put(String name, T identifier) {
            ticket.Renew();
            int pos = strs.Put(name);
            int end = logic == SearcherLogic.CONTAIN ? name.Length : 1;
            for (int i = 0; i < end; i++)
                root = root.Put(this, pos + i, objects.Count);
            objects.Add(identifier);
        }

        public List<T> Search(String s) {
            ticket.Renew();
            acc.Search(s);
            ISet<int> ret = new SortedSet<int>();
            root.Get(this, ret, 0);
            return ret.Select(i => objects[i]).ToList();
        }

        public PinIn GetContext() {
            return context;
        }

        public void Refresh() {
            ticket.Renew();
        }

        public interface INode<T> {
            public void Get(TreeSearcher<T> p, ISet<int> ret, int offset);
            public void Get(TreeSearcher<T> p, ISet<int> ret);
            public INode<T> Put(TreeSearcher<T> p, int name, int identifier);
        }

        private class NSlice<T> : INode<T> {
            private INode<T> exit = new NMap<T>();
            private int start, end;

            public NSlice(int start, int end) {
                this.start = start;
                this.end = end;
            }

            public void Get(TreeSearcher<T> p, ISet<int> ret, int offset) {
                Get(p, ret, offset, 0);
            }

            public void Get(TreeSearcher<T> p, ISet<int> ret) {
                exit.Get(p, ret);
            }

            public INode<T> Put(TreeSearcher<T> p, int name, int identifier) {
                int length = end - start;
                int match = p.acc.Common(start, name, length);
                if (match >= length) exit = exit.Put(p, name + length, identifier);
                else {
                    Cut(p, start + match);
                    exit = exit.Put(p, name + match, identifier);
                }

                return start == end ? exit : this;
            }

            private void Cut(TreeSearcher<T> p, int offset) {
                NMap<T> insert = new();
                if (offset + 1 == end) insert.Put(p.strs.Get(offset), exit);
                else {
                    NSlice<T> half = new(offset + 1, end);
                    half.exit = exit;
                    insert.Put(p.strs.Get(offset), half);
                }

                exit = insert;
                end = offset;
            }

            private void Get(TreeSearcher<T> p, ISet<int> ret, int offset, int start) {
                if (this.start + start == end)
                    exit.Get(p, ret, offset);
                else if (offset == p.acc.Search().Length) {
                    if (p.logic != SearcherLogic.EQUAL) exit.Get(p, ret);
                }
                else {
                    char ch = p.strs.Get(this.start + start);
                    p.acc.Get(ch, offset).Foreach(i => Get(p, ret, offset + i, start + 1));
                }
            }
        }

        private class NDense<T> : INode<T> {
            // offset, object, offset, object
            private List<int> data = new();

            public void Get(TreeSearcher<T> p, ISet<int> ret, int offset) {
                bool full = p.logic == SearcherLogic.EQUAL;
                if (!full && p.acc.Search().Length == offset) Get(p, ret);
                else {
                    for (int i = 0; i < data.Count / 2; i++) {
                        int ch = data[i * 2];
                        if (full ? p.acc.Matches(offset, ch) : p.acc.Begins(offset, ch))
                            ret.Add(data[i * 2 + 1]);
                    }
                }
            }

            public void Get(TreeSearcher<T> p, ISet<int> ret) {
                for (int i = 0; i < data.Count / 2; i++)
                    ret.Add(data[i * 2 + 1]);
            }

            public INode<T> Put(TreeSearcher<T> p, int name, int identifier) {
                if (data.Count >= THRESHOLD) {
                    int pattern = data[0];
                    INode<T> ret = new NSlice<T>(pattern, pattern + Match(p));
                    for (int j = 0; j < data.Count / 2; j++)
                        ret.Put(p, data[j * 2], data[j * 2 + 1]);
                    ret.Put(p, name, identifier);
                    return ret;
                }
                data.Add(name);
                data.Add(identifier);
                return this;
            }

            private int Match(TreeSearcher<T> p) {
                for (int i = 0;; i++) {
                    char a = p.strs.Get(data[0] + i);
                    for (int j = 1; j < data.Count / 2; j++) {
                        char b = p.strs.Get(data[j * 2] + i);
                        if (a != b || a == '\0') return i;
                    }
                }
            }
        }

        private class NMap<T> : INode<T> {
            public Dictionary<char, INode<T>> children;
            public HashSet<int> leaves = new(1);

            public virtual void Get(TreeSearcher<T> p, ISet<int> ret, int offset) {
                if (p.acc.Search().Length == offset) {
                    if (p.logic == SearcherLogic.EQUAL) ret.UnionWith(leaves);
                    else Get(p, ret);
                }
                else if (children != null) {
                    children.ForEach(pair => p.acc.Get(pair.Key, offset)
                                              .Foreach(i => pair.Value.Get(p, ret, offset + i)));
                }
            }

            public virtual void Get(TreeSearcher<T> p, ISet<int> ret) {
                ret.UnionWith(leaves);
                if (children != null) children.ForEach(pair => pair.Value.Get(p, ret));
            }

            public virtual INode<T> Put(TreeSearcher<T> p, int name, int identifier) {
                if (p.strs.Get(name) == '\0') {
                    if (leaves.Count >= THRESHOLD && leaves is List<int>)
                        leaves = new HashSet<int>(leaves);
                    leaves.Add(identifier);
                }
                else {
                    Init();
                    char ch = p.strs.Get(name);
                    INode<T> sub = children[ch];
                    if (sub == null) {
                        sub = new NDense<T>();
                        Put(ch, sub);
                    }

                    sub = sub.Put(p, name + 1, identifier);
                    children[ch] = sub;
                }

                return !(this is NAcc<T>) && children != null && children.Count > 32 ? new NAcc<T>(p, this) : this;
            }

            public void Put(char ch, INode<T> n) {
                Init();
                if (children.Count >= THRESHOLD && children is Dictionary<char, INode<T>>)
                    children = new(children);
                children[ch] = n;
            }

            private void Init() {
                if (children == null)
                    children = new Dictionary<char, INode<T>>();
            }
        }

        private class NAcc<T> : NMap<T> {
            Dictionary<Phoneme, HashSet<char>> index = new();

            public NAcc(TreeSearcher<T> p, NMap<T> n) {
                children = n.children;
                leaves = n.leaves;
                Reload(p);
            }

            public override void Get(TreeSearcher<T> p, ISet<int> ret, int offset) {
                if (p.acc.Search().Length == offset) {
                    if (p.logic == SearcherLogic.EQUAL) ret.UnionWith(leaves);
                    else Get(p, ret);
                }
                else {
                    INode<T> n = children[p.acc.Search()[offset]];
                    if (n != null) n.Get(p, ret, offset + 1);
                    index.ForEach(pair => {
                        var k = pair.Key;
                        var v = pair.Value;
                        if (!k.Match(p.acc.Search(), offset, true).IsEmpty()) {
                            v.ForEach(i => p.acc.Get((char)i, offset)
                                            .Foreach(j => children[i].Get(p, ret, offset + j)));
                        }
                    });
                }
            }

            public override INode<T> Put(TreeSearcher<T> p, int name, int identifier) {
                base.Put(p, name, identifier);
                Index(p, p.strs.Get(name));
                return this;
            }

            public void Reload(TreeSearcher<T> p) {
                index.Clear();
                children.Keys.ForEach(i => Index(p, i));
            }

            private void Index(TreeSearcher<T> p, char c) {
                Elements.Char ch = p.GetContext().GetChar(c);
                foreach (Pinyin py in ch.GetPinyins()) {
                    index.Compute(py.phonemes[0], (j, cs) => {
                        if (cs == null) return new HashSet<char>();
                        if (cs is HashSet<char> && cs.Count >= THRESHOLD && !cs.Contains(c))
                            return new HashSet<char>(cs);
                        return cs;
                    }).Add(c);
                }
            }
        }
    }
}