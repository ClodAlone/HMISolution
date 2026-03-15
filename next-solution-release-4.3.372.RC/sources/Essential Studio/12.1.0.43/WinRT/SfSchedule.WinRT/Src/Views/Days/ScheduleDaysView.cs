#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Collections;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a schedule's day view.
    /// </summary>
    public class ScheduleDaysView : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleDaysView">ScheduleDaysView</see>
        /// class.
        /// </summary>
        public ScheduleDaysView()
        {
            TodayTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            TodayTimer.Tick += TodayTimer_Tick;
            NonWorkingDateCollection = new ObservableCollection<DayOfWeek>();
            DefaultStyleKey = typeof(ScheduleDaysView);
            
            Loaded += ScheduleDaysView_Loaded;
#if WPF
            IsManipulationEnabled = true;
            SizeChanged += ScheduleDaysView_SizeChanged;
#endif
        }

        #endregion

        #region Internal Fields

        internal double dragDropCanvasHeight;
        internal double visibleCanvasHeight;
        internal bool dragResizeFlag;
        internal DispatcherTimer TodayTimer;
        internal ScrollViewer scrollviewer, headerscrollviewer, timelinescrollviewer;
        internal ItemsControl resourcecontainer;
        internal ContentPresenter previousNavigationTap;
        internal ContentPresenter nextNavigationTap;
        internal double hourHeight, doubleHour;
        internal double row = -1;
        internal bool dayCanvasFromAllday;
#if !WINRT
        internal TextBox appTextBox;
#endif

        #endregion

        #region Private Fields

        private ScheduleNonWorkingDayItemsControl nonworkingdaysLayout;
        private ScheduleAllDaysAppointmentItemsControl alldaysAppointmentsLayout;
        private ScheduleDaysAppointmentLayoutItemsControl daysAppointmentsLayout;
        private ScheduleHorizontalTimeSlotItemsControl scheduleHorizontalTimeSlotItemsControl;
        private TimeSpan endspan, startspan;
        private bool IsTemplateApplied;
        private SfSchedule schedule;
        private double DayDiff, DatetimeDiff;
        private double previousYValue;
        private double dayAppCanvasHeight;
        private bool dayCanvasInAllday;
        private ContentPresenter currentTimeIndicatorPresenter;
        List<List<string>> childstringcollection;
#if !WINRT
        private ScheduleDaysHeaderViewItemsControl scheduleDaysHeaderViewItemsControl;
