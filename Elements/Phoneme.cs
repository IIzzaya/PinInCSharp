using System;
using System.Collections.Generic;
using System.Linq;
using PinInCSharp.Utils;

namespace PinInCSharp.Elements {
  public class Phoneme : IElement {
    private String[] strs;

    public override String ToString() {
      return strs[0];
    }

    public Phoneme(String str, PinIn p) {
      Reload(str, p);
    }

    public IndexSet Match(String source, IndexSet idx, int start, bool partial) {
      if (strs.Length == 1 && string.IsNullOrEmpty(strs[0])) return new IndexSet(idx);
      IndexSet ret = new IndexSet();
      idx.Foreach(i => {
        IndexSet indexSet = Match(source, start + i, partial);
        indexSet.Offset(i);
        ret.Merge(indexSet);
      });
      return ret;
    }

    public bool MatchSequence(char c) {
      foreach (String str in strs) {
        if (str[0] == c) {
          return true;
        }
      }

      return false;
    }

    public bool IsEmpty() {
      return strs.Length == 1 && string.IsNullOrEmpty(strs[0]);
    }

    static int StrCmp(String a, String b, int aStart) {
      int len = Math.Min(a.Length - aStart, b.Length);
      for (int i = 0; i < len; i++)
        if (a[i + aStart] != b[i])
          return i;
      return len;
    }


    public IndexSet Match(String source, int start, bool partial) {
      IndexSet ret = new IndexSet();
      if (strs.Length == 1 && string.IsNullOrEmpty(strs[0])) return ret;
      foreach (String str in strs) {
        int size = StrCmp(source, str, start);
        if (partial && start + size == source.Length) ret.Set(size); // ending match
        else if (size == str.Length) ret.Set(size); // full match
      }

      return ret;
    }

    public void Reload(String str, PinIn p) {
      HashSet<String> ret = new();
      ret.Add(str);

      if (p.fCh2C && str.StartsWith("c")) ret.AddRange("c", "ch");
      if (p.fSh2S && str.StartsWith("s")) ret.AddRange("s", "sh");
      if (p.fZh2Z && str.StartsWith("z")) ret.AddRange("z", "zh");
      if (p.fU2V && str.StartsWith("v")) ret.Add("u" + str.SubstringWithIndex(1));
      if ((p.fAng2An && str.EndsWith("ang")) ||
          (p.fEng2En && str.EndsWith("eng")) ||
          (p.fIng2In && str.EndsWith("ing")))
        ret.Add(str.SubstringWithIndex(0, str.Length - 1));
      if ((p.fAng2An && str.EndsWith("an")) ||
          (p.fEng2En && str.EndsWith("en")) ||
          (p.fIng2In && str.EndsWith("in")))
        ret.Add(str + 'g');

      strs = ret.Select(p.keyboard.GetKeys).ToArray();
    }
  }
}