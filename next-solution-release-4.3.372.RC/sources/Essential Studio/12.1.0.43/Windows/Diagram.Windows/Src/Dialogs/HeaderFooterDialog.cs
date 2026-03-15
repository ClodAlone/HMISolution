#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The HeaderFooterDialog provides an interactive form-based interface for initializing the Header and Footer settings of 
    /// a diagram. Initializing the HeaderFooterDialog's <see cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterDialog.Header"/> 
    /// and <see cref="Syncfusion.Windows.Forms.Diagram.HeaderFooterDialog.Header"/> properties with the corresponding 
    /// <see cref="Diagram.Header"/> and <see cref="Diagram.Footer"/> 
    /// members of the diagram model's <see cref="Syncfusion.Windows.Forms.Diagram.Model.HeaderFooterData"/> object will let users 
    /// configure the header/footers settings using the dialog controls.
    /// <p>
    /// Please refer to the DiagramBuilder sample to see the HeaderFooterDialog in use.
    /// </p>
    /// </summary>
    public class HeaderFooterDialog : Form
    {
        #region Fields

        #region Constants
        private const string c_strTEXT_PROPERTY_NAME = "Text";
        private const string c_strSELECTED_ITEM_PROPERTY_NAME = "SelectedItem";
        #endregion Constants

        private PictureBox m_pictureHFBorderPreview;
        private GroupBox m_groupHFSelection;
        private PictureBox m_pictureHFReducedView;
        private Button m_btnHFOK;
        private Button m_btnHFCancel;
        private CheckBox m_checkHFTextVisible;
        private System.Windows.Forms.Label m_lblHFBoundsLeftMargin;
        private System.Windows.Forms.Label m_lblHFBoundsRightMargin;
        private System.Windows.Forms.Label m_lblHFBoundsWidth;
        private System.Windows.Forms.Label m_lblHFBoundsHeight;
        private CheckBox m_checkHFBoundsAutosize;
        private System.Windows.Forms.Label m_lblHFBorderColor;
        private System.Windows.Forms.Label m_lblHFBorderStyle;
        private ComboBox m_comboHFBorderStyle;
        private ComboBox m_comboHFBorderWeight;
        private System.Windows.Forms.Label m_lblHFBorderWeight;
        private CheckBox m_checkHFBorderVisible;
        private RadioButton m_radioHFFooter;
        private RadioButton m_radioHFHeader;
        private Panel m_panelHFBounds;
        private Panel m_panelHFBorder;
        private Button m_btnHFBorderSelectColor;
        private NumericUpDown m_numHFBoundsLeftMargin;
        private NumericUpDown m_numHFBoundsRightMargin;
        private NumericUpDown m_numHFBoundsWidth;
        private NumericUpDown m_numHFBoundsHeight;
        private ContextMenu conMenuHF;
        private MenuItem menuItem6;
        private MenuItem mnuPage;
        private MenuItem mnuTotalPages;
        private MenuItem mnuCurrentTime;
        private MenuItem mnuCurrentDateShort;
        private MenuItem mnuCurrentDateLong;
        private MenuItem mnuHelp;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private Container components = null;

        private Header m_header;
        private Footer m_footer;
        private bool m_bHeader;
        private MeasureUnits m_gphUnit;
        private DeepBindingHelper m_hlpDeepBinding;

        // Hashtable for panel switching
        private Hashtable m_hashPanels;

        // Textbox to add text to
        private TextBox m_txtAddingText;
        private bool m_bValidatedImagePath = false;
        private bool m_bInitializing = false;

        #region Reduced Drawing
        private Hashtable m_hashImgLayers;
        private RectangleF m_rectHeaderReduced;
        private RectangleF m_rectFooterReduced;
        private float m_fHeaderScaleFactor;
        private float m_fFooterScaleFactor;
        private float m_fHeaderX;
        private Panel m_panelHFBackgroundImage;
        private TextBox m_txtHFImagePath;
        private PictureBox m_pictureHFImagePreview;
        private ComboBox m_comboHFImageLayout;
        private Button m_btnHFImageChoose;
        private System.Windows.Forms.Label m_lblHFImageLayout;
        private Panel m_panelHFText;
        private System.Windows.Forms.Label m_lblHFTextSample;
        private System.Windows.Forms.Label m_lblHFTextCulture;
        private Button m_btnHFTextLeft;
        private TextBox m_txtHFTextLeft;
        private TextBox m_txtHFTextRight;
        private System.Windows.Forms.Label m_lblHFTextLeft;
        private TextBox m_txtHFTextCenter;
        private System.Windows.Forms.Label m_lblHFTextRight;
        private Button m_btnHFTextRight;
        private Button m_btnHFTextCenter;
        private System.Windows.Forms.Label m_lblHFTextCenter;
        private ComboBox m_comboHFTextCulture;
        private Button m_btnHFTextFontSelection;
        private System.Windows.Forms.Label m_lblHFTextFont;
        private Button m_btnHFTextColorSelection;
        private System.Windows.Forms.Label m_lblHFTextColor;
        private TabControl tabControl1;
        private TabPage m_tpText;
        private TabPage m_tpBackgroundImage;
        private TabPage m_tpBounds;
        private TabPage m_tpBorderStyle;
        private float m_fFooterX;
        #endregion Reduced Drawing        

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the dialog's <see cref="Syncfusion.Windows.Forms.Diagram.Header"/> object.
        /// </summary>
        public Header Header
        {
            get
            {
                return m_header;
            }
            set
            {
                if (m_header != value)
                {
                    if (m_header == null)
                        m_header = (Header)value.Clone();
                }
            }
        }

        /// <summary>
        /// Gets or sets the dialog's <see cref="Syncfusion.Windows.Forms.Diagram.Footer"/> object.
        /// </summary>
        public Footer Footer
        {
            get
            {
                return m_footer;
            }
            set
            {
                if (m_footer != value)
                {
                    if (m_footer == null)
                        m_footer = (Footer)value.Clone();
                }
            }
        }

        /// <summary>
        /// Gets or sets the measurement units.
        /// </summary>
        /// <value>The measurement units.</value>
        [Documentation.DocumentationExclude()]
        public MeasureUnits MeasurementUnits
        {
            get 
            { 
                return m_gphUnit; 
            }
            set
            {
                if (m_gphUnit != value)
                    m_gphUnit = value;
            }
        }

        // Binding bug Workaround
        private DeepBindingHelper BindHelper
        {
            get
            {
                if (m_hlpDeepBinding == null)
                {
                    m_hlpDeepBinding = new DeepBindingHelper(Header, Footer, IsHeader);
                    m_hlpDeepBinding.InitBounds(this.MeasurementUnits);
                }
                return m_hlpDeepBinding;
            }
            set
            {
                if (m_hlpDeepBinding != value)
                    m_hlpDeepBinding = value;
            }
        }

        private bool IsHeader
        {
            get
            {
                return m_bHeader;
            }
            set
            {
                m_bHeader = value;
                BindHelper.IsHeader = value;
            }
        }

        private HeaderFooterBase HF
        {
            get
            {
                object objHF;
                if (IsHeader)
                    objHF = Header;
                else
                    objHF = Footer;
                if (objHF == null)
                    objHF = new HeaderFooterBase();
                return (HeaderFooterBase)objHF;
            }
        }

        private RectangleF ReducedHFRect
        {
            get
            {
                RectangleF rectToreturn;
                if (this.IsHeader)
                    rectToreturn = m_rectHeaderReduced;
                else
                    rectToreturn = m_rectFooterReduced;

                return rectToreturn;
            }
            set
            {
                if (this.IsHeader)
                {
                    if (m_rectHeaderReduced != value)
                        m_rectHeaderReduced = value;
                }
                else
                {
                    if (m_rectFooterReduced != value)
                        m_rectFooterReduced = value;
                }
            }
        }

        private float ScaleFactor
        {
            get
            {
                float fScaleFactor;

                if (this.IsHeader)
                    fScaleFactor = m_fHeaderScaleFactor;
                else
                    fScaleFactor = m_fFooterScaleFactor;

                return fScaleFactor;
            }
            set
            {
                if (this.IsHeader)
                {
                    if (m_fHeaderScaleFactor != value)
                    {
                        m_fHeaderScaleFactor = value;

                        UpdateImageScaleFactor(value);
                    }
                }
                else
                {
                    if (m_fFooterScaleFactor != value)
                        m_fFooterScaleFactor = value;

                    UpdateImageScaleFactor(value);
                }
            }
        }

        private float HFX
        {
            get
            {
                float fValueToReturn;
                if (this.IsHeader)
                    fValueToReturn = m_fHeaderX;
                else
                    fValueToReturn = m_fFooterX;

                return fValueToReturn;
            }
            set
            {
                if (this.IsHeader)
                {
                    if (m_fHeaderX != value)
                        m_fHeaderX = value;
                }
                else
                {
                    if (m_fFooterX != value)
                        m_fFooterX = value;
                }
            }
        }
        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderFooterDialog"/> class.
        /// </summary>
        public HeaderFooterDialog()
        {
            m_bInitializing = true;
            InitializeComponent();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeaderFooterDialog));
            this.m_checkHFTextVisible = new System.Windows.Forms.CheckBox();
            this.m_btnHFOK = new System.Windows.Forms.Button();
            this.m_btnHFCancel = new System.Windows.Forms.Button();
            this.m_lblHFBoundsLeftMargin = new System.Windows.Forms.Label();
            this.m_lblHFBoundsRightMargin = new System.Windows.Forms.Label();
            this.m_lblHFBoundsWidth = new System.Windows.Forms.Label();
            this.m_lblHFBoundsHeight = new System.Windows.Forms.Label();
            this.m_checkHFBoundsAutosize = new System.Windows.Forms.CheckBox();
            this.m_lblHFBorderColor = new System.Windows.Forms.Label();
            this.m_lblHFBorderStyle = new System.Windows.Forms.Label();
            this.m_comboHFBorderStyle = new System.Windows.Forms.ComboBox();
            this.m_comboHFBorderWeight = new System.Windows.Forms.ComboBox();
            this.m_lblHFBorderWeight = new System.Windows.Forms.Label();
            this.m_checkHFBorderVisible = new System.Windows.Forms.CheckBox();
            this.m_radioHFFooter = new System.Windows.Forms.RadioButton();
            this.m_radioHFHeader = new System.Windows.Forms.RadioButton();
            this.m_panelHFBounds = new System.Windows.Forms.Panel();
            this.m_numHFBoundsLeftMargin = new System.Windows.Forms.NumericUpDown();
            this.m_numHFBoundsRightMargin = new System.Windows.Forms.NumericUpDown();
            this.m_numHFBoundsWidth = new System.Windows.Forms.NumericUpDown();
            this.m_numHFBoundsHeight = new System.Windows.Forms.NumericUpDown();
            this.m_panelHFBorder = new System.Windows.Forms.Panel();
            this.m_pictureHFBorderPreview = new System.Windows.Forms.PictureBox();
            this.m_btnHFBorderSelectColor = new System.Windows.Forms.Button();
            this.mnuPage = new System.Windows.Forms.MenuItem();
            this.mnuTotalPages = new System.Windows.Forms.MenuItem();
            this.mnuCurrentTime = new System.Windows.Forms.MenuItem();
            this.mnuCurrentDateShort = new System.Windows.Forms.MenuItem();
            this.mnuCurrentDateLong = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.m_groupHFSelection = new System.Windows.Forms.GroupBox();
            this.m_pictureHFReducedView = new System.Windows.Forms.PictureBox();
            this.conMenuHF = new System.Windows.Forms.ContextMenu();
            this.m_panelHFBackgroundImage = new System.Windows.Forms.Panel();
            this.m_txtHFImagePath = new System.Windows.Forms.TextBox();
            this.m_pictureHFImagePreview = new System.Windows.Forms.PictureBox();
            this.m_comboHFImageLayout = new System.Windows.Forms.ComboBox();
            this.m_btnHFImageChoose = new System.Windows.Forms.Button();
            this.m_lblHFImageLayout = new System.Windows.Forms.Label();
            this.m_lblHFTextColor = new System.Windows.Forms.Label();
            this.m_btnHFTextColorSelection = new System.Windows.Forms.Button();
            this.m_lblHFTextFont = new System.Windows.Forms.Label();
            this.m_btnHFTextFontSelection = new System.Windows.Forms.Button();
            this.m_comboHFTextCulture = new System.Windows.Forms.ComboBox();
            this.m_lblHFTextCenter = new System.Windows.Forms.Label();
            this.m_btnHFTextCenter = new System.Windows.Forms.Button();
            this.m_btnHFTextRight = new System.Windows.Forms.Button();
            this.m_lblHFTextRight = new System.Windows.Forms.Label();
            this.m_txtHFTextCenter = new System.Windows.Forms.TextBox();
            this.m_lblHFTextLeft = new System.Windows.Forms.Label();
            this.m_txtHFTextRight = new System.Windows.Forms.TextBox();
            this.m_txtHFTextLeft = new System.Windows.Forms.TextBox();
            this.m_btnHFTextLeft = new System.Windows.Forms.Button();
            this.m_lblHFTextCulture = new System.Windows.Forms.Label();
            this.m_lblHFTextSample = new System.Windows.Forms.Label();
            this.m_panelHFText = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.m_tpText = new System.Windows.Forms.TabPage();
            this.m_tpBackgroundImage = new System.Windows.Forms.TabPage();
            this.m_tpBounds = new System.Windows.Forms.TabPage();
            this.m_tpBorderStyle = new System.Windows.Forms.TabPage();
            this.m_panelHFBounds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsLeftMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsRightMargin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsHeight)).BeginInit();
            this.m_panelHFBorder.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureHFBorderPreview)).BeginInit();
            this.m_groupHFSelection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureHFReducedView)).BeginInit();
            this.m_panelHFBackgroundImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureHFImagePreview)).BeginInit();
            this.m_panelHFText.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.m_tpText.SuspendLayout();
            this.m_tpBackgroundImage.SuspendLayout();
            this.m_tpBounds.SuspendLayout();
            this.m_tpBorderStyle.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_checkHFTextVisible
            // 
            this.m_checkHFTextVisible.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_checkHFTextVisible, "m_checkHFTextVisible");
            this.m_checkHFTextVisible.Name = "m_checkHFTextVisible";
            this.m_checkHFTextVisible.CheckedChanged += new System.EventHandler(this.HFTextVisible_CheckedChanged);
            // 
            // m_btnHFOK
            // 
            this.m_btnHFOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btnHFOK, "m_btnHFOK");
            this.m_btnHFOK.Name = "m_btnHFOK";
            // 
            // m_btnHFCancel
            // 
            this.m_btnHFCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnHFCancel, "m_btnHFCancel");
            this.m_btnHFCancel.Name = "m_btnHFCancel";
            // 
            // m_lblHFBoundsLeftMargin
            // 
            this.m_lblHFBoundsLeftMargin.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFBoundsLeftMargin, "m_lblHFBoundsLeftMargin");
            this.m_lblHFBoundsLeftMargin.Name = "m_lblHFBoundsLeftMargin";
            // 
            // m_lblHFBoundsRightMargin
            // 
            this.m_lblHFBoundsRightMargin.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFBoundsRightMargin, "m_lblHFBoundsRightMargin");
            this.m_lblHFBoundsRightMargin.Name = "m_lblHFBoundsRightMargin";
            // 
            // m_lblHFBoundsWidth
            // 
            this.m_lblHFBoundsWidth.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFBoundsWidth, "m_lblHFBoundsWidth");
            this.m_lblHFBoundsWidth.Name = "m_lblHFBoundsWidth";
            // 
            // m_lblHFBoundsHeight
            // 
            this.m_lblHFBoundsHeight.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFBoundsHeight, "m_lblHFBoundsHeight");
            this.m_lblHFBoundsHeight.Name = "m_lblHFBoundsHeight";
            // 
            // m_checkHFBoundsAutosize
            // 
            this.m_checkHFBoundsAutosize.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_checkHFBoundsAutosize, "m_checkHFBoundsAutosize");
            this.m_checkHFBoundsAutosize.Name = "m_checkHFBoundsAutosize";
            this.m_checkHFBoundsAutosize.CheckedChanged += new System.EventHandler(this.HFBoundsAutosize_CheckedChanged);
            // 
            // m_lblHFBorderColor
            // 
            this.m_lblHFBorderColor.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFBorderColor, "m_lblHFBorderColor");
            this.m_lblHFBorderColor.Name = "m_lblHFBorderColor";
            // 
            // m_lblHFBorderStyle
            // 
            this.m_lblHFBorderStyle.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFBorderStyle, "m_lblHFBorderStyle");
            this.m_lblHFBorderStyle.Name = "m_lblHFBorderStyle";
            // 
            // m_comboHFBorderStyle
            // 
            this.m_comboHFBorderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboHFBorderStyle, "m_comboHFBorderStyle");
            this.m_comboHFBorderStyle.Name = "m_comboHFBorderStyle";
            this.m_comboHFBorderStyle.SelectedIndexChanged += new System.EventHandler(this.HFBorderStyle_SelectedIndexChanged);
            // 
            // m_comboHFBorderWeight
            // 
            this.m_comboHFBorderWeight.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboHFBorderWeight, "m_comboHFBorderWeight");
            this.m_comboHFBorderWeight.Name = "m_comboHFBorderWeight";
            this.m_comboHFBorderWeight.SelectedIndexChanged += new System.EventHandler(this.HFBorderWeight_SelectedIndexChanged);
            // 
            // m_lblHFBorderWeight
            // 
            this.m_lblHFBorderWeight.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFBorderWeight, "m_lblHFBorderWeight");
            this.m_lblHFBorderWeight.Name = "m_lblHFBorderWeight";
            // 
            // m_checkHFBorderVisible
            // 
            this.m_checkHFBorderVisible.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_checkHFBorderVisible, "m_checkHFBorderVisible");
            this.m_checkHFBorderVisible.Name = "m_checkHFBorderVisible";
            this.m_checkHFBorderVisible.CheckedChanged += new System.EventHandler(this.HFBorderVisible_CheckedChanged);
            // 
            // m_radioHFFooter
            // 
            this.m_radioHFFooter.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_radioHFFooter, "m_radioHFFooter");
            this.m_radioHFFooter.Name = "m_radioHFFooter";
            // 
            // m_radioHFHeader
            // 
            this.m_radioHFHeader.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_radioHFHeader, "m_radioHFHeader");
            this.m_radioHFHeader.Name = "m_radioHFHeader";
            this.m_radioHFHeader.CheckedChanged += new System.EventHandler(this.HFHeader_CheckedChanged);
            // 
            // m_panelHFBounds
            // 
            this.m_panelHFBounds.Controls.Add(this.m_numHFBoundsLeftMargin);
            this.m_panelHFBounds.Controls.Add(this.m_lblHFBoundsWidth);
            this.m_panelHFBounds.Controls.Add(this.m_lblHFBoundsHeight);
            this.m_panelHFBounds.Controls.Add(this.m_checkHFBoundsAutosize);
            this.m_panelHFBounds.Controls.Add(this.m_lblHFBoundsLeftMargin);
            this.m_panelHFBounds.Controls.Add(this.m_lblHFBoundsRightMargin);
            this.m_panelHFBounds.Controls.Add(this.m_numHFBoundsRightMargin);
            this.m_panelHFBounds.Controls.Add(this.m_numHFBoundsWidth);
            this.m_panelHFBounds.Controls.Add(this.m_numHFBoundsHeight);
            resources.ApplyResources(this.m_panelHFBounds, "m_panelHFBounds");
            this.m_panelHFBounds.Name = "m_panelHFBounds";
            // 
            // m_numHFBoundsLeftMargin
            // 
            this.m_numHFBoundsLeftMargin.DecimalPlaces = 2;
            this.m_numHFBoundsLeftMargin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.m_numHFBoundsLeftMargin, "m_numHFBoundsLeftMargin");
            this.m_numHFBoundsLeftMargin.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.m_numHFBoundsLeftMargin.Name = "m_numHFBoundsLeftMargin";
            this.m_numHFBoundsLeftMargin.TextChanged += new System.EventHandler(this.HFBoundsLeftMargin_TextChanged);
            // 
            // m_numHFBoundsRightMargin
            // 
            this.m_numHFBoundsRightMargin.DecimalPlaces = 2;
            this.m_numHFBoundsRightMargin.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.m_numHFBoundsRightMargin, "m_numHFBoundsRightMargin");
            this.m_numHFBoundsRightMargin.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.m_numHFBoundsRightMargin.Name = "m_numHFBoundsRightMargin";
            this.m_numHFBoundsRightMargin.TextChanged += new System.EventHandler(this.HFBoundsRightMargin_TextChanged);
            // 
            // m_numHFBoundsWidth
            // 
            this.m_numHFBoundsWidth.DecimalPlaces = 2;
            this.m_numHFBoundsWidth.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.m_numHFBoundsWidth, "m_numHFBoundsWidth");
            this.m_numHFBoundsWidth.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.m_numHFBoundsWidth.Name = "m_numHFBoundsWidth";
            this.m_numHFBoundsWidth.TextChanged += new System.EventHandler(this.HFBoundsWidth_TextChanged);
            // 
            // m_numHFBoundsHeight
            // 
            this.m_numHFBoundsHeight.DecimalPlaces = 2;
            this.m_numHFBoundsHeight.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            resources.ApplyResources(this.m_numHFBoundsHeight, "m_numHFBoundsHeight");
            this.m_numHFBoundsHeight.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.m_numHFBoundsHeight.Name = "m_numHFBoundsHeight";
            this.m_numHFBoundsHeight.TextChanged += new System.EventHandler(this.HFBoundsHeight_TextChanged);
            // 
            // m_panelHFBorder
            // 
            this.m_panelHFBorder.Controls.Add(this.m_pictureHFBorderPreview);
            this.m_panelHFBorder.Controls.Add(this.m_btnHFBorderSelectColor);
            this.m_panelHFBorder.Controls.Add(this.m_lblHFBorderColor);
            this.m_panelHFBorder.Controls.Add(this.m_lblHFBorderStyle);
            this.m_panelHFBorder.Controls.Add(this.m_comboHFBorderStyle);
            this.m_panelHFBorder.Controls.Add(this.m_comboHFBorderWeight);
            this.m_panelHFBorder.Controls.Add(this.m_lblHFBorderWeight);
            this.m_panelHFBorder.Controls.Add(this.m_checkHFBorderVisible);
            resources.ApplyResources(this.m_panelHFBorder, "m_panelHFBorder");
            this.m_panelHFBorder.Name = "m_panelHFBorder";
            // 
            // m_pictureHFBorderPreview
            // 
            resources.ApplyResources(this.m_pictureHFBorderPreview, "m_pictureHFBorderPreview");
            this.m_pictureHFBorderPreview.Name = "m_pictureHFBorderPreview";
            this.m_pictureHFBorderPreview.TabStop = false;
            // 
            // m_btnHFBorderSelectColor
            // 
            this.m_btnHFBorderSelectColor.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_btnHFBorderSelectColor, "m_btnHFBorderSelectColor");
            this.m_btnHFBorderSelectColor.Name = "m_btnHFBorderSelectColor";
            this.m_btnHFBorderSelectColor.Click += new System.EventHandler(this.HFBorderSelectColor_Click);
            // 
            // mnuPage
            // 
            this.mnuPage.Index = 0;
            resources.ApplyResources(this.mnuPage, "mnuPage");
            this.mnuPage.Click += new System.EventHandler(this.MenuItemPage_Click);
            // 
            // mnuTotalPages
            // 
            this.mnuTotalPages.Index = 1;
            resources.ApplyResources(this.mnuTotalPages, "mnuTotalPages");
            this.mnuTotalPages.Click += new System.EventHandler(this.MenuTotalPages_Click);
            // 
            // mnuCurrentTime
            // 
            this.mnuCurrentTime.Index = 2;
            resources.ApplyResources(this.mnuCurrentTime, "mnuCurrentTime");
            this.mnuCurrentTime.Click += new System.EventHandler(this.MenuCurrentTime_Click);
            // 
            // mnuCurrentDateShort
            // 
            this.mnuCurrentDateShort.Index = 3;
            resources.ApplyResources(this.mnuCurrentDateShort, "mnuCurrentDateShort");
            this.mnuCurrentDateShort.Click += new System.EventHandler(this.MenuCurrentDateShort_Click);
            // 
            // mnuCurrentDateLong
            // 
            this.mnuCurrentDateLong.Index = 4;
            resources.ApplyResources(this.mnuCurrentDateLong, "mnuCurrentDateLong");
            this.mnuCurrentDateLong.Click += new System.EventHandler(this.MenuCurrentDateLong_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 5;
            resources.ApplyResources(this.menuItem6, "menuItem6");
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 6;
            resources.ApplyResources(this.mnuHelp, "mnuHelp");
            // 
            // m_groupHFSelection
            // 
            this.m_groupHFSelection.Controls.Add(this.m_radioHFFooter);
            this.m_groupHFSelection.Controls.Add(this.m_radioHFHeader);
            this.m_groupHFSelection.Controls.Add(this.m_checkHFTextVisible);
            this.m_groupHFSelection.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            resources.ApplyResources(this.m_groupHFSelection, "m_groupHFSelection");
            this.m_groupHFSelection.Name = "m_groupHFSelection";
            this.m_groupHFSelection.TabStop = false;
            // 
            // m_pictureHFReducedView
            // 
            resources.ApplyResources(this.m_pictureHFReducedView, "m_pictureHFReducedView");
            this.m_pictureHFReducedView.Name = "m_pictureHFReducedView";
            this.m_pictureHFReducedView.TabStop = false;
            // 
            // conMenuHF
            // 
            this.conMenuHF.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuPage,
            this.mnuTotalPages,
            this.mnuCurrentTime,
            this.mnuCurrentDateShort,
            this.mnuCurrentDateLong,
            this.menuItem6,
            this.mnuHelp});
            // 
            // m_panelHFBackgroundImage
            // 
            this.m_panelHFBackgroundImage.Controls.Add(this.m_txtHFImagePath);
            this.m_panelHFBackgroundImage.Controls.Add(this.m_pictureHFImagePreview);
            this.m_panelHFBackgroundImage.Controls.Add(this.m_comboHFImageLayout);
            this.m_panelHFBackgroundImage.Controls.Add(this.m_btnHFImageChoose);
            this.m_panelHFBackgroundImage.Controls.Add(this.m_lblHFImageLayout);
            resources.ApplyResources(this.m_panelHFBackgroundImage, "m_panelHFBackgroundImage");
            this.m_panelHFBackgroundImage.Name = "m_panelHFBackgroundImage";
            // 
            // m_txtHFImagePath
            // 
            this.m_txtHFImagePath.AcceptsReturn = true;
            resources.ApplyResources(this.m_txtHFImagePath, "m_txtHFImagePath");
            this.m_txtHFImagePath.Name = "m_txtHFImagePath";
            this.m_txtHFImagePath.TextChanged += new System.EventHandler(this.HFImagePath_TextChanged);
            // 
            // m_pictureHFImagePreview
            // 
            this.m_pictureHFImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.m_pictureHFImagePreview, "m_pictureHFImagePreview");
            this.m_pictureHFImagePreview.Name = "m_pictureHFImagePreview";
            this.m_pictureHFImagePreview.TabStop = false;
            // 
            // m_comboHFImageLayout
            // 
            this.m_comboHFImageLayout.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboHFImageLayout, "m_comboHFImageLayout");
            this.m_comboHFImageLayout.Name = "m_comboHFImageLayout";
            this.m_comboHFImageLayout.SelectedIndexChanged += new System.EventHandler(this.HFImageLayout_SelectedIndexChanged);
            // 
            // m_btnHFImageChoose
            // 
            this.m_btnHFImageChoose.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_btnHFImageChoose, "m_btnHFImageChoose");
            this.m_btnHFImageChoose.Name = "m_btnHFImageChoose";
            this.m_btnHFImageChoose.Click += new System.EventHandler(this.HFImageChoose_Click);
            // 
            // m_lblHFImageLayout
            // 
            this.m_lblHFImageLayout.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFImageLayout, "m_lblHFImageLayout");
            this.m_lblHFImageLayout.Name = "m_lblHFImageLayout";
            // 
            // m_lblHFTextColor
            // 
            this.m_lblHFTextColor.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFTextColor, "m_lblHFTextColor");
            this.m_lblHFTextColor.Name = "m_lblHFTextColor";
            // 
            // m_btnHFTextColorSelection
            // 
            this.m_btnHFTextColorSelection.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_btnHFTextColorSelection, "m_btnHFTextColorSelection");
            this.m_btnHFTextColorSelection.Name = "m_btnHFTextColorSelection";
            this.m_btnHFTextColorSelection.Click += new System.EventHandler(this.HFTextColorSelection_Click);
            // 
            // m_lblHFTextFont
            // 
            this.m_lblHFTextFont.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFTextFont, "m_lblHFTextFont");
            this.m_lblHFTextFont.Name = "m_lblHFTextFont";
            // 
            // m_btnHFTextFontSelection
            // 
            this.m_btnHFTextFontSelection.AutoEllipsis = true;
            this.m_btnHFTextFontSelection.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_btnHFTextFontSelection, "m_btnHFTextFontSelection");
            this.m_btnHFTextFontSelection.Name = "m_btnHFTextFontSelection";
            this.m_btnHFTextFontSelection.Click += new System.EventHandler(this.HFTextFontSelection_Click);
            // 
            // m_comboHFTextCulture
            // 
            this.m_comboHFTextCulture.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboHFTextCulture, "m_comboHFTextCulture");
            this.m_comboHFTextCulture.Name = "m_comboHFTextCulture";
            this.m_comboHFTextCulture.SelectedIndexChanged += new System.EventHandler(this.HFTextCulture_SelectedIndexChanged);
            // 
            // m_lblHFTextCenter
            // 
            this.m_lblHFTextCenter.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFTextCenter, "m_lblHFTextCenter");
            this.m_lblHFTextCenter.Name = "m_lblHFTextCenter";
            // 
            // m_btnHFTextCenter
            // 
            this.m_btnHFTextCenter.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_btnHFTextCenter, "m_btnHFTextCenter");
            this.m_btnHFTextCenter.Name = "m_btnHFTextCenter";
            this.m_btnHFTextCenter.Click += new System.EventHandler(this.HFTextCenter_Click);
            // 
            // m_btnHFTextRight
            // 
            this.m_btnHFTextRight.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_btnHFTextRight, "m_btnHFTextRight");
            this.m_btnHFTextRight.Name = "m_btnHFTextRight";
            this.m_btnHFTextRight.Click += new System.EventHandler(this.HFTextRight_Click);
            // 
            // m_lblHFTextRight
            // 
            this.m_lblHFTextRight.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFTextRight, "m_lblHFTextRight");
            this.m_lblHFTextRight.Name = "m_lblHFTextRight";
            // 
            // m_txtHFTextCenter
            // 
            resources.ApplyResources(this.m_txtHFTextCenter, "m_txtHFTextCenter");
            this.m_txtHFTextCenter.Name = "m_txtHFTextCenter";
            this.m_txtHFTextCenter.TextChanged += new System.EventHandler(this.HFTextCenter_TextChanged);
            // 
            // m_lblHFTextLeft
            // 
            this.m_lblHFTextLeft.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFTextLeft, "m_lblHFTextLeft");
            this.m_lblHFTextLeft.Name = "m_lblHFTextLeft";
            // 
            // m_txtHFTextRight
            // 
            resources.ApplyResources(this.m_txtHFTextRight, "m_txtHFTextRight");
            this.m_txtHFTextRight.Name = "m_txtHFTextRight";
            this.m_txtHFTextRight.TextChanged += new System.EventHandler(this.HFTextRight_TextChanged);
            // 
            // m_txtHFTextLeft
            // 
            resources.ApplyResources(this.m_txtHFTextLeft, "m_txtHFTextLeft");
            this.m_txtHFTextLeft.Name = "m_txtHFTextLeft";
            this.m_txtHFTextLeft.TextChanged += new System.EventHandler(this.HFTextLeft_TextChanged);
            // 
            // m_btnHFTextLeft
            // 
            this.m_btnHFTextLeft.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_btnHFTextLeft, "m_btnHFTextLeft");
            this.m_btnHFTextLeft.Name = "m_btnHFTextLeft";
            this.m_btnHFTextLeft.Click += new System.EventHandler(this.HFTextLeft_Click);
            // 
            // m_lblHFTextCulture
            // 
            this.m_lblHFTextCulture.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            resources.ApplyResources(this.m_lblHFTextCulture, "m_lblHFTextCulture");
            this.m_lblHFTextCulture.Name = "m_lblHFTextCulture";
            // 
            // m_lblHFTextSample
            // 
            this.m_lblHFTextSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_lblHFTextSample.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblHFTextSample, "m_lblHFTextSample");
            this.m_lblHFTextSample.Name = "m_lblHFTextSample";
            // 
            // m_panelHFText
            // 
            this.m_panelHFText.Controls.Add(this.m_lblHFTextSample);
            this.m_panelHFText.Controls.Add(this.m_lblHFTextCulture);
            this.m_panelHFText.Controls.Add(this.m_btnHFTextLeft);
            this.m_panelHFText.Controls.Add(this.m_txtHFTextLeft);
            this.m_panelHFText.Controls.Add(this.m_txtHFTextRight);
            this.m_panelHFText.Controls.Add(this.m_lblHFTextLeft);
            this.m_panelHFText.Controls.Add(this.m_txtHFTextCenter);
            this.m_panelHFText.Controls.Add(this.m_lblHFTextRight);
            this.m_panelHFText.Controls.Add(this.m_btnHFTextRight);
            this.m_panelHFText.Controls.Add(this.m_btnHFTextCenter);
            this.m_panelHFText.Controls.Add(this.m_lblHFTextCenter);
            this.m_panelHFText.Controls.Add(this.m_comboHFTextCulture);
            this.m_panelHFText.Controls.Add(this.m_btnHFTextFontSelection);
            this.m_panelHFText.Controls.Add(this.m_lblHFTextFont);
            this.m_panelHFText.Controls.Add(this.m_btnHFTextColorSelection);
            this.m_panelHFText.Controls.Add(this.m_lblHFTextColor);
            resources.ApplyResources(this.m_panelHFText, "m_panelHFText");
            this.m_panelHFText.Name = "m_panelHFText";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.m_tpText);
            this.tabControl1.Controls.Add(this.m_tpBackgroundImage);
            this.tabControl1.Controls.Add(this.m_tpBounds);
            this.tabControl1.Controls.Add(this.m_tpBorderStyle);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // m_tpText
            // 
            this.m_tpText.Controls.Add(this.m_panelHFText);
            resources.ApplyResources(this.m_tpText, "m_tpText");
            this.m_tpText.Name = "m_tpText";
            this.m_tpText.UseVisualStyleBackColor = true;
            // 
            // m_tpBackgroundImage
            // 
            this.m_tpBackgroundImage.Controls.Add(this.m_panelHFBackgroundImage);
            resources.ApplyResources(this.m_tpBackgroundImage, "m_tpBackgroundImage");
            this.m_tpBackgroundImage.Name = "m_tpBackgroundImage";
            this.m_tpBackgroundImage.UseVisualStyleBackColor = true;
            // 
            // m_tpBounds
            // 
            this.m_tpBounds.Controls.Add(this.m_panelHFBounds);
            resources.ApplyResources(this.m_tpBounds, "m_tpBounds");
            this.m_tpBounds.Name = "m_tpBounds";
            this.m_tpBounds.UseVisualStyleBackColor = true;
            // 
            // m_tpBorderStyle
            // 
            this.m_tpBorderStyle.Controls.Add(this.m_panelHFBorder);
            resources.ApplyResources(this.m_tpBorderStyle, "m_tpBorderStyle");
            this.m_tpBorderStyle.Name = "m_tpBorderStyle";
            this.m_tpBorderStyle.UseVisualStyleBackColor = true;
            // 
            // HeaderFooterDialog
            // 
            this.AcceptButton = this.m_btnHFOK;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.m_pictureHFReducedView);
            this.Controls.Add(this.m_btnHFCancel);
            this.Controls.Add(this.m_btnHFOK);
            this.Controls.Add(this.m_groupHFSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HeaderFooterDialog";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.HeaderFooterDialog_Load);
            this.m_panelHFBounds.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsLeftMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsRightMargin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_numHFBoundsHeight)).EndInit();
            this.m_panelHFBorder.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureHFBorderPreview)).EndInit();
            this.m_groupHFSelection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureHFReducedView)).EndInit();
            this.m_panelHFBackgroundImage.ResumeLayout(false);
            this.m_panelHFBackgroundImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureHFImagePreview)).EndInit();
            this.m_panelHFText.ResumeLayout(false);
            this.m_panelHFText.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.m_tpText.ResumeLayout(false);
            this.m_tpBackgroundImage.ResumeLayout(false);
            this.m_tpBounds.ResumeLayout(false);
            this.m_tpBorderStyle.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region EventHandlers
        private void HeaderFooterDialog_Load(object sender, EventArgs e)
        {
            FillPropertiesHash();
            FillCombos();
            m_radioHFHeader.Checked = true;
            m_bInitializing = false;

            // Header Preview routine
            float fHeight = GetHeight();
            m_fHeaderScaleFactor = CalculateScaleFactor(this.Header.Bounds.PageWidth, fHeight);

            // Convert to pixels
            RectangleF rectHF = new RectangleF(0, 0, this.Header.Bounds.PageWidth, fHeight);
            rectHF = ApplyScaleFactor(rectHF);
            rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
            rectHF.X += this.Header.Bounds.LeftMargin * m_fHeaderScaleFactor;
            rectHF.Width = this.Header.Bounds.Width * m_fHeaderScaleFactor;
            m_rectHeaderReduced = rectHF;

            // Footer Preview routine
            fHeight = GetHeight();
            m_fFooterScaleFactor = CalculateScaleFactor(this.Footer.Bounds.PageWidth, fHeight);

            // Convert to pixels
            rectHF = new RectangleF(0, 0, this.Footer.Bounds.PageWidth, fHeight);
            rectHF = ApplyScaleFactor(rectHF);
            rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
            rectHF.X += this.Footer.Bounds.LeftMargin * m_fFooterScaleFactor;
            rectHF.Width = this.Footer.Bounds.Width * m_fFooterScaleFactor;
            m_rectFooterReduced = rectHF;

            FillImageList(m_pictureHFReducedView.Size);
            MakeHFBackground(this.HF.BorderStyle.ShowBorder);
            MakeHFStrings();
            InvalidateReducedView();
        }        

        private void HFHeader_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            this.IsHeader = radio.Checked;
            HFRelatedActions();
            if (!m_bInitializing)
            {
                InvalidateReducedView();
                RefreshBorderPreview();
            }
        }

        /// <summary>
        /// Processes a dialog box key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>
        /// true if the keystroke was processed and consumed by the control; otherwise, false to allow further processing.
        /// </returns>
        [Documentation.DocumentationExclude()]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            bool bProcessed = true;
            if (keyData == Keys.Return)
            {
                if (this.ActiveControl == m_txtHFImagePath)
                {
                    if (m_bValidatedImagePath)
                        bProcessed = base.ProcessDialogKey(keyData);
                    else
                        ValidateImagePath();
                }
                else
                    bProcessed = base.ProcessDialogKey(keyData);
            }
            else
                bProcessed = base.ProcessDialogKey(keyData);
            return bProcessed;
        }

        #region Image Event Handlers
        private void HFImageChoose_Click(object sender, EventArgs e)
        {
            Stream streamImage;
            OpenFileDialog dlgImageOpen = new OpenFileDialog();
            dlgImageOpen.Title = "Select background image";
            dlgImageOpen.Filter = "All Image files (*.bmp,*.gif,*.jpg,*.jpeg,*.png,*.ico,*.emf,*.wmf)|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.ico;*.emf;*.wmf|"
                + "Bitmap files (*.bmp,*.gif,*.jpg,*.jpeg,*.png,*.ico)|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.ico|"
                + "Metafiles (*.emf,*.wmf)|*.emf;*.wmf";

            if (dlgImageOpen.ShowDialog() == DialogResult.OK)
            {
                streamImage = dlgImageOpen.OpenFile();
                if (streamImage != null)
                {
                    this.HF.ImagePath = dlgImageOpen.FileName;
                    m_txtHFImagePath.Text = dlgImageOpen.FileName;
                    this.HF.Image = Image.FromStream(streamImage);
                    DrawImageToPreviewBox(this.HF.Image);

                    float fHeight = GetHeight();
                    this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                    // Convert to pixels
                    RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
                    rectHF = ApplyScaleFactor(rectHF);
                    rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                    rectHF.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
                    rectHF.Width = this.HF.Bounds.Width * this.ScaleFactor;
                    this.ReducedHFRect = rectHF;

                    MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                }
            }
        }

        private void HFImagePath_TextChanged(object sender, EventArgs e)
        {
            if (m_bValidatedImagePath)
                m_bValidatedImagePath = false;
        }

        #endregion Image Event Handlers

        #region Text Event Handlers
        private void HFTextFontSelection_Click(object sender, EventArgs e)
        {
            HeaderFooterBase hfHFBase = HF;
            FontDialog dlgFont = new FontDialog();
            dlgFont.Font = hfHFBase.Font;
            if (dlgFont.ShowDialog() == DialogResult.OK)
            {
                m_btnHFTextFontSelection.Text = dlgFont.Font.Name;
                hfHFBase.Font = dlgFont.Font;
                m_lblHFTextSample.Font = dlgFont.Font;
                m_lblHFTextSample.Refresh();

                float fHeight = GetHeight();
                this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                // Convert to pixels
                RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
                rectHF = ApplyScaleFactor(rectHF);
                rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                rectHF.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
                rectHF.Width = this.HF.Bounds.Width * this.ScaleFactor;
                this.ReducedHFRect = rectHF;

                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                MakeHFStrings();
            }
        }

        private void HFTextColorSelection_Click(object sender, EventArgs e)
        {
            ColorDialog dlgFont = new ColorDialog();
            dlgFont.Color = this.HF.ForeColor;
            if (dlgFont.ShowDialog() == DialogResult.OK)
            {
                m_btnHFTextColorSelection.ForeColor = dlgFont.Color;
                m_btnHFOK.Focus();
                this.HF.ForeColor = dlgFont.Color;
                m_lblHFTextSample.ForeColor = dlgFont.Color;
                m_lblHFTextSample.Refresh();
                MakeHFStrings();
            }
        }

        private void HFTextLeft_Click(object sender, EventArgs e)
        {
            m_txtAddingText = m_txtHFTextLeft;
            ShowSelectionMenu();
            SetFocus();
        }

        private void HFTextCenter_Click(object sender, EventArgs e)
        {
            m_txtAddingText = m_txtHFTextCenter;
            ShowSelectionMenu();
            SetFocus();
        }

        private void HFTextRight_Click(object sender, EventArgs e)
        {
            m_txtAddingText = m_txtHFTextRight;
            ShowSelectionMenu();
            SetFocus();
        }

        private void MenuItemPage_Click(object sender, EventArgs e)
        {
            m_txtAddingText.Text += "&p";
            SetCaretPositionToEnd();
        }

        private void MenuTotalPages_Click(object sender, EventArgs e)
        {
            m_txtAddingText.Text += "&P";
            SetCaretPositionToEnd();
        }

        private void MenuCurrentTime_Click(object sender, EventArgs e)
        {
            m_txtAddingText.Text += "&T";
            SetCaretPositionToEnd();
        }

        private void MenuCurrentDateShort_Click(object sender, EventArgs e)
        {
            m_txtAddingText.Text += "&d";
            SetCaretPositionToEnd();
        }

        private void MenuCurrentDateLong_Click(object sender, EventArgs e)
        {
            m_txtAddingText.Text += "&D";
            SetCaretPositionToEnd();
        }

        #endregion Text Event Handlers

        #region Border Event Handlers
        private void HFBorderSelectColor_Click(object sender, EventArgs e)
        {
            ColorDialog dlgBorder = new ColorDialog();
            dlgBorder.Color = this.HF.BorderStyle.Color;
            if (dlgBorder.ShowDialog() == DialogResult.OK)
            {
                this.HF.BorderStyle.Color = dlgBorder.Color;
                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                RefreshBorderPreview();
            }
        }

        private void HFBorderVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                CheckBox check = sender as CheckBox;
                MakeHFBackground(check.Checked);
            }
        }

        #endregion Border Event Handlers

        private void HFTextVisible_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                InvalidateReducedView();
            }
        }

        private void HFBoundsAutosize_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                CheckBox check = sender as CheckBox;
                this.HF.AutoBounds = check.Checked;
                if (check.Checked)
                {
                    float fHeight = GetHeight();
                    this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                    // Convert to pixels
                    RectangleF rectHF = new RectangleF(0, 0, this.Header.Bounds.PageWidth, fHeight);
                    rectHF = ApplyScaleFactor(rectHF);
                    rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                    rectHF.X += this.Header.Bounds.LeftMargin * m_fHeaderScaleFactor;
                    rectHF.Width = this.Header.Bounds.Width * m_fHeaderScaleFactor;
                    this.ReducedHFRect = rectHF;
                }
                else
                {
                    float fValue;
                    if (!(this.MeasurementUnits == MeasureUnits.Inch))
                    {
                        fValue = MeasureUnitsConverter.Convert((float)m_numHFBoundsHeight.Value, this.MeasurementUnits, MeasureUnits.Inch) * 100f;
                        
                        // Measurements.Convert( this.MeasurementUnits, GraphicsUnit.Inch, ( float )m_numHFBoundsHeight.Value ) * 100;
                    }
                    else
                    {
                        fValue = (float)m_numHFBoundsHeight.Value * 100;
                    }

                    UpdateHFReducedRect(Bound.Height, fValue);
                }
                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                MakeHFStrings();
                InvalidateReducedView();
            }
        }
        private void HFBorderStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            this.HF.BorderStyle.Style = (DashStyle)combo.SelectedItem;

            if (!m_bInitializing)
            {
                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                RefreshBorderPreview();
            }
        }
        private void HFBorderWeight_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            this.HF.BorderStyle.Weight = (BorderWeight)combo.SelectedItem;

            if (!m_bInitializing)
            {
                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                RefreshBorderPreview();
            }
        }
        private void HFImageLayout_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.HF.Image != null)
            {
                ComboBox combo = sender as ComboBox;
                ImageLayout imgLayout = (ImageLayout)combo.SelectedItem;
                
                // binding workaround
                this.HF.ImageLayout = imgLayout;

                switch (imgLayout)
                {
                    case ImageLayout.Tile:
                        // this.ImageBrush.WrapMode = WrapMode.Tile;
                        break;
                    case ImageLayout.Center:
                        break;
                    case ImageLayout.AlignLeft:
                        break;
                    case ImageLayout.AlignRight:
                        break;
                }

                if (!m_bInitializing)
                    MakeHFBackground(this.HF.BorderStyle.ShowBorder);
            }
        }

        private void HFBoundsHeight_TextChanged(object sender, EventArgs e)
        {
            if (!this.HF.AutoBounds && !m_bInitializing)
            {
                NumericUpDown num = sender as NumericUpDown;
                float fValue;
                if (!(this.MeasurementUnits == MeasureUnits.Inch))
                {
                    fValue = MeasureUnitsConverter.Convert((float)num.Value, this.MeasurementUnits, MeasureUnits.Inch) * 100f;
                    
                    // Measurements.Convert( this.MeasurementUnits, GraphicsUnit.Inch, ( float )num.Value ) * 100;
                }
                else
                {
                    fValue = (float)num.Value * 100;
                    if (fValue == 0)
                        fValue = 0.01f;
                }
                if (fValue > 0)
                    UpdateHFReducedRect(Bound.Height, fValue);
            }
        }

        private void HFBoundsLeftMargin_TextChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                NumericUpDown num = sender as NumericUpDown;
                float fValue;
                if (!(this.MeasurementUnits == MeasureUnits.Inch))
                    fValue = MeasureUnitsConverter.Convert((float)num.Value, this.MeasurementUnits, MeasureUnits.Inch) * 100f;
                
                    // Measurements.Convert( this.MeasurementUnits, GraphicsUnit.Inch, ( float )num.Value ) * 100;
                else
                {
                    fValue = (float)num.Value * 100;
                }

                this.HF.Bounds.LeftMargin = fValue;
                UpdateHFReducedRect(Bound.Left, fValue);
            }
        }

        private void HFBoundsRightMargin_TextChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                NumericUpDown num = sender as NumericUpDown;
                float fValue;
                if (!(this.MeasurementUnits == MeasureUnits.Inch))
                {
                    fValue = MeasureUnitsConverter.Convert((float)num.Value, this.MeasurementUnits, MeasureUnits.Inch) * 100f;
                    
                    // Measurements.Convert( this.MeasurementUnits, GraphicsUnit.Inch, ( float )num.Value ) * 100;
                }
                else
                    fValue = (float)num.Value * 100;

                this.HF.Bounds.RightMargin = fValue;
                UpdateHFReducedRect(Bound.Right, fValue);
            }
        }

        private void HFBoundsWidth_TextChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                NumericUpDown num = sender as NumericUpDown;
                float fValue;

                if (!(this.MeasurementUnits == MeasureUnits.Inch))
                {
                    fValue = MeasureUnitsConverter.Convert((float)num.Value, this.MeasurementUnits, MeasureUnits.Inch) * 100f;
                    
                    // Measurements.Convert( this.MeasurementUnits, GraphicsUnit.Inch, ( float )num.Value ) * 100;
                }
                else
                    fValue = (float)num.Value * 100;

                if (fValue > this.HF.Bounds.PageWidth)
                    fValue = this.HF.Bounds.PageWidth;

                this.HF.Bounds.Width = fValue;
                UpdateHFReducedRect(Bound.Width, fValue);
            }
        }

        private void HFTextLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                TextBox txtBox = sender as TextBox;
                this.HF.Left = txtBox.Text;

                float fHeight = GetHeight();
                this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                // Convert to pixels
                RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
                rectHF = ApplyScaleFactor(rectHF);
                rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                rectHF.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
                rectHF.Width = this.HF.Bounds.Width * this.ScaleFactor;
                this.ReducedHFRect = rectHF;

                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                MakeHFStrings();
            }
        }

        private void HFTextCenter_TextChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                TextBox txtBox = sender as TextBox;
                this.HF.Center = txtBox.Text;

                float fHeight = GetHeight();
                this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                // Convert to pixels
                RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
                rectHF = ApplyScaleFactor(rectHF);
                rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                rectHF.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
                rectHF.Width = this.HF.Bounds.Width * this.ScaleFactor;
                this.ReducedHFRect = rectHF;

                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                MakeHFStrings();
            }
        }

        private void HFTextRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                TextBox txtBox = sender as TextBox;
                this.HF.Right = txtBox.Text;

                float fHeight = GetHeight();
                this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                // Convert to pixels
                RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
                rectHF = ApplyScaleFactor(rectHF);
                rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                rectHF.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
                rectHF.Width = this.HF.Bounds.Width * this.ScaleFactor;
                this.ReducedHFRect = rectHF;

                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                MakeHFStrings();
            }
        }

        private void HFTextCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_bInitializing)
            {
                ComboBox combo = sender as ComboBox;
                this.HF.Culture = (CultureInfo)combo.SelectedItem;

                float fHeight = GetHeight();
                this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                // Convert to pixels
                RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
                rectHF = ApplyScaleFactor(rectHF);
                rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                rectHF.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
                rectHF.Width = this.HF.Bounds.Width * this.ScaleFactor;
                this.ReducedHFRect = rectHF;

                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
                MakeHFStrings();
            }
        }

        #endregion EventHandlers

        #region Helper Methods
        private void UpdateImageScaleFactor(float fScaleFactor)
        {
            Matrix matrix = new Matrix();
            matrix.Scale(fScaleFactor, fScaleFactor);
            
            // this.ImageBrush.Transform = matrix;
        }

        private void SetCaretPositionToEnd()
        {
            m_txtAddingText.SelectionStart = m_txtAddingText.Text.Length;
        }

        private void SetFocus()
        {
            // Binding is done when TextBox loses its focus -- so we must set focus to m_txtAddingText 
            m_txtAddingText.Focus();
        }

        private void ShowSelectionMenu()
        {
            ContextMenu conMenu = new ContextMenu();
            conMenu.MergeMenu(conMenuHF);
            Point ptLocation = new Point();
            Point ptActiceCtrl = GetControlLocation(this.ActiveControl);
            ptLocation.X = ptActiceCtrl.X + this.ActiveControl.Width;
            ptLocation.Y = ptActiceCtrl.Y;
            conMenu.Show(this, ptLocation);
            conMenu.Dispose();
        }

        private Point GetControlLocation(Control ctrl)
        {
            Point ptToReturn = ctrl.Location;
            Control ctrlParent = ctrl.Parent;
            while (!(ctrlParent is Form))
            {
                ptToReturn.X += ctrlParent.Location.X;
                ptToReturn.Y += ctrlParent.Location.Y;
                ctrlParent = ctrlParent.Parent;
            }
            return ptToReturn;
        }        

        private void FillPropertiesHash()
        {
            m_hashPanels = new Hashtable();
            m_hashPanels.Add(HFProperties.BackgroundImage, m_panelHFBackgroundImage);
            m_hashPanels.Add(HFProperties.BorderStyle, m_panelHFBorder);
            m_hashPanels.Add(HFProperties.Bounds, m_panelHFBounds);
            m_hashPanels.Add(HFProperties.Text, m_panelHFText);
        }

        private void HFRelatedActions()
        {
            DoHFRelatedBinding();
            DoFormattingRelatedBinding();
            
            // image
            DoImageRelatedBinding();
            DrawImageToPreviewBox(this.HF.Image);
            m_txtHFImagePath.Text = this.HF.ImagePath;
            
            // bounds
            DoBoundsRelatedBinding();
            
            // border
            DoBorderRelatedBinding();
        }

        private void DoHFRelatedBinding()
        {
            BindTo(m_txtHFTextLeft, c_strTEXT_PROPERTY_NAME, HF, "Left");
            BindTo(m_txtHFTextCenter, c_strTEXT_PROPERTY_NAME, HF, "Center");
            BindTo(m_txtHFTextRight, c_strTEXT_PROPERTY_NAME, HF, "Right");
            BindTo(m_checkHFTextVisible, "Checked", HF, "Visible");
        }

        private void DoFormattingRelatedBinding()
        {
            BindTo(m_comboHFTextCulture, c_strSELECTED_ITEM_PROPERTY_NAME, HF, "Culture");
            m_lblHFTextSample.ForeColor = this.HF.ForeColor;
            m_lblHFTextSample.Font = this.HF.Font;
        }

        private void DoImageRelatedBinding()
        {
            BindTo(m_comboHFImageLayout, c_strSELECTED_ITEM_PROPERTY_NAME, HF, "ImageLayout");
        }

        private void DoBoundsRelatedBinding()
        {
            BindTo(m_numHFBoundsLeftMargin, "Text", BindHelper, "LeftMargin");
            BindTo(m_numHFBoundsRightMargin, "Text", BindHelper, "RightMargin");
            BindTo(m_numHFBoundsWidth, "Text", BindHelper, "Width");
            BindTo(m_numHFBoundsHeight, "Text", BindHelper, "Height");
            BindTo(m_checkHFBoundsAutosize, "Checked", HF, "AutoBounds");
        }

        private void DoBorderRelatedBinding()
        {
            BindTo(m_comboHFBorderStyle, c_strSELECTED_ITEM_PROPERTY_NAME, BindHelper, "Style");
            BindTo(m_comboHFBorderWeight, c_strSELECTED_ITEM_PROPERTY_NAME, BindHelper, "Weight");
            BindTo(m_checkHFBorderVisible, "Checked", BindHelper, "ShowBorder");
        }

        private void FillCombos()
        {
            FillCultureCombo();
            FillImageLayoutCombo();
            FillBorderStyleCombo();
            FillBorderWeightCombo();
        }

        private void FillBorderWeightCombo()
        {
            Array arr = Enum.GetValues(typeof(BorderWeight));
            foreach (object obj in arr)
            {
                m_comboHFBorderWeight.Items.Add(obj);
            }
            m_comboHFBorderWeight.SelectedItem = BorderWeight.Medium;
        }

        private void FillBorderStyleCombo()
        {
            Array arr = Enum.GetValues(typeof(DashStyle));
            foreach (object obj in arr)
            {
                m_comboHFBorderStyle.Items.Add(obj);
            }
            m_comboHFBorderStyle.SelectedItem = DashStyle.Solid;
        }

        private void FillImageLayoutCombo()
        {
            Array arr = Enum.GetValues(typeof(ImageLayout));
            foreach (object obj in arr)
            {
                m_comboHFImageLayout.Items.Add(obj);
            }
            m_comboHFImageLayout.SelectedIndex = 0;
        }

        private void FillCultureCombo()
        {
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures);
            foreach (CultureInfo culture in cultures)
            {
                // Cultureinfo.ToString() returns CultureInfo.Name -- we need CultureInfo.DisplayName
                CultureInfoModified cultureModified = new CultureInfoModified(culture.Name);
                m_comboHFTextCulture.Items.Add(cultureModified);
            }
            m_comboHFTextCulture.SelectedItem = CultureInfo.CurrentCulture;
        }

        private void BindTo(Control ctrlToBindTo, string strControlPropertyName, object objDataSource, string strDataSourcePropertyName)
        {
            if (ctrlToBindTo.DataBindings[strControlPropertyName] != null)
                ctrlToBindTo.DataBindings.Remove(ctrlToBindTo.DataBindings[strControlPropertyName]);
            ctrlToBindTo.DataBindings.Add(strControlPropertyName, objDataSource, strDataSourcePropertyName);
        }

        private Graphics CreateGraphicsToDraw(PictureBox pictureBox)
        {
            Image img = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = img;
            return Graphics.FromImage(img);
        }

        private void ValidateImagePath()
        {
            string strImagePath = m_txtHFImagePath.Text;
            if (File.Exists(strImagePath))
            {
                this.HF.Image = Image.FromFile(strImagePath);
                this.HF.ImagePath = strImagePath;
                DrawImageToPreviewBox(this.HF.Image);
                if (this.HF.AutoBounds)
                {
                    float fHeight = GetHeight();
                    this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

                    // Convert to pixels
                    RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
                    rectHF = ApplyScaleFactor(rectHF);
                    rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                    rectHF.X += this.HF.Bounds.LeftMargin * m_fHeaderScaleFactor;
                    rectHF.Width = this.HF.Bounds.Width * m_fHeaderScaleFactor;
                    this.ReducedHFRect = rectHF;
                }
                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
            }
            else
            {
                this.HF.Image = null;
                this.HF.ImagePath = strImagePath;
                DrawImageToPreviewBox(this.HF.Image);
                MakeHFBackground(this.HF.BorderStyle.ShowBorder);
            }
            m_bValidatedImagePath = true;
        }

        #region Image Preview
        private void DrawImageToPreviewBox(Image imgToDraw)
        {
            Graphics gph = CreateGraphicsToDraw(m_pictureHFImagePreview);
            if (imgToDraw != null)
            {
                RectangleF rectDestination = CalculateDestinationRect(imgToDraw, m_pictureHFImagePreview.Bounds);
                gph.DrawImage(imgToDraw, rectDestination, new RectangleF(new PointF(0, 0), imgToDraw.Size), GraphicsUnit.Pixel);
            }
            else
            {
                gph.Clear(SystemColors.Control);
            }
        }
        private RectangleF CalculateDestinationRect(Image img, System.Drawing.Rectangle pictureBoxBounds)
        {
            RectangleF rectToReturn = new RectangleF(new PointF(0, 0), new SizeF(0, 0));

            // X location and Width 
            if (img.Width >= pictureBoxBounds.Width)
            {
                rectToReturn.Width = pictureBoxBounds.Width;
            }
            else
            {
                rectToReturn.X = pictureBoxBounds.Width / 2 - img.Width / 2;
                rectToReturn.Width = img.Width;
            }

            // Y lacstion and Height
            if (img.Height >= pictureBoxBounds.Height)
            {
                rectToReturn.Height = pictureBoxBounds.Height;
            }
            else
            {
                rectToReturn.Y = pictureBoxBounds.Height / 2 - img.Height / 2;
                rectToReturn.Height = img.Height;
            }

            return rectToReturn;
        }
        #endregion Image Preview

        #region Reduced Preview
        private void FillImageList(Size szImg)
        {
            m_hashImgLayers = new Hashtable();

            Image imgBackGround = new Bitmap(szImg.Width, szImg.Height);
            Graphics gph = Graphics.FromImage(imgBackGround);
            gph.FillRectangle(new SolidBrush(Color.FromKnownColor(KnownColor.ControlLightLight)), new System.Drawing.Rectangle(new Point(0, 0), szImg));
            m_hashImgLayers.Add("Background", imgBackGround);

            Image imgHeaderBackground = new Bitmap(szImg.Width, szImg.Height);
            m_hashImgLayers.Add("HeaderBackground", imgHeaderBackground);

            Image imgHeaderStrings = new Bitmap(szImg.Width, szImg.Height);
            m_hashImgLayers.Add("HeaderStrings", imgHeaderStrings);

            Image imgFooterBackground = new Bitmap(szImg.Width, szImg.Height);
            m_hashImgLayers.Add("FooterBackground", imgFooterBackground);

            Image imgFooterStrings = new Bitmap(szImg.Width, szImg.Height);
            m_hashImgLayers.Add("FooterStrings", imgFooterStrings);
        }

        private void InvalidateReducedView()
        {
            Graphics gph = CreateGraphicsToDraw(m_pictureHFReducedView);

            DrawBackground(gph);
            if (m_checkHFTextVisible.Checked)
            {
                DrawHFBackground(gph);
                DrawHFStrings(gph);
            }
        }

        private void DrawBackground(Graphics gph)
        {
            Image imgBackground = (Image)m_hashImgLayers["Background"];
            gph.DrawImage(imgBackground, 0, 0);
        }

        private void DrawHFBackground(Graphics gph)
        {
            Image imgHFBackground;
            if (this.IsHeader)
                imgHFBackground = (Image)m_hashImgLayers["HeaderBackground"];
            else
                imgHFBackground = (Image)m_hashImgLayers["FooterBackground"];

            gph.DrawImage(imgHFBackground, 0, 0);
        }

        private void DrawHFStrings(Graphics gph)
        {
            Image imgHFStrings;
            if (this.IsHeader)
                imgHFStrings = (Image)m_hashImgLayers["HeaderStrings"];
            else
                imgHFStrings = (Image)m_hashImgLayers["FooterStrings"];

            gph.DrawImage(imgHFStrings, 0, 0);
        }

        private void MakeHFStrings()
        {
            Image imgHFStrings = GetHFStringsLayer();
            Graphics gph = Graphics.FromImage(imgHFStrings);
            gph.Clear(Color.Transparent);

            DrawStrings(gph);

            InvalidateReducedView();
        }

        private Image GetHFStringsLayer()
        {
            Image imgToReturn;

            if (this.IsHeader)
                imgToReturn = (Image)m_hashImgLayers["HeaderStrings"];
            else
                imgToReturn = (Image)m_hashImgLayers["FooterStrings"];

            return imgToReturn;
        }

        private void MakeHFBackground(bool bShowBorder)
        {
            Image imgHFBackground = GetHFBackgroundLayer();
            Graphics gph = Graphics.FromImage(imgHFBackground);
            gph.Clear(Color.Transparent);

            if (this.HF.Image != null)
                DrawImage(gph);

            if (bShowBorder)
                DrawBorder(gph);

            InvalidateReducedView();
        }

        private void DrawImage(Graphics gph)
        {
            ImageAttributes attr = new ImageAttributes();

            Graphics g;

            switch (this.HF.ImageLayout)
            {
                case ImageLayout.Stretch:
                    Image img = this.HF.Image;
                    if ((this.ReducedHFRect.Width > 0.5f) && (this.ReducedHFRect.Height > 0.5f))
                        gph.DrawImage(img, this.ReducedHFRect, new RectangleF(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
                    break;
                case ImageLayout.Tile:
                    // first -- convert to Hundreds of inches
                    int nWidthImage = (int)Math.Round((this.ReducedHFRect.Width / this.ScaleFactor));
                    int nHeightImage = (int)Math.Round((this.ReducedHFRect.Height / this.ScaleFactor));

                    if (nHeightImage < 1)
                        nHeightImage = 1;
                    if (nWidthImage < 1)
                        nWidthImage = 1;

                    Image image = new Bitmap(nWidthImage, nHeightImage);
                    g = Graphics.FromImage(image);
                    attr.SetWrapMode(WrapMode.Tile);
                    
                    // Location of three edges of rectangle for drawing.
                    PointF p1 = new PointF(0, 0);
                    PointF p2 = new PointF(image.Width, 0);
                    PointF p3 = new PointF(p1.X, image.Height);
                    PointF[] destPoints = { p1, p2, p3 };
                    RectangleF srcRect = new RectangleF(0, 0, image.Width, image.Height);
                    g.DrawImage(this.HF.Image, destPoints, srcRect, GraphicsUnit.Pixel, attr);
                    gph.DrawImage(image, this.ReducedHFRect);
                    break;
                case ImageLayout.AlignLeft:
                    AlignAndDrawImage(gph, ImageLayout.AlignLeft);
                    break;
                case ImageLayout.AlignRight:
                    AlignAndDrawImage(gph, ImageLayout.AlignRight);
                    break;
                case ImageLayout.Center:
                    AlignAndDrawImage(gph, ImageLayout.Center);
                    break;
            }
        }

        private void AlignAndDrawImage(Graphics gph, ImageLayout imgLayout)
        {
            Graphics g;
            int nWidth;
            int nHeight;

            nWidth = (int)Math.Round(this.ReducedHFRect.Width / this.ScaleFactor);

            if (this.HF.Image.Width < nWidth)
                nWidth = this.HF.Image.Width;

            if (nWidth < 1)
                nWidth = 1;
            nHeight = (int)Math.Round(this.ReducedHFRect.Height / this.ScaleFactor);

            if (this.HF.Image.Height < nHeight)
                nHeight = this.HF.Image.Height;

            if (nHeight < 1)
                nHeight = 1;

            Image imageLeft = new Bitmap(nWidth, nHeight);
            g = Graphics.FromImage(imageLeft);

            // int nHheight = ( int )Math.Round( this.ReducedHFRect.Height / this.ScaleFactor );
            RectangleF srcRectLeft = RectangleF.Empty;
            switch (imgLayout)
            {
                case ImageLayout.AlignLeft:
                    srcRectLeft = new RectangleF(0, (this.HF.Image.Height / 2) - (nHeight / 2), nWidth, nHeight);
                    break;
                case ImageLayout.AlignRight:
                    srcRectLeft = new RectangleF(this.HF.Image.Width - nWidth, (this.HF.Image.Height / 2) - (nHeight / 2), nWidth, nHeight);
                    break;
                case ImageLayout.Center:
                    srcRectLeft = new RectangleF((this.HF.Image.Width / 2) - (nWidth / 2), (this.HF.Image.Height / 2) - (nHeight / 2), nWidth, nHeight);
                    break;
            }

            g.DrawImage(this.HF.Image, new RectangleF(new PointF(0, 0), new SizeF(nWidth, nHeight)), srcRectLeft, GraphicsUnit.Pixel);

            RectangleF destRect = RectangleF.Empty;
            float fSmallWidth;
            float fSmallHeight;

            switch (imgLayout)
            {
                case ImageLayout.AlignLeft:
                    fSmallWidth = srcRectLeft.Width * this.ScaleFactor;
                    fSmallHeight = srcRectLeft.Height * this.ScaleFactor;
                    destRect = new RectangleF(this.ReducedHFRect.X, this.ReducedHFRect.Y + (this.ReducedHFRect.Height / 2) - (fSmallHeight / 2), fSmallWidth, fSmallHeight);
                    break;
                case ImageLayout.AlignRight:
                    fSmallWidth = srcRectLeft.Width * this.ScaleFactor;
                    fSmallHeight = srcRectLeft.Height * this.ScaleFactor;
                    destRect = new RectangleF((this.ReducedHFRect.X + this.ReducedHFRect.Width) - (srcRectLeft.Width * this.ScaleFactor), this.ReducedHFRect.Y + (this.ReducedHFRect.Height / 2) - (fSmallHeight / 2), fSmallWidth, fSmallHeight);
                    break;
                case ImageLayout.Center:
                    fSmallWidth = srcRectLeft.Width * this.ScaleFactor;
                    fSmallHeight = srcRectLeft.Height * this.ScaleFactor;
                    destRect = new RectangleF((this.ReducedHFRect.X + (this.ReducedHFRect.Width / 2)) - (fSmallWidth / 2), this.ReducedHFRect.Y + (this.ReducedHFRect.Height / 2) - (fSmallHeight / 2), fSmallWidth, fSmallHeight);
                    break;
            }

            gph.DrawImage(imageLeft, destRect);
        }

        private void DrawBorder(Graphics gph)
        {
            RectangleF rectBorder;

            if (this.IsHeader)
                rectBorder = m_rectHeaderReduced;
            else
                rectBorder = m_rectFooterReduced;

            Pen pen = new Pen(this.HF.BorderStyle.Color, this.HF.BorderStyle.Width * this.ScaleFactor);
            pen.DashStyle = this.HF.BorderStyle.Style;

            if (rectBorder.Height == 0)
                rectBorder.Height = 1;

            gph.DrawRectangle(pen, rectBorder.X, rectBorder.Y, rectBorder.Width, rectBorder.Height);
        }

        private void DrawStrings(Graphics gph)
        {
            StringFormat drawFormat = new StringFormat();
            float height = this.ReducedHFRect.Height;
            float width = this.ReducedHFRect.Width;
            float x = this.ReducedHFRect.X;
            float y = this.ReducedHFRect.Y;

            if (height != 0 && width != 0)
            {
                if (this.HF.AutoBounds)
                {
                    drawFormat.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit;
                }
                else
                {
                    drawFormat.FormatFlags = StringFormatFlags.LineLimit;
                }
                SolidBrush brush = new SolidBrush(this.HF.ForeColor);
                Font font = new Font(this.HF.Font.FontFamily, this.HF.Font.SizeInPoints * this.ScaleFactor, this.HF.Font.Style);

                string headerRight = this.HF.ComposedRight;
                string headerLeft = this.HF.ComposedLeft;
                string headerCenter = this.HF.ComposedCenter;

                // Checking for empty string after trim operation
                if (headerCenter == null)
                {
                    if (headerLeft == null)
                    {
                        if (headerRight != null)
                        {
                            // if there is only right header draw it using full page width
                            DrawHFRight(gph, headerRight, font, brush, x, y, width, width, height, drawFormat);
                        }
                    }
                    else if (headerRight != null)
                    {
                        // if there is no center header but left and right make left and right headers width halh page width
                        DrawHFRight(gph, headerRight, font, brush, x, y, width, width / 2, height, drawFormat);
                        DrawHFLeft(gph, headerLeft, font, brush, x, y, width / 2, height, drawFormat);
                    }
                    else
                    {
                        DrawHFLeft(gph, headerLeft, font, brush, x, y, width, height, drawFormat);
                    }
                }
                else
                {
                    // center header is present
                    // if there are no left and right headers draw center header using full page width
                    if ((headerLeft == null) && (headerRight == null))
                    {
                        DrawHFCenter(gph, headerCenter, font, brush, x, y, width, width, height, drawFormat);
                    }
                    else
                    {
                        // if there is left or right header or both set each header width equal
                        DrawHFLeft(gph, headerLeft, font, brush, x, y, width / 3, height, drawFormat);
                        DrawHFRight(gph, headerRight, font, brush, x, y, width, width / 3, height, drawFormat);
                        DrawHFCenter(gph, headerCenter, font, brush, x, y, width, width / 3, height, drawFormat);
                    }
                }
            }
        }

        private void DrawHFRight(Graphics gph, string str, Font font, SolidBrush brush, float x, float y, float pageWidth, float headerWidth, float highestHeader, StringFormat drawFormat)
        {
            if (headerWidth >= 1)
            {
                SizeF size = gph.MeasureString(str, font, (int)headerWidth);
                RectangleF rect = new RectangleF((x + (pageWidth - size.Width)), y, size.Width, highestHeader);
                gph.DrawString(str, font, brush, rect, drawFormat);
            }
        }

        private void DrawHFLeft(Graphics gph, string str, Font font, SolidBrush brush, float x, float y, float headerWidth, float highestHeader, StringFormat drawFormat)
        {
            if (headerWidth >= 1)
            {
                RectangleF rect = new RectangleF(x, y, headerWidth, highestHeader);
                gph.DrawString(str, font, brush, rect, drawFormat);
            }
        }

        private void DrawHFCenter(Graphics gph, string str, Font font, SolidBrush brush, float x, float y, float pageWidth, float headerWidth, float highestHeader, StringFormat drawFormat)
        {
            if (headerWidth >= 1)
            {
                SizeF size = gph.MeasureString(str, font, (int)headerWidth);
                RectangleF rect = new RectangleF((x + (pageWidth / 2 - size.Width / 2)), y, size.Width, highestHeader);
                gph.DrawString(str, font, brush, rect, drawFormat);
            }
        }

        private Image GetHFBackgroundLayer()
        {
            Image imgToReturn;

            if (this.IsHeader)
                imgToReturn = (Image)m_hashImgLayers["HeaderBackground"];
            else
                imgToReturn = (Image)m_hashImgLayers["FooterBackground"];

            return imgToReturn;
        }

        private float CalculateScaleFactor(float fWidth, float fHeight)
        {
            float fValueToReturn;
            fValueToReturn = Math.Min((m_pictureHFReducedView.Width - 3) / fWidth, (m_pictureHFReducedView.Height - 3) / fHeight);

            return fValueToReturn;
        }

        private float GetHeight()
        {
            float fValueToReturn;
            HeaderFooterBase hfBase = this.HF;

            if (hfBase.AutoBounds)
            {
                fValueToReturn = CalculateHeight(hfBase);
            }
            else
            {
                fValueToReturn = hfBase.Bounds.Height;
            }

            return fValueToReturn;
        }

        private float CalculateHeight(HeaderFooterBase hfBase)
        {
            float fHighest = 0;

            float fRealWidth;

            // if ( this.ScaleFactor == 0 )
            //   fRealWidth = this.HF.Bounds.PageWidth - this.HF.Bounds.LeftMargin - this.HF.Bounds.RightMargin;
            fRealWidth = this.HF.Bounds.Width;
            
            // else
            //   fRealWidth = this.ReducedHFRect.Width / this.ScaleFactor;
            Font font = hfBase.Font;

            string headerRight = hfBase.ComposedRight;
            string headerLeft = hfBase.ComposedLeft;
            string headerCenter = hfBase.ComposedCenter;

            Graphics g = Graphics.FromHwnd(IntPtr.Zero);

            // Checking for empty string after trim operation
            if (headerCenter == null)
            {
                if (headerLeft == null)
                {
                    if (headerRight == null)
                    {
                        // if there is no text header height will be as image height
                        if (hfBase.Image != null)
                            return hfBase.Image.Height;
                        else
                            return 0;
                    }
                    else
                    {
                        // if there is only right header draw it using full page width
                        MeasureString(g, headerRight, font, fRealWidth, ref fHighest);
                    }
                }
                else if (headerRight != null)
                {
                    MeasureString(g, headerRight, font, fRealWidth / 2, ref fHighest);
                    MeasureString(g, headerLeft, font, fRealWidth / 2, ref fHighest);
                }
                else
                {
                    MeasureString(g, headerLeft, font, fRealWidth, ref fHighest);
                }
            }
            else
            {
                // if there are no left and right headers draw center header using full page width
                if ((headerLeft == null) && (headerRight == null))
                {
                    MeasureString(g, headerCenter, font, fRealWidth, ref fHighest);
                }
                else
                {
                    MeasureString(g, headerLeft, font, fRealWidth / 3, ref fHighest);
                    MeasureString(g, headerRight, font, fRealWidth / 3, ref fHighest);
                    MeasureString(g, headerCenter, font, fRealWidth / 3, ref fHighest);
                }
            }

            return fHighest;
        }

        private void MeasureString(Graphics g, string str, Font font, float width, ref float highest)
        {
            SizeF size = g.MeasureString(str, font, (int)width);
            if (size.Height > highest)
                highest = size.Height;
        }

        private RectangleF ApplyScaleFactor(RectangleF rectHF)
        {
            RectangleF rectToReturn = new RectangleF(0, 0, rectHF.Width * this.ScaleFactor, rectHF.Height * this.ScaleFactor);
            return rectToReturn;
        }

        private PointF AlignMiddleCenter(SizeF szRect, SizeF szRelative)
        {
            PointF ptToReturn = new PointF();

            ptToReturn.X = (szRelative.Width / 2) - (szRect.Width / 2);
            ptToReturn.Y = (szRelative.Height / 2) - (szRect.Height / 2);
            this.HFX = ptToReturn.X;

            return ptToReturn;
        }

        private void UpdateHFReducedRect(Bound bound, float fValue)
        {
            RectangleF rectAffecting;

            if (this.IsHeader)
                rectAffecting = m_rectHeaderReduced;
            else
                rectAffecting = m_rectFooterReduced;

            switch (bound)
            {
                case Bound.Left:
                    rectAffecting = UpdateRect();
                    break;
                case Bound.Right:
                    rectAffecting = UpdateRect();
                    break;
                case Bound.Width:
                    rectAffecting = UpdateRect();
                    break;
                case Bound.Height:
                    this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fValue);
                    
                    // Convert to pixels
                    RectangleF rectHF = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fValue);
                    rectHF = ApplyScaleFactor(rectHF);
                    rectHF.Location = AlignMiddleCenter(rectHF.Size, m_pictureHFReducedView.Size);
                    rectHF.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
                    rectHF.Width = this.HF.Bounds.Width * this.ScaleFactor;
                    rectAffecting = rectHF;
                    break;
            }

            if (this.IsHeader)
                m_rectHeaderReduced = rectAffecting;
            else
                m_rectFooterReduced = rectAffecting;

            MakeHFBackground(this.HF.BorderStyle.ShowBorder);
            MakeHFStrings();
            if (this.HF.Visible)
                InvalidateReducedView();
        }

        private RectangleF UpdateRect()
        {
            RectangleF rectHFHeight;
            float fHeight;
            RectangleF rectAffecting;
            fHeight = GetHeight();
            this.ScaleFactor = CalculateScaleFactor(this.HF.Bounds.PageWidth, fHeight);

            // Convert to pixels
            rectHFHeight = new RectangleF(0, 0, this.HF.Bounds.PageWidth, fHeight);
            rectHFHeight = ApplyScaleFactor(rectHFHeight);
            rectHFHeight.Location = AlignMiddleCenter(rectHFHeight.Size, m_pictureHFReducedView.Size);
            rectHFHeight.X += this.HF.Bounds.LeftMargin * this.ScaleFactor;
            rectHFHeight.Width = this.HF.Bounds.Width * this.ScaleFactor;
            rectAffecting = rectHFHeight;
            return rectAffecting;
        }

        #endregion Reduced Preview

        #region Border Preview
        private void RefreshBorderPreview()
        {
            if (!m_bInitializing)
            {
                Graphics gph = CreateGraphicsToDraw(m_pictureHFBorderPreview);

                Pen pen = new Pen(this.HF.BorderStyle.Color, this.HF.BorderStyle.Width);
                pen.DashStyle = this.HF.BorderStyle.Style;

                PointF ptStart = new PointF(m_pictureHFBorderPreview.Width / 2, 0);
                PointF ptEnd = new PointF(m_pictureHFBorderPreview.Width / 2, m_pictureHFBorderPreview.Height);

                gph.DrawLine(pen, ptStart, ptEnd);
            }
        }

        #endregion Border Preview

        #endregion Helper Methods
    }

    [Documentation.DocumentationExclude()]
    internal class CultureInfoModified : CultureInfo
    {
        public CultureInfoModified(string strCultureName)
            : base(strCultureName)
        {
        }

        public override string ToString()
        {
            return base.DisplayName.ToString();
        }
    }

    [Documentation.DocumentationExclude()]
    internal class DeepBindingHelper
    {
        #region Fields
        private Header m_header;
        private Footer m_footer;
        private bool m_bIsHeader;
        private MeasureUnits m_units;
        
        // border
        private float m_fLeftMargin;
        private float m_fRightMargin;
        private float m_fWidth;
        private float m_fHeight;
        #endregion Fields

        #region Constructor
        public DeepBindingHelper(Header header, Footer footer, bool bIsHeader)
        {
            m_header = header;
            m_footer = footer;
            m_bIsHeader = bIsHeader;
        }
        #endregion Constructor

        #region Properties
        public bool IsHeader
        {
            get
            {
                return m_bIsHeader;
            }
            set
            {
                m_bIsHeader = value;
            }
        }
        private HeaderFooterBase HF
        {
            get
            {
                object objHF;
                if (IsHeader)
                    objHF = m_header;
                else
                    objHF = m_footer;
                return (HeaderFooterBase)objHF;
            }
        }
        #region Bounds
        public float LeftMargin
        {
            get
            {
                m_fLeftMargin = Convert(MeasureUnits.Inch, m_units, this.HF.Bounds.LeftMargin) / 100;
                return m_fLeftMargin;
            }
            set
            {
                if (m_fLeftMargin != value)
                {
                    m_fLeftMargin = value;
                    this.HF.Bounds.LeftMargin = Convert(m_units, MeasureUnits.Inch, value) * 100;
                }
            }
        }
        public float RightMargin
        {
            get
            {
                m_fRightMargin = Convert(MeasureUnits.Inch, m_units, this.HF.Bounds.RightMargin) / 100;
                return m_fRightMargin;
            }
            set
            {
                if (m_fRightMargin != value)
                {
                    m_fRightMargin = value;
                    this.HF.Bounds.RightMargin = Convert(m_units, MeasureUnits.Inch, value) * 100;
                }
            }
        }
        public float Width
        {
            get
            {
                m_fWidth = Convert(MeasureUnits.Inch, m_units, this.HF.Bounds.Width) / 100;
                return m_fWidth;
            }
            set
            {
                if (m_fWidth != value)
                {
                    m_fWidth = value;
                    this.HF.Bounds.Width = Convert(m_units, MeasureUnits.Inch, value) * 100;
                }
            }
        }
        public float Height
        {
            get
            {
                m_fHeight = Convert(MeasureUnits.Inch, m_units, this.HF.Bounds.Height) / 100;
                return m_fHeight;
            }
            set
            {
                if (m_fHeight != value)
                {
                    m_fHeight = value;
                    this.HF.Bounds.Height = Convert(m_units, MeasureUnits.Inch, value) * 100;
                }
            }
        }

        #endregion Bounds

        #endregion Properties

        #region Border
        public DashStyle Style
        {
            get
            {
                return HF.BorderStyle.Style;
            }
            set
            {
                if (HF.BorderStyle.Style != value)
                    HF.BorderStyle.Style = value;
            }
        }

        public Color Color
        {
            get
            {
                return this.HF.BorderStyle.Color;
            }
            set
            {
                if (this.HF.BorderStyle.Color != value)
                    this.HF.BorderStyle.Color = value;
            }
        }

        public BorderWeight Weight
        {
            get
            {
                return this.HF.BorderStyle.Weight;
            }
            set
            {
                if (this.HF.BorderStyle.Weight != value)
                    this.HF.BorderStyle.Weight = value;
            }
        }
        public bool ShowBorder
        {
            get
            {
                return this.HF.BorderStyle.ShowBorder;
            }
            set
            {
                this.HF.BorderStyle.ShowBorder = value;
            }
        }
        #endregion Border

        #region Helper Methods
        public void InitBounds(MeasureUnits units)
        {
            m_units = units;
            m_fLeftMargin = Convert(MeasureUnits.Inch, units, this.HF.Bounds.LeftMargin) / 100;
            m_fRightMargin = Convert(MeasureUnits.Inch, units, this.HF.Bounds.RightMargin) / 100;
            m_fWidth = Convert(MeasureUnits.Inch, units, this.HF.Bounds.Width) / 100;
            m_fHeight = Convert(MeasureUnits.Inch, units, this.HF.Bounds.Height) / 100;
        }

        private float Convert(MeasureUnits fromUnits, MeasureUnits toUnits, float fValue)
        {
            return MeasureUnitsConverter.Convert(fValue, fromUnits, toUnits);
        }
        #endregion Helper Methods
    }

    /// <summary>
    /// Header footer properties
    /// </summary>
    [Documentation.DocumentationExclude()]
    internal enum HFProperties
    {
        /// <summary>
        /// HF Text
        /// </summary>
        Text,

        /// <summary>
        /// Background image.
        /// </summary>
        BackgroundImage,

        /// <summary>
        /// HF bounds
        /// </summary>
        Bounds,

        /// <summary>
        /// Border style
        /// </summary>
        BorderStyle
    }

    /// <summary>
    /// HF Bounds
    /// </summary>
    [Documentation.DocumentationExclude()]
    internal enum Bound
    {
        /// <summary>
        /// Left position.
        /// </summary>
        Left,

        /// <summary>
        /// Right position.
        /// </summary>
        Right,

        /// <summary>
        /// Width of the bounds.
        /// </summary>
        Width,

        /// <summary>
        /// Height of the bounds.
        /// </summary>
        Height
    }
}
