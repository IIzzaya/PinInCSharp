using System;
using System.Collections.Generic;
using PinInCSharp.Utils;

namespace PinInCSharp.Searchers {
  public class SimpleSearcher<T> : ISearcher<T> {
    protected List<T> objs = new();
    protected readonly Accelerator acc;
    protected readonly Compressor strs = new Compressor();
    protected readonly PinIn m_context;
    protected readonly SearcherLogic logic;
    protected readonly PinIn.Ticket ticket;

    public SimpleSearcher(SearcherLogic logic, PinIn context) {
      this.m_context = context;
      this.logic = logic;
      acc = new Accelerator(context);
      acc.SetProvider(strs);
      ticket = context.NewTicket(Reset);
    }

    public virtual void Put(String name, T identifier) {
      strs.Put(name);
      for (int i = 0; i < name.Length; i++)
        m_context.GetChar(name[i]);
      objs.Add(identifier);
    }

    public virtual List<T> Search(String name) {
      List<T> ret = new();
      acc.Search(name);
      List<int> offsets = strs.offsets;
      for (int i = 0; i < offsets.Count; i++) {
        int s = offsets[i];
        if (logic.Test(acc, 0, s)) ret.Add(objs[i]);
      }

      return ret;
    }

    public PinIn GetContext() {
      return m_context;
    }

    public virtual void Reset() {
      acc.Reset();
    }
  }
}