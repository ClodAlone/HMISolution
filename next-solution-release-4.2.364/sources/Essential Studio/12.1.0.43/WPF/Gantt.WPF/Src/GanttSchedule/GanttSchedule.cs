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
using System.Linq;
using System.Windows.Media;
using Syncfusion.Windows.Controls.Gantt;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Collections.Generic;

namespace Syncfusion.Windows.Controls.Gantt.Schedule
{
    /// <summary>
    /// Represents a control that act as a measurement medium of Gantt.
    /// </summary>
    public class GanttSchedule : ItemsControl
    {
        #region Private and Internal Properties

        internal ItemsPresenter ItemsPresenter { get; set; }        
        internal TimeUnit LowerTimeUnit { get; set; }
        internal double LowerCellUnit { get; set; }
        internal double LowerCellWidth { get; set; }
        internal GanttControl ParentControl { get; set; }
        internal Double DayWidth = 1;
        internal bool IsWeekBeginsOnSet = true;
        internal bool IsTemplateApplied = false;
        internal List<GanttScheduleRowInfo> baseSource;

        /// <summary>
        /// Gets the schedule scroll view.
        /// </summary>
        /// <value>The schedule scroll view.</value>
        internal ScrollViewer ScheduleScrollView
        {
            get
            {
                return this.ParentControl.ScheduleViewScrollViewer;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GanttSchedule"/> class.
        /// </summary>
        public GanttSchedule()
        {
            DefaultStyleKey = typeof(GanttSchedule);
#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif
            this.Loaded += OnLoaded;
        }

        #endregion

        #region Dependency Registration

        // Using a DependencyProperty as the backing store for ScheduleType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ScheduleTypeProperty =
            DependencyProperty.Register("ScheduleType", typeof(ScheduleType), typeof(GanttSchedule), new PropertyMetadata(ScheduleType.WeekWithDays, OnScheduleTypePropertyChanged));

        // Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(double), typeof(GanttSchedule), new PropertyMetadata(10d));

        // Using a DependencyProperty as the backing store for End.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndProperty =
            DependencyProperty.Register("End", typeof(double), typeof(GanttSchedule), new PropertyMetadata(100d));

        // Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(GanttSchedule), new PropertyMetadata(DateTime.Today.AddDays(-14)));

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(GanttSchedule), new PropertyMetadata(DateTime.Today.AddDays(14)));

        // Using a DependencyProperty as the backing store for WeekBeginsOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeekBeginsOnProperty =
            DependencyProperty.Register("WeekBeginsOn", typeof(DayOfWeek), typeof(GanttSchedule), new PropertyMetadata(DayOfWeek.Sunday));

        // Using a DependencyProperty as the backing store for FiscalYearBeginsOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FiscalYearBeginsOnProperty =
            DependencyProperty.Register("FiscalYearBeginsOn", typeof(Month), typeof(GanttSchedule), new PropertyMetadata(Month.January, OnFiscalYearBeginsOnChanged));

        // Using a DependencyProperty as the backing store for IsFYNumberingEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFYNumberingEnabledProperty =
            DependencyProperty.Register("IsFYNumberingEnabled", typeof(bool), typeof(GanttSchedule), new PropertyMetadata(false, OnFYNumberingChanged));

        // Using a DependencyProperty as the backing store for IsUIVirtualizaionEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsUIVirtualizaionEnabledProperty =
            DependencyProperty.Register("IsUIVirtualizaionEnabled", typeof(bool), typeof(GanttSchedule), new PropertyMetadata(false));

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <value>The model.</value>
        public GanttModel Model
        {
            get
            {
                return this.ParentControl != null ? this.ParentControl.Model : null;
            }
        }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>
        /// The start.
        /// </value>
        public double Start
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end.
        /// </value>
        public double End
        {
            get { return (double)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime
        {
            get { return (DateTime)GetValue(StartTimeProperty); }
            set { SetValue(StartTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>
        /// The end time.
        /// </value>
        public DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the week begins on.
        /// </summary>
        /// <value>
        /// The week begins on.
        /// </value>
        public DayOfWeek WeekBeginsOn
        {
            get { return (DayOfWeek)GetValue(WeekBeginsOnProperty); }
            set { SetValue(WeekBeginsOnProperty, value); }
        }

        /// <summary>
        /// Gets or sets the fiscal year begins on.
        /// </summary>
        /// <value>
        /// The fiscal year begins on.
        /// </value>
        public Month FiscalYearBeginsOn
        {
            get { return (Month)GetValue(FiscalYearBeginsOnProperty); }
            set { SetValue(FiscalYearBeginsOnProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is FY numbering enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is FY numbering enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsFYNumberingEnabled
        {
            get { return (bool)GetValue(IsFYNumberingEnabledProperty); }
            set { SetValue(IsFYNumberingEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of the schedule.
        /// </summary>
        /// <value>
        /// The type of the schedule.
        /// </value>
        public ScheduleType ScheduleType
        {
            get { return (ScheduleType)GetValue(ScheduleTypeProperty); }
            set { SetValue(ScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is UI virtualizaion enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is UI virtualizaion enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsUIVirtualizaionEnabled
        {
            get { return (bool)GetValue(IsUIVirtualizaionEnabledProperty); }
            set { SetValue(IsUIVirtualizaionEnabledProperty, value); }
        }

        #endregion

        #region Dependency Callback

        /// <summary>
        /// Called when [FY numbering changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFYNumberingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GanttSchedule schedule = sender as GanttSchedule;
            if (schedule == null)
                return;
            if (schedule.FiscalYearBeginsOn != Month.January && schedule.IsTemplateApplied)
                schedule.InvalidateSchedule();
        }

        /// <summary>
        /// Called when [fiscal year begins on changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnFiscalYearBeginsOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            GanttSchedule schedule = sender as GanttSchedule;
            if (schedule == null)
                return;
            if (schedule.IsFYNumberingEnabled && schedule.IsTemplateApplied)
                schedule.InvalidateSchedule();
        }

        /// <summary>
        /// Called when [schedule type property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnScheduleTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GanttSchedule schedule = d as GanttSchedule;

            if (schedule == null || schedule.ParentControl == null|| schedule.ParentControl.ItemsSource == null)
                return;

            if (schedule.ScheduleType != ScheduleType.CustomNumeric && schedule.ScheduleType != ScheduleType.CustomDateTime)
            {
                schedule.ItemsSource = schedule.GetItemsSource((ScheduleType)e.NewValue);
                schedule.baseSource = new List<GanttScheduleRowInfo>(schedule.ItemsSource.Cast<GanttScheduleRowInfo>());
            }
        }

        #endregion

        #region ItemsSource for InBuiltScheduleTypes

        /// <summary>
        /// Generates the items source for InBuilt Schedule types.
        /// </summary>
        /// <param name="schType">Type of the Schedule.</param>
        /// <returns>the items source</returns>
        private IList<GanttScheduleRowInfo> GetItemsSource(ScheduleType schType)
        {
            IList<GanttScheduleRowInfo> scheduleRowInfo = new List<GanttScheduleRowInfo>();

            switch (schType)
            {
                case ScheduleType.WeekWithDays:
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Weeks, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Days,PixelsPerUnit=20d });
                    break;
#if !SILVERLIGHT
                case ScheduleType.DayWithHours:
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Days, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Hours, PixelsPerUnit = 20d });
                    break;

                case ScheduleType.DayWithMinutes:
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Days, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Hours, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Minutes, PixelsPerUnit = 20d, });
                    break;

                case ScheduleType.MonthWithHours:
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Months, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Weeks, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Days, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Hours, PixelsPerUnit = 20d });
                    break;
#endif
                case ScheduleType.MonthWithDays:
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Months });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Weeks, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Days, PixelsPerUnit = 20d });
                    break;

                case ScheduleType.YearWithDays:
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Years, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Months });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Weeks, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Days, PixelsPerUnit = 20d, });
                    break;

                case ScheduleType.YearWithMonths:
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Years, });
                    scheduleRowInfo.Add(new GanttScheduleRowInfo() { TimeUnit = TimeUnit.Months, PixelsPerUnit = 60d });
                    break;
                case ScheduleType.CustomDateTime:
                case ScheduleType.CustomNumeric:
                    if (ParentControl.CustomScheduleSource != null)
                        scheduleRowInfo = new ObservableCollection<GanttScheduleRowInfo>(ParentControl.CustomScheduleSource);
                    break;
            }

            // To calcuate the width based on unit of least most row info
            this.InitializeUnitValues(scheduleRowInfo);
            
            return scheduleRowInfo;
        }
        #endregion

        #region OnApplyTemplate

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {       
            this.IsTemplateApplied = true;
        }

        #endregion

        #region Loaded Event handler

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.ItemsSource != null)
                this.baseSource = new List<GanttScheduleRowInfo>(this.ItemsSource.Cast<GanttScheduleRowInfo>());
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Redraw schedule.
        /// </summary>
        internal void RedrawSchedule()
        {
            this.ItemsSource = null;
            this.ItemsSource = this.GetItemsSource(this.ScheduleType); 
        }

        /// <summary>
        /// Sets the initial values to display the schedule
        /// </summary>
        internal void InitializeUnitValues(IList<GanttScheduleRowInfo> rows)
        {
            if (rows == null || rows.Count <= 0 || ParentControl.ItemsSource == null)
            {
                this.Width = double.NaN;
                this.DayWidth = 0;
                return;
            }

            // Initializing Properties.
            if (this.ScheduleType == Gantt.ScheduleType.CustomNumeric)
            {
                this.LowerCellWidth = rows[rows.Count-1].PixelsPerUnit;
                if (this.LowerCellWidth <= 0)
                    return;

                this.Width = ((this.End - this.Start) * LowerCellWidth);
            }
            else
            {
                this.LowerCellWidth = rows[rows.Count - 1].PixelsPerUnit;
                DayWidth = this.GetDayWidth(rows[rows.Count - 1].TimeUnit, this.LowerCellWidth);

                this.Width = ((this.EndTime.Subtract(this.StartTime)).TotalDays * DayWidth);
                this.LowerTimeUnit = rows[rows.Count - 1].TimeUnit;
                this.LowerCellUnit = rows[rows.Count - 1].CellsPerUnit;

                this.IsWeekBeginsOnSet = true;
            }
        }

        /// <summary>
        /// Reinitialize the units.
        /// </summary>
        internal void ReInitializeUnits()
        {
            this.InitializeUnitValues((IList<GanttScheduleRowInfo>)this.ItemsSource);
        }

        /// <summary>
        /// Invalidates Entire Schedule.
        /// </summary>
        internal void InvalidateSchedule()
        {          
            if (this.IsTemplateApplied)
            {
                foreach (GanttScheduleRowInfo rowInfo in this.Items)
                {
                    GanttScheduleRow scheduleRow = this.ItemContainerGenerator.ContainerFromItem(rowInfo) as GanttScheduleRow;
                    scheduleRow.RefreshItems();
                }
            }            
        }

        /// <summary>
        /// Gets the width of the day.
        /// </summary>
        /// <param name="timeUnit">The time unit.</param>
        /// <param name="Pixels">The pixels.</param>
        /// <returns>the required width</returns>
        private Double GetDayWidth(TimeUnit timeUnit, double Pixels)
        {
            switch (timeUnit)
            {
                case TimeUnit.Years:
                    return Pixels / 365;

                case TimeUnit.Months:
                    return Pixels / 30;

                case TimeUnit.Weeks:
                    return Pixels / 7;

                case TimeUnit.Days:
                    return Pixels;
#if !SILVERLIGHT
                case TimeUnit.Hours:
                    return Pixels * 24;

                case TimeUnit.Minutes:
                    return Pixels * 1440;
#endif
            }
            return 1;
        }

        #endregion

        #region ItemGenerator
     
        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>
        /// The element that is used to display the given item.
        /// </returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GanttScheduleRow { ParentControl = this};
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>
        /// true if the item is (or is eligible to be) its own container; otherwise, false.
        /// </returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GanttScheduleRow;
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">Element used to display the specified item.</param>
        /// <param name="item">Specified item.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            GanttScheduleRow scheduleRow = element as GanttScheduleRow;

            // Binds the Properties in Schdule with Schedule Row
            this.SetBindings(scheduleRow);
            base.PrepareContainerForItemOverride(scheduleRow, item);
        }

        /// <summary>
        /// When overridden in a derived class, undoes the effects of the <see cref="M:System.Windows.Controls.ItemsControl.PrepareContainerForItemOverride(System.Windows.DependencyObject,System.Object)"/> method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            // To clear the binding
            this.ClearBindings(element);
            base.ClearContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Clears the bindings.
        /// </summary>
        /// <param name="element">The element.</param>
        private void ClearBindings(DependencyObject element)
        {
            (element as GanttScheduleRow).DataContext = null;
            (element as GanttScheduleRow).UnWireEvents();
            element.ClearValue(GanttScheduleRow.StartTimeProperty);
            element.ClearValue(GanttScheduleRow.EndTimeProperty);
            element.ClearValue(GanttScheduleRow.StartPointProperty);
            element.ClearValue(GanttScheduleRow.EndPointProperty);
            element.ClearValue(GanttScheduleRow.WeekBeginsOnProperty);
        }

        /// <summary>
        /// Sets the required bindings.
        /// </summary>
        /// <param name="scheduleRow">The schedule row.</param>
        private void SetBindings(GanttScheduleRow scheduleRow)
        {
            Binding widthBinding = new Binding("Width");
            widthBinding.Source = this;
            scheduleRow.SetBinding(GanttScheduleRow.WidthProperty, widthBinding);

            Binding startPointBinding = new Binding("Start");
            startPointBinding.Source = this;
            scheduleRow.SetBinding(GanttScheduleRow.StartPointProperty, startPointBinding);

            Binding endPointBinding = new Binding("End");
            endPointBinding.Source = this;
            scheduleRow.SetBinding(GanttScheduleRow.EndPointProperty, endPointBinding);

            Binding startDateBinding = new Binding("StartTime");
            startDateBinding.Source = this;
            scheduleRow.SetBinding(GanttScheduleRow.StartTimeProperty, startDateBinding);

            Binding endDateBinding = new Binding("EndTime");
            endDateBinding.Source = this;
            scheduleRow.SetBinding(GanttScheduleRow.EndTimeProperty, endDateBinding);

            Binding weekBeginsOnBinding = new Binding("WeekBeginsOn");
            weekBeginsOnBinding.Source = this;
            scheduleRow.SetBinding(GanttScheduleRow.WeekBeginsOnProperty, weekBeginsOnBinding);
        }
        
        #endregion

        #region Zoom opertation

        /// <summary>
        /// Gets the clone of base source.
        /// </summary>
        /// <returns></returns>
        internal List<GanttScheduleRowInfo> GetCloneOfBaseSource()
        {
            List<GanttScheduleRowInfo> tempSource = new List<GanttScheduleRowInfo>();
            this.baseSource.ForEach(row => tempSource.Add((GanttScheduleRowInfo)row.Clone()));
            return tempSource;
        }

        /// <summary>
        /// Sets the zoom factor.
        /// </summary>
        /// <param name="zoomArgs">The <see cref="Syncfusion.Windows.Controls.Gantt.ZoomChangedEventArgs"/> instance containing the event data.</param>
        internal void SetZoomFactor(ZoomChangedEventArgs zoomArgs)
        {
            List<GanttScheduleRowInfo> itemSources = this.GetCloneOfBaseSource();

            // Checking whether the new value will go beyond the min or max length and based on that the new soruce will be created
            bool? scheduleChanged = this.CheckScheduleChange(zoomArgs, itemSources, baseSource[baseSource.Count - 1]);

            // Assigning the new items source to the schedule
            this.ItemsSource = itemSources;

            // Calculating new start and end date for the schedule
            this.ParentControl.InitializeStartAndEnd();

            // Based on the new source creating the initializing the unit values will be invoked.
            if (scheduleChanged.HasValue)
                this.ReInitializeUnits();
        }

        /// <summary>
        /// Checks the schedule change.
        /// </summary>
        /// <param name="zoomArgs">The <see cref="Syncfusion.Windows.Controls.Gantt.ZoomChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="itemSources">The source.</param>
        /// <param name="baseRow">The base row.</param>
        /// <returns></returns>
        bool? CheckScheduleChange(ZoomChangedEventArgs zoomArgs, List<GanttScheduleRowInfo> itemSources, GanttScheduleRowInfo baseRow)
        {
            bool canIncrement = false;

            while (zoomArgs.ZoomFactor > 0)
            {
                GanttScheduleRowInfo rowInfo = itemSources[itemSources.Count - 1];

                if (rowInfo == null)
                    return null;

                // calculating the new pixel with the zoom factor
                double newPixel = !canIncrement ? (rowInfo.PixelsPerUnit * zoomArgs.ZoomFactor) / 100 : rowInfo.PixelsPerUnit + ((rowInfo.PixelsPerUnit * zoomArgs.ZoomFactor) / 100);

                // Getting new day width based on the zoom factor and new pixel
                double tempDayWidth = this.GetDayWidth(rowInfo.TimeUnit, newPixel);

                // Getting the new cell width based on the day width
                double tempCellWidth = this.GetDefaultWidhtOfDateTimeCell(tempDayWidth, rowInfo.TimeUnit, rowInfo.CellsPerUnit);

                // Validating the new cell width with the base cell  min length
                if (tempCellWidth < zoomArgs.BaseCelltMinLength)
                {
                    if (itemSources.Count == 1 && this.IsCellsPerUnitExceeds(itemSources[0]))
                    {
                        rowInfo.PixelsPerUnit = zoomArgs.BaseCelltMinLength / rowInfo.CellsPerUnit;
                        break;
                    }

                    // Getting new schedule source
                    itemSources = CreateScheduleSource(zoomArgs, itemSources, baseRow, false);
                }
                // Validating the new cell width with base cell max length
                else if (tempCellWidth > zoomArgs.BaseCelltMaxLength)
                {
#if !SILVERLIGHT
                    if (rowInfo.TimeUnit == TimeUnit.Minutes && rowInfo.CellsPerUnit == 1)
#else
                    if (rowInfo.TimeUnit == TimeUnit.Days && rowInfo.CellsPerUnit == 1)
#endif
                    {
                        rowInfo.PixelsPerUnit = zoomArgs.BaseCelltMaxLength;
                        break;
                    }

                    // Getting new schedule source
                    itemSources = CreateScheduleSource(zoomArgs, itemSources, baseRow, true);
                    rowInfo = itemSources[itemSources.Count - 1];

                    canIncrement = true;

                    if (zoomArgs.ZoomFactor > 0 && zoomArgs.ZoomFactor <= 100)
                    {
                        newPixel = (rowInfo.PixelsPerUnit * zoomArgs.ZoomFactor) / 100;
                        rowInfo.PixelsPerUnit += newPixel;
                        break;
                    }
                }
                else
                {
                    // Assigning the new pixel in the existing schedule source.
                    rowInfo.PixelsPerUnit = newPixel;
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether [is cells per unit exceeds] [the specified row info].
        /// </summary>
        /// <param name="rowInfo">The row info.</param>
        /// <returns>
        /// 	<c>true</c> if [is cells per unit exceeds] [the specified row info]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsCellsPerUnitExceeds(GanttScheduleRowInfo rowInfo)
        {
            switch (rowInfo.TimeUnit)
            {
                case TimeUnit.Days:
                case TimeUnit.Weeks:
                    return rowInfo.CellsPerUnit == 3;
                case TimeUnit.Months:
                    return rowInfo.CellsPerUnit == 6;
                case TimeUnit.Years:
                    return rowInfo.CellsPerUnit == 4;
            }
            return false;
        }

        /// <summary>
        /// Creates the schedule source.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Gantt.ZoomChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="scheduleSrc">The schedule SRC.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="addNewItem">if set to <c>true</c> [add new item].</param>
        /// <returns></returns>
        private List<GanttScheduleRowInfo> CreateScheduleSource(ZoomChangedEventArgs args, List<GanttScheduleRowInfo> scheduleSrc, GanttScheduleRowInfo baseRow, bool addNewItem)
        {
            GanttScheduleRowInfo tempBaseRow = scheduleSrc[scheduleSrc.Count - 1];

            double oldBaseWidth = Math.Floor(this.GetBaseUnitWidth(baseRow.TimeUnit, tempBaseRow.TimeUnit, tempBaseRow.PixelsPerUnit));

            // Based on cells per unit, base cell min lenght, base cell max lenght the scheudle soruce will be created.
            if (addNewItem)
            {
                switch (tempBaseRow.TimeUnit)
                {
#if !SILVERLIGHT
                    case TimeUnit.Minutes:
                        if (tempBaseRow.CellsPerUnit == 15)
                        {
                            tempBaseRow.CellsPerUnit = 5;
                            tempBaseRow.PixelsPerUnit = (args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        else if (tempBaseRow.CellsPerUnit == 5)
                        {
                            tempBaseRow.CellsPerUnit = 1;
                            tempBaseRow.PixelsPerUnit = (args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        else if (tempBaseRow.CellsPerUnit > 1)
                        {
                            tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit / 2);
                            tempBaseRow.PixelsPerUnit = (args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        break;
                    case TimeUnit.Hours:
                        if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            scheduleSrc.Add(new GanttScheduleRowInfo { TimeUnit = TimeUnit.Minutes, CellsPerUnit = 30, PixelsPerUnit = (args.BaseCelltMinLength / 30) });
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit / 2);
                            tempBaseRow.PixelsPerUnit = (args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        break;
#endif
                    case TimeUnit.Days:
                        if (tempBaseRow.CellsPerUnit <= 1)
                        {
#if !SILVERLIGHT
                            scheduleSrc.Add(new GanttScheduleRowInfo { TimeUnit = TimeUnit.Hours, CellsPerUnit = 12, PixelsPerUnit = (args.BaseCelltMinLength / 12) });
#endif
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = 1;
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        break;
                    case TimeUnit.Weeks:
                        if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            scheduleSrc.Add(new GanttScheduleRowInfo { TimeUnit = TimeUnit.Days, CellsPerUnit = 3, PixelsPerUnit = (args.BaseCelltMinLength / 3) });
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = 1;
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        break;
                    case TimeUnit.Months:
                        if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            scheduleSrc.Add(new GanttScheduleRowInfo { TimeUnit = TimeUnit.Weeks, CellsPerUnit = 3, PixelsPerUnit = (args.BaseCelltMinLength / 3) });
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit / 2);
                            tempBaseRow.PixelsPerUnit = (args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        break;
                    case TimeUnit.Years:
                        if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            scheduleSrc.Add(new GanttScheduleRowInfo { TimeUnit = TimeUnit.Months, CellsPerUnit = 6, PixelsPerUnit = (args.BaseCelltMinLength / 6) });
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit / 2);
                            tempBaseRow.PixelsPerUnit = (args.BaseCelltMinLength / tempBaseRow.CellsPerUnit);
                        }
                        break;
                }

                // Calculation to reduce the zoom value based on the addition of schedule row
                GanttScheduleRowInfo lastrow = scheduleSrc[scheduleSrc.Count - 1];
                double newWidth = Math.Floor(this.GetBaseUnitWidth(baseRow.TimeUnit, lastrow.TimeUnit, lastrow.PixelsPerUnit));
                double percent = Math.Ceiling((( newWidth - oldBaseWidth) * 100) / newWidth);
                args.ZoomFactor -= percent; //Math.Floor((args.ZoomFactor * percent) / 100); //
            }
            else
            {
                switch (tempBaseRow.TimeUnit)
                {
#if !SILVERLIGHT
                    case TimeUnit.Minutes:
                        if (tempBaseRow.CellsPerUnit >= 30)
                        {
                            scheduleSrc.Remove(tempBaseRow);
                            scheduleSrc[scheduleSrc.Count - 1].PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / scheduleSrc[scheduleSrc.Count - 1].CellsPerUnit);
                        }
                        else if (tempBaseRow.CellsPerUnit == 5)
                        {
                            tempBaseRow.CellsPerUnit = 15;
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        else if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            tempBaseRow.CellsPerUnit = 5;
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit * 2);
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        break;

                    case TimeUnit.Hours:
                        if (tempBaseRow.CellsPerUnit >= 12)
                        {
                            scheduleSrc.Remove(tempBaseRow);
                            scheduleSrc[scheduleSrc.Count - 1].PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / scheduleSrc[scheduleSrc.Count - 1].CellsPerUnit);
                        }
                        else if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            tempBaseRow.CellsPerUnit = 3;
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit * 2);
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        break;
#endif
                    case TimeUnit.Days:
                    case TimeUnit.Weeks:
                        if (tempBaseRow.CellsPerUnit >= 3)
                        {
                            scheduleSrc.Remove(tempBaseRow);
                            scheduleSrc[scheduleSrc.Count - 1].PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / scheduleSrc[scheduleSrc.Count - 1].CellsPerUnit);
                        }
                        else if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            tempBaseRow.CellsPerUnit = 3;
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        break;

                    case TimeUnit.Months:
                        if (tempBaseRow.CellsPerUnit >= 6)
                        {
                            scheduleSrc.Remove(tempBaseRow);
                            scheduleSrc[scheduleSrc.Count - 1].PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / scheduleSrc[scheduleSrc.Count - 1].CellsPerUnit);
                        }
                        else if (tempBaseRow.CellsPerUnit <= 1)
                        {
                            tempBaseRow.CellsPerUnit = 3;
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        else
                        {
                            tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit * 2);
                            tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        }
                        break;

                    case TimeUnit.Years:
                        tempBaseRow.CellsPerUnit = Math.Floor(tempBaseRow.CellsPerUnit * 2);
                        tempBaseRow.PixelsPerUnit = Math.Floor(args.BaseCelltMaxLength / tempBaseRow.CellsPerUnit);
                        break;
                }

                // Calculation to reduce the zoom value based on the addition of schedule row
                GanttScheduleRowInfo lastrow = scheduleSrc[scheduleSrc.Count - 1];
                double newWidth = Math.Floor(this.GetBaseUnitWidth(baseRow.TimeUnit, lastrow.TimeUnit, lastrow.PixelsPerUnit));
                double percent = Math.Ceiling(((oldBaseWidth - newWidth) * oldBaseWidth) / newWidth);
                //double percent = Math.Ceiling((oldBaseWidth * newWidth) / oldBaseWidth);
                args.ZoomFactor -= ((args.ZoomFactor * percent) / 100); //percent; //
            }

            // returning the new source
            return scheduleSrc;
        }

        /// <summary>
        /// Gets the width of the base unit.
        /// </summary>
        /// <param name="baseUnit">The base unit.</param>
        /// <param name="currentUnit">The current unit.</param>
        /// <param name="currentPixels">The current pixels.</param>
        /// <returns></returns>
        private double GetBaseUnitWidth(TimeUnit baseUnit, TimeUnit currentUnit, double currentPixels)
        {
            double dayWidth = this.GetDayWidth(currentUnit, currentPixels);

            switch (baseUnit)
            {
#if !SILVERLIGHT
                case TimeUnit.Minutes:
                    return dayWidth / 1440;

                case TimeUnit.Hours:
                    return dayWidth / 24;
#endif
                case TimeUnit.Days:
                    return dayWidth;

                case TimeUnit.Weeks:
                    return dayWidth * 7;

                case TimeUnit.Months:
                    return dayWidth * 30;

                case TimeUnit.Years:
                    return dayWidth * 365;
            }
            return dayWidth;
        }

        /// <summary>
        /// Gets the default widht of date time cell.
        /// </summary>
        /// <param name="dayWidth">Width of the day.</param>
        /// <param name="timeUnit">The time unit.</param>
        /// <param name="cellPerUnit">The cell per unit.</param>
        /// <returns></returns>
        internal double GetDefaultWidhtOfDateTimeCell(double dayWidth, TimeUnit timeUnit, double cellPerUnit)
        {
            if (timeUnit == TimeUnit.Years)
            {
                return (dayWidth * 365 * cellPerUnit);
            }
            else if (timeUnit == TimeUnit.Months)
            {
                return (dayWidth * 30 * cellPerUnit);
            }
            else if (timeUnit == TimeUnit.Weeks)
            {
                return (dayWidth * 7 * cellPerUnit);
            }
            else if (timeUnit == TimeUnit.Days)
            {
                return (dayWidth * cellPerUnit);
            }
#if !SILVERLIGHT
            else if (timeUnit == TimeUnit.Hours)
            {
                return ((dayWidth / 24) * cellPerUnit);
            }
            else if (timeUnit == TimeUnit.Minutes)
            {
                return ((dayWidth / (60 * 24)) * cellPerUnit);
            }
#endif
            return 0;
        }

        #endregion

    }
}