#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Windows.Forms.Grid;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;


namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    #region CellModel

    public class PivotGridExpandCellCellModel : GridStaticCellModel
    {
        public PivotGridExpandCellCellModel(GridModel grid)
            : base(grid)
        {
        }

        public override GridCellRendererBase CreateRenderer(GridControlBase control)
        {
            return new PivotGridExpandCellCellRenderer(control, this);
        }
    }
    #endregion

    #region Cell Renderer

    public class PivotGridExpandCellCellRenderer : GridStaticCellRenderer
    {
        private GridCellButton pushButton;
        GridRangeInfo hoverRange = GridRangeInfo.Empty;
        GridRangeInfo mouseDownRange = GridRangeInfo.Empty;
        bool inMouseDownRange = false;
        private int buttonMargin = 0;
        static readonly BrushInfo defaultInterior1 = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
        static readonly BrushInfo defaultInterior2 = new BrushInfo(GradientStyle.Horizontal, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));


        public PivotGridExpandCellCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            AddButton(pushButton = new GridCellButton(this));
        }

        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            int buttonWidth =  15;
            int buttonHeight = 12;
            if (!Grid.IsRightToLeft())
                buttonsBounds[0] = GridUtil.CenterInRect(new Rectangle(innerBounds.X, innerBounds.Y, buttonWidth + 5, buttonHeight + 5),
                    new Size(buttonWidth, buttonHeight));
            else
                buttonsBounds[0] = GridUtil.CenterInRect(new Rectangle(innerBounds.X + innerBounds.Width - (buttonWidth + 5), innerBounds.Y, buttonWidth + 5, buttonHeight + 5),
                    new Size(buttonWidth, buttonHeight));
            pushButton.Text = style.Description;
            buttonMargin = buttonWidth;
            // Creates white space behind the button
            //innerBounds = new Rectangle(innerBounds.X + buttonWidth + 2, innerBounds.Y, innerBounds.Width - buttonWidth, innerBounds.Height);//innerBounds.Height);
            return innerBounds;
        }

        /// <override/>
        protected /*internal*/ override void OnOutlineCurrentCell(Graphics g, Rectangle r)
        {
            // do nothing
        }

        /// <override/>
        protected override bool OnQueryShowButtons(int rowIndex, int colIndex, GridStyleInfo style)
        {
            if (style.ShowButtons == GridShowButtons.Hide)
                return false;
            else
                return true;

        }

        static GridIconPaint iconPainter = null;
        protected override void OnDraw(Graphics g, Rectangle clientRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            GridMargins margins = style.ReadOnlyTextMargins.ToMargins();
            if (Grid.IsRightToLeft())
            {
                margins = margins.SwapRightToLeft();
            }

            Rectangle textRectangle = GridMargins.RemoveMargins(clientRectangle, margins);
            Rectangle rc = clientRectangle;
            bool drawPressed = GetMarkHeaderState(rowIndex, colIndex, style);
            GridProperties propertyObject = Grid.Model.Properties;

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
                    if (cr.Contains(mouseClientPosition)
                        && Grid.GetWindow().ClientRectangle.Contains(mouseClientPosition))
                    {
                        if (this.hoverRange.Contains(cellRange))
                        {
                            state = ThemedHeaderDrawing.HeaderState.Hot;
                        }

                        if (drawPressed || (this.inMouseDownRange && mouseDownRange.Contains(cellRange)))
                        {
                            state = ThemedHeaderDrawing.HeaderState.Pressed;
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
                        if ((this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black
                            || this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                            && state == ThemedHeaderDrawing.HeaderState.Normal)
                        {
                            style.TextColor = Color.White;
                        }
                        if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                        {
                            style.Font.Bold = true;
                        }
                        if (this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.SystemTheme)
                        {
                            Grid.Model.Options.GridVisualStylesDrawing.DrawHeaderStyle(g, rc, state);
                        }

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
                //if (iconPainter == null)
                //{
                //    iconPainter = PivotIconPaint.Paint;
                //}
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
                if (Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
                    style.TextColor = Color.FromArgb(92, 92, 92);
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

                    if (orientation != 0)
                    {
                        // Let GDI+ do text rotation.
                        float angle = (float)orientation;
                        RotatePaint.DrawRotatedString(g, displayText, font, brText, textRectangle, format, angle);
                    }
                    else
                    {
                        if (!Grid.IsRightToLeft())
                            textRectangle.X += buttonMargin;
                        else
                            textRectangle.X -= buttonMargin;
                        if (!e.UseTextRenderer)
                            g.DrawString(displayText, font, brText, textRectangle, format);
                        else
                        {
                            if (TextRenderer.MeasureText(g, displayText, font).Width < textRectangle.Width)
                            {
                                
                                TextRenderer.DrawText(g, displayText, font, textRectangle, textColor);
                            }
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
        protected override void OnDrawCellBackground(GridDrawCellBackgroundEventArgs e)
        {
            if (Grid.PrintingMode && Grid.Model.Properties.BlackWhite)
            {
                return;
            }
            base.OnDrawCellBackground(e);
        }

        protected override void OnDrawCellButtonBackground(GridCellButton button, Graphics g, Rectangle rect, ButtonState buttonState, GridStyleInfo style)
        {
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
            {
                string value = style.Description == string.Empty ? "-" : style.Description;               
                button.DrawMetroButtonStyle(g, rect, buttonState, value, false);
            }
            else
                base.OnDrawCellButtonBackground(button, g, rect, buttonState, style);

        }

        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
           
            //directly call the draw to avoid base class and focus rectangle...
            button.Text = "";
            button.Draw(g, rowIndex, colIndex, false, style);

            Rectangle faceRect = button.Bounds;
            faceRect.Inflate(-2, -1);
            string text = string.Empty;
            if (style.Description == string.Empty)
            {
                text = "-";
            }
            else
                text = style.Description;

            if (text != null && text.Length > 0 && this.Grid.Model.Options.GridVisualStyles != GridVisualStyles.Metro)
            {
                using (Font font = new Font(style.Font.Facename, (float)7.0))
                {
                    StringFormat format = new StringFormat();
                    format.Alignment = text == "+" ? StringAlignment.Near : StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;
                    format.HotkeyPrefix = style.HotkeyPrefix;
                    format.Trimming = style.Trimming;
                    if (!style.WrapText)
                        format.FormatFlags = StringFormatFlags.NoWrap;

                    Color textColor = Grid.PrintingMode && Grid.Model.Properties.BlackWhite ? Color.Black : style.TextColor;

                    g.DrawString(text, font, new SolidBrush(textColor), faceRect, format);
                }
            }
            //base.OnDrawCellButton(button, g, rowIndex, colIndex, false, style);
        }
        /// <override/>
        protected override void OnButtonClicked(int rowIndex, int colIndex, int button)
        {
            base.OnButtonClicked(rowIndex, colIndex, button);
            OnPushButtonClick(rowIndex, colIndex);
        }


        /// <summary>
        /// Raises <see cref="GridControlBase.PushButtonClick"/> event when the user presses the PushButton.
        /// </summary>
        /// <param name="rowIndex">Specifies the row id.</param>
        /// <param name="colIndex">Specifies the column id.</param>
        protected virtual void OnPushButtonClick(int rowIndex, int colIndex)
        {
            Grid.RaisePushButtonClick(rowIndex, colIndex);
        }
    }
    #endregion
}
