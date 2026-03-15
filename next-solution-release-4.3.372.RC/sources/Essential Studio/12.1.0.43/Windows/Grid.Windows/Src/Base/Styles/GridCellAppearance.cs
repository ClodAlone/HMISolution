//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellAppearance.cs" company="syncfusion">
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
    /// Specifies the 3D-look for a cell.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridCellAppearance"/> enumeration represents the different options for the appearance of a cell
    /// that you can specify with the <see cref="GridStyleInfo.CellAppearance"/> property of the <see cref="GridStyleInfo"/> class.
    /// </remarks>
    public enum GridCellAppearance
    {
        /// <summary>
        /// Specifies that the cell is drawn flat (default).
        /// </summary>
        Flat = 0,

        /// <summary>
        /// Specifies that the cell is drawn with a raised border (default for row and column headers).
        /// </summary>
        Raised = 1,

        /// <summary>
        /// Specifies that the cell is drawn with a sunken border.
        /// </summary>
        Sunken = 2
    }
}
