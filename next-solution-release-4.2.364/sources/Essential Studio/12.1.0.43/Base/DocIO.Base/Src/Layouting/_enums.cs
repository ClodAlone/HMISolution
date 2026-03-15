#region Copyright Syncfusion Inc. 2001 - 2014
//
//  Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
//
#endregion

using System;

namespace Syncfusion.Layouting
{
    #region HorizonatalAlignment
    /// <summary>
    /// 
    /// </summary>
    internal enum HorizontalAlignment
    {
        /// <summary>
        /// 
        /// </summary>
        Left,

        /// <summary>
        /// 
        /// </summary>
        Center,

        /// <summary>
        /// 
        /// </summary>
        Right,

        /// <summary>
        /// 
        /// </summary>
        Justify,
        /// <summary>
        /// 
        /// </summary>
        Distributed
    }
    #endregion

    #region LayoutState
    /// <summary>
    /// Represents state information of LayoutContext object
    /// </summary>
    internal enum LayoutState
    {
        /// <summary>
        /// Unknown state, set by itialization of LayoutContext
        /// <remarks>Cann't returned to parent context</remarks>
        /// </summary>
        Unknown,

        /// <summary>
        /// Current context contains at last one child. Layouting process on 
        /// current level not finished. 
        /// <remarks>Cann't returned to parent context</remarks>
        /// </summary>
        //Fitting,
        ///// <summary>
        ///// Current context can not contains childs widgets or children contexts.
        ///// <remarks>Widget of current context didn't fit</remarks>
        ///// </summary>
        NotFitted,

        /// <summary>
        /// Current context contains at last one child. Last widget 
        /// ( or child context ) was splitted.
        /// </summary>
        Splitted,
        /// <summary>
        /// Wrap string based on TextWrapBounds
        /// </summary>
        WrapText,

        /// <summary>
        /// Current context contains at last one child. Layouting process 
        /// finished. 
        /// </summary>
        Fitted,

        /// <summary>
        /// Current context contains at last one child. Layouting process 
        /// breaked. 
        /// </summary>
        Breaked
    }
    #endregion

    #region TabJustification
    /// <summary>
    /// 
    /// </summary>
    internal enum TabJustification
    {
        /// <summary>
        /// Left tab.
        /// </summary>
        Left = 0,

        /// <summary>
        /// Centered tab.
        /// </summary>
        Centered = 1,

        /// <summary>
        /// Right tab.
        /// </summary>
        Right = 2,

        /// <summary>
        /// Decimal tab.
        /// </summary>
        Decimal = 3,

        /// <summary>
        /// Bar.
        /// </summary>
        Bar = 4,
    }
    #endregion

    #region TabLeader
    /// <summary>
    /// 
    /// </summary>
    internal enum TabLeader
    {
        /// <summary>
        /// No leader.
        /// </summary>
        NoLeader = 0,

        /// <summary>
        /// Dotted leader.
        /// </summary>
        Dotted = 1,

        /// <summary>
        /// Hyphenated leader.
        /// </summary>
        Hyphenated = 2,

        /// <summary>
        /// Single line leader. 
        /// </summary>
        Single = 3,

        /// <summary>
        /// Heavy line leader.
        /// </summary>
        Heavy = 4
    }
    #endregion

    #region ChildrenLayoutDirection
    /// <summary>
    /// 
    /// </summary>
    internal enum ChildrenLayoutDirection
    {
        /// <summary>
        /// 
        /// </summary>
        Horizontal,

        /// <summary>
        /// 
        /// </summary>
        Vertical
    }
    #endregion

    #region TextLineType
    /// <summary>
    /// Break type of the line.
    /// </summary>
    [Flags]
    internal enum TextLineType
    {
        /// <summary>
        /// Unknown type line.
        /// </summary>
        None = 0,

        /// <summary>
        /// The line has new line symbol.
        /// </summary>
        NewLineBreak = 0x0001,

        /// <summary>
        /// layout break.
        /// </summary>
        LayoutBreak = 0x0002,

        /// <summary>
        /// The line is the first in the paragraph.
        /// </summary>
        FirstParagraphLine = 0x0004,

        /// <summary>
        /// The line is the last in the paragraph.
        /// </summary>
        LastParagraphLine = 0x0008
    }
    #endregion
}