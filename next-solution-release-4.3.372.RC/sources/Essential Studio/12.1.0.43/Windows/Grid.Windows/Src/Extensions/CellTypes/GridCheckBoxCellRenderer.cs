//-------------------------------------------------------------------------------------------------
// <copyright file="GridCheckBoxCellRenderer.cs" company="syncfusion">
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
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using Syncfusion.Diagnostics;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Implements the renderer part for a check box cell.
    /// </summary>
    /// <remarks>
    /// A <see cref="GridCheckBoxCellRenderer"/> cell's behavior can be customized with the
    /// <see cref="GridStyleInfo.CheckBoxOptions"/> property of a <see cref="GridStyleInfo"/>
    /// instance where you can specify values for Checked, Unchecked, and Indeterminated. The
    /// <see cref="GridStyleInfo.TriState"/> property of a <see cref="GridStyleInfo"/> instance
    /// lets you toggle tri-state behavior for the cell.
    /// <para/>
    /// <para/> 
    /// <para/>
    /// The check box cell is XP Themes enabled. It will be drawn themed if <see cref="GridControlBase.ThemesEnabled"/> is True.
    /// <para/>
    /// A renderer is created for each <see cref="GridCellModelBase"/>
    /// and <see cref="GridControlBase"/>. There can be several renderers
    /// associated with one <see cref="GridCheckBoxCellModel"/> if several views display the same
    /// <see cref="GridModel"/>.
    /// <para/>
    /// Use "Check Box" as identifier in <see cref="GridStyleInfo.CellType"/> of a cell's <see cref="GridStyleInfo"/>
    /// to associate this cell type with a cell.
    /// <para/>
    /// The following table lists some characteristics about the Check Box cell type:
    /// <para/>
    /// <list type="table">
    ///     <listheader>
    ///         <term>Item</term>
    ///         <description>Description</description>
    ///     </listheader>
    ///     <item>
    ///         <term>CellType</term>
    ///         <description>Check Box</description>
    ///     </item>
    ///     <item>
    ///         <term>Renderer</term>
    ///         <description><see cref="GridCheckBoxCellRenderer"/></description>
    ///     </item>
    ///     <item>
    ///         <term>Model</term>
    ///         <description><see cref="GridCheckBoxCellModel"/></description>
    ///     </item>
    ///     <item>
    ///         <term>XP Themes Support</term>
    ///         <description>Yes</description>
    ///     </item>
    ///     <item>
    ///         <term>Interactive</term>
    ///         <description>Edit with Mouse Click or SpaceBar</description>
    ///     </item>
    ///     <item>
    ///         <term>Floating Support</term>
    ///         <description>Floating</description>
    ///     </item>
    ///     <item>
    ///         <term>Base Type</term>
    ///         <description><see cref="GridCellRendererBase"/></description>
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
    ///         <description>Specifies if cell edges shall be drawn raised, sunken, or flat (default). (Default: GridCellAppearance.Flat)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellTipText"/> (<see cref="System.String"/>)</term>
    ///         <description>ToolTip text to be displayed when user hovers mouse over cell. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellType"/> (<see cref="System.String"/>)</term>
    ///         <description>Check Box (Default: Text Box)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValue"/> (<see cref="System.Object"/>)</term>
    ///         <description>The cell value should match one of the values of <see cref="GridCheckBoxCellInfo"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CellValueType"/> (<see cref="System.Type"/>)</term>
    ///         <description>Specifies the preferred <see cref="System.Type"/> for cell values. When you assign a value to the <see cref="GridStyleInfo"/> object, the value will be converted to this type. If the value cannot be converted, <see cref="GridStyleInfo.Error"/> will contain error information. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.CheckBoxOptions"/> (<see cref="GridCheckBoxCellInfo"/>)</term>
    ///         <description>Gets / sets flat look and values that represent checked, unchecked, and indeterminated state of the check box. (Default: NULL)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Description"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the text that is shown in the check box. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Enabled"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the cell can be activated as current cell or if cell should be skipped when moving the current cell. When disabled, the check box will be drawn grayed out. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Font"/> (<see cref="GridFontInfo"/>)</term>
    ///         <description>The font for drawing text. (Default: GridFontInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HorizontalAlignment"/> (<see cref="GridHorizontalAlignment"/>)</term>
    ///         <description>Specifies horizontal alignment of text and the checkbox in the cell. (Default: GridHorizontalAlignment.Left)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.HotkeyPrefix"/> (<see cref="System.Drawing.Text.HotkeyPrefix"/>)</term>
    ///         <description>Specifies how hot-key prefixes should be displayed. Hot-keys are indicated in text with an '&amp;' (ampersand).
    /// When you enable hot-key prefix, the specific characters can be displayed underlined or regular. The '&amp;' will not
    /// be displayed. The <see cref="GridStyleInfo.Description"/> of a check box can have hotkeys. (Default: HotkeyPrefix.Show)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Interior"/> (<see cref="Syncfusion.Drawing.BrushInfo"/>)</term>
    ///         <description> Lets you specify a solid backcolor, gradient, or pattern style with both back and forecolor for a cell's
    ///  background. (Default: SystemColors.Window)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.MaskEdit"/> (<see cref="GridMaskEditInfo"/>)</term>
    ///         <description>Gets / sets MaskedEdit state. MaskedEdit is itself an expandable object with several properties that can be set individually and participate in style inheritance mechanism. (Default: GridMaskEditInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.ReadOnly"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell contents can be modified by the user. You can programmatically change Read-only cells by setting <see cref="GridModel.DiscardReadOnly"/> to True. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Text"/> (<see cref="System.String"/>)</term>
    ///         <description>Gets / sets the value as a string. If a <see cref="GridStyleInfo.CellValueType"/>
    /// is specified, the text will be parsed and converted to the type specified with
    /// <see cref="GridStyleInfo.CellValueType"/> using any <see cref="GridStyleInfo.CultureInfo"/>
    /// information. The cell value should match one of the values provided by <see cref="GridCheckBoxCellInfo"/>. (Default: String.Empty)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextColor"/> (<see cref="System.Drawing.Color"/>)</term>
    ///         <description>Lets you specify the color for drawing the cell text. (Default: SystemColors.WindowText)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TextMargins"/> (<see cref="GridMarginsInfo"/>)</term>
    ///         <description>Holds text margins in pixels. When drawing a cell, this specifies the empty area between the
    /// text rectangle including the check box and the borders of the cell. (Default: GridMarginsInfo.Default)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Themed"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if cell should be drawn using Windows XP themes when <see cref="GridControlBase.ThemesEnabled"/> has been set. (Default: True)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.TriState"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if this is a Tristate check box that has an additional indeterminated state. (Default: False)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.Trimming"/> (<see cref="System.Drawing.StringTrimming"/>)</term>
    ///         <description>Indicates how text is trimmed when it exceeds the edges of the cell text rectangle. (Default: StringTrimming.Character)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.VerticalAlignment"/> (<see cref="GridVerticalAlignment"/>)</term>
    ///         <description>Specifies vertical alignment of text and the check box in the cell. (Default: GridVerticalAlignment.Top)</description>
    ///     </item>
    ///     <item>
    ///         <term><see cref="GridStyleInfo.WrapText"/> (<see cref="System.Boolean"/>)</term>
    ///         <description>Specifies if the <see cref="GridStyleInfo.Description"/> text should be wrapped when it does not fit into a single line. (Default: True)</description>
    ///     </item>
    /// </list>
    /// <para/>
    /// </remarks>
    public class GridCheckBoxCellRenderer : GridCellRendererBase
    {
        // Fields

        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        internal Size checkBoxSize = new Size(13, 13);
        private Rectangle checkerBounds = Rectangle.Empty;
        private GridCellContextValue checkerRectangle = new GridCellContextValue(null);
        private GridCellContextValue cellScope = new GridCellContextValue(false);
        private GridCellContextValue latestHitTestContext = new GridCellContextValue(GridHitTestContext.None);
        ////private ThemedCheckBoxDrawing themedDrawing = null;
        ////private int latestHitTestContext = ;

        /// <summary>Gets or sets CheckBoxSize. For internal use.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public Size CheckBoxSize
        {
            get
            {
                return checkBoxSize;
            }

            set
            {
                checkBoxSize = value;
            }
        }

        /// <summary>
        /// Initializes a new <see cref="GridCheckBoxCellRenderer"/> object for the given GridControlBase
        /// and <see cref="GridCellModelBase"/>.
        /// </summary>
        /// <param name="grid">The <see cref="GridControlBase"/> that display this cell renderer.</param>
        /// <param name="cellModel">The <see cref="GridCheckBoxCellModel"/> that holds data for this cell renderer that should
        /// be shared among views.</param>
        /// <remarks>References to GridControlBase, 
        /// and GridCellModelBase will be saved.</remarks>
        public GridCheckBoxCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            grid.ViewLayout.LayoutChanged += new EventHandler(GridViewLayoutChanged);
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
                Grid.ViewLayout.LayoutChanged -= new EventHandler(GridViewLayoutChanged);
            }

            base.Dispose(disposing);
        }

        private void GridViewLayoutChanged(object sender, EventArgs e)
        {
            checkerRectangle.ResetValue();
        }

        /// <summary> 
        /// Calculates the checker boundaries taking alignment, margins and style information into account.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="clientBounds">Specifies the client rectangle. It is the cell rectangle without buttons and borders.</param>
        /// <param name="text">The text to display in the check box cell.</param>
        /// <param name="font">The font for drawing text.</param>
        /// <param name="align">The alignment of check box and text inside cell.</param>
        /// <param name="margins">The margins between check box, text, and the cell borders.</param>
        /// <param name="textAlign">Specifies if checker should be left or right of text.</param>      
        /// <returns>The rectangle with check box bounds.</returns>
        protected virtual Rectangle DetermineCheckBoxBounds(Graphics g, Rectangle clientBounds, string text, Font font, ContentAlignment align, GridMargins margins, GridTextAlign textAlign)
        {
            int x = clientBounds.Left;
            int y = clientBounds.Top;
            int dx = clientBounds.Width;
            int dy = clientBounds.Height;

            Size checkerSize = this.checkBoxSize;
            Size sizeWg = WinFormsUtils.MeasureSampleWString(g, font);
            if (text.Length > 0)
            {
                Size size = g.MeasureString(text, font).ToSize();
                ////size = GridMargins.AddMargins(size, margins);
                checkerSize = GridUtil.Max(size, sizeWg);
                checkerSize.Width += checkBoxSize.Width;
            }

            checkerSize = GridMargins.AddMargins(checkerSize, margins);
            checkerSize.Height = Math.Max(checkerSize.Height, checkBoxSize.Height);

            //// compute clientBounds for checkbox
            int yOffs = y + Math.Max(0, Math.Min(0, dy - checkerSize.Height));

            if ((align & GridUtil.AnyMiddle) != 0)
            {
                yOffs = y + Math.Max(Math.Abs(dy - checkerSize.Height) / 2, 0);
            }
            else if ((align & GridUtil.AnyBottom) != 0)
            {
                yOffs = y + Math.Max(dy - checkerSize.Height, 0);
            }

            yOffs += Math.Max(margins.Top, 0);
            int yCorner = Math.Min(y + dy, yOffs + this.checkBoxSize.Height);

            if ((textAlign == GridTextAlign.Right) != Grid.IsRightToLeft())
            {
                int xOffs = x + dx - Math.Max(0, Math.Min(0, dx - checkerSize.Width));

                if ((align & GridUtil.AnyCenter) != 0)
                {
                    xOffs = x + dx - Math.Max((dx - checkerSize.Width) / 2, 0);
                }
                else if (!Grid.IsRightToLeft() && (align & GridUtil.AnyRight) == 0)
                {
                    xOffs = x + dx - Math.Max(dx - checkerSize.Width, 0);
                }
                else if (Grid.IsRightToLeft() && (align & GridUtil.AnyRight) != 0)
                {
                    xOffs = x + dx - Math.Max(dx - checkerSize.Width, 0);
                }

                xOffs -= Math.Max(margins.Left, 0);
                int xCorner = Math.Max(x, xOffs - this.checkBoxSize.Width);
                return Rectangle.FromLTRB(xCorner, yOffs, xOffs, yCorner);
            }
            else
            {
                int xOffs = x + Math.Max(0, Math.Min(0, dx - checkerSize.Width));
                if ((align & GridUtil.AnyCenter) != 0)
                {
                    xOffs = x + Math.Max((dx - checkerSize.Width) / 2, 0);
                }
                else if (Grid.IsRightToLeft() && (align & GridUtil.AnyRight) == 0)
                {
                    xOffs = x + Math.Max(dx - checkerSize.Width, 0);
                }
                else if (!Grid.IsRightToLeft() && (align & GridUtil.AnyRight) != 0)
                {
                    xOffs = x + Math.Max(dx - checkerSize.Width, 0);
                }

                xOffs += Math.Max(margins.Left, 0);
                int xCorner = Math.Min(x + dx, xOffs + this.checkBoxSize.Width);

                return Rectangle.FromLTRB(xOffs, yOffs, xCorner, yCorner);
            }
        }

        /// <summary>
        /// Draws the checker at the given coordinates.
        /// </summary>
        /// <param name="g">The <see cref="System.Drawing.Graphics"/> context of the canvas.</param>
        /// <param name="clientBounds">Specifies the client rectangle. It is the cell rectangle without buttons and borders.</param>
        /// <param name="style">A reference to the style object of the cell.</param>       
        /// <param name="state">The current state of the check box to be drawn.</param>
        /// <param name="align">The alignment of check box and text inside cell.</param>
        /// <param name="text">The text to display in the checkb ox cell.</param>
        /// <param name="font">The font for drawing text.</param>       
        protected virtual void DrawCheckBox(Graphics g, Rectangle clientBounds, GridStyleInfo style, ButtonState state, ContentAlignment align, string text, Font font)
        {
            Rectangle r = DetermineCheckBoxBounds(g, clientBounds, text, font, align, style.ReadOnlyTextMargins.ToMargins(), style.TextAlign);
            if ((!Grid.PrintingMode && style.Themed && XPThemes.IsThemedOS && XPThemes.IsThemeActive && this.Grid.ThemesEnabled)
                || (style.Themed && this.Grid.ThemesEnabled &&  this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme))
            {
                //// Determine if the mixed state is set (when Tristate is on).
                bool mixedStateSet = false;
                if ((state & ButtonState.Inactive) > 0
                    && (state & ButtonState.Checked) > 0)
                {
                    mixedStateSet = true;
                }

                if (mixedStateSet)
                {
                    state &= ~ButtonState.Inactive;
                    state &= ~ButtonState.Checked;
                }

                //// Determine if "hot".
                bool isHovering = false;
                GridStyleInfoIdentity id = style.Identity as GridStyleInfoIdentity;
                if (((int)this.latestHitTestContext.GetValue(id.RowIndex, id.ColIndex)) == GridHitTestContext.CheckBoxChecker)
                {
                    if (style.Enabled)
                    {
                        state |= ButtonState.Flat;
                        isHovering = true;
                    }
                    else
                    {
                        state &= ~ButtonState.Pushed;
                    }
                }
                else
                {
                    state &= ~ButtonState.Flat;
                }

                ////                    if(this.themedDrawing == null)
                ////                        this.themedDrawing = new ThemedCheckBoxDrawing();
                ////                    this.themedDrawing.DrawCheckBox(g, r, state, mixedStateSet);

                this.Grid.Model.Options.GridVisualStylesDrawing.DrawCheckBoxStyle(g, r, state, mixedStateSet);

                if (isHovering)
                {
                    Grid.NotifyCellHighlighted(style.CellIdentity.RowIndex, style.CellIdentity.ColIndex, style);
                }
            }
            else
            {
                /*ControlPaint.*/
                DrawCheckBox(g, r.X, r.Y, r.Width, r.Height, state);
            }
        }

        /// <summary>
        /// Returns the current <see cref="ButtonState"/> for the checker for the specified cell.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <param name="style">A reference to the style object of the cell.</param>
        /// <returns>The current <see cref="ButtonState"/> at the given cell.</returns>
        protected ButtonState GetState(int rowIndex, int colIndex, GridStyleInfo style)
        {
            ButtonState w = ButtonState.Normal;
            CultureInfo culture = style.GetCulture(true);

           

            if (IsCellScope(rowIndex, colIndex))
            {
                w |= ButtonState.Pushed;
            }

            // Load representations for True / False.
            string strTrue = string.Empty, strFalse = string.Empty;
            GridCheckBoxCellInfo ua = style.ReadOnlyCheckBoxOptions;
            if (ua != null && ua.CheckedValue != null)
            {
                strTrue = ua.CheckedValue;
            }

            if (GridUtil.IsEmpty(strTrue))
            {
                strTrue = "1";
            }

            if (ua != null && ua.UncheckedValue != null)
            {
                strFalse = ua.UncheckedValue;
            }

            if (GridUtil.IsEmpty(strFalse))
            {
                strFalse = "0";
            }

            bool triState = style.TriState;
            string strVal = style.Text;

            if (strVal.ToLower(culture) == strTrue.ToLower(culture))
            {
                w |= ButtonState.Checked;
            }
            else if (!triState || strVal.ToLower(culture) == strFalse.ToLower(culture))
            {
                ////w |= ButtonState.Unchecked; 
            }
            else
            {
                w |= ButtonState.Inactive | ButtonState.Checked;
                //// Indeterminated
            }

            if (ua.FlatLook || Grid.PrintingMode)
            {
                w |= ButtonState.Flat;
            }

            return w;
        }

        /// <override/>
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            string text = style.Description;
            Font font = style.GdipFont;
            ButtonState state = GetState(rowIndex, colIndex, style);
            ContentAlignment align = GridUtil.ConvertToContentAlignment(style.HorizontalAlignment)
                & GridUtil.ConvertToContentAlignment(style.VerticalAlignment);

            bool alignToRight = (style.TextAlign == GridTextAlign.Right) != Grid.IsRightToLeft();
            DrawCheckBox(g, clientRectangle, style, state, align, text, font);

            if (text.Length > 0)
            {
                Rectangle textRectangle = clientRectangle;
                GridMargins margins = style.ReadOnlyTextMargins.ToMargins();
                if (Grid.IsRightToLeft())
                {
                    margins = margins.SwapRightToLeft();
                }

                if (alignToRight)
                {
                    textRectangle.Width -= this.checkBoxSize.Width + margins.Left;
                }
                else
                {
                    GridUtil.OffsetLeft(ref textRectangle, this.checkBoxSize.Width + margins.Left);
                }

                textRectangle = GridMargins.RemoveMargins(textRectangle, margins);
                if (textRectangle.Width > 0)
                {
                    GridDrawCellDisplayTextEventArgs e = new GridDrawCellDisplayTextEventArgs(g, text, textRectangle, style);
                    Grid.RaiseDrawCellDisplayText(e);
                    if (!e.Cancel)
                    {
                        text = e.DisplayText;
                        textRectangle = e.TextRectangle;
                        Color textColor = Grid.PrintingMode && Grid.Model.Properties.BlackWhite ? Color.Black : style.TextColor;
                        GridStaticCellRenderer.DrawText(g, text, font, textRectangle, style, textColor, Grid.IsRightToLeft());
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the check box at the specified cell coordinates has scope set.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <returns>True if cell has scope; False otherwise.</returns>
        public bool IsCellScope(int rowIndex, int colIndex)
        {
            return (bool)cellScope.GetValue(rowIndex, colIndex);
        }

        /// <summary>
        /// Sets scope for the specified cell.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <param name="value">True if cell has scope; False otherwise.</param>
        public void SetCellScope(int rowIndex, int colIndex, bool value)
        {
            if (cellScope.SetValue(rowIndex, colIndex, value))
            {
                Rectangle checkerBounds = GetCachedCheckerBounds(rowIndex, colIndex);
                Grid.Invalidate(checkerBounds);
            }
        }

        /// <summary>
        /// Returns the checker bounds for the cell at the specified cell coordinates.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        /// <returns>The <see cref="Rectangle"/> with bounds for the checker.</returns>
        protected Rectangle GetCachedCheckerBounds(int rowIndex, int colIndex)
        {
            if (checkerRectangle.GetValue(rowIndex, colIndex) == null)
            {
                // style
                GridStyleInfo style = Grid.Model[rowIndex, colIndex];

                if (style.ReadOnly)
                {
                    checkerRectangle.SetValue(rowIndex, colIndex, Rectangle.Empty);
                    return Rectangle.Empty;
                }
                else
                {
                    // Compute check box rectangle for mouse hit-testing.
                    GridCellLayout layout = PerformLayout(rowIndex, colIndex, style);
                    ContentAlignment align = GridUtil.ConvertToContentAlignment(style.HorizontalAlignment)
                        & GridUtil.ConvertToContentAlignment(style.VerticalAlignment);

                    Graphics g = Grid.CreateGridGraphics();
                    Font font = style.GdipFont;
                    this.checkerBounds = DetermineCheckBoxBounds(g, layout.ClientRectangle, style.Description, font, align, style.ReadOnlyTextMargins.ToMargins(), style.TextAlign);
                    g.Dispose();
                    checkerRectangle.SetValue(rowIndex, colIndex, this.checkerBounds);
                    return checkerBounds;
                }
            }

            return (Rectangle)checkerRectangle.GetValue(rowIndex, colIndex);
        }

        /// <summary>
        /// Overriden. Checks if mouse is inside the checker.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="e">A <see cref="MouseEventArgs"/> with data about the mouse event.</param>
        /// <param name="controller">The current controller that requested to handle this mouse event.</param>
        /// <returns>Non-zero if mouse is over the checker; 0 otherwise.</returns>
        protected override int OnHitTest(int rowIndex, int colIndex, MouseEventArgs e, IMouseController controller)
        {
            Point pt = new Point(e.X, e.Y);
            int context = GridHitTestContext.None;
            try
            {
#if DEBUG
                Trace.WriteLineIf(Switches.CheckBoxCellEvents.TraceVerbose, String.Format("Begin OnHitTest Checkbox({0},{1})", rowIndex, colIndex));
#endif
                checkerBounds = GetCachedCheckerBounds(rowIndex, colIndex);

                if (checkerBounds.Contains(pt))
                {
                    context = GridHitTestContext.CheckBoxChecker;
                }
                ////                else if (controller == null)
                ////                    context = GridHitTestContext.Cell;
                this.SetLatestHitTestContext(context, rowIndex, colIndex, false);
                return context;
            }
            finally
            {
#if DEBUG
                Trace.WriteLineIf(Switches.CheckBoxCellEvents.TraceVerbose, String.Format("End OnHitTest Checkbox({0},{1})", rowIndex, colIndex));
#endif
            }
        }

        internal void SetLatestHitTestContext(int context, int rowIndex, int colIndex, bool force)
        {
            if (force || ((int)this.latestHitTestContext.GetValue(rowIndex, colIndex)) != context)
            {
                this.latestHitTestContext.SetValue(rowIndex, colIndex, context);
                GridRangeInfo range;
                this.Grid.Model.CoveredRanges.Find(rowIndex, colIndex, out range);
                this.Grid.InvalidateRange(range);
            }
        }

        /// <override/>
        protected override void OnMouseHoverEnter(int rowIndex, int colIndex)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex);
            }
#else
            ;
#endif
            base.OnMouseHoverEnter(rowIndex, colIndex);
            this.SetLatestHitTestContext(GridHitTestContext.Cell, rowIndex, colIndex, true);
        }

        /// <override/>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(rowIndex, colIndex);
            }
