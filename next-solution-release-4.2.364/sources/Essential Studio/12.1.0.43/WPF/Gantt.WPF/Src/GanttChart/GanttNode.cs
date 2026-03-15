#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System;
using Syncfusion.Windows.GridCommon;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Windows.Data;
using System.IO;


namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Represents a control that can display the duration of a task in Gantt Chart.
    /// </summary>
    [
    TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_ProgressThumb", Type = typeof(Thumb)),
    TemplatePart(Name = "PART_Border", Type = typeof(Border)),
    TemplatePart(Name = "PART_DragDropThumb", Type = typeof(Thumb)),
    ]
    public class GanttNode : ContentControl, IDisposable
    {
        #region Internal Properties

        internal Thumb RightThumb;
        internal Thumb LeftThumb;
        internal Thumb ProgressThumb;
        internal Thumb DragdropThumb;

        internal Border NodeBorder;

        double rightChange = 0;
        double leftChange = 0;
        Point mouseDownPosition;

        bool isTemplateApplied = false;
        bool isUpdateSuspended = false;
        bool isOnProgressResize = false;
        bool isInMileStone = false;
        
     

        internal GanttChartRow ParentRow { get; set; }
        internal bool IsInHighlightedItems = false;

        Brush defaultBackground;
        internal DateTime OldEndTime;
        internal Double OldEndPoint;
        internal ResizingTooltipInfo toolTipInfo;

#if SILVERLIGHT
        bool canUpdateSource = false;
        double progressChange = 0;
#endif

        #endregion

        #region Public Properties

        #region Positioning properties

        /// <summary>
        /// Gets or sets the x1.
        /// </summary>
        /// <value>The x1.</value>
        public double X1
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the x2.
        /// </summary>
        /// <value>The x2.</value>
        public double X2
        {
            get;
            set;
        }

        #endregion

        #region Progress Width Property

        /// <summary>
        /// Gets or sets the width of the progress.
        /// </summary>
        /// <value>The width of the progress.</value>
        public double ProgressWidth
        {
            get { return (double)GetValue(ProgressWidthProperty); }
            internal set { SetValue(ProgressWidthProperty, value); }
        }

        public static DependencyProperty ProgressWidthProperty = DependencyProperty.Register("ProgressWidth", typeof(double), typeof(GanttNode), new PropertyMetadata(0.0001d, OnProgressWidthChanged));

        /// <summary>
        /// Called when [progress width changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProgressWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var node = (GanttNode)d;
            //node.ProgressWidth = (double)e.NewValue;
        }

        #endregion

        #region ToolTipTemplate Property
        public DataTemplate ToolTipTemplate
        {
            get { return (DataTemplate)GetValue(ToolTipTemplateProperty); }
            set { SetValue(ToolTipTemplateProperty, value); }
        }

        public static DependencyProperty ToolTipTemplateProperty = DependencyProperty.Register("ToolTipTemplate", typeof(DataTemplate), typeof(GanttNode), new PropertyMetadata(null));
        #endregion

        #region Progress Indicator Background Property

        /// <summary>
        /// Gets or sets the progress indicator background.
        /// </summary>
        /// <value>
        /// The progress indicator background.
        /// </value>
        public Brush ProgressIndicatorBackground
        {
            get { return (Brush)GetValue(ProgressIndicatorBackgroundProperty); }
            set { SetValue(ProgressIndicatorBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for progress Indicator Background brush..  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressIndicatorBackgroundProperty =
            DependencyProperty.Register("ProgressIndicatorBackground", typeof(Brush), typeof(GanttNode), new PropertyMetadata(new SolidColorBrush(Colors.Black)));


        #endregion

        #endregion

        #region  Bounded Properties

        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(GanttNode), new PropertyMetadata(DateTime.Today, OnStartTimeChanged));

        public static void OnStartTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttNode node = (sender as GanttNode);
#if SILVERLIGHT
            if (node != null && node.DataContext != null && node.canUpdateSource)
            {
                BindingExpression bexpression = node.GetBindingExpression(GanttNode.StartTimeProperty);
                if (bexpression != null)
                    bexpression.UpdateSource();
            }
#endif
             // To ensure the available of live parents
            if (node == null || node.ParentRow == null || node.ParentRow.ParentControl == null || node.ParentRow.ParentControl.ParentControl == null || node.isUpdateSuspended)
                return;
            node.SuspendUpdation();
            if (node != null && node.DataContext != null)
            {
                DateTime start = (DateTime)args.NewValue;
                // To ensure that start date and end date are within the possible region to draw the node
                if (start < node.ParentRow.ParentControl.ParentControl.StartTime)
                {
                    // To ensure that end date is also within the possible region to draw the node
                    // if it is not within the region no need to proceed further
                    if (!node.IsWithinFeasibleRegion(node.EndTime))
                        return;
                    // This is to check whether the user is entering the date that is beyond the the range of the chart
                    node.CheckToExtendChartWidth(start);
                }

                if(start.CompareTo(node.EndTime) > 0)
                {
                    node.EndTime = node.StartTime.Add(node.EndTime - (DateTime)args.OldValue);
                }
                

                if (node is MileStone && !node.StartTime.Equals(node.EndTime))
                {
                    node.EndTime = node.StartTime;
                }
                node.ResumeUpdation();
                node.UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is mile stone.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is mile stone; otherwise, <c>false</c>.
        /// </value>
        public bool IsMileStone
        {
            get { return (bool)GetValue(IsMileStoneProperty); }
            set { SetValue(IsMileStoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMileStone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMileStoneProperty =
            DependencyProperty.Register("IsMileStone", typeof(bool), typeof(GanttNode), new PropertyMetadata(false,OnIsMileStoneChanged));

        public static void 
            OnIsMileStoneChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttNode node = sender as GanttNode;
            

            if (node == null || node.isUpdateSuspended || node.DataContext==null || node.ParentRow == null || node.ParentRow.ParentControl == null || node.ParentRow.ParentControl.ParentControl == null)
                return;

            // To ensure the available of live parents
            node.isInMileStone = true;
            node.IsMileStone = true;

            if (node is MileStone)
            {
                if (!(bool)args.NewValue)
                {
                    MileStone mStone = node as MileStone;
#if SILVERLIGHT
                    mStone.canUpdateSource = true;
#endif
                    node.SuspendUpdation();
                    if (mStone.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
                    {
                        if (mStone.OldEndTime != DateTime.MinValue)
                            mStone.EndTime = mStone.OldEndTime;
                    }
                    else
                    {
                        if (mStone.OldEndPoint != double.MinValue && mStone.EndPoint == mStone.StartPoint)
                            mStone.EndPoint = mStone.OldEndPoint;
                    }
#if SILVERLIGHT
                    mStone.canUpdateSource = false;
#endif
                    node.ResumeUpdation();
                    mStone.UpdatePosition();

                    return;
                }
            }

            else
            {
                node.UpdatePosition();
            }
            node.isInMileStone = false;
        }
        

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(GanttNode), new PropertyMetadata(DateTime.Today.AddDays(1), OnEndTimeChanged));

        public static void OnEndTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttNode node = (sender as GanttNode);
            

#if SILVERLIGHT
            if (node != null && node.DataContext != null && node.canUpdateSource)
            {
                BindingExpression bexpression = node.GetBindingExpression(GanttNode.EndTimeProperty);
                if (bexpression != null)
                    bexpression.UpdateSource();
            }
#endif
                 // To ensure the available of live parents
            if (node == null || node.ParentRow == null || node.ParentRow.ParentControl == null || node.ParentRow.ParentControl.ParentControl == null ||node.isUpdateSuspended)
                return;
            node.SuspendUpdation();
            if (node != null && node.DataContext != null)
            {
                DateTime end = (DateTime)args.NewValue;

                // To ensure that end date is within the possible region to draw the node
                if (end > node.ParentRow.ParentControl.ParentControl.EndTime)
                {
                    // To ensure that start date is within the possible region to draw the node
                    // if it is not within the region no need to proceed further
                    if (!node.IsWithinFeasibleRegion(node.StartTime))
                        return;

                    // This is to check whether the user is entering the date that is beyond the the range of the chart
                    node.CheckToExtendChartWidth((DateTime)args.NewValue);
                }
                if (end.CompareTo(node.StartTime) < 0)
                {
                    node.StartTime = end;
                }
                if (node is MileStone && !node.StartTime.Equals(node.EndTime))
                {
                    node.StartTime = node.EndTime;                  
                }
                
                node.ResumeUpdation();
                if (!node.isInMileStone)
                    node.UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty) > 100 ? 100 : (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value < 100 ? value : 100); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(GanttNode), new PropertyMetadata(0d, OnProgressChanged));

        public static void OnProgressChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttNode node = (sender as GanttNode);
            if (node != null)
            {
                node.InvalidateArrange();
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
            DependencyProperty.Register("StartPoint", typeof(Double), typeof(GanttNode), new PropertyMetadata(0d, OnStartPointChanged));

        /// <summary>
        /// Called when [start point changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartPointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttNode node = sender as GanttNode;

#if SILVERLIGHT
            if (node != null && node.DataContext != null && node.canUpdateSource)
            {
                BindingExpression bexpression = node.GetBindingExpression(GanttNode.StartPointProperty);
                if (bexpression != null)
                    bexpression.UpdateSource();
            }
#endif          
            node.SuspendUpdation();
            if (node != null && node.DataContext != null)
            {
                double startpoint = (double)args.NewValue;

                // This is to check whether the user is entering the date that is beyond the the range of the chart
                node.CheckToExtendChartWidth(startpoint);

                if (startpoint > (node.EndPoint))
                {
                  node.EndPoint = node.StartPoint + (node.EndPoint - (double)args.OldValue);    
                }
                if (node is MileStone && !node.StartPoint.Equals(node.EndPoint))
                {                    
                    node.EndPoint = node.StartPoint;                   
                }
                node.ResumeUpdation();
                node.UpdatePosition();
            }
        }

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
            DependencyProperty.Register("EndPoint", typeof(Double), typeof(GanttNode), new PropertyMetadata(1d, OnEndPointChanged));

        /// <summary>
        /// Called when [end point changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndPointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttNode node = sender as GanttNode;
         
#if SILVERLIGHT
            if (node != null && node.DataContext != null && node.canUpdateSource)
            {
                BindingExpression bexpression = node.GetBindingExpression(GanttNode.EndPointProperty);
                if (bexpression != null)
                    bexpression.UpdateSource();
            }
#endif
            //if (node == null || node.ParentRow == null || node.ParentRow.ParentControl == null || node.ParentRow.ParentControl.ParentControl == null || node.isUpdateSuspended)
            //    return;
            node.SuspendUpdation();
            if (node != null && node.DataContext != null)
            {
                double endpoint = (double)args.NewValue;

                // This is to check whether the user is entering the date that is beyond the the range of the chart
                node.CheckToExtendChartWidth(endpoint);

                if (endpoint < node.StartPoint)
                {
                    node.StartPoint = endpoint;                    
                }
                if (node is MileStone && !node.StartPoint.Equals(node.EndPoint))
                {                    
                    node.StartPoint = node.EndPoint;                
                }
                node.ResumeUpdation();
                if (!node.isInMileStone)
                    node.UpdatePosition();
            }
        }

        #endregion

        #region Constructors and overrides

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttNode"/> class.
        /// </summary>
        public GanttNode()
        {
            DefaultStyleKey = GetType();
#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif
            mouseDownPosition = new Point();
        }

        #endregion

        #region Overrides

        protected override Size ArrangeOverride(Size finalSize)
        {
            // These progress width calcualtion is interlinked to the Measure override of the GanttChartRowItemPresenter, the X1 and X2 will be calculated there
            // baed on that calculation the progress width will be calculated. isOnProgressResize is used to prevent calculating the
            // Progress width on resizing, because on resizing the progress width will be changed based on the mouse direction and 
            // Arrange override will be invoke if we didnt have this checck the progress width will get reset based on the Progress.
            // Progress will get changed only after the resizing get completed.
            // 
            // Alternate of this isOnProgressResize is, we need to calculate the progress width on progress changed call back, but to calculate that
            // we need to find the start position and end position from start and end date this will lead to unwanted calculation. So isOnProgressResize
            // will save that calculation time.
            if (!isOnProgressResize)
            {
                // calculating node width from the positioning properties
                double NodeWidth = (this.X2 - this.X1);

                // To set progress width based on node width
                if (NodeWidth > 0)
                    ProgressWidth = (this.Progress / 100) * NodeWidth;
                else
                    ProgressWidth = 0;
            }
            return base.ArrangeOverride(finalSize);
        }

        public override void OnApplyTemplate()
        {
            if (this.ParentRow != null)
            {
                this.SuspendUpdation();
                // To updating the binding
                this.HookBinding();
                this.ResumeUpdation();
#if !SILVERLIGHT
                // To clear the binding
                this.DataContextChanged -= OnDataContextChanged;
                this.DataContextChanged += OnDataContextChanged;
#endif
            }

            // Updating template child
            this.RightThumb = (Thumb)GetTemplateChild("PART_RightThumb");
            this.LeftThumb = (Thumb)GetTemplateChild("PART_LeftThumb");
            this.DragdropThumb = (Thumb)GetTemplateChild("PART_DragDropThumb");
            this.ProgressThumb = (Thumb)GetTemplateChild("PART_ProgressThumb");
            this.NodeBorder = (Border)GetTemplateChild("PART_Border");

            // Getting the default brush to use it future 
            if (NodeBorder != null)
                this.defaultBackground = NodeBorder.Background;

            // Updates the current task node background Color
            if (this.NodeBorder != null && this.ParentRow.ParentControl.IsTaskNodeBackgroundChanged && !this.IsInHighlightedItems)
                this.NodeBorder.Background = this.ParentRow.ParentControl.TaskNodeBackground;

            // Updates the current progress Indicator Background Color.
            if (this.ParentRow != null && this.ParentRow.ParentControl.IsProgressBarBrushChanged)
                this.ProgressIndicatorBackground = this.ParentRow.ParentControl.ProgressIndicatorBackground;

            // Validating the highlight items
            if (this.IsInHighlightedItems && NodeBorder != null)
                this.NodeBorder.Background = this.ParentRow.ParentControl.HighlightItemBrush;

            // To update the exact position on scrolling
            this.UpdatePosition();

            // To hook the events of template child
            this.WireEvents();

            base.OnApplyTemplate();

            // To make the parent row refresh based on the change in items source
            this.isTemplateApplied = true;
        }

        #endregion

        #endregion

        #region Binding methods

        /// <summary>
        /// Hooks the binding.
        /// </summary>
        private void HookBinding()
        {
            // Default Bindings for DateTime Schedule
            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                if (!string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.StartDateMapping))
                {
                    Binding start = new Binding(this.ParentRow.Model.TaskAttributeMapping.StartDateMapping) { Source = this.DataContext, Mode = BindingMode.TwoWay };
                    this.SetBinding(StartTimeProperty, start);
                }

                if (!string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.FinishDateMapping))
                {
                    Binding end = new Binding(this.ParentRow.Model.TaskAttributeMapping.FinishDateMapping) { Source = this.DataContext, Mode = BindingMode.TwoWay };
                    this.SetBinding(EndTimeProperty, end);
                }
            }

            // Default Bindings for Numeric Schedule
            else
            {
                if (!string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.StartPointMapping))
                {
                    Binding startpoint = new Binding(this.ParentRow.Model.TaskAttributeMapping.StartPointMapping) { Source = this.DataContext, Mode = BindingMode.TwoWay };
                    this.SetBinding(StartPointProperty, startpoint);
                }
                if (!string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.FinishPointMapping))
                {
                    Binding endpoint = new Binding(this.ParentRow.Model.TaskAttributeMapping.FinishPointMapping) { Source = this.DataContext, Mode = BindingMode.TwoWay };
                    this.SetBinding(EndPointProperty, endpoint);
                }
            }

            // Progress Binding is Same for Both DateTime and Numeric Schedule
            if (!string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.ProgressMapping))
            {
                Binding prog = new Binding(this.ParentRow.Model.TaskAttributeMapping.ProgressMapping) { Source = this.DataContext, Mode = BindingMode.TwoWay };
                this.SetBinding(ProgressProperty, prog);
            }
            if (!string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.MileStoneMapping))
            {
                Binding mileStone = new Binding(this.ParentRow.Model.TaskAttributeMapping.MileStoneMapping) { Source = this.DataContext, Mode = BindingMode.TwoWay};
                this.SetBinding(IsMileStoneProperty, mileStone);
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Handles the DataContextChanged event of the GanttNode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
                return;

            // Clear the binding
            BindingOperations.ClearAllBindings(this);
            // Unhook the event
            this.DataContextChanged -= OnDataContextChanged;
        }
