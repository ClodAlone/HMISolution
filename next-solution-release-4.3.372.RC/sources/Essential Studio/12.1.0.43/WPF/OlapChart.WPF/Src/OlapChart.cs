#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Chart.Olap
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Microsoft.Win32;
    using Syncfusion.Olap.Manager;
    using Syncfusion.Olap.Reports;
    using Syncfusion.Windows.Shared;
    using Syncfusion.Windows.Tools.Controls;
    using Syncfusion.Windows.Shared.Olap;
    using System.ComponentModel;
    using System.Collections.Generic;
    using Syncfusion.Licensing;
    using System.Windows.Data;
    using Syncfusion.Olap.Engine;

    /// <summary>
    /// Represents OlapChart control.
    /// </summary>
    [TemplatePart(Name = "PART_OlapArea", Type = typeof(OlapArea))]
    [SkinType(SkinVisualStyle = Skin.Transparent,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Transparent.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Blue,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2010Blue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Black,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2010Black.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2010Silver,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2010Silver.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Blue,
  Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2007Blue.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Black,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2007Black.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2007Silver,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2007Silver.xaml")]
    [SkinType(SkinVisualStyle = Skin.Office2003,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2003.xaml")]
    [SkinType(SkinVisualStyle = Skin.Blend,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Blend.xaml")]
    [SkinType(SkinVisualStyle = Skin.Metro,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Metro.xaml")]
    [SkinType(SkinVisualStyle = Skin.Default,
    Type = typeof(OlapChart), XamlResource = "/Syncfusion.OlapChart.WPF;component/Themes/generic.xaml")]
    public class OlapChart : Control
    {
        #region Members
        private const string c_imageFilesFilter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg,*.jpeg)|*.jpg;*.jpeg|GIF(*.gif)|*.gif|TIFF(*.tiff)|*.tiff|PNG(*.png)|*.png|WDP(*.wdp)|*.wdp|Xps file (*.xps)|*.xps|All files (*.*)|*.*";
        private const string c_Key = @"/Syncfusion.OlapChart.WPF;component/Themes/generic.xaml";
        private const string Defaultmarker = "#BI_CHART_PLACEHOLDER#";

        #endregion

        #region DependencyProperties

        /// <summary>
        /// Identifies the ChartType dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartTypeProperty = OlapArea.ChartTypeProperty.AddOwner(typeof(OlapChart));

        /// <summary>
        /// Identifies the CornerRadius dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = System.Windows.Controls.Border.CornerRadiusProperty.AddOwner(typeof(OlapChart));

        /// <summary>
        /// Identifies the GridBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty GridBackgroundProperty = ChartArea.GridBackgroundProperty.AddOwner(typeof(OlapChart));

        /// <summary>
        /// Identifies the GridLineStroke dependency property.
        /// </summary>
        public static readonly DependencyProperty GridLineStrokeProperty =
            DependencyProperty.Register("GridLineStroke", typeof(Pen), typeof(OlapChart), new FrameworkPropertyMetadata(null, OnGridLineStrokeChanged));

        /// <summary>
        /// Identifies the Legend dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendProperty = ChartArea.LegendProperty.AddOwner(typeof(OlapChart));
        private OlapArea m_area;

        /// <summary>
        /// Identifies the OlapDataManager dependency property.
        /// </summary>
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(IOlapDataManager), typeof(OlapChart),
                                        new UIPropertyMetadata(null, OnOlapDataManagerChanged));

        private static void OnOlapDataManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapChart chart = d as OlapChart;
            if (chart != null && !DesignerProperties.GetIsInDesignMode(chart) && chart.m_area != null)
            {
                chart.m_area.OlapDataManager = e.NewValue as OlapDataManager;
            }
        }



        public PivotEngine PivotEngine
        {
            get { return (PivotEngine)GetValue(pivotEngineProperty); }
            set { SetValue(pivotEngineProperty, value); }
        }

        // Using a DependencyProperty as the backing store for pivotEngine.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty pivotEngineProperty =
            DependencyProperty.Register("PivotEngine", typeof(PivotEngine), typeof(OlapChart), new UIPropertyMetadata(null, new PropertyChangedCallback(
                (dependencyObject, args) => 
                {
                    OlapChart chart = dependencyObject as OlapChart;
                    if (chart != null && !DesignerProperties.GetIsInDesignMode(chart) && chart.m_area != null)
                    {
                        chart.m_area.SetValue(OlapArea.PivotEnginePropertyKey, args.NewValue as PivotEngine);
                    }
                })));

        

        /// <summary>
        /// Using a DependencyProperty as the backing store for SeriesStrokeThickness.
        /// </summary>
        public static readonly DependencyProperty SeriesStrokeThicknessProperty =
            DependencyProperty.Register("SeriesStrokeThickness", typeof(double), typeof(OlapChart), new PropertyMetadata(1d, (d, args) =>
            {
                OlapChart olapChart = d as OlapChart;
                if (olapChart.m_area != null)
                {
                    foreach (ChartSeries series in olapChart.Series)
                    {
                        series.StrokeThickness = (double)args.NewValue;
                    }
                }
            }));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ColorEachSeries.
        /// </summary>
        public static readonly DependencyProperty ColorEachSeriesProperty =
            DependencyProperty.Register("ColorEachSeries", typeof(bool), typeof(OlapChart), new UIPropertyMetadata(false, (d, args) =>
            {
                OlapChart olapChart = d as OlapChart;
                if (olapChart.m_area != null)
                {
                    foreach (ChartSeries item in olapChart.Series)
                    {
                        item.ColorEach = (bool)args.NewValue;
                    }
                }
            }));

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableSeriesAnimation.
        /// </summary>
        public static readonly DependencyProperty EnableSeriesAnimationProperty =
            DependencyProperty.Register("EnableSeriesAnimation", typeof(bool), typeof(OlapChart), new UIPropertyMetadata(false, (d, args) =>
            {
                OlapChart olapChart = d as OlapChart;
                if (olapChart.m_area != null)
                {
                    foreach (ChartSeries item in olapChart.Series)
                    {
                        item.EnableAnimation = (bool)args.NewValue;
                    }
                }
            }));
        
        /// <summary>
        /// Using a DependencyProperty as the backing store for SereiesAnimateOption.
        /// </summary>
        public static readonly DependencyProperty SeriesAnimateOptionProperty =
            DependencyProperty.Register("SeriesAnimateOption", typeof(AnimationOptions), typeof(OlapChart), new UIPropertyMetadata(AnimationOptions.Top, (d, args) =>
            {
                OlapChart olapChart = d as OlapChart;
                if (olapChart.m_area != null)
                {
                    foreach (ChartSeries item in olapChart.Series)
                    {
                        item.AnimateOption = (AnimationOptions)Enum.Parse(typeof(AnimationOptions), args.NewValue.ToString());
                    }
                }
            }));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SeriesAnimationDuration.
        /// </summary>
        public static readonly DependencyProperty SeriesAnimationDurationProperty =
            DependencyProperty.Register("SeriesAnimationDuration", typeof(TimeSpan), typeof(OlapChart), new UIPropertyMetadata(new TimeSpan(0, 0, 2), (d, args) =>
            {
                OlapChart olapChart = d as OlapChart;
                if (olapChart.m_area != null)
                {
                    foreach (ChartSeries series in olapChart.Series)
                    {
                        series.AnimationDuration = (TimeSpan)args.NewValue;
                    }
                }
            }));

        /// <summary>
        /// Using a DependencyProperty as the backing store for EnableSeriesEffects.
        /// </summary>
        public static readonly DependencyProperty EnableSeriesEffectsProperty =
            DependencyProperty.Register("EnableSeriesEffects", typeof(bool), typeof(OlapChart), new UIPropertyMetadata(false, (d, args) =>
            {
                OlapChart olapChart = d as OlapChart;
                if (olapChart.m_area != null)
                {
                    foreach (ChartSeries series in olapChart.Series)
                    {
                        series.EnableEffects = (bool)args.NewValue;
                    }
                }
            }));

        /// <summary>
        /// Using a DependencyProperty as the backing store for SeriesAnimateOneByOne.
        /// </summary>
        public static readonly DependencyProperty SeriesAnimateOneByOneProperty =
            DependencyProperty.Register("SeriesAnimateOneByOne", typeof(bool), typeof(OlapChart), new UIPropertyMetadata(false, (d, args) =>
            {
                OlapChart olapChart = d as OlapChart;
                if (olapChart.m_area != null)
                {
                    foreach (ChartSeries series in olapChart.Series)
                    {
                        series.AnimateOneByOne = (bool)args.NewValue;
                    }
                }
            }));

        /// <summary>
        /// Identifies the Primary axis dependency property.
        /// </summary>
        public static readonly DependencyProperty PrimaryAxisProperty =
            DependencyProperty.Register("PrimaryAxis", typeof(OlapChartAxis), typeof(OlapChart), new FrameworkPropertyMetadata(null, null, CoercePrimaryAxis));

        /// <summary>
        /// Identifies the SecondaryAxis dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryAxisProperty =
            ChartArea.SecondaryAxisProperty.AddOwner(typeof(OlapChart));

        public static readonly DependencyProperty SeriesToolTipTemplateProperty =
            DependencyProperty.Register("SeriesToolTipTemplate", typeof(ControlTemplate), typeof(OlapChart), new UIPropertyMetadata(null));
        string symbolTemplate = string.Empty;

        // Using a DependencyProperty as the backing store for PrimaryAxisLabelVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryAxisLabelVisibilityProperty =
            DependencyProperty.Register("PrimaryAxisLabelVisibility", typeof(Visibility), typeof(OlapChart), new UIPropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty ExpanderStyleProperty =
            DependencyProperty.Register("ExpanderStyle", typeof(System.Windows.Style), typeof(OlapChart), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty CategoricalAxisProperty =
            DependencyProperty.Register("CategoricalAxis", typeof(CategoricalAxis), typeof(OlapChart), new UIPropertyMetadata(new CategoricalAxis()));

        public static readonly DependencyProperty SeriesAxisProperty =
            DependencyProperty.Register("SeriesAxis", typeof(SeriesAxis), typeof(OlapChart), new UIPropertyMetadata(new SeriesAxis()));

        public static readonly DependencyProperty SlicerAxisProperty =
            DependencyProperty.Register("SlicerAxis", typeof(SlicerAxis), typeof(OlapChart), new UIPropertyMetadata(new SlicerAxis()));

        public static readonly DependencyProperty CalculatedMembersProperty =
            DependencyProperty.Register("CalculatedMembers", typeof(CalculatedMembers), typeof(OlapChart), new UIPropertyMetadata(new CalculatedMembers()));


        public static readonly DependencyProperty LabelForegroundProperty =
            DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(OlapChart), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Visual Style Dependency Property
        /// </summary>
        public static readonly DependencyProperty VisualStyleProperty =
            DependencyProperty.Register("VisualStyle", typeof(OlapChartVisualStyle), typeof(OlapChart), new UIPropertyMetadata(OlapChartVisualStyle.Default, OnVisualStyleChanged));

        /// <summary>
        /// Using a DependencyProperty as the backing store for ColorPalette
        /// </summary>
        public static readonly DependencyProperty ColorPaletteProperty =
            DependencyProperty.Register("ColorPalette", typeof(ChartColorPalette), typeof(OlapChart), new UIPropertyMetadata(ChartColorPalette.Default,
                (d, args) =>
                {
                    OlapChart olapChart = d as OlapChart;
                    if (olapChart.m_area != null)
                    {
                        olapChart.ColorModel.Palette = (ChartColorPalette)args.NewValue;
                    }
                }));

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OlapChart"/> class.
        /// </summary>
        public OlapChart()
        {
            EnvironmentTestTools.ValidateLicense(typeof(OlapChart));
            CoerceValue(OlapChart.PrimaryAxisProperty);
            Loaded += new RoutedEventHandler(OlapChart_Loaded);
        }

        void OlapChart_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.OlapDataManager == null)
            {
                if (!string.IsNullOrEmpty(this.SharedDataManagerName) && SharedDataManagers.Instance.DataManagers != null && SharedDataManagers.Instance.DataManagers.Count > 0)
                {
                    foreach (var dataManager in SharedDataManagers.Instance.DataManagers)
                    {
                        if (dataManager != null && !string.IsNullOrEmpty(dataManager.Name) && dataManager.OlapDataManager != null && dataManager.Name.Equals(this.SharedDataManagerName))
                        {
                            if (this.CategoricalAxis.Count > 0 || this.SeriesAxis.Count > 0 || this.SlicerAxis.Count > 0)
                            {
                                dataManager.OlapDataManager.SetCurrentReport(CreateOlapReport());
                            }

                            this.OlapDataManager = dataManager.OlapDataManager;
                        }
                    }
                }
            }
            else if (this.OlapDataManager != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager)
            {
                (this.OlapDataManager as OlapDataManager).ActiveReport = this.OlapDataManager.Reports[this.ReportName];
                Syncfusion.Olap.Data.CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
                this.PivotEngine = this.OlapDataManager.ExecuteOlapTable(cellSet, GridLayout.NoSummaries);
                (this.OlapDataManager as OlapDataManager).ActiveReportChanged += new ActiveReportChangedEventHandler(OlapChart_ActiveReportChanged);
            }
        }

        void OlapChart_ActiveReportChanged(object sender, ActiveReportChangedEventArgs e)
        {
            if (e.NewActiveReport != null && e.NewActiveReport.Name == this.ReportName && e.IsReportChanged)
            {
                Syncfusion.Olap.Data.CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
                this.PivotEngine = this.OlapDataManager.ExecuteOlapTable(cellSet, GridLayout.NoSummaries);
            }
        }

        /// <summary>
        /// Initializes the <see cref="OlapChart"/> class.
        /// </summary>
        static OlapChart()
        {
            EnvironmentTestTools.ValidateLicense(typeof(OlapChart));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OlapChart), new FrameworkPropertyMetadata(typeof(OlapChart)));

            //Registering Chart command bindings.
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ApplicationCommands.Print, OnPrintCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ChartCommands.SwitchPrinting, OnSwitchPrintingCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ApplicationCommands.Copy, OnCopyCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ApplicationCommands.Save, OnSaveCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ChartAreaCommands.ZoomIn, OnZoomInCommand, CanExecuteZoomInCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ChartAreaCommands.ZoomOut, OnZoomOutCommand, CanExecuteZoomOutCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ChartAreaCommands.ZoomReset, OnZoomResetCommand, CanExecuteZoomResetCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ChartAreaCommands.CancelZooming, OnCancelZoomingCommand, CanExecuteCancelZoomingCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(ChartAreaCommands.SwitchZooming, OnSwitchZoomingCommand, CanExecuteSwitchZoomingCommand));

            //Registering OlapChart Command bindings
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(OlapChartCommands.DataBind, OnDataBindCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(OlapChartCommands.SetChartApperanceDetails, OnSetChartAppDetailsCommand));
            CommandManager.RegisterClassCommandBinding(typeof(OlapChart), new CommandBinding(OlapChartCommands.ShowAppearanceDialog, OnShowAppDlgCommand));
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs after OLAP area refresh.
        /// </summary>
        public event OlapRefreshEventHandler AfterRefresh
        {
            add
            {
                VerifyAreaAccess("AfterRefresh event");
                m_area.AfterRefresh += value;
            }
            remove
            {
                VerifyAreaAccess("AfterRefresh event");
                m_area.AfterRefresh -= value;
            }
        }

        /// <summary>
        /// Occurs before area refresh.
        /// </summary>
        public event OlapRefreshEventHandler BeforeRefresh
        {
            add
            {
                VerifyAreaAccess("BeforeRefresh event");
                m_area.BeforeRefresh += value;
            }
            remove
            {
                VerifyAreaAccess("BeforeRefresh event");
                m_area.BeforeRefresh -= value;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "VisualStyle")
            {
                if (this.InternalChart != null)
                {
                    SkinStorage.SetVisualStyle(this.InternalChart, e.NewValue.ToString());
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the shared data manager.
        /// </summary>
        /// <value>The name of the shared data manager.</value>
        public string SharedDataManagerName { get; set; }

        /// <summary>
        /// Gets or sets the name of the report.
        /// </summary>
        /// <value>The name of the report.</value>
        public string ReportName { get; set; }

        /// <summary>
        /// Gets or sets the name of the current cube.
        /// </summary>
        /// <value>The name of the current cube.</value>
        public string CurrentCubeName { get; set; }

        /// <summary>
        /// Gets or sets the categorical axis.
        /// </summary>
        /// <value>The categorical axis.</value>
        [Browsable(false)]
        public CategoricalAxis CategoricalAxis
        {
            get { return (CategoricalAxis)GetValue(CategoricalAxisProperty); }
            set { SetValue(CategoricalAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the series axis.
        /// </summary>
        /// <value>The series axis.</value>
        [Browsable(false)]
        public SeriesAxis SeriesAxis
        {
            get { return (SeriesAxis)GetValue(SeriesAxisProperty); }
            set { SetValue(SeriesAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the slicer axis.
        /// </summary>
        /// <value>The slicer axis.</value>
        [Browsable(false)]
        public SlicerAxis SlicerAxis
        {
            get { return (SlicerAxis)GetValue(SlicerAxisProperty); }
            set { SetValue(SlicerAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the calculated members.
        /// </summary>
        /// <value>The calculated members.</value>
        public CalculatedMembers CalculatedMembers
        {
            get { return (CalculatedMembers)GetValue(CalculatedMembersProperty); }
            set { SetValue(CalculatedMembersProperty, value); }
        }

        /// <summary>
        /// Gets or sets the designer settings.
        /// </summary>
        /// <value>The designer settings.</value>
        [Browsable(false)]
        public DesignerSettings DesignerSettings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the chart appearance.
        /// </summary>
        /// <value>The chart appearance.</value>
        [Browsable(false)]
        public ChartAppearanceSettings ChartAppearance
        {
            get
            {
                return GetChartAppearanceDetails();
            }
            set
            {
                SetChartApperanceDetails(value);
            }
        }

        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>The type of the chart.</value>
        public ChartTypes ChartType
        {
            get { return (ChartTypes)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }

        /// <summary>
        /// Gets the color model used to paint series.
        /// </summary>
        /// <value>The color model.</value>
        [Browsable(false)]
        public ChartStyleModel ColorModel
        {
            get
            {
                VerifyAreaAccess("ColorModel property");
                return m_area.ColorModel;
            }
        }

        /// <summary>
        /// Gets or sets the color palette for the chart area.
        /// </summary>
        public ChartColorPalette ColorPalette
        {
            get { return (ChartColorPalette)GetValue(ColorPaletteProperty); }
            set { SetValue(ColorPaletteProperty, value); }
        }

        /// <summary>
        /// Gets or sets the corner radius of OLAP Chart.
        /// </summary>
        /// <value>The corner radius.</value>
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the grid background.
        /// </summary>
        /// <value>The grid background.</value>
        public Brush GridBackground
        {
            get { return (Brush)GetValue(GridBackgroundProperty); }
            set { SetValue(GridBackgroundProperty, value); }
        }


        /// <summary>
        /// Get or sets AxisLabelForegroundbrush
        /// </summary>
        public Brush LabelForeground
        {
            get { return (Brush)GetValue(LabelForegroundProperty); }
            set { SetValue(LabelForegroundProperty, value); }
        }
        /// <summary>
        /// Gets or sets the grid line stroke.
        /// </summary>
        /// <value>The grid line stroke.</value>
        [Browsable(false)]
        public Pen GridLineStroke
        {
            get { return (Pen)GetValue(GridLineStrokeProperty); }
            set { SetValue(GridLineStrokeProperty, value); }
        }


        /// <summary>
        /// Gets or sets the legend.
        /// </summary>
        /// <value>
        /// The legend.
        /// </value>
        [Browsable(false)]
        public ChartLegend Legend
        {
            get { return (ChartLegend)GetValue(LegendProperty); }
            set { SetValue(LegendProperty, value); }
        }

        /// <summary>
        /// Gets or sets the cube model.
        /// </summary>
        /// <value>The cube model instance.</value>
        [Browsable(false)]
        public IOlapDataManager OlapDataManager
        {
            get { return (IOlapDataManager)GetValue(OlapDataManagerProperty); }
            set
            {
                ((OlapChartAxis)this.PrimaryAxis).ProcessingLabelsState = true;
                SetValue(OlapDataManagerProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the primary axis.
        /// </summary>
        /// <value>The primary axis.</value>
        [Browsable(false)]
        public OlapChartAxis PrimaryAxis
        {
            get { return (OlapChartAxis)GetValue(PrimaryAxisProperty); }
            set { SetValue(PrimaryAxisProperty, value); }
        }

        /// <summary>
        /// Gets or sets the secondary axis.
        /// </summary>
        /// <value>The secondary axis.</value>
        [Browsable(false)]
        public ChartAxis SecondaryAxis
        {
            get { return (ChartAxis)GetValue(SecondaryAxisProperty); }
            set { SetValue(SecondaryAxisProperty, value); }
        }

        /// <summary>
        /// Gets the series collection.
        /// </summary>
        /// <value>The series.</value>
        [Browsable(false)]
        public SeriesReadOnlyCollection Series
        {
            get
            {
                VerifyAreaAccess("Series property");
                return m_area.Series;
            }
        }

        /// <summary>
        /// Gets or sets the stroke thickness for each series in the <see cref="OlapChart"/>.
        /// </summary>
        public double SeriesStrokeThickness
        {
            get { return (double)GetValue(SeriesStrokeThicknessProperty); }
            set { SetValue(SeriesStrokeThicknessProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether each series should be updated with different colors. By default false.
        /// </summary>
        public bool ColorEachSeries
        {
            get { return (bool)GetValue(ColorEachSeriesProperty); }
            set { SetValue(ColorEachSeriesProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether enable animation for each series.
        /// </summary>
        public bool EnableSeriesAnimation
        {
            get { return (bool)GetValue(EnableSeriesAnimationProperty); }
            set { SetValue(EnableSeriesAnimationProperty, value); }
        }

        /// <summary>
        /// Gets or sets animation option for each series.
        /// </summary>
        public AnimationOptions SeriesAnimateOption
        {
            get { return (AnimationOptions)GetValue(SeriesAnimateOptionProperty); }
            set { SetValue(SeriesAnimateOptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether enable effect on each series. By default false.
        /// </summary>
        public bool EnableSeriesEffects
        {
            get { return (bool)GetValue(EnableSeriesEffectsProperty); }
            set { SetValue(EnableSeriesEffectsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the animation duration for each series.
        /// </summary>
        public TimeSpan SeriesAnimationDuration
        {
            get { return (TimeSpan)GetValue(SeriesAnimationDurationProperty); }
            set { SetValue(SeriesAnimationDurationProperty, value); }
        }

        /// <summary>
        /// Gets or sets whether each series animate one by one. By default false.
        /// </summary>
        public bool SeriesAnimateOneByOne
        {
            get { return (bool)GetValue(SeriesAnimateOneByOneProperty); }
            set { SetValue(SeriesAnimateOneByOneProperty, value); }
        }        
                    
        /// <summary>
        /// Gets or sets the display mode.
        /// </summary>
        /// <value>The display mode.</value>
        [Category("Appearance")]
        public DisplayMode DisplayMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>The tool tip template.</value>
        [Browsable(false)]
        public ControlTemplate SeriesToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(SeriesToolTipTemplateProperty); }
            set { SetValue(SeriesToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the KPI alignment.
        /// </summary>
        /// <value>The KPI alignment.</value>
        [Category("Layout")]
        public KpiAlignment KpiAlignment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is data bind called.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is data bind called; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        internal bool IsDataBindCalled { get; set; }

        /// <summary>
        /// Gets or sets the primary axis label visibility.
        /// </summary>
        /// <value>
        /// The primary axis label visibility.
        /// </value>
        public Visibility PrimaryAxisLabelVisibility
        {
            get { return (Visibility)GetValue(PrimaryAxisLabelVisibilityProperty); }
            set { SetValue(PrimaryAxisLabelVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the internal chart.
        /// </summary>
        /// <value>
        /// The internal chart.
        /// </value>
        internal Chart InternalChart { get; set; }

        /// <summary>
        /// Gets or sets the expander style.
        /// </summary>
        /// <value>
        /// The expander style.
        /// </value>
        [Browsable(false)]
        public System.Windows.Style ExpanderStyle
        {
            get { return (System.Windows.Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the VisualStyle for OlapChart
        /// </summary>
        public OlapChartVisualStyle VisualStyle
        {
            get { return (OlapChartVisualStyle)GetValue(VisualStyleProperty); }
            set { SetValue(VisualStyleProperty, value); }
        }

        private bool _optimizeLargeDataLoading;
        /// <summary>
        /// Gets or sets the OptimizeLargeDataLoading property. By default, false. If it is true then Chart will be render with Minimum number of series data.
        /// </summary>
        public bool OptimizeLargeDataLoading
        {
            get
            {
                return _optimizeLargeDataLoading;
            }
            set
            {
                if (_optimizeLargeDataLoading != value)
                {
                    _optimizeLargeDataLoading = value;
                    if (_optimizeLargeDataLoading)
                    {
                        this.Legend = null;
                        this.ChartType = ChartTypes.FastColumn;
                    }
                    else
                    {
                        this.Legend = this.Legend ?? new ChartLegend();
                        this.ChartType = ChartTypes.Column;
                    }
                }
            }
        }
        #endregion

        #region Implementation

        #region Static Methods
        /// <summary>
        /// Determines whether <see cref="OlapChart"/> can execute cancel zooming command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteCancelZoomingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null && chart.ChartType != ChartTypes.Pie)
            {
                e.CanExecute = chart.m_area.ZoomSwitched && ChartAreaCommands.CancelZooming.CanExecute(null, chart.m_area);
            }
        }

        /// <summary>
        /// Determines whether <see cref="OlapChart"/> can execute switch zooming command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteSwitchZoomingCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null && chart.ChartType != ChartTypes.Pie)
            {
                e.CanExecute = !chart.m_area.ZoomSwitched;
            }
        }

        /// <summary>
        /// Determines whether <see cref="OlapChart"/> can execute zoom in command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteZoomInCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null && chart.ChartType != ChartTypes.Pie)
            {
                foreach (ChartAxis axis in chart.m_area.Axes)
                {
                    if (axis.ZoomFactor > axis.MinimalZoomFactor)
                    {
                        e.CanExecute = ChartAreaCommands.ZoomIn.CanExecute(null, chart.m_area);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether <see cref="OlapChart"/> can execute zoom out command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteZoomOutCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null && chart.ChartType != ChartTypes.Pie)
            {
                foreach (ChartAxis axis in chart.m_area.Axes)
                {
                    if (axis.ZoomFactor < 1)
                    {
                        e.CanExecute = ChartAreaCommands.ZoomOut.CanExecute(null, chart.m_area);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether this <see cref="OlapChart"/> can execute zoom reset command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        private static void CanExecuteZoomResetCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null && chart.ChartType != ChartTypes.Pie)
            {
                foreach (ChartAxis axis in chart.m_area.Axes)
                {
                    if (axis.ZoomFactor < 1)
                    {
                        e.CanExecute = ChartAreaCommands.ZoomReset.CanExecute(null, chart.m_area);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Coerces the primary axis.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoercePrimaryAxis(DependencyObject d, object baseValue)
        {
            return baseValue ?? new OlapChartAxis();
        }

        private static void OnGridLineStrokeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapChart chart = d as OlapChart;
            if (chart != null)
            {
                Pen gridLineStroke = e.NewValue as Pen;
                if (chart.PrimaryAxis != null)
                {
                    chart.PrimaryAxis.GroupLineStroke = gridLineStroke;
                    chart.PrimaryAxis.LineStroke = gridLineStroke;
                    chart.PrimaryAxis.TickLineStroke = gridLineStroke;
                    if (chart.PrimaryAxis.Area != null)
                    {
                        ChartArea.SetGridLineStroke(chart.PrimaryAxis, gridLineStroke);
                    }
                }
                if (chart.SecondaryAxis != null)
                {
                    chart.SecondaryAxis.LineStroke = gridLineStroke;
                    chart.SecondaryAxis.TickLineStroke = gridLineStroke;
                    if (chart.SecondaryAxis.Area != null)
                    {
                        ChartArea.SetGridLineStroke(chart.SecondaryAxis, gridLineStroke);
                    }
                }
            }
        }

        /// <summary>
        /// Calling method when [VisualStyle changed]
        /// </summary>
        /// <param name="dependencyObject">Olap Chart</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnVisualStyleChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ResourceDictionary resource = new ResourceDictionary();

            OlapChart olapChart = dependencyObject as OlapChart;
            if (olapChart != null)
            {
                switch ((OlapChartVisualStyle)e.NewValue)
                {
                    case OlapChartVisualStyle.Blend:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Blend.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["BlendOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["Blend.LegendForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["BlendChartLegendStyle"] as Style;
                        if(olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["BlendChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Default:
                        break;
                    case OlapChartVisualStyle.Metro:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Metro.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["MetroOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["MetroThemeLegendForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["MetroChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["MetroChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Office2003:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2003.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["Office2003OlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["Office2003.LegendForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["Office2003ChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["Office2003ChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Office2007Black:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2007Black.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["Office2007BlackOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["Office2007Black.LegendForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["Office2007BlackChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["Office2007BlackChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Office2007Blue:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2007Blue.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["Office2007BlueOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["Office2007Blue.LegendForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["Office2007BlueChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["Office2007BlueChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Office2007Silver:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2007Silver.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["Office2007SilverOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["Office2007Silver.LegendForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["Office2007SilverChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["Office2007SilverChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Office2010Black:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2010Black.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["Office2010BlackOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = new SolidColorBrush(Colors.White);
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["Office2010BlackChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["Office2010BlackChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Office2010Blue:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2010Blue.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["Office2010BlueOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["Office2010Blue.LabelForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["Office2010BlueChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["Office2010BlueChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Office2010Silver:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Office2010Silver.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["Office2010SilverOlapChartStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = resource["Office2010Silver.LabelForeground"] as Brush;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["Office2010SilverChartLegendStyle"] as Style;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["Office2010SilverChartStyle"] as Style;
                        break;
                    case OlapChartVisualStyle.Transparent:
                        resource.Source = new Uri("/Syncfusion.OlapChart.WPF;component/Themes/Generic.Brushes.Transparent.xaml", UriKind.RelativeOrAbsolute);
                        olapChart.Style = resource["TransparentOlapChartStyle"] as Style;
                        if (olapChart.Legend != null)
                            olapChart.Legend.Style = resource["TransparentChartLegendStyle"] as Style;
                        olapChart.PrimaryAxis.LabelForeground = new SolidColorBrush(Colors.Black);
                        olapChart.Legend.Background = resource["TransparentLegendBackground"] as LinearGradientBrush;
                        if (olapChart.InternalChart != null)
                            olapChart.InternalChart.Style = resource["TransparentChartStyle"] as Style;
                        break;
                    default:
                        break;
                }
                if(olapChart.Legend != null)
                    olapChart.Legend.Background = new SolidColorBrush(Colors.Transparent);
                SkinStorage.SetVisualStyle(olapChart, e.NewValue.ToString());
            }
        }
        #endregion

        #region Instant Methods

        /// <summary>
        /// Binds the data.
        /// </summary>
        public void DataBind()
        {
            //VerifyAreaAccess("Data Bind()");
            if (m_area != null && this.OlapDataManager != null)
            {
                m_area.DataBind();
            }
        }

        private ChartAppearanceSettings GetChartAppearanceDetails()
        {
            ChartAppearanceSettings chartAppearance = new ChartAppearanceSettings();

            if (this.BorderBrush != null)
            {
                Color borderColor = ((SolidColorBrush)this.BorderBrush).Color;
                chartAppearance.BorderColor = System.Drawing.Color.FromArgb(borderColor.A, borderColor.R, borderColor.G, borderColor.B);
            }
            chartAppearance.ChartColorPalette = this.ColorModel.Palette.ToString();

            //Label Tab

            if (this.Series.Count > 0)
            {
                if (this.Series[0].AdornmentsInfo.LabelContentPath == "DataPoint.X")
                    chartAppearance.IsXValues = true;
                if (this.Series[0].AdornmentsInfo.LabelContentPath == "DataPoint.Y")
                    chartAppearance.IsYValues = true;
                if (this.Series[0].AdornmentsInfo.LabelContentPath == "Series.Label")
                    chartAppearance.IsSeriesName = true;

                if (chartAppearance.IsXValues || chartAppearance.IsYValues || chartAppearance.IsSeriesName)
                    chartAppearance.LabelsVisibility = true;
                else
                    chartAppearance.LabelsVisibility = false;


                ChartAdornmentInfo cai = this.Series[0].AdornmentsInfo;
                ResourceDictionary c_resourcedictionary = new ResourceDictionary();
                c_resourcedictionary.Source = new Uri(c_Key, UriKind.RelativeOrAbsolute);
                if (cai.SymbolTemplate != null)
                {
                    if (symbolTemplate == "Circle")
                        chartAppearance.IsCircleSymbol = true;
                    if (symbolTemplate == "Rectangle")
                        chartAppearance.IsRectangleSymbol = true;
                    if (symbolTemplate == "Triangle")
                        chartAppearance.IsTriangleSymbol = true;

                    if (chartAppearance.IsCircleSymbol || chartAppearance.IsRectangleSymbol || chartAppearance.IsTriangleSymbol)
                        chartAppearance.SymbolsVisibility = true;
                    else
                        chartAppearance.SymbolsVisibility = false;
                }
            }

            if (this.GridBackground != null)
            {
                SolidColorBrush solidbrush = this.GridBackground as SolidColorBrush;
                if (solidbrush != null)
                {
                    Color interiorBackground = solidbrush.Color;
                    chartAppearance.InteriorBackground = System.Drawing.Color.FromArgb(interiorBackground.A, interiorBackground.R, interiorBackground.G, interiorBackground.B);
                }
            }

            if (this.Background != null)
            {
                Color chartBackground = ((SolidColorBrush)this.Background).Color;
                chartAppearance.ChartBackground = System.Drawing.Color.FromArgb(chartBackground.A, chartBackground.R, chartBackground.G, chartBackground.B);
            }

            if (this.Legend != null)
            {
                chartAppearance.ChartDockLegendPosition = Chart.GetDock(this.Legend).ToString();
                if (this.Legend.Visibility == Visibility.Visible)
                    chartAppearance.LegendVisibility = true;
                else if (this.Legend.Visibility == Visibility.Collapsed)
                    chartAppearance.LegendVisibility = false;
                if (this.Legend.CheckBoxVisibility == Visibility.Visible)
                    chartAppearance.LegendCheckBoxVisibility = true;
                else if (this.Legend.CheckBoxVisibility == Visibility.Collapsed)
                    chartAppearance.LegendCheckBoxVisibility = false;
            }

            chartAppearance.StrokeThickness = this.BorderThickness.Top;
            chartAppearance.ChartType = this.ChartType.ToString();
            chartAppearance.XAxisFontFace = this.PrimaryAxis.LabelFontFamily.ToString();
            Color xAxisForeGround = ((SolidColorBrush)this.PrimaryAxis.LabelForeground).Color;
            chartAppearance.XAxisForeGround = System.Drawing.Color.FromArgb(xAxisForeGround.A, xAxisForeGround.R, xAxisForeGround.G, xAxisForeGround.B);

            if (this.SecondaryAxis != null)
            {
                chartAppearance.YAxisFontFace = this.SecondaryAxis.LabelFontFamily.ToString();
                Color yAxisForeGround = ((SolidColorBrush)this.SecondaryAxis.LabelForeground).Color;
                chartAppearance.YAxisForeGround = System.Drawing.Color.FromArgb(yAxisForeGround.A, yAxisForeGround.R, yAxisForeGround.G, yAxisForeGround.B);
                chartAppearance.YLabelFontWeight = this.SecondaryAxis.LabelFontWeight.ToString();
                chartAppearance.ShowVerticalGridLines = ChartArea.GetShowGridLines(this.SecondaryAxis);
            }
            if (this.PrimaryAxis != null)
            {
                chartAppearance.XLabelFontWeight = this.PrimaryAxis.LabelFontWeight.ToString();
                chartAppearance.ShowHorizontalGridLines = ChartArea.GetShowGridLines(this.PrimaryAxis);
            }
           
            return chartAppearance;
        }

        public System.Drawing.Image GetChartImage()
        {
            Visual visual = this.Template.FindName("PART_OlapArea", this) as Visual;

            if (visual != null)
            {
                BitmapEncoder encoder = Chart.CreateBitmapEncoderByExtension("png");
                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)this.ActualWidth,
                  (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                Rectangle backgroundRect = new Rectangle();

                backgroundRect.Fill = Brushes.White;
                backgroundRect.Arrange(new Rect(this.RenderSize));
                bmpSource.Render(backgroundRect);
                bmpSource.Render(visual);
                encoder.Frames.Add(BitmapFrame.Create(bmpSource));

                MemoryStream m_stream = new MemoryStream();
                encoder.Save(m_stream);

                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(m_stream);
                return bitmap;
            }
            return null;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //Retrieving OlapArea templated child.
            m_area = GetTemplateChild("PART_OlapArea") as OlapArea;
            this.InternalChart = GetTemplateChild("PART_Chart") as Chart;
            //Style style = FindResource("OlapChartAreaTemplate") as System.Windows.Style;

            //Discarding template that does not have OlapArea in the visual tree.
            if (m_area == null)
            {
                throw new NotSupportedException("OlapChart template must contain OlapChartArea");
            }
            if (this.ColorModel != null) this.ColorModel.Palette = this.ColorPalette;
            m_area.ChartControl = this;
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                if (this.OlapDataManager != null)
                {
                    if (this.CategoricalAxis.Count > 0 || this.SeriesAxis.Count > 0 || this.SlicerAxis.Count > 0)
                    {
                        m_area.OlapDataManager.SetCurrentReport(this.CreateOlapReport());
                       
                    }
                         m_area.OlapDataManager = this.OlapDataManager as OlapDataManager;
                         if ((this.OlapDataManager as OlapDataManager).ActiveReport != null && (this.OlapDataManager as OlapDataManager).UseSharedDataManager)
                    {
                        m_area.SetValue(OlapArea.PivotEnginePropertyKey, this.PivotEngine as PivotEngine);
                    }
                    
                }
                else if (!string.IsNullOrEmpty(this.SharedDataManagerName) && SharedDataManagers.Instance.DataManagers != null && SharedDataManagers.Instance.DataManagers.Count > 0)
                {
                    foreach (var dataManager in SharedDataManagers.Instance.DataManagers)
                    {
                        if (dataManager != null && !string.IsNullOrEmpty(dataManager.Name) && dataManager.OlapDataManager != null && dataManager.Name.Equals(this.SharedDataManagerName))
                        {
                            if (this.CategoricalAxis.Count > 0 || this.SeriesAxis.Count > 0 || this.SlicerAxis.Count > 0)
                            {
                                dataManager.OlapDataManager.SetCurrentReport(CreateOlapReport());
                            }

                            this.OlapDataManager = dataManager.OlapDataManager;
                        }
                    }
                }
            }

            if (this.IsDataBindCalled)
                m_area.DataBind();


            if (this.DesignerSettings != null && !DesignerProperties.GetIsInDesignMode(this))
            {
                OlapDataManager olapDataManager = new OlapDataManager(this.DesignerSettings.ConnectionString);
                olapDataManager.SetCurrentReport(this.DesignerSettings.GetOlapReport());
                this.OlapDataManager = olapDataManager;
            }
            if (this.PrimaryAxis != null)
            {
                if (this.PrimaryAxis.Area != null)
                {
                    ChartArea.SetGridLineStroke(this.PrimaryAxis, GridLineStroke);
                }
            }
            if (this.SecondaryAxis != null)
            {
                if (this.SecondaryAxis.Area != null)
                {
                    ChartArea.SetGridLineStroke(this.SecondaryAxis, GridLineStroke);
                }

                Binding bind = new Binding("LabelForeground") { Mode = BindingMode.TwoWay, Source = this };
                this.SecondaryAxis.SetBinding(ChartAxis.LabelForegroundProperty, bind);
            }

        }

        private OlapReport CreateOlapReport()
        {
            OlapReport report = new OlapReport { Name = this.ReportName };
            try
            {
                report.CurrentCubeName = this.CurrentCubeName;

                #region Categorical Axis
                if (this.CategoricalAxis.Count > 0)
                {
                    foreach (var item in this.CategoricalAxis)
                    {
                        #region Dimension
                        if (item is Dimension)
                        {
                            Dimension dimension = item as Dimension;
                            DimensionElement dimensionElementColumn = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                            dimensionElementColumn.Hierarchy = new HierarchyElement { Name = dimension.HierarchyName };
                            dimensionElementColumn.AddLevel(dimension.HierarchyName, dimension.LevelName);
                            if (dimension.MemberProperties.Count > 0)
                            {
                                foreach (var memberProperty in dimension.MemberProperties)
                                {
                                    dimensionElementColumn.MemberProperties.Add(memberProperty.Name, memberProperty.UniqueName);
                                }
                            }

                            if (dimension.DimensionType == DimesnionType.Exclude)
                            {
                                DimensionElement excludedDimensionElementColumn = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                                excludedDimensionElementColumn.AddLevel(dimension.HierarchyName, dimension.LevelName);
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(excludedDimensionElementColumn.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.CategoricalElements.Add(dimensionElementColumn, excludedDimensionElementColumn);
                            }
                            else
                            {
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(dimensionElementColumn.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.CategoricalElements.Add(new Item { ElementValue = dimensionElementColumn });
                            }
                        }
                        #endregion
                        #region Measure
                        else if (item is Measure)
                        {
                            Measure measure = item as Measure;
                            MeasureElements measureElementsColumn = new MeasureElements();
                            measureElementsColumn.Elements.Add(new MeasureElement { Name = measure.Name });
                            report.CategoricalElements.Add(measureElementsColumn);
                        }
                        #endregion
                        #region KPI
                        else if (item is Kpi)
                        {
                            Kpi kpi = item as Kpi;
                            KpiElements kpiElements = new KpiElements();
                            kpiElements.Elements.Add(new KpiElement { Name = kpi.Name, ShowKPIGoal = kpi.ShowGoal, ShowKPIStatus = kpi.ShowStatus, ShowKPITrend = kpi.ShowTrend, ShowKPIValue = kpi.ShowValue });
                            report.CategoricalElements.Add(kpiElements);
                        }
                        #endregion
                        #region NamedSet
                        else if (item is NamedSet)
                        {
                            NamedSet namedSet = item as NamedSet;
                            NamedSetElement namedSetElement = new NamedSetElement { Name = namedSet.Name, DimensionName = namedSet.DimensionName, DimensionUniqueName = namedSet.DimensionUniqueName };
                            report.CategoricalElements.Add(namedSetElement);
                        }
                        #endregion
                        #region SortElement
                        else if (item is Sort)
                        {
                            Sort sortItem = item as Sort;
                            SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), sortItem.SortOrder.ToString(), true);
                            SortElement sortElement = new SortElement(AxisPosition.Categorical, sortOrder, true);
                            sortElement.Element.Name = sortItem.MeasureUniqueName;
                            report.CategoricalElements.Add(sortElement);
                        }
                        #endregion
                        #region SubSet
                        else if (item is SubSet)
                        {
                            SubSet subSetItem = item as SubSet;
                            SubsetElement subSetElement;
                            if (subSetItem.StartIndex != -1 && subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.StartIndex, subSetItem.EndIndex);
                            }
                            else if (subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.EndIndex);
                            }
                            else
                            {
                                subSetElement = new SubsetElement();
                            }
                            subSetElement.Name = subSetItem.Name;
                            report.CategoricalElements.SubSetElement = subSetElement;
                        }
                        #endregion
                        #region TopCountElement
                        else if (item is TopCount)
                        {
                            TopCount topCountItem = item as TopCount;
                            TopCountElement topCountElement = new TopCountElement(AxisPosition.Categorical, topCountItem.FieldCount);
                            topCountElement.MeasureName = topCountItem.MeasureName;
                            report.CategoricalElements.Add(topCountElement);
                        }
                        #endregion

                    }
                }
                #endregion

                #region Series Axis
                if (this.SeriesAxis.Count > 0)
                {
                    foreach (var item in this.SeriesAxis)
                    {
                        #region Dimension
                        if (item is Dimension)
                        {
                            Dimension dimension = item as Dimension;
                            DimensionElement dimensionElementRow = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                            //dimensionElementRow.Hierarchy = new HierarchyElement { Name = dimension.HierarchyName };
                            dimensionElementRow.AddLevel(dimension.HierarchyName, dimension.LevelName);
                            if (dimension.MemberProperties.Count > 0)
                            {
                                foreach (var memberProperty in dimension.MemberProperties)
                                {
                                    dimensionElementRow.MemberProperties.Add(memberProperty.Name, memberProperty.UniqueName);
                                }
                            }

                            if (dimension.DimensionType == DimesnionType.Exclude)
                            {
                                DimensionElement excludedDimensionElementRow = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                                excludedDimensionElementRow.AddLevel(dimension.HierarchyName, dimension.LevelName);
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(excludedDimensionElementRow.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SeriesElements.Add(dimensionElementRow, excludedDimensionElementRow);
                            }
                            else
                            {
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(dimensionElementRow.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SeriesElements.Add(new Item { ElementValue = dimensionElementRow });
                            }
                        }
                        #endregion
                        #region Measure
                        else if (item is Measure)
                        {
                            Measure measure = item as Measure;
                            MeasureElements measureElementsRow = new MeasureElements();
                            measureElementsRow.Elements.Add(new MeasureElement { Name = measure.Name });
                            report.SeriesElements.Add(measureElementsRow);
                        }
                        #endregion
                        #region KPI
                        else if (item is Kpi)
                        {
                            Kpi kpi = item as Kpi;
                            KpiElements kpiElements = new KpiElements();
                            kpiElements.Elements.Add(new KpiElement { Name = kpi.Name, ShowKPIGoal = kpi.ShowGoal, ShowKPIStatus = kpi.ShowStatus, ShowKPITrend = kpi.ShowTrend, ShowKPIValue = kpi.ShowValue });
                            report.SeriesElements.Add(kpiElements);
                        }
                        #endregion
                        #region NamedSet
                        else if (item is NamedSet)
                        {
                            NamedSet namedSet = item as NamedSet;
                            NamedSetElement namedSetElement = new NamedSetElement { Name = namedSet.Name, DimensionName = namedSet.DimensionName, DimensionUniqueName = namedSet.DimensionUniqueName };
                            report.SeriesElements.Add(namedSetElement);
                        }
                        #endregion
                        #region SortElement
                        else if (item is Sort)
                        {
                            Sort sortItem = item as Sort;
                            SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), sortItem.SortOrder.ToString(), true);
                            SortElement sortElement = new SortElement(AxisPosition.Categorical, sortOrder, true);
                            sortElement.Element.Name = sortItem.MeasureUniqueName;
                            report.SeriesElements.Add(sortElement);
                        }
                        #endregion
                        #region SubSet
                        else if (item is SubSet)
                        {
                            SubSet subSetItem = item as SubSet;
                            SubsetElement subSetElement;
                            if (subSetItem.StartIndex != -1 && subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.StartIndex, subSetItem.EndIndex);
                            }
                            else if (subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.EndIndex);
                            }
                            else
                            {
                                subSetElement = new SubsetElement();
                            }
                            subSetElement.Name = subSetItem.Name;
                            report.SeriesElements.SubSetElement = subSetElement;
                        }
                        #endregion
                        #region TopCountElement
                        else if (item is TopCount)
                        {
                            TopCount topCountItem = item as TopCount;
                            TopCountElement topCountElement = new TopCountElement(AxisPosition.Categorical, topCountItem.FieldCount);
                            topCountElement.MeasureName = topCountItem.MeasureName;
                            report.SeriesElements.Add(topCountElement);
                        }
                        #endregion
                    }
                }
                #endregion

                #region Slicer Axis
                if (this.SlicerAxis.Count > 0)
                {
                    foreach (var item in this.SlicerAxis)
                    {
                        #region Dimension
                        if (item is Dimension)
                        {
                            Dimension dimension = item as Dimension;
                            DimensionElement dimensionElementSlicer = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                            dimensionElementSlicer.Hierarchy = new HierarchyElement { Name = dimension.HierarchyName };
                            dimensionElementSlicer.AddLevel(dimension.HierarchyName, dimension.LevelName);
                            if (dimension.MemberProperties.Count > 0)
                            {
                                foreach (var memberProperty in dimension.MemberProperties)
                                {
                                    dimensionElementSlicer.MemberProperties.Add(memberProperty.Name, memberProperty.UniqueName);
                                }
                            }

                            if (dimension.DimensionType == DimesnionType.Exclude)
                            {
                                DimensionElement excludedDimensionElementSlicer = new DimensionElement { Name = dimension.Name, HierarchyName = dimension.HierarchyName };
                                excludedDimensionElementSlicer.AddLevel(dimension.HierarchyName, dimension.LevelName);
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(excludedDimensionElementSlicer.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SlicerElements.Add(dimensionElementSlicer, excludedDimensionElementSlicer);
                            }
                            else
                            {
                                if (dimension.IncludeMembers != null && dimension.IncludeMembers.Count > 0)
                                {
                                    GetIncludedMembers(dimensionElementSlicer.Hierarchy.LevelElements[dimension.LevelName], dimension.IncludeMembers);
                                }
                                report.SlicerElements.Add(dimensionElementSlicer);
                            }
                        }
                        #endregion
                        #region Measure
                        else if (item is Measure)
                        {
                            Measure measure = item as Measure;
                            MeasureElements measureElementsSlicer = new MeasureElements();
                            measureElementsSlicer.Elements.Add(new MeasureElement { Name = item.Name });
                            report.SlicerElements.Add(measureElementsSlicer);
                        }
                        #endregion
                        #region KPI
                        else if (item is Kpi)
                        {
                            Kpi kpi = item as Kpi;
                            KpiElements kpiElements = new KpiElements();
                            kpiElements.Elements.Add(new KpiElement { Name = kpi.Name, ShowKPIGoal = kpi.ShowGoal, ShowKPIStatus = kpi.ShowStatus, ShowKPITrend = kpi.ShowTrend, ShowKPIValue = kpi.ShowValue });
                            report.SlicerElements.Add(kpiElements);
                        }
                        #endregion
                        #region NamedSet
                        else if (item is NamedSet)
                        {
                            NamedSet namedSet = item as NamedSet;
                            NamedSetElement namedSetElement = new NamedSetElement { Name = namedSet.Name, DimensionName = namedSet.DimensionName, DimensionUniqueName = namedSet.DimensionUniqueName };
                            report.SlicerElements.Add(namedSetElement);
                        }
                        #endregion
                        #region SortElement
                        else if (item is Sort)
                        {
                            Sort sortItem = item as Sort;
                            SortOrder sortOrder = (SortOrder)Enum.Parse(typeof(SortOrder), sortItem.SortOrder.ToString(), true);
                            SortElement sortElement = new SortElement(AxisPosition.Categorical, sortOrder, true);
                            sortElement.Element.Name = sortItem.MeasureUniqueName;
                            report.SlicerElements.Add(sortElement);
                        }
                        #endregion
                        #region SubSet
                        else if (item is SubSet)
                        {
                            SubSet subSetItem = item as SubSet;
                            SubsetElement subSetElement;
                            if (subSetItem.StartIndex != -1 && subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.StartIndex, subSetItem.EndIndex);
                            }
                            else if (subSetItem.EndIndex != -1)
                            {
                                subSetElement = new SubsetElement(subSetItem.EndIndex);
                            }
                            else
                            {
                                subSetElement = new SubsetElement();
                            }
                            subSetElement.Name = subSetItem.Name;
                            report.SlicerElements.SubSetElement = subSetElement;
                        }
                        #endregion
                        #region TopCountElement
                        else if (item is TopCount)
                        {
                            TopCount topCountItem = item as TopCount;
                            TopCountElement topCountElement = new TopCountElement(AxisPosition.Categorical, topCountItem.FieldCount);
                            topCountElement.MeasureName = topCountItem.MeasureName;
                            report.SlicerElements.Add(topCountElement);
                        }
                        #endregion
                    }
                }
                #endregion

                #region CalculatedMembers
                if (this.CalculatedMembers.Count > 0)
                {
                    foreach (CalculatedMember calcMemberItem in this.CalculatedMembers)
                    {
                        TypeOfMember calcMemberType = (TypeOfMember)Enum.Parse(typeof(TypeOfMember), calcMemberItem.MemberType.ToString());
                        Syncfusion.Olap.Reports.CalculatedMember calculatedMember = new Syncfusion.Olap.Reports.CalculatedMember { Expression = calcMemberItem.Expression, Name = calcMemberItem.Name };
                        if (calcMemberType == TypeOfMember.Measure)
                        {
                            calculatedMember.AddElement(new MeasureElement { Name = calcMemberItem.ElementName });
                        }
                        else if (calcMemberType == TypeOfMember.Dimension)
                        {
                            DimensionElement dimenesionElement = new DimensionElement { Name = calcMemberItem.ElementName, HierarchyName = calcMemberItem.ElementHierarchyName };
                            dimenesionElement.AddLevel(calcMemberItem.ElementHierarchyName, calcMemberItem.ElementLevelName);
                            calculatedMember.AddElement(dimenesionElement);
                        }

                        report.CalculatedMembers.Add(calculatedMember);
                        switch (calcMemberItem.AxisPosition)
                        {
                            case PositionOfAxis.Categorical:
                                report.CategoricalElements.Add(calculatedMember);
                                break;
                            case PositionOfAxis.Series:
                                report.SeriesElements.Add(calculatedMember);
                                break;
                            case PositionOfAxis.Slicer:
                                break;
                            default:
                                break;
                        }

                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return report;
        }

        private void GetIncludedMembers(LevelElement levelElement, List<string> list)
        {
            foreach (string member in list)
            {
                levelElement.Add(member.Trim());
            }
            levelElement.IncludeAvailableMembers = true;
        }

        /// <summary>
        /// Handles cancel zooming command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCancelZoomingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                ChartAreaCommands.CancelZooming.Execute(null, chart.m_area);
            }
        }

        /// <summary>
        /// Handles copy command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnCopyCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                Visual visual = chart.Template.FindName("PART_OlapArea", chart) as Visual;

                if (visual != null)
                {
                    RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)chart.ActualWidth,
                      (int)chart.ActualHeight, 96, 96, PixelFormats.Pbgra32);

                    Rectangle backgroundRect = new Rectangle();

                    backgroundRect.Fill = Brushes.White;
                    backgroundRect.Arrange(new Rect(chart.RenderSize));

                    bmpSource.Render(backgroundRect);


                    bmpSource.Render(visual);

                    Clipboard.SetImage(bmpSource);
                }
                else
                {
                    throw new ArgumentNullException("visual");
                }
            }
        }

        /// <summary>
        /// Called when print command is executed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnPrintCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            chart.Print(Rect.Empty);
            Keyboard.Focus(chart);
        }

        /// <summary>
        /// Handles save command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                Visual visual = chart.Template.FindName("PART_OlapArea", chart) as Visual;
                if (visual != null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();

                    saveFileDialog.Filter = c_imageFilesFilter;

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        string fileName = saveFileDialog.FileName;
                        string extension = new FileInfo(fileName).Extension.ToLower(CultureInfo.InvariantCulture);

                        using (Stream stream = File.Create(saveFileDialog.FileName))
                        {
                            if (extension == ".xps")
                            {
                                Chart.SaveToXps(stream, visual);
                            }
                            else
                            {
                                BitmapEncoder encoder = Chart.CreateBitmapEncoderByExtension(extension);
                                RenderTargetBitmap bmpSource = new RenderTargetBitmap((int)chart.ActualWidth,
                                  (int)chart.ActualHeight, 96, 96, PixelFormats.Default);

                                Rectangle backgroundRect = new Rectangle();

                                backgroundRect.Fill = Brushes.White;
                                backgroundRect.Arrange(new Rect(chart.RenderSize));

                                bmpSource.Render(backgroundRect);
                                bmpSource.Render(visual);

                                encoder.Frames.Add(BitmapFrame.Create(bmpSource));
                                encoder.Save(stream);
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentNullException("PART_OlapArea templated part of chart control cannot be retrieved.");
                }
            }
        }

        /// <summary>
        /// Handles switch printing command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSwitchPrintingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                bool hasAdorner = false;
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(chart);
                Adorner[] adorners = adornerLayer.GetAdorners(chart);

                if (adorners != null)
                {
                    foreach (Adorner adorner in adorners)
                    {
                        if (adorner is ChartPrintingAdorner)
                        {
                            adornerLayer.Remove(adorner);
                            hasAdorner = true;
                        }
                    }
                }

                if (!hasAdorner)
                {
                    ChartPrintingAdorner printingAdorner = new ChartPrintingAdorner(chart);
                    printingAdorner.PrintButton.Click += (object printButton, RoutedEventArgs args) =>
                      {
                          chart.Print(printingAdorner.SelectedPrintingArea);
                      };
                    adornerLayer.Add(printingAdorner);
                    Keyboard.Focus(chart);
                }
            }
        }

        /// <summary>
        /// Handles switch zooming command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnSwitchZoomingCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                ChartAreaCommands.SwitchZooming.Execute(null, chart.m_area);
            }
        }

        /// <summary>
        /// Handles zoom in command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnZoomInCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                ChartAreaCommands.ZoomIn.Execute(null, chart.m_area);
            }
        }

        /// <summary>
        /// Handles zoom out command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnZoomOutCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                ChartAreaCommands.ZoomOut.Execute(null, chart.m_area);
            }
        }

        /// <summary>
        /// Handeles zoom reset command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        private static void OnZoomResetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            OlapChart chart = sender as OlapChart;
            if (chart != null)
            {
                ChartAreaCommands.ZoomReset.Execute(null, chart.m_area);
            }
        }

        private static void OnDataBindCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapChart chart = target as OlapChart;
            if (chart != null)
            {
                chart.DataBind();
            }
        }

        private static void OnShowAppDlgCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapChart chart = target as OlapChart;
            if (chart != null)
            {
                chart.ShowAppearanceDialog();
            }
        }

        private static void OnSetChartAppDetailsCommand(object target, ExecutedRoutedEventArgs args)
        {
            OlapChart chart = target as OlapChart;
            if (chart != null && args.Parameter is ChartAppearanceSettings)
            {
                chart.SetChartApperanceDetails(args.Parameter as ChartAppearanceSettings);
            }
        }

        /// <summary>
        /// Prints the specified area of olap chart.
        /// </summary>
        /// <param name="printArea">The print area.</param>
        private void Print(Rect printArea)
        {
            ChartPrintDialog printDialog = new ChartPrintDialog();
            printDialog.ShowPrintDialog(this, printArea, this.Height, this.Width);
        }

        public void SetChartApperanceDetails(ChartAppearanceSettings chartAppearance)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                Color clr_border = Color.FromArgb(chartAppearance.BorderColor.A, chartAppearance.BorderColor.R, chartAppearance.BorderColor.G, chartAppearance.BorderColor.B);
                this.BorderBrush = new SolidColorBrush(clr_border);

                foreach (ChartColorPalette colorPalette in Enum.GetValues(typeof(ChartColorPalette)))
                {
                    if (colorPalette.ToString() == chartAppearance.ChartColorPalette)
                        this.ColorModel.Palette = colorPalette;
                }

                // Apply Settings in Label Tab
                if (!OlapArea.IsKpiElement)
                {
                    foreach (ChartSeries series in this.Series)
                    {
                        if (chartAppearance.LabelsVisibility || chartAppearance.SymbolsVisibility || chartAppearance.TemplateVisibility)
                            series.AdornmentsInfo.Visible = true;
                        else
                        {
                            series.AdornmentsInfo.Visible = false;
                        }

                        ChartAdornmentInfo cai = series.AdornmentsInfo;
                        ResourceDictionary c_resourcedictionary = new ResourceDictionary();

                        if (chartAppearance.LabelsVisibility)
                        {
                            if (chartAppearance.IsXValues)
                                series.AdornmentsInfo.LabelContentPath = "DataPoint.X";

                            if (chartAppearance.IsYValues)
                                series.AdornmentsInfo.LabelContentPath = "DataPoint.Y";

                            if (chartAppearance.IsSeriesName)
                                series.AdornmentsInfo.LabelContentPath = "Series.Label";
                        }
                        else
                        {
                            series.AdornmentsInfo.LabelContentPath = "Series.Name";
                        }

                        if (chartAppearance.SymbolsVisibility)
                        {
                            c_resourcedictionary.Source = new Uri(c_Key, UriKind.RelativeOrAbsolute);
                            if (chartAppearance.IsCircleSymbol)
                            {
                                symbolTemplate = "Circle";
                                cai.SymbolTemplate = c_resourcedictionary[symbolTemplate] as DataTemplate;
                            }
                            if (chartAppearance.IsRectangleSymbol)
                            {
                                symbolTemplate = "Rectangle";
                                cai.SymbolTemplate = c_resourcedictionary[symbolTemplate] as DataTemplate;
                            }
                            if (chartAppearance.IsTriangleSymbol)
                            {
                                symbolTemplate = "Triangle";
                                cai.SymbolTemplate = c_resourcedictionary[symbolTemplate] as DataTemplate;
                            }
                        }
                        else
                        {
                            symbolTemplate = "EmptySymbol";
                            cai.SymbolTemplate = c_resourcedictionary[symbolTemplate] as DataTemplate;
                        }
                    }
                    if (this.m_area != null)
                    {

                        if (this.m_area.PrimaryAxis != null)
                        {
                            this.m_area.PrimaryAxis.SetValue(ChartArea.ShowGridLinesProperty, chartAppearance.ShowHorizontalGridLines);
                           
                        }
                        if (this.m_area.SecondaryAxis != null)
                        {
                            this.m_area.SecondaryAxis.SetValue(ChartArea.ShowGridLinesProperty, chartAppearance.ShowVerticalGridLines);
                            
                        }
                    }
                }

                //Color clr_GridBackground = Color.FromArgb(chartAppearance.InteriorBackground.A, chartAppearance.InteriorBackground.R, chartAppearance.InteriorBackground.G, chartAppearance.InteriorBackground.B);
                //this.GridBackground = new SolidColorBrush(clr_GridBackground);

                // Color clr_Background = Color.FromArgb(chartAppearance.ChartBackground.A, chartAppearance.ChartBackground.R, chartAppearance.ChartBackground.G, chartAppearance.ChartBackground.B);
                // this.Background = new SolidColorBrush(clr_Background);

                if (this.Legend != null)
                {
                    foreach (ChartDock chartDock in Enum.GetValues(typeof(ChartDock)))
                    {
                        if (chartDock.ToString() == chartAppearance.ChartDockLegendPosition.ToString())
                            Chart.SetDock(this.Legend, chartDock);
                    }
                    this.Legend.Visibility = chartAppearance.LegendVisibility ? Visibility.Visible : Visibility.Collapsed;
                    this.Legend.CheckBoxVisibility = chartAppearance.LegendCheckBoxVisibility ? Visibility.Visible : Visibility.Collapsed;
                }
                this.BorderThickness = new Thickness(chartAppearance.StrokeThickness);
                foreach (ChartTypes chartType in Enum.GetValues(typeof(ChartTypes)))
                {
                    if (chartType.ToString() == chartAppearance.ChartType)
                        this.ChartType = chartType;
                }
                if (this.PrimaryAxis != null)
                {
                    this.PrimaryAxis.LabelFontFamily = new FontFamily(chartAppearance.XAxisFontFace);
                    Color clr_XlabelForeground = Color.FromArgb(chartAppearance.XAxisForeGround.A, chartAppearance.XAxisForeGround.R, chartAppearance.XAxisForeGround.G, chartAppearance.XAxisForeGround.B);
                    this.PrimaryAxis.LabelForeground = new SolidColorBrush(clr_XlabelForeground);
                    if (chartAppearance.XLabelFontWeight == "Bold")
                        this.PrimaryAxis.LabelFontWeight = FontWeights.Bold;
                    if (chartAppearance.XLabelFontWeight == "Normal")
                        this.PrimaryAxis.LabelFontWeight = FontWeights.Normal;
                }
                if (this.SecondaryAxis != null)
                {
                    this.SecondaryAxis.LabelFontFamily = new FontFamily(chartAppearance.YAxisFontFace);
                    Color clr_YlabelForeground = Color.FromArgb(chartAppearance.YAxisForeGround.A, chartAppearance.YAxisForeGround.R, chartAppearance.YAxisForeGround.G, chartAppearance.YAxisForeGround.B);
                    if (clr_YlabelForeground.A != 0 && clr_YlabelForeground.R != 255 && clr_YlabelForeground.G != 255 && clr_YlabelForeground.B != 255)
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(clr_YlabelForeground);
                    if (chartAppearance.YLabelFontWeight == "Bold")
                        this.SecondaryAxis.LabelFontWeight = FontWeights.Bold;
                    if (chartAppearance.YLabelFontWeight == "Normal")
                        this.SecondaryAxis.LabelFontWeight = FontWeights.Normal;
                }
            }
        }

        public void ShowAppearanceDialog()
        {
            try
            {
                Appearance dialog = new Appearance(this.ChartAppearance);
                dialog.FlowDirection = this.FlowDirection;
                SkinStorage.SetVisualStyle(dialog, SkinStorage.GetVisualStyle(this));
                Window parentWindow = OlapChartUtils.GetParentWindow<Window>(this);

                if (parentWindow == null)
                    parentWindow = OlapChartUtils.GetParentWindow<ChromelessWindow>(this);

                if (parentWindow == null)
                    parentWindow = OlapChartUtils.GetParentWindow<RibbonWindow>(this);

                if (parentWindow != null)
                {
                    dialog.Owner = parentWindow;
                }

                if (dialog.ShowDialog() == true)
                {
                    SetChartApperanceDetails(dialog.ChartAppearance);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in Opening Property Dialog" + ex.Message);
            }
        }

        /// <summary>
        /// Verifies the area access.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        /// <exception cref="NotSupportedException"/>
        protected virtual void VerifyAreaAccess(string memberName)
        {
            if (m_area == null)
            {
                throw new NotSupportedException("OlapChart template should be applied before accessing " + memberName);
            }
        }

        #endregion

        #endregion
    }

    public sealed class OlapChartCommands
    {
        private static RoutedUICommand _showAppearanceDlgCmd;

        /// <summary>
        /// Gets the command for show appearance dialog.
        /// </summary>
        /// <value>The show appearance dialog.</value>
        public static RoutedUICommand ShowAppearanceDialog
        {
            get
            {
                _showAppearanceDlgCmd = _showAppearanceDlgCmd ?? new RoutedUICommand("Show Appearance Dialog", "ShowAppearanceDialog", typeof(OlapChartCommands));
                return _showAppearanceDlgCmd;
            }
        }
        private static RoutedUICommand _dataBindCmd;

        /// <summary>
        /// Gets the command for data bind.
        /// </summary>
        /// <value>The data bind.</value>
        public static RoutedUICommand DataBind
        {
            get
            {
                _dataBindCmd = _dataBindCmd ?? new RoutedUICommand("Data Bind", "DataBind", typeof(OlapChartCommands));
                return _dataBindCmd;
            }
        }

        private static RoutedUICommand _setApperanceDetailsCmd;

        /// <summary>
        /// Gets the command to set chart apperance details for OlapChart.
        /// </summary>
        /// <value>The set chart apperance details.</value>
        public static RoutedUICommand SetChartApperanceDetails
        {
            get
            {
                _setApperanceDetailsCmd = _setApperanceDetailsCmd ?? new RoutedUICommand("Set Chart Apperance Details", "SetChartApperanceDetails", typeof(OlapChartCommands));
                return _setApperanceDetailsCmd;
            }
        }

    }
}
