using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Net;

namespace BLRssLib {
  public class TItem : IToXml {
    #region Public properties
    public TUri Link { get; set; }
    public string Title { get; set; }
    public string TitleDecoded {
      get {
        return WebUtility.HtmlDecode(Title);
      }
    }
    public string Description { get; set; }
    public string DescriptionCleaned {
      get {
        return Regex.Replace(WebUtility.HtmlDecode(Description), @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", "");
      }
    }
    public DateTime PubDate { get; set; }
    public string PubDateBE {
      get {
        if (PubDate.Date == DateTime.Today.Date) {
          return PubDate.ToString("HH:mm");
        }
        return PubDate.ToString("dd/MM/yyyy HH:mm");
      }
    }
    public TEnclosure Enclosure { get; set; }
    public bool IsRead { get; set; }
    #endregion Public properties

    #region Constructor(s)
    public TItem() {
      Link = new TUri();
      Title = "";
      Description = "";
      PubDate = DateTime.MinValue;
      Enclosure = new TEnclosure();
      IsRead = false;
    }

    public TItem(XElement item) {
      Link = new TUri(item.SafeReadElementValue<string>("link", ""));
      Title = item.SafeReadElementValue<string>("title", "");
      Description = item.SafeReadElementValue<string>("description", "");
      PubDate = item.SafeReadElementValue<DateTime>("pubDate", DateTime.MinValue, CultureInfo.InvariantCulture);
      if (item.Attributes().Any(x => x.Name == "isread")) {
        IsRead = item.SafeReadAttribute<bool>("isread", false);
      }
      if (item.Elements().Any(x => x.Name == "enclosure")) {
        Enclosure = new TEnclosure(item.SafeReadElement("enclosure"));
      }
    }

    public TItem(TItem item) {
      Link = new TUri(item.Link);
      Title = item.Title;
      Description = item.Description;
      PubDate = item.PubDate;
      Enclosure = new TEnclosure(item.Enclosure);
      IsRead = item.IsRead;
    }
    #endregion Constructor(s)

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("{0}", Title);
      return RetVal.ToString();
    }

    public XElement ToXml() {
      XElement RetVal = new XElement("item");
      RetVal.SetElementValue("link", Link.ToString());
      RetVal.SetElementValue("title", Title);
      RetVal.SetElementValue("description", Description);
      RetVal.SetElementValue("pubDate", PubDate.ToString("yyyy-MM-dd HH:mm:ss"));
      RetVal.SetAttributeValue("isread", IsRead);
      if (Enclosure != null) {
        RetVal.Add(Enclosure.ToXml());
      }
      return RetVal;
    }
    #endregion Converters
  }

}
