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

    /// <summary>
    /// Represents an hour of <see cref="Schedule"/>'s day TimeLineHourControl.
    /// </summary>
#if SyncfusionFramework4_0 && !SILVERLIGHT
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ScheduleTimeLineHourControl : Control
    {
        /// <summary>
        ///  reference for static integer array
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

        #region ctor


        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleTimeLineHourControl"/>
        /// class.
        /// </summary>
        public ScheduleTimeLineHourControl()
        {
            this.DefaultStyleKey = typeof(ScheduleTimeLineHourControl);
        }


        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.Windows.Controls.Schedule.ScheduleTimeLineHourControl"/>
        /// class.
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

        #region Hour

        /// <summary>
        /// Gets or sets the hour of day.
        /// </summary>
        public int Hour
        {
            get
            {
                return (int)GetValue(ScheduleTimeLineHourControl.HourProperty);
            }

            set
            {
                SetValue(ScheduleTimeLineHourControl.HourProperty, value);
            }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for Hour.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HourProperty = DependencyProperty.Register("Hour", typeof(int), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(MinValue, new PropertyChangedCallback(OnHourChanged)));

        private static void OnHourChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleTimeLineHourControl instance = (ScheduleTimeLineHourControl)d;
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
        public static readonly DependencyProperty IsAmPmTimeModeProperty = DependencyProperty.Register("IsAmPmTimeMode", typeof(bool), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(true, new PropertyChangedCallback(OnIsAmPmTimeModeChanged)));

        private static void OnIsAmPmTimeModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleTimeLineHourControl instance = (ScheduleTimeLineHourControl)d;
            instance.UpdateHourValue();
        }

        #endregion

        #region TimelineHourDivisionVisibility (DependencyProperty)

        /// <summary>
        /// Gets / Sets the ScheduleTimelineHourDivisionVisibility.
        /// </summary>
        public Visibility TimelineHourDivisionVisibility
        {
            get { return (Visibility)GetValue(TimelineHourDivisionVisibilityProperty); }
            set { SetValue(TimelineHourDivisionVisibilityProperty, value); }
        }
        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimelineHourDivisionVisibility.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimelineHourDivisionVisibilityProperty = DependencyProperty.Register("TimelineHourDivisionVisibility", typeof(Visibility), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(Visibility.Visible));
        #endregion
        

        private bool isTemplateApplied = false;
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.hourValue = GetTemplateChild("HourValue") as TextBlock;
            this.minsValue = GetTemplateChild("MinsValue") as TextBlock;
            this.timeIntervalsPanel = this.GetTemplateChild("PART_TimeIntervalsPanel") as UniformStackPanel;
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

                if (hour == 0)
                {
                    //	hour = 12;
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
            if (hour < 10)
                hourString = "0" + hourString;             

            
            if (this.hourValue != null)
            {
                // if (this.TimeInterval != TimeInterval.OneHour)
                // {
                this.hourValue.Text = hourString;
                // this.hourValue.Visibility = Visibility.Visible;
                // }
                // else
                //  {
                //    this.hourValue.Visibility = Visibility.Collapsed;
                // }
            }

            if (this.minsValue != null)
            {
                //	if (this.TimeInterval != TimeInterval.OneHour)
                //	{
                this.minsValue.Text = minuteString;
                //	}
                //	else
                //	{
                //	this.minsValue.Text = hourString + ":" + minuteString;
                //	}				
            }
        }

        #region LinesStroke

        /// <summary>
        ///  Using a DependencyProperty as the backing store for LinesStroke.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty LinesStrokeProperty = DependencyProperty.Register("LinesStroke", typeof(Brush), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the lines' stroke.
        /// </summary>
        /// <value>The lines stroke.</value>
        public Brush LinesStroke
        {
            get
            {
                return (Brush)GetValue(ScheduleTimeLineHourControl.LinesStrokeProperty);
            }

            set
            {
                SetValue(ScheduleTimeLineHourControl.LinesStrokeProperty, value);
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
        public static readonly DependencyProperty TimelineVisibilityProperty = DependencyProperty.Register("TimelineVisibility", typeof(Visibility), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(Visibility.Visible));
        #endregion
       

        #region TimeInterval

        /// <summary>
        /// Gets or sets TimeInterval.
        /// </summary>
        public TimeInterval TimeInterval
        {
            get
            {
                return (TimeInterval)GetValue(ScheduleTimeLineHourControl.TimeIntervalProperty);
            }

            set
            {
                SetValue(ScheduleTimeLineHourControl.TimeIntervalProperty, value);
            }
        }

        /// <summary>
        ///  Using a DependencyProperty as the backing store for TimeInterval.  This
        /// enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TimeIntervalProperty = DependencyProperty.Register("TimeInterval", typeof(TimeInterval), typeof(ScheduleTimeLineHourControl), new PropertyMetadata(TimeInterval.OneHour, new PropertyChangedCallback(OnTimeIntervalChanged)));

        private static void OnTimeIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScheduleTimeLineHourControl instance = (ScheduleTimeLineHourControl)d;
            instance.OnTimeIntervalChanged(e);
        }

        private void OnTimeIntervalChanged(DependencyPropertyChangedEventArgs e)
        {
            this.UpdateTimeInterval();
        }

        /// <summary>
        /// reference for UniformStackPanel
        /// </summary>
        public UniformStackPanel timeIntervalsPanel;
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

            int intervalCount = ScheduleTimeLineHourControl.IntervalCount[(int)this.TimeInterval];
            int totalIntervals = intervalCount; //(this.EndWorkHour - this.StartWorkHour) * intervalCount;
            for (int i = 0;i < totalIntervals;i++)
            {
                var isVisible = i % intervalCount != 0;
                Line line = new Line()
                {
                    X2 = 1,
                    Stroke = isVisible ? this.LinesStroke : null,
                    StrokeThickness = 0.5,
                    Stretch = Stretch.Fill,
                    VerticalAlignment = VerticalAlignment.Top,
                    Height = 1
                };
                this.timeIntervalsPanel.Children.Add(line);
            }
        }

        #endregion


#if COMMENT
        public void handleViewMode(ViewMode viewmode, ResourceDictionary rd)
        {
            if (viewmode == ViewMode.Horizontal)
            {
                // this.Style = rd["Horizontal_TimelineHour"] as Style;
                Grid timelinehourgrid = this.GetTemplateChild("TimeLinehourGrid") as Grid;
                if (timelinehourgrid != null)
                {
                    ColumnDefinition cd1 = new ColumnDefinition();
                    cd1.Width = new GridLength(10d);
                    ColumnDefinition cd2 = new ColumnDefinition();
                    ColumnDefinition cd3 = new ColumnDefinition();
                    timelinehourgrid.ColumnDefinitions.Clear();
                    timelinehourgrid.RowDefinitions.Clear();
                    timelinehourgrid.ColumnDefinitions.Add(cd1);
                    timelinehourgrid.ColumnDefinitions.Add(cd2);
                    timelinehourgrid.ColumnDefinitions.Add(cd3);
                    timelinehourgrid.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetColumn(hourValue, 1);
                    Grid.SetColumn(minsValue, 2);
                    Line line1 = this.GetTemplateChild("Line1") as Line;
                    if (line1 != null)
                    {
                        line1.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {

                // this.Style = rd["Vertical_TimelineHour"] as Style;
                //Time Line Hour Grid definitions
                Grid timelinehourgrid = this.GetTemplateChild("TimeLinehourGrid") as Grid;
                if (timelinehourgrid != null)
                {
                    ColumnDefinition columndef1 = new ColumnDefinition();
                    ColumnDefinition columndef2 = new ColumnDefinition();
                    timelinehourgrid.ColumnDefinitions.Clear();
                    timelinehourgrid.RowDefinitions.Clear();
                    timelinehourgrid.ColumnDefinitions.Add(columndef1);
                    timelinehourgrid.ColumnDefinitions.Add(columndef2);
                    timelinehourgrid.HorizontalAlignment = HorizontalAlignment.Left;
                    Grid.SetColumn(hourValue, 0);
                    Grid.SetColumn(minsValue, 1);
                    Line line1 = this.GetTemplateChild("Line1") as Line;
                    if (line1 != null)
                    {
                        line1.Visibility = Visibility.Visible;
                    }
                }
            }

            this.UpdateLayout();
        } 
#endif
    }
}
