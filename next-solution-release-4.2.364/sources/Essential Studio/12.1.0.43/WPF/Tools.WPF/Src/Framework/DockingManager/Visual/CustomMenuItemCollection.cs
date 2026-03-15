// <copyright file="CustomMenuItemCollection.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Collections.Generic;
using System.Windows.Controls;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Represents collection of a custom menu items.
    /// </summary>
    /// <exclude/>
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public class CustomMenuItemCollection : List<CustomMenuItem>
    {
    }

    /// <summary>
    /// Represents collection of menu items
    /// </summary>
    public class DocumentTabItemMenuItemCollection : List<MenuItem>
    {
    }
}
