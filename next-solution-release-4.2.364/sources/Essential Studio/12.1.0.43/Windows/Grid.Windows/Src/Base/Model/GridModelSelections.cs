//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelSelections.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Manages selected ranges in the grid. Allows you to add and remove selections, determine
    /// selection state of a specific cell, and more.
    /// </summary>
    public class GridModelSelections : GridModelBound, IEnumerable
    {
        // Constructor

        /// <summary>
        /// Initializes a new <see cref="GridModelSelections"/> object and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelSelections(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Gets the collection with all selected ranges.
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
                {
                    ranges.Add(range);
                }
            }

            if (ranges.Count == 0 && considerCurrentCell)
            {
                GridCurrentCellInfo cci = model.CurrentCellInfo;
                if (cci != null)
                {
                    ranges.Add(GridRangeInfo.Cell(cci.RowIndex, cci.ColIndex));
                }
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
        /// If you specify False for <paramref name="bRangeColsOnly"/>, the method will return range C2:C5. <para/>
        /// If you specify True for <paramref name="bRangeColsOnly"/> and also True for <paramref name="considerCurrentCell"/>,  
        /// the method will return range C5. <para/>
        /// </remarks>
        public GridRangeInfoList GetSelectedCols(bool bRangeColsOnly/* = false*/, bool considerCurrentCell /*= true*/)
        {
            GridRangeInfoList ranges = model.SelectedRanges.GetColRanges(bRangeColsOnly ? GridRangeInfoType.Cols : GridRangeInfoType.Cells | GridRangeInfoType.Cols);
            if (ranges.Count == 0 && considerCurrentCell)
            {
                GridCurrentCellInfo cci = model.CurrentCellInfo;
                if (cci != null)
                {
                    ranges.Add(GridRangeInfo.Col(cci.ColIndex));
                }
            }
            if (ranges.Count == 1)
            {
                GridRangeInfo info = ranges[0];
                if (info.IsTable)
                {
                    if (model != null && model.RowCount > 0)
                    {
                        ranges.Clear();
                        ranges.Add(GridRangeInfo.UnionRange(GridRangeInfo.Col(1), GridRangeInfo.Col(model.ColCount)));
                    }
                }
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
        /// If you specify False for bRangeColsOnly the method will return range R1:R2;R4. <para/>
        /// If you specify True for bRangeColsOnly and also True for <paramref name="considerCurrentCell"/>,
        /// the method will return range R4. <para/>
        /// </remarks>
        public GridRangeInfoList GetSelectedRows(bool bRangeRowsOnly/* = false*/, bool considerCurrentCell /*= true*/)
        {
            GridRangeInfoList ranges = model.SelectedRanges.GetRowRanges(bRangeRowsOnly ? GridRangeInfoType.Rows : GridRangeInfoType.Cells | GridRangeInfoType.Rows);
            if (ranges.Count == 0 && considerCurrentCell)
            {
                GridCurrentCellInfo cci = model.CurrentCellInfo;
                if (cci != null)
                {
                    ranges.Add(GridRangeInfo.Row(cci.RowIndex));
                }
            }
            if (ranges.Count == 1)
            {
                GridRangeInfo info = ranges[0];
                if (info.IsTable)
                {
                    if (model != null && model.RowCount > 0)
                    {
                        ranges.Clear();
                        ranges.Add(GridRangeInfo.UnionRange(GridRangeInfo.Row(1), GridRangeInfo.Row(model.RowCount)));
                    }
                }
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
                Model.FloatingCells.lockEvaluateFloatingCells = true;           // Don't evaluate int/*float*/ cell state
                RaiseSelectionChanged(range, oldRanges, GridSelectionReason.SelectRange);
                Model.FloatingCells.lockEvaluateFloatingCells = false;
            }
        }

        /// <summary>
        /// Sets internal properties to simulate having clicked a particular row and col to start a shift-selection process.
        /// </summary>
        /// <param name="rowIndex">The grid row index.</param>
        /// <param name="colIndex">The grid column index.</param>
        /// <remarks>
        /// When you explicitly call gridControl1.Selections.SelectRange to select cells, the grid does not treat this the same 
        /// as clicking on the cells to select them. When you click cells to select them, internal fields track the clicks
        /// so that the selection can be extended using additional shift+clicks. If you want your range selected by using 
        /// gridControl1.Selections.SelectRange to be extendible in this manner, then you will need to make an additional call
        /// to tell the grid to set its internal fields.
        /// </remarks>
        /// <example>
        /// The code shows a simple use case.
        /// <code lang="C#">
        /// gridControl1.Selections.SelectRange(GridRangeInfo.Row(1), true);
        /// gridControl1.Selections.SetSelectClickRowCol(1, 0);
        /// </code>
        /// </example>
        public void SetSelectClickRowCol(int rowIndex, int colIndex)
        {
            if (Model.ActiveGridView != null)
            {
                GridSelectCellsMouseController mc = Model.ActiveGridView.MouseControllerDispatcher.Find("SelectCells")
                                                    as GridSelectCellsMouseController;
                SetSelectClickRowCol(rowIndex, colIndex, mc);
            }
        }

        /// <summary>
        /// Sets internal properties to simulate having clicked a particular row and col to start a shift-selection process.
        /// </summary>
        /// <param name="rowIndex">The grid row index.</param>
        /// <param name="colIndex">The grid column index.</param>
        /// <param name="mc">The <see cref="GridSelectCellsMouseController"/> mouse controller.</param>
        /// <remarks>
        /// When you explicitly call gridControl1.Selections.SelectRange to select cells, the grid does not treat this the same 
        /// as clicking on the cells to select them. When you click cells to select them, internal fields track the clicks
        /// so that the selection can be extended using additional shift+clicks. If you want your range selected by using 
        /// gridControl1.Selections.SelectRange to be extendible in this manner, then you will need to make an additional call
        /// to tell the grid to set its internal fields.
        /// </remarks>
        public void SetSelectClickRowCol(int rowIndex, int colIndex, GridSelectCellsMouseController mc)
        {
            if (mc != null)
            {
                mc.SetSelectClickRowCol(rowIndex, colIndex);
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
                {
                    return false;
                }

                pSelList.Add(range);
            }
            else if (range.IsTable)
            {
                pSelList.Clear();
            }
            else
            {
                // Remove it from the list of selections.
                GridRangeInfoList rl = pSelList.GetRangesIntersecting(range);
                GridRangeInfo intersect;
                GridRangeInfo expand;
                GridRangeInfo rangeEx = range.ExpandRange(1, 1, model.RowCount, model.ColCount);
                foreach (GridRangeInfo item in rl)
                {
                    intersect = range.IntersectRange(item);
                    if (item.Equals(intersect))
                    {
                        pSelList.Remove(item);
                    }
                    else if (item.IsRows && range.IsRows)
                    {
                        pSelList.Remove(item);
                        intersect = range.IntersectRange(item);
                        if (item.Top < intersect.Top)
                        {
                            pSelList.Add(GridRangeInfo.Rows(item.Top, intersect.Top - 1));
                        }

                        if (item.Bottom > intersect.Bottom)
                        {
                            pSelList.Add(GridRangeInfo.Rows(intersect.Bottom + 1, item.Bottom));
                        }
                    }
                    else if (item.IsCols && range.IsCols)
                    {
                        pSelList.Remove(item);
                        intersect = range.IntersectRange(item);
                        if (item.Left < intersect.Left)
                        {
                            pSelList.Add(GridRangeInfo.Cols(item.Left, intersect.Left - 1));
                        }

                        if (item.Right > intersect.Right)
                        {
                            pSelList.Add(GridRangeInfo.Cols(intersect.Right + 1, item.Right));
                        }
                    }
                    else
                    {
                        pSelList.Remove(item);
                        expand = item.ExpandRange(1, 1, model.RowCount, model.ColCount);

                        //// Check if is a interior rectangle of a given range.
                        //// If yes, exclude the area from the existing range and
                        //// break the existing range into maximum 4 pieces.

                        if (item.Top < intersect.Top)
                        {
                            if (item.IsRows)
                            {
                                pSelList.Add(GridRangeInfo.Rows(item.Top, intersect.Top - 1));
                            }
                            else
                            {
                                pSelList.Add(GridRangeInfo.Cells(item.Top, item.Left, intersect.Top - 1, item.Right));
                            }
                        }

                        if (item.Bottom > intersect.Bottom)
                        {
                            if (item.IsRows)
                            {
                                pSelList.Add(GridRangeInfo.Rows(intersect.Bottom + 1, item.Bottom));
                            }
                            else
                            {
                                pSelList.Add(GridRangeInfo.Cells(intersect.Bottom + 1, item.Left, item.Bottom, item.Right));
                            }
                        }

                        if (item.Left < intersect.Left)
                        {
                            if (item.IsCols)
                            {
                                pSelList.Add(GridRangeInfo.Cols(item.Left, intersect.Left - 1));
                            }
                            else
                            {
                                pSelList.Add(GridRangeInfo.Cells(item.Top, item.Left, item.Bottom, intersect.Left - 1));
                            }
                        }

                        if (item.Right > intersect.Right)
                        {
                            if (item.IsCols)
                            {
                                pSelList.Add(GridRangeInfo.Cols(intersect.Right + 1, item.Right));
                            }
                            else
                            {
                                pSelList.Add(GridRangeInfo.Cells(item.Top, intersect.Right + 1, item.Bottom, item.Right));
                            }
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
            Clear(false);
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        /// <param name="raiseEvents">Indicates if SelectionChanging and SelectionChanged should be raised.</param>
        public void Clear(bool raiseEvents)
        {
#if DEBUG
            Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, "GridSelectRange.Clear()");
#endif
            GridRangeInfoList pSelList = model.SelectedRanges;

            GridRangeInfo range = GridRangeInfo.Empty;
            if (!raiseEvents || this.RaiseSelectionChanging(ref range, GridSelectionReason.Clear))
            {
                // Clear all selections.
                Model.FloatingCells.lockEvaluateFloatingCells = true;
                if (pSelList.Count > 0)
                {
                    PrepareClearSelection(true);
                }

                pSelList.Clear();
                Model.FloatingCells.lockEvaluateFloatingCells = false;
                if (raiseEvents)
                {
                    this.RaiseSelectionChanged(GridRangeInfo.Empty, GridSelectionReason.Clear);
                }
            }
        }

        void PrepareClearSelection(bool bCreateHint /*= false*/)
        {
            Model.RaisePrepareClearSelection(EventArgs.Empty);
        }

        void PrepareChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange, bool bCreateHint /*= false*/)
        {
            Model.RaisePrepareChangeSelection(new GridPrepareChangeSelectionEventArgs(oldRange, newRange));
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
        public void ChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange)
        {
            ChangeSelection(oldRange, newRange, false);
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
        public void ChangeSelection(GridRangeInfo oldRange, GridRangeInfo newRange, bool raiseEvents)
        {
#if DEBUG
            Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, String.Format("GridSelectRange.ChangeSelection({0}, {1})", oldRange, newRange));
#endif

            GridRangeInfoList pSelList = model.SelectedRanges;

            if (oldRange.IsEmpty && newRange.IsEmpty)
            {
                Clear(raiseEvents);
            }
            else
            {
                if (!raiseEvents || this.RaiseSelectionChanging(ref newRange, GridSelectionReason.SelectRange))
                {
                    ////                    if (model.ListBoxMode != SelectionMode.None)
                    ////                        newRange = GridRangeInfo.Rows(newRange.Top, newRange.Bottom);
                    ////
                    //// Ensure there are no invalidated regions.
                    //// model.Update();

                    //// Remove old range.
                    int index = pSelList.IndexOf(oldRange);
                    if (index != -1)
                    {
                        pSelList.RemoveAt(index);
                    }

                    //// Draw the new range.
                    Model.FloatingCells.lockEvaluateFloatingCells = true;
                    PrepareChangeSelection(oldRange, newRange, true);
                    Model.FloatingCells.lockEvaluateFloatingCells = false;

                    GridRangeInfo pRange = newRange;
                    if (newRange.IsCells)
                    {
                        if (newRange.Top == 0 && newRange.Left == 0)
                        {
                            pRange = GridRangeInfo.Table();
                        }
                        else if (newRange.Left == 0)
                        {
                            pRange = GridRangeInfo.Rows(newRange.Top, newRange.Bottom);
                        }
                        else if (newRange.Top == 0)
                        {
                            pRange = GridRangeInfo.Cols(newRange.Left, newRange.Right);
                        }
                    }

                    // Add range to selected ranges list.
                    if (!pRange.IsEmpty)
                    {
                        pSelList.Add(pRange);
                    }
#if DEBUG
                    else
                    {
                        Trace.WriteLineIf(Switches.SelectRange.TraceVerbose, "Not added");
                    }
#endif
                    if (this.Model != null && Model.ActiveGridView != null && (Model.Properties.MarkRowHeader || Model.Properties.MarkColHeader))
                    {
                        Model.ActiveGridView.BeginUpdate();
                        Model.ActiveGridView.UpdateStyles();
                        Model.ActiveGridView.EndUpdate(true);
                    }
                    if (raiseEvents)
                    {
                        this.RaiseSelectionChanged(pRange, GridSelectionReason.SelectRange);
                    }
                }
            }
        }

        //// IList interface

        ////            object IList.get_Item(int index) 
        ////            {
        ////                return Ranges[index];
        ////            }
        ////            void ILisy.set_Item(int index, object value)
        ////            {
        ////                if (!(value is GridRangeInfo))
        ////                    throw new ArgumentException("value");
        ////                ChangeSelection(Ranges[index], (GridRangeInfo) value);
        ////            }
        ////            int Add(object value)
        ////            {
        ////                this.Add((GridRangeInfo) value);
        ////            }
        ////            bool Contains(object value)
        ////            {
        ////                return 
        ////            }

        /// <summary>
        /// Returns an enumerator for stepping through all selected ranges.
        /// </summary>
        /// <returns>The enumerator for the <see cref="Ranges"/> collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return Ranges.GetEnumerator();
        }

        /// <summary>
        /// Copies all range objects into an array of <see cref="GridRangeInfo"/> starting at specified index.
        /// </summary>
        /// <param name="array">The array of <see cref="GridRangeInfo"/> where the values should be copied to.</param>
        /// <param name="index">The starting index in the destination array.</param>
        public void CopyTo(Array array, int index)
        {
            Ranges.CopyTo(array, index);
        }

        /// <summary>
        /// Gets the number of ranges in the <see cref="GridModelSelections"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return Ranges.Count;
            }
        }

        /// <summary>
        ///  Gets NULL.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Is Synchronized. Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }
    }
}
