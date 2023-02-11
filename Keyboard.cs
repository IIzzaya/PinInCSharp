using System;
using System.Collections.Generic;
using PinInCSharp.Elements;

namespace PinInCSharp {
  public class Keyboard {
    private static Dictionary<String, String> DAQIAN_KEYS = new() {
      {"", ""}, {"0", ""}, {"1", " "}, {"2", "6"}, {"3", "3"},
      {"4", "4"}, {"a", "8"}, {"ai", "9"}, {"an", "0"}, {"ang", ";"},
      {"ao", "l"}, {"b", "1"}, {"c", "h"}, {"ch", "t"}, {"d", "2"},
      {"e", "k"}, {"ei", "o"}, {"en", "p"}, {"eng", "/"}, {"er", "-"},
      {"f", "z"}, {"g", "e"}, {"h", "c"}, {"i", "u"}, {"ia", "u8"},
      {"ian", "u0"}, {"iang", "u;"}, {"iao", "ul"}, {"ie", "u,"}, {"in", "up"},
      {"ing", "u/"}, {"iong", "m/"}, {"iu", "u."}, {"j", "r"}, {"k", "d"},
      {"l", "x"}, {"m", "a"}, {"n", "s"}, {"o", "i"}, {"ong", "j/"},
      {"ou", "."}, {"p", "q"}, {"q", "f"}, {"r", "b"}, {"s", "n"},
      {"sh", "g"}, {"t", "w"}, {"u", "j"}, {"ua", "j8"}, {"uai", "j9"},
      {"uan", "j0"}, {"uang", "j;"}, {"uen", "mp"}, {"ueng", "j/"}, {"ui", "jo"},
      {"un", "jp"}, {"uo", "ji"}, {"v", "m"}, {"van", "m0"}, {"vang", "m;"},
      {"ve", "m,"}, {"vn", "mp"}, {"w", "j"}, {"x", "v"}, {"y", "u"},
      {"z", "y"}, {"zh", "5"},
    };

    private static Dictionary<String, String> XIAOHE_KEYS = new() {
      {"ai", "d"}, {"an", "j"}, {"ang", "h"}, {"ao", "c"}, {"ch", "i"},
      {"ei", "w"}, {"en", "f"}, {"eng", "g"}, {"ia", "x"}, {"ian", "m"},
      {"iang", "l"}, {"iao", "n"}, {"ie", "p"}, {"in", "b"}, {"ing", "k"},
      {"iong", "s"}, {"iu", "q"}, {"ong", "s"}, {"ou", "z"}, {"sh", "u"},
      {"ua", "x"}, {"uai", "k"}, {"uan", "r"}, {"uang", "l"}, {"ui", "v"},
      {"un", "y"}, {"uo", "o"}, {"ve", "t"}, {"ue", "t"}, {"vn", "y"},
      {"zh", "v"},
    };

    private static Dictionary<String, String> ZIRANMA_KEYS = new() {
      {"ai", "l"}, {"an", "j"}, {"ang", "h"}, {"ao", "k"}, {"ch", "i"},
      {"ei", "z"}, {"en", "f"}, {"eng", "g"}, {"ia", "w"}, {"ian", "m"},
      {"iang", "d"}, {"iao", "c"}, {"ie", "x"}, {"in", "n"}, {"ing", "y"},
      {"iong", "s"}, {"iu", "q"}, {"ong", "s"}, {"ou", "b"}, {"sh", "u"},
      {"ua", "w"}, {"uai", "y"}, {"uan", "r"}, {"uang", "d"}, {"ui", "v"},
      {"un", "p"}, {"uo", "o"}, {"ve", "t"}, {"ue", "t"}, {"vn", "p"},
      {"zh", "v"},
    };

    private static Dictionary<String, String> PHONETIC_LOCAL = new() {
      {"yi", "i"}, {"you", "iu"}, {"yin", "in"}, {"ye", "ie"}, {"ying", "ing"},
      {"wu", "u"}, {"wen", "un"}, {"yu", "v"}, {"yue", "ve"}, {"yuan", "van"},
      {"yun", "vn"}, {"ju", "jv"}, {"jue", "jve"}, {"juan", "jvan"}, {"jun", "jvn"},
      {"qu", "qv"}, {"que", "qve"}, {"quan", "qvan"}, {"qun", "qvn"}, {"xu", "xv"},
      {"xue", "xve"}, {"xuan", "xvan"}, {"xun", "xvn"}, {"shi", "sh"}, {"si", "s"},
      {"chi", "ch"}, {"ci", "c"}, {"zhi", "zh"}, {"zi", "z"}, {"ri", "r"},
    };