#else
            ;
#endif

            base.OnMouseHoverLeave(rowIndex, colIndex, e);
            this.SetLatestHitTestContext(GridHitTestContext.None, rowIndex, colIndex, true);
        }

        /// <override/>
        protected override void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            if (((int)this.latestHitTestContext.GetValue(rowIndex, colIndex)) == GridHitTestContext.CheckBoxChecker)
            {
                SetCellScope(rowIndex, colIndex, true);
            }
            else
            {
                base.OnMouseDown(rowIndex, colIndex, e);
            }
        }

        /// <override/>
        protected override void OnMouseUp(int rowIndex, int colIndex, MouseEventArgs e)
        {
            try
            {
#if DEBUG
                Trace.WriteLineIf(Switches.CheckBoxCellEvents.TraceVerbose, String.Format("Begin OnMouseUp Checkbox({0},{1})", rowIndex, colIndex));
#endif

                Rectangle checkerBounds = GetCachedCheckerBounds(rowIndex, colIndex);
                Point pt = new Point(e.X, e.Y);
                if (checkerBounds.Contains(pt))
                {
                    cellScope.SetValue(rowIndex, colIndex, false);
                    if (Grid.RaiseCheckBoxClick(rowIndex, colIndex, e))
                    {
                        this.Grid.CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.None);
                        if (Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                        {
                            OnClickedCheckBox();
                            return;
                        }
                    }
                    if (!Grid.CurrentCell.HasCurrentCellAt(rowIndex, colIndex))
                        this.Grid.CurrentCell.MoveTo(rowIndex, colIndex, GridSetCurrentCellOptions.SetFocus);
                }
                else
                {
                    base.OnMouseUp(rowIndex, colIndex, e);
                }

                SetCellScope(rowIndex, colIndex, false);
            }
            finally
            {
#if DEBUG
                Trace.WriteLineIf(Switches.CheckBoxCellEvents.TraceVerbose, String.Format("End OnMouseUp Checkbox({0},{1})", rowIndex, colIndex));
#endif
            }
        }

        /// <override/>
        protected override void OnMouseMove(int rowIndex, int colIndex, MouseEventArgs e)
        {
            try
            {
#if DEBUG
                Trace.WriteLineIf(Switches.CheckBoxCellEvents.TraceVerbose, String.Format("Begin OnMouseMove Checkbox({0},{1})", rowIndex, colIndex));
#endif
                Rectangle checkerBounds = GetCachedCheckerBounds(rowIndex, colIndex);
                SetCellScope(rowIndex, colIndex, checkerBounds.Contains(new Point(e.X, e.Y)));
            }
            finally
            {
#if DEBUG
                Trace.WriteLineIf(Switches.CheckBoxCellEvents.TraceVerbose, String.Format("End OnMouseMove Checkbox({0},{1})", rowIndex, colIndex));
#endif
            }
        }

        /// <override/>
        protected override void OnCancelMode(int rowIndex, int colIndex)
        {
            SetCellScope(rowIndex, colIndex, false);
        }

        /// <override/>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (!e.Handled && e.KeyChar == ' ')
            {
                e.Handled = true;
                Grid.InvalidateRange(GridRangeInfo.Cell(RowIndex, ColIndex));

                // trigger event
                OnClickedCheckBox();
            }
        }

        /// <summary>
        /// Called when user clicked check box. Toggles the cell's value and stores it back into the cell.
        /// </summary>
        protected virtual void OnClickedCheckBox()
        {
            // style
            GridStyleInfo style = Grid.Model[RowIndex, ColIndex];
            if (style.ReadOnly || !this.NotifyCurrentCellChanging())
            {
                return;
            }

            bool triState = style.TriState;
            CultureInfo culture = style.GetCulture(true);

            // Load representations for True / False.
            string strTrue = string.Empty, strFalse = string.Empty;
            GridCheckBoxCellInfo ua = style.ReadOnlyCheckBoxOptions;
            if (ua != null && ua.CheckedValue != null)
            {
                strTrue = ua.CheckedValue;
            }

            if (GridUtil.IsEmpty(strTrue))
            {
                strTrue = "1";
            }

            if (ua != null && ua.UncheckedValue != null)
            {
                strFalse = ua.UncheckedValue;
            }

            if (GridUtil.IsEmpty(strFalse))
            {
                strFalse = "0";
            }

            string strIndeterm;
            if (ua != null && ua.IndetermValue != null)
            {
                strIndeterm = ua.IndetermValue;
            }
            else
            {
                strIndeterm = string.Empty;
            }

            // Switch to next value
            string strVal = style.Text;

            // Checked to unchecked.
            if (strVal.ToLower(culture) == strTrue.ToLower(culture))
            {
                strVal = strFalse;
            }
            else if (triState && strVal.ToLower(culture) == strFalse.ToLower(culture))
            {    
                // Unchecked to grayed.
                strVal = strIndeterm;
            }
            else
            {
                // If TriState did fall through.
                // Grayed to checked.
                strVal = strTrue;
            }

            // Store value.
            Grid.Model[RowIndex, ColIndex].Text = strVal;

            this.NotifyCurrentCellChanged();
        }

        /// <summary>
        /// This method is called from GridCurrentCell.ConfirmChanges when the current cell
        /// was marked as modified. Any drop-downs have been closed at this time. It saves changes for the current cell.
        /// </summary>
        /// <returns>
        /// True if changes were saved successfully; False if no changes were saved.
        /// </returns>
        /// <override/>
        protected /*internal*/ override bool OnSaveChanges()
        {
#if DEBUG
            if (Switches.CellRenderer.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ControlText);
            }
