//-------------------------------------------------------------------------------------------------
// <copyright file="GridEvents.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;

using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

using Syncfusion.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Represents a method that handles cancelable events associated with a specific cell.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellCancelEventArgs"/> that contains the event data.</param>
    public delegate void GridCellCancelEventHandler(object sender, GridCellCancelEventArgs e);

    /// <summary>
    /// Holds row and column coordinates for cancelable events associated with a specific cell.
    /// </summary>
    /// <remarks>
    /// No events use this class directly but it is used 
    /// as a base class for several other events related to a specific cell.
    /// </remarks>
    public class GridCellCancelEventArgs : SyncfusionCancelEventArgs
    {
        int rowIndex;
        int colIndex;

        /// <summary>
        /// Initializes a new <see cref="GridCellCancelEventArgs"/> object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridCellCancelEventArgs(int rowIndex, int colIndex)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }
    }

    /// <summary>
    /// Represents a method that handles events associated with a specific cell.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">An <see cref="GridCellEventArgs"/> that contains the event data.</param>
    public delegate void GridCellEventHandler(object sender, GridCellEventArgs e);

    /// <summary>
    /// Holds row and column coordinates for events associated with a specific cell.
    /// </summary>
    /// <remarks>
    /// Directly used by <see cref="GridCellModelBase.ActiveTextChanged"/>, <see cref="GridCellButton.Clicked"/>, <see cref="GridCellButton.HoveringChanged"/>,
    /// <see cref="GridCellButton.MouseDownChanged"/>, and <see cref="GridCellButton.PushedChanged"/>.
    /// <para/>
    /// Used also as base class for several other events related to a specific cell.
    /// </remarks>
    public class GridCellEventArgs : SyncfusionEventArgs
    {
        int rowIndex;
        int colIndex;

        /// <summary>
        /// Initializes a new <see cref="GridCellEventArgs"/> object.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        public GridCellEventArgs(int rowIndex, int colIndex)
        {
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        [TraceProperty(true)]
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        [TraceProperty(true)]
        public int ColIndex
        {
            get
            {
                return colIndex;
            }
        }
    }
    
    ////TODO: [Obsolete]

    /// <summary>For internal use.</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public delegate void GridChangeLayoutCellsEventHandler(object sender, GridChangeLayoutCellsEventArgs e);

    ////TODO: [Obsolete("Still being discussed if ChangeLayout event is needed.")]

    /// <summary>For internal use.</summary>
    /// <internalonly/>
    [Syncfusion.Documentation.DocumentationExclude()]
    public class GridChangeLayoutCellsEventArgs : SyncfusionEventArgs
    {
        private GridRangeInfo range;

        /// <summary>For internal use.</summary>
        /// <internalonly/>
        public GridChangeLayoutCellsEventArgs(GridRangeInfo range)
        {
            this.range = range;
        }

        /// <summary>Gets Range. For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [TraceProperty(true)]
        public GridRangeInfo Range
        {
            get
            {
                return this.range;
            }
        }
    }
}