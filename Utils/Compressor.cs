using System;
using System.Collections.Generic;

namespace PinInCSharp.Utils {
  public class Compressor : Accelerator.IProvider {
    private List<char> chars = new();
    private List<int> strs = new();

    public List<int> offsets => strs;

    public int Put(String s) {
      strs.Add(chars.Count);
      for (int i = 0; i < s.Length; i++) chars.Add(s[i]);
      chars.Add('\0');
      return strs[^1];
    }

    public bool End(int i) {
      return chars[i] == '\0';
    }

    public char Get(int i) {
      return chars[i];
    }
  }
}