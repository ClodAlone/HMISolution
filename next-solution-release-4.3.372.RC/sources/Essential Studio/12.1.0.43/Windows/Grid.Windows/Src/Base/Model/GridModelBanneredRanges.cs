//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelBanneredRanges.cs" company="syncfusion">
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
    internal class GridModelBanneredRangesEditor : UITypeEditor
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

            GridModelBanneredRanges coveredRanges = (GridModelBanneredRanges)value;
            foreach (GridRangeInfo range in coveredRanges)
            {
                ranges.Add(new GridRangeInfoItem(range));
            }

            GridRangeInfoListEditor dlg = new GridRangeInfoListEditor(ranges);
            dlg.Text = "Bannered Ranges";
            dlg.label1.Text = "Bannered Ranges";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                IComponentChangeService changeService = null;
                PropertyDescriptor pd = null;
                if (provider != null)
                {
                    changeService = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
                    pd = TypeDescriptor.GetProperties(instance)["BanneredRanges"];
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
            GridModelBanneredRanges coveredRanges = ((GridModelBanneredRanges) value);
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
    /// This class manages bannered cell ranges for a grid.
    /// </summary>
    /// <remarks>
    /// You access this class from a grid with the <see cref="GridModel.BanneredRanges"/>
    /// property of a <see cref="GridModel"/> instance.
    /// <para/>
    /// Bannered ranges are saved in two separate collections.
    /// <para/>
    /// The first collection is <see cref="GridRangeInfoList"/>,
    /// which allows quick enumeration through all bannered cell ranges in the grid. This is good when bannered ranges
    /// need to be recalculated because rows or column have been inserted, moved, or removed.
    /// <para/>
    /// The second collection is <see cref="GridBanneredCellPool"/>, which is optimized to look up if a specific cell
    /// is part of a bannered range.
    /// <para/>
    /// The <see cref="GridModel.QueryBanneredRange"/> event in a <see cref="GridModel"/> lets you provide customized
    /// bannered cells ranges at run-time. For example you might want to have a pattern of bannered ranges.
    /// This allows you to customize the grid's default behavior and manage bannered
    /// ranges by your own code and not with the <see cref="GridModelBanneredRanges"/> class.
    /// </remarks>
    [Serializable]
    [Editor(typeof(GridModelBanneredRangesEditor), typeof(UITypeEditor))]
    public sealed class GridModelBanneredRanges : GridModelBound, ICollection, ISerializable, IEnumerable
    {
        // Bannered Cells.
        private GridRangeInfoList banneredRanges = new GridRangeInfoList();
        private GridBanneredCellPool pool = new GridBanneredCellPool();

        /// <overload>
        /// Initializes a new <see cref="GridModelBanneredRanges"/> from a serialization stream.
        /// </overload>
        /// <summary>
        /// Initializes a new <see cref="GridModelBanneredRanges"/> object and associates it
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/></param>.
        public GridModelBanneredRanges(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelBanneredRanges"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelBanneredRanges(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            banneredRanges = (GridRangeInfoList)info.GetValue("Ranges", typeof(GridRangeInfoList));
            pool = (GridBanneredCellPool)info.GetValue("Pool", typeof(GridBanneredCellPool));
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelBanneredRanges"/>.
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
            info.AddValue("Ranges", banneredRanges); // GridRangeInfoList
            info.AddValue("Pool", pool); // GridBanneredCellPool
        }

        /// <summary>
        /// Gets the <see cref="GridRangeInfoList"/> and lets you enumerate through all bannered ranges
        /// managed by this <see cref="GridModelBanneredRanges"/> instance.
        /// </summary>
        public GridRangeInfoList Ranges
        {
            get { return banneredRanges; }
        }

        /// <summary>
        /// Adds a bannered range cell.
        /// </summary>
        /// <param name="range">Cell range.</param>
        public void Add(GridRangeInfo range)
        {
            SetBanneredRange(range, true);
        }

        /// <summary>
        /// Adds multiple bannered range cells.
        /// </summary>
        /// <param name="ranges">The list of ranges to add</param>
        public void AddRange(GridRangeInfo[] ranges)
        {
            foreach (GridRangeInfo range in ranges)
            {
                SetBanneredRange(range, true);
            }
        }

        /// <summary>
        /// Removes bannered cells that are contained in the specified range.
        /// </summary>
        /// <param name="range">The range that you want to clear from bannered cells.</param>
        public void Remove(GridRangeInfo range)
        {
            SetBanneredRange(range, false);
        }

        /// <summary>
        /// Adds or removes a bannered range.
        /// </summary>
        /// <param name="range">The affected range that indicates a new bannered cell or the range of cells where bannered cells should be removed.</param>
        /// <param name="setOrReset">True if you want to add a bannered; False if range should be cleared.</param>
        public void SetBanneredRange(GridRangeInfo range, bool setOrReset)
        {
            GridRangeInfoList ranges = new GridRangeInfoList();
            ranges.Add(range);
            SetBanneredRanges(ranges, setOrReset);
        }

        /// <summary>
        /// Resets all bannered ranges.
        /// </summary>
        public void Clear()
        {
            SetBanneredRange(GridRangeInfo.Table(), false);
        }

        /// <summary>
        /// Adds or removes one or multiple bannered range.
        /// </summary>
        /// <param name="ranges">The list that holds range with new bannered cells or ranges where bannered cells should be removed.</param>
        /// <param name="setOrReset">True if you want to add bannered cells; False if ranges should be cleared.</param>
        public void SetBanneredRanges(GridRangeInfoList ranges, bool setOrReset)
        {
            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, "SetBanneredRanges");
            try
            {
                if (OnChanging(new GridBanneredRangesChangingEventArgs(ranges, setOrReset)))
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

                            //// ... now, I can store the data
                            if (!setOrReset)
                            {
                                this.banneredRanges.GetRangesContained(range).CopyTo(oldRanges, 0);

                                if (pool.ResetSpanCells(cellRange))
                                {
                                    foreach (GridRangeInfo rg in banneredRanges.GetRangesIntersecting(range))
                                    {
                                        banneredRanges.Remove(rg);
                                    }

                                    success = true;
                                }
                            }
                            else
                            {
                                if (!banneredRanges.AnyRangeIntersects(range)
                                    && pool.StoreSpanCells(cellRange))
                                {
                                    banneredRanges.Add(range);
                                    oldRanges.Add(range);
                                    success = true;
                                }
                            }
                        }

                        if (!success)
                        {
                            return; //// nothing done
                        }

                        ////                        if (/*!discardUndo &&*/ model.CommandList.ShouldRecordCommandInfo)
                        ////                            model.CommandList.Add(new GridModelSetBanneredRangesCommand(this, ranges, setOrReset));

                        if (/*!discardUndo &&*/ model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            model.CommandStack.Push(new GridModelSetBanneredRangesCommand(this, oldRanges, !setOrReset));
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
                        OnChanged(new GridBanneredRangesChangedEventArgs(ranges, setOrReset, success));
                    }
                }
            }
            finally
            {
                Model.EndUpdate();
            }
        }

        /// <summary>
        /// Returns an enumerator for stepping through all bannered ranges.
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
        /// Gets the number of ranges in the <see cref="GridModelBanneredRanges"/>.
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
        ///  Gets a value indicating whether IsSynchronized. Returns False.
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

        internal void ResetCache()
        {
            cacheLastCoveredRange = GridRangeInfo.Empty;
        }

        /// <summary>
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the bannered range for the specified cell position
        /// or False if there is no bannered range for the given cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <param name="range">The <see cref="GridRangeInfo"/> where the found bannered range is returned.</param>
        /// <returns>True if a bannered range is at the specified cell position; False if not.</returns>
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
            GridQueryBanneredRangeEventArgs e = new GridQueryBanneredRangeEventArgs(rowIndex, colIndex);
            Model.RaiseQueryBanneredRange(e);
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
        /// Returns a <see cref="GridRangeInfo"/> object that indicates the bannered range for the specified cell position
        /// or <see cref="GridRangeInfo.Empty"/> if there is no bannered range for the given cell position.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="colIndex">The column index.</param>
        /// <returns>A reference to the bannered range is at the specified cell position or
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
        /// Combines all bannered ranges that intersect into one outer range that spans over all found ranges.
        /// </summary>
        /// <param name="range">The original range. </param>
        /// <returns>The <see cref="GridRangeInfo"/> with the outer range.</returns>
        public GridRangeInfo Merge(GridRangeInfo range)
        {
            if (banneredRanges.Count > 0)
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
        /// Synchronizes the <see cref="GridBanneredCellPool"/>. The pool will be emptied and initialized with ranges specified in the <see cref="Ranges"/> collection.
        /// </summary>
        public void UpdateList()
        {
            try
            {
                if (pool == null)
                {
                    pool = new GridBanneredCellPool();
                }

                pool.InitFromRangeList(this.banneredRanges, Model);
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
        /// Synchronizes the bannered ranges after rows have been inserted in the grid.
        /// </summary>
        /// <param name="insertAt">The starting row index where new rows should be inserted.</param>
        /// <param name="count">The number of rows to insert.</param>
        public void InsertRows(int insertAt, int count)
        {
            Ranges.InsertRows(insertAt, count);
            UpdateList();
        }

        /// <summary>
        /// Synchronizes the bannered ranges after columns have been inserted in the grid.
        /// </summary>
        /// <param name="insertAt">The starting column's index where new columns should be inserted.</param>
        /// <param name="count">The number of rows to insert.</param>
        public void InsertCols(int insertAt, int count)
        {
            Ranges.InsertCols(insertAt, count);
            UpdateList();
        }

        /// <summary>
        /// Synchronizes the bannered ranges after rows have been removed from the grid.
        /// </summary>
        /// <param name="fromRowIndex">The first row index.</param>
        /// <param name="toRowIndex">The last row index.</param>
        /// <returns>True if any bannered ranges were affected; False if no range needed to be changed.</returns>
        public bool RemoveRows(int fromRowIndex, int toRowIndex)
        {
            Ranges.RemoveRows(fromRowIndex, toRowIndex, this.Model.RowCount);
            UpdateList();
            return true;
        }

        /// <summary>
        /// Synchronizes the bannered ranges after columns have been removed from the grid.
        /// </summary>
        /// <param name="fromColIndex">The first column index.</param>
        /// <param name="toColIndex">The last column index.</param>
        /// <returns>True if any bannered ranges were affected; False if no range needed to be changed.</returns>
        public bool RemoveCols(int fromColIndex, int toColIndex)
        {
            Ranges.RemoveCols(fromColIndex, toColIndex, this.Model.ColCount);
            UpdateList();
            return true;
        }

        /// <summary>
        /// Synchronizes the bannered ranges after rows have been moved within the grid.
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
        /// Synchronizes the bannered ranges after columns have been moved within the grid.
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
        public void OnChanged(GridBanneredRangesChangedEventArgs e)
        {
            try
            {
                Model.RaiseBanneredRangesChanged(e);
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
        /// <returns>return boolean value to indicate changing</returns>
        public bool OnChanging(GridBanneredRangesChangingEventArgs e)
        {
            try
            {
                Model.RaiseBanneredRangesChanging(e);
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
