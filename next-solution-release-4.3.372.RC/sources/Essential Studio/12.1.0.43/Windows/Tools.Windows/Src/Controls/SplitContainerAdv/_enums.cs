#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System.ComponentModel;
namespace Syncfusion.Windows.Forms.Tools.Enums
{
    /// <summary>
    /// Specifies which split panel is fixed, if any.
    /// </summary>
    public enum FixedPanel
    {
        /// <summary>
        /// "None"   - no panel;
        /// </summary>
        None = 0,

        /// <summary>
        /// "Panel1" - panel #1 respectively.
        /// </summary>
       Panel1 = 1,

        /// <summary>
        /// "Panel2" - panel #1 respectively.
        /// </summary>
        Panel2 = 2
    }

    /// <summary>
    /// Specifies splitter rendering style.
    /// </summary>
    public enum Style
    {
        /// <summary>
        /// Style is absent.
        /// </summary>
        None,

        /// <summary>
        /// Office 2007 Black style.
        /// </summary>
        Office2007Black,

        /// <summary>
        /// Office 2007 Blue style.
        /// </summary>
        Office2007Blue,

        /// <summary>
        /// Office 2007 Silver style.
        /// </summary>
        Office2007Silver,

        /// <summary>
        /// Office 2003 style.
        /// </summary>
        Office2003,

        /// <summary>
        /// Office XP style.
        /// </summary>
        OfficeXP,

        /// <summary>
        /// Visual Studio 2005 style.
        /// </summary>
        VS2005,

        /// <summary>
        /// Mozilla browser style.
        /// </summary>
        Mozilla,

        /// <summary>
        /// default style.
        /// </summary>
        Default
    }

    /// <summary>
    /// Specifies splitter current state of being drawn.
    /// </summary>
    public enum DrawState
    {
        /// <summary>
        /// when splitter is not hovered;
        /// </summary>
        Normal,

        /// <summary>
        /// when splitter is hovered by the mouse.
        /// </summary>
        Hovered
    } 

    /// <summary>
    /// Specifies whether the control is currently being dragged, and how.
    /// </summary>
    public enum DragState
    {
        /// <summary>
        /// the splitter is still;
        /// </summary>
        Normal,

        /// <summary>
        /// the splitter is being dragged now;
        /// </summary>
        Dragged,

        /// <summary>
        /// the splitter is expanded;
        /// </summary>
        Expanded,

        /// <summary>
        /// the splitter is collapsed.
        /// </summary>
        Collapsed
    }

    /// <summary>
    /// Specifies currently collapsed panel.
    /// </summary>
    public enum CollapsedPanel
    {
        /// <summary>
        /// Panel1 respectively.
        /// </summary>
        Panel1,

        /// <summary>
        /// Panel2 respectively.
        /// </summary>
        Panel2,

        /// <summary>
        /// no panel is collapsed;
        /// </summary>
        None
    }

    /// <summary>
    /// Specifies an event which leads to collapsing of previously specified panel.
    /// </summary>
    public enum TogglePanelOn
    {
        /// <summary>
        /// panel is toggled when a click on it occurs;
        /// </summary>
        Click,

        /// <summary>
        /// panel is toggled when a doubleclick on it occurs.
        /// </summary>
        DoubleClick
    }

    /// <summary>
    /// Specifies needed property
    /// </summary>
    public enum RendererProperty
    {
        /// <summary>
        /// Represents Unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// BackgroundColor respectively
        /// </summary>
        BackgroundColor,

        /// <summary>
        /// ExpandFill respectively
        /// </summary>
        ExpandFill,

        /// <summary>
        /// ExpandLine respectively
        /// </summary>
        ExpandLine,

        /// <summary>
        /// GripLight respectively
        /// </summary>
        GripLight,

        /// <summary>
        /// GripDark respectively
        /// </summary>
        GripDark,

        /// <summary>
        /// HotBackgroundColor respectively
        /// </summary>
        HotBackgroundColor,

        /// <summary>
        /// HotExpandFill respectively
        /// </summary>
        HotExpandFill,

        /// <summary>
        /// HotExpandLine respectively
        /// </summary>
        HotExpandLine,

        /// <summary>
        /// HotGripLight respectively
        /// </summary>
        HotGripLight,

        /// <summary>
        /// HotGripDark respectively
        /// </summary>
        HotGripDark
    }
}