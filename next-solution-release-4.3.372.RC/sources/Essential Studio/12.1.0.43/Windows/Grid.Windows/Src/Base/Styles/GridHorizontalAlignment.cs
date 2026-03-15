//-------------------------------------------------------------------------------------------------
// <copyright file="GridHorizontalAlignment.cs" company="syncfusion">
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
    /// Specifies the horizontal alignment of contents in a cell.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridHorizontalAlignment"/> enumeration represents the different horizontal alignment options for contents of a cell
    /// that you can specify with the <see cref="GridStyleInfo.HorizontalAlignment"/> property of the <see cref="GridStyleInfo"/> class.
    /// </remarks>
    public enum GridHorizontalAlignment
    {
        /// <summary>
        /// Specifies that the contents of a cell are aligned with the left.
        /// </summary>
        Left    = 0, 

        /// <summary>
        /// Specifies that the contents of a cell are aligned with the right.
        /// </summary>
        Right   = 1, 

        /// <summary>
        /// Specifies that the contents of a cell are aligned with the center.
        /// </summary>
        Center  = 2
    }
}