#endif
        #endregion

        #region Positioning method

        /// <summary>
        /// Updates the position.
        /// </summary>
        private void UpdatePosition()
        {
            if (isUpdateSuspended || !isTemplateApplied)
                return;

            // Since we need to calculate the position on drag and drop and resizing these code has been move to arrange override to make the exat position on each arrange           
            //if (this.StartTime != null)
            //{
            //    this.X1 = this.ParentRow.ParentControl.GetStartPositionOfDate(this.StartTime);
            //}

            //if (this.EndTime != null)
            //{
            //    this.X2 = this.ParentRow.ParentControl.GetEndPositionOfDate(this.EndTime);
            //}

            if (ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                
                  // The items need to be regenerated to when its chagnes form one type to another type of node
                    if ((this.EndTime - this.StartTime).TotalDays <= 0 || this.IsMileStone)
                    {
                        this.SuspendUpdation();
                        if (string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.MileStoneMapping))
                            this.StartTime = this.EndTime;
                        this.ParentRow.RegenerateItems();
                        this.ResumeUpdation();
                    }
                    else
                    {
                        this.ParentRow.Invalidate();
                        // To repaint the connectors of this node
                        this.ParentRow.ParentControl.UpdateResizedNodeConnector(this.ParentRow);
                    }               
            }
            else
            {
                if ((this.EndPoint - this.StartPoint) <= 0 || (this is MileStone))
                {
                    this.SuspendUpdation();
                    this.StartPoint = this.EndPoint;
                    this.ParentRow.RegenerateItems();
                    this.ResumeUpdation();
                }
                else
                {
                    this.ParentRow.Invalidate();
                }
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Suspends the updation.
        /// </summary>
        internal void SuspendUpdation()
        {
            this.isUpdateSuspended = true;
        }

        /// <summary>
        /// Resumes the updation.
        /// </summary>
        internal void ResumeUpdation()
        {
            this.isUpdateSuspended = false;
        }

        /// <summary>
        /// Checks the forthe range.
        /// </summary>
        /// <param name="date">The date.</param>
        internal bool CheckToExtendChartWidth(DateTime date)
        {
            // Check whether the date is withing feasible region or not to extend the date
            if (!IsWithinFeasibleRegion(date))
                return false;

            if (date < this.ParentRow.ParentControl.StartTime)
            {
                this.ParentRow.ParentControl.Model.SetExtendedDate(date, true);
                return true;
            }
            else if (date > this.ParentRow.ParentControl.EndTime)
            {
                this.ParentRow.ParentControl.Model.SetExtendedDate(date, false);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether [the specified date] [is within feasible region].
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified date] [is within feasible region]; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsWithinFeasibleRegion(DateTime date)
        {
            if (date.Equals(DateTime.MinValue) || date.Equals(DateTime.MaxValue))
                return false;

            return true;
        }

        /// <summary>
        /// Checks the width of to extend chart.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        internal bool CheckToExtendChartWidth(Double point)
        {
            if (point < this.ParentRow.ParentControl.StartPoint)
            {
                this.ParentRow.ParentControl.StartPoint = Math.Round(point - 5);
                return true;
            }
            else if (point > this.ParentRow.ParentControl.EndPoint)
            {
                this.ParentRow.ParentControl.EndPoint = Math.Round(point + 5);
                return true;
            }
            return false;
        }

        #endregion

        #region Event wiring
        /// <summary>
        /// Wires the events.
        /// </summary>
        private void WireEvents()
        {
            this.ParentRow.ParentControl.TaskNodeBackgroundChanged -= TaskNodeBackgroundChanged;
            this.ParentRow.ParentControl.ToolTipTemplateChagned -= ParentControl_ToolTipTemplateChagned;
            this.ParentRow.ParentControl.ProgressBarBrushChanged -= ProgressBarBrushChanged;

            this.ParentRow.ParentControl.TaskNodeBackgroundChanged += TaskNodeBackgroundChanged;
            this.ParentRow.ParentControl.ToolTipTemplateChagned += ParentControl_ToolTipTemplateChagned;
            this.ParentRow.ParentControl.ProgressBarBrushChanged += ProgressBarBrushChanged;

            // To highlight only the task nodes
            if (this.NodeBorder != null)
            {
                this.ParentRow.ParentControl.HighlightItemsChanged -= ParentControl_HighlightItemsChanged;
                this.ParentRow.ParentControl.HighlightItemsChanged += ParentControl_HighlightItemsChanged;
            }

#if !SILVERLIGHT
            this.MouseDown -= GanttNode_MouseDown;
            this.MouseDown += GanttNode_MouseDown;
#else
            this.MouseLeftButtonDown -= GanttNode_MouseDown;
            this.MouseRightButtonDown -= GanttNode_MouseDown;

            this.MouseLeftButtonDown += GanttNode_MouseDown;
            this.MouseRightButtonDown += GanttNode_MouseDown;
#endif
            // To handle the resizing process of node in both direction.
            if (this.RightThumb != null)
            {
                this.RightThumb.DragStarted -= DragStarted;
                this.RightThumb.DragDelta -= RightThumb_DragDelta;
                this.RightThumb.DragCompleted -= RightThumb_DragCompleted;

                this.RightThumb.DragStarted += DragStarted;
                this.RightThumb.DragDelta += RightThumb_DragDelta;
                this.RightThumb.DragCompleted += RightThumb_DragCompleted;
            }

            if (this.LeftThumb != null)
            {
                this.LeftThumb.DragStarted -= DragStarted;
                this.LeftThumb.DragDelta -= LeftThumb_DragDelta;
                this.LeftThumb.DragCompleted -= LeftThumb_DragCompleted;

                this.LeftThumb.DragStarted += DragStarted;
                this.LeftThumb.DragDelta += LeftThumb_DragDelta;
                this.LeftThumb.DragCompleted += LeftThumb_DragCompleted;
            }

            if (DragdropThumb != null)
            {
                this.DragdropThumb.DragStarted -= DragStarted;
                this.DragdropThumb.DragDelta -= DragdropThumb_DragDelta;
                this.DragdropThumb.DragCompleted -= DragdropThumb_DragCompleted;

                this.DragdropThumb.DragStarted += DragStarted;
                this.DragdropThumb.DragDelta += DragdropThumb_DragDelta;
                this.DragdropThumb.DragCompleted += DragdropThumb_DragCompleted;
            }

            if (this.ProgressThumb != null)
            {
                this.ProgressThumb.DragStarted -= ProgressThumb_DragStarted;
                this.ProgressThumb.DragDelta -= ProgressThumb_DragDelta;
                this.ProgressThumb.DragCompleted -= ProgressThumb_DragCompleted;

                this.ProgressThumb.DragStarted += ProgressThumb_DragStarted;
                this.ProgressThumb.DragDelta += ProgressThumb_DragDelta;
                this.ProgressThumb.DragCompleted += ProgressThumb_DragCompleted;
            }
        }

        private void UNWireEvents()
        {
            if (ParentRow != null && ParentRow.ParentControl != null)
            {
                this.ParentRow.ParentControl.TaskNodeBackgroundChanged -= TaskNodeBackgroundChanged;
                this.ParentRow.ParentControl.ToolTipTemplateChagned -= ParentControl_ToolTipTemplateChagned;
                this.ParentRow.ParentControl.ProgressBarBrushChanged -= ProgressBarBrushChanged;
            }
            // To highlight only the task nodes
            if (this.NodeBorder != null)
                this.ParentRow.ParentControl.HighlightItemsChanged -= ParentControl_HighlightItemsChanged;

#if !SILVERLIGHT
            this.MouseDown -= GanttNode_MouseDown;
#else
            this.MouseLeftButtonDown -= GanttNode_MouseDown;
            this.MouseRightButtonDown -= GanttNode_MouseDown;
#endif
            // To handle the resizing process of node in both direction.
            if (this.RightThumb != null)
            {
                this.RightThumb.DragStarted -= DragStarted;
                this.RightThumb.DragDelta -= RightThumb_DragDelta;
                this.RightThumb.DragCompleted -= RightThumb_DragCompleted;
            }

            if (this.LeftThumb != null)
            {
                this.LeftThumb.DragStarted -= DragStarted;
                this.LeftThumb.DragDelta -= LeftThumb_DragDelta;
                this.LeftThumb.DragCompleted -= LeftThumb_DragCompleted;
            }

            if (DragdropThumb != null)
            {
                this.DragdropThumb.DragStarted -= DragStarted;
                this.DragdropThumb.DragDelta -= DragdropThumb_DragDelta;
                this.DragdropThumb.DragCompleted -= DragdropThumb_DragCompleted;
            }

            if (this.ProgressThumb != null)
            {
                this.ProgressThumb.DragStarted -= ProgressThumb_DragStarted;
                this.ProgressThumb.DragDelta -= ProgressThumb_DragDelta;
                this.ProgressThumb.DragCompleted -= ProgressThumb_DragCompleted;
            }
            this.ParentRow = null;
        }

        /// <summary>
        /// Handles the ToolTipTemplateChagned event of the ParentControl.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void ParentControl_ToolTipTemplateChagned(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ToolTipTemplate = (DataTemplate)e.NewValue;
        }

        /// <summary>
        /// Handles the Task node background changed Event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void TaskNodeBackgroundChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.NodeBorder != null)
            {
                this.NodeBorder.Background = (Brush)e.NewValue;
            }
        }

        /// <summary>
        /// Handles the progress bar brush changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void ProgressBarBrushChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.ProgressIndicatorBackground = (Brush)e.NewValue;
        }
        void GanttNode_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // To update the selected item in Gantt Grid, when there is no thumb
            this.ParentRow.ParentControl.UpdateSlectedItem(this.ParentRow);
            e.Handled = true;
        }

        #endregion

        #region Dragdrop handler

        /// <summary>
        /// Drags the started.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        void DragStarted(object sender, DragStartedEventArgs e)
        {
            rightChange = leftChange = 0;

            // To bring the background rectange to accurate position
            this.ParentRow.BringResizingRectToPosition(this);

            // To update the selected item in Gantt Grid
            this.ParentRow.ParentControl.UpdateSlectedItem(this.ParentRow);
            this.mouseDownPosition.X = e.HorizontalOffset;
            this.mouseDownPosition.Y = e.VerticalOffset;
#if SILVERLIGHT
            canUpdateSource = true;
#endif
            toolTipInfo = new ResizingTooltipInfo() { StartTime = this.StartTime, EndTime = this.EndTime, Start = this.StartPoint, End = this.EndPoint, Progress = this.Progress, TaskName=GetTaskName(this.DataContext) };

            if (this.ParentRow.ParentControl.ShowResizingTooltip)
                ShowResizingTooltip(toolTipInfo, this.ParentRow.ResizingPopup);
        }

        /// <summary>
        /// Handles the DragCompleted event of the DragdropThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        void DragdropThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.ParentRow.ResizingPopup.IsOpen = false;
