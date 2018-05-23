using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Net;
using System.IO;
using BLTools;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO.Compression;
using System.ComponentModel;

namespace BLRssLib {
  public class TChannel : IToXml, INotifyPropertyChanged {

    public const string DefaultChannelEncoding = "ISO-8859-1";

    #region Public properties
    public string Name { get; set; }
    public TUri Link { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string Language { get; set; }
    public string Copyright { get; set; }
    public DateTime LastBuildDate { get; set; }
    public TChannelImage Picture { get; set; }
    public TItemCollection Items { get; set; }
    public Encoding ChannelEncoding { get; set; }
    #endregion Public properties

    #region Constructor(s)
    public TChannel() {
      Name = "";
      Title = "";
      Link = new TUri();
      Description = "";
      Category = "";
      Language = "";
      Copyright = "";
      LastBuildDate = DateTime.MinValue;
      Picture = new TChannelImage();
      Items = new TItemCollection();
      ChannelEncoding = null;
    }
    public TChannel(XElement channel)
      : this() {

      Title = channel.SafeReadElementValue<string>("title", "");
      Trace.WriteLine(string.Format("Building TChannel {0}...", Title));
      Link = new TUri(channel.SafeReadElementValue<string>("link", ""));
      Description = channel.SafeReadElementValue<string>("description", "");
      if (channel.Elements().Any(x => x.Name == "category")) {
        Category = channel.SafeReadElementValue<string>("category", "");
      }
      if (channel.Elements().Any(x => x.Name == "language")) {
        Language = channel.SafeReadElementValue<string>("language", "");
      }
      if (channel.Elements().Any(x => x.Name == "copyright")) {
        Copyright = channel.SafeReadElementValue<string>("copyright", "");
      }
      LastBuildDate = channel.SafeReadElementValue<DateTime>("lastbuilddate", DateTime.MinValue);
      if (channel.Elements().Any(x => x.Name == "image")) {
        Picture = new TChannelImage(channel.SafeReadElement("image"));
      }
      if (channel.Elements().Any(x => x.Name == "items")) {
        Trace.WriteLine(string.Format("  Reading {0} item(s)", channel.Element("items").Elements().Count(x => x.Name == "item")));
        Items = new TItemCollection(channel.SafeReadElement("items"));
      }
      if (channel.Attributes().Any(x => x.Name == "encoding")) {
        string EncodingValue = channel.SafeReadAttribute<string>("encoding", DefaultChannelEncoding);
        if (EncodingValue != "") {
          ChannelEncoding = Encoding.GetEncoding(EncodingValue);
        }
      }
      if (channel.Attributes().Any(x => x.Name == "name")) {
        Name = channel.SafeReadAttribute<string>("name", "");
      } else {
        Name = Title;
      }
      Trace.WriteLine(string.Format("  {0}", this.ToString()));
      Trace.WriteLine("Done.");

    }
    public TChannel(TChannel channel)
      : this() {
      Name = channel.Name;
      Title = channel.Title;
      Link = new TUri(channel.Link);
      Description = channel.Description;
      Category = channel.Category;
      Language = channel.Language;
      Copyright = channel.Copyright;
      LastBuildDate = channel.LastBuildDate;
      Picture = new TChannelImage(channel.Picture);
      Items = new TItemCollection(channel.Items);
      ChannelEncoding = channel.ChannelEncoding;
    }
    public TChannel(TOpmlOutline opmlOutline)
      : this() {
      Name = opmlOutline.Title;
      Title = opmlOutline.Title;
      Link = new TUri(opmlOutline.XmlUrl.ToString());
      Description = opmlOutline.Description;
    }
    #endregion Constructor(s)

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("Channel: {0}", Name);
      RetVal.AppendFormat(", {0}", Title);
      switch (Items.Count) {
        case 0:
          break;
        case 1:
          RetVal.Append(", 1 item");
          break;
        default:
          RetVal.AppendFormat(", {0} items", Items.Count);
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
      RetVal.SetElementValue("lastbuilddate", LastBuildDate.ToString("yyyy-MM-dd HH:mm-ss"));
      RetVal.Add(Picture.ToXml());
      RetVal.Add(Items.ToXml());
      RetVal.SetAttributeValue("encoding", ChannelEncoding == null ? "" : ChannelEncoding.WebName);
      return RetVal;
    }
    #endregion Converters

    #region Public methods
    public void DownloadItems() {
      try {
        Trace.WriteLine(string.Format("Downloading items for channel {0} ...", Title));
        Trace.Indent();
        WebClient CurrentClient = new WebClient();

        CurrentClient.Headers.Add("user-agent", "RssWatcher/1.0");
        byte[] Data = CurrentClient.DownloadData(Link.Value);

        #region Discover the encoding
        if (ChannelEncoding == null || string.IsNullOrWhiteSpace(ChannelEncoding.WebName)) {
          string ContentType = CurrentClient.ResponseHeaders.Get("Content-Type");
          string[] ContentTypeItems = ContentType.Split(';');
          SplitArgs oArgs = new SplitArgs(ContentTypeItems);
          ChannelEncoding = Encoding.GetEncoding(oArgs.GetValue<string>("charset", DefaultChannelEncoding));
        }
        Trace.WriteLine(string.Format("Encoding for {0} : {1}", Name, ChannelEncoding.WebName));
        #endregion Discover the encoding

        #region If content is gzipped, then decompress
        string ContentEncoding = CurrentClient.ResponseHeaders.Get("Content-Encoding");
        if (!string.IsNullOrWhiteSpace(ContentEncoding) && ContentEncoding.ToLower() == "gzip") {
          using (MemoryStream TempStream = new MemoryStream()) {
            using (GZipStream CompressedStream = new GZipStream(new MemoryStream(Data), CompressionMode.Decompress)) {
              CompressedStream.CopyTo(TempStream);
            }
            TempStream.Flush();
            Data = new byte[TempStream.Length];
            Buffer.BlockCopy(TempStream.GetBuffer(), 0, Data, 0, (int)TempStream.Length);
          }
        }
        #endregion If content is gzipped, then decompress

        #region Read and parse the data
        XDocument SourceXml;
        try {
          #region Skip the BOM code if it exists
          string TempXml = ChannelEncoding.GetString(Data);
          if (TempXml.IndexOf('<') > 0) {
            TempXml = TempXml.Substring(TempXml.IndexOf("<"));
          }
          #endregion Skip the BOM code if it exists
          SourceXml = XDocument.Parse(TempXml);
          XElement XChannel = SourceXml.Element("rss").Element("channel");
          TItemCollection TempItems = new TItemCollection(XChannel.Elements("item"));
          DateTime TempLastBuildDate = LastBuildDate;
          Trace.WriteLine(string.Format("Found {0} item(s)", TempItems.Count));
          List<TItem> NewItems = TempItems.Where(i => i.PubDate > LastBuildDate).ToList();
          Trace.WriteLine(string.Format("{0} new item(s)", NewItems.Count));
          foreach (TItem ItemItem in NewItems) {
            if (ItemItem.PubDate > TempLastBuildDate) {
              TempLastBuildDate = ItemItem.PubDate;
            }
            Items.Add(ItemItem);
          }
          LastBuildDate = TempLastBuildDate;

          Description = XChannel.SafeReadElementValue<string>("description", "");
          if (XChannel.Elements().Any(x => x.Name == "image")) {
            Picture = new TChannelImage(XChannel.SafeReadElement("image"));
          }
          if (XChannel.Elements().Any(x => x.Name == "category")) {
            Category = XChannel.SafeReadElementValue<string>("category", "");
          }
          if (XChannel.Elements().Any(x => x.Name == "language")) {
            Language = XChannel.SafeReadElementValue<string>("language", "");
          }
          if (XChannel.Elements().Any(x => x.Name == "copyright")) {
            Copyright = XChannel.SafeReadElementValue<string>("copyright", "");
          }
          Title = XChannel.SafeReadElementValue<string>("title", "");

        } catch (Exception ex) {
          Trace.WriteLine(string.Format("Error while reading items for channel {0} : {1}", Name, ex.Message), Severity.Error);
        }
        #endregion Read and parse the data

        if (DownloadCompleted != null) {
          ItemsDownloadedEventArgs Args = new ItemsDownloadedEventArgs(Data, CurrentClient.ResponseHeaders, Encoding.UTF8.GetString(Data));
          DownloadCompleted(this, Args);
        }

        return;
      } finally {
        Trace.Unindent();
        Trace.WriteLine("Done.");
      }
    }
    public void DownloadItemsAsync() {
      Task DownloadTask = new Task(() => DownloadItems());
    }
    #endregion Public methods

    #region Events
    public event EventHandler<ItemsDownloadedEventArgs> DownloadCompleted;
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion Events

  }

  #region Support classes
  public class ItemsDownloadedEventArgs : EventArgs {
    public byte[] RawData;
    public WebHeaderCollection ResponseHeaders;
    public string Data;

    public ItemsDownloadedEventArgs(byte[] rawData, WebHeaderCollection headers, string data) {
      if (rawData != null) {
        RawData = new byte[rawData.Length];
        Buffer.BlockCopy(rawData, 0, RawData, 0, rawData.Length);
      }
      ResponseHeaders = headers;
      Data = data;
    }
  }
  #endregion Support classes
}
