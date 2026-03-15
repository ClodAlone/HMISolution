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
    /// Control for managing text appearance options of EditControl.
    /// </summary>
    [ToolboxItem(false)]
    public class AppearanceTextOptions : BaseOptionsControl, IOptionsControl
    {
        #region Class Initialization & Finalization
        /// <summary>
        /// Initializes a new instance of the AppearanceTextOptions class.
        /// </summary>
        public AppearanceTextOptions()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private System.Windows.Forms.CheckBox chkTransparentSelection;
        private System.Windows.Forms.CheckBox chkOutliningCollapsers;
        private System.Windows.Forms.CheckBox chkIndentationGuidelines;
        private System.Windows.Forms.CheckBox chkLinesWrappingMarks;
        private System.Windows.Forms.CheckBox chkColumnGuides;
        private System.Windows.Forms.CheckBox chkIndentationBlockBorders;
        private System.Windows.Forms.CheckBox chkWrappedLinesMarks;

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AppearanceTextOptions));
            this.chkTransparentSelection = new System.Windows.Forms.CheckBox();
            this.chkOutliningCollapsers = new System.Windows.Forms.CheckBox();
            this.chkIndentationGuidelines = new System.Windows.Forms.CheckBox();
            this.chkLinesWrappingMarks = new System.Windows.Forms.CheckBox();
            this.chkColumnGuides = new System.Windows.Forms.CheckBox();
            this.chkIndentationBlockBorders = new System.Windows.Forms.CheckBox();
            this.chkWrappedLinesMarks = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkTransparentSelection
            // 
            this.chkTransparentSelection.AccessibleDescription = resources.GetString("chkTransparentSelection.AccessibleDescription");
            this.chkTransparentSelection.AccessibleName = resources.GetString("chkTransparentSelection.AccessibleName");
            this.chkTransparentSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkTransparentSelection.Anchor")));
            this.chkTransparentSelection.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkTransparentSelection.Appearance")));
            this.chkTransparentSelection.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkTransparentSelection.BackgroundImage")));
            this.chkTransparentSelection.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkTransparentSelection.CheckAlign")));
            this.chkTransparentSelection.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkTransparentSelection.Dock")));
            this.chkTransparentSelection.Enabled = ((bool)(resources.GetObject("chkTransparentSelection.Enabled")));
            this.chkTransparentSelection.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkTransparentSelection.FlatStyle")));
            this.chkTransparentSelection.Font = ((System.Drawing.Font)(resources.GetObject("chkTransparentSelection.Font")));
            this.chkTransparentSelection.Image = ((System.Drawing.Image)(resources.GetObject("chkTransparentSelection.Image")));
            this.chkTransparentSelection.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkTransparentSelection.ImageAlign")));
            this.chkTransparentSelection.ImageIndex = ((int)(resources.GetObject("chkTransparentSelection.ImageIndex")));
            this.chkTransparentSelection.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkTransparentSelection.ImeMode")));
            this.chkTransparentSelection.Location = ((System.Drawing.Point)(resources.GetObject("chkTransparentSelection.Location")));
            this.chkTransparentSelection.Name = "chkTransparentSelection";
            this.chkTransparentSelection.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkTransparentSelection.RightToLeft")));
            this.chkTransparentSelection.Size = ((System.Drawing.Size)(resources.GetObject("chkTransparentSelection.Size")));
            this.chkTransparentSelection.TabIndex = ((int)(resources.GetObject("chkTransparentSelection.TabIndex")));
            this.chkTransparentSelection.Text = resources.GetString("chkTransparentSelection.Text");
            this.chkTransparentSelection.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkTransparentSelection.TextAlign")));
            this.chkTransparentSelection.Visible = ((bool)(resources.GetObject("chkTransparentSelection.Visible")));
            this.chkTransparentSelection.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkOutliningCollapsers
            // 
            this.chkOutliningCollapsers.AccessibleDescription = resources.GetString("chkOutliningCollapsers.AccessibleDescription");
            this.chkOutliningCollapsers.AccessibleName = resources.GetString("chkOutliningCollapsers.AccessibleName");
            this.chkOutliningCollapsers.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkOutliningCollapsers.Anchor")));
            this.chkOutliningCollapsers.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkOutliningCollapsers.Appearance")));
            this.chkOutliningCollapsers.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkOutliningCollapsers.BackgroundImage")));
            this.chkOutliningCollapsers.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkOutliningCollapsers.CheckAlign")));
            this.chkOutliningCollapsers.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkOutliningCollapsers.Dock")));
            this.chkOutliningCollapsers.Enabled = ((bool)(resources.GetObject("chkOutliningCollapsers.Enabled")));
            this.chkOutliningCollapsers.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkOutliningCollapsers.FlatStyle")));
            this.chkOutliningCollapsers.Font = ((System.Drawing.Font)(resources.GetObject("chkOutliningCollapsers.Font")));
            this.chkOutliningCollapsers.Image = ((System.Drawing.Image)(resources.GetObject("chkOutliningCollapsers.Image")));
            this.chkOutliningCollapsers.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkOutliningCollapsers.ImageAlign")));
            this.chkOutliningCollapsers.ImageIndex = ((int)(resources.GetObject("chkOutliningCollapsers.ImageIndex")));
            this.chkOutliningCollapsers.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkOutliningCollapsers.ImeMode")));
            this.chkOutliningCollapsers.Location = ((System.Drawing.Point)(resources.GetObject("chkOutliningCollapsers.Location")));
            this.chkOutliningCollapsers.Name = "chkOutliningCollapsers";
            this.chkOutliningCollapsers.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkOutliningCollapsers.RightToLeft")));
            this.chkOutliningCollapsers.Size = ((System.Drawing.Size)(resources.GetObject("chkOutliningCollapsers.Size")));
            this.chkOutliningCollapsers.TabIndex = ((int)(resources.GetObject("chkOutliningCollapsers.TabIndex")));
            this.chkOutliningCollapsers.Text = resources.GetString("chkOutliningCollapsers.Text");
            this.chkOutliningCollapsers.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkOutliningCollapsers.TextAlign")));
            this.chkOutliningCollapsers.Visible = ((bool)(resources.GetObject("chkOutliningCollapsers.Visible")));
            this.chkOutliningCollapsers.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkIndentationGuidelines
            // 
            this.chkIndentationGuidelines.AccessibleDescription = resources.GetString("chkIndentationGuidelines.AccessibleDescription");
            this.chkIndentationGuidelines.AccessibleName = resources.GetString("chkIndentationGuidelines.AccessibleName");
            this.chkIndentationGuidelines.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkIndentationGuidelines.Anchor")));
            this.chkIndentationGuidelines.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkIndentationGuidelines.Appearance")));
            this.chkIndentationGuidelines.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkIndentationGuidelines.BackgroundImage")));
            this.chkIndentationGuidelines.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndentationGuidelines.CheckAlign")));
            this.chkIndentationGuidelines.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkIndentationGuidelines.Dock")));
            this.chkIndentationGuidelines.Enabled = ((bool)(resources.GetObject("chkIndentationGuidelines.Enabled")));
            this.chkIndentationGuidelines.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkIndentationGuidelines.FlatStyle")));
            this.chkIndentationGuidelines.Font = ((System.Drawing.Font)(resources.GetObject("chkIndentationGuidelines.Font")));
            this.chkIndentationGuidelines.Image = ((System.Drawing.Image)(resources.GetObject("chkIndentationGuidelines.Image")));
            this.chkIndentationGuidelines.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndentationGuidelines.ImageAlign")));
            this.chkIndentationGuidelines.ImageIndex = ((int)(resources.GetObject("chkIndentationGuidelines.ImageIndex")));
            this.chkIndentationGuidelines.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkIndentationGuidelines.ImeMode")));
            this.chkIndentationGuidelines.Location = ((System.Drawing.Point)(resources.GetObject("chkIndentationGuidelines.Location")));
            this.chkIndentationGuidelines.Name = "chkIndentationGuidelines";
            this.chkIndentationGuidelines.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkIndentationGuidelines.RightToLeft")));
            this.chkIndentationGuidelines.Size = ((System.Drawing.Size)(resources.GetObject("chkIndentationGuidelines.Size")));
            this.chkIndentationGuidelines.TabIndex = ((int)(resources.GetObject("chkIndentationGuidelines.TabIndex")));
            this.chkIndentationGuidelines.Text = resources.GetString("chkIndentationGuidelines.Text");
            this.chkIndentationGuidelines.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndentationGuidelines.TextAlign")));
            this.chkIndentationGuidelines.Visible = ((bool)(resources.GetObject("chkIndentationGuidelines.Visible")));
            this.chkIndentationGuidelines.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkLinesWrappingMarks
            // 
            this.chkLinesWrappingMarks.AccessibleDescription = resources.GetString("chkLinesWrappingMarks.AccessibleDescription");
            this.chkLinesWrappingMarks.AccessibleName = resources.GetString("chkLinesWrappingMarks.AccessibleName");
            this.chkLinesWrappingMarks.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkLinesWrappingMarks.Anchor")));
            this.chkLinesWrappingMarks.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkLinesWrappingMarks.Appearance")));
            this.chkLinesWrappingMarks.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkLinesWrappingMarks.BackgroundImage")));
            this.chkLinesWrappingMarks.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkLinesWrappingMarks.CheckAlign")));
            this.chkLinesWrappingMarks.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkLinesWrappingMarks.Dock")));
            this.chkLinesWrappingMarks.Enabled = ((bool)(resources.GetObject("chkLinesWrappingMarks.Enabled")));
            this.chkLinesWrappingMarks.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkLinesWrappingMarks.FlatStyle")));
            this.chkLinesWrappingMarks.Font = ((System.Drawing.Font)(resources.GetObject("chkLinesWrappingMarks.Font")));
            this.chkLinesWrappingMarks.Image = ((System.Drawing.Image)(resources.GetObject("chkLinesWrappingMarks.Image")));
            this.chkLinesWrappingMarks.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkLinesWrappingMarks.ImageAlign")));
            this.chkLinesWrappingMarks.ImageIndex = ((int)(resources.GetObject("chkLinesWrappingMarks.ImageIndex")));
            this.chkLinesWrappingMarks.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkLinesWrappingMarks.ImeMode")));
            this.chkLinesWrappingMarks.Location = ((System.Drawing.Point)(resources.GetObject("chkLinesWrappingMarks.Location")));
            this.chkLinesWrappingMarks.Name = "chkLinesWrappingMarks";
            this.chkLinesWrappingMarks.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkLinesWrappingMarks.RightToLeft")));
            this.chkLinesWrappingMarks.Size = ((System.Drawing.Size)(resources.GetObject("chkLinesWrappingMarks.Size")));
            this.chkLinesWrappingMarks.TabIndex = ((int)(resources.GetObject("chkLinesWrappingMarks.TabIndex")));
            this.chkLinesWrappingMarks.Text = resources.GetString("chkLinesWrappingMarks.Text");
            this.chkLinesWrappingMarks.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkLinesWrappingMarks.TextAlign")));
            this.chkLinesWrappingMarks.Visible = ((bool)(resources.GetObject("chkLinesWrappingMarks.Visible")));
            this.chkLinesWrappingMarks.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkColumnGuides
            // 
            this.chkColumnGuides.AccessibleDescription = resources.GetString("chkColumnGuides.AccessibleDescription");
            this.chkColumnGuides.AccessibleName = resources.GetString("chkColumnGuides.AccessibleName");
            this.chkColumnGuides.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkColumnGuides.Anchor")));
            this.chkColumnGuides.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkColumnGuides.Appearance")));
            this.chkColumnGuides.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkColumnGuides.BackgroundImage")));
            this.chkColumnGuides.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkColumnGuides.CheckAlign")));
            this.chkColumnGuides.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkColumnGuides.Dock")));
            this.chkColumnGuides.Enabled = ((bool)(resources.GetObject("chkColumnGuides.Enabled")));
            this.chkColumnGuides.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkColumnGuides.FlatStyle")));
            this.chkColumnGuides.Font = ((System.Drawing.Font)(resources.GetObject("chkColumnGuides.Font")));
            this.chkColumnGuides.Image = ((System.Drawing.Image)(resources.GetObject("chkColumnGuides.Image")));
            this.chkColumnGuides.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkColumnGuides.ImageAlign")));
            this.chkColumnGuides.ImageIndex = ((int)(resources.GetObject("chkColumnGuides.ImageIndex")));
            this.chkColumnGuides.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkColumnGuides.ImeMode")));
            this.chkColumnGuides.Location = ((System.Drawing.Point)(resources.GetObject("chkColumnGuides.Location")));
            this.chkColumnGuides.Name = "chkColumnGuides";
            this.chkColumnGuides.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkColumnGuides.RightToLeft")));
            this.chkColumnGuides.Size = ((System.Drawing.Size)(resources.GetObject("chkColumnGuides.Size")));
            this.chkColumnGuides.TabIndex = ((int)(resources.GetObject("chkColumnGuides.TabIndex")));
            this.chkColumnGuides.Text = resources.GetString("chkColumnGuides.Text");
            this.chkColumnGuides.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkColumnGuides.TextAlign")));
            this.chkColumnGuides.Visible = ((bool)(resources.GetObject("chkColumnGuides.Visible")));
            this.chkColumnGuides.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkIndentationBlockBorders
            // 
            this.chkIndentationBlockBorders.AccessibleDescription = resources.GetString("chkIndentationBlockBorders.AccessibleDescription");
            this.chkIndentationBlockBorders.AccessibleName = resources.GetString("chkIndentationBlockBorders.AccessibleName");
            this.chkIndentationBlockBorders.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkIndentationBlockBorders.Anchor")));
            this.chkIndentationBlockBorders.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkIndentationBlockBorders.Appearance")));
            this.chkIndentationBlockBorders.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkIndentationBlockBorders.BackgroundImage")));
            this.chkIndentationBlockBorders.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndentationBlockBorders.CheckAlign")));
            this.chkIndentationBlockBorders.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkIndentationBlockBorders.Dock")));
            this.chkIndentationBlockBorders.Enabled = ((bool)(resources.GetObject("chkIndentationBlockBorders.Enabled")));
            this.chkIndentationBlockBorders.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkIndentationBlockBorders.FlatStyle")));
            this.chkIndentationBlockBorders.Font = ((System.Drawing.Font)(resources.GetObject("chkIndentationBlockBorders.Font")));
            this.chkIndentationBlockBorders.Image = ((System.Drawing.Image)(resources.GetObject("chkIndentationBlockBorders.Image")));
            this.chkIndentationBlockBorders.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndentationBlockBorders.ImageAlign")));
            this.chkIndentationBlockBorders.ImageIndex = ((int)(resources.GetObject("chkIndentationBlockBorders.ImageIndex")));
            this.chkIndentationBlockBorders.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkIndentationBlockBorders.ImeMode")));
            this.chkIndentationBlockBorders.Location = ((System.Drawing.Point)(resources.GetObject("chkIndentationBlockBorders.Location")));
            this.chkIndentationBlockBorders.Name = "chkIndentationBlockBorders";
            this.chkIndentationBlockBorders.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkIndentationBlockBorders.RightToLeft")));
            this.chkIndentationBlockBorders.Size = ((System.Drawing.Size)(resources.GetObject("chkIndentationBlockBorders.Size")));
            this.chkIndentationBlockBorders.TabIndex = ((int)(resources.GetObject("chkIndentationBlockBorders.TabIndex")));
            this.chkIndentationBlockBorders.Text = resources.GetString("chkIndentationBlockBorders.Text");
            this.chkIndentationBlockBorders.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkIndentationBlockBorders.TextAlign")));
            this.chkIndentationBlockBorders.Visible = ((bool)(resources.GetObject("chkIndentationBlockBorders.Visible")));
            this.chkIndentationBlockBorders.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkWrappedLinesMarks
            // 
            this.chkWrappedLinesMarks.AccessibleDescription = resources.GetString("chkWrappedLinesMarks.AccessibleDescription");
            this.chkWrappedLinesMarks.AccessibleName = resources.GetString("chkWrappedLinesMarks.AccessibleName");
            this.chkWrappedLinesMarks.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkWrappedLinesMarks.Anchor")));
            this.chkWrappedLinesMarks.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkWrappedLinesMarks.Appearance")));
            this.chkWrappedLinesMarks.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkWrappedLinesMarks.BackgroundImage")));
            this.chkWrappedLinesMarks.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWrappedLinesMarks.CheckAlign")));
            this.chkWrappedLinesMarks.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkWrappedLinesMarks.Dock")));
            this.chkWrappedLinesMarks.Enabled = ((bool)(resources.GetObject("chkWrappedLinesMarks.Enabled")));
            this.chkWrappedLinesMarks.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkWrappedLinesMarks.FlatStyle")));
            this.chkWrappedLinesMarks.Font = ((System.Drawing.Font)(resources.GetObject("chkWrappedLinesMarks.Font")));
            this.chkWrappedLinesMarks.Image = ((System.Drawing.Image)(resources.GetObject("chkWrappedLinesMarks.Image")));
            this.chkWrappedLinesMarks.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWrappedLinesMarks.ImageAlign")));
            this.chkWrappedLinesMarks.ImageIndex = ((int)(resources.GetObject("chkWrappedLinesMarks.ImageIndex")));
            this.chkWrappedLinesMarks.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkWrappedLinesMarks.ImeMode")));
            this.chkWrappedLinesMarks.Location = ((System.Drawing.Point)(resources.GetObject("chkWrappedLinesMarks.Location")));
            this.chkWrappedLinesMarks.Name = "chkWrappedLinesMarks";
            this.chkWrappedLinesMarks.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkWrappedLinesMarks.RightToLeft")));
            this.chkWrappedLinesMarks.Size = ((System.Drawing.Size)(resources.GetObject("chkWrappedLinesMarks.Size")));
            this.chkWrappedLinesMarks.TabIndex = ((int)(resources.GetObject("chkWrappedLinesMarks.TabIndex")));
            this.chkWrappedLinesMarks.Text = resources.GetString("chkWrappedLinesMarks.Text");
            this.chkWrappedLinesMarks.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWrappedLinesMarks.TextAlign")));
            this.chkWrappedLinesMarks.Visible = ((bool)(resources.GetObject("chkWrappedLinesMarks.Visible")));
            this.chkWrappedLinesMarks.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // AppearanceTextOptions
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.chkTransparentSelection);
            this.Controls.Add(this.chkOutliningCollapsers);
            this.Controls.Add(this.chkIndentationGuidelines);
            this.Controls.Add(this.chkLinesWrappingMarks);
            this.Controls.Add(this.chkColumnGuides);
            this.Controls.Add(this.chkIndentationBlockBorders);
            this.Controls.Add(this.chkWrappedLinesMarks);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.Name = "AppearanceTextOptions";
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
            chkOutliningCollapsers.Checked = control.ShowOutliningCollapsers;
            chkLinesWrappingMarks.Checked = control.MarkLineWrapping;
            chkWrappedLinesMarks.Checked = control.MarkWrappedLines;
            chkIndentationBlockBorders.Checked = control.ShowIndentationBlockBorders;
            chkIndentationGuidelines.Checked = control.ShowIndentationGuidelines;
            chkColumnGuides.Checked = control.ShowColumnGuides;
            chkTransparentSelection.Checked = control.TransparentSelection;

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
                control.ShowOutliningCollapsers = chkOutliningCollapsers.Checked;
                control.MarkLineWrapping = chkLinesWrappingMarks.Checked;
                control.MarkWrappedLines = chkWrappedLinesMarks.Checked;
                control.ShowIndentationBlockBorders = chkIndentationBlockBorders.Checked;
                control.ShowIndentationGuidelines = chkIndentationGuidelines.Checked;
                control.ShowColumnGuides = chkColumnGuides.Checked;
                control.TransparentSelection = chkTransparentSelection.Checked;
            }

            return null;
        }
        #endregion
    }
}
