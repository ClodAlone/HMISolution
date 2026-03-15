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
    using System.Windows;
    using System.Windows.Controls;
    using Syncfusion.Linq;
    using Syncfusion.Windows.ComponentModel;
    using System.Diagnostics;
    using Syncfusion.Windows.Controls.Scroll;
    using System.Windows.Media;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Data;
#if !SILVERLIGHT
    using System.Data;
#else
    using System.Reflection;
#endif
    using Syncfusion.Windows.Collections.Generic;
    using System.Collections.Specialized;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Input;

    /// <summary>
    /// CurrencyManager for Grid Data Table Model. It maintains all the editing work and updates the underlying source.
    /// </summary>
    /// <remarks>
    /// Use the shortcut keys Insert and Delete to insert or delete the current focused record.
    /// </remarks>
    public class GridDataCurrentRecordManager : Disposable
    {
        private Exception exception = null;

        //Setting the IsEditing Property to false when UpdateMode is LostFocus
        private bool skipResetEndEdit = false;
        internal bool isLastRowCol = false;
        internal object newItem;
        private object newItemCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyManager"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public GridDataCurrentRecordManager(GridDataTableModel model)
        {
            this.Model = model;
            this.CachedStorage = new Dictionary<string, object>();
            this.AddNewRowCachedStorage = new Dictionary<string, object>();
            this.CurrentRow = new Dictionary<string, object>();
            this.IsInDelete = false;
        }

        /// <summary>
        /// Occurs when current record selection is changed.
        /// </summary>
        public event GridDataCurrentRecordSelectionChangedEventHandler CurrentRecordSelectionChanged;

        /// <summary>
        /// Occurs when current record selection is changing.
        /// </summary>
        public event GridDataCurrentRecordSelectionChangingEventHandler CurrentRecordSelectionChanging;




        /// <summary>
        /// Occurs when the cell goes into begin edit.
        /// </summary>
        public event EventHandler OnBeginEdit;

        /// <summary>
        /// Occurs when the cell/row goes into end edit.
        /// </summary>
        public event EventHandler OnEndEdit;

        /// <summary>
        /// Gets the current cell.
        /// </summary>
        /// <value>The current cell.</value>
        public GridCurrentCell CurrentCell
        {
            get
            {
                if (Model == null || Model.Grid == null)
                    return null;
                return this.Model.Grid.CurrentCell;
            }
        }

        //private UpdateMode updateMode = UpdateMode.LostFocus;
        private int prevColIndex = -1;
        private UpdateMode currentUpdateMode = UpdateMode.Empty;
        public UpdateMode UpdateMode
        {
            get
            {
                var column = this.GetCurrentColumn();
                if (column != null)
                {
                    if (prevColIndex != this.CurrentCell.ColumnIndex || this.currentUpdateMode != column.UpdateMode)
                    {
                        prevColIndex = this.CurrentCell.ColumnIndex;
                        var updateMode = column.UpdateMode;
                        this.currentUpdateMode = updateMode;
                    }
                }
                else
                {
                    this.currentUpdateMode = UpdateMode.Empty;
                }

                return this.currentUpdateMode;
            }
            //set
            //{
            //    if (this.updateMode != value)
            //    {
            //        this.updateMode = value;
            //        if (this.updateMode == UpdateMode.PropertyChanged)
            //        {
            //            this.shouldGridFocusOnEdit = false;
            //        }
            //        else
            //        {
            //            this.shouldGridFocusOnEdit = true;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Gets the index of the current row with the Grid's row index.
        /// </summary>
        /// <value>The index of the current row.</value>
        private int currentRowIndex;
        public int CurrentRowIndex
        {
            get { return this.currentRowIndex; }
            internal set
            {
                this.currentRowIndex = value;
            }
        }

        /// <summary>
        /// Gets the index of the current record based on the Records collection.
        /// </summary>
        /// <value>The index of the current record.</value>
        public int CurrentRecordIndex
        {
            get
            {
                var currentRowIndex = this.CurrentRowIndex;
                if (currentRowIndex > -1 && !this.IsInAddNewRow)
                {
                    var rowIdx = this.Model.ResolveIndexToRecordPosition(currentRowIndex);
                    return rowIdx;
                }

                return -1;
            }
        }

        /// <summary>
        /// This flag used to check, is record deleting message box already shown, while multi record deleting
        /// </summary>
        internal bool IsDeleteMessageBoxShown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        private bool isEditing = false;
        public bool IsEditing
        {
            get
            {
                return this.isEditing;
            }

            private set
            {
                if (this.isEditing != value)
                {
                    this.isEditing = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public GridDataTableModel Model
        {
            get;
            internal set;
        }

        public bool IsInAddNewRow
        {
            get
            {
                return this.Grid.CurrentCell.RowIndex >= 0
                    && this.Model.ResolveAddNewPositionInGrid() != -1
                    && this.Grid.CurrentCell.RowIndex == this.Model.ResolveAddNewPositionInGrid();
            }
        }

        internal bool IsInFilterBarRow
        {
            get
            {
                return this.Grid.CurrentCell.RowIndex >= 0
                    && this.Model.ResolveFilterBarPositionInGrid() != -1
                    && this.Grid.CurrentCell.RowIndex == this.Model.ResolveFilterBarPositionInGrid();
            }
        }

        private Dictionary<string, object> CachedStorage
        {
            get;
            set;
        }

        private Dictionary<string, object> AddNewRowCachedStorage
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the current row.
        /// </summary>
        /// <value>The current row.</value>
        private Dictionary<string, object> CurrentRow
        {
            get;
            set;
        }

        private GridControlBase Grid
        {
            get
            {
                if (this.Model.Grid != null)
                {
                    return this.Model.Grid;
                }

                return null;
            }
        }

        public bool IsModified
        {
            get
            {
                return this.CachedStorage.Count > 0;
            }
        }

        public bool HasError
        {
            get
            {
                GridDataRecord Rec = null;
                Rec = this.Model.View.Records[this.CurrentRecordIndex] as GridDataRecord;
                if (Rec.ErrorList.Count == 0)
                    return false;
                else
                    return true;
            }
        }


        private int lastBeginEditRowIndex = -2;
        /// <summary>
        /// Begins the editing process. Specify the record index to identify the <see cref="GridDataRecord"/> and start editing on it.
        /// </summary>
        public void BeginEdit(int recordIndex)
        {
            try
            {
                if ((!this.IsEditing || this.IsInAddNewRow) && this.IsRecordCell && !this.IsInDelete)
                {
                    this.IsEditing = true;
                    // Check the current record is not Addnew Row and Unbound Row as this should affect underlying collection.
                    if (!this.IsInAddNewRow && !this.Model.IsInUnboundRow(this.currentRowIndex))
                    {
                        if (recordIndex > -1 && this.Model.Table.RaiseGridRecordBeforeBeginEdit(recordIndex))
                        {
                            RecordEntry record = null;
                            if (!this.Model.Table.HasGroups)
                            {
                                record = this.Model.View.Records[recordIndex] as RecordEntry;
                            }
                            else
                            {
                                record = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                            }
                            if (record != null)
                            {
                                this.Model.View.EditItem(record.Data);
                                this.Model.Table.RaiseGridRecordAfterBeginEdit(recordIndex);
                            }
                        }
                    }
                    else
                    {
#if !SILVERLIGHT
                        if (!this.Model.IsLegacyDataTable)
#endif

                        {
                            if (CurrentCell.RowIndex == lastBeginEditRowIndex && newItem != null)
                            {
                                foreach (var column in this.Model.TableProperties.VisibleColumns)
                                {
                                    var value = this.Model.Table.GetValue(this.newItem, column.MappingName) as string;
                                    if (value != null && value != string.Empty)
                                    {
                                        if (this.CachedStorage.ContainsKey(column.MappingName))
                                        {
                                            if (this.CachedStorage[column.MappingName].Equals(value))
                                            {
                                                this.CachedStorage[column.MappingName] = value;
                                            }
                                        }
                                        else
                                        {
                                            if (this.IsModified && (!this.newItemCache.Equals(newItem)
                                                && this.Model.TableProperties.AddNewRowBehaviour == AddNewRowBehaviour.AtleastOneItem)
                                                | (this.Model.TableProperties.AddNewRowBehaviour == AddNewRowBehaviour.Default))
                                            {
                                                this.CachedStorage.Add(column.MappingName, value);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                this.newItem = this.newItemCache = this.Model.Table.RaiseInitializingNewItem(this.Model.View.AddNew());

                                this.CachedStorage.Clear();
                                if (this.Model.TableProperties.AddNewRowBehaviour == AddNewRowBehaviour.Default)
                                {
                                    foreach (var column in this.Model.TableProperties.VisibleColumns)
                                    {
                                        var value = this.Model.Table.GetValue(this.newItem, column.MappingName) as string;
                                        if (!string.IsNullOrEmpty(value) && this.CurrentCell.IsModified)
                                        {
                                            this.CachedStorage.Add(column.MappingName, value);
                                        }
                                    }
                                }
                            }
                        }
#if !SILVERLIGHT
                        else
                        {
                            if (CurrentCell.RowIndex == lastBeginEditRowIndex && newItem != null)
                            {
                                foreach (var column in this.Model.TableProperties.VisibleColumns)
                                {
                                    var value = this.Model.Table.GetValue(this.newItem, column.MappingName) as string;
                                    if (!string.IsNullOrEmpty(value))
                                    {
                                        if (this.CachedStorage.ContainsKey(column.MappingName))
                                        {
                                            this.CachedStorage[column.MappingName] = value;
                                        }
                                        else
                                        {
                                            this.CachedStorage.Add(column.MappingName, value);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var source = this.Model.View.SourceCollection;
                                var table = source is DataView ? ((DataView)source).Table : (DataTable)source;
                                this.newItem = this.Model.Table.RaiseInitializingNewItem(table.NewRow());
                                this.CachedStorage.Clear();
                                foreach (var column in this.Model.TableProperties.VisibleColumns)
                                {
                                    if (!(column is GridDataUnboundVisibleColumn))
                                    {
                                        var dRow = this.newItem as DataRow;
                                        var value = dRow[column.MappingName] as string;
                                        if (!string.IsNullOrEmpty(value))
                                        {
                                            this.CachedStorage.Add(column.MappingName, value);
                                        }
                                    }
                                }
                            }
                        }
#endif
                    }

                    this.lastBeginEditRowIndex = CurrentCell.RowIndex;

                }
            }
            catch { }
        }

        /// <summary>
        /// Confirms the changes that were last done with the row.
        /// </summary>
        public void ConfirmChanges()
        {
            //this.CurrentCell.EndEdit();
            // this would clear the cached data  
            if (this.UpdateMode == Controls.Grid.UpdateMode.RowCachedMode && this.CurrentRecordIndex >= 0)
            {
                var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                GridDataRecord record=null;
                if (this.Model.Table.HasGroups)
                {
                    record = this.Model.View.TopLevelGroup.DisplayElements[this.CurrentRowIndex] as GridDataRecord;
                }
                else
                {
                    record = this.Model.View.Records[this.CurrentRecordIndex] as GridDataRecord;
                }
                if(this.CurrentCell.IsEditing||this.IsEditing)
                    this.CurrentCell.EndEdit();

                if (!dataGrid.RaiseRowValueCommittingEvent(record, this.CachedStorage, this.CurrentRowIndex))
                {
                    if (this.EndEdit())
                    {
                        dataGrid.RaiseRowValueCommittedEvent(record, this.CachedStorage, this.CurrentRowIndex);
                    }
                    this.Model.Data.Clear();
                }
                else
                {
                    dataGrid.RaiseRowValueCommittingCancelledEvent(record, this.CurrentRowIndex);
                    this.CancelEdit();
                }
            }
            else
            {
                this.Model.Data.Clear();
                this.EndEdit();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                this.UnwireEvents();
                if (this.Model.Grid != null)
                this.CachedStorage.Clear();
                this.CachedStorage = null;
                this.AddNewRowCachedStorage.Clear();
                this.AddNewRowCachedStorage = null;
            }
        }

        internal void Reset()
        {
            this.CurrentRowIndex = -1;
        }

        private bool CanAddNewRow()
        {
            bool addNewRow = false;

            if (!this.Model.Table.IsDynamicBound)
            {
#if !SILVERLIGHT
                this.Model.View.GetItemProperties().ForEach<PropertyDescriptor>(pd =>
                {
                    var cacheval = this.Model.CurrencyManager.GetValueFromCache(pd.Name);
                    var value = cacheval != null ? cacheval.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(value) && !pd.IsReadOnly)
                    {
                        addNewRow = true;
                    }
                });
#else
                this.Model.View.GetItemProperties().ForEach<KeyValuePair<string, PropertyInfo>>(pd =>
                {
                    var value = this.Model.CurrencyManager.GetValueFromCache(pd.Value.Name);
                    if (value != null && pd.Value.CanWrite)
                    {
                        addNewRow = true;
                    }
                });
#endif
            }
            return addNewRow;
        }

        public bool IsInEndEdit
        {
            get;
            private set;
        }

        /// <summary>
        /// Ends the edit for the current record. If the editing is done for the AddNew row, a new record
        /// would be added, otherwise it is a simple update to the underlying source based on the <see cref="GridDataVisibleColumn.UpdateMode"/>.
        /// </summary>
        public bool EndEdit()
        {
            if (this.IsInEndEdit)
            {
                return false;
            }

            var result = true;
            var move = false;

            bool shouldAddNewItem = false;

            if (this.currentRowIndex == this.Model.ResolveAddNewPositionInGrid()
                && this.Model.ResolveAddNewPositionInGrid() != -1)
            {
                shouldAddNewItem = true;
            }

            if (((this.CurrentCell.IsEditing || this.IsEditing) && (this.IsRecordCell || this.IsInAddNewRow || shouldAddNewItem)) || (this.UpdateMode == UpdateMode.PropertyChanged && !this.CurrentCell.IsEditing))
            {
                this.IsInEndEdit = true;
                if (this.UpdateMode != UpdateMode.PropertyChanged || (this.UpdateMode == UpdateMode.PropertyChanged && !this.CurrentCell.IsEditing) || this.IsInAddNewRow)
                {
                    this.CurrentCell.EndEdit();
                }

                if ((this.IsInAddNewRow || shouldAddNewItem) && (this.CanAddNewRow() || this.Model.TableProperties.AddNewRowBehaviour == AddNewRowBehaviour.Default))
                {
                    try
                    {
                        // since its an add new row, we simply add it to the end of the source list count
#if !SILVERLIGHT
                        if (!this.Model.IsLegacyDataTable)
#endif
                        {
                            this.Model.SuspendEvents();

                            var data = this.newItem;

                            if (data != null)
                            {
                                // Variable is assigned but it is never used
                                Boolean IsAllFieldsAreEmpty = true;
                                //Boolean IsAllFieldsAreEntered = true;

                                // Type objType = data.GetType();
                                // D:TODO - Implement SetValue call from here
                                var properties = this.Model.View.GetItemProperties();
                                this.Model.TableProperties.VisibleColumns.ForEach<GridDataVisibleColumn>(v =>
                                {
                                    var value = this.GetValueFromAddNewRowCache(v.MappingName);
                                    if (value != null)
                                    {
                                        IsAllFieldsAreEmpty = false;
                                        var pDesc = properties[v.MappingName];
                                        // var pInfo = objType.GetProperty(v.MappingName);
                                        if (pDesc != null)
                                        {
                                            if (NullableHelperInternal.IsComplexType(pDesc.PropertyType))
                                            {
                                                if (v.ValueConverter != null)
                                                {
                                                    value = v.ValueConverter.ConvertBack(value, null, v.ValueConverterParameter == null ? v.MappingName : v.ValueConverterParameter, System.Globalization.CultureInfo.CurrentCulture);
                                                }
                                            }

                                            value = NullableHelper.FixDbNUllasNull(value, pDesc.PropertyType);
                                            value = NullableHelper.ChangeType(value, pDesc.PropertyType);
                                            // if we match the value with the property type, then set it
                                            if (value != null || !(value is DBNull))
                                            {
                                                pDesc.SetValue(data, value);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        value = this.GetValueFromCache(v.MappingName);
                                        if (value != null)
                                            IsAllFieldsAreEmpty = false;
                                        //IsAllFieldsAreEntered = false;
                                    }
                                });

                                var identityColumns = this.Model.TableProperties.VisibleColumns.Where(v => v.IsIdentity);
#if !SILVERLIGHT
                                if (identityColumns.Count() > 0 && !this.Model.IsLegacyDataTable && this.Model.SourceListCount > 0)
#else
                                if (identityColumns.Count() > 0 && this.Model.SourceListCount>0)
#endif
                                {
                                    identityColumns.ForEach<GridDataVisibleColumn>(v =>
                                    {
                                        var pDesc = properties[v.MappingName];
                                        // var pInfo = objType.GetProperty(v.MappingName);
                                        // get max value and increment that according to the seed
                                        var maxValue = (int)this.Model.SourceList.AsQueryable().Max(v.MappingName);
                                        pDesc.SetValue(data, maxValue + v.IncrementSeed);
                                    });
                                }
                                var args = this.Model.Table.RaiseRecordAdding(data);
                                if (args.Handled)
                                {
                                    data = args.Data;
                                }

                                if (this.Model.TableProperties.AddNewRowBehaviour == AddNewRowBehaviour.Default)
                                {
                                    if (!args.Cancel)
                                    {
                                        this.Model.ResumeEvents();
                                        this.Model.View.CommitNew();

                                        //This code after add the new record we have to Refresh the paged source
                                        if (this.Model.View.EnablePaging && this.Model.View.PagedSource != null)
                                            this.Model.View.PagedSource.Refresh();

                                        this.Model.Table.RaiseRecordAdded(this.Model.View.Records.GetRecord(data));
                                        newItem = null;
                                    }
                                    else
                                    {
                                        result = false;
                                        newItem = null;
                                        if (shouldAddNewItem)
                                        {
                                            var view = this.Model.View as CollectionViewAdv;
                                            if (view != null)
                                            {
                                                view.CancelNew(true, false);
                                                this.Model.ResumeEvents();

                                            }
                                        }

                                    }
                                }
                                else if (this.Model.TableProperties.AddNewRowBehaviour == AddNewRowBehaviour.AtleastOneItem)
                                {
                                    if (!args.Cancel && !IsAllFieldsAreEmpty)
                                    {
                                        this.Model.ResumeEvents();
                                        this.Model.View.CommitNew();
                                        this.Model.Table.RaiseRecordAdded(this.Model.View.Records.GetRecord(data));
                                        newItem = null;
                                    }
                                    else
                                    {
                                        result = false;
                                        newItem = null;
                                        //this.Model.ResumeEvents();
                                        //this.Model.View.CommitNew();
                                        //return false;
                                        //data = null;
                                        if (shouldAddNewItem)
                                        {
                                            var view = this.Model.View as CollectionViewAdv;
                                            if (view != null)
                                            {
                                                view.CancelNew(true, false);
                                            }
                                        }
                                    }
                                }
                            }
                        }
#if !SILVERLIGHT
                        else
                        {
                            //this.Model.SuspendEvents();
                            var source = this.Model.View.SourceCollection;
                            var table = source is DataView ? ((DataView)source).Table : (DataTable)source;
                            var drow = this.newItem as DataRow;
                            this.Model.TableProperties.VisibleColumns.ForEach<GridDataVisibleColumn>(v =>
                            {
                                var value = this.GetValueFromCache(v.MappingName);
                                if (value != null)
                                {
                                    drow[v.MappingName] = value;
                                }
                            });

                            var args = this.Model.Table.RaiseRecordAdding(drow);

                            if (args.Handled)
                                drow = args.Data as DataRow;

                            if (!args.Cancel)
                            {
                                //this.Model.SuspendEvents();
                                bool canaddnewrow = true;

                                // When the Data Table is modified using MD Data Grid the collections adds and empty row before we add the data in an MS DataGrid Add new Row. 
                                //  Hence the the new item added to the list is validated before adding here. 
                                if (drow != null && this.Model.TableProperties.AddNewRowBehaviour != AddNewRowBehaviour.Default)
                                {
                                    canaddnewrow = false;
                                    foreach (var value in drow.ItemArray)
                                    {
                                        if (value.ToString() != "" && value != null)
                                        {
                                            canaddnewrow = true;
                                        }
                                    }
                                }
                                if (canaddnewrow)
                                {
                                    table.Rows.Add(drow);
                                    this.Model.Table.RaiseRecordAdded(this.Model.View.Records.FirstOrDefault(d =>
                                    {
                                        var data = d.Data as DataRowView;
                                        if (data.Row == args.Data)
                                        {
                                            return true;
                                        }

                                        return false;
                                    }));
                                }
                            }

                            //if (this.Model.Table.HasGroups)
                            //{
                            //    var groupList = this.Model.View.TopLevelGroup as IGroupList;
                            //    groupList.Add(drow);
                            //}
                            this.newItem = null;
                            drow = null;

                            //else
                            //{
                            //    this.Model.View.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, drow));
                            //}
                            //this.Model.ResumeEvents();
                        }

#endif
                        // adding to the record will automatically push the data into the underlying datasource
                        var lineSizeCollection = this.Model.RowHeights as LineSizeCollection;
                        lineSizeCollection.SuspendUpdates();
                        this.ResetCache();
                        // hide everything before adding up any new row
                        this.Model.Table.HideAllUIRows();
                        this.Model.RefreshDisplay(false);
                        this.isNewRecordAdded = true;
                        lineSizeCollection.ResumeUpdates();
                        this.Model.InvalidateCell(GridRangeInfo.Row(this.CurrentRowIndex));
                        this.Model.Table.RootModel.InvalidateVisual(true);
                        this.Model.Data.Clear();
                        move = true;
                        if (result == false)
                            move = false;
                    }
                    catch (Exception ex)
                    {
                        this.exception = ex;
                        result = false;
                    }
                }
                else if (this.IsInAddNewRow && !this.CanAddNewRow() && this.CurrentCell.IsInMoveTo)
                {
                    this.newItem = null;
                    var view = this.Model.View as CollectionViewAdv;
                    if (view != null)
                    {
                        view.CancelNew(true, true);
                    }
                }
                else
                {
                    try
                    {
                        foreach (KeyValuePair<string, object> kvp in this.CachedStorage.ToList())
                        {
                            this.Model.Table.SetValue(this.CurrentRecordIndex, kvp.Key, kvp.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.exception = ex;
                        result = false;
                    }
                }

                if (result)
                {
                    if (!this.skipResetEndEdit)
                    {
                        this.IsEditing = false;
                    }

                    //If paging is enabled means after editing we have to refresh the paged source
                    if (this.Model.View.PagedSource != null)
                    {
                        var NotifyProperty = this.Model.View.PagedSource as INotifyPropertyChanged;
                        NotifyProperty.PropertyChanged += new PropertyChangedEventHandler(NotifyProperty_PropertyChanged);
                    }


                    this.ResetCache();
                    this.Model.ResumeEvents();
                    // Check the current record is not Unbound Row as this should affect underlying collection.
                    if (!this.Model.IsInUnboundRow(this.Grid.CurrentCell.RowIndex))
                    {
                        RecordEntry recordEntry = !this.IsInAddNewRow && !this.IsInFilterBarRow && this.CurrentRowIndex >= 0
                                                      ? this.GetCurrentRecord()
                                                      : null;
                        var expandedState = recordEntry != null ? recordEntry.IsExpanded : false;
                        if (this.Model.View.IsEditingItem && !this.skipResetEndEdit)// && this.Model.Table.HasGroups))
                        {
                            var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
                            if (colIdx >= 0)
                            {
                                var column = this.Model.TableProperties.VisibleColumns[colIdx];
                                    (this.Model.View as CollectionViewAdv).CommitEdit(column.MappingName);
                            }
                            else
                                this.Model.View.CommitEdit();
                        }
                        /*else if (this.Model.View.IsAddingNew)
                        {
                            this.Model.View.CommitNew();
                        }*/

                        if (!this.IsInAddNewRow && !this.IsInFilterBarRow && this.CurrentRowIndex >= 0)
                        {
                            var currentRecordAfterEdit = this.GetCurrentRecord();
                            if (currentRecordAfterEdit != null)
                            {
                                currentRecordAfterEdit.IsExpanded = expandedState;
                            }
                        }
                    }

                    this.RaiseOnEndEdit();

                    var addNewRowIndex = this.Model.ResolveAddNewPositionInGrid();
                    if (this.Model.Grid != null && this.IsInAddNewRow)
                    {
                        for (int i = this.Model.ResolveDefaultColumnOffset(); i < this.Model.ColumnCount; i++)
                        {
                            var rowColIndex = new RowColumnIndex(addNewRowIndex, i);
                            //this.Model.Grid.ArrangedCellUIElements.Unload(rowColIndex);
                            if (rowColIndex.RowIndex >= 0 && rowColIndex.ColumnIndex >= 0)
                                this.Model.Grid.RefreshCellUIElementsContent(rowColIndex);
                        }

                        if (!this.IsInAddNewRow)
                        {
                            //this.Model.InvalidateCell(GridRangeInfo.Row(addNewRowIndex));
                        }
                    }

                    if (this.IsInAddNewRow)
                    {
                        if (move && !this.IsInDeactivate && !this.isLastRowCol && !this.CurrentCell.IsInMoveTo)
                        {
                            this.CurrentCell.MoveTo(addNewRowIndex, this.Model.ResolveDefaultColumnOffset());
#if !SILVERLIGHT

                            if (!this.Grid.IsFocused)
                                this.Grid.Focus();
#endif
                            this.CurrentCell.ScrollInView();
                        }
                    }
                }
                this.IsInEndEdit = false;
                if (this.UpdateMode != Controls.Grid.UpdateMode.PropertyChanged)
                    this.IsEditing = false;       // IsEditing property should be set to False as we end editing here or else it will not enter BeginEdit() method. 
            }

            else if (this.CachedStorage.Count > 0)
                this.ResetCache();
           
            return result;
        }


        void NotifyProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Model.View != null && this.Model.TableProperties.EnablePaging && this.Model.View.PagedSource != null && !this.Model.RefreshFromFilter)
            {
                var NotifyProperty = this.Model.View.PagedSource as INotifyPropertyChanged;
                NotifyProperty.PropertyChanged -= new PropertyChangedEventHandler(NotifyProperty_PropertyChanged);
                this.Model.InvalidateDisplay();
            }
        }


        private RecordEntry GetCurrentRecord()
        {
            RecordEntry recordEntry = null;
            if (!this.Model.Table.HasGroups)
            {
                if (this.CurrentRecordIndex > -1 && this.CurrentRecordIndex < this.Model.View.Records.Count)
                    recordEntry = this.Model.View.Records[this.CurrentRecordIndex];
            }
            else
            {
                if (this.CurrentRecordIndex > -1)
                    recordEntry = this.Model.View.TopLevelGroup.DisplayElements[this.CurrentRecordIndex] as RecordEntry;
            }
            return recordEntry;
        }

        public bool IsInCancelEdit
        {
            get;
            private set;
        }

        /// <summary>
        /// Cancels the edit.
        /// </summary>
        public void CancelEdit()
        {
            this.IsInCancelEdit = true;
            this.CurrentCell.CancelEdit();
            this.ResetCache();
            if (this.Model.View != null)
            {
                this.Model.View.CancelEdit();
            }
            this.IsEditing = false;
            this.IsInCancelEdit = false;
            if (this.IsInAddNewRow && this.Model.View != null)
            {
                var view = this.Model.View as CollectionViewAdv;
                view.CancelNew(true, false);
            }
        }

        internal void RemoveRows(int removeAtRowIndex, int count, GridDataCurrentRecordMoveState moveState)
        {
            var rowIndex = this.CurrentRowIndex;

            if (rowIndex >= removeAtRowIndex)
            {
                if (rowIndex >= removeAtRowIndex + count)
                {
                    rowIndex -= count;
                }
                else if (moveState != null)
                {
                    moveState.Index = rowIndex - removeAtRowIndex;
                }
                //else
                //{
                //    rowIndex = -1;
                //}
            }

            this.CurrentRowIndex = rowIndex;
#if DEBUG
            if (this.Model.Grid != null)
            {
                Debug.WriteLine("CRIndex At Remove -> " + this.CurrentRowIndex);
                Debug.WriteLine("CCRIndex at Remove -> " + this.CurrentCell.RowIndex);
            }
#endif
        }

        internal void InsertRows(int insertAtRowIndex, int count, GridDataCurrentRecordMoveState moveState)
        {
            var rowIndex = this.CurrentRowIndex;
            if (rowIndex >= insertAtRowIndex)
            {
                rowIndex += count;
            }

            if (moveState != null)
            {
                if (moveState.Index != -1)
                {
                    rowIndex = moveState.Index + insertAtRowIndex;
                }
            }

            this.CurrentRowIndex = rowIndex;
#if DEBUG
            Debug.WriteLine("CRIndex At Insert -> " + this.CurrentRowIndex);
            Debug.WriteLine("CCRIndex at Insert -> " + this.CurrentCell.RowIndex);
#endif
        }

        /// <summary>
        /// Moves to the specified row index and column index.
        /// </summary>
        /// <param name="rowIdx">The row idx.</param>
        /// <param name="colIdx">The col idx.</param>
        public void MoveTo(int rowIdx, int colIdx)
        {
            var actualRowIdx = this.Model.ResolvePositionToIndex(rowIdx);
            if (this.Grid != null && this.CurrentCell != null)
            {
                if (this.CurrentRowIndex != actualRowIdx || this.CurrentRowIndex != this.CurrentCell.RowIndex)
                {
                    if (this.IsEditing)
                    {
                        this.CancelEdit();
                    }

                    var activateOptions = this.CurrentCell.ActivateOptions;
                    var shouldBeginInvoke = activateOptions == null;
                    if (activateOptions != null && activateOptions.IsActivateTriggeredByMouseDownIntoUIElement)
                    {
                        shouldBeginInvoke = true;
                    }

                    if (this.Model.IsInGroup || this.Model.IsInFilter || this.Model.IsInSort)
                        shouldBeginInvoke = false;

                    if (!shouldBeginInvoke)
                    {                                                
                        this.CurrentCell.MoveTo(actualRowIdx, colIdx);
                        this.CurrentCell.ScrollInView();
                        this.CurrentRowIndex = actualRowIdx;
                    }
                    else
                    {
                        this.Grid.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                var datagrid = this.Grid.FindParentElementOfType<GridDataControl>();
                                if (datagrid != null)
                                {

                                    if (datagrid.Model != null && datagrid.Model.View != null && datagrid.Model.View.Records != null && datagrid.Model.View.Records.Count > 0)
                                    {
                                        this.CurrentCell.MoveTo(actualRowIdx, colIdx);
                                        this.CurrentCell.ScrollInView();
                                        this.CurrentRowIndex = actualRowIdx;
                                    }
                                }
                                else
                                {
                                    this.CurrentCell.MoveTo(actualRowIdx, colIdx);
                                    this.CurrentCell.ScrollInView();
                                    this.CurrentRowIndex = actualRowIdx;
                                }
                            }
                            catch
                            { }
                        }));

                    }
                }
                   
                else if (this.CurrentCell.RowIndex >= this.CurrentRowIndex)
                {
                    this.CurrentRowIndex = this.CurrentCell.RowIndex;
                }
                
            }
        }

        /// <summary>
        /// Moves to the specified row idx.
        /// </summary>
        /// <param name="rowIdx">The row idx.</param>
        public void MoveTo(int rowIdx)
        {
            if (this.Model == null || this.Model.Grid == null)
                return;

            var colIdx = 0;

            if (CurrentCell.ColumnIndex <= 0)
            {
                colIdx = this.Model.ResolveDefaultColumnOffset();
            }
            else
            {                
                //if (this.Model.SortColumnVisibleIndexPosition >= 0)
                //{
                //    colIdx = this.Model.ResolveVisibleColumnIndexToPosition(this.Model.SortColumnVisibleIndexPosition);
                //}
                //else
                //{
                    colIdx = CurrentCell.ColumnIndex;
                //}
            }

            if (this.Model is GridDataChildTableModel)
            {
                if (object.Equals(this.Model, this.Grid.Model))
                {
                    this.MoveTo(rowIdx, colIdx);
                }
            }
            else
            {
                this.MoveTo(rowIdx, colIdx);
            }
        }

        /// <summary>
        /// Moves next to the current row index.
        /// </summary>
        public void MoveNext()
        {
            if (this.Model.SourceListCount > 0 && (this.CurrentRowIndex < this.Model.SourceListCount))
            {
                this.CurrentRowIndex += 1;
                int recordIndex = this.Model.ResolveIndexToRecordPosition(this.CurrentRowIndex);
                this.MoveTo(recordIndex);
            }
        }

        /// <summary>
        /// Moves previous to the current row index.
        /// </summary>
        public void MovePrevious()
        {
            int recordIndex = this.Model.ResolveIndexToRecordPosition(this.CurrentRowIndex);

            if (this.Model.SourceListCount > 0 && recordIndex > 0)
            {
                recordIndex -= 1;
                this.MoveTo(recordIndex);
            }
        }

        /// <summary>
        /// Moves to the first row.
        /// </summary>
        public void MoveToFirst()
        {
            if (this.Model.SourceListCount > 0)
            {
                this.CurrentRowIndex = 0;
                this.MoveTo(this.CurrentRowIndex);
            }
        }

        /// <summary>
        /// Moves to the last row.
        /// </summary>
        public void MoveToLast()
        {
            if (this.Model.SourceListCount > 0)
            {
                this.CurrentRowIndex = this.Model.SourceListCount;
                this.MoveTo(this.CurrentRowIndex);
            }
        }

        /// <summary>
        /// Gets the value from cache.
        /// </summary>
        /// <param name="columnMappingName">Name of the column mapping.</param>
        /// <returns></returns>
        public object GetValueFromCache(string columnMappingName)
        {
            object value = null;
            this.CachedStorage.TryGetValue(columnMappingName, out value);
            return value;
        }
        private object GetValueFromAddNewRowCache(string columnMappingName)
        {
            object value = null;
            this.AddNewRowCachedStorage.TryGetValue(columnMappingName, out value);
            return value;
        }
        internal void WireEvents()
        {
            if (this.Grid != null)
            {
                this.Grid.CurrentCellMoving += this.OnCurrentCellMoving;
                this.Grid.CurrentCellMoved += this.OnCurrentCellMoved;
                this.Grid.CurrentCellAcceptedChanges += this.OnGridCurrentCellAcceptedChanges;
#if !SILVERLIGHT
                this.Grid.CurrentCellPreviewKeyDown += new GridCellKeyEventHandler(Grid_CurrentCellPreviewKeyDown);
                this.Grid.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(Grid_PreviewKeyDown);
#else
                this.Grid.CurrentCellKeyDown += new GridCellKeyEventHandler(Grid_CurrentCellPreviewKeyDown);
                this.Grid.KeyDown += new System.Windows.Input.KeyEventHandler(Grid_PreviewKeyDown);
#endif
                this.Grid.CurrentCellChanging += this.OnGridCurrentCellChanging;
                this.Grid.CurrentCellChanged += new GridRoutedEventHandler(OnGridCurrentCellChanged);
                this.Grid.PrepareRenderCell += this.OnGridPrepareRenderCell;
                this.Grid.CurrentCellStartEditing += new GridCancelRoutedEventHandler(OnGridCurrentCellStartEditing);
                this.Grid.CurrentCellEditingComplete += new GridRoutedEventHandler(OnGridCurrentCellEditingComplete);
                this.Grid.CurrentCellDeactivated += new GridCurrentCellDeactivatedEventHandler(OnGridCurrentCellDeactivated);
#if !SILVERLIGHT

                this.Grid.CurrentCellDeactivating += new GridCancelRoutedEventHandler(OnGridCurrentCellDeactivating);
#else
                this.Grid.CurrentCellDeactivating += new SyncfusionCancelEventHandler(OnGridCurrentCellDeactivating);
#endif
            }
        }

        internal void UnwireEvents()
        {
            if (this.Grid != null)
            {
                this.Grid.CurrentCellMoving -= this.OnCurrentCellMoving;
                this.Grid.CurrentCellMoved -= this.OnCurrentCellMoved;
                this.Grid.CurrentCellAcceptedChanges -= this.OnGridCurrentCellAcceptedChanges;
#if !SILVERLIGHT
                this.Grid.CurrentCellPreviewKeyDown -= new GridCellKeyEventHandler(Grid_CurrentCellPreviewKeyDown);
                this.Grid.PreviewKeyDown -= new System.Windows.Input.KeyEventHandler(Grid_PreviewKeyDown);
#else
                this.Grid.CurrentCellKeyDown -= new GridCellKeyEventHandler(Grid_CurrentCellPreviewKeyDown);
                this.Grid.KeyDown -= new System.Windows.Input.KeyEventHandler(Grid_PreviewKeyDown);
#endif
                this.Grid.CurrentCellChanging -= this.OnGridCurrentCellChanging;
                this.Grid.CurrentCellChanged -= new GridRoutedEventHandler(OnGridCurrentCellChanged);
                this.Grid.PrepareRenderCell -= this.OnGridPrepareRenderCell;
                this.Grid.CurrentCellStartEditing -= new GridCancelRoutedEventHandler(OnGridCurrentCellStartEditing);
                this.Grid.CurrentCellEditingComplete -= new GridRoutedEventHandler(OnGridCurrentCellEditingComplete);
                this.Grid.CurrentCellDeactivated -= new GridCurrentCellDeactivatedEventHandler(OnGridCurrentCellDeactivated);
#if !SILVERLIGHT
                this.Grid.CurrentCellDeactivating -= new GridCancelRoutedEventHandler(OnGridCurrentCellDeactivating);
#else
                this.Grid.CurrentCellDeactivating -= new SyncfusionCancelEventHandler(OnGridCurrentCellDeactivating);
#endif
            }
        }

        void OnGridCurrentCellChanged(object sender, SyncfusionRoutedEventArgs args)
        {
            if (this.UpdateMode == UpdateMode.PropertyChanged && this.IsRecordCell && !this.IsInAddNewRow && !this.IsInEndEdit)
            {
                if (!this.AcceptChanges())
                {
                    this.EndEdit();

                    if (!this.CurrentCell.CellRowColumnIndex.IsEmpty)
                    {
                        if (this.CurrentCell.Renderer != null)
                        {
                            this.CurrentCell.Renderer.UpdateCurrentStyle();
                        }
                    }
                }
                else
                {
                    this.CancelEdit();
                }
            }
        }

        private GridDataVisibleColumn GetCurrentColumn()
        {
            var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
            var column = colIdx > -1 && colIdx < this.Model.TableProperties.VisibleColumns.Count ? this.Model.TableProperties.VisibleColumns[colIdx] : null;
            return column;
        }

        void OnGridCurrentCellEditingComplete(object sender, SyncfusionRoutedEventArgs args)
        {
            if (this.IsInCancelEdit)
            {
                return;
            }

            var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
            var column = colIdx > -1 && colIdx < this.Model.TableProperties.VisibleColumns.Count ? this.Model.TableProperties.VisibleColumns[colIdx] : null;
            if (column != null && this.IsRecordCell && !this.IsInAddNewRow)
            {
                if (!this.Model.TableProperties.AllowEdit || column.IsReadOnly)
                {
                    return;
                }

                if (!this.IsInAddNewRow && column != null && (column.IsReadOnly || column.IsIdentity))
                {
                    return;
                }

                // If it is complex property block the editing option
                if (!this.Model.Table.IsDynamicBound && this.Model.View != null)
                {
                    var itemProperties = this.Model.View.GetItemProperties();
                    if (itemProperties != null)
                    {
                        if (column.MappingName != null)
                        {
                            var pd = itemProperties.GetPropertyDescriptor(column.MappingName);
                            if (pd != null)
                            {
#if !SILVERLIGHT
                                if (pd.IsReadOnly)
#else
                            if (!pd.CanWrite)
#endif
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            // Check the current record is not Addnew Row and Unbound Row as this should affect underlying collection.
            if (this.UpdateMode == UpdateMode.LostFocus && this.IsRecordCell && !this.IsInAddNewRow && !this.Model.IsInUnboundRow(this.Grid.CurrentCell.RowIndex))
            {
                if (!this.AcceptChanges())
                {
                    // this.EndEdit();
                }
                else
                {
                    this.CancelEdit();
                }
            }
            else if (this.UpdateMode == UpdateMode.PropertyChanged && this.IsRecordCell && !this.Model.IsInUnboundRow(this.Grid.CurrentCell.RowIndex))
            {
                if (this.skipResetEndEdit)
                {
                    this.skipResetEndEdit = false;
                }
            }
        }

        object record = null;

        /// <summary>
        /// Called when [grid current cell deactivating].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Syncfusion.Windows.ComponentModel.SyncfusionCancelRoutedEventArgs"/> instance containing the event data.</param>       
        
#if !SILVERLIGHT

        void OnGridCurrentCellDeactivating(object sender, SyncfusionCancelRoutedEventArgs args)
#else
        void OnGridCurrentCellDeactivating(object sender, SyncfusionCancelEventArgs args)
#endif
        {

            this.AddCurrentRowDetails();
            var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();

            if (dataGrid != null && this.CurrentRowIndex >= 0 && this.Grid.CurrentCell.MoveToRowIndex != this.CurrentRowIndex)
            {                
                if (CurrentRow.Count>0 && !dataGrid.RaiseRowValidatingEvent(this.Grid.CurrentCell.RowIndex, record, CurrentRow))
                {
                    args.Cancel = true;
                }
                else
                {
                    var recordIndex = dataGrid.Model.ResolveIndexToRecordPosition(this.Grid.CurrentCell.MoveToRowIndex);
                    if (this.Model.View!=null && recordIndex < this.Model.View.Records.Count && recordIndex >= 0)
                    {
                        if (this.Model.Table.HasGroups)
                        {
                            record = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as GridDataRecord;
                        }
                        else
                        {
                            record = this.Model.View.Records[recordIndex] as GridDataRecord;
                        }
                    }

                    this.CurrentRow.Clear();
                }
            }
        }

        /// <summary>
        /// Adds the current row details.
        /// </summary>
        private void AddCurrentRowDetails()
        {
            var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
            if (colIdx <= -1 || this.CurrentRowIndex <= -1 || this.Model.TableProperties.VisibleColumns.Count <= colIdx)
            {
                return;
            }
            var column = this.Model.TableProperties.VisibleColumns[colIdx];
            if (!(column is GridDataUnboundVisibleColumn))
            {
                if (record == null)
                {
                    var recordIndex = this.Model.ResolveIndexToRecordPosition(this.CurrentRowIndex);
                    if (this.Model.View!=null&&recordIndex < this.Model.View.Records.Count && recordIndex >= 0)
                    {
                        if (this.Model.Table.HasGroups)
                        {
                            record = this.Model.View.TopLevelGroup.DisplayElements[recordIndex];
                        }
                        else
                        {
                            record = this.Model.View.Records.GetItemAt(recordIndex);
                        }
                    }
                }
                if (this.CurrentCell.Renderer != null)
                {
                    object currentValue = this.CurrentCell.Renderer.ControlValue;
                    object value = null;
                    this.CurrentRow.TryGetValue(column.MappingName, out value);
                    if (this.CurrentCell.IsModified)
                    {
                        if (!this.CurrentRow.ContainsKey(column.MappingName) && value == null)
                        {
                            this.CurrentRow.Add(column.MappingName, currentValue);
                        }
                        else if (value != null)
                        {
                            if (currentValue != null)
                            {
                                this.CurrentRow[column.MappingName] = currentValue;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method would call an endedit when deactivated and when UpdateMode=LostFocus, For RowCached, it would be handled in FocusRow method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnGridCurrentCellDeactivated(object sender, GridCurrentCellDeactivatedEventArgs args)
        {
            if (this.UpdateMode == UpdateMode.LostFocus && this.IsRecordCell && !this.IsInAddNewRow && !this.Model.IsInUnboundRow(this.currentRowIndex))
            {
                if (this.IsModified)
                {
                    this.EndEdit();
                }
            }
            // Check the current record is not Addnew Row and Unbound Row as this should affect underlying collection.
            if (this.UpdateMode == UpdateMode.PropertyChanged && this.IsRecordCell && !this.IsInAddNewRow && !this.Model.IsInUnboundRow(this.currentRowIndex))
            {
                if (!this.CurrentCell.IsEditing)
                {
                    //        var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex); //this.ResolveColumnIndexToPosition();
                    //        if (colIdx >= 0)
                    //        {
                    //            var column = this.Model.TableProperties.VisibleColumns[colIdx];
                    //            (this.Model.View as CollectionViewAdv).CommitEdit(column.MappingName);
                    //        }
                    //        else
                    //        {
                    //            this.Model.View.CommitEdit();
                    //        }
                    this.EndEdit();
                }
            }
            
        }

        private void OnGridCurrentCellChanging(object sender, SyncfusionCancelRoutedEventArgs args)
        {
            var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
            var column = colIdx > -1 && colIdx < this.Model.TableProperties.VisibleColumns.Count ? this.Model.TableProperties.VisibleColumns[colIdx] : null;
            if (column != null && this.IsRecordCell && !this.IsInAddNewRow)
            {
                if (!this.Model.TableProperties.AllowEdit || column.IsReadOnly)
                {
                    args.Cancel = true;
                    return;
                }

                if (!this.IsInAddNewRow && column != null && (column.IsReadOnly || column.IsIdentity))
                {
                    args.Cancel = true;
                    return;
                }

                // If it is complex property block the editing option
                if (!this.Model.Table.IsDynamicBound)
                {
                    var itemProperties = this.Model.View.GetItemProperties();
                    if (itemProperties != null)
                    {
                        var pd = itemProperties.GetPropertyDescriptor(column.MappingName);
                        if (pd != null)
                        {
#if !SILVERLIGHT
                            if (pd.IsReadOnly)
#else
                            if (!pd.CanWrite)
#endif
                            {
                                args.Cancel = true;
                                return;
                            }
                        }
                    }
                }
                /*else
                {
                    args.Cancel = true;
                    return;
                }*/

                if (this.UpdateMode == UpdateMode.PropertyChanged && this.IsRecordCell) 
                    //if (this.UpdateMode == UpdateMode.PropertyChanged && this.IsRecordCell && !this.Model.IsInUnboundRow(this.Grid.CurrentCell.RowIndex))
                {
                    var currentRecordIndex = this.Model.ResolveIndexToRecordPosition(this.CurrentCell.RowIndex);
                    //this.BeginEdit(currentRecordIndex);
                     this.BeginEdit(currentRecordIndex);                   
                    if (!this.skipResetEndEdit)
                    {
                       this.skipResetEndEdit = true;
                    }
                }
            }
        }

        private void OnGridCurrentCellStartEditing(object sender, SyncfusionCancelRoutedEventArgs args)
        {
            this.FocusCurrentRow();
            var currentRecordIndex = this.Model.ResolveIndexToRecordPosition(this.CurrentCell.RowIndex);
            var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
            var column = colIdx > -1 && colIdx < this.Model.TableProperties.VisibleColumns.Count ? this.Model.TableProperties.VisibleColumns[colIdx] : null;
            if (column != null)
            {
                // check only for record cells
                if (this.IsRecordCell && !this.IsInFilterBarRow && !this.IsInAddNewRow)
                {
                    if (!this.Model.TableProperties.AllowEdit || column.IsReadOnly)
                    {
                        args.Cancel = true;
                        return;
                    }

                    // If it is complex property block the editing option
                    if (!this.Model.Table.IsDynamicBound)
                    {
                        var itemProperties = this.Model.View.GetItemProperties();
                        if (itemProperties != null && column.MappingName != null)
                        {
                            var pd = itemProperties.GetPropertyDescriptor(column.MappingName);
                            if (pd != null)
                            {
#if !SILVERLIGHT
                                if (pd.IsReadOnly)

#else
                                if (!pd.CanWrite)
                                //if (column.MappingName.Contains("."))
#endif
                                {
                                    args.Cancel = true;
                                    return;
                                }
                            }
                        }
                    }
                }

                if (this.IsInAddNewRow)
                {
                    this.BeginEdit(currentRecordIndex);
                }
                else if (this.IsRecordCell && !this.IsInFilterBarRow && !this.IsInAddNewRow && !(this.CurrentRowIndex == this.Model.ResolveFilterBarPositionInGrid())) //Line modified for FilterBar issue in silverlight
                {
                    // Previously BeginEdit was called only for RowCahce and LostFocus update mode. We need to call BeginEdit for PropertyChanged also, as CurrentEditItem is not set while editing.
                    if (this.UpdateMode != UpdateMode.Empty) 
                    {
                        if (this.IsRecordCell && (this.Model.TableProperties.AllowEdit || !column.IsReadOnly))/*&& this.isCurrentCellMoved)*/
                        {
                            this.BeginEdit(currentRecordIndex);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prepares all the non editing celltypes to render the grid. 
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Syncfusion.Windows.Controls.Grid.GridPrepareRenderCellEventArgs">GridPrepareRenderCellEventArgs</see> that contains the event data.</param>
        /// <remarks></remarks>
        private void OnGridPrepareRenderCell(object sender, GridPrepareRenderCellEventArgs e)
        {
            if (this.Model.TableProperties.ShowRowHeader && this.Model.TableProperties.ShowRowHeaderArrow)
            {
                if (this.CurrentCell != null)
                {
                    var isNestedGridRenderer = this.CurrentCell.Renderer is GridDataCellNestedGridRenderer;
                    if (isNestedGridRenderer)
                    {
                        return;
                    }
                }

                var dataStyle = (e.Style as GridRenderStyleInfo).ModelStyle as GridDataStyleInfo;
                var currentRowIndex = this.CurrentCell.RowIndex;
                var addNewRowIndex = this.Model.ResolveAddNewPositionInGrid();
              
                if (currentRowIndex > -1 && currentRowIndex != addNewRowIndex && dataStyle != null &&
                    dataStyle.CellIdentity.TableCellType == GridDataTableCellType.RowHeaderCell &&
                    dataStyle.CellIdentity.TableCellType != GridDataTableCellType.AddNewRowHeaderCell)
                {
                    if (this.Model.CurrencyManager.CurrentCell.HasCurrentCell && currentRowIndex == e.Cell.RowIndex && this.Model.TableProperties.ShowRowHeaderArrow)
                    {
                        e.Style.CellType = "RowHeaderCell";
                        e.Style.Background = this.Model.GetRowHeaderSelectionBackground();
                        e.Style.Foreground = this.Model.GetRowHeaderForeground();
                    }
                    else
                    {
                        e.Style.CellType = "Static";
                        e.Style.Background = this.Model.GetRowHeaderBackground();
                        e.Style.Foreground = this.Model.GetHeaderForeground();
                    }
                }
                
                if (!this.Model.TableProperties.ShowAddNewRow)
                    return;

                int actualrowindex = 0;
                var headerIndex = this.Model.HeaderRows > 0 ? this.Model.HeaderRows - 1 : 0;
                // var filterbarindex = this.Model.TableProperties.ShowFilterBar ? 1 : 0; Unused local variable
                if (e.Cell.RowIndex > headerIndex && e.Cell.ColumnIndex >= 0)
                {
                    if (this.Model.TableProperties.ShowRowHeader && e.Cell.ColumnIndex == 0)
                    {
                       actualrowindex +=  this.Model.TableProperties.ShowFilterBar ? 1 : 0;
                       actualrowindex += this.Model.HeaderRows > 0 ? this.Model.HeaderRows : 0;

                       if (this.Model.TableProperties.AddNewRowPosition == Position.Top)
                       {
                           if (this.Model.TableProperties.UnboundRowPosition == Position.Top)
                               actualrowindex += this.Model.UnboundRowsCount;
                           if (this.Model.TableProperties.TableSummaryRows.Count > 0 && Model.TableProperties.TableSummaryPosition == Position.Top)
                               actualrowindex += this.Model.TableProperties.TableSummaryRows.Count;
                       }
                       else
                       {
                           if (this.Model.TableProperties.UnboundRowPosition == Position.Top)
                               actualrowindex = this.Model.RowCount - 1;
                           else
                               actualrowindex = this.Model.RowCount - this.Model.UnboundRowsCount - 1;
                       }
                     
                        if (this.Model.TableProperties.AddNewRowPosition == Position.Top)
                        {
                            if (this.Model.TableProperties.ShowAddNewRow && e.Cell.RowIndex ==  actualrowindex)
                            {
                                e.Style.CellType = "AddNewHeaderCell";
                                dataStyle.CellIdentity.TableCellType = GridDataTableCellType.AddNewRowHeaderCell;
                                //return style;
                            }
                        }
                        else if (this.Model.TableProperties.AddNewRowPosition == Position.Bottom)
                        {
                            if (this.Model.TableProperties.ShowAddNewRow && e.Cell.RowIndex == actualrowindex)
                            {
                                e.Style.CellType = "AddNewHeaderCell";
                                dataStyle.CellIdentity.TableCellType = GridDataTableCellType.AddNewRowHeaderCell;
                                //return style;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:CurrentRecordSelectionChanged"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridDataCurrentRecordSelectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnCurrentRecordSelectionChanged(GridDataCurrentRecordSelectionChangedEventArgs args)
        {
            if (this.CurrentRecordSelectionChanged != null)
            {
                this.CurrentRecordSelectionChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RecordsSelectionChanging"/> event.
        /// </summary>
        /// <param name="args">The <see cref="Syncfusion.Windows.Controls.Grid.GridDataCurrentRecordSelectionChangingEventArgs"/> instance containing the event data.</param>
        protected virtual void OnRecordsSelectionChanging(GridDataCurrentRecordSelectionChangingEventArgs args)
        {
            if (this.CurrentRecordSelectionChanging != null)
            {
                this.CurrentRecordSelectionChanging(this, args);
            }
        }

        private bool AcceptChanges()
        {
            if (this.Model.ResolveFilterBarPositionInGrid() == this.CurrentCell.RowIndex || this.CurrentCell.Renderer == null)
                return false;
            var cannotSave = false;
            GridDataRecord record = null;
            var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex); //this.ResolveColumnIndexToPosition();
            var column = this.Model.TableProperties.VisibleColumns[colIdx];
            if (this.CurrentRecordIndex >= 0 && this.CurrentRecordIndex < this.Model.View.Records.Count)
            {
                record = this.Model.View.Records[this.CurrentRecordIndex] as GridDataRecord;
            }

            object currentValue = this.CurrentCell.Renderer.ControlValue;
            GridDataChildTableModel value = currentValue as GridDataChildTableModel;
            if (value != null)
                return false;

            try
            {
                if (column != null)
                {
                    if (record != null)
                    {
                        record.ErrorList.Clear();
                    }

#if SyncfusionFramework4_0
                    if (!this.Model.Table.IsDynamicBound)
                    {
#endif
                        var itemProperties = this.Model.View.GetItemProperties();
                        if (itemProperties != null || column.ColumnType != null)
                        {
                            var pd = itemProperties.GetPropertyDescriptor(column.MappingName);
                            if (pd != null || column.ColumnType != null)
                            {
                                if (column.ValueConverter != null)
                                {
                                    currentValue = column.ValueConverter.ConvertBack(currentValue, null, column.ValueConverterParameter, System.Globalization.CultureInfo.CurrentCulture);
                                    cannotSave = AddToCache(cannotSave, column, currentValue);
                                }
                                else
                                {
                                    // type check the value before adding it to the cache
                                    //currentValue = NullableHelper.FixDbNUllasNull(currentValue, pd.PropertyType);
                                    //currentValue = NullableHelper.ChangeType(currentValue, pd.PropertyType);
                                    // Date can be an Empty String. Hence the empty string is converted as DBNull.
                                    if ((column.ColumnType.Name == "DateTime" && currentValue != null && currentValue.ToString() == "") || (column.ColumnType.Name == "Boolean" && currentValue == null))
                                    {
                                        currentValue = Convert.DBNull;
                                    }
                                    if (column.Binding != null && column.Binding.Converter != null)
                                    {
                                        currentValue = column.Binding.Converter.ConvertBack(currentValue, null, column.Binding.ConverterParameter, System.Globalization.CultureInfo.CurrentCulture);
                                    }
                                    currentValue = NullableHelper.FixDbNUllasNull(currentValue, pd == null ? column.ColumnType : pd.PropertyType);
                                    currentValue = NullableHelper.ChangeType(currentValue, pd == null ? column.ColumnType : pd.PropertyType);
                                    cannotSave = AddToCache(cannotSave, column, currentValue);
                                }
                            }
                        }
#if SyncfusionFramework4_0
                    }
                    else
                    {
                        var type = column.GetDynamicColumnType();
                        currentValue = NullableHelper.FixDbNUllasNull(currentValue, type);
                        currentValue = NullableHelper.ChangeType(currentValue, type);
                        cannotSave = AddToCache(cannotSave, column, currentValue);
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                this.exception = ex;
                cannotSave = true;
                if (record != null)
                {
                    if (column.MappingName != null)
                        record.ErrorList.Add(column.MappingName, new GridDataErrorInfo(currentValue, ex.Message));

                    this.Model.InvalidateCell(this.CurrentCell.CellRowColumnIndex);
                }
            }

            return cannotSave;
        }

        private bool AddToCache(bool cannotSave, GridDataVisibleColumn column, object currentValue)
        {
            if (this.UpdateMode != Controls.Grid.UpdateMode.RowCachedMode)
                cannotSave = this.Model.Table.RaiseCurrentCellValidatingEvent(column.MappingName, currentValue);
            else
                cannotSave = false;
            if (!cannotSave)
            {
                object value = null;
                this.CachedStorage.TryGetValue(column.MappingName, out value);
                if (!this.CachedStorage.ContainsKey(column.MappingName) && value == null)
                {
                    this.CachedStorage.Add(column.MappingName, currentValue);
                }
                else if (value != null)
                {
                    if (currentValue != null)
                    {
                        this.CachedStorage[column.MappingName] = currentValue;
                    }
                    else
                    {
                        // if we have a null value, it means we need to remove it from the cache alone.
                        this.CachedStorage.Remove(column.MappingName);
                    }
                }
                //Maintain a cache for AddNewRow
                object AddNewValue = null;
                this.AddNewRowCachedStorage.TryGetValue(column.MappingName, out AddNewValue);
                if(!this.AddNewRowCachedStorage.ContainsKey(column.MappingName) && AddNewValue==null)
                    this.AddNewRowCachedStorage.Add(column.MappingName,currentValue);
                else if (AddNewValue != null)
                {
                    if (currentValue != null)
                        this.AddNewRowCachedStorage[column.MappingName] = currentValue;
                    else
                        this.AddNewRowCachedStorage.Remove(column.MappingName);
                }
                var data = (this.Model.View as CollectionViewAdv).CurrentAddItem;
                if (data != null)
                {
                    // Type dataType = data.GetType(); Unused local variable
                    var properties = this.Model.View.GetItemProperties();
                    var pDesc = properties[column.MappingName];
                    if (pDesc != null)
                    {
                        pDesc.SetValue(data, currentValue);
                        // the current value is maintained in data so the current value is cleared form CacheStorage.
                        this.AddNewRowCachedStorage.Remove(column.MappingName);
                    }
                    (this.Model.View as CollectionViewAdv).CurrentAddItem = data;
                }

                this.Model.Table.RaiseCurrentCellChanged(column.MappingName, currentValue, this.CurrentRecordIndex);
                if (this.Model.Table.HasConditionalFormats)
                {
                    this.Model.InvalidateCell(GridRangeInfo.Row(this.CurrentCell.RowIndex));
                    this.Model.InvalidateVisual(true);
                }
            }
            return cannotSave;
        }

        //private void Deactivate()
        //{
        //    this.EndEdit();
        //}

        /// <summary>
        /// Gets a value indicating whether this instance is in deactivate.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in deactivate; otherwise, <c>false</c>.
        /// </value>
        public bool IsInDeactivate
        {
            get;
            private set;
        }

        private void OnGridCurrentCellAcceptedChanges(object sender, SyncfusionRoutedEventArgs args)
        {
            if (((this.UpdateMode == UpdateMode.RowCachedMode || this.UpdateMode== UpdateMode.PropertyChanged)&& this.IsEditing) || this.IsInAddNewRow)
            {
                args.Handled = this.AcceptChanges();
            }
        }

        private bool CurrentCellIsNotNested()
        {
            var result = false;
            if (this.CurrentCell.Renderer != null)
            {
                var style = this.CurrentCell.Renderer.CurrentStyle;
                var dataStyle = style.ModelStyle as GridDataStyleInfo;
                if (dataStyle!=null && dataStyle.CellIdentity != null && dataStyle.CellIdentity.TableCellType == GridDataTableCellType.NestedTableCell)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is in delete.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is in delete; otherwise, <c>false</c>.
        /// </value>
        public bool IsInDelete
        {
            get;
            private set;
        }

        private bool CurrentCellIsHeaderAndPopupOpen()
        {
            var result = false;
            var renderer = this.Grid.CurrentCell.Renderer as GridDataHeaderCellRenderer;
            if (renderer != null && renderer.CurrentCellUIElement != null)
            {
                result = renderer.CurrentCellUIElement.IsColumnOptionsDropDownOpen || renderer.CurrentCellUIElement.IsDropDownOpen;
            }
            return result;
        }

        private bool CheckDataTemplateCell()
        {
            if (this.CurrentCell != null && this.CurrentCell.Renderer != null)
            {
                var style = this.CurrentCell.Renderer.CurrentStyle;
                if ((style != null && style.CellType == "GridDataBoundTemplate")||(style != null && style.CellItemTemplate != null))// && this.Grid.Model.Options.ActivateCurrentCellBehavior != GridCellActivateAction.ClickOnCell)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Handles the CurrentCellPreviewKeyDown event of the Grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Syncfusion.Windows.Controls.Grid.GridCellKeyEventArgs"/> instance containing the event data.</param>
        private void Grid_CurrentCellPreviewKeyDown(object sender, GridCellKeyEventArgs e)
        {
            if (this.CurrentCellIsNotNested() || this.CurrentCellIsHeaderAndPopupOpen())
            {
                return;
            }           

            var checkDtCell = this.CheckDataTemplateCell();
            if (e.Key == System.Windows.Input.Key.Delete && this.Model.TableProperties.AllowDelete && !this.CurrentCell.IsEditing && !checkDtCell && this.CurrentRecordIndex != -1)
            {
                object currentRecord = null;

                if (this.Model.TableProperties.AllowMultipleRecordDeletion)
                {
                    var RangeTop = new List<int>();
                    var records = new List<object>();
                    var isNestedCollection = this.Model.TableProperties.Relations.Count > 0;

                    //Calculating the Row Index from the selected ranges by selection using Shift key
                    if (this.Model.SelectedRanges.Count == 1)
                        RangeTop = this.GetRowIndexFromSingleRange(isNestedCollection);
                    else
                        //Calculating the Row Index from the selected ranges by select records one by one.
                        RangeTop = this.GetRowIndexFromMultiRange();

                    // Storing the selected records in a temperary collection to delete.
                    var recordIndex = 0;
                    if (!this.Model.Table.HasGroups)
                    {
                        for (var row = 0; row < RangeTop.Count; row++)
                            records.Add(this.Model.Table.GetRecordFromRow(RangeTop[row]));
                    }
                    else
                    {
                        for (var row = 0; row < RangeTop.Count; row++)
                        {
                            recordIndex = this.Model.ResolveIndexToGroupPosition(RangeTop[row]);
                            var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[recordIndex] as RecordEntry;
                            records.Add(currentRecord = recordEntry != null ? recordEntry.Data : null);
                        }
                    }
                    this.DeleteRecord(records, RangeTop);
                }
                else
                {
                    var records = new List<object>();
                    if (this.Model.Table.HasGroups)
                    {
                        var recordEntry = this.Model.View.TopLevelGroup.DisplayElements[this.CurrentRecordIndex] as RecordEntry;
                        currentRecord = recordEntry != null ? recordEntry.Data : null;
                        records.Add(currentRecord);

                        if (currentRecord != null)
                            this.DeleteRecord(records, new List<int>(new int[] { this.CurrentRecordIndex }));
#if !SILVERLIGHT
                        this.Model.InvalidateDisplay(false);
#else
                        this.Model.InvalidateDisplay(true);
#endif
                    }
                    else
                    {
                        currentRecord = this.Model.View.Records.GetItemAt(this.CurrentRecordIndex);
                        records.Add(currentRecord);
                        if (currentRecord != null)
                            this.DeleteRecord(records, new List<int>(new int[] {this.CurrentRecordIndex}));
                    }
                }

                //resetting the IsDeleteMessageBoxShown after deleting the Child record.
                if (this.Model is GridDataChildTableModel)
                {
                    var childModel = this.Model as GridDataChildTableModel;
                    if (this.Model.View.Records.Count == 0)
                    {
                        if (childModel.ParentRecord.Model.ExpandedRecordCount > 0)
                            childModel.ParentRecord.Model.ExpandedRecordCount--;
                        if (!this.Model.Table.ShouldExpand(childModel.ParentRecord))
                            childModel.ParentRecord.IsExpanded = false;
                    }
                    childModel.ParentTable.Model.CurrencyManager.IsDeleteMessageBoxShown = false;
#if SILVERLIGHT
                    e.Handled = true;
#else
                    e.Cancel = true;
#endif
                }
                else
                    this.Model.CurrencyManager.IsDeleteMessageBoxShown = false;
            }

            //If the user edit the current cell and pressing escape key should not commit its value to the underlying collection.
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                //this.IsInCancelEdit = true;
                //this.CurrentCell.CancelEdit();                
                if (this.IsEditing)
                {
                    this.CancelEdit();
                    this.Model.InvalidateCell(GridRangeInfo.Row(this.CurrentRowIndex));
                }
#if SILVERLIGHT
                    e.Handled = true;
#endif
            }
        }

        private List<int> GetRowIndexFromSingleRange(bool isNested)
        {
            var RowTop = new List<int>();
            var Activerange = this.Model.SelectedRanges.ActiveRange;            
            int SingleIncrement = 1;
            int DoubleIncrement = 2;
            // Code to get the row index when the selection is Table.
            if (Activerange.IsTable)
            {
                //If the table has groups we expand all groups to calculate rowIndex Correctly.
                if(this.Model.Table.HasGroups)
               		 this.Model.Table.ExpandAllGroups();

                for (int Row = 0; Row < this.Model.RowCount; Row = (isNested == true) ? Row + DoubleIncrement : Row + SingleIncrement)
                    RowTop.Add(Row);                
            }
            else
                // Code to get the row index when the selection is other than Table.
                for (int Top = Activerange.Top; Top <= Activerange.Bottom; Top = (isNested == true) ? Top + DoubleIncrement : Top + SingleIncrement)
                    RowTop.Add(Top);

            return RowTop;
        }
        /// <summary>
        /// Gets the row top from range.
        /// </summary>
        /// <returns></returns>
        private List<int> GetRowIndexFromMultiRange()
        {
            List<int> RowTop = new List<int>();

            var RowRanges = this.Model.SelectedRanges;
            for (int index = 0; index < RowRanges.Count; index++)
                RowTop.Add(RowRanges[index].Top);

            return RowTop;
        }
        /// <summary>
        /// Deletes the record.
        /// </summary>
        /// <param name="currentRecord">The current record.</param>
        /// <param name="RecordIndex">Index of the record.</param>
        private void DeleteRecord(List<object> records, List<int> indexes)
        {
            var args = new GridDataRecordDeletingEventArgs()
                {
                    Records = records,
                    Record = records[0],
                    RecordIndex = indexes[0],
                    Cancel = false,
                };
            var flag = this.Model.Table.RaiseGridRecordDeleting(args);
            if (records.Count <= 0 || flag)
                return;

            var deleteFlag = this.Model is GridDataChildTableModel ? (this.Model as GridDataChildTableModel).ParentTable.Model.CurrencyManager.IsDeleteMessageBoxShown : this.Model.CurrencyManager.IsDeleteMessageBoxShown;
            if (!deleteFlag && !args.Handled)
            {
#if !SILVERLIGHT
                if (MessageBox.Show(GridDataResourceWrapper.DeleteMessage, GridDataResourceWrapper.ConfirmDeleteMessage,
                                    MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.OK) ==
                    MessageBoxResult.Cancel)
                    return;
#else
                    var msg = MessageBox.Show("Do you want to remove the selected record(s)?", "Confirm delete", MessageBoxButton.OKCancel);
                    if (msg == MessageBoxResult.Cancel)
                    {
                        return;
                    }
#endif
                //IsDeleteMessageBoxShown flag set here to avoid message box showing multi times.
                if (this.Model is GridDataChildTableModel)
                    (this.Model as GridDataChildTableModel).ParentTable.Model.CurrencyManager.IsDeleteMessageBoxShown = true;
                else
                    this.Model.CurrencyManager.IsDeleteMessageBoxShown = true;
            }

            if (this.Model.View.CanRemove)
            {
                for (int index = 0; index < args.Records.Count; index++)
                {
                    var d_record = args.Records[index];
                    this.IsInDelete = true;
                    if (this.Model.View.Contains(d_record))
                        this.Model.View.Remove(d_record);
                    this.IsInDelete = false;
                    this.RaiseCurrentRecordSelectionChangedEvent(this.Model.ResolvePositionToIndex(indexes[index]));
                }
                var args1 = new GridDataRecordDeletedEventArgs()
                {
                    Records = records,
                    Record = records[0],
                    RecordIndex = indexes[0],
                };
                this.Model.Table.RaiseGridRecordDeleted(args1);
            }
            //else
            //{
            //    MessageBox.Show(GridDataResourceWrapper.NotSupportDeletingItemMessage, "Not Supported", MessageBoxButton.OK);
            //}
        }

        private void Grid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var isNestedCell = (this.CurrentCell.Renderer as GridDataCellNestedGridRenderer) != null;
            var hasNestedModel = (this.Model as GridDataChildTableModel) != null;
#if !SILVERLIGHT
            bool isShiftKey = (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
#else
            bool isShiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None;
#endif            
            if (isNestedCell && !hasNestedModel)
            {
                return;
            }

#if !SILVERLIGHT
            //Enables the CellRenderer to Handel PrevienKeyDown actions
            if (!this.Grid.shouldGridTryToHandlePreviewKeyDown)
                return;

            if (((e.Key == System.Windows.Input.Key.Insert || e.Key == System.Windows.Input.Key.Enter || e.Key == Key.Return) && this.IsInAddNewRow) || (e.Key == System.Windows.Input.Key.Enter && !this.IsInAddNewRow) || (e.Key == System.Windows.Input.Key.Tab && this.IsInAddNewRow))
            {
#else
            if (((e.Key == System.Windows.Input.Key.Insert || e.Key == System.Windows.Input.Key.Enter) && this.IsInAddNewRow) || (e.Key == System.Windows.Input.Key.Enter && !this.IsInAddNewRow) ||(e.Key == System.Windows.Input.Key.Tab && this.IsInAddNewRow) )
            {
#endif
                if (this.IsEditing)
                {
                    if (this.IsInAddNewRow)
                    {
                        this.AcceptChanges();
                    }
                    if (this.Model is GridDataChildTableModel)
                    {
                        if (object.Equals(this.Model, this.Grid.Model))
                        {
                            this.ConfirmChanges();
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        if (!isShiftKey)
                        {
                            this.ConfirmChanges();
                            this.Model.Grid.InvalidateCells();
                            e.Handled = true;
                        }
                    }
                }
            }
            else if (e.Key == System.Windows.Input.Key.Escape)/* && this.IsInAddNewRow)*/
            {
                if (CurrentCell.IsEditing)
                    this.CurrentCell.CancelEdit();
                // if we press escape twice, then clear the whole row and invalidate
                this.escapePressCount += 1;
                if (this.IsEditing && this.escapePressCount == 2)
                {
                    this.CancelEdit();

                    if (this.Model.View.IsAddingNew)
                    {
                        var view = this.Model.View as CollectionViewAdv;
                        if (view != null)
                        {
                            view.CancelNew(true, false);
                        }
                    }

                    this.escapePressCount = 0;
                    this.Model.InvalidateCell(GridRangeInfo.Row(this.CurrentRowIndex));
                }

                e.Handled = true;
            }
        }

        private int escapePressCount = 0;
        private bool isNewRecordAdded = false;

        private void OnCurrentCellMoved(object sender, GridCurrentCellMovedEventArgs args)
        {
            // header cell
            var headerscount = 1 + this.Model.TableProperties.StackedHeaderRows.Count; //Adding 1 by default for ColumnHeader
            if (this.Grid.CurrentCell.RowIndex <headerscount)
            {
                this.CurrentRowIndex = this.Grid.CurrentCell.RowIndex;
                this.Model.InvalidateCell(GridRangeInfo.Col(this.Grid.CurrentCell.ColumnIndex));
                return;
            }

            if (this.Model.TableProperties.ShowRowHeader && this.Model.TableProperties.ShowRowHeaderArrow)
            {
                this.Model.InvalidateCell(GridRangeInfo.Col(0));
#if SILVERLIGHT

                this.Model.InvalidateVisual();//In Silverlight after changing rows Row Header arrow doesnt get refreshed so we have to Invalidate here.
#endif
            }
   
           
            if (this.Model is GridDataChildTableModel)
            {
                if (object.Equals(this.Model, this.Grid.Model))
                {
                    this.FocusCurrentRow();
                }
            }
            else
            {
                this.FocusCurrentRow();
            }
           
            if (this.IsGroupCaptionPlusMinusCell)
            {
                var grid = this.Grid.FindParentElementOfType<GridDataControl>();
                if (grid != null)
                {
                    if (grid.SelectedItem != null)
                        grid.SelectedItem = null;
                    else if (this.Model.TableProperties.ShowRowHeader && this.Model.TableProperties.ShowRowHeaderArrow)
                        this.Model.InvalidateCell(GridRangeInfo.Cell(this.CurrentCell.MoveFromRowIndex, 0));
                }
            }

            if (this.Model.View != null
                && this.Grid != null
                && this.Grid.CurrentCell != null
                && this.Grid.CurrentCell.HasCurrentCell
                && this.isNewRecordAdded
                && this.Model.View.SortDescriptions.Count > 0
                && this.Grid.CurrentCell.MoveFromRowIndex == this.Model.ResolveAddNewPositionInGrid())
            {
                this.isNewRecordAdded = false;
                this.Model.InvalidateCell(GridRangeInfo.Cell(this.Grid.CurrentCell.RowIndex + 1, this.Grid.CurrentCell.ColumnIndex));
            }

            if (this.Model.View != null && this.Model.View.IsEditingItem && this.IsGroupCaptionCell)
            {
                var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
                if (colIdx >= 0)
                {
                    var column = this.Model.TableProperties.VisibleColumns[colIdx];
                    (this.Model.View as CollectionViewAdv).CommitEdit(column.MappingName);
                }
                else
                    this.Model.View.CommitEdit();
            }
        }

        private void FocusCurrentRow()
        {
            if (this.IsInAddNewRow && this.Model.TableProperties.ShowRowHeaderArrow)
            {
                this.Model.InvalidateCell(GridRangeInfo.Cell(this.Model.ResolveAddNewPositionInGrid(), 0));
            }

            // While deleting the record CurrentRowIndex and CurrentCell.RowIndex are Same, but it should fire the RecordSelectionChangedEvent.
            if (this.Grid.CurrentCell.RowIndex > -1 && !this.Model.IsInGroup && (this.CurrentRowIndex!=this.Grid.CurrentCell.RowIndex || this.Model.IsInDeteteRecord))
            {
                this.IsInDeactivate = true;
                if ((this.IsInAddNewRow && this.IsEditing && !this.CurrentCell.IsInMoveTo) || (this.UpdateMode == UpdateMode.RowCachedMode && this.IsEditing && this.IsModified))
                {
                    if (this.CurrentRecordIndex >= 0)
                    {
                        var dataGrid = this.Grid.FindParentElementOfType<GridDataControl>();
                        GridDataRecord record = null;
                        if (this.Model.Table.HasGroups)
                        {
                            record = this.Model.View.TopLevelGroup.DisplayElements[this.CurrentRecordIndex] as GridDataRecord;
                        }
                        else
                        {
                            record = this.Model.View.Records[this.CurrentRecordIndex] as GridDataRecord;
                        }
                        if (!dataGrid.RaiseRowValueCommittingEvent(record, this.CachedStorage, this.CurrentRowIndex))
                        {
                            if (this.EndEdit())
                                dataGrid.RaiseRowValueCommittedEvent(record, this.CachedStorage, this.CurrentRowIndex);
                        }
                        else
                        {                            
                            dataGrid.RaiseRowValueCommittingCancelledEvent(record, this.CurrentRowIndex);
                            this.CancelEdit();
                        }
                    }
                }
                this.IsInDeactivate = false;

                if (this.CurrentRowIndex > -1 && this.Model.TableProperties.ShowRowHeader)
                {
                    // before we reset the CurrentRowIndex, we reset it to Static CellType
                    this.Model.InvalidateCell(GridRangeInfo.Cell(this.CurrentRowIndex, 0));
                }

                // reset the current row index
                var oldRowIndex = this.CurrentRowIndex;
                this.CurrentRowIndex = this.Grid.CurrentCell.RowIndex;

                var addNewRowIndex = this.Model.ResolveAddNewPositionInGrid();
                if (this.CurrentRowIndex == addNewRowIndex)
                {
                    //this.IsInAddNewRow = true;
                }
                else
                {
                    //this.IsInAddNewRow = false;
                    this.escapePressCount = 0;
                }

                if (this.Model.TableProperties.ShowRowHeader)
                {
                    this.Model.InvalidateCell(GridRangeInfo.Col(0));
                    this.Model.InvalidateCell(GridRangeInfo.Cell(this.CurrentCell.RowIndex, 0));
                }

                //if (this.IsRecordCell || this.IsRowHeaderCell || this.IsRecordPlusMinusCell ||( (this.IsEmptyCell || this.IsGroupCaptionCell) ))
                if (this.IsRecordCell || this.IsRowHeaderCell || this.IsRecordPlusMinusCell)
                {
                    if (this.Model is GridDataChildTableModel)
                    {
                        if (object.Equals(this.Model, this.Grid.Model))
                        {
                            this.RaiseCurrentRecordSelectionChangedEvent(oldRowIndex);
                        }
                    }
                    else
                    {
                        if ( !this.Model.IsInSummaryPosition(this.Grid.CurrentCell.RowIndex))
                        {
                            this.RaiseCurrentRecordSelectionChangedEvent(oldRowIndex);
                        }
                    }
                }
                if (this.CurrentRowIndex == -1)
                    this.CurrentRowIndex = this.Grid.CurrentCell.RowIndex;
                // var currentRecordIndex = this.Model.ResolveIndexToRecordPosition(this.CurrentCell.RowIndex); Unused local variable
                var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
                var column = colIdx > -1 && colIdx < this.Model.TableProperties.VisibleColumns.Count ? this.Model.TableProperties.VisibleColumns[colIdx] : null;

                if (column != null)
                {
                    // if the activate behavior is not double click, then the current cell would be in editing state, we simply call BeginEdit to ensure 
                    // the edit state is started for that record
                    if ((this.UpdateMode == UpdateMode.RowCachedMode || this.UpdateMode == UpdateMode.LostFocus) && this.CurrentCell.IsEditing)
                    {
                        this.BeginEdit(this.CurrentRecordIndex);
                    }
                }
            }
            else if (this.Model.Table.HasGroups && this.CurrentCell.RowIndex > -1 &&
                     this.CurrentRowIndex != this.Grid.CurrentCell.RowIndex)
            {
                var oldRowIndex = this.CurrentCell.RowIndex;
                if (this.IsRecordCell || this.IsRowHeaderCell || this.IsRecordPlusMinusCell)
                {
                    if (this.Model is GridDataChildTableModel)
                    {
                        if (object.Equals(this.Model, this.Grid.Model))
                        {
                            this.RaiseCurrentRecordSelectionChangedEvent(oldRowIndex);
                        }
                    }
                    else
                    {
                        if (!this.Model.IsInSummaryPosition(this.Grid.CurrentCell.RowIndex))
                        {
                            this.RaiseCurrentRecordSelectionChangedEvent(oldRowIndex);
                        }
                    }
                }
            }
        }

        private void OnCurrentCellMoving(object sender, GridCurrentCellMovingEventArgs args)
        {
            //// header row
            if (/*args.CellRowColumnIndex.RowIndex == 0 ||*/ this.CurrentCellIsNotNested())
            {
                return;
            }

            if (args.CellRowColumnIndex.RowIndex < 0)
            {
                args.Cancel = true;
                return;
            }

            // Raise RecordsSelectionChanging event
            //if ((this.IsRecordCell || this.IsRowHeaderCell ||((this.IsEmptyCell || this.IsGroupCaptionCell)) && !this.Model.IsInUnboundRow(this.Grid.CurrentCell.RowIndex)))
            if ((this.IsRecordCell || this.IsRowHeaderCell) && !this.Model.IsInUnboundRow(this.Grid.CurrentCell.RowIndex))
            {
                if (args.CellRowColumnIndex.RowIndex > 0 || this.CurrentRowIndex == args.CellRowColumnIndex.RowIndex)
                {
                    args.Cancel = this.RaiseCurrentRecordSelectionChangingEvent(args.CellRowColumnIndex.RowIndex);
                }
            }

            if ((this.Grid.CurrentCell.RowIndex == this.Model.ResolveAddNewPositionInGrid()
                && args.CellRowColumnIndex.RowIndex != this.Model.ResolveAddNewPositionInGrid()
                && args.CellRowColumnIndex.RowIndex >= this.Model.ResolveStartIndexBasedOnPosition())
                //CurrentCell move from add new row to normal rows
                ||
                args.CellRowColumnIndex.RowIndex == this.Model.ResolveAddNewPositionInGrid() &&
                this.Grid.CurrentCell.RowIndex != this.Model.ResolveAddNewPositionInGrid())
            //CurrentCell move from normal rows to add new row.
            {

                //if (this.Model.TableProperties.AddNewRowBehaviour == AddNewRowBehaviour.AtleastOneItem &&
                  if( this.newItemCache != null)
                {

                    var colIdx = this.Model.ResolvePositionToVisibleColumnIndex(this.CurrentCell.ColumnIndex);
                    var column = colIdx > -1 && colIdx < this.Model.TableProperties.VisibleColumns.Count ? this.Model.TableProperties.VisibleColumns[colIdx] : null;


                    if (column != null && this.IsRecordCell && this.IsInAddNewRow && this.CurrentCell.Renderer!=null)
                    {
                        var value = this.CurrentCell.Renderer.ControlValue as string;
                        // var cacheValue = this.Model.Table.GetValue(this.newItemCache, column.MappingName); Unused local variable

                        if (!string.IsNullOrEmpty(value))
                        {
                            if (this.CachedStorage.ContainsKey(column.MappingName))
                            {
                                this.CachedStorage[column.MappingName] = value;
                            }
                            else
                            {
                                this.CachedStorage.Add(column.MappingName, value);
                            }
                        }
                    }
                }

                this.EndEdit();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is record cell.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is record cell; otherwise, <c>false</c>.
        /// </value>
        public bool IsRecordCell
        {
            get
            {
                var result = false;
                var colIndex = this.CurrentCell.ColumnIndex;
                if (this.CurrentRowIndex > -1 && colIndex > -1 && this.UpdateMode != UpdateMode.LostFocus)
                {
                    var rowColIdx = new RowColumnIndex(this.CurrentRowIndex, colIndex);
                    GridDataStyleInfo style;
                    //If we get Style using this.Grid.GetRenderStyleInfo(rowColIdx).ModelStyle as GridDataStyleInfo; it breaks the cell selection using keyboard;
                    //if we get style using renderer.CurrentStyle.ModelStyle as GridDataStyleInfo; then it throws exception when sorting the column while the cell is in edit mode
                    //So for headercell we are getting the style using this.Grid.GetRenderStyleInfo(rowColIdx).ModelStyle as GridDataStyleInfo; 
                    // for normal cell getting the style using renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    var renderer = this.CurrentCell.Renderer;
                    if (renderer != null)
                    {
                        style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    }
                    else
                    {
                        style = this.Model[rowColIdx.RowIndex, rowColIdx.ColumnIndex] as GridDataStyleInfo;                        
                    }

                    if (style != null && style.CellIdentity != null && (style.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell || style.CellIdentity.TableCellType == GridDataTableCellType.UnboundColumnCell || style.CellIdentity.TableCellType == GridDataTableCellType.RecordCell))
                    {
                        result = true;
                    }
                    else if (style != null && style.CellIdentity != null && style.CellIdentity.TableCellType == GridDataTableCellType.RowHeaderCell)
                    {
                        result = this.Model.ResolveIndexToRecordPosition(this.CurrentRowIndex) != -1;
                    }

                }
                if (this.UpdateMode == UpdateMode.LostFocus)
                {
                    return true;
                    //var rowColIdx = new RowColumnIndex(this.CurrentRowIndex, colIndex);
                    //var style = this.Grid.GetRenderStyleInfo(rowColIdx).ModelStyle as GridDataStyleInfo;
                    //if (style != null)
                    //{
                    //    if (style.CellIdentity.TableCellType == GridDataTableCellType.RecordCell)
                    //    {
                    //        result = true;
                    //    }
                    //    else if (style.CellIdentity.TableCellType == GridDataTableCellType.RowHeaderCell)
                    //    {
                    //        result = this.Model.ResolveIndexToRecordPosition(this.CurrentRowIndex) != -1;
                    //    }
                    //    else if (style.CellIdentity.TableCellType == GridDataTableCellType.AddNewRecordCell)
                    //    {
                    //        result = true;
                    //    }
                    //}
                }
                return result;
            }
        }


        ///// <summary>
        ///// Gets a value indicating whether this is Empty Cell .
        ///// </summary>  
        //internal bool IsEmptyCell
        //{
        //    get
        //    {
        //        var result = false;
        //        var renderer = this.CurrentCell.Renderer;
        //        if (renderer != null)
        //        {
        //            var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
        //            if (style.CellIdentity.TableCellType == GridDataTableCellType.EmptyCell)
        //            {
        //                result = true;
        //            }
        //        }
        //        return result;
        //    }
        //}

        private bool IsRowHeaderCell
        {
            get
            {
                var result = false;
                var renderer = this.CurrentCell.Renderer;
                if (renderer != null)
                {
                    var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    if (style != null && style.CellIdentity != null && style.CellIdentity.TableCellType == GridDataTableCellType.RowHeaderCell)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        internal bool IsGroupCaptionCell
        {
            get
            {
                var result = false;
                if (this.Model.Grid != null && this.CurrentCell != null && this.CurrentCell.Renderer != null)
                {
                    var renderer = this.CurrentCell.Renderer;
                    var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    if (style != null && style.CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionCell)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        internal bool IsGroupCaptionSummaryCoveredCell
        {
            get
            {
                var result = false;
                if (this.Model.Grid != null && this.CurrentCell != null && this.CurrentCell.Renderer != null)
                {
                    var renderer = this.CurrentCell.Renderer;
                    var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    if (style != null && style.CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryCoveredCell)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        internal bool IsGroupCaptionPlusMinusCell
        {
            get
            {
                var result = false;
                if (this.CurrentCell != null && this.CurrentCell.Renderer != null)
                {
                    var renderer = this.CurrentCell.Renderer;
                    var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    if (style != null && style.CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionPlusMinusCell)
                    {
                        result = true;
                    }
                }
                return result;
            }
        }

        internal bool ISummarysEmptyCell
        {
            get
            {
                var result = false;
                if (this.Model.Grid != null && this.CurrentCell != null && this.CurrentCell.Renderer != null)
                {
                    var renderer = this.CurrentCell.Renderer;
                    var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    if (style != null && style.CellIdentity.TableCellType == GridDataTableCellType.SummaryEmptyCell)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        internal bool IsGroupCaptionSummaryEmptyCell
        {
            get
            {
                var result = false;
                if (this.Model.Grid != null && this.CurrentCell != null && this.CurrentCell.Renderer != null)
                {
                    var renderer = this.CurrentCell.Renderer;
                    var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    if (style != null && style.CellIdentity.TableCellType == GridDataTableCellType.GroupCaptionSummaryEmptyCell)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        internal bool IsRecordPlusMinusCell
        {
            get
            {
                var result = false;
                if (this.CurrentCell != null && this.CurrentCell.Renderer != null)
                {
                    var renderer = this.CurrentCell.Renderer;
                    var style = renderer.CurrentStyle.ModelStyle as GridDataStyleInfo;
                    if (style != null && style.CellIdentity.TableCellType == GridDataTableCellType.RecordPlusMinusCell)
                    {
                        result = true;
                    }
                }
                return result;
            }
        }

        private void RaiseCurrentRecordSelectionChangedEvent(int oldRowIndex)
        {
            var currentRecordIndex = this.CurrentRecordIndex;

            object Record;
            if (!this.Model.Table.HasGroups)
            {
                Record = currentRecordIndex > -1 && currentRecordIndex < this.Model.SourceListCount ? this.Model.View.Records[currentRecordIndex] : null;
            }
            else
            {
                Record = this.Model.View.TopLevelGroup.DisplayElements[currentRecordIndex] as RecordEntry;
            }

            this.OnCurrentRecordSelectionChanged(new GridDataCurrentRecordSelectionChangedEventArgs()
            {
                //OldIndex = oldRowIndex,
                OldIndex = this.Model.ResolveIndexToRecordPosition(oldRowIndex),
                NewIndex = currentRecordIndex,
                Record = Record
            });
        }


        private bool RaiseCurrentRecordSelectionChangingEvent(int rowIdx)
        {
            int recordIndex = this.Model.ResolveIndexToRecordPosition(rowIdx);
            var record = recordIndex > -1 && recordIndex < this.Model.View.Records.Count ? this.Model.View.Records[recordIndex] : null;
            GridDataCurrentRecordSelectionChangingEventArgs args = new GridDataCurrentRecordSelectionChangingEventArgs(recordIndex) { Record = record };
            this.OnRecordsSelectionChanging(args);
            return args.Cancel;
        }

        private void RaiseOnBeginEdit()
        {
            if (this.OnBeginEdit != null)
            {
                this.OnBeginEdit(this, EventArgs.Empty);
            }
        }

        private void RaiseOnEndEdit()
        {
            if (this.OnEndEdit != null)
            {
                this.OnEndEdit(this, EventArgs.Empty);
            }

            this.Model.Table.RaiseGridRecordEndEdit(this.CurrentRecordIndex);
        }

        internal void ResetCache()
        {
           
            if (this.CachedStorage.Count > 0)
            {
                this.CachedStorage.Clear();
            }
            if (this.AddNewRowCachedStorage.Count > 0)
            {
                this.AddNewRowCachedStorage.Clear();
            }
            if(this.newItemCache != null)
            {
                this.newItemCache = null;
            }
        }

        /*private int ResolveColumnIndexToPosition()
        {
            var colIdx = this.CurrentCell.ColumnIndex;

            // get the column
            if (this.Model.Table.HasNestedTables)
            {
                colIdx = this.Model.TableProperties.ShowRecordPlusMinus ? colIdx - 1 : colIdx;
            }

            if (this.Model.Table.HasGroups && this.Model.TableProperties.ShowGroupCaptionPlusMinus)
            {
                var maxLevel = this.Model.Table.GroupModel.GetMaxLevel();
                colIdx = colIdx - maxLevel;
            }

            //// adjust the value of colidx to get the exact column
            colIdx = this.Model.TableProperties.ShowRowHeader ? colIdx - 1 : colIdx;
            return colIdx;
        }*/
    }

    /// <summary>
    /// Provides the data for current record move state used by <see cref="GridDataTableModel"/> class.
    /// </summary>
    public class GridDataCurrentRecordMoveState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridDataCurrentRecordMoveState"/> class.
        /// </summary>
        public GridDataCurrentRecordMoveState()
        {
            this.Index = -1;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public GridDataVisibleColumn Column
        {
            get;
            internal set;
        }
    }

    /// <summary>
    /// Specifies the ENUM with the type of changes done to the underlying source when the record is edited.
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// Applies the changes only when the focus moves to the next row.
        /// </summary>
        RowCachedMode,
        /// <summary>
        /// Applies the changes when the current cell is out of focus or moved to another cell.
        /// </summary>
        LostFocus,
        /// <summary>
        /// Applies the changes when a single value of a property is chnaged.
        /// </summary>
        PropertyChanged,
        /// <summary>
        /// Empty value that is used internally.
        /// </summary>
        Empty
    }

    public enum AddNewRowBehaviour
    {
        Default,
        AtleastOneItem,
    }
}
