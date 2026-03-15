//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using Syncfusion.Windows.Controls.Primitives;
using Syncfusion.Windows.Automation.Peers;
using System.Threading;
using Syncfusion.Licensing;
using System.ComponentModel;

namespace Syncfusion.Windows.Controls
{
    /// <summary>
    /// Represents a control that enables a user to select a date by using a visual calendar display. 
    /// </summary>
    [TemplatePart(Name = Calendar.ElementRoot, Type = typeof(Panel))]
    [TemplatePart(Name = Calendar.ElementMonth, Type = typeof(Syncfusion.Windows.Controls.Primitives.CalendarItem))]
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class Calendar : Control
    {
        #region Constants

        private const string ElementRoot = "PART_Root";
        private const string ElementMonth = "PART_CalendarItem";

        private const int COLS = 7;
        private const int ROWS = 7;
        private const int YEAR_ROWS = 3;
        private const int YEAR_COLS = 4;
        private const int YEARS_PER_DECADE = 10;
        internal bool AllowClose = false;
        #endregion Constants

        #region Data
        private DateTime? _hoverStart;
        private DateTime? _hoverEnd;
        private bool _isShiftPressed;
        private DateTime? _currentDate;
        internal Syncfusion.Windows.Controls.Primitives.CalendarItem _monthControl;
        private CalendarBlackoutDatesCollection _blackoutDates;
        private SelectedDatesCollection _selectedDates;

        #endregion Data

        #region Public Events
        /// <summary>
        /// 
        /// </summary>
        public static readonly RoutedEvent SelectedDatesChangedEvent = EventManager.RegisterRoutedEvent("SelectedDatesChanged", RoutingStrategy.Direct, typeof(EventHandler<SelectionChangedEventArgs>), typeof(Calendar));

        /// <summary>
        /// Occurs when a date is selected.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectedDatesChanged
        {
            add { AddHandler(SelectedDatesChangedEvent, value); }
            remove { RemoveHandler(SelectedDatesChangedEvent, value); }
        }

        /// <summary>
        /// Occurs when the DisplayDate property is changed.
        /// </summary>
        public event EventHandler<CalendarDateChangedEventArgs> DisplayDateChanged;

        /// <summary>
        /// Occurs when the DisplayMode property is changed. 
        /// </summary>
        public event EventHandler<CalendarModeChangedEventArgs> DisplayModeChanged;

        /// <summary>
        /// Occurs when the SelectionMode property is changed. 
        /// </summary>
        public event EventHandler<EventArgs> SelectionModeChanged;
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedCallback DisableDateSelectionChanged;

        internal event PropertyChangedCallback NewDateChanged;
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler MonthChanged;
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler PreviousCalendarPageChanged;
        /// <summary>
        /// 
        /// </summary>
        public event RoutedEventHandler NextCalendarPageChanged;
        /// <summary>
        /// 
        /// </summary>
        public event DependencyPropertyChangedEventHandler SelectedDateChanged;

        #endregion Public Events

        /// <summary>
        /// Static constructor
        /// </summary>
        static Calendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(typeof(Calendar)));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(KeyboardNavigationMode.Contained));

            EventManager.RegisterClassHandler(typeof(Calendar), UIElement.GotFocusEvent, new RoutedEventHandler(OnGotFocus));
            LanguageProperty.OverrideMetadata(typeof(Calendar), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLanguageChanged)));
        }

        /// <summary>
        /// Initializes a new instance of the Calendar class.
        /// </summary>
        public Calendar()
        {
            this._blackoutDates = new CalendarBlackoutDatesCollection(this);
            this._selectedDates = new SelectedDatesCollection(this);
            this.FirstDayOfWeek = DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek;
            this.DisplayDate = DateTime.Today;
            if (EnvironmentTest.IsSecurityGranted)
            {
                EnvironmentTest.StartValidateLicense(typeof(Calendar));
            }
        }

        #region Public Properties

        #region BlackoutDates

        /// <summary>
        /// Gets or sets the dates that are not selectable.
        /// </summary>
        public CalendarBlackoutDatesCollection BlackoutDates
        {
            get { return _blackoutDates; }
            set { _blackoutDates = value; }
        }

        #endregion BlackoutDates

        #region CalendarButtonStyle

        /// <summary>
        /// Gets or sets the style for displaying a CalendarButton.
        /// </summary>
        public Style CalendarButtonStyle
        {
            get { return (Style)GetValue(CalendarButtonStyleProperty); }
            set { SetValue(CalendarButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the CalendarButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarButtonStyleProperty =
            DependencyProperty.Register(
            "CalendarButtonStyle",
            typeof(Style),
            typeof(Calendar));

        #endregion CalendarButtonStyle

        #region CalendarDayButtonStyle

        /// <summary>
        /// Gets or sets the style for displaying a day.
        /// </summary>
        public Style CalendarDayButtonStyle
        {
            get { return (Style)GetValue(CalendarDayButtonStyleProperty); }
            set { SetValue(CalendarDayButtonStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the DayButtonStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarDayButtonStyleProperty =
            DependencyProperty.Register(
            "CalendarDayButtonStyle",
            typeof(Style),
            typeof(Calendar));

        #endregion CalendarDayButtonStyle

        #region CalendarItemStyle

        /// <summary>
        /// Gets or sets the style for a Month.
        /// </summary>
        public Style CalendarItemStyle
        {
            get { return (Style)GetValue(CalendarItemStyleProperty); }
            set { SetValue(CalendarItemStyleProperty, value); }
        }

        /// <summary>
        /// Identifies the MonthStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty CalendarItemStyleProperty =
            DependencyProperty.Register(
            "CalendarItemStyle",
            typeof(Style),
            typeof(Calendar));

        #endregion CalendarItemStyle

        #region DisplayDate

        /// <summary>
        /// Gets or sets the date to display.
        /// </summary>
        /// 
        public DateTime DisplayDate
        {
            get { return (DateTime)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayDate dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register(
            "DisplayDate",
            typeof(DateTime),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DateTime.MinValue, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateChanged, CoerceDisplayDate));

        /// <summary>
        /// DisplayDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null);

            c.DisplayDateInternal = DateTimeHelper.DiscardDayTime((DateTime)e.NewValue);
            c.UpdateCellItems();
            c.OnDisplayDateChanged(new CalendarDateChangedEventArgs((DateTime)e.OldValue, (DateTime)e.NewValue));
        }

        private static object CoerceDisplayDate(DependencyObject d, object value)
        {
            Calendar c = d as Calendar;

            DateTime date = (DateTime)value;
            if (c.DisplayDateStart.HasValue && (date < c.DisplayDateStart.Value))
            {
                value = c.DisplayDateStart.Value;
            }
            else if (c.DisplayDateEnd.HasValue && (date > c.DisplayDateEnd.Value))
            {
                value = c.DisplayDateEnd.Value;
            }

            return value;
        }

        #endregion DisplayDate

        #region DisplayDateEnd

        /// <summary>
        /// Gets or sets the last date to be displayed.
        /// </summary>
        /// 
        public DateTime? DisplayDateEnd
        {
            get { return (DateTime?)GetValue(DisplayDateEndProperty); }
            set { SetValue(DisplayDateEndProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayDateEnd dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateEndProperty =
            DependencyProperty.Register(
            "DisplayDateEnd",
            typeof(DateTime?),
            typeof(Calendar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateEndChanged, CoerceDisplayDateEnd));

        /// <summary>
        /// DisplayDateEndProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateEnd.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null);

            c.CoerceValue(DisplayDateProperty);
            c.UpdateCellItems();
        }

        private static object CoerceDisplayDateEnd(DependencyObject d, object value)
        {
            Calendar c = d as Calendar;

            DateTime? date = (DateTime?)value;

            if (date.HasValue)
            {
                if (c.DisplayDateStart.HasValue && (date.Value < c.DisplayDateStart.Value))
                {
                    value = c.DisplayDateStart;
                }

                DateTime? maxSelectedDate = c.SelectedDates.MaximumDate;
                if (maxSelectedDate.HasValue && (date.Value < maxSelectedDate.Value))
                {
                    value = maxSelectedDate;
                }
            }

            return value;
        }

        #endregion DisplayDateEnd

        #region DisplayDateStart

        /// <summary>
        /// Gets or sets the first date to be displayed.
        /// </summary>
        /// 
        public DateTime? DisplayDateStart
        {
            get { return (DateTime?)GetValue(DisplayDateStartProperty); }
            set { SetValue(DisplayDateStartProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayDateStart dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayDateStartProperty =
            DependencyProperty.Register(
            "DisplayDateStart",
            typeof(DateTime?),
            typeof(Calendar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayDateStartChanged, CoerceDisplayDateStart));

        /// <summary>
        /// DisplayDateStartProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayDateStart.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayDateStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null);

            c.CoerceValue(DisplayDateEndProperty);
            c.CoerceValue(DisplayDateProperty);
            c.UpdateCellItems();
        }

        private static object CoerceDisplayDateStart(DependencyObject d, object value)
        {
            Calendar c = d as Calendar;

            DateTime? date = (DateTime?)value;

            if (date.HasValue)
            {
                DateTime? minSelectedDate = c.SelectedDates.MinimumDate;
                if (minSelectedDate.HasValue && (date.Value > minSelectedDate.Value))
                {
                    value = minSelectedDate;
                }
            }

            return value;
        }

        #endregion DisplayDateStart

        #region DisplayMode

        /// <summary>
        /// Gets or sets a value indicating whether the calendar is displayed in months or years.
        /// </summary>
        public CalendarMode DisplayMode
        {
            get { return (CalendarMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayMode dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
            "DisplayMode",
            typeof(CalendarMode),
            typeof(Calendar),
            new FrameworkPropertyMetadata(CalendarMode.Month, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayModePropertyChanged),
            new ValidateValueCallback(IsValidDisplayMode));

        /// <summary>
        /// DisplayModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its DisplayMode.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null);
            CalendarMode mode = (CalendarMode)e.NewValue;
            CalendarMode oldMode = (CalendarMode)e.OldValue;
            Syncfusion.Windows.Controls.Primitives.CalendarItem monthControl = c.MonthControl;

            switch (mode)
            {
                case CalendarMode.Month:
                    {
                        if (oldMode == CalendarMode.Year || oldMode == CalendarMode.Decade)
                        {
                            // Cancel highlight when switching to month display mode
                            c.HoverStart = c.HoverEnd = null;
                            c.CurrentDate = c.DisplayDate;
                        }

                        c.UpdateCellItems();
                        break;
                    }

                case CalendarMode.Year:
                case CalendarMode.Decade:
                    {
                        if (oldMode == CalendarMode.Month)
                        {
                            c.DisplayDate = c.CurrentDate;
                        }

                        c.UpdateCellItems();
                        break;
                    }

                default:
                    Debug.Assert(false);
                    break;
            }

            c.OnDisplayModeChanged(new CalendarModeChangedEventArgs((CalendarMode)e.OldValue, mode));
        }

        #endregion DisplayMode

        #region FirstDayOfWeek

        /// <summary>
        /// Gets or sets the day that is considered the beginning of the week.
        /// </summary>
        public DayOfWeek FirstDayOfWeek
        {
            get { return (DayOfWeek)GetValue(FirstDayOfWeekProperty); }
            set { SetValue(FirstDayOfWeekProperty, value); }
        }

        /// <summary>
        /// Identifies the FirstDayOfWeek dependency property.
        /// </summary>
        public static readonly DependencyProperty FirstDayOfWeekProperty =
            DependencyProperty.Register(
            "FirstDayOfWeek",
            typeof(DayOfWeek),
            typeof(Calendar),
            new FrameworkPropertyMetadata(DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek,
                                OnFirstDayOfWeekChanged),
            new ValidateValueCallback(IsValidFirstDayOfWeek));

        /// <summary>
        /// FirstDayOfWeekProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its FirstDayOfWeek.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnFirstDayOfWeekChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            c.UpdateCellItems();
        }
        /// <summary>
        /// 
        /// </summary>
        public bool CanBlockWeekEnds
        {
            get { return (bool)GetValue(CanBlockWeekEndsProperty); }
            set { SetValue(CanBlockWeekEndsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsWeekEndEnabled.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CanBlockWeekEndsProperty =
            DependencyProperty.Register("CanBlockWeekEnds", typeof(bool), typeof(Calendar), new UIPropertyMetadata(false, new PropertyChangedCallback(OnCanBlockWeekEndsChanged)));


        private static void OnCanBlockWeekEndsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            if ((bool)e.NewValue)
            {
                DateTime datetime = DateTimeHelper.DiscardDayTime(new DateTime(c.DisplayDate.Year-5, 1, 1));
                int count = Thread.CurrentThread.CurrentCulture.DateTimeFormat.Calendar.GetDaysInYear(datetime.Year, datetime.Month) * 20;
                for (int day = 0; day < count; day++)
                {
                    if (c.CanBlockWeekEnds && datetime.DayOfWeek == Thread.CurrentThread.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                    {
                        c.BlackoutDates.Add(new CalendarDateRange(datetime.AddDays(-1)));
                        c.BlackoutDates.Add(new CalendarDateRange(datetime));
                    }
                    datetime = datetime.AddDays(1);
                }
            }
        }


        #endregion FirstDayOfWeek


        /// <summary>
        /// 
        /// </summary>
        public bool DisableDateSelection
        {
            get { return (bool)GetValue(DisableDateSelectionProperty); }
            set { SetValue(DisableDateSelectionProperty, value); }
        }



        internal DateTime? NewDate
        {
            get { return (DateTime?)GetValue(NewDateProperty); }
            set { SetValue(NewDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewDate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NewDateProperty =
            DependencyProperty.Register("NewDate", typeof(DateTime?), typeof(Calendar), new PropertyMetadata(null,new PropertyChangedCallback(OnNewDateChanged)));
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
         public static void OnNewDateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Calendar d = (Calendar)obj;
            if (d != null)
            {
                d.OnNewDateChanged(args);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
         protected void OnNewDateChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.NewDateChanged != null)
                this.NewDateChanged(this, args);
            FireMonthChanged();
        }

         private void FireMonthChanged()
         {
             if (this.MonthChanged != null)
             {
                 this.MonthChanged(this, new RoutedEventArgs());
             }

         }
        // Using a DependencyProperty as the backing store for DisableDateSelection.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisableDateSelectionProperty =
            DependencyProperty.Register("DisableDateSelection", typeof(bool), typeof(Calendar), new PropertyMetadata(false,new PropertyChangedCallback(OnDisableDateSelectionChanged)));  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
         public static void OnDisableDateSelectionChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Calendar d = (Calendar)obj;
            if (d != null)
            {
                d.OnDisableDateSelectionChanged(args);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected void OnDisableDateSelectionChanged(DependencyPropertyChangedEventArgs args)
        {
            if (this.DisableDateSelectionChanged != null)
                this.DisableDateSelectionChanged(this, args);
            if (this.DisableDateSelection)
                this.DisplayMode = CalendarMode.Year;
            else if (!this.DisableDateSelection)
            {
                this.DisplayMode = CalendarMode.Month;
            }
        }

        #region IsTodayHighlighted

        /// <summary>
        /// Gets or sets a value indicating whether the current date is highlighted.
        /// </summary>
        public bool IsTodayHighlighted
        {
            get { return (bool)GetValue(IsTodayHighlightedProperty); }
            set { SetValue(IsTodayHighlightedProperty, value); }
        }

        /// <summary>
        /// Identifies the IsTodayHighlighted dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTodayHighlightedProperty =
            DependencyProperty.Register(
            "IsTodayHighlighted",
            typeof(bool),
            typeof(Calendar),
            new FrameworkPropertyMetadata(true, OnIsTodayHighlightedChanged));

        /// <summary>
        /// IsTodayHighlightedProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its IsTodayHighlighted.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnIsTodayHighlightedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;

            int i = DateTimeHelper.CompareYearMonth(c.DisplayDateInternal, DateTime.Today);

            if (i > -2 && i < 2)
            {
                c.UpdateCellItems();
            }
        }

        #endregion IsTodayHighlighted
        
        #region Language
        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            if (DependencyPropertyHelper.GetValueSource(d, Calendar.FirstDayOfWeekProperty).BaseValueSource == BaseValueSource.Default)
            {
                c.CoerceValue(FirstDayOfWeekProperty);
                c.UpdateCellItems();
            }
        }
        #endregion

        #region SelectedDate

        /// <summary>
        /// Gets or sets the currently selected date.
        /// </summary>
        /// 
        public DateTime? SelectedDate
        {
            get { return (DateTime?)GetValue(SelectedDateProperty); }
            set { SetValue(SelectedDateProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedDate dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
            "SelectedDate",
            typeof(DateTime?),
            typeof(Calendar),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateChanged));

        /// <summary>
        /// SelectedDateProperty property changed handler.
        /// </summary>
        /// <param name="d">Calendar that changed its SelectedDate.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param>
        private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null);

            if (c.SelectionMode != CalendarSelectionMode.None || e.NewValue == null)
            {
                DateTime? addedDate;

                addedDate = (DateTime?)e.NewValue;

                if (IsValidDateSelection(c, addedDate))
                {
                    if (!addedDate.HasValue)
                    {
                        c.SelectedDates.ClearInternal(true /*fireChangeNotification*/);
                    }
                    else
                    {
                        if (addedDate.HasValue && !(c.SelectedDates.Count > 0 && c.SelectedDates[0] == addedDate.Value))
                        {
                            c.SelectedDates.ClearInternal();
                            c.SelectedDates.Add(addedDate.Value);
                        }
                    }

                    // We update the current date for only the Single mode.For the other modes it automatically gets updated
                    if (c.SelectionMode == CalendarSelectionMode.SingleDate)
                    {
                        if (addedDate.HasValue)
                        {
                            c.CurrentDate = addedDate.Value;
                        }
                        c.OnSelectedDateChanged(e);
                        c.UpdateCellItems();
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("d", SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidValue));
                }
            }
            else
            {
                throw new InvalidOperationException(SR.Get(SRID.Calendar_OnSelectedDateChanged_InvalidOperation));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected void OnSelectedDateChanged(DependencyPropertyChangedEventArgs e)
        {
            if (SelectedDateChanged != null)
            {
                SelectedDateChanged(this, e);
            }
        }
        #endregion SelectedDate

        #region SelectedDates

        // TODO: Should it be of type ObservableCollection?

        /// <summary>
        /// Gets the dates that are currently selected.
        /// </summary>
        public SelectedDatesCollection SelectedDates
        {
            get { return _selectedDates; }
        }

        #endregion SelectedDates

        #region SelectionMode

        /// <summary>
        /// Gets or sets the selection mode for the calendar.
        /// </summary>
        public CalendarSelectionMode SelectionMode
        {
            get { return (CalendarSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
            "SelectionMode",
            typeof(CalendarSelectionMode),
            typeof(Calendar),
            new FrameworkPropertyMetadata(CalendarSelectionMode.SingleDate, OnSelectionModeChanged),
            new ValidateValueCallback(IsValidSelectionMode));

        private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Calendar c = d as Calendar;
            Debug.Assert(c != null);

            c.HoverStart = c.HoverEnd = null;
            c.SelectedDates.ClearInternal(true /*fireChangeNotification*/);
            c.OnSelectionModeChanged(EventArgs.Empty);
        }

        #endregion SelectionMode

        #endregion Public Properties

        #region Internal Events

        internal event MouseButtonEventHandler DayButtonMouseUp;

        internal event RoutedEventHandler DayOrMonthPreviewKeyDown;

        #endregion Internal Events

        #region Internal Properties

        /// <summary>
        /// This flag is used to determine whether DatePicker should change its 
        /// DisplayDate because of a SelectedDate change on its Calendar
        /// </summary>
        internal bool DatePickerDisplayDateFlag
        {
            get;
            set;
        }

        internal DateTime DisplayDateInternal
        {
            get;
            private set;
        }

        internal DateTime DisplayDateEndInternal
        {
            get
            {
                return this.DisplayDateEnd.GetValueOrDefault(DateTime.MaxValue);
            }
        }

        internal DateTime DisplayDateStartInternal
        {
            get
            {
                return this.DisplayDateStart.GetValueOrDefault(DateTime.MinValue);
            }
        }

        internal DateTime CurrentDate
        {
            get { return _currentDate.GetValueOrDefault(this.DisplayDateInternal); }
            set { _currentDate = value; }
        }

        internal DateTime? HoverStart
        {
            get
            {
                return this.SelectionMode == CalendarSelectionMode.None ? null : _hoverStart;
            }

            set 
            { 
                _hoverStart = value; 
            }
        }

        internal DateTime? HoverEnd
        {
            get
            {
                return this.SelectionMode == CalendarSelectionMode.None ? null : _hoverEnd;
            }

            set
            {
                _hoverEnd = value;
            }
        }

        internal Syncfusion.Windows.Controls.Primitives.CalendarItem MonthControl
        {
            get { return _monthControl; }
        }

        internal DateTime DisplayMonth
        {
            get
            {
                return DateTimeHelper.DiscardDayTime(DisplayDate);
            }
        }

        internal DateTime DisplayYear
        {
            get
            {
                return new DateTime(DisplayDate.Year, 1, 1);
            }
        }

        #endregion Internal Properties

        #region Private Properties

        private bool IsRightToLeft
        {
            get 
            {
                return FlowDirection == FlowDirection.RightToLeft;
            }
        }
        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Invoked whenever application code or an internal process, 
        /// such as a rebuilding layout pass, calls the ApplyTemplate method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            if (_monthControl != null)
            {
                _monthControl.Owner = null;
            }

            base.OnApplyTemplate();

            _monthControl = GetTemplateChild(ElementMonth) as Syncfusion.Windows.Controls.Primitives.CalendarItem;

            if (_monthControl != null)
            {
                _monthControl.Owner = this;
            }

            this.CurrentDate = this.DisplayDate;
            UpdateCellItems();
        }

        /// <summary>
        /// Provides a text representation of the selected date.
        /// </summary>
        /// <returns>A text representation of the selected date, or an empty string if SelectedDate is a null reference.</returns>
        public override string ToString()
        {
            if (this.SelectedDate != null)
            {
                return this.SelectedDate.Value.ToString(DateTimeHelper.GetDateFormat(DateTimeHelper.GetCulture(this)));
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Public Methods

        #region Protected Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectedDatesChanged(SelectionChangedEventArgs e)
        {
            RaiseEvent(e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDisplayDateChanged(CalendarDateChangedEventArgs e)
        {
            EventHandler<CalendarDateChangedEventArgs> handler = this.DisplayDateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDisplayModeChanged(CalendarModeChangedEventArgs e)
        {
            EventHandler<CalendarModeChangedEventArgs> handler = this.DisplayModeChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnSelectionModeChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.SelectionModeChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Creates the automation peer for this Calendar Control.
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new Syncfusion.Windows.Automation.Peers.CalendarAutomationPeer(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = ProcessCalendarKey(e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    ProcessShiftKeyUp();
                }
            }
        }

        #endregion Protected Methods

        #region Internal Methods

        internal Syncfusion.Windows.Controls.Primitives.CalendarDayButton FindDayButtonFromDay(DateTime day)
        {
            if (this.MonthControl != null)
            {
                foreach (Syncfusion.Windows.Controls.Primitives.CalendarDayButton b in this.MonthControl.GetCalendarDayButtons())
                {
                    if (b.DataContext is DateTime)
                    {
                        if (DateTimeHelper.CompareDays((DateTime)b.DataContext, day) == 0)
                        {
                            return b;
                        }
                    }
                }
            }

            return null;
        }

        internal static bool IsValidDateSelection(Calendar cal, object value)
        {
            return (value == null) || (!cal.BlackoutDates.Contains((DateTime)value));
        }

        internal void OnDayButtonMouseUp(MouseButtonEventArgs e)
        {
            MouseButtonEventHandler handler = this.DayButtonMouseUp;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        internal void OnDayOrMonthPreviewKeyDown(RoutedEventArgs e)
        {
            RoutedEventHandler handler = this.DayOrMonthPreviewKeyDown;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        // If the day is a trailing day, Update the DisplayDate
        internal void OnDayClick(DateTime selectedDate)
        {
            if (this.SelectionMode == CalendarSelectionMode.None)
            {
                this.CurrentDate = selectedDate;
            }

            if (DateTimeHelper.CompareYearMonth(selectedDate, this.DisplayDateInternal) != 0)
            {
                MoveDisplayTo(selectedDate);
            }
            else
            {
                UpdateCellItems();
                FocusDate(selectedDate);
            }            
        }

        internal void OnMonthCalendarButtonPressed(Syncfusion.Windows.Controls.Primitives.CalendarButton b, bool switchDisplayMode)
        {
            if (b.DataContext is DateTime)
            {
                DateTime d = (DateTime)b.DataContext;

                DateTime? newDate = null;
                CalendarMode newMode = CalendarMode.Month;

                switch (this.DisplayMode)
                {                   
                    case CalendarMode.Year:
                        {
                            newDate = DateTimeHelper.SetYearMonth(this.DisplayDate, d);
                            if (this.DisableDateSelection)
                            {
                                DateTime date = (DateTime)newDate;
                                int day = DateTime.DaysInMonth((int)date.Year, (int)date.Month);
                                this.NewDate = DateTimeHelper.SetDay(date, day);
                            }
                            else
                            this.NewDate = newDate;                              
                            break;
                        }

                    case CalendarMode.Decade:
                        {
                            newDate = DateTimeHelper.SetYear(this.DisplayDate, d.Year);
                            if (this.DisableDateSelection)
                            {
                                DateTime date = (DateTime)newDate;
                                int day = DateTime.DaysInMonth((int)date.Year, (int)date.Month);
                                this.NewDate = DateTimeHelper.SetDay(date, day);
                            }
                            else
                                this.NewDate = newDate; 
                            newMode = CalendarMode.Year;
                            break;
                        }

                    default:
                        Debug.Assert(false);
                        break;
                }

                if (newDate.HasValue)
                {
                    this.DisplayDate = newDate.Value;
                    if (switchDisplayMode)
                    {
                        this.DisplayMode = newMode;
                        this.SelectedDate=this.CurrentDate;
                        FocusDate(this.DisplayMode == CalendarMode.Year ? this.CurrentDate : this.DisplayDate);
                    }
                }
            }
        }
        internal void OnCalendarButtonPressed(Syncfusion.Windows.Controls.Primitives.CalendarButton b, bool switchDisplayMode)
        {
            if (b.DataContext is DateTime)
            {
                DateTime d = (DateTime)b.DataContext;

                DateTime? newDate = null;
                CalendarMode newMode = CalendarMode.Month;

                switch (this.DisplayMode)
                {
                    case CalendarMode.Month:
                    {
                        Debug.Assert(false);
                        break;
                    }

                    case CalendarMode.Year:
                    {
                        newDate = DateTimeHelper.SetYearMonth(this.DisplayDate, d);
                        newMode = CalendarMode.Month;
                        break;
                    }

                    case CalendarMode.Decade:
                    {
                        newDate = DateTimeHelper.SetYear(this.DisplayDate, d.Year);
                        if (this.DisableDateSelection)
                        {
                            DateTime date = (DateTime)newDate;
                            int day = DateTime.DaysInMonth((int)date.Year, (int)date.Month);
                            this.NewDate = DateTimeHelper.SetDay(date, day);
                        }
                        else
                            this.NewDate = newDate; 
                        newMode = CalendarMode.Year;
                        break;
                    }

                    default: 
                        Debug.Assert(false); 
                        break;
                }

                if (newDate.HasValue)
                {
                    this.DisplayDate = newDate.Value;
                    if (switchDisplayMode)
                    {
                        this.DisplayMode = newMode;
                        FocusDate(this.DisplayMode == CalendarMode.Month ? this.CurrentDate : this.DisplayDate);
                    }
                }
            }
        }

        private DateTime? GetDateOffset(DateTime date, int offset, CalendarMode displayMode)
        {
            DateTime? result = null;
            switch (displayMode)
            {
                case CalendarMode.Month:
                {
                    result = DateTimeHelper.AddMonths(date, offset);
                    break;
                }

                case CalendarMode.Year:
                {
                    result = DateTimeHelper.AddYears(date, offset);
                    break;
                }

                case CalendarMode.Decade:
                {
                    result = DateTimeHelper.AddYears(this.DisplayDate, offset * YEARS_PER_DECADE);
                    break;
                }

                default:
                Debug.Assert(false);
                break;
            }

            return result;
        }

        private void MoveDisplayTo(DateTime? date)
        {
            if (date.HasValue)
            {
                DateTime d = date.Value.Date;
                switch (this.DisplayMode)
                {
                    case CalendarMode.Month:
                    {
                        this.DisplayDate = DateTimeHelper.DiscardDayTime(d);
                        this.CurrentDate = d;
                        UpdateCellItems();

                        break;
                    }

                    case CalendarMode.Year:
                    case CalendarMode.Decade:
                    {
                        this.DisplayDate = d;
                        UpdateCellItems();
                        break;
                    }

                    default:
                    Debug.Assert(false);
                    break;
                }

                FocusDate(d);
            }
        }

        internal void FireOnNextClick()
        {
             if (NextCalendarPageChanged != null)
              {
                 NextCalendarPageChanged(this, new RoutedEventArgs());
              }
        }


        internal void FireOnPreviousClick()
        {
            if (PreviousCalendarPageChanged != null)
            {
                PreviousCalendarPageChanged(this, new RoutedEventArgs());
            }
        }

        internal void OnNextClick()
        {
            DateTime? nextDate = GetDateOffset(this.DisplayDate, 1, this.DisplayMode);
            if (nextDate.HasValue)
            {
                MoveDisplayTo(DateTimeHelper.DiscardDayTime(nextDate.Value));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowNextCalendarPage()
        {
            this.OnNextClick();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowPreviousCalendarPage()
        {
            this.OnPreviousClick();
        }
        internal void OnPreviousClick()
        {
            DateTime? nextDate = GetDateOffset(this.DisplayDate, -1, this.DisplayMode);
            if (nextDate.HasValue)
            {
                MoveDisplayTo(DateTimeHelper.DiscardDayTime(nextDate.Value));
            }
        }

        internal void OnSelectedDatesCollectionChanged(SelectionChangedEventArgs e)
        {
            if (IsSelectionChanged(e))
            {
                if (AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementSelected) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementAddedToSelection) ||
                    AutomationPeer.ListenerExists(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection))
                {
                    Syncfusion.Windows.Automation.Peers.CalendarAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(this) as Syncfusion.Windows.Automation.Peers.CalendarAutomationPeer;
                    if (peer != null)
                    {
                        peer.RaiseSelectionEvents(e);
                    }
                }

                CoerceFromSelection();
                OnSelectedDatesChanged(e);
            }
        }

        internal void UpdateCellItems()
        {
            Syncfusion.Windows.Controls.Primitives.CalendarItem monthControl = this.MonthControl;
            if (monthControl != null)
            {
                switch (this.DisplayMode)
                {
                    case CalendarMode.Month:
                    {
                        monthControl.UpdateMonthMode();
                        break;
                    }

                    case CalendarMode.Year:
                    {
                        monthControl.UpdateYearMode();
                        break;
                    }

                    case CalendarMode.Decade:
                    {
                        monthControl.UpdateDecadeMode();
                        break;
                    }

                    default: 
                        Debug.Assert(false); 
                        break;
                }
            }
        }

        #endregion Internal Methods

        #region Private Methods

        private void CoerceFromSelection()
        {
            CoerceValue(DisplayDateStartProperty);
            CoerceValue(DisplayDateEndProperty);
            CoerceValue(DisplayDateProperty);
        }

        // This method adds the days that were selected by Keyboard to the SelectedDays Collection 
        private void AddKeyboardSelection()
        {
            if (this.HoverStart != null)
            {
                this.SelectedDates.ClearInternal();

                // In keyboard selection, we are sure that the collection does not include any blackout days
                this.SelectedDates.AddRange(this.HoverStart.Value, this.CurrentDate);
            }
        }

        private static bool IsSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != e.RemovedItems.Count)
            {
                return true;
            }

            foreach (DateTime addedDate in e.AddedItems)
            {
                if (!e.RemovedItems.Contains(addedDate))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsValidDisplayMode(object value)
        {
            CalendarMode mode = (CalendarMode)value;

            return mode == CalendarMode.Month
                || mode == CalendarMode.Year
                || mode == CalendarMode.Decade;
        }

        internal static bool IsValidFirstDayOfWeek(object value)
        {
            DayOfWeek day = (DayOfWeek)value;

            return day == DayOfWeek.Sunday
                || day == DayOfWeek.Monday
                || day == DayOfWeek.Tuesday
                || day == DayOfWeek.Wednesday
                || day == DayOfWeek.Thursday
                || day == DayOfWeek.Friday
                || day == DayOfWeek.Saturday;
        }

        private static bool IsValidKeyboardSelection(Calendar cal, object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                if (cal.BlackoutDates.Contains((DateTime)value))
                {
                    return false;
                }
                else
                {
                    return DateTime.Compare((DateTime)value, cal.DisplayDateStartInternal) >= 0 && DateTime.Compare((DateTime)value, cal.DisplayDateEndInternal) <= 0;
                }
            }
        }

        private static bool IsValidSelectionMode(object value)
        {
            CalendarSelectionMode mode = (CalendarSelectionMode)value;

            return mode == CalendarSelectionMode.SingleDate
                || mode == CalendarSelectionMode.SingleRange
                || mode == CalendarSelectionMode.MultipleRange
                || mode == CalendarSelectionMode.None;
        }

        private void OnSelectedMonthChanged(DateTime? selectedMonth)
        {
            if (selectedMonth.HasValue)
            {
                Debug.Assert(this.DisplayMode == CalendarMode.Year);
                this.DisplayDate = selectedMonth.Value;

                UpdateCellItems();

                FocusDate(selectedMonth.Value);
            }
        }

        private void OnSelectedYearChanged(DateTime? selectedYear)
        {
            if (selectedYear.HasValue)
            {
                Debug.Assert(this.DisplayMode == CalendarMode.Decade);
                this.DisplayDate = selectedYear.Value;

                UpdateCellItems();

                FocusDate(selectedYear.Value);
            }
        }

        internal void FocusDate(DateTime date)
        {
            if (MonthControl != null)
            {
                MonthControl.FocusDate(date);
            }
        }

        /// <summary>
        ///     Called when this element gets focus.
        /// </summary>
        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // When Calendar gets focus move it to the DisplayDate
            var c = (Calendar)sender;
            if (!e.Handled && e.OriginalSource == c)
            {
                // This check is for the case where the DisplayDate is the first of the month
                // and the SelectedDate is in the middle of the month.  If you tab into the Calendar
                // the focus should go to the SelectedDate, not the DisplayDate.
                if (c.SelectedDate.HasValue && DateTimeHelper.CompareYearMonth(c.SelectedDate.Value, c.DisplayDateInternal) == 0)
                {
                    c.FocusDate(c.SelectedDate.Value);
                }
                else
                {
                    c.FocusDate(c.DisplayDate);
                }

                e.Handled = true;
            }
        }

        private bool ProcessCalendarKey(KeyEventArgs e)
        {
            if (this.DisplayMode == CalendarMode.Month)
            {
                // If a blackout day is inactive, when clicked on it, the previous inactive day which is not a blackout day can get the focus.
                // In this case we should allow keyboard functions on that inactive day
                Syncfusion.Windows.Controls.Primitives.CalendarDayButton currentDayButton = (MonthControl != null) ? MonthControl.GetCalendarDayButton(this.CurrentDate) : null;

                if (DateTimeHelper.CompareYearMonth(this.CurrentDate, this.DisplayDateInternal) != 0 && currentDayButton != null && !currentDayButton.IsInactive)
                {
                    return false;
                }
            }

            bool ctrl, shift;
            KeyboardHelper.GetMetaKeyState(out ctrl, out shift);

            switch (e.Key)
            {
                case Key.Up:
                {
                    ProcessUpKey(ctrl, shift);
                    return true;
                }

                case Key.Down:
                {
                    ProcessDownKey(ctrl, shift);
                    return true;
                }

                case Key.Left:
                {
                    ProcessLeftKey(shift);
                    return true;
                }

                case Key.Right:
                {
                    ProcessRightKey(shift);
                    return true;
                }

                case Key.PageDown:
                {
                    ProcessPageDownKey(shift);
                    return true;
                }

                case Key.PageUp:
                {
                    ProcessPageUpKey(shift);
                    return true;
                }

                case Key.Home:
                {
                    ProcessHomeKey(shift);
                    return true;
                }

                case Key.End:
                {
                    ProcessEndKey(shift);
                    return true;
                }

                case Key.Enter:
                case Key.Space:
                {
                    return ProcessEnterKey();
                }
            }

            return false;
        }

        private void ProcessDownKey(bool ctrl, bool shift)
        {
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    if (!ctrl || shift)
                    {
                        DateTime? selectedDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, COLS), 1);
                        ProcessSelection(shift, selectedDate);
                    }

                    break;
                }

                case CalendarMode.Year:
                {
                    if (ctrl)
                    {
                        this.DisplayMode = CalendarMode.Month;
                        FocusDate(this.DisplayDate);
                    }
                    else
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddMonths(this.DisplayDate, YEAR_COLS);
                        OnSelectedMonthChanged(selectedMonth);
                    }

                    break;
                }

                case CalendarMode.Decade:
                {
                    if (ctrl)
                    {                        
                        this.DisplayMode = CalendarMode.Year;
                        FocusDate(this.DisplayDate);
                    }
                    else
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(this.DisplayDate, YEAR_COLS);
                        OnSelectedYearChanged(selectedYear);
                    }

                    break;
                }
            }
        }

        private void ProcessEndKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    if (this.DisplayDate != null)
                    {
                        DateTime? selectedDate = new DateTime(this.DisplayDateInternal.Year, this.DisplayDateInternal.Month, 1);

                        if (DateTimeHelper.CompareYearMonth(DateTime.MaxValue, selectedDate.Value) > 0)
                        {
                            // since DisplayDate is not equal to DateTime.MaxValue we are sure selectedDate is not null
                            selectedDate = DateTimeHelper.AddMonths(selectedDate.Value, 1).Value;
                            selectedDate = DateTimeHelper.AddDays(selectedDate.Value, -1).Value;
                        }
                        else
                        {
                            selectedDate = DateTime.MaxValue;
                        }

                        ProcessSelection(shift, selectedDate);
                    }

                    break;
                }

                case CalendarMode.Year:
                {
                    DateTime selectedMonth = new DateTime(this.DisplayDate.Year, 12, 1);
                    OnSelectedMonthChanged(selectedMonth);
                    break;
                }

                case CalendarMode.Decade:
                {
                    DateTime? selectedYear = new DateTime(DateTimeHelper.EndOfDecade(this.DisplayDate), 1, 1);
                    OnSelectedYearChanged(selectedYear);
                    break;
                }
            }
        }

        private bool ProcessEnterKey()
        {
            switch (this.DisplayMode)
            {
                case CalendarMode.Year:
                {
                    this.DisplayMode = CalendarMode.Month;
                    FocusDate(this.DisplayDate);
                    return true;
                }

                case CalendarMode.Decade:
                {
                    this.DisplayMode = CalendarMode.Year;
                    FocusDate(this.DisplayDate);
                    return true;
                }
            }

            return false;
        }

        private void ProcessHomeKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    // TODO: Not all types of calendars start with Day1. If Non-Gregorian is supported check this:
                    DateTime? selectedDate = new DateTime(this.DisplayDateInternal.Year, this.DisplayDateInternal.Month, 1);
                    ProcessSelection(shift, selectedDate);
                    break;
                }

                case CalendarMode.Year:
                {
                    DateTime selectedMonth = new DateTime(this.DisplayDate.Year, 1, 1);
                    OnSelectedMonthChanged(selectedMonth);
                    break;
                }

                case CalendarMode.Decade:
                {
                    DateTime? selectedYear = new DateTime(DateTimeHelper.DecadeOfDate(this.DisplayDate), 1, 1);
                    OnSelectedYearChanged(selectedYear);
                    break;
                }
            }
        }

        private void ProcessLeftKey(bool shift)
        {
            int moveAmmount = (!this.IsRightToLeft) ? -1 : 1;
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    DateTime? selectedDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, moveAmmount), moveAmmount);
                    ProcessSelection(shift, selectedDate);
                    break;
                }

                case CalendarMode.Year:
                {
                    DateTime? selectedMonth = DateTimeHelper.AddMonths(this.DisplayDate, moveAmmount);
                    OnSelectedMonthChanged(selectedMonth);
                    break;
                }

                case CalendarMode.Decade:
                {
                    DateTime? selectedYear = DateTimeHelper.AddYears(this.DisplayDate, moveAmmount);
                    OnSelectedYearChanged(selectedYear);
                    break;
                }
            }
        }

        private void ProcessPageDownKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    DateTime? selectedDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddMonths(this.CurrentDate, 1), 1);
                    ProcessSelection(shift, selectedDate);
                    break;
                }

                case CalendarMode.Year:
                {
                    DateTime? selectedMonth = DateTimeHelper.AddYears(this.DisplayDate, 1);
                    OnSelectedMonthChanged(selectedMonth);
                    break;
                }

                case CalendarMode.Decade:
                {
                    DateTime? selectedYear = DateTimeHelper.AddYears(this.DisplayDate, 10);
                    OnSelectedYearChanged(selectedYear);
                    break;
                }
            }
        }

        private void ProcessPageUpKey(bool shift)
        {
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    DateTime? selectedDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddMonths(this.CurrentDate, -1), -1);
                    ProcessSelection(shift, selectedDate);
                    break;
                }

                case CalendarMode.Year:
                {
                    DateTime? selectedMonth = DateTimeHelper.AddYears(this.DisplayDate, -1);
                    OnSelectedMonthChanged(selectedMonth);
                    break;
                }

                case CalendarMode.Decade:
                {
                    DateTime? selectedYear = DateTimeHelper.AddYears(this.DisplayDate, -10);
                    OnSelectedYearChanged(selectedYear);
                    break;
                }
            }
        }

        private void ProcessRightKey(bool shift)
        {
            int moveAmmount = (!this.IsRightToLeft) ? 1 : -1;
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    DateTime? selectedDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, moveAmmount), moveAmmount);
                    ProcessSelection(shift, selectedDate);
                    break;
                }

                case CalendarMode.Year:
                {
                    DateTime? selectedMonth = DateTimeHelper.AddMonths(this.DisplayDate, moveAmmount);
                    OnSelectedMonthChanged(selectedMonth);
                    break;
                }

                case CalendarMode.Decade:
                {
                    DateTime? selectedYear = DateTimeHelper.AddYears(this.DisplayDate, moveAmmount);
                    OnSelectedYearChanged(selectedYear);
                    break;
                }
            }
        }

        private void ProcessSelection(bool shift, DateTime? lastSelectedDate)
        {
            if (this.SelectionMode == CalendarSelectionMode.None && lastSelectedDate != null)
            {
                OnDayClick(lastSelectedDate.Value);
                return;
            }

            if (lastSelectedDate != null && IsValidKeyboardSelection(this, lastSelectedDate.Value))
            {
                if (this.SelectionMode == CalendarSelectionMode.SingleRange || this.SelectionMode == CalendarSelectionMode.MultipleRange)
                {
                    this.SelectedDates.ClearInternal();
                    if (shift)
                    {
                        this._isShiftPressed = true;
                        if (!this.HoverStart.HasValue)
                        {
                            this.HoverStart = this.HoverEnd = this.CurrentDate;
                        }
                        
                        // If we hit a BlackOutDay with keyboard we do not update the HoverEnd
                        CalendarDateRange range;

                        if (DateTime.Compare(this.HoverStart.Value, lastSelectedDate.Value) < 0)
                        {
                            range = new CalendarDateRange(this.HoverStart.Value, lastSelectedDate.Value);
                        }
                        else
                        {
                            range = new CalendarDateRange(lastSelectedDate.Value, this.HoverStart.Value);
                        }

                        if (!this.BlackoutDates.ContainsAny(range))
                        {
                            this._currentDate = lastSelectedDate;
                            this.HoverEnd = lastSelectedDate;
                        }

                        OnDayClick(this.CurrentDate);
                    }
                    else
                    {
                        this.HoverStart = this.HoverEnd = this.CurrentDate = lastSelectedDate.Value;
                        AddKeyboardSelection();
                        OnDayClick(lastSelectedDate.Value);                        
                    }                    
                }
                else
                {
                    // ON CLEAR 
                    this.CurrentDate = lastSelectedDate.Value;
                    this.HoverStart = this.HoverEnd = null;
                    if (this.SelectedDates.Count > 0)
                    {
                        this.SelectedDates[0] = lastSelectedDate.Value;
                    }
                    else
                    {
                        this.SelectedDates.Add(lastSelectedDate.Value);
                    }

                    OnDayClick(lastSelectedDate.Value);
                }

                UpdateCellItems();
            }
        }

        private void ProcessShiftKeyUp()
        {
            if (this._isShiftPressed && (this.SelectionMode == CalendarSelectionMode.SingleRange || this.SelectionMode == CalendarSelectionMode.MultipleRange))
            {
                AddKeyboardSelection();
                this._isShiftPressed = false;
                this.HoverStart = this.HoverEnd = null;
            }
        }

        private void ProcessUpKey(bool ctrl, bool shift)
        {
            switch (this.DisplayMode)
            {
                case CalendarMode.Month:
                {
                    if (ctrl)
                    {
                        this.DisplayMode = CalendarMode.Year;
                        FocusDate(this.DisplayDate);
                    }
                    else
                    {
                        DateTime? selectedDate = this._blackoutDates.GetNonBlackoutDate(DateTimeHelper.AddDays(this.CurrentDate, -COLS), -1);
                        ProcessSelection(shift, selectedDate);
                    }

                    break;
                }

                case CalendarMode.Year:
                {
                    if (ctrl)
                    {
                        this.DisplayMode = CalendarMode.Decade;
                        FocusDate(this.DisplayDate);
                    }
                    else
                    {
                        DateTime? selectedMonth = DateTimeHelper.AddMonths(this.DisplayDate, -YEAR_COLS);
                        OnSelectedMonthChanged(selectedMonth);
                    }

                    break;
                }

                case CalendarMode.Decade:
                {
                    if (!ctrl)
                    {
                        DateTime? selectedYear = DateTimeHelper.AddYears(this.DisplayDate, -YEAR_COLS);
                        OnSelectedYearChanged(selectedYear);
                    }

                    break;
                }
            }
        }

        #endregion Private Methods
    }
}
