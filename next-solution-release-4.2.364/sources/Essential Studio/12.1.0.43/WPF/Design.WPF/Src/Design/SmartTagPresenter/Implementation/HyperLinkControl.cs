// <copyright file="HyperLinkControl.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Syncfusion.Windows.Design
{
    /// <summary>
    /// HyperLinkControl Help us to provide Uri reference for control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class HyperLinkControl : ContentControl
    {
        #region Dependency Property

        /// <summary>
        /// Uri property is defined
        /// </summary>
        public static readonly DependencyProperty UriProperty =
           DependencyProperty.Register("Uri", typeof(Uri), typeof(HyperLinkControl), new FrameworkPropertyMetadata(null));

        #endregion
        
        #region Properties

        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI property.</value>
        public Uri Uri
        {
            get
            {
                return (Uri)GetValue(UriProperty);
            }

            set
            {
                SetValue(UriProperty, value);
            }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes static members of the <see cref="HyperLinkControl"/> class.
        /// </summary>
        static HyperLinkControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HyperLinkControl), new FrameworkPropertyMetadata(typeof(HyperLinkControl)));
        }

        #endregion

        #region Implementation

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.PreviewMouseDown"/> attached routed event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseButtonEventArgs"/> that contains the event data. The event data reports that one or more mouse buttons were pressed.</param>
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            Process.Start(new ProcessStartInfo(Uri.AbsoluteUri));
            e.Handled = true;
            base.OnPreviewMouseDown(e);
        }

        #endregion
    }
}
