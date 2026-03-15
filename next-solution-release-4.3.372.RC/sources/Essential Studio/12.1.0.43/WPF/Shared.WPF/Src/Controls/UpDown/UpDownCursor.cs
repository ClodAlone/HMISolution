// <copyright file="UpDownCursor.cs" company="Syncfusion">
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
    /// Responsible for cursor visibility.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class UpDownCursor : Control
    {
        /// <summary>
        /// Initializes static members of the <see cref="UpDownCursor"/> class.
        /// </summary>
        static UpDownCursor()
        {
            //// This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //// This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UpDownCursor), new FrameworkPropertyMetadata(typeof(UpDownCursor)));
        }
    }
}