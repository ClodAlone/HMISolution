//-------------------------------------------------------------------------------------------------
// <copyright file="GridSortColumnHeaderCellRenderer.cs" company="syncfusion">
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
using System.Windows.Forms;
using System.IO;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Drawing;
using Syncfusion.Diagnostics;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// Determine the class for header cell render.
    /// </summary>
    public class GroupedHeaderCellRenderer : GridHeaderCellRenderer
    {
        private GridCellButton pushButton;
        GridRangeInfo hoverRange = GridRangeInfo.Empty;
        GridRangeInfo mouseDownRange = GridRangeInfo.Empty;
        static readonly BrushInfo defaultInterior1 = new BrushInfo(GradientStyle.Vertical, Color.FromArgb(203, 199, 184), Color.FromArgb(238, 234, 216));
        GridGroupDropArea groupArea;
        private int  imageWidth;
        private int proposedX = 0; 
        private Rectangle imgRect = new Rectangle(); 
        private bool isImageHidden = false; 
        internal bool imageApplied = false, isRightImage = false;
        /// <summary>
        /// Determine the header cell renders.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="cellModel"></param>
        public GroupedHeaderCellRenderer(GridControlBase grid, GridCellModelBase cellModel)
            : base(grid, cellModel)
        {
            if (grid.GetType().Name == "GridGroupDropArea")
                groupArea = (GridGroupDropArea)grid;
            AddButton(pushButton = new GridCellButton(this));
        }
        /// <summary>
        /// Determine the mouse hover.
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Col index</param>
        /// <param name="e">Event args</param>
        protected override void OnMouseHover(int rowIndex, int colIndex, MouseEventArgs e)
        {
            
            if (groupArea != null)
            {
                GridStyleInfo style = groupArea.GetViewStyleInfo(rowIndex, colIndex);
                if (headerrectangle.Contains(e.X, e.Y))
                {
                    isCursor = true;
                }
                else
                    isCursor = false;
            }
            GridRangeInfo range = GridRangeInfo.Cell(rowIndex, colIndex);
            if (!hoverRange.Equals(range))
            {
                this.hoverRange = range;
            }
            base.OnMouseHover(rowIndex, colIndex, e);
        }

        /// <override/>
        protected override void OnMouseHoverLeave(int rowIndex, int colIndex, EventArgs e)
        {
            if (!hoverRange.IsEmpty)
            {
                this.Grid.InvalidateRange(this.hoverRange);
                this.hoverRange = GridRangeInfo.Empty;
            }
            base.OnMouseHoverLeave(rowIndex, colIndex, e);
        }
        bool isCursor = false;
        /// <summary>
        /// Determine the cursors.
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Col index</param>
        /// <returns> Cursor indication</returns>
        protected override System.Windows.Forms.Cursor OnGetCursor(int rowIndex, int colIndex)
        {
            return isCursor ? Cursors.Hand : base.OnGetCursor(rowIndex, colIndex);
        }
        /// <override/>
        protected override void OnDrawDisplayText(Graphics g, Rectangle textRectangle, int rowIndex, int colIndex, GridStyleInfo style)
        {
            style.Trimming = StringTrimming.EllipsisWord;
            // No arrow needed when printing.
            object tag = style.Tag;
            
            if (Grid.PrintingMode || !(tag is ListSortDirection))
            {
                tag = null;
            }

            GridTableControl tableControl = null;
            GridGroupDropArea groupDropArea = null;
            if (Grid is GridTableControl)
            {
                tableControl = (GridTableControl)Grid;
            }
            if (Grid is GridGroupDropArea) 
            {
                groupDropArea = (GridGroupDropArea)Grid;
                tableControl = groupDropArea.GroupingControl.TableControl;
            }
            if (tableControl != null && tableControl.TableDescriptor != null && tableControl.TableDescriptor.columnImageCollection.ContainsKey(style.Text))
            {
                imageApplied = true;
                if (tableControl.TableDescriptor.Columns[style.Text].HeaderImageAlignment == HeaderImageAlignment.Right)
                    isRightImage = true;
                else
                    isRightImage = false;
            }
            else
            {
                imageApplied = false;
                isRightImage = false;
            }

            ListSortDirection listSortDirection = ListSortDirection.Ascending;
            int margin = 0;
            if (tag != null)
            {
                listSortDirection = (ListSortDirection)tag;
                margin = 23;
            }

            if (tableControl != null && tableControl.TableDescriptor != null && tableControl.TableDescriptor.columnImageCollection.ContainsKey(style.Text))
            {
                imageWidth = 16;
            }
            else
            {
                imageWidth = 0;
            }

            bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
            if (isTextRightToLeft)
            {
                GridUtil.OffsetLeft(ref textRectangle, margin);
            }
            else
            {
                textRectangle.Width -= margin;
            }

            #region image rendering
            if (tableControl != null && tableControl.TableDescriptor != null && tableControl.TableDescriptor.columnImageCollection.ContainsKey(style.Text))
            {
                Image drImage = (Image)tableControl.TableDescriptor.Columns[style.Text].HeaderImage;
                 if (tableControl.TableDescriptor.Columns[style.Text].SerializedImageArray != string.Empty)
                {
                    byte[] array = Convert.FromBase64String(tableControl.TableDescriptor.Columns[style.Text].SerializedImageArray);
                    drImage = Image.FromStream(new MemoryStream(array));
                }

                Size sz = TextRenderer.MeasureText(style.Text, style.GdipFont);
                int textWidth = (int)g.MeasureString(style.Text, style.GdipFont).Width;

                tableControl.Model.UpdateColumnWidths(true);

                if (tableControl.TableDescriptor.Columns[style.Text].HeaderImageAlignment == HeaderImageAlignment.Left)
                {
                    if (tag != null)
                    {
                        textRectangle.Width += margin;
                    }

                    proposedX = textRectangle.Right - (textRectangle.Width / 2) - (textWidth / 2);
                    Point p1 = new Point(proposedX, (textRectangle.Height / 2) - (sz.Height / 2));
                    Point p2 = new Point(proposedX, p1.Y + imageWidth);
                    Point p3 = new Point(proposedX - imageWidth, p1.Y);
                    Point p4 = new Point(proposedX - imageWidth, p1.Y + imageWidth);

                    if (Grid is GridGroupDropArea)
                    {
                        style.HorizontalAlignment = GridHorizontalAlignment.Center;
                        imgRect = new Rectangle(p3.X, textRectangle.Y, imageWidth, imageWidth);

                        if (p3.X > textRectangle.Left)
                        {
                            g.DrawImage(drImage, imgRect);
                            style.WrapText = false;
                            style.Trimming = StringTrimming.EllipsisCharacter;
                            isImageHidden = false;
                        }
                        else
                        {
                            isImageHidden = true;
                            textRectangle.Width -= margin;
                        }
                    }

                }
                else
                {
                    proposedX = textRectangle.Left + textWidth;
                    Point p1 = new Point(proposedX, (textRectangle.Height / 2) - (sz.Height / 2));
                    Point p2 = new Point(proposedX, p1.Y + imageWidth);
                    Point p3 = new Point(proposedX + imageWidth, p1.Y);
                    Point p4 = new Point(proposedX + imageWidth, p1.Y + imageWidth);
                    
                    imgRect = new Rectangle(p1.X, textRectangle.Y, imageWidth, imageWidth);
                    
                    if(Grid is GridGroupDropArea) // Grid is groupDropArea and for image in right
                    {
                        if (p3.X <= textRectangle.Right)
                        {
                            g.DrawImage(drImage, imgRect);
                            style.WrapText = false;
                            style.Trimming = StringTrimming.EllipsisCharacter;
                            isImageHidden = false;
                        }
                        else
                        {
                            isImageHidden = true;
                            if (tag != null && Grid.SortIconPlacement == SortIconPlacement.Left)
                            {
                                GridUtil.OffsetLeft(ref textRectangle, margin);
                            }

                        }
                    }
                }
            }
            #endregion 

            string displayText = String.Empty;
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
                    //bool isTextRightToLeft = (style.RightToLeft == RightToLeft.Inherit && Grid.IsRightToLeft()) || style.RightToLeft == RightToLeft.Yes;
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
                        if (!e.UseTextRenderer)
                            g.DrawString(displayText, font, brText, textRectangle, format);
                        else //Text renderer draw text(SD3608)
                        {
                            TextRenderer.DrawText(g, displayText, font, textRectangle, textColor);
                        }
                    }

                    brText.Dispose();
                    format.Dispose();
                    //base.OnDrawDisplayText(g, textRectangle, rowIndex, colIndex, style);
                }
                    if (tag != null)
                    {

                        if (isTextRightToLeft)
                        {
                            if (isRightImage && !isImageHidden && !(Grid is GridGroupDropArea))
                                rect = new Rectangle(textRectangle.Left + margin, textRectangle.Y, 10, textRectangle.Height);
                            else
                                rect = new Rectangle(textRectangle.Left - margin, textRectangle.Y, 10, textRectangle.Height);
                        }
                        else
                        {
                            if (tableControl != null && tableControl.TableDescriptor.Columns[style.Text] != null && tableControl.TableDescriptor.Columns[style.Text].isImageApplied)
                            {
                                if (tableControl.TableDescriptor.Columns[style.Text].HeaderImageAlignment == HeaderImageAlignment.Left && !isImageHidden)
                                {
                                    rect = new Rectangle(textRectangle.Right - margin, textRectangle.Y, 10, textRectangle.Height);
                                }
                                else
                                {
                                    rect = new Rectangle(textRectangle.Right, textRectangle.Y, 10, textRectangle.Height);
                                }
                            }
                            else
                                rect = new Rectangle(textRectangle.Right, textRectangle.Y, 10, textRectangle.Height);
                        }

                        rect = GridUtil.CenterInRect(rect, new Size(8, 8));

                        Brush brush = null;
                        Pen pen1 = null;

                        this.Grid.Model.Options.GridVisualStylesDrawing.GetSortIconBrush(out brush, out pen1);

                        ////g.FillRectangle(brush, rect);
                        int i2 = Math.Max(0, (rect.Height - 3) / 2);
                        rect.Inflate(-i2, -i2);
                        ////Pen pen1 = new Pen(SystemColors.WindowFrame);
                        Pen pen2 = new Pen(SystemColors.Control);
                        GridTriangleDirection triangleDirection = listSortDirection == ListSortDirection.Ascending ? GridTriangleDirection.Up : GridTriangleDirection.Down;
                        GridPaintTriangle.Paint(g, rect, triangleDirection, brush, pen1, true);
                        pen1.Dispose();
                        pen2.Dispose();
                        brush.Dispose();
                }
        }
        Rectangle rect;
        /// <summary>
        /// Determine the mouse down events.
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Col index</param>
        /// <param name="e">Event args</param>
        protected override void OnMouseDown(int rowIndex, int colIndex, MouseEventArgs e)
        {
            base.OnMouseDown(rowIndex, colIndex, e);
        }
        Rectangle headerrectangle;
        Rectangle touch_headerrectangle;
       
        /// <summary>
        /// Determine the draw cell button.
        /// </summary>
        /// <param name="button">Draw cell button</param>
        /// <param name="g">Graphics</param>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Col index</param>
        /// <param name="bActive">Active</param>
        /// <param name="style">Style</param>
        protected override void OnDrawCellButton(GridCellButton button, Graphics g, int rowIndex, int colIndex, bool bActive, GridStyleInfo style)
        {
            button.Text = "";

            //button.Draw(g, rowIndex, colIndex, false, style);
            Point ptOffset = new Point(1, 1); ;// Point.Empty;
            Bitmap bm = null;
            GridRangeInfo cellRange = GridRangeInfo.Cell(rowIndex, colIndex);
            if (this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Office2010Black)
                bm = GridGroupingBitmaps.IconPainter.GetBitmap("removeWhite.png");
            else if (this.hoverRange.Contains(cellRange) && this.Grid.Model.Options.GridVisualStyles == GridVisualStyles.Metro)
            {
                if (groupArea.GroupingControl.EnableTouchMode)
                {
                    bm = GridGroupingBitmaps.IconPainter.GetBitmap("removeWhite_zoom.png");
                }
                else
                {
                    bm = GridGroupingBitmaps.IconPainter.GetBitmap("removeWhite.png");
                }
            }
            else
            {
                if (groupArea.GroupingControl.EnableTouchMode)
                {
                    bm = GridGroupingBitmaps.IconPainter.GetBitmap("removeBlack_zoom.png");
                }
                else
                {
                    bm = GridGroupingBitmaps.IconPainter.GetBitmap("removeBlack.png");
                }
            }

            headerrectangle = button.Bounds;
            if (groupArea.GroupingControl.EnableTouchMode)
            {
                touch_headerrectangle = button.Bounds;
                touch_headerrectangle.Location = new Point(headerrectangle.Location.X - 4, headerrectangle.Location.Y);
                touch_headerrectangle.Size = new Size(headerrectangle.Size.Width + 5, headerrectangle.Size.Height);

                GridGroupingBitmaps.IconPainter.PaintIcon(g, touch_headerrectangle, ptOffset, bm, Color.Blue);
            }
            else
            {
                GridGroupingBitmaps.IconPainter.PaintIcon(g, button.Bounds, ptOffset, bm, Color.Blue);
            }

            Rectangle faceRect = button.Bounds;
            faceRect.Inflate(-2, -1);
        }
        /// <summary>
        /// Determine the rectangel layout.
        /// </summary>
        /// <param name="rowIndex">Row index</param>
        /// <param name="colIndex">Col index</param>
        /// <param name="style">Style</param>
        /// <param name="innerBounds">Inner bounds</param>
        /// <param name="buttonsBounds">Buttons bounds.</param>
        /// <returns>Rectangle</returns>
        protected override Rectangle OnLayout(int rowIndex, int colIndex, GridStyleInfo style, Rectangle innerBounds, Rectangle[] buttonsBounds)
        {
            int buttonWidth = 8;

            Rectangle buttonArea = Rectangle.FromLTRB((((innerBounds.Right + 2 - (buttonWidth * 2)))), (innerBounds.Top - 1), (innerBounds.Right), innerBounds.Bottom);
            buttonsBounds[0] = GridUtil.CenterInRect(buttonArea, new Size(buttonWidth, 20));


            return innerBounds;
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
}
