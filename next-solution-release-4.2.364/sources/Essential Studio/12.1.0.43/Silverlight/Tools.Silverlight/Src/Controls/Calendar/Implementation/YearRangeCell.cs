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
    /// Represents the Year Range Cell Class.
    /// </summary>
    public class YearRangeCell : Cell
    {
        /// <summary>
        /// Identifies <see cref="IsBelongToCurrentRange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBelongToCurrentRangeProperty =
            DependencyProperty.Register("IsBelongToCurrentRange", typeof(bool), typeof(YearRangeCell), new PropertyMetadata(false));

        /// <summary>
        /// Identifies <see cref="Years"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YearsProperty =
            DependencyProperty.Register("Years", typeof(YearsRange), typeof(YearRangeCell), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value indicating whether the cell 
        /// belongs to the current range.
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
        /// Gets or sets the years range.
        /// </summary>
        /// <value>
        /// Type: <see cref="YearsRange"/>
        /// </value>
        /// <seealso cref="YearsRange"/>
        public YearsRange Years
        {
            get
            {
                return (YearsRange)GetValue(YearsProperty);
            }

            set
            {
                SetValue(YearsProperty, value);
            }
        }
    }
}
