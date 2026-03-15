// <copyright file="WeekNumberCellPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a week number cell of the <see cref="Syncfusion.Windows.Shared.CalendarEdit"/> control.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class WeekNumberCellPanel : Cell
    {
        #region Dependency Properties

        /// <summary>
        /// Identifies <see cref="WeekNumber"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WeekNumberProperty = DependencyProperty.Register("WeekNumber", typeof(string), typeof(WeekNumberCellPanel));

        #endregion Dependency Properties

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="WeekNumberCellPanel"/> class.  It overrides some dependency properties.
        /// </summary>
        static WeekNumberCellPanel()
        {
            // This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            // This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WeekNumberCellPanel), new FrameworkPropertyMetadata(typeof(WeekNumberCellPanel)));
            ContentControl.FocusableProperty.OverrideMetadata(typeof(WeekNumberCellPanel), new FrameworkPropertyMetadata(false));
            Border.CornerRadiusProperty.AddOwner(typeof(WeekNumberCellPanel));
        }

        #endregion Initialization

        #region DP getters and setters

        /// <summary>
        /// Gets or sets WeekNumberProperty.
        /// This is a dependency property.
        /// </summary>
        /// <value>
        /// Type: <see cref="string"/>
        /// </value>
        /// <seealso cref="string"/>
        public string WeekNumber
        {
            get
            {
                return (string)GetValue(WeekNumberProperty);
            }

            set
            {
                SetValue(WeekNumberProperty, value);
            }
        }

        #endregion DP getters and setters
    }
}