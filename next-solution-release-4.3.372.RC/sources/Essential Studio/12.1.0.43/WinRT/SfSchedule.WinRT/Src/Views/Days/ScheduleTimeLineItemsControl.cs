#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
#if WINRT
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging time lines.
    /// </summary>
    public class ScheduleTimeLineItemsControl : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleTimeLineItemsControl">ScheduleTimeLineItemsControl</see>
        /// class.
        /// </summary>
        public ScheduleTimeLineItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleTimeLineItemsControl);
            SizeChanged += ScheduleTimeLineItemsControl_SizeChanged;
        }

        static ScheduleTimeLineItemsControl()
        {
            IntervalCount = new int[7];
            IntervalCount[(int)TimeInterval.FiveMin] = 12;
            IntervalCount[(int)TimeInterval.SixMin] = 10;
            IntervalCount[(int)TimeInterval.TenMin] = 6;
            IntervalCount[(int)TimeInterval.FifteenMin] = 4;
            IntervalCount[(int)TimeInterval.TwentyMin] = 3;
            IntervalCount[(int)TimeInterval.ThirtyMin] = 2;
            IntervalCount[(int)TimeInterval.OneHour] = 1;
        }

        #endregion

        #region Constants

        public static int MinValue = 0;
        public static int MaxValue = 24;
        public const double DefaultIntervalHeight = 40d;

        #endregion

        #region Private Fields

        private bool isSizeDetermined;
        internal UniformStackPanel timelineitemspanel;
        internal UniformStackPanel timelinelabelspanel;

        #endregion

        #region Public Fields

        public readonly static int[] IntervalCount;

        #endregion

        #region Dependency Properties

        #region TimeMode
        /// <summary>
        /// Gets the time mode for time line which may be 12 hrs or 24 hrs.
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
            DependencyProperty.Register("TimeMode", typeof(TimeModes), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(TimeModes.TwelveHours, OnTimeModeChanged));

        private static void OnTimeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineItemsControl)
            {
                var schedule = (d as ScheduleTimeLineItemsControl).FindParentElementOfType<SfSchedule>();
                if (schedule != null)
                {
                    TimeModes mode = (d as ScheduleTimeLineItemsControl).TimeMode;
                    schedule.MajorTickTimeFormat = mode == TimeModes.TwentyFourHours ? "HH : mm" : "hh:mm tt";
                }
            }
        }
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets the time interval for time line.
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
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeLinePropertyChanged));

        private static void OnTimeLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineItemsControl)
            {
                var scheduleTimeLineItemsControl = (d as ScheduleTimeLineItemsControl);
                if (scheduleTimeLineItemsControl.isSizeDetermined)
                    scheduleTimeLineItemsControl.SetupHours();
            }
        }
        #endregion

        #region IntervalHeight
        public double IntervalHeight
        {
            get { return (double)GetValue(IntervalHeightProperty); }
            internal set { SetValue(IntervalHeightProperty, value); }
        }

        public static readonly DependencyProperty IntervalHeightProperty = DependencyProperty.Register("IntervalHeight", typeof(double), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(DefaultIntervalHeight, OnIntervalHeightChanged));

        private static void OnIntervalHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleTimeLineItemsControl)d;
            instance.UpdateHeight();
        }
        #endregion

        #region MajorTickStroke
        /// <summary>
        /// Gets the stroke for major ticks that represents hour in time line.
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
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MinorTickStroke
        /// <summary>
        /// Gets the stroke for minor ticks that represents minute in time line.
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
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickLabelStroke
        public Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MinorTickLabelStroke
        public Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MajorTickVisibility
        /// <summary>
        /// Gets the visibility of major ticks that represent hour in time line.
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
            DependencyProperty.Register("MajorTickVisibility", typeof(Visibility), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region MinorTickVisibility
        /// <summary>
        /// Gets the visibility of minor ticks that represent minute in time line.
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
            DependencyProperty.Register("MinorTickVisibility", typeof(Visibility), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(Visibility.Visible));
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
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(new DoubleCollection(), OnMajorTickStrokeDashArrayChanged));

        private static void OnMajorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineItemsControl)
            {
                (d as ScheduleTimeLineItemsControl).MajorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                OnTimeLinePropertyChanged(d, e);
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

        // Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(new DoubleCollection(), OnMinorTickStrokeDashArrayChanged));

        private static void OnMinorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineItemsControl)
            {
                (d as ScheduleTimeLineItemsControl).MinorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                OnTimeLinePropertyChanged(d, e);
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
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineItemsControl)
            {
                ScheduleTimeLineItemsControl timeLineItemsControl = (d as ScheduleTimeLineItemsControl);
                MinValue = timeLineItemsControl.ShowNonWorkingHours ? 0 : timeLineItemsControl.Minimum;
                MaxValue = timeLineItemsControl.ShowNonWorkingHours ? 24 : timeLineItemsControl.Maximum;
                timeLineItemsControl.UpdateHeight();
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
            DependencyProperty.Register("Minimum", typeof(int), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(0, OnMinimumChanged));

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineItemsControl)
            {
                ScheduleTimeLineItemsControl timeLineItemsControl = d as ScheduleTimeLineItemsControl;
                if (!timeLineItemsControl.ShowNonWorkingHours)
                {
                    MinValue = (int)e.NewValue;
                    timeLineItemsControl.UpdateHeight();
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
            DependencyProperty.Register("Maximum", typeof(int), typeof(ScheduleTimeLineItemsControl), new PropertyMetadata(24, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleTimeLineItemsControl)
            {
                ScheduleTimeLineItemsControl timeLineItemsControl = d as ScheduleTimeLineItemsControl;
                if (!timeLineItemsControl.ShowNonWorkingHours)
                {
                    MaxValue = (int)e.NewValue;
                    timeLineItemsControl.UpdateHeight();
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

        #region UpdateHeight

        private void UpdateHeight()
        {
            if (IntervalHeight < 0)
            {
                Height = 0;
            }
            else
            {
                Height = (MaxValue - MinValue) * IntervalHeight * IntervalCount[(int)TimeInterval];
            }
        }

        #endregion

        #region Set Hours

        private void SetupHours()
        {
            if (timelineitemspanel != null && timelinelabelspanel != null)
            {
                timelineitemspanel.Children.Clear();
                timelinelabelspanel.Children.Clear();
                var schedule = this.FindParentElementOfType<SfSchedule>();
                var hourformatbinding = new Binding();
                var minuteformatbinding = new Binding();
                if (schedule != null)
                {
                    hourformatbinding = new Binding { Path = new PropertyPath("MajorTickTimeFormat"), Source = schedule };
                    minuteformatbinding = new Binding
                    {
                        Path = new PropertyPath("MinorTickTimeFormat"),
                        Source = schedule
                    };
                }
                var bindMajorTickStroke = new Binding { Path = new PropertyPath("MajorTickStroke"), Source = this };
                var bindHourvisibility = new Binding { Path = new PropertyPath("MajorTickVisibility"), Source = this };
                var bindMinutevisibility = new Binding { Path = new PropertyPath("MinorTickVisibility"), Source = this };
                var bindMinorTickStroke = new Binding { Path = new PropertyPath("MinorTickStroke"), Source = this };
                var bindMinorTickLabelStroke = new Binding { Path = new PropertyPath("MinorTickLabelStroke"), Source = this };
                var bindMajorTickLabelStroke = new Binding { Path = new PropertyPath("MajorTickLabelStroke"), Source = this };
                //var hourLabelStrokeBinding = new Binding
                //{
                //    Path = new PropertyPath(BindTickStrokeWithLabel ? "MajorTickStroke" : "MajorTickLabelStroke"),
                //    Source = this
                //};
                //var minuteLabelStrokeBinding = new Binding
                //{
                //    Path = new PropertyPath(BindTickStrokeWithLabel ? "MinorTickStroke" : "MinorTickLabelStroke"),
                //    Source = this
                //};
                for (int i = MinValue; i < MaxValue; i++)
                {
                    var timeLineHour = new Line
                    {
                        X2 = 50,
                        MinWidth = 50,
                        Stretch = Stretch.Fill,
                        StrokeThickness = 1,
                        VerticalAlignment = VerticalAlignment.Top,
                        StrokeDashArray = MajorLineStrokeDashArray
                    };
                    timeLineHour.SetBinding(Shape.StrokeProperty, bindMajorTickStroke);
                    timeLineHour.SetBinding(VisibilityProperty, bindHourvisibility);
                    var textblock = new CustomTextBlock
                    {
                        FontFamily = new FontFamily("Segoe UI"),
                        TimeSpanValue = new TimeSpan(i, 0, 0),
                        FontSize = 14
                    };
                    textblock.SetBinding(CustomTextBlock.HourFormatProperty, hourformatbinding);
                    textblock.SetBinding(VisibilityProperty, bindHourvisibility);
                    textblock.SetBinding(ForegroundProperty, bindMajorTickLabelStroke);
                    timelinelabelspanel.Children.Add(textblock);
                    timelineitemspanel.Children.Add(timeLineHour);
                    int intervalCount = IntervalCount[(int)TimeInterval];
                    for (int j = 1; j < intervalCount; j++)
                    {
                        var timeLineMinute = new Line
                        {
                            X2 = 35,
                            Width = 35,
                            Stretch = Stretch.Fill,
                            StrokeThickness = 1,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Top,
                            StrokeDashArray = MinorLineStrokeDashArray
                        };
                        timeLineMinute.SetBinding(Shape.StrokeProperty, bindMinorTickStroke);
                        timeLineMinute.SetBinding(VisibilityProperty, bindMinutevisibility);
                        var minutetextblock = new CustomTextBlock
                        {
                            FontFamily = new FontFamily("Segoe UI"),
                            TimeSpanValue = new TimeSpan(i, (j * (60 / intervalCount)), 0),
                            FontSize = 14,
                            HorizontalAlignment = HorizontalAlignment.Right
                        };
                        minutetextblock.SetBinding(VisibilityProperty, bindMinutevisibility);
                        minutetextblock.SetBinding(CustomTextBlock.MinuteFormatProperty, minuteformatbinding);
                        minutetextblock.SetBinding(ForegroundProperty, bindMinorTickLabelStroke);
                        timelinelabelspanel.Children.Add(minutetextblock);
                        timelineitemspanel.Children.Add(timeLineMinute);
                    }
                }
                UpdateHeight();
            }
        }

        #endregion

        #endregion

        #region Overrides

        #region OnApplyTemplate

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            timelineitemspanel = GetTemplateChild("timelineitemspanel") as UniformStackPanel;
            timelinelabelspanel = GetTemplateChild("timelinelabelspanel") as UniformStackPanel;
            if (timelinelabelspanel != null) timelinelabelspanel.Children.Clear();
            if (timelineitemspanel != null) timelineitemspanel.Children.Clear();
            base.OnApplyTemplate();
        }

        #endregion

        #endregion

        #region Events

        private void ScheduleTimeLineItemsControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            isSizeDetermined = true;
            SetupHours();
        }

        #endregion
    }
}
