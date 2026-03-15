#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections;
using Syncfusion.Windows.Controls.Gantt.Schedule;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{    
    /// <summary>
    /// Represents a control that can display the progress of project in graphical notation.
    /// </summary>
   [TemplatePart(Name = "PART_GanttChartScrollViewer", Type = typeof(ScrollViewer))]
   [TemplatePart(Name = "PART_GanttChartBackgroundPanel", Type = typeof(GanttChartBackgroundPanel))]
   [TemplatePart(Name = "PART_GanttChartStripLinePanel", Type = typeof(GanttChartStripLinePanel))]
    public class GanttChart : ItemsControl
    {
        #region Private/Internal Members

        private bool isBackgroundRefreshed;

        private bool isLoaded = false;

        internal bool isConnectorRefreshed;

        internal bool isSizeChanged = false;

        internal bool IsTaskNodeBackgroundChanged;

        internal bool IsProgressBarBrushChanged;

       /// <summary>
       /// This is to avoid recreating too tip for each node.
       /// </summary>
        internal DataTemplate InBuiltTooltipTemplate;

        /// <summary>
        /// Gets or sets the GanttChart scroll viewer.
        /// </summary>
        /// <value>The chart scroll viewer.</value>
        internal ScrollViewer ChartScrollViewer { get; set; }

        /// <summary>
        /// Gets or sets the parent control.
        /// </summary>
        /// <value>The parent control.</value>
        internal GanttControl ParentControl { get; set; }

        /// <summary>
        /// Gets or sets the background panel.
        /// </summary>
        /// <value>The background panel.</value>
        internal GanttChartBackgroundPanel BackgroundPanel { get; set; }

        /// <summary>
        /// Gets or sets the srripline panel.
        /// </summary>
        /// <value>The srripline panel.</value>
        internal GanttChartStripLinePanel StriplinePanel { get; set; }

        /// <summary>
        /// Gets or sets the node connector panel.
        /// </summary>
        /// <value>The node connector panel.</value>
        internal GanttNodeConnector NodeConnectorPanel { get; set; }

        /// <summary>
        /// Gets or sets the chart items panel.
        /// </summary>
        /// <value>The chart items panel.</value>
        /// This will available only with Silverlight Gantt
        internal GanttChartItemsPanel ChartItemsPanel { get; set; }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public GanttModel Model
        {
            get
            {
                return this.ParentControl != null ? this.ParentControl.Model: null;
            }
        }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>
        /// The start point.
        /// </value>
        public Double StartPoint
        {
            get { return (Double)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Double), typeof(GanttChart), new PropertyMetadata(0d,OnStartPointChanged));

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public Double EndPoint
        {
            get { return (Double)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Double), typeof(GanttChart), new PropertyMetadata(100d,OnEndPointChanged));


        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime StartTime
        {
            get
            {
                return (DateTime)GetValue(StartTimeProperty);
            }
           internal set
            {
                if (value != StartTime)
                {
                    // To maintain the unique date throught the Gantt, the follwing code is commented and the date applied as it is.
                    //var date = ParentControl.Model.GetStartDate();
                    //SetValue(StartTimeProperty, date < value ? date : value);

                    SetValue(StartTimeProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(GanttChart), new PropertyMetadata(DateTime.Today.AddDays(-14), OnStartTimeChanged));

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            internal set
            {
                if (value != EndTime)
                {
                    // To maintain the unique date throught the Gantt, the follwing code is commented and the date applied as it is.
                    //var date = ParentControl.Model.GetEndDate();
                    //SetValue(EndTimeProperty, date < value ? value : date);

                    SetValue(EndTimeProperty, value);
                }
            }
        }

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(GanttChart), new PropertyMetadata(DateTime.Today.AddDays(14),OnEndTimeChanged));

        /// <summary>
        /// Gets or sets a value indicating whether [ show chartlines].
        /// </summary>
        /// <value><c>true</c> if [show chart lines]; otherwise, <c>false</c>.</value>
        public bool ShowChartLines
        {
            get { return (bool)GetValue(ShowChartLinesProperty); }
            set { SetValue(ShowChartLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for  chart lines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowChartLinesProperty =
            DependencyProperty.Register("ShowChartLines", typeof(bool), typeof(GanttChart), new PropertyMetadata(true, OnShowChartLinesChanged));

        
        /// <summary>
        /// Gets or sets a value indicating whether [show non working Hours background].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [show non working Hours background]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowNonWorkingHoursBackground
        {
            get { return (bool)GetValue(ShowNonWorkingHoursBackgroundProperty); }
            set { SetValue(ShowNonWorkingHoursBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for show non working Hours background.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNonWorkingHoursBackgroundProperty =
            DependencyProperty.Register("ShowNonWorkingHoursBackground", typeof(bool), typeof(GanttChart), new PropertyMetadata(true, OnShowNonWorkingHoursBackgroundChanged));

        /// <summary>
        /// Gets or sets the task node background.
        /// </summary>
        /// <value>The task node background.</value>
        public Brush TaskNodeBackground
        {
            get { return (Brush)GetValue(TaskNodeBackgroundProperty); }
            set { SetValue(TaskNodeBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TaskNodeBackgroundBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TaskNodeBackgroundProperty =
            DependencyProperty.Register("TaskNodeBackground", typeof(Brush), typeof(GanttChart), new PropertyMetadata(null, OnTaskNodeBackgroudChanged));

        /// <summary>
        /// Gets or sets the connector stroke.
        /// </summary>
        /// <value>The connector stroke.</value>
        public Brush ConnectorStroke
        {
            get { return (Brush)GetValue(ConnectorStrokeProperty); }
            set { SetValue(ConnectorStrokeProperty, value); }
        }
#if !SILVERLIGHT
        // Using a DependencyProperty as the backing store for ConnectorStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorStrokeProperty =
            DependencyProperty.Register("ConnectorStroke", typeof(Brush), typeof(GanttChart), new PropertyMetadata(Brushes.Black, OnConnectorStrokeChange));
#else
        // Using a DependencyProperty as the backing store for ConnectorStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConnectorStrokeProperty =
            DependencyProperty.Register("ConnectorStroke", typeof(Brush), typeof(GanttChart), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnConnectorStrokeChange));
#endif
        /// <summary>
        /// Gets or sets the stick current date line to.
        /// </summary>
        /// <value>The stick current date line to.</value>
        public CurentDateLinePositions StickCurrentDateLineTo
        {
            get { return (CurentDateLinePositions)GetValue(StickCurrentDateLineToProperty); }
            set { SetValue(StickCurrentDateLineToProperty, value); }
        }

        // Dependency Registration for StickCurrentDateLineTo
        public static readonly DependencyProperty StickCurrentDateLineToProperty =
            DependencyProperty.Register("StickCurrentDateLineTo", typeof(CurentDateLinePositions), typeof(GanttChart), new PropertyMetadata( CurentDateLinePositions.Today,OnStickCurrentDateLineChanged));
               
       
        /// <summary>
        /// Gets or sets the current date line.
        /// </summary>
        /// <value>The current date line.</value>
        public Line CurrentDateLine
        {
            get { return (Line)GetValue(CurrentDateLineProperty); }
            set { SetValue(CurrentDateLineProperty, value); }
        }

        /// <summary>
        /// Dependency registration for CurrentDateLine
        /// </summary>
        public static readonly DependencyProperty CurrentDateLineProperty =
            DependencyProperty.Register("CurrentDateLine", typeof(Line), typeof(GanttChart), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the tool tip template.
        /// </summary>
        /// <value>The tool tip template.</value>
        public DataTemplate ToolTipTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
            set { SetValue(ToolTipTemplateProperty, value); }
        }

        /// <summary>
        ///Dependency registration for Connector Stroke 
        /// </summary>
        public static DependencyProperty ToolTipTemplateProperty =
            DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(GanttChart), new PropertyMetadata(null));


        /// <summary>
        /// Gets or sets the resource name visibility.
        /// </summary>
        /// <value>The resource name visibility.</value>
        public Visibility ResourceNameVisibility
        {
            get { return (Visibility)GetValue(ResourceNameVisibilityProperty); }
            set { SetValue(ResourceNameVisibilityProperty, value); }
        }

        /// <summary>
        ///Dependency registration for Resource name visibility
        /// </summary>
        public static DependencyProperty ResourceNameVisibilityProperty =
            DependencyProperty.Register("ResourceNameVisibility", typeof(Visibility), typeof(GanttChart), new PropertyMetadata(Visibility.Visible, OnResourceNameVisibilityChanged));


        /// <summary>
        /// Gets or sets the non working hours background
        /// </summary>
        /// <value>
        /// The non working hours background
        /// </value>
        public Brush NonWorkingHoursBackground
        {
            get { return (Brush)GetValue(NonWorkingHoursBackgroundProperty); }
            set { SetValue(NonWorkingHoursBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for non working hours background  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NonWorkingHoursBackgroundProperty =
            DependencyProperty.Register("NonWorkingHoursBackground", typeof(Brush), typeof(GanttChart), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Gets or sets the progress Indicator Background brush.
        /// </summary>
        /// <value>
        /// The progress Indicator Background brush.
        /// </value>
        public Brush ProgressIndicatorBackground
        {
            get { return (Brush)GetValue(ProgressIndicatorBackgroundProperty); }
            set { SetValue(ProgressIndicatorBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for progress Indicator Background brush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressIndicatorBackgroundProperty =
            DependencyProperty.Register("ProgressIndicatorBackground", typeof(Brush), typeof(GanttChart), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnProgressIndicatorBackgroundChanged));

        /// <summary>
        /// Gets or sets the resource name brush.
        /// </summary>
        /// <value>
        /// The resource name brush.
        /// </value>
        public Brush ResourceNameForeground
        {
            get { return (Brush)GetValue(ResourceNameForegroundProperty); }
            set { SetValue(ResourceNameForegroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceNameBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ResourceNameForegroundProperty =
            DependencyProperty.Register("ResourceNameForeground", typeof(Brush), typeof(GanttChart), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the highlight item brush.
        /// </summary>
        /// <value>The highlight item brush.</value>
        public Brush HighlightItemBrush
        {
            get { return (Brush)GetValue(HighlightItemBrushProperty); }
            set { SetValue(HighlightItemBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlightItemBrush.  This enables animation, styling, binding, etc...
#if !SILVERLIGHT
        public static readonly DependencyProperty HighlightItemBrushProperty =
            DependencyProperty.Register("HighlightItemBrush", typeof(Brush), typeof(GanttChart), new PropertyMetadata(Brushes.Red, OnHighlightItemBrushChanged));
#else
        public static readonly DependencyProperty HighlightItemBrushProperty =
             DependencyProperty.Register("HighlightItemBrush", typeof(Brush), typeof(GanttChart), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnHighlightItemBrushChanged));
#endif

		/// <summary>
        /// Gets or sets a value indicating whether [show resizing tooltip].
        /// </summary>
        /// <value><c>true</c> if [show resizing tooltip]; otherwise, <c>false</c>.</value>
        public bool  ShowResizingTooltip
        {
            get { return (bool )GetValue(ShowResizingTooltipProperty); }
            set { SetValue(ShowResizingTooltipProperty, value); }
        }

        /// <summary>
        /// Dependency Registration for ShowResizingTooltip Property
        /// </summary>
        public static readonly DependencyProperty ShowResizingTooltipProperty =
            DependencyProperty.Register("ShowResizingTooltip", typeof(bool ), typeof(GanttChart), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the row.</value>
        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); }
        }

        /// <summary>
        /// Dependency property for Chart Row Height
        /// </summary>
        public static readonly DependencyProperty RowHeightProperty =
            DependencyProperty.Register("RowHeight", typeof(double), typeof(GanttChart), new PropertyMetadata(24d, OnRowHeightChanged));

        /// <summary>
        /// Gets or sets the resource placement target.
        /// </summary>
        /// <value>The resource placement target.</value>
        public PlacementMode ResourceNamePlacement
        {
            get { return (PlacementMode)GetValue(ResourceNamePlacementProperty); }
            set { SetValue(ResourceNamePlacementProperty, value); }
        }

        /// <summary>
        /// Dependency Registration for Resource Name Placement Target
        /// </summary>
        public static readonly DependencyProperty ResourceNamePlacementProperty =
            DependencyProperty.Register("ResourceNamePlacement", typeof(PlacementMode), typeof(GanttChart), new PropertyMetadata(PlacementMode.Right, OnResourcePlacementTargetChanged));

		 /// <value>
        /// 	<c>true</c> if [show grid lines on zooming]; otherwise, <c>false</c>.
        /// </value>
        public bool ShowGridLinesOnZooming
        {
            get { return (bool)GetValue(ShowGridLinesOnZoomingProperty); }
            set { SetValue(ShowGridLinesOnZoomingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowGridLinesOnZooming.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowGridLinesOnZoomingProperty =
            DependencyProperty.Register("ShowGridLinesOnZooming", typeof(bool), typeof(GanttChart), new PropertyMetadata(false));

        #endregion

        #region Dependency property callback

        /// <summary>
        /// Called when [start point changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = d as GanttChart;

            if (chart == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (chart.ParentControl != null)
                chart.ParentControl.StartPoint = Math.Round((Double)args.NewValue);

            if (chart.BackgroundPanel != null)
                chart.BackgroundPanel.StartPoint = Math.Round((Double)args.NewValue);

            // To rearrange the child in the new chart width
            if (chart.isLoaded)
                chart.InvalidateArrange();
        }

        /// <summary>
        /// Called when [end point changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = d as GanttChart;

            if (chart == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (chart.ParentControl != null)
                chart.ParentControl.EndPoint = Math.Round((Double)args.NewValue);

            if (chart.BackgroundPanel != null)
                chart.BackgroundPanel.EndPoint = Math.Round((Double)args.NewValue);

            // To rearrange the child in the new chart width
            if (chart.isLoaded)
                chart.InvalidateArrange();

        }

        /// <summary>
        /// Called when [start time changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = d as GanttChart;
            
            if (chart == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (chart.ParentControl != null)
                chart.ParentControl.StartTime = (DateTime)args.NewValue;

            if (chart.BackgroundPanel != null)
                chart.BackgroundPanel.StartTime = (DateTime)args.NewValue;

            // To rearrange the child in the new chart width
            if (chart.isLoaded)
                chart.InvalidateArrange();
        }

        /// <summary>
        /// Called when [end time changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = d as GanttChart;

            if (chart == null)
                return;

            // Forcing the changes in Schedule and Chart
            if (chart.ParentControl != null)
                chart.ParentControl.EndTime = (DateTime)args.NewValue;

            if (chart.BackgroundPanel != null)
                chart.BackgroundPanel.EndTime = (DateTime)args.NewValue;

            // To rearrange the child in the new chart width
            if (chart.isLoaded)
                chart.InvalidateArrange();
        }


        /// <summary>
        /// Called when [task node backgroud changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnTaskNodeBackgroudChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart ganttChart = (sender as GanttChart);
            if (ganttChart == null)
                return;

            ganttChart.IsTaskNodeBackgroundChanged = true;

            if (ganttChart.TaskNodeBackgroundChanged != null)
                ganttChart.TaskNodeBackgroundChanged(sender, args);
        }

        /// <summary>
        /// Called when [stick current date line changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStickCurrentDateLineChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart ganttchart = sender as GanttChart;
            if (ganttchart != null && ganttchart.BackgroundPanel!=null)
            {
                ganttchart.BackgroundPanel.SetStictCurrentDateLineto((CurentDateLinePositions)args.NewValue);
            }
        }

        /// <summary>
        /// Called when [connector stroke change].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnConnectorStrokeChange(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = sender as GanttChart;

            if (chart == null)
                return;
            if (chart.NodeConnectorPanel != null)
                chart.NodeConnectorPanel.ConnectorStroke = (Brush)args.NewValue;
        }

        /// <summary>
        /// Called when [resource name visibility changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public static void OnResourceNameVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = sender as GanttChart;

            if (chart == null)
                return;

            if (chart.ResourceNameVisibilityChanged != null)
                chart.ResourceNameVisibilityChanged(chart, args);
        }

        /// <summary>
        /// Called when [progress Indicator Background brush. changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProgressIndicatorBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = sender as GanttChart;
            if (chart == null)
                return;
            chart.IsProgressBarBrushChanged = true;

            if (chart.ProgressBarBrushChanged != null)
                chart.ProgressBarBrushChanged(chart, args);
        }

        /// <summary>
        /// Called when [show chart lines changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowChartLinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = sender as GanttChart;

            if (chart == null || chart.BackgroundPanel==null)
                return;

            chart.BackgroundPanel.ResetBackground();

        }

        /// <summary>
        /// Called when [show non working Hours background changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnShowNonWorkingHoursBackgroundChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = sender as GanttChart;
            if (chart == null || chart.BackgroundPanel == null)
                return;

            chart.BackgroundPanel.ResetBackground();
        }

        /// <summary>
        /// Called when [highlight item brush changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnHighlightItemBrushChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as GanttChart).RaiseHighlightedItemsChanged(args);
        }

        /// <summary>
        /// Raises the highlighted items changed.
        /// </summary>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        public void RaiseHighlightedItemsChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.HighlightItemsChanged != null)
                this.HighlightItemsChanged(this, args);
        }

        /// <summary>
        /// Called when [row height changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnRowHeightChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = sender as GanttChart;
            if (chart != null && chart.ChartItemsPanel!=null)
            {
                chart.ChartItemsPanel.RowHeight = (double)args.NewValue;

#if SILVERLIGHT
                chart.ChartItemsPanel.InvalidateMeasure();
                chart.ChartItemsPanel.InvalidateArrange();
#endif
            }
        }

        /// <summary>
        /// Called when [resource placement target changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnResourcePlacementTargetChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            GanttChart chart = sender as GanttChart;
            if (chart != null && chart.ChartItemsPanel != null)
            {
                foreach (GanttChartRow row in chart.ChartItemsPanel.Children)
                {
                    row.ItemsPresenter.InvalidateMeasure();
                    row.ItemsPresenter.InvalidateArrange();
                }
            }
        }

        #endregion

        #region DependencyPropertyChangedEventHandler

        /// <summary>
        /// Occurs when [Task node background changed].
        /// </summary>
        public event DependencyPropertyChangedEventHandler TaskNodeBackgroundChanged;

        /// <summary>
        /// Occurs when [tool tip template chagned].
        /// </summary>
        public event DependencyPropertyChangedEventHandler ToolTipTemplateChagned;

        /// <summary>
        /// Occurs when [resource name visibility changed].
        /// </summary>
        public event DependencyPropertyChangedEventHandler ResourceNameVisibilityChanged;

        /// <summary>
        /// Occurs when [progress Indicator Background changed].
        /// </summary>
        public event DependencyPropertyChangedEventHandler ProgressBarBrushChanged;

        /// <summary>
        /// Occurs when [highlight items changed].
        /// </summary>
        public event DependencyPropertyChangedEventHandler HighlightItemsChanged;

        #endregion

        #region Constructor & OnApplyTemplate

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttChart"/> class.
        /// </summary>
        public GanttChart()
        {
            DefaultStyleKey = typeof(GanttChart);
#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChartScrollViewer = (ScrollViewer)GetTemplateChild("PART_GanttChartScrollViewer");           
            ParentControl.GanttChartScrollViewer = this.ChartScrollViewer;                

            this.Loaded += GanttChart_Loaded;
            this.LayoutUpdated += GanttChart_LayoutUpdated;
#if SILVERLIGHT
            this.ParentControl.ItemsSourceChanged += ParentControl_ItemsSourceChanged;
#endif
        }

        /// <summary>
        /// Handles the LayoutUpdated event of the GanttChart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void GanttChart_LayoutUpdated(object sender, EventArgs e)
        {
            if (!isBackgroundRefreshed)
            {
                this.BackgroundPanel = this.ChartScrollViewer.FindElementOfType<GanttChartBackgroundPanel>();
                this.BackgroundPanel.ParentControl = this;

                this.BackgroundPanel.StartTime = this.StartTime;
                this.BackgroundPanel.EndTime = this.EndTime;

                this.BackgroundPanel.ResetBackground();

                this.StriplinePanel = this.ChartScrollViewer.FindElementOfType<GanttChartStripLinePanel>();
                this.StriplinePanel.ParentControl = this;

                this.StriplinePanel.StartTime = this.StartTime;
                this.StriplinePanel.EndTime = this.EndTime;

                this.StriplinePanel.ResetStripline();

                this.isBackgroundRefreshed = true;

                this.NodeConnectorPanel = this.ChartScrollViewer.FindElementOfType<GanttNodeConnector>();
                this.ChartItemsPanel = this.ChartScrollViewer.FindElementOfType<GanttChartItemsPanel>();

                this.NodeConnectorPanel.ConnectorStroke = this.ConnectorStroke;
                this.NodeConnectorPanel.ParentControl = this;

                this.NodeConnectorPanel.UpdatePredecessorInfo();
            }

            // In Silverlight during measure override the NodeConnectorPanel is not get rendered, hence the connectes are not getting refreshed on loading.
            // The following variables, isConnectorRefreshed, ChartItemsPanel, VirtualStartIndex and VirtualEndIndex of ChartItemsPanel
            // Since silverlight is having different hierary on rendering the child and parent. Need to find a generic way for silverlight to ensure all the childs of the 
            // chart are get arranged properly in the new position. This will fix the issue of refreshing the connector in Silverlight.
            // One advantange in refreshing the connector here is the refresh connect will be invoked only once, but on measure overrid it will be invoked for multiple time.
            if (!isConnectorRefreshed && this.ChartItemsPanel != null && this.NodeConnectorPanel != null)
            {
#if SILVERLIGHT
                // To clip the connectors that goes beyond the region.
                this.NodeConnectorPanel.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
#endif
                this.NodeConnectorPanel.StartIndex = this.ChartItemsPanel.VirtualStartIndex;
                this.NodeConnectorPanel.EndIndex = this.ChartItemsPanel.VirtualEndIndex;
                // To refresh the connector.
                this.NodeConnectorPanel.RefreshConnectors();
                isConnectorRefreshed = true;
            }

            if (!this.Model.IsAllNodesExpanded)
            {
                // To expand the node only when there exists children
                if (this.Model.TaskAttributeMapping.HasChildMapping)
                    this.Model.ExpandAllNodes();
                else
                    this.Model.IsAllNodesExpanded = true;
            }
        }

#if !SILVERLIGHT
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            this.isBackgroundRefreshed = false;
            //if (NodeConnectorPanel != null)
            //{
            //    this.NodeConnectorPanel.UpdatePredecessorInfo();
            //}
        }
#else
        /// <summary>
        /// Handles the ItemsSourceChanged event of the ParentControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void ParentControl_ItemsSourceChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.isBackgroundRefreshed = false;
            //if (this.isLayoutUpdated && this.NodeConnectorPanel != null)
            //{
            //    this.NodeConnectorPanel.UpdatePredecessorInfo();
            //}
        }
#endif

        /// <summary>
        /// Handles the Loaded event of the GanttChart control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void GanttChart_Loaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
        }

        #endregion

        #region ItemGenerator

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GanttChartRow { ParentControl= this };
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GanttChartRow;
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
#if !SILVERLIGHT
            GanttChartRow row = (element as GanttChartRow);
            if(row is IDisposable)
                (row as IDisposable).Dispose();
#endif
            base.ClearContainerForItemOverride(element, item);
        }

        #endregion

        #region  Helper Methods

        /// <summary>
        /// Updates the chart back ground.
        /// </summary>
        internal void UpdateChartBackGround()
        {
            if (this.BackgroundPanel != null)
                this.BackgroundPanel.ResetBackground();            
        }

        /// <summary>
        /// Gets the start position of date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        internal double GetStartPositionOfDate(DateTime date)
        {
          //  return this.ParentControl.GanttSchedule.GetPosition(ParentControl.GanttSchedule.StartTime, date);
            return this.ParentControl.GanttSchedule.GetPosition(date);
        }
#if !SILVERLIGHT
        /// <summary>
        /// Gets the start position of date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="scheduleUnit">The schedule unit.</param>
        /// <returns></returns>
        internal double GetStartPositionOfDate(DateTime date, TimeUnit scheduleUnit)
        {
           // return this.ParentControl.GanttSchedule.GetPosition(ParentControl.GanttSchedule.StartTime, date, scheduleUnit);
            return this.ParentControl.GanttSchedule.GetPosition(date, scheduleUnit);
        }
#endif
        /// <summary>
        /// Gets the end position of date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        internal double GetEndPositionOfDate(DateTime date)
        {
            return this.ParentControl.GanttSchedule.GetPosition(date);
        }

        ///// <summary>
        ///// Converts the distance to time span.
        ///// </summary>
        ///// <param name="distance">The distance.</param>
        ///// <returns></returns>
        //internal TimeSpan ConvertDistanceToTimeSpan(double distance)
        //{
        //    //return this.ParentControl.GanttSchedule.GetTimespan(ParentControl.GanttSchedule.StartTime, distance);
        //}

        /// <summary>
        /// Converts the chart position to date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        internal DateTime ConvertPositionToDate(DateTime date, double width)
        {
            return this.ParentControl.GanttSchedule.PositionToDate(date, width);
        }

        /// <summary>
        /// Gets the position of point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal double GetPositionOfPoint(double point)
        {
            return this.ParentControl.GanttSchedule.GetPointPosition(point-1);        
        }

        /// <summary>
        /// Converts the position to point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        internal double ConvertPositionToPoint(double point, double width)
        {
            return this.ParentControl.GanttSchedule.PositionToPoint(point, width);
        }


        /// <summary>
        /// Updates the slected item.
        /// </summary>
        /// <param name="ganttChartRow">The gantt chart row.</param>
        internal void UpdateSlectedItem(GanttChartRow ganttChartRow)
        {
            GanttRecord obj = this.ItemContainerGenerator.ItemFromContainer(ganttChartRow) as GanttRecord;
            if (obj == null)
                return;

            this.Model.UpdateSelectedItem(obj.DataItem);
        }

        /// <summary>
        /// Updates the selected node connector.
        /// </summary>
        /// <param name="ganttChartRow">The gantt chart row.</param>
        internal void UpdateResizedNodeConnector(GanttChartRow ganttChartRow)
        {
            if (this.NodeConnectorPanel == null)
                return;

            var obj = this.ItemContainerGenerator.ItemFromContainer(ganttChartRow);
            if (obj == null || obj == DependencyProperty.UnsetValue)
                return;

            // To repaint the connector of the particular node
            this.NodeConnectorPanel.RepaintResizedNodeConnector((obj as GanttRecord).DataItem);
        }

        /// <summary>
        /// Handles the underlying collection chnage
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="ChangeAction">The change action.</param>
        internal void CollectionChanged(object obj, NotifyCollectionChangedAction ChangeAction)
        {
            if (this.NodeConnectorPanel != null)
                this.NodeConnectorPanel.CollectionChanged(obj, ChangeAction);
        }

        /// <summary>
        /// Ups the type of the date node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        internal void UpdateNodeType(object parent)
        {
            // This it to change the node type on when a normal node become a header node on collection changed event.
            if (parent != null)
            {
                GanttChartRow chartRow = (GanttChartRow)this.ItemContainerGenerator.ContainerFromItem(parent);
                if (chartRow != null)
                    chartRow.RegenerateItems();
            }
        }

        /// <summary>
        /// Ins the line collection changed.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        internal void InLineCollectionChanged(object record, NotifyCollectionChangedEventArgs e)
        {
            // This it to change the node type on when a normal node become a header node on collection changed event.
            if (record != null)
            {
                GanttChartRow chartRow = (GanttChartRow)this.ItemContainerGenerator.ContainerFromItem(record);
                if (chartRow != null)
                    chartRow.OnInLineCollectionChanged(record, e);
            }
        }

        /// <summary>
        /// Removes the children.
        /// </summary>
        internal void RemoveChildren()
        {
            if (this.BackgroundPanel != null)
            {
                //Here we remove the CurrentDateLine from the UIElement collection to avoid the Exception thrown when we change the visual style dynamically.
                if (BackgroundPanel.Children.Contains(this.CurrentDateLine))
                {
                    this.BackgroundPanel.Children.Remove(this.CurrentDateLine);
                }
            }
        }

        /// <summary>
        /// Raises the node created.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeCreatedEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeCreated(NodeCreatedEventArgs args)
        {
            this.ParentControl.RaiseNodeCreated(args);
        }

        /// <summary>
        /// Raises the node drag completed.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeDragAndDropEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeDragCompleted(NodeDragAndDropEventArgs args)
        {
            this.ParentControl.RaiseNodeDragCompleted(args);
        }

        /// <summary>
        /// Raises the node drag delta.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeDragAndDropEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeDragDelta(NodeDragAndDropEventArgs args)
        {
            this.ParentControl.RaiseNodeDragDelta(args);
        }

        /// <summary>
        /// Raises the node resizing completed.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeResizingEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeResizingCompleted(NodeResizingEventArgs args)
        {
            this.ParentControl.RaiseNodeResizingCompleted(args);
        }

        /// <summary>
        /// Raises the node resizing delta.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.NodeResizingEventArgs"/> instance containing the event data.</param>
        internal void RaiseNodeResizingDelta(NodeResizingEventArgs args)
        {
            this.ParentControl.RaiseNodeResizingDelta(args);
        }

        #endregion

    }
}
