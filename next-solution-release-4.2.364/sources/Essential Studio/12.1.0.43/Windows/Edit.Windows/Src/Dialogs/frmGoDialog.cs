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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
    /// <summary>
    /// Dialog for jumping to defined line in the code.
    /// </summary>
    public class FrmGoDialog
        : System.Windows.Forms.Form, IGotoDialogForm
    {
        #region Constants
        /// <summary>
        /// Default text on the dialog.
        /// </summary>
        public static string DEF_FORMAT = Localizer.DEF_GOTO_CAPTION_FORMAT;
        #endregion

        #region Fields
        /// <summary>
        /// Minimum line number.
        /// </summary>
        private int m_iMinNumber = 1;

        /// <summary>
        /// Maximum line number.
        /// </summary>
        private int m_iMaxNumber = 1;

        /// <summary>
        /// Text on the dialog.
        /// </summary>
        private string m_strLabel = DEF_FORMAT;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets text on the dialog.
        /// </summary>
        [Category("Appearance"), Localizable(true), Browsable(true), Description("DescrLineLable")]
        public string LineLabelFormat
        {
            get
            {
                return m_strLabel;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("LineLabelFormat");

                if (value.Length == 0)
                    throw new ArgumentException(Syncfusion.Windows.Forms.Localization.ExceptionTextLocalizer.DEF_EXCEPTION_UNNAMED_102);

                if (value != m_strLabel)
                {
                    m_strLabel = value;
                    UpdateLabel();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum line number.
        /// </summary>
        public int MaxLine
        {
            get
            {
                return m_iMaxNumber;
            }
            set
            {
                if (value != m_iMaxNumber)
                {
                    m_iMaxNumber = value;
                    UpdateLabel();
                }
            }
        }

        /// <summary>
        /// Gets or sets minimum line number.
        /// </summary>
        public int MinLine
        {
            get
            {
                return m_iMinNumber;
            }
            set
            {
                if (value != m_iMinNumber)
                {
                    m_iMinNumber = value;
                    UpdateLabel();
                }
            }
        }

        /// <summary>
        /// Gets line number.
        /// </summary>
        public int LineNumber
        {
            get
            {
                double result;
                bool bResult = double.TryParse(txtNumber.Text, NumberStyles.Integer, null, out result);

                if (bResult)
                {
                    return (int)result;
                }

                return MinLine;
            }
        }

        /// <summary>
        /// Gets a value indicating whether line number is valid.
        /// </summary>
        public bool IsValidNumber
        {
            get
            {
                double result;
                bool valid = double.TryParse(txtNumber.Text, NumberStyles.Integer, null, out result);

                if (valid)
                    valid = (result >= MinLine) && (result <= MaxLine);

                return valid;
            }
        }
        #endregion

        #region Form Controls
        private System.Windows.Forms.Label lblNumber;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TextBox txtNumber;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion

        #region Initialize/Finalize Methods
        /// <summary>
        /// Initializes a new instance of the FrmGoDialog class.
        /// </summary>
        public FrmGoDialog()
        {
            InitializeComponent();

            EnableDoubleBuffering();
        }

        /// <summary>
        /// Initializes a new instance of the FrmGoDialog class.
        /// </summary>
        /// <param name="owner">The form</param>
        public FrmGoDialog(Form owner)
            : this()
        {
            if (owner == null) throw new ArgumentNullException("owner");

            this.Owner = owner;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmGoDialog));
            this.lblNumber = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblNumber
            // 
            this.lblNumber.AccessibleDescription = resources.GetString("lblNumber.AccessibleDescription");
            this.lblNumber.AccessibleName = resources.GetString("lblNumber.AccessibleName");
            this.lblNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblNumber.Anchor")));
            this.lblNumber.AutoSize = ((bool)(resources.GetObject("lblNumber.AutoSize")));
            this.lblNumber.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblNumber.Dock")));
            this.lblNumber.Enabled = ((bool)(resources.GetObject("lblNumber.Enabled")));
            this.lblNumber.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblNumber.Font = ((System.Drawing.Font)(resources.GetObject("lblNumber.Font")));
            this.lblNumber.Image = ((System.Drawing.Image)(resources.GetObject("lblNumber.Image")));
            this.lblNumber.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblNumber.ImageAlign")));
            this.lblNumber.ImageIndex = ((int)(resources.GetObject("lblNumber.ImageIndex")));
            this.lblNumber.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblNumber.ImeMode")));
            this.lblNumber.Location = ((System.Drawing.Point)(resources.GetObject("lblNumber.Location")));
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblNumber.RightToLeft")));
            this.lblNumber.Size = ((System.Drawing.Size)(resources.GetObject("lblNumber.Size")));
            this.lblNumber.TabIndex = ((int)(resources.GetObject("lblNumber.TabIndex")));
            this.lblNumber.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGoNumber) == null) ? resources.GetString("lblNumber.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGoNumber);
            this.lblNumber.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblNumber.TextAlign")));
            this.lblNumber.Visible = ((bool)(resources.GetObject("lblNumber.Visible")));
            // 
            // txtNumber
            // 
            this.txtNumber.AccessibleDescription = resources.GetString("txtNumber.AccessibleDescription");
            this.txtNumber.AccessibleName = resources.GetString("txtNumber.AccessibleName");
            this.txtNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtNumber.Anchor")));
            this.txtNumber.AutoSize = ((bool)(resources.GetObject("txtNumber.AutoSize")));
            this.txtNumber.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtNumber.BackgroundImage")));
            this.txtNumber.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtNumber.Dock")));
            this.txtNumber.Enabled = ((bool)(resources.GetObject("txtNumber.Enabled")));
            this.txtNumber.Font = ((System.Drawing.Font)(resources.GetObject("txtNumber.Font")));
            this.txtNumber.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtNumber.ImeMode")));
            this.txtNumber.Location = ((System.Drawing.Point)(resources.GetObject("txtNumber.Location")));
            this.txtNumber.MaxLength = ((int)(resources.GetObject("txtNumber.MaxLength")));
            this.txtNumber.Multiline = ((bool)(resources.GetObject("txtNumber.Multiline")));
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.PasswordChar = ((char)(resources.GetObject("txtNumber.PasswordChar")));
            this.txtNumber.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtNumber.RightToLeft")));
            this.txtNumber.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtNumber.ScrollBars")));
            this.txtNumber.Size = ((System.Drawing.Size)(resources.GetObject("txtNumber.Size")));
            this.txtNumber.TabIndex = ((int)(resources.GetObject("txtNumber.TabIndex")));
            this.txtNumber.Text = resources.GetString("txtNumber.Text");
            this.txtNumber.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtNumber.TextAlign")));
            this.txtNumber.Visible = ((bool)(resources.GetObject("txtNumber.Visible")));
            this.txtNumber.WordWrap = ((bool)(resources.GetObject("txtNumber.WordWrap")));
            this.txtNumber.TextChanged += new System.EventHandler(this.TxtNumber_TextChanged);
            this.txtNumber.Enter += new System.EventHandler(this.TxtNumber_Enter);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleDescription = resources.GetString("btnCancel.AccessibleDescription");
            this.btnCancel.AccessibleName = resources.GetString("btnCancel.AccessibleName");
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnCancel.Anchor")));
            this.btnCancel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCancel.BackgroundImage")));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnCancel.Dock")));
            this.btnCancel.Enabled = ((bool)(resources.GetObject("btnCancel.Enabled")));
            this.btnCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnCancel.FlatStyle")));
            this.btnCancel.Font = ((System.Drawing.Font)(resources.GetObject("btnCancel.Font")));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.ImageAlign")));
            this.btnCancel.ImageIndex = ((int)(resources.GetObject("btnCancel.ImageIndex")));
            this.btnCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnCancel.ImeMode")));
            this.btnCancel.Location = ((System.Drawing.Point)(resources.GetObject("btnCancel.Location")));
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnCancel.RightToLeft")));
            this.btnCancel.Size = ((System.Drawing.Size)(resources.GetObject("btnCancel.Size")));
            this.btnCancel.TabIndex = ((int)(resources.GetObject("btnCancel.TabIndex")));
            this.btnCancel.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnCancel) == null) ? resources.GetString("btnCancel.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnCancel);
            this.btnCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.TextAlign")));
            this.btnCancel.Visible = ((bool)(resources.GetObject("btnCancel.Visible")));
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = resources.GetString("btnOK.AccessibleDescription");
            this.btnOK.AccessibleName = resources.GetString("btnOK.AccessibleName");
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnOK.Anchor")));
            this.btnOK.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOK.BackgroundImage")));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnOK.Dock")));
            this.btnOK.Enabled = ((bool)(resources.GetObject("btnOK.Enabled")));
            this.btnOK.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnOK.FlatStyle")));
            this.btnOK.Font = ((System.Drawing.Font)(resources.GetObject("btnOK.Font")));
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnOK.ImageAlign")));
            this.btnOK.ImageIndex = ((int)(resources.GetObject("btnOK.ImageIndex")));
            this.btnOK.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnOK.ImeMode")));
            this.btnOK.Location = ((System.Drawing.Point)(resources.GetObject("btnOK.Location")));
            this.btnOK.Name = "btnOK";
            this.btnOK.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnOK.RightToLeft")));
            this.btnOK.Size = ((System.Drawing.Size)(resources.GetObject("btnOK.Size")));
            this.btnOK.TabIndex = ((int)(resources.GetObject("btnOK.TabIndex")));
            this.btnOK.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnOK) == null) ? resources.GetString("btnOK.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnOK);
            this.btnOK.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnOK.TextAlign")));
            this.btnOK.Visible = ((bool)(resources.GetObject("btnOK.Visible")));
            // 
            // FrmGoDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.CancelButton = this.btnCancel;
            this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtNumber);
            this.Controls.Add(this.lblNumber);
            this.Controls.Add(this.btnOK);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.MaximizeBox = false;
            this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
            this.MinimizeBox = false;
            this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
            this.Name = "FrmGoDialog";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.RightToLeftLayout = true;
            this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
            this.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGoTitle) == null) ? resources.GetString("$this.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGoTitle);
            this.Enter += new System.EventHandler(this.TxtNumber_Enter);
            this.ResumeLayout(false);

        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Changes label text.
        /// </summary>
        private void UpdateLabel()
        {
            lblNumber.Text =  string.Format( (Localizer.GetString(Localizer.EditResourceIdentifiers.FGoNumber)), MinLine, MaxLine);
        }
        private void LocalizeGoDialog()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmGoDialog));
            this.lblNumber.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGoNumber) == null) ? resources.GetString("lblNumber.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGoNumber);
            this.btnCancel.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnCancel) == null) ? resources.GetString("btnCancel.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnCancel);
            this.btnOK.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnOK) == null) ? resources.GetString("btnOK.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGobtnOK);
            this.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FGoTitle) == null) ? resources.GetString("$this.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FGoTitle);
        }
        /// <summary>
        /// Loads dialog.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.MinimumSize = new Size(this.Width, this.Height);
            LocalizeGoDialog();
            UpdateLabel();
        }

        /// <summary>
        /// Enables double buffering.
        /// </summary>
        public void EnableDoubleBuffering()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            this.UpdateStyles();
        }
        #endregion

        #region Event Handlers

        /// <summary>
        /// Raises when text in text box is chagned.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void TxtNumber_TextChanged(object sender, System.EventArgs e)
        {
            btnOK.Enabled = this.IsValidNumber;
        }

        /// <summary>
        /// Selects number.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void TxtNumber_Enter(object sender, System.EventArgs e)
        {
            txtNumber.SelectAll();
        }
        #endregion
    }
}