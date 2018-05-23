using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TConfigGroup : IToXml {

    #region Public properties
    public string Name { get; set; }
    public TConfigGroupCollection Groups { get; set; }
    public TLocalChannelCollection Channels { get; set; } 
    #endregion Public properties

    #region Constructor(s)
    public TConfigGroup() {
      Name = "";
      Groups = new TConfigGroupCollection();
      Channels = new TLocalChannelCollection();
    }
    public TConfigGroup(string name)
      : this() {
      Name = name;
    }
    public TConfigGroup(TConfigGroup group)
      : this() {
      Name = group.Name;
      Groups = new TConfigGroupCollection(group.Groups);
      Channels = new TLocalChannelCollection(group.Channels);
    }
    public TConfigGroup(XElement group)
      : this() {
      Name = group.SafeReadAttribute<string>("name", "");
      Channels = new TLocalChannelCollection(group.SafeReadElement("channels"));
      Groups = new TConfigGroupCollection(group.SafeReadElement("groups"));
    } 
    #endregion Constructor(s)

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("Group {0}", Name);
      if (Groups.Count > 0) {
        RetVal.AppendFormat(", {0} groups", Groups.Count);
      }
      if (Channels.Count > 0) {
        RetVal.AppendFormat(", {0} channels", Channels.Count);
      }
      return RetVal.ToString();
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("group");
      RetVal.SetAttributeValue("name", Name);
      RetVal.Add(Groups.ToXml());
      RetVal.Add(Channels.ToXml());
      return RetVal;
    } 
    #endregion Converters

    #region Public methods
    public void ImportOpmlOutline(TOpmlOutline outline, string dataPath) {
      switch (outline.OutlineType) {
        case "rss":
          TLocalChannel NewChannel = new TLocalChannel(outline);
          NewChannel.StoragePath = dataPath;
          Channels.Add(NewChannel);
          break;
        case "group":
          TConfigGroup NewGroup = new TConfigGroup();
          NewGroup.Name = outline.Title;
          if (outline.Outlines.Count > 0) {
            foreach (TOpmlOutline OutlineItem in outline.Outlines) {
              NewGroup.ImportOpmlOutline(OutlineItem, dataPath);
            }
          }
          Groups.Add(NewGroup);
          break;
      }

    }  
    #endregion Public methods
  }
}