#else
            ;
#endif
            return true;
        }

        [ThreadStatic]
        private static Bitmap checkImage;

        /// <summary>
        /// Draws check box at the specified bounds.
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="rectangle">The Rectangle Bounds.</param>
        /// <param name="state">Button state.</param>
        public static void DrawCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
        {
            DrawCheckBox(graphics, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, state);
        }

        /// <summary>
        /// Draws checks at the given bounds.
        /// </summary>
        /// <param name="graphics">Graphics context.</param>
        /// <param name="x">X co-ordinate.</param>
        /// <param name="y">Y co-ordinate.</param>
        /// <param name="width">Width of the rectangle bounds.</param>
        /// <param name="height">Height of the rectangle bounds.</param>
        /// <param name="state">Button state.</param>
        public static void DrawCheckBox(Graphics graphics, int x, int y, int width, int height, ButtonState state)
        {
            if ((state & ButtonState.Flat) == ButtonState.Flat)
            {
                DrawFlatCheckBox(graphics, new Rectangle(x, y, width, height), state);
            }
            else
            {
                DrawFrameControl(graphics, x, y, width, height, 4, (int)state);
            }
        }

        private static void DrawFlatCheckBox(Graphics graphics, Rectangle rectangle, ButtonState state)
        {
            Brush brush1 = ((state & ButtonState.Inactive) == ButtonState.Inactive) ? SystemBrushes.Control : SystemBrushes.Window;
            Color color1 = ((state & ButtonState.Inactive) == ButtonState.Inactive) ? SystemColors.ControlDark : SystemColors.ControlText;
            DrawFlatCheckBox(graphics, rectangle, color1, brush1, state);
        }

        private static void DrawFlatCheckBox(Graphics graphics, Rectangle rectangle, Color foreground, Brush background, ButtonState state)
        {
            Rectangle bounds = new Rectangle(rectangle.X + 1, rectangle.Y + 1, rectangle.Width - 2, rectangle.Height - 2);
            graphics.FillRectangle(background, bounds);
            if ((state & ButtonState.Checked) == ButtonState.Checked)
            {
                if (((checkImage == null) || (checkImage.Width != rectangle.Width)) || (checkImage.Height != rectangle.Height))
                {
                    if (checkImage != null)
                    {
                        checkImage.Dispose();
                        checkImage = null;
                    }

                    NativeMethods.RECT nativeRect = NativeMethods.RECT.FromXYWH(0, 0, rectangle.Width, rectangle.Height);
                    Bitmap bm = new Bitmap(rectangle.Width, rectangle.Height);
                    using (Graphics bmGraphics = Graphics.FromImage(bm))
                    {
                        bmGraphics.Clear(Color.Transparent);
                        IntPtr hdc = bmGraphics.GetHdc();
                        try
                        {
                            NativeMethods.DrawFrameControl(hdc, ref nativeRect, 2, 1);
                        }
                        finally
                        {
                            bmGraphics.ReleaseHdcInternal(hdc);
                        }
                    }

                    bm.MakeTransparent();
                    checkImage = bm;
                }

                rectangle.X++;
                DrawImageColorized(graphics, checkImage, rectangle, foreground);
                rectangle.X--;
            }

            graphics.DrawRectangle(SystemPens.ControlDark, bounds.X, bounds.Y, (bounds.Width - 1), (bounds.Height - 1));
        }

        internal static void DrawImageColorized(Graphics graphics, Image image, Rectangle destination, Color replaceBlack)
        {
            DrawImageColorized(graphics, image, destination, RemapBlackAndWhitePreserveTransparentMatrix(replaceBlack, Color.White));
        }

        private static ColorMatrix RemapBlackAndWhitePreserveTransparentMatrix(Color replaceBlack, Color replaceWhite)
        {
            float single1 = ((float)replaceBlack.R) / 255f;
            float single2 = ((float)replaceBlack.G) / 255f;
            float single3 = ((float)replaceBlack.B) / 255f;
            float single7 = ((float)replaceBlack.A) / 255f;
            float single4 = ((float)replaceWhite.R) / 255f;
            float single5 = ((float)replaceWhite.G) / 255f;
            float single6 = ((float)replaceWhite.B) / 255f;
            float single8 = ((float)replaceWhite.A) / 255f;
            ColorMatrix colorMatrix = new ColorMatrix();
            colorMatrix.Matrix00 = -single1;
            colorMatrix.Matrix01 = -single2;
            colorMatrix.Matrix02 = -single3;
            colorMatrix.Matrix10 = single4;
            colorMatrix.Matrix11 = single5;
            colorMatrix.Matrix12 = single6;
            colorMatrix.Matrix33 = 1f;
            colorMatrix.Matrix40 = single1;
            colorMatrix.Matrix41 = single2;
            colorMatrix.Matrix42 = single3;
            colorMatrix.Matrix44 = 1f;
            return colorMatrix;
        }

        private static ColorMatrix RemapBlackAndWhiteAndTransparentMatrix(Color replaceBlack, Color replaceWhite)
        {
            float single1 = ((float)replaceBlack.R) / 255f;
            float single2 = ((float)replaceBlack.G) / 255f;
            float single3 = ((float)replaceBlack.B) / 255f;
            float single4 = ((float)replaceBlack.A) / 255f;
            float single5 = ((float)replaceWhite.R) / 255f;
            float single6 = ((float)replaceWhite.G) / 255f;
            float single7 = ((float)replaceWhite.B) / 255f;
            float single8 = ((float)replaceWhite.A) / 255f;
            ColorMatrix matrix1 = new ColorMatrix();
            matrix1.Matrix00 = -single1;
            matrix1.Matrix01 = -single2;
            matrix1.Matrix02 = -single3;
            matrix1.Matrix03 = -single4;
            matrix1.Matrix10 = single5;
            matrix1.Matrix11 = single6;
            matrix1.Matrix12 = single7;
            matrix1.Matrix13 = single8;
            matrix1.Matrix40 = single1;
            matrix1.Matrix41 = single2;
            matrix1.Matrix42 = single3;
            matrix1.Matrix43 = single4;
            matrix1.Matrix44 = 1f;
            return matrix1;
        }
        
        private static void DrawImageColorized(Graphics graphics, Image image, Rectangle destination, ColorMatrix matrix)
        {
            ImageAttributes imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(matrix);
            graphics.DrawImage(image, destination, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
            imageAttributes.Dispose();
        }

        internal static void DrawImageColorized(Graphics graphics, Image image, Rectangle destination, Color replaceBlack, Color replaceWhite)
        {
            DrawImageColorized(graphics, image, destination, RemapBlackAndWhiteAndTransparentMatrix(replaceBlack, replaceWhite));
        }
        
        private static void DrawFrameControl(Graphics graphics, int x, int y, int width, int height, int kind, int state)
        {
            NativeMethods.RECT nativeRect = NativeMethods.RECT.FromXYWH(0, 0, width, height);
            Bitmap bm = new Bitmap(width, height);
            Graphics bmGraphics = Graphics.FromImage(bm);
            bmGraphics.Clear(Color.Transparent);
            IntPtr hdc = bmGraphics.GetHdc();
            NativeMethods.DrawFrameControl(hdc, ref nativeRect, kind, state);
            ////IntSecurity.Win32HandleManipulation.Assert();
            try
            {
                bmGraphics.ReleaseHdc(hdc);
            }
            finally
            {
                ////CodeAccessPermission.RevertAssert();
                bmGraphics.Dispose();
            }

            graphics.DrawImage(bm, x, y);
            bm.Dispose();
        }
        
#if NativeMethods
        class NativeMethods
        {
            [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true, CallingConvention=CallingConvention.Winapi)] 
            internal static extern bool DrawFrameControl(IntPtr hDC, ref NativeMethods.RECT rect, int type, int state); 
   
            [StructLayout(LayoutKind.Sequential)]
                internal struct RECT 
            {
                public RECT(Rectangle rect)
                {
                    this.bottom = rect.Bottom;
                    this.left = rect.Left;
                    this.right = rect.Right;
                    this.top = rect.Top;
                }

                public RECT(int left, int top, int right, int bottom)
                {
                    this.bottom = bottom;
                    this.left = left;
                    this.right = right;
                    this.top = top;
                }                
            
                public static RECT FromXYWH(int x, int y, int width, int height)
                {
                    return new RECT(x, y, x+width, y+height);
                }
                
                internal int Width{get{return this.right-this.left;}}
                internal int Height{get{return this.bottom-this.top;}}
            
                public int left;
                public int top;
                public int right;
                public int bottom;

                public override /*Object*/ string ToString()
                {
                    return String.Concat(
                        "Left = ",
                        this.left,
                        " Top ",
                        this.top,
                        " Right = ",
                        this.right,
                        " Bottom = ",
                        this.bottom);
                } 
            }
        }
#endif
    }
}
