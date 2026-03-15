// <copyright file="_enums.cs" company="Syncfusion">
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws.
// </copyright>

namespace Syncfusion.Windows.Tools
{
    /// <summary>
    /// Contains sort directions.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Defines that there is no sort.
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines that sort direction is ascending.
        /// </summary>
        Ascending = 1,

        /// <summary>
        /// Defines that sort direction is descending.
        /// </summary>
        Descending = 2
    }

    public enum DragMode
    {
        /// <summary>
        /// Defines that click mode is LeftClick.
        /// </summary>
        LeftButton,

        /// <summary>
        /// Defines that click mode is RightClick.
        /// </summary>
        RightButton,

        /// <summary>
        /// Defines that click mode is BothClick.
        /// </summary>
        Both
    }

    /// <summary>
    /// Specifies the effects of a drag-and-drop operation for TreeViewAdv.
    /// </summary>
    public enum TreeViewItemAdvDragDropEffects
    {
        /// <summary>
        /// The drop target does not accept the data.
        /// </summary>
        None = 0,

        /// <summary>
        /// The data is copied to the drop target.
        /// </summary>
        Copy = 1,

        /// <summary>
        /// The data from the drag source is moved to the drop target.
        /// </summary>
        Move = 2,

        /// <summary>
        /// The data moved to the drop target, the dragdrop effect will not change by clicking the ctrl or shift keys
        /// </summary>
        MoveOnly,

        /// <summary>
        /// The data copied to the drop target, the dragdrop effect will not change by clicking the ctrl or shift keys
        /// </summary>
        CopyOnly
    }

    /// <summary>
    /// Specifies the animation type for TreeViewItemAdv.
    /// </summary>
    public enum AnimationType
    {
        /// <summary>
        /// Defines that there is no animation.
        /// </summary>
        None = 0,

        /// <summary>
        /// Defines that animation is slide.
        /// </summary>
        Slide = 1,

        /// <summary>
        /// Defines that animation is fade.
        /// </summary>
        Fade = 2
    }

    /// <summary>
    /// Specifies visual styles used for drawing <see cref="Syncfusion.Windows.Tools.Controls.TreeViewAdv"/> control.
    /// </summary>
    public enum TreeViewAdvVisualStyle
    {
        /// <summary>
        /// VisualStudio2005 visual style.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Office2003 visual style.
        /// </summary>
        Office2003 = 1,

        /// <summary>
        /// Office2007 Blue visual style.
        /// </summary>
        Office2007Blue = 2,

        /// <summary>
        /// Office2007 Silver visual style.
        /// </summary>
        Office2007Silver = 3,

        /// <summary>
        /// Office2007 Black visual style.
        /// </summary>
        Office2007Black = 4,

        /// <summary>
        /// Blend visual style.
        /// </summary>
        Blend = 5,

        /// <summary>
        /// Office2010 Blue visual style
        /// </summary>
        Office2010Blue = 6,

        /// <summary>
        /// Office2010 Silver visual style
        /// </summary>
        Office2010Silver = 7,

        /// <summary>
        /// Office2010 Black visual style
        /// </summary>
        Office2010Black = 8,

        /// <summary>
        /// Metri visual style
        /// </summary>
        Metro = 9,

        /// <summary>
        /// Transparent visual style
        /// </summary>
        Transparent = 10,

        /// <summary>
        /// Shiny Red visual style
        /// </summary>
        ShinyRed = 11,

        /// <summary>
        /// Shiny Blue visual style
        /// </summary>
        ShinyBlue = 12
    }

    /// <summary>
    /// Specifies column measure state
    /// </summary>
    public enum ColumnMeasureState
    {
        /// <summary>
        /// Init Measure state
        /// </summary>
        Init,

        /// <summary>
        /// Header Measure state
        /// </summary>
        Headered,

        /// <summary>
        /// Data Measure state
        /// </summary>
        Data,

        /// <summary>
        /// Width Measure state
        /// </summary>
        SpecificWidth,

        //Added here

        /// <summary>
        /// Width Measure state Automatic
        /// </summary>
        Auto,

        /// <summary>
        /// Width Measure state Propotion
        /// </summary>
        Star

        //Ended Here
    }

    /// <summary>
    /// Identifies the TreeView column header role
    /// </summary>
    public enum TreeViewColumnHeaderRole
    {
        /// <summary>
        /// Set the Normal value for the TreeViewColumnHeaderRole
        /// </summary>
        Normal,

        /// <summary>
        /// Set the Padding value for the TreeViewColumnHeaderRole
        /// </summary>
        Padding
    }

    public enum VirtualizationMode
    {
        Normal,
        Extended
    }
}