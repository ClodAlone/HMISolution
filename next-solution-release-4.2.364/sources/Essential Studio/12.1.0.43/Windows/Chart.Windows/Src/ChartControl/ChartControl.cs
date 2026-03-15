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

#region File using derectives

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Syncfusion.Documentation;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Chart;
using Syncfusion.Windows.Forms.Chart.Design;
using Syncfusion.Windows.Forms.Chart.Renderers;
using Syncfusion.Windows.Forms.Chart.Scrolling;
using Syncfusion.Windows.Forms.Chart.Utils;
using ComponentModel_TypeConverter = System.ComponentModel.TypeConverter;
using System.IO;
using System.Globalization;

#endregion

namespace Syncfusion.Windows.Forms.Chart
{
    /// <summary>
    /// Indicates allowed actions with chart.
    /// </summary>
    public enum ChartMouseAction
    {
        /// <summary>
        /// The zooming feature is enabled.
        /// </summary>
        Zooming,

        /// <summary>
        /// The panning feature is enabled.
        /// </summary>
        Panning
    }

    /// <summary>
    /// ChartControl is the root class of the chart control.
    /// </summary>
    /// <remarks>
    /// <para>The ChartControl lets you create enterprise level charts with support for numerous chart types (see <see cref="ChartSeries.Type"/>).
    /// A <see cref="ChartSeries"/> containing pints of X and Y values can be added to the <see cref="Series"/> collection to get a chart
    /// rendered. The chart control has a built in "nice range calculation" engine which will automatically determine the appropriate
    /// ranges for the axes based on the series data. Chart readily supports rendering multiple series.</para>
    /// <para>
    /// There is also support for multiple axes, in case you want to render multiple series with each series rendered against a unique axes.
    /// Use the <see cref="Axes"/> collection to add one or more custom axis to the default set of <see cref="PrimaryXAxis"/> and <see cref="PrimaryYAxis"/>,
    /// which also can be referred from the Axes collection. Numerous customization can be done on the axis labels through the 
    /// corresponding <see cref="ChartAxis"/> instance.
    /// </para>
    /// <para>
    /// The default <see cref="Legend"/> renders some default <see cref="ChartLegendItem"/>s representing each series in the chart.
    /// You can add custom legends to the chart through the <see cref="Legends"/> collection. You can add custom legend items to the 
    /// <see cref="ChartLegend"/> instance through the <see cref="ChartLegend.CustomItems"/> collection.
    /// </para>
    /// <para>
    /// There is also a default <see cref="Title"/> and the ability to add multiple titles through the <see cref="Titles"/> collection.
    /// </para>
    /// <para>The look and feel of the plot can also be customized through the <see cref="ChartArea"/> property.</para>
    /// <para>
    /// There is also built in support for applying some statistical formulas and functions to the series data. Take a look
    /// at the <see cref="Syncfusion.Windows.Forms.Chart.Statistics.BasicStatisticalFormulas"/> type and the <see cref="Syncfusion.Windows.Forms.Chart.Statistics.UtilityFunctions"/> type.4
    /// </para>
    /// </remarks>
    [ToolboxBitmap(typeof(ChartControl), "ToolboxIcons.chartcontrol.bmp")]
    [Designer(typeof(ChartControlDesigner), typeof(IDesigner))]
    [Description("Displays a windows form chart with other elements like axes, series")]
    public class ChartControl : Control, IChartAreaHost
    {
        #region Internal types
        /// <summary>
        /// The Enumerator InternalMouseAction decides the Mouse action.
        /// </summary>
        enum InternalMouseAction
        {
            /// <summary>
            /// The Mouse Action is None.
            /// </summary>
            None,

            /// <summary>
            /// The Mouse Action is Zooming.
            /// </summary>
            Zooming,

            /// <summary>
            /// The Mouse Action is Panning.
            /// </summary>
            Panning,

            /// <summary>
            /// The Mouse Action is CursorMoving.
            /// </summary>
            CursorMoving
        }
        #endregion

        #region Constants
        private static readonly BrushInfo c_defBackInterior = new BrushInfo(Color.White);
        private static readonly BrushInfo c_defShadowInterior = new BrushInfo(155, new BrushInfo(Color.DarkGray));
        private const string c_ismMessage = "Invalid mode is not supported because of the GDI+ behavior";
        private const string c_internalSeriesName = "677132InternalSeries";
        private const int c_emptyIndex = -1;

        private const int c_randomSeriesCount = 1;
        private const int c_randomSeriesPointCount = 6;
        private const int c_randomSeriesMaxY1 = 400;
        private const int c_randomSeriesMaxY2 = 100;
        private const int c_randomSeriesMaxY3 = 300;
        private const int c_randomSeriesMaxY4 = 400;
        private ChartPoint CursorLocation;

        private const double m_minimalZoomFactor = 0.00001;
        private static readonly Cursor c_horizontalCursorCursor = Cursors.SizeNS;
        private static readonly Cursor c_verticalCursorCursor = Cursors.SizeWE;
        private static readonly Cursor c_panningCloseCursor = new Cursor(typeof(ChartControl), "Resources.HandClose.cur");

        private readonly static Cursor c_panningOpenCursor = new Cursor(typeof(ChartControl), "Resources.HandOpen.cur");

        internal const string c_defaultTitleName = "Default";
        #endregion

        #region Members
        private double m_minPointsDelta = double.NaN;
        private double oldPointX=double.NaN;
        private double oldPointY = double.NaN;
        private double newPointX = double.NaN;
        private double newPointY = double.NaN;
        private double mPointX = double.NaN;
        private double mPointY = double.NaN;
        private double cPointX=double.NaN;
        private double cPointY = double.NaN;
        /// <summary>
        /// Store value indicates that zooming is updated after mouse up.
        /// </summary>
        private bool m_bInZoomTransaction = false;

        private Image m_canvas;
        private IntPtr m_internalHdc = IntPtr.Zero;
        private bool m_isWindowLess = false;
        private bool m_style3D = false;

