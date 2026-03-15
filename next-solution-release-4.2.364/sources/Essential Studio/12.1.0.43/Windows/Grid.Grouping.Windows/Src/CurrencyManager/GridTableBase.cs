//-------------------------------------------------------------------------------------------------
// <copyright file="GridTableBase.cs" company="syncfusion">
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
using System.ComponentModel.Design;
using System.Data;
using System.Windows.Forms;

using Syncfusion.Collections;
using Syncfusion.ComponentModel;
using Syncfusion.Diagnostics;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using Table = Syncfusion.Grouping.Table;

namespace Syncfusion.Windows.Forms.Grid.Grouping
{
    /// <summary>
    /// Manages all the records from the underlying source list. The source list can be any IList collection.
    /// If it implements IBindingList the GridTable will listen to the ListChangedEvent and update its internal
    /// data whenever changes are made to the source list.
    /// </summary>
    /// <remarks>
    /// See the <see cref="Syncfusion.Grouping.Table"/> class for a more detailed overview about this class.
    /// <para/>
    /// GridTableBase add support for the Windows Forms CurrencyManager. It detects when the
    /// CurrencyManager.Position is changed. <para/>
    /// GridTableBase also makes the IBindingList.ListChanged event
    /// thread-safe. When rows are added on a different thread, the event is marshaled onto the current
    /// UI thread before it is processed.
    /// </remarks>
    [TypeConverter(typeof(DescriptorBaseConverter))]
    public class GridTableBase : Table
    {
        CurrencyManager _currencyManager;
        bool inCurrentRecordContextChange = false;
        bool inCurrencyManagerPositionChanged = false;

        /// <summary>
        /// Initializes a new table object that belongs to a <see cref="TableDescriptor"/> and optionally belongs to a parent table.
        /// </summary>
        /// <param name="tableDescriptor">The table descriptor with schema information about the table.</param>
        /// <param name="relationParentTable">The parent table of this table; NULL if this table is not a child table of a relation.</param>
        public GridTableBase(TableDescriptor tableDescriptor, Table relationParentTable)
            : base(tableDescriptor, relationParentTable)
        {
        }

        /// <override/>
        protected override void Dispose(bool disposing)
        {
            UnwireCurrencyManager();
            this._currencyManager = null;
            this.bindingContext = null;

            base.Dispose(disposing);
        }

        BindingContext bindingContext;

        /// <summary>
        /// Gets / sets the BindingContext for the control.
        /// </summary>
        /// <remarks>
        /// The BindingContext object of a Control is used to return a single BindingManagerBase object for all
        /// data-bound controls contained by the Control. The BindingManagerBase object keeps all controls that
        /// are bound to the same datasource synchronized. For example, setting the Position property of the
        /// BindingManagerBase specifies the item in the underlying list that all data-bound controls point to.<para/>
        /// For more information about creating a new BindingContext and assigning it to the BindingContext property,
        /// see the BindingContext.
        /// </remarks>
        [System.Xml.Serialization.XmlIgnore]
        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                {
                    BindingContext = new BindingContext();
                }

                return bindingContext;
            }

