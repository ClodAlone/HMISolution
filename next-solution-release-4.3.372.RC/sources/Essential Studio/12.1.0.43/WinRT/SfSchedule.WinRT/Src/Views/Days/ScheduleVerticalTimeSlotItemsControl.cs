#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.ObjectModel;
using System.Linq;
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Data;
using Windows.UI;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Data;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging items in day view's vertical time slot.
    /// </summary>
    public class ScheduleVerticalTimeSlotItemsControl : ItemsControl
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleVerticalTimeSlotItemsControl">ScheduleVerticalTimeSlotItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleVerticalTimeSlotItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleVerticalTimeSlotItemsControl);
        }

        #endregion

        #region Public Fields

        public const double DefaultIntervalHeight = 24d;

        #endregion

        #region Private Fields

        bool IsScrollLoaded;

        #endregion

        #region Dependency Properties

        #region TimeInterval
        /// <summary>
        /// Gets the time interval of vertical time slot.
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
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleVerticalTimeSlotItemsControl), new PropertyMetadata(TimeInterval.ThirtyMin, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var timeLineItemsControl = dpo as ScheduleVerticalTimeSlotItemsControl;
            if (timeLineItemsControl != null && timeLineItemsControl.SelectedDates != null)
                timeLineItemsControl.GenerateItems(timeLineItemsControl.SelectedDates.Count);

        }
        #endregion

        #region ResourceCount
        /// <summary>
        /// Gets the count of resources in vertical time slot.
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
            DependencyProperty.Register("ResourceCount", typeof(int), typeof(ScheduleVerticalTimeSlotItemsControl), new PropertyMetadata(1, OnResourceCountChanged));

        static void OnResourceCountChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs arg)
        {
            var vertim = dpo as ScheduleVerticalTimeSlotItemsControl;
            if (vertim != null)
                vertim.GenerateItems(vertim.SelectedDates.Count);
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
            DependencyProperty.Register("DayViewColumnCount", typeof(int), typeof(ScheduleVerticalTimeSlotItemsControl), new PropertyMetadata(1, OnDayViewColumnCountChanged));

        private static void OnDayViewColumnCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleVerticalTimeSlotItemsControl)
            {
                var scheduleVerticalTimeSlotItemsControl = d as ScheduleVerticalTimeSlotItemsControl;
                var schedule = scheduleVerticalTimeSlotItemsControl.FindParentElementOfType<SfSchedule>();
                if (schedule != null && schedule.ScheduleType == ScheduleType.Day && scheduleVerticalTimeSlotItemsControl.SelectedDates != null)
                {
                    scheduleVerticalTimeSlotItemsControl.Items.Clear();
                    scheduleVerticalTimeSlotItemsControl.GenerateItems(scheduleVerticalTimeSlotItemsControl.SelectedDates.Count);
                }
            }
        }
        #endregion

        #region IntervalHeight
        /// <summary>
        /// Gets the interval height in time slot.
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
            DependencyProperty.Register("IntervalHeight", typeof(double), typeof(ScheduleVerticalTimeSlotItemsControl), new PropertyMetadata(DefaultIntervalHeight, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleVerticalTimeSlotItemsControl)d;
            instance.OnIntervalHeightChanged();
        }

        private void OnIntervalHeightChanged()
        {
            if (SelectedDates != null)
            {
                GenerateItems(SelectedDates.Count);
            }
        }
        #endregion

        #region DayViewVerticaLineStroke
        public Brush DayViewVerticaLineStroke
        {
            get { return (Brush)GetValue(DayViewVerticaLineStrokeProperty); }
            set { SetValue(DayViewVerticaLineStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayViewVerticaLineStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayViewVerticaLineStrokeProperty =
            DependencyProperty.Register("DayViewVerticaLineStroke", typeof(Brush), typeof(ScheduleVerticalTimeSlotItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region SelectedDates
        /// <summary>
        /// Gets the collection of selected dates in vertical time slot.
        /// </summary>
        public ObservableCollection<DateTime> SelectedDates
        {
            get { return (ObservableCollection<DateTime>)GetValue(SelectedDatesProperty); }
            internal set { SetValue(SelectedDatesProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SelectedDates.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SelectedDatesProperty =
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleVerticalTimeSlotItemsControl), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs args)
        {
            var scheduleVerticalTimeSlotItemsControl = dpo as ScheduleVerticalTimeSlotItemsControl;
            if (scheduleVerticalTimeSlotItemsControl != null && (args.OldValue is ObservableCollection<DateTime> && args.NewValue is ObservableCollection<DateTime>) &&
                ((args.OldValue as ObservableCollection<DateTime>).Count != (args.NewValue as ObservableCollection<DateTime>).Count))
                scheduleVerticalTimeSlotItemsControl.GenerateItems(scheduleVerticalTimeSlotItemsControl.SelectedDates.Count());
        }
        #endregion

        #endregion

        #region Methods

        #region CalculateLeafChildCount

        int CalculateLeafChildCount(ResourceType restype)
        {
            int leafchild = 1;
            ResourceType tempresotype = restype.SubResourceType;
            while (tempresotype != null)
            {
                leafchild = leafchild * tempresotype.ResourceCollection.Count;
                tempresotype = tempresotype.SubResourceType;
            }
            return leafchild;
        }

        #endregion

        #region Generate Items

        private void GenerateItems(int count)
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (IsScrollLoaded && schedule != null)
            {
                int leafcntofsingleitem = 1;
                if (schedule.ScheduleResourceType != null)
                    leafcntofsingleitem = CalculateLeafChildCount(schedule.ScheduleResourceType);
                if (Items != null && Items.Count != (count * ResourceCount * leafcntofsingleitem) + 1)
                {
                    var schedulelinesbinding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("DayViewVerticaLineStroke")
                    };
                    Items.Clear();
                    UpdateHeight();
                    for (var i = 0; i < (count * ResourceCount * leafcntofsingleitem) + 1; i++)
                    {
                        var timeSlot = new Line
                            {
                                Stretch = Stretch.Fill,
                                StrokeThickness = 1,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                Y1 = 1
                            };
                        timeSlot.SetBinding(Shape.StrokeProperty, schedulelinesbinding);
                        Items.Add(timeSlot);
                    }
                }
                else if (Items != null)
                {
                    UpdateHeight();
                }

            }
        }

        #endregion

        #region UpdateHeight

        private void UpdateHeight()
        {
            if (IntervalHeight < 0)
            {
                Height = 0;
            }
            else
            {
                Height = ScheduleTimeLineItemsControl.MaxValue * IntervalHeight * ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
            }
        }

        #endregion

        #endregion

        #region overrides
#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            var scroll = this.FindParentElementOfType<ScrollViewer>();
            scroll.Loaded += scroll_Loaded;
        }

        void scroll_Loaded(object sender, RoutedEventArgs e)
        {
            IsScrollLoaded = true;
            GenerateItems(SelectedDates.Count);
        }
        #endregion
    }
}
