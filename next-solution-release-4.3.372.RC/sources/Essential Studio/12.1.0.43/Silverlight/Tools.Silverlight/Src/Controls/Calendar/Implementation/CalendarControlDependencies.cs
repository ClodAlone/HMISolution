#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Net;
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
    using System.Collections.ObjectModel;

    #region - EnumDeclaration -
    /// <summary>
    /// Represents the Calendar Visual Mode Enum.
    /// </summary>
    public enum CalendarVisualMode
    {
        /// <summary>
        /// Days are displayed in <see cref="CalendarControl"/> control.
        /// </summary>
        Days,

        /// <summary>
        /// Months are displayed in <see cref="CalendarControl"/> control.
        /// </summary>
        Months,

        /// <summary>
        /// Years are displayed in <see cref="CalendarControl"/> control.
        /// </summary>
        Years,

        /// <summary>
        /// Ten year ranges are displayed in <see cref="CalendarControl"/> control.
        /// </summary>
        YearsRange
    }

    /// <summary>
    /// Defines direction of month change animation.
    /// </summary>
    public enum AnimationDirection
    {
        /// <summary>
        /// Horizontal direction of month change animation.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Vertical direction of month change animation.
        /// </summary>
        Vertical
    }

    /// <summary>
    /// Defines selection range when click on <see cref="Syncfusion.Windows.Tools.Controls.DayNameCell"/>
    /// with pressed Ctrl button.
    /// </summary>
    public enum SelectionRangeMode
    {
        /// <summary>
        /// The whole column is selected.
        /// </summary>
        WholeColumn,

        /// <summary>
        /// Only days belonging to the current month from the column are selected.
        /// </summary>
        CurrentMonth
    }

    /// <summary>
    /// Defines calendar style.
    /// </summary>
    public enum CalendarStyle
    {
        /// <summary>
        /// Standard calendar style.
        /// </summary>
        Standard,

        /// <summary>
        /// Vista calendar style.
        /// </summary>
        Vista
    }
    #endregion

    #region - structs -
    /// <summary>
    /// Visible date settings.
    /// </summary>
    public struct VisibleDate
    {
        #region Public members
        /// <summary>
        /// Represents visible year.
        /// </summary>
        public int VisibleYear;

        /// <summary>
        /// Represents visible month.
        /// </summary>
        public int VisibleMonth;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes a new instance of the VisibleDate struct.
        /// </summary>
        /// <param name="year">Sets visible year.</param>
        /// <param name="month">Sets visible month.</param>
        public VisibleDate(int year, int month)
        {
            this.VisibleYear = year;
            this.VisibleMonth = month;
        }
        #endregion
    }

    /// <summary>
    /// Represents year range.
    /// </summary>
    public struct YearsRange
    {
        /// <summary>
        /// Start of the year range.
        /// </summary>
        public int StartYear;

        /// <summary>
        /// End of the year range.
        /// </summary>
        public int EndYear;

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Tools.Controls.YearsRange">YearsRange</see>
        /// struct.
        /// </summary>
        /// <param name="startYear">Start of the year range.</param>
        /// <param name="endYear">End of the year range.</param>
        public YearsRange(int startYear, int endYear)
        {
            this.StartYear = startYear;
            this.EndYear = endYear;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(YearsRange a, YearsRange b)
        {
            return a.StartYear == b.StartYear && a.EndYear == b.EndYear;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(YearsRange a, YearsRange b)
        {
            return !(a == b);
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
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    public  class SelectedDates : ObservableCollection<DateTime>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        public void AddRange(List<DateTime> dateTime)
        {
            foreach (DateTime item in dateTime)
            {
                this.Add(item);
            }
        }
    }

    /// <summary>
    /// Partially calls Calendar Control
    /// </summary>
    public partial class CalendarControl
    {
        bool isnull;
        #region - Public DP -
        /// <summary>
        /// Identifies the <see cref="AllowMultipleSelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowMultiplySelectionProperty =
            DependencyProperty.Register("AllowMultipleSelection", typeof(bool), typeof(CalendarControl), new PropertyMetadata(false, new PropertyChangedCallback(OnAllowMultipleSelectionChanged)));

        /// <summary>
        /// Identifies the <see cref="AllowMultipleSelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowSelectionProperty =
            DependencyProperty.Register("AllowSelection", typeof(bool), typeof(CalendarControl), new PropertyMetadata(true, new PropertyChangedCallback(OnAllowSelectionChanged)));

        /// <summary>
        /// Identifies the <see cref="AllowYearSelection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AllowYearSelectionProperty =
            DependencyProperty.Register("AllowYearSelection", typeof(bool), typeof(CalendarControl), new PropertyMetadata(false, new PropertyChangedCallback(OnAllowYearSelectionChanged)));
        
        /// <summary>
        /// Identifies the <see cref="AnimationTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationTimeProperty =
            DependencyProperty.Register("AnimationTime", typeof(int), typeof(CalendarControl), new PropertyMetadata(300, new PropertyChangedCallback(OnAnimationTimeChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly new DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(null, new PropertyChangedCallback(OnBackgroundPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="CalendarStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarStyleProperty =
            DependencyProperty.Register("CalendarStyle", typeof(CalendarStyle), typeof(CalendarControl), new PropertyMetadata(CalendarStyle.Vista, new PropertyChangedCallback(OnCalendarStyleChanged)));

        /// <summary>
        /// Identifies the <see cref="Culture"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(CalendarControl), new PropertyMetadata(new CultureInfo(Thread.CurrentThread.CurrentCulture.Name), new PropertyChangedCallback(OnCultureChanged)));

        /// <summary>
        /// Identifies the <see cref="DateForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateForegroundProperty =
            DependencyProperty.Register("DateForeground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x68, 0x93, 0xCF)), new PropertyChangedCallback(OnCellForegroundPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="Date"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(DateTime), typeof(CalendarControl), new PropertyMetadata(DateTime.Now.Date, new PropertyChangedCallback(OnDateChanged)));
        /// <summary>
        /// Identifies the <see cref="DayForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DayForegroundProperty =
           DependencyProperty.Register("DayForeground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x24, 0x3A, 0x76)), new PropertyChangedCallback(OnDayForegroundPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="DaysAbbreviationLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DaysAbbreviationLengthProperty =
                    DependencyProperty.Register("DaysAbbreviationLength", typeof(int), typeof(CalendarControl), new PropertyMetadata(2, new PropertyChangedCallback(OnDaysAbbreviationLengthPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="FrameMovingTime"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrameMovingTimeProperty =
            DependencyProperty.Register("FrameMovingTime", typeof(int), typeof(CalendarControl), new PropertyMetadata(300, new PropertyChangedCallback(OnFrameMovingTimeChanged)));

        /// <summary>
        /// Identifies the <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));

        /// <summary>
        /// Identifies the <see cref="HeaderForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty =
           DependencyProperty.Register("HeaderForeground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x24, 0x3A, 0x76))));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(TimeSpan), typeof(CalendarControl), new PropertyMetadata(TimeSpan.FromMilliseconds(500)));

        /// <summary>
        /// Identifies the <see cref="IsDayNameAbbreviated"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDayNameAbbreviatedProperty =
            DependencyProperty.Register("IsDayNameAbbreviated", typeof(bool), typeof(CalendarControl), new PropertyMetadata(true, new PropertyChangedCallback(OnIsDayNameAbbreviatedChanged)));

        /// <summary>
        /// Identifies the <see cref="IsMonthNameAbbreviated"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsMonthNameAbbreviatedProperty =
            DependencyProperty.Register("IsMonthNameAbbreviated", typeof(bool), typeof(CalendarControl), new PropertyMetadata(false, new PropertyChangedCallback(OnIsMonthNameAbbreviatedChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowWeekNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsShowWeekNumbersProperty =
            DependencyProperty.Register("this.ShowWeekNumber", typeof(bool), typeof(CalendarControl), new PropertyMetadata(false, new PropertyChangedCallback(OnIsShowWeekNumbersChanged)));

        /// <summary>
        /// Identifies the <see cref="MonthChangeDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthChangeDirectionProperty =
            DependencyProperty.Register("MonthChangeDirection", typeof(AnimationDirection), typeof(CalendarControl), new PropertyMetadata(new PropertyChangedCallback(OnMonthChangeDirectionChanged)));

        /// <summary>
        /// Identifies the <see cref="MouseHoverCellBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseHoverCellBackgroundProperty =
            DependencyProperty.Register("MouseHoverCellBackground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Colors.Orange), new PropertyChangedCallback(OnMouseHoverCellBackgroundBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="MouseHoverCellBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseHoverCellBorderBrushProperty =
            DependencyProperty.Register("MouseHoverCellBorderBrush", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Colors.Red), new PropertyChangedCallback(OnMouseHoverBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="MouseHoverCellBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseHoverCellBorderThicknessProperty =
           DependencyProperty.Register("MouseHoverCellBorderThickness", typeof(Thickness), typeof(CalendarControl), new PropertyMetadata(new Thickness(1), new PropertyChangedCallback(OnMouseHoverCellBorderThicknessChanged)));

        /// <summary>
        /// Identifies the <see cref="MouseHoverCellCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseHoverCellCornerRadiusProperty =
           DependencyProperty.Register("MouseHoverCellCornerRadius", typeof(CornerRadius), typeof(CalendarControl), new PropertyMetadata(new CornerRadius(3), new PropertyChangedCallback(OnMouseHoverCellCornerRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="MouseHoverCellForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseHoverCellForegroundProperty =
             DependencyProperty.Register("MouseHoverCellForeground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnMouseHoverCellForegroundPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="MouseHoverScrollButtonFill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MouseHoverScrollButtonFillProperty =
            DependencyProperty.Register("MouseHoverScrollButtonFill", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x68, 0x93, 0xCF)), new PropertyChangedCallback(OnMouseHoverScrollButtonFillChanged)));

        /// <summary>
        /// Identifies the <see cref="NextMonthDaysForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NextMonthDaysForegroundProperty =
            DependencyProperty.Register("NextMonthDaysForeground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x24, 0x3A, 0x76)), new PropertyChangedCallback(OnNextMonthDaysForegroundChanged)));

        ////PreviousMonthDaysForegroundProperty

        /// <summary>
        /// Identifies the <see cref="PreviousMonthDaysForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviousMonthDaysForegroundProperty =
            DependencyProperty.Register("PreviousMonthDaysForeground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0x67, 0x00)), new PropertyChangedCallback(OnPreviousMonthDaysForegroundChanged)));

        /// <summary>
        /// Identifies the <see cref="ScrollButtonFill"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ScrollButtonFillProperty =
            DependencyProperty.Register("ScrollButtonFill", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Colors.DarkGray), new PropertyChangedCallback(OnScrollButtonFillChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedCellBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedCellBackgroundProperty =
            DependencyProperty.Register("SelectedCellBackground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0x67, 0x00)), new PropertyChangedCallback(OnSelectedCellBackgroundChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedCellBorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedCellBorderThicknessProperty =
           DependencyProperty.Register("SelectedCellBorderThickness", typeof(Thickness), typeof(CalendarControl), new PropertyMetadata(new Thickness(1), new PropertyChangedCallback(OnSelectedCellBorderThicknessChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedCellCornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedCellCornerRadiusProperty =
           DependencyProperty.Register("SelectedCellCornerRadius", typeof(CornerRadius), typeof(CalendarControl), new PropertyMetadata(new CornerRadius(3), new PropertyChangedCallback(OnSelectedCellCornerRadiusChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedCellForeground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedCellForegroundProperty =
            DependencyProperty.Register("SelectedCellForeground", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Colors.White), new PropertyChangedCallback(OnSelectedCellForegroundPropertyChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
             DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(CalendarControl), new PropertyMetadata(DateTime.Now.Date, new PropertyChangedCallback(OnSelectedDateChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectedDates"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty=
            DependencyProperty.Register("SelectedDates", typeof(SelectedDates), typeof(CalendarControl), new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedDatesChanged))); 

        /// <summary>
        /// Identifies the <see cref="SelectedCellBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionBorderBrushProperty =
            DependencyProperty.Register("SelectedCellBorderBrush", typeof(Brush), typeof(CalendarControl), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0x67, 0x00)), new PropertyChangedCallback(OnSelectionBorderBrushChanged)));

        /// <summary>
        /// Identifies the <see cref="SelectionRangeMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionRangeModeProperty =
            DependencyProperty.Register("SelectionRangeMode", typeof(SelectionRangeMode), typeof(CalendarControl), new PropertyMetadata(SelectionRangeMode.CurrentMonth, new PropertyChangedCallback(OnSelectionRangeModeChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowNextMonthDates"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowNextMonthDaysProperty =
            DependencyProperty.Register("ShowNextMonthDates", typeof(bool), typeof(CalendarControl), new PropertyMetadata(false, new PropertyChangedCallback(OnShowNextMonthDaysChanged)));

        /// <summary>
        /// Identifies the <see cref="ShowPreviousMonthDates"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowPreviousMonthDaysProperty =
            DependencyProperty.Register("ShowPreviousMonthDates", typeof(bool), typeof(CalendarControl), new PropertyMetadata(false, new PropertyChangedCallback(OnShowPreviousMonthDaysChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty TodayRowIsVisibleProperty =
            DependencyProperty.Register("DisplayCurrentDate", typeof(bool), typeof(CalendarControl), new PropertyMetadata(false, new PropertyChangedCallback(OnTodayRowIsVisibleChanged)));
        #endregion

        #region - Internal -
        /// <summary>
        /// Identifies the <see cref="Calendar"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty CalendarProperty =
            DependencyProperty.Register("Calendar", typeof(System.Globalization.Calendar), typeof(CalendarControl), new PropertyMetadata(Thread.CurrentThread.CurrentCulture.Calendar, new PropertyChangedCallback(OnCalendarChanged)));

        /// <summary>
        /// Identifies the <see cref="VisibleData"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty VisibleDataProperty =
            DependencyProperty.Register("VisibleData", typeof(VisibleDate), typeof(CalendarControl), new PropertyMetadata(new PropertyChangedCallback(OnVisibleDataChanged)));

        ////WeekNumberGridProperty
        /// <summary>
        /// Identifies the <see cref="WeekNumberGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty WeekNumberGridProperty =
             DependencyProperty.Register("this.WeekNumberGrid", typeof(WeekNumbersGrid), typeof(CalendarControl), new PropertyMetadata(null));

        ////YearGridProperty
        /// <summary>
        /// Identifies the <see cref="YearGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty YearGridProperty =
           DependencyProperty.Register("YearGrid", typeof(YearGrid), typeof(CalendarControl), new PropertyMetadata(null));

        ////YearRangeGridProperty
        /// <summary>
        /// Identifies the <see cref="YearRangedGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty YearRangeGridProperty =
            DependencyProperty.Register("YearRangedGrid", typeof(YearRangeGrid), typeof(CalendarControl), new PropertyMetadata(null));

        ////MonthGrid
        /// <summary>
        /// Identifies the <see cref="MonthNameGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty MonthGridProperty =
            DependencyProperty.Register("MonthNameGrid", typeof(MonthGrid), typeof(CalendarControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DayNamesAndDateGrid"/> dependency property.
        /// </summary>
        protected internal static readonly DependencyProperty DayNamesAndDateGridProperty =
            DependencyProperty.Register("DayNamesAndDateGrid", typeof(DayGrid), typeof(CalendarControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TodayDate"/> dependency property key.
        /// </summary>
        protected static readonly DependencyProperty TodayDatePropertyKey =
            DependencyProperty.Register("TodayDate", typeof(string), typeof(CalendarControl), new PropertyMetadata(string.Empty));

        private DispatcherTimer storytimer = new DispatcherTimer();

        private DispatcherTimer storytimer2 = new DispatcherTimer();
        #endregion

        #region - Public Properties -
        /// <summary>
        /// Gets or sets a value indicating whether multiply date selection 
        /// is allowed. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool AllowMultipleSelection
        {
            get
            {
                return (bool)GetValue(AllowMultiplySelectionProperty);
            }

            set
            {
                SetValue(AllowMultiplySelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether year editing should be enabled.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool AllowYearSelection
        {
            get
            {
                return (bool)GetValue(AllowYearSelectionProperty);
            }

            set
            {
                SetValue(AllowYearSelectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the calendar mode changing animation time. (in milliseconds)
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Default value is 300.
        /// </value>
        /// <seealso cref="int"/>
        public int AnimationTime
        {
            get
            {
                return (int)GetValue(AnimationTimeProperty);
            }

            set
            {
                SetValue(AnimationTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a brush that provides the background of the control.
        /// </summary>
        /// <value></value>
        /// <returns>The brush that provides the background of the control. The default is null.</returns>
        public new Brush Background
        {
            get
            {
                return (Brush)GetValue(BackgroundProperty);
            }

            set
            {
                SetValue(BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the calendar style.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CalendarStyle"/>
        /// Default value is CalendarStyle.Standard.
        /// </value>
        /// <seealso cref="CalendarStyle"/>
        public CalendarStyle CalendarStyle
        {
            get
            {
                return (CalendarStyle)GetValue(CalendarStyleProperty);
            }

            set
            {
                SetValue(CalendarStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the this.Culture of the control.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CultureInfo"/>
        /// </value>
        /// <seealso cref="CultureInfo"/>
        /// <example>
        /// <code>
        ///    //Create a new instance of the CalendarEdit
        ///    CalendarEdit calendarEdit = new CalendarEdit();
        ///    //Initialize the calendar in the german this.Culture 
        ///    calendarEdit.this.Culture = new System.Globalization.this.CultureInfo("de-DE");
        /// Result:
        ///    this.Calendar will be displayed in the german this.Culture.
        ///  </code>
        /// </example>
        public CultureInfo Culture
        {
            get
            {
                return (CultureInfo)GetValue(CultureProperty);
            }

            set
            {
                string s = value.Name.ToString();
                SetValue(CultureProperty, (CultureInfo)value);
            }
        }

        /// <summary>
        /// Gets or sets the date.Before set the date, change DateChange = true
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DateTime"/>
        /// Default value is DateTime.Now.this.Date.
        /// </value>
        /// <seealso cref="DateTime"/>
        public DateTime Date
        {
            get
            {
                return (DateTime)GetValue(DateProperty);
            }

            set
            {
                SetValue(DateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether bool.Used to Change the calendar by Given this.Date
        /// This is a dependency property.
        /// </summary>
        public bool AutoDateChange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the date foreground.
        /// </summary>
        /// <value>The date foreground.</value>
        public Brush DateForeground
        {
            get
            {
                return (Brush)GetValue(DateForegroundProperty);
            }

            set
            {
                SetValue(DateForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day foreground.
        /// </summary>
        /// <value>The day foreground.</value>
        public Brush DayForeground
        {
            get
            {
                return (Brush)GetValue(DayForegroundProperty);  ////    Days_ForeColor
            }

            set
            {
                SetValue(DayForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether default background. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        public int DaysAbbreviationLength
        {
            //// NumberOfCharMonthProperty
            get
            {
                return (int)GetValue(DaysAbbreviationLengthProperty);
            }

            set
            {
                SetValue(DaysAbbreviationLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether today bar is visible.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool DisplayCurrentDate
        {
            get
            {
                return (bool)GetValue(TodayRowIsVisibleProperty);
            }

            set
            {
                SetValue(TodayRowIsVisibleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the month changing animation time.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="int"/>
        /// Default value is 300.
        /// </value>
        /// <seealso cref="int"/>
        public int FrameMovingTime
        {
            get
            {
                return (int)GetValue(FrameMovingTimeProperty);
            }

            set
            {
                SetValue(FrameMovingTimeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header background of the calendar.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush HeaderBackground
        {
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }

            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the header DateForeground of the calendar.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// Default value is Brushes.Transparent.
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush HeaderForeground
        {
            get
            {
                return (Brush)GetValue(HeaderForegroundProperty);
            }

            set
            {
                SetValue(HeaderForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether day names 
        /// should be abbreviated. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsDayNameAbbreviated
        {
            get
            {
                return (bool)GetValue(IsDayNameAbbreviatedProperty);
            }

            set
            {
                SetValue(IsDayNameAbbreviatedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether month names 
        /// should be abbreviated. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsMonthNameAbbreviated
        {
            get
            {
                return (bool)GetValue(IsMonthNameAbbreviatedProperty);
            }

            set
            {
                SetValue(IsMonthNameAbbreviatedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the month change direction. 
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="AnimationDirection"/>
        /// </value>
        /// <seealso cref="AnimationDirection"/>
        public AnimationDirection MonthChangeDirection
        {
            get
            {
                return (AnimationDirection)GetValue(MonthChangeDirectionProperty);
            }

            set
            {
                SetValue(MonthChangeDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse hover cell background.
        /// </summary>
        /// <value>The mouse hover cell background.</value>
        public Brush MouseHoverCellBackground
        {
            get
            {
                return (Brush)GetValue(MouseHoverCellBackgroundProperty);
            }

            set
            {
                SetValue(MouseHoverCellBackgroundProperty, value);
            }
        }

        ////Property

        /// <summary>
        /// Gets or sets the mouse hover cell border brush.
        /// </summary>
        /// <value>The mouse hover cell border brush.</value>
        public Brush MouseHoverCellBorderBrush
        {
            get
            {
                return (Brush)GetValue(MouseHoverCellBorderBrushProperty);
            }

            set
            {
                SetValue(MouseHoverCellBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse hover cell border thickness.
        /// </summary>
        /// <value>The mouse hover cell border thickness.</value>
        public Thickness MouseHoverCellBorderThickness
        {
            get
            {
                return (Thickness)GetValue(MouseHoverCellBorderThicknessProperty);
            }

            set
            {
                SetValue(MouseHoverCellBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse hover cell corner radius.
        /// </summary>
        /// <value>The mouse hover cell corner radius.</value>
        public CornerRadius MouseHoverCellCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(MouseHoverCellCornerRadiusProperty);
            }

            set
            {
                SetValue(MouseHoverCellCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse hover cell foreground.
        /// </summary>
        /// <value>The mouse hover cell foreground.</value>
        public Brush MouseHoverCellForeground
        {
            get
            {
                return (Brush)GetValue(MouseHoverCellForegroundProperty);
            }

            set
            {
                SetValue(MouseHoverCellForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mouse hover scroll button fill.
        /// </summary>
        /// <value>The mouse hover scroll button fill.</value>
        public Brush MouseHoverScrollButtonFill
        {
            get
            {
                return (Brush)GetValue(MouseHoverScrollButtonFillProperty);
            }

            set
            {
                SetValue(MouseHoverScrollButtonFillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the next month days foreground.
        /// </summary>
        /// <value>The next month days foreground.</value>
        public Brush NextMonthDaysForeground
        {
            get
            {
                return (Brush)GetValue(NextMonthDaysForegroundProperty);
            }

            set
            {
                SetValue(NextMonthDaysForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the previous month days foreground.
        /// </summary>
        /// <value>The previous month days foreground.</value>
        public Brush PreviousMonthDaysForeground
        {
            get
            {
                return (Brush)GetValue(PreviousMonthDaysForegroundProperty);
            }

            set
            {
                SetValue(PreviousMonthDaysForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the scroll button fill.
        /// </summary>
        /// <value>The scroll button fill.</value>
        public Brush ScrollButtonFill
        {
            get
            {
                return (Brush)GetValue(ScrollButtonFillProperty);
            }

            set
            {
                SetValue(ScrollButtonFillProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets selection Cell Background of the day grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Default Background is Yellow.
        /// </value>
        /// <seealso cref="Thickness"/>
        public Brush SelectedCellBackground
        {
            //// OnSelectedCellBackgroundChanged
            get
            {
                return (Brush)GetValue(SelectedCellBackgroundProperty);
            }

            set
            {
                SetValue(SelectedCellBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets selection border brush of the day grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Brush"/>
        /// </value>
        /// <seealso cref="Brush"/>
        public Brush SelectedCellBorderBrush
        {
            get
            {
                return (Brush)GetValue(SelectionBorderBrushProperty);
            }

            set
            {
                SetValue(SelectionBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets selection border Thickness of the day grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Thickness"/>
        /// Default Thickness is 5.
        /// </value>
        /// <seealso cref="Thickness"/>
        public Thickness SelectedCellBorderThickness
        {
            get
            {
                return (Thickness)GetValue(SelectedCellBorderThicknessProperty);
            }

            set
            {
                SetValue(SelectedCellBorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets selection border corner radius of the day grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// Default radius is 5.
        /// </value>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius SelectedCellCornerRadius
        {
            get
            {
                return (CornerRadius)GetValue(SelectedCellCornerRadiusProperty);
            }

            set
            {
                SetValue(SelectedCellCornerRadiusProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected cell foreground.
        /// </summary>
        /// <value>The selected cell foreground.</value>
        public Brush SelectedCellForeground
        {
            get
            {
                return (Brush)GetValue(SelectedCellForegroundProperty);
            }

            set
            {
                SetValue(SelectedCellForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected date.
        /// </summary>
        /// <value>The selected date.</value>
        public DateTime? SelectedDate
        {
            get
            {
                return (DateTime?)GetValue(SelectedDateProperty);
            }

            private set
            {
                SetValue(SelectedDateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selected dates.
        /// </summary>
        /// <value>The selected dates.</value>
        //public List<DateTime> SelectedDates
        //{
        //    get 
        //    { 
        //        return (List<DateTime>)GetValue(SelectedDatesProperty); 
        //    }
        //    set 
        //    { 
        //        SetValue(SelectedDatesProperty, value); 
        //    }
        //}

        public SelectedDates SelectedDates
        {
            get
            {
                return (SelectedDates)GetValue(SelectedDatesProperty);
            }
            set
            {
                SetValue(SelectedDatesProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the selection range mode.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="SelectionRangeMode"/>
        /// Default value is SelectionRangeMode.CurrentMonth.
        /// </value>
        /// <seealso cref="SelectionRangeMode"/>
        public SelectionRangeMode SelectionRangeMode
        {
            get
            {
                return (SelectionRangeMode)GetValue(SelectionRangeModeProperty);
            }

            set
            {
                SetValue(SelectionRangeModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether next month days are visible.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool ShowNextMonthDates
        {
            get
            {
                return (bool)GetValue(ShowNextMonthDaysProperty);
            }

            set
            {
                SetValue(ShowNextMonthDaysProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether previous month days are visible.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool ShowPreviousMonthDates
        {
            get
            {
                return (bool)GetValue(ShowPreviousMonthDaysProperty);
            }

            set
            {
                SetValue(ShowPreviousMonthDaysProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether week numbers 
        /// should be shown. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool ShowWeekNumber
        {
            get
            {
                return (bool)GetValue(IsShowWeekNumbersProperty);
            }

            set
            {
                SetValue(IsShowWeekNumbersProperty, value);
            }
        }

        /// <summary>
        /// Gets the today date.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// Default value is string.Empty.
        /// </value>
        /// <seealso cref="string"/>//TodayDateControl
        public string TodayDate
        {
            get
            {
                return (string)GetValue(TodayDatePropertyKey);
            }

            private set
            {
                SetValue(TodayDatePropertyKey, value);
            }
        }
        #endregion
        /// <summary>
        /// Gets or sets the calendar object.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Calendar"/>
        /// </value>
        /// <seealso cref="Calendar"/>
        internal System.Globalization.Calendar Calendar
        {
            get
            {
                return (System.Globalization.Calendar)GetValue(CalendarProperty);
            }

            set
            {
                SetValue(CalendarProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the day names grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="DayGrid"/>
        /// </value>
        protected internal DayGrid DayNamesAndDateGrid
        {
            get
            {
                return (DayGrid)GetValue(DayNamesAndDateGridProperty);
            }

            set
            {
                SetValue(DayNamesAndDateGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day names grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MonthNameGrid"/>
        /// </value>
        /// <seealso cref="MonthNameGrid"/>
        protected internal MonthGrid MonthNameGrid
        {
            get
            {
                return (MonthGrid)GetValue(MonthGridProperty);
            }

            set
            {
                SetValue(MonthGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the date visible settings.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="VisibleDate"/>
        /// </value>
        /// <seealso cref="VisibleDate"/>
        protected internal VisibleDate VisibleData
        {
            get
            {
                return (VisibleDate)GetValue(VisibleDataProperty);
            }

            set
            {
                SetValue(VisibleDataProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Week Number grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="WeekNumbersGrid"/>
        /// </value>
        /// <seealso cref="WeekNumbersGrid"/>
        protected internal WeekNumbersGrid WeekNumberGrid
        {
            get
            {
                return (WeekNumbersGrid)GetValue(WeekNumberGridProperty);
            }

            set
            {
                SetValue(WeekNumberGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day names grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MonthNameGrid"/>
        /// </value>
        /// <seealso cref="MonthNameGrid"/>
        protected internal YearGrid YearGrid
        {
            get
            {
                return (YearGrid)GetValue(YearGridProperty);
            }

            set
            {
                SetValue(YearGridProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the day names grid.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="MonthNameGrid"/>
        /// </value>
        /// <seealso cref="MonthNameGrid"/>
        protected internal YearRangeGrid YearRangedGrid
        {
            get
            {
                return (YearRangeGrid)GetValue(YearRangeGridProperty);
            }

            set
            {
                SetValue(YearRangeGridProperty, value);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowMultipleSelection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnAllowMultipleSelectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (AllowMultipleSelectionChanged != null)
            {
                AllowMultipleSelectionChanged(this, e);
            }

            this.DayNamesAndDateGrid.AllowMultiplySelect = (bool)e.NewValue;
            if (!(bool)e.NewValue)
            {
                this.SelectedDates.Clear();
                if (this.DayNamesAndDateGrid.EndDate.Day != 0)
                {
                    this.DayNamesAndDateGrid.SelectedDate = new Date(this.DayNamesAndDateGrid.EndDate.ToDateTime(Calendar), this.Calendar);
                    this.DayNamesAndDateGrid.EndDate = new Date();
                }

                this.SelectedDate = this.DayNamesAndDateGrid.SelectedDate.ToDateTime(Calendar);
                SelectedDates.Add(this.DayNamesAndDateGrid.SelectedDate.ToDateTime(Calendar));
                    this.ApplyBorderOnlySelectDate(this.DayNamesAndDateGrid.SelectedDate);
                    this.TodayDate = DateTime.Now.ToString("D", this.Culture.DateTimeFormat);
                    ApplyBorderTodayDate(this.DayNamesAndDateGrid.TodayDate);
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowMultipleSelection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnAllowSelectionChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Updates property value cache and raises the <see cref="AnimationTime"/>
        /// event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnAnimationTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            ////if (AnimationTimeChanged != null)
            ////{
            ////    AnimationTimeChanged(this, e);
            ////}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnBackgroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (BackgroundPropertyChanged != null)
            {
                BackgroundPropertyChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                if (outerGrid != null)
                {
                    outerGrid.Background = newBrush;
                }
            }
            else
            {
                throw new ArgumentException("Grid Background Changed property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="Calendar"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnCalendarChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CalendarChanged != null)
            {
                CalendarChanged(this, e);
            }

            if (e.NewValue == null)
            {
                ClearValue(CalendarProperty);
            }

            ////else
            ////this.DayNamesAndDateGrid.ParentCalendar.cal

            if (this.Date == MinDate)
            {
                MinDate = this.Calendar.MinSupportedDateTime;
                this.Date = this.Calendar.MinSupportedDateTime;
            }

            if (MinDate < this.Calendar.MinSupportedDateTime)
            {
                MinDate = this.Calendar.MinSupportedDateTime;
            }

            if (MaxDate > this.Calendar.MaxSupportedDateTime)
            {
                MaxDate = this.Calendar.MaxSupportedDateTime;
            }
            
            CoerceVisibleData(e);
            MinDate = this.Calendar.MinSupportedDateTime;
            MaxDate = this.Calendar.MaxSupportedDateTime;
            SetCorrectDate();
            VisibleDate result;
            if (this.VisibleData.VisibleMonth != 0 && this.VisibleData.VisibleYear != 0)
            {
                result = this.VisibleData;
            }

            /*if (this.DayNamesAndDateGrid != null)
            {
                try
                {
                    result.VisibleMonth = ((System.Globalization.Calendar)e.NewValue).GetMonth(this.DayNamesAndDateGrid.SelectedDate.ToDateTime(((System.Globalization.Calendar)e.OldValue)));
                    result.VisibleYear = ((System.Globalization.Calendar)e.NewValue).GetYear(this.DayNamesAndDateGrid.SelectedDate.ToDateTime(((System.Globalization.Calendar)e.OldValue)));

                }
                catch
                {
                    Date minDate = new Date(this.Calendar.MinSupportedDateTime, this.Calendar);
                    Date maxDate = new Date(this.Calendar.MaxSupportedDateTime, this.Calendar);
                    if (this.DayNamesAndDateGrid.SelectedDate.Year > maxDate.Year)
                    {
                        result.VisibleMonth = maxDate.Month;
                        result.VisibleYear = maxDate.Year;
                        this.DayNamesAndDateGrid.SelectedDate = new Date(DateTime.Now.Date, this.Calendar);
                        object temp = maxDate.Day.ToString() + "/" + maxDate.Month.ToString() + "/" + maxDate.Year.ToString();
                        this.SelectedDate = Convert.ToDateTime(temp, this.Culture.DateTimeFormat);
                    }
                    else
                    {
                        result.VisibleMonth = minDate.Month;
                        result.VisibleYear = minDate.Year;
                        this.DayNamesAndDateGrid.SelectedDate = new Date(DateTime.Now.Date, this.Calendar);
                        object temp = minDate.Day.ToString() + "/" + minDate.Month.ToString() + "/" + minDate.Year.ToString();
                        this.SelectedDate = Convert.ToDateTime(temp, this.Culture.DateTimeFormat);
                    }
                }
            }

            else
            {
                result.VisibleMonth = ((System.Globalization.Calendar)e.NewValue).GetMonth(this.Date);
                result.VisibleYear = ((System.Globalization.Calendar)e.NewValue).GetYear(this.Date);
            }*/
            if (!AllowMultipleSelection)
            {
                UpdateSelectedDatesList();
            }
           //// this.VisibleData = result;
            DateTimeFormatInfo format = this.Culture.DateTimeFormat;
            if (this.DayNamesAndDateGrid != null && this.YearRangedGrid != null)
            {
                this.DayNamesAndDateGrid.SelectedDate = new Date((DateTime)this.SelectedDate, this.Calendar);
                this.DayNamesAndDateGrid.SelectedDate = new Date(this.DayNamesAndDateGrid.SelectedDate.ToDateTime(this.Calendar), this.Culture.Calendar);
                this.DayNamesAndDateGrid.TodayDate = new Date(DateTime.Now.Date, this.Calendar);
                this.DayNamesAndDateGrid.Initialize(this.VisibleData, this.Culture, this.Calendar);
                this.MonthNameGrid.SetMonthNumber(this.VisibleData, this.Calendar, this.Culture);
                YearGrid.SetYear(this.VisibleData, this.Calendar, string.Empty);
                this.YearRangedGrid.SetYearRange(this.VisibleData, this.Calendar);
            }

            if (dateGrid != null)
            {
                if (dateGrid.Children.Contains((UIElement)this.DayNamesAndDateGrid.PopulatedDayGrid()))
                {
                    SetMonthCellContent(this.Culture, this.VisibleData.VisibleMonth, this.VisibleData.VisibleYear);
                }
                else if (dateGrid.Children.Contains((UIElement)this.MonthNameGrid.PopulatedMonthGrid))
                {
                    SetMonthCellContent(this.Culture, this.VisibleData.VisibleMonth, this.VisibleData.VisibleYear);
                }
                else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
                {
                    SetYearCellContent(this.Culture, YearGrid.StartYear, YearGrid.EndYear - 1);
                }
                else
                {
                    SetYearRangeCellContent(this.YearRangedGrid.SelectedYearRange);
                }

                ////this.DayNamesAndDateGrid.EndDate = new Date();
                ////this.SelectedDates.Clear();
                
                this.TodayDate = DateTime.Now.ToString("D", this.Culture.DateTimeFormat);
                todayDateControl.Text = this.TodayDate;                
                if (AllowMultipleSelection)
                { 
                    ApplyBorderForShiftedDate();
                    ApplyBorderControlDates();
                }

                if (this.DayNamesAndDateGrid.EndDate.Day == 0 && (SelectedDates.Count == 0 || SelectedDates.Count == 1))
                {
                    ApplyBorderOnlySelectDate(this.DayNamesAndDateGrid.SelectedDate);
                    ApplyBorderTodayDate(this.DayNamesAndDateGrid.TodayDate);
                }
            }

            this.TodayDate = DateTime.Now.ToString("D", this.Culture.DateTimeFormat);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CalendarStyle"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnCalendarStyleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CalendarStyleChanged != null)
            {
                CalendarStyleChanged(this, e);
            }

            if ((CalendarStyle)e.NewValue == CalendarStyle.Standard)
            {
                if (this.MonthNameGrid != null)
                {
                    if (dateGrid.Children.Contains((UIElement)this.MonthNameGrid.PopulatedMonthGrid))
                    {
                        StoryBoardForMonth(((FrameworkElement)this.MonthNameGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)this.DayNamesAndDateGrid.PopulatedDayGrid(), (FrameworkElement)this.MonthNameGrid.PopulatedMonthGrid);
                        ////ApplyStoryBoardParticularCell(null, (FrameworkElement)this.DayNamesAndDateGrid.PopulatedDayGrid(), (FrameworkElement)this.MonthNameGrid.PopulatedMonthGrid);
                    }
                    else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
                    {
                        ////bool er = AppliedTimer;
                        StoryBoardForMonth(((FrameworkElement)YearGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)this.MonthNameGrid.PopulatedMonthGrid, (FrameworkElement)YearGrid.PopulatedYearGrid);
                        if (timer1.IsEnabled)
                        {
                            this.storytimer.Start();
                        }
                    }
                    else if (dateGrid.Children.Contains((UIElement)this.YearRangedGrid.PopulatedYearRangeGrid))
                    {
                        StoryBoardForMonth(((FrameworkElement)this.YearRangedGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)YearGrid.PopulatedYearGrid, (FrameworkElement)this.YearRangedGrid.PopulatedYearRangeGrid);
                        if (timer1.IsEnabled)
                        {
                            this.storytimer2.Start();
                        }
                        ////StoryBoardForMonth(((FrameworkElement)this.MonthNameGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)this.DayNamesAndDateGrid.PopulatedDayGrid(), (FrameworkElement)this.MonthNameGrid.PopulatedMonthGrid);
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CellForegroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCellForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DateForegroundPropertyChanged != null)
            {
                DateForegroundPropertyChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;
            DateTimeFormatInfo format = this.Culture.DateTimeFormat;
            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.Cell_ForeColor = newBrush;
                if (this.MonthNameGrid != null)
                {
                    this.MonthNameGrid.Cell_ForeColor = newBrush;
                    YearGrid.Cell_ForeColor = newBrush;
                    this.YearRangedGrid.Cell_ForeColor = newBrush;
                }

                if (this.DayNamesAndDateGrid != null && this.YearRangedGrid != null)
                {
                    this.DayNamesAndDateGrid.Initialize(this.VisibleData, this.Culture, this.Calendar);
                    this.MonthNameGrid.SetMonthNumber(this.VisibleData, this.Calendar, this.Culture);
                    YearGrid.SetYear(this.VisibleData, this.Calendar, string.Empty);
                    this.YearRangedGrid.SetYearRange(this.VisibleData, this.Calendar);
                }

                if (todayDateControl != null)
                {
                    todayDateControl.Foreground = newBrush;
                }

                this.ChangeAllBackground();
            }
            else
            {
                throw new ArgumentException("CellForeColorChanged property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Coerces the <see cref="Date"/> property.
        /// </summary>
        /// <param name="date">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        protected virtual DateTime OnCoerceDate(DateTime date)
        {
            if (date > MaxDate)
            {
                return MaxDate;
            }

            if (date < MinDate)
            {
                return MinDate;
            }

            return date;
        }

        /// <summary>
        /// Invoked whenever the <see cref="Culture"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnCultureChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CultureChanged != null)
            {
                CultureChanged(this, e);
            }

            CultureInfo newCulture = (CultureInfo)e.NewValue;

            if (newCulture.IsNeutralCulture)
            {
                this.Culture = new CultureInfo(newCulture.Name);
            }

            this.UpdateMinDate(e);
           // CoerceVisibleData(this.Calendar);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Date"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnDateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DateChanged != null)
            {
                DateChanged(this, e);
            }

            if ((DateTime)e.NewValue == (DateTime)e.OldValue)
            {
                Date d = new Date((DateTime)e.OldValue, this.Calendar);
                this.DayNamesAndDateGrid.SelectedDate = d;
                ApplyBorderOnlySelectDate(this.DayNamesAndDateGrid.SelectedDate);
            }

            if ((((DateTime)e.NewValue).Day != 0 || ((DateTime)e.NewValue).Month != 0) && ((DateTime)e.NewValue).Year != 0)
            {
                Date d = new Date((DateTime)e.NewValue, this.Calendar);
                if (monthText != null)
                {
                    VisibleDate result;
                    result.VisibleMonth = d.Month;
                    result.VisibleYear = d.Year;
                    this.VisibleData = result;
                    this.DayNamesAndDateGrid.SelectedDate = d;
                    this.DayNamesAndDateGrid.CtrlDateCellsCollection.Add(d);
                    DateTimeFormatInfo format = this.Culture.DateTimeFormat;
                    if (this.DayNamesAndDateGrid != null && this.YearRangedGrid != null)
                    {
                        // this.DayNamesAndDateGrid.this.SelectedDate = new this.Date(this.SelectedDate, this.Calendar);
                        this.DayNamesAndDateGrid.SelectedDate = new Date(this.DayNamesAndDateGrid.SelectedDate.ToDateTime(this.Calendar), this.Culture.Calendar);
                        this.DayNamesAndDateGrid.TodayDate = new Date(DateTime.Now.Date, this.Calendar);
                        this.DayNamesAndDateGrid.Initialize(this.VisibleData, this.Culture, this.Calendar);
                        this.MonthNameGrid.SetMonthNumber(this.VisibleData, this.Calendar, this.Culture);
                        YearGrid.SetYear(this.VisibleData, this.Calendar, string.Empty);
                        this.YearRangedGrid.SetYearRange(this.VisibleData, this.Calendar);
                    }

                    if (dateGrid != null)
                    {
                        if (dateGrid.Children.Contains((UIElement)this.DayNamesAndDateGrid.PopulatedDayGrid()))
                        {
                            SetMonthCellContent(this.Culture, this.VisibleData.VisibleMonth, this.VisibleData.VisibleYear);
                        }
                        else if (dateGrid.Children.Contains((UIElement)this.MonthNameGrid.PopulatedMonthGrid))
                        {
                            SetYearCellContent(this.Culture, YearGrid.StartYear, YearGrid.EndYear - 1);
                        }
                        else if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
                        {
                            SetYearRangeCellContent(this.YearRangedGrid.SelectedYearRange);
                        }
                        else
                        {
                            SetYearRangeCellContent(this.YearRangedGrid.SelectedYearRange);
                        }

                        this.DayNamesAndDateGrid.EndDate = new Date();
                        this.SelectedDates.Clear();
                        ApplyBorderOnlySelectDate(this.DayNamesAndDateGrid.SelectedDate);
                        this.TodayDate = DateTime.Now.ToString("D", this.Culture.DateTimeFormat);
                        todayDateControl.Text = this.TodayDate;
                        ApplyBorderTodayDate(this.DayNamesAndDateGrid.TodayDate);
                    }

                    this.TodayDate = DateTime.Now.ToString("D", this.Culture.DateTimeFormat);
                }
            }

            this.DayNamesAndDateGrid.SelectedColumnDates.Clear();
        }

        /// <summary>
        /// Raises the <see cref="E:DayForegroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDayForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DayForegroundChanged != null)
            {
                DayForegroundChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;
            DateTimeFormatInfo format = this.Culture.DateTimeFormat;
            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.Days_ForeColor = newBrush;
                this.DayNamesAndDateGrid.SetDayNames(format);
                if (this.ShowWeekNumber && this.WeekNumberGrid != null)
                {
                    this.WeekNumberGrid.SetWeekNumbers(this.DayNamesAndDateGrid.WeekNumbers);
                }

                this.ChangeAllBackground();
            }
            else
            {
                throw new ArgumentException("DaysForeColorChanged property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>NumberOfCharMonthChanged
        /// Invoked whenever <see cref="DaysAbbreviationLength"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Greater then 2.</exception>
        protected virtual void OnDaysAbbreviationLengthPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (DaysAbbreviationLengthChanged != null)
            {
                DaysAbbreviationLengthChanged(this, e);
            }

            int newLength = (int)e.NewValue;

            if (newLength != 0)
            {
                DateTimeFormatInfo format = this.Culture.DateTimeFormat;
                this.DayNamesAndDateGrid.NumCharMonthName = newLength;
                this.DayNamesAndDateGrid.SetDayNames(format);
            }
            else
            {
                throw new ArgumentException("DaysAbbreviationLengthProperty property can be assigned only by a int type value.");
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="FrameMovingTime"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnFrameMovingTimeChanged(DependencyPropertyChangedEventArgs e)
        {
            ////if (FrameMovingTimeChanged != null)
            ////{
            ////    FrameMovingTimeChanged(this, e);
            ////}
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsDayNameAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnIsDayNameAbbreviatedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsDayNameAbbreviatedChanged != null)
            {
                IsDayNameAbbreviatedChanged(this, e);
            }

                this.DayNamesAndDateGrid.SetDayNames(this.Culture.DateTimeFormat); 
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsMonthNameAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnIsMonthNameAbbreviatedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsMonthNameAbbreviatedChanged != null)
            {
                IsMonthNameAbbreviatedChanged(this, e);
            }

            if (monthText != null)
            {
                if (dateGrid.Children.Contains((UIElement)this.DayNamesAndDateGrid.PopulatedDayGrid()))
                {
                    SetMonthCellContent(this.Culture, this.VisibleData.VisibleMonth, this.VisibleData.VisibleYear);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="IsShowWeekNumbersChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnIsShowWeekNumbersChanged(DependencyPropertyChangedEventArgs e)
        {
            if (IsShowWeekNumbersChanged != null)
            {
                IsShowWeekNumbersChanged(this, e);
            }

            if (dateGrid != null)
            {
                if ((bool)e.NewValue && dateGrid.Children.Contains((UIElement)this.DayNamesAndDateGrid.PopulatedDayGrid()))
                {
                    if (this.ShowWeekNumber)
                    {
                        AddWeekNumberGrid();
                        this.WeekNumberGrid.SetWeekNumbers(this.DayNamesAndDateGrid.WeekNumbers);

                        GenerateWeekNumber();
                        int count = 0;
                        for (int temp = 0; temp < weekGrid.RowDefinitions.Count; temp++)
                        {
                            ////foreach (WeekNumberCell wc in this.WeekNumberGrid.WeekNumberCellsCollection)
                            ////{
                            WeekNumberCell wc = (WeekNumberCell)this.WeekNumberGrid.WeekNumberCellsCollection[temp];
                            int iweeknumberRow = Grid.GetRow(wc);
                            if (count == 0)
                            {
                                if (((DayNameCell)this.DayNamesAndDateGrid.DayNameCellsCollection[1]).ActualHeight > 0)
                                {
                                    weekGrid.RowDefinitions[temp].Height = new GridLength(((DayNameCell)this.DayNamesAndDateGrid.DayNameCellsCollection[1]).ActualHeight, GridUnitType.Pixel);
                                }

                                ////wc.Height = ((DayNameCell)this.DayNamesAndDateGrid.DayNameCellsCollection[1]).ActualHeight;
                                count = 1;
                            }
                            else
                            {
                                for (int i = 0; i < this.DayNamesAndDateGrid.BorderCellsCollection.Count; i++)
                                {
                                    int irow = Grid.GetRow((FrameworkElement)((Border)this.DayNamesAndDateGrid.BorderCellsCollection[i]));
                                    if (irow == iweeknumberRow)
                                    {
                                        if (((Border)this.DayNamesAndDateGrid.BorderCellsCollection[i]).ActualHeight > 0)
                                        {
                                            weekGrid.RowDefinitions[temp].Height = new GridLength(((Border)this.DayNamesAndDateGrid.BorderCellsCollection[i]).ActualHeight, GridUnitType.Pixel);
                                        }

                                        ////wc.Height = ((Border)this.DayNamesAndDateGrid.BorderCellsCollection[i]).ActualHeight;
                                        ////wc.VerticalContentAlignment = VerticalAlignment.Top;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (outerGrid != null)
                {
                    outerGrid.Children.Remove(weekGrid);
                    Grid.SetColumnSpan((FrameworkElement)dateGrid, 3);
                    Grid.SetRow((FrameworkElement)dateGrid, 1);
                    Grid.SetColumn((FrameworkElement)dateGrid, 0);
                }
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="MonthChangeDirection"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnMonthChangeDirectionChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MonthChangeDirectionChanged != null)
            {
                MonthChangeDirectionChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseHoverBorderBrushChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseHoverBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MouseHoverBorderBrushChanged != null)
            {
                MouseHoverBorderBrushChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.MouseHoverCellBorderBrush = newBrush;
            }
            else
            {
                throw new ArgumentException("MouseHoverCellBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseHoverCellBackgroundBrushChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseHoverCellBackgroundBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MouseHoverCellBackgroundBrushChanged != null)
            {
                MouseHoverCellBackgroundBrushChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.MouseHoverCellBackground = newBrush;
            }
            else
            {
                throw new ArgumentException("MouseHoverCellBackgroundBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseHoverCellBorderThicknessChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseHoverCellBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MouseHoverCellBorderThicknessChanged != null)
            {
                MouseHoverCellBorderThicknessChanged(this, e);
            }

            Thickness newThickness = (Thickness)e.NewValue;

            if (newThickness != null)
            {
                this.DayNamesAndDateGrid.MouseHoverCellBorderThickness = newThickness;
            }
            else
            {
                throw new ArgumentException("SelectedCellBorderThickness property can be assigned only by a Thickness or a Thickness inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseHoverCellCornerRadiusChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseHoverCellCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MouseHoverCellCornerRadiusChanged != null)
            {
                MouseHoverCellCornerRadiusChanged(this, e);
            }

            CornerRadius newCornerRadius = (CornerRadius)e.NewValue;

            if (newCornerRadius != null)
            {
                this.DayNamesAndDateGrid.MouseHoverCellCornerRadius = newCornerRadius;
            }
            else
            {
                throw new ArgumentException("MouseHoverCellCornerRadius property can be assigned only by a CornerRadius or a CornerRadius inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseHoverCellForegroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseHoverCellForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (MouseHoverForegroundPropertyChanged != null)
            {
                MouseHoverForegroundPropertyChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.MouseHoverCellForeground = newBrush;
            }
            else
            {
                throw new ArgumentException("MouseHoverForeColorChanged property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:NextMonthDaysForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnNextMonthDaysForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (NextMonthDaysForegroundChanged != null)
            {
                NextMonthDaysForegroundChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.NextMonthDays_ForeColor = newBrush;
                this.DayNamesAndDateGrid.ShowNextMonthDays(this.VisibleData.VisibleMonth);
            }
            else
            {
                throw new ArgumentException("NextMonthDaysForegroundChanged property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:PreviousMonthDaysForegroundChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPreviousMonthDaysForegroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (PreviousMonthDaysForegroundChanged != null)
            {
                PreviousMonthDaysForegroundChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.PreviousMonthDays_ForeColor = newBrush;
                this.DayNamesAndDateGrid.ShowPreviousMonthDays(this.VisibleData.VisibleMonth);
            }
            else
            {
                throw new ArgumentException("PreviousMonthDaysForegroundChanged property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ScrollButtonFillChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnScrollButtonFillChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ScrollButtonFillPropertyChanged != null)
            {
                ScrollButtonFillPropertyChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                if (previous != null)
                {
                    previous.Fill = newBrush;
                }

                if (next != null)
                {
                    next.Fill = newBrush;
                }
            }
            else
            {
                throw new ArgumentException("Scroll Button Fill Changed property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="SelectedCellBackground"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Brush or a Brush inherited type.</exception>
        protected virtual void OnSelectedCellBackgroundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedCellBackgroundChanged != null)
            {
                SelectedCellBackgroundChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.SelectedCellBackground = newBrush;
                this.ChangeAllBackground();
            }
            else
            {
                throw new ArgumentException("SelectedCellBackground property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>OnSelectedCellCornerRadiusChanged
        /// Invoked whenever <see cref="SelectedCellCornerRadius"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a CornerRadius or a CornerRadius inherited type.</exception>
        protected virtual void OnSelectedCellCornerRadiusChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedCellCornerRadiusChanged != null)
            {
                SelectedCellCornerRadiusChanged(this, e);
            }

            CornerRadius newCornerRadius = (CornerRadius)e.NewValue;
            if (newCornerRadius != null)
            {
                this.DayNamesAndDateGrid.SelectedCellCornerRadius = newCornerRadius;
                this.ChangeAllBackground();
            }
            else
            {
                throw new ArgumentException("SelectedCellCornerRadius property can be assigned only by a CornerRadius or a CornerRadius inherited type value.");
            }
        }

        /// <summary>OnSelectedCellBorderThicknessChanged
        /// Invoked whenever <see cref="SelectedCellBorderThickness"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a CornerRadius or a CornerRadius inherited type.</exception>
        protected virtual void OnSelectedCellBorderThicknessChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedCellBorderThicknessChanged != null)
            {
                SelectedCellBorderThicknessChanged(this, e);
            }

            Thickness newThickness = (Thickness)e.NewValue;

            if (newThickness != null)
            {
                this.DayNamesAndDateGrid.SelectedCellBorderThickness = newThickness;
                this.ChangeAllBackground();
            }
            else
            {
                throw new ArgumentException("MouseHoverCellBorderThickness property can be assigned only by a Thickness or a Thickness inherited type value.");
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SelectedCellForegroundPropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedCellForegroundPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedCellForegroundPropertyChanged != null)
            {
                SelectedCellForegroundPropertyChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;
            DateTimeFormatInfo format = this.Culture.DateTimeFormat;
            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.SelectedCell_ForeColor = newBrush;
                if (this.DayNamesAndDateGrid != null && this.YearRangedGrid != null)
                {
                    this.DayNamesAndDateGrid.Initialize(this.VisibleData, this.Culture, this.Calendar);
                    this.MonthNameGrid.SetMonthNumber(this.VisibleData, this.Calendar, this.Culture);
                    YearGrid.SetYear(this.VisibleData, this.Calendar, string.Empty);
                    this.YearRangedGrid.SetYearRange(this.VisibleData, this.Calendar);
                }

                this.ChangeAllBackground();
            }
            else
            {
                throw new ArgumentException("DaysForeColorChanged property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }
        internal DateTime olddate;
        /// <summary>
        /// Raises the <see cref="E:SelectedDateChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedDateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                olddate = (DateTime)e.OldValue;
            }
            if (e.NewValue != null)
            {
                if (((DateTime)e.NewValue).Day != 0)
                {
                    Date d = new Date((DateTime)e.NewValue, this.Calendar);
                    this.DayNamesAndDateGrid.SelectedDate = d;
                    if (!this.DayNamesAndDateGrid.CtrlDateCellsCollection.Contains(d))
                    {
                        this.DayNamesAndDateGrid.CtrlDateCellsCollection.Add(d);
                    }

                    ApplyBorderSelectedDate(this.DayNamesAndDateGrid.SelectedDate);
                }
                if (e.OldValue == null && e.NewValue != null)
                {
                    this.SelectedDate = (DateTime)e.NewValue;
                }
                if (e.NewValue != null && e.OldValue != null)
                {
                    if ((DateTime)e.NewValue == (DateTime)e.OldValue)
                    {
                        Date d = new Date((DateTime)e.OldValue, this.Calendar);
                        this.DayNamesAndDateGrid.SelectedDate = d;
                        ApplyBorderSelectedDate(this.DayNamesAndDateGrid.SelectedDate);
                    }
                }
                    this.DayNamesAndDateGrid.SelectedColumnDates.Clear();
                    if (SelectedDateChanged != null)
                    {
                        SelectedDateChanged(this, e);
                    }
                
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="SelectedCellBorderBrush"/> property is changed.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedDatesChanged(DependencyPropertyChangedEventArgs e)            
        {
            if(AllowMultipleSelection)
            {
                ApplyBorderForShiftedDate();
            }

            if (SelectedDatesChanged != null)
            {
                SelectedDatesChanged(this, e);
            }
        }

        /// <summary>
        /// Invoked whenever <see cref="SelectedCellBorderBrush"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        /// <exception cref="ArgumentException">New value must be of a Brush or a Brush inherited type.</exception>
        protected virtual void OnSelectionBorderBrushChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectionBorderBrushChanged != null)
            {
                SelectionBorderBrushChanged(this, e);
            }

            Brush newBrush = e.NewValue as Brush;

            if (newBrush != null)
            {
                this.DayNamesAndDateGrid.SelectedCellBorderBrush = newBrush;
                this.ChangeAllBackground();
            }
            else
            {
                throw new ArgumentException("SelectionBorderBrush property can be assigned only by a Brush or a Brush inherited type value.");
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectionRangeMode"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnSelectionRangeModeChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectionRangeModeChanged != null)
            {
                SelectionRangeModeChanged(this, e);
            }

            if (this.DayNamesAndDateGrid != null)
            {
                this.DayNamesAndDateGrid.SelectionRangeMode = (SelectionRangeMode)e.NewValue;
            }
        }

        /// <summary>
        /// Raises the <see cref="ShowNextMonthDaysChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnShowNextMonthDaysChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowNextMonthDaysChanged != null)
            {
                ShowNextMonthDaysChanged(this, e);
            }

            if ((bool)e.NewValue)
            {
                this.DayNamesAndDateGrid.ShowNextMonthDaysProperty = (bool)e.NewValue;
                this.DayNamesAndDateGrid.ShowNextMonthDays(this.VisibleData.VisibleMonth);
            }
            else
            {
                this.DayNamesAndDateGrid.ShowNextMonthDaysProperty = (bool)e.NewValue;
                this.DayNamesAndDateGrid.ShowNextMonthDays(this.VisibleData.VisibleMonth);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:ShowPreviousMonthDaysChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnShowPreviousMonthDaysChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ShowPreviousMonthDaysChanged != null)
            {
                ShowPreviousMonthDaysChanged(this, e);
            }

            this.DayNamesAndDateGrid.ShowPreviousMonthDaysProperty = (bool)e.NewValue;
            this.DayNamesAndDateGrid.ShowPreviousMonthDays(this.VisibleData.VisibleMonth);
        }

        /// <summary>
        /// Raises the <see cref="TodayRowIsVisibleChanged"/> event.
        /// </summary>
        /// <param name="e">Property change details, such as old value and new value.</param>
        protected virtual void OnTodayRowIsVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            if (TodayRowIsVisibleChanged != null)
            {
                TodayRowIsVisibleChanged(this, e);
            }

            if ((bool)e.NewValue)
            {
                if (todayDateControl != null)
                {
                    todayDateControl.Text = this.TodayDate;
                    todayDateControl.Visibility = Visibility.Visible;
                    todayDatebtn.Visibility = Visibility.Visible;
                }
            }
            else
            {
                todayDateControl.Visibility = Visibility.Collapsed;
                todayDatebtn.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="VisibleData"/> property is changed.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> containing 
        /// the event data.</param>
        protected virtual void OnVisibleDataChanged(DependencyPropertyChangedEventArgs e)
        {
            if (VisibleDataChanged != null)
            {
                VisibleDataChanged(this, e);
            }

            DateTimeFormatInfo format = this.Culture.DateTimeFormat;
            if (VisualMode == CalendarVisualMode.Days)
            {
                if (this.DayNamesAndDateGrid != null)
                {
                    this.DayNamesAndDateGrid.SetDayNames(format);
                }
            }

            if (this.DayNamesAndDateGrid != null && e.NewValue != null)
            {
                this.DayNamesAndDateGrid.VisibleData = (VisibleDate)e.NewValue;
            }
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowMultipleSelection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnAllowMultipleSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnAllowMultipleSelectionChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnAllowSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnAllowSelectionChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="AllowYearSelection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnAllowYearSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            ////instance.OnAllowYearSelectionChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="AnimationTime"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnAnimationTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnAnimationTimeChanged(e);
        }

        /// <summary>
        /// Called when [background property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnBackgroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnBackgroundPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Calendar"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCalendarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnCalendarChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="CalendarStyle"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCalendarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnCalendarStyleChanged(e);
        }

        /// <summary>
        /// Coerces the value of <see cref="Date"/> property.
        /// </summary>
        /// <param name="d">Object to which the property belongs.</param>
        /// <param name="value">Value that should be checked.</param>
        /// <returns>
        /// Checked value.
        /// </returns>
        private static object OnCoerceDate(DependencyObject d, object value)
        {
            CalendarControl instance = (CalendarControl)d;
            DateTime date = (DateTime)value;
            DateTime newDate = instance.OnCoerceDate(date);

            if (date != newDate)
            {
                return newDate;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Called when [cell foreground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCellForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnCellForegroundPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="Culture"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnCultureChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnCultureChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="DaysAbbreviationLength"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDaysAbbreviationLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnDaysAbbreviationLengthPropertyChanged(e);
        }
       
        /// <summary>
        /// Invoked whenever the <see cref="Date"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnDateChanged(e);
        }

        /// <summary>
        /// Called when [day foreground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDayForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnDayForegroundPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="FrameMovingTime"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnFrameMovingTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnFrameMovingTimeChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsDayNameAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsDayNameAbbreviatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnIsDayNameAbbreviatedChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="IsMonthNameAbbreviated"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsMonthNameAbbreviatedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnIsMonthNameAbbreviatedChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="ShowWeekNumber"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnIsShowWeekNumbersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnIsShowWeekNumbersChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="MonthChangeDirection"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnMonthChangeDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnMonthChangeDirectionChanged(e);
        }

        /// <summary>
        /// Called when [mouse hover border brush changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMouseHoverBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnMouseHoverBorderBrushChanged(e);
        }

        /// <summary>
        /// Called when [mouse hover cell background brush changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMouseHoverCellBackgroundBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnMouseHoverCellBackgroundBrushChanged(e);
        }

        /// <summary>
        /// Called when [mouse hover cell border thickness changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMouseHoverCellBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnMouseHoverCellBorderThicknessChanged(e);
        }

        /// <summary>
        /// Called when [mouse hover cell corner radius changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMouseHoverCellCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnMouseHoverCellCornerRadiusChanged(e);
        }

        /// <summary>
        /// Called when [mouse hover cell foreground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMouseHoverCellForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnMouseHoverCellForegroundPropertyChanged(e);
        }

        /// <summary>
        /// Called when [mouse hover scroll button fill changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnMouseHoverScrollButtonFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when [next month days foreground changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnNextMonthDaysForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnNextMonthDaysForegroundChanged(e);
        }

        /// <summary>
        /// Called when [previous month days foreground changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnPreviousMonthDaysForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnPreviousMonthDaysForegroundChanged(e);
        }

        /// <summary>
        /// Called when [scroll button fill changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnScrollButtonFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnScrollButtonFillChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectedCellBackground"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectedCellBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectedCellBackgroundChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectedCellBorderThickness"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectedCellBorderThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectedCellBorderThicknessChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectedCellCornerRadius"/> property is
        /// changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectedCellCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectedCellCornerRadiusChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="ShowPreviousMonthDates"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowPreviousMonthDaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnShowPreviousMonthDaysChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="ShowNextMonthDates"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnShowNextMonthDaysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnShowNextMonthDaysChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnTodayRowIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnTodayRowIsVisibleChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectedCellBorderBrush"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectionBorderBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectionBorderBrushChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectionRangeMode"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectionRangeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectionRangeModeChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="VisibleData"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnVisibleDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnVisibleDataChanged(e);
        }

        /// <summary>
        /// Called when [selected cell foreground property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSelectedCellForegroundPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectedCellForegroundPropertyChanged(e);
        }

        /// <summary>
        /// Invoked whenever the <see cref="SelectedDate"/> property is changed.
        /// </summary>
        /// <param name="d">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> that contains the event data.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectedDateChanged(e);
        }

        private static void OnSelectedDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CalendarControl instance = (CalendarControl)d;
            instance.OnSelectedDatesChanged(e);
        }

        /// <summary>
        /// Changes all background.
        /// </summary>
        private void ChangeAllBackground()
        {

            if (this.DayNamesAndDateGrid != null)
            {
                    for (int i = 0; i < this.DayNamesAndDateGrid.CellsCollection.Count; i++)
                    {
                        if (((Cell)this.DayNamesAndDateGrid.CellsCollection[i]).IsSelected == true)
                        {
                            Border b = (Border)this.DayNamesAndDateGrid.BorderCellsCollection[i];
                            b.CornerRadius = this.SelectedCellCornerRadius;
                            b.BorderBrush = this.SelectedCellBorderBrush;
                            b.BorderThickness = this.SelectedCellBorderThickness;
                            b.Background = this.SelectedCellBackground;
                            ((DayCell)this.DayNamesAndDateGrid.CellsCollection[i]).Foreground = this.SelectedCellForeground;
                            b.UpdateLayout();
                        }
                    }
            }

            if (this.MonthNameGrid != null)
            {
                //if (dateGrid.Children.Contains((UIElement)MonthNameGrid.PopulatedMonthGrid))
                //{
                    for (int i = 0; i < this.MonthNameGrid.MonthCellsCollection.Count; i++)
                    {
                        if (((Cell)this.MonthNameGrid.MonthCellsCollection[i]).IsSelected == true)
                        {
                            Border b = (Border)this.MonthNameGrid.MonthBorderCollection[i];
                            if (b == MonthNameGrid.SelectedMonthBorder)
                            {
                                b.CornerRadius = this.SelectedCellCornerRadius;
                                b.BorderBrush = this.SelectedCellBorderBrush;
                                b.BorderThickness = this.SelectedCellBorderThickness;
                                b.Background = this.SelectedCellBackground;
                                ((MonthCell)this.MonthNameGrid.MonthCellsCollection[i]).Foreground = this.SelectedCellForeground;
                                b.UpdateLayout();
                            }
                        }
                    }
               // }
            }

                if (this.YearGrid != null)
                {
                    //if (dateGrid.Children.Contains((UIElement)YearGrid.PopulatedYearGrid))
                    //{
                        for (int i = 0; i < YearGrid.YearCellsCollection.Count; i++)
                        {
                            if (((Cell)YearGrid.YearCellsCollection[i]).IsSelected == true)
                            {
                                Border b = (Border)YearGrid.YearBorderCollection[i];
                                if (b == YearGrid.SelectedMonthBorder && YearGrid.FocusedCellContent == ((YearCell)YearGrid.YearCellsCollection[i]).Year)
                                {
                                    b.CornerRadius = this.SelectedCellCornerRadius;
                                    b.BorderBrush = this.SelectedCellBorderBrush;
                                    b.BorderThickness = this.SelectedCellBorderThickness;
                                    b.Background = this.SelectedCellBackground;
                                    ((YearCell)YearGrid.YearCellsCollection[i]).Foreground = this.SelectedCellForeground;
                                    b.UpdateLayout();
                                }
                            }
                        //}
                    }
                }

                if (this.YearRangedGrid != null)
                {
                    //if (dateGrid.Children.Contains((UIElement)YearRangedGrid.PopulatedYearRangeGrid))
                    //{
                        for (int i = 0; i < this.YearRangedGrid.YearCellsCollection.Count; i++)
                        {
                            if (((Cell)this.YearRangedGrid.YearCellsCollection[i]).IsSelected == true)
                            {
                                Border b = (Border)this.YearRangedGrid.YearBorderCollection[i];
                                if (b == YearRangedGrid.SelectedMonthBorder && YearRangedGrid.FocusedCellContent == ((YearRangeCell)YearRangedGrid.YearCellsCollection[i]).Years)
                                {
                                    b.CornerRadius = this.SelectedCellCornerRadius;
                                    b.BorderBrush = this.SelectedCellBorderBrush;
                                    b.BorderThickness = this.SelectedCellBorderThickness;
                                    b.Background = this.SelectedCellBackground;
                                    ((YearRangeCell)this.YearRangedGrid.YearCellsCollection[i]).Foreground = this.SelectedCellForeground;
                                    b.UpdateLayout();
                                }
                            }
                        }
                    //}
                }            
        }

        /// <summary>
        /// Initializes popup window date values.
        /// </summary>
        private void InitializePopup()
        {
            if (mPopup != null)
            {
                mPopup.Format = this.Culture.DateTimeFormat;
                mPopup.CurrentDate = new Date(this.VisibleData.VisibleYear, this.VisibleData.VisibleMonth, 1);
                mPopup.MinDate = new Date(MinDate, this.Calendar);
                mPopup.MaxDate = new Date(MaxDate, this.Calendar);
            }
        }

        /// <summary>
        /// Handles the Tick event of the Storytimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Storytimer_Tick(object sender, EventArgs e)
        {
            if (!timer1.IsEnabled)
            {
                this.storytimer.Stop();
                StoryBoardForMonth(((FrameworkElement)this.MonthNameGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)this.DayNamesAndDateGrid.PopulatedDayGrid(), (FrameworkElement)this.MonthNameGrid.PopulatedMonthGrid);
            }
        }

        /// <summary>
        /// Handles the Tick event of the Storytimer2 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Storytimer2_Tick(object sender, EventArgs e)
        {
            if (!timer1.IsEnabled)
            {
                this.storytimer2.Stop();
                StoryBoardForMonth(((FrameworkElement)YearGrid.SelectedMonthBorder), 2d, 2d, (FrameworkElement)this.MonthNameGrid.PopulatedMonthGrid, (FrameworkElement)YearGrid.PopulatedYearGrid);
                this.storytimer.Start();
            }
        }

        /// <summary>
        /// Updates the min date.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void UpdateMinDate(DependencyPropertyChangedEventArgs e)
        {
            CultureInfo newCulture = (CultureInfo)e.NewValue;
            CultureInfo oldCulture = (CultureInfo)e.OldValue;

            if (newCulture != oldCulture)
            {
                this.Calendar = newCulture.Calendar;

                if (MinDate == oldCulture.Calendar.MinSupportedDateTime)
                {
                    MinDate = newCulture.Calendar.MinSupportedDateTime;
                }
            }
        }

        /// <summary>
        /// Corrects this.VisibleData property to min this.Date if this.Date is min.
        /// </summary>
        private void VisibleDataToMinSupportedDate(DateTimeFormatInfo format)
        {
            DateTime minSupported = format.Calendar.MinSupportedDateTime;
            if (this.Date == minSupported)
            {
                this.VisibleData = new VisibleDate(minSupported.Year, minSupported.Month);
            }
        }
    }
}