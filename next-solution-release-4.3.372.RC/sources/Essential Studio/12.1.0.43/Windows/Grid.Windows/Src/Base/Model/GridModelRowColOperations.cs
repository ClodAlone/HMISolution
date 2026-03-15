//-------------------------------------------------------------------------------------------------
// <copyright file="GridModelRowColOperations.cs" company="syncfusion">
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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;

using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Drawing;
using Syncfusion.Styles;
using Syncfusion.Windows.Forms;

namespace Syncfusion.Windows.Forms.Grid
{
    /// <summary>
    /// Abstract class for row and column operations in the grid. Allows you to insert, move, remove rows, and more.
    /// </summary>
    /// <remarks>
    /// See <see cref="GridModelRowOperations"/> for row operations and <see cref="GridModelColOperations"/> for column operations.
    /// <para/>
    /// You typically do not derive or instantiate this class. The <see cref="GridModel"/> class instantiates
    /// objects of this class. You can access all members of this class through the <see cref="GridModel.Rows"/> and
    /// <see cref="GridModel.Cols"/> objects.
    /// </remarks>
    [Serializable]
    public abstract class GridModelRowColOperations : GridModelBound, ISerializable
    {
        // Fields
        private int/*float*/ maxSize = 1024;
        private int/*float*/ defaultSize = 0;
        private int headerCount = 0;
        private int frozenCount = 0;

        // Events

