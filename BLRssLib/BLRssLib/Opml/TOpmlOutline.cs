using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TOpmlOutline {

    public const string XML_THIS_ELEMENT = "outline";
    public const string XML_ATTRIBUTE_TITLE = "title";
    public const string XML_ATTRIBUTE_TYPE = "type";
    public const string XML_ATTRIBUTE_DESCRIPTION = "description";
    public const string XML_ATTRIBUTE_XML_URL = "xmlUrl";
    public const string XML_ATTRIBUTE_HTML_URL = "htmlUrl";

    public const string XML_DEFAULT_XML_URL = "http://127.0.0.1";
    public const string XML_DEFAULT_HTML_URL = "http://127.0.0.1";
    public const string XML_DEFAULT_TYPE = "group";

    #region --- Public properties ------------------------------------------------------------------------------
    public string OutlineType { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Uri XmlUrl { get; set; }
    public Uri HtmlUrl { get; set; }
    public TOpmlOutlineCollection Outlines { get; set; }
    #endregion --- Public properties ---------------------------------------------------------------------------

    #region --- Constructor(s) ---------------------------------------------------------------------------------
    public TOpmlOutline() {
      OutlineType = XML_DEFAULT_TYPE;
      Title = "";
      Description = "";
      XmlUrl = new Uri(XML_DEFAULT_XML_URL);
      HtmlUrl = new Uri(XML_DEFAULT_HTML_URL);
      Outlines = new TOpmlOutlineCollection();
    }

    public TOpmlOutline(XElement opmlOutline)
      : this() {
      Title = opmlOutline.SafeReadAttribute<string>(XML_ATTRIBUTE_TITLE, "");
      if ( opmlOutline.Attributes().Any(x => x.Name == XML_ATTRIBUTE_TYPE) ) {
        OutlineType = opmlOutline.SafeReadAttribute<string>(XML_ATTRIBUTE_TYPE, XML_DEFAULT_TYPE);
      }
      if ( OutlineType == "rss" ) {
        Description = opmlOutline.SafeReadAttribute<string>(XML_ATTRIBUTE_DESCRIPTION, "");
        XmlUrl = new Uri(opmlOutline.SafeReadAttribute<string>(XML_ATTRIBUTE_XML_URL, XML_DEFAULT_XML_URL));
        string HtmlUrlAttribute = opmlOutline.SafeReadAttribute<string>(XML_ATTRIBUTE_HTML_URL, XML_DEFAULT_HTML_URL);
        HtmlUrl = new Uri(HtmlUrlAttribute == "" ? XML_DEFAULT_HTML_URL : HtmlUrlAttribute);
        Outlines = new TOpmlOutlineCollection();
      } else {
        Description = "";
        XmlUrl = new Uri(XML_DEFAULT_XML_URL);
        HtmlUrl = new Uri(XML_DEFAULT_HTML_URL);
        if ( opmlOutline.Elements(XML_THIS_ELEMENT).Count() > 0 ) {
          Outlines = new TOpmlOutlineCollection(opmlOutline.Elements(XML_THIS_ELEMENT));
        } else {
          Outlines = new TOpmlOutlineCollection();
        }
      }
    }

    public TOpmlOutline(TOpmlOutline opmlOutline)
      : this() {
      OutlineType = opmlOutline.OutlineType;
      Title = opmlOutline.Title;
      Description = opmlOutline.Description;
      XmlUrl = new Uri(opmlOutline.XmlUrl.ToString());
      HtmlUrl = new Uri(opmlOutline.HtmlUrl.ToString());
      Outlines = new TOpmlOutlineCollection(opmlOutline.Outlines);
    }
    #endregion --- Constructor(s) ------------------------------------------------------------------------------

    #region --- Converters -------------------------------------------------------------------------------------
    public override string ToString() {
      return ToString(false);
    }
    public string ToString(bool deep) {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("Type={0}", OutlineType);
      RetVal.AppendFormat(", {0}", Title);
      if ( deep ) {
        RetVal.AppendFormat(", Description={0}", Description);
        RetVal.AppendFormat(", XmlUrl=\"{0}\"", XmlUrl);
        RetVal.AppendFormat(", HtmlUrl=\"{0}\"", HtmlUrl);
      }
      switch ( Outlines.Count ) {
        case 1:
          RetVal.Append(" (1 item)");
          break;
        case 0:
          break;
        default:
          RetVal.AppendFormat(" ({0} items)", Outlines.Count);
          break;
      }

      return RetVal.ToString();
    }
    #endregion --- Converters -------------------------------------------------------------------------------------

  }


}
