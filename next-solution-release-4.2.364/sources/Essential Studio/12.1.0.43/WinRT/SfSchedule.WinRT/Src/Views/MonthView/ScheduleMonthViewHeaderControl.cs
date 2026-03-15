#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;
using System.Windows.Controls;
#endif

namespace Syncfusion.UI.Xaml.Schedule
{
    /// <summary>
    /// Represents a month view header.
    /// </summary>
    public class ScheduleMonthViewHeaderControl : Control
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see
        /// cref="T:Syncfusion.UI.Xaml.Schedule.ScheduleMonthViewHeaderControl">ScheduleMonthViewHeaderControl</see>
        /// class.
        /// </summary>
        public ScheduleMonthViewHeaderControl()
        {
            DefaultStyleKey = typeof(ScheduleMonthViewHeaderControl);
        }

        #endregion

        #region Dependency properties

        #region DayText
        /// <summary>
        /// Gets or sets the month view header.
        /// </summary>
        public string DayText
        {
            get { return (string)GetValue(DayTextProperty); }
            set { SetValue(DayTextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayText.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayTextProperty =
            DependencyProperty.Register("DayText", typeof(string), typeof(ScheduleMonthViewHeaderControl), new PropertyMetadata(string.Empty));
        #endregion

        #region DayOfWeek
        /// <summary>
        /// Gets or sets the day of week header in month view.
        /// </summary>
        public string DayOfWeek
        {
            get { return (string)GetValue(DayOfWeekProperty); }
            set { SetValue(DayOfWeekProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DayOfWeek.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DayOfWeekProperty = 
            DependencyProperty.Register("DayOfWeek", typeof(string), typeof(ScheduleMonthViewHeaderControl), new PropertyMetadata(string.Empty));
        #endregion

        #endregion
    }
}