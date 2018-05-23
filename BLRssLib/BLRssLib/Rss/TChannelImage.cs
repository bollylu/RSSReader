using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;
using System.ComponentModel;

namespace BLRssLib {
  public class TChannelImage : IToXml, INotifyPropertyChanged {
    public string Title { get; set; }
    public TUri Url {
      get {
        if (_Url == null || _Url.Value.AbsoluteUri == "http://127.0.0.1/") {
          _Url = new TUri("pack://application:,,,/BLRssLib;component/Resources/rss.png");
        }
        return _Url;
      }
      set {
        _Url = value;
      }
    }
    TUri _Url;
    public int Width { get; set; }
    public int Height { get; set; }

    public TChannelImage() {
      Title = "";
      Url = new TUri();
      Width = 0;
      Height = 0;
    }
    public TChannelImage(XElement image) {
      Title = image.SafeReadElementValue<string>("title", "");
      Url = new TUri(image.SafeReadElementValue<string>("url", ""));
      if (image.Elements().Any(x => x.Name == "width")) {
        Width = image.SafeReadElementValue<int>("width", 0);
      }
      if (image.Elements().Any(x => x.Name == "height")) {
        Height = image.SafeReadElementValue<int>("height", 0);
      }
    }

    public TChannelImage(TChannelImage image) {
      Title = image.Title;
      Url = new TUri(image.Url);
      Width = image.Width;
      Height = image.Height;
    }

    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("ChannelImage: {0}", Title);
      RetVal.AppendFormat(", Url={0}", Url.ToString());
      RetVal.AppendFormat(", width={0}", Width);
      RetVal.AppendFormat(", height={0}", Height);
      return RetVal.ToString();
    }

    public XElement ToXml() {
      XElement RetVal = new XElement("image");
      RetVal.SetElementValue("title", Title);
      RetVal.SetElementValue("url", Url.ToString());
      RetVal.SetElementValue("width", Width);
      RetVal.SetElementValue("height", Height);
      return RetVal;
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }
}
