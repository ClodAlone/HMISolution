#region Copyright Syncfusion Inc. 2001 - 2014

// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Re-distribution in any form is strictly
// prohibited. Any infringement will be prosecuted under applicable laws. 
#endregion

using System;
namespace Syncfusion.Windows.Forms.Tools.MultiColumnTreeView
{
    /// <summary>
    /// Specifies the selection mode for the tree.
    /// </summary>
    public enum TreeSelectionMode
    {
        /// <summary>
        /// Lets you select one node at a time.
        /// </summary>
        Single,

        /// <summary>
        /// Lets you select multiple nodes within the same level.
        /// </summary>
        MultiSelectSameLevel,

        /// <summary>
        /// Lets you select multiple nodes across all levels.
        /// </summary>
        MultiSelectAll
    }

    /// <summary>
    /// Specifies the different sort types that can be specified in the <see cref="TreeNodeAdv.Sort(Syncfusion.Windows.Forms.Tools.TreeNodeAdvSortType)"/> method.
    /// </summary>
    public enum TreeNodeAdvSortType
    {
        /// <summary>
        /// Sorts by text.
        /// </summary>
        Text,

        /// <summary>
        /// Sorts by the tag value.
        /// </summary>
        Tag,

        /// <summary>
        /// Sorts by the checkbox value.
        /// </summary>
        CheckBox
    }

    /// <summary>
    /// Specifies the node positions in a node collection.
    /// </summary>
    public enum NodePositions
    {
        /// <summary>Represents First Node position</summary>
        First,

        /// <summary>Represents Last Node position</summary>
        Last,

        /// <summary>Represents Previous Node position</summary>
        Previous,

        /// <summary>Represents Next Node position</summary>
        Next
    }

    public enum PredefinedPrimitiveTypes
    {
        /// <summary>Represents Text</summary>
        Text = 0,

        /// <summary>Represents Left Images</summary>
        LeftImages = 1,

        /// <summary>Represents Right Images</summary>
        RightImages = 2,

        /// <summary>Represents CheckBox</summary>
        CheckBox = 3,

        /// <summary>Represents State Image</summary>
        StateImage = 4,

        /// <summary>Represents Option Button</summary>
        OptionsButton = 5,

        /// <summary> Represents Custom Control</summary>
        CustomControl = 6
    }

    /// <summary>
    /// Specifies the action that raised a TreeViewAdv event.
    /// </summary>
    public enum TreeViewAdvAction
    {
        /// <summary>
        /// The event was caused by a keystroke.
        /// </summary>
        ByKeyboard,

        /// <summary>
        /// The event was caused by a mouse operation.
        /// </summary>
        ByMouse,

        /// <summary>
        /// The event was caused by the <see cref="TreeNodeAdv"/> collapsing.
        /// </summary>
        Collapse,

        /// <summary>
        /// The event was caused by the <see cref="TreeNodeAdv"/> expanding.
        /// </summary>
        Expand,

        /// <summary>
        /// The action that caused the event is unknown.
        /// </summary>
        Unknown
    }

    #region TreeSearchFunctions

    /// <summary>
    /// Enum for specifying the options to find and replace in TreeView.
    /// </summary>
    [Flags]
    public enum TreeViewSearchOption
    {
        MatchWholeText = 0,
        MatchCase = 1,
    }

    /// <summary>
    /// Enum for specifying the levels of range to find and replace in TreeView.
    /// </summary>
    public enum TreeViewSearchRange
    {
        TreeView,
        RootNode,
        ChildNode
    }

    /// <summary>
    /// Enum for specifying the navigation style to find and replace in TreeView.
    /// </summary>
    public enum TreeViewSearchNavigation
    {
        SearchUp,
        SearchDown,
        SearchAll
    }

    #endregion
}