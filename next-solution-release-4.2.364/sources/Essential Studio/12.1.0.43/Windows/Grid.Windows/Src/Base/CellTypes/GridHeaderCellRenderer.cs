//-------------------------------------------------------------------------------------------------
// <copyright file="GridHeaderCellRenderer.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
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
using System.Windows.Forms;
using System.Text;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the renderer as part of a column or row header.
    /// </summary>
    /// <remarks>
    /// <para/>
    /// There can be several renderers
    /// associated with one <see cref="GridHeaderCellModel"/> if several views display the same.
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The header cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is true.
    /// <para/>
    /// <para/>
    /// The following table lists some characteristics about the header cell type.
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
    ///         <description><see cref="GridHeaderCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridHeaderCellModel"/></description>
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
    /// The cell's behavior and appearance can be customized with the following properties of the <see cref="GridStyleInfo"/> class.
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
    ///         <description>Header (Default: TextBox)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>If empty, the standard header text will be drawn as specified with the <see cref="GridModelOptions.NumberedRowHeaders"/> and <see cref="GridModelOptions.NumberedColHeaders"/> properties in <see cref="GridModel"/>. If <see cref="GridStyleInfo.CellValue"/> is not NULL, the cell value will be displayed as header text.  (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cell's value. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the header cell can be activated as current cell when the user clicks onto the header. Usually you do not want a header to be activated as current cell unless you want to have editing capabilities such as allowing user to rename header text in place. (You would have to implement a custom header cell for this.) (Default: true)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Error"/> (<see cref="System.String"/>)</term>
    ///         <description>Holds error information if a value can not be converted to the <see cref="System.Type"/> specified with <see cref="GridStyleInfo.CellValueType"/>. (Default: String.Empty)</description>
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
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand). When you enable hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not be displayed. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageIndex"/> (<see cref="System.Int32"/>)</term>
    ///         <description>Specifies an index for a image in the <see cref="GridStyleInfo.ImageList"/> of a <see cref="GridStyleInfo"/>
    /// instance. (Default: -1)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ImageList"/> (<see cref="System.Windows.Forms.ImageList"/>)</term>
    ///         <description>The <see cref="GridStyleInfo.ImageList"/> that holds a collection of images. Cells can choose images with the <see cref="GridStyleInfo.ImageIndex"/> property in a <see cref="GridStyleInfo"/>
    /// instance. (Default: null)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description>Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. If grid is XP Themes enabled, this color will be ignored and the header will be drawn with default XP Themes header background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MergeCell"/> (<see cref="GridMergeCellDirection"/>)</term>
    ///         <description>Specifies merge behavior for a individual cell when merging cells feature has been enabled in a <see cref="GridModel"/> with  <see cref="GridModelOptions.MergeCellsMode"/>. (Default: GridMergeCellDirection.None)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>If empty, the standard header text will be drawn as specified with the <see cref="GridModelOptions.NumberedRowHeaders"/> and <see cref="GridModelOptions.NumberedColHeaders"/> properties in <see cref="GridModel"/>. If <see cref="GridStyleInfo.CellValue"/> is not NULL, the cell value will be displayed as header text.  (Default: String.Empty)</description>
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
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set.  (Default: true)</description>
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
    ///         <description>Specifies if text should be wrapped when it does not fit into a single line. (Default: true)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridHeaderCellRenderer : GridStaticCellRenderer
    {
        ////private ThemedHeaderDrawing themedDrawing = null;
        GridRangeInfo hoverRange = GridRangeInfo.Empty;
        GridRangeInfo mouseDownRange = GridRangeInfo.Empty;
        bool mouseDown = false;
        bool inMouseDownRange = false;

        static readonly BrushInfo defaultInterior1 = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
        static readonly BrushInfo defaultInterior2 = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));

        /// <summary>
        /// Initializes a new GridHeaderCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase,
        /// and GridCellModelBase will be saved.</remarks>
        public GridHeaderCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AlwaysRaiseCellClick = false;
            this.WireGrid();
            this.SupportsEditing = false;
        }

        /// <summary>
        /// Sets up mouse event hooks with the grid.
        /// </summary>
        protected virtual void WireGrid()
        {
            ScrollControl grid = Grid.GetWindow() as ScrollControl;
        }

        /// <summary>
        /// Resets up mouse event hooks with the grid.
        /// </summary>
        protected virtual void UnwireGrid()
        {
            ScrollControl grid = Grid.GetWindow() as ScrollControl;
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ////                if (this.themedDrawing != null)
                ////                {
                ////                    this.themedDrawing.Dispose();
                ////                    this.themedDrawing = null;
                ////                }
                if (XPThemes.IsThemedOS)
                {
                    if (Grid != null)
                    {
                        this.UnwireGrid();
                    }
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// This method is called to determine whether the cell renderer wants to receive mouse events
        /// for the give cell at the given coordinates.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller requested to handle this mouse event.</param>
        /// <returns>
        /// Non-zero hit context value if you request to handle the mouse event; zero if you vote
        /// not to handle the mouse event.
        /// </returns>
        /// <override/>
        protected override int OnHitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
            // only process hovering messages.
            if (Control.MouseButtons != MouseButtons.None) 
            {
                return GridHitTestContext.None;
            }

            return GridHitTestContext.Cell;
        }

        /// <override/>
        protected override void OnMouseMove(int rowIndex, int colIndex, MouseEventArgs e)
        {
            base.OnMouseMove(rowIndex, colIndex, e);
        }

        /// <override/>
        protected override void OnMouseHover(int rowIndex, int colIndex, MouseEventArgs e)
        {
            bool isInvalidated = false;
            GridRangeInfo range = GridRangeInfo.Cell(rowIndex, colIndex);
            if (!range.IsEmpty)
            {
                Grid.Model.GetSpannedRangeInfo(range.Top, range.Left, out range);
            }

            GridStyleInfo style = this.Grid.Model[range.Top, range.Left];
            if (!style.Clickable || this.Grid.CellRenderers[style.CellType] != this)
            {
                range = GridRangeInfo.Empty;
            }

            if (!mouseDown)
            {
                if (IntelliMouseDragScroll.ActiveIntelliMouseDragScroll != null)
                {
                    range = GridRangeInfo.Empty;
                }

                if (!hoverRange.Equals(range))
                {
                    if (!hoverRange.IsEmpty)
                    {
                        this.Grid.InvalidateRange(this.hoverRange);
                        isInvalidated = true;
                    }

                    this.hoverRange = range;
                    if (!hoverRange.IsEmpty)
                    {
                        this.Grid.InvalidateRange(this.hoverRange);
                        isInvalidated = true;
                    }
                }
            }
            else
            {
                if (this.inMouseDownRange != mouseDownRange.Equals(range))
                {
                    this.inMouseDownRange = !inMouseDownRange;
                    this.Grid.InvalidateRange(this.mouseDownRange);
                    isInvalidated = true;
                }
            }

            base.OnMouseHover(rowIndex, colIndex, e);
            if (isInvalidated)
            {
                Grid.GetWindow().Update();
            }

            isInvalidated = false;
        }

        /// <override/>
        protected override void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            GridRangeInfo range = GridRangeInfo.Cell(rowIndex, colIndex);
            if (!range.IsEmpty)
            {
                Grid.Model.GetSpannedRangeInfo(range.Top, range.Left, out this.mouseDownRange);

                GridStyleInfo style = this.Grid.Model[range.Top, range.Left];
                if (!style.Clickable || this.Grid.CellRenderers[style.CellType] != this)
                {
                    this.mouseDownRange = GridRangeInfo.Empty;
                }
            }

            this.mouseDown = !mouseDownRange.IsEmpty;
            this.hoverRange = GridRangeInfo.Empty;
            if (this.mouseDown)
            {
                this.inMouseDownRange = true;
                this.Grid.InvalidateRange(this.mouseDownRange);
            }

            base.OnMouseDown(rowIndex, colIndex, e);
        }

        /// <override/>
        protected override void OnMouseUp(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (this.mouseDown)
            {
                this.mouseDown = false;
                this.Grid.InvalidateRange(this.mouseDownRange);
                this.mouseDownRange = GridRangeInfo.Empty;
            }

            base.OnMouseUp(rowIndex, colIndex, e);
        }

        /// <override/>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            if (!hoverRange.IsEmpty)
            {
                this.Grid.InvalidateRange(this.hoverRange);
                this.hoverRange = GridRangeInfo.Empty;
            }

            if (!mouseDownRange.IsEmpty)
            {
                this.Grid.InvalidateRange(this.mouseDownRange);
                this.mouseDownRange = GridRangeInfo.Empty;
                this.inMouseDownRange = false;
            }

            base.OnMouseHoverLeave(rowIndex, colIndex, e);
        }
        static GridIconPaint iconPainter;
        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridMargins margins = style.ReadOnlyTextMargins.ToMargins();
            if (Grid.IsRightToLeft())
            {
                margins = margins.SwapRightToLeft();
            }

            Rectangle textRectangle = GridMargins.RemoveMargins(clientRectangle, margins);
            Rectangle rc = clientRectangle;
            bool drawPressed = false;// GetMarkHeaderState(rowIndex, colIndex, style);
            GridProperties propertyObject = Grid.Model.Properties;
            Color hoverTextColor = Color.Empty, normalTextColor = Color.Empty, pressedTextColor = Color.Empty;
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                ((IThemeStyle)this.Grid.Model.Options.GridVisualStylesDrawing).GetHeaderTextColors(out normalTextColor, out hoverTextColor, out pressedTextColor);
            if (colIndex > 0 && (this.Grid.Selections.Ranges.AnyRangeIntersects(GridRangeInfo.Col(colIndex))
               || this.CurrentCell.ColIndex.Equals(colIndex)) && this.CurrentCell.IsActive && Grid.MarkColHeader)
                drawPressed = true;
            else if (Grid.Model.Rows.HeaderCount < rowIndex && (this.CurrentCell.IsActive && Grid.MarkRowHeader && (this.Grid.Selections.Ranges.AnyRangeIntersects(GridRangeInfo.Row(rowIndex))
               || this.CurrentCell.RowIndex.Equals(rowIndex))))
                drawPressed = true;
            // Draw button look (if not pressed).
            if (style.CellAppearance == GridCellAppearance.Flat && propertyObject.Buttons3D)
            {
                Color shadow = SystemColors.ControlDarkDark;

                if (!Grid.PrintingMode)
                {
                    ThemedHeaderDrawing.HeaderState state = ThemedHeaderDrawing.HeaderState.Normal;
                    GridRangeInfo cellRange = GridRangeInfo.Cell(rowIndex, colIndex);
                    Point mouseClientPosition = Grid.GetWindow().PointToClient(Control.MousePosition);
                    Rectangle cr = clientRectangle;
                    cr.Inflate(1, 1);
                    if (drawPressed && this.Grid.Model.Properties.MarkColHeader)
                        state = ThemedHeaderDrawing.HeaderState.Pressed;
                    else if (drawPressed && this.Grid.Model.Properties.MarkRowHeader)
                        state = ThemedHeaderDrawing.HeaderState.Pressed;  
                    if (cr.Contains(mouseClientPosition)
                        && Grid.GetWindow().ClientRectangle.Contains(mouseClientPosition))
                    {
                        if (this.hoverRange.Contains(cellRange))
                        {
                            state = ThemedHeaderDrawing.HeaderState.Hot;
                            style.TextColor = (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black) ? hoverTextColor : style.TextColor;
                        }

                        if (drawPressed || (this.inMouseDownRange && mouseDownRange.Contains(cellRange)))
                        {
                            state = ThemedHeaderDrawing.HeaderState.Pressed;
                            style.TextColor = (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black) ? pressedTextColor : style.TextColor;
                        }
                        if (style.TextColor == SystemColors.WindowText)
                            style.TextColor = SystemColors.ControlText;
                    }

                      if ((style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled || ((style.Themed && this.Grid.ThemesEnabled) && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))
                    {
                        ////TraceUtil.TraceCurrentMethodInfo(clientRectangle, rowIndex, colIndex, state, mouseClientPosition, clientRectangle, Grid.GetWindow().ClientRectangle, hoverRange, cellRange );

                        //////                        if (this.themedDrawing == null)
                        //////                            this.themedDrawing = new ThemedHeaderDrawing();
                        //////                        this.themedDrawing.DrawHeader(g, rc, state);
                        bool isGroupCaption = this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && style.CellIdentity.Info.StartsWith("GroupCaptionCell");
                        if ((this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black) && !isGroupCaption
                            && state == ThemedHeaderDrawing.HeaderState.Normal)
                        {
                            if (state == ThemedHeaderDrawing.HeaderState.Hot)
                                style.TextColor = hoverTextColor;
                            else if (((style.CellType == "CustomColumnHeaderCell") || (style.TextColor == Color.FromArgb(91, 91, 91) || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black) && state == ThemedHeaderDrawing.HeaderState.Normal))
                                style.TextColor = normalTextColor;
                            else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
                                style.TextColor = pressedTextColor;
                        }

                        if (isGroupCaption)
                        {
                            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && rowIndex >= this.Grid.TopRowIndex)
                            {
                                DrawMetroCaptionRowStyle(g, rc, state);
                            }
                            else
                            {
                                DrawMetroHeaderStyle(g, rc, state);
                                if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                                {
                                    if (state == ThemedHeaderDrawing.HeaderState.Hot)
                                        style.TextColor = hoverTextColor;
                                    else if (state == ThemedHeaderDrawing.HeaderState.Normal)
                                        style.TextColor = normalTextColor;
                                    else
                                        style.TextColor = pressedTextColor;
                                }
                            }
                        }
                        else
                            Grid.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(g, rc, state);

                        if (state != ThemedHeaderDrawing.HeaderState.Normal)
                        {
                            Grid.NotifyCellHighlighted(rowIndex, colIndex, style);
                        }
                    }
                    else
                    {
                        Color hilight = SystemColors.ControlLightLight;
                        if (!drawPressed)
                        {
                            // Draw Raised like a normal GridHeader (looks better
                            // when grid lines are turned on)
                            ////ControlPaint.DrawBorder3D(g, Rectangle.FromLTRB(rc.Left, rc.Top, rc.Right-1, rc.Bottom-1),
                            //    Border3DStyle.Raised, Border3DSide.All);
                            GridUtil.Draw3dFrame(g, rc.Left, rc.Top, rc.Right - 1, rc.Bottom - 1, 1, hilight, shadow);
                        }
                        else
                        {
                            ////GridPaintPattern.Draw3dFrame(g, rc.Left, rc.Top, rc.Right-1, rc.Bottom-1, 1,
                            //    hilight, shadow);
                            ////ControlPaint.DrawBorder(g, Rectangle.FromLTRB(rc.Left, rc.Top, rc.Right-1, rc.Bottom-1),
                            //    shadow, ButtonBorderStyle.Solid); //Border3DStyle.Flat, Border3DSide.All);
                            Brush br = new SolidBrush(shadow);
                            g.FillRectangle(br, Rectangle.FromLTRB(rc.Left, rc.Bottom - 1, rc.Right - 1, rc.Bottom));
                            g.FillRectangle(br, Rectangle.FromLTRB(rc.Right - 1, rc.Top, rc.Right, rc.Bottom));
                            br.Dispose();
                            Grid.NotifyCellHighlighted(rowIndex, colIndex, style);
                        }
                    }
                }
                else
                {
                    // Border between headers and cells when printing.
                    int nhr = Grid.InternalGetHeaderRows();
                    int nhc = Grid.InternalGetHeaderCols();
                    GridBorder border = new GridBorder(GridBorderStyle.Solid, Color.Black);

                    if (colIndex <= nhc || rowIndex <= nhr)
                    {
                        if (style.ReadOnlyBorders.Bottom.Style == GridBorderStyle.None
                            && propertyObject.DisplayHorzLines)
                        {
                            GridBorderPaint.DrawRectangle(g, border, clientRectangle, Color.White, GridBorderSide.Bottom);
                        }

                        if (style.ReadOnlyBorders.Right.Style == GridBorderStyle.None
                            && propertyObject.DisplayVertLines)
                        {
                            GridBorderPaint.DrawRectangle(g, border, clientRectangle, Color.White, Grid.IsRightToLeft() ? GridBorderSide.Right : GridBorderSide.Left);
                        }
                    }
                }
            }

            if (textRectangle.IsEmpty)
            {
                return;
            }

            if (drawPressed)
            {
                // Text will be moved to the bottom-right corner a bit.
                GridUtil.OffsetLeft(ref textRectangle, 1);
                GridUtil.OffsetTop(ref textRectangle, 1);
            }

            #region Header Theme Settings
            Color clrBottom = Color.Empty;
            Color clrRight = Color.Empty;
            Color clrInteriorFirst = Color.Empty;
            Color clrInteriorLast = Color.Empty;

            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast))
            {
                if (style.CellAppearance != GridCellAppearance.Flat)
                {
                    style.Interior = new BrushInfo(GradientStyle.Vertical, clrInteriorFirst, clrInteriorLast);
                }
            }

            if (this.CurrentCell.ErrorMessage != string.Empty)
            {
                if (iconPainter == null)
                {
                    iconPainter = GridIconPaint.GridPainter;
                }
                int textMargin = 15;               
                string bitmapName = "SFERROR.BMP";
                if (this.CurrentCell.HasCurrentCellAt(rowIndex) && (style.CellType == "RowHeaderCell" || style.CellType == "Header") && this.Grid.ShowRowHeaderErroricon)
                {                   
                    Rectangle iconBounds = Rectangle.FromLTRB(rc.Right - textMargin, rc.Top, rc.Right, rc.Bottom);
                    iconBounds.Offset(-2, 0);
                    iconPainter.PaintIcon(g, iconBounds, Point.Empty, bitmapName, Color.Black);
                }             
            }
            #endregion
       
            this.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);
        }

        /// <summary>
        /// Draws Metro header skins
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the header.</param>
        public void DrawMetroHeaderStyle(Graphics g, Rectangle rect, ThemedHeaderDrawing.HeaderState state)
        {
            //Check for empty headers
            if (rect.Height == 0 && rect.Width == 0)
                return;
            Color normalHeaderColor = Color.FromArgb(235, 235, 235);
            Color hoverHeaderColor = Color.FromArgb(253, 143, 0);
            Color pressedHeaderColor = Color.FromArgb(255, 180, 0);
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
            {
                ((IThemeStyle)this.Grid.Model.Options.GridVisualStylesDrawing).GetHeaderColors(out normalHeaderColor, out hoverHeaderColor, out pressedHeaderColor);
            }
            //Check for the current state of the header and paints the foreground accordingly.

            if (state == ThemedHeaderDrawing.HeaderState.Normal)
            {
                SolidBrush br = new SolidBrush(normalHeaderColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else if (state == ThemedHeaderDrawing.HeaderState.Hot)
            {
                SolidBrush br = new SolidBrush(hoverHeaderColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else
            {
                SolidBrush br = new SolidBrush(pressedHeaderColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
        }

        /// <summary>
        /// Draws Metro header skins for CaptionRowHeader
        /// </summary>
        /// <param name="g">A <see cref="Graphics"/> object.</param>
        /// <param name="rect">A <see cref="System.Drawing.Rectangle"/> object that represents the drawing area.</param>
        /// <param name="state">The current state of the header.</param>
        public void DrawMetroCaptionRowStyle(Graphics g, Rectangle rect, ThemedHeaderDrawing.HeaderState state)
        {
            //Check for empty headers
            if (rect.Height == 0 && rect.Width == 0)
                return;

            Color normalHeaderColor = Color.FromArgb(230, 230, 230);
            Color hoverHeaderColor = Color.FromArgb(94, 171, 222);
            Color pressedHeaderColor = Color.FromArgb(35, 130, 195);
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
            {
                ((IThemeStyle)this.Grid.Model.Options.GridVisualStylesDrawing).GetHeaderColors(out normalHeaderColor, out hoverHeaderColor, out pressedHeaderColor);
            }
            //Check for the current state of the header and paints the foreground accordingly.

            if (state == ThemedHeaderDrawing.HeaderState.Normal)
            {
                SolidBrush br = new SolidBrush(normalHeaderColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else if (state == ThemedHeaderDrawing.HeaderState.Pressed)
            {
                SolidBrush br = new SolidBrush(pressedHeaderColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
            else
            {
                SolidBrush br = new SolidBrush(hoverHeaderColor);
                g.FillRectangle(br, rect);
                br.Dispose();
            }
        }


        /// <override/>
        /// <summary>Allows custom formatting of a cell by changing its style object.</summary>
        /// <param name="e">Event data.</param>
        public override void OnPrepareViewStyleInfo(GridPrepareViewStyleInfoEventArgs e)
        {
            Color clrBottom = Color.Empty;
            Color clrRight = Color.Empty;
            Color clrInteriorFirst = Color.Empty;
            Color clrInteriorLast = Color.Empty;
            Color clrHeaderBottom = Color.Empty;
            GridBottomBorderWeight bottomBorderWeight = GridBottomBorderWeight.ExtraThin;
            int headerCount = this.Grid.InternalGetHeaderCols();
            bool isGroupCaption = e.Style.CellIdentity.Info.StartsWith("GroupCaptionCell");
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                ((IThemeStyle)this.Grid.Model.Options.GridVisualStylesDrawing).GetHeaderBottomBorderStyle(out clrHeaderBottom, out bottomBorderWeight);
            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast) &&
               ((e.Style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                      || ((e.Style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))))))
            {
                if (bottomBorderWeight != GridBottomBorderWeight.None && this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && this.Grid.GetType().Name != "GridGroupDropArea" && !isGroupCaption && e.ColIndex > headerCount)
                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, clrHeaderBottom, (GridBorderWeight)bottomBorderWeight);
                else
                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, clrBottom, GridBorderWeight.ExtraThin);
                e.Style.Borders.Right = new GridBorder(GridBorderStyle.Solid, clrRight, GridBorderWeight.ExtraThin);
            }
            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast) &&
               ((e.Style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                      || ((e.Style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))
                      && (e.RowIndex == 0 || isGroupCaption))
            {
                e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, clrBottom, GridBorderWeight.ExtraThin);
            }               
            if (Grid.PrintingMode)
            {
                int nhr = Grid.InternalGetHeaderRows();
                int nhc = Grid.InternalGetHeaderCols();

                if (!Grid.Model.Properties.BlackWhite)
                {
                    // Replace brush with SystemColors.Control if it is set to the
                    // gradient that is specified in GridControl.ResetBaseStylesMap:
                    //                    header.Interior = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
                    //                    rowHeader.Interior = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
                    BrushInfo br = e.Style.Interior;
                    if (br.Equals(defaultInterior1) || br.Equals(defaultInterior2))
                    {
                        e.Style.BackColor = SystemColors.Control;
                    }
                }

                if (e.ColIndex <= nhc || e.RowIndex <= nhr)
                {
                    GridBorder border = new GridBorder(GridBorderStyle.Solid, Color.Black);
                    e.Style.Borders.Right = border;
                    e.Style.Borders.Bottom = border;
                }
            }

            base.OnPrepareViewStyleInfo(e);
        }

        /// <override/>
        protected override void OnDrawCellBackground(GridDrawCellBackgroundEventArgs e)
        {
            if (Grid.PrintingMode && Grid.Model.Properties.BlackWhite)
            {
                return;
            }

            base.OnDrawCellBackground(e);
        }

        /// <summary>
        /// This method is called from <see cref="OnDraw"/> to draw the face text of the header cell after
        /// its background has been drawn.
        /// </summary>
        /// <param name="g">Points to the device context.</param>
        /// <param name="textRectangle">Specifies the text rectangle. It is the cell rectangle without buttons, borders, or text margins.</param>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <param name="style">A reference to the style object of the cell.</param>
        protected virtual void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            string displayText = String.Empty;

            int numColHeader = Grid.Model.Cols.HeaderCount;
            int numRowHeader = Grid.Model.Rows.HeaderCount;
            try
            {
                displayText = Model.GetFormattedOrActiveTextAt(rowIndex, colIndex, style);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
                ////Trace.WriteLineIf(Switches.ValueConversion.TraceWarning, ex.ToString());
            }

            if (GridUtil.IsEmpty(displayText))
            {
                string label = String.Empty;
                if (colIndex > numColHeader && rowIndex == 0 && Grid.Model.Options.NumberedColHeaders)
                {
                    label = GridRangeInfo.GetAlphaLabel(colIndex - numColHeader);
                }
                else if (rowIndex > numRowHeader && colIndex == 0 && Grid.Model.Options.NumberedRowHeaders)
                {
                    label = GridRangeInfo.GetNumericLabel(rowIndex - numRowHeader);
                }

                displayText = label;
            }

            if (displayText.Length > 0)
            {
                GridDrawCellDisplayTextEventArgs e = new GridDrawCellDisplayTextEventArgs(g, displayText, textRectangle, style);
                Grid.RaiseDrawCellDisplayText(e);
                if (!e.Cancel)
                {
                    textRectangle = e.TextRectangle;
                    displayText = e.DisplayText;
                    Font font = style.GdipFont;
                    Color textColor = Grid.PrintingMode && Grid.Model.Properties.BlackWhite ? Color.Black : style.TextColor;

                    Brush brText = new SolidBrush(textColor);

                    StringFormat format = new StringFormat();
                    format.LineAlignment = GridUtil.ConvertToStringAlignment(style.VerticalAlignment);
                    format.Alignment = GridUtil.ConvertToStringAlignment(style.HorizontalAlignment);
                    format.SetTabStops(0f, new float[] { 50 });
                    bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
                    if (isTextRightToLeft)
                    {
                        format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                    }

                    format.HotkeyPrefix = style.HotkeyPrefix;
                    format.Trimming = style.Trimming;

                    if (!style.WrapText)
                    {
                        format.FormatFlags = StringFormatFlags.NoWrap;
                    }

                    int orientation = style.ReadOnlyFont.Orientation;
                    bool isTextTop = Grid.SortIconPlacement == SortIconPlacement.Top;
                    if (isTextTop)
                        textRectangle = new Rectangle(textRectangle.X, (textRectangle.Y / 2) + 3, textRectangle.Width, textRectangle.Height);                    
                    if (orientation != 0)
                    {
                        // Let GDI+ do text rotation.
                        float angle = (float)orientation;
                        RotatePaint.DrawRotatedString(g, displayText, font, brText, textRectangle, format, angle);
                    }
                    else
                    {
                        if (!e.UseTextRenderer)
                            g.DrawString(displayText, font, brText, textRectangle, format);
                        else //Text renderer draw text(SD3608)
                        {
                            if (TextRenderer.MeasureText(g, displayText, font).Width < textRectangle.Width)
                                TextRenderer.DrawText(g, displayText, font, textRectangle, textColor);
                            else
                            {
                                string modDisplayText = displayText;
                                do
                                {
                                    modDisplayText = MeasureTextRenderer(g, modDisplayText, textRectangle, font);
                                }
                                while (!recurssioncheck);
                                TextRenderer.DrawText(g, modDisplayText, font, textRectangle, textColor);
                            }
                        }
                    }

                    brText.Dispose();
                    format.Dispose();
                }
            }
        }

        #region For wrapping header text renderer on e.UseTextRenderer (SD3608)
        private bool recurssioncheck = true;
        private string MeasureTextRenderer(Graphics g, string displayText, Rectangle textRectangle, Font font)
        {
            int templength = 0;
            string str = displayText;
            if (displayText.Contains("\r\n"))
            {
                string[] strarray = displayText.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                str = strarray[strarray.Length - 1];
                for (int i = 0; i < strarray.Length - 1; i++)
                {
                    templength += strarray[i].Length + 2;
                }
            }

            recurssioncheck = true;
            if (str != string.Empty)
            {
                for (int i = 0; i <= str.Length; i++)
                {
                    if (TextRenderer.MeasureText(g, str.Substring(0, i), font).Width > textRectangle.Width)
                    {
                        if (i > 1)
                            str = str.Insert(i - 1, "\r\n");
                        else
                            str = str.Insert(1, "\r\n");
                        recurssioncheck = false;
                        break;
                    }
                }
                str = displayText.Remove(templength) + str;
                return str;
            }
            return displayText;
        }
        #endregion

        /// <override/>
        protected /*internal*/ override void OnOutlineCurrentCell(Graphics g, Rectangle r)
        {
            GridShowCurrentCellBorder showBorder = Grid.Model.Options.ShowCurrentCellBorderBehavior;

            if (showBorder == GridShowCurrentCellBorder.HideAlways)
            {
                return;
            }

            bool gridFocused = Grid.HasControlFocus && !Grid.CurrentCell.StaticDrawing;
            bool drawGreyed = !(gridFocused || showBorder == GridShowCurrentCellBorder.AlwaysVisible);

            GridDrawCurrentCellBorderEventArgs e = new GridDrawCurrentCellBorderEventArgs(currentRowIndex, currentColIndex, g, r, gridFocused, StyleInfo, showBorder);

            // Give programmer chance to do own drawing (should set e.Cancel = true then).
            Grid.RaiseDrawCurrentCellBorder(e);

            if (!e.Cancel)
            {
                // Mark the header as current cell.
                ControlPaint.DrawFocusRectangle(g, r);
            }
        }
    }
}