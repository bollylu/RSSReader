using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;

namespace BLRssLib {
  public class TOpmlDocument {

    public const string XML_THIS_ELEMENT = "opml";

    public string Name { get; set; }
    public string Title { get; set; }
    public TOpmlOutlineCollection Outlines { get; set; }

    #region Constructor(s)
    public TOpmlDocument() {
      Name = "";
      Title = "";
      Outlines = new TOpmlOutlineCollection();
    }
    public TOpmlDocument(string filename) {
      Name = filename;
      Load();
    }
    public TOpmlDocument(TOpmlDocument document) {
      Name = document.Name;
      Title = document.Title;
      Outlines = new TOpmlOutlineCollection(document.Outlines);
    }
    #endregion Constructor(s)

    public bool Load() {
      return Load(Name);
    }

    public bool Load( string filename ) {
      #region === Validate parameters ===
      if ( string.IsNullOrWhiteSpace(filename) ) {
        Trace.WriteLine("Unable to open an null or empty filename");
        return false;
      }

      if ( !File.Exists(filename) ) {
        Trace.WriteLine(string.Format("filename is missing or access is denied : {0}", filename));
        return false;
      } 
      #endregion === Validate parameters ===

      Name = filename;
      try {
        XDocument SourceDocument = XDocument.Load(filename);
        Title = SourceDocument.Element(XML_THIS_ELEMENT).Element("head").Element("title").Value;
        Outlines = new TOpmlOutlineCollection(SourceDocument.Element(XML_THIS_ELEMENT).Element("body").Elements(TOpmlOutline.XML_THIS_ELEMENT));
      } catch (Exception ex) {
        Trace.WriteLine(string.Format("Unable to read or parse XML document : {0} : {1}", filename, ex.Message));
      }
      return true;
    }
    
  }
}