#endif

        #endregion

        #region Dependency Properties

        #region Public Properties

        #region VisibleAppointments
        /// <summary>
        /// Gets or sets the collection of visible appointments in day view.
        /// </summary>
        /// <seealso
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleAppointmentCollection">ScheduleAppointmentCollection</seealso>
        public ScheduleAppointmentCollection VisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(VisibleAppointmentsProperty); }
            set { SetValue(VisibleAppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleAppointmentsProperty =
            DependencyProperty.Register("VisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(ScheduleDaysView), new PropertyMetadata(null, VisibleAppointmentsChanged));

        private static void VisibleAppointmentsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var sdv = dpo as ScheduleDaysView;
            if (sdv != null && sdv.IsTemplateApplied)
            {
#if WINRT
                if (sdv.schedule != null && (sdv.schedule.ScheduleType == ScheduleType.Day || sdv.schedule.ScheduleType == ScheduleType.Week || sdv.schedule.ScheduleType == ScheduleType.WorkWeek) && sdv.schedule.VisibleDateChanged == false)
#endif
                    sdv.SetupAppointments();
            }
        }
        #endregion

        #region SelectedDates
        /// <summary>
        /// Gets or sets the selected dates of day view.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleDaysView), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var sdv = dpo as ScheduleDaysView;
            if (sdv != null && sdv.schedule != null && (sdv.schedule.ScheduleType == ScheduleType.Day || sdv.schedule.ScheduleType == ScheduleType.Week || sdv.schedule.ScheduleType == ScheduleType.WorkWeek) && sdv.IsTemplateApplied)
            {
                sdv.SetupAppointments();
            }
        }
        #endregion

        #region ShowAppointmentNavigationButtons
        /// <summary>
        /// Gets or sets a value indicating whether the appointment navigation buttons should be shown in day view.
        /// </summary>
        public bool ShowAppointmentNavigationButtons
        {
            get { return (bool)GetValue(ShowAppointmentNavigationButtonsProperty); }
            set { SetValue(ShowAppointmentNavigationButtonsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ShowAppointmentNavigationButtons.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ShowAppointmentNavigationButtonsProperty =
            DependencyProperty.Register("ShowAppointmentNavigationButtons", typeof(bool), typeof(ScheduleDaysView), new PropertyMetadata(false));
        #endregion

        #region PreviousNavigationButtonTemplate
        /// <summary>
        /// Gets or sets the template for customizing button which navigates to previous appointments.
        /// </summary>
        public DataTemplate PreviousNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(PreviousNavigationButtonTemplateProperty); }
            set { SetValue(PreviousNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for PreviousNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty PreviousNavigationButtonTemplateProperty =
            DependencyProperty.Register("PreviousNavigationButtonTemplate", typeof(DataTemplate), typeof(ScheduleDaysView), new PropertyMetadata(null));
        #endregion

        #region NextNavigationButtonTemplate
        /// <summary>
        /// Gets or sets the template for customizing button which navigates to next appointments.
        /// </summary>
        public DataTemplate NextNavigationButtonTemplate
        {
            get { return (DataTemplate)GetValue(NextNavigationButtonTemplateProperty); }
            set { SetValue(NextNavigationButtonTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for NextNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty NextNavigationButtonTemplateProperty =
            DependencyProperty.Register("NextNavigationButtonTemplate", typeof(DataTemplate), typeof(ScheduleDaysView), new PropertyMetadata(null));
        #endregion

#if !WINRT
        #region AppointmentTooltipVisibility
        /// <summary>
        /// Gets or sets the visibility of appointment's tooltip.
        /// </summary>
        public Visibility AppointmentTooltipVisibility
        {
            get { return (Visibility)GetValue(AppointmentTooltipVisibilityProperty); }
            set { SetValue(AppointmentTooltipVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppoointmentTooltipVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTooltipVisibilityProperty =
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentToolTipTemplate
        /// <summary>
        /// Gets or sets a template for customizing appointment's tooltip.
        /// </summary>
        public ControlTemplate AppointmentToolTipTemplate
        {
            get { return (ControlTemplate)GetValue(AppointmentToolTipTemplateProperty); }
            set { SetValue(AppointmentToolTipTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentToolTipTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentToolTipTemplateProperty =
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleDaysView), new PropertyMetadata(null));
        #endregion
#endif

        #endregion

        #region Read-only Properties

        #region MinorTickVisibility
        /// <summary>
        /// Gets the visibility of minor ticks which represents minute in time slot.
        /// </summary>
        public Visibility MinorTickVisibility
        {
            get { return (Visibility)GetValue(MinorTickVisibilityProperty); }
            internal set { SetValue(MinorTickVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickVisibilityProperty =
            DependencyProperty.Register("MinorTickVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region MajorTickVisibility
        /// <summary>
        /// Gets the visibility of major ticks which represent hour in time slot.
        /// </summary>
        public Visibility MajorTickVisibility
        {
            get { return (Visibility)GetValue(MajorTickVisibilityProperty); }
            internal set { SetValue(MajorTickVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickVisibilityProperty =
            DependencyProperty.Register("MajorTickVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Visible));

        #endregion

        #region IntervalHeight
        /// <summary>
        /// Gets the height of interval in day view.
        /// </summary>
        public double IntervalHeight
        {
            get { return (double)GetValue(IntervalHeightProperty); }
            internal set { SetValue(IntervalHeightProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IntervalHeight.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalHeightProperty =
            DependencyProperty.Register("IntervalHeight", typeof(double), typeof(ScheduleDaysView), new PropertyMetadata(ScheduleTimeLineItemsControl.DefaultIntervalHeight, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleDaysView)
            {
                var scheduleDaysView = d as ScheduleDaysView;
                if (scheduleDaysView.NonAccessibleBlocks != null)
                    scheduleDaysView.SetNonAccessibleBlocks();
            }
        }
        #endregion

        #region TimeMode
        /// <summary>
        /// Gets the time mode of day view which may be 12 hrs or 24 hrs.
        /// </summary>
        public TimeModes TimeMode
        {
            get { return (TimeModes)GetValue(TimeModeProperty); }
            internal set { SetValue(TimeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeModeProperty =
            DependencyProperty.Register("TimeMode", typeof(TimeModes), typeof(ScheduleDaysView), new PropertyMetadata(TimeModes.TwelveHours));
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets the time interval of day view which may differs based on schedule's enum "TimeInterval".
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeInterval"></seealso>
        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)GetValue(TimeIntervalProperty); }
            internal set { SetValue(TimeIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty =
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleDaysView), new PropertyMetadata(TimeInterval.ThirtyMin, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleDaysView)
            {
                var scheduleDaysView = d as ScheduleDaysView;
                if (scheduleDaysView.NonAccessibleBlocks != null)
                    scheduleDaysView.SetNonAccessibleBlocks();
            }
        }

        #endregion

        #region CurrentScheduleType
        /// <summary>
        /// Gets the schedule type of current view. 
        /// </summary>
        public ScheduleType CurrentScheduleType
        {
            get { return (ScheduleType)GetValue(CurrentScheduleTypeProperty); }
            internal set { SetValue(CurrentScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentScheduleType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentScheduleTypeProperty =
            DependencyProperty.Register("CurrentScheduleType", typeof(ScheduleType), typeof(ScheduleDaysView), new PropertyMetadata(ScheduleType.Day));
        #endregion

        #endregion

        #region Internal Properties

        #region ShowAllDay
        internal bool ShowAllDay
        {
            get { return (bool)GetValue(ShowAllDayProperty); }
            set { SetValue(ShowAllDayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowAllDay.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowAllDayProperty =
            DependencyProperty.Register("ShowAllDay", typeof(bool), typeof(ScheduleDaysView), new PropertyMetadata(true));
        #endregion

        #region ResourceCount
        internal int ResourceCount
        {
            get { return (int)GetValue(ResourceCountProperty); }
            set { SetValue(ResourceCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceCount.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ResourceCountProperty =
            DependencyProperty.Register("ResourceCount", typeof(int), typeof(ScheduleDaysView), new PropertyMetadata(1));
        #endregion

        #region AppointmentSelectionBrush
        internal Brush AppointmentSelectionBrush
        {
            get { return (Brush)GetValue(AppointmentSelectionBrushProperty); }
            set { SetValue(AppointmentSelectionBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentSelectionBrush.  This enables animation, styling, binding, etc...
#if WINRT
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#else
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif
        #endregion

        #region AppointmentTemplate
        internal DataTemplate AppointmentTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTemplateProperty); }
            set { SetValue(AppointmentTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppointmentTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty AppointmentTemplateProperty =
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleDaysView), new PropertyMetadata(null));
        #endregion

        #region CurrentTimeIndicatorTemplate
        internal DataTemplate CurrentTimeIndicatorTemplate
        {
            get { return (DataTemplate)GetValue(CurrentTimeIndicatorTemplateProperty); }
            set { SetValue(CurrentTimeIndicatorTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeIndicatorTemplate.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorTemplateProperty =
              DependencyProperty.Register("CurrentTimeIndicatorTemplate", typeof(DataTemplate), typeof(ScheduleDaysView), new PropertyMetadata(null));
        #endregion

        #region CurrentTimeIndicatorMargin
        internal Thickness CurrentTimeIndicatorMargin
        {
            get { return (Thickness)GetValue(CurrentTimeIndicatorMarginProperty); }
            set { SetValue(CurrentTimeIndicatorMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeIndicatorMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorMarginProperty =
              DependencyProperty.Register("CurrentTimeIndicatorMargin", typeof(Thickness), typeof(ScheduleDaysView), new PropertyMetadata(null));
        #endregion

        #region CurrentTimeIndicatorWidth
        internal double CurrentTimeIndicatorWidth
        {
            get { return (double)GetValue(CurrentTimeIndicatorWidthProperty); }
            set { SetValue(CurrentTimeIndicatorWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeIndicatorWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorWidthProperty =
              DependencyProperty.Register("CurrentTimeIndicatorWidth", typeof(double), typeof(ScheduleDaysView), new PropertyMetadata(50d));
        #endregion

        #region CurrentTimeIndicatorVisibility
        internal Visibility CurrentTimeIndicatorVisibility
        {
            get { return (Visibility)GetValue(CurrentTimeIndicatorVisibilityProperty); }
            set { SetValue(CurrentTimeIndicatorVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragDropEndTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorVisibilityProperty =
              DependencyProperty.Register("CurrentTimeIndicatorVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region CurrentDateBackground
        internal Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickStroke
        internal Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickStroke
        internal Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickLabelStroke
        internal Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickLabelStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickLabelStroke
        internal Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickLabelStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region NonWorkingDateCollection
        internal ObservableCollection<DayOfWeek> NonWorkingDateCollection
        {
            get { return (ObservableCollection<DayOfWeek>)GetValue(NonWorkingDateCollectionProperty); }
            set { SetValue(NonWorkingDateCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonWorkingDateCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonWorkingDateCollectionProperty =
            DependencyProperty.Register("NonWorkingDateCollection", typeof(ObservableCollection<DayOfWeek>), typeof(ScheduleDaysView), new PropertyMetadata(null, OnNonWorkingDaysCollectionChanged));

        private static void OnNonWorkingDaysCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduledays = d as ScheduleDaysView;
            if (scheduledays != null && scheduledays.IsTemplateApplied)
                scheduledays.GenerateNonworkingdaysItems();
        }
        #endregion

        #region RectXPosition
        internal double RectXPosition
        {
            get { return (double)GetValue(RectXPositionProperty); }
            set { SetValue(RectXPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectXPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectXPositionProperty =
            DependencyProperty.Register("RectXPosition", typeof(double), typeof(ScheduleDaysView), new PropertyMetadata(0d));
        #endregion

        #region RectYPosition
        internal double RectYPosition
        {
            get { return (double)GetValue(RectYPositionProperty); }
            set { SetValue(RectYPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectXPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectYPositionProperty =
           DependencyProperty.Register("RectYPosition", typeof(double), typeof(ScheduleDaysView), new PropertyMetadata(0d));
        #endregion

        #region RectHeight
        internal double RectHeight
        {
            get { return (double)GetValue(RectHeightProperty); }
            set { SetValue(RectHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectHeightProperty =
            DependencyProperty.Register("RectHeight", typeof(double), typeof(ScheduleDaysView), new PropertyMetadata(0d));
        #endregion

        #region RectWidth
        internal double RectWidth
        {
            get { return (double)GetValue(RectWidthProperty); }
            set { SetValue(RectWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectWidthProperty =
            DependencyProperty.Register("RectWidth", typeof(double), typeof(ScheduleDaysView), new PropertyMetadata(0d));
        #endregion

        #region RectVisibility
        internal Visibility RectVisibility
        {
            get { return (Visibility)GetValue(RectVisibilityProperty); }
            set { SetValue(RectVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectVisibilityProperty =
            DependencyProperty.Register("RectVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

#if !WINRT

        public Visibility AppTextVisibility
        {
            get { return (Visibility)GetValue(AppTextVisibilityProperty); }
            set { SetValue(AppTextVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppTextVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AppTextVisibilityProperty =
            DependencyProperty.Register("AppTextVisibility", typeof(Visibility), typeof(ScheduleDaysView), new PropertyMetadata(Visibility.Collapsed));


#endif
        #region ResourceHeaderRow
        internal int ResourceHeaderRow
        {
            get { return (int)GetValue(ResourceHeaderRowProperty); }
            set { SetValue(ResourceHeaderRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ResourceHeaderRow.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ResourceHeaderRowProperty =
            DependencyProperty.Register("ResourceHeaderRow", typeof(int), typeof(ScheduleDaysView), new PropertyMetadata(0));
        #endregion

        #region DayHeaderRow
        internal int DayHeaderRow
        {
            get { return (int)GetValue(DayHeaderRowProperty); }
            set { SetValue(DayHeaderRowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayHeaderRow.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DayHeaderRowProperty =
            DependencyProperty.Register("DayHeaderRow", typeof(int), typeof(ScheduleDaysView), new PropertyMetadata(1));
        #endregion

        #region DayViewVerticaLineStroke
        internal Brush DayViewVerticaLineStroke
        {
            get { return (Brush)GetValue(DayViewVerticaLineStrokeProperty); }
            set { SetValue(DayViewVerticaLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewVerticaLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        internal static readonly DependencyProperty DayViewVerticaLineStrokeProperty =
            DependencyProperty.Register("DayViewVerticaLineStroke", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MajorTickStrokeDashArray
        internal DoubleCollection MajorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MajorTickStrokeDashArrayProperty); }
            set { SetValue(MajorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleDaysView), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region MinorTickStrokeDashArray
        internal DoubleCollection MinorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MinorTickStrokeDashArrayProperty); }
            set { SetValue(MinorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleDaysView), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region DayHeaderOrder
        internal DayHeaderOrder DayHeaderOrder
        {
            get { return (DayHeaderOrder)GetValue(DayHeaderOrderProperty); }
            set { SetValue(DayHeaderOrderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayHeaderOrder.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DayHeaderOrderProperty =
            DependencyProperty.Register("DayHeaderOrder", typeof(DayHeaderOrder), typeof(ScheduleDaysView), new PropertyMetadata(DayHeaderOrder.OrderByResource, OnDayResourceViewChanged));

        private static void OnDayResourceViewChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var dayview = dpo as ScheduleDaysView;
            if (dayview != null && (dayview.IsTemplateApplied && arg.OldValue != null && ((DayHeaderOrder)arg.OldValue) != dayview.DayHeaderOrder))
            {
                dayview.SetupAppointments();
            }
        }
        #endregion

        #region ScheduleResourceType
        internal ResourceType ScheduleResourceType
        {
            get { return (ResourceType)GetValue(ScheduleResourceTypeProperty); }
            set { SetValue(ScheduleResourceTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ScheduleResourceType.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ScheduleResourceTypeProperty =
            DependencyProperty.Register("ScheduleResourceType", typeof(ResourceType), typeof(ScheduleDaysView), new PropertyMetadata(null, OnTypeChanged));

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dv = d as ScheduleDaysView;
            if (dv != null) dv.SetupAppointments();
        }
        #endregion

        #region DayViewColumnCount
        internal int DayViewColumnCount
        {
            get { return (int)GetValue(DayViewColumnCountProperty); }
            set { SetValue(DayViewColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayViewColumnCount.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty DayViewColumnCountProperty =
            DependencyProperty.Register("DayViewColumnCount", typeof(int), typeof(ScheduleDaysView), new PropertyMetadata(0, OnDayViewColumnCountChanged));

        private static void OnDayViewColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleDaysView)
            {
                ScheduleDaysView daysView = d as ScheduleDaysView;
                if (daysView.CurrentScheduleType == ScheduleType.Day)
                {
                    daysView.RectWidth = daysView.RectHeight = 0;
                }
            }
        }
        #endregion

        #region IsTimeIntervalChanged
        internal bool IsTimeIntervalChanged
        {
            get { return (bool)GetValue(IsTimeIntervalChangedProperty); }
            set { SetValue(IsTimeIntervalChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTimeIntervalChanged.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsTimeIntervalChangedProperty =
            DependencyProperty.Register("IsTimeIntervalChanged", typeof(bool), typeof(ScheduleDaysView), new PropertyMetadata(false, OnIsTimeIntervalChanged));

        private static void OnIsTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var days = d as ScheduleDaysView;
            if (days != null && days.IsTemplateApplied)
                days.GenerateNonworkingdaysItems();
        }
        #endregion

        #region HeaderBackground
        internal Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { SetValue(HeaderBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region NonWorkingHourBrush
        internal Brush NonWorkingHourBrush
        {
            get { return (Brush)GetValue(NonWorkingHourBrushProperty); }
            set { SetValue(NonWorkingHourBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonWorkingHourBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonWorkingHourBrushProperty =
            DependencyProperty.Register("NonWorkingHourBrush", typeof(Brush), typeof(ScheduleDaysView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region IsHighLightWorkingHours
        internal bool IsHighLightWorkingHours
        {
            get { return (bool)GetValue(IsHighLightWorkingHoursProperty); }
            set { SetValue(IsHighLightWorkingHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHightLightWorkingHours.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsHighLightWorkingHoursProperty =
            DependencyProperty.Register("IsHighLightWorkingHours", typeof(bool), typeof(ScheduleDaysView), new PropertyMetadata(false, OnIsHighLightWorkingHoursChanged));

        private static void OnIsHighLightWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var days = d as ScheduleDaysView;
            if (days != null && days.IsTemplateApplied)
                days.GenerateNonworkingdaysItems();
        }
        #endregion

        #region ShowNonWorkingHours
        internal bool ShowNonWorkingHours
        {
            get { return (bool)GetValue(ShowNonWorkingHoursProperty); }
            set { SetValue(ShowNonWorkingHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNonWorkingHours.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowNonWorkingHoursProperty =
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(ScheduleDaysView), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleDaysView)
            {
                ScheduleDaysView scheduleDaysView = d as ScheduleDaysView;
                scheduleDaysView.GenerateNonworkingdaysItems();
                scheduleDaysView.SetNavigationTapVisibility();
                scheduleDaysView.SetupAppointments();
                if (scheduleDaysView.CurrentTimeIndicatorTemplate != null && scheduleDaysView.CurrentTimeIndicatorVisibility == Visibility.Visible)
                    scheduleDaysView.TodayTimer.Interval = new TimeSpan(0, 0, 1);
            }
        }
        #endregion

        #region WorkStartHour
        internal int WorkStartHour
        {
            get { return (int)GetValue(WorkStartHourProperty); }
            set { SetValue(WorkStartHourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkStartHour.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WorkStartHourProperty =
            DependencyProperty.Register("WorkStartHour", typeof(int), typeof(ScheduleDaysView), new PropertyMetadata(9, OnWorkingHoursChanged));
        #endregion

        #region WorkEndHour
        internal int WorkEndHour
        {
            get { return (int)GetValue(WorkEndHourProperty); }
            set { SetValue(WorkEndHourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkEndHour.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WorkEndHourProperty =
            DependencyProperty.Register("WorkEndHour", typeof(int), typeof(ScheduleDaysView), new PropertyMetadata(18, OnWorkingHoursChanged));

        private static void OnWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleDaysView)
            {
                var scheduleDaysView = d as ScheduleDaysView;
                scheduleDaysView.SetNavigationTapVisibility();
                if (!scheduleDaysView.ShowNonWorkingHours && scheduleDaysView.CurrentTimeIndicatorVisibility == Visibility.Visible)
                    scheduleDaysView.TodayTimer.Interval = new TimeSpan(0, 0, 1);
            }
        }
        #endregion

        #region NonAccessibleBlocks
        internal NonAccessibleBlockCollection NonAccessibleBlocks
        {
            get { return (NonAccessibleBlockCollection)GetValue(NonAccessibleBlocksProperty); }
            set { SetValue(NonAccessibleBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonAccessibleBlocks.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonAccessibleBlocksProperty =
            DependencyProperty.Register("NonAccessibleBlocks", typeof(NonAccessibleBlockCollection), typeof(ScheduleDaysView), new PropertyMetadata(null, OnNonAccessibleBlocksChanged));

        private static void OnNonAccessibleBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleDaysView)
            {
                ScheduleDaysView daysView = d as ScheduleDaysView;
                if (daysView.IsTemplateApplied)
                    daysView.SetNonAccessibleBlocks();
            }
        }
        #endregion

        #endregion

        #endregion

        #region Methods

        #region Appointments population

        internal void GenerateHeaderItems()
        {
            if (resourcecontainer.Items != null)
            {
                resourcecontainer.Items.Clear();
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
#if WINRT
                    scrollviewer.HorizontalScrollMode = ScrollMode.Enabled;
                    headerscrollviewer.HorizontalScrollMode = ScrollMode.Enabled;
#endif
                    scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                    headerscrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    double resourcecount = schedule.ScheduleResourceType.ResourceCollection.Count;
                    ResourceCount = (int)resourcecount;
                    List<ObservableCollection<Resource>> rescoll = GenerateHeaderGroups(schedule.ScheduleResourceType);
                    if (schedule.DayHeaderOrder == DayHeaderOrder.OrderByDate)
                    {
                        ResourceHeaderRow = 1;
                        DayHeaderRow = 0;
                        double daycount = SelectedDates.Count;
                        int level = 1;
                        foreach (ObservableCollection<Resource> res in rescoll)
                        {
                            resourcecount = res.Count;
                            var headeritemscontrol = new ResourceHeaderItemsControl();
                            if (headeritemscontrol.Items != null)
                            {
                                for (int days = 0; days < daycount * level; days++)
                                {
                                    for (int k = 0; k < resourcecount; k++)
                                    {
                                        headeritemscontrol.Items.Add(new DayViewItemHeader { DataContext = res[k] });
                                    }
                                }
                            }
                            resourcecontainer.Items.Add(headeritemscontrol);
                            level = level * (int)resourcecount;
                        }
                    }
                    else
                    {
                        ResourceHeaderRow = 0;
                        DayHeaderRow = 1;
                        int level = 1;
                        foreach (ObservableCollection<Resource> res in rescoll)
                        {
                            resourcecount = res.Count;
                            var headeritemscontrol = new ResourceHeaderItemsControl();
                            if (headeritemscontrol.Items != null)
                            {
                                for (int i = 0; i < level; i++)
                                {
                                    for (int k = 0; k < resourcecount; k++)
                                    {
                                        headeritemscontrol.Items.Add(new DayViewItemHeader { DataContext = res[k] });
                                    }
                                }
                            }
                            resourcecontainer.Items.Add(headeritemscontrol);
                            level = level * (int)resourcecount;
                        }
                    }
                }
                else
                {
                    ResourceCount = 1;
#if WINRT
                    scrollviewer.HorizontalScrollMode = ScrollMode.Disabled;
                    headerscrollviewer.HorizontalScrollMode = ScrollMode.Disabled;
#endif
                    scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                    if (headerscrollviewer != null)
                        headerscrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                }
            }
        }

        #endregion

        #region GetResourceCombinationColl

        List<List<string>> GetResourceCombinationColl(ResourceType restype)
        {
            int leafchild = 1;
            int resourceLevel = 0;
            ResourceType tempresotype = restype;
            var resourceCombColl = new List<List<string>>();
            while (tempresotype != null)
            {
                leafchild = leafchild * tempresotype.ResourceCollection.Count;
                tempresotype = tempresotype.SubResourceType;
                resourceLevel += 1;
            }
            schedule = this.FindParentElementOfType<SfSchedule>();
            List<ResourceType> resourceList = schedule.FindResourceList(restype);
            for (int i = 0; i < leafchild; i++)
            {
                var resourceNameColl = new List<string>();
                int level = resourceLevel;
                int resPos = i;
                while (level != 0)
                {
                    int resIndex = resPos % resourceList[level - 1].ResourceCollection.Count;
                    resourceNameColl.Add(resourceList[level - 1].ResourceCollection[resIndex].ResourceName);
                    resPos = resPos / resourceList[level - 1].ResourceCollection.Count;
                    level--;
                }
                resourceCombColl.Add(resourceNameColl);
            }
            return resourceCombColl;
        }

        #endregion

        #region GenerateNonWorkingdaysItems

        void GenerateNonworkingdaysItems()
        {
            if (nonworkingdaysLayout != null && IsHighLightWorkingHours && ShowNonWorkingHours)
            {
                var grid1 = new Grid();
                var grid2 = new Grid();
                if (nonworkingdaysLayout.Items != null) nonworkingdaysLayout.Items.Clear();
                int k = 0;
                var selecteddates = new ObservableCollection<DateTime>(SelectedDates.OrderBy(s => s));
                double dayscount = selecteddates.Count;
                var backgroundBinding = new Binding { Source = this, Path = new PropertyPath("NonWorkingHourBrush") };
                grid1.SetBinding(Panel.BackgroundProperty, backgroundBinding);
                grid2.SetBinding(Panel.BackgroundProperty, backgroundBinding);
                int resourcecount = 1;
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0 && schedule.DayHeaderOrder == DayHeaderOrder.OrderByResource)
                {
                    resourcecount = schedule.CalculateLeafCount(schedule.ScheduleResourceType);
                }
                for (int res = 0; res < resourcecount; res++)
                {
                    for (int i = 0; i < dayscount; i++)
                    {
                        var grid = new Grid();
                        grid.SetBinding(Panel.BackgroundProperty, backgroundBinding);
                        DateTime currentdate = selecteddates[i].Date;
                        if (NonWorkingDateCollection != null)
                        {
                            if (NonWorkingDateCollection.Contains(currentdate.DayOfWeek))
                            {
                                k = 1;
                                grid.DataContext = currentdate.DayOfWeek.ToString();
                                if (nonworkingdaysLayout.Items != null)
                                    nonworkingdaysLayout.Items.Add(grid);
                            }
                        }

                    }
                }
                if (k == 0 && nonworkingdaysLayout.Items != null)
                {
                    nonworkingdaysLayout.Items.Clear();
                }
                if (nonworkingdaysLayout.Items != null)
                {
                    nonworkingdaysLayout.Items.Add(grid1);
                    nonworkingdaysLayout.Items.Add(grid2);
                }
            }
            else if (nonworkingdaysLayout != null && nonworkingdaysLayout.Items != null)
            {
                nonworkingdaysLayout.Items.Clear();
            }
        }

        #endregion

        #region Set NonAccessible Blocks

        internal void SetNonAccessibleBlocks()
        {
            if (NonAccessibleBlocks != null)
            {
                var nonAccessibleBlockTemplateBinding = new Binding { Source = schedule, Path = new PropertyPath("NonAccessibleBlockTemplate") };
                foreach (NonAccessibleBlock nonAccessibleBlock in NonAccessibleBlocks)
                {
                    BindingOperations.SetBinding(nonAccessibleBlock, NonAccessibleBlock.CustomTemplateProperty, nonAccessibleBlockTemplateBinding);
                    double startHour = nonAccessibleBlock.StartHour > ScheduleTimeLineItemsControl.MinValue ? nonAccessibleBlock.StartHour : ScheduleTimeLineItemsControl.MinValue;
                    double top = GetTimePosition(startHour);
                    nonAccessibleBlock.Margin = new Thickness(0, top, 0, 0);
                    double endHour = nonAccessibleBlock.EndHour < ScheduleTimeLineItemsControl.MaxValue ? nonAccessibleBlock.EndHour : ScheduleTimeLineItemsControl.MaxValue;
                    endHour = endHour > startHour ? endHour : startHour;
                    double height = GetTimePosition(endHour) - top;
                    if (scheduleHorizontalTimeSlotItemsControl != null && (height + top) > scheduleHorizontalTimeSlotItemsControl.Height)
                        height = scheduleHorizontalTimeSlotItemsControl.Height - top;
                    double tempHeight = height >= 0 ? height : 0;
                    nonAccessibleBlock.Size = tempHeight + 1; // 1 is added for managing schedule's line stroke thickness
                }
            }
        }

        private double GetTimePosition(double hour)
        {
            return (hour - ScheduleTimeLineItemsControl.MinValue) * IntervalHeight * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
        }

        #endregion

        internal List<ObservableCollection<Resource>> GenerateHeaderGroups(ResourceType restype)
        {
            var returncollection = new List<ObservableCollection<Resource>> { restype.ResourceCollection };
            ResourceType temprestype = restype.SubResourceType;
            while (temprestype != null)
            {
                returncollection.Add(temprestype.ResourceCollection);
                temprestype = temprestype.SubResourceType;
            }
            return returncollection;
        }

        internal bool resourceCheck(ScheduleAppointment app, List<string> resCombColl)
        {
            int count = app.ResourceCollection.Count(res => resCombColl.Contains(res.ResourceName));
            if (count == resCombColl.Count)
                return true;
            return false;
        }

#if WINRT
        internal void SetupAppointments()
#else
        internal void SetupAppointments()
#endif
        {
            childstringcollection = new List<List<string>>();
            if (alldaysAppointmentsLayout != null && daysAppointmentsLayout != null)
            {
                schedule = this.FindParentElementOfType<SfSchedule>();
                ClearAllDaysAppointments();
                ClearAppointments();
                GenerateHeaderItems();
#if WINRT
                //if (schedule != null && schedule.VisibleDateChanged)
                //    await Task.Delay(800);
#endif
            }
            if (alldaysAppointmentsLayout != null && daysAppointmentsLayout != null && VisibleAppointments != null)
            {
                var selecteddates = new ObservableCollection<DateTime>(SelectedDates.OrderBy(s => s));
#if WINRT
                startspan = scrollviewer.ViewportHeight != scrollviewer.ExtentHeight ? FindStartspan(scrollviewer.VerticalOffset) : new TimeSpan(ScheduleTimeLineItemsControl.MinValue, 0, 0);
                endspan = scrollviewer.ViewportHeight != scrollviewer.ExtentHeight ? FindEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight) : new TimeSpan(ScheduleTimeLineItemsControl.MaxValue, 0, 0);
#else
                startspan = Math.Round(scrollviewer.ViewportHeight, 5) != scrollviewer.ExtentHeight ? FindStartspan(scrollviewer.VerticalOffset) : new TimeSpan(ScheduleTimeLineItemsControl.MinValue, 0, 0);
                endspan = Math.Round(scrollviewer.ViewportHeight, 5) != scrollviewer.ExtentHeight ? FindEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight) : new TimeSpan(ScheduleTimeLineItemsControl.MaxValue, 0, 0);
#endif
                double dayscount = selecteddates.Count;
                var arrcollection = new ScheduleAppointment[VisibleAppointments.Count];
                List<ScheduleAppointment> tempcollection;
                VisibleAppointments.CopyTo(arrcollection, 0);

                var appointmentSelectionBrushBinding = new Binding { Path = new PropertyPath("AppointmentSelectionBrush"), Source = this };
                var appointmentTemplateBinding = new Binding { Path = new PropertyPath("AppointmentTemplate"), Source = this };
                var dayviewVerticalLineStrokeBinding = new Binding { Path = new PropertyPath("DayViewVerticaLineStroke"), Source = this };
#if !WINRT
                var appointmentTooltipVisibilityBinding = new Binding { Path = new PropertyPath("AppointmentTooltipVisibility"), Source = this };
                var appointmentToolTipTemplateBinding = new Binding { Path = new PropertyPath("AppointmentToolTipTemplate"), Source = this };
#endif
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    tempcollection = arrcollection.OrderBy(app => app.InternalStartTime).ToList();
                    ResourceType restype = schedule.ScheduleResourceType;
                    string typename = schedule.Resource;
                    while (restype != null)
                    {
                        var resourcenamecoll = restype.ResourceCollection.Select(resrc => resrc.ResourceName).ToList();
                        tempcollection = (from app in tempcollection where (app.ResourceCollection.FirstOrDefault(res => (res.TypeName == typename && resourcenamecoll.Contains(res.ResourceName))) != null) select app).ToList();
                        restype = restype.SubResourceType;
                        if (restype != null)
                            typename = restype.TypeName;
                    }
                    for (int i = 0; i < dayscount; i++)
                    {
                        DateTime currentdate = selecteddates[i].Date;
                        foreach (ScheduleAppointment app in (from app in tempcollection where (!app.AllDay && (app.InternalStartTime.Date == currentdate || (app.InternalStartTime < currentdate && app.InternalEndTime > currentdate)) && ((app.InternalStartTime.TimeOfDay > startspan && app.InternalStartTime.TimeOfDay < endspan) || (app.InternalEndTime.TimeOfDay < endspan && app.InternalEndTime.TimeOfDay > startspan) || (app.InternalStartTime.TimeOfDay < startspan && app.InternalEndTime.TimeOfDay > endspan))) select app).ToList())
                        {
                            if (daysAppointmentsLayout.Items != null) daysAppointmentsLayout.Items.Add(app);
                            tempcollection.Remove(app);
                            app.PropertyChanged += app_PropertyChanged;
                        }
                    }
                    childstringcollection = GetResourceCombinationColl(schedule.ScheduleResourceType);
                    if (schedule.DayHeaderOrder == DayHeaderOrder.OrderByResource)
                    {
                        foreach (var leafList in childstringcollection)
                        {
                            for (var index = 0; index < dayscount; index++)
                            {
                                var currentdate = selecteddates[index].Date;
                                var appointmentCollectionView = new AllDayAppointmentItemscontrol();
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentSelectionBrushProperty, appointmentSelectionBrushBinding);
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentTemplateProperty, appointmentTemplateBinding);
                                BindingOperations.SetBinding(appointmentCollectionView, AllDayAppointmentItemscontrol.DayViewVerticaLineStrokeProperty, dayviewVerticalLineStrokeBinding);
#if !WINRT
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
#endif
                                var tempchilcoll = new List<ScheduleAppointment>();
                                foreach (ScheduleAppointment app in (from app in tempcollection where (((app.InternalStartTime.Date == currentdate && app.AllDay) || (app.InternalStartTime < currentdate && app.InternalEndTime > currentdate)) && app.AllDay && resourceCheck(app, leafList)) select app).ToList())
                                {
                                    if (!tempchilcoll.Contains(app))
                                    {
                                        tempchilcoll.Add(app);
                                        tempcollection.Remove(app);
                                        app.PropertyChanged += app_PropertyChanged;
                                    }
                                }
                                appointmentCollectionView.ChildCollection = tempchilcoll;
                                //appointmentCollectionView.BorderThickness = new Thickness(3, 0, 0, 0);
                                if (alldaysAppointmentsLayout.Items != null)
                                    alldaysAppointmentsLayout.Items.Add(appointmentCollectionView);
                            }
                        }
                    }
                    else
                    {
                        for (int index = 0; index < dayscount; index++)
                        {

                            foreach (List<string> child in childstringcollection)
                            {
                                DateTime currentdate = selecteddates[index].Date;
                                var appointmentCollectionView = new AllDayAppointmentItemscontrol();
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentSelectionBrushProperty, appointmentSelectionBrushBinding);
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentTemplateProperty, appointmentTemplateBinding);
                                BindingOperations.SetBinding(appointmentCollectionView, AllDayAppointmentItemscontrol.DayViewVerticaLineStrokeProperty, dayviewVerticalLineStrokeBinding);
#if !WINRT
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
                                appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
#endif
                                var tempchilcoll = new List<ScheduleAppointment>();

                                foreach (ScheduleAppointment app in (from app in tempcollection where (((app.InternalStartTime.Date == currentdate && app.AllDay) || (app.InternalStartTime < currentdate && app.InternalEndTime > currentdate)) && app.AllDay && resourceCheck(app, child)) select app).ToList())
                                {
                                    if (!tempchilcoll.Contains(app))
                                    {
                                        tempchilcoll.Add(app);
                                        tempcollection.Remove(app);
                                        app.PropertyChanged += app_PropertyChanged;
                                    }
                                }
                                appointmentCollectionView.ChildCollection = tempchilcoll;
                                appointmentCollectionView.BorderThickness = new Thickness(3, 0, 0, 0);
                                if (alldaysAppointmentsLayout.Items != null)
                                    alldaysAppointmentsLayout.Items.Add(appointmentCollectionView);
                            }
                        }
                    }
                }
                else
                {
                    tempcollection = arrcollection.OrderBy(app => app.InternalStartTime).ToList();
                    for (int i = 0; i < dayscount; i++)
                    {
                        DateTime currentdate = selecteddates[i].Date;
                        var appointmentCollectionView = new AllDayAppointmentItemscontrol();
                        appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentSelectionBrushProperty, appointmentSelectionBrushBinding);
                        appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentTemplateProperty, appointmentTemplateBinding);
                        BindingOperations.SetBinding(appointmentCollectionView, AllDayAppointmentItemscontrol.DayViewVerticaLineStrokeProperty, dayviewVerticalLineStrokeBinding);
#if !WINRT
                        appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
                        appointmentCollectionView.SetBinding(AllDayAppointmentItemscontrol.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
#endif
                        var tempchilcoll = new List<ScheduleAppointment>();
                        foreach (ScheduleAppointment app in (from app in tempcollection where (((app.InternalStartTime.Date == currentdate && app.AllDay) || (app.InternalStartTime < currentdate && app.InternalEndTime > currentdate)) && app.AllDay) select app).ToList())
                        {
                            if (!tempchilcoll.Contains(app))
                            {
                                tempchilcoll.Add(app);
                                tempcollection.Remove(app);
                                app.PropertyChanged += app_PropertyChanged;
                            }
                        }
                        appointmentCollectionView.ChildCollection = tempchilcoll;
                        appointmentCollectionView.BorderThickness = new Thickness(3, 0, 0, 0);
                        if (alldaysAppointmentsLayout.Items != null)
                        {
                            alldaysAppointmentsLayout.Items.Add(appointmentCollectionView);
                        }
                        foreach (ScheduleAppointment app in (from app in tempcollection where (!app.AllDay && (app.InternalStartTime.Date == currentdate || 
                                                                 (app.InternalStartTime < currentdate && app.InternalEndTime > currentdate)) && 
                                                                 ((app.InternalStartTime.TimeOfDay >= startspan && app.InternalStartTime.TimeOfDay <= endspan) || 
                                                                 (app.InternalEndTime.TimeOfDay <= endspan && app.InternalEndTime.TimeOfDay >= startspan) || 
                                                                 (app.InternalStartTime.TimeOfDay <= startspan && app.InternalEndTime.TimeOfDay >= endspan))) select app).ToList())
                        {
                            if (daysAppointmentsLayout.Items != null) daysAppointmentsLayout.Items.Add(app);
                            tempcollection.Remove(app);
                            app.PropertyChanged += app_PropertyChanged;
                        }

                    }
                }
            }
            GenerateNonworkingdaysItems();
            var selectedDates = new ObservableCollection<DateTime>(SelectedDates.OrderBy(s => s));
            if (schedule != null && schedule.CurrentTimeIndicatorVisibility == Visibility.Visible)
            {
                if (selectedDates.Contains(DateTime.Now.Date))
                {
                    TodayTimer.Start();
                }
                else
                {
#if WINRT
                    CurrentTimeIndicatorVisibility = Visibility.Collapsed;
#else
                    CurrentTimeIndicatorVisibility = Visibility.Collapsed;
#endif
                    TodayTimer.Stop();
                }
            }
        }

        internal void SetWorkingHoursPosition()
        {
            double intervalPerHour = 60 / schedule.GetTimeInterval().TotalMinutes;
            double offset = schedule.WorkStartHour * schedule.IntervalHeight * intervalPerHour;
#if SyncfusionFramework4_5_11 && WINRT
            scrollviewer.ChangeView(null, offset, null);
#else
            scrollviewer.ScrollToVerticalOffset(offset);
#endif
        }

        #region Clear Appointments

        private void ClearAllDaysAppointments()
        {

            if (alldaysAppointmentsLayout.Items != null)
            {
                foreach (AllDayAppointmentItemscontrol childitem in alldaysAppointmentsLayout.Items)
                {
                    if (childitem.Items != null)
                        childitem.Items.Clear();
                }
                alldaysAppointmentsLayout.Items.Clear();
            }
        }

        private void ClearAppointments()
        {
            if (daysAppointmentsLayout.Items != null)
                daysAppointmentsLayout.Items.Clear();
        }

        #endregion

        #region Find Start Span

        private TimeSpan FindStartspan(double height)
        {
            if (height > IntervalHeight)
            {
                double startingInterval = Math.Ceiling(height / IntervalHeight);
                double numberOfIntervalsperhour = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                double totalmin = startingInterval * (60 / numberOfIntervalsperhour);
                var startingspan = new TimeSpan(ScheduleTimeLineItemsControl.MinValue, 0, 0) + new TimeSpan(0, (int)totalmin, 0);
                return startingspan;
            }
            return new TimeSpan(ScheduleTimeLineItemsControl.MinValue, 0, 0);
        }

        #endregion

        #region Find End Span

        private TimeSpan FindEndspan(double height)
        {
            if (height > IntervalHeight)
            {
                //Adding five to load additional time to improve scrolling performance.
                double endingInterval = Math.Floor(height / IntervalHeight) + Math.Floor(height / IntervalHeight) / 2;
                double numberOfIntervalsperhour = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                double totalmin = endingInterval * (60 / numberOfIntervalsperhour);
                var endingspan = new TimeSpan(ScheduleTimeLineItemsControl.MinValue, 0, 0) + new TimeSpan(0, (int)totalmin, 0);
                int timediff = ScheduleTimeLineItemsControl.MinValue + (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
                if (endingspan.Hours > timediff - 1)
                {
                    endingspan = new TimeSpan(timediff, 0, 0);
                }
                return endingspan;
            }
            return new TimeSpan(0, 0, 0);
        }

        #endregion

        #region Find Exact EndSpan

        private TimeSpan FindExactEndspan(double height)
        {
            if (height > IntervalHeight)
            {
                double endingInterval = Math.Floor(height / IntervalHeight) + Math.Floor(height / IntervalHeight) / 3;
                double numberOfIntervalsperhour = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                double totalmin = endingInterval * (60 / numberOfIntervalsperhour);
                var endingspan = new TimeSpan(0, (int)totalmin, 0);
                return endingspan;
            }
            return new TimeSpan(0, 0, 0);
        }

        #endregion

        #region Getting Current Selected Date

        internal DateTime GetSelectedDate(Point position, bool isDroppedDate)
        {
            var selectedDate = new DateTime();
            if (schedule != null)
            {
                schedule.SelectedPoint = position;
                double hrHeight = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualHeight / (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
                var timespan = (int)schedule.GetTimeInterval().TotalMinutes;
                double total = (position.Y / hrHeight);
                int min = 0;
                double time1;
                if (hrHeight == 0)
                {
                    hrHeight = (60 / timespan) * schedule.IntervalHeight;
                }
                if (isDroppedDate)
                {
                    time1 = (int)total + ScheduleTimeLineItemsControl.MinValue;
                    double decimalvalue = (total - (int)total) * 60;
                    if (timespan != 0)
                    {
                        min = (int)decimalvalue;// ((int)decimalvalue / timespan) * timespan;
                    }
                }
                else
                {
                    time1 = Math.Floor(total) + ScheduleTimeLineItemsControl.MinValue;
                    double decimalvalue = Math.Round(total - (int)total, 2) * 60;
                    if (timespan != 0)
                    {
                        min = ((int)decimalvalue / timespan) * timespan;
                    }
                }
                double daysCount = SelectedDates.Count;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    daysCount = daysCount * schedule.CalculateLeafCount(ScheduleResourceType);
                }
                double dayWidth = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualWidth / daysCount;
                var dayIndex = (int)Math.Floor(position.X / dayWidth);
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    dayIndex = GetSelectedResource(dayIndex);
                }
                else
                {
                    if (dayIndex > SelectedDates.Count - 1)
                    {
                        dayIndex = SelectedDates.Count - 1;
                    }
                    else if (dayIndex < 0)
                    {
                        dayIndex = 0;
                    }
                }
                selectedDate = SelectedDates.Count > 0 ? SelectedDates[dayIndex] : new DateTime();
                if (!isDroppedDate)
                {
                    selectedDate = selectedDate.AddHours(time1 >= 0 ? time1 : 0).AddMinutes(min);
                }
                else if (total > 0)
                {
                    selectedDate = selectedDate.AddHours(time1 >= 0 ? time1 : 0).AddMinutes(min);
                    schedule.allDayFlag = false;
                }
                else if (position.Y < 0)
                    schedule.allDayFlag = true;
            }
            return selectedDate;
        }

        #endregion

        #region Get selected resource

        internal int GetSelectedResource(int dayIndex)
        {
            int resourcecount = schedule.CalculateLeafCount(ScheduleResourceType);
            List<ResourceType> types = schedule.FindResourceList(ScheduleResourceType);
            if (DayHeaderOrder == DayHeaderOrder.OrderByDate)
            {
                schedule.selectedResourcename = new List<Resource>();
                int level = types.Count;
                ResourceType maintype = types[level - 1];
                int index = (dayIndex) % maintype.ResourceCollection.Count;
                int parentindex = (dayIndex) / maintype.ResourceCollection.Count;
                while (maintype != null && level > 0)
                {
                    var selectionresourse = new Resource();
                    index = index < maintype.ResourceCollection.Count ? index < 0 ? 0 : index : maintype.ResourceCollection.Count - 1;
                    selectionresourse.ResourceName = maintype.ResourceCollection.ElementAt(index).ResourceName;
                    selectionresourse.TypeName = maintype.TypeName;
                    schedule.selectedResourcename.Add(selectionresourse);
                    level--;
                    if (level != 0)
                    {
                        maintype = types[level - 1];
                        index = parentindex % maintype.ResourceCollection.Count;
                        parentindex = parentindex / maintype.ResourceCollection.Count;
                    }
                    else
                    {
                        maintype = null;
                    }
                }
                schedule.selectedResourcename.Reverse();
                dayIndex = dayIndex / resourcecount;
            }
            else
            {
                schedule.selectedResourcename = new List<Resource>();
                int level = types.Count;
                ResourceType maintype = types[level - 1];
                int index = (dayIndex / SelectedDates.Count) % maintype.ResourceCollection.Count;
                int parentindex = (dayIndex / SelectedDates.Count) / maintype.ResourceCollection.Count;
                while (maintype != null && level > 0)
                {
                    var selectionresourse = new Resource();
                    index = index < maintype.ResourceCollection.Count ? index < 0 ? 0 : index : maintype.ResourceCollection.Count - 1;
                    selectionresourse.ResourceName = maintype.ResourceCollection.ElementAt(index).ResourceName;
                    selectionresourse.TypeName = maintype.TypeName;
                    schedule.selectedResourcename.Add(selectionresourse);
                    level--;
                    if (level != 0)
                    {
                        maintype = types[level - 1];
                        index = parentindex % maintype.ResourceCollection.Count;
                        parentindex = parentindex / maintype.ResourceCollection.Count;
                    }
                    else
                    {
                        maintype = null;
                    }
                }
                schedule.selectedResourcename.Reverse();
                if (dayIndex > SelectedDates.Count - 1 && resourcecount > 1)
                {
                    dayIndex = (dayIndex % SelectedDates.Count);
                }
            }
            if (dayIndex > (SelectedDates.Count) - 1)
            {
                dayIndex = (SelectedDates.Count) - 1;
            }
            else if (dayIndex < 0)
            {
                dayIndex = 0;
            }
            return dayIndex;
        }

        #endregion

        #region Updating Selection Region

        internal void UpdateSelection(bool isTapped)
        {
            if (schedule == null)
                schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule != null && IsTemplateApplied)
            {
                var timeslotWidth = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualWidth;
                var timeslotHeight = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualHeight;
                if (scheduleHorizontalTimeSlotItemsControl != null)
                {
                    timeslotHeight = scheduleHorizontalTimeSlotItemsControl.Height;
                }
                if (!isTapped && (schedule.ScheduleType == ScheduleType.Week || schedule.ScheduleType == ScheduleType.WorkWeek) && schedule.CurrentSelectedDates.Contains(schedule.SelectedDate.Date))
                {
                    var width = timeslotWidth / schedule.CurrentSelectedDates.Count;
                    double prevIndex = 0;
                    int leafcount = 1;
                    if (ScheduleResourceType != null && schedule.ScheduleResourceTypeCollection.Count > 0)
                    {
                        var tempres = ScheduleResourceType;
                        while (tempres != null)
                        {
                            if (tempres.ResourceCollection.Count > 0)
                            {
                                leafcount *= tempres.ResourceCollection.Count;
                            }
                            tempres = tempres.SubResourceType;
                        }
                        int level = 0;
                        if (schedule.selectedResourcename.Count > 0)
                        {
                            var topResource = ScheduleResourceType.ResourceCollection.FirstOrDefault(x => x.ResourceName == schedule.selectedResourcename[level].ResourceName);
                            prevIndex = ScheduleResourceType.ResourceCollection.IndexOf(topResource);
                            var res = ScheduleResourceType.SubResourceType;
                            while (res != null)
                            {
                                level++;
                                if (res.ResourceCollection.Count > 0)
                                {
                                    var curres = res.ResourceCollection.FirstOrDefault(x => x.ResourceName == schedule.selectedResourcename[level].ResourceName);
                                    int levelIndex = res.ResourceCollection.IndexOf(curres);
                                    prevIndex = (prevIndex * res.ResourceCollection.Count) + levelIndex;
                                }
                                res = res.SubResourceType;
                            }
                        }
                    }
                    if (ScheduleResourceType != null && schedule.ScheduleResourceTypeCollection.Count > 0)
                    {
                        width = width / leafcount;
                        schedule.SelectedPoint.X = ((schedule.CurrentSelectedDates.IndexOf(schedule.SelectedDate.Date) + 1) * width * prevIndex) + 5;
                    }
                    else
                    {
                        schedule.SelectedPoint.X = (schedule.CurrentSelectedDates.IndexOf(schedule.SelectedDate.Date) * width) + 5;
                    }
                }
                Point position = schedule.SelectedPoint;
                int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                hourHeight = timeslotHeight / (interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue));
                if (isTapped)
                    row = Math.Floor((position.Y / IntervalHeight) / (hourHeight / IntervalHeight));
                else
                    row = schedule.SelectedDate.Hour - ScheduleTimeLineItemsControl.MinValue;
                doubleHour = row / interval;
                double min = (doubleHour - (int)doubleHour) * 60;
                double daysCount = SelectedDates.Count;
                double dayWidth;
                int overAllLeafCount = 1;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    overAllLeafCount = schedule.CalculateLeafCount(ScheduleResourceType);
                    dayWidth = timeslotWidth / (daysCount * overAllLeafCount);
                }
                else
                {
                    dayWidth = timeslotWidth / daysCount;
                }
                var ColumnIndex = 0;
                var dayIndex = 0;
                if (position.X < timeslotWidth)
                {
                    ColumnIndex = (int)Math.Floor(position.X / dayWidth);
                    dayIndex = ColumnIndex;
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        int resourcecount = overAllLeafCount;
                        if (DayHeaderOrder == DayHeaderOrder.OrderByDate)
                        {
                            dayIndex = dayIndex / resourcecount;
                        }
                        else if (dayIndex > SelectedDates.Count - 1 && resourcecount > 1)
                        {
                            dayIndex = (dayIndex % SelectedDates.Count);
                        }
                        if (dayIndex > (SelectedDates.Count) - 1)
                        {
                            dayIndex = (SelectedDates.Count) - 1;
                        }
                        else if (dayIndex < 0)
                        {
                            dayIndex = 0;
                        }
                    }
                    else
                    {
                        if (dayIndex > SelectedDates.Count - 1)
                        {
                            dayIndex = SelectedDates.Count - 1;
                        }
                        else if (dayIndex < 0)
                        {
                            dayIndex = 0;
                        }
                    }
                    if (ColumnIndex < 0)
                    {
                        ColumnIndex = 0;
                    }
                }
                DateTime SelDate = SelectedDates.Count > 0 ? SelectedDates[dayIndex] : new DateTime();
                double xposition = ColumnIndex * dayWidth;
                double yposition = row * hourHeight;
                RectXPosition = double.IsNaN(xposition) ? 0 : xposition;
                RectYPosition = double.IsNaN(yposition) ? 0 : yposition;
                RectHeight = hourHeight;
                RectWidth = dayWidth;
                RectVisibility = Visibility.Visible;
#if !WINRT
                schedule.InternalSelectedDate = SelDate.AddHours((int)doubleHour).AddMinutes(min);
#endif
#if WINRT
                if (isTapped)
#endif
                {
                    SelDate = SelDate.AddHours(ScheduleTimeLineItemsControl.MinValue + row);
                    schedule.SelectedDate = SelDate;
                }
            }
        }

        #endregion

#if !WINRT
        #region Move Selection to Particular DateTime

        internal void MoveSelectionToDateTime(DateTime selectedDateTime)
        {
            // finding row from time
            var timeslotWidth = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualWidth;
            var timeslotHeight = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualHeight;
            double time = selectedDateTime.Hour + (double)(selectedDateTime.Minute) / 60;
            int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            double row = (time - ScheduleTimeLineItemsControl.MinValue) * interval;
            hourHeight = timeslotHeight / (interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue));

            //finding collumn from date (without resources)

            int dateIndex = 0;
            if (SelectedDates.Contains(selectedDateTime.Date))
            {
                dateIndex = SelectedDates.IndexOf(selectedDateTime.Date);                
            }
            else
            {
                schedule.MoveToDate(selectedDateTime.Date);
                dateIndex = SelectedDates.IndexOf(selectedDateTime.Date);
            }
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
               dateIndex= FindResourceColumnIndex(dateIndex);
            }
            double column = dateIndex;
            double dayWidth;
            double daysCount = SelectedDates.Count;
            int overAllLeafCount = 1;
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                overAllLeafCount = schedule.CalculateLeafCount(ScheduleResourceType);
                dayWidth = timeslotWidth / (daysCount * overAllLeafCount);
            }
            else
            {
                dayWidth = timeslotWidth / daysCount;
            }

            //Positioning the selection rect
            double xposition = column * dayWidth;
            double yposition = row * hourHeight;
            RectXPosition = double.IsNaN(xposition) ? 0 : xposition;
            RectYPosition = double.IsNaN(yposition) ? 0 : yposition;
            RectHeight = hourHeight;
            RectWidth = dayWidth;
            RectVisibility = System.Windows.Visibility.Visible;
            schedule.Currentselecteddate = selectedDateTime;
            schedule.InternalSelectedDate = selectedDateTime;
            schedule.SelectedDate = selectedDateTime.Date;
            AdjustScroll();
        }

       

        #endregion

        #region FindResourceColumnIndex
        private int FindResourceColumnIndex(int dateIndex)
        {
            string resourcetype = schedule.Resource;
            var levelrescoll = new List<ObservableCollection<Resource>>();
            levelrescoll.Add(schedule.ScheduleResourceType.ResourceCollection);
            ResourceType restype = schedule.ScheduleResourceType.SubResourceType;
            while (restype != null)
            {
                levelrescoll.Add(restype.ResourceCollection);
                resourcetype = restype.TypeName;
                restype = restype.SubResourceType;
            }
            int currentIndex = 0;
            int leafChildrenCount = 1;
            ResourceType scheduleresourcetype = schedule.ScheduleResourceType;
            List<Resource> resourceCollection = schedule.SelectedAppointment.ResourceCollection.ToList();
            foreach (ObservableCollection<Resource> res in levelrescoll)
            {
                if (res != null)
                {
                    Resource selectedResource = resourceCollection.FirstOrDefault(resource => scheduleresourcetype != null && resource.TypeName == scheduleresourcetype.TypeName);
                    string correspondingResourceName = selectedResource.ResourceName;
                    Resource appres = res.FirstOrDefault(Resrc => Resrc.ResourceName == correspondingResourceName);
                    if (appres == null)
                    {
                        resourceCollection.Remove(selectedResource);
                        selectedResource = resourceCollection.FirstOrDefault(resource => scheduleresourcetype != null && resource.TypeName == scheduleresourcetype.TypeName);
                        correspondingResourceName = selectedResource.ResourceName;
                        appres = res.FirstOrDefault(Resrc => Resrc.ResourceName == correspondingResourceName);
                        int levelIndex = res.IndexOf(appres);
                        currentIndex = (currentIndex * res.Count) + levelIndex;
                        leafChildrenCount = leafChildrenCount * res.Count;
                    }
                    else
                    {
                        int levelIndex = res.IndexOf(appres);
                        currentIndex = (currentIndex * res.Count) + levelIndex;
                        leafChildrenCount = leafChildrenCount * res.Count;
                    }
                }
                scheduleresourcetype = scheduleresourcetype.SubResourceType;
            }
            int resourcecolumnindex = dateIndex;
            if (schedule.DayHeaderOrder == DayHeaderOrder.OrderByResource)
            {
                resourcecolumnindex = resourcecolumnindex + (currentIndex * SelectedDates.Count);
            }
            else
            {
                resourcecolumnindex = (resourcecolumnindex * leafChildrenCount) + currentIndex;
            }
            return resourcecolumnindex;
        }
#endregion

        #region Adjust scroll based on selection

        internal void AdjustScroll()
        {
            if (RectYPosition + RectHeight > scrollviewer.VerticalOffset + scrollviewer.ViewportHeight)
            {
                scrollviewer.ScrollToVerticalOffset(RectYPosition + RectHeight - scrollviewer.ViewportHeight);
            }
            else if (RectYPosition + RectHeight < scrollviewer.VerticalOffset)
            {
                int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
               double  hourHeight = scheduleHorizontalTimeSlotItemsControl.Height / (interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue));

               scrollviewer.ScrollToVerticalOffset(RectYPosition + RectHeight - (2*hourHeight));
            }
            else if (RectYPosition < scrollviewer.VerticalOffset)
            {
                int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                double hourHeight = scheduleHorizontalTimeSlotItemsControl.Height / (interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue));

                scrollviewer.ScrollToVerticalOffset(RectYPosition - (hourHeight));
            }
        }

        #endregion

        #region Move Selection for PageDown on Keyboard input

        internal void MoveSelectionPageDown()
        {
            double pageDownY = Math.Ceiling(scrollviewer.ViewportHeight / IntervalHeight) * IntervalHeight;
            RectYPosition = RectYPosition + pageDownY;
            if (RectYPosition > scrollviewer.ExtentHeight)
            {
                RectYPosition = scrollviewer.ExtentHeight - RectHeight;
            }
            double row = RectYPosition / hourHeight;
            int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            double hours = row / interval;
#if !WINRT
            schedule.Currentselecteddate = schedule.InternalSelectedDate.Date.AddHours(hours);
            schedule.InternalSelectedDate = schedule.InternalSelectedDate.Date.AddHours(hours);
#endif
            if (RectYPosition + RectHeight > scrollviewer.VerticalOffset + scrollviewer.ViewportHeight)
            {
                scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + scrollviewer.ViewportHeight);
            }

        }
        #endregion

        #region Move Selection for PageUp on Keyboard input

        internal void MoveSelectionPageUp()
        {
            double pageDownY = Math.Ceiling(scrollviewer.ViewportHeight / IntervalHeight) * IntervalHeight;
            RectYPosition = RectYPosition - pageDownY;
            if (RectYPosition < 0)
            {
                RectYPosition = 0;
            }
            double row = RectYPosition / hourHeight;
            int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            double hours = row / interval;

            schedule.Currentselecteddate = schedule.InternalSelectedDate.Date.AddHours(hours);
            schedule.InternalSelectedDate = schedule.InternalSelectedDate.Date.AddHours(hours);

            if (RectYPosition < scrollviewer.VerticalOffset)
            {
                scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - scrollviewer.ViewportHeight);
            }
        }
        #endregion

        #region Add day for right

        internal void AddDayRight()
        {
            if (schedule.ScheduleType == ScheduleType.Day && schedule.ScheduleDateRange != null && schedule.ScheduleDateRange.Count > 1)
            {
                if (schedule.CurrentSelectedDates.Contains(schedule.InternalSelectedDate.Date))
                {
                    TimeSpan diffTime = schedule.InternalSelectedDate - schedule.InternalSelectedDate.Date;
                    int index = schedule.CurrentSelectedDates.IndexOf(schedule.InternalSelectedDate.Date);
                    if (index < schedule.CurrentSelectedDates.Count-1)
                    {
                        schedule.Currentselecteddate = schedule.CurrentSelectedDates[index + 1].AddTimeSpan(diffTime);
                        schedule.InternalSelectedDate = schedule.CurrentSelectedDates[index + 1].AddTimeSpan(diffTime);
                        schedule.SelectedDate = schedule.CurrentSelectedDates[index + 1].Date;
                        schedule.MinMaxSelection = schedule.CurrentSelectedDates[index + 1].AddTimeSpan(diffTime);
                    }
                    else
                    {
                        schedule.Currentselecteddate = schedule.InternalSelectedDate.AddDays(1);
                        schedule.InternalSelectedDate = schedule.InternalSelectedDate.AddDays(1);
                        schedule.SelectedDate = schedule.InternalSelectedDate.AddDays(1).Date;
                        if(schedule.MinMaxSelection != new DateTime())
                            schedule.MinMaxSelection = schedule.MinMaxSelection.AddDays(1);
                        else
                            schedule.MinMaxSelection = schedule.InternalSelectedDate.AddDays(1);
                    }
                }
            }
            else
            {
                schedule.Currentselecteddate = schedule.InternalSelectedDate.AddDays(1);
                schedule.InternalSelectedDate = schedule.InternalSelectedDate.AddDays(1);
                schedule.SelectedDate = schedule.InternalSelectedDate.AddDays(1).Date;
                if (schedule.MinMaxSelection != new DateTime())
                    schedule.MinMaxSelection = schedule.MinMaxSelection.AddDays(1);
                else
                    schedule.MinMaxSelection = schedule.InternalSelectedDate.AddDays(1);
            }

        }

        #endregion

        #region MoveRight Selection Rectangle on Keyboard input

        internal void MoveRightSelectionRectangle()
        {
            if (RectHeight != 0 && RectWidth != 0)
            {
                RectVisibility = Visibility.Visible;
                double dayWidth;
                int overAllLeafCount = 1;
                double daysCount = SelectedDates.Count;
                var timeslotWidth = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualWidth;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    overAllLeafCount = schedule.CalculateLeafCount(ScheduleResourceType);
                    dayWidth = timeslotWidth / (daysCount * overAllLeafCount);
                }
                else
                {
                    dayWidth = timeslotWidth / daysCount;
                }

                RectXPosition = double.IsNaN(RectXPosition + dayWidth) ? 0 : RectXPosition + dayWidth;
                double column = RectXPosition / dayWidth;
                int dayIndx = (int)Math.Round(column);
                if (schedule.MinMaxSelection == new DateTime())
                    schedule.MinMaxSelection = schedule.Currentselecteddate;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    bool navigateFlag = false;
                    if (dayIndx == daysCount * overAllLeafCount)
                    {
                        dayIndx = 0;
                        AddDayRight();
                        navigateFlag = true;
                    }
                    dayIndx = GetSelectedResource(dayIndx);
                    DateTime selDate = SelectedDates[dayIndx];
                    if (selDate.Date != schedule.InternalSelectedDate.Date && !navigateFlag)
                    {
                        TimeSpan diffTime = schedule.InternalSelectedDate - schedule.InternalSelectedDate.Date;
                        schedule.Currentselecteddate = selDate.Date.AddTimeSpan(diffTime);
                        schedule.InternalSelectedDate = selDate.Date.AddTimeSpan(diffTime);
                        schedule.SelectedDate = selDate;
                        schedule.MinMaxSelection = selDate.Date.AddTimeSpan(diffTime);
                    }
                }
                else
                {
                    AddDayRight();
                }
                if (RectXPosition + RectWidth > scrollviewer.HorizontalOffset + scrollviewer.ViewportWidth)
                {
                    scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + dayWidth);
                }
                if (!(SelectedDates.Contains(schedule.InternalSelectedDate.Date)))
                {
                    if (schedule.ScheduleType == ScheduleType.WorkWeek)
                    {
                        TimeSpan diffTime = schedule.InternalSelectedDate - schedule.InternalSelectedDate.Date;
                        if (schedule.InternalSelectedDate.Date < SelectedDates[0].AddDays(7).Date && schedule.InternalSelectedDate.Date > SelectedDates[SelectedDates.Count - 1])
                        {
                            schedule.Currentselecteddate = SelectedDates[0].AddDays(7).Date.AddTimeSpan(diffTime);
                            schedule.InternalSelectedDate = SelectedDates[0].AddDays(7).Date.AddTimeSpan(diffTime);
                            schedule.SelectedDate = SelectedDates[0].AddDays(7).Date;
                            schedule.MinMaxSelection = SelectedDates[0].AddDays(7).Date.AddTimeSpan(diffTime);
                        }
                    }
                    schedule.MoveToDate(schedule.InternalSelectedDate.Date);
                    scrollviewer.ScrollToHorizontalOffset(0);
                    RectXPosition = 0;
                    RectHeight = hourHeight;
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        overAllLeafCount = schedule.CalculateLeafCount(ScheduleResourceType);
                        dayWidth = timeslotWidth / (daysCount * overAllLeafCount);
                    }
                    else
                    {
                        dayWidth = timeslotWidth / daysCount;
                    }
                    RectWidth = dayWidth;
                }

            }

        }

        #endregion

        #region Add day for left

        internal void AddDayLeft()
        {
            if (schedule.ScheduleType == ScheduleType.Day && schedule.ScheduleDateRange != null && schedule.ScheduleDateRange.Count > 1)
            {
                if (schedule.CurrentSelectedDates.Contains(schedule.InternalSelectedDate.Date))
                {
                    TimeSpan diffTime = schedule.InternalSelectedDate - schedule.InternalSelectedDate.Date;
                    int index = schedule.CurrentSelectedDates.IndexOf(schedule.InternalSelectedDate.Date);
                    if (index > 0)
                    {
                        schedule.Currentselecteddate = schedule.CurrentSelectedDates[index - 1].AddTimeSpan(diffTime);
                        schedule.InternalSelectedDate = schedule.CurrentSelectedDates[index - 1].AddTimeSpan(diffTime);
                        schedule.SelectedDate = schedule.CurrentSelectedDates[index - 1].Date;
                        schedule.MinMaxSelection = schedule.CurrentSelectedDates[index - 1].AddTimeSpan(diffTime);
                    }
                    else
                    {
                        schedule.Currentselecteddate = schedule.InternalSelectedDate.AddDays(-1);
                        schedule.InternalSelectedDate = schedule.InternalSelectedDate.AddDays(-1);
                        schedule.SelectedDate = schedule.InternalSelectedDate.AddDays(-1).Date;
                        if (schedule.MinMaxSelection != new DateTime())
                            schedule.MinMaxSelection = schedule.MinMaxSelection.AddDays(-1);
                        else
                            schedule.MinMaxSelection = schedule.InternalSelectedDate.AddDays(-1);
                    }
                }
            }
            else
            {
                schedule.Currentselecteddate = schedule.InternalSelectedDate.AddDays(-1);
                schedule.InternalSelectedDate = schedule.InternalSelectedDate.AddDays(-1);
                schedule.SelectedDate = schedule.InternalSelectedDate.AddDays(-1).Date;
                if (schedule.MinMaxSelection != new DateTime())
                    schedule.MinMaxSelection = schedule.MinMaxSelection.AddDays(-1);
                else
                    schedule.MinMaxSelection = schedule.InternalSelectedDate.AddDays(-1);
            }

        }

        #endregion

        #region MoveLeft Selection Rectangle on Keyboard input

        internal void MoveLeftSelectionRectangle()
        {
            if (RectHeight == 0 && RectWidth == 0)
            {

            }
            else
            {
                double dayWidth;
                int overAllLeafCount = 1;
                double daysCount = SelectedDates.Count;
                var timeslotWidth = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualWidth;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    overAllLeafCount = schedule.CalculateLeafCount(ScheduleResourceType);
                    dayWidth = timeslotWidth / (daysCount * overAllLeafCount);
                }
                else
                {
                    dayWidth = timeslotWidth / daysCount;
                }

                RectXPosition = double.IsNaN(RectXPosition - dayWidth) ? 0 : RectXPosition - dayWidth;
                double column = RectXPosition / dayWidth;
                int dayIndx = (int)Math.Round(column);
                if (schedule.MinMaxSelection == new DateTime())
                    schedule.MinMaxSelection = schedule.Currentselecteddate;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {

                    dayIndx = GetSelectedResource(dayIndx);
                    DateTime selDate = SelectedDates[dayIndx];
                    if (selDate.Date != schedule.InternalSelectedDate.Date)
                    {
                        TimeSpan diffTime = schedule.InternalSelectedDate - schedule.InternalSelectedDate.Date;
                        schedule.Currentselecteddate = selDate.Date.AddTimeSpan(diffTime);
                        schedule.InternalSelectedDate = selDate.Date.AddTimeSpan(diffTime);
                        schedule.SelectedDate = selDate;
                        schedule.MinMaxSelection = selDate.Date.AddTimeSpan(diffTime);

                    }

                }
                else
                {
                    AddDayLeft();
                }

                if (RectXPosition < scrollviewer.HorizontalOffset)
                {
                    scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset - dayWidth);
                }
                if (!(SelectedDates.Contains(schedule.InternalSelectedDate.Date)))
                {
                    if (schedule.ScheduleType == ScheduleType.WorkWeek)
                    {
                        TimeSpan diffTime = schedule.InternalSelectedDate - schedule.InternalSelectedDate.Date;
                        if (schedule.InternalSelectedDate.Date < SelectedDates[0].Date)
                        {
                            schedule.Currentselecteddate = SelectedDates[SelectedDates.Count-1].AddDays(-7).Date.AddTimeSpan(diffTime);
                            schedule.InternalSelectedDate = SelectedDates[SelectedDates.Count - 1].AddDays(-7).Date.AddTimeSpan(diffTime);
                            schedule.SelectedDate = SelectedDates[SelectedDates.Count - 1].AddDays(-7).Date;
                            schedule.MinMaxSelection = SelectedDates[SelectedDates.Count - 1].AddDays(-7).Date.AddTimeSpan(diffTime);
                        }
                    }
                    schedule.MoveToDate(schedule.InternalSelectedDate.Date);
                    scrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset + scrollviewer.ViewportWidth);
                    RectXPosition = (daysCount - 1) * dayWidth;
                    RectHeight = hourHeight;
                    timeslotWidth = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>().ActualWidth;
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        overAllLeafCount = schedule.CalculateLeafCount(ScheduleResourceType);
                        dayWidth = timeslotWidth / (daysCount * overAllLeafCount);
                    }
                    else
                    {
                        dayWidth = timeslotWidth / daysCount;
                    }
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        RectXPosition = ((daysCount * overAllLeafCount) - 1) * dayWidth;
                    }
                    RectWidth = dayWidth;
                    RectVisibility = Visibility.Visible;
                }

            }

        }

        #endregion

        #region MoveTop Selection Rectangle on Keyboard input

        internal void MoveTopSelectionRectangle()
        {
            if (RectHeight == 0 && RectWidth == 0)
            {

            }
            else
            {
                double row = RectYPosition / hourHeight;
                if (row > 0)
                {
                    RectYPosition = double.IsNaN((row - 1) * hourHeight) ? 0 : (row - 1) * hourHeight;
                    RectHeight = hourHeight;
                    double ts = schedule.GetTimeInterval().TotalMinutes;
                    schedule.Currentselecteddate = schedule.InternalSelectedDate.AddMinutes(-ts);
                    schedule.InternalSelectedDate = schedule.InternalSelectedDate.AddMinutes(-ts);
                    schedule.MinMaxSelection = schedule.Currentselecteddate;
                }
                if (RectYPosition < scrollviewer.VerticalOffset)
                {
                    scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset - RectHeight);
                }
            }
        }

        #endregion

        #region MoveBottom Selection Rectangle on Keyboard input

        internal void MoveBottomSelectionRectangle()
        {
            if (RectHeight == 0 && RectWidth == 0)
            {

            }
            else
            {
                
                int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                int maxRow = interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
                double row = RectYPosition / hourHeight;
                if (row < maxRow - 1)
                {
                    RectYPosition = double.IsNaN((row + 1) * hourHeight) ? 0 : (row + 1) * hourHeight;
                    RectHeight = hourHeight;
                    double ts = schedule.GetTimeInterval().TotalMinutes;
                    schedule.Currentselecteddate = schedule.InternalSelectedDate.AddMinutes(ts);
                    schedule.InternalSelectedDate = schedule.InternalSelectedDate.AddMinutes(ts);
                    schedule.MinMaxSelection = schedule.Currentselecteddate;
                }
                if (RectYPosition + RectHeight > scrollviewer.VerticalOffset + scrollviewer.ViewportHeight)
                {
                    scrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset + RectHeight);
                }
            }
        }

        #endregion

        #region Add selection up
        internal void AddSelectionUp(DateTime MinMaxSelection)
        {
            RectYPosition = RectYPosition - hourHeight;
            RectHeight = RectHeight + hourHeight;
            AdjustScroll();
        }

        #endregion

        #region Sub selection up
        internal void SubSelectionUp(DateTime MinMaxSelection)
        {
            //RectYPosition = RectYPosition + hourHeight;
            RectHeight = RectHeight - hourHeight;
            AdjustScroll();
        }
        #endregion

        #region Add selection down
        internal void AddSelectionDown(DateTime MinMaxSelection)
        {
            RectHeight = RectHeight + hourHeight;
            AdjustScroll();
        }
        #endregion

        #region Sub selection down
        internal void SubSelectionDown(DateTime MinMaxSelection)
        {
            RectYPosition = RectYPosition + hourHeight;
            RectHeight = RectHeight - hourHeight;
            AdjustScroll();
        }
        #endregion

#endif

        #region Toggling NavigationTap Visibility

        internal void SetNavigationTapVisibility()
        {
            if (schedule != null)
            {
                int count = VisibleAppointments.Count;
                bool prevAppAvail = false;
                bool nextAppAvail = false;
                if (schedule.Resource != string.Empty && schedule.ScheduleResourceTypeCollection.Count > 0)
                {
                    var appointments = VisibleAppointments.Where(x => x.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null);
                    count = appointments.Count();
                }
                if (previousNavigationTap != null && nextNavigationTap != null && ShowAppointmentNavigationButtons)
                {
                    if (count == 0 && schedule.Appointments != null && schedule.Appointments.Count > 0)
                    {
                        nextNavigationTap.Opacity = 1;
                        previousNavigationTap.Opacity = 1;
                        ObservableCollection<DateTime> orderedNextAppointmentDates = new ObservableCollection<DateTime>();
                        ObservableCollection<DateTime> orderedPrevAppointmentDates = new ObservableCollection<DateTime>();
                        
                        if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0 && schedule.Resource != string.Empty)
                        {
                            var filteredNextAppointments = schedule.ProxyAppointments.OrderBy(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date > SelectedDates[SelectedDates.Count - 1].Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)) != null));
                            var filteredPrevAppointments = schedule.ProxyAppointments.OrderBy(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date < SelectedDates[0].Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)) != null));
                            if (!ShowNonWorkingHours)
                            {
                                orderedNextAppointmentDates = new ObservableCollection<DateTime>(schedule.ProxyAppointments.Keys.OrderBy(x => x.Date));
                                orderedNextAppointmentDates = new ObservableCollection<DateTime>(orderedNextAppointmentDates.OrderByDescending(x => x.Date).Reverse());
                                orderedPrevAppointmentDates = new ObservableCollection<DateTime>(orderedNextAppointmentDates.OrderByDescending(x => x.Date));
                                foreach (DateTime date in orderedNextAppointmentDates)
                                {
                                    if (date.Date > SelectedDates[SelectedDates.Count - 1].Date)
                                    {
                                        ObservableCollection<ScheduleAppointment> appColl = new ObservableCollection<ScheduleAppointment>();
                                        appColl = schedule.ProxyAppointments[date.Date];
                                        foreach (ScheduleAppointment scheduleApp in appColl)
                                        {

                                            if ((scheduleApp.InternalStartTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalStartTime.TimeOfDay.Hours < schedule.WorkEndHour)
                                            || (scheduleApp.InternalEndTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalEndTime.TimeOfDay.Hours < schedule.WorkEndHour))
                                            {
                                                nextAppAvail = true;
                                                break;
                                            }

                                        }
                                    }
                                }
                                foreach (DateTime date in orderedPrevAppointmentDates)
                                {
                                    if (date.Date < SelectedDates[0].Date)
                                    {
                                        ObservableCollection<ScheduleAppointment> appColl = new ObservableCollection<ScheduleAppointment>();
                                        appColl = schedule.ProxyAppointments[date.Date];
                                        foreach (ScheduleAppointment scheduleApp in appColl)
                                        {

                                            if ((scheduleApp.InternalStartTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalStartTime.TimeOfDay.Hours < schedule.WorkEndHour)
                                               || (scheduleApp.InternalEndTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalEndTime.TimeOfDay.Hours < schedule.WorkEndHour))
                                            {
                                                prevAppAvail = true;
                                                break;
                                            }

                                        }
                                    }
                                }

                            }
                            if ((ShowNonWorkingHours && filteredPrevAppointments.Key != new DateTime() && filteredNextAppointments.Key != new DateTime() && filteredNextAppointments.Value != null && filteredPrevAppointments.Value != null)
                                || (!ShowNonWorkingHours && nextAppAvail && prevAppAvail))
                            {
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                            }
                            else if ( (ShowNonWorkingHours && filteredNextAppointments.Key != new DateTime() && filteredNextAppointments.Value != null)
                                || (!ShowNonWorkingHours && nextAppAvail))
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = false;
                                    previousNavigationTap.Opacity = 0.5;
                                }
                            }
                            else if ((ShowNonWorkingHours && filteredPrevAppointments.Value != null )|| (!ShowNonWorkingHours && prevAppAvail))
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = false;
                                    nextNavigationTap.Opacity = 0.5;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                            }
                            else
                            {
                                nextNavigationTap.Opacity = 0;
                                previousNavigationTap.Opacity = 0;
                            }
                        }
                        else
                        {
                            var filteredNextAppointments = schedule.ProxyAppointments.Keys.FirstOrDefault(x => x.Date > SelectedDates[SelectedDates.Count - 1].Date);
                            var filteredPrevAppointments = schedule.ProxyAppointments.Keys.FirstOrDefault(x => x.Date < SelectedDates[0].Date);
                            if (!ShowNonWorkingHours)
                            {
                               orderedNextAppointmentDates = new ObservableCollection<DateTime>( schedule.ProxyAppointments.Keys.OrderBy(x => x.Date));
                               orderedNextAppointmentDates = new ObservableCollection<DateTime>( orderedNextAppointmentDates.OrderByDescending(x => x.Date).Reverse() );
                               orderedPrevAppointmentDates = new ObservableCollection<DateTime>(orderedNextAppointmentDates.OrderByDescending(x => x.Date));
                               foreach (DateTime date in orderedNextAppointmentDates)
                               {
                                   if (date.Date > SelectedDates[SelectedDates.Count - 1].Date)
                                   {
                                       ObservableCollection<ScheduleAppointment> appColl = new ObservableCollection<ScheduleAppointment>();
                                       appColl = schedule.ProxyAppointments[date.Date];
                                       foreach (ScheduleAppointment scheduleApp in appColl)
                                       {

                                           if ((scheduleApp.InternalStartTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalStartTime.TimeOfDay.Hours < schedule.WorkEndHour)
                                           || (scheduleApp.InternalEndTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalEndTime.TimeOfDay.Hours < schedule.WorkEndHour))
                                           {
                                               nextAppAvail = true;
                                               break;
                                           }

                                       }
                                   }
                               }
                               foreach (DateTime date in orderedPrevAppointmentDates)
                               {
                                   if (date.Date < SelectedDates[0].Date)
                                   {
                                       ObservableCollection<ScheduleAppointment> appColl = new ObservableCollection<ScheduleAppointment>();
                                       appColl = schedule.ProxyAppointments[date.Date];
                                       foreach (ScheduleAppointment scheduleApp in appColl)
                                       {

                                           if ((scheduleApp.InternalStartTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalStartTime.TimeOfDay.Hours < schedule.WorkEndHour)
                                              || (scheduleApp.InternalEndTime.TimeOfDay.Hours > schedule.WorkStartHour && scheduleApp.InternalEndTime.TimeOfDay.Hours < schedule.WorkEndHour))
                                           {
                                               prevAppAvail = true;
                                               break;
                                           }

                                       }
                                   }
                               }
                               
                            }
                            if ((ShowNonWorkingHours && filteredPrevAppointments != new DateTime() && filteredNextAppointments != new DateTime())
                                || (!ShowNonWorkingHours && nextAppAvail && prevAppAvail))
                            {
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                            }
                            else if ((ShowNonWorkingHours && filteredNextAppointments != new DateTime() )|| (!ShowNonWorkingHours && nextAppAvail))
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = false;
                                    previousNavigationTap.Opacity = 0.5;
                                }
                            }
                            else if ((ShowNonWorkingHours ) || (!ShowNonWorkingHours && prevAppAvail))
                            {
                                if (nextNavigationTap != null)
                                {
                                    nextNavigationTap.IsHitTestVisible = false;
                                    nextNavigationTap.Opacity = 0.5;
                                }
                                if (previousNavigationTap != null)
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                            }
                            else
                            {
                                nextNavigationTap.Opacity = 0;
                                previousNavigationTap.Opacity = 0;
                            }
                        }
                    }
                    else
                    {
                        nextNavigationTap.Opacity = 0;
                        previousNavigationTap.Opacity = 0;
                    }
                }
            }
        }

        private bool CheckVisibleAppointments(ObservableCollection<ScheduleAppointment> filteredAppointments)
        {
            if (!ShowNonWorkingHours)
            {
                var visibleAppointments = filteredAppointments.Where(x => (x as ScheduleAppointment).StartTime.Hour < WorkEndHour &&
                                                                          (x as ScheduleAppointment).EndTime.Hour > WorkStartHour);
                return (visibleAppointments.ToList().Count > 0);
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region Enabling Drag & Drop

        internal void EnableDragDrop(SfSchedule sfSchedule)
        {
            if (SelectedDates.Count <= 0 || (SelectedDates.Count > 0 && !SelectedDates.Contains(sfSchedule.Currentselecteddate.Date)))
                return;
            int resourcecount = sfSchedule.CalculateLeafCount(ScheduleResourceType);
            var HrTimeSlotItem = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
            double Columnwidth = HrTimeSlotItem.ActualWidth / (sfSchedule.CurrentSelectedDates.Count * resourcecount);
            var timelineControl = this.FindElementOfType<ScheduleTimeLineItemsControl>();
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;


            double appHeight = sfSchedule.FloatingAppointmentSize.Height;
            var drag_app = new ScheduleDaysAppointmentViewControl
            {
                AppWidth = Columnwidth,
                DragRectangleVisibility = Visibility.Visible,
                DataContext = sfSchedule.dvc.DataContext
            };
#if WINRT
            double yPosition = sfSchedule.currentpoint.Position.Y - sfSchedule.Appointmentpoint.Position.Y;
            double scrollYpos = sfSchedule.Scrollpoint.Position.Y - sfSchedule.Appointmentpoint.Position.Y;
#else
            double yPosition = sfSchedule.currentpoint.Y - sfSchedule.Appointmentpoint.Y;
            double scrollYpos = sfSchedule.Scrollpoint.Y - sfSchedule.Appointmentpoint.Y;
#endif


            //added for drag drop feature
            if (yPosition < sfSchedule.ActualHeight - scrollviewer.ViewportHeight - 5)
            {
                double height = appHeight - (sfSchedule.ActualHeight - scrollviewer.ViewportHeight - 5 - yPosition);
                drag_app.Height = height>0 ? height : IntervalHeight;
                
                yPosition = sfSchedule.ActualHeight - scrollviewer.ViewportHeight - 5;
                if (schedule.SelectedAppointment.AllDay)
                {
                    drag_app.Height = 30;
                    #if WINRT
                    yPosition = yPosition - 30;
#else
                    yPosition = yPosition - 27;
#endif
                }
            }
            else if (scrollYpos + appHeight > scrollviewer.ViewportHeight)
            {
                drag_app.Height = scrollviewer.ViewportHeight - scrollYpos;
            }
            else
            {
                drag_app.Height = appHeight;
            }

            if (obj != null)
            {
                var DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
                sfSchedule.dayScrollViewer = DaysScrollviewer;
            }


#if WINRT
            drag_app.PointerPressed += sfSchedule.drag_app_PointerPressed;
#else
            drag_app.MouseLeftButtonDown += sfSchedule.drag_app_MouseLeftButtonDown;
#endif
            if ((sfSchedule.dvc.DataContext is ScheduleAppointment) && !(sfSchedule.dvc.DataContext as ScheduleAppointment).AllDay)
            {
                drag_app.Loaded += sfSchedule.drag_app_Loaded;
            }
            double scrollOffset = scrollviewer.HorizontalOffset;
            double remain = scrollOffset % Columnwidth;
#if WINRT
            double leftPosition = sfSchedule.currentpoint.Position.X - sfSchedule.Appointmentpoint.Position.X - (Columnwidth - sfSchedule.FloatingAppointmentSize.Width) - 10;
            //Changed the following condition for issue fix of Draganddrop overlay appointment is not viewed if scroll bar is moved to right most position
            //if (scrollviewer.ActualWidth - 6 <= scrollOffset)
            if (scrollviewer.ActualWidth < scrollOffset)
                leftPosition = sfSchedule.currentpoint.Position.X - sfSchedule.Appointmentpoint.Position.X - (Columnwidth - sfSchedule.FloatingAppointmentSize.Width);
            double moveRight = Columnwidth - ((leftPosition - timelineControl.ActualWidth) % Columnwidth);
            moveRight = moveRight >= Columnwidth + 3 ? moveRight - Columnwidth : moveRight;
            Canvas.SetLeft(drag_app, sfSchedule.currentpoint.Position.X - sfSchedule.Appointmentpoint.Position.X - (Columnwidth - sfSchedule.FloatingAppointmentSize.Width) + moveRight - remain - 5);
            //Changed the following condition for issue fix of Draganddrop overlay appointment is not viewed if scroll bar is moved to right most position
            //if (scrollviewer.ActualWidth - 6 <= scrollOffset)
            if (scrollviewer.ActualWidth < scrollOffset)
                Canvas.SetLeft(drag_app, sfSchedule.currentpoint.Position.X - sfSchedule.Appointmentpoint.Position.X - (Columnwidth - sfSchedule.FloatingAppointmentSize.Width) + moveRight - remain + 5);
            Canvas.SetTop(drag_app, yPosition);
#else
            if ((int)remain == (int)Columnwidth)
                remain = 0;
            double leftPosition = sfSchedule.currentpoint.X - sfSchedule.Appointmentpoint.X - (Columnwidth - sfSchedule.FloatingAppointmentSize.Width) - 10;
            double moveRight = Columnwidth - ((leftPosition - timelineControl.ActualWidth) % Columnwidth);
            moveRight = moveRight > Columnwidth ? moveRight - Columnwidth : moveRight;
            Canvas.SetLeft(drag_app, sfSchedule.currentpoint.X - sfSchedule.Appointmentpoint.X - (Columnwidth - sfSchedule.FloatingAppointmentSize.Width) + moveRight - remain - 8);
            Canvas.SetTop(drag_app, yPosition);
            //drag_app.Height = appHeight;
#endif

            drag_app.Width = Columnwidth;


            sfSchedule.DragDropCanvas.Children.Add(drag_app);
        }

        #endregion

        #region Releasing Drag & Drop
#if WINRT
        internal void ReleaseDragDrop(SfSchedule sfSchedule, PointerRoutedEventArgs e)
        {
            var AllDayviewscroll = sfSchedule.viewcontrol.FindElementOfType<ScrollViewer>();
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            var realStartDate = new DateTime();
            var realEndDate = new DateTime();
            if (obj != null)
            {
                var DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
                var dropappointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                var selectedDate = sfSchedule.GetCurrentDropLocation(sfSchedule.viewcontrol, e);
                DateTime EndDate;
                {
                    DatetimeDiff = (sfSchedule.dvc.ActualHeight) / IntervalHeight;
                }
                EndDate = selectedDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                if (!(dragDropCanvasHeight == sfSchedule.dvc.ActualHeight))
                {
                    double diffHeight = dragDropCanvasHeight - sfSchedule.dvc.ActualHeight;
                    double diffTime = diffHeight / IntervalHeight;
                    if (dragResizeFlag)
                    {
                        realStartDate = selectedDate.AddMinutes(-(sfSchedule.GetTimeInterval().TotalMinutes * diffTime));
                    }
                    else
                    {
                        realEndDate = EndDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * diffTime);
                    }
                }
                if (selectedDate == EndDate)
                {
                    EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                }

                if (dropappointment != null)
                {
                    if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                    {
                        sfSchedule.AddResources(dropappointment);
                    }
                    DateTime daystarttime, dayendtime;
                    if (!sfSchedule.allDayFlag)
                    {
                        if (!(dragDropCanvasHeight == sfSchedule.dvc.ActualHeight))
                        {
                            if (dragResizeFlag)
                            {
                                selectedDate = realStartDate;
                            }
                            else
                            {
                                EndDate = realEndDate;
                            }
                        }
                        daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                        if (dropappointment.IsRecursive)
                        {
                            int _totalRecursiveCount = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID].Count;
                            DateTime _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0];
                            DateTime _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount -1] + TimeSpan.FromHours(DatetimeDiff);
                            ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID];
                                if (dropappointment.StartTime.Date >= _firstApppointmentStartTime && dropappointment.EndTime.Date <= _lastApppointmentEndTime)
                                {
                                    if (dropappointment.StartTime.Date == _firstApppointmentStartTime.Date)
                                    {
                                        if (daystarttime.Date < sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][1].Date)
                                        {
                                       if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Remove(dropappointment);
                                        }
                                            dropappointment.StartTime = daystarttime;
                                            dropappointment.EndTime = dayendtime;
                                       if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Add(dropappointment);
                                        }
                                        else
                                        {
                                            schedule.ProxyAppointments.Add(dropappointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { dropappointment });
                                        }
                                            _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0] = daystarttime;
                                        }
                                        else
                                        {
                                            sfSchedule.DragDropCanvas.Children.Clear();
                                        }
                                    }
                                    else if (dropappointment.EndTime.Date == _lastApppointmentEndTime.Date)
                                    {
                                        if (daystarttime.Date > sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 2].Date)
                                        {
        if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Remove(dropappointment);
                                        }
                                            dropappointment.StartTime = daystarttime;
                                            dropappointment.EndTime = dayendtime;
         if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Add(dropappointment);
                                        }
                                        else
                                        {
                                            schedule.ProxyAppointments.Add(dropappointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { dropappointment });
                                        }
                                            sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] = dayendtime;
                                            _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                                        }
                                        else
                                        {
                                            sfSchedule.DragDropCanvas.Children.Clear();
                                        }
                                    }
                                    else if (daystarttime >= _firstApppointmentStartTime && dayendtime <= _lastApppointmentEndTime)
                                    {
                                        int _indexCount = 0;
                                        int _droppedAppointmentIndex = 0;
                                        

                                        foreach (var item in _recursiveAppCollection)
                                        {
                                            if (dropappointment.StartTime.Date == ((DateTime)item).Date )//&& daystarttime.Date > sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_indexCount + 1] && daystarttime.Date < sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_indexCount - 1])
                                            {
                                                _droppedAppointmentIndex = _indexCount;

                                            }
                                            _indexCount++;
                                        }
                                        sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_droppedAppointmentIndex] = daystarttime;
        if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Remove(dropappointment);
                                        }
                                        dropappointment.StartTime = daystarttime;
                                        dropappointment.EndTime = dayendtime;
         if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Add(dropappointment);
                                        }
                                        else
                                        {
                                            schedule.ProxyAppointments.Add(dropappointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { dropappointment });
                                        }
                                    }
                                    else
                                    {
                                        sfSchedule.DragDropCanvas.Children.Clear();
                                    }
                                }
                                else
                                {
                                    sfSchedule.DragDropCanvas.Children.Clear();
                                }
                          
                        }
                        else if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                        {
                            bool ispropertyset = false;
                            foreach (object dayobj in (sfSchedule.ItemsSource as IEnumerable))
                            {
                                Type type = dayobj.GetType();
                                if (dayobj.GetHashCode() == dropappointment.ObjectID)
                                {
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                dropappointment.StartTime = daystarttime;
                                dropappointment.EndTime = dayendtime;
                            }
                        }
                        else
                        {
                            dropappointment.StartTime = daystarttime;
                            dropappointment.EndTime = dayendtime;
                        }
                        dropappointment.AllDay = false;
                    }
                    else
                    {

                        if (dropappointment.StartTime == ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null))
                        {
                            daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null).AddTicks(1);
                        }
                        else
                        {
                            daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        }
                        if (dropappointment.EndTime == dropappointment.StartTime)
                        {
                            dayendtime = ScheduleAppointment.ConvertToActualTime(selectedDate.AddTicks(1), "StartTime", dropappointment.StartTimeZone, null);
                        }
                        else
                        {
                            dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                        }
                        if (dropappointment.IsRecursive)
                        {
                            if ((dropappointment.StartTime.Date != daystarttime.Date || dropappointment.EndTime.Date != dayendtime.Date) && sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID].Contains(daystarttime.Date) || sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID].Contains(dayendtime.Date))
                            {
                                sfSchedule.DragDropCanvas.Children.Clear();
                            }
                            else
                            {
                                dropappointment.StartTime = daystarttime;
                                dropappointment.EndTime = dayendtime;
                            }
                        }
                        else if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                        {
                            bool ispropertyset = false;
                            foreach (object dayobj in (sfSchedule.ItemsSource as IEnumerable))
                            {
                                Type type = dayobj.GetType();
                                if (dayobj.GetHashCode() == dropappointment.ObjectID)
                                {
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                dropappointment.StartTime = daystarttime;
                                dropappointment.EndTime = dayendtime;
                            }
                        }
                        else
                        {
                            dropappointment.StartTime = daystarttime;
                            dropappointment.EndTime = dayendtime;
                        }
                        dropappointment.AllDay = true;
                        sfSchedule.DragDropCanvas.Children.Clear();
                    }
                    dayCanvasInAllday = false;
                }
                if (DaysScrollviewer != null)
                    DaysScrollviewer.VerticalScrollMode = ScrollMode.Auto;
            }
            sfSchedule.IsResizeEnabled = true;
            DatetimeDiff = DayDiff = 0;
            if (sfSchedule.Dayviewrb.EllipseAnimation1 != null)
            {
                sfSchedule.Dayviewrb.EllipseAnimation1.Begin();
                sfSchedule.Dayviewrb.EllipseAnimation2.Begin();
            }
            sfSchedule.dvc = null;
            if ((sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null) || ((sfSchedule.ScheduleType == ScheduleType.WorkWeek || sfSchedule.ScheduleType == ScheduleType.Week || sfSchedule.ScheduleType == ScheduleType.Day) && dayCanvasFromAllday))
            {
                sfSchedule.DragDropCanvas.Children.Clear();
                dayCanvasFromAllday = false;
            }
        }
#else
        internal void ReleaseDragDrop(SfSchedule sfSchedule, MouseButtonEventArgs e)
        {
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            var realStartDate = new DateTime();
            var realEndDate = new DateTime();
            if (obj != null)
            {
                bool containitems = sfSchedule.CheckItemsInItemsSource();
                var dropappointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                var selectedDate = sfSchedule.GetCurrentDropLocationForMouseButtonEventArgs(sfSchedule.currentSelectedItem, e);
                DateTime EndDate;
                {
                    DatetimeDiff = (sfSchedule.dvc.ActualHeight) / IntervalHeight;
                }
                EndDate = selectedDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                if (!(dragDropCanvasHeight == sfSchedule.dvc.ActualHeight))
                {
                    double diffHeight = dragDropCanvasHeight - sfSchedule.dvc.ActualHeight;
                    double diffTime = diffHeight / IntervalHeight;
                    if (dragResizeFlag)
                    {
                        realStartDate = selectedDate.AddMinutes(-(sfSchedule.GetTimeInterval().TotalMinutes * diffTime));
                    }
                    else
                    {
                        realEndDate = EndDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * diffTime);
                    }
                }
                if (selectedDate == EndDate)
                {
                    EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                }
                if (dropappointment != null)
                {
                    if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                    {
                        sfSchedule.AddResources(dropappointment);
                    }
                    DateTime daystarttime, dayendtime;
                    if (!sfSchedule.allDayFlag)
                    {
                        if (!(dragDropCanvasHeight == sfSchedule.dvc.ActualHeight))
                        {
                            if (dragResizeFlag)
                            {
                                selectedDate = realStartDate;
                            }
                            else
                            {
                                EndDate = realEndDate;
                            }
                        }
                        daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);


                        if (dropappointment.IsRecursive)
                        {
                            int _totalRecursiveCount = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID].Count;
                            DateTime _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0];
                            DateTime _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                            ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID];
                            if (dropappointment.StartTime.Date >= _firstApppointmentStartTime  && dropappointment.EndTime.Date <= _lastApppointmentEndTime.Date)
                            {
                                if (dropappointment.StartTime.Date == _firstApppointmentStartTime.Date)
                                {
                                    if (daystarttime.Date < sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][1].Date)
                                    {
                                        if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Remove(dropappointment);
                                        }
                                        dropappointment.StartTime = daystarttime;
                                        dropappointment.EndTime = dayendtime;
                                        if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Add(dropappointment);
                                        }
                                        else
                                        {
                                            schedule.ProxyAppointments.Add(dropappointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { dropappointment });
                                        }
                                        _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0] = daystarttime;
                                    }
                                    else
                                    {
                                        sfSchedule.DragDropCanvas.Children.Clear();
                                    }
                                }
                                else if (dropappointment.EndTime.Date == _lastApppointmentEndTime.Date)
                                {
                                    if (daystarttime.Date > sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 2].Date)
                                    {
                                        if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Remove(dropappointment);
                                        }
                                        dropappointment.StartTime = daystarttime;
                                        dropappointment.EndTime = dayendtime;
                                        if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                        {
                                            schedule.ProxyAppointments[dropappointment.StartTime.Date].Add(dropappointment);
                                        }
                                        else
                                        {
                                            schedule.ProxyAppointments.Add(dropappointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { dropappointment });
                                        }
                                        sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] = dayendtime;
                                        _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                                    }
                                    else
                                    {
                                        sfSchedule.DragDropCanvas.Children.Clear();
                                    }
                                }
                                else if (daystarttime.Date > _firstApppointmentStartTime.Date && daystarttime.Date < _lastApppointmentEndTime.Date && dayendtime.Date < _lastApppointmentEndTime.Date)
                                {
                                    int _indexCount = 0;
                                    int _droppedAppointmentIndex = 0;


                                    foreach (var item in _recursiveAppCollection)
                                    {
                                        if (dropappointment.StartTime.Date == ((DateTime)item).Date)
                                        {
                                            _droppedAppointmentIndex = _indexCount;
                                        }
                                        _indexCount++;
                                    }
                                    sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_droppedAppointmentIndex] = daystarttime;
                                    if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                    {
                                        schedule.ProxyAppointments[dropappointment.StartTime.Date].Remove(dropappointment);
                                    }
                                    dropappointment.StartTime = daystarttime;
                                    dropappointment.EndTime = dayendtime;
                                    if (schedule.ProxyAppointments.Keys.Contains(dropappointment.StartTime.Date))
                                    {
                                        schedule.ProxyAppointments[dropappointment.StartTime.Date].Add(dropappointment);
                                    }
                                    else
                                    {
                                        schedule.ProxyAppointments.Add(dropappointment.StartTime.Date, new ObservableCollection<ScheduleAppointment>() { dropappointment });
                                    }
                                }
                                else
                                {
                                    sfSchedule.DragDropCanvas.Children.Clear();
                                }
                            }
                            else
                            {
                                sfSchedule.DragDropCanvas.Children.Clear();
                            }

                        }
                        else if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                        {
                            bool ispropertyset = false;
                            foreach (object dayobj in (sfSchedule.ItemsSource as IEnumerable))
                            {
                                Type type = dayobj.GetType();
                                if (dayobj.GetHashCode() == dropappointment.ObjectID)
                                {
                                    type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                    type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                //This property is set to avoid adding of items in proxy appointment dictionary  
                                sfSchedule.stopUpdate = true;
                                dropappointment.StartTime = daystarttime;
                                sfSchedule.stopUpdate = false;
                                dropappointment.EndTime = dayendtime;
                            }
                        }
                        else
                        {
                            //This property is set to avoid adding of items in proxy appointment dictionary  
                            sfSchedule.stopUpdate = true;
                            dropappointment.StartTime = daystarttime;
                            sfSchedule.stopUpdate = false;
                            dropappointment.EndTime = dayendtime;
                        }
                        dropappointment.AllDay = false;
                    }
                    else
                    {
                        if (dropappointment.StartTime == ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null))
                        {
                            daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null).AddTicks(1);
                        }
                        else
                        {
                            daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        }
                        if (dropappointment.EndTime == dropappointment.StartTime)
                        {
                            dayendtime = ScheduleAppointment.ConvertToActualTime(selectedDate.AddTicks(1), "StartTime", dropappointment.StartTimeZone, null);
                        }
                        else
                        {
                            dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                        }
                        if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                        {
                            bool ispropertyset = false;
                            foreach (object dayobj in (sfSchedule.ItemsSource as IEnumerable))
                            {
                                Type type = dayobj.GetType();
                                if (dayobj.GetHashCode() == dropappointment.ObjectID)
                                {
                                    type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                    type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                //This property is set to avoid adding of items in proxy appointment dictionary  
                                sfSchedule.stopUpdate = true;
                                dropappointment.StartTime = daystarttime;
                                sfSchedule.stopUpdate = true;
                                dropappointment.EndTime = dayendtime;
                            }
                        }
                        else
                        {
                            //This property is set to avoid adding of items in proxy appointment dictionary  
                            sfSchedule.stopUpdate = true;
                            dropappointment.StartTime = daystarttime;
                            sfSchedule.stopUpdate = true;
                            dropappointment.EndTime = dayendtime;
                        }
                        dropappointment.AllDay = true;
                        sfSchedule.DragDropCanvas.Children.Clear();
                    }
                    dayCanvasInAllday = false;
                }
            }
            sfSchedule.IsResizeEnabled = true;
            DatetimeDiff = DayDiff = 0;
            sfSchedule.dvc = null;
            if ((sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null) || ((sfSchedule.ScheduleType == ScheduleType.WorkWeek ||sfSchedule.ScheduleType == ScheduleType.Week || sfSchedule.ScheduleType == ScheduleType.Day) && dayCanvasFromAllday))
            {
                sfSchedule.DragDropCanvas.Children.Clear();
                dayCanvasFromAllday = false;
            }
        }
#endif
        #endregion

        #region Releasing Resize

        internal void ReleaseResize(SfSchedule sfSchedule)
        {
            if (sfSchedule.dvc != null)
            {
                DatetimeDiff = (sfSchedule.dvc.ActualHeight) / IntervalHeight;
                var scheduleAppointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                bool multiday = false;
                bool containitems = sfSchedule.CheckItemsInItemsSource();
                if (scheduleAppointment != null)
                {
                    if (scheduleAppointment.InternalEndTime.Date.Date > scheduleAppointment.InternalStartTime.Date.Date)
                        multiday = true;
                    if (sfSchedule.Dayviewrb.resizePart == ResizeBehavior.ControlParts.Bottom)
                    {
                        DateTime dayendtime;
                        if (!multiday)
                        {
                            DateTime end = scheduleAppointment.InternalStartTime.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                            dayendtime = ScheduleAppointment.ConvertToActualTime(end, "EndTime", null, scheduleAppointment.EndTimeZone);
                        }
                        else
                        {
                            DateTime end = scheduleAppointment.InternalEndTime.Date.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                            dayendtime = ScheduleAppointment.ConvertToActualTime(end, "EndTime", null, scheduleAppointment.EndTimeZone);
                        }
                        if (sfSchedule.ItemsSource is IEnumerable && sfSchedule.AppointmentMapping != null &&
                            sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty &&
                            sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                        {
                            bool ispropertyset = false;
                            foreach (object obj in (sfSchedule.ItemsSource as IEnumerable))
                            {
                                Type type = obj.GetType();
                                if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                                {
#if WINRT
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(obj, dayendtime, null);
#else
                                    type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping)
                                        .SetValue(obj, dayendtime, null);
#endif
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                scheduleAppointment.EndTime = dayendtime;
                            }
                        }
                        else
                        {
                            scheduleAppointment.EndTime = dayendtime;
                        }
                    }
                    else if (sfSchedule.Dayviewrb.resizePart == ResizeBehavior.ControlParts.Top)
                    {
                        DateTime daystarttime;
                        if (!multiday)
                        {
                            DateTime start =
                                scheduleAppointment.InternalEndTime.AddMinutes(
                                    -(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff));
                            daystarttime = ScheduleAppointment.ConvertToActualTime(start, "StartTime",
                                scheduleAppointment.StartTimeZone, null);
                        }
                        else
                        {
                            DateTime currentDate = scheduleAppointment.InternalStartTime.Date.AddHours(24);
                            daystarttime =
                                currentDate.AddMinutes(
                                    -(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff));
                        }
                        if (sfSchedule.ItemsSource is IEnumerable && sfSchedule.AppointmentMapping != null &&
                            sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty &&
                            sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                        {
                            bool ispropertyset = false;
                            foreach (object obj in (sfSchedule.ItemsSource as IEnumerable))
                            {
                                Type type = obj.GetType();
                                if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                                {
#if WINRT
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(obj, daystarttime, null);
#else
                                    type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping)
                                        .SetValue(obj, daystarttime, null);
#endif
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                scheduleAppointment.StartTime = daystarttime;
                            }
                        }
                        else
                        {
                            scheduleAppointment.StartTime = daystarttime;
                        }
                    }
                }

                var appointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                if (appointment != null)
                    DayDiff = appointment.InternalEndTime.DayOfYear - appointment.InternalStartTime.DayOfYear;
#if WINRT
                sfSchedule.Dayviewrb.EllipseAnimation1.Begin();
                sfSchedule.Dayviewrb.EllipseAnimation2.Begin();
#endif
            }
        }

        #endregion

        #region Moving Dragged Appointment

#if WINRT
        internal void MoveDragDrop(SfSchedule sfSchedule, bool enableDrag, PointerRoutedEventArgs e)
        {
            var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl);
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            var DaysScrollviewer = (obj != null) ? obj.FindName("Scrollviewer") as ScrollViewer : null;
            int resourcecount = sfSchedule.CalculateLeafCount(ScheduleResourceType);
            var HrTimeSlotItem = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
            var timeLineControlWidth = this.FindElementOfType<ScheduleTimeLineItemsControl>().ActualWidth;
            double Columnwidth, remain;
            double xPos = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X;
            double scrollOffset = scrollviewer.HorizontalOffset;
            double adjustCanvas = 0d;
            if (enableDrag)
            {
                Columnwidth = HrTimeSlotItem.ActualWidth / (sfSchedule.SelectedDates.Count * resourcecount);
                remain = scrollOffset % Columnwidth;
                if (remain > 1)
                {
                    adjustCanvas = remain - Columnwidth;
                }
                var currentdragcolumn = (int)((xPos - timeLineControlWidth - 4 + (adjustCanvas)) / Columnwidth);
                Canvas.SetLeft(drag_app, currentdragcolumn * Columnwidth + timeLineControlWidth + 4 - (adjustCanvas));
                if (DaysScrollviewer != null)
                {
                    var selectedDate = sfSchedule.GetCurrentDropLocation(this, e);
                    DateTime EndDate;
                    if (DatetimeDiff.Equals(0))
                    {
                        DatetimeDiff = (sfSchedule.dvc.ActualHeight) / IntervalHeight;
                    }
                    EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                    if (selectedDate == EndDate)
                    {
                        EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                    }
                    var dropappointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                    if (dropappointment != null)
                    {
                        if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                        {
                            sfSchedule.AddResources(dropappointment);
                        }
                        if (!sfSchedule.allDayFlag)
                        {
                            dropappointment.StartTime = selectedDate;
                            dropappointment.EndTime = EndDate;
                            dropappointment.AllDay = false;
                        }
                        else
                        {
                            dropappointment.StartTime = selectedDate;
                            dropappointment.EndTime = selectedDate;
                            dropappointment.AllDay = true;
                            sfSchedule.DragDropCanvas.Children.Clear();
                        }
                    }
                    DaysScrollviewer.VerticalScrollMode = ScrollMode.Auto;
                }
                sfSchedule.IsResizeEnabled = true;
                DatetimeDiff = DayDiff = 0;
                sfSchedule.dvc = null;
            }
            if (sfSchedule.dvc != null)
            {
                #region DragDropTimer
                if (!dayCanvasInAllday)
                {
                    sfSchedule.dvc = (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleDaysAppointmentViewControl>();
                    sfSchedule.SelectedAppointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                    DateTime selectedDate1;
                    var mainitem1 = (sfSchedule.flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
                    selectedDate1 = sfSchedule.GetCurrentDropLocation(mainitem1, e);//.AddTimeSpan(SelectedAppointment.InternalStartTime.TimeOfDay); ;
                    DateTime EndDate;
                    {
                        DatetimeDiff = (sfSchedule.dvc.ActualHeight) / IntervalHeight;
                    }
                    EndDate = selectedDate1.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                    if (!(dragDropCanvasHeight == sfSchedule.dvc.ActualHeight))
                    {
                        double diffHeight = dragDropCanvasHeight - sfSchedule.dvc.ActualHeight;
                        double diffTime = diffHeight / IntervalHeight;
                        if (dragResizeFlag)
                        {
                            selectedDate1 = selectedDate1.AddMinutes(-(sfSchedule.GetTimeInterval().TotalMinutes * diffTime));
                        }
                        else
                        {
                            EndDate = EndDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * diffTime);
                        }
                    }
                    sfSchedule.dvc.DragDropStartTime = selectedDate1.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                    sfSchedule.dvc.DragDropEndTime = EndDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                }
                #endregion

                #region DayViewControl

                sfSchedule.isscrollmoveondragging = true;
                if (DaysScrollviewer != null)
                {
                    double CurrentPositionInScroll = e.GetCurrentPoint(DaysScrollviewer).Position.Y;
                    double CurrentPositionInCanvas = e.GetCurrentPoint(sfSchedule.DragDropCanvas.Children[0]).Position.Y;
                    double pos = e.GetCurrentPoint(DaysScrollviewer).Position.X;
                    if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null)
                    {
                        if (pos + 40 > DaysScrollviewer.ViewportWidth)
                        {
#if SyncfusionFramework4_5_11
                            DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset + 10, null, null);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));

#else
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#endif
                        }

                        else if (pos - 40 < 0)
                        {
#if SyncfusionFramework4_5_11
                            DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset - 10, null, null);//+ pos - AllDayviewscroll.ViewportWidth);
#else
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
#endif
                        }
                    }
                    else
                    {
                        if (pos + drag_app.ActualWidth > DaysScrollviewer.ViewportWidth)
                        {
#if SyncfusionFramework4_5_11
                            DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset + 10, null, null);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#else
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#endif
                        }
                        else if (pos - drag_app.ActualWidth < 0)
                        {
#if SyncfusionFramework4_5_11
                            DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset - 10, null, null);//+ pos - AllDayviewscroll.ViewportWidth);
#else
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
#endif
                        }
                    }
                    if (CurrentPositionInScroll >= 0 && dayCanvasInAllday)
                    {
                        sfSchedule.Dayviewrb.Attach((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).FindElementOfType<Grid>());
                        dayCanvasInAllday = false;
                        dayCanvasFromAllday = true;
                    }
                    if (CurrentPositionInScroll >= 0
                        && CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight < DaysScrollviewer.ViewportHeight)
                    {
                        if (CurrentPositionInScroll - IntervalHeight < 0 && DaysScrollviewer.VerticalOffset > 0)
                        {
#if SyncfusionFramework4_5_11
                            DaysScrollviewer.ChangeView(null, DaysScrollviewer.VerticalOffset - 20, null);//+ pos - AllDayviewscroll.ViewportWidth);
#else
                            DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset - 20);//+ pos - AllDayviewscroll.ViewportWidth);
#endif
                        }
                        else if (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Position.Y <= 0)
                        {
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Position.Y);
                            Canvas.SetTop(drag_app, sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                            dragResizeFlag = true;
                        }
                        else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                        {
                            double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                            previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                            Canvas.SetTop(drag_app, previousYValue);
                            dragResizeFlag = false;
                        }
                        else
                        {
                            previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                            if (CurrentPositionInCanvas == CurrentPositionInScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                            {
                                double diffHeight = previousYValue - (sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                                Canvas.SetTop(drag_app, sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                            }
                            else
                            {
                                Canvas.SetTop(drag_app, previousYValue);
                            }
                        }
                    }
                    else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                    {
                        if (CurrentPositionInScroll + IntervalHeight > DaysScrollviewer.ViewportHeight && DaysScrollviewer.VerticalOffset < DaysScrollviewer.ExtentHeight - DaysScrollviewer.ViewportHeight)
                        {
#if SyncfusionFramework4_5_11
                            DaysScrollviewer.ChangeView(null, DaysScrollviewer.VerticalOffset + 20, null);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#else
                            DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset + 20);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#endif
                        }
                        else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                        {
                            double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                            previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                            if (CurrentPositionInCanvas == CurrentPositionInScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                            {
                                diffHeight = previousYValue - (sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                            }
                            Canvas.SetTop(drag_app, previousYValue);
                            dragResizeFlag = false;
                        }
                    }
                    else if (sfSchedule.DragDropCanvas.Children.Count > 0 && !dayCanvasInAllday && CurrentPositionInScroll < 0 && DaysScrollviewer.VerticalOffset == 0)
                    {
                        dayAppCanvasHeight = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height;
                        (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = 30;
                        Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
                        sfSchedule.Dayviewrb.Detach();
                        sfSchedule.dvc.DragDropStartTime = null;
                        sfSchedule.dvc.DragDropEndTime = null;
                        dayCanvasInAllday = true;
                    }
                }

                Columnwidth = HrTimeSlotItem.ActualWidth / (sfSchedule.CurrentSelectedDates.Count * resourcecount);
                remain = scrollOffset % Columnwidth;
                if (remain > 1)
                {
                    adjustCanvas = remain - Columnwidth;
                }
                var currentdragcolumn = (int)((xPos - timeLineControlWidth - 4 + adjustCanvas) / Columnwidth);
                if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null && SelectedDates.Count == 1)
                {
                    Canvas.SetLeft(drag_app, xPos - 10);
                    Canvas.SetTop(drag_app, e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - 10);
                }
                else
                {
                    if (currentdragcolumn * Columnwidth + timeLineControlWidth + 4 < scrollviewer.ViewportWidth - Columnwidth)
                        Canvas.SetLeft(drag_app, currentdragcolumn * Columnwidth + timeLineControlWidth + 4 - (adjustCanvas));
                }
                #endregion
            }
            else
                sfSchedule.isscrollmoveondragging = false;
        }
#else
        internal void MoveDragDrop(SfSchedule sfSchedule, bool enableDrag, MouseEventArgs e)
        {
            bool containitems = sfSchedule.CheckItemsInItemsSource();
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;

            #region DragDrop
            if (enableDrag && obj != null)
            {
                var dropappointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                if (dropappointment != null)
                {
                    TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                    var selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                    DatetimeDiff = (sfSchedule.dvc.ActualHeight) / IntervalHeight;
                    DateTime EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                    if (selectedDate == EndDate)
                    {
                        EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                    }
                    if (dropappointment.InternalEndTime.Date.Date > dropappointment.InternalStartTime.Date.Date)
                    {
                        EndDate = selectedDate.Add(datetimeDiff);
                        var scheduleDaysAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl;
                        if (scheduleDaysAppointmentViewControl != null)
                        {
                            double height;
                            if (EndDate.Date > dropappointment.InternalStartTime.Date.Date &&
                                selectedDate.Date == dropappointment.InternalStartTime.Date.Date)
                            {
                                TimeSpan timeHeight = selectedDate.Date.AddHours(24) - selectedDate;
                                height = (timeHeight.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * IntervalHeight;
                            }
                            else
                            {
                                TimeSpan timeHeigth = EndDate - selectedDate;
                                height = (timeHeigth.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * IntervalHeight;
                            }
                            scheduleDaysAppointmentViewControl.Height = height;
                        }
                    }
                    if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                    {
                        sfSchedule.AddResources(dropappointment);
                    }
                    DateTime daystarttime, dayendtime;
                    if (!sfSchedule.allDayFlag)
                    {
                        daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                        if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                        {
                            bool ispropertyset = false;
                            foreach (object dayobj in ((IEnumerable)sfSchedule.ItemsSource))
                            {
                                Type type = dayobj.GetType();
                                if (dayobj.GetHashCode() == (int)dropappointment.ObjectID)
                                {
                                    type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                    type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                dropappointment.StartTime = daystarttime;
                                dropappointment.EndTime = dayendtime;
                            }
                        }
                        else
                        {
                            dropappointment.StartTime = daystarttime;
                            dropappointment.EndTime = dayendtime;
                        }
                        dropappointment.AllDay = false;
                    }
                    else
                    {
                        daystarttime = dropappointment.StartTime == ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null) ? ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null).AddTicks(1) : ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        dayendtime = dropappointment.EndTime == dropappointment.StartTime ? dropappointment.StartTime.AddTicks(1) : dropappointment.StartTime;
                        if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                        {
                            bool ispropertyset = false;
                            foreach (object dayobj in ((IEnumerable)sfSchedule.ItemsSource))
                            {
                                Type type = dayobj.GetType();
                                if (dayobj.GetHashCode() == (int)dropappointment.ObjectID)
                                {
                                    type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                    type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                dropappointment.StartTime = daystarttime;
                                dropappointment.EndTime = dayendtime;
                            }
                        }
                        else
                        {
                            dropappointment.StartTime = daystarttime;
                            dropappointment.EndTime = dayendtime;
                        }
                        dropappointment.AllDay = true;
                        sfSchedule.DragDropCanvas.Children.Clear();
                    }
                    dayCanvasInAllday = false;
                }
                sfSchedule.IsResizeEnabled = true;
                DatetimeDiff = DayDiff = 0;
                sfSchedule.dvc = null;
                if ((sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null) || ((sfSchedule.ScheduleType == ScheduleType.WorkWeek ||sfSchedule.ScheduleType == ScheduleType.Week || sfSchedule.ScheduleType == ScheduleType.Day) && dayCanvasFromAllday))
                {
                    sfSchedule.DragDropCanvas.Children.Clear();
                    dayCanvasFromAllday = false;
                }
            }

            #endregion

            if (sfSchedule.dvc != null)
            {
        #region DragDropTimer
                if (!dayCanvasInAllday)
                {
                    sfSchedule.dvc = (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleDaysAppointmentViewControl>();
                    sfSchedule.SelectedAppointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                    DateTime selectedDate1;
                    selectedDate1 = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                    DateTime EndDate;
                    {
                        DatetimeDiff = (sfSchedule.dvc.ActualHeight) / IntervalHeight;
                    }
                    EndDate = selectedDate1.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                    if (!(dragDropCanvasHeight == sfSchedule.dvc.ActualHeight))
                    {
                        double diffHeight = dragDropCanvasHeight - sfSchedule.dvc.ActualHeight;
                        double diffTime = diffHeight / IntervalHeight;
                        if (dragResizeFlag)
                        {
                            selectedDate1 = selectedDate1.AddMinutes(-(sfSchedule.GetTimeInterval().TotalMinutes * diffTime));
                        }
                        else
                        {
                            EndDate = EndDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * diffTime);
                        }
                    }
                    sfSchedule.dvc.DragDropStartTime = selectedDate1.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                    sfSchedule.dvc.DragDropEndTime = EndDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                }
                #endregion

        #region DayViewControl
                sfSchedule.isscrollmoveondragging = true;
                int resourcecount = sfSchedule.CalculateLeafCount(ScheduleResourceType);
                var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl);
                if (obj != null)
                {
                    var DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
                    if (DaysScrollviewer != null)
                    {
                        double CurrentPositionInScroll = e.GetPosition(DaysScrollviewer).Y;
                        double CurrentPositionInCanvas = e.GetPosition(sfSchedule.DragDropCanvas.Children[0]).Y;
                        double pos = e.GetPosition(DaysScrollviewer).X;
                        if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null)
                        {
                            if (pos + 40 > DaysScrollviewer.ViewportWidth)
                            {
                                DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                            }

                            else if (pos - 40 < 0)
                            {
                                DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
                            }
                        }
                        else if (drag_app != null)
                        {
                            if (pos + drag_app.ActualWidth > DaysScrollviewer.ViewportWidth)
                            {
                                DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                            }

                            else if (pos - drag_app.ActualWidth < 0)
                            {
                                DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
                            }
                        }
                        if (CurrentPositionInScroll >= 0 && dayCanvasInAllday && drag_app != null)
                        {
                            drag_app.Height = dayAppCanvasHeight;
                            sfSchedule.Dayviewrb.Attach(drag_app.FindElementOfType<Grid>());
                            dayCanvasInAllday = false;
                        }
                        if (CurrentPositionInScroll >= 0
                        && CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight < DaysScrollviewer.ViewportHeight)
                        {
                            if (CurrentPositionInScroll - IntervalHeight < 0 && DaysScrollviewer.VerticalOffset > 0)
                            {
                                DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset - 20);//+ pos - AllDayviewscroll.ViewportWidth);
                            }
                            else if (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Y <= 0)
                            {
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Y);
                                Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                dragResizeFlag = true;
                            }
                            else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                            {
                                double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                                previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                                Canvas.SetTop(drag_app, previousYValue);
                                dragResizeFlag = false;
                            }
                            else
                            {
                                previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                                if (CurrentPositionInCanvas == CurrentPositionInScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                                {
                                    double diffHeight = previousYValue - (sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                                    Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                }
                                else
                                {
                                    Canvas.SetTop(drag_app, previousYValue);
                                }
                            }
                        }
                        //if (CurrentPositionInScroll + 40 > DaysScrollviewer.ViewportHeight && DaysScrollviewer.VerticalOffset < DaysScrollviewer.ExtentHeight - DaysScrollviewer.ViewportHeight)
                        //{
                        //    DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset + 20);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                        //}
                        else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                        {
                            if (CurrentPositionInScroll + IntervalHeight > DaysScrollviewer.ViewportHeight && DaysScrollviewer.VerticalOffset < DaysScrollviewer.ExtentHeight - DaysScrollviewer.ViewportHeight)
                            {
                                DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset + 20);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                            }
                            else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                            {
                                double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                                previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                                if (CurrentPositionInCanvas == CurrentPositionInScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                                {
                                    diffHeight = previousYValue - (sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                                }
                                Canvas.SetTop(drag_app, previousYValue);
                                dragResizeFlag = false;
                            }
                        }
                        //else if (CurrentPositionInScroll - 40 < 0 && DaysScrollviewer.VerticalOffset > 0)
                        //{
                        //    DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset - 20);//+ pos - AllDayviewscroll.ViewportWidth);
                        //}
                        else if (sfSchedule.DragDropCanvas.Children.Count > 0 && !dayCanvasInAllday && CurrentPositionInScroll < 0 && DaysScrollviewer.VerticalOffset == 0)
                        {
#if WPF
                            var horScrollBar = DaysScrollviewer.FindElementOfType<Grid>().FindName("PART_HorizontalScrollBar") as ScrollBar;
#endif
                            dayAppCanvasHeight = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height;
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = 30;
#if SILVERLIGHT
                                    //Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
                                    Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
#else
                            //Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 32 - horScrollBar.ActualHeight);
                            Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 32);
#endif
                                    //Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
                            sfSchedule.Dayviewrb.Detach();
                            sfSchedule.dvc.DragDropStartTime = null;
                            sfSchedule.dvc.DragDropEndTime = null;
                            dayCanvasInAllday = true;
                        }
//                        else if (drag_app != null)
//                        {
//#if SILVERLIGHT
//                            if (CurrentPositionInScroll >= 0)
//#else
//                            var horScrollBar = DaysScrollviewer.FindElementOfType<Grid>().FindName("PART_HorizontalScrollBar") as ScrollBar;
//                            if (horScrollBar != null && (CurrentPositionInScroll >= 0 && CurrentPositionInScroll <= DaysScrollviewer.ViewportHeight - horScrollBar.ActualHeight))
//#endif
//                            {
//                                previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
//                                Canvas.SetTop(drag_app, previousYValue);
//                            }
//                            else
//                            {
//#if SILVERLIGHT
//                                if (sfSchedule.DragDropCanvas.Children.Count > 0 && !dayCanvasInAllday)
//#else
//                                if (horScrollBar != null && (sfSchedule.DragDropCanvas.Children.Count > 0 && CurrentPositionInScroll <= DaysScrollviewer.ViewportHeight - horScrollBar.ActualHeight))
//#endif
//                                {
//                                    dayAppCanvasHeight = drag_app.Height;
//                                    drag_app.Height = 30;
//#if SILVERLIGHT
//                                    Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
//#else
//                                    Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 32 - horScrollBar.ActualHeight);
//#endif
//                                    sfSchedule.Dayviewrb.Detach();
//                                    sfSchedule.dvc.DragDropStartTime = null;
//                                    sfSchedule.dvc.DragDropEndTime = null;
//                                    dayCanvasInAllday = true;
//                                }
//                            }
//                        }
                    }
                }
                var HrTimeSlotItem = (sfSchedule.currentSelectedItem.Content as ScheduleDaysView).FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
                var timeLineControlWidth = (sfSchedule.currentSelectedItem.Content as ScheduleDaysView).FindElementOfType<ScheduleTimeLineItemsControl>().ActualWidth;
                double Columnwidth = HrTimeSlotItem.ActualWidth / (sfSchedule.CurrentSelectedDates.Count * resourcecount);
                double xPos = e.GetPosition(sfSchedule.DragDropCanvas).X;
                double scrollOffset = scrollviewer.HorizontalOffset;
                double remain = scrollOffset % Columnwidth;
                double adjustCanvas = 0d;
                if (remain > 1)
                {
                    adjustCanvas = remain - Columnwidth;
                }
                var currentdragcolumn = (int)((xPos - timeLineControlWidth - 4 + adjustCanvas) / Columnwidth);
                if (drag_app != null)
                {
                    if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null)
                    {
                        Canvas.SetLeft(drag_app, xPos - 10);
                        Canvas.SetTop(drag_app, e.GetPosition(sfSchedule.DragDropCanvas).Y - 10);
                    }
                    else
                    {
                        Canvas.SetLeft(drag_app, currentdragcolumn * Columnwidth + timeLineControlWidth - (adjustCanvas) + 2);
                    }
                }

                #endregion
            }
            else
                sfSchedule.isscrollmoveondragging = false;
        }
#endif
        #endregion

        #region Start Dragging Appointment

#if WINRT
        internal void StartDragDrop(SfSchedule sfSchedule, bool enableDrag, PointerRoutedEventArgs e)
        {
            var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl);
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            ScrollViewer DaysScrollviewer = null;
            if (obj != null)
                DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
            int resourcecount = sfSchedule.CalculateLeafCount(ScheduleResourceType);
            var HrTimeSlotItem = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
            double Columnwidth = HrTimeSlotItem.ActualWidth / (sfSchedule.SelectedDates.Count * resourcecount);
            var timeLineControlWidth = this.FindElementOfType<ScheduleTimeLineItemsControl>().ActualWidth;
            double xPos = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X;
            double scrollOffset = scrollviewer.HorizontalOffset;
            double remain = scrollOffset % Columnwidth;
            double adjustCanvas = 0d;
            bool containitems = sfSchedule.CheckItemsInItemsSource();
            if (remain > 1)
            {
                adjustCanvas = remain - Columnwidth;
            }
            var currentdragcolumn = (int)((xPos - timeLineControlWidth - 4 + (adjustCanvas)) / Columnwidth);

            #region DragDrop
            if (enableDrag)
            {
                Canvas.SetLeft(drag_app, currentdragcolumn * Columnwidth + timeLineControlWidth + 4 - (adjustCanvas));
                var mainitem = (sfSchedule.flipviewselecteditem).FindElementOfType<ContentControl>();
                while (mainitem is FlipViewItem)
                {
                    var subelement = mainitem.FindElementOfType<Grid>();
                    mainitem = subelement.FindElementOfType<ContentControl>();
                }

                if (DaysScrollviewer != null)
                {
                    if (DatetimeDiff.Equals(0))
                    {
                        DatetimeDiff = (sfSchedule.dvc.ActualHeight + 2) / IntervalHeight;
                    }
                    var selectedDate = sfSchedule.GetCurrentDropLocation(mainitem, e);
                    var EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                    if (selectedDate == EndDate)
                    {
                        EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                    }
                    var dropappointment = sfSchedule.dvc.DataContext as ScheduleAppointment;
                    if (dropappointment != null)
                    {
                        if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.selectedResourcename[0].ResourceName != null && sfSchedule.Resource != string.Empty)
                        {
                            sfSchedule.AddSelectedResource(dropappointment, sfSchedule.selectedResourcename[0].ResourceName);
                        }
                        DateTime daystarttime, dayendtime;
                        if (!sfSchedule.allDayFlag)
                        {
                            daystarttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                            dayendtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                            if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                            {
                                bool ispropertyset = false;
                                var enumerable = sfSchedule.ItemsSource as IEnumerable;
                                if (enumerable != null)
                                    foreach (object dayobj in enumerable)
                                    {
                                        Type type = dayobj.GetType();
                                        if (dayobj.GetHashCode() == (int)dropappointment.ObjectID)
                                        {
                                            type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                            type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                            ispropertyset = true;
                                            break;
                                        }
                                    }
                                if (!ispropertyset)
                                {
                                    dropappointment.StartTime = daystarttime;
                                    dropappointment.EndTime = dayendtime;
                                }
                            }
                            else
                            {
                                dropappointment.StartTime = daystarttime;
                                dropappointment.EndTime = dayendtime;
                            }
                            dropappointment.AllDay = false;
                        }
                        else
                        {
                            daystarttime = selectedDate;
                            dayendtime = selectedDate;
                            if (sfSchedule.ItemsSource is IEnumerable && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                            {
                                bool ispropertyset = false;
                                foreach (object dayobj in (sfSchedule.ItemsSource as IEnumerable))
                                {
                                    Type type = dayobj.GetType();
                                    if (dayobj.GetHashCode() == (int)dropappointment.ObjectID)
                                    {
                                        type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(dayobj, daystarttime, null);
                                        type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(dayobj, dayendtime, null);
                                        ispropertyset = true;
                                        break;
                                    }
                                }
                                if (!ispropertyset)
                                {
                                    dropappointment.StartTime = daystarttime;
                                    dropappointment.EndTime = dayendtime;
                                }
                            }
                            else
                            {
                                dropappointment.StartTime = daystarttime;
                                dropappointment.EndTime = dayendtime;
                            }
                            dropappointment.AllDay = true;
                            sfSchedule.DragDropCanvas.Children.Clear();
                        }
                    }
                    DaysScrollviewer.VerticalScrollMode = ScrollMode.Auto;
                }
                sfSchedule.IsResizeEnabled = true;
                DatetimeDiff = DayDiff = 0;
                sfSchedule.dvc = null;
                dayCanvasInAllday = false;
            }
            #endregion

            #region DayViewControl

            sfSchedule.isscrollmoveondragging = true;
            if (DaysScrollviewer != null)
            {
                double CurrentPositionInScroll = e.GetCurrentPoint(DaysScrollviewer).Position.Y;
                double CurrentPositionInCanvas = e.GetCurrentPoint(sfSchedule.DragDropCanvas.Children[0]).Position.Y;
                double pos = e.GetCurrentPoint(DaysScrollviewer).Position.X;
                if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null)
                {
                    if (pos + 40 > DaysScrollviewer.ViewportWidth)
                    {
#if SyncfusionFramework4_5_11
                        DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset + 10, null, null);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#else
                        DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#endif
                    }

                    else if (pos - 40 < 0)
                    {
#if SyncfusionFramework4_5_11
                        DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset - 10, null, null);//+ pos - AllDayviewscroll.ViewportWidth);
#else
                        DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
#endif
                    }
                }
                else
                {
                    if (pos + drag_app.ActualWidth > DaysScrollviewer.ViewportWidth)
                    {
#if SyncfusionFramework4_5_11
                        DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset + 10, null, null);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#else
                        DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#endif
                    }

                    else if (pos - drag_app.ActualWidth < 0)
                    {
#if SyncfusionFramework4_5_11
                        DaysScrollviewer.ChangeView(DaysScrollviewer.HorizontalOffset - 10, null, null);//+ pos - AllDayviewscroll.ViewportWidth);
#else
                        DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
#endif
                    }
                }
                if (CurrentPositionInScroll >= 0 && dayCanvasInAllday)
                {
                    sfSchedule.Dayviewrb.Attach((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).FindElementOfType<Grid>());
                    dayCanvasInAllday = false;
                    dayCanvasFromAllday = true;
                }
                if (CurrentPositionInScroll >= 0
                    && CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight < DaysScrollviewer.ViewportHeight)
                {
                    if (CurrentPositionInScroll - IntervalHeight < 0 && DaysScrollviewer.VerticalOffset > 0)
                    {
#if SyncfusionFramework4_5_11
                        DaysScrollviewer.ChangeView(null, DaysScrollviewer.VerticalOffset - 20, null);//+ pos - AllDayviewscroll.ViewportWidth);
#else
                        DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset - 20);//+ pos - AllDayviewscroll.ViewportWidth);
#endif
                    }
                    else if (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Position.Y <= 0)
                    {
                        (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Position.Y);
                        Canvas.SetTop(drag_app, sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                        dragResizeFlag = true;
                    }
                    else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                    {
                        double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                        (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                        previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                        Canvas.SetTop(drag_app, previousYValue);
                        dragResizeFlag = false;
                    }
                    else
                    {
                        previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                        if (CurrentPositionInCanvas == CurrentPositionInScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                        {
                            double diffHeight = previousYValue - (sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                            Canvas.SetTop(drag_app, sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                        }
                        else
                        {
                            Canvas.SetTop(drag_app, previousYValue);
                        }
                    }
                }
                else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                {
                    if (CurrentPositionInScroll + IntervalHeight > DaysScrollviewer.ViewportHeight && DaysScrollviewer.VerticalOffset < DaysScrollviewer.ExtentHeight - DaysScrollviewer.ViewportHeight)
                    {
#if SyncfusionFramework4_5_11
                        DaysScrollviewer.ChangeView(null, DaysScrollviewer.VerticalOffset + 20, null);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#else
                        DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset + 20);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
#endif
                    }
                    else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                    {
                        double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                        (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                        previousYValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - sfSchedule.Appointmentpoint.Position.Y;
                        if (CurrentPositionInCanvas == CurrentPositionInScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                        {
                            diffHeight = previousYValue - (sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                        }
                        Canvas.SetTop(drag_app, previousYValue);
                        dragResizeFlag = false;
                    }
                }
                else if (sfSchedule.DragDropCanvas.Children.Count > 0 && !dayCanvasInAllday && CurrentPositionInScroll < 0 && DaysScrollviewer.VerticalOffset == 0)
                {
                    dayAppCanvasHeight = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height;
                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = 30;
                    Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
                    sfSchedule.Dayviewrb.Detach();
                    sfSchedule.dvc.DragDropStartTime = null;
                    sfSchedule.dvc.DragDropEndTime = null;
                    dayCanvasInAllday = true;
                }
            }

            if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null && SelectedDates.Count == 1)
            {
                Canvas.SetLeft(drag_app, xPos - 10);
                Canvas.SetTop(drag_app, e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - 10);
            }
            else if (currentdragcolumn * Columnwidth + timeLineControlWidth + 4 < (this).scrollviewer.ViewportWidth)
            {
                Canvas.SetLeft(drag_app, currentdragcolumn * Columnwidth + timeLineControlWidth + 4 - (adjustCanvas));
            }
            if (sfSchedule.Dayviewrb.EllipseAnimation1 != null)
            {
                sfSchedule.Dayviewrb.EllipseAnimation1.Begin();
                sfSchedule.Dayviewrb.EllipseAnimation2.Begin();
            }
            #endregion
        }
#else
        internal void StartDragDrop(SfSchedule sfSchedule, MouseEventArgs e)
        {
            var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl);
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            if (obj != null)
            {
                var DaysScrollviewer = obj.FindName("Scrollviewer") as ScrollViewer;
                if (DaysScrollviewer != null)
                {
                    double CurrentPositionInScroll = e.GetPosition(DaysScrollviewer).Y;
                    double CurrentPositionInCanvas = e.GetPosition(sfSchedule.DragDropCanvas.Children[0]).Y;
                    double pos = e.GetPosition(DaysScrollviewer).X;
                    if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null)
                    {
                        if (pos + 40 > DaysScrollviewer.ViewportWidth)
                        {
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                        }
                        else if (pos - 40 < 0)
                        {
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
                        }
                    }
                    else if (drag_app != null)
                    {
                        if (pos + drag_app.ActualWidth > DaysScrollviewer.ViewportWidth)
                        {
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset + 10);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                        }

                        else if (pos - drag_app.ActualWidth < 0)
                        {
                            DaysScrollviewer.ScrollToHorizontalOffset(DaysScrollviewer.HorizontalOffset - 10);//+ pos - AllDayviewscroll.ViewportWidth);
                        }
                    }
                    if (CurrentPositionInScroll >= 0 && dayCanvasInAllday)
                    {
                        sfSchedule.Dayviewrb.Attach((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).FindElementOfType<Grid>());
                        dayCanvasInAllday = false;
                        dayCanvasFromAllday = true;
                    }
                    //if (CurrentPositionInScroll >= 0 && dayCanvasInAllday && sfSchedule.DragDropCanvas.Children.Count > 0 && drag_app != null)
                    //{
                    //    drag_app.Height = dayAppCanvasHeight;
                    //    var gr = drag_app.FindElementOfType<Grid>();
                    //    if (gr != null)
                    //        sfSchedule.Dayviewrb.Attach(gr);
                    //    dayCanvasInAllday = false;
                    //}
                    if (CurrentPositionInScroll >= 0
                        && CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight < DaysScrollviewer.ViewportHeight)
                    {
                        if (CurrentPositionInScroll - IntervalHeight < 0 && DaysScrollviewer.VerticalOffset > 0)
                        {
                            DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset - 20);//+ pos - AllDayviewscroll.ViewportWidth);
                        }
                        else if (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Y <= 0)
                        {
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + (CurrentPositionInScroll - sfSchedule.Appointmentpoint.Y);
                            Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                            dragResizeFlag = true;
                        }
                        else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                        {
                            double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                            previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                            Canvas.SetTop(drag_app, previousYValue);
                            dragResizeFlag = false;
                        }
                        else
                        {
                            previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                            if ((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                            {
                                double diffHeight = previousYValue - (sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                                Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                            }
                            else
                            {
                                Canvas.SetTop(drag_app, previousYValue);
                            }
                        }
                    }
                    //if (CurrentPositionInScroll + 40 > DaysScrollviewer.ViewportHeight && DaysScrollviewer.VerticalOffset < DaysScrollviewer.ExtentHeight - DaysScrollviewer.ViewportHeight)
                    //{
                    //    DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset + 20);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                    //}
                    else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                    {
                        if (CurrentPositionInScroll + IntervalHeight > DaysScrollviewer.ViewportHeight && DaysScrollviewer.VerticalOffset < DaysScrollviewer.ExtentHeight - DaysScrollviewer.ViewportHeight)
                        {
                            DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset + 20);//(pos + drag_app.ActualWidth - DaysScrollviewer.ViewportWidth));
                        }
                        else if (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight >= DaysScrollviewer.ViewportHeight && CurrentPositionInScroll < DaysScrollviewer.ViewportHeight)
                        {
                            double diffHeight = DaysScrollviewer.ViewportHeight - (CurrentPositionInScroll - CurrentPositionInCanvas + dragDropCanvasHeight);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = dragDropCanvasHeight + diffHeight;
                            previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
                            if (CurrentPositionInCanvas == CurrentPositionInScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height < dragDropCanvasHeight)
                            {
                                diffHeight = previousYValue - (sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 5);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = visibleCanvasHeight + diffHeight;
                            }
                            Canvas.SetTop(drag_app, previousYValue);
                            dragResizeFlag = false;
                        }
                    }
                    //else if (CurrentPositionInScroll - 40 < 0 && DaysScrollviewer.VerticalOffset > 0)
                    //{
                    //    DaysScrollviewer.ScrollToVerticalOffset(DaysScrollviewer.VerticalOffset - 20);//+ pos - AllDayviewscroll.ViewportWidth);
                    //}
                    else if (sfSchedule.DragDropCanvas.Children.Count > 0 && !dayCanvasInAllday && CurrentPositionInScroll < 0 && DaysScrollviewer.VerticalOffset == 0)
                    {
                        dayAppCanvasHeight = (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height;
                        (sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl).Height = 30;
#if SILVERLIGHT
                        //Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
                        Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
#else
                        Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 32 );
                                //Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 32 - horScrollBar.ActualHeight);
#endif
                        //Canvas.SetTop((sfSchedule.DragDropCanvas.Children[0] as ScheduleDaysAppointmentViewControl), sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
                        sfSchedule.Dayviewrb.Detach();
                        sfSchedule.dvc.DragDropStartTime = null;
                        sfSchedule.dvc.DragDropEndTime = null;
                        dayCanvasInAllday = true;
                    }
//                    else if (drag_app != null)
//                    {
//#if SILVERLIGHT
//                        if (CurrentPositionInScroll >= 0)
//#else
//                        var horScrollBar = DaysScrollviewer.FindElementOfType<Grid>().FindName("PART_HorizontalScrollBar") as ScrollBar;
//                        if (horScrollBar != null && (CurrentPositionInScroll >= 0 && CurrentPositionInScroll <= DaysScrollviewer.ViewportHeight - horScrollBar.ActualHeight))
//#endif
//                        {
//                            previousYValue = e.GetPosition(sfSchedule.DragDropCanvas).Y - sfSchedule.Appointmentpoint.Y;
//                            Canvas.SetTop(drag_app, previousYValue);
//                        }
//                        else
//                        {
//#if SILVERLIGHT
//                            if (sfSchedule.DragDropCanvas.Children.Count > 0 && !dayCanvasInAllday)
//#else
//                            if (horScrollBar != null && (sfSchedule.DragDropCanvas.Children.Count > 0 && CurrentPositionInScroll <= DaysScrollviewer.ViewportHeight - horScrollBar.ActualHeight))
//#endif
//                            {
//                                dayAppCanvasHeight = drag_app.Height;
//                                drag_app.Height = 30;
//#if SILVERLIGHT
//                                Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 35);
//#else
//                                Canvas.SetTop(drag_app, sfSchedule.DragDropCanvas.ActualHeight - DaysScrollviewer.ViewportHeight - 32 - horScrollBar.ActualHeight);
//#endif
//                                sfSchedule.Dayviewrb.Detach();
//                                drag_app.MouseLeftButtonDown += sfSchedule.drag_app_MouseLeftButtonDown;
//                                dayCanvasInAllday = true;
//                            }
//                        }
//                    }
                }
            }
            int resourcecount = sfSchedule.CalculateLeafCount(ScheduleResourceType);
            var HrTimeSlotItem = (this).FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
            var timeLineControlWidth = (this).FindElementOfType<ScheduleTimeLineItemsControl>().ActualWidth;
            double Columnwidth = HrTimeSlotItem.ActualWidth / (sfSchedule.CurrentSelectedDates.Count * resourcecount);
            double xPos = e.GetPosition(sfSchedule.DragDropCanvas).X;
            double scrollOffset = (this).scrollviewer.HorizontalOffset;
            double remain = scrollOffset % Columnwidth;
            double adjustCanvas = 0d;
            if (remain > 1)
            {
                adjustCanvas = remain - Columnwidth;
            }
            var currentdragcolumn = (int)((xPos - timeLineControlWidth - 4 + adjustCanvas) / Columnwidth);
            if (drag_app != null)
            {
                if (sfSchedule.ScheduleType == ScheduleType.Day && ScheduleResourceType != null)
                {
                    Canvas.SetLeft(drag_app, xPos - 10);
                    Canvas.SetTop(drag_app, e.GetPosition(sfSchedule.DragDropCanvas).Y - 10);
                }
                else
                {
                    Canvas.SetLeft(drag_app, currentdragcolumn * Columnwidth + timeLineControlWidth - (adjustCanvas) + 2);
                }
            }
        }
#endif

        #endregion

        public void Dispose()
        {
            if (alldaysAppointmentsLayout.Items != null)
            {
                //foreach (AllDayAppointmentItemscontrol childitem in alldaysAppointmentsLayout.Items)
                //{
                //    if (childitem.Items != null)
                //        childitem.Items.Clear();
                //}
                alldaysAppointmentsLayout.Items.Clear();
            }
            if (daysAppointmentsLayout.Items != null)
            {
                // foreach (ScheduleDaysAppointmentViewControl appview in daysAppointmentsLayout.Items)
                //foreach (ScheduleAppointment app in daysAppointmentsLayout.Items)
                //{
                //    Appointments.Clear();
                //    Appointments = null;
                //}
                daysAppointmentsLayout.Items.Clear();
            }
            if (TodayTimer != null)
            {
                TodayTimer.Tick -= TodayTimer_Tick;
            }
            Loaded -= ScheduleDaysView_Loaded;
            if (VisibleAppointments != null)
            {
                foreach (ScheduleAppointment app in VisibleAppointments)
                {
                    app.PropertyChanged -= app_PropertyChanged;
                }
                VisibleAppointments.Clear();
                VisibleAppointments = null;
            }
#if WPF
            SizeChanged -= ScheduleDaysView_SizeChanged;
#endif
#if SILVERLIGHT
            if(timelinescrollviewer!= null)
            {
                timelinescrollviewer.GotFocus -= scrollviewer_GotFocus;
            }
            SizeChanged -= ScheduleDaysView_SizeChanged;
            if(scrollviewer != null)
                scrollviewer.GotFocus -= scrollviewer_GotFocus;
#endif
#if !WINRT
            if (scheduleDaysHeaderViewItemsControl != null)
                scheduleDaysHeaderViewItemsControl.SizeChanged -= scheduleDaysHeaderViewItemsControl_SizeChanged;
            if (appTextBox != null)
                appTextBox.LostFocus -= appTextBox_LostFocus;
#endif

#if WINRT
            if (scrollviewer != null) scrollviewer.ViewChanged -= scrollviewer_ViewChanged;
            if (timelinescrollviewer != null) timelinescrollviewer.ViewChanged -= timelinescrollviewer_ViewChanged;
            if (headerscrollviewer != null) headerscrollviewer.ViewChanged -= headerscrollviewer_ViewChanged;
#elif WPF
            if (scrollviewer != null)
            {

                scrollviewer.PreviewMouseWheel -= scrollviewer_MouseWheel;
                scrollviewer.ScrollChanged -= scrollviewer_ScrollChanged;
            }
            if (timelinescrollviewer != null)
            {
                timelinescrollviewer.PreviewMouseWheel -= timelinescrollviewer_MouseWheel;
            }
            if (headerscrollviewer != null)
            {
                headerscrollviewer.PreviewMouseLeftButtonUp -= headerscrollviewer_PreviewMouseLeftButtonUp;
                headerscrollviewer.PreviewMouseWheel -= headerscrollviewer_MouseWheel;
            }
#endif
            if (scrollviewer != null) scrollviewer.Loaded -= scrollviewer_Loaded;
            if (previousNavigationTap != null)
            {
#if WINRT
                previousNavigationTap.Tapped -= PreviousNavigationTap_Tapped;
#elif WPF
                previousNavigationTap.PreviewMouseLeftButtonDown -= PreviousNavigationTap_MouseLeftButtonUp;
#else
                previousNavigationTap.MouseLeftButtonUp -= PreviousNavigationTap_MouseLeftButtonUp;
#endif
            }
            if (nextNavigationTap != null)
            {
#if WINRT
                nextNavigationTap.Tapped -= NextNavigationTap_Tapped;
#elif WPF
                nextNavigationTap.PreviewMouseLeftButtonUp -= NextNavigationTap_MouseLeftButtonUp;
#else
                nextNavigationTap.MouseLeftButtonUp -= NextNavigationTap_MouseLeftButtonUp;
#endif
            }
        }

        #endregion

        #region Events

        #region Scroll Viewer Loaded

        void scrollviewer_Loaded(object sender, RoutedEventArgs e)
        {
            SetupAppointments();
            if (IsHighLightWorkingHours)
            {
                SetWorkingHoursPosition();
            }
#if WPF
            var verticalScrollBar = scrollviewer.Template.FindName("PART_VerticalScrollBar", scrollviewer) as ScrollBar;
            if (verticalScrollBar != null)
            {
                verticalScrollBar.MouseDoubleClick += verticalScrollBar_MouseDoubleClick;
                verticalScrollBar.MouseLeave += verticalScrollBar_MouseLeave;
            }
            var horizontalScrollBar = scrollviewer.Template.FindName("PART_HorizontalScrollBar", scrollviewer) as ScrollBar;
            if (horizontalScrollBar != null)
            {
                horizontalScrollBar.MouseDoubleClick += horizontalScrollBar_MouseDoubleClick;
                horizontalScrollBar.MouseLeave += horizontalScrollBar_MouseLeave;
            }
#endif
        }

#if WPF
        void verticalScrollBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = false;
        }

        void verticalScrollBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = true;
        }

        void horizontalScrollBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = false;
        }

        void horizontalScrollBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (schedule != null)
                schedule.allowEditorsToOpen = true;
        }
#endif
        #endregion

        #region Appointment property Changed

        void app_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (daysAppointmentsLayout != null && (e.PropertyName == "AllDay"))
            {
                SetupAppointments();
            }
        }

        #endregion

        #region Scrollviewer's View Changed
#if !SILVERLIGHT
#if WINRT
        void scrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
#else
        void scrollviewer_MouseWheel(object sender, MouseWheelEventArgs e)
#endif
        {
#if WINRT
            if (schedule != null)
            {
                if (!schedule.isscrollmoveondragging)
                {
                    if (schedule.DragDropCanvas.Children.Count > 0 && !schedule.isScrollResizeEnabled)
                    {
                        schedule.Dayviewrb.Detach();
                        schedule.Monthviewrb.Detach();
                        schedule.Timelineviewrb.Detach();
                        var control = schedule.DragDropCanvas.Children[0] as Control;
                        if (control != null)
                        {
                            control.PointerPressed -= schedule.drag_app_PointerPressed;
                            control.Loaded -= schedule.drag_app_Loaded;
                        }
                        schedule.DragDropCanvas.Children.Clear();
                        schedule.ResetDragDropAppointmentOpacity();
                    }
                }
            }
#else
            schedule.editpopup.IsOpen = false;
            schedule.addnewpopup.IsOpen = false;
            schedule.contextmenupopup.IsOpen = false;
            schedule.AddnewContextmenuPopup.IsOpen = false;
            if (!schedule.isscrollmoveondragging)
            {
                if (schedule.DragDropCanvas.Children.Count > 0 && !schedule.isScrollResizeEnabled)
                {
                    schedule.Dayviewrb.Detach();
                    schedule.Monthviewrb.Detach();
                    schedule.Timelineviewrb.Detach();
                    var control = schedule.DragDropCanvas.Children[0] as Control;
                    if (control != null)
                    {
                        control.MouseLeftButtonDown -= schedule.drag_app_MouseLeftButtonDown;
                        control.Loaded -= schedule.drag_app_Loaded;
                    }
                    schedule.DragDropCanvas.Children.Clear();
                    schedule.ResetDragDropAppointmentOpacity();
                }
            }
#endif
            if (schedule != null)
            {
                schedule.editpopup.IsOpen = false;
                schedule.addnewpopup.IsOpen = false;
            }
#if SyncfusionFramework4_5_11 && WINRT
            timelinescrollviewer.ChangeView(null, scrollviewer.VerticalOffset, null);
            headerscrollviewer.ChangeView(scrollviewer.HorizontalOffset, null, null);
#else
            timelinescrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset);
            headerscrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset);
#endif
            TimeSpan tempstartspan = FindStartspan(scrollviewer.VerticalOffset);
            TimeSpan tempendspan = FindExactEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight);

            if (tempstartspan < startspan || tempendspan >= endspan)
            {
                startspan = tempstartspan;
                endspan = FindEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight);
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    List<ScheduleAppointment> tempcollection = VisibleAppointments.OrderBy(app => app.InternalStartTime).ToList();
                    ResourceType restype = schedule.ScheduleResourceType;
                    string typename = schedule.Resource;
                    while (restype != null)
                    {
                        var resourcenamecoll = restype.ResourceCollection.Select(resrc => resrc.ResourceName).ToList();
                        tempcollection = (from app in tempcollection where (app.ResourceCollection.FirstOrDefault(res => (res.TypeName == typename && resourcenamecoll.Contains(res.ResourceName))) != null) select app).ToList();
                        restype = restype.SubResourceType;
                        if (restype != null)
                            typename = restype.TypeName;
                    }


                    foreach (ScheduleAppointment app in (from app in tempcollection where ((!app.AllDay && ((app.InternalStartTime.TimeOfDay > startspan && app.InternalStartTime.TimeOfDay < endspan) || (app.InternalEndTime.TimeOfDay < endspan && app.InternalEndTime.TimeOfDay > startspan) || (app.InternalStartTime.TimeOfDay < startspan && app.InternalEndTime.TimeOfDay > endspan)))) select app).ToList())
                    {
                        if (daysAppointmentsLayout.Items != null && !daysAppointmentsLayout.Items.Contains(app))
                            daysAppointmentsLayout.Items.Add(app);
                    }
                }
                else
                {
                    foreach (ScheduleAppointment app in (from app in VisibleAppointments where ((!app.AllDay && ((app.InternalStartTime.TimeOfDay > startspan && app.InternalStartTime.TimeOfDay < endspan) || (app.InternalEndTime.TimeOfDay < endspan && app.InternalEndTime.TimeOfDay > startspan) || (app.InternalStartTime.TimeOfDay < startspan && app.InternalEndTime.TimeOfDay > endspan)))) select app).ToList())
                    {
                        if (daysAppointmentsLayout.Items != null && !daysAppointmentsLayout.Items.Contains(app))
                            daysAppointmentsLayout.Items.Add(app);
                    }
                }
            }
        }

#if WINRT
        void timelinescrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
#else
        void timelinescrollviewer_MouseWheel(object sender, MouseWheelEventArgs e)
#endif
        {
            scrollviewer.ScrollToVerticalOffset(timelinescrollviewer.VerticalOffset);
        }

#if WINRT
        void headerscrollviewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
#else
        void headerscrollviewer_MouseWheel(object sender, MouseWheelEventArgs e)
#endif
        {
#if SyncfusionFramework4_5_11 && WINRT
            scrollviewer.ChangeView(headerscrollviewer.HorizontalOffset, null, null);
#else
            scrollviewer.ScrollToHorizontalOffset(headerscrollviewer.HorizontalOffset);
#endif
        }

#endif

        #endregion

#if WINRT
        void TodayTimer_Tick(object sender, object e)
#else
        void TodayTimer_Tick(object sender, EventArgs e)
#endif
        {
            TodayTimer.Interval = new TimeSpan(0, 1, 0);
            var timelineitem = this.FindElementOfType<ScheduleHorizontalTimeSlotItemsControl>();
            if (timelineitem != null)
            {
                double hrHeight = timelineitem.ActualHeight / (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
                double min = (((Double)DateTime.Now.Minute) / 100) * ((Double)5 / 3);
                double posY = ((DateTime.Now.Hour - ScheduleTimeLineItemsControl.MinValue + min) / (IntervalHeight / hrHeight)) * IntervalHeight;
                if (Double.IsNaN(posY))
                {
                    posY = 0;
                }
                CurrentTimeIndicatorMargin = new Thickness(0, posY - (currentTimeIndicatorPresenter.ActualHeight / 2), 0, 0);
                currentTimeIndicatorPresenter.DataContext = DateTime.Now;
            }
        }

#if !WINRT
        void ScheduleDaysView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsTemplateApplied && scheduleDaysHeaderViewItemsControl != null && (scheduleDaysHeaderViewItemsControl.CurrentScheduleType == ScheduleType.Week ||scheduleDaysHeaderViewItemsControl.CurrentScheduleType == ScheduleType.WorkWeek))
            {
                scheduleDaysHeaderViewItemsControl.GenerateItems();
            }

            var sfSchedule = (sender as ScheduleDaysView).FindParentElementOfType<SfSchedule>();
            if (sfSchedule != null)
            {
                scheduleHorizontalTimeSlotItemsControl.UpdateTimeSlot();
                this.GenerateHeaderItems();
                sfSchedule.DragDropCanvas.Children.Clear();
                sfSchedule.editpopup.IsOpen = false;
                sfSchedule.addnewpopup.IsOpen = false;
                sfSchedule.contextmenupopup.IsOpen = false;
                sfSchedule.AddnewContextmenuPopup.IsOpen = false;
            }

        }

        void scheduleDaysHeaderViewItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var timelineitem = this.FindElementOfType<ScheduleVerticalTimeSlotItemsControl>();
            if (timelineitem != null)
            {
                timelineitem.Width = scheduleDaysHeaderViewItemsControl.ActualWidth;
            }
        }
#endif

        void ScheduleDaysView_Loaded(object sender, RoutedEventArgs e)
        {
            if (schedule != null && schedule.SelectedPoint != new Point() && schedule.currentAllDaySelectedItem == null)
                UpdateSelection(false);
            if (NonAccessibleBlocks != null && NonAccessibleBlocks.Count > 0)
                SetNonAccessibleBlocks();
            var currentTimeIndicatorWidthBinding = new Binding { Path = new PropertyPath("ActualWidth"), Source = timelinescrollviewer };
            SetBinding(CurrentTimeIndicatorWidthProperty, currentTimeIndicatorWidthBinding);
#if SILVERLIGHT
            timelinescrollviewer.GotFocus += scrollviewer_GotFocus;
            schedule = this.FindParentElementOfType<SfSchedule>();
            SetNavigationTapVisibility();
            if (scrollviewer != null)
            {
                var vertical = ((FrameworkElement)VisualTreeHelper.GetChild(scrollviewer, 0)).FindName("VerticalScrollBar") as ScrollBar;
                var horizontal = ((FrameworkElement)VisualTreeHelper.GetChild(scrollviewer, 0)).FindName("HorizontalScrollBar") as ScrollBar;
                if (vertical != null && horizontal != null)
                {
                    vertical.Scroll += vertical_Scroll;
                    horizontal.Scroll += vertical_Scroll;
                    vertical.ValueChanged += verticalbar_ValueChanged;
                    horizontal.ValueChanged += verticalbar_ValueChanged;
                }
            }
            if (timelinescrollviewer != null)
            {
                var timelinebar = timelinescrollviewer.FindElementOfType<ScrollBar>();
                if (timelinebar != null)
                    timelinebar.ValueChanged += timelinebar_ValueChanged;
            }
#endif
        }

#if SILVERLIGHT
        void vertical_Scroll(object sender, ScrollEventArgs e)
        {
            schedule.editpopup.IsOpen = false;
            schedule.addnewpopup.IsOpen = false;
            if ((sender as ScrollBar).Name != "HorizontalScrollBar")
            {
                timelinescrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset);
            }
            headerscrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset);
            TimeSpan tempstartspan = FindStartspan(scrollviewer.VerticalOffset);
            TimeSpan tempendspan = FindExactEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight);
            if (tempstartspan < startspan || tempendspan >= endspan)
            {
                startspan = tempstartspan;
                endspan = FindEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight);
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    List<ScheduleAppointment> tempcollection = VisibleAppointments.OrderBy(app => app.InternalStartTime).ToList();
                    ResourceType restype = schedule.ScheduleResourceType;
                    string typename = schedule.Resource;
                    while (restype != null)
                    {
                        var resourcenamecoll = restype.ResourceCollection.Select(resrc => resrc.ResourceName).ToList();
                        tempcollection = (from app in tempcollection where (app.ResourceCollection.FirstOrDefault(res => (res.TypeName == typename && resourcenamecoll.Contains(res.ResourceName))) != null) select app).ToList();
                        restype = restype.SubResourceType;
                    }
                    foreach (ScheduleAppointment app in (from app in tempcollection where ((!app.AllDay && ((app.InternalStartTime.TimeOfDay > startspan && app.InternalStartTime.TimeOfDay < endspan) || (app.InternalEndTime.TimeOfDay < endspan && app.InternalEndTime.TimeOfDay > startspan) || (app.InternalStartTime.TimeOfDay < startspan && app.InternalEndTime.TimeOfDay > endspan)))) select app).ToList())
                    {
                        if (daysAppointmentsLayout.Items != null && !daysAppointmentsLayout.Items.Contains(app))
                            daysAppointmentsLayout.Items.Add(app);
                    }
                }
                else
                {
                    foreach (ScheduleAppointment app in (from app in VisibleAppointments where ((!app.AllDay && ((app.InternalStartTime.TimeOfDay > startspan && app.InternalStartTime.TimeOfDay < endspan) || (app.InternalEndTime.TimeOfDay < endspan && app.InternalEndTime.TimeOfDay > startspan) || (app.InternalStartTime.TimeOfDay < startspan && app.InternalEndTime.TimeOfDay > endspan)))) select app).ToList())
                    {
                        if (daysAppointmentsLayout.Items != null && !daysAppointmentsLayout.Items.Contains(app))
                            daysAppointmentsLayout.Items.Add(app);
                    }
                }
            }

        }

        void timelinebar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            schedule.editpopup.IsOpen = false;
            schedule.addnewpopup.IsOpen = false;
            if ((sender as ScrollBar).Name != "HorizontalScrollBar")
            {
                scrollviewer.ScrollToVerticalOffset(timelinescrollviewer.VerticalOffset);
            }
        }

        void verticalbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timelinescrollviewer != null && (sender as ScrollBar).Name != "HorizontalScrollBar")
                timelinescrollviewer.ScrollToVerticalOffset(e.NewValue);
            if (!e.OldValue.Equals(e.NewValue))
            {
                var scrollBar = sender as ScrollBar;
                if (scrollBar != null && scrollBar.Orientation == Orientation.Vertical)
                {
                    timelinescrollviewer.ScrollToVerticalOffset(e.NewValue);
                    scrollviewer.ScrollToVerticalOffset(e.NewValue);
                }
                else
                {
                    headerscrollviewer.ScrollToHorizontalOffset(e.NewValue);
                }
                if (!schedule.isscrollmoveondragging)
                {
                    if (schedule.DragDropCanvas.Children.Count > 0 && !schedule.isScrollResizeEnabled)
                    {
                        schedule.Dayviewrb.Detach();
                        schedule.Monthviewrb.Detach();
                        schedule.Timelineviewrb.Detach();
                        var control = schedule.DragDropCanvas.Children[0] as Control;
                        if (control != null)
                        {
                            control.MouseLeftButtonDown -= schedule.drag_app_MouseLeftButtonDown;
                            control.Loaded -= schedule.drag_app_Loaded;
                        }
                        schedule.DragDropCanvas.Children.Clear();
                        schedule.ResetDragDropAppointmentOpacity();
                    }
                }

            }
        }
