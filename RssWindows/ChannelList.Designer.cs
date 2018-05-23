namespace RssWindows {
  partial class ChannelList {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.listChannel = new System.Windows.Forms.ListView();
      this.SuspendLayout();
      // 
      // listChannel
      // 
      this.listChannel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.listChannel.Location = new System.Drawing.Point(0, 0);
      this.listChannel.Name = "listChannel";
      this.listChannel.Size = new System.Drawing.Size(311, 622);
      this.listChannel.TabIndex = 0;
      this.listChannel.UseCompatibleStateImageBehavior = false;
      // 
      // ChannelList
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(311, 622);
      this.Controls.Add(this.listChannel);
      this.Name = "ChannelList";
      this.Text = "ChannelList";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView listChannel;
  }
}