//-------------------------------------------------------------------------------------------------
// <copyright file="CurrentRecord.cs" company="syncfusion">
// Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//
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
using System.Text;

using Syncfusion.Diagnostics;

using Syncfusion.Collections;
using Syncfusion.Collections.BinaryTree;
using Syncfusion.Grouping.Internals;
using System.Collections.Generic;

namespace Syncfusion.Grouping
{
    /// <summary>
    /// Manages the current record or element in a table and provides routines both for
    /// navigation and editing.
    /// </summary>
    public class CurrentRecordManager : IDisposable
    {
        IRecordUpdateHelper updateHelper;

        /// <summary>
        /// Gets or sets the record update helper.
        /// </summary>
        public IRecordUpdateHelper UpdateHelper
        {
            get { return this.updateHelper; }
            set { this.updateHelper = value; }
        }

        Table table;
        Element _currentRecord = null; // can be a record or table
        internal bool isSetModifiedValue = false;
        internal bool isResetModifiedValue = false;

        Element currentRecord
        {
            get
            {
                return _currentRecord;
            }

            set
            {
                if (_currentRecord != value)
                {
                    if (_currentRecord != null)
                    {
                        _currentRecord.Disposed -= recordDisposedEventHandler;
                    }

                    _currentRecord = value;
                    if (_currentRecord != null)
                    {
                        _currentRecord.Disposed += recordDisposedEventHandler;
                    }
                }
            }
        }

        internal void InternalSetCurrentRecord(Element el)
        {
            if (el == null || el.ParentSection == null || el.ParentSection.IsDisposed)
            {
                el = null;
            }

            currentRecord = el;
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes of the object.
        /// </summary>
        public void Dispose()
        {
            table = null;
            if (_currentRecord != null)
            {
                _currentRecord.Disposed -= recordDisposedEventHandler;
            }

            _currentRecord = null;
            currentField = null;
            oldCurrentField = null;
            properties = null;
            sourceListItem = null;
        }

        #endregion

        bool isEditing = false;
        bool inBeginEdit = false;
        bool inEndEdit = false;
        bool inCancelEdit = false;
        bool isModified = false;
        bool inNavigate = false;
        object sourceListItem = null;
        bool endEditSuccess = false;
        bool cancelEditSuccess = false;
        bool inLeaveRecord = false;
        bool leaveRecordSuccess = false;
        bool inEnterRecord = false;
        bool enterRecordSuccess = false;
        int isLocked = 0;
        CurrentRecordPropertyCollection properties;
        FieldDescriptor currentField;
        FieldDescriptor oldCurrentField;
        bool currentFieldModified;
        EventHandler recordDisposedEventHandler;

        /// <summary>
        /// Resets the current record state and raises a <see cref="Table.CurrentRecordManagerReset"/> event
        /// on the parent table.
        /// </summary>
        public void Reset()
        {
            ////TraceUtil.TraceCalledFromIf(true, 10, this.Table);
            currentRecord = null;
            isEditing = false;
            inBeginEdit = false;
            inEndEdit = false;
            inCancelEdit = false;
            isModified = false;
            inNavigate = false;
            sourceListItem = null;
            endEditSuccess = false;
            cancelEditSuccess = false;
            inLeaveRecord = false;
            leaveRecordSuccess = false;
            inEnterRecord = false;
            enterRecordSuccess = false;
            isLocked = 0;
            properties = null;
            currentField = null;
            oldCurrentField = null;
            ParentTable.RaiseCurrentRecordManagerReset();
        }

        /// <summary>
        /// Resets the current record state and raises a <see cref="Table.CurrentRecordManagerReset"/> event
        /// on the parent table. Afterwards it internally sets the 
        /// <see cref="CurrentElement"/> without raising any events. This is an internal helper method that 
        /// is called from Table when AddNewRecord has been transformed into a regular Record element when
        /// EndEdit was called.
        /// </summary>
        /// <param name="record">The Record</param>
        public void ResetCurrentRecord(Record record)
        {
            FieldDescriptor fd = currentField;
            Reset();
            currentRecord = record;
            currentField = fd;
            oldCurrentField = null;
        }

        /// <summary>
        /// Gets debug information about the object.
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Info
        {
            get
            {
                return ToString();
            }
        }

        bool IsCurrentFieldModified
        {
            get
            {
                return currentFieldModified;
            }

            set
            {
                currentFieldModified = value;
            }
        }

        /// <summary>
        /// Gets / sets the current field (in a grid this is the current cell in the active record).
        /// </summary>
        public FieldDescriptor CurrentField
        {
            get
            {
                return currentField;
            }

            set
            {
                if (currentField != value)
                {
                    oldCurrentField = currentField;
                    currentField = value;
                    currentFieldModified = true;
                }
            }
        }

        /// <summary>Restores the current field to the previous state.</summary>
        /// <exclude/>
        public void RestoreOldCurrentField()
        {
            currentField = oldCurrentField;
            oldCurrentField = null;
        }

        /// <summary>Returns string representation of the current record manager.</summary>
        /// <returns>String representation of the current object.</returns>
        /// <override/>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("CurrentRecord at ");
            if (currentRecord is Record)
            {
                sb.Append(table.UnsortedRecords.IndexOf((Record)currentRecord));
            }
            else if (CurrentElement != null)
            {
                sb.Append(CurrentElement.ToString());
            }
            else
            {
                sb.Append("(none)");
            }

            sb.Append(": ");
            if (isModified)
            {
                sb.AppendFormat(" Modified");
            }

            if (isEditing)
            {
                sb.AppendFormat(" Editing");
            }

            if (inEndEdit)
            {
                sb.AppendFormat(" EndEdit");
            }

            if (inCancelEdit)
            {
                sb.AppendFormat(" CancelEdit");
            }

            if (inNavigate)
            {
                sb.AppendFormat(" Navigate");
            }

            if (inLeaveRecord)
            {
                sb.AppendFormat(" LeaveRecord");
            }

            if (inEnterRecord)
            {
                sb.AppendFormat(" EnterRecord");
            }

            if (IsLocked)
            {
                sb.AppendFormat(" Locked");
            }

            foreach (CurrentRecordProperty prop in Properties)
            {
                if (prop.IsModified)
                {
                    sb.Append(", " + prop.ToString());
                }
            }

            return sb.ToString();
        }

