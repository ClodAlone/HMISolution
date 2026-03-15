// <copyright file="IItemContainer.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents IItemContainer interface.
    /// </summary>
#if SyncfusionFramework4_0

    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    internal interface IItemContainer
    {
        /// <summary>
        /// Gets the item.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Tree View Item Adv</returns>
        TreeViewItemAdv GetItem(int index);

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <param name="obj">The obj of index.</param>
        /// <returns> int type for index</returns>
        int GetIndex(object obj);

        /// <summary>
        /// Gets the first visible item.
        /// </summary>
        /// <returns>TreeViewItemAdv first visibility</returns>
        TreeViewItemAdv GetFirstVisibleItem();

        /// <summary>
        /// Gets the last visible item.
        /// </summary>
        /// <returns>Last Visible Item</returns>
        TreeViewItemAdv GetLastVisibleItem();
    }
}