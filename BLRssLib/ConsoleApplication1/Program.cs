using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLRssLib;
using BLTools;
using System.Xml.Linq;

namespace ConsoleApplication1 {
  class Program {
    static void Main( string[] args ) {
      SplitArgs oArgs = new SplitArgs(args);

      string SourceOpml = oArgs.GetValue<string>("source");
      TOpmlDocument ImportDocument = new TOpmlDocument(SourceOpml);
      Console.WriteLine(ImportDocument.Outlines.ToString(true));
      ConsoleExtension.Pause();

      TConfigDocument ConfigDocument = new TConfigDocument();
      ConfigDocument.Import(SourceOpml);
      Console.WriteLine(ConfigDocument.ToString());
      ConsoleExtension.Pause();
    }
  }
}