        internal CurrentRecordManager(Table table)
        {
            this.table = table;
            recordDisposedEventHandler = new EventHandler(RecordDisposed);
        }

        void RecordDisposed(object sender, EventArgs e)
        {
            currentRecord = null;
            isEditing = false;
            inBeginEdit = false;
            inEndEdit = false;
            inCancelEdit = false;
            isModified = false;
            inNavigate = false;
            sourceListItem = null;
            endEditSuccess = false;
            cancelEditSuccess = false;
            inLeaveRecord = false;
            leaveRecordSuccess = false;
            inEnterRecord = false;
            enterRecordSuccess = false;
            isLocked = 0;
            properties = null;
            ////currentField = null;
            ParentTable.RaiseCurrentRecordManagerReset();
        }

        internal void NotifyPropertyChanged(CurrentRecordProperty property)
        {
            FieldDescriptor field = property.FieldDescriptor;
            if (field != null && field.IsForeignKeyField())
            {
                foreach (CurrentRecordProperty p in this.Properties)
                {
                    if (p.FieldDescriptor.parentFieldDescriptor != null
                        && field.Name == p.FieldDescriptor.parentFieldDescriptor.Name)
                    {
                        p.Exception = null;
                    }
                }
            }

            if (property.IsModified)
            {
                SetModified(true, true);
            }
            else if (!IsAnyPropertyModified)
            {
                SetModified(false, true);
            }
        }

        internal void NotifyException(CurrentRecordProperty prop)
        {
            if (this.inEndEdit)
            {
                throw prop.Exception;
            }
        }

        /// <summary>
        /// Resets the <see cref="CurrentRecordProperty.ModifiedValue"/> for the specified <see cref="CurrentRecordProperty"/>. Works also with nested properties and related fields.
        /// </summary>
        /// <param name="fieldDescriptor">The FieldDescriptor</param>
        public void ResetModifiedValue(FieldDescriptor fieldDescriptor)
        {
            RelationDescriptor rd = fieldDescriptor.GetRelation();
            if (fieldDescriptor.IsRelatedField() && rd.RelationKeys.Count > 0)
            {
                FieldDescriptor foreignKeyField = rd.RelationKeys[rd.RelationKeys.Count - 1].ParentKeyField;
                fieldDescriptor = foreignKeyField;
            }

            this.ParentTable.CurrentRecordManager.Properties[fieldDescriptor.Name].ResetModifiedValue();
        }

        /// <summary>
        /// Gets the collection of <see cref="CurrentRecordProperty"/> elements that
        /// contain modified values of the current record.
        /// </summary>
        public CurrentRecordPropertyCollection Properties
        {
            get
            {
                if (properties == null)
                {
                    properties = new CurrentRecordPropertyCollection(this);
                }

                return properties;
            }
        }

        /// <summary>Internal only.</summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        public void ResetCachedState()
        {
            sourceListItem = null;
            isModified = false;
            if (!this.InCancelEdit)
            {
                properties = null;
            }
        }

        /// <summary>
        /// Gets the underlying data of the current record (calls <see cref="Record.GetData"/> on the current <see cref="Record"/>).
        /// </summary>
        public object Data
        {
            get
            {
                if (currentRecord == null)
                {
                    return null;
                }

                if (sourceListItem == null)
                {
                    sourceListItem = currentRecord.GetData();
                }
#if SyncfusionFramework4_0
                if (this.table.Engine.IsDynamicData)
                {
                    IDictionary<string, object> item = sourceListItem as IDictionary<string, object>;
                    if (item != null && item.Count == 0)
                    {
                        foreach (DynamicPropertyDescriptor info in this.table.TableDescriptor.ItemProperties)
                        {
                            item.Add(info.Name, string.Empty);
                        }
                    }
                    return item;
                }
#endif
                return sourceListItem;
            }
        }

        /// <summary>
        /// Casts <see cref="Data"/> as <see cref="IEditableObject"/>. NULL if Data is not of this type.
        /// </summary>
        public IEditableObject EditableObject
        {
            get
            {
                return Data as IEditableObject;
            }
        }

        /// <summary>
        /// The <see cref="Table"/> this object belongs to.
        /// </summary>
        public Table ParentTable  //
        {
            get
            {
                return table;
            }

            set
            {
                table = value;
            }
        }

