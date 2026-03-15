// <copyright file="ToggleButtonExt.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class overrides metadata for <see cref="ToggleButton"/> class.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ToggleButtonExt : ToggleButton
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="ToggleButtonExt"/> class.
        /// </summary>
        static ToggleButtonExt()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToggleButtonExt), new FrameworkPropertyMetadata(typeof(ToggleButtonExt)));
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.Initialized"/> event. This method is invoked whenever <see cref="P:System.Windows.FrameworkElement.IsInitialized"/> is set to true internally.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.RoutedEventArgs"/> that contains the event data.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.Loaded += new RoutedEventHandler(ToggleButtonExt_Loaded);
        }

        /// <summary>
        /// Handles the Loaded event of the ToggleButtonExt control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ToggleButtonExt_Loaded(object sender, RoutedEventArgs e)
        {
            if (!TaskBar.GetIsOpened(this))
            {
                base.RaiseEvent(new RoutedEventArgs(ToggleButtonExt.UncheckedEvent, this));
            }
        }

        #endregion Initialization
    }
}