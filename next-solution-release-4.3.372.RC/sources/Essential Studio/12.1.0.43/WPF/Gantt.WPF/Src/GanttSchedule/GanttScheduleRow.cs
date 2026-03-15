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
using System.Windows.Data;
using System.Windows.Media;

namespace Syncfusion.Windows.Controls.Gantt.Schedule
{
    /// <summary>
    /// Represents the control that presents the Schedule cells.
    /// </summary>
    [TemplatePart(Name = "PART_RowItemsPresenter", Type = typeof(StackPanel))]
    public class GanttScheduleRow : ContentControl
    {
        #region Private variables

        /// <summary>
        ///  Used in Custom Year calculations.
        /// </summary>
        private double remainYears = 0;
        private bool IsYearRounded = false;

        /// Used in Creating cell item and cell tool tip for Numerice Schedule
        double numericCellItem = 0;
        double numericCellTooltip = 0;

        /// used in Creating Cell item and celltooltip for Datetime schedule
        DateTime dateCellItem;
        DateTime dateCellWidth;
        DateTime dateCelltooltip;
        DateTime cellDate;
        bool isLoaded = false;

        /// To compare the values on scrolling.
        DateTime oldStartDate;
        double oldHOffset;
        double oldVWidth;

        #endregion

        #region Internal Properties

        internal GanttScheduleRowPanel ItemsPresenter { get; set; }
        internal bool IsTemplateApplied = false;

        /// <summary>
        /// Horizontal offest to use in UI virtualizaion in the row panel
        /// </summary>
        internal double HorizontalOffest
        {
            get
            {
                // since UseOnDemoand Schedule work ony for the Date Time schedule, the custom numeric type is validated.
                if (this.ParentControl == null || this.ParentControl.ScheduleType== ScheduleType.CustomNumeric)
                    return 0;
                else
                    return this.ParentControl.IsUIVirtualizaionEnabled ? this.ParentControl.ScheduleScrollView.HorizontalOffset : 0;
            }
        }

        /// <summary>
        /// Gets or sets the parent control.
        /// </summary>
        /// <value>The parent control.</value>
        internal GanttSchedule ParentControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width of the schedule cell.
        /// </summary>
        /// <value>The width of the schedule cell.</value>
        internal double ScheduleCellWidth
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the row info.
        /// </summary>
        /// <value>The row info.</value>
        internal GanttScheduleRowInfo RowInfo
        {
            get
            {
                return (this.DataContext as GanttScheduleRowInfo);
            }
        }

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>
        /// The start point.
        /// </value>
        internal Double StartPoint
        {
            get { return (Double)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        internal Double EndPoint
        {
            get { return (Double)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        internal DateTime StartTime
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
        internal DateTime EndTime
        {
            get { return (DateTime)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        #endregion

        #region Dependency Registration

        // Using a DependencyProperty as the backing store for StartPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof(Double), typeof(GanttScheduleRow), new PropertyMetadata(0d, OnStartPointChanged));

        // Using a DependencyProperty as the backing store for EndPoint.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof(Double), typeof(GanttScheduleRow), new PropertyMetadata(0d, OnEndPointChanged));

        // Using a DependencyProperty as the backing store for StartTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartTimeProperty =
            DependencyProperty.Register("StartTime", typeof(DateTime), typeof(GanttScheduleRow), new PropertyMetadata(DateTime.Today.AddDays(-14), OnStartTimeChanged));

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(DateTime), typeof(GanttScheduleRow), new PropertyMetadata(DateTime.Today.AddDays(14), OnEndTimeChanged));

        // Using a DependencyProperty as the backing store for WeekBeginsOn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WeekBeginsOnProperty =
            DependencyProperty.Register("WeekBeginsOn", typeof(DayOfWeek), typeof(GanttScheduleRow), new PropertyMetadata(DayOfWeek.Sunday, OnWeekBeginsOnChanged));

        #endregion

        #region Dependency CallBack

        /// <summary>
        /// Called when [start point changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartPointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttScheduleRow ganttScheduleRow = sender as GanttScheduleRow;
            if (ganttScheduleRow == null || ganttScheduleRow.RowInfo == null)
                return;
            if (ganttScheduleRow.IsTemplateApplied)
            {
                ganttScheduleRow.RefreshItems();
            }
        }

        /// <summary>
        /// Called when [end point changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndPointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttScheduleRow ganttScheduleRow = sender as GanttScheduleRow;
            if (ganttScheduleRow == null || ganttScheduleRow.RowInfo == null)
                return;
            if (ganttScheduleRow.IsTemplateApplied)
            {
                ganttScheduleRow.RefreshItems();
            }
        }

        /// <summary>
        /// Called when [start time changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnStartTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttScheduleRow ganttScheduleRow = sender as GanttScheduleRow;
            if (ganttScheduleRow == null || ganttScheduleRow.RowInfo == null)
                return;


            if (ganttScheduleRow.IsTemplateApplied && ganttScheduleRow.isLoaded)
            {
                // Redraws the Year, Month Rows
                if (ganttScheduleRow.RowInfo.TimeUnit == TimeUnit.Years || ganttScheduleRow.RowInfo.TimeUnit == TimeUnit.Months)
                {
                    ganttScheduleRow.RefreshItems();
                }
                // Adds only the Required Cells
                else
                {
                    ganttScheduleRow.AddPreviousItems((DateTime)args.OldValue,(DateTime)args.NewValue);
                }
            }
        }

        /// <summary>
        /// Called when [end time changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnEndTimeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttScheduleRow ganttScheduleRow = sender as GanttScheduleRow;
            if (ganttScheduleRow == null || ganttScheduleRow.RowInfo == null)
                return;
           
            if (ganttScheduleRow.IsTemplateApplied && ganttScheduleRow.isLoaded)
            {
                // Redraws the Year, Month Rows.
                if (ganttScheduleRow.RowInfo.TimeUnit == TimeUnit.Years || ganttScheduleRow.RowInfo.TimeUnit == TimeUnit.Months)
                {
                    ganttScheduleRow.RefreshItems();
                }
                // Adds Only the required Cells
                else
                {
                    ganttScheduleRow.AddContinuousItems((DateTime)args.OldValue, (DateTime)args.NewValue);
                }
            }
        }

        /// <summary>
        /// Called when [week begins on changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnWeekBeginsOnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            GanttScheduleRow ganttScheduleRow = sender as GanttScheduleRow;
            if (ganttScheduleRow == null || ganttScheduleRow.RowInfo==null)
                return;

            if (ganttScheduleRow.IsTemplateApplied && ganttScheduleRow.RowInfo.TimeUnit == TimeUnit.Weeks)
            {
                ganttScheduleRow.RefreshItems();
            }
        }

        #endregion

        #region Constructor & Overrides

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttScheduleRow"/> class.
        /// </summary>
        public GanttScheduleRow()
        {
            DefaultStyleKey = GetType();

#if !SyncfusionFramework3_5
            UseLayoutRounding = false;
#endif
            this.WireEvents();
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ItemsPresenter = this.GetTemplateChild("PART_RowItemsPresenter") as GanttScheduleRowPanel;
            ItemsPresenter.ParentControl = this;

            // Assigning the initial values to the variables used in the schedule cell creation.
            InitializeCalculateValue();

            this.IsTemplateApplied = true;
        }

