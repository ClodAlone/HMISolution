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
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Text;
#else
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents an items control for arranging items in timeline view.
    /// </summary>
    public class ScheduleHorizontalTimeLineItemsControl : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleHorizontalTimeLineItemsControl">ScheduleHorizontalTimeLineItemsControl</see> class. 
        /// </summary>
        public ScheduleHorizontalTimeLineItemsControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalTimeLineItemsControl);
        }

        #endregion

        #region Public Fields

        public const double DefaultIntervalWidth = 80d;

        #endregion

        #region Internal Fields

        private bool isTemplateApplied;
        internal UniformStackPanel timelineitemspanel;
        internal UniformStackPanel timelinelabelspanel;

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
            DependencyProperty.Register("SelectedDates", typeof(ObservableCollection<DateTime>), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(null, SelectedDatesChanged));

        private static void SelectedDatesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                var obj = d as ScheduleHorizontalTimeLineItemsControl;
                var observableCollection = e.OldValue as ObservableCollection<DateTime>;
                if (observableCollection != null && observableCollection.Count != obj.SelectedDates.Count)//|| obj.Items.Count != obj.SelectedDates.Count)
                    OnTimeLinePropertyChanged(d, e);
            }
        }
        #endregion

        #region TimeInterval
        /// <summary>
        /// Gets or sets the time interval for timeline view.
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
            DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeLinePropertyChanged));
        #endregion

        #region TimeMode
        /// <summary>
        /// Gets or sets the time mode of timeline view.
        /// </summary>
        /// <seealso cref="T:Syncfusion.UI.Xaml.Schedule.TimeModes"></seealso>
        public TimeModes TimeMode
        {
            get { return (TimeModes)GetValue(TimeModeProperty); }
            set { SetValue(TimeModeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for TimeMode.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeModeProperty = DependencyProperty.Register("TimeMode", typeof(TimeModes),
            typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(TimeModes.TwelveHours, OnTimeModeChanged));

        private static void OnTimeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                var schedule = (d as ScheduleHorizontalTimeLineItemsControl).FindParentElementOfType<SfSchedule>();
                if (schedule != null)
                {
                    TimeModes mode = (d as ScheduleHorizontalTimeLineItemsControl).TimeMode;
                    schedule.MajorTickTimeFormat = mode == TimeModes.TwentyFourHours ? "HH" : "hh tt";
                }
            }
        }
        #endregion

        #region IntervalWidth
        /// <summary>
        /// Gets the width of the interval in timeline view.
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
            DependencyProperty.Register("IntervalWidth", typeof(double), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(DefaultIntervalWidth, OnIntervalWidthChanged));

        private static void OnIntervalWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleHorizontalTimeLineItemsControl)d;
            instance.UpdateWidth();
        }
        #endregion

        #region MinorTickVisibility
        /// <summary>
        /// Gets or sets the visibility of minor ticks that represent minute in time slot.
        /// </summary>
        public Visibility MinorTickVisibility
        {
            get { return (Visibility)GetValue(MinorTickVisibilityProperty); }
            set { SetValue(MinorTickVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickVisibility.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickVisibilityProperty =
            DependencyProperty.Register("MinorTickVisibility", typeof(Visibility), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(Visibility.Visible));
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
            DependencyProperty.Register("MajorTickVisibility", typeof(Visibility), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region MinorTickStroke
        /// <summary>
        /// Gets or sets the stroke color for minor ticks that represent minute in time slot.
        /// </summary>
        public Brush MinorTickStroke
        {
            get { return (Brush)GetValue(MinorTickStrokeProperty); }
            set { SetValue(MinorTickStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MinorTickStrokeProperty =
            DependencyProperty.Register("MinorTickStroke", typeof(Brush), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.White)));
        #endregion

        #region MajorTickStroke
        /// <summary>
        /// Gets or sets the stroke color for major ticks that represent hour in time slot.
        /// </summary>
        public Brush MajorTickStroke
        {
            get { return (Brush)GetValue(MajorTickStrokeProperty); }
            set { SetValue(MajorTickStrokeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty MajorTickStrokeProperty =
            DependencyProperty.Register("MajorTickStroke", typeof(Brush), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));
        #endregion

        #region MajorTickLabelStroke
        public Brush MajorTickLabelStroke
        {
            get { return (Brush)GetValue(MajorTickLabelStrokeProperty); }
            set { SetValue(MajorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickLabelStrokeProperty =
            DependencyProperty.Register("MajorTickLabelStroke", typeof(Brush), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));
        #endregion

        #region MinorTickLabelStroke
        public Brush MinorTickLabelStroke
        {
            get { return (Brush)GetValue(MinorTickLabelStrokeProperty); }
            set { SetValue(MinorTickLabelStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickLabelStrokeProperty =
            DependencyProperty.Register("MinorTickLabelStroke", typeof(Brush), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(new SolidColorBrush(Colors.LightGray)));

        private static void OnTimeLinePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                var scheduleHorizontalTimeLineItemsControl = d as ScheduleHorizontalTimeLineItemsControl;
                scheduleHorizontalTimeLineItemsControl.UpdateTimeLineItemsControl();
            }
        }
        #endregion

        #region MajorTickStrokeDashArray
        public DoubleCollection MajorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MajorTickStrokeDashArrayProperty); }
            internal set { SetValue(MajorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MajorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(new DoubleCollection(), OnMajorTickStrokeDashArrayChanged));

        private static void OnMajorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                var scheduleHorizontalTimeLineItemsControl = (d as ScheduleHorizontalTimeLineItemsControl);
                scheduleHorizontalTimeLineItemsControl.MajorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                scheduleHorizontalTimeLineItemsControl.UpdateTimeLineItemsControl();
            }
        }
        #endregion

        #region MinorTickStrokeDashArray
        public DoubleCollection MinorTickStrokeDashArray
        {
            get { return (DoubleCollection)GetValue(MinorTickStrokeDashArrayProperty); }
            internal set { SetValue(MinorTickStrokeDashArrayProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorTickStrokeDashArray.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorTickStrokeDashArrayProperty =
            DependencyProperty.Register("MinorTickStrokeDashArray", typeof(DoubleCollection), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(new DoubleCollection(), OnMinorTickStrokeDashArrayChanged));

        private static void OnMinorTickStrokeDashArrayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                var scheduleHorizontalTimeLineItemsControl = (d as ScheduleHorizontalTimeLineItemsControl);
                scheduleHorizontalTimeLineItemsControl.MinorLineStrokeDashArray = (DoubleCollection)e.NewValue;
                scheduleHorizontalTimeLineItemsControl.UpdateTimeLineItemsControl();
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
            DependencyProperty.Register("Minimum", typeof(int), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(0, OnMinimumChanged));

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                ScheduleHorizontalTimeLineItemsControl timeLineItemsControl = (d as ScheduleHorizontalTimeLineItemsControl);
                if (!timeLineItemsControl.ShowNonWorkingHours)
                {
                    ScheduleTimeLineItemsControl.MinValue = (int)e.NewValue;
                    timeLineItemsControl.UpdateTimeLineItemsControl();
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
            DependencyProperty.Register("Maximum", typeof(int), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(24, OnMaximumChanged));

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                ScheduleHorizontalTimeLineItemsControl timeLineItemsControl = (d as ScheduleHorizontalTimeLineItemsControl);
                if (!timeLineItemsControl.ShowNonWorkingHours)
                {
                    ScheduleTimeLineItemsControl.MaxValue = (int)e.NewValue;
                    timeLineItemsControl.UpdateTimeLineItemsControl();
                }
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
            DependencyProperty.Register("ShowNonWorkingHours", typeof(bool), typeof(ScheduleHorizontalTimeLineItemsControl), new PropertyMetadata(true, OnShowNonWorkingHoursChanged));

        private static void OnShowNonWorkingHoursChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScheduleHorizontalTimeLineItemsControl)
            {
                ScheduleHorizontalTimeLineItemsControl timeLineItemsControl = (d as ScheduleHorizontalTimeLineItemsControl);
                ScheduleTimeLineItemsControl.MinValue = timeLineItemsControl.ShowNonWorkingHours ? 0 : timeLineItemsControl.Minimum;
                ScheduleTimeLineItemsControl.MaxValue = timeLineItemsControl.ShowNonWorkingHours ? 24 : timeLineItemsControl.Maximum;
                timeLineItemsControl.UpdateTimeLineItemsControl();
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

        private void UpdateWidth()
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            var timelineview = this.FindParentElementOfType<ScheduleTimeLineView>();
            if (schedule != null && SelectedDates != null && (schedule.isIntervalHeightset || !schedule.EnableAutoFormat) && timelineview != null)
            {
                Width = timelineview.GetTimeSlotWidth(SelectedDates.Count);
            }
        }

        private void UpdateTimeLineItemsControl()
        {
            if (isTemplateApplied)
                SetupHours(SelectedDates.Count);
        }

        private void SetupHours(int selectedDatesCount)
        {
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule == null)
            {
                return;
            }
            if (timelineitemspanel != null && timelinelabelspanel != null)
            {
                timelineitemspanel.Children.Clear();
                timelinelabelspanel.Children.Clear();
                var hourformatbinding = new Binding { Path = new PropertyPath("MajorTickTimeFormat"), Source = schedule };
                var minuteformatbinding = new Binding
                {
                    Path = new PropertyPath("MinorTickTimeFormat"),
                    Source = schedule
                };
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
                for (int cnt = 0; cnt < selectedDatesCount; cnt++)
                {
                    for (int i = ScheduleTimeLineItemsControl.MinValue; i < ScheduleTimeLineItemsControl.MaxValue; i++)
                    {
                        var timeLineHour = new Line
                        {
                            Y2 = 55,
                            Height = 55,
                            StrokeThickness = 1,
                            Stretch = Stretch.Fill,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            StrokeDashArray = MajorLineStrokeDashArray
                        };
                        timeLineHour.SetBinding(Shape.StrokeProperty, bindMajorTickStroke);
                        timeLineHour.SetBinding(VisibilityProperty, bindHourvisibility);
                        var textblock = new CustomTextBlock
                        {
                            FontFamily = new FontFamily("Segoe UI"),
                            TimeSpanValue = new TimeSpan(i, 0, 0),
                            FontSize = 14,
                            FontWeight = FontWeights.Light,
                            Margin = new Thickness(5, 0, 0, 0),
                            TextWrapping = TextWrapping.Wrap
                        };
                        textblock.SetBinding(CustomTextBlock.HourFormatProperty, hourformatbinding);
                        textblock.SetBinding(VisibilityProperty, bindHourvisibility);
                        textblock.SetBinding(ForegroundProperty, bindMajorTickLabelStroke);
                        timelinelabelspanel.Children.Add(textblock);
                        timelineitemspanel.Children.Add(timeLineHour);
                        int intervalCount = ScheduleTimeLineItemsControl.IntervalCount[(int)TimeInterval];
                        for (int j = 1; j < intervalCount; j++)
                        {
                            var timeLineMinute = new Line
                            {
                                Y2 = 25,
                                Height = 25,
                                StrokeThickness = 1,
                                Stretch = Stretch.Fill,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                StrokeDashArray = MinorLineStrokeDashArray
                            };
                            timeLineMinute.SetBinding(Shape.StrokeProperty, bindMinorTickStroke);
                            timeLineMinute.SetBinding(VisibilityProperty, bindMinutevisibility);
                            var minutetextblock = new CustomTextBlock
                            {
                                FontFamily = new FontFamily("Segoe UI"),
                                TimeSpanValue = new TimeSpan(i, (j * (60 / intervalCount)), 0),
                                FontSize = 14,
                                FontWeight = FontWeights.Light,
                                Margin = new Thickness(5, 0, 0, 0),
                                HorizontalAlignment = HorizontalAlignment.Left,
                                VerticalAlignment = VerticalAlignment.Center
                            };
                            minutetextblock.SetBinding(VisibilityProperty, bindMinutevisibility);
                            minutetextblock.SetBinding(CustomTextBlock.MinuteFormatProperty, minuteformatbinding);
                            minutetextblock.SetBinding(ForegroundProperty, bindMinorTickLabelStroke);
                            timelinelabelspanel.Children.Add(minutetextblock);
                            timelineitemspanel.Children.Add(timeLineMinute);
                        }
                    }
                }
                UpdateWidth();
            }
        }

        #endregion

        #region Override Methods

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            timelineitemspanel = GetTemplateChild("timelineitemspanel") as UniformStackPanel;
            timelinelabelspanel = GetTemplateChild("timelinelabelspanel") as UniformStackPanel;
            if (timelinelabelspanel != null) timelinelabelspanel.Children.Clear();
            base.OnApplyTemplate();
            if (!isTemplateApplied)
            {
                SetupHours(SelectedDates.Count);
            }
            isTemplateApplied = true;
        }

        #endregion
    }
}