        /// <summary>
        /// The AspMode.
        /// </summary>
        /// <internalonly/>
        [DocumentationExclude()]
        [Obsolete("This member isn't used anymore.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static bool AspMode = false;

        private Timer m_toolTipTimer = new Timer();

        private bool m_idleTime = true;
        private bool showToolbarInImage = true;
        private InternalMouseAction m_currectMouseAction = InternalMouseAction.None;
        private MouseEventArgs m_oldMouseArgs = new MouseEventArgs(MouseButtons.None, 0, 0, 0, 0);

        private ChartRegion m_activeRegion = null;
        private bool m_needRecalculateSizes = true;
        private bool m_allowUserEditStyles = false;
        private bool m_autoHighlight = false;
        private bool m_seriesHighlight = false;
        private bool m_highlightSymbol = false;
        public int m_seriesHighlightIndex = -1;
        private bool m_calcRegions = true;
        private ChartArea m_chartArea;
        private bool chartAreaShadow = false;
        private ChartRegionCollection m_chartRegions = new ChartRegionCollection();
        private string chartToolTip = string.Empty;
        private Point clickPoint = Point.Empty;

        private ChartBorderInfo m_borderAppearance = new ChartBorderInfo();

        private ChartColumnDrawMode columnDrawMode = ChartColumnDrawMode.InDepthMode;
        private ChartColumnWidthMode columnWidthMode = ChartColumnWidthMode.DefaultWidthMode;
        private Skins skins = Skins.None;
        private Color BackInterior1;
        private Color BackInterior2;
        private Color ChartAreaInterior1;
        private Color ChartAreaInterior2;
        private ColorConverter ColorConv = new ColorConverter();
        private bool m_compatibleSeries = true;
        private ChartSeriesType designTimeSeriesType = ChartSeriesType.Column;
        private int m_elementsSpacing = 10;
        private bool m_enableXZooming = false;
        private bool m_enableYZooming = false;
        private bool m_inRandomDataMode = false;

        private ChartToolBar m_toolbar;
        private bool keyZoom = false;
        private ChartLegendsList m_legends = new ChartLegendsList();
        private ChartLegend m_defaultLegend = null;
        private double m_minZoomFactorX = 0.01;
        private double m_minZoomFactorY = 0.01;
        private ChartModel m_model;
        private MouseButtons m_lastMouseButton = MouseButtons.None;
        private Point mouseDownPosition;
        private bool m_needRegionUpdate = true;
        private ChartFancyToolTipController m_fancyToolTipController;

        private ChartMouseAction m_mouseAction = ChartMouseAction.Zooming;
        private ChartPrintDocument m_printDocument;
        private bool printing;
        private bool disposeStarted = false;
        private ChartRadarAxisStyle radarStyle = ChartRadarAxisStyle.Polygon;
        private int roundingPlaces = 2;
        private Hashtable scrollBarH = new Hashtable(5);
        private Hashtable scrollBarV = new Hashtable(5);
        private BrushInfo m_shadowInterior = c_defShadowInterior.Clone();
        private int shadowWidth = 5;
        private bool improvePerformance = false;
        private bool m_needPerformance = false;
        private bool showToolTips = false;
        private SmoothingMode m_smoothingMode = SmoothingMode.AntiAlias;
        private ToolTipAdv m_toolTip;
        private bool updating = false;
        private Size virtualSize;
        private ChartZooming m_zoomBarInfo = new ChartZooming();
        private double m_zoomOutIncrement = 0.2;
        private int m_scrollPrecision = 100;
        private bool m_enableMouseRotation = false;
        private bool m_invertedSeriesIsCompatible = false;

        private Keys m_zoomLeft = Keys.Left;
        private Keys m_zoomRight = Keys.Right;
        private Keys m_zoomUp = Keys.Up;
        private Keys m_zoomDown = Keys.Down;
        private Keys m_zoomIn = Keys.Add;
        private Keys m_zoomOut = Keys.Subtract;
        private Keys m_zoomCancel = Keys.Escape;

        private TextRenderingHint m_textRenderingHint = TextRenderingHint.SystemDefault;
        private ChartInteractiveCursor m_interactiveCursorDown = null;
        private bool dropSeriesPoints = false;

        // popup menus
        private ChartSeriesContextMenu m_seriesContextMenu = null;
        private bool m_displaySeriesContextMenu = true;

        private ChartContextMenu m_chartContextMenu = null;
        private bool m_displayChartContextMenu = true;
        private bool m_showContextMenu = false;
        private bool m_showContextMenuInLegend = false;
        private bool m_showScrollBars = true;

        // Need to maintain a separate copy and keep this in sync with base.BackInterior to prevent designer from persisting default value
        private BrushInfo m_backInterior = new BrushInfo(c_defBackInterior);
        private ChartDockingManager m_dockingManager;
        private ChartDockingManager m_titleDockingManager;
        private ChartDockingManager m_toolBarDockingManager;

        private Image m_buffer = null;
        private bool m_needRedraw = true;

        private bool m_addRandomData = false;
        private Cursor m_baseCursor = null;
        private bool m_axesSeriesAndChartShouldBePrepared = true;

        private ChartTitlesList m_titles = null;
        private ChartTitle m_defaultTitle = null;

        private ChartStyleDialogOptions m_styleDialogOptions = new ChartStyleDialogOptions();

        private RectangleF m_borderBounds = RectangleF.Empty;
        private bool m_defaultBackgroundPainting = false;
        private string m_DataSourceName=string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of data source bound with chart control in ChartWizard
        /// </summary>
        public string DataSourceName
        {
            get 
            {
                return m_DataSourceName;
            }
            set 
            {
                m_DataSourceName = value;
            }
        }
        /// <summary>
        /// Gets or sets the background image displayed in the control.
        /// </summary>
        public override Image BackgroundImage
        {
            get
            {
                return m_defaultBackgroundPainting ? null : base.BackgroundImage;
            }

            set
            {
                base.BackgroundImage = value;
                this.Refresh();
            }
        }
        /// <summary>
        /// Gets or sets the localize.
        /// </summary>
        /// <value>The localize.</value>
        /// <example>
        /// The following example shows how to use the <b>Localize </b>property
        /// <para></para>
        /// <code lang="C#">GridGroupinControl grid = new GridGroupingControl();
        ///  grid.TableDescriptor.Localize="fr-FR";</code>
        /// </example>
        private string m_localize;
        public string Localize
        {
            get
            {
                return m_localize;
            }
            set
            {
                m_localize = value;
                this.localization = null;
                this.m_chartContextMenu =null;
               
            }
        }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        /// <value>The resource location.</value>
        /// <example>
        /// The following example shows how to use the <b>LocalizationPath </b>property
        /// <para></para>
        /// <code lang="C#">GridGroupinControl grid = new GridGroupingControl();
        ///  grid.TableDescriptor.LocalizationPath="~/App_LocalResources";</code>
        /// </example>
        private string m_localizePath;
        internal string LocalizationPath
        {
            get
            {
                return m_localizePath;
            }
            set
            {
                m_localizePath = value;
            }
        }

        private ChartLocalizationStrings localization;
        protected internal ChartLocalizationStrings Localization
        {
            get
            {
                if (localization == null)
                {
                    if (string.IsNullOrEmpty(Localize))
                        localization = new ChartLocalizationStrings(System.Threading.Thread.CurrentThread.CurrentCulture);
                    else if (string.IsNullOrEmpty(this.LocalizationPath))
                        localization = new ChartLocalizationStrings(CultureInfo.GetCultureInfo(Localize));
                    else
                        localization = new ChartLocalizationStrings(this.LocalizationPath, CultureInfo.GetCultureInfo(Localize));

                }
                return localization;
            }
          
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chart control is working as simple object.
        /// If it's true then chart don't uses functional of the control.
        /// </summary>
        public bool IsWindowLess
        {
            get
            {
                return this.m_isWindowLess;
            }

            set
            {
                this.m_isWindowLess = value;

                if (this.m_canvas == null)               
                    this.m_canvas = new Bitmap(1, 1);
            }
        }

        /// <summary>
        /// Gets the Index of ChartRegion which was selected. For internal use.    
        /// </summary>
        ///<internalonly/>
        [DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ActiveIndex
        {
            get
            {
                if (m_activeRegion != null)
                {
                    return m_chartRegions.IndexOf(m_activeRegion);
                }

                return c_emptyIndex;
            }
        }

        /// <summary>
        /// Gets the chart area of the ChartControl.
        /// </summary>
        /// <value></value>
        [TypeConverter(typeof(ExpandableObjectConverter)), Description("Specifies the chart area hosted by the ChartControl."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.Content), Category("Appearance")]
        public ChartArea ChartArea
        {
            get
            {
                return m_chartArea;
            }
        }

        /// <summary>
        /// Gets the chart area hosted by the ChartControl.
        /// </summary>
        /// <returns>Instance of class that implements the interface <see cref="IChartArea"/></returns>
        IChartArea IChartAreaHost.GetChartArea()
        {
            return m_chartArea;
        }

        /// <summary>
        /// Gets a reference to the <see cref="ChartDockingManager"/> that manages the position of elements on the chart, like the legend.
        /// </summary>
        /// <value></value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ChartTemplate(ChartTemplateSet.Content)]
        public ChartDockingManager DockingManager
        {
            get
            {
                return m_dockingManager;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chartcontrol is selectable.
        /// </summary>
        [Description("Indicates if the chartcontrol is focussed."), DefaultValue(true), Category("Behavior")]
        public bool AcceptFocus
        {
            get
            {
                return this.GetStyle(ControlStyles.Selectable);
            }

            set
            {
                this.SetStyle(ControlStyles.Selectable, value);
                this.UpdateStyles();
            }
        }

        #region Misc
        /// <summary>
        /// Gets or sets a value indicating whether the chart requires axes.
        /// </summary>
        [Description("Specifies if the chart requires axes"), DefaultValue(true), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Axes")]        
        public bool RequireAxes
        {
            get
            {
                return this.ChartArea.RequireAxes;
            }

            set
            {
                this.ChartArea.RequireAxes = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chart requires inverted axes.
        /// </summary>
        [Description("Specifies if the chart requires inverted axes"), DefaultValue(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Axes")]        
        public bool RequireInvertedAxes
        {
            get
            {
                return this.ChartArea.RequireInvertedAxes;
            }

            set
            {
                this.ChartArea.RequireInvertedAxes = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether improve the Performance. This property will automatically set to true when BeginUpdate() is called and improve the ChartType rendering time. 
        /// So while adding a large number of points to a series, performance is improved significantly.
        /// </summary>
        [DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple), Category("Behavior")]
        [Description("Indicates the performance improvement when chart rendering ChartType")]
        public bool NeedPerformance
        {
            get
            {
                return m_needPerformance;
            }

            set
            {
                m_needPerformance = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether improve the chart performance. If set this property instructs chart to calculate axes ranges only before painting, 
        /// not at avery series adding or removing. So while adding a large number of points to a series, performance is improved
        /// significantly.
        /// </summary>
        [DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple), Category("Behavior")]
        [Description("Indicates when chart calculate the axes ranges.")]
        public bool ImprovePerformance
        {
            get
            {
                return improvePerformance;
            }

            set
            {
                improvePerformance = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow the user to edit series styles when double clicking on a series.
        /// </summary>
        [DefaultValue(false), Description("Allows the user to edit series styles when double clicking on a series."), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool AllowUserEditStyles
        {
            get
            {
                return this.m_allowUserEditStyles;
            }

            set
            {
                this.m_allowUserEditStyles = value;
            }
        }

        /// <summary>
        /// Gets or sets Chart's ForeColor. This member overrides <see cref="Control.ForeColor"/>
        /// </summary>        
        [ChartTemplate(ChartTemplateSet.Simple), Category("Appearance"), Description("Gets or sets fore color of the chart control.")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }

            set
            {
                base.ForeColor = value;
                this.Refresh();
            }
        }

        /// <summary>
        ///  Gets or sets backcolor of the chart control.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple), Category("Appearance"), Description("Gets or sets backcolor of the chart control.")]
        public BrushInfo BackInterior
        {
            get
            {
                return m_backInterior;
            }

            set
            {
                if (!m_backInterior.Equals(value))
                {
                    m_backInterior = new BrushInfo(value);
                    this.Redraw(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether calculate the regions for visual update. If ToolTips, AutoHighlighting, or RegionHit events are used, set this to True (default). If the previous aren't used,
        /// for better performance set to False.
        /// </summary>
        [Description("If ToolTips, AutoHighlighting, or RegionHit events are used set this to True (default)."), DefaultValue(true), Category("Behavior"), ChartTemplate(ChartTemplateSet.Simple)]
        public bool CalcRegions
        {
            get
            {
                return m_calcRegions;
            }

            set
            {
                if (m_calcRegions != value)
                {
                    m_calcRegions = value;
                    m_activeRegion = null;

                    if (value)
                    {
                        this.NeedRegionUpdate = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates the margins that will be deduced from the ChartArea's representation rectangle.
        /// Negative values are supported.
        /// </summary>
        [Description("Indicates the margins that will be deduced from the ChartArea's representation rectangle."), Category("Appearance")]        
        public ChartMargins ChartAreaMargins
        {
            get
            {
                return this.ChartArea.ChartAreaMargins;
            }

            set
            {
                this.ChartArea.ChartAreaMargins = value;
            }
        }

        /// <summary>
        /// Gets the list of ChartRegions. Please refer to <see cref="ChartRegion"/> for more information.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartRegionCollection ChartRegions
        {
            get { return m_chartRegions; }
        }

        /// <summary>
        /// Gets or sets the mode of drawing the column chart. See also <see cref="ColumnWidthMode"/>.
        /// </summary>
        [Description("Specifies the mode of column drawing."), Category("Appearance"), DefaultValue(ChartColumnDrawMode.InDepthMode), Browsable(true), ChartTemplate(ChartTemplateSet.Simple)]
        public ChartColumnDrawMode ColumnDrawMode
        {
            get
            {
                return columnDrawMode;
            }

            set
            {
                if (columnDrawMode != value)
                {
                    columnDrawMode = value;
                    Redraw(true);
                }
            }
        }
        [Description("Specifies the AutoFormat."), Category("Appearance"), DefaultValue(Skins.None), Browsable(true)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public Skins Skins
        {
            get
            {
                return skins;
            }

            set
            {
                if (skins != value)
                {
                    skins = value;
                    SetAutoformat();
                }
            }
        }

        private void SetAutoformat()
        {
            switch (this.Skins)
            {
                case Skins.None:
                    this.ChartInterior = new BrushInfo(Color.White);
                    this.BackInterior = new BrushInfo(Color.White);
                    this.BorderAppearance.BaseColor = Color.Gray;
                    this.BorderAppearance.Interior.ForeColor = Color.DarkGray;
                    break;
                case Skins.Office2007Blue:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FFB0D3FF");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFF0F7FF");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Office2007Black:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF898C8F");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFCECFD1");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Office2007Silver:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FFE0E0E0");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFEAEBEB");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Olive:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FFDCE593");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFF8F9EB");
                    ChartAreaInterior1 = (Color)ColorConv.ConvertFromString("#FFF8F9EB");
                    ChartAreaInterior2 = (Color)ColorConv.ConvertFromString("#FFDCE593");
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { Color.White, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Almond:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FFE9BDBD");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFFBF3F3");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Blend:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF404040");
                    this.BackInterior = new BrushInfo(BackInterior1);
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.BorderAppearance.BaseColor = BackInterior1;
                    this.ChartInterior = new BrushInfo(GradientStyle.None, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Blueberry:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF9EBDF5");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFD3E6FD");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Marble:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FFE8E0E0");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFFAFAFA");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Midnight:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF19191A");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FF545454");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Monochrome:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF88B2DC");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFDCE4EE");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Sandune:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FFE7D3AB");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFFBF8F2");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = (Color)ColorConv.ConvertFromString("#FFD8C399");
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Turquoise:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF54B8C2");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FFE0F2F2");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Vista:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF1D5D7B");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FF61BED3");
                    ChartAreaInterior1 = Color.White;
                    ChartAreaInterior2 = Color.White;
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.VS2010:
                    BackInterior1 = (Color)ColorConv.ConvertFromString("#FF4E648E");
                    BackInterior2 = (Color)ColorConv.ConvertFromString("#FF798CAB");
                    ChartAreaInterior1 = (Color)ColorConv.ConvertFromString("#FFE7DAC1");
                    ChartAreaInterior2 = (Color)ColorConv.ConvertFromString("#FF99B0D8");
                    this.ChartInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { Color.White, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior1, ChartAreaInterior2 });
                    break;
                case Skins.Metro:
                    break;

            }
            if (this.Skins != Skins.None && this.Skins != Skins.Blend && this.Skins != Skins.Metro)
            {
                this.BackInterior = new BrushInfo(GradientStyle.Vertical, new Color[] { BackInterior1, BackInterior2 });
                this.BorderAppearance.BaseColor = BackInterior1;
                this.BorderAppearance.Interior.ForeColor = BackInterior2;
            }
        }

        /// <summary>
        /// Gets or sets the mode of column drawing. See also <see cref="ColumnDrawMode"/>.
        /// </summary>
        [Description("Specifies the mode of column drawing."), Category("Appearance"), DefaultValue(ChartColumnWidthMode.DefaultWidthMode), Browsable(true)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public ChartColumnWidthMode ColumnWidthMode
        {
            get
            {
                return columnWidthMode;
            }

            set
            {
                if (columnWidthMode != value)
                {
                    columnWidthMode = value;
                    this.SeriesChanged(this, new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Reset));
                }
            }
        }

        private int columnFixedWidth = 20;

        /// <summary>
        /// Gets or sets the width of each column when <see cref="ColumnWidthMode"/> is set to FixedWidthMode.
        /// </summary>
        [Description("The width of each column when ColumnWidthMode is set to FixedWidthMode."), Category("Appearance"), DefaultValue(20), Browsable(true), ChartTemplate(ChartTemplateSet.Simple)]
        public int ColumnFixedWidth
        {
            get
            {
                return columnFixedWidth;
            }

            set
            {
                if (columnFixedWidth != value)
                {
                    columnFixedWidth = value;
                    Redraw(true);
                }
            }
        }

        /// <summary>
        /// Gets the collection of custom points. Please refer to <see cref="ChartCustomPoint"/> for more information.
        /// </summary>
        [Description("Collection of custom points to be displayed on the chart."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
        public ChartCustomPointCollection CustomPoints
        {
            get
            {
                return m_chartArea.CustomPoints;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether if a partially visible axis label should be hidden or shown. Default is false.
        /// </summary>
        [Description("Indicates if the partially visible labels are shown"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple), Obsolete("Use ChartAxis.HidePartialLabels instead."), Category("Behavior")]
        public bool HidePartialLabels
        {
            get 
            {
                return this.m_chartArea.HidePartialLabels; 
            }

            set
            {
                this.m_chartArea.HidePartialLabels = value; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the chart is Radar. This returns True if type of the first series is ChartSeriesType.Radar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool Radar
        {
            get
            {
                return Series.Count > 0 && (Series[0].Type == ChartSeriesType.Radar || Series[0].Type == ChartSeriesType.Polar);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the chart is Polar. This returns True if type of the first series is ChartSeriesType.Polar.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool Polar
        {
            get
            {
                return Series.Count > 0 && (Series[0].Type == ChartSeriesType.Polar);
            }
        }

        /// <summary>
        /// Gets or sets a value indicates the style of the radar chart.
        /// </summary>
        [Description("Indicates the style of the radar chart."), DefaultValue(ChartRadarAxisStyle.Polygon), Category("Appearance"), ChartTemplate(ChartTemplateSet.Simple)]
        public ChartRadarAxisStyle RadarStyle
        {
            get
            {
                return radarStyle;
            }

            set
            {
                if (radarStyle != value)
                {
                    radarStyle = value;

                    if (Radar)
                    {
                        RefreshArea();
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the number of places to round data to for display (by default).
        /// </summary>
        [Description("Specifies the number of places to round the data"), DefaultValue(2), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category("Behavior"), Obsolete("This property is useless")]
        public int RoundingPlaces
        {
            get
            {
                return this.roundingPlaces;
            }

            set
            {
                this.roundingPlaces = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets size with minimal height and width of chart control with enough places to draw chart area.
        /// This property must be used generally by parent control of chart control in order to determine the size margins.
        /// </summary>
        [Browsable(false), Obsolete("Use properties of ChartArea.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Size SizeMargins
        {
            get
            {
                Size size = new Size(this.Size.Width, this.Size.Height);

                size.Height -= ChartArea.RenderBounds.Height;
                size.Width -= ChartArea.RenderBounds.Width;

                return size;
            }
        }

        /// <summary>
        /// Gets or sets the position of text displayed as the chart's caption. Please refer to <see cref="ChartTextPosition"/> for
        /// available options.
        /// </summary>
        [Description("The position of text displayed as the chart's caption."), Category("Appearance"), DefaultValue(ChartTextPosition.Top), ChartTemplate(ChartTemplateSet.Simple)]
        public ChartTextPosition TextPosition
        {
            get
            {
                if (m_defaultTitle != null)
                    if (!m_defaultTitle.IsDisposed)
                        switch (m_defaultTitle.Position)
                        {
                            case ChartDock.Left:
                                return ChartTextPosition.Left;
                            case ChartDock.Right:
                                return ChartTextPosition.Right;
                            case ChartDock.Top:
                                return ChartTextPosition.Top;
                            case ChartDock.Bottom:
                                return ChartTextPosition.Bottom;
                        }

                return ChartTextPosition.Top;
            }

            set
            {
                if (m_defaultTitle != null && !m_defaultTitle.IsDisposed)
                {
                    switch (value)
                    {
                        case ChartTextPosition.Top:
                            m_defaultTitle.Position = ChartDock.Top;
                            break;
                        case ChartTextPosition.Bottom:
                            m_defaultTitle.Position = ChartDock.Bottom;
                            break;
                        case ChartTextPosition.Left:
                            m_defaultTitle.Position = ChartDock.Left;
                            break;
                        case ChartTextPosition.Right:
                            m_defaultTitle.Position = ChartDock.Right;
                            break;
                    }

                    this.Redraw(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the chart should be displayed in a 3D plane.
        /// </summary>
        [Description("Specifies if the chart is to displayed in 3D plane."), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple), Category("Behavior")]
        public bool RealMode3D
        {
            get
            {
                return m_chartArea == null ? false : m_chartArea.RealSeries3D;
            }

            set
            {
                if (m_chartArea != null)
                {
                    m_chartArea.RealSeries3D = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse rotation should be enabled.
        /// </summary>
        [Description("Specifies if the mouse rotation is to enabled."), DefaultValue(false), Category("Behavior")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool EnableMouseRotation
        {
            get
            {
                return m_enableMouseRotation;
            }

            set
            {
                m_enableMouseRotation = value;

                if (value)
                {
                    DisplayChartContextMenu = false;
                    DisplaySeriesContextMenu = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the inverted series is compatible
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool InvertedSeriesIsCompatible
        {
            get
            {
                return m_invertedSeriesIsCompatible;
            }

            set
            {
                if (m_invertedSeriesIsCompatible != value)
                {
                    m_invertedSeriesIsCompatible = value;
                    Redraw(true);
                }
            }
        }
        
        /// <summary>
        /// Gets the list of <see cref="ChartTitle"/> entries. The first one is the default <see cref="Title"/>.
        /// </summary>
        [Description("Collection of titles."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
        public ChartTitlesList Titles
        {
            get
            {
                return m_titles;
            }
        }

        /// <summary>
        /// Gets the default title.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartTitle Title
        {
            get
            {
                return m_defaultTitle;
            }
        }

        /// <summary>
        /// Gets the collection of controls contained within the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Windows.Forms.Control.ControlCollection"></see> representing the collection of controls contained within the control.</returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public ControlCollection Controls
        {
            get
            {
                return base.Controls;
            }
        }

        /// <summary>
        /// Gets or sets the background color for the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Drawing.Color"></see> that represents the background color of the control. The default is the value of the <see cref="P:System.Windows.Forms.Control.DefaultBackColor"></see> property.</returns>
        /// <PermissionSet><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]       
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }

            set
            {
                base.BackColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the border appearance.
        /// </summary>
        /// <value>The border appearance.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Content)] 
        public ChartBorderInfo BorderAppearance
        {
            get 
            {
                return m_borderAppearance; 
            }

            set
            {
                m_borderAppearance = value; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether random series will be added.
        /// </summary>
        /// <value><c>true</c> if random series will be added; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Category("Data")]
        [Description("Gets or sets a value indicating whether random series will be added.")]        
        public bool AddRandomSeries
        {
            get
            {
                return m_addRandomData;
            }

            set
            {
                if (m_addRandomData != value)
                {
                    m_addRandomData = value;
                    this.SeriesChanged(this, new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Reset));
                }
            }
        }

        /// <summary>
        /// Gets the style dialog options.
        /// </summary>
        /// <value>The style dialog options.</value>
        [Description("Specifies the options of style editor dialog.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartStyleDialogOptions StyleDialogOptions
        {
            get 
            {
                return m_styleDialogOptions; 
            }
        }

        /// <summary>
        /// Gets the fancy tool tip controller.
        /// </summary>
        /// <value>The fancy tool tip controller.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartFancyToolTipController FancyToolTipController
        {
            get
            {
                return m_fancyToolTipController;
            }
        }

        /// <summary>
        /// Gets or sets the cursor that is displayed when the mouse pointer is over the control.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Windows.Forms.Cursor"/> that represents the cursor to display when the mouse pointer is over the control.</returns>
        /// <PermissionSet>
        ///     <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///     <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        ///     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        ///     <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override Cursor Cursor
        {
            get
            {
                return base.Cursor;
            }

            set
            {
                if (m_baseCursor != null)
                {
                    m_baseCursor = value;
                }
                else
                {
                    base.Cursor = value;
                }
            }
        }
        #endregion

        #region Zooming

        /// <summary>
        /// Gets or sets a value that Enables or Disables zooming with keyboard shortcuts.
        /// </summary>
        [Description(" Enables zooming with keyboard shortcuts."), Category("Zooming"), DefaultValue(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool KeyZoom
        {
            get
            {
                return keyZoom;
            }

            set
            {
                keyZoom = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowable zooming factor for X axis.
        /// </summary>
        [Description("The minimum allowable zooming factor on the X axis."), Category("Zooming"), DefaultValue(0.01)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double MinZoomFactorX
        {
            get
            {
                return m_minZoomFactorX;
            }

            set
            {
                if (m_minZoomFactorX != value)
                {
                    m_minZoomFactorX = Math.Max(value, m_minimalZoomFactor);

                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis.Orientation == ChartOrientation.Horizontal)
                        {
                            axis.CenteredZoom(Math.Max(axis.ZoomFactor, value));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum allowable zooming factor for Y axis.
        /// </summary>
        [Description("The minimum allowable zooming factor on the Y axis."), Category("Zooming"), DefaultValue(0.01)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double MinZoomFactorY
        {
            get
            {
                return m_minZoomFactorY;
            }

            set
            {
                if (m_minZoomFactorY != value)
                {
                    m_minZoomFactorY = Math.Max(value, m_minimalZoomFactor);

                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis.Orientation == ChartOrientation.Vertical)
                        {
                            axis.CenteredZoom(Math.Max(axis.ZoomFactor, value));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the ZoomFactor. The zooming factor is the value by which intervals are multiplied to have the effect of a more fine grained range.
        /// For example, if the calculated interval is 5 and the zoom factor is 0.5, then the zoomed interval with this zoom factor
        /// will be 2.5. Therefore, there will be twice as many interval points across the same range. Scrollbars will automatically
        /// appear to allow any section of the zoomed range to be viewed.
        /// </summary>
        [Browsable(false), DefaultValue(1.0d)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
         public double ZoomFactorX
        {
            get
            {
                return m_chartArea == null ? 1d : m_chartArea.PrimaryXAxis.ZoomFactor;
            }

            set
            {
                if (m_chartArea != null)
                {
                    if (value < m_minZoomFactorX)
                    {
                        value = m_minZoomFactorX;
                    }

                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis.Orientation == ChartOrientation.Horizontal)
                        {
                            axis.CenteredZoom(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The zooming factor is the value by which intervals are multiplied to have the effect of a more fine grained range.
        /// For example, if the calculated interval is 5 and the zoom factor is 0.5, then the zoomed interval with this zoom factor
        /// will be 2.5. Therefore, there will be twice as many interval points across the same range. Scrollbars will automatically
        /// appear to allow any section of the zoomed range to be viewed.
        /// </summary>
        [Browsable(false), DefaultValue(1.0d)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double ZoomFactorY
        {
            get
            {
                return m_chartArea == null ? 1d : m_chartArea.PrimaryYAxis.ZoomFactor;
            }

            set
            {
                if (m_chartArea != null)
                {
                    if (value < m_minZoomFactorY)
                    {
                        value = m_minZoomFactorY;
                    }

                    foreach (ChartAxis axis in this.Axes)
                    {
                        if (axis.Orientation == ChartOrientation.Vertical)
                        {
                            axis.CenteredZoom(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating the chart is currently zoomed in.
        /// </summary>
        [Description("Returns True if the chart is currently zoomed in."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Zooming")]
        [ChartTemplate(ChartTemplateSet.ContentBehavior)]
        public ChartZooming Zooming
        {
            get
            {
                return m_zoomBarInfo;
            }
        }

        /// <summary>
        /// Gets or sets what value to add to the ZoomFactors when the zoom out button is pressed.
        /// </summary>
        [Description("Indicates what value to multiply to the ZoomFactors when the zoom out button is pressed."), Category("Zooming"), DefaultValue(0.2d)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double ZoomOutIncrement
        {
            get
            {
                return m_zoomOutIncrement;
            }

            set
            {
                m_zoomOutIncrement = value;
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the offset position when zoomed along the X axis.
        /// </summary>
        [Browsable(false), DefaultValue(0.0), Description("Gets or sets offset position when zoomed along the X axis.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double ZoomPositionX
        {
            get
            {
                return this.PrimaryXAxis.ZoomPosition;
            }

            set
            {
                if (this.PrimaryXAxis.ZoomPosition != value)
                {
                    this.PrimaryXAxis.ZoomPosition = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the offset position when zoomed along the Y axis.
        /// </summary>
        [Browsable(false), DefaultValue(0.0), Description("Gets or sets offset position when zoomed along the Y axis.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public double ZoomPositionY
        {
            get
            {
                return this.PrimaryYAxis.ZoomPosition;
            }

            set
            {
                if (this.PrimaryYAxis.ZoomPosition != value)
                {
                    this.PrimaryYAxis.ZoomPosition = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the scroll precision.  Specifies the scrolling accuracy of the scroll bars.
        /// </summary>
        /// <value>The scroll precision.</value>
        [Description("Specifies the scrolling accuracy of the scroll bars."), DefaultValue(100), Category("Behavior")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public int ScrollPrecision
        {
            get
            {
                return m_scrollPrecision;
            }

            set
            {
                if (m_scrollPrecision != value)
                {
                    m_scrollPrecision = value;

                    for (int i = 0; i < this.Axes.Count; i++)
                    {
                        this.ZoomingChanged(Axes[i]);
                    }

                    this.Redraw(true);
                }
            }
        }

        #region ZoomKeys
        /// <summary>
        /// Gets or sets the keyboard shortcut to control zoom left
        /// </summary>
        [Description("Specifies the keyboard shortcut to control Zoom left."), Category("Zooming"), DefaultValue(Keys.Left)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public Keys ZoomLeft
        {
            get
            {
                return m_zoomLeft;
            }

            set
            {
                m_zoomLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets the zoom right keyboard shortcut..
        /// </summary>
        /// <value>The zoom right.</value>
        [Description("Specifies the keyboard shortcut to control Zoom right."), Category("Zooming"), DefaultValue(Keys.Right)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public Keys ZoomRight
        {
            get
            {
                return m_zoomRight;
            }

            set
            {
                m_zoomRight = value;
            }
        }

        /// <summary>
        /// Gets or sets the zoom up keyboard shortcut.
        /// </summary>
        /// <value>The zoom up.</value>
        [Description("Specifies the keyboard shortcut to control Zoom up."), Category("Zooming"), DefaultValue(Keys.Up)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public Keys ZoomUp
        {
            get
            {
                return m_zoomUp;
            }

            set
            {
                m_zoomUp = value;
            }
        }

        /// <summary>
        /// Gets or sets the zoom down. Specifies the keyboard shortcut to control Zoom down.
        /// </summary>
        /// <value>The zoom down.</value>
        [Description("Specifies the keyboard shortcut to control Zoom down."), Category("Zooming"), DefaultValue(Keys.Down)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public Keys ZoomDown
        {
            get
            {
                return m_zoomDown;
            }

            set
            {
                m_zoomDown = value;
            }
        }

        /// <summary>
        /// Gets or sets the zoom in. Specifies the keyboard shortcut to control Zoom in.
        /// </summary>
        /// <value>The zoom in.</value>
        [Description("Specifies the keyboard shortcut to control Zoom in."), Category("Zooming"), DefaultValue(Keys.Add)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public Keys ZoomIn
        {
            get
            {
                return m_zoomIn;
            }

            set
            {
                m_zoomIn = value;
            }
        }

        /// <summary>
        /// Gets or sets the zoom out. Specifies the keyboard shortcut to control zoom out.
        /// </summary>
        /// <value>The zoom out.</value>
        [Description("Specifies the keyboard shortcut to control Zoom out."), Category("Zooming"), DefaultValue(Keys.Subtract)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public Keys ZoomOut
        {
            get
            {
                return m_zoomOut;
            }

            set
            {
                m_zoomOut = value;
            }
        }

        /// <summary>
        /// Gets or sets the zoom cancel. Specifies the keyboard shortcut to control ZoomCancel.
        /// </summary>
        /// <value>The zoom cancel.</value>
        [Description("Specifies the keyboard shortcut to control ZoomCancel."), Category("Zooming"), DefaultValue(Keys.Escape)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public Keys ZoomCancel
        {
            get
            {
                return m_zoomCancel;
            }

            set
            {
                m_zoomCancel = value;
            }
        }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether show the scroll bars.
        /// </summary>
        /// <value><c>true</c> if [show scroll bars]; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Category("Appearance"), Description("Specifies if the scrollbars is displayed.")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool ShowScrollBars
        {
            get
            {
                return m_showScrollBars;
            }

            set
            {
                if (m_showScrollBars != value)
                {
                    m_showScrollBars = value;
                    ChangeScrollBars(m_showScrollBars);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mouse action.
        /// </summary>
        /// <value>The mouse action.</value>
        [DefaultValue(ChartMouseAction.Zooming), Description("Mouse action"), Category("Zooming")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public ChartMouseAction MouseAction
        {
            get
            {
                return m_mouseAction;
            }

            set
            {
                m_mouseAction = value;
            }
        }
        #endregion

        #region Legend
        /// <summary>
        /// Gets a collection of <see cref="ChartLegend"/>s that are shown in the chart. The first one is the default <see cref="Legend"/>.
        /// </summary>
        [Description("Collection of legends."), Category("Legend"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartLegendsList Legends
        {
            get
            {
                return m_legends;
            }
        }

        /// <summary>
        /// Gets the configuration information for the default legend object. Please refer to <see cref="ChartLegend"/> for more information.
        /// </summary>
        [Description("Configuration information for the default legend object."), Category("Legend"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.Content)]
        public ChartLegend Legend
        {
            get
            {
                return m_defaultLegend;
            }
        }

        /// <summary>
        /// Gets or sets the position of the legend element. Please refer to <see cref="ChartLegend.Position"/> for more information
        /// on legend positioning options. Default is Right.
        /// </summary>
        [Description("Configuration information for the legend object."), Category("Legend")]
        [DefaultValue(ChartDock.Right)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public ChartDock LegendPosition
        {
            get
            {
                return m_defaultLegend.Position;
            }

            set
            {
                m_defaultLegend.Position = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether display the ChartLegend or not.
        /// </summary>
        [Description("Specifies if the legend is displayed."), Category("Legend"), DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowLegend
        {
            get
            {
                return m_defaultLegend == null ? false : m_defaultLegend.Visible;
            }

            set
            {
                if (m_defaultLegend != null)
                {
                    m_defaultLegend.Visible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the legend alignment.
        /// </summary>
        [Description("Gets or sets the legend alignment."), Category("Legend"), DefaultValue(ChartAlignment.Near), ChartTemplate(ChartTemplateSet.Simple)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartAlignment LegendAlignment
        {
            get
            {
                return m_defaultLegend == null ? ChartAlignment.Near : m_defaultLegend.Alignment;
            }

            set
            {
                if (m_defaultLegend != null)
                {
                    m_defaultLegend.Alignment = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicates whether legends is located inside or outside of chart.
        /// </summary>
        /// <value>The legends placement.</value>
        [Description("Specifies legend is located inside or outside of chart."), Category("Legend"), DefaultValue(ChartPlacement.Inside)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public ChartPlacement LegendsPlacement
        {
            get 
            {
                return m_dockingManager.Placement; 
            }

            set 
            {
                m_dockingManager.Placement = value; 
            }
        }
        #endregion

        #region ToolBar
        /// <summary>
        /// Gets the configuration information for the toolbar object.
        /// </summary>
        [Description("Configuration information for the toolbar object."), Category("ToolBar"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.Content)]
        public ChartToolBarInfo ToolBar
        {
            get
            {
                return m_toolbar.Info;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [show toolbar while saving the Chartcontrol as image].
        /// </summary>
        /// <value><c>true</c> if [show toolbar in image]; otherwise, <c>false</c>.</value>
        [Description("Specifies if the toolbar is displayed while exporting as image."),  Category("ToolBar"), DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        public bool ShowToolbarInImage
        {
            get
            {
                return showToolbarInImage;
            }

            set
            {
                if (showToolbarInImage != value)
                    showToolbarInImage = value;
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether show the toolbar or not.
        /// </summary>
        [Description("Specifies if the toolbar is displayed."),  Category("ToolBar"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        public bool ShowToolbar
        {
            get
            {
                return this.ToolBar.Visible;
            }

            set
            {
                this.ToolBar.Visible = value;
            }
        }

        #endregion

        #region Model
        /// <summary>
        /// The model contains all the data that is associated with a chart. Models can be shared between charts. Each model can
        /// be seen as a collection of a series. Each of these series contains data points and associated style information.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChartModel Model
        {
            get
            {
                if (m_model == null)
                {
                    this.SetModel(this.OnCreateChartModel());
                }

                return m_model;
            }

            set
            {
                if (m_model != value)
                {
                    this.SetModel(m_model);
                    this.RefreshArea();
                }
            }
        }
        #endregion

        #region Series
        /// <summary>
        /// Gets or sets a value indicating whether the series are compatible.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]       
        public bool CompatibleSeries
        {
            get
            {
                return m_compatibleSeries;
            }

            set
            {
                if (this.m_compatibleSeries != value)
                {
                    m_compatibleSeries = value;
                    RefreshArea();
                }
            }
        }

        /// <summary>
        /// Shortcut method that provides access to the <see cref="ChartSeriesCollection"/> contained in the chart's model.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.SimpleAndCollection, typeof(ChartSeries)), Browsable(false), Category("Data")]        
        public ChartSeriesCollection Series
        {
            get
            {
                return Model.Series;
            }
        }
        #endregion

        #region Axes

        /// <summary>
        /// Gets the collection of axes associated with this chart. You can add and remove axes from this collection.
        /// Primary X and Y axes may not be removed. Use the <see cref="PrimaryXAxis"/> and <see cref="PrimaryYAxis"/> to access
        /// the primary axes.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ChartAxisCollection Axes
        {
            get
            {
                return this.ChartArea.Axes;
            }
        }

        /// <summary>
        /// Specifies if this chart is to be rendered in indexed mode. By default, chart points have X and Y values, both of
        /// which are taken into account for plotting. In indexed mode, the X values are ignored. The X axis purely represents
        /// the position of the points and not their X value. This mode is useful when representing data that has no real X axis value
        /// except position. For example, sales of two months side-by-side.
        /// </summary>
        [Description("Specifies if this chart is to be rendered in indexed mode. In indexed mode, X values are ignored and positions are used."), Category("Axes"), DefaultValue(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool Indexed
        {
            get
            {
                return m_chartArea.IsIndexed;
            }

            set
            {
                m_chartArea.IsIndexed = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow or disallow the gap when points are empty in indexed mode
        /// </summary>
        /// <value></value>
        /// <internalonly/>
        [Description("Allow or Disallow the gap when points are empty in indexed mode"), Category("Axes"), DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        public bool AllowGapForEmptyPoints
        {
            get
            {
                return m_chartArea.IsAllowGap;
            }

            set
            {
                m_chartArea.IsAllowGap = value;
            }
        }
       
        /// <summary>
        /// Gets the primary X axis.
        /// </summary>
        /// <value>The primary X axis.</value>
        [Description("The primary X axis."), Category("Axes"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.Content)]
        public ChartAxis PrimaryXAxis
        {
            get
            {
                return this.ChartArea.PrimaryXAxis;
            }
        }

        /// <summary>
        /// Gets the primary Y axis.
        /// </summary>
        /// <value>The primary Y axis.</value>
        [Description("The primary Y axis."), Category("Axes"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ChartTemplate(ChartTemplateSet.Content)]
        public ChartAxis PrimaryYAxis
        {
            get
            {
                return this.ChartArea.PrimaryYAxis;
            }
        }

        #endregion

        #region Display
        /// <summary>
        /// Gets or sets a value indicating whether the points on the chart should be highlighted when the mouse hovers over them or not.
        /// </summary>
        /// <value></value>
        /// <internalonly/>
        [Description("Specified if points on the chart should be highlighted when the mouse hovers over them."), Category("Appearance"), DefaultValue(false)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool AutoHighlight
        {
            get
            {
                return this.m_autoHighlight;
            }

            set
            {
                if (this.m_autoHighlight != value)
                {
                    this.m_autoHighlight = value;
                    Redraw(true);
                }
            }
        }

        /// <summary>
        /// Specified if series on the chart should be highlighted when the mouse hovers over them or on legend item.
        /// </summary>        
        /// <internalonly/>
        [Description("Specified if series on the chart should be highlighted when the mouse hovers over them or on legend item."), Category("Appearance"), DefaultValue(false)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool SeriesHighlight
        {
            get
            {
                return this.m_seriesHighlight;
            }

            set
            {
                if (this.m_seriesHighlight != value)
                {
                    this.m_seriesHighlight = value;
                    Redraw(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether symbols on the series should be highlighted when the mouse hovers over them or not.
        /// </summary>
        /// <value><c>true</c> if [highlight symbol]; otherwise, <c>false</c>.</value>
        [Description("Specified if symbols on the series should be highlighted when the mouse hovers over them."), Category("Appearance"), DefaultValue(false)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool HighlightSymbol
        {
            get
            {
                return this.m_highlightSymbol;
            }

            set
            {
                if (this.m_highlightSymbol != value)
                {
                    this.m_highlightSymbol = value;
                    Redraw(true);
                }
            }
        }

        /// <summary>
        /// Specifies the series's index when mouse hovering on the legend item and the correspnding series will be highlighted. 
        /// It is used for internal purpose. If it is set externally, only the specified series will be others won't.
        /// </summary>        
        /// <internalonly/>
        [Description("Specified if series on the chart should be highlighted when the mouse hovers over them or on legend item. It is used for internal purpose."), Category("Appearance"), DefaultValue(-1)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public int SeriesHighlightIndex
        {
            get
            {
                return this.m_seriesHighlightIndex;
            }

            set
            {
                if (this.m_seriesHighlightIndex != value)
                {
                    this.m_seriesHighlightIndex = value;
                    Redraw(true);
                }
            }
        }


        /// <summary>
        /// The chart can display an image as its background. This property specifies the image to be used as the background.
        /// </summary>
        [DefaultValue(null), Description("The chart can display an image as its background. This property specifies the image to be used as the background."), Category("Appearance")]
        public Image ChartAreaBackImage
        {
            get
            {
                return m_chartArea.BackImage;
            }

            set
            {
                this.ChartArea.BackImage = value;
            }
        }

        /// <summary>
        /// The chart can display an image as its interior background. This property specifies the image to be used as the chart interior background.
        /// </summary>
        [DefaultValue(null), Description("The chart can display an image as its interior background. This property specifies the image to be used as the chart interior background."), Category("Appearance")]
        public Image ChartInteriorBackImage
        {
            get
            {
                return m_chartArea.InteriorBackImage;
            }

            set
            {
                this.ChartArea.InteriorBackImage = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether display the chart area shadow.
        /// </summary>
        /// <value><c>true</c> if [chart area shadow]; otherwise, <c>false</c>.</value>
        [Description("Indicates whether the ChartArea has a shadow."), Category("Appearance"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        public bool ChartAreaShadow
        {
            get
            {
                return chartAreaShadow;
            }

            set
            {
                if (chartAreaShadow != value)
                {
                    chartAreaShadow = value;
                    RefreshArea();
                }
            }
        }

        /// <summary>
        /// Gets or sets the ToolTip text that is to be displayed when the mouse is over the area 
        /// of the chart but not over other elements of interest within the chart area.
        /// </summary>
        [Description("ToolTip text for the ChartArea."), Category("Appearance"), DefaultValue("")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public string ChartAreaToolTip
        {
            get
            {
                return this.ChartArea.ChartAreaToolTip;
            }

            set
            {
                if (this.ChartArea.ChartAreaToolTip != value)
                {
                    this.m_chartArea.ChartAreaToolTip = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the interior of the chart. Please refer to <see cref="BrushInfo"/> for more information.
        /// </summary>
        [Description("Specifies the interior of the chart."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), ChartTemplate(ChartTemplateSet.Simple)]
        public BrushInfo ChartInterior
        {
            get
            {
                return m_chartArea.GridBackInterior;
            }

            set
            {
                m_chartArea.GridBackInterior = value;
            }
        }

        /// <summary>
        /// Gets or sets the ToolTip text that is to be displayed when the mouse is over the chart 
        /// but not over other elements of interest within the chart.
        /// </summary>
        [Description("ToolTip text for the chart."), Category("Appearance"), DefaultValue("")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public string ChartToolTip
        {
            get
            {
                return chartToolTip;
            }

            set
            {
                chartToolTip = value;
            }
        }

        /// <summary>
        /// Gets or sets axes depth, "coordinate Z". 
        /// Only for enabled Series3D.
        /// </summary>
        [Description("Specifies the depth of the axes in z coordiante."), DefaultValue(50.0f), ChartTemplate(ChartTemplateSet.Simple), Category("Layout")]
        public float Depth
        {
            get
            {
                return this.ChartArea.Depth;
            }

            set
            {
                this.ChartArea.Depth = value;
            }
        }

        /// <summary>
        /// This property specifies the SeriesType to be displayed during design-time. Series displayed during design-time will
        /// be reset during run-time and are provided for representative display only.
        /// </summary>
        [Description("This property specifies the SeriesType to be displayed during design-time. Series displayed during design-time will be reset during run-time and are provided for representative display only."), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ChartSeriesType DesignTimeSeriesType
        {
            get
            {
                return designTimeSeriesType;
            }

            set
            {
                if (this.designTimeSeriesType != value && this.designTimeSeriesType != ChartSeriesType.Custom)
                {
                    designTimeSeriesType = value;
                    this.ChangeRandomDataSeries();
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets value for spacing from border inside ChartControl
        /// e.g. space between ChartControl right border and legend right border if LegendPosition set to right.
        /// </summary>
        [Description("Specifies the spacing value from the border inside chart"), DefaultValue(10), ChartTemplate(ChartTemplateSet.Simple), Category("Appearance")]
        public int ElementsSpacing
        {
            get
            {
                return m_elementsSpacing;
            }

            set
            {
                if (value != m_elementsSpacing)
                {
                    Redraw(true);
                    m_elementsSpacing = value;
                    Redraw(true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether zooming should be enabled for the chart along the X axis.
        /// </summary>
        [Description("Specifies whether zooming should be enabled for the chart along the X axis."), Category("Zooming"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        public bool EnableXZooming
        {
            get
            {
                return m_enableXZooming;
            }

            set
            {
                m_enableXZooming = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether zooming should be enabled for the chart along the Y axis.
        /// </summary>
        [Description("Specifies whether zooming should be enabled for the chart along the Y axis."), Category("Zooming"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        public bool EnableYZooming
        {
            get
            {
                return m_enableYZooming;
            }

            set
            {
                m_enableYZooming = value;
            }
        }

        /// <summary>
        /// Gets or sets the palette that is to be used to provide default colors for series and other elements of a chart. Please refer to
        /// <see cref="ChartColorPalette"/> for available options and customization.
        /// </summary>
        [Description("The palette that is to be used to provide default colors for series and other elements of a chart."), Category("Appearance"), DefaultValue(ChartColorPalette.Default), ChartTemplate(ChartTemplateSet.Simple), Editor(typeof(ChartColorPaletteEditor), typeof(UITypeEditor))]
        public ChartColorPalette Palette
        {
            get
            {
                return Model.ColorModel.Palette;
            }

            set
            {
                Model.ColorModel.Palette = value;
                RefreshArea();
                RefreshLegend();
            }
        }

        /// <summary>
        /// Gets or sets an array of custom palette colors. If <see cref="ChartControl.Palette"/> is <see cref="ChartColorPalette.Custom"/>
        /// this colors is to be used to provide colors of series and other elements. This property duplicates <see cref="ChartColorModel.CustomColors"/> property.
        /// <seealso cref="ChartControl.Palette"/>
        /// </summary>
        [Description("If Palette property is custom this colors to be used to provide default colors for series and other elements of a chart."), Category("Appearance"), DefaultValue(null), Editor(typeof(ColorsUIEditor), typeof(UITypeEditor))]
        public Color[] CustomPalette
        {
            get
            {
                return this.Model.ColorModel.CustomColors;
            }

            set
            {
                this.Model.ColorModel.CustomColors = value;

                if (this.Palette == ChartColorPalette.Custom)
                {
                    this.RefreshArea();
                    this.RefreshLegend();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether allow the gradient palette.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow gradient palette]; otherwise, <c>false</c>.
        /// </value>
        [Description("Enable gradients palettes"), Category("Appearance"), DefaultValue(false)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool AllowGradientPalette
        {
            get
            {
                return this.Model.ColorModel.AllowGradient;
            }

            set
            {
                this.Model.ColorModel.AllowGradient = value;
                this.RefreshArea();
                this.RefreshLegend();
            }
        }

        /// <summary>
        /// Gets or sets the angle rotation of axes relative to axis X.
        /// </summary>
        [Description("Specifies the angle of rotation relative to X axis"), DefaultValue(30F), ChartTemplate(ChartTemplateSet.Simple), Category("Behavior")]
        public float Rotation
        {
            get
            {
                return this.ChartArea.Rotation;
            }

            set
            {
                this.ChartArea.Rotation = value;
            }
        }

        /// <summary>
        /// Specifies if the chart is displayed as 3D.
        /// </summary>
        [Description("Specifies if the chart is displayed as 3D"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple), Category("Behavior")]
        public bool Series3D
        {
            get
            {
                return m_chartArea != null && m_chartArea.Series3D;
            }

            set
            {
                m_model.Series.ShouldSort = this.m_model.Series.ShouldSort;
                m_chartArea.Series3D = value;
            }
        }
        /// <summary>
        /// Specifies if the 3D chart is displayed with 3D style.
        /// </summary>
        [Description(" Specifies if the 3D chart is displayed with 3D style"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple), Category("Behavior")]
        public bool Style3D
        {
            get
            {
                return m_style3D;
            }

            set
            {
                if (m_style3D != value)
                    m_style3D = value;
                this.Redraw(true);
            }
        }
        /// <summary>
        /// Gets or sets color of shadow.
        /// </summary>
        [Description("Specifies the color of the shadow"), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public BrushInfo ShadowColor
        {
            get
            {
                if (m_shadowInterior == null)
                {
                    m_shadowInterior = new BrushInfo(c_defShadowInterior);
                }

                return m_shadowInterior;
            }

            set
            {
                if (m_shadowInterior != value)
                {
                    m_shadowInterior = value;
                    RefreshArea();
                }
            }
        }

        /// <summary>
        /// Gets or sets width of shadow.
        /// </summary>
        [Description("Specifies the width of the shadow"), DefaultValue(5), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public int ShadowWidth
        {
            get
            {
                return shadowWidth; 
            }

            set
            {
                if ((value != shadowWidth) && ((ChartArea.Bottom + value) < this.Bottom))
                {
                    shadowWidth = value;
                    RefreshArea();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show the tool tips.
        /// </summary>
        /// <value><c>true</c> if [show tool tips]; otherwise, <c>false</c>.</value>
        [Description("Specified whether ToolTips should be displayed for the chart."), Category("Appearance"), DefaultValue(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool ShowToolTips
        {
            get
            {
                return showToolTips;
            }

            set
            {
                showToolTips = value;
            }
        }

        /// <summary>
        /// Specified how the chart elements should be rendered. Default is AntiAlias. Please refer to <see cref="SmoothingMode"/> 
        /// in the GDI+ documentation for more information.
        /// </summary>
        [Description("Specifies how the chart elements should be rendered. "), Category("Appearance"), DefaultValue(SmoothingMode.AntiAlias), ChartTemplate(ChartTemplateSet.Simple)]
        public SmoothingMode SmoothingMode
        {
            get
            {
                return m_smoothingMode;
            }

            set
            {
                if (value == SmoothingMode.Invalid)
                {
                    if ((this.Site != null) && (this.Site.DesignMode))
                    {
                        MessageBox.Show(c_ismMessage);
                    }
                }
                else if (m_smoothingMode != value)
                {
                    m_smoothingMode = value;
                    RefreshArea();
                }
            }
        }

        /// <summary>
        /// Spacing is set as a percentage value and controls what portion of the interval partitioned for rendering gets
        /// used for empty space. Default value is 10%.
        /// </summary>
        [Description("Spacing is set as a percentage value and controls what portion of the interval partitioned for rendering gets used for empty space."), DefaultValue(30f), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public float Spacing
        {
            get
            {
                return (float)Math.Round(100 * m_chartArea.SeriesParameters.PointSpacing, 4);
            }

            set
            {
                m_chartArea.SeriesParameters.PointSpacing = value / 100f;
            }
        }

        /// <summary>
        /// Series Spacing is set as a percentage value and it controls the spacing between the series. Default value is 10%.
        /// </summary>
        [Description("Series Spacing is set as a percentage value and it controls the spacing between the series."), DefaultValue(10f), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public float SpacingBetweenSeries
        {
            get
            {
                return (float)Math.Round(100 * m_chartArea.SeriesParameters.SeriesDepthSpacing, 4);
            }

            set
            {
                m_chartArea.SeriesParameters.SeriesDepthSpacing = value / 100;
            }
        }

        /// <summary>
        /// Gets or sets the spacing between points. Default value is 10%.
        /// </summary>
        /// <value>The spacing between points.</value>
        [Description("Series Spacing is set as a percentage value and it controls the spacing between the series."), DefaultValue(0f), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public float SpacingBetweenPoints
        {
            get
            {
                return (float)Math.Round(100 * m_chartArea.SeriesParameters.SeriesSpacing, 4);
            }

            set
            {
                m_chartArea.SeriesParameters.SeriesSpacing = value / 100;
            }
        }

        /// <summary>
        /// Gets or sets a value which specifies the alignment of the chart's text.
        /// </summary>
        [Description("This property specifies the alignment of the chart's text."), Category("Appearance"), DefaultValue(StringAlignment.Center), ChartTemplate(ChartTemplateSet.Simple)]
        public StringAlignment TextAlignment
        {
            get
            {
                if (m_defaultTitle != null)
                    if (!m_defaultTitle.IsDisposed)
                        switch (m_defaultTitle.Alignment)
                        {
                            case ChartAlignment.Near:
                                return StringAlignment.Near;
                            case ChartAlignment.Center:
                                return StringAlignment.Center;
                            case ChartAlignment.Far:
                                return StringAlignment.Far;
                        }

                return StringAlignment.Center;
            }

            set
            {
                if (m_defaultTitle != null && !m_defaultTitle.IsDisposed)
                {
                    switch (value)
                    {
                        case StringAlignment.Near:
                            m_defaultTitle.Alignment = ChartAlignment.Near;
                            break;
                        case StringAlignment.Center:
                            m_defaultTitle.Alignment = ChartAlignment.Center;
                            break;
                        case StringAlignment.Far:
                            m_defaultTitle.Alignment = ChartAlignment.Far;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the angle tilt of axes relative to axis Y.
        /// </summary>
        [Description("Specifies the tilt angle relative to Y axis."), DefaultValue(30f), Category("Appearance")]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public float Tilt
        {
            get
            {
                return this.ChartArea.Tilt;
            }

            set
            {
                this.ChartArea.Tilt = value;
            }
        }

        /// <summary>
        /// When a huge amount of points are drawn on the chart and these points can't be drawn
        /// correctly because of monitor resolution. Then performance can be improved by dropping 
        /// points ( not drawing ). This property controls the intelligent point dropping. Default is false.
        /// </summary>
        [Description("Indicates if points can be dropped when drawing large number of points."), DefaultValue(false), Category("Behavior")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool DropSeriesPoints
        {
            get
            {
                return this.dropSeriesPoints;
            }

            set
            {
                if (this.dropSeriesPoints != value)
                {
                    this.dropSeriesPoints = value;
                }
            }
        }

        /// <summary>
        /// Specifies the default title text.
        /// </summary>
        [ChartTemplate(ChartTemplateSet.Simple)]
        [Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public override string Text
        {
            get
            {
                if (m_defaultTitle != null || !m_defaultTitle.IsDisposed)
                {
                    return m_defaultTitle.Text;
                }

                return String.Empty;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    if (m_titles.Contains(m_defaultTitle))
                    {
                        m_titles.Remove(m_defaultTitle);
                    }
                }
                else
                {
                    if (!m_titles.Contains(m_defaultTitle))
                    {
                        m_titles.Add(m_defaultTitle);
                    }
                }

                m_defaultTitle.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the way text is drawn. Default is SystemDefault.
        /// </summary>
        [Description("Specifies the way text is drawn."), Category("Appearance"), DefaultValue(TextRenderingHint.SystemDefault)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return m_textRenderingHint;
            }

            set
            {
                if (value != m_textRenderingHint)
                {
                    m_textRenderingHint = value;

                    this.RefreshArea();
                }
            }
        }
        #endregion

        #region Printing

        /// <summary>
        /// Gets the print document.
        /// </summary>
        /// <value>The print document.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ChartPrintDocument PrintDocument
        {
            get
            {
                if (m_printDocument == null)
                {
                    m_printDocument = new ChartPrintDocument(this);
                }

                return m_printDocument;
            }
        }

        /// <summary>
        /// Gets or sets the print color mode.
        /// </summary>
        /// <value>The print color mode.</value>
        [Description("Indicates the color mode during printing."), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(ChartPrintColorMode.CheckPrinter), Category("Behavior")]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public ChartPrintColorMode PrintColorMode
        {
            get
            {
                return this.PrintDocument.ColorMode;
            }

            set
            {
                this.PrintDocument.ColorMode = value;
            }
        }

        /// <summary>
        /// Gets size or if printing == True virtualsize.
        /// Printing == True when call method Draw.
        /// </summary>
        private Size VirtualSize
        {
            get
            {
                if (this.printing)
                {
                    return this.virtualSize;
                }

                return Size;
            }
        }
        #endregion

        #region Context Menus
        /// <summary>
        ///  Gets or sets a value indicating whether the Series Context Menu should be shown. This is used only if <see cref="ShowContextMenu"/> is true.
        /// </summary>
        [Description("Specifies if the series context menu should be shown."), Category("ContextMenus"), DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        public bool DisplaySeriesContextMenu
        {
            get
            {
                return m_displaySeriesContextMenu;
            }

            set
            {
                if (m_displaySeriesContextMenu != value)
                {
                    m_displaySeriesContextMenu = value;

                    if (value)
                    {
                        EnableMouseRotation = false;
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the Chart Context menu should be shown. This is used only if <see cref="ShowContextMenu"/> is true.
        /// </summary>
        [Description("Specifies if the chart context menu should be shown."), Category("ContextMenus"), DefaultValue(true), ChartTemplate(ChartTemplateSet.Simple)]
        public bool DisplayChartContextMenu
        {
            get
            {
                return m_displayChartContextMenu;
            }

            set
            {
                if (m_displayChartContextMenu != value)
                {
                    m_displayChartContextMenu = value;

                    if (value)
                    {
                        EnableMouseRotation = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show the context menu. It specifies if chart area and series context menus can be shown. See <see cref="DisplayChartContextMenu"/> and <see cref="DisplaySeriesContextMenu"/>.
        /// </summary>
        [Description("Specifies if chart area and series context menus can be shown."), Category("ContextMenus"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        public bool ShowContextMenu
        {
            get
            {
                return this.m_showContextMenu;
            }

            set
            {
                this.m_showContextMenu = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show the Context menu for the Legend. It specifies if the context menu in the legend is enabled.
        /// </summary>
        [Description("Specifies if the context menu in the legend is enabled."), Category("ContextMenus"), DefaultValue(false), ChartTemplate(ChartTemplateSet.Simple)]
        public bool ShowContextMenuInLegend
        {
            get
            {
                return this.m_showContextMenuInLegend;
            }

            set
            {
                this.m_showContextMenuInLegend = value;
            }
        }

        /// <summary>
        /// Gets the configuration information for the chart context menu object
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public ChartContextMenu ChartContextMenu
        {
            get
            {
                if (m_chartContextMenu == null)
                {
                    m_chartContextMenu = new ChartContextMenu(this);
                }

                return m_chartContextMenu;
            }
        }

        /// <summary>
        /// Gets the configuration information for the series context menu object
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]        
        public ChartSeriesContextMenu SeriesContextMenu
        {
            get
            {
                if (m_seriesContextMenu == null)
                {
                    m_seriesContextMenu = new ChartSeriesContextMenu();
                }

                return m_seriesContextMenu;
            }
        }
        #endregion

        #region Internal
        /// <summary>
        /// Gets a value indicating whether if any buttons of mouse was pressed or not.
        /// </summary>
        private bool IsMouseDown
        {
            get
            {
                return MouseButtons != MouseButtons.None;
            }
        }

        /// <summary>
        /// Gets the last mouse position on the chartcontrol when the any mouse button was pressed.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This property is used for zooming only.
        /// </remarks>
        [DocumentationExclude(), Browsable(false)]
        public Point MouseDownPosition
        {
            get
            {
                if (m_currectMouseAction == InternalMouseAction.Zooming)
                {
                    return mouseDownPosition;
                }
                else
                {
                    return Point.Empty;
                }
            }
        }

        /// <summary>
        /// Gets last mouse position on the chartcontrol on mouse click. 
        /// </summary>
        /// <returns>The mouse position</returns>
        /// <remarks>
        /// This property is used for zooming only.
        /// </remarks>
        [DocumentationExclude(), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point ClickPoint
        {
            get
            {
                if (m_currectMouseAction == InternalMouseAction.Zooming)
                {
                    return clickPoint;
                }
                else
                {
                    return Point.Empty;
                }
            }
        }

        /// <summary>
        /// Gets a collection of all the indexed values. It's used for indexed mode.
        /// </summary>
        [DocumentationExclude(), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public ChartIndexedValues IndexValues
        {
            get
            {
                return m_model.IndexedValues;
            }
        }

        /// <summary>
        /// Gets or sets indicating whether the Regions are need to update. It specifies if the internal <see cref="ChartRegion"/>s are to be updated. Also redraws the chart.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [ChartTemplate(ChartTemplateSet.Simple)]
        public bool NeedRegionUpdate
        {
            get
            {
                return m_needRegionUpdate && m_calcRegions;
            }

            set
            {
                if (m_needRegionUpdate != value)
                {
                    if (!this.m_calcRegions)
                    {
                        m_needRegionUpdate = value;

                        if (value)
                        {
                            Redraw(true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the state of Interactive mouse cursor is down.
        /// </summary>
        [Browsable(false)]
        [ChartTemplate(ChartTemplateSet.SimpleBehavior)]
        public bool InteractiveCursorMouseDown
        {
            get
            {
                return m_interactiveCursorDown != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [random data present].
        /// </summary>
        /// <value><c>true</c> if [random data present]; otherwise, <c>false</c>.</value>
        private bool RandomDataPresent
        {
            get
            {
                if (this.Series.Count > 0)
                {
                    return IsRandomSeries(this.Series[0]);
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the minimal points delta.
        /// </summary>
        /// <value>The min points delta.</value>
        private double MinPointsDelta
        {
            get
            {
                if (double.IsNaN(m_minPointsDelta))
                {
                    m_minPointsDelta = double.MaxValue;

                    if (this.Indexed)
                    {
                        m_minPointsDelta = 1;
                    }
                    else
                    {
                        foreach (ChartSeries series in this.Series)
                        {
                            if (series.Visible)
                            {
                                double[] xValues = new double[series.Points.Count];

                                for (int i = 0; i < series.Points.Count; i++)
                                {
                                    xValues[i] = series.Points[i].X;
                                }

                                Array.Sort(xValues);

                                for (int i = 1; i < xValues.Length; i++)
                                {
                                    double delta = xValues[i] - xValues[i - 1];

                                    if (delta != 0)
                                    {
                                        m_minPointsDelta = Math.Min(m_minPointsDelta, delta);
                                    }
                                }
                            }
                        }
                    }

                    if (m_minPointsDelta == double.MaxValue)
                    {
                        m_minPointsDelta = 1;
                    }
                }

                return m_minPointsDelta;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is panning enabled.
        /// </summary>
        /// <value>
        ///       <c>true</c> if this instance is panning enabled; otherwise, <c>false</c>.
        /// </value>
        private bool IsPanningEnabled
        {
            get
            {
                return m_mouseAction == ChartMouseAction.Panning
                    || (Control.ModifierKeys & Keys.Control) == Keys.Control;
            }
        }
        #endregion

        #region Obsolete properites

        /// <summary>
        /// Gets or sets a value indicating whether legend is painted.
        /// </summary>
        /// <value><c>true</c> if [legend painted]; otherwise, <c>false</c>.</value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This property isn't used anymore.")]
        public bool LegendPainted
        {
            get
            {
                return false;
            }

            set
            {
            }
        }
        #endregion

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ChartControl"/> class.
        /// </summary>
        public ChartControl()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartControl"/> class. For internal use. Do not use this constructor directly.
        /// </summary>
        /// <param name="addRandomData">if set to <c>true</c> [add random data].</param>
        /// <param name="isWindowLess">if set to <c>true</c> [is window less].</param>
        /// <internalonly/>
        [DocumentationExclude()]
        public ChartControl(bool addRandomData, bool isWindowLess)
        {
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
                new Syncfusion.Core.Licensing.LicensedComponent(typeof(ChartControl));
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(AssemblyInfo.AssemblyResolver);
            }

            IsWindowLess = isWindowLess;

            #region Set ControlStyles
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint
                | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor
                | ControlStyles.Selectable | ControlStyles.EnableNotifyMessage, true);
            #endregion

            #region Set ChartArea
            m_chartArea = new ChartArea(this);

            foreach (ChartAxis axis in m_chartArea.Axes)
            {
                axis.Zooming += new ChartAxisZoomingEventHandler(OnAxisZooming);
                axis.Zoomed += new EventHandler(OnAxisZoomed);
            }

            m_chartArea.Axes.Changed += new ChartListChangeHandler(this.OnAxesChanged);
            #endregion

            #region Set model
            this.SetModel(this.OnCreateChartModel());
            #endregion

            #region Border
            m_borderAppearance = new ChartBorderInfo();
            m_borderAppearance.Changed += new EventHandler(OnBorderAppearanceChanged);
            #endregion

            #region Set docking manager
            m_dockingManager = new ChartDockingManager(this);
            m_dockingManager.SizeChanged += new EventHandler(DockingManager_SizeChanged);
            m_dockingManager.DockAlignment = true;
            m_dockingManager.Placement = ChartPlacement.Inside;

            m_titleDockingManager = new ChartDockingManager(this);
            m_titleDockingManager.SizeChanged += new EventHandler(DockingManager_SizeChanged);

            m_toolBarDockingManager = new ChartDockingManager(this);
            m_toolBarDockingManager.SizeChanged += new EventHandler(DockingManager_SizeChanged);
            #endregion

            #region Set toolbar and toolbar controller
            m_toolbar = new ChartToolBar(this);
            m_toolbar.Visible = false;

            this.Controls.Add(m_toolbar);

            m_toolBarDockingManager.Add(m_toolbar);
            #endregion

            #region Set titles
            m_defaultTitle = new ChartTitle();
            m_defaultTitle.Name = c_defaultTitleName;

            m_titles = new ChartTitlesList();
            m_titles.Changed += new ChartListChangeHandler(this.OnTitlesChanged);
            ////m_titles.Add(m_defaultTitle);
            #endregion

            #region Set legend
            m_defaultLegend = new ChartLegend(this, isWindowLess);
            m_defaultLegend.Name = ChartLegend.DefaultName;

            m_legends.Changed += new ChartListChangeHandler(OnLegendsChanged);
            m_legends.Add(m_defaultLegend);
            #endregion

            #region Set tooltip
            m_toolTip = new ToolTipAdv(this);
            m_toolTip.BackgroundColor = new BrushInfo(SystemColors.Info);
            m_toolTip.ForeColor = SystemColors.InfoText;
            m_toolTip.BorderStyle = BorderStyle.FixedSingle;
            #endregion

            #region Set fancy tooltip controller
            m_fancyToolTipController = new ChartFancyToolTipController(this);
            #endregion

            #region Random data
            m_addRandomData = addRandomData;
            this.AddRandomData();
            #endregion

            #region Set tooltip timer
            m_toolTipTimer.Interval = 500;
            m_toolTipTimer.Tick += new EventHandler(this.ToolTipTick);
            #endregion

            #region Initialize
            this.Size = new Size(400, 300);
            #endregion
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartControl"/> class.
        /// </summary>
        /// <param name="addRandomData">if set to <c>true</c> random data will be added.</param>
        public ChartControl(bool addRandomData)
            : this(addRandomData, false)
        {
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Creates the instance of Graphics for the control.
        /// </summary>
        /// <returns>Graphics context.</returns>
        public Graphics GetGraphics()
        {
            Graphics graphics = null;

            if (m_internalHdc != IntPtr.Zero)
            {
                graphics = Graphics.FromHdc(m_internalHdc);
            }
            else if (m_isWindowLess && m_canvas != null)
            {
                graphics = Graphics.FromImage(m_canvas);
            }
            else
            {
                graphics = this.CreateGraphics();
            }

            graphics.TextRenderingHint = m_textRenderingHint;
            graphics.SmoothingMode = m_smoothingMode;

            return graphics;
        }

        /// <summary>
        /// Checks is whether a series is added by ChartControl (Random datas).
        /// </summary>
        /// <param name="series">Instance of the ChartSeries.</param>
        /// <returns>True if the series is added by ChartControl.</returns>
        public static bool IsRandomSeries(ChartSeries series)
        {
            return series.Name.IndexOf(c_internalSeriesName) == 0;
        }

        /// <summary>
        /// Call this method if you perform multiple changes in quick succession.
        /// <seealso cref="ChartControl.EndUpdate"/>
        /// </summary>
        public void BeginUpdate()
        {
            Series.BeginUpdate();
            updating = true;
        }

        /// <summary>
        /// When we called this method then it improves the performance by dropping some points(not drawing).
        /// This method controls the intelligent point dropping and also it disables some visual related features like tooltip.
        /// <seealso cref="ChartControl.EndUpdate"/>
        /// </summary>
        /// <param name="improvePerformance">if set to <c>true</c> [improve performance].</param>
        public void BeginUpdate(bool improvePerformance)
        {
            m_needPerformance = true;
            this.dropSeriesPoints = true;
            Series.DisableStyles = improvePerformance;
            this.CalcRegions = false;
            Series.BeginUpdate();
            updating = true;
        }

        /// <summary>
        /// Call this method if you called <see cref="ChartControl.BeginUpdate()"/> earlier and you are done with your changes.
        /// <seealso cref="ChartControl.BeginUpdate()"/>
        /// </summary>
        public void EndUpdate()
        {
            updating = false;
            this.Series.EndUpdate();
            this.CheckRandomData();
            this.SeriesChanged(this, new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Changed));
        }

        /// <summary>
        ///     The ChartAreaPointToChartControl method takes a point object in ChartArea
        ///     coordinates and returns a point object in ChartControl coordinates.
        /// </summary>
        /// <param name="p" type="System.Drawing.Point">
        ///     <para>
        ///         The point in ChartArea coordinates.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A System.Drawing.Point value that represents the point in ChartControl coordinates.
        /// </returns>
        public Point ChartAreaPointToChartControl(Point p)
        {
            return new Point(p.X + ChartArea.Left, p.Y + ChartArea.Top);
        }

        /// <summary>
        ///     The ChartAreaRectToChartControl method takes a RectangleF object in ChartArea
        ///     coordinates and returns a RectangleF object in ChartControl coordinates.
        /// </summary>
        /// <param name="rect" type="System.Drawing.RectangleF">
        ///     <para>
        ///         The RectangleF in ChartArea coordinates.
        ///     </para>
        /// </param>
        /// <returns>
        ///     A System.Drawing.RectangleF value that represents the RectangleF in ChartControl coordinates.
        /// </returns>
        public RectangleF ChartAreaRectToChartControl(RectangleF rect)
        {
            rect.Offset(ChartArea.Left, ChartArea.Top);
            return rect;
        }

        /// <summary>
        /// Displays the dialog to edit user styles for a series.
        /// </summary>
        /// <param name="seriesIndex">The index of the series.</param>
        public void DisplayUserEditStylesDialog(int seriesIndex)
        {
            BrushInfo seriesInterior;
            ChartStyleInfo seriesStyle = this.Series[seriesIndex].Style;

            if (seriesStyle.Interior == null)
            {
                seriesInterior = new BrushInfo(m_model.ColorModel.GetColor(seriesIndex));
            }
            else
            {
                seriesInterior = seriesStyle.Interior;
            }

            using (SeriesStyleEditorForm stylesEditorForm = new SeriesStyleEditorForm(Series[seriesIndex],
                this.Series[seriesIndex].Name, seriesInterior, m_styleDialogOptions))
            {
                if (stylesEditorForm.ShowDialog() == DialogResult.OK)
                {
                    this.Redraw(true);
                }
            }
        }

        /// <summary>
        /// Paints the chart to graphics.
        /// </summary>
        /// <param name="g">Instance of the <see cref="Graphics"/>.</param>
        /// <param name="rc">The Rectangle within which to paint.</param>
        public void Draw(Graphics g, Rectangle rc)
        {
            g.TranslateTransform((float)rc.X, (float)rc.Y);
            this.Draw(g, rc.Size);
        }
        /// <summary>
        /// Draws the images.
        /// </summary>
        /// <param name="grap">The grap.</param>
        /// <param name="curr_control">The curr_control.</param>
        private void DrawImages(Graphics grap, Control curr_control)
        {

            foreach (Control ctr in curr_control.Controls)
            {
                if(ctr.Visible)
                {
                Image img = ConvertToMetafile(ctr.Width, ctr.Height, ctr);
                Control par = ctr.Parent;
                int x, y;
                if (par is ChartDockControl)
                {
                    x = ctr.Bounds.X;
                    y = ctr.Bounds.Y;
                }
                else
                {
                    x = ctr.Parent.Bounds.X + ctr.Bounds.X;
                    y = ctr.Parent.Bounds.Y + ctr.Bounds.Y;
                }
                grap.DrawImage(img, new Rectangle(x, y, ctr.Width, ctr.Height));
                if (ctr.HasChildren)
                {
                    DrawImages(grap, ctr);
                }
                }
            }


        }
        /// <summary>
        /// Converts to metafile.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="ctr">The CTR.</param>
        /// <returns></returns>
        private Metafile ConvertToMetafile(int width, int height, Control ctr)
        {
            Metafile mf;
            MemoryStream ms = new MemoryStream();

            try
            {
                System.Drawing.Size size = new System.Drawing.Size(width, height);
                Bitmap bmp = new Bitmap(1, 1);
                Graphics ctlGfx = Graphics.FromImage(bmp);
                IntPtr refHdc = ctlGfx.GetHdc();
		
			    size.Width = size.Width == 0 ? 1 : size.Width;
                size.Height = size.Height == 0 ? 1 : size.Height;

                RectangleF bounds = new RectangleF(0, 0, size.Width, size.Height);

                mf = new Metafile(ms, refHdc, bounds,
                  MetafileFrameUnit.Pixel, EmfType.EmfOnly);

                Graphics graphics = Graphics.FromImage(mf);
                IntPtr memdc = graphics.GetHdc();

                Print(memdc, false, ctr);

                graphics.ReleaseHdc(memdc);
                graphics.Dispose();
                ctlGfx.ReleaseHdc(refHdc);
                ctlGfx.Dispose();
                bmp.Dispose();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                throw;
            }
            finally
            {
                ms.Close();
            }

            return mf;
        }
        /// <summary>
        /// Prints the specified PTR.
        /// </summary>
        /// <param name="ptr">The PTR.</param>
        /// <param name="isBitmap">if set to <c>true</c> [is bitmap].</param>
        /// <param name="ctr">The CTR.</param>
        private void Print(IntPtr ptr, bool isBitmap, Control ctr)
        {
            uint lParam = Native.PRF_CLIENT |
              Native.PRF_OWNED | Native.PRF_ERASEBKGND;

            if (isBitmap)
            {
                lParam |= Native.PRF_CHILDREN;
            }

            int err = Native.SendMessage(ctr.Handle,
              Native.WM_PRINT, (uint)ptr, lParam);
        }
      

        /// <summary>
        /// Paints the chart to graphics.
        /// </summary>
        /// <param name="g">Instance of the <see cref="Graphics"/>.</param>
        /// <param name="sz">The size of rectangle within which to paint.</param>
        public void Draw(Graphics g, Size sz)
        {
            bool isToolBarVisible = this.ShowToolbar;
            bool isVisible = this.Visible;
            this.Visible = true;

            try
            {
                m_isWindowLess = true;

                foreach (ChartLegend legend in m_legends)
                {
                    legend.IsWindowLess = true;
                }

                foreach (ChartTitle title in m_titles)
                {
                    title.IsWindowLess = true;
                }

                this.RefreshArea();
                GraphicsContainer cont;
                this.printing = true;
                this.virtualSize = sz;

                m_dockingManager.Freeze();
                m_titleDockingManager.Freeze();
                m_toolBarDockingManager.Freeze();

                if(this.ShowToolbar)
                this.ShowToolbar = this.ShowToolbarInImage;

                ////this.OnPaintBackground(new PaintEventArgs(g, new Rectangle(Point.Empty, sz)));

                g.TextRenderingHint = m_textRenderingHint;
                g.SmoothingMode = m_smoothingMode;

                m_internalHdc = g.GetHdc();

                this.PrepareAxesSeriesAndChart();
                this.RecalculateSizes(g);

                g.ReleaseHdc(m_internalHdc);
                m_internalHdc = IntPtr.Zero;

                m_needRecalculateSizes = false;

                _Paint(g, new Rectangle(new Point(0, 0), virtualSize));

                foreach (Control cntr in this.Controls)
                {
                    if (!(cntr is ChartLegend || cntr is ChartTitle || cntr is ChartToolBar) && cntr.Visible)
                    {
                        Image im;
                        Metafile meta;
                        MemoryStream msm;
                        msm = new MemoryStream();
                        Size sz1 = new Size(cntr.Width, cntr.Height);
                        Bitmap bbmp = new Bitmap(1, 1);
                        Graphics grDfx = Graphics.FromImage(bbmp);
                        IntPtr intpr = grDfx.GetHdc();
                        RectangleF bounds = new RectangleF(0, 0, sz1.Width, sz1.Height);
                        meta = new Metafile(msm, intpr, bounds,
                          MetafileFrameUnit.Pixel, EmfType.EmfOnly);
                        Graphics grap = Graphics.FromImage(meta);


                        DrawImages(grap, cntr);

                        grap.Dispose();
                        msm.Dispose();

                        im = meta;
                        if (cntr.Width > (VirtualSize.Width - (this.ChartAreaMargins.Left + this.ChartAreaMargins.Right)))
                        {
                            int start = (this.Margin.Left + this.Margin.Vertical);
                            Point p = new Point(start, cntr.Bounds.Y);
                            g.DrawImage(im, p.X, p.Y, (VirtualSize.Width - (this.ChartAreaMargins.Left + this.ChartAreaMargins.Right + start)), cntr.Height);
                        }
                        else
                        {
                            g.DrawImage(im, cntr.Bounds);
                        }
                    }
                }
                ;

                foreach (ChartLegend legend in m_legends)
                {
                    if (legend.Visible)
                    {
                        cont = BeginTransform(g);
                        if (legend.Position == ChartDock.Floating)
                        {
                            float legendXPos = legend.Location.X * sz.Width / Width;
                            float legendYPos = legend.Location.Y * sz.Height / Height;
                            g.TranslateTransform(legendXPos, legendYPos);
                        }
                        else
                        {
                            g.TranslateTransform(legend.Location.X, legend.Location.Y);
                        }

                        legend.Draw(new PaintEventArgs(g, legend.ClientRectangle));
                        ChartControl.EndTransform(g, cont);
                    }
                }

                foreach (ChartTitle title in m_titles)
                {
                    if (title.Visible)
                    {
                        title.Print(g, title.Bounds);
                    }
                }

                if (m_toolbar.Visible)
                {
                    m_toolbar.Draw(g);
                }
            }
            finally
            {
                this.printing = false;

                foreach (ChartLegend legend in m_legends)
                {
                    legend.IsWindowLess = false;
                }

                foreach (ChartTitle title in m_titles)
                {
                    title.IsWindowLess = false;
                }

                this.ShowToolbar = isToolBarVisible;
                this.Visible = isVisible;

                m_isWindowLess = false;
                m_toolBarDockingManager.Melt();
                m_titleDockingManager.Melt();
                m_dockingManager.Melt();

                this.RecalculateSizes();
            }
        }

        /// <summary>
        /// Paints the chart to image.
        /// </summary>
        /// <param name="img">Instance of the <see cref="Image"/>.</param>
        public void Draw(Image img)
        {
            Graphics g = Graphics.FromImage(img);
            g.Clear(BackColor);
            Draw(g, img.Size);
            g.Dispose();
        }

        /// <summary>
        /// Paints the chart to image.
        /// </summary>
        /// <param name="img">Instance of the <see cref="Image"/>.</param>
        /// <param name="sz">The size of rectangle within which to paint.</param>
        public void Draw(Image img, Size sz)
        {
            Graphics g = Graphics.FromImage(img);
            g.Clear(BackColor);
            Draw(g, sz);
            g.Dispose();
        }

        /// <summary>
        /// Paints the thumbnail of chart to image.
        /// </summary>
        /// <param name="img">Instance of the <see cref="Image"/>.</param>
        [DocumentationExclude()]
        public void DrawThumbnail(Image img)
        {
            DrawThumbnail(img, new Rectangle(0, 0, img.Width, img.Height));
        }

        /// <summary>
        /// Paints the thumbnail of chart to image.
        /// </summary>
        /// <param name="img">Instance of the <see cref="Image"/>.</param>
        /// <param name="r">The rectangle within which to paint.</param>
        [DocumentationExclude()]
        public void DrawThumbnail(Image img, Rectangle r)
        {
            Bitmap b = new Bitmap(this.Size.Width, this.Size.Height);

            using (Graphics g = Graphics.FromImage(b))
            {
                g.Clear(BackColor);
                Draw(g, b.Size);
            }

            using (Graphics g = Graphics.FromImage(img))
            {
                g.DrawImage(b, r);
            }
        }

        /// <summary>
        ///     The GetChartAreaBounds method provides access to the bounds of the ChartArea.
        /// </summary>
        /// <returns>
        ///     A System.Drawing.Rectangle value of the bounds of the ChartArea in ChartControl coordinates.
        /// </returns>
        public Rectangle GetChartAreaBounds()
        {
            return this.ChartArea.Bounds;
        }

        #region Obsolete methods
        /// <summary>
        /// This method is not used anymore. Use the Legends collection.
        /// </summary>
        /// <param name="legend">Instance of the new legend.</param>
        [Obsolete("This method is not used anymore. Please use Legends collection.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SetLegend(ChartLegend legend)
        {
        }

        /// <summary>
        /// Append point to series.
        /// </summary>
        /// <param name="series">The series.</param>
        /// <param name="point">The point.</param>
        [Obsolete("This method is not used anymore. Use ChartSeries.Points collection.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AppendSeries(ChartSeries series, ChartPoint point)
        {
            series.Points.Add(point);
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the specified data points in the specified axes.
        /// </summary>
        /// <param name="xValue">An X value.</param>
        /// <param name="yValue">An Y value.</param>
        /// <param name="xAxis">An X axis.</param>
        /// <param name="yAxis">An Y axis.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(double xValue, double yValue, ChartAxis xAxis, ChartAxis yAxis)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue, xAxis), this.GetChartAreaCoordinateY(yValue, yAxis));
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the primary x and y axes.
        /// </summary>
        /// <param name="xValue">An X value.</param>
        /// <param name="yValue">An Y value.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(double xValue, double yValue)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue), this.GetChartAreaCoordinateY(yValue));
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the specified data points in the specified axes.
        /// </summary>
        /// <param name="xValue">An X value.</param>
        /// <param name="yValue">An Y <see cref="DateTime"/> value.</param>
        /// <param name="xAxis">An X axis.</param>
        /// <param name="yAxis">An Y axis.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(double xValue, DateTime yValue, ChartAxis xAxis, ChartAxis yAxis)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue, xAxis), this.GetChartAreaCoordinateY(yValue, yAxis));
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the primary x and y axes.
        /// </summary>
        /// <param name="xValue">An X value.</param>
        /// <param name="yValue">An Y <see cref="DateTime"/> value.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(double xValue, DateTime yValue)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue), this.GetChartAreaCoordinateY(yValue));
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the specified data points in the specified axes.
        /// </summary>
        /// <param name="xValue">An X <see cref="DateTime"/> value.</param>
        /// <param name="yValue">An Y value.</param>
        /// <param name="xAxis">An X axis.</param>
        /// <param name="yAxis">An Y axis.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(DateTime xValue, double yValue, ChartAxis xAxis, ChartAxis yAxis)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue, xAxis), this.GetChartAreaCoordinateY(yValue, yAxis));
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the primary x and y axes.
        /// </summary>
        /// <param name="xValue">An X <see cref="DateTime"/> value.</param>
        /// <param name="yValue">An Y value.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(DateTime xValue, double yValue)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue), this.GetChartAreaCoordinateY(yValue));
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the specified data points in the specified axes.
        /// </summary>
        /// <param name="xValue">An X <see cref="DateTime"/> value.</param>
        /// <param name="yValue">An Y <see cref="DateTime"/> value.</param>
        /// <param name="xAxis">An X axis.</param>
        /// <param name="yAxis">An Y axis.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(DateTime xValue, DateTime yValue, ChartAxis xAxis, ChartAxis yAxis)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue, xAxis), this.GetChartAreaCoordinateY(yValue, yAxis));
        }

        /// <summary>
        /// Returns the client coordinates on ChartArea for the primary x and y axes.
        /// </summary>
        /// <param name="xValue">An X <see cref="DateTime"/> value.</param>
        /// <param name="yValue">An Y <see cref="DateTime"/> value.</param>
        /// <returns>The coordinates on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public Point GetChartAreaCoordinates(DateTime xValue, DateTime yValue)
        {
            return new Point(this.GetChartAreaCoordinateX(xValue), this.GetChartAreaCoordinateY(yValue));
        }

        /// <summary>
        /// Returns the matching X client coordinate on ChartArea for the specified X value.
        /// </summary>
        /// <param name="xValue">An X value.</param>
        /// <returns>The X coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateX(double xValue)
        {
            return this.GetChartAreaCoordinateX(xValue, this.PrimaryXAxis);
        }

        /// <summary>
        /// Returns the matching X client coordinate on ChartArea for the specified X value in the specified axis.
        /// </summary>
        /// <param name="xValue">An X value.</param>
        /// <param name="xAxis">An X axis.</param>
        /// <returns>The X coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateX(double xValue, ChartAxis xAxis)
        {
            MinMaxInfo axisRange = xAxis.VisibleRange;
            return (int)(m_chartArea.OffsetX + (float)m_chartArea.RenderBounds.Width * ((xValue - axisRange.Min) / (axisRange.Delta)));
        }

        /// <summary>
        /// Returns the matching X client coordinate on ChartArea for the specified X date time value in the specified axis.
        /// </summary>
        /// <param name="xValue">An X <see cref="DateTime"/> value.</param>
        /// <returns>The X coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateX(DateTime xValue)
        {
            return this.GetChartAreaCoordinateX(xValue.ToOADate(), this.PrimaryXAxis);
        }

        /// <summary>
        /// Returns the matching X client coordinate on ChartArea for the specified X value in the specified axis.
        /// </summary>
        /// <param name="xValue">An X <see cref="DateTime"/> value.</param>
        /// <param name="xAxis">An X axis.</param>
        /// <returns>The X coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateX(DateTime xValue, ChartAxis xAxis)
        {
            MinMaxInfo axisRange = xAxis.VisibleRange;
            return (int)(m_chartArea.OffsetX + (float)m_chartArea.RenderBounds.Width * ((xValue.ToOADate() - axisRange.Min) / (axisRange.Delta)));
        }

        /// <summary>
        /// Returns the Y client coordinate on ChartArea for the specified Y value.
        /// </summary>
        /// <param name="yValue">An Y value.</param>
        /// <returns>The Y coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateY(double yValue)
        {
            return this.GetChartAreaCoordinateY(yValue, this.PrimaryYAxis);
        }

        /// <summary>
        /// Returns the Y client coordinate on ChartArea for the specified Y value in the specified axis.
        /// </summary>
        /// <param name="yValue">An Y value.</param>
        /// <param name="yAxis">An Y axis.</param>
        /// <returns>The Y coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateY(double yValue, ChartAxis yAxis)
        {
            MinMaxInfo axisRange = yAxis.VisibleRange;
            return (int)(m_chartArea.OffsetY + m_chartArea.RenderBounds.Height - (float)m_chartArea.RenderBounds.Height * ((yValue - axisRange.Min) / (axisRange.Delta)));
        }

        /// <summary>
        /// Returns a Y client coordinate on ChartArea for the specified Y value.
        /// </summary>
        /// <param name="yValue">An Y <see cref="DateTime"/> value.</param>
        /// <returns>The Y coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateY(DateTime yValue)
        {
            return this.GetChartAreaCoordinateY(yValue.ToOADate(), this.PrimaryYAxis);
        }

        /// <summary>
        /// Returns a Y client coordinate on ChartArea for the specified Y value on the specified axis.
        /// </summary>
        /// <param name="yValue">An Y <see cref="DateTime"/> value.</param>
        /// <param name="yAxis">An Y axis.</param>
        /// <returns>The Y coordinate on ChartArea.</returns>
        [Obsolete("This method is obsolete. Use the ChartArea.GetPointByValue or ChartAxis.GetVisibleValue methods."), EditorBrowsable(EditorBrowsableState.Never)]
        public int GetChartAreaCoordinateY(DateTime yValue, ChartAxis yAxis)
        {
            MinMaxInfo axisRange = yAxis.VisibleRange;
            return (int)(m_chartArea.OffsetY + m_chartArea.RenderBounds.Height - (float)m_chartArea.RenderBounds.Height * ((yValue.ToOADate() - axisRange.Min) / (axisRange.Delta)));
        }

        /// <summary>
        /// List of all ChartRegion objects that make up the ChartControl.
        /// </summary>
        /// <returns>
        /// A System.Collections.IList of the ChartRegions.
        /// </returns>
        [Obsolete("This method is obsolete. Use the ChartControl.ChartRegions property."), EditorBrowsable(EditorBrowsableState.Never)]
        public IList GetChartRegions()
        {
            return ChartRegions;
        }
        #endregion

        /// <summary>
        /// Returns the ChartRegion corresponding to the series and point indexed supplied.
        /// </summary>
        /// <param name="seriesIndex">Index of the series.</param>
        /// <param name="pointIndex">Index of the point.</param>
        /// <returns>The ChartRegion.</returns>
        public ChartRegion GetRegionFromIndexed(int seriesIndex, int pointIndex)
        {
            int count = m_chartRegions.Count;

            foreach (ChartRegion rgn in m_chartRegions)
            {
                if (rgn.SeriesIndex == seriesIndex && rgn.PointIndex == pointIndex)
                {
                    return rgn;
                }
            }

            return null;
        }

        /// <internalonly/>
        /// <summary>
        /// Calls the <see cref="ChartControl.SeriesChanged"/> method.
        /// </summary>
        [DocumentationExclude()]
        public void NotifySeriesChanged()
        {
            this.SeriesChanged(this, new ChartSeriesCollectionChangedEventArgs(ChartSeriesCollectionChangeType.Changed));
        }

        /// <summary>
        /// Raises the <see cref="ChartControl.ChartFormatAxisLabel"/>event.
        /// </summary>
        /// <param name="axis">Instance of ChartAxis.</param>
        /// <param name="args">Argument.</param>
        /// <internalonly/>
        [DocumentationExclude()]
        public void OnChartFormatAxisLabel(ChartAxis axis, ChartFormatAxisLabelEventArgs args)
        {
            if (ChartFormatAxisLabel != null)
            {
                ChartFormatAxisLabel(axis, args);
            }
        }

        /// <summary>
        /// Forces the redraw of the Chart Area.
        /// </summary>
        /// <param name="update"><c>true</c> to redraw chart area; otherwise,<c>false</c> to use buffered image.</param>
        public void Redraw(bool update)
        {
            m_needRecalculateSizes = update;
            m_needRegionUpdate = update;
            m_needRedraw = update;

            if (update)
            {
                m_activeRegion = null;
            }

            foreach (Control control in this.Controls)
            {
                if (!(control is IScrollBar))
                {
                    control.Invalidate();
                }
            }

            this.Invalidate();
        }

        /// <summary>
        /// Displays the Wizard.
        /// </summary> 
        public void DisplayWizard()
        {
            using (ChartWizardForm wizard = new ChartWizardForm(this.Site))
            {
                wizard.ShowWizard(this);                
            }                       
        }

        /// <summary>
        /// Saves the chart as an image in the specified format.
        /// </summary>
        /// <param name="filename">The filename with extension that specifies the type of format to save to.</param>
        /// <remarks>
        /// Supported formats are bmp, jpg, gif, tiff, wmf, emf, svg and eps.
        /// </remarks>
        public void SaveImage(string filename)
        {
            string ext = filename.Substring(filename.LastIndexOf("."));

            switch (ext)
            {
                case ".bmp":
                    {
                        Bitmap img = new Bitmap(Width, Height);
                        Draw(img);
                        img.Save(filename, ImageFormat.Bmp);
                    }

                    break;

                case ".jpg":
                    {
                        Bitmap img = new Bitmap(Width, Height);
                        Draw(img);
                        img.Save(filename, ImageFormat.Jpeg);
                    }

                    break;

                case ".jpeg":
                    {
                        Bitmap img = new Bitmap(Width, Height);
                        Draw(img);
                        img.Save(filename, ImageFormat.Jpeg);
                    }

                    break;

                case ".gif":
                    {
                        Bitmap img = new Bitmap(Width, Height);
                        Draw(img);
                        img.Save(filename, ImageFormat.Gif);
                    }

                    break;

                case ".tiff":
                    {
                        Bitmap img = new Bitmap(Width, Height);
                        Draw(img);
                        img.Save(filename, ImageFormat.Tiff);
                    }

                    break;

                case ".Wmf":
                    {
                        Bitmap img = new Bitmap(Width, Height);
                        Draw(img);
                        img.Save(filename, ImageFormat.Wmf);
                    }

                    break;

                case ".emf":
                    {
                        Image img = new Bitmap(Width, Height);

                        using (Graphics g = Graphics.FromImage(img))
                        {
                            IntPtr hdc = g.GetHdc();
                            Metafile mf = new Metafile(filename, hdc,
                                new Rectangle(0, 0, Width, Height), MetafileFrameUnit.Pixel);
                            Draw(mf, img.Size);
                            g.ReleaseHdc(hdc);
                            g.Dispose();
                            mf.Dispose();
                        }
                    }

                    break;

                case ".svg":
                    {
                        ToSvg svg = new ToSvg();

                        using (Graphics g = svg.GetRealGraphics(Size))
                        {
                            g.Clear(BackColor);
                            Draw(g, Size);
                            g.Dispose();
                            svg.Save(filename);
                        }
                    }

                    break;

                case ".eps":
                    {
                        ToPostScript eps = new ToPostScript();

                        using (Graphics g = eps.GetRealGraphics(Size))
                        {
                            g.Clear(BackColor);
                            Draw(g, Size);
                            g.Dispose();
                            eps.Save(filename);
                        }
                    }

                    break;

                default:
                    {
                        Bitmap img = new Bitmap(Width, Height);
                        this.Draw(img);
                        img.Save(filename);
                    }

                    break;
            }
        }

        /// <summary>
        /// Returns the minimum size needed to display the chart.
        /// </summary>
        /// <returns>Minimal size.</returns>
        public SizeF GetMinSize()
        {
            float width = 0;
            float height = 0;

            using (Graphics g = this.GetGraphics())
            {
                foreach (ChartSeries ser in Series)
                {
                    SizeF sz = ser.Renderer.GetMinSize(g);

                    width = Math.Max(width, sz.Width);
                    height = Math.Max(height, sz.Height);
                }

                width += ChartArea.ChartAreaMargins.Left + ChartArea.ChartAreaMargins.Right +
                    Size.Width - ChartArea.Width;
                height += ChartArea.ChartAreaMargins.Top + ChartArea.ChartAreaMargins.Bottom +
                    Size.Height - ChartArea.Height;
            }

            return new SizeF(width, height);
        }

        /// <summary>
        /// This method overrides <see cref="Control.Refresh"/>
        /// </summary>
        public override void Refresh()
        {
            this.RefreshArea();
            base.Refresh();
        }
        #endregion

        #region Events
        /// <summary>
        /// Event that is raised when the ChartArea is painted.
        /// </summary>
        [Category("Appearance")]
        [Description("Event that is raised when the ChartArea is painted.")]
        public event PaintEventHandler ChartAreaPaint;

				/// <summary>
				/// Event that is raised before the ChartArea is painted.
				/// </summary>
				[Category("Appearance")]
				[Description("Event that is raised before the ChartArea is painted.")]
				public event PaintEventHandler PreChartAreaPaint;

        /// <summary>
        /// Event that is raised when image is created for chart.
        /// </summary>
        [Category("Appearance")]
        [Description("Event that is raised when image is created for chart.")]
        public event ChartAreaImageEventHandler ChartImage;

        /// <summary>
        /// Event for dynamic formatting of axis labels.
        /// </summary>
        [Category("Behavior")]
        [Description("Event for dynamic formatting of axis labels.")]
        public event ChartFormatAxisLabelEventHandler ChartFormatAxisLabel;

        /// <summary>
        /// Event that will be raised when Chart completed updating series and finds out that series are incompatible.
        /// </summary>
        [Category("Behavior")]
        [Description("Event that will be raised when Chart completed updating series and finds out that series are incompatible.")]
        public event EventHandler SeriesIncompatible;

        /// <summary>
        /// Event that will be raised when Chart has completed laying out of axes, legend
        /// </summary>
        [Category("Layout")]
        [Description("Event that will be raised when Chart has completed laying out of axes, legend.")]
        public event EventHandler LayoutCompleted;

        /// <summary>
        /// Event that will be raised when the mouse is clicked within a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse is clicked within a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionClick;

        /// <summary>
        /// Event that will be raised when the mouse is double clicked within a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse is double clicked within a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionDoubleClick;

        /// <summary>
        /// Event that will be raised when the mouse is pressed down within a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse is pressed down within a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionMouseDown;

        /// <summary>
        /// Event that will be raised when the mouse enters a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse enters a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionMouseEnter;

        /// <summary>
        /// Event that will be raised when the mouse hovers over a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse hovers over a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionMouseHover;

        /// <summary>
        /// Event that will be raised when the mouse leaves a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse leaves a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionMouseLeave;

        /// <summary>
        /// Event that will be raised when the mouse is released within a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse is released within a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionMouseMove;

        /// <summary>
        /// Event that will be raised when the mouse is released within a chart region.
        /// </summary>
        [Category("Regions")]
        [Description("Event that will be raised when the mouse is released within a chart region.")]
        public event ChartRegionMouseEventHandler ChartRegionMouseUp;
        
        /// <summary>
        /// Event that will be raised when the visible range changes during zooming.
        /// </summary>
        [Category("Zooming")]
        [Description("Event that will be raised when the visible range changes during zooming.")]
        public event EventHandler VisibleRangeChanged;

        /// <summary>
        /// Event that will be raised when the visible range is changing during zooming.
        /// </summary>
        [Category("Zooming")]
        [Description("Event that will be raised when the visible range is changing during zooming.")]
        public event ChartAxisZoomingEventHandler VisibleRangeChanging;      
        #endregion

        #region Event risers
        /// <summary>
        /// Method is called for drawing of area. Calling in the method _Paint.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected virtual void OnChartAreaPaint(PaintEventArgs e)
        {
            if (this.ChartAreaPaint != null)
            {
                try
                {
                    this.ChartAreaPaint(m_chartArea, e);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

                    if (ex.InnerException != null)
                    {
                        Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");
                    }
                }
            }
        }

		/// <summary>
		/// Method is called for drawing of area. Calling in the method _Paint.
		/// </summary>
		/// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
		protected virtual void OnPreChartAreaPaint(PaintEventArgs e)
		{
			if (this.PreChartAreaPaint != null)
			{
				try
				{
					this.PreChartAreaPaint(m_chartArea, e);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace, "Exception");

					if (ex.InnerException != null)
					{
						Debug.WriteLine(ex.InnerException.Message + Environment.NewLine + ex.InnerException.StackTrace, "Inner Exception");
					}
				}
			}
		}

        /// <summary>
        /// Raises the <see cref="E:ChartAreaImage"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Syncfusion.Windows.Forms.Chart.ChartAreaImageEventArgs"/> instance containing the event data.</param>
        protected virtual void OnChartAreaImage(ChartAreaImageEventArgs e)
        {
            if (this.ChartImage != null)
            {
                this.ChartImage(this, e);
            }
        }

        /// <summary>
        ///     Method called when the user clicks inside a chart region.
        ///     <seealso cref="ChartControl.ChartRegionClick"/>
        /// </summary>
        /// <param name="e" type="Syncfusion.Windows.Forms.Chart.ChartRegionMouseEventArgs">.
        ///     <para>
        ///     Argument.
        ///     </para>
        /// </param>
        protected virtual void OnChartRegionClick(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionClick != null)
            {
                this.ChartRegionClick(this, e);
            }
        }

        /// <summary>
        /// Method called when the user double clicks inside a chart region.
        /// <seealso cref="ChartControl.ChartRegionDoubleClick"/>
        /// </summary>
        /// <param name="e">.
        /// <para>
        /// Argument.
        /// </para></param>
        protected virtual void OnChartRegionDoubleClick(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionDoubleClick != null)
            {
                this.ChartRegionDoubleClick(this, e);
            }
        }

        /// <summary>
        /// Method called when the mouse is pressed down inside a chart region.
        /// <seealso cref="ChartControl.ChartRegionMouseDown"/>
        /// </summary>
        /// <param name="e">Argument.</param>
        protected virtual void OnChartRegionMouseDown(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionMouseDown != null)
            {
                this.ChartRegionMouseDown(this, e);
            }
        }

        /// <summary>
        /// Method called when the mouse enters a chart region.
        /// <seealso cref="ChartControl.ChartRegionMouseEnter"/>
        /// </summary>
        /// <param name="e">Argument.</param>
        protected virtual void OnChartRegionMouseEnter(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionMouseEnter != null)
            {
                this.ChartRegionMouseEnter(this, e);
            }
        }

        /// <summary>
        /// Method called when the mouse hovers over a chart region.
        /// <seealso cref="ChartControl.ChartRegionMouseHover"/>
        /// </summary>
        /// <param name="e">Argument.</param>
        protected virtual void OnChartRegionMouseHover(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionMouseHover != null)
            {
                this.ChartRegionMouseHover(this, e);
            }
        }

        /// <summary>
        /// Method called when the mouse leaves a chart region.
        /// <seealso cref="ChartControl.ChartRegionMouseLeave"/>
        /// </summary>
        /// <param name="e">Argument.</param>
        protected virtual void OnChartRegionMouseLeave(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionMouseLeave != null)
            {
                this.ChartRegionMouseLeave(this, e);
            }
        }

        /// <summary>
        /// Method called when the mouse is moved over a chart region.
        /// </summary>
        /// <param name="e">Argument.</param>
        protected virtual void OnChartRegionMouseMove(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionMouseMove != null)
            {
                this.ChartRegionMouseMove(this, e);
            }
        }

        /// <summary>
        /// Method called when the mouse is released over a chart region.
        /// </summary>
        /// <param name="e">Argument.</param>
        protected virtual void OnChartRegionMouseUp(ChartRegionMouseEventArgs e)
        {
            if (this.ChartRegionMouseUp != null)
            {
                this.ChartRegionMouseUp(this, e);
            }
        }

        /// <summary>
        /// Override this method to instantiate a derived chart model if needed.
        /// </summary>
        /// <returns>Returns ChartModel.</returns>
        protected virtual ChartModel OnCreateChartModel()
        {
            return new ChartModel();
        }

        /// <summary>
        /// Method is called when the visible range is changed during zooming.
        /// <seealso cref="ChartControl.VisibleRangeChanged"/>
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnVisibleRangeChanged(EventArgs e)
        {
            if (VisibleRangeChanged != null)
            {
                VisibleRangeChanged(this, e);
            }
        }
        #endregion

        #region Implementation

        #region Commands implementation
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameter">The parameter.</param>
        internal void ExecuteCommand(ChartCommand command, string parameter)
        {
            if (command == ChartCommands.Save)
            {
                this.ExecuteSaveCommand(parameter);
            }
            else if (command == ChartCommands.Copy)
            {
                this.ExecuteCopyCommand(parameter);
            }
            else if (command == ChartCommands.Print)
            {
                this.ExecutePrintCommand(parameter);
            }
            else if (command == ChartCommands.PrintPriview)
            {
                this.ExecutePrintPreviewCommand(parameter);
            }
            else if (command == ChartCommands.ZoomIn)
            {
                this.ExecuteZoomInCommand(parameter);
            }
            else if (command == ChartCommands.ZoomOut)
            {
                this.ExecuteZoomOutCommand(parameter);
            }
            else if (command == ChartCommands.ResetZooming)
            {
                this.ExecuteResetZoomingCommand(parameter);
            }
            else if (command == ChartCommands.ShowLegend)
            {
                this.ShowLegend = !this.ShowLegend;
            }
            else if (command == ChartCommands.Toggle3D)
            {
                this.Series3D = !this.Series3D;
            }
            else if (command == ChartCommands.ToggleXZooming)
            {
                this.EnableXZooming = !this.EnableXZooming;
            }
            else if (command == ChartCommands.ToggleYZooming)
            {
                this.EnableYZooming = !this.EnableYZooming;
            }
            else if (command == ChartCommands.TogglePanning)
            {
                if (this.MouseAction == ChartMouseAction.Zooming)
                {
                    this.MouseAction = ChartMouseAction.Panning;

                    foreach (ChartAxis axis in this.Axes)
                    {
                        axis.ZoomActions = ChartZoomingAction.Panning;
                    }
                }
                else
                {
                    this.MouseAction = ChartMouseAction.Zooming;

                    foreach (ChartAxis axis in this.Axes)
                    {
                        axis.ZoomActions = ChartZoomingAction.None;
                    }
                }
            }
            else if (command == ChartCommands.AutoHighlight)
            {
                this.AutoHighlight = !this.AutoHighlight;
            }
        }

        /// <summary>
        /// Determines whether the specified command is toogled.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameter">The parameter.</param>
        internal bool IsCommandToggled(ChartCommand command, string parameter)
        {
            if (command == ChartCommands.ShowLegend)
            {
                return this.ShowLegend;
            }
            else if (command == ChartCommands.Toggle3D)
            {
                return this.Series3D;
            }
            else if (command == ChartCommands.ToggleXZooming)
            {
                return this.EnableXZooming;
            }
            else if (command == ChartCommands.ToggleYZooming)
            {
                return this.EnableYZooming;
            }
            else if (command == ChartCommands.TogglePanning)
            {
                return this.MouseAction == ChartMouseAction.Panning;
            }
            else if (command == ChartCommands.AutoHighlight)
            {
                return this.AutoHighlight;
            }

            return false;
        }

        /// <summary>
        /// Executes the save command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecuteSaveCommand(string parameter)
        {            
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "Image file(*.bmp,*.jpeg,*.jpg,*.tiff,*.gif,*.emf)|*.bmp;*.jpeg;*.jpg;*.tiff;*.gif;*.emf|All files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.SaveImage(saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Executes the copy command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecuteCopyCommand(string parameter)
        {
            Image img = new Bitmap(this.Width, this.Height);
            this.Draw(img);
            Clipboard.SetDataObject(img);
        }

        /// <summary>
        /// Executes the print command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecutePrintCommand(string parameter)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = this.PrintDocument;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    this.PrintDocument.Print();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        /// <summary>
        /// Executes the print command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecutePrintPreviewCommand(string parameter)
        {
            PrintPreviewDialog printDialog = new PrintPreviewDialog();

            printDialog.Document = this.PrintDocument;
            printDialog.ShowDialog();
        }

        /// <summary>
        /// Executes the zoom in command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecuteZoomInCommand(string parameter)
        {
            if (m_enableXZooming)
                this.MulZoomFactor(0.5, ChartOrientation.Horizontal);
            if (m_enableYZooming)
                this.MulZoomFactor(0.5, ChartOrientation.Vertical);
        }

        /// <summary>
        /// Executes the zoom in command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecuteZoomOutCommand(string parameter)
        {
            if (m_enableXZooming)
            {
                this.MulZoomFactor(2, ChartOrientation.Horizontal);
            }
            if (m_enableYZooming)
            {
                this.MulZoomFactor(2, ChartOrientation.Vertical);
            }
        }

        /// <summary>
        /// Executes the zoom in command.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        private void ExecuteResetZoomingCommand(string parameter)
        {
            this.ResetZoom();
        }
        #endregion

        #region ShouldSerialize And Reset Methods
        /// <summary>
        /// Used by the designer to persist the current value of the Font property.
        /// </summary>
        /// <returns>True if the value is to be persisted, otherwise false.</returns>
        protected bool ShouldSerializeChartAreaMargins()
        {
            return !this.ChartAreaMargins.Equals(DefaultChartAreaMargins);
        }

        /// <summary>
        /// Used by the designer to reset the current value of the BackFont property.
        /// </summary>
        protected void ResetChartAreaMargins()
        {
            this.ChartAreaMargins = new ChartMargins(10, 10, 10, 10);
        }

        /// <summary>
        /// Gets  default value for ChartMargins property (Used only by VS designer).
        /// </summary>
        protected ChartMargins DefaultChartAreaMargins
        {
            get
            {
                return new ChartMargins(10, 10, 10, 10);
            }
        }

        /// <summary>
        /// Used by the designer to persist the current value of the Font property.
        /// </summary>
        /// <returns>true if the value is to be persisted.</returns>
        protected bool ShouldSerializeShadowColor()
        {
            return !m_shadowInterior.Equals(c_defShadowInterior);
        }

        /// <summary>
        /// Used by the designer to reset the current value of the BackFont property.
        /// </summary>
        protected void ResetShadowColor()
        {
            this.ShadowColor = new BrushInfo(c_defShadowInterior);
        }

        /// <summary>
        /// Used by the designer to persist the current value of the BackInterior property.
        /// </summary>
        /// <returns>true if the value is to be persisted.</returns>
        protected bool ShouldSerializeBackInterior()
        {
            return !m_backInterior.Equals(c_defBackInterior);
        }

        /// <summary>
        /// Used by the designer to reset the current value of the BackInterior property.
        /// </summary>
        protected void ResetBackInterior()
        {
            BackInterior = new BrushInfo(c_defBackInterior);
        }

        /// <summary>
        /// Used by the designer to reset the current value of the ChartInterior property.
        /// </summary>
        protected void ResetChartInterior()
        {
            this.ChartInterior = new BrushInfo(Color.White);
        }

        /// <summary>
        /// Used by the designer to persist the current value of the ChartInterior property.
        /// </summary>
        /// <returns>true if the value is to be persisted.</returns>
        protected bool ShouldSerializeChartInterior()
        {
            return !BrushInfo.Equals(m_chartArea.GridBackInterior, new BrushInfo(Color.White));
        }

        /// <summary>
        /// Used by the designer to reset the current value of the Text property.
        /// </summary>
        public override void ResetText()
        {
            if (this.Site != null)
            {
                this.Text = this.Site.Name;
            }
            else
            {
                base.ResetText();
            }
        }

        /// <summary>
        /// Should the serialize text.
        /// </summary>
        /// <returns></returns>
        private bool ShouldSerializeText()
        {
            return (this.Text != String.Empty);
        }

        /// <summary>
        /// Should the serialize legends.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        private bool ShouldSerializeLegends()
        {
            return m_legends != null && m_legends.Count > 1;
        }

        /// <summary>
        /// Should the serialize series.
        /// </summary>
        /// <returns>Returns true if the element should serialize otherwise false.</returns>
        private bool ShouldSerializeSeries()
        {
            return !this.RandomDataPresent;
        }

        /// <summary>
        /// Resets the series.
        /// </summary>
        private void ResetSeries()
        {
            this.Series.Clear();
        }
        #endregion

        #region Zooming Implementation
        /// <summary>
        /// Call this method for updated settings of the scrollbar corresponding to the axis supplied.
        /// </summary>
        /// <param name="axis">An Axis that indicates the scrollbar.</param>
        public void ZoomingChanged(ChartAxis axis)
        {
            m_bInZoomTransaction = true;

            IChartScrollBar scroolBar = axis.Orientation == ChartOrientation.Horizontal ?
                this.GetHScrollBar(axis) : this.GetVScrollBar(axis);

            if (axis.ZoomFactor == 1d || !m_showScrollBars)
            {
                if (scroolBar.Visible)
                {
                    scroolBar.Visible = false;
                    m_needRecalculateSizes = true;
                }
            }
            else
            {
                MinMaxInfo zr = axis.ZoomedRange;
                double maxValue = 1 / axis.ZoomFactor;
                double zoomPosX = axis.GetScrollBarValueFromZoomPosition();

                if (maxValue <= 1)
                {
                    scroolBar.Visible = false;
                }
                else
                {
                    scroolBar.Minimum = 0;
                    scroolBar.Maximum = (int)Math.Round(m_scrollPrecision * maxValue * zr.NumberOfIntervals);
                    scroolBar.LargeChange = zr.NumberOfIntervals * m_scrollPrecision;
                    scroolBar.SmallChange = m_scrollPrecision;

                    int val = (int)Math.Max(0, Math.Round(scroolBar.Maximum * zoomPosX));

                    scroolBar.Value = val;
                    scroolBar.Visible = true;
                    m_needRecalculateSizes = true;
                }
            }

            m_bInZoomTransaction = false;
        }
        /// <summary>
        /// Changes the scroll bars visibility.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        private void ChangeScrollBars(bool value)
        {
            foreach (ChartAxis axis in this.Axes)
            {

                IChartScrollBar scroolBar = axis.Orientation == ChartOrientation.Horizontal ?
                    this.GetHScrollBar(axis) : this.GetVScrollBar(axis);

                scroolBar.Visible = value;

            }
        }

        /// <summary>
        /// Method called when the value of horizontal scrollbar was changed.
        /// </summary>
        /// <param name="sender">The sender of event.</param>
        /// <param name="e">Argument.</param>
        protected void OnScrollBarValueChanged(object sender, ChartScrollBarValueChangedEventArgs e)
        {
            IChartScrollBar scrollBar = (IChartScrollBar)sender;
            ChartAxis axis = scrollBar.ChartAxis;

            if (!m_bInZoomTransaction)
            {
                double zoomPosition = (double)scrollBar.Value / scrollBar.Maximum;

                if (axis.Inversed ^ axis.Orientation == ChartOrientation.Vertical)
                {
                    axis.ZoomPosition = 1 - axis.ZoomFactor - zoomPosition;
                }
                else
                {
                    axis.ZoomPosition = zoomPosition;
                }
            }
        }

        /// <summary>
        /// Method called when the user clicks on the zoom button.
        /// </summary>
        /// <param name="sender">The sender of event.</param>
        /// <param name="e">Argument.</param>
        private void OnScrollBarZoomButtonClick(object sender, ChartScrollBarZoomButtonClickedEventArgs e)
        {
            ChartAxis axis = (sender as IChartScrollBar).ChartAxis;

            axis.ZoomFactor = Math.Min(1, axis.ZoomFactor + m_zoomOutIncrement);

            this.ZoomingChanged(axis);
            this.Redraw(true);
        }

        /// <summary>
        /// Returns the horizontal scrollbar corresponding to the axis supplied.
        /// </summary>
        /// <param name="axis">Instance of ChartAxis.</param>
        /// <returns>The object that implements interface <see cref="IChartScrollBar"/>.</returns>
        public IChartScrollBar GetHScrollBar(ChartAxis axis)
        {
            if (scrollBarH.Contains(axis))
            {
                return (IChartScrollBar)scrollBarH[axis];
            }
            else
            {
                ChartHScrollBar hscr = new ChartHScrollBar();
                hscr.Size = new Size(1, 1);
                hscr.ChartAxis = axis;
                hscr.Dock = DockStyle.None;
                hscr.SendToBack();
                hscr.Visible = false;
                axis.ScrollBar = hscr;
                this.Controls.Add(hscr);
                scrollBarH.Add(axis, hscr);
                hscr.ZoomButtonClicked += new ChartScrollBarZoomButtonClickedEventHandler(this.OnScrollBarZoomButtonClick);
                hscr.ValueChanged += new ChartScrollBarValueChangedEventHandler(this.OnScrollBarValueChanged);
                return hscr;
            }
        }

        /// <summary>
        /// Returns the vertical scrollbar corresponding to the axis supplied.
        /// </summary>
        /// <param name="axis">Instance of ChartAxis.</param>
        /// <returns>The object that implements interface <see cref="IChartScrollBar"/>.</returns>
        public IChartScrollBar GetVScrollBar(ChartAxis axis)
        {
            if (scrollBarV.Contains(axis))
            {
                return (IChartScrollBar)scrollBarV[axis];
            }
            else
            {
                ChartVScrollBar vscr = new ChartVScrollBar();
                vscr.Size = new Size(1, 1);
                vscr.ChartAxis = axis;
                vscr.SendToBack();
                vscr.Dock = DockStyle.None;
                vscr.Visible = false;
                axis.ScrollBar = vscr;
                this.Controls.Add(vscr);
                scrollBarV.Add(axis, vscr);
                vscr.ZoomButtonClicked += new ChartScrollBarZoomButtonClickedEventHandler(this.OnScrollBarZoomButtonClick);
                vscr.ValueChanged += new ChartScrollBarValueChangedEventHandler(this.OnScrollBarValueChanged);
                return vscr;
            }
        }

        /// <summary>
        /// Zooms the all necessary axes by the selected region.
        /// </summary>
        /// <param name="upPoint">The started position of the zoomed region.</param>
        /// <param name="downPoint">The ended position of the zoomed region.</param>
        /// <param name="orientation">Orientation of the necessary axes</param>
        private void ZoomNeededAxes(PointF upPoint, PointF downPoint, ChartOrientation orientation)
        {
            RectangleF bounds = this.ChartArea.RenderBounds;

            if (Series3D && RealMode3D)
            {
                upPoint = this.ChartArea.CorrectionTo(upPoint);
                downPoint = this.ChartArea.CorrectionTo(downPoint);
            }

            upPoint = new PointF(ChartMath.MinMax(upPoint.X, bounds.Left, bounds.Right),
                ChartMath.MinMax(upPoint.Y, bounds.Top, bounds.Bottom));
            downPoint = new PointF(ChartMath.MinMax(downPoint.X, bounds.Left, bounds.Right),
                ChartMath.MinMax(downPoint.Y, bounds.Top, bounds.Bottom));

            double minZoomFactor = orientation == ChartOrientation.Vertical ? m_minZoomFactorY : m_minZoomFactorX;

            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.Orientation == orientation)
                {
                    axis.Zoom(upPoint, downPoint, minZoomFactor);
                }
            }
        }

        /// <summary>
        /// Multiplies zoomfactor of the all necessary axes.
        /// </summary>
        /// <param name="coef">It value will be multiplied by zoomfactor.</param>
        /// <param name="orientation">Orientation of the necessary axes.</param>
        private void MulZoomFactor(double coef, ChartOrientation orientation)
        {
            double minZoomFactor = orientation == ChartOrientation.Vertical ?
                m_minZoomFactorY : m_minZoomFactorX;

            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.Orientation == orientation)
                {
                    axis.CenteredZoom(ChartMath.MinMax(axis.ZoomFactor * coef, minZoomFactor, 1d));
                }
            }
        }

        /// <summary>
        /// Shifts zoomposition of the all necessary axes.
        /// </summary>
        /// <param name="coef">It value will be added by zoomfactor.</param>
        /// <param name="orientation">Orientation of the necessary axes</param>
        private void ShiftZoomPosition(double coef, ChartOrientation orientation)
        {
            foreach (ChartAxis axis in this.Axes)
            {
                if (axis.Orientation == orientation)
                {
                    axis.ZoomPosition += coef * axis.ZoomFactor;
                }
            }
        }

        /// <summary>
        /// Resets zoom.
        /// </summary>
        private void ResetZoom()
        {
            foreach (ChartAxis axis in this.Axes)
            {
                axis.ResetZoom();
            }
        }
        #endregion

        #region SideBySideInfo and StackInfo methods
        /// <summary>
        /// Returns the value of side by side displacement in pixels.
        /// </summary>
        /// <param name="ser">Instance of <see cref="ChartSeries"/>.</param>
        /// <param name="renderer">Instance of <see cref="ChartSeriesRenderer"/>.</param>
        /// <returns>A instance of PointF where X - location of side; Y - width of side.</returns>
        [Obsolete("This method isn't used anymore and may be deleted.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PointF GetSideBySideInfo(ChartSeries ser, ChartSeriesRenderer renderer)
        {
            int pos = -1;
            int all = 0;
            bool invert = ser.RequireInvertedAxes;

            GetSideBySidePositions(ser, out all, out pos);

            double w, h;

            if (ColumnWidthMode == ChartColumnWidthMode.FixedWidthMode)
            {
                w = ColumnFixedWidth;
            }
            else
            {
                w = (renderer.DividedIntervalSpace.Width) * (1 - Spacing / 100);
            }

            h = (renderer.DividedIntervalSpace.Height) * (1 - Spacing / 100);

            double div = invert ? h / all : w / all;
            double loc = (invert ? div * pos - h / 2 : w / 2 - div * pos);

            return new PointF((float)loc, (float)div);
        }

        /// <summary>
        /// Returns the value of side by side displacement in chart coordinates.
        /// </summary>
        /// <param name="ser">Instance of <see cref="ChartSeries"/>.</param>
        /// <param name="renderer">Instance of <see cref="ChartSeriesRenderer"/>.</param>
        /// <returns>A instance of ChartPoint where X - location of side; Y - width of side.</returns>
        [Obsolete("This method isn't used anymore and may be deleted.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ChartPoint GetSideBySideInfoValue(ChartSeries ser, ChartSeriesRenderer renderer)
        {
            int pos = -1;
            int all = 0;
            bool invert = ser.RequireInvertedAxes;

            GetSideBySidePositions(ser, out all, out pos);

            double w = 0, h = 0;

            if (ColumnWidthMode == ChartColumnWidthMode.FixedWidthMode)
            {
                if (renderer.DividedIntervalSpace.Width != 0)
                {
                    w = ColumnFixedWidth / renderer.DividedIntervalSpace.Width;
                }
            }
            else
            {
                w = this.MinPointsDelta * (1 - Spacing / 100);
                h = this.MinPointsDelta * (1 - Spacing / 100);
            }

            double div = invert ? h / all : w / all;
            double l = div * (pos - 1);
            double loc = (invert ? h / 2 - l : w / 2 - l);

            return new ChartPoint(loc, div);
        }

        /// <summary>
        /// Returns the value of side by side displacement.
        /// </summary>
        /// <param name="ser">Instance of <see cref="ChartSeries"/>.</param>
        /// <param name="all">A sum of all sides.</param>
        /// <param name="pos">Position of side of a series.</param>
        private void GetSideBySidePositions(ChartSeries ser, out int all, out int pos)
        {
            pos = -1;
            all = 0;

            Hashtable stackedSeriesTypes = new Hashtable(5);
            if (ser.BaseType == ChartSeriesBaseType.SideBySide)
            {
                for (int i = 0; i < Series.VisibleCount; i++)
                {
                    ChartSeries seriesVisibleByI = Series.GetSeriesByVisible(i);
                    if (seriesVisibleByI.BaseType == ChartSeriesBaseType.SideBySide)
                    {
                        if (seriesVisibleByI.BaseStackingType == ChartSeriesBaseStackingType.NotStacked)
                        {
                            all++;

                            if (seriesVisibleByI == ser)
                            {
                                pos = all;
                            }
                        }
                        else
                        ////if( seriesVisibleByI.BaseStackingType == ChartSeriesBaseStackingType.Stacked )
                        {
                            if (stackedSeriesTypes.Contains(ser.Type))
                            {
                                if (seriesVisibleByI == ser)
                                {
                                    pos = (int)stackedSeriesTypes[ser.Type];
                                }
                            }
                            else
                            {
                                all++;
                                stackedSeriesTypes.Add(ser.Type, all);
                                if (seriesVisibleByI == ser)
                                {
                                    pos = all;
                                }
                            }
                        }
                    }
                }
            }

            if (all < 1)
            {
                all = 1;
                pos = 1;
            }
        }

        /// <summary>
        /// This method is used when series are rendered stacked. The value returned is a cumulative value of
        /// Y from all series that are below the series passed in in the contained <see cref="ChartSeriesCollection"/>.
        /// <seealso cref="ChartSeriesRenderer"/>
        /// </summary>
        /// <param name="ser">Instance of the ChartSeries.</param>
        /// <param name="pos">The index value of the point</param>
        /// <returns>A sum of Y values from all series are below the series.</returns>
        [Obsolete("This method isn't used anymore and may be deleted.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetStackInfo(ChartSeries ser, int pos)
        {
            return GetStackInfo(ser, pos, false);
        }

        /// <summary>
        /// This method is used when series are rendered stacked. The value returned is a cumulative value of
        /// Y from all series that are below the series passed in in the contained <see cref="ChartSeriesCollection"/>.
        /// <seealso cref="ChartSeriesRenderer"/>
        /// </summary>
        /// <param name="ser">Instance of the ChartSeries.</param>
        /// <param name="pos">The index value of the point</param>
        /// <param name="isWithMe">If true the value form this series added too.</param>
        /// <returns>A sum of Y values from all series are below the series.</returns>
        [Obsolete("This method isn't used anymore and may be deleted.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double GetStackInfo(ChartSeries ser, int pos, bool isWithMe)
        {
            double y = isWithMe ? ser.Points[pos].YValues[0] : 0;
            double x = ser.Points[pos].X;
            double all = 0;

            bool isFullStacking = ser.BaseStackingType == ChartSeriesBaseStackingType.FullStacked;
            bool isEnd = false;

            if (ser.BaseStackingType != ChartSeriesBaseStackingType.NotStacked)
            {
                for (int i = 0, c = Series.VisibleCount; (i < c) && (!isEnd || isFullStacking); i++)
                {
                    ChartSeries curr = Series.GetSeriesByVisible(i);
                    isEnd = (!isEnd) ? (curr == ser) : isEnd;

                    if ((curr.Type == ser.Type) && (curr.BaseStackingType == ser.BaseStackingType))
                    {
                        for (int j = 0; j < curr.Points.Count; j++)
                        {
                            if ((!curr.Points[j].IsEmpty) && (curr.Points[j].X == x))
                            {
                                if (!isEnd)
                                {
                                    y += curr.Points[j].YValues[0];
                                }

                                all += curr.Points[j].YValues[0];
                                break;
                            }
                        }
                    }
                }

                if (isFullStacking)
                {
                    y = all == 0 ? 0 : this.ChartArea.FullStackMax * y / all;
                }
            }

            return y;
        }
        #endregion

        /// <summary>
        /// Updates necessary sizes.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void RecalculateSizes()
        {
            using (Graphics g = this.GetGraphics())
            {
                RecalculateSizes(g);
            }
        }

        /// <summary>
        /// Updates necessary sizes.
        /// </summary>
        /// <param name="g">Instance of Graphics.</param>    
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void RecalculateSizes(Graphics g)
        {
            Rectangle rect = new Rectangle(Point.Empty, this.VirtualSize);

            m_toolBarDockingManager.Spacing = 0;
            rect = m_toolBarDockingManager.DoLayout(rect);

            if (m_borderAppearance != null)
            {
                m_borderBounds = rect;
                rect = m_borderAppearance.Thickness.Deflate(rect);
            }

            m_titleDockingManager.Spacing = m_elementsSpacing;
            rect = m_titleDockingManager.DoLayout(rect);

            rect.Inflate(m_elementsSpacing, m_elementsSpacing);
            m_dockingManager.Spacing = m_elementsSpacing;

            if (m_dockingManager.Placement == ChartPlacement.Outside)
            {
                rect = m_dockingManager.DoLayout(rect);
            }
            else
            {
                rect.Inflate(-m_elementsSpacing, -m_elementsSpacing);
            }

            m_chartArea.CalculateSizes(rect);

            if (m_dockingManager.Placement == ChartPlacement.Inside)
            {
                m_dockingManager.DoLayout(m_chartArea.RenderBounds);
            }

            #region ScrollBars
            int index = 0;
            RectangleF[] csbRects = new RectangleF[Axes.Count-1];
            Rectangle areaRect = rect;

            areaRect.X += (int)(m_chartArea.ChartAreaMargins.Left);
            areaRect.Width -= (int)(m_chartArea.ChartAreaMargins.Left + m_chartArea.ChartAreaMargins.Right);
            areaRect.Y += (int)(m_chartArea.ChartAreaMargins.Top);
            areaRect.Height -= (int)(m_chartArea.ChartAreaMargins.Top + m_chartArea.ChartAreaMargins.Bottom);

            foreach (IChartScrollBar csb in scrollBarV.Values)
                {
                    if (csb.Visible)
                    {
                        if (this.Axes.Count > 2)
                        {
                            RectangleF axisRect = csb.ChartAxis.Rect;

                            RectangleF csbRect = new RectangleF(areaRect.Right, axisRect.Top,
                                csb.Dimension, axisRect.Height);

                            for (int i = 0; i < index; i++)
                            {
                                if (csbRects[i].IntersectsWith(csbRect))
                                {
                                    csbRect.X = csbRects[i].Left - csb.Dimension;
                                }
                            }

                            csb.ChartArea = this.ChartArea;
                            csb.SetPosition(Rectangle.Round(csbRect));
                            csbRects[index++] = csbRect;
                        }
                        else
                        {
                            RectangleF axisRect = csb.ChartAxis.Rect;

                            RectangleF csbRect = new RectangleF(areaRect.Right - csb.Dimension, axisRect.Top,
                                csb.Dimension, axisRect.Height);

                            for (int i = 0; i < index; i++)
                            {
                                if (csbRects[i].IntersectsWith(csbRect))
                                {
                                    csbRect.X = csbRects[i].Left + csb.Dimension;
                                }
                            }

                            csb.ChartArea = this.ChartArea;
                            csb.SetPosition(Rectangle.Round(csbRect));
                            csbRects[index++] = csbRect;
                        }
                    }
                }
            

            index = 0;

            foreach (IChartScrollBar csb in scrollBarH.Values)
            {
                if (csb.Visible) //Check condition whether the Axis scrollbar visible or not to draw rectangle
                {
                    RectangleF axisRect = csb.ChartAxis.Rect;
                    RectangleF csbRect = new RectangleF(axisRect.X, areaRect.Bottom - csb.Dimension,
                        axisRect.Width, csb.Dimension);

                    for (int i = 0; i <= index; i++)
                    {
                        if (csb.Visible)
                        {
                            if (csbRects[i].IntersectsWith(csbRect))
                            {
                                csbRect.Y = csbRects[i].Top - csb.Dimension;
                            }
                        }
                        else
                        {
                            csbRect.Y = csbRects[i].Top;
                        }
                    }

                    csb.ChartArea = this.ChartArea;
                    csb.SetPosition(Rectangle.Round(csbRect));
                    csbRects[index++] = csbRect;
                }
            }
            #endregion

            this.RaiseLayoutCompleted(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the LayoutCompleted event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        public void OnLayoutCompleted(object sender, EventArgs e)
        {
            if (LayoutCompleted != null)
            {
                LayoutCompleted(sender, e);
            }
        }

        /// <summary>
        /// Raises the SeriesIncompatible event.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object that contains the event data.</param>
        public void OnSeriesIncompatible(object sender, EventArgs e)
        {
            if (SeriesIncompatible != null)
            {
                SeriesIncompatible(this, e);
            }
        }

        /// <summary>
        /// Method is called when the series was changed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SeriesChanged(object sender, ChartSeriesCollectionChangedEventArgs e)
        {
            if (e.ChangeType != ChartSeriesCollectionChangeType.Changed
                && this.RandomDataPresent && !m_inRandomDataMode)
            {
                if (this.Series.Count > (m_addRandomData ? 1 : 0))
                {
                    this.ResetRandomData();
                }
            }

            m_minPointsDelta = double.NaN;

            if (e.ChangeType == ChartSeriesCollectionChangeType.Added)
            {
                e.Series.AppearanceChanged += new EventHandler(OnSeriesAppearanceChanged);
            }
            else if (e.ChangeType == ChartSeriesCollectionChangeType.Removed)
            {
                e.Series.AppearanceChanged -= new EventHandler(OnSeriesAppearanceChanged);
            }

            if (Series.Count == 0)
                this.AddRandomData();

            CheckAndPrepareSeries();

            m_axesSeriesAndChartShouldBePrepared = true;

            if (!this.ImprovePerformance)
                this.PrepareAxesSeriesAndChart();

            foreach (ChartSeries s in this.Series)
            {
                if (s.BaseType == ChartSeriesBaseType.Single || s.BaseType == ChartSeriesBaseType.Circular)
                {
                    this.ResetZoom();
                }
            }

            this.Redraw(true);
        }

        /// <summary>
        /// This method is called when the <see cref="ChartControl.Legends"/> collection was changed.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        private void OnLegendsChanged(ChartBaseList list, ChartListChangeArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (ChartLegend legend in args.NewItems)
                {
                    legend.MouseDown += new MouseEventHandler(this.LegendMouseDown);

                    this.Controls.Add(legend);
                    this.DockingManager.Add(legend);
                }
            }

            if (args.OldItems != null)
            {
                foreach (ChartLegend legend in args.OldItems)
                {
                    legend.MouseDown -= new MouseEventHandler(this.LegendMouseDown);

                    this.DockingManager.Remove(legend);
                    this.Controls.Remove(legend);
                }
            }
        }

        /// <summary>
        /// This method is called when the <see cref="ChartControl.Titles"/> collction was changed.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="args"></param>
        private void OnTitlesChanged(ChartBaseList list, ChartListChangeArgs args)
        {
            if (args.OldItems != null)
            {
                foreach (ChartTitle title in args.OldItems)
                {
                    m_titleDockingManager.Remove(title);
                    this.Controls.Remove(title);
                }
            }

            if (args.NewItems != null)
            {
                foreach (ChartTitle title in args.NewItems)
                {
                    this.Controls.Add(title);
                    m_titleDockingManager.Add(title);
                }
            }

            this.Redraw(true);
        }

        /// <summary>
        /// Wires the all series to the chart.
        /// </summary>
        private void CheckAndPrepareSeries()
        {
            for (int i = 0; i < Series.Count; i++)
            {
                ChartSeries series = Series[i];

                series.Renderer.SetChart(this);
                series.ChartModel = this.Model;

                if (series.XAxis == null)
                {
                    series.XAxis = this.PrimaryXAxis;
                }

                if (series.YAxis == null)
                {
                    series.YAxis = this.PrimaryYAxis;
                }
            }
        }

        /// <summary>
        /// Computes ranges of the axes by the points and series type.
        /// </summary>
        private void PrepareAxesSeriesAndChart()
        {
            if (disposeStarted && updating)
            {
                return;
            }

            if (m_axesSeriesAndChartShouldBePrepared)
            {
                this.CheckSeriesCompatibility();
                this.Model.UpdateArea(this.ChartArea);

                m_axesSeriesAndChartShouldBePrepared = false;
            }
        }

        /// <summary>
        /// Calling in the method ChartArea.OnLayoutCompleted.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseLayoutCompleted(EventArgs e)
        {
            OnLayoutCompleted(this, e);
        }

        /// <summary>
        /// Calling in the method ChartArea.OnSeriesIncompatible.
        /// </summary>
        /// <param name="e"></param>
        private void RaiseSeriesIncompatible(EventArgs e)
        {
            OnSeriesIncompatible(this, e);
        }

        /// <summary>
        /// Draws control.
        /// </summary>
        /// <param name="g">Instance of the Graphics.</param>
        /// <param name="r">The Rectangle within which to paint.</param>
        private void _Paint(Graphics g, Rectangle r)
        {
            if (NeedRegionUpdate)
            {
                m_chartRegions.Clear();
                m_chartRegions.Add(new ChartRegion(new Region(ClientRectangle), chartToolTip, "ChartControl Region"));
            }

            g.TextRenderingHint = TextRenderingHint;
            g.SmoothingMode = SmoothingMode;

            if (m_borderAppearance == null)
            {
                BrushPaint.FillRectangle(g, m_borderBounds, this.BackInterior);
            }
            else
            {
#if SyncfusionFramework2_0
                m_borderAppearance.Draw(g, m_borderBounds,
                    this.BackInterior, this.BackgroundImage, (ChartImageLayout)this.BackgroundImageLayout);
#else
				m_borderAppearance.Draw(g, m_borderBounds,
					this.BackInterior, this.BackgroundImage, ChartImageLayout.None);
#endif
            }

            // Drawing shadow.
            if (ChartAreaShadow && !Series3D)
            {
                Rectangle rc = new Rectangle(m_chartArea.Location.X + ShadowWidth, m_chartArea.Location.Y + ShadowWidth,
                    m_chartArea.Width, m_chartArea.Height);
                BrushPaint.FillRectangle(g, rc, ShadowColor);
            }

						PaintEventArgs args = new PaintEventArgs(g,r);

						this.OnPreChartAreaPaint(args);

						m_chartArea.Draw(args);

						this.OnChartAreaPaint(args);

            m_needRegionUpdate = false;
        }

        #region Random data methods
        /// <summary>
        /// Adds the random count of series and random points to them.
        /// </summary>
        private void AddRandomData()
        {
            if (m_addRandomData)
            {
                m_inRandomDataMode = true;

                for (int i = 0; i < c_randomSeriesCount; i++)
                {
                    ChartSeries series = new ChartSeries(c_internalSeriesName + i.ToString(), this.designTimeSeriesType);
                    series.Text = "Series " + i.ToString();
                    Random random = new Random(DateTime.Now.Millisecond);

                    for (int j = 1; j < c_randomSeriesPointCount; j++)
                    {
                        double[] points = new double[] { 
            random.Next(c_randomSeriesMaxY1), random.Next(c_randomSeriesMaxY2), 
            random.Next(c_randomSeriesMaxY3), random.Next(c_randomSeriesMaxY4) };
                        Array.Sort(points);
                        Array.Reverse(points);
                        series.Points.Add(j, points);
                    }
                    Model.Series.Add(series);
                }

                m_inRandomDataMode = false;
                RecalculateSizes();
            }
        }

        /// <summary>
        /// Change the type of series of random data on designTimeSeriesType.
        /// </summary>
        private void ChangeRandomDataSeries()
        {
            m_inRandomDataMode = true;
            this.Series.BeginUpdate();

            for (int i = 0; i < this.Series.Count; i++)
            {
                ChartSeries series = this.Series[i];

                if (series.Name.IndexOf(c_internalSeriesName) != -1)
                {
                    series.Type = this.designTimeSeriesType;
                }
            }
            this.Series.EndUpdate();
            m_inRandomDataMode = false;
        }

        /// <summary>
        /// Remove random series.
        /// </summary>
        private void ResetRandomData()
        {
            m_inRandomDataMode = true;
            ChartSeries[] seriesToRemove = new ChartSeries[c_randomSeriesCount];
            int j = 0;

            for (int i = 0; i < this.Series.Count; i++)
            {
                ChartSeries series = this.Series[i];

                if (series.Name.IndexOf(c_internalSeriesName) != -1)
                {
                    seriesToRemove[j++] = series;
                }

                if (j == c_randomSeriesCount)
                {
                    break;
                }
            }

            foreach (ChartSeries series in seriesToRemove)
            {
                if (Series.IndexOf(series) != -1)
                {
                    this.Series.Remove(series);
                }
            }

            m_inRandomDataMode = false;
        }

        /// <summary>
        /// Checks the random data.
        /// </summary>
        private void CheckRandomData()
        {
            bool isRandomDataPresent = false;
            bool isUnRandomDataPresent = false;

            foreach (ChartSeries series in this.Series)
            {
                if (ChartControl.IsRandomSeries(series))
                {
                    isRandomDataPresent = true;
                }
                else
                {
                    isUnRandomDataPresent = true;
                }
            }

            if (isRandomDataPresent && isUnRandomDataPresent)
            {
                this.ResetRandomData();
            }
        }
        #endregion

        /// <summary>
        /// This member overrides <see cref="Control.OnDoubleClick"/>.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnDoubleClick(EventArgs e)
        {
            Point mousePt = this.PointToClient(Control.MousePosition);
            ChartRegion rgn = m_chartRegions.HitTest(mousePt);
            this.OnChartRegionDoubleClick(new ChartRegionMouseEventArgs(rgn, mousePt, m_lastMouseButton));

            if (m_lastMouseButton == MouseButtons.Left)
            {
                if (m_allowUserEditStyles && rgn.SeriesIndex >= 0)
                {
                    this.DisplayUserEditStylesDialog(rgn.SeriesIndex);
                }
            }

            base.OnDoubleClick(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnClick"/>.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);

            if (this.CanSelect)
            {
                this.Select();
            }
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnMouseDown"/>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (m_idleTime)
            {
                m_lastMouseButton = e.Button;

                if (m_toolTip.Visible)
                {
                    m_toolTip.HidePopup(PopupCloseType.Canceled);
                }

                if (e.Button == MouseButtons.Left)
                {
                    clickPoint = new Point(e.X, e.Y);

                    m_interactiveCursorDown = null;
                    m_currectMouseAction = InternalMouseAction.CursorMoving;

                    foreach (ChartInteractiveCursor interactiveCursor in m_chartArea.InteractiveCursors)
                    {
                        Point areaPoint = m_chartArea.CorrectionTo(clickPoint);

                        if (interactiveCursor.IsXLocation(areaPoint))
                        {
                            this.SetCursor(c_verticalCursorCursor);
                            m_interactiveCursorDown = interactiveCursor;
                            break;
                        }
                        else if (interactiveCursor.IsYLocation(areaPoint))
                        {
                            this.SetCursor(c_horizontalCursorCursor);
                            m_interactiveCursorDown = interactiveCursor;
                            break;
                        }
                    }

                    if (this.ChartArea.RenderBounds.Contains(e.Location)
                        && this.ChartArea.RequireAxes)
                    {
                        if (this.IsPanningEnabled)
                        {
                            m_currectMouseAction = InternalMouseAction.Panning;
                            this.SetCursor(c_panningCloseCursor);
                        }
                        else if (m_enableXZooming || m_enableYZooming)
                        {
                            m_currectMouseAction = InternalMouseAction.Zooming;
                        }
                    }
                }

                if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle
                    && this.ShowContextMenu == false)
                {
                    clickPoint = new Point(e.X, e.Y);
                }

                ChartRegion rgn = m_chartRegions.HitTest(new Point(e.X, e.Y));

                this.OnChartRegionMouseDown(new ChartRegionMouseEventArgs(rgn, e.Location, e.Button));

                if (e.Button == MouseButtons.Right && this.ShowContextMenu == true)
                {
                    if (m_displaySeriesContextMenu && rgn != null && rgn.SeriesIndex != -1)
                    {
                        this.SeriesContextMenu.Show(this, e.Location, this.Series[rgn.SeriesIndex]);
                    }
                    else if (m_displayChartContextMenu)
                    {
                        this.ChartContextMenu.Show(this, e.Location, this);
                    }
                }
            }

            base.OnMouseDown(e);
            if (m_idleTime)
            {
                ChartRegion rgnClick = m_chartRegions.HitTest(new Point(e.X, e.Y));
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right || e.Button == MouseButtons.Middle)
                {
                    this.OnChartRegionClick(new ChartRegionMouseEventArgs(rgnClick, e.Location, e.Button));
                }
            }
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnMouseMove"/>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {            
            if (m_idleTime)
            {
                bool refreshArea = false;
                bool refreshControl = false;

                #region Enable tooltip timer and hide tooltip
                if (m_oldMouseArgs.Location != e.Location)
                {
                    if (m_toolTip.Visible)
                    {
                        m_toolTip.HidePopup();
                    }

                    m_toolTipTimer.Enabled = false;
                    m_toolTipTimer.Enabled = !IsMouseDown && showToolTips && true;
                }
                #endregion

                #region Mouse rotation
                if ((m_enableMouseRotation && (e.Button == MouseButtons.Right) && (!DisplayChartContextMenu))
                    || ((e.Button == MouseButtons.Middle)))
                {
                    Rotation += m_oldMouseArgs.X - e.X;
                    Tilt -= m_oldMouseArgs.Y - e.Y;
                    refreshArea = true;
                }
                #endregion

                if (e.Button == MouseButtons.Left)
                {
                    mouseDownPosition = e.Location;
                }
                else
                {
                    mouseDownPosition = Point.Empty;
                }

                #region ChartRegions Events
                if (m_calcRegions)
                {
                    ChartRegion crgn = m_chartRegions.HitTest(e.Location);

                    if (m_activeRegion != crgn)
                    {
                        if (m_activeRegion != null)
                        {
                            this.OnChartRegionMouseLeave(new ChartRegionMouseEventArgs(m_activeRegion, e.Location));
                        }

                        m_activeRegion = crgn;                        
                        refreshArea = m_autoHighlight || m_seriesHighlight;

                        if (m_activeRegion != null)
                        {
                            this.OnChartRegionMouseEnter(new ChartRegionMouseEventArgs(m_activeRegion, e.Location));
                        }
                    }

                    if (m_activeRegion != null)
                    {
                        ChartRegionMouseEventArgs crgnMouseEventArgs = new ChartRegionMouseEventArgs(m_activeRegion, e.Location);
                        this.OnChartRegionMouseMove(crgnMouseEventArgs);
                        this.OnChartRegionMouseHover(crgnMouseEventArgs);
                    }
                }
                #endregion

                if (m_currectMouseAction == InternalMouseAction.Panning)
                {
                    #region Panning
                    Point pt1 = this.ChartArea.CorrectionTo(m_oldMouseArgs.Location);
                    Point pt2 = this.ChartArea.CorrectionTo(e.Location);

                    foreach (ChartAxis axis in this.Axes)
                    {
                        if ((axis.ZoomActions & ChartZoomingAction.Panning) == ChartZoomingAction.Panning
                            && axis.ZoomFactor < 1)
                        {
                            double x1 = axis.GetRealValue(
                                axis.Orientation == ChartOrientation.Vertical ? pt1.Y : pt1.X);
                            double x2 = axis.GetRealValue(
                                axis.Orientation == ChartOrientation.Vertical ? pt2.Y : pt2.X);

                            if (axis.Orientation == ChartOrientation.Horizontal)
                            {
                                axis.ZoomPosition -= (x2 - x1) / axis.Range.Delta;
                            }
                            else
                            {
                                axis.ZoomPosition += (x2 - x1) / axis.Range.Delta;
                            }

                            this.SetCursor(c_panningCloseCursor);
                            refreshArea = true;
                        }
                    }
                    #endregion
                }
                else if (m_currectMouseAction == InternalMouseAction.Zooming)
                {
                    #region Zooming
                    this.SetCursor(Cursors.Cross);
                    refreshArea = true;
                    #endregion
                }
                else if (m_currectMouseAction == InternalMouseAction.CursorMoving)
                {
                    #region CursorMoving
                    if ((this.Cursor == c_verticalCursorCursor) && (m_interactiveCursorDown !=null))
                    {
                        if (m_interactiveCursorDown.MoveToChartArea == true)
                        {                            
                            if (double.IsNaN(oldPointX))
                            this.oldPointX = this.ChartArea.GetValueByPoint(new Point((int)Math.Ceiling(this.m_interactiveCursorDown.Location.X), (int)this.m_interactiveCursorDown.Location.Y)).X;
                            else
                            this.newPointX = this.ChartArea.GetValueByPoint(e.Location).X;                                                                                                                                  
                            if (oldPointX < newPointX)
                            {                            
                                cPointX= oldPointX + m_interactiveCursorDown.XInterval;
                                if (newPointX > cPointX)
                                    oldPointX = cPointX;                                                              
                                ChartArea.CursorLocation = e.Location;
                                                                               
                            }
                            else
                            {
                                cPointX = Math.Abs(oldPointX - m_interactiveCursorDown.XInterval);
                                if (newPointX < cPointX)
                                    oldPointX = cPointX;                            
                                ChartArea.CursorLocation = e.Location;
                              
                            }
                                                                         
                            m_interactiveCursorDown.LineLocation = ChartArea.CursorLocation;
                            ChartArea.CursorReDraw = true;
                            m_interactiveCursorDown.LineRedraw = true;
                            refreshControl = true;
                          
                        }
                        else
                        {
                            m_interactiveCursorDown.HorizontalMove(ChartArea.GetValueByPoint(e.Location).X);
                            refreshControl = true;
                        }

                    }
                    else if ((this.Cursor == c_horizontalCursorCursor) && (m_interactiveCursorDown!=null))
                    {
                        if (m_interactiveCursorDown.MoveToChartArea == true)
                        {

                            if (double.IsNaN(this.oldPointY))
                                this.oldPointY = this.ChartArea.GetValueByPoint(new Point((int)this.m_interactiveCursorDown.Location.X, (int)Math.Ceiling(this.m_interactiveCursorDown.Location.Y))).YValues[0];
                            else
                                this.newPointY = this.ChartArea.GetValueByPoint(e.Location).YValues[0];
                            if (this.oldPointY < this.newPointY)
                            {
                                this.cPointY = this.oldPointY + this.m_interactiveCursorDown.YInterval;
                                if (this.newPointY > this.cPointY)
                                    this.oldPointY = this.cPointY;
                                this.ChartArea.CursorLocation = e.Location;
                            }
                            else
                            {
                                this.cPointY = Math.Abs(this.oldPointY - this.m_interactiveCursorDown.YInterval);
                                if (this.newPointY < this.cPointY)
                                    this.oldPointY = this.cPointY;
                                this.ChartArea.CursorLocation = e.Location;
                            }
                            ChartArea.CursorReDraw = true;
                            m_interactiveCursorDown.LineRedraw = true;
                            refreshControl = true;
                        }
                        else
                        {
                            m_interactiveCursorDown.VerticalMove(ChartArea.GetValueByPoint(e.Location).YValues[0]);
                        }
                        refreshControl = true;
                    }
                    #endregion
                }
                else
                {
                    this.GetBackCursor();

                    #region None
                    foreach (ChartInteractiveCursor interactiveCursor in m_chartArea.InteractiveCursors)
                    {
                        Point areaPoint = m_chartArea.CorrectionTo(e.Location);

                        if (interactiveCursor.IsXLocation(areaPoint))
                        {
                            this.SetCursor(c_verticalCursorCursor);
                            break;
                        }
                        else if (interactiveCursor.IsYLocation(areaPoint))
                        {
                            this.SetCursor(c_horizontalCursorCursor);
                            break;
                        }
                        else
                        {
                            this.GetBackCursor();
                        }
                    }

                    if (m_baseCursor == null)
                    {
                        if (this.IsPanningEnabled && this.ChartArea.RenderBounds.Contains(e.Location))
                        {
                            this.SetCursor(c_panningOpenCursor);
                        }
                        else
                        {
                            this.GetBackCursor();
                        }
                    }
                    #endregion
                }

                if (refreshControl)
                {
                    this.Redraw(true);
                }
                else if (refreshArea)
                {
                    this.RefreshArea();
                }
            }

            m_oldMouseArgs = new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta);

            base.OnMouseMove(e);
        }

        /// <summary>
        /// This method returns the real point value for specific X and Y value.
        /// </summary>
        /// <param name="xVal">X value</param>
        /// <param name="yVal">Y value</param>
        private Point GetRealPoint(double xVal, double yVal)
        {
            ChartPoint cPoint = new ChartPoint(xVal,yVal);
            Point point = ChartArea.GetPointByValue(cPoint);
            return point;
        }
        /// <summary>
        /// This member overrides <see cref="Control.OnMouseUp"/>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.m_interactiveCursorDown != null)
            {
                if (this.Cursor == ChartControl.c_verticalCursorCursor && !double.IsNaN(this.oldPointX) && !double.IsNaN(this.newPointX))
                {
                    if (this.oldPointX < this.newPointX)
                    {
                        double xVal = this.oldPointX + this.m_interactiveCursorDown.XInterval;
                        double num = (this.oldPointX + xVal) / 2.0;
                        this.ChartArea.CursorLocation = num < this.ChartArea.GetValueByPoint(e.Location).X ? this.GetRealPoint(xVal, this.ChartArea.GetValueByPoint(e.Location).YValues[0]) : this.GetRealPoint(this.oldPointX, this.ChartArea.GetValueByPoint(e.Location).YValues[0]);
                        this.oldPointX = num < this.ChartArea.GetValueByPoint(e.Location).X ? xVal : this.oldPointX;
                    }
                    else
                    {
                        double xVal = Math.Abs(this.oldPointX - this.m_interactiveCursorDown.XInterval);
                        double num = (this.oldPointX + xVal) / 2.0;
                        this.ChartArea.CursorLocation = num > this.ChartArea.GetValueByPoint(e.Location).X ? this.GetRealPoint(xVal, this.ChartArea.GetValueByPoint(e.Location).YValues[0]) : this.GetRealPoint(this.oldPointX, this.ChartArea.GetValueByPoint(e.Location).YValues[0]);
                        this.oldPointX = num > this.ChartArea.GetValueByPoint(e.Location).X ? xVal : this.oldPointX;
                    }
                }
                else if (this.Cursor == ChartControl.c_horizontalCursorCursor && !double.IsNaN(this.oldPointY) && !double.IsNaN(this.newPointY))
                {
                    if (this.oldPointY < this.newPointY)
                    {
                        double yVal = this.oldPointY + this.m_interactiveCursorDown.YInterval;
                        double num = (this.oldPointY + yVal) / 2.0;
                        this.ChartArea.CursorLocation = num < this.ChartArea.GetValueByPoint(e.Location).YValues[0] ? this.GetRealPoint(this.ChartArea.GetValueByPoint(e.Location).X, yVal) : this.GetRealPoint(this.ChartArea.GetValueByPoint(e.Location).X, this.oldPointY);
                        this.oldPointY = num < this.ChartArea.GetValueByPoint(e.Location).YValues[0] ? yVal : this.oldPointY;
                    }
                    else
                    {
                        double yVal = Math.Abs(this.oldPointY - this.m_interactiveCursorDown.YInterval);
                        double num = (this.oldPointY + yVal) / 2.0;
                        this.ChartArea.CursorLocation = num > this.ChartArea.GetValueByPoint(e.Location).YValues[0] ? this.GetRealPoint(this.ChartArea.GetValueByPoint(e.Location).X, yVal) : this.GetRealPoint(this.ChartArea.GetValueByPoint(e.Location).X, this.oldPointY);
                        this.oldPointY = num > this.ChartArea.GetValueByPoint(e.Location).YValues[0] ? yVal : this.oldPointY;
                    }
                }
                else
                    this.ChartArea.CursorLocation = e.Location;
                this.Redraw(true);
            }
                                
            if (m_idleTime)
            {
                bool needRedraw = false;

                try
                {
                    m_activeRegion = m_chartRegions.HitTest(e.Location);
                    if (m_activeRegion != null)
                    {
                        this.OnChartRegionMouseUp(new ChartRegionMouseEventArgs(m_activeRegion, e.Location, e.Button));
                    }

                    if (m_currectMouseAction == InternalMouseAction.Zooming && e.Location != clickPoint)
                    {
                        if (m_enableXZooming && (e.X != clickPoint.X))
                        {
                            this.ZoomNeededAxes(e.Location, clickPoint, ChartOrientation.Horizontal);
                            needRedraw = true;
                        }

                        if (m_enableYZooming && (e.Y != clickPoint.Y))
                        {
                            this.ZoomNeededAxes(e.Location, clickPoint, ChartOrientation.Vertical);
                            needRedraw = true;
                        }
                    }
                }
                finally
                {
                    m_interactiveCursorDown = null;
                    clickPoint = Point.Empty;
                    mouseDownPosition = Point.Empty;
                    m_currectMouseAction = InternalMouseAction.None;
                    this.GetBackCursor();

                    if (needRedraw)
                    {
                        this.RefreshArea();
                    }
                }
            }

            base.OnMouseUp(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnMouseLeave"/>.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            m_toolTipTimer.Enabled = false;

            if (m_calcRegions)
            {
                if (m_activeRegion != null)
                {
                    this.OnChartRegionMouseLeave(new ChartRegionMouseEventArgs(m_activeRegion, m_oldMouseArgs.Location));
                }

                m_activeRegion = null;
            }

            base.OnMouseLeave(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnMouseWheel"/>.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            this.GetVScrollBar(this.PrimaryYAxis).Value -= e.Delta;

            base.OnMouseWheel(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnTextChanged"/>.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            this.Redraw(true);
            base.OnTextChanged(e);            
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnFontChanged"/>.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnFontChanged(EventArgs e)
        {
            ChartStyleInfo baseStyle = this.Model.GetStylesMap().Lookup("Standard");

            baseStyle.Font.FontFamilyTemplate = new FontFamily(this.Font.FontFamily.Name);
            baseStyle.Font.Size = this.Font.Size;
            baseStyle.Font.FontStyle = this.Font.Style;

            this.Series.ResetCache();

            this.Redraw(true);
            base.OnFontChanged(e);
        }

        /// <summary>
        /// This member overrides <see cref="Control.OnBackColorChanged"/>.
        /// </summary>
        /// <param name="e">An EventArgs that contains the event data.</param>
        protected override void OnBackColorChanged(EventArgs e)
        {
            switch (m_backInterior.Style)
            {
                case BrushStyle.None:
                    break;

                case BrushStyle.Solid:
                    BackInterior = new BrushInfo(this.BackColor);
                    break;

                case BrushStyle.Gradient:
                    {
                        Color[] colors = (Color[])m_backInterior.GradientColors.ToArray(typeof(Color));
                        colors[0] = this.BackColor;
                        BackInterior = new BrushInfo(m_backInterior.GradientStyle, colors);
                    }

                    break;

                case BrushStyle.Pattern:
                    {
                        Color[] colors = (Color[])m_backInterior.GradientColors.ToArray(typeof(Color));
                        colors[0] = this.BackColor;
                        BackInterior = new BrushInfo(m_backInterior.PatternStyle, colors);
                    }

                    break;
            }

            base.OnBackColorChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.ForeColorChanged"></see> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            ChartStyleInfo baseStyle = this.Model.GetStylesMap().Lookup("Standard");

            baseStyle.TextColor = this.ForeColor;

            this.Series.ResetCache();
            this.Legend.Refresh();
            this.Redraw(true);
            base.OnForeColorChanged(e);
        }

        /// <summary>
        /// Overrides the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            m_idleTime = false;

            if (Series.Count == 0 && m_addRandomData)
                this.AddRandomData();

            this.PrepareAxesSeriesAndChart();

            if (m_needRecalculateSizes)
            {
                this.RecalculateSizes(e.Graphics);
                m_needRecalculateSizes = false;
            }

            this.SuspendLayout();

            bool areWeSelectingZoomRangeIn2D = !(this.RealMode3D && this.Series3D)
                && (m_currectMouseAction == InternalMouseAction.Zooming);

            if ((m_buffer == null) || (m_needRedraw && !areWeSelectingZoomRangeIn2D))
            {
                m_buffer = new Bitmap(Width, Height);

                using (Graphics g = Graphics.FromImage(m_buffer))
                {
                    _Paint(g, ClientRectangle);
                }

                m_needRedraw = false;                
            }

            ChartAreaImageEventArgs imageArgs = new ChartAreaImageEventArgs(m_buffer);

            this.OnChartAreaImage(imageArgs);
            
            if (imageArgs.Handled)
            {
                if (imageArgs.BufferImage != null)
                    m_buffer = imageArgs.BufferImage;
                e.Graphics.DrawImage(m_buffer, imageArgs.Location.X, imageArgs.Location.Y, imageArgs.Size.Width, imageArgs.Size.Height);
            }
            else
            {
                e.Graphics.DrawImage(m_buffer, Point.Empty);
            }            

            if (areWeSelectingZoomRangeIn2D)
            {
                // higlight the zooming range
                m_chartArea.DrawZoomingRange(e.Graphics);
            }

            this.ResumeLayout();
            base.OnPaint(e);

            m_idleTime = true;
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="pevent">A <see cref="T:System.Windows.Forms.PaintEventArgs"></see> that contains information about the control to paint.</param>
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            m_defaultBackgroundPainting = true;
            base.OnPaintBackground(pevent);
            m_defaultBackgroundPainting = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
        /// </summary>
        /// <param name="e">Argument.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Redraw(true);
        }

        /// <summary>
        /// Raises the	<see cref="Control.ProcessCmdKey"/> event.
        /// </summary>
        /// <param name="msg">Windows message.</param>
        /// <param name="keyData">Key code.</param>
        /// <returns>
        /// true if the character was processed by the control; otherwise, false.
        /// </returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
#if DEBUG
			Debug.WriteLine(keyData.ToString(), "ProcessCmdKey");
#endif

            if (msg.HWnd != this.Handle)
                return false;

            if (keyData == m_zoomOut)
            {
                if (m_enableXZooming) this.MulZoomFactor(2, ChartOrientation.Horizontal);
                if (m_enableYZooming) this.MulZoomFactor(2, ChartOrientation.Vertical);
            }
            else if (keyData == m_zoomIn)
            {
                if (m_enableXZooming) this.MulZoomFactor(0.5, ChartOrientation.Horizontal);
                if (m_enableYZooming) this.MulZoomFactor(0.5, ChartOrientation.Vertical);
            }
            else if (keyData == m_zoomRight)
            {
                this.ShiftZoomPosition(0.1, ChartOrientation.Horizontal);
            }
            else if (keyData == m_zoomUp)
            {
                this.ShiftZoomPosition(0.1, ChartOrientation.Vertical);
            }
            else if (keyData == m_zoomLeft)
            {
                this.ShiftZoomPosition(-0.1, ChartOrientation.Horizontal);
            }
            else if (keyData == m_zoomDown)
            {
                this.ShiftZoomPosition(-0.1, ChartOrientation.Vertical);
            }
            else if (keyData == m_zoomCancel)
            {
                if (this.m_enableXZooming) this.ZoomFactorX = 1;
                if (this.m_enableYZooming) this.ZoomFactorY = 1;
            }
            else
            {
                return base.ProcessCmdKey(ref msg, keyData);
            }

            return true;
        }

        /// <summary>
        /// Called when [border appearance changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnBorderAppearanceChanged(object sender, EventArgs e)
        {
            this.Redraw(true);
        }

        /// <summary>
        /// Checking series on the compatibility.
        /// </summary>
        private void CheckSeriesCompatibility()
        {
            if (!m_model.CheckSeriesCompatibility(m_chartArea, m_invertedSeriesIsCompatible))
            {
                this.RaiseSeriesIncompatible(null);
            }
        }

        /// <summary>
        /// This method is called before the beginning of transformations 
        /// in the Graphics.
        /// </summary>
        /// <param name="g"></param>
        /// <returns>Return GraphicsContainer object.</returns>
        internal static GraphicsContainer BeginTransform(Graphics g)
        {
            SmoothingMode mode = g.SmoothingMode;
            TextRenderingHint textRH = g.TextRenderingHint;
            GraphicsContainer cont = g.BeginContainer();
            g.SmoothingMode = mode;
            g.TextRenderingHint = textRH;
            return cont;
        }

        /// <summary>
        /// This method is called after the ending of transformations 
        /// in the Graphics.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="cont"></param>
        internal static void EndTransform(Graphics g, GraphicsContainer cont)
        {
            g.EndContainer(cont);
        }

        /// <summary>
        /// Dispose this control and its children.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        [DocumentationExclude()]
        protected override void Dispose(bool disposing)
        {            
            if (disposing)
            {
                disposeStarted = true;

                if (m_toolTipTimer != null)
                {
                    m_toolTipTimer.Dispose();
                    m_toolTipTimer = null;
                }

                if (m_toolTip != null)
                {
                    m_toolTip.Dispose();
                    m_toolTip = null;
                }

                if (m_canvas != null)
                {
                    m_canvas.Dispose();
                    m_canvas = null;
                }
                if (m_borderAppearance!=null)
                {
                    m_borderAppearance = null;
                }
                if (m_printDocument != null)
                {
                    m_printDocument.Dispose();
                    m_printDocument = null;
                }

                if (m_fancyToolTipController != null)
                {
                    m_fancyToolTipController.Dispose();
                    m_fancyToolTipController = null;
                }

                if (scrollBarH != null)
                {
                    scrollBarH.Clear();
                    scrollBarH = null;
                }

                if (scrollBarV != null)
                {
                    scrollBarV.Clear();
                    scrollBarV = null;
                }

                if (m_legends != null)
                {
                    for (int i = 0; i < m_legends.Count; i++)
                    {
                        m_legends[i].Dispose();
                        m_legends[i] = null;
                    }

                    m_legends = null;
                }

                if (m_defaultLegend != null)
                {
                    m_defaultLegend.Dispose();
                    m_defaultLegend = null;
                }

                if (m_defaultTitle != null)
                {
                    m_defaultTitle.Dispose();
                    m_defaultTitle = null;
                }
                if (m_titles != null)
                {
                     
                    m_titles = null;
                }
                if (m_toolBarDockingManager != null)
                {
                    m_toolBarDockingManager.Dispose();
                    m_toolBarDockingManager = null;
                }
                if (m_dockingManager != null)
                {
                    m_dockingManager.Dispose();
                    m_dockingManager = null;
                }

                if (m_titleDockingManager != null)
                {
                    m_titleDockingManager.Dispose();
                    m_titleDockingManager = null;
                }

                if (m_toolbar != null)
                {
                    m_toolbar.Dispose();               
                    m_toolbar = null;
                }

                if (m_chartArea != null)
                {
                    foreach (ChartAxis ax in m_chartArea.Axes)
                    {
                        ChartScrollBar scrollBar = ax.ScrollBar as ChartScrollBar;

                        if (scrollBar != null)
                        {
                            scrollBar.Dispose();
                            ax.ScrollBar = null;
                        }
                    }

                    m_chartArea.Axes.Changed -= new ChartListChangeHandler(OnAxesChanged);
                    m_chartArea.Dispose();
                    m_chartArea = null;
                }

                if (m_model != null)
                {
                    this.SetModel(null);
                }

                if (m_chartContextMenu != null)
                {
                    m_chartContextMenu.Dispose();
                    m_chartContextMenu = null;
                }

                if (m_seriesContextMenu != null)
                {
                    m_seriesContextMenu.Dispose();
                    m_seriesContextMenu = null;
                }

                if (m_chartRegions != null)
                {
                    m_chartRegions.Clear();
                    m_chartRegions = null;
                }
                if (m_styleDialogOptions != null)
                {
                     m_styleDialogOptions = null;
                }
                if (m_oldMouseArgs != null)
                {
                    m_oldMouseArgs = null;
                }
                m_zoomBarInfo = null;

                ChartAreaPaint = null;
                ChartFormatAxisLabel = null;
                ChartRegionClick = null;
                ChartRegionDoubleClick = null;
                ChartRegionMouseDown = null;
                ChartRegionMouseEnter = null;
                ChartRegionMouseHover = null;
                ChartRegionMouseLeave = null;
                ChartRegionMouseMove = null;
                ChartRegionMouseUp = null;
                VisibleRangeChanged = null;
                LayoutCompleted = null;
                SeriesIncompatible = null;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Called when [axes changed].
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="args">The args.</param>
        private void OnAxesChanged(ChartBaseList list, ChartListChangeArgs args)
        {
            if (args.NewItems != null)
            {
                foreach (ChartAxis axis in args.NewItems)
                {
                    axis.Zooming += new ChartAxisZoomingEventHandler(OnAxisZooming);
                    axis.Zoomed += new EventHandler(OnAxisZoomed);
                }
            }

            if (args.OldItems != null)
            {
                foreach (ChartAxis axis in args.OldItems)
                {
                    axis.Zooming -= new ChartAxisZoomingEventHandler(OnAxisZooming);
                    axis.Zoomed -= new EventHandler(OnAxisZoomed);
                }
            }
        }

        /// <summary>
        /// Called when axis is zooming.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void OnAxisZooming(object sender, ChartAxisZoomingArgs args)
        {
            if (this.VisibleRangeChanging != null)
            {
                this.VisibleRangeChanging(this, args);
            }
        }
        
        /// <summary>
        /// Called when axis is zoomed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxisZoomed(object sender, EventArgs args)
        {
            this.ZoomingChanged(sender as ChartAxis);

            if (this.VisibleRangeChanged != null)
            {
                this.VisibleRangeChanged(this, args);
            }
        }

        /// <summary>
        /// Event handler to event AppearanceChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSeriesAppearanceChanged(object sender, EventArgs e)
        {
            Redraw(true);
        }

        /// <summary>
        /// Unwires the old series model and sets the new.
        /// </summary>
        /// <param name="model">A new series model.</param>
        private void SetModel(ChartModel model)
        {
            this.UnwireSeriesCollection();

            if (m_model != null)
            {
                m_model.SetChart(null);
            }

            m_model = model;

            if (m_model != null)
            {
                m_model.SetChart(this);
            }

            this.WireSeriesCollection();
        }

        /// <summary>
        /// Unwires the current series collection form the chart.
        /// </summary>
        private void UnwireSeriesCollection()
        {
            if (m_model != null)
            {
                foreach (ChartSeries series in m_model.Series)
                {
                    series.AppearanceChanged -= new EventHandler(OnSeriesAppearanceChanged);
                }

                m_model.Series.Changed -= new ChartSeriesCollectionChangedEventHandler(this.SeriesChanged);
            }
        }

        /// <summary>
        /// Wires the current series collection to chart.
        /// </summary>
        private void WireSeriesCollection()
        {
            if (m_model != null)
            {
                foreach (ChartSeries series in m_model.Series)
                {
                    series.AppearanceChanged += new EventHandler(OnSeriesAppearanceChanged);
                }

                m_model.Series.Changed += new ChartSeriesCollectionChangedEventHandler(this.SeriesChanged);
            }
        }

        /// <summary>
        /// Is called when the size of contols in the docing manager was changed.
        /// </summary>
        /// <param name="sender">A sender of event.</param>
        /// <param name="e">Argument.</param>
        private void DockingManager_SizeChanged(object sender, EventArgs e)
        {
            Redraw(true);
        }

        /// <summary>
        /// Invalidates the chart and area.
        /// </summary>
        private void RefreshArea()
        {
            m_needRedraw = true;
            this.Invalidate(true);
        }

        /// <summary>
        /// Invalidates the legend.
        /// </summary>
        private void RefreshLegend()
        {
            foreach (ChartLegend legend in m_legends)
            {
                legend.Invalidate();
            }
        }

        /// <summary>
        /// Is called after a click on the legend. Shows the series context menu.
        /// </summary>
        /// <param name="sender">A sender of event.</param>
        /// <param name="e">Argument.</param>
        private void LegendMouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Right) == MouseButtons.Right)
            {
                if (ShowContextMenuInLegend)
                {
                    ChartSeriesLegendItem csli = Legend.GetItemBy(new Point(e.X, e.Y)) as ChartSeriesLegendItem;

                    if (csli != null)
                    {
                        SeriesContextMenu.Show(sender as Control, new Point(e.X, e.Y), csli.Series);
                    }
                }
            }
        }

        /// <summary>
        /// Is called when is a need to show the tooltip.
        /// </summary>
        /// <param name="sender">A sender of event.</param>
        /// <param name="e">Argument.</param>
        private void ToolTipTick(object sender, EventArgs e)
        {
            ChartRegion rgn = m_chartRegions.HitTest(this.PointToClient(Control.MousePosition));

            if (this.ShowToolTips && rgn != null
                && MouseButtons == MouseButtons.None)
            {
                Point pt = Control.MousePosition;
                pt.Offset(10, 10);

                // Get the regions tooltip text.
                String szText = rgn.ToolTip;

                if (szText != string.Empty)
                {
                    this.m_toolTip.Text = szText;
                    m_toolTip.ShowPopup(pt);
                }
                else
                {
                    if (m_toolTip.Visible)
                    {
                        m_toolTip.HidePopup(PopupCloseType.Canceled);
                    }
                }
            }
            else
            {
                // Hide the tooltip control if it is visible.
                if (m_toolTip.Visible)
                {
                    m_toolTip.HidePopup(PopupCloseType.Canceled);
                }
            }

            m_toolTipTimer.Enabled = false;
        }

        /// <summary>
        /// Saves a previous mouse cursor and sets a new.
        /// </summary>
        /// <param name="cursor">A new cursor.</param>
        private void SetCursor(Cursor cursor)
        {
            if (m_baseCursor == null)
            {
                m_baseCursor = base.Cursor;
            }

            base.Cursor = cursor;
        }

        /// <summary>
        /// Gets back a old mouse cursor.
        /// </summary>
        private void GetBackCursor()
        {
            if (m_baseCursor != null)
            {
                base.Cursor = m_baseCursor;
                m_baseCursor = null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether it's design time.
        /// </summary>
        /// <value><c>true</c> if it's design time; otherwise, <c>false</c>.</value>
        bool IChartAreaHost.IsDesignTime
        {
            get
            {
                return (this.Site != null) && (this.Site.DesignMode);
            }
        }
        #endregion
    }
}