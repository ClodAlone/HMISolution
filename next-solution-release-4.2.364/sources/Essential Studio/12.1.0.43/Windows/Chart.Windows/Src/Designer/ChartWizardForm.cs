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
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Text;

using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart.Design;
using Syncfusion.ComponentModel;
using Syncfusion.Documentation;
#if SyncfusionFramework2_0
using System.ComponentModel.Design.Data;
using System.Resources;
#endif

namespace Syncfusion.Windows.Forms.Chart
{
	/// <summary>
	/// Provides the wizard dialog to configure the chart control.
	/// </summary>
	///<internalonly/>
	[DocumentationExclude()]
	public class ChartWizardForm : Office2007Form
	{
		#region Helper classes
		/// <summary>
		/// 
		/// </summary>
		private class NamedObject
		{
			#region Members
			private string m_name;
			private object m_tag;
			#endregion

			#region Properties
			/// <summary>
			/// 
			/// </summary>
			public string Name
			{
				get
				{
					return m_name;
				}
				set
				{
					m_name = value;
				}
			}
			/// <summary>
			/// 
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		 , Browsable(false)]
			public object Tag
			{
				get
				{
					return m_tag;
				}
			}
			#endregion

			#region Constructor
			/// <summary>
			/// 
			/// </summary>
			public NamedObject()
			{
				m_name = "";
				m_tag = null;
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="name"></param>
			/// <param name="tag"></param>
			public NamedObject(string name, object tag)
			{
				m_name = name;
				m_tag = tag;
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return Name;
			}
			#endregion
		}
		/// <summary>
		/// 
		/// </summary>
		private class DesignChartControl : ChartControl, IChartAreaHost
		{
			#region Constructor
			/// <summary>
			/// 
			/// </summary>
			public DesignChartControl()
				: base( false )
			{ 
			}
			#endregion

			#region Implementation
			/// <summary>
			/// 
			/// </summary>
			bool IChartAreaHost.IsDesignTime
			{
				get { return true; }
			}
			#endregion
		}
		#endregion

		#region Constants
		private readonly static Size c_thumbTypeSize = new Size(210, 140);
		private const int c_randomSeriesCount = 2;
		private const int c_randomSeriesPointCount = 5;
		private const int c_randomSeriesMaxY1 = 400;
		private const int c_randomSeriesMaxY2 = 100;
		private const int c_randomSeriesMaxY3 = 300;
		private const int c_randomSeriesMaxY4 = 400;

		private const int c_tabStartIndex = 0;
		private const int c_tabEndTabIndex = 6;

		private const int c_tabChartTypeIndex = 0;
		private const int c_tabSeriesIndex = 1;
		private const int c_tabAppearanceIndex = 2;
		private const int c_tabAxesIndex = 3;
		private const int c_tabPoinsIndex = 4;
		private const int c_tabToolBarIndex = 5;
		private const int c_tabLegendIndex = 6;

		private const string c_newName = "[new...]";
		private const string c_newOleDataAdapterName = "[new OleDataAdapter...]";
		private const string c_newSqlDataAdapterName = "[new SqlDataAdapter...]";
		private const string c_noneName = "[none]";
		private const string c_allName = "[all]";

		private static readonly NamedObject c_noneNamedObject = new NamedObject(c_noneName, null);
		private static readonly NamedObject c_newOleDataAdapterNamedObject = new NamedObject(c_newOleDataAdapterName, null);
		private static readonly NamedObject c_newSqlDataAdapterNamedObject = new NamedObject(c_newSqlDataAdapterName, null);
		private static readonly NamedObject c_newNamedObject = new NamedObject(c_newName, null);
		private static readonly NamedObject c_allNamedObject = new NamedObject(c_allName, null);

#if SyncfusionFramework2_0
		private const string c_newBindingSourceName = "[new BindingSource...]";
		private static readonly NamedObject c_newBindingSourceNamedObject = new NamedObject(c_newBindingSourceName, null);
#endif

		private static Random c_random = new Random();
		#endregion

		#region Members

		#region	Internal members
		private ChartControl m_chart = null;
		private int m_currentTabIndex = 0;
		private ImageList m_seriesTypeThumbs = null;
		private string m_curentDataMember = "";
		private object m_curentDataSource = null;
		private ChartSeriesType m_chartSeriesType = ChartSeriesType.Column;
		#endregion

		private ImageButton bttnCancel;
		private ImageButton bttnOk;
		private System.Windows.Forms.Label lbTitle;
		private HighlightImageLabel bttnNext;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnApply;
		private HighlightImageLabel bttnPrevious;
		private Syncfusion.Windows.Forms.Chart.ChartControl chcrWizardChart;
		private ChartTabControl tbcrChartToolBar;
		private System.Windows.Forms.TabPage tbpgChartToolBar;
		private ChartGroupBox grbxEditChartToolBar;
		private ChartGroupBox grbxChartToolBarButtonStyle;
		private System.Windows.Forms.Label lbChartToolBarButtonHeight;
		private System.Windows.Forms.Label lbChartToolBarButtonWidth;
		private System.Windows.Forms.NumericUpDown nmupChartToolBarButtonHeight;
		private System.Windows.Forms.NumericUpDown nmupChartToolBarButtonWidth;
		private System.Windows.Forms.Label lbButtonStyle;
		private System.Windows.Forms.ComboBox cbbxChartToolBarButtonStyle;
		private System.Windows.Forms.Label lbChartToolBarBackColor;
		private System.Windows.Forms.CheckBox chbxChartToolBarVisible;
		private System.Windows.Forms.TabPage tbpgChartToolbarBorder;
		private ChartGroupBox grbxChartToolBarBorder;
		private System.Windows.Forms.Label lbChartToolBarBorderColor;
		private System.Windows.Forms.Label lbChartToolBarBorderWidth;
		private System.Windows.Forms.NumericUpDown nmudChartToolBarBorderWidth;
		private System.Windows.Forms.Label lbChartToolBarBorderStyle;
		private System.Windows.Forms.ComboBox cbbxChartToolBarBorderStyle;
		private System.Windows.Forms.CheckBox chbxChartToolBarShowBorder;
		private System.Windows.Forms.CheckBox chbxIs3D;
		private System.Windows.Forms.Label lbChartTypeGroups;
		private ChartComboBox cbbxChartTypeGroups;
		private ChartTabControl tbcrChartLegend;
		private ChartGroupBox grbxEditChartLegend;
		private System.Windows.Forms.Label lbCgartLegendOrientation;
		private System.Windows.Forms.Label lbChartLegendAlignment;
		private System.Windows.Forms.Label lbCgartLegendPosition;
		private System.Windows.Forms.ComboBox cbbxChartLegendOrientation;
		private System.Windows.Forms.ComboBox cbbxChartLegendAlignment;
		private System.Windows.Forms.ComboBox cbbxChartLegendPosition;
		private System.Windows.Forms.CheckBox chbxChartLegendVisible;
		private System.Windows.Forms.TabPage tbpgChartLegendBorderStyle;
		private ChartGroupBox grbxBorderStyle;
		private System.Windows.Forms.Label lbChartLegendBorderWidth;
		private System.Windows.Forms.NumericUpDown nmudChartLegendBorderWidth;
		private System.Windows.Forms.ComboBox cbbxChartLegendBorderDashStyle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox chbxChartLegendShowBorder;
		private ChartTabControl tbcrSeries;
		private System.Windows.Forms.TabPage tbpgDataSource;
		private System.Windows.Forms.ComboBox cbbxDataSources;
		private System.Windows.Forms.DataGrid dtgdDataSource;
		private System.Windows.Forms.TabPage tbpgSeriesData;
		private ChartGroupBox grbxChaerSeriesData;
		private System.Windows.Forms.Label lbChartSeriesYValue;
		private System.Windows.Forms.Label lbChartSeriesXValue;
		private System.Windows.Forms.ComboBox cbbxChartSeriesYValue;
		private System.Windows.Forms.ComboBox cbbxChartSeriesXValue;
		private System.Windows.Forms.Label lbSeriesType;
		private System.Windows.Forms.TabPage tbpgAddPointToSeries;
		private ChartGroupBox grbcChartAddSeriesCodeGenerator;
		private System.Windows.Forms.Button bttnEditPoints;
		private System.Windows.Forms.ListBox lsbxChartPoints;
		private System.Windows.Forms.ComboBox cbbxChartSeries;
		private ChartTabControl tbcrAppearance;
		private System.Windows.Forms.TabPage tbpgColorPalette;
		private System.Windows.Forms.ListBox lsbxColorPalette;
		private System.Windows.Forms.TabPage tbpgChartBorder;
		private ChartGroupBox grbxEditChartBorder;
		private System.Windows.Forms.Label lbChartAreaBackColor;
		private System.Windows.Forms.Label lbChartBackColor;
		private System.Windows.Forms.Label lbChartAreaBorderColor;
		private System.Windows.Forms.Label lbChartAreaBorderStyle;
		private System.Windows.Forms.ComboBox cbbxChartBorderStyle;
		private System.Windows.Forms.TabPage tbpgTitle;
		private ChartGroupBox grbxEditChartTitle;
		private System.Windows.Forms.Label lbChartTitleAlignment;
		private System.Windows.Forms.ComboBox cbbxTitleAlignment;
		private System.Windows.Forms.Label lbChartTitleColor;
		private System.Windows.Forms.Label lbChartTitlePosition;
		private System.Windows.Forms.ComboBox cbbxChartTitlePosition;
		private System.Windows.Forms.Label lbChartTitleText;
		private System.Windows.Forms.TextBox txtbxChartTitleText;
		private ChartTabControl tbcrAxes;
		private System.Windows.Forms.TabPage tbpgPrimaryXAxis;
		private ChartGroupBox grbxEditXAxis;
		private System.Windows.Forms.Button bttnXAxisEditLabels;
		private System.Windows.Forms.Label lbXAxisValueType;
		private System.Windows.Forms.ComboBox cbbxXAxisValueType;
		private System.Windows.Forms.Label lbXAxisTitle;
		private System.Windows.Forms.TextBox txtbxXAxisTitle;
		private System.Windows.Forms.CheckBox chbxXAxisGridLine;
		private System.Windows.Forms.TabPage tbpgPrimaryYAxis;
		private ChartGroupBox grbxEditYAxis;
		private System.Windows.Forms.Button bttnYAxisEditLabels;
		private System.Windows.Forms.Label lbYAxisValueType;
		private System.Windows.Forms.ComboBox cbbxYaxisValueType;
		private System.Windows.Forms.Label lbYAxisTitle;
		private System.Windows.Forms.TextBox txtbxYAxisTitle;
		private System.Windows.Forms.CheckBox chbxYAxisGridLine;
		private System.Windows.Forms.ListView lsvwSeriesTypes;
		private CheckBox checkAutoRun;
		private CheckBox checkBox1;
		private Syncfusion.Windows.Forms.Tools.GradientPanel gradientPanel1;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnTabToolBar;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnTabLegend;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnTabAxes;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnTabAppearance;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnTabSeries;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnTabChartStyle;
		private Syncfusion.Windows.Forms.Tools.GradientPanel gradientPanel2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ComboBox comboBox1;
		private Syncfusion.Windows.Forms.Chart.ImageButton bttnTabPoints;
		private Syncfusion.Windows.Forms.Chart.ChartGroupBox chartGroupBox2;
		private Syncfusion.Windows.Forms.Tools.GradientPanel pnPoints;
		private Syncfusion.Windows.Forms.Chart.ColorBox clrbxPointsLabelsColor;
		private System.Windows.Forms.Label label2;
		private Syncfusion.Windows.Forms.Chart.ColorBox colorBox1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NumericUpDown nmudPointsLabelsRotate;
		private System.Windows.Forms.Label label5;
		private Syncfusion.Windows.Forms.Chart.FontBox fntbxPointsLabelsFont;
		private System.Windows.Forms.ComboBox cbbxPointLabelsApplySeries;
		private System.Windows.Forms.CheckBox chbxPointsLabelsShow;
		private Syncfusion.Windows.Forms.Chart.ChartTextOrientationBox orbxPointsLabelsAlignment;
		private Syncfusion.Windows.Forms.Chart.ColorBox clrbxToolBarBackColor;
		private Syncfusion.Windows.Forms.Chart.BrushInfoBox brushInfoBox2;
		private Syncfusion.Windows.Forms.Chart.BrushInfoBox brushInfoBox3;
		private Syncfusion.Windows.Forms.Chart.ColorBox colorBox2;
		private Syncfusion.Windows.Forms.Chart.StringAlignmentBox sabxXAxisTitleAlignment;
		private System.Windows.Forms.ComboBox cbbxXAxisIntersectAction;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox chbxXAxisInversed;
		private System.Windows.Forms.CheckBox chbxXAxisOpposed;
		private Syncfusion.Windows.Forms.Chart.StringAlignmentBox sabxYAxisTitleAlignment;
		private System.Windows.Forms.CheckBox chbxYAxisInversed;
		private System.Windows.Forms.CheckBox chbxYAxisOpposed;
		private Syncfusion.Windows.Forms.Chart.ColorBox clrbxToolBarBorderColor;
		private System.Windows.Forms.Button bttnToolBarEditItems;
		private Syncfusion.Windows.Forms.Chart.ColorBox clrbxLegendBorderColor;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TabPage tbpgChartLegend;
		private Syncfusion.Windows.Forms.Chart.BrushInfoBox bibxLegendBackInterior;
		private System.Windows.Forms.Button bttnAddSeries;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtbxSeriesName;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox txtbxSeriesName2;
		private System.Windows.Forms.ComboBox cbbxSeriesPointsType;
		private System.Windows.Forms.ComboBox cbbxSeriesDataType;
		private System.Windows.Forms.TextBox txtbxXAxisFormat;
		private System.Windows.Forms.TextBox txtbxYAxisFormat;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label lblDoesntUseXAxis;
		private System.Windows.Forms.Label lblDoesntUseYAxis;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label8;

		private ISite m_chartControlSite = null;
		#endregion

