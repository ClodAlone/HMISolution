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
    /// Zoom mode to specify value in Percentage or double value.
    /// Example: 
    ///     0.5d is 50%
    ///     1d is equal to 100%
    ///     1.5d is 150%
    /// </summary>
    public enum ZoomMode
    {
        /// <summary>
        /// Access zoom value in double type
        /// </summary>
        Unit,

        /// <summary>
        /// Access zoom value in Percentage
        /// </summary>
        Percentage
    }

    /// <summary>
    /// Define MouseWheel, MouseClick, KeyCombination gesture for zooming.
    /// </summary>
    public enum ZoomGesture
    {
        /// <summary>
        /// No gesture.
        /// </summary>
        None = 0,

        /// <summary>
        /// Mouse is scrolled up.
        /// </summary>
        MouseWheelUp = 1,

        /// <summary>
        /// Mouse is scrolled down.
        /// </summary>
        MouseWheelDown = 2,

        /// <summary>
        /// Ctrl key.
        /// </summary>
        Ctrl = 4,

        /// <summary>
        /// Shift key.
        /// </summary>
        Shift = 8,

        /// <summary>
        /// Alt key.
        /// </summary>
        Alt = 16,

        /// <summary>
        /// Mouse left click.
        /// </summary>
        LeftClick = 32,

        /// <summary>
        /// Mouse right click.
        /// </summary>
        RightClick = 64,

        /// <summary>
        /// Mouse left double click.
        /// </summary>
        LeftDoubleClick = 128,

        /// <summary>
        /// Mouse right double click.
        /// </summary>
        RightDoubleClick = 256,

        /// <summary>
        /// All the Ctrl, Shift, Alt gesture should be satisfied.
        /// </summary>
        And = 512
    }

    /// <summary>
    /// Mouse state.
    /// </summary>
    internal enum OverviewMouseState
    {
        /// <summary>
        /// Mouse left button clicked.
        /// </summary>
        LeftClick,

        /// <summary>
        /// Mouse right button clicked.
        /// </summary>
        RightClick,

        /// <summary>
        /// Mouse left button double clicked.
        /// </summary>
        LeftDoubleClick,

        /// <summary>
        /// Mouse right button double clicked.
        /// </summary>
        RightDoubleClick,

        /// <summary>
        /// Panning.
        /// </summary>
        Pan,

        /// <summary>
        /// None state.
        /// </summary>
        None
    }

    internal enum ScrollParamether
    {
        HorizontalOffset,
        VerticalOffset,
        ViewportWidth,
        ViewportHeight,
        ExtentWidth,
        ExtentHeight
    }
}
