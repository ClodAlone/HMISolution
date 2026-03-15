#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Schedule
{
    using System;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

#if SyncfusionFramework4_0 && !SILVERLIGHT
    /// <summary>
    ///  class  that holds TimeLine control for schedule view
    /// </summary>
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleHorizontalTimeLineHourControl : Control
    {
        /// <summary>
        ///  IntervalCount as a reference for static integer array
        /// </summary>
        public readonly static int[] IntervalCount;
        private TextBlock hourValue;
        private TextBlock minsValue;

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

        #region Hour

        /// <summary>
        /// Gets or sets the hour of day.
        /// </summary>
        public int Hour
        {
            get
            {
                return (int)GetValue(ScheduleHorizontalTimeLineHourControl.HourProperty);
            }

            set
            {
                SetValue(ScheduleHorizontalTimeLineHourControl.HourProperty, value);
            }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Hour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour", typeof(int), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(MinValue, new PropertyChangedCallback(OnHourChanged)));

        private static void OnHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleHorizontalTimeLineHourControl instance = (ScheduleHorizontalTimeLineHourControl)d;
            instance.OnHourChanged(e);
        }

        private void OnHourChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!this.isTemplateApplied)
            {
                return;
            }

            int val = (int)e.NewValue;
            if (val < MinValue)
            {
                this.Hour = MinValue;
            }
            else if (val > MaxValue)
            {
                this.Hour = MaxValue;
            }

            this.UpdateHourValue();
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
        ///  Using a DependencyProperty as the backing store for IsAmPmTimeMode.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsAmPmTimeModeProperty = DependencyProperty.Register("IsAmPmTimeMode", typeof(bool), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(true, new PropertyChangedCallback(OnIsAmPmTimeModeChanged)));

        private static void OnIsAmPmTimeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleHorizontalTimeLineHourControl instance = (ScheduleHorizontalTimeLineHourControl)d;
            instance.UpdateHourValue();
        }

        #endregion

        #region LinesStroke

        /// <summary>
        ///  Using a DependencyProperty as the backing store for LinesStroke.  This
        /// enables animation, styling, binding, etc...
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
                return (Brush)GetValue(ScheduleHorizontalTimeLineHourControl.LinesStrokeProperty);
            }

            set
            {
                SetValue(ScheduleHorizontalTimeLineHourControl.LinesStrokeProperty, value);
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
                return (TimeInterval)GetValue(ScheduleHorizontalTimeLineHourControl.TimeIntervalProperty);
            }

            set
            {
                SetValue(ScheduleHorizontalTimeLineHourControl.TimeIntervalProperty, value);
            }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimeInterval.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(TimeInterval.OneHour, new PropertyChangedCallback(OnTimeIntervalChanged)));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleHorizontalTimeLineHourControl instance = (ScheduleHorizontalTimeLineHourControl)d;
            instance.OnTimeIntervalChanged(e);
        }

        private void OnTimeIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateTimeInterval();
        }

        private void UpdateTimeInterval()
        {
            if (this.timeIntervalsPanel == null)
            {
                return;
            }

            if (this.timeIntervalsPanel.Children.Count > 0)
            {
                this.timeIntervalsPanel.Children.Clear();
            }

            int intervalCount = ScheduleHorizontalTimeLineHourControl.IntervalCount[(int)this.TimeInterval];
            for (int i = 0; i < intervalCount; i++)
            {
                var rectangletopside = new ScheduleRectangleBorderExt()
                {
                    RightBrush = ((intervalCount - 1) == i) ? null : this.LinesStroke,
                    BorderThickness = new Thickness(0, 0, 0.5, 0)
                };
                this.timeIntervalsPanel.Children.Add(rectangletopside);
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
            set { SetValue(TimelineVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimelineVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(ScheduleHorizontalTimeLineHourControl), new PropertyMetadata(Visibility.Visible));
      
        #endregion
       

        #region constructor


        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeLineHourControl"/>
        /// class.
        /// </summary>
        public ScheduleHorizontalTimeLineHourControl()
        {
            this.DefaultStyleKey = typeof(ScheduleHorizontalTimeLineHourControl);
        }


        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleHorizontalTimeLineHourControl"/>
        /// class.
        /// </summary>
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

        /// <summary>
        ///  reference for uniformStackPanel
        /// </summary>
        public UniformStackPanel timeIntervalsPanel;
        private bool isTemplateApplied = false;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.hourValue = GetTemplateChild("HourValue") as TextBlock;
            this.minsValue = GetTemplateChild("MinsValue") as TextBlock;
            this.timeIntervalsPanel = this.GetTemplateChild("PART_HorizontalTimeIntervalsPanel") as UniformStackPanel;
            this.isTemplateApplied = true;

            this.UpdateHourValue();
            this.UpdateTimeInterval();
        }

        private void UpdateHourValue()
        {
            if (!this.isTemplateApplied)
            {
                return;
            }

            int hour = this.Hour;
            if (this.IsAmPmTimeMode)
            {
                if (hour > 12)
                {
                    hour -= 12;
                }
            }

            string minuteString;
            if (this.IsAmPmTimeMode && (hour % 12) == 0)
            {
                if (hour != 0)
                {
                    minuteString = hour < 12 ? "am" : "pm";
                }
                else
                {
                    hour = 12;
                    minuteString = "am";
                }
            }
            else
            {
                minuteString = "00";
            }

            string hourString = hour.ToString();
            if (this.hourValue != null)
            {
                this.hourValue.Text = hourString;
            }

            if (this.minsValue != null)
            {
                this.minsValue.Text = minuteString;
            }
        }
    }
}