		#region Properties
		/// <summary>
		/// 
		/// </summary>
		private ImageList SeriesTypeThumbs
		{
			get
			{
				if (m_seriesTypeThumbs == null)
				{
					m_seriesTypeThumbs = new ImageList();
					m_seriesTypeThumbs.ColorDepth = ColorDepth.Depth32Bit;
					InitializeThumbs();
				}

				return m_seriesTypeThumbs;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		private bool IsDesigTime
		{
			get
			{
				return m_chartControlSite != null;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartWizardForm"/> class.
		/// </summary>
		/// <param name="site">The site.</param>
		public ChartWizardForm(ISite site)
		{
			m_chartControlSite = site;

			InitializeComponent();
			UserInitializeComponent();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ChartWizardForm));
			this.fntbxPointsLabelsFont = new Syncfusion.Windows.Forms.Chart.FontBox();
			this.clrbxPointsLabelsColor = new Syncfusion.Windows.Forms.Chart.ColorBox();
			this.bttnTabToolBar = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnTabLegend = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnTabAxes = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnTabAppearance = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnTabSeries = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnTabChartStyle = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnApply = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnNext = new Syncfusion.Windows.Forms.Chart.HighlightImageLabel();
			this.bttnPrevious = new Syncfusion.Windows.Forms.Chart.HighlightImageLabel();
			this.bttnOk = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.bttnCancel = new Syncfusion.Windows.Forms.Chart.ImageButton();
			this.lbTitle = new System.Windows.Forms.Label();
			this.chcrWizardChart = new DesignChartControl();
			this.chbxIs3D = new System.Windows.Forms.CheckBox();
			this.lbChartTypeGroups = new System.Windows.Forms.Label();
			this.cbbxChartTypeGroups = new Syncfusion.Windows.Forms.Chart.ChartComboBox();
			this.gradientPanel1 = new Syncfusion.Windows.Forms.Tools.GradientPanel();
			this.lsvwSeriesTypes = new System.Windows.Forms.ListView();
			this.tbcrSeries = new Syncfusion.Windows.Forms.Chart.ChartTabControl();
			this.tbpgAddPointToSeries = new System.Windows.Forms.TabPage();
			this.grbcChartAddSeriesCodeGenerator = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.label10 = new System.Windows.Forms.Label();
			this.txtbxSeriesName = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.cbbxSeriesPointsType = new System.Windows.Forms.ComboBox();
			this.bttnEditPoints = new System.Windows.Forms.Button();
			this.lsbxChartPoints = new System.Windows.Forms.ListBox();
			this.bttnAddSeries = new System.Windows.Forms.Button();
			this.cbbxChartSeries = new System.Windows.Forms.ComboBox();
			this.button2 = new System.Windows.Forms.Button();
			this.tbpgDataSource = new System.Windows.Forms.TabPage();
			this.cbbxDataSources = new System.Windows.Forms.ComboBox();
			this.dtgdDataSource = new System.Windows.Forms.DataGrid();
			this.tbpgSeriesData = new System.Windows.Forms.TabPage();
			this.grbxChaerSeriesData = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.txtbxSeriesName2 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.lbChartSeriesYValue = new System.Windows.Forms.Label();
			this.lbChartSeriesXValue = new System.Windows.Forms.Label();
			this.cbbxChartSeriesYValue = new System.Windows.Forms.ComboBox();
			this.cbbxChartSeriesXValue = new System.Windows.Forms.ComboBox();
			this.lbSeriesType = new System.Windows.Forms.Label();
			this.cbbxSeriesDataType = new System.Windows.Forms.ComboBox();
			this.button3 = new System.Windows.Forms.Button();
			this.tbcrAppearance = new Syncfusion.Windows.Forms.Chart.ChartTabControl();
			this.tbpgColorPalette = new System.Windows.Forms.TabPage();
			this.lsbxColorPalette = new System.Windows.Forms.ListBox();
			this.tbpgChartBorder = new System.Windows.Forms.TabPage();
			this.grbxEditChartBorder = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.lbChartAreaBackColor = new System.Windows.Forms.Label();
			this.lbChartBackColor = new System.Windows.Forms.Label();
			this.lbChartAreaBorderColor = new System.Windows.Forms.Label();
			this.lbChartAreaBorderStyle = new System.Windows.Forms.Label();
			this.cbbxChartBorderStyle = new System.Windows.Forms.ComboBox();
			this.brushInfoBox2 = new Syncfusion.Windows.Forms.Chart.BrushInfoBox();
			this.brushInfoBox3 = new Syncfusion.Windows.Forms.Chart.BrushInfoBox();
			this.colorBox2 = new Syncfusion.Windows.Forms.Chart.ColorBox();
			this.tbpgTitle = new System.Windows.Forms.TabPage();
			this.grbxEditChartTitle = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.lbChartTitleAlignment = new System.Windows.Forms.Label();
			this.cbbxTitleAlignment = new System.Windows.Forms.ComboBox();
			this.lbChartTitleColor = new System.Windows.Forms.Label();
			this.lbChartTitlePosition = new System.Windows.Forms.Label();
			this.cbbxChartTitlePosition = new System.Windows.Forms.ComboBox();
			this.lbChartTitleText = new System.Windows.Forms.Label();
			this.txtbxChartTitleText = new System.Windows.Forms.TextBox();
			this.colorBox1 = new Syncfusion.Windows.Forms.Chart.ColorBox();
			this.tbcrChartLegend = new Syncfusion.Windows.Forms.Chart.ChartTabControl();
			this.tbpgChartLegend = new System.Windows.Forms.TabPage();
			this.grbxEditChartLegend = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.lbCgartLegendOrientation = new System.Windows.Forms.Label();
			this.lbChartLegendAlignment = new System.Windows.Forms.Label();
			this.lbCgartLegendPosition = new System.Windows.Forms.Label();
			this.cbbxChartLegendOrientation = new System.Windows.Forms.ComboBox();
			this.cbbxChartLegendAlignment = new System.Windows.Forms.ComboBox();
			this.cbbxChartLegendPosition = new System.Windows.Forms.ComboBox();
			this.chbxChartLegendVisible = new System.Windows.Forms.CheckBox();
			this.bibxLegendBackInterior = new Syncfusion.Windows.Forms.Chart.BrushInfoBox();
			this.tbpgChartLegendBorderStyle = new System.Windows.Forms.TabPage();
			this.grbxBorderStyle = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.lbChartLegendBorderWidth = new System.Windows.Forms.Label();
			this.nmudChartLegendBorderWidth = new System.Windows.Forms.NumericUpDown();
			this.cbbxChartLegendBorderDashStyle = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chbxChartLegendShowBorder = new System.Windows.Forms.CheckBox();
			this.clrbxLegendBorderColor = new Syncfusion.Windows.Forms.Chart.ColorBox();
			this.label14 = new System.Windows.Forms.Label();
			this.tbcrChartToolBar = new Syncfusion.Windows.Forms.Chart.ChartTabControl();
			this.tbpgChartToolBar = new System.Windows.Forms.TabPage();
			this.grbxEditChartToolBar = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.bttnToolBarEditItems = new System.Windows.Forms.Button();
			this.lbChartToolBarBackColor = new System.Windows.Forms.Label();
			this.chbxChartToolBarVisible = new System.Windows.Forms.CheckBox();
			this.grbxChartToolBarButtonStyle = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.lbChartToolBarButtonHeight = new System.Windows.Forms.Label();
			this.lbChartToolBarButtonWidth = new System.Windows.Forms.Label();
			this.nmupChartToolBarButtonHeight = new System.Windows.Forms.NumericUpDown();
			this.nmupChartToolBarButtonWidth = new System.Windows.Forms.NumericUpDown();
			this.lbButtonStyle = new System.Windows.Forms.Label();
			this.cbbxChartToolBarButtonStyle = new System.Windows.Forms.ComboBox();
			this.clrbxToolBarBackColor = new Syncfusion.Windows.Forms.Chart.ColorBox();
			this.tbpgChartToolbarBorder = new System.Windows.Forms.TabPage();
			this.grbxChartToolBarBorder = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.lbChartToolBarBorderColor = new System.Windows.Forms.Label();
			this.lbChartToolBarBorderWidth = new System.Windows.Forms.Label();
			this.nmudChartToolBarBorderWidth = new System.Windows.Forms.NumericUpDown();
			this.lbChartToolBarBorderStyle = new System.Windows.Forms.Label();
			this.cbbxChartToolBarBorderStyle = new System.Windows.Forms.ComboBox();
			this.chbxChartToolBarShowBorder = new System.Windows.Forms.CheckBox();
			this.clrbxToolBarBorderColor = new Syncfusion.Windows.Forms.Chart.ColorBox();
			this.tbcrAxes = new Syncfusion.Windows.Forms.Chart.ChartTabControl();
			this.tbpgPrimaryXAxis = new System.Windows.Forms.TabPage();
			this.lblDoesntUseXAxis = new System.Windows.Forms.Label();
			this.grbxEditXAxis = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.txtbxXAxisFormat = new System.Windows.Forms.TextBox();
			this.chbxXAxisOpposed = new System.Windows.Forms.CheckBox();
			this.chbxXAxisInversed = new System.Windows.Forms.CheckBox();
			this.bttnXAxisEditLabels = new System.Windows.Forms.Button();
			this.lbXAxisValueType = new System.Windows.Forms.Label();
			this.cbbxXAxisValueType = new System.Windows.Forms.ComboBox();
			this.lbXAxisTitle = new System.Windows.Forms.Label();
			this.txtbxXAxisTitle = new System.Windows.Forms.TextBox();
			this.chbxXAxisGridLine = new System.Windows.Forms.CheckBox();
			this.sabxXAxisTitleAlignment = new Syncfusion.Windows.Forms.Chart.StringAlignmentBox();
			this.cbbxXAxisIntersectAction = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.tbpgPrimaryYAxis = new System.Windows.Forms.TabPage();
			this.lblDoesntUseYAxis = new System.Windows.Forms.Label();
			this.grbxEditYAxis = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.txtbxYAxisTitle = new System.Windows.Forms.TextBox();
			this.txtbxYAxisFormat = new System.Windows.Forms.TextBox();
			this.chbxYAxisOpposed = new System.Windows.Forms.CheckBox();
			this.chbxYAxisInversed = new System.Windows.Forms.CheckBox();
			this.sabxYAxisTitleAlignment = new Syncfusion.Windows.Forms.Chart.StringAlignmentBox();
			this.bttnYAxisEditLabels = new System.Windows.Forms.Button();
			this.lbYAxisValueType = new System.Windows.Forms.Label();
			this.cbbxYaxisValueType = new System.Windows.Forms.ComboBox();
			this.lbYAxisTitle = new System.Windows.Forms.Label();
			this.chbxYAxisGridLine = new System.Windows.Forms.CheckBox();
			this.label12 = new System.Windows.Forms.Label();
			this.checkAutoRun = new System.Windows.Forms.CheckBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.gradientPanel2 = new Syncfusion.Windows.Forms.Tools.GradientPanel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pnPoints = new Syncfusion.Windows.Forms.Tools.GradientPanel();
			this.chartGroupBox2 = new Syncfusion.Windows.Forms.Chart.ChartGroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.cbbxPointLabelsApplySeries = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.chbxPointsLabelsShow = new System.Windows.Forms.CheckBox();
			this.orbxPointsLabelsAlignment = new Syncfusion.Windows.Forms.Chart.ChartTextOrientationBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.nmudPointsLabelsRotate = new System.Windows.Forms.NumericUpDown();
			this.bttnTabPoints = new Syncfusion.Windows.Forms.Chart.ImageButton();
			((System.ComponentModel.ISupportInitialize)(this.gradientPanel1)).BeginInit();
			this.gradientPanel1.SuspendLayout();
			this.tbcrSeries.SuspendLayout();
			this.tbpgAddPointToSeries.SuspendLayout();
			this.grbcChartAddSeriesCodeGenerator.SuspendLayout();
			this.tbpgDataSource.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dtgdDataSource)).BeginInit();
			this.tbpgSeriesData.SuspendLayout();
			this.grbxChaerSeriesData.SuspendLayout();
			this.tbcrAppearance.SuspendLayout();
			this.tbpgColorPalette.SuspendLayout();
			this.tbpgChartBorder.SuspendLayout();
			this.grbxEditChartBorder.SuspendLayout();
			this.tbpgTitle.SuspendLayout();
			this.grbxEditChartTitle.SuspendLayout();
			this.tbcrChartLegend.SuspendLayout();
			this.tbpgChartLegend.SuspendLayout();
			this.grbxEditChartLegend.SuspendLayout();
			this.tbpgChartLegendBorderStyle.SuspendLayout();
			this.grbxBorderStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nmudChartLegendBorderWidth)).BeginInit();
			this.tbcrChartToolBar.SuspendLayout();
			this.tbpgChartToolBar.SuspendLayout();
			this.grbxEditChartToolBar.SuspendLayout();
			this.grbxChartToolBarButtonStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nmupChartToolBarButtonHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nmupChartToolBarButtonWidth)).BeginInit();
			this.tbpgChartToolbarBorder.SuspendLayout();
			this.grbxChartToolBarBorder.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nmudChartToolBarBorderWidth)).BeginInit();
			this.tbcrAxes.SuspendLayout();
			this.tbpgPrimaryXAxis.SuspendLayout();
			this.grbxEditXAxis.SuspendLayout();
			this.tbpgPrimaryYAxis.SuspendLayout();
			this.grbxEditYAxis.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gradientPanel2)).BeginInit();
			this.gradientPanel2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnPoints)).BeginInit();
			this.pnPoints.SuspendLayout();
			this.chartGroupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.nmudPointsLabelsRotate)).BeginInit();
			this.SuspendLayout();
			// 
			// fntbxPointsLabelsFont
			// 
			this.fntbxPointsLabelsFont.Location = new System.Drawing.Point(8, 56);
			this.fntbxPointsLabelsFont.Name = "fntbxPointsLabelsFont";
			this.fntbxPointsLabelsFont.Size = new System.Drawing.Size(152, 24);
			this.fntbxPointsLabelsFont.TabIndex = 24;
			this.fntbxPointsLabelsFont.SelectedFontChanged += new System.EventHandler(this.OnPointsLabelsChanged);
			// 
			// clrbxPointsLabelsColor
			// 
			this.clrbxPointsLabelsColor.Location = new System.Drawing.Point(8, 104);
			this.clrbxPointsLabelsColor.Name = "clrbxPointsLabelsColor";
			this.clrbxPointsLabelsColor.Size = new System.Drawing.Size(152, 21);
			this.clrbxPointsLabelsColor.TabIndex = 23;
			this.clrbxPointsLabelsColor.ColorChanged += new System.EventHandler(this.OnPointsLabelsChanged);
			// 
			// bttnTabToolBar
			// 
			this.bttnTabToolBar.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnTabToolBar.Location = new System.Drawing.Point(32, 304);
			this.bttnTabToolBar.Name = "bttnTabToolBar";
			this.bttnTabToolBar.NormalImage = ((System.Drawing.Image)(resources.GetObject("bttnTabToolBar.NormalImage")));
			this.bttnTabToolBar.SelectedImage = ((System.Drawing.Image)(resources.GetObject("bttnTabToolBar.SelectedImage")));
			this.bttnTabToolBar.Size = new System.Drawing.Size(120, 24);
			this.bttnTabToolBar.TabIndex = 0;
			this.bttnTabToolBar.Text = "ToolBar";
			this.bttnTabToolBar.Click += new System.EventHandler(this.TabButtonsClick);
			// 
			// bttnTabLegend
			// 
			this.bttnTabLegend.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnTabLegend.Location = new System.Drawing.Point(32, 336);
			this.bttnTabLegend.Name = "bttnTabLegend";
			this.bttnTabLegend.NormalImage = ((System.Drawing.Image)(resources.GetObject("bttnTabLegend.NormalImage")));
			this.bttnTabLegend.SelectedImage = ((System.Drawing.Image)(resources.GetObject("bttnTabLegend.SelectedImage")));
			this.bttnTabLegend.Size = new System.Drawing.Size(120, 24);
			this.bttnTabLegend.TabIndex = 0;
			this.bttnTabLegend.Text = "Legend";
			this.bttnTabLegend.Click += new System.EventHandler(this.TabButtonsClick);
			// 
			// bttnTabAxes
			// 
			this.bttnTabAxes.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnTabAxes.Location = new System.Drawing.Point(32, 240);
			this.bttnTabAxes.Name = "bttnTabAxes";
			this.bttnTabAxes.NormalImage = ((System.Drawing.Image)(resources.GetObject("bttnTabAxes.NormalImage")));
			this.bttnTabAxes.SelectedImage = ((System.Drawing.Image)(resources.GetObject("bttnTabAxes.SelectedImage")));
			this.bttnTabAxes.Size = new System.Drawing.Size(120, 24);
			this.bttnTabAxes.TabIndex = 0;
			this.bttnTabAxes.Text = "Axes";
			this.bttnTabAxes.Click += new System.EventHandler(this.TabButtonsClick);
			// 
			// bttnTabAppearance
			// 
			this.bttnTabAppearance.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnTabAppearance.Location = new System.Drawing.Point(32, 208);
			this.bttnTabAppearance.Name = "bttnTabAppearance";
			this.bttnTabAppearance.NormalImage = ((System.Drawing.Image)(resources.GetObject("bttnTabAppearance.NormalImage")));
			this.bttnTabAppearance.SelectedImage = ((System.Drawing.Image)(resources.GetObject("bttnTabAppearance.SelectedImage")));
			this.bttnTabAppearance.Size = new System.Drawing.Size(120, 24);
			this.bttnTabAppearance.TabIndex = 0;
			this.bttnTabAppearance.Text = "Appearance";
			this.bttnTabAppearance.Click += new System.EventHandler(this.TabButtonsClick);
			// 
			// bttnTabSeries
			// 
			this.bttnTabSeries.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnTabSeries.Location = new System.Drawing.Point(32, 176);
			this.bttnTabSeries.Name = "bttnTabSeries";
			this.bttnTabSeries.NormalImage = ((System.Drawing.Image)(resources.GetObject("bttnTabSeries.NormalImage")));
			this.bttnTabSeries.SelectedImage = ((System.Drawing.Image)(resources.GetObject("bttnTabSeries.SelectedImage")));
			this.bttnTabSeries.Size = new System.Drawing.Size(120, 24);
			this.bttnTabSeries.TabIndex = 0;
			this.bttnTabSeries.Text = "Series";
			this.bttnTabSeries.Click += new System.EventHandler(this.TabButtonsClick);
			// 
			// bttnTabChartStyle
			// 
			this.bttnTabChartStyle.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnTabChartStyle.Location = new System.Drawing.Point(32, 144);
			this.bttnTabChartStyle.Name = "bttnTabChartStyle";
			this.bttnTabChartStyle.NormalImage = ((System.Drawing.Image)(resources.GetObject("bttnTabChartStyle.NormalImage")));
			this.bttnTabChartStyle.SelectedImage = ((System.Drawing.Image)(resources.GetObject("bttnTabChartStyle.SelectedImage")));
			this.bttnTabChartStyle.Size = new System.Drawing.Size(120, 24);
			this.bttnTabChartStyle.TabIndex = 0;
			this.bttnTabChartStyle.Text = "Chart type";
			this.bttnTabChartStyle.Click += new System.EventHandler(this.TabButtonsClick);
			// 
			// bttnApply
			// 
			this.bttnApply.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bttnApply.Location = new System.Drawing.Point(184, 448);
			this.bttnApply.Name = "bttnApply";
			this.bttnApply.Size = new System.Drawing.Size(95, 24);
			this.bttnApply.TabIndex = 4;
			this.bttnApply.Text = "Apply";
			this.bttnApply.Click += new System.EventHandler(this.bttnApply_Click);
			// 
			// bttnNext
			// 
			this.bttnNext.BackColor = System.Drawing.Color.Transparent;
			this.bttnNext.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.bttnNext.Font = new System.Drawing.Font("Times New Roman", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.bttnNext.ForeColor = System.Drawing.SystemColors.Window;
			this.bttnNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bttnNext.Location = new System.Drawing.Point(656, 448);
			this.bttnNext.Name = "bttnNext";
			this.bttnNext.Size = new System.Drawing.Size(64, 24);
			this.bttnNext.TabIndex = 3;
			this.bttnNext.Text = "Next";
			this.bttnNext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.bttnNext.Click += new System.EventHandler(this.bttnNext_Click);
			// 
			// bttnPrevious
			// 
			this.bttnPrevious.BackColor = System.Drawing.Color.Transparent;
			this.bttnPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnPrevious.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.bttnPrevious.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.bttnPrevious.ForeColor = System.Drawing.SystemColors.Window;
			this.bttnPrevious.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.bttnPrevious.Location = new System.Drawing.Point(592, 448);
			this.bttnPrevious.Name = "bttnPrevious";
			this.bttnPrevious.Size = new System.Drawing.Size(64, 24);
			this.bttnPrevious.TabIndex = 2;
			this.bttnPrevious.Text = "Prev";
			this.bttnPrevious.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.bttnPrevious.Click += new System.EventHandler(this.bttnPreview_Click);
			// 
			// bttnOk
			// 
			this.bttnOk.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.bttnOk.Location = new System.Drawing.Point(288, 448);
			this.bttnOk.Name = "bttnOk";
			this.bttnOk.Size = new System.Drawing.Size(96, 24);
			this.bttnOk.TabIndex = 1;
			this.bttnOk.Text = "Finish";
			this.bttnOk.Click += new System.EventHandler(this.bttnOk_Click);
			// 
			// bttnCancel
			// 
			this.bttnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bttnCancel.Location = new System.Drawing.Point(392, 448);
			this.bttnCancel.Name = "bttnCancel";
			this.bttnCancel.Size = new System.Drawing.Size(96, 24);
			this.bttnCancel.TabIndex = 0;
			this.bttnCancel.Text = "Cancel";
			this.bttnCancel.Click += new EventHandler(bttnCancel_Click);
			// 
			// lbTitle
			// 
			this.lbTitle.BackColor = System.Drawing.Color.Transparent;
			this.lbTitle.Location = new System.Drawing.Point(176, 96);
			this.lbTitle.Name = "lbTitle";
			this.lbTitle.Size = new System.Drawing.Size(552, 32);
			this.lbTitle.TabIndex = 5;
			this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chcrWizardChart
			// 
			this.chcrWizardChart.BackInterior = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.Color.FromArgb(((System.Byte)(203)), ((System.Byte)(209)), ((System.Byte)(226))), System.Drawing.Color.FromArgb(((System.Byte)(218)), ((System.Byte)(234)), ((System.Byte)(255))));
			// 
			// chcrWizardChart.Legend
			// 
			this.chcrWizardChart.Legend.BackInterior = new Syncfusion.Drawing.BrushInfo(Syncfusion.Drawing.GradientStyle.Vertical, System.Drawing.Color.FromArgb(((System.Byte)(203)), ((System.Byte)(209)), ((System.Byte)(226))), System.Drawing.Color.FromArgb(((System.Byte)(218)), ((System.Byte)(234)), ((System.Byte)(255))));
			this.chcrWizardChart.Legend.Location = new System.Drawing.Point(0, 0);
			this.chcrWizardChart.Legend.TabIndex = 1;
			this.chcrWizardChart.Legend.Visible = false;
			this.chcrWizardChart.Location = new System.Drawing.Point(352, 168);
			this.chcrWizardChart.Name = "chcrWizardChart";
			this.chcrWizardChart.ShowLegend = false;
			this.chcrWizardChart.Size = new System.Drawing.Size(368, 264);
			this.chcrWizardChart.TabIndex = 0;
			this.chcrWizardChart.Text = "ChartControl";
			// 
			// chbxIs3D
			// 
			this.chbxIs3D.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxIs3D.Location = new System.Drawing.Point(456, 16);
			this.chbxIs3D.Name = "chbxIs3D";
			this.chbxIs3D.Size = new System.Drawing.Size(72, 16);
			this.chbxIs3D.TabIndex = 2;
			this.chbxIs3D.Text = "3D Style";
			this.chbxIs3D.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.chbxIs3D.CheckedChanged += new System.EventHandler(this.chbxIs3D_CheckedChanged);
			// 
			// lbChartTypeGroups
			// 
			this.lbChartTypeGroups.Location = new System.Drawing.Point(24, 8);
			this.lbChartTypeGroups.Name = "lbChartTypeGroups";
			this.lbChartTypeGroups.Size = new System.Drawing.Size(96, 24);
			this.lbChartTypeGroups.TabIndex = 1;
			this.lbChartTypeGroups.Text = "Chart type groups ";
			this.lbChartTypeGroups.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cbbxChartTypeGroups
			// 
			this.cbbxChartTypeGroups.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartTypeGroups.Items.AddRange(new object[] {
																														 "Line",
																														 "Area",
																														 "Column",
																														 "Bar",
																														 "Bubble & Scatter",
																														 "Financial",
																														 "Pie, Funnel, Pyramid",
																														 "Polar & Radar",
																														 "Other"});
			this.cbbxChartTypeGroups.Location = new System.Drawing.Point(128, 8);
			this.cbbxChartTypeGroups.Name = "cbbxChartTypeGroups";
			this.cbbxChartTypeGroups.Size = new System.Drawing.Size(182, 21);
			this.cbbxChartTypeGroups.TabIndex = 0;
			this.cbbxChartTypeGroups.SelectedIndexChanged += new System.EventHandler(this.cbbxChartTypeGroups_SelectedIndexChanged);
			// 
			// gradientPanel1
			// 
			this.gradientPanel1.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(117)), ((System.Byte)(151)), ((System.Byte)(197)));
			this.gradientPanel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gradientPanel1.Controls.Add(this.lsvwSeriesTypes);
			this.gradientPanel1.Location = new System.Drawing.Point(8, 32);
			this.gradientPanel1.Name = "gradientPanel1";
			this.gradientPanel1.Size = new System.Drawing.Size(536, 264);
			this.gradientPanel1.TabIndex = 1;
			// 
			// lsvwSeriesTypes
			// 
			this.lsvwSeriesTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.lsvwSeriesTypes.Cursor = System.Windows.Forms.Cursors.Hand;
			this.lsvwSeriesTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lsvwSeriesTypes.Location = new System.Drawing.Point(0, 0);
			this.lsvwSeriesTypes.Name = "lsvwSeriesTypes";
			this.lsvwSeriesTypes.Size = new System.Drawing.Size(534, 262);
			this.lsvwSeriesTypes.TabIndex = 0;
			this.lsvwSeriesTypes.SelectedIndexChanged += new System.EventHandler(this.lsvwSeriesTypes_SelectedIndexChanged);
            this.lsvwSeriesTypes.MouseMove += new MouseEventHandler(lsvwSeriesTypes_MouseMove);
			// 
			// tbcrSeries
			// 
			this.tbcrSeries.Controls.Add(this.tbpgAddPointToSeries);
			this.tbcrSeries.Controls.Add(this.tbpgDataSource);
			this.tbcrSeries.Controls.Add(this.tbpgSeriesData);
			this.tbcrSeries.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbcrSeries.Location = new System.Drawing.Point(0, 0);
			this.tbcrSeries.Name = "tbcrSeries";
			this.tbcrSeries.SelectedIndex = 0;
			this.tbcrSeries.Size = new System.Drawing.Size(553, 306);
			this.tbcrSeries.TabIndex = 17;
			this.tbcrSeries.VisibleChanged += new System.EventHandler(this.tbcrSeries_VisibleChanged);
			this.tbcrSeries.SelectedIndexChanged += new System.EventHandler(this.tbcrSeries_SelectedIndexChanged);
			// 
			// tbpgAddPointToSeries
			// 
			this.tbpgAddPointToSeries.Controls.Add(this.grbcChartAddSeriesCodeGenerator);
			this.tbpgAddPointToSeries.Location = new System.Drawing.Point(4, 25);
			this.tbpgAddPointToSeries.Name = "tbpgAddPointToSeries";
			this.tbpgAddPointToSeries.Size = new System.Drawing.Size(545, 277);
			this.tbpgAddPointToSeries.TabIndex = 2;
			this.tbpgAddPointToSeries.Text = "Add points to series";
			this.tbpgAddPointToSeries.Visible = false;
			// 
			// grbcChartAddSeriesCodeGenerator
			// 
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.label10);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.txtbxSeriesName);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.label9);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.cbbxSeriesPointsType);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.bttnEditPoints);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.lsbxChartPoints);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.bttnAddSeries);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.cbbxChartSeries);
			this.grbcChartAddSeriesCodeGenerator.Controls.Add(this.button2);
			this.grbcChartAddSeriesCodeGenerator.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbcChartAddSeriesCodeGenerator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbcChartAddSeriesCodeGenerator.Location = new System.Drawing.Point(0, 0);
			this.grbcChartAddSeriesCodeGenerator.Name = "grbcChartAddSeriesCodeGenerator";
			this.grbcChartAddSeriesCodeGenerator.Size = new System.Drawing.Size(168, 277);
			this.grbcChartAddSeriesCodeGenerator.TabIndex = 0;
			this.grbcChartAddSeriesCodeGenerator.TabStop = false;
			this.grbcChartAddSeriesCodeGenerator.Text = "Add Points";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 120);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(96, 16);
			this.label10.TabIndex = 23;
			this.label10.Text = "Series Name";
			// 
			// txtbxSeriesName
			// 
			this.txtbxSeriesName.Location = new System.Drawing.Point(8, 136);
			this.txtbxSeriesName.Name = "txtbxSeriesName";
			this.txtbxSeriesName.Size = new System.Drawing.Size(152, 20);
			this.txtbxSeriesName.TabIndex = 22;
			this.txtbxSeriesName.Text = "";
			this.txtbxSeriesName.TextChanged += new System.EventHandler(this.txtbxSeriesName_TextChanged);
			// 
			// label9
			// 
			this.label9.BackColor = System.Drawing.Color.White;
			this.label9.Location = new System.Drawing.Point(8, 80);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(144, 16);
			this.label9.TabIndex = 21;
			this.label9.Text = "Type";
			// 
			// cbbxSeriesType
			// 
			this.cbbxSeriesPointsType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxSeriesPointsType.Location = new System.Drawing.Point(8, 96);
			this.cbbxSeriesPointsType.Name = "cbbxSeriesType";
			this.cbbxSeriesPointsType.Size = new System.Drawing.Size(152, 21);
			this.cbbxSeriesPointsType.TabIndex = 20;
			this.cbbxSeriesPointsType.SelectedIndexChanged += new System.EventHandler(this.cbbxSeriesPointsType_SelectedIndexChanged);
			// 
			// bttnEditPoints
			// 
			this.bttnEditPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.bttnEditPoints.BackColor = System.Drawing.SystemColors.Control;
			this.bttnEditPoints.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.bttnEditPoints.Location = new System.Drawing.Point(8, 241);
			this.bttnEditPoints.Name = "bttnEditPoints";
			this.bttnEditPoints.Size = new System.Drawing.Size(150, 24);
			this.bttnEditPoints.TabIndex = 19;
			this.bttnEditPoints.Text = "Edit points...";
			this.bttnEditPoints.Click += new System.EventHandler(this.bttnEditPoints_Click);
			// 
			// lsbxChartPoints
			// 
			this.lsbxChartPoints.Location = new System.Drawing.Point(8, 160);
			this.lsbxChartPoints.Name = "lsbxChartPoints";
			this.lsbxChartPoints.Size = new System.Drawing.Size(152, 69);
			this.lsbxChartPoints.TabIndex = 18;
			// 
			// bttnAddSeries
			// 
			this.bttnAddSeries.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.bttnAddSeries.Location = new System.Drawing.Point(8, 48);
			this.bttnAddSeries.Name = "bttnAddSeries";
			this.bttnAddSeries.Size = new System.Drawing.Size(72, 24);
			this.bttnAddSeries.TabIndex = 17;
			this.bttnAddSeries.Text = "Add";
			this.bttnAddSeries.Click += new System.EventHandler(this.OnButtonAddSeriesClick);
			// 
			// cbbxChartSeries
			// 
			this.cbbxChartSeries.DisplayMember = "Name";
			this.cbbxChartSeries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartSeries.Location = new System.Drawing.Point(8, 24);
			this.cbbxChartSeries.Name = "cbbxChartSeries";
			this.cbbxChartSeries.Size = new System.Drawing.Size(152, 21);
			this.cbbxChartSeries.TabIndex = 16;
			this.cbbxChartSeries.SelectedIndexChanged += new System.EventHandler(this.cbbxChartSeries_SelectedIndexChanged);
			// 
			// button2
			// 
			this.button2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button2.Location = new System.Drawing.Point(88, 48);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(72, 24);
			this.button2.TabIndex = 17;
			this.button2.Text = "Remove";
			this.button2.Click += new System.EventHandler(this.OnButtonRemoveSeriesClick);
			// 
			// tbpgDataSource
			// 
			this.tbpgDataSource.Controls.Add(this.cbbxDataSources);
			this.tbpgDataSource.Controls.Add(this.dtgdDataSource);
			this.tbpgDataSource.Location = new System.Drawing.Point(4, 25);
			this.tbpgDataSource.Name = "tbpgDataSource";
			this.tbpgDataSource.Size = new System.Drawing.Size(545, 275);
			this.tbpgDataSource.TabIndex = 0;
			this.tbpgDataSource.Text = "Data Source";
			// 
			// cbbxDataSources
			// 
			this.cbbxDataSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxDataSources.Location = new System.Drawing.Point(8, 16);
			this.cbbxDataSources.Name = "cbbxDataSources";
			this.cbbxDataSources.Size = new System.Drawing.Size(216, 21);
			this.cbbxDataSources.TabIndex = 1;
			this.cbbxDataSources.SelectedIndexChanged += new System.EventHandler(this.cbbxDataSources_SelectedIndexChanged);
			// 
			// dtgdDataSource
			// 
			this.dtgdDataSource.AlternatingBackColor = System.Drawing.Color.Silver;
			this.dtgdDataSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.dtgdDataSource.BackColor = System.Drawing.Color.White;
			this.dtgdDataSource.CaptionBackColor = System.Drawing.Color.DarkOrange;
			this.dtgdDataSource.CaptionFont = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.dtgdDataSource.CaptionForeColor = System.Drawing.Color.White;
			this.dtgdDataSource.DataMember = "";
			this.dtgdDataSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.dtgdDataSource.ForeColor = System.Drawing.Color.Black;
			this.dtgdDataSource.GridLineColor = System.Drawing.Color.Silver;
			this.dtgdDataSource.HeaderBackColor = System.Drawing.Color.Silver;
			this.dtgdDataSource.HeaderFont = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.dtgdDataSource.HeaderForeColor = System.Drawing.Color.Black;
			this.dtgdDataSource.LinkColor = System.Drawing.Color.Maroon;
			this.dtgdDataSource.Location = new System.Drawing.Point(8, 40);
			this.dtgdDataSource.Name = "dtgdDataSource";
			this.dtgdDataSource.ParentRowsBackColor = System.Drawing.Color.Silver;
			this.dtgdDataSource.ParentRowsForeColor = System.Drawing.Color.Black;
			this.dtgdDataSource.SelectionBackColor = System.Drawing.Color.Maroon;
			this.dtgdDataSource.SelectionForeColor = System.Drawing.Color.White;
			this.dtgdDataSource.Size = new System.Drawing.Size(528, 228);
			this.dtgdDataSource.TabIndex = 0;
			this.dtgdDataSource.Navigate += new System.Windows.Forms.NavigateEventHandler(this.dtgdDataSource_Navigate);
			// 
			// tbpgSeriesData
			// 
			this.tbpgSeriesData.Controls.Add(this.grbxChaerSeriesData);
			this.tbpgSeriesData.Location = new System.Drawing.Point(4, 25);
			this.tbpgSeriesData.Name = "tbpgSeriesData";
			this.tbpgSeriesData.Size = new System.Drawing.Size(545, 275);
			this.tbpgSeriesData.TabIndex = 1;
			this.tbpgSeriesData.Text = "Series Data";
			this.tbpgSeriesData.Visible = false;
			// 
			// grbxChaerSeriesData
			// 
			this.grbxChaerSeriesData.Controls.Add(this.label11);
			this.grbxChaerSeriesData.Controls.Add(this.txtbxSeriesName2);
			this.grbxChaerSeriesData.Controls.Add(this.button1);
			this.grbxChaerSeriesData.Controls.Add(this.comboBox1);
			this.grbxChaerSeriesData.Controls.Add(this.lbChartSeriesYValue);
			this.grbxChaerSeriesData.Controls.Add(this.lbChartSeriesXValue);
			this.grbxChaerSeriesData.Controls.Add(this.cbbxChartSeriesYValue);
			this.grbxChaerSeriesData.Controls.Add(this.cbbxChartSeriesXValue);
			this.grbxChaerSeriesData.Controls.Add(this.lbSeriesType);
			this.grbxChaerSeriesData.Controls.Add(this.cbbxSeriesDataType);
			this.grbxChaerSeriesData.Controls.Add(this.button3);
			this.grbxChaerSeriesData.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxChaerSeriesData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxChaerSeriesData.Location = new System.Drawing.Point(0, 0);
			this.grbxChaerSeriesData.Name = "grbxChaerSeriesData";
			this.grbxChaerSeriesData.Size = new System.Drawing.Size(168, 275);
			this.grbxChaerSeriesData.TabIndex = 0;
			this.grbxChaerSeriesData.TabStop = false;
			this.grbxChaerSeriesData.Text = "Series Data";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(8, 120);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(152, 16);
			this.label11.TabIndex = 25;
			this.label11.Text = "Series Name";
			// 
			// txtbxSeriesName2
			// 
			this.txtbxSeriesName2.Location = new System.Drawing.Point(8, 136);
			this.txtbxSeriesName2.Name = "txtbxSeriesName2";
			this.txtbxSeriesName2.Size = new System.Drawing.Size(152, 20);
			this.txtbxSeriesName2.TabIndex = 24;
			this.txtbxSeriesName2.Text = "";
			this.txtbxSeriesName2.TextChanged += new System.EventHandler(this.OnTextBoxSeriesNameChanged);
			// 
			// button1
			// 
			this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button1.Location = new System.Drawing.Point(8, 48);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(72, 24);
			this.button1.TabIndex = 19;
			this.button1.Text = "Add";
			this.button1.Click += new System.EventHandler(this.OnButtonAddSeriesClick);
			// 
			// comboBox1
			// 
			this.comboBox1.DisplayMember = "Name";
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Location = new System.Drawing.Point(8, 24);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(152, 21);
			this.comboBox1.TabIndex = 18;
			this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// lbChartSeriesYValue
			// 
			this.lbChartSeriesYValue.BackColor = System.Drawing.Color.White;
			this.lbChartSeriesYValue.Location = new System.Drawing.Point(8, 200);
			this.lbChartSeriesYValue.Name = "lbChartSeriesYValue";
			this.lbChartSeriesYValue.Size = new System.Drawing.Size(152, 16);
			this.lbChartSeriesYValue.TabIndex = 11;
			this.lbChartSeriesYValue.Text = "Y Value :";
			// 
			// lbChartSeriesXValue
			// 
			this.lbChartSeriesXValue.BackColor = System.Drawing.Color.White;
			this.lbChartSeriesXValue.Location = new System.Drawing.Point(8, 160);
			this.lbChartSeriesXValue.Name = "lbChartSeriesXValue";
			this.lbChartSeriesXValue.Size = new System.Drawing.Size(152, 16);
			this.lbChartSeriesXValue.TabIndex = 10;
			this.lbChartSeriesXValue.Text = "X Value :";
			// 
			// cbbxChartSeriesYValue
			// 
			this.cbbxChartSeriesYValue.BackColor = System.Drawing.Color.White;
			this.cbbxChartSeriesYValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartSeriesYValue.Location = new System.Drawing.Point(8, 216);
			this.cbbxChartSeriesYValue.Name = "cbbxChartSeriesYValue";
			this.cbbxChartSeriesYValue.Size = new System.Drawing.Size(152, 21);
			this.cbbxChartSeriesYValue.TabIndex = 9;
			this.cbbxChartSeriesYValue.SelectedIndexChanged += new System.EventHandler(this.cbbxChartSeriesYValue_SelectedIndexChanged);
			// 
			// cbbxChartSeriesXValue
			// 
			this.cbbxChartSeriesXValue.BackColor = System.Drawing.Color.White;
			this.cbbxChartSeriesXValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartSeriesXValue.Location = new System.Drawing.Point(8, 176);
			this.cbbxChartSeriesXValue.Name = "cbbxChartSeriesXValue";
			this.cbbxChartSeriesXValue.Size = new System.Drawing.Size(152, 21);
			this.cbbxChartSeriesXValue.TabIndex = 8;
			this.cbbxChartSeriesXValue.SelectedIndexChanged += new System.EventHandler(this.cbbxChartSeriesXValue_SelectedIndexChanged);
			// 
			// lbSeriesType
			// 
			this.lbSeriesType.BackColor = System.Drawing.Color.White;
			this.lbSeriesType.Location = new System.Drawing.Point(8, 80);
			this.lbSeriesType.Name = "lbSeriesType";
			this.lbSeriesType.Size = new System.Drawing.Size(152, 16);
			this.lbSeriesType.TabIndex = 7;
			this.lbSeriesType.Text = "Type";
			// 
			// cbbxSeriesType1
			// 
			this.cbbxSeriesDataType.BackColor = System.Drawing.Color.White;
			this.cbbxSeriesDataType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxSeriesDataType.Location = new System.Drawing.Point(8, 96);
			this.cbbxSeriesDataType.Name = "cbbxSeriesType1";
			this.cbbxSeriesDataType.Size = new System.Drawing.Size(152, 21);
			this.cbbxSeriesDataType.TabIndex = 6;
			this.cbbxSeriesDataType.SelectedIndexChanged += new System.EventHandler(this.cbbxSeriesDataType_SelectedIndexChanged);
			// 
			// button3
			// 
			this.button3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button3.Location = new System.Drawing.Point(88, 48);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(72, 24);
			this.button3.TabIndex = 19;
			this.button3.Text = "Remove";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// tbcrAppearance
			// 
			this.tbcrAppearance.Controls.Add(this.tbpgColorPalette);
			this.tbcrAppearance.Controls.Add(this.tbpgChartBorder);
			this.tbcrAppearance.Controls.Add(this.tbpgTitle);
			this.tbcrAppearance.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbcrAppearance.Location = new System.Drawing.Point(0, 0);
			this.tbcrAppearance.Name = "tbcrAppearance";
			this.tbcrAppearance.SelectedIndex = 0;
			this.tbcrAppearance.Size = new System.Drawing.Size(553, 306);
			this.tbcrAppearance.TabIndex = 18;
			this.tbcrAppearance.VisibleChanged += new System.EventHandler(this.tbcrAppearance_VisibleChanged);
			this.tbcrAppearance.SelectedIndexChanged += new System.EventHandler(this.tbcrAppearance_SelectedIndexChanged);
			// 
			// tbpgColorPalette
			// 
			this.tbpgColorPalette.Controls.Add(this.lsbxColorPalette);
			this.tbpgColorPalette.Location = new System.Drawing.Point(4, 25);
			this.tbpgColorPalette.Name = "tbpgColorPalette";
			this.tbpgColorPalette.Size = new System.Drawing.Size(545, 277);
			this.tbpgColorPalette.TabIndex = 0;
			this.tbpgColorPalette.Text = "Color Palette";
			// 
			// lsbxColorPalette
			// 
			this.lsbxColorPalette.Dock = System.Windows.Forms.DockStyle.Left;
			this.lsbxColorPalette.Location = new System.Drawing.Point(0, 0);
			this.lsbxColorPalette.Name = "lsbxColorPalette";
			this.lsbxColorPalette.Size = new System.Drawing.Size(168, 277);
			this.lsbxColorPalette.TabIndex = 1;
			this.lsbxColorPalette.SelectedIndexChanged += new System.EventHandler(this.lsbxColorPalette_SelectedIndexChanged);
			// 
			// tbpgChartBorder
			// 
			this.tbpgChartBorder.BackColor = System.Drawing.Color.Transparent;
			this.tbpgChartBorder.Controls.Add(this.grbxEditChartBorder);
			this.tbpgChartBorder.Location = new System.Drawing.Point(4, 25);
			this.tbpgChartBorder.Name = "tbpgChartBorder";
			this.tbpgChartBorder.Size = new System.Drawing.Size(545, 275);
			this.tbpgChartBorder.TabIndex = 1;
			this.tbpgChartBorder.Text = "Border & Back Color";
			this.tbpgChartBorder.Visible = false;
			// 
			// grbxEditChartBorder
			// 
			this.grbxEditChartBorder.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxEditChartBorder.Controls.Add(this.lbChartAreaBackColor);
			this.grbxEditChartBorder.Controls.Add(this.lbChartBackColor);
			this.grbxEditChartBorder.Controls.Add(this.lbChartAreaBorderColor);
			this.grbxEditChartBorder.Controls.Add(this.lbChartAreaBorderStyle);
			this.grbxEditChartBorder.Controls.Add(this.cbbxChartBorderStyle);
			this.grbxEditChartBorder.Controls.Add(this.brushInfoBox2);
			this.grbxEditChartBorder.Controls.Add(this.brushInfoBox3);
			this.grbxEditChartBorder.Controls.Add(this.colorBox2);
			this.grbxEditChartBorder.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxEditChartBorder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxEditChartBorder.Location = new System.Drawing.Point(0, 0);
			this.grbxEditChartBorder.Name = "grbxEditChartBorder";
			this.grbxEditChartBorder.Size = new System.Drawing.Size(168, 275);
			this.grbxEditChartBorder.TabIndex = 1;
			this.grbxEditChartBorder.TabStop = false;
			this.grbxEditChartBorder.Text = "Border & Back Color";
			// 
			// lbChartAreaBackColor
			// 
			this.lbChartAreaBackColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartAreaBackColor.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartAreaBackColor.Location = new System.Drawing.Point(8, 80);
			this.lbChartAreaBackColor.Name = "lbChartAreaBackColor";
			this.lbChartAreaBackColor.Size = new System.Drawing.Size(150, 16);
			this.lbChartAreaBackColor.TabIndex = 7;
			this.lbChartAreaBackColor.Text = "ChartArea Back Interior";
			// 
			// lbChartBackColor
			// 
			this.lbChartBackColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartBackColor.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartBackColor.Location = new System.Drawing.Point(8, 24);
			this.lbChartBackColor.Name = "lbChartBackColor";
			this.lbChartBackColor.Size = new System.Drawing.Size(150, 16);
			this.lbChartBackColor.TabIndex = 5;
			this.lbChartBackColor.Text = "Chart Back Interior";
			// 
			// lbChartAreaBorderColor
			// 
			this.lbChartAreaBorderColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartAreaBorderColor.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartAreaBorderColor.Location = new System.Drawing.Point(8, 184);
			this.lbChartAreaBorderColor.Name = "lbChartAreaBorderColor";
			this.lbChartAreaBorderColor.Size = new System.Drawing.Size(150, 16);
			this.lbChartAreaBorderColor.TabIndex = 3;
			this.lbChartAreaBorderColor.Text = "ChartArea Border Color";
			// 
			// lbChartAreaBorderStyle
			// 
			this.lbChartAreaBorderStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartAreaBorderStyle.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartAreaBorderStyle.Location = new System.Drawing.Point(8, 136);
			this.lbChartAreaBorderStyle.Name = "lbChartAreaBorderStyle";
			this.lbChartAreaBorderStyle.Size = new System.Drawing.Size(150, 16);
			this.lbChartAreaBorderStyle.TabIndex = 1;
			this.lbChartAreaBorderStyle.Text = "ChartArea Border Style";
			// 
			// cbbxChartBorderStyle
			// 
			this.cbbxChartBorderStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbbxChartBorderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartBorderStyle.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxChartBorderStyle.Location = new System.Drawing.Point(8, 152);
			this.cbbxChartBorderStyle.Name = "cbbxChartBorderStyle";
			this.cbbxChartBorderStyle.Size = new System.Drawing.Size(150, 20);
			this.cbbxChartBorderStyle.TabIndex = 0;
			this.cbbxChartBorderStyle.SelectedIndexChanged += new System.EventHandler(this.cbbxChartBorderStyle_SelectedIndexChanged);
			// 
			// brushInfoBox2
			// 
			this.brushInfoBox2.Location = new System.Drawing.Point(8, 40);
			this.brushInfoBox2.Name = "brushInfoBox2";
			this.brushInfoBox2.Size = new System.Drawing.Size(152, 24);
			this.brushInfoBox2.TabIndex = 23;
			this.brushInfoBox2.BrushInfoChanged += new System.EventHandler(this.brushInfoBox2_BrushInfoChanged);
			// 
			// brushInfoBox3
			// 
			this.brushInfoBox3.Location = new System.Drawing.Point(8, 96);
			this.brushInfoBox3.Name = "brushInfoBox3";
			this.brushInfoBox3.Size = new System.Drawing.Size(152, 24);
			this.brushInfoBox3.TabIndex = 23;
			this.brushInfoBox3.BrushInfoChanged += new System.EventHandler(this.brushInfoBox3_BrushInfoChanged);
			// 
			// colorBox2
			// 
			this.colorBox2.Location = new System.Drawing.Point(8, 200);
			this.colorBox2.Name = "colorBox2";
			this.colorBox2.Size = new System.Drawing.Size(152, 24);
			this.colorBox2.TabIndex = 23;
			this.colorBox2.ColorChanged += new System.EventHandler(this.colorBox2_ColorChanged);
			// 
			// tbpgTitle
			// 
			this.tbpgTitle.BackColor = System.Drawing.Color.Transparent;
			this.tbpgTitle.Controls.Add(this.grbxEditChartTitle);
			this.tbpgTitle.Location = new System.Drawing.Point(4, 25);
			this.tbpgTitle.Name = "tbpgTitle";
			this.tbpgTitle.Size = new System.Drawing.Size(545, 275);
			this.tbpgTitle.TabIndex = 2;
			this.tbpgTitle.Text = "Title";
			this.tbpgTitle.Visible = false;
			// 
			// grbxEditChartTitle
			// 
			this.grbxEditChartTitle.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxEditChartTitle.Controls.Add(this.lbChartTitleAlignment);
			this.grbxEditChartTitle.Controls.Add(this.cbbxTitleAlignment);
			this.grbxEditChartTitle.Controls.Add(this.lbChartTitleColor);
			this.grbxEditChartTitle.Controls.Add(this.lbChartTitlePosition);
			this.grbxEditChartTitle.Controls.Add(this.cbbxChartTitlePosition);
			this.grbxEditChartTitle.Controls.Add(this.lbChartTitleText);
			this.grbxEditChartTitle.Controls.Add(this.txtbxChartTitleText);
			this.grbxEditChartTitle.Controls.Add(this.colorBox1);
			this.grbxEditChartTitle.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxEditChartTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxEditChartTitle.Location = new System.Drawing.Point(0, 0);
			this.grbxEditChartTitle.Name = "grbxEditChartTitle";
			this.grbxEditChartTitle.Size = new System.Drawing.Size(168, 275);
			this.grbxEditChartTitle.TabIndex = 1;
			this.grbxEditChartTitle.TabStop = false;
			this.grbxEditChartTitle.Text = "Title";
			// 
			// lbChartTitleAlignment
			// 
			this.lbChartTitleAlignment.BackColor = System.Drawing.Color.White;
			this.lbChartTitleAlignment.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartTitleAlignment.Location = new System.Drawing.Point(8, 120);
			this.lbChartTitleAlignment.Name = "lbChartTitleAlignment";
			this.lbChartTitleAlignment.Size = new System.Drawing.Size(144, 16);
			this.lbChartTitleAlignment.TabIndex = 11;
			this.lbChartTitleAlignment.Text = "Title Alignment";
			// 
			// cbbxTitleAlignment
			// 
			this.cbbxTitleAlignment.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxTitleAlignment.Location = new System.Drawing.Point(8, 136);
			this.cbbxTitleAlignment.Name = "cbbxTitleAlignment";
			this.cbbxTitleAlignment.Size = new System.Drawing.Size(152, 21);
			this.cbbxTitleAlignment.TabIndex = 10;
			this.cbbxTitleAlignment.SelectedIndexChanged += new System.EventHandler(this.cbbxTitleAlignment_SelectedIndexChanged);
			// 
			// lbChartTitleColor
			// 
			this.lbChartTitleColor.BackColor = System.Drawing.Color.White;
			this.lbChartTitleColor.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartTitleColor.Location = new System.Drawing.Point(8, 168);
			this.lbChartTitleColor.Name = "lbChartTitleColor";
			this.lbChartTitleColor.Size = new System.Drawing.Size(144, 16);
			this.lbChartTitleColor.TabIndex = 9;
			this.lbChartTitleColor.Text = "Title Color";
			// 
			// lbChartTitlePosition
			// 
			this.lbChartTitlePosition.BackColor = System.Drawing.Color.White;
			this.lbChartTitlePosition.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartTitlePosition.Location = new System.Drawing.Point(8, 72);
			this.lbChartTitlePosition.Name = "lbChartTitlePosition";
			this.lbChartTitlePosition.Size = new System.Drawing.Size(144, 16);
			this.lbChartTitlePosition.TabIndex = 3;
			this.lbChartTitlePosition.Text = "Title Position";
			// 
			// cbbxChartTitlePosition
			// 
			this.cbbxChartTitlePosition.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxChartTitlePosition.Location = new System.Drawing.Point(8, 88);
			this.cbbxChartTitlePosition.Name = "cbbxChartTitlePosition";
			this.cbbxChartTitlePosition.Size = new System.Drawing.Size(152, 21);
			this.cbbxChartTitlePosition.TabIndex = 2;
			this.cbbxChartTitlePosition.SelectedIndexChanged += new System.EventHandler(this.cbbxChartTitlePosition_SelectedIndexChanged);
			// 
			// lbChartTitleText
			// 
			this.lbChartTitleText.BackColor = System.Drawing.Color.White;
			this.lbChartTitleText.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartTitleText.Location = new System.Drawing.Point(8, 24);
			this.lbChartTitleText.Name = "lbChartTitleText";
			this.lbChartTitleText.Size = new System.Drawing.Size(144, 16);
			this.lbChartTitleText.TabIndex = 1;
			this.lbChartTitleText.Text = "Title Text";
			// 
			// txtbxChartTitleText
			// 
			this.txtbxChartTitleText.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.txtbxChartTitleText.Location = new System.Drawing.Point(8, 40);
			this.txtbxChartTitleText.Name = "txtbxChartTitleText";
			this.txtbxChartTitleText.Size = new System.Drawing.Size(152, 20);
			this.txtbxChartTitleText.TabIndex = 0;
			this.txtbxChartTitleText.Text = "";
			this.txtbxChartTitleText.TextChanged += new System.EventHandler(this.txtbxChartTitleText_TextChanged);
			// 
			// colorBox1
			// 
			this.colorBox1.Location = new System.Drawing.Point(8, 184);
			this.colorBox1.Name = "colorBox1";
			this.colorBox1.Size = new System.Drawing.Size(152, 24);
			this.colorBox1.TabIndex = 23;
			this.colorBox1.ColorChanged += new System.EventHandler(this.colorBox1_ColorChanged);
			// 
			// tbcrChartLegend
			// 
			this.tbcrChartLegend.Controls.Add(this.tbpgChartLegend);
			this.tbcrChartLegend.Controls.Add(this.tbpgChartLegendBorderStyle);
			this.tbcrChartLegend.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbcrChartLegend.Location = new System.Drawing.Point(0, 0);
			this.tbcrChartLegend.Name = "tbcrChartLegend";
			this.tbcrChartLegend.SelectedIndex = 0;
			this.tbcrChartLegend.Size = new System.Drawing.Size(553, 306);
			this.tbcrChartLegend.TabIndex = 16;
			// 
			// tbpgChartLegend
			// 
			this.tbpgChartLegend.Controls.Add(this.grbxEditChartLegend);
			this.tbpgChartLegend.Location = new System.Drawing.Point(4, 25);
			this.tbpgChartLegend.Name = "tbpgChartLegend";
			this.tbpgChartLegend.Size = new System.Drawing.Size(545, 277);
			this.tbpgChartLegend.TabIndex = 0;
			this.tbpgChartLegend.Text = "Legend";
			// 
			// grbxEditChartLegend
			// 
			this.grbxEditChartLegend.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxEditChartLegend.Controls.Add(this.label7);
			this.grbxEditChartLegend.Controls.Add(this.lbCgartLegendOrientation);
			this.grbxEditChartLegend.Controls.Add(this.lbChartLegendAlignment);
			this.grbxEditChartLegend.Controls.Add(this.lbCgartLegendPosition);
			this.grbxEditChartLegend.Controls.Add(this.cbbxChartLegendOrientation);
			this.grbxEditChartLegend.Controls.Add(this.cbbxChartLegendAlignment);
			this.grbxEditChartLegend.Controls.Add(this.cbbxChartLegendPosition);
			this.grbxEditChartLegend.Controls.Add(this.chbxChartLegendVisible);
			this.grbxEditChartLegend.Controls.Add(this.bibxLegendBackInterior);
			this.grbxEditChartLegend.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxEditChartLegend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxEditChartLegend.Location = new System.Drawing.Point(0, 0);
			this.grbxEditChartLegend.Name = "grbxEditChartLegend";
			this.grbxEditChartLegend.Size = new System.Drawing.Size(168, 277);
			this.grbxEditChartLegend.TabIndex = 1;
			this.grbxEditChartLegend.TabStop = false;
			this.grbxEditChartLegend.Text = "Chart Legend";
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label7.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.label7.Location = new System.Drawing.Point(8, 48);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(150, 16);
			this.label7.TabIndex = 24;
			this.label7.Text = "Back Interior";
			// 
			// lbCgartLegendOrientation
			// 
			this.lbCgartLegendOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbCgartLegendOrientation.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbCgartLegendOrientation.Location = new System.Drawing.Point(8, 176);
			this.lbCgartLegendOrientation.Name = "lbCgartLegendOrientation";
			this.lbCgartLegendOrientation.Size = new System.Drawing.Size(150, 16);
			this.lbCgartLegendOrientation.TabIndex = 6;
			this.lbCgartLegendOrientation.Text = "Orientation";
			this.lbCgartLegendOrientation.Visible = false;
			// 
			// lbChartLegendAlignment
			// 
			this.lbChartLegendAlignment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartLegendAlignment.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartLegendAlignment.Location = new System.Drawing.Point(8, 136);
			this.lbChartLegendAlignment.Name = "lbChartLegendAlignment";
			this.lbChartLegendAlignment.Size = new System.Drawing.Size(150, 16);
			this.lbChartLegendAlignment.TabIndex = 5;
			this.lbChartLegendAlignment.Text = "Alignment";
			// 
			// lbCgartLegendPosition
			// 
			this.lbCgartLegendPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbCgartLegendPosition.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbCgartLegendPosition.Location = new System.Drawing.Point(8, 96);
			this.lbCgartLegendPosition.Name = "lbCgartLegendPosition";
			this.lbCgartLegendPosition.Size = new System.Drawing.Size(150, 16);
			this.lbCgartLegendPosition.TabIndex = 4;
			this.lbCgartLegendPosition.Text = "Position";
			// 
			// cbbxChartLegendOrientation
			// 
			this.cbbxChartLegendOrientation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbbxChartLegendOrientation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartLegendOrientation.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxChartLegendOrientation.Location = new System.Drawing.Point(8, 192);
			this.cbbxChartLegendOrientation.Name = "cbbxChartLegendOrientation";
			this.cbbxChartLegendOrientation.Size = new System.Drawing.Size(152, 21);
			this.cbbxChartLegendOrientation.TabIndex = 3;
			this.cbbxChartLegendOrientation.Visible = false;
			this.cbbxChartLegendOrientation.SelectedIndexChanged += new System.EventHandler(this.cbbxChartLegendOrientation_SelectedIndexChanged);
			// 
			// cbbxChartLegendAlignment
			// 
			this.cbbxChartLegendAlignment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbbxChartLegendAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartLegendAlignment.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxChartLegendAlignment.Location = new System.Drawing.Point(8, 152);
			this.cbbxChartLegendAlignment.Name = "cbbxChartLegendAlignment";
			this.cbbxChartLegendAlignment.Size = new System.Drawing.Size(152, 21);
			this.cbbxChartLegendAlignment.TabIndex = 2;
			this.cbbxChartLegendAlignment.SelectedIndexChanged += new System.EventHandler(this.cbbxChartLegendAlignment_SelectedIndexChanged);
			// 
			// cbbxChartLegendPosition
			// 
			this.cbbxChartLegendPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbbxChartLegendPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartLegendPosition.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxChartLegendPosition.Location = new System.Drawing.Point(8, 112);
			this.cbbxChartLegendPosition.Name = "cbbxChartLegendPosition";
			this.cbbxChartLegendPosition.Size = new System.Drawing.Size(152, 21);
			this.cbbxChartLegendPosition.TabIndex = 1;
			this.cbbxChartLegendPosition.SelectedIndexChanged += new System.EventHandler(this.cbbxChartLegendPosition_SelectedIndexChanged);
			// 
			// chbxChartLegendVisible
			// 
			this.chbxChartLegendVisible.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.chbxChartLegendVisible.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxChartLegendVisible.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.chbxChartLegendVisible.Location = new System.Drawing.Point(8, 24);
			this.chbxChartLegendVisible.Name = "chbxChartLegendVisible";
			this.chbxChartLegendVisible.Size = new System.Drawing.Size(150, 24);
			this.chbxChartLegendVisible.TabIndex = 0;
			this.chbxChartLegendVisible.Text = "Visible";
			this.chbxChartLegendVisible.CheckedChanged += new System.EventHandler(this.chbxChartLegendVisible_CheckedChanged);
			// 
			// bibxLegendBackInterior
			// 
			this.bibxLegendBackInterior.Location = new System.Drawing.Point(8, 64);
			this.bibxLegendBackInterior.Name = "bibxLegendBackInterior";
			this.bibxLegendBackInterior.Size = new System.Drawing.Size(152, 24);
			this.bibxLegendBackInterior.TabIndex = 23;
			this.bibxLegendBackInterior.BrushInfoChanged += new System.EventHandler(this.bibxLegendBackInterior_BrushInfoChanged);
			// 
			// tbpgChartLegendBorderStyle
			// 
			this.tbpgChartLegendBorderStyle.BackColor = System.Drawing.Color.Transparent;
			this.tbpgChartLegendBorderStyle.Controls.Add(this.grbxBorderStyle);
			this.tbpgChartLegendBorderStyle.Location = new System.Drawing.Point(4, 25);
			this.tbpgChartLegendBorderStyle.Name = "tbpgChartLegendBorderStyle";
			this.tbpgChartLegendBorderStyle.Size = new System.Drawing.Size(545, 275);
			this.tbpgChartLegendBorderStyle.TabIndex = 1;
			this.tbpgChartLegendBorderStyle.Text = "Border Style";
			this.tbpgChartLegendBorderStyle.Visible = false;
			// 
			// grbxBorderStyle
			// 
			this.grbxBorderStyle.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxBorderStyle.Controls.Add(this.lbChartLegendBorderWidth);
			this.grbxBorderStyle.Controls.Add(this.nmudChartLegendBorderWidth);
			this.grbxBorderStyle.Controls.Add(this.cbbxChartLegendBorderDashStyle);
			this.grbxBorderStyle.Controls.Add(this.label1);
			this.grbxBorderStyle.Controls.Add(this.chbxChartLegendShowBorder);
			this.grbxBorderStyle.Controls.Add(this.clrbxLegendBorderColor);
			this.grbxBorderStyle.Controls.Add(this.label14);
			this.grbxBorderStyle.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxBorderStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxBorderStyle.Location = new System.Drawing.Point(0, 0);
			this.grbxBorderStyle.Name = "grbxBorderStyle";
			this.grbxBorderStyle.Size = new System.Drawing.Size(168, 275);
			this.grbxBorderStyle.TabIndex = 2;
			this.grbxBorderStyle.TabStop = false;
			this.grbxBorderStyle.Text = "Border Style";
			// 
			// lbChartLegendBorderWidth
			// 
			this.lbChartLegendBorderWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartLegendBorderWidth.Location = new System.Drawing.Point(8, 136);
			this.lbChartLegendBorderWidth.Name = "lbChartLegendBorderWidth";
			this.lbChartLegendBorderWidth.Size = new System.Drawing.Size(152, 16);
			this.lbChartLegendBorderWidth.TabIndex = 4;
			this.lbChartLegendBorderWidth.Text = "Border Width";
			// 
			// nmudChartLegendBorderWidth
			// 
			this.nmudChartLegendBorderWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.nmudChartLegendBorderWidth.Location = new System.Drawing.Point(8, 152);
			this.nmudChartLegendBorderWidth.Name = "nmudChartLegendBorderWidth";
			this.nmudChartLegendBorderWidth.Size = new System.Drawing.Size(152, 20);
			this.nmudChartLegendBorderWidth.TabIndex = 3;
			this.nmudChartLegendBorderWidth.ValueChanged += new System.EventHandler(this.nmudChartLegendBorderWidth_ValueChanged);
			// 
			// cbbxChartLegendBorderDashStyle
			// 
			this.cbbxChartLegendBorderDashStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbbxChartLegendBorderDashStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartLegendBorderDashStyle.Location = new System.Drawing.Point(8, 112);
			this.cbbxChartLegendBorderDashStyle.Name = "cbbxChartLegendBorderDashStyle";
			this.cbbxChartLegendBorderDashStyle.Size = new System.Drawing.Size(152, 20);
			this.cbbxChartLegendBorderDashStyle.TabIndex = 2;
			this.cbbxChartLegendBorderDashStyle.SelectedIndexChanged += new System.EventHandler(this.cbbxLegendDashStyle_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(8, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Dash Style";
			// 
			// chbxChartLegendShowBorder
			// 
			this.chbxChartLegendShowBorder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.chbxChartLegendShowBorder.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxChartLegendShowBorder.Location = new System.Drawing.Point(8, 24);
			this.chbxChartLegendShowBorder.Name = "chbxChartLegendShowBorder";
			this.chbxChartLegendShowBorder.Size = new System.Drawing.Size(152, 24);
			this.chbxChartLegendShowBorder.TabIndex = 0;
			this.chbxChartLegendShowBorder.Text = "Show border";
			this.chbxChartLegendShowBorder.CheckedChanged += new System.EventHandler(this.chbxChartLegendShowBorder_CheckedChanged);
			// 
			// clrbxLegendBorderColor
			// 
			this.clrbxLegendBorderColor.Location = new System.Drawing.Point(8, 64);
			this.clrbxLegendBorderColor.Name = "clrbxLegendBorderColor";
			this.clrbxLegendBorderColor.Size = new System.Drawing.Size(152, 24);
			this.clrbxLegendBorderColor.TabIndex = 23;
			this.clrbxLegendBorderColor.ColorChanged += new System.EventHandler(this.clrbxLegendBorderColor_ColorChanged);
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label14.Location = new System.Drawing.Point(8, 48);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(152, 16);
			this.label14.TabIndex = 1;
			this.label14.Text = "Border Color";
			// 
			// tbcrChartToolBar
			// 
			this.tbcrChartToolBar.Controls.Add(this.tbpgChartToolBar);
			this.tbcrChartToolBar.Controls.Add(this.tbpgChartToolbarBorder);
			this.tbcrChartToolBar.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbcrChartToolBar.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.tbcrChartToolBar.Location = new System.Drawing.Point(0, 0);
			this.tbcrChartToolBar.Multiline = true;
			this.tbcrChartToolBar.Name = "tbcrChartToolBar";
			this.tbcrChartToolBar.Padding = new System.Drawing.Point(6, 2);
			this.tbcrChartToolBar.SelectedIndex = 0;
			this.tbcrChartToolBar.Size = new System.Drawing.Size(553, 306);
			this.tbcrChartToolBar.TabIndex = 13;
			// 
			// tbpgChartToolBar
			// 
			this.tbpgChartToolBar.Controls.Add(this.grbxEditChartToolBar);
			this.tbpgChartToolBar.Location = new System.Drawing.Point(4, 23);
			this.tbpgChartToolBar.Name = "tbpgChartToolBar";
			this.tbpgChartToolBar.Size = new System.Drawing.Size(545, 279);
			this.tbpgChartToolBar.TabIndex = 0;
			this.tbpgChartToolBar.Text = "ToolBar";
			// 
			// grbxEditChartToolBar
			// 
			this.grbxEditChartToolBar.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxEditChartToolBar.Controls.Add(this.bttnToolBarEditItems);
			this.grbxEditChartToolBar.Controls.Add(this.lbChartToolBarBackColor);
			this.grbxEditChartToolBar.Controls.Add(this.chbxChartToolBarVisible);
			this.grbxEditChartToolBar.Controls.Add(this.grbxChartToolBarButtonStyle);
			this.grbxEditChartToolBar.Controls.Add(this.clrbxToolBarBackColor);
			this.grbxEditChartToolBar.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxEditChartToolBar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxEditChartToolBar.Location = new System.Drawing.Point(0, 0);
			this.grbxEditChartToolBar.Name = "grbxEditChartToolBar";
			this.grbxEditChartToolBar.Size = new System.Drawing.Size(168, 279);
			this.grbxEditChartToolBar.TabIndex = 1;
			this.grbxEditChartToolBar.TabStop = false;
			this.grbxEditChartToolBar.Text = "ToolBar";
			// 
			// bttnToolBarEditItems
			// 
			this.bttnToolBarEditItems.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.bttnToolBarEditItems.Location = new System.Drawing.Point(8, 96);
			this.bttnToolBarEditItems.Name = "bttnToolBarEditItems";
			this.bttnToolBarEditItems.Size = new System.Drawing.Size(152, 23);
			this.bttnToolBarEditItems.TabIndex = 24;
			this.bttnToolBarEditItems.Text = "Edit items...";
			this.bttnToolBarEditItems.Click += new System.EventHandler(this.bttnToolBarEditItems_Click);
			// 
			// lbChartToolBarBackColor
			// 
			this.lbChartToolBarBackColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartToolBarBackColor.BackColor = System.Drawing.Color.Transparent;
			this.lbChartToolBarBackColor.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartToolBarBackColor.Location = new System.Drawing.Point(8, 48);
			this.lbChartToolBarBackColor.Name = "lbChartToolBarBackColor";
			this.lbChartToolBarBackColor.Size = new System.Drawing.Size(152, 16);
			this.lbChartToolBarBackColor.TabIndex = 2;
			this.lbChartToolBarBackColor.Text = "Back Color";
			// 
			// chbxChartToolBarVisible
			// 
			this.chbxChartToolBarVisible.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.chbxChartToolBarVisible.BackColor = System.Drawing.Color.Transparent;
			this.chbxChartToolBarVisible.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxChartToolBarVisible.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.chbxChartToolBarVisible.Location = new System.Drawing.Point(8, 24);
			this.chbxChartToolBarVisible.Name = "chbxChartToolBarVisible";
			this.chbxChartToolBarVisible.Size = new System.Drawing.Size(152, 16);
			this.chbxChartToolBarVisible.TabIndex = 0;
			this.chbxChartToolBarVisible.Text = "Visible";
			this.chbxChartToolBarVisible.CheckedChanged += new System.EventHandler(this.chbxChartToolBarVisible_CheckedChanged);
			// 
			// grbxChartToolBarButtonStyle
			// 
			this.grbxChartToolBarButtonStyle.BackColor = System.Drawing.Color.Transparent;
			this.grbxChartToolBarButtonStyle.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxChartToolBarButtonStyle.Controls.Add(this.lbChartToolBarButtonHeight);
			this.grbxChartToolBarButtonStyle.Controls.Add(this.lbChartToolBarButtonWidth);
			this.grbxChartToolBarButtonStyle.Controls.Add(this.nmupChartToolBarButtonHeight);
			this.grbxChartToolBarButtonStyle.Controls.Add(this.nmupChartToolBarButtonWidth);
			this.grbxChartToolBarButtonStyle.Controls.Add(this.lbButtonStyle);
			this.grbxChartToolBarButtonStyle.Controls.Add(this.cbbxChartToolBarButtonStyle);
			this.grbxChartToolBarButtonStyle.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.grbxChartToolBarButtonStyle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxChartToolBarButtonStyle.Location = new System.Drawing.Point(3, 124);
			this.grbxChartToolBarButtonStyle.Name = "grbxChartToolBarButtonStyle";
			this.grbxChartToolBarButtonStyle.Size = new System.Drawing.Size(162, 152);
			this.grbxChartToolBarButtonStyle.TabIndex = 3;
			this.grbxChartToolBarButtonStyle.TabStop = false;
			this.grbxChartToolBarButtonStyle.Text = "Button Style";
			// 
			// lbChartToolBarButtonHeight
			// 
			this.lbChartToolBarButtonHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartToolBarButtonHeight.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartToolBarButtonHeight.Location = new System.Drawing.Point(8, 104);
			this.lbChartToolBarButtonHeight.Name = "lbChartToolBarButtonHeight";
			this.lbChartToolBarButtonHeight.Size = new System.Drawing.Size(146, 16);
			this.lbChartToolBarButtonHeight.TabIndex = 5;
			this.lbChartToolBarButtonHeight.Text = "Button Height";
			// 
			// lbChartToolBarButtonWidth
			// 
			this.lbChartToolBarButtonWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartToolBarButtonWidth.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartToolBarButtonWidth.Location = new System.Drawing.Point(8, 64);
			this.lbChartToolBarButtonWidth.Name = "lbChartToolBarButtonWidth";
			this.lbChartToolBarButtonWidth.Size = new System.Drawing.Size(146, 16);
			this.lbChartToolBarButtonWidth.TabIndex = 4;
			this.lbChartToolBarButtonWidth.Text = "Button Width";
			// 
			// nmupChartToolBarButtonHeight
			// 
			this.nmupChartToolBarButtonHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.nmupChartToolBarButtonHeight.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.nmupChartToolBarButtonHeight.Location = new System.Drawing.Point(8, 120);
			this.nmupChartToolBarButtonHeight.Name = "nmupChartToolBarButtonHeight";
			this.nmupChartToolBarButtonHeight.Size = new System.Drawing.Size(146, 20);
			this.nmupChartToolBarButtonHeight.TabIndex = 3;
			this.nmupChartToolBarButtonHeight.Value = new System.Decimal(new int[] {
																																							 24,
																																							 0,
																																							 0,
																																							 0});
			this.nmupChartToolBarButtonHeight.ValueChanged += new System.EventHandler(this.nmupChartToolBarButtonSize_ValueChanged);
			// 
			// nmupChartToolBarButtonWidth
			// 
			this.nmupChartToolBarButtonWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.nmupChartToolBarButtonWidth.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.nmupChartToolBarButtonWidth.Location = new System.Drawing.Point(8, 80);
			this.nmupChartToolBarButtonWidth.Name = "nmupChartToolBarButtonWidth";
			this.nmupChartToolBarButtonWidth.Size = new System.Drawing.Size(146, 20);
			this.nmupChartToolBarButtonWidth.TabIndex = 2;
			this.nmupChartToolBarButtonWidth.Value = new System.Decimal(new int[] {
																																							24,
																																							0,
																																							0,
																																							0});
			this.nmupChartToolBarButtonWidth.ValueChanged += new System.EventHandler(this.nmupChartToolBarButtonSize_ValueChanged);
			// 
			// lbButtonStyle
			// 
			this.lbButtonStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbButtonStyle.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbButtonStyle.Location = new System.Drawing.Point(8, 24);
			this.lbButtonStyle.Name = "lbButtonStyle";
			this.lbButtonStyle.Size = new System.Drawing.Size(146, 16);
			this.lbButtonStyle.TabIndex = 1;
			this.lbButtonStyle.Visible = false;
			this.lbButtonStyle.Text = "Button Style";
			// 
			// cbbxChartToolBarButtonStyle
			// 
			this.cbbxChartToolBarButtonStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbbxChartToolBarButtonStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartToolBarButtonStyle.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxChartToolBarButtonStyle.Location = new System.Drawing.Point(8, 40);
			this.cbbxChartToolBarButtonStyle.Name = "cbbxChartToolBarButtonStyle";
			this.cbbxChartToolBarButtonStyle.Size = new System.Drawing.Size(146, 21);
			this.cbbxChartToolBarButtonStyle.TabIndex = 0;
			// 
			// clrbxToolBarBackColor
			// 
			this.clrbxToolBarBackColor.Location = new System.Drawing.Point(8, 64);
			this.clrbxToolBarBackColor.Name = "clrbxToolBarBackColor";
			this.clrbxToolBarBackColor.Size = new System.Drawing.Size(152, 23);
			this.clrbxToolBarBackColor.TabIndex = 23;
			this.clrbxToolBarBackColor.ColorChanged += new System.EventHandler(this.clrbxToolBarBackColor_ColorChanged);
			// 
			// tbpgChartToolbarBorder
			// 
			this.tbpgChartToolbarBorder.Controls.Add(this.grbxChartToolBarBorder);
			this.tbpgChartToolbarBorder.Location = new System.Drawing.Point(4, 23);
			this.tbpgChartToolbarBorder.Name = "tbpgChartToolbarBorder";
			this.tbpgChartToolbarBorder.Size = new System.Drawing.Size(545, 277);
			this.tbpgChartToolbarBorder.TabIndex = 1;
			this.tbpgChartToolbarBorder.Text = "Border Style";
			this.tbpgChartToolbarBorder.Visible = false;
			// 
			// grbxChartToolBarBorder
			// 
			this.grbxChartToolBarBorder.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxChartToolBarBorder.Controls.Add(this.lbChartToolBarBorderColor);
			this.grbxChartToolBarBorder.Controls.Add(this.lbChartToolBarBorderWidth);
			this.grbxChartToolBarBorder.Controls.Add(this.nmudChartToolBarBorderWidth);
			this.grbxChartToolBarBorder.Controls.Add(this.lbChartToolBarBorderStyle);
			this.grbxChartToolBarBorder.Controls.Add(this.cbbxChartToolBarBorderStyle);
			this.grbxChartToolBarBorder.Controls.Add(this.chbxChartToolBarShowBorder);
			this.grbxChartToolBarBorder.Controls.Add(this.clrbxToolBarBorderColor);
			this.grbxChartToolBarBorder.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxChartToolBarBorder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxChartToolBarBorder.Location = new System.Drawing.Point(0, 0);
			this.grbxChartToolBarBorder.Name = "grbxChartToolBarBorder";
			this.grbxChartToolBarBorder.Size = new System.Drawing.Size(168, 277);
			this.grbxChartToolBarBorder.TabIndex = 2;
			this.grbxChartToolBarBorder.TabStop = false;
			this.grbxChartToolBarBorder.Text = "Border Style";
			// 
			// lbChartToolBarBorderColor
			// 
			this.lbChartToolBarBorderColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartToolBarBorderColor.BackColor = System.Drawing.Color.White;
			this.lbChartToolBarBorderColor.Location = new System.Drawing.Point(8, 48);
			this.lbChartToolBarBorderColor.Name = "lbChartToolBarBorderColor";
			this.lbChartToolBarBorderColor.Size = new System.Drawing.Size(152, 16);
			this.lbChartToolBarBorderColor.TabIndex = 6;
			this.lbChartToolBarBorderColor.Text = "Border Color";
			// 
			// lbChartToolBarBorderWidth
			// 
			this.lbChartToolBarBorderWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartToolBarBorderWidth.BackColor = System.Drawing.Color.White;
			this.lbChartToolBarBorderWidth.Location = new System.Drawing.Point(8, 144);
			this.lbChartToolBarBorderWidth.Name = "lbChartToolBarBorderWidth";
			this.lbChartToolBarBorderWidth.Size = new System.Drawing.Size(152, 16);
			this.lbChartToolBarBorderWidth.TabIndex = 4;
			this.lbChartToolBarBorderWidth.Text = "Border Width";
			// 
			// nmudChartToolBarBorderWidth
			// 
			this.nmudChartToolBarBorderWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.nmudChartToolBarBorderWidth.Location = new System.Drawing.Point(8, 160);
			this.nmudChartToolBarBorderWidth.Name = "nmudChartToolBarBorderWidth";
			this.nmudChartToolBarBorderWidth.Size = new System.Drawing.Size(152, 20);
			this.nmudChartToolBarBorderWidth.TabIndex = 3;
			this.nmudChartToolBarBorderWidth.ValueChanged += new System.EventHandler(this.nmudChartToolBarBorderWidth_ValueChanged);
			// 
			// lbChartToolBarBorderStyle
			// 
			this.lbChartToolBarBorderStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lbChartToolBarBorderStyle.BackColor = System.Drawing.Color.White;
			this.lbChartToolBarBorderStyle.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbChartToolBarBorderStyle.Location = new System.Drawing.Point(8, 96);
			this.lbChartToolBarBorderStyle.Name = "lbChartToolBarBorderStyle";
			this.lbChartToolBarBorderStyle.Size = new System.Drawing.Size(152, 16);
			this.lbChartToolBarBorderStyle.TabIndex = 2;
			this.lbChartToolBarBorderStyle.Text = "Border Style";
			// 
			// cbbxChartToolBarBorderStyle
			// 
			this.cbbxChartToolBarBorderStyle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.cbbxChartToolBarBorderStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxChartToolBarBorderStyle.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.cbbxChartToolBarBorderStyle.Location = new System.Drawing.Point(8, 112);
			this.cbbxChartToolBarBorderStyle.Name = "cbbxChartToolBarBorderStyle";
			this.cbbxChartToolBarBorderStyle.Size = new System.Drawing.Size(152, 20);
			this.cbbxChartToolBarBorderStyle.TabIndex = 1;
			this.cbbxChartToolBarBorderStyle.SelectedIndexChanged += new System.EventHandler(this.cbbxChartToolBarBorderStyle_SelectedIndexChanged);
			// 
			// chbxChartToolBarShowBorder
			// 
			this.chbxChartToolBarShowBorder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.chbxChartToolBarShowBorder.BackColor = System.Drawing.Color.White;
			this.chbxChartToolBarShowBorder.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxChartToolBarShowBorder.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.chbxChartToolBarShowBorder.Location = new System.Drawing.Point(8, 24);
			this.chbxChartToolBarShowBorder.Name = "chbxChartToolBarShowBorder";
			this.chbxChartToolBarShowBorder.Size = new System.Drawing.Size(152, 16);
			this.chbxChartToolBarShowBorder.TabIndex = 0;
			this.chbxChartToolBarShowBorder.Text = "Show Border";
			this.chbxChartToolBarShowBorder.CheckedChanged += new System.EventHandler(this.chbxChartToolBarShowBorder_CheckedChanged);
			// 
			// clrbxToolBarBorderColor
			// 
			this.clrbxToolBarBorderColor.Location = new System.Drawing.Point(8, 64);
			this.clrbxToolBarBorderColor.Name = "clrbxToolBarBorderColor";
			this.clrbxToolBarBorderColor.Size = new System.Drawing.Size(152, 24);
			this.clrbxToolBarBorderColor.TabIndex = 23;
			this.clrbxToolBarBorderColor.ColorChanged += new System.EventHandler(this.clrbxToolBarBorderColor_ColorChanged);
			// 
			// tbcrAxes
			// 
			this.tbcrAxes.Controls.Add(this.tbpgPrimaryXAxis);
			this.tbcrAxes.Controls.Add(this.tbpgPrimaryYAxis);
			this.tbcrAxes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbcrAxes.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
			this.tbcrAxes.ItemSize = new System.Drawing.Size(144, 18);
			this.tbcrAxes.Location = new System.Drawing.Point(0, 0);
			this.tbcrAxes.Name = "tbcrAxes";
			this.tbcrAxes.SelectedIndex = 0;
			this.tbcrAxes.Size = new System.Drawing.Size(553, 306);
			this.tbcrAxes.TabIndex = 19;
			// 
			// tbpgPrimaryXAxis
			// 
			this.tbpgPrimaryXAxis.Controls.Add(this.lblDoesntUseXAxis);
			this.tbpgPrimaryXAxis.Controls.Add(this.grbxEditXAxis);
			this.tbpgPrimaryXAxis.Location = new System.Drawing.Point(4, 22);
			this.tbpgPrimaryXAxis.Name = "tbpgPrimaryXAxis";
			this.tbpgPrimaryXAxis.Size = new System.Drawing.Size(545, 280);
			this.tbpgPrimaryXAxis.TabIndex = 0;
			this.tbpgPrimaryXAxis.Text = "X Axis";
			// 
			// lblDoesntUseXAxis
			// 
			this.lblDoesntUseXAxis.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblDoesntUseXAxis.Location = new System.Drawing.Point(168, 0);
			this.lblDoesntUseXAxis.Name = "lblDoesntUseXAxis";
			this.lblDoesntUseXAxis.Size = new System.Drawing.Size(168, 280);
			this.lblDoesntUseXAxis.TabIndex = 2;
			this.lblDoesntUseXAxis.Text = "This type doesn\'t use axes";
			this.lblDoesntUseXAxis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// grbxEditXAxis
			// 
			this.grbxEditXAxis.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxEditXAxis.Controls.Add(this.txtbxXAxisFormat);
			this.grbxEditXAxis.Controls.Add(this.chbxXAxisOpposed);
			this.grbxEditXAxis.Controls.Add(this.chbxXAxisInversed);
			this.grbxEditXAxis.Controls.Add(this.bttnXAxisEditLabels);
			this.grbxEditXAxis.Controls.Add(this.lbXAxisValueType);
			this.grbxEditXAxis.Controls.Add(this.cbbxXAxisValueType);
			this.grbxEditXAxis.Controls.Add(this.lbXAxisTitle);
			this.grbxEditXAxis.Controls.Add(this.txtbxXAxisTitle);
			this.grbxEditXAxis.Controls.Add(this.chbxXAxisGridLine);
			this.grbxEditXAxis.Controls.Add(this.sabxXAxisTitleAlignment);
			this.grbxEditXAxis.Controls.Add(this.cbbxXAxisIntersectAction);
			this.grbxEditXAxis.Controls.Add(this.label6);
			this.grbxEditXAxis.Controls.Add(this.label13);
			this.grbxEditXAxis.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxEditXAxis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxEditXAxis.Location = new System.Drawing.Point(0, 0);
			this.grbxEditXAxis.Name = "grbxEditXAxis";
			this.grbxEditXAxis.Size = new System.Drawing.Size(168, 280);
			this.grbxEditXAxis.TabIndex = 1;
			this.grbxEditXAxis.TabStop = false;
			this.grbxEditXAxis.Text = "Primary X Axis";
			// 
			// txtbxXAxisFormat
			// 
			this.txtbxXAxisFormat.Location = new System.Drawing.Point(8, 184);
			this.txtbxXAxisFormat.Name = "txtbxXAxisFormat";
			this.txtbxXAxisFormat.Size = new System.Drawing.Size(152, 20);
			this.txtbxXAxisFormat.TabIndex = 28;
			this.txtbxXAxisFormat.Text = "";
			this.txtbxXAxisFormat.TextChanged += new System.EventHandler(this.txtbxXAxisFormat_TextChanged);
			// 
			// chbxXAxisOpposed
			// 
			this.chbxXAxisOpposed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxXAxisOpposed.Location = new System.Drawing.Point(8, 112);
			this.chbxXAxisOpposed.Name = "chbxXAxisOpposed";
			this.chbxXAxisOpposed.Size = new System.Drawing.Size(152, 16);
			this.chbxXAxisOpposed.TabIndex = 27;
			this.chbxXAxisOpposed.Text = "Opposed";
			this.chbxXAxisOpposed.CheckedChanged += new System.EventHandler(this.chbxXAxisOpposed_CheckedChanged);
			// 
			// chbxXAxisInversed
			// 
			this.chbxXAxisInversed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxXAxisInversed.Location = new System.Drawing.Point(8, 96);
			this.chbxXAxisInversed.Name = "chbxXAxisInversed";
			this.chbxXAxisInversed.Size = new System.Drawing.Size(152, 16);
			this.chbxXAxisInversed.TabIndex = 26;
			this.chbxXAxisInversed.Text = "Inversed";
			this.chbxXAxisInversed.CheckedChanged += new System.EventHandler(this.chbxXAxisInversed_CheckedChanged);
			// 
			// bttnXAxisEditLabels
			// 
			this.bttnXAxisEditLabels.BackColor = System.Drawing.SystemColors.Control;
			this.bttnXAxisEditLabels.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.bttnXAxisEditLabels.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.bttnXAxisEditLabels.Location = new System.Drawing.Point(8, 248);
			this.bttnXAxisEditLabels.Name = "bttnXAxisEditLabels";
			this.bttnXAxisEditLabels.Size = new System.Drawing.Size(152, 24);
			this.bttnXAxisEditLabels.TabIndex = 5;
			this.bttnXAxisEditLabels.Text = "Edit Labels...";
			this.bttnXAxisEditLabels.Click += new System.EventHandler(this.bttnXAxisEditLabels_Click);
			// 
			// lbXAxisValueType
			// 
			this.lbXAxisValueType.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbXAxisValueType.Location = new System.Drawing.Point(8, 128);
			this.lbXAxisValueType.Name = "lbXAxisValueType";
			this.lbXAxisValueType.Size = new System.Drawing.Size(152, 16);
			this.lbXAxisValueType.TabIndex = 4;
			this.lbXAxisValueType.Text = "Value Type";
			// 
			// cbbxXAxisValueType
			// 
			this.cbbxXAxisValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxXAxisValueType.Location = new System.Drawing.Point(8, 144);
			this.cbbxXAxisValueType.Name = "cbbxXAxisValueType";
			this.cbbxXAxisValueType.Size = new System.Drawing.Size(152, 21);
			this.cbbxXAxisValueType.TabIndex = 3;
			this.cbbxXAxisValueType.SelectedIndexChanged += new System.EventHandler(this.cbbxXAxisValueType_SelectedIndexChanged);
			// 
			// lbXAxisTitle
			// 
			this.lbXAxisTitle.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.lbXAxisTitle.Location = new System.Drawing.Point(8, 40);
			this.lbXAxisTitle.Name = "lbXAxisTitle";
			this.lbXAxisTitle.Size = new System.Drawing.Size(152, 16);
			this.lbXAxisTitle.TabIndex = 2;
			this.lbXAxisTitle.Text = "Axis Title";
			// 
			// txtbxXAxisTitle
			// 
			this.txtbxXAxisTitle.ForeColor = System.Drawing.Color.Navy;
			this.txtbxXAxisTitle.Location = new System.Drawing.Point(8, 56);
			this.txtbxXAxisTitle.Name = "txtbxXAxisTitle";
			this.txtbxXAxisTitle.Size = new System.Drawing.Size(152, 20);
			this.txtbxXAxisTitle.TabIndex = 1;
			this.txtbxXAxisTitle.Text = "";
			this.txtbxXAxisTitle.TextChanged += new System.EventHandler(this.txtbxXAxisTitle_TextChanged);
			// 
			// chbxXAxisGridLine
			// 
			this.chbxXAxisGridLine.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxXAxisGridLine.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.chbxXAxisGridLine.Location = new System.Drawing.Point(8, 16);
			this.chbxXAxisGridLine.Name = "chbxXAxisGridLine";
			this.chbxXAxisGridLine.Size = new System.Drawing.Size(152, 24);
			this.chbxXAxisGridLine.TabIndex = 0;
			this.chbxXAxisGridLine.Text = "Grid Line";
			this.chbxXAxisGridLine.CheckedChanged += new System.EventHandler(this.chbxXAxisGridLine_CheckedChanged);
			// 
			// sabxXAxisTitleAlignment
			// 
			this.sabxXAxisTitleAlignment.Location = new System.Drawing.Point(8, 72);
			this.sabxXAxisTitleAlignment.Name = "sabxXAxisTitleAlignment";
			this.sabxXAxisTitleAlignment.Size = new System.Drawing.Size(152, 24);
			this.sabxXAxisTitleAlignment.TabIndex = 25;
			this.sabxXAxisTitleAlignment.StringAlignmentChanged += new System.EventHandler(this.sabxXAxisTitleAlignment_StringAlignmentChanged);
			// 
			// cbbxXAxisIntersectAction
			// 
			this.cbbxXAxisIntersectAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxXAxisIntersectAction.Location = new System.Drawing.Point(8, 224);
			this.cbbxXAxisIntersectAction.Name = "cbbxXAxisIntersectAction";
			this.cbbxXAxisIntersectAction.Size = new System.Drawing.Size(152, 21);
			this.cbbxXAxisIntersectAction.TabIndex = 3;
			this.cbbxXAxisIntersectAction.SelectedIndexChanged += new System.EventHandler(this.cbbxXAxisIntersectAction_SelectedIndexChanged);
			// 
			// label6
			// 
			this.label6.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.label6.Location = new System.Drawing.Point(8, 208);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(152, 16);
			this.label6.TabIndex = 4;
			this.label6.Text = "Intersect Action";
			// 
			// label13
			// 
			this.label13.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(48)), ((System.Byte)(63)), ((System.Byte)(128)));
			this.label13.Location = new System.Drawing.Point(8, 168);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(152, 16);
			this.label13.TabIndex = 4;
			this.label13.Text = "Format";
			// 
			// tbpgPrimaryYAxis
			// 
			this.tbpgPrimaryYAxis.Controls.Add(this.lblDoesntUseYAxis);
			this.tbpgPrimaryYAxis.Controls.Add(this.grbxEditYAxis);
			this.tbpgPrimaryYAxis.Location = new System.Drawing.Point(4, 22);
			this.tbpgPrimaryYAxis.Name = "tbpgPrimaryYAxis";
			this.tbpgPrimaryYAxis.Size = new System.Drawing.Size(545, 278);
			this.tbpgPrimaryYAxis.TabIndex = 1;
			this.tbpgPrimaryYAxis.Text = "Y Axis";
			this.tbpgPrimaryYAxis.Visible = false;
			// 
			// lblDoesntUseYAxis
			// 
			this.lblDoesntUseYAxis.Dock = System.Windows.Forms.DockStyle.Left;
			this.lblDoesntUseYAxis.Location = new System.Drawing.Point(168, 0);
			this.lblDoesntUseYAxis.Name = "lblDoesntUseYAxis";
			this.lblDoesntUseYAxis.Size = new System.Drawing.Size(168, 278);
			this.lblDoesntUseYAxis.TabIndex = 4;
			this.lblDoesntUseYAxis.Text = "This type doesn\'t use axes";
			this.lblDoesntUseYAxis.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// grbxEditYAxis
			// 
			this.grbxEditYAxis.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.grbxEditYAxis.Controls.Add(this.txtbxYAxisTitle);
			this.grbxEditYAxis.Controls.Add(this.txtbxYAxisFormat);
			this.grbxEditYAxis.Controls.Add(this.chbxYAxisOpposed);
			this.grbxEditYAxis.Controls.Add(this.chbxYAxisInversed);
			this.grbxEditYAxis.Controls.Add(this.sabxYAxisTitleAlignment);
			this.grbxEditYAxis.Controls.Add(this.bttnYAxisEditLabels);
			this.grbxEditYAxis.Controls.Add(this.lbYAxisValueType);
			this.grbxEditYAxis.Controls.Add(this.cbbxYaxisValueType);
			this.grbxEditYAxis.Controls.Add(this.lbYAxisTitle);
			this.grbxEditYAxis.Controls.Add(this.chbxYAxisGridLine);
			this.grbxEditYAxis.Controls.Add(this.label12);
			this.grbxEditYAxis.Dock = System.Windows.Forms.DockStyle.Left;
			this.grbxEditYAxis.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.grbxEditYAxis.Location = new System.Drawing.Point(0, 0);
			this.grbxEditYAxis.Name = "grbxEditYAxis";
			this.grbxEditYAxis.Size = new System.Drawing.Size(168, 278);
			this.grbxEditYAxis.TabIndex = 3;
			this.grbxEditYAxis.TabStop = false;
			this.grbxEditYAxis.Text = "Primary Y Axis";
			// 
			// txtbxYAxisTitle
			// 
			this.txtbxYAxisTitle.Location = new System.Drawing.Point(8, 56);
			this.txtbxYAxisTitle.Name = "txtbxYAxisTitle";
			this.txtbxYAxisTitle.Size = new System.Drawing.Size(152, 20);
			this.txtbxYAxisTitle.TabIndex = 1;
			this.txtbxYAxisTitle.Text = "";
			this.txtbxYAxisTitle.TextChanged += new System.EventHandler(this.txtbxYAxisTitle_TextChanged);
			// 
			// txtbxYAxisFormat
			// 
			this.txtbxYAxisFormat.Location = new System.Drawing.Point(8, 184);
			this.txtbxYAxisFormat.Name = "txtbxYAxisFormat";
			this.txtbxYAxisFormat.Size = new System.Drawing.Size(152, 20);
			this.txtbxYAxisFormat.TabIndex = 33;
			this.txtbxYAxisFormat.Text = "";
			this.txtbxYAxisFormat.TextChanged += new System.EventHandler(this.txtbxYAxisFormat_TextChanged);
			// 
			// chbxYAxisOpposed
			// 
			this.chbxYAxisOpposed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxYAxisOpposed.Location = new System.Drawing.Point(8, 112);
			this.chbxYAxisOpposed.Name = "chbxYAxisOpposed";
			this.chbxYAxisOpposed.Size = new System.Drawing.Size(152, 16);
			this.chbxYAxisOpposed.TabIndex = 32;
			this.chbxYAxisOpposed.Text = "Opposed";
			this.chbxYAxisOpposed.CheckedChanged += new System.EventHandler(this.chbxYAxisOpposed_CheckedChanged);
			// 
			// chbxYAxisInversed
			// 
			this.chbxYAxisInversed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxYAxisInversed.Location = new System.Drawing.Point(8, 96);
			this.chbxYAxisInversed.Name = "chbxYAxisInversed";
			this.chbxYAxisInversed.Size = new System.Drawing.Size(152, 16);
			this.chbxYAxisInversed.TabIndex = 31;
			this.chbxYAxisInversed.Text = "Inversed";
			this.chbxYAxisInversed.CheckedChanged += new System.EventHandler(this.chbxYAxisInversed_CheckedChanged);
			// 
			// sabxYAxisTitleAlignment
			// 
			this.sabxYAxisTitleAlignment.Location = new System.Drawing.Point(8, 72);
			this.sabxYAxisTitleAlignment.Name = "sabxYAxisTitleAlignment";
			this.sabxYAxisTitleAlignment.Size = new System.Drawing.Size(152, 24);
			this.sabxYAxisTitleAlignment.TabIndex = 30;
			this.sabxYAxisTitleAlignment.StringAlignmentChanged += new System.EventHandler(this.sabxYAxisTitleAlignment_StringAlignmentChanged);
			// 
			// bttnYAxisEditLabels
			// 
			this.bttnYAxisEditLabels.BackColor = System.Drawing.SystemColors.Control;
			this.bttnYAxisEditLabels.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.bttnYAxisEditLabels.Location = new System.Drawing.Point(8, 248);
			this.bttnYAxisEditLabels.Name = "bttnYAxisEditLabels";
			this.bttnYAxisEditLabels.Size = new System.Drawing.Size(152, 24);
			this.bttnYAxisEditLabels.TabIndex = 5;
			this.bttnYAxisEditLabels.Text = "Edit Labels...";
			this.bttnYAxisEditLabels.Click += new System.EventHandler(this.bttnYAxisEditLabels_Click);
			// 
			// lbYAxisValueType
			// 
			this.lbYAxisValueType.Location = new System.Drawing.Point(8, 128);
			this.lbYAxisValueType.Name = "lbYAxisValueType";
			this.lbYAxisValueType.Size = new System.Drawing.Size(152, 16);
			this.lbYAxisValueType.TabIndex = 4;
			this.lbYAxisValueType.Text = "Value Type";
			// 
			// cbbxYaxisValueType
			// 
			this.cbbxYaxisValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxYaxisValueType.Location = new System.Drawing.Point(8, 144);
			this.cbbxYaxisValueType.Name = "cbbxYaxisValueType";
			this.cbbxYaxisValueType.Size = new System.Drawing.Size(152, 21);
			this.cbbxYaxisValueType.TabIndex = 3;
			this.cbbxYaxisValueType.SelectedIndexChanged += new System.EventHandler(this.cbbxYaxisValueType_SelectedIndexChanged);
			// 
			// lbYAxisTitle
			// 
			this.lbYAxisTitle.Location = new System.Drawing.Point(8, 40);
			this.lbYAxisTitle.Name = "lbYAxisTitle";
			this.lbYAxisTitle.Size = new System.Drawing.Size(152, 16);
			this.lbYAxisTitle.TabIndex = 2;
			this.lbYAxisTitle.Text = "Axis Title";
			// 
			// chbxYAxisGridLine
			// 
			this.chbxYAxisGridLine.BackColor = System.Drawing.Color.Transparent;
			this.chbxYAxisGridLine.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxYAxisGridLine.Location = new System.Drawing.Point(8, 16);
			this.chbxYAxisGridLine.Name = "chbxYAxisGridLine";
			this.chbxYAxisGridLine.Size = new System.Drawing.Size(152, 24);
			this.chbxYAxisGridLine.TabIndex = 0;
			this.chbxYAxisGridLine.Text = "Grid Line";
			this.chbxYAxisGridLine.CheckedChanged += new System.EventHandler(this.chbxYAxisGridLine_CheckedChanged);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(8, 168);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(152, 16);
			this.label12.TabIndex = 4;
			this.label12.Text = "Format";
			// 
			// checkAutoRun
			// 
			this.checkAutoRun.BackColor = System.Drawing.Color.Transparent;
			this.checkAutoRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(204)));
			this.checkAutoRun.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(35)), ((System.Byte)(69)), ((System.Byte)(141)));
			this.checkAutoRun.Location = new System.Drawing.Point(24, 112);
			this.checkAutoRun.Name = "checkAutoRun";
			this.checkAutoRun.Size = new System.Drawing.Size(136, 17);
			this.checkAutoRun.TabIndex = 20;
			this.checkAutoRun.Text = "Auto Run Wizard";
			this.checkAutoRun.CheckedChanged += new System.EventHandler(this.checkAutoRun_CheckedChanged);
			// 
			// checkBox1
			// 
			this.checkBox1.BackColor = System.Drawing.Color.Transparent;
			this.checkBox1.ForeColor = System.Drawing.SystemColors.Window;
			this.checkBox1.Location = new System.Drawing.Point(525, 12);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(107, 17);
			this.checkBox1.TabIndex = 20;
			this.checkBox1.Text = "Auto Run Wizard";
			// 
			// gradientPanel2
			// 
			this.gradientPanel2.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(190)), ((System.Byte)(216)), ((System.Byte)(253)));
			this.gradientPanel2.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(132)), ((System.Byte)(170)), ((System.Byte)(217)));
			this.gradientPanel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.gradientPanel2.Controls.Add(this.chbxIs3D);
			this.gradientPanel2.Controls.Add(this.lbChartTypeGroups);
			this.gradientPanel2.Controls.Add(this.cbbxChartTypeGroups);
			this.gradientPanel2.Controls.Add(this.gradientPanel1);
			this.gradientPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gradientPanel2.Location = new System.Drawing.Point(0, 0);
			this.gradientPanel2.Name = "gradientPanel2";
			this.gradientPanel2.Size = new System.Drawing.Size(553, 306);
			this.gradientPanel2.TabIndex = 21;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.pnPoints);
			this.panel1.Controls.Add(this.gradientPanel2);
			this.panel1.Controls.Add(this.tbcrChartToolBar);
			this.panel1.Controls.Add(this.tbcrChartLegend);
			this.panel1.Controls.Add(this.tbcrSeries);
			this.panel1.Controls.Add(this.tbcrAppearance);
			this.panel1.Controls.Add(this.tbcrAxes);
			this.panel1.Location = new System.Drawing.Point(175, 134);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(553, 306);
			this.panel1.TabIndex = 22;
			// 
			// pnPoints
			// 
			this.pnPoints.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(190)), ((System.Byte)(216)), ((System.Byte)(253)));
			this.pnPoints.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(132)), ((System.Byte)(170)), ((System.Byte)(217)));
			this.pnPoints.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnPoints.Controls.Add(this.chartGroupBox2);
			this.pnPoints.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnPoints.Location = new System.Drawing.Point(0, 0);
			this.pnPoints.Name = "pnPoints";
			this.pnPoints.Size = new System.Drawing.Size(553, 306);
			this.pnPoints.TabIndex = 23;
			// 
			// chartGroupBox2
			// 
			this.chartGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((System.Byte)(105)), ((System.Byte)(110)), ((System.Byte)(152)));
			this.chartGroupBox2.Controls.Add(this.label8);
			this.chartGroupBox2.Controls.Add(this.label5);
			this.chartGroupBox2.Controls.Add(this.cbbxPointLabelsApplySeries);
			this.chartGroupBox2.Controls.Add(this.label2);
			this.chartGroupBox2.Controls.Add(this.chbxPointsLabelsShow);
			this.chartGroupBox2.Controls.Add(this.clrbxPointsLabelsColor);
			this.chartGroupBox2.Controls.Add(this.orbxPointsLabelsAlignment);
			this.chartGroupBox2.Controls.Add(this.label3);
			this.chartGroupBox2.Controls.Add(this.fntbxPointsLabelsFont);
			this.chartGroupBox2.Controls.Add(this.label4);
			this.chartGroupBox2.Controls.Add(this.nmudPointsLabelsRotate);
			this.chartGroupBox2.Dock = System.Windows.Forms.DockStyle.Left;
			this.chartGroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chartGroupBox2.Location = new System.Drawing.Point(0, 0);
			this.chartGroupBox2.Name = "chartGroupBox2";
			this.chartGroupBox2.Size = new System.Drawing.Size(168, 304);
			this.chartGroupBox2.TabIndex = 4;
			this.chartGroupBox2.TabStop = false;
			this.chartGroupBox2.Text = "Points";
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.Color.Transparent;
			this.label8.Location = new System.Drawing.Point(8, 256);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(152, 16);
			this.label8.TabIndex = 27;
			this.label8.Text = "Apply to";
			// 
			// label5
			// 
			this.label5.BackColor = System.Drawing.Color.Transparent;
			this.label5.Location = new System.Drawing.Point(8, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(152, 16);
			this.label5.TabIndex = 26;
			this.label5.Text = "Labels Rotate Angle";
			// 
			// cbbxPointLabelsApplySeries
			// 
			this.cbbxPointLabelsApplySeries.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbbxPointLabelsApplySeries.Location = new System.Drawing.Point(8, 272);
			this.cbbxPointLabelsApplySeries.Name = "cbbxPointLabelsApplySeries";
			this.cbbxPointLabelsApplySeries.Size = new System.Drawing.Size(152, 21);
			this.cbbxPointLabelsApplySeries.TabIndex = 25;
			this.cbbxPointLabelsApplySeries.SelectedIndexChanged += new System.EventHandler(this.OnPointLabelsSerieToApplyChnaged);
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(8, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 16);
			this.label2.TabIndex = 24;
			this.label2.Text = "Labels Color";
			// 
			// chbxPointsLabelsShow
			// 
			this.chbxPointsLabelsShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chbxPointsLabelsShow.Location = new System.Drawing.Point(8, 24);
			this.chbxPointsLabelsShow.Name = "chbxPointsLabelsShow";
			this.chbxPointsLabelsShow.Size = new System.Drawing.Size(152, 16);
			this.chbxPointsLabelsShow.TabIndex = 0;
			this.chbxPointsLabelsShow.Text = "Show labels";
			this.chbxPointsLabelsShow.CheckedChanged += new System.EventHandler(this.OnPointsLabelsChanged);
			// 
			// orbxPointsLabelsAlignment
			// 
			this.orbxPointsLabelsAlignment.Location = new System.Drawing.Point(48, 184);
			this.orbxPointsLabelsAlignment.Name = "orbxPointsLabelsAlignment";
			this.orbxPointsLabelsAlignment.Size = new System.Drawing.Size(72, 72);
			this.orbxPointsLabelsAlignment.TabIndex = 24;
			this.orbxPointsLabelsAlignment.OrientationChanged += new System.EventHandler(this.OnPointsLabelsChanged);
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(8, 168);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(152, 16);
			this.label3.TabIndex = 24;
			this.label3.Text = "Labels Alignment";
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Location = new System.Drawing.Point(8, 40);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(152, 16);
			this.label4.TabIndex = 24;
			this.label4.Text = "Labels Font";
			// 
			// nmudPointsLabelsRotate
			// 
			this.nmudPointsLabelsRotate.Location = new System.Drawing.Point(8, 144);
			this.nmudPointsLabelsRotate.Maximum = new System.Decimal(new int[] {
																																					 360,
																																					 0,
																																					 0,
																																					 0});
			this.nmudPointsLabelsRotate.Name = "nmudPointsLabelsRotate";
			this.nmudPointsLabelsRotate.Size = new System.Drawing.Size(152, 20);
			this.nmudPointsLabelsRotate.TabIndex = 25;
			this.nmudPointsLabelsRotate.ValueChanged += new System.EventHandler(this.OnPointsLabelsChanged);
			// 
			// bttnTabPoints
			// 
			this.bttnTabPoints.Cursor = System.Windows.Forms.Cursors.Hand;
			this.bttnTabPoints.Location = new System.Drawing.Point(32, 272);
			this.bttnTabPoints.Name = "bttnTabPoints";
			this.bttnTabPoints.NormalImage = ((System.Drawing.Image)(resources.GetObject("bttnTabPoints.NormalImage")));
			this.bttnTabPoints.SelectedImage = ((System.Drawing.Image)(resources.GetObject("bttnTabPoints.SelectedImage")));
			this.bttnTabPoints.Size = new System.Drawing.Size(120, 24);
			this.bttnTabPoints.TabIndex = 0;
			this.bttnTabPoints.Text = "Points";
			this.bttnTabPoints.Click += new System.EventHandler(this.TabButtonsClick);
			// 
			// ChartWizardForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(218)), ((System.Byte)(234)), ((System.Byte)(255)));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = new System.Drawing.Size(746, 488);
			this.Controls.Add(this.checkAutoRun);
			this.Controls.Add(this.bttnPrevious);
			this.Controls.Add(this.bttnNext);
			this.Controls.Add(this.lbTitle);
			this.Controls.Add(this.bttnTabChartStyle);
			this.Controls.Add(this.bttnTabAxes);
			this.Controls.Add(this.bttnTabAppearance);
			this.Controls.Add(this.bttnTabSeries);
			this.Controls.Add(this.bttnTabLegend);
			this.Controls.Add(this.bttnTabToolBar);
			this.Controls.Add(this.bttnCancel);
			this.Controls.Add(this.bttnApply);
			this.Controls.Add(this.bttnOk);
			this.Controls.Add(this.bttnTabPoints);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.chcrWizardChart);
			this.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(53)), ((System.Byte)(69)), ((System.Byte)(141)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MaximumSize = new System.Drawing.Size(752, 530);
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(752, 530);
			this.Name = "ChartWizardForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Chart Wizard";
			((System.ComponentModel.ISupportInitialize)(this.gradientPanel1)).EndInit();
			this.gradientPanel1.ResumeLayout(false);
			this.tbcrSeries.ResumeLayout(false);
			this.tbpgAddPointToSeries.ResumeLayout(false);
			this.grbcChartAddSeriesCodeGenerator.ResumeLayout(false);
			this.tbpgDataSource.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dtgdDataSource)).EndInit();
			this.tbpgSeriesData.ResumeLayout(false);
			this.grbxChaerSeriesData.ResumeLayout(false);
			this.tbcrAppearance.ResumeLayout(false);
			this.tbpgColorPalette.ResumeLayout(false);
			this.tbpgChartBorder.ResumeLayout(false);
			this.grbxEditChartBorder.ResumeLayout(false);
			this.tbpgTitle.ResumeLayout(false);
			this.grbxEditChartTitle.ResumeLayout(false);
			this.tbcrChartLegend.ResumeLayout(false);
			this.tbpgChartLegend.ResumeLayout(false);
			this.grbxEditChartLegend.ResumeLayout(false);
			this.tbpgChartLegendBorderStyle.ResumeLayout(false);
			this.grbxBorderStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nmudChartLegendBorderWidth)).EndInit();
			this.tbcrChartToolBar.ResumeLayout(false);
			this.tbpgChartToolBar.ResumeLayout(false);
			this.grbxEditChartToolBar.ResumeLayout(false);
			this.grbxChartToolBarButtonStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nmupChartToolBarButtonHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nmupChartToolBarButtonWidth)).EndInit();
			this.tbpgChartToolbarBorder.ResumeLayout(false);
			this.grbxChartToolBarBorder.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nmudChartToolBarBorderWidth)).EndInit();
			this.tbcrAxes.ResumeLayout(false);
			this.tbpgPrimaryXAxis.ResumeLayout(false);
			this.grbxEditXAxis.ResumeLayout(false);
			this.tbpgPrimaryYAxis.ResumeLayout(false);
			this.grbxEditYAxis.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gradientPanel2)).EndInit();
			this.gradientPanel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnPoints)).EndInit();
			this.pnPoints.ResumeLayout(false);
			this.chartGroupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.nmudPointsLabelsRotate)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion
		#endregion

		#region Event handlers

        /// <summary>
        /// Handles the MouseMove event of the ListViewSeriesTypes control.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void lsvwSeriesTypes_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem focusingItem = this.lsvwSeriesTypes.GetItemAt(e.X, e.Y);

            if (focusingItem == null)
            {
                this.lsvwSeriesTypes.Cursor = Cursors.Default;
            }
            else
            {
                this.lsvwSeriesTypes.Cursor = Cursors.Hand;
            }

        }

		/// <summary>
		/// Handles the CheckedChanged event of the checkAutoRun control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void checkAutoRun_CheckedChanged(object sender, EventArgs e)
		{
			ChartControlDesigner.AutoRunWizard = checkAutoRun.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lsvwSeriesTypes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (lsvwSeriesTypes.SelectedItems.Count > 0)
			{
				m_chartSeriesType = (ChartSeriesType)lsvwSeriesTypes.SelectedItems[0].Tag;

				foreach (ChartSeries series in chcrWizardChart.Series)
				{
					this.ApplyTypeSettings(series, m_chartSeriesType);
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void lsbxColorPalette_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string styleName = lsbxColorPalette.SelectedItem.ToString();

			ChartAppearanceStyles.ApplyFormat(chcrWizardChart, styleName);

			//lsvwSeriesTypes.LargeImageList = null;
			//InitializeThumbs();
			//lsvwSeriesTypes.LargeImageList = SeriesTypeThumbs;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartTypeGroups_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			SelectChartTypeGroup(cbbxChartTypeGroups.SelectedIndex);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxIs3D_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Series3D = chbxIs3D.Checked;

			lsvwSeriesTypes.LargeImageList = null;
			InitializeThumbs();
			lsvwSeriesTypes.LargeImageList = SeriesTypeThumbs;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnPreview_Click(object sender, System.EventArgs e)
		{
			SelectPseudoTab(ChartMath.MinMax(m_currentTabIndex - 1, c_tabStartIndex, c_tabEndTabIndex));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnNext_Click(object sender, System.EventArgs e)
		{
			SelectPseudoTab(ChartMath.MinMax(m_currentTabIndex + 1, c_tabStartIndex, c_tabEndTabIndex));
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnApply_Click(object sender, System.EventArgs e)
		{
			bool currentVisible = chcrWizardChart.Visible;
			chcrWizardChart.Visible = true;

			this.EnsureSeriesAdded();
			this.CopyProperties(chcrWizardChart, m_chart);
			//this.CopyProperties(m_chart, chcrWizardChart);
			//this.UpdateSeriesCollection(chcrWizardChart);

			chcrWizardChart.Visible = currentVisible;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnOk_Click(object sender, System.EventArgs e)
		{
			chcrWizardChart.Visible = true;
			this.EnsureSeriesAdded();
			this.CopyProperties(chcrWizardChart, m_chart);
			chcrWizardChart.Series.Clear();
		}
		/// <summary>
		/// Handles the Click event of the bttnCancel control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void bttnCancel_Click(object sender, EventArgs e)
		{
			m_chart.ToolBar.RewireItems();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbcrAppearance_VisibleChanged(object sender, System.EventArgs e)
		{
			if (tbcrAppearance.Visible)
			{
				if (tbcrAppearance.SelectedIndex == 0)
				{
					lbTitle.Text = ChartWizardResources.PaletteTitle;
				}
				else if (tbcrAppearance.SelectedIndex == 1)
				{
					lbTitle.Text = ChartWizardResources.BorderAndBackgroundTitle;
				}
				else
				{
					lbTitle.Text = ChartWizardResources.TitleTitle;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbcrAppearance_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (tbcrAppearance.SelectedIndex == 0)
			{
				lbTitle.Text = ChartWizardResources.PaletteTitle;
			}
			else if (tbcrAppearance.SelectedIndex == 1)
			{
				lbTitle.Text = ChartWizardResources.BorderAndBackgroundTitle;
			}
			else
			{
				lbTitle.Text = ChartWizardResources.TitleTitle;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbcrSeries_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (tbcrSeries.SelectedIndex)
			{ 
				case 0:
					lbTitle.Text = ChartWizardResources.SeriesPointsTitle;
					break;

				case 1:
					lbTitle.Text = ChartWizardResources.DataSourceTitle;
					break;

				case 2:
					lbTitle.Text = ChartWizardResources.SeriesDataTitle;
					break;
			}

			chcrWizardChart.Visible = ShowChartPreview();
			chcrWizardChart.BringToFront();
		}

		private bool ShowChartPreview()
		{
			if (m_currentTabIndex == c_tabChartTypeIndex) return false;
			if (m_currentTabIndex == c_tabSeriesIndex && tbcrSeries.SelectedIndex == 1) return false;

			return true;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartSeries_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ChartSeries series = cbbxChartSeries.SelectedItem as ChartSeries;

			if (series != null)
			{
				lsbxChartPoints.DataSource = new ChartPointIndexer( series.SeriesModel );
				txtbxSeriesName.Text = series.Name;
				cbbxSeriesPointsType.SelectedItem = series.Type;

				lsbxChartPoints.Enabled = true;
				txtbxSeriesName.Enabled = true;
				cbbxSeriesPointsType.Enabled = true;
			}
			else
			{
				txtbxSeriesName.Text = "";
				cbbxSeriesPointsType.SelectedItem = m_chartSeriesType;
				lsbxChartPoints.DataSource = null;

				lsbxChartPoints.Enabled = false;
				txtbxSeriesName.Enabled = false;
				cbbxSeriesPointsType.Enabled = false;
			}
		}
		/// <summary>
		/// Tabs the buttons click.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void TabButtonsClick(object sender, System.EventArgs e)
		{
			this.SelectPseudoTab((sender as ImageButton).TagIndex);
		}

		#region	Data tab
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnEditPoints_Click(object sender, System.EventArgs e)
		{
			ChartSeries series = cbbxChartSeries.SelectedItem as ChartSeries;

			if (this.EnsureSeriesModelPresent(series))
			{
				IChartSeriesModel seriesModel = series.SeriesModel;

				if (seriesModel.Count == 0)
				{
					seriesModel = new ChartSeriesModel();
				}
				
				ChartPointIndexer pointIndexer = new ChartPointIndexer(seriesModel);

				this.ShowPropertyEditor(series, "Points", pointIndexer);

				if (seriesModel.Count > 0)
				{
					series.SeriesModel = seriesModel;
				}

				lsbxChartPoints.DataSource = null;
				lsbxChartPoints.DataSource = new ChartPointIndexer(series.SeriesModel);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartSeriesXValue_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string member = cbbxChartSeriesXValue.SelectedItem as string;
			ChartSeries series = comboBox1.SelectedItem as ChartSeries;

			if (this.EnsureDataBindModelPresent(series))
			{
				ChartDataBindModel dataBindModel = series.SeriesModel as ChartDataBindModel;

				dataBindModel.DataSource = (cbbxDataSources.SelectedItem as NamedObject).Tag;
				dataBindModel.DataMember = m_curentDataMember;
				dataBindModel.XName = (member == c_noneName) ? null : member;
			}
			else
			{
				cbbxChartSeriesXValue.SelectedItem = c_noneName;
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="name"></param>
        private object getDataSource(ComboBox.ObjectCollection items, string name)
        {
            for (int i = 0; i< items.Count; i++)
                if (items[i].ToString().Equals(name))
                    return items[i];
            return c_noneNamedObject;
        }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxDataSources_SelectedIndexChanged(object sender, System.EventArgs e)
		{
#if SyncfusionFramework2_0
			if (cbbxDataSources.SelectedItem == c_newBindingSourceNamedObject)
			{
				this.CreateNewBindingSource(m_chart);

				cbbxDataSources.SelectedItem = string.IsNullOrEmpty(m_chart.DataSourceName)?c_noneNamedObject:getDataSource(cbbxDataSources.Items, m_chart.DataSourceName);
			}
#else
			if (cbbxDataSources.SelectedItem == c_newOleDataAdapterNamedObject)
			{
				this.CreateNewOleDataAdapter(m_chart);

				cbbxDataSources.SelectedItem = c_noneNamedObject;
			}
			else if (cbbxDataSources.SelectedItem == c_newSqlDataAdapterNamedObject)
			{
				this.CreateNewSqlDataAdapter(m_chart);

				cbbxDataSources.SelectedItem = c_noneNamedObject;
			}
#endif
			else
			{
				this.UpdateDataMembers("");
			}
            m_chart.DataSourceName = cbbxDataSources.SelectedItem.ToString();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartSeriesYValue_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string member = cbbxChartSeriesYValue.SelectedItem as string;
			ChartSeries series = comboBox1.SelectedItem as ChartSeries;

			if (this.EnsureDataBindModelPresent(series))
			{
				ChartDataBindModel dataBindModel = series.SeriesModel as ChartDataBindModel;

				dataBindModel.DataSource = (cbbxDataSources.SelectedItem as NamedObject).Tag;
				dataBindModel.DataMember = m_curentDataMember;
				dataBindModel.YNames = member == c_noneName ? null : new string[] { member };
			}
			else
			{
				cbbxChartSeriesYValue.SelectedItem = c_noneName;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="ne"></param>
		private void dtgdDataSource_Navigate(object sender, System.Windows.Forms.NavigateEventArgs ne)
		{
			this.UpdateDataMembers((sender as DataGrid).DataMember);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void comboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ChartSeries series = comboBox1.SelectedItem as ChartSeries;

			if (series != null)
			{
				txtbxSeriesName2.Text = series.Name;
				cbbxSeriesDataType.SelectedItem = series.Type;

				txtbxSeriesName2.Enabled = true;
				cbbxSeriesDataType.Enabled = true;
				cbbxChartSeriesXValue.Enabled = true;
				cbbxChartSeriesYValue.Enabled = true;
			}
			else
			{
				txtbxSeriesName2.Text = "";
				cbbxSeriesDataType.SelectedItem = m_chartSeriesType;

				txtbxSeriesName2.Enabled = false;
				cbbxSeriesDataType.Enabled = false;
				cbbxChartSeriesXValue.Enabled = false;
				cbbxChartSeriesYValue.Enabled = false;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxSeriesDataType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ChartSeries series = comboBox1.SelectedItem as ChartSeries;

			if (series != null)
			{
				ChartSeriesType oldType = series.Type;

				this.ApplyTypeSettings(series, (ChartSeriesType)cbbxSeriesDataType.SelectedItem);

				if (!series.Compatible)
				{
					if (MessageBox.Show(ChartWizardResources.SeriesIncompatibleMessage, "Type isn't compatible", MessageBoxButtons.YesNo) == DialogResult.No)
					{
						this.ApplyTypeSettings(series, oldType);
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTextBoxSeriesNameChanged(object sender, System.EventArgs e)
		{
			ChartSeries series = comboBox1.SelectedItem as ChartSeries;

			if (series != null)
			{
				series.Name = (sender as TextBoxBase).Text;
				series.Text = series.Name;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnButtonAddSeriesClick(object sender, System.EventArgs e)
		{
			int curIndex = FilterRandomSeries(this.chcrWizardChart.Series).Length;

			chcrWizardChart.Series.Add(new ChartSeries("Series" + curIndex, m_chartSeriesType));

			this.UpdateSeriesCollection(chcrWizardChart);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnButtonRemoveSeriesClick(object sender, System.EventArgs e)
		{
			ChartSeries series = cbbxChartSeries.SelectedItem as ChartSeries;

			if (series != null && chcrWizardChart.Series.Count > 1)
			{
				chcrWizardChart.Series.Remove(series);

				this.UpdateSeriesCollection(chcrWizardChart);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtbxSeriesName_TextChanged(object sender, System.EventArgs e)
		{
			ChartSeries series = cbbxChartSeries.SelectedItem as ChartSeries;

			if (series != null)
			{
				series.Name = txtbxSeriesName.Text;
				series.Text = series.Name;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxSeriesPointsType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ChartSeries series = cbbxChartSeries.SelectedItem as ChartSeries;

			if (series != null)
			{
				ChartSeriesType oldType = series.Type;

				this.ApplyTypeSettings(series, (ChartSeriesType)cbbxSeriesPointsType.SelectedItem);

				if (!series.Compatible)
				{
					if (MessageBox.Show(ChartWizardResources.SeriesIncompatibleMessage, "Type isn't compatible", MessageBoxButtons.YesNo) == DialogResult.No)
					{
						this.ApplyTypeSettings(series, oldType);
					}
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, System.EventArgs e)
		{
			ChartSeries series = comboBox1.SelectedItem as ChartSeries;

			if (series != null && chcrWizardChart.Series.Count > 1)
			{
				chcrWizardChart.Series.Remove(series);
				this.UpdateSeriesCollection(chcrWizardChart);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tbcrSeries_VisibleChanged(object sender, EventArgs e)
		{
			switch (tbcrSeries.SelectedIndex)
			{
				case 0:
					lbTitle.Text = ChartWizardResources.SeriesPointsTitle;
					break;

				case 1:
					lbTitle.Text = ChartWizardResources.DataSourceTitle;
					break;

				case 2:
					lbTitle.Text = ChartWizardResources.SeriesDataTitle;
					break;
			}
		}
		#endregion

		#region	Appearance tab
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartTitlePosition_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.TextPosition = (ChartTextPosition)cbbxChartTitlePosition.SelectedItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxTitleAlignment_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.TextAlignment = (StringAlignment)cbbxTitleAlignment.SelectedItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtbxChartTitleText_TextChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Text = txtbxChartTitleText.Text;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartBorderStyle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ChartArea.BorderStyle = (BorderStyle)cbbxChartBorderStyle.SelectedItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void brushInfoBox2_BrushInfoChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.BackInterior = brushInfoBox2.BrushInfo;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void brushInfoBox3_BrushInfoChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ChartArea.BackInterior = brushInfoBox3.BrushInfo;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void colorBox2_ColorChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ChartArea.BorderColor = colorBox2.Color;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void colorBox1_ColorChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Title.ForeColor = colorBox1.Color;
		}
		#endregion

		#region	Axes Tab
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sabxXAxisTitleAlignment_StringAlignmentChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryXAxis.TitleAlignment = sabxXAxisTitleAlignment.StringAlignment;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxXAxisIntersectAction_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryXAxis.LabelIntersectAction = (ChartLabelIntersectAction)cbbxXAxisIntersectAction.SelectedItem;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxXAxisInversed_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryXAxis.Inversed = chbxXAxisInversed.Checked;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxXAxisOpposed_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryXAxis.OpposedPosition = chbxXAxisOpposed.Checked;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxXAxisGridLine_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryXAxis.DrawGrid = chbxXAxisGridLine.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtbxXAxisTitle_TextChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryXAxis.Title = txtbxXAxisTitle.Text;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxXAxisValueType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ChartValueType type = chcrWizardChart.PrimaryXAxis.ValueType;

			try
			{
				chcrWizardChart.PrimaryXAxis.ValueType = (ChartValueType)cbbxXAxisValueType.SelectedItem;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Chart Control", MessageBoxButtons.OK, MessageBoxIcon.Error);
				chcrWizardChart.PrimaryXAxis.ValueType = type;
			}

			if( chcrWizardChart.PrimaryXAxis.ValueType == ChartValueType.DateTime )
			{
				txtbxXAxisFormat.Text = chcrWizardChart.PrimaryXAxis.DateTimeFormat;
			}
			else
			{
				txtbxXAxisFormat.Text = chcrWizardChart.PrimaryXAxis.Format;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnXAxisEditLabels_Click(object sender, System.EventArgs e)
		{
			this.ShowPropertyEditor(chcrWizardChart.PrimaryXAxis, "Labels", chcrWizardChart.PrimaryXAxis.Labels);

			if (chcrWizardChart.PrimaryXAxis.Labels.Count > 0
				&& (chcrWizardChart.PrimaryXAxis.TickLabelsDrawingMode & ChartAxisTickLabelDrawingMode.UserMode) == ChartAxisTickLabelDrawingMode.None)
			{
				chcrWizardChart.PrimaryXAxis.TickLabelsDrawingMode |= ChartAxisTickLabelDrawingMode.UserMode;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sabxYAxisTitleAlignment_StringAlignmentChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryYAxis.TitleAlignment = sabxYAxisTitleAlignment.StringAlignment;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxYAxisInversed_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryYAxis.Inversed = chbxYAxisInversed.Checked;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxYAxisOpposed_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryYAxis.OpposedPosition = chbxYAxisOpposed.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxYAxisGridLine_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryYAxis.DrawGrid = chbxYAxisGridLine.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtbxYAxisTitle_TextChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.PrimaryYAxis.Title = txtbxYAxisTitle.Text;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxYaxisValueType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ChartValueType type = chcrWizardChart.PrimaryYAxis.ValueType;

			try
			{
				chcrWizardChart.PrimaryYAxis.ValueType = (ChartValueType)cbbxYaxisValueType.SelectedItem;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Chart Control", MessageBoxButtons.OK, MessageBoxIcon.Error);
				chcrWizardChart.PrimaryYAxis.ValueType = type;
			}

			if( chcrWizardChart.PrimaryYAxis.ValueType == ChartValueType.DateTime )
			{
				txtbxYAxisFormat.Text = chcrWizardChart.PrimaryYAxis.DateTimeFormat;
			}
			else
			{
				txtbxYAxisFormat.Text = chcrWizardChart.PrimaryYAxis.Format;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnYAxisEditLabels_Click(object sender, System.EventArgs e)
		{
			this.ShowPropertyEditor(chcrWizardChart.PrimaryYAxis, "Labels", chcrWizardChart.PrimaryYAxis.Labels);

			if (chcrWizardChart.PrimaryYAxis.Labels.Count > 0
				&& (chcrWizardChart.PrimaryYAxis.TickLabelsDrawingMode & ChartAxisTickLabelDrawingMode.UserMode) == ChartAxisTickLabelDrawingMode.None)
			{
				chcrWizardChart.PrimaryYAxis.TickLabelsDrawingMode |= ChartAxisTickLabelDrawingMode.UserMode;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtbxXAxisFormat_TextChanged(object sender, System.EventArgs e)
		{
			if( chcrWizardChart.PrimaryXAxis.ValueType == ChartValueType.DateTime )
			{
				string format = chcrWizardChart.PrimaryXAxis.DateTimeFormat;

				try
				{
					DateTime.Now.ToString(txtbxXAxisFormat.Text);
					chcrWizardChart.PrimaryXAxis.DateTimeFormat = txtbxXAxisFormat.Text;
				}
				catch
				{
					chcrWizardChart.PrimaryXAxis.DateTimeFormat = format;
				}
			}
			else
			{
				string format = chcrWizardChart.PrimaryXAxis.Format;

				try
				{
					double.MinValue.ToString(txtbxXAxisFormat.Text);
					chcrWizardChart.PrimaryXAxis.Format = txtbxXAxisFormat.Text;
				}
				catch
				{
					chcrWizardChart.PrimaryXAxis.Format = format;
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void txtbxYAxisFormat_TextChanged(object sender, System.EventArgs e)
		{
			if( chcrWizardChart.PrimaryYAxis.ValueType == ChartValueType.DateTime )
			{
				string format = chcrWizardChart.PrimaryYAxis.DateTimeFormat;

				try
				{
					DateTime.Now.ToString(txtbxYAxisFormat.Text);
					chcrWizardChart.PrimaryYAxis.DateTimeFormat = txtbxYAxisFormat.Text;
				}
				catch
				{
					chcrWizardChart.PrimaryYAxis.DateTimeFormat = format;
				}
			}
			else
			{
				string format = chcrWizardChart.PrimaryYAxis.Format;

				try
				{
					double.MinValue.ToString(txtbxYAxisFormat.Text);
					chcrWizardChart.PrimaryYAxis.Format = txtbxYAxisFormat.Text;
				}
				catch
				{
					chcrWizardChart.PrimaryYAxis.Format = format;
				}
			}		
		}
		#endregion

		#region	ToolBar Tab
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void clrbxToolBarBackColor_ColorChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ToolBar.BackColor = clrbxToolBarBackColor.Color;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void clrbxToolBarBorderColor_ColorChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ToolBar.Border.ForeColor = clrbxToolBarBorderColor.Color;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bttnToolBarEditItems_Click(object sender, System.EventArgs e)
		{
			this.ShowPropertyEditor(chcrWizardChart.ToolBar, "Items", chcrWizardChart.ToolBar.Items);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxChartToolBarVisible_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ShowToolbar = chbxChartToolBarVisible.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nmupChartToolBarButtonSize_ValueChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ToolBar.ButtonSize = new Size((int)nmupChartToolBarButtonWidth.Value, (int)nmupChartToolBarButtonHeight.Value);

			nmupChartToolBarButtonWidth.Value = chcrWizardChart.ToolBar.ButtonSize.Width;
			nmupChartToolBarButtonHeight.Value = chcrWizardChart.ToolBar.ButtonSize.Height;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartToolBarBorderStyle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ToolBar.Border.DashStyle = (DashStyle)cbbxChartToolBarBorderStyle.SelectedItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxChartToolBarShowBorder_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ToolBar.ShowBorder = chbxChartToolBarShowBorder.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nmudChartToolBarBorderWidth_ValueChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ToolBar.Border.Width = (float)nmudChartToolBarBorderWidth.Value;
		}
		#endregion

		#region Points Tab
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnPointsLabelsChanged(object sender, EventArgs args)
		{
			this.ApplyPointsParameters();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPointLabelsSerieToApplyChnaged(object sender, System.EventArgs e)
		{
			this.SetPointsParameters(chcrWizardChart);
		}
		#endregion

		#region	Legend Tab
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxChartLegendVisible_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.ShowLegend = chbxChartLegendVisible.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartLegendPosition_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.Position = (ChartDock)cbbxChartLegendPosition.SelectedItem;

			lbCgartLegendOrientation.Visible = chcrWizardChart.Legend.Position == ChartDock.Floating;
			cbbxChartLegendOrientation.Visible = chcrWizardChart.Legend.Position == ChartDock.Floating;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartLegendAlignment_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.Alignment = (ChartAlignment)cbbxChartLegendAlignment.SelectedItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxChartLegendOrientation_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.Orientation = (ChartOrientation)cbbxChartLegendOrientation.SelectedItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void clrbxLegendBorderColor_ColorChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.Border.ForeColor = clrbxLegendBorderColor.Color;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void nmudChartLegendBorderWidth_ValueChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.Border.Width = (float)nmudChartLegendBorderWidth.Value;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cbbxLegendDashStyle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.Border.DashStyle = (DashStyle)cbbxChartLegendBorderDashStyle.SelectedItem;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void chbxChartLegendShowBorder_CheckedChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.ShowBorder = chbxChartLegendShowBorder.Checked;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bibxLegendBackInterior_BrushInfoChanged(object sender, System.EventArgs e)
		{
			chcrWizardChart.Legend.BackInterior = bibxLegendBackInterior.BrushInfo;
		}
		#endregion
		#endregion

		#region Helper methdos
		/// <summary>
		/// Adds the default series.
		/// </summary>
		private void EnsureSeriesAdded()
		{
			if (chcrWizardChart.Series.Count == 0)
			{
				int count = ChartPredefinedValues.GetSeriesCount(m_chartSeriesType);

				for (int i = 0; i < count; i++)
				{
					ChartSeries series = new ChartSeries("Default" + i);
					this.ApplyTypeSettings(series, m_chartSeriesType);

					if (m_chartSeriesType == ChartSeriesType.Bubble && i % 2 == 0)
					{
						series.ConfigItems.BubbleItem.BubbleType = ChartBubbleType.Square;
					}

					chcrWizardChart.Series.Add(series);
				}

				this.UpdateSeriesCollection(chcrWizardChart);
			}
		}
		/// <summary>
		/// Filters the random series.
		/// </summary>
		/// <param name="seriesList">The series list.</param>
		/// <returns></returns>
		private ChartSeries[] FilterRandomSeries(IList seriesList)
		{
			ArrayList list = new ArrayList(seriesList.Count);

			foreach (ChartSeries series in seriesList)
			{
				if (!ChartControl.IsRandomSeries(series))
				{
					list.Add(series);
				}
			}

			return (ChartSeries[])list.ToArray(typeof(ChartSeries));
		}
		/// <summary>
		/// Users the initialize component.
		/// </summary>
		private void UserInitializeComponent()
		{
			this.Icon = ChartWizardResources.WizardIcon;
			this.BackgroundImage = ChartWizardResources.WizardBackImage;

			bttnNext.NormalImage = ChartWizardResources.ButtonNextNormalImage;
			bttnNext.HighlightImage = ChartWizardResources.ButtonNextSelectedImage;

			bttnPrevious.NormalImage = ChartWizardResources.ButtonPrevNormalImage;
			bttnPrevious.HighlightImage = ChartWizardResources.ButtonPrevSelectedImage;

			bttnApply.NormalImage = ChartWizardResources.ButtonAlternativeNormal;
			bttnCancel.NormalImage = ChartWizardResources.ButtonAlternativeNormal;
			bttnOk.NormalImage = ChartWizardResources.ButtonAlternativeNormal;

			bttnApply.SelectedImage = ChartWizardResources.ButtonAlternativeSelected;
			bttnCancel.SelectedImage = ChartWizardResources.ButtonAlternativeSelected;
			bttnOk.SelectedImage = ChartWizardResources.ButtonAlternativeSelected;

			bttnTabToolBar.TagIndex = c_tabToolBarIndex;
			bttnTabLegend.TagIndex = c_tabLegendIndex;
			bttnTabSeries.TagIndex = c_tabSeriesIndex;
			bttnTabChartStyle.TagIndex = c_tabChartTypeIndex;
			bttnTabAppearance.TagIndex = c_tabAppearanceIndex;
			bttnTabPoints.TagIndex = c_tabPoinsIndex;
			bttnTabAxes.TagIndex = c_tabAxesIndex;

			object obj = Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Area);

			lsbxColorPalette.Items.Add(ChartAppearanceStyles.NoneFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.BlackFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.ContrastFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.DefaultFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.GainsBoroFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.GradientFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.LightOliveFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.LinenFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.MistyRoseFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.PaleYellowFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.PinkOverlayFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.SolidColorFormat);
			lsbxColorPalette.Items.Add(ChartAppearanceStyles.TriColorFormat);

			lsbxColorPalette.SelectedItem = ChartAppearanceStyles.DefaultFormat;

			cbbxChartBorderStyle.DataSource = Enum.GetValues(typeof(BorderStyle));
			cbbxChartBorderStyle.SelectedItem = BorderStyle.None;

			cbbxChartTitlePosition.DataSource = Enum.GetValues(typeof(ChartTextPosition));
			cbbxChartTitlePosition.SelectedItem = ChartTextPosition.Top;

			cbbxTitleAlignment.DataSource = Enum.GetValues(typeof(StringAlignment));
			cbbxTitleAlignment.SelectedItem = StringAlignment.Center;

			cbbxXAxisValueType.DataSource = Enum.GetValues(typeof(ChartValueType));
			cbbxXAxisValueType.SelectedItem = ChartValueType.Double;

			cbbxYaxisValueType.DataSource = Enum.GetValues(typeof(ChartValueType));
			cbbxYaxisValueType.SelectedItem = ChartValueType.Double;

			cbbxChartLegendPosition.DataSource = Enum.GetValues(typeof(ChartDock));
			cbbxChartLegendPosition.SelectedItem = ChartDock.Right;

			cbbxChartLegendAlignment.DataSource = Enum.GetValues(typeof(ChartAlignment));
			cbbxChartLegendAlignment.SelectedItem = ChartAlignment.Center;

			cbbxChartLegendOrientation.DataSource = Enum.GetValues(typeof(ChartOrientation));
			cbbxChartLegendOrientation.SelectedItem = ChartOrientation.Horizontal;

			cbbxChartToolBarButtonStyle.DataSource = Enum.GetValues(typeof(FlatStyle));
			cbbxChartToolBarButtonStyle.SelectedItem = FlatStyle.Popup;
			cbbxChartToolBarButtonStyle.Visible = false;

			cbbxChartToolBarBorderStyle.DataSource = Enum.GetValues(typeof(DashStyle));
			cbbxChartToolBarBorderStyle.SelectedItem = DashStyle.Solid;

			cbbxChartLegendBorderDashStyle.DataSource = Enum.GetValues(typeof(DashStyle));
			cbbxChartLegendBorderDashStyle.SelectedItem = DashStyle.Solid;

			cbbxSeriesPointsType.DataSource = Enum.GetValues(typeof(ChartSeriesType));
			cbbxSeriesPointsType.SelectedItem = ChartSeriesType.Line;

			cbbxSeriesDataType.DataSource = Enum.GetValues(typeof(ChartSeriesType));
			cbbxSeriesDataType.SelectedItem = ChartSeriesType.Line;

			cbbxXAxisIntersectAction.DataSource = Enum.GetValues(typeof(ChartLabelIntersectAction));
			cbbxXAxisIntersectAction.SelectedItem = ChartLabelIntersectAction.None;

			ImageList iconsImageList = new ImageList();
			iconsImageList.ImageSize = new Size(16, 16);

			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Line));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Area));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Column));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Bar));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Bubble));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Kagi));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Pie));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.Polar));
			iconsImageList.Images.Add(Utils.ChartSeriesTypeImages.GetImage(ChartSeriesType.BoxAndWhisker));

			cbbxChartTypeGroups.ImageList = iconsImageList;
			cbbxChartTypeGroups.SelectedIndex = 0;
			SelectPseudoTab(0);

			checkAutoRun.Checked = ChartControlDesigner.AutoRunWizard;

			chcrWizardChart.Enabled = false;
		}
		/// <summary>
		/// Search's all members in the data source.
		/// </summary>
		/// <param name="context">Instance of the BindingContext.</param>
		/// <param name="source">Data source.</param>
		/// <param name="dataMember">Member of the data source.</param>
		/// <returns>The array of members from the data source.</returns>
		private string[] FindDataMembers(BindingContext context, object source, string dataMember)
		{
			ArrayList result = new ArrayList();
			PropertyDescriptorCollection members = (dataMember == "") ? context[source].GetItemProperties()
				: context[source, dataMember].GetItemProperties();

			for (int i = 0; i < members.Count; i++)
			{
				result.Add(members[i].Name);
			}

			return (string[])result.ToArray(typeof(string));
		}
		/// <summary>
		/// Copies and applies settings from the chart.
		/// </summary>
		/// <param name="chart">Incatnse of the ChartControl</param>
		private void SetChart(ChartControl chart)
		{
			m_chart = chart;

			this.CopyProperties(chart, chcrWizardChart);

			this.UpdateDataSources(chart);
			this.SetChartParameters(chcrWizardChart);
			this.SetLegendParameters(chart);
			this.SetToolBarParameters(chart);
			this.SetAxesParameters(chart);
			this.SetPointsParameters(chart);

			this.UpdateSeriesCollection(chcrWizardChart);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="chart"></param>
		private void SetChartParameters(ChartControl chart)
		{
			chbxIs3D.Checked = chart.Series3D;
			brushInfoBox2.BrushInfo = chart.BackInterior;
			brushInfoBox3.BrushInfo = chart.ChartArea.BackInterior;
			cbbxChartBorderStyle.SelectedItem = chart.ChartArea.BorderStyle;
			colorBox2.Color = chart.ChartArea.BorderColor;
			txtbxChartTitleText.Text = chart.Text;
			cbbxChartTitlePosition.SelectedItem = chart.TextPosition;
			cbbxTitleAlignment.SelectedItem = chart.TextAlignment;
			colorBox1.Color = chart.Title.ForeColor;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="chart"></param>
		private void SetAxesParameters(ChartControl chart)
		{
			sabxXAxisTitleAlignment.StringAlignment = chart.PrimaryXAxis.TitleAlignment;
			cbbxXAxisIntersectAction.SelectedItem = chart.PrimaryXAxis.LabelIntersectAction;
			chbxXAxisGridLine.Checked = chart.PrimaryXAxis.DrawGrid;
			chbxXAxisInversed.Checked = chart.PrimaryXAxis.Inversed;
			chbxXAxisOpposed.Checked = chart.PrimaryXAxis.OpposedPosition;
			txtbxXAxisTitle.Text = chart.PrimaryXAxis.Title;
			cbbxXAxisValueType.SelectedItem = chart.PrimaryXAxis.ValueType;

			if (chart.PrimaryXAxis.ValueType == ChartValueType.DateTime)
			{
				txtbxXAxisFormat.Text = chart.PrimaryXAxis.DateTimeFormat;
			}
			else
			{
				txtbxXAxisFormat.Text = chart.PrimaryXAxis.Format;
			}

			sabxYAxisTitleAlignment.StringAlignment = chart.PrimaryYAxis.TitleAlignment;
			chbxYAxisGridLine.Checked = chart.PrimaryYAxis.DrawGrid;
			chbxYAxisInversed.Checked = chart.PrimaryYAxis.Inversed;
			chbxYAxisOpposed.Checked = chart.PrimaryYAxis.OpposedPosition;
			txtbxYAxisTitle.Text = chart.PrimaryYAxis.Title;
			cbbxYaxisValueType.SelectedItem = chart.PrimaryYAxis.ValueType;

			if (chart.PrimaryYAxis.ValueType == ChartValueType.DateTime)
			{
				txtbxYAxisFormat.Text = chart.PrimaryYAxis.DateTimeFormat;
			}
			else
			{
				txtbxYAxisFormat.Text = chart.PrimaryYAxis.Format;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="chart"></param>
		private void SetToolBarParameters(ChartControl chart)
		{
			clrbxToolBarBackColor.Color = chart.ToolBar.BackColor;
			clrbxToolBarBorderColor.Color = chart.ToolBar.Border.ForeColor;
			chbxChartToolBarVisible.Checked = chart.ToolBar.Visible;
			nmupChartToolBarButtonWidth.Value = chart.ToolBar.ButtonSize.Width;
			nmupChartToolBarButtonHeight.Value = chart.ToolBar.ButtonSize.Height;
			nmudChartToolBarBorderWidth.Value = (decimal)chart.ToolBar.Border.Width;
			cbbxChartToolBarBorderStyle.SelectedItem = chart.ToolBar.Border.DashStyle;
			chbxChartToolBarShowBorder.Checked = chart.ToolBar.ShowBorder;
		}
		/// <summary>
		/// Sets the legend parameters.
		/// </summary>
		/// <param name="chart">The chart.</param>
		private void SetLegendParameters(ChartControl chart)
		{
			chbxChartLegendVisible.Checked = chart.Legend.Visible;
			cbbxChartLegendPosition.SelectedItem = chart.Legend.Position;
			cbbxChartLegendAlignment.SelectedItem = chart.Legend.Alignment;
			cbbxChartLegendOrientation.SelectedItem = chart.Legend.Orientation;
			chbxChartLegendShowBorder.Checked = chart.Legend.ShowBorder;
			cbbxChartLegendBorderDashStyle.SelectedItem = chart.Legend.Border.DashStyle;
			clrbxLegendBorderColor.Color = chart.Legend.Border.ForeColor;
			nmudChartLegendBorderWidth.Value = (decimal)chart.Legend.Border.Width;
			bibxLegendBackInterior.BrushInfo = chart.Legend.BackInterior;
		}
		/// <summary>
		/// Sets the points parameters.
		/// </summary>
		/// <param name="chart">The chart.</param>
		private void SetPointsParameters(ChartControl chart)
		{
			ChartSeries selectedSeries = cbbxPointLabelsApplySeries.SelectedItem as ChartSeries;

			if (selectedSeries == null)
			{
				if (chart.Series.Count > 0)
				{
					this.SetPointsParameters(chart.Series[0]);
				}
			}
			else
			{
				this.SetPointsParameters(selectedSeries);
			}
		}
		/// <summary>
		/// Sets the points parameters.
		/// </summary>
		/// <param name="series">The series.</param>
		private void SetPointsParameters(ChartSeries series)
		{
			fntbxPointsLabelsFont.SelectedFont = series.Style.Font.GdipFont;
			nmudPointsLabelsRotate.Value = series.Style.Font.Orientation;
			chbxPointsLabelsShow.Checked = series.Style.DisplayText;
			clrbxPointsLabelsColor.Color = series.Style.TextColor;
			orbxPointsLabelsAlignment.Orientation = series.Style.TextOrientation;
		}
		/// <summary>
		/// Applies the points parameters.
		/// </summary>
		private void ApplyPointsParameters()
		{
			ChartSeries selectedSeries = cbbxPointLabelsApplySeries.SelectedItem as ChartSeries;

			if (selectedSeries == null)
			{
				foreach (ChartSeries series in chcrWizardChart.Series)
				{
					this.ApplyPointsParameters(series);
				}
			}
			else
			{
				this.ApplyPointsParameters(selectedSeries);
			}

			chcrWizardChart.Redraw(true);
		}
		/// <summary>
		/// Applies the points parameters.
		/// </summary>
		/// <param name="series">The series.</param>
		private void ApplyPointsParameters(ChartSeries series)
		{
			Font font = fntbxPointsLabelsFont.SelectedFont;

			if (font.FontFamily == null)
			{
				series.Style.Font.Facename = font.Name;
			}
			else
			{
				series.Style.Font.FontFamilyTemplate = font.FontFamily;
			}

			series.Style.Font.FontStyle = font.Style;
			series.Style.Font.Size = font.Size;
			series.Style.Font.Unit = font.Unit;
			series.Style.Font.Orientation = (int)nmudPointsLabelsRotate.Value;

			series.Style.DisplayText = chbxPointsLabelsShow.Checked;
			series.Style.TextColor = clrbxPointsLabelsColor.Color;
			series.Style.TextOrientation = orbxPointsLabelsAlignment.Orientation;
		}
		/// <summary>
		/// Updates the data sources.
		/// </summary>
		/// <param name="chart">The chart.</param>
		private void UpdateDataSources(ChartControl chart)
		{
			cbbxDataSources.Items.Clear();
			cbbxDataSources.Items.Add(c_noneNamedObject);

#if SyncfusionFramework2_0
			cbbxDataSources.Items.Add(c_newBindingSourceNamedObject);
#else
			cbbxDataSources.Items.Add(c_newOleDataAdapterNamedObject);
			cbbxDataSources.Items.Add(c_newSqlDataAdapterNamedObject);
#endif

			if (chart.Site != null)
			{
				foreach (IComponent comp in chart.Site.Container.Components)
				{
					if ((comp is IListSource) || (comp is IEnumerable))
					{
						cbbxDataSources.Items.Add(new NamedObject(comp.Site.Name, comp));
					}
				}

				cbbxChartSeriesXValue.Enabled = true;
				cbbxChartSeriesYValue.Enabled = true;
				cbbxDataSources.Enabled = true;
			}
			else
			{
				cbbxChartSeriesXValue.Enabled = false;
				cbbxChartSeriesYValue.Enabled = false;
				cbbxDataSources.Enabled = false;
			}

            cbbxDataSources.SelectedItem = string.IsNullOrEmpty(m_chart.DataSourceName) ? c_noneNamedObject : getDataSource(cbbxDataSources.Items, m_chart.DataSourceName);
			this.UpdateDataMembers("");
		}
		/// <summary>
		/// Updates the data members.
		/// </summary>
		private void UpdateDataMembers( string baseMember )
		{
			m_curentDataMember = baseMember;
			m_curentDataSource = (cbbxDataSources.SelectedItem as NamedObject).Tag;
			dtgdDataSource.DataSource = m_curentDataSource;

			cbbxChartSeriesXValue.Items.Clear();
			cbbxChartSeriesYValue.Items.Clear();

			cbbxChartSeriesXValue.Items.Add(c_noneName);
			cbbxChartSeriesYValue.Items.Add(c_noneName);

			if (dtgdDataSource.DataSource != null)
			{
				string[] dataMembers = this.FindDataMembers(this.BindingContext, m_curentDataSource, m_curentDataMember);

				foreach (string member in dataMembers)
				{
					cbbxChartSeriesXValue.Items.Add(member);
					cbbxChartSeriesYValue.Items.Add(member);
				}
			}

			cbbxChartSeriesXValue.SelectedItem = c_noneName;
			cbbxChartSeriesYValue.SelectedItem = c_noneName;
		}
		/// <summary>
		/// Updates the series collection.
		/// </summary>
		/// <param name="chart">The chart.</param>
		private void UpdateSeriesCollection(ChartControl chart)
		{
			ChartSeries[] filtredSeries = this.FilterRandomSeries(chart.Series);

			cbbxPointLabelsApplySeries.Items.Clear();
			cbbxChartSeries.Items.Clear();
			comboBox1.Items.Clear();

			cbbxPointLabelsApplySeries.DisplayMember = "Name";
			cbbxChartSeries.DisplayMember = "Name";
			comboBox1.DisplayMember = "Name";

			cbbxPointLabelsApplySeries.Items.Add(c_allName);
			cbbxChartSeries.Items.Add(c_noneName);
			comboBox1.Items.Add(c_noneName);

			for (int i = 0; i < filtredSeries.Length; i++)
			{
				cbbxPointLabelsApplySeries.Items.Add(filtredSeries[i]);
				cbbxChartSeries.Items.Add(filtredSeries[i]);
				comboBox1.Items.Add(filtredSeries[i]);
			}

			cbbxPointLabelsApplySeries.SelectedItem = c_allName;
			cbbxChartSeries.SelectedItem = c_noneName;
			comboBox1.SelectedItem = c_noneName;
		}
		/// <summary>
		/// Ensures the data bind model present.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		private bool EnsureDataBindModelPresent(ChartSeries series)
		{
			if (series != null)
			{
				if (series.SeriesModel is ChartDataBindModel)
				{
					return true;
				}
				else
				{
					if (MessageBox.Show(this, ChartWizardResources.RemovePointsMessage, ChartWizardResources.ModelChangeTitle, 
						MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						series.SeriesModel = new ChartDataBindModel();
						return true;
					}

					return false;
				}
			}

			return false;
		}
		/// <summary>
		/// Ensures the series model present.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <returns></returns>
		private bool EnsureSeriesModelPresent(ChartSeries series)
		{
			if (series != null)
			{
				if (series.SeriesModel is ChartSeriesModel)
				{
					return true;
				}
				else
				{
					if (MessageBox.Show(this, ChartWizardResources.RemoveBindingMessage, ChartWizardResources.ModelChangeTitle,
						MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						series.SeriesModel = new ChartSeriesModel();
						return true;
					}

					return false;
				}
			}

			return false;
		}
		/// <summary>
		/// Selects the pseudo tab.
		/// </summary>
		/// <param name="index">The index.</param>
		private void SelectPseudoTab(int index)
		{
			m_currentTabIndex = index;

			gradientPanel2.Visible = index == c_tabChartTypeIndex;
			tbcrSeries.Visible = index == c_tabSeriesIndex;
			tbcrAppearance.Visible = index == c_tabAppearanceIndex;

			tbcrAxes.Visible = index == c_tabAxesIndex;

			if (tbcrAxes.Visible)
			{
				grbxEditXAxis.Visible = chcrWizardChart.RequireAxes;
				grbxEditYAxis.Visible = chcrWizardChart.RequireAxes;
				lblDoesntUseXAxis.Visible = !chcrWizardChart.RequireAxes;
				lblDoesntUseYAxis.Visible = !chcrWizardChart.RequireAxes;
			}

			tbcrChartLegend.Visible = index == c_tabLegendIndex;
			tbcrChartToolBar.Visible = index == c_tabToolBarIndex;
			pnPoints.Visible = index == c_tabPoinsIndex;

			bttnTabChartStyle.IsPushed = index == bttnTabChartStyle.TagIndex;
			bttnTabSeries.IsPushed = index == bttnTabSeries.TagIndex;
			bttnTabAppearance.IsPushed = index == bttnTabAppearance.TagIndex;
			bttnTabAxes.IsPushed = index == bttnTabAxes.TagIndex;
			bttnTabLegend.IsPushed = index == bttnTabLegend.TagIndex;
			bttnTabToolBar.IsPushed = index == bttnTabToolBar.TagIndex;
			bttnTabPoints.IsPushed = index == bttnTabPoints.TagIndex;

			if (index != c_tabChartTypeIndex)
			{
				this.EnsureSeriesAdded();
			}

			switch (index)
			{
				case c_tabChartTypeIndex:
					lbTitle.Text = ChartWizardResources.ChartTypeTitle;
					break;

				case c_tabSeriesIndex:
					lbTitle.Text = ChartWizardResources.SeriesPointsTitle;
					break;

				case c_tabAppearanceIndex:
					lbTitle.Text = ChartWizardResources.PaletteTitle;
					break;

				case c_tabAxesIndex:
					lbTitle.Text = ChartWizardResources.AxesTitle;
					break;

				case c_tabLegendIndex:
					lbTitle.Text = ChartWizardResources.LegendTitle;
					break;

				case c_tabPoinsIndex:
					lbTitle.Text = ChartWizardResources.PointsTitle;
					break;

				case c_tabToolBarIndex:
					lbTitle.Text = ChartWizardResources.ToolBarTitle;
					break;
			}

			chcrWizardChart.Visible = ShowChartPreview();
			chcrWizardChart.BringToFront();
		}
		/// <summary>
		/// Selects the chart type group.
		/// </summary>
		/// <param name="index">The index.</param>
		private void SelectChartTypeGroup(int index)
		{
			lsvwSeriesTypes.Items.Clear();
			lsvwSeriesTypes.LargeImageList = this.SeriesTypeThumbs;

			switch (index)
			{
				case 0:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Line));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Spline));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.RotatedSpline));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StepLine));
					break;

				case 1:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Area));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.RangeArea));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StackingArea));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StackingArea100));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.SplineArea));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StepArea));
					break;

				case 2:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Column));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.ColumnRange));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StackingColumn));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StackingColumn100));
					break;

				case 3:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Bar));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StackingBar));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.StackingBar100));
					break;

				case 4:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Bubble));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Scatter));
					break;

				case 5:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Candle));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.HiLo));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.HiLoOpenClose));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Kagi));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.PointAndFigure));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Renko));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.ThreeLineBreak));
					break;

				case 6:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Pie));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Funnel));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Pyramid));
					break;

				case 7:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Polar));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Radar));
					break;

				case 8:
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.BoxAndWhisker));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Gantt));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Histogram));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.Tornado));
					lsvwSeriesTypes.Items.Add(CreateThumb(ChartSeriesType.HeatMap));
					break;
			}
		}
		/// <summary>
		/// Initializes the thumbs.
		/// </summary>
		private void InitializeThumbs()
		{
			if (m_seriesTypeThumbs == null)
			{
				m_seriesTypeThumbs = new ImageList();
				m_seriesTypeThumbs.ColorDepth = ColorDepth.Depth32Bit;
			}

			m_seriesTypeThumbs.Images.Clear();
			m_seriesTypeThumbs.ImageSize = c_thumbTypeSize;

			ChartSeriesType[] types = Enum.GetValues(typeof(ChartSeriesType)) as ChartSeriesType[];

			using (ChartControl chart = new DesignChartControl())
			{
				chart.Width = c_thumbTypeSize.Width;
				chart.Height = c_thumbTypeSize.Height;
				chart.Depth = 16;
				//chart.Palette = lsbxColorPalette.SelectedItem != null ?
				//	(ChartColorPalette)lsbxColorPalette.SelectedItem : ChartColorPalette.Default;
				chart.ShowLegend = false;
				chart.ToolBar.Visible = false;
				chart.ElementsSpacing = 0;
				chart.PrimaryXAxis.TickLabelsDrawingMode = ChartAxisTickLabelDrawingMode.None;
				chart.PrimaryYAxis.TickLabelsDrawingMode = ChartAxisTickLabelDrawingMode.None;
				chart.ElementsSpacing = 0;
				chart.AddRandomSeries = false;
				chart.Series3D = chbxIs3D.Checked;

				foreach (ChartSeriesType stype in types)
				{
					int seriesCount = ChartPredefinedValues.GetSeriesCount(stype);

					chart.Series.Clear();
					chart.BeginUpdate();

					for (int si = 0; si < seriesCount; si++)
					{
						ChartSeries series = new ChartSeries("series" + si.ToString());
						this.ApplyTypeSettings(series, stype);

						if (stype == ChartSeriesType.Bubble && si % 2 == 0)
						{
							series.ConfigItems.BubbleItem.BubbleType = ChartBubbleType.Square;
						}

						chart.Series.Add(series);
					}

					chart.EndUpdate();

					Bitmap bmp = new Bitmap(c_thumbTypeSize.Width, c_thumbTypeSize.Height);

					chart.Draw(bmp);
					m_seriesTypeThumbs.Images.Add(bmp);
				}
			}
		}
		/// <summary>
		/// Creates the random seris.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private ChartSeries CreateRandomSeris( string name )
		{
			ChartSeries series = new ChartSeries(name);

			series.Text = name;

			for (int i = 0; i < c_randomSeriesPointCount; i++)
			{
			  DoubleRange y12 = new DoubleRange(c_random.Next(c_randomSeriesMaxY1), c_random.Next(c_randomSeriesMaxY2));
			  double y3 = c_random.Next((int)y12.Start, (int)y12.End);
			  double y4 = c_random.Next((int)y12.Start, (int)y12.End);

			  series.Points.Add(i, y12.Start, y12.End, y3, y4);
			}

			return series;
		}
		/// <summary>
		/// Creates the thumb.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		private ListViewItem CreateThumb(ChartSeriesType type)
		{
			ListViewItem item = new ListViewItem(type.ToString(), (int)type);
			item.Tag = type;
			return item;
		}
		/// <summary>
		/// Copies the properties.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="output">The output.</param>
		private void CopyProperties(ChartControl input, ChartControl output)
		{
			output.BeginUpdate();
			input.BeginUpdate();

			if (input.ToolBar.ShouldSerializeItems())
			{
				output.ToolBar.Items.Clear();

				foreach (ChartToolBarItemBase item in input.ToolBar.Items)
				{
					output.ToolBar.Items.Add(item);
				}
			}
			else
			{
				output.ToolBar.ResetItems();
			}		

			// Chart properties
			output.BackInterior = input.BackInterior;
			output.Series3D = input.Series3D;
			output.Palette = input.Palette;
			output.CustomPalette = input.CustomPalette;
			output.Text = input.Text;
			output.TextAlignment = input.TextAlignment;
			output.TextPosition = input.TextPosition;

			this.CopyPropertyValue(input, output, "ForeColor");
			this.CopyPropertyValue(input, output, "ShowLegend");
			this.CopyPropertyValue(input, output, "ShowToolbar");
			this.CopyPropertyValue(input, output, "ChartInterior");

			// Title properties
			this.CopyPropertyValue(input.Title, output.Title, "ForeColor");

			// Axes properties
			output.PrimaryXAxis.LabelIntersectAction = input.PrimaryXAxis.LabelIntersectAction;
			output.PrimaryXAxis.DrawGrid = input.PrimaryXAxis.DrawGrid;
			output.PrimaryXAxis.HidePartialLabels = input.PrimaryXAxis.HidePartialLabels;
			output.PrimaryXAxis.TickLabelsDrawingMode = input.PrimaryXAxis.TickLabelsDrawingMode;
			output.PrimaryXAxis.OpposedPosition = input.PrimaryXAxis.OpposedPosition;
			output.PrimaryXAxis.Inversed = input.PrimaryXAxis.Inversed;
			output.PrimaryXAxis.Title = input.PrimaryXAxis.Title;
			output.PrimaryXAxis.TitleAlignment = input.PrimaryXAxis.TitleAlignment;
			output.PrimaryXAxis.ValueType = input.PrimaryXAxis.ValueType;
			output.PrimaryXAxis.Format = input.PrimaryXAxis.Format;
			output.PrimaryXAxis.DateTimeFormat = input.PrimaryXAxis.DateTimeFormat;
			output.PrimaryXAxis.GridLineType.ForeColor = input.PrimaryXAxis.GridLineType.ForeColor;
			output.PrimaryXAxis.LineType.ForeColor = input.PrimaryXAxis.LineType.ForeColor;
			output.PrimaryXAxis.TickColor = input.PrimaryXAxis.TickColor;
            output.PrimaryXAxis.TitleColor = input.PrimaryXAxis.TitleColor;
            output.PrimaryXAxis.TitleFont = input.PrimaryXAxis.TitleFont;

			this.CopyPropertyValue(output.PrimaryXAxis, input.PrimaryXAxis, "ForeColor");

			output.PrimaryYAxis.DrawGrid = input.PrimaryYAxis.DrawGrid;
			output.PrimaryYAxis.HidePartialLabels = input.PrimaryYAxis.HidePartialLabels;
			output.PrimaryYAxis.TickLabelsDrawingMode = input.PrimaryYAxis.TickLabelsDrawingMode;
			output.PrimaryYAxis.OpposedPosition = input.PrimaryYAxis.OpposedPosition;
			output.PrimaryYAxis.Inversed = input.PrimaryYAxis.Inversed;
			output.PrimaryYAxis.Title = input.PrimaryYAxis.Title;
			output.PrimaryYAxis.TitleAlignment = input.PrimaryYAxis.TitleAlignment;
			output.PrimaryYAxis.ValueType = input.PrimaryYAxis.ValueType;
			output.PrimaryYAxis.Format = input.PrimaryYAxis.Format;
			output.PrimaryYAxis.DateTimeFormat = input.PrimaryYAxis.DateTimeFormat;
			output.PrimaryYAxis.GridLineType.ForeColor = input.PrimaryYAxis.GridLineType.ForeColor;
			output.PrimaryYAxis.LineType.ForeColor = input.PrimaryYAxis.LineType.ForeColor;
			output.PrimaryYAxis.TickColor = input.PrimaryYAxis.TickColor;
            output.PrimaryYAxis.TitleColor = input.PrimaryYAxis.TitleColor;
            output.PrimaryYAxis.TitleFont = input.PrimaryYAxis.TitleFont;

			this.CopyPropertyValue(output.PrimaryYAxis, input.PrimaryYAxis, "ForeColor");

			// ChartArea properties
			this.CopyPropertyValue(input.ChartArea, output.ChartArea, "BackInterior");
			this.CopyPropertyValue(input.ChartArea, output.ChartArea, "BorderStyle");
			this.CopyPropertyValue(input.ChartArea, output.ChartArea, "BorderColor");

			// Legend properties
			output.Legend.Visible = input.Legend.Visible;
			output.Legend.Position = input.Legend.Position;
			output.Legend.Alignment = input.Legend.Alignment;
			output.Legend.Orientation = input.Legend.Orientation;
			output.Legend.BackInterior = input.Legend.BackInterior;
			output.Legend.Border.BackColor = input.Legend.Border.BackColor;
			output.Legend.Border.DashStyle = input.Legend.Border.DashStyle;
			output.Legend.Border.Width = input.Legend.Border.Width;

			// ToolBar properties
			output.ToolBar.Visible = input.ToolBar.Visible;
			output.ToolBar.Position = input.ToolBar.Position;
			output.ToolBar.Alignment = input.ToolBar.Alignment;
			output.ToolBar.Orientation = input.ToolBar.Orientation;
			output.ToolBar.Border.ForeColor = input.ToolBar.Border.ForeColor;
			output.ToolBar.Border.BackColor = input.ToolBar.Border.BackColor;
			output.ToolBar.Border.DashStyle = input.ToolBar.Border.DashStyle;
			output.ToolBar.Border.Width = input.ToolBar.Border.Width;
			output.ToolBar.ButtonSize = input.ToolBar.ButtonSize;
			output.ToolBar.BackColor = input.ToolBar.BackColor;

			// Series 
			output.Series.Clear();

			for (int i = 0; i < input.Series.Count; i++)
			{
				if (!ChartControl.IsRandomSeries(input.Series[i]))
				{
					ChartSeries inputSeries = input.Series[i];
					ChartSeries onputSeries = new ChartSeries();

					output.Series.Add(onputSeries);

					onputSeries.Name = inputSeries.Name;
					onputSeries.Text = inputSeries.Text;
					onputSeries.Type = inputSeries.Type;
					onputSeries.SeriesModel = inputSeries.SeriesModel;

					this.CopyPropertyValue(inputSeries.Style.Font, onputSeries.Style.Font, "FontStyle");
					this.CopyPropertyValue(inputSeries.Style.Font, onputSeries.Style.Font, "Size");
					this.CopyPropertyValue(inputSeries.Style.Font, onputSeries.Style.Font, "Unit");
					this.CopyPropertyValue(inputSeries.Style.Font, onputSeries.Style.Font, "Orientation");

					this.CopyPropertyValue(inputSeries.Style.Border, onputSeries.Style.Border, "Width");

					this.CopyPropertyValue(inputSeries.Style, onputSeries.Style, "DisplayText");
					this.CopyPropertyValue(inputSeries.Style, onputSeries.Style, "TextColor");
					this.CopyPropertyValue(inputSeries.Style, onputSeries.Style, "TextOrientation");
					this.CopyPropertyValue(inputSeries.Style, onputSeries.Style, "DisplayShadow");
				}
			}

			// Axes labels
			if (output.PrimaryXAxis.Labels != null)
			{
				output.PrimaryXAxis.Labels.Clear();

				foreach (ChartAxisLabel xlabel in input.PrimaryXAxis.Labels)
				{
					output.PrimaryXAxis.Labels.Add(xlabel);
				}

				if (output.PrimaryXAxis.Labels.Count > 0
					&& (output.PrimaryXAxis.TickLabelsDrawingMode & ChartAxisTickLabelDrawingMode.UserMode) == ChartAxisTickLabelDrawingMode.None)
				{
					output.PrimaryXAxis.TickLabelsDrawingMode |= ChartAxisTickLabelDrawingMode.UserMode;
				}
			}

			if (output.PrimaryYAxis.Labels != null)
			{
				output.PrimaryYAxis.Labels.Clear();

				foreach (ChartAxisLabel ylabel in input.PrimaryYAxis.Labels)
				{
					output.PrimaryYAxis.Labels.Add(ylabel);
				}

				if (output.PrimaryYAxis.Labels.Count > 0
					&& (output.PrimaryYAxis.TickLabelsDrawingMode & ChartAxisTickLabelDrawingMode.UserMode) == ChartAxisTickLabelDrawingMode.None)
				{
					output.PrimaryYAxis.TickLabelsDrawingMode |= ChartAxisTickLabelDrawingMode.UserMode;
				}
			}

			input.EndUpdate();
			output.EndUpdate();
		}
		/// <summary>
		/// Shows the property editor.
		/// </summary>
		/// <param name="component">The component.</param>
		/// <param name="property">The property.</param>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private object ShowPropertyEditor(object component, string property, object value)
		{
			PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(component)[property];

			if (propDescriptor != null)
			{
				UITypeEditor uiEditor = propDescriptor.GetEditor(typeof(UITypeEditor)) as UITypeEditor;

				if (uiEditor != null)
				{
					TypeDescriptorContext typeDescriptorContext = new TypeDescriptorContext(value, propDescriptor);
					typeDescriptorContext.ServiceProvider = m_chart.Site;
					WindowsFormsEditorServiceContainer windowsFormsEditorServiceContainer = new WindowsFormsEditorServiceContainer(m_chart.Site);

					value = uiEditor.EditValue(typeDescriptorContext, windowsFormsEditorServiceContainer, value);
					propDescriptor.SetValue(component, value);
				}
			}

			return value;
		}
		/// <summary>
		/// Creates the new OLEDB data adapter.
		/// </summary>
		/// <param name="chart">The chart.</param>
		private void CreateNewOleDataAdapter(ChartControl chart)
		{
			OleDbDataAdapter adapter = new OleDbDataAdapter();
			chart.Site.Container.Add(adapter);

			IDesignerHost designerHost = chart.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			IDesigner designer = designerHost.GetDesigner(adapter);

			designer.Verbs[0].Invoke();

			if (adapter.SelectCommand != null || adapter.SelectCommand.CommandText != "")
			{
				designer.Verbs[1].Invoke();
			}

			this.UpdateDataSources(chart);

			cbbxDataSources.SelectedItem = c_noneNamedObject;
		}
		/// <summary>
		/// Creates the new SQL data adapter.
		/// </summary>
		/// <param name="chart">The chart.</param>
		private void CreateNewSqlDataAdapter(ChartControl chart)
		{
			if (chart.Site != null)
			{
				SqlDataAdapter adapter = new SqlDataAdapter();
				chart.Site.Container.Add(adapter);

				IDesignerHost designerHost = chart.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
				IDesigner designer = designerHost.GetDesigner(adapter);

				designer.Verbs[0].Invoke();

				if (adapter.SelectCommand != null || adapter.SelectCommand.CommandText != "")
				{
					designer.Verbs[1].Invoke();
				}

				this.UpdateDataSources(chart);
			}
		}