        /// <summary>
        /// Initializes the calculate value.
        /// </summary>
        private void InitializeCalculateValue()
        {
            if (ParentControl.ScheduleType == ScheduleType.CustomNumeric)
            {

                // Initializing values to form numeric cells
                numericCellItem = (ParentControl.Start * (ParentControl.LowerCellWidth));
                numericCellTooltip = (ParentControl.Start * (ParentControl.LowerCellWidth));

                // To generate numeric cells with initialized values
                GenerateNumericCells(this.ParentControl.Width, 0);
            }

            // Assigning Values to variables used for DateTime Schedule
            else
            {
                // Check of virtualizaion mode enabled.
                if (this.ParentControl.IsUIVirtualizaionEnabled)
                {
                    if (this.ParentControl.ScheduleScrollView != null)
                    {
                        // Hooking the Layout update mode to predic the minor chagne and redraw the schedule
                        this.LayoutUpdated += OnLayoutUpdated;

                        // Update the date time schedule limit based on the scroll index.
                        UpdateDateTimeScrollInfo(false);
                    }
                }
                else
                {
                    // Initializing the date time to draw the schedule.
                    dateCellItem = ParentControl.StartTime;
                    dateCellWidth = ParentControl.StartTime;
                    dateCelltooltip = ParentControl.StartTime;
                    cellDate = ParentControl.StartTime;

                    // Populating the schedule cells based on the width of the schedule
                    this.GenerateDateTimeCells(this.ParentControl.Width, false);
                }
            }
        }

        /// <summary>
        /// Wires the events.
        /// </summary>
        private void WireEvents()
        {
            this.Loaded += OnLoaded;
        }

        /// <summary>
        /// Uns the wire events.
        /// </summary>
        internal void UnWireEvents()
        {
            this.Loaded -= OnLoaded;
        }

        /// <summary>
        /// Handles the Loaded event of the GanttScheduleRow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.isLoaded = true;
            this.ItemsPresenter.InvalidateArrange();
        }