            set
            {
                if (bindingContext != value)
                {
                    bindingContext = value;
                    ResetCurrencyManager();
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="BindingContext"/> is changed.
        /// </summary>
        [Description("Occurs when the BindingContext is changed.")]
        public event EventHandler BindingContextChanged;

        /// <summary>
        /// Raises the <see cref="BindingContextChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs" /> that contains the event data.</param>
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            if (BindingContextChanged != null)
            {
                BindingContextChanged(this, e);
            }
        }

        /// <summary>
        /// Gets the CurrencyManager for the assigned SourceList.
        /// </summary>
        /// <returns>Currency manager.</returns>
        public CurrencyManager GetCurrencyManager()
        {
            if (!IsPassThroughGrouping
             && ((GridEngineBase)Engine).BindToCurrencyManager
             && _currencyManager == null
             && SourceList != null)
            {
                if (this.RelationParentTable != null)
                {
                    _currencyManager = (CurrencyManager)BindingContext[SourceList];
                }
                else
                {
                    _currencyManager = ((GridEngineBase)Engine).CurrencyManager;
                }

                WireCurrencyManager();
            }

            return _currencyManager;
        }

        /// <summary>
        /// Resets the CurrencyManager to NULL and unwires any events.
        /// </summary>
        public void ResetCurrencyManager()
        {
            if (_currencyManager != null)
            {
                UnwireCurrencyManager();
                _currencyManager = null;
            }
        }

        //// CurrencyManager ICurrencyManagerSource.GetCurrencyManager()
        //// {
        //// return CurrencyManager;
        //// }

        /// <override/>
        protected override void OnSourceListChanged(TableEventArgs e)
        {
            ResetCurrencyManager();
            if (this.HasSourceList)
            {
                GetCurrencyManager();
            }

            base.OnSourceListChanged(e);
        }

        void WireCurrencyManager()
        {
            if (_currencyManager != null)
            {
                _currencyManager.CurrentChanged += new EventHandler(_currencyManager_CurrentChanged);
                _currencyManager.ItemChanged += new ItemChangedEventHandler(_currencyManager_ItemChanged);
                //// MetaDataChanged only available in .NET 1.1
                //// _currencyManager.MetaDataChanged += new EventHandler(_currencyManager_MetaDataChanged);
                _currencyManager.PositionChanged += new EventHandler(_currencyManager_PositionChanged);
            }
        }

        void UnwireCurrencyManager()
        {
            if (_currencyManager != null)
            {
                _currencyManager.CurrentChanged -= new EventHandler(_currencyManager_CurrentChanged);
                _currencyManager.ItemChanged -= new ItemChangedEventHandler(_currencyManager_ItemChanged);
                //// _currencyManager.MetaDataChanged -= new EventHandler(_currencyManager_MetaDataChanged);
                _currencyManager.PositionChanged -= new EventHandler(_currencyManager_PositionChanged);
            }
        }

        /// <summary>
        /// Determines if the current thread is the same UI thread as the parent control or if
        /// the current method call should be marshaled.
        /// </summary>
        /// <returns>A control that can be used to marshal the current method by calling its Invoke method.</returns>
        /// <remarks>
        /// A GridEngine overrides this method and returns a reference to the GridGroupingControl since all
        /// events need to be marshaled to the same thread that the GridGroupingControl is running on.
        /// </remarks>
        protected virtual Control GetInvokeRequiredControl()
        {
            return null;
        }

        bool ignoreCurrenyManagerEvents = false;

        /// <override/>
        protected override void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                //// Marshal call onto the grid's thread and block this thread until call returns.
                invokeControl.Invoke(new ListChangedEventHandler(this.bindingList_ListChanged), new object[] { sender, e });
                return;
            }

            base.bindingList_ListChanged(sender, e);
        }

        /// <override/>
        protected override void dt_RowDeleting(object sender, System.Data.DataRowChangeEventArgs e)
        {
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                //// Marshal call onto the grid's thread and block this thread until call returns.
                invokeControl.Invoke(new DataRowChangeEventHandler(dt_RowDeleting), new object[] { sender, e });
                return;
            }

            base.dt_RowDeleting(sender, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Syncfusion.Grouping.Table.SourceListListChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.ListChangedEventArgs"/> that contains the event data.</param>
        /// <override/>
        protected override void OnSourceListListChanged(TableListChangedEventArgs e)
        {
            //// Do not call UnsortedRecords.Count when TableDirty = true. Otherwise it will trigger
            //// CategorizeElements and with CategorizeElements that one record that is modified here
            //// has already been changed.

            int savedCategorizeElementsVersion = this.CategorizeElementsVersion;
            base.OnSourceListListChanged(e);
            if (savedCategorizeElementsVersion != this.CategorizeElementsVersion)
            {
                //// CategorizeElements was called - immeditally return.
                return;
            }

            if (ignoreCurrenyManagerEvents)
            {
                return;
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemDeleted:
                    {
                        if (!TableDirty && e.NewIndex >= 0 && e.NewIndex < this.UnsortedRecords.Count)
                        {
                            Record record = this.UnsortedRecords[e.NewIndex];
                            if (record.IsCurrent)
                            {
                                if (!CurrentRecordManager.InEndEdit)
                                {
                                    CurrentRecordManager.Reset();
                                }

                                //// When last record is deleted and the current record was not moved off that record
                                //// before it was deleted, set cm.Position will cause an exception. Calling
                                //// Refresh resolves this problem.
                                CurrencyManager cm = this.GetCurrencyManager();
                                if (cm != null && cm.Position >= cm.List.Count - 1)
                                {
                                    ignoreCurrenyManagerEvents = true;
                                    try
                                    {
                                        cm.Refresh();
                                        ////cm.Position = 0;
                                    }
                                    catch (Exception ex)
                                    {
                                        TraceUtil.TraceExceptionCatched(ex);
                                        Console.WriteLine("CurrencyManager {0}, {1}", cm.Position, cm.List.Count);
                                    }
                                    finally
                                    {
                                        ignoreCurrenyManagerEvents = false;
                                    }
                                }
                            }
                        }

                        break;
                    }
            }
        }

        private void _currencyManager_CurrentChanged(object sender, EventArgs e)
        {
            if (this.ignoreCurrenyManagerEvents)
            {
                return;
            }

            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new EventHandler(this._currencyManager_CurrentChanged), new object[] { sender, e });
                return;
            }

            CurrencyManager cm = this.GetCurrencyManager();
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e);
            }
#else
               ;
#endif
        }

        private void _currencyManager_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            if (this.ignoreCurrenyManagerEvents)
            {
                return;
            }

            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new ItemChangedEventHandler(this._currencyManager_ItemChanged), new object[] { sender, e });
                return;
            }

            CurrencyManager cm = this.GetCurrencyManager();
