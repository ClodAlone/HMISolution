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
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;
#endif
namespace Syncfusion.UI.Xaml.Schedule
{
    public class ScheduleHorizontalTimeLineHourControl : Control
    {

        #region Constructor

        public ScheduleHorizontalTimeLineHourControl()
        {
            DefaultStyleKey = typeof(ScheduleHorizontalTimeLineHourControl);
        }

        static ScheduleHorizontalTimeLineHourControl()
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

        #region Public Fields

        public readonly static int[] IntervalCount;

        /// <summary>
        /// Minimum value of <see cref="Hour"/> property.
        /// </summary>
        public const int MinValue = 0;

        /// <summary>
        /// Maximum value of <see cref="Hour"/> property.
        /// </summary>
        public const int MaxValue = 23;

        public ItemsControl timeIntervalsPanel;

        #endregion

        #region Private Fields

        private bool isTemplateApplied;

        #endregion

        #region Dependency Properties

        #region HourValue

        internal string HourValue
        {
            get { return (string)GetValue(HourValueProperty); }
            set { SetValue(HourValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HourValueProperty =
            DependencyProperty.Register("HourValue", typeof(string), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(string.Empty));

        #endregion

        #region MinuValue

        internal string MinuValue
        {
            get { return (string)GetValue(MinuValueProperty); }
            set { SetValue(MinuValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinuValueProperty =
            DependencyProperty.Register("MinuValue", typeof(string), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(string.Empty));

        #endregion

        #region ChildTickLabels

        public ObservableCollection<CustomTextBlock> ChildTickLabels
        {
            get { return (ObservableCollection<CustomTextBlock>)GetValue(ChildTickLabelsProperty); }
            internal set { SetValue(ChildTickLabelsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildTickLabelsProperty =
            DependencyProperty.Register("ChildTickLabels", typeof(ObservableCollection<CustomTextBlock>), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(null));



        #endregion

        #region MinuteFormat




        internal string MinuteFormat
        {
            get { return (string)GetValue(MinuteFormatProperty); }
            set { SetValue(MinuteFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinuteFormat.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty MinuteFormatProperty =
            DependencyProperty.Register("MinuteFormat", typeof(string), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(null));



        #endregion

        #region HourFormat



        internal string HourFormat
        {
            get { return (string)GetValue(HourFormatProperty); }
            set { SetValue(HourFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HourFormat.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty HourFormatProperty =
            DependencyProperty.Register("HourFormat", typeof(string), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(null));


        #endregion

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
        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour", typeof(int), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(MinValue, OnHourChanged));

        private static void OnHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleHorizontalTimeLineHourControl)d;
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

            UpdateHourValue();
        }

        #endregion

        #region IsAmPmTimeMode

        /// <summary>
        /// Gets or sets a value indicating whether hour is showed in 24 hours or AM/PM time format.
        /// </summary>
        public bool IsAmPmTimeMode
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
        public static readonly DependencyProperty IsAmPmTimeModeProperty = DependencyProperty.Register("IsAmPmTimeMode", typeof(bool), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(true, OnIsAmPmTimeModeChanged));

        private static void OnIsAmPmTimeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleHorizontalTimeLineHourControl)d;
            instance.UpdateHourValue();
        }

        #endregion

        #region LinesStroke

        /// <summary>
        /// The identifier for the <see cref="TimeLineHourControl.LinesStroke"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty LinesStrokeProperty = DependencyProperty.Register("LinesStroke", typeof(Brush), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the lines' stroke.
        /// </summary>
        /// <value>The lines stroke.</value>
        public Brush LinesStroke
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

        #region HourStroke

        /// <summary>
        /// The identifier for the <see cref="TimeLineHourControl.HourStroke"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty HourStrokeProperty = DependencyProperty.Register("HourStroke", typeof(Brush), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

        /// <summary>
        /// Gets or sets the Hour' stroke.
        /// </summary>
        /// <value>The Hour stroke.</value>
        public Brush HourStroke
        {
            get
            {
                return (Brush)GetValue(HourStrokeProperty);
            }

            set
            {
                SetValue(HourStrokeProperty, value);
            }
        }

        #endregion

        #region TimeInterval

        /// <summary>
        /// Gets or sets TimeInterval.
        /// </summary>
        public TimeInterval TimeInterval
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
        public static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(TimeInterval.OneHour, OnTimeIntervalChanged));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ScheduleHorizontalTimeLineHourControl)d;
            instance.OnTimeIntervalChanged();
        }

        private void OnTimeIntervalChanged()
        {
            UpdateTimeInterval();
        }


        #endregion

        #region TimelineVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineVisibility.
        /// </summary>

        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(Visibility.Visible));
        public Visibility TimelineVisibility
        {
            get { return (Visibility)GetValue(TimelineVisibilityProperty); }
            set { SetValue(TimelineVisibilityProperty, value); }
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

        internal static readonly DependencyProperty TimelineHourDivisionVisibilityProperty = DependencyProperty.Register("TimelineHourDivisionVisibility", typeof(Visibility), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(Visibility.Visible));
        #endregion


        internal TimeSpan TimeSpanValue
        {
            get { return (TimeSpan)GetValue(TimeSpanValueProperty); }
            set { SetValue(TimeSpanValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeSpanValue.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty TimeSpanValueProperty =
            DependencyProperty.Register("TimeSpanValue", typeof(TimeSpan), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(null));



        #endregion


        #region Methods

        private void UpdateHourValue()
        {

            if (IsAmPmTimeMode)
            {
                HourFormat = "hh tt";
                MinuteFormat = "mm";

            }
            else
            {
                HourFormat = "hh";
                MinuteFormat = "mm";
            }
        }

        private void UpdateTimeInterval()
        {
            if (timeIntervalsPanel == null)
            {
                return;
            }

            if (timeIntervalsPanel.Items != null && timeIntervalsPanel.Items.Count > 0)
            {
                timeIntervalsPanel.Items.Clear();
            }
            var minuteformatebinding = new Binding { Source = this };
            minuteformatebinding.Path = new PropertyPath("MinuteFormat");
            var bindMinuteStroke = new Binding { Path = new PropertyPath("LinesStroke"), Source = this };
            int intervalCount = IntervalCount[(int)TimeInterval];
            var childstring = new ObservableCollection<CustomTextBlock>();
            int totalIntervals = intervalCount;
            if (totalIntervals > 1)
            {
                for (int i = 0; i < totalIntervals; i++)
                {                    
                    var isVisible = i % intervalCount != 0;
                    var line = new Line
                    {
                        Y2 = 1,
                        StrokeThickness = 0.5,
                        Stretch = Stretch.Fill,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Width = 1
                    };
                    if (isVisible)
                    {
                        var text = new CustomTextBlock
                            {
                                VerticalAlignment = VerticalAlignment.Bottom,
                                HorizontalAlignment = HorizontalAlignment.Left,
                                TimeSpanValue = new TimeSpan(TimeSpanValue.Hours, (i * (60 / totalIntervals)), 0)
                            };

                        text.FontSize = 14;
                        text.Margin = new Thickness(0, 5, 0, 0);
                        text.SetBinding(CustomTextBlock.MinuteFormatProperty, minuteformatebinding);
                        text.Foreground = Foreground;
                        childstring.Add(text);
                        line.SetBinding(Shape.StrokeProperty, bindMinuteStroke);
                    }                   
                    if (timeIntervalsPanel.Items != null)
                    {
                        timeIntervalsPanel.Items.Add(line);                        
                    }
                    //To Fix the test placed wrong when interval is more than one
                    if (i == 0 )
                    {
                        var text = new CustomTextBlock();
                      
                        childstring.Add(text);
                    }
                }
            }
            ChildTickLabels = childstring;
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
            timeIntervalsPanel = GetTemplateChild("PART_HorizontalTimeIntervalsPanel") as ItemsControl;
            isTemplateApplied = true;
            var schedule = this.FindParentElementOfType<SfSchedule>();
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
            UpdateHourValue();
            UpdateTimeInterval();
        }

        #endregion

    }
}