        /// <summary>
        /// Navigates to the <see cref="AddNewRecord"/> and calls <see cref="BeginEdit"/>.
        /// </summary>
        public void AddNew()
        {
#if DEBUG
            if (Switches.CurrentRecord.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ParentTable, this.CurrentElement);
            }
#else
            ;
#endif
            if (NavigateTo(table.AddNewRecord) != null)
            {
                BeginEdit();
            }
        }

        /// <summary>
        /// Switches the current record into edit mode. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        public void BeginEdit()
        {
#if DEBUG
            if (Switches.CurrentRecord.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ParentTable, this.CurrentElement);
            }
#else
            ;
#endif
            if (IsLocked)
            {
                return;
            }

            if (isEditing)
            {
                return;
            }

            bool success = false;
            inBeginEdit = true;
            try
            {
                inNotifyBeginEdit = true;
                if (table.NotifyBeginEditCalled()
                    && currentRecord.OnBeginEditCalled())
                {
                    inNotifyBeginEdit = false;
                    ResetCachedState();
                    success = BeginEditHelper();
                    isEditing = true;
                }
            }
            finally
            {
                inNotifyBeginEdit = false;
                inBeginEdit = false;
                currentRecord.OnBeginEditComplete(success);
                table.NotifyBeginEditComplete(success);
            }
        }

        internal bool inNotifyBeginEdit = false;

        /// <summary>
        /// Ends edit mode for the current record. If changes are detected, they will be saved to the
        /// underlying data source. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <returns>Returns the record that was changed. Normally this is a reference to the current record. But if the current
        /// record is an AddNewRecord, a new record will be added to the table and instead of returning the AddNewRecord, a reference
        /// to the newly created record element is returned.</returns>
        public Record EndEdit()
        {
            if (ParentTable.CurrentRecordManager.IsEditing)
            {
                _EndEdit();

                if (!ParentTable.CurrentRecordManager.IsEditing)
                {
                    return ParentTable.LastChangedRecord;
                }
            }

            return ParentTable.CurrentRecord;
        }

        void _EndEdit()
        {
#if DEBUG
            if (Switches.CurrentRecord.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ParentTable, this.CurrentElement);
            }
#else
            ;
#endif
            if (IsLocked)
            {
                return;
            }

            if (!isEditing)
            {
                Properties.Reset();
                return;
            }

            bool success = false;
            inEndEdit = true;
            try
            {
                if (table.NotifyEndEditCalled()
                    && currentRecord.OnEndEditCalled())
                {
                    if (IsModified)
                    {
                        SaveRecordHelper();
                    }
                    else
                    {
                        CancelRecordHelper();
                    }
                    // couldn't save nor cancel
                    if (IsModified)
                    {
                        return;
                    }

                    isEditing = false;
                    success = true;
                    ResetCachedState();
                    Properties.Reset();
                }
            }
            finally
            {
                inEndEdit = false;
                if (currentRecord != null)
                {
                    currentRecord.OnEndEditComplete(success);
                }

                table.NotifyEndEditComplete(success);
                endEditSuccess = success;
            }
        }

        internal void OnCurrentRecordListChanged(TableListChangedEventArgs te)
        {
            NotifyCurrentRecordListChanged(te);
        }

        /// <summary>
        /// Invoked when the underlying datasource, to which the current record belongs, is changed.
        /// </summary>
        /// <param name="te">The <see cref="TableListChangedEventArgs"/> object.</param>
        public void NotifyCurrentRecordListChanged(TableListChangedEventArgs te)
        {
            if (!this.InEndEdit && te.ShouldResetCurrentRecord)
            {
                if (te.ListChangedType == ListChangedType.ItemDeleted)
                {
                    ResetCurrentRecord(null);
                }
                else
                {
                    ResetCurrentRecord(this.CurrentRecord);
                }
            }
            else if (this.inEndEdit || this.inSaveHelper)
            {
                isModified = false;
                Properties.Reset();
            }
        }

        bool inSaveHelper = false;

        /// <summary>
        /// Deletes the current record.
        /// </summary>
        public void DeleteCurrentRecord()
        {
            if (this.currentRecord is Record)
            {
                bool deleted = false;
                if (this.updateHelper != null && this.updateHelper.CanSaveRecord(this.Data))
                {
                    deleted = this.UpdateHelper.DeleteRecord(this.Data, this.ParentTable);
                }
                // Also updating the view.
                if (deleted)
                {
                    ((Record)this.currentRecord).Delete();
                }
            }
        }

        bool SaveRecordHelper()
        {
            if (properties == null || currentRecord == null)
            {
                return false;
            }

            if (this.IsAnyPropertyInvalid)
            {
                return false;
            }
#if DEBUG

            if (Switches.CurrentRecord.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ParentTable, this.CurrentElement);
            }
#else

            ;
