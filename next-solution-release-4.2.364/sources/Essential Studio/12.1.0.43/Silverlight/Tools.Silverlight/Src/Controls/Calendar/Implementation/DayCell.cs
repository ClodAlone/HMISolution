#region Copyright
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

namespace Syncfusion.Windows.Tools.Controls
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
    /// When Day displaying this class calls.
    /// </summary>
    public class DayCell : Cell
    {
        /// <summary>
        /// Identifies <see cref="Date"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DateProperty =
            DependencyProperty.Register("Date", typeof(Date), typeof(DayCell), new PropertyMetadata(new Date()));

        /// <summary>
        /// Identifies <see cref="IsCurrentMonth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCurrentMonthProperty =
            DependencyProperty.Register("IsCurrentMonth", typeof(bool), typeof(DayCell), new PropertyMetadata(true));

        /// <summary>
        /// Identifies <see cref="IsDate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDateProperty =
            DependencyProperty.Register("IsDate", typeof(bool), typeof(DayCell), new PropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="IsToday"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTodayProperty =
            DependencyProperty.Register("IsToday", typeof(bool), typeof(DayCell), new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets the cell date.
        /// </summary>
        /// <value>
        /// Type: <see cref="Date"/>
        /// </value>
        /// <seealso cref="Date"/>
        public Date Date
        {
            get
            {
                return (Date)GetValue(DateProperty);
            }

            set
            {
                SetValue(DateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell date 
        /// belongs to the current month. This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is true.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsCurrentMonth
        {
            get
            {
                return (bool)GetValue(IsCurrentMonthProperty);
            }

            set
            {
                SetValue(IsCurrentMonthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell date is equal to 
        /// <see cref="Syncfusion.Windows.Tools.Controls.CalendarControl"/> date.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsDate
        {
            get
            {
                return (bool)GetValue(IsDateProperty);
            }

            set
            {
                SetValue(IsDateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cell date 
        /// is equal to today date.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsToday
        {
            get
            {
                return (bool)GetValue(IsTodayProperty);
            }

            set
            {
                SetValue(IsTodayProperty, value);
            }
        }
    }
}