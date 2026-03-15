//-------------------------------------------------------------------------------------------------
// <copyright file="PivotGridHeaderCellModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Runtime.Serialization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    /// <summary>
    /// Implements the data / model part of a column or row header.
    /// </summary>
    [Serializable]
    public class PivotGridHeaderCellModel : GridStaticCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="PivotGridHeaderCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="PivotGridHeaderCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public PivotGridHeaderCellModel(GridModel grid)
            : base(grid)
        {
            AllowMerging = true;
        }

        /// <summary>
        /// Initializes a new <see cref="PivotGridHeaderCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected PivotGridHeaderCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotGridHeaderCellRenderer(control, this);
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
            return new Size(size.Width + 4, size.Height + 2);
        }
    }
}
