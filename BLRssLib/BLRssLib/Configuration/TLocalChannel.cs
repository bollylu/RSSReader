using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BLTools;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BLRssLib {
  public class TLocalChannel : IToXml {

    #region Public constants
    public const string LocalChannelStorageNameExtension = "channel";
    #endregion Public constants

    #region Public properties
    public string Name { get; set; }

    public string StorageName {
      get {
        if (string.IsNullOrWhiteSpace(_StorageName)) {
          if (Channel == null || string.IsNullOrWhiteSpace(Channel.Link.ToString())) {
            _StorageName = string.Format("{0}.{1}", Guid.NewGuid().ToString(), LocalChannelStorageNameExtension);
          } else {
            _StorageName = string.Format("{0}.{1}", _CreateAuthorizedFilename(Channel.Link.Value.AbsoluteUri).Left(64), LocalChannelStorageNameExtension);
          }
        }
        return _StorageName;
      }
      set {
        _StorageName = value;
      }
    }
    private string _StorageName;

    public string StoragePath { get; set; }
    public string StorageFullName {
      get {
        return Path.Combine(StoragePath, StorageName);
      }
    }

    public bool Exists {
      get {
        return File.Exists(StorageName);
      }
    }
    public TChannel Channel { get; set; }
    #endregion Public properties

    #region Constructor(s)
    public TLocalChannel() {
      Name = "";
      StoragePath = "";
      Channel = new TChannel();
    }
    public TLocalChannel(TLocalChannel localChannel)
      : this() {
      Name = localChannel.Name;
      StoragePath = localChannel.StoragePath;
      Channel = new TChannel(localChannel.Channel);
    }
    public TLocalChannel(XElement localChannel)
      : this() {
      Name = localChannel.SafeReadAttribute<string>("name", "");
      if (localChannel.Elements().Any(x => x.Name == "storagename")) {
        StorageName = localChannel.SafeReadElementValue<string>("storagename", "");
      }
      if (localChannel.Elements().Any(x => x.Name == "storagepath")) {
        StoragePath = localChannel.SafeReadElementValue<string>("storagepath", "");
      }
      Load();
    }
    public TLocalChannel(TOpmlOutline opmlOutline)
      : this() {
      Name = opmlOutline.Title;
      Channel = new TChannel(opmlOutline);
    }
    #endregion Constructor(s)

    #region Converters
    public override string ToString() {
      StringBuilder RetVal = new StringBuilder();
      RetVal.AppendFormat("ConfigChannel: \"{0}\"", Name);
      RetVal.AppendFormat(", storage: \"{0}\"", StorageName);
      return RetVal.ToString();
    }
    public XElement ToXml() {
      XElement RetVal = new XElement("channel");
      RetVal.SetAttributeValue("name", Name);
      RetVal.SetElementValue("storagename", StorageName);
      RetVal.SetElementValue("storagepath", StoragePath);
      return RetVal;
    }
    #endregion Converters

    #region Events
    public event EventHandler LoadCompleted;
    public event EventHandler<ItemsDownloadedEventArgs> DownloadCompleted;
    #endregion Events

    #region Public methods
    public void Save() {
      Save(StorageName);
    }
    public void Save(string storageName) {
      if (string.IsNullOrWhiteSpace(storageName) || Path.GetFileNameWithoutExtension(storageName) == "") {
        Trace.WriteLine(string.Format("Unable to save Local Channel \"{0}\" : storage name is invalid ", StorageName));
      }
      XDocument DocumentToSave = new XDocument();
      DocumentToSave.Add(new XElement("Root"));
      DocumentToSave.Root.Add(Channel.ToXml());
      try {
        if (!Directory.Exists(StoragePath)) {
          Directory.CreateDirectory(StoragePath);
        }
        DocumentToSave.Save(Path.Combine(StoragePath, storageName));
      } catch (Exception ex) {
        Trace.WriteLine(string.Format("Unable to save Local Channel \"{0}\" : {1} ", storageName, ex.Message));
      }
    }

    public void Load() {
      Load(StorageName);
    }
    public void Load(string storageName) {
      try {
        Trace.Indent();
        XDocument DocumentToRead;
        try {
          DocumentToRead = XDocument.Load(Path.Combine(StoragePath, storageName));
        } catch (Exception ex) {
          Trace.WriteLine(string.Format("Unable to read Channel \"{0}\" : {1} ", StorageName, ex.Message));
          if (LoadCompleted != null) {
            LoadCompleted(this, EventArgs.Empty);
          }
          return;
        }

        Channel = new TChannel(DocumentToRead.Root.SafeReadElement("channel"));
        if (LoadCompleted != null) {
          LoadCompleted(this, EventArgs.Empty);
        }
      } finally {
        Trace.Unindent();
      }
    }

    public void LoadAsync() {
      Task.Factory.StartNew(() => Load(), TaskCreationOptions.None);
    }
    public void LoadAsync(string storageName) {
      Task LoadTask = Task.Factory.StartNew(() => Load(storageName), TaskCreationOptions.None);
    }

    /// <summary>
    /// Calls the DownloadItems of the channel. Once download is completed, throws an event
    /// </summary>
    public void DownloadItems() {
      if (Channel != null) {
        Channel.DownloadCompleted += new EventHandler<ItemsDownloadedEventArgs>(Channel_DownloadCompleted);
        Channel.DownloadItems();
      }
    }
    #endregion Public methods

    #region Private methods
    private void Channel_DownloadCompleted(object sender, ItemsDownloadedEventArgs e) {
      TChannel CurrentChannel = sender as TChannel;
      CurrentChannel.DownloadCompleted -= new EventHandler<ItemsDownloadedEventArgs>(Channel_DownloadCompleted);
      Save();
      if (DownloadCompleted != null) {
        ItemsDownloadedEventArgs Args = new ItemsDownloadedEventArgs(e.RawData, e.ResponseHeaders, e.Data);
        DownloadCompleted(this, Args);
      }
    }
    private string _CreateAuthorizedFilename(string source) {
      StringBuilder RetVal = new StringBuilder();
      List<char> InvalidChars = Path.GetInvalidFileNameChars().Union(Path.GetInvalidPathChars()).ToList();
      foreach (char charItem in source.ToLower()) {
        if (InvalidChars.Contains(charItem)) {
          RetVal.Append('-');
        } else {
          RetVal.Append(charItem);
        }
      }
      if (RetVal.ToString().StartsWith("http---")) {
        RetVal.Remove(0, 7);
      }
      return RetVal.ToString();
    }
    #endregion Private methods
  }
}
