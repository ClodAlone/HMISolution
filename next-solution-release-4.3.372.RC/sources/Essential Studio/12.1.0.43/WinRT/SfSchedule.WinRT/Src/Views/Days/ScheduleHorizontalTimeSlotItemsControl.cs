#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging items in day view's horizontal time slot.
    /// </summary>
    public class ScheduleHorizontalTimeSlotItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleHorizontalTimeSlotItemsControl">ScheduleHorizontalTimeSlotItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleHorizontalTimeSlotItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalTimeSlotItemsControl);
            SizeChanged += ScheduleHorizontalTimeSlotItemsControl_SizeChanged;
        }

        #endregion

        #region Public Fields

        bool isSizeDetermined;
        ScheduleDaysView scheduleDaysView;
        SfSchedule schedule;

        #endregion

        #region Dependency Properties

        #region ResourceCount
        /// <summary>
        /// Gets the count of resources in horizontal time slot.
        /// </summary>
        public int ResourceCount
        {
            get { return (int)GetValue(ResourceCountProperty); }
            internal set { SetValue(ResourceCountProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ResourceCount.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ResourceCountProperty =
            DependencyProperty.Register("ResourceCount", typeof(int), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(1, OnResourceCountChanged));

        private static void OnResourceCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                (d as ScheduleHorizontalTimeSlotItemsControl).UpdateTimeSlot();
            }
        }
        #endregion

        #region DayViewColumnCount
        public int DayViewColumnCount
        {
            get { return (int)GetValue(DayViewColumnCountProperty); }
            internal set { SetValue(DayViewColumnCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DayViewColumnCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DayViewColumnCountProperty =
            DependencyProperty.Register("DayViewColumnCount", typeof(int), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(1, OnDayViewColumnCountChanged));

        private static void OnDayViewColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                var scheduleHorizontalTimeSlotItemsControl = d as ScheduleHorizontalTimeSlotItemsControl;
                var schedule = scheduleHorizontalTimeSlotItemsControl.schedule;
                if (schedule != null && schedule.ScheduleType == ScheduleType.Day)
                {
                    scheduleHorizontalTimeSlotItemsControl.UpdateTimeSlot();
                }
            }
        }
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets the time interval of horizontal time slot.
        /// </summary>
        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)GetValue(TimeIntervalProperty); }
            internal set { SetValue(TimeIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty =
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                double line_count = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * ScheduleTimeLineItemsControl.IntervalCount[(int)e.NewValue];
                (d as ScheduleHorizontalTimeSlotItemsControl).GenerateItems(line_count);
            }
        }
        #endregion

        #region IntervalHeight
        /// <summary>
        /// Gets the interval height of horizontal time slot.
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
            DependencyProperty.Register("IntervalHeight", typeof(double), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(ScheduleTimeLineItemsControl.DefaultIntervalHeight, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                var timeSlotItemsControl = (d as ScheduleHorizontalTimeSlotItemsControl);
                double line_count = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * ScheduleTimeLineItemsControl.IntervalCount[(int)timeSlotItemsControl.TimeInterval];
                timeSlotItemsControl.GenerateItems(line_count);
            }
        }
        #endregion

        #region MajorTickStrokeDashArray
        /// <summary>
        /// Gets the collection of double values that indicates the pattern of dashes and gaps for major tick lines.
        /// </summary>
        public DoubleCollection MajorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MajorTickStrokeDashArrayProperty); }
            internal set { SetValue(MajorTickStrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(new DoubleCollection(), OnMajorTickStrokeDashArrayChanged));

        private static void OnMajorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                (d as ScheduleHorizontalTimeSlotItemsControl).MajorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                OnTimeSlotPropertyChanged(d, e);
            }
        }
        #endregion

        #region MinorTickStrokeDashArray
        /// <summary>
        /// Gets the collection of double values that indicates the pattern of dashes and gaps for minor tick lines.
        /// </summary>
        public DoubleCollection MinorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MinorTickStrokeDashArrayProperty); }
            internal set { SetValue(MinorTickStrokeDashArrayProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(new DoubleCollection(), OnMinorTickStrokeDashArrayChanged));

        private static void OnMinorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                (d as ScheduleHorizontalTimeSlotItemsControl).MinorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                OnTimeSlotPropertyChanged(d, e);
            }
        }
        #endregion

        #region MajorTickStroke
        /// <summary>
        /// Gets the stroke of major ticks which represents hour in time slot.
        /// </summary>
        public Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            internal set { SetValue(MajorTickStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush()));
        private static void OnTimeSlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
                (d as ScheduleHorizontalTimeSlotItemsControl).UpdateTimeSlot();
        }
        #endregion

        #region MinorTickStroke
        /// <summary>
        /// Gets the stroke of minor ticks which represents minute in time slot.
        /// </summary>
        public Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            internal set { SetValue(MinorTickStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush()));
        #endregion

        #region MajorTickLabelStroke
        public Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MinorTickLabelStroke
        public Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region ShowNonWorkingHours
        public bool ShowNonWorkingHours
        {
            get { return (bool)GetValue(ShowNonWorkingHoursProperty); }
            internal set { SetValue(ShowNonWorkingHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNonWorkingHours.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNonWorkingHoursProperty =
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                ScheduleHorizontalTimeSlotItemsControl timeSlotItemsControl = (d as ScheduleHorizontalTimeSlotItemsControl);
                ScheduleTimeLineItemsControl.MinValue = timeSlotItemsControl.ShowNonWorkingHours ? 0 : timeSlotItemsControl.Minimum;
                ScheduleTimeLineItemsControl.MaxValue = timeSlotItemsControl.ShowNonWorkingHours ? 24 : timeSlotItemsControl.Maximum;
                timeSlotItemsControl.UpdateTimeSlotHours();
            }
        }
        #endregion

        #region Minimum
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            internal set { SetValue(MinimumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Minimum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(int), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(0, OnMinimumChanged));

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                ScheduleHorizontalTimeSlotItemsControl timeSlotItemsControl = (d as ScheduleHorizontalTimeSlotItemsControl);
                if (!timeSlotItemsControl.ShowNonWorkingHours)
                {
                    ScheduleTimeLineItemsControl.MinValue = (int)e.NewValue;
                    timeSlotItemsControl.UpdateTimeSlotHours();
                }
            }
        }
        #endregion

        #region Maximum
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            internal set { SetValue(MaximumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Maximum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(int), typeof(ScheduleHorizontalTimeSlotItemsControl), new PropertyMetadata(24, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotItemsControl)
            {
                ScheduleHorizontalTimeSlotItemsControl timeSlotItemsControl = (d as ScheduleHorizontalTimeSlotItemsControl);
                if (!timeSlotItemsControl.ShowNonWorkingHours)
                {
                    ScheduleTimeLineItemsControl.MaxValue = (int)e.NewValue;
                    timeSlotItemsControl.UpdateTimeSlotHours();
                }
            }
        }
        #endregion

        #endregion

        #region CLR Properties

        #region MajorLineStrokeDashArray
        DoubleCollection majorLineStrokeDashArray = new DoubleCollection();

        DoubleCollection MajorLineStrokeDashArray
        {
            get
            {
                var strokeDashArray = new DoubleCollection();

                foreach (double val in majorLineStrokeDashArray)
                    strokeDashArray.Add(val);
                return strokeDashArray;
            }
            set { majorLineStrokeDashArray = value; }
        }
        #endregion

        #region MinorLineStrokeDashArray
        DoubleCollection minorLineStrokeDashArray = new DoubleCollection();

        DoubleCollection MinorLineStrokeDashArray
        {
            get
            {
                var strokeDashArray = new DoubleCollection();

                foreach (double val in minorLineStrokeDashArray)
                    strokeDashArray.Add(val);
                return strokeDashArray;
            }
            set { minorLineStrokeDashArray = value; }
        }
        #endregion

        #endregion

        #region Methods

        #region Update Height

        private void UpdateHeight()
        {
            if (IntervalHeight < 0)
            {
                Height = 0;
            }
            else
            {
                Height = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * IntervalHeight * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            }
        }

        #endregion

        #region Update TimeSlot

        internal void UpdateTimeSlot()
        {
            if (isSizeDetermined)
            {
                double line_count = (ScheduleTimeLineItemsControl.MaxValue - ScheduleTimeLineItemsControl.MinValue) * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                GenerateItems(line_count);
            }
        }

        private void UpdateTimeSlotHours()
        {
            if (isSizeDetermined)
            {
                UpdateTimeSlot();
                if (scheduleDaysView != null)
                {
                    if (scheduleDaysView.row >= 0 && schedule.currentAllDaySelectedItem == null)
                    {
                        scheduleDaysView.UpdateSelection(false);
                    }
                    scheduleDaysView.SetNonAccessibleBlocks();
                }
            }
        }
 
        #endregion

        #region Generate Items

        private void GenerateItems(double lineCount)
        {
            if (Items != null)
            {
                Items.Clear();
                UpdateHeight();

                int timeIntervalCount = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];

                for (int i = 0; i < lineCount + 1; i++)
                {
                    var timeSlot = new Line
                    {
                        StrokeThickness = 1,
                    };
                    var scheduleLineStrokeBinding = new Binding { Source = this };
                    if (i % timeIntervalCount == 0)
                    {
                        timeSlot.StrokeDashArray = MajorLineStrokeDashArray;
                        scheduleLineStrokeBinding.Path = new PropertyPath("MajorTickStroke");
                    }
                    else
                    {
                        timeSlot.StrokeDashArray = MinorLineStrokeDashArray;
                        scheduleLineStrokeBinding.Path = new PropertyPath("MinorTickStroke");
                    }
                    timeSlot.SetBinding(Shape.StrokeProperty, scheduleLineStrokeBinding);
                    var lineX2Binding = new Binding { Source = this, Path = new PropertyPath("ActualWidth") };
                    timeSlot.SetBinding(Line.X2Property, lineX2Binding);
                    Items.Add(timeSlot);
                }
            }
        }

        #endregion

        #endregion

        #region Events

        #region Size Changed

        void ScheduleHorizontalTimeSlotItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            isSizeDetermined = true;
            scheduleDaysView = this.FindParentElementOfType<ScheduleDaysView>();
            schedule = this.FindParentElementOfType<SfSchedule>();
            if (scheduleDaysView.row >= 0)
            {
                scheduleDaysView.UpdateSelection(false);
            }
            UpdateTimeSlot();
        }

        #endregion

        #endregion
    }
}