    private static Dictionary<String, String> SOUGOU_KEYS = new() {
      {"ai", "l"}, {"an", "j"}, {"ang", "h"}, {"ao", "k"}, {"ch", "i"},
      {"ei", "z"}, {"en", "f"}, {"eng", "g"}, {"ia", "w"}, {"ian", "m"},
      {"iang", "d"}, {"iao", "c"}, {"ie", "x"}, {"in", "n"}, {"ing", ";"},
      {"iong", "s"}, {"iu", "q"}, {"ong", "s"}, {"ou", "b"}, {"sh", "u"},
      {"ua", "w"}, {"uai", "y"}, {"uan", "r"}, {"uang", "d"}, {"ui", "v"},
      {"un", "p"}, {"uo", "o"}, {"ve", "t"}, {"ue", "t"}, {"v", "y"},
      {"zh", "v"}
    };

    private static Dictionary<String, String> ZHINENG_ABC_KEYS = new() {
      {"ai", "l"}, {"an", "j"}, {"ang", "h"}, {"ao", "k"}, {"ch", "e"},
      {"ei", "q"}, {"en", "f"}, {"eng", "g"}, {"er", "r"}, {"ia", "d"},
      {"ian", "w"}, {"iang", "t"}, {"iao", "z"}, {"ie", "x"}, {"in", "c"},
      {"ing", "y"}, {"iong", "s"}, {"iu", "r"}, {"ong", "s"}, {"ou", "b"},
      {"sh", "v"}, {"ua", "d"}, {"uai", "c"}, {"uan", "p"}, {"uang", "t"},
      {"ui", "m"}, {"un", "n"}, {"uo", "o"}, {"ve", "v"}, {"ue", "m"},
      {"zh", "a"},
    };

    private static Dictionary<String, String> GUOBIAO_KEYS = new() {
      {"ai", "k"}, {"an", "f"}, {"ang", "g"}, {"ao", "c"}, {"ch", "i"},
      {"ei", "b"}, {"en", "r"}, {"eng", "h"}, {"er", "l"}, {"ia", "q"},
      {"ian", "d"}, {"iang", "n"}, {"iao", "m"}, {"ie", "t"}, {"in", "l"},
      {"ing", "j"}, {"iong", "s"}, {"iu", "y"}, {"ong", "s"}, {"ou", "p"},
      {"sh", "u"}, {"ua", "q"}, {"uai", "y"}, {"uan", "w"}, {"uang", "n"},
      {"ui", "v"}, {"un", "z"}, {"uo", "o"}, {"van", "w"}, {"ve", "x"},
      {"vn", "z"}, {"zh", "v"},
    };

    private static Dictionary<String, String> MICROSOFT_KEYS = new() {
      {"ai", "l"}, {"an", "j"}, {"ang", "h"}, {"ao", "k"}, {"ch", "i"},
      {"ei", "z"}, {"en", "f"}, {"eng", "g"}, {"er", "r"}, {"ia", "w"},
      {"ian", "m"}, {"iang", "d"}, {"iao", "c"}, {"ie", "x"}, {"in", "n"},
      {"ing", ";"}, {"iong", "s"}, {"iu", "q"}, {"ong", "s"}, {"ou", "b"},
      {"sh", "u"}, {"ua", "w"}, {"uai", "y"}, {"uan", "r"}, {"uang", "d"},
      {"ui", "v"}, {"un", "p"}, {"uo", "o"}, {"ve", "v"}, {"ue", "t"},
      {"v", "y"}, {"zh", "v"}
    };

