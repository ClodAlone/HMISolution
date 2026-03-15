#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Diagram
{
    /// <summary>
    /// Allow users to setup print, page and drawing setting.
    /// </summary>
    public class PageSetupDialog : Form
    {
        private readonly View m_view;

        #region Form components
        private TabControl tbControl;
        private TabPage tbPrintSetup;
        private TabPage tbPageSize;
        private Button btnCancel;
        private Button btnOk;
        private Button btnApply;
        private TabPage tbDrawingScale;
        private GroupBox grpPreview;
        private PictureBox pcPreview;
        private System.Windows.Forms.Label lblPreviewDrawingPage;
        private System.Windows.Forms.Label lblPreviewPrinterPage;
        private System.Windows.Forms.Label lblPreviewLine;
        private System.Windows.Forms.Label lblPreviewPrintZoom;
        private System.Windows.Forms.Label lblPreviewPrintZoomValue;
        private System.Windows.Forms.Label lblPreviewDrawingPageValue;
        private System.Windows.Forms.Label lblPreviewPrinterPageOrientation;
        private System.Windows.Forms.Label lblPreviewPrinterPageValue;
        private System.Windows.Forms.Label lblPreviewDrawingPageOrientation;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        #endregion

        #region Class members
        private PageSettings m_printSetting;
        private PrintZoom m_nPrintZoom;
        private Hashtable m_hashPaperSize = null;
        private GroupBox grpPrintSetupPrintZoom;
        private Syncfusion.Windows.Forms.Tools.IntegerTextBox txtPrintSetupZoomFitToBy;
        private Syncfusion.Windows.Forms.Tools.IntegerTextBox txtPrintSetupZoomFitToAcross;
        private ComboBox comboPrintSetupZoomAdjustTo;
        private RadioButton rdPrintSetupZoomFitTo;
        private RadioButton rdPrintSetupZoomAdjustTo;
        private System.Windows.Forms.Label lblPrintSetupZoomFitToAcross;
        private System.Windows.Forms.Label lblPrintSetupZoomFitToBy;
        private GroupBox grpPrintSetupPrinterPaper;
        private Button btnPrintSetup;
        private RadioButton rdPrintSetupPaperPortrait;
        private ComboBox comboPrintSetupPaperSize;
        private RadioButton rdPrintSetupPaperLandscape;
        private PageSizeControl cntPageSize;
        private DrawingScaleControl cntDrawingScale;
        private bool m_bLockUpdate = false;
        #endregion

        #region Class properties
        /// <summary>
        /// Gets or sets the print settings.
        /// </summary>
        /// <value>The print settings.</value>
        public PageSettings PrintSettings
        {
            get
            {
                if (m_printSetting == null)
                {
                    m_printSetting = new PageSettings();

                    // set default margins
                    m_printSetting.Margins = new Margins(0, 0, 0, 0);
                }

                return m_printSetting;
            }
            set
            {
                if (m_printSetting != value)
                {
                    m_printSetting = value;
                    InitializePrintPaper();
                    UpdatePrintPreviewInfo();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the print zoom factor.
        /// </summary>
        /// <value>The print zoom.</value>
        public PrintZoom PrintZoom
        {
            get
            {
                if (m_nPrintZoom == null)
                    m_nPrintZoom = new PrintZoom();

                return m_nPrintZoom;
            }
            set
            {
                if (m_nPrintZoom != value)
                {
                    m_nPrintZoom = value;
                    InitializePrintZoom();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the page size setting.
        /// </summary>
        /// <value>The size of the page.</value>
        public PageSize PageSize
        {
            get 
            { 
                return cntPageSize.PageSize; 
            }
            set
            {
                cntPageSize.PageSize = value;
                UpdateDrawPreviewInfo();
            }
        }

        /// <summary>
        /// Gets or sets the page scale setting.
        /// </summary>
        /// <value>The page scale.</value>
        public PageScale PageScale
        {
            get { return cntDrawingScale.PageScale; }
            set { cntDrawingScale.PageScale = value; }
        }

        /// <summary>
        /// Gets a value indicating whether page landscape is checked.
        /// </summary>
        /// <value><c>true</c> if page landscape is checked; otherwise, <c>false</c>.</value>
        protected bool PageLandscape
        {
            get { return cntPageSize.rdPageSizeLandscape.Checked; }
        }

        /// <summary>
        /// Gets or sets the current setting measure units.
        /// </summary>
        /// <value>The measure units.</value>
        protected MeasureUnits MeasureUnits
        {
            get { return cntDrawingScale.MeasureUnits; }
            set { cntDrawingScale.MeasureUnits = value; }
        }
        #endregion

        #region Class initialize/finalize
        /// <summary>
        /// Prevents a default instance of the <see cref="PageSetupDialog"/> class from being created.
        /// </summary>
        private PageSetupDialog()
        {
            InitializeComponent();

            // initalize combos
            FillSetupCombos();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSetupDialog"/> class.
        /// </summary>
        /// <param name="view">The diagram <see cref="Syncfusion.Windows.Forms.Diagram.View"/>.</param>
        public PageSetupDialog(View view)
            : this()
        {
            if (view == null)
                throw new ArgumentNullException("View can't be null");

            m_view = view;
            Model model = m_view.Model;

            // set model values
            this.MeasureUnits = model.MeasurementUnits;
            this.PageScale = (PageScale)model.DocumentScale.Clone();
            this.PrintSettings = (PageSettings)m_view.PageSettings.Clone();                       
            if (this.PrintSettings.Landscape)
                this.cntPageSize.rdPageSizeLandscape.Checked = true;
            this.PageSize = ChangeOrientation((PageSize)model.DocumentSize.Clone(), this.PrintSettings.Landscape);           
			this.cntPageSize.rdPageSizeLandscape.CheckedChanged += new EventHandler(PageSizeLandscape_CheckedChanged);
            this.cntPageSize.rdPageSizePortrait.CheckedChanged += new EventHandler(PageSizeLandscape_CheckedChanged);
            RectangleF rcRect = new HandleRenderer().GetBoundingRect(model.Nodes);
            float fWidth = Math.Max(rcRect.Right, model.MinimumSize.Width);
            float fHeight = Math.Max(rcRect.Bottom, model.MinimumSize.Height);
            cntPageSize.ModelContentSize = new SizeF(fWidth, fHeight);
            if (m_view.Model.SizeToContent)
                cntPageSize.rdPageSizeToFitDrawingContent.Checked = true;
            // set view values            
            this.PrintZoom = (PrintZoom)m_view.PrintZoom.Clone();

            // update settings
            UpdatePrintSettings();

            // generate preview image
            UpdatePreview();
            btnApply.Enabled = false;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PageSetupDialog));
            Syncfusion.Windows.Forms.Diagram.PageSize pageSize3 = new Syncfusion.Windows.Forms.Diagram.PageSize();
            Syncfusion.Windows.Forms.Diagram.PageScale pageScale2 = new Syncfusion.Windows.Forms.Diagram.PageScale();
            Syncfusion.Windows.Forms.Diagram.PageSize pageSize4 = new Syncfusion.Windows.Forms.Diagram.PageSize();
            this.tbControl = new System.Windows.Forms.TabControl();
            this.tbPrintSetup = new System.Windows.Forms.TabPage();
            this.grpPrintSetupPrintZoom = new System.Windows.Forms.GroupBox();
            this.txtPrintSetupZoomFitToBy = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
            this.txtPrintSetupZoomFitToAcross = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
            this.comboPrintSetupZoomAdjustTo = new System.Windows.Forms.ComboBox();
            this.rdPrintSetupZoomFitTo = new System.Windows.Forms.RadioButton();
            this.rdPrintSetupZoomAdjustTo = new System.Windows.Forms.RadioButton();
            this.lblPrintSetupZoomFitToAcross = new System.Windows.Forms.Label();
            this.lblPrintSetupZoomFitToBy = new System.Windows.Forms.Label();
            this.grpPrintSetupPrinterPaper = new System.Windows.Forms.GroupBox();
            this.btnPrintSetup = new System.Windows.Forms.Button();
            this.rdPrintSetupPaperPortrait = new System.Windows.Forms.RadioButton();
            this.comboPrintSetupPaperSize = new System.Windows.Forms.ComboBox();
            this.rdPrintSetupPaperLandscape = new System.Windows.Forms.RadioButton();
            this.tbPageSize = new System.Windows.Forms.TabPage();
            this.cntPageSize = new Syncfusion.Windows.Forms.Diagram.PageSizeControl();
            this.tbDrawingScale = new System.Windows.Forms.TabPage();
            this.cntDrawingScale = new Syncfusion.Windows.Forms.Diagram.DrawingScaleControl();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.lblPreviewDrawingPageValue = new System.Windows.Forms.Label();
            this.lblPreviewPrinterPageValue = new System.Windows.Forms.Label();
            this.lblPreviewLine = new System.Windows.Forms.Label();
            this.lblPreviewPrintZoom = new System.Windows.Forms.Label();
            this.lblPreviewDrawingPage = new System.Windows.Forms.Label();
            this.lblPreviewPrintZoomValue = new System.Windows.Forms.Label();
            this.lblPreviewDrawingPageOrientation = new System.Windows.Forms.Label();
            this.lblPreviewPrinterPageOrientation = new System.Windows.Forms.Label();
            this.lblPreviewPrinterPage = new System.Windows.Forms.Label();
            this.pcPreview = new System.Windows.Forms.PictureBox();
            this.tbControl.SuspendLayout();
            this.tbPrintSetup.SuspendLayout();
            this.grpPrintSetupPrintZoom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrintSetupZoomFitToBy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrintSetupZoomFitToAcross)).BeginInit();
            this.grpPrintSetupPrinterPaper.SuspendLayout();
            this.tbPageSize.SuspendLayout();
            this.tbDrawingScale.SuspendLayout();
            this.grpPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // tbControl
            // 
            this.tbControl.Controls.Add(this.tbPrintSetup);
            this.tbControl.Controls.Add(this.tbPageSize);
            this.tbControl.Controls.Add(this.tbDrawingScale);
            resources.ApplyResources(this.tbControl, "tbControl");
            this.tbControl.Name = "tbControl";
            this.tbControl.SelectedIndex = 0;
            // 
            // tbPrintSetup
            // 
            this.tbPrintSetup.Controls.Add(this.grpPrintSetupPrintZoom);
            this.tbPrintSetup.Controls.Add(this.grpPrintSetupPrinterPaper);
            resources.ApplyResources(this.tbPrintSetup, "tbPrintSetup");
            this.tbPrintSetup.Name = "tbPrintSetup";
            this.tbPrintSetup.UseVisualStyleBackColor = true;
            // 
            // grpPrintSetupPrintZoom
            // 
            this.grpPrintSetupPrintZoom.Controls.Add(this.txtPrintSetupZoomFitToBy);
            this.grpPrintSetupPrintZoom.Controls.Add(this.txtPrintSetupZoomFitToAcross);
            this.grpPrintSetupPrintZoom.Controls.Add(this.comboPrintSetupZoomAdjustTo);
            this.grpPrintSetupPrintZoom.Controls.Add(this.rdPrintSetupZoomFitTo);
            this.grpPrintSetupPrintZoom.Controls.Add(this.rdPrintSetupZoomAdjustTo);
            this.grpPrintSetupPrintZoom.Controls.Add(this.lblPrintSetupZoomFitToAcross);
            this.grpPrintSetupPrintZoom.Controls.Add(this.lblPrintSetupZoomFitToBy);
            resources.ApplyResources(this.grpPrintSetupPrintZoom, "grpPrintSetupPrintZoom");
            this.grpPrintSetupPrintZoom.Name = "grpPrintSetupPrintZoom";
            this.grpPrintSetupPrintZoom.TabStop = false;
            // 
            // txtPrintSetupZoomFitToBy
            // 
            resources.ApplyResources(this.txtPrintSetupZoomFitToBy, "txtPrintSetupZoomFitToBy");
            this.txtPrintSetupZoomFitToBy.IntegerValue = ((long)(1));
            this.txtPrintSetupZoomFitToBy.MaxValue = ((long)(50));
            this.txtPrintSetupZoomFitToBy.MinValue = ((long)(1));
            this.txtPrintSetupZoomFitToBy.Name = "txtPrintSetupZoomFitToBy";
            this.txtPrintSetupZoomFitToBy.NegativeInputPendingOnSelectAll = false;
            this.txtPrintSetupZoomFitToBy.NullString = "";
            this.txtPrintSetupZoomFitToBy.OverflowIndicatorToolTipText = null;
            this.txtPrintSetupZoomFitToBy.TextChanged += new System.EventHandler(this.PrintZoomFitTo_TextChanged);
            // 
            // txtPrintSetupZoomFitToAcross
            // 
            resources.ApplyResources(this.txtPrintSetupZoomFitToAcross, "txtPrintSetupZoomFitToAcross");
            this.txtPrintSetupZoomFitToAcross.IntegerValue = ((long)(1));
            this.txtPrintSetupZoomFitToAcross.MaxValue = ((long)(50));
            this.txtPrintSetupZoomFitToAcross.MinValue = ((long)(1));
            this.txtPrintSetupZoomFitToAcross.Name = "txtPrintSetupZoomFitToAcross";
            this.txtPrintSetupZoomFitToAcross.NegativeInputPendingOnSelectAll = false;
            this.txtPrintSetupZoomFitToAcross.NullString = "";
            this.txtPrintSetupZoomFitToAcross.OverflowIndicatorToolTipText = null;
            this.txtPrintSetupZoomFitToAcross.TextChanged += new System.EventHandler(this.PrintZoomFitTo_TextChanged);
            // 
            // comboPrintSetupZoomAdjustTo
            // 
            this.comboPrintSetupZoomAdjustTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPrintSetupZoomAdjustTo.FormattingEnabled = true;
            resources.ApplyResources(this.comboPrintSetupZoomAdjustTo, "comboPrintSetupZoomAdjustTo");
            this.comboPrintSetupZoomAdjustTo.Name = "comboPrintSetupZoomAdjustTo";
            this.comboPrintSetupZoomAdjustTo.SelectedIndexChanged += new System.EventHandler(this.PrintZoomAdjustTo_SelectedIndexChanged);
            // 
            // rdPrintSetupZoomFitTo
            // 
            this.rdPrintSetupZoomFitTo.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.rdPrintSetupZoomFitTo, "rdPrintSetupZoomFitTo");
            this.rdPrintSetupZoomFitTo.Name = "rdPrintSetupZoomFitTo";
            this.rdPrintSetupZoomFitTo.CheckedChanged += new System.EventHandler(this.PrintZoom_CheckedChanged);
            // 
            // rdPrintSetupZoomAdjustTo
            // 
            this.rdPrintSetupZoomAdjustTo.Checked = true;
            this.rdPrintSetupZoomAdjustTo.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.rdPrintSetupZoomAdjustTo, "rdPrintSetupZoomAdjustTo");
            this.rdPrintSetupZoomAdjustTo.Name = "rdPrintSetupZoomAdjustTo";
            this.rdPrintSetupZoomAdjustTo.TabStop = true;
            this.rdPrintSetupZoomAdjustTo.CheckedChanged += new System.EventHandler(this.PrintZoom_CheckedChanged);
            // 
            // lblPrintSetupZoomFitToAcross
            // 
            this.lblPrintSetupZoomFitToAcross.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.lblPrintSetupZoomFitToAcross, "lblPrintSetupZoomFitToAcross");
            this.lblPrintSetupZoomFitToAcross.Name = "lblPrintSetupZoomFitToAcross";
            // 
            // lblPrintSetupZoomFitToBy
            // 
            this.lblPrintSetupZoomFitToBy.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.lblPrintSetupZoomFitToBy, "lblPrintSetupZoomFitToBy");
            this.lblPrintSetupZoomFitToBy.Name = "lblPrintSetupZoomFitToBy";
            // 
            // grpPrintSetupPrinterPaper
            // 
            this.grpPrintSetupPrinterPaper.Controls.Add(this.btnPrintSetup);
            this.grpPrintSetupPrinterPaper.Controls.Add(this.rdPrintSetupPaperPortrait);
            this.grpPrintSetupPrinterPaper.Controls.Add(this.comboPrintSetupPaperSize);
            this.grpPrintSetupPrinterPaper.Controls.Add(this.rdPrintSetupPaperLandscape);
            resources.ApplyResources(this.grpPrintSetupPrinterPaper, "grpPrintSetupPrinterPaper");
            this.grpPrintSetupPrinterPaper.Name = "grpPrintSetupPrinterPaper";
            this.grpPrintSetupPrinterPaper.TabStop = false;
            // 
            // btnPrintSetup
            // 
            resources.ApplyResources(this.btnPrintSetup, "btnPrintSetup");
            this.btnPrintSetup.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnPrintSetup.Name = "btnPrintSetup";
            this.btnPrintSetup.UseVisualStyleBackColor = true;
            this.btnPrintSetup.Click += new System.EventHandler(this.PrintSetup_Click);
            // 
            // rdPrintSetupPaperPortrait
            // 
            this.rdPrintSetupPaperPortrait.Checked = true;
            this.rdPrintSetupPaperPortrait.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.rdPrintSetupPaperPortrait, "rdPrintSetupPaperPortrait");
            this.rdPrintSetupPaperPortrait.Name = "rdPrintSetupPaperPortrait";
            this.rdPrintSetupPaperPortrait.TabStop = true;
            this.rdPrintSetupPaperPortrait.CheckedChanged += new System.EventHandler(this.PrintSetupPaper_CheckedChanged);
            // 
            // comboPrintSetupPaperSize
            // 
            resources.ApplyResources(this.comboPrintSetupPaperSize, "comboPrintSetupPaperSize");
            this.comboPrintSetupPaperSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPrintSetupPaperSize.Name = "comboPrintSetupPaperSize";
            this.comboPrintSetupPaperSize.Sorted = true;
            this.comboPrintSetupPaperSize.SelectedIndexChanged += new System.EventHandler(this.PrintSetupPaperSize_SelectedIndexChanged);
            // 
            // rdPrintSetupPaperLandscape
            // 
            this.rdPrintSetupPaperLandscape.ForeColor = System.Drawing.SystemColors.ControlText;
            resources.ApplyResources(this.rdPrintSetupPaperLandscape, "rdPrintSetupPaperLandscape");
            this.rdPrintSetupPaperLandscape.Name = "rdPrintSetupPaperLandscape";
            this.rdPrintSetupPaperLandscape.CheckedChanged += new System.EventHandler(this.PrintSetupPaper_CheckedChanged);
            // 
            // tbPageSize
            // 
            this.tbPageSize.Controls.Add(this.cntPageSize);
            resources.ApplyResources(this.tbPageSize, "tbPageSize");
            this.tbPageSize.Name = "tbPageSize";
            this.tbPageSize.UseVisualStyleBackColor = true;
            // 
            // cntPageSize
            // 
            resources.ApplyResources(this.cntPageSize, "cntPageSize");
            this.cntPageSize.ModelContentSize = new System.Drawing.SizeF(0F, 0F);
            this.cntPageSize.Name = "cntPageSize";
            this.cntPageSize.PageLandscape = false;
            pageSize3.DisplayName = "Letter: 8.5 in x 11 in";
            pageSize3.Height = 11F;
            pageSize3.HeightMeasureUnit = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Inch;
            pageSize3.Width = 8.5F;
            pageSize3.WidthMeasureUnit = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Inch;
            this.cntPageSize.PageSize = pageSize3;
            this.cntPageSize.PrinterSettings = ((System.Drawing.Printing.PageSettings)(resources.GetObject("cntPageSize.PrinterSettings")));
            this.cntPageSize.PageSizeChanged += new System.EventHandler(this.PageSize_PageSizeChanged);
            // 
            // tbDrawingScale
            // 
            this.tbDrawingScale.Controls.Add(this.cntDrawingScale);
            resources.ApplyResources(this.tbDrawingScale, "tbDrawingScale");
            this.tbDrawingScale.Name = "tbDrawingScale";
            this.tbDrawingScale.UseVisualStyleBackColor = true;
            // 
            // cntDrawingScale
            // 
            resources.ApplyResources(this.cntDrawingScale, "cntDrawingScale");
            this.cntDrawingScale.MeasureUnits = Syncfusion.Windows.Forms.Diagram.MeasureUnits.Pixel;
            this.cntDrawingScale.Name = "cntDrawingScale";
            pageScale2.DisplayName = "NoScale";
            this.cntDrawingScale.PageScale = pageScale2;
            pageSize4.DisplayName = "Custom";
            this.cntDrawingScale.PageSize = pageSize4;
            this.cntDrawingScale.PageSizeChanged += new System.EventHandler(this.DrawingScale_PageSizeChanged);
            this.cntDrawingScale.PageScaleChanged += new System.EventHandler(this.DrawingScale_PageScaleChanged);
            this.cntDrawingScale.MeasureUnitsChanged += new System.EventHandler(this.DrawingScale_MeasureUnitsChanged);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.Ok_Click);
            // 
            // btnApply
            // 
            resources.ApplyResources(this.btnApply, "btnApply");
            this.btnApply.Name = "btnApply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.Apply_Click);
            // 
            // grpPreview
            // 
            this.grpPreview.Controls.Add(this.lblPreviewDrawingPageValue);
            this.grpPreview.Controls.Add(this.lblPreviewPrinterPageValue);
            this.grpPreview.Controls.Add(this.lblPreviewLine);
            this.grpPreview.Controls.Add(this.lblPreviewPrintZoom);
            this.grpPreview.Controls.Add(this.lblPreviewDrawingPage);
            this.grpPreview.Controls.Add(this.lblPreviewPrintZoomValue);
            this.grpPreview.Controls.Add(this.lblPreviewDrawingPageOrientation);
            this.grpPreview.Controls.Add(this.lblPreviewPrinterPageOrientation);
            this.grpPreview.Controls.Add(this.lblPreviewPrinterPage);
            this.grpPreview.Controls.Add(this.pcPreview);
            resources.ApplyResources(this.grpPreview, "grpPreview");
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.TabStop = false;
            // 
            // lblPreviewDrawingPageValue
            // 
            resources.ApplyResources(this.lblPreviewDrawingPageValue, "lblPreviewDrawingPageValue");
            this.lblPreviewDrawingPageValue.Name = "lblPreviewDrawingPageValue";
            // 
            // lblPreviewPrinterPageValue
            // 
            resources.ApplyResources(this.lblPreviewPrinterPageValue, "lblPreviewPrinterPageValue");
            this.lblPreviewPrinterPageValue.Name = "lblPreviewPrinterPageValue";
            // 
            // lblPreviewLine
            // 
            resources.ApplyResources(this.lblPreviewLine, "lblPreviewLine");
            this.lblPreviewLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblPreviewLine.Name = "lblPreviewLine";
            // 
            // lblPreviewPrintZoom
            // 
            resources.ApplyResources(this.lblPreviewPrintZoom, "lblPreviewPrintZoom");
            this.lblPreviewPrintZoom.Name = "lblPreviewPrintZoom";
            // 
            // lblPreviewDrawingPage
            // 
            resources.ApplyResources(this.lblPreviewDrawingPage, "lblPreviewDrawingPage");
            this.lblPreviewDrawingPage.Name = "lblPreviewDrawingPage";
            // 
            // lblPreviewPrintZoomValue
            // 
            resources.ApplyResources(this.lblPreviewPrintZoomValue, "lblPreviewPrintZoomValue");
            this.lblPreviewPrintZoomValue.Name = "lblPreviewPrintZoomValue";
            // 
            // lblPreviewDrawingPageOrientation
            // 
            resources.ApplyResources(this.lblPreviewDrawingPageOrientation, "lblPreviewDrawingPageOrientation");
            this.lblPreviewDrawingPageOrientation.Name = "lblPreviewDrawingPageOrientation";
            // 
            // lblPreviewPrinterPageOrientation
            // 
            resources.ApplyResources(this.lblPreviewPrinterPageOrientation, "lblPreviewPrinterPageOrientation");
            this.lblPreviewPrinterPageOrientation.Name = "lblPreviewPrinterPageOrientation";
            // 
            // lblPreviewPrinterPage
            // 
            resources.ApplyResources(this.lblPreviewPrinterPage, "lblPreviewPrinterPage");
            this.lblPreviewPrinterPage.Name = "lblPreviewPrinterPage";
            // 
            // pcPreview
            // 
            resources.ApplyResources(this.pcPreview, "pcPreview");
            this.pcPreview.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.pcPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pcPreview.Name = "pcPreview";
            this.pcPreview.TabStop = false;
            // 
            // PageSetupDialog
            // 
            this.AcceptButton = this.btnOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.grpPreview);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tbControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PageSetupDialog";
            this.tbControl.ResumeLayout(false);
            this.tbPrintSetup.ResumeLayout(false);
            this.grpPrintSetupPrintZoom.ResumeLayout(false);
            this.grpPrintSetupPrintZoom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrintSetupZoomFitToBy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPrintSetupZoomFitToAcross)).EndInit();
            this.grpPrintSetupPrinterPaper.ResumeLayout(false);
            this.tbPageSize.ResumeLayout(false);
            this.tbDrawingScale.ResumeLayout(false);
            this.grpPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pcPreview)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        #region Class events
        private void Apply_Click(object sender, EventArgs e)
        {
            btnApply.Enabled = false;
            ApplyChanges();
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            if (btnApply.Enabled)
            {
                ApplyChanges();
            }
        }
		private void PageSizeLandscape_CheckedChanged(object sender, EventArgs e)
        {
          this.rdPrintSetupPaperLandscape.Checked = cntPageSize.rdPageSizeLandscape.Checked;
          this.rdPrintSetupPaperPortrait.Checked = !cntPageSize.rdPageSizeLandscape.Checked;
        }
        private void PageSize_PageSizeChanged(object sender, EventArgs e)
        {
            // update pageScale
            if (!m_bLockUpdate)
            {
                m_bLockUpdate = true;
                this.cntDrawingScale.PageSize = (PageSize)this.PageSize.Clone();
                m_bLockUpdate = false;
            }

            UpdateDrawPreviewInfo();
            OnPropertyChanged();
        }
        private void DrawingScale_PageSizeChanged(object sender, EventArgs e)
        {
            // update pageSize
            if (!m_bLockUpdate)
            {
                m_bLockUpdate = true;
                cntPageSize.PageSize = cntDrawingScale.PageSize;
                m_bLockUpdate = false;
            }

            UpdateDrawPreviewInfo();
            OnPropertyChanged();
        }
        private void DrawingScale_MeasureUnitsChanged(object sender, EventArgs e)
        {
            OnPropertyChanged();
        }
        private void DrawingScale_PageScaleChanged(object sender, EventArgs e)
        {
            OnPropertyChanged();
        }
        #endregion

        #region PrintSetup Tab events
        private void PrintSetupPaperSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            // save changes
            if (this.PrintSettings != null)
            {
                // Print Paper
                this.PrintSettings.PaperSize = (PaperSize)m_hashPaperSize[comboPrintSetupPaperSize.SelectedItem];
                UpdatePrintSettings();
                OnPropertyChanged();
            }
        }
        private void PrintSetupPaper_CheckedChanged(object sender, EventArgs e)
        {
            // save changes
            if (this.PrintSettings != null)
            {
                this.PrintSettings.Landscape = rdPrintSetupPaperLandscape.Checked;
                UpdatePrintSettings();
                OnPropertyChanged();
            }
        }
        private void PrintSetup_Click(object sender, EventArgs e)
        {
            if (this.PrintSettings == null || this.PrintZoom == null)
                return;

            using (PrintSetupDialog dlgPrintSetup = new PrintSetupDialog())
            {
                // Made to make values more user friendly 
                dlgPrintSetup.PageSettings = this.PrintSettings;
                dlgPrintSetup.PrintZoom = this.PrintZoom;

                if (dlgPrintSetup.ShowDialog() == DialogResult.OK)
                {
                    // save changes
                    this.PrintZoom = dlgPrintSetup.PrintZoom;
                    this.PrintSettings = dlgPrintSetup.PageSettings;

                    UpdatePrintSettings();
                }
            }
        }
        private void PrintZoom_CheckedChanged(object sender, EventArgs e)
        {
            // enable/disable sheets fields
            txtPrintSetupZoomFitToAcross.Enabled = rdPrintSetupZoomFitTo.Checked;
            txtPrintSetupZoomFitToBy.Enabled = rdPrintSetupZoomFitTo.Checked;

            comboPrintSetupZoomAdjustTo.Enabled = !rdPrintSetupZoomFitTo.Checked;

            // save changes
            if (this.PrintZoom != null)
            {
                this.PrintZoom.UsePrintingZoom = rdPrintSetupZoomAdjustTo.Checked;
                UpdatePreviewZoomInfo();
                OnPropertyChanged();
            }
        }
        private void PrintZoomAdjustTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strValue = comboPrintSetupZoomAdjustTo.SelectedItem.ToString();

            if (strValue.EndsWith("%"))
                strValue = strValue.Remove(strValue.Length - 1, 1);

            // save changes
            if (this.PrintZoom != null)
            {
                this.PrintZoom.PrintingZoom = int.Parse(strValue);
                UpdatePreviewZoomInfo();
                OnPropertyChanged();
            }
        }
        private void PrintZoomFitTo_TextChanged(object sender, EventArgs e)
        {
            // save changes
            if (this.PrintZoom != null)
            {
                if (sender == txtPrintSetupZoomFitToAcross)
                    this.PrintZoom.SheetsAcross = (int)txtPrintSetupZoomFitToAcross.IntegerValue;
                if (sender == txtPrintSetupZoomFitToBy)
                    this.PrintZoom.SheetsDown = (int)txtPrintSetupZoomFitToBy.IntegerValue;
                UpdatePreviewZoomInfo();
                OnPropertyChanged();
            }
        }
        #endregion

        #region Class Initialize methods
        private void FillSetupCombos()
        {
            // zoom combo
            string[] lstZoom = new string[] { "400%", "200%", "100%", "50%", "25%" };
            FillCombo(comboPrintSetupZoomAdjustTo, lstZoom);

            // printer papers combo
            using (PrintDocument printDoc = new PrintDocument())
            {
                PrinterSettings.PaperSizeCollection paperSizes = printDoc.PrinterSettings.PaperSizes;

                string strPaperName = string.Empty;

                if (m_hashPaperSize == null)
                    m_hashPaperSize = new Hashtable();

                // Get paper sizes supported with current printer and add them to m_hashPaperSize
                foreach (PaperSize paperSize in paperSizes)
                {
                    strPaperName = paperSize.PaperName;
                    comboPrintSetupPaperSize.Items.Add(strPaperName);
                    m_hashPaperSize.Add(strPaperName, paperSize);
                }

                comboPrintSetupPaperSize.SelectedIndex = 0;
            }
        }
        private void FillCombo(ComboBox combo, IList array)
        {
            for (int i = 0, length = array.Count; i < length; i++)
            {
                combo.Items.Add(array[i]);
            }

            // select first item
            combo.SelectedIndex = 0;
        }
        private void InitializePrintPaper()
        {
            if (this.PrintSettings == null)
                return;

            // print paper
            comboPrintSetupPaperSize.SelectedItem = this.PrintSettings.PaperSize.PaperName;

            // print orientation
            rdPrintSetupPaperLandscape.Checked = this.PrintSettings.Landscape;
            rdPrintSetupPaperPortrait.Checked = !this.PrintSettings.Landscape;
        }
        private void InitializePrintZoom()
        {
            if (this.PrintZoom == null)
                return;

            // "adjust to" flag
            rdPrintSetupZoomAdjustTo.Checked = this.PrintZoom.UsePrintingZoom;
            rdPrintSetupZoomFitTo.Checked = !this.PrintZoom.UsePrintingZoom;

            // zoom combo value
            string strZoomAdjTo = string.Format("{0}%", this.PrintZoom.PrintingZoom);
            if (!comboPrintSetupZoomAdjustTo.Items.Contains(strZoomAdjTo))         
                comboPrintSetupZoomAdjustTo.Items.Add(strZoomAdjTo);
            comboPrintSetupZoomAdjustTo.Text = strZoomAdjTo;

            // fit to values
            txtPrintSetupZoomFitToAcross.IntegerValue = this.PrintZoom.SheetsAcross;
            txtPrintSetupZoomFitToBy.IntegerValue = this.PrintZoom.SheetsDown;

            UpdatePreviewZoomInfo();
        }
        private void InitializePageSize()
        {
            PageSize pageSize = this.PageSize;

            PaperStandart[] standarts = PaperStandart.GetStandarts();
            bool bFound = false;

            for (int i = 0, length = standarts.Length; i < length && !bFound; i++)
            {
                PageSize[] sizes = standarts[i].PageSizes;

                for (int j = 0, nLength = sizes.Length; j < nLength && !bFound; j++)
                {
                    if (sizes[j].DisplayName == pageSize.DisplayName)
                    {
                        cntPageSize.rdPageSizePreDefinedSize.Checked = true;
                        cntPageSize.comboPageSizeStandart.SelectedIndex = i;
                        cntPageSize.comboPageSizePaper.SelectedIndex = j;
                        bFound = true;
                    }
                }
            }

            if (!bFound && pageSize.PixelWidth != this.PrintSettings.PaperSize.Width
                || pageSize.PixelHeight != this.PrintSettings.PaperSize.Height)
            {
                cntPageSize.rdPageSizeCustomSize.Checked = true;
            }

            // set orientation
            cntPageSize.rdPageSizeLandscape.Checked = this.PageLandscape;
            float fWidth = pageSize.Width;
            float fHeight = pageSize.Height;
            MeasureUnits mWidth = pageSize.WidthMeasureUnit;
            MeasureUnits mHeight = pageSize.HeightMeasureUnit;

            // set custom size
            if (this.PageLandscape)
            {
                cntPageSize.txtPageSizeCustomHeight.SetValue(fWidth, mWidth);
                cntPageSize.txtPageSizeCustomWidth.SetValue(fHeight, mHeight);
            }
            else
            {
                cntPageSize.txtPageSizeCustomWidth.SetValue(fWidth, mWidth);
                cntPageSize.txtPageSizeCustomHeight.SetValue(fHeight, mHeight);
            }
        }
        #endregion

        #region Class Helper methods
        private void UpdatePreview()
        {
            Size szClientSize = pcPreview.ClientSize;
            Image imgPreview = new Bitmap(szClientSize.Width, szClientSize.Height);

            float marging = 10f;
            RectangleF rcPaperArea = new RectangleF(marging, marging, szClientSize.Width / 2 - marging, szClientSize.Height - marging * 2);
            RectangleF rcTextArea = new RectangleF(rcPaperArea.Right, marging, rcPaperArea.Width - marging, rcPaperArea.Height);

            using (Graphics gfx = Graphics.FromImage(imgPreview))
            {
                PaperSize paperSize = this.PrintSettings.PaperSize;

                // prepare graphics to draw
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                float fPrintWidth = this.PrintSettings.Landscape ? paperSize.Height : paperSize.Width;
                float fPrintHeight = this.PrintSettings.Landscape ? paperSize.Width : paperSize.Height;
                float fPageWidth = this.PageLandscape ? this.PageSize.PixelHeight : this.PageSize.PixelWidth;
                float fPageHeight = this.PageLandscape ? this.PageSize.PixelWidth : this.PageSize.PixelHeight;

                if (fPrintWidth > 0 && fPrintHeight > 0 && fPageWidth > 0 && fPageHeight > 0)
                {
                    float fZoom = this.PrintZoom.PrintingZoom / 100f;

                    if (rdPrintSetupZoomAdjustTo.Checked)
                    {
                        fPrintWidth /= fZoom;
                        fPrintHeight /= fZoom;
                    }
                    else
                    {
                        fPrintWidth /= txtPrintSetupZoomFitToAcross.IntegerValue;
                        fPrintHeight /= txtPrintSetupZoomFitToBy.IntegerValue;
                    }

                    float maxZoomWidth = rcPaperArea.Width / Math.Max(fPrintWidth, fPageWidth);
                    float maxZoomHeight = rcPaperArea.Height / Math.Max(fPrintHeight, fPageHeight);
                    fZoom = Math.Min(maxZoomWidth, maxZoomHeight);

                    RectangleF rcDrawing = new RectangleF(rcPaperArea.X, rcPaperArea.Y, fPageWidth * fZoom, fPageHeight * fZoom);
                    RectangleF rcPrint = new RectangleF(rcPaperArea.X, rcPaperArea.Y, fPrintWidth * fZoom, fPrintHeight * fZoom);

                    HatchStyle hatch = HatchStyle.BackwardDiagonal;
                    HatchBrush brush = new HatchBrush(hatch, Color.Black, Color.White);

                    // 1 - draw print rectangle
                    gfx.FillRectangle(brush, rcPrint);
                    gfx.DrawRectangle(Pens.Black, rcPrint.X, rcPrint.Y, rcPrint.Width, rcPrint.Height);

                    // 2 - draw drawing rectangle
                    gfx.FillRectangle(Brushes.White, rcDrawing);
                    gfx.DrawRectangle(Pens.Black, rcDrawing.X, rcDrawing.Y, rcDrawing.Width, rcDrawing.Height);

                    // 3 - draw print track
                    int nColumnCount = (int)((rcDrawing.Width - 1) / rcPrint.Width);
                    int nRowCount = (int)((rcDrawing.Height - 1) / rcPrint.Height);

                    using (Pen pnDashLine = new Pen(Color.Gray))
                    {
                        pnDashLine.DashStyle = DashStyle.Dash;

                        for (int nColumn = 1; nColumn <= nColumnCount; nColumn++)
                        {
                            gfx.DrawLine(pnDashLine, marging + nColumn * rcPrint.Width, marging, marging + nColumn * rcPrint.Width, marging + rcDrawing.Height);
                        }

                        for (int nRow = 1; nRow <= nRowCount; nRow++)
                        {
                            gfx.DrawLine(pnDashLine, marging, marging + nRow * rcPrint.Height, marging + rcDrawing.Width, marging + nRow * rcPrint.Height);
                        }
                    }

                    // 4 - draw notes
                    marging /= 2;
                    PointF ptStart = new PointF(rcDrawing.Right - marging, rcDrawing.Y + marging);
                    DrawAreaNote(gfx, ptStart, new PointF(rcTextArea.Right - marging, ptStart.Y), "Drawing Page");

                    if (!(rcPrint.Width - rcDrawing.Width < marging * 2 && rcPrint.Width - rcDrawing.Width > -marging * 2) ||
                        !(rcPrint.Height - rcDrawing.Height < marging * 2 && rcPrint.Height - rcDrawing.Height > -marging * 2))
                    {
                        if (marging * 4 < rcPrint.Height || marging * 4 < rcPrint.Width)
                        {
                            ptStart = new PointF(rcPrint.Right - marging, rcPrint.Bottom - marging);
                            DrawAreaNote(gfx, ptStart, new PointF(rcTextArea.Right - marging, ptStart.Y), "Printer Paper");
                        }
                    }
                }
            }

            pcPreview.Image = imgPreview;
        }
        private void DrawAreaNote(Graphics gfx, PointF ptStart, PointF ptEnd, string strNote)
        {
            Color lineColor = Color.Black;

            using (SolidBrush brush = new SolidBrush(lineColor))
            using (Pen pnLine = new Pen(lineColor))
            {
                SizeF szSize = gfx.MeasureString(strNote, this.Font);
                ptEnd.X -= szSize.Width;

                gfx.FillRectangle(brush, Geometry.WidenRect(new RectangleF(ptStart, SizeF.Empty), 2));
                gfx.DrawLine(pnLine, ptStart, ptEnd);

                ptEnd.Y -= szSize.Height / 2;
                float fontSize = (MeasureUnitsConverter.Convert(MeasureUnitsConverter.FromPixelX(gfx.PageScale, MeasureUnits.Point) * (MeasureUnitsConverter.FromPixelX(this.Font.Size,MeasureUnits.Point)), MeasureUnits.Point, MeasureUnits.Inch));
                if (fontSize > (1 / 72f))
                gfx.DrawString(strNote, this.Font, brush, ptEnd);
            }
        }

        private void UpdatePrintSettings()
        {
            //if (cntPageSize.rdPageSizeSameAsPrinterSize.Checked)
                cntPageSize.PrinterSettings = (PageSettings)this.PrintSettings.Clone();

            UpdatePrintPreviewInfo();
        }
        private void UpdatePreviewZoomInfo()
        {
            PrintZoom printZoom = this.PrintZoom;
            string strText;

            // update zoom label
            if (printZoom.UsePrintingZoom)
            {
                strText = (printZoom.PrintingZoom != CommonUsedValues.HUNDRED_PERCENT)
                    ? string.Format("{0} % of normal size", printZoom.PrintingZoom) : "None";
            }
            else
            {
                strText = (printZoom.SheetsAcross == 1 && printZoom.SheetsDown == 1)
                    ? "Fit to exactly 1 sheet" : string.Format("Fit to {0} x {1} sheets", printZoom.SheetsAcross, printZoom.SheetsDown);
            }

            lblPreviewPrintZoomValue.Text = strText;
        }
        private void UpdatePrintPreviewInfo()
        {
            PageSettings printSettings = this.PrintSettings;

            // orientation
            lblPreviewPrinterPageOrientation.Text = printSettings.Landscape ? "(Landscape)" : "(Portrait)";

            // size
            float fWidth = printSettings.Landscape ? printSettings.PaperSize.Height : printSettings.PaperSize.Width;
            float fHeight = printSettings.Landscape ? printSettings.PaperSize.Width : printSettings.PaperSize.Height;
            lblPreviewPrinterPageValue.Text = FormatPageSize(fWidth, MeasureUnits.Pixel, fHeight, MeasureUnits.Pixel);
        }
        private void UpdateDrawPreviewInfo()
        {
            lblPreviewDrawingPageOrientation.Text = cntPageSize.rdPageSizeLandscape.Checked ? "(Landscape)" : "(Portrait)";

            lblPreviewDrawingPageValue.Text = FormatPageSize(
                cntPageSize.txtPageSizeCustomWidth.Value,
                cntPageSize.txtPageSizeCustomWidth.MeasureUnits, 
                cntPageSize.txtPageSizeCustomHeight.Value, 
                cntPageSize.txtPageSizeCustomHeight.MeasureUnits);
        }
        private string FormatPageSize(float width, MeasureUnits widthUnit, float height, MeasureUnits heightUnit)
        {
            string strResult;
            width = (float)Math.Round(width, 1);
            height = (float)Math.Round(height, 1);

            if (widthUnit == heightUnit)
            {
                strResult = string.Format("{0} X {1} {2}", width, height, MeasureUnitsConverter.GetAbbreviation(widthUnit));
            }
            else
            {
                strResult = string.Format("{0:0} {2} X {1:0} {3}", width, height, MeasureUnitsConverter.GetAbbreviation(widthUnit), MeasureUnitsConverter.GetAbbreviation(heightUnit));
            }

            return strResult;
        }
        private void ApplyChanges()
        {
            m_view.Model.BeginUpdate();

            // apply measure units
            MeasureUnits units = (MeasureUnits)cntDrawingScale.comboMeasureUnits.SelectedItem;
            m_view.Model.MeasurementUnits = units;

            // apply page size changes
            if (cntPageSize.rdPageSizeToFitDrawingContent.Checked)
            {
                m_view.Model.SizeToContent = true;
            }
            else
            {
                m_view.Model.SizeToContent = false;
                m_view.Model.DocumentSize = this.PageSize;//ChangeOrientation(this.PageSize, cntPageSize.PageLandscape);
            }

            m_view.Model.DocumentScale = this.PageScale;

            // apply print setting changes
			if (this.cntPageSize.PageLandscape)
                this.PrintSettings.Landscape = true;
            m_view.PageSettings = this.PrintSettings;
            m_view.PrintZoom = this.PrintZoom;

            m_view.Model.EndUpdate();

            m_view.RefreshPageSettings();
        }

        /// <summary>
        /// Change the page size orientation.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="bRotate">if set to <c>true</c> flip width and height value.</param>
        /// <returns>Page size.</returns>
        private PageSize ChangeOrientation(PageSize pageSize, bool bRotate)
        {
            PageSize pgSize = pageSize;

            if (bRotate)
            {
                pgSize = new PageSize(
                    pageSize.DisplayName, 
                    pageSize.Height, 
                    pageSize.HeightMeasureUnit, 
                    pageSize.Width, 
                    pageSize.WidthMeasureUnit);
            }

            return pgSize;
        }
        private void OnPropertyChanged()
        {
            btnApply.Enabled = true;
            UpdatePreview();
        }
        #endregion
    }
}