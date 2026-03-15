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
    /// Stores the TileViewItemState of the Tile view item
    /// </summary>
    public enum TileViewItemState
    {
        /// <summary>
        /// Normal state of the report card 
        /// </summary>
        Normal,

        /// <summary>
        /// Maximized state of the report card 
        /// </summary>
        Maximized,

        /// <summary>
        /// Minimized state of the report card 
        /// </summary>
        Minimized, Hidden
    }

    public enum CloseMode
    { 
    Hide, Delete}
}
