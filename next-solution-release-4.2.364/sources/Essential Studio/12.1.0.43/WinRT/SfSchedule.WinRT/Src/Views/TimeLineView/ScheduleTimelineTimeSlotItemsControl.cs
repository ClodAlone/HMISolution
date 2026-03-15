#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
using Windows.UI;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging items in time slot of timeline view.
    /// </summary>
    public class ScheduleTimelineTimeSlotItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleTimelineTimeSlotItemsControl">ScheduleTimelineTimeSlotItemsControl</see> class. 
        /// </summary>
        /// <remarks></remarks>
        public ScheduleTimelineTimeSlotItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleTimelineTimeSlotItemsControl);
            SizeChanged += ScheduleTimelineTimeSlotItemsControl_SizeChanged;
        }

        #endregion

        #region Public Fields

        public const double DefaultIntervalWidth = 24d;

        #endregion

        #region Private Fields

        bool isSizeDetermined;
        ScheduleTimeLineView scheduleTimeLineView;

        #endregion

        #region Dependency Properties

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
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var obj = dpo as ScheduleTimelineTimeSlotItemsControl;
            if (obj != null && obj.isSizeDetermined)
            {
                var observableCollection = args.OldValue as ObservableCollection<DateTime>;
                if (observableCollection != null && observableCollection.Count != obj.SelectedDates.Count)// || obj.Items.Count != obj.SelectedDates.Count)
                {
                    obj.PopulateTimeSlots();
                }
            }
        }
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets or sets the time interval of timeline view.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeInterval"></seealso>
        public TimeInterval TimeInterval
        {
            get { return (TimeInterval)GetValue(TimeIntervalProperty); }
            set { SetValue(TimeIntervalProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeInterval.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty =
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeLinePropertyChanged));
        #endregion

        #region IntervalWidth
        /// <summary>
        /// Gets the interval width of timeline view's time slot.
        /// </summary>
        public double IntervalWidth
        {
            get { return (double)GetValue(IntervalWidthProperty); }
            internal set { SetValue(IntervalWidthProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IntervalWidth.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IntervalWidthProperty =
            DependencyProperty.Register("IntervalWidth", typeof(double), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(DefaultIntervalWidth, OnIntervalWidthChanged));

        private static void OnIntervalWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleTimelineTimeSlotItemsControl)d;
            instance.UpdateWidth();
        }
        #endregion

        #region MajorTickStroke
        /// <summary>
        /// Gets the stroke color for major ticks that represent hour in time slot.
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
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickStroke
        /// <summary>
        /// Gets the stroke color for minor ticks that represent minute in time slot.
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
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickLabelStroke
        public Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MinorTickLabelStroke
        public Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
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
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(new DoubleCollection(), OnTimeLinePropertyChanged));
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
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(new DoubleCollection(), OnTimeLinePropertyChanged));

        private static void OnTimeLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimelineTimeSlotItemsControl)
            {
                var timeLineItemsControl = d as ScheduleTimelineTimeSlotItemsControl;
                if (timeLineItemsControl.isSizeDetermined)
                    timeLineItemsControl.PopulateTimeSlots();
            }
        }        
        #endregion

        #region ShowNonWorkingHours
        public bool ShowNonWorkingHours
        {
            get { return (bool)GetValue(ShowNonWorkingHoursProperty); }
            internal set { SetValue(ShowNonWorkingHoursProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowNonWorkingHours.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowNonWorkingHoursProperty =
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimelineTimeSlotItemsControl)
            {
                ScheduleTimelineTimeSlotItemsControl timeSlotItemsControl = (d as ScheduleTimelineTimeSlotItemsControl);
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
            DependencyProperty.Register("Minimum", typeof(int), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(0, OnMinimumChanged));

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimelineTimeSlotItemsControl)
            {
                ScheduleTimelineTimeSlotItemsControl timeSlotItemsControl = (d as ScheduleTimelineTimeSlotItemsControl);
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
            DependencyProperty.Register("Maximum", typeof(int), typeof(ScheduleTimelineTimeSlotItemsControl), new PropertyMetadata(24, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimelineTimeSlotItemsControl)
            {
                ScheduleTimelineTimeSlotItemsControl timeSlotItemsControl = (d as ScheduleTimelineTimeSlotItemsControl);
                if (!timeSlotItemsControl.ShowNonWorkingHours)
                {
                    ScheduleTimeLineItemsControl.MaxValue = (int)e.NewValue;
                    timeSlotItemsControl.UpdateTimeSlotHours();
                }
            }
        }
        #endregion

        #endregion

        #region Methods

        private void UpdateWidth()
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            scheduleTimeLineView = this.FindParentElementOfType<ScheduleTimeLineView>();
            if (schedule != null && SelectedDates != null)
            {
                if (SelectedDates != null && scheduleTimeLineView != null)
                {
                    Width = scheduleTimeLineView.GetTimeSlotWidth(SelectedDates.Count);
                }
            }
        }

        private void UpdateTimeSlotHours()
        {
            if (isSizeDetermined)
            {
                Width = scheduleTimeLineView.GetTimeSlotWidth(SelectedDates.Count);
                PopulateTimeSlots();
                if (scheduleTimeLineView != null)
                {
                    scheduleTimeLineView.UpdateSelection(false);
                    scheduleTimeLineView.SetNonAccessibleBlocks();
                }
            }
        }

        private void PopulateTimeSlots()
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule == null)
            {
                return;
            }

            if (Items != null)
            {
                Items.Clear();
                scheduleTimeLineView = this.FindParentElementOfType<ScheduleTimeLineView>();
                if (scheduleTimeLineView != null && (schedule.isIntervalHeightset || !schedule.EnableAutoFormat))
                    Width = scheduleTimeLineView.GetTimeSlotWidth(SelectedDates.Count);
                for (int index = 0; index < SelectedDates.Count; index++)
                {
                    for (var i = ScheduleTimeLineItemsControl.MinValue; i < ScheduleTimeLineItemsControl.MaxValue; i++)
                    {
                        var timeSlot = (ScheduleHorizontalTimeSlotControl)GetContainerForItemOverride();
                        Items.Add(timeSlot);
                    }
                }
            }
        }

        #endregion

        #region Override Methods

        protected override DependencyObject GetContainerForItemOverride()
        {
            var majorTickStrokeDashArrayBinding = new Binding { Source = this, Path = new PropertyPath("MajorTickStrokeDashArray") };
            var minorTickStrokeDashArrayBinding = new Binding { Source = this, Path = new PropertyPath("MinorTickStrokeDashArray") };
            var majorTickStrokeBinding = new Binding { Source = this, Path = new PropertyPath("MajorTickStroke") };
            var minorTickStrokeBinding = new Binding { Source = this, Path = new PropertyPath("MinorTickStroke") };
            var majorTickLabelStrokeBinding = new Binding { Source = this, Path = new PropertyPath("MajorTickLabelStroke") };
            var minorTickLabelStrokeBinding = new Binding { Source = this, Path = new PropertyPath("MinorTickLabelStroke") };

            var timeSlotControl = new ScheduleHorizontalTimeSlotControl();
            timeSlotControl.SetBinding(ScheduleHorizontalTimeSlotControl.MajorTickStrokeDashArrayProperty, majorTickStrokeDashArrayBinding);
            timeSlotControl.SetBinding(ScheduleHorizontalTimeSlotControl.MinorTickStrokeDashArrayProperty, minorTickStrokeDashArrayBinding);
            timeSlotControl.SetBinding(ScheduleHorizontalTimeSlotControl.MajorTickStrokeProperty, majorTickStrokeBinding);
            timeSlotControl.SetBinding(ScheduleHorizontalTimeSlotControl.MinorTickStrokeProperty, minorTickStrokeBinding);
            timeSlotControl.SetBinding(ScheduleHorizontalTimeSlotControl.MajorTickLabelStrokeProperty, majorTickLabelStrokeBinding);
            timeSlotControl.SetBinding(ScheduleHorizontalTimeSlotControl.MinorTickLabelStrokeProperty, minorTickLabelStrokeBinding);

#if WINRT
            if (ItemContainerStyle != null)
            {
                timeSlotControl.Style = ItemContainerStyle;
            }
#endif
            return timeSlotControl;
        }

        #endregion

        #region Events

        void ScheduleTimelineTimeSlotItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            isSizeDetermined = true;
            PopulateTimeSlots();
            if (scheduleTimeLineView != null)
            {
                scheduleTimeLineView.UpdateSelection(false);
#if WINRT
                scheduleTimeLineView.GenerateNonworkingdaysItems();
#endif
            }
        }

        #endregion
    }
}