#if DEBUG
            if (Switches.GroupingGrid.TraceVerbose)
            {
                TraceUtil.TraceCurrentMethodInfo(e.Index);
            }
#else
               ;
#endif

            // Since there is no IBindingList - let's inform the underlying table about the change
            if (e.Index != -1 && !(cm.List is IBindingList))
            {
                this.SimulateListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, e.Index, e.Index));
            }
        }

        private void _currencyManager_MetaDataChanged(object sender, EventArgs e)
        {
            if (this.ignoreCurrenyManagerEvents)
            {
                return;
            }

            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new EventHandler(this._currencyManager_MetaDataChanged), new object[] { sender, e });
                return;
            }

            CurrencyManager cm = this.GetCurrencyManager();
            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose);

            // Since there is no IBindingList - let's inform the underlying table about the change
            if (!(cm.List is IBindingList))
            {
                this.SimulateListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1, -1));
            }
        }

        /// <override/>
        protected override void OnCurrentRecordContextChange(CurrentRecordContextChangeEventArgs e)
        {
            inCurrentRecordContextChange = true;

            try
            {
                if (e.Action == CurrentRecordAction.LeaveRecordComplete && e.Success)
                {
                    ////CurrencyManager.Position = -1; // not sure if allowed
                }
                else if (e.Action == CurrentRecordAction.EnterRecordComplete && e.Success)
                {
                    if (!inCurrencyManagerPositionChanged)
                    {
                        int pos = UnsortedRecords.IndexOf(GetRecordOrParentRecord(e.Record));
                        if (pos != -1)
                        {
                            CurrencyManager cm = GetCurrencyManager();
                            if (cm != null && pos < cm.List.Count)
                            {
                                cm.Position = pos;
                            }
                        }
                    }
                }
                else if (e.Action == CurrentRecordAction.NavigateComplete && e.Success)
                {
                }
            }
            finally
            {
                inCurrentRecordContextChange = false;
            }

            base.OnCurrentRecordContextChange(e);
        }

        /// <override/>
        protected override void OnCategorizedRecords(TableEventArgs e)
        {
            base.OnCategorizedRecords(e);

            if (pending_currencyManager_PositionChanged && this.EngineTable.InInitialize)
            {
                _currencyManager_PositionChanged(this._currencyManager, EventArgs.Empty);
            }
        }

        bool pending_currencyManager_PositionChanged = false;

        private void _currencyManager_PositionChanged(object sender, EventArgs e)
        {
            //// SH 8/2/2004 - Added check for this.CurrentRecord == null. That means
            //// when you have no current record, PositionChanged events will be ignored.
            //// This kind of lets you detach the table from being updated whenever
            //// Position changes. A good case for this is if you delete the parent record
            //// of a child table. The child table will get PositionChanged notifications
            //// but it will ignore them.
            if (this.ignoreCurrenyManagerEvents || this.CurrentRecord == null)
            {
                return;
            }

            //// || this.UnsortedRecords.Count == 0)
            if (this.TableDirty)
            {
                pending_currencyManager_PositionChanged = true;
                return;
            }

            pending_currencyManager_PositionChanged = false;
            Control invokeControl = this.GetInvokeRequiredControl();
            if (invokeControl != null)
            {
                invokeControl.Invoke(new EventHandler(this._currencyManager_PositionChanged), new object[] { sender, e });
                return;
            }

            CurrencyManager cm = this.GetCurrencyManager();
            TraceUtil.TraceCurrentMethodInfoIf(Switches.GroupingGrid.TraceVerbose | Switches.CurCellNestedGrid.TraceVerbose);

            if (inCurrentRecordContextChange || CurrentRecordManager.InBeginEdit || CurrentRecordManager.InEndEdit)
            {
                return;
            }

            inCurrencyManagerPositionChanged = true;
            try
            {
                GridGroupingControl parent = ((GridEngine)Engine).ParentControl;
                //// if (parent != null)
                //// parent.TableControl.SynchronizeGridWithEngine();

                Record record;
                int pos = cm.Position;
                if (pos <= -1)
                {
                    record = null;
                }
                else if (pos == UnsortedRecords.Count && this.SourceListAllowNew)
                {
                    record = AddNewRecord;
                }
                else if (pos < UnsortedRecords.Count && cm.Count >= UnsortedRecords.Count)
                {
                    record = UnsortedRecords[pos];
                }
                else
                {
                    pending_currencyManager_PositionChanged = true;
                    return;
                }

                ////throw new InvalidOperationException("position");

                if (record == null || !record.IsInitialized || !record.MeetsFilterCriteria())
                {
                    return;
                }

                CurrentRecordManager.NavigateTo(record);
            }
            finally
            {
                inCurrencyManagerPositionChanged = false;
            }
        }
    }
}
