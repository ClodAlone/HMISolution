// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Contains visual modes.
    /// </summary>
    public enum GalleryVisualMode
    {
        /// <summary>
        /// Defines standard visual mode.
        /// </summary>
        Standard = 0,

        /// <summary>
        /// Defines detailed visual mode.
        /// </summary>
        Detailed = 1
    }

    /// <summary>
    /// Contains allowed animations.
    /// </summary>
    [Flags]
    public enum AllowedAnimations
    {
        /// <summary>
        /// No animation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Layout animation.
        /// </summary>
        Layout = 1,

        /// <summary>
        /// Drag allowed animation.
        /// </summary>
        Drag = 2,

        /// <summary>
        /// New animation.
        /// </summary>
        New = 4,

        /// <summary>
        /// Resize animation.
        /// </summary>
        Resize = 8,

        /// <summary>
        /// All animation.
        /// </summary>
        All = 15
    }

    /// <summary>
    /// Contains allowed item resize modes.
    /// </summary>
    public enum AllowedItemResizeModes
    {
        /// <summary>
        /// No item resize.
        /// </summary>
        None,

        /// <summary>
        /// Allows to resize items.
        /// </summary>
        Resize,

        /// <summary>
        /// Adds spaces between items.
        /// </summary>
        Space
    }

    /// <summary>
    /// Contains alignments of caption used in GalleryItem.
    /// </summary>
    public enum CaptionAlignment
    {
        /// <summary>
        /// Captions are aligned to left.
        /// </summary>
        Left,

        /// <summary>
        /// Captions are aligned to right.
        /// </summary>
        Right,

        /// <summary>
        /// Captions are aligned to top.
        /// </summary>
        Top,

        /// <summary>
        /// Captions are aligned to bottom.
        /// </summary>
        Bottom
    }
}
