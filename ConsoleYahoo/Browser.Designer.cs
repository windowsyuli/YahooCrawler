﻿namespace ConsoleYahoo
{
    partial class Browser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.YahooBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // YahooBrowser
            // 
            this.YahooBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.YahooBrowser.Location = new System.Drawing.Point(0, 0);
            this.YahooBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.YahooBrowser.Name = "YahooBrowser";
            this.YahooBrowser.ScriptErrorsSuppressed = true;
            this.YahooBrowser.Size = new System.Drawing.Size(800, 480);
            this.YahooBrowser.TabIndex = 0;
            this.YahooBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.YahooBrowser_DocumentCompleted);
            // 
            // Browser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.YahooBrowser);
            this.Name = "Browser";
            this.Text = "FormBrowser";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser YahooBrowser;
    }
}