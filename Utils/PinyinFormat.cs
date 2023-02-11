using System;
using System.Collections.Generic;
using System.Text;
using PinInCSharp.Elements;

namespace PinInCSharp.Utils {
  public abstract class PinyinFormat {
    // finals with tones on the second character
    private static readonly HashSet<String> OFFSET = new() {
      "ui", "iu", "uan", "uang", "ian", "iang", "ua",
      "ie", "uo", "iong", "iao", "ve", "ia"
    };

    private static readonly Dictionary<char, char> NONE = new() {
      {'a', 'a'}, {'o', 'o'}, {'e', 'e'}, {'i', 'i'}, {'u', 'u'}, {'v', 'ü'}
    };

    private static readonly Dictionary<char, char> FIRST = new() {
      {'a', 'ā'}, {'o', 'ō'}, {'e', 'ē'}, {'i', 'ī'}, {'u', 'ū'}, {'v', 'ǖ'}
    };

    private static readonly Dictionary<char, char> SECOND = new() {
      {'a', 'á'}, {'o', 'ó'}, {'e', 'é'}, {'i', 'í'}, {'u', 'ú'}, {'v', 'ǘ'}
    };

    private static readonly Dictionary<char, char> THIRD = new() {
      {'a', 'ǎ'}, {'o', 'ǒ'}, {'e', 'ě'}, {'i', 'ǐ'}, {'u', 'ǔ'}, {'v', 'ǚ'}
    };

    private static readonly Dictionary<char, char> FOURTH = new() {
      {'a', 'à'}, {'o', 'ò'}, {'e', 'è'}, {'i', 'ì'}, {'u', 'ù'}, {'v', 'ǜ'}
    };

    private static readonly List<Dictionary<char, char>> TONES = new() {
      NONE, FIRST, SECOND, THIRD, FOURTH
    };

    private static readonly Dictionary<String, String> SYMBOLS = new() {
      {"a", "ㄚ"}, {"o", "ㄛ"}, {"e", "ㄜ"}, {"er", "ㄦ"}, {"ai", "ㄞ"},
      {"ei", "ㄟ"}, {"ao", "ㄠ"}, {"ou", "ㄡ"}, {"an", "ㄢ"}, {"en", "ㄣ"},
      {"ang", "ㄤ"}, {"eng", "ㄥ"}, {"ong", "ㄨㄥ"}, {"i", "ㄧ"}, {"ia", "ㄧㄚ"},
      {"iao", "ㄧㄠ"}, {"ie", "ㄧㄝ"}, {"iu", "ㄧㄡ"}, {"ian", "ㄧㄢ"}, {"in", "ㄧㄣ"},
      {"iang", "ㄧㄤ"}, {"ing", "ㄧㄥ"}, {"iong", "ㄩㄥ"}, {"u", "ㄨ"}, {"ua", "ㄨㄚ"},
      {"uo", "ㄨㄛ"}, {"uai", "ㄨㄞ"}, {"ui", "ㄨㄟ"}, {"uan", "ㄨㄢ"}, {"un", "ㄨㄣ"},
      {"uang", "ㄨㄤ"}, {"ueng", "ㄨㄥ"}, {"uen", "ㄩㄣ"}, {"v", "ㄩ"}, {"ve", "ㄩㄝ"},
      {"van", "ㄩㄢ"}, {"vang", "ㄩㄤ"}, {"vn", "ㄩㄣ"}, {"b", "ㄅ"}, {"p", "ㄆ"},
      {"m", "ㄇ"}, {"f", "ㄈ"}, {"d", "ㄉ"}, {"t", "ㄊ"}, {"n", "ㄋ"},
      {"l", "ㄌ"}, {"g", "ㄍ"}, {"k", "ㄎ"}, {"h", "ㄏ"}, {"j", "ㄐ"},
      {"q", "ㄑ"}, {"x", "ㄒ"}, {"zh", "ㄓ"}, {"ch", "ㄔ"}, {"sh", "ㄕ"},
      {"r", "ㄖ"}, {"z", "ㄗ"}, {"c", "ㄘ"}, {"s", "ㄙ"}, {"w", "ㄨ"},
      {"y", "ㄧ"}, {"1", ""}, {"2", "ˊ"}, {"3", "ˇ"}, {"4", "ˋ"},
      {"0", "˙"}, {"", ""}
    };

