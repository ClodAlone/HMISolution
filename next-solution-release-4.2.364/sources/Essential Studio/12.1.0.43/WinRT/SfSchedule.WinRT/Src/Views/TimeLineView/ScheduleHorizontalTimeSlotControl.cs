#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a time slot of timeline view.
    /// </summary>
    public class ScheduleHorizontalTimeSlotControl : Control, IDisposable
    {
        #region Constructor

        public ScheduleHorizontalTimeSlotControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalTimeSlotControl);
            SizeChanged += ScheduleHorizontalTimeSlotControl_SizeChanged;
        }

        #endregion

        #region Private Fields

        UniformStackPanel timeSlotsPanel;
        bool isSizeDetermined;

        #endregion

        #region Dependency Properties

        #region MajorTickStroke
        internal Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickStroke
        internal Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickLabelStroke
        public Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MinorTickLabelStroke
        public Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(ScheduleHorizontalTimeSlotControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MajorTickStrokeDashArray
        internal DoubleCollection MajorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MajorTickStrokeDashArrayProperty); }
            set { SetValue(MajorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MajorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleHorizontalTimeSlotControl), new PropertyMetadata(new DoubleCollection(), OnMajorTickStrokeDashArrayChanged));

        private static void OnMajorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotControl)
            {
                var scheduleHorizontalTimeSlotControl = d as ScheduleHorizontalTimeSlotControl;
                scheduleHorizontalTimeSlotControl.MajorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                scheduleHorizontalTimeSlotControl.UpdateTimeSlots();
            }
        }
        #endregion

        #region MinorTickStrokeDashArray
        internal DoubleCollection MinorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MinorTickStrokeDashArrayProperty); }
            set { SetValue(MinorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleHorizontalTimeSlotControl), new PropertyMetadata(new DoubleCollection(), OnMinorTickStrokeDashArrayChanged));

        private static void OnMinorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeSlotControl)
            {
                var scheduleHorizontalTimeSlotControl = d as ScheduleHorizontalTimeSlotControl;
                scheduleHorizontalTimeSlotControl.MinorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                scheduleHorizontalTimeSlotControl.UpdateTimeSlots();
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

        private void UpdateTimeSlots()
        {
            if (isSizeDetermined)
                GenerateTimeSlots();
        }

        private void GenerateTimeSlots()
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule == null)
            {
                return;
            }
            var intervalsCount = GetTimeSlotIntervals(schedule.TimeInterval);
            timeSlotsPanel.Children.Clear();
            var lineY2Binding = new Binding { Source = this, Path = new PropertyPath("ActualHeight") };
            for (int i = 0; i < intervalsCount; i++)
            {
                var line = new Line
                {
                    StrokeThickness = 1,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Stretch = Stretch.Fill
                };
                var scheduleLineStrokeBinding = new Binding { Source = this };
                if (i % intervalsCount == 0)
                {
                    line.StrokeDashArray = MajorLineStrokeDashArray;
                    scheduleLineStrokeBinding.Path = new PropertyPath("MajorTickStroke");
                }
                else
                {
                    line.StrokeDashArray = MinorLineStrokeDashArray;
                    scheduleLineStrokeBinding.Path = new PropertyPath("MinorTickStroke");
                }
                line.SetBinding(Shape.StrokeProperty, scheduleLineStrokeBinding);
                line.SetBinding(Line.Y2Property, lineY2Binding);
                timeSlotsPanel.Children.Add(line);
            }
        }

        int GetTimeSlotIntervals(TimeInterval timeInterval)
        {
            int intervalCount = ScheduleTimeLineItemsControl.IntervalCount[(int)timeInterval];
            return intervalCount;
        }

        public void Dispose()
        {
            SizeChanged -= ScheduleHorizontalTimeSlotControl_SizeChanged;
        }

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            timeSlotsPanel = GetTemplateChild("PART_TimeSlotPanel") as UniformStackPanel;
        }

        #endregion

        #region Events

        void ScheduleHorizontalTimeSlotControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            isSizeDetermined = true;
            GenerateTimeSlots();
        }

        #endregion
    }
}
