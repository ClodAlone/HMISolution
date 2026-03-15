//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableColumnHeader.cs" company="syncfusion">
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    /// <summary>
    /// Implements the DataModel part of a column header with sort indicator.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    [Serializable]
    public class PivotGridColumnHeaderCellModel: GridHeaderCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridTableColumnHeaderCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableColumnHeaderCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public PivotGridColumnHeaderCellModel(GridModel grid)
            : base(grid)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableColumnHeaderCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected PivotGridColumnHeaderCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }

        /// <summary>
        /// This is called from GridStyleInfo.GetFormattedText.
        /// </summary>
        /// <param name="style">The <see cref="GridStyleInfo"/> object that holds cell information.</param>
        /// <param name="value">The value to format.</param>
        /// <param name="textInfo">textInfo is a hint who is calling, default is GridCellBaseTextInfo.DisplayText</param>
        /// <returns>The formatted text for the given value.</returns>
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

#if ASPNET
#else
        /// <override/>
        /// <summary>
        /// Creates a cell renderer for this cell model.
        /// </summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotGridColumnHeaderCellRenderer(control, this);
        }
#endif

        /// <summary>
        /// Calculates the preferred size of the cell based on its contents without margins
        /// and any buttons.
        /// </summary>
        /// <param name="g">The System.Drawing.Graphics context of the canvas</param>
        /// <param name="rowIndex">The row index</param>
        /// <param name="colIndex">The column index</param>
        /// <param name="style">The Syncfusion.Windows.Forms.Grid.GridStyleInfo object that holds cell information.</param>
        /// <param name="queryBounds">The grid query bounds</param>
        /// <returns>The optimal size of the cell.</returns>
        /// <override/>
        protected override Size OnQueryPrefferedClientSize(Graphics g, int rowIndex, int colIndex, GridStyleInfo style, GridQueryBounds queryBounds)
        {
            Size size = base.OnQueryPrefferedClientSize(g, rowIndex, colIndex, style, queryBounds);

            size.Width += 12; //// for sort triangle
            return size;
        }
    }
