// <copyright file="TileViewItemState.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if !WINDOWS_PHONE_7
using System.Threading.Tasks;
#endif
#if WINDOWS_PHONE||WINDOWS_PHONE_7
namespace Syncfusion.WP.Controls.Layout
#else
namespace Syncfusion.UI.Xaml.Controls.Layout
#endif
{
    /// <summary>
    /// Specifies the state of the <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
    /// </summary>
    /// <remarks>
    /// State are Normal,Maximized.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public enum TileViewItemState
    {
        /// <summary>
        /// The TileViewItems are in normal state
        /// </summary>
        Normal,

        /// <summary>
        /// The TileViewItems are in maximized state
        /// </summary>
        Maximized
    }

    /// <summary>
    /// specify the dimensions by which <see
    /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>&apos;s are stacked
    /// </summary>
    /// <remarks>
    /// Orientations are Top,Bottom,Right,Left.
    /// </remarks>
    [ClassReference(IsReviewed = false)]
    public enum MinimizedItemsOrientation
    {
        /// <summary>
        /// Position of the minimized TileViewItems in top of the maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        Top,

        /// <summary>
        /// Position of the minimized TileViewItems in Bottom of the maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        Bottom,

        /// <summary>
        /// position of the minimized TileViewItems in Right side of the maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        Right,

        /// <summary>
        /// position of the minimized TileViewItems in Left side of the maximized <see
        /// cref="T:Syncfusion.UI.Xaml.Controls.Layout.TileViewItem"/>.
        /// </summary>
        Left

    }

}
