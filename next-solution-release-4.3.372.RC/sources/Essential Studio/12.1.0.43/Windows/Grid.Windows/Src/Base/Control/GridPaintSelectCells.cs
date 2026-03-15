//-------------------------------------------------------------------------------------------------
// <copyright file="GridPaintSelectCells.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{    
    /// <internalonly/>
    /// <summary>
    /// Redraws the selected range of grid cells.    
    /// </summary>  
    [Syncfusion.Documentation.DocumentationExclude()]
   
    public sealed class GridPaintSelectCells : GridSubComponent, IGridPaintSelectCells
    {
        // Fields
        private GridPaintSelectCellsInternal paintSelectCells;

        // Constructor

        /// <internalonly/>
        /// <summary>
        /// Initializes a <see cref="GridPaintSelectCells"/> and associates it with a grid.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        public GridPaintSelectCells(GridControlBase grid)
            : base(grid)
        {
            try
            {
                paintSelectCells = new GridPaintSelectCellsInternal();
                paintSelectCells.grid = grid;
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                // implementation not found, functionality is disabled.
                paintSelectCells = null;
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void UpdateSelectRange(GridRangeInfo range, GridRangeInfoList pOldRangeList)
        {
            if (paintSelectCells != null)
            {
                paintSelectCells.UpdateSelectRange(range, pOldRangeList);
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void PrepareClearSelection()
        {
            if (paintSelectCells != null)
            { 
                paintSelectCells.PrepareClearSelection();
            }
        }

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void PrepareChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange)
        {
            if (paintSelectCells != null)
            {
                paintSelectCells.PrepareChangeSelection(oldRange, newRange);
            }
        }
    }
}
