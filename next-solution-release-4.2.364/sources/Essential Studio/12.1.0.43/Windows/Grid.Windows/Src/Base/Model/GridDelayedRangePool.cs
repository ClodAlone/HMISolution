//-------------------------------------------------------------------------------------------------
// <copyright file="GridDelayedRangePool.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;

namespace Syncfusion.Windows.Forms.Grid
{
    [Syncfusion.Documentation.DocumentationExclude()]
    [Serializable]
    internal class GridDelayedRange : ICloneable
    {
        internal GridRangeInfo rangeInfo;

        // Done row intervals.
        internal int[] lastRows;
        internal int[] firstRows;
        internal int length = 0;
        internal int count = 0;

        internal bool isResolved;

        public object Clone()
        {
            GridDelayedRange rg = new GridDelayedRange();
            rg.rangeInfo = rangeInfo;
            if (firstRows != null)
            {
                rg.firstRows = (int[])firstRows.Clone();
                rg.lastRows = (int[])lastRows.Clone();
            }

            return rg;
        }

        void EnsureRowCapacity(int count)
        {
            int n = (((count - 1) / 16) + 1) * 16;
            if (firstRows == null || lastRows == null)
            {
                firstRows = new int[n];
                lastRows = new int[n];
            }
            else if (firstRows.Length < count)
            {
                int[] items = new int[n];
                firstRows.CopyTo(items, 0);
                firstRows = items;
                items = new int[n];
                lastRows.CopyTo(items, 0);
                lastRows = items;
            }

            length = firstRows.Length;
        }

        void InsertAt(int index, int firstRow, int lastRow)
        {
            EnsureRowCapacity(count + 1);
            for (int n = count - 1; n > index; n--)
            {
                lastRows[n] = lastRows[n - 1];
                firstRows[n] = firstRows[n - 1];
            }

            lastRows[index] = lastRow;
            firstRows[index] = firstRow;
            count++;
        }

        void Append(int firstRow, int lastRow)
        {
            EnsureRowCapacity(count + 1);
            lastRows[count] = lastRow;
            firstRows[count] = firstRow;
            count++;
        }

        void RemoveAt(int index)
        {
            if (index < count)
            {
                for (int n = index + 1; n < length; n++)
                {
                    lastRows[n - 1] = lastRows[n];
                    firstRows[n - 1] = firstRows[n];
                }

                lastRows[length - 1] = 0;
                firstRows[length - 1] = 0;
                count--;
            }
        }

        public bool ResolveRange(GridRangeInfo rg, out int top, out int bottom)
        {
            bool bFound = false;

            top = rg.Top;
            bottom = rg.Bottom;

            int n = 0;
            for (; !bFound && n < count; n++)
            {
                if (lastRows[n] + 1 < rg.Top)
                {
                    continue;
                }

                if (firstRows[n] > rg.Bottom + 1)
                {
                    break;
                }

                if (firstRows[n] <= rg.Top && lastRows[n] >= rg.Bottom)
                {
                    // Interval is already found.
                    return false;
                }
                else if (firstRows[n] <= rg.Top && lastRows[n] <= rg.Bottom)
                {
                    bFound = true;
                    top = lastRows[n] + 1;
                    lastRows[n] = rg.Bottom;
                }
                else if (firstRows[n] >= rg.Top && lastRows[n] >= rg.Bottom)
                {
                    bFound = true;
                    bottom = firstRows[n] - 1;
                    firstRows[n] = rg.Top;
                }
                else if (firstRows[n] >= rg.Top && lastRows[n] <= rg.Bottom)
                {
                    bFound = true;
                    firstRows[n] = rg.Top;
                    lastRows[n] = rg.Bottom;
                }
            }

            if (!bFound)
            {
                try
                {
                    if (n < count)
                    {
                        InsertAt(n, top, bottom);
                    }
                    else
                    {
                        Append(top, bottom);
                    }
                }
                catch (OutOfMemoryException)
                {
                    // Free some memory.
                    Clear();
                }
            }

            for (n = 0; n + 1 < count; n++)
            {
                if (lastRows[n] >= firstRows[n + 1])
                {
                    lastRows[n] = lastRows[n + 1];

                    RemoveAt(n + 1);
                }
            }

            if (count == 1
                && firstRows[0] == rangeInfo.Top
                && lastRows[0] >= rangeInfo.Bottom)
            {
#if DEBUG
                Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format("Resolved Range: ({0})", rangeInfo));
#endif

                Clear();
            }

            return true;
        }