#if SyncfusionFramework2_0
		/// <summary>
		/// Creates the new binding source.
		/// </summary>
		/// <param name="chart">The chart.</param>
		private void CreateNewBindingSource(ChartControl chart)
		{
			if (chart.Site != null)
			{
				IDesignerHost designerHost = chart.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
				DataSourceProviderService dataSourceProviderService = chart.Site.GetService(typeof(DataSourceProviderService)) as DataSourceProviderService;
				DataSourceGroup dsGroup = dataSourceProviderService.InvokeAddNewDataSource(this, FormStartPosition.CenterScreen);

				if (dsGroup != null && dsGroup.DataSources.Count > 0)
				{
					object dataSource = dataSourceProviderService.AddDataSourceInstance(designerHost, dsGroup.DataSources[0]);

					if (dataSource is DataSet)
					{
						DataSet dataSet = dataSource as DataSet;

						if (dataSet.Tables.Count > 0)
						{
							BindingSource bindingSource = new BindingSource(dataSet, dataSet.Tables[0].TableName);

							chart.Site.Container.Add(bindingSource);
							dataSourceProviderService.NotifyDataSourceComponentAdded(bindingSource);
						}
					}

					this.UpdateDataSources(chart);
				}
			}
		}
#endif

		/// <summary>
		/// Copies the property value.
		/// </summary>
		/// <param name="component1">The component1.</param>
		/// <param name="component2">The component2.</param>
		/// <param name="propertyName">Name of the property.</param>
		private void CopyPropertyValue(object component1, object component2, string propertyName)
		{
			PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(component1)[propertyName];

			if (object.Equals(propertyDescriptor.GetValue(component1),propertyDescriptor.GetValue(component2)))
			{ 
			}
			else if (propertyDescriptor.ShouldSerializeValue(component1))
			{
				propertyDescriptor.SetValue(component2, propertyDescriptor.GetValue(component1));
			}
			else if( propertyDescriptor.ShouldSerializeValue(component2))
			{
				propertyDescriptor.ResetValue(component2);
			}
		}
		/// <summary>
		/// Applies the type settings.
		/// </summary>
		/// <param name="series">The series.</param>
		/// <param name="type">The type.</param>
		private void ApplyTypeSettings(ChartSeries series, ChartSeriesType type )
		{
			series.Type = type;

			switch (type)
			{
				case ChartSeriesType.Line:
				case ChartSeriesType.Spline:
				case ChartSeriesType.RotatedSpline:
				case ChartSeriesType.StepLine:
				case ChartSeriesType.HiLo:
				case ChartSeriesType.HiLoOpenClose:
				case ChartSeriesType.Kagi:
					series.Style.Border.Width = 2;
					series.Style.DisplayShadow = true;
					break;

				case ChartSeriesType.Gantt:
					series.ConfigItems.GanttItem.DrawMode = ChartGanttDrawMode.AutoSizeMode;
					series.Style.Border.ResetWidth();
					series.Style.ResetDisplayShadow();
					break;

				case ChartSeriesType.Histogram:
					series.ConfigItems.HistogramItem.ShowNormalDistribution = true;
					series.Style.Border.ResetWidth();
					series.Style.ResetDisplayShadow();
					break;

				case ChartSeriesType.Scatter:
				case ChartSeriesType.Column:
				case ChartSeriesType.Bar:
				case ChartSeriesType.StackingBar:
				case ChartSeriesType.Area:
				case ChartSeriesType.RangeArea:
				case ChartSeriesType.SplineArea:
				case ChartSeriesType.StackingArea:
				case ChartSeriesType.StackingColumn:
				case ChartSeriesType.StackingArea100:
				case ChartSeriesType.StackingBar100:
				case ChartSeriesType.StackingColumn100:
				case ChartSeriesType.Pie:
				case ChartSeriesType.Funnel:
				case ChartSeriesType.Pyramid:
				case ChartSeriesType.Candle:
				case ChartSeriesType.Bubble:
				case ChartSeriesType.StepArea:
				case ChartSeriesType.Radar:
				case ChartSeriesType.Renko:
				case ChartSeriesType.Polar:
				case ChartSeriesType.ColumnRange:
				case ChartSeriesType.ThreeLineBreak:
				case ChartSeriesType.PointAndFigure:
				case ChartSeriesType.BoxAndWhisker:
				case ChartSeriesType.Tornado:
				case ChartSeriesType.Custom:
					series.Style.Border.ResetWidth();
					series.Style.ResetDisplayShadow();
					break;
			}
		}
		#endregion

		#region Public methods
		/// <summary>
		/// Shows the wizard.
		/// </summary>
		/// <param name="chart">The chart.</param>
		public void ShowWizard(ChartControl chart)
		{
			this.SetChart(chart);
			this.ShowDialog();

			if (chart.Site != null)
			{
			  IComponentChangeService changeService = chart.Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;

			  if (changeService != null)
			  {
			    changeService.OnComponentChanged(chart, null, null, null);
			  }
			}
		}
		#endregion
	}
}
