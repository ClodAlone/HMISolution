// <copyright file="WindowTitleBarButton.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents window maximize, Minimize or close button.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class VistaWindowTitleBarButton : Button
    {
        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="VistaWindowTitleBarButton"/> class.
        /// </summary>
        static VistaWindowTitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(VistaWindowTitleBarButton), new FrameworkPropertyMetadata(typeof(VistaWindowTitleBarButton)));
        }
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets a value indicating whether this instance is close button.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is close button; otherwise, <c>false</c>.
        /// </value>
        public bool IsCloseButton
        {
            get
            {
                return (bool)GetValue(IsCloseButtonProperty);
            }

            internal set
            {
                SetValue(IsCloseButtonProperty, value);
            }
        }
        #endregion

        #region Depeandancy Properties
        /// <summary>
        /// Identifies <see cref="IsCloseButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCloseButtonProperty =
            DependencyProperty.Register("IsCloseButton", typeof(bool), typeof(VistaWindowTitleBarButton), new UIPropertyMetadata(false));
        #endregion
    }
}
