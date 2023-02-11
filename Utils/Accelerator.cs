using System;
using System.Collections.Generic;
using PinInCSharp.Elements;

namespace PinInCSharp.Utils {
  public class Accelerator {
    readonly PinIn context;
    List<IndexSet.Storage> cache;
    char[] searchChars;
    String searchStr;
    IProvider provider;
    Str str = new Str();
    bool partial;

    public Accelerator(PinIn context) {
      this.context = context;
    }

    public void Search(String s) {
      if (!s.Equals(searchStr)) {
        // here we store both search token as string and char array
        // it seems stupid, but saves over 10% of accelerator overhead
        searchStr = s;
        searchChars = s.ToCharArray();
        Reset();
      }
    }

    public IndexSet Get(char ch, int offset) {
      Elements.Char c = context.GetChar(ch);
      IndexSet ret = (searchChars[offset] == c.GetChar() ? IndexSet.ONE : IndexSet.NONE).Copy();
      foreach (Pinyin p in c.GetPinyins()) ret.Merge(Get(p, offset));
      return ret;
    }

    public IndexSet Get(Pinyin p, int offset) {
      for (int i = cache.Count; i <= offset; i++)
        cache.Add(new IndexSet.Storage());
      IndexSet.Storage data = cache[offset];
      IndexSet ret = data.Get(p.id);
      if (ret == null) {
        ret = p.Match(searchStr, offset, partial);
        data.Set(ret, p.id);
      }

      return ret;
    }

    public void SetProvider(IProvider p) {
      provider = p;
    }

    public void SetProvider(String s) {
      str.s = s;
      provider = str;
    }

    public void Reset() {
      cache = new List<IndexSet.Storage>();
    }
    
    /// <summary>
    /// </summary>
    /// <param name="offset">offset in search string</param>
    /// <param name="start">start point in raw text</param>
    public bool Check(int offset, int start) {
      if (offset == searchStr.Length) return partial || provider.End(start);
      if (provider.End(start)) return false;

      IndexSet s = Get(provider.Get(start), offset);

      if (provider.End(start + 1)) {
        int i = searchStr.Length - offset;
        return s.Get(i);
      }
      else return s.Traverse(i => Check(offset + i, start + 1));
    }

    public bool Matches(int offset, int start) {
      if (partial) {
        partial = false;
        Reset();
      }

      return Check(offset, start);
    }

    public bool Begins(int offset, int start) {
      if (!partial) {
        partial = true;
        Reset();
      }

      return Check(offset, start);
    }

    public bool Contains(int offset, int start) {
      if (!partial) {
        partial = true;
        Reset();
      }

      for (int i = start; !provider.End(i); i++) {
        if (Check(offset, i)) return true;
      }

      return false;
    }

    public String Search() {
      return searchStr;
    }

    public int Common(int s1, int s2, int max) {
      for (int i = 0;; i++) {
        if (i >= max) return max;
        char a = provider.Get(s1 + i);
        char b = provider.Get(s2 + i);
        if (a != b || a == '\0') return i;
      }
    }

    public interface IProvider {
      bool End(int i);
      char Get(int i);
    }

    class Str : IProvider {
      public String s;

      public bool End(int i) {
        return i >= s.Length;
      }

      public char Get(int i) {
        return s[i];
      }
    }
  }
}