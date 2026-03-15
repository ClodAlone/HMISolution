//-------------------------------------------------------------------------------------------------
// <copyright file="GridCurrentCellInfo.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Holds information about position of current cell, current cell renderer, and last active grid control.
    /// </summary>
    public class GridCurrentCellInfo
    {
        [NonSerialized]
        GridControlBase gridView;
        [NonSerialized]
        GridCellRendererBase cellView;
        int rowIndex;
        int colIndex;

        /// <summary>
        /// Initalizes a GridCurrentCellInfo with position of current cell, current cell renderer, and last active grid control.
        /// </summary>
        /// <param name="gridView">Last active grid control.</param>
        /// <param name="cellView">Current cell renderer.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        public GridCurrentCellInfo(GridControlBase gridView, GridCellRendererBase cellView, int rowIndex, int colIndex)
        {
            this.gridView = gridView;
            this.cellView = cellView;
            this.rowIndex = rowIndex;
            this.colIndex = colIndex;
        }

        /// <summary>
        /// Gets or sets the last active grid control.
        /// </summary>
        public GridControlBase GridView
        {
            [DebuggerStepThrough()]
            get
            {
                return gridView;
            }

            [DebuggerStepThrough()]
            set
            {
                gridView = value;
            }
        }

        /// <summary>
        /// Gets or sets the current cell renderer.
        /// </summary>
        public GridCellRendererBase CellView
        {
            [DebuggerStepThrough()]
            get
            {
                return cellView;
            }

            [DebuggerStepThrough()]
            set
            {
                cellView = value;
            }
        }

        /// <summary>
        /// Gets or sets the row index.
        /// </summary>
        public int RowIndex
        {
            [DebuggerStepThrough()]
            get
            {
                return rowIndex;
            }

            [DebuggerStepThrough()]
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
            [DebuggerStepThrough()]
            get
            {
                return colIndex;
            }

            [DebuggerStepThrough()]
            set
            {
                colIndex = value;
            }
        }
    }
}
