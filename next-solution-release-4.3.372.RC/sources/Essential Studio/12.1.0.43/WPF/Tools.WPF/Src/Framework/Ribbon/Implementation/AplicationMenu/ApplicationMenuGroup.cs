
// <copyright file="ApplicationMenuGroup.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Syncfusion.Windows.Shared;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents ApplicationMenuGroup control.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ApplicationMenuGroup : RibbonMenuGroup
    {
        #region Implementation
        /// <summary>
        /// Invoked whenever application code or internal processes call
        /// ApplyTemplate.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            FrameworkElement parent = (FrameworkElement)Parent;
            FrameworkElement grandParent = (FrameworkElement)parent.Parent;
            if (grandParent != null && !(grandParent is ApplicationMenu))
            {
                (GetVisualChild(0) as LayoutPanel).Width = double.NaN;
            }
        }
        #endregion
    }
}
