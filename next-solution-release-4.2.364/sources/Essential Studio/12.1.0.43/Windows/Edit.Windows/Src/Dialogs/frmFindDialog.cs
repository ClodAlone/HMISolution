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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Edit.Enums;
using Syncfusion.Windows.Forms.Edit.Implementation.IO;
using Syncfusion.Windows.Forms.Edit.Interfaces;
using Syncfusion.Windows.Forms.Localization;

namespace Syncfusion.Windows.Forms.Edit.Dialogs
{
    /// <summary>
    /// Find text tool window.
    /// </summary>
    public class FrmFindDialog
        : System.Windows.Forms.Form, IFindDialogForm
    {
        #region Constants
        /// <summary>
        /// Regexp symbols for inserting in text searching field.
        /// </summary>
        protected readonly string[] DEF_REGEXP =
    {
      ".",   " *",   " +",   " ",  "^",   "$",
      "\\b",   "\\n", " ",  "[ ]",  "[^ ]",
      "|",   "\\",  "{}",  " ",  ":i",  ":q",
      ":b",  ":z"
    };

        /// <summary>
        /// Template for regex to match whole word.
        /// </summary>
        internal const string DEF_STR_WHOLEWORD_REGEX_TEMPLATE = @"(?<=(^|\W))(?<{0}>{1})(?=(\W|$))";
        #endregion

        #region Control Members
        /// <summary>
        /// Defines a new Combobox
        /// </summary>
        protected System.Windows.Forms.ComboBox cmbFind;
        /// <summary>
        /// Defines a new Checkbox
        /// </summary>
        protected System.Windows.Forms.CheckBox chkCase;
        /// <summary>
        /// Defines a new Button
        /// </summary>
        protected System.Windows.Forms.Button btnFind;
        /// <summary>
        /// Defines a new Button
        /// </summary>
        protected System.Windows.Forms.Button btnClose;
        /// <summary>
        /// Defines a new Label
        /// </summary>
        protected System.Windows.Forms.Label lblFind;
        /// <summary>
        /// Defines a new Button
        /// </summary>
        protected System.Windows.Forms.Button btnTempaltes;
        /// <summary>
        /// Defines a new Button
        /// </summary>
        protected System.Windows.Forms.Button btnMarkAll;
        /// <summary>
        /// Defines a new CheckBox
        /// </summary>
        protected System.Windows.Forms.CheckBox chkWholeWord;
        /// <summary>
        /// Defines a new CheckBox
        /// </summary>
        protected System.Windows.Forms.CheckBox chkHidden;
        /// <summary>
        /// Defines a new CheckBox
        /// </summary>
        protected System.Windows.Forms.CheckBox chkUp;
        /// <summary>
        /// Defines a new CheckBox
        /// </summary>
        protected System.Windows.Forms.CheckBox chkRegular;
        /// <summary>
        /// Defines a new CheckBox
        /// </summary>
        protected System.Windows.Forms.CheckBox chkWrap;
        /// <summary>
        /// Defines a new Groupbox
        /// </summary>
        protected System.Windows.Forms.GroupBox groupBox1;
        /// <summary>
        /// Defines a new Radiobutton
        /// </summary>
        protected System.Windows.Forms.RadioButton rdbDocument;
        /// <summary>
        /// Defines a new Radiobutton
        /// </summary>
        protected System.Windows.Forms.RadioButton rdbSelection;
        /// <summary>
        /// Defines a new Contextmenu
        /// </summary>
        protected System.Windows.Forms.ContextMenu cmnTemplates;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem9;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem menuItem12;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem menuItem14;
        private System.Windows.Forms.MenuItem menuItem15;
        private System.Windows.Forms.MenuItem menuItem16;
        private System.Windows.Forms.MenuItem menuItem17;
        private System.Windows.Forms.MenuItem menuItem18;
        private System.Windows.Forms.MenuItem menuItem19;
        private System.Windows.Forms.MenuItem menuItem20;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        #endregion

        #region Fields

        /// <summary>
        /// Position of the Starting point of search
        /// </summary>
        private IParsePoint m_ptSearchStartPosition;

        /// <summary>
        /// Starting point string
        /// </summary>
        private String m_SearchStartString;

        /// <summary>
        /// Control instance.
        /// </summary>
        protected StreamEditControl m_control;

        /// <summary>
        /// Indicates if search in selected text.
        /// </summary>
        protected bool m_bInSelection;

        /// <summary>
        /// Indicates features for searching.
        /// </summary>
        protected SearchAttributes m_attributes;

        /// <summary>
        /// Type of searching.
        /// </summary>
        protected SearchType m_searchType;

        /// <summary>
        /// Text for searching.
        /// </summary>
        private string m_searchText;

        /// <summary>
        /// History of searched text.
        /// </summary>
        private ArrayList m_history;

        /// <summary>
        /// Indicates if we must change history because selected index in combobox has been changed.
        /// </summary>
        protected bool m_bIndexChaged;

        /// <summary>
        /// Instance on active comboBox.
        /// </summary>
        protected ComboBox m_activeCombo;

        /// <summary>
        /// start of the selection line 
        /// </summary>
        private int m_stLine;
        /// <summary>
        /// start of the selection column
        /// </summary>
        private int m_stColumn;
        /// <summary>
        /// End of the selection line 
        /// </summary>
        private int m_edLine;
        /// <summary>
        /// End of the selection column
        /// </summary>
        private int m_edColumn;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets searching text.
        /// </summary>
        public string SearchText
        {
            get
            {
                return m_searchText;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("SearchText");

                if (m_searchText != value)
                {
                    m_searchText = value;
                    cmbFind.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets searching history.
        /// </summary>
        public ArrayList History
        {
            get
            {
                return m_history;
            }
        }
        #endregion

        #region Initialize/Finalize Methods
        /// <summary>
        /// Initializes a new instance of the FrmFindDialog class.
        /// </summary>
        public FrmFindDialog()
        {
            m_attributes = SearchAttributes.Unknown;
            m_searchType = SearchType.Unknown;
            m_searchText = string.Empty;
            m_history = new ArrayList();

            m_activeCombo = cmbFind;

            // Required for Windows Form Designer support          
            InitializeComponent();
            EnableDoubleBuffering();
            this.FormClosing += new FormClosingEventHandler(FrmFindDialog_FormClosing);
            this.Deactivate += new EventHandler(FrmFindDialog_Deactivate);
            
        }

        /// <summary>
        /// Reset the Starting position and string value of find
        /// </summary>
        void ResetStartingPointofSearch()
        {
            this.m_SearchStartString = null;
            this.m_ptSearchStartPosition = null;
        }
        /// <summary>
        /// Returns the result of Find next result
        /// </summary>
        /// <param name="findResult"></param>
        /// <param name="findNextResult"></param>
        /// <returns></returns>
        protected virtual FindNextResult RaiseOnFound(FindResult findResult, FindNextResult findNextResult)
        {
            if (Found != null)
                Found(this, new FindNextEventArgs(findResult, findNextResult));
            if(m_control.FindDialogLocation != Point.Empty)
                this.Location = m_control.FindDialogLocation;
            return findNextResult;
        }

        void FrmFindDialog_Deactivate(object sender, EventArgs e)
        {
            if (rdbSelection.Checked == true)
            {
                m_bInSelection = false;
            }
            ResetStartingPointofSearch();
        }


        void FrmFindDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResetFlags();

        }
        /// <summary>
        /// Constructor for FrmFindDialog class
        /// </summary>
        /// <param name="edtControl"></param>
        public FrmFindDialog(EditControl edtControl)
            :this(edtControl.edtCode)
        {
        }
        /// <summary>
        /// Initializes a new instance of the FrmFindDialog class.
        /// </summary>
        /// <param name="parent">Instance of control class.</param>
        public FrmFindDialog(StreamEditControl parent)
            : this()
        {
            if (parent == null) throw new ArgumentNullException("parent");

            m_control = parent;
            chkWrap.Checked = m_control.WrapAroundSearch;
            this.Owner = parent.FindForm();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.FormClosing -= new FormClosingEventHandler(FrmFindDialog_FormClosing);
                this.Deactivate -= new EventHandler(FrmFindDialog_Deactivate);

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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmFindDialog));
            this.lblFind = new System.Windows.Forms.Label();
            this.cmbFind = new System.Windows.Forms.ComboBox();
            this.chkCase = new System.Windows.Forms.CheckBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnTempaltes = new System.Windows.Forms.Button();
            this.btnMarkAll = new System.Windows.Forms.Button();
            this.chkWholeWord = new System.Windows.Forms.CheckBox();
            this.chkHidden = new System.Windows.Forms.CheckBox();
            this.chkUp = new System.Windows.Forms.CheckBox();
            this.chkRegular = new System.Windows.Forms.CheckBox();
            this.chkWrap = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdbDocument = new System.Windows.Forms.RadioButton();
            this.rdbSelection = new System.Windows.Forms.RadioButton();
            this.cmnTemplates = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuItem14 = new System.Windows.Forms.MenuItem();
            this.menuItem15 = new System.Windows.Forms.MenuItem();
            this.menuItem16 = new System.Windows.Forms.MenuItem();
            this.menuItem17 = new System.Windows.Forms.MenuItem();
            this.menuItem18 = new System.Windows.Forms.MenuItem();
            this.menuItem19 = new System.Windows.Forms.MenuItem();
            this.menuItem20 = new System.Windows.Forms.MenuItem();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFind
            // 
            this.lblFind.AccessibleDescription = resources.GetString("lblFind.AccessibleDescription");
            this.lblFind.AccessibleName = resources.GetString("lblFind.AccessibleName");
            this.lblFind.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("lblFind.Anchor")));
            this.lblFind.AutoSize =true;
            this.lblFind.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("lblFind.Dock")));
            this.lblFind.Enabled = ((bool)(resources.GetObject("lblFind.Enabled")));
            this.lblFind.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblFind.Font = ((System.Drawing.Font)(resources.GetObject("lblFind.Font")));
            this.lblFind.Image = ((System.Drawing.Image)(resources.GetObject("lblFind.Image")));
            this.lblFind.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblFind.ImageAlign")));
            this.lblFind.ImageIndex = ((int)(resources.GetObject("lblFind.ImageIndex")));
            this.lblFind.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("lblFind.ImeMode")));
            this.lblFind.Location = ((System.Drawing.Point)(resources.GetObject("lblFind.Location")));
            this.lblFind.Name = "lblFind";
            this.lblFind.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("lblFind.RightToLeft")));
            this.lblFind.Size = ((System.Drawing.Size)(resources.GetObject("lblFind.Size")));
            this.lblFind.TabIndex = ((int)(resources.GetObject("lblFind.TabIndex")));
            this.lblFind.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDMain) == null) ? resources.GetString("lblFind.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDMain);
            this.lblFind.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("lblFind.TextAlign")));
            this.lblFind.Visible = ((bool)(resources.GetObject("lblFind.Visible")));
            // 
            // cmbFind
            // 
            this.cmbFind.AccessibleDescription = resources.GetString("cmbFind.AccessibleDescription");
            this.cmbFind.AccessibleName = resources.GetString("cmbFind.AccessibleName");
            this.cmbFind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cmbFind.BackgroundImage")));
            this.cmbFind.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("cmbFind.Dock")));
            this.cmbFind.Enabled = ((bool)(resources.GetObject("cmbFind.Enabled")));
            this.cmbFind.Font = ((System.Drawing.Font)(resources.GetObject("cmbFind.Font")));
            this.cmbFind.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("cmbFind.ImeMode")));
            this.cmbFind.IntegralHeight = ((bool)(resources.GetObject("cmbFind.IntegralHeight")));
            this.cmbFind.ItemHeight = ((int)(resources.GetObject("cmbFind.ItemHeight")));
            this.cmbFind.Location = ((System.Drawing.Point)(resources.GetObject("cmbFind.Location")));
            this.cmbFind.MaxDropDownItems = ((int)(resources.GetObject("cmbFind.MaxDropDownItems")));
            this.cmbFind.MaxLength = ((int)(resources.GetObject("cmbFind.MaxLength")));
            this.cmbFind.Name = "cmbFind";
            this.cmbFind.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("cmbFind.RightToLeft")));
            this.cmbFind.Size = ((System.Drawing.Size)(resources.GetObject("cmbFind.Size")));
            this.cmbFind.TabIndex = ((int)(resources.GetObject("cmbFind.TabIndex")));
            this.cmbFind.Text = resources.GetString("cmbFind.Text");
            this.cmbFind.Visible = ((bool)(resources.GetObject("cmbFind.Visible")));
            this.cmbFind.TextChanged += new System.EventHandler(this.CmbFind_TextChanged);
            this.cmbFind.SelectedIndexChanged += new System.EventHandler(this.CmbFind_SelectedIndexChanged);
            // 
            // chkCase
            // 
            this.chkCase.AutoSize = true;
            this.chkCase.AccessibleDescription = resources.GetString("chkCase.AccessibleDescription");
            this.chkCase.AccessibleName = resources.GetString("chkCase.AccessibleName");
            this.chkCase.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkCase.Anchor")));
            this.chkCase.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkCase.Appearance")));
            this.chkCase.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkCase.BackgroundImage")));
            this.chkCase.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkCase.CheckAlign")));
            this.chkCase.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkCase.Dock")));
            this.chkCase.Enabled = ((bool)(resources.GetObject("chkCase.Enabled")));
            this.chkCase.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkCase.FlatStyle")));
            this.chkCase.Font = ((System.Drawing.Font)(resources.GetObject("chkCase.Font")));
            this.chkCase.Image = ((System.Drawing.Image)(resources.GetObject("chkCase.Image")));
            this.chkCase.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkCase.ImageAlign")));
            this.chkCase.ImageIndex = ((int)(resources.GetObject("chkCase.ImageIndex")));
            this.chkCase.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkCase.ImeMode")));
            this.chkCase.Location = ((System.Drawing.Point)(resources.GetObject("chkCase.Location")));
            this.chkCase.Name = "chkCase";
            this.chkCase.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkCase.RightToLeft")));
            this.chkCase.Size = ((System.Drawing.Size)(resources.GetObject("chkCase.Size")));
            this.chkCase.TabIndex = ((int)(resources.GetObject("chkCase.TabIndex")));
            this.chkCase.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkCase) == null) ? resources.GetString("chkCase.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkCase);
            this.chkCase.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkCase.TextAlign")));
            this.chkCase.Visible = ((bool)(resources.GetObject("chkCase.Visible")));
            this.chkCase.CheckedChanged += new System.EventHandler(this.SearchAttr_Changed);
            // 
            // btnFind
            // 
            this.btnFind.AutoSize = true;
            this.btnFind.AccessibleDescription = resources.GetString("btnFind.AccessibleDescription");
            this.btnFind.AccessibleName = resources.GetString("btnFind.AccessibleName");
            this.btnFind.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnFind.BackgroundImage")));
            this.btnFind.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnFind.Dock")));
            this.btnFind.Enabled = ((bool)(resources.GetObject("btnFind.Enabled")));
            this.btnFind.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnFind.FlatStyle")));
            this.btnFind.Font = ((System.Drawing.Font)(resources.GetObject("btnFind.Font")));
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnFind.ImageAlign")));
            this.btnFind.ImageIndex = ((int)(resources.GetObject("btnFind.ImageIndex")));
            this.btnFind.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnFind.ImeMode")));
            this.btnFind.Location = ((System.Drawing.Point)(resources.GetObject("btnFind.Location")));
            this.btnFind.Name = "btnFind";
            this.btnFind.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnFind.RightToLeft")));
            this.btnFind.Size = ((System.Drawing.Size)(resources.GetObject("btnFind.Size")));
            this.btnFind.TabIndex = ((int)(resources.GetObject("btnFind.TabIndex")));
            this.btnFind.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnFind) == null) ? resources.GetString("btnFind.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnFind);
            this.btnFind.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnFind.TextAlign")));
            this.btnFind.Visible = ((bool)(resources.GetObject("btnFind.Visible")));
            this.btnFind.Click += new System.EventHandler(this.Find_Text);
            // 
            // btnClose
            // 
            this.btnClose.AutoSize = true;
            this.btnClose.AccessibleDescription = resources.GetString("btnClose.AccessibleDescription");
            this.btnClose.AccessibleName = resources.GetString("btnClose.AccessibleName");
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnClose.Dock")));
            this.btnClose.Enabled = ((bool)(resources.GetObject("btnClose.Enabled")));
            this.btnClose.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnClose.FlatStyle")));
            this.btnClose.Font = ((System.Drawing.Font)(resources.GetObject("btnClose.Font")));
            this.btnClose.Image = ((System.Drawing.Image)(resources.GetObject("btnClose.Image")));
            this.btnClose.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnClose.ImageAlign")));
            this.btnClose.ImageIndex = ((int)(resources.GetObject("btnClose.ImageIndex")));
            this.btnClose.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnClose.ImeMode")));
            this.btnClose.Location = ((System.Drawing.Point)(resources.GetObject("btnClose.Location")));
            this.btnClose.Name = "btnClose";
            this.btnClose.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnClose.RightToLeft")));
            this.btnClose.Size = ((System.Drawing.Size)(resources.GetObject("btnClose.Size")));
            this.btnClose.TabIndex = ((int)(resources.GetObject("btnClose.TabIndex")));
            this.btnClose.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnClose) == null) ? resources.GetString("btnClose.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnClose);
            this.btnClose.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnClose.TextAlign")));
            this.btnClose.Visible = ((bool)(resources.GetObject("btnClose.Visible")));
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
            // 
            // btnTempaltes
            // 
            this.btnTempaltes.AutoSize = true;
            this.btnTempaltes.AccessibleDescription = resources.GetString("btnTempaltes.AccessibleDescription");
            this.btnTempaltes.AccessibleName = resources.GetString("btnTempaltes.AccessibleName");
            this.btnTempaltes.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnTempaltes.BackgroundImage")));
            this.btnTempaltes.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnTempaltes.Dock")));
            this.btnTempaltes.Enabled = ((bool)(resources.GetObject("btnTempaltes.Enabled")));
            this.btnTempaltes.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnTempaltes.FlatStyle")));
            this.btnTempaltes.Font = ((System.Drawing.Font)(resources.GetObject("btnTempaltes.Font")));
            this.btnTempaltes.Image = ((System.Drawing.Image)(resources.GetObject("btnTempaltes.Image")));
            this.btnTempaltes.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnTempaltes.ImageAlign")));
            this.btnTempaltes.ImageIndex = ((int)(resources.GetObject("btnTempaltes.ImageIndex")));
            this.btnTempaltes.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnTempaltes.ImeMode")));
            this.btnTempaltes.Location = ((System.Drawing.Point)(resources.GetObject("btnTempaltes.Location")));
            this.btnTempaltes.Name = "btnTempaltes";
            this.btnTempaltes.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnTempaltes.RightToLeft")));
            this.btnTempaltes.Size = ((System.Drawing.Size)(resources.GetObject("btnTempaltes.Size")));
            this.btnTempaltes.TabIndex = ((int)(resources.GetObject("btnTempaltes.TabIndex")));
            this.btnTempaltes.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnTempaltes) == null) ? resources.GetString("btnTempaltes.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnTempaltes);
            this.btnTempaltes.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnTempaltes.TextAlign")));
            this.btnTempaltes.Visible = ((bool)(resources.GetObject("btnTempaltes.Visible")));
            this.btnTempaltes.Click += new System.EventHandler(this.BtnTempaltes_Click);
            // 
            // btnMarkAll
            // 
            this.btnMarkAll.AutoSize = true;
            this.btnMarkAll.AccessibleDescription = resources.GetString("btnMarkAll.AccessibleDescription");
            this.btnMarkAll.AccessibleName = resources.GetString("btnMarkAll.AccessibleName");
            this.btnMarkAll.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnMarkAll.BackgroundImage")));
            this.btnMarkAll.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("btnMarkAll.Dock")));
            this.btnMarkAll.Enabled = ((bool)(resources.GetObject("btnMarkAll.Enabled")));
            this.btnMarkAll.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("btnMarkAll.FlatStyle")));
            this.btnMarkAll.Font = ((System.Drawing.Font)(resources.GetObject("btnMarkAll.Font")));
            this.btnMarkAll.Image = ((System.Drawing.Image)(resources.GetObject("btnMarkAll.Image")));
            this.btnMarkAll.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMarkAll.ImageAlign")));
            this.btnMarkAll.ImageIndex = ((int)(resources.GetObject("btnMarkAll.ImageIndex")));
            this.btnMarkAll.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("btnMarkAll.ImeMode")));
            this.btnMarkAll.Location = ((System.Drawing.Point)(resources.GetObject("btnMarkAll.Location")));
            this.btnMarkAll.Name = "btnMarkAll";
            this.btnMarkAll.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("btnMarkAll.RightToLeft")));
            this.btnMarkAll.Size = ((System.Drawing.Size)(resources.GetObject("btnMarkAll.Size")));
            this.btnMarkAll.TabIndex = ((int)(resources.GetObject("btnMarkAll.TabIndex")));
            this.btnMarkAll.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnMarkAll) == null) ? resources.GetString("btnMarkAll.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnMarkAll);
            this.btnMarkAll.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("btnMarkAll.TextAlign")));
            this.btnMarkAll.Visible = ((bool)(resources.GetObject("btnMarkAll.Visible")));
            this.btnMarkAll.Click += new System.EventHandler(this.BtnMarkAll_Click);
            // 
            // chkWholeWord
            // 
            this.chkWholeWord.AutoSize = true;
            this.chkWholeWord.AccessibleDescription = resources.GetString("chkWholeWord.AccessibleDescription");
            this.chkWholeWord.AccessibleName = resources.GetString("chkWholeWord.AccessibleName");
            this.chkWholeWord.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkWholeWord.Anchor")));
            this.chkWholeWord.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkWholeWord.Appearance")));
            this.chkWholeWord.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkWholeWord.BackgroundImage")));
            this.chkWholeWord.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWholeWord.CheckAlign")));
            this.chkWholeWord.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkWholeWord.Dock")));
            this.chkWholeWord.Enabled = ((bool)(resources.GetObject("chkWholeWord.Enabled")));
            this.chkWholeWord.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkWholeWord.FlatStyle")));
            this.chkWholeWord.Font = ((System.Drawing.Font)(resources.GetObject("chkWholeWord.Font")));
            this.chkWholeWord.Image = ((System.Drawing.Image)(resources.GetObject("chkWholeWord.Image")));
            this.chkWholeWord.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWholeWord.ImageAlign")));
            this.chkWholeWord.ImageIndex = ((int)(resources.GetObject("chkWholeWord.ImageIndex")));
            this.chkWholeWord.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkWholeWord.ImeMode")));
            this.chkWholeWord.Location = ((System.Drawing.Point)(resources.GetObject("chkWholeWord.Location")));
            this.chkWholeWord.Name = "chkWholeWord";
            this.chkWholeWord.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkWholeWord.RightToLeft")));
            this.chkWholeWord.Size = ((System.Drawing.Size)(resources.GetObject("chkWholeWord.Size")));
            this.chkWholeWord.TabIndex = ((int)(resources.GetObject("chkWholeWord.TabIndex")));
            this.chkWholeWord.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWholeWord) == null) ? resources.GetString("chkWholeWord.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWholeWord);
            this.chkWholeWord.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWholeWord.TextAlign")));
            this.chkWholeWord.Visible = ((bool)(resources.GetObject("chkWholeWord.Visible")));
            this.chkWholeWord.CheckedChanged += new System.EventHandler(this.SearchAttr_Changed);
            // 
            // chkHidden
            // 
            this.chkHidden.AutoSize = true;
            this.chkHidden.AccessibleDescription = resources.GetString("chkHidden.AccessibleDescription");
            this.chkHidden.AccessibleName = resources.GetString("chkHidden.AccessibleName");
            this.chkHidden.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkHidden.Anchor")));
            this.chkHidden.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkHidden.Appearance")));
            this.chkHidden.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkHidden.BackgroundImage")));
            this.chkHidden.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkHidden.CheckAlign")));
            this.chkHidden.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkHidden.Dock")));
            this.chkHidden.Enabled = ((bool)(resources.GetObject("chkHidden.Enabled")));
            this.chkHidden.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkHidden.FlatStyle")));
            this.chkHidden.Font = ((System.Drawing.Font)(resources.GetObject("chkHidden.Font")));
            this.chkHidden.Image = ((System.Drawing.Image)(resources.GetObject("chkHidden.Image")));
            this.chkHidden.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkHidden.ImageAlign")));
            this.chkHidden.ImageIndex = ((int)(resources.GetObject("chkHidden.ImageIndex")));
            this.chkHidden.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkHidden.ImeMode")));
            this.chkHidden.Location = ((System.Drawing.Point)(resources.GetObject("chkHidden.Location")));
            this.chkHidden.Name = "chkHidden";
            this.chkHidden.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkHidden.RightToLeft")));
            this.chkHidden.Size = ((System.Drawing.Size)(resources.GetObject("chkHidden.Size")));
            this.chkHidden.TabIndex = ((int)(resources.GetObject("chkHidden.TabIndex")));
            this.chkHidden.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkHidden) == null) ? resources.GetString("chkHidden.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkHidden);
            this.chkHidden.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkHidden.TextAlign")));
            this.chkHidden.Visible = ((bool)(resources.GetObject("chkHidden.Visible")));
            this.chkHidden.CheckedChanged += new System.EventHandler(this.SearchAttr_Changed);
            // 
            // chkUp
            // 
            this.chkUp.AutoSize = true;
            this.chkUp.AccessibleDescription = resources.GetString("chkUp.AccessibleDescription");
            this.chkUp.AccessibleName = resources.GetString("chkUp.AccessibleName");
            this.chkUp.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkUp.Anchor")));
            this.chkUp.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkUp.Appearance")));
            this.chkUp.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkUp.BackgroundImage")));
            this.chkUp.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUp.CheckAlign")));
            this.chkUp.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkUp.Dock")));
            this.chkUp.Enabled = ((bool)(resources.GetObject("chkUp.Enabled")));
            this.chkUp.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkUp.FlatStyle")));
            this.chkUp.Font = ((System.Drawing.Font)(resources.GetObject("chkUp.Font")));
            this.chkUp.Image = ((System.Drawing.Image)(resources.GetObject("chkUp.Image")));
            this.chkUp.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUp.ImageAlign")));
            this.chkUp.ImageIndex = ((int)(resources.GetObject("chkUp.ImageIndex")));
            this.chkUp.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkUp.ImeMode")));
            this.chkUp.Location = ((System.Drawing.Point)(resources.GetObject("chkUp.Location")));
            this.chkUp.Name = "chkUp";
            this.chkUp.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkUp.RightToLeft")));
            this.chkUp.Size = ((System.Drawing.Size)(resources.GetObject("chkUp.Size")));
            this.chkUp.TabIndex = ((int)(resources.GetObject("chkUp.TabIndex")));
            this.chkUp.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkUp) == null) ? resources.GetString("chkUp.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkUp);
            this.chkUp.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkUp.TextAlign")));
            this.chkUp.Visible = ((bool)(resources.GetObject("chkUp.Visible")));
            this.chkUp.CheckedChanged += new System.EventHandler(this.SearchAttr_Changed);
            // 
            // chkRegular
            // 
            this.chkRegular.AutoSize = true;
            this.chkRegular.AccessibleDescription = resources.GetString("chkRegular.AccessibleDescription");
            this.chkRegular.AccessibleName = resources.GetString("chkRegular.AccessibleName");
            this.chkRegular.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkRegular.Anchor")));
            this.chkRegular.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkRegular.Appearance")));
            this.chkRegular.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkRegular.BackgroundImage")));
            this.chkRegular.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkRegular.CheckAlign")));
            this.chkRegular.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkRegular.Dock")));
            this.chkRegular.Enabled = ((bool)(resources.GetObject("chkRegular.Enabled")));
            this.chkRegular.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkRegular.FlatStyle")));
            this.chkRegular.Font = ((System.Drawing.Font)(resources.GetObject("chkRegular.Font")));
            this.chkRegular.Image = ((System.Drawing.Image)(resources.GetObject("chkRegular.Image")));
            this.chkRegular.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkRegular.ImageAlign")));
            this.chkRegular.ImageIndex = ((int)(resources.GetObject("chkRegular.ImageIndex")));
            this.chkRegular.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkRegular.ImeMode")));
            this.chkRegular.Location = ((System.Drawing.Point)(resources.GetObject("chkRegular.Location")));
            this.chkRegular.Name = "chkRegular";
            this.chkRegular.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkRegular.RightToLeft")));
            this.chkRegular.Size = ((System.Drawing.Size)(resources.GetObject("chkRegular.Size")));
            this.chkRegular.TabIndex = ((int)(resources.GetObject("chkRegular.TabIndex")));
            this.chkRegular.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkRegular) == null) ? resources.GetString("chkRegular.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkRegular);
            this.chkRegular.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkRegular.TextAlign")));
            this.chkRegular.Visible = ((bool)(resources.GetObject("chkRegular.Visible")));
            this.chkRegular.CheckedChanged += new System.EventHandler(this.SearchAttr_Changed);
            // 
            // chkWrap
            // 
            this.chkWrap.AutoSize = true;
            this.chkWrap.AccessibleDescription = resources.GetString("chkWrap.AccessibleDescription");
            this.chkWrap.AccessibleName = resources.GetString("chkWrap.AccessibleName");
            this.chkWrap.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("chkRegular.Anchor")));
            this.chkWrap.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("chkWrap.Appearance")));
            this.chkWrap.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chkWrap.BackgroundImage")));
            this.chkWrap.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWrap.CheckAlign")));
            this.chkWrap.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("chkWrap.Dock")));
            this.chkWrap.Enabled = ((bool)(resources.GetObject("chkWrap.Enabled")));
            this.chkWrap.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("chkWrap.FlatStyle")));
            this.chkWrap.Font = ((System.Drawing.Font)(resources.GetObject("chkWrap.Font")));
            this.chkWrap.Image = ((System.Drawing.Image)(resources.GetObject("chkWrap.Image")));
            this.chkWrap.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWrap.ImageAlign")));
            this.chkWrap.ImageIndex = ((int)(resources.GetObject("chkWrap.ImageIndex")));
            this.chkWrap.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("chkWrap.ImeMode")));
            this.chkWrap.Location = ((System.Drawing.Point)(resources.GetObject("chkWrap.Location")));
            this.chkWrap.Name = "chkWrap";
            this.chkWrap.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("chkWrap.RightToLeft")));
            this.chkWrap.Size = ((System.Drawing.Size)(resources.GetObject("chkWrap.Size")));
            this.chkWrap.TabIndex = ((int)(resources.GetObject("chkWrap.TabIndex")));
            this.chkWrap.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWrap) == null) ? resources.GetString("chkWrap.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWrap);
            this.chkWrap.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("chkWrap.TextAlign")));
            this.chkWrap.Visible = ((bool)(resources.GetObject("chkWrap.Visible")));
            this.chkWrap.CheckedChanged += new System.EventHandler(this.WrapAroundSearch_Changed);
            // 
            // groupBox1
            // 
            this.groupBox1.AccessibleDescription = resources.GetString("groupBox1.AccessibleDescription");
            this.groupBox1.AccessibleName = resources.GetString("groupBox1.AccessibleName");
            this.groupBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("groupBox1.BackgroundImage")));
            this.groupBox1.Controls.Add(this.rdbDocument);
            this.groupBox1.Controls.Add(this.rdbSelection);
            this.groupBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("groupBox1.Dock")));
            this.groupBox1.Enabled = ((bool)(resources.GetObject("groupBox1.Enabled")));
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Font = ((System.Drawing.Font)(resources.GetObject("groupBox1.Font")));
            this.groupBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("groupBox1.ImeMode")));
            this.groupBox1.Location = ((System.Drawing.Point)(resources.GetObject("groupBox1.Location")));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("groupBox1.RightToLeft")));
            this.groupBox1.Size = ((System.Drawing.Size)(resources.GetObject("groupBox1.Size")));
            this.groupBox1.TabIndex = ((int)(resources.GetObject("groupBox1.TabIndex")));
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDGroupTitle) == null) ? resources.GetString("groupBox1.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDGroupTitle);
            this.groupBox1.Visible = ((bool)(resources.GetObject("groupBox1.Visible")));
            // 
            // rdbDocument
            // 
            this.rdbDocument.AccessibleDescription = resources.GetString("rdbDocument.AccessibleDescription");
            this.rdbDocument.AccessibleName = resources.GetString("rdbDocument.AccessibleName");
            this.rdbDocument.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rdbDocument.Anchor")));
            this.rdbDocument.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rdbDocument.Appearance")));
            this.rdbDocument.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rdbDocument.BackgroundImage")));
            this.rdbDocument.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbDocument.CheckAlign")));
            this.rdbDocument.Checked = true;
            this.rdbDocument.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rdbDocument.Dock")));
            this.rdbDocument.Enabled = ((bool)(resources.GetObject("rdbDocument.Enabled")));
            this.rdbDocument.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rdbDocument.FlatStyle")));
            this.rdbDocument.Font = ((System.Drawing.Font)(resources.GetObject("rdbDocument.Font")));
            this.rdbDocument.Image = ((System.Drawing.Image)(resources.GetObject("rdbDocument.Image")));
            this.rdbDocument.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbDocument.ImageAlign")));
            this.rdbDocument.ImageIndex = ((int)(resources.GetObject("rdbDocument.ImageIndex")));
            this.rdbDocument.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rdbDocument.ImeMode")));
            this.rdbDocument.Location = ((System.Drawing.Point)(resources.GetObject("rdbDocument.Location")));
            this.rdbDocument.Name = "rdbDocument";
            this.rdbDocument.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rdbDocument.RightToLeft")));
            this.rdbDocument.Size = ((System.Drawing.Size)(resources.GetObject("rdbDocument.Size")));
            this.rdbDocument.TabIndex = ((int)(resources.GetObject("rdbDocument.TabIndex")));
            this.rdbDocument.TabStop = true;
            this.rdbDocument.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbDocument) == null) ? resources.GetString("rdbDocument.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbDocument);
            this.rdbDocument.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbDocument.TextAlign")));
            this.rdbDocument.Visible = ((bool)(resources.GetObject("rdbDocument.Visible")));
            this.rdbDocument.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // rdbSelection
            // 
            this.rdbSelection.AccessibleDescription = resources.GetString("rdbSelection.AccessibleDescription");
            this.rdbSelection.AccessibleName = resources.GetString("rdbSelection.AccessibleName");
            this.rdbSelection.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("rdbSelection.Anchor")));
            this.rdbSelection.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("rdbSelection.Appearance")));
            this.rdbSelection.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rdbSelection.BackgroundImage")));
            this.rdbSelection.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbSelection.CheckAlign")));
            this.rdbSelection.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("rdbSelection.Dock")));
            this.rdbSelection.Enabled = ((bool)(resources.GetObject("rdbSelection.Enabled")));
            this.rdbSelection.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("rdbSelection.FlatStyle")));
            this.rdbSelection.Font = ((System.Drawing.Font)(resources.GetObject("rdbSelection.Font")));
            this.rdbSelection.Image = ((System.Drawing.Image)(resources.GetObject("rdbSelection.Image")));
            this.rdbSelection.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbSelection.ImageAlign")));
            this.rdbSelection.ImageIndex = ((int)(resources.GetObject("rdbSelection.ImageIndex")));
            this.rdbSelection.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("rdbSelection.ImeMode")));
            this.rdbSelection.Location = ((System.Drawing.Point)(resources.GetObject("rdbSelection.Location")));
            this.rdbSelection.Name = "rdbSelection";
            this.rdbSelection.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("rdbSelection.RightToLeft")));
            this.rdbSelection.Size = ((System.Drawing.Size)(resources.GetObject("rdbSelection.Size")));
            this.rdbSelection.TabIndex = ((int)(resources.GetObject("rdbSelection.TabIndex")));
            this.rdbSelection.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbSelection) == null) ? resources.GetString("rdbSelection.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbSelection);
            this.rdbSelection.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("rdbSelection.TextAlign")));
            this.rdbSelection.Visible = ((bool)(resources.GetObject("rdbSelection.Visible")));
            this.rdbSelection.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // cmnTemplates
            // 
            this.cmnTemplates.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																																								 this.menuItem1,
																																								 this.menuItem2,
																																								 this.menuItem3,
																																								 this.menuItem5,
																																								 this.menuItem4,
																																								 this.menuItem6,
																																								 this.menuItem7,
																																								 this.menuItem9,
																																								 this.menuItem11,
																																								 this.menuItem10,
																																								 this.menuItem12,
																																								 this.menuItem13,
																																								 this.menuItem14,
																																								 this.menuItem15,
																																								 this.menuItem16,
																																								 this.menuItem17,
																																								 this.menuItem18,
																																								 this.menuItem19,
																																								 this.menuItem20});
            this.cmnTemplates.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("cmnTemplates.RightToLeft")));
            // 
            // menuItem1
            // 
            this.menuItem1.Enabled = ((bool)(resources.GetObject("menuItem1.Enabled")));
            this.menuItem1.Index = 0;
            this.menuItem1.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem1.Shortcut")));
            this.menuItem1.ShowShortcut = ((bool)(resources.GetObject("menuItem1.ShowShortcut")));
            this.menuItem1.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSingleChar) == null) ? resources.GetString("menuItem1.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSingleChar);
            this.menuItem1.Visible = ((bool)(resources.GetObject("menuItem1.Visible")));
            this.menuItem1.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem2
            // 
            this.menuItem2.Enabled = ((bool)(resources.GetObject("menuItem2.Enabled")));
            this.menuItem2.Index = 1;
            this.menuItem2.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem2.Shortcut")));
            this.menuItem2.ShowShortcut = ((bool)(resources.GetObject("menuItem2.ShowShortcut")));
            this.menuItem2.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexZeroOrMore) == null) ? resources.GetString("menuItem2.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexZeroOrMore);
            this.menuItem2.Visible = ((bool)(resources.GetObject("menuItem2.Visible")));
            this.menuItem2.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem3
            // 
            this.menuItem3.Enabled = ((bool)(resources.GetObject("menuItem3.Enabled")));
            this.menuItem3.Index = 2;
            this.menuItem3.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem3.Shortcut")));
            this.menuItem3.ShowShortcut = ((bool)(resources.GetObject("menuItem3.ShowShortcut")));
            this.menuItem3.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOneorMore) == null) ? resources.GetString("menuItem3.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOneorMore);
            this.menuItem3.Visible = ((bool)(resources.GetObject("menuItem3.Visible")));
            this.menuItem3.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem5
            // 
            this.menuItem5.Enabled = ((bool)(resources.GetObject("menuItem5.Enabled")));
            this.menuItem5.Index = 3;
            this.menuItem5.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem5.Shortcut")));
            this.menuItem5.ShowShortcut = ((bool)(resources.GetObject("menuItem5.ShowShortcut")));
            this.menuItem5.Text = resources.GetString("menuItem5.Text");
            this.menuItem5.Visible = ((bool)(resources.GetObject("menuItem5.Visible")));
            // 
            // menuItem4
            // 
            this.menuItem4.Enabled = ((bool)(resources.GetObject("menuItem4.Enabled")));
            this.menuItem4.Index = 4;
            this.menuItem4.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem4.Shortcut")));
            this.menuItem4.ShowShortcut = ((bool)(resources.GetObject("menuItem4.ShowShortcut")));
            this.menuItem4.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBegining) == null) ? resources.GetString("menuItem4.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBegining);
            this.menuItem4.Visible = ((bool)(resources.GetObject("menuItem4.Visible")));
            this.menuItem4.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem6
            // 
            this.menuItem6.Enabled = ((bool)(resources.GetObject("menuItem6.Enabled")));
            this.menuItem6.Index = 5;
            this.menuItem6.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem6.Shortcut")));
            this.menuItem6.ShowShortcut = ((bool)(resources.GetObject("menuItem6.ShowShortcut")));
            this.menuItem6.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineEnd) == null) ? resources.GetString("menuItem6.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineEnd);
            this.menuItem6.Visible = ((bool)(resources.GetObject("menuItem6.Visible")));
            this.menuItem6.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem7
            // 
            this.menuItem7.Enabled = ((bool)(resources.GetObject("menuItem7.Enabled")));
            this.menuItem7.Index = 6;
            this.menuItem7.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem7.Shortcut")));
            this.menuItem7.ShowShortcut = ((bool)(resources.GetObject("menuItem7.ShowShortcut")));
            this.menuItem7.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexBeginEndWord) == null) ? resources.GetString("menuItem7.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexBeginEndWord);
            this.menuItem7.Visible = ((bool)(resources.GetObject("menuItem7.Visible")));
            this.menuItem7.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem9
            // 
            this.menuItem9.Enabled = ((bool)(resources.GetObject("menuItem9.Enabled")));
            this.menuItem9.Index = 7;
            this.menuItem9.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem9.Shortcut")));
            this.menuItem9.ShowShortcut = ((bool)(resources.GetObject("menuItem9.ShowShortcut")));
            this.menuItem9.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBreak) == null) ? resources.GetString("menuItem9.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBreak);
            this.menuItem9.Visible = ((bool)(resources.GetObject("menuItem9.Visible")));
            this.menuItem9.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem11
            // 
            this.menuItem11.Enabled = ((bool)(resources.GetObject("menuItem11.Enabled")));
            this.menuItem11.Index = 8;
            this.menuItem11.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem11.Shortcut")));
            this.menuItem11.ShowShortcut = ((bool)(resources.GetObject("menuItem11.ShowShortcut")));
            this.menuItem11.Text = resources.GetString("menuItem11.Text");
            this.menuItem11.Visible = ((bool)(resources.GetObject("menuItem11.Visible")));
            // 
            // menuItem10
            // 
            this.menuItem10.Enabled = ((bool)(resources.GetObject("menuItem10.Enabled")));
            this.menuItem10.Index = 9;
            this.menuItem10.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem10.Shortcut")));
            this.menuItem10.ShowShortcut = ((bool)(resources.GetObject("menuItem10.ShowShortcut")));
            this.menuItem10.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyOneCharset) == null) ? resources.GetString("menuItem10.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyOneCharset);
            this.menuItem10.Visible = ((bool)(resources.GetObject("menuItem10.Visible")));
            this.menuItem10.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem12
            // 
            this.menuItem12.Enabled = ((bool)(resources.GetObject("menuItem12.Enabled")));
            this.menuItem12.Index = 10;
            this.menuItem12.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem12.Shortcut")));
            this.menuItem12.ShowShortcut = ((bool)(resources.GetObject("menuItem12.ShowShortcut")));
            this.menuItem12.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyCharset) == null) ? resources.GetString("menuItem12.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyCharset);
            this.menuItem12.Visible = ((bool)(resources.GetObject("menuItem12.Visible")));
            this.menuItem12.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem13
            // 
            this.menuItem13.Enabled = ((bool)(resources.GetObject("menuItem13.Enabled")));
            this.menuItem13.Index = 11;
            this.menuItem13.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem13.Shortcut")));
            this.menuItem13.ShowShortcut = ((bool)(resources.GetObject("menuItem13.ShowShortcut")));
            this.menuItem13.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOr) == null) ? resources.GetString("menuItem13.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOr);
            this.menuItem13.Visible = ((bool)(resources.GetObject("menuItem13.Visible")));
            this.menuItem13.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem14
            // 
            this.menuItem14.Enabled = ((bool)(resources.GetObject("menuItem14.Enabled")));
            this.menuItem14.Index = 12;
            this.menuItem14.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem14.Shortcut")));
            this.menuItem14.ShowShortcut = ((bool)(resources.GetObject("menuItem14.ShowShortcut")));
            this.menuItem14.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexEscapeChar) == null) ? resources.GetString("menuItem15.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexEscapeChar);
            this.menuItem14.Visible = ((bool)(resources.GetObject("menuItem14.Visible")));
            this.menuItem14.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem15
            // 
            this.menuItem15.Enabled = ((bool)(resources.GetObject("menuItem15.Enabled")));
            this.menuItem15.Index = 13;
            this.menuItem15.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem15.Shortcut")));
            this.menuItem15.ShowShortcut = ((bool)(resources.GetObject("menuItem15.ShowShortcut")));
            this.menuItem15.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexTag) == null) ? resources.GetString("menuItem15.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexTag);
            this.menuItem15.Visible = ((bool)(resources.GetObject("menuItem15.Visible")));
            this.menuItem15.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem16
            // 
            this.menuItem16.Enabled = ((bool)(resources.GetObject("menuItem16.Enabled")));
            this.menuItem16.Index = 14;
            this.menuItem16.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem16.Shortcut")));
            this.menuItem16.ShowShortcut = ((bool)(resources.GetObject("menuItem16.ShowShortcut")));
            this.menuItem16.Text = resources.GetString("menuItem16.Text");
            this.menuItem16.Visible = ((bool)(resources.GetObject("menuItem16.Visible")));
            // 
            // menuItem17
            // 
            this.menuItem17.Enabled = ((bool)(resources.GetObject("menuItem17.Enabled")));
            this.menuItem17.Index = 15;
            this.menuItem17.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem17.Shortcut")));
            this.menuItem17.ShowShortcut = ((bool)(resources.GetObject("menuItem17.ShowShortcut")));
            this.menuItem17.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexIdentifier) == null) ? resources.GetString("menuItem17.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexIdentifier);
            this.menuItem17.Visible = ((bool)(resources.GetObject("menuItem17.Visible")));
            this.menuItem17.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem18
            // 
            this.menuItem18.Enabled = ((bool)(resources.GetObject("menuItem18.Enabled")));
            this.menuItem18.Index = 16;
            this.menuItem18.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem18.Shortcut")));
            this.menuItem18.ShowShortcut = ((bool)(resources.GetObject("menuItem18.ShowShortcut")));
            this.menuItem18.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexQuotedString) == null) ? resources.GetString("menuItem18.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexQuotedString);
            this.menuItem18.Visible = ((bool)(resources.GetObject("menuItem18.Visible")));
            this.menuItem18.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem19
            // 
            this.menuItem19.Enabled = ((bool)(resources.GetObject("menuItem19.Enabled")));
            this.menuItem19.Index = 17;
            this.menuItem19.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem19.Shortcut")));
            this.menuItem19.ShowShortcut = ((bool)(resources.GetObject("menuItem19.ShowShortcut")));
            this.menuItem19.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSpaceorTab) == null) ? resources.GetString("menuItem19.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSpaceorTab);
            this.menuItem19.Visible = ((bool)(resources.GetObject("menuItem19.Visible")));
            this.menuItem19.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // menuItem20
            // 
            this.menuItem20.Enabled = ((bool)(resources.GetObject("menuItem20.Enabled")));
            this.menuItem20.Index = 18;
            this.menuItem20.Shortcut = ((System.Windows.Forms.Shortcut)(resources.GetObject("menuItem20.Shortcut")));
            this.menuItem20.ShowShortcut = ((bool)(resources.GetObject("menuItem20.ShowShortcut")));
            this.menuItem20.Text = Localizer.GetString(Localizer.EditResourceIdentifiers.RegexInteger);// (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexInteger) == null) ? resources.GetString("menuItem20.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexInteger);
            this.menuItem20.Visible = ((bool)(resources.GetObject("menuItem20.Visible")));
            this.menuItem20.Click += new System.EventHandler(this.Regexp_Insert);
            // 
            // FrmFindDialog
            // 
            this.AcceptButton = this.btnFind;
            this.AccessibleDescription = resources.GetString("$this.AccessibleDescription");
            this.AccessibleName = resources.GetString("$this.AccessibleName");
            this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
            this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
            this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
            this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.CancelButton = this.btnClose;
            this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnTempaltes);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.chkCase);
            this.Controls.Add(this.cmbFind);
            this.Controls.Add(this.lblFind);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnMarkAll);
            this.Controls.Add(this.chkWholeWord);
            this.Controls.Add(this.chkHidden);
            this.Controls.Add(this.chkUp);
            this.Controls.Add(this.chkRegular);
            this.Controls.Add(this.chkWrap);
            this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
            this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
            this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
            this.MaximizeBox = false;
            this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
            this.MinimizeBox = false;
            this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
            this.Name = "FrmFindDialog";
            this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
            this.RightToLeftLayout = true;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
            this.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDTitle) == null) ? resources.GetString("$this.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDTitle);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Selects text in find combo box and focuses it.
        /// </summary>
        public void SelectTextAndFocus()
        {
            cmbFind.Focus();
            cmbFind.SelectAll();
        }

        /// <summary>
        /// This flag indicates whether to store the physical point when selection is made.
        /// </summary>
        private bool flag = true;

        /// <summary>
        /// This flag indicates whether to cycle back the searching while finding text in selected region.
        /// </summary>
        private bool flagForRotateFind = false;
        /// <summary>
        /// It indicates that whether word is available or not
        /// </summary>
        private int count = 0;

        /// <summary>
        /// IPasePoint throug which we store the selected bottom physical point if only if flag==1
        /// </summary>
        private IParsePoint bottomPhysicalPoint;
        /// <summary>
        /// IPasePoint throug which we store the selected top physical point if only if flag==1
        /// </summary>
        private IParsePoint topPhysicalPoint;

        /// <summary>
        /// Invokes searching process.
        /// </summary>
        /// <returns>Find result.</returns>
        public virtual FindNextResult FindNext()
        {
            FindNextResult result = FindNextResult.Error;
            FindResult findResult = FindResult.Empty;
            string findtext = cmbFind.Text;
            if (cmbFind.Text.Length != 0)
            {
                if (!cmbFind.Items.Contains(cmbFind.Text))
                {
                    m_history.Insert(0, cmbFind.Text);
                    cmbFind.Items.Insert(0, cmbFind.Text);
                }
                else
                {
                    // Sets the recently used item at the zeroth index.
                    m_history.Remove(cmbFind.Text);
                    cmbFind.Items.Remove(cmbFind.Text);
                    if (cmbFind.Text == string.Empty)
                        cmbFind.Text = findtext;
                    m_history.Insert(0, cmbFind.Text);
                    cmbFind.Items.Insert(0, cmbFind.Text);
                }

                bool bSearchInHidden = ((m_attributes & SearchAttributes.SearchHidden) == SearchAttributes.SearchHidden);
                bool bSearchUp = ((m_attributes & SearchAttributes.SearchUp) == SearchAttributes.SearchUp);

                Regex expression = CreateSearchRegex();

                // If expression could not be created, than we just do nothing.
                if (expression != null)
                {
                    IParsePoint position = m_control.GetRealCursorPosition();
                    if (m_bInSelection)
                    {

                        if(flag)
                        {
                            position = m_control.Selection.Start.PhysicalPoint;
                            bottomPhysicalPoint = m_control.Selection.Bottom.PhysicalPoint;
                            topPhysicalPoint = m_control.Selection.Top.PhysicalPoint;
                            flag = false;

                            m_stColumn = m_control.Selection.Start.VirtualColumn;
                            m_stLine = m_control.Selection.Start.VirtualLine;
                            m_edColumn = m_control.Selection.End.VirtualColumn;
                            m_edLine = m_control.Selection.End.VirtualLine;
                           
                        }
                      
                    }

                    string selectedText = m_control.SelectedText;

                    if (selectedText != string.Empty && expression.IsMatch(selectedText))
                    {
                        if (m_bInSelection)
                        {
                            m_bInSelection = false;
                            flagForRotateFind = true;
                            position = bSearchUp ? bottomPhysicalPoint : topPhysicalPoint;
                        }

                        else
                        {
                            position = bSearchUp ? m_control.Selection.Top.PhysicalPoint : m_control.Selection.Bottom.PhysicalPoint;
                        }
                    }
                   
                    findResult = m_control.GetFindResult(position,expression, bSearchInHidden, bSearchUp, cmbFind.Text);

                    if (!findResult.IsEmpty)
                    {
                        if (flagForRotateFind || m_bInSelection)
                        {
                            if (!bSearchUp && (findResult.StartPoint == null || findResult.StartPoint.Line > bottomPhysicalPoint.Line || findResult.StartPoint.Line < topPhysicalPoint.Line))
                            {
                                if (count == 0)
                                {
                                    ResetFlags();
                                    return RaiseOnFound(FindResult.Empty, FindNextResult.NotFound);
                                    
                                }
                                findResult = m_control.GetFindResult(topPhysicalPoint, expression, bSearchInHidden, bSearchUp, cmbFind.Text);
                               
                            }

                            if (bSearchUp && (findResult.StartPoint == null || findResult.StartPoint.Line < topPhysicalPoint.Line || findResult.StartPoint.Line > bottomPhysicalPoint.Line))
                            {
                                if (count == 0)
                                {
                                    ResetFlags();
                                    return RaiseOnFound(FindResult.Empty, FindNextResult.NotFound);

                                }

                               findResult = m_control.GetFindResult(bottomPhysicalPoint, expression, bSearchInHidden, bSearchUp, cmbFind.Text);
                               
                            }

                        }

                        m_control.MarkSearchResult(findResult, bSearchUp);
                        count++;
                        result = findResult.Result.Success ? FindNextResult.Ok : FindNextResult.NotFound;

                        if (findResult.Result.Success && m_ptSearchStartPosition != null)
                        {
                            if (m_ptSearchStartPosition == findResult.StartPoint && m_SearchStartString == cmbFind.Text)
                            {
                                bool local_rdbSelection = rdbSelection.Checked;


                                FindCompleteEventArgs args = new FindCompleteEventArgs(Localizer.GetString(Localizer.EditResourceIdentifiers.StringFindCompleteEventArgs));
                                OnFindComplete(args);

                                Form frmObj = m_control.TopLevelControl.FindForm();

                                MessageBox.Show(args.Message, frmObj.Text);

                                

                                if (m_stLine != 0)
                                {
                                    m_control.StartSelection(m_stColumn, m_stLine);
                                    m_control.StopSelection(m_edColumn, m_edLine);

                                    if (local_rdbSelection)
                                    {
                                        rdbSelection.Checked = true;
                                        
                                    }
                                   
                                    flag = true;
                                    
                                }
                                m_stLine = 0;
                                m_ptSearchStartPosition = null;
                                                                                                                              
                            }
                        }

                        else if (m_ptSearchStartPosition == null)
                        {
                            m_ptSearchStartPosition = findResult.StartPoint;
                            m_SearchStartString = cmbFind.Text;
                        }

                    }
                    else
                    {
                        result = FindNextResult.NotFound;
                    }
                }
            }
            if (findResult.Result.Success)
            {
                if (this.m_control != null)
                    this.m_control.RaiseFindAndReplaceEvent();
            }
            return RaiseOnFound(findResult, result);
        }

        /// <summary>
        /// Handling the event when find completed the specified text
        /// </summary>
        public event EventHandler<FindCompleteEventArgs> FindComplete;

        /// <summary>
        /// Handles the event when FindNext founds the search text.
        /// </summary>
        public event EventHandler<FindNextEventArgs> Found;

        /// <summary>
        /// Fired on OnFindComplete 
        /// </summary>
        /// <param name="e">FindCompleteEvent Args</param>
        protected virtual void OnFindComplete(FindCompleteEventArgs e)
        {
            if (FindComplete != null)
            {
                FindComplete(this, e);
            }
        }

        /// <summary>
        /// Sets the Localizable text as message when find reached the start point
        /// </summary>
        public class FindNextEventArgs : EventArgs
        {
            FindNextResult findNextResult = FindNextResult.Error;
            FindResult findResult = FindResult.Empty;

            /// <summary>
            /// Gets FindNextResult of current Find progress.
            /// </summary>
            public FindNextResult FindNextResult
            {
                get { return findNextResult; }
            }

            /// <summary>
            /// FindResult of current Find progress.
            /// </summary>
            public FindResult FindResult
            {
                get { return findResult; }
            }

            /// <summary>
            /// Initilaizes a new instance of the FindNextEventArgs.
            /// </summary>
            /// <param name="findResult">FindResult of current Find Process</param>
            /// <param name="findNextResult">FindNextResult of current Find Process</param>
            public FindNextEventArgs(FindResult findResult, FindNextResult findNextResult)
            {
                this.findResult = findResult;
                this.findNextResult = findNextResult;
            }
        }

        /// <summary>
        /// Sets the Localizable text as message when find reached the start point
        /// </summary>
        public class FindCompleteEventArgs : EventArgs
        {
            string messge = string.Empty;

            /// <summary>
            /// Initilaizes a new instance of the FindCompleteEventArgs class.
            /// </summary>
            /// <param name="msg">Set the message</param>
            public FindCompleteEventArgs(string msg)
            {
                this.messge = msg;
            }

            /// <summary>
            /// Gets or sets the message
            /// </summary>
            public string Message
            {
                get
                {
                    return this.messge;
                }
                set
                {
                    if (this.messge != value)
                        this.messge = value;
                }
            }
        }
        /// <summary>
        /// Marks all found text in document.
        /// </summary>
        public void MarkAll()
        {
            if (cmbFind.Text.Length == 0) return;

            if (!cmbFind.Items.Contains(cmbFind.Text))
            {
                m_history.Insert(0, cmbFind.Text);
                cmbFind.Items.Insert(0, cmbFind.Text);
            }

            bool bSearchUp = ((m_attributes & SearchAttributes.SearchUp) == SearchAttributes.SearchUp);

            Regex expression = CreateSearchRegex();

            if (expression == null) return;

            IParsePoint position =
                        m_bInSelection
                        ? m_control.Selection.Top.PhysicalPoint
                        : m_control.Parser.GetParsePoint(1, 1);

            while (true)
            {
                FindResult res = m_control.FindRegex(position, expression, bSearchUp, false);

                if (res.Result == null || !res.Result.Success)
                {
                    break;
                }

                if (m_bInSelection)
                {
                    IParsePoint p = m_control.Selection.Bottom.PhysicalPoint;
                    int iSelEnd = p.Line;

                    if (1 == p.Position) iSelEnd--;

                    if (res.StartPoint.Line > iSelEnd) break;
                }

                m_control.Parser.EnsureVisibility(res.StartPoint);
                Point virtPoint = m_control.Parser.PhysicalToVirtual(res.StartPoint);
                m_control.Bookmarks.BookmarkAdd(virtPoint.Y);

                if (virtPoint.Y == m_control.Parser.TotalLines)
                    break;

                position = m_control.Parser.GetParsePoint(virtPoint.Y + 1, 1);
            }
        }

        /// <summary>
        /// Creates regular expression object that can be used for search.
        /// </summary>
        /// <returns>Regular expression object or null if there is no sufficient information for search.</returns>
        public Regex CreateSearchRegex()
        {
            string searchText = cmbFind.Text;

            if (searchText == string.Empty) return null;

            RegexOptions opt = RegexOptions.None;

            bool bUseRegExp =
                (m_attributes & SearchAttributes.UseRegexp) == SearchAttributes.UseRegexp;

            if (!bUseRegExp)
                searchText = Regex.Escape(searchText);

            // Prepare search data for regular expression.
            if ((m_attributes & SearchAttributes.MatchCase) != SearchAttributes.MatchCase)
                opt = opt | RegexOptions.IgnoreCase;

            opt = opt | RegexOptions.ExplicitCapture;

            if (!bUseRegExp && (m_attributes & SearchAttributes.MatchWholeWord) ==
                SearchAttributes.MatchWholeWord)
                searchText = string.Format(DEF_STR_WHOLEWORD_REGEX_TEMPLATE, StreamsWrapper.DEF_SEARCH_DATA_GROUP, searchText);

            Regex expression = null;

            try
            {
                expression = new Regex(searchText, opt);
            }
            catch
            {
                MessageBox.Show(this,Localizer.GetString(Localizer.EditResourceIdentifiers.DEF_MSG_REGEX_INCORRECT), Localizer.GetString(Localizer.EditResourceIdentifiers.DEF_MSG_REGEX_INCORRECT_CAPTION), MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);

                cmbFind.Focus();
                expression = null;
            }

            return expression;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Reacts on changing finding text.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void CmbFind_TextChanged(object sender, System.EventArgs e)
        {
            ToggleElementsEnable();
        }

        /// <summary>
        /// Hides dialog.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            
            ResetFlags();
            HideDialog();
            
        }

        /// <summary>
        /// Changes searching source.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void RadioButton_CheckedChanged(object sender, System.EventArgs e)
        {
            m_bInSelection = sender == rdbSelection;

            if (!rdbSelection.Checked && !rdbDocument.Checked)
            {
                rdbDocument.Checked = true;
            }

            ResetFlags();
            ////if( cmbFind.Text.Length > 0 )
            ////{
            ////  btnMarkAll.Enabled = !m_bInSelection;
            ////}
        }

        /// <summary>
        /// Reset flags for finding word in selected text area
        /// </summary>
        private void ResetFlags()
        {
            this.flagForRotateFind = false;
            this.count = 0;
            this.flag = true;
        }
        /// <summary>
        /// Changes searching attributes.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void SearchAttr_Changed(object sender, System.EventArgs e)
        {
            m_attributes = SearchAttributes.Unknown;

            if (chkCase.Checked) m_attributes |= SearchAttributes.MatchCase;
            if (chkHidden.Checked) m_attributes |= SearchAttributes.SearchHidden;
            if (chkRegular.Checked) m_attributes |= SearchAttributes.UseRegexp;
            if (chkUp.Checked) m_attributes |= SearchAttributes.SearchUp;
            if (chkWholeWord.Checked) m_attributes |= SearchAttributes.MatchWholeWord;

            ToggleButtons();
        }

        /// <summary>
        /// Changes searching attributes.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void WrapAroundSearch_Changed(object sender, System.EventArgs e)
        {
            m_control.WrapAroundSearch = chkWrap.Checked;
        }
        /// <summary>
        /// Defines search type and invokes searching process.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void Find_Text(object sender, System.EventArgs e)
        {
            m_searchType = (sender == btnFind) ?
                SearchType.FindNext : SearchType.MarkAll;

            if (FindNext() == FindNextResult.NotFound)
                MessageBox.Show(this as IWin32Window,Localizer.GetString(Localizer.DEF_MSG_FOUND_NOTHING), Localizer.GetString(Localizer.DEF_MSG_FOUND_NOTHING_CAPTION));
        }

        /// <summary>
        /// Marks all.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void BtnMarkAll_Click(object sender, System.EventArgs e)
        {
            MarkAll();
        }

        /// <summary>
        /// Raises when selected item from history.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void CmbFind_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!m_bIndexChaged) return;

            string text = (string)cmbFind.Items[cmbFind.SelectedIndex];

            m_ptSearchStartPosition = null;

            if (m_history.Contains(text))
            {
                int index = cmbFind.SelectedIndex;
                m_history.RemoveAt(index);
                m_history.Insert(0, text);
            }
        }

        /// <summary>
        /// Shows context menu.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event argument</param>
        private void BtnTempaltes_Click(object sender, System.EventArgs e)
        {
            m_activeCombo = cmbFind;
            Point menuPoint = btnTempaltes.PointToClient(Control.MousePosition);
            cmnTemplates.Show(btnTempaltes, menuPoint);
        }

        /// <summary>
        /// Inserts Regexp template to combo box.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The EventArgs.</param>
        protected void Regexp_Insert(object sender, System.EventArgs e)
        {
            if (sender is MenuItem == false) return;

            int index = cmnTemplates.MenuItems.IndexOf(sender as MenuItem);

            if (index >= DEF_REGEXP.Length || index < 0) return;

            ComboBox activeCombo = GetActiveComboBox();

            string template = DEF_REGEXP[index];
            string text = activeCombo.Text;
            string newText = String.Empty;

            int iPlaceToInsertText = template.IndexOf(" ");

            if (iPlaceToInsertText >= 0)
            {
                newText = template.Remove(iPlaceToInsertText, 1).Insert(iPlaceToInsertText, text);
            }
            else
            {
                newText = text + template;
            }

            activeCombo.Text = newText;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Overriden. Hides dialog.
        /// </summary>
        /// <param name="e">The CancelEventArgs.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            m_bIndexChaged = false;
            e.Cancel = true;
            base.OnClosing(e);

            HideDialog();
        }
        private void LocalizeFindDialog()
        {
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FrmFindDialog));
            this.lblFind.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDMain) == null) ? resources.GetString("lblFind.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDMain);
            this.chkCase.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkCase) == null) ? resources.GetString("chkCase.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkCase);
            this.btnFind.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnFind) == null) ? resources.GetString("btnFind.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnFind);
            this.btnClose.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnClose) == null) ? resources.GetString("btnClose.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnClose);
            this.btnTempaltes.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnTempaltes) == null) ? resources.GetString("btnTempaltes.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnTempaltes);
            this.btnMarkAll.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnMarkAll) == null) ? resources.GetString("btnMarkAll.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDbtnMarkAll);
            this.chkWholeWord.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWholeWord) == null) ? resources.GetString("chkWholeWord.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWholeWord);
            this.chkHidden.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkHidden) == null) ? resources.GetString("chkHidden.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkHidden);
            this.chkUp.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkUp) == null) ? resources.GetString("chkUp.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkUp);
            this.chkRegular.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkRegular) == null) ? resources.GetString("chkRegular.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkRegular);
            this.chkWrap.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWrap) == null) ? resources.GetString("chkWrap.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDchkWrap);
            this.groupBox1.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDGroupTitle) == null) ? resources.GetString("groupBox1.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDGroupTitle);
            this.rdbDocument.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbDocument) == null) ? resources.GetString("rdbDocument.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbDocument);
            this.rdbSelection.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbSelection) == null) ? resources.GetString("rdbSelection.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDrdbSelection);
            this.menuItem1.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSingleChar) == null) ? resources.GetString("menuItem1.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSingleChar);
            this.menuItem2.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexZeroOrMore) == null) ? resources.GetString("menuItem2.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexZeroOrMore);
            this.menuItem3.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOneorMore) == null) ? resources.GetString("menuItem3.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOneorMore);
            this.menuItem4.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBegining) == null) ? resources.GetString("menuItem4.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBegining);
            this.menuItem6.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineEnd) == null) ? resources.GetString("menuItem6.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineEnd);
            this.menuItem7.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexBeginEndWord) == null) ? resources.GetString("menuItem7.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexBeginEndWord);
            this.menuItem9.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBreak) == null) ? resources.GetString("menuItem9.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexLineBreak);
            this.menuItem10.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyOneCharset) == null) ? resources.GetString("menuItem10.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyOneCharset);
            this.menuItem12.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyCharset) == null) ? resources.GetString("menuItem12.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexAnyCharset);
            this.menuItem13.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOr) == null) ? resources.GetString("menuItem13.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexOr);
            this.menuItem14.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexEscapeChar) == null) ? resources.GetString("menuItem15.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexEscapeChar);
            this.menuItem15.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexTag) == null) ? resources.GetString("menuItem15.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexTag);
            this.menuItem17.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexIdentifier) == null) ? resources.GetString("menuItem17.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexIdentifier);
            this.menuItem18.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexQuotedString) == null) ? resources.GetString("menuItem18.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexQuotedString);
            this.menuItem19.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSpaceorTab) == null) ? resources.GetString("menuItem19.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexSpaceorTab);
            this.menuItem20.Text = Localizer.GetString(Localizer.EditResourceIdentifiers.RegexInteger);// (Localizer.GetString(Localizer.EditResourceIdentifiers.RegexInteger) == null) ? resources.GetString("menuItem20.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.RegexInteger);
            this.Text = (Localizer.GetString(Localizer.EditResourceIdentifiers.FDTitle) == null) ? resources.GetString("$this.Text") : Localizer.GetString(Localizer.EditResourceIdentifiers.FDTitle);
        }

        /// <summary>
        /// Overriden. Invokes when dialog shows first time.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            ToggleElementsEnable();
            ToggleButtons();
        }

        /// <summary>
        /// Overriden. Raises when dialog is shown.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        protected override void OnActivated(EventArgs e)
        {
            m_bIndexChaged = false;

            base.OnActivated(e);
            LocalizeFindDialog();
            int x = 0;
            this.cmbFind.Location = new Point(this.lblFind.Bounds.X + this.lblFind.Width, this.lblFind.Bounds.Y);
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is CheckBox)
                {
                    if (ctrl.Width > x)
                        x = ctrl.Width;
                }
            }
            this.groupBox1.Location = new Point(x, this.groupBox1.Location.Y);
            this.btnTempaltes.Location = new Point(this.cmbFind.Bounds.X + this.cmbFind.Width + 2, this.cmbFind.Bounds.Y);
            if ((this.btnTempaltes.Location.X + this.btnTempaltes.Width) > (this.groupBox1.Location.X+this.groupBox1.Width))
                x = this.btnTempaltes.Location.X + this.btnTempaltes.Width + 2;
            else
                x = this.groupBox1.Location.X + this.groupBox1.Width+2;
            this.btnMarkAll.Location = new Point(x, this.btnMarkAll.Location.Y);
            this.btnClose.Location = new Point(x, this.btnClose.Location.Y);
            this.btnFind.Location = new Point(x, this.btnTempaltes.Bounds.Y);
            this.Width = this.btnFind.Location.X + this.btnFind.Width +this.btnTempaltes.Width+ 4;

            this.MinimumSize = new Size(this.Width, this.Height);
            cmbFind.Items.Clear();
            cmbFind.Items.AddRange(m_history.ToArray());

            if (m_history.Count > 0 && cmbFind.Text == string.Empty)
            {
                cmbFind.Text = m_searchText;
            }

            cmbFind.Focus();

            m_bIndexChaged = true;
            
            if (m_control.SelectedText != string.Empty && Regex.IsMatch(m_control.SelectedText,"\n"))
            {
                rdbSelection.Enabled = true;
                rdbSelection.Checked = true;
            }
            else
            {
                if (m_control.SelectedText != string.Empty)
                {
                    if (!rdbSelection.Checked)
                    {
                        rdbDocument.Checked = true;
                    }
                    rdbSelection.Enabled = true;
                }
                else
                {
                    rdbSelection.Enabled = false;
                    rdbDocument.Checked = true;
                }
            }

            
        }

        /// <summary>
        /// Enables or disables Regexp button.
        /// </summary>
        protected virtual void ToggleButtons()
        {
            btnTempaltes.Enabled = chkRegular.Checked;
            chkWholeWord.Enabled = !chkRegular.Checked;
        }

        /// <summary>
        /// Returns ComboBox for data inserting.
        /// </summary>
        /// <returns>Active ComboBox.</returns>
        protected virtual ComboBox GetActiveComboBox()
        {
            return cmbFind;
        }

        /// <summary>
        /// Disables or enables elements which depends from searching text.
        /// </summary>
        protected virtual void ToggleElementsEnable()
        {
            if (m_control != null)
            {
                rdbSelection.Enabled = m_control.SelectedText != string.Empty;

                if (cmbFind.Text.Length == 0)
                {
                    btnFind.Enabled = false;
                    btnMarkAll.Enabled = false;
                }
                else
                {
                    btnFind.Enabled = true; ////!rdbSelection.Checked;
                    btnMarkAll.Enabled = true; ////!m_bInSelection;
                }
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Hides dialog.
        /// </summary>
        internal void HideDialog()
        {
            Form form = m_control.FindForm();

            if (form != null)
            {
                if (form.IsMdiChild )
                    form.MdiParent.Activate();
                else 
                    form.Activate();
            }
            this.Hide();
        }

        /// <summary>
        /// Enables double buffering.
        /// </summary>
        public virtual void EnableDoubleBuffering()
        {
            // Set the value of the double-buffering style bits to true.
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);

            this.UpdateStyles();
        }
        #endregion
    }
}