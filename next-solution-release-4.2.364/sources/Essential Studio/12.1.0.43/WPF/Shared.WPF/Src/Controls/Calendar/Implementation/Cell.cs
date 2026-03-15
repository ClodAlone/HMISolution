// <copyright file="Cell.cs" company="Syncfusion">
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
    /// Implements the basic functionality required by the cell.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public abstract class Cell : ContentControl
    {
        #region Dependency Property

        /// <summary>
        /// Identifies <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(Cell), new UIPropertyMetadata(false));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IsInvalidDateProperty =
           DependencyProperty.Register("IsInvalidDate", typeof(bool), typeof(Cell), new UIPropertyMetadata(false));

        #endregion Dependency Property

        #region Private members

        /// <summary>
        /// Corner radius for the cell.
        /// </summary>
        private CornerRadius mcornerRadius;

        #endregion Private members

        #region Initialization

        /// <summary>
        /// Initializes static members of the Cell class.  It overrides some dependency properties.
        /// </summary>
        static Cell()
        {
            ContentControl.FocusableProperty.OverrideMetadata(typeof(Cell), new FrameworkPropertyMetadata(false));
            Border.CornerRadiusProperty.AddOwner(typeof(Cell));
        }

        #endregion Initialization

        #region Properies

        /// <summary>
        /// Gets or sets corner radius for the cell.
        /// </summary>
        /// <value>
        /// Type: <see cref="CornerRadius"/>
        /// </value>
        /// <seealso cref="CornerRadius"/>
        public CornerRadius CornerRadius
        {
            get
            {
                return this.mcornerRadius;
            }

            set
            {
                this.mcornerRadius = value;
            }
        }

        #endregion Properies

        #region Dependency Property

        /// <summary>
        /// Gets or sets a value indicating whether cell is selected.
        /// </summary>
        /// <value>
        /// Type: <see cref="bool"/>
        /// Default value is false.
        /// </value>
        /// <seealso cref="bool"/>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }

            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsInvalidDate
        {
            get
            {
                return (bool)GetValue(IsInvalidDateProperty);
            }
            set
            {
                SetValue(IsInvalidDateProperty, value);
            }
        }

        #endregion Dependency Property
    }
}