#endif

        #endregion

        #region Overrides

        #region OnApplyTemplate

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            nonworkingdaysLayout = GetTemplateChild("PART_NonWorkingDaysLayout") as ScheduleNonWorkingDayItemsControl;
            alldaysAppointmentsLayout = GetTemplateChild("PART_AllDaysAppointmentsLayout") as ScheduleAllDaysAppointmentItemsControl;
            daysAppointmentsLayout = GetTemplateChild("PART_DaysAppointmentsLayout") as ScheduleDaysAppointmentLayoutItemsControl;
            scheduleHorizontalTimeSlotItemsControl = GetTemplateChild("PART_HorizontalSlot") as ScheduleHorizontalTimeSlotItemsControl;
            scrollviewer = GetTemplateChild("Scrollviewer") as ScrollViewer;
            headerscrollviewer = GetTemplateChild("View") as ScrollViewer;
            timelinescrollviewer = GetTemplateChild("Scrollviewer1") as ScrollViewer;
            currentTimeIndicatorPresenter = GetTemplateChild("PART_DayCurrentTimeIndicator") as ContentPresenter;
#if !WINRT
            scheduleDaysHeaderViewItemsControl = GetTemplateChild("PART_ScheduleDaysHeaderViewItemsControl") as ScheduleDaysHeaderViewItemsControl;
            if (scheduleDaysHeaderViewItemsControl != null)
                scheduleDaysHeaderViewItemsControl.SizeChanged += scheduleDaysHeaderViewItemsControl_SizeChanged;