        /// <summary>
        /// Called when [layout updated].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (this.isLoaded && this.IsTemplateApplied)
            {
                this.UpdateDateTimeScrollInfo(false);
#if SILVERLIGHT
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight) };
#endif
            }
        }

        /// <summary>
        /// Updates the date time scroll info.
        /// </summary>
        private void UpdateDateTimeScrollInfo(bool forceUpdated)
        {
            if (this.RowInfo == null || this.ParentControl.ScheduleScrollView.ViewportWidth == 0)
                return;

            // Validate the horizontal offset and view port width to calculate the start date
            double hOffset = this.ParentControl.ScheduleScrollView.HorizontalOffset;
            double vWidth = this.ParentControl.ScheduleScrollView.ViewportWidth;

            if (oldVWidth == vWidth && oldHOffset == hOffset && !forceUpdated)
                return;

            // Validating the width of the schedule against view port width
            if (vWidth > this.ParentControl.ActualWidth)
            {
                vWidth = this.ParentControl.ActualWidth;
#if SILVERLIGHT
                this.Clip = new RectangleGeometry { Rect = new Rect(0, 0, vWidth, this.ActualHeight) };
#endif
            }

            // Calculating start date to draw the schedule
            DateTime start = this.ParentControl.PositionToDate(this.ParentControl.StartTime, hOffset);

            // Validating the new start date with old one.
            if (oldStartDate.Equals(start) && oldVWidth == vWidth && !forceUpdated)
                return;

            // Initializing the start date to draw the schedule.
            dateCellItem = start;
            dateCellWidth = start;
            dateCelltooltip = start;
            cellDate = start;
            // For year schedule
            remainYears = 0.0;
            IsYearRounded = false;

            // Clearing the items to generate new items
            this.ItemsPresenter.Children.Clear();

            // Generating new items based on view port width
            this.GenerateDateTimeCells(vWidth, false);

            // Preserving the new values for future validation
            oldStartDate = start;
            oldHOffset = hOffset;
            oldVWidth = vWidth;
        }

        #endregion

        #region Numeric Cells

        #region Generate cells

        /// <summary>
        /// Generates the numeric cells.
        /// </summary>
        /// <param name="available">The available.</param>
        /// <param name="location">The location.</param>
        private void GenerateNumericCells(double available, double location)
        {
            if (double.IsInfinity(available))
                return;

            this.ScheduleCellWidth = this.GetDefaultWidthOfNumericCell();

            while (location <= available)
            {
                GanttScheduleCell cell = new GanttScheduleCell
                {
                    Content = GetNumericCellItem(),
                    CellToolTip = GetNumericCellToolTip(),
                    Width = this.ScheduleCellWidth,
                    HorizontalContentAlignment = this.RowInfo.HorizontalAlignment,
                    VerticalContentAlignment = this.RowInfo.VerticalAlignment,
                };

                ScheduleCellCreatedEventArgs args = new ScheduleCellCreatedEventArgs 
                { 
                    CurrentCell = cell, 
#if !SILVERLIGHT
                    RoutedEvent=GanttControl.ScheduleCellCreatedEvent 
#endif
                };

                this.ParentControl.ParentControl.RaiseScheduleCellCreated(args);

                this.ItemsPresenter.Width += args.CurrentCell.Width;
                ItemsPresenter.Children.Add(args.CurrentCell);
                
                location += this.ScheduleCellWidth;
#if SILVERLIGHT
                Binding borderbushBinding = new Binding("BorderBrush");
                borderbushBinding.Source = this.ParentControl;
                args.CurrentCell.SetBinding(GanttScheduleCell.CellBorderBrushProperty, borderbushBinding);

                Binding foregroundBinding = new Binding("Foreground");
                foregroundBinding.Source = this.ParentControl;
                args.CurrentCell.SetBinding(GanttScheduleCell.CellForegroundProperty, foregroundBinding);
#endif
            }
        }
        #endregion

        #region Cell Item & Cell Tooltip

        /// <summary>
        /// Gets the cell item.
        /// </summary>
        /// <returns>Cell Item </returns>
        private object GetNumericCellItem()
        {
            if (ScheduleCellWidth != this.ParentControl.LowerCellWidth)
            {
                numericCellItem += ScheduleCellWidth;
                double result = Math.Round(((numericCellItem - (ScheduleCellWidth - ParentControl.LowerCellWidth)) / ParentControl.LowerCellWidth), 2);
                return string.IsNullOrEmpty(this.RowInfo.CellTextFormat) ? result.ToString() : result.ToString(this.RowInfo.CellTextFormat);
            }
            else
            {
                numericCellItem += ScheduleCellWidth;
                return Math.Round(((numericCellItem) / ParentControl.LowerCellWidth), 2);
            }
        }

        /// <summary>
        /// Gets the cell tool tip.
        /// </summary>
        /// <returns>The cell tooltip</returns>
        private string GetNumericCellToolTip()
        {
            if (ScheduleCellWidth != this.ParentControl.LowerCellWidth)
            {
                numericCellTooltip += ScheduleCellWidth;
                string text = Math.Round(((numericCellTooltip - (ScheduleCellWidth - ParentControl.LowerCellWidth)) / ParentControl.LowerCellWidth), 2).ToString() + " - " + Math.Round(((numericCellTooltip) / ParentControl.LowerCellWidth), 2).ToString();
                return text;
            }
            else
            {
                numericCellTooltip += ScheduleCellWidth;
                return Math.Round((numericCellTooltip / ParentControl.LowerCellWidth), 2).ToString();
            }
        }

        #endregion

        #region Cell width calculation.

        /// <summary>
        /// Gets the default width of numeric cell.
        /// </summary>
        /// <param name="CurrentInfo">The current info.</param>
        /// <param name="currentItem">The current item.</param>
        /// <returns></returns>
        private double GetDefaultWidthOfNumericCell(double CurrentInfo, int currentItem)
        {
            if (currentItem >= this.ParentControl.Items.Count - 1)
            {
                return ((ParentControl.LowerCellWidth) * (this.ParentControl.Items[this.ParentControl.Items.Count - 1] as GanttScheduleRowInfo).CellsPerUnit);
            }
            return CurrentInfo * GetDefaultWidthOfNumericCell((this.ParentControl.Items[++currentItem] as GanttScheduleRowInfo).CellsPerUnit, currentItem);
        }

        /// <summary>
        /// Gets the default width of numeric cell.
        /// </summary>
        /// <returns></returns>
        internal double GetDefaultWidthOfNumericCell()
        {
            if (this.RowInfo == null)
                return 0;

            return this.GetDefaultWidthOfNumericCell(this.RowInfo.CellsPerUnit, this.ParentControl.Items.IndexOf(this.RowInfo));
        }

        #endregion

        #endregion

        #region Cell Creation

        /// <summary>
        /// Gets the schedule cell.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="cellitem">The cellitem.</param>
        /// <param name="celltooltip">The celltooltip.</param>
        /// <returns></returns>
        private GanttScheduleCell GetScheduleCell(double width, object cellitem, object celltooltip)
        {
            GanttScheduleCell currentCell = new GanttScheduleCell
            {
                Content = cellitem,
                Width = width,
                CellToolTip = celltooltip,
                CellDate = cellDate,
                CellTimeUnit=this.RowInfo.TimeUnit,
                HorizontalContentAlignment = this.RowInfo.HorizontalAlignment,
                VerticalContentAlignment = this.RowInfo.VerticalAlignment
            };

            ScheduleCellCreatedEventArgs args = new ScheduleCellCreatedEventArgs() 
            { 
                CurrentCell = currentCell , 
#if !SILVERLIGHT
                RoutedEvent= GanttControl.ScheduleCellCreatedEvent
#endif
            };
            this.ParentControl.ParentControl.RaiseScheduleCellCreated(args);
#if SILVERLIGHT
            Binding borderbushBinding = new Binding("BorderBrush");
            borderbushBinding.Source = this.ParentControl;
            args.CurrentCell.SetBinding(GanttScheduleCell.CellBorderBrushProperty, borderbushBinding);
#endif
            this.ItemsPresenter.Width += args.CurrentCell.Width;           
            return args.CurrentCell;
        }

        #endregion

        #region DateTime Cells

        #region Generate cells

        /// <summary>
        /// Generates the date time cells.
        /// </summary>
        /// <param name="available">The available.</param>
        /// <param name="isDynamicChange">if set to <c>true</c> [is dynamic change].</param>
        private void GenerateDateTimeCells(double available, bool isDynamicChange)
        {
            if (double.IsInfinity(available) || double.IsNaN(available) || available == 0 || this.RowInfo == null)
                return;

            // Used as temporary storage of cell information
            object tempCellItem = null;
            object tempCellTooltip = null;
            Double tempCellWidth = 0d;
            double location = 0d;

            this.ScheduleCellWidth = this.GetDefaultWidhtOfDateTimeCell();

            // Creating Year Cells
            if (RowInfo.TimeUnit == TimeUnit.Years)
            {
                // Lower cell unit is considered to check for zooming, on zooming the schedule type will be a built-in one but the cells perunit will be chagned based on zoom factor.
                if (ParentControl.ScheduleType == ScheduleType.CustomDateTime || this.ParentControl.LowerCellUnit != 1)
                {
                    // Adding First  Year Cell
                    tempCellWidth = this.GetCustomYearWidth(dateCellWidth);
                    this.IsYearRounded = true;

                    // Getting different values of cells
                    tempCellItem = this.GetCellItem(dateCellItem);
                    tempCellTooltip = this.GetCellToolTip(dateCelltooltip, true);

                    // Adding first cell
                    ItemsPresenter.Children.Add(this.GetScheduleCell(tempCellWidth, tempCellItem, tempCellTooltip));
                    location += tempCellWidth;

                    // Adding Continuation year cells
                    while (location <= available)
                    {
                        tempCellWidth = (this.GetCustomYearWidth(dateCellWidth));
                        tempCellItem = this.GetCellItem(dateCellItem);
                        tempCellTooltip = this.GetCellToolTip(dateCelltooltip, false);

                        // Adding the new cell
                        ItemsPresenter.Children.Add(this.GetScheduleCell(tempCellWidth, tempCellItem, tempCellTooltip));
                        location += tempCellWidth;
                    }
                }
                else
                {
                    if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                    {
                        // Gets the fiscal year starting Month and Calculates the Required width for the first year cell
                        double availableWidth = 0;

                        // Given fiscal year starting Month as temp time
                        DateTime temp = new DateTime(dateCellWidth.Year, (int)ParentControl.FiscalYearBeginsOn, 01);

                        // checks the given month is greater/less than the start time
                        if (dateCellWidth.DayOfYear < temp.DayOfYear)
                        {
                            availableWidth = (temp.DayOfYear - dateCellWidth.DayOfYear) * ParentControl.DayWidth;
                            dateCellWidth = temp;
                        }
                        else if (dateCellWidth.DayOfYear > temp.DayOfYear)
                        {
                            temp = temp.AddYears(1);
                            availableWidth = (temp.Subtract(dateCellWidth).TotalDays) * ParentControl.DayWidth;
                            dateCellWidth = temp;
                        }
                        tempCellTooltip = "FY " + dateCellItem.ToString("MM/dd/") + (dateCellWidth.Year) + " - " + dateCellWidth.AddDays(-1).ToString("MM/dd/") + (dateCellWidth.Year);

                        ItemsPresenter.Children.Add(this.GetScheduleCell(availableWidth, (dateCellWidth.Year), tempCellTooltip));
                        dateCellItem = dateCellWidth;
                        dateCelltooltip = dateCellWidth;
                        location += availableWidth;
                    }

                    // Adding Additional Cells for Year
                    while (location <= available)
                    {
                        if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                        {
                            // Getting the required values for Fiscal Year.
                            DateTime tempDate = dateCellWidth;
                            dateCellWidth = dateCellWidth.AddYears(1);
                            tempCellWidth = (dateCellWidth.Subtract(tempDate).TotalDays) * ParentControl.DayWidth;
                            tempCellItem = GetCellItem(dateCellItem);
                            tempCellTooltip = GetCellToolTip(dateCelltooltip, false);
                        }
                        else
                        {
                            // Calculating required values for Custom Year.
                            tempCellWidth = this.GetStandardYearWidth(dateCellWidth);
                            tempCellItem = GetCellItem(dateCellItem);
                            tempCellTooltip = GetCellToolTip(dateCelltooltip, false);
                        }
                        // Adding the new cells
                        ItemsPresenter.Children.Add(this.GetScheduleCell(tempCellWidth, tempCellItem, tempCellTooltip));
                        location += tempCellWidth;
                    }
                }
            }

            // Creating Month Cells for Built-in/Custom DateTime schedule
            else if (RowInfo.TimeUnit == TimeUnit.Months )
            {
                // Getting the start Date
                DateTime currentStart = dateCellWidth;

                // Round the date based on time unit.
                dateCellWidth = this.GetRoundedDate(dateCellWidth, TimeUnit.Days);

                // calculates the required Month based on the  given cells per Unit
                double requiredMonth = (Math.Ceiling((double)dateCellWidth.Month / RowInfo.CellsPerUnit));
                requiredMonth *= RowInfo.CellsPerUnit;

                // Calculates the additional Month Needed
                double additionalMonth = requiredMonth - dateCellWidth.Month;

                // Sets the available days based on start date
                int totalDaysInMonth = DateTime.DaysInMonth(dateCellWidth.Year, dateCellWidth.Month);
                int availableDays = dateCellWidth.Day == 1 && RowInfo.CellsPerUnit == 1 ? 0 : totalDaysInMonth - dateCellWidth.Day + 1;
                dateCellWidth = dateCellWidth.AddDays(availableDays);

                // Adds the Rounded Months to dateCell Width
                for (int i = 0; i < Math.Floor(additionalMonth); i++)
                {
                    dateCellWidth = dateCellWidth.AddMonths(1);
                }

                // Calculates the remaining Month Width
                double remMonth = additionalMonth - Math.Floor(additionalMonth);
                dateCellWidth = dateCellWidth.AddDays(remMonth * DateTime.DaysInMonth(dateCellWidth.Year, dateCellWidth.Month));

                // Assigns the width, cellitem, celltooltip
                tempCellWidth = (dateCellWidth.Subtract(currentStart).TotalDays) * ParentControl.DayWidth;
                tempCellItem = GetCellItem(dateCellItem);
                tempCellTooltip = GetCellToolTip(dateCelltooltip, true);

                // Adding first cell of month row
                ItemsPresenter.Children.Add(this.GetScheduleCell(tempCellWidth, tempCellItem, tempCellTooltip));
                location += tempCellWidth;

                // Lower cell unit is considered to check for zooming, on zooming the schedule type will be a built-in one but the cells perunit will be chagned based on zoom factor.
                if (this.ParentControl.ScheduleType == ScheduleType.CustomDateTime || this.ParentControl.LowerCellUnit != 1)
                {
                    //  Adding Continuation cells for Month.
                    while (location <= available)
                    {
                        var tempwidth = GetCustomMonthWidth(dateCellWidth) * ParentControl.DayWidth;
                        tempCellItem = GetCellItem(dateCellItem);
                        tempCellTooltip = GetCellToolTip(dateCelltooltip, false);

                        ItemsPresenter.Children.Add(this.GetScheduleCell(tempwidth, tempCellItem, tempCellTooltip));
                        location += tempwidth;
                    }
                }
                else
                {
                    //  Adding Continuation cells for Month.
                    while (location <= available)
                    {
                        var tempwidth = GetStandardMonthWidth(dateCellWidth);
                        tempCellItem = GetCellItem(dateCellItem);
                        tempCellTooltip = GetCellToolTip(dateCelltooltip, false);

                        ItemsPresenter.Children.Add(this.GetScheduleCell(tempwidth, tempCellItem, tempCellTooltip));
                        location += tempwidth;
                    }
                }
            }
            // Creating cells for other Time units
            else
            {
                // Preserving the start date for further calculation 
                DateTime currentStart = dateCellWidth;

                if (RowInfo.TimeUnit == TimeUnit.Weeks)
                {
                    // Round the date based on time unit.
                    dateCellWidth = this.GetRoundedDate(dateCellWidth, RowInfo.TimeUnit);

                    // calculates the required days based on the  given cells per Unit
                    double requiredDays = (Math.Ceiling((double)dateCellWidth.DayOfWeek / RowInfo.CellsPerUnit));
                    requiredDays *= RowInfo.CellsPerUnit;

                    // Get the additional days need to round the date based on cells per unit
                    double additionalDays = requiredDays - (int)dateCellWidth.DayOfWeek;

                    // Round the date based on Cells per unit, by adding the calculated additional days
                    dateCellWidth = dateCellWidth.Day != requiredDays ? dateCellWidth.AddDays((int)Math.Floor(additionalDays)) : dateCellWidth;

                    // Get the remaining fractional part to round the date
                    double remDays = additionalDays - Math.Floor(additionalDays);

                    // Finalized rounded date to draw the schedule cell
                    dateCellWidth = dateCellWidth.AddDays(remDays * 7);
                }
                else if (RowInfo.TimeUnit == TimeUnit.Days)
                {
                    // Round the date based on time unit.
                    dateCellWidth = this.GetRoundedDate(dateCellWidth, RowInfo.TimeUnit);

                    // calculates the required days based on the  given cells per Unit
                    double requiredDays = (Math.Ceiling((double)dateCellWidth.Day / RowInfo.CellsPerUnit));
                    requiredDays *= RowInfo.CellsPerUnit;

                    // Get the additional days need to round the date based on cells per unit
                    double additionalDays = requiredDays - dateCellWidth.Day;

                    // Round the date based on Cells per unit, by adding the calculated additional days
                    dateCellWidth = dateCellWidth.Day != requiredDays ? dateCellWidth.AddDays((int)Math.Floor(additionalDays)) : dateCellWidth;

                    // Get the remaining fractional part to round the date
                    double remDays = additionalDays - Math.Floor(additionalDays);

                    // Finalized rounded date to draw the schedule cell
                    dateCellWidth = dateCellWidth.AddHours(remDays * 24);
                }
#if !SILVERLIGHT
                else if (RowInfo.TimeUnit == TimeUnit.Hours)
                {
                    // Round the date based on time unit.
                    dateCellWidth = this.GetRoundedDate(dateCellWidth, RowInfo.TimeUnit);

                    // calculates the required Hour based on the  given cells per Unit
                    double requiredHour = (Math.Ceiling((double)dateCellWidth.Hour / RowInfo.CellsPerUnit));
                    requiredHour *= RowInfo.CellsPerUnit;

                    // Get the additional hours need to round the date based on cells per unit
                    double additionalHours = requiredHour - dateCellWidth.Hour;

                    // Round the date based on Cells per unit, by adding the calculated additional hours
                    dateCellWidth = dateCellWidth.Hour != requiredHour ? dateCellWidth.AddHours((int)Math.Floor(additionalHours)) : dateCellWidth;

                    // Get the remaining fractional part to round the date
                    double remHours = additionalHours - Math.Floor(additionalHours);

                    // Finalized rounded date to draw the schedule cell
                    dateCellWidth = dateCellWidth.AddMinutes(remHours * 60);
                }
                else if (RowInfo.TimeUnit == TimeUnit.Minutes)
                {
                    // Round the date based on time unit.
                    dateCellWidth = this.GetRoundedDate(dateCellWidth, RowInfo.TimeUnit);

                    // calculates the required Hour based on the  given cells per Unit
                    double requiredMinute = (Math.Ceiling((double)dateCellWidth.Minute / RowInfo.CellsPerUnit));
                    requiredMinute *= RowInfo.CellsPerUnit;

                    // Round the date based on time unit.
                    dateCellWidth = dateCellWidth.Minute != requiredMinute ? this.GetRoundedDate(dateCellWidth, RowInfo.TimeUnit) : dateCellWidth;

                    // Get the additional minutes need to round the date based on cells per unit
                    double additionalMinutes = requiredMinute - dateCellWidth.Minute;

                    // Round the date based on Cells per unit, by adding the calculated additional minutes
                    dateCellWidth = dateCellWidth.AddMinutes((int)Math.Floor(additionalMinutes));
                    
                    // Get the remaining fractional part to round the date
                    double remMinutes = additionalMinutes - Math.Floor(additionalMinutes);

                    // Finalized rounded date to draw the schedule cell
                    dateCellWidth = dateCellWidth.AddSeconds(remMinutes * 60);
                }
#endif
                // Assigns the width, cellitem, celltooltip
                tempCellWidth = (dateCellWidth.Subtract(currentStart).TotalDays) * ParentControl.DayWidth;
                tempCellItem = GetCellItem(dateCellItem);
                tempCellTooltip = GetCellToolTip(dateCelltooltip, true);

                // Adding the first cell based on dynamic inclusion.
                if (isDynamicChange)
                {
                    ItemsPresenter.Children.Insert(0, this.GetScheduleCell(tempCellWidth, tempCellItem, tempCellTooltip));
                }
                else
                {
                    ItemsPresenter.Children.Add(this.GetScheduleCell(tempCellWidth, tempCellItem, tempCellTooltip));
                }

                // assigning the values after adding first cell
                location += tempCellWidth;
                dateCellItem = dateCellWidth;
                dateCelltooltip = dateCellWidth;

                // Checking for dynamic cell creation
                if (isDynamicChange)
                {
                    // Since the first cell will be added before this loop the index is starting from 1.
                    int index = 1;

                    //  Adding Continuation cells
                    while (location < available)
                    {
                        tempCellItem = GetCellItem(dateCellItem);
                        tempCellTooltip = GetCellToolTip(dateCelltooltip, false);

                        // Inserting the cell is corresponding location 
                        ItemsPresenter.Children.Insert(index, this.GetScheduleCell(this.ScheduleCellWidth, tempCellItem, tempCellTooltip));
                        location += this.ScheduleCellWidth;
                        index++;
                    }
                }
                else
                {
                    //  Adding Continuation cells
                    while (location <= available)
                    {
                        tempCellItem = GetCellItem(dateCellItem);
                        tempCellTooltip = GetCellToolTip(dateCelltooltip, false);

                        // Adding the new cell
                        ItemsPresenter.Children.Add(this.GetScheduleCell(this.ScheduleCellWidth, tempCellItem, tempCellTooltip));
                        location += this.ScheduleCellWidth;
                    }
                }
            }
        }

        #endregion

        #region Cell Item & Cell Tooltip

        /// <summary>
        /// Gets the cell item.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns></returns>
        private object GetCellItem(DateTime current)
        {
            // initializing local  variables
            DateTime tempTime = current;
            object cellItem = string.Empty;

            bool isFYNumbering = false;

            // To preserve the cell date to store it in Schedule cell
            cellDate = current;

            // checking for fiscal year
            if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January && tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn)
            {
                isFYNumbering = true;
            }

            switch (this.RowInfo.TimeUnit)
            {
                case TimeUnit.Years:

                    if (isFYNumbering)
                    {
                        cellItem = (tempTime.Year + 1).ToString();
                    }
                    else
                    {
                        // Converting date to srting based on the given format.
                        cellItem = string.IsNullOrEmpty(this.RowInfo.CellTextFormat) ? tempTime.ToString("yyyy") : tempTime.ToString(this.RowInfo.CellTextFormat);
                    }

                    dateCellItem = dateCellWidth;
                    break;
                case TimeUnit.Months:

                    if (isFYNumbering)
                    {
                        cellItem = tempTime.ToString("MMM");
                        cellItem = cellItem + " "+(tempTime.Year + 1).ToString();
                    }
                    else
                    {
                        // Converting date to srting based on the given format.
                        cellItem = string.IsNullOrEmpty(this.RowInfo.CellTextFormat) ? tempTime.ToString("MMM yyyy") : tempTime.ToString(this.RowInfo.CellTextFormat);
                        //cellItem = tempTime.ToString("MMM yyyy");
                    }

                    dateCellItem = dateCellWidth;
                    break;
                case TimeUnit.Weeks:

                    if (isFYNumbering)
                    {
                        cellItem = tempTime.ToString("MMM dd");
                        cellItem = cellItem + " " + (tempTime.Year + 1).ToString();
                    }
                    else
                    {
                        // Converting date to srting based on the given format.
                        cellItem = string.IsNullOrEmpty(this.RowInfo.CellTextFormat) ? tempTime.ToString("MMM dd yyyy") : tempTime.ToString(this.RowInfo.CellTextFormat);
                        //cellItem = tempTime.ToString("MMM dd yyyy");
                    }

                    dateCellItem = AddType(dateCellItem, this.RowInfo.TimeUnit, this.RowInfo.CellsPerUnit);

                     break;
                case TimeUnit.Days:

                    // Converting date to srting based on the given format.
                    cellItem = string.IsNullOrEmpty(this.RowInfo.CellTextFormat) ? tempTime.ToString("ddd").Substring(0, 1) : tempTime.ToString(this.RowInfo.CellTextFormat);
                    //cellItem = tempTime.ToString("ddd").Substring(0, 1);

                    // Increment DateTime
                    dateCellItem = AddType(dateCellItem, this.RowInfo.TimeUnit, this.RowInfo.CellsPerUnit);
                    break;
#if !SILVERLIGHT
                case TimeUnit.Hours:

                    // Converting date to srting based on the given format.
                    cellItem = string.IsNullOrEmpty(this.RowInfo.CellTextFormat) ? tempTime.ToString("HH") : tempTime.ToString(this.RowInfo.CellTextFormat);
                    //cellItem = tempTime.ToString("HH");

                    // Increment DateTime
                    dateCellItem = AddType(dateCellItem, this.RowInfo.TimeUnit, this.RowInfo.CellsPerUnit);
                    break;
                case TimeUnit.Minutes:
                    
                    // Minute Cell Items
                    cellItem = tempTime.Minute.ToString();

                    // Increment Date
                    dateCellItem = AddType(dateCellItem, this.RowInfo.TimeUnit, this.RowInfo.CellsPerUnit);
                    break;
#endif
            }

            return cellItem;
        }

        /// <summary>
        /// Gets the cell tool tip.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <param name="isFirstCell">IsFirstCell.</param>
        /// <returns></returns>
        private object GetCellToolTip(DateTime current, bool isFirstCell)
        {
            DateTime tempTime = current;
            object cellToolTip = string.Empty;
            TimeUnit timeUnit = this.RowInfo.TimeUnit;
            double cellsPerUnit = this.RowInfo.CellsPerUnit;

            switch (timeUnit)
            {
                case TimeUnit.Years:
                    dateCelltooltip = dateCellWidth;

                    // Temp Variable used in Tooltip calculation
                    DateTime celltooltipTime = dateCelltooltip.AddDays(-1);

                    // Default Tool tip for year
                    cellToolTip = tempTime.ToString("MMM dd yyyy") + "-" + celltooltipTime.ToString("MMM dd yyyy");

                    // Tool tip for Fiscal years
                    if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                    {
                        // Checking the start Time and End time of the  year and creates the Fiscal year tooltip accordingly
                        if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn && celltooltipTime.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MMM dd ") + (tempTime.Year + 1) + " - " + celltooltipTime.ToString("MMM dd ") + (celltooltipTime.Year + 1);
                        }
                        else if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn && celltooltipTime.Month < (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MMM dd ") + (tempTime.Year + 1) + " - " + celltooltipTime.ToString("MMM dd yyyy");
                        }
                        else if (tempTime.Month < (int)ParentControl.FiscalYearBeginsOn && celltooltipTime.Month < (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MMM dd yyyy") + " - " + celltooltipTime.ToString("MMM dd yyyy");
                        }
                        else if (tempTime.Month < (int)ParentControl.FiscalYearBeginsOn && celltooltipTime.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MMM dd yyyy") + " - " + celltooltipTime.ToString("MMM dd ") + (celltooltipTime.Year + 1);
                        }
                        else
                            cellToolTip = "FY " + cellToolTip;
                    }
                    break;

                case TimeUnit.Months:
                    cellToolTip = tempTime.ToString("MMMM yyyy");
                    dateCelltooltip = dateCellWidth;

                    // Temp Variable used in Tooltip calculation.
                    DateTime monthcellTooltip = dateCelltooltip.AddDays(-1);

                    if (ParentControl.ScheduleType == ScheduleType.CustomDateTime)
                    {
                        cellToolTip = tempTime.ToString("MM/dd/yyyy") + " - " + monthcellTooltip.ToString("MM/dd/yyyy");
                    }

                    // Generates tooltip for Fiscal year
                    if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                    {
                        // Checking  start Time and end time of a month and generates the fiscal year tooltip accordingly.
                        if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn && monthcellTooltip.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1) + " - " + monthcellTooltip.ToString("MM/dd/") + (monthcellTooltip.Year + 1);
                        }
                        else if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn && monthcellTooltip.Month < (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1) + " - " + monthcellTooltip.ToString("MM/dd/yyyy");
                        }
                        else if (tempTime.Month < (int)ParentControl.FiscalYearBeginsOn && monthcellTooltip.Month < (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/yyyy") + " - " + monthcellTooltip.ToString("MM/dd/yyyy");
                        }
                        else if (tempTime.Month < (int)ParentControl.FiscalYearBeginsOn && monthcellTooltip.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/yyyy") + " - " + monthcellTooltip.ToString("MM/dd/") + (monthcellTooltip.Year + 1);
                        }
                        else
                            cellToolTip = "FY " + cellToolTip;
                    }
                    break;
                case TimeUnit.Weeks:

                    // Tool tip creation for First Week Cell
                    DateTime weekcelltooltip = isFirstCell ? dateCellWidth.AddDays(-1) : (cellsPerUnit > 1 ? tempTime.AddDays(7 * cellsPerUnit - 1) : tempTime.AddDays(6));

                    // checks the given week is in Fiscal year and assigns corresponding values
                    if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                    {
                        // Tool tip for fiscal Week with in a fiscal year
                        if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = ("FY " + dateCelltooltip.ToString("MM/dd/") + (dateCelltooltip.Year + 1) + " - " + weekcelltooltip.ToString("MM/dd/") + (weekcelltooltip.Year + 1)).ToString();
                        }
                        else if (weekcelltooltip.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = ("FY " + tempTime.ToString("MM/dd/yyyy") + " - " + weekcelltooltip.ToString("MM/dd/") + (weekcelltooltip.Year + 1)).ToString();
                        }
                        else
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/yyyy") + "-" + weekcelltooltip.ToString("MM/dd/yyyy");
                        }
                    }
                    else
                    {
                        // Tooltip for general Week
                        cellToolTip = tempTime.ToString("MM/dd/yyyy") + "-" + weekcelltooltip.ToString("MM/dd/yyyy");
                    }

                    dateCelltooltip = AddType(dateCelltooltip, timeUnit, cellsPerUnit);

                    break;

                case TimeUnit.Days:

                    DateTime daycelltooltip = isFirstCell ? dateCellWidth.Subtract(new TimeSpan(1, 0, 0)) : cellsPerUnit > 1 ? tempTime.AddDays(cellsPerUnit).Subtract(new TimeSpan(1, 0, 0)) : tempTime;

                    // Generates tool tip  for fiscal year
                    if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                    {
                        if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            // Generates the tool tip for custom year.
                            if (cellsPerUnit > 1)
                                cellToolTip = "FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1) + " - " + daycelltooltip.ToString("MM/dd/") + (tempTime.Year + 1);
                            else
                                cellToolTip = ("FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1));
                        }
                        else if (daycelltooltip != tempTime && daycelltooltip.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/yyyy") + " - " + daycelltooltip.ToString("MM/dd/") + (tempTime.Year + 1);
                        }
                        else
                        {
                            cellToolTip = "FY " + (cellsPerUnit > 1 ? tempTime.ToString("MM/dd/yyyy") + " - " + daycelltooltip.ToString("MM/dd/yyyy") : tempTime.ToString("MM/dd/yyyy"));
                        }

                    }
                    else
                    {
                        cellToolTip = cellsPerUnit > 1 ? tempTime.ToString("MM/dd/yyyy") + " - " + daycelltooltip.ToString("MM/dd/yyyy") : tempTime.ToString("MM/dd/yyyy");
                    }

                    // Increments date
                    dateCelltooltip = AddType(dateCelltooltip, timeUnit, cellsPerUnit);
                    break;
