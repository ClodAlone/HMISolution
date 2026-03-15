// <copyright file="DayGrid.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Calendar = System.Globalization.Calendar;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a grid that consists of columns and rows which
    /// contain day cell elements.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class DayGrid : CalendarEditGrid
    {
        #region Constants

        /// <summary>
        /// Default number of rows.
        /// </summary>
        internal const int DEFROWSCOUNT = 6;

        /// <summary>
        /// Default number of columns.
        /// </summary>
        internal const int DEFCOLUMNSCOUNT = 7;

        #endregion Constants

        #region Private members

        /// <summary>
        /// Selection border.
        /// </summary>
        private Border mselectionBorder;

        /// <summary>
        /// Matrix that represents days for the current date and culture.
        /// </summary>
        private int[,] mcalendarMatrix;

        /// <summary>
        /// The parent instance.
        /// </summary>
        private CalendarEdit mparentCalendar;

        /// <summary>
        /// Collection of Date/DayCell pairs.
        /// </summary>
        private Hashtable mdateCells;

        /// <summary>
        /// Collection of week numbers.
        /// </summary>
        private List<int> mweekNumbers = null;

        /// <summary>
        /// List of week number cells.
        /// </summary>
        private List<WeekNumberCell> mweekNumberCells = null;

        /// <summary>
        /// Contains index and old tooltip of the cell. Is used to restore old tooltip to the cell.
        /// </summary>
        private Hashtable moldTooltipIndexes = new Hashtable();

        /// <summary>
        /// Contains index and new date of the cell. Is used to restore new tooltip to the cell.
        /// </summary>
        private Hashtable mnewTooltipDates = new Hashtable();

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes static members of the DayGrid class.  It overrides some dependency properties.
        /// </summary>
        static DayGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DayGrid), new FrameworkPropertyMetadata(typeof(DayGrid)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DayGrid"/> class.
        /// </summary>
        public DayGrid()
            : base(DEFROWSCOUNT, DEFCOLUMNSCOUNT)
        {
            this.mweekNumbers = new List<int>();
            this.SelectionBorder = new Border();
            this.DateCells = new Hashtable();
            this.SelectionBorder.Opacity = 0;
            Grid.SetRowSpan((UIElement)this.SelectionBorder, RowsCount);
            AddToInnerGrid((UIElement)this.SelectionBorder);
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets week number cells list.
        /// </summary>
        internal List<WeekNumberCell> WeekNumbers
        {
            get
            {
                return this.mweekNumberCells;
            }
        }

        /// <summary>
        /// Gets or sets the collection of Date/DayCell pairs.
        /// </summary>
        /// <value>
        /// Type: <see cref="Hashtable"/>
        /// </value>
        /// <seealso cref="Hashtable"/>
        protected internal Hashtable DateCells
        {
            get
            {
                return this.mdateCells;
            }

            set
            {
                this.mdateCells = value;
            }
        }

        /// <summary>
        /// Gets or sets the matrix that represents days for the current date and culture.
        /// </summary>
        /// <value>
        /// Type: <see cref="Syncfusion.Windows.Shared.DayGrid.CalendarMatrix"/>
        /// </value>
        protected internal int[,] CalendarMatrix
        {
            get
            {
                return this.mcalendarMatrix;
            }

            set
            {
                this.mcalendarMatrix = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// Type: <see cref="CalendarEdit"/>
        /// </value>
        /// <seealso cref="CalendarEdit"/>
        protected internal CalendarEdit ParentCalendar
        {
            get
            {
                return this.mparentCalendar;
            }

            set
            {
                this.mparentCalendar = value;
            }
        }

        /// <summary>
        /// Gets or sets the selection border.
        /// </summary>
        /// <value>
        /// Type: <see cref="Border"/>
        /// </value>
        /// <seealso cref="Border"/>
        protected internal Border SelectionBorder
        {
            get
            {
                return this.mselectionBorder;
            }

            set
            {
                this.mselectionBorder = value;
            }
        }

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Initializes the content of DayGrid.
        /// </summary>
        /// <param name="data">Visible information.</param>
        /// <param name="culture">Current culture.</param>
        /// <param name="calendar">Current calendar.</param>
        public override void Initialize(VisibleDate data, CultureInfo culture, Calendar calendar)
        {
            if (this.ParentCalendar != null)
            {
                DateTimeFormatInfo format = culture.DateTimeFormat;
                VisibleDate visibleData = data;
                this.SetDayCellDate(data, format, calendar);
                this.SetDayCellContent();
                this.SetIsCurrentMonth(data.VisibleMonth);
                this.SetIsToday(calendar);
                this.SetIsFirstDayofMonth();
                this.UpdateDateCells(calendar, visibleData.VisibleMonth);
                this.SetIsDate(calendar);
                this.SetIsSelected();
                this.SetIsInvalid();
                this.ParentCalendar.InitilizeDayCellTemplates(this);
                this.ParentCalendar.InitilizeDayCellStyles(this);

                if (Visibility == Visibility.Hidden)
                {
                    for (int i = 0; i < CellsCollection.Count; i++)
                    {
                        DayCell dc = (DayCell)CellsCollection[i];

                        if (dc.IsDate)
                        {
                            FocusedCellIndex = i;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets tooltip to the day cell.
        /// </summary>
        /// <param name="dc">Day cell to set tooltip to.</param>
        /// <param name="index">Index of the cell in the CellsCollection.</param>
        internal void SetDayCellToolTip(DayCell dc, int index)
        {
            if (this.moldTooltipIndexes.Count > 0 && this.IsInTooltipIndexArrays(index))
            {
                this.SetOldCellTooltip(dc, index);
            }

            Hashtable toolTipDates = this.ParentCalendar.TooltipDates;

            if (toolTipDates.Count > 0 && toolTipDates.ContainsKey(dc.Date))
            {
                this.SetNewCellTooltip(dc, toolTipDates, index);
            }
        }

        /// <summary>
        /// Sets the selected cell.
        /// </summary>
        protected internal void SetIsSelected()
        {
            bool result = false;

            if (this.ParentCalendar.SelectedDatesList.Count != 0)
            {
                if (this.ParentCalendar.SelectedDatesList.Count == 1 || this.ParentCalendar.AllowMultiplySelection == true)
                {
                    foreach (DayCell dc in CellsCollection)
                    {
                        int foundIndex = this.ParentCalendar.SelectedDatesList.BinarySearch(dc.Date);

                        if (foundIndex >= 0)
                        {
                            result = true;
                        }

                        dc.IsSelected = result;
                        result = false;
                    }
                }
                else
                {
                    this.ParentCalendar.SelectedDatesList.Clear();
                }
            }

            else
            {
                foreach (DayCell dc in CellsCollection)
                {
                    dc.IsSelected = false;
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        protected internal void SetIsInvalid()
        {
            bool result = false;
            if (this.ParentCalendar.InvalidDates.Count != 0)
            {
                foreach (DayCell dc in CellsCollection)
                {
                    int foundIndex = this.ParentCalendar.InvalidDates.BinarySearch(dc.Date);

                    if (foundIndex >= 0)
                    {
                        result = true;
                    }

                    dc.IsInvalidDate = result;

                    result = false;
                }
            }

            else
            {
                foreach (DayCell dc in CellsCollection)
                {
                    dc.IsInvalidDate = false;
                }
            }

            //this.ParentCalendar.InvalidDates.Clear();
        }

        /// <summary>
        /// Sets <see cref="DayCell.IsDate"/> to true if the cell date is equal to
        /// the <see cref="CalendarEdit.Date"/>.
        /// </summary>
        /// <param name="calendar">The <see cref="System.Globalization.Calendar"/> for date transformation.</param>
        protected internal void SetIsDate(Calendar calendar)
        {
            Date date = new Date(this.ParentCalendar.Date, calendar);

            foreach (DayCell dc in CellsCollection)
            {
                if (dc.Date == date)
                {
                    dc.IsDate = true;
                }
                else
                {
                    dc.IsDate = false;
                }
            }
        }

        /// <summary>
        /// Sets <see cref="DayCell.IsFirstDayofMonth"/> to true if the cell date is equal to
        /// FirstDay of the Month
        /// </summary>
        /// <param name="calendar">The <see cref="System.Globalization.Calendar"/> for date transformation.</param>
        protected internal void SetIsFirstDayofMonth()
        {
            foreach (DayCell dc in CellsCollection)
            {
                if (dc.Date.Day == 1)
                {
                    dc.IsFirstDayofMonth = true;
                }
                else
                {
                    dc.IsFirstDayofMonth = false;
                }
            }
        }

        /// <summary>
        /// Sets <see cref="DayCell.IsToday"/> to true if the cell date is equal to
        /// today's date.
        /// </summary>
        /// <param name="calendar">The <see cref="System.Globalization.Calendar"/> for date transformation.</param>
        protected internal void SetIsToday(Calendar calendar)
        {
            Date today = new Date(DateTime.Now, calendar);

            foreach (DayCell dc in CellsCollection)
            {
                if (dc.Date == today)
                {
                    dc.IsToday = true;
                }
                else
                {
                    dc.IsToday = false;
                }
            }
        }

        /// <summary>
        /// Sets <see cref="DayCell.IsCurrentMonth"/> to true if the cell date belongs to
        /// the current visible month.
        /// </summary>
        /// <param name="month">Current month.</param>
        protected internal void SetIsCurrentMonth(int month)
        {
            foreach (DayCell dc in CellsCollection)
            {
                if (dc.Date.Month == month)
                {
                    dc.IsCurrentMonth = true;
                }
                else
                {
                    dc.IsCurrentMonth = false;
                }
            }
        }

        /// <summary>
        /// Sets the <see cref="SetDayCellContent"/> property of the <see cref="Syncfusion.Windows.Shared.DayCell"/>.
        /// </summary>
        protected internal void SetDayCellContent()
        {
            foreach (DayCell dc in CellsCollection)
            {
                dc.Content = dc.Date;
            }
        }

        /// <summary>
        /// Sets the day cell date.
        /// </summary>
        /// <param name="data">The data value.</param>
        /// <param name="format">The date format.</param>
        /// <param name="calendar">The calendar control.</param>
        protected internal void SetDayCellDate(VisibleDate data, DateTimeFormatInfo format, Calendar calendar)
        {
            int month = data.VisibleMonth;
            int year = data.VisibleYear;
            DateTime dateMonthStart = DateUtils.GetFirstDayOfMonth(year, month, calendar);
            DayOfWeek dw = calendar.GetDayOfWeek(dateMonthStart);
            int ifirstDayOfMonth = (int)dw;
            int ifirstDayOfWeek = (int)format.FirstDayOfWeek;
            int idaysInMonth = calendar.GetDaysInMonth(year, month);
            Date resultDate = new Date();
            int first = ((6 + (ifirstDayOfMonth - ifirstDayOfWeek)) % 7) + 1;
            this.CalendarMatrix = DateUtils.GenerateMatrix(month, year, format, calendar);

            for (int i = 0; i < CellsCollection.Count; i++)
            {
                int irow = Grid.GetRow((UIElement)CellsCollection[i]);
                int icolumn = Grid.GetColumn((UIElement)CellsCollection[i]);

                // current month
                if ((i >= first) && (i < idaysInMonth + first))
                {
                    resultDate.Year = year;
                    resultDate.Month = month;
                    resultDate.Day = this.CalendarMatrix[irow, icolumn];
                }

                // month before
                if (i < first)
                {
                    if (month == 1)
                    {
                        resultDate.Year = year - 1;
                    }
                    else
                    {
                        resultDate.Year = year;
                    }
                    if (resultDate.Year < calendar.MinSupportedDateTime.Year)
                        resultDate.Year = calendar.MaxSupportedDateTime.Year;
                    resultDate.Month = DateUtils.AddMonth(month, -1);
                    resultDate.Day = this.CalendarMatrix[irow, icolumn];
                }

                // month after current
                if (i >= idaysInMonth + first)
                {
                    if (month == 12)
                    {
                        resultDate.Year = year + 1;
                    }
                    else
                    {
                        resultDate.Year = year;
                    }

                    resultDate.Month = DateUtils.AddMonth(month, 1);
                    resultDate.Day = this.CalendarMatrix[irow, icolumn];
                }

                Date maxDate;
                Date minDate;

                if (this.ParentCalendar.MinMaxHidden)
                {
                    maxDate = new Date(this.ParentCalendar.MaxDate, calendar);
                    minDate = new Date(this.ParentCalendar.MinDate, calendar);
                }
                else
                {
                    maxDate = new Date(this.ParentCalendar.mxDate, calendar);
                    minDate = new Date(this.ParentCalendar.miDate, calendar);
                }

                DayCell dc = (DayCell)CellsCollection[i];

                if (this.ParentCalendar.MinMaxHidden)
                {
                    if (resultDate > maxDate || resultDate < minDate)
                    {
                        dc.Visibility = Visibility.Hidden;
                        dc.Date = new Date(0, 0, 0);
                    }
                    else
                    {
                        if (dc.Visibility == Visibility.Hidden)
                        {
                            dc.Visibility = Visibility.Visible;
                        }
                        dc.Opacity = 1;
                        dc.IsEnabled = true;
                        dc.Date = resultDate;
                    }
                }
                else
                {
                    if (dc.Visibility == Visibility.Hidden)
                    {
                        dc.Visibility = Visibility.Visible;
                    }

                    Date maxDate1 = new Date(this.ParentCalendar.MaxDate, calendar);
                    Date minDate1 = new Date(this.ParentCalendar.MinDate, calendar);
                    if (resultDate > maxDate1 || resultDate < minDate1)
                    {
                        dc.IsEnabled = false;
                        dc.Opacity = 0.5;
                        dc.Date = resultDate;
                    }
                    else
                    {
                        dc.IsEnabled = true;
                        dc.Opacity = 1;
                        dc.Date = resultDate;
                    }
                }

                this.SetDayCellToolTip(dc, i);
            }
        }

        /// <summary>
        /// Updates data template and data template selector on all
        /// cells.
        /// </summary>
        /// <param name="template">Data template to be set to the
        /// cell. If it is null the local value of data template would be
        /// cleared.</param>
        /// <param name="selector">Data template selector to be set
        /// to the cell. If it is null the local value would be cleared.</param>
        /// <param name="dateTemplates">Collection of date/template
        /// pairs.</param>
        /// <remarks>
        /// Both template and selector can not be set at the same time.
        /// Cells that have their date in the date/template list will not
        /// be updated.
        /// </remarks>
        protected internal void UpdateTemplateAndSelector(DataTemplate template, DataTemplateSelector selector, DataTemplatesDictionary dateTemplates)
        {
            if (template != null && selector != null)
            {
                throw new ArgumentException("Both template and selector can not be set at one time.");
            }

            if (this.ParentCalendar == null)
            {
                Debug.WriteLine("Data template and selector update skipped: parent calendar has not been found.");
                return;
            }

            Calendar calendar = this.ParentCalendar.Calendar;

            if (dateTemplates == null)
            {
                foreach (DayCell dc in CellsCollection)
                {
                    dc.UpdateCellTemplateAndSelector(template, selector);
                }

                return;
            }

            foreach (DayCell dc in CellsCollection)
            {
                if (dc.Visibility != Visibility.Hidden)
                {
                    DateTime date = dc.Date.ToDateTime(calendar);

                    if (!dateTemplates.Contains(date))
                    {
                        dc.UpdateCellTemplateAndSelector(template, selector);
                    }
                    else
                    {
                        dc.SetTemplate((DataTemplate)dateTemplates[date]);
                    }
                }
            }
        }

        /// <summary>
        /// Updates styles on all cells.
        /// </summary>
        /// <param name="style">Style to be set.</param>
        /// <param name="dateStyles">Collection of date/style pairs.</param>
        /// <remarks>
        /// Date styles have higher priority than style.
        /// </remarks>
        protected internal void UpdateStyles(Style style, StylesDictionary dateStyles)
        {
            if (this.ParentCalendar == null)
            {
                Debug.WriteLine("Data styles update skipped: parent calendar has not been found.");
                return;
            }

            Calendar calendar = this.ParentCalendar.Calendar;

            if (dateStyles == null)
            {
                foreach (DayCell dc in CellsCollection)
                {
                    dc.SetStyle(style);
                }

                return;
            }

            foreach (DayCell dc in CellsCollection)
            {
                if (dc.Visibility != Visibility.Hidden)
                {
                    DateTime date = dc.Date.ToDateTime(calendar);

                    if (!dateStyles.Contains(date))
                    {
                        dc.SetStyle(style);
                    }
                    else
                    {
                        dc.SetStyle((Style)dateStyles[date]);
                    }
                }
            }
        }

        #region Overrides

        /// <summary>
        /// Invoked when the parent is changed.
        /// </summary>
        /// <param name="oldParent">The previous parent. Set to a null
        /// reference (Nothing in Visual Basic) if the DependencyObject did not have
        /// a previous parent.</param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            this.UpdateParent();
            this.Initialize(this.ParentCalendar.VisibleData, this.ParentCalendar.Culture, this.ParentCalendar.Calendar);
            this.ParentCalendar.WeekNumbersGrid.SetWeekNumbers(this.WeekNumbers);
        }

        /// <summary>
        /// Creates the instance of the single cell.
        /// </summary>
        /// <returns>
        /// New instance of the cell.
        /// </returns>
        protected override Cell CreateCell()
        {
            return new DayCell();
        }

        #endregion Overrides

        /// <summary>
        /// Updates the parent.
        /// </summary>
        private void UpdateParent()
        {
            if (this.Parent == null)
            {
                throw new NullReferenceException("Parent is null");
            }

            CalendarEdit calendar = this.Parent as CalendarEdit;

            if (calendar == null)
            {
                throw new NotSupportedException("Parent must be inherited from CalendarEdit type.");
            }

            this.ParentCalendar = calendar;
        }

        /// <summary>
        /// Updates <see cref="DateCells"/> collection in accordance to the calendar.
        /// </summary>
        /// <param name="calendar">The object of calendar class.</param>
        /// <param name="visibleMonth">The visible month.</param>
        private void UpdateDateCells(Calendar calendar, int visibleMonth)
        {
            this.DateCells.Clear();
            this.mweekNumbers.Clear();

            foreach (DayCell dayCell in CellsCollection)
            {
                if (dayCell.Visibility != Visibility.Hidden)
                {
                    DateTime date = dayCell.Date.ToDateTime(calendar);
                    int number = this.ParentCalendar.GetWeekNumber(date);

                    if (date.DayOfWeek == DayOfWeek.Thursday && !this.mweekNumbers.Contains(number))
                    {
                        this.mweekNumbers.Add(number);
                    }

                    this.DateCells.Add(date, dayCell);

                    this.HideNextMonthDays(dayCell, visibleMonth);
                    this.HidePrevMonthDays(dayCell, visibleMonth);
                }
            }

            this.FillWeekNumberCells();
        }

        /// <summary>
        /// Gets next month after current.
        /// </summary>
        /// <param name="visibleMonth">current visible month</param>
        /// <returns>First month after current month</returns>
        private int GetNextMonth(int visibleMonth)
        {
            int nextMonth = visibleMonth + 1;

            if (visibleMonth == 12)
            {
                nextMonth = 1;
            }

            return nextMonth;
        }

        /// <summary>
        /// Gets previous month before current.
        /// </summary>
        /// <param name="visibleMonth">current visible month</param>
        /// <returns>First month before current month</returns>
        private int GetPrevMonth(int visibleMonth)
        {
            int prevMonth = visibleMonth - 1;

            if (visibleMonth == 1)
            {
                prevMonth = 12;
            }

            return prevMonth;
        }

        /// <summary>
        /// Hides previous month days in the visible day grid if ShowPreviousMonthDays is false.
        /// </summary>
        /// <param name="dayCell">day cell to show</param>
        /// <param name="visibleMonth">current visible month</param>
        private void HidePrevMonthDays(DayCell dayCell, int visibleMonth)
        {
            if (!this.ParentCalendar.ShowPreviousMonthDays)
            {
                if (dayCell.Date.Month == this.GetPrevMonth(visibleMonth))
                {
                    dayCell.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Hides next month days in the visible day grid if ShowNextMonthDays is false.
        /// </summary>
        /// <param name="dayCell">day cell to show</param>
        /// <param name="visibleMonth">current visible month</param>
        private void HideNextMonthDays(DayCell dayCell, int visibleMonth)
        {
            if (!this.ParentCalendar.ShowNextMonthDays)
            {
                if (dayCell.Date.Month == this.GetNextMonth(visibleMonth))
                {
                    dayCell.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        /// Fills m_weekNumberCells list.
        /// </summary>
        private void FillWeekNumberCells()
        {
            this.mweekNumberCells = new List<WeekNumberCell>();

            foreach (int number in this.mweekNumbers)
            {
                WeekNumberCell wnc = new WeekNumberCell();
                wnc.Content = number;

                this.mweekNumberCells.Add(wnc);
            }
        }

        /// <summary>
        /// Sets new tooltip to the cell.
        /// </summary>
        /// <param name="dc">Cell to set tooltip to.</param>
        /// <param name="toolTipDates">Hash table that contains tooltips set to the dates (not to the cells).</param>
        /// <param name="index">Index of the cell in the CellsCollection.</param>
        private void SetNewCellTooltip(DayCell dc, Hashtable toolTipDates, int index)
        {
            if (!this.moldTooltipIndexes.Contains(index))
            {
                this.moldTooltipIndexes.Add(index, dc.ToolTip);
            }

            ToolTip toolTip = toolTipDates[dc.Date] as ToolTip;
            dc.ToolTip = toolTip;

            if (!this.mnewTooltipDates.Contains(index))
            {
                this.mnewTooltipDates.Add(index, dc.Date);
            }
        }

        /// <summary>
        /// Sets old tooltip to the cell.
        /// </summary>
        /// <param name="dc">Cell to set tooltip to.</param>
        /// <param name="index">Index of the cell in the CellsCollection.</param>
        private void SetOldCellTooltip(DayCell dc, int index)
        {
            Date newDate = (Date)this.mnewTooltipDates[index];

            if (dc.Content != null && (Date)dc.Content == newDate)
            {
                ToolTip toolTip = this.moldTooltipIndexes[index] as ToolTip;
                dc.ToolTip = toolTip;

                this.mnewTooltipDates.Remove(index);
                this.moldTooltipIndexes.Remove(index);
            }
        }

        /// <summary>
        /// Gets if index of the cell in CellCollection is in m_oldTooltipIndexes and m_newTooltipDates.
        /// </summary>
        /// <param name="index">Index of the cell in the CellsCollection.</param>
        /// <returns>True if index of the cell in CellCollection is in m_oldTooltipIndexes and m_newTooltipDates.</returns>
        private bool IsInTooltipIndexArrays(int index)
        {
            return this.moldTooltipIndexes.Contains(index) && this.mnewTooltipDates.Contains(index);
        }

        #endregion Implementation
    }
}