#endif
            try
            {
                IList sourceList = GetCurrentSourceList();

                ParentTable.wasItemChanged = false;

                CurrentRecordProperty errorProperty = null;

                // Save modified properties first. This ensures that we can loop
                // through all modified properties and save them one by one. Otherwise
                // the Properties in the collection might be modified by the ListChanged
                // event handler and thus not generate ListChanged notification with 
                // e.PropertyDescriptor being set for each modified property.
                // Creating modifiedProperties collection first also fixes problems
                // seen in incidents 48085 and 47989 with items implementing INotifyPropertyChanged 
                // in a BindingList<> or List<> of objects.
                ArrayList modifiedProperties = new ArrayList();
                foreach (CurrentRecordProperty prop in Properties)
                {
                    if (prop.IsModified)
                    {
                        modifiedProperties.Add(prop);
                    }
                }

                // If the underlying record implements INotifyPropertyChanged a batch of ItemChanged 
                // notifications will now be raised with the e.PropertyDescriptor being set for 
                // every modified value while looping through modifiedProperties.
                foreach (CurrentRecordProperty prop in modifiedProperties)
                {
                    errorProperty = prop;
                    prop.SaveChanges();
                    prop.Exception = null;
                }

                errorProperty = null;

                inSaveHelper = true;
                try
                {
                    if (this.UpdateHelper != null && this.UpdateHelper.CanSaveRecord(sourceListItem))
                    {
                        if (this.CurrentElement is AddNewRecord)
                        {
                            this.UpdateHelper.AddRecord(sourceListItem, this.table);
                        }
                        else
                        {
                            this.UpdateHelper.SaveRecord(sourceListItem, this.table);
                        }
                    }
                    else if (EditableObject != null)
                    {
                        EditableObject.EndEdit();

                        // In the case that the IEditableObject implementation did not properly add the
                        // edited item to the underlying source list as normally expected then we
                        // just go ahead and manually add it. In this case also the SimulateListChangedWithSourceList
                        // call below will be executed and the Table object will then be able to process
                        // the ListChanged notification and add a record element to UnsortedRecords collection.
                        if (this.CurrentElement is AddNewRecord && sourceList != null && !sourceList.Contains(sourceListItem))
                        {
                            sourceList.Add(sourceListItem);
                        }
                    }
                    else if (sourceListItem is System.Data.DataRowView)
                    {
                        if (this.CurrentElement is AddNewRecord)
                        {
                            ((System.Data.DataRowView)sourceListItem).DataView.Table.Rows.Add(((System.Data.DataRowView)sourceListItem).Row);
                        }
                        else
                        {
                            (((System.Data.DataRowView)sourceListItem).Row).EndEdit();
                        }
                    }
                    else if (sourceListItem is System.Data.DataRow)
                    {
                        if (this.CurrentElement is AddNewRecord)
                        {
                            ((System.Data.DataRow)sourceListItem).Table.Rows.Add((System.Data.DataRow)sourceListItem);
                        }
                        else
                        {
                            ((System.Data.DataRow)sourceListItem).EndEdit();
                        }
                    }
                    else if (!(sourceList is IBindingList))
                    {
                        if (this.CurrentElement is AddNewRecord)
                        {
                            sourceList.Add(sourceListItem);
                        }
                    }
                }
                finally
                {
                    inSaveHelper = false;
                }

                ParentTable.ClearCollectionCaches();

                isModified = false;
                if (CurrentElement != null && !ParentTable.wasItemChanged)
                {
                    IList sl = sourceList;
                    if (sl == null)
                    {
                        // New in version 4.2: The ParentChildTable of a record has the list
                        // for the child table in a UniformChildList relation.
                        sl = CurrentElement.ParentChildTable.SourceList;
                        if (!(CurrentElement is AddNewRecord))
                        {
                            CurrentElement.InvalidateCounterBottomUp();
                        }
                        else
                        {
                            CurrentElement.ParentChildTable.InvalidateCounterTopDown(true);
                        }
                    }
                    else
                    {
                        if (!(CurrentElement is AddNewRecord))
                        {
                            ParentTable.SimulateListChanged((Record)CurrentRecord);
                        }
                        else if (this.ParentTable.IsNewUniformChildListRelation())
                        {
                            sl = CurrentElement.ParentChildTable.SourceList;
                            ParentTable.SimulateListChangedWithSourceList(sl, CurrentElement.ParentChildTable, new ListChangedEventArgs(ListChangedType.ItemAdded, sl.Count - 1, -1));
                        }
                        else
                        {
                            ParentTable.SimulateListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, sl.Count - 1, -1));
                        }
                        ////ParentTable.SimulateListChanged(new ListChangedEventArgs(ListChangedType.Reset, sourceList.Count-1, -1));
                    }
                }
                //// if no exception occurred ...

                Properties.Reset();

                ParentTable.ClearCollectionCaches();

                return true;
            }
            catch (Exception ex)
            {
                UpdatePropertyErrors(ex);

                table.NotifyException("SaveRecord", ex);

                return false;
            }
        }

        private IList GetCurrentSourceList()
        {
            IList sourceList = ParentTable.SourceList;

            // New in version 4.2: The ParentChildTable of a record has the list
            // for the child table in a UniformChildList relation.
            if (sourceList == null || this.ParentTable.IsNewUniformChildListRelation())
            {
                sourceList = currentRecord.ParentChildTable.SourceList;
            }

            return sourceList;
        }

        void UpdatePropertyErrors(Exception ex)
        {
            bool anyErrorsReported = false;
            IDataErrorInfo errorInfo = this.Data as IDataErrorInfo;
            if (errorInfo != null)
            {
                CurrentRecordProperty errorProperty = null;
                try
                {
                    foreach (CurrentRecordProperty prop in Properties)
                    {
                        errorProperty = prop;
                        string name = errorProperty.FieldDescriptor.MappingName;
                        if (name.IndexOf(".") == -1)
                        {
                            string err = errorInfo[name];
                            if (err != null && err.Length > 0)
                            {
                                Trace.WriteLine(errorProperty.FieldDescriptor.MappingName + ": " + err.ToString());
                                errorProperty.Exception = ex;
                                anyErrorsReported = true;
                            }
                        }
                        ////                        if (prop.IsModified)
                        ////                            prop.SaveChanges();
                    }
                }
                catch (Exception ex2)
                {
                    Trace.WriteLine(ex2.ToString());
                }
            }

            if (!anyErrorsReported)
            {
                string s = ex.Message;
                //int col = s.IndexOf("Column '");
                //if (col != -1)
                foreach (CurrentRecordProperty prop in Properties)
                {
                    //  s = s.Substring(col + "Column '".Length);
                    //col = s.IndexOf("' ");
                    // if (col != -1)
                    if (s.Contains('\'' + prop.FieldDescriptor.MappingName + '\''))
                    {
                        //   s = s.Substring(0, col);
                        CurrentRecordProperty errorProperty = prop;
                        if (errorProperty != null)
                        {
                            errorProperty.Exception = ex;
                            FieldDescriptor field = errorProperty.FieldDescriptor;
                            if (field != null && field.IsForeignKeyField())
                            {
                                foreach (CurrentRecordProperty p in this.Properties)
                                {
                                    if (p.FieldDescriptor.parentFieldDescriptor != null
                                        && field.Name == p.FieldDescriptor.parentFieldDescriptor.Name)
                                    {
                                        p.Exception = ex;
                                    }
                                }
                            }
                        }
                        return;
                    }
                }
            }
        }

        bool BeginEditHelper()
        {
#if DEBUG
            if (Switches.CurrentRecord.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ParentTable, this.CurrentElement);
            }
