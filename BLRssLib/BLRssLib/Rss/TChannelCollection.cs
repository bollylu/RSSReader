using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;
using System.Collections.ObjectModel;

namespace BLRssLib {
  public class TChannelCollection : ObservableCollection<TChannel>, IToXml {

    #region Constructor(s)
    //public TChannelCollection(string filename) {
    //  XDocument Channels = XDocument.Load(filename);
    //  foreach (XElement ChannelItem in Channels.Descendants("Channels")) {
    //    this.Add(new TChannel(ChannelItem));
    //  }
    //}

    public TChannelCollection(IEnumerable<XElement> channels) {
      foreach (XElement ChannelItem in channels) {
        this.Add(new TChannel(ChannelItem));
      }
    }
    public TChannelCollection(XElement channels) {
      if (channels.HasElements) {
        foreach (XElement ChannelItem in channels.Elements("channel")) {
          this.Add(new TChannel(ChannelItem));
        }
      }
    }
    public TChannelCollection(TChannelCollection channels) {
      foreach (TChannel ChannelItem in channels) {
        this.Add(new TChannel(ChannelItem));
      }
    }
    public TChannelCollection() { }
    #endregion Constructor(s)

    //public TChannel this[string key] {
    //  get {
    //    TChannel RetVal = this.Find(x => x.Title.ToLower() == key.ToLower());
    //    return RetVal;
    //  }
    //}

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      foreach (TChannel ChannelItem in this) {
        RetVal.AppendLine(ChannelItem.ToString());
      }
      return RetVal.ToString().Trim('\n');
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("channels");
      foreach (TChannel ChannelItem in this) {
        RetVal.Add(ChannelItem.ToXml());
      }
      return RetVal;
    }
    #endregion Converters
  }
}