#if !SILVERLIGHT
            this.ParentRow.ResizingRect.Visibility = System.Windows.Visibility.Hidden;
#else
            this.ParentRow.ResizingRect.Visibility = System.Windows.Visibility.Collapsed;
#endif
            if (e.HorizontalChange == 0 || e.Canceled)
                return;

#if !SILVERLIGHT
            Point newPos = this.ParentRow.ResizingRect.TranslatePoint(new Point(0, 0), this.ParentRow);
#else
            GeneralTransform objGeneralTransform = this.ParentRow.ResizingRect.TransformToVisual(this.ParentRow as UIElement);
            Point newPos = objGeneralTransform.Transform(new Point(0, 0));
#endif
            double adjustedWidth = newPos.X - this.X1;

            NodeDragAndDropEventArgs args = new NodeDragAndDropEventArgs() 
            { 
                Node = this, NodePresenter = this.Parent as GanttChartRowItemsPresenter, 
#if !SILVERLIGHT
                RoutedEvent = GanttControl.NodeDragCompletedEvent,
#endif
                HorizontalChange = e.HorizontalChange };

            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                args.StartTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.StartTime, adjustedWidth);
                args.EndTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.EndTime, adjustedWidth);
            }
            else
            {
                args.Start = this.ParentRow.ParentControl.ConvertPositionToPoint(this.StartPoint, adjustedWidth);
                args.End = this.ParentRow.ParentControl.ConvertPositionToPoint(this.EndPoint, adjustedWidth);
            }

            this.ParentRow.ParentControl.RaiseNodeDragCompleted(args);
            
            // To avoid node getting repainted on setting the start date itself
            this.SuspendUpdation();

            //Node Start and End value is set only when the NodeDragCompleted event is not handled
            if (!args.Handled)
            {
                this.StartTime = args.StartTime;
                this.EndTime = args.EndTime;
                this.StartPoint = args.Start;
                this.EndPoint = args.End;
            }
                   
            // To make the node to reflect the changes in the soruce
            this.ResumeUpdation();

            // To get the node repaint in the new location
            this.UpdatePosition();

            // To rearrange the node in new position
            this.ParentRow.ItemsPresenter.InvalidateArrange();

            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                bool chartWidthChanged = CheckToExtendChartWidth(this.StartTime) || CheckToExtendChartWidth(this.EndTime);

                if (!chartWidthChanged)
                {
                    // To repaint the connectors of this node
                    this.ParentRow.ParentControl.UpdateResizedNodeConnector(this.ParentRow);
                }
            }