    private static readonly Dictionary<String, String> LOCAL = new() {
      {"yi", "i"}, {"you", "iu"}, {"yin", "in"}, {"ye", "ie"}, {"ying", "ing"},
      {"wu", "u"}, {"wen", "un"}, {"yu", "v"}, {"yue", "ve"}, {"yuan", "van"},
      {"yun", "vn"}, {"ju", "jv"}, {"jue", "jve"}, {"juan", "jvan"}, {"jun", "jvn"},
      {"qu", "qv"}, {"que", "qve"}, {"quan", "qvan"}, {"qun", "qvn"}, {"xu", "xv"},
      {"xue", "xve"}, {"xuan", "xvan"}, {"xun", "xvn"}, {"shi", "sh"}, {"si", "s"},
      {"chi", "ch"}, {"ci", "c"}, {"zhi", "zh"}, {"zi", "z"}, {"ri", "r"}
    };

    public static readonly PinyinFormat RAW = new PinyinFormatRaw();

    public class PinyinFormatRaw : PinyinFormat {
      public override String Format(Pinyin p) {
        return p.ToString().SubstringWithIndex(0, p.ToString().Length - 1);
      }
    }

    public static readonly PinyinFormat NUMBER = new PinyinFormatNumber();

    public class PinyinFormatNumber : PinyinFormat {
      public override String Format(Pinyin p) {
        return p.ToString();
      }
    }

    public static readonly PinyinFormat PHONETIC = new PinyinFormatPhonetic();


    public class PinyinFormatPhonetic : PinyinFormat {
      public override String Format(Pinyin p) {
        String s = p.ToString();
        String str = LOCAL.GetValueOrDefault(s.SubstringWithIndex(0, s.Length - 1), null);
        if (str != null) s = str + s[^1];
        StringBuilder sb = new StringBuilder();

        String[] split;
        int len = s.Length;
        if (!Pinyin.HasInitial(s)) {
          split = new String[] {
            "",
            s.SubstringWithIndex(0, len - 1),
            s.SubstringWithIndex(len - 1)
          };
        }
        else {
          int i = s.Length > 2 && s[1] == 'h' ? 2 : 1;
          split = new String[] {
            s.SubstringWithIndex(0, i),
            s.SubstringWithIndex(i, len - 1),
            s.SubstringWithIndex(len - 1)
          };
        }

        bool weak = split[2].Equals("0");
        if (weak) sb.Append(SYMBOLS[split[2]]);
        sb.Append(SYMBOLS[split[0]]);
        sb.Append(SYMBOLS[split[1]]);
        if (!weak) sb.Append(SYMBOLS[split[2]]);
        return sb.ToString();
      }
    }

    public static readonly PinyinFormat UNICODE = new PinyinFormatUnicode();

    public class PinyinFormatUnicode : PinyinFormat {
      public override String Format(Pinyin p) {
        StringBuilder sb = new StringBuilder();
        String s = p.ToString();
        String finale;
        int len = s.Length;

        if (!Pinyin.HasInitial(s)) {
          finale = s.SubstringWithIndex(0, len - 1);
        }
        else {
          int i = s.Length > 2 && s[1] == 'h' ? 2 : 1;
          sb.AppendSafely(s, 0, i);
          finale = s.SubstringWithIndex(i, len - 1);
        }

        int offset = OFFSET.Contains(finale) ? 1 : 0;
        if (offset == 1) sb.AppendSafely(finale, 0, 1);
        Dictionary<char, char> group = TONES[s[^1] - '0'];
        sb.Append(group[finale[offset]]);
        if (finale.Length > offset + 1) {
          sb.AppendSafely(finale, offset + 1, finale.Length);
        }
        return sb.ToString();
      }
    }

    public abstract String Format(Pinyin p);
  }
}