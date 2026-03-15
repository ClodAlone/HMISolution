// <copyright file="IRibbonControl.cs" company="Syncfusion">
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
using System.Windows.Media;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents basic interface for Ribbon controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface IRibbonControl
    {
        /// <summary>
        /// Gets the value of the Label property.
        /// </summary>
        string Label
        {
            get;
        }
        
        /// <summary>
        /// Gets the value of the Image property.
        /// </summary>
        ImageSource SmallIcon
        {
            get;
        }
        
        /// <summary>
        /// Gets the value of the ToolTip property.
        /// </summary>
        object ToolTip
        {
            get;
        }
    }
}
