//-------------------------------------------------------------------------------------------------
// <copyright file="GridIndentCell.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.IO;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms.Grid;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    #region CellModel
    /// <summary>
    /// Implements the DataModel part for an indent cell in a <see cref="GridTableModel"/>.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridTableIndentCellModel"/> can serve as model for several <see cref="GridTableIndentCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridTableIndentCellModel"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridTableIndentCellModel : GridCellModelBase
    {
        /// <overload>
        /// Initializes a new <see cref="GridTableIndentCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableIndentCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableIndentCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableIndentCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableIndentCellModel(GridModel grid)
            : base(grid)
        {
        }
#if ASPNET
#else
        /// <override/>
        /// <summary>Creates a cell renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridTableIndentCellRenderer(control, this);
        }
#endif
    }
    #endregion
    #region CellRenderer

#if ASPNET
#else
    /// <summary>
    /// Implements the renderer part of an indent cell which is used inside the grouping grid
    /// to display indention of groups or tables and optionally draw tree-lines.
    /// </summary>
    /// <remarks>
    /// The <see cref="GridStyleInfo.Text"/> property is used to indicate which
    /// part of a tree-line to draw. Possible line part types are:
    /// <list type="table">
    /// <listheader><term>Text</term><description>Descriptions</description></listheader>
    /// <item><term>T</term><description>A T-node.</description></item>
    /// <item><term>L</term><description>The last node.</description></item>
    /// <item><term>I</term><description>A continues line.</description></item>
    /// </list>
    /// </remarks>
    public class GridTableIndentCellRenderer : GridCellRendererBase
    {
        /// <summary>
        /// Initializes a new <see cref="GridCheckBoxCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCheckBoxCellModel"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridTableIndentCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }

        /// <summary>
        /// A reference to the parent grid.
        /// </summary>
        public new GridTableControl Grid
        {
            get
            {
                return (GridTableControl)base.Grid;
            }
        }

        private GridBorder defaultBorder;

        /// <summary>
        /// The default border-style used for tree lines.
        /// </summary>
        GridBorder DefaultBorder
        {
            get
            {
                if (this.defaultBorder == null)
                {
                    this.defaultBorder = new GridBorder(GridBorderStyle.Solid, SystemColors.ControlDarkDark);
                }

                return this.defaultBorder;
            }
        }

        /// <override/>
        /// <summary>
        /// Draw the contents of specified cell.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="cellRectangle">Cell rectangle.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">Cell style information.</param>
        public override void Draw(Graphics g, Rectangle cellRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.Draw(g, cellRectangle, rowIndex, colIndex, style);

            cellRectangle.Offset(((cellRectangle.Width / 2) - 1), 0);
            cellRectangle.Width = (cellRectangle.Width / 2) - 2;

            GridBorder border = style.Tag as GridBorder;
            if (border == null)
            {
                border = this.DefaultBorder;
            }

            switch (style.Text)
            {
                case "I":
                    {
                        GridBorderPaint.DrawRectangle(g, border, cellRectangle, style.BackColor, GridBorderSide.Left, false);
                        break;
                    }

                case "T":
                    {
                        GridBorderPaint.DrawRectangle(g, border, cellRectangle, style.BackColor, GridBorderSide.Left, false);
                        cellRectangle.Height = cellRectangle.Height / 2;
                        cellRectangle.Offset(1, 0);
                        GridBorderPaint.DrawRectangle(g, border, cellRectangle, style.BackColor, GridBorderSide.Bottom, false);
                        break;
                    }

                case "L":
                    {
                        cellRectangle.Height = cellRectangle.Height / 2;
                        GridBorderPaint.DrawRectangle(g, border, cellRectangle, style.BackColor, GridBorderSide.Left, false);
                        cellRectangle.Offset(1, 0);
                        GridBorderPaint.DrawRectangle(g, border, cellRectangle, style.BackColor, GridBorderSide.Bottom, false);
                        break;
                    }
            }
        }

        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            //// suppress click event - don't move current cell
        }
    }
#endif
    #endregion
}
