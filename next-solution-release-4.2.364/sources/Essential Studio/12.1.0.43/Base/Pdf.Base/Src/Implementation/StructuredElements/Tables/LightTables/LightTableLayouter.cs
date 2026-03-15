#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System;
using System.Collections;
using System.Drawing;
using Syncfusion.Pdf.Graphics;

/// <summary>
/// The Syncfusion.Pdf.Tables namespace contains classes for creating tables.
/// </summary>
namespace Syncfusion.Pdf.Tables
{
    /// <summary>
    /// Holds the lay outing of tables.
    /// </summary>
    /// <seealso cref="ElementLayouter"/> Class    
    internal class LightTableLayouter : ElementLayouter
    {
        #region Fields
        /// <summary>
        /// Cell values of a row being shared by pages.
        /// </summary>
        private string[] m_row;

        /// <summary>
        /// The latest text results.
        /// </summary>
        private PdfStringLayoutResult[] m_latestTextResults;

        /// <summary>
        /// Current cell width.
        /// </summary>
        private float[] m_cellWidths;

        /// <summary>
        /// Current page.
        /// </summary>
        private PdfPage m_currentPage;

        /// <summary>
        /// The chached bounds of the current page.
        /// </summary>
        private SizeF m_currentPageBounds;

        /// <summary>
        /// Holds the current graphics and is used when current page
        /// is not available (e.g. drawing on a graphics).
        /// </summary>
        private PdfGraphics m_currentGraphics;

        /// <summary>
        /// Current bounds.
        /// </summary>
        private RectangleF m_currentBounds;

        /// <summary>
        /// Stores cell spacing value.
        /// </summary>
        private float m_cellSpacing;

        /// <summary>
        /// Holds an array of integers that specify column spanning (horizontal mergin)
        /// </summary>
        private int[] m_spanMap;

        /// <summary>
        /// The index of the row dropped to the next page.
        /// </summary>
        private int m_dropIndex;

        /// <summary>
        /// The index of the start column.
        /// </summary>
        private int m_startColumn;

        private int m_endColumn;

