//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableRowHeaderCell.cs" company="syncfusion">
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
    /// Implements the DataModel part for a row header cell in a <see cref="GridTableModel"/>. The
    /// row header cell will display an arrow for the current row, a star for the append row, and
    /// a pencil when the row is being edited.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridTableRowHeaderCellModel"/> can serve as model for several <see cref="GridTableRowHeaderCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridTableRowHeaderCellModel"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridTableRowHeaderCellModel: GridTableHeaderCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridTableRowHeaderCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableRowHeaderCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableRowHeaderCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableRowHeaderCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableRowHeaderCellModel(GridModel grid)
            : base(grid)
        {
        }

#if ASPNET
#else
        /// <override/>
        /// <summary>Creates a cell renderer for this object.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>returns the Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridTableRowHeaderCellRenderer(control, this);
        }
#endif

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins
        /// and any buttons.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics context of the canvas.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="style">The Syncfusion.Windows.Forms.Grid.GridStyleInfo object that holds cell information.</param>
        /// <param name="queryBounds">The GridQueryBounds</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size size = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);
            return new Size(34, size.Height);
        }
    }
    #endregion

    #region CellRenderer
#if ASPNET
#else
    /// <summary>
    /// Implements the renderer part of a row header.
    /// </summary>
    /// <remarks>
    /// <para/>
    /// There can be several renderers
    /// associated with one <see cref="GridTableRowHeaderCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The header cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is true.
    /// <para/>
    /// <para/>
    /// The following table lists some characteristics about the Header cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>Header</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridTableHeaderCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridTableHeaderCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>Yes</description>
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
    ///         <description><see cref="GridStaticCellRenderer"/></description>
    ///     </item>
    /// </list>
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
    ///         <description>When set to <see cref="GridCellAppearance.Flat"/>, the header will be drawn with slightly raised edges typical for cell headers. If the grid is XP Themes enabled the headers will be drawn with XP Themes look. If you specify Sunken or Raised, the header will be drawn with sunken or raised edges and not XP Themed. (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>Header (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>If empty, the standard header text will be drawn as specified with the <see cref="GridModelOptions.NumberedRowHeaders"/> and <see cref="GridModelOptions.NumberedColHeaders"/> properties in <see cref="GridModel"/>. If <see cref="GridStyleInfo.CellValue"/> is not NULL, the cell value will be displayed as header text. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cell's value. (Default: NNULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the header cell can be activated as current cell when the user clicks onto the header. Usually you do not want a header to be activated as current cell unless you want to have editing capabilities such as allowing a user to rename header text in place. (You would have to implement a custom header cell for this.) (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value could not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Format"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the format mask for formatting the cell's value. You can specify numeric format strings,
    /// date format strings, or enumeration format strings as discussed in the section "Format Specifiers and Format Providers" of the .NET Framework Developers Guide (see ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconformatspecifiersformatproviders.htm) (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies horizontal alignment of text in the cell. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable a hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for an image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. (Default: -1)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageList"/> (<see cref="System.Windows.Forms.ImageList"/>)</term>
    ///         <description>The <see cref="GridStyleInfo.ImageList"/> that holds a collection of images. Cells can choose images with the <see cref="GridStyleInfo.ImageIndex"/> property in a <see cref="GridStyleInfo"/>
    /// instance. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description>Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. If grid is XP Themes enabled, this color will be ignored and the header will be drawn with default XP Themes header background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MergeCell"/> (<see cref="GridMergeCellDirection"/>)</term>
    ///         <description>Specifies merge behavior for an individual cell when the merging cell's feature has been enabled in a <see cref="GridModel"/> with <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>If empty, the standard header text will be drawn as specified with the <see cref="GridModelOptions.NumberedRowHeaders"/> and <see cref="GridModelOptions.NumberedColHeaders"/> properties in <see cref="GridModel"/>. If <see cref="GridStyleInfo.CellValue"/> is not NULL, the cell value will be displayed as header text. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color for drawing the cell text. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>Holds text margins in pixels. When drawing a cell, this specifies the empty area between the
    /// text rectangle and the client rectangle of the cell without borders and cell buttons. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// <para/>
    /// <para/>
    /// <para/>
    /// </remarks>
    public class GridTableRowHeaderCellRenderer : GridTableHeaderCellRenderer
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
        public GridTableRowHeaderCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            base.OnDraw(g, clientRectangle, rowIndex, colIndex, style);
        }

        /// <summary>
        /// Gets a reference to the parent grid.
        /// </summary> 
        public new GridTableControl Grid
        {
            get
            {
                return (GridTableControl) base.Grid;
            }
        }

        /// <summary>
        /// This method is called to determine whether the cell renderer wants to receive mouse events
        /// for the give cell at the given coordinates.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller requested to handle this mouse event.</param>
        /// <returns>
        /// Non-zero hit context value if you request to handle the mouse event; zero if you vote
        /// not to handle the mouse event.
        /// </returns>
        /// <override/>
        protected override int OnHitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
            return GridHitTestContext.None;
        }

        /// <override/>
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            ////TraceUtil.TraceCurrentMethodInfo(textRectangle, rowIndex, colIndex, Grid.Table.CurrentRecordManager.IsEditing );
            GridDrawCellDisplayTextEventArgs e = new GridDrawCellDisplayTextEventArgs(g, string.Empty, textRectangle, style);
            this.Grid.RaiseDrawCellDisplayText(e);

            if (!Grid.PrintingMode)
            {
                GridTableControl datagrid = this.Grid as GridTableControl;
                int numRowHeader = this.Grid.Model.Rows.HeaderCount;
                if (!(rowIndex > numRowHeader && colIndex == 0 && this.Grid.Model.Options.NumberedRowHeaders))
                {
                    if (!e.Cancel)
                    {
                        if (datagrid != null && datagrid.HasTable && !datagrid.PrintingMode) 
                        {
                            //// && !CurrentCell.StaticDrawing) 
                            string bitmapName = null;
                            if (colIndex == 0)
                            {
                                if (datagrid.Table.HasCurrentRecord && datagrid.Table.IsCurrentRecord(rowIndex))                                    
                                {
                                    ////&& datagrid.Table == datagrid.Table.DisplayElements[rowIndex].Table)
                                    ////TraceUtil.TraceCurrentMethodInfo(g.ClipBounds, textRectangle, rowIndex, colIndex, datagrid.ToString());
                                    if (datagrid.RightToLeft == RightToLeft.Yes)
                                    {
                                        if (datagrid.Table.CurrentRecord is AddNewRecord)
                                        {
                                            bitmapName = datagrid.Table.CurrentRecordManager.IsEditing ? "SFPENCILR.BMP" : "SFSTAR.BMP";
                                        }
                                        else
                                        {
                                            bitmapName = datagrid.Table.CurrentRecordManager.IsEditing ? "SFPENCILR.BMP" : "SFARROWR.BMP";
                                        }
                                    }
                                    else
                                    {
                                        if (datagrid.Table.CurrentRecord is AddNewRecord)
                                        {
                                            bitmapName = datagrid.Table.CurrentRecordManager.IsEditing ? "SFPENCIL.BMP" : "SFSTAR.BMP";
                                        }
                                        else
                                        {
                                            bitmapName = datagrid.Table.CurrentRecordManager.IsEditing ? "SFPENCIL.BMP" : "SFARROW.BMP";
                                        }
                                    }
                                }
                                else if (Record.GetParentRecord(datagrid.Table.DisplayElements[rowIndex]) is AddNewRecord)
                                {
                                    bitmapName = "SFSTAR.BMP";
                                }
                            }

                            if (bitmapName != null)
                            {
                                GridGroupingBitmaps.IconPainter.PaintIcon(g, textRectangle, Point.Empty, bitmapName, style.TextColor);
                            }

                            ////                g.DrawString(rowIndex.ToString(), style.GdipFont, new SolidBrush(style.TextColor), textRectangle, StringFormat.GenericDefault);
                        }
                    }
                }
            }
        }

        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            //// suppress click event - don't move current cell
        }

        /// <summary>
        /// Paint the specified bitmap substituting black pixels with a new color.
        /// </summary>
        /// <param name="g">A Graphic object used to draw the bitmap.</param>
        /// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
        /// <param name="offset">A Point that specifies pixel to offset the bitmap from its origin point.</param>
        /// <param name="bmp">The Bitmap to be drawn on the screen.</param>
        /// <param name="foreColor">The new color used to substitute black pixels.</param>
        /// <returns>A Rectangle which containts the boundary data of the drawn bitmap.</returns>
        /// <remarks>
        /// The PaintIcon routine
        /// will substitute black pixels of the original bitmap and draw them with the
        /// specified forecolor. The bitmap is centered inside the specified bounds.
        /// Use the offset if you want to display a "pressed button" state. If the button is
        /// pressed, specify offset = new Point(1, 1).
        /// </remarks>
        public static Rectangle PaintIcon(Graphics g, Rectangle bounds, Point offset, Bitmap bmp, Color foreColor)
        {
            return GridGroupingBitmaps.IconPainter.PaintIcon(g, bounds, offset, bmp, foreColor);
        }

        /// <summary>
        /// Load the bitmap from manifest and paint it substituting black pixels with a new color.
        /// </summary>
        /// <param name="g">A Graphic object used to draw the bitmap.</param>
        /// <param name="bounds">A Rectangle which contains the boundary data of the rectangle.</param>
        /// <param name="offset">A Point that specifies pixel to offset the bitmap from its origin point.</param>
        /// <param name="bitmapName">The name of the bitmap.</param>
        /// <param name="foreColor">The new color used to substitute black pixels.</param>        /// 
        /// <remarks>
        /// A Rectangle which contains the boundary data of the drawn bitmap.
        /// The PaintIcon routine
        /// will substitute black pixels of the original bitmap and draw them with the
        /// specified forecolor. The bitmap is centered inside the specified bounds.
        /// Use the offset if you want to display a "pressed button" state. If the button is
        /// pressed, specify offset = new Point(1, 1).
        /// </remarks>
        public static void PaintIcon(Graphics g, Rectangle bounds, Point offset, string bitmapName, Color foreColor)
        {
            GridGroupingBitmaps.IconPainter.PaintIcon(g, bounds, offset, bitmapName, foreColor);
        }
    }
#endif
    #endregion
}
