using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TConfigGroupCollection : ObservableCollection<TConfigGroup>, IToXml {

    #region Constructor(s)
    public TConfigGroupCollection() { }
    public TConfigGroupCollection(IEnumerable<TConfigGroup> groups) {
      foreach (TConfigGroup ConfigGroupItem in groups) {
        Add(new TConfigGroup(ConfigGroupItem));
      }
    }
    public TConfigGroupCollection(IEnumerable<XElement> groups) {
      foreach (XElement ConfigGroupItem in groups) {
        Add(new TConfigGroup(ConfigGroupItem));
      }
    }
    public TConfigGroupCollection(XElement groups) {
      if (groups.HasElements) {
        foreach (XElement ConfigGroupItem in groups.Elements("group")) {
          Add(new TConfigGroup(ConfigGroupItem));
        }
      }
    } 
    #endregion Constructor(s)

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      foreach (TConfigGroup ConfigGroupItem in this) {
        RetVal.AppendLine(ConfigGroupItem.ToString());
      }
      return RetVal.ToString().Trim('\n');
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("groups");
      foreach (TConfigGroup ConfigGroupItem in this) {
        RetVal.Add(ConfigGroupItem.ToXml());
      }
      return RetVal;
    } 
    #endregion Converters
  }
}