        /// <summary>
        /// Raises the <see cref="GridModel.DefaultRowHeightChanging"/> or <see cref="GridModel.OnDefaultColWidthChanging"/> event in the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data.</param>
        /// <returns>True if operation can proceed; False if it should abort.</returns>
        public abstract bool OnDefaultSizeChanging(GridDefaultSizeChangingEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridModel.OnDefaultRowHeightChanged"/> or <see cref="GridModel.OnDefaultColWidthChanged"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data</param>
        public abstract void OnDefaultSizeChanged(GridDefaultSizeChangedEventArgs e);

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract bool OnHeaderCountChanging(GridCountChangingEventArgs e);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void OnHeaderCountChanged(GridCountChangedEventArgs e);

        /// <summary>
        /// Used internally.
        /// </summary>
        /// <returns>returns boolean value</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract bool OnFrozenCountChanging(GridCountChangingEventArgs e);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void OnFrozenCountChanged(GridCountChangedEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridModel.OnColsMoving"/> or <see cref="GridModel.OnRowsMoving"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data.</param>
        /// <returns>True if operation can proceed; False if it should abort.</returns>
        public abstract bool OnRangeMoving(GridRangeMovingEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridModel.OnColsMoved"/> or <see cref="GridModel.OnRowsMoved"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data.</param>
        public abstract void OnRangeMoved(GridRangeMovedEventArgs e);

        /// <summary>
        /// Updates internal structures before removing rows or columns.
        /// </summary>
        /// <param name="e">Event data.</param>
        /// <returns>True if operation can proceed; False if it should abort.</returns>
        public abstract bool OnRangeRemoving(GridRangeRemovingEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridModel.OnColsRemoving"/> or <see cref="GridModel.OnRowsRemoving"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data.</param>
        /// <returns>True if operation can proceed; False if it should abort.</returns>
        protected abstract bool RaiseRemoving(GridRangeRemovingEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridModel.OnColsRemoved"/> or <see cref="GridModel.OnRowsRemoved"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data.</param>
        public abstract void OnRangeRemoved(GridRangeRemovedEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridModel.OnColsInserting"/> or <see cref="GridModel.OnRowsInserting"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data.</param>
        /// <returns>True if operation can proceed; False if it should abort.</returns>
        public abstract bool OnRangeInserting(GridRangeInsertingEventArgs e);

        /// <summary>
        /// Raises the <see cref="GridModel.OnColsInserted"/> or <see cref="GridModel.OnRowsInserted"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">Event data.</param>
        public abstract void OnRangeInserted(GridRangeInsertedEventArgs e);

        ////[Syncfusion.Documentation.DocumentationExclude()]

        /// <summary>
        /// Initializes a <see cref="GridModelRowColOperations"/> and associates it 
        /// with a <see cref="GridModel"/>.
        /// </summary>
        /// <param name="model">A reference to the parent <see cref="GridModel"/>.</param>
        protected/*internal*/ GridModelRowColOperations(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelRowColOperations"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        protected/*internal*/ GridModelRowColOperations(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            maxSize = info.GetInt32("MaxSize");
            defaultSize = info.GetInt32("DefaultSize");
            headerCount = info.GetInt32("HeaderCount");
            frozenCount = info.GetInt32("FrozenCount");
        }

        /// <summary>
        /// Implements the ISerializable interface and returns the data needed to serialize the <see cref="GridModelRowColOperations"/>.
        /// </summary>
        /// <param name="info">A SerializationInfo object containing the information required to serialize the object.</param>
        /// <param name="context">A StreamingContext object containing the source and destination of the serialized stream.</param>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
            info.AddValue("MaxSize", maxSize); // Int32
            info.AddValue("DefaultSize", defaultSize); // Int32
            info.AddValue("HeaderCount", headerCount); // Int32
            info.AddValue("FrozenCount", frozenCount); // Int32
        }

        // Abstract methods

        /// <summary>
        /// Changes the cell contents at a specific row or column index.
        /// </summary>
        /// <param name="index">The row (or column) index.</param>
        /// <param name="data">A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</param>
        /// <seealso cref="GridModelRowColOperations.GetCells(int,int)"/>
        public void SetCells(int index, GridStyleInfoStoreTable data)
        {
            SetCells(index, data, true, true);
        }

        /// <summary>
        /// Changes the cell contents at a specific row or column index.
        /// </summary>
        /// <param name="index">The row (or column) index.</param>
        /// <param name="data">A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</param>
        /// <param name="extendRows">True if appending rows is allowed if data holds more rows than fit into the grid.</param>
        /// <param name="extendCols">True if appending columns is allowed if data holds more columns than fit into the grid.</param>
        /// <seealso cref="GridModelRowColOperations.GetCells(int,int)"/>
        public abstract void SetCells(int index, GridStyleInfoStoreTable data, bool extendRows, bool extendCols);
        
        /// <summary>
        /// Gets a table that represents a range of row (or columns).
        /// </summary>
        /// <param name="from">The first row (or column) index.</param>
        /// <param name="last">The last row (or column) index.</param>
        /// <returns>A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</returns>
        /// <seealso cref="GridModelRowColOperations.SetCells(int,Syncfusion.Windows.Forms.Grid.GridStyleInfoStoreTable)"/>
        public GridStyleInfoStoreTable GetCells(int from, int last)
        {
            return GetCells(from, last, true, true);
        }

        /// <summary>
        /// Gets a table that represents a range of row (or columns).
        /// </summary>
        /// <param name="from">The first row (or column) index.</param>
        /// <param name="last">The last row (or column) index.</param>
        /// <param name="getRowColStyle">True if row or column parent styles should be copied.</param>
        /// <param name="copyCells">True if cell contents should be copied.</param>
        /// <returns>A <see cref="GridStyleInfoStoreTable"/> object that holds contents for all the cells.</returns>
        /// <seealso cref="GridModelRowColOperations.SetCells(int,Syncfusion.Windows.Forms.Grid.GridStyleInfoStoreTable)"/>
        public abstract GridStyleInfoStoreTable GetCells(int from, int last, bool getRowColStyle, bool copyCells);

        /// <summary>
        /// Gets a reference to <see cref="GridModel.RowHeights"/> or <see cref="GridModel.ColWidths"/>.
        /// </summary>
        public abstract GridModelRowColSizeIndexer Size { get; }

        /// <summary>
        /// Gets a reference to <see cref="GridModel.HideRows"/> or <see cref="GridModel.HideCols"/>.
        /// </summary>
        public abstract GridModelHideRowColsIndexer Hidden { get; }

        /// <summary>
        /// Force recalculation of floating cell's state for the specified range of rows (or columns).
        /// </summary>
        /// <param name="from">The first row (or column) index.</param>
        /// <param name="last">The last row (or column) index.</param>
        public abstract void DelayFloatingCells(int from, int last);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected/*internal*/ abstract void DataRemoveRange(int removeAt, int count);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected/*internal*/ abstract void DataInsertRange(int insertAt, int count);

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected/*internal*/ abstract void DataMoveRange(int from, int count, int target);

        /// <summary>
        /// Gets "Row" or "Col" string.
        /// </summary>
        public abstract string RowColName { get; }

        // Constructor

        /// <summary>
        /// Gets or sets the maximum row height (or column width).
        /// </summary>
        public int/*float*/ MaxSize
        {
            get
            {
                return maxSize;
            }

            set
            {
                maxSize = value;
            }
        }

        // Default size of row or column.

        /// <summary>
        /// Gets or sets the default row height (or column width).
        /// </summary>
        public int/*float*/ DefaultSize
        {
            get
            {
                return defaultSize;
            }

            set
            {
                Model.NotifyChangingLayoutCells(GridRangeInfo.Table());
                Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, this.RowColName + ".DefaultSize");
                try
                {
                    if (OnDefaultSizeChanging(new GridDefaultSizeChangingEventArgs(value)))
                    {
                        bool success = false;
                        int/*float*/ savedValue = DefaultSize;
                        try
                        {
                            defaultSize = value;
                            success = true;
                            Model.FloatingCells.DelayFloatCells(GridRangeInfo.Table());
                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                model.CommandStack.Push(new GridModelSetDefaultSizeCommand(this, savedValue));
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
                            OnDefaultSizeChanged(new GridDefaultSizeChangedEventArgs(savedValue, success));
                        }
                    }
                }
                finally
                {
                    Model.EndUpdate();
                    Model.NotifyChangedLayoutCells(GridRangeInfo.Table());
                }
            }
        }

        /// <summary>
        /// Gets / sets the number of header rows or columns without raising <see cref="GridModel.HeaderRowCountChanging"/>, <see cref="GridModel.HeaderColCountChanging"/>, 
        /// <see cref="GridModel.HeaderRowCountChanged"/>, and <see cref="GridModel.HeaderColCountChanged"/> events.
        /// </summary>
        /// <param name="value">The new count.</param>
        /// <param name="raiseEvents">Specifies if <see cref="GridModel.HeaderRowCountChanging"/>, <see cref="GridModel.HeaderColCountChanging"/>, 
        /// <see cref="GridModel.HeaderRowCountChanged"/>, and <see cref="GridModel.HeaderColCountChanged"/> events should be raised.</param>
        public void SetHeaderCount(int value, bool raiseEvents)
        {
            if (raiseEvents)
            {
                HeaderCount = value;
            }
            else
            {
                headerCount = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the number of row (or column) headers.
        /// </summary>
        public int HeaderCount
        {
            get
            {
                return headerCount;
            }

            set
            {
                if (HeaderCount == value)
                {
                    return;
                }

                bool success = false;
                Model.NotifyChangingLayoutCells(GridRangeInfo.Table());
                Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, this.RowColName + ".HeaderCount");
                try
                {
                    if (OnHeaderCountChanging(new GridCountChangingEventArgs(value)))
                    {
                        int savedValue = HeaderCount;
                        try
                        {
                            Model.ResetVolatileData();
                            DelayFloatingCells(Math.Min(savedValue, value), Math.Max(savedValue, value) + 1);
                            headerCount = value;
                            success = true;
                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                model.CommandStack.Push(new GridModelSetHeaderCountCommand(this, savedValue));
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
                            OnHeaderCountChanged(new GridCountChangedEventArgs(savedValue, success));
                        }
                    }
                }
                finally
                {
                    Model.EndUpdate();
                    Model.NotifyChangedLayoutCells(GridRangeInfo.Table());
                }
            }
        }

        // Number of fixed rows or columns.
        [NonSerialized]
        int restoreFreezeFrom = 0;
        [NonSerialized]
        int restoreFreezeCount = 0;
        [NonSerialized]
        int restoreFreezeDest = 0;

        /// <summary>
        /// Queries selected row or column ranges.
        /// </summary>
        /// <returns>Returns a <see cref="GridRangeInfoList"/> with all selected rows (or columns).</returns>
        public abstract GridRangeInfoList GetSelectedRowColRanges();

        /// <internalonly/>
        /// <summary>Used internally.</summary>
        [Syncfusion.Documentation.DocumentationExclude()]
        protected abstract void GetRangeFromTo(GridRangeInfo range, out int from, out int last);

        /// <summary>
        /// Used internally.
        /// </summary>    
        /// <returns>returns GridRangeInfo</returns>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public abstract GridRangeInfo CreateRangeFromTo(int from, int last);

        /// <summary>
        /// Freeze the current selected range of rows (or columns). Moves them to the start of the grid and sets <see cref="FrozenCount"/>.
        /// </summary>
        public void FreezeSelection()
        {
            if (FrozenCount == 0)
            {
                GridRangeInfoList ranges = GetSelectedRowColRanges();
                if (ranges.Count == 1)
                {
                    int from;
                    int last;
                    GetRangeFromTo(ranges[0], out from, out last);
                    FreezeRange(from, last);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the grid has a current selected range that can be frozen.
        /// </summary>
        public bool CanFreezeSelection
        {
            get
            {
                return FrozenCount == HeaderCount && GetSelectedRowColRanges().Count == 1;
            }
        }

        /// <summary>
        /// Freeze the specified range of rows (or columns). Moves them to the start of the grid and sets <see cref="FrozenCount"/>.
        /// </summary>
        /// <param name="from">The first row (or column) index.</param>
        /// <param name="last">The last row (or column) index.</param>
        public void FreezeRange(int from, int last)
        {
            Model.NotifyChangingLayoutCells(GridRangeInfo.Table());
            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, this.RowColName + ".FreezeRange");
            try
            {
                Model.CommandStack.BeginTrans(this.RowColName + ".FreezeRange");
                int headerCount = HeaderCount;
                int count = last - from + 1;
                this.FrozenCount = count + headerCount;
                if (from > headerCount + 1)
                {
                    MoveRange(from, count, headerCount + 1);
                }

                restoreFreezeFrom = headerCount + 1;
                restoreFreezeCount = count;
                restoreFreezeDest = from;
                Model.CommandStack.CommitTrans();
            }
            finally
            {
                Model.EndUpdate();
                Model.NotifyChangedLayoutCells(GridRangeInfo.Table());
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is a frozen range of rows (or columns) that can be unfrozen.
        /// </summary>
        public bool CanRestoreFrozen
        {
            get
            {
                return FrozenCount > HeaderCount && restoreFreezeCount > 0;
            }
        }

        /// <summary>
        /// Unfreezes a previously frozen range of rows (or columns) and moves the rows or columns back to original position.
        /// </summary>
        public void RestoreFrozen()
        {
            if (!CanRestoreFrozen)
            {
                return;
            }

            Model.NotifyChangingLayoutCells(GridRangeInfo.Table());
            Model.BeginUpdate(BeginUpdateOptions.None);
            try
            {
                Model.CommandStack.BeginTrans(SR.GetString("Restore Frozen")); // TODO: SR
                int headerCount = HeaderCount;
                this.FrozenCount = headerCount;
                if (restoreFreezeFrom != restoreFreezeDest)
                {
                    this.MoveRange(restoreFreezeFrom, restoreFreezeCount, restoreFreezeDest);
                }

                Model.CommandStack.CommitTrans();
            }
            finally
            {
                Model.EndUpdate();
                Model.NotifyChangedLayoutCells(GridRangeInfo.Table());
            }
        }

        /// <summary>
        /// Gets / sets the number of frozen rows or columns without raising <see cref="GridModel.FrozenRowCountChanging"/>, <see cref="GridModel.FrozenColCountChanging"/>, 
        /// <see cref="GridModel.FrozenRowCountChanged"/>, and <see cref="GridModel.FrozenColCountChanged"/> events.
        /// </summary>
        /// <param name="value">The new count.</param>
        /// <param name="raiseEvents">Specifies if <see cref="GridModel.FrozenRowCountChanging"/>, <see cref="GridModel.FrozenColCountChanging"/>, 
        /// <see cref="GridModel.FrozenRowCountChanged"/>, and <see cref="GridModel.FrozenColCountChanged"/> events should be raised.</param>
        public void SetFrozenCount(int value, bool raiseEvents)
        {
            if (raiseEvents)
            {
                FrozenCount = value;
            }
            else
            {
                frozenCount = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of frozen rows (or columns).
        /// </summary>
        public int FrozenCount
        {
            get
            {
                return frozenCount;
            }

            set
            {
                if (value == FrozenCount)
                {
                    return;
                }

                if (value < 0)
                {
                    value = 0;
                }
                ////throw new ArgumentOutOfRangeException("Value must be greater or equal than 0");

                bool success = false;
                Model.NotifyChangingLayoutCells(GridRangeInfo.Table());
                Model.BeginUpdate(BeginUpdateOptions.None, this.RowColName + ".FrozenCount");
                try
                {
                    if (OnFrozenCountChanging(new GridCountChangingEventArgs(value)))
                    {
                        int savedValue = FrozenCount;
                        try
                        {
                            DelayFloatingCells(Math.Min(savedValue, value), Math.Max(savedValue, value) + 1);
                            frozenCount = value;
                            success = true;
                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                model.CommandStack.Push(new GridModelSetFrozenCountCommand(this, savedValue));
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
                            OnFrozenCountChanged(new GridCountChangedEventArgs(savedValue, success));
                        }
                    }
                }
                finally
                {
                    Model.EndUpdate();
                    Model.NotifyChangedLayoutCells(GridRangeInfo.Table());
                }
            }
        }

        // RangeRemoving rows or columns.

        /// <summary>
        /// Removes a range of rows or columns.
        /// </summary>
        /// <param name="from">The first row or column index.</param>
        /// <param name="last">The last row or column index.</param>
        /// <remarks>
        /// <see cref="RemoveRange"/> checks <see cref="GridModelCommandManager.ShouldGenerateUndoInfo"/>
        /// and generates undo information if necessary.
        /// </remarks>
        public void RemoveRange(int from, int last)
        {
            bool success = false;
            Model.NotifyChangingLayoutCells(this.CreateRangeFromTo(from, GridConstants.MaxRowCol));
            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, this.RowColName + ".RemoveRange");
            try
            {
                GridRangeRemovingEventArgs e = new GridRangeRemovingEventArgs(from, last);
                if (RaiseRemoving(e))
                {
                    GridModelInsertRangeOptions iro = null;

                    OperationFeedback op = new OperationFeedback(this.model);
                    op.Description = SR.GetString("DescriptionRemove" + this.RowColName, from, last);
                    op.Name = "Remove" + this.RowColName;

                    model.ResetVolatileData();

                    try
                    {
                        int count = last - from + 1;

                        ////
                        //// Old row heights are needed either for efficient
                        //// updating of the model when rows inside the model are deleted
                        //// (and not the last rows)
                        //// - Or -
                        //// when undo information is created last restore the old row heights
                        //// if not is locked updated && last+1 < row count.
                        iro = new GridModelInsertRangeOptions();
                        iro.rowColSizes = Size.GetRange(from, last);
                        iro.RowColHide = Hidden.GetRange(from, last);
                        //// TODO: Covered Cells

                        if (model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            iro.data = GetCells(from, last, true, true);
                        }

                        if (OnRangeRemoving(e))
                        {
                            DataRemoveRange(from, count);

                            success = true;

                            ////                        if (model.CommandList.ShouldRecordCommandInfo)
                            ////                            model.CommandList.Add(new GridModelRemoveRangeCommand(this, from, last));

                            if (model.CommandStack.ShouldGenerateUndoInfo)
                            {
                                model.CommandStack.Push(new GridModelInsertRangeCommand(this, from, count, iro));
                            }

                            model.SelectedRanges.EnsureRowLimits(Model.RowCount);
                            model.SelectedRanges.EnsureColLimits(Model.ColCount);
                            model.SelectedRanges.RemoveEmptyRanges();
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
                        OnRangeRemoved(new GridRangeRemovedEventArgs(from, last, iro, success));
                        op.Close();
                    }
                }
            }
            finally
            {
                Model.EndUpdate();
                Model.NotifyChangedLayoutCells(this.CreateRangeFromTo(from, GridConstants.MaxRowCol));
            }
        }

        // Inserting rows or columns.

        /// <overload>
        /// Inserts a range of rows or columns at a specified index.
        /// </overload>
        /// <summary>
        /// Inserts a range of rows or columns at a specified index.
        /// </summary>
        /// <param name="insertAt">The index where rows or columns should be inserted.</param>
        /// <param name="count">The number of rows or columns to insert.</param>
        /// <remarks>
        /// <see cref="InsertRange(int,int)"/> checks <see cref="GridModelCommandManager.ShouldGenerateUndoInfo"/>
        /// and generates undo information if necessary.
        /// </remarks>
        public void InsertRange(int insertAt, int count)
        {
            InsertRange(insertAt, count, new GridModelInsertRangeOptions());
        }

        /// <summary>
        /// Inserts a range of rows or columns at a specified index.
        /// </summary>
        /// <param name="insertAt">The index where rows or columns should be inserted.</param>
        /// <param name="count">The number of rows or columns to insert.</param>
        /// <param name="iro">Specifies cell contents, row, and columns heights, hidden state, and covered cells information.</param>
        /// <genoverload/>
        public void InsertRange(int insertAt, int count, GridModelInsertRangeOptions iro)
        {
            bool success = false;
            Model.NotifyChangingLayoutCells(this.CreateRangeFromTo(insertAt, GridConstants.MaxRowCol));
            Model.BeginUpdate(BeginUpdateOptions.InvalidateAndScroll, this.RowColName + ".InsertRange(int,int)");
            try
            {
                if (OnRangeInserting(new GridRangeInsertingEventArgs(insertAt, count, iro)))
                {
                    OperationFeedback op = new OperationFeedback(this.model);
                    op.Description = SR.GetString("DescriptionInsert" + RowColName, insertAt, count);
                    op.Name = "Insert" + RowColName;

                    model.ResetVolatileData();

                    try
                    {
                        DataInsertRange(insertAt, count);

                        success = true;

                        if (iro != null)
                        {
                            if (iro.data != null)
                            {
                                this.SetCells(insertAt, iro.data, false, false);
                            }

                            if (iro.rowColSizes != null)
                            {
                                this.Size.SetRange(insertAt, insertAt + count - 1, iro.rowColSizes, true);
                            }

                            if (iro.RowColHide != null)
                            {
                                this.Hidden.SetRange(insertAt, insertAt + count - 1, iro.RowColHide, true);
                            }
                        }

                        ////                        if (model.CommandList.ShouldRecordCommandInfo)
                        ////                            model.CommandList.Add(new GridModelInsertRangeCommand(this, insertAt, count, iro));

                        if (model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            model.CommandStack.Push(new GridModelRemoveRangeCommand(this, insertAt, insertAt + count - 1));
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
                        OnRangeInserted(new GridRangeInsertedEventArgs(insertAt, count, iro, success));
                        op.Close();
                    }
                }
            }
            finally
            {
                Model.EndUpdate();
                Model.NotifyChangedLayoutCells(this.CreateRangeFromTo(insertAt, GridConstants.MaxRowCol));
            }
        }

        // Moving rows or columns.

        /// <overload>
        /// Moves a range of rows or columns.
        /// </overload>
        /// <summary>
        /// Moves a range of rows or columns.
        /// </summary>
        /// <param name="from">The first row or column index.</param>
        /// <param name="target">The destination row or column index.</param>
        /// <remarks>
        /// <see cref="MoveRange(int,int)"/> checks <see cref="GridModelCommandManager.ShouldGenerateUndoInfo"/>
        /// and generates undo information if necessary.
        /// </remarks>
        public void MoveRange(int from, int target)
        {
            MoveRange(from, 1, target);
        }

        /// <summary>
        /// Moves a range of rows or columns.
        /// </summary>
        /// <param name="from">The first row or colum index.</param>
        /// <param name="count">The number of rows or columns to move.</param>
        /// <param name="target">The destination row or column index.</param>
        /// <genoverload/>
        public void MoveRange(int from, int count, int target)
        {
            if (from == target)
            {
                return;
            }

            bool success = false;
            Model.NotifyChangingLayoutCells(this.CreateRangeFromTo(Math.Min(from, target), GridConstants.MaxRowCol));
            Model.BeginUpdate(BeginUpdateOptions.None, this.RowColName + ".MoveRange(int,int)");
            try
            {
                if (OnRangeMoving(new GridRangeMovingEventArgs(from, count, target)))
                {
                    OperationFeedback op = new OperationFeedback(this.model);
                    op.Description = SR.GetString("DescriptionMove" + RowColName, from, count, target);
                    op.Name = "Move(Syncfusion.Windows.Forms.Grid.GridDirectionType,int,bool)" + RowColName;

                    model.ResetVolatileData();

                    try
                    {
                        DataMoveRange(from, count, target);

                        success = true;

                        if (model.CommandStack.ShouldGenerateUndoInfo)
                        {
                            model.CommandStack.Push(new GridModelMoveRangeCommand(this, target, count, from));
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
                        OnRangeMoved(new GridRangeMovedEventArgs(from, count, target, success));
                        op.Close();
                    }
                }
            }
            finally
            {
                Model.EndUpdate();
                Model.NotifyChangedLayoutCells(this.CreateRangeFromTo(Math.Min(from, target), GridConstants.MaxRowCol));
            }
        }
    }

    [Serializable]
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class GridModelRowOperations : GridModelRowColOperations
    {
        [NonSerialized]
        const int delta = 1;

        /// <summary>
        /// Initializes a new <see cref="GridModelRowOperations"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelRowOperations(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        /// <summary>
        /// Initializes a new <see cref="GridModelRowOperations"/> and attaches it to a <see cref="GridModel"/>.
        /// </summary>
        public GridModelRowOperations(GridModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Returns "Row" string.
        /// </summary>
        public override string RowColName
        {
            get { return "Row"; }
        }

        /// <summary>
        /// Returns <see cref="GridModel.RowHeights"/> from the <see cref="GridModel"/> object.
        /// </summary>
        public override GridModelRowColSizeIndexer Size
        {
            get { return model.RowHeights; }
        }

        /// <summary>
        /// Returns <see cref="GridModel.HideRows"/> from the <see cref="GridModel"/> object.
        /// </summary>
        public override GridModelHideRowColsIndexer Hidden
        {
            get { return model.HideRows; }
        }

        /// <summary>
        /// Raises <see cref="GridModel.DefaultRowHeightChanging"/> event in  the <see cref="GridModel"/> object.
        /// </summary>
        /// <param name="e">The GridDefaultSizeChangingEventArgs</param>
        /// <returns>returns boolean value</returns>
        public override bool OnDefaultSizeChanging(GridDefaultSizeChangingEventArgs e)
        {            
            try
            {
                Model.RaiseDefaultRowHeightChanging(e);
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

        public override void OnDefaultSizeChanged(GridDefaultSizeChangedEventArgs e)
        {
            try
            {
                Model.RaiseDefaultRowHeightChanged(e);
            }
            catch (Exception ex)
            {
                TraceUtil.TraceExceptionCatched(ex);
                if (!ExceptionManager.RaiseExceptionCatched(this, ex))
                {
                    throw;
                }

                TraceUtil.TraceExceptionCatched(ex);
            }
        }

        protected/*internal*/ override bool OnHeaderCountChanging(GridCountChangingEventArgs e)
        {
            try
            {
                Model.RaiseHeaderRowCountChanging(e);
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

        protected/*internal*/ override void OnHeaderCountChanged(GridCountChangedEventArgs e)
        {
            try
            {
                Model.RaiseHeaderRowCountChanged(e);
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

        protected/*internal*/ override bool OnFrozenCountChanging(GridCountChangingEventArgs e)
        {
            try
            {
                Model.RaiseFrozenRowCountChanging(e);
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

        protected/*internal*/ override void OnFrozenCountChanged(GridCountChangedEventArgs e)
        {
            try
            {
                Model.RaiseFrozenRowCountChanged(e);
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

        bool RaiseMoving(GridRangeMovingEventArgs e)
        {
            try
            {
                Model.RaiseRowsMoving(e);
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

        public override void OnRangeMoved(GridRangeMovedEventArgs e)
        {
            try
            {
                Model.RaiseRowsMoved(e);
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

        protected override bool RaiseRemoving(GridRangeRemovingEventArgs e)
        {
            try
            {
                Model.RaiseRowsRemoving(e);
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

        public override void OnRangeRemoved(GridRangeRemovedEventArgs e)
        {
            try
            {
                Model.RaiseRowsRemoved(e);
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

        bool RaiseInserting(GridRangeInsertingEventArgs e)
        {
            try
            {
                Model.RaiseRowsInserting(e);
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

        public override void OnRangeInserted(GridRangeInsertedEventArgs e)
        {
            try
            {
                Model.RaiseRowsInserted(e);
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

        public override GridStyleInfoStoreTable GetCells(int from, int lastCell, bool rangeStyles, bool cellStyles)
        {
            using (OperationFeedback op = new OperationFeedback(model))
            {
                GridStyleInfoStoreTable data;
                int rowCount = lastCell - from + 1;
                int colCount = cellStyles ? model.ColCount + delta : 0; // model does not count column 0 
                int last = colCount - delta;
                int first = rangeStyles ? -1 : 0;
                int total = rowCount * (last - first + 1);
                data = new GridStyleInfoStoreTable(rowCount, colCount);
                int count = 0;
                if (total > 0)
                {
                    // Store styles in array, but allow user lastCell abort
                    for (int r = 0; r < rowCount; r++)
                    {
                        for (int c = first; c <= last; c++)
                        {
                            ////GridStyleInfo style = model[from+r, c];
                            ////data[r, c] = (GridStyleInfoStore) style.Store.Clone();
                            GridStyleInfo style = new GridStyleInfo();
                            model.GetCellInfo(from + r, c, style);
                            data[r, c] = (GridStyleInfoStore)style.Store;
                            op.PercentComplete = (int)((++count) * 100 / total);
                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }
                    }
                }

                return data;
            }
        }

        public override void SetCells(int index, GridStyleInfoStoreTable data, bool extendRows, bool extendCols)
        {
            Model.NotifyChangingLayoutCells(GridRangeInfo.Rows(index, index + data.RowCount));
            OperationFeedback op = new OperationFeedback(model);

            // TODO: undo info?
            try
            {
                // TODO: op.Description
                int dataRowCount = data.RowCount;
                int dataColCount = data.ColCount;
                int gridRowCount = model.RowCount + delta;
                int gridColCount = model.ColCount + delta;

                if (dataRowCount + index > gridRowCount)
                {
                    if (extendRows)
                    {
                        model.Rows.InsertRange(gridRowCount + 1, dataRowCount + index - gridRowCount);
                        gridRowCount = model.RowCount + delta;
                    }

                    if (dataRowCount + index > gridRowCount)
                    {
                        dataRowCount = gridRowCount - index;
                    }
                }

                if (dataColCount > gridColCount)
                {
                    if (extendCols)
                    {
                        model.Cols.InsertRange(gridColCount + 1, dataColCount - gridColCount);
                        gridColCount = model.ColCount + delta;
                    }

                    if (dataColCount > gridColCount)
                    {
                        dataColCount = gridColCount;
                    }
                }

                int last = dataColCount - delta;
                int first = -1;
                int total = dataRowCount * (last - first + 1);
                int count = 0;
                if (total > 0)
                {
                    // Store styles in array, but allow user to abort
                    for (int r = 0; r < dataRowCount; r++)
                    {
                        for (int c = first; c <= last; c++)
                        {
                            GridStyleInfoStore store = data[r, c];
                            if (store != null)
                            {
                                GridStyleInfo style = new GridStyleInfo(store);
                                model.SetCellInfo(index + r, c, style, StyleModifyType.Copy);
                            }
                            else
                            {  // TODO: option to skip null cells
                                model.SetCellInfo(index + r, c, null, StyleModifyType.Copy);
                            }

                            op.PercentComplete = (int)((++count) * 100 / total);
                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }
                    }
                }
            }
            finally
            {
                op.Close();
                Model.NotifyChangedLayoutCells(GridRangeInfo.Rows(index, index + data.RowCount));
            }
        }

        protected/*internal*/ override void DataRemoveRange(int removeAt, int count)
        {
            model.Data.RemoveRows(removeAt, count);
        }

        protected/*internal*/ override void DataInsertRange(int insertAt, int count)
        {
            model.Data.InsertRows(insertAt, count);
        }

        protected/*internal*/ override void DataMoveRange(int from, int count, int target)
        {
            model.Data.MoveRows(from, count, target);
        }

        public override void DelayFloatingCells(int from, int to)
        {
            Model.FloatingCells.DelayFloatCells(GridRangeInfo.Rows(from, to));
        }

        /// <override/>
        protected override void GetRangeFromTo(GridRangeInfo range, out int from, out int last)
        {
            from = range.Top;
            last = range.Bottom;
        }

        public override GridRangeInfo CreateRangeFromTo(int from, int to)
        {
            return GridRangeInfo.Rows(from, to);
        }

        public override GridRangeInfoList GetSelectedRowColRanges()
        {
            return Model.SelectedRanges.GetRowRanges(GridRangeInfoType.Rows);
        }

        public override bool OnRangeInserting(GridRangeInsertingEventArgs e)
        {
            if (RaiseInserting(e))
            {
                int insertAt = e.InsertAt;
                int count = e.Count;
                if (insertAt <= model.RowCount)
                {
                    Hidden.Dictionary.InsertItems(insertAt, count);
                    Size.Dictionary.InsertItems(insertAt, count);
                    Model.FloatingCells.InsertRows(insertAt, count);
                    Model.SelectedRanges.InsertRows(insertAt, count);
                    Model.CoveredRanges.InsertRows(insertAt, count);
                    Model.BanneredRanges.InsertRows(insertAt, count);
                    Model.FloatingCells.InsertRows(insertAt, count);
                    Model.FloatingCells.DelayFloatCells(GridRangeInfo.Rows(insertAt, Model.RowCount));
                    Model.MergeCells.DelayMergeCells(GridRangeInfo.Rows(insertAt, Model.RowCount));
                }

                return true;
            }

            return false;
        }

        public override bool OnRangeMoving(GridRangeMovingEventArgs e)
        {
            if (RaiseMoving(e))
            {
                int from = e.From;
                int dest = e.Target;
                int count = e.Count;
                int to = from + count - 1;
                int rowCount = Model.RowCount;

                Hidden.Dictionary.MoveItems(from, count, dest);
                Size.Dictionary.MoveItems(from, count, dest);
                Model.SelectedRanges.MoveRows(from, to, dest, rowCount);
                Model.CoveredRanges.MoveRows(from, to, dest, rowCount);
                Model.BanneredRanges.MoveRows(from, to, dest, rowCount);
                Model.FloatingCells.MoveRows(from, to, dest);
                int min = Math.Min(from, to);
                int max = Math.Max(to, dest + count);
                Model.FloatingCells.DelayFloatCells(GridRangeInfo.Rows(min, max));
                Model.MergeCells.DelayMergeCells(GridRangeInfo.Rows(min, max));
                return true;
            }

            return false;
        }

        public override bool OnRangeRemoving(GridRangeRemovingEventArgs e)
        {
            if (RaiseRemoving(e))
            {
                int from = e.From;
                int to = e.To;
                int count = to - from + 1;
                Hidden.Dictionary.RemoveItems(from, count);
                Size.Dictionary.RemoveItems(from, count);
                int rowCount = Model.RowCount;
                Model.SelectedRanges.RemoveRows(from, to, rowCount);
                Model.CoveredRanges.RemoveRows(from, to);
                Model.BanneredRanges.RemoveRows(from, to);
                Model.FloatingCells.RemoveRows(from, rowCount);
                Model.MergeCells.DelayMergeCells(GridRangeInfo.Rows(from, rowCount));
                return true;
            }

            return false;
        }
    }

    [Serializable]
    [Syncfusion.Documentation.DocumentationExclude()]
    sealed class GridModelColOperations : GridModelRowColOperations
    {
        [NonSerialized]
        private const int delta = 1;

        /// <summary>
        /// Initializes a new <see cref="GridModelColOperations"/> from a serialization stream.
        /// </summary>
        /// <param name="info">An object that holds all the data needed to serialize or deserialize this instance.</param>
        /// <param name="context">Describes the source and destination of the serialized stream specified by info. </param>
        GridModelColOperations(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
#if DEBUG
            if (Switches.Serialization.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(info.FullTypeName, info.MemberCount);
            }
#else
            ;
#endif
        }

        public GridModelColOperations(GridModel model)
            : base(model)
        {
        }

        public override string RowColName
        {
            get { return "Column"; }
        }

        public override GridModelRowColSizeIndexer Size
        {
            get { return model.ColWidths; }
        }

        public override GridModelHideRowColsIndexer Hidden
        {
            get { return model.HideCols; }
        }

        public override bool OnDefaultSizeChanging(GridDefaultSizeChangingEventArgs e)
        {
            try
            {
                Model.RaiseDefaultColWidthChanging(e);
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

        public override void OnDefaultSizeChanged(GridDefaultSizeChangedEventArgs e)
        {
            try
            {
                Model.RaiseDefaultColWidthChanged(e);
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

        protected/*internal*/ override bool OnHeaderCountChanging(GridCountChangingEventArgs e)
        {
            try
            {
                Model.RaiseHeaderColCountChanging(e);
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

        protected/*internal*/ override void OnHeaderCountChanged(GridCountChangedEventArgs e)
        {
            try
            {
                Model.RaiseHeaderColCountChanged(e);
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

        protected/*internal*/ override bool OnFrozenCountChanging(GridCountChangingEventArgs e)
        {
            try
            {
                Model.RaiseFrozenColCountChanging(e);
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

        protected/*internal*/ override void OnFrozenCountChanged(GridCountChangedEventArgs e)
        {
            try
            {
                Model.RaiseFrozenColCountChanged(e);
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

        bool RaiseMoving(GridRangeMovingEventArgs e)
        {
            try
            {
                Model.RaiseColsMoving(e);
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

        public override void OnRangeMoved(GridRangeMovedEventArgs e)
        {
            try
            {
                Model.RaiseColsMoved(e);
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

        protected override bool RaiseRemoving(GridRangeRemovingEventArgs e)
        {
            try
            {
                Model.RaiseColsRemoving(e);
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

        public override void OnRangeRemoved(GridRangeRemovedEventArgs e)
        {
            try
            {
                Model.RaiseColsRemoved(e);
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

        bool RaiseInserting(GridRangeInsertingEventArgs e)
        {
            try
            {
                Model.RaiseColsInserting(e);
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

        public override void OnRangeInserted(GridRangeInsertedEventArgs e)
        {
            try
            {
                Model.RaiseColsInserted(e);
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

        public override GridStyleInfoStoreTable GetCells(int from, int lastCell, bool rangeStyles, bool cellStyles)
        {
            using (OperationFeedback op = new OperationFeedback(model))
            {
                GridStyleInfoStoreTable data;
                int colCount = lastCell - from + 1;
                int rowCount = cellStyles ? model.RowCount + delta : 0; // model does not count column 0 
                int last = rowCount - delta;
                int first = rangeStyles ? -1 : 0;
                int total = colCount * (last - first + 1);
                data = new GridStyleInfoStoreTable(rowCount, colCount);
                int count = 0;
                if (total > 0)
                {
                    // Store styles in array, but allow user lastCell abort
                    for (int r = first; r <= last; r++)
                    {
                        for (int c = 0; c < colCount; c++)
                        {
                            ////GridStyleInfo style = model[from+r, c];
                            ////data[r, c] = (GridStyleInfoStore) style.Store.Clone();
                            GridStyleInfo style = new GridStyleInfo();
                            model.GetCellInfo(r, from + c, style);
                            data[r, c] = (GridStyleInfoStore)style.Store;
                            op.PercentComplete = (int)((++count) * 100 / total);
                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }
                    }
                }

                return data;
            }
        }

        public override void SetCells(int index, GridStyleInfoStoreTable data, bool extendRows, bool extendCols)
        {
            Model.NotifyChangingLayoutCells(GridRangeInfo.Cols(index, index + data.ColCount));
            OperationFeedback op = new OperationFeedback(model);
            //// TODO: undo info?
            try
            {
                //// TODO: op.Description
                int dataRowCount = data.RowCount;
                int dataColCount = data.ColCount;
                int gridRowCount = model.RowCount + delta;
                int gridColCount = model.ColCount + delta;

                if (dataRowCount > gridRowCount)
                {
                    if (extendRows)
                    {
                        model.Rows.InsertRange(gridRowCount + 1, dataRowCount - gridRowCount);
                        gridRowCount = model.RowCount + delta;
                    }

                    if (dataRowCount > gridRowCount)
                    {
                        dataRowCount = gridRowCount;
                    }
                }

                if (dataColCount + index > gridColCount)
                {
                    if (extendCols)
                    {
                        model.Cols.InsertRange(gridColCount + 1, dataColCount + index - gridColCount);
                        gridColCount = model.ColCount + delta;
                    }

                    if (dataColCount + index > gridColCount)
                    {
                        dataColCount = gridColCount - index;
                    }
                }

                int last = dataRowCount - delta;
                int first = -1;
                int total = dataColCount * (last - first + 1);
                int count = 0;
                if (total > 0)
                {
                    // Store styles in array, but allow user to abort.
                    for (int r = first; r <= last; r++)
                    {
                        for (int c = 0; c < dataColCount; c++)
                        {
                            GridStyleInfoStore store = data[r, c];
                            if (store != null)
                            {
                                GridStyleInfo style = new GridStyleInfo(store);
                                model.SetCellInfo(r, index + c, style, StyleModifyType.Copy);
                            }
                            else
                            {
                                // TODO: option to skip empty cells
                                model.SetCellInfo(r, index + c, null, StyleModifyType.Copy);
                            }

                            op.PercentComplete = (int)((++count) * 100 / total);
                            if (op.ShouldCancel)
                            {
                                throw new GridUserCanceledException();
                            }
                        }
                    }
                }
            }
            finally
            {
                op.Close();
                Model.NotifyChangedLayoutCells(GridRangeInfo.Cols(index, index + data.ColCount));
            }
        }

        protected/*internal*/ override void DataRemoveRange(int removeAt, int count)
        {
            model.Data.RemoveCols(removeAt, count);
            ////throw new NotImplementedException("DataRemoveRange");
        }

        protected/*internal*/ override void DataInsertRange(int insertAt, int count)
        {
            model.Data.InsertCols(insertAt, count);
            ////throw new NotImplementedException("DataInsertRange");
        }

        protected/*internal*/ override void DataMoveRange(int from, int count, int target)
        {
            model.Data.MoveCols(from, count, target);
            ////throw new NotImplementedException("DataMoveRange");
        }

        public override void DelayFloatingCells(int from, int to)
        {
            Model.FloatingCells.DelayFloatCells(GridRangeInfo.Cols(from, to));
        }

        /// <override/>
        protected override void GetRangeFromTo(GridRangeInfo range, out int from, out int last)
        {
            from = range.Left;
            last = range.Right;
        }

        public override GridRangeInfo CreateRangeFromTo(int from, int to)
        {
            return GridRangeInfo.Cols(from, to);
        }

        public override GridRangeInfoList GetSelectedRowColRanges()
        {
            return Model.SelectedRanges.GetColRanges(GridRangeInfoType.Cols);
        }

        public override bool OnRangeInserting(GridRangeInsertingEventArgs e)
        {
            if (RaiseInserting(e))
            {
                int insertAt = e.InsertAt;
                int count = e.Count;
                if (insertAt <= model.ColCount)
                {
                    Hidden.Dictionary.InsertItems(insertAt, count);
                    Size.Dictionary.InsertItems(insertAt, count);
                    Model.SelectedRanges.InsertCols(insertAt, count);
                    Model.CoveredRanges.InsertCols(insertAt, count);
                    Model.BanneredRanges.InsertCols(insertAt, count);
                    Model.FloatingCells.InsertCols(insertAt, count);
                    Model.FloatingCells.DelayFloatCells(GridRangeInfo.Cols(insertAt, Model.ColCount));
                    Model.MergeCells.DelayMergeCells(GridRangeInfo.Cols(insertAt, Model.ColCount));
                }

                return true;
            }

            return false;
        }

        public override bool OnRangeMoving(GridRangeMovingEventArgs e)
        {
            if (RaiseMoving(e))
            {
                int from = e.From;
                int dest = e.Target;
                int count = e.Count;
                int to = from + count - 1;
                int colCount = Model.ColCount;

                Hidden.Dictionary.MoveItems(from, count, dest);
                Size.Dictionary.MoveItems(from, count, dest);
                Model.SelectedRanges.MoveCols(from, to, dest, colCount);
                Model.CoveredRanges.MoveCols(from, to, dest, colCount);
                Model.BanneredRanges.MoveCols(from, to, dest, colCount);
                Model.FloatingCells.MoveCols(from, to, dest);
                Model.FloatingCells.DelayFloatCells(GridRangeInfo.Cols(Math.Min(from, dest), Model.ColCount));
                Model.MergeCells.DelayMergeCells(GridRangeInfo.Cols(Math.Min(from, dest), Model.ColCount));
                return true;
            }

            return false;
        }

        public override bool OnRangeRemoving(GridRangeRemovingEventArgs e)
        {
            int from = e.From;
            int to = e.To;
            int count = to - from + 1;
            Hidden.Dictionary.RemoveItems(from, count);
            Size.Dictionary.RemoveItems(from, count);
            int colCount = Model.ColCount;
            Model.SelectedRanges.RemoveCols(from, to, colCount);
            Model.CoveredRanges.RemoveCols(from, to);
            Model.BanneredRanges.RemoveCols(from, to);
            Model.FloatingCells.RemoveCols(from, to);
            Model.FloatingCells.DelayFloatCells(GridRangeInfo.Cols(from, Model.ColCount));
            Model.MergeCells.DelayMergeCells(GridRangeInfo.Cols(from, Model.ColCount));
            return true;
        }
    }
}