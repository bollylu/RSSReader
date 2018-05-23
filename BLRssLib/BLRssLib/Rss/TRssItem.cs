using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TRssItem : IToXml {
    public Uri Link { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime PubDate { get; set; }
    public string PubDateBE {
      get {
        return PubDate.ToString("dd/MM/yyyy HH:mm:ss");
      }
    }
    public TRssItemEnclosure Enclosure { get; set; }

    public TRssItem() {
      Link = new Uri("");
      Title = "";
      Description = "";
      PubDate = DateTime.MinValue;
      Enclosure = new TRssItemEnclosure();
    }

    public TRssItem(XElement rssItem) {
      Link = new Uri(rssItem.SafeReadElementValue<string>("link", ""));
      Title = rssItem.SafeReadElementValue<string>("title", "");
      Description = rssItem.SafeReadElementValue<string>("description", "");
      PubDate = rssItem.SafeReadElementValue<DateTime>("pubDate", DateTime.MinValue);
      Enclosure = new TRssItemEnclosure(rssItem.SafeReadElement("enclosure"));
    }

    public TRssItem(TRssItem rssItem) {
      Link = new Uri(rssItem.Link.ToString());
      Title = rssItem.Title;
      Description = rssItem.Description;
      PubDate = rssItem.PubDate;
      Enclosure = new TRssItemEnclosure(rssItem.Enclosure);
    }

    public XElement ToXml() {
      XElement RetVal = new XElement("item");
      RetVal.SetElementValue("link", Link.ToString());
      RetVal.SetElementValue("title", Title);
      RetVal.SetElementValue("description", Description);
      RetVal.SetElementValue("pubDate", PubDate.ToString("yyyy-MM-dd HH:mm:ss"));
      RetVal.Add(Enclosure.ToXml());
      return RetVal;
    }
  }


}
