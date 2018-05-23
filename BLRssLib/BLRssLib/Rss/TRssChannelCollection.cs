using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TRssChannelCollection : List<TRssChannel>, IToXml {
    
    public TRssChannelCollection(string filename) {
      XDocument Channels = XDocument.Load(filename);
      foreach (XElement RssChannelItem in Channels.Descendants("Channels")) {
        this.Add(new TRssChannel(RssChannelItem));
      }
    }

    public TRssChannelCollection(IEnumerable<XElement> channels) {
      foreach (XElement RssChannelItem in channels) {
        this.Add(new TRssChannel(RssChannelItem));
      }
    }
    public TRssChannelCollection(TRssChannelCollection channels) {
      foreach (TRssChannel RssChannelItem in channels) {
        this.Add(new TRssChannel(RssChannelItem));
      }
    }
    public TRssChannelCollection() { }

    public TRssChannel this[string key] {
      get {
        TRssChannel RetVal = this.Find(x => x.Title.ToLower() == key.ToLower());
        return RetVal;
      }
    }

    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      foreach (TRssChannel ChannelItem in this) {
        RetVal.AppendLine(ChannelItem.ToString());
      }
      return RetVal.ToString().Trim('\n');
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("channels");
      foreach (TRssChannel ChannelItem in this) {
        RetVal.Add(ChannelItem.ToXml());
      }
      return RetVal;
    }
  }
}
