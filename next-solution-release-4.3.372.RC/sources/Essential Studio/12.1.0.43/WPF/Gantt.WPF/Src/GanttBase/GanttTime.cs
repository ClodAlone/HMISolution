#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Syncfusion.Windows.Controls.Gantt
{
    /// <summary>
    /// Class used to pass the time to Gantt.
    /// </summary>
    public class GanttTime
    {
        /// <summary>
        /// Gets or sets the hour.
        /// </summary>
        /// <value>The hour.</value>
        public int Hour { get; set; }

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        /// <value>The minutes.</value>
        public int Minutes { get; set; }

        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        /// <value>The seconds.</value>
        public int Seconds { get; set; }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time.</value>
        internal TimeSpan Time
        {
            get
            {
                return new TimeSpan(Hour, Minutes, Seconds);
            }
        }

        /// <summary>
        /// Gets the default start time.
        /// </summary>
        /// <value>The default start time.</value>
        static internal GanttTime DefaultStartTime
        {
            get
            {
                return new GanttTime { Hour = 9, Minutes = 0, Seconds = 0 };
            }
        }

        /// <summary>
        /// Gets the default end time.
        /// </summary>
        /// <value>The default end time.</value>
        static internal GanttTime DefaultEndTime
        {
            get
            {
                return new GanttTime { Hour = 18, Minutes = 0, Seconds = 0 };
            }
        }
    }
}
