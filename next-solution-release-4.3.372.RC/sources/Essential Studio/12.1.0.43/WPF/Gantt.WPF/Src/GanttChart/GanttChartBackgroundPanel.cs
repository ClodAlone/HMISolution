#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
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
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Collections.Generic;
using Syncfusion.Windows.Controls.Gantt.Schedule;
using System.Windows.Data;
using System.Windows.Threading;

namespace Syncfusion.Windows.Controls.Gantt.Chart
{
    /// <summary>
    /// Represents a panel that will arrange the background cells of GanttChart
    /// </summary>
    public class GanttChartBackgroundPanel : VirtualizingPanel
    {
        #region Internal/Private members

#if !SILVERLIGHT
        private double previousTimeDiff = 0d;
        TimeSpan previousStartTime = new TimeSpan();
#endif
        private double _startPoint = 0d;
        private double _endPoint = 1000d;

		DispatcherTimer dynamicTimer = new DispatcherTimer();
        DateTime defaultChartStartTime = DateTime.Today.AddDays(-14);
        DateTime defaultChartEndTime = DateTime.Today.AddDays(14);
        DateTime _startTime = DateTime.Now.AddDays(-15);
        DateTime _endTime = DateTime.Now.AddDays(45);
        
        internal GanttChart ParentControl { get; set; }
        internal bool IsOnInitialize = true;

        #endregion      

        /// <summary>
        /// Gets or sets the start point.
        /// </summary>
        /// <value>
        /// The start point.
        /// </value>
        public double StartPoint
        {
            get { return _startPoint; }
            set { _startPoint = value; }
        }