#if SILVERLIGHT
            canUpdateSource = false;
#endif
        }

        /// <summary>
        /// Handles the DragDelta event of the DragdropThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void DragdropThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {

#if !SILVERLIGHT
            double adjustedWidth = e.HorizontalChange - leftChange;
            Point newPos = this.ParentRow.ResizingRect.TranslatePoint(new Point(0, 0), this.ParentRow);

            this.ParentRow.ResizingRect.Arrange(new Rect(newPos.X + adjustedWidth, 0, this.ParentRow.ResizingRect.Width, this.ParentRow.ResizingRect.Height));
            leftChange = e.HorizontalChange;
#else
            GeneralTransform objGeneralTransform = this.ParentRow.ResizingRect.TransformToVisual(this.ParentRow as UIElement);
            Point newPos = objGeneralTransform.Transform(new Point(0, 0));

            Canvas.SetLeft(this.ParentRow.ResizingRect, newPos.X + e.HorizontalChange);
#endif
            toolTipInfo = new ResizingTooltipInfo();
            toolTipInfo.TaskName = GetTaskName(this.DataContext);
            
            NodeDragAndDropEventArgs args = new NodeDragAndDropEventArgs() 
            { 
                Node = this, NodePresenter = this.Parent as GanttChartRowItemsPresenter,
#if !SILVERLIGHT
                RoutedEvent = GanttControl.NodeDragDeltaEvent, 
#endif
              
                HorizontalChange = e.HorizontalChange };

