using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using BLTools;

namespace BLRssLib {

  public class TConfigDocument : IToXml {

    public const string DefaultDataFolderPath = "data";

    #region Public properties
    public string Name { get; set; }
    public TConfigGroup RootGroup { get; set; }
    public string DataFolderPath { get; set; }
    #endregion Public properties

    #region Constructor(s)
    public TConfigDocument() {
      Name = "";
      RootGroup = new TConfigGroup();
      DataFolderPath = DefaultDataFolderPath;
    }
    public TConfigDocument(string filename)
      : this() {
      Name = filename;
      Load();
    }
    #endregion Constructor(s)

    public TLocalChannel this[string name] {
      get {
        foreach (TLocalChannel ChannelItem in _GetChannels(this.RootGroup)) {
          if (ChannelItem.Name.ToLower() == name.ToLower()) {
            return ChannelItem;
          }
        }
        return null;
      }
    }

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("ConfigDocument {0}", Name);
      RootGroup.ToString().Split('\n').ToList().ForEach(l => RetVal.AppendFormat("  {0}\n", l));
      return RetVal.ToString();
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("config");
      RetVal.SetAttributeValue("name", Name);
      RetVal.SetAttributeValue("datafolderpath", DataFolderPath);
      RetVal.Add(RootGroup.ToXml());
      return RetVal;
    }
    #endregion Converters

    #region Public methods
    public void Load() {
      if (!string.IsNullOrWhiteSpace(Name)) {
        Load(Name);
      }
    }
    public void Load(string filename) {
      try {
        Trace.WriteLine(string.Format("-- Loading config from \"{0}\"...", Path.GetFullPath(filename)));
        Trace.Indent();

        #region Validate parameters
        if (string.IsNullOrWhiteSpace(filename)) {
          Trace.WriteLine("Unable to load TConfigDocument : filename is null or empty");
          return;
        }
        if (!File.Exists(filename)) {
          Trace.WriteLine(string.Format("Unable to load TConfigDocument \"{0}\" : file is missing or access is denied.", filename));
          return;
        }
        #endregion Validate parameters
        try {
          XDocument SourceDocument = XDocument.Load(filename);
          DataFolderPath = SourceDocument.Root.SafeReadAttribute<string>("datafolderpath", DefaultDataFolderPath);
          RootGroup = new TConfigGroup(SourceDocument.Root.Element("group"));
          //Name = SourceDocument.Root.SafeReadAttribute<string>("name", filename);
        } catch (Exception ex) {
          Trace.WriteLine(string.Format("Error reading TConfigDocument \"{0}\" : {1}", filename, ex.Message));
          return;
        }
      } finally {
        Trace.Unindent();
        Trace.WriteLine("Done.");
      }
    }

    public void Import(string opmlFilename) {
      TOpmlDocument CurrentOpml = new TOpmlDocument(opmlFilename);
      Import(CurrentOpml);
    }
    public void ImportAsync(string opmlFilename) {
      Task ImportTask = Task.Factory.StartNew(() => Import(opmlFilename));
    }

    public void Import(TOpmlDocument opmlDocument) {
      if (opmlDocument.Load()) {
        TConfigGroup ImportedGroup = new TConfigGroup();
        ImportedGroup.Name = opmlDocument.Name;
        foreach (TOpmlOutline OutlineItem in opmlDocument.Outlines) {
          ImportedGroup.ImportOpmlOutline(OutlineItem, DataFolderPath);
        }
        RootGroup.Groups.Add(ImportedGroup);
      }
      Save();
      if (ImportCompleted != null) {
        ImportCompleted(this, EventArgs.Empty);
      }
    }
    public void ImportAsync(TOpmlDocument opmlDocument) {
      Task ImportTask = Task.Factory.StartNew(() => Import(opmlDocument));
    }

    public void Save() {
      XDocument SavedDocument = new XDocument();
      SavedDocument.Add(this.ToXml());

      SavedDocument.Save(Name);
      foreach (TLocalChannel ChannelItem in _GetChannels(this.RootGroup)) {
        ChannelItem.Save();
      }
    }

    #endregion Public methods

    #region Events
    public event EventHandler ImportCompleted;
    #endregion Events

    #region Private methods
    private TLocalChannelCollection _GetChannels(TConfigGroup group) {
      TLocalChannelCollection RetVal = new TLocalChannelCollection();
      RetVal.Add(group.Channels);
      foreach (TConfigGroup GroupItem in group.Groups) {
        RetVal.Add(_GetChannels(GroupItem));
      }
      return RetVal;
    }
    #endregion Private methods
  }

}