using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BLRssLib {
  public class TOpmlOutlineCollection : List<TOpmlOutline> {
    #region Constructor(s)
    public TOpmlOutlineCollection() { }
    public TOpmlOutlineCollection(IEnumerable<XElement> opmlOutlines) {
      foreach (XElement OpmlOutlineItem in opmlOutlines) {
        this.Add(new TOpmlOutline(OpmlOutlineItem));
      }
    }
    public TOpmlOutlineCollection(TOpmlOutlineCollection opmlOutlines) {
      foreach (TOpmlOutline OpmlOutlineItem in opmlOutlines) {
        this.Add(new TOpmlOutline(OpmlOutlineItem));
      }
    }
    #endregion Constructor(s)

    public override string ToString() {
      return ToString(false);
    }
    public string ToString(bool recurse) {
      StringBuilder RetVal = new StringBuilder();
      foreach (TOpmlOutline OutlineItem in this) {
        RetVal.AppendLine(OutlineItem.ToString());
        if (recurse && OutlineItem.Outlines.Count>0) {
          OutlineItem.Outlines.ToString(recurse).Trim('\n', '\r').Split('\n').ToList().ForEach(l => RetVal.AppendFormat("  {0}\n", l));
        }
      }
      return RetVal.ToString();
    }
  }
}
