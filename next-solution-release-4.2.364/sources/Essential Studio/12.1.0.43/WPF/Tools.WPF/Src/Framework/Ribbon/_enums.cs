// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Specifies hit test area of the SplitButton control.
    /// </summary>
    public enum HitTestArea
    {
        /// <summary>
        /// Only image is selected.
        /// </summary>
        ImageOnly,

        /// <summary>
        /// Both label and image are selected.
        /// </summary>
        LabelAndImage
    }

    /// <summary>
    /// Represents the size form
    /// </summary>
    public enum SizeForm
    {
        /// <summary>
        /// Defines large size.
        /// </summary>
        Large,

        /// <summary>
        /// Defines small size.
        /// </summary>
        Small,

        /// <summary>
        /// Defines extra small size.
        /// </summary>
        ExtraSmall
    }

    /// <summary>
    /// Specifies state of RibbonBar layout.
    /// </summary>
    public enum RibbonBarState
    {
        /// <summary>
        /// Collapsed state.
        /// </summary>
        Collapsed,

        /// <summary>
        /// Layout in two rows.
        /// </summary>
        TwoRow,

        /// <summary>
        /// Extra small state for hosting in QuickAccessToolbar.
        /// </summary>
        ExtraSmall
    }

    /// <summary>
    /// Specifies state of the Ribbon.
    /// </summary>
    public enum RibbonState
    {
        /// <summary>
        /// Normal state.
        /// </summary>
        Normal,

        /// <summary>
        /// Hidden state.
        /// </summary>
        Hide,

        /// <summary>
        /// Adorned above the content.
        /// </summary>
        Adorner
    }

    /// <summary>
    /// Specifies state of the Ribbon TabButton.
    /// </summary>
    public enum TabStates
    {
        /// <summary>
        /// Normal state.
        /// </summary>
        Normal,

        /// <summary>
        /// Mouse hover state.
        /// </summary>
        Hover,

        /// <summary>
        /// Selected state.
        /// </summary>
        Selected,

        /// <summary>
        /// Selected mouse hover state.
        /// </summary>
        HoverSelected
    }

    /// <summary>
    /// Specifies type of ribbon color scheme
    /// </summary>
    public enum RibbonColorSchemeType
    {
        /// <summary>
        /// Blue scheme
        /// </summary>
        Blue,

        /// <summary>
        /// Silver scheme
        /// </summary>
        Silver,

        /// <summary>
        /// Black scheme
        /// </summary>
        Black
    }

    /// <summary>
    /// Specifies type of RibbonGallery resize direction.
    /// </summary>
    public enum ResizeDirection
    {
        /// <summary>
        /// Horizontal and vertical resize.
        /// </summary>
        HorizontalAndVertical,

        /// <summary>
        /// Vertical resize.
        /// </summary>
        VerticalOnly,

        /// <summary>
        /// No resize.
        /// </summary>
        NoResize
    }

    /// <summary>
    /// Specifies type of RibbonGallery visual mode.
    /// </summary>
    public enum RibbonGalleryVisualMode
    {
        /// <summary>
        /// InRibbon gallery.
        /// </summary>
        InRibbon,

        /// <summary>
        /// DropDown gallery.
        /// </summary>
        DropDown
    }

    /// <summary>
    /// Specifies RibbonGallery popup placement.
    /// </summary>
    public enum PopupPosition
    {
        /// <summary>
        /// Above the gallery.
        /// </summary>
        Above,

        /// <summary>
        /// Below the gallery.
        /// </summary>
        Below
    }

    /// <summary>
    /// Specified the mode of maximized RibbonWindow.
    /// </summary>
    public enum MaximizedMode
    {
        /// <summary>
        /// Maximized window occupy working area of the screen.
        /// </summary>
        Default,

        /// <summary>
        /// Maximized window occupy total area of the screen.
        /// </summary>
        KioskStyle
    }

    /// <summary>
    /// Specified the mode of RibbonBar in Resizing
    /// </summary>
    internal enum MeasureMode
    {
        /// <summary>
        /// Compresed Ribbon bar
        /// </summary>
        Compressed,

        /// <summary>
        /// Ribbon bar in default state.
        /// </summary>
        Default
    }
}

