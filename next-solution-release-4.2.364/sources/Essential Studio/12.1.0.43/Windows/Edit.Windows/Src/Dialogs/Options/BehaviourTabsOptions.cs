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

using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs.Options
{
    /// <summary>
    /// Control for managing tabs behaviour of EditControl.
    /// </summary>
    [ToolboxItem(false)]
    public class BehaviourTabsOptions : BaseOptionsControl, IOptionsControl
    {
        #region Class Initialization & Finalization
        /// <summary>
        /// Initializes a new instance of the BehaviourTabsOptions class.
        /// </summary>
        public BehaviourTabsOptions()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
        }
        private System.Windows.Forms.TextBox txtTabSize;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkUseTabs;
        private System.Windows.Forms.CheckBox chkTabStops;
        private System.Windows.Forms.ComboBox comboIndentMode;
        private System.Windows.Forms.Label label2;

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(BehaviourTabsOptions));
            this.txtTabSize = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkUseTabs = new System.Windows.Forms.CheckBox();
            this.chkTabStops = new System.Windows.Forms.CheckBox();
            this.comboIndentMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtTabSize
            // 
            this.txtTabSize.AccessibleDescription = resources.GetString("txtTabSize.AccessibleDescription");
            this.txtTabSize.AccessibleName = resources.GetString("txtTabSize.AccessibleName");
            this.txtTabSize.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtTabSize.Anchor")));
            this.txtTabSize.AutoSize = ((bool)(resources.GetObject("txtTabSize.AutoSize")));
            this.txtTabSize.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtTabSize.BackgroundImage")));
            this.txtTabSize.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtTabSize.Dock")));
            this.txtTabSize.Enabled = ((bool)(resources.GetObject("txtTabSize.Enabled")));
            this.txtTabSize.Font = ((System.Drawing.Font)(resources.GetObject("txtTabSize.Font")));
            this.txtTabSize.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtTabSize.ImeMode")));
            this.txtTabSize.Location = ((System.Drawing.Point)(resources.GetObject("txtTabSize.Location")));
            this.txtTabSize.MaxLength = ((int)(resources.GetObject("txtTabSize.MaxLength")));
            this.txtTabSize.Multiline = ((bool)(resources.GetObject("txtTabSize.Multiline")));
            this.txtTabSize.Name = "txtTabSize";
            this.txtTabSize.PasswordChar = ((char)(resources.GetObject("txtTabSize.PasswordChar")));
            this.txtTabSize.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtTabSize.RightToLeft")));
            this.txtTabSize.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtTabSize.ScrollBars")));
            this.txtTabSize.Size = ((System.Drawing.Size)(resources.GetObject("txtTabSize.Size")));
            this.txtTabSize.TabIndex = ((int)(resources.GetObject("txtTabSize.TabIndex")));
            this.txtTabSize.Text = resources.GetString("txtTabSize.Text");
            this.txtTabSize.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtTabSize.TextAlign")));
            this.txtTabSize.Visible = ((bool)(resources.GetObject("txtTabSize.Visible")));
            this.txtTabSize.WordWrap = ((bool)(resources.GetObject("txtTabSize.WordWrap")));
            this.txtTabSize.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtTabSize_KeyPress);
            this.txtTabSize.TextChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // label1
            // 
            this.label1.AccessibleDescription = resources.GetString("label1.AccessibleDescription");
            this.label1.AccessibleName = resources.GetString("label1.AccessibleName");
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
            this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
            this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
            this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
            this.label1.Font = ((System.Drawing.Font)(resources.GetObject("label1.Font")));
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
            this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
            this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
            this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
            this.label1.Name = "label1";
            this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
            this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
            this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
            this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
            // 
            // chkUseTabs
            // 
            this.chkUseTabs.AccessibleDescription = resources.GetString("chkUseTabs.AccessibleDescription");
            this.chkUseTabs.AccessibleName = resources.GetString("chkUseTabs.AccessibleName");
            this.chkUseTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkUseTabs.Anchor")));
            this.chkUseTabs.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkUseTabs.Appearance")));
            this.chkUseTabs.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkUseTabs.BackgroundImage")));
            this.chkUseTabs.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUseTabs.CheckAlign")));
            this.chkUseTabs.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkUseTabs.Dock")));
            this.chkUseTabs.Enabled = ((bool)(resources.GetObject("chkUseTabs.Enabled")));
            this.chkUseTabs.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkUseTabs.FlatStyle")));
            this.chkUseTabs.Font = ((System.Drawing.Font)(resources.GetObject("chkUseTabs.Font")));
            this.chkUseTabs.Image = ((System.Drawing.Image)(resources.GetObject("chkUseTabs.Image")));
            this.chkUseTabs.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUseTabs.ImageAlign")));
            this.chkUseTabs.ImageIndex = ((int)(resources.GetObject("chkUseTabs.ImageIndex")));
            this.chkUseTabs.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkUseTabs.ImeMode")));
            this.chkUseTabs.Location = ((System.Drawing.Point)(resources.GetObject("chkUseTabs.Location")));
            this.chkUseTabs.Name = "chkUseTabs";
            this.chkUseTabs.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkUseTabs.RightToLeft")));
            this.chkUseTabs.Size = ((System.Drawing.Size)(resources.GetObject("chkUseTabs.Size")));
            this.chkUseTabs.TabIndex = ((int)(resources.GetObject("chkUseTabs.TabIndex")));
            this.chkUseTabs.Text = resources.GetString("chkUseTabs.Text");
            this.chkUseTabs.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUseTabs.TextAlign")));
            this.chkUseTabs.Visible = ((bool)(resources.GetObject("chkUseTabs.Visible")));
            this.chkUseTabs.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // chkTabStops
            // 
            this.chkTabStops.AccessibleDescription = resources.GetString("chkTabStops.AccessibleDescription");
            this.chkTabStops.AccessibleName = resources.GetString("chkTabStops.AccessibleName");
            this.chkTabStops.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkTabStops.Anchor")));
            this.chkTabStops.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkTabStops.Appearance")));
            this.chkTabStops.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkTabStops.BackgroundImage")));
            this.chkTabStops.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkTabStops.CheckAlign")));
            this.chkTabStops.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkTabStops.Dock")));
            this.chkTabStops.Enabled = ((bool)(resources.GetObject("chkTabStops.Enabled")));
            this.chkTabStops.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkTabStops.FlatStyle")));
            this.chkTabStops.Font = ((System.Drawing.Font)(resources.GetObject("chkTabStops.Font")));
            this.chkTabStops.Image = ((System.Drawing.Image)(resources.GetObject("chkTabStops.Image")));
            this.chkTabStops.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkTabStops.ImageAlign")));
            this.chkTabStops.ImageIndex = ((int)(resources.GetObject("chkTabStops.ImageIndex")));
            this.chkTabStops.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkTabStops.ImeMode")));
            this.chkTabStops.Location = ((System.Drawing.Point)(resources.GetObject("chkTabStops.Location")));
            this.chkTabStops.Name = "chkTabStops";
            this.chkTabStops.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkTabStops.RightToLeft")));
            this.chkTabStops.Size = ((System.Drawing.Size)(resources.GetObject("chkTabStops.Size")));
            this.chkTabStops.TabIndex = ((int)(resources.GetObject("chkTabStops.TabIndex")));
            this.chkTabStops.Text = resources.GetString("chkTabStops.Text");
            this.chkTabStops.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkTabStops.TextAlign")));
            this.chkTabStops.Visible = ((bool)(resources.GetObject("chkTabStops.Visible")));
            this.chkTabStops.CheckedChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // comboIndentMode
            // 
            this.comboIndentMode.AccessibleDescription = resources.GetString("comboIndentMode.AccessibleDescription");
            this.comboIndentMode.AccessibleName = resources.GetString("comboIndentMode.AccessibleName");
            this.comboIndentMode.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("comboIndentMode.Anchor")));
            this.comboIndentMode.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("comboIndentMode.BackgroundImage")));
            this.comboIndentMode.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("comboIndentMode.Dock")));
            this.comboIndentMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboIndentMode.Enabled = ((bool)(resources.GetObject("comboIndentMode.Enabled")));
            this.comboIndentMode.Font = ((System.Drawing.Font)(resources.GetObject("comboIndentMode.Font")));
            this.comboIndentMode.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("comboIndentMode.ImeMode")));
            this.comboIndentMode.IntegralHeight = ((bool)(resources.GetObject("comboIndentMode.IntegralHeight")));
            this.comboIndentMode.ItemHeight = ((int)(resources.GetObject("comboIndentMode.ItemHeight")));
            this.comboIndentMode.Items.AddRange(new object[] {
																										 resources.GetString("comboIndentMode.Items"),
																										 resources.GetString("comboIndentMode.Items1"),
																										 resources.GetString("comboIndentMode.Items2")});
            this.comboIndentMode.Location = ((System.Drawing.Point)(resources.GetObject("comboIndentMode.Location")));
            this.comboIndentMode.MaxDropDownItems = ((int)(resources.GetObject("comboIndentMode.MaxDropDownItems")));
            this.comboIndentMode.MaxLength = ((int)(resources.GetObject("comboIndentMode.MaxLength")));
            this.comboIndentMode.Name = "comboIndentMode";
            this.comboIndentMode.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("comboIndentMode.RightToLeft")));
            this.comboIndentMode.Size = ((System.Drawing.Size)(resources.GetObject("comboIndentMode.Size")));
            this.comboIndentMode.TabIndex = ((int)(resources.GetObject("comboIndentMode.TabIndex")));
            this.comboIndentMode.Text = resources.GetString("comboIndentMode.Text");
            this.comboIndentMode.Visible = ((bool)(resources.GetObject("comboIndentMode.Visible")));
            this.comboIndentMode.SelectedIndexChanged += new System.EventHandler(this.OptionsChanged);
            // 
            // label2
            // 
            this.label2.AccessibleDescription = resources.GetString("label2.AccessibleDescription");
            this.label2.AccessibleName = resources.GetString("label2.AccessibleName");
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label2.Anchor")));
            this.label2.AutoSize = ((bool)(resources.GetObject("label2.AutoSize")));
            this.label2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label2.Dock")));
            this.label2.Enabled = ((bool)(resources.GetObject("label2.Enabled")));
            this.label2.Font = ((System.Drawing.Font)(resources.GetObject("label2.Font")));
            this.label2.Image = ((System.Drawing.Image)(resources.GetObject("label2.Image")));
            this.label2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.ImageAlign")));
            this.label2.ImageIndex = ((int)(resources.GetObject("label2.ImageIndex")));
            this.label2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label2.ImeMode")));
            this.label2.Location = ((System.Drawing.Point)(resources.GetObject("label2.Location")));
            this.label2.Name = "label2";
            this.label2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label2.RightToLeft")));
            this.label2.Size = ((System.Drawing.Size)(resources.GetObject("label2.Size")));
            this.label2.TabIndex = ((int)(resources.GetObject("label2.TabIndex")));
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label2.TextAlign")));
            this.label2.Visible = ((bool)(resources.GetObject("label2.Visible")));
            // 
            // BehaviourTabsOptions
            // 
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboIndentMode);
            this.Controls.Add(this.chkTabStops);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkUseTabs);
            this.Controls.Add(this.txtTabSize);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.Name = "BehaviourTabsOptions";
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
            chkTabStops.Checked = control.UseTabStops;
            chkUseTabs.Checked = control.UseTabs;
            txtTabSize.Text = control.TabSize.ToString();

            switch (control.AutoIndentMode)
            {
                case AutoIndentMode.None:
                    comboIndentMode.SelectedIndex = 0;
                    break;

                case AutoIndentMode.Block:
                    comboIndentMode.SelectedIndex = 1;
                    break;

                case AutoIndentMode.Smart:
                    comboIndentMode.SelectedIndex = 2;
                    break;
            }

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
                int tab = int.MaxValue;
                try
                {
                    tab = int.Parse(txtTabSize.Text);
                    control.TabSize = tab;
                }
                catch (Exception e)
                {
                    if (e is ArgumentOutOfRangeException || e is OverflowException)
                    {
                        MessageBox.Show(
                        string.Format(ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_181, StreamEditControl.MIN_TAB_SIZE, StreamEditControl.MAX_TAB_SIZE), Localizer.ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (tab < StreamEditControl.MIN_TAB_SIZE)
                        {
                            control.TabSize = StreamEditControl.MIN_TAB_SIZE;
                        }
                        else
                        {
                            control.TabSize = StreamEditControl.MAX_TAB_SIZE;
                        }

                        txtTabSize.Text = control.TabSize.ToString();
                        return txtTabSize;
                    }
                    else
                    {
                        throw e;
                    }
                }

                control.UseTabStops = chkTabStops.Checked;
                control.UseTabs = chkUseTabs.Checked;

                switch (comboIndentMode.SelectedIndex)
                {
                    case 0:
                        control.AutoIndentMode = AutoIndentMode.None;
                        break;

                    case 1:
                        control.AutoIndentMode = AutoIndentMode.Block;
                        break;

                    case 2:
                        control.AutoIndentMode = AutoIndentMode.Smart;
                        break;
                }
            }

            return null;
        }
        #endregion

        #region Class Event Handlers
        /// <summary>
        /// Allows only numbers to be inserted.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void TxtTabSize_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != '\b'))
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}
