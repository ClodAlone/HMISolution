#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Syncfusion.Silverlight.Shared;
using Syncfusion.Windows.Tools.Controls;
using System.Collections.ObjectModel;
namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Main class for Calendar Control.
    /// </summary>
    #region Template Parts

    [TemplatePart(Name = "DateGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "weekGrid", Type = typeof(Grid))]  ////WeekNumber
    [TemplatePart(Name = "WeekNumber", Type = typeof(StackPanel))]
    [TemplatePart(Name = "RootElement", Type = typeof(Grid))]
    [TemplatePart(Name = "Previous", Type = typeof(Polygon))]
    [TemplatePart(Name = "Next", Type = typeof(Polygon))]
    [TemplatePart(Name = "MonthText", Type = typeof(TextBlock))]
    [TemplatePart(Name = "TodayDate", Type = typeof(TextBlock))]
    [TemplatePart(Name = "TodayDatebtn", Type = typeof(TextBlock))]
    //[TemplatePart(Name = TreeViewItemAdv.ElementRootName, Type = typeof(FrameworkElement))]
    //[TemplateVisualState(Name = "Selected", GroupName = "SelectionStates")]
    #endregion

    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Blend,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Blend;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Blue,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Office2007Blue;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Black,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Office2007Black;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2007Silver,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Office2007Silver;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Default,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Default;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Blue,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Office2010Blue;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Black,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Office2010Black;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Office2010Silver,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Office2010Silver;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Windows7,
        Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Windows7;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.VS2010,
       Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.VS2010;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Metro,
      Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Metro;component/CalendarControl.xaml")]
    [Syncfusion.Windows.Shared.SkinType(SkinVisualStyle = Syncfusion.Windows.Shared.VisualStyle.Transparent,
     Type = typeof(CalendarControl), XamlResource = "/Syncfusion.Theming.Transparent;component/CalendarControl.xaml")]
    public partial class CalendarControl : ContentControl
    {
        /// <summary>
        /// 
        /// </summary>
        protected internal Brush buttonDisabledColor = new SolidColorBrush(Color.FromArgb(0xFF, 0x68, 0x92, 0xCF));

        /// <summary>
        /// Get the grid from the generic.xaml.
        /// </summary>
        private Grid dateGrid;
        private int isLoaded = 0;
       
        /// <summary>
        /// Maximum date supported by the current <see cref="System.Globalization.Calendar"/>.
        /// </summary>
        private DateTime mMaxDate;

        /// <summary>
        /// Minimum date supported by the current <see cref="System.Globalization.Calendar"/>.
        /// </summary>
        private DateTime mMinDate;

        /// <summary>
        /// Changes the visual mode depending on the <see cref="VisualMode"/> property.
        /// </summary>
        Storyboard m_monthStoryboard;

        private MonthPopup mPopup;

        private List<Date> mSelectedDatesList;

        private List<Date> m_toolTipDates = new List<Date>();

        private List<ToolTip> mtoolTipValue = new List<ToolTip>();

        /// <summary>
        /// Defines calendar visual mode.
        /// </summary>
        ////private static MonthGrid ObjMonth = new MonthGrid();
        private CalendarVisualMode mVisualMode;

        /// <summary>
        /// Get the TextBlock from the generic.xaml.
        /// </summary>
        private TextBlock monthText;

        /// <summary>
        /// Get the Polygon for Next from the generic.xaml.
        /// </summary>
        protected internal Polygon next;
        /// <summary>
        /// 
        /// </summary>
        protected internal Ellipse circle;
        /// <summary>
        /// 
        /// </summary>
        protected internal Ellipse circle1;

        private static int objLoaded = 0;

        /// <summary>
        /// Get the grid from the generic.xaml.
        /// </summary>
        private Grid outerGrid;

        /// <summary>
        /// Get the Polygon for previous from the generic.xaml.
        /// </summary>
        protected internal Polygon previous;

        private DispatcherTimer timer, timer1, ToDayDateChange, storyBoardTimer;
        private Popup tempPopUp;

        /// <summary>
        /// Get the TextBlock from the generic.xaml.
        /// </summary>
        private TextBlock todayDatebtn;

        /// <summary>
        /// Get the TextBlock from the generic.xaml.
        /// </summary>
        private TextBlock todayDateControl;

        /// <summary>
        /// Get the grid from the generic.xaml.
        /// </summary>
        private Grid weekGrid;
        private StackPanel weekNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarControl"/> class.
        /// </summary>
        public CalendarControl()
        {
            objLoaded += 1;

            this.DefaultStyleKey = typeof(CalendarControl);
            
            if (objLoaded == 1)
            {
                DayNamesAndDateGrid = new DayGrid();
            }
            else if (objLoaded == 2)
            {
                objLoaded = 0;
            }

            VisibleDate date;
            date.VisibleMonth = Calendar.GetMonth(Date);
            date.VisibleYear = Calendar.GetYear(Date);
            VisibleData = date;
            MinDate = Calendar.MinSupportedDateTime;
            MaxDate = Calendar.MaxSupportedDateTime;
            TodayDate = DateTime.Now.ToString("D", Culture.DateTimeFormat);
           //SkinList list = SkinManager.GetSkinList(this);
        }

        /// <summary>
        /// Initializes the <see cref="CalendarControl"/> class.
        /// </summary>
        static CalendarControl()
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                Syncfusion.Windows.Shared.LoadDependentAssemblies load = new Syncfusion.Windows.Shared.LoadDependentAssemblies();
                load = null;
            }
        }
        /// <summary>
        /// Event that is raised when <see cref="AllowMultipleSelection"/> property is
        /// changed.
        /// </summary>
        public event PropertyChangedCallback AllowMultipleSelectionChanged;

        /// <summary>
        /// Occurs when [background property changed].
        /// </summary>
        public event PropertyChangedCallback BackgroundPropertyChanged;

        /// <summary>
        /// Event that is raised when <see cref="Calendar"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CalendarChanged;

        ////protected internal event MouseButtonEventHandler MonthCellMouseLeftButtonUp;

        /// <summary>
        /// Event that is raised when <see cref="CalendarStyle"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CalendarStyleChanged;

        /// <summary>
        /// Event that is raised when <see cref="CultureInfo"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback CultureChanged;

        /// <summary>
        /// Event that is raised when <see cref="Date"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback DateChanged;

        /// <summary>
        /// Occurs when [date foreground property changed].
        /// </summary>
        public event PropertyChangedCallback DateForegroundPropertyChanged;

        /// <summary>
        /// Occurs when [day foreground changed].
        /// </summary>
        public event PropertyChangedCallback DayForegroundChanged;

        /// <summary>
        /// Occurs when [days abbreviation length changed].
        /// </summary>
        public event PropertyChangedCallback DaysAbbreviationLengthChanged;

        /// <summary>
        /// Occurs when [is day name abbreviated changed].
        /// </summary>
        public event PropertyChangedCallback IsDayNameAbbreviatedChanged;

        /// <summary>
        /// Occurs when [is month name abbreviated changed].
        /// </summary>
        public event PropertyChangedCallback IsMonthNameAbbreviatedChanged;

        /// <summary>
        /// Occurs when [is show week numbers changed].
        /// </summary>
        public event PropertyChangedCallback IsShowWeekNumbersChanged;

        /// <summary>
        /// Occurs when [month change direction changed].
        /// </summary>
        public event PropertyChangedCallback MonthChangeDirectionChanged;

        /// <summary>
        /// Occurs when [mouse hover border brush changed].
        /// </summary>
        public event PropertyChangedCallback MouseHoverBorderBrushChanged;

        /// <summary>
        /// Occurs when [mouse hover cell background brush changed].
        /// </summary>
        public event PropertyChangedCallback MouseHoverCellBackgroundBrushChanged;

        /// <summary>
        /// Occurs when [mouse hover cell border thickness changed].
        /// </summary>
        public event PropertyChangedCallback MouseHoverCellBorderThicknessChanged;

        /// <summary>
        /// Occurs when [mouse hover cell corner radius changed].
        /// </summary>
        public event PropertyChangedCallback MouseHoverCellCornerRadiusChanged;

        /// <summary>
        /// Occurs when [mouse hover foreground property changed].
        /// </summary>
        public event PropertyChangedCallback MouseHoverForegroundPropertyChanged;

        /// <summary>
        /// Occurs when [mouse wheel].
        /// </summary>
        public new event EventHandler<MouseWheelEventArgs> MouseWheel;

        /// <summary>
        /// Occurs when [next month days foreground changed].
        /// </summary>
        public event PropertyChangedCallback NextMonthDaysForegroundChanged;

        /// <summary>
        /// Occurs when [previous month days foreground changed].
        /// </summary>
        public event PropertyChangedCallback PreviousMonthDaysForegroundChanged;

        /// <summary>
        /// Occurs when [scroll button fill property changed].
        /// </summary>
        public event PropertyChangedCallback ScrollButtonFillPropertyChanged;

        /// <summary>
        /// Event that is fired when <see cref="SelectedCellBackground"/> Property Changed
        /// </summary>
        public event PropertyChangedCallback SelectedCellBackgroundChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedCellBorderThickness"/> Property changed
        /// </summary>
        public event PropertyChangedCallback SelectedCellBorderThicknessChanged;

        /// <summary>
        /// Event that is raised when <see cref="SelectedCellCornerRadius"/> property changed 
        /// </summary>
        public event PropertyChangedCallback SelectedCellCornerRadiusChanged;

        /// <summary>
        /// Occurs when [selected cell foreground property changed].
        /// </summary>
        public event PropertyChangedCallback SelectedCellForegroundPropertyChanged;

        /// <summary>
        /// Occurs when [selected date changed].
        /// </summary>
        public event PropertyChangedCallback SelectedDateChanged;

        /// <summary>
        /// Occurs when [selected date changed].
        /// </summary>
        public event PropertyChangedCallback SelectedDatesChanged;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback SelectionBorderBrushChanged;

        /// <summary>
        /// Occurs when [selection range mode changed].
        /// </summary>
        public event PropertyChangedCallback SelectionRangeModeChanged;

        /// <summary>
        /// Occurs when [show next month days changed].
        /// </summary>
        public event PropertyChangedCallback ShowNextMonthDaysChanged;

        /// <summary>
        /// Occurs when [show previous month days changed].
        /// </summary>
        public event PropertyChangedCallback ShowPreviousMonthDaysChanged;

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback TodayRowIsVisibleChanged;

        /// <summary>
        /// Event that is raised when <see cref="VisibleData"/> property is changed.
        /// </summary>
        public event PropertyChangedCallback VisibleDataChanged;

        /// <summary>
        /// Gets or sets the current date.
        /// </summary>
        /// <value>The current date.</value>
        private DateTime CurrentDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is applied border today date.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is applied border today date; otherwise, <c>false</c>.
        /// </value>
        protected internal bool IsAppliedBorderTodayDate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum date.
        /// </summary>
        /// <value>
        /// Type: <see cref="DateTime"/>
        /// </value>
        /// <seealso cref="DateTime"/>
        public DateTime MaxDate
        {
            get
            {
                return mMaxDate;
            }

            set
            {
                mMaxDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the minimum date.
        /// </summary>
        /// <value>
        /// Type: <see cref="DateTime"/>
        /// </value>
        /// <seealso cref="DateTime"/>
        public DateTime MinDate
        {
            get
            {
                return mMinDate;
            }

            set
            {
                mMinDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected dates list.
        /// </summary>
        /// <value>The selected dates list.</value>
        protected internal List<Date> SelectedDatesList
        {
            get
            {
                return mSelectedDatesList;
            }

            set
            {
                mSelectedDatesList = value;
            }
        }

        /// <summary>
        /// Gets tooltip from the date.
        /// </summary>
        internal List<Date> TooltipDates
        {
            get
            {
                return m_toolTipDates;
            }
        }

        /// <summary>
        /// Gets tooltip from the date.
        /// </summary>
        internal List<ToolTip> TooltipValue
        {
            get
            {
                return mtoolTipValue;
            }
        }

        /// <summary>
        /// Gets or sets the calendar visual mode.
        /// </summary>
        protected CalendarVisualMode VisualMode
        {
            get
            {
                return mVisualMode;
            }

            set
            {
                if (mVisualMode != value)
                {
                    mVisualMode = value;
                }
            }
        }

        /// <summary>
        /// Adds the specified number of months to the visible month.
        /// </summary>
        /// <param name="month">Number of months. It can be
        /// negative or positive.</param>
        private void AddMonth(int month)
        {
            VisibleDate result = VisibleData;
            int count = Math.Abs(month);
            int months = 0;
            int years = 0;

            if (count >= 12)
            {
                years = month / 12;
                months = month % 12;
            }
            else
            {
                months = month;
            }

            int tempMonth = VisibleData.VisibleMonth + months;
            result.VisibleYear += years;
            if (tempMonth > 12)
            {
                result.VisibleYear += tempMonth / 12;
                result.VisibleMonth = tempMonth % 12;
            }

            if (tempMonth < 1)
            {
                result.VisibleMonth = 12 + tempMonth;
                result.VisibleYear--;
            }

            if (tempMonth >= 1 && tempMonth <= 12)
            {
                result.VisibleMonth = tempMonth;
            }

            VisibleData = result;
        }

        /// <summary>
        /// Adds the week number grid.
        /// </summary>
        private void AddWeekNumberGrid()
        {
            if (!outerGrid.Children.Contains(weekGrid))
            {
                outerGrid.Children.Add(weekGrid);
                Grid.SetColumnSpan((FrameworkElement)dateGrid, 3);
                Grid.SetRow((FrameworkElement)dateGrid, 1);
                Grid.SetColumn((FrameworkElement)dateGrid, 1);
            }
        }

        /// <summary>
        /// Adds the specified number of years to the visible year.
        /// </summary>
        /// <param name="year">Number of years. It can be
        /// negative or positive.</param>
        private void AddYear(int year)
        {
            VisibleDate result = VisibleData;
            result.VisibleYear += year;
            Date minDate = new Date(MinDate, Calendar);
            Date maxDate = new Date(MaxDate, Calendar);
            if (result.VisibleYear <= maxDate.Year)
            {
                VisibleData = result;
            }
        }

        /// <summary>
        /// Applies the border control dates.
        /// </summary>
        void ApplyBorderControlDates()
        {
            if (DayNamesAndDateGrid.CtrlDateCellsCollection.Count >= 1)
            {
                foreach (Date date in DayNamesAndDateGrid.CtrlDateCellsCollection)
                {
                    if (!SelectedDates.Contains(date.ToDateTime(Calendar)))
                    {
                        SelectedDates.Add(date.ToDateTime(Calendar));
                    }
                }
            }
        }

        /// <summary>
        /// Applies the border for shifted date.
        /// </summary>
        protected internal void ApplyBorderForShiftedDate()
        {
            for (int i = 0; i < DayNamesAndDateGrid.BorderCellsCollection.Count; i++)
            {
                if (((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected)
                {
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = false;
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = false;
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                    Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                    if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date > curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = NextMonthDaysForeground;
                    }
                    else if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date < curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = PreviousMonthDaysForeground;
                    }
                    else
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                    }

                    ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);
                }
            }

            if (DayNamesAndDateGrid.EndDate.Day != 0 && DayNamesAndDateGrid.SelectedDate.Day != 0)
            {
                DateTime endDate = DayNamesAndDateGrid.EndDate.ToDateTime(Calendar);
                DateTime startDate = DayNamesAndDateGrid.SelectedDate.ToDateTime(Calendar);
                if (SelectedDates.Count < 1)
                {
                    if (startDate <= endDate)
                    {
                        while (startDate <= endDate)
                        {
                            if (!SelectedDates.Contains(startDate))
                            {
                                SelectedDates.Add(startDate);
                            }

                            startDate = startDate.AddDays(1);
                        }
                    }
                    else
                    {
                        while (startDate >= endDate)
                        {
                            if (!SelectedDates.Contains(startDate))
                            {
                                SelectedDates.Add(startDate);
                            }

                            startDate = startDate.AddDays(-1);
                        }
                    }
                    ////Date = endDate;
                    CurrentDate = endDate;
                }
            }

            foreach (DateTime date in SelectedDates)
            {
                ////VisibleDate result = VisibleData;
                Date d = new Date(date, Calendar);
                ApplyBorderSelectedDate(d);
            }
        }

        /// <summary>
        /// Applies the border only select date.
        /// </summary>
        /// <param name="d">The d.</param>
        private void ApplyBorderOnlySelectDate(Date d)
        {
            for (int i = 0; i < DayNamesAndDateGrid.CellsCollection.Count; i++)
            {
                if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date == d)
                {
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = true;
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = true;
                    Border b = (Border)DayNamesAndDateGrid.BorderCellsCollection[i];
                    b.CornerRadius = SelectedCellCornerRadius;
                    b.BorderBrush = SelectedCellBorderBrush;
                    b.BorderThickness = SelectedCellBorderThickness;
                    b.Background = SelectedCellBackground;
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = SelectedCellForeground;
                    b.UpdateLayout();
                }
                else
                {
                    ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = false;
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = false;
                    Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                    if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date > curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = NextMonthDaysForeground;
                    }
                    else if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date < curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = PreviousMonthDaysForeground;
                    }
                    else
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                    }
                    ////((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = CellForeground;
                }
            }
        }

        /// <summary>
        /// Applies the border selected date.
        /// </summary>
        /// <param name="d">The d.</param>
        protected internal void ApplyBorderSelectedDate(Date d)
        {
            for (int i = 0; i < DayNamesAndDateGrid.CellsCollection.Count; i++)
            {
                if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date == d)
                {
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = true;
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = true;
                    Border b = (Border)DayNamesAndDateGrid.BorderCellsCollection[i];
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = SelectedCellForeground;                                    
                    b.CornerRadius = SelectedCellCornerRadius;
                    b.BorderBrush = SelectedCellBorderBrush;
                    b.BorderThickness = SelectedCellBorderThickness;
                    b.Background = SelectedCellBackground;                   
                    b.UpdateLayout();
                    break;
                }
            }
        }

        /// <summary>
        /// Applies the border selected date only.
        /// </summary>
        /// <param name="date">The date.</param>
        protected internal void ApplyBorderSelectedDateOnly(Date date)
        {
            for (int i = 0; i < DayNamesAndDateGrid.BorderCellsCollection.Count; i++)
            {
                ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = false;
                ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = false;
                Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date > curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                {
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = NextMonthDaysForeground;
                }
                else if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date < curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                {
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = PreviousMonthDaysForeground;
                }
                else
                {
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                }

                ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);

                if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date == date)
                {
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).Focus();
                    DayNamesAndDateGrid.m_SelectedDateCell = (Cell)DayNamesAndDateGrid.CellsCollection[i];
                    DayNamesAndDateGrid.m_MouseHoverCell = (Cell)DayNamesAndDateGrid.CellsCollection[i];
                    DayNamesAndDateGrid.MouseHoverDate = date;
                    DayNamesAndDateGrid.SelectedDate = date;
                    DayNamesAndDateGrid.m_SelectedDateBorderCell = (Border)DayNamesAndDateGrid.BorderCellsCollection[i];
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = true;
                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = true;
                    Border b = (Border)DayNamesAndDateGrid.BorderCellsCollection[i];
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = SelectedCellForeground;
                    b.CornerRadius = SelectedCellCornerRadius;
                    b.BorderBrush = SelectedCellBorderBrush;
                    b.BorderThickness = SelectedCellBorderThickness;
                    b.Background = SelectedCellBackground;
                    b.UpdateLayout();
                    ////break;
                }
            }
        }

        /// <summary>
        /// Applies the border today date.
        /// </summary>
        /// <param name="d">The d.</param>
        protected internal void ApplyBorderTodayDate(Date d)
        {
            if (!(DayNamesAndDateGrid.SelectedDate == new Date(DateTime.Now.Date, Calendar)))
            {
                for (int i = 0; i < DayNamesAndDateGrid.CellsCollection.Count; i++)
                {
                    if (!((DayCell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected)
                    {
                        if (((Border)DayNamesAndDateGrid.BorderCellsCollection[i]).BorderBrush == Foreground)
                        {
                            ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);
                        }
                        if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date == d)
                        {
                            Border b = (Border)DayNamesAndDateGrid.BorderCellsCollection[i];
                            if (((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected)
                            {
                                ((Cell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = SelectedCellForeground; ////CellForeground;
                            }
                            else
                            {
                                Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                                if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date > curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                {
                                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = NextMonthDaysForeground;
                                }
                                else if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date < curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                                {
                                    ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = PreviousMonthDaysForeground;
                                }
                                else
                                {
                                    if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Background == DayNamesAndDateGrid.MouseHoverCellBackground)
                                    {
                                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                                    }
                                }
                                ////((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = CellForeground;//CellForeground;
                            }

                            if (Foreground != null)
                            {
                                    b.BorderBrush = Foreground;
                            }
                            else
                            {
                                b.BorderBrush = Foreground;
                            }

                            b.BorderThickness = new Thickness(0.2);
                            IsAppliedBorderTodayDate = true;
                            b.UpdateLayout();
                            break;
                        }
                    }
                }
            }
            else if (DayNamesAndDateGrid.SelectedDate == new Date(DateTime.Now.Date, Calendar))
            {
                IsAppliedBorderTodayDate = false;
            }
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
        public void ApplyStoryBoard(FrameworkElement element, string move)
        {
            DoubleAnimation followingAnimation = new DoubleAnimation();
            DoubleAnimation opacityAnimation = new DoubleAnimation();
            Duration duration = new Duration(TimeSpan.FromSeconds(200));
            Storyboard mMonthStoryboard = new Storyboard();
            TransformGroup tg = new TransformGroup();
            TranslateTransform tt = new TranslateTransform();
            mMonthStoryboard.Duration = duration;
            followingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime));
            opacityAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime));
            if (move == "Prev")
            {
                if (MonthChangeDirection == AnimationDirection.Horizontal)
                {
                    tt.X = -element.ActualWidth / 6; ////0;
                    followingAnimation.To = 0;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
                }
                else
                {
                    tt.Y = -15;
                    followingAnimation.To = 0;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
                }
            }
            else
            {
                if (MonthChangeDirection == AnimationDirection.Horizontal)
                {
                    tt.X = element.ActualWidth; ////0;
                    followingAnimation.To = 0;
                    followingAnimation.From = element.ActualWidth / 6;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
                }
                else
                {
                    tt.Y = element.ActualHeight / 4;
                    followingAnimation.To = 0;
                    Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.Y)"));
                }
            }

            tg.Children.Add(tt);
            element.RenderTransform = tg;
            opacityAnimation.To = 1;
            element.Opacity = 0.00001;
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("(FrameworkElement.Opacity)"));
            Storyboard.SetTarget(opacityAnimation, element);
            mMonthStoryboard.Children.Add(opacityAnimation);
            Storyboard.SetTarget(followingAnimation, element);
            Storyboard.SetTargetName(followingAnimation, "element");
            //// Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(Height)"));
            ////followingAnimation.Completed += new EventHandler(followingAnimation_Completed);
            mMonthStoryboard.Children.Add(followingAnimation);

            if (!dateGrid.Resources.Contains("sample"))
            {
                dateGrid.Resources.Add("sample", mMonthStoryboard);
            }

            mMonthStoryboard.Begin();
        }

        /// <summary>
        /// Applies the story board particular cell.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        /// <param name="showGrid">The show grid.</param>
        /// <param name="hideGrid">The hide grid.</param>
        private void ApplyStoryBoardParticularCell(MouseButtonEventArgs e, FrameworkElement showGrid, FrameworkElement hideGrid)
        {
            if (ShowWeekNumber && dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
            {
                AddWeekNumberGrid();
                WeekNumberGrid.SetWeekNumbers(DayNamesAndDateGrid.WeekNumbers);
            }

            Type tq = e.OriginalSource.GetType();
            if (tq.FullName == "System.Windows.Controls.Border")
            {
                Border b = (Border)e.OriginalSource;

                StoryBoardForMonth(((FrameworkElement)b), 2d, 2d, showGrid, hideGrid);
            }
            else if (tq.FullName == "System.Windows.Controls.TextBlock")
            {
                TextBlock ct = (TextBlock)e.OriginalSource;
                Border b = new Border();
                if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
                {
                    for (int i = 0; i < MonthNameGrid.MonthCellsCollection.Count; i++)
                    {
                        if (MonthNameGrid.MonthCellsCollection[i].Content.ToString() == ct.Text.ToString())
                        {
                            b = (Border)MonthNameGrid.MonthBorderCollection[i];
                        }
                    }
                }
                else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
                {
                    for (int i = 0; i < YearGrid.YearCellsCollection.Count; i++)
                    {
                        if (YearGrid.YearCellsCollection[i].Content.ToString() == ct.Text.ToString())
                        {
                            b = YearGrid.YearBorderCollection[i];
                        }
                    }
                }
                else if (dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
                {
                    for (int i = 0; i < YearRangedGrid.YearCellsCollection.Count; i++)
                    {
                        if (YearRangedGrid.YearCellsCollection[i].Content != null)
                        {
                            if (YearRangedGrid.YearCellsCollection[i].Content.ToString() == ct.Text.ToString())
                            {
                                b = YearRangedGrid.YearBorderCollection[i];
                            }
                        }
                    }
                }

                StoryBoardForMonth(((FrameworkElement)b), 2d, 2d, showGrid, hideGrid);
            }

            YearGrid.m_MouseHoveredCell = null;
            YearRangedGrid.m_MouseHoveredCell = null;
            MonthNameGrid.m_MouseHoveredCell = null;
            DayNamesAndDateGrid.m_MouseHoveredCell = null;
        }

        /// <summary>
        /// AttachBrowserMouseWheelEvent attaches the decorator to the browser events.
        /// </summary>
        private void AttachBrowserMouseWheelEvent()
        {
            if (HtmlPage.IsEnabled)
            {
                if (HtmlPage.BrowserInformation.UserAgent.Contains("Firefox"))
                {
                    HtmlPage.Plugin.AttachEvent("DOMMouseScroll", this.ProcessOnMouseWheel);
                }

                if (HtmlPage.BrowserInformation.UserAgent.Contains("MSIE") ||
                    HtmlPage.BrowserInformation.UserAgent.Contains("Opera") ||
                    HtmlPage.BrowserInformation.UserAgent.Contains("Safari"))
                {
                    HtmlPage.Plugin.AttachEvent("onmousewheel", this.ProcessOnMouseWheel);
                }
            }
        }

        /// <summary>
        /// Changes the visible data.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="y">The y.</param>
        protected internal void ChangeVisibleData(int m, int y)
        {
            VisibleDate result = VisibleData;
            if (y == 0)
            {
                result.VisibleMonth = m;
                VisibleData = result;
            }

            if (m == 0)
            {
                result.VisibleYear = y;
                VisibleData = result;
            }
        }

        /// <summary>
        /// Checks the current month.
        /// </summary>
        /// <param name="currentDate">The current date.</param>
        /// <param name="parentCal">The parent cal.</param>
        /// <param name="prevMonthDisp">if set to <c>true</c> [prev month disp].</param>
        /// <param name="nextMonthDisp">if set to <c>true</c> [next month disp].</param>
        /// <returns></returns>
        protected internal bool CheckCurrentMonth(Date currentDate, ref System.Globalization.Calendar parentCal, ref bool prevMonthDisp, ref bool nextMonthDisp)
        {
            parentCal = Calendar;
            prevMonthDisp = ShowPreviousMonthDates;
            nextMonthDisp = ShowNextMonthDates;
            bool hasDate = false;
            foreach (DayCell dc in DayNamesAndDateGrid.CellsCollection)
            {
                if (dc.Date == currentDate)
                {
                    hasDate = true;
                    break;
                }
            }

            if (!hasDate)
            {
                VisibleDate d;
                d.VisibleMonth = CurrentDate.Month;
                d.VisibleYear = CurrentDate.Year;
                VisibleData = d;
                DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
                YearGrid.SetYear(VisibleData, Calendar, string.Empty);
                YearRangedGrid.SetYearRange(VisibleData, Calendar);
            }

            return hasDate;
        }

        /// <summary>
        /// Checks the status.
        /// </summary>
        /// <param name="typeGrid">The type grid.</param>
        /// <returns></returns>
        private bool CheckStatus(string typeGrid)
        {
            bool lastValue = false;
            if (typeGrid == "YearGrid")
            {
                foreach (YearCell yc in YearGrid.YearCellsCollection)
                {
                    if (yc.Visibility == Visibility.Collapsed)
                    {
                        lastValue = true;
                        break;
                    }
                }
            }

            if (typeGrid == "YearRangeGrid")
            {
                foreach (YearRangeCell yrc in YearRangedGrid.YearCellsCollection)
                {
                    if (yrc.Visibility == Visibility.Collapsed)
                    {
                        lastValue = true;
                        break;
                    }
                }
            }

            return lastValue;
        }

        /// <summary>
        /// Coerces the visible data.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        void CoerceVisibleData(DependencyPropertyChangedEventArgs e)
        {
            SetCorrectDate();
            VisibleDate result;
            Date temp = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, DayNamesAndDateGrid.SelectedDate.Day);
            if (VisibleData.VisibleMonth != 0 && VisibleData.VisibleYear != 0)
            {
                result = VisibleData;
            }

            try
            {
                result.VisibleMonth = ((System.Globalization.Calendar)e.NewValue).GetMonth(temp.ToDateTime(((System.Globalization.Calendar)e.OldValue)));
                result.VisibleYear = ((System.Globalization.Calendar)e.NewValue).GetYear(temp.ToDateTime(((System.Globalization.Calendar)e.OldValue)));
            }
            catch
            {
                Date minDate = new Date(((System.Globalization.Calendar)e.NewValue).MinSupportedDateTime, ((System.Globalization.Calendar)e.NewValue));
                Date maxDate = new Date(((System.Globalization.Calendar)e.NewValue).MaxSupportedDateTime, ((System.Globalization.Calendar)e.NewValue));

                if (temp < minDate)
                {
                    result.VisibleMonth = minDate.Month;
                    result.VisibleYear = minDate.Year;
                }
                else
                {
                    result.VisibleMonth = maxDate.Month;
                    result.VisibleYear = maxDate.Year;
                }
            }
            if (!AllowMultipleSelection)
            {
                UpdateSelectedDatesList();
            }

            ////if (result.VisibleYear < MinDate.Year)
            ////{
            ////    result.VisibleMonth = MinDate.Month;
            ////    result.VisibleYear = MinDate.Year;
            ////}
            VisibleData = result;
        }

        /// <summary>
        /// Creates the story board for prevoius_ next BTN.
        /// </summary>
        private void CreateStoryBoardForPrevoius_NextBtn()
        {
            ColorAnimation ca = new ColorAnimation();
            Storyboard sb = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromSeconds(5));
            sb.Duration = duration;
            Storyboard.SetTargetProperty(ca, new PropertyPath("Fill"));
            Storyboard.SetTarget(ca, previous);
            Storyboard.SetTargetName(ca, "prevoius");
            ca.Duration = duration;
            ca.To = Colors.Gray;

            if (!outerGrid.Resources.Contains("sample"))
            {
                outerGrid.Resources.Add("sample", sb);
            }

            sb.Begin();
        }

        /// <summary>
        /// Handles the GotFocus event of the DateGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void DateGrid_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the KeyDown event of the DateGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        void DateGrid_KeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// Handles the MouseEnter event of the DateGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DateGrid_MouseEnter(object sender, EventArgs e)
        {
            if (isLoaded == 0)
            {
                AttachBrowserMouseWheelEvent();
                isLoaded = 1;
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the DateGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void DateGrid_MouseLeave(object sender, EventArgs e)
        {
            if (isLoaded == 1)
            {
                DeAttachBrowserMouseWheelEvent();
                isLoaded = 0;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the DateGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void DateGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
            {               
                if (AllowMultipleSelection)
                {
                    if (Keyboard.Modifiers != ModifierKeys.None)
                    {
                        if ((Keyboard.Modifiers == ModifierKeys.Shift) && DayNamesAndDateGrid.EndDate.Day != 0)
                        {
                            ShowDayGridonSelectedCell(DayNamesAndDateGrid.EndDate);
                            SelectedDates.Clear();
                            DayNamesAndDateGrid.SelectedColumnDates.Clear();
                            if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
                            {
                                if (DayNamesAndDateGrid.EndDate.Day != 0 && DayNamesAndDateGrid.SelectedDate.Day != 0)
                                {
                                    ApplyBorderForShiftedDate();
                                    //DayNamesAndDateGrid.SelectedDate = DayNamesAndDateGrid.EndDate;
                                    // DayNamesAndDateGrid.MouseHoverDate = DayNamesAndDateGrid.EndDate;
                                }
                            }
                        }
                        else if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            ////ShowDayGridonSelectedCell(DayNamesAndDateGrid.SelectedDate);
                            if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
                            {
                                foreach (Date d in DayNamesAndDateGrid.SelectedColumnDates)
                                {
                                    if (!SelectedDates.Contains(d.ToDateTime(Calendar)))
                                    {
                                        SelectedDates.Add(d.ToDateTime(Calendar));
                                    }
                                }

                                ApplyBorderControlDates();
                                ApplyBorderForShiftedDate();
                            }
                        }
                    }
                    else
                    {
                        if (DayNamesAndDateGrid.SelectedColumnDates.Count > 1)
                        {
                            if (!(Keyboard.Modifiers == ModifierKeys.Control))
                            {
                                SelectedDates.Clear();
                            }

                            foreach (Date d in DayNamesAndDateGrid.SelectedColumnDates)
                            {
                                if (!SelectedDates.Contains(d.ToDateTime(Calendar)))
                                {
                                    SelectedDates.Add(d.ToDateTime(Calendar));
                                }
                            }

                            ApplyBorderForShiftedDate();
                            DayNamesAndDateGrid.SelectedDate = DayNamesAndDateGrid.EndDate;
                            DayNamesAndDateGrid.MouseHoverDate = DayNamesAndDateGrid.EndDate;
                        }
                        else
                        {
                            if (DayNamesAndDateGrid.EndDate.Day != 0)
                            {
                                SelectedDates.Clear();
                                if (!AllowMultipleSelection)
                                {
                                    ShowDayGridonSelectedCell(DayNamesAndDateGrid.EndDate);
                                }

                                if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
                                {
                                    ApplyBorderForShiftedDate();
                                    DayNamesAndDateGrid.SelectedDate = DayNamesAndDateGrid.EndDate;
                                    DayNamesAndDateGrid.MouseHoverDate = DayNamesAndDateGrid.EndDate;
                                }
                            }
                            else
                            {
                                isnull = true;
                                SelectedDates.Clear();
                                ShowDayGridonSelectedCell(DayNamesAndDateGrid.SelectedDate);
                                if (DayNamesAndDateGrid.SelectedDate.Day != 0)
                                {
                                    SelectedDate = DayNamesAndDateGrid.SelectedDate.ToDateTime(Calendar);
                                    SelectedDates.Add((DateTime)SelectedDate);
                                    // Date = SelectedDate;
                                    CurrentDate = (DateTime)SelectedDate;
                                    if (CurrentDate == SelectedDate)
                                    {
                                        ApplyBorderSelectedDate(DayNamesAndDateGrid.SelectedDate);
                                    }
                                }
                                isnull = false;
                            }
                        }
                    }
                }
                else
                {
                    ShowDayGridonSelectedCell(DayNamesAndDateGrid.SelectedDate);                    
                    isnull = true;
                    SelectedDates.Clear();
                    if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
                    {
                        SelectedDates.Clear();
                        if (DayNamesAndDateGrid.SelectedDate.Day != 0)
                        {
                            SelectedDate = DayNamesAndDateGrid.SelectedDate.ToDateTime(Calendar);
                            SelectedDates.Add((DateTime)SelectedDate);
                            //// string ss = SelectedDate.ToString("dd/MM/yyyy", Culture.DateTimeFormat);
                            ////DateTime dt = Convert.ToDateTime(ss);
                            ////SelectedDate = Culture.Calendar.ToDateTime(DayNamesAndDateGrid.SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, 0, 0, 0, 0);
                            ////SelectedDate = DateTime.Parse(Calendar.ToDateTime(DayNamesAndDateGrid.SelectedDate.Year, SelectedDate.Month, SelectedDate.Day, 0, 0, 0, 0).ToString(), Culture.DateTimeFormat);
                            ////Date = SelectedDate;
                            CurrentDate = (DateTime)SelectedDate;
                        }
                    }
                    isnull = false;
                }

                ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
            }

            if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
            {
                string month = GetType(e);
                DateTimeFormatInfo format = Culture.DateTimeFormat;
                string[] monthName = format.AbbreviatedMonthNames;
                for (int i = 0; i < monthName.Length; i++)
                {
                    if (month.ToString().Trim() == format.AbbreviatedMonthNames[i])
                    {
                        VisibleDate result = VisibleData;
                        result.VisibleMonth = i + 1;
                        result.VisibleYear = VisibleData.VisibleYear;
                        VisibleData = result;
                        foreach (MonthCell mc in MonthNameGrid.MonthCellsCollection)
                        {
                            if (mc.MonthNumber == (i + 1))
                            {
                                MonthNameGrid.FocusedCell = mc;
                                MonthNameGrid.FocusedCellContent = mc.MonthNumber;
                                Border b = (Border)mc.Parent;
                                b.Background = SelectedCellBackground;
                                b.BorderBrush = SelectedCellBorderBrush;
                                b.BorderThickness = SelectedCellBorderThickness;
                                b.CornerRadius = SelectedCellCornerRadius;
                            }
                        }

                        ApplyStoryBoardParticularCell(e, (FrameworkElement)DayNamesAndDateGrid.PopulatedDayGrid(), (FrameworkElement)MonthNameGrid.PopulatedMonthGrid);
                        ////DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
                        ////SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                        ////DateGrid.Children.Remove((UIElement)MonthNameGrid.PopulatedMonthGrid);
                        ////DateGrid.Children.Add((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
                        break;
                    }
                }
            }
            else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
            {
                RemoveWeekNumber();
                string year = GetType(e);
                VisibleDate result = VisibleData;
                result.VisibleMonth = VisibleData.VisibleMonth;
                result.VisibleYear = Convert.ToInt32(year);
                VisibleData = result;
                foreach (YearCell yc in YearGrid.YearCellsCollection)
                {
                    if (yc.Year == result.VisibleYear)
                    {
                        YearGrid.FocusedCell = yc;
                        YearGrid.FocusedCellContent = yc.Year;
                        Border b = (Border)yc.Parent;
                        b.Background = SelectedCellBackground;
                        b.BorderBrush = SelectedCellBorderBrush;
                        b.BorderThickness = SelectedCellBorderThickness;
                        b.CornerRadius = SelectedCellCornerRadius;
                    }
                }

                ApplyStoryBoardParticularCell(e, (FrameworkElement)MonthNameGrid.PopulatedMonthGrid, (FrameworkElement)YearGrid.PopulatedYearGrid);
                //// YearGrid.SetYear(VisibleData, Calendar);
                //// SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                //// DateGrid.Children.Remove((UIElement)YearGrid.PopulatedYearGrid);
                //// DateGrid.Children.Add((UIElement)MonthNameGrid.PopulatedMonthGrid);
            }
            else if (dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
            {
                RemoveWeekNumber();
                string year = GetType(e);
                string[] aBSYear = year.Split('-', '\n');
                VisibleDate result = VisibleData;
                result.VisibleMonth = VisibleData.VisibleMonth;
                result.VisibleYear = Convert.ToInt32(aBSYear[0].ToString());
                VisibleData = result;
                YearsRange yrs;
                yrs.StartYear = Convert.ToInt32(aBSYear[0].ToString());
                yrs.EndYear = Convert.ToInt32(aBSYear[2].ToString());
                foreach (YearRangeCell yc in YearRangedGrid.YearCellsCollection)
                {
                    if (yc.Years == yrs)
                    {
                        YearRangedGrid.FocusedCell = yc;
                        Border b = (Border)yc.Parent;
                        b.Background = SelectedCellBackground;
                        b.BorderBrush = SelectedCellBorderBrush;
                        b.BorderThickness = SelectedCellBorderThickness;
                        b.CornerRadius = SelectedCellCornerRadius;
                        YearRangedGrid.FocusedCellContent = yc.Years;
                    }
                }

                ApplyStoryBoardParticularCell(e, (FrameworkElement)YearGrid.PopulatedYearGrid, (FrameworkElement)YearRangedGrid.PopulatedYearRangeGrid);
                ////YearGrid.SetYear(VisibleData, Calendar);
                ////SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear-1);
                ////DateGrid.Children.Remove((UIElement)YearRangedGrid.PopulatedYearRangeGrid);
                ////DateGrid.Children.Add((UIElement)YearGrid.PopulatedYearGrid);
            }

            monthText.Opacity = 1;
            isnull = false;
        }

        /// <summary>
        /// Des the attach browser mouse wheel event.
        /// </summary>
        private void DeAttachBrowserMouseWheelEvent()
        {
            if (HtmlPage.IsEnabled)
            {
                if (HtmlPage.BrowserInformation.UserAgent.Contains("Firefox"))
                {
                    HtmlPage.Plugin.AttachEvent("DOMMouseScroll", this.ProcessOnMouseWheel);
                    HtmlPage.Plugin.DetachEvent("DOMMouseScroll", this.ProcessOnMouseWheel);
                }

                if (HtmlPage.BrowserInformation.UserAgent.Contains("MSIE") ||
                    HtmlPage.BrowserInformation.UserAgent.Contains("Opera") ||
                    HtmlPage.BrowserInformation.UserAgent.Contains("Safari"))
                {
                    HtmlPage.Plugin.DetachEvent("onmousewheel", this.ProcessOnMouseWheel);
                    HtmlPage.Plugin.DetachEvent("DOMMouseScroll", this.ProcessOnMouseWheel);
                }
            }
        }

        /// <summary>
        /// Focuses the particular date.
        /// </summary>
        void FocusParticularDate()
        {
            bool hasdate = false;
            for (int i = 0; i < DayNamesAndDateGrid.CellsCollection.Count; i++)
            {
                if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date == new Date((DateTime)SelectedDate, Calendar))
                {
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).Focus();
                    hasdate = true;
                    break;
                }
            }

            if (!hasdate)
            {
                this.Focus();
            }
        }

        /// <summary>
        /// Generates the week number.
        /// </summary>
        protected internal void GenerateWeekNumber()
        {
            if (dateGrid != null)
            {
                if (ShowWeekNumber && dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
                {
                    if (!outerGrid.Children.Contains(weekGrid))
                    {
                        outerGrid.Children.Add(weekGrid);
                    }

                    int i = 0;
                    foreach (WeekNumberCell wc in WeekNumberGrid.WeekNumberCellsCollection)
                    {
                        Grid.SetRow((FrameworkElement)wc, i);
                        Grid.SetColumn((FrameworkElement)wc, 0);
                        if (!weekGrid.Children.Contains(wc))
                        {
                            weekGrid.Children.Add(wc);
                        }

                        i++;
                    }
                    ////    weekGrid.Children.Add((Grid)WeekNumberGrid.PopulatedweekNumberGrid);
                }
                else
                {
                    outerGrid.Children.Remove(weekGrid);
                    Grid.SetColumnSpan((FrameworkElement)dateGrid, 3);
                    Grid.SetRow((FrameworkElement)dateGrid, 1);
                    Grid.SetColumn((FrameworkElement)dateGrid, 0);
                }
            }
        }

        /// <summary>
        /// Gets the paretnt calendar.
        /// </summary>
        /// <param name="parentCul">The parent cul.</param>
        /// <param name="visibleData">The visible data.</param>
        protected internal void GetParetntCalendar(ref CultureInfo parentCul, ref VisibleDate visibleData)
        {
            visibleData = VisibleData;
            parentCul = Culture;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        string GetType(MouseButtonEventArgs e)
        {
            Type tq = e.OriginalSource.GetType();
            string value = string.Empty;
            if (tq.FullName == "System.Windows.Controls.Border")
            {
                Border b = (Border)e.OriginalSource;
                ContentControl ct = (ContentControl)b.Child;
                value = ct.Content.ToString();
            }
            else if (tq.FullName == "System.Windows.Controls.TextBlock")
            {
                TextBlock ct = (TextBlock)e.OriginalSource;
                value = ct.Text.ToString();
            }

            return value;
        }

        /// <summary>
        /// Gets week number from the current date. 
        /// </summary>
        /// <param name="dt">Date to get week number from.</param>
        /// <returns>Week number</returns>
        internal int GetWeekNumber(DateTime dt)
        {
            int dayOfYear = dt.DayOfYear;
            DayOfWeek firstDayName = Culture.Calendar.GetDayOfWeek(new DateTime(dt.Year, 1, 1));
            int doy1st = (int)firstDayName;
            int shift = (doy1st <= 4) ? (4 - doy1st) : (7 + 4 - doy1st);
            int dayOfYear1thir = dayOfYear + shift;

            int result = ((dayOfYear - shift) / 7) + 1;

            return result;
        }

        /// <summary>
        /// Determines whether [is out of date range] [the specified month].
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns>
        /// 	<c>true</c> if [is out of date range] [the specified month]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsOutOfDateRange(int month)
        {
            if (VisualMode != CalendarVisualMode.Days)
            {
                throw new NotSupportedException("This function should be called in Days mode only.");
            }
            else
            {
                Date minDate = new Date(MinDate, Calendar);
                Date maxDate = new Date(MaxDate, Calendar);
                Date curDate = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                Date nextDate = new Date();
                nextDate = curDate.AddMonthToDate(month);
                ////SetMonthCellContent(Culture, nextDate.Month, nextDate.Year);
                if (nextDate > maxDate || nextDate < minDate)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Checks whether the year, years range changing is available.
        /// </summary>
        /// <param name="direction">Changing direction.</param>
        /// <param name="returnDate">Next visible date.</param>
        /// <returns>
        /// True, if changing is not available.
        /// </returns>
        /// <remarks>
        /// Is not used in the days mode.
        /// </remarks>
        private bool IsOutOfDateRange(MoveDirection direction, ref Date returnDate)
        {
            if (VisualMode == CalendarVisualMode.Days)
            {
                throw new NotSupportedException("This function should not be called in Days mode.");
            }
            else
            {
                Date minDate = new Date(MinDate, Calendar);
                Date maxDate = new Date(MaxDate, Calendar);
                Date curDate = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                return true;
            }
        }

        /// <summary>
        /// Handles the Completed event of the m_monthStoryboard control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void m_monthStoryboard_Completed(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Handles the OnHidePopup event of the m_Popup control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Tools.Controls.HidePopupEventArgs"/> instance containing the event data.</param>
        void m_Popup_OnHidePopup(object sender, HidePopupEventArgs e)
        {
            VisibleDate result;
            result.VisibleMonth = e.SelectedDate.Month;
            result.VisibleYear = e.SelectedDate.Year;
            VisibleData = result;
            SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
            DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
            MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
            YearGrid.SetYear(VisibleData, Calendar, string.Empty);
            YearRangedGrid.SetYearRange(VisibleData, Calendar);
            ApplyBorderSelectedDateOnly(new Date((DateTime)SelectedDate, Calendar));
            if (ShowWeekNumber)
            {
                WeekNumberGrid.SetWeekNumbers(DayNamesAndDateGrid.WeekNumbers);
            }

            if (AllowMultipleSelection && SelectedDates.Count > 2)
            {
                ApplyBorderForShiftedDate();
                ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
            }
            else
            {
                ApplyBorderOnlySelectDate(DayNamesAndDateGrid.SelectedDate);
                ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the MonthText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void MonthText_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
            {
                // DayNamesAndDateGrid.ApplyStoryBoard((FrameworkElement)sender, "header");
                ((FrameworkElement)sender).Opacity = 0.5;
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the MonthText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void MonthText_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
            {
                // DayNamesAndDateGrid.ApplyStoryBoard((FrameworkElement)sender, "header");
                ((FrameworkElement)sender).Opacity = 1;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the MonthText control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void MonthText_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (CalendarStyle.Standard == CalendarStyle)
            {
                int targerSize;
                if (ShowWeekNumber)
                {
                    targerSize = 35;
                }
                else
                {
                    targerSize = 0; ////    65
                }

                tempPopUp.Margin = new Thickness(-((outerGrid.ActualWidth / (outerGrid.ActualWidth / 100d)) - (targerSize - (DaysAbbreviationLength * 1))), -(outerGrid.ActualHeight / ((outerGrid.ActualHeight / 100) - 0.2d)), 0, 0);
                mPopup.ScrollButtonFill = ScrollButtonFill;
                mPopup.Show(new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1), IsMonthNameAbbreviated, Culture, new Date(MinDate, Calendar), new Date(MaxDate, Calendar));
                Border b = (Border)tempPopUp.Child;
                StackPanel sp = (StackPanel)b.Child;
                ListBox li = (ListBox)sp.Children[1];
                if (AllowMultipleSelection && SelectedDates.Count > 2)
                {
                    ApplyBorderForShiftedDate();
                    ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
                }
                else
                {
                    ApplyBorderOnlySelectDate(DayNamesAndDateGrid.SelectedDate);
                    ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
                }

                ListBoxItem lbi = (ListBoxItem)li.SelectedItem;
                lbi.Focus();
                e.Handled = true;
            }
            else if (CalendarStyle.Vista == CalendarStyle)
            {
                YearGrid.m_MouseHoveredCell = null;
                YearRangedGrid.m_MouseHoveredCell = null;
                MonthNameGrid.m_MouseHoveredCell = null;
                DayNamesAndDateGrid.m_MouseHoveredCell = null;
                RemoveWeekNumber();
                if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
                {
                    previous.Fill = ScrollButtonFill;
                    next.Fill = ScrollButtonFill;
                    StoryBoardForMonth(((FrameworkElement)DayNamesAndDateGrid.PopulatedDayGrid()), 0d, 0d, DayNamesAndDateGrid.PopulatedDayGrid(), MonthNameGrid.PopulatedMonthGrid);
                    //// DateGrid.Children.Remove((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
                    ////DateGrid.Children.Add((UIElement)MonthNameGrid.PopulatedMonthGrid);
                }
                else if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
                {
                    previous.Fill = ScrollButtonFill;
                    next.Fill = ScrollButtonFill;
                    StoryBoardForMonth(((FrameworkElement)MonthNameGrid.PopulatedMonthGrid), 0d, 0d, MonthNameGrid.PopulatedMonthGrid, YearGrid.PopulatedYearGrid);
                    ////SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear - 1);
                    ////DateGrid.Children.Remove((UIElement)MonthNameGrid.PopulatedMonthGrid);
                    ////DateGrid.Children.Add((UIElement)YearGrid.PopulatedYearGrid);
                }
                else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
                {
                    StoryBoardForMonth(((FrameworkElement)YearGrid.PopulatedYearGrid), 0d, 0d, YearGrid.PopulatedYearGrid, YearRangedGrid.PopulatedYearRangeGrid);
                    ////SetYearRangeCellContent(YearRangedGrid.SelectedYearRange);
                    ////DateGrid.Children.Remove((UIElement)YearGrid.PopulatedYearGrid);
                    ////DateGrid.Children.Add((UIElement)YearRangedGrid.PopulatedYearRangeGrid);
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the Next control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void Next_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsOutOfDateRange(1))
            {
                //// DayNamesAndDateGrid.ApplyStoryBoard((FrameworkElement)sender, "header");
                if (next.Fill == ScrollButtonFill)
                {

                    next.Fill = MouseHoverScrollButtonFill;
                    if(circle1!=null)
                    circle1.Stroke = MouseHoverScrollButtonFill;
                    
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the Next control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void Next_MouseLeave(object sender, MouseEventArgs e)
        {
            if (next.Fill == MouseHoverScrollButtonFill)
            {
                next.Fill = ScrollButtonFill;
                if (circle1 != null)
                    circle1.Stroke = new SolidColorBrush(Colors.LightGray);
            
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Next control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Next_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NextMonthDisplay();
        }

        /// <summary>
        /// Nexts the month display.
        /// </summary>
        protected internal void NextMonthDisplay()
        {
            previous.Fill = ScrollButtonFill;
            next.Fill = ScrollButtonFill;
            Date minDate = new Date(Calendar.MinSupportedDateTime, Calendar);
            Date maxDate = new Date(Calendar.MaxSupportedDateTime, Calendar);
            if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
            {
                if (!IsOutOfDateRange(1))
                {
                    AddMonth(1);
                    DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
                    SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                    MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
                    YearGrid.SetYear(VisibleData, Calendar, "Next");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);

                    if (SelectedDates.Count >= 1)
                    {
                        ApplyBorderControlDates();
                        ApplyBorderForShiftedDate();
                    }
                    else
                    {
                        for (int i = 0; i < DayNamesAndDateGrid.BorderCellsCollection.Count; i++)
                        {
                            ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = false;
                            ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = false;
                            ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);
                            Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                            if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date > curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                            {
                                ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = NextMonthDaysForeground;
                            }
                            else if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date < curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                            {
                                ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = PreviousMonthDaysForeground;
                            }
                            else
                            {
                                ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                            }
                        }

                        ApplyBorderSelectedDate(DayNamesAndDateGrid.SelectedDate);
                    }

                    ApplyStoryBoard((FrameworkElement)DayNamesAndDateGrid.PopulatedDayGrid(), "Next");
                    if (ShowWeekNumber)
                    {
                        WeekNumberGrid.SetWeekNumbers(DayNamesAndDateGrid.WeekNumbers);
                    }

                    if (maxDate.Year == VisibleData.VisibleYear && maxDate.Month == VisibleData.VisibleMonth)
                    {
                        next.Fill = buttonDisabledColor;
                    }
                    else
                    {
                        next.Fill = ScrollButtonFill;
                    }
                }
                else
                {
                    next.Fill = buttonDisabledColor;
                }

                ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
            }
            else if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
            {
                AddYear(1);

                MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
                YearGrid.SetYear(VisibleData, Calendar, "Next");
                YearRangedGrid.SetYearRange(VisibleData, Calendar);
                if (maxDate.Year == VisibleData.VisibleYear)
                {
                    next.Fill = buttonDisabledColor;
                }
                else
                {
                    next.Fill = ScrollButtonFill;
                }

                if (monthText.Text.ToString() != maxDate.Year.ToString())
                {
                    ApplyStoryBoard((FrameworkElement)MonthNameGrid.PopulatedMonthGrid, "Next");
                }

                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
            }
            else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
            {
                VisibleDate result = VisibleData;
                result.VisibleMonth = VisibleData.VisibleMonth;
                result.VisibleYear = VisibleData.VisibleYear + 10;
                if (result.VisibleYear <= maxDate.Year)
                {
                    VisibleData = result;

                    YearGrid.SetYear(VisibleData, Calendar, "Next");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);
                    if (result.VisibleYear == 1)
                    {
                        SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear);
                    }
                    else
                    {
                        SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear - 1);
                    }

                    ApplyStoryBoard((FrameworkElement)YearGrid.PopulatedYearGrid, "Next");
                }
                else
                {
                    result.VisibleYear = maxDate.Year;
                    VisibleData = result;
                    YearGrid.SetYear(VisibleData, Calendar, string.Empty);
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);
                    SetYearCellContent(Culture, YearGrid.StartYear, VisibleData.VisibleYear);
                    string data = ++YearGrid.StartYear + " - " + VisibleData.VisibleYear;

                    if (monthText.Text != data)
                    {
                        ApplyStoryBoard((FrameworkElement)YearGrid.PopulatedYearGrid, "Next");
                    }
                    else
                    {
                        next.Fill = buttonDisabledColor;
                    }
                }

                if (YearGrid.EndYear - 1 == maxDate.Year)
                {
                    next.Fill = buttonDisabledColor;
                }

                if (CheckStatus("YearGrid"))
                {
                    next.Fill = buttonDisabledColor;
                }
            }
            else if (dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
            {
                VisibleDate result = VisibleData;
                result.VisibleMonth = VisibleData.VisibleMonth;
                result.VisibleYear = YearRangedGrid.EndYearRange;
                if (result.VisibleYear <= maxDate.Year)
                {
                    VisibleData = result;
                    YearGrid.SetYear(VisibleData, Calendar, "Next");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);
                    SetYearRangeCellContent(YearRangedGrid.SelectedYearRange);
                    ApplyStoryBoard((FrameworkElement)YearRangedGrid.PopulatedYearRangeGrid, "Next");
                }
                else
                {
                    next.Fill = buttonDisabledColor;
                }

                if (CheckStatus("YearRangeGrid"))
                {
                    next.Fill = buttonDisabledColor;
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            timer = new DispatcherTimer();
            timer1 = new DispatcherTimer();
            storyBoardTimer = new DispatcherTimer();
            ToDayDateChange = new DispatcherTimer();
            if (dateGrid != null)
                dateGrid.Children.Clear();
            weekNumber = this.GetTemplateChild("WeekNumber") as StackPanel;
            weekGrid = this.GetTemplateChild("weekGrid") as Grid;
            dateGrid = this.GetTemplateChild("DateGrid") as Grid;
            outerGrid = this.GetTemplateChild("RootElement") as Grid;
            monthText = this.GetTemplateChild("MonthText") as TextBlock;
            todayDateControl = this.GetTemplateChild("TodayDate") as TextBlock;
            todayDatebtn = this.GetTemplateChild("TodayDatebtn") as TextBlock;
            previous = this.GetTemplateChild("Previous") as Polygon;
            next = this.GetTemplateChild("Next") as Polygon;
        circle = this.GetTemplateChild("circle") as Ellipse;
  circle1 = this.GetTemplateChild("circle1") as Ellipse;
            todayDatebtn.MouseLeftButtonDown += new MouseButtonEventHandler(TodayDatebtn_MouseLeftButtonDown);
            next.MouseLeftButtonUp += new MouseButtonEventHandler(Next_MouseLeftButtonUp);
            previous.MouseLeftButtonUp += new MouseButtonEventHandler(Previous_MouseLeftButtonUp);
            previous.MouseEnter += new MouseEventHandler(Previous_MouseEnter);
            previous.MouseLeave += new MouseEventHandler(Previous_MouseLeave);
            monthText.MouseLeftButtonUp += new MouseButtonEventHandler(MonthText_MouseLeftButtonUp);
            monthText.MouseEnter += new MouseEventHandler(MonthText_MouseEnter);
            monthText.MouseLeave += new MouseEventHandler(MonthText_MouseLeave);
            todayDatebtn.MouseEnter += new MouseEventHandler(TodayDateControl_MouseEnter);
            todayDatebtn.MouseLeave += new MouseEventHandler(todayDatebtn_MouseLeave);
            dateGrid.MouseLeftButtonUp += new MouseButtonEventHandler(DateGrid_MouseLeftButtonUp);
            isnull = true;
            storytimer.Interval = TimeSpan.FromMilliseconds(200);
            storytimer.Tick += new EventHandler(Storytimer_Tick);
            storytimer2.Interval = TimeSpan.FromMilliseconds(200);
            storytimer2.Tick += new EventHandler(Storytimer2_Tick);

            next.MouseEnter += new MouseEventHandler(Next_MouseEnter);
            next.MouseLeave += new MouseEventHandler(Next_MouseLeave);
            outerGrid.MouseMove += new MouseEventHandler(DateGrid_MouseEnter);
            outerGrid.MouseLeave += new MouseEventHandler(DateGrid_MouseLeave);
            timer.Tick += new EventHandler(timer_Tick);

            ToDayDateChange.Tick += new EventHandler(ToDayDateChange_Tick);
            ToDayDateChange.Interval = TimeSpan.FromMilliseconds(1);
            ToDayDateChange.Start();

            timer.Interval = TimeSpan.FromMilliseconds(AnimationTime);
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = TimeSpan.FromMilliseconds(AnimationTime);

            storyBoardTimer.Interval = TimeSpan.FromMilliseconds(AnimationTime);
            storyBoardTimer.Tick += new EventHandler(storyBoardTimer_Tick);

            //SelectedDates = new List<DateTime>();
            SelectedDates = new SelectedDates();
            SelectedDates.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedDates_CollectionChanged);
            SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
            MonthNameGrid = new MonthGrid();
            YearRangedGrid = new YearRangeGrid();
            YearGrid = new YearGrid();
            MonthNameGrid.KeyDown += new KeyEventHandler(DateGrid_KeyDown);

            WeekNumberGrid = new WeekNumbersGrid();
            DayNamesAndDateGrid.ShowBlackBorderForKeyEvent = false;
            DayNamesAndDateGrid.ParentCalendar = this;
            MonthNameGrid.ParentCalendar = this;
            YearGrid.ParentCalendar = this;
            YearRangedGrid.ParentCalendar = this;
            WeekNumberGrid.ParentCalendar = this;
            MonthNameGrid.Cell_ForeColor = DateForeground;
            YearGrid.Cell_ForeColor = DateForeground;
            YearRangedGrid.Cell_ForeColor = DateForeground;
            MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
            YearGrid.SetYear(VisibleData, Calendar, string.Empty);
            YearRangedGrid.SetYearRange(VisibleData, Calendar);
            dateGrid.Children.Add((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
            Popup monthPopup = new Popup();
            mPopup = new MonthPopup(monthPopup, new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1), Culture.DateTimeFormat, new Date(MinDate, Calendar), new Date(MaxDate, Calendar));
            tempPopUp = mPopup.PopUp;
            tempPopUp.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetRow((FrameworkElement)tempPopUp, 0);
            Grid.SetColumn((FrameworkElement)tempPopUp, 0);
            dateGrid.Children.Add(tempPopUp);
            mPopup.HidePopup += new EventHandler<HidePopupEventArgs>(m_Popup_OnHidePopup);
            if (DisplayCurrentDate)
            {
                todayDatebtn.Visibility = Visibility.Visible;
                todayDateControl.Text = TodayDate;
            }
            else
            {
                todayDatebtn.Visibility = Visibility.Collapsed;
                todayDateControl.Visibility = Visibility.Collapsed;
            }

            DayNamesAndDateGrid.TodayDate = new Date(DateTime.Now.Date, Calendar);

            if (ShowWeekNumber)
            {
                AddWeekNumberGrid();
                WeekNumberGrid.SetWeekNumbers(DayNamesAndDateGrid.WeekNumbers);
            }
            else
            {
                RemoveWeekNumber();
            }

            GenerateWeekNumber();
            if (Background != null)
            {
                outerGrid.Background = Background;
            }

            if (Date != DateTime.Now.Date)
            {
                Date d = new Date((DateTime)Date, Calendar);
                VisibleDate result;
                result.VisibleMonth = d.Month;
                result.VisibleYear = d.Year;
                VisibleData = result;
                DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
                YearGrid.SetYear(VisibleData, Calendar, string.Empty);
                YearRangedGrid.SetYearRange(VisibleData, Calendar);
                DayNamesAndDateGrid.SelectedDate = d;
                DayNamesAndDateGrid.CtrlDateCellsCollection.Add(d);
                ApplyBorderSelectedDate(DayNamesAndDateGrid.SelectedDate);
                DayNamesAndDateGrid.SelectedColumnDates.Clear();
            }
            else
            {
                DayNamesAndDateGrid.SelectedDate = new Date((DateTime)Date, Calendar);
            }

            CurrentDate = Date;
            DayNamesAndDateGrid.Cell_ForeColor = DateForeground;
            DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
            ApplyBorderSelectedDate(DayNamesAndDateGrid.SelectedDate);
            SelectedDate = DayNamesAndDateGrid.SelectedDate.ToDateTime(Calendar);
            SelectedDates.Add((DateTime)SelectedDate);
            todayDateControl.Foreground = DateForeground;
            this.Focus();
            base.OnApplyTemplate();
        }

        void SelectedDates_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (AllowMultipleSelection)
            //{
            //    ApplyBorderForShiftedDate();
            //}
      
            for (int i = 0; i < DayNamesAndDateGrid.BorderCellsCollection.Count; i++)
            {
                if (((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected)
                {
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = false;
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = false;
                    ((Cell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                    Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                    if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date > curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = NextMonthDaysForeground;
                    }
                    else if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date < curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = PreviousMonthDaysForeground;
                    }
                    else
                    {
                        ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                    }

                    ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);
                }
            }


            foreach (DateTime date in SelectedDates)
            {
                ////VisibleDate result = VisibleData;
                Date d = new Date(date, Calendar);
                ApplyBorderSelectedDate(d);
            }
            if (isnull != true)
            {
                this.SelectedDate = null;
            }
            if (isnull == true && this.SelectedDate == null)
            {
                //this.SelectedDate = (DateTime?)olddate;
            }
            
               
        }

        /// <summary>
        /// Raises the MouseWheel event.
        /// </summary>
        /// <param name="delta">Contains the amount the mouse wheel has been scrolled.</param>
        /// <returns>true when the mouse wheel event has been handled and should not be propagated to the browser.</returns>
        protected bool OnMouseWheel(double delta)
        {
            EventHandler<MouseWheelEventArgs> m = MouseWheel;
            if (m != null)
            {
                MouseWheelEventArgs e = new MouseWheelEventArgs(delta);
                return e.Handled;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Previous the month display.
        /// </summary>
        protected internal void PreviouMonthDisplay()
        {
            previous.Fill = ScrollButtonFill;
            next.Fill = ScrollButtonFill;
            if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
            {
                if (!IsOutOfDateRange(-1))
                {
                    Date minDate = new Date(MinDate, Calendar);
                    AddMonth(-1);
                    DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
                    SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                    MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
                    YearGrid.SetYear(VisibleData, Calendar, "Prev");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);

                    if (SelectedDates.Count >= 1)
                    {
                        ApplyBorderControlDates();
                        ApplyBorderForShiftedDate();
                    }
                    else
                    {
                        for (int i = 0; i < DayNamesAndDateGrid.BorderCellsCollection.Count; i++)
                        {
                            ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = false;
                            ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = false;
                            ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);
                            Date curr = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                            if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date > curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                            {
                                ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = NextMonthDaysForeground;
                            }
                            else if (((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date < curr && ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Date.Month != VisibleData.VisibleMonth)
                            {
                                ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = PreviousMonthDaysForeground;
                            }
                            else
                            {
                                ((DayCell)DayNamesAndDateGrid.CellsCollection[i]).Foreground = DateForeground;
                            }
                        }

                        ApplyBorderSelectedDate(DayNamesAndDateGrid.SelectedDate);
                    }

                    ApplyStoryBoard((FrameworkElement)DayNamesAndDateGrid.PopulatedDayGrid(), "Prev");

                    if (ShowWeekNumber)
                    {
                        WeekNumberGrid.SetWeekNumbers(DayNamesAndDateGrid.WeekNumbers);
                    }

                    if (minDate.Year == VisibleData.VisibleYear && minDate.Month == VisibleData.VisibleMonth)
                    {
                        previous.Fill = buttonDisabledColor;
                        next.Fill = ScrollButtonFill;
                    }
                    else
                    {
                        previous.Fill = ScrollButtonFill;
                    }
                }
                else
                {
                    previous.Fill = buttonDisabledColor;
                }

                ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
            }
            else if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
            {
                SubtractYear(1);
                Date minDate = new Date(MinDate, Calendar);
                MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
                YearGrid.SetYear(VisibleData, Calendar, "Prev");
                YearRangedGrid.SetYearRange(VisibleData, Calendar);
                if (monthText.Text.ToString() != minDate.Year.ToString())
                {
                    ApplyStoryBoard((FrameworkElement)MonthNameGrid.PopulatedMonthGrid, "Prev");
                }

                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                if (minDate.Year == VisibleData.VisibleYear)
                {
                    previous.Fill = buttonDisabledColor;
                }
                else
                {
                    previous.Fill = ScrollButtonFill;
                }
            }
            else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
            {
                VisibleDate result = VisibleData;
                result.VisibleMonth = VisibleData.VisibleMonth;
                result.VisibleYear = VisibleData.VisibleYear - 10;
                Date minDate = new Date(MinDate, Calendar);
                if (result.VisibleYear >= minDate.Year && YearGrid.StartYear != 1)
                {
                    VisibleData = result;
                    YearGrid.SetYear(VisibleData, Calendar, "Prev");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);
                    if (result.VisibleYear == 1)
                    {
                        SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear);
                    }
                    else
                    {
                        SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear - 1);
                    }

                    ApplyStoryBoard((FrameworkElement)YearGrid.PopulatedYearGrid, "Prev");
                }
                else if (YearGrid.StartYear != 1)
                {
                    result.VisibleYear = minDate.Year;
                    VisibleData = result;
                    YearGrid.SetYear(VisibleData, Calendar, "Prev");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);
                    previous.Fill = buttonDisabledColor;
                    string data = String.Empty;
                    ////if (YearGrid.StartYear != 1)
                    ////    Data = ++YearGrid.StartYear + " - " + YearGrid.EndYear;
                    ////else
                    data = (YearGrid.StartYear + 1) + " - " + --YearGrid.EndYear;
                    if (monthText.Text != data)
                    {
                        SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear);
                        ApplyStoryBoard((FrameworkElement)YearGrid.PopulatedYearGrid, "Prev");
                    }
                    else
                    {
                        previous.Fill = buttonDisabledColor;
                    }
                }

                if (CheckStatus("YearGrid"))
                {
                    previous.Fill = buttonDisabledColor;
                }
            }
            else if (dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
            {
                VisibleDate result = VisibleData;
                result.VisibleMonth = VisibleData.VisibleMonth;
                result.VisibleYear = YearRangedGrid.EndYearRange - 200;
                Date minDate = new Date(MinDate, Calendar);
                if (result.VisibleYear >= minDate.Year)
                {
                    VisibleData = result;
                    YearGrid.SetYear(VisibleData, Calendar, "Prev");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);
                    SetYearRangeCellContent(YearRangedGrid.SelectedYearRange);
                    ApplyStoryBoard((FrameworkElement)YearRangedGrid.PopulatedYearRangeGrid, "Prev");
                }
                else
                {
                    result.VisibleYear = minDate.Year;
                    VisibleData = result;
                    YearGrid.SetYear(VisibleData, Calendar, "Prev");
                    YearRangedGrid.SetYearRange(VisibleData, Calendar);
                    previous.Fill = new SolidColorBrush(Colors.Transparent);
                    string data = YearRangedGrid.SelectedYearRange.ToString();
                    if (monthText.Text != data)
                    {
                        SetYearRangeCellContent(YearRangedGrid.SelectedYearRange);
                        ApplyStoryBoard((FrameworkElement)YearRangedGrid.PopulatedYearRangeGrid, "Prev");
                    }
                    else
                    {
                        previous.Fill = buttonDisabledColor;
                    }
                }

                if (CheckStatus("YearRangeGrid"))
                {
                    previous.Fill = buttonDisabledColor;
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the Previous control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void Previous_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsOutOfDateRange(-1))
            {
                //// DayNamesAndDateGrid.ApplyStoryBoard((FrameworkElement)sender, "header");
                if (previous.Fill == ScrollButtonFill)
                {
                    previous.Fill = MouseHoverScrollButtonFill;
                    if (circle != null)
                        circle.Stroke = MouseHoverScrollButtonFill;
                }
            }
        }

        /// <summary>
        /// Handles the MouseLeave event of the Previous control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void Previous_MouseLeave(object sender, MouseEventArgs e)
        {
            if (previous.Fill == MouseHoverScrollButtonFill)
            {
                previous.Fill = ScrollButtonFill;
                if (circle != null)
                    circle.Stroke = new SolidColorBrush(Colors.LightGray);
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the Previous control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void Previous_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PreviouMonthDisplay();
        }

        /// <summary>
        /// ProcessOnMouseWheel processes the browser's event mouse wheel data and raises the event when the mouse is over the decorated child.
        /// When the MouseWheel event returns the Handled property of the MouseWheelEventArgs is inspected.
        /// If Handled is true the mouse wheel event will not be propagated to the browser so the browser will not scroll or zoom.
        /// </summary>
        public void ProcessOnMouseWheel(object sender, HtmlEventArgs e)
        {
            double delta = 0;

            ScriptObject eventObj = e.EventObject;

            if (eventObj.GetProperty("wheelDelta") != null)
            {
                delta = ((double)eventObj.GetProperty("wheelDelta")) / 120;
                if (HtmlPage.Window.GetProperty("opera") != null)
                {
                    delta = -delta;
                }
            }
            else if (eventObj.GetProperty("detail") != null)
            {
                delta = -((double)eventObj.GetProperty("detail")) / 3;

                if (HtmlPage.BrowserInformation.UserAgent.Contains("Macintosh"))
                {
                    delta = delta * 3;
                }
            }

            if (delta != 0)
            {
                if (delta < 0)
                {
                    PreviouMonthDisplay();
                }
                else if (delta > 0)
                {
                    NextMonthDisplay();
                }

                bool handled = OnMouseWheel(delta);
                if (handled)
                {
                    e.PreventDefault();
                }
            }
        }

        /// <summary>
        /// Removes the week number.
        /// </summary>
        private void RemoveWeekNumber()
        {
            if (outerGrid.Children.Contains(weekGrid))
            {
                outerGrid.Children.Remove(weekGrid);
                Grid.SetColumnSpan((FrameworkElement)dateGrid, 3);
                Grid.SetRow((FrameworkElement)dateGrid, 1);
                Grid.SetColumn((FrameworkElement)dateGrid, 0);
            }
        }

        /// <summary>
        /// Sets correct date that depends on MaxDate and MinDate.
        /// </summary>
        private void SetCorrectDate()
        {
            if (Date > MaxDate)
            {
                ////Date = MaxDate;
                CurrentDate = MaxDate;
            }

            if (Date < MinDate)
            {
                ////Date = MinDate;
                CurrentDate = MinDate;
            }
        }

        /// <summary>
        /// Sets the current date.
        /// </summary>
        /// <param name="date">The date.</param>
        protected internal void setCurrentDate(Date date)
        {
            if (date.Day != 0)
            {
                CurrentDate = date.ToDateTime(Calendar);
                SelectedDate = CurrentDate;
                SelectedDates.Add(CurrentDate);
            }
        }

        /// <summary>
        /// Sets the content of the month cell.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="m">The m.</param>
        /// <param name="y">The y.</param>
        protected internal void SetMonthCellContent(CultureInfo culture, int m, int y)
        {
            DateTimeFormatInfo format = culture.DateTimeFormat;

            if (!IsMonthNameAbbreviated)
            {
                monthText.Text = format.MonthNames[m - 1] + " - " + y;
            }
            else
            {
                monthText.Text = format.AbbreviatedMonthNames[m - 1] + " - " + y;
            }

            if (MonthNameGrid != null)
            {
                if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
                {
                    monthText.Text = y.ToString();
                }
            }
        }

        /// <summary>
        /// Sets tooltip to the day cell.
        /// </summary>
        /// <param name="rowIndex">Row number of the cell (starts from 0)</param>
        /// <param name="colIndex">Column number of the cell (starts from 0)</param>
        /// <param name="tooltip">Tooltip that should be set.</param>
        public void SetToolTip(int rowIndex, int colIndex, ToolTip tooltip)
        {
            List<Border> cellsCollection = DayNamesAndDateGrid.BorderCellsCollection;
            for (int i = 0, cnt = cellsCollection.Count; i < cnt; i++)
            {
                int row = Grid.GetRow((FrameworkElement)cellsCollection[i]);
                int col = Grid.GetColumn((FrameworkElement)cellsCollection[i]);

                if (row == rowIndex + 1 && col == colIndex)
                {
                    Border dc = DayNamesAndDateGrid.BorderCellsCollection[i] as Border;
                    ToolTipService.SetToolTip(dc, tooltip);
                }
            }
        }

        /// <summary>
        /// Sets tooltip to the day cell.
        /// </summary>
        /// <param name="date">Date of the cell to set tooltip.</param>
        /// <param name="tooltip">Tooltip that should be set.</param>
        public void SetToolTip(DateTime date, ToolTip tooltip)
        {
            Date result = new Date(date, Calendar);
            if (DayNamesAndDateGrid.ParentCalendar.TooltipDates.Contains(result))
            {
                int i = DayNamesAndDateGrid.ParentCalendar.TooltipDates.IndexOf(result);
                DayNamesAndDateGrid.ParentCalendar.TooltipValue.Insert(i, tooltip);
            }
            else
            {
                DayNamesAndDateGrid.ParentCalendar.TooltipDates.Add(result);
                DayNamesAndDateGrid.ParentCalendar.TooltipValue.Add(tooltip);
            }

            for (int b = 0; b < DayNamesAndDateGrid.BorderCellsCollection.Count; b++)
            {
                if (((DayCell)DayNamesAndDateGrid.CellsCollection[b]).Date == result)
                {
                    ToolTipService.SetToolTip((Border)DayNamesAndDateGrid.BorderCellsCollection[b], tooltip);
                    break;
                }
            }
            ////m_toolTipDates.Add(date);
        }

        /// <summary>
        /// Sets the content of the year cell.
        /// </summary>
        /// <param name="culture">The culture.</param>
        /// <param name="startYear">The start year.</param>
        /// <param name="endYear">The end year.</param>
        protected internal void SetYearCellContent(CultureInfo culture, int startYear, int endYear)
        {
            DateTimeFormatInfo format = culture.DateTimeFormat;
            if (!(endYear <= MaxDate.Year))
            {
                endYear = MaxDate.Year;
            }

            if (startYear != 1)
            {
                monthText.Text = ++startYear + " - " + endYear;
            }
            else
            {
                monthText.Text = startYear + " - " + endYear;
            }
        }

        /// <summary>
        /// Sets the content of the year range cell.
        /// </summary>
        /// <param name="yearRange">The year range.</param>
        protected internal void SetYearRangeCellContent(string yearRange)
        {
            monthText.Text = yearRange;
        }

        /// <summary>
        /// Shows the day gridon selected cell.
        /// </summary>
        /// <param name="date">The date.</param>
        protected internal void ShowDayGridonSelectedCell(Date date)
        {
            if (date.Day != 0)
            {
                Date prevoiusdate = new Date(VisibleData.VisibleYear, VisibleData.VisibleMonth, 1);
                if (VisibleData.VisibleMonth != date.Month)
                {
                    if (date < prevoiusdate)
                    {
                        if (ShowPreviousMonthDates)
                        {
                            PreviouMonthDisplay();
                        }
                    }
                    else
                    {
                        if (ShowNextMonthDates)
                        {
                            NextMonthDisplay();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stories the board for month.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="to1">The to1.</param>
        /// <param name="to2">The to2.</param>
        /// <param name="showGrid">The show grid.</param>
        /// <param name="hideGrid">The hide grid.</param>
        protected internal void StoryBoardForMonth(FrameworkElement element, double to1, double to2, FrameworkElement showGrid, FrameworkElement hideGrid)
        {
            if (!timer1.IsEnabled && !timer.IsEnabled)
            {
                TranslateTransform moveFollowing = new TranslateTransform();
                TranslateTransform moveCurrent = new TranslateTransform();
                DoubleAnimation followingAnimation = new DoubleAnimation();
                DoubleAnimation followingAnimation2 = new DoubleAnimation();
                Duration duration = new Duration(TimeSpan.FromSeconds(200));
                m_monthStoryboard = new Storyboard();
                m_monthStoryboard.Completed += new EventHandler(m_monthStoryboard_Completed);
                m_monthStoryboard.Duration = duration;
                followingAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime));
                followingAnimation2.Duration = new Duration(TimeSpan.FromMilliseconds(AnimationTime));
                followingAnimation.To = to1;
                followingAnimation2.To = to2;
                Storyboard.SetTarget(followingAnimation, element);
                Storyboard.SetTarget(followingAnimation2, element);
                Storyboard.SetTargetName(followingAnimation, "element");
                TransformGroup tg = new TransformGroup();
                ScaleTransform st = new ScaleTransform();
                int selectedColumn = 0, selectedRow = 0;
                double mode1ScaleX, mode1ScaleY, centerX = 0, centerY = 0;
                double param = hideGrid.ActualWidth / 24;

                Cell selectedCell = null;
                int i = 0;
                if (hideGrid == MonthNameGrid.PopulatedMonthGrid)
                {
                    for (i = 0; i < MonthNameGrid.MonthCellsCollection.Count; i++)
                    {
                        if (((Cell)MonthNameGrid.MonthCellsCollection[i]).IsSelected)
                        {
                            selectedCell = (Cell)MonthNameGrid.MonthCellsCollection[i];
                            break;
                        }
                    }

                    try
                    {
                        selectedColumn = Grid.GetColumn((FrameworkElement)((Border)MonthNameGrid.MonthBorderCollection[i]));
                        selectedRow = Grid.GetRow((FrameworkElement)((Border)MonthNameGrid.MonthBorderCollection[i]));
                    }
                    catch
                    {
                        selectedCell = (Cell)MonthNameGrid.MonthCellsCollection[1];
                        selectedColumn = Grid.GetColumn((FrameworkElement)((Border)MonthNameGrid.MonthBorderCollection[1]));
                        selectedRow = Grid.GetRow((FrameworkElement)((Border)MonthNameGrid.MonthBorderCollection[1]));
                        i = 1;
                    }
                }
                else if (hideGrid == YearGrid.PopulatedYearGrid)
                {
                    i = 0;
                    for (i = 0; i < YearGrid.YearCellsCollection.Count; i++)
                    {
                        if (((Cell)YearGrid.YearCellsCollection[i]).IsSelected)
                        {
                            selectedCell = (Cell)YearGrid.YearCellsCollection[i];
                            break;
                        }
                    }

                    try
                    {
                        selectedColumn = Grid.GetColumn((FrameworkElement)((Border)YearGrid.YearBorderCollection[i]));
                        selectedRow = Grid.GetRow((FrameworkElement)((Border)YearGrid.YearBorderCollection[i]));
                    }
                    catch
                    {
                        selectedCell = (Cell)YearGrid.YearCellsCollection[1];
                        selectedColumn = Grid.GetColumn((FrameworkElement)((Border)YearGrid.YearBorderCollection[1]));
                        selectedRow = Grid.GetRow((FrameworkElement)((Border)YearGrid.YearBorderCollection[1]));
                        i = 1;
                    }
                }
                else if (hideGrid == YearRangedGrid.PopulatedYearRangeGrid)
                {
                    i = 0;
                    for (i = 0; i < YearRangedGrid.YearCellsCollection.Count; i++)
                    {
                        if (((Cell)YearRangedGrid.YearCellsCollection[i]).IsSelected)
                        {
                            selectedCell = (Cell)YearRangedGrid.YearCellsCollection[i];
                            break;
                        }
                    }

                    try
                    {
                        selectedColumn = Grid.GetColumn((FrameworkElement)((Border)YearRangedGrid.YearBorderCollection[i]));
                        selectedRow = Grid.GetRow((FrameworkElement)((Border)YearRangedGrid.YearBorderCollection[i]));
                    }
                    catch
                    {
                        selectedCell = (Cell)YearGrid.YearCellsCollection[1];
                        selectedColumn = Grid.GetColumn((FrameworkElement)((Border)YearRangedGrid.YearBorderCollection[1]));
                        selectedRow = Grid.GetRow((FrameworkElement)((Border)YearRangedGrid.YearBorderCollection[1]));
                        i = 1;
                    }
                }

                double d = ((Border)MonthNameGrid.MonthBorderCollection[i]).ActualWidth;
                switch (selectedColumn)
                {
                    case 0:
                        centerX = 0;
                        break;

                    case 1:
                        centerX = (selectedCell.ActualWidth + (selectedCell.ActualWidth / 2)) - param;
                        break;

                    case 2:
                        centerX = (2 * (selectedCell.ActualWidth + (selectedCell.ActualWidth / 2))) + param;
                        break;

                    case 3:
                        centerX = 4 * selectedCell.ActualWidth;
                        break;
                }

                switch (selectedRow)
                {
                    case 0:
                        centerY = 0;
                        break;

                    case 1:
                        centerY = selectedCell.ActualHeight + (selectedCell.ActualHeight / 2);
                        break;

                    case 2:
                        centerY = 3 * selectedCell.ActualHeight;
                        break;
                }

                st.CenterX = centerX - 10;  ////centerY-15;
                st.CenterY = centerY - 10;  ////centerX;

                mode1ScaleX = element.ActualWidth / showGrid.ActualWidth;
                mode1ScaleY = element.ActualHeight / (showGrid.ActualHeight + showGrid.ActualHeight);
                if (to1 != 2d)
                {
                    st.CenterX = outerGrid.ActualWidth / 2;
                    st.CenterY = outerGrid.ActualHeight / 2;
                    followingAnimation.To = 0;
                    followingAnimation2.To = 0;
                    followingAnimation.From = 1;
                    followingAnimation2.From = 1;
                }
                else
                {
                    followingAnimation.To = 3; ////mode1ScaleX;
                    followingAnimation2.To = 3; //// mode1ScaleY;               
                }

                tg.Children.Add(st);
                element.RenderTransform = tg;
                Storyboard.SetTargetProperty(followingAnimation, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleX)"));
                Storyboard.SetTargetProperty(followingAnimation2, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleY)"));
                m_monthStoryboard.Children.Add(followingAnimation);
                m_monthStoryboard.Children.Add(followingAnimation2);
                if (!dateGrid.Resources.Contains("112"))
                {
                    dateGrid.Resources.Add("112", m_monthStoryboard);
                }

                m_monthStoryboard.Begin();
                if (to1 == 2d)
                {
                    timer1.Start();
                }
                else
                {
                    timer.Start();
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the storyBoardTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void storyBoardTimer_Tick(object sender, EventArgs e)
        {
            ////storyBoardTimer.Stop();
            ////StoryBoardForMonth(((FrameworkElement)YearGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)MonthNameGrid.PopulatedMonthGrid, (FrameworkElement)YearGrid.PopulatedYearGrid);           
            ////StoryBoardForMonth(((FrameworkElement)MonthNameGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)DayNamesAndDateGrid.PopulatedDayGrid(), (FrameworkElement)MonthNameGrid.PopulatedMonthGrid);
        }

        /// <summary>
        /// Subtracts the specified number of years to the visible year.
        /// </summary>
        /// <param name="year">Number of years. It can be
        /// negative or positive.</param>
        private void SubtractYear(int year)
        {
            VisibleDate result = VisibleData;
            result.VisibleYear -= year;
            Date minDate = new Date(MinDate, Calendar);
            Date maxDate = new Date(MaxDate, Calendar);
            if (result.VisibleYear >= minDate.Year)
            {
                VisibleData = result;
            }
        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void timer_Tick(object sender, EventArgs e)
        {
            previous.Fill = ScrollButtonFill;
            next.Fill = ScrollButtonFill;
            Date minDate = new Date(MinDate, Calendar);
            Date maxDate = new Date(MaxDate, Calendar);
            timer.Stop();
            if (dateGrid.Children.Contains((UIElement)DayNamesAndDateGrid.PopulatedDayGrid()))
            {
                dateGrid.Children.Remove((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
                for (int i = 0; i < MonthNameGrid.MonthCellsCollection.Count; i++)
                {
                    if (((MonthCell)MonthNameGrid.MonthCellsCollection[i]).MonthNumber != MonthNameGrid.FocusedCellContent)
                    {
                        ((MonthCell)MonthNameGrid.MonthCellsCollection[i]).Foreground = DateForeground;
                        ApplyNullBorderValue((Border)MonthNameGrid.MonthBorderCollection[i]);
                    }
                }

                dateGrid.Children.Add((UIElement)MonthNameGrid.PopulatedMonthGrid);
                ((Cell)MonthNameGrid.FocusedCell).Focus();
                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                if (minDate.Year == VisibleData.VisibleYear)
                {
                    previous.Fill = buttonDisabledColor;
                    next.Fill = ScrollButtonFill;
                }
                else if (maxDate.Year == VisibleData.VisibleYear)
                {
                    previous.Fill = ScrollButtonFill;
                    next.Fill = buttonDisabledColor;
                }
            }
            else if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
            {
                SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear - 1);
                dateGrid.Children.Remove((UIElement)MonthNameGrid.PopulatedMonthGrid);
                for (int i = 0; i < YearGrid.YearCellsCollection.Count; i++)
                {
                    if (((YearCell)YearGrid.YearCellsCollection[i]).Year != YearGrid.FocusedCellContent)
                    {
                        ((YearCell)YearGrid.YearCellsCollection[i]).Foreground = DateForeground;
                        ApplyNullBorderValue((Border)YearGrid.YearBorderCollection[i]);
                    }
                }

                dateGrid.Children.Add((UIElement)YearGrid.PopulatedYearGrid);
                ((Cell)YearGrid.FocusedCell).Focus();
                if (CheckStatus("YearGrid"))
                {
                    if (VisibleData.VisibleYear >= 1318)
                    {
                        next.Fill = MouseHoverScrollButtonFill;
                    }
                    else
                    {
                        previous.Fill = MouseHoverScrollButtonFill;
                    }
                }
            }
            else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
            {
                SetYearRangeCellContent(YearRangedGrid.SelectedYearRange);
                dateGrid.Children.Remove((UIElement)YearGrid.PopulatedYearGrid);
                dateGrid.Children.Add((UIElement)YearRangedGrid.PopulatedYearRangeGrid);
                ((Cell)YearRangedGrid.FocusedCell).Focus();
                if (CheckStatus("YearRangeGrid"))
                {
                    if (VisibleData.VisibleYear >= 1318)
                    {
                        next.Fill = MouseHoverScrollButtonFill;
                    }
                    else
                    {
                        previous.Fill = MouseHoverScrollButtonFill;
                    }
                }
            }

            m_monthStoryboard.Stop();
            monthText.Opacity = 1;
        }

        /// <summary>
        /// Handles the Tick event of the timer1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void timer1_Tick(object sender, EventArgs e)
        {
            previous.Fill = ScrollButtonFill;
            next.Fill = ScrollButtonFill;
            Date minDate = new Date(MinDate, Calendar);
            Date maxDate = new Date(MaxDate, Calendar);
            timer1.Stop();
            m_monthStoryboard.Stop();
            if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
            {
                DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                dateGrid.Children.Remove((UIElement)MonthNameGrid.PopulatedMonthGrid);
                dateGrid.Children.Add((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
                if (ShowWeekNumber)
                {
                    AddWeekNumberGrid();
                    WeekNumberGrid.SetWeekNumbers(DayNamesAndDateGrid.WeekNumbers);
                    GenerateWeekNumber();
                }

                if (SelectedDates.Count >= 1)
                {
                    ApplyBorderControlDates();
                    ApplyBorderForShiftedDate();
                }
                else
                {
                    for (int i = 0; i < DayNamesAndDateGrid.BorderCellsCollection.Count; i++)
                    {
                        ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsSelected = false;
                        ((Cell)DayNamesAndDateGrid.CellsCollection[i]).IsMouseHover = false;
                        ApplyNullBorderValue((Border)DayNamesAndDateGrid.BorderCellsCollection[i]);
                    }

                    ApplyBorderSelectedDate(DayNamesAndDateGrid.SelectedDate);
                }

                ApplyBorderTodayDate(new Date(DateTime.Now.Date, Calendar));
                FocusParticularDate();
                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                if (minDate.Year == VisibleData.VisibleYear && minDate.Month == VisibleData.VisibleMonth)
                {
                    previous.Fill = buttonDisabledColor;
                    next.Fill = ScrollButtonFill;
                }
                else if (maxDate.Year == VisibleData.VisibleYear && maxDate.Month == VisibleData.VisibleMonth)
                {
                    previous.Fill = ScrollButtonFill;
                    next.Fill = buttonDisabledColor;
                }
            }
            else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
            {
                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                dateGrid.Children.Remove((UIElement)YearGrid.PopulatedYearGrid);
                dateGrid.Children.Add((UIElement)MonthNameGrid.PopulatedMonthGrid);
                ((Cell)MonthNameGrid.FocusedCell).Focus();
                SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
                if (minDate.Year == VisibleData.VisibleYear)
                {
                    previous.Fill = buttonDisabledColor;
                    next.Fill = ScrollButtonFill;
                }
                else if (maxDate.Year == VisibleData.VisibleYear)
                {
                    previous.Fill = ScrollButtonFill;
                    next.Fill = buttonDisabledColor;
                }
            }
            else if (dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
            {
                YearGrid.SetYear(VisibleData, Calendar, string.Empty);
                SetYearCellContent(Culture, YearGrid.StartYear, YearGrid.EndYear - 1);
                dateGrid.Children.Remove((UIElement)YearRangedGrid.PopulatedYearRangeGrid);
                dateGrid.Children.Add((UIElement)YearGrid.PopulatedYearGrid);
                ((Cell)YearGrid.FocusedCell).Focus();
                if (CheckStatus("YearGrid"))
                {
                    if (VisibleData.VisibleYear >= 1318)
                    {
                        next.Fill = MouseHoverScrollButtonFill;
                    }
                    else
                    {
                        previous.Fill = MouseHoverScrollButtonFill;
                    }
                }
            }

            monthText.Opacity = 1;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the TodayDatebtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        void TodayDatebtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            
            Date date = new Date(DateTime.Now.Date, Calendar);
            VisibleDate result;
            if (!SelectedDates.Contains(date.ToDateTime(Calendar)))
            {
                SelectedDates.Add(date.ToDateTime(Calendar));
            }
            result.VisibleMonth = date.Month;
            result.VisibleYear = date.Year;
            VisibleData = result;
            SetMonthCellContent(Culture, VisibleData.VisibleMonth, VisibleData.VisibleYear);
            DayNamesAndDateGrid.Initialize(VisibleData, Culture, Calendar);
            MonthNameGrid.SetMonthNumber(VisibleData, Calendar, Culture);
            YearGrid.SetYear(VisibleData, Calendar, string.Empty);
            YearRangedGrid.SetYearRange(VisibleData, Calendar);
            ApplyBorderSelectedDateOnly(date);
            DayNamesAndDateGrid.SelectedDate = date;
            SelectedDate = date.ToDateTime(Calendar);
            SelectedDates.Add((DateTime)SelectedDate);
            if (ShowWeekNumber)
            {
                AddWeekNumberGrid();
                WeekNumberGrid.SetWeekNumbers(DayNamesAndDateGrid.WeekNumbers);
            }

            if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
            {
                dateGrid.Children.Remove((UIElement)MonthNameGrid.PopulatedMonthGrid);
                dateGrid.Children.Add((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
            }
            else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
            {
                dateGrid.Children.Remove((UIElement)YearGrid.PopulatedYearGrid);
                dateGrid.Children.Add((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
            }
            else if (dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
            {
                dateGrid.Children.Remove((UIElement)YearRangedGrid.PopulatedYearRangeGrid);
                dateGrid.Children.Add((UIElement)DayNamesAndDateGrid.PopulatedDayGrid());
            }
            DayNamesAndDateGrid.CtrlDateCellsCollection.Clear();
            SelectedDates.Clear();
            DayNamesAndDateGrid.EndDate = new Date();
            VisualStateManager.GoToState(this, "Selected", true);
        }

        /// <summary>
        /// Handles the Tick event of the ToDayDateChange control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ToDayDateChange_Tick(object sender, EventArgs e)
        {
            if (AutoDateChange)
            {
                if (Date.Day != DateTime.Now.Day)
                {
                    Date = DateTime.Now.Date;
                }
            }
        }

        /// <summary>
        /// Handles the MouseEnter event of the TodayDateControl control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void TodayDateControl_MouseEnter(object sender, MouseEventArgs e)
        {
            todayDatebtn.Opacity = 0.5;
            ////DayNamesAndDateGrid.ApplyStoryBoard((FrameworkElement)sender, "header");
        }

        /// <summary>
        /// Handles the MouseLeave event of the todayDatebtn control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        void todayDatebtn_MouseLeave(object sender, MouseEventArgs e)
        {
            todayDatebtn.Opacity = 1;
        }

        /// <summary>
        /// Updates the selected dates list.
        /// </summary>
        private void UpdateSelectedDatesList()
        {
            if (SelectedDatesList != null)
            {
                SelectedDatesList.Clear();

                ////foreach (DateTime date in SelectedDates)
                ////{
                ////    if (date < MaxDate && date > MinDate)
                ////        SelectedDatesList.Add(new Date(date, Calendar));

                ////    // TODO: else: clear not supported dates from collection
                ////}

                SelectedDatesList.Sort();
            }
        }

        #region Structure
        /// <summary>
        /// Describes previous and current calendar visual mode.
        /// </summary>
        protected struct VisualModeHistory
        {
            /// <summary>
            /// Defines old calendar visual mode. 
            /// </summary>
            public CalendarVisualMode OldMode;

            /// <summary>
            /// Defines new calendar visual mode.
            /// </summary>
            public CalendarVisualMode NewMode;

            /// <summary>
            /// Initializes new instance of the VisualModeHistory struct.
            /// </summary>
            /// <param name="oldMode">Old visual mode.</param>
            /// <param name="newMode">New visual mode.</param>
            public VisualModeHistory(CalendarVisualMode oldMode, CalendarVisualMode newMode)
            {
                OldMode = oldMode;
                NewMode = newMode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected enum MoveDirection
        {
            /// <summary>
            /// Next month changing direction. 
            /// </summary>
            Next,

            /// <summary>
            /// Previous month changing direction.
            /// </summary>
            Prev
        }
        #endregion
    }
}