#endif
            resourcecontainer = GetTemplateChild("ResourceContainer") as ItemsControl;
            nextNavigationTap = GetTemplateChild("NextApp") as ContentPresenter;
            previousNavigationTap = GetTemplateChild("PrevApp") as ContentPresenter;
            schedule = this.FindParentElementOfType<SfSchedule>();
#if WINRT
            if (scrollviewer != null)
            {
                scrollviewer.ViewChanged += scrollviewer_ViewChanged;
                scrollviewer.Loaded += scrollviewer_Loaded;
            }
            if (timelinescrollviewer != null)
                timelinescrollviewer.ViewChanged += timelinescrollviewer_ViewChanged;
            if (headerscrollviewer != null)
                headerscrollviewer.ViewChanged += headerscrollviewer_ViewChanged;
#elif WPF
            if (scrollviewer != null)
            {
                scrollviewer.PreviewMouseWheel += scrollviewer_MouseWheel;
                scrollviewer.ScrollChanged += scrollviewer_ScrollChanged;
                scrollviewer.Loaded += scrollviewer_Loaded;
            }
            if (timelinescrollviewer != null)
            {
                timelinescrollviewer.PreviewMouseWheel += timelinescrollviewer_MouseWheel;
            }
            if (headerscrollviewer != null)
            {
                headerscrollviewer.PreviewMouseLeftButtonUp += headerscrollviewer_PreviewMouseLeftButtonUp;
                headerscrollviewer.PreviewMouseWheel += headerscrollviewer_MouseWheel;
            }
