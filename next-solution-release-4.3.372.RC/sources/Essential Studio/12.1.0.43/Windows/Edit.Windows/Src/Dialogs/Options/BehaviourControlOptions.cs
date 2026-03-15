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
    /// Control for managing EditControl behaviour.
    /// </summary>
    [ToolboxItem(false)]
    public class BehaviourControlOptions : BaseOptionsControl, IOptionsControl
    {
        #region Class Initialization & Finalization
        /// <summary>
        /// Initializes a new instance of the BehaviourControlOptions class.
        /// </summary>
        public BehaviourControlOptions()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }

        private System.Windows.Forms.CheckBox chkViewWhteSpace;
        private System.Windows.Forms.CheckBox chkGroupUndo;
        private System.Windows.Forms.CheckBox chkWordWrap;
        private System.Windows.Forms.CheckBox chkVirtualSpaceMode;
        private System.Windows.Forms.CheckBox chkInsertMode;

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BehaviourControlOptions));
            this.chkViewWhteSpace = new System.Windows.Forms.CheckBox();
            this.chkGroupUndo = new System.Windows.Forms.CheckBox();
            this.chkWordWrap = new System.Windows.Forms.CheckBox();
            this.chkVirtualSpaceMode = new System.Windows.Forms.CheckBox();
            this.chkInsertMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkViewWhteSpace
            // 
            this.chkViewWhteSpace.AccessibleDescription = resources.GetString("chkViewWhteSpace.AccessibleDescription");
            this.chkViewWhteSpace.AccessibleName = resources.GetString("chkViewWhteSpace.AccessibleName");
            this.chkViewWhteSpace.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkViewWhteSpace.Anchor")));
            this.chkViewWhteSpace.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkViewWhteSpace.Appearance")));
            this.chkViewWhteSpace.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkViewWhteSpace.BackgroundImage")));
            this.chkViewWhteSpace.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkViewWhteSpace.CheckAlign")));
            this.chkViewWhteSpace.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkViewWhteSpace.Dock")));
            this.chkViewWhteSpace.Enabled = ((bool)(resources.GetObject("chkViewWhteSpace.Enabled")));
            this.chkViewWhteSpace.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkViewWhteSpace.FlatStyle")));
            this.chkViewWhteSpace.Font = ((System.Drawing.Font)(resources.GetObject("chkViewWhteSpace.Font")));
            this.chkViewWhteSpace.Image = ((System.Drawing.Image)(resources.GetObject("chkViewWhteSpace.Image")));
            this.chkViewWhteSpace.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkViewWhteSpace.ImageAlign")));
            this.chkViewWhteSpace.ImageIndex = ((int)(resources.GetObject("chkViewWhteSpace.ImageIndex")));
            this.chkViewWhteSpace.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkViewWhteSpace.ImeMode")));
            this.chkViewWhteSpace.Location = ((System.Drawing.Point)(resources.GetObject("chkViewWhteSpace.Location")));
            this.chkViewWhteSpace.Name = "chkViewWhteSpace";
            this.chkViewWhteSpace.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkViewWhteSpace.RightToLeft")));
            this.chkViewWhteSpace.Size = ((System.Drawing.Size)(resources.GetObject("chkViewWhteSpace.Size")));
            this.chkViewWhteSpace.TabIndex = ((int)(resources.GetObject("chkViewWhteSpace.TabIndex")));
            this.chkViewWhteSpace.Text = resources.GetString("chkViewWhteSpace.Text");
            this.chkViewWhteSpace.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkViewWhteSpace.TextAlign")));
            this.chkViewWhteSpace.Visible = ((bool)(resources.GetObject("chkViewWhteSpace.Visible")));
            this.chkViewWhteSpace.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkGroupUndo
            // 
            this.chkGroupUndo.AccessibleDescription = resources.GetString("chkGroupUndo.AccessibleDescription");
            this.chkGroupUndo.AccessibleName = resources.GetString("chkGroupUndo.AccessibleName");
            this.chkGroupUndo.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkGroupUndo.Anchor")));
            this.chkGroupUndo.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkGroupUndo.Appearance")));
            this.chkGroupUndo.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkGroupUndo.BackgroundImage")));
            this.chkGroupUndo.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkGroupUndo.CheckAlign")));
            this.chkGroupUndo.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkGroupUndo.Dock")));
            this.chkGroupUndo.Enabled = ((bool)(resources.GetObject("chkGroupUndo.Enabled")));
            this.chkGroupUndo.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkGroupUndo.FlatStyle")));
            this.chkGroupUndo.Font = ((System.Drawing.Font)(resources.GetObject("chkGroupUndo.Font")));
            this.chkGroupUndo.Image = ((System.Drawing.Image)(resources.GetObject("chkGroupUndo.Image")));
            this.chkGroupUndo.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkGroupUndo.ImageAlign")));
            this.chkGroupUndo.ImageIndex = ((int)(resources.GetObject("chkGroupUndo.ImageIndex")));
            this.chkGroupUndo.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkGroupUndo.ImeMode")));
            this.chkGroupUndo.Location = ((System.Drawing.Point)(resources.GetObject("chkGroupUndo.Location")));
            this.chkGroupUndo.Name = "chkGroupUndo";
            this.chkGroupUndo.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkGroupUndo.RightToLeft")));
            this.chkGroupUndo.Size = ((System.Drawing.Size)(resources.GetObject("chkGroupUndo.Size")));
            this.chkGroupUndo.TabIndex = ((int)(resources.GetObject("chkGroupUndo.TabIndex")));
            this.chkGroupUndo.Text = resources.GetString("chkGroupUndo.Text");
            this.chkGroupUndo.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkGroupUndo.TextAlign")));
            this.chkGroupUndo.Visible = ((bool)(resources.GetObject("chkGroupUndo.Visible")));
            this.chkGroupUndo.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkWordWrap
            // 
            this.chkWordWrap.AccessibleDescription = resources.GetString("chkWordWrap.AccessibleDescription");
            this.chkWordWrap.AccessibleName = resources.GetString("chkWordWrap.AccessibleName");
            this.chkWordWrap.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkWordWrap.Anchor")));
            this.chkWordWrap.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkWordWrap.Appearance")));
            this.chkWordWrap.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkWordWrap.BackgroundImage")));
            this.chkWordWrap.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWordWrap.CheckAlign")));
            this.chkWordWrap.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkWordWrap.Dock")));
            this.chkWordWrap.Enabled = ((bool)(resources.GetObject("chkWordWrap.Enabled")));
            this.chkWordWrap.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkWordWrap.FlatStyle")));
            this.chkWordWrap.Font = ((System.Drawing.Font)(resources.GetObject("chkWordWrap.Font")));
            this.chkWordWrap.Image = ((System.Drawing.Image)(resources.GetObject("chkWordWrap.Image")));
            this.chkWordWrap.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWordWrap.ImageAlign")));
            this.chkWordWrap.ImageIndex = ((int)(resources.GetObject("chkWordWrap.ImageIndex")));
            this.chkWordWrap.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkWordWrap.ImeMode")));
            this.chkWordWrap.Location = ((System.Drawing.Point)(resources.GetObject("chkWordWrap.Location")));
            this.chkWordWrap.Name = "chkWordWrap";
            this.chkWordWrap.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkWordWrap.RightToLeft")));
            this.chkWordWrap.Size = ((System.Drawing.Size)(resources.GetObject("chkWordWrap.Size")));
            this.chkWordWrap.TabIndex = ((int)(resources.GetObject("chkWordWrap.TabIndex")));
            this.chkWordWrap.Text = resources.GetString("chkWordWrap.Text");
            this.chkWordWrap.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWordWrap.TextAlign")));
            this.chkWordWrap.Visible = ((bool)(resources.GetObject("chkWordWrap.Visible")));
            this.chkWordWrap.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkVirtualSpaceMode
            // 
            this.chkVirtualSpaceMode.AccessibleDescription = resources.GetString("chkVirtualSpaceMode.AccessibleDescription");
            this.chkVirtualSpaceMode.AccessibleName = resources.GetString("chkVirtualSpaceMode.AccessibleName");
            this.chkVirtualSpaceMode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkVirtualSpaceMode.Anchor")));
            this.chkVirtualSpaceMode.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkVirtualSpaceMode.Appearance")));
            this.chkVirtualSpaceMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkVirtualSpaceMode.BackgroundImage")));
            this.chkVirtualSpaceMode.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkVirtualSpaceMode.CheckAlign")));
            this.chkVirtualSpaceMode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkVirtualSpaceMode.Dock")));
            this.chkVirtualSpaceMode.Enabled = ((bool)(resources.GetObject("chkVirtualSpaceMode.Enabled")));
            this.chkVirtualSpaceMode.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkVirtualSpaceMode.FlatStyle")));
            this.chkVirtualSpaceMode.Font = ((System.Drawing.Font)(resources.GetObject("chkVirtualSpaceMode.Font")));
            this.chkVirtualSpaceMode.Image = ((System.Drawing.Image)(resources.GetObject("chkVirtualSpaceMode.Image")));
            this.chkVirtualSpaceMode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkVirtualSpaceMode.ImageAlign")));
            this.chkVirtualSpaceMode.ImageIndex = ((int)(resources.GetObject("chkVirtualSpaceMode.ImageIndex")));
            this.chkVirtualSpaceMode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkVirtualSpaceMode.ImeMode")));
            this.chkVirtualSpaceMode.Location = ((System.Drawing.Point)(resources.GetObject("chkVirtualSpaceMode.Location")));
            this.chkVirtualSpaceMode.Name = "chkVirtualSpaceMode";
            this.chkVirtualSpaceMode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkVirtualSpaceMode.RightToLeft")));
            this.chkVirtualSpaceMode.Size = ((System.Drawing.Size)(resources.GetObject("chkVirtualSpaceMode.Size")));
            this.chkVirtualSpaceMode.TabIndex = ((int)(resources.GetObject("chkVirtualSpaceMode.TabIndex")));
            this.chkVirtualSpaceMode.Text = resources.GetString("chkVirtualSpaceMode.Text");
            this.chkVirtualSpaceMode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkVirtualSpaceMode.TextAlign")));
            this.chkVirtualSpaceMode.Visible = ((bool)(resources.GetObject("chkVirtualSpaceMode.Visible")));
            this.chkVirtualSpaceMode.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkInsertMode
            // 
            this.chkInsertMode.AccessibleDescription = resources.GetString("chkInsertMode.AccessibleDescription");
            this.chkInsertMode.AccessibleName = resources.GetString("chkInsertMode.AccessibleName");
            this.chkInsertMode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkInsertMode.Anchor")));
            this.chkInsertMode.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkInsertMode.Appearance")));
            this.chkInsertMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkInsertMode.BackgroundImage")));
            this.chkInsertMode.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkInsertMode.CheckAlign")));
            this.chkInsertMode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkInsertMode.Dock")));
            this.chkInsertMode.Enabled = ((bool)(resources.GetObject("chkInsertMode.Enabled")));
            this.chkInsertMode.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkInsertMode.FlatStyle")));
            this.chkInsertMode.Font = ((System.Drawing.Font)(resources.GetObject("chkInsertMode.Font")));
            this.chkInsertMode.Image = ((System.Drawing.Image)(resources.GetObject("chkInsertMode.Image")));
            this.chkInsertMode.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkInsertMode.ImageAlign")));
            this.chkInsertMode.ImageIndex = ((int)(resources.GetObject("chkInsertMode.ImageIndex")));
            this.chkInsertMode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkInsertMode.ImeMode")));
            this.chkInsertMode.Location = ((System.Drawing.Point)(resources.GetObject("chkInsertMode.Location")));
            this.chkInsertMode.Name = "chkInsertMode";
            this.chkInsertMode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkInsertMode.RightToLeft")));
            this.chkInsertMode.Size = ((System.Drawing.Size)(resources.GetObject("chkInsertMode.Size")));
            this.chkInsertMode.TabIndex = ((int)(resources.GetObject("chkInsertMode.TabIndex")));
            this.chkInsertMode.Text = resources.GetString("chkInsertMode.Text");
            this.chkInsertMode.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkInsertMode.TextAlign")));
            this.chkInsertMode.Visible = ((bool)(resources.GetObject("chkInsertMode.Visible")));
            this.chkInsertMode.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // BehaviourControlOptions
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.chkViewWhteSpace);
            this.Controls.Add(this.chkGroupUndo);
            this.Controls.Add(this.chkWordWrap);
            this.Controls.Add(this.chkVirtualSpaceMode);
            this.Controls.Add(this.chkInsertMode);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.Name = "BehaviourControlOptions";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.Size = ((System.Drawing.Size)(resources.GetObject("$this.Size")));
            this.ResumeLayout(false);

        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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
            chkInsertMode.Checked = control.InsertMode;
            chkGroupUndo.Checked = control.GroupUndo;
            chkViewWhteSpace.Checked = control.ShowWhitespaces;
            chkVirtualSpaceMode.Checked = control.VirtualSpaceMode;
            chkWordWrap.Checked = control.WordWrap;

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
                control.InsertMode = chkInsertMode.Checked;
                control.GroupUndo = chkGroupUndo.Checked;
                control.ShowWhitespaces = chkViewWhteSpace.Checked;
                control.VirtualSpaceMode = chkVirtualSpaceMode.Checked;
                control.WordWrap = chkWordWrap.Checked;
            }

            return null;
        }
        #endregion
    }
}
