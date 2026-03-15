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
    /// Control for managing areas (margins) appearance options of EditControl.
    /// </summary>
    [ToolboxItem(false)]
    public class AppearanceAreasOptions : BaseOptionsControl, IOptionsControl
    {
        #region Class Initialization & Finalization
        /// <summary>
        /// Initializes a new instance of the AppearanceAreasOptions class.
        /// </summary>
        public AppearanceAreasOptions()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private System.Windows.Forms.CheckBox chkUserMargin;
        private System.Windows.Forms.CheckBox chkSelectionMargin;
        private System.Windows.Forms.CheckBox chkIndicatorMargin;
        private System.Windows.Forms.CheckBox chkLineNumbers;
        private System.Windows.Forms.CheckBox chkWordWrapMargin;
        private System.Windows.Forms.CheckBox chkChangedLinesMarking;

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AppearanceAreasOptions));
            this.chkUserMargin = new System.Windows.Forms.CheckBox();
            this.chkSelectionMargin = new System.Windows.Forms.CheckBox();
            this.chkIndicatorMargin = new System.Windows.Forms.CheckBox();
            this.chkLineNumbers = new System.Windows.Forms.CheckBox();
            this.chkWordWrapMargin = new System.Windows.Forms.CheckBox();
            this.chkChangedLinesMarking = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkUserMargin
            // 
            this.chkUserMargin.AccessibleDescription = resources.GetString("chkUserMargin.AccessibleDescription");
            this.chkUserMargin.AccessibleName = resources.GetString("chkUserMargin.AccessibleName");
            this.chkUserMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkUserMargin.Anchor")));
            this.chkUserMargin.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkUserMargin.Appearance")));
            this.chkUserMargin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkUserMargin.BackgroundImage")));
            this.chkUserMargin.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUserMargin.CheckAlign")));
            this.chkUserMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkUserMargin.Dock")));
            this.chkUserMargin.Enabled = ((bool)(resources.GetObject("chkUserMargin.Enabled")));
            this.chkUserMargin.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkUserMargin.FlatStyle")));
            this.chkUserMargin.Font = ((System.Drawing.Font)(resources.GetObject("chkUserMargin.Font")));
            this.chkUserMargin.Image = ((System.Drawing.Image)(resources.GetObject("chkUserMargin.Image")));
            this.chkUserMargin.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUserMargin.ImageAlign")));
            this.chkUserMargin.ImageIndex = ((int)(resources.GetObject("chkUserMargin.ImageIndex")));
            this.chkUserMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkUserMargin.ImeMode")));
            this.chkUserMargin.Location = ((System.Drawing.Point)(resources.GetObject("chkUserMargin.Location")));
            this.chkUserMargin.Name = "chkUserMargin";
            this.chkUserMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkUserMargin.RightToLeft")));
            this.chkUserMargin.Size = ((System.Drawing.Size)(resources.GetObject("chkUserMargin.Size")));
            this.chkUserMargin.TabIndex = ((int)(resources.GetObject("chkUserMargin.TabIndex")));
            this.chkUserMargin.Text = resources.GetString("chkUserMargin.Text");
            this.chkUserMargin.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUserMargin.TextAlign")));
            this.chkUserMargin.Visible = ((bool)(resources.GetObject("chkUserMargin.Visible")));
            this.chkUserMargin.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkSelectionMargin
            // 
            this.chkSelectionMargin.AccessibleDescription = resources.GetString("chkSelectionMargin.AccessibleDescription");
            this.chkSelectionMargin.AccessibleName = resources.GetString("chkSelectionMargin.AccessibleName");
            this.chkSelectionMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkSelectionMargin.Anchor")));
            this.chkSelectionMargin.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkSelectionMargin.Appearance")));
            this.chkSelectionMargin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkSelectionMargin.BackgroundImage")));
            this.chkSelectionMargin.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkSelectionMargin.CheckAlign")));
            this.chkSelectionMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkSelectionMargin.Dock")));
            this.chkSelectionMargin.Enabled = ((bool)(resources.GetObject("chkSelectionMargin.Enabled")));
            this.chkSelectionMargin.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkSelectionMargin.FlatStyle")));
            this.chkSelectionMargin.Font = ((System.Drawing.Font)(resources.GetObject("chkSelectionMargin.Font")));
            this.chkSelectionMargin.Image = ((System.Drawing.Image)(resources.GetObject("chkSelectionMargin.Image")));
            this.chkSelectionMargin.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkSelectionMargin.ImageAlign")));
            this.chkSelectionMargin.ImageIndex = ((int)(resources.GetObject("chkSelectionMargin.ImageIndex")));
            this.chkSelectionMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkSelectionMargin.ImeMode")));
            this.chkSelectionMargin.Location = ((System.Drawing.Point)(resources.GetObject("chkSelectionMargin.Location")));
            this.chkSelectionMargin.Name = "chkSelectionMargin";
            this.chkSelectionMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkSelectionMargin.RightToLeft")));
            this.chkSelectionMargin.Size = ((System.Drawing.Size)(resources.GetObject("chkSelectionMargin.Size")));
            this.chkSelectionMargin.TabIndex = ((int)(resources.GetObject("chkSelectionMargin.TabIndex")));
            this.chkSelectionMargin.Text = resources.GetString("chkSelectionMargin.Text");
            this.chkSelectionMargin.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkSelectionMargin.TextAlign")));
            this.chkSelectionMargin.Visible = ((bool)(resources.GetObject("chkSelectionMargin.Visible")));
            this.chkSelectionMargin.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkIndicatorMargin
            // 
            this.chkIndicatorMargin.AccessibleDescription = resources.GetString("chkIndicatorMargin.AccessibleDescription");
            this.chkIndicatorMargin.AccessibleName = resources.GetString("chkIndicatorMargin.AccessibleName");
            this.chkIndicatorMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkIndicatorMargin.Anchor")));
            this.chkIndicatorMargin.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkIndicatorMargin.Appearance")));
            this.chkIndicatorMargin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkIndicatorMargin.BackgroundImage")));
            this.chkIndicatorMargin.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndicatorMargin.CheckAlign")));
            this.chkIndicatorMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkIndicatorMargin.Dock")));
            this.chkIndicatorMargin.Enabled = ((bool)(resources.GetObject("chkIndicatorMargin.Enabled")));
            this.chkIndicatorMargin.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkIndicatorMargin.FlatStyle")));
            this.chkIndicatorMargin.Font = ((System.Drawing.Font)(resources.GetObject("chkIndicatorMargin.Font")));
            this.chkIndicatorMargin.Image = ((System.Drawing.Image)(resources.GetObject("chkIndicatorMargin.Image")));
            this.chkIndicatorMargin.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndicatorMargin.ImageAlign")));
            this.chkIndicatorMargin.ImageIndex = ((int)(resources.GetObject("chkIndicatorMargin.ImageIndex")));
            this.chkIndicatorMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkIndicatorMargin.ImeMode")));
            this.chkIndicatorMargin.Location = ((System.Drawing.Point)(resources.GetObject("chkIndicatorMargin.Location")));
            this.chkIndicatorMargin.Name = "chkIndicatorMargin";
            this.chkIndicatorMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkIndicatorMargin.RightToLeft")));
            this.chkIndicatorMargin.Size = ((System.Drawing.Size)(resources.GetObject("chkIndicatorMargin.Size")));
            this.chkIndicatorMargin.TabIndex = ((int)(resources.GetObject("chkIndicatorMargin.TabIndex")));
            this.chkIndicatorMargin.Text = resources.GetString("chkIndicatorMargin.Text");
            this.chkIndicatorMargin.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndicatorMargin.TextAlign")));
            this.chkIndicatorMargin.Visible = ((bool)(resources.GetObject("chkIndicatorMargin.Visible")));
            this.chkIndicatorMargin.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkLineNumbers
            // 
            this.chkLineNumbers.AccessibleDescription = resources.GetString("chkLineNumbers.AccessibleDescription");
            this.chkLineNumbers.AccessibleName = resources.GetString("chkLineNumbers.AccessibleName");
            this.chkLineNumbers.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkLineNumbers.Anchor")));
            this.chkLineNumbers.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkLineNumbers.Appearance")));
            this.chkLineNumbers.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkLineNumbers.BackgroundImage")));
            this.chkLineNumbers.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkLineNumbers.CheckAlign")));
            this.chkLineNumbers.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkLineNumbers.Dock")));
            this.chkLineNumbers.Enabled = ((bool)(resources.GetObject("chkLineNumbers.Enabled")));
            this.chkLineNumbers.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkLineNumbers.FlatStyle")));
            this.chkLineNumbers.Font = ((System.Drawing.Font)(resources.GetObject("chkLineNumbers.Font")));
            this.chkLineNumbers.Image = ((System.Drawing.Image)(resources.GetObject("chkLineNumbers.Image")));
            this.chkLineNumbers.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkLineNumbers.ImageAlign")));
            this.chkLineNumbers.ImageIndex = ((int)(resources.GetObject("chkLineNumbers.ImageIndex")));
            this.chkLineNumbers.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkLineNumbers.ImeMode")));
            this.chkLineNumbers.Location = ((System.Drawing.Point)(resources.GetObject("chkLineNumbers.Location")));
            this.chkLineNumbers.Name = "chkLineNumbers";
            this.chkLineNumbers.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkLineNumbers.RightToLeft")));
            this.chkLineNumbers.Size = ((System.Drawing.Size)(resources.GetObject("chkLineNumbers.Size")));
            this.chkLineNumbers.TabIndex = ((int)(resources.GetObject("chkLineNumbers.TabIndex")));
            this.chkLineNumbers.Text = resources.GetString("chkLineNumbers.Text");
            this.chkLineNumbers.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkLineNumbers.TextAlign")));
            this.chkLineNumbers.Visible = ((bool)(resources.GetObject("chkLineNumbers.Visible")));
            this.chkLineNumbers.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkWordWrapMargin
            // 
            this.chkWordWrapMargin.AccessibleDescription = resources.GetString("chkWordWrapMargin.AccessibleDescription");
            this.chkWordWrapMargin.AccessibleName = resources.GetString("chkWordWrapMargin.AccessibleName");
            this.chkWordWrapMargin.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkWordWrapMargin.Anchor")));
            this.chkWordWrapMargin.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkWordWrapMargin.Appearance")));
            this.chkWordWrapMargin.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkWordWrapMargin.BackgroundImage")));
            this.chkWordWrapMargin.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWordWrapMargin.CheckAlign")));
            this.chkWordWrapMargin.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkWordWrapMargin.Dock")));
            this.chkWordWrapMargin.Enabled = ((bool)(resources.GetObject("chkWordWrapMargin.Enabled")));
            this.chkWordWrapMargin.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkWordWrapMargin.FlatStyle")));
            this.chkWordWrapMargin.Font = ((System.Drawing.Font)(resources.GetObject("chkWordWrapMargin.Font")));
            this.chkWordWrapMargin.Image = ((System.Drawing.Image)(resources.GetObject("chkWordWrapMargin.Image")));
            this.chkWordWrapMargin.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWordWrapMargin.ImageAlign")));
            this.chkWordWrapMargin.ImageIndex = ((int)(resources.GetObject("chkWordWrapMargin.ImageIndex")));
            this.chkWordWrapMargin.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkWordWrapMargin.ImeMode")));
            this.chkWordWrapMargin.Location = ((System.Drawing.Point)(resources.GetObject("chkWordWrapMargin.Location")));
            this.chkWordWrapMargin.Name = "chkWordWrapMargin";
            this.chkWordWrapMargin.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkWordWrapMargin.RightToLeft")));
            this.chkWordWrapMargin.Size = ((System.Drawing.Size)(resources.GetObject("chkWordWrapMargin.Size")));
            this.chkWordWrapMargin.TabIndex = ((int)(resources.GetObject("chkWordWrapMargin.TabIndex")));
            this.chkWordWrapMargin.Text = resources.GetString("chkWordWrapMargin.Text");
            this.chkWordWrapMargin.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWordWrapMargin.TextAlign")));
            this.chkWordWrapMargin.Visible = ((bool)(resources.GetObject("chkWordWrapMargin.Visible")));
            this.chkWordWrapMargin.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkChangedLinesMarking
            // 
            this.chkChangedLinesMarking.AccessibleDescription = resources.GetString("chkChangedLinesMarking.AccessibleDescription");
            this.chkChangedLinesMarking.AccessibleName = resources.GetString("chkChangedLinesMarking.AccessibleName");
            this.chkChangedLinesMarking.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkChangedLinesMarking.Anchor")));
            this.chkChangedLinesMarking.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkChangedLinesMarking.Appearance")));
            this.chkChangedLinesMarking.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkChangedLinesMarking.BackgroundImage")));
            this.chkChangedLinesMarking.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkChangedLinesMarking.CheckAlign")));
            this.chkChangedLinesMarking.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkChangedLinesMarking.Dock")));
            this.chkChangedLinesMarking.Enabled = ((bool)(resources.GetObject("chkChangedLinesMarking.Enabled")));
            this.chkChangedLinesMarking.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkChangedLinesMarking.FlatStyle")));
            this.chkChangedLinesMarking.Font = ((System.Drawing.Font)(resources.GetObject("chkChangedLinesMarking.Font")));
            this.chkChangedLinesMarking.Image = ((System.Drawing.Image)(resources.GetObject("chkChangedLinesMarking.Image")));
            this.chkChangedLinesMarking.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkChangedLinesMarking.ImageAlign")));
            this.chkChangedLinesMarking.ImageIndex = ((int)(resources.GetObject("chkChangedLinesMarking.ImageIndex")));
            this.chkChangedLinesMarking.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkChangedLinesMarking.ImeMode")));
            this.chkChangedLinesMarking.Location = ((System.Drawing.Point)(resources.GetObject("chkChangedLinesMarking.Location")));
            this.chkChangedLinesMarking.Name = "chkChangedLinesMarking";
            this.chkChangedLinesMarking.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkChangedLinesMarking.RightToLeft")));
            this.chkChangedLinesMarking.Size = ((System.Drawing.Size)(resources.GetObject("chkChangedLinesMarking.Size")));
            this.chkChangedLinesMarking.TabIndex = ((int)(resources.GetObject("chkChangedLinesMarking.TabIndex")));
            this.chkChangedLinesMarking.Text = resources.GetString("chkChangedLinesMarking.Text");
            this.chkChangedLinesMarking.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkChangedLinesMarking.TextAlign")));
            this.chkChangedLinesMarking.Visible = ((bool)(resources.GetObject("chkChangedLinesMarking.Visible")));
            this.chkChangedLinesMarking.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // AppearanceAreasOptions
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.chkUserMargin);
            this.Controls.Add(this.chkSelectionMargin);
            this.Controls.Add(this.chkIndicatorMargin);
            this.Controls.Add(this.chkLineNumbers);
            this.Controls.Add(this.chkWordWrapMargin);
            this.Controls.Add(this.chkChangedLinesMarking);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.Name = "AppearanceAreasOptions";
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
            chkIndicatorMargin.Checked = control.ShowIndicatorMargin;
            chkSelectionMargin.Checked = control.ShowSelectionMargin;
            chkUserMargin.Checked = control.ShowUserMargin;
            chkWordWrapMargin.Checked = control.WordWrapMarginVisible;
            chkLineNumbers.Checked = control.ShowLineNumbers;
            chkChangedLinesMarking.Checked = control.MarkChangedLines;

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
                control.ShowIndicatorMargin = chkIndicatorMargin.Checked;
                control.ShowSelectionMargin = chkSelectionMargin.Checked;
                control.ShowUserMargin = chkUserMargin.Checked;
                control.WordWrapMarginVisible = chkWordWrapMargin.Checked;
                control.ShowLineNumbers = chkLineNumbers.Checked;
                control.MarkChangedLines = chkChangedLinesMarking.Checked;
            }

            return null;
        }
        #endregion
    }
}
