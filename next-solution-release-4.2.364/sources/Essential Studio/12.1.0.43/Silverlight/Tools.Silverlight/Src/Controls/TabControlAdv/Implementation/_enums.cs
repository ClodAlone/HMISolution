#region Copyright
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

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Specifies the position of the drag marker relative to the tab item.
    /// </summary>
    public enum DragMarkerAlignment
    {
        /// <summary>
        /// Specifies that drag marker will be positioned to left of the tab item.
        /// </summary>
        LeftSide,

        /// <summary>
        /// Specifies that drag marker will be positioned to right of the tab item.
        /// </summary>
        RightSide
    }

    /// <summary>
    /// Specifies the tab scroll buttons visibility.
    /// </summary>
    public enum TabScrollButtonVisibility
    {
        /// <summary>
        /// Specifies that scroll buttons will appear when tab panel width is less than width of tab items.
        /// </summary>
        Auto,

        /// <summary>
        /// Specifies that scroll buttons will be hidden.
        /// </summary>
        Hidden,

        /// <summary>
        /// Specifies that scroll buttons will be visible.
        /// </summary>
        Visible
    }

    /// <summary>
    /// Specifies the tab scroll style.
    /// </summary>
    public enum TabScrollStyle
    {
        /// <summary>
        /// Specifies the normal scroll style.
        /// </summary>
        Normal,

        /// <summary>
        /// Specifies the extended scroll style.
        /// </summary>
        Extended
    }

    /// <summary>
    /// Specifies the tab items layout type.
    /// </summary>
    public enum TabItemLayoutType
    {
        /// <summary>
        /// Specifies that tab items will be layouted in single line.
        /// </summary>
        SingleLine,

        /// <summary>
        /// Specifies that tab items will be layouted in several lines.
        /// </summary>
        MultiLine,

        /// <summary>
        /// Specifies that tab items will be layouted in several lines 
        /// and every line will be fully covered with them.
        /// </summary>
        MultiLineWithFullWidth
    }

    /// <summary>
    /// Specifies the tab items size mode.
    /// </summary>
    public enum TabItemSizeMode
    {
        /// <summary>
        /// Specifies that tab items will be layouted normally.
        /// </summary>
        Normal,

        /// <summary>
        /// Specifies that tab items will be shrinked to fit the tab panel size.
        /// </summary>
        ShrinkToFit
    }

    /// <summary>
    /// Specifies the tab item's image alignment.
    /// </summary>
    public enum ImageAlignment
    {
        /// <summary>
        /// Specifies the tab item's image will be aligned to the left of the text.
        /// </summary>
        LeftOfText,

        /// <summary>
        /// Specifies the tab item's image will be aligned above the text.
        /// </summary>
        AboveText,

        /// <summary>
        /// Specifies the tab item's image will be aligned to the right of the text.
        /// </summary>
        RightOfText,

        /// <summary>
        /// Specifies the tab item's image will be aligned below the text.
        /// </summary>
        BelowText
    }

    /// <summary>
    /// Specifies the tab item's header alignment.
    /// </summary>
    public enum HeaderAlignment
    {
        /// <summary>
        /// Specifies the tab item's header will be aligned to the left.
        /// </summary>
        Left,

        /// <summary>
        /// Specifies the tab item's header will be aligned to the center.
        /// </summary>
        Center,

        /// <summary>
        /// Specifies the tab item's header will be aligned to the right.
        /// </summary>
        Right
    }

    /// <summary>
    /// Specifies the close button type.
    /// </summary>
    public enum CloseButtonType
    {
        /// <summary>
        /// Specifies that close button will be displayed only on the tab panel.
        /// </summary>
        Common,

        /// <summary>
        /// Specifies that close button will be displayed on each tab item.
        /// </summary>
        Individual,

        /// <summary>
        /// Specifies that close button will be displayed on the tab panel and on each tab item.
        /// </summary>
        Both,

        /// <summary>
        /// Specifies that close button will be displayed on each tab item when mouse is over it.
        /// </summary>
        IndividualOnMouseOver,

        /// <summary>
        /// Specifies that close button will be hidden.
        /// </summary>
        Hide
    }

    /// <summary>
    /// Specifies the states of the tab item.
    /// </summary>
    public enum TabStates
    {
        /// <summary>
        /// Specifies the normal state of the tab item.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the tab item is hovered with mouse pointer.
        /// </summary>
        Hover,

        /// <summary>
        /// Specifies that the tab item is selected.
        /// </summary>
        Selected,

        /// <summary>
        /// Specifies that the tab item is selected and is hovered with mouse pointer.
        /// </summary>
        HoverSelected
    }

    /// <summary>
    /// Specifies the scroll direction.
    /// </summary>
    public enum ScrollDirection
    {
        /// <summary>
        /// Specifies that tab panel will be scrolled to the next tab.
        /// </summary>
        NextTab,

        /// <summary>
        /// Specifies that tab panel will be scrolled to the previous tab.
        /// </summary>
        PrevTab,

        /// <summary>
        /// Specifies that tab panel will be scrolled to the next page.
        /// </summary>
        NextPage,

        /// <summary>
        /// Specifies that tab panel will be scrolled to the previous page.
        /// </summary>
        PrevPage,

        /// <summary>
        /// Specifies that tab panel will be scrolled to the first tab.
        /// </summary>
        FirstTab,

        /// <summary>
        /// Specifies that tab panel will be scrolled to the last tab.
        /// </summary>
        LastTab
    }   

    /// <summary>
    /// Specifies how the tab headers align relative to the tab content.
    /// </summary>
    public enum TabStripPlacement
    {
        /// <summary>
        /// Specifies that the tab headers should be positioned on the top of the tab content.
        /// </summary>
        Top,

        /// <summary>
        /// Specifies that the tab headers should be positioned on the left of the tab content.
        /// </summary>
        Left,

        /// <summary>
        /// Specifies that the tab headers should be positioned on the right of the tab content.
        /// </summary>
        Right,

        /// <summary>
        /// Specifies that the tab headers should be positioned on the bottom of the tab content.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TabItemOpenMode
    {   
        /// <summary>
        /// 
        /// </summary>
        OnMouseClick,
        /// <summary>
        /// 
        /// </summary>
        OnMouseOver
    }
}
