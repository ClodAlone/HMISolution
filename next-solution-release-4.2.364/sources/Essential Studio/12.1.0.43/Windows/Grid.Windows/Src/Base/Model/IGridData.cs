//-------------------------------------------------------------------------------------------------
// <copyright file="IGridData.cs" company="syncfusion">
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
    /// <see cref="IGridData"/> defines an interface that <see cref="GridStyleInfoIdentity"/>
    /// utilizes to query cell contents, base styles, look up cell types, and save changes back to the
    /// grid.
    /// </summary>
    public interface IGridData
    {
        /// <summary>
        /// Gets an array that consists of table, row, and column base styles for the specified row and column index.
        /// </summary>       
        /// <returns>returns GridStyleInfo array</returns>
        GridStyleInfo[] GetBaseStyles(GridStyleInfo styleInfo, int rowIndex, int colIndex);
        
        /// <summary>
        /// Gives access to a <see cref="GridStyleInfo"/> at a given row and column index. 
        /// </summary>
        GridStyleInfo this[int rowIndex, int colIndex] 
        { 
            get; set; 
        }

        /// <summary>
        /// Gets access to the <see cref="GridBaseStylesMap"/> that is stored with <see cref="GridModel.BaseStylesMap"/>.
        /// </summary>
        GridBaseStylesMap BaseStylesMap 
        { 
            get;
        }

        /// <summary>
        /// Looks up a <see cref="GridCellModelBase"/> for a given cell type as specified with <see cref="GridStyleInfo.CellType"/>.
        /// </summary>
        /// <returns>returns GridCellModelBase</returns>
        GridCellModelBase LookupCellModel(string id);
    }
}
