using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BLRssLib;
using BLTools.WPF;
using System.Globalization;
using System.Net;
using System.Diagnostics;

namespace RssWatcher {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    private static readonly string DefaultConfigurationFile = "RssWatcherConfig.xml";
    private TConfigDocument _ConfigDocument;


    public MainWindow() {
      InitializeComponent();
    }

    #region Window events
    private void Window_Loaded(object sender, RoutedEventArgs e) {
      BLTools.Log.DebugLevel = BLTools.ErrorLevel.Info;
      _ConfigDocument = new TConfigDocument(DefaultConfigurationFile);
      TreeViewRssItem RootItem = new TreeViewRssItem(_ConfigDocument.RootGroup);
      tvChannels.ItemsSource = RootItem.Items;
    }
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
      //_ConfigDocument.Save();
    }
    #endregion Window events

    #region Menu
    private void mnuFileOpen_Click(object sender, RoutedEventArgs e) {

    }

    private void mnuFileNew_Click(object sender, RoutedEventArgs e) {

    }

    private void mnuFileQuit_Click(object sender, RoutedEventArgs e) {
      Application.Current.Shutdown();
    }

    private void mnuFileSave_Click(object sender, RoutedEventArgs e) {
      _ConfigDocument.Save();
    }

    private void mnuFileExport_Click(object sender, RoutedEventArgs e) {

    }

    private void mnuFileImport_Click(object sender, RoutedEventArgs e) {
      System.Windows.Forms.OpenFileDialog OFD = new System.Windows.Forms.OpenFileDialog();
      if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
        if (_ConfigDocument == null) {
          _ConfigDocument = new TConfigDocument();
        }
        _ConfigDocument.Import(OFD.FileName);
        TreeViewRssItem RootItem = new TreeViewRssItem(_ConfigDocument.RootGroup);
        tvChannels.ItemsSource = RootItem.Items;
      }
    }

    private void mnuToolsOptions_Click(object sender, RoutedEventArgs e) {

    }

    private void mnuHelpAbout_Click(object sender, RoutedEventArgs e) {
      MessageBox.Show("RSS Watcher v0.1\n(c) 2011 Luc Bolly");
    }
    #endregion Menu

    #region Window Status helper
    private void SetStatusLeft(string message) {
      txtStatusBarLeft.Text = message;
    }
    private void SetStatusRight(string message) {
      txtStatusBarRight.Text = message;
    }
    #endregion Window Status helper

    #region Channel handling
    private void tvChannels_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
      if (e.NewValue is TLocalChannel) {
        TLocalChannel CurrentLocalChannel = e.NewValue as TLocalChannel;
        if (CurrentLocalChannel.Channel.Items == null || CurrentLocalChannel.Channel.Items.Count() == 0) {
          SetStatusRight("Loading news ...");
          LoadChannel(CurrentLocalChannel);
        } else {
          _RefreshItems(CurrentLocalChannel);
        }
      }
      //e.Handled = true;
    }
    private void LoadChannel(TLocalChannel channel) {
      Trace.WriteLine(string.Format("Loading local channel \"{0}\" from disk [{1}]", channel.Name, channel.StorageFullName));
      channel.LoadCompleted += new EventHandler(Channel_LoadCompleted);
      channel.LoadAsync();
    }
    private void Channel_LoadCompleted(object sender, EventArgs e) {
      TLocalChannel CurrentLocalChannel = sender as TLocalChannel;
      CurrentLocalChannel.LoadCompleted -= new EventHandler(Channel_LoadCompleted);
      this.Dispatch(() => { _RefreshItems(CurrentLocalChannel); });
    }

    private void DownloadChannel(TChannel channel) {
      channel.DownloadCompleted += new EventHandler<ItemsDownloadedEventArgs>(Channel_DownloadCompleted);
      channel.DownloadItemsAsync();
    }
    private void Channel_DownloadCompleted(object sender, ItemsDownloadedEventArgs e) {
      TLocalChannel CurrentChannel = sender as TLocalChannel;
      CurrentChannel.DownloadCompleted -= new EventHandler<ItemsDownloadedEventArgs>(Channel_DownloadCompleted);
      this.Dispatch(() => {
        DebugDisplay.Text = _DebugDisplay(e.ResponseHeaders);
        _RefreshItems(CurrentChannel);
      });
    }

    private static string _DebugDisplay(WebHeaderCollection responseHeaders) {
      StringBuilder DebugDisplayString = new StringBuilder();
      foreach (string HeaderKeyItem in responseHeaders.AllKeys) {
        DebugDisplayString.AppendFormat("{0}={1}\n", HeaderKeyItem, responseHeaders.Get(HeaderKeyItem));
      }
      return DebugDisplayString.ToString();
    }
    private void _RefreshItems(TLocalChannel localChannel) {
      lvItems.ItemsSource = localChannel.Channel.Items.OrderByDescending(x => x.PubDate).ToList();
      lvItems.Items.Refresh();
      lvItems.UpdateLayout();
      lvItems.ResizeLastColumn();
      SetStatusRight("News updated.");
    }

    #region Context menu
    private void ctxmnuRefreshChannel_Click(object sender, RoutedEventArgs e) {
      TLocalChannel CurrentChannel = tvChannels.SelectedItem as TLocalChannel;
      CurrentChannel.DownloadCompleted += new EventHandler<ItemsDownloadedEventArgs>(Channel_DownloadCompleted);
      CurrentChannel.DownloadItems();
    }
    private void ctxmnuEncoding_SubmenuOpened(object sender, RoutedEventArgs e) {
      TLocalChannel CurrentChannel = tvChannels.SelectedItem as TLocalChannel;
      if (CurrentChannel != null) {
        MenuItem CurrentMenuItem = sender as MenuItem;
        if (CurrentChannel.Channel.ChannelEncoding != null) {
          string EncodingName = CurrentChannel.Channel.ChannelEncoding.WebName.ToLower();
          foreach (MenuItem MenuItemItem in CurrentMenuItem.Items) {
            if (MenuItemItem.Header.ToString().ToLower() == EncodingName) {
              MenuItemItem.IsChecked = true;
            } else {
              MenuItemItem.IsChecked = false;
            }
          }
        }
      }
    }
    private void ctxmnuEncoding_Click(object sender, RoutedEventArgs e) {
      if (sender is MenuItem) {
        MenuItem CurrentMenuItem = sender as MenuItem;
        TLocalChannel CurrentChannel = tvChannels.SelectedItem as TLocalChannel;
        CurrentChannel.Channel.ChannelEncoding = Encoding.GetEncoding(CurrentMenuItem.Header.ToString());
        CurrentChannel.Channel.LastBuildDate = DateTime.MinValue;
        CurrentChannel.Channel.Items.Clear();
        CurrentChannel.Save();
        CurrentChannel.Load();
        lvItems.Items.Refresh();
        CurrentChannel.DownloadCompleted += new EventHandler<ItemsDownloadedEventArgs>(Channel_DownloadCompleted);
        CurrentChannel.DownloadItems();
      }
    }
    #endregion Context menu

    #endregion Channel handling

    #region Items handling
    private void lvItems_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (e.AddedItems.Count == 1) {
        SetStatusRight("Loading RSS item ...");
        TItem CurrentItem = lvItems.SelectedItem as TItem;
        CurrentItem.IsRead = true;
        lvItems.Items.Refresh();
        frmBrowser.Source = CurrentItem.Link.Value;
        TLocalChannel CurrentChannel = tvChannels.SelectedItem as TLocalChannel;
        CurrentChannel.Save();
      }
    }
    private void lvItems_SizeChanged(object sender, SizeChangedEventArgs e) {
      ListViewEx CurrentListView = sender as ListViewEx;
      CurrentListView.ResizeLastColumn();
    }
    #endregion Items handling

    #region Browser handling
    private void frmBrowser_Navigated(object sender, NavigationEventArgs e) {
      SetStatusRight("RSS completely loaded.");
      WebBrowser Browser = sender as WebBrowser;
      mshtml.HTMLDocument HTMLDoc = Browser.Document as mshtml.HTMLDocument;
      mshtml.IHTMLScriptElement HTMLScript = (mshtml.IHTMLScriptElement)HTMLDoc.createElement("SCRIPT");
      HTMLScript.type = "text/javascript";
      HTMLScript.text = @"function noError() { return true; } window.onerror = noError;";
      mshtml.IHTMLElementCollection Nodes = HTMLDoc.getElementsByTagName("HEAD");
      foreach (mshtml.HTMLHeadElement HTMLElementItem in Nodes) {
        HTMLElementItem.appendChild((mshtml.IHTMLDOMNode)HTMLScript);
      }
    }
    #endregion Browser handling

    private void lvItems_SourceUpdated(object sender, DataTransferEventArgs e) {

    }

    
  }

}