#else
            ;
#endif
            if (currentRecord == null)
            {
                return false;
            }

            try
            {
                if (EditableObject != null)
                {
                    EditableObject.BeginEdit();
                }
                else if (sourceListItem is System.Data.DataRow)
                {
                    ((System.Data.DataRow)sourceListItem).BeginEdit();
                }

                return true;
            }
            catch (Exception ex)
            {
                table.NotifyException("BeginEdit", ex);
                return false;
            }
        }

        bool CancelRecordHelper()
        {
            ////TraceUtil.TraceCurrentMethodInfo(this.Table, this.CurrentElement);
            if (properties == null || currentRecord == null)
            {
                return false;
            }

            try
            {
                if (EditableObject != null)
                {
                    try
                    {
                        foreach (CurrentRecordProperty p in this.properties)
                        {
                            p.ResetModifiedValue();
                        }
                    }
                    catch
                    {
                        // At this time simply ignore any exception, record will be canceled below anyway.
                    }

                    EditableObject.CancelEdit();
                }
                else if (sourceListItem is System.Data.DataRow)
                {
                    ((System.Data.DataRow)sourceListItem).CancelEdit();
                }
                else if (CurrentElement is AddNewRecord)
                {
                    if (ParentTable.SourceList is IBindingList)
                    {
                        ParentTable.SourceList.RemoveAt(ParentTable.SourceList.Count - 1);
                    }
                    ////                        ParentTable.SimulateListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, -1, -1));
                    ////                    else
                }
                ////else if (!ParentTable.wasItemChanged && (CurrentElement is AddNewRecord))
                ////Fix for defect #12617.

                //// if no exception occurred ...
                Properties.Reset();

                return true;
            }
            catch (Exception ex)
            {
                table.NotifyException("CancelEdit", ex);
                return false;
            }
        }

        /// <summary>
        /// Cancels editing for the current record; <see cref="CancelEdit"/> is called, events are raised, and current record
        /// is marked as deactivated without raising events.
        /// </summary>
        public void Reinititalize()
        {
            CancelEdit();
            this.currentRecord = null;
        }

        /// <summary>
        /// Cancels editing for the current record. Changes in the current record are
        /// discarded. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        public void CancelEdit()
        {
#if DEBUG
            if (Switches.CurrentRecord.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ParentTable, this.CurrentElement);
            }
#else
            ;
