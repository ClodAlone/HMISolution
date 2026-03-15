//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableHeaderCell.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows.Forms;

using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing.Drawing2D;

#if ASPNET
namespace Syncfusion.Web.UI.WebControls.Grid.Grouping
#else
namespace Syncfusion.Windows.Forms.Grid.Grouping
#endif
{
    /// <summary>
    /// Implements the DataModel part of a column or row header.
    /// </summary>
    /// <remarks>
    /// You typically access cell models through the <see cref="GridModel.CellModels"/>
    /// property of the <see cref="GridModel"/> class.<para/>
    /// A <see cref="GridTableHeaderCellModel"/> can serve as model for several <see cref="GridTableHeaderCellRenderer"/>
    /// instances if there are several <see cref="GridControlBase"/> views for a <see cref="GridModel"/>.
    /// <para/>
    /// See <see cref="GridTableHeaderCellRenderer"/> for more detailed information about this cell type.
    /// </remarks>
    [Serializable]
    public class GridTableHeaderCellModel: GridStaticCellModel
    {
        /// <overload>
        /// Initializes a new <see cref="GridTableHeaderCellModel"/> object.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridTableHeaderCellModel"/> object
        /// and stores a reference to the <see cref="GridModel"/> this cell belongs to.
        /// </summary>
        /// <param name="grid">The <see cref="GridModel"/> for this cell model.</param>
        /// <remarks>
        /// You typically access cell models through the <see cref="GridModel.CellModels"/>
        /// property of the <see cref="GridModel"/> class.
        /// </remarks>
        public GridTableHeaderCellModel(GridModel grid)
            : base(grid)
        {
            AllowMerging = true;
        }

        /// <summary>
        /// Initializes a new <see cref="GridTableHeaderCellModel"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected GridTableHeaderCellModel(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ////TraceUtil.TraceCurrentMethodInfoIf(Switches.Serialization.TraceVerbose, info.FullTypeName, info.MemberCount);
        }
#if ASPNET
#else
        /// <override/>
        /// <summary>Creates a cell renderer for this cell model.</summary>
        /// <param name="control">The grid control.</param>
        /// <returns>Cell renderer.</returns>
        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new GridTableHeaderCellRenderer(control, this);
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
            return new Size(size.Width+4, size.Height+2);
        }
    }

#if ASPNET
#else
    /// <summary>
    /// Implements the renderer part of a column or row header.
    /// </summary>
    /// <remarks>
    /// <para/>
    /// There can be several renderers
    /// associated with one <see cref="GridTableHeaderCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// The header cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
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
    ///         <description>When set to <see cref="GridCellAppearance.Flat"/>, the header will be drawn with slightly raised edges typical for cell headers. If the grid is XP Themes enabled, the headers will be drawn with an XP Themes look. If you specify Sunken or Raised, the header will be drawn with sunken or raised edges and not XP Themed. (Default: GridCellAppearance.Flat)</description>
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
    ///         <description>If empty, the standard header text will be drawn as specified with the <see cref="GridModelOptions.NumberedRowHeaders"/> and <see cref="GridModelOptions.NumberedColHeaders"/> properties in <see cref="GridModel"/>. If <see cref="GridStyleInfo.CellValue"/> is not NULL, the cell value will be displayed as header text. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CultureInfo"/> (<see cref="System.Globalization.CultureInfo"/>)</term>
    ///         <description>The culture information holds rules for parsing and formatting the cell's value. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the header cell can be activated as current cell when the user clicks onto the header. Usually you do not want a header to be activated as current cell unless you want to have editing capabilities such as allowing the user to rename header text in place. (You would have to implement a custom header cell for this.) (Default: True)</description>
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
    public class GridTableHeaderCellRenderer: GridStaticCellRenderer
    {
////        private ThemedHeaderDrawing themedDrawing = null;
        GridRangeInfo hoverRange = GridRangeInfo.Empty;
        GridRangeInfo mouseDownRange = GridRangeInfo.Empty;
        bool mouseDown = false;
        bool inMouseDownRange = false;

        /// <summary>
        /// Initializes a new GridTableHeaderCellRenderer object for the given GridControlBase
        /// and GridCellModelBase.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that displays this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCellModelBase"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase
        /// and GridCellModelBase will be saved.</remarks>
        public GridTableHeaderCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
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
        }

        /// <summary>
        /// Resets up mouse event hooks with the grid.
        /// </summary>
        protected virtual void UnwireGrid()
        {
        }
       /// <summary>
       /// Determine the dispose.
       /// </summary>
       /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
