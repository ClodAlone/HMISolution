//-------------------------------------------------------------------------------------------------
// <copyright file="GridScroll.cs" company="syncfusion">
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
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

using Syncfusion.Drawing;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Diagnostics;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;
////using Syncfusion.Windows.Forms.Grid.GridInternal;

//// TODO: Get rid of.
////using Syncfusion.Windows.Forms.Grid.Grouping;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// GridScroll implements scrollbar logic for GridControlBase. See GridControlBase.HScrollBehavior and GridControlBase.VScrollBehavior.
    /// </summary>
    [Syncfusion.Documentation.DocumentationExclude()]
    internal class GridScroll
    {
        // Fields
        GridControlBase grid;

        int _nTopRow = 0;

        internal int m_nTopRow
        {
            get
            {
                return _nTopRow;
            }

            set
            {
                _nTopRow = value;
                if (GridControlBase.UseOldHiddenScrollLogic)
                {
                    vScrollPos = value;
                }
                else
                {
                    vScrollPos = RowIndexToScrollPosition(value);
                }
            }
        }

        int vScrollPos = 0;

        public int VScrollPos
        {
            get
            {
                return vScrollPos;
            }

            set
            {
                vScrollPos = value;
                if (GridControlBase.UseOldHiddenScrollLogic)
                {
                    _nTopRow = value;
                }
                else
                {
                    _nTopRow = ScrollPositionToRowIndex(vScrollPos);
                }
            }
        }

        int _nLeftCol = 0;

        internal int m_nLeftCol
        {
            get
            {
                return _nLeftCol;
            }

            set
            {
                _nLeftCol = value;
                if (GridControlBase.UseOldHiddenScrollLogic)
                {
                    hScrollPos = value;
                }
                else
                {
                    hScrollPos = ColIndexToScrollPosition(value);
                }
            }
        }

        int hScrollPos = 0;

        public int HScrollPos
        {
            get
            {
                return hScrollPos;
            }

            set
            {
                hScrollPos = value;
                if (GridControlBase.UseOldHiddenScrollLogic)
                {
                    _nLeftCol = value;
                }
                else
                {
                    _nLeftCol = ScrollPositionToColIndex(hScrollPos);
                }
            }
        }

        internal int m_nLastSBRowCount = 0;      //// UpdateScrollbars stores grid.GetRowCount, GetColCount to these
        internal int m_nLastSBColCount = 0;      //// attributes.

        internal GridScrollbarMode hScrollSetting = GridScrollbarMode.DetectIfShared;
        internal GridScrollbarMode vScrollSetting = GridScrollbarMode.DetectIfShared;

        internal bool m_bInDoScroll = false;

        internal int m_cxOld = 0;
        internal int m_cyOld = 0;

        internal bool m_bUpdateScrollbar = false;

        internal bool ConsiderLocationOnly = true;

        bool inScroll = false;

        public override string ToString()
        {
            return String.Format(
                "TopRow={0},LeftCol={1},hScrollSetting={2},vScrollSetting={3},m_bInDoScroll={4},m_nLastSBRowCount={5},m_nLastSBColCount={6]",
                m_nTopRow,
                m_nLeftCol,
                hScrollSetting,
                vScrollSetting,
                m_bInDoScroll,
                m_nLastSBRowCount,
                m_nLastSBColCount);
        }

        /// <summary>
        /// Gets results of ToString method.
        /// </summary>
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        public bool GetNextRowIndex(ref int rowIndex)
        {
            int saved = rowIndex;
            bool result = GetNextRowIndex(ref rowIndex, rowIndex > 0);
            if (result && rowIndex == saved)
            {
                result = GetNextRowIndex(ref rowIndex, true);
            }

            return result;
        }

        public bool GetNextRowIndex(ref int rowIndex, bool skipFirstHidden)
        {
            int scrollPos = this.RowIndexToScrollPosition(rowIndex);

            if (scrollPos < this.GetMaxRowScrollPosition())
            {
                if (skipFirstHidden || !grid.GetRowHidden(rowIndex))
                {
                    scrollPos++;
                }

                rowIndex = ScrollPositionToRowIndex(scrollPos);
                return true;
            }

            return false;
        }

        public int GetNextRowIndex(int rowIndex, bool skipFirstHidden)
        {
            GetNextRowIndex(ref rowIndex, skipFirstHidden);
            return rowIndex;
        }

        public bool GetPrevRowIndex(ref int rowIndex)
        {
            int n = this.RowIndexToScrollPosition(rowIndex);
            if (n > 0)
            {
                rowIndex = ScrollPositionToRowIndex(n - 1);
                return true;
            }

            return false;
        }

        public int GetPrevRowIndex(int rowIndex)
        {
            GetPrevRowIndex(ref rowIndex);
            return rowIndex;
        }

        public bool ScrollPositionDiffersAtRowIndex(int rowIndex1, int rowIndex2)
        {
            return RowIndexToScrollPosition(rowIndex1) != RowIndexToScrollPosition(rowIndex2);
        }

        public bool RowIndexDiffersAtScrollPosition(int scrollPos1, int scrollPos2)
        {
            return ScrollPositionToRowIndex(scrollPos1) != ScrollPositionToRowIndex(scrollPos2);
        }

        public bool GetNextColIndex(ref int colIndex)
        {
            int saved = colIndex;
            bool result = GetNextColIndex(ref colIndex, colIndex > 0);
            if (result && colIndex == saved)
            {
                result = GetNextColIndex(ref colIndex, true);
            }

            return result;
        }

        public bool GetNextColIndex(ref int colIndex, bool skipFirstHidden)
        {
            int scrollPos = this.ColIndexToScrollPosition(colIndex);

            if (scrollPos < this.GetMaxColScrollPosition())
            {
                if (skipFirstHidden || !grid.GetColHidden(colIndex))
                {
                    scrollPos++;
                }

                colIndex = ScrollPositionToColIndex(scrollPos);
                return true;
            }

            return false;
        }

        public int GetNextColIndex(int colIndex, bool skipFirstHidden)
        {
            GetNextColIndex(ref colIndex, skipFirstHidden);
            return colIndex;
        }

        public bool GetPrevColIndex(ref int colIndex)
        {
            int n = this.ColIndexToScrollPosition(colIndex);
            if (n > 0)
            {
                colIndex = ScrollPositionToColIndex(n - 1);
                return true;
            }

            return false;
        }

        public int GetPrevColIndex(int colIndex)
        {
            GetPrevColIndex(ref colIndex);
            return colIndex;
        }

        public bool ScrollPositionDiffersAtColIndex(int colIndex1, int colIndex2)
        {
            return ColIndexToScrollPosition(colIndex1) != ColIndexToScrollPosition(colIndex2);
        }

        public bool ColIndexDiffersAtScrollPosition(int scrollPos1, int scrollPos2)
        {
            return ScrollPositionToColIndex(scrollPos1) != ScrollPositionToColIndex(scrollPos2);
        }

        #region NewHiddenScrollLogic
        internal GridScrollVisibleElementsMapper visibleColumns = new GridScrollVisibleElementsMapper();
        internal GridScrollVisibleElementsMapper visibleRows = new GridScrollVisibleElementsMapper();

        internal bool use42CodeForHiding 
        { 
            get 
            { 
                return Grid.GridControlBase.UseOldHiddenScrollLogic; 
            } 
        }

        // newcode with fallback to old hidden columns logic
        public int GetMaxRowScrollPosition()
        {
            if (use42CodeForHiding)
            {
                return OldGetMaxRowScrollPosition();
            }

            visibleRows.Count = grid.Model.RowCount + 1; // 1 extra (grid has x cols + 1 header ...)
            return Math.Max(0, visibleRows.VisibleCount - 1); // remove 1 (grid always has one more extra column, the header cell ...)
        }

        public int ScrollPositionToRowIndex(int scrollPos)
        {
            if (use42CodeForHiding)
            {
                return OldScrollPositionToRowIndex(scrollPos);
            }

            if (scrollPos > GetMaxRowScrollPosition())
            {
                return visibleRows.Count + 1;
            }

            return visibleRows.GetOriginalPositionAtVisiblePosition(scrollPos);
        }

        public int RowIndexToScrollPosition(int rowIndex)
        {
            if (use42CodeForHiding)
            {
                return OldRowIndexToScrollPosition(rowIndex);
            }

            return Math.Min(visibleRows.GetVisiblePositionForOriginalPosition(rowIndex), GetMaxRowScrollPosition());
        }

        void FixRowCount()
        {
            visibleRows.Count = grid.Model.RowCount + 1; // 1 extra (grid has x cols + 1 header ...)
        }

        void FixColCount()
        {
            visibleColumns.Count = grid.Model.ColCount + 1; // 1 extra (grid has x cols + 1 header ...)
        }

        void FixHScrollPos()
        {
            if (GridControlBase.UseOldHiddenScrollLogic)
            {
                hScrollPos = this._nLeftCol;
            }
            else
            {
                hScrollPos = ColIndexToScrollPosition(_nLeftCol);
            }
        }

        void FixVScrollPos()
        {
            if (GridControlBase.UseOldHiddenScrollLogic)
            {
                vScrollPos = this._nTopRow;
            }
            else
            {
                vScrollPos = RowIndexToScrollPosition(_nTopRow);
            }
        }

        public void RecalcHiddenRowState(int start, int end)
        {
            if (use42CodeForHiding)
            {
                OldRecalcHiddenRowState(start, end);
                return;
            }

            FixRowCount();
            end = Math.Min(grid.Model.RowCount, end);
            for (int n = start; n <= end; n++)
            {
                this.visibleRows[n] = !grid.GetRowHidden(n);
            }

            FixVScrollPos();
        }

        public void RecalcHiddenColState(int start, int end)
        {
            if (use42CodeForHiding)
            {
                OldRecalcHiddenColState(start, end);
                return;
            }

            FixColCount();
            end = Math.Min(grid.Model.ColCount, end);
            for (int n = start; n <= end; n++)
            {
                this.visibleColumns[n] = !grid.GetColHidden(n);
            }

            FixHScrollPos();
        }

        public void RecalcHiddenRowColState(GridControlBase otherGrid)
        {
            //// Fix hidden row and column state after a new pane was created 
            //// in a splitter control.
            RecalcHiddenColState(0, Math.Min(grid.Model.ColCount, otherGrid.ScrollGrid.visibleColumns.Count));
            RecalcHiddenRowState(0, Math.Min(grid.Model.RowCount, otherGrid.ScrollGrid.visibleRows.Count));
        }

        public void HideRows(int rowIndex, int count) ////, bool[] oldState)
        {
            if (use42CodeForHiding)
            {
                OldHideRows(rowIndex, count);
                return;
            }

            FixRowCount();
            this.visibleRows.HideRange(rowIndex, rowIndex + count - 1);
            FixVScrollPos();
        }

        public void ShowRows(int rowIndex, int count)
        {
            if (use42CodeForHiding)
            {
                OldShowRows(rowIndex, count);
                return;
            }

            FixRowCount();
            this.visibleRows.ShowRange(rowIndex, rowIndex + count - 1);
            FixVScrollPos();
        }

        public int GetMaxColScrollPosition()
        {
            if (use42CodeForHiding)
            {
                return OldGetMaxColScrollPosition();
            }

            visibleColumns.Count = grid.Model.ColCount + 1; // 1 extra (grid has x cols + 1 header ...)
            return Math.Max(0, visibleColumns.VisibleCount - 1); // remove 1 (grid always has one more extra column, the header cell ...)
        }

        public int ScrollPositionToColIndex(int scrollPos)
        {
            if (use42CodeForHiding)
            {
                return OldScrollPositionToColIndex(scrollPos);
            }

            if (scrollPos > GetMaxColScrollPosition())
            {
                return visibleColumns.Count + 1;
            }

            return visibleColumns.GetOriginalPositionAtVisiblePosition(scrollPos);
        }

        public int ColIndexToScrollPosition(int colIndex)
        {
            if (use42CodeForHiding)
            {
                return OldColIndexToScrollPosition(colIndex);
            }

            return Math.Min(visibleColumns.GetVisiblePositionForOriginalPosition(colIndex), GetMaxColScrollPosition());
        }

        public void HideCols(int colIndex, int count)
        {
            if (use42CodeForHiding)
            {
                OldHideCols(colIndex, count);
                return;
            }

            FixColCount();
            this.visibleColumns.HideRange(colIndex, colIndex + count - 1);
            FixHScrollPos();
        }

        public void ShowCols(int colIndex, int count)
        {
            if (use42CodeForHiding)
            {
                OldShowCols(colIndex, count);
                return;
            }

            FixColCount();
            this.visibleColumns.ShowRange(colIndex, colIndex + count - 1);
            FixHScrollPos();
        }
        #endregion
        #region OldHiddenScrollLogic for backward compatibility
        ArrayList scrollPositionToRowIndex = new ArrayList();
        ArrayList rowIndexToScrollPosition = new ArrayList();
        int hiddenRowCount = 0;

        int OldGetMaxRowScrollPosition()
        {
            return grid.Model.RowCount - hiddenRowCount;
        }

        int OldScrollPositionToRowIndex(int scrollPos)
        {
            if (scrollPos < 0)
            {
                return -1;
            }
            else if (scrollPos >= scrollPositionToRowIndex.Count)
            {
                return scrollPos + hiddenRowCount;
            }
            else
            {
                return (int)scrollPositionToRowIndex[scrollPos];
            }
        }

        int OldRowIndexToScrollPosition(int rowIndex)
        {
            if (rowIndex < 0)
            {
                return -1;
            }
            else if (rowIndex >= rowIndexToScrollPosition.Count)
            {
                return rowIndex - hiddenRowCount;
            }
            else
            {
                return (int)rowIndexToScrollPosition[rowIndex];
            }
        }

        void OldEnsureScrollPositionToRowIndexCount(int scrollPos)
        {
            if (this.scrollPositionToRowIndex.Count == 0)
            {
                this.scrollPositionToRowIndex.Add(0);
            }

            int lastRowIndex = (int)this.scrollPositionToRowIndex[this.scrollPositionToRowIndex.Count - 1];
            while (this.scrollPositionToRowIndex.Count <= scrollPos)
            {
                this.scrollPositionToRowIndex.Add(++lastRowIndex);
            }
        }

        void OldEnsureRowIndexToScrollPositionCount(int rowIndex)
        {
            if (this.rowIndexToScrollPosition.Count == 0)
            {
                this.rowIndexToScrollPosition.Add(0);
            }

            int lastScrollPos = (int)this.rowIndexToScrollPosition[this.rowIndexToScrollPosition.Count - 1];
            while (this.rowIndexToScrollPosition.Count <= rowIndex)
            {
                this.rowIndexToScrollPosition.Add(++lastScrollPos);
            }
        }

        void OldRecalcHiddenRowState(int start, int end)
        {
            if (start >= rowIndexToScrollPosition.Count)
            {
                return;
            }

            int rowIndex = Math.Max(0, start - 1);
            int lastScrollPos = (int)this.rowIndexToScrollPosition[rowIndex];
            int count = Math.Max(rowIndexToScrollPosition.Count, end);
            for (int row = rowIndex + 1; row < count; row++)
            {
                if (!grid.GetRowHidden(row))
                {
                    lastScrollPos++;
                }

                if (row < rowIndexToScrollPosition.Count)
                {
                    this.rowIndexToScrollPosition[row] = lastScrollPos;
                }
                else
                {
                    this.rowIndexToScrollPosition.Add(lastScrollPos);
                }
            }

            this.scrollPositionToRowIndex.Clear();
            int lastRowIndex = 0;
            this.scrollPositionToRowIndex.Add(0);
            hiddenRowCount = 0;
            while (this.scrollPositionToRowIndex.Count <= lastScrollPos)
            {
                lastRowIndex++;
                if (!grid.GetRowHidden(lastRowIndex))
                {
                    this.scrollPositionToRowIndex.Add(lastRowIndex);
                }
                else
                {
                    this.hiddenRowCount++;
                }
            }
        }

        void OldRecalcHiddenColState(int start, int end)
        {
            if (start >= colIndexToScrollPosition.Count)
            {
                return;
            }

            int colIndex = Math.Max(0, start - 1);
            int lastScrollPos = (int)this.colIndexToScrollPosition[colIndex];
            int count = Math.Max(colIndexToScrollPosition.Count, end);
            for (int col = colIndex + 1; col < count; col++)
            {
                if (!grid.GetColHidden(col))
                {
                    lastScrollPos++;
                }
                else if (col == count - 1)
                {
                    count++;
                }

                if (col < colIndexToScrollPosition.Count)
                {
                    this.colIndexToScrollPosition[col] = lastScrollPos;
                }
                else
                {
                    this.colIndexToScrollPosition.Add(lastScrollPos);
                }
            }

            this.scrollPositionToColIndex.Clear();
            int lastColIndex = 0;
            this.scrollPositionToColIndex.Add(0);
            hiddenColCount = 0;
            while (this.scrollPositionToColIndex.Count <= lastScrollPos)
            {
                lastColIndex++;
                if (!grid.GetColHidden(lastColIndex))
                {
                    this.scrollPositionToColIndex.Add(lastColIndex);
                }
                else
                {
                    this.hiddenColCount++;
                }
            }

            ////            this.DumpCol();
        }

        void _HideRows(int rowIndex, int count)
        {
            int n;
            int scrollPos = this.RowIndexToScrollPosition(rowIndex);
            this.OldEnsureRowIndexToScrollPositionCount(rowIndex + count);

            for (n = rowIndex; n < rowIndex + count; n++)
            {
                this.rowIndexToScrollPosition[n] = (int)this.RowIndexToScrollPosition(rowIndex - 1);
            }

            for (; n < this.rowIndexToScrollPosition.Count; n++)
            {
                this.rowIndexToScrollPosition[n] = (int)this.rowIndexToScrollPosition[n] - count;
            }

            this.OldEnsureScrollPositionToRowIndexCount(scrollPos + count - 1);

            int spCount = this.scrollPositionToRowIndex.Count;
            for (n = scrollPos; n < spCount; n++)
            {
                this.scrollPositionToRowIndex[n] = (int)this.ScrollPositionToRowIndex(n + count);
            }

            this.hiddenRowCount += count;
        }

        void OldHideRows(int rowIndex, int count) ////, bool[] oldState)
        {
            if (rowIndex < grid.InternalGetFrozenRows() + 1)
            {
                int diff = grid.InternalGetFrozenRows() + 1 - rowIndex;
                count -= diff;
                rowIndex += diff;
            }

            ////            TraceUtil.TraceCurrentMethodInfo(rowIndex, count);
            int c = 0;
            int oRowIndex = rowIndex;
            for (int n = 0; n < count; n++)
            {
                if (this.RowIndexToScrollPosition(n + oRowIndex) == this.RowIndexToScrollPosition(n + oRowIndex - 1))
                {
                    if (c > 0)
                    {
                        _HideRows(rowIndex, c);
                    }

                    rowIndex += c + 1;
                    c = 0;
                }
                else
                {
                    c++;
                }
            }
            //// && this.RowIndexToScrollPosition(count+rowIndex) != this.RowIndexToScrollPosition(count+rowIndex-1))
            if (c > 0)
            {
                _HideRows(rowIndex, c);
            }
            ////            DumpRows();
        }

        void DumpRows()
        {
            for (int n = 0; n < this.rowIndexToScrollPosition.Count; n++)
            {
                Trace.WriteLine(String.Format("Row {0}, Pos {1}", n, rowIndexToScrollPosition[n]));
            }

            for (int n = 0; n < this.scrollPositionToRowIndex.Count; n++)
            {
                Trace.WriteLine(String.Format("Pos {0}, Row {1}", n, scrollPositionToRowIndex[n]));
            }
        }

        void _ShowRows(int rowIndex, int count)
        {
            int n;
            int scrollPos = this.RowIndexToScrollPosition(rowIndex);

            this.OldEnsureRowIndexToScrollPositionCount(rowIndex + count);
            int row = (int)this.rowIndexToScrollPosition[rowIndex];
            for (n = rowIndex; n < rowIndex + count; n++)
            {
                this.rowIndexToScrollPosition[n] = ++row;
            }

            for (; n < this.rowIndexToScrollPosition.Count; n++)
            {
                this.rowIndexToScrollPosition[n] = (int)this.rowIndexToScrollPosition[n] + count;
            }

            this.OldEnsureScrollPositionToRowIndexCount(scrollPos);
            this.OldEnsureScrollPositionToRowIndexCount(scrollPositionToRowIndex.Count + count);

            int spCount = this.scrollPositionToRowIndex.Count;
            for (n = spCount - 1; n > scrollPos + count; n--)
            {
                this.scrollPositionToRowIndex[n] = this.ScrollPositionToRowIndex(n - count);
            }

            row = rowIndex;
            for (int i = 0; i < count; i++)
            {
                this.scrollPositionToRowIndex[i + scrollPos + 1] = row++;
            }

            this.hiddenRowCount -= count;
        }

        void OldShowRows(int rowIndex, int count)
        {
            int topScrollPos = this.RowIndexToScrollPosition(this.m_nTopRow);

            if (rowIndex < grid.InternalGetFrozenRows() + 1)
            {
                int diff = grid.InternalGetFrozenRows() + 1 - rowIndex;
                count -= diff;
                rowIndex += diff;
            }

            ////TraceUtil.TraceCurrentMethodInfo(rowIndex, count);
            int c = 0;
            for (int n = 0; n < count; n++)
            {
                if (this.RowIndexToScrollPosition(c + rowIndex) != this.RowIndexToScrollPosition(c + rowIndex - 1))
                {
                    if (c > 0)
                    {
                        _ShowRows(rowIndex, c);
                        rowIndex += c + 1;
                        c = 0;
                    }
                    else
                    {
                        rowIndex++;
                        c = 0;
                    }
                }
                else
                {
                    c++;
                }
            }

            if (c > 0)
            {
                _ShowRows(rowIndex, c);
            }

            grid.TopRowIndex = this.ScrollPositionToRowIndex(topScrollPos);

            ////Dump();
        }    
        
        ArrayList scrollPositionToColIndex = new ArrayList();
        ArrayList colIndexToScrollPosition = new ArrayList();
        int hiddenColCount = 0;

        int OldGetMaxColScrollPosition()
        {
            return grid.Model.ColCount - hiddenColCount;
        }

        int OldScrollPositionToColIndex(int scrollPos)
        {
            if (scrollPos < 0)
            {
                return -1;
            }
            else if (scrollPos >= scrollPositionToColIndex.Count)
            {
                return scrollPos + hiddenColCount;
            }
            else
            {
                return (int)scrollPositionToColIndex[scrollPos];
            }
        }

        int OldColIndexToScrollPosition(int colIndex)
        {
            if (colIndex < 0)
            {
                return -1;
            }
            else if (colIndex >= colIndexToScrollPosition.Count)
            {
                return colIndex - hiddenColCount;
            }
            else
            {
                return (int)colIndexToScrollPosition[colIndex];
            }
        }

        void OldEnsureScrollPositionToColIndexCount(int scrollPos)
        {
            if (this.scrollPositionToColIndex.Count == 0)
            {
                this.scrollPositionToColIndex.Add(0);
            }

            int lastColIndex = (int)this.scrollPositionToColIndex[this.scrollPositionToColIndex.Count - 1];
            while (this.scrollPositionToColIndex.Count <= scrollPos)
            {
                this.scrollPositionToColIndex.Add(++lastColIndex);
            }
        }

        void OldEnsureColIndexToScrollPositionCount(int colIndex)
        {
            if (this.colIndexToScrollPosition.Count == 0)
            {
                this.colIndexToScrollPosition.Add(0);
            }

            int lastScrollPos = (int)this.colIndexToScrollPosition[this.colIndexToScrollPosition.Count - 1];
            while (this.colIndexToScrollPosition.Count <= colIndex)
            {
                this.colIndexToScrollPosition.Add(++lastScrollPos);
            }
        }

        void _HideCols(int colIndex, int count)
        {
            int n;
            int scrollPos = this.ColIndexToScrollPosition(colIndex);
            this.OldEnsureColIndexToScrollPositionCount(colIndex + count);

            for (n = colIndex; n < colIndex + count; n++)
            {
                this.colIndexToScrollPosition[n] = (int)this.ColIndexToScrollPosition(colIndex - 1);
            }

            for (; n < this.colIndexToScrollPosition.Count; n++)
            {
                this.colIndexToScrollPosition[n] = (int)this.colIndexToScrollPosition[n] - count;
            }

            this.OldEnsureScrollPositionToColIndexCount(scrollPos + count - 1);
            this.OldEnsureScrollPositionToColIndexCount(grid.InternalGetFrozenCols() + 1);

            int spCount = this.scrollPositionToColIndex.Count;
            for (n = scrollPos; n < spCount; n++)
            {
                this.scrollPositionToColIndex[n] = (int)this.ScrollPositionToColIndex(n + count);
            }

            this.hiddenColCount += count;
        }

        void OldHideCols(int colIndex, int count)
        {
            if (colIndex < grid.InternalGetFrozenCols() + 1)
            {
                int diff = grid.InternalGetFrozenCols() + 1 - colIndex;
                count -= diff;
                colIndex += diff;
            }

            ////TraceUtil.TraceCurrentMethodInfo(colIndex, count);
            int c = 0;
            int oColIndex = colIndex;
            for (int n = 0; n < count; n++)
            {
                if (this.ColIndexToScrollPosition(n + oColIndex) == this.ColIndexToScrollPosition(n + oColIndex - 1))
                {
                    if (c > 0)
                    {
                        _HideCols(colIndex, c);
                    }

                    colIndex += c + 1;
                    c = 0;
                }
                else
                {
                    c++;
                }
            }

            if (c > 0)
            {
                _HideCols(colIndex, c);
            }
            ////            DumpCol();
        }

        void DumpCol()
        {
            Trace.WriteLine(String.Format("Hidden Cols: {0}", this.hiddenColCount));

            for (int n = 0; n < this.colIndexToScrollPosition.Count; n++)
            {
                Trace.WriteLine(String.Format("Col {0}, Pos {1}", n, colIndexToScrollPosition[n]));
            }

            for (int n = 0; n < this.scrollPositionToColIndex.Count; n++)
            {
                Trace.WriteLine(String.Format("Pos {0}, Col {1}", n, scrollPositionToColIndex[n]));
            }
        }

        void _ShowCols(int colIndex, int count)
        {
            int n;
            int scrollPos = this.ColIndexToScrollPosition(colIndex);

            this.OldEnsureColIndexToScrollPositionCount(colIndex + count);
            int col = (int)this.colIndexToScrollPosition[colIndex];
            for (n = colIndex; n < colIndex + count; n++)
            { 
                this.colIndexToScrollPosition[n] = ++col;
            }

            for (; n < this.colIndexToScrollPosition.Count; n++)
            {
                this.colIndexToScrollPosition[n] = (int)this.colIndexToScrollPosition[n] + count;
            }

            this.OldEnsureScrollPositionToColIndexCount(scrollPos);
            this.OldEnsureScrollPositionToColIndexCount(scrollPositionToColIndex.Count + count);

            int spCount = this.scrollPositionToColIndex.Count;
            for (n = spCount - 1; n > scrollPos + count; n--)
            {
                this.scrollPositionToColIndex[n] = this.ScrollPositionToColIndex(n - count);
            }

            col = colIndex;
            for (int i = 0; i < count; i++)
            {
                this.scrollPositionToColIndex[i + scrollPos + 1] = col++;
            }

            this.hiddenColCount -= count;
        }

        void OldShowCols(int colIndex, int count)
        {
            int leftScrollPos = this.ColIndexToScrollPosition(this.m_nLeftCol);

            if (colIndex < grid.InternalGetFrozenCols() + 1)
            {
                int diff = grid.InternalGetFrozenCols() + 1 - colIndex;
                count -= diff;
                colIndex += diff;
            }

            ////TraceUtil.TraceCurrentMethodInfo(colIndex, count);
            int c = 0;
            for (int n = 0; n < count; n++)
            {
                if (this.ColIndexToScrollPosition(c + colIndex) != this.ColIndexToScrollPosition(c + colIndex - 1))
                {
                    if (c > 0)
                    {
                        _ShowCols(colIndex, c);
                        colIndex += c + 1;
                        c = 0;
                    }
                    else
                    {
                        colIndex++;
                        c = 0;
                    }
                }
                else
                {
                    c++;
                }
            }

            if (c > 0)
            {
                _ShowCols(colIndex, c);
            }
            
            grid.LeftColIndex = this.ScrollPositionToColIndex(leftScrollPos);
            ////Dump();
        }
        #endregion

        [Obsolete("Use GridControlBase.HScrollBehavior method")]
        public GridScrollbarMode HScrollSetting
        {
            get
            {
                return this.hScrollSetting;
            }

            set
            {
                this.hScrollSetting = value;
            }
        }

        [Obsolete("Use GridControlBase.VScrollBehavior method")]
        public GridScrollbarMode VScrollSetting
        {
            get
            {
                return this.vScrollSetting;
            }

            set
            {
                this.vScrollSetting = value;
            }
        }

        public GridScroll(GridControlBase grid)
        {
            this.grid = grid;
            WireScrollEvents();
        }

        void WireScrollEvents()
        {
            grid.HorizontalScroll += new ScrollEventHandler(OnHScrollBarScroll);
            grid.VerticalScroll += new ScrollEventHandler(OnVScrollBarScroll);
            grid.HScrollBar.ValueChanged += new EventHandler(OnHScrollBarValueChanged);
            grid.VScrollBar.ValueChanged += new EventHandler(OnVScrollBarValueChanged);
            grid.IntelliMouseDragScrolling += new IntelliMouseDragScrollEventHandler(IntelliMouseDragScrollEvent);
        }

        void UnwireScrollEvents()
        {
            grid.HorizontalScroll -= new ScrollEventHandler(OnHScrollBarScroll);
            grid.VerticalScroll -= new ScrollEventHandler(OnVScrollBarScroll);
            grid.HScrollBar.ValueChanged -= new EventHandler(OnHScrollBarValueChanged);
            grid.VScrollBar.ValueChanged -= new EventHandler(OnVScrollBarValueChanged);
            grid.IntelliMouseDragScrolling -= new IntelliMouseDragScrollEventHandler(IntelliMouseDragScrollEvent);
        }

        public void Dispose()
        {
            if (grid != null)
            {
                UnwireScrollEvents();
            }

            GC.SuppressFinalize(this);
        }

        private void NotifyRenderer(int message)
        {
            GridCellRendererBase cellRenderer = grid.CurrentCell.Renderer;
            if (cellRenderer != null)
            {
                Message msg = Message.Create(grid.GetWindow().Handle, message, (IntPtr)0, (IntPtr)0);
                cellRenderer.OnNotifyMsg(ref msg);
            }
        }

        const int hAutoScrollPixelDelta = 20;

        protected virtual void OnHScrollBarScroll(object sender, ScrollEventArgs se)
        {
            if (!grid.hPixelScroll)
            {
                return;
            }

            int newValue = 0;

            bool isRTL = grid.IsRightToLeft();

            int hScrollValue = grid.HScrollBar.Value;
            ScrollEventType seType = se.Type;

            if (isRTL)
            {
                hScrollValue = this.ReverseHScrollValueRTL(hScrollValue);
                se.NewValue = this.ReverseHScrollValueRTL(se.NewValue);
                if (grid.HScrollBar.InnerScrollBar.RightToLeft == RightToLeft.No)
                {
                    // Only do these for internal (WS_HSCROLL style) scrollbars,
                    // not for shared scrollbars since Windows Forms already
                    // does flip the types.
                    switch (se.Type)
                    {
                        case ScrollEventType.SmallDecrement:
                            seType = ScrollEventType.SmallIncrement;
                            break;
                        case ScrollEventType.SmallIncrement:
                            seType = ScrollEventType.SmallDecrement;
                            break;
                    }
                }
            }

            switch (seType)
            {
                case ScrollEventType.SmallDecrement:
                    {
                        if (grid.AutoScrolling != ScrollBars.None)
                        {
                            se.NewValue = hScrollValue - hAutoScrollPixelDelta;
                        }
                        else
                        {
                            if (grid.hScrollPixelDelta > 0)
                            {
                                se.NewValue = hScrollValue - grid.hScrollPixelDelta;
                            }
                            else
                            {
                                int colIndex = grid.LeftColIndex;
                                int width = 0;
                                while (width == 0 && GetPrevColIndex(ref colIndex))
                                {
                                    width = grid.GetColWidth(colIndex);
                                }

                                se.NewValue = hScrollValue - width;
                            }
                        }
                       
                        se.NewValue = Math.Max(se.NewValue, grid.HScrollBar.Minimum);
                        break;
                    }

                case ScrollEventType.SmallIncrement:
                    {
                        grid.ViewLayout.Reset();
                        if (grid.AutoScrolling != ScrollBars.None)
                        {
                            se.NewValue = hScrollValue + hAutoScrollPixelDelta;
                        }
                        else
                        {
                            if (grid.hScrollPixelDelta > 0)
                            {
                                newValue = hScrollValue + grid.GetColWidth(grid.LeftColIndex) - grid.hScrollPixelDelta;
                            }
                            else
                            {
                                newValue = hScrollValue + grid.GetColWidth(grid.LeftColIndex);
                            }

                            se.NewValue = Math.Max(se.NewValue, newValue);
                        }

                        se.NewValue = Math.Min(se.NewValue, grid.HScrollBar.Maximum - grid.HScrollBar.LargeChange);
                        break;
                    }

                default:
                    break;
            }

            if (isRTL)
            {
                se.NewValue = this.ReverseHScrollValueRTL(se.NewValue);
            }
        }

        void IntelliMouseDragScrollEvent(object sender, IntelliMouseDragScrollEventArgs e)
        {
            int dy = e.DY;
            int dx = e.DX;
            int newValue;

            try
            {
                if (!e.Cancel)
                {
                    if (Math.Abs(dy) > Math.Abs(dx))
                    {
                        return;
                    }
                    else
                    {
                        TraceUtil.TraceCurrentMethodInfo(e);
                        if (!grid.hPixelScroll)
                        {
                            return;
                        }

                        newValue = grid.HScrollBar.Value - (dx < 0 ? hAutoScrollPixelDelta : -hAutoScrollPixelDelta);
                        newValue = GridUtil.MinMax(newValue, grid.HScrollBar.Minimum, grid.HScrollBar.Maximum - grid.HScrollBar.LargeChange + 1);
                        grid.HScrollBar.Value = this.ReverseHScrollValueRTL(newValue);
                        e.Scrolled = true;
                    }
                }
            }
            finally
            {
            }
        }

        protected virtual void OnVScrollBarScroll(object sender, ScrollEventArgs se)
        {
            if (!grid.vPixelScroll)
            {
                return;
            }

            switch (se.Type)
            {
                case ScrollEventType.SmallDecrement:
                    {
                        if (grid.vScrollPixelDelta > 0)
                        {
                            se.NewValue = grid.VScrollBar.Value - grid.vScrollPixelDelta;
                        }
                        else
                        {
                            int rowIndex = grid.TopRowIndex;
                            int height = 0;
                            while (height == 0 && GetPrevRowIndex(ref rowIndex))
                            {
                                height = grid.GetRowHeight(rowIndex);
                            }

                            se.NewValue = grid.VScrollBar.Value - height;
                        }

                        se.NewValue = Math.Max(se.NewValue, grid.VScrollBar.Minimum);
                        break;
                    }

                case ScrollEventType.SmallIncrement:
                    {
                        grid.ViewLayout.Reset();
                        int topRowHeight = grid.GetRowHeight(grid.TopRowIndex);
                        if (grid.vScrollPixelDelta > 0 && grid.vScrollPixelDelta < topRowHeight)
                        {
                            se.NewValue = grid.VScrollBar.Value + topRowHeight - grid.vScrollPixelDelta;
                        }
                        else
                        {
                            se.NewValue = grid.VScrollBar.Value + topRowHeight;
                        }

                        se.NewValue = Math.Min(se.NewValue, grid.VScrollBar.Maximum - grid.VScrollBar.LargeChange);
                        break;
                    }

                default:
                    break;
            }
        }

        protected virtual void OnHScrollBarValueChanged(object sender, EventArgs e)
        {
            if (inScroll || (!grid.HScrollBar.SupportsThumbTrack && grid.HScrollBar.IsThumbTracking))
            {
                return;
            }

            if (grid.Visible)
            {
                inScroll = true;
                grid.CurrentCell.CloseDropDown(PopupCloseType.Deactivated);
                NotifyRenderer(NativeMethods.WM_HSCROLL);
                if (grid.hPixelScroll)
                {
                    grid.SetCurrentHScrollPixelPos(ReverseHScrollValueRTL(grid.HScrollBar.Value));
                }
                else
                {
                    grid.LeftColIndex = this.ScrollPositionToColIndex(ReverseHScrollValueRTL(grid.HScrollBar.Value));
                }

                inScroll = false;
            }
        }

        protected virtual void OnVScrollBarValueChanged(object sender, EventArgs e)
        {
            if (inScroll || (!grid.VScrollBar.SupportsThumbTrack && grid.VScrollBar.IsThumbTracking))
            {
                return;
            }

            if (grid.Visible)
            {
                inScroll = true;
                grid.CurrentCell.CloseDropDown(PopupCloseType.Deactivated);
                NotifyRenderer(NativeMethods.WM_VSCROLL);
                if (grid.vPixelScroll)
                {
                    grid.SetCurrentVScrollPixelPos(grid.VScrollBar.Value);
                }
                else
                {
                    grid.TopRowIndex = this.ScrollPositionToRowIndex(grid.VScrollBar.Value);
                }

                inScroll = false;
            }
        }

        internal Rectangle GetCurrentCellBoundsCore(GridCellRendererBase pControl, int rowIndex, int colIndex)
        {
            Rectangle r = pControl.GetCellBoundsCoreInt(rowIndex, colIndex, true);
            if (ConsiderLocationOnly)
            {
                r.Width = 0;
                r.Height = 0;
            }

            return r;
            /*
                    if (rowIndex <= grid.ViewLayout.LastVisibleRow && colIndex <= grid.ViewLayout.LastVisibleCol)
            {
                GridRangeInfo rgCell = grid.ViewLayout.CombineSpannedRanges(GridRangeInfo.Cell(rowIndex, colIndex));
                if (grid.ViewLayout.IsRangeVisible(rgCell))
                {
                    Rectangle r = grid.RangeInfoToRectangle(rgCell, GridRangeOptions.CalculateNonClientArea);
                    if (ConsiderLocationOnly)
                    {
                        r.Width = 0;
                        r.Height = 0;
                    }
                    return r;
                }
            }
            return Rectangle.Empty;
*/
        }

        public bool DoPixelScroll(GridDirectionType direction, int amount)
        {
            if (this.m_bInDoScroll)
            {
                return false;
            }

            bool success = false;
            bool vert = false;
            bool horz = false;
            Rectangle scrollAreaBounds = Rectangle.Empty;
            Rectangle clipBounds = Rectangle.Empty;

            //// Current cell
            Rectangle currentCellBounds = Rectangle.Empty;
            GridCellRendererBase cellRenderer = null;
            Control cellControl = null;
            int currentCellRowIndex, currentCellColIndex;
            grid.GetScrollOutOfViewCurrentCellState(out cellRenderer, out cellControl, out currentCellBounds, out currentCellRowIndex, out currentCellColIndex, direction);

            m_bInDoScroll = true;
            try
            {
                switch (direction)
                {
                    case GridDirectionType.Left:
                        {
                            int hScrollPixelPos = grid.GetCurrentHScrollPixelPos();
                            int xAmount = amount;
                            int minHScrollPixelPos = grid.GetHScrollPixelMinimum();
                            hScrollPixelPos = Math.Max(minHScrollPixelPos, hScrollPixelPos - xAmount);

                            scrollAreaBounds = grid.ViewLayout.HscrollAreaBounds;
                            clipBounds = scrollAreaBounds;

                            if (xAmount != 0)
                            {
                                // Give the programmer the possibility to inhibit scrolling.
                                if (grid.RaiseHScrollPixelPosChanging(hScrollPixelPos))
                                {
                                    if (grid.IsRightToLeft())
                                    {
                                        GridUtil.OffsetLeft(ref scrollAreaBounds, xAmount - 1);
                                    }
                                    else
                                    {
                                        scrollAreaBounds.Width -= xAmount - 1;
                                    }

                                    bool noScroll = scrollAreaBounds.Left >= scrollAreaBounds.Right || scrollAreaBounds.Width < clipBounds.Width / 4;

                                    int pixelDelta;
                                    int leftColIndex;
                                    grid.HScrollPixelPosToColIndex(hScrollPixelPos, out leftColIndex, out pixelDelta);

                                    if (grid.NotifyLeftColChanging(leftColIndex))
                                    {
                                        ////                                    currentCellBounds.Offset(xAmount, 0);
                                        //// Take away current cell focus if current cell is scrolled out of view.
                                        if (cellRenderer != null
                                            && cellRenderer.HasFocusControl
                                            && !grid.InternalIsFrozenCol(currentCellColIndex)
                                            && (!scrollAreaBounds.Contains(currentCellBounds)
                                            || currentCellBounds.IsEmpty))
                                        {
                                            cellRenderer.SetHasFocusControl(false);
                                            if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                            {
                                                grid.Focus();
                                                if (!noScroll)
                                                { 
                                                    grid.Update();
                                                }
                                            }
                                        }

                                        this.m_nLeftCol = leftColIndex;
                                        grid.hScrollPixelDelta = pixelDelta;
                                        grid.ViewLayout.Reset();

                                        // Scroll view or simply invalidate; this is more efficient.
                                        if (noScroll)
                                        {
                                            grid.InternalInvalidate(clipBounds);
                                        }
                                        else
                                        {
                                            if (grid.IsRightToLeft())
                                            {
                                                grid.ScrollWindow(-xAmount, 0, clipBounds, clipBounds, true);
                                            }
                                            else
                                            {
                                                grid.ScrollWindow(xAmount, 0, clipBounds, clipBounds, true);
                                            }
                                        }

                                        // Update scrollbar value (and related splitter panes in a SplitterControl).
                                        if (!this.m_bUpdateScrollbar)
                                        {
                                            grid.HScrollBar.Value = ReverseHScrollValueRTL(hScrollPixelPos);
                                        }

                                        grid.Update();

                                        success = true;
                                        horz = true;
                                    }
                                }
                            }
                        }

                        break;

                    case GridDirectionType.Right:
                        {
                            int hScrollPixelPos = grid.GetCurrentHScrollPixelPos();
                            int xAmount = amount;

                            scrollAreaBounds = grid.ViewLayout.HscrollAreaBounds;
                            clipBounds = scrollAreaBounds;

                            // Now it is safe to use Model.ColCount.
                            int maxHScrollPixelPos = grid.GetHScrollPixelWidth(); //// - grid.ViewLayout.HscrollAreaBounds.Width;
                            hScrollPixelPos = Math.Min(maxHScrollPixelPos, hScrollPixelPos + xAmount);

                            if (maxHScrollPixelPos > hScrollPixelPos)
                            {
                                // Give the programmer the possibility to inhibit scrolling.
                                if (grid.RaiseHScrollPixelPosChanging(hScrollPixelPos))
                                {
                                    if (grid.IsRightToLeft())
                                    {
                                        scrollAreaBounds.Width -= xAmount;
                                    }
                                    else
                                    {
                                        GridUtil.OffsetLeft(ref scrollAreaBounds, xAmount);
                                    }

                                    bool noScroll = scrollAreaBounds.Left >= scrollAreaBounds.Right || scrollAreaBounds.Width < clipBounds.Width / 4;

                                    int pixelDelta;
                                    int leftColIndex;
                                    grid.HScrollPixelPosToColIndex(hScrollPixelPos, out leftColIndex, out pixelDelta);

                                    if (grid.NotifyLeftColChanging(leftColIndex))
                                    {
                                        ////                                    currentCellBounds.Offset(-xAmount, 0);
                                        //// Take away current cell focus if current cell is scrolled out of view.
                                        if (cellRenderer != null
                                            && cellRenderer.HasFocusControl
                                            && !grid.InternalIsFrozenCol(currentCellColIndex)
                                            && (!scrollAreaBounds.Contains(currentCellBounds)
                                            || currentCellBounds.IsEmpty))
                                        {
                                            cellRenderer.SetHasFocusControl(false);
                                            if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                            {
                                                grid.Focus();
                                                if (!noScroll)
                                                {
                                                    grid.Update();
                                                }
                                            }
                                        }

                                        this.m_nLeftCol = leftColIndex;
                                        grid.hScrollPixelDelta = pixelDelta;
                                        grid.ViewLayout.Reset();

                                        // Scroll view or simply invalidate; this is more efficient.
                                        if (noScroll)
                                        {
                                            grid.InternalInvalidate(clipBounds);
                                        }
                                        else
                                        {
                                            if (grid.IsRightToLeft())
                                            {
                                                grid.ScrollWindow(xAmount, 0, clipBounds, clipBounds, true);
                                            }
                                            else
                                            {
                                                grid.ScrollWindow(-xAmount, 0, clipBounds, clipBounds, true);
                                            }
                                        }

                                        // Update scrollbar value (and related splitter panes in a SplitterControl).
                                        if (!this.m_bUpdateScrollbar)
                                        {
                                            grid.HScrollBar.Value = ReverseHScrollValueRTL(hScrollPixelPos);
                                        }

                                        grid.Update();

                                        success = true;
                                        horz = true;
                                    }
                                }
                            }
                        }

                        break;

                    case GridDirectionType.Up:
                        {
                            int vScrollPixelPos = grid.GetCurrentVScrollPixelPos();
                            ////    Return false; // not implemented yet.
                            int yAmount = amount;
                            int minVScrollPixelPos = grid.GetVScrollPixelMinimum();
                            vScrollPixelPos = Math.Max(minVScrollPixelPos, vScrollPixelPos - yAmount);

                            ////SS                grid.ViewLayout.Reset();
                            scrollAreaBounds = grid.ViewLayout.VscrollAreaBounds;
                            clipBounds = scrollAreaBounds;

                            if (yAmount != 0)
                            {
                                //// Give the programmer the possibility to inhibit scrolling.
                                if (grid.RaiseVScrollPixelPosChanging(vScrollPixelPos))
                                {
                                    ////SS
                                    scrollAreaBounds.Y -= yAmount - 1;
                                    bool noScroll = scrollAreaBounds.Top >= scrollAreaBounds.Bottom || scrollAreaBounds.Height < clipBounds.Height / 4;

                                    int pixelDelta;
                                    int topRowIndex;
                                    grid.VScrollPixelPosToRowIndex(vScrollPixelPos, out topRowIndex, out pixelDelta);

                                    if (grid.NotifyTopRowChanging(topRowIndex))
                                    {
                                        ////                                    currentCellBounds.Offset(0, yAmount);
                                        //// Take away current cell focus if current cell is scrolled out of view.
                                        ////SS
                                        if (cellRenderer != null
                                            && cellRenderer.HasFocusControl
                                            && !grid.InternalIsFrozenRow(currentCellRowIndex)
                                            && (!scrollAreaBounds.Contains(currentCellBounds)
                                            || currentCellBounds.IsEmpty))        
                                        {
                                            cellRenderer.SetHasFocusControl(false);
                                            if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                            {
                                                grid.Focus();
                                                if (!noScroll)
                                                {
                                                    grid.Update();
                                                }
                                            }

                                            currentCellBounds = Rectangle.Empty;
                                        }

                                        this.m_nTopRow = topRowIndex;
                                        grid.vScrollPixelDelta = pixelDelta;
                                        grid.ViewLayout.Reset();

                                        //// Scroll view or simply invalidate; this is more efficient.
                                        if (noScroll)
                                        {
                                            grid.InternalInvalidate(clipBounds);
                                        }
                                        else
                                        {
                                            grid.ScrollWindow(0, yAmount, clipBounds, clipBounds, true);
                                        }

                                        //// Update scrollbar value (and related splitter panes in a SplitterControl).
                                        if (!this.m_bUpdateScrollbar)
                                        {
                                            grid.VScrollBar.Value = vScrollPixelPos;
                                        }

                                        grid.Update();

                                        success = true;
                                        vert = true;
                                    }
                                }
                                ////SS
                                scrollAreaBounds = grid.ViewLayout.VscrollAreaBounds;
                            }
                        }

                        break;

                    case GridDirectionType.Down:
                        {
                            int vScrollPixelPos = grid.GetCurrentVScrollPixelPos();
                            ////Return false; // not implemented yet.
                            int yAmount = amount;

                            scrollAreaBounds = grid.ViewLayout.VscrollAreaBounds;
                            clipBounds = scrollAreaBounds;

                            // Now it is safe to use Model.RowCount.
                            int maxVScrollPixelPos = grid.GetVScrollPixelHeight(); //// - grid.ViewLayout.VscrollAreaBounds.Height;
                            vScrollPixelPos = Math.Min(maxVScrollPixelPos, vScrollPixelPos + yAmount);

                            if (maxVScrollPixelPos > vScrollPixelPos)
                            {
                                //// Give the programmer the possibility to inhibit scrolling.
                                if (grid.RaiseVScrollPixelPosChanging(vScrollPixelPos))
                                {
                                    ////SS
                                    scrollAreaBounds.Y += yAmount;
                                    ////scrollAreaBounds.Height -= yAmount;

                                    bool noScroll = scrollAreaBounds.Top >= scrollAreaBounds.Bottom || scrollAreaBounds.Height < clipBounds.Height / 4;

                                    int pixelDelta;
                                    int topRowIndex;
                                    grid.VScrollPixelPosToRowIndex(vScrollPixelPos, out topRowIndex, out pixelDelta);

                                    if (grid.NotifyTopRowChanging(topRowIndex))
                                    {
                                        ////currentCellBounds.Offset(0, -yAmount);
                                        //// Take away current cell focus if current cell is scrolled out of view.
                                        ////TraceUtil.TraceCurrentMethodInfo(scrollAreaBounds, yAmount, currentCellBounds);
                                        if (cellRenderer != null
                                            && cellRenderer.HasFocusControl
                                            && !grid.InternalIsFrozenRow(currentCellRowIndex)
                                            && (!scrollAreaBounds.Contains(currentCellBounds)
                                            || currentCellBounds.IsEmpty))
                                        {
                                            cellRenderer.SetHasFocusControl(false);
                                            if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                            {
                                                grid.Focus();
                                                if (!noScroll)
                                                {
                                                    grid.Update();
                                                }
                                            }

                                            currentCellBounds = Rectangle.Empty;
                                        }

                                        this.m_nTopRow = topRowIndex;
                                        grid.vScrollPixelDelta = pixelDelta;
                                        grid.ViewLayout.Reset();

                                        //// Scroll view or simply invalidate; this is more efficient.
                                        if (noScroll)
                                        {
                                            grid.InternalInvalidate(clipBounds);
                                        }
                                        else
                                        {
                                            ////TraceUtil.TraceCurrentMethodInfo(0, -yAmount, clipBounds, clipBounds, true);
                                            grid.ScrollWindow(0, -yAmount, clipBounds, clipBounds, true);
                                        }

                                        //// Update scrollbar value (and related splitter panes in a SplitterControl).
                                        if (!this.m_bUpdateScrollbar)
                                        {
                                            grid.VScrollBar.Value = vScrollPixelPos;
                                        }

                                        grid.Update();

                                        success = true;
                                        vert = true;
                                    }
                                }
                                ////SS
                                scrollAreaBounds = grid.ViewLayout.VscrollAreaBounds;
                            }
                        }

                        break;

                    default:
                        break;
                }

                //// Show current cell focus when current cell scrolls back into view.
                if (grid.HasControlFocus && cellRenderer != null)
                {
                    grid.GetScrollOutOfViewCurrentCellState(out cellRenderer, out cellControl, out currentCellBounds, out currentCellRowIndex, out currentCellColIndex, direction);

                    ////SS TraceUtil.TraceCurrentMethodInfo(scrollAreaBounds, currentCellBounds);
                    if (cellRenderer != null && cellRenderer.Grid.CurrentCell.IsEditing && !cellRenderer.HasFocusControl)
                    {
                        ////GetCurrentCellState(out cellRenderer, out cellControl, out currentCellBounds, out currentCellRowIndex, out currentCellColIndex).
                        if (((vert && !grid.InternalIsFrozenRow(currentCellRowIndex))
                            || (horz && !grid.InternalIsFrozenCol(currentCellColIndex)))
                            && scrollAreaBounds.Contains(currentCellBounds))
                        {
                            cellRenderer.SetHasFocusControl(true);
                        }
                    }
                }

                grid.Update();
            }
            finally
            {
                m_bInDoScroll = false;

                //// Let the programmer know that grid has scrolled.
                if (vert)
                {
                    grid.NotifyTopRowChanged(success);
                }
                else if (horz)
                {
                    grid.NotifyLeftColChanged(success);
                }

                if (vert)
                {
                    grid.RaiseVScrollPixelPosChanged(success);
                }
                else
                {
                    grid.RaiseHScrollPixelPosChanged(success);
                }
            }

            return success;   //// no Scrolling: false
        }

        internal int ReverseHScrollValueRTL(int value)
        {
            if (grid.IsRightToLeft() && (grid.HScrollBar.InnerScrollBar == null || grid.HScrollBar.InnerScrollBar.RightToLeft != RightToLeft.Yes))
            {
                return grid.HScrollBar.Maximum - grid.HScrollBar.LargeChange + 1 + grid.HScrollBar.Minimum - value;
            }

            return value;
        }

        public bool DoScroll(GridDirectionType direction, int nCell)
        {
            if (this.m_bInDoScroll)
            {
                return false;
            }

            ////if (grid.hPixelScroll && direction == GridDirectionType.Right)
            ////    return this.DoPixelScroll(direction, nCell);

            bool success = false;
            int wCell = nCell;
            bool vert = false;
            bool horz = false;
            int nLeftCol = grid.LeftColIndex;
            int nTopRow = grid.TopRowIndex;
            Rectangle rectMove = Rectangle.Empty;
            Rectangle rectClip = Rectangle.Empty;
            Rectangle currentCellBounds = Rectangle.Empty;

            // Current cell.
            int nEditRow, nEditCol;
            GridCellRendererBase cellRenderer = null;
            Control cellControl = null;
#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.GetCurrentCell"))
#endif
            {
                if (grid.CurrentCell.GetCurrentCell(out nEditRow, out nEditCol))
                {
                    cellRenderer = grid.GetCellRenderer(nEditRow, nEditCol);
                    GridCurrentCell ncc = cellRenderer.GetNestedCurrentCell();
                    if (ncc == null)
                    {
                        cellRenderer = null;
                    }
                    else if (ncc.Renderer != null)
                    {
                        cellRenderer = ncc.Renderer;
                        currentCellBounds = GetCurrentCellBoundsCore(cellRenderer, nEditRow, nEditCol);
                        cellControl = cellRenderer.Control;
                    }
                }
            }

            m_bInDoScroll = true;
            try
            {
                switch (direction)
                {
                    case GridDirectionType.Left:
                        {
                            int x = 0;
                            int nLeftClient = grid.GetClientCol(nLeftCol);
                            int nPos;
                            if (GridControlBase.UseOldHiddenScrollLogic)
                            {
                                nPos = Math.Max(nLeftCol, nLeftClient + wCell) - wCell;  // nPos > 1
                            }
                            else
                            {
                                nPos = grid.ScrollGrid.ScrollPositionToColIndex(Math.Max(grid.ScrollGrid.ColIndexToScrollPosition(nLeftCol), nLeftClient + wCell) - wCell);  //// nPos > 1
                            }

                            int nnfc = grid.GetFirstScrollableCol();

                            rectMove = grid.ViewLayout.RectangleRightOfCol(nnfc, GridCellSizeKind.VisibleSize);
                            rectClip = rectMove;

                            if (nLeftCol != nPos)
                            {
                                //// || nLeftCol > nfc+1 && grid.GetColWidth(nLeftCol-1) == 0))
                                while (nLeftCol > nPos) 
                                {
                                    if (x > grid.GridBounds.Width)
                                    {
                                        nLeftCol = Math.Max(nnfc, this.GetPrevColIndex(nPos + 1));
                                        break;
                                    }

                                    if (!this.GetPrevColIndex(ref nLeftCol))
                                    {
                                        break;
                                    }

                                    if (nLeftCol >= 0)
                                    {
                                        x += grid.GetColWidth(nLeftCol);
                                    }
                                }

                                nLeftCol = Math.Max(1, Math.Min(nLeftCol, nPos));

                                // Give the programmer the possibility to inhibit scrolling.
                                if (grid.NotifyLeftColChanging(nLeftCol))
                                {
                                    if (grid.IsRightToLeft())
                                    {
                                        GridUtil.OffsetLeft(ref rectMove, x - 1);
                                    }
                                    else
                                    {
                                        rectMove.Width -= x - 1;
                                    }

                                    bool noScroll = rectMove.Left >= rectMove.Right || rectMove.Width < rectClip.Width / 4;

                                    // Take away current cell focus if current cell is scrolled out of view.
                                    if (cellRenderer != null
                                        && cellRenderer.HasFocusControl
                                        && !grid.InternalIsFrozenCol(nEditCol)
                                        && !rectMove.Contains(currentCellBounds))
                                    {
                                        cellRenderer.SetHasFocusControl(false);
                                        if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                        {
                                            grid.Focus();
                                            if (!noScroll)
                                            {
                                                grid.Update();
                                            }
                                        }
                                    }

                                    this.m_nLeftCol = nLeftCol;
                                    x += grid.hScrollPixelDelta;
                                    grid.hScrollPixelDelta = 0;
                                    grid.ViewLayout.Reset();

                                    // Scroll view or simply invalidate; this is more efficient.
                                    if (noScroll)
                                    {
                                        grid.InternalInvalidate(rectClip);
                                    }
                                    else
                                    {
                                        if (grid.IsRightToLeft())
                                        {
                                            grid.ScrollWindow(-x, 0, rectClip, rectClip, true);
                                        }
                                        else
                                        {
                                            grid.ScrollWindow(x, 0, rectClip, rectClip, true);
                                        }
                                    }

                                    if ((this.hScrollSetting & GridScrollbarMode.Automatic) == GridScrollbarMode.Automatic
                                        && grid.LeftColIndex == nnfc)
                                    {
                                        grid.Update();
                                        UpdateScrollbars();
                                    }

                                    // Update scrollbar value (and related splitter panes in a SplitterControl).
                                    if (!this.m_bUpdateScrollbar)
                                    {
                                        if (grid.HScrollPixel)
                                        {
                                            grid.HScrollBar.Value = ReverseHScrollValueRTL(grid.GetCurrentHScrollPixelPos());
                                        }
                                        else
                                        {
                                            grid.HScrollBar.Value = ReverseHScrollValueRTL(this.ColIndexToScrollPosition(grid.LeftColIndex));
                                        }
                                    }

                                    grid.Update();

                                    success = true;
                                    horz = true;
                                }

                                // Let the programmer know that grid has scrolled.
                                //                            grid.NotifyLeftColChanged(success);
                            }
                        }

                        break;

                    case GridDirectionType.Right:
                        {
                            int x = 0;
                            int nScroll = 0;

                            // Check right col.
                            // Make sure that Model.ColCount will not estimate too big a value.
                            grid.Model.RaiseQueryMaximumRowCol(0, grid.Model.ColCount);

                            // Now it is safe to use Model.ColCount.
                            int nLastLeftCol = grid.GetMaximumPossibleLeftCol(out rectMove);
                            rectClip = rectMove;

                            int nCols = nLastLeftCol - nLeftCol;

                            wCell = Math.Min(nCols, wCell);
                            bool noScroll = false;
                            if (nCols > 0)
                            {
                                for (x = 0; nScroll < wCell; nScroll++)
                                {
                                    if (nLeftCol >= nLastLeftCol)
                                    {
                                        break;
                                    }

                                    if (x <= rectMove.Width)
                                    {
                                        x += grid.GetColWidth(nLeftCol);
                                    }

                                    if (GridControlBase.UseOldHiddenScrollLogic)
                                    {
                                        nLeftCol++;
                                    }
                                    else if (nLeftCol > visibleColumns.InternalCount && x > rectMove.Width)
                                    {
                                        nLeftCol += wCell - nScroll;
                                        noScroll = true;
                                        break;
                                    }
                                    else
                                    {
                                        if (!GetNextColIndex(ref nLeftCol))
                                        {
                                            break;
                                        }
                                    }
                                }

                                // Give the programmer the possibility to inhibit scrolling.
                                if (grid.NotifyLeftColChanging(nLeftCol))
                                {
                                    if (grid.IsRightToLeft())
                                    {
                                        rectMove.Width -= x;
                                    }
                                    else
                                    {
                                        GridUtil.OffsetLeft(ref rectMove, x);
                                    }

                                    noScroll |= rectMove.Left >= rectMove.Right || rectMove.Width < rectClip.Width / 4;

                                    // Take away current cell focus if current cell is scrolled out of view.
                                    if (cellRenderer != null
                                        && cellRenderer.HasFocusControl
                                        && !grid.InternalIsFrozenCol(nEditCol)
                                        && !rectMove.Contains(currentCellBounds))
                                    {
                                        cellRenderer.SetHasFocusControl(false);
                                        if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                        {
                                            grid.Focus();
                                            if (!noScroll)
                                            {
                                                grid.Update();
                                            }
                                        }
                                    }

                                    if (grid.HScrollPixel)
                                    {
                                        int currentHScrollPixelPos = grid.GetCurrentHScrollPixelPos();
                                        int hScrollPixelWidth = grid.GetHScrollPixelWidth();
                                        if (currentHScrollPixelPos + x >= hScrollPixelWidth - grid.ViewLayout.ScrollAreaBounds.Width)
                                        {
                                            int pixelDelta;
                                            grid.HScrollPixelPosToColIndex(hScrollPixelWidth - grid.ViewLayout.ScrollAreaBounds.Width, out nLeftCol, out pixelDelta);
                                            grid.hScrollPixelDelta = pixelDelta;
                                            x = hScrollPixelWidth - grid.ViewLayout.ScrollAreaBounds.Width - currentHScrollPixelPos;
                                        }
                                        else
                                        {
                                            x -= grid.hScrollPixelDelta;
                                            grid.hScrollPixelDelta = 0;
                                        }
                                    }
                                    else
                                    {
                                        x -= grid.hScrollPixelDelta;
                                        grid.hScrollPixelDelta = 0;
                                    }

                                    this.m_nLeftCol = nLeftCol;
                                    grid.ViewLayout.Reset();

                                    // Scroll view or simply invalidate; this is more efficient.
                                    if (noScroll)
                                    {
                                        grid.InternalInvalidate(rectClip);
                                    }
                                    else
                                    {
                                        if (grid.IsRightToLeft())
                                        {
                                            grid.ScrollWindow(x, 0, rectClip, rectClip, true);
                                        }
                                        else
                                        {
                                            grid.ScrollWindow(-x, 0, rectClip, rectClip, true);
                                        }
                                    }

                                    // Update scrollbar value (and related splitter panes in a SplitterControl).
                                    if (!this.m_bUpdateScrollbar)
                                    {
                                        if (grid.HScrollPixel)
                                        {
                                            grid.HScrollBar.Value = ReverseHScrollValueRTL(grid.GetCurrentHScrollPixelPos());
                                        }
                                        else
                                        {
                                            grid.HScrollBar.Value = ReverseHScrollValueRTL(this.ColIndexToScrollPosition(grid.LeftColIndex));
                                        }
                                    }

                                    grid.Update();

                                    success = true;
                                    horz = true;
                                }

                                //// Let the programmer know that grid has scrolled.
                                ////                            grid.NotifyLeftColChanged(success);
                            }
                        }

                        break;

                    case GridDirectionType.Up:
                        {
                            int y = 0;
                            int nTopClient = grid.GetClientRow(nTopRow);
                            int nPos;
                            if (GridControlBase.UseOldHiddenScrollLogic)
                            {
                                nPos = Math.Max(nTopRow, nTopClient + wCell) - wCell;   // nPos > 1
                            }
                            else
                            {
                                nPos = grid.ScrollGrid.ScrollPositionToRowIndex(Math.Max(grid.ScrollGrid.RowIndexToScrollPosition(nTopRow), nTopClient + wCell) - wCell);  //// nPos > 1
                            }

                            int nnfr = grid.GetFirstScrollableRow();

                            rectMove = grid.ViewLayout.RectangleBottomOfRow(nnfr, GridCellSizeKind.VisibleSize);
                            rectClip = rectMove;

                            if (nTopRow != nPos)
                            {
                                while (nTopRow > nPos)
                                {
                                    if (y > grid.GridBounds.Height)
                                    {
                                        nTopRow = Math.Max(nnfr, this.GetPrevRowIndex(nPos + 1));
                                        break;
                                    }

                                    if (!this.GetPrevRowIndex(ref nTopRow))
                                    {
                                        break;
                                    }

                                    if (nTopRow > 0)
                                    {
                                        y += grid.GetRowHeight(nTopRow);
                                    }
                                }

                                nTopRow = Math.Max(1, Math.Min(nTopRow, nPos));

                                if (grid.NotifyTopRowChanging(nTopRow))
                                {
                                    rectMove.Height -= y - 1;

                                    bool noScroll = rectMove.Top >= rectMove.Bottom || rectMove.Height < rectClip.Height / 4;

                                    //// Take away current cell focus if current cell is scrolled out of view.
                                    if (cellRenderer != null && cellRenderer.HasFocusControl && !grid.InternalIsFrozenRow(nEditRow)
                                        && !rectMove.Contains(currentCellBounds))
                                    {
                                        cellRenderer.SetHasFocusControl(false);
                                        if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                        {
                                            grid.Focus();
                                            if (!noScroll)
                                            {
                                                grid.Update();
                                            }
                                        }
                                    }

                                    this.m_nTopRow = nTopRow;
                                    y += grid.vScrollPixelDelta;
                                    grid.vScrollPixelDelta = 0;
                                    grid.ViewLayout.Reset();

                                    // Scroll view or simply invalidate; this is more efficient.
                                    if (noScroll)
                                    {
                                        grid.InternalInvalidate(rectClip);
                                    }
                                    else
                                    { 
                                        grid.ScrollWindow(0, y, rectClip, rectClip, true);
                                    }

                                    // Sometimes scrollbars need to be hidden when scrolled to top.
                                    if ((this.vScrollSetting & GridScrollbarMode.Automatic) == GridScrollbarMode.Automatic
                                        && grid.TopRowIndex == nnfr)
                                    {
                                        grid.Update();
                                        UpdateScrollbars();
                                    }

                                    // Update scrollbar value (and related splitter panes in a SplitterControl).
                                    if (!this.m_bUpdateScrollbar)
                                    {
                                        if (grid.VScrollPixel)
                                        {
                                            grid.VScrollBar.Value = grid.GetCurrentVScrollPixelPos();
                                        }
                                        else
                                        {
                                            grid.VScrollBar.Value = this.RowIndexToScrollPosition(grid.TopRowIndex);
                                        }
                                    }

                                    grid.Update();

                                    success = true;
                                    vert = true;
                                }

                                //// Let the programmer know that grid has scrolled.
                                ////                            grid.NotifyTopRowChanged(success);
                            }
                        }

                        break;

                    case GridDirectionType.Down:
#if MEASURE
                        using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.GridDirectionType.Down"))
#endif
                        {
                            int nScroll = 0;

                            //// Check bottom row.
                            //// Make sure that Model.RowCount will not estimate too big a value.
#if MEASURE
                        using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.RaiseQueryMaximumRowCol"))
#endif
                            {
                                grid.Model.RaiseQueryMaximumRowCol(grid.Model.RowCount, 0);
                            }
                            // Now it is safe to use grid.Model.RowCount.
                            int nLastTopRow;
#if MEASURE
                        using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.GetMaximumPossibleTopRow"))
#endif
                            {
                                nLastTopRow = grid.GetMaximumPossibleTopRow(out rectMove);
                            }

                            rectClip = rectMove;

                            int nRows = nLastTopRow - nTopRow;

                            wCell = Math.Min(nRows, wCell);

                            bool noScroll = false;
                            if (nRows > 0)
                            {
                                int y;
#if MEASURE
                                using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.GetRowHeight"))
#endif
                                {
                                    for (y = 0; nScroll < wCell; nScroll++)
                                    {
                                        if (nTopRow >= nLastTopRow)
                                        {
                                            break;
                                        }

                                        if (y <= rectMove.Height)
                                        {
                                            y += grid.GetRowHeight(nTopRow);
                                        }

                                        if (GridControlBase.UseOldHiddenScrollLogic)
                                        {
                                            nTopRow++;
                                        }
                                        else if (nTopRow > visibleRows.InternalCount && y > rectMove.Height)
                                        {
                                            nTopRow += wCell - nScroll;
                                            noScroll = true;
                                            break;
                                        }
                                        else
                                        {
                                            if (!GetNextRowIndex(ref nTopRow))
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }

                                //// Give the programmer the possibility to inhibit scrolling.
                                if (grid.NotifyTopRowChanging(nTopRow))
                                {
                                    rectMove.Y += y;
                                    rectMove.Height -= y;

                                    noScroll |= rectMove.Top >= rectMove.Bottom || rectMove.Height < rectClip.Height / 4;

#if MEASURE
                                    using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.Focus"))
#endif
                                    {
                                        // Take away current cell focus if current cell is scrolled out of view.
                                        if (cellRenderer != null
                                            && cellRenderer.HasFocusControl
                                            && !grid.InternalIsFrozenRow(nEditRow)
                                            && !rectMove.Contains(currentCellBounds))
                                        {
                                            cellRenderer.SetHasFocusControl(false);
                                            if (grid.IsActiveControl && cellControl != null && cellControl.ContainsFocus)
                                            {
                                                grid.Focus();
                                                if (!noScroll)
                                                {
                                                    grid.Update();
                                                }
                                            }
                                        }
                                    }

#if MEASURE
                                    using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.VScrollPixel"))
#endif
                                    {
                                        if (grid.VScrollPixel)
                                        {
                                            int currentVScrollPixelPos = grid.GetCurrentVScrollPixelPos();
                                            int vScrollPixelHeight = grid.GetVScrollPixelHeight();
                                            y = Math.Min(y, grid.VScrollBar.Maximum - currentVScrollPixelPos);
                                            if (currentVScrollPixelPos + y >= vScrollPixelHeight - grid.ViewLayout.ScrollAreaBounds.Height)
                                            {
                                                int pixelDelta;
                                                grid.VScrollPixelPosToRowIndex(vScrollPixelHeight - grid.ViewLayout.ScrollAreaBounds.Height, out nTopRow, out pixelDelta);
                                                grid.vScrollPixelDelta = pixelDelta;
                                                y = vScrollPixelHeight - grid.ViewLayout.ScrollAreaBounds.Height - currentVScrollPixelPos;
                                            }
                                            else
                                            {
                                                y -= grid.vScrollPixelDelta;
                                                grid.vScrollPixelDelta = 0;
                                            }
                                        }
                                        else
                                        {
                                            y -= grid.vScrollPixelDelta;
                                            grid.vScrollPixelDelta = 0;
                                        }
                                    }

                                    this.m_nTopRow = nTopRow;
                                    grid.ViewLayout.Reset();

#if MEASURE
                                    using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.ScrollWindow"))
#endif
                                    {
                                        // Scroll view or simply invalidate; this is more efficient.
                                        if (noScroll)
                                        {
                                            grid.InternalInvalidate(rectClip);
                                        }
                                        else
                                        {
                                            grid.ScrollWindow(0, -y, rectClip, rectClip, true);
                                        }
                                    }

                                    int scrollValue;
#if MEASURE
                                    using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.RowIndexToScrollPosition"))
#endif
                                    {
                                        // Update scrollbar value (and related splitter panes in a SplitterControl).
                                        if (grid.VScrollPixel)
                                        {
                                            scrollValue = grid.GetCurrentVScrollPixelPos();
                                        }
                                        else
                                        {
                                            scrollValue = this.RowIndexToScrollPosition(grid.TopRowIndex);
                                        }
                                    }

#if MEASURE
                                    using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.Value"))
#endif
                                    {
                                        if (scrollValue > grid.VScrollBar.Maximum || scrollValue < grid.VScrollBar.Minimum)
                                        {
                                            grid.UpdateScrollBars();
                                        }
                                        else if (!this.m_bUpdateScrollbar)
                                        {
                                            grid.VScrollBar.Value = scrollValue;
                                        }
                                    }
#if MEASURE
                                    using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.Update"))
#endif
                                    {
                                        grid.Update();
                                    }

                                    success = true;
                                    vert = true;
                                }
                                //// Let the programmer know that grid has scrolled.
                                ////                            grid.NotifyTopRowChanged(success);
                            }
                        }

                        break;
                }

                // Show current cell focus when current cell is scrolled back into view.
                if (grid.HasControlFocus && grid.CurrentCell.IsEditing && cellRenderer != null)
                {
                    if (!cellRenderer.HasFocusControl)
                    {
                        currentCellBounds = GetCurrentCellBoundsCore(cellRenderer, nEditRow, nEditCol);
                        if (((vert && !grid.InternalIsFrozenRow(nEditRow))
                            || (horz && !grid.InternalIsFrozenCol(nEditCol)))
                            && rectClip.Contains(currentCellBounds))
                        {
                            cellRenderer.SetHasFocusControl(true);
                        }
                    }
                }

                grid.Update();
            }
            finally
            {
                m_bInDoScroll = false;

                // Let the programmer know that grid has scrolled.
                if (vert)
                {
                    grid.NotifyTopRowChanged(success);
                }
                else if (horz)
                {
                    grid.NotifyLeftColChanged(success);
                }
            }

            return success;   //// no Scrolling: false
        }

        public void UpdateScrollbars()
        {
            UpdateScrollbars(true, false);
        }

        public void UpdateScrollbars(bool b)
        {
            UpdateScrollbars(b, false);
        }

        public void UpdateScrollbars(bool bRedraw, bool bOnlyIfDimensionChanged)
        {
            if (grid.Parent == null)
            {
                return;
            }

            if (this.vScrollSetting == GridScrollbarMode.DetectIfShared)
            {
                grid.splitterControl = (IDynamicSplitterFrame)GridUtil.GetParentControl(grid, typeof(IDynamicSplitterFrame));

                if (grid.splitterControl != null)
                {
                    this.hScrollSetting = GridScrollbarMode.Shared | GridScrollbarMode.AutoScroll;
                    this.vScrollSetting = GridScrollbarMode.Shared | GridScrollbarMode.AutoScroll;
                }
                else
                {
                    this.hScrollSetting = GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll;
                    this.vScrollSetting = GridScrollbarMode.Automatic | GridScrollbarMode.AutoScroll;
                }
            }
           //// !grid.m_bInitDone - OnInitialUpdate must have been called.
             ////   || m_bUpdateScrollbar - Avoid infinite recursion (UpdateScrollbars() will set m_bUpdateScrollbar.
            if (!grid.m_bInitDone || m_bUpdateScrollbar)  
            {
                return;
            }
 // no scrollbar update when drawing is locked
            if (grid.VScrollBar.Locked) 
            {
                grid.recalcScrollBars = ScrollBars.Both;
                return;
            }

            //// Are both scrollbars disabled?
            if (this.vScrollSetting == GridScrollbarMode.Disabled
                && this.hScrollSetting == GridScrollbarMode.Disabled)
            {
                return;
            }

            // Make sure that grid window is correctly visible.
            Rectangle rect = grid.ClientRectangle;
            ////            if (rect.Right <= rect.Left || rect.Bottom <= rect.Top)
            ////                return;
            if (rect.IsEmpty)
            {
                rect = new Rectangle(Point.Empty, new Size(10, 10));
            }

            // Determine row / column count only if the corresponding
            // scrollbar is not disabled.
            int nMaxRowScrollPos;
            if (grid.vPixelScroll)
            {
                nMaxRowScrollPos = grid.GetVScrollPixelHeight();
            }
            else
            {
                nMaxRowScrollPos = this.vScrollSetting == GridScrollbarMode.Disabled ? 0 : this.GetMaxRowScrollPosition(); ////grid.Model.RowCount;
            }

            int nMaxColScrollPos;
            if (grid.hPixelScroll)
            {
                nMaxColScrollPos = grid.GetHScrollPixelWidth(); //// - grid.ViewLayout.ScrollAreaBounds.Width;
            }
            else
            {
                nMaxColScrollPos = this.hScrollSetting == GridScrollbarMode.Disabled ? 0 : this.GetMaxColScrollPosition(); ////grid.Model.ColCount;
            }

            if (bOnlyIfDimensionChanged)
            {
                //// Return immediately if row / column colum count did not change
                //// since last call to UpdateScrollbars().
                if (nMaxRowScrollPos == m_nLastSBRowCount && nMaxColScrollPos == m_nLastSBColCount)
                {
                    return;
                }
            }

            int nFirstColScrollPos;
            int nFirstRowScrollPos;

#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.UpdateScrollbars.RowIndexToVScrollPixelPos"))
#endif
            {
                // Reset last visible row / col so that they will be recalculated again.
                if (grid.vPixelScroll)
                {
                    nFirstRowScrollPos = Math.Max(0, grid.RowIndexToVScrollPixelPos(this.grid.GetFirstScrollableRow()));
                }
                else
                {
                    nFirstRowScrollPos = Math.Max(0, this.RowIndexToScrollPosition(this.grid.GetFirstScrollableRow()));
                }

                if (grid.hPixelScroll)
                {
                    nFirstColScrollPos = Math.Max(0, grid.ColIndexToHScrollPixelPos(this.grid.GetFirstScrollableCol()));
                }
                else
                {
                    nFirstColScrollPos = Math.Max(0, ColIndexToScrollPosition(this.grid.GetFirstScrollableCol()));
                }
            }

            int nLastRowScrollPos /*=0*/, nLastColScrollPos = 0;

            // BLOCK: Calculate last possible top-row and left-column.
            rect = grid.GridBounds;

            // check right col
            nLastColScrollPos = nMaxColScrollPos;
#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.UpdateScrollbars.nLastColScrollPos"))
#endif
            {
                if (this.hScrollSetting != GridScrollbarMode.Disabled)
                {
                    if (grid.hPixelScroll)
                    {
                        nLastColScrollPos = grid.GetHScrollPixelWidth() - grid.ViewLayout.ScrollAreaBounds.Width;
                    }
                    else
                    {
                        int x = 0;
                        rect = grid.ViewLayout.RectangleRightOfCol(grid.LeftColIndex, GridCellSizeKind.ActualSize);
                        rect.Intersect(grid.GridBounds);
                        x = grid.GetColWidth(this.ScrollPositionToColIndex(nLastColScrollPos));
                        while (x <= rect.Width && nLastColScrollPos > nFirstColScrollPos)
                        {
                            x += grid.GetColWidth(this.ScrollPositionToColIndex(--nLastColScrollPos));
                        }

                        if (x > rect.Width && nLastColScrollPos < nMaxColScrollPos)
                        {
                            nLastColScrollPos++;
                        }
                    }
                }
            }

            // Check bottom Row.
            nLastRowScrollPos = nMaxRowScrollPos;
#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.UpdateScrollbars.nLastRowScrollPos"))
#endif
            {
                if (this.vScrollSetting != GridScrollbarMode.Disabled)
                {
                    if (grid.vPixelScroll)
                    {
                        nLastRowScrollPos = grid.GetVScrollPixelHeight() - grid.ViewLayout.ScrollAreaBounds.Height;
                    }
                    else
                    {
                        int y = 0;
                        rect = grid.ViewLayout.RectangleBottomOfRow(grid.TopRowIndex, GridCellSizeKind.ActualSize);
                        rect.Intersect(grid.GridBounds);
                        ////ViewLayout.GetClientRowRangeHeight(0, Math.Max(grid.GetClientRow(grid.TopRowIndex), 1)-1));
                        y = grid.GetRowHeight(this.ScrollPositionToRowIndex(nLastRowScrollPos));
                        while (y <= rect.Height && nLastRowScrollPos > nFirstRowScrollPos)
                        {
                            y += grid.GetRowHeight(this.ScrollPositionToRowIndex(--nLastRowScrollPos));
                        }

                        if (y > rect.Height && nLastRowScrollPos < nMaxRowScrollPos)
                        {
                            nLastRowScrollPos++;
                        }
                    }
                }
            }

            // Semaphor avoids infinite recursion.
            m_bUpdateScrollbar = true;

            // Automatic scrolling of the grid so that as many cells as
            // possibile are visible. This is useful if user enlarged the grid window, so we can scroll some
            // rows into view (GridScrollbarMode.AutoScroll flag must be set).

            // Vertical scrolling.
#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.UpdateScrollbars.SetTopRowIndex"))
#endif
            {
                if (grid.vPixelScroll)
                {
                    if ((this.vScrollSetting & GridScrollbarMode.DisableAutoScroll) != GridScrollbarMode.DisableAutoScroll)
                    {
                        if (grid.GetCurrentVScrollPixelPos() > grid.GetVScrollPixelHeight() - grid.ViewLayout.ScrollAreaBounds.Height)
                        {
                            grid.SetCurrentVScrollPixelPos(Math.Max(grid.GetVScrollPixelMinimum(), grid.GetVScrollPixelHeight() - grid.ViewLayout.ScrollAreaBounds.Height));
                        }
                    }
                }
                else
                {
                    if ((this.vScrollSetting & GridScrollbarMode.DisableAutoScroll) != GridScrollbarMode.DisableAutoScroll
                        && grid.TopRowIndex > this.ScrollPositionToRowIndex(nFirstRowScrollPos) && this.ScrollPositionToRowIndex(nLastRowScrollPos) < grid.TopRowIndex)
                    {
                        grid.TopRowIndex = this.ScrollPositionToRowIndex(nLastRowScrollPos);
                        grid.Update();
                    }
                    else if (nMaxRowScrollPos > 0 && grid.TopRowIndex > ScrollPositionToRowIndex(nMaxRowScrollPos))
                    {
                        grid.TopRowIndex = this.ScrollPositionToRowIndex(nLastRowScrollPos);
                        grid.Update();
                    }
                }
            }

#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.UpdateScrollbars.SetLeftColIndex"))
#endif
            {
                // Horizontal scrolling.
                if (grid.hPixelScroll)
                {
                    if ((this.hScrollSetting & GridScrollbarMode.DisableAutoScroll) != GridScrollbarMode.DisableAutoScroll)
                    {
                        ////TraceUtil.TraceCurrentMethodInfo(grid.GetCurrentHScrollPixelPos(), grid.GetHScrollPixelWidth(), grid.ViewLayout.ScrollAreaBounds.Width, grid.GetHScrollPixelWidth() - grid.ViewLayout.ScrollAreaBounds.Width);
                        if (grid.GetCurrentHScrollPixelPos() > grid.GetHScrollPixelWidth() - grid.ViewLayout.ScrollAreaBounds.Width)
                        {
                            grid.SetCurrentHScrollPixelPos(Math.Max(grid.GetHScrollPixelMinimum(), grid.GetHScrollPixelWidth() - grid.ViewLayout.ScrollAreaBounds.Width));
                        }
                    }
                }
                else
                {
                    if ((this.hScrollSetting & GridScrollbarMode.DisableAutoScroll) != GridScrollbarMode.DisableAutoScroll
                        && grid.LeftColIndex > this.ScrollPositionToColIndex(nFirstColScrollPos) && this.ScrollPositionToColIndex(nLastColScrollPos) < grid.LeftColIndex)
                    {
                        grid.LeftColIndex = this.ScrollPositionToColIndex(nLastColScrollPos);
                        grid.Update();
                    }
                    else if (nMaxColScrollPos > 0 && grid.LeftColIndex > ScrollPositionToColIndex(nMaxColScrollPos))
                    {
                        grid.LeftColIndex = this.ScrollPositionToColIndex(nLastColScrollPos);
                        grid.Update();
                    }
                }
            }

            int nRowScrollPos;
            if (grid.vPixelScroll)
            {
                nRowScrollPos = grid.GetCurrentVScrollPixelPos();
            }
            else
            {
                nRowScrollPos = this.RowIndexToScrollPosition(grid.TopRowIndex);
            }

            int nColScrollPos;
            if (grid.hPixelScroll)
            {
                nColScrollPos = grid.GetCurrentHScrollPixelPos();
            }
            else
            {
                nColScrollPos = this.ColIndexToScrollPosition(grid.LeftColIndex);
            }

            bool bHorz = false,
                bVert = false;

            //// Show or hide scrollbars and mark bVert or bHorz true
            //// if the corresponding scrollbar was hidden or shown.

            grid.SuspendLayout();
#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.UpdateScrollbars.scrollbar"))
#endif
            {
                // Vertical scrollbar.
                switch (this.vScrollSetting & (GridScrollbarMode.Enabled | GridScrollbarMode.Shared | GridScrollbarMode.Disabled | GridScrollbarMode.Automatic))
                {
                    case GridScrollbarMode.Automatic:
                        // enable scrollbar
                        if (grid.ViewLayout.ScrollAreaBounds.Height > 0 && nMaxRowScrollPos > 0 && (nRowScrollPos > nFirstRowScrollPos || nLastRowScrollPos > nFirstRowScrollPos)) 
                        {
                            bVert = !grid.VScroll;
                            grid.VScroll = true;
                        }
                        else 
                        {
                            // disable scrollbar
                            bVert = grid.VScroll;
                            grid.VScroll = false;
                        }

                        break;

                    case GridScrollbarMode.Enabled:
                        bVert = !grid.VScroll;
                        grid.VScroll = true;
                        break;

                    case GridScrollbarMode.Shared:
                        grid.VScroll = false;
                        break;

                    case GridScrollbarMode.Disabled:
                        bVert = grid.VScroll;
                        grid.VScroll = false;
                        break;
                }

                // Horizontal scrollbar.
                switch (this.hScrollSetting & (GridScrollbarMode.Enabled | GridScrollbarMode.Shared | GridScrollbarMode.Disabled | GridScrollbarMode.Automatic))
                {
                    case GridScrollbarMode.Automatic:
                        //// enable scrollbar
                        if (grid.ViewLayout.ScrollAreaBounds.Width > 0 && nMaxColScrollPos > 0 && (nColScrollPos > nFirstColScrollPos || nLastColScrollPos > nFirstColScrollPos))    
                        {
                            //// Set bHorz true, when scrollbar was disabled.
                            bHorz = !grid.HScroll;
                            grid.HScroll = true;
                        }
                        else
                        { 
                            //// disable scrollbar
                            //// Set bHorz true, when scrollbar was enabled.
                            bHorz = grid.HScroll;
                            grid.HScroll = false;
                        }

                        break;

                    case GridScrollbarMode.Enabled:
                        bHorz = !grid.HScroll;
                        grid.HScroll = true;
                        break;

                    case GridScrollbarMode.Shared:
                        grid.HScroll = false;
                        break;

                    case GridScrollbarMode.Disabled:
                        bHorz = grid.HScroll;
                        grid.HScroll = false;
                        break;
                }

                // Optimized updating of grid window when
                // only the scrollbar regions shall be redrawn
                // and other parts of the window shall not
                // be redrawn.
                grid.ResumeLayout(false);

                if (bHorz || bVert)
                {
                    grid.UpdateStyles();
                    grid.ViewLayout.Reset();
                    grid.PerformLayout();
                }
            }

#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.DoScroll.ScrollBar.Value"))
#endif
            {
                if (nMaxRowScrollPos > 0)
                {
                    bool vEnabled = nRowScrollPos > nFirstRowScrollPos || nLastRowScrollPos > nFirstRowScrollPos;
                    if (vEnabled)
                    {
                        grid.VScrollBar.Enabled = true;
                    }

                    grid.VScrollBar.Minimum = nFirstRowScrollPos;
                    grid.VScrollBar.Maximum = Math.Max(nMaxRowScrollPos, nFirstRowScrollPos);
                    grid.VScrollBar.LargeChange = Math.Max(0, nMaxRowScrollPos - nLastRowScrollPos + 1);
                    grid.VScrollBar.Value = Math.Min(Math.Max(nMaxRowScrollPos, nFirstRowScrollPos), Math.Max(nRowScrollPos, nFirstRowScrollPos));
                    if (!vEnabled)
                    {
                        grid.VScrollBar.Enabled = false;
                    }

                    // Update scrollbar positions.
                    if (nRowScrollPos < nFirstRowScrollPos)
                    {
                        if (grid.vPixelScroll)
                        {
                        }
                        else
                        {
                            nRowScrollPos = nFirstRowScrollPos;
                            grid.TopRowIndex = this.ScrollPositionToRowIndex(nFirstRowScrollPos);
                        }
                    }
                }
                else
                {
                    grid.VScrollBar.Enabled = false;
                }

                if (nMaxColScrollPos > 0)
                {
                    bool hEnabled = nColScrollPos > nFirstColScrollPos || nLastColScrollPos > nFirstColScrollPos;
                    ////                int test = this.ScrollPositionToColIndex(nFirstColScrollPos);
                    if (hEnabled)
                    {
                        grid.HScrollBar.Enabled = true;
                    }

                    grid.HScrollBar.Minimum = nFirstColScrollPos;
                    grid.HScrollBar.Maximum = Math.Max(nMaxColScrollPos, nFirstColScrollPos);
                    grid.HScrollBar.LargeChange = Math.Max(0, nMaxColScrollPos - nLastColScrollPos + 1);
                    grid.HScrollBar.Value = ReverseHScrollValueRTL(Math.Max(nColScrollPos, nFirstColScrollPos));
                    grid.HScrollBar.Value = ReverseHScrollValueRTL(Math.Min(Math.Max(nMaxColScrollPos, nFirstColScrollPos), Math.Max(nColScrollPos, nFirstColScrollPos)));
                    if (!hEnabled)
                    {
                        grid.HScrollBar.Enabled = false;
                    }

                    if (nColScrollPos < nFirstColScrollPos)
                    {
                        if (grid.hPixelScroll)
                        {
                        }
                        else
                        {
                            nColScrollPos = nFirstColScrollPos;
                            grid.LeftColIndex = this.ScrollPositionToColIndex(nFirstColScrollPos);
                        }
                    }
                }
                else
                {
                    grid.HScrollBar.Enabled = false;
                }
            }

#if MEASURE
            using (Syncfusion.Diagnostics.MeasureTime.Measure("ScrollGrid.UpdateScrollbars.RaiseScrollInfoChanged"))
#endif
            {
                grid.RaiseScrollInfoChanged();
            }
            //// Remember row count (see check for bOnlyIfDimensionChanged above).
            m_nLastSBRowCount = nMaxRowScrollPos;
            m_nLastSBColCount = nMaxColScrollPos;

            //// Reset semaphor.
            m_bUpdateScrollbar = false;
        }
    }

    /// <summary>
    /// Use this class if you have an array of rows or column elements 
    /// and you want to be able to hide a range of rows or columns. The
    /// class uses binary trees to map from original to visible position
    /// and vice versa.
    /// </summary>
    internal class GridScrollVisibleElementsMapper
    {
        TreeTableWithCounter rbTree;
        int count = 0;

        /// <summary>
        /// Constructs the class and initializes the internal tree.
        /// </summary>
        public GridScrollVisibleElementsMapper()
        {
            TreeTableVisibleCounter startPos = new TreeTableVisibleCounter(0);
            rbTree = new TreeTableWithCounter(startPos, false);
        }

        /// <summary>
        /// Gets or sets the raw number of rows or columns.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }

            set
            {
                count = value;
            }
        }

        /// <summary>
        /// Gets the actual visible number of rows or columns.
        /// </summary>
        public int VisibleCount
        {
            get
            {
                int delta = Count - InternalCount;
                return InternalVisibleCount + delta;
            }
        }

        /// <summary>
        /// Hides a specified range of rows or columns
        /// </summary>
        /// <param name="from">The raw index for the first element></param>
        /// <param name="to">The raw index for the last element</param>
        public void HideRange(int from, int to)
        {
            CheckRange("from", 0, Count - 1, from);
            CheckRange("to", 0, Count - 1, to);

            EnsureTreeCount(to + 1);

            for (int n = from; n <= to; n++)
            {
                TreeTableWithCounterEntry rbEntry = rbTree[n];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[n].GetCounterTotal();
                if (counter.GetVisibleCount() != 0)
                {
                    rbEntry.Value = new TreeTableVisibleCounterSource(0);
                    //Added for the Fix: SD3418
                    if (rbEntry.IsCounterDirty())
                        rbEntry.InvalidateCounterBottomUp(false);
                }
            }
        }

        /// <summary>
        /// Shows a specified range of rows or columns
        /// </summary>
        /// <param name="from">The raw index for the first element></param>
        /// <param name="to">The raw index for the last element</param>
        public void ShowRange(int from, int to)
        {
            CheckRange("from", 0, Count - 1, from);
            CheckRange("to", 0, Count - 1, to);

            EnsureTreeCount(to + 1);

            for (int n = from; n <= to; n++)
            {
                TreeTableWithCounterEntry rbEntry = rbTree[n];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[n].GetCounterTotal();
                if (counter.GetVisibleCount() != 1)
                {
                    rbEntry.Value = new TreeTableVisibleCounterSource(1);
                    rbEntry.InvalidateCounterBottomUp(false);
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the specified element is visible.
        /// </summary>
        /// <param name="pos">The raw index for the element</param>
        /// <returns>true if visible; false if hidden.</returns>
        public bool this[int pos]
        {
            get
            {
                CheckRange("pos", 0, Count - 1, pos);

                if (pos >= InternalCount)
                {
                    return true;
                }

                TreeTableWithCounterEntry rbEntry = rbTree[pos];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[pos].GetCounterTotal();
                return counter.GetVisibleCount() == 1;
            }

            set
            {
                CheckRange("pos", 0, Count - 1, pos);

                if (pos >= InternalCount)
                {
                    EnsureTreeCount(pos + 1);
                }

                double v = value ? 1 : 0;
                TreeTableWithCounterEntry rbEntry = rbTree[pos];
                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree[pos].GetCounterTotal();
                if (counter.GetVisibleCount() != v)
                {
                    rbEntry.Value = new TreeTableVisibleCounterSource(v);
                    rbEntry.InvalidateCounterBottomUp(false);
                }
            }
        }

        /// <summary>
        /// Gets the raw index for a visible position
        /// </summary>
        /// <param name="pos">The visible position of the element.</param>
        /// <returns>The raw index.</returns>
        public int GetOriginalPositionAtVisiblePosition(int pos)
        {
            CheckRange("pos", 0, Count - 1, pos);

            if (InternalCount == 0)
            {
                return pos;
            }

            int delta = 0;
            int visibleCount = InternalVisibleCount;
            if (pos >= visibleCount)
            {
                delta = pos - visibleCount + 1;
                pos = visibleCount;
            }

            TreeTableEntry entry = rbTree.GetEntryAtCounterPosition(new TreeTableVisibleCounter(pos), TreeTableCounterCookies.CountVisible, false);
            return rbTree.IndexOf(entry) + delta;
        }

        /// <summary>
        /// Gets the visible position for an element
        /// </summary>
        /// <param name="pos">The raw index of the element.</param>
        /// <returns>The visible position. If the element at the specified raw index is hidden, 
        /// the method returns the visible position of the previous element.</returns>
        public int GetVisiblePositionForOriginalPosition(int pos)
        {
            CheckRange("pos", 0, Count - 1, pos);

            int delta = 0;
            int count = InternalCount;
            if (count == 0)
            {
                return pos;
            }

            TreeTableVisibleCounter counter;
            if (pos >= count)
            {
                delta = pos - count;
                counter = (TreeTableVisibleCounter)rbTree.GetCounterTotal();
            }
            else
            {
                counter = (TreeTableVisibleCounter)rbTree[pos].GetCounterPosition();
            }

            return (int)counter.GetVisibleCount() + delta;
        }

        /// <summary>
        /// Checks whether the column at the specified raw index is hidden.
        /// </summary>
        /// <param name="pos">The raw index of the element.</param>
        /// <returns>true if hidden; false otherwise.</returns>
        public bool IsColumnHidden(int pos)
        {
            CheckRange("pos", 0, Count - 1, pos);

            return !this[pos];
        }

        void EnsureTreeCount(int count)
        {
            int treeCount = rbTree.GetCount();
            if (treeCount == 0)
            {
                rbTree.BeginInit();
                for (int n = 0; n < count; n++)
                {
                    TreeTableWithCounterEntry rbEntry = new TreeTableWithCounterEntry();
                    rbEntry.Value = new TreeTableVisibleCounterSource(1);
                    rbEntry.Tree = rbTree;
                    rbTree.Add(rbEntry);
                }

                rbTree.EndInit();
            }
            else if (treeCount < count)
            {
                for (int n = treeCount; n < count; n++)
                {
                    TreeTableWithCounterEntry rbEntry = new TreeTableWithCounterEntry();
                    rbEntry.Value = new TreeTableVisibleCounterSource(1);
                    rbEntry.Tree = rbTree;
                    rbTree.Add(rbEntry);
                }
            }
        }

        internal int InternalCount
        {
            get
            {
                return rbTree.GetCount();
            }

            set
            {
                if (value >= InternalCount)
                {
                    EnsureTreeCount(value);
                }
                else
                {
                    int n = InternalCount - value;
                    //// TODO:
                    ////while (n-- > 0)
                    ////    rbTree.RemoveAt(value + n);
                }
            }
        }

        int InternalVisibleCount
        {
            get
            {
                int treeCount = InternalCount;
                if (treeCount == 0)
                {
                    return 0;
                }

                TreeTableVisibleCounter counter = (TreeTableVisibleCounter)rbTree.GetCounterTotal();
                return (int)counter.GetVisibleCount();
            }
        }

        void CheckRange(string paramName, int from, int to, int actualValue)
        {
            ////if (actualValue < from || actualValue > to)
            ////    throw new ArgumentOutOfRangeException(paramName, actualValue, "out of range " + from.ToString() + " to " + to.ToString());
        }
    }
}