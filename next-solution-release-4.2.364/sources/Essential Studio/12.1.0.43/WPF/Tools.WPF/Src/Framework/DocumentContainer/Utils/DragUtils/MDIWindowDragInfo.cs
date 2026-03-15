// <copyright file="MDIWindowDragInfo.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System.Windows;
using System.Diagnostics;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Presents struct that contains info of MDI dragging.
    /// </summary>

    internal struct MDIWindowDragInfo
    {
        #region Members
        /// <summary>
        /// Presents start rect of drag.
        /// </summary>
        internal readonly PercentRect DragStartRect;
        
        /// <summary>
        /// Presents start point of drag.
        /// </summary>
        internal readonly PercentPoint DragStartPoint;
        
        /// <summary>
        /// Presents drag mode.
        /// </summary>
        internal readonly MDIBorder Border;
        
        /// <summary>
        /// Presents dragged window. 
        /// </summary>
        internal readonly MDIWindow WindowDragged;
        
        /// <summary>
        /// Presents content of dragged window.
        /// </summary>
        internal readonly UIElement Content;
        #endregion

        #region Initializaer
        /// <summary>
        /// Initializes a new instance of the <see cref="MDIWindowDragInfo"/> struct.
        /// </summary>
        /// <param name="rect">The rect Content.</param>
        /// <param name="point">The point Content.</param>
        /// <param name="border">The border Content.</param>
        /// <param name="window">The window Content.</param>
        internal MDIWindowDragInfo(PercentRect rect, PercentPoint point, MDIBorder border, MDIWindow window)
        {
            DragStartRect = rect;
            DragStartPoint = point;
            Border = border;
            WindowDragged = window;
            Content = (window != null) ? window.Content : null;
        }
        #endregion
    }
}