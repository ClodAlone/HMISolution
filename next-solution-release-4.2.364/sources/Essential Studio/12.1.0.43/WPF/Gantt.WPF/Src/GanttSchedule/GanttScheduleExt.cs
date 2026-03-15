#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.Gantt.Schedule
{
    /// <summary>
    /// Class that handles the basic calculations of Schedule.
    /// </summary>
    internal static class GanttScheduleExt
    {
        #region DateTime Calculations

        /// <summary>
        /// Converts the given time to pixels.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="currentTime">The current time.</param>
        /// <param name="timeUnit">The time unit.</param>
        /// <returns></returns>
        internal static double ConvertToPixels(this GanttSchedule schedule, DateTime currentTime, TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Days:
                    return schedule.DayWidth;
                default:
                    return schedule.DayWidth;
            }

        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="currentTime">The current time.</param>
        /// <param name="time">The time.</param>
        /// <param name="timeUnit">The time unit.</param>
        /// <returns></returns>
        internal static double GetWidth(this GanttSchedule schedule, DateTime currentTime, DateTime time, TimeUnit timeUnit)
        {
            double result = ConvertToPixels(schedule, time, timeUnit);

#if !SILVERLIGHT
            switch (timeUnit)
            {
                case TimeUnit.Hours:
                    result /= 24;
                    break;
            }
#endif
            return result;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="targetTime">The target time.</param>
        /// <returns></returns>
        internal static double GetPosition(this GanttSchedule schedule, DateTime targetTime)
        {
            // var ts = (double)targetTime.Subtract(currentTime).TotalDays;
            var ts = (double)targetTime.Subtract(schedule.StartTime).TotalDays;

            var unitWidth = ConvertToPixels(schedule, targetTime, schedule.LowerTimeUnit);

            var result = (unitWidth * ts);
            return result;

        }
#if !SILVERLIGHT
        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="targetTime">The target time.</param>
        /// <param name="scheduleUnit">The schedule unit.</param>
        /// <returns></returns>
        internal static double GetPosition(this GanttSchedule schedule, DateTime targetTime, TimeUnit scheduleUnit)
        {
            // var ts = (targetTime - currentTime);
            var ts = (targetTime - schedule.StartTime);

            var unitWidth = ConvertToPixels(schedule, targetTime, scheduleUnit);

            double result = 0d;

            switch (scheduleUnit)
            {
                case TimeUnit.Hours:
                    result = (unitWidth / 24);
                    return result * ts.TotalHours;

            }
            return -1;
        }
#endif
        /// <summary>
        /// Converts the Positions to date.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="dt">The dt.</param>
        /// <param name="wt">The wt.</param>
        /// <returns></returns>
        internal static DateTime PositionToDate(this GanttSchedule schedule, DateTime dt, double wt)
        {
            DateTime newDate = new DateTime();
            double totalDays = wt / schedule.DayWidth;
            double value = double.IsNaN(totalDays) ? 0 : totalDays;
            newDate = dt.AddDays(value);

            return newDate;
        }

        #endregion

        #region Numeric Calculations

        /// <summary>
        /// Converts the given point to pixels.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="currentPoint">The current point.</param>
        /// <returns></returns>
        internal static double ConvertPointToPixels(this GanttSchedule schedule, double currentPoint)
        {
            return schedule.LowerCellWidth;
        }

        /// <summary>
        /// Gets the point of position.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="endPoint">The end point.</param>
        /// <returns></returns>
        internal static double GetPointPosition(this GanttSchedule schedule, double endPoint)
        {
            // double var = endPoint - startPoint;
            double var = endPoint - schedule.Start;
            double lowerunit = ConvertPointToPixels(schedule, endPoint);
            return (lowerunit * var);
        }

        /// <summary>
        /// Converts Positions to point.
        /// </summary>
        /// <param name="schedule">The schedule.</param>
        /// <param name="point">The point.</param>
        /// <param name="width">The width.</param>
        /// <returns></returns>
        internal static double PositionToPoint(this GanttSchedule schedule, double point, double width)
        {
            double newpoint = 0d;
            double totaldistance = width / schedule.LowerCellWidth;
            newpoint = Math.Round(point + totaldistance, 2);
            return newpoint;
        }

        #endregion
    }
}
