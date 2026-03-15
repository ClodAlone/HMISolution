// <copyright file="RibbonSeparator.cs" company="Syncfusion">
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents a control that is used to separate items in items controls.  
    /// </summary>
    /// <remarks>
    /// A RibbonSeparator control draws a vertical line between items in RibbonBar control. 
    /// RibbonSeparator control do not react to any keyboard, mouse, mouse wheel, or tablet input and cannot be enabled 
    /// or selected.
    /// </remarks>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class RibbonSeparator : System.Windows.Controls.Control
    {
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="RibbonSeparator"/> class.
        /// </summary>
        static RibbonSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonSeparator), new FrameworkPropertyMetadata(typeof(RibbonSeparator)));
        }
        #endregion
    }
}
