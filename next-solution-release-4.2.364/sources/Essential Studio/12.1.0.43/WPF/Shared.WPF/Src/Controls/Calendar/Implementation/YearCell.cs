// <copyright file="YearCell.cs" company="Syncfusion">
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
    /// Represents a year cell of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class YearCell : Cell
    {
        #region Dependency properties

        /// <summary>
        /// Identifies <see cref="Year"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YearProperty = DependencyProperty.Register("Year", typeof(int), typeof(YearCell));

        /// <summary>
        /// Identifies <see cref="IsBelongToCurrentRange"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsBelongToCurrentRangeProperty = DependencyProperty.Register("IsBelongToCurrentRange", typeof(bool), typeof(YearCell));

        #endregion Dependency properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="YearCell"/> class.  It overrides some dependency properties.
        /// </summary>
        static YearCell()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(YearCell), new FrameworkPropertyMetadata(typeof(YearCell)));
        }

        #endregion Initialization

        #region Properties

        /// <summary>
        /// Gets or sets the year.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Syncfusion.Windows.Shared.CalendarEdit">Integer.</see>
        /// </value>
        /// <seealso cref="Syncfusion.Windows.Shared.CalendarEdit">Integer.</seealso>
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

        /// <summary>
        /// Gets or sets a value indicating whether
        /// the cell belongs to the current range.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="Syncfusion.Windows.Shared.YearCell.IsBelongToCurrentRange">Boolean.</see>
        /// </value>
        /// <seealso cref="Syncfusion.Windows.Shared.YearCell">Boolean.</seealso>
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