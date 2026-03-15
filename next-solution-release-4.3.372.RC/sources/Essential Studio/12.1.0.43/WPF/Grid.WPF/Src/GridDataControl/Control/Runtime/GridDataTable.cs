#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
namespace Syncfusion.Windows.Controls.Grid
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Syncfusion.Linq;
    using Syncfusion.Windows.Collections.Generic;
    using Syncfusion.Windows.ComponentModel;
    using Syncfusion.Windows.Controls.Scroll;
    using Syncfusion.Windows.Data;
#if !SILVERLIGHT
    using System.Data;
#else
    //using System.Windows.Controls;
    using System.Reflection;
#endif
    using Syncfusion.Windows.Controls.Cells;
    using System.Collections.Specialized;
#if SyncfusionFramework4_0
    using Syncfusion.Dynamic;
    using System.Windows.Controls;
#endif

    /// <summary>
    /// Specifies the events and properties related to the <see cref="GridDataTableModel"/>.
    /// </summary>
    public class GridDataTable : Disposable, ISupportInitialize
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="GridTable"/> class.
        /// </summary>
        /// <param name="parentGroup">The parent group.</param>
        public GridDataTable(GridDataTableModel parentGroup)
        {
            this.Model = parentGroup;
            this.NeedsInvalidate = true;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the record is collapsed.
        /// </summary>
        public event EventHandler<GridDataValueEventArgs<GridDataRecord>> RecordCollapsed;

        /// <summary>
        /// Occurs when the record is collapsing.
        /// </summary>
        public event EventHandler<GridDataValueCancelEventArgs<GridDataRecord>> RecordCollapsing;

        /// <summary>
        /// Occurs when the record is expanded.
        /// </summary>
        public event EventHandler<GridDataValueEventArgs<GridDataRecord>> RecordExpanded;

        /// <summary>
        /// Occurs when the record is expanding.
        /// </summary>
        public event GridDataRecordExpandingEventHandler RecordExpanding;

        /// <summary>
        /// Occurs when [details view expanding].
        /// </summary>
        public event GridDataDetailsViewExpandingEventHandler DetailsViewExpanding;

        /// <summary>
        /// Occurs when [details view expanded].
        /// </summary>
        public event GridDataDetailsViewExpandedEventHandler DetailsViewExpanded;

        /// <summary>
        /// Occurs when [details view collapsing].
        /// </summary>
        public event EventHandler<GridDataValueCancelEventArgs<GridDataRecord>> DetailsViewCollapsing;

        /// <summary>
        /// Occurs when [details view collapsed].
        /// </summary>
        public event EventHandler<GridDataValueEventArgs<GridDataRecord>> DetailsViewCollapsed;

        /// <summary>
        /// Occurs when the record goes into begin edit state.
        /// </summary>
        public event EventHandler<GridDataValueCancelEventArgs<int>> RecordBeforeBeginEdit;

        /// <summary>
        /// Occurs when the record completes the begin edit state.
        /// </summary>
        public event EventHandler<GridDataValueEventArgs<int>> RecordAfterBeginEdit;

        /// <summary>
        /// Occurs when the record completes the end edit.
        /// </summary>
        public event EventHandler<GridDataValueEventArgs<int>> RecordEndEdit;


        /// <summary>
        /// Occurs when the record is deleting.
        /// </summary>
        public event GridDataRecordDeletingEventHandler RecordDeleting;

        /// <summary>
        /// Occurs when the record is deleted.
        /// </summary>
        public event GridDataRecordDeletedEventHandler RecordDeleted;


        /// <summary>
        /// Occurs when the current cell value is validating.
        /// </summary>
        public event GridDataCurrentCellValidatingEventHandler CurrentCellValidating;

        /// <summary>
        /// Occurs when the sort columns are changing.
        /// </summary>
        public event GridDataSortColumnsChangingEventHandler SortColumnsChanging;

        /// <summary>
        /// Occurs when the sort columns are changed.
        /// </summary>
        public event GridDataSortColumnsChangedEventHandler SortColumnsChanged;

        /// <summary>
        /// Occurs when the grouped columns are changed.
        /// </summary>
        public event GridDataGroupedColumnsChangedEventHandler GroupedColumnsChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has nested tables.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has nested tables; otherwise, <c>false</c>.
        /// </value>
        public bool HasNestedTables
        {
            get
            {
#if !SILVERLIGHT
                if (GridDataTableModelHelper.IsInDesignMode)
                    return false;

#endif
                return this.Model.TableProperties.ShowRecordPlusMinus && this.Model.TableProperties.Relations.Where(r => r.RelationType == RelationType.MasterDetails).Count() > 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has table summaries.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has table summaries; otherwise, <c>false</c>.
        /// </value>
        public bool HasTableSummaries
        {
            get
            {
                return this.Model.TableProperties.ShowTableSummaries && this.Model.TableProperties.TableSummaryRows.Count > 0 ? true : false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has stacked headers.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has stacked headers; otherwise, <c>false</c>.
        /// </value>
        public bool HasStackedHeaders
        {
            get
            {
                return this.Model.TableProperties.StackedHeaderRows.Count > 0 ? true : false;
            }
        }

        GridDataTableModel _model;

        /// <summary>
        /// Gets or sets the parent group.
        /// </summary>
        /// <value>The parent group.</value>
        public GridDataTableModel Model
        {
            get { return _model; }
            internal set { _model = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [needs invalidate].
        /// </summary>
        /// <value><c>true</c> if [needs invalidate]; otherwise, <c>false</c>.</value>
        internal bool NeedsInvalidate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the total column count.
        /// </summary>
        /// <value>The total column count.</value>
        public int TotalColumnCount
        {
            get
            {
                return this.Model.TableProperties.Relations.Count > 0 ? this.Model.TableProperties.VisibleColumns.Count + 1 : this.Model.TableProperties.VisibleColumns.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has detail view.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has detail view; otherwise, <c>false</c>.
        /// </value>
        public bool HasDetailsView
        {
            get
            {
                return this.Model.TableProperties.DetailsViewTemplate != null;
            }
        }

        #endregion

        #region Record GetValue

        internal object GetUnboundValue(int recordIndex, string columnName)
        {
            var model = this.Model;
            object result = null;
            object record = null;
            if (!this.HasGroups)
            {
                record = recordIndex < this.Model.View.Records.Count ? this.Model.View.Records[recordIndex] : null;
            }
            else
            {
                var groupCache = this.GroupModel.DisplayElements[recordIndex];
                if (groupCache is RecordEntry)
                {
                    var recordEntry = groupCache as RecordEntry;
                    var flag = true;
                    if (this.Model.CurrencyManager.IsEditing && this.Model.CurrencyManager.UpdateMode == UpdateMode.RowCachedMode)
                    {
                        var cellValue = this.Model.CurrencyManager.GetValueFromCache(columnName);
                        if (cellValue != null)
                        {
                            record = cellValue;
                            flag = false;
                        }
                    }

                    if (flag)
                    {
                        record = recordEntry.Data;
                    }
                }
            }
            if (record != null)
            {
                var column = model.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(v => v.MappingName == columnName);
                result = this.GetUnboundValue(recordIndex, column);
                if (result == null)
                {
                    var colIndex = model.TableProperties.VisibleColumns.IndexOf(column);
                    colIndex = model.ResolveVisibleColumnIndexToPosition(colIndex);
                    var rowIndex = model.ResolvePositionToIndex(recordIndex);
                    var rowColIndex = new RowColumnIndex(rowIndex, colIndex);
                    var style = (GridDataStyleInfo)model[rowIndex, colIndex];
                    model.RaiseQueryUnboundValue(rowColIndex, style, recordIndex, column, false);
                    result = style.CellValue;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the unbound value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetUnboundValue(object record, string columnName)
        {
            var model = this.Model;
            var column = model.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(v => v.MappingName == columnName);
            return this.GetUnboundValue(record, column);
        }

        internal object GetUnboundValueForEvent(object record, GridDataUnboundVisibleColumn column)
        {
            // var model = this.Model;
            object value = null;
            if (column.Format != string.Empty)
            {
                value = column.Format.FormatByName(null, (key) =>
                {
                    var itemProperties = this.Model.View.GetItemProperties();
                    var pd = itemProperties.GetPropertyDescriptor(key);
                    if (pd != null)
                    {
                        return pd.GetValue(record);
                    }

                    return null;
                });
            }
            else if (column.Expression != string.Empty)
            {
                value = column.ComputedValue(record);
            }
            else if (column.MappingName != null && column.MappingName != string.Empty)
            {
                var itemProperties = this.Model.View.GetItemProperties();
                var pd = itemProperties.GetPropertyDescriptor(column.MappingName);
                if (pd != null)
                {
                    return pd.GetValue(record);
                }
            }
            return value;
        }

        /// <summary>
        /// Gets the unbound value.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public object GetUnboundValue(object record, GridDataUnboundVisibleColumn column)
        {
            var model = this.Model;
            object value = null;
            if (column.Format != string.Empty)
            {
                value = column.Format.FormatByName(null, (key) =>
                {
                    var itemProperties = this.Model.View.GetItemProperties();
                    var pd = itemProperties.GetPropertyDescriptor(key);
                    if (pd != null)
                    {
                        return pd.GetValue(record);
                    }

                    return null;
                });
            }
            else if (column.Expression != string.Empty)
            {
                value = column.ComputedValue(record);
            }

            if (value == null)
            {
                int recordIndex = -1;
                if (!this.HasGroups)
                {
                    recordIndex = this.Model.View.Records.IndexOfRecord(record);
                }
                else
                {
                    recordIndex = this.Model.View.TopLevelGroup.IndexOf(record);
                }
                var colIndex = model.TableProperties.VisibleColumns.IndexOf(column);
                colIndex = model.ResolveVisibleColumnIndexToPosition(colIndex);
                var rowIndex = model.ResolvePositionToIndex(recordIndex);
                var rowColIndex = new RowColumnIndex(rowIndex, colIndex);
                var style = (GridDataStyleInfo)model[rowIndex, colIndex];
                model.RaiseQueryUnboundValue(rowColIndex, style, recordIndex, column, false);
                value = style.CellValue;
            }

            return value;
        }

        /// <summary>
        /// Gets the unbound value.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public object GetUnboundValue(int recordIndex, GridDataUnboundVisibleColumn column)
        {
            object value = null;
            if (recordIndex > -1 && recordIndex < this.Model.SourceListCount)
            {
                var record = this.Model.View.Records.GetItemAt(recordIndex);
                value = this.GetUnboundValue(record, column);
            }
            return value;
        }

        /// <summary>
        /// Gets the unbound value.
        /// </summary>
        /// <param name="rowIdx">The row idx.</param>
        /// <param name="colIdx">The col idx.</param>
        /// <returns></returns>
        public object GetUnboundValue(int rowIdx, int colIdx)
        {
            var actualRowIdx = this.Model.ResolvePositionToIndex(rowIdx);
            var recordIndex = this.Model.ResolveIndexToRecordPosition(actualRowIdx);
            colIdx = this.Model.ResolvePositionToVisibleColumnIndex(colIdx);

            object value = null;
            if (colIdx >= 0 && this.Model.TableProperties.VisibleColumns[colIdx].IsUnbound)
            {
                GridDataUnboundVisibleColumn column = (GridDataUnboundVisibleColumn)this.Model.TableProperties.VisibleColumns[colIdx];
                if (recordIndex > -1 && recordIndex < this.Model.SourceListCount && column.Format != string.Empty)
                {
                    var record = this.Model.View.Records.GetItemAt(recordIndex);
                    value = column.Format.FormatByName(null, (key) =>
                    {
                        var itemProperties = this.Model.View.GetItemProperties();
                        var item = itemProperties.GetPropertyDescriptor(key);
                        if (item != null)
                        {
                            return item.GetValue(record);
                        }

                        return null;
                    });
                }
                else if (recordIndex > -1 && recordIndex < this.Model.SourceListCount && column.Expression != string.Empty)
                {
                    var record = this.Model.View.Records.GetItemAt(recordIndex);
                    return column.ComputedValue(record);
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the record from the actual row index present in the Grid. You could use this in MouseDown events.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        public object GetRecordFromRow(int rowIndex)
        {
            var recordIndex = this.Model.ResolveIndexToRecordPosition(rowIndex);
            if (this.HasGroups)
                return this.Model.View.TopLevelGroup.DisplayElements.ElementAt(recordIndex);
            else
                return this.Model.View.Records.GetItemAt(recordIndex);
        }

        internal void SetValue(int recordIndex, string columnName, object value)
        {
            if (recordIndex > -1)
            {
                object record = null;
                if (!this.HasGroups)
                {
                    record = this.Model.View.Records.GetItemAt(recordIndex);
                }
                else
                {
                    var rec = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                    if (rec != null)
                        record = rec.Data;
                }
#if !SILVERLIGHT
                if (!this.Model.IsLegacyDataTable)
#endif
                {
                    var provider = this.Model.View.GetPropertyAccessProvider();
                    provider.SetValue(record, columnName, value);
                }
#if !SILVERLIGHT
                else
                {
                    var datarow = record as DataRowView;
                    var propertyType = this.Model.View.GetItemProperties()[columnName].PropertyType;
                    //var grouplist = this.Model.View.TopLevelGroup as IGroupList;
                    //if (grouplist != null)
                    //{
                    //    grouplist.Remove(datarow);
                    //}
                    value = NullableHelper.FixDbNUllasNull(value, propertyType);
                    value = NullableHelper.ChangeType(value, propertyType);
                    if (!datarow[columnName].Equals(value))
                    {                        
                        datarow[columnName] = value;
                        datarow.EndEdit();
                        //datarow.Row.AcceptChanges();
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Gets the value of the underlying record object.
        /// </summary>
        /// <param name="record">The record.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetValue(object record, string columnName)
        {
            object data = null;
#if !SILVERLIGHT
            if (this.Model.IsLegacyDataTable)
            {
                var rowView = record as DataRowView;
                if (rowView != null && rowView.Row != null && rowView.Row.RowState != DataRowState.Detached)
                {
                    data = rowView[columnName];
                }
                return data;
            }
#endif
            var provider = this.Model.View.GetPropertyAccessProvider();
            if (provider != null)
            {
                data = provider.GetValue(record, columnName);
            }

            return data;
        }

        /// <summary>
        /// Gets the value of the column from the record. This method returns proper results for data when it is grouped / cached in edit mode.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetValue(int recordIndex, string columnName)
        {
            object data = null;
            if (!this.HasGroups)
            {
                // we get the value from the cache instance
                var flag = true;

                var visibleCol = this.Model.TableProperties.VisibleColumns.FirstOrDefault(v => v.MappingName == columnName);
                if (visibleCol != null)
                {
                    if (visibleCol.UpdateMode == UpdateMode.RowCachedMode && this.Model.CurrencyManager.CurrentCell.IsEditing)
                    {
                        var cellValue = this.Model.CurrencyManager.GetValueFromCache(columnName);
                        if (cellValue != null)
                        {
                            data = cellValue;
                            flag = false;
                        }
                    }
                }

                if (flag)
                {
                    if (recordIndex > -1 && recordIndex < this.Model.SourceListCount)
                    {
                        var record = this.Model.View.Records.GetItemAt(recordIndex);
                        data = this.GetValue(record, columnName);
                    }
                }
            }
            else
            {
                var groupCache = this.GroupModel.DisplayElements[recordIndex];
                if (groupCache is RecordEntry)
                {
                    var recordEntry = groupCache as RecordEntry;
                    var flag = true;
                    if (this.Model.CurrencyManager.IsEditing && this.Model.CurrencyManager.UpdateMode == UpdateMode.RowCachedMode)
                    {
                        var cellValue = this.Model.CurrencyManager.GetValueFromCache(columnName);
                        if (cellValue != null)
                        {
                            data = cellValue;
                            flag = false;
                        }
                    }

                    if (flag)
                    {
                        var item = recordEntry.Data;
                        data = this.GetValue(item, columnName);
                    }
                }
            }

            return data;
        }

        #endregion

        #region BeginInit/EndInit

        /// <summary>
        /// Signals the object that initialization is starting.
        /// </summary>
        public void BeginInit()
        {
            if (!this.NeedsInvalidate)
            {
                this.NeedsInvalidate = true;
            }
        }

        /// <summary>
        /// Signals the object that initialization is complete.
        /// </summary>
        public void EndInit()
        {
            if (this.NeedsInvalidate)
            {
                this.NeedsInvalidate = true;
                if (!this.Model.ValidateColumns())
                {
                    this.Model.RefreshColumns(true, true);
                }

                this.Model.ColumnAutoSizer.RefreshAll();
                if (this.Model.View != null)
                {
                    this.Model.RefreshSourceListCount();
                }
                this.Model.InvalidateDisplay();
                if (this.Model.Grid != null)
                {
                    this.Model.Grid.InvalidateMeasure();
                }
            }
        }

        #endregion

        #region Expand / Collapse records
        /// <summary>
        /// Expands the record at specified record index.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        public void ExpandRecordAt(int recordIndex)
        {
            if (this.HasNestedTables && !this.HasGroups)
            {
                this.BeginInit();
                this.ExpandAllRecordAt(recordIndex);
                this.EndInit();
            }
        }

        /// <summary>
        /// Expands all the records with nested tables.
        /// </summary>
        public void ExpandAll()
        {
            if (this.HasNestedTables)
            {
                this.BeginInit();
                var lines = this.Model.RowHeights as LineSizeCollection;
                lines.SuspendUpdates();

                if (!this.HasGroups)
                {
                    for (int i = 0; i < this.Model.SourceListCount; i++)
                    {
                        this.ExpandAllRecordAt(i);
                    }
                }
                else
                {
                    foreach (var nodeEntry in this.Model.View.TopLevelGroup.DisplayElements)
                    {
                        if (nodeEntry is RecordEntry)
                        {
                            var recordEntry = nodeEntry as RecordEntry;
                            //this.Model.ExpandedRecordCount++;
                            recordEntry.IsExpanded = true;
                        }
                    }
                }

                lines.ResumeUpdates(true);
                this.EndInit();
                this.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Expands all the nested table at specified record index.
        /// </summary>
        /// <param name="i">The recordIndex.</param>
        private void ExpandAllRecordAt(int recordIndex)
        {
            var record = recordIndex > -1 && recordIndex < this.Model.SourceListCount ? this.Model.View.Records[recordIndex] as GridDataRecord : null;
            if (record != null && !record.IsExpanded && this.ShouldExpand(record))
            {
                //this.Model.ExpandedRecordCount++;
                record.IsExpanded = true;
                if (record.ChildModels.Count > 0)
                {
                    foreach (KeyValuePair<int, GridDataChildTableModel> kvp in record.ChildModels)
                    {
                        kvp.Value.Table.ExpandAll();
                    }
                }
            }
        }

        internal bool ShouldExpand(GridDataRecord record)
        {
#if !SILVERLIGHT
            if (!record.Model.TableProperties.HideEmptyChildGrid)
            {
                return true;
            }

            int count = 0;
            
            foreach (var rd in record.Model.TableProperties.Relations)
            {
                if (record.ChildModels != null && record.ChildModels.Count > 0)
                {
                    foreach (var item in record.ChildModels)
                    {
                        if (item.Value.View != null && item.Value.View.Records.Count > 0)
                        {
                            count++;
                        }
                    }
                    return count > 0;
                }
                var childSource = record.GetChildSource(rd.RelationalColumn, record.Model.IsLegacyDataTable, record.Model.View);
                if (childSource!= null && !record.Model.IsLegacyDataTable && childSource.AsQueryable().Count() > 0)
                {
                    count++;
                }
                else if(childSource!= null && record.Model.IsLegacyDataTable)
                {
                    if (((DataView)childSource).Count > 0)
                    {
                        count++;
                    }
                }
            }

            return count > 0;
#else
            bool Expandable = false;

            if (!record.Model.TableProperties.HideEmptyChildGrid)
                return true;

            int count = 0;
            foreach (var rd in record.Model.TableProperties.Relations)
            {
               var childSource = record.GetChildSource(rd.RelationalColumn, false, record.Model.View);
                if (childSource!= null && childSource.AsQueryable().Count() > 0)
                {
                    count++;
                }
            }

            if (count == 0)
            {
                Expandable = false;
            }
            else
            {

                Expandable = true;
            }
            return Expandable;
#endif
        }

        /// <summary>
        /// Collapses the nested table at specified record index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        public void CollapseAt(int recordIndex)
        {
            if (this.HasNestedTables)
            {
                this.BeginInit();
                this.CollapseAllRecordAt(recordIndex);
                this.EndInit();
            }
        }

        /// <summary>
        /// Collapses all the nested tables for all the records.
        /// </summary>
        public void CollapseAll()
        {
            if (this.HasNestedTables)
            {
                this.BeginInit();
                var lines = this.Model.RowHeights as LineSizeCollection;
                lines.SuspendUpdates();
                if (!this.HasGroups)
                {
                    // collapse all records
                    for (int i = 0; i < this.Model.SourceListCount; i++)
                    {
                        this.CollapseAllRecordAt(i);
                    }
                }
                else
                {
                    //Previous Code: foreach (var nodeEntry in this.Model.View.TopLevelGroup.DisplayElements) This code change as below because DisplayElements contains all elements which are in display(GroupHeaders,TableSummary)
                    foreach (var nodeEntry in this.Model.View.Records)
                    {
                        if (nodeEntry is RecordEntry)
                        {
                            var recordEntry = nodeEntry as RecordEntry;
                            if (this.Model.ExpandedRecordCount > 0)
                                this.Model.ExpandedRecordCount--;
                            recordEntry.IsExpanded = false;
                        }
                    }
                }

                lines.ResumeUpdates(true);
                this.EndInit();
                this.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Collapses all the nested table at specified record index.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        private void CollapseAllRecordAt(int recordIndex)
        {
            var record = recordIndex > -1 && recordIndex < this.Model.SourceListCount ? this.Model.View.Records[recordIndex] as GridDataRecord : null;
            if (record != null && record.IsExpanded)
            {
                if (this.Model.ExpandedRecordCount > 0)
                    this.Model.ExpandedRecordCount--;
                record.IsExpanded = false;
                if (record.ChildModels.Count > 0)
                {
                    foreach (KeyValuePair<int, GridDataChildTableModel> kvp in record.ChildModels)
                    {
                        kvp.Value.Table.CollapseAll();
                    }
                }
            }
        }

        internal void ShowAllUIRows()
        {
            this.BeginInit();
            this.Model.View.Records.OfType<GridDataRecord>().Where(r => !r.IsExpanded).ForEach<GridDataRecord>(r => r.IsExpanded = true);
            this.EndInit();
            this.Model.InvalidateDisplay();
        }

        internal void HideAllUIRows()
        {
            if (this.Model.View == null)
            {
                return;
            }

            /// To find the existance of expanded relatio and expanded details view
            var hasExpandedRows = this.Model.View.Records.FirstOrDefault((r)=> r.IsExpanded || ((r as GridDataRecord) != null && (r as GridDataRecord).IsDetailsViewExpanded)) != null;
            if (hasExpandedRows)
            {
                this.BeginInit();
                foreach (var record in this.Model.View.Records)
                {
                    if (record is GridDataRecord)
                    {
                        var gridRecord = record as GridDataRecord;
                        gridRecord.IsExpanded = false;
                        gridRecord.IsDetailsViewExpanded = false;
                    }
                }
                if (this.HasGroups)
                {
                    this.CollapseRecords();
                    this.Model.RefreshColumns(true, true);
                }
                this.EndInit();
                this.Model.InvalidateDisplay();
            }
        }

        private void CollapseRecords()
        {
            foreach (var record in this.Model.View.TopLevelGroup.DisplayElements)
            {
                if (record is RecordEntry)
                {
                    var gridRecord = record as GridDataRecord;
                    gridRecord.IsExpanded = false;
                }
            }
        }

        #endregion

        #region Expand / Collapse groups

        /// <summary>
        /// Expands all groups.
        /// </summary>
        public void ExpandAllGroups()
        {
            this.BeginInit();
            var lineSizeCollection = this.Model.RowHeights as LineSizeCollection;
            lineSizeCollection.SuspendUpdates();

            if (this.HasNestedTables)
            {
                if (this.GroupModel != null)
                {
                    if (this.GroupModel.TableProperties != null)
                        ExpandAllGroups(this.GroupModel);
                }
            }
            else
            {
                this.GroupModel.ExpandAll();
                this.Refresh();
            }
            lineSizeCollection.ResumeUpdates();
            this.EndInit();
        }

        /// <summary>
        /// Expands all groups at level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void ExpandAllGroupsAtLevel(int level)
        {
            var maxlevel = this.GroupModel != null ? this.GroupModel.GetMaxLevel() : 0;
            if (level > 0 && level <= maxlevel)
            {
                this.BeginInit();
                var lineSizeCollection = this.Model.RowHeights as LineSizeCollection;
                lineSizeCollection.SuspendUpdates();
                if (this.HasNestedTables)
                {
                    ExpandAllGroupsAtLevel(level, this.GroupModel);
                }
                else
                {
                    this.GroupModel.ExpandAllAtLevel(level);
                    this.Refresh();
                }
                lineSizeCollection.ResumeUpdates();
                this.EndInit();
            }
        }
        private bool isParentExpanded = false;
        private void ExpandAllGroupsAtLevel(int level, Group group)
        {
            foreach (var innergroup in group.Groups)
            {
                if (innergroup.Level == level)
                {
                    if (isParentExpanded)
                    {
                        this.ExpandGroup(innergroup);
                    }
                    else
                    {
                        innergroup.IsExpanded = true;
                        isParentExpanded = false;
                    }
                }
                else
                {
                    if (innergroup.IsExpanded)
                    {
                        isParentExpanded = true;
                    }
                    ExpandAllGroupsAtLevel(level, innergroup);
                }
            }
        }

        private void ExpandAllGroups(Group baseGroup)
        {
            foreach (var group in baseGroup.Groups)
            {
                this.ExpandGroup(group);
                if (!group.IsBottomLevel)
                {
                    ExpandAllGroups(group);
                }
            }
        }

        /// <summary>
        /// Expands the group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void ExpandGroup(Group group)
        {
            if (!this.HasGroups || group == null)
            {
                return;
            }

            if (!group.IsExpanded && !this.RaiseGroupExpanding(group))
            {
                var insertedItems = this.GroupModel.ExpandGroup(group);
                var startIdx = this.Model.ResolveStartIndexOfGroup(group) + 1;
                if (this.HasNestedTables || this.HasDetailsView)
                {
                    this.Model.InsertRows(startIdx, insertedItems);
                    // var itemsWithoutSummaries = insertedItems - this.Model.TableProperties.SummaryRows.Count;
                    ResetExpandedState(group);
                }
                else
                {
                    this.Model.InsertRows(startIdx, insertedItems);
                }
                this.Model.InvalidateCell(GridRangeInfo.Row(startIdx - 1));
                this.RaiseGroupExpanded(group);
                if (this.NeedsInvalidate)
                {
                    if (!this.Model.ValidateColumns())
                    {
                        this.Model.RefreshColumns(true, true);
                    }
                    else
                    {
                        this.Model.ColumnAutoSizer.SetStarWidth();
                    }
                }
            }
        }

        private void ResetExpandedState(Group group)
        {
            var lineSizeCollection = this.Model.RowHeights as LineSizeCollection;
            if (!group.IsBottomLevel)
            {
                foreach (var childGroup in group.Groups)
                {
                    ResetExpandedState(childGroup);
                }
            }
            else
            {
                if (group.IsExpanded)
                {
                    int startIdx = this.Model.ResolveStartIndexOfGroup(group) + 1;
                    var endIndex = startIdx + group.GetRecordCount();
                    lineSizeCollection.SetHiddenIntervalWithState(startIdx, endIndex, this.ResolveHiddenPattern());
                    startIdx++;
                    foreach (GridDataRecord record in group.Records)
                    {
                        if (record.IsDetailsViewExpanded)
                        {
                            /// Default padding in top and bottom is 12.5, hence extra 25 is added to height
                            lineSizeCollection.SetNestedLines(startIdx, new Scroll.LineSizeCollection { LineCount = 1, DefaultLineSize = record.DetailsViewCellSize.Height + 25 });

                            lineSizeCollection.SetHidden(startIdx, startIdx, false);
                            startIdx++;
                        }
                        if (record.IsExpanded)
                        {
                            for (int i = 0; i < record.ChildModels.Count; i++)
                            {
                                lineSizeCollection.SetNestedLines(startIdx, record.ChildModels[i].RowHeights);
                            }
                            lineSizeCollection.SetHidden(startIdx, startIdx, false);
                        }
                        startIdx += 2;
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the group is expanded.
        /// </summary>
        public event GroupExpandedEventHandler GroupExpanded;

        internal void RaiseGroupExpanded(Group group)
        {
            var handler = this.GroupExpanded;
            if (handler != null)
            {
                handler(this, new GroupExpandedEventArgs() { Group = group });
            }
        }

        /// <summary>
        /// Occurs when the group is expanding.
        /// </summary>
        public event GroupExpandingEventHandler GroupExpanding;

        internal bool RaiseGroupExpanding(Group group)
        {
            var result = false;
            var handler = this.GroupExpanding;
            if (handler != null)
            {
                var args = new GroupExpandingEventArgs() { Group = group };
                handler(this, args);
                result = args.Cancel;
            }

            return result;
        }

        /// <summary>
        /// Collapses all groups.
        /// </summary>
        public void CollapseAllGroups()
        {
            if (this.HasGroups)
            {
                this.BeginInit();
                var lineSizeCollection = this.Model.RowHeights as LineSizeCollection;
                lineSizeCollection.SuspendUpdates();
                var topLevelGroup = this.GroupModel;
                topLevelGroup.CollapseAll();
                lineSizeCollection.ResumeUpdates();
                this.Refresh();
                this.EndInit();
            }
        }

        /// <summary>
        /// Collapses all groups at level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void CollapseAllGroupsAtLevel(int level)
        {
            if (this.HasGroups)
            {
                this.BeginInit();
                var lineSizeCollection = this.Model.RowHeights as LineSizeCollection;
                lineSizeCollection.SuspendUpdates();
                this.GroupModel.CollapseAllAtLevel(level);
                lineSizeCollection.ResumeUpdates();
                this.Refresh();
                this.EndInit();
            }
        }

        /// <summary>
        /// Collapses the group.
        /// </summary>
        /// <param name="group">The group.</param>
        public void CollapseGroup(Group group)
        {
            if (!this.HasGroups || group == null)
            {
                return;
            }

            if (group.IsExpanded && !this.RaiseGroupCollapsing(group))
            {
                var collapsedItems = this.GroupModel.CollapseGroup(group);
                var startIndex = this.Model.ResolveStartIndexOfGroup(group) + 1;
                this.Model.RemoveRows(startIndex, collapsedItems);
                this.Model.InvalidateCell(GridRangeInfo.Row(startIndex - 1));
                this.RaiseGroupCollapsed(group);
                if (this.NeedsInvalidate)
                {
                    if (!this.Model.ValidateColumns())
                    {
                        this.Model.RefreshColumns(true, true);
                    }
                    else
                    {
                        this.Model.ColumnAutoSizer.SetStarWidth();
                    }
                    this.Model.Grid.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Occurs when the group is collapsed.
        /// </summary>
        public event GroupCollapsedEventHandler GroupCollapsed;

        internal void RaiseGroupCollapsed(Group group)
        {
            var handler = this.GroupCollapsed;
            if (handler != null)
            {
                handler(this, new GroupCollapsedEventArgs() { Group = group });
            }
        }

        /// <summary>
        /// Occurs when the group is collapsing.
        /// </summary>
        public event GroupCollapsingEventHandler GroupCollapsing;

        internal bool RaiseGroupCollapsing(Group group)
        {
            var result = false;
            var handler = this.GroupCollapsing;
            if (handler != null)
            {
                var args = new GroupCollapsingEventArgs() { Group = group };
                handler(this, args);
                result = args.Cancel;
            }

            return result;
        }

        #endregion

        #region Expand/Collapse Details View

        /// <summary>
        /// Expands the record at specified record index.
        /// </summary>
        /// <param name="recordIndex">Index of the record.</param>
        public void ExpandDetailsViewAt(int recordIndex)
        {
            if (this.HasDetailsView && !this.HasGroups)
            {
                this.BeginInit();
                var record = recordIndex > -1 && recordIndex < this.Model.SourceListCount ? this.Model.View.Records[recordIndex] as GridDataRecord : null;
                if (record != null && !record.IsDetailsViewExpanded)
                {
                    record.IsDetailsViewExpanded = true;
                }
                this.EndInit();
            }
        }

        /// <summary>
        /// Expands all the records with nested tables.
        /// </summary>
        public void ExpandAllDetailsViews()
        {
            if (this.HasDetailsView)
            {
                this.BeginInit();
                var lines = this.Model.RowHeights as LineSizeCollection;
                lines.SuspendUpdates();

                if (!this.HasGroups)
                {
                    for (int i = 0; i < this.Model.SourceListCount; i++)
                    {
                        this.ExpandDetailsViewAt(i);
                    }
                }
                else
                {
                    foreach (var nodeEntry in this.Model.View.TopLevelGroup.DisplayElements)
                    {
                        if (nodeEntry is GridDataRecord)
                        {
                            var recordEntry = nodeEntry as GridDataRecord;
                            recordEntry.IsDetailsViewExpanded = true;
                        }
                    }
                }
                lines.ResumeUpdates(true);
                this.EndInit();
                this.Model.InvalidateDisplay();
            }
        }

        /// <summary>
        /// Collapses the nested table at specified record index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        public void CollapseDetailsViewAt(int recordIndex)
        {
            if (this.HasDetailsView && !this.HasGroups)
            {
                this.BeginInit();
                
                var record = recordIndex > -1 && recordIndex < this.Model.SourceListCount ? this.Model.View.Records[recordIndex] as GridDataRecord : null;
                if (record != null && record.IsDetailsViewExpanded)
                {
                    record.IsDetailsViewExpanded = false;
                }

                this.EndInit();
            }
        }

        /// <summary>
        /// Collapses all the nested tables for all the records.
        /// </summary>
        public void CollapseAllDetailsViews()
        {
            if (this.HasDetailsView)
            {
                this.BeginInit();
                var lines = this.Model.RowHeights as LineSizeCollection;
                lines.SuspendUpdates();

                if (!this.HasGroups)
                {
                    // collapse all records
                    for (int i = 0; i < this.Model.SourceListCount; i++)
                    {
                        this.CollapseDetailsViewAt(i);
                    }
                }
                else
                {
                    foreach (var nodeEntry in this.Model.View.TopLevelGroup.DisplayElements)
                    {
                        if (nodeEntry is GridDataRecord)
                        {
                            var recordEntry = nodeEntry as GridDataRecord;
                            recordEntry.IsDetailsViewExpanded = false;
                        }
                    }
                }

                lines.ResumeUpdates(true);
                this.EndInit();
                this.Model.InvalidateDisplay();
            }
        }

        #endregion

        #region Event handlers

        #region Records Selection Events

        internal void RaiseRecordsSelectionChanged(GridDataRecordsSelectionChangedEventArgs e)
        {
            OnRecordsSelectionChanged(e);
        }

        /// <summary>
        /// Raises the <see cref="GridModel.SelectionChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GridSelectionChangedEventArgs"/> that contains the event data.</param>
        protected virtual void OnRecordsSelectionChanged(GridDataRecordsSelectionChangedEventArgs e)
        {
            var handler = this.RecordsSelectionChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs after the model updates its internal data structures when the model in the process of selecting
        /// a range of cells as a result of a <see cref="GridModelSelections.SelectRange"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="GridModel"/> will raise a  <see cref="GridModel.SelectionChanging"/> event before
        /// it updates its internal data structures and a  <see cref="GridModel.SelectionChanged"/> event after
        /// afterwards. A <see cref="GridControlBase"/> grid listens to this event and outline
        /// the selected range of cells.
        /// </remarks>
        [Description("Occurs after internal data structures were updated with new selection state from a SelectRange command.")]
        [Category("Behavior")]
        public event GridDataRecordsSelectionChangedEventHandler RecordsSelectionChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        internal GridDataRecordSelectionChangingEventArgs RaiseRecordSelectionChanging(GridDataRecordSelectionChangingEventArgs e)
        {
            var args = new GridDataRecordSelectionChangingEventArgs();
            OnRecordsSelectionChanging(e);
            return args;
        }


        protected virtual void OnRecordsSelectionChanging(GridDataRecordSelectionChangingEventArgs e)
        {
            var handler = this.RecordSelectionChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event GridDataRecordSelectionChangingEventHandler RecordSelectionChanging;

        #endregion

        #region New Record events

        /// <summary>
        /// Occurs when a new record is adding from insert new record row
        /// </summary>
        public event GridDataNewRecordAddingEventHandler RecordAdding;

        /// <summary>
        /// Occurs when a new record is added from insert new record row
        /// </summary>
        public event GridDataNewRecordAddedEventHandler RecordAdded;

        internal GridDataNewRecordAddingEventArgs RaiseRecordAdding(object data)
        {
            var args = new GridDataNewRecordAddingEventArgs(data);
            this.OnRecordAdding(args);
            return args;
        }


        internal void RaiseRecordAdded(RecordEntry entry)
        {
            var args = new GridDataNewRecordAddedEventArgs(entry);
            this.OnRecordAdded(args);
        }

        /// <summary>
        /// Raises the <see cref="E:RecordAdding"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridDataNewRecordAddingEventArgs"/> instance containing the event data.</param></param>
        protected virtual void OnRecordAdding(GridDataNewRecordAddingEventArgs args)
        {
            if (this.RecordAdding != null)
            {
                this.RecordAdding(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RecordAdded"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridDataNewRecordAddedEventArgs"/> instance containing the event data.</param></param>
        protected virtual void OnRecordAdded(GridDataNewRecordAddedEventArgs args)
        {
            if (this.RecordAdded != null)
            {
                this.RecordAdded(this, args);
            }
        }


        #endregion

        #region InitializingNewItem

        public event GridDataInitializingNewItemEventHandler InitializingNewItem;

        internal object RaiseInitializingNewItem(object newItem)
        {
            var args = new GridDataInitializingNewItemEventArgs();
            args.NewItem = newItem;
            this.OnRaiseInitializingNewItem(args);
            return args.NewItem;
        }

        protected virtual void OnRaiseInitializingNewItem(GridDataInitializingNewItemEventArgs args)
        {
            if (this.InitializingNewItem != null)
            {
                this.InitializingNewItem(this, args);
            }
        }

        #endregion

        /// <summary>
        /// Occurs when the current cell is changed.
        /// </summary>
        public event GridDataCurentCellChangedEventHandler CurrentCellChanged;

        internal void RaiseCurrentCellChanged(string colName, object currentValue, int recordIndex)
        {
            var args = new GridDataCurrentCellChanged()
            {
                ColumnName = colName,
                CurrentValue = currentValue,
                RecordIndex = recordIndex
            };

            var handler = this.CurrentCellChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        internal bool RaiseCurrentCellValidatingEvent(string columnName, object value)
        {
            GridDataCurrentCellValidatingEventArgs args = new GridDataCurrentCellValidatingEventArgs()
            {
                ColumnName = columnName,
                Value = value
            };

            if (this.CurrentCellValidating != null)
            {
                this.CurrentCellValidating(this, args);
            }

            return args.Cancel;
        }

        internal void RefreshChildVisualStyles(VisualStyle visualStyle)
        {
            var view = this.Model.View;
            if (view != null && view.Records != null && view.Records.Count > 0)
            {
                if (!this.HasGroups)
                {
                    foreach (GridDataRecord r in view.Records)
                    {
                        foreach (var kvp in r.ChildModels)
                        {
                            kvp.Value.TableProperties.VisualStyle = visualStyle;
                        }
                    }
                }
                else
                {
                    this.RefreshChildVisualStylesForGroup(this.Model.View.TopLevelGroup, visualStyle);
                }
            }
        }

        internal void RefreshChildVisualStyles(VisualStyle visualStyle, bool forceRefresh)
        {
            var view = this.Model.View;
            if (view != null && view.Records != null && view.Records.Count > 0)
            {
                if (!this.HasGroups)
                {
                    foreach (GridDataRecord r in view.Records)
                    {
                        foreach (var kvp in r.ChildModels)
                        {
                            if (forceRefresh)
                                kvp.Value.TableProperties.ClearValue(GridDataTableProperties.VisualStyleProperty);

                            kvp.Value.TableProperties.VisualStyle = visualStyle;
                        }
                    }
                }
                else
                {
                    this.RefreshChildVisualStylesForGroup(this.Model.View.TopLevelGroup, visualStyle);
                }
            }
        }

        private void RefreshChildVisualStylesForGroup(Group parentGroup, VisualStyle visualStyle)
        {
            foreach (Group grp in parentGroup.Groups)
            {
                if (grp.IsBottomLevel)
                {
                    foreach (GridDataRecord r in grp.Records)
                    {
                        foreach (var kvp in r.ChildModels)
                        {
                            kvp.Value.TableProperties.VisualStyle = visualStyle;
                        }
                    }
                }
                else
                {
                    this.RefreshChildVisualStylesForGroup(grp, visualStyle);
                }
            }
        }

        internal bool RaiseGridRecordBeforeBeginEdit(int recordIndex)
        {
            var args = new GridDataValueCancelEventArgs<int>(recordIndex);
            if (this.RecordBeforeBeginEdit != null)
            {
                this.RecordBeforeBeginEdit(this, args);
            }

            return !args.Cancel;
        }

        internal void RaiseGridRecordAfterBeginEdit(int record)
        {
            if (this.RecordAfterBeginEdit != null)
            {
                this.RecordAfterBeginEdit(this, new GridDataValueEventArgs<int>(record));
            }
        }

        internal void RaiseGridRecordEndEdit(int record)
        {
            if (this.RecordEndEdit != null)
            {
                this.RecordEndEdit(this, new GridDataValueEventArgs<int>(record));
            }
        }

        internal bool RaiseGridRecordDeleting(GridDataRecordDeletingEventArgs args)
        {
            if (this.RecordDeleting != null)
            {
                this.RecordDeleting(this, args);
            }
            return args.Cancel;
        }

        internal void RaiseGridRecordDeleted(object record, int recordindex)
        {
            var args = new GridDataRecordDeletedEventArgs();
            args.Record = record;
            args.Records = new List<object>(new object[] {record});
            args.RecordIndex = recordindex;
            this.RaiseGridRecordDeleted(args);
        }

        internal void RaiseGridRecordDeleted(GridDataRecordDeletedEventArgs args)
        {
            if (this.RecordDeleted != null)
            {
                this.RecordDeleted(this, args);
            }
        }

        internal void RaiseGridRecordCollapsedEvent(GridDataRecord record)
        {
            if (this.RecordCollapsed != null)
            {
                this.RecordCollapsed(this, new GridDataValueEventArgs<GridDataRecord>(record));
            }
        }

        internal bool RaiseGridRecordCollapsingEvent(GridDataRecord record)
        {
            var args = new GridDataValueCancelEventArgs<GridDataRecord>(record);
            if (this.RecordCollapsing != null)
            {
                this.RecordCollapsing(this, args);
            }

            return !args.Cancel;
        }

        internal void RaiseGridRecordExpandedEvent(GridDataRecord record)
        {
            if (this.RecordExpanded != null)
            {
                this.RecordExpanded(this, new GridDataValueEventArgs<GridDataRecord>(record));
            }
        }

        internal GridDataRecordExpandingEventArgs RaiseGridRecordExpandingEvent(GridDataRecord record)
        {
            var args = new GridDataRecordExpandingEventArgs() { Record = record };

            if (this.RecordExpanding != null)
            {
                this.RecordExpanding(this, args);
            }

            return args;
        }

        internal GridDataDetailsViewExpandingEventArgs RaiseGridDetailsViewExpandingEvent(GridDataRecord record)
        {
            var args = new GridDataDetailsViewExpandingEventArgs(record);
            if (this.DetailsViewExpanding != null)
                this.DetailsViewExpanding(this, args);

            return args;
        }

        internal void RaiseGridDetailsViewExpandedEvent(GridDataDetailsViewExpandingEventArgs args)
        {
            var Expandedargs = new GridDataDetailsViewExpandedEventArgs(args.Record);
            if (this.DetailsViewExpanded != null)
                this.DetailsViewExpanded(this, Expandedargs);
        }

        internal bool RaiseGridDetailsViewCollapsingEvent(GridDataRecord record)
        {
            var args = new GridDataValueCancelEventArgs<GridDataRecord>(record);
            if (this.DetailsViewCollapsing != null)
                this.DetailsViewCollapsing(this, args);

            return args.Cancel;
        }

        internal void RaiseGridDetailsViewCollapsedEvent(GridDataRecord record)
        {
            var args = new GridDataValueEventArgs<GridDataRecord>(record);
            if (this.DetailsViewCollapsed != null)
                this.DetailsViewCollapsed(this, args);
        }

        internal bool RaiseSortColumnsChanging(IList<GridDataSortColumn> addedColumns, IList<GridDataSortColumn> removedColumns, NotifyCollectionChangedAction action)
        {
            var args = new GridDataSortColumnsChangingEventArgs(addedColumns, removedColumns, action);
            var handler = this.SortColumnsChanging;
            if (handler != null)
            {
                handler(this, args);
            }
            if (this.Model.TableProperties.SortColumnChangingCommand != null && this.Model.TableProperties.SortColumnChangingCommand.CanExecute(args))
            {
                this.Model.TableProperties.SortColumnChangingCommand.Execute(args);
            }

            return !args.Cancel;
        }

        internal void RaiseSortColumnsChanged(IList<GridDataSortColumn> addedColumns, IList<GridDataSortColumn> removedColumns, NotifyCollectionChangedAction action)
        {
            var args = new GridDataSortColumnsChangedEventArgs(addedColumns, removedColumns, action);
            var handler = this.SortColumnsChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        internal void RaiseGroupedColumnsChanged(IList<GridDataGroupColumn> addedColumns, IList<GridDataGroupColumn> removedColumns, NotifyCollectionChangedAction action)
        {
            var args = new GridDataGroupedColumnsChangedEventArgs(addedColumns, removedColumns, action);
            var handler = this.GroupedColumnsChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        internal void RefreshHiddenColumns()
        {
            var hiddenColumns = this.Model.TableProperties.VisibleColumns.Select(v => v.IsHidden).ToList();
            var lineSizeCollection = this.Model.ColumnWidths as LineSizeCollection;
            var startIdx = this.Model.ResolveDefaultColumnOffset();
            for (int i = 0; i < startIdx; i++)
            {
                hiddenColumns.Insert(0, false);
            }

            lineSizeCollection.SetHiddenInterval(0, this.Model.ColumnCount, hiddenColumns.ToArray());
#if SILVERLIGHT
            this.Model.InvalidateVisual(true);
#endif
        }

        private bool alreadyInRefresh = false;

        internal void Refresh()
        {
            if (this.alreadyInRefresh)
            {
                return;
            }

            this.alreadyInRefresh = true;
            this.RefreshRows();

            /// For Row Details Template
            if (this.Model.SourceListCount > 0 && this.HasNestedTables || this.HasDetailsView)
            {
                if (!this.HasGroups)
                {
                    // if we have a group and only if it is expanded we need to add the hidden pattern
                    // set the pattern for hiding rows
                    bool[] hiddenPattern = this.ResolveHiddenPattern();
                    var startIdx = this.Model.ResolveStartIndexBasedOnPosition();

                    int endIdx = 0;
                    if (this.Model.TableProperties.TableSummaryPosition == Position.Bottom)
                    {
                        endIdx = this.Model.RowCount - this.Model.TableProperties.TableSummaryRows.Count;
                    }
                    else
                    {
                        endIdx = this.Model.RowCount;
                    }
                    var lineSizeCollection = this.Model.RowHeights as LineSizeCollection;

                    lineSizeCollection.SetHiddenInterval(startIdx, endIdx, hiddenPattern.ToArray());
                    if (this.HasNestedTables)
                        this.Model.ExpandedRecordCount = 0; // when the child record is expanded and modfied the child visible column the ExpandedRecrodCount is not reset before.
                }
                else
                {
                    ResetExpandedState(this.GroupModel);
                }
            }

            this.alreadyInRefresh = false;
        }

        internal bool[] ResolveHiddenPattern()
        {
            List<bool> hiddenPattern = new List<bool>() { false };

            if (this.HasDetailsView)
                hiddenPattern.Add(true);

            this.Model.TableProperties.Relations.ForEach<GridDataRelation>(r => hiddenPattern.Add(true));
            return hiddenPattern.ToArray();
        }

        internal GridDataTable RootTable
        {
            get
            {
                GridDataChildTableModel dctm = this.Model as GridDataChildTableModel;
                if (dctm == null)
                {
                    return this;
                }

                GridDataTable parentTable = null;
                while (dctm != null)
                {
                    parentTable = dctm.ParentTable;
                    dctm = parentTable.Model as GridDataChildTableModel;
                }
                return parentTable;
            }
        }

        internal GridDataTableModel RootModel
        {
            get
            {
                GridDataTable rootTable = this.RootTable;
                if (rootTable != null)
                {
                    return rootTable.Model;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has conditional formats.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has conditional formats; otherwise, <c>false</c>.
        /// </value>
        public bool HasConditionalFormats
        {
            get
            {
                return this.Model.TableProperties.ConditionalFormats.Count > 0 ? true : false;
            }
        }

#if SyncfusionFramework4_0
        internal bool isDynamicSourceEvaluated = false;
#endif
        private bool isDynamicBound = false;
        /// <summary>
        /// Gets a value indicating whether this instance is dynamic bound.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is dynamic bound; otherwise, <c>false</c>.
        /// </value>
        public bool IsDynamicBound
        {
            get
            {
#if SyncfusionFramework4_0
                if (this.Model.View == null)
                {
                    return false;
                }

                if (!this.isDynamicSourceEvaluated)
                {
                    var record = this.Model.View.Records.Count > 0 ? this.Model.View.Records[0] : null;
                    if (record != null)
                    {
                        if (record.Data != null)
                        {
                            this.isDynamicBound = DynamicHelper.CheckIsDynamicObject(record.Data.GetType());
                        }

                        if (this.Model.TableProperties != null)
                        {
                            if (this.Model.TableProperties.SourceType != null && this.Model.TableProperties.SourceType.FullName == "IronRuby.Ruby")
                                this.isDynamicBound = false;
                        }
                    }
                    this.isDynamicSourceEvaluated = true;
                }
#endif
                return this.isDynamicBound;
            }
        }

#if SyncfusionFramework4_0
        private DynamicHelper dynamicHelper = null;
        internal DynamicHelper GetDynamicInstance()
        {
            if (this.dynamicHelper == null)
            {
                this.dynamicHelper = new DynamicHelper();
            }

            return this.dynamicHelper;
        }
#endif

        internal IEnumerable<string> GetDynamicProperties()
        {
            if (!this.IsDynamicBound)
            {
                yield return string.Empty;
            }
#if SyncfusionFramework4_0
            var dynObj = this.Model.View.Records[0].Data as System.Dynamic.IDynamicMetaObjectProvider;
            if (dynObj != null)
            {
                var metaType = dynObj.GetType();
                var metaData = dynObj.GetMetaObject(System.Linq.Expressions.Expression.Parameter(metaType, metaType.Name));
                foreach (var propName in metaData.GetDynamicMemberNames())
                {
                    yield return propName;
                }
            }
#endif
        }

        /// <summary>
        /// Gets a value indicating whether this instance has expression columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has expression columns; otherwise, <c>false</c>.
        /// </value>
        public bool HasExpressionColumns
        {
            get
            {
                return this.Model.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().FirstOrDefault(col => col.Expression != null) != null;
            }
        }

        internal bool HasAlternateRowBackground
        {
            get
            {
                return this.Model.TableProperties.AlternatingRowBackground != null;
            }
        }

        internal bool HasAlternateRowForeground
        {
            get
            {
                return this.Model.TableProperties.AlternatingRowForeground != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has unbound columns.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has unbound columns; otherwise, <c>false</c>.
        /// </value>
        public bool HasUnboundColumns
        {
            get
            {
                return this.Model.TableProperties.VisibleColumns.OfType<GridDataUnboundVisibleColumn>().Count() > 0;
            }
        }


        private bool validatedIDataErrorInfo = false;
        private bool hasIDataErrorInfo = false;
        internal bool HasIDataErrorInfo
        {
            get
            {
                if (!this.validatedIDataErrorInfo)
                {
                    if (this.Model.SourceListCount > 0)
                    {
                        this.hasIDataErrorInfo = ((RecordEntry)this.Model.View.Records[0]).Data as IDataErrorInfo != null ? true : false;
                        this.validatedIDataErrorInfo = true;
                    }
                }

                return this.hasIDataErrorInfo;
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance has groups.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has groups; otherwise, <c>false</c>.
        /// </value>
        public bool HasGroups
        {
            get
            {
                var result = false;
                if (this.Model.View != null && this.Model.View.GroupDescriptions.Count > 0)
                {
                    result = true;
                }
                return result;
            }
        }

        /// <summary>
        /// Gets the group model.
        /// </summary>
        /// <value>The group model.</value>
        public GridDataTopLevelGroup GroupModel
        {
            get
            {
                return this.Model.View.TopLevelGroup as GridDataTopLevelGroup;
            }
        }

        private void RefreshRows()
        {
            if (!this.HasGroups)
            {
                // add UI header row count
                int count = this.Model.SourceListCount;
                var rowHeights = this.Model.RowHeights as LineSizeCollection;
                rowHeights.SuspendUpdates();
                if (count > 0)
                {
                    var nestedTableCount = this.HasNestedTables ? count * (this.Model.TableProperties.Relations.Count + 1) : count + this.Model.TableProperties.HeaderRows;
                    //Adding Details View row based on its existance
                    count = this.HasDetailsView ? nestedTableCount + count : nestedTableCount;
                }
                else
                {
                    // ensure header rows is shown
                    count = this.Model.TableProperties.HeaderRows;
                }

                if (this.Model.TableProperties.HeaderRows == 0)
                {
                    count -= this.Model.TableProperties.HeaderRows;
                }

                count += this.Model.UnboundRowsCount;
                count += count > 0 && this.Model.TableProperties.ShowAddNewRow ? 1 : 0;
                count += count > 0 && this.Model.TableProperties.ShowFilterBar ? 1 : 0;
                if (this.Model.SourceListCount > 0)
                {
                    count += this.HasNestedTables ? 1 : 0;
                }

                if (this.Model.TableProperties.TableSummaryPosition == Position.Bottom)
                {
                    count += this.HasTableSummaries ? this.Model.TableProperties.TableSummaryRows.Count : 0;
                }
                else
                {
                    count += this.HasTableSummaries ? this.Model.TableProperties.TableSummaryRows.Count : 0;
                }
                count += this.HasStackedHeaders ? this.Model.TableProperties.StackedHeaderRows.Count : 0;
                this.Model.RowCount = count;

                var childModel = this.Model as GridDataChildTableModel;
                if (childModel != null && childModel.ParentTable.Model.TableProperties.AllowNestedGridPadding)
                {
                    rowHeights.PaddingDistance = 19;
                }
                rowHeights.ResetHiddenState();
                rowHeights.ResumeUpdates();
            }
            else
            {
                var rowHeights = this.Model.RowHeights as LineSizeCollection;
                rowHeights.SuspendUpdates();
                var count = this.GroupModel.DisplayElements.Count + 1;
                // count = this.Model.HeaderRows + count;
                /*if (this.HasNestedTables)
                {
                    var nestedHiddenCount = this.GroupModel.DisplayElements.Where(o => o.GroupType == GridDataGroupType.Item).Count() * (this.Model.TableProperties.Relations.Count + 1);
                    count += nestedHiddenCount;
                }*/
                count += this.Model.UnboundRowsCount;
                count += count > 0 && this.Model.TableProperties.ShowAddNewRow ? 1 : 0;
                count += count > 0 && this.Model.TableProperties.ShowFilterBar ? 1 : 0;
                if (this.Model.TableProperties.TableSummaryPosition == Position.Bottom)
                {
                    count += this.HasTableSummaries ? this.Model.TableProperties.TableSummaryRows.Count : 0;
                }
                else
                {
                    count += this.HasTableSummaries ? this.Model.TableProperties.TableSummaryRows.Count : 0;
                }
                count += this.HasStackedHeaders ? this.Model.TableProperties.StackedHeaderRows.Count : 0;
                this.Model.RowCount = count;

                var childModel = this.Model as GridDataChildTableModel;
                if (childModel != null && childModel.ParentTable.Model.TableProperties.AllowNestedGridPadding)
                {
                    rowHeights.PaddingDistance = 19;
                }

                rowHeights.ResetHiddenState();
                rowHeights.ResumeUpdates();
            }

            this.SetHeaderRows();
            this.SetFrozenRows();
            this.SetFooterRows();
            this.SetFrozenColumns();
            // Below code is commented for the fix of SD15560.
            //if (this.Model.HeaderRows > 1)
            //{
            //    for (int i = 0; i < this.Model.HeaderRows; i++)
            //    {
            //        //this.Model.RowHeights[i] = GridDataTableModel.HeaderRowHeight;
            //        this.Model.RowHeights[i] = this.Model.TableProperties.DefaultHeaderRowHeight;
            //    }
            //}
            //The following code added for the fix SD8370 (this.Midel.Grid != null)
            if (this.Model.Grid !=null && (HasNestedTables | HasDetailsView) && !this.Model.CurrencyManager.IsInAddNewRow && this.Model.IsSourceListReset)
            {
                //To find the existance of expanded relatio and expanded details view
                var hasExpandedRows = this.Model.View.Records.FirstOrDefault(r => r.IsExpanded || (r is GridDataRecord && (r as GridDataRecord).IsDetailsViewExpanded)) != null;
                if (hasExpandedRows)
                {
                    //Iterating through the records to reset the expand state on refreshing the rows.
                    foreach (var record in this.Model.View.Records)
                    {
                        var gridRecord = record as GridDataRecord;

                        if (gridRecord != null && gridRecord.IsExpanded)
                        {
                            if (this.Model.ExpandedRecordCount > 0)
                                this.Model.ExpandedRecordCount--;
                            gridRecord.IsExpanded = false;
                        }

                        if (gridRecord != null && gridRecord.IsDetailsViewExpanded)
                            gridRecord.IsDetailsViewExpanded = false;
                    }
                }
            }

            this.RootModel.InvalidateVisual(true);
            this.validatedIDataErrorInfo = false;
        }

        private void SetHeaderRows()
        {
            // Compute and set HeaderRows
            var headerRows = this.Model.TableProperties.HeaderRows;
            ////if (this.Model.TableProperties.TableSummaryPosition == Position.Bottom)
            ////{
            ////    this.Model.FooterRows = this.HasTableSummaries ? this.Model.TableProperties.TableSummaryRows.Count : this.Model.TableProperties.FooterRows;
            ////}
            ////else
            ////{
            ////    //headerRows = this.HasTableSummaries ? this.Model.TableProperties.TableSummaryRows.Count + headerRows : headerRows;
            ////    //headerRows = this.HasStackedHeaders ? headerRows + this.Model.TableProperties.StackedHeaderRows.Count : headerRows;
            ////    //this.Model.HeaderRows = headerRows;
            ////    //this.Model.FrozenRows = this.Model.HeaderRows;
            ////}
            headerRows = this.HasStackedHeaders ? this.Model.TableProperties.StackedHeaderRows.Count + headerRows : headerRows;
            this.Model.HeaderRows = headerRows;
        }

        internal void SetFrozenRows()
        {
            // compute and set frozen rows
            var frozenRows = this.Model.TableProperties.FrozenRows;
            if (this.Model.TableProperties.TableSummaryPosition == Position.Top)
            {
                frozenRows = this.HasTableSummaries ? this.Model.TableProperties.TableSummaryRows.Count + frozenRows : frozenRows;
            }
            frozenRows = this.HasStackedHeaders ? this.Model.TableProperties.StackedHeaderRows.Count + frozenRows : frozenRows;
            frozenRows += this.Model.TableProperties.ShowFilterBar ? 1 : 0;
            this.Model.FrozenRows = frozenRows;
        }

        internal void SetFrozenColumns()
        {

            var frozenColumns = this.Model.TableProperties.FrozenColumns >= this.Model.TableProperties.HeaderColumns + (this.Model.TableProperties.ShowRowHeader == true ? 1 : 0) ?
                this.Model.TableProperties.FrozenColumns : this.Model.TableProperties.HeaderColumns + (this.Model.TableProperties.ShowRowHeader == true ? 1 : 0);
            this.Model.FrozenColumns = frozenColumns;
        }

        internal void SetFooterRows()
        {
            if (this.Model.TableProperties.TableSummaryPosition == Position.Bottom)
            {
                if (this.HasTableSummaries)
                    this.Model.FooterRows = this.Model.TableProperties.TableSummaryRows.Count > this.Model.TableProperties.FooterRows ? this.Model.TableProperties.TableSummaryRows.Count : this.Model.TableProperties.FooterRows;
                else
                    this.Model.FooterRows = this.Model.TableProperties.FooterRows; ;
            }
            else
                this.Model.FooterRows = this.Model.TableProperties.FooterRows;
        }

        internal void SetDirty()
        {
            //this.isInitialized = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            //this.isInitialized = false;

            if (disposing)
            {
                //this.recordStates.Clear();
                //this.Model = null;

#if SyncfusionFramework4_0
                if (this.dynamicHelper != null)
                {
                    this.dynamicHelper.Dispose();
                }
#endif
            }

            base.Dispose(disposing);
        }

        internal void RefreshDeltaColumnWidth()
        {
            // TODO - CHECK HERE
            //if (this.recordStates.Values.Where(r => r.IsExpanded).Count() == 0)
            //{
            //    this.Model.ColumnWidths[this.Model.ColumnCount - 1] = 0;
            //}
        }

        ///// <summary>
        ///// Raised after a record is deleted
        ///// </summary>
        //public event EventHandler RecordDeleted;

        //internal void RaiseGridRecordDeleted()
        //{
        //    var handler = this.RecordDeleted;
        //    if (handler != null)
        //    {
        //        handler(this, EventArgs.Empty);
        //    }
        //}

        /// <summary>
        /// Gets a value specifying if the Model has filters applied.
        /// </summary>
        public bool HasFilters
        {
            get { return this.Model.View.CanFilter; }
        }
    }


    public enum SortClickAction
    {
        //GridDataControl sorting on SingleClick
        SingleClick,

        //GridDataControl sorting on DoulbeClick
        DoubleClick
    }
}
