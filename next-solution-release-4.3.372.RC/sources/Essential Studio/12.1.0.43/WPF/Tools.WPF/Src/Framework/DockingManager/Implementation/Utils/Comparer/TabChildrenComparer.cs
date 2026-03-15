// <copyright file="TabChildrenComparer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// This class is used to compare tab children.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class TabChildrenComparer : ComparerBase
    {
        #region Implementation
        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>return index.</returns>
        protected override int GetIndex(FrameworkElement element)
        {
            int result = -1;
            DockState state = DockingManager.GetState(element);

            if (state == DockState.Dock)
            {
                result = DockedElementTabbedHost.GetTabOrderInDockMode(element);
            }
            else if (state == DockState.Float)
            {
                result = DockedElementTabbedHost.GetTabOrderInFloatMode(element);
            }

            return result;
        }

        /// <summary>
        /// Validates the elements.
        /// </summary>
        /// <param name="element1">The element1.</param>
        /// <param name="element2">The element2.</param>
        protected override void ValidateElements(FrameworkElement element1, FrameworkElement element2)
        {
            base.ValidateElements(element1, element2);

            DockState stateX = DockingManager.GetState(element1);
            DockState stateY = DockingManager.GetState(element2);

            TabControlExt tab1element1 = null,tab1element2=null;
            TabControl tab2element1 = null, tab2element2=null;

            if (DockingManager.GetTabControl(element1 as DependencyObject) != null)
            {
                tab1element1 = DockingManager.GetTabControl(element1 as DependencyObject);
            }

            if (tab1element1 == null)
            {
                if (DockingManager.GetTabControl(element1) != null)
                {
                    tab2element1 = DockingManager.GetTabControl(element1);
                }
            }

            if (DockingManager.GetTabControl(element2 as DependencyObject) != null)
            {
                tab1element2 = DockingManager.GetTabControl(element2 as DependencyObject);
            }

            if (tab1element2 == null)
            {
                if (DockingManager.GetTabControl(element2) != null)
                {
                    tab2element2 = DockingManager.GetTabControl(element2);
                }
            }

            if (tab1element1 != null && tab1element2 != null)
            {
                if (stateX != stateY)
                {
                    throw new ArgumentException("TabChildrenComparer: Incorrect DockState!");
                }
            }
            if (tab2element1 != null && tab2element2 != null)
            {
                if (stateX != stateY)
                {
                    throw new ArgumentException("TabChildrenComparer: Incorrect DockState!");
                }
            }
        }
        #endregion
    }
}