/*                if (this.themedDrawing != null)
////                {
////                    this.themedDrawing.Dispose();
////                    this.themedDrawing = null;
                }*/
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
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller requested to handle this mouse event.</param>
        /// <returns>
        /// Non-zero hit context value if you request to handle the mouse event; zero if you vote
        /// not to handle the mouse event.
        /// </returns>
        /// <override/>
        protected override int OnHitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
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
        protected override void OnCancelMode(int rowIndex, int colIndex)
        {
            this.Grid.InvalidateRange(GridRangeInfo.Cell(rowIndex, colIndex));
            base.OnCancelMode(rowIndex, colIndex);
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
            if (colIndex > 0 && (this.Grid.Selections.Ranges.AnyRangeIntersects(GridRangeInfo.Col(colIndex))
               || this.CurrentCell.ColIndex.Equals(colIndex)) && this.CurrentCell.IsActive && Grid.MarkColHeader)
                drawPressed = true;
            else if (this.CurrentCell.IsActive && Grid.MarkRowHeader && (this.Grid.Selections.Ranges.AnyRangeIntersects(GridRangeInfo.Row(rowIndex))
               || this.CurrentCell.RowIndex.Equals(rowIndex)))
                drawPressed = true;
            // Draw button look (if not pressed)
            if (style.CellAppearance == GridCellAppearance.Flat && propertyObject.Buttons3D)
            {
                Color shadow = SystemColors.ControlDarkDark;

                if (!Grid.PrintingMode)
                {
                    if ((style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                       || ((style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))  
                    {
                        Color hoverTextColor = Color.Empty, normalTextColor = Color.Empty, pressedTextColor = Color.Empty;
                        if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                            ((IThemeStyle)this.Grid.Model.Options.GridVisualStylesDrawing).GetHeaderTextColors(out normalTextColor, out hoverTextColor, out pressedTextColor);
                        ThemedHeaderDrawing.HeaderState state = ThemedHeaderDrawing.HeaderState.Normal;
                        GridRangeInfo cellRange = GridRangeInfo.Cell(rowIndex, colIndex);
                        Point mouseClientPosition = Grid.GetWindow().PointToClient(Control.MousePosition);
                        if (drawPressed && this.Grid.Model.Properties.MarkRowHeader )
                            state = ThemedHeaderDrawing.HeaderState.Pressed;
                        else if (drawPressed && this.Grid.Model.Properties.MarkColHeader)                        
                            state = ThemedHeaderDrawing.HeaderState.Pressed;                         
                        if ((!(Grid is GridTableControl) || ((GridTableControl)Grid).MouseOperationChildTable == ((GridTableControl)Grid).Table.FilteredChildTable)
                            && clientRectangle.Contains(mouseClientPosition)
                            && Grid.GetWindow().ClientRectangle.Contains(mouseClientPosition))
                        {
                            //// TODO: when user hovers mouse out of grid window it still stays active
                            if (this.hoverRange.Contains(cellRange))
                            {
                                state = ThemedHeaderDrawing.HeaderState.Hot;
                                style.TextColor = (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black) ? hoverTextColor : style.TextColor;
                            }

                            if ((drawPressed || this.inMouseDownRange) && mouseDownRange.Contains(cellRange))
                            {
                                state = ThemedHeaderDrawing.HeaderState.Pressed;
                                style.TextColor = (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black) ? pressedTextColor : style.TextColor;
                            }
                            if (style.TextColor == SystemColors.WindowText)
                                style.TextColor = SystemColors.ControlText;
                        }
                        else
                        {
                            if (this.hoverRange.Contains(cellRange))
                            {
                                this.hoverRange = GridRangeInfo.Empty;
                            }

                            if (this.inMouseDownRange && mouseDownRange.Contains(cellRange))
                            {
                                this.mouseDownRange = GridRangeInfo.Empty;
                            }
                        }
                        if ((this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro  || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                            && state == ThemedHeaderDrawing.HeaderState.Normal)
                        {
                            style.TextColor = normalTextColor;
                        }

                        bool isGroupCaption = this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && style.CellIdentity.Info.StartsWith("GroupCaptionRow");

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
                            //// Draw Raised like a normal GridHeader (looks better
                            //// when grid lines are turned on)
                            //// ControlPaint.DrawBorder3D(g, Rectangle.FromLTRB(rc.Left, rc.Top, rc.Right-1, rc.Bottom-1),
                            //// Border3DStyle.Raised, Border3DSide.All);
                            GridUtil.Draw3dFrame(
                                g, 
                                rc.Left, 
                                rc.Top, 
                                rc.Right-1, 
                                rc.Bottom-1, 
                                1,
                                hilight, 
                                shadow);
                        }
                        else
                        {
                            ////GridPaintPattern.Draw3dFrame(g, rc.Left, rc.Top, rc.Right-1, rc.Bottom-1, 1,
                            ////  hilight, shadow);
                            ////ControlPaint.DrawBorder(g, Rectangle.FromLTRB(rc.Left, rc.Top, rc.Right-1, rc.Bottom-1),
                            //// shadow, ButtonBorderStyle.Solid); //Border3DStyle.Flat, Border3DSide.All);
                            Brush br = new SolidBrush(shadow);
                            g.FillRectangle(br, Rectangle.FromLTRB(rc.Left, rc.Bottom-1, rc.Right-1, rc.Bottom));
                            g.FillRectangle(br, Rectangle.FromLTRB(rc.Right-1, rc.Top, rc.Right, rc.Bottom));
                            br.Dispose();
                        }
                    }
                }
                else
                {
                    GridBorder border = new GridBorder(GridBorderStyle.Solid, Color.Black);
                    if (style.ReadOnlyBorders.Bottom.Style == GridBorderStyle.None
                        && propertyObject.DisplayHorzLines)
                    {
                        GridBorderPaint.DrawRectangle(g, border, clientRectangle, Color.White, GridBorderSide.Bottom);
                    }

                    if (style.ReadOnlyBorders.Right.Style == GridBorderStyle.None
                        && propertyObject.DisplayVertLines)
                    {
                        GridBorderPaint.DrawRectangle(g, border, clientRectangle, Color.White, GridBorderSide.Right);
                    }
                }
            }

            if (textRectangle.IsEmpty)
            {
                return;
            }

            if (drawPressed)
            {
                // text will be moved to the bottom-right corner a bit
                GridUtil.OffsetLeft(ref textRectangle, 1);
                GridUtil.OffsetTop(ref textRectangle, 1);
            }
            if (this.CurrentCell.ErrorMessage != string.Empty)
            {
                GridTableCellStyleInfo cellStyle = (GridTableCellStyleInfo) style;

                if ((this.CurrentCell.HasCurrentCellAt(rowIndex) && this.Grid.ShowRowHeaderErroricon) && cellStyle.CellType == "RowHeaderCell")
                {
                    int textMargin = 15;
                    string bitmapName = "SFERROR.BMP"; 
                    Rectangle iconBounds = Rectangle.FromLTRB(textRectangle.Right - textMargin, textRectangle.Top, textRectangle.Right, textRectangle.Bottom);
                    iconBounds.Offset(-2, 0);
                    GridGroupingBitmaps.IconPainter.PaintIcon(g, iconBounds, Point.Empty, bitmapName, Color.Black);                    
                }
            }
            # region Header Theme Settings
            Color clrBottom = Color.Empty;
            Color clrRight = Color.Empty;
            Color clrInteriorFirst = Color.Empty;
            Color clrInteriorLast = Color.Empty;
            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast))
            {
                Rectangle rect = clientRectangle;
                rect.Inflate(1, 2);
                if (style.CellAppearance != GridCellAppearance.Flat)
                {
                    LinearGradientBrush br = new LinearGradientBrush(rect, clrInteriorFirst, clrInteriorLast, LinearGradientMode.Vertical);
                    g.FillRectangle(br, rect);
                    br.Dispose();
                }
            }
            #endregion

            this.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);
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
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                ((IThemeStyle)this.Grid.Model.Options.GridVisualStylesDrawing).GetHeaderBottomBorderStyle(out clrHeaderBottom, out bottomBorderWeight);
            bool isGroupCaption = this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && e.Style.CellIdentity.Info.StartsWith("GroupCaption");
            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast) &&
                ((e.Style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                       || ((e.Style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))))))
            {
                if (bottomBorderWeight != GridBottomBorderWeight.None && this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro && this.Grid.GetType().Name != "GridGroupDropArea" && !isGroupCaption && (e.Style.CellType == "ColumnHeaderCell" || e.Style.CellType == "StackedHeaderCell" || e.Style.CellType == "Header"))
                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, clrHeaderBottom, (GridBorderWeight)bottomBorderWeight);
                else
                    e.Style.Borders.Bottom = new GridBorder(GridBorderStyle.Solid, clrBottom, GridBorderWeight.Thin);
                e.Style.Borders.Right = new GridBorder(GridBorderStyle.Solid, clrRight, GridBorderWeight.Thin);
            }
            if (this.Grid.Model.Options.GridVisualStylesDrawing.GetHeaderBorderColors(out clrBottom, out clrRight, out clrInteriorFirst, out clrInteriorLast)
                && ((e.Style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                || ((e.Style.Themed && this.Grid.ThemesEnabled && ((this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)))))
                && e.RowIndex == 1 && !(this.Grid is GridNestedTableControl))
            {
                e.Style.Borders.Top = new GridBorder(GridBorderStyle.Solid, clrBottom, GridBorderWeight.Thin);
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
        /// <param name="textRectangle">Specifies the text rectangle. It is the cell rectangle without buttons and borders and text margins.</param>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <param name="style">A reference to the style object of the cell.</param>
        protected virtual void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            Font font = style.GdipFont;
            Color textColor = Grid.PrintingMode && Grid.Model.Properties.BlackWhite ? Color.Black : style.TextColor;

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

            GridDrawCellDisplayTextEventArgs e = new GridDrawCellDisplayTextEventArgs(g, displayText, textRectangle, style);
            Grid.RaiseDrawCellDisplayText(e);
            if (!e.Cancel)
            {
                Brush brText = new SolidBrush(textColor);

                StringFormat format = new StringFormat();
                format.LineAlignment = GridUtil.ConvertToStringAlignment(style.VerticalAlignment);
                format.Alignment = GridUtil.ConvertToStringAlignment(style.HorizontalAlignment);
                format.HotkeyPrefix = style.HotkeyPrefix;
                format.Trimming = style.Trimming;

                if (!style.WrapText)
                {
                    format.FormatFlags = StringFormatFlags.NoWrap;
                }

                int orientation = style.ReadOnlyFont.Orientation;
                bool isTextTop = Grid.SortIconPlacement == SortIconPlacement.Top;
                int margin = 3;
                if (isTextTop)
                    textRectangle = new Rectangle(textRectangle.X, textRectangle.Y, textRectangle.Width, textRectangle.Height + margin);
                if (orientation != 0)
                {
                    // Let GDI+ do text rotation.
                    float angle = (float)orientation;
                    RotatePaint.DrawRotatedString(
                        g,
                        displayText,
                        font,
                        brText,
                        textRectangle,
                        format,
                        angle);
                }
                else
                {
                    g.DrawString(displayText, font, brText, textRectangle, format);
                }

                brText.Dispose();
            }
        }

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

            GridDrawCurrentCellBorderEventArgs e = new GridDrawCurrentCellBorderEventArgs(
                RowIndex, ColIndex, g, r, gridFocused, StyleInfo, showBorder);

            // Give programmer chance to do own drawing (should set e.Cancel = true then).
            Grid.RaiseDrawCurrentCellBorder(e);

            if (!e.Cancel)
            {
                // Mark the header as current cell.
                ControlPaint.DrawFocusRectangle(g, r);
            }
        }
    }
#endif
}
