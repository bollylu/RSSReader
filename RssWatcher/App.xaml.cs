using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using BLTools;
using System.Diagnostics;

namespace RssWatcher {
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {
    private void Application_Startup(object sender, StartupEventArgs e) {
      TimeStampTraceListener GlobalLog = new TimeStampTraceListener("rsswatcher.log", "global");
      GlobalLog.ResetLog();
      Trace.Listeners.Add(GlobalLog);
      Trace.IndentSize = 2;
      Trace.AutoFlush = true;
      Trace.WriteLine("=============== RSS Watcher startup ==============");
    }

    private void Application_Exit(object sender, ExitEventArgs e) {
      Trace.WriteLine("RSS watcher completed.");
      Trace.Close();
    }
  }
}