#if ASPNET
#else
    /// <summary>
    /// Implements the renderer part of a column header with sort indicator.
    /// </summary>
    /// <remarks>
    /// The header cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// <para/>
    /// The <see cref="GridTableControl"/> registers "ColumnHeaderCell" as identifier in <see cref="GridStyleInfo.CellType"/>
    /// of a cell's <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// The sort indicator is defined through the <see cref="GridStyleInfo.Tag"/> of a cell's <see cref="GridStyleInfo"/>. The
    /// <see cref="GridStyleInfo.Tag"/> will be cast to <see cref="ListSortDirection"/>.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridTableColumnHeaderCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// <para/>
    /// The following table lists some characteristics about the TableColumnHeader cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>ColumnHeaderCell</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridTableColumnHeaderCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridTableColumnHeaderCellModel"/></description>
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
    ///         <description><see cref="GridHeaderCellRenderer"/></description>
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
    ///         <description>When set to <see cref="GridCellAppearance.Flat"/>, the header will be drawn with slightly raised edges typical for cell headers. If the grid is XP Themes enabled, the headers will be drawn with XP Themes look. If you specify Sunken or Raised, the header will be drawn with sunken or raised edges and not XP Themed. (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>ColumnHeaderCell (Default: TextBox)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>If empty, the standard header text will be drawn as specified with the <see cref="GridModelOptions.NumberedRowHeaders"/> and <see cref="GridModelOptions.NumberedColHeaders"/> properties in <see cref="GridModel"/>. If <see cref="GridStyleInfo.CellValue"/> is not NULL the cell value will be displayed as header text. (Default: String.Empty)</description>
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
    ///         <description>Specifies if the header cell can be activated as current cell when the user clicks onto the header. Usually you do not want a header to be activated as current cell unless you want to have editing capabilities like allowing user to rename header text in place. (You would have to implement a custom header cell for this.) (Default: True)</description>
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
    ///         <description>Gets / sets the format mask for formatting the cell value. You can specify numeric format strings,
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
    ///         <term><see cref="GridStyleInfo.Tag"/> (<see cref="System.Object"/>)</term>
    ///         <description>The sort indicator is defined through the <see cref="GridStyleInfo.Tag"/> of a cell's <see cref="GridStyleInfo"/>. The
    ///  <see cref="GridStyleInfo.Tag"/> will be cast to <see cref="ListSortDirection"/>. (Default: NULL)</description>
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
    public class PivotGridColumnHeaderCellRenderer : PivotGridHeaderCellRenderer
    {
        /// <summary>
        /// Initializes a new GridTableColumnHeaderCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public PivotGridColumnHeaderCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
        }

        /// <override/>
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            // No arrow needed when printing.
            object tag = style.Tag;
            if (Grid.PrintingMode || !(tag is ListSortDirection))
            {
                tag = null;
            }

            ListSortDirection listSortDirection = ListSortDirection.Ascending;
            int margin = 0;
            if (tag != null)
            {
                listSortDirection = (ListSortDirection) tag;
                margin = 12;
            }

            if (Grid.IsRightToLeft())
            {
                GridUtil.OffsetLeft(ref textRectangle, margin);
            }
            else
            {
                textRectangle.Width -= margin;
            }

            base.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);

            if (tag != null)
            {
                string s = style.ValueMember; // Multi-column sort position is set in ValueMember (didn't want to add
                // an extra property for this and also did not want to break backward-compatibility and change Tag ...)
                int dig = (!string.IsNullOrEmpty(s) && s.Length > 1) ? 2 : 1;

                Rectangle rect;
                if (Grid.IsRightToLeft())
                {
                    rect = new Rectangle(textRectangle.Left - margin, textRectangle.Y, 10 * dig, textRectangle.Height);
                }
                else
                {
                    rect = new Rectangle(textRectangle.Right, textRectangle.Y, 10 * dig, textRectangle.Height);
                }

                rect = GridUtil.CenterInRect(rect, new Size(8 * dig, 8));

                //// Code used in GridTable.SetupColumnHeaderCell:
                /*
                    //// Sort Indicator
                    int index = TableDescriptor.SortedColumns.IndexOf(e.Column.MappingName);
                    SortColumnDescriptor sd = null;
                    if (index != -1)
                    {
                        sd = TableDescriptor.SortedColumns[e.Column.MappingName];
                        if (TableDescriptor.TableOptions.AllowMultiColumnSort && TableDescriptor.SortedColumns.Count > 1)
                            style.ValueMember = index.ToString();
                    }
                */

                if (s != string.Empty)
                {
                    //// Draw sort-position slightly to the right and above.
                    rect.Offset(-4 * (dig - 1),  -4);
                    Font numFont = new Font(style.Font.Facename, 7, style.Font.FontStyle);
                    DrawText(g, s, numFont, rect, style, SystemColors.ControlDark, Grid.IsRightToLeft());
                    numFont.Dispose();
                    rect.Offset(4 * (dig - 1), 4);

                    if (Grid.IsRightToLeft())
                    {
                        rect.Offset(6, 0);
                    }
                    else
                    {
                        rect.Offset(-6, 0);
                    }
                }

                //// Draw Sort-Arrow
                ////Brush brush = new SolidBrush(SystemColors.ControlDark);
                Brush brush = null;
                Pen pen1 = null;

                this.Grid.Model.Options.GridVisualStylesDrawing.GetSortIconBrush(out brush, out pen1);

                int i2 = Math.Max(0, (rect.Height - 3) / 2);
                rect.Inflate(-i2, -i2);
                ////Pen pen1 = new Pen(SystemColors.WindowFrame);
                GridTriangleDirection triangleDirection = listSortDirection == ListSortDirection.Ascending ? GridTriangleDirection.Up : GridTriangleDirection.Down;
                GridPaintTriangle.Paint(g, rect, triangleDirection, brush, pen1, true);
                pen1.Dispose();
                brush.Dispose();
            }
        }

        /// <override/>
        protected override void OnClick(int rowIndex, int colIndex, MouseEventArgs e)
        {
            // suppress click event - don't move current cell
        }
    }
#endif
}
