#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the Calendar Edit Grid.
    /// </summary>
    public abstract class CalendarEditGrid : FrameworkElement
    {
        private DispatcherTimer timer, mouseHovertimer;   ////    Detect Double Click
        private static int iNTERVAL = 200;  ////    Double Click Detect Interval

        private List<Border> mBorderCellsCollection;
        private Brush mCellForeColor;

        /// <summary>
        /// Collection of cells.
        /// </summary>
        private List<Cell> mCellsCollection;

        /// <summary>
        /// Number of columns.
        /// </summary>
        private int mColumnsCount;

        /// <summary>
        /// Collection of DayNameCell
        /// </summary>
        private List<DayNameCell> mDayNameCellsCollection;

        private Brush mDaysForeColor;

        /// <summary>
        /// Inner grid for cells layout.
        /// </summary>
        private Grid minnerGrid;

        private Storyboard mMonthStoryboard = new Storyboard();
        private Brush mNextMonthDaysForeColor;

        /// <summary>
        /// The parent instance.
        /// </summary>
        private CalendarControl mParentCalendar;
        private Brush mPreviousMonthDaysForeColor;

        /// <summary>
        /// Number of rows.
        /// </summary>
        private int mRowsCount;
        private Brush mSelectedCellForeColor;
        private Brush mSelectedCellBackground;
        private Brush mSelectedCellBorderBrush;
        private Thickness mSelectedCellBorderThickness;
        private CornerRadius mSelectedCellCornerRadius;

        private List<Date> mShiftSelectedDate;
        private bool mShowNextMonthDays;
        private bool mShowPreviousMonthDays;

        private List<Border> mTBorderCellsCollection;
        private List<Cell> mTCellsCollection;

        /// <summary>
        /// List of week number cells.
        /// </summary>
        private List<WeekNumberCell> mweekNumberCells = null;

        /// <summary>
        /// Collection of week numbers.
        /// </summary>
        private List<int> mweekNumbers = null;

        private bool mouseDrag;
        private int numofCharToDisplayMonthName;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEditGrid"/> class.
        /// </summary>
        /// <param name="rowsCount">The rows count.</param>
        /// <param name="coulmnsCount">The coulmns count.</param>
        public CalendarEditGrid(int rowsCount, int coulmnsCount)
        {
            minnerGrid = new Grid();
            mweekNumbers = new List<int>();
            SelectedColumnDates = new List<Date>();
            SelectedMonth = new List<string>();
            ParentCalendar = new CalendarControl();
            mPreviousMonthDaysForeColor = ParentCalendar.PreviousMonthDaysForeground;
            mCellForeColor = ParentCalendar.DateForeground;
            mSelectedCellForeColor = ParentCalendar.SelectedCellForeground;
            mDaysForeColor = ParentCalendar.DayForeground;
            mNextMonthDaysForeColor = ParentCalendar.NextMonthDaysForeground;
            SelectedCellBorderBrush = ParentCalendar.SelectedCellBorderBrush;
            SelectedCellBackground = ParentCalendar.SelectedCellBackground;
            SelectedCellCornerRadius = ParentCalendar.SelectedCellCornerRadius;
            SelectedCellBorderThickness = ParentCalendar.SelectedCellBorderThickness;
            SelectionRangeMode = ParentCalendar.SelectionRangeMode;
            DayNameCellsCollection = new List<DayNameCell>();
            CtrlDateCellsCollection = new List<Date>();
            RowsCount = rowsCount + 1;  ////    This 1 for Displaying MonthName;
            ColumnsCount = coulmnsCount;
            NumCharMonthName = 2;
            MouseHoverCellBorderBrush = ParentCalendar.MouseHoverCellBorderBrush;
            MouseHoverCellCornerRadius = ParentCalendar.MouseHoverCellCornerRadius;
            MouseHoverCellForeground = ParentCalendar.MouseHoverCellForeground;
            MouseHoverCellBorderThickness = ParentCalendar.MouseHoverCellBorderThickness;
            MouseHoverCellBackground = ParentCalendar.MouseHoverCellBackground;
            TotalBorderCellsCollection = new List<Border>();
            TotalCellsCollection = new List<Cell>();
            CellsCollection = new List<Cell>();
            BorderCellsCollection = new List<Border>();
            CultureInfo ci = ParentCalendar.Culture;
            mouseHovertimer = new DispatcherTimer();
            mouseHovertimer.Interval = new TimeSpan(0, 0, 0, 0, 2000);
            mouseHovertimer.Tick += new EventHandler(_mouseHovertimer_Tick);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, iNTERVAL);
            timer.Tick += new EventHandler(_timer_Tick);
            DateTimeFormatInfo format = ci.DateTimeFormat;
            TodayDate = new Date(DateTime.Now.Date, ParentCalendar.Calendar);
            SelectedDate = new Date(ParentCalendar.Date, ParentCalendar.Calendar);
            GenerateGrid();
            FillGrid();
            SetDayNames(format);
            VisibleDate date;
            date.VisibleMonth = ParentCalendar.Calendar.GetMonth(ParentCalendar.Date);
            date.VisibleYear = ParentCalendar.Calendar.GetYear(ParentCalendar.Date);
            SetDayCellDate(date, format, ParentCalendar.Calendar);
            for (int i = 0; i < CellsCollection.Count; i++)
            {
                if (((DayCell)CellsCollection[i]).Date == SelectedDate)
                {
                    m_SelectedDateCell = CellsCollection[i];
                    m_SelectedDateBorderCell = BorderCellsCollection[i];
                    SelectedDate = ((DayCell)CellsCollection[i]).Date;
                    if (!CtrlDateCellsCollection.Contains(((DayCell)CellsCollection[i]).Date))
                    {
                        CtrlDateCellsCollection.Add(((DayCell)CellsCollection[i]).Date);
                    }

                    MouseHoverDate = SelectedDate;
                    m_MouseHoverCell = CellsCollection[i];
                    m_MouseHoveredCell = CellsCollection[i];
                    CellsCollection[i].Focus();
                    break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEditGrid"/> class.
        /// </summary>
        public CalendarEditGrid()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow multiply select].
        /// </summary>
        /// <value><c>true</c> if [allow multiply select]; otherwise, <c>false</c>.</value>
        protected internal bool AllowMultiplySelect
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the border cells collection.
        /// </summary>
        /// <value>The border cells collection.</value>
        protected internal List<Border> BorderCellsCollection
        {
            get
            {
                return mBorderCellsCollection;
            }

            set
            {
                mBorderCellsCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the calender matrix.
        /// </summary>
        /// <value>The calender matrix.</value>
        protected int[,] CalenderMatrix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the cell_ fore.
        /// </summary>
        /// <value>The color of the cell_ fore.</value>
        protected internal Brush Cell_ForeColor
        {
            get
            {
                return mCellForeColor;
            }

            set
            {
                mCellForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the cells collection.
        /// </summary>
        /// <value>The cells collection.</value>
        protected internal List<Cell> CellsCollection
        {
            get
            {
                return mCellsCollection;
            }

            set
            {
                mCellsCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets count of the columns.
        /// </summary>
        /// <value>The columns count.</value>
        protected internal int ColumnsCount
        {
            get
            {
                return mColumnsCount;
            }

            set
            {
                mColumnsCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the CTRL date cells collection.
        /// </summary>
        /// <value>The CTRL date cells collection.</value>
        protected internal List<Date> CtrlDateCellsCollection
        {
            get
            {
                return mShiftSelectedDate;
            }

            set
            {
                mShiftSelectedDate = value;
            }
        }

        /// <summary>
        /// Gets the day grid.
        /// </summary>
        /// <value>The day grid.</value>
        protected Grid DayGrid
        {
            get
            {
                return minnerGrid;
            }
        }

        /// <summary>
        /// Gets or sets the day name cells collection.
        /// </summary>
        /// <value>The day name cells collection.</value>
        protected internal List<DayNameCell> DayNameCellsCollection
        {
            get
            {
                return mDayNameCellsCollection;
            }

            set
            {
                mDayNameCellsCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the days_ fore.
        /// </summary>
        /// <value>The color of the days_ fore.</value>
        protected internal Brush Days_ForeColor
        {
            get
            {
                return mDaysForeColor;
            }

            set
            {
                mDaysForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        protected internal Date EndDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ enter date.
        /// </summary>
        /// <value>The m_ enter date.</value>
        protected internal Date m_EnterDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ mouse hover cell.
        /// </summary>
        /// <value>The m_ mouse hover cell.</value>
        protected internal Cell m_MouseHoverCell
        {
            get;
            set;
        }

        ////with Border and Cell
        /// <summary>
        /// Gets or sets the m_ selected date border cell.
        /// </summary>
        /// <value>The m_ selected date border cell.</value>
        protected internal Border m_SelectedDateBorderCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ selected date cell.
        /// </summary>
        /// <value>The m_ selected date cell.</value>
        protected internal Cell m_SelectedDateCell
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse hover cell background.
        /// </summary>
        /// <value>The mouse hover cell background.</value>
        protected internal Brush MouseHoverCellBackground
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse hover cell border brush.
        /// </summary>
        /// <value>The mouse hover cell border brush.</value>
        protected internal Brush MouseHoverCellBorderBrush
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse hover cell border thickness.
        /// </summary>
        /// <value>The mouse hover cell border thickness.</value>
        protected internal Thickness MouseHoverCellBorderThickness
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse hover cell corner radius.
        /// </summary>
        /// <value>The mouse hover cell corner radius.</value>
        protected internal CornerRadius MouseHoverCellCornerRadius
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse hover cell foreground.
        /// </summary>
        /// <value>The mouse hover cell foreground.</value>
        protected internal Brush MouseHoverCellForeground
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mouse hover date.
        /// </summary>
        /// <value>The mouse hover date.</value>
        protected internal Date MouseHoverDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the color of the next month days_ fore.
        /// </summary>
        /// <value>The color of the next month days_ fore.</value>
        protected internal Brush NextMonthDays_ForeColor
        {
            get
            {
                return mNextMonthDaysForeColor;
            }

            set
            {
                mNextMonthDaysForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the num char month.
        /// </summary>
        /// <value>The name of the num char month.</value>
        protected internal int NumCharMonthName
        {
            get
            {
                return numofCharToDisplayMonthName;
            }

            set
            {
                if (value >= 1)
                {
                    numofCharToDisplayMonthName = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// Type: <see cref="CalendarControl"/>
        /// </value>
        /// <seealso cref="CalendarControl"/>
        protected internal CalendarControl ParentCalendar
        {
            get
            {
                return mParentCalendar;
            }

            set
            {
                mParentCalendar = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the previous month days_ fore.
        /// </summary>
        /// <value>The color of the previous month days_ fore.</value>
        protected internal Brush PreviousMonthDays_ForeColor
        {
            get
            {
                return mPreviousMonthDaysForeColor;
            }

            set
            {
                mPreviousMonthDaysForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets count of the rows.
        /// </summary>
        protected internal int RowsCount
        {
            get
            {
                return mRowsCount;
            }

            set
            {
                mRowsCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the color of the selected cell_ fore.
        /// </summary>
        /// <value>The color of the selected cell_ fore.</value>
        protected internal Brush SelectedCell_ForeColor
        {
            get
            {
                return mSelectedCellForeColor;
            }

            set
            {
                mSelectedCellForeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected cell background.
        /// </summary>
        /// <value>The selected cell background.</value>
        protected internal Brush SelectedCellBackground
        {
            get
            {
                return mSelectedCellBackground;
            }

            set
            {
                mSelectedCellBackground = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected cell border brush.
        /// </summary>
        /// <value>The selected cell border brush.</value>
        protected internal Brush SelectedCellBorderBrush
        {
            get
            {
                return mSelectedCellBorderBrush;
            }

            set
            {
                mSelectedCellBorderBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected cell border thickness.
        /// </summary>
        /// <value>The selected cell border thickness.</value>
        protected internal Thickness SelectedCellBorderThickness
        {
            get
            {
                return mSelectedCellBorderThickness;
            }

            set
            {
                mSelectedCellBorderThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected cell corner radius.
        /// </summary>
        /// <value>The selected cell corner radius.</value>
        protected internal CornerRadius SelectedCellCornerRadius
        {
            get
            {
                return mSelectedCellCornerRadius;
            }

            set
            {
                mSelectedCellCornerRadius = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected column dates.
        /// </summary>
        /// <value>The selected column dates.</value>
        protected internal List<Date> SelectedColumnDates
        {
            get;
            private set;
        }

        Date temp = new Date();
        /// <summary>
        /// Gets or sets the selected date.
        /// </summary>
        /// <value>The selected date.</value>
        protected internal Date SelectedDate
        {
            get
            {
                return temp;
            }
            set
            {
                temp = (Date)value;
            }
        }

        /// <summary>
        /// Gets or sets the selected month.
        /// </summary>
        /// <value>The selected month.</value>
        protected internal List<string> SelectedMonth
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selection range mode.
        /// </summary>
        /// <value>The selection range mode.</value>
        protected internal SelectionRangeMode SelectionRangeMode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show black border for key event].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show black border for key event]; otherwise, <c>false</c>.
        /// </value>
        protected internal bool ShowBlackBorderForKeyEvent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the m_ mouse hovered cell.
        /// </summary>
        /// <value>The m_ mouse hovered cell.</value>
		protected internal Cell m_MouseHoveredCell
        {
            ////this is used for Getting Mousehovercell for keydown handling only
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show next month days property].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show next month days property]; otherwise, <c>false</c>.
        /// </value>
        protected internal bool ShowNextMonthDaysProperty
        {
            get
            {
                return mShowNextMonthDays;
            }

            set
            {
                mShowNextMonthDays = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show previous month days property].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [show previous month days property]; otherwise, <c>false</c>.
        /// </value>
        protected internal bool ShowPreviousMonthDaysProperty
        {
            get
            {
                return mShowPreviousMonthDays;
            }

            set
            {
                mShowPreviousMonthDays = value;
            }
        }

        /// <summary>
        /// Gets or sets the today date.
        /// </summary>
        /// <value>The today date.</value>
        protected internal Date TodayDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total border cells collection.
        /// </summary>
        /// <value>The total border cells collection.</value>
        protected internal List<Border> TotalBorderCellsCollection
        {
            get
            {
                return mTBorderCellsCollection;
            }

            set
            {
                mTBorderCellsCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the total cells collection.
        /// </summary>
        /// <value>The total cells collection.</value>
        protected internal List<Cell> TotalCellsCollection
        {
            get
            {
                return mTCellsCollection;
            }

            set
            {
                mTCellsCollection = value;
            }
        }

        /// <summary>
        /// Gets or sets the visible data.
        /// </summary>
        /// <value>The visible data.</value>
        protected internal VisibleDate VisibleData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets week number cells list.
        /// </summary>
        /// <value>The week numbers.</value>
        internal List<WeekNumberCell> WeekNumbers
        {
            get
            {
                return mweekNumberCells;
            }
        }

        /// <summary>
        /// Handles the Tick event of the _mouseHovertimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _mouseHovertimer_Tick(object sender, EventArgs e)
        {
            mouseHovertimer.Stop();
        }

        /// <summary>
        /// Handles the Tick event of the _timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void _timer_Tick(object sender, EventArgs e)
        {
            timer.Stop();
        }

        /// <summary>
        /// Adds the month.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public static int AddMonth(int month, int param)
        {
            int result = 0;
            int tempMonth = month + param;
            if (tempMonth <= 0)
            {
                result = month;
            }

            if (tempMonth > 12)
            {
                result = tempMonth % 12;
            }

            if (tempMonth < 1)
            {
                result = 12 + tempMonth;
            }

            if (tempMonth >= 1 && tempMonth <= 12)
            {
                result = tempMonth;
            }

            return result;
        }

        /// <summary>
        /// Adds element to the innerGrid children collection.
        /// </summary>
        /// <param name="element">Element to be added.</param>
        protected void AddToInnerGrid(UIElement element)
        {
            minnerGrid.Children.Add(element);
        }

        /// <summary>
        /// Applies the border.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        private Border ApplyBorder(int row, int column)
        {
            Border b = new Border();
            ////    b.Name = "ApplyBorder" + row.ToString() + column.ToString();
            b.CornerRadius = new CornerRadius(5);
            b.BorderBrush = new SolidColorBrush(Colors.Black);
            b.BorderThickness = new Thickness(15);
            return b;
        }

        /// <summary>
        /// Applies the border cell.
        /// </summary>
        /// <returns></returns>
        private Border ApplyBorderCell()
        {
            Border b = new Border();
            b.CornerRadius = new CornerRadius(5);
            b.BorderBrush = new SolidColorBrush(Colors.Black);
            b.BorderThickness = new Thickness(15);
            b.Background = new SolidColorBrush(Colors.Gray);
            return b;
        }

        /// <summary>
        /// Applies the border for cell.
        /// </summary>
        /// <param name="b">The b.</param>
        private void ApplyBorderForCell(Border b)
        {
            b.BorderThickness = SelectedCellBorderThickness;
            b.CornerRadius = SelectedCellCornerRadius;
            b.BorderBrush = SelectedCellBorderBrush;
            b.Background = SelectedCellBackground;
        }

        /// <summary>
        /// Applies the null border.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        private Border ApplyNullBorder(int row, int column)
        {
            Border b = new Border();
            ////    b.Name = "ApplyBorder" + row.ToString() + column.ToString();
            return b;
        }

        /// <summary>
        /// Applies the null border value.
        /// </summary>
        /// <param name="b">The b.</param>
        private void ApplyNullBorderValue(Border b)
        {
            b.Background = new SolidColorBrush();
            b.CornerRadius = new CornerRadius(0);
            b.BorderThickness = new Thickness(0);
            b.BorderBrush = new SolidColorBrush();
        }

        /// <summary>
        /// Applies the story board.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="move">The move.</param>
        protected internal void ApplyStoryBoard(FrameworkElement element, string move)
        {
            if (move == string.Empty)
            {
                move = "2000";
            }
            else if (move == "header")
            {
                move = "600";
            }

            DoubleAnimation opacityAnimation = new DoubleAnimation();
            Duration duration = new Duration(TimeSpan.FromSeconds(Convert.ToInt32(move)));
            mMonthStoryboard = new Storyboard();
            TransformGroup tg = new TransformGroup();
            TranslateTransform tt = new TranslateTransform();
            mMonthStoryboard.Duration = duration;
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(Convert.ToInt32(move)));
            tg.Children.Add(tt);
            element.RenderTransform = tg;
            opacityAnimation.To = 1;
            opacityAnimation.From = 0.0001;
            element.Opacity = 1;
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(FrameworkElement.Opacity)"));
            Storyboard.SetTarget(opacityAnimation, element);
            mMonthStoryboard.Children.Add(opacityAnimation);
            if (!minnerGrid.Resources.Contains("samplee"))
            {
                minnerGrid.Resources.Add("samplee", mMonthStoryboard);
            }

            mMonthStoryboard.Begin();
        }

        /// <summary>
        /// Applies the story board animation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        protected internal void ApplyStoryBoardAnimation(object sender)
        {
            int iRow = Grid.GetRow((FrameworkElement)(DayNameCell)sender);
            int iColumn = Grid.GetColumn((FrameworkElement)(DayNameCell)sender);
            for (int i = 0; i < CellsCollection.Count; i++)
            {
                int cRow = Grid.GetRow((FrameworkElement)(Border)BorderCellsCollection[i]);
                int cColumn = Grid.GetColumn((FrameworkElement)(Border)BorderCellsCollection[i]);
                if (((Border)BorderCellsCollection[i]).Visibility == Visibility.Visible)
                {
                    if (iColumn == cColumn && ((Cell)CellsCollection[i]).IsSelected == false)
                    {
                        if (SelectionRangeMode.WholeColumn == SelectionRangeMode)
                        {
                            ApplyBorderForCell((Border)BorderCellsCollection[i]);
                            ((Cell)CellsCollection[i]).Foreground = Cell_ForeColor;
                            ((Border)BorderCellsCollection[i]).Background = new SolidColorBrush();
                            ApplyStoryBoard((Border)BorderCellsCollection[i], string.Empty);
                        }
                        else if (SelectionRangeMode.CurrentMonth == SelectionRangeMode)
                        {
                            if (((DayCell)CellsCollection[i]).Date.Month == VisibleData.VisibleMonth)
                            {
                                ApplyBorderForCell((Border)BorderCellsCollection[i]);
                                ////((Cell)CellsCollection[i]).Foreground = SelectedCell_ForeColor;
                                ((Border)BorderCellsCollection[i]).Background = new SolidColorBrush();
                                ApplyStoryBoard((Border)BorderCellsCollection[i], string.Empty);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void br_MouseLeave(object sender, MouseEventArgs e)
        {
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);
            m_MouseHoveredCell = null;
            Cell cell = (Cell)b.Child;  //// m_innerGrid.FindName("Cell" + iRow.ToString() + iColumn.ToString());
            if (!cell.IsMouseHover)
            {
                b.CornerRadius = new CornerRadius(0);
                b.BorderBrush = new SolidColorBrush();
                b.Background = new SolidColorBrush();
                b.BorderThickness = new Thickness(0);
                Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                if (((DayCell)cell).Date > curr && ((DayCell)cell).Date.Month != VisibleData.VisibleMonth)
                {
                    cell.Foreground = NextMonthDays_ForeColor;
                }
                else if (((DayCell)cell).Date < curr && ((DayCell)cell).Date.Month != VisibleData.VisibleMonth)
                {
                    cell.Foreground = PreviousMonthDays_ForeColor;
                }
                else
                {
                    if (!cell.IsSelected)
                    {
                        cell.Foreground = Cell_ForeColor;
                    }
                }

                b.UpdateLayout();
            }

            if (!ParentCalendar.IsAppliedBorderTodayDate)
            {
                ParentCalendar.ApplyBorderTodayDate(TodayDate);
            }

            if (TodayDate == ((DayCell)cell).Date)
            {
                ParentCalendar.IsAppliedBorderTodayDate = false;
                ParentCalendar.ApplyBorderTodayDate(TodayDate);
            }
        }

        /// <summary>
        /// Res the apply cell fore ground.
        /// </summary>
        /// <param name="cell">The cell.</param>
        protected internal void ReApplyCellForeGround(Cell cell)
        {
            Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
            if (((DayCell)cell).Date > curr && ((DayCell)cell).Date.Month != VisibleData.VisibleMonth)
            {
                cell.Foreground = NextMonthDays_ForeColor;
            }
            else if (((DayCell)cell).Date < curr && ((DayCell)cell).Date.Month != VisibleData.VisibleMonth)
            {
                cell.Foreground = PreviousMonthDays_ForeColor;
            }
            else
            {
                cell.Foreground = Cell_ForeColor;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void br_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AllowMultiplySelect)
            {
                mouseDrag = true;
            }
            else
            {
                mouseDrag = false;
            }

            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);
            Cell cell = (Cell)b.Child;  ////    m_innerGrid.FindName("Cell" + iRow.ToString() + iColumn.ToString());
            if (AllowMultiplySelect && cell.IsSelected && Keyboard.Modifiers != ModifierKeys.Control)
            {
                int numberOfOccur = NumOfSelectedDate();
                if (numberOfOccur == 2)
                {
                    for (int i = 0; i < BorderCellsCollection.Count; i++)
                    {
                        if (((DayCell)CellsCollection[i]).IsSelected && ((DayCell)CellsCollection[i]).Date != ((DayCell)cell).Date)
                        {
                            if (Keyboard.Modifiers == ModifierKeys.Shift)
                            {
                                if (SelectedDate < ((DayCell)cell).Date)
                                {
                                    if (((DayCell)cell).Date <= EndDate && ((DayCell)CellsCollection[i]).Date > ((DayCell)cell).Date)
                                    {
                                        ApplyNullBorderValue(BorderCellsCollection[i]);
                                        ReApplyCellForeGround(CellsCollection[i]);
                                    }
                                }
                                else
                                {
                                    if (((DayCell)cell).Date >= EndDate && ((DayCell)CellsCollection[i]).Date < ((DayCell)cell).Date)
                                    {
                                        ApplyNullBorderValue(BorderCellsCollection[i]);
                                        ReApplyCellForeGround(CellsCollection[i]);
                                    }
                                }
                            }
                            else
                            {
                                ApplyNullBorderValue(BorderCellsCollection[i]);
                                ReApplyCellForeGround(CellsCollection[i]);
                            }
                        }

                        CellsCollection[i].IsSelected = false;
                        CellsCollection[i].IsMouseHover = false;
                    }
                }
            }

            m_MouseHoverCell = cell;
            MouseHoverDate = ((DayCell)cell).Date;
            cell.Focus();
            if (!cell.IsSelected)
            {
                if (AllowMultiplySelect && Keyboard.Modifiers != ModifierKeys.None)
                {
                    if (SelectedDate.Day == 0)
                    {
                        SelectedDate = ((DayCell)cell).Date;
                    }
                    
                    MultiplySelect(((DayCell)cell).Date, Keyboard.Modifiers);
                    cell.Foreground = SelectedCell_ForeColor;
                    cell.IsSelected = true;
                    cell.IsMouseHover = true;
                }
                else
                {
                    CtrlDateCellsCollection.Clear();
                    SelectedColumnDates.Clear();
                    SelectedMonth.Clear();
                    EndDate = new Date();
                    for (int i = 0; i < BorderCellsCollection.Count; i++)
                    {
                        if (((DayCell)CellsCollection[i]).IsSelected)
                        {
                            ApplyNullBorderValue(BorderCellsCollection[i]);
                            Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                            if (((DayCell)CellsCollection[i]).Date > curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                            {
                                CellsCollection[i].Foreground = NextMonthDays_ForeColor;
                            }
                            else if (((DayCell)CellsCollection[i]).Date < curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                            {
                                CellsCollection[i].Foreground = PreviousMonthDays_ForeColor;
                            }
                            else
                            {
                                CellsCollection[i].Foreground = Cell_ForeColor;
                            }

                            CellsCollection[i].IsSelected = false;
                            CellsCollection[i].IsMouseHover = false;
                        }
                    }

                    if (m_SelectedDateBorderCell != null && m_SelectedDateCell != null && m_SelectedDateBorderCell != ((Border)sender))
                    {
                        ApplyNullBorderValue(m_SelectedDateBorderCell);
                        Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                        if (((DayCell)m_SelectedDateCell).Date > curr && ((DayCell)m_SelectedDateCell).Date.Month != VisibleData.VisibleMonth)
                        {
                            m_SelectedDateCell.Foreground = NextMonthDays_ForeColor;
                        }
                        else if (((DayCell)m_SelectedDateCell).Date < curr && ((DayCell)m_SelectedDateCell).Date.Month != VisibleData.VisibleMonth)
                        {
                            m_SelectedDateCell.Foreground = PreviousMonthDays_ForeColor;
                        }
                        else
                        {
                            m_SelectedDateCell.Foreground = Cell_ForeColor;
                        }

                        ////    m_SelectedDateCell.Foreground = Cell_ForeColor;
                        m_SelectedDateCell.IsSelected = false;
                        m_SelectedDateCell.IsMouseHover = false;
                    }

                    m_SelectedDateCell = cell;
                    m_SelectedDateBorderCell = b;
                    SelectedDate = ((DayCell)cell).Date;
                    if (!CtrlDateCellsCollection.Contains(((DayCell)cell).Date))
                    {
                        CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                    }

                    cell.IsSelected = true;
                    cell.IsMouseHover = true;
                    cell.Foreground = SelectedCell_ForeColor;
                    b.CornerRadius = SelectedCellCornerRadius;
                    b.BorderBrush = SelectedCellBorderBrush;
                    b.BorderThickness = SelectedCellBorderThickness;
                    b.Background = SelectedCellBackground;
                    b.UpdateLayout();
                }
            }
            else if (m_SelectedDateCell == null)
            {
                m_SelectedDateCell = cell;
                m_SelectedDateBorderCell = b;
                SelectedDate = ((DayCell)cell).Date;
                if (!CtrlDateCellsCollection.Contains(((DayCell)cell).Date))
                {
                    CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                }
            }
            else if (AllowMultiplySelect && Keyboard.Modifiers != ModifierKeys.None && Keyboard.Modifiers == ModifierKeys.Control)
            {
                ApplyNullBorderValue(b);
                Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                if (((DayCell)cell).Date > curr && ((DayCell)cell).Date.Month != VisibleData.VisibleMonth)
                {
                    cell.Foreground = NextMonthDays_ForeColor;
                }
                else if (((DayCell)cell).Date < curr && ((DayCell)cell).Date.Month != VisibleData.VisibleMonth)
                {
                    cell.Foreground = PreviousMonthDays_ForeColor;
                }
                else
                {
                    cell.Foreground = Cell_ForeColor;
                }

                e.Handled = true;
                cell.IsSelected = false;
                cell.IsMouseHover = false;
                CtrlDateCellsCollection.Remove(((DayCell)cell).Date);
                ParentCalendar.SelectedDates.Remove(((DayCell)cell).Date.ToDateTime(ParentCalendar.Calendar));
                SelectedColumnDates.Remove(((DayCell)cell).Date);
            }

            cell.Focus();
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void br_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mouseDrag = false;
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);

            Cell cell = (Cell)b.Child;
            
            if (!cell.IsSelected && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (AllowMultiplySelect)
                {
                    if (SelectedDate.Day == 0)
                    {
                        SelectedDate = ((DayCell)cell).Date;
                    }

                    CtrlDateCellsCollection.Clear();
                    EndDate = ((DayCell)cell).Date;
                    cell.IsSelected = true;
                    cell.IsMouseHover = true;
                }
            }

            cell.Focus();
        }

        /// <summary>
        /// Handles the MouseMove event of the br control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void br_MouseMove(object sender, MouseEventArgs e)
        {
            Border b = (Border)sender;
            int iRow = Grid.GetRow((FrameworkElement)b);
            int iColumn = Grid.GetColumn((FrameworkElement)b);
            Cell cell = (Cell)b.Child;  ////    m_innerGrid.FindName("Cell" + iRow.ToString() + iColumn.ToString());
            m_MouseHoveredCell = cell;  
            if (mouseDrag)
            {
                EndDate = ((DayCell)cell).Date;
                ParentCalendar.SelectedDates.Clear();
                ParentCalendar.ApplyBorderForShiftedDate();
                m_MouseHoveredCell = cell;   
            }

            if (!cell.IsSelected)
            {
                ////m_MouseHoverCell = cell;  
				m_MouseHoveredCell = cell;    				
                b.CornerRadius = MouseHoverCellCornerRadius;
                b.BorderBrush = MouseHoverCellBorderBrush;
                b.BorderThickness = MouseHoverCellBorderThickness;
                b.Background = MouseHoverCellBackground;
                cell.Foreground = MouseHoverCellForeground;
                b.UpdateLayout();
            }
        }

        /// <summary>
        /// Checks the current date.
        /// </summary>
        /// <param name="dc">The dc.</param>
        private void CheckCurrentDate(DayCell dc)
        {
            Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
            if (dc.Date > curr && dc.Date.Month != VisibleData.VisibleMonth)
            {
                dc.Foreground = NextMonthDays_ForeColor;
            }
            else if (dc.Date < curr && dc.Date.Month != VisibleData.VisibleMonth)
            {
                dc.Foreground = PreviousMonthDays_ForeColor;
            }
            else
            {
                dc.Foreground = Cell_ForeColor;
            }
        }

        /// <summary>
        /// Creates instance of the single cell.
        /// </summary>
        /// <returns>
        /// New instance of the cell.
        /// </returns>
        protected abstract Cell CreateCell();

        /// <summary>
        /// Handles the KeyDown event of the ct control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void ct_KeyDown(object sender, KeyEventArgs e)
        {
            System.Globalization.Calendar parentCal = null;
            bool prevoiusMonthDisplay = false;
            bool nextMonthDisplay = false;
            if (e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Up)
            {
                ////Keyboard.Modifiers != ModifierKeys.Alt && Keyboard.Modifiers != ModifierKeys.Control && Keyboard.Modifiers != ModifierKeys.Shift && Keyboard.Modifiers != ModifierKeys.Windows)
                bool hasDate = ParentCalendar.CheckCurrentMonth(MouseHoverDate, ref parentCal, ref prevoiusMonthDisplay, ref nextMonthDisplay);
                if (((DayCell)m_SelectedDateCell).Date == SelectedDate)
                {
                    sender = m_SelectedDateCell;
                }

                if (!e.Handled)
                {
                    if (m_SelectedDateCell != null && Keyboard.Modifiers == ModifierKeys.None)
                    {
                        bool changingEnabled = false;
                        switch (e.Key)
                        {
                            case Key.Down:
                                e.Handled = true;
                                break;
                            case Key.Left:
                                e.Handled = true;
                                break;
                            case Key.Right:
                                e.Handled = true;
                                break;
                            case Key.Up:
                                e.Handled = true;
                                break;
                            case Key.Enter:
                                changingEnabled = true;
                                e.Handled = true;
                                break;
                            default:
                                e.Handled = true;
                                break;
                        }

                        Cell dc = (Cell)sender;
                        ////    string[] split = dc.Name.ToString().Split(new string[1] { "Cell" }, StringSplitOptions.None);
                        Border b = (Border)dc.Parent;   ////m_innerGrid.FindName("ApplyBorder" + split[1].ToString());
                        int iRow = Grid.GetRow((FrameworkElement)b);
                        int iColumn = Grid.GetColumn((FrameworkElement)b);
                        if (dc.IsSelected)
                        {
                            if (((DayCell)dc).Date == SelectedDate)
                            {
                                if (ShowBlackBorderForKeyEvent)
                                {
                                    b.BorderBrush = new SolidColorBrush(Colors.Black);
                                    dc.Foreground = Cell_ForeColor;
                                    b.CornerRadius = new CornerRadius(2);
                                    b.BorderThickness = new Thickness(0.02);
                                    b.Background = new SolidColorBrush();
                                }
                                else
                                {
                                    ////    dc.Foreground = Cell_ForeColor;
                                    CheckCurrentDate(((DayCell)dc));
                                    ApplyNullBorderValue(b);
                                }
                            }
                        }
                        else
                        {
                            if (m_MouseHoverCell.IsSelected)
                            {
                                if (((DayCell)m_MouseHoverCell).Date == SelectedDate)
                                {
                                    int loc = BorderCellsCollection.IndexOf(m_SelectedDateBorderCell);
                                    Border b_new = (Border)BorderCellsCollection[loc];
                                    CheckCurrentDate(((DayCell)m_MouseHoverCell));
                                    ////    ((Cell)m_MouseHoverCell).Foreground = Cell_ForeColor;
                                    if (ShowBlackBorderForKeyEvent)
                                    {
                                        b_new.BorderBrush = new SolidColorBrush(Colors.Black);
                                        b.CornerRadius = new CornerRadius(2);
                                        b.BorderThickness = new Thickness(0.02);
                                        b.Background = new SolidColorBrush();
                                    }
                                    else
                                    {
                                        ApplyNullBorderValue(b_new);
                                    }
                                }
                            }
                        }

                        if (dc.Visibility == Visibility.Visible)
                        {
                            if (changingEnabled)
                            {
                                ApplyNullBorderValue(b);
                            }

                            OnDayCellClick(dc, changingEnabled, e, hasDate, parentCal, prevoiusMonthDisplay, nextMonthDisplay);
                        }

                        ApplyBorderMouseHoveredCell();
                    }
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the dncCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void dncCell_MouseEnter(object sender, MouseEventArgs e)
        {
            int column = Grid.GetColumn((FrameworkElement)(DayNameCell)sender);
            if (!hasData(column.ToString() + VisibleData.VisibleMonth.ToString() + VisibleData.VisibleYear.ToString()))
            {
                if (AllowMultiplySelect)
                {
                    ApplyStoryBoardAnimation(sender);
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the dncCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private void dncCell_MouseLeave(object sender, MouseEventArgs e)
        {
            int column = Grid.GetColumn((FrameworkElement)(DayNameCell)sender);
            if (!hasData(column.ToString() + VisibleData.VisibleMonth.ToString() + VisibleData.VisibleYear.ToString()))
            {
                if (AllowMultiplySelect)
                {
                    int iRow = Grid.GetRow((FrameworkElement)(DayNameCell)sender);
                    int iColumn = Grid.GetColumn((FrameworkElement)(DayNameCell)sender);
                    for (int i = 0; i < CellsCollection.Count; i++)
                    {
                        if (((Border)BorderCellsCollection[i]).Visibility == Visibility.Visible)
                        {
                            int cRow = Grid.GetRow((FrameworkElement)(Border)BorderCellsCollection[i]);
                            int cColumn = Grid.GetColumn((FrameworkElement)(Border)BorderCellsCollection[i]);
                            if (iColumn == cColumn && ((Cell)CellsCollection[i]).IsSelected == false)
                            {
                                if (((DayCell)CellsCollection[i]).Date != TodayDate)
                                {
                                    ApplyNullBorderValue((Border)BorderCellsCollection[i]);
                                }
                                else
                                {
                                    ParentCalendar.ApplyBorderTodayDate(TodayDate);
                                }

                                Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                                if (((DayCell)CellsCollection[i]).Date > curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                {
                                    ((DayCell)CellsCollection[i]).Foreground = NextMonthDays_ForeColor;
                                }
                                else if (((DayCell)CellsCollection[i]).Date < curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                {
                                    ((DayCell)CellsCollection[i]).Foreground = PreviousMonthDays_ForeColor;
                                }
                                else
                                {
                                    ((DayCell)CellsCollection[i]).Foreground = Cell_ForeColor;
                                }
                            }
                            else if (iColumn == cColumn && ((Cell)CellsCollection[i]).IsSelected == true)
                            {
                                ////    ApplyBorderForCell((Border)BorderCellsCollection[i]);
                                ((DayCell)CellsCollection[i]).Foreground = SelectedCell_ForeColor;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the dncCell control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void dncCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int column = Grid.GetColumn((FrameworkElement)(DayNameCell)sender);
            if (!hasData(column.ToString() + VisibleData.VisibleMonth.ToString() + VisibleData.VisibleYear.ToString()))
            {
                if (AllowMultiplySelect)
                {
                    if (timer.IsEnabled)
                    {
                        ////    stop the timer
                        this.timer.Stop();
                        if (!(Keyboard.Modifiers == ModifierKeys.Control))
                        {
                            SelectedColumnDates.Clear();
                            CtrlDateCellsCollection.Clear();
                            SelectedMonth.Clear();
                        }

                        int dayRow = 1;
                        int iRow = Grid.GetRow((FrameworkElement)(DayNameCell)sender);
                        int iColumn = Grid.GetColumn((FrameworkElement)(DayNameCell)sender);
                        for (int i = 0; i < CellsCollection.Count; i++)
                        {
                            int cRow = Grid.GetRow((FrameworkElement)(Border)BorderCellsCollection[i]);
                            int cColumn = Grid.GetColumn((FrameworkElement)(Border)BorderCellsCollection[i]);

                            if (((Border)BorderCellsCollection[i]).Visibility == Visibility.Visible)
                            {
                                if (iColumn == cColumn)
                                {
                                    ////ApplyBorderForCell((Border)BorderCellsCollection[i]);
                                    if (AllowMultiplySelect)
                                    {
                                        if (!SelectedColumnDates.Contains(((DayCell)CellsCollection[i]).Date))
                                        {
                                            Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                                            if (((DayCell)CellsCollection[i]).Date > curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                            {
                                                if (ParentCalendar.SelectionRangeMode == SelectionRangeMode.WholeColumn)
                                                {
                                                    SelectedColumnDates.Add(((DayCell)CellsCollection[i]).Date);
                                                    ((Cell)CellsCollection[i]).Foreground = SelectedCell_ForeColor;
                                                    ((Cell)CellsCollection[i]).IsSelected = true;
                                                    ((Cell)CellsCollection[i]).IsMouseHover = true;
                                                }
                                            }
                                            else if (((DayCell)CellsCollection[i]).Date < curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                            {
                                                if (ParentCalendar.SelectionRangeMode == SelectionRangeMode.WholeColumn)
                                                {
                                                    SelectedColumnDates.Add(((DayCell)CellsCollection[i]).Date);
                                                    ((Cell)CellsCollection[i]).Foreground = SelectedCell_ForeColor;
                                                    ((Cell)CellsCollection[i]).IsSelected = true;
                                                    ((Cell)CellsCollection[i]).IsMouseHover = true;
                                                }
                                            }
                                            else
                                            {
                                                SelectedColumnDates.Add(((DayCell)CellsCollection[i]).Date);
                                                ((Cell)CellsCollection[i]).Foreground = SelectedCell_ForeColor;
                                                ((Cell)CellsCollection[i]).IsSelected = true;
                                                ((Cell)CellsCollection[i]).IsMouseHover = true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!(Keyboard.Modifiers == ModifierKeys.Control))
                                    {
                                        ApplyNullBorderValue(((Border)BorderCellsCollection[i]));
                                        Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                                        if (((DayCell)CellsCollection[i]).Date > curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                        {
                                            ((DayCell)CellsCollection[i]).Foreground = NextMonthDays_ForeColor;
                                        }
                                        else if (((DayCell)CellsCollection[i]).Date < curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                        {
                                            ((DayCell)CellsCollection[i]).Foreground = PreviousMonthDays_ForeColor;
                                        }
                                        else
                                        {
                                            ((DayCell)CellsCollection[i]).Foreground = Cell_ForeColor;
                                        }
                                        ////((Cell)CellsCollection[i]).Foreground = Cell_ForeColor;
                                        ((Cell)CellsCollection[i]).IsSelected = false;
                                        ((Cell)CellsCollection[i]).IsMouseHover = false;
                                    }
                                }
                            }

                            if (iColumn == cColumn && cRow == dayRow)
                            {
                                if (((Border)BorderCellsCollection[i]).Visibility == Visibility.Visible)
                                {
                                    CtrlDateCellsCollection.Clear();                                   
                                    Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                                    if (((DayCell)CellsCollection[i]).Date > curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                    {
                                        if (ParentCalendar.SelectionRangeMode == SelectionRangeMode.WholeColumn)
                                        {
                                            CtrlDateCellsCollection.Add(((DayCell)((Cell)CellsCollection[i])).Date);
                                            SelectedColumnDates.Add(((DayCell)CellsCollection[i]).Date);
                                            ((Cell)CellsCollection[i]).IsSelected = true;
                                            ((Cell)CellsCollection[i]).IsMouseHover = true;
                                        }
                                    }
                                    else if (((DayCell)CellsCollection[i]).Date < curr && ((DayCell)CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                    {
                                        if (ParentCalendar.SelectionRangeMode == SelectionRangeMode.WholeColumn)
                                        {
                                            CtrlDateCellsCollection.Add(((DayCell)((Cell)CellsCollection[i])).Date);
                                            SelectedColumnDates.Add(((DayCell)CellsCollection[i]).Date);
                                            ((Cell)CellsCollection[i]).IsSelected = true;
                                            ((Cell)CellsCollection[i]).IsMouseHover = true;
                                        }
                                    }
                                    else
                                    {
                                        CtrlDateCellsCollection.Add(((DayCell)((Cell)CellsCollection[i])).Date);
                                        SelectedColumnDates.Add(((DayCell)CellsCollection[i]).Date);
                                        ((Cell)CellsCollection[i]).IsSelected = true;
                                        ((Cell)CellsCollection[i]).IsMouseHover = true;
                                    }
                                }
                                else
                                {
                                    dayRow = 2;
                                }
                            }
                        }

                        if (!SelectedMonth.Contains(column.ToString() + VisibleData.VisibleMonth.ToString() + VisibleData.VisibleYear.ToString()))
                        {
                            SelectedMonth.Add(column.ToString() + VisibleData.VisibleMonth.ToString() + VisibleData.VisibleYear.ToString());
                        }
                    }
                    else
                    {
                        timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// Adds day and dayName cells to the grid.
        /// </summary>
        private void FillGrid()
        {
            ////m_innerGrid.k
            for (int i = 0, k = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    if (i == 0)
                    {
                        ////Adding DayNameCell
                        DayNameCell dncCell = new DayNameCell();
                        dncCell.HorizontalAlignment = HorizontalAlignment.Center;
                        dncCell.VerticalAlignment = VerticalAlignment.Center;
                        dncCell.MouseEnter += new MouseEventHandler(dncCell_MouseEnter);
                        dncCell.MouseLeave += new MouseEventHandler(dncCell_MouseLeave);
                        dncCell.MouseLeftButtonDown += new MouseButtonEventHandler(dncCell_MouseLeftButtonDown);
                        DayNameCellsCollection.Add(dncCell);
                        Grid.SetRow((FrameworkElement)dncCell, 0);
                        Grid.SetColumn((FrameworkElement)dncCell, j);
                        minnerGrid.Children.Add(dncCell);
                    }
                    else
                    {
                        Border br = ApplyNullBorder(i, j);
                        br.MouseMove += new MouseEventHandler(br_MouseMove);
                        br.MouseLeave += new MouseEventHandler(br_MouseLeave);
                        ////    br.MouseLeftButtonUp += new MouseButtonEventHandler(br_MouseLeftButtonUp);
                        br.MouseLeftButtonUp += new MouseButtonEventHandler(br_MouseLeftButtonUp);
                        br.MouseLeftButtonDown += new MouseButtonEventHandler(br_MouseLeftButtonDown);

                        Cell ct = CreateCell();
                        ////    ct.Name = "Cell" + i.ToString() + j.ToString();
                        ct.HorizontalAlignment = HorizontalAlignment.Center;
                        ct.VerticalAlignment = VerticalAlignment.Center;
                        ct.KeyDown += new KeyEventHandler(ct_KeyDown);
                        br.Child = ct;
                        Grid.SetRow((FrameworkElement)br, i);
                        Grid.SetColumn((FrameworkElement)br, j);
                        CellsCollection.Add(ct);
                        BorderCellsCollection.Add(br);
                        TotalBorderCellsCollection.Add(br);
                        TotalCellsCollection.Add(ct);
                        minnerGrid.Children.Add(br);
                        k++;
                    }
                }
            }
        }

        /// <summary>
        /// Fills m_weekNumberCells list.
        /// </summary>
        private void FillWeekNumberCells()
        {
            mweekNumberCells = new List<WeekNumberCell>();
            foreach (int number in mweekNumbers)
            {
                WeekNumberCell wnc = new WeekNumberCell();
                wnc.Content = number;
                mweekNumberCells.Add(wnc);
            }
        }

        /// <summary>
        /// Fills the grid with column definitions and row definitions.
        /// </summary>
        private void GenerateGrid()
        {
            for (int i = 0; i < ColumnsCount; i++)
            {
                ColumnDefinition cdColumn = new ColumnDefinition();
                cdColumn.Width = new GridLength(1, GridUnitType.Star);
                minnerGrid.ColumnDefinitions.Add(cdColumn);
            }

            // Define Rows
            for (int i = 0; i < RowsCount; i++)
            {
                RowDefinition rdRow = new RowDefinition();
                if (i == 0)
                {
                    rdRow.Height = new GridLength(15, GridUnitType.Pixel);
                }
                else
                {
                    rdRow.Height = new GridLength(1, GridUnitType.Star);
                }

                minnerGrid.RowDefinitions.Add(rdRow);
            }
        }

        /// <summary>
        /// Generates the matrix.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <param name="year">The year.</param>
        /// <param name="format">The format.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns></returns>
        public static int[,] GenerateMatrix(int month, int year, DateTimeFormatInfo format, System.Globalization.Calendar calendar)
        {
            DateTime dateMonthStart = GetFirstDayOfMonth(year, month, calendar);
            DayOfWeek dw = calendar.GetDayOfWeek(dateMonthStart);
            int[,] matrix = new int[6, 7];
            int firstDayOfMonth = (int)dw;
            int firstDayOfWeek = (int)format.FirstDayOfWeek;
            int iDaysInMonth = calendar.GetDaysInMonth(year, month);
            int first = ((6 + firstDayOfMonth - firstDayOfWeek) % 7) + 1;
            int start = first;

            for (int i = 0, k = 1; i < 6; i++)
            {
                if (i > 0)
                {
                    start = 0;
                }

                for (int j = start; j < 7; j++, k++)
                {
                    matrix[i, j] = k;
                }
            }

            start = first;
            for (int i = 0, k = 0, c = 1; i < 6; i++)
            {
                if (i > 0)
                {
                    start = 0;
                }

                for (int j = start; j < 7; j++, k++)
                {
                    if (k >= iDaysInMonth)
                    {
                        matrix[i, j] = c;
                        c++;
                    }
                }
            }

            start = first;
            if (month == 1)
            {
                year--;
            }

            if (year != (calendar.MinSupportedDateTime.Year - 1))
            {
                int daysInPrevMonth = 0;
                try
                {
                    daysInPrevMonth = calendar.GetDaysInMonth(year, AddMonth(month, -1));
                }
                catch
                {
                    year = year + 1;
                    month = 2;
                    daysInPrevMonth = calendar.GetDaysInMonth(year, AddMonth(month, -1));
                }

                for (int i = 0, k = daysInPrevMonth - first + 1; i < first; i++, k++)
                {
                    matrix[0, i] = k;
                }
            }

            return matrix;
        }

        /// <summary>
        /// Gets the first day of month.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="month">The month.</param>
        /// <param name="calendar">The calendar.</param>
        /// <returns></returns>
        public static DateTime GetFirstDayOfMonth(int year, int month, System.Globalization.Calendar calendar)
        {
            DateTime dateMonthStart = calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
            return dateMonthStart;
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
        /// Determines whether the specified value has data.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value has data; otherwise, <c>false</c>.
        /// </returns>
        private bool hasData(string value)
        {
            bool has = false;
            foreach (string str in SelectedMonth)
            {
                if (str == value)
                {
                    has = true;
                    break;
                }
            }

            return has;
        }

        /// <summary>
        /// Hides next month days in the visible day grid if ShowNextMonthDays is false.
        /// </summary>
        /// <param name="dayCell">day cell to show</param>
        /// <param name="visibleMonth">current visible month</param>
        public void HideNextMonthDays(DayCell dayCell, int visibleMonth)
        {
            if (!ParentCalendar.ShowNextMonthDates)
            {
                if (dayCell.Date.Month == GetNextMonth(visibleMonth))
                {
                    dayCell.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Hides previous month days in the visible day grid if ShowPreviousMonthDays is false.
        /// </summary>
        /// <param name="dayCell">day cell to show</param>
        /// <param name="visibleMonth">current visible month</param>
        public void HidePrevMonthDays(DayCell dayCell, int visibleMonth)
        {
            if (!ParentCalendar.ShowPreviousMonthDates)
            {
                if (dayCell.Date.Month == GetPrevMonth(visibleMonth))
                {
                    dayCell.Visibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Initializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="culture">The culture.</param>
        /// <param name="calendar">The calendar.</param>
        public void Initialize(VisibleDate data, CultureInfo culture, System.Globalization.Calendar calendar)
        {
            if (ParentCalendar != null)
            {
                DateTimeFormatInfo format = culture.DateTimeFormat;
                VisibleDate visibleData = data;
                SetDayCellDate(data, format, calendar);
                SetDayNames(format);
            }
        }

        /// <summary>
        /// Implements multiply selection logic.
        /// </summary>
        /// <param name="date">The <see cref="T:System.DataTime"/> date
        /// that should be selected.</param>
        /// <param name="modifiers">The <see cref="System.Windows.Input.ModifierKeys"/> object used for 
        /// implemeting the selection logic.</param>
        private void MultiplySelect(Date date, ModifierKeys modifiers)
        {
            if (modifiers == ModifierKeys.Control)
            {
                ////Selection with Control
                EndDate = new Date();
                if (CtrlDateCellsCollection.Contains(date))
                {
                    CtrlDateCellsCollection.Remove(date);
                }
                else
                {
                    CtrlDateCellsCollection.Add(date);
                    SelectedDate = date;
                    MouseHoverDate = SelectedDate;
                }
            }
            else
            {
                if (modifiers == ModifierKeys.Shift)
                {
                    CtrlDateCellsCollection.Clear();
                    EndDate = date;
                   // SelectedDate = EndDate;
                }
            }
        }

        /// <summary>
        /// Nums the of selected date.
        /// </summary>
        /// <returns></returns>
        private int NumOfSelectedDate()
        {
            int k = 0;
            for (int i = 0; i < BorderCellsCollection.Count; i++)
            {
                if (CellsCollection[i].IsSelected)
                {
                    k++;
                }

                if (k == 2)
                {
                    break;
                }
            }

            return k;
        }

        /// <summary>
        /// Called when [day cell click].
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="changingEnabled">if set to <c>true</c> [changing enabled].</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <param name="hasDate">if set to <c>true</c> [has date].</param>
        /// <param name="parentCal">The parent cal.</param>
        /// <param name="allowprevoiusMonthDisplay">if set to <c>true</c> [allowprevoius month display].</param>
        /// <param name="allowNextMonthDisplay">if set to <c>true</c> [allow next month display].</param>
        private void OnDayCellClick(Cell dc, bool changingEnabled, KeyEventArgs e, bool hasDate, System.Globalization.Calendar parentCal, bool allowprevoiusMonthDisplay, bool allowNextMonthDisplay)
        {
            ////    This Method used for  When the  Key is Moved on the Calendar.Selected Date will change. 
            int iNewFocusedCellIndex = (int)ValidateFocusIndex();
            if (!changingEnabled)
            {
                int m = VisibleData.VisibleMonth;
                if (iNewFocusedCellIndex >= 0 && CellsCollection.Count >= iNewFocusedCellIndex + 1)
                {
                    Border b = BorderCellsCollection[iNewFocusedCellIndex];
                    ////    ((Cell)CellsCollection[iNewFocusedCellIndex]).IsMouseHover = false;
                    int iRow = Grid.GetRow((FrameworkElement)BorderCellsCollection[iNewFocusedCellIndex]);
                    int iColumn = Grid.GetColumn((FrameworkElement)BorderCellsCollection[iNewFocusedCellIndex]);
                    if (e.Key == Key.Right)
                    {
                        if (iNewFocusedCellIndex != CellsCollection.Count - 1)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex + 1]).Visibility != Visibility.Collapsed)
                            {
                                ApplyNullBorderValue(b);
                                ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                CellsCollection[iNewFocusedCellIndex].IsSelected = false;

                                b = BorderCellsCollection[iNewFocusedCellIndex + 1];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex + 1];
                                cell.IsMouseHover = true;
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);

                                if (MouseHoverDate.Month != VisibleData.VisibleMonth)
                                {
                                    CtrlDateCellsCollection.Clear();
                                    m_SelectedDateCell = cell;
                                    m_SelectedDateBorderCell = b;
                                    SelectedDate = ((DayCell)cell).Date;
                                    CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                                    cell.IsSelected = true;
                                    cell.IsMouseHover = true;
                                    m_MouseHoverCell = cell;
                                    MouseHoverDate = ((DayCell)cell).Date;
                                    m_EnterDate = ((DayCell)cell).Date;
                                    cell.Foreground = SelectedCell_ForeColor;
                                    ApplyBorderForCell(b);
                                    dc.IsSelected = false;
                                    dc.IsMouseHover = false;
                                    m_MouseHoveredCell = null;
                                    ParentCalendar.ShowDayGridonSelectedCell(MouseHoverDate);
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                    ////    ParentCalendar.NextMonthDisplay();
                                    ////    ParentCalendar.ApplyBorderSelectedDate(MouseHoverDate);
                                }
                                else
                                {
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                }
                            }
                            else
                            {
                                Cell c = m_MouseHoverCell;
                                Border b1 = (Border)c.Parent;
                                c.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                            }
                        }

                        if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                    }

                    if (e.Key == Key.Left)
                    {
                        if (iNewFocusedCellIndex >= 1)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex - 1]).Visibility != Visibility.Collapsed)
                            {
                                ApplyNullBorderValue(b);
                                ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                CellsCollection[iNewFocusedCellIndex].IsSelected = false;

                                b = BorderCellsCollection[iNewFocusedCellIndex - 1];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex - 1];
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.IsMouseHover = true;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                                if (MouseHoverDate.Month != VisibleData.VisibleMonth)
                                {
                                    CtrlDateCellsCollection.Clear();
                                    m_SelectedDateCell = cell;
                                    m_SelectedDateBorderCell = b;
                                    SelectedDate = ((DayCell)cell).Date;
                                    CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                                    cell.IsSelected = true;
                                    cell.IsMouseHover = true;
                                    m_MouseHoverCell = cell;
                                    MouseHoverDate = ((DayCell)cell).Date;
                                    m_EnterDate = ((DayCell)cell).Date;
                                    cell.Foreground = SelectedCell_ForeColor;
                                    ApplyBorderForCell(b);
                                    dc.IsSelected = false;
                                    dc.IsMouseHover = false;
                                    m_MouseHoveredCell = null;
                                    ParentCalendar.ShowDayGridonSelectedCell(MouseHoverDate);
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                    ////ParentCalendar.NextMonthDisplay();
                                    ////ParentCalendar.ApplyBorderSelectedDate(MouseHoverDate);
                                }
                                else
                                {
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                }
                            }
                            else
                            {
                                Cell c = m_MouseHoverCell;
                                Border b1 = (Border)c.Parent;
                                c.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                            }
                        }

                        if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                    }

                    if (e.Key == Key.Up)
                    {
                        if (0 <= iNewFocusedCellIndex - 7)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex - 7]).Visibility != Visibility.Collapsed)
                            {
                                ApplyNullBorderValue(b);
                                ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                CellsCollection[iNewFocusedCellIndex].IsSelected = false;

                                b = BorderCellsCollection[iNewFocusedCellIndex - 7];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex - 7];
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.IsMouseHover = true;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                                if (MouseHoverDate.Month != VisibleData.VisibleMonth)
                                {
                                    CtrlDateCellsCollection.Clear();
                                    m_SelectedDateCell = cell;
                                    m_SelectedDateBorderCell = b;
                                    SelectedDate = ((DayCell)cell).Date;
                                    CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                                    cell.IsSelected = true;
                                    cell.IsMouseHover = true;
                                    m_MouseHoverCell = cell;
                                    MouseHoverDate = ((DayCell)cell).Date;
                                    m_EnterDate = ((DayCell)cell).Date;
                                    cell.Foreground = SelectedCell_ForeColor;
                                    ApplyBorderForCell(b);
                                    dc.IsSelected = false;
                                    dc.IsMouseHover = false;
                                    m_MouseHoveredCell = null;
                                    ParentCalendar.ShowDayGridonSelectedCell(MouseHoverDate);
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                    ////ParentCalendar.NextMonthDisplay();
                                    ////ParentCalendar.ApplyBorderSelectedDate(MouseHoverDate);
                                }
                                else
                                {
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                }
                            }
                            else
                            {
                                Cell c = m_MouseHoverCell;
                                Border b1 = (Border)c.Parent;
                                c.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                                MouseHoverDate = ((DayCell)c).Date;
                                ParentCalendar.ShowDayGridonSelectedCell(MouseHoverDate);
                                ParentCalendar.setCurrentDate(MouseHoverDate);
                            }
                        }
                        else
                        {
                            ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                            Border b1 = (Border)((Cell)m_MouseHoverCell).Parent;
                            ApplyBorderForCell(b1);
                            MouseHoverDate = ((DayCell)((Cell)m_MouseHoverCell)).Date;
                            if (ParentCalendar.ShowPreviousMonthDates)
                            {
                                ParentCalendar.PreviouMonthDisplay();
                                ParentCalendar.setCurrentDate(MouseHoverDate);
                            }
                        }

                        if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                    }

                    if (e.Key == Key.Down)
                    {
                        if (CellsCollection.Count > iNewFocusedCellIndex + 7)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex + 7]).Visibility != Visibility.Collapsed)
                            {
                                ApplyNullBorderValue(b);
                                ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                CellsCollection[iNewFocusedCellIndex].IsSelected = false;

                                b = BorderCellsCollection[iNewFocusedCellIndex + 7];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex + 7];
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.IsMouseHover = true;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                                if (MouseHoverDate.Month != VisibleData.VisibleMonth)
                                {
                                    CtrlDateCellsCollection.Clear();
                                    m_SelectedDateCell = cell;
                                    m_SelectedDateBorderCell = b;
                                    SelectedDate = ((DayCell)cell).Date;
                                    CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                                    cell.IsSelected = true;
                                    cell.IsMouseHover = true;
                                    m_MouseHoverCell = cell;
                                    MouseHoverDate = ((DayCell)cell).Date;
                                    m_EnterDate = ((DayCell)cell).Date;
                                    cell.Foreground = SelectedCell_ForeColor;
                                    ApplyBorderForCell(b);
                                    dc.IsSelected = false;
                                    dc.IsMouseHover = false;
                                    m_MouseHoveredCell = null;
                                    ParentCalendar.ShowDayGridonSelectedCell(MouseHoverDate);
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                    ////ParentCalendar.NextMonthDisplay();
                                    ////ParentCalendar.ApplyBorderSelectedDate(MouseHoverDate);
                                }
                                else
                                {
                                    ParentCalendar.setCurrentDate(MouseHoverDate);
                                }
                            }
                            else
                            {
                                Cell c = m_MouseHoverCell;
                                Border b1 = (Border)c.Parent;
                                c.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                            }
                        }
                        else if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                        else
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                            if (ParentCalendar.ShowNextMonthDates)
                            {
                                ParentCalendar.NextMonthDisplay();
                                ParentCalendar.setCurrentDate(MouseHoverDate);
                            }
                        }
                    }

                    if (!hasDate)
                    {
                        Cell c = m_MouseHoverCell;
                        Border b1 = (Border)c.Parent;
                        c.Foreground = SelectedCell_ForeColor;
                        ApplyBorderForCell(b);
                    }
                }

                m_MouseHoverCell.IsSelected = true;
            }
            else
            {
                for (int i = 0; i < BorderCellsCollection.Count; i++)
                {
                    Border b = (Border)BorderCellsCollection[i];
                    ApplyNullBorderValue(b);
                    ////    ((Cell)CellsCollection[i]).Foreground = Cell_ForeColor;
                    CheckCurrentDate((DayCell)CellsCollection[i]);
                }

                if (((Border)BorderCellsCollection[iNewFocusedCellIndex]).Visibility != Visibility.Collapsed)
                {
                    CtrlDateCellsCollection.Clear();
                    Border b = BorderCellsCollection[iNewFocusedCellIndex];
                    Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex];
                    m_SelectedDateCell = cell;
                    m_SelectedDateBorderCell = b;
                    SelectedDate = ((DayCell)cell).Date;
                    CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                    cell.IsSelected = true;
                    cell.IsMouseHover = true;
                    m_MouseHoverCell = cell;
                    MouseHoverDate = ((DayCell)cell).Date;
                    m_EnterDate = ((DayCell)cell).Date;
                    cell.Foreground = SelectedCell_ForeColor;
                    ApplyBorderForCell(b);
                    dc.IsSelected = false;
                    dc.IsMouseHover = false;
                    m_MouseHoveredCell = null;
                    ParentCalendar.ShowDayGridonSelectedCell(MouseHoverDate);
                    ParentCalendar.setCurrentDate(MouseHoverDate);
                }
            }

            if (MouseHoverDate != TodayDate)
            {
                ParentCalendar.ApplyBorderTodayDate(TodayDate);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Applies the border mouse hovered cell.
        /// </summary>
		void ApplyBorderMouseHoveredCell()
        {
            if (m_MouseHoveredCell != null && MouseHoverDate != null)
            {
                if (MouseHoverDate != ((DayCell)m_MouseHoveredCell).Date)
                {
                    Cell cell = (Cell)m_MouseHoveredCell;
                    Border b = cell.Parent as Border;
                    b.CornerRadius = MouseHoverCellCornerRadius;
                    b.BorderBrush = MouseHoverCellBorderBrush;
                    b.BorderThickness = MouseHoverCellBorderThickness;
                    b.Background = MouseHoverCellBackground;
                    cell.Foreground = MouseHoverCellForeground;
                    b.UpdateLayout();
                }
            }
        }

        /// <summary>
        /// Called when [day cell click_ enter].
        /// </summary>
        /// <param name="dc">The dc.</param>
        /// <param name="changingEnabled">if set to <c>true</c> [changing enabled].</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        /// <param name="hasDate">if set to <c>true</c> [has date].</param>
        /// <param name="parentCal">The parent cal.</param>
        /// <param name="allowprevoiusMonthDisplay">if set to <c>true</c> [allowprevoius month display].</param>
        /// <param name="allowNextMonthDisplay">if set to <c>true</c> [allow next month display].</param>
        private void OnDayCellClick_Enter(Cell dc, bool changingEnabled, KeyEventArgs e, bool hasDate, System.Globalization.Calendar parentCal, bool allowprevoiusMonthDisplay, bool allowNextMonthDisplay)
        {
            ////    This Method used for  When the Enter Key is Pressed on the Calendar.Selected Date will change otherwise it wont.
            for (int temp = 0; temp < CellsCollection.Count; temp++)
            {
                CheckCurrentDate(((DayCell)CellsCollection[temp]));
                if (((DayCell)CellsCollection[temp]).Date == m_EnterDate)
                {
                    CheckCurrentDate(((DayCell)CellsCollection[temp]));
                    ////    ((Cell)CellsCollection[temp]).Foreground = Cell_ForeColor;
                }
            }

            int iNewFocusedCellIndex = (int)ValidateFocusIndex();
            if (!changingEnabled)
            {
                int m = VisibleData.VisibleMonth;
                if (iNewFocusedCellIndex >= 0 && CellsCollection.Count >= iNewFocusedCellIndex + 1)
                {
                    Border b = BorderCellsCollection[iNewFocusedCellIndex];
                    ////    ((Cell)CellsCollection[iNewFocusedCellIndex]).IsMouseHover = false;
                    int iRow = Grid.GetRow((FrameworkElement)BorderCellsCollection[iNewFocusedCellIndex]);
                    int iColumn = Grid.GetColumn((FrameworkElement)BorderCellsCollection[iNewFocusedCellIndex]);
                    if (e.Key == Key.Right)
                    {
                        if (iNewFocusedCellIndex != CellsCollection.Count - 1)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex + 1]).Visibility != Visibility.Collapsed)
                            {
                                if (((DayCell)CellsCollection[iNewFocusedCellIndex]).Date != SelectedDate)
                                {
                                    ApplyNullBorderValue(b);
                                    ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                    CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                    CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                    CellsCollection[iNewFocusedCellIndex].IsSelected = false;
                                }

                                b = BorderCellsCollection[iNewFocusedCellIndex + 1];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex + 1];
                                cell.IsMouseHover = true;
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                            }
                            else
                            {
                                ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                            }
                        }
                        else
                        {
                            DateTime d = ((Date)((DayCell)m_MouseHoverCell).Date).ToDateTime(parentCal);
                            d = d.AddDays(1);
                            Date date = new Date(d, parentCal);
                            ParentCalendar.NextMonthDisplay();
                            for (int ii = 0; ii < BorderCellsCollection.Count; ii++)
                            {
                                DayCell m_dc = (DayCell)CellsCollection[ii];
                                if (new Date((DateTime)ParentCalendar.SelectedDate, parentCal) == ((DayCell)CellsCollection[ii]).Date)
                                {
                                    b = BorderCellsCollection[ii];
                                    Cell cell = (Cell)CellsCollection[ii];
                                    cell.IsSelected = false;
                                    cell.IsMouseHover = false;
                                    CheckCurrentDate((DayCell)CellsCollection[ii]);
                                    ApplyNullBorderValue(b);
                                }

                                if (m_dc.Date == date)
                                {
                                    b = BorderCellsCollection[ii];
                                    Cell cell = (Cell)CellsCollection[ii];
                                    m_MouseHoverCell = cell;
                                    MouseHoverDate = ((DayCell)cell).Date;
                                    cell.IsMouseHover = true;
                                    cell.Foreground = SelectedCell_ForeColor;
                                    ApplyBorderForCell(b);
                                }
                            }

                            ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                        }

                        if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                    }

                    if (e.Key == Key.Left)
                    {
                        if (iNewFocusedCellIndex >= 1)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex - 1]).Visibility != Visibility.Collapsed)
                            {
                                if (((DayCell)CellsCollection[iNewFocusedCellIndex]).Date != SelectedDate)
                                {
                                    ApplyNullBorderValue(b);
                                    ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                    CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                    CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                    CellsCollection[iNewFocusedCellIndex].IsSelected = false;
                                }

                                b = BorderCellsCollection[iNewFocusedCellIndex - 1];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex - 1];
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.IsMouseHover = true;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                            }
                            else
                            {
                                ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                            }
                        }
                        else
                        {
                            DateTime d = ((Date)((DayCell)m_MouseHoverCell).Date).ToDateTime(parentCal);
                            d = d.AddDays(-1);
                            Date date = new Date(d, parentCal);
                            ParentCalendar.PreviouMonthDisplay();
                            for (int ii = 0; ii < BorderCellsCollection.Count; ii++)
                            {
                                DayCell m_dc = (DayCell)CellsCollection[ii];
                                if (new Date((DateTime)ParentCalendar.SelectedDate, parentCal) == ((DayCell)CellsCollection[ii]).Date)
                                {
                                    b = BorderCellsCollection[ii];
                                    Cell cell = (Cell)CellsCollection[ii];
                                    cell.IsSelected = false;
                                    cell.IsMouseHover = false;
                                    CheckCurrentDate((DayCell)CellsCollection[ii]);
                                    ApplyNullBorderValue(b);
                                }

                                if (m_dc.Date == date)
                                {
                                    b = BorderCellsCollection[ii];
                                    Cell cell = (Cell)CellsCollection[ii];
                                    m_MouseHoverCell = cell;
                                    MouseHoverDate = ((DayCell)cell).Date;
                                    cell.IsMouseHover = true;
                                    cell.Foreground = SelectedCell_ForeColor;
                                    ApplyBorderForCell(b);
                                }
                            }

                            ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                        }

                        if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                    }

                    if (e.Key == Key.Up)
                    {
                        if (0 <= iNewFocusedCellIndex - 7)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex - 7]).Visibility != Visibility.Collapsed)
                            {
                                if (((DayCell)CellsCollection[iNewFocusedCellIndex]).Date != SelectedDate)
                                {
                                    ApplyNullBorderValue(b);
                                    ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                    CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                    CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                    CellsCollection[iNewFocusedCellIndex].IsSelected = false;
                                }

                                b = BorderCellsCollection[iNewFocusedCellIndex - 7];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex - 7];
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.IsMouseHover = true;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                            }
                            else
                            {
                                ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                            }
                        }
                        else
                        {
                            ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                            Border b1 = (Border)((Cell)m_MouseHoverCell).Parent;
                            ApplyBorderForCell(b1);
                        }

                        if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                    }

                    if (e.Key == Key.Down)
                    {
                        if (CellsCollection.Count > iNewFocusedCellIndex + 7)
                        {
                            if (((Border)BorderCellsCollection[iNewFocusedCellIndex + 7]).Visibility != Visibility.Collapsed)
                            {
                                if (((DayCell)CellsCollection[iNewFocusedCellIndex]).Date != SelectedDate)
                                {
                                    ApplyNullBorderValue(b);
                                    ////    CellsCollection[iNewFocusedCellIndex].Foreground = Cell_ForeColor;
                                    CheckCurrentDate((DayCell)CellsCollection[iNewFocusedCellIndex]);
                                    CellsCollection[iNewFocusedCellIndex].IsMouseHover = false;
                                    CellsCollection[iNewFocusedCellIndex].IsSelected = false;
                                }

                                b = BorderCellsCollection[iNewFocusedCellIndex + 7];
                                Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex + 7];
                                m_MouseHoverCell = cell;
                                MouseHoverDate = ((DayCell)cell).Date;
                                cell.IsMouseHover = true;
                                cell.Foreground = SelectedCell_ForeColor;
                                ApplyBorderForCell(b);
                            }
                            else
                            {
                                ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                            }
                        }
                        else if (!hasDate)
                        {
                            Cell c = m_MouseHoverCell;
                            Border b1 = (Border)c.Parent;
                            c.Foreground = SelectedCell_ForeColor;
                            ApplyBorderForCell(b);
                        }
                        else
                        {
                            ((Cell)m_MouseHoverCell).Foreground = SelectedCell_ForeColor;
                        }
                    }

                    if (!hasDate)
                    {
                        Cell c = m_MouseHoverCell;
                        Border b1 = (Border)c.Parent;
                        c.Foreground = SelectedCell_ForeColor;
                        ApplyBorderForCell(b);
                    }
                }

                m_MouseHoverCell.IsSelected = true;
            }
            else
            {
                for (int i = 0; i < BorderCellsCollection.Count; i++)
                {
                    Border b = (Border)BorderCellsCollection[i];
                    ApplyNullBorderValue(b);
                    ////    ((Cell)CellsCollection[i]).Foreground = Cell_ForeColor;
                    CheckCurrentDate((DayCell)CellsCollection[i]);
                }

                if (((Border)BorderCellsCollection[iNewFocusedCellIndex]).Visibility != Visibility.Collapsed)
                {
                    CtrlDateCellsCollection.Clear();
                    Border b = BorderCellsCollection[iNewFocusedCellIndex];
                    Cell cell = (Cell)CellsCollection[iNewFocusedCellIndex];
                    m_SelectedDateCell = cell;
                    m_SelectedDateBorderCell = b;
                    SelectedDate = ((DayCell)cell).Date;
                    CtrlDateCellsCollection.Add(((DayCell)cell).Date);
                    cell.IsSelected = true;
                    cell.IsMouseHover = true;
                    m_MouseHoverCell = cell;
                    MouseHoverDate = ((DayCell)cell).Date;
                    m_EnterDate = ((DayCell)cell).Date;
                    cell.Foreground = SelectedCell_ForeColor;
                    ApplyBorderForCell(b);
                    dc.IsSelected = false;
                    dc.IsMouseHover = false;
                    ParentCalendar.ShowDayGridonSelectedCell(MouseHoverDate);
                    ParentCalendar.setCurrentDate(MouseHoverDate);
                }
            }

            if (MouseHoverDate != TodayDate)
            {
                ParentCalendar.ApplyBorderTodayDate(TodayDate);
            }

            e.Handled = true;
        }

        /// <summary>
        /// Sets the day cell date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="format">The format.</param>
        /// <param name="calendar">The calendar.</param>
        protected internal void SetDayCellDate(VisibleDate date, DateTimeFormatInfo format, System.Globalization.Calendar calendar)
        {
            int month = date.VisibleMonth;
            int year = date.VisibleYear;
            DateTime dateMonthStart = GetFirstDayOfMonth(year, month, calendar);
            DayOfWeek dw = calendar.GetDayOfWeek(dateMonthStart);
            int iFirstDayOfMonth = (int)dw;
            Date resultDate = new Date();
            int iFirstDayOfWeek = (int)format.FirstDayOfWeek;
            int iDaysInMonth = calendar.GetDaysInMonth(year, month);
            ////    Date resultDate = new Date();
            int first = ((6 + iFirstDayOfMonth - iFirstDayOfWeek) % 7) + 1;
            CalenderMatrix = GenerateMatrix(month, year, format, calendar);

            for (int i = 0; i < CellsCollection.Count; i++)
            {
                int r = minnerGrid.RowDefinitions.Count;
                int m = minnerGrid.ColumnDefinitions.Count;
                int iRow = Grid.GetRow((FrameworkElement)BorderCellsCollection[i]) - 1;
                int iColumn = Grid.GetColumn((FrameworkElement)BorderCellsCollection[i]);
                ////    (CellsCollection[i] as Cell).Content = CalenderMatrix[iRow, iColumn];
                DayCell dc = (DayCell)CellsCollection[i];
                ////    current month
                if ((i >= first) && (i < iDaysInMonth + first))
                {
                    resultDate.Year = year;
                    resultDate.Month = month;
                    resultDate.Day = CalenderMatrix[iRow, iColumn];
                    if (((Border)BorderCellsCollection[i]).Visibility == Visibility.Collapsed)
                    {
                        ((Border)BorderCellsCollection[i]).Visibility = Visibility.Visible;
                    }

                    ((DayCell)CellsCollection[i]).Foreground = Cell_ForeColor;
                }

                ////    month before
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

                    resultDate.Month = AddMonth(month, -1);
                    resultDate.Day = CalenderMatrix[iRow, iColumn];
                    if (!ShowPreviousMonthDaysProperty)
                    {
                        ((Border)BorderCellsCollection[i]).Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (((Border)BorderCellsCollection[i]).Visibility == Visibility.Collapsed)
                        {
                            ((Border)BorderCellsCollection[i]).Visibility = Visibility.Visible;
                        }
                    }

                    ((DayCell)CellsCollection[i]).Foreground = PreviousMonthDays_ForeColor;
                    ////    dc.Visibility = Visibility.Collapsed;
                }
                ////    month after current
                if (i >= iDaysInMonth + first)
                {
                    if (month == 12)
                    {
                        resultDate.Year = year + 1;
                    }
                    else
                    {
                        resultDate.Year = year;
                    }

                    resultDate.Month = AddMonth(month, 1);
                    resultDate.Day = CalenderMatrix[iRow, iColumn];
                    if (!ShowNextMonthDaysProperty)
                    {
                        ((Border)BorderCellsCollection[i]).Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (((Border)BorderCellsCollection[i]).Visibility == Visibility.Collapsed)
                        {
                            ((Border)BorderCellsCollection[i]).Visibility = Visibility.Visible;
                        }

                        ((DayCell)CellsCollection[i]).Foreground = NextMonthDays_ForeColor;
                    }
                }

                Date maxDate = new Date(ParentCalendar.MaxDate, calendar);
                Date minDate = new Date(ParentCalendar.MinDate, calendar);

                if (resultDate > maxDate || resultDate < minDate)
                {
                    ((Border)BorderCellsCollection[i]).Visibility = Visibility.Collapsed;
                }
                else
                {
                    dc.Content = resultDate.Day;
                    dc.Date = resultDate;
                }
            }

            if (ParentCalendar.TooltipDates.Count > 0)
            {
                for (int m = 0; m < ParentCalendar.TooltipDates.Count; m++)
                {
                    for (int b = 0; b < BorderCellsCollection.Count; b++)
                    {
                        if (((DayCell)CellsCollection[b]).Date == (Date)ParentCalendar.TooltipDates[m])
                        {
                            ToolTipService.SetToolTip((Border)BorderCellsCollection[b], ParentCalendar.TooltipValue[m]);
                            break;
                        }
                    }
                }
            }

            UpdateDateCells(calendar, date.VisibleMonth);
        }

        /// <summary>
        /// Sets the cell's content.
        /// </summary>
        /// <param name="format">The format.</param>
        protected internal void SetDayNames(DateTimeFormatInfo format)
        {
            int iFirstDayOfWeek = (int)format.FirstDayOfWeek;
            string[] dayNames;
            dayNames = format.DayNames;
            if (ParentCalendar.IsDayNameAbbreviated)
            {
                for (int i = 0; i < dayNames.Length; i++)
                {
                    if (dayNames[i].Length >= NumCharMonthName)
                    {
                        dayNames[i] = dayNames[i].Substring(0, NumCharMonthName);
                    }
                }
            }

            int curDay;
            for (int i = 0; i < DayNameCellsCollection.Count; i++)
            {
                curDay = i + iFirstDayOfWeek;
                curDay %= 7;
                (DayNameCellsCollection[i] as DayNameCell).VerticalAlignment = VerticalAlignment.Center;
                (DayNameCellsCollection[i] as DayNameCell).HorizontalAlignment = HorizontalAlignment.Center;
                (DayNameCellsCollection[i] as DayNameCell).Foreground = Days_ForeColor;
                (DayNameCellsCollection[i] as DayNameCell).Content = dayNames[curDay];
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
        /// Sets the selected element.
        /// </summary>
        /// <param name="date">Current date.</param>
        public virtual void SetIsSelected(VisibleDate date)
        {
        }

        /// <summary>
        /// Sets <see cref="DayCell.IsToday"/> to true if the cell date is equal to
        /// today's date.
        /// </summary>
        /// <param name="calendar">The <see cref="System.Globalization.Calendar"/> for date transformation.</param>
        protected internal void SetIsToday(System.Globalization.Calendar calendar)
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
        /// Shows the next month days.
        /// </summary>
        /// <param name="visibleMonth">The visible month.</param>
        public void ShowNextMonthDays(int visibleMonth)
        {
            if (ShowNextMonthDaysProperty)
            {
                int nextMonth = GetNextMonth(visibleMonth);
                for (int i = 0; i < CellsCollection.Count; i++)
                {
                    if (((DayCell)CellsCollection[i]).Date.Month == nextMonth)
                    {
                        ((Border)BorderCellsCollection[i]).Visibility = Visibility.Visible;
                        ((DayCell)CellsCollection[i]).Foreground = NextMonthDays_ForeColor;
                    }
                }
            }
            else
            {
                int nextMonth = GetNextMonth(visibleMonth);
                for (int i = 0; i < CellsCollection.Count; i++)
                {
                    if (((DayCell)CellsCollection[i]).Date.Month == nextMonth)
                    {
                        ((Border)BorderCellsCollection[i]).Visibility = Visibility.Collapsed;
                        ////    ((DayCell)CellsCollection[i]).Foreground = NextMonthDays_ForeColor;
                    }
                }
            }
        }

        /// <summary>
        /// Shows the previous month days.
        /// </summary>
        /// <param name="visibleMonth">The visible month.</param>
        public void ShowPreviousMonthDays(int visibleMonth)
        {
            if (ShowPreviousMonthDaysProperty)
            {
                int previousMonth = GetPrevMonth(visibleMonth);
                for (int i = 0; i < CellsCollection.Count; i++)
                {
                    if (((DayCell)CellsCollection[i]).Date.Month == previousMonth)
                    {
                        ((Border)BorderCellsCollection[i]).Visibility = Visibility.Visible;
                        ((DayCell)CellsCollection[i]).Foreground = PreviousMonthDays_ForeColor;
                    }
                }
            }
            else
            {
                int previousMonth = GetPrevMonth(visibleMonth);
                for (int i = 0; i < CellsCollection.Count; i++)
                {
                    if (((DayCell)CellsCollection[i]).Date.Month == previousMonth)
                    {
                        ((Border)BorderCellsCollection[i]).Visibility = Visibility.Collapsed;
                        ////    ((DayCell)CellsCollection[i]).Foreground = PreviousMonthDays_ForeColor;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the date cells.
        /// </summary>
        /// <param name="calendar">The calendar.</param>
        /// <param name="visibleMonth">The visible month.</param>
        private void UpdateDateCells(System.Globalization.Calendar calendar, int visibleMonth)
        {
            mweekNumbers.Clear();

            foreach (DayCell dayCell in CellsCollection)
            {
                if (dayCell.Visibility != Visibility.Collapsed)
                {
                    DateTime date = dayCell.Date.ToDateTime(calendar);
                    int number = ParentCalendar.GetWeekNumber(date);

                    if (date.DayOfWeek == DayOfWeek.Thursday && !mweekNumbers.Contains(number))
                    {
                        mweekNumbers.Add(number);
                    }
                }
            }

            FillWeekNumberCells();
        }

        /// <summary>
        /// Validates the index of the focus.
        /// </summary>
        /// <returns></returns>
        private int ValidateFocusIndex()
        {
            int iNewFocusedCellIndex = -1;
            bool hasDate = false;

            for (int i = 0; i < CellsCollection.Count; i++)
            {
                Cell dc = (Cell)CellsCollection[i];
                if (((DayCell)dc).Date != SelectedDate)
                {
                    dc.IsSelected = false;
                    dc.IsMouseHover = false;
                    Border b_new = (Border)BorderCellsCollection[i];
                    CheckCurrentDate(((DayCell)CellsCollection[i]));
                    ApplyNullBorderValue(b_new);
                }

                if (((DayCell)dc).Date == MouseHoverDate)
                {
                    hasDate = true;
                    if (!(iNewFocusedCellIndex >= 0))
                    {
                        iNewFocusedCellIndex = i;
                    }
                }

                if (((DayCell)dc).Date == SelectedDate)
                {
                    if (ShowBlackBorderForKeyEvent)
                    {
                        Border b_new = (Border)BorderCellsCollection[i];
                        b_new.BorderBrush = new SolidColorBrush(Colors.Black);
                        b_new.CornerRadius = new CornerRadius(3);
                        b_new.BorderThickness = new Thickness(1);
                        b_new.Background = new SolidColorBrush();
                    }
                    else
                    {
                        Border b_new = (Border)BorderCellsCollection[i];
                        CheckCurrentDate(((DayCell)CellsCollection[i]));
                        ApplyNullBorderValue(b_new);
                    }
                }
            }

            if (!hasDate)
            {
                for (int i = 0; i < CellsCollection.Count; i++)
                {
                    Cell dc = (Cell)CellsCollection[i];
                    if (dc.IsSelected)
                    {
                        iNewFocusedCellIndex = i;
                    }
                }
            }

            return iNewFocusedCellIndex;
        }
    }
}

/// <summary>
/// Represents the Struct date.
/// </summary>
public struct Date
{
    private int mYear;
    private int mMonth;
    private int mDay;

    /// <summary>
    /// Gets or sets the year.
    /// </summary>
    /// <value>The year.</value>
    public int Year
    {
        get
        {
            return mYear;
        }

        set
        {
            mYear = value;
        }
    }

    /// <summary>
    /// Gets or sets the month.
    /// </summary>
    /// <value>The month.</value>
    public int Month
    {
        get
        {
            return mMonth;
        }

        set
        {
            mMonth = value;
        }
    }

    /// <summary>
    /// Gets or sets the day.
    /// </summary>
    /// <value>The day.</value>
    public int Day
    {
        get { return mDay; }

        set { mDay = value; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Date"/> struct.
    /// </summary>
    /// <param name="year">The year.</param>
    /// <param name="month">The month.</param>
    /// <param name="day">The day.</param>
    public Date(int year, int month, int day)
    {
        mYear = year;
        mMonth = month;
        mDay = day;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Date"/> struct.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="calendar">The calendar.</param>
    public Date(DateTime date, System.Globalization.Calendar calendar)
    {
        try
        {
            mYear = calendar.GetYear(date);
            mMonth = calendar.GetMonth(date);
            mDay = calendar.GetDayOfMonth(date);
        }
        catch
        {
            mYear = date.Year;
            mMonth = date.Month;
            mDay = date.Day;
        }
    }

    /// <summary>
    /// Toes the date time.
    /// </summary>
    /// <param name="calendar">The calendar.</param>
    /// <returns></returns>
    public DateTime ToDateTime(System.Globalization.Calendar calendar)
    {
        if (calendar == null)
        {
            throw new ArgumentNullException("Calendar object cannot be null");
        }

        try
        {
            return new DateTime(mYear, mMonth, mDay, calendar);
            ////    object temp = m_Month.ToString() + "/" + m_Day.ToString() + "/" + m_Year.ToString();
            ////    return Convert.ToDateTime(temp, );
        }
        catch
        {
            ////    object temp = m_Month.ToString() + "/" + m_Day.ToString() + "/" + m_Year.ToString();
            ////    return Convert.ToDateTime(temp, Culture.DateTimeFormat);
            return new DateTime(mYear, mMonth, mDay);
        }
    }

    /// <summary>
    /// Adds the month to date.
    /// </summary>
    /// <param name="month">The month.</param>
    /// <returns></returns>
    public Date AddMonthToDate(int month)
    {
        Date result = this;
        int months = 0;
        int years = 0;
        int count = Math.Abs(month);

        if (count > 12)
        {
            years = month / 12;
            months = month % 12;
        }
        else
        {
            months = month;
        }

        int tempMonth = result.Month + months;
        result.Year += years;

        if (tempMonth > 12)
        {
            result.Year += tempMonth / 12;
            result.Month = tempMonth % 12;
        }

        if (tempMonth < 1)
        {
            result.Month = 12 + tempMonth;
            result.Year--;
        }

        if (tempMonth >= 1 && tempMonth <= 12)
        {
            result.Month = tempMonth;
        }

        result.Day = 1;
        return result;
    }
    #region Operators
    /// <summary>
    /// Determines whether two specified instances of <see cref="Date"/> are equal.
    /// </summary>
    /// <param name="a">First operand of comparison.</param>
    /// <param name="b">Second operand of comparison.</param>
    /// <returns>True if a and b represent the same date; otherwise, false.</returns>
    public static bool operator ==(Date a, Date b)
    {
        return a.Year == b.Year && a.Month == b.Month && a.Day == b.Day;
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="Date"/> are not
    /// equal.
    /// </summary>
    /// <param name="a">First operand of comparison.</param>
    /// <param name="b">Second operand of comparison.</param>
    /// <returns>
    /// True if a and b do not represent the same date; otherwise,
    /// false.
    /// </returns>
    public static bool operator !=(Date a, Date b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Determines whether one specified <see cref="Date"/> is greater than another
    /// specified <see cref="Date"/>.
    /// </summary>
    /// <param name="a">First operand of comparison.</param>
    /// <param name="b">Second operand of comparison.</param>
    /// <returns>
    /// True if a is greater than b; otherwise, false.
    /// </returns>
    public static bool operator >(Date a, Date b)
    {
        if (a.Year > b.Year)
        {
            return true;
        }

        if (a.Year < b.Year)
        {
            return false;
        }
        else
        {
            if (a.Month > b.Month)
            {
                return true;
            }

            if (a.Month < b.Month)
            {
                return false;
            }
            else
            {
                if (a.Day > b.Day)
                {
                    return true;
                }

                if (a.Day < b.Day)
                {
                    return false;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether one specified <see cref="Date"/> is less than another
    /// specified <see cref="Date"/>.
    /// </summary>
    /// <param name="a">First operand of comparison.</param>
    /// <param name="b">Second operand of comparison.</param>
    /// <returns>True if a is less than b; otherwise, false.</returns>
    public static bool operator <(Date a, Date b)
    {
        if (!(a > b) && a != b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether one specified <see cref="Date"/> is greater than or
    /// equal to another specified <see cref="Date"/>.
    /// </summary>
    /// <param name="a">First operand of comparison.</param>
    /// <param name="b">Second operand of comparison.</param>
    /// <returns>True if a is less than b; otherwise, false. </returns>
    public static bool operator >=(Date a, Date b)
    {
        if (a > b || a == b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether one specified <see cref="Date"/> is less than or equal
    /// to another specified <see cref="Date"/>.
    /// </summary>
    /// <param name="a">First operand of comparison.</param>
    /// <param name="b">Second operand of comparison.</param>
    /// <returns>True if a is less than b; otherwise, false. </returns>
    public static bool operator <=(Date a, Date b)
    {
        if (a < b || a == b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
    /// </returns>
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    /// <summary>
    /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
    /// </summary>
    /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
    /// <returns>
    /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
    /// </returns>
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    #endregion
}