#elif SILVERLIGHT
            if (scrollviewer != null)
            {
                scrollviewer.GotFocus += scrollviewer_GotFocus;
                scrollviewer.Loaded += scrollviewer_Loaded;
            }
            SizeChanged += ScheduleDaysView_SizeChanged;
#endif
            IsTemplateApplied = true;

            if (previousNavigationTap != null)
            {
#if WINRT
                previousNavigationTap.Tapped += PreviousNavigationTap_Tapped;
#elif WPF
                previousNavigationTap.PreviewMouseLeftButtonDown += PreviousNavigationTap_MouseLeftButtonUp;
#else
                previousNavigationTap.MouseLeftButtonUp += PreviousNavigationTap_MouseLeftButtonUp;
#endif
            }
            if (nextNavigationTap != null)
            {
#if WINRT
                nextNavigationTap.Tapped += NextNavigationTap_Tapped;
#elif WPF
                nextNavigationTap.PreviewMouseLeftButtonUp += NextNavigationTap_MouseLeftButtonUp;
#else
                nextNavigationTap.MouseLeftButtonUp += NextNavigationTap_MouseLeftButtonUp;
#endif
            }
            SetNavigationTapVisibility();
            GenerateNonworkingdaysItems();

#if !WINRT
            appTextBox = GetTemplateChild("AppText") as TextBox;
            if (appTextBox != null)
                appTextBox.LostFocus += appTextBox_LostFocus;
