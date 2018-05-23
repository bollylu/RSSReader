using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace RssWatcher {
  public class ListViewEx : ListView {
    public void ResizeLastColumn() {
      GridView CurrentGridView = this.View as GridView;

      double TotalSizeOfAllColumnsButLast = 0;
      foreach (GridViewColumn GridViewColumnItem in CurrentGridView.Columns) {
        TotalSizeOfAllColumnsButLast += GridViewColumnItem.ActualWidth;
      }

      Decorator ListViewBorder = this.GetVisualChild(0) as Decorator;
      ScrollViewer ListViewScrollViewer = ListViewBorder.Child as ScrollViewer;
      double ScrollbarWidth = ListViewScrollViewer.ScrollableHeight > 0 ? SystemParameters.VerticalScrollBarWidth : 0;

      GridViewColumn LastColumn = CurrentGridView.Columns[CurrentGridView.Columns.Count - 1];
      TotalSizeOfAllColumnsButLast -= LastColumn.ActualWidth;

      LastColumn.Width = this.ActualWidth - ScrollbarWidth - TotalSizeOfAllColumnsButLast - 12;
    }
  }
}
