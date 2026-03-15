//-------------------------------------------------------------------------------------------------
// <copyright file="GridDataBoundTreeCell.cs" company="syncfusion">
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
    /// Defines a cell button element that looks like a + and - button for expanding and collapsing nodes in a tree. 
    /// Used with <see cref="GridDataBoundTreeCellRenderer"/> as it assumes it is in a hierarchical GridDataBoundGrid.
    /// </summary>
    public class GridDataBoundTreeCellButton : GridCellButton
    {
        /// <summary>
        /// Initializes a <see cref="GridDataBoundTreeCellButton"/> and associates it with a <see cref="GridCellRendererBase"/>.
        /// </summary>
        /// <param name="control">The <see cref="GridCellRendererBase"/> that draws this cell button element.</param>
        public GridDataBoundTreeCellButton(GridCellRendererBase control)
            : base(control)
        {
        }

        /// <override/>
        /// <summary>
        /// Draws the cell button element at the specified row and column index.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="bActive">True if this is the active current cell; False otherwise.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        public override void Draw(Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            object val = this.Grid.Model[rowIndex, 1].CellValue; ////assumes a hierarchical GridDataBoundGrid 

            if (val.ToString().Length == 0)
            {
                return;
            }

            // Draw the button.
            bool isHovering = IsHovering(rowIndex, colIndex);
            bool isMouseDown = IsMouseDown(rowIndex, colIndex);
            
            bool expanded = (bool)Convert.ChangeType(val, typeof(bool));
            bool disabled = !style.Clickable;

            Rectangle rect = Bounds;

            string bitmapName = string.Empty;
            if (disabled)
            {
                bitmapName = "SFEXPANDING.BMP";
            }
            else if (!isHovering && !isMouseDown)
            {
                if (!expanded)
                {
                    bitmapName = "SFEXPAND.BMP";
                }
                else
                {
                    bitmapName = "SFCOLLAPSE.BMP";
                }
            }
            else
            {
                if (!expanded)
                {
                    bitmapName = "SFEXPANDING.BMP";
                }
                else
                {
                    bitmapName = "SFCOLLAPSING.BMP";
                }
            }

            GridDataBoundIconPaint.Paint.PaintIcon(g, rect, Point.Empty, bitmapName, Color.Black);
        }
    }
    
    /// <summary>
    /// Implements the data / model part for an expandable row header cell in a <see cref="GridDataBoundGrid"/> that
    /// displays treelines. It is used exclusively with a hierarchical GridDataBoundGrid.
    /// The expandable row header cell will display a '+' for expanded rows and a '-' for collapsed rows similar to a TreeControl.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridDataBoundTreeCellModel"/> can serve as model for several <see cref="GridDataBoundTreeCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridDataBoundTreeCellModel"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridDataBoundTreeCellModel : GridTextBoxCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridDataBoundTreeCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridDataBoundTreeCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridDataBoundTreeCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
            base.ButtonBarSize = new Size(11, 11);
        }

        /// <summary>
        /// Initializes a new <see cref="GridDataBoundTreeCellModel"/> object 
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>    
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridDataBoundTreeCellModel(GridModel grid)
            : base(grid)
        {
        }

        private GridControlBase grid;
        private GridDataBoundTreeCellRenderer renderer;

        /// <override/>
        /// <summary>Creates a renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            this.grid = control;
            this.renderer = new GridDataBoundTreeCellRenderer(control, this);
            return this.renderer;
        }

        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size size = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            int indent = 36;
            if (this.grid != null)
            {
                GridBoundRecordState state = ((GridDataBoundGrid)this.grid).Binder.GetRecordStateAtRowIndex(rowIndex);
                indent = ((state.level + 1) * this.renderer.offSet) + this.renderer.midPt + this.renderer.leftSide + 2;
            }

            return new Size(size.Width + indent, size.Height);
        }
    }

    /// <summary>
    /// Implements the renderer part for an expandable row header cell in a <see cref="GridDataBoundGrid"/>. The
    /// The expandable row header cell will display a '+' for expanded rows and a '-' for collapsed rows with treelines similar to a TreeControl.
    /// </summary>
    /// <remarks>
    /// Defines the renderer part of a expandable row header cell. A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridDataBoundTreeCellRenderer"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The <see cref="GridDataBoundGrid"/> registers "DataBoundRowExpandCell" as identifier in <see cref="GridStyleInfo.CellType"/>
    /// of a cells <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// The following table lists some characteristics about the DataBoundRowExpandCell cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>DataBoundRowExpandCell</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridDataBoundTreeCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridDataBoundTreeCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Click Only</description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>No</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridCellRendererBase"/></description>
    ///     </item>
    /// </list>
    /// <para/>
    ///  <para/>
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized
    /// for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. To hide grid lines for a certain cell, you can
    /// set the <see cref="GridBorder.Style"/> of the specific edge to to be
    /// <see cref="GridBorderStyle.None"/>. By default, the right and bottom borders are initialized to
    /// <see cref="GridBorderStyle.Standard"/> and borders are drawn as specified in the
    /// <see cref="GridModelOptions.DefaultGridBorderStyle"/> property of a <see cref="GridModel"/> instance. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>When set to <see cref="GridCellAppearance.Flat"/>, the header will be drawn with slightly raised edges typical for cell headers. If the grid is XP Themes enabled the headers will be drawn with XP Themes look. If you specify Sunken or Raised, the header will be drawn with sunken or raised edges and not XP Themed. (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>DataBoundRowExpandCell (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as the3 current cell when the user clicks onto the header. Usually you do not want a header to be activated as the current cell unless you want to have editing capabilities such as allowing users to rename header text in place. Such renaming functionality needs to be implemented in a derived class. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description>Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. If the grid is XP Themes enabled, this color will be ignored and the header will be drawn with default XP Themes header background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color of the icon. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>When drawing this header cell this specifies the minimum empty area between the text rectangle without borders and the icon. The icon will be centered inside the remaining rectangle. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridDataBoundTreeCellRenderer : GridTextBoxCellRenderer
    {
        ////
        ////        private int offSet = 14;
        ////        private int leftSide = 4;
        ////        private int midPt = 5;
        ////        private int indentLen = 8;
        ////        private int centerLine = 4 + 5; //leftside + midPt;

        internal int offSet = 12;
        internal int leftSide = 4;
        internal int midPt = 5;
        internal int indentLen = 6;
        internal int centerLine = 4 + 5; ////leftside + midPt;

        private Rectangle innerBounds;
        private Color lineColor;

        /// <summary>
        /// Gets or sets color used for drawing the treelines.
        /// </summary>
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }

        /// <summary>
        /// Initializes a new <see cref="GridCellRendererBase"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase 
        /// and GridCellModelBase will be saved.</remarks>
        public GridDataBoundTreeCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(new GridDataBoundTreeCellButton(this));

            lineColor = Color.Black;

            dataBoundGrid = grid as GridDataBoundGrid;
        }

        private GridDataBoundGrid dataBoundGrid;

        /// <override/>
        /// <summary>
        /// Allows custom formatting of a cell by changing its style object.
        /// </summary>
        /// <param name="e">A reference to <see cref="GridPrepareViewStyleInfoEventArgs"/> that holds the event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            GridBoundRecordState state = dataBoundGrid.Binder.GetRecordStateAtRowIndex(e.Style.CellIdentity.RowIndex);
            if (state.RowIndexInRecord > 0)
            {
                e.Style.ShowButtons = GridShowButtons.Hide;
            }
            else
            {
                e.Style.HorizontalAlignment = GridHorizontalAlignment.Center;
                e.Style.VerticalAlignment = GridVerticalAlignment.Middle;
                object val = this.Grid.Model[e.Style.CellIdentity.RowIndex, 1].CellValue;
                e.Style.ShowButtons = Convert.ToInt32((val.ToString().Length > 0) ? val : "1") != -1 ? GridShowButtons.Show : GridShowButtons.Hide;
                e.Style.Clickable = true;
            }
        }

        /// <summary>
        /// This method is called from PerformLayout to calculate the client rectangle given
        /// the inner rectangle of a cell and any boundaries of cell buttons.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="innerBounds">The <see cref="System.Drawing.Rectangle"/> with the inner bounds of a cell.</param>
        /// <param name="buttonsBounds">An array of <see cref="System.Drawing.Rectangle"/> with bounds for each cell button element.</param>
        /// <returns>
        /// A <see cref="System.Drawing.Rectangle"/> with the bounds.
        /// </returns>
        /// <override/>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            ////            TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex, style, innerBounds, buttonsBounds);

            buttonsBounds[0] = GridUtil.CenterInRect(innerBounds, new Size(11, 11));
            ////return innerBounds;

            ////Rectangle rect = base.OnLayout(rowIndex, colIndex, style, innerBounds, buttonsBounds);
            buttonsBounds[0].X = innerBounds.X + leftSide;
            this.innerBounds = innerBounds;

            if (rowIndex > this.Grid.Model.Rows.HeaderCount)
            {
                ////Console.WriteLine(buttonsBounds[0]);
                GridBoundRecordState state = ((GridDataBoundGrid)this.Grid).Binder.GetRecordStateAtRowIndex(rowIndex);
                int leftSideButton = buttonsBounds[0].X + (state.LevelIndex * offSet);

                buttonsBounds[0].X = this.Grid.IsRightToLeft()
                        ? innerBounds.Right - leftSideButton + innerBounds.Left - buttonsBounds[0].Width
                        : buttonsBounds[0].X = leftSideButton;
            }

            return innerBounds; ////return rect;
        }

        /// <override/>
        /// <summary>
        /// Draws the content for specified cell.
        /// </summary>
        /// <param name="g">Graphics context.</param>
        /// <param name="cellRectangle">Cell rectangle.</param>
        /// <param name="rowIndex">Row index.</param>
        /// <param name="colIndex">Column index.</param>
        /// <param name="style">Cell style information.</param>
        public override void Draw(System.Drawing.Graphics g, System.Drawing.Rectangle cellRectangle, int rowIndex, int colIndex, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
            GridDataBoundGrid dbGrid = (GridDataBoundGrid)this.Grid;

            GridBoundRecordState state = dbGrid.Binder.GetRecordStateAtRowIndex(rowIndex);
            int indent = ((state.level + 1) * offSet) + this.midPt + this.leftSide + 2;
            if (dbGrid.IndentHierarchies)
            {
                indent -= indentLen;
            }

            style.TextMargins.Left = indent;
            style.HorizontalAlignment = GridHorizontalAlignment.Left;

            base.Draw(g, cellRectangle, rowIndex, colIndex, style);

            DrawTreeLines(g, rowIndex, this.GetButton(0).Bounds);
        }
        /// <override/>
        protected override bool OnQueryShowButtons(int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridBoundRecordState state = ((GridDataBoundGrid)this.Grid).Binder.GetRecordStateAtRowIndex(rowIndex);
            if ((state != null && !state.HasChildList)
                || (dataBoundGrid.EnableAddNew && rowIndex > this.dataBoundGrid.Model.RowCount - this.dataBoundGrid.Binder.GetHierarchyLevel(0).RowCountPerRecord))
            {
                return false;
            }

            return base.OnQueryShowButtons(rowIndex, colIndex, style) || Grid.IsPrinting(); ////required to show +/- cell
        }

        /// <override/>
        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, Syncfusion.Windows.Forms.Grid.GridStyleInfo style)
        {
           ////don't draw button on addnewrow
            GridDataBoundGrid dbGrid = (GridDataBoundGrid)this.Grid;

            if (dbGrid.Binder.AllowAddNew && rowIndex == dbGrid.Model.RowCount)
            {
                return;
            }

            base.OnDrawCellButton(button, g, rowIndex, colIndex, bActive, style);
        }

        private void DrawTreeLines(Graphics g, int rowIndex, Rectangle rect)
        {
            if (((GridDataBoundGrid)this.Grid).IndentHierarchies)
            {
                return;
            }

            if (dataBoundGrid.EnableAddNew && rowIndex > this.dataBoundGrid.Model.RowCount - this.dataBoundGrid.Binder.GetHierarchyLevel(0).RowCountPerRecord)
            {
                return;
            }

            if (rowIndex > this.Grid.Model.Rows.HeaderCount)
            {
                using (Pen p = new Pen(lineColor, 1))
                {
                    GridBoundRecordState state = dataBoundGrid.Binder.GetRecordStateAtRowIndex(rowIndex);
                    for (int level = 0; level <= state.LevelIndex; ++level)
                    {
                        int middle = this.innerBounds.Left + centerLine + (level * this.offSet);
                        if (this.Grid.IsRightToLeft())
                        {
                            middle = innerBounds.Right - middle + innerBounds.Left - 1;
                        }
                        
                        if (level < state.LevelIndex
                            && (level < state.LevelIndex || state.Position < state.Table.Count - 1))
                        {
                            GridBoundRecordState parentState = state.Parent;
                            while (parentState != null
                                && parentState.LevelIndex > level)
                            {
                                parentState = parentState.Parent;
                            }

                            if (parentState == null || parentState.Position < parentState.Table.Count - 1)
                            {
                                ////draw long line on outer levels                            
                                g.DrawLine(p, middle, this.innerBounds.Top, middle, this.innerBounds.Bottom);
                            }
                        }
                        else
                        {
                            if (state.RowIndexInRecord > 0)
                            {
                                ////draw straight vertical lines for multirowrecord rows
                                if (dataBoundGrid.IsExpandedAtRowIndex(rowIndex - state.RowIndexInRecord + 1))
                                {
                                    g.DrawLine(p, middle + this.offSet, this.innerBounds.Top, middle + this.offSet, this.innerBounds.Bottom);
                                }

                                if (state.Position == state.Table.Count - 1)
                                {
                                    return; ////skip last row in table
                                }

                                g.DrawLine(p, middle, this.innerBounds.Top, middle, this.innerBounds.Bottom);
                            }
                            else
                            {
                                int xOff = state.HasChildList ? 0 : this.indentLen - 1;
                                int yOff = state.HasChildList ? 0 : this.midPt;

                                ////draw the neck
                                if (level > 0 || state.Position > 0)
                                {
                                    g.DrawLine(p, middle, this.innerBounds.Top, middle, rect.Top + yOff);
                                }
////draw the leg
                                if (state.Position < state.Table.Count - 1) 
                                {
                                    g.DrawLine(p, middle, rect.Bottom - 1 - yOff, middle, this.innerBounds.Bottom);
                                }

                                ////draw left arm
                                if (this.Grid.IsRightToLeft())
                                {
                                    g.DrawLine(p, rect.Left - this.indentLen, rect.Top + midPt, rect.Left + xOff, rect.Top + midPt);
                                }
                                else
                                {
                                    g.DrawLine(p, rect.Right - 1 - xOff, rect.Top + midPt, rect.Right + this.indentLen, rect.Top + midPt);
                                }
////draw left arm bend
                                if (state.Expanded) 
                                {
                                    if (this.Grid.IsRightToLeft())
                                    {
                                        g.DrawLine(p, rect.Left - this.indentLen - 1, rect.Top + midPt, rect.Left - this.indentLen - 1, this.innerBounds.Bottom);
                                    }
                                    else
                                    {
                                        g.DrawLine(p, rect.Right + this.indentLen, rect.Top + midPt, rect.Right + this.indentLen, this.innerBounds.Bottom);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
