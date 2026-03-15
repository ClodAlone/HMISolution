//-------------------------------------------------------------------------------------------------
// <copyright file="GridPushButtonCellModel.cs" company="syncfusion">
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
using System.Windows.Forms;

using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the data / model part of a push button cell.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridPushButtonCellModel"/> can serve as model for several <see cref="GridPushButtonCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridPushButtonCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridPushButtonCellModel : GridCellModelBase
    {
        /// <overload>
        /// Initializes a new <see cref="GridPushButtonCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridPushButtonCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridPushButtonCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridPushButtonCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridPushButtonCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridPushButtonCellRenderer(control, this);
        }

        // Methods

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
            Font font = style.GdipFont;
            Size size;
            if (queryBounds == GridQueryBounds.Height)
            {
                GridRangeInfo area;
                Grid.GetSpannedRangeInfo(rowIndex, colIndex, out area);
                Rectangle rc = new Rectangle(0, 0, (int)Grid.ColWidths.GetTotal(area.Left, area.Right), (int)Grid.RowHeights.GetTotal(area.Top, area.Bottom));
                Rectangle cellRect = SubtractBorders(rc, style, false);
                size = g.MeasureString(style.Description, font, cellRect.Width).ToSize();
            }
            else
            {
                size = g.MeasureString(style.Description, font).ToSize();
            }

            size.Width += 8;
            size.Height += 4;
            return size;
        }
    }
}