        public void Clear()
        {
            lastRows = null;
            firstRows = null;
            length = 0;
            count = 0;
            isResolved = true;
        }
    }

    [Syncfusion.Documentation.DocumentationExclude()]
    [Serializable]
    internal class GridDelayedRangePool : ICloneable
    {
        ArrayList delayedRanges = new ArrayList();

        public GridDelayedRangePool()
        {
        }

        public void Clear()
        {
            delayedRanges.Clear();
        }

        public void CleanUp()
        {
            ArrayList items = new ArrayList(delayedRanges.Capacity);
            foreach (GridDelayedRange range in delayedRanges)
            {
                if (!range.isResolved)
                {
                    items.Add(range);
                }
            }
        }

        public void DelayRange(GridRangeInfo range)
        {
            GridDelayedRange prgDelayed = null;
            int nSize = delayedRanges.Count;
            int nLastResolved = -1;

            for (int i = 0; i < nSize; i++)
            {
                prgDelayed = (GridDelayedRange)delayedRanges[i];

                if (prgDelayed.isResolved)
                {
                    nLastResolved = i;
                    continue;
                }

                int right = prgDelayed.rangeInfo.Right;
                int bottom = prgDelayed.rangeInfo.Bottom;

                // Mark those ranges as resolved which are only a subset of the new range
                // to be added.
                if (range.Contains(prgDelayed.rangeInfo))
                {
#if DEBUG
                    Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format("Delete Range: ({0})", prgDelayed.rangeInfo));
#endif
                    prgDelayed.Clear();
                    if (nLastResolved == -1)
                    {
                        nLastResolved = i;
                    }
                }                   
                else if (prgDelayed.count == 0)
                {
                    //// Check if we can extend an existing range.
                    GridRangeInfo r = prgDelayed.rangeInfo;
                    if (r.Right == range.Left - 1)
                    {
                        right = range.Right;
                    }

                    if (r.Bottom == range.Top - 1)
                    {
                        bottom = range.Bottom;
                    }

                    prgDelayed.rangeInfo = GridRangeInfo.Cells(r.Top, r.Left, bottom, right);
                    if (prgDelayed.rangeInfo.Contains(range))
                    {
                        return;
                    }
                }
            }

            // Add range to be evaluated.
            if (nLastResolved == -1)
            {
                try
                {
                    delayedRanges.Add(prgDelayed = new GridDelayedRange());
                }
                catch (OutOfMemoryException)
                {
                }
            }
            else
            {
                prgDelayed = (GridDelayedRange)delayedRanges[nLastResolved];
#if DEBUG
                Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format("Overwriting range: ({0})  ", prgDelayed.rangeInfo));
#endif
            }

            Debug.Assert(prgDelayed != null);
            prgDelayed.isResolved = false;
            prgDelayed.rangeInfo = range;
#if DEBUG
            Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format("Added Range: ({0})", prgDelayed.rangeInfo));
