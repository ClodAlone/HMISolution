//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelCoveredRanges.cs" company="syncfusion">
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
    internal class GridModelCoveredRangesEditor : UITypeEditor
    {
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.Instance == null || provider == null)
            {
                return value; 
            }

#if SyncfusionFramework2_0
            object instance = context.Instance;

            RangesList ranges = new RangesList();
            ranges.AllowNew = true;
            ranges.AllowEdit = true;

            GridModelCoveredRanges coveredRanges = (GridModelCoveredRanges)value;
            foreach (GridRangeInfo range in coveredRanges)
            {
                ranges.Add(new GridRangeInfoItem(range));
            }

            GridRangeInfoListEditor dlg = new GridRangeInfoListEditor(ranges);
            dlg.Text = "Covered Ranges";
            dlg.label1.Text = "Covered Ranges";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                IComponentChangeService changeService = null;
                PropertyDescriptor pd = null;
                if (provider != null)
                {
                    changeService = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    pd = TypeDescriptor.GetProperties(instance)["CoveredRanges"];
                }

                if (changeService != null)
                {
                    changeService.OnComponentChanging(instance, pd);
                }

                coveredRanges.Clear();
                foreach (GridRangeInfoItem range in ranges)
                {
                    coveredRanges.Add(range.GetRangeInfo());
                }

                if (changeService != null)
                {
                    changeService.OnComponentChanged(instance, pd, coveredRanges, coveredRanges);
                }
            }
            else
            {
                return value;
            }
#else
            CollectionEditor ce = new CollectionEditor(typeof(GridRangeInfoList));
            GridModelCoveredRanges coveredRanges = ((GridModelCoveredRanges) value);
            object obj = ce.EditValue(context, provider, coveredRanges.Ranges);
