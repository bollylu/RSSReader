using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using BLRssLib;

namespace RssWatcher {
  public class TreeViewRssItem {
    public string Header { get; set; }
    public ObservableCollection<object> Items { get; set; }
    public TreeViewRssItem(TConfigGroup group) {
      Header = group.Name;
      Items = new ObservableCollection<object>();
      foreach (TLocalChannel ChannelItem in group.Channels) {
        Items.Add(ChannelItem);
      }
      foreach (TConfigGroup GroupItem in group.Groups) {
        Items.Add(new TreeViewRssItem(GroupItem));
      }
    }
    public override string ToString() {
      return Header;
    }
  }
}
