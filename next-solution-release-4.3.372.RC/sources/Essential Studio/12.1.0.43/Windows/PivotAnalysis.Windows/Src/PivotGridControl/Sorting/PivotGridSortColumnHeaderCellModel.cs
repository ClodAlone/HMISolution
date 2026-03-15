//-------------------------------------------------------------------------------------------------
// <copyright file="GridSortColumnHeaderCellModel.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;

using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    /// <summary>
    /// Implements the data / model part of a column header with sort indicator.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridSortColumnHeaderCellModel"/> can serve as model for several <see cref="GridSortColumnHeaderCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridSortColumnHeaderCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class PivotGridSortColumnHeaderCellModel : GridHeaderCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridSortColumnHeaderCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridSortColumnHeaderCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public PivotGridSortColumnHeaderCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridSortColumnHeaderCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected PivotGridSortColumnHeaderCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText. 
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText.</param>
        /// <returns>The formatted test for the gives value.</returns>
        public override string GetFormattedText(GridStyleInfo style, object value, int textInfo)
        {
            GridStyleInfo styleColumn = Grid.ColStyles[((GridStyleInfoIdentity)style.Identity).ColIndex];
            string desc = styleColumn.Description;
            if (desc.Length > 0)
            {
                return desc;
            }

            return base.GetFormattedText(style, value, textInfo);
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotGridSortColumnHeaderCellRenderer(control, this);
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

            size.Width += 12; // for sort triangle
            return size;
        }
    }
}
