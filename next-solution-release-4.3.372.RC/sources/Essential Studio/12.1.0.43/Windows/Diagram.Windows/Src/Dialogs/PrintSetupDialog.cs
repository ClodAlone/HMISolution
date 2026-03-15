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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Syncfusion.Runtime.InteropServices.WinAPI;
using Syncfusion.Windows.Forms.Diagram;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// The PrintSetupDialog provides a dialog for diagram users to set the page settings, margins and zoom ratios for 
    /// print and print preview. Initializing the dialog's <see cref="Syncfusion.Windows.Forms.Diagram.PrintSetupDialog.PageSettings"/> 
    /// and <see cref="Syncfusion.Windows.Forms.Diagram.PrintSetupDialog.PrintZoom"/> properties with their equivalent members from 
    /// the diagram's <see cref="Syncfusion.Windows.Forms.Diagram.View"/> will configure the dialog for the current print settings, and 
    /// enables users to modify the data.
    /// <p>
    /// Please refer to the DiagramBuilder sample to see the PrintSetupDialog in use.
    /// </p>
    /// </summary>
    public class PrintSetupDialog : Form
    {
        #region Fields

        #region Constants and Read-only
        private const int c_nONE = 1;
        private const int c_nSHADOW = 10;
        private const int c_nA4_FORMAT_WIDTH = 827;
        private const int c_nA4_FORMAT_HEIGHT = 1169;
        private const int c_nMAX_INTEGRAL_PART_DIGITS = 4;
        private const int c_nMAX_FRACTIONAL_PART_DIGITS = 4;
        private const int c_nMAX_TWO_DIGITS = 2;
        private const int c_nTEN = 10;
        private const int c_nONE_HUNDRED = 100;
        private readonly char c_charCOMMA = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator[0];
        private readonly string c_strCOMMA = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
        #endregion Constants

        private System.Windows.Forms.Button m_btnPrintSetupOK;
        private System.Windows.Forms.Button m_btnPrintSetupCancel;
        private System.Windows.Forms.GroupBox m_groupPrintSetupPrintZoom;
        private System.Windows.Forms.RadioButton m_radioPrintSetupPrintZoomFitTo;
        private System.Windows.Forms.RadioButton m_radioPrintSetupPrintZoomAdjustTo;
        private System.Windows.Forms.TextBox m_txtPrintSetupPrintZoomAdjustTo;
        private System.Windows.Forms.TextBox m_txtPrintSetupPrintZoomFitToAcross;
        private System.Windows.Forms.TextBox m_txtPrintSetupPrintZoomFitToBy;
        private System.Windows.Forms.Label m_lblPrintSetupPrintZoomAdjustTo;
        private System.Windows.Forms.Label m_lblPrintSetupPrintZoomFitToAcross;
        private System.Windows.Forms.Label m_lblPrintSetupPrintZoomFitToBy;
        private System.Windows.Forms.GroupBox m_groupPrintSetupMargins;
        private System.Windows.Forms.Label m_lblPrintSetupMarginsLeft;
        private System.Windows.Forms.TextBox m_txtPrintSetupMarginsLeft;
        private System.Windows.Forms.Label m_lblPrintSetupMarginsRight;
        private System.Windows.Forms.TextBox m_txtPrintSetupMarginsRight;
        private System.Windows.Forms.Label m_lblPrintSetupMarginsBottom;
        private System.Windows.Forms.TextBox m_txtPrintSetupMarginsBottom;
        private System.Windows.Forms.Label m_lblPrintSetupMarginsTop;
        private System.Windows.Forms.TextBox m_txtPrintSetupMarginsTop;
        private System.Windows.Forms.GroupBox m_groupPrintSetupPaper;
        private System.Windows.Forms.Label m_lblPrintSetupPaperSize;
        private System.Windows.Forms.Label m_lblPrintSetupPaperSource;
        private System.Windows.Forms.Label m_lblPrintSetupPaperOrientation;
        private System.Windows.Forms.RadioButton m_radioPrintSetupPaperPortrait;
        private System.Windows.Forms.RadioButton m_radioPrintSetupPaperLandscape;
        private System.Windows.Forms.ComboBox m_comboPrintSetupPaperSize;
        private System.Windows.Forms.ComboBox m_comboPrintSetupPaperSource;
        private System.Windows.Forms.PictureBox m_pictPrintSetupLayoutPreview;
        private System.ComponentModel.IContainer components;
        private PageSettings m_psPageSettings;
        private bool m_bIsMetric = false;
        private InternalMargins m_intMargins;
        private PrintZoom m_pzPrintZoom;
        private Hashtable m_hashPaperSize;
        private Hashtable m_hashPaperSource;
        private RectangleF m_rectReducedPaperRect = RectangleF.Empty;
        private RectangleF m_rectReducedMarginBounds = RectangleF.Empty;
        private ImageList imageList1;
        private float m_fScaleFactor = 1f;
        private bool m_bBackDelete = false;
        private bool m_bCancelActive = false;
        private Control m_ctrlFocused;        
        private bool m_activated = false;
        
        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="System.Drawing.Printing.PageSettings"/> value that corresponds to the diagram's page settings.
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.PageSettings"/>
        /// </summary>
        public PageSettings PageSettings
        {
            get
            {
                return m_psPageSettings;
            }
            set
            {
                if (m_psPageSettings != value)
                {
                    if (m_psPageSettings == null)
                    {
                        m_psPageSettings = (PageSettings)value.Clone();
                        SetInternalMargins(value.Margins);
                        DoPaperRelatedActions(value);
                    }
                    else
                        m_psPageSettings = value;
                }
            }
        }
        private InternalMargins InternalMargins
        {
            get
            {
                return m_intMargins;
            }
            set
            {
                if (m_intMargins != value)
                    m_intMargins = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Syncfusion.Windows.Forms.Diagram.PrintZoom"/> value that corresponds to the diagram's zoom settings.
        /// <see cref="Syncfusion.Windows.Forms.Diagram.View.PrintZoom"/>
        /// </summary>
        public PrintZoom PrintZoom
        {
            get
            {
                return m_pzPrintZoom;
            }
            set
            {
                if (m_pzPrintZoom != value)
                {
                    m_pzPrintZoom = (PrintZoom)value.Clone();
                    DoPrintZoomRelatedActions(m_pzPrintZoom.UsePrintingZoom);
                }
            }
        }

        #region ReducedPaperView Properties
        private RectangleF ReducedPaperViewRect
        {
            get
            {
                if (m_rectReducedPaperRect == RectangleF.Empty)
                    m_rectReducedPaperRect = new RectangleF();
                return m_rectReducedPaperRect;
            }
            set
            {
                if (m_rectReducedPaperRect != value)
                {
                    m_rectReducedPaperRect = value;
                    RefreshReducedPaperView();
                }
            }
        }
        private RectangleF ReducedMarginBounds
        {
            get
            {
                if (m_rectReducedMarginBounds == RectangleF.Empty)
                    m_rectReducedMarginBounds = new RectangleF();
                return m_rectReducedMarginBounds;
            }
            set
            {
                if (m_rectReducedMarginBounds != value)
                {
                    m_rectReducedMarginBounds = value;
                    RefreshReducedPaperView();
                }
            }
        }

        private float ScaleFactor
        {
            get
            {
                return m_fScaleFactor;
            }
            set
            {
                if (m_fScaleFactor != value)
                    m_fScaleFactor = value;
            }
        }

        #endregion ReducedPaperView Properties
        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintSetupDialog"/> class.
        /// </summary>
        public PrintSetupDialog()
        {
            InitializeComponent();
            m_bIsMetric = IsMetricCurrentMeasureUnits();
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintSetupDialog));
            this.m_btnPrintSetupOK = new System.Windows.Forms.Button();
            this.m_btnPrintSetupCancel = new System.Windows.Forms.Button();
            this.m_groupPrintSetupPrintZoom = new System.Windows.Forms.GroupBox();
            this.m_lblPrintSetupPrintZoomAdjustTo = new System.Windows.Forms.Label();
            this.m_txtPrintSetupPrintZoomAdjustTo = new System.Windows.Forms.TextBox();
            this.m_radioPrintSetupPrintZoomFitTo = new System.Windows.Forms.RadioButton();
            this.m_radioPrintSetupPrintZoomAdjustTo = new System.Windows.Forms.RadioButton();
            this.m_txtPrintSetupPrintZoomFitToAcross = new System.Windows.Forms.TextBox();
            this.m_txtPrintSetupPrintZoomFitToBy = new System.Windows.Forms.TextBox();
            this.m_lblPrintSetupPrintZoomFitToAcross = new System.Windows.Forms.Label();
            this.m_lblPrintSetupPrintZoomFitToBy = new System.Windows.Forms.Label();
            this.m_groupPrintSetupMargins = new System.Windows.Forms.GroupBox();
            this.m_txtPrintSetupMarginsLeft = new System.Windows.Forms.TextBox();
            this.m_lblPrintSetupMarginsLeft = new System.Windows.Forms.Label();
            this.m_lblPrintSetupMarginsRight = new System.Windows.Forms.Label();
            this.m_txtPrintSetupMarginsRight = new System.Windows.Forms.TextBox();
            this.m_lblPrintSetupMarginsBottom = new System.Windows.Forms.Label();
            this.m_txtPrintSetupMarginsBottom = new System.Windows.Forms.TextBox();
            this.m_lblPrintSetupMarginsTop = new System.Windows.Forms.Label();
            this.m_txtPrintSetupMarginsTop = new System.Windows.Forms.TextBox();
            this.m_groupPrintSetupPaper = new System.Windows.Forms.GroupBox();
            this.m_radioPrintSetupPaperPortrait = new System.Windows.Forms.RadioButton();
            this.m_comboPrintSetupPaperSize = new System.Windows.Forms.ComboBox();
            this.m_lblPrintSetupPaperSize = new System.Windows.Forms.Label();
            this.m_lblPrintSetupPaperSource = new System.Windows.Forms.Label();
            this.m_comboPrintSetupPaperSource = new System.Windows.Forms.ComboBox();
            this.m_lblPrintSetupPaperOrientation = new System.Windows.Forms.Label();
            this.m_radioPrintSetupPaperLandscape = new System.Windows.Forms.RadioButton();
            this.m_pictPrintSetupLayoutPreview = new System.Windows.Forms.PictureBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.m_groupPrintSetupPrintZoom.SuspendLayout();
            this.m_groupPrintSetupMargins.SuspendLayout();
            this.m_groupPrintSetupPaper.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_pictPrintSetupLayoutPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btnPrintSetupOK
            // 
            this.m_btnPrintSetupOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.m_btnPrintSetupOK, "m_btnPrintSetupOK");
            this.m_btnPrintSetupOK.Name = "m_btnPrintSetupOK";
            // 
            // m_btnPrintSetupCancel
            // 
            this.m_btnPrintSetupCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.m_btnPrintSetupCancel, "m_btnPrintSetupCancel");
            this.m_btnPrintSetupCancel.Name = "m_btnPrintSetupCancel";
            // 
            // m_groupPrintSetupPrintZoom
            // 
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_lblPrintSetupPrintZoomAdjustTo);
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_txtPrintSetupPrintZoomAdjustTo);
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_radioPrintSetupPrintZoomFitTo);
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_radioPrintSetupPrintZoomAdjustTo);
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_txtPrintSetupPrintZoomFitToAcross);
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_txtPrintSetupPrintZoomFitToBy);
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_lblPrintSetupPrintZoomFitToAcross);
            this.m_groupPrintSetupPrintZoom.Controls.Add(this.m_lblPrintSetupPrintZoomFitToBy);
            this.m_groupPrintSetupPrintZoom.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            resources.ApplyResources(this.m_groupPrintSetupPrintZoom, "m_groupPrintSetupPrintZoom");
            this.m_groupPrintSetupPrintZoom.Name = "m_groupPrintSetupPrintZoom";
            this.m_groupPrintSetupPrintZoom.TabStop = false;
            // 
            // m_lblPrintSetupPrintZoomAdjustTo
            // 
            this.m_lblPrintSetupPrintZoomAdjustTo.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupPrintZoomAdjustTo, "m_lblPrintSetupPrintZoomAdjustTo");
            this.m_lblPrintSetupPrintZoomAdjustTo.Name = "m_lblPrintSetupPrintZoomAdjustTo";
            // 
            // m_txtPrintSetupPrintZoomAdjustTo
            // 
            resources.ApplyResources(this.m_txtPrintSetupPrintZoomAdjustTo, "m_txtPrintSetupPrintZoomAdjustTo");
            this.m_txtPrintSetupPrintZoomAdjustTo.Name = "m_txtPrintSetupPrintZoomAdjustTo";
            this.m_txtPrintSetupPrintZoomAdjustTo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupPrintZoomAdjustTo_KeyDown);
            this.m_txtPrintSetupPrintZoomAdjustTo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PrintSetupPrintZoomAdjustTo_KeyPress);
            // 
            // m_radioPrintSetupPrintZoomFitTo
            // 
            this.m_radioPrintSetupPrintZoomFitTo.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_radioPrintSetupPrintZoomFitTo, "m_radioPrintSetupPrintZoomFitTo");
            this.m_radioPrintSetupPrintZoomFitTo.Name = "m_radioPrintSetupPrintZoomFitTo";
            this.m_radioPrintSetupPrintZoomFitTo.CheckedChanged += new System.EventHandler(this.PrintSetupPrintZoomFitTo_CheckedChanged);
            // 
            // m_radioPrintSetupPrintZoomAdjustTo
            // 
            this.m_radioPrintSetupPrintZoomAdjustTo.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_radioPrintSetupPrintZoomAdjustTo, "m_radioPrintSetupPrintZoomAdjustTo");
            this.m_radioPrintSetupPrintZoomAdjustTo.Name = "m_radioPrintSetupPrintZoomAdjustTo";
            this.m_radioPrintSetupPrintZoomAdjustTo.CheckedChanged += new System.EventHandler(this.PrintSetupPrintZoomAdjustTo_CheckedChanged);
            // 
            // m_txtPrintSetupPrintZoomFitToAcross
            // 
            resources.ApplyResources(this.m_txtPrintSetupPrintZoomFitToAcross, "m_txtPrintSetupPrintZoomFitToAcross");
            this.m_txtPrintSetupPrintZoomFitToAcross.Name = "m_txtPrintSetupPrintZoomFitToAcross";
            this.m_txtPrintSetupPrintZoomFitToAcross.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupPrintZoomFitToAcross_KeyDown);
            this.m_txtPrintSetupPrintZoomFitToAcross.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PrintSetupPrintZoomFitToAcross_KeyPress);
            // 
            // m_txtPrintSetupPrintZoomFitToBy
            // 
            resources.ApplyResources(this.m_txtPrintSetupPrintZoomFitToBy, "m_txtPrintSetupPrintZoomFitToBy");
            this.m_txtPrintSetupPrintZoomFitToBy.Name = "m_txtPrintSetupPrintZoomFitToBy";
            this.m_txtPrintSetupPrintZoomFitToBy.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupPrintZoomFitToBy_KeyDown);
            this.m_txtPrintSetupPrintZoomFitToBy.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PrintSetupPrintZoomFitToBy_KeyPress);
            // 
            // m_lblPrintSetupPrintZoomFitToAcross
            // 
            this.m_lblPrintSetupPrintZoomFitToAcross.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupPrintZoomFitToAcross, "m_lblPrintSetupPrintZoomFitToAcross");
            this.m_lblPrintSetupPrintZoomFitToAcross.Name = "m_lblPrintSetupPrintZoomFitToAcross";
            // 
            // m_lblPrintSetupPrintZoomFitToBy
            // 
            this.m_lblPrintSetupPrintZoomFitToBy.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupPrintZoomFitToBy, "m_lblPrintSetupPrintZoomFitToBy");
            this.m_lblPrintSetupPrintZoomFitToBy.Name = "m_lblPrintSetupPrintZoomFitToBy";
            // 
            // m_groupPrintSetupMargins
            // 
            this.m_groupPrintSetupMargins.Controls.Add(this.m_txtPrintSetupMarginsLeft);
            this.m_groupPrintSetupMargins.Controls.Add(this.m_lblPrintSetupMarginsLeft);
            this.m_groupPrintSetupMargins.Controls.Add(this.m_lblPrintSetupMarginsRight);
            this.m_groupPrintSetupMargins.Controls.Add(this.m_txtPrintSetupMarginsRight);
            this.m_groupPrintSetupMargins.Controls.Add(this.m_lblPrintSetupMarginsBottom);
            this.m_groupPrintSetupMargins.Controls.Add(this.m_txtPrintSetupMarginsBottom);
            this.m_groupPrintSetupMargins.Controls.Add(this.m_lblPrintSetupMarginsTop);
            this.m_groupPrintSetupMargins.Controls.Add(this.m_txtPrintSetupMarginsTop);
            this.m_groupPrintSetupMargins.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            resources.ApplyResources(this.m_groupPrintSetupMargins, "m_groupPrintSetupMargins");
            this.m_groupPrintSetupMargins.Name = "m_groupPrintSetupMargins";
            this.m_groupPrintSetupMargins.TabStop = false;
            // 
            // m_txtPrintSetupMarginsLeft
            // 
            resources.ApplyResources(this.m_txtPrintSetupMarginsLeft, "m_txtPrintSetupMarginsLeft");
            this.m_txtPrintSetupMarginsLeft.Name = "m_txtPrintSetupMarginsLeft";
            this.m_txtPrintSetupMarginsLeft.TextChanged += new System.EventHandler(this.PrintSetupMarginsLeft_TextChanged);
            this.m_txtPrintSetupMarginsLeft.Validated += new System.EventHandler(this.PrintSetupMarginsLeft_Validated);
            this.m_txtPrintSetupMarginsLeft.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupMarginsLeft_KeyDown);
            this.m_txtPrintSetupMarginsLeft.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PrintSetupMarginsLeft_KeyPress);
            // 
            // m_lblPrintSetupMarginsLeft
            // 
            this.m_lblPrintSetupMarginsLeft.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupMarginsLeft, "m_lblPrintSetupMarginsLeft");
            this.m_lblPrintSetupMarginsLeft.Name = "m_lblPrintSetupMarginsLeft";
            // 
            // m_lblPrintSetupMarginsRight
            // 
            this.m_lblPrintSetupMarginsRight.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupMarginsRight, "m_lblPrintSetupMarginsRight");
            this.m_lblPrintSetupMarginsRight.Name = "m_lblPrintSetupMarginsRight";
            // 
            // m_txtPrintSetupMarginsRight
            // 
            resources.ApplyResources(this.m_txtPrintSetupMarginsRight, "m_txtPrintSetupMarginsRight");
            this.m_txtPrintSetupMarginsRight.Name = "m_txtPrintSetupMarginsRight";
            this.m_txtPrintSetupMarginsRight.TextChanged += new System.EventHandler(this.PrintSetupMarginsRight_TextChanged);
            this.m_txtPrintSetupMarginsRight.Validated += new System.EventHandler(this.PrintSetupMarginsRight_Validated);
            this.m_txtPrintSetupMarginsRight.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupMarginsRight_KeyDown);
            this.m_txtPrintSetupMarginsRight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PrintSetupMarginsRight_KeyPress);
            // 
            // m_lblPrintSetupMarginsBottom
            // 
            this.m_lblPrintSetupMarginsBottom.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupMarginsBottom, "m_lblPrintSetupMarginsBottom");
            this.m_lblPrintSetupMarginsBottom.Name = "m_lblPrintSetupMarginsBottom";
            // 
            // m_txtPrintSetupMarginsBottom
            // 
            resources.ApplyResources(this.m_txtPrintSetupMarginsBottom, "m_txtPrintSetupMarginsBottom");
            this.m_txtPrintSetupMarginsBottom.Name = "m_txtPrintSetupMarginsBottom";
            this.m_txtPrintSetupMarginsBottom.TextChanged += new System.EventHandler(this.PrintSetupMarginsBottom_TextChanged);
            this.m_txtPrintSetupMarginsBottom.Validated += new System.EventHandler(this.PrintSetupMarginsBottom_Validated);
            this.m_txtPrintSetupMarginsBottom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupMarginsBottom_KeyDown);
            this.m_txtPrintSetupMarginsBottom.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PrintSetupMarginsBottom_KeyPress);
            // 
            // m_lblPrintSetupMarginsTop
            // 
            this.m_lblPrintSetupMarginsTop.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupMarginsTop, "m_lblPrintSetupMarginsTop");
            this.m_lblPrintSetupMarginsTop.Name = "m_lblPrintSetupMarginsTop";
            // 
            // m_txtPrintSetupMarginsTop
            // 
            resources.ApplyResources(this.m_txtPrintSetupMarginsTop, "m_txtPrintSetupMarginsTop");
            this.m_txtPrintSetupMarginsTop.Name = "m_txtPrintSetupMarginsTop";
            this.m_txtPrintSetupMarginsTop.TextChanged += new System.EventHandler(this.PrintSetupMarginsTop_TextChanged);
            this.m_txtPrintSetupMarginsTop.Validated += new System.EventHandler(this.PrintSetupMarginsTop_Validated);
            this.m_txtPrintSetupMarginsTop.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PrintSetupMarginsTop_KeyDown);
            this.m_txtPrintSetupMarginsTop.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PrintSetupMarginsTop_KeyPress);
            // 
            // m_groupPrintSetupPaper
            // 
            this.m_groupPrintSetupPaper.Controls.Add(this.m_radioPrintSetupPaperPortrait);
            this.m_groupPrintSetupPaper.Controls.Add(this.m_comboPrintSetupPaperSize);
            this.m_groupPrintSetupPaper.Controls.Add(this.m_lblPrintSetupPaperSize);
            this.m_groupPrintSetupPaper.Controls.Add(this.m_lblPrintSetupPaperSource);
            this.m_groupPrintSetupPaper.Controls.Add(this.m_comboPrintSetupPaperSource);
            this.m_groupPrintSetupPaper.Controls.Add(this.m_lblPrintSetupPaperOrientation);
            this.m_groupPrintSetupPaper.Controls.Add(this.m_radioPrintSetupPaperLandscape);
            this.m_groupPrintSetupPaper.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            resources.ApplyResources(this.m_groupPrintSetupPaper, "m_groupPrintSetupPaper");
            this.m_groupPrintSetupPaper.Name = "m_groupPrintSetupPaper";
            this.m_groupPrintSetupPaper.TabStop = false;
            // 
            // m_radioPrintSetupPaperPortrait
            // 
            this.m_radioPrintSetupPaperPortrait.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_radioPrintSetupPaperPortrait, "m_radioPrintSetupPaperPortrait");
            this.m_radioPrintSetupPaperPortrait.Name = "m_radioPrintSetupPaperPortrait";
            // 
            // m_comboPrintSetupPaperSize
            // 
            this.m_comboPrintSetupPaperSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboPrintSetupPaperSize, "m_comboPrintSetupPaperSize");
            this.m_comboPrintSetupPaperSize.Name = "m_comboPrintSetupPaperSize";
            this.m_comboPrintSetupPaperSize.Sorted = true;
            this.m_comboPrintSetupPaperSize.SelectedIndexChanged += new System.EventHandler(this.PrintSetupPaperSize_SelectedIndexChanged);
            // 
            // m_lblPrintSetupPaperSize
            // 
            this.m_lblPrintSetupPaperSize.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupPaperSize, "m_lblPrintSetupPaperSize");
            this.m_lblPrintSetupPaperSize.Name = "m_lblPrintSetupPaperSize";
            // 
            // m_lblPrintSetupPaperSource
            // 
            this.m_lblPrintSetupPaperSource.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupPaperSource, "m_lblPrintSetupPaperSource");
            this.m_lblPrintSetupPaperSource.Name = "m_lblPrintSetupPaperSource";
            // 
            // m_comboPrintSetupPaperSource
            // 
            this.m_comboPrintSetupPaperSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.m_comboPrintSetupPaperSource, "m_comboPrintSetupPaperSource");
            this.m_comboPrintSetupPaperSource.Name = "m_comboPrintSetupPaperSource";
            this.m_comboPrintSetupPaperSource.SelectedIndexChanged += new System.EventHandler(this.PrintSetupPaperSource_SelectedIndexChanged);
            // 
            // m_lblPrintSetupPaperOrientation
            // 
            this.m_lblPrintSetupPaperOrientation.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_lblPrintSetupPaperOrientation, "m_lblPrintSetupPaperOrientation");
            this.m_lblPrintSetupPaperOrientation.Name = "m_lblPrintSetupPaperOrientation";
            // 
            // m_radioPrintSetupPaperLandscape
            // 
            this.m_radioPrintSetupPaperLandscape.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.m_radioPrintSetupPaperLandscape, "m_radioPrintSetupPaperLandscape");
            this.m_radioPrintSetupPaperLandscape.Name = "m_radioPrintSetupPaperLandscape";
            this.m_radioPrintSetupPaperLandscape.CheckedChanged += new System.EventHandler(this.PrintSetupPaperLandscape_CheckedChanged);
            // 
            // m_pictPrintSetupLayoutPreview
            // 
            resources.ApplyResources(this.m_pictPrintSetupLayoutPreview, "m_pictPrintSetupLayoutPreview");
            this.m_pictPrintSetupLayoutPreview.Name = "m_pictPrintSetupLayoutPreview";
            this.m_pictPrintSetupLayoutPreview.TabStop = false;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            // 
            // PrintSetupDialog
            // 
            this.AcceptButton = this.m_btnPrintSetupOK;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.m_pictPrintSetupLayoutPreview);
            this.Controls.Add(this.m_groupPrintSetupPaper);
            this.Controls.Add(this.m_groupPrintSetupMargins);
            this.Controls.Add(this.m_groupPrintSetupPrintZoom);
            this.Controls.Add(this.m_btnPrintSetupOK);
            this.Controls.Add(this.m_btnPrintSetupCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintSetupDialog";
            this.ShowInTaskbar = false;
            this.Deactivate += new System.EventHandler(this.PrintSetupDialog_Deactivate);
            this.Load += new System.EventHandler(this.PrintSetupDialog_Load);
            this.Activated += new System.EventHandler(this.PrintSetupDialog_Activated);
            this.m_groupPrintSetupPrintZoom.ResumeLayout(false);
            this.m_groupPrintSetupPrintZoom.PerformLayout();
            this.m_groupPrintSetupMargins.ResumeLayout(false);
            this.m_groupPrintSetupMargins.PerformLayout();
            this.m_groupPrintSetupPaper.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_pictPrintSetupLayoutPreview)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region Event Handlers
        private void PrintSetupDialog_Load(object sender, EventArgs e)
        {
            FillParerCombos();
            DetectAndSetMeasureUnits();
            RefreshReducedPaperView();
        }

        private void PrintSetupPrintZoomAdjustTo_CheckedChanged(object sender, EventArgs e)
        {
            AdjustTo_CheckedChanged_Handler(sender as RadioButton);
        }

        private void PrintSetupPrintZoomFitTo_CheckedChanged(object sender, EventArgs e)
        {
            FitTo_CheckedChanged_Handler(sender as RadioButton);
        }

        private void PrintSetupPaperLandscape_CheckedChanged(object sender, EventArgs e)
        {
            Landscape_CheckedChanged_Handler(sender as RadioButton);
        }

        private void PrintSetupPaperSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboSender = sender as ComboBox;
            string strSelectedItemName = (string)comboSender.SelectedItem;
            if (m_hashPaperSize.ContainsKey(strSelectedItemName))
            {
                PaperSize psSelected = (PaperSize)m_hashPaperSize[strSelectedItemName];
                this.PageSettings.PaperSize = psSelected;
                if (this.PageSettings.PaperSize.Kind == PaperKind.Custom)
                {
                    this.PageSettings.PaperSize =
                        new PaperSize(strSelectedItemName, c_nA4_FORMAT_WIDTH, c_nA4_FORMAT_HEIGHT);
                }

                // Update ReducedPaperViewRect and Refresh ReducedPaperView
                this.ReducedPaperViewRect =
                    UpdateReducedPaperRect(this.PageSettings.PaperSize.Width, this.PageSettings.PaperSize.Height, this.PageSettings.Landscape);
                this.ReducedMarginBounds = UpdateReducedMarginsBounds();
                RefreshReducedPaperView();
            }
        }

        private void PrintSetupPaperSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboSender = sender as ComboBox;
            string strSelectedItemName = (string)comboSender.SelectedItem;
            if (m_hashPaperSource.ContainsKey(strSelectedItemName))
                this.PageSettings.PaperSource = (PaperSource)m_hashPaperSource[strSelectedItemName];
        }

        private void PrintSetupDialog_Deactivate(object sender, EventArgs e)
        {
            m_ctrlFocused = this.ActiveControl;
            this.m_activated = false;
        }

        private void PrintSetupDialog_Activated(object sender, EventArgs e)
        {
            this.ActiveControl = m_ctrlFocused;
            this.m_activated = true;
        }

        private void PrintSetupMarginsLeft_TextChanged(object sender, EventArgs e)
        {
            float fValue = GetValue(((TextBox)sender).Text);
            UpdateMargins(Syncfusion.Windows.Forms.Diagram.Margin.Left, fValue);
            if (ValidateMargin(sender as TextBox))
            {
                RefreshReducedPaperView();
            }
        }

        private void PrintSetupMarginsLeft_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, false, c_nMAX_INTEGRAL_PART_DIGITS + c_nMAX_FRACTIONAL_PART_DIGITS);
        }

        private void PrintSetupMarginsLeft_KeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }

        private void PrintSetupMarginsRight_KeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }

        private void PrintSetupMarginsRight_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, false, c_nMAX_INTEGRAL_PART_DIGITS + c_nMAX_FRACTIONAL_PART_DIGITS);
        }

        private void PrintSetupMarginsRight_TextChanged(object sender, EventArgs e)
        {
            float fValue = GetValue(((TextBox)sender).Text);
            UpdateMargins(Syncfusion.Windows.Forms.Diagram.Margin.Right, fValue);
            if (ValidateMargin(sender as TextBox))
            {
                RefreshReducedPaperView();
            }
        }

        private void PrintSetupMarginsBottom_KeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }

        private void PrintSetupMarginsBottom_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, false, c_nMAX_INTEGRAL_PART_DIGITS + c_nMAX_FRACTIONAL_PART_DIGITS);
        }

        private void PrintSetupMarginsBottom_TextChanged(object sender, EventArgs e)
        {
            float fValue = GetValue(((TextBox)sender).Text);
            UpdateMargins(Syncfusion.Windows.Forms.Diagram.Margin.Bottom, fValue);
            if (ValidateMargin(sender as TextBox))
            {
                RefreshReducedPaperView();
            }
        }
        private void PrintSetupMarginsTop_KeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }

        private void PrintSetupMarginsTop_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, false, c_nMAX_INTEGRAL_PART_DIGITS + c_nMAX_FRACTIONAL_PART_DIGITS);
        }

        private void PrintSetupMarginsTop_TextChanged(object sender, EventArgs e)
        {
            float fValue = GetValue(((TextBox)sender).Text);
            UpdateMargins(Syncfusion.Windows.Forms.Diagram.Margin.Top, fValue);
            if (ValidateMargin(sender as TextBox))
            {
                RefreshReducedPaperView();
            }
        }

        private void PrintSetupPrintZoomAdjustTo_KeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }

        private void PrintSetupPrintZoomAdjustTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, true, c_nMAX_INTEGRAL_PART_DIGITS + c_nMAX_FRACTIONAL_PART_DIGITS);
        }

        private void PrintSetupPrintZoomFitToAcross_KeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }

        private void PrintSetupPrintZoomFitToAcross_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, true, c_nMAX_TWO_DIGITS);
        }

        private void PrintSetupPrintZoomFitToBy_KeyDown(object sender, KeyEventArgs e)
        {
            m_bBackDelete = IsBackOrDelete(e);
        }

        private void PrintSetupPrintZoomFitToBy_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !IsHandled(sender as TextBox, e, true, c_nMAX_TWO_DIGITS);
        }
        private void PrintSetupMarginsLeft_Validated(object sender, EventArgs e)
        {
            ValidateMargin(sender as TextBox);
        }

        private void PrintSetupMarginsRight_Validated(object sender, EventArgs e)
        {
            ValidateMargin(sender as TextBox);
        }

        private void PrintSetupMarginsTop_Validated(object sender, EventArgs e)
        {
            ValidateMargin(sender as TextBox);
        }

        private void PrintSetupMarginsBottom_Validated(object sender, EventArgs e)
        {
            ValidateMargin(sender as TextBox);
        }

        #endregion Event Handlers

        #region Helper Methods
        private void AdjustTo_CheckedChanged_Handler(RadioButton sender)
        {
            m_txtPrintSetupPrintZoomAdjustTo.Enabled = sender.Checked;
            this.PrintZoom.UsePrintingZoom = sender.Checked;
        }

        private void FitTo_CheckedChanged_Handler(RadioButton sender)
        {
            m_txtPrintSetupPrintZoomFitToAcross.Enabled = sender.Checked;
            m_txtPrintSetupPrintZoomFitToBy.Enabled = sender.Checked;
        }

        private void Landscape_CheckedChanged_Handler(RadioButton sender)
        {
            this.PageSettings.Landscape = sender.Checked;

            int nWidth = this.PageSettings.PaperSize.Width;
            int nHeight = this.PageSettings.PaperSize.Height;
            this.ReducedPaperViewRect = UpdateReducedPaperRect(nHeight, nWidth, !this.PageSettings.Landscape);
            this.ReducedMarginBounds = UpdateReducedMarginsBounds();
            RefreshReducedPaperView();
        }

        private void SetInternalMargins(Margins margins)
        {
            if (this.InternalMargins == null)
            {
                Margins marginsHardware = this.PageSettings.PrinterSettings.DefaultPageSettings.Margins;
                PaperSize defaultPaper = this.PageSettings.PrinterSettings.DefaultPageSettings.PaperSize;
                this.InternalMargins = new InternalMargins(this.PageSettings.Margins, new Margins(0, 0, 0, 0), m_bIsMetric);
            }

            this.InternalMargins.Init(margins);
            DoMarginsBinding();
        }
        private void DoPaperRelatedActions(PageSettings psPageSettings)
        {
            // Orientation related
            m_radioPrintSetupPaperLandscape.Checked = psPageSettings.Landscape;
            m_radioPrintSetupPaperPortrait.Checked = !m_radioPrintSetupPaperLandscape.Checked;
        }

        private void DoPrintZoomRelatedActions(bool bUsePrintingZoom)
        {
            m_radioPrintSetupPrintZoomAdjustTo.Checked = bUsePrintingZoom;
            AdjustTo_CheckedChanged_Handler(m_radioPrintSetupPrintZoomAdjustTo);
            m_radioPrintSetupPrintZoomFitTo.Checked = !m_radioPrintSetupPrintZoomAdjustTo.Checked;
            FitTo_CheckedChanged_Handler(m_radioPrintSetupPrintZoomFitTo);

            DoPrintZoomRelatedBinding();
        }

        private void DoPrintZoomRelatedBinding()
        {
            if (m_txtPrintSetupPrintZoomAdjustTo.DataBindings["Text"] != null)
                m_txtPrintSetupPrintZoomAdjustTo.DataBindings.Remove(m_txtPrintSetupPrintZoomAdjustTo.DataBindings["Text"]);
            m_txtPrintSetupPrintZoomAdjustTo.DataBindings.Add("Text", PrintZoom, "PrintingZoom");

            if (m_txtPrintSetupPrintZoomFitToAcross.DataBindings["Text"] != null)
                m_txtPrintSetupPrintZoomFitToAcross.DataBindings.Remove(m_txtPrintSetupPrintZoomFitToAcross.DataBindings["Text"]);
            m_txtPrintSetupPrintZoomFitToAcross.DataBindings.Add("Text", PrintZoom, "SheetsAcross");

            if (m_txtPrintSetupPrintZoomFitToBy.DataBindings["Text"] != null)
                m_txtPrintSetupPrintZoomFitToBy.DataBindings.Remove(m_txtPrintSetupPrintZoomFitToBy.DataBindings["Text"]);
            m_txtPrintSetupPrintZoomFitToBy.DataBindings.Add("Text", PrintZoom, "SheetsDown");
        }

        private void DoMarginsBinding()
        {
            if (m_txtPrintSetupMarginsLeft.DataBindings["Text"] != null)
                m_txtPrintSetupMarginsLeft.DataBindings.Remove(m_txtPrintSetupMarginsLeft.DataBindings["Text"]);
            m_txtPrintSetupMarginsLeft.DataBindings.Add("Text", InternalMargins, "Left");

            if (m_txtPrintSetupMarginsRight.DataBindings["Text"] != null)
                m_txtPrintSetupMarginsRight.DataBindings.Remove(m_txtPrintSetupMarginsRight.DataBindings["Text"]);
            m_txtPrintSetupMarginsRight.DataBindings.Add("Text", InternalMargins, "Right");

            if (m_txtPrintSetupMarginsTop.DataBindings["Text"] != null)
                m_txtPrintSetupMarginsTop.DataBindings.Remove(m_txtPrintSetupMarginsTop.DataBindings["Text"]);
            m_txtPrintSetupMarginsTop.DataBindings.Add("Text", InternalMargins, "Top");

            if (m_txtPrintSetupMarginsBottom.DataBindings["Text"] != null)
                m_txtPrintSetupMarginsBottom.DataBindings.Remove(m_txtPrintSetupMarginsBottom.DataBindings["Text"]);
            m_txtPrintSetupMarginsBottom.DataBindings.Add("Text", InternalMargins, "Bottom");
        }

        private void FillPaperSizeCombo(PrinterSettings.PaperSizeCollection paperSizes)
        {
            string strPaperName = string.Empty;
            if (m_hashPaperSize == null)
                m_hashPaperSize = new Hashtable();
            
            // Get paper sizes supported with current printer and add them to m_hashPaperSize
            foreach (PaperSize paperSize in paperSizes)
            {
                strPaperName = paperSize.PaperName;
                m_comboPrintSetupPaperSize.Items.Add(strPaperName);
                m_hashPaperSize.Add(strPaperName, paperSize);
            }

            if (m_comboPrintSetupPaperSize.Items.Count > 0)
                m_comboPrintSetupPaperSize.SelectedItem = this.PageSettings.PaperSize.PaperName;
        }

        private void FillParerSourceCombo(PrinterSettings.PaperSourceCollection paperSources)
        {
            string strSourceName = string.Empty;
            if (m_hashPaperSource == null)
                m_hashPaperSource = new Hashtable();
            foreach (PaperSource paperSource in paperSources)
            {
                strSourceName = paperSource.SourceName;
                if (!m_hashPaperSource.ContainsKey(strSourceName))
                {
                    if (!m_hashPaperSource.Contains(strSourceName))
                    {
                        m_comboPrintSetupPaperSource.Items.Add(strSourceName);
                        m_hashPaperSource.Add(strSourceName, paperSource);
                    }
                }
            }

            if (m_comboPrintSetupPaperSource.Items.Count > 0)
                m_comboPrintSetupPaperSource.SelectedItem = this.PageSettings.PaperSource.SourceName;
        }

        private void DetectAndSetMeasureUnits()
        {
            string strMeasureUnits = @"(";
            if (m_bIsMetric)
                strMeasureUnits += @"millimeters";
            else
                strMeasureUnits += @"inches";
            strMeasureUnits += @")";
            m_groupPrintSetupMargins.Text += strMeasureUnits;
        }

        private bool IsMetricCurrentMeasureUnits()
        {
            bool bIsMetric = false;
            if (CultureInfo.CurrentCulture.UseUserOverride)
            {
                int nCharNumner = 2;
                StringBuilder strbldToReturn = new StringBuilder(nCharNumner);
                LocaleInfo.GetLocaleInfo(
                    LocaleInfo.LOCALE_USER_DEFAULT, LocaleInfo.LOCALE_IMEASURE, strbldToReturn, nCharNumner);
                if (int.Parse(strbldToReturn.ToString()) == 0)
                    bIsMetric = true;
            }
            else
            {
                RegionInfo region = RegionInfo.CurrentRegion;
                bIsMetric = region.IsMetric;
            }

            return bIsMetric;
        }

        private void FillParerCombos()
        {
            PrintDocument printDoc = new PrintDocument();
            FillPaperSizeCombo(printDoc.PrinterSettings.PaperSizes);
            FillParerSourceCombo(printDoc.PrinterSettings.PaperSources);
        }

        private bool ValidateMargin(TextBox txtBox)
        {
            Form form = txtBox.Parent.Parent as Form;
            bool res = true;
            if (!this.m_activated) return res;

            // if we press cancel -- no need to validate margins
            if (form.ActiveControl == m_btnPrintSetupCancel)
                m_bCancelActive = true;

            if (this.ReducedMarginBounds.Width < 0 && !m_bCancelActive)
            {
                m_bCancelActive = false;
                MessageBox.Show("The margins overlap. Enter a different margin size!", "Page Setup", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtBox.SelectionStart = 0;
                txtBox.SelectionLength = txtBox.Text.Length;
                txtBox.Focus();
                res = false;
            }

            return res;
        }

        #region PaperView Helpers
        private void RefreshReducedPaperView()
        {
            Graphics gph = CreateGraphicsToRender();
            DrawShadow(gph);
            DrawReducedPaperView(gph);
            DrawReducedMarginBounds(gph);
        }

        private Graphics CreateGraphicsToRender()
        {
            Image img = new Bitmap(m_pictPrintSetupLayoutPreview.Bounds.Width, m_pictPrintSetupLayoutPreview.Bounds.Height);
            m_pictPrintSetupLayoutPreview.Image = img;
            return Graphics.FromImage(m_pictPrintSetupLayoutPreview.Image);
        }

        private void DrawShadow(Graphics gph)
        {
            SolidBrush brushShadow;

            RectangleF rectShadow = new RectangleF(
                new PointF(this.ReducedPaperViewRect.X + c_nSHADOW, this.ReducedPaperViewRect.Y + c_nSHADOW),
                this.ReducedPaperViewRect.Size);

            using (brushShadow = new SolidBrush(Color.FromKnownColor(KnownColor.ControlDark)))
            {
                gph.FillRectangle(brushShadow, rectShadow);
            }
        }

        private void DrawReducedPaperView(Graphics gph)
        {
            gph.FillRectangle(Brushes.White, this.ReducedPaperViewRect);

            DrawPaperBorders(gph);
        }

        private void DrawPaperBorders(Graphics gph)
        {
            SolidBrush brushPaperBorder;
            using (brushPaperBorder = new SolidBrush(Color.FromKnownColor(KnownColor.ControlDark)))
            {
                Pen pen = new Pen(brushPaperBorder, c_nONE);
                gph.DrawLine(
                    pen, 
                    this.ReducedPaperViewRect.Location,
                    new PointF(this.ReducedPaperViewRect.X + this.ReducedPaperViewRect.Width, this.ReducedPaperViewRect.Y));
                gph.DrawLine(
                    pen, 
                    this.ReducedPaperViewRect.Location,
                    new PointF(this.ReducedPaperViewRect.X, this.ReducedPaperViewRect.Y + this.ReducedPaperViewRect.Height));
            }

            using (brushPaperBorder = new SolidBrush(Color.FromKnownColor(KnownColor.Black)))
            {
                Pen pen = new Pen(brushPaperBorder, c_nONE);
                gph.DrawLine(
                    pen, 
                    new PointF(this.ReducedPaperViewRect.X + this.ReducedPaperViewRect.Width, this.ReducedPaperViewRect.Y),
                    new PointF(this.ReducedPaperViewRect.X + this.ReducedPaperViewRect.Width, this.ReducedPaperViewRect.Y + this.ReducedPaperViewRect.Height));
                gph.DrawLine(
                    pen, 
                    new PointF(this.ReducedPaperViewRect.X, this.ReducedPaperViewRect.Y + this.ReducedPaperViewRect.Height),
                    new PointF(this.ReducedPaperViewRect.X + this.ReducedPaperViewRect.Width, this.ReducedPaperViewRect.Y + this.ReducedPaperViewRect.Height));
            }
        }

        private void DrawReducedMarginBounds(Graphics gph)
        {
            SolidBrush brushBorder;

            RectangleF rectSource = new RectangleF(new PointF(0, 0), this.ReducedMarginBounds.Size);
            RectangleF rectDestination = RectangleF.Inflate(this.ReducedMarginBounds, -c_nONE, -c_nONE);

            gph.DrawImage(imageList1.Images[0], rectDestination, rectSource, GraphicsUnit.Pixel);

            // Draw bounds
            using (brushBorder = new SolidBrush(Color.FromKnownColor(KnownColor.ControlDark)))
            {
                Pen pen = new Pen(brushBorder, c_nONE);
                pen.DashStyle = DashStyle.Dash;
                gph.DrawRectangle(pen, rectDestination.X, rectDestination.Y, rectDestination.Width, rectDestination.Height);
            }
        }

        private RectangleF UpdateReducedPaperRect(int nWidth, int nHeight, bool bLandscape)
        {
            int nCurWidth;
            int nCurHeight;
            if (bLandscape)
            {
                nCurWidth = nHeight;
                nCurHeight = nWidth;
            }
            else
            {
                nCurWidth = nWidth;
                nCurHeight = nHeight;
            }
            return CalculateReducedPaperRect(nCurWidth, nCurHeight);
        }

        private RectangleF CalculateReducedPaperRect(int nWidth, int nHeight)
        {
            if ((nWidth <= 0) || (nHeight <= 0))
                throw new ArgumentException("Invalid argument.");

            RectangleF rectToReturn = new RectangleF();
            Size szPaperView =
                new Size(m_pictPrintSetupLayoutPreview.Width - c_nSHADOW, m_pictPrintSetupLayoutPreview.Height - c_nSHADOW);

            rectToReturn.Size = CalculateSize(nWidth, nHeight);
            rectToReturn.Location = CalculateCenterLocation(rectToReturn.Size, szPaperView);
            return rectToReturn;
        }

        private SizeF CalculateSize(float nWidth, float nHeight)
        {
            this.ScaleFactor = Math.Min(
                (m_pictPrintSetupLayoutPreview.Size.Width - c_nSHADOW) / nWidth,
                (m_pictPrintSetupLayoutPreview.Size.Height - c_nSHADOW) / nHeight);
            return new SizeF(nWidth * this.ScaleFactor, nHeight * this.ScaleFactor);
        }

        private PointF CalculateCenterLocation(SizeF szRect, Size szRelative)
        {
            PointF ptToReturn = new PointF();

            ptToReturn.X = (szRelative.Width / 2) - (szRect.Width / 2);
            ptToReturn.Y = (szRelative.Height / 2) - (szRect.Height / 2);

            return ptToReturn;
        }

        private RectangleF UpdateReducedMarginsBounds()
        {
            RectangleF rectReducedMarginBounds = new RectangleF();
            RectangleF rectReducedPaper = this.ReducedPaperViewRect;
            float fScaleFactor = this.ScaleFactor;

            rectReducedMarginBounds.X = rectReducedPaper.X + (this.InternalMargins.Margins.Left * fScaleFactor);
            rectReducedMarginBounds.Y = rectReducedPaper.Y + (this.InternalMargins.Margins.Top * fScaleFactor);
            rectReducedMarginBounds.Width = rectReducedPaper.Width - ((this.InternalMargins.Margins.Right * fScaleFactor) + (rectReducedMarginBounds.X - rectReducedPaper.X));
            rectReducedMarginBounds.Height = rectReducedPaper.Height - ((this.InternalMargins.Margins.Bottom * fScaleFactor) + (rectReducedMarginBounds.Y - rectReducedPaper.Y));

            return rectReducedMarginBounds;
        }

        private void UpdateMargins(Syncfusion.Windows.Forms.Diagram.Margin marginAffected, float fNewValue)
        {
            switch (marginAffected)
            {
                case Syncfusion.Windows.Forms.Diagram.Margin.Left:
                    float fNewXPosition = this.ReducedPaperViewRect.X + fNewValue;
                    m_rectReducedMarginBounds.Width -= fNewXPosition - m_rectReducedMarginBounds.X;
                    m_rectReducedMarginBounds.X = fNewXPosition;
                    break;
                case Syncfusion.Windows.Forms.Diagram.Margin.Right:
                    m_rectReducedMarginBounds.Width = this.ReducedPaperViewRect.Width - (fNewValue + (m_rectReducedMarginBounds.X - this.ReducedPaperViewRect.X));
                    break;
                case Syncfusion.Windows.Forms.Diagram.Margin.Top:
                    float fNewYPosition = this.ReducedPaperViewRect.Y + fNewValue;
                    m_rectReducedMarginBounds.Height -= fNewYPosition - m_rectReducedMarginBounds.Y;
                    m_rectReducedMarginBounds.Y = fNewYPosition;
                    break;
                case Syncfusion.Windows.Forms.Diagram.Margin.Bottom:
                    m_rectReducedMarginBounds.Height = this.ReducedPaperViewRect.Height - (fNewValue + (m_rectReducedMarginBounds.Y - this.ReducedPaperViewRect.Y));
                    break;
            }
        }

        private float GetValue(string strValue)
        {
            // Convert to int
            NumberFormatInfo nfi = new NumberFormatInfo();
            RegexOptions regOpt = RegexOptions.IgnoreCase;
            Match match = Regex.Match(strValue, "[" + c_strCOMMA + "]", regOpt);
            if (match.Success)
            {
                if (strValue.Length == 1)
                    strValue = "0";
            }
            else if (strValue.Length == 0)
            {
                strValue = "0";
            }

            float fValue = (float)double.Parse(strValue, nfi);

            // Convert to PrinterUnits.HundredsOfInch
            float nConvertedValue;
            if (m_bIsMetric)
            {
                nConvertedValue = (float)PrinterUnitConvert.Convert(fValue, PrinterUnit.HundredthsOfAMillimeter, PrinterUnit.ThousandthsOfAnInch);
                nConvertedValue *= c_nTEN;
            }
            else
            {
                nConvertedValue = fValue * c_nONE_HUNDRED;
            }

            return nConvertedValue * this.ScaleFactor;
        }

        private bool IsBackOrDelete(KeyEventArgs e)
        {
            bool bBackDelete = false;

            if (e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete)
                bBackDelete = true;

            return bBackDelete;
        }

        private bool IsHandled(TextBox txtBox, KeyPressEventArgs e, bool bOnlyNumbers, int nMaxAllowedDigits)
        {
            bool bHandled = false;
            if (m_bBackDelete)
            {
                bHandled = true;
            }
            else
            {
                if (txtBox.Text.Length == nMaxAllowedDigits)
                {
                    // if there is selected text -- allow enter numbers
                    // consider comma also
                    if (txtBox.SelectionLength > 0)
                    {
                        if (!m_bIsMetric && !bOnlyNumbers)
                        {
                            Match match = Regex.Match(txtBox.Text, c_strCOMMA);
                            if (match.Success)
                            {
                                Match matchCommaInSelected = Regex.Match(txtBox.SelectedText, c_strCOMMA);
                                if (matchCommaInSelected.Success)
                                    bHandled = true;
                                else
                                {
                                    if (Char.IsDigit(e.KeyChar))
                                        bHandled = true;
                                    else
                                        bHandled = false;
                                }
                            }
                            else
                                bHandled = true;
                        }
                        else
                        {
                            if (Char.IsDigit(e.KeyChar))
                                bHandled = true;
                        }
                    }
                    else
                        bHandled = false;
                }
                else
                {
                    if (Char.IsDigit(e.KeyChar))
                        bHandled = true;
                    else if (!m_bIsMetric && !bOnlyNumbers)
                    {
                        // If comma is already present -- no more needed
                        Match match = Regex.Match(txtBox.Text, "[" + c_strCOMMA + "]");
                        if (match.Success)
                        {
                            if (txtBox.SelectionLength > 0)
                            {
                                Match matchCommaInSelected = Regex.Match(txtBox.SelectedText, "[" + c_strCOMMA + "]");
                                if (matchCommaInSelected.Success)
                                    bHandled = true;
                                else
                                    bHandled = false;
                            }
                            else
                                bHandled = false;
                        }
                        else
                        {
                            if (e.KeyChar == c_charCOMMA)
                                bHandled = true;
                            else
                                bHandled = false;
                        }
                    }
                }
            }
            return bHandled;
        }
        #endregion PaperView Helpers

        #endregion Helper Methods
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    internal class InternalMargins
    {
        #region Fields

        #region Constants
        private const int c_nTWO_DIGITS = 2;
        private const int c_nTEN = 10;
        private const int c_nONE_HUNDRED = 100;
        #endregion Constants

        private float m_fLeft;
        private float m_fRight;
        private float m_fTop;
        private float m_fBottom;
        private Margins m_marginsAffected;
        private Margins m_minMargins;
        private bool m_bIsMetric = false;
        
        #endregion Fields

        #region Constructor
        public InternalMargins(Margins margins, Margins minMargins, bool bIsMetric)
        {
            m_bIsMetric = bIsMetric;
            m_marginsAffected = margins;
            m_minMargins = minMargins;
        }
        #endregion Constructor

        #region Properties
        public Margins Margins
        {
            get
            {
                return m_marginsAffected;
            }
            set
            {
                if (m_marginsAffected != value)
                    m_marginsAffected = value;
            }
        }
        public float Left
        {
            get
            {
                return m_fLeft;
            }
            set
            {
                if (m_fLeft != value)
                {
                    int nValue = (int)Convert(value, false);
                    float fValue;

                    // Consider hardware margins
                    if (nValue < m_minMargins.Left)
                    {
                        nValue = m_minMargins.Left;
                        fValue = Convert(nValue, true);
                    }
                    else
                        fValue = value;

                    this.Margins.Left = nValue;

                    if (m_bIsMetric)
                        m_fLeft = (int)fValue;
                    else
                        m_fLeft = (float)Math.Round(fValue, c_nTWO_DIGITS);
                }
            }
        }
        public float Right
        {
            get
            {
                return m_fRight;
            }
            set
            {
                if (m_fRight != value)
                {
                    int nValue = (int)Convert(value, false);
                    float fValue;

                    // Consider hardware margins
                    if (nValue < m_minMargins.Right)
                    {
                        nValue = m_minMargins.Right;
                        fValue = Convert(nValue, true);
                    }
                    else
                        fValue = value;

                    this.Margins.Right = nValue;

                    if (m_bIsMetric)
                        m_fRight = (int)fValue;
                    else
                        m_fRight = (float)Math.Round(fValue, c_nTWO_DIGITS);
                }
            }
        }
        public float Top
        {
            get
            {
                return m_fTop;
            }
            set
            {
                if (m_fTop != value)
                {
                    int nValue = (int)Convert(value, false);
                    float fValue;

                    // Consider hardware margins
                    if (nValue < m_minMargins.Top)
                    {
                        nValue = m_minMargins.Top;
                        fValue = Convert(nValue, true);
                    }
                    else
                        fValue = value;

                    this.Margins.Top = nValue;

                    if (m_bIsMetric)
                        m_fTop = (int)fValue;
                    else
                        m_fTop = (float)Math.Round(fValue, c_nTWO_DIGITS);
                }
            }
        }
        public float Bottom
        {
            get
            {
                return m_fBottom;
            }
            set
            {
                if (m_fBottom != value)
                {
                    int nValue = (int)Convert(value, false);
                    float fValue;

                    // Consider hardware margins
                    if (nValue < m_minMargins.Bottom)
                    {
                        nValue = m_minMargins.Bottom;
                        fValue = Convert(nValue, true);
                    }
                    else
                        fValue = value;

                    this.Margins.Bottom = nValue;

                    if (m_bIsMetric)
                        m_fBottom = (int)fValue;
                    else
                        m_fBottom = (float)Math.Round(fValue, c_nTWO_DIGITS);
                }
            }
        }

        #endregion Properties

        #region Helper Methods
        public void Init(Margins marginPageSettings)
        {
            m_fLeft = Convert(marginPageSettings.Left, true);
            m_fRight = Convert(marginPageSettings.Right, true);
            m_fTop = Convert(marginPageSettings.Top, true);
            m_fBottom = Convert(marginPageSettings.Bottom, true);
        }

        private float Convert(float nMargin, bool bToUser)
        {
            float fValueToReturn;
            if (m_bIsMetric)
            {
                fValueToReturn = MilimetersConvert(nMargin, bToUser);
            }
            else
            {
                fValueToReturn = InchesConvert(nMargin, bToUser);
            }
            return fValueToReturn;
        }

        private float MilimetersConvert(float nMargin, bool bToUser)
        {
            float fValueToReturn;
            if (bToUser)
            {
                float nTempValue = PrinterUnitConvert.Convert((int)nMargin, PrinterUnit.ThousandthsOfAnInch, PrinterUnit.HundredthsOfAMillimeter);
                fValueToReturn = (int)Math.Round(nTempValue / c_nTEN);
            }
            else
            {
                int nValue = (int)nMargin * c_nTEN;
                fValueToReturn = PrinterUnitConvert.Convert(nValue, PrinterUnit.HundredthsOfAMillimeter, PrinterUnit.ThousandthsOfAnInch);
            }
            return fValueToReturn;
        }

        private float InchesConvert(float nMargin, bool bToUser)
        {
            float fValueToReturn;
            if (bToUser)
                fValueToReturn = nMargin / c_nONE_HUNDRED;
            else
                fValueToReturn = nMargin * c_nONE_HUNDRED;
            return fValueToReturn;
        }

        #endregion Helper Methods
    }

    /// <summary>
    /// Internal Margins
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    public enum Margin
    {
        /// <summary>
        /// Left margin.
        /// </summary>
        Left = 1,

        /// <summary>
        /// Right margin.
        /// </summary>
        Right,

        /// <summary>
        /// Top margin.
        /// </summary>
        Top,

        /// <summary>
        /// Bottom margin.
        /// </summary>
        Bottom
    }
}

