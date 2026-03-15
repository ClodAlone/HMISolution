//-------------------------------------------------------------------------------------------------
// <copyright file="GridDataBoundRowHeaderCellRenderer.cs" company="syncfusion">
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
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the renderer part of a row header cell in a <see cref="GridDataBoundGrid"/>. The
    /// row header cell will display an arrow for the current row, a star for the append row, and
    /// a pencil when the row is being edited.
    /// </summary>
    /// <remarks>
    /// Defines the renderer part of a row header cell. A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridDataBoundRowHeaderCellRenderer"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The header cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// <para/>
    /// The <see cref="GridDataBoundGrid"/> registers "RowHeaderCell" as identifier in <see cref="GridStyleInfo.CellType"/>
    /// of a cell's <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// <para/>
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>PropertyName</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.BaseStyle"/> (<see cref="System.String"/>)</term>
    ///         <description>The base style for this style instance with default values for properties that are not initialized for this style object. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Borders"/> (<see cref="GridBordersInfo"/>)</term>
    ///         <description>Top, left, bottom, and right border settings. (Default: GridBordersInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellAppearance"/> (<see cref="GridCellAppearance"/>)</term>
    ///         <description>When set to <see cref="GridCellAppearance.Flat"/>, the header will be drawn with slightly raised edges typical for cell headers. If the grid is XP Themes enabled, the headers will be drawn with XP Themes look. If you specify Sunken or Raised, the header will be drawn with sunken or raised edges and not XP Themed. (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>RowHeaderCell (Default: TextBox)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as the current cell when the user clicks onto the header. Usually you do not want a header to be activated as the current cell unless you want to have editing capabilities like allowing user to rename header text in place. Such renaming functionality needs to be implemented in a derived class. (Default: true)</description>
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
    ///         <description>Lets you specify the color of the arrow, pencil, or star icon. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>When drawing this header cell this specifies the minimum empty area between the text rectangle without borders and the icon. The icon will be centered inside the remaining rectangle. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set.  (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridDataBoundRowHeaderCellRenderer : GridHeaderCellRenderer
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
        public GridDataBoundRowHeaderCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }

        /// <override/>
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridDataBoundGrid datagrid = Grid as GridDataBoundGrid;
            if (datagrid != null && !datagrid.PrintingMode && !CurrentCell.StaticDrawing)
            {
                string bitmapName = null;
                if (datagrid.binder.RecordEqualsAtRowIndex(datagrid.binder.CurrentRowIndex, rowIndex))
                {
                    if (datagrid.IsRightToLeft())
                    {
                        if (colIndex == 0)
                        {
                            if (rowIndex == datagrid.Model.RowCount && datagrid.Binder.SupportsAddNew)
                            {
                                bitmapName = "SFSTAR.BMP";
                            }
                            else
                            {
                                bitmapName = datagrid.Binder.IsEditing ? "SFPENCILR.BMP" : "SFARROWR.BMP";
                            }
                        }
                    }
                    else
                    {
                        if (colIndex == 0)
                        {
                            if (rowIndex == datagrid.Model.RowCount && datagrid.Binder.SupportsAddNew)
                            {
                                bitmapName = "SFSTAR.BMP";
                            }
                            else
                            {
                                bitmapName = datagrid.Binder.IsEditing ? "SFPENCIL.BMP" : "SFARROW.BMP";
                            }
                        }
                    }
                }

                if (bitmapName != null)
                {
                    GridDataBoundIconPaint.Paint.PaintIcon(g, textRectangle, Point.Empty, bitmapName, style.TextColor);
                }
            }
        }

        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            GridDataBoundGrid datagrid = Grid as GridDataBoundGrid;
            if (rowIndex > 0 && this.Grid != null && this.Grid.Model != null)
                datagrid.binder.SetCurrentPosition(rowIndex - this.Grid.Model.Rows.HeaderCount - 1, true);
            base.OnClick(rowIndex, colIndex, e);
        }

        /// <summary>
        /// Paint the specified bitmap substituting black pixels with a new color.
        /// </summary>
        /// <param name="g">A Graphics object used to draw the bitmap.</param>
        /// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
        /// <param name="offset">A Point that specifies pixels to offset the bitmap from its origin point.</param>
        /// <param name="bmp">The Bitmap to be drawn on the screen.</param>
        /// <param name="foreColor">The new color used to substitute black pixels.</param>
        /// <returns>A Rectangle which contains the boundary data of the drawn bitmap.</returns>
        /// <remarks>
        /// The PaintIcon routine
        /// will substitute black pixels of the original bitmap and draw them with the
        /// specified forecolor. The bitmap is centered inside the specified bounds. 
        /// Use the offset if you want to display a "pressed button" state. If the button is
        /// pressed, specify offset = new Point(1, 1).
        /// </remarks>
        public static Rectangle PaintIcon(Graphics g, Rectangle bounds, Point offset, Bitmap bmp, Color foreColor)
        {
            return GridDataBoundIconPaint.Paint.PaintIcon(g, bounds, offset, bmp, foreColor);
        }

        /// <summary>
        /// Load the bitmap from manifest and paint it substituting black pixels with a new color.
        /// </summary>
        /// <param name="g">A Graphics object used to draw the bitmap.</param>
        /// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
        /// <param name="offset">A Point that specifies pixels to offset the bitmap from its origin point.</param>
        /// <param name="bitmapName">The name of the bitmap.</param>
        /// <param name="foreColor">The new color used to substitute black pixels.</param>
        /// <remarks>
        /// The PaintIcon routine
        /// will substitute black pixels of the original bitmap and draw them with the
        /// specified forecolor. The bitmap is centered inside the specified bounds. 
        /// Use the offset if you want to display a "pressed button" state. If the button is
        /// pressed, specify offset = new Point(1, 1).
        /// </remarks>
        public static void PaintIcon(Graphics g, Rectangle bounds, Point offset, string bitmapName, Color foreColor)
        {
            GridDataBoundIconPaint.Paint.PaintIcon(g, bounds, offset, bitmapName, foreColor);
        }
    }
}
