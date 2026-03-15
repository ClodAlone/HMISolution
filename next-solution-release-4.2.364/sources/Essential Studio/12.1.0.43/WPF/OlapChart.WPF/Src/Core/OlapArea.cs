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
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Syncfusion.Olap.Data;
    using Syncfusion.Olap.Engine;
    using Syncfusion.Olap.Manager;
    using Syncfusion.Windows.Chart;
    using Syncfusion.Olap.Reports;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Shapes;
    using Syncfusion.Windows.Chart.Olap.Resources;

    /// <summary>
    /// Represents OLAP Chart area class.
    /// </summary>
    
#if SyncfusionFramework4_0
    [DesignTimeVisible(false)]
#endif
    [TemplatePart(Name = "PART_AreaBorder", Type = typeof(Border))]
    public sealed class OlapArea : ChartArea
    {
        #region Members
        
        private Border m_areaBorder;
        private OlapChartRefreshEventArgs m_refreshingArgs;
        private OlapChartAxis m_refreshingAxis;
        private DependencyProperty[] m_seriesReadonlyProperties;
        private bool m_seriesSealed = true;
        private bool m_shouldClearSeriesCollection = true;
        private bool m_showLoadingIndicator = true;
        private IValueConverter m_toolTipProvider = new SeriesToolTipConverter();
        private static ChartTypes[] m_toolTipSupportingChartTypes;
        private WaitingAdorner m_waitingAdorner;
        private BackgroundWorker m_worker = new BackgroundWorker();
        public static bool IsKpiElement = false;
        private bool m_isDrillUpDown = false;
        private const string m_Key = @"/Syncfusion.OlapChart.WPF;component/Themes/KpiResources.xaml";
        private ContextMenu olapChartContextMenu = new ContextMenu();
        private ChartSeries chartSeries = new ChartSeries();
        #endregion

        #region DependencyProperties

        ///<summary>
        /// Identifies the OlapDataManager dependency property.
        ///</summary>
        public static readonly DependencyProperty OlapDataManagerProperty =
        DependencyProperty.Register("OlapDataManager", typeof(OlapDataManager), typeof(OlapArea), new UIPropertyMetadata(null, OnOlapDataManagerChanged));

        ///<summary>
        /// Identifies the ChartType dependency property.
        ///</summary>
        public static readonly DependencyProperty ChartTypeProperty =
            DependencyProperty.Register("ChartType", typeof(ChartTypes), typeof(OlapArea), new UIPropertyMetadata(ChartTypes.Column, new PropertyChangedCallback(OnChartTypeChanged)));

        internal static readonly DependencyPropertyKey PivotEnginePropertyKey =
           DependencyProperty.RegisterReadOnly("PivotEngine", typeof(PivotEngine), typeof(OlapArea),
           new FrameworkPropertyMetadata(null, OnPivotEnginePropertyChanged, CoercePivotEngine));

        /// <summary>
        /// Identifies the PivotEngine dependency property.
        /// </summary>
        public static readonly DependencyProperty PivotEngineProperty = PivotEnginePropertyKey.DependencyProperty;

        ///<summary>
        /// Toggles the Primary axis expander label scroll panel visibility.
        ///</summary>
        private static readonly DependencyProperty PrimaryAxisLabelVisibilityProperty =
            DependencyProperty.Register("PrimaryAxisLabelVisibility", typeof(Visibility), typeof(OlapArea), new UIPropertyMetadata(Visibility.Visible));        

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the <see cref="OlapArea"/> class.
        /// </summary>
        /// <remarks>
        /// Primary and secondary axes are being created automatically.
        /// </remarks>
        static OlapArea()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(OlapArea), new FrameworkPropertyMetadata(typeof(OlapArea)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapArea"/> class.
        /// </summary>
        /// <remarks>
        /// Primary and secondary axes are being created automatically.
        /// </remarks>
        public OlapArea()
        {
            this.DefaultStyleKey = typeof(OlapArea);
            this.ColorModel.Palette = ChartColorPalette.Default;
            this.AddHandler(OlapArea.MouseRightButtonDownEvent, new MouseButtonEventHandler(this.OlapArea_OnMouseRightClicked));
            //this.ContextMenu = new OlapAreaContextMenu(this);
            //m_worker.DoWork += (object sender, DoWorkEventArgs e)=>
            //{
            //  object[] args = e.Argument as object[];
            //  PivotCellDescriptor member = args[0] as PivotCellDescriptor;
            //  OlapDataManager model = args[1] as OlapDataManager;
            //  model.ToggleExpandableState(member);
            //};

            //m_worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e)=>
            //{
            //  SetValue(PivotEnginePropertyKey, OlapDataManager.PivotEngine);
            //  AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(m_areaBorder);
            //  Adorner[] adorners = adornerLayer.GetAdorners(m_areaBorder);
            //  if (adorners != null)
            //  {
            //    foreach (Adorner adorner in adorners)
            //    {
            //      if (adorner is WaitingAdorner)
            //      {
            //        adornerLayer.Remove(adorner);
            //      }
            //    }
            //  }
            //  if (AfterRefresh != null)
            //  {
            //    AfterRefresh(m_refreshingAxis, m_refreshingArgs);
            //  }
            //  m_refreshingArgs = null;
            //  m_refreshingAxis = null;
            //};
            m_seriesReadonlyProperties = GetSeriesReadonlyProperties();
            m_toolTipSupportingChartTypes = GetToolTipSupportingChartTypes();
            base.DisableIsIndexedForOLAP = true;
        }
        #endregion

        #region Events
        public event OlapRefreshEventHandler AfterRefresh;
        public event OlapRefreshEventHandler BeforeRefresh;
        #endregion

        #region Properties
        OlapChart _ChartControl;
        internal OlapChart ChartControl
        {
            get
            {
                return _ChartControl;
            }
            set
            {
                _ChartControl = value;
            }
        }

        /// <summary>
        /// Gets or sets the ChartType. This is a dependency property.
        /// </summary>
        /// <value>The ChartType.</value>
        public ChartTypes ChartType
        {
            get { return (ChartTypes)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the cube model.
        /// </summary>
        /// <value>The cube model.</value>
        public OlapDataManager OlapDataManager
        {
            get { return (OlapDataManager)GetValue(OlapDataManagerProperty); }
            set
            {
                SetValue(OlapDataManagerProperty, value);
                //if (ChartControl != null)
                //{
                //    if (OlapDataManager.CurrentCellSet != null)
                //    {
                //        PivotEngine pivotEngine =
                //            OlapDataManager.ExecuteOlapTable(OlapDataManager.CurrentCellSet, GridLayout.Normal);
                //        SetValue(PivotEngineProperty, pivotEngine);
                //    }
                //    else
                //    {
                //        DataBind();
                //    }
                //}
            }
        }

        /// <summary>
        /// Gets the value of the PivotEngine. This is a dependency property.
        /// </summary>
        public PivotEngine PivotEngine
        {
            get { return (PivotEngine)GetValue(PivotEngineProperty); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ChartAxis PrimaryAxis
        {
            get
            {
                return base.PrimaryAxis;
            }
            set
            {
                throw new NotSupportedException("Use PrimaryOlapAxis property for OlapArea");
            }
        }

        /// <summary>
        /// Gets the <see cref="ReadOnlyCollection<ChartSeries"/> of <see cref="ChartSeries"/>.
        /// </summary>
        /// <value>The series.</value>
        new public SeriesReadOnlyCollection Series
        {
            get
            {
                return new SeriesReadOnlyCollection(base.Series);
            }
        }

        /// <summary>
        /// Called to coerce pivot engine.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="baseValue">The base value.</param>
        /// <returns></returns>
        private static object CoercePivotEngine(DependencyObject d, object baseValue)
        {
            //PivotEngine engine = baseValue as PivotEngine;
            //if (engine != null)
            //{
            //    PivotEngine cloneEngine = engine;
            //    if (cloneEngine != null)
            //    {
            //        cloneEngine.SummaryPosition = SummaryLayout.None;
            //        cloneEngine.RemoveTotalsElements();
            //        cloneEngine.ClearLevelHeadersArea();
            //    }
            //    return cloneEngine;
            //}
            return baseValue;
        }

        internal Visibility PrimaryAxisLabelVisibility 
        {
            get 
            {
                try
                {
                    return this.ChartControl.PrimaryAxisLabelVisibility;
                }
                catch 
                {
                    return Visibility.Collapsed;
                }
            }
            set 
            {
                if (this.ChartControl != null)
                {
                    this.ChartControl.PrimaryAxisLabelVisibility = value;
                }
            } 
        }

        #endregion

        #region Implementation
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
#if DEBUG
        Stopwatch swatch = new Stopwatch();
#endif

        public void DataBind()
        {
            if (this.OlapDataManager != null)
            {
                ((OlapChartAxis) this.PrimaryAxis).ProcessingLabelsState = true;

                PivotEngine engine = null;

                if (this.OlapDataManager.ItemSource == null)
                {
#if DEBUG                    
#if SyncfusionFramework3_5
                    swatch.Reset();
                    swatch.Start();
#else
                    swatch.Restart();
#endif

#endif
                    CellSet cellSet = this.OlapDataManager.ExecuteCellSet();
#if DEBUG
                    swatch.Stop();
                    Debug.WriteLine("Obtained cell set for given items source in: " + swatch.ElapsedMilliseconds.ToString(CultureInfo.CurrentUICulture) + " Milliseconds");
#if SyncfusionFramework3_5
                    swatch.Reset();
                    swatch.Start();
#else
                    swatch.Restart();
#endif
#endif
                    engine = this.OlapDataManager.ExecuteOlapTable(cellSet, GridLayout.NoSummaries);
#if DEBUG
                    swatch.Stop();
                    Debug.WriteLine("Obtained engine for given cell set in: " + swatch.ElapsedMilliseconds.ToString(CultureInfo.CurrentUICulture) + " Milliseconds");                    
#endif
                }
                else
                {
#if DEBUG
#if SyncfusionFramework3_5
                    swatch.Reset();
                    swatch.Start();
#else
                    swatch.Restart();
#endif
#endif
                    engine = this.OlapDataManager.ExecuteOlapTable(GridLayout.NoSummaries);
#if DEBUG
                    swatch.Stop();
                    Debug.WriteLine("Obtained engine for given cell set in: " + swatch.ElapsedMilliseconds.ToString(CultureInfo.CurrentUICulture) + " Milliseconds");
#endif
                }

                //if (engine != null)
                {
                    this.SetValue(PivotEnginePropertyKey, engine);
                }

                ((OlapChartAxis) this.PrimaryAxis).ProcessingLabelsState = false;
            }
        }

        /// <summary>
        /// Does the dispatcher events in order to invalidate GUI.
        /// </summary>
        private void DoEvents()
        {
            DispatcherFrame f = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
            (SendOrPostCallback)delegate(object arg)
                                    {
                                        DispatcherFrame fr = arg as DispatcherFrame;
                                        if (fr != null) fr.Continue = false;
                                    }, f);
            Dispatcher.PushFrame(f);
        }

        /// <summary>
        /// Gets the series readonly properties array.
        /// </summary>
        /// <returns></returns>
        private static DependencyProperty[] GetSeriesReadonlyProperties()
        {
            return new DependencyProperty[]
                       {
                           ChartSeries.AreaProperty,
                           ChartSeries.ChartTypeProperty,
                           ChartSeries.TypeProperty,
                           ChartSeries.XAxisProperty,
                           ChartSeries.YAxisProperty,
                           ChartSeries.SelectedItemProperty,
                           ChartSeries.LabelProperty,
                           ChartSeries.IsZoomableProperty,
                           ChartSeries.IsRotatedProperty,
                           ChartSeries.IsIndexedProperty,
                           ChartSeries.DataProperty,
                           FrameworkContentElement.DataContextProperty,
                           ChartSeries.DataSourceProperty,
                           FrameworkContentElement.ToolTipProperty,
                           ChartSeries.AdornmentsInfoProperty
                       };
        }

        /// <summary>
        /// Gets the ToolTip supporting chart types.
        /// </summary>
        /// <returns></returns>
        private static ChartTypes[] GetToolTipSupportingChartTypes()
        {
            return new ChartTypes[]
                      {
                        ChartTypes.Bar,
                        ChartTypes.Column,
                        ChartTypes.Line,
                        ChartTypes.RangeColumn,
                        ChartTypes.RotatedSpline,
                        ChartTypes.StepLine,
                        ChartTypes.Scatter,
                        ChartTypes.Spline,
                        ChartTypes.StackingBar,
                        ChartTypes.StackingColumn,
                        ChartTypes.StackingColumn100,
                      };
        }

        /// <summary>
        /// Determines whether ToolTip is supported for the specified chart type.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <returns>
        /// 	<c>true</c> if ToolTip is supported for the specified chart type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsToolTipSupported(ChartTypes chartType)
        {
            return m_toolTipSupportingChartTypes.Contains<ChartTypes>(chartType);
        }

        /// <summary>
        /// Invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_areaBorder = GetTemplateChild("PART_AreaBorder") as Border;
            AddContextMenu();
            // object temp = GetTemplateChild("FrameBorder");
            if (m_areaBorder == null)
            {
                throw new NotSupportedException(
                    "OLAP Chart must have border with name PART_AreaBorder in the root of its template tree");
            }
            m_waitingAdorner = new WaitingAdorner(m_areaBorder);
        }

        private void AddContextMenu()
        {
            MenuItem chartTypeMenuItem = new MenuItem();
            chartTypeMenuItem.Header = SR.GetString(CultureInfo.CurrentUICulture, "OlapChart_Context_Menu");

            foreach (ChartTypes chartType in Enum.GetValues(typeof(ChartTypes)))
            {
                if (chartType == ChartTypes.Area || chartType == ChartTypes.Bar || chartType == ChartTypes.StackingBar || chartType == ChartTypes.Column || chartType == ChartTypes.StackingColumn || chartType == ChartTypes.StackingColumn100 || chartType == ChartTypes.Line || chartType == ChartTypes.Radar
                    || chartType == ChartTypes.StackingArea || chartType == ChartTypes.StepArea || chartType == ChartTypes.SplineArea || chartType == ChartTypes.Spline || chartType == ChartTypes.RotatedSpline || chartType == ChartTypes.StepLine || chartType == ChartTypes.Scatter)
                {
                    MenuItem item = new MenuItem();
                    item.Header = chartType.ToString();
                    item.Click += MenuItem_Click;
                    chartTypeMenuItem.Items.Add(item);
                }
            }

            olapChartContextMenu.Items.Add(chartTypeMenuItem);

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ChartTypes chartType = (ChartTypes)Enum.Parse(typeof(ChartTypes), (sender as MenuItem).Header.ToString());
            chartSeries.Type = chartType;
        }


        private void OlapArea_OnMouseRightClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                try
                {
                    if (e.OriginalSource is Rectangle || e.OriginalSource is Path || e.OriginalSource is Ellipse || e.OriginalSource is Polygon || e.OriginalSource is Line || e.OriginalSource is Polyline)
                    {
                        ChartSegment chartSegment;
                        if (e.OriginalSource.GetType().Name == "Rectangle")
                            chartSegment = (e.OriginalSource as Rectangle).DataContext as ChartSegment;
                        else if (e.OriginalSource.GetType().Name == "Path")
                            chartSegment = (e.OriginalSource as Path).DataContext as ChartSegment;
                        else if (e.OriginalSource.GetType().Name == "Ellipse")
                            chartSegment = (e.OriginalSource as Ellipse).DataContext as ChartSegment;
                        else if (e.OriginalSource.GetType().Name == "Line")
                            chartSegment = (e.OriginalSource as Line).DataContext as ChartSegment;
                        else if (e.OriginalSource.GetType().Name == "Polygon")
                            chartSegment = (e.OriginalSource as Polygon).DataContext as ChartSegment;
                        else
                            chartSegment = (e.OriginalSource as Polyline).DataContext as ChartSegment;

                        if (chartSegment != null && chartSegment.Series != null)
                        {
                            foreach (ChartSeries series in this.Series)
                            {
                                if (series == chartSegment.Series && series.Type != ChartTypes.Pie && series.Type != ChartTypes.Funnel)
                                {
                                    e.Handled = true;
                                    series.ContextMenu = olapChartContextMenu;
                                    series.ContextMenu.IsOpen = true;
                                    chartSeries = series;
                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        /// <summary>
        /// Handles the <see cref="OlapDataManager"/> state changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAxisElementChanged(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, new Action(delegate()
            {
                PivotEngine pivotEngine = this.OlapDataManager.ExecuteOlapTable(OlapDataManager.CurrentCellSet, GridLayout.NoSummaries);
                if (m_isDrillUpDown)
                {
                    m_shouldClearSeriesCollection = false;
                    SetValue(PivotEnginePropertyKey, pivotEngine);
                    m_shouldClearSeriesCollection = true;
                }
                else
                    SetValue(PivotEnginePropertyKey, pivotEngine);
            }));
        }

        /// <summary>
        /// Called when chart type was changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChartTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapArea area = d as OlapArea;
            if (area != null)
            {
                area.m_seriesSealed = false;
                //Iteration over all series in order to change the chart type.
                if (e.NewValue != null)
                {
                    ChartTypes newType = (ChartTypes)Enum.Parse(typeof(ChartTypes), e.NewValue.ToString());
                    ChartTypes oldType = ChartTypes.Column;
                    if (e.OldValue != null)
                        oldType = (ChartTypes)Enum.Parse(typeof(ChartTypes), e.OldValue.ToString());
                    int numberOfSeries = 0;
                    numberOfSeries = area.Series.Count;
                    if (newType == ChartTypes.Pie)
                        numberOfSeries = (area.Series.Count < 1) ? area.Series.Count : 1;

                    for (int i = 0; i < numberOfSeries; i++)
                    {
                        ChartSeries series = area.Series[i];
                        // assigning a new chart type value.
                        series.Type = newType;
                        if (oldType == ChartTypes.Pie)
                        {
                            area.SetToolTip(series);
                            series.ColorEach = ((OlapArea)d).ChartControl.ColorEachSeries;
                            series.AdornmentsInfo.Visible = false;
                            series.AdornmentsInfo.SegmentShowLine = false;
                            series.AdornmentsInfo.LabelContentPath = "DataPoint.Values[0]";
                            series.AdornmentsInfo.LabelTemplate = null;
                            series.AdornmentsInfo.ConnectorTemplate = null;
                        }
                        else if (newType == ChartTypes.Pie)
                        {
                            ResourceDictionary resourceDictionary = new ResourceDictionary();
                            resourceDictionary.Source = new Uri(m_Key, UriKind.RelativeOrAbsolute);
                            series.ColorEach = ((OlapArea)d).ChartControl.ColorEachSeries;
                            series.AdornmentsInfo.Visible = true;
                            series.AdornmentsInfo.SegmentShowLine = true;
                            series.AdornmentsInfo.LabelContentPath = "DataPoint";
                            series.AdornmentsInfo.LabelTemplate = resourceDictionary["PieLabelsTemplate"] as DataTemplate;
                            series.AdornmentsInfo.ConnectorTemplate = resourceDictionary["PieConnectorTemplate"] as DataTemplate;
                        }
                    }
                }

                area.m_seriesSealed = true;
            }
        }

        /// <summary>
        /// Called when cube model was changed.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOlapDataManagerChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapArea area = dependencyObject as OlapArea;
            if (area != null)
            {
                OlapDataManager oldModel = e.OldValue as OlapDataManager;
                OlapDataManager newModel = e.NewValue as OlapDataManager;
                if (oldModel != null)
                {
                    // Unsubscribing from notification from an old axis.
                    oldModel.AxisElementChanged -= area.OnAxisElementChanged;
                    oldModel.ReportChanged -= area.OnReportChanged;
                }
                if (newModel != null)
                {
                    // Subscribing to notification on the new axis.
                    newModel.AxisElementChanged += area.OnAxisElementChanged;
                    newModel.ReportChanged += area.OnReportChanged;
                
                    if (newModel.CurrentCellSet != null)
                    {
                        PivotEngine pivotEngine =
                            newModel.ExecuteOlapTable(newModel.CurrentCellSet, GridLayout.NoSummaries);
                        area.SetValue(PivotEnginePropertyKey, pivotEngine);
                    }
                    else
                    {
                        area.DataBind();
                    }
                }
                
            }
        }


        /// <summary>
        /// Called when pivot engine property changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPivotEnginePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            OlapArea area = d as OlapArea;
            if (area != null)
            {
                // Updating series on PivotEngine property changes.
                PivotEngine engine = e.NewValue as PivotEngine;
#if DEBUG
#if SyncfusionFramework3_5
                area.swatch.Reset();
                area.swatch.Start();
#else
                area.swatch.Restart();
#endif
#endif
                area.UpdateSeries(engine);
#if DEBUG
                area.swatch.Stop();
                Debug.WriteLine("Updated the series with given engine in: " + area.swatch.ElapsedMilliseconds.ToString(CultureInfo.CurrentUICulture) + " Milliseconds");                
#endif

            }

        }

        /// <summary>
        /// Called when primary axis was changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPrimaryAxisChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChartArea area = d as ChartArea;
            if (area != null) area.PrimaryAxis = e.NewValue as OlapChartAxis;
        }

        /// <summary>
        /// Called when primary axis label click occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private static void OnPrimaryAxisLabelClick(OlapChartAxis sender, OlapLabelClickEvenArgs e)
        {
            PivotCellDescriptor member = e.Member;
            if (member != null)
            {
                OlapArea area = sender.Area;
                area.m_shouldClearSeriesCollection = false;
                area.m_isDrillUpDown = true;
                // Expanding/collapsing olap label.
                OlapDataManager model = (area.OlapDataManager as OlapDataManager);
                if (model.CurrentReport.ShowExpanders == true)
                {
                    area.m_refreshingArgs = new OlapChartRefreshEventArgs(area);
                    if (area.BeforeRefresh != null)
                    {
                        area.m_refreshingAxis = sender;
                        area.BeforeRefresh(sender, area.m_refreshingArgs);
                    }
                    area.m_showLoadingIndicator = area.m_refreshingArgs.ShowDefaultLoadingIndicator;
                    area.ToggleCellState(member, model);
                    // area.Chart.SetChartApperanceDetails(area.Chart.ChartAppearance);
                }
                area.m_shouldClearSeriesCollection = true;
            }
        }

        /// <summary>
        /// Invoked whenever the effective value of any dependency property on this <see cref="T:System.Windows.FrameworkElement"/> has been updated. The specific dependency property that changed is reported in the arguments parameter. Overrides <see cref="M:System.Windows.DependencyObject.OnPropertyChanged(System.Windows.DependencyPropertyChangedEventArgs)"/>.
        /// </summary>
        /// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            //Checking whether changed property in PrimaryAxis property.
            if (e.Property == PrimaryAxisProperty)
            {
                OlapChartAxis oldAxis = e.OldValue as OlapChartAxis;
                OlapChartAxis newAxis = e.NewValue as OlapChartAxis;

                // Removing old subscriber.
                if (oldAxis != null)
                {
                    oldAxis.LabelClick -= new OlapMouseEventHandler(OnPrimaryAxisLabelClick);
                }

                // Adding new subscriber.
                if (newAxis != null)
                {
                    newAxis.LabelClick += new OlapMouseEventHandler(OnPrimaryAxisLabelClick);
                }
            }
            if (e.Property == ChartTypeProperty)
            {
                if (e.NewValue != null)
                {
                    RefreshChartType();
                }
            }

            base.OnPropertyChanged(e);
        }
        private bool _isPieChartTypeUpdated;
		private bool showPrimaryAxisGridLines, showSecondaryAxisGridLines;
        private void RefreshChartType()
        {
            if (this.ChartType == ChartTypes.Pie)
            {
                _isPieChartTypeUpdated = true;
                ResourceDictionary resourceDictionary = new ResourceDictionary();
                resourceDictionary.Source = new Uri(m_Key, UriKind.RelativeOrAbsolute);
				showPrimaryAxisGridLines = ChartArea.GetShowGridLines(this.PrimaryAxis);
                showSecondaryAxisGridLines = ChartArea.GetShowGridLines(this.SecondaryAxis);
                ChartArea.SetShowGridLines(this.PrimaryAxis, false);
                ChartArea.SetShowGridLines(this.SecondaryAxis, false);
                if (this.Legend != null)
                {
                    if (this.OlapDataManager.ItemSource == null)
                        this.Legend.ItemTemplate = resourceDictionary["LegendTemplate"] as DataTemplate;
                    else
                        this.Legend.ItemTemplate = resourceDictionary["LegendTemplate2"] as DataTemplate;
                }
                this.SecondaryAxis.LineStroke = new Pen(Brushes.Transparent, 0);
                this.SecondaryAxis.LabelForeground = Brushes.Transparent;
                this.SecondaryAxis.TickLineStroke = new Pen(Brushes.Transparent, 0);
                this.PrimaryAxisLabelVisibility = System.Windows.Visibility.Collapsed;                
            }
            else
            {
				if (_isPieChartTypeUpdated)
                {
                    ChartArea.SetShowGridLines(this.PrimaryAxis, showPrimaryAxisGridLines);
                    ChartArea.SetShowGridLines(this.SecondaryAxis, showSecondaryAxisGridLines);
                }

                if (this.Legend != null)
                    this.Legend.ItemTemplate = null;
                if (this.ChartControl != null && _isPieChartTypeUpdated)
                {
                    string value = Syncfusion.Windows.Shared.SkinStorage.GetVisualStyle(this.ChartControl);
                    if (value == "Blend")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.White);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.DarkGray, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.DarkGray, 1);
                    }
                    else if (value == "Classic")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.Black);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.Black, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.Black, 1);
                    }
                    else if (value == "Metro")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.DodgerBlue);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.DarkSlateGray, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.DarkSlateGray, 1);
                    }
                    else if (value == "Office2003")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.Black);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.SteelBlue, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.SteelBlue, 1);
                    }
                    else if (value == "Office2007Black")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.White);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.DarkGray, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.DarkGray, 1);
                    }
                    else if (value == "Office2007Blue")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.DarkBlue);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.SteelBlue, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.SteelBlue, 1);
                    }
                    else if (value == "Office2007Silver")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.Black);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.DimGray, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.DimGray, 1);
                    }
                    else if (value == "Default")
                    {
                        this.SecondaryAxis.LabelForeground = new SolidColorBrush(Colors.DodgerBlue);
                        this.SecondaryAxis.LineStroke = new Pen(Brushes.DarkSlateGray, 1);
                        this.SecondaryAxis.TickLineStroke = new Pen(Brushes.DarkSlateGray, 1);
                    }
                }
                this.PrimaryAxisLabelVisibility = System.Windows.Visibility.Visible;
                _isPieChartTypeUpdated = false;
            }
        }

        private void OnReportChanged(object sender, ReportChangedEventArgs e)
        {
            if (this.ChartControl != null)
                this.ChartControl.ChartAppearance = e.NewReport.ChartSettings;
        }

        /// <summary>
        /// Sets the ToolTip for series.
        /// </summary>
        /// <param name="series">The series.</param>
        private void SetToolTip(ChartSeries series)
        {
            Binding toolTipBinding = new Binding("Type")
                                         {
                                             RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                                             ConverterParameter = TemplatedParent,
                                             Converter = m_toolTipProvider
                                         };
            series.SetBinding(FrameworkContentElement.ToolTipProperty, toolTipBinding);
        }
        /// <summary>
        /// Expands or collapses the cell.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="model">The model.</param>
        private void ToggleCellState(PivotCellDescriptor member, OlapDataManager model)
        {
            try
            {
                if (m_showLoadingIndicator)
                {
                    this.Cursor = Cursors.Wait;
                }
                //Store current execution loop and start a new processing.
                DoEvents();
                //Toggle cell.
                if (model.CurrentReport.DrillType == Syncfusion.Olap.Reports.DrillType.DrillPosition)
                    model.ToggleExpandableStateOnDrillPosition(member);
                else if (model.ActiveReport != null && !string.IsNullOrEmpty(this.ChartControl.ReportName) && model.UseSharedDataManager)
                {
                    model.ActiveReport = model.Reports[this.ChartControl.ReportName];
                    model.ToggleExpandableState(member);
                    CellSet cellSet = model.ExecuteCellSet();
                    PivotEngine localPivotEngine = model.ExecuteOlapTable(cellSet, GridLayout.NoSummaries);
                    this.ChartControl.PivotEngine = localPivotEngine;
                    this.OlapDataManager.NotifyActiveReportChanged();
                }
                else
                    model.ToggleExpandableState(member);
                DoEvents();
                //Frame pushing logic should be changed to multithreaded one.
                DoEvents();
                DoEvents();
                if (m_showLoadingIndicator)
                    this.Cursor = Cursors.Arrow;
                if (AfterRefresh != null)
                {
                    AfterRefresh(m_refreshingAxis, m_refreshingArgs);
                }
            }
            catch (Exception ex)
            {
                if (m_showLoadingIndicator)
                    this.Cursor = Cursors.Arrow;
                throw ex;
            }
        }
        private bool IsValidCellData(object celldata)
        {
            if (celldata != null)
            {
                if (celldata is string)
                {
                    return false;
                }
                else
                {
                    double cellValue = Convert.ToDouble(celldata);
                    if (cellValue != 0.0)
                        return true;
                    else
                        return false;
                }

            }
            else
                return false;
        }

        internal const int MinSeriesCount = 95;

        /// <summary>
        /// Rebuilds the series.
        /// </summary>
        /// <param name="engine">The engine.</param>
        private void UpdateSeries(PivotEngine engine)
        {
            if (m_shouldClearSeriesCollection)
            {
                base.Series.Clear();
            }
            if (engine != null)
            {
                //Determining values section.
                GridRangeInfo valuesSection = Syncfusion.Olap.Engine.GridRangeInfo.Cells(
                  engine.HeaderSection.Bottom,
                  String.IsNullOrEmpty(engine.RowHeaderSection.Info) ? engine.RowHeaderSection.Right : engine.RowHeaderSection.Right + 1,
                  engine.RowsCount,
                  engine.TableColumns.Count
                  );
                var isKPI = (this.OlapDataManager.ActiveReport == null && !this.OlapDataManager.UseSharedDataManager) ? (this.OlapDataManager.CurrentReport.CategoricalElements.List.Where(m => m.ElementValue is KpiElements || m.ElementValue is VirtualKpiElement).Any() ? true : false)
                    : (this.OlapDataManager.ActiveReport.CategoricalElements.List.Where(n => n.ElementValue is KpiElements || n.ElementValue is VirtualKpiElement).Any() ? true : false);
#if DEBUG
                Stopwatch updateSeriesWatch = new Stopwatch();
#endif
                int numberOfSeries;
                int numberOfSeries1 = (this.ChartControl.OptimizeLargeDataLoading && (valuesSection.Right > MinSeriesCount)) ? MinSeriesCount : valuesSection.Right;
                if (this.ChartType == ChartTypes.Pie)
                    numberOfSeries = (numberOfSeries1 <= valuesSection.Left + 1) ? numberOfSeries1 : valuesSection.Left + 1;
                else
                    numberOfSeries = numberOfSeries1;
                this.BeginInit();
                for (int i = valuesSection.Left; i < numberOfSeries; i++)
                {
                    //Walking thru all columns.
                    ChartSeries series;
                    ChartListData data = new ChartListData();
                    if (m_shouldClearSeriesCollection)
                    {
                        series = new ChartSeries(ChartType)
                        {
                            EnableAnimation = this.ChartControl.EnableSeriesAnimation,
                            ColorEach = this.ChartControl.ColorEachSeries,
                            AnimateOneByOne = this.ChartControl.SeriesAnimateOneByOne,
                            AnimateOption = this.ChartControl.SeriesAnimateOption,
                            AnimationDuration = this.ChartControl.SeriesAnimationDuration,
                            Palette = this.ChartControl.ColorPalette,
                            StrokeThickness = this.ChartControl.SeriesStrokeThickness,
                            EnableEffects = this.ChartControl.EnableSeriesEffects
                        };
                        SetToolTip(series);
                        if (this.ChartType == ChartTypes.Pie && this.OlapDataManager.ItemSource == null)
                        {
                            ResourceDictionary resourceDictionary = new ResourceDictionary();
                            resourceDictionary.Source = new Uri(m_Key, UriKind.RelativeOrAbsolute);
                            series.ColorEach = this.ChartControl.ColorEachSeries;
                            series.AdornmentsInfo.Visible = true;
                            series.AdornmentsInfo.SegmentShowLine = true;
                            series.AdornmentsInfo.LabelContentPath = "DataPoint";
                            series.AdornmentsInfo.LabelTemplate = resourceDictionary["PieLabelsTemplate"] as DataTemplate;
                            series.AdornmentsInfo.ConnectorTemplate = resourceDictionary["PieConnectorTemplate"] as DataTemplate;
                        }
                    }
                    else
                    {
                        series = Series[i - valuesSection.Left];
                    }
                    PivotColumnDescriptor column = null;
                    if (this.ChartType == ChartTypes.Pie && i < 2)
                        column = engine.TableColumns[i];
                    else
                        column = engine.TableColumns[i];
                    int counter = 0;
                    //Walking thru all column cells.
                    string legendText = string.Empty;
#if DEBUG
#if SyncfusionFramework3_5
                    updateSeriesWatch.Reset();
                    updateSeriesWatch.Start();
#else
                    updateSeriesWatch.Restart();
#endif
#endif
                    if (column != null)
                    {
                        foreach (PivotCellDescriptor cell in column.Cells)
                        {
                            if (this.OlapDataManager.ItemSource == null)
                            {
                                if (cell.Tag is Cell)
                                {
                                    if (IsValidCellData((cell.Tag as Cell).Value))
                                    {
                                        if (cell.KpiType == KpiTypeEnum.Kpi_None || cell.KpiType == KpiTypeEnum.Kpi_Value ||
                                            cell.KpiType == KpiTypeEnum.Kpi_Goal)
                                        {
                                            data.Add(new ChartPoint(++counter, Convert.ToDouble((cell.Tag as Cell).Value)) { Item = new DataPointInfoProvider(this.PivotEngine, cell) });
                                        }
                                        else
                                        {
                                            data.Add(new ChartPoint(++counter, 0) { Item = Convert.ToDouble(cell.CellValue) });
                                        }
                                    }
                                    else
                                    {
                                        data.Add(new ChartPoint(++counter, 0));
                                    }
                                }
                                else
                                {
                                    if (cell.Tag != null)
                                    {
                                        legendText += " - " + cell.CellValue;
                                    }
                                }
                            }
                            else
                            {
                                double p = 0;
                                if (cell.CellType == PivotCellDescriptorType.Value)
                                {
                                    if (double.TryParse(cell.CellValue, out p))
                                    {
                                        data.Add(new ChartPoint(counter++, p) { Item = p });// { Item = new DataPointInfoProvider(this.PivotEngine.GetCellData(cell)) });
                                    }
                                    else
                                    {
                                        data.Add(new ChartPoint(++counter, 0));
                                    }
                                }
                                else if (cell.CellType == PivotCellDescriptorType.ColumnHeader)
                                {
                                    legendText += " - " + cell.CellValue;
                                }
                            }
                            if (isKPI)
                                UpdateKPIValues(cell, series);
                        }
                    }
#if DEBUG
                    updateSeriesWatch.Stop();
                    Debug.WriteLine("Data Point and Legend Text calculation in: " + updateSeriesWatch.ElapsedMilliseconds.ToString(CultureInfo.CurrentUICulture) + " Milliseconds.");
                    Debug.WriteLine("Data Point count for the series " + (i).ToString(CultureInfo.CurrentUICulture) + " is " + data.Count.ToString(CultureInfo.CurrentUICulture));
#endif
                    series.Label = legendText.Contains(" - ") ? legendText.Remove(legendText.IndexOf(" - "), 3) : legendText;
                    series.Data = data;

#if DEBUG
#if SyncfusionFramework3_5
                    updateSeriesWatch.Reset();
                    updateSeriesWatch.Start();
#else
                    updateSeriesWatch.Restart();
#endif
#endif
                    if (m_shouldClearSeriesCollection)
                    {
                        //Adding new series to OlapChart area.
                        base.Series.Add(series);
                        series.IsIndexed = true;
                    }
#if DEBUG
                    updateSeriesWatch.Stop();
                    Debug.WriteLine("Series collection added to base class in: " + updateSeriesWatch.ElapsedMilliseconds.ToString(CultureInfo.CurrentUICulture) + " Milliseconds.");
#endif
                }
                this.EndInit();
                //Disabling auto range computations.
                PrimaryAxis.IsAutoSetRange = false;
                if (this.Series.Count > 0)
                {
                    var dataSeries = this.Series.Where(j => j.Data.Count > 0);
                    if (dataSeries.Count() > 0)
                    {
                        PrimaryAxis.Range = new DoubleRange(-0.5, dataSeries.Select(j => j.Data.Count).Max() - 0.5);
                    }
                    else
                    {
                        PrimaryAxis.Range = new DoubleRange(-0.5, -0.5);
                    }
                }
            }
            //// refreshing the chart type
            RefreshChartType();
            m_isDrillUpDown = false;
        }
        #endregion

        #region KPI Implementation
        /// <summary>
        /// Updates the KPI values.
        /// </summary>
        /// <param name="cellDescriptor">The cell descriptor.</param>
        /// <param name="series">The series.</param>
        /// <param name="data">The data.</param>
        public void UpdateKPIValues(PivotCellDescriptor cellDescriptor, ChartSeries series)
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri(m_Key, UriKind.RelativeOrAbsolute);

            if (cellDescriptor.CellType == PivotCellDescriptorType.Value
            && (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status || cellDescriptor.KpiType == KpiTypeEnum.Kpi_Trend))
            {
                IsKpiElement = true;
                series.AdornmentsInfo.Visible = true;
                if (cellDescriptor.KpiType == KpiTypeEnum.Kpi_Status)
                {
                    if (cellDescriptor.KpiGraphicsStyle == KPIGraphics.RoadSigns)
                    {
                        series.AdornmentsInfo.Visible = true;
                        series.AdornmentsInfo.LabelContentPath = "DataPoint";
                        series.AdornmentsInfo.LabelTemplate = resourceDictionary["StatusRoadSigns"] as DataTemplate;
                    }
                    else
                    {
                        series.AdornmentsInfo.Visible = true;
                        series.AdornmentsInfo.LabelContentPath = "DataPoint";
                        series.AdornmentsInfo.LabelTemplate = resourceDictionary["Status"] as DataTemplate;
                    }
                }
                else
                {
                    series.AdornmentsInfo.Visible = true;
                    series.AdornmentsInfo.LabelContentPath = "DataPoint";
                    series.AdornmentsInfo.LabelTemplate = resourceDictionary["Trend"] as DataTemplate;
                }
            }
            else
            {
                IsKpiElement = false;
            }

            series.AdornmentsInfo.AdornmentsPosition = AdornmentsPosition.Top;
            if (this.ChartControl != null)
            {
                switch (this.ChartControl.KpiAlignment)
                {
                    case KpiAlignment.Top:
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case KpiAlignment.Bottom:
                        series.AdornmentsInfo.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                }
            }
        }
        #endregion
        
    }

    public class KPIGraphics
    {
        public const string Cylinder = "Cylinder";
        public const string RoadSigns = "Traffic light";
        public const string StandardArrow = "Standard arrow";
    }
}
