//-------------------------------------------------------------------------------------------------
// <copyright file="GridInCell.cs" company="Syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.GridHelperClasses
{
    using System;
    using System.Drawing;
    using System.Collections;
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.Data;
    using System.Runtime.Serialization;

    using Syncfusion.Diagnostics;
    using Syncfusion.Windows.Forms;
    using Syncfusion.Windows.Forms.Grid;

    /// <summary>
    /// Implements a data model for GridInCell cell type.
    /// </summary>
    public class GridInCellModel : GridGenericControlCellModel
    {
        /// <summary>
        /// Constructor for GridInCellModel class
        /// </summary>
        /// <param name="info">SerializationInfo</param>
        /// <param name="context">StreamingContext</param>
        protected GridInCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Constructor for GridInCellModel.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public GridInCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
        }

        /// <summary>
        /// Creates renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridInCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Implements a renderer for GridInCell cell type.
    /// </summary>
    public class GridInCellRenderer : GridGenericControlCellRenderer
    {
        private CellEmbeddedGrid activeGrid;

        /// <summary>
        /// Constructor for GridInCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public GridInCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this.SupportsFocusControl = true;
        }
        /// <summary>
        /// Is triggered when the cells are drawn
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            if (this.ShouldDrawFocused(rowIndex, colIndex))
            {
                if (style.Control is CellEmbeddedGrid)
                {
                    this.activeGrid = (CellEmbeddedGrid)style.Control;
                }

                base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
            }
            else
            {
                // Draw a static grid
                if (style.Control is CellEmbeddedGrid)
                {
                    CellEmbeddedGrid grid = (CellEmbeddedGrid) style.Control;
                    grid.DrawGrid(g, clientRectangle, true);
                }
            }
        }
        /// <summary>
        /// ProcessKeyEventArgs
        /// </summary>
        /// <param name="m">ref</param>
        /// <returns></returns>
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            TraceUtil.TraceCurrentMethodInfo(m.ToString());

            // forward keyboard events to child grid that would otherwise
            // be handled by parent grid (right arrow, page down etc.)
            if (this.activeGrid != null && this.activeGrid.Focused)
            {
                return this.activeGrid.InitiateProcessKeyEventArgs(ref m);
            }

            return base.ProcessKeyEventArgs(ref m);
        }
    }

    /// <summary>
    /// Implements a grid control that can be embedded in a grid cell. It is typically used with <see cref="GridInCellRenderer"/>.
    /// </summary>
    public class CellEmbeddedGrid : GridControl
    {
        /// <summary>
        /// Constructor for CellEmbeddedGrid.
        /// </summary>
        /// <param name="parent">The grid control.</param>
        public CellEmbeddedGrid(GridControl parent)
        {
            this.RowHeights[0] = this.RowHeights[1];
            this.DefaultColWidth = 50;

            this.FloatCellsMode = GridFloatCellsMode.OnDemandCalculation;
            this.VScrollBehavior = GridScrollbarMode.Automatic;
            this.HScrollBehavior = GridScrollbarMode.Automatic;
            this.BorderStyle = BorderStyle.None;
            this.Location = new Point(-10000, -10000);
            this.ActivateCurrentCellBehavior = GridCellActivateAction.PositionCaret;
            this.ShowCurrentCellBorderBehavior = GridShowCurrentCellBorder.GrayWhenLostFocus;
            this.VerticalThumbTrack = true;
            this.HorizontalThumbTrack = true;
            this.FillSplitterPane = false;
            this.Properties.GridLineColor = System.Drawing.Color.Silver;
            this.DefaultGridBorderStyle = GridBorderStyle.Solid;
        }
        /// <summary>
        /// Is Triggered when the key in pressed in Keyboard
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            TraceUtil.TraceCurrentMethodInfo(e.KeyCode);
            base.OnKeyDown(e);
        }

        internal bool InitiateProcessKeyEventArgs(ref Message m)
        {
            return base.ProcessKeyEventArgs(ref m);
        }
    }
}
