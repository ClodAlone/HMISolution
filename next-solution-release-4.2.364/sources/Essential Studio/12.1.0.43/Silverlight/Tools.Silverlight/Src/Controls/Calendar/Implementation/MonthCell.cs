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
    /// Represents the Month Cell Class.
    /// </summary>
    public class MonthCell : Cell
    {
        /// <summary>
        /// Identifies <see cref="MonthNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MonthNumberProperty =
            DependencyProperty.Register("MonthNumber", typeof(int), typeof(MonthCell), new PropertyMetadata(0));

        /// <summary>
        /// Gets or sets sequential number of the month in the year.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type : <see cref="int"/>
        /// </value>
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
                if ((value >= 1 && value <= 12) || value == -1)
                {
                    SetValue(MonthNumberProperty, value);
                }
                else
                {
                    throw new ArgumentException("MonthNumber must be in the range 1..12");
                }
            }
        }
    }
}