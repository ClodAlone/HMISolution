//-------------------------------------------------------------------------------------------------
// <copyright file="GridBorderSide.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Specifies the sides of a rectangle to apply a border to.
    /// </summary>
    /// <remarks>
    ///       Use the members of this enumeration with the <see cref="GridBorderPaint.DrawRectangle(System.Drawing.Graphics,Syncfusion.Windows.Forms.Grid.GridBorder,System.Drawing.Rectangle,System.Drawing.Color,Syncfusion.Windows.Forms.Grid.GridBorderSide)"/>
    ///       method. 
    /// </remarks>
    [FlagsAttribute]
    public enum GridBorderSide
    {
        /// <summary>
        ///    <para>
        ///       A three-dimensional border on
        ///       the left edge
        ///       of the control.
        ///    </para>
        /// </summary>
        Left = 1, // 0x0001

        /// <summary>
        ///    <para>
        ///       A three-dimensional border on
        ///       the top edge
        ///       of the rectangle.
        ///    </para>
        /// </summary>
        Top = 2, // 0x0002

        /// <summary>
        ///    <para>
        ///       A three-dimensional border on
        ///       the right side
        ///       of the rectangle.
        ///    </para>
        /// </summary>
        Right = 4, // 0x0004

        /// <summary>
        ///    <para>
        ///       A three-dimensional border on
        ///       the bottom side
        ///       of the rectangle.
        ///    </para>
        /// </summary>
        Bottom = 8, // 0x0008

        /// <summary>
        ///    <para>
        ///       A three-dimensional border on all four
        ///       edges and fill the middle of
        ///       the rectangle with
        ///       the color defined for three-dimensional controls.
        ///    </para>
        /// </summary>
        All = 0x0f,
    }
}

