#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Syncfusion.Windows.Shared
{
    /// <summary>
    /// Stores the state of the Tile view item
    /// </summary>
    public enum TileViewItemState
    {
        /// <summary>
        /// Normal state of the report card / TileViewItem
        /// </summary>
        Normal,

        /// <summary>
        /// Maximized state of the report card / TileViewItem
        /// </summary>
        Maximized,

        /// <summary>
        /// Minimized state of the report card / TileViewItem
        /// </summary>
        Minimized,

        /// <summary>
        /// Hidden state of the report card / TileViewItem
        /// </summary>
        Hidden
    }
    /// <summary>
    /// Stores the CloseMode of the TileViewItem.
    /// </summary>
    public enum CloseMode
    {
        /// <summary>
        /// Hide Mode of the TileViewItem
        /// </summary>
        Hide,

        /// <summary>
        /// Delete Mode of the TileViewItem
        /// </summary>
        Delete
    }
}
