#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System.Windows;
using System.Windows.Controls;
using Syncfusion.Licensing;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// class which has the Gradient items collection
    /// </summary>

#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif

    internal class GradientItemCollection : ItemsControl
    {
        /// <summary>
        /// Gets or sets the gradient item.
        /// </summary>
        /// <value>The gradient item.</value>
        internal GradientStopItem gradientItem { get; set; }

        /// <summary>
        /// Initializes the <see cref="GradientItemCollection"/> class.
        /// </summary>
        static GradientItemCollection()
        {
            EnvironmentTest.ValidateLicense(typeof(GradientItemCollection));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GradientItemCollection), new FrameworkPropertyMetadata(typeof(GradientItemCollection)));
        }
    }
}