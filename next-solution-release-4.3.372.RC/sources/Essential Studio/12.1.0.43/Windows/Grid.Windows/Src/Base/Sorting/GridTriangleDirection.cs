//-------------------------------------------------------------------------------------------------
// <copyright file="GridTriangleDirection.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Specifies the direction of a triangle when drawing triangle with <see cref="GridPaintTriangle.Paint(System.Drawing.Graphics,System.Drawing.Rectangle,Syncfusion.Windows.Forms.Grid.GridTriangleDirection,System.Drawing.Brush,System.Drawing.Pen,System.Drawing.Pen,System.Drawing.Pen,bool)"/>.
    /// </summary>
    [Serializable]
    public enum GridTriangleDirection
    {
        /// <summary>
        /// A triangle pointing up.
        /// </summary>
        Up = 0,

        /// <summary>
        /// A triangle pointing down.
        /// </summary>
        Down = 1,

        /// <summary>
        /// A triangle pointing to the left.
        /// </summary>
        Left = 2,

        /// <summary>
        /// A triangle pointing to the right.
        /// </summary>
        Right = 3,
    }
}