#if !SILVERLIGHT
                case TimeUnit.Hours:

                    DateTime hourcelltooltip = isFirstCell ? dateCellWidth.Subtract(new TimeSpan(0, 1, 0)) : cellsPerUnit > 1 ? tempTime.AddHours(cellsPerUnit).Subtract(new TimeSpan(0, 1, 0)) : tempTime;

                    // Generates tool tip  for fiscal year
                    if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                    {
                        if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            if (cellsPerUnit > 1)
                                cellToolTip = "FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1) + tempTime.ToString(" hh:mm tt") + " - " +
                                    hourcelltooltip.ToString("MM/dd/") + (hourcelltooltip.Year + 1) + hourcelltooltip.ToString(" hh:mm tt");
                            else
                                cellToolTip = ("FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1) + tempTime.ToString(" hh:mm tt")).ToString();
                        }
                        else if (hourcelltooltip != tempTime && hourcelltooltip.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/yyyy h:mm tt") + " - " +
                                        hourcelltooltip.ToString("MM/dd/") + (hourcelltooltip.Year + 1) + hourcelltooltip.ToString(" hh:mm tt");
                        }
                        else
                            cellToolTip = "FY " + (cellsPerUnit > 1 ? tempTime.ToString("MM/dd/yyyy") + tempTime.ToString(" hh:mm tt") + " - " +
                                hourcelltooltip.ToString("MM/dd/yyyy") + hourcelltooltip.ToString(" hh:mm tt") : tempTime.ToString("MM/dd/yyyy hh:mm tt"));
                    }
                    else
                    {
                        cellToolTip = (cellsPerUnit > 1 ? tempTime.ToString("MM/dd/yyyy h:mm tt") + " - " +
                                hourcelltooltip.ToString("MM/dd/yyyy h:mm tt") : tempTime.ToString("MM/dd/yyyy hh:mm tt"));
                    }

                    // Increment date.
                    dateCelltooltip = AddType(dateCelltooltip, timeUnit, cellsPerUnit);

                    break;
                case TimeUnit.Minutes:

                    DateTime minutecelltooltip = isFirstCell ? dateCellWidth.Subtract(new TimeSpan(0, 0, 1)) : cellsPerUnit > 1 ? tempTime.AddMinutes(cellsPerUnit).Subtract(new TimeSpan(0, 0, 1)) : tempTime;

                    // Generates tool tip  for fiscal year
                    if (ParentControl.IsFYNumberingEnabled && ParentControl.FiscalYearBeginsOn != Month.January)
                    {
                        if (tempTime.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            if (cellsPerUnit > 1)
                                cellToolTip = "FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1) + tempTime.ToString(" hh:mm:ss tt") + " - " +
                                    minutecelltooltip.ToString("MM/dd/") + (minutecelltooltip.Year + 1) + minutecelltooltip.ToString(" hh:mm:ss tt");
                            else
                                cellToolTip = ("FY " + tempTime.ToString("MM/dd/") + (tempTime.Year + 1) + tempTime.ToString(" hh:mm:ss tt")).ToString();
                        }
                        else if (minutecelltooltip != tempTime && minutecelltooltip.Month >= (int)ParentControl.FiscalYearBeginsOn)
                        {
                            cellToolTip = "FY " + tempTime.ToString("MM/dd/yyyy h:mm:ss tt") + " - " +
                                        minutecelltooltip.ToString("MM/dd/") + (minutecelltooltip.Year + 1) + minutecelltooltip.ToString(" hh:mm:ss tt");
                        }
                        else
                            cellToolTip = "FY " + (cellsPerUnit > 1 ? tempTime.ToString("MM/dd/yyyy ") + tempTime.ToString("hh:mm:ss tt") + " - " +
                                minutecelltooltip.ToString("MM/dd/yyyy") + minutecelltooltip.ToString(" hh:mm:ss tt") : tempTime.ToString("MM/dd/yyyy hh:mm:ss tt"));
                    }
                    else
                    {
                        cellToolTip = (cellsPerUnit > 1 ? tempTime.ToString("MM/dd/yyyy h:mm:ss tt") + " - " +
                                minutecelltooltip.ToString("MM/dd/yyyy h:mm:ss tt") : tempTime.ToString("MM/dd/yyyy hh:mm:ss tt"));
                    }

                    // Increments Minutes
                    dateCelltooltip = AddType(dateCelltooltip, timeUnit, cellsPerUnit);
                    break;
