// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
// </copyright>

namespace Syncfusion.Windows.Tools.Controls
{
    /// <summary>
    /// Describes how control is docked to it's container.
    /// </summary>
    public enum DockSide
    {
        /// <summary>
        /// Control is docked to the left side of it's container.
        /// </summary>
        Left,
        
        /// <summary>
        /// Control is docked to the right side of it's container.
        /// </summary>
        Top,
        
        /// <summary>
        /// Control is docked to the top side of it's container.
        /// </summary>
        Right,

        /// <summary>
        /// Control is docked to the bottom side of it's container.
        /// </summary>
        Bottom,

        /// <summary>
        /// Control is docked as a tab page of it's container.
        /// </summary>
        Tabbed,

        /// <summary>
        /// Control is not docked.
        /// </summary>
        None
    }

    /// <summary>
    /// Specifies the control state.
    /// </summary>
    public enum DockState
    {
        /// <summary>
        /// Control is docked to the docking manager's surface.
        /// </summary>
        Dock,

        /// <summary>
        /// Control is not docked to the docking manager's surface.
        /// </summary>
        Float,

        /// <summary>
        /// Control is not visible at all.
        /// </summary>
        Hidden,

        /// <summary>
        /// Control is hidden and will show if mouse move under tab.
        /// </summary>
        AutoHidden,

        /// <summary>
        /// Control is tabbed or MDI document.
        /// </summary>
        Document
    }

    /// <summary>
    /// Specifies border mode of float window.
    /// </summary>
    public enum FloatWindowBorderMode
    {
        /// <summary>
        /// Set Border to FloatWindow's Header.
        /// </summary>
        Header,

        /// <summary>
        /// Set Border to FloatWindow's LeftTop.
        /// </summary>
        LeftTop,

        /// <summary>
        /// Set Border to FloatWindow's RightTop.
        /// </summary>
        RightTop,

        /// <summary>
        /// Set Border to FloatWindow's Left.
        /// </summary>
        Left,

        /// <summary>
        /// Set Border to FloatWindow's Right.
        /// </summary>
        Right,

        /// <summary>
        /// Set Border to FloatWindow's LeftBottom.
        /// </summary>
        LeftBottom,

        /// <summary>
        /// Set Border to FloatWindow's RightBottom.
        /// </summary>
        RightBottom,

        /// <summary>
        /// Set Border to FloatWindow's Bottom.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Describes possible splitter positions regards target.
    /// </summary>
    public enum SplitterTargetSide
    {
        /// <summary>
        /// Splitter position set to Before
        /// </summary>
        Before,

        /// <summary>
        /// Splitter position set to After
        /// </summary>
        After,

        /// <summary>
        /// Splitter position set to Both
        /// </summary>
        Both,

        /// <summary>
        /// Splitter position set to None
        /// </summary>
        None
    }

    /// <summary>
    /// CloseTabsMode is used to specify the behavior during tabs closing.
    /// </summary>
    public enum CloseTabsMode
    {
        /// <summary>
        /// Close all tabs in tabbed host.
        /// </summary>
        CloseAll,

        /// <summary>
        /// Close only the active tab in the tabbed host.
        /// </summary>
        CloseActive
    }

    /// <summary>
    /// AutoHideTabsMode is used to specify the behavior during auto hide tabs.
    /// </summary>
    public enum AutoHideTabsMode
    {
        /// <summary>
        /// AutoHide all tabs in tabbed host.
        /// </summary>
        AutoHideGroup,

        /// <summary>
        /// Auto hide only active tab in tabbed host.
        /// </summary>
        AutoHideActive
    }

    /// <summary>
    /// Specifies auto hide animation mode.
    /// </summary>
    public enum AutoHideAnimationMode
    {
        /// <summary>
        /// The element will slide while auto hide
        /// </summary>
        Slide,

        /// <summary>
        /// The element will scale while auto hide
        /// </summary>
        Scale,

        /// <summary>
        /// The element will fade while auto hide
        /// </summary>
        Fade
    }

    /// <summary>
    /// Specifies all possible drag provider actions.
    /// </summary>
    public enum DragProviderAction
    {
        /// <summary>
        /// Sets the DragProviderAction to Left
        /// </summary>
        Left,

        /// <summary>
        /// Sets the DragProviderAction to Top
        /// </summary>
        Top,

        /// <summary>
        /// Sets the DragProviderAction to Right
        /// </summary>
        Right,

        /// <summary>
        /// Sets the DragProviderAction to Bottom
        /// </summary>
        Bottom,

        /// <summary>
        /// Sets the DragProviderAction to Center
        /// </summary>
        Center,

        /// <summary>
        /// Sets the DragProviderAction to GlobalLeft
        /// </summary>
        GlobalLeft,

        /// <summary>
        /// Sets the DragProviderAction to Left
        /// </summary>
        GlobalTop,

        /// <summary>
        /// Sets the DragProviderAction to GlobalRight
        /// </summary>
        GlobalRight,

        /// <summary>
        /// Sets the DragProviderAction to GlobalBottom
        /// </summary>
        GlobalBottom,

        /// <summary>
        /// Sets the DragProviderAction to None
        /// </summary>
        None
    }

