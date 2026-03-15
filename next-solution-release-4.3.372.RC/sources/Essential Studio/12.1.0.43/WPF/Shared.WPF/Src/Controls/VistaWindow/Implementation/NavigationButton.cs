// <copyright file="NavigationButton.cs" company="Syncfusion">
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
using System.Threading;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents button for navigating.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class NavigationButton : Button
    {
        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="NavigationButton"/> class.
        /// </summary>
        static NavigationButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NavigationButton), new FrameworkPropertyMetadata(typeof(NavigationButton)));
        }
        #endregion

        #region DP Getters and setters

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        /// <value>The navigation button icon.</value>
        public object Icon
        {
            get
            {
                return (object)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }
        #endregion

        #region Dependency properties
        /// <summary>
        /// Identifies <see cref="Icon"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(NavigationButton), new UIPropertyMetadata(null));
        #endregion
    }
}