#endif
        }

#if SILVERLIGHT
        void scrollviewer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(TextBox))
                schedule.Focus();
        }
#endif

#if !WINRT
        void appTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (AppTextVisibility == System.Windows.Visibility.Visible)
                schedule.CreateAppointmentOnSelection();
        }
#endif

#if WPF
        void headerscrollviewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            headerscrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset);
        }

        void scrollviewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            timelinescrollviewer.ScrollToVerticalOffset(scrollviewer.VerticalOffset);
            headerscrollviewer.ScrollToHorizontalOffset(scrollviewer.HorizontalOffset);
            TimeSpan tempstartspan = FindStartspan(scrollviewer.VerticalOffset);
            TimeSpan tempendspan = FindExactEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight);
            if (tempstartspan < startspan || tempendspan >= endspan)
            {
                startspan = tempstartspan;
                endspan = FindEndspan(scrollviewer.VerticalOffset + scrollviewer.ActualHeight);
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    var tempcollection = VisibleAppointments.OrderBy(app => app.InternalStartTime).ToList();
                    var restype = schedule.ScheduleResourceType;
                    string typename = schedule.Resource;
                    while (restype != null)
                    {
                        var resourcenamecoll = restype.ResourceCollection.Select(resrc => resrc.ResourceName).ToList();
                        tempcollection = (from app in tempcollection where (app.ResourceCollection.FirstOrDefault(res => (res.TypeName == typename && resourcenamecoll.Contains(res.ResourceName))) != null) select app).ToList();
                        restype = restype.SubResourceType;
                    }
                    foreach (ScheduleAppointment app in (from app in tempcollection where ((!app.AllDay && ((app.InternalStartTime.TimeOfDay > startspan && app.InternalStartTime.TimeOfDay < endspan) || (app.InternalEndTime.TimeOfDay < endspan && app.InternalEndTime.TimeOfDay > startspan) || (app.InternalStartTime.TimeOfDay < startspan && app.InternalEndTime.TimeOfDay > endspan)))) select app).ToList())
                    {
                        if (daysAppointmentsLayout.Items != null && !daysAppointmentsLayout.Items.Contains(app))
                            daysAppointmentsLayout.Items.Add(app);
                    }
                }
                else
                {
                    foreach (ScheduleAppointment app in (from app in VisibleAppointments where ((!app.AllDay && ((app.InternalStartTime.TimeOfDay > startspan && app.InternalStartTime.TimeOfDay < endspan) || (app.InternalEndTime.TimeOfDay < endspan && app.InternalEndTime.TimeOfDay > startspan) || (app.InternalStartTime.TimeOfDay < startspan && app.InternalEndTime.TimeOfDay > endspan)))) select app).ToList())
                    {
                        if (daysAppointmentsLayout.Items != null && !daysAppointmentsLayout.Items.Contains(app))
                            daysAppointmentsLayout.Items.Add(app);
                    }
                }
            }

        }

#endif

#if WINRT
        void PreviousNavigationTap_Tapped(object sender, TappedRoutedEventArgs e)
#else
        void PreviousNavigationTap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
#endif
        {
            if (schedule != null)
            {
                schedule.MoveToPreviousAppointment();
                previousNavigationTap.Opacity = 0;
                nextNavigationTap.Opacity = 0;
            }
            e.Handled = true;
        }

#if WINRT
        void NextNavigationTap_Tapped(object sender, TappedRoutedEventArgs e)
#else
        void NextNavigationTap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
#endif
        {
            if (schedule != null)
            {
                schedule.MoveToNextAppointment();
                previousNavigationTap.Opacity = 0;
                nextNavigationTap.Opacity = 0;
            }
            e.Handled = true;
        }

        #endregion

        #endregion
    }
}
