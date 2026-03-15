// <copyright file="ItemsControlInternal.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Represents a control that can be used to present a collection of items.
    /// </summary>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class ItemsControlInternal : ItemsControl
    {
        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is (or is eligible to be) its own container; otherwise, false</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ItemsControlInternalItem;
        }

        /// <summary>
        /// Creates or identifies the <see cref="ItemsControlInternalItem"/> instance that is used to display the given item.
        /// </summary>
        /// <returns>The <see cref="ItemsControlInternalItem"/> instance that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ItemsControlInternalItem();
        }
    }
}