    /// <summary>
    /// Specifies the dragging type.
    /// </summary>
    public enum DraggingType
    {
        /// <summary>
        /// Only border of dragged element will be displayed at the cursor position while dragging.
        /// </summary>
        BorderDragging,

        /// <summary>
        /// Shadow of dragged element will be displayed at the cursor position while dragging.
        /// </summary>
        ShadowDragging,

        /// <summary>
        /// Entire dragged element will be displayed at the cursor position while dragging.
        /// </summary>
        NormalDragging
    }

    /// <summary>
    /// Specifies the dragging source.
    /// </summary>
    internal enum DraggingSource
    {
        /// <summary>
        /// Set as Tab
        /// </summary>
        Tab,

        /// <summary>
        /// Set as Host
        /// </summary>
        Host,

        /// <summary>
        /// Set as Window
        /// </summary>
        Window
    }

    /// <summary>
    /// Represents the Action Mode
    /// </summary>
    public enum ActionMode
    {
        /// <summary>
        /// Set as Active
        /// </summary>
        Active,

        /// <summary>
        /// Set as Group
        /// </summary>
        Group
    }

    /// <summary>
    /// Represents the Dock ability of the Docking WIndow
    /// </summary>
    public enum DockAbility
    {
        /// <summary>
        /// Sets the DockAbility to None
        /// </summary>
        None = 0,

        /// <summary>
        /// Sets the DockAbility to Left
        /// </summary>
        Left = 1,

        /// <summary>
        /// Sets the DockAbility to Right
        /// </summary>
        Right = 2,

        /// <summary>
        /// Sets the DockAbility to Horizontal
        /// </summary>
        Horizontal = 3,

        /// <summary>
        /// Sets the DockAbility to Top
        /// </summary>
        Top = 4,

        /// <summary>
        /// Sets the DockAbility to Bottom
        /// </summary>
        Bottom = 8,

        /// <summary>
        /// Sets the DockAbility to Vertical
        /// </summary>
        Vertical = 12,

        /// <summary>
        /// Sets the DockAbility to Tabbed
        /// </summary>
        Tabbed = 16,

        /// <summary>
        /// Sets the DockAbility to All
        /// </summary>
        All = 31
    }

    public enum OuterDockAbility
    {
        /// <summary>
        /// Sets the DockAbility to None
        /// </summary>
        None = 0,

        /// <summary>
        /// Sets the DockAbility to Left
        /// </summary>
        Left = 1,

        /// <summary>
        /// Sets the DockAbility to Right
        /// </summary>
        Right = 2,

        /// <summary>
        /// Sets the DockAbility to Horizontal
        /// </summary>
        Horizontal = 3,

        /// <summary>
        /// Sets the DockAbility to Top
        /// </summary>
        Top = 4,

        /// <summary>
        /// Sets the DockAbility to Bottom
        /// </summary>
        Bottom = 8,

        /// <summary>
        /// Sets the DockAbility to Vertical
        /// </summary>
        Vertical = 12,
        
        /// <summary>
        /// Sets the DockAbility to All
        /// </summary>
        All = 31
    }

    /// <summary>
    /// Represents the  DockFillMode of the Docking Window
    /// </summary>
    public enum DockFillModes
    {
        /// <summary>
        /// Sets the DockFillMode to Default
        /// </summary>
        Default,

        /// <summary>
        /// Sets the DockFillMode to Absolute
        /// </summary>
        Absolute
    }

    /// <summary>
    /// Represents the  MaximizeMode of the Docking Window
    /// </summary>
    public enum MaximizeMode
    {
        /// <summary>
        /// Sets the MaximizeMode to FullScreen
        /// </summary>
        FullScreen,

        /// <summary>
        /// Sets the MaximizeMode to Default
        /// </summary>
        Default
    }

    /// <summary>
    /// Represents the Splitter resize mode of the Docking Window
    /// </summary>
    public enum SplitterResizeMode
    {
        /// <summary>
        /// Sets the SplitterResizeMode to AllChildren
        /// </summary>
        AllChildren,

        /// <summary>
        /// Sets the SplitterResizeMode to EdgeChildren
        /// </summary>
        EdgeChildren
    }

    /// <summary>
    /// Represents the  DockFillDocumentMode of the Docking Window
    /// </summary>
    public enum DockFillDocumentMode
    {
        /// <summary>
        /// Sets the DockFillDocumentMode to Fill
        /// </summary>
        Fill,

        /// <summary>
        /// Sets the DockFillDocumentMode to Normal
        /// </summary>
        Normal
    }

    /// <summary>
    /// Represents the  VisibilityMode of the Docking Window
    /// </summary>
    public enum VisibilityMode
    {
        /// <summary>
        /// Sets the VisibilityMode to Collapse
        /// </summary>
        Collapse,

        /// <summary>
        /// Sets the VisibilityMode to Disable
        /// </summary>
        Disable
    }

    /// <summary>
    /// Represents the ScrollButtonsBar Behaviour
    /// </summary>
    public enum ScrollingButtonMode
    {
        Normal,

        Extended
    }

    /// <summary>
    /// Represents the Dock window size behaviour when window maximized
    /// </summary>
    public enum SizeChangeOnMaximizeMode
    {
        Normal,

        Extended
    }
}