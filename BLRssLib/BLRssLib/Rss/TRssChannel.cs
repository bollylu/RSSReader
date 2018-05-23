using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using BLTools;
using System.Threading.Tasks;

namespace BLRssLib {
  public class TRssChannel : IToXml{
    #region Public properties
    public string Name { get; set; }
    public Uri Link { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Language { get; set; }
    public string Copyright { get; set; }
    public DateTime LastBuildDate { get; set; }
    public TChannelImage Image { get; set; }
    public TItemCollection RssItems { get; set; }
    #endregion Public properties

    public TRssChannel() {
      Name = "";
      Title = "";
      Link = new Uri("http://127.0.0.1");
      Description = "";
      Category = "";
      Language = "";
      Copyright = "";
      LastBuildDate = DateTime.MinValue;
      Image = new TChannelImage();
      RssItems = new TItemCollection();
    }
    public TRssChannel(XElement rssChannel) : this() {
      Name = "";
      Title = rssChannel.SafeReadElementValue<string>("title", "");
      Link = new Uri(rssChannel.SafeReadElementValue<string>("link", "http://127.0.0.1"));
      Description = rssChannel.SafeReadElementValue<string>("description", "");
      Category = rssChannel.SafeReadElementValue<string>("category", "");
      Language = rssChannel.SafeReadElementValue<string>("language", "");
      Copyright = rssChannel.SafeReadElementValue<string>("copyright", "");
      LastBuildDate = rssChannel.SafeReadElementValue<DateTime>("lastBuildDate", DateTime.MinValue);
      Image = new TRssChannelImage(rssChannel.SafeReadElement("image"));
      RssItems = new TRssItemCollection(rssChannel.Elements("item"));
    }
    public TRssChannel(TRssChannel rssChannel) : this() {
      Name = rssChannel.Name;
      Title = rssChannel.Title;
      Link = new Uri(rssChannel.Link.ToString());
      Description = rssChannel.Description;
      Category = rssChannel.Category;
      Language = rssChannel.Language;
      Copyright = rssChannel.Copyright;
      LastBuildDate = rssChannel.LastBuildDate;
      Image = new TRssChannelImage(rssChannel.Image);
      RssItems = new TRssItemCollection(rssChannel.RssItems);
    }
    public TRssChannel(TOpmlOutline opmlOutline)
      : this() {
      Name = opmlOutline.Title;
      Title = opmlOutline.Title;
      Link = new Uri(opmlOutline.XmlUrl.ToString());
      Description = opmlOutline.Description;
    }

    public TRssItemCollection DownloadItems() {
      WebClient CurrentClient = new WebClient();

      CurrentClient.Headers.Add("user-agent", "RssWatcher/1.0");
      string XmlString = Encoding.Default.GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(new StreamReader(CurrentClient.OpenRead(Link), Encoding.GetEncoding("ISO-8859-1")).ReadToEnd()));
      XDocument SourceXml;
      try {
        SourceXml = XDocument.Parse(XmlString, LoadOptions.PreserveWhitespace);
        RssItems = new TRssItemCollection(SourceXml.Element("rss").Element("channel").Elements("item"));
      } catch {
        RssItems = new TRssItemCollection();
      }
      
      if (DownloadCompleted != null) {
        DownloadCompleted(this, EventArgs.Empty);
      }
      return RssItems;
    }

    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("Channel: {0}", Name);
      RetVal.AppendFormat(", {0}", Title);
      switch (RssItems.Count) {
        case 0:
          break;
        case 1:
          RetVal.Append(", 1 item");
          break;
        default:
          RetVal.AppendFormat(", {0} items", RssItems.Count);
          break;
      }
      return RetVal.ToString();
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("channel");
      RetVal.SetElementValue("title", Title);
      RetVal.SetElementValue("link", Link.ToString());
      RetVal.SetElementValue("description", Description);
      RetVal.SetElementValue("category", Category);
      RetVal.SetElementValue("language", Language);
      RetVal.SetElementValue("copyright", Copyright);
      RetVal.SetElementValue("lastbuildate", LastBuildDate.ToString("yyyy-MM-dd HH:mm-ss"));
      RetVal.Add(Image.ToXml());
      RetVal.Add(RssItems.ToXml());
      return RetVal;
    }
    public void DownloadItemsAsync() {
      Task DownloadTask = Task.Factory.StartNew(() => DownloadItems());
    }
    public event EventHandler DownloadCompleted;

    
  }

}