#endif
            if (IsLocked)
            {
                return;
            }

            if (!isEditing)
            {
                Properties.Reset();
                return;
            }

            bool success = false;
            try
            {
                inCancelEdit = true;

                if (table.NotifyCancelEditCalled()
                    && currentRecord.OnCancelEditCalled())
                {
                    success = CancelRecordHelper();
                    isEditing = false;
                    ResetCachedState();
                    Properties.Reset();
                }
            }
            finally
            {
                inCancelEdit = false;
                currentRecord.OnCancelEditComplete(success);
                table.NotifyCancelEditComplete(success);
                cancelEditSuccess = success;
            }
        }

        /// <summary>
        /// Determines if any field in the current record was modified.
        /// </summary>
        /// <internalonly/>
        [Syncfusion.Documentation.DocumentationExclude()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsAnyPropertyModified
        {
            get
            {
                if (properties == null || currentRecord == null)
                {
                    return false;
                }

                foreach (CurrentRecordProperty prop in Properties)
                {
                    if (prop.IsModified)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Checks if validation failed because of invalid data in the current record.
        /// </summary>
        public bool IsAnyPropertyInvalid
        {
            get
            {
                if (properties == null || currentRecord == null)
                {
                    return false;
                }

                foreach (CurrentRecordProperty prop in Properties)
                {
                    if (prop.Exception != null)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Determines if record was marked as modified.
        /// </summary>
        public bool IsModified
        {
            get { return isModified; }
        }

        internal void SetModified(bool modified, bool callHelper)
        {
            if (modified != isModified)
            {
                ////table.NotifyCurrentRecordModified();
                isModified = modified;
                if (IsEditing && callHelper)
                {
                    if (isModified)
                    {
                        BeginEditHelper();
                    }
                    else
                    {
                        CancelRecordHelper();
                    }
                }
            }
        }

        /// <summary>
        /// called form Table.bindingList_ListChanged
        /// </summary>
        internal void ResetEditingInternal()
        {
            if (!this.InEndEdit)
            {
                throw new InvalidOperationException();
            }

            this.isEditing = false;
            this.isModified = false;
        }

        /// <summary>
        /// Gets if <see cref="BeginEdit"/> was called and is in editing mode. Will be set to False when <see cref="CancelEdit"/> or
        /// <see cref="EndEdit"/> is called.
        /// </summary>
        public bool IsEditing
        {
            get { return isEditing; }
        }

        /// <summary>
        /// True while processing <see cref="BeginEdit"/> call. Will be set to False after <see cref="BeginEdit"/> returns.
        /// </summary>
        public bool InBeginEdit
        {
            get { return inBeginEdit; }
        }

        /// <summary>
        /// True while processing <see cref="CancelEdit"/> call. Will be set to False after <see cref="CancelEdit"/> returns.
        /// </summary>
        public bool InCancelEdit
        {
            get { return inCancelEdit; }
        }

        /// <summary>
        /// True while processing <see cref="EndEdit"/> call. Will be set to False after <see cref="EndEdit"/> returns.
        /// </summary>
        public bool InEndEdit
        {
            get { return inEndEdit; }
        }

        /// <summary>
        /// True while processing <see cref="Navigate"/> call. Will be set to False after <see cref="Navigate"/> returns.
        /// </summary>
        public bool InNavigate
        {
            get { return inNavigate; }
        }

        /// <summary>
        /// Determines if table has an active current element that is a <see cref="Record"/> (and not a <see cref="NestedTable"/>).
        /// </summary>
        public bool HasCurrentRecord
        {
            get
            {
                return currentRecord is Record;
            }
        }

        /// <summary>
        /// Determines if table has an active current element (either a <see cref="Record"/> or <see cref="NestedTable"/>).
        /// </summary>
        public bool HasCurrentElement
        {
            get
            {
                return currentRecord != null;
            }
        }

        /// <summary>
        /// Gets / sets the current element. Setting the current element will trigger a <see cref="Navigate"/> call.
        /// </summary>
        public Element CurrentElement
        {
            get
            {
                return currentRecord;
            }

            set
            {
                if (IsLocked)
                {
                    return;
                }

                if (currentRecord != value)
                {
                    Element r = NavigateTo(value);
                }
            }
        }

        /// <summary>
        /// Gets / sets the current record. When current element is not a <see cref="Record"/>, NULL is returned, (e.g. if
        /// element is a <see cref="NestedTable"/>).
        /// Setting the current record will trigger a <see cref="Navigate"/> call.
        /// </summary>
        public Record CurrentRecord
        {
            get
            {
                return CurrentElement as Record;
            }

            set
            {
                CurrentElement = value;
            }
        }

        /// <overload>
        /// Navigates the record up or down, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </overload>
        /// <summary>
        /// Navigates the record up or down, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="step">The number of records to advance. Positive step will move the record down, negative steps will move the record up.</param>
        /// <returns>The current record after navigation.</returns>
        public Record Navigate(int step)
        {
            return Navigate(CurrentRecord, step);
        }

        /// <summary>
        /// Navigates the record up or down from the given initial record, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="startRecord">The initial record to move off from.</param>
        /// <param name="step">The number of records to advance. Positive step will move the record down, negative steps will move the record up.</param>
        /// <returns>The current record after navigation.</returns>
        public Record Navigate(Record startRecord, int step)
        {
            if (IsLocked)
            {
                return null;
            }

            int vpos = ParentTable.FilteredRecords.IndexOf(startRecord);
            vpos = Math.Max(0, Math.Min(vpos + step, ParentTable.FilteredRecords.Count - 1));
            return NavigateTo(ParentTable.FilteredRecords[vpos]);
        }

        /// <overload>
        /// Navigates to a specific record, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </overload>
        /// <summary>
        /// Navigates to a specific record, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="record">The record to navigate to.</param>
        /// <returns>The current record after navigation.</returns>
        public Record NavigateTo(Record record)
        {
            return ((NavigateTo((Element)record, false, true)) as Record);
        }

        /// <summary>
        /// Navigates to a specific record, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="record">The record to navigate to.</param>
        /// <param name="cancelEditIfNotValid">True if any changes should be discarded if they do not meet validation constraints; False if record should not be deactivated if not valid.</param>
        /// <returns>The current record after navigation.</returns>
        public Record NavigateTo(Record record, bool cancelEditIfNotValid)
        {
            return ((NavigateTo((Element)record, cancelEditIfNotValid, true))as Record);
        }

        /// <summary>
        /// Navigates to a specific record. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="record">The record to navigate to.</param>
        /// <param name="cancelEditIfNotValid">True if any changes should be discarded if they do not meet validation constraints; False if record should not be deactivated if not valid.</param>
        /// <param name="scrollInView">True if record should be scrolled into view.</param>
        /// <returns>The current record after navigation.</returns>
        public Record NavigateTo(Record record, bool cancelEditIfNotValid, bool scrollInView)
        {
            return ((NavigateTo((Element)record, cancelEditIfNotValid, scrollInView)) as Record);
        }

        /// <summary>
        /// Navigates to a specific element, the current element should not be deactivated if not valid, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="record">The element to navigate to.</param>
        /// <returns>The current element after navigation.</returns>
        public Element NavigateTo(Element record)
        {
            return NavigateTo(record, false, true);
        }

        /// <summary>
        /// Navigates to a specific element, element should be scrolled into view. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="record">The element to navigate to.</param>
        /// <param name="cancelEditIfNotValid">True if any changes should be discarded if they do not meet validation constraints; False if record should not be deactivated if not valid.</param>
        /// <returns>The current element after navigation.</returns>
        public Element NavigateTo(Element record, bool cancelEditIfNotValid)
        {
            return NavigateTo((Element)record, cancelEditIfNotValid, true);
        }

        bool scrollInView = true;

        /// <summary>
        /// Gets if element should be scrolled into view during a <see cref="NavigateTo"/> call.
        /// </summary>
        /// <returns>True if the element should be scrolled into view.</returns>
        public bool ShouldScrollInView()
        {
            return scrollInView;
        }

        // bool forceShowCurrentRecord = false;

        /// <exclude/>
        /// <summary>
        /// Gets a value indicating whether the engine should ensure that a record is visible and all its parent
        /// elements are expanded when setting the current record. The default setting is true.
        /// </summary>
        public bool ForceShowCurrentRecord
        {
            get { return this.ParentTable.Engine.ForceShowCurrentRecord; }
            ////set { forceShowCurrentRecord = value; }
        }

        /// <summary>
        /// Navigates to a specific element. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="record">The element to navigate to.</param>
        /// <param name="cancelEditIfNotValid">True if any changes should be discarded if they do not meet validation constraints; False if record should not be deactivated if not valid.</param>
        /// <param name="scrollInView">True if record should be scrolled into view.</param>
        /// <returns>The current element after navigation.</returns>
        public Element NavigateTo(Element record, bool cancelEditIfNotValid, bool scrollInView)
        {
            TraceUtil.TraceCalledFromIf(Switches.CurrentRecord.TraceVerbose, 10, this.ParentTable, record, cancelEditIfNotValid);
            if (IsLocked)
            {
                return null;
            }

            bool savedscrollInView = this.scrollInView;
            try
            {
                this.scrollInView = scrollInView;
                ////bool cc = false;

                CurrentRecordManager pm;
                //// bubble up to parent records.
                if (record != null)
                {
                    ChildTable parentChildTable = record.ParentChildTable;
                    if (parentChildTable != null)
                    {
                        NestedTable parentNestedTable = parentChildTable.ParentNestedTable;
                        if (parentNestedTable != null)
                        {
                            ////parentChildTable.ParentTable.FilteredChildTable = parentChildTable;
                            if (ForceShowCurrentRecord)
                            {
                                if (!(record.Kind == DisplayElementKind.Caption && record.ParentGroup == parentChildTable))
                                {
                                    parentNestedTable.IsExpanded = true;
                                }
                            }

                            parentNestedTable.ParentRecord.IsExpanded = true;
                            pm = parentNestedTable.ParentTable.CurrentRecordManager;
                            ////pm.forceShowCurrentRecord = forceShowCurrentRecord;
                            pm.NavigateTo(parentNestedTable, false, scrollInView);
                            if (parentNestedTable != pm.CurrentElement)
                            {
                                //// NavigateTo failed. Don't proceed any further.
                                return CurrentElement;
                            }
                        }
                        else if (parentChildTable.IsTopLevelGroup)   
                        {
                            //// FK_SUPPORT: Modal GridTableControl display
                            //// TopLevelGroup should not set FilteredChildTable
                            parentChildTable.ParentTable.FilteredChildTable = null;
                        }

                        //// Note: FilteredChildTable will not be reset here so that parent table remains in needed state.
                    }

                    if (ForceShowCurrentRecord)
                    {
                        Group g = record.ParentGroup;
                        if (record.Kind == DisplayElementKind.Caption && g != null)
                        {
                            g = g.ParentGroup;
                        }

                        while (g != null)
                        {
                            g.IsExpanded = true;
                            g = g.ParentGroup;
                        }
                    }
                }

                if (currentRecord == record || record == null)
                {
                    if (this.IsCurrentFieldModified)
                    {
                        table.NotifyCurrentFieldChanged();
                    }

                    IsCurrentFieldModified = false;
                    return record;
                }

                IsCurrentFieldModified = false;

                if (inNavigate)
                {
                    return null;
                }

                Element savedRecord = currentRecord;
                bool success = false;
                inNavigate = true;
                try
                {
                    if (table.NotifyNavigateCalled(record))
                    {
                        if (CurrentElement != null)
                        {
                            LeaveRecord(cancelEditIfNotValid);
                        }

                        if (CurrentElement != null)
                        {
                            return CurrentElement; // TODO: not sure if it would be better to return null here so
                        }                       
                        else
                        { 
                            // user knows it failed. But for now user can also check targetRecord.IsCurrent() to
                        // check if NavigateTo worked ok.
                            if (record == null)
                            {
                                success = true;
                            }
                            else
                            {
                                ////if (false && cc)
                                ////    Table.ClearCollectionCaches();
                                EnterRecord(record);
                                success = CurrentElement == record;
                            }
                        }

                        return CurrentElement;
                    }

                    return null;
                }
                finally
                {
                    inNavigate = false;
                    table.NotifyNavigateComplete(success, savedRecord);
                }
            }
            finally
            {
                this.scrollInView = savedscrollInView;
            }
        }

        /// <summary>
        /// Gets if <see cref="LeaveRecord"/> was called. Will be set to False when <see cref="LeaveRecord"/> returns.
        /// </summary>
        public bool InLeaveRecord
        {
            get { return inLeaveRecord; }
        }

        /// <summary>
        /// Deactivates the current record; if current record is in editing mode <see cref="EndEdit"/> is called and / or
        /// <see cref="CancelEdit"/> if record could not be validated. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="cancelEditIfNotValid">True if any changes should be discarded if they do not meet validation constraints; False if record should not be deactivated if not valid.</param>
        public void LeaveRecord(bool cancelEditIfNotValid)
        {
            if (IsLocked)
            {
                return;
            }

            if (currentRecord == null || table == null)
            {
                return;
            }

            TraceUtil.TraceCalledFromIf(Switches.CurrentRecord.TraceVerbose, 10, this.ParentTable);
            ////TraceUtil.TraceCurrentMethodInfo();
            ////TraceUtil.TraceCalledFromIf(true, 10, this.Table);

            Element savedRecord = currentRecord;
            bool success = false;
            inLeaveRecord = true;
            try
            {
                if (table.NotifyLeaveRecordCalled()
                    && currentRecord.OnLeaveRecordCalled())
                {
                    if (IsEditing)
                    {
                        EndEdit();
                    }

                    if (IsEditing)
                    {
                        if (cancelEditIfNotValid)
                        {
                            CancelEdit();
                            if (!cancelEditSuccess)
                            {
                                return;
                            }
                        }
                        else
                        {
                            return;  // could not Deactivate current record.
                        }
                    }

                    success = true;
                    currentRecord = null;
                    ResetCachedState();
                }
            }
            finally
            {
                inLeaveRecord = false;
                savedRecord.OnLeaveRecordComplete(success);
                table.NotifyLeaveRecordComplete(success, savedRecord);
                leaveRecordSuccess = success;
            }
        }

        /// <summary>
        /// Gets if <see cref="EnterRecord"/> was called. Will be set to False when <see cref="EnterRecord"/> returns.
        /// </summary>
        public bool InEnterRecord
        {
            get { return inEnterRecord; }
        }

        /// <summary>
        /// Activates a new record; if there is a current record the method will throw an exception. You first
        /// need to call <see cref="LeaveRecord"/> in such case. Raises <see cref="Table.CurrentRecordContextChange"/> events on the <see cref="Table"/>.
        /// </summary>
        /// <param name="record">The record to navigate to.</param>
        public void EnterRecord(Element record)
        {
            if (IsLocked)
            {
                return;
            }

            if (currentRecord != null)
            {
                throw new InvalidOperationException("You must first leave a record before you can enter another one.");
            }

            if (record.GetVisibleCount() == 0)
            {
                return;
            }

            Section section = record as Section;
            if (section == null)
            {
                section = record.ParentSection;
            }

            if (section == null || section.IsDisposed)
            {
                return;
            }
#if DEBUG
            if (Switches.CurrentRecord.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(this.ParentTable);
            }
#else

            ;
#endif
            bool success = false;
            inEnterRecord = true;
            try
            {
                if (table.NotifyEnterRecordCalled(record)
                    && record.OnEnterRecordCalled())
                {
                    // Loop through other tables and deactivate current record
                    if (DeactivateRecord(this.table, this.table))
                    {
                        if (ForceShowCurrentRecord)
                        {
                            table.ShowRecord(record, false);
                        }

                        ResetCachedState();
                        currentRecord = record;
                        success = true;
                    }
                }
            }
            finally
            {
                inEnterRecord = false;
                record.OnEnterRecordComplete(success);
                table.NotifyEnterRecordComplete(success);
                enterRecordSuccess = success;
            }
        }

        bool DeactivateRecord(Table table, Table notThisTable)
        {
            if (table != notThisTable)
            {
                table.CurrentRecordManager.LeaveRecord(false);
                if (table.CurrentRecordManager.CurrentElement != null)
                {
                    return false; // was not deactivated, e.g. some validation rule failed.
                }
            }

            foreach (Table relatedTable in table.RelatedTables)
            {
                if (!DeactivateRecord(relatedTable, notThisTable))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Locks the current record. While locked, calls to Navigate, BeginEdit, EndEdit, or CancelEdit will be
        /// ignored.
        /// </summary>
        public void Lock()
        {
            isLocked++;
        }

        /// <summary>
        /// Unlocks the current record after a <see cref="Lock"/> call.
        /// </summary>
        public void Unlock()
        {
            isLocked--;
        }

        /// <summary>
        /// Determines if current record is locked after a <see cref="Lock"/> call.
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return isLocked > 0;
            }
        }
    }

    /// <exclude/>
    public interface IRecordUpdateHelper
    {
        bool CanSaveRecord(object item);
        void SaveRecord(object item, Table table);
        void AddRecord(object item, Table table);
        bool DeleteRecord(object item, Table table);
    }
}