        /// <summary>
        /// Gets or sets the end point.
        /// </summary>
        /// <value>
        /// The end point.
        /// </value>
        public double EndPoint
        {
            get { return _endPoint; }
            set { _endPoint = value; }
        }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        internal DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (_startTime != value && defaultChartStartTime != value)
                {
                    _startTime = value;

                    if (!this.IsOnInitialize)
                        ResetBackground();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        internal DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (_endTime != value && defaultChartEndTime!=value)
                {
                    _endTime = value;

                    if (!this.IsOnInitialize)
                        ResetBackground();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GanttChartBackgroundPanel"/> class.
        /// </summary>
        public GanttChartBackgroundPanel()
        {
            dynamicTimer.Interval = TimeSpan.FromMilliseconds(1);
            dynamicTimer.Tick += timer_Tick;
        }
      
        /// <summary>
        /// Resets the background.
        /// </summary>
        internal void ResetBackground()
        {
            if (this.ParentControl.ParentControl.GanttSchedule != null)
#if !SILVERLIGHT
                // LowerCellUnit is considered to avoid drawing the backgound on zooming/custom schedule.
                if ((this.ParentControl.ParentControl.GanttSchedule.LowerTimeUnit != TimeUnit.Days && (this.ParentControl.ParentControl.GanttSchedule.LowerTimeUnit != TimeUnit.Hours
                    || this.ParentControl.ParentControl.GanttSchedule.ScheduleType == ScheduleType.CustomDateTime)) || this.ParentControl.ParentControl.GanttSchedule.LowerCellUnit != 1)
                {
#else
                if (this.ParentControl.ParentControl.GanttSchedule.LowerTimeUnit != TimeUnit.Days || this.ParentControl.ParentControl.GanttSchedule.LowerCellUnit != 1)
                {
#endif
                    this.Children.Clear();
#if !SILVERLIGHT
                    if (this.ParentControl.ShowGridLinesOnZooming && (this.ParentControl.ParentControl.GanttSchedule.LowerTimeUnit == TimeUnit.Hours || this.ParentControl.ParentControl.GanttSchedule.LowerTimeUnit== TimeUnit.Minutes))
                        this.AddGridLines();
#endif
                    return;
                }
            // LowerCellUnit is considered to avoid drawing the backgound on zooming/custom schedule.
            if (this.ParentControl.ParentControl.ScheduleType != ScheduleType.CustomNumeric && this.ParentControl.ParentControl.GanttSchedule.LowerCellUnit == 1)
            {               
                if (IsOnInitialize)
                    IsOnInitialize = false;

                this.Children.Clear();

                this.AddBackgroundCells();               
#if !SILVERLIGHT
                if (this.ParentControl.ParentControl.ScheduleType == ScheduleType.MonthWithHours || this.ParentControl.ParentControl.ScheduleType == ScheduleType.DayWithHours)
                {
                    this.AddHourBackgroundCells();
                }
#endif
                this.AddGridLines();  
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Adds the hour background cells.
        /// </summary>
        private void AddHourBackgroundCells()
        {
            DateTime tempStart = this.StartTime;
            DateTime tempEnd = this.EndTime;
            TimeSpan startTime = this.ParentControl.ParentControl.DefaultStartTime;
            TimeSpan endTime = this.ParentControl.ParentControl.DefaultEndTime;

            if (!this.ParentControl.ShowNonWorkingHoursBackground)
                return;
            
            //Calculating the time difference between default start time and default end time.
            double timeDiff = (endTime.Subtract(startTime).TotalHours);

            if (timeDiff > 0)
            {
                //Time Difference is stored in one variable when the time difference is having positive value
                //If the tim difference is getting negative value means the Hour background cells will be drawn using the previous stored positive time difference value.
                previousTimeDiff = timeDiff;
                previousStartTime = startTime;
            }
            else
            {
                timeDiff=previousTimeDiff;
            }
           

            while (tempStart.CompareTo(tempEnd) < 0)
            {
                if (tempStart.DayOfWeek != DayOfWeek.Sunday && tempStart.DayOfWeek != DayOfWeek.Saturday)
                {
                    double tempTimeDiff = timeDiff;
                    //To display the NonWorkingHours in correct place we round the hour value of the tempStart
                    int hour = tempStart.Hour == 0 ? 0 : -tempStart.Hour;
                    tempStart = tempStart.AddHours(hour);

                    int min = tempStart.Minute == 0 ? 0 : -tempStart.Minute;
                    tempStart = tempStart.AddMinutes(min);

                    var temp = tempStart;
                    double mergeCells;

                    //When the DefaultStartTime is greater then the DefaultEndTime the postion of the Nonworking hours will be get the wrong value.
                    //So we storing the PrevioustartTime to the mergecell to predict that issue.
                    if (startTime >= endTime)
                    {
                        mergeCells = previousStartTime.TotalHours;
                    }
                    else
                    {
                        mergeCells = startTime.TotalHours;
                    }

                    var backgroundCell = GetBackgroundCell(tempStart, TimeUnit.Hours, mergeCells);
                    backgroundCell.Tag = this.ParentControl.GetStartPositionOfDate(tempStart);
                    this.Children.Add(backgroundCell);

                    if (tempTimeDiff < 4)
                    {
                        //here we add the hour value with timediff to show the Working hours below 4.
                        temp = temp.AddHours(mergeCells + tempTimeDiff);
                    }
                    else
                    {
                        //here we add the hour value with 4 to show the working hours in a Day
                        temp = temp.AddHours(mergeCells + 4);
                    }

                    tempTimeDiff = tempTimeDiff - 4;

                    if (tempTimeDiff > 0)
                    {
                        backgroundCell = GetBackgroundCell(tempStart, TimeUnit.Hours, 1);
                        backgroundCell.Tag = this.ParentControl.GetStartPositionOfDate(temp, TimeUnit.Hours);
                        this.Children.Add(backgroundCell);

                        temp = temp.AddHours(1);
                        tempTimeDiff = tempTimeDiff - 1;
                    }

                    if (tempTimeDiff > 0)
                    {
                        temp = temp.AddHours(tempTimeDiff); 
                    }
               
                    mergeCells = new TimeSpan(24, 0, 0).Subtract(endTime).TotalHours;

                    backgroundCell = GetBackgroundCell(tempStart, TimeUnit.Hours, mergeCells);
                    backgroundCell.Tag = this.ParentControl.GetStartPositionOfDate(temp, TimeUnit.Hours);
                    this.Children.Add(backgroundCell);
                }

                tempStart = tempStart.AddDays(1);
            }
        }

#endif
        /// <summary>
        /// Arranges the override.
        /// </summary>
        /// <param name="arrangeSize">Size of the arrange.</param>
        /// <returns></returns>
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            foreach (UIElement child in Children)
            {
                if (child is Line)
                {
                    //Arrange the CurrentDateLine 
                    Line line = (child as Line);
                    
                    //Set the Y2 value for the Line as ActualHeight of the panel only when the StickCurrentDateLineTo is not equal to "Custom"
                    line.Y2 = this.ParentControl.StickCurrentDateLineTo != CurentDateLinePositions.Absolute ? arrangeSize.Height : line.Y2;
                    line.Arrange(new Rect(0, 0, this.ActualWidth, arrangeSize.Height));
                }
                else
                {
                    Rectangle rect = (child as Rectangle);
                    rect.Arrange(new Rect((double)rect.Tag, 0, rect.Width, arrangeSize.Height));
                }
            }

#if SILVERLIGHT
            // To provide the sharp edge the clip is added. To avoid exception following conditions are added.
            if (this.ParentControl != null && !double.IsNaN(this.ParentControl.Width) && !double.IsInfinity(this.ParentControl.Width) && this.ParentControl.Width > 0 &&
                !double.IsNaN(this.ActualHeight) && !double.IsInfinity(this.ActualHeight) && this.ActualHeight > 0)
                // To provide the sharp edge the clip is added and "17 is added to include horizontal scroll bar height"
                this.Clip = new System.Windows.Media.RectangleGeometry { Rect = new Rect(0, 0, this.ParentControl.Width, this.ActualHeight + 17 ) };
#endif

            return base.ArrangeOverride(arrangeSize);
        }


        /// <summary>
        /// Adds the grid lines.
        /// </summary>
        private void AddGridLines()
        {
            // Checks for showing the gridlines or not
            if (this.ParentControl.ShowChartLines)
            {
                TimeSpan diffTime = this.EndTime - this.StartTime;
                int count = (diffTime.Days % 7) > 0 ? (diffTime.Days / 7) + 1 : (diffTime.Days / 7);

                DateTime tempTime = this.StartTime;

                int hour = tempTime.Hour == 0 ? 0 : 24 - tempTime.Hour;
                tempTime = tempTime.AddHours(hour);

                int min = tempTime.Minute == 0 ? 0 : 60 - tempTime.Minute;
                tempTime = tempTime.AddMinutes(min);

                while (tempTime.DayOfWeek != this.ParentControl.ParentControl.WeekBeginsOn)
                {
                    int day = tempTime.DayOfWeek < this.ParentControl.ParentControl.WeekBeginsOn ? 1 : -1;
                    tempTime = tempTime.AddDays(day);
                }

#if !SILVERLIGHT
                var normalLineBrush = Brushes.LightGray;
#else
                var normalLineBrush = new SolidColorBrush(Colors.LightGray);
#endif
                for (int i = 0; i <= count; i++)
                {
                    double pos = this.ParentControl.GetStartPositionOfDate(tempTime);
                    Rectangle rect = new Rectangle { VerticalAlignment = System.Windows.VerticalAlignment.Stretch, Tag = pos, Width = 1,Stroke=normalLineBrush,Fill=normalLineBrush };                                  
#if SILVERLIGHT                    
                    rect.Width = 0.01d;
                    rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;               
#endif
                    this.Children.Add(rect);
                    tempTime = tempTime.AddDays(7);
                }

                double currentDatePosition = this.ParentControl.GetStartPositionOfDate(DateTime.Today);
                if (this.StartTime < DateTime.Now && this.EndTime > DateTime.Now && this.ParentControl.StickCurrentDateLineTo != CurentDateLinePositions.None)
                {
                    if (this.ParentControl.StickCurrentDateLineTo != CurentDateLinePositions.Absolute)
                    {
                        if (this.ParentControl.StickCurrentDateLineTo == CurentDateLinePositions.DynamicTime)
                        {
                            currentDatePosition = this.ParentControl.GetStartPositionOfDate(DateTime.Now);
                            this.dynamicTimer.Start();
                        }
                        if (this.ParentControl.StickCurrentDateLineTo == CurentDateLinePositions.LoadedTime)
                        {
                            currentDatePosition = this.ParentControl.GetStartPositionOfDate(DateTime.Now);
                        }
                        this.ParentControl.CurrentDateLine.X1 = currentDatePosition;
                        this.ParentControl.CurrentDateLine.X2 = currentDatePosition;
                    }
                    this.Children.Add(this.ParentControl.CurrentDateLine);
                }
            }
        }


        /// <summary>
        /// Sets the stict current date lineto.
        /// </summary>
        /// <param name="option">The option.</param>
        internal void SetStictCurrentDateLineto(CurentDateLinePositions option)
        {
            //In this method we change the position and behavior of the CurrentDateline. This method will used in Changeing the StictCurrentLineto Dynamically.
           
            double currentDatePosition = this.ParentControl.GetStartPositionOfDate(DateTime.Today);
            dynamicTimer.Stop();
            
            this.ParentControl.CurrentDateLine.Visibility = System.Windows.Visibility.Visible;
            this.ParentControl.StickCurrentDateLineTo = option;
            if (option != CurentDateLinePositions.Absolute)
            {
                if (option == CurentDateLinePositions.None)
                {
#if !SILVERLIGHT
                    this.ParentControl.CurrentDateLine.Visibility = System.Windows.Visibility.Hidden;
#else
                this.ParentControl.CurrentDateLine.Visibility = System.Windows.Visibility.Collapsed;
#endif
                }
                if (option == CurentDateLinePositions.DynamicTime)
                {
                    dynamicTimer.Start();
                    currentDatePosition = this.ParentControl.GetStartPositionOfDate(DateTime.Now);
                }

                if (option == CurentDateLinePositions.LoadedTime)
                {
                    currentDatePosition = this.ParentControl.GetStartPositionOfDate(DateTime.Now);
                }

                this.ParentControl.CurrentDateLine.X1 = currentDatePosition;
                this.ParentControl.CurrentDateLine.X2 = currentDatePosition;
            }
        }

        /// <summary>
        /// Handles the Tick event of the timer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void timer_Tick(object sender, EventArgs e)
        {
            double position = this.ParentControl.GetStartPositionOfDate(DateTime.Now);

            this.ParentControl.CurrentDateLine.X1 = position;
            this.ParentControl.CurrentDateLine.X2 = position;
        }

        /// <summary>
        /// Adds the background cells.
        /// </summary>
        private void AddBackgroundCells()
        {
            //Checks the background cells can be added or not.

            if (this.ParentControl.ShowNonWorkingHoursBackground)
            {
                int cellCount = this.GetCellCount();
                var tempTime = this.StartTime;
                int weekDay = (int)(6 - tempTime.DayOfWeek);

                if (weekDay >= 0 && weekDay < 6)
                    tempTime = tempTime.AddDays(weekDay);
                

                //To display the NonWorkingHours in correct place we round the hour value of the temptime
                int hour = tempTime.Hour == 0 ? 0 : -tempTime.Hour;
                tempTime = tempTime.AddHours(hour);

                int min = tempTime.Minute == 0 ? 0 : -tempTime.Minute;
                tempTime = tempTime.AddMinutes(min);

                while (cellCount > 0)
                {
                    var backgroundCell = GetBackgroundCell(tempTime, TimeUnit.Days, 0);
                    backgroundCell.Tag = this.ParentControl.GetStartPositionOfDate(tempTime);
                    this.Children.Add(backgroundCell);

                    if (tempTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        tempTime = tempTime.AddDays(1);
                    }
                    else
                    {
                        tempTime = tempTime.AddDays(6);
                    }
                    cellCount--;
                }
            }
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="timeUnit">The time unit.</param>
        /// <returns></returns>
        internal double GetWidth(DateTime time, TimeUnit timeUnit)
        {
           return this.ParentControl.ParentControl.GanttSchedule.GetWidth(this.StartTime, time, timeUnit);
           // return 1;
        }

        /// <summary>
        /// Gets the cell count.
        /// </summary>
        /// <returns></returns>
        private int GetCellCount()
        {
            TimeSpan timeDiff = this.EndTime - this.StartTime;
            //return (int)timeDiff.Days % 7 > 0 ? ((int)timeDiff.Days / 7) * 2 : (((int)timeDiff.Days / 7) * 2) + 1;

            // Getting whole week ends Count
            int weekEnds = (int)(timeDiff.Days / 7) * 2;

            // Calculations for finding additional week ends 
            // Gets the day of the startTime
            int startDay = GetDay(this.StartTime.DayOfWeek);

            // Gets the Day of the End time
            int endDay = GetDay(this.EndTime.DayOfWeek);

            // Checking and Adding Additional week end days.
            if (startDay > endDay)
            {
                // Adding the additional Sunday
                weekEnds++;

                // Checking and Adding the additional Saturday 
                weekEnds += (startDay < GetDay(DayOfWeek.Sunday)) ? 1 : 0;
            }
            else if (startDay < endDay)
            {
                // Checking and adding Additional Saturday.
                weekEnds += (endDay == GetDay(DayOfWeek.Sunday)) ? 1 : 0;
            }

            return weekEnds;
        }

        /// <summary>
        /// Gets the dayofWeek's Day.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns></returns>
        private int GetDay(DayOfWeek day)
        {
            // Returning the value of the Sunday.
            // [For Sunday the Default Value is 0, Hence for calculation Purposes we are changing that to 7].
            if (day == DayOfWeek.Sunday)
                return 7;

            // Returning the default value of the other days.
            else
                return (int)day;
        }

        /// <summary>
        /// Gets the background cell.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <param name="scheduleUnit">The ScheduleUnit.</param>
        /// <param name="MergeCells">The MergeCells.</param>
        /// <returns></returns>
        private Rectangle GetBackgroundCell(DateTime time, TimeUnit scheduleUnit, double MergeCells)
        {
            Binding backgroundBinding = new Binding("NonWorkingHoursBackground");
            backgroundBinding.Source = this.ParentControl;          

            Rectangle rect = new Rectangle {VerticalAlignment = System.Windows.VerticalAlignment.Stretch };
            rect.SetBinding(Rectangle.FillProperty, backgroundBinding);         

            if (scheduleUnit == TimeUnit.Days)
                rect.Width = GetWidth(time, TimeUnit.Days);
#if !SILVERLIGHT
            else
                rect.Width = (GetWidth(time, TimeUnit.Hours) * MergeCells); 
#endif
            return rect;
        }

        /// <summary>
        /// Disposes the Events
        /// </summary>
        /// <remarks></remarks>
        ~ GanttChartBackgroundPanel()
        {
#if !SILVERLIGHT
            dynamicTimer.Tick -= timer_Tick;
#endif
        }
    }

#if SILVERLIGHT
    internal static class ColorExtensions
    {
        public static Color StringToColor(string hexaColor)
        {
            var color = Color.FromArgb(Convert.ToByte(hexaColor.Substring(1, 2), 16), Convert.ToByte(hexaColor.Substring(3, 2), 16), Convert.ToByte(hexaColor.Substring(5, 2), 16), Convert.ToByte(hexaColor.Substring(7, 2), 16));
            return color;
        }
    }
#endif
}
