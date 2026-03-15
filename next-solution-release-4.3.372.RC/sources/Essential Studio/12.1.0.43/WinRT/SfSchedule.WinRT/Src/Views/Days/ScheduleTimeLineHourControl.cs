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
using Windows.UI.Xaml.Shapes;
using Windows.UI;
#else
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    public class ScheduleTimeLineHourControl : Control
    {

        #region Constants

        /// <summary>
        /// Minimum value of <see cref="Hour"/> property.
        /// </summary>
        public const int MinValue = 0;

        /// <summary>
        /// Maximum value of <see cref="Hour"/> property.
        /// </summary>
        public const int MaxValue = 23;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TimelineHour"/> class.
        /// </summary>
        public ScheduleTimeLineHourControl()
        {
            DefaultStyleKey = typeof(ScheduleTimeLineHourControl);
            ChildLines = new ObservableCollection<Line>();
            ChildTickLabels = new ObservableCollection<CustomTextBlock>();
            var schedule = this.FindParentElementOfType<SfSchedule>();
            if (schedule != null)
            {
                var formatbinding = new Binding { Source = schedule, Path = new PropertyPath("MajorTickStringFormat") };
                SetBinding(HourFormatProperty, formatbinding);
            }

        }



        /// <summary>
        /// Initializes the <see cref="TimelineHour"/> class.
        /// </summary>
        static ScheduleTimeLineHourControl()
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

        #region Dependency Properties

        #region MinuteFormat




        public string MinuteFormat
        {
            get { return (string)GetValue(MinuteFormatProperty); }
            set { SetValue(MinuteFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinuteFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinuteFormatProperty =
            DependencyProperty.Register("MinuteFormat", typeof(string), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(null));



        #endregion

        #region HourFormat



        public string HourFormat
        {
            get { return (string)GetValue(HourFormatProperty); }
            set { SetValue(HourFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourFormatProperty =
            DependencyProperty.Register("HourFormat", typeof(string), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(null));


        #endregion

        #region LinesStroke

        /// <summary>
        /// The identifier for the <see cref="TimeLineHourControl.LinesStroke"/> dependency property. 
        /// </summary>
        internal static readonly DependencyProperty LinesStrokeProperty = DependencyProperty.Register("LinesStroke", typeof(Brush), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the lines' stroke.
        /// </summary>
        /// <value>The lines stroke.</value>
        internal Brush LinesStroke
        {
            get
            {
                return (Brush)GetValue(LinesStrokeProperty);
            }

            set
            {
                SetValue(LinesStrokeProperty, value);
            }
        }

        #endregion

        #region TimelineVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineVisibility.
        /// </summary>
        public Visibility TimelineVisibility
        {
            get { return (Visibility)GetValue(TimelineVisibilityProperty); }
            internal set { SetValue(TimelineVisibilityProperty, value); }
        }

        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region HourStroke

        public Brush HourStroke
        {
            get { return (Brush)GetValue(HourStrokeProperty); }
            internal set { SetValue(HourStrokeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourStroke.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HourStrokeProperty =
            DependencyProperty.Register("HourStroke", typeof(Brush), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        #endregion

        #region HourValue

        internal string HourValue
        {
            get { return (string)GetValue(HourValueProperty); }
            set { SetValue(HourValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HourValueProperty =
            DependencyProperty.Register("HourValue", typeof(string), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(string.Empty));

        #endregion

        #region MinuValue

        internal string MinuValue
        {
            get { return (string)GetValue(MinuValueProperty); }
            set { SetValue(MinuValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinuValueProperty =
            DependencyProperty.Register("MinuValue", typeof(string), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(string.Empty));

        #endregion

        #region TimeInterval

        /// <summary>
        /// Gets or sets TimeInterval.
        /// </summary>
        internal TimeInterval TimeInterval
        {
            get
            {
                return (TimeInterval)GetValue(TimeIntervalProperty);
            }

            set
            {
                SetValue(TimeIntervalProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="TimelineHour.TimeInterval"/> dependency property. 
        /// </summary>
        internal static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleTimeLineHourControl)d;
            instance.OnTimeIntervalChanged();
        }

        private void OnTimeIntervalChanged()
        {
            UpdateTimeInterval();
        }

        #endregion



        internal TimeSpan TimeSpanValue
        {
            get { return (TimeSpan)GetValue(TimeSpanValueProperty); }
            set { SetValue(TimeSpanValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeSpanValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TimeSpanValueProperty =
            DependencyProperty.Register("TimeSpanValue", typeof(TimeSpan), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(null));



        #region Hour

        /// <summary>
        /// Gets or sets the hour of day.
        /// </summary>
        public int Hour
        {
            get
            {
                return (int)GetValue(HourProperty);
            }

            set
            {
                SetValue(HourProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="TimelineHour.Hour"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour", typeof(int), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(MinValue, OnHourChanged));

        private static void OnHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleTimeLineHourControl)d;
            instance.OnHourChanged(e);
        }

        private void OnHourChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!isTemplateApplied)
            {
                return;
            }

            var val = (int)e.NewValue;
            if (val < MinValue)
            {
                Hour = MinValue;
            }
            else if (val > MaxValue)
            {
                Hour = MaxValue;
            }


        }

        #endregion

        #region IsAmPmTimeMode

        /// <summary>
        /// Gets or sets a value indicating whether hour is showed in 24 hours or AM/PM time format.
        /// </summary>
        internal bool IsAmPmTimeMode
        {
            get
            {
                return (bool)GetValue(IsAmPmTimeModeProperty);
            }

            set
            {
                SetValue(IsAmPmTimeModeProperty, value);
            }
        }

        /// <summary>
        /// The identifier for the <see cref="TimelineHour.IsAmPmTimeMode"/> dependency property. 
        /// </summary>
        internal static readonly DependencyProperty IsAmPmTimeModeProperty = DependencyProperty.Register("IsAmPmTimeMode", typeof(bool), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(true, OnIsAmPmTimeModeChanged));

        private static void OnIsAmPmTimeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleTimeLineHourControl)d;
            instance.UpdateHourValue();
        }

        #endregion

        #region TimelineHourDivisionVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineHourDivisionVisibility.
        /// </summary>
        internal Visibility TimelineHourDivisionVisibility
        {
            get { return (Visibility)GetValue(TimelineHourDivisionVisibilityProperty); }
            set { SetValue(TimelineHourDivisionVisibilityProperty, value); }
        }

        internal static readonly DependencyProperty TimelineHourDivisionVisibilityProperty = DependencyProperty.Register("TimelineHourDivisionVisibility", typeof(Visibility), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(Visibility.Visible));
        #endregion

        #region ChildLines

        public ObservableCollection<Line> ChildLines
        {
            get { return (ObservableCollection<Line>)GetValue(ChildLinesProperty); }
            internal set { SetValue(ChildLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildLinesProperty =
            DependencyProperty.Register("ChildLines", typeof(ObservableCollection<Line>), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(null));


        #endregion

        #region ChildTickLabels

        internal ObservableCollection<CustomTextBlock> ChildTickLabels
        {
            get { return (ObservableCollection<CustomTextBlock>)GetValue(ChildTickLabelsProperty); }
            set { SetValue(ChildTickLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildString.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty ChildTickLabelsProperty =
            DependencyProperty.Register("ChildTickLabels", typeof(ObservableCollection<CustomTextBlock>), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(null));



        #endregion

        #endregion

        #region Private Fields

        private bool isTemplateApplied;

        #endregion

        #region Public Fields

        public readonly static int[] IntervalCount;

        #endregion

        #region Methods

        #region Update time Interval

        private void UpdateTimeInterval()
        {
            if (ChildLines == null && ChildTickLabels == null)
            {
                return;
            }

            if (ChildLines != null && ChildLines.Count > 0)
            {
                ChildLines.Clear();
            }
            if (ChildTickLabels.Count > 0)
                ChildTickLabels.Clear();

            int intervalCount = IntervalCount[(int)TimeInterval];
            int totalIntervals = intervalCount;
            var childlines = new ObservableCollection<Line>();
            var childstring = new ObservableCollection<CustomTextBlock>();

            for (int i = 0; i < totalIntervals; i++)
            {

                var isVisible = i % intervalCount != 0;
                if (isVisible)
                {
                    var text = new CustomTextBlock
                        {
                            VerticalAlignment = VerticalAlignment.Bottom,
                            TimeSpanValue = new TimeSpan(TimeSpanValue.Hours, (i * (60 / totalIntervals)), 0),
                            FontSize = 16,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            Foreground = Foreground
                        };
                    var binding = new Binding { Source = this, Path = new PropertyPath("MinuteFormat") };
                    text.SetBinding(CustomTextBlock.MinuteFormatProperty, binding);
                    childstring.Add(text);
                }
                var line = new Line
                    {
                        X2 = 1,
                        StrokeThickness = 3,
                        Stretch = Stretch.Fill,
                        VerticalAlignment = VerticalAlignment.Top,

                    };
                if (isVisible)
                {
                    var bindMinuteStroke = new Binding { Path = new PropertyPath("LinesStroke"), Source = this };
                    line.SetBinding(Shape.StrokeProperty, bindMinuteStroke);
                }

                childlines.Add(line);
            }


            ChildLines = childlines;
            ChildTickLabels = childstring;
        }

        #endregion

        #region Update Hour Value


        private void UpdateHourValue()
        {
            HourFormat = IsAmPmTimeMode ? "hh:mm tt" : "HH : mm";
        }

        #endregion

        #endregion

        #region Overrides

        #region OnApplytemplate

#if WINRT
        protected override void OnApplyTemplate()
#else
        public override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();
            isTemplateApplied = true;
            var schedule = this.FindParentElementOfType<SfSchedule>();
            UpdateHourValue();
            if (schedule != null)
            {
                var hourformatbinding = new Binding
                    {
                        Mode = BindingMode.TwoWay,
                        Source = schedule,
                        Path = new PropertyPath("MajorTickStringFormat")
                    };
                SetBinding(HourFormatProperty, hourformatbinding);
                var minuteformatbinding = new Binding
                    {
                        Mode = BindingMode.TwoWay,
                        Source = schedule,
                        Path = new PropertyPath("MinorTickStringFormat")
                    };
                SetBinding(MinuteFormatProperty, minuteformatbinding);
            }
            UpdateTimeInterval();
        }

        #endregion

        #endregion


    }
}
