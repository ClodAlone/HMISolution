//-------------------------------------------------------------------------------------------------
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.Serialization;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{

    /// <summary>
    /// Implements a data model for HyperLinkLabel cell.
    /// </summary>
    public class PivotGridHyperlinkCellModel : GridStaticCellModel
    {
        protected PivotGridHyperlinkCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Constructor for HyperLinkLabelCellModel.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public PivotGridHyperlinkCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
        }

        /// <summary>
        /// Creates cell renderer.
        /// </summary>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotGridHyperlinkCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Implements the cell renderer for LinkLabel cell.
    /// </summary>
    public class PivotGridHyperlinkCellRenderer : GridStaticCellRenderer
    {
        private bool _isMouseDown;
        private bool _drawHotLink;
        private Color _hotColor;
        private Color _visitedColor;
        private string _EXEname;

        /// <summary>
        /// Constructor for LinkLabelCellRenderer.
        /// </summary>
        /// <param name="grid">The grid control.</param>
        /// <param name="cellModel">The cell model.</param>
        public PivotGridHyperlinkCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            this._isMouseDown = false;
            this._drawHotLink = false;

            this._hotColor = Color.Red;
            this._visitedColor = Color.Purple;

            this._EXEname = "iexplore.exe";
        }

        /// <summary>
        /// Gets or sets the color for visited link.
        /// </summary>
        public Color VisitedLinkColor
        {
            get
            {
                return this._visitedColor;
            }

            set
            {
                this._visitedColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the color for active link.
        /// </summary>
        public Color ActiveLinkColor
        {
            get
            {
                return this._hotColor;
            }

            set
            {
                this._hotColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the browser.
        /// </summary>
        public string EXEname
        {
            get
            {
                return this._EXEname;
            }

            set
            {
                this._EXEname = value;
            }
        }

        /// <summary>
        /// A method to launch the web browser.
        /// </summary>
        /// <param name="style">cell style</param>
        protected virtual void LaunchBrowser(GridStyleInfo style)
        {
            try
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = this.EXEname;
                process.StartInfo.Arguments = (string)style.Tag;
                process.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
        }

        private void DrawLink(bool useHotColor, int rowIndex, int colIndex)
        {
            if (useHotColor)
            {
                this._drawHotLink = true;
            }

            this.Grid.RefreshRange(GridRangeInfo.Cell(rowIndex, colIndex), GridRangeOptions.None);

            this._drawHotLink = false;
        }

        protected override void OnMouseDown(int rowIndex, int colIndex, System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(rowIndex, colIndex, e);
            this.DrawLink(true, rowIndex, colIndex);
            this._isMouseDown = true;
        }

        protected override void OnMouseUp(int rowIndex, int colIndex, System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(rowIndex, colIndex, e);
            int row, col;
            this.Grid.PointToRowCol(new Point(e.X, e.Y), out row, out col);
            if ((row == rowIndex) && (col == colIndex))
            {
                GridStyleInfo style = this.Grid.Model[row, col];
                this.LaunchBrowser(style);
                style.TextColor = this.VisitedLinkColor;
            }

            this.DrawLink(false, rowIndex, colIndex);
            this._isMouseDown = false;
        }

        protected override void OnCancelMode(int rowIndex, int colIndex)
        {
            base.OnCancelMode(rowIndex, colIndex);
            this._isMouseDown = false;
            this._drawHotLink = false;
        }

        protected override System.Windows.Forms.Cursor OnGetCursor(int rowIndex, int colIndex)
        {
            // if over cell, return HandPointerCursor otherwise NoCursor...
            Point pt = this.Grid.PointToClient(Cursor.Position);
            int row, col;
            this.Grid.PointToRowCol(pt, out row, out col);

            return (row == rowIndex && col == colIndex) ? Cursors.Hand : this._isMouseDown ? Cursors.No : base.OnGetCursor(rowIndex, colIndex);
        }

        protected override int OnHitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
            // return a nonzero so the mouse messages will be forwarded to the cell render
            // but don't include the cell borders so D&D can be handled
            if ((controller != null) && (controller.Name == "OleDataSource"))
            {
                // other controllers have higher priority than me
                return 0;
            }

            return 1;
        }

        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            style.Font.Underline = true;

            if (this._drawHotLink)
            {
                style.TextColor = this.ActiveLinkColor;
            }

            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }

        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            base.OnMouseHoverEnter(rowIndex, colIndex);
            this.DrawLink(true, rowIndex, colIndex);
        }

        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, System.EventArgs e)
        {
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
            this.DrawLink(false, rowIndex, colIndex);
        }
    }
}
