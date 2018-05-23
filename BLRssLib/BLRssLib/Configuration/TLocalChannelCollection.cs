using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using BLTools;

namespace BLRssLib {
  public class TLocalChannelCollection : ObservableCollection<TLocalChannel>, IToXml {

    #region Constructor(s)
    public TLocalChannelCollection() { }
    public TLocalChannelCollection(IEnumerable<TLocalChannel> channels) {
      foreach (TLocalChannel ConfigChannelItem in channels) {
        Add(new TLocalChannel(ConfigChannelItem));
      }
    }
    public TLocalChannelCollection(IEnumerable<XElement> channels) {
      foreach (XElement ConfigChannelItem in channels) {
        Add(new TLocalChannel(ConfigChannelItem));
      }
    }
    public TLocalChannelCollection(XElement channels) {
      foreach (XElement ConfigChannelItem in channels.Elements("channel")) {
        Add(new TLocalChannel(ConfigChannelItem));
      }
    } 
    #endregion Constructor(s)

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      foreach (TLocalChannel ConfigChannelItem in this) {
        RetVal.AppendLine(ConfigChannelItem.ToString());
      }
      return RetVal.ToString().Trim('\n');
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("channels");
      foreach (TLocalChannel ConfigChannelItem in this) {
        RetVal.Add(ConfigChannelItem.ToXml());
      }
      return RetVal;
    } 
    #endregion Converters

    public void Add(IEnumerable<TLocalChannel> channels) {
      foreach (TLocalChannel ConfigChannelItem in channels) {
        Add(new TLocalChannel(ConfigChannelItem));
      }
    }
  }
}
