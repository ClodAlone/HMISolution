#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Diagnostics;
using System.Collections;

#if !WinRT
namespace Syncfusion.Windows.Controls.Grid
#else
namespace Syncfusion.WinRT.Controls.Grid
#endif
{
    /// <summary>
    /// Manages selected ranges in the grid. Allows you to add and remove selections, determine
    /// selection state of a specific cell, and more.
    /// </summary>
#if WinRT
    [ClassReference(IsReviewed = false)]
#endif
    public class GridModelSelections : IEnumerable
    {
        // Constructor
        GridModel model;

        public GridModel Model
        {
            get { return model; }
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelSelections"/> object and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelSelections(GridModel model)
        {
            this.model = model;
        }

        /// <summary>
        /// The collection with all selected ranges.
        /// </summary>
        public GridRangeInfoList Ranges
        {
            get
            {
                return model.SelectedRanges;
            }
        }

        // Methods.
        /// <summary>
        /// Retrieves a list with selected ranges or if there are no selected ranges, returns the current cell as selected range.
        /// </summary>
        /// <param name="ranges">A <see cref="GridRangeInfoList"/> where selected ranges will be copied to.</param>
        /// <param name="considerCurrentCell">True if current cell should be returned as selected range if there are no other selected ranges.</param>
        /// <returns>True if one or multiple ranges or current cell range could be returned; False otherwise.</returns>
        public bool GetSelectedRanges(out GridRangeInfoList ranges, bool considerCurrentCell)
        {
            ranges = new GridRangeInfoList();
            foreach (GridRangeInfo range in model.SelectedRanges)
            {
                if (!ranges.AnyRangeContains(range))
                    ranges.Add(range);
            }
            if (ranges.Count == 0 && considerCurrentCell)
            {
                GridModelCurrentCellState cci = model.CurrentCellState;
                if (!cci.IsEmpty)
                    ranges.Add(GridRangeInfo.Cell(cci.RowIndex, cci.ColumnIndex));
            }
            return ranges.Count > 0;
        }

        /// <summary>
        /// Determines if the specified cell position is found in a range list.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="rl">The range list with ranges to be searched.</param>
        /// <returns>True if cell position was found; False otherwise.</returns>
        public bool GetInvertStateRowCol(int rowIndex, int colIndex, GridRangeInfoList rl)
        {
            return rl != null && rl.AnyRangeContains(GridRangeInfo.Cell(rowIndex, colIndex));
        }

        /// <summary>
        /// Returns selected columns in the grid.
        /// </summary>
        /// <param name="bRangeColsOnly">True if only selected columns should be returned; False if you want to treat single range cell selections as full column selections.</param>
        /// <param name="considerCurrentCell">True if current cell should be returned as selected range if there are no other selected ranges.</param>
        /// <returns>The <see cref="GridRangeInfoList"/> with column ranges.</returns>
        /// <remarks>
        /// If the user has selected the cell's range R1C2:R2C4 and the current cell is at R4C5.<para/>
        /// If you specify False for <paramref name="bRangeColsOnly"/>, the method will return range C2:C4. <para/>
        /// If you specify True for <paramref name="bRangeColsOnly"/> and also True for <paramref name="considerCurrentCell"/>,  
        /// the method will return range C5. <para/>
        /// </remarks>
        public GridRangeInfoList GetSelectedCols(bool bRangeColsOnly/* = false*/, bool considerCurrentCell /*= true*/)
        {
            GridRangeInfoList ranges = model.SelectedRanges.GetColRanges(bRangeColsOnly ? GridRangeInfoType.Cols : GridRangeInfoType.Cells);
            if (ranges.Count == 0 && considerCurrentCell)
            {
                GridModelCurrentCellState cci = model.CurrentCellState;
                if (!cci.IsEmpty)
                    ranges.Add(GridRangeInfo.Col(cci.ColumnIndex));
            }
            return ranges;
        }

        /// <summary>
        /// Returns selected rows in the grid.
        /// </summary>
        /// <param name="bRangeRowsOnly">True if only selected rows should be returned; False if you want to treat single range cell selections as full row selections.</param>
        /// <param name="considerCurrentCell">True if current cell should be returned as selected range if there are no other selected ranges.</param>
        /// <returns>The <see cref="GridRangeInfoList"/> with row ranges.</returns>
        /// <remarks>
        /// If the user has selected the cell's range R1C2:R2C4 and the current cell is at R4C5.<para/>
        /// If you specify False for <paramref name="bRangeColsOnly"/>, the method will return range R1:R2. <para/>
        /// If you specify True for <paramref name="bRangeColsOnly"/> and also True for <paramref name="considerCurrentCell"/>,
        /// the method will return range R4. <para/>
        /// </remarks>
        public GridRangeInfoList GetSelectedRows(bool bRangeRowsOnly/* = false*/, bool considerCurrentCell /*= true*/)
        {
            GridRangeInfoList ranges = model.SelectedRanges.GetRowRanges(bRangeRowsOnly ? GridRangeInfoType.Rows : GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            if (ranges.Count == 0 && considerCurrentCell)
            {
                GridModelCurrentCellState cci = model.CurrentCellState;
                if (!cci.IsEmpty)
                    ranges.Add(GridRangeInfo.Row(cci.RowIndex));
            }
            return ranges;
        }

        /// <summary>
        /// Adds a <see cref="GridRangeInfo"/> to the list of selected ranges.
        /// </summary>
        /// <param name="range">The new selection range to be added.</param>
        public void Add(GridRangeInfo range)
        {
            SelectRange(range, true);
        }

        /// <summary>
        /// Removes a <see cref="GridRangeInfo"/> from the list of selected ranges.
        /// </summary>
        /// <param name="range">The new selection range to be added.</param>
        public void Remove(GridRangeInfo range)
        {
            SelectRange(range, false);
        }

        /// <summary>
        /// Adds or removes a <see cref="GridRangeInfo"/> from the list of seletced ranges.
        /// </summary>
        /// <param name="range">The selection range to be added or removed.</param>
        /// <param name="bSelect">True if range should be added; False otherwise.</param>
        public void SelectRange(GridRangeInfo range, bool bSelect)
        {
            // Save old state.
            GridRangeInfoList oldRanges;
            GetSelectedRanges(out oldRanges, false);

            // Store new range.
            if (RaiseSelectionChanging(ref range, GridSelectionReason.SelectRange)
                && StoreSelectRange(range, bSelect))
            {
                //Model.FloatingCells.lockEvaluateFloatingCells = true;           // Don't evaluate int/*float*/ cell state
                RaiseSelectionChanged(range, oldRanges, GridSelectionReason.SelectRange);
                //Model.FloatingCells.lockEvaluateFloatingCells = false;
            }
        }

        void RaiseSelectionChanged(GridRangeInfo range, GridSelectionReason reason)
        {
            GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(range, null, reason);
            Model.RaiseSelectionChanged(e);
        }

        void RaiseSelectionChanged(GridRangeInfo range, GridRangeInfoList oldRanges, GridSelectionReason reason)
        {
            GridSelectionChangedEventArgs e = new GridSelectionChangedEventArgs(range, oldRanges, reason);
            Model.RaiseSelectionChanged(e);
        }

        bool RaiseSelectionChanging(ref GridRangeInfo range, GridSelectionReason reason)
        {
            return RaiseSelectionChanging(ref range, reason, GridRangeInfo.Empty);
        }

        bool RaiseSelectionChanging(ref GridRangeInfo range, GridSelectionReason reason, GridRangeInfo clickRange)
        {
            GridSelectionChangingEventArgs e = new GridSelectionChangingEventArgs(range, reason, clickRange);
            Model.RaiseSelectionChanging(e);
            range = e.Range;
            return !e.Cancel;
        }

        bool StoreSelectRange(GridRangeInfo range, bool bSelect)
        {
            // Check.
            GridRangeInfoList pSelList = model.SelectedRanges;

            if (bSelect)
            {
                // Simply add it to the list of selections.
                if (pSelList.Contains(range))
                    return false;
                pSelList.Add(range);
            }
            else if (range.IsTable)
                pSelList.Clear();
            else
            {
                // Remove it from the list of selections.
                GridRangeInfoList rl = pSelList.GetRangesIntersecting(range);
                GridRangeInfo intersect;
                GridRangeInfo expand;
                GridRangeInfo rangeEx = range.ExpandRange(1, 1, model.RowCount, model.ColumnCount);
                foreach (GridRangeInfo item in rl)
                {
                    intersect = range.IntersectRange(item);
                    if (item.Equals(intersect))
                        pSelList.Remove(item);
                    else if (item.IsRows && range.IsRows)
                    {
                        pSelList.Remove(item);
                        intersect = range.IntersectRange(item);
                        if (item.Top < intersect.Top)
                            pSelList.Add(GridRangeInfo.Rows(item.Top, intersect.Top - 1));
                        if (item.Bottom > intersect.Bottom)
                            pSelList.Add(GridRangeInfo.Rows(intersect.Bottom + 1, item.Bottom));
                    }
                    else if (item.IsCols && range.IsCols)
                    {
                        pSelList.Remove(item);
                        intersect = range.IntersectRange(item);
                        if (item.Left < intersect.Left)
                            pSelList.Add(GridRangeInfo.Cols(item.Left, intersect.Left - 1));
                        if (item.Right > intersect.Right)
                            pSelList.Add(GridRangeInfo.Cols(intersect.Right + 1, item.Right));
                    }
                    else
                    {
                        pSelList.Remove(item);
                        expand = item.ExpandRange(1, 1, model.RowCount, model.ColumnCount);

                        // Check if is a interior rectangle of a given range.
                        // If yes, exclude the area from the existing range and
                        // break the existing range into maximum 4 pieces.

                        if (item.Top < intersect.Top)
                        {
                            if (item.IsRows)
                                pSelList.Add(GridRangeInfo.Rows(item.Top, intersect.Top - 1));
                            else
                                pSelList.Add(GridRangeInfo.Cells(item.Top, item.Left, intersect.Top - 1, item.Right));
                        }

                        if (item.Bottom > intersect.Bottom)
                        {
                            if (item.IsRows)
                                pSelList.Add(GridRangeInfo.Rows(intersect.Bottom + 1, item.Bottom));
                            else
                                pSelList.Add(GridRangeInfo.Cells(intersect.Bottom + 1, item.Left, item.Bottom, item.Right));
                        }

                        if (item.Left < intersect.Left)
                        {
                            if (item.IsCols)
                                pSelList.Add(GridRangeInfo.Cols(item.Left, intersect.Left - 1));
                            else
                                pSelList.Add(GridRangeInfo.Cells(item.Top, item.Left, item.Bottom, intersect.Left - 1));
                        }

                        if (item.Right > intersect.Right)
                        {
                            if (item.IsCols)
                                pSelList.Add(GridRangeInfo.Cols(intersect.Right + 1, item.Right));
                            else
                                pSelList.Add(GridRangeInfo.Cells(item.Top, intersect.Right + 1, item.Bottom, item.Right));
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Clears all selections.
        /// </summary>
        public void Clear()
        {
            Clear(true);
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        /// <param name="raiseEvents">Indicates if SelectionChanging and SelectionChanged should be raised.</param>
        public void Clear(bool raiseEvents)
        {
            GridRangeInfoList pSelList = model.SelectedRanges;

            GridRangeInfo range = GridRangeInfo.Empty;
            if (!raiseEvents || this.RaiseSelectionChanging(ref range, GridSelectionReason.Clear))
            {
                // Clear all selections.
                //Model.FloatingCells.lockEvaluateFloatingCells = true;
                pSelList.Clear();
                //Model.FloatingCells.lockEvaluateFloatingCells = false;
                if (raiseEvents)
                    this.RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.Clear);
            }
        }

        /// <overload>
        /// Changes an existing selection.
        /// </overload>
        /// <summary>
        /// Changes an existing selection.
        /// </summary>
        /// <param name="oldRange">The range to be changed.</param>
        /// <param name="newRange">The new boundaries of the range.</param>
        /// <remarks>
        /// The grid calls this method when you select cells with the mouse.
        /// </remarks>
        public void ChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange, GridSelectionReason reason)
        {
            ChangeSelection(oldRange, newRange, false, reason);
        }

        /// <summary>
        /// Changes an existing selection.
        /// </summary>
        /// <param name="oldRange">The range to be changed.</param>
        /// <param name="newRange">The new boundaries of the range.</param>
        /// <param name="raiseEvents">Specifies if SelectionChanging and SelectionChanged events should be raised.</param>
        /// <remarks>
        /// The grid calls this method when you select cells with the mouse.
        /// </remarks>
        public void ChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange, bool raiseEvents, GridSelectionReason reason)
        {
            GridRangeInfoList pSelList = model.SelectedRanges;

            if (oldRange.IsEmpty && newRange.IsEmpty)
            {
                Clear(raiseEvents);
            }
            else
            {
                if (!raiseEvents || this.RaiseSelectionChanging(ref newRange,reason))
                {
                    // Remove old range.
                    int index = pSelList.IndexOf(oldRange);
                    if (index != -1)
                        pSelList.RemoveAt(index);

                    GridRangeInfo pRange = newRange;
                    //if (newRange.IsCells)
                    //{
                    //    if (newRange.Top == 0 && newRange.Left == 0 && model.HeaderColumns > 0 && model.HeaderRows > 0)
                    //        pRange = GridRangeInfo.Table();
                    //    else if (newRange.Left == 0 && model.HeaderColumns > 0 || this.Model.Options.ListBoxSelectionMode == GridSelectionMode.MultiExtended)
                    //        pRange = GridRangeInfo.Rows(newRange.Top, newRange.Bottom);
                    //    else if (newRange.Top == 0 && model.HeaderRows > 0)
                    //        pRange = GridRangeInfo.Cols(newRange.Left, newRange.Right);
                    //}

                    // Add range to selected ranges list.
                    if (!pRange.IsEmpty)
                        pSelList.Add(pRange);
#if DEBUG
                    //else
                    //    Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, "Not added");
#endif


                    if (raiseEvents)
                        this.RaiseSelectionChanged(pRange, reason);
                }
            }
        }


        /// <summary>
        /// Returns an enumerator for stepping through all selected ranges.
        /// </summary>
        /// <returns>The enumerator for the <see cref="Ranges"/> collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return Ranges.GetEnumerator();
        }

        ///// <summary>
        ///// Copies all range objects into an array of <see cref="GridRangeInfo"/> starting at specified index.
        ///// </summary>
        ///// <param name="array">The array of <see cref="GridRangeInfo"/> where the values should be copied to.</param>
        ///// <param name="index">The starting index in the destination array.</param>
        //public void CopyTo(Array array, int index)
        //{
        //    //Ranges.CopyTo(array, index);
        //}

        /// <summary>
        /// The number of ranges in the <see cref="GridModelSelections"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return Ranges.Count;
            }
        }

        /// <summary>
        ///  Returns NULL.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        ///  Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public void InsertRows(int insertAtRowIndex, int count, GridRangeInfoList moveState)
        {
            for (int n = 0; n < Ranges.Count; n++)
            {
                GridRangeInfo range = Ranges[n];
                if (!range.IsEmpty && range.Top >= insertAtRowIndex)
                {
                    range = range.OffsetRange(count, 0);
                    Ranges[n] = range;
                }
            }

            if (moveState != null)
            {
                foreach (GridRangeInfo range in moveState)
                {
                    Ranges.Add(range.OffsetRange(insertAtRowIndex, 0));
                }
            }


//                this.RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.Clear);
        }

        public void RemoveRows(int removeAtRowIndex, int count, GridRangeInfoList moveState)
        {
            for (int n = 0; n < Ranges.Count; n++)
            {
                GridRangeInfo range = Ranges[n];
                if (!range.IsEmpty && range.Top >= removeAtRowIndex)
                {
                    if (range.Top >= removeAtRowIndex + count)
                        range = range.OffsetRange(-count, 0);
                    else 
                    {
                        if (moveState != null)
                            moveState.Add(range.OffsetRange(-removeAtRowIndex, 0));
                        range = GridRangeInfo.Empty;
                    }
                    Ranges[n] = range;
                }
            }
        }

        public void InsertColumns(int insertAtColumnIndex, int count, GridRangeInfoList moveState)
        {
            for (int n = 0; n < Ranges.Count; n++)
            {
                GridRangeInfo range = Ranges[n];
                if (!range.IsEmpty && range.Left >= insertAtColumnIndex)
                {
                    range = range.OffsetRange(0, count);
                    Ranges[n] = range;
                }
            }

            if (moveState != null)
            {
                foreach (GridRangeInfo range in moveState)
                {
                    //Ranges.Add(range.OffsetRange(0, insertAtColumnIndex));
                    Ranges.Add(range);
                }
            }
        }

        public void RemoveColumns(int removeAtColumnIndex, int count, GridRangeInfoList moveState)
        {
            for (int n = 0; n < Ranges.Count; n++)
            {
                GridRangeInfo range = Ranges[n];
                if (!range.IsEmpty && range.Left >= removeAtColumnIndex)
                {
                    if (range.Left >= removeAtColumnIndex + count)
                        range = range.OffsetRange(0, -count);
                    else 
                    {
                        if (moveState != null)
                            moveState.Add(range.OffsetRange(0, -removeAtColumnIndex));
                        range = GridRangeInfo.Empty;
                    }
                    Ranges[n] = range;
                }
            }
            Ranges.RemoveEmptyRanges();
        }
    }
}