#if !SILVERLIGHT
            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
               args.StartTime= toolTipInfo.StartTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.StartTime, e.HorizontalChange);
               args.EndTime= toolTipInfo.EndTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.EndTime, e.HorizontalChange);
            }
            else
            {
               args.Start= toolTipInfo.Start = this.ParentRow.ParentControl.ConvertPositionToPoint(this.StartPoint, e.HorizontalChange);
               args.End= toolTipInfo.End = this.ParentRow.ParentControl.ConvertPositionToPoint(this.EndPoint, e.HorizontalChange);
            }
#else
            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                args.StartTime = toolTipInfo.StartTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.ParentRow.ParentControl.StartTime,newPos.X+e.HorizontalChange);
                args.EndTime = toolTipInfo.EndTime = this.ParentRow.ParentControl.ConvertPositionToDate(toolTipInfo.StartTime, this.ParentRow.ResizingRect.Width);
            }
            else
            {
                args.Start = toolTipInfo.Start = this.ParentRow.ParentControl.ConvertPositionToPoint(this.ParentRow.ParentControl.StartPoint, newPos.X + e.HorizontalChange);
                args.End = toolTipInfo.End = this.ParentRow.ParentControl.ConvertPositionToPoint(toolTipInfo.Start, this.ParentRow.ResizingRect.Width);
            }

