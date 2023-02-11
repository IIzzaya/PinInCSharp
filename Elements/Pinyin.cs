using System;
using System.Collections.Generic;
using System.Linq;
using PinInCSharp.Utils;

namespace PinInCSharp.Elements {
  public class Pinyin : IElement {
    private bool duo = false;
    private bool sequence = false;
    public readonly int id;
    private String raw;

    public Phoneme[] phonemes;

    public Pinyin(String str, PinIn p, int id) {
      raw = str;
      this.id = id;
      Reload(str, p);
    }

    public IndexSet Match(String str, int start, bool partial) {
      IndexSet ret;
      if (duo) {
        // in shuangpin we require initial and final both present,
        // the phoneme, which is tone here, is optional
        ret = IndexSet.ZERO;
        ret = phonemes[0].Match(str, ret, start, partial);
        ret = phonemes[1].Match(str, ret, start, partial);
        ret.Merge(phonemes[2].Match(str, ret, start, partial));
      }
      else {
        // in other keyboards, match of precedent phoneme
        // is compulsory to match subsequent phonemes
        // for example, zhong1, z+h+ong+1 cannot match zong or zh1
        IndexSet active = IndexSet.ZERO;
        ret = new IndexSet();
        foreach (Phoneme phoneme in phonemes) {
          active = phoneme.Match(str, active, start, partial);
          if (active.IsEmpty()) break;
          ret.Merge(active);
        }
      }

      if (sequence && phonemes[0].MatchSequence(str[start])) {
        ret.Set(1);
      }

      return ret;
    }

    public override String ToString() {
      return raw;
    }

    public void Reload(String str, PinIn p) {
      ICollection<String> split = p.keyboard.Split(str);
      List<Phoneme> l = new();
      foreach (String s in split) {
        l.Add(p.GetPhoneme(s));
      }

      phonemes = l.ToArray();

      duo = p.keyboard.duo;
      sequence = p.keyboard.sequence;
    }

    private static char[] initialArray = new[] {'a', 'e', 'i', 'o', 'u', 'v'};

    public static bool HasInitial(String s) {
      return !initialArray.Contains(s[0]);
    }
  }
}