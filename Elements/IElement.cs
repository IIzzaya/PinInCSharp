using PinInCSharp.Utils;

namespace PinInCSharp.Elements {
  public interface IElement {
    IndexSet Match(string str, int start, bool partial);
  }
}