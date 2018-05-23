using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TRssChannelImage : IToXml{
    public string Title { get; set; }
    public Uri Url { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public TRssChannelImage() {
      Title = "";
      Url = new Uri("http://127.0.0.1");
      Width = 0;
      Height = 0;
    }
    public TRssChannelImage(XElement rssChannelImage) {
      Title = rssChannelImage.SafeReadElementValue<string>("title", "");
      string url = rssChannelImage.SafeReadElementValue<string>("url", "http://127.0.0.1");
      Url = new Uri(url == "" ? "http://127.0.0.1" : url);
      Width = rssChannelImage.SafeReadElementValue<int>("width", 0);
      Height = rssChannelImage.SafeReadElementValue<int>("height", 0);
    }

    public TRssChannelImage(TRssChannelImage rssChannelImage) {
      Title = rssChannelImage.Title;
      Url = new Uri(rssChannelImage.Url.ToString());
      Width = rssChannelImage.Width;
      Height = rssChannelImage.Height;
    }

    public XElement ToXml() {
      XElement RetVal = new XElement("image");
      RetVal.SetElementValue("title", Title);
      RetVal.SetElementValue("url", Url.ToString());
      RetVal.SetElementValue("width", Width);
      RetVal.SetElementValue("height", Height);
      return RetVal;
    }
  }
}
