//-------------------------------------------------------------------------------------------------
// <copyright file="GridCellLinkLabel.cs" company="Syncfusion">
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
    using Syncfusion.Drawing;
    using Syncfusion.Windows.Forms.Grid;
    using Syncfusion.Windows.Forms;

    /// <summary>
    /// Implements a data model for LinkLabel cell.
    /// </summary>
    public class LinkLabelCellModel : GridStaticCellModel
    {
        /// <summary>
        /// constructor for LinkLabelCellModel class
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected LinkLabelCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Constructor for LinkLabelCellModel.
        /// </summary>
        /// <param name="grid">The grid model.</param>
        public LinkLabelCellModel(GridModel grid)
            : base(grid)
        {
            AllowFloating = false;
        }

        /// <summary>
        /// Creates cell renderer.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new LinkLabelCellRenderer(control, this);
        }
    }

    /// <summary>
    /// Implements the cell renderer for LinkLabel cell.
    /// </summary>
    public class LinkLabelCellRenderer : GridStaticCellRenderer
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
        public LinkLabelCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
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
                MessageBoxAdv.Show(SR.GetString(SR.Error) + ex.ToString());
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
        /// <summary>
        /// Is triggeres when the mouse is pressed down
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="e">MouseEventArgs</param>
        protected override void OnMouseDown(int rowIndex, int colIndex, System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(rowIndex, colIndex, e);
            this.DrawLink(true, rowIndex, colIndex);
            this._isMouseDown = true;
        }
        /// <summary>
        /// Is triggered When the mose button is released
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="e">MouseEventArgs</param>
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
        /// <summary>
        /// Is t
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        protected override void OnCancelMode(int rowIndex, int colIndex)
        {
            base.OnCancelMode(rowIndex, colIndex);
            this._isMouseDown = false;
            this._drawHotLink = false;
        }
        /// <summary>
        /// Is triggered when the cursor is captured
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <returns>Cursor</returns>
        protected override System.Windows.Forms.Cursor OnGetCursor(int rowIndex, int colIndex)
        {
            // if over cell, return HandPointerCursor otherwise NoCursor...
            Point pt = this.Grid.PointToClient(Cursor.Position);
            int row, col;
            this.Grid.PointToRowCol(pt, out row, out col);

            return (row == rowIndex && col == colIndex) ? Cursors.Hand : this._isMouseDown ? Cursors.No : base.OnGetCursor(rowIndex, colIndex);
        }
        /// <summary>
        /// Is triggered when the cursor is hit and actions are performed.
        /// </summary>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="e">MouseEventArgs</param>
        /// <param name="controller"></param>
        /// <returns>int</returns>
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
        /// <summary>
        /// Is triggered when the cells are drawn in grid
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <param name="clientRectangle">Rectangle</param>
        /// <param name="rowIndex">int</param>
        /// <param name="colIndex">int</param>
        /// <param name="style">GridStyleInfo</param>
        protected override void OnDraw(System.Drawing.Graphics g, System.Drawing.Rectangle clientRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            style.Font.Underline = true;

            if (this._drawHotLink)
            {
                style.TextColor = this.ActiveLinkColor;
            }

            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }
        /// <summary>
        /// Occurs when mouse hover enters the column header.
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="colIndex">col index</param>
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
            base.OnMouseHoverEnter(rowIndex, colIndex);
            this.DrawLink(true, rowIndex, colIndex);
        }
        /// <summary>
        /// Occurs when mouse leaves the area.
        /// </summary>
        /// <param name="rowIndex">row index</param>
        /// <param name="colIndex">col index</param>
        /// <param name="e">EventArgs</param>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, System.EventArgs e)
        {
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
            this.DrawLink(false, rowIndex, colIndex);
        }
    }
}
