// <copyright file="CornerPanel.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents the class for the corner panel
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CornerPanel : Control
    {
        #region Initialization
        /// <summary>
        /// Initializes static members of the <see cref="CornerPanel"/> class.
        /// </summary>
        static CornerPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CornerPanel), new FrameworkPropertyMetadata(typeof(CornerPanel)));
        }
        #endregion
    }
}
