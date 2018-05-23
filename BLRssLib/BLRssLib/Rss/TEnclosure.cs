using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;
using System.Windows.Media.Imaging;

namespace BLRssLib {
  public class TEnclosure : IToXml {
    #region Public properties
    public TUri Url { get; set; }
    public int Length { get; set; }
    public string Mimetype { get; set; }
    public string Picture {
      get {
        return Url.Value.ToString();
      }
    }
    #endregion Public properties

    #region Constructor(s)
    public TEnclosure() {
      Url = new TUri();
      Length = 0;
      Mimetype = "";
    }
    public TEnclosure(XElement enclosure)
      : this() {
      if (enclosure.HasAttributes) {
        Url = new TUri(enclosure.SafeReadAttribute<string>("url", ""));
        if (enclosure.Attributes().Any(a => a.Name == "length")) {
          Length = enclosure.SafeReadAttribute<int>("length", 0);
        }
        if (enclosure.Attributes().Any(a => a.Name == "mimetype")) {
          Mimetype = enclosure.SafeReadAttribute<string>("mimetype", "");
        }
      }
    }

    public TEnclosure(TEnclosure enclosure)
      : this() {
        if (enclosure != null) {
          Url = new TUri(enclosure.Url);
          Length = enclosure.Length;
          Mimetype = enclosure.Mimetype;
        }
    }
    #endregion Constructor(s)

    #region Converters
    public XElement ToXml() {
      XElement RetVal = new XElement("enclosure");
      RetVal.SetAttributeValue("url", Url.ToString());
      RetVal.SetAttributeValue("length", Length);
      RetVal.SetAttributeValue("mimetype", Mimetype);
      return RetVal;
    }
    public override string ToString() {
      if (Url.ToString() == "http://127.0.0.1") {
        return "No enclosure";
      } else {
        StringBuilder RetVal = new StringBuilder();
        RetVal.AppendFormat("{0}", Url.ToString());
        RetVal.AppendFormat(", {0}", Length);
        RetVal.AppendFormat(", {0}", Mimetype);
        return RetVal.ToString();
      }

    }
    #endregion Converters
  }
}
