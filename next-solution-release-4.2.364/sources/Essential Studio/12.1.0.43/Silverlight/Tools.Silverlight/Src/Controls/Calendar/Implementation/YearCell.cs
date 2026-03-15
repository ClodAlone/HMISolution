#region Copyright
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the Year Cell Class.
    /// </summary>
    public class YearCell : Cell
    {
        /// <summary>
        /// Identifies <see cref="IsBelongToCurrentRange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBelongToCurrentRangeProperty =
            DependencyProperty.Register("IsBelongToCurrentRange", typeof(bool), typeof(YearCell), new PropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="MonthNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthNumberProperty =
           DependencyProperty.Register("MonthNumber", typeof(int), typeof(MonthCell), new PropertyMetadata(0));

        /// <summary>
        /// Identifies <see cref="Year"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YearProperty =
            DependencyProperty.Register("Year", typeof(int), typeof(YearCell), new PropertyMetadata(1));

        /// <summary>
        /// Gets or sets a value indicating whether 
        /// the cell belongs to the current range.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsBelongToCurrentRange
        {
            get
            {
                return (bool)GetValue(IsBelongToCurrentRangeProperty);
            }

            set
            {
                SetValue(IsBelongToCurrentRangeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the month number.
        /// </summary>
        /// <value>The month number.</value>
        public int MonthNumber
        {
            get
            {
                return (int)GetValue(MonthNumberProperty);
            }

            set
            {
                if ((value >= 1 && value <= 12) || value == -1)
                {
                    SetValue(MonthNumberProperty, value);
                }
                else
                {
                    throw new ArgumentException("YearCell must be in the range 1..12");
                }
            }
        }

        /// <summary>
        /// Gets or sets the year.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type : <see cref="int"/>
        /// </value>
        /// <seealso cref="int"/>
        public int Year
        {
            get
            {
                return (int)GetValue(YearProperty);
            }

            set
            {
                SetValue(YearProperty, value);
            }
        }
    }
}