        private int m_previousRowIndex=-1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets shape element.
        /// </summary>
        public PdfLightTable Table
        {
            get
            {
                return (base.Element as PdfLightTable);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LightTableLayouter"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        internal LightTableLayouter(PdfLightTable table)
            : base(table)
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Layouts the table on the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="location">The location.</param>
        public void Layout(PdfGraphics graphics, PointF location)
        {
            RectangleF boundaries = new RectangleF(location, SizeF.Empty);

            Layout(graphics, boundaries);
        }

        /// <summary>
        /// Layouts the table on the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="boundaries">The boundaries.</param>
        public void Layout(PdfGraphics graphics, RectangleF boundaries)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            
			if (graphics.ClientSize.Height < 0)
                boundaries.Y += graphics.ClientSize.Height;

            // Determine the width.
            float width = graphics.ClientSize.Width - boundaries.X;
            // Layout row by row.
            PdfLayoutParams param = new PdfLayoutParams();
            param.Bounds = boundaries;
            m_currentGraphics = graphics;
            LayoutInternal(param);
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected override PdfLayoutResult LayoutInternal(PdfLayoutParams param)
        {
            if (param == null)
                throw new ArgumentNullException("param");

            PdfLightTableLayoutFormat format = GetFormat(param.Format);

            if (format != null)
            {
                m_startColumn = format.StartColumnIndex;
                m_endColumn = format.EndColumnIndex;
            }

            if ( /*m_startColumn == 0 &&*/ m_endColumn == 0)
            {
                m_endColumn = Table.Columns.Count - 1;
            }

            if (m_endColumn < m_startColumn)
                throw new PdfLightTableException("End column index is less than start column index.");

            int count = Table.Columns.Count;

            if (m_startColumn < 0 || m_startColumn >= count || m_endColumn >= count || m_endColumn - m_startColumn > count)
                throw new PdfLightTableException("The selected columns are out of the existing range.");

            m_dropIndex = -2; // Default value of the dropped index.
            m_row = null;
            m_latestTextResults = null;

            m_currentPage = param.Page;
            m_currentPageBounds = (m_currentPage != null) ?
                m_currentPage.GetClientSize() :
                m_currentGraphics.ClientSize;

            m_currentBounds = param.Bounds;
            PdfLightTableLayoutResult result = null;
            PageLayoutResult pageResult = null;

            //if (format != null && param.Format.UsePaginateBounds)
			//m_currentBounds = GetPaginateBounds(param);

            if (m_currentBounds.Width <= 0)
            {
                float width = m_currentPageBounds.Width - m_currentBounds.X;

                if (width < 0)
                    throw new PdfLightTableException("Can't draw table outside of the page.");

                m_currentBounds.Width = width;
                
            }
			param.Bounds = m_currentBounds;	
				
            PdfLightTableStyle props = Table.Style;

            m_cellSpacing = props.CellSpacing;

            int startRowIndex = (props.HeaderSource == PdfHeaderSource.Rows) ? props.HeaderRowCount : 0;
            // Retrieve cell widths.
            m_cellWidths = GetWidths(param.Bounds);
            bool isPageFirst = true;

            while (true)
            {
                // Raise page drawing event.
                bool cancel = RaiseBeforePageLayout(m_currentPage, ref m_currentBounds, ref startRowIndex);
                LightTableEndPageLayoutEventArgs endArgs = null;

                if (!cancel)
                {
                    // Layout on a page
                    pageResult = LayoutOnPage(startRowIndex, param, isPageFirst);

                    // Raise page drawn event.
                    endArgs = RaisePageLayouted(pageResult);
                    cancel = (endArgs == null) ? false : endArgs.Cancel;
                }

                if (cancel || pageResult.Finish)
                {
                    result = GetLayoutResult(pageResult);
                    break;
                }

                m_currentPage = GetNextPage(m_currentPage);
                m_currentPageBounds = (m_currentPage != null) ?
                    m_currentPage.GetClientSize() :
                    m_currentGraphics.ClientSize;

                isPageFirst = false;
                startRowIndex = pageResult.LastRowIndex;
                m_currentBounds = GetPaginateBounds(param);

                if (m_currentBounds.Height == 0)
                {
                    m_currentBounds.Y = 0;
                }
            }

            return result;
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <param name="format">The format structure passed through parameters.</param>
        /// <returns>
        /// PdfLightTableLayoutFormat class instance.
        /// </returns>
        private PdfLightTableLayoutFormat GetFormat(PdfLayoutFormat format)
        {
            PdfLightTableLayoutFormat f = format as PdfLightTableLayoutFormat;

            if (format != null && f == null)
            {
                f = new PdfLightTableLayoutFormat(format);
            }

            return f;
        }

        /// <summary>
        /// Gets the layout result.
        /// </summary>
        /// <param name="pageResult">The page result.</param>
        /// <returns>Table layout result.</returns>
        private PdfLightTableLayoutResult GetLayoutResult(PageLayoutResult pageResult)
        {
            PdfPage page = (pageResult != null) ? pageResult.Page : m_currentPage;
            RectangleF bounds = (pageResult != null) ? pageResult.Bounds : RectangleF.Empty;

            PdfLightTableLayoutResult result = new PdfLightTableLayoutResult(page, bounds,
                pageResult.LastRowIndex, m_latestTextResults);

            return result;
        }

        /// <summary>
        /// Layouts the table part on a page.
        /// </summary>
        /// <param name="startRowIndex">Start row index.</param>
        /// <param name="param">The lay outing parameters.</param>
        /// <param name="isPageFirst">if set to <c>true</c> the current page is the first one.</param>
        /// <returns>Result of the lay outing.</returns>
        private PageLayoutResult LayoutOnPage(int startRowIndex, PdfLayoutParams param, bool isPageFirst)
        {
            // Draw header
            // ..Check if there is enough place for header.
            // ..Draw row by row.
            // Draw body 
            // .. Check if there is enough place for a part of rows
            // .. Draw as many rows as possible.
            int rowIndex = startRowIndex;

            RectangleF rowBounds = m_currentBounds;

            if (rowBounds.Height == 0 && m_currentPage != null)
            {
                rowBounds.Height = m_currentPageBounds.Height - rowBounds.Y;
            }

            RectangleF bounds = rowBounds;
            PdfLightTableStyle tableStyle = Table.Style;

            PdfPen borderPen = tableStyle.BorderPen;

            if (borderPen != null)
            {
                rowBounds = PreserveForBorder(rowBounds, borderPen, tableStyle.BorderOverlapStyle);
            }

            // Reserve place for the last spacing gaps.
            rowBounds.Height -= m_cellSpacing;
            rowBounds.Width -= m_cellSpacing;

            float rowHeight = 0;
            PageLayoutResult result = new PageLayoutResult();
            bool isEmpty = false;

            bool header = tableStyle.ShowHeader && (isPageFirst || tableStyle.RepeatHeader);
            bool useColumnCaptions = tableStyle.HeaderSource != PdfHeaderSource.Rows;
            int headerRowsCount = tableStyle.HeaderRowCount;

            if (header && !useColumnCaptions)
            {
                if (headerRowsCount > 0)
                {
                    rowIndex = 0;
                }
                else
                {
                    header = false;
                }
            }

            string[] prevRow = m_row;

            if (header)
            {
                m_row = null;
            }

            PdfGraphics graphics = (m_currentPage != null) ? m_currentPage.Graphics : m_currentGraphics;

            while (true)
            {
                // Retrieve a row data.
                string[] row = null;

                if (header && useColumnCaptions)
                {
                    rowIndex = -1;
                    m_previousRowIndex = -2;
                    row = Table.GetColumnCaptions();

                    if (row == null)
                    {
                        header = false;
                        rowIndex = startRowIndex;
                        m_row = prevRow;
                        continue;
                    }
                    else // Resize to [start index; end index]
                    {
                        row = CropRow(row);
                    }
                }
                else

                {
#if SILVERLIGHT || NETFX_CORE || WP
                   if (rowIndex < this.Table.Rows.Count)
                        row = GetRow(rowIndex, param);                                           
#else
                        row = GetRow(rowIndex, param); 
#endif
                }

                bool stop = (row == null);

                if (row != null)
                {
                    // Skip cell spacing.
                    rowBounds.Y += m_cellSpacing;
                    rowBounds.Height -= m_cellSpacing;

                    // Draw a row and raise the events.
                    bool isIncomplete = DrawRow(param, ref rowIndex, row, rowBounds, out rowHeight, header, out stop);

                    rowBounds.Y += rowHeight;
                    rowBounds.Height -= rowHeight;

                    stop |= isIncomplete;
                    isEmpty |= (rowHeight <= 0 && (startRowIndex == rowIndex || header));

                    stop |= isEmpty;
                }
                else
                {
                    result.Finish = true;
                }

                if (stop)
                {
                    if (rowHeight > 0)
                    {
                        rowBounds.Y += m_cellSpacing;
                    }

                    if (borderPen != null)
                    {
                        rowBounds.Y += borderPen.Width;
                    }

                    result.Page = m_currentPage;
                    result.FirstRowIndex = startRowIndex;
                    result.LastRowIndex = rowIndex;
                    result.Bounds = bounds;
                    result.Bounds.Height = rowBounds.Y - bounds.Y;

                    RectangleF b = result.Bounds;

                    if (borderPen != null)
                    {
                        if (tableStyle.BorderOverlapStyle == PdfBorderOverlapStyle.Overlap)
                        {
                            b.Height -= borderPen.Width / 2;
                        }

                        b = PreserveForBorder(b, borderPen, PdfBorderOverlapStyle.Overlap);
                    }

                    // Draw border.
                    if (borderPen != null && (b.Bottom < m_currentPageBounds.Height))
                    {
                        float alpha = (float)borderPen.Color.A / 255.0f;

                        graphics.Save();

                        graphics.SetTransparency(alpha);

                        graphics.DrawRectangle(borderPen, b);

                        graphics.Restore();

                    }

                    break;
                }

                if (header)
                {
                    if (useColumnCaptions || rowIndex >= headerRowsCount)
                    {
                        header = false;
                        rowIndex = startRowIndex;
                        m_row = prevRow;
                    }
                }
            }

            bool layoutOnePage = (param.Format != null) ?
                (param.Format.Layout == PdfLayoutType.OnePage) : true;

            result.Finish |= ( /*isEmpty ||*/ layoutOnePage);

            if (isEmpty || (m_row != null && header))
                throw new PdfLightTableException("Can't draw table, because there is not enough space for it.");




            return result;
        }

        /// <summary>
        /// Crops a row to make it fit the starting and ending columns.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>The cropped row.</returns>
        private string[] CropRow(string[] row)
        {
            string[] r = row;

            if (row != null && !(m_endColumn == 0 && m_startColumn == 0))
            {
                int length = m_endColumn - m_startColumn + 1;
                r = new string[length];

                Array.Copy(row, m_startColumn, r, 0, length);
            }
            return r;
        }

        /// <summary>
        /// Resizes rectangle so that the border will be drawn insize the bounds specified.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="pen">The pen.</param>
        /// <returns></returns>
        private static RectangleF PreserveForBorder(RectangleF bounds, PdfPen pen, PdfBorderOverlapStyle overlapStyle)
        {
            // Resize bounds to allow border fit into boundaries.
            if (pen != null)
            {
                float penWidth = pen.Width;

                if (overlapStyle == PdfBorderOverlapStyle.Overlap)
                {
                    float halfWidth = penWidth / 2f;
                    bounds.X += halfWidth;
                    bounds.Y += halfWidth;
                    bounds.Width -= penWidth;
                    bounds.Height -= penWidth;
                }
                else if (overlapStyle == PdfBorderOverlapStyle.Inside)
                {
                    float doubleWidth = penWidth * 2f;
                    bounds.X += penWidth;
                    bounds.Y += penWidth;
                    bounds.Width -= doubleWidth;
                    bounds.Height -= doubleWidth;
                }
                else
                {
                    throw new ArgumentException("Unsupported overlap style.");
                }
            }

            return bounds;
        }

        /// <summary>
        /// Draws a row.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="row">The row data.</param>
        /// <param name="rowBouds">The row bouds.</param>
        /// <param name="rowHeight">Height of the row.</param>
        /// <param name="isHeader">if it is header, set to <c>true</c>.</param>
        /// <param name="stop">if it is stop, set to <c>true</c>.</param>
        /// <returns>Indicator if the row was drawn partly.</returns>
        private bool DrawRow(PdfLayoutParams param, ref int rowIndex, string[] row,
            RectangleF rowBouds, out float rowHeight, bool isHeader, out bool stop)
        {
            int count = m_cellWidths.Length;
            PdfStringLayoutResult[] results = null;
            bool headerFormat;
            PdfCellStyle cs = GetCellStyle(rowIndex, isHeader, out headerFormat);

            // Raise StartRowLayout event.
            BeginRowLayoutEventArgs srlArgs = RaiseBeforeRowLayout(rowIndex, cs);
            bool ignore = false;
            m_spanMap = null;

            rowHeight = 0;

            if (srlArgs != null)
            {
                stop = srlArgs.Cancel;
                ignore = srlArgs.Skip;
                m_spanMap = srlArgs.ColumnSpanMap;
                cs = srlArgs.CellStyle;

                ValidateSpanMap();
                rowHeight = Math.Max(srlArgs.MinimalHeight, rowHeight);
            }
            else
            {
                stop = false;
            }


            if (!stop)
            {
                float height = DetermineRowHeight(param, rowIndex, row, rowBouds, out results, cs);
                if (height > 0)
                {
                    rowHeight = Math.Max(height, rowHeight);
                }
                else
                {
                    rowHeight = height;
                }
                m_latestTextResults = results;
            }

            if (rowHeight <= 0 || stop)
            {
                bool incomplete = IsIncomplete(results);

                incomplete |= (m_currentPageBounds.Height - rowBouds.Y) <= 0;

                return incomplete;
            }

            rowBouds.Height = rowHeight;

            float value=(rowBouds.Y + rowBouds.Height);
            if (value > m_currentPageBounds.Height && m_currentPage != null)
            {
                return true;
            }
         
            bool isIncomplete = false;
            RectangleF bounds = rowBouds;
            PdfGraphics graphics = (m_currentPage != null) ? m_currentPage.Graphics : m_currentGraphics;

            int skipKnt = 0;

            if (!ignore)
            {
                for (int i = 0; i < count; ++i)
                {
                    bounds.Width = GetCellWidth(i);
                    bool spanned = (m_spanMap != null && m_spanMap[i] < 0);
                    string text = row[i];
                    bool skipCell = false;

                    if (!spanned)
                    {
                        bounds.X += m_cellSpacing;

                        BeginCellLayoutEventArgs bclArgs = RaiseBeforeCellLayout(graphics, rowIndex, i, bounds, text);

                        if (bclArgs != null)
                        {
                            skipCell = bclArgs.Skip;
                        }

                        if (skipCell)
                            skipKnt++;
                    }

                    PdfStringLayoutResult slr = results[i];

                    if (!ignore && !skipCell && !slr.Empty)
                    {
                        bool ignoreFormat = false;

                        if (srlArgs != null)
                        {
                            ignoreFormat = srlArgs.IgnoreColumnFormat;
                        }

                        if (isHeader && headerFormat)
                        {
                            ignoreFormat = true;
                        }

                        slr = DrawCell(slr, bounds, rowIndex, i, cs, ignoreFormat);
                    }

                    if (!spanned)
                    {
                        // Raise the event.
                        RaiseAfterCellLayout(graphics, rowIndex, i, bounds, text);
                    }

                    string remainder = slr.Remainder;

                    if (remainder != null && remainder != string.Empty)
                    {
                        isIncomplete = true;
                    }

                    row[i] = remainder;

                    if (!spanned)
                    {
                        bounds.X += bounds.Width;
                    }
                }
            }
            else
            {
                rowHeight = 0.0f;
            }

            if (skipKnt == count)
                rowHeight = 0.0f;

            if (!isIncomplete)
            {
                m_row = null;
                ++rowIndex;
            }
            else
            {
                m_row = row;
            }

            stop = RaiseAfterRowLayout(rowIndex, !isIncomplete, rowBouds);

            return isIncomplete;
        }

        /// <summary>
        /// Validates the span map.
        /// </summary>
        private void ValidateSpanMap()
        {
            if (m_spanMap != null)
            {
                int length = m_spanMap.Length;

                for (int i = 0; i < length; ++i)
                {
                    int count = m_spanMap[i];

                    if (count > 1)
                    {
                        int lastSpanned = count + i;

                        for (++i; i < lastSpanned && i < length; ++i)
                        {
                            m_spanMap[i] = -1;
                        }

                        --i;
                    }
                    else if (count < 0)
                    {
                        throw new PdfLightTableException("Invalid span map.");
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified row is incomplete.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <returns>
        /// 	<c>true</c> if the specified results is incomplete; otherwise, <c>false</c>.
        /// </returns>
        private bool IsIncomplete(PdfStringLayoutResult[] results)
        {
            bool incomplete = false;

            if (results != null)
            {
                foreach (PdfStringLayoutResult result in results)
                {
                    if (result.Remainder != null && result.Remainder != string.Empty)
                    {
                        incomplete = true;
                        break;
                    }
                }
            }
            else
            {
                // TODO: check this.
                incomplete = true;
            }

            return incomplete;
        }

        /// <summary>
        /// Determines the height of the row.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="row">The row.</param>
        /// <param name="rowBouds">The row bouds.</param>
        /// <param name="results">The results.</param>
        /// <param name="cs">The cell style.</param>
        /// <returns>The height of the row.</returns>
        private float DetermineRowHeight(PdfLayoutParams param, int rowIndex,
            string[] row, RectangleF rowBouds, out PdfStringLayoutResult[] results, PdfCellStyle cs)
        {
            int count = row.Length;
            float height = 0;

            if (m_currentPage != null)
            {
                height = Math.Min(m_currentPageBounds.Height - rowBouds.Y, rowBouds.Height);
            }

            SizeF size = new SizeF(m_cellWidths[0], height);
            float borderWidth = cs.BorderPen.Width;
            float cellPadding = Table.Style.CellPadding;
            bool overlapped = Table.Style.BorderOverlapStyle == PdfBorderOverlapStyle.Overlap;

            height = 0;

            size.Height = ApplyBordersToHeight(size.Height, borderWidth, overlapped);

            if (cellPadding > 0)
            {
                size.Height = ApplyBordersToHeight(size.Height, cellPadding, false);
            }

            results = new PdfStringLayoutResult[count];
            PdfColumnCollection colums = Table.Columns;

            for (int i = 0; i < count; i++)
            {
                PdfStringLayoutResult result = null;

                if (m_spanMap != null && m_spanMap[i] < 0)
                {
                    result = new PdfStringLayoutResult();
                    result.m_actualSize = SizeF.Empty;
                }
                else
                {
                    string text = row[i];

                    size.Width = GetCellWidth(i);

                    size.Width = ApplyBordersToHeight(size.Width, borderWidth, overlapped);

                    if (cellPadding > 0)
                    {
                        size.Width = ApplyBordersToHeight(size.Width, cellPadding, false);
                    }

                    if (text != null)
                    {
                        if (text.Equals(string.Empty))
                            text = " ";
                        if (m_previousRowIndex != rowIndex)
                        {
                            text = PdfGraphics.NormalizeText(cs.Font, text);
                        }
                    }
                    else
                    {
                        text = string.Empty;
                    }

                    PdfStringLayouter layouter = new PdfStringLayouter();
                    PdfStringFormat sf = colums[i].StringFormat;

                    if (sf == null)
                    {
                        sf = cs.StringFormat;
                    }

                    result = layouter.Layout(text, cs.Font, sf, size);

                    bool dropToNextPage = (param.Format != null &&
                        param.Format.Break == PdfLayoutBreakType.FitElement);

                    string remainder = result.Remainder;

                    if (Table.AllowRowBreakAcrossPages)
                    {
                        dropToNextPage &= (!string.IsNullOrEmpty(remainder));
                    }
                    else
                    {
                        dropToNextPage = (!string.IsNullOrEmpty(remainder));
                    }

                    if (dropToNextPage && m_dropIndex != rowIndex)
                    {
                        DropToNextPage(results, count, row);
                        m_dropIndex = rowIndex;
                        height = 0;
                        break;
                    }
                    else if (size.Height > 0 || m_currentPage == null)
                    {
                        height = Math.Max(result.ActualSize.Height, height);
                    }
                    else
                    {
                        result = new PdfStringLayoutResult();
                        result.m_remainder = text;
                        result.m_actualSize = SizeF.Empty;
                    }
                }
                results[i] = result;
            }
            m_previousRowIndex = rowIndex;
            if (height > 0)
            {
                if (m_currentPage != null)
                {
                    height = Math.Min(rowBouds.Height, height);
                }

                if (cellPadding > 0)
                {
                    height = ApplyBordersToHeight(height, -cellPadding, false);
                }

                height = ApplyBordersToHeight(height, -borderWidth, overlapped);
            }

            return height;
        }

        /// <summary>
        /// Drops lay outing to the next page.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="count">The count.</param>
        /// <param name="row">The row.</param>
        private void DropToNextPage(PdfStringLayoutResult[] results, int count, string[] row)
        {
            for (int i = 0; i < count; ++i)
            {
                PdfStringLayoutResult result = new PdfStringLayoutResult();

                result.m_remainder = row[i];
                result.m_actualSize = SizeF.Empty;

                results[i] = result;
            }
        }

        /// <summary>
        /// Returns the width of the cell specified by the index.
        /// </summary>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <returns>The width of the cell.</returns>
        private float GetCellWidth(int cellIndex)
        {
            float width = m_cellWidths[cellIndex];

            if (m_spanMap != null && m_spanMap.Length == m_cellWidths.Length)
            {
                int count = m_spanMap[cellIndex];

                if (count > 1)
                {
                    int length = m_spanMap.Length;
                    int lastSpanned = count + cellIndex;
                    float cellSpacing = Table.Style.CellSpacing;

                    for (int i = cellIndex + 1; i < lastSpanned && i < length; i++)
                    {
                        width += m_cellWidths[i] + cellSpacing;
                        m_spanMap[i] = -1;
                    }
                }
            }

            return width;
        }

        /// <summary>
        /// Reduces the height (or width) according to overlapped and border width value.
        /// </summary>
        /// <param name="height">The height.</param>
        /// <param name="borderWidth">Width of the border.</param>
        /// <param name="overlapped">if it is overlapped, set to <c>true</c>.</param>
        /// <returns>The proper height.</returns>
        private static float ApplyBordersToHeight(float height, float borderWidth, bool overlapped)
        {
            if (overlapped)
            {
                height -= borderWidth;
            }
            else
            {
                height -= borderWidth * 2;
            }

            if (height < 0)
            {
                height = 0;
            }

            return height;
        }

        /// <summary>
        /// Draws a cell.
        /// </summary>
        /// <param name="layoutResult">The layout result.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <param name="cs">The cell style.</param>
        /// <returns>The result of the drawing.</returns>
        private PdfStringLayoutResult DrawCell(PdfStringLayoutResult layoutResult, RectangleF bounds, int rowIndex,
            int cellIndex, PdfCellStyle cs, bool ignoreColumnFormat)
        {
            PdfGraphics gr = (m_currentPage != null) ? m_currentPage.Graphics : m_currentGraphics;
            bool overlapped = Table.Style.BorderOverlapStyle == PdfBorderOverlapStyle.Overlap;
            float cellPadding = Table.Style.CellPadding;
            PdfPen pen = cs.BorderPen;
            PdfBrush brush = cs.BackgroundBrush;

            // Skip cells that are spanned.
            if (m_spanMap != null && m_spanMap[cellIndex] == -1)
            {
                PdfStringLayoutResult lr = new PdfStringLayoutResult();

                return lr;
            }

            // Draw border.
            if (!overlapped)
            {
                bounds = PreserveForBorder(bounds, pen, PdfBorderOverlapStyle.Overlap);
            }

            // Draw background.
            if (brush != null)
            {
                float alpha = GetAlpha(brush);

                gr.Save();
                gr.SetTransparency(alpha);
                gr.DrawRectangle(null, brush, bounds);
                gr.Restore();
            }

            if (pen != null)
            {
                float alpha = (float)pen.Color.A / 255.0f;

                gr.Save();
                gr.SetTransparency(alpha);
                gr.DrawRectangle(pen, null, bounds);
                gr.Restore();
            }

            bounds = PreserveForBorder(bounds, pen, PdfBorderOverlapStyle.Overlap);

            if (cellPadding > 0)
            {
                bounds.X += cellPadding;
                bounds.Y += cellPadding;
                bounds.Width -= cellPadding * 2;
                bounds.Height -= cellPadding * 2;
            }

            if (!layoutResult.Empty)
            {
                PdfColumn column = Table.Columns[cellIndex];

                PdfStringFormat format = ignoreColumnFormat ? cs.StringFormat : column.StringFormat;

                if (format == null)
                {
                    format = cs.StringFormat;
                }

                // Layout text.
                RectangleF layoutRectangle = bounds;
                RectangleF rect = gr.CheckCorrectLayoutRectangle(layoutResult.ActualSize,
                    layoutRectangle.X, layoutRectangle.Y, format);

                if (layoutRectangle.Width <= 0)
                {
                    layoutRectangle.X = rect.X;
                    layoutRectangle.Width = rect.Width;
                }
                if (layoutRectangle.Height <= 0)
                {
                    layoutRectangle.Y = rect.Y;
                    layoutRectangle.Height = rect.Height;
                }
                //for (int i = 0; i < layoutResult.Lines.Length;i++ )
                //{
                //    layoutResult.Lines[i].Text = PdfStandardFont.Convert(layoutResult.Lines[i].Text);

                //}
                

                // Draw text.
                gr.DrawStringLayoutResult(layoutResult, cs.Font, cs.TextPen, cs.TextBrush,
                    layoutRectangle, format);
            }

            //gr.GetResources().RequireProcSet( ProcedureSets.Text );

            return layoutResult;
        }

        /// <summary>
        /// Gets the cell style.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="isHeader">if set to <c>true</c> the cell is in header.</param>
        /// <returns>The cell style.</returns>
        private PdfCellStyle GetCellStyle(int rowIndex, bool isHeader, out bool hasOwnStyle)
        {
            PdfCellStyle cs;
            PdfLightTableStyle tableProps = Table.Style;
            hasOwnStyle = false;

            if (isHeader)
            {
                cs = tableProps.HeaderStyle;
                hasOwnStyle = true;
            }
            else if ((rowIndex & 1) > 0)
            {
                cs = tableProps.AlternateStyle;
            }
            else
            {
                cs = tableProps.DefaultStyle;
            }

            if (cs == null)
            {
                cs = tableProps.DefaultStyle;
                hasOwnStyle = false;
            }

            return cs;
        }

        /// <summary>
        /// Gets the widths.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <returns>An array containing the widhts.</returns>
        private float[] GetWidths(RectangleF bounds)
        {
            int count = m_endColumn - m_startColumn + 1;
            PdfLightTableStyle style = Table.Style;
            PdfPen borderPen = style.BorderPen;
            float width = (borderPen == null) ? 0 : borderPen.Width;

            if (style.BorderOverlapStyle == PdfBorderOverlapStyle.Inside)
            {
                width *= 2f;
            }

            float totalWidth = bounds.Width - (style.CellSpacing * (count + 1)) - width;
            float[] widths = Table.Columns.GetWidths(totalWidth, m_startColumn, m_endColumn);

            return widths;
        }

        /// <summary>
        /// Retrieves the next row.
        /// </summary>
        /// <param name="startRowIndex">Start index of the row.</param>
        /// <param name="param">The param.</param>
        /// <returns>The next row.</returns>
        private string[] GetRow(int startRowIndex, PdfLayoutParams param)
        {
            string[] row;

            if (m_row != null)
            {
                row = m_row;
            }
            else
            {
                row = Table.GetNextRow(ref startRowIndex);
                row = CropRow(row);
            }

            return row;
        }

        /// <summary>
        /// Gets the alpha channel value.
        /// </summary>
        /// <param name="brush">The brush.</param>
        /// <returns>The alpha channel value.</returns>
        private float GetAlpha(PdfBrush brush)
        {
            PdfSolidBrush sBrush = brush as PdfSolidBrush;
            PdfLinearGradientBrush lBrush = brush as PdfLinearGradientBrush;

            float alpha = 1.0f;

            if (sBrush != null)
            {
                alpha = (float)(sBrush.Color.A) / 255;
            }

            else if (lBrush != null)
            {

                PdfColor color1 = new PdfColor(0, 0, 0);
                PdfColor color2 = new PdfColor(0, 0, 0);

                PdfColor[] linearColors = lBrush.LinearColors;
                PdfColorBlend ncBlend = null;

                if (linearColors != null)
                {
                    color1 = linearColors[0];
                    color2 = linearColors[1];
                }

                if ((color1.IsEmpty && color2.IsEmpty) || (color1.A == 0 && color2.A == 0))
                {
                    ncBlend = lBrush.InterpolationColors;
                    color1 = ncBlend.Colors[0];
                }

                alpha = (float)(color1.A) / 255;

            }

            return alpha;
        }

        /// <summary>
        /// Raises BeforePageLayout event.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="currentBounds">The current bounds.</param>
        /// <param name="currentRow">The current row.</param>
        /// <returns>If true, stop lay outing.</returns>
        private bool RaiseBeforePageLayout(PdfPage currentPage, ref RectangleF currentBounds, ref int currentRow)
        {
            bool cancel = false;

            if (Element.RaiseBeginPageLayout)
            {
                LightTableBeginPageLayoutEventArgs args =
                    new LightTableBeginPageLayoutEventArgs(currentBounds, currentPage, currentRow);

                Element.OnBeginPageLayout(args);

                cancel = args.Cancel;
                currentBounds = args.Bounds;
                currentRow = args.StartRowIndex;
            }

            return cancel;
        }

        /// <summary>
        /// Raises PageLayout event if needed.
        /// </summary>
        /// <param name="pageResult">Page layout result.</param>
        /// <returns>Event arguments.</returns>
        private LightTableEndPageLayoutEventArgs RaisePageLayouted(PageLayoutResult pageResult)
        {
            LightTableEndPageLayoutEventArgs args = null;

            if (Element.RaiseEndPageLayout)
            {
                PdfLightTableLayoutResult res = GetLayoutResult(pageResult);
                int lastRowIndex = pageResult.LastRowIndex;

                if (m_row == null)
                {
                    --lastRowIndex;
                }

                args = new LightTableEndPageLayoutEventArgs(res, pageResult.FirstRowIndex, lastRowIndex);

                Element.OnEndPageLayout(args);
            }

            return args;
        }

        /// <summary>
        /// Raises the before row layout.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellStyle">The cell style.</param>
        /// <returns>The arguments modified by the user.</returns>
        private BeginRowLayoutEventArgs RaiseBeforeRowLayout(int rowIndex, PdfCellStyle cellStyle)
        {
            BeginRowLayoutEventArgs args = null;

            if (Table.RaiseBeginRowLayout)
            {
                args = new BeginRowLayoutEventArgs(rowIndex, cellStyle);

                Table.OnBeginRowLayout(args);
            }

            return args;
        }

        /// <summary>
        /// Raises the after row layout.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="isComplete">if set to <c>true</c> the row was drawn completely.</param>
        /// <param name="rowBouds">The row bouds.</param>
        /// <returns>Indicator whether the user stopped lay outing.</returns>
        private bool RaiseAfterRowLayout(int rowIndex, bool isComplete, RectangleF rowBouds)
        {
            bool stop = false;

            if (Table.RaiseEndRowLayout)
            {
                EndRowLayoutEventArgs args = new EndRowLayoutEventArgs(rowIndex, isComplete, rowBouds);

                Table.OnEndRowLayout(args);
                stop = args.Cancel;
            }

            return stop;
        }

        /// <summary>
        /// Raises the before cell layout.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="value">The cell value.</param>
        private BeginCellLayoutEventArgs RaiseBeforeCellLayout(PdfGraphics graphics, int rowIndex, int cellIndex,
            RectangleF bounds, string value)
        {
            BeginCellLayoutEventArgs args = null;

            if (Table.RaiseBeginCellLayout)
            {
                args = new BeginCellLayoutEventArgs(graphics, rowIndex, cellIndex, bounds, value);
                Table.OnBeginCellLayout(args);
            }

            return args;
        }

        /// <summary>
        /// Raises the after cell layout event.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellIndex">Index of the cell.</param>
        /// <param name="bounds">The bounds of the cell.</param>
        /// <param name="value">The value of the cell.</param>
        private void RaiseAfterCellLayout(PdfGraphics graphics, int rowIndex, int cellIndex,
            RectangleF bounds, string value)
        {
            if (Table.RaiseEndCellLayout)
            {
                EndCellLayoutEventArgs args = new EndCellLayoutEventArgs(graphics, rowIndex, cellIndex,
                    bounds, value);

                Table.OnEndCellLayout(args);
            }
        }
        #endregion

        #region Internals
        private class PageLayoutResult
        {
            #region Fields
            /// <summary>
            /// The last page where the text was drawn.
            /// </summary>
            public PdfPage Page;
            /// <summary>
            /// The bounds of the element on the last page where it was drawn.
            /// </summary>
            public RectangleF Bounds;
            /// <summary>
            /// Indicates whether the lay outing has been finished.
            /// </summary>
            public bool Finish;
            /// <summary>
            /// The index of the first row on the page.
            /// </summary>
            public int FirstRowIndex;
            /// <summary>
            /// The index of the last row on the page.
            /// </summary>
            public int LastRowIndex;
            #endregion
        }
        #endregion
    }

    /// <summary>
    /// Represents the results of lay outing tables.
    /// </summary>
    /// <example>
    /// <code lang="C#">
    ///  // Creates a new document
    ///  PdfDocument doc = new PdfDocument();
    ///  //Creates a new page and adds it as the last page of the document
    ///  PdfPage page = doc.Pages.Add();
    ///  // Creates a new table
    ///  PdfLightTable table = new PdfLightTable();
    ///  // Set the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.TableDirect;
    ///  // Creating Columns
    ///  table.Columns.Add(new PdfColumn("Roll Number"));
    ///  table.Columns.Add(new PdfColumn("Name"));
    ///  table.Columns.Add(new PdfColumn("Class"));          
    ///  // Adding rows
    ///  PdfRowCollection rowCollection = table.Rows;
    ///  // Gets the first row from the collection.          
    ///  rowCollection.Add(new object[] { "111", "Maxim", "III" });
    ///  // Creates the layout format
    ///  PdfLightTableLayoutFormat format = new PdfLightTableLayoutFormat();
    ///  format.EndColumnIndex = 2;
    ///  format.StartColumnIndex = 1;         
    ///  format.Layout = PdfLayoutType.Paginate;
    ///  format.Break = PdfLayoutBreakType.FitElement;                       
    ///  // Draws the table 
    ///  PdfLightTableLayoutResult result = table.Draw(page, new PointF(0, 0), format);
    ///  // Returns the drawn table boundary value
    ///  Console.WriteLine(result.Bounds.ToString());
    ///  doc.Save("Tables.pdf");
    /// </code>
    /// <code lang="VB">
    ///  ' Creates a new document
    ///  Dim doc As PdfDocument = New PdfDocument()
    ///  ' Create a page
    ///  Dim page As PdfPage = doc.Pages.Add()
    ///  ' Creates a new table
    ///  Dim table As PdfLightTable = New PdfLightTable()
    ///  ' Set the DataSourceType as Direct
    ///  table.DataSourceType = PdfLightTableDataSourceType.TableDirect
    ///  ' Creating Columns
    ///  table.Columns.Add(New PdfColumn("Roll Number"))
    ///  table.Columns.Add(New PdfColumn("Name"))
    ///  table.Columns.Add(New PdfColumn("Class"))
    ///  ' Adding rows
    ///  Dim rowCollection As PdfRowCollection = table.Rows
    ///  ' Gets the first row from the collection.          
    ///  rowCollection.Add(New Object() { "111", "Maxim", "III" })
    ///  ' Creates the layout format
    ///  Dim format As PdfLightTableLayoutFormat = New PdfLightTableLayoutFormat()
    ///  format.EndColumnIndex = 2
    ///  format.StartColumnIndex = 1
    ///  format.Layout = PdfLayoutType.Paginate
    ///  format.Break = PdfLayoutBreakType.FitElement
    ///  ' Draws the table 
    ///  Dim result As PdfLightTableLayoutResult = table.Draw(page, New PointF(0, 0), format)
    ///  ' Returns the drawn table boundary value
    ///  Console.WriteLine(result.Bounds.ToString())
    ///  doc.Save("Tables.pdf")
    /// </code>
    /// </example>
    /// <seealso cref="PdfLayoutResult"/> Class 
    public class PdfLightTableLayoutResult : PdfLayoutResult
    {
        #region Fields
        /// <summary>
        /// Holds text layout results for the last row.
        /// </summary>
        private PdfStringLayoutResult[] m_cellResults;
        /// <summary>
        /// The index of the last row.
        /// </summary>
        private int m_rowIndex;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="T:PdfTextLayoutResult"/> at the specified index.
        /// </summary>
        internal PdfStringLayoutResult[] CellResults
        {
            get
            {
                return m_cellResults;
            }
        }

        /// <summary>
        /// Gets the index of the last row.
        /// </summary>
        /// <example>
        /// <code lang="C#">
        /// // Draws the table 
        /// PdfLightTableLayoutResult result = table.Draw(page, new PointF(0, 0), format);
        /// // Gets the last row index
        /// int lastRow = result.LastRowIndex;
        /// </code>
        /// <code lang="VB">
        /// ' Draws the table 
        /// Dim result As PdfLightTableLayoutResult = table.Draw(page, New PointF(0, 0), format)
        /// ' Gets the last row index
        /// Dim lastRow As Integer = result.LastRowIndex
        /// </code>
        /// </example>
        public int LastRowIndex
        {
            get
            {
                return m_rowIndex;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:PdfLightTableLayoutResult"/> class.
        /// </summary>
        /// <param name="page">The current page.</param>
        /// <param name="bounds">The current bounds.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="cellResults">The cell results.</param>
        internal PdfLightTableLayoutResult(PdfPage page, RectangleF bounds,
            int rowIndex, PdfStringLayoutResult[] cellResults)
            : base(page, bounds)
        {
            m_rowIndex = rowIndex;
            m_cellResults = cellResults;
        }
        #endregion
    }

    /// <summary>
    /// Arguments of BeginPageLayoutEvent.
    /// </summary>
    /// <seealso cref="BeginPageLayoutEventArgs"/> Class    
    public class LightTableBeginPageLayoutEventArgs : BeginPageLayoutEventArgs
    {
        #region Fields
        private int m_startRow;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start row.
        /// </summary>
        /// <value>The start row.</value>
        public int StartRowIndex
        {
            get
            {
                return m_startRow;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LightTablesBeginPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        /// <param name="page">The page.</param>
        /// <param name="startRow">The start row.</param>
        internal LightTableBeginPageLayoutEventArgs(RectangleF bounds, PdfPage page, int startRow)
            : base(bounds, page)
        {
            m_startRow = startRow;
        }

        #endregion
    }

    /// <summary>
    /// Holds arguments for LightTableEndPageLayout Event.
    /// </summary>
    /// <seealso cref="EndPageLayoutEventArgs"/> Class    
    public class LightTableEndPageLayoutEventArgs : EndPageLayoutEventArgs
    {
        #region Fields
        private int m_startRow;
        private int m_endRow;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the start row.
        /// </summary>
        /// <value>The start row.</value>
        public int StartRowIndex
        {
            get
            {
                return m_startRow;
            }
        }

        /// <summary>
        /// Gets the end row.
        /// </summary>
        /// <value>The end row.</value>
        public int EndRowIndex
        {
            get
            {
                return m_endRow;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LightTableEndPageLayoutEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="startRow">The start row.</param>
        /// <param name="endRow">The end row.</param>
        internal LightTableEndPageLayoutEventArgs(PdfLightTableLayoutResult result,
            int startRow, int endRow)
            : base(result)
        {
            m_startRow = startRow;
            m_endRow = endRow;
        }
        #endregion
    }
}
