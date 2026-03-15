// <copyright file="YearRangeCell.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a year range cell of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class YearRangeCell : Cell
    {
        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="Years"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YearsProperty = DependencyProperty.Register("Years", typeof(YearsRange), typeof(YearRangeCell));

        /// <summary>
        /// Identifies <see cref="IsBelongToCurrentRange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBelongToCurrentRangeProperty = DependencyProperty.Register("IsBelongToCurrentRange", typeof(bool), typeof(YearRangeCell));

        #endregion Dependency properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="YearRangeCell"/> class.  It overrides some dependency properties.
        /// </summary>
        static YearRangeCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YearRangeCell), new FrameworkPropertyMetadata(typeof(YearRangeCell)));
        }

        #endregion Initialization

        #region Properties

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

        #endregion Properties
    }
}