using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using BLTools;

namespace BLRssLib {
  public class TItemCollection : ObservableCollection<TItem>, IToXml {
    #region Constructor(s)
    public TItemCollection() {
    }
    public TItemCollection(IEnumerable<XElement> items) {
      foreach (XElement ItemItem in items) {
        this.Add(new TItem(ItemItem));
      }
    }
    public TItemCollection(XElement items) {
      foreach (XElement ItemItem in items.Elements("item")) {
        this.Add(new TItem(ItemItem));
      }
    }
    public TItemCollection(TItemCollection items) {
      foreach (TItem ItemItem in items) {
        this.Add(new TItem(ItemItem));
      }
    }
    #endregion Constructor(s)

    #region Public methods
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      foreach (TItem ItemItem in this) {
        RetVal.AppendLine(ItemItem.ToString());
      }
      return RetVal.ToString().Trim('\n');
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("items");
      foreach (TItem ItemItem in this) {
        RetVal.Add(ItemItem.ToXml());
      }
      return RetVal;
    }
    #endregion Public methods
  }
}
