// <copyright file="PopupSidePanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class PopupSidePanel : Popup
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="OpacityDockPanel"/> class.
        /// </summary>
        static PopupSidePanel()
        {
            OpacityProperty.OverrideMetadata(typeof(PopupSidePanel), new FrameworkPropertyMetadata(1d, new PropertyChangedCallback(OnOpacityChanged)));
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Called when [opacity changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PopupSidePanel instance = (PopupSidePanel)d;
            double newOpacity = (double)e.NewValue;

            if (0 == newOpacity)
            {
                instance.Visibility = Visibility.Collapsed;
            }
            else if (Visibility.Collapsed == instance.Visibility)
            {
                instance.Visibility = Visibility.Visible;
            }
        }
        #endregion
    }
}
