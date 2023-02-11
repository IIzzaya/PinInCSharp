using System;
using System.Text;

namespace PinInCSharp.Utils {
  public class IndexSet {
    public static readonly IndexSet ZERO = new IndexSet(0x1);
    public static readonly IndexSet ONE = new IndexSet(0x2);
    public static readonly IndexSet NONE = new IndexSet(0x0);

    public int value = 0x0;

    public IndexSet() { }

    public IndexSet(IndexSet set) {
      value = set.value;
    }

    public IndexSet(int value) {
      this.value = value;
    }

    public virtual void Set(int index) {
      int i = 0x1 << index;
      value |= i;
    }

    public virtual bool Get(int index) {
      int i = 0x1 << index;
      return (value & i) != 0;
    }

    public virtual void Merge(IndexSet s) {
      value = value == 0x1 ? s.value : (value | s.value);
    }

    public virtual bool Traverse(Func<int, bool> p) {
      int v = value;
      for (int i = 0; i < 7; i++) {
        if ((v & 0x1) == 0x1 && p.Invoke(i)) return true;
        else if (v == 0) return false;
        v >>= 1;
      }

      return false;
    }

    public void Foreach(Action<int> c) {
      int v = value;
      for (int i = 0; i < 7; i++) {
        if ((v & 0x1) == 0x1) c.Invoke(i);
        else if (v == 0) return;
        v >>= 1;
      }
    }

    public virtual void Offset(int i) {
      value <<= i;
    }

    public override string ToString() {
      StringBuilder builder = new StringBuilder();
      Foreach(i => {
        builder.Append(i);
        builder.Append(", ");
      });
      if (builder.Length != 0) {
        builder.Remove(builder.Length - 2, builder.Length);
        return builder.ToString();
      }
      else return "0";
    }

    public bool IsEmpty() {
      return value == 0x0;
    }

    public IndexSet Copy() {
      return new IndexSet(value);
    }

    private class Immutable : IndexSet {
      public override void Set(int index) {
        throw new SystemException("Immutable collection");
      }

      public override void Merge(IndexSet s) {
        throw new SystemException("Immutable collection");
      }

      public override void Offset(int i) {
        throw new SystemException("Immutable collection");
      }
    }

    public class Storage {
      private IndexSet tmp = new Immutable();
      private int[] data = new int[16];

      public void Set(IndexSet indexSet, int index) {
        if (index >= data.Length) {
          // here we get the smallest power of 2 that is larger than index
          int size = index;
          size |= size >> 1;
          size |= size >> 2;
          size |= size >> 4;
          size |= size >> 8;
          size |= size >> 16;
          int[] replace = new int[size + 1];
          System.Array.Copy(data, 0, replace, 0, data.Length);
          data = replace;
        }

        data[index] = indexSet.value + 1;
      }

      public IndexSet Get(int index) {
        if (index >= data.Length) return null;
        int ret = data[index];
        if (ret == 0) return null;
        tmp.value = ret - 1;
        return tmp;
      }
    }
  }
}