#endif
            coveredRanges.UpdateList();
            coveredRanges.Model.Modified |= coveredRanges.Count > 0;
            coveredRanges.Model.Refresh();

            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    /// <summary>
    /// This class manages covered cell ranges for a grid.
    /// </summary>
    /// <remarks>
    /// You access this class from a grid with the <see cref="GridModel.CoveredRanges"/>
    /// property of a <see cref="GridModel"/> instance.
    /// <para/>
    /// Covered ranges are saved in two separate collections.
    /// <para/>
    /// The first collection is <see cref="GridRangeInfoList"/>
    /// that allows quick enumeration through all covered cell ranges in the grid. This is good when covered ranges
    /// need to be recalculated because rows or column have been inserted, moved, or removed.
    /// <para/>
    /// The second collection is <see cref="GridCoveredCellPool"/> that is optimized to look up if a specific cell
    /// is part of a covered range.
    /// <para/>
    /// The <see cref="GridModel.QueryCoveredRange"/> event in a <see cref="GridModel"/> lets you also provide customized
    /// covered cell ranges at run-time. For example, you might want to have a pattern of covered ranges. 
    /// This allows you to customize the grid's default behavior and manage covered
    /// ranges by your own code and not with the <see cref="GridModelCoveredRanges"/> class.
    /// </remarks>
    [Serializable]
    [Editor(typeof(GridModelCoveredRangesEditor), typeof(UITypeEditor))]
    public sealed class GridModelCoveredRanges : GridModelBound, ICollection, ISerializable, IEnumerable
    {
        // Covered Cells.
        private GridRangeInfoList coveredRanges = new GridRangeInfoList();
        private GridCoveredCellPool pool = new GridCoveredCellPool();

        /// <overload>
        /// Initializes a new <see cref="GridModelCoveredRanges"/> from a serialization stream.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridModelCoveredRanges"/> object and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        public GridModelCoveredRanges(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelCoveredRanges"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needs to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelCoveredRanges(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            coveredRanges = (GridRangeInfoList)info.GetValue("Ranges", typeof(GridRangeInfoList));
            pool = (GridCoveredCellPool)info.GetValue("Pool", typeof(GridCoveredCellPool));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelCoveredRanges"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("Ranges", coveredRanges); // GridRangeInfoList
            info.AddValue("Pool", pool); // GridCoveredCellPool
        }
        
        /// <summary>
        /// Gets the <see cref="GridRangeInfoList"/> and lets you enumerate through all covered ranges
        /// managed by this <see cref="GridModelCoveredRanges"/> instance.
        /// </summary>
        public GridRangeInfoList Ranges
        {
            get
            {
                return coveredRanges;
            }
        }

        /// <summary>
        /// Adds a covered range cell.
        /// </summary>
        /// <param name="range">A range to add.</param>
        public void Add(GridRangeInfo range)
        {
            SetCoveredRange(range, true);
        }

        /// <summary>
        /// Adds multiple covered cell ranges.
        /// </summary>
        /// <param name="ranges">The list of ranges to add</param>
        public void AddRange(GridRangeInfo[] ranges)
        {
            foreach (GridRangeInfo range in ranges)
            {
                SetCoveredRange(range, true);
            }
        }

        /// <summary>
        /// Removes covered cells that are contained in the specified range.
        /// </summary>
        /// <param name="range">The range that you want to clear from covered cells.</param>
        public void Remove(GridRangeInfo range)
        {
            SetCoveredRange(range, false);
        }

        /// <summary>
        /// Adds or removes a covered range.
        /// </summary>
        /// <param name="range">The affected range that indicates a new covered cell or the range of cells where covered cells should be removed.</param>
        /// <param name="setOrReset">True if you want to add a covered; False if range should be cleared.</param>
        public void SetCoveredRange(GridRangeInfo range, bool setOrReset)
        {
            GridRangeInfoList ranges = new GridRangeInfoList();
            ranges.Add(range);
            SetCoveredRanges(ranges, setOrReset);
        }

        /// <summary>
        /// Resets all covered ranges.
        /// </summary>
        public void Clear()
        {
            SetCoveredRange(GridRangeInfo.Table(), false);
        }

        /// <summary>
        /// Adds or removes one or multiple covered ranges.
        /// </summary>
        /// <param name="ranges">The list that holds range with new covered cells or ranges where covered cells should be removed.</param>
        /// <param name="setOrReset">True if you want to add covered cells; False if ranges should be cleared.</param>
        public void SetCoveredRanges(GridRangeInfoList ranges, bool setOrReset)
        {
            cacheLastCoveredRange = GridRangeInfo.Empty;

            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "SetCoveredRanges");
            try
            {
                if (OnChanging(new GridCoveredRangesChangingEventArgs(ranges, setOrReset)))
                {
                    bool success = false;
                    GridRangeInfoList oldRanges = new GridRangeInfoList();
                    try
                    {
                        //// Check if model is Read-only.
                        ////                        if (pParam.lockReadOnly && (model.IsReadOnly || model.Model.BaseStylesMap["Standard"].StyleInfo.ReadOnly))
                        ////                            return false;

                        foreach (GridRangeInfo range in ranges)
                        {
                            GridRangeInfo cellRange = range.ExpandRange(0, 0, Model.RowCount, Model.ColCount);

                            // ... now, I can store the data
                            if (!setOrReset)
                            {
                                this.coveredRanges.GetRangesContained(range).CopyTo(oldRanges, 0);

                                if (pool.ResetSpanCells(cellRange))
                                {
                                    foreach (GridRangeInfo rg in coveredRanges.GetRangesIntersecting(range))
                                    {
                                        coveredRanges.Remove(rg);
                                        Model.FloatingCells.DelayFloatCells(rg);
                                        Model.MergeCells.DelayMergeCells(rg);
                                    }

                                    success = true;
                                }
                            }
                            else
                            {
                                if (!coveredRanges.AnyRangeIntersects(range)
                                    && pool.StoreSpanCells(cellRange))
                                {
                                    coveredRanges.Add(range);
                                    oldRanges.Add(range);
                                    Model.FloatingCells.DelayFloatCells(range);
                                    success = true;
                                }
                            }
                        }

                        if (!success)
                        {
                            return; //// nothing done
                        }

                        ////                        if (/*!discardUndo &&*/ model.CommandList.ShouldRecordCommandInfo)
                        ////                            model.CommandList.Add(new GridModelSetCoveredRangesCommand(this, ranges, setOrReset));

                        if (/*!discardUndo &&*/ model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            model.CommandStack.Push(new GridModelSetCoveredRangesCommand(this, oldRanges, !setOrReset));
                        }
                    }
                    catch (Exception ex)
                    {
                        TraceUtil.TraceExceptionCatched(ex);
                        if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        OnChanged(new GridCoveredRangesChangedEventArgs(ranges, setOrReset, success));
                    }
                }
            }
            finally
            {
                Model.EndUpdate();
            }
        }

        /// <summary>
        /// Returns an enumerator for stepping through all covered ranges.
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
        /// Gets the number of ranges in the <see cref="GridModelCoveredRanges"/>.
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
        ///  Gets a value indicating whether is synchronized. Returns False.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        internal GridRangeInfo cacheLastCoveredRange = GridRangeInfo.Empty;
        internal bool cacheLastHandled = false;

        /// <exclude/>
        /// <summary>Resets the cache.</summary>
        public void ResetCache()
        {
            cacheLastCoveredRange = GridRangeInfo.Empty;
        }

        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the covered range for the specified cell position
        /// or False if there is no covered range for the given cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">The <see cref="GridRangeInfo"/> where the found covered range is returned.</param>
        /// <returns>True if a covered range is at the specified cell position; False if not.</returns>
        public bool Find(int rowIndex, int colIndex, out GridRangeInfo range)
        {
            if (cacheLastCoveredRange.IsCells && cacheLastCoveredRange.Contains(GridRangeInfo.Cell(rowIndex, colIndex)))
            {
                range = cacheLastCoveredRange;
                return cacheLastHandled;
            }

            if (_Find(rowIndex, colIndex, out range))
            {
                if (range != null && range.IsCells)
                {
                    cacheLastCoveredRange = range;
                    cacheLastHandled = true;
                    return true;
                }
            }

            range = cacheLastCoveredRange = GridRangeInfo.Cell(rowIndex, colIndex);
            cacheLastHandled = false;
            return false;
        }

        bool _Find(int rowIndex, int colIndex, out GridRangeInfo range)
        {
            GridQueryCoveredRangeEventArgs e = new GridQueryCoveredRangeEventArgs(rowIndex, colIndex);
            Model.RaiseQueryCoveredRange(e);
            if (e.Handled)
            {
                range = e.Range;
                if (range != null && range.IsCells)
                {
                    cacheLastCoveredRange = range;
                    return true;
                }
                else
                {
                    e.Range = cacheLastCoveredRange = GridRangeInfo.Cell(rowIndex, colIndex);
                    cacheLastHandled = false;
                }

                return false;
            }

            return pool.GetSpanCellsRowCol(rowIndex, colIndex, out range);
        }

        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the covered range for the specified cell position
        /// or <see cref="GridRangeInfo.Empty"/> if there is no covered range for the given cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A reference to the covered range is at the specified cell position or
        /// <see cref="GridRangeInfo.Empty"/> if not.</returns>
        public GridRangeInfo FindRange(int rowIndex, int colIndex)
        {
            GridRangeInfo foundRange;
            if (Find(rowIndex, colIndex, out foundRange))
            {
                colIndex = foundRange.Right + 1;
                return foundRange;
            }

            return GridRangeInfo.Empty;
        }

        /// <summary>
        /// Combines all covered ranges that intersect into one outer range that spans over all found ranges.
        /// </summary>
        /// <param name="range">The original range.</param>
        /// <returns>The <see cref="GridRangeInfo"/> with the outer range.</returns>
        public GridRangeInfo Merge(GridRangeInfo range)
        {
            if (coveredRanges.Count > 0)
            {
                GridRangeInfo savedRange = range;
                for (int rowIndex = savedRange.Top; rowIndex <= savedRange.Bottom; rowIndex++)
                {
                    for (int colIndex = savedRange.Left; colIndex <= savedRange.Right; colIndex++)
                    {
                        GridRangeInfo foundRange;
                        if (Find(rowIndex, colIndex, out foundRange))
                        {
                            colIndex = foundRange.Right + 1;
                            range = GridRangeInfo.UnionRange(foundRange, range);
                        }
                    }
                }
            }

            return range;
        }

        /// <summary>
        /// Synchronizes the <see cref="GridCoveredCellPool"/>. The pool will be emptied and initialized with ranges specified in the <see cref="Ranges"/> collection.
        /// </summary>
        public void UpdateList()
        {
            try
            {
                if (pool == null)
                {
                    pool = new GridCoveredCellPool();
                }

                pool.InitFromRangeList(this.coveredRanges);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Synchronizes the covered ranges after rows have been inserted in the grid.
        /// </summary>
        /// <param name="insertAt">The starting row index where new rows should be inserted.</param>
        /// <param name="count">The number of rows to insert.</param>
        public void InsertRows(int insertAt, int count)
        {
            Model.CoveredRanges.Ranges.InsertRows(insertAt, count);
            UpdateList();
        }

        /// <summary>
        /// Synchronizes the covered ranges after columns have been inserted in the grid.
        /// </summary>
        /// <param name="insertAt">The starting columns index where new columns should be inserted.</param>
        /// <param name="count">The number of rows to insert.</param>
        public void InsertCols(int insertAt, int count)
        {
            Ranges.InsertCols(insertAt, count);
            UpdateList();
        }

        /// <summary>
        /// Synchronizes the covered ranges after rows have been removed from the grid.
        /// </summary>
        /// <param name="fromRowIndex">The first row index.</param>
        /// <param name="toRowIndex">The last row index.</param>
        /// <returns>True if any covered ranges were affected; False if no range needed to be changed.</returns>
        public bool RemoveRows(int fromRowIndex, int toRowIndex)
        {
            // Covered Cells
            if (model.CommandStack.IsRecording)
            {
                GridRangeInfoList rl = coveredRanges.Clone();
                foreach (GridRangeInfo r in rl)
                {
                    if (r.Top >= fromRowIndex && r.Top <= toRowIndex)
                    {
                        SetCoveredRange(r, false);
                        SetCoveredRange(GridRangeInfo.Cells(r.Top, r.Left, r.Top, r.Left), true);
                    }
                    else if (r.Top < fromRowIndex && r.Bottom >= fromRowIndex && r.Bottom <= toRowIndex)
                    {
                        SetCoveredRange(r, false);
                        SetCoveredRange(GridRangeInfo.Cells(r.Top, r.Left, r.Top, r.Left), true);
                        SetCoveredRange(GridRangeInfo.Cells(r.Top, r.Left, fromRowIndex - 1, r.Right), true);
                    }
                }
            }

            Ranges.RemoveRows(fromRowIndex, toRowIndex, this.Model.RowCount);
            UpdateList();
            return true;
        }

        /// <summary>
        /// Synchronizes the covered ranges after columns have been removed from the grid.
        /// </summary>
        /// <param name="fromColIndex">The first column index.</param>
        /// <param name="toColIndex">The last column index.</param>
        /// <returns>True if any covered ranges were affected; False if no range needed to be changed.</returns>
        public bool RemoveCols(int fromColIndex, int toColIndex)
        {
            // Covered Cells
            if (model.CommandStack.IsRecording)
            {
                GridRangeInfoList rl = coveredRanges.Clone();
                foreach (GridRangeInfo r in rl)
                {
                    if (r.Left >= fromColIndex && r.Left <= toColIndex)
                    {
                        SetCoveredRange(r, false);
                        SetCoveredRange(GridRangeInfo.Cells(r.Top, r.Left, r.Top, r.Left), true);
                    }
                    else if (r.Left < fromColIndex && r.Right >= fromColIndex && r.Right <= toColIndex)
                    {
                        SetCoveredRange(r, false);
                        SetCoveredRange(GridRangeInfo.Cells(r.Top, r.Left, r.Top, r.Left), true);
                        SetCoveredRange(GridRangeInfo.Cells(r.Top, r.Left, r.Bottom, fromColIndex - 1), true);
                    }
                }
            }

            Ranges.RemoveCols(fromColIndex, toColIndex, this.Model.ColCount);
            UpdateList();
            return true;
        }

        /// <summary>
        /// Synchronizes the covered ranges after rows have been moved within the grid.
        /// </summary>
        /// <param name="from">The first row index.</param>
        /// <param name="last">The last row index.</param>
        /// <param name="target">The target row.</param>
        /// <param name="rowCount">The current row count in the grid.</param>
        public void MoveRows(int from, int last, int target, int rowCount)
        {
            Ranges.MoveRows(from, last, target, rowCount);
            UpdateList();
        }

        /// <summary>
        /// Synchronizes the covered ranges after columns have been moved within the grid.
        /// </summary>
        /// <param name="from">The first column index.</param>
        /// <param name="last">The last column index.</param>
        /// <param name="target">The target row.</param>
        /// <param name="colCount">The current column count in the grid.</param>
        public void MoveCols(int from, int last, int target, int colCount)
        {
            Ranges.MoveCols(from, last, target, colCount);
            UpdateList();
        }

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangedEventArgs that contains the event data. </param>
        public void OnChanged(GridCoveredRangesChangedEventArgs e)
        {
            try
            {
                Model.RaiseCoveredRangesChanged(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Raises the Changing event.
        /// </summary>
        /// <param name="e">A GridRowColSizeChangingEventArgs that contains the event data.</param>
        /// <returns>returns boolean value on changing event</returns>
        public bool OnChanging(GridCoveredRangesChangingEventArgs e)
        {
            try
            {
                Model.RaiseCoveredRangesChanging(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                e.Cancel = true;
            }

            return !e.Cancel;
        }
    }
}
