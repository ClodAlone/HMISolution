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
using Syncfusion.Pdf.Graphics;
using System.Drawing;
using Syncfusion.Pdf.Lists;

namespace Syncfusion.Pdf.Grid
{
    internal class PdfGridLayouter : ElementLayouter
    {
        #region Fields
        private PdfGraphics m_currentGraphics;
        private PdfPage m_currentPage;
        private SizeF m_currentPageBounds;
        private RectangleF m_currentBounds;
        private List<int[]> m_columnRanges = new List<int[]>();
        private int m_cellStartIndex;
        private int m_cellEndIndex;
        private int m_currentRowIndex;
        private PointF m_startLocation;
        private float m_newheight;
        private int m_repeatRowIndex = -1;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the grid.
        /// </summary>
        /// <value>The grid.</value>
        internal PdfGrid Grid
        {
            get
            {
                return (base.Element as PdfGrid);
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LightTableLayouter"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        internal PdfGridLayouter(PdfGrid grid)
            : base(grid)
        {

        }
        #endregion

        #region Implementation
        /// <summary>
        /// Layouts the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="location">The location.</param>
        public void Layout(PdfGraphics graphics, PointF location)
        {
            RectangleF boundaries = new RectangleF(location, SizeF.Empty);
            Layout(graphics, boundaries);
        }

        /// <summary>
        /// Layouts the specified graphics.
        /// </summary>
        /// <param name="graphics">The graphics.</param>
        /// <param name="bounds">The bounds.</param>
        public void Layout(PdfGraphics graphics, RectangleF bounds)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            // Determine the width.
            float width = graphics.ClientSize.Width - bounds.X;
            

            PdfLayoutParams param = new PdfLayoutParams();
            param.Bounds = bounds;
            m_currentGraphics = graphics;
            LayoutInternal(param);
        }

        /// <summary>
        /// Layouts the element.
        /// </summary>
        /// <param name="param">Lay outing parameters.</param>
        /// <returns>Lay outing result.</returns>
        protected override PdfLayoutResult LayoutInternal(PdfLayoutParams param)
        {
            if (param == null)
                throw new ArgumentNullException("param");

            PdfGridLayoutFormat format = GetFormat(param.Format);

            m_currentPage = param.Page;

            m_currentPageBounds = (m_currentPage != null) ?
                m_currentPage.GetClientSize() :
                m_currentGraphics.ClientSize;

            m_currentGraphics = (m_currentPage != null) ? m_currentPage.Graphics : m_currentGraphics;

            m_currentBounds = new RectangleF(param.Bounds.Location, m_currentGraphics.ClientSize);
            m_currentBounds.Width = (param.Bounds.Width > 0) ? param.Bounds.Width : m_currentBounds.Width;
            m_startLocation = param.Bounds.Location;

            if (!Grid.Style.AllowHorizontalOverflow)
            {
                Grid.MeasureColumnsWidth(m_currentBounds);
                m_columnRanges.Add(new int[] { 0, Grid.Columns.Count - 1 });
            }
            else
            {
                Grid.MeasureColumnsWidth();
                DetermineColumnDrawRanges();
            }

            PdfGridLayoutResult result = LayoutOnPage(param);

            return result;
        }

        /// <summary>
        /// Layouts the on page.
        /// </summary>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        private PdfGridLayoutResult LayoutOnPage(PdfLayoutParams param)
        {
            PdfGridLayoutFormat format = GetFormat(param.Format);
            PdfGridEndPageLayoutEventArgs endArgs = null;
            PdfGridLayoutResult result = null;
            Dictionary<PdfPage, int[]> layoutedPages = new Dictionary<PdfPage, int[]>();
            PdfPage startPage = param.Page;

            foreach (int[] range in m_columnRanges)
            {
                m_cellStartIndex = range[0];
                m_cellEndIndex = range[1];

                if (RaiseBeforePageLayout(m_currentPage, ref m_currentBounds, ref m_currentRowIndex))
                {
                    result = new PdfGridLayoutResult(m_currentPage, m_currentBounds);
                    break;
                }

                //Draw Headers.
                bool drawHeader;

                foreach (PdfGridRow row in Grid.Headers)
                {
                    float headerHeight = m_currentBounds.Y;

                    RowLayoutResult headerResult = DrawRow(row);

                    if (headerHeight == m_currentBounds.Y)
                    {
                        drawHeader = true;
                        m_repeatRowIndex = Grid.Rows.IndexOf(row);
                    }
                    else
                    {
                        drawHeader = false;
                    }
                    if (!headerResult.IsFinish && startPage != null
                        && format.Layout != PdfLayoutType.OnePage && drawHeader)
                    {
                        m_startLocation.X = m_currentBounds.X;
                        m_currentPage = GetNextPage(format);

                        m_startLocation.Y = m_currentBounds.Y;
                        if (format.PaginateBounds == RectangleF.Empty)
                            m_currentBounds.X += m_startLocation.X;

                        DrawRow(row);
                    }
                }

                int i = 0; int length = Grid.Rows.Count;
                bool repeatRow;

                //Draw row by row with the specified cell range.
                foreach (PdfGridRow row in Grid.Rows)
                {
                    i++;
                    float originalHeight = m_currentBounds.Y;
                    if (m_currentPage != null && !layoutedPages.ContainsKey(m_currentPage))
                        layoutedPages.Add(m_currentPage, range);

                    RowLayoutResult rowResult = DrawRow(row);

                    //if height remains same, it is understood that row is not drawn in the page
                    if (originalHeight == m_currentBounds.Y)
                    {
                        repeatRow = true;
                        m_repeatRowIndex = Grid.Rows.IndexOf(row);
                    }
                    else
                    {
                        repeatRow = false;
                        m_repeatRowIndex = -1;
                    }

                    while (!rowResult.IsFinish && startPage != null)
                    {
                        PdfGridLayoutResult tempResult = GetLayoutResult();

                        if (startPage != m_currentPage)
                        {
                            if (row.Grid.IsChildGrid && row.Grid.ParentCell != null)
                            {
                                RectangleF bounds = new RectangleF(format.PaginateBounds.Location, new SizeF(param.Bounds.Width, tempResult.Bounds.Height));
                                bounds.X += param.Bounds.X;

                                // Draw border for cells in the nested grid cell's row.
                                for (int c = 0; c < row.Cells.Count; c++)
                                {
                                    PdfGridCell cell = row.Cells[c];
                                    float cellWidth = 0.0f;
                                    if (cell.ColumnSpan > 1)
                                    {
                                        for (; c < cell.ColumnSpan; c++)
                                            cellWidth += row.Grid.Columns[c].Width;
                                    }
                                    else
                                        cellWidth = Math.Max(cell.Width, row.Grid.Columns[c].Width);
                                    cell.DrawCellBorders(ref m_currentGraphics, new RectangleF(bounds.Location, new SizeF(cellWidth, bounds.Height)));
                                    bounds.X += cellWidth;
                                    c += (cell.ColumnSpan - 1);
                                }
                            }
                        }

                        endArgs = RaisePageLayouted(tempResult);
                        if (endArgs.Cancel || repeatRow)
                            break;
                        else if (Grid.AllowRowBreakAcrossPages)
                        {
                            //If there is no space in the current page, add new page and then draw the remaining row.
                            m_currentPage = GetNextPage(format);
                            originalHeight = m_currentBounds.Y;
                            rowResult = DrawRow(row);
                        }
                        else if (!Grid.AllowRowBreakAcrossPages && i < length)
                        {
                            m_currentPage = GetNextPage(format);
                            break;
                        }
                        else if (i >= length)
                            break;
                    }

                    if (!rowResult.IsFinish && startPage != null
                        && format.Layout != PdfLayoutType.OnePage && repeatRow)
                    {
                        // During pagination, cell position is maintained here.
                        m_startLocation.X = m_currentBounds.X;
                        m_currentPage = GetNextPage(format);

                        if (RaiseBeforePageLayout(m_currentPage, ref m_currentBounds, ref m_currentRowIndex))
                            break;

                        m_startLocation.Y = m_currentBounds.Y;
                        if (format.PaginateBounds == RectangleF.Empty)
                            m_currentBounds.X += m_startLocation.X;

                        if (Grid.RepeatHeader)
                        {
                            foreach (PdfGridRow header in Grid.Headers)
                            {
                                DrawRow(header);
                            }
                        }
                        DrawRow(row);
                    }

                    if (row.NestedGridLayoutResult != null)
                    {
                        // Position for next row in the grid.
                        m_currentPage = row.NestedGridLayoutResult.Page;
                        m_currentGraphics = m_currentPage.Graphics; //If not, next row will not be drawn in the layouted page.
                        m_startLocation = row.NestedGridLayoutResult.Bounds.Location;
                        m_currentBounds.Y = row.NestedGridLayoutResult.Bounds.Bottom;

                        if (startPage != m_currentPage)
                        {
                            PdfSection secion = m_currentPage.Section;
                            int startIndex = secion.IndexOf(startPage) + 1;
                            int endIndex = secion.IndexOf(m_currentPage);

                            for (int page = startIndex; page < endIndex + 1; page++)
                            {
                                PdfGraphics pageGraphics = secion[page].Graphics;
                                PointF location = format.PaginateBounds.Location;

                                float height = page == endIndex ? row.NestedGridLayoutResult.Bounds.Height : (m_currentBounds.Height - location.Y);
                                if (row.Grid.IsChildGrid && row.Grid.ParentCell != null)
                                    location.X += param.Bounds.X;
                                // Draw border for last paginated row containing nested grid.
                                for (int c = 0; c < row.Cells.Count; c++)
                                {
                                    PdfGridCell cell = row.Cells[c];
                                    float cellWidth = 0.0f;
                                    if (cell.ColumnSpan > 1)
                                    {
                                        for (; c < cell.ColumnSpan; c++)
                                            cellWidth += row.Grid.Columns[c].Width;
                                    }
                                    else
                                        cellWidth = Math.Max(cell.Width, row.Grid.Columns[c].Width);

                                    cell.DrawCellBorders(ref pageGraphics, new RectangleF(location, new SizeF(cellWidth, height)));
                                    location.X += cellWidth;
                                    c += (cell.ColumnSpan - 1);
                                }
                            }

                            // So, nested grid drawing is completed for the current row. Update page.
                            // Otherwise, the next nested grid of the parent will draw borders from start.
                            startPage = m_currentPage;
                        }
                    }
                }

                if (m_columnRanges.IndexOf(range) < m_columnRanges.Count - 1
                    && startPage != null && format.Layout != PdfLayoutType.OnePage)
                {
                    m_currentPage = GetNextPage(format);
                }
            }

            result = GetLayoutResult();

            if (Grid.Style.AllowHorizontalOverflow 
                && Grid.Style.HorizontalOverflowType == PdfHorizontalOverflowType.NextPage)
            {
                ReArrangePages(layoutedPages);
            }

            //Raise End Event.
            RaisePageLayouted(result);

            return result;
        }

        /// <summary>
        /// Rearranges the pages.
        /// </summary>
        /// <param name="layoutedPages">The layouted pages.</param>
        private void ReArrangePages(Dictionary<PdfPage, int[]> layoutedPages)
        {
            PdfDocument document = m_currentPage.Document;
            List<PdfPage> pages = new List<PdfPage>();

            foreach (PdfPage page in layoutedPages.Keys)
            {
                page.Section = null;
                pages.Add(page);
                document.Pages.Remove(page);
            }

            for (int i = 0; i < layoutedPages.Count; i++)
            {
                for (int j = i, Count = layoutedPages.Count / m_columnRanges.Count; j < layoutedPages.Count; j += Count)
                {
                    PdfPage page = pages[j];
                    if (document.Pages.IndexOf(page) == -1)
                        document.Pages.Add(page);
                }
            }

        }

        /// <summary>
        /// Draws the row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="cellStartIndex">Start index of the cell.</param>
        /// <param name="cellEndIndex">End index of the cell.</param>
        private RowLayoutResult DrawRow(PdfGridRow row)
        {
            //.. Check if required space available.
            //.....If the row conains spans which  falls through more than one page, then draw the row to next page.

            RowLayoutResult result = new RowLayoutResult();

            float rowHeightWithSpan = 0;
            PointF location = PointF.Empty;
            SizeF size = SizeF.Empty;
            bool isHeader = false;

            if (row.RowSpanExists)
            {
                int maxSpan = 0;
                int currRowIndex = Grid.Rows.IndexOf(row);

                if (currRowIndex == -1)
                {
                    currRowIndex = Grid.Headers.IndexOf(row);
                    if (currRowIndex != -1)
                        isHeader = true;
                }

                foreach (PdfGridCell cell in row.Cells)
                {
                    maxSpan = Math.Max(maxSpan, cell.RowSpan);
                }

                for (int i = currRowIndex; i < currRowIndex + maxSpan; i++)
                {
                    rowHeightWithSpan += (isHeader ? Grid.Headers[i].Height : Grid.Rows[i].Height);
                }

                if (rowHeightWithSpan > m_currentBounds.Height)
                {
                    rowHeightWithSpan = 0;
                    foreach (PdfGridCell cell in row.Cells)
                    {
                        maxSpan = cell.RowSpan;

                        for (int i = currRowIndex; i < currRowIndex + maxSpan; i++)
                        {
                            rowHeightWithSpan += (isHeader ? Grid.Headers[i].Height : Grid.Rows[i].Height);
                            if ((m_currentBounds.Y + rowHeightWithSpan) > m_currentPageBounds.Height)
                            {
                                rowHeightWithSpan -= (isHeader ? Grid.Headers[i].Height : Grid.Rows[i].Height);
                                for (int j = 0; j < Grid.Rows[currRowIndex].Cells.Count; j++)
                                {
                                    //assigning new span values
                                    int newSpan = (i - currRowIndex);
                                    if (!isHeader && (Grid.Rows[currRowIndex].Cells[j].RowSpan == maxSpan))
                                    {
                                        Grid.Rows[currRowIndex].Cells[j].RowSpan = newSpan == 0 ? 1 : newSpan;
                                        Grid.Rows[i].Cells[j].RowSpan = (maxSpan - newSpan);

                                        //cell attributes
                                        Grid.Rows[i].Cells[j].StringFormat = Grid.Rows[currRowIndex].Cells[j].StringFormat;
                                        Grid.Rows[i].Cells[j].Style = Grid.Rows[currRowIndex].Cells[j].Style;
                                        Grid.Rows[i].Cells[j].ColumnSpan = Grid.Rows[currRowIndex].Cells[j].ColumnSpan;
                                        Grid.Rows[i].Cells[j].Value = Grid.Rows[currRowIndex].Cells[j].Value;
                                        Grid.Rows[i - 1].RowSpanExists = false;
                                        Grid.Rows[i].Cells[j].IsRowMergeContinue = false;
                                        Grid.Rows[i].Cells[j].IsRowMergeStart = true;
                                    }
                                    else if(isHeader && (Grid.Headers[currRowIndex].Cells[j].RowSpan == maxSpan))
                                    {
                                        Grid.Headers[currRowIndex].Cells[j].RowSpan = newSpan == 0 ? 1 : newSpan;
                                        Grid.Headers[i].Cells[j].RowSpan = (maxSpan - newSpan);

                                        //cell attributes
                                        Grid.Headers[i].Cells[j].StringFormat = Grid.Headers[currRowIndex].Cells[j].StringFormat;
                                        Grid.Headers[i].Cells[j].Style = Grid.Headers[currRowIndex].Cells[j].Style;
                                        Grid.Headers[i].Cells[j].ColumnSpan = Grid.Headers[currRowIndex].Cells[j].ColumnSpan;
                                        Grid.Headers[i].Cells[j].Value = Grid.Headers[currRowIndex].Cells[j].Value;
                                        Grid.Headers[i - 1].RowSpanExists = false;
                                        Grid.Headers[i].Cells[j].IsRowMergeContinue = false;
                                        Grid.Headers[i].Cells[j].IsRowMergeStart = true;
                                    }
                                }
                                break;
                            }
                        }
                        rowHeightWithSpan = 0;
                    }

                }
            }
            float height = row.RowBreakHeight > 0.0f ? row.RowBreakHeight : row.Height;

            //Split row only if row height exceeds page height and AllowRowBreakAcrossPages is true.
            if (height > m_currentPageBounds.Height)
            {
                if (Grid.AllowRowBreakAcrossPages)
                {
                    result.IsFinish = true;
                    DrawRowWithBreak(ref result, row, height);
                }
                else
                {
                    //If AllowRowBreakAcrossPages is not true, draw the row till it fits the page.
                    result.IsFinish = false;
                    DrawRow(ref result, row, height);
                }
            }
            else if (m_currentBounds.Y + height > m_currentPageBounds.Height || m_currentBounds.Y + rowHeightWithSpan > m_currentPageBounds.Height)
            {
                // If a row is repeated and still cannot fit in page, proceed draw.
                if (m_repeatRowIndex > -1 && m_repeatRowIndex == row.RowIndex)
                {
                    if (Grid.AllowRowBreakAcrossPages)
                    {
                        result.IsFinish = true;
                        DrawRowWithBreak(ref result, row, height);
                    }
                    else
                    {
                        result.IsFinish = false;
                        DrawRow(ref result, row, height);
                    }
                }
                else
                    result.IsFinish = false;
            }
            else
            {
                result.IsFinish = true;
                DrawRow(ref result, row, height);
            }

            return result;
        }

        /// <summary>
        /// Draws row till it fits the page and then calculates height for the next page.
        /// </summary>
        /// <param name="result">The RowLayoutResult.</param>
        /// <param name="row">Row being drawn.</param>
        /// <param name="height">Height of the row.</param>
        private void DrawRowWithBreak(ref RowLayoutResult result, PdfGridRow row, float height)
        {
            PointF location = m_currentBounds.Location;
            result.Bounds = new RectangleF(location, SizeF.Empty);

            m_newheight = row.RowBreakHeight > 0 ? m_currentPageBounds.Height : 0;

            // Calculate the remaining height.
            row.RowBreakHeight = m_currentBounds.Y + height - m_currentPageBounds.Height;

            // No need to explicit break if the row height is equal to grid height.
            foreach (PdfGridCell cell in row.Cells)
            {
                float cellHeight = cell.MeasureHeight();
                if (cellHeight == height && cell.Value is PdfGrid)
                    row.RowBreakHeight = 0;
                else if (cellHeight == height && (cell.Value as PdfGrid) == null)
                    row.RowBreakHeight = m_currentBounds.Y + height - m_currentPageBounds.Height;
            }

            for (int i = m_cellStartIndex; i <= m_cellEndIndex; i++)
            {
                bool cancelSpans = ((row.Cells[i].ColumnSpan + i > m_cellEndIndex + 1) && (row.Cells[i].ColumnSpan > 1));
                if (!cancelSpans)
                {
                    for (int j = 1; j < row.Cells[i].ColumnSpan; j++)
                    {
                        row.Cells[i + j].IsCellMergeContinue = true;
                    }
                }
                SizeF size = new SizeF(Grid.Columns[i].Width, m_newheight > 0.0 ? m_newheight : m_currentPageBounds.Height);
                if (!CheckIfDefaultFormat(Grid.Columns[i].Format) && CheckIfDefaultFormat(row.Cells[i].StringFormat))
                {
                    row.Cells[i].StringFormat = Grid.Columns[i].Format;
                }
                PdfStringLayoutResult stringResult = row.Cells[i].Draw(m_currentGraphics, new RectangleF(location, size), cancelSpans);

                //If still row is to be drawn, set cell finished drawing cell as false and update the text to be drawn.
                if (row.RowBreakHeight > 0.0 && stringResult != null)
                {
                    row.Cells[i].FinishedDrawingCell = false;
                    row.Cells[i].RemainingString = stringResult.Remainder == null ? string.Empty : stringResult.Remainder;
                }

                result.IsFinish = (!result.IsFinish) ? result.IsFinish : row.Cells[i].FinishedDrawingCell;
                location.X += Grid.Columns[i].Width;
            }
            m_currentBounds.Y += m_newheight > 0.0 ? m_newheight : height;

            result.Bounds = new RectangleF(result.Bounds.Location, new SizeF(location.X, location.Y));
        }

        /// <summary>
        /// Draws row
        /// </summary>
        /// <param name="result">The RowLayoutResult.</param>
        /// <param name="row">Row being drawn.</param>
        /// <param name="height">Height of the row.</param>
        private void DrawRow(ref RowLayoutResult result, PdfGridRow row, float height)
        {
            PointF location = m_currentBounds.Location;
            result.Bounds = new RectangleF(location, SizeF.Empty);

            height = ReCalculateHeight(row, height);

            for (int i = m_cellStartIndex; i <= m_cellEndIndex; i++)
            {
                bool cancelSpans = ((/*row.Cells[i].ColumnSpan +*/ i > m_cellEndIndex + 1) && (row.Cells[i].ColumnSpan > 1));
                if (!cancelSpans)
                {
                    for (int j = 1; j < row.Cells[i].ColumnSpan; j++)
                    {
                        row.Cells[i + j].IsCellMergeContinue = true;
                    }
                }
                SizeF size = new SizeF(Grid.Columns[i].Width, height);
                if (!CheckIfDefaultFormat(Grid.Columns[i].Format) && CheckIfDefaultFormat(row.Cells[i].StringFormat))
                {
                    row.Cells[i].StringFormat = Grid.Columns[i].Format;
                }
                PdfStringLayoutResult stringResult = row.Cells[i].Draw(m_currentGraphics, new RectangleF(location, size), cancelSpans);
                if (row.Grid.Style.AllowHorizontalOverflow && (row.Cells[i].ColumnSpan > m_cellEndIndex || i + row.Cells[i].ColumnSpan > m_cellEndIndex + 1) && m_cellEndIndex < row.Cells.Count - 1)
                    row.RowOverflowIndex = m_cellEndIndex;
                
                if (row.Grid.Style.AllowHorizontalOverflow && (row.RowOverflowIndex > 0 && (row.Cells[i].ColumnSpan > m_cellEndIndex || i + row.Cells[i].ColumnSpan > m_cellEndIndex + 1)) && row.Cells[i].ColumnSpan - m_cellEndIndex + i - 1 > 0)
                {
                    row.Cells[row.RowOverflowIndex + 1].Value = stringResult != null ? stringResult.m_remainder : null;
                    row.Cells[row.RowOverflowIndex + 1].StringFormat = row.Cells[i].StringFormat;
                    row.Cells[row.RowOverflowIndex + 1].Style = row.Cells[i].Style;
                    row.Cells[row.RowOverflowIndex + 1].ColumnSpan = row.Cells[i].ColumnSpan - m_cellEndIndex + i - 1;
                }
                location.X += Grid.Columns[i].Width;
            }
            m_currentBounds.Y += height;

            result.Bounds = new RectangleF(result.Bounds.Location, new SizeF(location.X, location.Y));
        }

        /// <summary>
        /// Recalculate row height for the split cell to be drawn.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private float ReCalculateHeight(PdfGridRow row, float height)
        {
            float newHeight = 0.0f;
            for (int i = m_cellStartIndex; i <= m_cellEndIndex; i++)
            {
                if (!string.IsNullOrEmpty(row.Cells[i].RemainingString))
                    newHeight = Math.Max(newHeight, row.Cells[i].MeasureHeight());
            }
            return Math.Max(height, newHeight);
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
                PdfGridBeginPageLayoutEventArgs args =
                    new PdfGridBeginPageLayoutEventArgs(currentBounds, currentPage, currentRow);

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
        private PdfGridEndPageLayoutEventArgs RaisePageLayouted(PdfLayoutResult result)
        {

            PdfGridEndPageLayoutEventArgs args = new PdfGridEndPageLayoutEventArgs(result);
            
            if (Element.RaiseEndPageLayout)
            {
                Element.OnEndPageLayout(args);
            }

            return args;
        }

        /// <summary>
        /// Checks if the given format is default format or not.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private bool CheckIfDefaultFormat(PdfStringFormat format)
        {
            PdfStringFormat defaultFormat = new PdfStringFormat();
            return (format.Alignment == defaultFormat.Alignment && format.CharacterSpacing == defaultFormat.CharacterSpacing && format.ClipPath == defaultFormat.ClipPath && format.FirstLineIndent == defaultFormat.FirstLineIndent && format.HorizontalScalingFactor == defaultFormat.HorizontalScalingFactor && format.LineAlignment == defaultFormat.LineAlignment && format.LineLimit == defaultFormat.LineLimit && format.LineSpacing == defaultFormat.LineSpacing && format.MeasureTrailingSpaces == defaultFormat.MeasureTrailingSpaces && format.NoClip == defaultFormat.NoClip && format.ParagraphIndent == defaultFormat.ParagraphIndent && format.RightToLeft == defaultFormat.RightToLeft && format.SubSuperScript == defaultFormat.SubSuperScript && format.WordSpacing == defaultFormat.WordSpacing && format.WordWrap == defaultFormat.WordWrap);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Determines the column draw ranges.
        /// </summary>
        private void DetermineColumnDrawRanges()
        {
            int startColumn = 0;
            int endColumn = 0;
            float cellWidths = 0f;
            float availableWidth = m_currentBounds.Width;

            for (int i = 0; i < Grid.Columns.Count; i++)
            {
                cellWidths += Grid.Columns[i].Width;
                if (cellWidths > availableWidth)
                {
                    float subWidths = 0;
                    for (int j = startColumn; j <= i; j++)
                    {
                        subWidths += Grid.Columns[j].Width;
                        if (subWidths > availableWidth)
                            break;
                        endColumn = j;
                    }

                    m_columnRanges.Add(new int[] { startColumn, endColumn });
                    startColumn = endColumn + 1;
                    cellWidths = (endColumn < i) ? Grid.Columns[i].Width : 0;
                }
            }

            m_columnRanges.Add(new int[] { startColumn, Grid.Columns.Count - 1 });
        }

        /// <summary>
        /// Gets the next page.
        /// </summary>
        /// <returns></returns>
        public PdfPage GetNextPage(PdfLayoutFormat format)
        {
            PdfSection section = m_currentPage.Section;
            PdfPage nextPage = null;
            int index = section.IndexOf(m_currentPage);

            if (index == section.Count - 1)
            {
                nextPage = section.Add();
            }
            else
            {
                nextPage = section[index + 1];
            }

            m_currentGraphics = nextPage.Graphics;
            m_currentBounds = new RectangleF(PointF.Empty, nextPage.GetClientSize());

            if (format.PaginateBounds != RectangleF.Empty)
            {
                m_currentBounds.X = format.PaginateBounds.X;
                m_currentBounds.Y = format.PaginateBounds.Y;
                //m_currentPageBounds.Height = format.PaginateBounds.Size.Height;
                m_currentBounds.Height = format.PaginateBounds.Size.Height;
            }

            return nextPage;
        }

        /// <summary>
        /// Gets the format.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        private PdfGridLayoutFormat GetFormat(PdfLayoutFormat format)
        {
            PdfGridLayoutFormat f = format as PdfGridLayoutFormat;

            if (format != null && f == null)
            {
                f = new PdfGridLayoutFormat(format);
            }

            return f;
        }

        /// <summary>
        /// Gets the layout result.
        /// </summary>
        /// <returns></returns>
        private PdfGridLayoutResult GetLayoutResult()
        {
            RectangleF bounds = new RectangleF(m_startLocation,
                new SizeF(m_currentBounds.Width, m_currentBounds.Y - m_startLocation.Y));

            return new PdfGridLayoutResult(m_currentPage, bounds);
        }
        #endregion

        #region Internals
        internal class RowLayoutResult
        {
            #region Fields
            private bool m_bIsFinished;
            private RectangleF m_layoutedBounds;
            #endregion

            #region Properties
            /// <summary>
            /// Gets or sets a value indicating whether this instance is finish.
            /// </summary>
            /// <value><c>true</c> if this instance is finish; otherwise, <c>false</c>.</value>
            public bool IsFinish
            {
                get
                {
                    return m_bIsFinished;
                }
                set
                {
                    m_bIsFinished = value;
                }
            }

            /// <summary>
            /// Gets or sets the bounds.
            /// </summary>
            /// <value>The bounds.</value>
            public RectangleF Bounds
            {
                get
                {
                    return m_layoutedBounds;
                }
                set
                {
                    m_layoutedBounds = value;
                }
            }
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="RowLayoutResult"/> class.
            /// </summary>
            public RowLayoutResult()
            {
                m_layoutedBounds = new RectangleF();
            }
            #endregion
        }
        #endregion

    }

    /// <summary>
    /// Defines parameters for PdfGrid layout.
    /// </summary>
    public class PdfGridLayoutFormat : PdfLayoutFormat
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridLayoutFormat"/> class.
        /// </summary>
        public PdfGridLayoutFormat()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridLayoutFormat"/> class.
        /// </summary>
        /// <param name="baseFormat">The base format.</param>
        public PdfGridLayoutFormat(PdfLayoutFormat baseFormat)
            : base(baseFormat)
        {
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class PdfGridLayoutResult : PdfLayoutResult
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PdfGridLayoutResult"/> class.
        /// </summary>
        /// <param name="page">The current page.</param>
        /// <param name="bounds">The current bounds.</param>
        /// <remarks>The page might be null, which means that
        /// lay outing was performed on PdfGraphics.</remarks>
        public PdfGridLayoutResult(PdfPage page, RectangleF bounds)
            : base(page, bounds)
        {

        }
        #endregion
    }
}
