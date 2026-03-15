#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Edit.Dialogs.Options
{
    /// <summary>
    /// Control for managing EditControl appearance options.
    /// </summary>
    [ToolboxItem(false)]
    public class AppearanceControlOptions : BaseOptionsControl, IOptionsControl
    {
        #region Class Initialization & Filnalization
        /// <summary>
        /// Initializes a new instance of the AppearanceControlOptions class.
        /// </summary>
        public AppearanceControlOptions()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private System.Windows.Forms.CheckBox chkXPStyle;
        private System.Windows.Forms.CheckBox chkVerticalScrollbar;
        private System.Windows.Forms.CheckBox chkHorizontalScrollbar;
        private System.Windows.Forms.CheckBox chkStatusBar;

        #region Component Designer generated code
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AppearanceControlOptions));
            this.chkXPStyle = new System.Windows.Forms.CheckBox();
            this.chkVerticalScrollbar = new System.Windows.Forms.CheckBox();
            this.chkHorizontalScrollbar = new System.Windows.Forms.CheckBox();
            this.chkStatusBar = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkXPStyle
            // 
            this.chkXPStyle.AccessibleDescription = resources.GetString("chkXPStyle.AccessibleDescription");
            this.chkXPStyle.AccessibleName = resources.GetString("chkXPStyle.AccessibleName");
            this.chkXPStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkXPStyle.Anchor")));
            this.chkXPStyle.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkXPStyle.Appearance")));
            this.chkXPStyle.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkXPStyle.BackgroundImage")));
            this.chkXPStyle.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkXPStyle.CheckAlign")));
            this.chkXPStyle.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkXPStyle.Dock")));
            this.chkXPStyle.Enabled = ((bool)(resources.GetObject("chkXPStyle.Enabled")));
            this.chkXPStyle.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkXPStyle.FlatStyle")));
            this.chkXPStyle.Font = ((System.Drawing.Font)(resources.GetObject("chkXPStyle.Font")));
            this.chkXPStyle.Image = ((System.Drawing.Image)(resources.GetObject("chkXPStyle.Image")));
            this.chkXPStyle.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkXPStyle.ImageAlign")));
            this.chkXPStyle.ImageIndex = ((int)(resources.GetObject("chkXPStyle.ImageIndex")));
            this.chkXPStyle.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkXPStyle.ImeMode")));
            this.chkXPStyle.Location = ((System.Drawing.Point)(resources.GetObject("chkXPStyle.Location")));
            this.chkXPStyle.Name = "chkXPStyle";
            this.chkXPStyle.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkXPStyle.RightToLeft")));
            this.chkXPStyle.Size = ((System.Drawing.Size)(resources.GetObject("chkXPStyle.Size")));
            this.chkXPStyle.TabIndex = ((int)(resources.GetObject("chkXPStyle.TabIndex")));
            this.chkXPStyle.Text = resources.GetString("chkXPStyle.Text");
            this.chkXPStyle.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkXPStyle.TextAlign")));
            this.chkXPStyle.Visible = ((bool)(resources.GetObject("chkXPStyle.Visible")));
            this.chkXPStyle.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkVerticalScrollbar
            // 
            this.chkVerticalScrollbar.AccessibleDescription = resources.GetString("chkVerticalScrollbar.AccessibleDescription");
            this.chkVerticalScrollbar.AccessibleName = resources.GetString("chkVerticalScrollbar.AccessibleName");
            this.chkVerticalScrollbar.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkVerticalScrollbar.Anchor")));
            this.chkVerticalScrollbar.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkVerticalScrollbar.Appearance")));
            this.chkVerticalScrollbar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkVerticalScrollbar.BackgroundImage")));
            this.chkVerticalScrollbar.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkVerticalScrollbar.CheckAlign")));
            this.chkVerticalScrollbar.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkVerticalScrollbar.Dock")));
            this.chkVerticalScrollbar.Enabled = ((bool)(resources.GetObject("chkVerticalScrollbar.Enabled")));
            this.chkVerticalScrollbar.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkVerticalScrollbar.FlatStyle")));
            this.chkVerticalScrollbar.Font = ((System.Drawing.Font)(resources.GetObject("chkVerticalScrollbar.Font")));
            this.chkVerticalScrollbar.Image = ((System.Drawing.Image)(resources.GetObject("chkVerticalScrollbar.Image")));
            this.chkVerticalScrollbar.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkVerticalScrollbar.ImageAlign")));
            this.chkVerticalScrollbar.ImageIndex = ((int)(resources.GetObject("chkVerticalScrollbar.ImageIndex")));
            this.chkVerticalScrollbar.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkVerticalScrollbar.ImeMode")));
            this.chkVerticalScrollbar.Location = ((System.Drawing.Point)(resources.GetObject("chkVerticalScrollbar.Location")));
            this.chkVerticalScrollbar.Name = "chkVerticalScrollbar";
            this.chkVerticalScrollbar.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkVerticalScrollbar.RightToLeft")));
            this.chkVerticalScrollbar.Size = ((System.Drawing.Size)(resources.GetObject("chkVerticalScrollbar.Size")));
            this.chkVerticalScrollbar.TabIndex = ((int)(resources.GetObject("chkVerticalScrollbar.TabIndex")));
            this.chkVerticalScrollbar.Text = resources.GetString("chkVerticalScrollbar.Text");
            this.chkVerticalScrollbar.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkVerticalScrollbar.TextAlign")));
            this.chkVerticalScrollbar.Visible = ((bool)(resources.GetObject("chkVerticalScrollbar.Visible")));
            this.chkVerticalScrollbar.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkHorizontalScrollbar
            // 
            this.chkHorizontalScrollbar.AccessibleDescription = resources.GetString("chkHorizontalScrollbar.AccessibleDescription");
            this.chkHorizontalScrollbar.AccessibleName = resources.GetString("chkHorizontalScrollbar.AccessibleName");
            this.chkHorizontalScrollbar.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkHorizontalScrollbar.Anchor")));
            this.chkHorizontalScrollbar.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkHorizontalScrollbar.Appearance")));
            this.chkHorizontalScrollbar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkHorizontalScrollbar.BackgroundImage")));
            this.chkHorizontalScrollbar.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkHorizontalScrollbar.CheckAlign")));
            this.chkHorizontalScrollbar.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkHorizontalScrollbar.Dock")));
            this.chkHorizontalScrollbar.Enabled = ((bool)(resources.GetObject("chkHorizontalScrollbar.Enabled")));
            this.chkHorizontalScrollbar.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkHorizontalScrollbar.FlatStyle")));
            this.chkHorizontalScrollbar.Font = ((System.Drawing.Font)(resources.GetObject("chkHorizontalScrollbar.Font")));
            this.chkHorizontalScrollbar.Image = ((System.Drawing.Image)(resources.GetObject("chkHorizontalScrollbar.Image")));
            this.chkHorizontalScrollbar.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkHorizontalScrollbar.ImageAlign")));
            this.chkHorizontalScrollbar.ImageIndex = ((int)(resources.GetObject("chkHorizontalScrollbar.ImageIndex")));
            this.chkHorizontalScrollbar.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkHorizontalScrollbar.ImeMode")));
            this.chkHorizontalScrollbar.Location = ((System.Drawing.Point)(resources.GetObject("chkHorizontalScrollbar.Location")));
            this.chkHorizontalScrollbar.Name = "chkHorizontalScrollbar";
            this.chkHorizontalScrollbar.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkHorizontalScrollbar.RightToLeft")));
            this.chkHorizontalScrollbar.Size = ((System.Drawing.Size)(resources.GetObject("chkHorizontalScrollbar.Size")));
            this.chkHorizontalScrollbar.TabIndex = ((int)(resources.GetObject("chkHorizontalScrollbar.TabIndex")));
            this.chkHorizontalScrollbar.Text = resources.GetString("chkHorizontalScrollbar.Text");
            this.chkHorizontalScrollbar.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkHorizontalScrollbar.TextAlign")));
            this.chkHorizontalScrollbar.Visible = ((bool)(resources.GetObject("chkHorizontalScrollbar.Visible")));
            this.chkHorizontalScrollbar.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkStatusBar
            // 
            this.chkStatusBar.AccessibleDescription = resources.GetString("chkStatusBar.AccessibleDescription");
            this.chkStatusBar.AccessibleName = resources.GetString("chkStatusBar.AccessibleName");
            this.chkStatusBar.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkStatusBar.Anchor")));
            this.chkStatusBar.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkStatusBar.Appearance")));
            this.chkStatusBar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkStatusBar.BackgroundImage")));
            this.chkStatusBar.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkStatusBar.CheckAlign")));
            this.chkStatusBar.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkStatusBar.Dock")));
            this.chkStatusBar.Enabled = ((bool)(resources.GetObject("chkStatusBar.Enabled")));
            this.chkStatusBar.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkStatusBar.FlatStyle")));
            this.chkStatusBar.Font = ((System.Drawing.Font)(resources.GetObject("chkStatusBar.Font")));
            this.chkStatusBar.Image = ((System.Drawing.Image)(resources.GetObject("chkStatusBar.Image")));
            this.chkStatusBar.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkStatusBar.ImageAlign")));
            this.chkStatusBar.ImageIndex = ((int)(resources.GetObject("chkStatusBar.ImageIndex")));
            this.chkStatusBar.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkStatusBar.ImeMode")));
            this.chkStatusBar.Location = ((System.Drawing.Point)(resources.GetObject("chkStatusBar.Location")));
            this.chkStatusBar.Name = "chkStatusBar";
            this.chkStatusBar.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkStatusBar.RightToLeft")));
            this.chkStatusBar.Size = ((System.Drawing.Size)(resources.GetObject("chkStatusBar.Size")));
            this.chkStatusBar.TabIndex = ((int)(resources.GetObject("chkStatusBar.TabIndex")));
            this.chkStatusBar.Text = resources.GetString("chkStatusBar.Text");
            this.chkStatusBar.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkStatusBar.TextAlign")));
            this.chkStatusBar.Visible = ((bool)(resources.GetObject("chkStatusBar.Visible")));
            this.chkStatusBar.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // AppearanceControlOptions
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.chkXPStyle);
            this.Controls.Add(this.chkVerticalScrollbar);
            this.Controls.Add(this.chkHorizontalScrollbar);
            this.Controls.Add(this.chkStatusBar);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.Name = "AppearanceControlOptions";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
            this.ResumeLayout(false);

        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        #endregion

        #region IOptionsControl Members
        /// <summary>
        /// Initializes options control with data from EditControl.
        /// </summary>
        /// <param name="control">EditControl used for initializing.</param>
        public void Init(EditControl control)
        {
            chkHorizontalScrollbar.Checked = control.ShowHorizontalScroller;
            chkStatusBar.Checked = control.StatusBarSettings.Visible;
            chkVerticalScrollbar.Checked = control.ShowVerticalScroller;
            chkXPStyle.Checked = control.UseXPStyle;

            m_bChanged = false;
        }

        /// <summary>
        /// Applies set options to given EditControl.
        /// </summary>
        /// <param name="control">EditControl to apply options to.</param>
        /// <returns>Null if everything is OK; control to transfer focus to if error occured.</returns>
        public Control Apply(EditControl control)
        {
            if (m_bChanged)
            {
                control.ShowHorizontalScroller = chkHorizontalScrollbar.Checked;
                control.StatusBarSettings.Visible = chkStatusBar.Checked;
                control.ShowVerticalScroller = chkVerticalScrollbar.Checked;
                control.UseXPStyle = chkXPStyle.Checked;
            }

            return null;
        }
        #endregion
    }
}