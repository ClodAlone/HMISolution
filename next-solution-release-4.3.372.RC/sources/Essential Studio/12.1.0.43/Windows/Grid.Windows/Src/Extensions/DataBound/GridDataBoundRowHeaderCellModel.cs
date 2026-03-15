//-------------------------------------------------------------------------------------------------
// <copyright file="GridDataBoundRowHeaderCellModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part for a row header cell in a <see cref="GridDataBoundGrid"/>. The
    /// row header cell will display an arrow for the current row, a star for the append row, and
    /// a pencil when the row is being edited.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDataBoundRowHeaderCellModel"/> can serve as model for several <see cref="GridDataBoundRowHeaderCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDataBoundRowHeaderCellModel"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridDataBoundRowHeaderCellModel : GridHeaderCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridDataBoundRowHeaderCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDataBoundRowHeaderCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDataBoundRowHeaderCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <summary>
        /// Initializes a new <see cref="GridDataBoundRowHeaderCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDataBoundRowHeaderCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <override/>
        /// <summary>Creates a cell renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridDataBoundRowHeaderCellRenderer(control, this);
        }

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins and any buttons.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="queryBounds">grsphical bounds</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size size = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            return new Size(34, size.Height);
        }
    }
}