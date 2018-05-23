using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TRssItemCollection : List<TRssItem>, IToXml {
    public TRssItemCollection() {
    }
    public TRssItemCollection(IEnumerable<XElement> rssItems) {
      foreach (XElement RssItemItem in rssItems) {
        this.Add(new TRssItem(RssItemItem));
      }
    }
    public TRssItemCollection(TRssItemCollection rssItems) {
      foreach (TRssItem RssItemItem in rssItems) {
        this.Add(new TRssItem(RssItemItem));
      }
    }

    public XElement ToXml() {
      XElement RetVal = new XElement("rss");
      foreach (TRssItem RssItemItem in this) {
        RetVal.Add(RssItemItem.ToXml());
      }
      return RetVal;
    }
  }
}
