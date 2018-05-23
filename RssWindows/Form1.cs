using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLRssLib;
using System.Xml.Linq;
using System.Diagnostics;

namespace RssWindows {
  public partial class Form1 : Form {

    //private OpmlDocument _OpmlDocument;
    private TConfigDocument _RssConfigDocument;
    private TRssChannelCollection _RssChannels;

    public Form1() {
      InitializeComponent();
    }

    private void Form1_Load( object sender, EventArgs e ) {
      #region Tree view
      treeChannel.BeginUpdate();
      treeChannel.FullRowSelect = true;
      treeChannel.LabelEdit = false;
      treeChannel.ShowLines = true;

      _RssConfigDocument = new TConfigDocument("RssReaderConfig.xml");
      treeChannel.Nodes.Clear();
      treeChannel.Nodes.Add("root", "RSS Channels");
      FillTree(treeChannel.Nodes["root"], _RssConfigDocument.Groups);
      treeChannel.ExpandAll();

      treeChannel.EndUpdate();
      #endregion Tree view

      #region Items view
      dgvItems.AllowUserToResizeColumns = true;
      dgvItems.AllowUserToResizeRows = false;
      dgvItems.ReadOnly = true;
      dgvItems.ColumnHeadersVisible = false;
      dgvItems.RowHeadersVisible = false;
      dgvItems.AutoGenerateColumns = false;
      dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Link", DataPropertyName = "Link" });
      dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "PubDate", DataPropertyName = "PubDate" });
      dgvItems.Columns.Add(new DataGridViewTextBoxColumn() { Name = "Title", DataPropertyName = "Title" });
      dgvItems.Columns["Link"].Visible = false;
      dgvItems.Columns["PubDate"].Width = 135;
      dgvItems.Columns["PubDate"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm:ss";
      dgvItems.Columns["Title"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
      #endregion Items view

      #region Web browser
      _WebDisplay.AllowNavigation = false;
      #endregion Web browser
    }

    private void FillTree( TreeNode currentNode, TGroupCollection rssGroups ) {
      foreach ( TGroup RssGroupItem in rssGroups ) {
        currentNode.Nodes.Add(RssGroupItem.Name, RssGroupItem.Name);
        currentNode.Nodes[RssGroupItem.Name].Tag = RssGroupItem;
        foreach ( TRssChannel RssChannelItem in RssGroupItem.Channels ) {
          currentNode.Nodes[RssGroupItem.Name].Nodes.Add(RssChannelItem.Title, RssChannelItem.Title);
          currentNode.Nodes[RssGroupItem.Name].Nodes[RssChannelItem.Title].Tag = RssChannelItem;
        }
        if ( RssGroupItem.Groups.Count>0 ) {
          FillTree(currentNode.Nodes[RssGroupItem.Name], RssGroupItem.Groups);
        }
      }
      return;
    }

    private void FillTree(TreeNode currentNode, TOpmlOutlineCollection opmlOutlines ) {
      foreach(TOpmlOutline OpmlOutlineItem in opmlOutlines) {
        currentNode.Nodes.Add(OpmlOutlineItem.Title, OpmlOutlineItem.Title);
        if ( OpmlOutlineItem.OutlineType == "" ) {
          FillTree(currentNode.Nodes[OpmlOutlineItem.Title], OpmlOutlineItem.Outlines);
        } else {
          currentNode.Nodes[OpmlOutlineItem.Title].Tag = OpmlOutlineItem;
        }
      }
      return;
    }

    private void treeChannel_AfterSelect( object sender, TreeViewEventArgs e ) {
      TreeNode CurrentNode = e.Node;
      TreeView CurrentTreeView = sender as TreeView;
      dgvItems.SuspendLayout();
      if ( CurrentTreeView.SelectedNode.IsSelected) {
        if ( CurrentNode.Tag is TRssChannel ) {
          TRssChannel CurrentChannel = CurrentNode.Tag as TRssChannel;
          dgvItems.DataSource = CurrentChannel.DownloadItems().OrderBy(x => x.PubDate).ToList();
          dgvItems.Refresh();
          //Uri TargetUrl = ((OpmlOutline)CurrentNode.Tag).XmlUrl;
          //try {
          //  if ( _WebDisplay.ReadyState != WebBrowserReadyState.Complete && _WebDisplay.ReadyState!= WebBrowserReadyState.Uninitialized) {
          //    _WebDisplay.Stop();
          //  }
          //  _WebDisplay.AllowNavigation = true;
          //  _WebDisplay.ScriptErrorsSuppressed = true;
          //  StatusLabel.Text = _WebDisplay.ReadyState.ToString();
          //  Trace.WriteLine(TargetUrl.ToString());
          //  _WebDisplay.Url = TargetUrl;
          //  _WebDisplay.Update();
          //} catch ( Exception ex ) {
          //  Trace.WriteLine(string.Format("Error during navigation to {0} : {1}", TargetUrl.ToString(), ex.Message));
          //}
        }
      } else {
        dgvItems.DataSource = null;
      }
      dgvItems.Refresh();
      dgvItems.ResumeLayout();
    }

    private void dgvItems_SelectionChanged( object sender, EventArgs e ) {
      DataGridView CurrentDataGridView = sender as DataGridView;
      
      //if ( CurrentDataGridView.CurrentRow.Selected == true ) {
        Uri TargetUrl = new Uri(CurrentDataGridView.CurrentRow.Cells["Link"].Value.ToString());
        if ( _WebDisplay.ReadyState != WebBrowserReadyState.Complete && _WebDisplay.ReadyState != WebBrowserReadyState.Uninitialized ) {
          _WebDisplay.Stop();
        }
        _WebDisplay.AllowNavigation = true;
        _WebDisplay.ScriptErrorsSuppressed = true;
        StatusLabel.Text = _WebDisplay.ReadyState.ToString();
        Trace.WriteLine(TargetUrl.ToString());
        _WebDisplay.Url = TargetUrl;
      //} else {
      //  _WebDisplay.Url = null;
      //}
    }

    private void openToolStripMenuItem_Click( object sender, EventArgs e ) {
      OpenFileDialog OFD = new OpenFileDialog();
      if ( OFD.ShowDialog() == DialogResult.OK ) {
        _RssConfigDocument = new TConfigDocument(OFD.FileName);
        treeChannel.Nodes.Clear();
        treeChannel.Nodes.Add("root", "RSS Channels");
        FillTree(treeChannel.Nodes["root"], _RssConfigDocument.Groups);
        treeChannel.ExpandAll();
      }
    }

    private void _WebDisplay_DocumentCompleted( object sender, WebBrowserDocumentCompletedEventArgs e ) {
      StatusLabel.Text = _WebDisplay.ReadyState.ToString();
    }

    private void _WebDisplay_ProgressChanged( object sender, WebBrowserProgressChangedEventArgs e ) {
      StatusLabel.Text = string.Format("Loading {0}/{1}", e.CurrentProgress, e.MaximumProgress);
    }

    private void _WebDisplay_Navigating( object sender, WebBrowserNavigatingEventArgs e ) {
      StatusAddress.Text = e.Url.AbsoluteUri;

    }
  }
}