#endif

            if (this.ParentRow.ParentControl.ShowResizingTooltip)
                ShowResizingTooltip(toolTipInfo, this.ParentRow.ResizingPopup);

            this.ParentRow.ParentControl.RaiseNodeDragDelta(args);
        }

        #endregion

        #region Left resizing handler

        /// <summary>
        /// Handles the DragCompleted event of the LeftThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        void LeftThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.ParentRow.ResizingPopup.IsOpen = false;
#if !SILVERLIGHT
            this.ParentRow.ResizingRect.Visibility = System.Windows.Visibility.Hidden;
            Point newPos = this.ParentRow.ResizingRect.TranslatePoint(new Point(0, 0), this.ParentRow);

            newPos.X = Math.Abs(newPos.X - this.X2) <= 1.5 ? this.X2 : newPos.X;
#else
            this.ParentRow.ResizingRect.Visibility = System.Windows.Visibility.Collapsed;

            GeneralTransform objGeneralTransform = this.ParentRow.ResizingRect.TransformToVisual(this.ParentRow as UIElement);
            Point newPos = objGeneralTransform.Transform(new Point(0, 0));

            if (newPos.X + this.ParentRow.ResizingRect.StrokeThickness > this.X2)
                newPos.X = this.X2;
#endif
            if (e.Canceled)
                return;

            double adjustedWidth = newPos.X - this.X1;

            NodeResizingEventArgs args = new NodeResizingEventArgs() 
            { 
                Node = this, NodePresenter = this.Parent as GanttChartRowItemsPresenter, ResizingDirection = Directions.Left,
#if !SILVERLIGHT
                RoutedEvent = GanttControl.NodeResizingCompletedEvent, 
#endif
                EndTime = this.EndTime, End=this.EndPoint };

            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                args.StartTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.StartTime, adjustedWidth);
            }
            else
            {
                args.Start = this.ParentRow.ParentControl.ConvertPositionToPoint(this.StartPoint, adjustedWidth);
            }

            this.ParentRow.ParentControl.RaiseNodeResizingCompleted(args);

            //Node Start Value is set only when the NodeResizing Completed event is not handled
            if (!args.Handled)
            {
                this.StartTime = args.StartTime;
                this.StartPoint = args.Start;
            }
            if (ParentRow != null)
            {
                this.ParentRow.ItemsPresenter.InvalidateArrange();

                if (!CheckToExtendChartWidth(this.StartTime))
                {
                    // To repaint the connectors of this node
                    this.ParentRow.ParentControl.UpdateResizedNodeConnector(this.ParentRow);
                }
            }

#if SILVERLIGHT
            canUpdateSource = false;
#endif
        }

        /// <summary>
        /// Handles the DragDelta event of the LeftThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void LeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
#if !SILVERLIGHT
            double adjustedWidth = e.HorizontalChange - leftChange;
            double newWidth = this.ParentRow.ResizingRect.Width + (adjustedWidth < 0 ? Math.Abs(adjustedWidth) : -adjustedWidth);

            if (newWidth > 0)
            {
                this.ParentRow.ResizingRect.Width = newWidth;

                Point newPos = this.ParentRow.ResizingRect.TranslatePoint(new Point(0, 0), this.ParentRow);
                this.ParentRow.ResizingRect.Arrange(new Rect(newPos.X + adjustedWidth, 0, this.ParentRow.ResizingRect.Width, this.ParentRow.Height));

                leftChange = e.HorizontalChange;
            }
#else
            if (leftChange <= (this.ActualWidth - (this.ParentRow.ResizingRect.StrokeThickness * 1)))
            {
                double adjustedWidth = this.ParentRow.ResizingRect.Width - e.HorizontalChange;

                GeneralTransform objGeneralTransform = this.ParentRow.ResizingRect.TransformToVisual(this.ParentRow as UIElement);
                Point newPos = objGeneralTransform.Transform(new Point(0, 0));
                if (adjustedWidth >= 0)
                {
                    this.ParentRow.ResizingRect.Width = adjustedWidth;
                    Canvas.SetLeft(this.ParentRow.ResizingRect, newPos.X + e.HorizontalChange);
                }
                else
                {
                    Canvas.SetLeft(this.ParentRow.ResizingRect, newPos.X + this.ParentRow.ResizingRect.Width);
                    this.ParentRow.ResizingRect.Width = 0;
                }
            }

            leftChange += e.HorizontalChange;
