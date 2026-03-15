// <copyright file="ICollapsable.cs" company="Syncfusion">
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
    /// Represents interface for collapsible Ribbon controls.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public interface ICollapsable : IRibbonControl
    {
        /// <summary>
        /// Gets or sets the size form.
        /// </summary>
        /// <value>The size form.</value>
        SizeForm SizeForm
        {
            get;
            set;
        }
        
        /// <summary>
        /// Gets control's large icon.
        /// </summary>
        ImageSource LargeIcon
        {
            get;
        }
    }
}