#endif
        }

        public void SplitDelayedRange(GridRangeInfo range)
        {
            int nCount = delayedRanges.Count;

            for (int i = 0; i < nCount; i++)
            {
                GridDelayedRange prgDelayed = (GridDelayedRange)delayedRanges[i];

                if (prgDelayed != null && !prgDelayed.isResolved)
                {
                    GridRangeInfo rgIntersect = GridRangeInfo.IntersectRange(prgDelayed.rangeInfo, range);

                    if (!rgIntersect.IsEmpty)
                    {
                        if (prgDelayed.rangeInfo.Left < rgIntersect.Left)
                        {
                            GridDelayedRange prgLeftDelayed = null;
                            GridRangeInfo r = prgDelayed.rangeInfo;
                            try
                            {
                                prgLeftDelayed = (GridDelayedRange)prgDelayed.Clone();

                                prgLeftDelayed.rangeInfo = GridRangeInfo.Cells(r.Top, r.Left, r.Bottom, rgIntersect.Left - 1);
                                prgDelayed.rangeInfo = GridRangeInfo.Cells(r.Top, rgIntersect.Left, r.Bottom, r.Right);
                                delayedRanges.Add(prgLeftDelayed);
                            }
                            catch (OutOfMemoryException)
                            {
                            }

#if DEBUG
                            Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format("Split left Range: left {0} / )", prgLeftDelayed.rangeInfo));
                            Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format(" right ({0})", prgDelayed.rangeInfo));
#endif
                        }

                        if (prgDelayed.rangeInfo.Right > rgIntersect.Right)
                        {
                            GridDelayedRange prgRightDelayed = null;
                            GridRangeInfo r = prgDelayed.rangeInfo;
                            try
                            {
                                prgRightDelayed = (GridDelayedRange)prgDelayed.Clone();
                                prgRightDelayed.rangeInfo = GridRangeInfo.Cells(r.Top, rgIntersect.Right + 1, r.Bottom, r.Right);
                                prgDelayed.rangeInfo = GridRangeInfo.Cells(r.Top, r.Left, r.Bottom, rgIntersect.Right);
                                delayedRanges.Add(prgRightDelayed);
                            }
                            catch (OutOfMemoryException)
                            {
                            }

#if DEBUG
                            Trace.WriteIf(Switches.DelayedRange.TraceVerbose, String.Format("Split right Range: left {0} / )", prgDelayed.rangeInfo));
                            Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format(" right ({0})", prgRightDelayed.rangeInfo));
#endif
                        }
                    }
                }
            }
        }

        public bool EvalRows(GridRangeInfo range, out int[] dwColStart, out int[] dwColEnd)
        {
            bool bFound = false;

            dwColStart = new int[0];
            dwColEnd = new int[0];
            if (range.Bottom < range.Top || range.Right < range.Left)
            {
                return false;
            }

            try
            {
                dwColStart = new int[range.Height];
                dwColEnd = new int[range.Height];
                for (int n = 0; n < dwColStart.Length; n++)
                {
                    dwColEnd[n] = -1;
                    dwColStart[n] = int.MaxValue;
                }
            }
            catch (OutOfMemoryException)
            {
                return false;
            }

            // Split ranges which are a superset of range.
            SplitDelayedRange(range);

            // Check each range and assign column ids to dwColStart[i] and dwColEnd[i].
            int nCount = delayedRanges.Count;
            for (int i = 0; i < nCount; i++)
            {
                GridDelayedRange prgDelayed = (GridDelayedRange)delayedRanges[i];

                if (!prgDelayed.isResolved)
                {
                    GridRangeInfo rg = GridRangeInfo.IntersectRange(prgDelayed.rangeInfo, range);
                    int top, bottom;
                    if (!rg.IsEmpty)
                    {
                        if (prgDelayed.ResolveRange(rg, out top, out bottom))
                        {
                            int dwFirstRow = Math.Max(top, prgDelayed.rangeInfo.Top);
                            int dwLastRow = Math.Min(bottom, prgDelayed.rangeInfo.Bottom);
                            int nFirstCol = prgDelayed.rangeInfo.Left;
                            int nLastCol = prgDelayed.rangeInfo.Right;

                            ////Trace.WriteLineIf(Switches.DelayedRange.TraceVerbose, String.Format("Row1 = {0}, nRow2 = {1}", nIndexFirstRow, nIndexLastRow));

                            int nFirstRow = (int)(dwFirstRow - range.Top);
                            int nLastRow = (int)(dwLastRow - range.Top);

                            for (int nIndex = nFirstRow; nIndex <= nLastRow; nIndex++)
                            {
                                if (dwColStart[nIndex] == -1)
                                {
                                    dwColStart[nIndex] = nFirstCol;
                                    dwColEnd[nIndex] = nLastCol;
                                }
                                else
                                {
                                    dwColStart[nIndex] = Math.Min(dwColStart[nIndex], nFirstCol);
                                    dwColEnd[nIndex] = Math.Max(dwColEnd[nIndex], nLastCol);
                                }
                            }

                            bFound = true;
                        }
                    }
                }
            }

            if (bFound)
            {
                CleanUp();  // Delete resolved ranges.
            }

            return bFound;
        }

        public void SetRowCount(int nMaxRow)
        {
            int nCount = delayedRanges.Count;
            for (int i = 0; i < nCount; i++)
            {
                GridDelayedRange prgDelayed = (GridDelayedRange)delayedRanges[i];
                GridRangeInfo r = prgDelayed.rangeInfo;

                if (r.Bottom > nMaxRow)
                {
                    prgDelayed.rangeInfo = GridRangeInfo.Cells(Math.Min(r.Top, nMaxRow), r.Left, Math.Min(r.Bottom, nMaxRow), r.Right);
                }
            }
        }

        public void SetColCount(int nMaxCol)
        {
            int nCount = delayedRanges.Count;
            for (int i = 0; i < nCount; i++)
            {
                GridDelayedRange prgDelayed = (GridDelayedRange)delayedRanges[i];
                GridRangeInfo r = prgDelayed.rangeInfo;

                if (r.Right > nMaxCol)
                {
                    prgDelayed.rangeInfo = GridRangeInfo.Cells(r.Top, Math.Min(r.Left, nMaxCol), r.Bottom, Math.Min(r.Right, nMaxCol));
                }
            }
        }

        public object Clone()
        {
            CleanUp();

            GridDelayedRangePool pool = new GridDelayedRangePool();

            int nCount = delayedRanges.Count;
            pool.delayedRanges = new ArrayList(delayedRanges.Capacity);

            for (int i = 0; i < nCount; i++)
            {
                pool.delayedRanges.Add(((GridDelayedRange)delayedRanges[i]).Clone());
            }

            return pool;
        }
    }
}