#endif
            toolTipInfo = new ResizingTooltipInfo() { End = this.EndPoint, EndTime = this.EndTime, TaskName = GetTaskName(this.DataContext) };

            NodeResizingEventArgs args = new NodeResizingEventArgs() 
            { 
                Node = this, NodePresenter = this.Parent as GanttChartRowItemsPresenter, ResizingDirection = Directions.Left,
#if !SILVERLIGHT
                RoutedEvent = GanttControl.NodeResizingDeltaEvent, 
#endif
                EndTime = this.EndTime , End=this.EndPoint};


            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                args.StartTime = toolTipInfo.StartTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.StartTime, leftChange);
            }
            else
            {
                args.Start = toolTipInfo.Start = this.ParentRow.ParentControl.ConvertPositionToPoint(this.StartPoint, leftChange);
            }

             if (this.ParentRow.ParentControl.ShowResizingTooltip)
                 ShowResizingTooltip(toolTipInfo, this.ParentRow.ResizingPopup);

            this.ParentRow.ParentControl.RaiseNodeResizingDelta(args);
        }

        #endregion

        #region Right resizing handler

        /// <summary>
        /// Handles the DragCompleted event of the RightThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        void RightThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //Clossing the resizing Popup if opened
            this.ParentRow.ResizingPopup.IsOpen = false;
#if !SILVERLIGHT
            this.ParentRow.ResizingRect.Visibility = System.Windows.Visibility.Hidden;
#else
            this.ParentRow.ResizingRect.Visibility = System.Windows.Visibility.Collapsed;
#endif

            if (e.Canceled)
                return;

            double adjustedWidth = this.ParentRow.ResizingRect.ActualWidth <= 1.5 ? 0 : this.ParentRow.ResizingRect.ActualWidth;

            NodeResizingEventArgs args = new NodeResizingEventArgs() 
            { 
                Node = this, NodePresenter = this.Parent as GanttChartRowItemsPresenter, ResizingDirection = Directions.Right,
#if !SILVERLIGHT
                RoutedEvent = GanttControl.NodeResizingCompletedEvent, 
#endif
                StartTime=this.StartTime,Start=this.StartPoint };

            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                args.EndTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.StartTime, adjustedWidth);
            }
            else
            {
                args.End = this.ParentRow.ParentControl.ConvertPositionToPoint(this.StartPoint, adjustedWidth);
            }

            this.ParentRow.ParentControl.RaiseNodeResizingCompleted(args);

            //Node Start Value is set only when the NodeResizing Completed event is not handled
            if (!args.Handled)
            {
                this.EndTime = args.EndTime;
                this.EndPoint = args.End;
            }

            if (ParentRow != null)
            {
               this.ParentRow.ItemsPresenter.InvalidateArrange();

                if (!this.CheckToExtendChartWidth(this.EndTime))
                {
                    // To repaint the connectors of this node
                    this.ParentRow.ParentControl.UpdateResizedNodeConnector(this.ParentRow);
                }
            }

#if SILVERLIGHT
            canUpdateSource = false;
#endif
        }

        /// <summary>
        /// Handles the DragDelta event of the RightThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void RightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
#if !SILVERLIGHT
            double adjustedWidth = e.HorizontalChange - rightChange;
            double newWidth = this.ParentRow.ResizingRect.Width + adjustedWidth;

            if (newWidth > 0)
            {
                this.ParentRow.ResizingRect.Width = newWidth;

                rightChange = e.HorizontalChange;
            }
#else
            if (rightChange >= -(this.ActualWidth - (this.ParentRow.ResizingRect.StrokeThickness * 1)))
            {
                double adjustedWidth = this.ParentRow.ResizingRect.Width + e.HorizontalChange;
                this.ParentRow.ResizingRect.Width = adjustedWidth > 0 ? adjustedWidth : 0;
            }

            rightChange += e.HorizontalChange;
#endif
            toolTipInfo = new ResizingTooltipInfo() { Start = this.StartPoint, StartTime = this.StartTime, TaskName = GetTaskName(this.DataContext) };

            NodeResizingEventArgs args = new NodeResizingEventArgs() 
            {
                Node = this, NodePresenter = this.Parent as GanttChartRowItemsPresenter, ResizingDirection = Directions.Right, 
#if !SILVERLIGHT
                RoutedEvent = GanttControl.NodeResizingDeltaEvent, 
#endif
                StartTime = this.StartTime, Start = this.StartPoint };

            if (this.ParentRow.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric)
            {
                args.EndTime = toolTipInfo.EndTime = this.ParentRow.ParentControl.ConvertPositionToDate(this.EndTime, rightChange);
            }
            else
            {
                args.End = toolTipInfo.End = this.ParentRow.ParentControl.ConvertPositionToPoint(this.EndPoint, rightChange);
            }

            if (this.ParentRow.ParentControl.ShowResizingTooltip)
                ShowResizingTooltip(toolTipInfo,this.ParentRow.ResizingPopup);

            this.ParentRow.ParentControl.RaiseNodeResizingDelta(args);
        }
        #endregion

        #region Progres resizing handler

        /// <summary>
        /// Handles the DragStarted event of the ProgressThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragStartedEventArgs"/> instance containing the event data.</param>
        void ProgressThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
#if SILVERLIGHT
            this.progressChange = 0;
#endif
            this.isOnProgressResize = true;

            // To update the selected item in Gantt Grid
            this.ParentRow.ParentControl.UpdateSlectedItem(this.ParentRow);
            this.mouseDownPosition.X = e.HorizontalOffset;
            this.mouseDownPosition.Y = e.VerticalOffset;
#if SILVERLIGHT
            canUpdateSource = true;
#endif
            toolTipInfo = new ResizingTooltipInfo() { Progress = this.Progress, TaskName = GetTaskName(this.DataContext) };
            
            if (this.ParentRow.ParentControl.ShowResizingTooltip)
                ShowResizingTooltip(toolTipInfo,this.ParentRow.ProgressPopup);
        }

        /// <summary>
        /// Handles the DragCompleted event of the ProgressThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragCompletedEventArgs"/> instance containing the event data.</param>
        void ProgressThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            this.ParentRow.ProgressPopup.IsOpen = false;
            if (e.Canceled)
            {
                ProgressWidth = (this.Progress / 100) * (X2 - X1);
                return;
            }

            this.Progress = Math.Round((ProgressWidth * 100) / (X2 - X1));

            // enabling recalculating progress width on arrange override.
            this.isOnProgressResize = false;

