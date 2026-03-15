// <copyright file="MonthCell.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a month cell of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class MonthCell : Cell
    {
        #region Dependency Property

        /// <summary>
        /// Identifies <see cref="MonthNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthNumberProperty = DependencyProperty.Register("MonthNumber", typeof(int), typeof(MonthCell));

        #endregion Dependency Property

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="MonthCell"/> class.  It overrides some dependency properties.
        /// </summary>
        static MonthCell()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MonthCell), new FrameworkPropertyMetadata(typeof(MonthCell)));
        }

        #endregion Initialization

        #region Dependency properties

        /// <summary>
        /// Gets or sets sequential number of the month in the year.
        /// This is a dependency property.
        /// </summary>
        /// <value>Type: <see cref="int"/>Integer.</value>
        /// <exception cref="ArgumentException">MonthNumber must be in the range 1..12.</exception>
        /// <seealso cref="int"/>
        public int MonthNumber
        {
            get
            {
                return (int)GetValue(MonthNumberProperty);
            }

            set
            {
                if ((value >= 1 && value <= 12) || (value == -1))
                {
                    SetValue(MonthNumberProperty, value);
                }
                else
                {
                    throw new ArgumentException("MonthNumber must be in the range 1..12");
                }
            }
        }

        #endregion Dependency properties
    }
}