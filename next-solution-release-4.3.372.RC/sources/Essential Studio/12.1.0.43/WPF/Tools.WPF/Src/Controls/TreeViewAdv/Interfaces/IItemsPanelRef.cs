// <copyright file="IItemsPanelRef.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents IItemsPanelRef interface
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal interface IItemsPanelRef
    {
        /// <summary>
        /// Gets the items panel.
        /// </summary>
        /// <value>The items panel.</value>
        TreeViewAdvItemsPanel ItemsPanel
        {
            get;
        }
    }
}