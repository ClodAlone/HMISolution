#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Tools;

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// The ChartStyleDialogOptions class.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public sealed class ChartStyleDialogOptions
    {
        #region Members
        private bool m_showInteriorTab = true;
        private bool m_showBorderTab = true;
        private bool m_showTextTab = true;
        private bool m_showSymbolTab = true;
        private bool m_showFancyToolTipTab = true;
        private bool m_showShadowTab = true;
        #endregion

        #region Properites
        /// <summary>
        /// Gets or sets a value indicating whether interior tab is visible.
        /// </summary>
        /// <value><c>true</c> if interior tab is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Description("Indicates whether interiors tab is visible")]
        public bool ShowInteriorTab
        {
            get { return m_showInteriorTab; }
            set { m_showInteriorTab = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether border tab is visible.
        /// </summary>
        /// <value><c>true</c> if border tab is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Description("Indicates whether border tab is visible")]
        public bool ShowBorderTab
        {
            get { return m_showBorderTab; }
            set { m_showBorderTab = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether text tab is visible.
        /// </summary>
        /// <value><c>true</c> if text tab is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Description("Indicates whether text tab is visible")]
        public bool ShowTextTab
        {
            get { return m_showTextTab; }
            set { m_showTextTab = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether symbol tab is visible.
        /// </summary>
        /// <value><c>true</c> if symbol tab is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Description("Indicates whether symbol tab is visible")]
        public bool ShowSymbolTab
        {
            get { return m_showSymbolTab; }
            set { m_showSymbolTab = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether fancy tooltips tab is visible.
        /// </summary>
        /// <value><c>true</c> if fancy tooltips tab is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Description("Indicates whether fancy tooltips tab is visible")]
        public bool ShowFancyToolTipsTab
        {
            get { return m_showFancyToolTipTab; }
            set { m_showFancyToolTipTab = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether shadow tab is visible.
        /// </summary>
        /// <value><c>true</c> if shadow tab is visible; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Description("Indicates whether shadow tab is visible")]
        public bool ShowShadowTab
        {
            get { return m_showShadowTab; }
            set { m_showShadowTab = value; }
        }
        #endregion
    }

    /// <summary>
    /// Represents the dialog to edit the series style.
    /// </summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public sealed class SeriesStyleEditorForm : Form
    {
        #region Members
        private ChartSeries m_series = null;
        // Values that will be passed in from the ChartControl:
        private ChartStyleInfo m_seriesStyle;
        private string m_seriesName;
        private BrushInfo m_seriesInterior;
        private bool m_interiorIsReset = false;

        // Temporary objects for editing:
        private BrushInfo shadowInterior;
        private InteriorEditor shadowEditor = new InteriorEditor();
        private OffsetEditor offsetEditor = new OffsetEditor();
        private FontEditor fontEditor = new FontEditor();
        private TextEditor textEditor = new TextEditor();
        private SymbolEditor symbolEditor = new SymbolEditor();
        private InteriorEditor seriesInteriorEditor = new InteriorEditor();

        // Controls for diaplying and editing the ChartStyleInfo:
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private GroupBox groupBox1;
        private ToolTip toolTip1;
        private Label borderColorLabel;
        private IContainer components;
        private Label label2;
        private Label label3;
        private ComboBox enumEditComboBox1;
        private Label label4;
        private NumericUpDown widthNumericUpDown;
        private Button buttonOK;
        private Button buttonCancel;
        private Button buttonBorderColorReset;
        private ColorUIControl borderColorControl;
        private TabPage tabPage3;
        private CheckBox displayShadowCheckBox;
        private PropertyGrid shadowInteriorPropertyGrid;
        private PropertyGrid shadowOffsetPropertyGrid;
        private TabPage tabPage4;
        private PropertyGrid fontPropertyGrid;
        private PropertyGrid textPropertyGrid;
        private CheckBox displayTextCheckBox;
        private ComboBox enumEditComboBox2;
        private Panel panel1;
        private GradientPanel seriesInteriorPanel;
        private Label seriesNameLabel;
        private Label label1;
        private TabPage tabPage5;
        private PropertyGrid seriesInteriorPropertyGrid;
        private System.Windows.Forms.TabPage tbpgToolTip;
        private System.Windows.Forms.PropertyGrid prgdToolTip;
        private PropertyGrid symbolPropertyGrid;
        private Button bttnResetInterior;
        private ChartFancyToolTipInfo toolTipInfo;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the series style.
        /// </summary>
        /// <value>The series style.</value>
        /// <internalonly/>
        public ChartStyleInfo SeriesStyle
        {
            get
            {
                return m_seriesStyle;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesStyleEditorForm"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="seriesInterior">The series interior.</param>
        /// <param name="options">The options.</param>
        /// <internalonly/>
        public SeriesStyleEditorForm(ChartSeries series, string seriesName, BrushInfo seriesInterior, ChartStyleDialogOptions options)
        {
            m_series = series;
            m_seriesStyle = series.Style;
            m_seriesName = seriesName;
            m_seriesInterior = seriesInterior;

            toolTipInfo = series.FancyToolTip;

            //
            // Required for Windows Form Designer support.
            //
            this.InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call.
            //
            this.enumEditComboBox1.DataSource = Enum.GetValues(typeof(DashStyle));
            this.enumEditComboBox2.DataSource = Enum.GetValues(typeof(PenAlignment));

            if (options != null)
            {
                if (!options.ShowInteriorTab)
                {
                    tabControl1.TabPages.Remove(tabPage5);
                }

                if (!options.ShowBorderTab)
                {
                    tabControl1.TabPages.Remove(tabPage1);
                }

                if (!options.ShowTextTab)
                {
                    tabControl1.TabPages.Remove(tabPage2);
                }

                if (!options.ShowSymbolTab)
                {
                    tabControl1.TabPages.Remove(tabPage4);
                }

                if (!options.ShowShadowTab)
                {
                    tabControl1.TabPages.Remove(tabPage3);
                }

                if (!options.ShowFancyToolTipsTab)
                {
                    tabControl1.TabPages.Remove(tbpgToolTip);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SeriesStyleEditorForm"/> class.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="seriesName">Name of the series.</param>
        /// <param name="seriesInterior">The series interior.</param>
        /// <internalonly/>
        public SeriesStyleEditorForm(ChartSeries series, string seriesName, BrushInfo seriesInterior)
            : this(series, seriesName, seriesInterior, null)
        {
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Cleans up any resources that is being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.buttonOK = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.seriesInteriorPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.enumEditComboBox1 = new ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.enumEditComboBox2 = new ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.widthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonBorderColorReset = new System.Windows.Forms.Button();
            this.borderColorLabel = new System.Windows.Forms.Label();
            this.borderColorControl = new Syncfusion.Windows.Forms.ColorUIControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.displayTextCheckBox = new System.Windows.Forms.CheckBox();
            this.textPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.fontPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.shadowOffsetPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.displayShadowCheckBox = new System.Windows.Forms.CheckBox();
            this.shadowInteriorPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.symbolPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tbpgToolTip = new System.Windows.Forms.TabPage();
            this.prgdToolTip = new System.Windows.Forms.PropertyGrid();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.seriesInteriorPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
            this.seriesNameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.bttnResetInterior = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tbpgToolTip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seriesInteriorPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(272, 368);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(88, 24);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tbpgToolTip);
            this.tabControl1.Location = new System.Drawing.Point(16, 72);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(448, 280);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.bttnResetInterior);
            this.tabPage5.Controls.Add(this.seriesInteriorPropertyGrid);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Size = new System.Drawing.Size(440, 254);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Interior";
            // 
            // seriesInteriorPropertyGrid
            // 
            this.seriesInteriorPropertyGrid.HelpVisible = false;
            this.seriesInteriorPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.seriesInteriorPropertyGrid.Location = new System.Drawing.Point(24, 32);
            this.seriesInteriorPropertyGrid.Name = "seriesInteriorPropertyGrid";
            this.seriesInteriorPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.seriesInteriorPropertyGrid.Size = new System.Drawing.Size(256, 180);
            this.seriesInteriorPropertyGrid.TabIndex = 3;
            this.seriesInteriorPropertyGrid.ToolbarVisible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.enumEditComboBox1);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.enumEditComboBox2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.widthNumericUpDown);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(440, 254);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Border";
            // 
            // enumEditComboBox1
            // 
            this.enumEditComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.enumEditComboBox1.Location = new System.Drawing.Point(328, 117);
            this.enumEditComboBox1.Name = "enumEditComboBox1";
            this.enumEditComboBox1.Size = new System.Drawing.Size(104, 21);
            this.enumEditComboBox1.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(240, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 5;
            this.label4.Text = "Dash Style";
            // 
            // enumEditComboBox2
            // 
            this.enumEditComboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.enumEditComboBox2.Location = new System.Drawing.Point(328, 72);
            this.enumEditComboBox2.Name = "enumEditComboBox2";
            this.enumEditComboBox2.Size = new System.Drawing.Size(104, 21);
            this.enumEditComboBox2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(240, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "Alignment";
            // 
            // widthNumericUpDown
            // 
            this.widthNumericUpDown.Location = new System.Drawing.Point(328, 24);
            this.widthNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.widthNumericUpDown.Name = "widthNumericUpDown";
            this.widthNumericUpDown.Size = new System.Drawing.Size(104, 20);
            this.widthNumericUpDown.TabIndex = 2;
            this.widthNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(240, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Width";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonBorderColorReset);
            this.groupBox1.Controls.Add(this.borderColorLabel);
            this.groupBox1.Controls.Add(this.borderColorControl);
            this.groupBox1.Location = new System.Drawing.Point(16, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 232);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Color";
            // 
            // buttonBorderColorReset
            // 
            this.buttonBorderColorReset.Location = new System.Drawing.Point(16, 200);
            this.buttonBorderColorReset.Name = "buttonBorderColorReset";
            this.buttonBorderColorReset.Size = new System.Drawing.Size(184, 24);
            this.buttonBorderColorReset.TabIndex = 3;
            this.buttonBorderColorReset.Text = "Reset Color";
            this.buttonBorderColorReset.Click += new System.EventHandler(this.buttonBorderColorReset_Click);
            // 
            // borderColorLabel
            // 
            this.borderColorLabel.Location = new System.Drawing.Point(16, 24);
            this.borderColorLabel.Name = "borderColorLabel";
            this.borderColorLabel.Size = new System.Drawing.Size(176, 24);
            this.borderColorLabel.TabIndex = 2;
            // 
            // borderColorControl
            // 
            this.borderColorControl.Location = new System.Drawing.Point(16, 64);
            this.borderColorControl.Name = "borderColorControl";
            this.borderColorControl.Size = new System.Drawing.Size(184, 120);
            this.borderColorControl.TabIndex = 1;
            this.borderColorControl.Text = "colorUIControl1";
            this.borderColorControl.UserTabName = "User Colors";
            this.borderColorControl.ColorSelected += new System.EventHandler(this.borderColorControl_ColorSelected);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.displayTextCheckBox);
            this.tabPage2.Controls.Add(this.textPropertyGrid);
            this.tabPage2.Controls.Add(this.fontPropertyGrid);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(440, 254);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Text";
            // 
            // displayTextCheckBox
            // 
            this.displayTextCheckBox.Location = new System.Drawing.Point(16, 8);
            this.displayTextCheckBox.Name = "displayTextCheckBox";
            this.displayTextCheckBox.Size = new System.Drawing.Size(104, 24);
            this.displayTextCheckBox.TabIndex = 5;
            this.displayTextCheckBox.Text = "Display Text";
            // 
            // textPropertyGrid
            // 
            this.textPropertyGrid.HelpVisible = false;
            this.textPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.textPropertyGrid.Location = new System.Drawing.Point(248, 40);
            this.textPropertyGrid.Name = "textPropertyGrid";
            this.textPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.textPropertyGrid.Size = new System.Drawing.Size(184, 88);
            this.textPropertyGrid.TabIndex = 4;
            this.textPropertyGrid.ToolbarVisible = false;
            // 
            // fontPropertyGrid
            // 
            this.fontPropertyGrid.HelpVisible = false;
            this.fontPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.fontPropertyGrid.Location = new System.Drawing.Point(16, 40);
            this.fontPropertyGrid.Name = "fontPropertyGrid";
            this.fontPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.fontPropertyGrid.Size = new System.Drawing.Size(224, 192);
            this.fontPropertyGrid.TabIndex = 3;
            this.fontPropertyGrid.ToolbarVisible = false;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.shadowOffsetPropertyGrid);
            this.tabPage3.Controls.Add(this.displayShadowCheckBox);
            this.tabPage3.Controls.Add(this.shadowInteriorPropertyGrid);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(440, 254);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Shadow";
            // 
            // shadowOffsetPropertyGrid
            // 
            this.shadowOffsetPropertyGrid.HelpVisible = false;
            this.shadowOffsetPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.shadowOffsetPropertyGrid.Location = new System.Drawing.Point(288, 72);
            this.shadowOffsetPropertyGrid.Name = "shadowOffsetPropertyGrid";
            this.shadowOffsetPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.shadowOffsetPropertyGrid.Size = new System.Drawing.Size(136, 64);
            this.shadowOffsetPropertyGrid.TabIndex = 3;
            this.shadowOffsetPropertyGrid.ToolbarVisible = false;
            // 
            // displayShadowCheckBox
            // 
            this.displayShadowCheckBox.Location = new System.Drawing.Point(16, 16);
            this.displayShadowCheckBox.Name = "displayShadowCheckBox";
            this.displayShadowCheckBox.Size = new System.Drawing.Size(104, 24);
            this.displayShadowCheckBox.TabIndex = 1;
            this.displayShadowCheckBox.Text = "Display Shadow";
            // 
            // shadowInteriorPropertyGrid
            // 
            this.shadowInteriorPropertyGrid.HelpVisible = false;
            this.shadowInteriorPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.shadowInteriorPropertyGrid.Location = new System.Drawing.Point(16, 72);
            this.shadowInteriorPropertyGrid.Name = "shadowInteriorPropertyGrid";
            this.shadowInteriorPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.shadowInteriorPropertyGrid.Size = new System.Drawing.Size(256, 136);
            this.shadowInteriorPropertyGrid.TabIndex = 2;
            this.shadowInteriorPropertyGrid.ToolbarVisible = false;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.symbolPropertyGrid);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(440, 254);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Symbol";
            // 
            // symbolPropertyGrid
            // 
            this.symbolPropertyGrid.HelpVisible = false;
            this.symbolPropertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.symbolPropertyGrid.Location = new System.Drawing.Point(24, 32);
            this.symbolPropertyGrid.Name = "symbolPropertyGrid";
            this.symbolPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.symbolPropertyGrid.Size = new System.Drawing.Size(312, 192);
            this.symbolPropertyGrid.TabIndex = 4;
            this.symbolPropertyGrid.ToolbarVisible = false;
            // 
            // tbpgToolTip
            // 
            this.tbpgToolTip.Controls.Add(this.prgdToolTip);
            this.tbpgToolTip.Location = new System.Drawing.Point(4, 22);
            this.tbpgToolTip.Name = "tbpgToolTip";
            this.tbpgToolTip.Size = new System.Drawing.Size(440, 254);
            this.tbpgToolTip.TabIndex = 5;
            this.tbpgToolTip.Text = "Fancy ToolTip";
            // 
            // prgdToolTip
            // 
            this.prgdToolTip.HelpVisible = false;
            this.prgdToolTip.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.prgdToolTip.Location = new System.Drawing.Point(24, 32);
            this.prgdToolTip.Name = "prgdToolTip";
            this.prgdToolTip.Size = new System.Drawing.Size(256, 192);
            this.prgdToolTip.TabIndex = 0;
            this.prgdToolTip.ToolbarVisible = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(376, 368);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(88, 24);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.seriesInteriorPanel);
            this.panel1.Controls.Add(this.seriesNameLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(16, 16);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(448, 48);
            this.panel1.TabIndex = 7;
            // 
            // seriesInteriorPanel
            // 
            this.seriesInteriorPanel.BorderColor = System.Drawing.Color.Black;
            this.seriesInteriorPanel.Location = new System.Drawing.Point(328, 16);
            this.seriesInteriorPanel.Name = "seriesInteriorPanel";
            this.seriesInteriorPanel.Size = new System.Drawing.Size(104, 24);
            this.seriesInteriorPanel.TabIndex = 9;
            // 
            // seriesNameLabel
            // 
            this.seriesNameLabel.Location = new System.Drawing.Point(112, 20);
            this.seriesNameLabel.Name = "seriesNameLabel";
            this.seriesNameLabel.Size = new System.Drawing.Size(176, 16);
            this.seriesNameLabel.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 7;
            this.label1.Text = "Series";
            // 
            // bttnResetInterior
            // 
            this.bttnResetInterior.Location = new System.Drawing.Point(24, 218);
            this.bttnResetInterior.Name = "bttnResetInterior";
            this.bttnResetInterior.Size = new System.Drawing.Size(151, 23);
            this.bttnResetInterior.TabIndex = 4;
            this.bttnResetInterior.Text = "Reset Interior";
            this.bttnResetInterior.Click += new System.EventHandler(this.bttnResetInterior_Click);
            // 
            // SeriesStyleEditorForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(482, 398);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SeriesStyleEditorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Chart Series Style";
            this.Load += new System.EventHandler(this.SeriesStyleEditorForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.widthNumericUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tbpgToolTip.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.seriesInteriorPanel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        /// <summary>
        /// Handles the Load event of the SeriesStyleEditorForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SeriesStyleEditorForm_Load(object sender, EventArgs e)
        {
            TransferValuesFromStyleInfoToControls();
        }

        /// <summary>
        /// Transfers the values from style info to controls.
        /// </summary>
        private void TransferValuesFromStyleInfoToControls()
        {
            // ********* Series Name *******
            this.seriesNameLabel.Text = m_seriesName;

            // ********* Series Interior *******
            if (m_seriesInterior != null)
                this.seriesInteriorPanel.BackgroundColor = m_seriesInterior;
            else
                this.seriesInteriorPanel.Visible = false;

            // **********Border**************

            // Color
            this.borderColorLabel.BackColor = m_seriesStyle.Border.Color;
            this.borderColorControl.SelectedColor = m_seriesStyle.Border.Color;

            // Alignment
            this.enumEditComboBox2.SelectedItem = m_seriesStyle.Border.Alignment;

            // DashStyle
            this.enumEditComboBox1.SelectedItem = m_seriesStyle.Border.DashStyle;

            // DashPattern
            //this.enumEditComboBox2.FloatArray = seriesStyle.Border.DashPattern;

            // Width
            this.widthNumericUpDown.Value = (decimal)m_seriesStyle.Border.Width;

            // *********Shadow**************

            // Display Shadow
            this.displayShadowCheckBox.Checked = m_seriesStyle.DisplayShadow;

            // Shadow Interior
            this.shadowInterior = m_seriesStyle.ShadowInterior.Clone();
            this.shadowEditor.Interior = this.shadowInterior;

            this.shadowInteriorPropertyGrid.SelectedObject = this.shadowEditor;
            this.shadowInteriorPropertyGrid.ExpandAllGridItems();

            // Shadow offset
            this.offsetEditor.Offset = m_seriesStyle.ShadowOffset;
            this.shadowOffsetPropertyGrid.SelectedObject = this.offsetEditor;
            this.shadowOffsetPropertyGrid.ExpandAllGridItems();

            // ***********Text***********

            // Display Text
            this.displayTextCheckBox.Checked = m_seriesStyle.DisplayText;

            // Font
            this.fontEditor.Font = m_seriesStyle.Font;
            this.fontPropertyGrid.SelectedObject = this.fontEditor;
            this.fontPropertyGrid.ExpandAllGridItems();

            // Text
            this.textEditor.Text = m_seriesStyle.Text;
            this.textEditor.TextColor = m_seriesStyle.TextColor;
            this.textEditor.TextFormat = m_seriesStyle.TextFormat;
            this.textEditor.TextOffset = m_seriesStyle.TextOffset;
            this.textEditor.TextOrientation = m_seriesStyle.TextOrientation;
            this.textPropertyGrid.SelectedObject = this.textEditor;
            this.textPropertyGrid.ExpandAllGridItems();

            // *************Symbol*************
            this.symbolEditor.Symbol = m_seriesStyle.Symbol;
            this.symbolPropertyGrid.SelectedObject = this.symbolEditor;
            this.symbolPropertyGrid.ExpandAllGridItems();

            // **********Interior***************

            // Shadow Interior
            this.seriesInteriorEditor.Interior = m_seriesInterior;

            this.seriesInteriorPropertyGrid.SelectedObject = this.seriesInteriorEditor;
            this.seriesInteriorPropertyGrid.ExpandAllGridItems();

            #region FancyToolTip
            ChartFancyToolTipInfo ttInfo = new ChartFancyToolTipInfo();
            ttInfo.Read(toolTipInfo);
            prgdToolTip.SelectedObject = ttInfo;
            #endregion
        }

        /// <summary>
        /// Transfers the values from controls to style info.
        /// </summary>
        private void TransferValuesFromControlsToStyleInfo()
        {
            #region FancyToolTip
            ChartFancyToolTipInfo ttInfo = prgdToolTip.SelectedObject as ChartFancyToolTipInfo;
            toolTipInfo.Read(ttInfo);
            #endregion

            // **********Border**************

            // Color
            m_seriesStyle.Border.Color = this.borderColorControl.SelectedColor;

            // Alignment
            m_seriesStyle.Border.Alignment = (PenAlignment)this.enumEditComboBox2.SelectedItem;

            // DashStyle
            m_seriesStyle.Border.DashStyle = (DashStyle)this.enumEditComboBox1.SelectedItem;

            // Width
            m_seriesStyle.Border.Width = (int)this.widthNumericUpDown.Value;

            // *********Shadow**************

            // Display Shadow
            m_seriesStyle.DisplayShadow = this.displayShadowCheckBox.Checked;

            // Shadow Interior
            m_seriesStyle.ShadowInterior = this.shadowEditor.Interior;

            // Shadow offset
            m_seriesStyle.ShadowOffset = this.offsetEditor.Offset;

            // ***********Text***********

            // Display Text
            m_seriesStyle.DisplayText = this.displayTextCheckBox.Checked;

            // Font
            m_seriesStyle.Font = this.fontEditor.Font;

            // Text
            m_seriesStyle.Text = this.textEditor.Text;
            m_seriesStyle.TextColor = this.textEditor.TextColor;
            m_seriesStyle.TextFormat = this.textEditor.TextFormat;
            m_seriesStyle.TextOffset = this.textEditor.TextOffset;
            m_seriesStyle.TextOrientation = this.textEditor.TextOrientation;

            // *************Symbol*************
            m_seriesStyle.Symbol = this.symbolEditor.Symbol;

            // **************Interior*************
            if (m_seriesInterior != this.seriesInteriorEditor.Interior)
            {
                m_seriesStyle.Interior = this.seriesInteriorEditor.Interior;
            }
            else if (m_interiorIsReset)
            {
                m_seriesStyle.Interior = null;
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonOK control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonOK_Click(object sender, EventArgs e)
        {
            TransferValuesFromControlsToStyleInfo();
        }

        /// <summary>
        /// Handles the ColorSelected event of the borderColorControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void borderColorControl_ColorSelected(object sender, EventArgs e)
        {
            this.borderColorLabel.BackColor = this.borderColorControl.SelectedColor;
        }

        /// <summary>
        /// Handles the Click event of the buttonBorderColorReset control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void buttonBorderColorReset_Click(object sender, EventArgs e)
        {
            this.borderColorLabel.BackColor = m_seriesStyle.Border.Color;
            this.borderColorControl.SelectedColor = m_seriesStyle.Border.Color;
        }

        /// <summary>
        /// Handles the Click event of the bttnResetInterior control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void bttnResetInterior_Click(object sender, EventArgs e)
        {
            m_interiorIsReset = true;
            m_seriesInterior = new BrushInfo(m_series.BackColor);
            seriesInteriorEditor.Interior = m_seriesInterior;

            this.seriesInteriorPropertyGrid.Refresh();
        }
        #endregion
    }
}