#if SILVERLIGHT
            canUpdateSource = false;
#endif
        }

        /// <summary>
        /// Handles the DragDelta event of the ProgressThumb control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.Primitives.DragDeltaEventArgs"/> instance containing the event data.</param>
        void ProgressThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
#if !SILVERLIGHT
            double newWidth = (this.ProgressWidth + e.HorizontalChange) < 0 ? 0 : this.ProgressWidth + e.HorizontalChange;

            if (newWidth >= 0 && newWidth <= this.ActualWidth)
            {
                this.isOnProgressResize = true;
                this.ProgressWidth = newWidth;
            }
#else
            // these type of check has been maintained to avoid the unexpect size, because the thumb in silverlight will return
            // only the immediate chagne, so we need to track of the exact position of the mouse with the help of cumulative change.
            if(progressChange > -(this.ProgressWidth))
            {
                double newWidth = this.ProgressWidth + e.HorizontalChange < 0 ? 0 : this.ProgressWidth + e.HorizontalChange;
                this.ProgressWidth = newWidth;
            }

            this.progressChange += e.HorizontalChange;
#endif
            toolTipInfo = new ResizingTooltipInfo() {  TaskName = GetTaskName(this.DataContext) };

#if !SILVERLIGHT
            toolTipInfo.Progress =Math.Round((ProgressWidth * 100) / (X2 - X1));
#else
            toolTipInfo.Progress = Math.Round((ProgressWidth * 100) / (X2 - X1)) > 100 ? 100 : Math.Round((ProgressWidth * 100) / (X2 - X1));
#endif

            if (this.ParentRow.ParentControl.ShowResizingTooltip)
                ShowResizingTooltip(toolTipInfo, this.ParentRow.ProgressPopup);
        }

        #endregion

        #region Highlighted Items Chagned

        /// <summary>
        /// Handles the HighlightItemsChanged event of the ParentControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void ParentControl_HighlightItemsChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!this.isTemplateApplied)
                return;
			
			//While setting the background for NodeBorder in codebehind or loaded event the defaultBackground property will be null.
			//So here we assinging the default background value of the node.
            if (this.defaultBackground == null)
                this.defaultBackground = this.NodeBorder.Background;

            // Validating the change of Brush
            if (e.Property == GanttChart.HighlightItemBrushProperty && this.IsInHighlightedItems)
            {
                this.NodeBorder.Background = this.ParentRow.ParentControl.HighlightItemBrush;
                return;
            }

            // Validating the presence of data context in highlighted items
            if (this.ParentRow.IsInHighlightedItems(this.DataContext))
            {
                this.NodeBorder.Background = this.ParentRow.ParentControl.HighlightItemBrush;
                this.IsInHighlightedItems = true;
                return;
            }

            this.IsInHighlightedItems = false;

            // Validating for the bursh chagne and applying the background
            this.NodeBorder.Background = this.ParentRow.ParentControl.IsTaskNodeBackgroundChanged ? this.ParentRow.ParentControl.TaskNodeBackground : this.defaultBackground;
        }

        #endregion

        #region Resizing Tooltip Helper Methods

        /// <summary>
        /// Shows the resizing tooltip.
        /// </summary>
        /// <param name="dataContext">The data context.</param>
        /// <param name="popup">The popup.</param>
        private void ShowResizingTooltip(object dataContext, Popup popup)
        {
            popup.DataContext = dataContext;
#if !SILVERLIGHT
            popup.PlacementTarget = this;
            popup.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            popup.VerticalOffset = 10;
#else
            SetPopupPosition(popup);
#endif
            popup.IsOpen = true;
        }

        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <returns></returns>
        private string GetTaskName(object record)
        {
            string taskName = null;

            if (!string.IsNullOrEmpty(this.ParentRow.Model.TaskAttributeMapping.TaskNameMapping))
            {
                taskName = (string)this.ParentRow.Model.ItemProperties[this.ParentRow.Model.TaskAttributeMapping.TaskNameMapping].GetValue(record);
            }
            return taskName;
        }

#if SILVERLIGHT

        /// <summary>
        /// Sets the popup position.
        /// </summary>
        /// <param name="popup">The popup.</param>
        private void SetPopupPosition(Popup popup)
        {
            popup.VerticalOffset = mouseDownPosition.Y+30;
            popup.HorizontalOffset = mouseDownPosition.X;
        }

#endif
        #endregion

        public virtual void Dispose(bool flag)
        {
            this.DataContext = null;
#if !SILVERLIGHT
            this.DataContextChanged -= OnDataContextChanged;
#endif
            this.UNWireEvents();
            this.ParentRow = null;
            
            
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }

    #region HeaderNode

    /// <summary>
    /// Represents a control that can display the duration of a header task in Gantt Chart.
    /// </summary>
    public class HeaderNode : GanttNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderNode"/> class.
        /// </summary>
        public HeaderNode()
        {
            DefaultStyleKey = GetType();

#if !SILVERLIGHT
        }
#else
            this.WireEvent();            
        }

        /// <summary>
        /// Called when [layout updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnLayoutUpdated(object sender, EventArgs e)
        {
            // To refresh the header node on zooming
            if (this.RenderSize.Width != this.Width && !double.IsNaN(this.Width))
            {
                this.Measure(new Size(this.Width, this.ActualHeight));
            }
        }
        private void WireEvent()
        {
            this.LayoutUpdated += OnLayoutUpdated;
        }
        private void UnWireEvent()
        {
            this.LayoutUpdated -= OnLayoutUpdated;
        }
        public override void Dispose(bool flag)
        {
            this.UnWireEvent();
            base.Dispose(flag);
        }
#endif
    }

    #endregion

    #region Milstone

    /// <summary>
    /// Represents a control that can display the Milstone of a project in Gantt Chart.
    /// </summary>
    public class MileStone : GanttNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MileStone"/> class.
        /// </summary>
        public MileStone()
        {
            DefaultStyleKey = GetType();
        }
    }
    #endregion
}