    private static Dictionary<String, String> PINYINPP_KEYS = new() {
      {"ai", "s"}, {"an", "f"}, {"ang", "g"}, {"ao", "d"}, {"ch", "u"},
      {"ei", "w"}, {"en", "r"}, {"eng", "t"}, {"er", "q"}, {"ia", "b"},
      {"ian", "j"}, {"iang", "h"}, {"iao", "k"}, {"ie", "m"}, {"in", "l"},
      {"ing", "q"}, {"iong", "y"}, {"iu", "n"}, {"ong", "y"}, {"ou", "p"},
      {"ua", "b"}, {"uai", "x"}, {"uan", "c"}, {"uang", "h"}, {"ue", "x"},
      {"ui", "v"}, {"un", "z"}, {"uo", "o"}, {"sh", "i"}, {"zh", "v"}
    };

    private static Dictionary<String, String> ZIGUANG_KEYS = new() {
      {"ai", "p"}, {"an", "r"}, {"ang", "s"}, {"ao", "q"}, {"ch", "a"},
      {"ei", "k"}, {"en", "w"}, {"eng", "t"}, {"er", "j"}, {"ia", "x"},
      {"ian", "f"}, {"iang", "g"}, {"iao", "b"}, {"ie", "d"}, {"in", "y"},
      {"ing", ";"}, {"iong", "h"}, {"iu", "j"}, {"ong", "h"}, {"ou", "z"},
      {"ua", "x"}, {"uan", "l"}, {"uai", "y"}, {"uang", "g"}, {"ue", "n"},
      {"un", "m"}, {"uo", "o"}, {"ve", "n"}, {"sh", "i"}, {"zh", "u"},
    };

    public static Keyboard QUANPIN = new Keyboard(null, null, Standard, false, true);
    public static Keyboard DAQIAN = new Keyboard(PHONETIC_LOCAL, DAQIAN_KEYS, Standard, false, false);
    public static Keyboard XIAOHE = new Keyboard(null, XIAOHE_KEYS, Zero, true, false);
    public static Keyboard ZIRANMA = new Keyboard(null, ZIRANMA_KEYS, Zero, true, false);
    public static Keyboard SOUGOU = new Keyboard(null, SOUGOU_KEYS, Zero, true, false);
    public static Keyboard GUOBIAO = new Keyboard(null, GUOBIAO_KEYS, Zero, true, false);
    public static Keyboard MICROSOFT = new Keyboard(null, MICROSOFT_KEYS, Zero, true, false);
    public static Keyboard PINYINPP = new Keyboard(null, PINYINPP_KEYS, Zero, true, false);
    public static Keyboard ZIGUANG = new Keyboard(null, ZIGUANG_KEYS, Zero, true, false);

    public readonly Dictionary<String, String> local;
    public readonly Dictionary<String, String> keys;
    public readonly Func<String, ICollection<String>> cutter;
    public readonly bool duo;
    public readonly bool sequence;

    public Keyboard(Dictionary<String, String> local, Dictionary<String, String> keys,
      Func<String, ICollection<String>> cutter, bool duo, bool sequence) {
      this.local = local;
      this.keys = keys;
      this.cutter = cutter;
      this.duo = duo;
      this.sequence = sequence;
    }

    public static List<String> Standard(String s) {
      List<String> ret = new();
      int cursor = 0;

      // initial
      if (Pinyin.HasInitial(s)) {
        cursor = s.Length > 2 && s[1] == 'h' ? 2 : 1;
        ret.Add(s.SubstringWithIndex(0, cursor));
      }

      // final
      if (s.Length != cursor + 1) {
        ret.Add(s.SubstringWithIndex(cursor, s.Length - 1));
      }

      // tone
      ret.Add(s.SubstringWithIndex(s.Length - 1));

      return ret;
    }

    public String GetKeys(String s) {
      if (keys != null && keys.ContainsKey(s)) {
        return keys[s];
      }
      return s;
    }

    public static List<String> Zero(String s) {
      List<String> ss = Standard(s);
      if (ss.Count == 2) {
        String finale = ss[0];
        ss[0] = char.ToString(finale[0]);
        ss.Insert(1, finale.Length == 2 ? char.ToString(finale[1]) : finale);
      }
      return ss;
    }

    public ICollection<String> Split(String s) {
      if (local != null) {
        String cut = s.SubstringWithIndex(0, s.Length - 1);
        String alt = local.GetValueOrDefault(cut, null);
        if (alt != null) s = alt + s[^1];
      }
      return cutter.Invoke(s);
    }
  }
}