#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Syncfusion.Pdf.Graphics;



namespace Syncfusion.Pdf.Grid
{
    public class PdfGridCell
    {
        #region Fields
        private float m_width = float.MinValue;
        private float m_height = float.MinValue;
        private int m_rowSpan;
        private int m_colSpan;
        private PdfGridRow m_row;
        private PdfGridCellStyle m_style;
        private object m_value;
        private PdfStringFormat m_format;
        private bool m_bIsCellMergeStart;
        private bool m_bIsCellMergeContinue;
        private bool m_bIsRowMergeStart;
        private bool m_bIsRowMergeContinue;
        private bool m_finsh = true;
        private string m_remainingString;
        ///<summary>
        ///Alignment of the image.
        ///</summary>
        private PdfGridImagePosition m_imagePosition = PdfGridImagePosition.Stretch;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public float Width
        {
            get
            {
                if (m_width == float.MinValue)
                    m_width = MeasureWidth();

                return (float)Math.Round(m_width,4);
            }
            internal set
            {
                m_width = value;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public float Height
        {
            get
            {
                if (m_height == float.MinValue)
                    m_height = MeasureHeight();
                return m_height;
            }
            internal set
            {
                m_height = value;
            }
        }

        /// <summary>
        /// Gets or sets the row span.
        /// </summary>
        /// <value>The row span.</value>
        public int RowSpan
        {
            get
            {
                return m_rowSpan;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Invalid span specified, must be greater than or equal to 1");

                if (value > 1)
                {
                    m_rowSpan = value;
                    Row.RowSpanExists = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the column span.
        /// </summary>
        /// <value>The column span.</value>
        public int ColumnSpan
        {
            get
            {
                return m_colSpan;
            }
            set
            {
                if (value < 1)
                    throw new ArgumentException("Invalid span specified, must be greater than or equal to 1");

                if (value > 1)
                {
                    m_colSpan = value;
                    Row.ColumnSpanExists = true;
                }
            }
        }

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        /// <value>The cell style.</value>
        public PdfGridCellStyle Style
        {
            get
            {
                if (m_style == null)
                    m_style = new PdfGridCellStyle();

                return m_style;
            }
            set
            {
                m_style = value;
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = value;
                if (m_value is PdfGrid)
                {
                    (m_value as PdfGrid).Style.AllowHorizontalOverflow = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the string format.
        /// </summary>
        /// <value>The string format.</value>
        public PdfStringFormat StringFormat
        {
            get
            {
                if (m_format == null)
                    m_format = new PdfStringFormat();

                return m_format;
            }
            set
            {
                m_format = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent row.
        /// </summary>
        /// <value>The parent row.</value>
        internal PdfGridRow Row
        {
            get
            {
                return m_row;
            }
            set
            {
                m_row = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cell merge continue.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is cell merge continue; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCellMergeContinue
        {
            get
            {
                return m_bIsCellMergeContinue;
            }
            set
            {
                m_bIsCellMergeContinue = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is cell merge start.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is cell merge start; otherwise, <c>false</c>.
        /// </value>
        internal bool IsCellMergeStart
        {
            get
            {
                return m_bIsCellMergeStart;
            }
            set
            {
                m_bIsCellMergeStart = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is row merge start.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row merge start; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRowMergeStart
        {
            get
            {
                return m_bIsRowMergeStart;
            }
            set
            {
                m_bIsRowMergeStart = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is row merge continue.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is row merge continue; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRowMergeContinue
        {
            get
            {
                return m_bIsRowMergeContinue;
            }
            set
            {
                m_bIsRowMergeContinue = value;
            }
        }

        /// <summary>
        /// Gets the next cell.
        /// </summary>
        /// <value>The next cell.</value>
        internal PdfGridCell NextCell
        {
            get
            {
                return GetNextCell();
            }
        }

        /// <summary>
        /// Gets or sets the remaining string after the row split between pages.
        /// </summary>
        internal string RemainingString
        {
            get
            {
                return m_remainingString;
            }
            set
            {
                m_remainingString = value;
            }
        }
        /// <summary>
        /// Gets or sets whether drawing of cell is completed.
        /// </summary>
        internal bool FinishedDrawingCell
        {
            get
            {
                return m_finsh;
            }
            set
            {
                m_finsh = value;
            }
        }        
       

        /// <summary>
        /// Gets or sets the image alignment type of the PdfGridCell background image.
        /// </summary>
        public PdfGridImagePosition ImagePosition
        {
            get
            {
                return m_imagePosition;
            }
            set
            {
                m_imagePosition = value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridCell"/> class.
        /// </summary>
        public PdfGridCell()
        {
            m_rowSpan = 1;
            m_colSpan = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridCell"/> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public PdfGridCell(PdfGridRow row) 
            : this()
        {
            m_row = row;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Draws the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="bounds">The bounds.</param>
        internal PdfStringLayoutResult Draw(PdfGraphics graphics, RectangleF bounds, bool cancelSubsequentSpans)
        {
            PdfStringLayoutResult result = null;
            if (cancelSubsequentSpans)
            {
                //..Cancel all subsequent cell spans, if no space exists.
                int currentCellIndex = Row.Cells.IndexOf(this);
                for (int i = currentCellIndex + 1; i <= currentCellIndex + m_colSpan; i++)
                {
                    Row.Cells[i].IsCellMergeContinue = false;
                    Row.Cells[i].IsRowMergeContinue = false;
                }
                m_colSpan = 1;
            }

            //..Skip cells which were already covered by spanmap.
            if (m_bIsCellMergeContinue || m_bIsRowMergeContinue)
            {
                if (m_bIsCellMergeContinue && Row.Grid.Style.AllowHorizontalOverflow)
                {
                    if ((Row.RowOverflowIndex > 0 && (Row.Cells.IndexOf(this) != Row.RowOverflowIndex + 1)) || (Row.RowOverflowIndex == 0 && m_bIsCellMergeContinue))
                        return result;
                }
                else
                    return result;
            }

            bounds = AdjustOuterLayoutArea(bounds, graphics);
            DrawCellBackground(ref graphics, bounds);

            PdfPen textPen = GetTextPen();
            PdfBrush textBrush = GetTextBrush();
            PdfFont font = GetTextFont();
            PdfStringFormat strFormat = GetStringFormat();

            RectangleF innerLayoutArea = bounds;
            if (innerLayoutArea.Height >= graphics.ClientSize.Height)
            {
                // If to break row to next page.
                if (this.Row.Grid.AllowRowBreakAcrossPages)
                {
                    innerLayoutArea.Height -= innerLayoutArea.Y;
                    bounds.Height -= bounds.Y;
                }
                // if user choose to cut the row whose height is more than page height.
                else
                {
                    innerLayoutArea.Height = graphics.ClientSize.Height;
                    bounds.Height = graphics.ClientSize.Height;
                }
            }
 
            innerLayoutArea = AdjustContentLayoutArea(innerLayoutArea);

            if (m_value is PdfGrid)
            {
                if ((m_value as PdfGrid).Size.Width > innerLayoutArea.Size.Width
                    //|| (m_value as PdfGrid).Size.Height > innerLayoutArea.Size.Height
                    )
                    throw new PdfException("Can't draw one or more inner grids, no enough space available for it.");

                PdfGrid childGrid = m_value as PdfGrid;
                childGrid.IsChildGrid = true;
                childGrid.ParentCell = this;

                PdfGridLayouter layouter = new PdfGridLayouter(childGrid);

                // Get layout format from parent grid, required to maintain pagination bounds.
                PdfLayoutFormat format = new PdfGridLayoutFormat();
                if (Row.Grid.LayoutFormat != null)
                    format = Row.Grid.LayoutFormat;
                else
                    format.Layout = PdfLayoutType.Paginate;
                if (graphics.Layer != null)
                {
                    // Define layout parameters.
                    PdfLayoutParams param = new PdfLayoutParams();
                    param.Page = graphics.Page as PdfPage;
                    param.Bounds = innerLayoutArea;
                    param.Format = format;

                    // Draw the child grid.
                    PdfLayoutResult childGridResult = layouter.Layout(param);
                    if (param.Page != childGridResult.Page)
                    {
                        Row.NestedGridLayoutResult = childGridResult;

                        // After drawing paginated nested grid, the bounds of the parent grid in start page should be corrected for borders.
                        bounds.Height = graphics.ClientSize.Height - bounds.Y;
                    }
                }
                else
                {
                    layouter = new PdfGridLayouter(m_value as PdfGrid);
                    layouter.Layout(graphics, innerLayoutArea);
                }
            }
            else if(m_value is string || m_remainingString is string)
            {
                string temp;
                if (m_finsh)
                {
                    temp = m_remainingString == string.Empty ? m_remainingString : (string)m_value;
                    graphics.DrawString(temp, font, textPen, textBrush, innerLayoutArea, strFormat);
                }
                else
                    graphics.DrawString((string)m_remainingString, font, textPen, textBrush, innerLayoutArea, strFormat);

                result = graphics.StringLayoutResult;
            }

            if (Style.Borders != null)
            {
                DrawCellBorders(ref graphics, bounds);
            }

            return result;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Calculates the width.
        /// </summary>
        /// <returns></returns>
        private float MeasureWidth()
        {
            // .. Calculate the cell text width.
            // .....Add border widths, cell spacings and paddings to the width.

            float width = 0;
            PdfStringLayouter layouter = new PdfStringLayouter();
            if (m_value is string)
            {
                PdfStringLayoutResult slr = layouter.Layout((string)m_value, GetTextFont(), StringFormat, new SizeF(float.MaxValue, float.MaxValue));
                width += slr.ActualSize.Width;
                width += (Style.Borders.Left.Width + Style.Borders.Right.Width) * 2;
            }
            else if (m_value is PdfGrid)
            {

                width = (m_value as PdfGrid).Size.Width;
            }

            width += (Row.Grid.Style.CellPadding.Left + Row.Grid.Style.CellPadding.Right);
            width += Row.Grid.Style.CellSpacing;

            return width;
        }

        /// <summary>
        /// Calcualtes the height.
        /// </summary>
        /// <returns></returns>
        internal float MeasureHeight()
        {
            // .. Calculate the cell text height.
            // .....Add border widths, cell spacings and paddings to the height.
            float width = CalculateWidth();
            width -= (m_row.Grid.Style.CellPadding.Right + m_row.Grid.Style.CellPadding.Left);
            width -= (Style.Borders.Left.Width + Style.Borders.Right.Width);

            float height = 0;
            PdfStringLayouter layouter = new PdfStringLayouter();
            if (m_value is string || m_remainingString is string)
            {
                string currentValue = (string)m_value;
                if (!m_finsh)
                    currentValue = !String.IsNullOrEmpty(m_remainingString) ? m_remainingString : (string)m_value;
                PdfStringLayoutResult slr = layouter.Layout(currentValue, GetTextFont(), StringFormat, new SizeF(width, float.MaxValue));
                height += slr.ActualSize.Height;
                height += (Style.Borders.Top.Width + Style.Borders.Bottom.Width) * 2;
            }
            else if (m_value is PdfGrid)
            {
                height = (m_value as PdfGrid).Size.Height;
            }

            height += (Row.Grid.Style.CellPadding.Top + Row.Grid.Style.CellPadding.Bottom);
            height += Row.Grid.Style.CellSpacing;

            return height;
        }

        /// <summary>
        /// Adjusts the outer layout area.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        private RectangleF AdjustOuterLayoutArea(RectangleF bounds, PdfGraphics g)
        {
            bool isHeader = false;

            float cellSpacing = Row.Grid.Style.CellSpacing;
            if (cellSpacing > 0)
                bounds = new RectangleF(bounds.X + cellSpacing, bounds.Y + cellSpacing,
                    bounds.Width - cellSpacing, bounds.Height - cellSpacing);

            int currentColIndex = Row.Cells.IndexOf(this);

            if (ColumnSpan > 1 || (Row.RowOverflowIndex > 0 && (currentColIndex == Row.RowOverflowIndex + 1) && m_bIsCellMergeContinue))
            {
                int span = ColumnSpan;

                if (currentColIndex < Row.RowOverflowIndex)
                    span = Row.RowOverflowIndex - currentColIndex + 1;

                if (span == 1 && m_bIsCellMergeContinue)
                {
                    for (int j = currentColIndex + 1; j < Row.Grid.Columns.Count; j++)
                    {
                        if (Row.Cells[j].m_bIsCellMergeContinue)
                            span++;
                        else
                            break;
                    }
                }

                float totalWidth = 0;
                for (int i = currentColIndex; i < currentColIndex + span; i++)
                {
                    if (Row.Grid.Style.AllowHorizontalOverflow)
                    {
                        float width = bounds.X + totalWidth + Row.Grid.Columns[i].Width;
                        float compWidth = Row.Grid.Size.Width < g.ClientSize.Width ? Row.Grid.Size.Width : g.ClientSize.Width;

                        if (width > compWidth)
                            break;
                    }
                    totalWidth += Row.Grid.Columns[i].Width;
                }
                totalWidth -= Row.Grid.Style.CellSpacing;
                bounds.Width = totalWidth;
            }

            if (RowSpan > 1 || Row.RowSpanExists)
            {
                int span = RowSpan;
                int currentRowIndex = Row.Grid.Rows.IndexOf(Row);

                if (currentRowIndex == -1)
                {
                    currentRowIndex = Row.Grid.Headers.IndexOf(Row);
                    if (currentRowIndex != -1)
                        isHeader = true;
                }

                if (span == 1 && m_bIsCellMergeContinue)
                {
                    for (int j = currentRowIndex + 1; j < Row.Grid.Rows.Count; j++)
                    {
                        bool flag = (isHeader ? Row.Grid.Headers[j].Cells[currentColIndex].m_bIsCellMergeContinue : Row.Grid.Rows[j].Cells[currentColIndex].m_bIsCellMergeContinue);
                        if (flag)
                            span++;
                        else
                            break;
                    }
                }

                float totalHeight = 0;
                for (int i = currentRowIndex; i < currentRowIndex + span; i++)
                {
                    totalHeight += (isHeader ? Row.Grid.Headers[i].Height : Row.Grid.Rows[i].Height);
                }
                totalHeight -= Row.Grid.Style.CellSpacing;
                bounds.Height = totalHeight;
            }

            return bounds;
        }

        /// <summary>
        /// Adjusts the text layout area.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns></returns>
        private RectangleF AdjustContentLayoutArea(RectangleF bounds)
        {
            if (m_value is PdfGrid)
            {
                SizeF size = (m_value as PdfGrid).Size;

                bounds.Width -= (m_row.Grid.Style.CellPadding.Right + m_row.Grid.Style.CellPadding.Left);
                bounds.Height -= (m_row.Grid.Style.CellPadding.Bottom + m_row.Grid.Style.CellPadding.Top);
                
				if (this.StringFormat.Alignment == PdfTextAlignment.Center)
                {
                    bounds.X += m_row.Grid.Style.CellPadding.Left + (bounds.Width - size.Width) / 2;
                    bounds.Y += m_row.Grid.Style.CellPadding.Top + (bounds.Height - size.Height) / 2;
                }
                else if (this.StringFormat.Alignment == PdfTextAlignment.Left)
                {
                    bounds.X += m_row.Grid.Style.CellPadding.Left;
                    bounds.Y += m_row.Grid.Style.CellPadding.Top;
                }
                else if (this.StringFormat.Alignment == PdfTextAlignment.Right)
                {
                    bounds.X += m_row.Grid.Style.CellPadding.Left + (bounds.Width - size.Width);
                    bounds.Y += m_row.Grid.Style.CellPadding.Top;
                }

            }
            else
            {
                bounds.X += m_row.Grid.Style.CellPadding.Left;
                bounds.Y += m_row.Grid.Style.CellPadding.Top;
                bounds.Width -= (m_row.Grid.Style.CellPadding.Right + m_row.Grid.Style.CellPadding.Left);
                bounds.Height -= (m_row.Grid.Style.CellPadding.Bottom + m_row.Grid.Style.CellPadding.Top);
            }
            
            return bounds;
        }

        /// <summary>
        /// Draws the cell border constructed by drawing lines.
        /// </summary>
        /// <param name="gr">The Current Graphics.</param>
        /// <param name="cs">The CellStyle.</param>
        /// <param name="bounds">The bounds.</param>
        internal void DrawCellBorders(ref PdfGraphics graphics, RectangleF bounds)
        {
            if (Row.Grid.Style.BorderOverlapStyle == PdfBorderOverlapStyle.Inside)
            {
                bounds.X += Style.Borders.Left.Width;
                bounds.Y += Style.Borders.Top.Width;
                bounds.Width -= Style.Borders.Right.Width;
                bounds.Height -= Style.Borders.Bottom.Width;
            }

            if (Style.Borders.IsAll)
            {
                SetTransparency(ref graphics, m_style.Borders.Left);
                graphics.DrawRectangle(m_style.Borders.Left, bounds);
                graphics.Restore();
                return;
            }
            else
            {
                PointF p1 = new PointF(bounds.X, bounds.Y + bounds.Height);
                PointF p2 = bounds.Location;
                PdfPen pen = m_style.Borders.Left;
                if (pen.IsImmutable)
                    pen = new PdfPen(m_style.Borders.Left.Color, m_style.Borders.Left.Width);
                pen.LineCap = PdfLineCap.Square;
                SetTransparency(ref graphics, pen);
                graphics.DrawLine(pen, p1, p2);
                graphics.Restore();

                p1 = new PointF(bounds.X + bounds.Width, bounds.Y);
                p2 = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
                pen = m_style.Borders.Right;
                if (pen.IsImmutable)
                    pen = new PdfPen(m_style.Borders.Right.Color, m_style.Borders.Right.Width);
                pen.LineCap = PdfLineCap.Square;
                SetTransparency(ref graphics, pen);
                graphics.DrawLine(pen, p1, p2);
                graphics.Restore();

                p1 = bounds.Location;
                p2 = new PointF(bounds.X + bounds.Width, bounds.Y);
                pen = m_style.Borders.Top;
                if (pen.IsImmutable)
                    pen = new PdfPen(m_style.Borders.Top.Color, m_style.Borders.Top.Width);
                pen.LineCap = PdfLineCap.Square;
                SetTransparency(ref graphics, pen);
                graphics.DrawLine(pen, p1, p2);
                graphics.Restore();

                p1 = new PointF(bounds.X + bounds.Width, bounds.Y + bounds.Height);
                p2 = new PointF(bounds.X, bounds.Y + bounds.Height);
                pen = m_style.Borders.Bottom;
                if (pen.IsImmutable)
                    pen = new PdfPen(m_style.Borders.Bottom.Color, m_style.Borders.Bottom.Width);
                pen.LineCap = PdfLineCap.Square;
                SetTransparency(ref graphics, pen);
                graphics.DrawLine(pen, p1, p2);
                graphics.Restore();
            }
        }

        /// <summary>
        /// Sets the transparency.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="pen">The pen.</param>
        private void SetTransparency(ref PdfGraphics graphics, PdfPen pen)
        {
            float alpha = (float)pen.Color.A / 255.0f;
            graphics.Save();
            graphics.SetTransparency(alpha);
        }

        /// <summary>
        /// Gets the next cell.
        /// </summary>
        /// <returns></returns>
        private PdfGridCell GetNextCell()
        {
            int currentCellIndex = m_row.Cells.IndexOf(this);
            if (currentCellIndex + 1 <= m_row.Cells.Count)
                return m_row.Cells[currentCellIndex + 1];
            else
                return null;
        }

        private void DrawCellBackground(ref PdfGraphics graphics, RectangleF bounds)
        {
            PdfBrush backgroundBrush = GetBackgroundBrush();

            if (backgroundBrush != null)
            {
                graphics.Save();
                graphics.DrawRectangle(backgroundBrush, bounds);
                graphics.Restore();
            }
            
            if (Style.BackgroundImage != null)
            {
                PdfImage image = Style.BackgroundImage;
                if (m_imagePosition == PdfGridImagePosition.Stretch)
                {
                    graphics.DrawImage(Style.BackgroundImage, bounds);
                    }
                else if (m_imagePosition == PdfGridImagePosition.Center)
                {

                    float imageX = 0, imageY = 0, gridCentreX, gridCentreY;
                    gridCentreX = bounds.X + (bounds.Width / 2);
                    gridCentreY = bounds.Y + (bounds.Height / 2);
                    imageX = gridCentreX - (image.Width / 2);
                    imageY = gridCentreY - (image.Height / 2);
                    graphics.DrawImage(image, imageX, imageY, image.Width, image.Height);

                }
                else if (m_imagePosition == PdfGridImagePosition.Fit)
                {
                    float cellWidth=bounds.Width, cellHeight=bounds.Height;
                    float imageWidth=image.PhysicalDimension.Width, imageHeight=image.PhysicalDimension.Height;
                    float ratio, x, y;
                    if (cellHeight <= cellWidth)
                    {
                        if (imageHeight > imageWidth)
                        {
                            y = bounds.Y;
                            ratio = cellHeight / imageHeight;
                            imageHeight = cellHeight;
                            imageWidth *= ratio;
                            x = bounds.X + (bounds.Width - imageWidth) / 2;
                            graphics.DrawImage(image, x, y, imageWidth, imageHeight);
                        }
                        else
                        {
                            x = bounds.X;
                            ratio = cellWidth / imageWidth;
                            imageWidth = cellWidth;
                            imageHeight *= ratio;
                            y = bounds.Y + (bounds.Height - imageHeight) / 2;
                            graphics.DrawImage(image, x, y, imageWidth, imageHeight);
                        }
                    }
                    else if (cellHeight > cellWidth)
                    {
                        if (imageHeight < imageWidth)
                        {
                            x = bounds.X;
                            ratio = cellWidth / imageWidth;
                            imageWidth = cellWidth;
                            imageHeight *= ratio;
                            y = bounds.Y + (bounds.Height - imageHeight) / 2;
                            graphics.DrawImage(image, x, y, imageWidth, imageHeight);
                        }
                        else
                        {
                            y = bounds.Y;
                            ratio = cellHeight / imageHeight;
                            imageHeight = cellHeight;
                            imageWidth *= ratio;
                            x = bounds.X + (bounds.Width - imageWidth) / 2;
                            graphics.DrawImage(image, x, y, imageWidth, imageHeight);
                        }
                    }
                }
                else if (m_imagePosition == PdfGridImagePosition.Tile)
                {
                    float cellLeft = bounds.X, cellTop = bounds.Y;
                    float imageWidth = 0, imageHeight = 0;
                    float x = cellLeft, y = cellTop;
                    float remainingWidth = bounds.Width;
                    PdfUnitConvertor converter = new PdfUnitConvertor();

                    for (; y < bounds.Bottom; )
                    {
                        
                        for (x = cellLeft; x < bounds.Right; )
                        {
#if !SILVERLIGHT && !NETFX_CORE && !WP
                            if (x + image.PhysicalDimension.Width > bounds.Right && y + image.PhysicalDimension.Height > bounds.Bottom)
                            {
                                
                                imageWidth = converter.ConvertToPixels(bounds.Right - x, PdfGraphicsUnit.Point);
                                imageHeight = converter.ConvertToPixels(bounds.Bottom - y, PdfGraphicsUnit.Point);
                                Rectangle cropRectangle = new Rectangle(0, 0, (int)imageWidth, (int)imageHeight);

                                Bitmap tileImage = new Bitmap(image.InternalImage, new Size((int)image.Width, (int)image.Height));
                                Bitmap cropImage = tileImage.Clone(cropRectangle, tileImage.PixelFormat);
                                MemoryStream stream = new MemoryStream();
                                if (Bitmap.IsAlphaPixelFormat(image.InternalImage.PixelFormat) || image.InternalImage is System.Drawing.Imaging.Metafile)
                                    cropImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                else
                                    cropImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                PdfBitmap pdfTileImage = new PdfBitmap(stream);
                                graphics.DrawImage(pdfTileImage, x, y);
                                stream.Dispose();
                                tileImage.Dispose();
                                cropImage.Dispose();
                                pdfTileImage.Dispose();                         
                            }

                               
                            else if (x + image.PhysicalDimension.Width > bounds.Right)
                            {
                                imageWidth = converter.ConvertToPixels(bounds.Right - x,PdfGraphicsUnit.Point);
                                Rectangle cropRectangle = new Rectangle(0, 0, (int)imageWidth, (int)image.Height);
                              
                                Bitmap tileImage = new Bitmap(image.InternalImage, new Size((int)image.Width, (int)image.Height));
                                Bitmap cropImage = tileImage.Clone(cropRectangle, tileImage.PixelFormat);
                                MemoryStream stream = new MemoryStream();
                                if (Bitmap.IsAlphaPixelFormat(image.InternalImage.PixelFormat) || image.InternalImage is System.Drawing.Imaging.Metafile)
                                    cropImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                else
                                    cropImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                PdfBitmap pdfTileImage = new PdfBitmap(stream);
                                graphics.DrawImage(pdfTileImage, x, y);
                                stream.Dispose();
                                tileImage.Dispose();
                                cropImage.Dispose();
                                pdfTileImage.Dispose(); 
                         
                            }
                               
                            else if (y + image.PhysicalDimension.Height > bounds.Bottom)
                            {
                                imageHeight = converter.ConvertToPixels(bounds.Bottom - y,PdfGraphicsUnit.Point);
                                Rectangle cropRectangle = new Rectangle(0, 0, (int)image.Width, (int)imageHeight);

                                Bitmap tileImage = new Bitmap(image.InternalImage, new Size((int)image.Width, (int)image.Height));
                                Bitmap cropImage = tileImage.Clone(cropRectangle, tileImage.PixelFormat);
                                MemoryStream stream = new MemoryStream();
                                
                                if (Bitmap.IsAlphaPixelFormat(image.InternalImage.PixelFormat)||image.InternalImage is System.Drawing.Imaging.Metafile)
                                    cropImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                                else
                                    cropImage.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                                PdfBitmap pdfTileImage = new PdfBitmap(stream);
                                graphics.DrawImage(pdfTileImage, x, y);
                                stream.Dispose();
                                tileImage.Dispose();
                                cropImage.Dispose();
                                pdfTileImage.Dispose(); 
                            }
                                
                            else
                            {
                                graphics.DrawImage(image, new PointF(x, y));
                            }
#else
                            if (x + image.PhysicalDimension.Width < bounds.Right && y + image.PhysicalDimension.Height < bounds.Bottom)
                                graphics.DrawImage(image, new PointF(x, y));
#endif
                            x += image.PhysicalDimension.Width;
                        }
                        y += image.PhysicalDimension.Height;

                    }

                }
                
            }
        }

        private PdfStringFormat GetStringFormat()
        {
            return Style.StringFormat ?? StringFormat;
        }

        /// <summary>
        /// Gets the text font.
        /// </summary>
        /// <returns></returns>
        private PdfFont GetTextFont()
        {
            return Style.Font ?? Row.Style.Font ?? Row.Grid.Style.Font ?? PdfDocument.DefaultFont;
        }

        /// <summary>
        /// Gets the text brush.
        /// </summary>
        /// <returns></returns>
        private PdfBrush GetTextBrush()
        {
            return Style.TextBrush ?? Row.Style.TextBrush ?? Row.Grid.Style.TextBrush ?? PdfBrushes.Black;
        }

        /// <summary>
        /// Gets the text pen.
        /// </summary>
        /// <returns></returns>
        private PdfPen GetTextPen()
        {
            return Style.TextPen ?? Row.Style.TextPen ?? Row.Grid.Style.TextPen;
        }

        /// <summary>
        /// Gets the background brush.
        /// </summary>
        /// <returns></returns>
        private PdfBrush GetBackgroundBrush()
        {
            return Style.BackgroundBrush ?? Row.Style.BackgroundBrush ?? Row.Grid.Style.BackgroundBrush;
        }

        float CalculateWidth()
        {
            int cellIndex = Row.Cells.IndexOf(this);
            int columnSpan = this.ColumnSpan;
            float width = 0;

            for (int i = 0; i < columnSpan; i++)
            {
                width += Row.Grid.Columns[cellIndex + i].Width;
            }

            return width;
        }
        #endregion
    }

    public class PdfGridCellCollection : IEnumerable
    {
        #region Fields
        private PdfGridRow m_row;
        private List<PdfGridCell> m_cells = new List<PdfGridCell>();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridCellCollection"/> class.
        /// </summary>
        internal PdfGridCellCollection(PdfGridRow row)
        {
            m_row = row;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="Syncfusion.Pdf.Grid.PdfGridCell"/> at the specified index.
        /// </summary>
        /// <value></value>
        public PdfGridCell this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new IndexOutOfRangeException();

                return (m_cells[index]);
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return m_cells.Count;
            }
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Adds this instance.
        /// </summary>
        /// <returns></returns>
        internal PdfGridCell Add()
        {
            PdfGridCell cell = new PdfGridCell();
            cell.Style = ((PdfGridStyleBase)m_row.Style as PdfGridCellStyle);
            Add(cell);
            return cell;
        }

        /// <summary>
        /// Adds the specified cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        internal void Add(PdfGridCell cell)
        {
            if(cell.Style == null)
               cell.Style = ((PdfGridStyleBase)m_row.Style as PdfGridCellStyle);

            cell.Row = m_row;

            m_cells.Add(cell);
        }

        /// <summary>
        /// Returns the index of a particular cell in the collection.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <returns></returns>
        public int IndexOf(PdfGridCell cell)
        {
            return m_cells.IndexOf(cell);
        }
        #endregion

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return new PdfGridCellEnumerator(this);
        }
        #endregion

        #region Internals
        /// <summary>
        /// Column collection enumerator.
        /// </summary>
        private struct PdfGridCellEnumerator : IEnumerator
        {
            #region Fields
            private PdfGridCellCollection m_cellCollection;
            private int m_currentIndex;
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="PdfGridCellEnumerator"/> struct.
            /// </summary>
            /// <param name="columnCollection">The column collection.</param>
            internal PdfGridCellEnumerator(PdfGridCellCollection columnCollection)
            {
                if (columnCollection == null)
                    throw new ArgumentNullException("columnCollection");

                m_cellCollection = columnCollection;
                m_currentIndex = -1;
            }
            #endregion

            #region IEnumerator Members
            /// <summary>
            /// Gets the current.
            /// </summary>
            /// <value>The current.</value>
            public object Current
            {
                get
                {
                    CheckIndex();
                    return m_cellCollection[m_currentIndex];
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public bool MoveNext()
            {
                ++m_currentIndex;

                return (m_currentIndex < m_cellCollection.Count);
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">
            /// The collection was modified after the enumerator was created.
            /// </exception>
            public void Reset()
            {
                m_currentIndex = -1;
            }
            #endregion

            #region Helper methods
            /// <summary>
            /// Checks the index.
            /// </summary>
            private void CheckIndex()
            {
                if (m_currentIndex < 0 || m_currentIndex >= m_cellCollection.Count)
                    throw new IndexOutOfRangeException();
            }
            #endregion
        }
        #endregion
    }
}
