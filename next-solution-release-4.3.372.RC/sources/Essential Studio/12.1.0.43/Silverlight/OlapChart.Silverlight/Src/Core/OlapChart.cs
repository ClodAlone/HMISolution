#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Syncfusion.OlapSilverlight.Reports;
using Syncfusion.Windows.Chart;
using Syncfusion.OlapSilverlight.Manager;
using Syncfusion.OlapSilverlight.Engine;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Syncfusion.Silverlight.Chart.Olap
{
    /// <summary>
    /// Represents the OlapChart control.
    /// </summary>
    [TemplatePart(Name = "PART_OlapArea", Type = typeof(OlapArea))]
    public class OlapChart : Control
    {
        #region Members

        private Popup _seriesToolTipPopup = null;
        private object _previousSender = null;
        private Syncfusion.Windows.Chart.Chart _baseChart = null;
        private OlapDataManager _olapDataManager;
        private bool _hasThemeChangeApplied;
        private bool _showProcessingBar = true;

        #endregion

        #region Delegates

        public delegate void DataRefreshCompleted(object sender, DataRefreshCompletedEventArgs e);
        public delegate void DataRefreshBegin(object sender, DataRefreshBeginEventArgs e);

        #endregion

        #region Initilize/Finalize

        /// <summary>
        /// Initializes a new instance of the <see cref="OlapChart"/> class.
        /// </summary>
        public OlapChart()
        {
            DefaultStyleKey = typeof(OlapChart);
            this.Loaded += new RoutedEventHandler(OlapChart_Loaded);
        }

        void OlapChart_Loaded(object sender, RoutedEventArgs e)
        {
            if (this._hasThemeChangeApplied == true)
            {
                ApplyExpanderStyle(this.ChartVisualStyle.ToString(), this);
            }
            if (this.OlapDataManager != null)
            {
                if (this.OlapDataManager.ItemSource != null)
                {
                    //// Data Refresh completed event args.
                    this.RaiseDataRefreshCompleted(new DataRefreshCompletedEventArgs());
                }
            }
        }

        #endregion

        #region Dependency Properties
        
        /// <summary>
        /// Service Uri Dependency Property
        /// </summary>
        public static readonly DependencyProperty ServiceUriProperty =
            DependencyProperty.Register("ServiceUri", typeof(Uri), typeof(OlapChart), new PropertyMetadata(null,
                (dependencyObject, args) =>
                {
                    OlapChart olapChart = dependencyObject as OlapChart;
                    if (olapChart != null)
                    {
                        System.ServiceModel.Channels.Binding customBinding = new System.ServiceModel.Channels.CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement { MaxReceivedMessageSize = 2147483647 });
                        EndpointAddress address = new EndpointAddress((Uri)args.NewValue);
                        ChannelFactory<IOlapDataProvider> channel = new ChannelFactory<IOlapDataProvider>(customBinding, address);
                        olapChart.DataProvider = channel.CreateChannel();
                        if (olapChart.DefaultReport != null)
                        {
                            OlapDataManager olapDataManager = new OlapDataManager();
                            olapDataManager.DataProvider = olapChart.DataProvider;
                            olapDataManager.SetCurrentReport(olapChart.DefaultReport);
                            olapChart.OlapDataManager = olapDataManager;
                            olapChart.DataBind();
                        }
                    }
                }));

        /// <summary>
        /// CurrentReport Dependency Property
        /// </summary>
        public static readonly DependencyProperty CurrentReportProperty =
            DependencyProperty.Register("CurrentReport", typeof(OlapReport), typeof(OlapChart), new PropertyMetadata(null,
                (dependencyObject,args)=>
                {
                    OlapChart olapChart = dependencyObject as OlapChart;
                    if (olapChart != null && olapChart.DataProvider != null)
                    {
                            OlapDataManager olapDataManager = new OlapDataManager();
                            olapDataManager.DataProvider = olapChart.DataProvider;
                            olapDataManager.SetCurrentReport((OlapReport)args.NewValue);
                            olapChart.OlapDataManager = olapDataManager;
                            olapChart.DataBind();
                    }
                    else
                        olapChart.DefaultReport = (OlapReport)args.NewValue;
                }));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.LegendVisibility"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LegendVisibilityProperty =
            DependencyProperty.Register("LegendVisibility", typeof(Visibility), typeof(OlapChart), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.LegendIconVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendIconVisibilityProperty =
            DependencyProperty.Register("LegendIconVisibility", typeof(Visibility), typeof(OlapChart), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.LegendCheckboxVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendCheckboxVisibilityProperty =
            DependencyProperty.Register("LegendCheckboxVisibility", typeof(Visibility), typeof(OlapChart), new PropertyMetadata(Visibility.Collapsed));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.LegendDockPosition"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LegendDockPositionProperty =
            DependencyProperty.Register("LegendDockPosition", typeof(ChartDock), typeof(OlapChart), new PropertyMetadata(ChartDock.Top));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.ChartType"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty ChartTypeProperty =
            DependencyProperty.Register("ChartType", typeof(Syncfusion.Windows.Chart.ChartTypes), typeof(OlapChart), new PropertyMetadata(Syncfusion.Windows.Chart.ChartTypes.Column));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.EdgeLabelsDrawingMode"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty EdgeLabelsDrawingModeProperty =
            DependencyProperty.Register("EdgeLabelsDrawingMode", typeof(EdgeLabelsDrawingMode), typeof(OlapArea), new PropertyMetadata(EdgeLabelsDrawingMode.Center));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.LabelFormat"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LabelFormatProperty =
            DependencyProperty.Register("LabelFormat", typeof(string), typeof(OlapChart), new PropertyMetadata(string.Empty));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.OlapChartAdornmentInfo"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty OlapChartAdornmentInfoProperty =
            DependencyProperty.Register("OlapChartAdornmentInfo", typeof(ChartAdornmentInfo), typeof(OlapChart), new PropertyMetadata(new ChartAdornmentInfo()));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.OlapChartType"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register("OlapChartType", typeof(OlapChartTypes), typeof(OlapChart), new PropertyMetadata(OlapChartTypes.Column, OnChartTypeChangedCallBack));

        // Using a DependencyProperty as the backing store for ShowSeriesToolTip.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSeriesToolTipProperty =
            DependencyProperty.Register("ShowSeriesToolTip", typeof(bool), typeof(OlapChart), new PropertyMetadata(true,
                (dependencyObject, eventArgs) =>
                {
                    var olapChart = dependencyObject as OlapChart;

                    if (olapChart != null)
                    {
                        bool newValue = (bool)eventArgs.NewValue;

                        if (newValue)
                        {
                            olapChart.InitializeTooltip();
                        }
                        else
                        {
                            olapChart.DisposeTooltip();
                        }
                    }
                }));

        public static readonly DependencyProperty ShowHeaderToolTipProperty =
            DependencyProperty.Register("ShowHeaderToolTip", typeof(bool), typeof(OlapChart), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Syncfusion.Silverlight.Chart.Olap.OlapChart.OlapChartColorPalette"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty OlapChartColorPaletteProperty =
            DependencyProperty.Register("OlapChartColorPalette", typeof(ChartColorPalette), typeof(OlapChart), new PropertyMetadata(ChartColorPalette.Default,
                (d, args) =>
                {
                    OlapArea area = (d as OlapChart).OlapArea;
                    if (area != null && area.Legends != null && area.Legends.Area != null)
                    {
                        /*TODO:
                         * Below lines are workaround because, Chart series & Chart legends are not getting changed with specified color palette, even we gave two way binding in Generic.xaml
                         * as (<syncfusion:ChartStyleModel Palette="{Binding RelativeSource={RelativeSource TemplatedParent}, Path=OlapChartColorPalette, Mode=TwoWay}" />) */
                        area.Legends.Area.ColorModel.Palette = area.ChartControl.OlapChartColorPalette;
                        area.Legends.InvalidateMeasure();
                        area.BeginInit();
                        area.EndInit();
                    }
                }));

        // Using a DependencyProperty as the backing store for ChartVisualStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChartVisualStyleProperty =
            DependencyProperty.Register("ChartVisualStyle", typeof(OlapChartVisualStyle), typeof(OlapChart), new PropertyMetadata(OlapChartVisualStyle.Default, OnChartVisualStyleChangedCallback));

        // Using a DependencyProperty as the backing store for PrimaryAxisLabelRoatationAngle.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty PrimaryAxisLabelRoatationAngleProperty =
            DependencyProperty.Register("PrimaryAxisLabelRoatationAngle", typeof(double), typeof(OlapChart), new PropertyMetadata(0d));

        // Using a DependencyProperty as the backing store for ExpanderStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpanderStyleProperty =
            DependencyProperty.Register("ExpanderStyle", typeof(Style), typeof(OlapChart), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for PrimaryAxisStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrimaryAxisStyleProperty =
            DependencyProperty.Register("PrimaryAxisStyle", typeof(ChartCommonStyles), typeof(OlapChart), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for SecondaryAxisStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondaryAxisStyleProperty =
            DependencyProperty.Register("SecondaryAxisStyle", typeof(ChartCommonStyles), typeof(OlapChart), new PropertyMetadata(null));

        // Using a DependencyProperty as the backing store for LegendStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LegendStyleProperty =
            DependencyProperty.Register("LegendStyle", typeof(ChartCommonStyles), typeof(OlapChart), new PropertyMetadata(null));

        #endregion

        #region Internal Properties

        /// <summary>
        /// Chart processing progress bar
        /// </summary>
        internal Popup ChartProgressBarPopup { get; set; }

        /// <summary>
        /// Gets or sets the chart progress bar.
        /// </summary>
        /// <value>The chart progress bar.</value>
        internal ProgressBar ChartProgressBar { get; set; }

        /// <summary>
        /// Gets or sets the current MDX query.
        /// </summary>
        /// <value>The current MDX query.</value>
        internal string CurrentMDXQuery { get; set; }

        /// <summary>
        /// Gets or Sets the Temporary Report
        /// </summary>
        internal OlapReport DefaultReport { get; set; }

        /// <summary>
        /// Gets or Sets the DataProvider
        /// </summary>
        internal IOlapDataProvider DataProvider { get; set; }

        /// <summary>
        /// Gets or sets the expander style.
        /// </summary>
        /// <value>The expander style.</value>
        public Style ExpanderStyle
        {
            get { return (Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the chart settings.
        /// </summary>
        /// <value>The chart settings.</value>
        public ChartAppearanceSettings ChartSettings
        {
            get
            {
                return GetChartAppearanceSettings();
            }
            set
            {
                SetChartAppearanceSettings(value);
            }
        }

        /// <summary>
        /// Gets or sets the internal olap area.
        /// </summary>
        /// <value>The internal olap area.</value>        
        [Browsable(false)]
        public OlapArea OlapArea { get; set; }

        /// <summary>
        /// Gets or sets the legend dock position.
        /// </summary>
        /// <value>The legend dock position.</value>
        [Category("Legend")]
        public ChartDock LegendDockPosition
        {
            get { return (ChartDock)GetValue(LegendDockPositionProperty); }
            set { SetValue(LegendDockPositionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the legend icon visibility.
        /// </summary>
        /// <value>The legend icon visibility.</value>
        [Category("Legend")]
        public Visibility LegendIconVisibility
        {
            get { return (Visibility)GetValue(LegendIconVisibilityProperty); }
            set { SetValue(LegendIconVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the legend checkbox visibility.
        /// </summary>
        /// <value>The legend checkbox visibility.</value>
        [Category("Legend")]
        public Visibility LegendCheckboxVisibility
        {
            get { return (Visibility)GetValue(LegendCheckboxVisibilityProperty); }
            set { SetValue(LegendCheckboxVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the legend visibility.
        /// </summary>
        /// <value>The legend visibility.</value>
        [Category("Legend")]
        public Visibility LegendVisibility
        {
            get { return (Visibility)GetValue(LegendVisibilityProperty); }
            set { SetValue(LegendVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>The type of the chart.</value>
        [Obsolete("Use the OlapChartType property instead of this.")]
        [Browsable(false)]
        public Syncfusion.Windows.Chart.ChartTypes ChartType
        {
            get { return (Syncfusion.Windows.Chart.ChartTypes)GetValue(ChartTypeProperty); }
            set { SetValue(ChartTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the olap chart type.
        /// </summary>
        /// <value>The chart type.</value>
        [Category("Appearance")]
        public OlapChartTypes OlapChartType
        {
            get { return (OlapChartTypes)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the olap chart color palette.
        /// </summary>
        /// <value>The olap chart color palette.</value>
        [Category("Appearance")]
        public ChartColorPalette OlapChartColorPalette
        {
            get { return (ChartColorPalette)GetValue(OlapChartColorPaletteProperty); }
            set { SetValue(OlapChartColorPaletteProperty, value); }
        }

        /// <summary>
        /// Gets or sets the edge labels drawing mode.
        /// </summary>
        /// <value>The edge labels drawing mode.</value>
        [Category("Appearance")]
        public EdgeLabelsDrawingMode EdgeLabelsDrawingMode
        {
            get { return (EdgeLabelsDrawingMode)GetValue(EdgeLabelsDrawingModeProperty); }
            set { SetValue(EdgeLabelsDrawingModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the label format for the secondary axis.
        /// </summary>
        /// <value>The label format.</value>
        [Category("Axis")]
        public string LabelFormat
        {
            get { return (string)GetValue(LabelFormatProperty); }
            set { SetValue(LabelFormatProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the Service Path
        /// </summary>
        public Uri ServiceUri
        {
            get { return (Uri)GetValue(ServiceUriProperty); }
            set { SetValue(ServiceUriProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the CurrentReport
        /// </summary>
        public OlapReport CurrentReport
        {
            get { return (OlapReport)GetValue(CurrentReportProperty); }
            set { SetValue(CurrentReportProperty, value); }
        }

        /// <summary>
        /// Gets or sets OlapDataManager.
        /// </summary>
        /// <value>The olap data manager.</value>
        /// <remarks></remarks>        
        [Browsable(false)]
        public OlapDataManager OlapDataManager
        {
            get { return (OlapDataManager)GetValue(OlapDataManagerProperty); }
            set { SetValue(OlapDataManagerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OlapDataManager.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OlapDataManagerProperty =
            DependencyProperty.Register("OlapDataManager", typeof(OlapDataManager), typeof(OlapChart), new PropertyMetadata(null, OnOlapDataManagerChanged));

        private static void OnOlapDataManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool baseChartFound = false;
            UIElement currentUIElement = null;
            OlapChart chart = d as OlapChart;
            if (chart != null)
            {
                if (chart.OlapDataManager != null)
                {
                    chart.OlapDataManager.CellSetChanged -= chart.OlapDataManager_CellSetChanged;
                    chart.OlapDataManager.CellSetChanged += chart.OlapDataManager_CellSetChanged;
                    chart.OlapDataManager.ReportChanged -= chart.OlapDataManager_ReportChanged;
                    chart.OlapDataManager.ReportChanged += chart.OlapDataManager_ReportChanged;
                    if (chart.OlapDataManager.CurrentCellSet != null)
                    {
                        chart.OlapDataManager.ExecuteOlapTable(GridLayout.NoSummaries);
                        chart.DataBind(chart.OlapDataManager.PivotEngine);
                        chart.GetChartControl(ref baseChartFound, ref currentUIElement);
                        if (!chart._hasThemeChangeApplied)
                        {
                            chart._baseChart.ChartVisualStyle = (ChartStyles)Enum.Parse(typeof(ChartStyles),chart.ChartVisualStyle.ToString(),true);
                            chart._hasThemeChangeApplied = true;
                        }
                    }
                    else
                    {
                        chart.DataBind();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show processing bar].
        /// </summary>
        /// <value><c>true</c> if [show processing bar]; otherwise, <c>false</c>.</value>
        public bool ShowProcessingBar
        {
            get
            {
                return _showProcessingBar;
            }
            set
            {
                _showProcessingBar = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is processing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is processing; otherwise, <c>false</c>.
        /// </value>        
        [Browsable(false)]
        public bool IsProcessing
        {
            get
            {
                // return _IsProcessing;
                return false;
            }
            internal set
            {
                if (ChartProgressBarPopup != null)
                {
                    try
                    {
                        //// if its processing, then display the processing dialog
                        if (ShowProcessingBar)
                        {
                            //// if its processing and showProcessingBar is true, then display the processing dialog
                            this.ChartProgressBarPopup.IsOpen = value;
                            if (value == true)
                            {
                                Storyboard animation1 = ChartProgressBarPopup.Resources["Storyboard1"] as Storyboard;
                                animation1.Begin();
                                Storyboard animation2 = ChartProgressBarPopup.Resources["Storyboard2"] as Storyboard;
                                animation2.Begin();
                            }
                        }

                        //if (this.ChartProgressBar != null)
                        //    this.ChartProgressBar.IsIndeterminate = value;

                        if (value)
                        {
                            if (this.OlapArea != null)
                            {
                                this.IsEnabled = false;
                            }
                        }
                        else
                        {
                            if (this.OlapArea != null)
                            {
                                this.IsEnabled = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the olap chart adornment info.
        /// </summary>
        /// <value>The olap chart adornment info.</value>        
        [Category("Series")]
        public ChartAdornmentInfo OlapChartAdornmentInfo
        {
            get { return (ChartAdornmentInfo)GetValue(OlapChartAdornmentInfoProperty); }
            set { SetValue(OlapChartAdornmentInfoProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show series tool tip].
        /// </summary>
        /// <value><c>true</c> if [show series tool tip]; otherwise, <c>false</c>.</value>
        [Category("Series")]
        public bool ShowSeriesToolTip
        {
            get { return (bool)GetValue(ShowSeriesToolTipProperty); }
            set { SetValue(ShowSeriesToolTipProperty, value); }
        }


        public bool ShowHeaderToolTip
        {
            get { return (bool)GetValue(ShowHeaderToolTipProperty); }
            set { SetValue(ShowHeaderToolTipProperty, value); }
        }

        /// <summary>
        /// Gets or sets the chart visual style.
        /// </summary>
        /// <value>The chart visual style.</value>
        [Category("Appearance")]
        public OlapChartVisualStyle ChartVisualStyle
        {
            get { return (OlapChartVisualStyle)GetValue(ChartVisualStyleProperty); }
            set { SetValue(ChartVisualStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the primary axis label rotation angle.
        /// </summary>
        /// <value>The primary axis label rotation angle.</value>
        public double PrimaryAxisLabelRoatationAngle
        {
            get { return (double)GetValue(PrimaryAxisLabelRoatationAngleProperty); }
            set { SetValue(PrimaryAxisLabelRoatationAngleProperty, value); }
        }

        [Category("Axis")]
        public ChartCommonStyles PrimaryAxisStyle
        {
            get { return (ChartCommonStyles)GetValue(PrimaryAxisStyleProperty); }
            set { SetValue(PrimaryAxisStyleProperty, value); }
        }

        [Category("Axis")]
        public ChartCommonStyles SecondaryAxisStyle
        {
            get { return (ChartCommonStyles)GetValue(SecondaryAxisStyleProperty); }
            set { SetValue(SecondaryAxisStyleProperty, value); }
        }

        [Category("Legend")]
        public ChartCommonStyles LegendStyle
        {
            get { return (ChartCommonStyles)GetValue(LegendStyleProperty); }
            set { SetValue(LegendStyleProperty, value); }
        }

        #endregion

        #region Events

        public event DataRefreshCompleted OnDataRefreshCompleted;
        public event DataRefreshBegin OnDataRefreshBegin;

        #endregion

        #region Overrides

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Retrieving OlapArea templated child.
            this.OlapArea = GetTemplateChild("PART_OlapArea") as OlapArea;

            //// Getting the OlapChart processing progress bar popup
            this.ChartProgressBarPopup = GetTemplateChild("PART_OlapChartProgressbarPopup") as Popup;

            //// Getting the Progress bar
            this.ChartProgressBar = GetTemplateChild("PART_OlapChartProgressbar") as ProgressBar;

            if (this.OlapArea != null)
            {
                this.OlapArea.ChartControl = this;
            }
        }

        #endregion

        #region CallBacks

        /// <summary>
        /// Called when [chart type changed call back].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChartTypeChangedCallBack(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapChart olapChart = dependencyObject as OlapChart;

            if (olapChart != null)
            {
                olapChart.ChartType = MapWithBaseChartType((OlapChartTypes)e.NewValue);

                if (olapChart.OlapArea != null)
                {
                    if ((olapChart.OlapDataManager != null && olapChart.OlapDataManager.CurrentCellSet != null) ||
                        olapChart.OlapDataManager.ItemSource != null)
                    {
                        olapChart.OlapArea.DataBind(olapChart.OlapDataManager.ExecuteOlapTable(GridLayout.NoSummaries));
                    }
                }
            }
        }

        /// <summary>
        /// Called when [chart visual style changed callback].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnChartVisualStyleChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            OlapChart olapChart = dependencyObject as OlapChart;
            ResourceDictionary dict = new ResourceDictionary();

            if (olapChart != null)
            {
                if (!DesignerProperties.GetIsInDesignMode(olapChart))
                {
                    if (olapChart._baseChart != null)
                    {
                        switch ((OlapChartVisualStyle)e.NewValue)
                        {
                            case OlapChartVisualStyle.Default:
                            case OlapChartVisualStyle.Blend:
                            case OlapChartVisualStyle.Office2007Blue:
                            case OlapChartVisualStyle.Office2007Black:
                            case OlapChartVisualStyle.Office2007Silver:
                                olapChart._baseChart.ChartVisualStyle = (ChartStyles)Enum.Parse(typeof(ChartStyles), e.NewValue.ToString(), true);
                                break;
                            case OlapChartVisualStyle.Metro:
                                {
                                    dict.Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/Office2010Theme.xaml", UriKind.RelativeOrAbsolute);
                                    olapChart._baseChart.Style = dict["MetroStyle"] as Style;
                                    olapChart.OlapChartColorPalette = ChartColorPalette.Metro;
                                }
                                break;
                            case OlapChartVisualStyle.Office2010Blue:
                            case OlapChartVisualStyle.Office2010Black:
                            case OlapChartVisualStyle.Office2010Silver:
                                {
                                    dict.Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/Office2010Theme.xaml", UriKind.RelativeOrAbsolute);
                                    olapChart._baseChart.Style = dict[e.NewValue.ToString()+"Style"] as Style;
                                    olapChart.OlapChartColorPalette = ChartColorPalette.Default;
                                }
                                break;
                            case OlapChartVisualStyle.Transparent:
                                {
                                    dict.Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/Office2010Theme.xaml", UriKind.RelativeOrAbsolute);
                                    olapChart._baseChart.Style = dict["TransparentStyle"] as Style;
                                    olapChart.OlapChartColorPalette = ChartColorPalette.Default;
                                }
                                break;
                            default:
                                break;
                        }
                        olapChart._hasThemeChangeApplied = true;
                        olapChart.ApplyExpanderStyle(e.NewValue.ToString(), olapChart);
                    }
                    else
                    {
                        olapChart._hasThemeChangeApplied = false;
                        olapChart.ApplyExpanderStyle(e.NewValue.ToString(), olapChart);
                    }
                }
                else
                {
                    if (olapChart._baseChart != null)
                    {
                        var currentStyle = (ChartStyles)e.NewValue;
                        olapChart._baseChart.ChartVisualStyle = currentStyle;
                        olapChart.ApplyExpanderStyle(currentStyle.ToString(), olapChart);
                    }
                }
            }
        }
        private void ApplyExpanderStyle(string selectedStyle, OlapChart olapChart)
        {
            ResourceDictionary resource = new ResourceDictionary
            {
                Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/generic.xaml", UriKind.RelativeOrAbsolute)
            };
            if (selectedStyle.Contains("Blend"))
            {
                olapChart.ExpanderStyle = resource["Blend.ExpanderStyle"] as Style;

            }
            else if (selectedStyle.Contains("Screen"))
            {
                olapChart.ExpanderStyle = resource["Transparent.ExpanderStyle"] as Style;
            }
            else if (selectedStyle.Contains("Office2007Silver"))
            {
                olapChart.ExpanderStyle = resource["Office2007Silver.ExpanderStyle"] as Style;
            }
            else if (selectedStyle.Contains("Office2007Black"))
            {
                olapChart.ExpanderStyle = resource["Office2007Black.ExpanderStyle"] as Style;
            }
            else if (selectedStyle.Contains("Office2007Blue"))
            {
                olapChart.ExpanderStyle = resource["Office2007Blue.ExpanderStyle"] as Style;

            }
            else if (selectedStyle.Contains("VS2010"))
            {
                olapChart.ExpanderStyle = resource["VS2010.ExpanderStyle"] as Style;
            }
            else if (selectedStyle.Contains("Metro"))
            {
                olapChart.ExpanderStyle = resource["Metro.ExpanderStyle"] as Style;
            }
            else if (selectedStyle.Contains("Transparent"))
                olapChart.ExpanderStyle = resource["Transparent.ExpanderStyle"] as Style;
            else if (selectedStyle.Contains("Office2010"))
                olapChart.ExpanderStyle = resource["Office2010.ExpanderStyle"] as Style;
            else
            {
                olapChart.ExpanderStyle = resource["Classic.ExpanderStyle"] as Style;
            }
        }
        #endregion

        #region Raise Events

        internal void RaiseDataRefreshCompleted(DataRefreshCompletedEventArgs renderArgs)
        {
            if (OnDataRefreshCompleted != null)
                OnDataRefreshCompleted(this, renderArgs);
        }

        internal void RaiseDataRefreshBegin(DataRefreshBeginEventArgs renderArgs)
        {
            if (OnDataRefreshBegin != null)
            {
                OnDataRefreshBegin(this, renderArgs);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Binds the data to the OlapChart control.
        /// </summary>
        public void DataBind()
        {
            var dataRefreshEventArgs = new DataRefreshBeginEventArgs(RefreshType.DataBind);
            this.RaiseDataRefreshBegin(dataRefreshEventArgs);

            bool baseChartFound = false;
            UIElement currentUIElement = null;
            ResourceDictionary dict = new ResourceDictionary();
            if (OlapDataManager != null && OlapDataManager.ItemSource != null)
            {
                this.UpdateLayout();

                PivotEngine customCollectionEngine = this.OlapDataManager.ExecuteOlapTable(GridLayout.NoSummaries); //TableBuilder.BuildEngineFromIQueryable(IQueryableSource, SortType.Ascending, pivotDataElements.ColumnItems.ToArray(), pivotDataElements.SeriesItems.ToArray(), pivotDataElements.Summaries.ToArray(), pivotDataElements.IsRowSummary, GridLayout.Normal);

                if (this.OlapArea != null)
                {
                    this.OlapArea.DataBind(customCollectionEngine);
                    GetChartControl(ref baseChartFound, ref currentUIElement);
                    if (!this._hasThemeChangeApplied)
                    {
                        ApplyVisualStyle(dict);
                        this._hasThemeChangeApplied = true;
                    }
                }
            }
            else
            {
                if (this.OlapDataManager != null)
                {
                    if (!this.OlapDataManager.IsProcessing)
                    {
                        this.UpdateLayout();
                        this.IsProcessing = true;

                        this.OlapDataManager.ExecuteCellSet();
                    }

                    this.GetChartControl(ref baseChartFound, ref currentUIElement);

                    if (!this._hasThemeChangeApplied)
                    {
                        ApplyVisualStyle(dict);
                        this._hasThemeChangeApplied = true;
                    }
                }
            }
        }

        private void ApplyVisualStyle(ResourceDictionary dict)
        {
            if (_baseChart != null)
            {
                switch (this.ChartVisualStyle)
                {
                    case OlapChartVisualStyle.Default:
                    case OlapChartVisualStyle.Blend:
                    case OlapChartVisualStyle.Office2007Blue:
                    case OlapChartVisualStyle.Office2007Black:
                    case OlapChartVisualStyle.Office2007Silver:
                        this._baseChart.ChartVisualStyle = (ChartStyles)Enum.Parse(typeof(ChartStyles), this.ChartVisualStyle.ToString(), true);
                        break;
                    case OlapChartVisualStyle.Metro:
                        {
                            dict.Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/Office2010Theme.xaml", UriKind.RelativeOrAbsolute);
                            this._baseChart.Style = dict["MetroStyle"] as Style;
                            this.OlapChartColorPalette = ChartColorPalette.Metro;
                        }
                        break;
                    case OlapChartVisualStyle.Office2010Blue:
                    case OlapChartVisualStyle.Office2010Black:
                    case OlapChartVisualStyle.Office2010Silver:
                        {
                            dict.Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/Office2010Theme.xaml", UriKind.RelativeOrAbsolute);
                            this._baseChart.Style = dict[this.ChartVisualStyle.ToString() + "Style"] as Style;
                            this.OlapChartColorPalette = ChartColorPalette.Default;
                        }
                        break;

                    case OlapChartVisualStyle.Transparent:
                        {
                            dict.Source = new Uri("/Syncfusion.OlapChart.Silverlight;component/Themes/Office2010Theme.xaml", UriKind.RelativeOrAbsolute);
                            this._baseChart.Style = dict["TransparentStyle"] as Style;
                            this.OlapChartColorPalette = ChartColorPalette.Default;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the chart control.
        /// </summary>
        /// <param name="baseChartFound">if set to <c>true</c> [base chart found].</param>
        /// <param name="currentUIElement">The current UI element.</param>
        private void GetChartControl(ref bool baseChartFound, ref UIElement currentUIElement)
        {
            if (this.OlapArea == null)
                this.ApplyTemplate();

            if (this.OlapArea != null)
            {
                currentUIElement = this.OlapArea as UIElement;

                if (currentUIElement != null)
                {
                    do
                    {
                        DependencyObject temp = System.Windows.Media.VisualTreeHelper.GetParent(currentUIElement);
                        if (temp == null)
                            currentUIElement = null;
                        else if (temp is Syncfusion.Windows.Chart.Chart)
                        {
                            baseChartFound = true;
                            _baseChart = (temp as Syncfusion.Windows.Chart.Chart);
                        }
                        else
                        {
                            currentUIElement = temp as UIElement;
                        }

                    } while (currentUIElement != null && !baseChartFound);
                }
            }
        }

        #endregion

        #region Helper methods

        private ChartAppearanceSettings GetChartAppearanceSettings()
        {
            ChartAppearanceSettings chartSettings = new ChartAppearanceSettings();
            chartSettings.ChartColorPalette = this.OlapChartColorPalette.ToString();
            chartSettings.ChartType = this.OlapChartType.ToString();
            if (this.LegendCheckboxVisibility == System.Windows.Visibility.Visible)
                chartSettings.ShowLegendCheckbox = true;
            else
                chartSettings.ShowLegendCheckbox = false;
            if (this.LegendIconVisibility == System.Windows.Visibility.Visible)
                chartSettings.ShowLegendIcon = true;
            else
                chartSettings.ShowLegendIcon = false;
            chartSettings.LegendDockPosition = this.LegendDockPosition.ToString();
            if (this.LegendVisibility == System.Windows.Visibility.Visible)
                chartSettings.ShowLegend = true;
            else
                chartSettings.ShowLegend = false;
            chartSettings.ShowSeriesTooltip = this.ShowSeriesToolTip;
            chartSettings.ShowProcessingBar = this.ShowProcessingBar;
            chartSettings.ChartVisualStyle = this.ChartVisualStyle.ToString();
            return chartSettings;
        }

        private void SetChartAppearanceSettings(ChartAppearanceSettings chartSettings)
        {
            if (!string.IsNullOrEmpty(chartSettings.ChartColorPalette))
                this.OlapChartColorPalette = (ChartColorPalette)Enum.Parse(typeof(ChartColorPalette), chartSettings.ChartColorPalette, true);
            this.LegendCheckboxVisibility= chartSettings.ShowLegendCheckbox ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.LegendIconVisibility = chartSettings.ShowLegendIcon ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            if (!string.IsNullOrEmpty(chartSettings.LegendDockPosition))
                this.LegendDockPosition = (ChartDock)Enum.Parse(typeof(ChartDock), chartSettings.LegendDockPosition, true);
            this.LegendVisibility = chartSettings.ShowLegend ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            this.ShowSeriesToolTip = chartSettings.ShowSeriesTooltip;
            this.ShowProcessingBar = chartSettings.ShowProcessingBar;
            if (!string.IsNullOrEmpty(chartSettings.ChartType))
                this.OlapChartType = (OlapChartTypes)Enum.Parse(typeof(OlapChartTypes), chartSettings.ChartType, true);
            if (!string.IsNullOrEmpty(chartSettings.ChartVisualStyle))
                this.ChartVisualStyle = (OlapChartVisualStyle)Enum.Parse(typeof(OlapChartVisualStyle), chartSettings.ChartVisualStyle, true);
        }

        /// <summary>
        /// Maps the olap chart type with the base chart's chart type.
        /// </summary>
        /// <param name="chartType">Returns a valid base chart's chart type.</param>
        /// <returns></returns>
        internal static ChartTypes MapWithBaseChartType(OlapChartTypes chartType)
        {
            return (ChartTypes)Enum.Parse(typeof(ChartTypes), chartType.ToString(), false);
        }

        #region Tooltip Configuration

        private void InitializeTooltip()
        {
            if (this.OlapArea != null)
            {
                if (_seriesToolTipPopup == null)
                {
                    _seriesToolTipPopup = OlapArea.resource["SeriesToolTipPopup"] as Popup;
                    _seriesToolTipPopup.FlowDirection = this.FlowDirection;
                }

                _seriesToolTipPopup.IsHitTestVisible = false;
            }
        }

        private void TagSeriesToolTipEvents()
        {
            foreach (var series in this.OlapArea.Series)
            {
                //// Tagging mouse move and mouse leave to set the location of the popup.
                series.MouseMove += new ChartSeries.ChartMouseEventHandler(series_MouseMove);
                series.MouseLeave += new ChartSeries.ChartMouseEventHandler(series_MouseLeave);
            }
        }

        private void UpdateToolTipPosition(Point position)
        {
            _seriesToolTipPopup.HorizontalOffset = position.X;
            _seriesToolTipPopup.VerticalOffset = position.Y;
        }

        private void DisposeTooltip()
        {
            _seriesToolTipPopup = null;
        }

        private static PivotCellDescriptor GetToolTipDataContext(ChartSeries series)
        {
            int segmentIndex = series.Segments.IndexOf(series.currSegment);
            return (PivotCellDescriptor)(series.DataSource as OlapChartPointCollection)[segmentIndex].Tag;
        }

        #endregion

        #endregion

        #region Helper Events

        private void OlapDataManager_ReportChanged(object sender, ReportChangedEventArgs e)
        {
            this.ChartSettings = e.NewReport.ChartSettings;
        }

        private void OlapDataManager_CellSetChanged(object sender, CellSetChangedEventArgs e)
        {
            if (this.OlapDataManager != null)
            {
                this.Dispatcher.BeginInvoke(delegate
                {
                    OlapDataManager.ExecuteOlapTable(GridLayout.NoSummaries);
                    DataBind(this.OlapDataManager.PivotEngine);
                });
            }
        }

        private void DataBind(PivotEngine engine)
        {
            if (engine != null)
            {
                this.OlapArea.DataBind(engine);
            }

            this.IsProcessing = false;
            this.TagSeriesToolTipEvents();
            this.RaiseDataRefreshCompleted(new DataRefreshCompletedEventArgs());
        }

        private void series_MouseMove(object sender, ChartMouseEventArgs e)
        {
            if (this.ShowSeriesToolTip)
            {
                ChartSeries series = sender as ChartSeries;
                this.InitializeTooltip();
                var position = e.MouseEventArgs.GetPosition(null);

                //// Known issue in Silverlight Pop-up. When HitTest is performed. It's visibility gets toggled/
                //// Workaround to avoid the hit testing happening in dynamic tooltip mode.
                position.X += 10;
                position.Y += 10;

                //// Getting the DataContext for the series segment.
                this._seriesToolTipPopup.DataContext = GetToolTipDataContext(series);
                this._seriesToolTipPopup.IsOpen = true;

                //// Constantly updates the tooltip position as and when the mouse moves.
                this.UpdateToolTipPosition(position);
            }
        }

        private void series_MouseLeave(object sender, ChartMouseEventArgs e)
        {
            if (this._seriesToolTipPopup != null)
            {
                this._seriesToolTipPopup.IsOpen = false;
            }
        }

        #endregion
    }
}