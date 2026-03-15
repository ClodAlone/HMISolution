//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellHitTestInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Saves Hit-Test information for cell renderers and cell button elements.
    /// </summary>
    public class GridCellHitTestInfo : ICloneable
    {
        private Point point = Point.Empty;
        private int rowIndex = 0;
        private int colIndex = 0;
        private GridCellRendererBase cellRenderer = null;
        private Rectangle cellBounds = Rectangle.Empty;
        private GridCellButton cellButtonElement = null;
        private Rectangle cellButtonBounds = Rectangle.Empty;
        private int cellButtonIndex = 0;

        /// <summary>
        /// Initializes an empty <see cref="GridCellHitTestInfo"/> object.
        /// </summary>
        public GridCellHitTestInfo()
        {
        }

        /// <summary>
        /// Performs a copy and returns the new object.
        /// </summary>
        /// <returns>A copy of the current object.</returns>
        public object Clone()
        {
            GridCellHitTestInfo cc = new GridCellHitTestInfo();
            cc.point = point;
            cc.rowIndex = rowIndex;
            cc.colIndex = colIndex;
            cc.cellRenderer = cellRenderer;
            cc.cellBounds = cellBounds;
            cc.cellButtonElement = cellButtonElement;
            cc.cellButtonBounds = cellButtonBounds;
            cc.cellButtonIndex = cellButtonIndex;

            return cc;
        }

        /// <override/>
        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see
        /// cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see
        /// cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            return String.Concat( 
                "row = ", 
                rowIndex.ToString(), 
                "col = ", 
                colIndex.ToString(),
                "index = ", 
                cellButtonIndex.ToString(), 
                " cellButtonBounds = ", 
                cellButtonBounds,
                "cellButton = ", 
                cellButtonElement.ToString());
        }

        /// <summary>
        /// Gets or sets the mouse coordinates.
        /// </summary>
        public Point Point
        {
            get
            {
                return point;
            }

            set
            {
                point = value;
            }
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return rowIndex;
            }

            set
            {
                rowIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the column index.
        /// </summary>
        public int ColIndex
        {
            get
            {
                return colIndex;
            }

            set
            {
                colIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="GridCellRendererBase"/>.
        /// </summary>
        public GridCellRendererBase CellRenderer
        {
            get
            {
                return cellRenderer;
            }

            set
            {
                cellRenderer = value;
            }
        }

        /// <summary>
        /// Gets or sets the cell boundaries.
        /// </summary>
        public Rectangle CellBounds
        {
            get
            {
                return cellBounds;
            }

            set
            {
                cellBounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the affected <see cref="GridCellButton"/>.
        /// </summary>
        public GridCellButton CellButtonElement
        {
            get
            {
                return cellButtonElement;
            }

            set
            {
                cellButtonElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the boundaries of the <see cref="GridCellButton"/>.
        /// </summary>
        public Rectangle CellButtonBounds
        {
            get
            {
                return cellButtonBounds;
            }

            set
            {
                cellButtonBounds = value;
            }
        }

        /// <summary>
        /// Gets or sets the index of the <see cref="GridCellButton"/> in the <see cref="GridCellRendererBase"/>.
        /// </summary>
        public int CellButtonIndex
        {
            get
            {
                return cellButtonIndex;
            }

            set
            {
                cellButtonIndex = value;
            }
        }
    }
}
