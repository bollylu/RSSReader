using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using BLTools;

namespace BLRssLib {
  public class TUri : IToXml {

    public static readonly string DefaultUriValue = "http://127.0.0.1";
    public Uri Value { get; set; }

    #region Constructor(s)
    public TUri() {
      Value = new Uri(DefaultUriValue);
    }

    public TUri(string uriString)
      : this() {
      if (string.IsNullOrWhiteSpace(uriString)) {
        return;
      }
      try {
        Value = new Uri(uriString);
      } catch (Exception ex) {
        Trace.WriteLine(string.Format("Unable to create TUri from string \"{0)\" : {1}", uriString, ex.Message));
      }
    }

    public TUri(Uri uri)
      : this() {
      try {
        Value = new Uri(uri.ToString());
      } catch (Exception ex) {
        Trace.WriteLine(string.Format("Unable to create TUri from Uri \"{0)\" : {1}", uri.ToString(), ex.Message));
      }
    }

    public TUri(TUri uri)
      : this() {
      try {
        Value = new Uri(uri.Value.ToString());
      } catch (Exception ex) {
        Trace.WriteLine(string.Format("Unable to create TUri from Uri \"{0)\" : {1}", uri.ToString(), ex.Message));
      }
    } 
    #endregion Constructor(s)

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("{0}", Value);
      return RetVal.ToString();
    }

    public XElement ToXml() {
      XElement RetVal = new XElement("uri");
      RetVal.SetAttributeValue("value", Value.ToString());
      return RetVal;
    } 
    #endregion Converters

  }
}