#endif
            }
            return cellToolTip;
        }

        #endregion

        #region Cell width Calculation

        /// <summary>
        /// Gets the default widhtof date time cell.
        /// </summary>
        /// <returns></returns>
        internal double GetDefaultWidhtOfDateTimeCell()
        {
            if (this.RowInfo == null)
                return 0;

            if (this.RowInfo.TimeUnit == TimeUnit.Weeks)
            {
                return (ParentControl.DayWidth * 7 * this.RowInfo.CellsPerUnit);
            }
            else if (this.RowInfo.TimeUnit == TimeUnit.Days)
            {
                return (ParentControl.DayWidth * this.RowInfo.CellsPerUnit);
            }
#if !SILVERLIGHT
            else if (this.RowInfo.TimeUnit == TimeUnit.Hours)
            {
                return ((ParentControl.DayWidth / 24) * this.RowInfo.CellsPerUnit);
            }
            else if (this.RowInfo.TimeUnit == TimeUnit.Minutes)
            {
                return ((ParentControl.DayWidth / (60 * 24)) * this.RowInfo.CellsPerUnit);
            }
#endif
            return 0;
        }

        /// <summary>
        /// Gets the width of the standard year.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        /// <returns></returns>
        private double GetStandardYearWidth(DateTime currentTime)
        {
            int totalDaysinYear = DateTime.IsLeapYear(currentTime.Year) ? 366 : 365;
            int availableDays = totalDaysinYear - currentTime.DayOfYear + 1;
            dateCellWidth = dateCellWidth.AddDays(availableDays);

            return availableDays * ParentControl.DayWidth;
        }

        /// <summary>
        /// Gets the width of the year.
        /// </summary>
        /// <param name="current">The current.</param>
        /// <returns></returns>
        private double GetCustomYearWidth(DateTime current)
        {
            // temp storage of start time
            DateTime startDate = current;
            double cellsPerUnit = this.RowInfo.CellsPerUnit;

            // Calculates the Round year, remaining year from cells per unit
            cellsPerUnit = Math.Round(cellsPerUnit - remainYears, 1);
            int roundedYear = (int)Math.Floor(cellsPerUnit);
            double remYear = Math.Round(cellsPerUnit - roundedYear, 1);
            double totaldays = 0;
            if (!IsYearRounded)
            {
                if (roundedYear > 0)
                {
                    for (int i = 1; i <= roundedYear; i++)
                    {
                        totaldays = this.GetTotalDays(1, startDate.Year);
                        if (i == 1)
                            startDate = startDate.AddDays(totaldays - current.DayOfYear + 1);
                        else
                            startDate = startDate.AddDays(totaldays);
                    }
                }

                if (remYear > 0)
                {
                    double additionalDays = this.GetTotalDays(remYear, startDate.Year);
                    startDate = startDate.AddDays(additionalDays);
                    remainYears = 1 - remYear;
                }
                dateCellWidth = startDate;
                return (startDate.Subtract(current).TotalDays * ParentControl.DayWidth);

            }
            else
            {
                if (remainYears > 0 && remainYears < 1)
                {
                    double addDays = this.GetTotalDays(remainYears, dateCellWidth.Year);
                    dateCellWidth = dateCellWidth.AddDays(addDays);
                    remainYears = 0;
                }
                if (roundedYear > 0)
                {
                    for (int i = 1; i <= roundedYear; i++)
                    {
                        totaldays = this.GetTotalDays(1, dateCellWidth.Year);
                        dateCellWidth = dateCellWidth.AddDays(totaldays);
                    }
                }

                if (remYear > 0)
                {
                    double additionalDays = this.GetTotalDays(remYear, dateCellWidth.Year);
                    dateCellWidth = dateCellWidth.AddDays(additionalDays);
                    remainYears = 1 - remYear;
                }

                return (dateCellWidth.Subtract(current).TotalDays * ParentControl.DayWidth);
            }
        }

        /// <summary>
        /// Gets the width of the standard month.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        /// <returns></returns>
        private double GetStandardMonthWidth(DateTime currentTime)
        {
            // Calculations for Standard Month.
            int totalDaysInMonth = DateTime.DaysInMonth(currentTime.Year, currentTime.Month);
            int availableDays = totalDaysInMonth - currentTime.Day + 1;
            dateCellWidth = dateCellWidth.AddDays(availableDays);

            return availableDays * ParentControl.DayWidth;
        }

        /// <summary>
        /// Gets the width of the custom month.
        /// </summary>
        /// <param name="currentTime">The current time.</param>
        /// <returns></returns>
        private Double GetCustomMonthWidth(DateTime currentTime)
        {
            //Gets the current start date
            DateTime currentStart = dateCellWidth;

            // Calculates additional and remaining Month required
            double additionalMonth = this.RowInfo.CellsPerUnit;
            double remMonth = additionalMonth - Math.Floor(additionalMonth);

            // Calculates total days in the required Month
            for (Double i = 0.0; i < additionalMonth; i += 0.25)
            {
                int totalDaysInMonth = DateTime.DaysInMonth(dateCellWidth.Year, dateCellWidth.Month);
                dateCellWidth = dateCellWidth.AddDays(totalDaysInMonth * 0.25);
            }
            return dateCellWidth.Subtract(currentStart).TotalDays;
        }

        /// <summary>
        /// Gets the total days in year.
        /// </summary>
        /// <param name="Num">The Partition of the year.</param>
        /// <param name="year">The year.</param>
        /// <returns>Number of days in Year</returns>
        private double GetTotalDays(double Num, int year)
        {
            if (DateTime.IsLeapYear(year))
                return Num * 366;
            else
                return Num * 365;
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Gets the rounded date.
        /// </summary>
        /// <param name="tempWidthDate">The temp width date.</param>
        /// <param name="timeUnit">The time unit.</param>
        /// <returns></returns>
        private DateTime GetRoundedDate(DateTime tempWidthDate, TimeUnit timeUnit)
        {
            if (timeUnit == TimeUnit.Months)
            {
                // To rounding date based on preceding units
                tempWidthDate = GetRoundedDate(tempWidthDate, TimeUnit.Days);

                // Calculate the extra available days to make the rounded date.
                int availableDays = tempWidthDate.Day == 1 ? 0 : DateTime.DaysInMonth(tempWidthDate.Year, tempWidthDate.Month) - (int)tempWidthDate.Day;
                // Adding the available days to round the date.
                return tempWidthDate.AddDays(availableDays);
            }
            else if (timeUnit == TimeUnit.Weeks)
            {
                // To rounding date based on preceding units
                tempWidthDate = GetRoundedDate(tempWidthDate, TimeUnit.Days);

                // Calculate the extra available days to make the rounded date.
                int availableDays = (int)tempWidthDate.DayOfWeek == 0 ? 0 : 7 - (int)tempWidthDate.DayOfWeek;
                // Adding the available days to round the date.
                return tempWidthDate.AddDays(availableDays);
            }
            else if (timeUnit == TimeUnit.Days)
            {
#if !SILVERLIGHT
                // To rounding date based on preceding units
                tempWidthDate = GetRoundedDate(tempWidthDate, TimeUnit.Hours);
#endif
                // Calculate the extra available hours to make the rounded date.
                int availableHours = tempWidthDate.Hour == 0 ? 0 : 24 - tempWidthDate.Hour;
                // Adding the available hours to round the date.
                return tempWidthDate.AddHours(availableHours);
            }
#if !SILVERLIGHT
            else if (timeUnit == TimeUnit.Hours)
            {
                // To rounding date based on preceding units
                tempWidthDate = GetRoundedDate(tempWidthDate, TimeUnit.Minutes);

                // Calculate the extra available minutes to make the rounded date.
                int availableminutes = tempWidthDate.Minute == 0 ? 0 : 60 - tempWidthDate.Minute;
                // Adding the available minutes to round the date.
                return tempWidthDate.AddMinutes(availableminutes);
            }
            else if (timeUnit == TimeUnit.Minutes)
            {
                // Calculate the extra available seconds to make the rounded date.
                int availableSeconds = tempWidthDate.Second == 0 ? 0 : 60 - tempWidthDate.Second;
                // Adding the available seconds to round the date.
                return tempWidthDate.AddSeconds(availableSeconds);
            }
#endif
            return tempWidthDate;
        }

        /// <summary>
        /// Adds the Corresponding Time Unit
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="type">The type.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>Added Value</returns>
        internal DateTime AddType(DateTime instance, TimeUnit type, double increment)
        {
            DateTime result = instance;
            switch (type)
            {
                case TimeUnit.Weeks:
                    result = instance.AddMinutes(increment * 7 * 24 * 60);
                    break;
                case TimeUnit.Days:
                    result = instance.AddMinutes(increment * 24 * 60);
                    break;
#if !SILVERLIGHT
                case TimeUnit.Hours:
                    result = instance.AddMinutes(increment * 60);
                    break;
                case TimeUnit.Minutes:
                    result = instance.AddMinutes(increment);
                    break;
#endif
            }

            return result;
        }

        /// <summary>
        /// Refreshes the Schedule Row Items
        /// </summary>
        public void RefreshItems()
        {
            if (this.ItemsPresenter == null)
                return;

            this.ItemsPresenter.Children.Clear();
            this.ItemsPresenter.Width = 0d;

            if (ParentControl.ScheduleType == ScheduleType.CustomNumeric)
            {
                // Resets the width
                ParentControl.Width = ((this.EndPoint - this.StartPoint) * ParentControl.LowerCellWidth);

                // Initializing values to form numeric cells
                numericCellItem = (ParentControl.Start * (ParentControl.LowerCellWidth));
                numericCellTooltip = (ParentControl.Start * (ParentControl.LowerCellWidth));

                // To generate numeric cells with initialized values
                GenerateNumericCells(this.ParentControl.Width, 0);
            }
            // Assigning Values to variables used for DateTime Schedule
            else
            {
                if (this.EndTime < this.StartTime)
                    return;

                // Resets the width
                ParentControl.Width = ((this.EndTime.Subtract(this.StartTime).TotalDays) * ParentControl.DayWidth);

                // Initializing values to form DateTime Cells
                dateCellItem = ParentControl.StartTime;
                dateCellWidth = ParentControl.StartTime;
                dateCelltooltip = ParentControl.StartTime;

                // Setting the initial Requirements for dateTime cells
                this.IsYearRounded = false;
                ParentControl.IsWeekBeginsOnSet = true;

                if (!this.ParentControl.IsUIVirtualizaionEnabled)
                    // Generates the Datetime cells with the initialized values
                    this.GenerateDateTimeCells(this.ParentControl.Width, false);
                else
                    this.UpdateDateTimeScrollInfo(true);
            }
        }

        #endregion

        #region Drag & Drop beyound region

        /// <summary>
        /// Adds the next DateTime Cell items that is not in the schedule cells.
        /// </summary>
        private void AddContinuousItems(DateTime oldEndTime, DateTime newEndTime)
        {
            // Calculating new width based on changes in the date
            double newWidth = ((this.EndTime.Subtract(this.StartTime).TotalDays) * ParentControl.DayWidth);

            // Validating the new width
            if (newWidth < 0 || this.ItemsPresenter.Children.Count <= 0)
                return;

            // Resets the Width
            ParentControl.Width = newWidth;

            //In Silverlight when we adding the Task to InbuiltTaskCollection in Gantt loaded event, sometimes newEndTime value is lesser the Old value.
            //Hence, we get negative value when we substarct the newEndTime with OldEndtime to prevent this we made the check here.
            double additionalWidth = newEndTime.Subtract(oldEndTime).TotalDays>0?newEndTime.Subtract(oldEndTime).TotalDays * this.ParentControl.DayWidth:0;
            
            GanttScheduleCell lastCell = this.ItemsPresenter.Children[this.ItemsPresenter.Children.Count - 1] as GanttScheduleCell;

            // checking for the cell width to modify the width of first cell instead of creating the cell again
            if (lastCell.ActualWidth + additionalWidth <= this.ScheduleCellWidth)
            {
                // assigning the new values in the first cell
                lastCell.Width = lastCell.ActualWidth + additionalWidth;
                return;
            }

            this.ItemsPresenter.Children.Remove(lastCell);
            // Since we have removed the first cell we are adding its width to the available width
            additionalWidth += lastCell.ActualWidth;

            // Initializes the Required Variables.
            dateCellWidth = lastCell.CellDate;
            dateCellItem = lastCell.CellDate;
            dateCelltooltip = lastCell.CellDate;

            // Generating the cells for the aditional width
            this.GenerateDateTimeCells(additionalWidth, false);
        }

        /// <summary>
        /// Adds the previous DateTime Cell items.
        /// </summary>
        private void AddPreviousItems(DateTime oldStartTime, DateTime newStartTime)
        {
            // Calculating new width based on changes in the date
            double newWidth = ((this.EndTime.Subtract(this.StartTime).TotalDays) * ParentControl.DayWidth);

            // Validating the new width
            if (newWidth < 0)
                return;

            // Resets the Width
            ParentControl.Width = newWidth;

            // Getting the extend width from the date
            double additionalWidth = oldStartTime.Subtract(newStartTime).TotalDays * this.ParentControl.DayWidth;
            GanttScheduleCell firstCell = this.ItemsPresenter.Children[0] as GanttScheduleCell;

            // checking for the cell width to modify the width of first cell instead of creating the cell again
            if (firstCell.ActualWidth + additionalWidth <= this.ScheduleCellWidth)
            {
                // assigning the new values in the first cell
                firstCell.Width = firstCell.ActualWidth + additionalWidth;
                firstCell.CellDate = newStartTime;
                firstCell.Content = this.GetCellItem(newStartTime);
                firstCell.CellToolTip = GetCellToolTip(newStartTime, true);
                return;
            }

            this.ItemsPresenter.Children.Remove(firstCell);
            // Since we have removed the first cell we are adding its width to the available width
            additionalWidth += firstCell.ActualWidth;

            // Initializing Required Variables
            dateCellWidth = this.StartTime;
            dateCellItem = this.StartTime;
            dateCelltooltip = this.StartTime;

            // Generating the cells for the aditional width
            this.GenerateDateTimeCells(additionalWidth, true);
        }

        #endregion

#if SILVERLIGHT
        #region  Clipping
        
        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // To provide the sharp edge the clip is added. To avoid exception following conditions are added.
            if (!this.DesiredSize.IsEmpty && !double.IsNaN(this.ParentControl.Width) && !double.IsInfinity(this.ParentControl.Width) && this.ParentControl.Width > 0 &&
                 !double.IsNaN(this.DesiredSize.Height) && !double.IsInfinity(this.DesiredSize.Height) && this.DesiredSize.Height > 0)
                // To provide the sharp edge the clip is added
                this.Clip = new System.Windows.Media.RectangleGeometry { Rect = new Rect(0, 0, this.ParentControl.Width, this.DesiredSize.Height) };

            return base.ArrangeOverride(finalSize);
        }
        #endregion
#endif

        #endregion
    }
}