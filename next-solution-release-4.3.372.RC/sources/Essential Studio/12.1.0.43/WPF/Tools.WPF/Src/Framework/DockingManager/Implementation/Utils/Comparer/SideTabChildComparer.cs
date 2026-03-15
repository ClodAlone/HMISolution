// <copyright file="SideTabChildComparer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is used to compare side tab children.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class SideTabChildComparer : ComparerBase
    {
        #region Implementation
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return index.</returns>
        protected override int GetIndex(FrameworkElement element)
        {
            return SidePanel.GetTabChildOrder(element);
        }
        #endregion
    }
}
