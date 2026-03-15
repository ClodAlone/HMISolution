#region Copyright Syncfusion Inc. 2001 - 2014

////  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
////  prohibited. Any infringement will be prosecuted under applicable laws. 

#endregion

using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

using Syncfusion.Windows.Forms.Edit.Implementation.Config;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
    /// <summary>
    /// Form for creating language.
    /// </summary>
    public class FrmCreateLangDialog
        : System.Windows.Forms.Form
    {
        #region Constants

        /// <summary>
        /// XPath for selecting name of languages.
        /// </summary>
        internal const string DEF_XPATH_LANG_NAME = "/ArrayOfConfigLanguage/ConfigLanguage/@name";

        /// <summary>
        /// Selects all configuration languages.
        /// </summary>
        internal const string DEF_XPATH_LANG = "/ArrayOfConfigLanguage/ConfigLanguage";

        /// <summary>
        /// Error message when name duplicates.
        /// </summary>
        private const string DEF_ERROR = "Configuration with such name already exists in current collection";
        #endregion

        #region Controls
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnFilePath;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblConfigFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Label lblLanguageList;
        private System.Windows.Forms.OpenFileDialog openDlg;
        private System.Windows.Forms.ComboBox comboLanguages;
        private System.Windows.Forms.ErrorProvider errorName;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion

        #region Fields

        /// <summary>
        /// Path to source configuration file.
        /// </summary>
        private string m_filePath;

        /// <summary>
        /// Configurator instance.
        /// </summary>
        private Config m_config;

        /// <summary>
        /// Inheritance configuration language name.
        /// </summary>
        private string m_inheritName;

        /// <summary>
        /// Name of new configuration.
        /// </summary>
        private string m_confName;

        /// <summary>
        /// Regex for validating name.
        /// </summary>
        private Regex m_regName;
        #endregion

        #region Properties

        /// <summary>
        /// Gets path to source config file. if NULL - from resources.
        /// </summary>
        public string FilePath
        {
            get
            {
                return m_filePath;
            }
        }

        /// <summary>
        /// Gets name of new configuration.
        /// </summary>
        public string ConfigurationName
        {
            get
            {
                return m_confName;
            }
        }

        /// <summary>
        /// Gets inheritance configuration language name.
        /// </summary>
        public string InheritanceName
        {
            get
            {
                return m_inheritName;
            }
        }
        #endregion

        #region Initialize/Finalize Methods

        /// <summary>
        /// Initializes a new instance of the FrmCreateLangDialog class.
        /// </summary>
        public FrmCreateLangDialog()
        {            
            // Required for Windows Form Designer support
            InitializeComponent();

            EnableDoubleBuffering();

            m_inheritName = string.Empty;
            m_confName = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the FrmCreateLangDialog class.
        /// </summary>
        /// <param name="config">The Config.</param>
        public FrmCreateLangDialog(Config config)
            : this()
        {
            if (config == null) throw new ArgumentNullException("config");

            m_config = config;
            m_filePath = null;

            InfillLanguageList();

            GenerateRegex();
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmCreateLangDialog));
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.comboLanguages = new System.Windows.Forms.ComboBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblConfigFile = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnFilePath = new System.Windows.Forms.Button();
            this.lblLanguageList = new System.Windows.Forms.Label();
            this.openDlg = new System.Windows.Forms.OpenFileDialog();
            this.errorName = new System.Windows.Forms.ErrorProvider();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.AccessibleDescription = resources.GetString("txtName.AccessibleDescription");
            this.txtName.AccessibleName = resources.GetString("txtName.AccessibleName");
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtName.Anchor")));
            this.txtName.AutoSize = ((bool)(resources.GetObject("txtName.AutoSize")));
            this.txtName.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtName.BackgroundImage")));
            this.txtName.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtName.Dock")));
            this.txtName.Enabled = ((bool)(resources.GetObject("txtName.Enabled")));
            this.errorName.SetError(this.txtName, resources.GetString("txtName.Error"));
            this.txtName.Font = ((System.Drawing.Font)(resources.GetObject("txtName.Font")));
            this.errorName.SetIconAlignment(this.txtName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("txtName.IconAlignment"))));
            this.errorName.SetIconPadding(this.txtName, ((int)(resources.GetObject("txtName.IconPadding"))));
            this.txtName.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtName.ImeMode")));
            this.txtName.Location = ((System.Drawing.Point)(resources.GetObject("txtName.Location")));
            this.txtName.MaxLength = ((int)(resources.GetObject("txtName.MaxLength")));
            this.txtName.Multiline = ((bool)(resources.GetObject("txtName.Multiline")));
            this.txtName.Name = "txtName";
            this.txtName.PasswordChar = ((char)(resources.GetObject("txtName.PasswordChar")));
            this.txtName.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtName.RightToLeft")));
            this.txtName.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtName.ScrollBars")));
            this.txtName.Size = ((System.Drawing.Size)(resources.GetObject("txtName.Size")));
            this.txtName.TabIndex = ((int)(resources.GetObject("txtName.TabIndex")));
            this.txtName.Text = resources.GetString("txtName.Text");
            this.txtName.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtName.TextAlign")));
            this.txtName.Visible = ((bool)(resources.GetObject("txtName.Visible")));
            this.txtName.WordWrap = ((bool)(resources.GetObject("txtName.WordWrap")));
            this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.TxtName_Validating);
            this.txtName.TextChanged += new System.EventHandler(this.TxtName_TextChanged);
            // 
            // lblName
            // 
            this.lblName.AccessibleDescription = resources.GetString("lblName.AccessibleDescription");
            this.lblName.AccessibleName = resources.GetString("lblName.AccessibleName");
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblName.Anchor")));
            this.lblName.AutoSize = ((bool)(resources.GetObject("lblName.AutoSize")));
            this.lblName.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblName.Dock")));
            this.lblName.Enabled = ((bool)(resources.GetObject("lblName.Enabled")));
            this.errorName.SetError(this.lblName, resources.GetString("lblName.Error"));
            this.lblName.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblName.Font = ((System.Drawing.Font)(resources.GetObject("lblName.Font")));
            this.errorName.SetIconAlignment(this.lblName, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblName.IconAlignment"))));
            this.errorName.SetIconPadding(this.lblName, ((int)(resources.GetObject("lblName.IconPadding"))));
            this.lblName.Image = ((System.Drawing.Image)(resources.GetObject("lblName.Image")));
            this.lblName.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblName.ImageAlign")));
            this.lblName.ImageIndex = ((int)(resources.GetObject("lblName.ImageIndex")));
            this.lblName.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblName.ImeMode")));
            this.lblName.Location = ((System.Drawing.Point)(resources.GetObject("lblName.Location")));
            this.lblName.Name = "lblName";
            this.lblName.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblName.RightToLeft")));
            this.lblName.Size = ((System.Drawing.Size)(resources.GetObject("lblName.Size")));
            this.lblName.TabIndex = ((int)(resources.GetObject("lblName.TabIndex")));
            this.lblName.Text = resources.GetString("lblName.Text");
            this.lblName.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblName.TextAlign")));
            this.lblName.Visible = ((bool)(resources.GetObject("lblName.Visible")));
            // 
            // comboLanguages
            // 
            this.comboLanguages.AccessibleDescription = resources.GetString("comboLanguages.AccessibleDescription");
            this.comboLanguages.AccessibleName = resources.GetString("comboLanguages.AccessibleName");
            this.comboLanguages.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("comboLanguages.Anchor")));
            this.comboLanguages.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("comboLanguages.BackgroundImage")));
            this.comboLanguages.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("comboLanguages.Dock")));
            this.comboLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLanguages.Enabled = ((bool)(resources.GetObject("comboLanguages.Enabled")));
            this.errorName.SetError(this.comboLanguages, resources.GetString("comboLanguages.Error"));
            this.comboLanguages.Font = ((System.Drawing.Font)(resources.GetObject("comboLanguages.Font")));
            this.errorName.SetIconAlignment(this.comboLanguages, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("comboLanguages.IconAlignment"))));
            this.errorName.SetIconPadding(this.comboLanguages, ((int)(resources.GetObject("comboLanguages.IconPadding"))));
            this.comboLanguages.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("comboLanguages.ImeMode")));
            this.comboLanguages.IntegralHeight = ((bool)(resources.GetObject("comboLanguages.IntegralHeight")));
            this.comboLanguages.ItemHeight = ((int)(resources.GetObject("comboLanguages.ItemHeight")));
            this.comboLanguages.Location = ((System.Drawing.Point)(resources.GetObject("comboLanguages.Location")));
            this.comboLanguages.MaxDropDownItems = ((int)(resources.GetObject("comboLanguages.MaxDropDownItems")));
            this.comboLanguages.MaxLength = ((int)(resources.GetObject("comboLanguages.MaxLength")));
            this.comboLanguages.Name = "comboLanguages";
            this.comboLanguages.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("comboLanguages.RightToLeft")));
            this.comboLanguages.Size = ((System.Drawing.Size)(resources.GetObject("comboLanguages.Size")));
            this.comboLanguages.TabIndex = ((int)(resources.GetObject("comboLanguages.TabIndex")));
            this.comboLanguages.Text = resources.GetString("comboLanguages.Text");
            this.comboLanguages.Visible = ((bool)(resources.GetObject("comboLanguages.Visible")));
            this.comboLanguages.SelectedIndexChanged += new System.EventHandler(this.ComboLanguages_SelectedIndexChanged);
            // 
            // btnOk
            // 
            this.btnOk.AccessibleDescription = resources.GetString("btnOk.AccessibleDescription");
            this.btnOk.AccessibleName = resources.GetString("btnOk.AccessibleName");
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnOk.Anchor")));
            this.btnOk.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnOk.BackgroundImage")));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnOk.Dock")));
            this.btnOk.Enabled = ((bool)(resources.GetObject("btnOk.Enabled")));
            this.errorName.SetError(this.btnOk, resources.GetString("btnOk.Error"));
            this.btnOk.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnOk.FlatStyle")));
            this.btnOk.Font = ((System.Drawing.Font)(resources.GetObject("btnOk.Font")));
            this.errorName.SetIconAlignment(this.btnOk, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnOk.IconAlignment"))));
            this.errorName.SetIconPadding(this.btnOk, ((int)(resources.GetObject("btnOk.IconPadding"))));
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnOk.ImageAlign")));
            this.btnOk.ImageIndex = ((int)(resources.GetObject("btnOk.ImageIndex")));
            this.btnOk.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnOk.ImeMode")));
            this.btnOk.Location = ((System.Drawing.Point)(resources.GetObject("btnOk.Location")));
            this.btnOk.Name = "btnOk";
            this.btnOk.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnOk.RightToLeft")));
            this.btnOk.Size = ((System.Drawing.Size)(resources.GetObject("btnOk.Size")));
            this.btnOk.TabIndex = ((int)(resources.GetObject("btnOk.TabIndex")));
            this.btnOk.Text = resources.GetString("btnOk.Text");
            this.btnOk.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnOk.TextAlign")));
            this.btnOk.Visible = ((bool)(resources.GetObject("btnOk.Visible")));
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
            this.errorName.SetError(this.btnCancel, resources.GetString("btnCancel.Error"));
            this.btnCancel.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnCancel.FlatStyle")));
            this.btnCancel.Font = ((System.Drawing.Font)(resources.GetObject("btnCancel.Font")));
            this.errorName.SetIconAlignment(this.btnCancel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnCancel.IconAlignment"))));
            this.errorName.SetIconPadding(this.btnCancel, ((int)(resources.GetObject("btnCancel.IconPadding"))));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.ImageAlign")));
            this.btnCancel.ImageIndex = ((int)(resources.GetObject("btnCancel.ImageIndex")));
            this.btnCancel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnCancel.ImeMode")));
            this.btnCancel.Location = ((System.Drawing.Point)(resources.GetObject("btnCancel.Location")));
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnCancel.RightToLeft")));
            this.btnCancel.Size = ((System.Drawing.Size)(resources.GetObject("btnCancel.Size")));
            this.btnCancel.TabIndex = ((int)(resources.GetObject("btnCancel.TabIndex")));
            this.btnCancel.Text = resources.GetString("btnCancel.Text");
            this.btnCancel.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnCancel.TextAlign")));
            this.btnCancel.Visible = ((bool)(resources.GetObject("btnCancel.Visible")));
            // 
            // lblConfigFile
            // 
            this.lblConfigFile.AccessibleDescription = resources.GetString("lblConfigFile.AccessibleDescription");
            this.lblConfigFile.AccessibleName = resources.GetString("lblConfigFile.AccessibleName");
            this.lblConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblConfigFile.Anchor")));
            this.lblConfigFile.AutoSize = ((bool)(resources.GetObject("lblConfigFile.AutoSize")));
            this.lblConfigFile.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblConfigFile.Dock")));
            this.lblConfigFile.Enabled = ((bool)(resources.GetObject("lblConfigFile.Enabled")));
            this.errorName.SetError(this.lblConfigFile, resources.GetString("lblConfigFile.Error"));
            this.lblConfigFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblConfigFile.Font = ((System.Drawing.Font)(resources.GetObject("lblConfigFile.Font")));
            this.errorName.SetIconAlignment(this.lblConfigFile, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblConfigFile.IconAlignment"))));
            this.errorName.SetIconPadding(this.lblConfigFile, ((int)(resources.GetObject("lblConfigFile.IconPadding"))));
            this.lblConfigFile.Image = ((System.Drawing.Image)(resources.GetObject("lblConfigFile.Image")));
            this.lblConfigFile.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblConfigFile.ImageAlign")));
            this.lblConfigFile.ImageIndex = ((int)(resources.GetObject("lblConfigFile.ImageIndex")));
            this.lblConfigFile.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblConfigFile.ImeMode")));
            this.lblConfigFile.Location = ((System.Drawing.Point)(resources.GetObject("lblConfigFile.Location")));
            this.lblConfigFile.Name = "lblConfigFile";
            this.lblConfigFile.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblConfigFile.RightToLeft")));
            this.lblConfigFile.Size = ((System.Drawing.Size)(resources.GetObject("lblConfigFile.Size")));
            this.lblConfigFile.TabIndex = ((int)(resources.GetObject("lblConfigFile.TabIndex")));
            this.lblConfigFile.Text = resources.GetString("lblConfigFile.Text");
            this.lblConfigFile.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblConfigFile.TextAlign")));
            this.lblConfigFile.Visible = ((bool)(resources.GetObject("lblConfigFile.Visible")));
            // 
            // txtFilePath
            // 
            this.txtFilePath.AccessibleDescription = resources.GetString("txtFilePath.AccessibleDescription");
            this.txtFilePath.AccessibleName = resources.GetString("txtFilePath.AccessibleName");
            this.txtFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("txtFilePath.Anchor")));
            this.txtFilePath.AutoSize = ((bool)(resources.GetObject("txtFilePath.AutoSize")));
            this.txtFilePath.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("txtFilePath.BackgroundImage")));
            this.txtFilePath.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("txtFilePath.Dock")));
            this.txtFilePath.Enabled = ((bool)(resources.GetObject("txtFilePath.Enabled")));
            this.errorName.SetError(this.txtFilePath, resources.GetString("txtFilePath.Error"));
            this.txtFilePath.Font = ((System.Drawing.Font)(resources.GetObject("txtFilePath.Font")));
            this.errorName.SetIconAlignment(this.txtFilePath, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("txtFilePath.IconAlignment"))));
            this.errorName.SetIconPadding(this.txtFilePath, ((int)(resources.GetObject("txtFilePath.IconPadding"))));
            this.txtFilePath.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("txtFilePath.ImeMode")));
            this.txtFilePath.Location = ((System.Drawing.Point)(resources.GetObject("txtFilePath.Location")));
            this.txtFilePath.MaxLength = ((int)(resources.GetObject("txtFilePath.MaxLength")));
            this.txtFilePath.Multiline = ((bool)(resources.GetObject("txtFilePath.Multiline")));
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.PasswordChar = ((char)(resources.GetObject("txtFilePath.PasswordChar")));
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("txtFilePath.RightToLeft")));
            this.txtFilePath.ScrollBars = ((System.Windows.Forms.ScrollBars)(resources.GetObject("txtFilePath.ScrollBars")));
            this.txtFilePath.Size = ((System.Drawing.Size)(resources.GetObject("txtFilePath.Size")));
            this.txtFilePath.TabIndex = ((int)(resources.GetObject("txtFilePath.TabIndex")));
            this.txtFilePath.Text = resources.GetString("txtFilePath.Text");
            this.txtFilePath.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("txtFilePath.TextAlign")));
            this.txtFilePath.Visible = ((bool)(resources.GetObject("txtFilePath.Visible")));
            this.txtFilePath.WordWrap = ((bool)(resources.GetObject("txtFilePath.WordWrap")));
            // 
            // btnFilePath
            // 
            this.btnFilePath.AccessibleDescription = resources.GetString("btnFilePath.AccessibleDescription");
            this.btnFilePath.AccessibleName = resources.GetString("btnFilePath.AccessibleName");
            this.btnFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("btnFilePath.Anchor")));
            this.btnFilePath.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFilePath.BackgroundImage")));
            this.btnFilePath.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnFilePath.Dock")));
            this.btnFilePath.Enabled = ((bool)(resources.GetObject("btnFilePath.Enabled")));
            this.errorName.SetError(this.btnFilePath, resources.GetString("btnFilePath.Error"));
            this.btnFilePath.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnFilePath.FlatStyle")));
            this.btnFilePath.Font = ((System.Drawing.Font)(resources.GetObject("btnFilePath.Font")));
            this.errorName.SetIconAlignment(this.btnFilePath, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("btnFilePath.IconAlignment"))));
            this.errorName.SetIconPadding(this.btnFilePath, ((int)(resources.GetObject("btnFilePath.IconPadding"))));
            this.btnFilePath.Image = ((System.Drawing.Image)(resources.GetObject("btnFilePath.Image")));
            this.btnFilePath.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnFilePath.ImageAlign")));
            this.btnFilePath.ImageIndex = ((int)(resources.GetObject("btnFilePath.ImageIndex")));
            this.btnFilePath.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnFilePath.ImeMode")));
            this.btnFilePath.Location = ((System.Drawing.Point)(resources.GetObject("btnFilePath.Location")));
            this.btnFilePath.Name = "btnFilePath";
            this.btnFilePath.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnFilePath.RightToLeft")));
            this.btnFilePath.Size = ((System.Drawing.Size)(resources.GetObject("btnFilePath.Size")));
            this.btnFilePath.TabIndex = ((int)(resources.GetObject("btnFilePath.TabIndex")));
            this.btnFilePath.Text = resources.GetString("btnFilePath.Text");
            this.btnFilePath.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnFilePath.TextAlign")));
            this.btnFilePath.Visible = ((bool)(resources.GetObject("btnFilePath.Visible")));
            this.btnFilePath.Click += new System.EventHandler(this.BtnFilePath_Click);
            // 
            // lblLanguageList
            // 
            this.lblLanguageList.AccessibleDescription = resources.GetString("lblLanguageList.AccessibleDescription");
            this.lblLanguageList.AccessibleName = resources.GetString("lblLanguageList.AccessibleName");
            this.lblLanguageList.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblLanguageList.Anchor")));
            this.lblLanguageList.AutoSize = ((bool)(resources.GetObject("lblLanguageList.AutoSize")));
            this.lblLanguageList.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblLanguageList.Dock")));
            this.lblLanguageList.Enabled = ((bool)(resources.GetObject("lblLanguageList.Enabled")));
            this.errorName.SetError(this.lblLanguageList, resources.GetString("lblLanguageList.Error"));
            this.lblLanguageList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblLanguageList.Font = ((System.Drawing.Font)(resources.GetObject("lblLanguageList.Font")));
            this.errorName.SetIconAlignment(this.lblLanguageList, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("lblLanguageList.IconAlignment"))));
            this.errorName.SetIconPadding(this.lblLanguageList, ((int)(resources.GetObject("lblLanguageList.IconPadding"))));
            this.lblLanguageList.Image = ((System.Drawing.Image)(resources.GetObject("lblLanguageList.Image")));
            this.lblLanguageList.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblLanguageList.ImageAlign")));
            this.lblLanguageList.ImageIndex = ((int)(resources.GetObject("lblLanguageList.ImageIndex")));
            this.lblLanguageList.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblLanguageList.ImeMode")));
            this.lblLanguageList.Location = ((System.Drawing.Point)(resources.GetObject("lblLanguageList.Location")));
            this.lblLanguageList.Name = "lblLanguageList";
            this.lblLanguageList.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblLanguageList.RightToLeft")));
            this.lblLanguageList.Size = ((System.Drawing.Size)(resources.GetObject("lblLanguageList.Size")));
            this.lblLanguageList.TabIndex = ((int)(resources.GetObject("lblLanguageList.TabIndex")));
            this.lblLanguageList.Text = resources.GetString("lblLanguageList.Text");
            this.lblLanguageList.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblLanguageList.TextAlign")));
            this.lblLanguageList.Visible = ((bool)(resources.GetObject("lblLanguageList.Visible")));
            // 
            // openDlg
            // 
            this.openDlg.Filter = resources.GetString("openDlg.Filter");
            this.openDlg.Title = resources.GetString("openDlg.Title");
            // 
            // errorName
            // 
            this.errorName.ContainerControl = this;
            this.errorName.Icon = ((System.Drawing.Icon)(resources.GetObject("errorName.Icon")));
            // 
            // FrmCreateLangDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.CancelButton = this.btnCancel;
            this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.btnFilePath);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.comboLanguages);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.lblConfigFile);
            this.Controls.Add(this.lblLanguageList);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.MaximizeBox = false;
            this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
            this.MinimizeBox = false;
            this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
            this.Name = "FrmCreateLangDialog";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.RightToLeftLayout = true;
            this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
            this.Text = resources.GetString("$this.Text");
            this.ResumeLayout(false);

        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Choose source configuration file.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void BtnFilePath_Click(object sender, System.EventArgs e)
        {
            if (openDlg.ShowDialog(this) == DialogResult.OK)
            {
                m_filePath = openDlg.FileName;
                txtFilePath.Text = m_filePath;

                InfillLanguageList();
            }
        }

        /// <summary>
        /// Inheritance name of language.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void ComboLanguages_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            m_inheritName = comboLanguages.Text;
        }

        /// <summary>
        /// Name of configuration changed.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void TxtName_TextChanged(object sender, System.EventArgs e)
        {
            m_confName = txtName.Text;

            btnOk.Enabled = txtName.Text.Length > 0;
        }

        /// <summary>
        /// Validating Name of language.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void TxtName_Validating(object sender, CancelEventArgs e)
        {
            // Configuration with such name already exists.
            if (m_regName.Match(txtName.Text).Success)
            {
                e.Cancel = true;
                errorName.SetIconAlignment(txtName, ErrorIconAlignment.MiddleLeft);
                errorName.SetError(txtName, DEF_ERROR);
            }
            else
            {
                errorName.SetError(txtName, string.Empty);
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Infills list of defined configurations.
        /// </summary>
        private void InfillLanguageList()
        {
            comboLanguages.BeginUpdate();

            if (m_filePath == null)
            {
                InfillDefaultLanguages();
            }
            else
            {
                InfillCustomLanguages();
            }

            comboLanguages.EndUpdate();
        }

        /// <summary>
        /// Infills default languages list.
        /// </summary>
        private void InfillDefaultLanguages()
        {
            comboLanguages.Items.Clear();

            IConfigLanguage lang = null;

            for (int i = 0, len = m_config.KnownLanguages.Count; i < len; i++)
            {
                lang = m_config[i];
                comboLanguages.Items.Add(lang.Language);
            }

            if (comboLanguages.Items.Count > 0)
            {
                comboLanguages.SelectedIndex = 0;
                m_inheritName = (string)comboLanguages.Items[0];
            }
        }

        /// <summary>
        /// Infills list of languages from custom source file.
        /// </summary>
        private void InfillCustomLanguages()
        {
            if (m_filePath == null || m_filePath.Length == 0) return;

            if (!File.Exists(m_filePath)) return;

            XmlDocument document = null;
            try
            {
                document = new XmlDocument();
                document.Load(m_filePath);

                comboLanguages.Items.Clear();
                XmlNodeList nodeList = document.SelectNodes(DEF_XPATH_LANG_NAME);

                for (int i = 0, len = nodeList.Count; i < len; i++)
                {
                    comboLanguages.Items.Add(nodeList[i].Value);
                }

                if (comboLanguages.Items.Count > 0)
                {
                    comboLanguages.SelectedIndex = 0;
                    m_inheritName = (string)comboLanguages.Items[0];
                }
            }
            catch (XmlException)
            {
                MessageBox.Show(Localizer.DEF_MSG_XML_CONFIG_LOAD_ERROR);
            }
        }

        /// <summary>
        /// Creates regex instance.
        /// </summary>
        private void GenerateRegex()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0, len = m_config.KnownLanguages.Count; i < len; i++)
            {
                builder.Append("^" + m_config[i].Language + "$|");
            }

            builder.Remove(builder.Length - 1, 1);

            m_regName = new Regex(builder.ToString(), Syncfusion.Windows.Forms.Edit.Implementation.Config.Config.DEF_COMPILED_REGEX);
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
    }
}