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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
#else
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections;
using System.Windows.Controls.Primitives;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a schedule's timeline view.
    /// </summary>
    public class ScheduleTimeLineView : Control, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleTimeLineView">ScheduleTimeLineView</see> class. 
        /// </summary>
        public ScheduleTimeLineView()
        {
            TodayTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            TodayTimer.Tick += TodayTimer_Tick;
            DefaultStyleKey = typeof(ScheduleTimeLineView);
            Loaded += ScheduleTimeLineView_Loaded;
        }

        #endregion

        #region Internal Fields

        internal ContentPresenter previousNavigationTap;
        internal ContentPresenter nextNavigationTap;
        internal ScheduleHorizontalTimeLineItemsControl scheduleHorizontalTimeLineItemsControl;
        internal ScrollViewer timelinescroll;
        internal DispatcherTimer TodayTimer;
        internal ScheduleTimelineTimeSlotItemsControl scheduleTimelineTimeSlotItemsControl;
        internal double dragDropCanvasWidth;
        internal double visibleCanvasWidth;
        internal bool dragResizeFlag;
#if !WINRT
        internal TextBox timelineAppTextBox;
#endif

        #endregion

        #region Private Fields

        private ScheduleNonWorkingDayItemsControl nonworkingdaysLayout;
        private double DayDiff, DatetimeDiff;
        private bool IsTemplateApplied;
        private ItemsControl Resourceheadercontainer;
        private SfSchedule schedule;
        private ContentPresenter currentTimeIndicatorPresenter;
        private ItemsControl AppointmentsLayoutItemsHost;
        private double hourHeight, time;
        private double previousXValue;
        #endregion

        #region Dependency Properties

        #region Public Properties

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
            DependencyProperty.Register("AppointmentTooltipVisibility", typeof(Visibility), typeof(ScheduleTimeLineView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region AppointmentToolTipTemplate
        /// <summary>
        /// Gets or sets the template for customizing appointment's tooltip.
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
            DependencyProperty.Register("AppointmentToolTipTemplate", typeof(ControlTemplate), typeof(ScheduleTimeLineView), new PropertyMetadata(null));
        #endregion
#endif

        #region AppointmentTemplate
        /// <summary>
        /// Gets or sets the template for customizing appointment in timeline view.
        /// </summary>
        public DataTemplate AppointmentTemplate
        {
            get { return (DataTemplate)GetValue(AppointmentTemplateProperty); }
            set { SetValue(AppointmentTemplateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for AppointmentTemplate.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty AppointmentTemplateProperty =
            DependencyProperty.Register("AppointmentTemplate", typeof(DataTemplate), typeof(ScheduleTimeLineView), new PropertyMetadata(null));
        #endregion

        #region VisibleAppointments
        /// <summary>
        /// Gets or sets the appointments that are visible in timeline view.
        /// </summary>
        public ScheduleAppointmentCollection VisibleAppointments
        {
            get { return (ScheduleAppointmentCollection)GetValue(VisibleAppointmentsProperty); }
            set { SetValue(VisibleAppointmentsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for VisibleAppointments.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty VisibleAppointmentsProperty =
            DependencyProperty.Register("VisibleAppointments", typeof(ScheduleAppointmentCollection), typeof(ScheduleTimeLineView), new PropertyMetadata(null, VisibleAppointmentsChanged));

        private static void VisibleAppointmentsChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var stv = dpo as ScheduleTimeLineView;
            if (stv != null && stv.IsTemplateApplied)
            {
#if WINRT
                if (stv.schedule != null && stv.CurrentScheduleType == ScheduleType.TimeLine && stv.schedule.VisibleDateChanged == false)
#endif
                    stv.SetupAppointments();
            }
        }
        #endregion

        #region SelectedDates
        /// <summary>
        /// Gets or sets the selected dates in timeline view.
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
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleTimeLineView), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var stv = dpo as ScheduleTimeLineView;
            if (stv != null && (args.NewValue != null && args.NewValue != args.OldValue && stv.CurrentScheduleType == ScheduleType.TimeLine))
            {
                if (stv.schedule != null && stv.IsTemplateApplied)
                {

                    stv.SetupAppointments();
                }
            }
        }
        #endregion

        #region ShowAppointmentNavigationButtons
        /// <summary>
        /// Gets or sets a value indicating whether the appointment navigation buttons should be shown in timeline view.
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
            DependencyProperty.Register("ShowAppointmentNavigationButtons", typeof(bool), typeof(ScheduleTimeLineView), new PropertyMetadata(false));
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
            DependencyProperty.Register("PreviousNavigationButtonTemplate", typeof(DataTemplate), typeof(ScheduleTimeLineView), new PropertyMetadata(null));
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

        // Using a DependencyProperty as the backing store for NextNavigationButtonTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextNavigationButtonTemplateProperty =
            DependencyProperty.Register("NextNavigationButtonTemplate", typeof(DataTemplate), typeof(ScheduleTimeLineView), new PropertyMetadata(null));
        #endregion

        #region NonAccessibleBlocks
        internal NonAccessibleBlockCollection NonAccessibleBlocks
        {
            get { return (NonAccessibleBlockCollection)GetValue(NonAccessibleBlocksProperty); }
            set { SetValue(NonAccessibleBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonAccessibleBlocks.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonAccessibleBlocksProperty =
            DependencyProperty.Register("NonAccessibleBlocks", typeof(NonAccessibleBlockCollection), typeof(ScheduleTimeLineView), new PropertyMetadata(null, OnNonAccessibleBlocksChanged));
        #endregion

        #region NonAccessibleBlockCollection
        internal NonAccessibleBlockCollection NonAccessibleBlockCollection
        {
            get { return (NonAccessibleBlockCollection)GetValue(NonAccessibleBlockCollectionProperty); }
            set { SetValue(NonAccessibleBlockCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonAccessibleBlockCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonAccessibleBlockCollectionProperty =
            DependencyProperty.Register("NonAccessibleBlockCollection", typeof(NonAccessibleBlockCollection), typeof(ScheduleTimeLineView), new PropertyMetadata(null));

        private static void OnNonAccessibleBlocksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineView)
            {
                var timelineView = d as ScheduleTimeLineView;
                if (timelineView.IsTemplateApplied)
                    timelineView.SetNonAccessibleBlocks();
            }
        }
        #endregion

        #endregion

        #region Read-only Properties

        #region MinorTickVisibility
        /// <summary>
        /// Gets the visibility of minor ticks that represent minute in time slot.
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
            DependencyProperty.Register("MinorTickVisibility", typeof(Visibility), typeof(ScheduleTimeLineView), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region MajorTickVisibility
        /// <summary>
        /// Gets the visibility of major ticks that represent hour in time slot.
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
            DependencyProperty.Register("MajorTickVisibility", typeof(Visibility), typeof(ScheduleTimeLineView), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region IntervalHeight
        /// <summary>
        /// Gets the height of the interval in timeline view.
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
            DependencyProperty.Register("IntervalHeight", typeof(double), typeof(ScheduleTimeLineView), new PropertyMetadata(ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineView)
            {
                var scheduleTimeLineView = d as ScheduleTimeLineView;
                if (scheduleTimeLineView.NonAccessibleBlocks != null)
                    scheduleTimeLineView.SetNonAccessibleBlocks();
            }
        }
        #endregion

        #region TimeMode
        /// <summary>
        /// Gets the time mode of the timeline view.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeModes"></seealso>
        public TimeModes TimeMode
        {
            get { return (TimeModes)GetValue(TimeModeProperty); }
            internal set { SetValue(TimeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeModeProperty =
            DependencyProperty.Register("TimeMode", typeof(TimeModes), typeof(ScheduleTimeLineView), new PropertyMetadata(TimeModes.TwelveHours));
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets the time interval of timeline view.
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
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleTimeLineView), new PropertyMetadata(TimeInterval.ThirtyMin, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineView)
            {
                var scheduleTimeLineView = d as ScheduleTimeLineView;
                if (scheduleTimeLineView.NonAccessibleBlocks != null)
                    scheduleTimeLineView.SetNonAccessibleBlocks();
            }
        }

        #endregion

        #region CurrentScheduleType
        /// <summary>
        /// Gets the schedule type of current view.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleType"></seealso>
        public ScheduleType CurrentScheduleType
        {
            get { return (ScheduleType)GetValue(CurrentScheduleTypeProperty); }
            internal set { SetValue(CurrentScheduleTypeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CurrentScheduleType.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CurrentScheduleTypeProperty =
            DependencyProperty.Register("CurrentScheduleType", typeof(ScheduleType), typeof(ScheduleTimeLineView), new PropertyMetadata(ScheduleType.Day));
        #endregion

        #endregion

        #region Internal Properties
#if !WINRT
        #region TimelineAppTextVisibility



        public Visibility TimelineAppTextVisibility
        {
            get { return (Visibility)GetValue(TimelineAppTextVisibilityProperty); }
            set { SetValue(TimelineAppTextVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimelineAppTextVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimelineAppTextVisibilityProperty =
            DependencyProperty.Register("TimelineAppTextVisibility", typeof(Visibility), typeof(ScheduleTimeLineView), new PropertyMetadata(Visibility.Collapsed));


        #endregion
#endif
        #region CurrentDateBackground
        internal Brush CurrentDateBackground
        {
            get { return (Brush)GetValue(CurrentDateBackgroundProperty); }
            set { SetValue(CurrentDateBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentBackground.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentDateBackgroundProperty =
            DependencyProperty.Register("CurrentDateBackground", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickStroke
        internal Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickStroke
        internal Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickLabelStroke
        internal Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickLabelStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickLabelStroke
        internal Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickLabelStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region NonWorkingDateCollection
        internal ObservableCollection<DayOfWeek> NonWorkingDateCollection
        {
            get { return (ObservableCollection<DayOfWeek>)GetValue(NonWorkingDateCollectionProperty); }
            set { SetValue(NonWorkingDateCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonWorkingDateCollection.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonWorkingDateCollectionProperty =
            DependencyProperty.Register("NonWorkingDateCollection", typeof(ObservableCollection<DayOfWeek>), typeof(ScheduleTimeLineView), new PropertyMetadata(null, OnNonWorkingDaysCollectionChanged));

        private static void OnNonWorkingDaysCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheduletimelineview = d as ScheduleTimeLineView;
            if (scheduletimelineview != null && scheduletimelineview.IsTemplateApplied)
                scheduletimelineview.GenerateNonworkingdaysItems();
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
            DependencyProperty.Register("RectXPosition", typeof(double), typeof(ScheduleTimeLineView), new PropertyMetadata(0d));
        #endregion

        #region RectYPosition
        internal double RectYPosition
        {
            get { return (double)GetValue(RectYPositionProperty); }
            set { SetValue(RectYPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectXPosition.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectYPositionProperty =
           DependencyProperty.Register("RectYPosition", typeof(double), typeof(ScheduleTimeLineView), new PropertyMetadata(0d));
        #endregion

        #region RectHeight
        internal double RectHeight
        {
            get { return (double)GetValue(RectHeightProperty); }
            set { SetValue(RectHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectHeightProperty =
            DependencyProperty.Register("RectHeight", typeof(double), typeof(ScheduleTimeLineView), new PropertyMetadata(0d));
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
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#else
        internal static readonly DependencyProperty AppointmentSelectionBrushProperty =
            DependencyProperty.Register("AppointmentSelectionBrush", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Black)));
#endif
        #endregion

        #region RectVisibility
        internal Visibility RectVisibility
        {
            get { return (Visibility)GetValue(RectVisibilityProperty); }
            set { SetValue(RectVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectVisibilityProperty =
            DependencyProperty.Register("RectVisibility", typeof(Visibility), typeof(ScheduleTimeLineView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region RectWidth
        internal double RectWidth
        {
            get { return (double)GetValue(RectWidthProperty); }
            set { SetValue(RectWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RectHeight.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty RectWidthProperty =
            DependencyProperty.Register("RectWidth", typeof(double), typeof(ScheduleTimeLineView), new PropertyMetadata(0d));
        #endregion

        #region IsTimeIntervalChanged
        internal bool IsTimeIntervalChanged
        {
            get { return (bool)GetValue(IsTimeIntervalChangedProperty); }
            set { SetValue(IsTimeIntervalChangedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsTimeIntervalChanged.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsTimeIntervalChangedProperty =
            DependencyProperty.Register("IsTimeIntervalChanged", typeof(bool), typeof(ScheduleTimeLineView), new PropertyMetadata(false, OnIsTimeIntervalChanged));

        private static void OnIsTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeline = d as ScheduleTimeLineView;
            if (timeline != null && timeline.IsTemplateApplied)
                timeline.GenerateNonworkingdaysItems();
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
            DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region NonWorkingHourBrush
        internal Brush NonWorkingHourBrush
        {
            get { return (Brush)GetValue(NonWorkingHourBrushProperty); }
            set { SetValue(NonWorkingHourBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NonWorkingHourBrush.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty NonWorkingHourBrushProperty =
            DependencyProperty.Register("NonWorkingHourBrush", typeof(Brush), typeof(ScheduleTimeLineView), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region IsHighLightWorkingHours
        internal bool IsHighLightWorkingHours
        {
            get { return (bool)GetValue(IsHighLightWorkingHoursProperty); }
            set { SetValue(IsHighLightWorkingHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsHightLightWorkingHours.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty IsHighLightWorkingHoursProperty =
            DependencyProperty.Register("IsHighLightWorkingHours", typeof(bool), typeof(ScheduleTimeLineView), new PropertyMetadata(false, OnIsHighLightWorkingHoursChanged));

        private static void OnIsHighLightWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeline = d as ScheduleTimeLineView;
            if (timeline != null && timeline.IsTemplateApplied)
                timeline.GenerateNonworkingdaysItems();
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
            DependencyProperty.Register("ScheduleResourceType", typeof(ResourceType), typeof(ScheduleTimeLineView), new PropertyMetadata(null, OnTypeChanged));

        private static void OnTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tlv = d as ScheduleTimeLineView;
            if (tlv != null && tlv.IsTemplateApplied)
            {
                tlv.GenerateHeaderChildItems();
                tlv.SetupAppointments();

            }
        }
        #endregion

        #region MajorTickStrokeDashArray
        internal DoubleCollection MajorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MajorTickStrokeDashArrayProperty); }
            set { SetValue(MajorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleTimeLineView), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region MinorTickStrokeDashArray
        internal DoubleCollection MinorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MinorTickStrokeDashArrayProperty); }
            set { SetValue(MinorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleTimeLineView), new PropertyMetadata(new DoubleCollection()));
        #endregion

        #region CurrentTimeIndicatorTemplate
        internal DataTemplate CurrentTimeIndicatorTemplate
        {
            get { return (DataTemplate)GetValue(CurrentTimeIndicatorTemplateProperty); }
            set { SetValue(CurrentTimeIndicatorTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DragDropEndTime.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorTemplateProperty =
              DependencyProperty.Register("CurrentTimeIndicatorTemplate", typeof(DataTemplate), typeof(ScheduleTimeLineView), new PropertyMetadata(null));
        #endregion

        #region CurrentTimeIndicatorMargin
        internal Thickness CurrentTimeIndicatorMargin
        {
            get { return (Thickness)GetValue(CurrentTimeIndicatorMarginProperty); }
            set { SetValue(CurrentTimeIndicatorMarginProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeIndicatorMargin.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorMarginProperty =
              DependencyProperty.Register("CurrentTimeIndicatorMargin", typeof(Thickness), typeof(ScheduleTimeLineView), new PropertyMetadata(null));
        #endregion

        #region CurrentTimeIndicatorVisibility
        internal Visibility CurrentTimeIndicatorVisibility
        {
            get { return (Visibility)GetValue(CurrentTimeIndicatorVisibilityProperty); }
            set { SetValue(CurrentTimeIndicatorVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeIndicatorVisibility.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorVisibilityProperty =
              DependencyProperty.Register("CurrentTimeIndicatorVisibility", typeof(Visibility), typeof(ScheduleTimeLineView), new PropertyMetadata(Visibility.Collapsed));
        #endregion

        #region CurrentTimeIndicatorWidth
        internal double CurrentTimeIndicatorWidth
        {
            get { return (double)GetValue(CurrentTimeIndicatorWidthProperty); }
            set { SetValue(CurrentTimeIndicatorWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentTimeIndicatorWidth.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty CurrentTimeIndicatorWidthProperty =
              DependencyProperty.Register("CurrentTimeIndicatorWidth", typeof(double), typeof(ScheduleTimeLineView), new PropertyMetadata(50d));
        #endregion

        #region ShowNonWorkingHours
        internal bool ShowNonWorkingHours
        {
            get { return (bool)GetValue(ShowNonWorkingHoursProperty); }
            set { SetValue(ShowNonWorkingHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNonWorkingHours.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ShowNonWorkingHoursProperty =
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(ScheduleTimeLineView), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineView)
            {
                var scheduleTimeLineView = (d as ScheduleTimeLineView);
                scheduleTimeLineView.GenerateNonworkingdaysItems();
                if (scheduleTimeLineView.CurrentTimeIndicatorVisibility == Visibility.Visible)
                    scheduleTimeLineView.TodayTimer.Interval = new TimeSpan(0, 0, 1);
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
            DependencyProperty.Register("WorkStartHour", typeof(int), typeof(ScheduleTimeLineView), new PropertyMetadata(9, OnWorkingHoursChanged));
        #endregion

        #region WorkEndHour
        internal int WorkEndHour
        {
            get { return (int)GetValue(WorkEndHourProperty); }
            set { SetValue(WorkEndHourProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WorkEndHour.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty WorkEndHourProperty =
            DependencyProperty.Register("WorkEndHour", typeof(int), typeof(ScheduleTimeLineView), new PropertyMetadata(18, OnWorkingHoursChanged));

        private static void OnWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineView)
            {
                var scheduleTimeLineView = d as ScheduleTimeLineView;
                if(scheduleTimeLineView.schedule != null)
                    scheduleTimeLineView.schedule.ResetDragDropAppointmentOpacity();
                scheduleTimeLineView.SetupAppointments();                
                if (scheduleTimeLineView.CurrentTimeIndicatorVisibility == Visibility.Visible)
                    scheduleTimeLineView.TodayTimer.Interval = new TimeSpan(0, 0, 1);
            }
        }
        #endregion

        #endregion

        #endregion

        #region Methods

        #region Appointments population


#if WINRT
        internal async void SetupAppointments()
#else
        internal void SetupAppointments()
#endif
        {
            if (VisibleAppointments == null)
            {
                return;
            }
            var sfSchedule = this.FindParentElementOfType<SfSchedule>();
            if (sfSchedule != null && sfSchedule.ScheduleResourceType != null && sfSchedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                ClearAppointments();
#if WINRT
                if (sfSchedule.VisibleDateChanged)
                    await Task.Delay(800);
#endif
            }

            var widthBinding = new Binding { Path = new PropertyPath("Width"), Source = AppointmentsLayoutItemsHost };

            var selectionbrushBinding = new Binding { Path = new PropertyPath("AppointmentSelectionBrush"), Source = sfSchedule };
#if !WINRT
            var appointmentTooltipVisibilityBinding = new Binding { Path = new PropertyPath("AppointmentTooltipVisibility"), Source = sfSchedule };
            var appointmentToolTipTemplateBinding = new Binding { Path = new PropertyPath("AppointmentToolTipTemplate"), Source = sfSchedule };
#endif
            var appointmentTemplateBinding = new Binding { Path = new PropertyPath("AppointmentTemplate"), Source = sfSchedule };
            if (sfSchedule != null && sfSchedule.ScheduleResourceType != null && sfSchedule.ScheduleResourceType.ResourceCollection.Count > 0)
            {
                foreach (Resource t in sfSchedule.ScheduleResourceType.ResourceCollection)
                {
                    string resourcename = t.ResourceName;
                    var contenthost = new ScheduleHorizontalAppointmentLayoutItemsControl { ResourceName = resourcename };
                    List<ScheduleAppointment> resourceappcoll = (from app in VisibleAppointments where (app.ResourceCollection.FirstOrDefault(res => res.TypeName == sfSchedule.Resource) != null) select app).ToList();
                    foreach (var app in (from app in resourceappcoll let firstOrDefault = app.ResourceCollection.FirstOrDefault(res => res.TypeName == sfSchedule.Resource) where firstOrDefault != null && (firstOrDefault.ResourceName == resourcename) select app).ToList())
                    {
                        if (contenthost.Items != null && (app != null && !contenthost.Items.Contains(app) && !(app.AllDay)))
                        {
                            contenthost.Items.Add(app);
                        }
                    }
                    contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentTemplateProperty, appointmentTemplateBinding);
                    contenthost.SetBinding(WidthProperty, widthBinding);
                    contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentSelectionBrushProperty, selectionbrushBinding);
#if !WINRT
                    contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
                    contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
#endif
                    if (AppointmentsLayoutItemsHost.Items != null)
                        AppointmentsLayoutItemsHost.Items.Add(contenthost);
                }
            }
            else if (sfSchedule != null)
            {
                ClearAppointments();
                var contenthost = new ScheduleHorizontalAppointmentLayoutItemsControl();
                foreach (var app in VisibleAppointments)
                {
                    if (app != null && !(app.AllDay))
                    {
                        if (contenthost.Items != null)
                            contenthost.Items.Add(app);
                    }
                }
                contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentTemplateProperty, appointmentTemplateBinding);
                contenthost.SetBinding(WidthProperty, widthBinding);
                contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentSelectionBrushProperty, selectionbrushBinding);
#if !WINRT
                contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentToolTipTemplateProperty, appointmentToolTipTemplateBinding);
                contenthost.SetBinding(ScheduleHorizontalAppointmentLayoutItemsControl.AppointmentTooltipVisibilityProperty, appointmentTooltipVisibilityBinding);
#endif
                if (AppointmentsLayoutItemsHost.Items != null)
                    AppointmentsLayoutItemsHost.Items.Add(contenthost);
            }

#if !WINRT
            GenerateNonworkingdaysItems();
#endif
#if WINRT
            if (sfSchedule != null && sfSchedule.CurrentTimeIndicatorVisibility == Visibility.Visible)
#else
            if (sfSchedule != null && sfSchedule.CurrentTimeIndicatorVisibility == Visibility.Visible)
#endif
            {
                if (SelectedDates.Contains(DateTime.Now.Date))
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
            //UpdateLayout();
        }

        private void ClearAppointments()
        {
            if (AppointmentsLayoutItemsHost.Items != null)
            {
                foreach (ItemsControl itmcnrt in AppointmentsLayoutItemsHost.Items)
                {
                    if (itmcnrt.Items != null)
                        itmcnrt.Items.Clear();
                }
                AppointmentsLayoutItemsHost.Items.Clear();
            }
        }

        #endregion

        #region GenerateNonWorkingdaysItems

        internal void GenerateNonworkingdaysItems()
        {
            if (nonworkingdaysLayout != null && IsHighLightWorkingHours && ShowNonWorkingHours)
            {
                if (nonworkingdaysLayout.Items != null)
                    nonworkingdaysLayout.Items.Clear();
                int k = 0;
                var selecteddates = new ObservableCollection<DateTime>(SelectedDates.OrderBy(s => s));
                double dayscount = selecteddates.Count;
                var backgroundBinding = new Binding { Source = this, Path = new PropertyPath("NonWorkingHourBrush") };

                int resourcecount = 1;
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0 && schedule.DayHeaderOrder == DayHeaderOrder.OrderByResource)
                {
                    resourcecount = schedule.ScheduleResourceType.ResourceCollection.Count;
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
                if (k == 0)
                {
                    if (nonworkingdaysLayout.Items != null)
                        nonworkingdaysLayout.Items.Clear();
                }
                for (int i = 0; i < selecteddates.Count; i++)
                {
                    var grid1 = new Grid { DataContext = null };
                    grid1.SetBinding(Panel.BackgroundProperty, backgroundBinding);
                    var grid2 = new Grid { DataContext = null };
                    grid2.SetBinding(Panel.BackgroundProperty, backgroundBinding);
                    if (nonworkingdaysLayout.Items != null)
                    {
                        nonworkingdaysLayout.Items.Add(grid1);
                        nonworkingdaysLayout.Items.Add(grid2);
                    }
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
                NonAccessibleBlockCollection = new NonAccessibleBlockCollection();
                var nonAccessibleBlockTemplateBinding = new Binding { Source = schedule, Path = new PropertyPath("NonAccessibleBlockTemplate") };
                foreach (NonAccessibleBlock nonAccessibleBlock in NonAccessibleBlocks)
                {
                    BindingOperations.SetBinding(nonAccessibleBlock, NonAccessibleBlock.CustomTemplateProperty, nonAccessibleBlockTemplateBinding);
                    int index = 0;
                    double startHour = nonAccessibleBlock.StartHour > ScheduleTimeLineItemsControl.MinValue ? nonAccessibleBlock.StartHour : ScheduleTimeLineItemsControl.MinValue;
                    double tempLeft = GetTimePosition(startHour, index);

                    double left = tempLeft >= 0 ? tempLeft : 0;

                    nonAccessibleBlock.Margin = new Thickness(left, 0, 0, 0);
                    double endHour = nonAccessibleBlock.EndHour < ScheduleTimeLineItemsControl.MaxValue ? nonAccessibleBlock.EndHour : ScheduleTimeLineItemsControl.MaxValue;
                    endHour = endHour > startHour ? endHour : startHour;
                    double width = GetTimePosition(endHour, index) - left;
                    if (width < 0)
                        width = scheduleTimelineTimeSlotItemsControl != null ? scheduleTimelineTimeSlotItemsControl.Width - left : 0;
                    if (width > 0)
                    {
                        nonAccessibleBlock.Size = width;
                        NonAccessibleBlockCollection.Add(nonAccessibleBlock);
                        while (index < SelectedDates.Count - 1)
                        {
                            index++;
                            var block = new NonAccessibleBlock
                            {
                                Background = nonAccessibleBlock.Background,
                                StartHour = nonAccessibleBlock.StartHour,
                                EndHour = nonAccessibleBlock.EndHour,
                                Label = nonAccessibleBlock.Label,
                                Size = nonAccessibleBlock.Size
                            };
                            BindingOperations.SetBinding(block, NonAccessibleBlock.CustomTemplateProperty, nonAccessibleBlockTemplateBinding);
                            double leftPosition = GetTimePosition(startHour, index);
                            block.Margin = new Thickness(leftPosition, 0, 0, 0);
                            NonAccessibleBlockCollection.Add(block);
                        }
                    }
                }
            }
            else if (NonAccessibleBlockCollection != null)
            {
                NonAccessibleBlockCollection.Clear();
            }
        }

        private double GetTimePosition(double hour, int index)
        {
            double intervalWidth = IntervalHeight;
            double timeSlotWidth = GetTimeSlotWidth(SelectedDates.Count);
            if (schedule != null && !schedule.isIntervalHeightset && scheduleTimelineTimeSlotItemsControl != null)
            {
                timeSlotWidth = scheduleTimelineTimeSlotItemsControl.Width / SelectedDates.Count;
                intervalWidth = timeSlotWidth / ((ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval]);
            }
            double position = (index * timeSlotWidth) + (hour - ScheduleTimeLineItemsControl.MinValue) * intervalWidth * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            return position;
        }

        #endregion

        #region GenerateHeaderChildItems
        void GenerateHeaderChildItems()
        {
            if (Resourceheadercontainer.Items != null)
            {
                Resourceheadercontainer.Items.Clear();
                if (schedule != null && schedule.ScheduleResourceType != null && schedule.ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    int resourcecount = schedule.ScheduleResourceType.ResourceCollection.Count;

                    for (int i = 0; i < resourcecount; i++)
                    {
                        var headeritem = new TimeLineViewItemHeader { DataContext = schedule.ScheduleResourceType.ResourceCollection[i] };

                        Resourceheadercontainer.Items.Add(headeritem);
                    }

                }
            }
        }
        #endregion

        #region Finding Width

        internal double GetTimeSlotWidth(int selecteddatecount)
        {
            double width;
            if (schedule.ScheduleType == ScheduleType.TimeLine && !schedule.isIntervalHeightset)
            {
                // setting interval value as 80 since in timeline view only when it is set to 80, it occupies full screen
                width = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth * ScheduleTimeLineItemsControl.IntervalCount[(int)schedule.TimeInterval];
            }
            else
            {
                width = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * schedule.IntervalHeight * ScheduleTimeLineItemsControl.IntervalCount[(int)schedule.TimeInterval];
            }
            width = (width) * selecteddatecount;
            return width;
        }

        #endregion

        #region Updating Selection Region

        internal void UpdateSelection(bool isPointerTapped)
        {
            if (schedule == null)
                schedule = this.FindParentElementOfType<SfSchedule>();
            var timeSlotItemsControl = this.FindElementOfType<ScheduleTimelineTimeSlotItemsControl>();
            int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            if (schedule != null && timeSlotItemsControl != null)
            {
                int timeDiff = ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue;
                double heightofttimeslot = timeSlotItemsControl.ActualHeight;
                double rowindex;
                if (isPointerTapped && schedule.SelectedPoint.X <= timeSlotItemsControl.Width && time >= 0 && schedule.SelectedPoint.Y >= 0)
                {
                    hourHeight = timeSlotItemsControl.Width / SelectedDates.Count;
                    double total = ((schedule.SelectedPoint.X / hourHeight) * timeDiff) % timeDiff;
                    time = Math.Floor(total);
                    var timespan = (int)schedule.GetTimeInterval().TotalMinutes;
                    double decimalvalue = Math.Round(total - (int)total, 2) * 60;
                    int min = 0;
                    if (timespan != 0)
                    {
                        min = ((int)decimalvalue / timespan) * timespan;
                    }
                    var dayIndex = (int)Math.Floor(schedule.SelectedPoint.X / (hourHeight));
                    if (dayIndex >= 0 && dayIndex < SelectedDates.Count)
                    {
                        var selDate = SelectedDates[dayIndex];
                        schedule.SelectedDate = selDate.AddHours(time + ScheduleTimeLineItemsControl.MinValue);
                    }
                    RectXPosition = ((time + (dayIndex * timeDiff)) + ((double)min / 60)) * (hourHeight / timeDiff);
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        rowindex = Math.Floor(schedule.SelectedPoint.Y / (heightofttimeslot / ScheduleResourceType.ResourceCollection.Count));
                        RectYPosition = rowindex * (heightofttimeslot / ScheduleResourceType.ResourceCollection.Count);
                        RectHeight = heightofttimeslot / ScheduleResourceType.ResourceCollection.Count;
                    }
                    else
                    {
                        rowindex = Math.Floor(schedule.SelectedPoint.Y / heightofttimeslot);
                        RectYPosition = rowindex * heightofttimeslot;
                        RectHeight = heightofttimeslot;
                    }
                    RectWidth = (hourHeight / timeDiff) / interval;
                }
                // For selecting date in time slot items control 
                else if (schedule.CurrentSelectedDates.Contains(schedule.SelectedDate.Date) && schedule.SelectedPoint.Y >= 0)
                {
                    var height = timeSlotItemsControl.Width / schedule.SelectedDates.Count;
                    var hr = schedule.Currentselecteddate.Hour - ScheduleTimeLineItemsControl.MinValue;
                    var min = schedule.Currentselecteddate.Minute;
                    var dayIndex = schedule.SelectedDates.IndexOf(schedule.Currentselecteddate.Date);
                    if (dayIndex >= 0)
                        RectXPosition = ((hr + (dayIndex * timeDiff)) + ((double)min / 60)) * (height / timeDiff);
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0 && schedule.selectedResourcename.Count > 0)
                    {
                        var resource = ScheduleResourceType.ResourceCollection.FirstOrDefault(x => x.ResourceName.Equals(schedule.selectedResourcename[0].ResourceName));
                        rowindex = ScheduleResourceType.ResourceCollection.IndexOf(resource);
                        RectYPosition = rowindex * (heightofttimeslot / ScheduleResourceType.ResourceCollection.Count);
                        RectHeight = heightofttimeslot / ScheduleResourceType.ResourceCollection.Count;
                    }
                    else
                    {
                        RectYPosition = 0;
                        RectHeight = heightofttimeslot;
                    }
                    if (schedule.SelectedPoint != new Point())
                        RectWidth = (height / timeDiff) / interval;
                }
                // For selecting date in time line items control 
                else if (schedule.SelectedPoint.Y < 0)
                {
                    Point position = schedule.SelectedPoint;
                    double timelineWidth = timeSlotItemsControl.Width;
                    double hourWidth = timeSlotItemsControl.Width / (SelectedDates.Count * interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue));
                    int columnIndex = (int)Math.Floor(position.X / hourWidth);
                    double xposition = columnIndex * hourWidth;
                    double yposition = 0;
                    RectXPosition = double.IsNaN(xposition) ? 0 : xposition;
                    RectYPosition = double.IsNaN(yposition) ? 0 : yposition;
                    RectHeight = heightofttimeslot / (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 1 ? ScheduleResourceType.ResourceCollection.Count : 1);
                    RectWidth = hourWidth;
                } 
                RectVisibility = Visibility.Visible;
            }
        }

        #endregion
#if !WINRT

        #region Move Selection to Particular DateTime

        internal void MoveSelectionRectToDateTime(DateTime selectedDateTime)
        {
            //finding row (required only for schedule with reosurce)
            double selectedTime = selectedDateTime.TimeOfDay.TotalMinutes / 60;
            if (!ShowNonWorkingHours)
            {
                if (selectedTime < ScheduleTimeLineItemsControl.MinValue)
                {
                    DateTime prevDate = new DateTime();
                    prevDate = selectedDateTime.AddDays(-1).Date;
                    prevDate = prevDate.AddHours(ScheduleTimeLineItemsControl.MaxValue);
                    selectedDateTime = prevDate;
                }
                else if (selectedTime >= ScheduleTimeLineItemsControl.MaxValue)
                {
                    DateTime prevDate = new DateTime();
                    prevDate = selectedDateTime.AddDays(1).Date;
                    prevDate = prevDate.AddHours(ScheduleTimeLineItemsControl.MinValue);
                    selectedDateTime = prevDate;
                }
                selectedTime = selectedDateTime.TimeOfDay.TotalMinutes / 60;
            }
            //finding column using selectedDatetime
            if (ShowNonWorkingHours || (!ShowNonWorkingHours && selectedTime >= ScheduleTimeLineItemsControl.MinValue && selectedTime < ScheduleTimeLineItemsControl.MaxValue))
            {
                hourHeight = GetTimelineHourHeight();
                double selectedColumn = 0;
                int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                double time = selectedDateTime.Hour - ScheduleTimeLineItemsControl.MinValue + (double)(selectedDateTime.Minute) / 60;
                double columnPerDay = interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
                if (!(SelectedDates.Contains(selectedDateTime.Date)))
                {
                    schedule.MoveToDate(selectedDateTime.Date);
                    RectXPosition = 0;
                }
                double dayindex = SelectedDates.IndexOf(selectedDateTime.Date);
                selectedColumn = dayindex * columnPerDay + time * interval;
                double xposition = selectedColumn * hourHeight;
                RectXPosition = double.IsNaN(xposition) ? 0 : xposition;
                RectWidth = hourHeight;
                RectVisibility = Visibility.Visible;
                schedule.Currentselecteddate = selectedDateTime;
                schedule.InternalSelectedDate = selectedDateTime;
                schedule.SelectedDate = selectedDateTime.Date;
                schedule.MinMaxSelection = schedule.Currentselecteddate;
                AdjustHrScroll();
            }
        }

        #endregion

        #region AdjustScroll

        internal void AdjustHrScroll()
        {
            if (RectXPosition + RectWidth > timelinescroll.HorizontalOffset + timelinescroll.ViewportWidth)
            {
                timelinescroll.ScrollToHorizontalOffset(RectXPosition + RectWidth - timelinescroll.ViewportWidth);
            }
            if (RectXPosition < timelinescroll.HorizontalOffset)
            {
                timelinescroll.ScrollToHorizontalOffset(RectXPosition);
            }
        }
        #endregion

        #region GethourHeight
        internal double GetTimelineHourHeight()
        {
            var timeSlotItemsControl = this.FindElementOfType<ScheduleTimelineTimeSlotItemsControl>();
            double timeslotWidth = timeSlotItemsControl.ActualWidth;
            int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            double columnPerDay = interval * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
            hourHeight = timeslotWidth / (SelectedDates.Count * columnPerDay);
            return hourHeight;
        }
        #endregion

        #region Move Left Selection rectangle
        internal void MoveLeftSelectionRect(DateTime selectedDateTime)
        {
            int interval = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            if (!ShowNonWorkingHours)
            {
                double selectedTime = selectedDateTime.TimeOfDay.TotalMinutes / 60;
                if (selectedTime < ScheduleTimeLineItemsControl.MinValue)
                {
                    DateTime prevDate = new DateTime();
                    prevDate = selectedDateTime.AddDays(-1).Date;
                    prevDate = prevDate.AddHours(ScheduleTimeLineItemsControl.MaxValue).AddMinutes(-(60/interval));
                    selectedDateTime = prevDate;
                }
            }
            if (!(SelectedDates.Contains(selectedDateTime.Date)))
            {
                var timeSlotItemsControl = this.FindElementOfType<ScheduleTimelineTimeSlotItemsControl>();
                double timeslotWidth = timeSlotItemsControl.ActualWidth;
                schedule.MoveToDate(selectedDateTime.Date);
                hourHeight = timeslotWidth / (SelectedDates.Count * interval * 24);
                RectXPosition = timeslotWidth - hourHeight;
            }
            MoveSelectionRectToDateTime(selectedDateTime);
            //timelinescroll.ScrollToHorizontalOffset(timelinescroll.ExtentWidth- timelinescroll.ViewportWidth);

        }
        #endregion

        #region Move down selection rectangle

        internal void MoveDownSelectionRect(DateTime selectedDateTime)
        {
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                var tmslot = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                var individualht = (int)(tmslot.ActualHeight / ScheduleResourceType.ResourceCollection.Count);
                Point position = schedule.SelectedPoint;
                position.Y = position.Y + individualht;
                int resIndex = (int)((position.Y) / individualht) >= ScheduleResourceType.ResourceCollection.Count ? ScheduleResourceType.ResourceCollection.Count - 1 : (int)((position.Y) / individualht) < 0 ? 0 : (int)((position.Y) / individualht);
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    GetSelectedResourceName(resIndex);
                }
                RectYPosition = resIndex * individualht;
                RectHeight = individualht;
            }
        }

        #endregion

        #region Move up selection rectangle

        internal void MoveUpSelectionRect(DateTime selectedDateTime)
        {
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                var tmslot = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                var individualht = (int)(tmslot.ActualHeight / ScheduleResourceType.ResourceCollection.Count);
                Point position = schedule.SelectedPoint;
                position.Y = position.Y - individualht;
                int resIndex = (int)((position.Y) / individualht) >= ScheduleResourceType.ResourceCollection.Count ? ScheduleResourceType.ResourceCollection.Count - 1 : (int)((position.Y) / individualht) < 0 ? 0 : (int)((position.Y) / individualht);
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    GetSelectedResourceName(resIndex);
                }
                RectYPosition = resIndex * individualht;
                RectHeight = individualht;
            }
        }

        #endregion

        #region Add selection left
        internal void AddSelectionLeft(DateTime MinMaxSelection)
        {
            hourHeight = GetTimelineHourHeight();
            RectXPosition = RectXPosition - hourHeight;
            RectWidth = RectWidth + hourHeight;
            AdjustHrScroll();
        }
        #endregion

        #region Sub selection left
        internal void SubSelectionLeft(DateTime MinMaxSelection)
        {
            hourHeight = GetTimelineHourHeight();
            RectWidth = RectWidth - hourHeight;
            AdjustHrScroll();
        }
        #endregion

        #region Add selection right
        internal void AddSelectionRight(DateTime MinMaxSelection)
        {
            hourHeight = GetTimelineHourHeight();
            RectWidth = RectWidth + hourHeight;
            AdjustHrScroll();
        }
        #endregion

        #region Sub selection right
        internal void SubSelectionRight(DateTime MinMaxSelection)
        {
            hourHeight = GetTimelineHourHeight();
            RectXPosition = RectXPosition + hourHeight;
            RectWidth = RectWidth - hourHeight;
            AdjustHrScroll();
        }
        #endregion

#endif

        #region Getting Current Selected Date

        internal DateTime GetSelectedDate(Point position, bool isDroppedDate)
        {
            var selectedDate = new DateTime();
            if (schedule != null)
            {
                int timeDiff = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue);
                var tmslot = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                hourHeight = tmslot.ActualWidth * timeDiff;
                double total = ((position.X / hourHeight) * timeDiff) % timeDiff;
                var timespan = (int)schedule.GetTimeInterval().TotalMinutes;
                var dayIndex = (int)Math.Floor(position.X / (hourHeight));
                if (dayIndex == 7)
                {
                    dayIndex = 6;
                }
                time = Math.Floor(total) + ScheduleTimeLineItemsControl.MinValue;
                double decimalvalue;
                int min = 0;

                if (isDroppedDate)
                {
                    decimalvalue = (total - (int)total) * 60;
                    if (timespan != 0)
                        min = (int)decimalvalue;
                }
                else
                {
                    if (time < 0)
                    {
                        time = 0;
                        total = 0;
                        position.X = 0;
                    }
                    decimalvalue = Math.Round(total - (int)total, 2) * 60;
                    if (timespan != 0)
                        min = ((int)decimalvalue / timespan) * timespan;
                }
                if (ScheduleResourceType != null)
                {
                    ResourceType maintype = ScheduleResourceType;
                    var individualht = (int)(tmslot.ActualHeight / maintype.ResourceCollection.Count);
                    int resIndex = (int)((position.Y) / individualht) >= ScheduleResourceType.ResourceCollection.Count ? ScheduleResourceType.ResourceCollection.Count - 1 : (int)((position.Y) / individualht) < 0 ? 0 : (int)((position.Y) / individualht);
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        GetSelectedResourceName(resIndex);
                    }
                }
                dayIndex = dayIndex > schedule.SelectedDates.Count - 1 ? schedule.SelectedDates.Count - 1 : dayIndex < 0 ? 0 : dayIndex;
                selectedDate = SelectedDates[dayIndex];
                selectedDate = selectedDate.AddHours(time).AddMinutes(min);
            }
            return selectedDate;
        }

        #endregion

        #region Get Selected Resourcename

        internal void GetSelectedResourceName(int index)
        {
            schedule.selectedResourcename = new List<Resource>();
            var selectionresourse = new Resource();
            ResourceType maintype = ScheduleResourceType;
            selectionresourse.ResourceName = maintype.ResourceCollection.ElementAt(index).ResourceName;
            selectionresourse.TypeName = maintype.TypeName;
            schedule.selectedResourcename.Add(selectionresourse);
            maintype = maintype.SubResourceType;
            while (maintype != null)
            {
                var tempselectionresource = new Resource { ResourceName = maintype.ResourceCollection[0].ResourceName, TypeName = maintype.TypeName };
                schedule.selectedResourcename.Add(tempselectionresource);
                maintype = maintype.SubResourceType;
            }
        }
        #endregion

        #region Toggling NavigationTap Visibility

        internal void SetNavigationTapVisibility()
        {
            if (schedule != null)
            {
                int count = VisibleAppointments.Count;
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
                        ObservableCollection<ScheduleAppointment> filteredNextAppointments, filteredPrevAppointments;
                        if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0 && schedule.Resource != string.Empty)
                        {
                            filteredNextAppointments = schedule.ProxyAppointments.OrderBy(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date > SelectedDates[SelectedDates.Count - 1].Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)) != null)).Value;
                            filteredPrevAppointments = schedule.ProxyAppointments.OrderBy(x => x.Key.Date).ToArray().FirstOrDefault(x => x.Key.Date < SelectedDates[0].Date && ((x.Value.FirstOrDefault(m => m.ResourceCollection.FirstOrDefault(res => res.TypeName == schedule.Resource) != null)) != null)).Value;
                        }
                        else
                        {
                            filteredNextAppointments = schedule.ProxyAppointments.FirstOrDefault(x => x.Key.Date > SelectedDates[SelectedDates.Count - 1].Date).Value;
                            filteredPrevAppointments = schedule.ProxyAppointments.FirstOrDefault(x => x.Key.Date < SelectedDates[0].Date).Value;
                        }
                        if (filteredPrevAppointments != null && filteredNextAppointments != null)
                        {
                            if (previousNavigationTap != null)
                            {
                                if (CheckVisibleAppointments(filteredPrevAppointments))
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                                else
                                {
                                    previousNavigationTap.IsHitTestVisible = false;
                                    previousNavigationTap.Opacity = 0.5;
                                }
                            }
                            if (nextNavigationTap != null)
                            {
                                if (CheckVisibleAppointments(filteredNextAppointments))
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                                else
                                {
                                    nextNavigationTap.IsHitTestVisible = false;
                                    nextNavigationTap.Opacity = 0.5;
                                }
                            }
                        }
                        else if (filteredNextAppointments != null)
                        {
                            if (nextNavigationTap != null)
                            {
                                if (CheckVisibleAppointments(filteredNextAppointments))
                                {
                                    nextNavigationTap.IsHitTestVisible = true;
                                    nextNavigationTap.Opacity = 1;
                                }
                                else
                                {
                                    nextNavigationTap.IsHitTestVisible = false;
                                    nextNavigationTap.Opacity = 0.5;
                                }
                            }
                            if (previousNavigationTap != null)
                            {
                                previousNavigationTap.IsHitTestVisible = false;
                                previousNavigationTap.Opacity = 0.5;
                            }
                        }
                        else if (filteredPrevAppointments != null)
                        {
                            if (nextNavigationTap != null)
                            {
                                nextNavigationTap.IsHitTestVisible = false;
                                nextNavigationTap.Opacity = 0.5;
                            }
                            if (previousNavigationTap != null)
                            {
                                if (CheckVisibleAppointments(filteredPrevAppointments))
                                {
                                    previousNavigationTap.IsHitTestVisible = true;
                                    previousNavigationTap.Opacity = 1;
                                }
                                else
                                {
                                    previousNavigationTap.IsHitTestVisible = false;
                                    previousNavigationTap.Opacity = 0.5;
                                }
                            }
                        }
                    }
                    else
                    {
                        nextNavigationTap.Opacity = 0;
                        previousNavigationTap.Opacity = 0;
                    }
                }
                RectVisibility = SelectedDates.Contains(schedule.SelectedDate.Date) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private bool CheckVisibleAppointments(ObservableCollection<ScheduleAppointment> filteredAppointments)
        {
            if (!ShowNonWorkingHours)
            {
                var visibleAppointments = filteredAppointments.Where(x => x.StartTime.Hour < WorkEndHour &&
                                                                          x.EndTime.Hour > WorkStartHour);
                return (visibleAppointments.ToList().Count > 0);
            }
            return true;
        }

        #endregion

        #region Enabling Drag & Drop

        internal void EnableDragDrop(SfSchedule sfSchedule)
        {
            var tmslot = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
            var timelineControl = this.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>();
            var dayHeaderControl = this.FindElementOfType<ScheduleHorizontalDaysHeaderViewItemsControl>();
            var sch = this.FindParentElementOfType<SfSchedule>();
            var grid = sch.FindElementOfType<Grid>();
            var schHeader = grid.FindElementOfType<HeaderTitleBarView>();
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            if (obj != null)
            {
                var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                sfSchedule.timelineScrollViewer = timeScrollviewer;
            }
#if WINRT
            double xPosition = sfSchedule.currentpoint.Position.X - sfSchedule.Appointmentpoint.Position.X;
            double scrollXpos = sfSchedule.Scrollpoint.Position.X - sfSchedule.Appointmentpoint.Position.X;
#else
            double xPosition = sfSchedule.currentpoint.X - sfSchedule.Appointmentpoint.X;
            double scrollXpos = sfSchedule.Scrollpoint.X - sfSchedule.Appointmentpoint.X;
#endif
            double appWidth = sfSchedule.FloatingAppointmentSize.Width;
            var drag_app = new ScheduleHorizontalAppointmentViewControl
            {
                AppWidth = sfSchedule.FloatingAppointmentSize.Width,
                DataContext = sfSchedule.hvc.DataContext,
                DragRectangleVisibility = Visibility.Visible
            };
            double leftend;
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                leftend = sfSchedule.DragDropCanvas.ActualWidth - timelinescroll.ViewportWidth + timeLineViewItemHeaderWidth - 5;
            }
            else
            {
                leftend = sfSchedule.DragDropCanvas.ActualWidth - timelinescroll.ViewportWidth - 5;
            }
            //added for drag drop feature
            if (xPosition < leftend)
            {
                drag_app.Width = appWidth - (leftend - xPosition);
                drag_app.AppWidth = appWidth - (leftend - xPosition);
                xPosition = leftend;
            }
            else if (scrollXpos + appWidth > timelinescroll.ViewportWidth)
            {
                drag_app.Width = timelinescroll.ViewportWidth - scrollXpos;
                drag_app.AppWidth = timelinescroll.ViewportWidth - scrollXpos;
            }
            else
            {
                drag_app.Width = appWidth;
                drag_app.AppWidth = appWidth;
            }
            int individualht;
            if (ScheduleResourceType != null)
            {
                int resourceCount = ScheduleResourceType.ResourceCollection.Count;
                individualht = (int)(tmslot.ActualHeight / resourceCount);
            }
            else
            {
                individualht = (int)(tmslot.ActualHeight);
            }
#if WINRT
            double topPosition = sfSchedule.currentpoint.Position.Y - sfSchedule.Appointmentpoint.Position.Y - (individualht - sfSchedule.FloatingAppointmentSize.Height);
            double rowcount = Math.Floor((topPosition - timelineControl.ActualHeight - dayHeaderControl.ActualHeight - schHeader.ActualHeight - 10) / individualht);
            rowcount = rowcount + 1;
            Canvas.SetLeft(drag_app, xPosition);
            Canvas.SetTop(drag_app, (rowcount * individualht) + timelineControl.ActualHeight + dayHeaderControl.ActualHeight + schHeader.ActualHeight + 5);

            drag_app.PointerPressed += sfSchedule.drag_app_PointerPressed;
#else
            double topPosition = sfSchedule.currentpoint.Y - sfSchedule.Appointmentpoint.Y - (individualht - sfSchedule.FloatingAppointmentSize.Height);
            double rowcount = Math.Floor((topPosition - timelineControl.ActualHeight - dayHeaderControl.ActualHeight - schHeader.ActualHeight - 10) / individualht);
            rowcount = rowcount + 1;
            Canvas.SetLeft(drag_app, xPosition);
            Canvas.SetTop(drag_app, (rowcount * individualht) + timelineControl.ActualHeight + dayHeaderControl.ActualHeight + schHeader.ActualHeight + 2);
            drag_app.MouseLeftButtonDown += sfSchedule.drag_app_MouseLeftButtonDown;
#endif
            drag_app.Width = sfSchedule.FloatingAppointmentSize.Width;
            drag_app.Height = individualht - 2;
            drag_app.Loaded += sfSchedule.drag_app_Loaded;

            sfSchedule.DragDropCanvas.Children.Add(drag_app);
        }

        #endregion

        #region Releasing Drag & Drop

#if WINRT
        internal void ReleaseDragDrop(SfSchedule sfSchedule, PointerRoutedEventArgs e, ScheduleTimeLineView timelineview1, ScheduleTimeLineView timelineview2, ScheduleTimeLineView timelineview3)
        {
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            if (obj != null)
            {
                var realStartDate = new DateTime();
                var realEndDate = new DateTime();
                var dropappointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                if (dropappointment != null)
                {
                    var selectedDate = sfSchedule.GetCurrentDropLocation(sfSchedule.viewcontrol, e);
                    double tempIntervalHeight = sfSchedule.isIntervalHeightset ? IntervalHeight : 80;
                    if (!(selectedDate == new DateTime()))
                    {
                        sfSchedule.hvc.DragDropStartTime = selectedDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                        DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (tempIntervalHeight);
                        DateTime EndDate = selectedDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                        if (SelectedDates.Count > 1 && !ShowNonWorkingHours &&
                            EndDate.TimeOfDay.TotalMinutes > WorkEndHour * 60)
                        {
                            TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, WorkStartHour - WorkEndHour, 0, 0);
                            EndDate = EndDate + nonWorkingHourTimeSpan;
                        }
                        if (!(dragDropCanvasWidth == sfSchedule.hvc.ActualWidth))
                        {
                            double diffHeight = dragDropCanvasWidth - sfSchedule.hvc.ActualWidth;
                            double diffTime = diffHeight / tempIntervalHeight;
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
                        var selectedDatesCount = SelectedDates.Count;
                        if (!(dragDropCanvasWidth == sfSchedule.hvc.ActualWidth))
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
                        sfSchedule.hvc.DragDropEndTime = EndDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                        DateTime starttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        DateTime endtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                        if (dropappointment.IsRecursive)
                        {
                            int _totalRecursiveCount = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID].Count;
                            DateTime _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0];
                            DateTime _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                            ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID];
                            if (dropappointment.StartTime.Date >= _firstApppointmentStartTime && dropappointment.EndTime.Date <= _lastApppointmentEndTime.Date)
                            {
                                if (dropappointment.StartTime.Date == _firstApppointmentStartTime)
                                {
                                    if (starttime.Date < sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][1].Date)
                                    {
                                        dropappointment.StartTime = starttime;
                                        dropappointment.EndTime = endtime;
                                        _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0] = starttime;
                                    }
                                    else
                                    {
                                        sfSchedule.DragDropCanvas.Children.Clear();
                                    }
                                }
                                else if (dropappointment.EndTime.Date == _lastApppointmentEndTime)
                                {
                                    if (starttime.Date > sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 2].Date)
                                    {
                                        dropappointment.StartTime = starttime;
                                        dropappointment.EndTime = endtime;
                                        sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] = endtime;
                                        _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                                    }
                                    else
                                    {
                                        sfSchedule.DragDropCanvas.Children.Clear();
                                    }
                                }
                                else if (starttime.Date >= _firstApppointmentStartTime && endtime.Date <= _lastApppointmentEndTime)
                                {
                                    int _indexCount = 0;
                                    int _droppedAppointmentIndex = 0;


                                    foreach (var item in _recursiveAppCollection)
                                    {
                                        if (dropappointment.StartTime.Date == ((DateTime)item).Date)//&& daystarttime.Date > sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_indexCount + 1] && daystarttime.Date < sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_indexCount - 1])
                                        {
                                            _droppedAppointmentIndex = _indexCount;

                                        }
                                        _indexCount++;
                                    }
                                    sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_droppedAppointmentIndex] = starttime;
                                    dropappointment.StartTime = starttime;
                                    dropappointment.EndTime = endtime;
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
                            foreach (object objec in ((IEnumerable)sfSchedule.ItemsSource))
                            {
                                Type type = objec.GetType();
                                if (objec.GetHashCode() == (int)dropappointment.ObjectID)
                                {
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(objec, starttime, null);
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(objec, endtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                dropappointment.StartTime = starttime;
                                dropappointment.EndTime = endtime;
                            }
                        }
                        else
                        {
                            dropappointment.StartTime = starttime;
                            dropappointment.EndTime = endtime;
                        }

                        if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                        {
                            sfSchedule.AddResources(dropappointment);
                            timelineview1.SetupAppointments();
                            timelineview2.SetupAppointments();
                            timelineview3.SetupAppointments();
                        }
                    }
                    if (timeScrollviewer != null)
                        timeScrollviewer.VerticalScrollMode = ScrollMode.Auto;
                }
            }
            DatetimeDiff = DayDiff = 0;
            if (sfSchedule.Timelineviewrb.EllipseAnimation1 != null)
            {
                sfSchedule.Timelineviewrb.EllipseAnimation1.Begin();
                sfSchedule.Timelineviewrb.EllipseAnimation2.Begin();
            }
            sfSchedule.hvc = null;
        }
#else
        internal void ReleaseDragDrop(SfSchedule sfSchedule, MouseButtonEventArgs e)
        {
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            if (obj != null)
            {
                var realStartDate = new DateTime();
                var realEndDate = new DateTime();
                var dropappointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                if (dropappointment != null)
                {
                    TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                    var selectedDate = sfSchedule.GetCurrentDropLocationForMouseButtonEventArgs(sfSchedule.currentSelectedItem, e);
                    double tempIntervalHeight = sfSchedule.isIntervalHeightset ? IntervalHeight : 80;
                    if (!(selectedDate == new DateTime()))
                    {
                        sfSchedule.hvc.DragDropStartTime = selectedDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                        DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (tempIntervalHeight);
                        DateTime EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                        if (SelectedDates.Count > 1 && !ShowNonWorkingHours &&
                            EndDate.TimeOfDay.TotalMinutes > WorkEndHour * 60)
                        {
                            TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, WorkStartHour - WorkEndHour, 0, 0);
                            EndDate = EndDate + nonWorkingHourTimeSpan;
                        }
                        if (!(dragDropCanvasWidth == sfSchedule.hvc.ActualWidth))
                        {
                            double diffHeight = dragDropCanvasWidth - sfSchedule.hvc.ActualWidth;
                            double diffTime = diffHeight / tempIntervalHeight;
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
                        var selectedDatesCount = SelectedDates.Count;
                        if (!(dragDropCanvasWidth == sfSchedule.hvc.ActualWidth))
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
                        sfSchedule.hvc.DragDropEndTime = EndDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                        DateTime starttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                        DateTime endtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                        if (dropappointment.IsRecursive)
                        {
                            int _totalRecursiveCount = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID].Count;
                            DateTime _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0];
                            DateTime _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                            ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID];
                            if (dropappointment.StartTime.Date >= _firstApppointmentStartTime && dropappointment.EndTime.Date <= _lastApppointmentEndTime.Date)
                            {
                                if (dropappointment.StartTime.Date == _firstApppointmentStartTime.Date)
                                {
                                    if (starttime.Date < sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][1].Date)
                                    {
                                        dropappointment.StartTime = starttime;
                                        dropappointment.EndTime = endtime;
                                        _firstApppointmentStartTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][0] = starttime;
                                    }
                                    else
                                    {
                                        sfSchedule.DragDropCanvas.Children.Clear();
                                    }
                                }
                                else if (dropappointment.EndTime.Date == _lastApppointmentEndTime.Date)
                                {
                                    if (starttime.Date > sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 2].Date)
                                    {
                                        dropappointment.StartTime = starttime;
                                        dropappointment.EndTime = endtime;
                                        sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] = endtime;
                                        _lastApppointmentEndTime = sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_totalRecursiveCount - 1] + TimeSpan.FromHours(DatetimeDiff);
                                    }
                                    else
                                    {
                                        sfSchedule.DragDropCanvas.Children.Clear();
                                    }
                                }
                                else if (starttime.Date > _firstApppointmentStartTime && starttime.Date < _lastApppointmentEndTime && endtime.Date < _lastApppointmentEndTime.Date)
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
                                    sfSchedule.RecursiveAddedDates[dropappointment.RecurrenceID][_droppedAppointmentIndex] = starttime;
                                    dropappointment.StartTime = starttime;
                                    dropappointment.EndTime = endtime;
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
                            foreach (object objec in ((IEnumerable)sfSchedule.ItemsSource))
                            {
                                Type type = objec.GetType();
                                if (objec.GetHashCode() == (int)dropappointment.ObjectID)
                                {
                                    type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(objec, starttime, null);
                                    type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(objec, endtime, null);
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                //This property is set to avoid adding of items in proxy appointment dictionary  
                                sfSchedule.stopUpdate = true;
                                dropappointment.StartTime = starttime;
                                sfSchedule.stopUpdate = false;
                                dropappointment.EndTime = endtime;
                            }
                        }
                        else
                        {
                            //This property is set to avoid adding of items in proxy appointment dictionary  
                            sfSchedule.stopUpdate = true;
                            dropappointment.StartTime = starttime;
                            sfSchedule.stopUpdate = false;
                            dropappointment.EndTime = endtime;
                        }

                        if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                        {
                            sfSchedule.AddResources(dropappointment);
                            SetupAppointments();
                        }
                    }
                }
            }
            DatetimeDiff = DayDiff = 0;
            sfSchedule.hvc = null;
        }
#endif

        #endregion

        #region Releasing Resize

        internal void ReleaseResize(SfSchedule sfSchedule)
        {
            if (sfSchedule.hvc != null)
            {
                var selectedDaysCount = sfSchedule.SelectedDates.Count;
                var scheduleAppointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                bool multiday = false;
                bool containitems = sfSchedule.CheckItemsInItemsSource();
                if (scheduleAppointment != null)
                {
                    if (scheduleAppointment.InternalEndTime.Date.Date > scheduleAppointment.InternalStartTime.Date.Date)
                    {
                        multiday = true;
                    }
                    if (sfSchedule.isIntervalHeightset)
                    {
                        DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (IntervalHeight);
                    }
                    else
                    {
                        DatetimeDiff = (sfSchedule.hvc.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                    }
                    DateTime hrendtime =  new DateTime();
                    if (sfSchedule.Timelineviewrb.resizePart == ResizeBehavior.ControlParts.Right)
                    {
                        
                        if (scheduleAppointment.IsRecursive)
                        {
                            DateTime _endTime = scheduleAppointment.InternalStartTime.AddMinutes(
                                            sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                            ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID];
                            bool _canResizeApp = true;
                            foreach (var item in _recursiveAppCollection)
                            {

                                if (_endTime.Date == (DateTime)item.Date)
                                {
                                    int _appointmentIndex = _recursiveAppCollection.IndexOf((DateTime)item);
                                    if (_endTime.Date < _recursiveAppCollection[_appointmentIndex].Date)
                                    {
                                        hrendtime = ScheduleAppointment.ConvertToActualTime(_endTime, "EndTime", null,
                                            scheduleAppointment.EndTimeZone);
                                        _canResizeApp = true;
                                    }
                                    else
                                    {
                                        _canResizeApp = false;
                                    }
                                }
                            }
                            if (!_canResizeApp)
                            {
                                hrendtime = scheduleAppointment.InternalEndTime;
                            }
                        }
                        else   if (!multiday || selectedDaysCount > 1)
                        {
                            DateTime end = scheduleAppointment.InternalStartTime.AddMinutes(
                                           sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                            if (!ShowNonWorkingHours && end.TimeOfDay.TotalMinutes > WorkEndHour * 60)
                            {
                                TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, WorkStartHour - WorkEndHour, 0, 0);
                                end = end + nonWorkingHourTimeSpan;
                            }
                            hrendtime = ScheduleAppointment.ConvertToActualTime(end, "EndTime", null,
                                scheduleAppointment.EndTimeZone);
                        }
                        else
                        {
                            DateTime currentDate = scheduleAppointment.InternalEndTime.Date;
                            hrendtime =
                                currentDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                        }
                        if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null &&
                            sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty &&
                            sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                        {
                            bool ispropertyset = false;
                            foreach (object obj in ((IEnumerable)sfSchedule.ItemsSource))
                            {
                                Type type = obj.GetType();
                                if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                                {
#if WINRT
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(obj, hrendtime, null);
#else
                                    type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping)
                                        .SetValue(obj, hrendtime, null);
#endif
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                scheduleAppointment.EndTime = hrendtime;
                            }
                        }
                        else
                        {
                            scheduleAppointment.EndTime = hrendtime;
                        }
                    }
                    else if (sfSchedule.Timelineviewrb.resizePart == ResizeBehavior.ControlParts.Left)
                    {
                        DateTime hrstarttime = new DateTime();
                        if (scheduleAppointment.IsRecursive)
                        {
                            DateTime _startTime = scheduleAppointment.InternalEndTime.AddMinutes(-(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff));
                            ObservableCollection<DateTime> _recursiveAppCollection = sfSchedule.RecursiveAddedDates[scheduleAppointment.RecurrenceID];
                            bool _canResizeApp = true;
                            foreach (var item in _recursiveAppCollection)
                            {

                                if (_startTime.Date == (DateTime)item.Date)
                                {
                                    int _appointmentIndex = _recursiveAppCollection.IndexOf((DateTime)item);
                                    if (_startTime.Date > _recursiveAppCollection[_appointmentIndex].Date)
                                    {
                                        hrstarttime = ScheduleAppointment.ConvertToActualTime(_startTime, "StartTime",
                                scheduleAppointment.StartTimeZone, null);
                                        _canResizeApp = true;
                                    }
                                    else
                                    {
                                        _canResizeApp = false;
                                    }
                                }
                            }
                            if (!_canResizeApp)
                            {
                                hrstarttime = scheduleAppointment.InternalStartTime;
                            }
                        }
                       else if (!multiday || selectedDaysCount > 1)
                        {
                            DateTime start = scheduleAppointment.InternalEndTime.AddMinutes(-(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff));
                            if (!ShowNonWorkingHours && start.TimeOfDay.TotalMinutes < WorkStartHour * 60)
                            {
                                TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, WorkStartHour - WorkEndHour, 0, 0);
                                start = start - nonWorkingHourTimeSpan;
                            }
                            hrstarttime = ScheduleAppointment.ConvertToActualTime(start, "StartTime",
                                scheduleAppointment.StartTimeZone, null);
                        }
                        else
                        {
                            DateTime currentDate = scheduleAppointment.InternalStartTime.Date.AddHours(24);
                            hrstarttime =
                                currentDate.AddMinutes(
                                    -(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff));
                        }
                        if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null &&
                            sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty &&
                            sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && containitems)
                        {
                            bool ispropertyset = false;
                            foreach (object obj in ((IEnumerable)sfSchedule.ItemsSource))
                            {
                                Type type = obj.GetType();
                                if (obj.GetHashCode() == (int)scheduleAppointment.ObjectID)
                                {
#if WINRT
                                    type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(obj, hrstarttime, null);
#else
                                    type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping)
                                        .SetValue(obj, hrstarttime, null);
#endif
                                    ispropertyset = true;
                                    break;
                                }
                            }
                            if (!ispropertyset)
                            {
                                scheduleAppointment.StartTime = hrstarttime;
                            }
                        }
                        else
                        {
                            scheduleAppointment.StartTime = hrstarttime;
                        }
                    }
                }

                var appointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                if (appointment != null)
                    DayDiff = appointment.InternalEndTime.DayOfYear - appointment.InternalStartTime.DayOfYear;
#if WINRT
                sfSchedule.Timelineviewrb.EllipseAnimation1.Begin();
                sfSchedule.Timelineviewrb.EllipseAnimation2.Begin();
#endif
            }
        }

        #endregion

        #region Moving Dragged Appointment

#if WINRT
        internal void MoveDragDrop(SfSchedule sfSchedule, bool check, PointerRoutedEventArgs e, ScheduleTimeLineView timelineview1, ScheduleTimeLineView timelineview2, ScheduleTimeLineView timelineview3)
        {
            if (check)
            {
                var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
                if (obj != null)
                {
                    var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                    var selectedDate = sfSchedule.GetCurrentDropLocation(this, e);
                    DateTime EndDate;
                    if (DatetimeDiff.Equals(0))
                    {
                        if (sfSchedule.isIntervalHeightset)
                        {
                            DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (IntervalHeight);
                        }
                        else
                        {
                            DatetimeDiff = (sfSchedule.hvc.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                        }
                    }
                    EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                    if (selectedDate == EndDate)
                    {
                        EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                    }
                    var dropappointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                    if (dropappointment != null)
                    {
                        dropappointment.StartTime = selectedDate;
                        dropappointment.EndTime = EndDate;
                        if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                        {
                            sfSchedule.AddResources(dropappointment);
                            timelineview1.SetupAppointments();
                            timelineview2.SetupAppointments();
                            timelineview3.SetupAppointments();
                        }
                    }
                    if (timeScrollviewer != null)
                        timeScrollviewer.VerticalScrollMode = ScrollMode.Auto;
                }
                DatetimeDiff = DayDiff = 0;
                sfSchedule.hvc = null;
            }

            if (sfSchedule.hvc != null)
            {
                #region DragDropTimer
                sfSchedule.hvc = (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleHorizontalAppointmentViewControl>();
                sfSchedule.SelectedAppointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                DateTime selectedDate1;
                var mainitem1 = (sfSchedule.flipviewselecteditem).FindElementOfTypeWithName<ContentControl>("PART_MainViewItems");
                selectedDate1 = sfSchedule.GetCurrentDropLocation(mainitem1, e);//.AddTimeSpan(SelectedAppointment.InternalStartTime.TimeOfDay); ;
                var dayView = sfSchedule.hvc;
                dayView.DragDropStartTime = selectedDate1.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                double DatetimeDiff1;
                if (sfSchedule.isIntervalHeightset)
                {
                    DatetimeDiff1 = (sfSchedule.hvc.ActualWidth) / (IntervalHeight);
                }
                else
                {
                    DatetimeDiff1 = (sfSchedule.hvc.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                }
                DateTime currentDate = selectedDate1;
                DateTime endDate = currentDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff1);
                if (SelectedDates.Count > 1 && !ShowNonWorkingHours &&
                    endDate.TimeOfDay.TotalMinutes > WorkEndHour * 60)
                {
                    TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, WorkStartHour - WorkEndHour, 0, 0);
                    endDate = endDate + nonWorkingHourTimeSpan;
                }
                dayView.DragDropEndTime = endDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                #endregion

                #region TimelineViewControl
                sfSchedule.isscrollmoveondragging = true;
                var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
                var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
                if (obj != null)
                {
                    var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                    if (timeScrollviewer != null)
                    {
                        double tempIntervalHeight = sfSchedule.isIntervalHeightset ? IntervalHeight : 80;
                        double posinScroll = e.GetCurrentPoint(timeScrollviewer).Position.X;
                        double posinCanvas = e.GetCurrentPoint(sfSchedule.DragDropCanvas.Children[0]).Position.X;
                        if (posinScroll >= 0 && posinCanvas < drag_app.ActualWidth
                        && posinScroll - posinCanvas + drag_app.ActualWidth < timeScrollviewer.ViewportWidth)
                        {
                            if (posinScroll - tempIntervalHeight < 0 && timeScrollviewer.HorizontalOffset > 0)
                            {
                                timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset - 20);
                            }
                            else if (posinScroll - sfSchedule.Appointmentpoint.Position.X < 0)
                            {
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.Position.X);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.Position.X);
                                Canvas.SetLeft(drag_app, sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                dragResizeFlag = true;
                            }
                            else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                            {
                                double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                                previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                                Canvas.SetLeft(drag_app, previousXValue);
                                dragResizeFlag = false;
                            }
                            else
                            {
                                previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                                if ((sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                                {
                                    double diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                                    {
                                        double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                                        Canvas.SetLeft(drag_app, 5 + timeLineViewItemHeaderWidth);
                                    }
                                    else
                                    {
                                        //Canvas.SetLeft(drag_app, 5);
                                    }
                                }
                                else
                                {
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth;
                                    Canvas.SetLeft(drag_app, previousXValue);
                                }
                            }
                        }
                        else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth && posinCanvas < drag_app.ActualWidth)
                        {
                            if (posinScroll + tempIntervalHeight > timeScrollviewer.ViewportWidth && timeScrollviewer.HorizontalOffset < timeScrollviewer.ExtentWidth - timeScrollviewer.ViewportWidth)
                            {
                                timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset + 20);//(pos + drag_app.ActualWidth - timeScrollviewer.ViewportWidth));
                            }
                            else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                            {
                                double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                                previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                                if (posinCanvas == posinScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                                {
                                    diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                                }
                                Canvas.SetLeft(drag_app, previousXValue);
                                dragResizeFlag = false;
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth;
                            previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                            Canvas.SetLeft(drag_app, previousXValue);
                        }
                        //if (drag_app != null && pos + drag_app.ActualWidth > timeScrollviewer.ViewportWidth)
                        //{
                        //    timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset + 30);
                        //}
                        //else if (pos - sfSchedule.Appointmentpoint.Position.X < 0)
                        //{
                        //    timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset - 30);// sfSchedule.Appointmentpoint.Position.X);
                        //}
                    }
                }
                var timelineControl = this.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>();
                //if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                //{
                //    double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                //    if ((e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) > timeLineViewItemHeaderWidth + 5 && (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) < timelineControl.ActualWidth + timeLineViewItemHeaderWidth)
                //        Canvas.SetLeft(drag_app, (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X));
                //}

                //else if ((e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) > 5 && (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) < timelineControl.ActualWidth)
                //{
                //    Canvas.SetLeft(drag_app, (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X));
                //}
                double resourcecount = 1;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    resourcecount = ScheduleResourceType.ResourceCollection.Count;
                }
                var headerControl = sfSchedule.FindElementOfType<HeaderTitleBarView>();
                double daysHeaderViewHeight = this.FindElementOfType<ScheduleHorizontalDaysHeaderViewItemsControl>().ActualHeight;
                double Rowheight = this.FindElementOfType<ScheduleTimelineTimeSlotItemsControl>().ActualHeight / resourcecount;
                var currentdragheight = (int)((e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - (daysHeaderViewHeight) - timelineControl.Height - headerControl.ActualHeight - 5) / Rowheight);
                if (currentdragheight < resourcecount && !(currentdragheight < 0) && resourcecount > 1)
                {
                    Canvas.SetTop(drag_app, (currentdragheight * Rowheight) + headerControl.ActualHeight + timelineControl.Height + daysHeaderViewHeight + 5);
                }

                #endregion
            }
            else
                sfSchedule.isscrollmoveondragging = false;
        }
#else
        internal void MoveDragDrop(SfSchedule sfSchedule, bool enableDrag, MouseEventArgs e)
        {
        #region DragDrop
            if (enableDrag)
            {
                var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
                if (obj != null)
                {
                    var dropappointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                    if (dropappointment != null)
                    {
                        TimeSpan datetimeDiff = dropappointment.InternalEndTime - dropappointment.InternalStartTime;
                        var selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                        if (!(selectedDate == new DateTime()))
                        {
                            if (DatetimeDiff.Equals(0))
                            {
                                if (sfSchedule.isIntervalHeightset)
                                {
                                    DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (IntervalHeight);
                                }
                                else
                                {
                                    DatetimeDiff = (sfSchedule.hvc.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                                }
                            }
                            DateTime EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                            if (selectedDate == EndDate)
                            {
                                EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                            }
                            var selectedDatesCount = SelectedDates.Count;
                            if (dropappointment.InternalEndTime.Date.Date > dropappointment.InternalStartTime.Date.Date)
                            {
                                EndDate = selectedDate.Add(datetimeDiff);
                                var scheduleHorizontalAppointmentViewControl = sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl;
                                if (scheduleHorizontalAppointmentViewControl != null)
                                {
                                    TimeSpan timeHeight;
                                    if (EndDate.Date > dropappointment.InternalStartTime.Date.Date &&
                                        selectedDatesCount == 1 && selectedDate.Date == dropappointment.InternalStartTime.Date.Date)
                                    {
                                        timeHeight = selectedDate.Date.AddHours(24) - selectedDate;
                                    }
                                    else
                                    {
                                        timeHeight = EndDate - selectedDate;
                                    }
                                    double intHeight = sfSchedule.isIntervalHeightset ? IntervalHeight : ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                                    double width = (timeHeight.TotalMilliseconds / sfSchedule.GetTimeInterval().TotalMilliseconds) * intHeight;
                                    scheduleHorizontalAppointmentViewControl.Width = width;
                                    scheduleHorizontalAppointmentViewControl.AppWidth = width;
                                }
                            }
                            sfSchedule.hvc.DragDropEndTime = EndDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                            DateTime starttime = ScheduleAppointment.ConvertToActualTime(selectedDate, "StartTime", dropappointment.StartTimeZone, null);
                            DateTime endtime = ScheduleAppointment.ConvertToActualTime(EndDate, "EndTime", null, dropappointment.EndTimeZone);
                            if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty &&
                                sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                            {
                                bool ispropertyset = false;
                                foreach (object objec in ((IEnumerable)sfSchedule.ItemsSource))
                                {
                                    Type type = objec.GetType();
                                    if (objec.GetHashCode() == (int)dropappointment.ObjectID)
                                    {
                                        type.GetProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(objec, starttime, null);
                                        type.GetProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(objec, endtime, null);
                                        ispropertyset = true;
                                        break;
                                    }
                                }
                                if (!ispropertyset)
                                {
                                    dropappointment.StartTime = starttime;
                                    dropappointment.EndTime = endtime;
                                }
                            }
                            else
                            {
                                dropappointment.StartTime = starttime;
                                dropappointment.EndTime = endtime;
                            }
                            if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                            {
                                sfSchedule.AddResources(dropappointment);
                                SetupAppointments();
                            }
                        }
                    }
                }
                DatetimeDiff = DayDiff = 0;
                sfSchedule.hvc = null;
            }
        #endregion

            if (sfSchedule.hvc != null)
            {
        #region DragDropTimer
                sfSchedule.hvc = (e.OriginalSource as FrameworkElement).FindParentElementOfType<ScheduleHorizontalAppointmentViewControl>();
                sfSchedule.SelectedAppointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                var selectedDate = sfSchedule.GetCurrentDropLocationForMouseEventArgs(sfSchedule.currentSelectedItem, e);
                sfSchedule.hvc.DragDropStartTime = selectedDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
                double DatetimeDiff1;
                if (sfSchedule.isIntervalHeightset)
                {
                    DatetimeDiff1 = (sfSchedule.hvc.ActualWidth) / (IntervalHeight);
                }
                else
                {
                    DatetimeDiff1 = (sfSchedule.hvc.ActualWidth) / ScheduleHorizontalTimeLineItemsControl.DefaultIntervalWidth;
                }
                DateTime currentDate = selectedDate;
                DateTime endDate = currentDate.AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff1);
                if (SelectedDates.Count > 1 && !ShowNonWorkingHours &&
                    endDate.TimeOfDay.TotalMinutes > WorkEndHour * 60)
                {
                    TimeSpan nonWorkingHourTimeSpan = new TimeSpan(1, WorkStartHour - WorkEndHour, 0, 0);
                    endDate = endDate + nonWorkingHourTimeSpan;
                }
                sfSchedule.hvc.DragDropEndTime = endDate.ToString(TimeMode == TimeModes.TwelveHours ? "hh:mm:tt" : "HH:mm");
        #endregion

        #region TimeLineViewControl
                sfSchedule.isscrollmoveondragging = true;
                var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
                var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
                if (obj != null)
                {
                    var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                    if (timeScrollviewer != null)
                    {
                        double tempIntervalHeight = sfSchedule.isIntervalHeightset ? IntervalHeight : 80;
                        double posinScroll = e.GetPosition(timeScrollviewer).X;
                        double posinCanvas = e.GetPosition(sfSchedule.DragDropCanvas.Children[0]).X;
                        if (posinScroll >= 0 && posinCanvas < drag_app.ActualWidth
                        && posinScroll - posinCanvas + drag_app.ActualWidth < timeScrollviewer.ViewportWidth)
                        {
                            if (posinScroll - tempIntervalHeight < 0 && timeScrollviewer.HorizontalOffset > 0)
                            {
                                timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset - 20);
                            }
                            else if (posinScroll - sfSchedule.Appointmentpoint.X < 0)
                            {
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.X);
                                //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.Position.X);
                                Canvas.SetLeft(drag_app, sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                dragResizeFlag = true;
                            }
                            else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                            {
                                double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                                //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                                previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                                Canvas.SetLeft(drag_app, previousXValue);
                                dragResizeFlag = false;
                            }
                            else
                            {
                                previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                                if ((sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                                {
                                    double diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                    //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                                    {
                                        double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                                        Canvas.SetLeft(drag_app, 5 + timeLineViewItemHeaderWidth);
                                    }
                                    else
                                    {
                                        //Canvas.SetLeft(drag_app, 5);
                                    }
                                }
                                else
                                {
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                                    Canvas.SetLeft(drag_app, previousXValue);
                                }
                            }
                        }
                        else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth && posinCanvas < drag_app.ActualWidth)
                        {
                            if (posinScroll + tempIntervalHeight > timeScrollviewer.ViewportWidth && timeScrollviewer.HorizontalOffset < timeScrollviewer.ExtentWidth - timeScrollviewer.ViewportWidth)
                            {
                                timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset + 20);//(pos + drag_app.ActualWidth - timeScrollviewer.ViewportWidth));
                            }
                            else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                            {
                                double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                                //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                                previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                                if (posinCanvas == posinScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                                {
                                    diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                                }
                                Canvas.SetLeft(drag_app, previousXValue);
                                dragResizeFlag = false;
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                            //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth;
                            previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                            Canvas.SetLeft(drag_app, previousXValue);
                        }
                    }
                }
                var timelineControl = this.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>();
                if (drag_app != null)
                {
                    //if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    //{
                    //    double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                    //    if ((e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) > timeLineViewItemHeaderWidth + 5 && (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) < timelineControl.ActualWidth + timeLineViewItemHeaderWidth)
                    //        Canvas.SetLeft(drag_app, (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X));
                    //}
                    //else if ((e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) > 5 && (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) < timelineControl.ActualWidth)
                    //{
                    //    Canvas.SetLeft(drag_app, (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X));
                    //}
                }
                double resourcecount = 1;
                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                {
                    resourcecount = ScheduleResourceType.ResourceCollection.Count;
                }
                var tmslot = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
                var dayHeaderControl = this.FindElementOfType<ScheduleHorizontalDaysHeaderViewItemsControl>();
                var sch = this.FindParentElementOfType<SfSchedule>();
                var grid = sch.FindElementOfType<Grid>();
                var schHeader = grid.FindElementOfType<HeaderTitleBarView>();

                int resourceCount = 1;
                if (ScheduleResourceType != null)
                    resourceCount = ScheduleResourceType.ResourceCollection.Count;
                var individualht = (int)(tmslot.ActualHeight / resourceCount);
                double topPosition = e.GetPosition(sfSchedule.DragDropCanvas).Y;// -sfSchedule.Appointmentpoint.Y - (individualht - FloatingAppointmentSize.Height);
                double rowcount = Math.Floor((topPosition - timelineControl.ActualHeight - dayHeaderControl.ActualHeight - schHeader.ActualHeight) / individualht);
                if (drag_app != null && rowcount <= resourcecount - 1 && !(rowcount < 0) && resourcecount > 1)
                    Canvas.SetTop(drag_app, (rowcount * individualht) + timelineControl.ActualHeight + dayHeaderControl.ActualHeight + schHeader.ActualHeight + (rowcount + 2));
        #endregion
            }
            else
                sfSchedule.isscrollmoveondragging = false;
        }
#endif
        #endregion

        #region Start Dragging Appointment
#if WINRT
        internal void StartDragDrop(SfSchedule sfSchedule, bool enableDrag, PointerRoutedEventArgs e, ScheduleTimeLineView timelineview1, ScheduleTimeLineView timelineview2, ScheduleTimeLineView timelineview3)
        {
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            if (obj != null)
            {
                var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;

                #region DragDrop
                if (enableDrag)
                {
                    var mainitem = (sfSchedule.flipviewselecteditem).FindElementOfType<ContentControl>();
                    while (mainitem is FlipViewItem)
                    {
                        var subelement = mainitem.FindElementOfType<Grid>();
                        mainitem = subelement.FindElementOfType<ContentControl>();
                    }
                    if (timeScrollviewer != null)
                    {
                        if (DatetimeDiff.Equals(0))
                        {
                            DatetimeDiff = (sfSchedule.hvc.ActualWidth) / (IntervalHeight);
                        }
                        var selectedDate = sfSchedule.GetCurrentDropLocation(mainitem, e);
                        var EndDate = selectedDate.AddDays(DayDiff).AddMinutes(sfSchedule.GetTimeInterval().TotalMinutes * DatetimeDiff);
                        if (selectedDate == EndDate)
                        {
                            EndDate = selectedDate.Add(sfSchedule.GetTimeInterval());
                        }
                        var dropappointment = sfSchedule.hvc.DataContext as ScheduleAppointment;
                        if (dropappointment != null)
                        {
                            DateTime starttime = selectedDate;
                            DateTime endtime = EndDate;
                            if (sfSchedule.ItemsSource != null && sfSchedule.AppointmentMapping != null && sfSchedule.AppointmentMapping.StartTimeMapping != string.Empty && sfSchedule.AppointmentMapping.EndTimeMapping != string.Empty && sfSchedule.CheckItemsInItemsSource())
                            {
                                bool ispropertyset = false;
                                foreach (object objec in ((IEnumerable)sfSchedule.ItemsSource))
                                {
                                    Type type = objec.GetType();
                                    if (objec.GetHashCode() == (int)dropappointment.ObjectID)
                                    {
                                        type.GetRuntimeProperty(sfSchedule.AppointmentMapping.StartTimeMapping).SetValue(objec, starttime, null);
                                        type.GetRuntimeProperty(sfSchedule.AppointmentMapping.EndTimeMapping).SetValue(objec, endtime, null);
                                        ispropertyset = true;
                                        break;
                                    }
                                }
                                if (!ispropertyset)
                                {
                                    dropappointment.StartTime = starttime;
                                    dropappointment.EndTime = endtime;
                                }
                            }
                            else
                            {
                                dropappointment.StartTime = starttime;
                                dropappointment.EndTime = endtime;
                            }
                            if (sfSchedule.selectedResourcename.Count > 0 && sfSchedule.Resource != string.Empty)
                            {
                                sfSchedule.AddSelectedResource(dropappointment, sfSchedule.selectedResourcename[0].ResourceName);
                                timelineview1.SetupAppointments();
                                timelineview2.SetupAppointments();
                                timelineview3.SetupAppointments();
                            }
                        }
                        timeScrollviewer.VerticalScrollMode = ScrollMode.Auto;
                    }
                    DatetimeDiff = DayDiff = 0;
                    sfSchedule.hvc = null;
                }
                #endregion

                #region HorizontalViewControl
                if (sfSchedule.hvc != null)
                {
                    var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
                    if (timeScrollviewer != null)
                    {
                        double tempIntervalHeight = sfSchedule.isIntervalHeightset ? IntervalHeight : 80;
                        double posinScroll = e.GetCurrentPoint(timeScrollviewer).Position.X;
                        double posinCanvas = e.GetCurrentPoint(sfSchedule.DragDropCanvas.Children[0]).Position.X;
                        if (posinScroll >= 0 && posinCanvas < drag_app.ActualWidth
                        && posinScroll - posinCanvas + drag_app.ActualWidth < timeScrollviewer.ViewportWidth)
                        {
                            if (posinScroll - tempIntervalHeight < 0 && timeScrollviewer.HorizontalOffset > 0)
                            {
                                timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset - 20);
                            }
                            else if (posinScroll - sfSchedule.Appointmentpoint.Position.X < 0)
                            {
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.Position.X);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.Position.X);
                                Canvas.SetLeft(drag_app, sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                dragResizeFlag = true;
                            }
                            else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                            {
                                double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                                previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                                Canvas.SetLeft(drag_app, previousXValue);
                                dragResizeFlag = false;
                            }
                            else
                            {
                                previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                                if ((sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                                {
                                    double diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                                    {
                                        double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                                        Canvas.SetLeft(drag_app, 5 + timeLineViewItemHeaderWidth);
                                    }
                                    else
                                    {
                                        //Canvas.SetLeft(drag_app, 5);
                                    }
                                }
                                else
                                {
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth;
                                    Canvas.SetLeft(drag_app, previousXValue);
                                }
                            }
                        }
                        else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth && posinCanvas < drag_app.ActualWidth)
                        {
                            if (posinScroll + tempIntervalHeight > timeScrollviewer.ViewportWidth && timeScrollviewer.HorizontalOffset < timeScrollviewer.ExtentWidth - timeScrollviewer.ViewportWidth)
                            {
                                timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset + 20);//(pos + drag_app.ActualWidth - timeScrollviewer.ViewportWidth));
                            }
                            else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                            {
                                double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                                previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                                if (posinCanvas == posinScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                                {
                                    diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                    (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                                }
                                Canvas.SetLeft(drag_app, previousXValue);
                                dragResizeFlag = false;
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth;
                            previousXValue = e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X;
                            Canvas.SetLeft(drag_app, previousXValue);
                        }
                    }
                    double resourcecount = 1;
                    var timelineControl = this.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>();
                    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    {
                        resourcecount = ScheduleResourceType.ResourceCollection.Count;
                    }
                    //if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                    //{
                    //    double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                    //    if ((e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) > timeLineViewItemHeaderWidth + 5 && (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) < timelineControl.ActualWidth + timeLineViewItemHeaderWidth)
                    //        Canvas.SetLeft(drag_app, (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X));
                    //}
                    //else if ((e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) > 5 && (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X) < timelineControl.ActualWidth)
                    //{
                    //    Canvas.SetLeft(drag_app, (e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.X - sfSchedule.Appointmentpoint.Position.X));
                    //}
                    var headerControl = sfSchedule.FindElementOfType<HeaderTitleBarView>();
                    double daysHeaderViewHeight = this.FindElementOfType<ScheduleHorizontalDaysHeaderViewItemsControl>().ActualHeight;
                    double Rowheight = this.FindElementOfType<ScheduleTimelineTimeSlotItemsControl>().ActualHeight / resourcecount;
                    var currentdragheight = (int)((e.GetCurrentPoint(sfSchedule.DragDropCanvas).Position.Y - (daysHeaderViewHeight) - timelineControl.Height - headerControl.ActualHeight - 5) / Rowheight);
                    if (currentdragheight < resourcecount && !(currentdragheight < 0) && resourcecount > 1)
                    {
                        Canvas.SetTop(drag_app, (currentdragheight * Rowheight) + headerControl.ActualHeight + timelineControl.Height + daysHeaderViewHeight + 5);
                    }
                    if (sfSchedule.Timelineviewrb.EllipseAnimation1 != null)
                    {
                        sfSchedule.Timelineviewrb.EllipseAnimation1.Begin();
                        sfSchedule.Timelineviewrb.EllipseAnimation2.Begin();
                    }
                }
                else
                    sfSchedule.isscrollmoveondragging = false;
            }

                #endregion
        }
#else
        internal void StartDragDrop(SfSchedule sfSchedule, MouseEventArgs e)
        {
            var drag_app = (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl);
            var obj = VisualTreeHelper.GetChild(this, 0) as Grid;
            if (obj != null)
            {
                var timeScrollviewer = obj.FindName("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
                if (timeScrollviewer != null)
                {
                    double tempIntervalHeight = sfSchedule.isIntervalHeightset ? IntervalHeight : 80;
                    double posinScroll = e.GetPosition(timeScrollviewer).X;
                    double posinCanvas = e.GetPosition(sfSchedule.DragDropCanvas.Children[0]).X;
                    if (posinScroll >= 0 && posinCanvas < drag_app.ActualWidth
                    && posinScroll - posinCanvas + drag_app.ActualWidth < timeScrollviewer.ViewportWidth)
                    {
                        if (posinScroll - tempIntervalHeight < 0 && timeScrollviewer.HorizontalOffset > 0)
                        {
                            timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset - 20);
                        }
                        else if (posinScroll - sfSchedule.Appointmentpoint.X < 0)
                        {
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.X);
                            //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + (posinScroll - sfSchedule.Appointmentpoint.Position.X);
                            Canvas.SetLeft(drag_app, sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                            dragResizeFlag = true;
                        }
                        else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                        {
                            double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                            //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                            previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                            Canvas.SetLeft(drag_app, previousXValue);
                            dragResizeFlag = false;
                        }
                        else
                        {
                            previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                            if ((sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                            {
                                double diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                                if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
                                {
                                    double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
                                    Canvas.SetLeft(drag_app, 5 + timeLineViewItemHeaderWidth);
                                }
                                else
                                {
                                    //Canvas.SetLeft(drag_app, 5);
                                }
                            }
                            else
                            {
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                                Canvas.SetLeft(drag_app, previousXValue);
                            }
                        }
                    }
                    else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth && posinCanvas < drag_app.ActualWidth)
                    {
                        if (posinScroll + tempIntervalHeight > timeScrollviewer.ViewportWidth && timeScrollviewer.HorizontalOffset < timeScrollviewer.ExtentWidth - timeScrollviewer.ViewportWidth)
                        {
                            timeScrollviewer.ScrollToHorizontalOffset(timeScrollviewer.HorizontalOffset + 20);//(pos + drag_app.ActualWidth - timeScrollviewer.ViewportWidth));
                        }
                        else if (posinScroll - posinCanvas + dragDropCanvasWidth >= timeScrollviewer.ViewportWidth && posinScroll < timeScrollviewer.ViewportWidth)
                        {
                            double diffWidth = timeScrollviewer.ViewportWidth - (posinScroll - posinCanvas + dragDropCanvasWidth);
                            (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth + diffWidth;
                            //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth + diffWidth;
                            previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                            if (posinCanvas == posinScroll && (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width < dragDropCanvasWidth)
                            {
                                diffWidth = previousXValue - (sfSchedule.DragDropCanvas.ActualWidth - timeScrollviewer.ViewportWidth - 5);
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = visibleCanvasWidth + diffWidth;
                                (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = visibleCanvasWidth + diffWidth;
                            }
                            Canvas.SetLeft(drag_app, previousXValue);
                            dragResizeFlag = false;
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        (sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).Width = dragDropCanvasWidth;
                        //(sfSchedule.DragDropCanvas.Children[0] as ScheduleHorizontalAppointmentViewControl).AppWidth = dragDropCanvasWidth;
                        previousXValue = e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X;
                        Canvas.SetLeft(drag_app, previousXValue);
                    }

                }
            }
            var timelineControl = this.FindElementOfType<ScheduleHorizontalTimeLineItemsControl>();
            //if (drag_app != null)
            //{
            //    if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            //    {
            //        double timeLineViewItemHeaderWidth = this.FindElementOfType<TimeLineViewItemHeader>().ActualWidth;
            //        if ((e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) > timeLineViewItemHeaderWidth + 5 && (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) < timelineControl.ActualWidth + timeLineViewItemHeaderWidth)
            //            Canvas.SetLeft(drag_app, (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X));
            //    }
            //    else if ((e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) > 5 && (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X) < timelineControl.ActualWidth)
            //    {
            //        Canvas.SetLeft(drag_app, (e.GetPosition(sfSchedule.DragDropCanvas).X - sfSchedule.Appointmentpoint.X));
            //    }
            //}
            double resourcecount = 1;
            if (ScheduleResourceType != null && ScheduleResourceType.ResourceCollection.Count > 0)
            {
                resourcecount = ScheduleResourceType.ResourceCollection.Count;
            }
            var tmslot = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
            var dayHeaderControl = this.FindElementOfType<ScheduleHorizontalDaysHeaderViewItemsControl>();
            var sch = this.FindParentElementOfType<SfSchedule>();
            var grid = sch.FindElementOfType<Grid>();
            var schHeader = grid.FindElementOfType<HeaderTitleBarView>();

            int resourceCount = 1;
            if (ScheduleResourceType != null)
                resourceCount = ScheduleResourceType.ResourceCollection.Count;
            var individualht = (int)(tmslot.ActualHeight / resourceCount);
            double topPosition = e.GetPosition(sfSchedule.DragDropCanvas).Y;// -sfSchedule.Appointmentpoint.Y - (individualht - FloatingAppointmentSize.Height);
            double rowcount = Math.Floor((topPosition - timelineControl.ActualHeight - dayHeaderControl.ActualHeight - schHeader.ActualHeight) / individualht);
            if (drag_app != null && rowcount <= resourcecount - 1 && !(rowcount < 0) && resourcecount > 1)
                Canvas.SetTop(drag_app, (rowcount * individualht) + timelineControl.ActualHeight + dayHeaderControl.ActualHeight + schHeader.ActualHeight + (rowcount + 2));
        }
#endif
        #endregion

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            nonworkingdaysLayout = GetTemplateChild("PART_NonWorkingDaysLayout") as ScheduleNonWorkingDayItemsControl;
            scheduleHorizontalTimeLineItemsControl = GetTemplateChild("PART_HorizontalTimeLineItemsControl") as ScheduleHorizontalTimeLineItemsControl;
            scheduleTimelineTimeSlotItemsControl = GetTemplateChild("PART_HorizontalTimeSlot") as ScheduleTimelineTimeSlotItemsControl;
            currentTimeIndicatorPresenter = GetTemplateChild("PART_TimeLineCurrentTimeIndicator") as ContentPresenter;
#if !WINRT
            if (scheduleTimelineTimeSlotItemsControl != null)
            {
                scheduleTimelineTimeSlotItemsControl.SizeChanged += scheduleTimelineTimeSlotItemsControl_SizeChanged;
            }
#endif
            timelinescroll = GetTemplateChild("PART_HorizontalTimeSlotScrollViewer") as ScrollViewer;
            if (timelinescroll != null)
            {
#if WINRT
                timelinescroll.ViewChanged += timelinescroll_ViewChanged;
#elif WPF
                timelinescroll.MouseWheel += timelinescroll_MouseWheel;
#elif SILVERLIGHT
                timelinescroll.GotFocus += scrollviewer_GotFocus;
#endif
                timelinescroll.Loaded += timelinescroll_Loaded;
            }
            previousNavigationTap = new ContentPresenter();
            nextNavigationTap = new ContentPresenter();
            IsTemplateApplied = true;
            Resourceheadercontainer = GetTemplateChild("resourceheadercontainer") as ItemsControl;
            schedule = this.FindParentElementOfType<SfSchedule>();
            AppointmentsLayoutItemsHost = GetTemplateChild("PART_HorizontalAppointmentsLayoutHost") as ItemsControl;
            nextNavigationTap = GetTemplateChild("NextApp") as ContentPresenter;
            previousNavigationTap = GetTemplateChild("PrevApp") as ContentPresenter;
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
            GenerateHeaderChildItems();
            SetupAppointments();
#if !WINRT
            timelineAppTextBox = GetTemplateChild("TimelineAppText") as TextBox;
            if (timelineAppTextBox != null)
                timelineAppTextBox.LostFocus += timelineAppTextBox_LostFocus;
#endif
        }
#if SILVERLIGHT
        private void scrollviewer_GotFocus(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource.GetType() != typeof(TextBox))
                schedule.Focus();
        }
#endif
#if !WINRT
        void timelineAppTextBox_LostFocus(object sender, RoutedEventArgs e)
        {

            if (TimelineAppTextVisibility == System.Windows.Visibility.Visible)
                schedule.CreateAppointmentOnSelection();

        }
#endif
        #endregion

        #region Events

#if !WINRT
        void scheduleTimelineTimeSlotItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var sfSchedule = (sender as ScheduleTimelineTimeSlotItemsControl).FindParentElementOfType<SfSchedule>();
            if (sfSchedule != null)
            {
                sfSchedule.DragDropCanvas.Children.Clear();
                sfSchedule.editpopup.IsOpen = false;
                sfSchedule.addnewpopup.IsOpen = false;
                sfSchedule.AddnewContextmenuPopup.IsOpen = false;
                sfSchedule.contextmenupopup.IsOpen = false;
            }
        }

#endif

        void timelinescroll_Loaded(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            var horizontal = ((FrameworkElement)VisualTreeHelper.GetChild(timelinescroll, 0)).FindName("HorizontalScrollBar") as ScrollBar;
            if (horizontal != null)
                horizontal.ValueChanged += horizontal_ValueChanged;
#elif WPF
            var verticalScrollBar = timelinescroll.Template.FindName("PART_VerticalScrollBar", timelinescroll) as ScrollBar;
            if (verticalScrollBar != null)
            {
                verticalScrollBar.MouseDoubleClick += verticalScrollBar_MouseDoubleClick;
                verticalScrollBar.MouseLeave += verticalScrollBar_MouseLeave;
            }
            var horizontalScrollBar = timelinescroll.Template.FindName("PART_HorizontalScrollBar", timelinescroll) as ScrollBar;
            if (horizontalScrollBar != null)
            {
                horizontalScrollBar.MouseDoubleClick += horizontalScrollBar_MouseDoubleClick;
                horizontalScrollBar.MouseLeave += horizontalScrollBar_MouseLeave;
            }
#endif
        }

        internal void SetWorkingHoursPosition()
        {
            double width = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>().ActualWidth;
            double offset = width * schedule.WorkStartHour;
#if SyncfusionFramework4_5_11 && WINRT
            timelinescroll.ChangeView(offset, null, null);
#else
            timelinescroll.ScrollToHorizontalOffset(offset);
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

#if WINRT
        void timelinescroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
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
            schedule.editpopup.IsOpen = false;
            schedule.addnewpopup.IsOpen = false;

        }
#else
#if WPF
        void timelinescroll_MouseWheel(object sender, MouseWheelEventArgs e)
#else
        void horizontal_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
#endif
        {
            if (!schedule.isscrollmoveondragging)
            {
                if (schedule.DragDropCanvas.Children.Count > 0)
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
            schedule.editpopup.IsOpen = false;
            schedule.addnewpopup.IsOpen = false;
            schedule.contextmenupopup.IsOpen = false;
            schedule.AddnewContextmenuPopup.IsOpen = false;
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

        void ScheduleTimeLineView_Loaded(object sender, RoutedEventArgs e)
        {
            if (schedule != null && schedule.SelectedPoint != new Point())
                UpdateSelection(false);
            if (NonAccessibleBlocks != null && NonAccessibleBlocks.Count > 0)
                SetNonAccessibleBlocks();
            var currentTimeIndicatorWidthBinding = new Binding { Path = new PropertyPath("ActualHeight"), Source = scheduleHorizontalTimeLineItemsControl };
            SetBinding(CurrentTimeIndicatorWidthProperty, currentTimeIndicatorWidthBinding);
        }

#if WINRT
        void TodayTimer_Tick(object sender, object e)
#else
        void TodayTimer_Tick(object sender, EventArgs e)
#endif
        {
            TodayTimer.Interval = new TimeSpan(0, 1, 0);
            var timelineitem = this.FindElementOfType<ScheduleHorizontalTimeSlotControl>();
            if (timelineitem != null)
            {
                double height = timelineitem.ActualWidth;
                double index = SelectedDates.IndexOf(DateTime.Now.Date) * (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * height;
                double min = (((Double)DateTime.Now.Minute) / 100) * ((Double)5 / 3);
                double posX = (DateTime.Now.Hour - ScheduleTimeLineItemsControl.MinValue + min) * (height);
                CurrentTimeIndicatorMargin = new Thickness(posX + index + currentTimeIndicatorPresenter.ActualHeight / 2, 0, 0, 0);
                currentTimeIndicatorPresenter.DataContext = DateTime.Now;
            }
        }

        #endregion

        public void Dispose()
        {
            TodayTimer.Tick -= TodayTimer_Tick;
            Loaded -= ScheduleTimeLineView_Loaded;
#if !WINRT
            if (scheduleTimelineTimeSlotItemsControl != null)
            {
                scheduleTimelineTimeSlotItemsControl.SizeChanged -= scheduleTimelineTimeSlotItemsControl_SizeChanged;
            }
#endif
            if (timelinescroll != null)
            {
#if WINRT
                timelinescroll.ViewChanged -= timelinescroll_ViewChanged;
#elif WPF
                timelinescroll.MouseWheel -= timelinescroll_MouseWheel;
#endif
                timelinescroll.Loaded -= timelinescroll_Loaded;
            }
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
#if !WINRT
            if (timelineAppTextBox != null)
                timelineAppTextBox.LostFocus -= timelineAppTextBox_LostFocus;
#endif

        }
    }
}
