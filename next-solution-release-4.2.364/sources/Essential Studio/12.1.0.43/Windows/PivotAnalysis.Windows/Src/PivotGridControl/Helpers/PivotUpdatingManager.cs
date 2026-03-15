#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Syncfusion.PivotAnalysis.Base;
using System.Windows;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Data;
using Syncfusion.Windows.Forms.Grid;

namespace Syncfusion.Windows.Forms.PivotAnalysis
{
    /// <summary>
    /// This class allows a PrivoGridControl to automatically respond to changes in the underlying data
    /// provided that data supports appropriate events. You enable this support by setting <see cref="PivotGridControl.EnableUpdating"/> 
    /// to true. 
    /// </summary>
    /// <remarks>
    /// In order for the PivotGridControl to automatically respond to the changes in the underlying data, the underlying data must be either:
    ///     A) a DataTable or DataView
    /// or
    ///     B) an IList&LT;T&GT; where T implements both INotifyPropertyChanging and INotifyPropertyChanged. Additionally, the IList must also
    ///     implement INotifyCollectionChanged or IBindingList."
    /// </remarks>
    public class PivotUpdatingManager : IDisposable
    {
        GridControlBase gridBase = null;
        PivotEngine engine = null;
        bool isDisposed = false;
        PivotGridControlBase grid = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="grid">The PivotGidControl that should be responding to updates.</param>
        public PivotUpdatingManager(PivotGridControlBase grid)
        {
            InitializeUpdatingManager(grid);
            this.grid = grid;
        }

        /// <summary>
        /// Initializes the updating manager with its associated settings
        /// </summary>
        /// <param name="grid"></param>
        internal void InitializeUpdatingManager(PivotGridControlBase grid)
        {
            this.gridBase = grid;
            this.engine = grid.PivotEngine;
            WireEvents();
            isDisposed = false;
        }

        #region public methods and properties

        /// <summary>
        /// Gets or sets a millisecond value for time between UI refreshes. Zero indicates immediate refreshes of the UI 
        /// without delays. Throttling the refresh rate can minimize CPU usage. The default value is zero, but depending
        /// upon your updating rate, values of 300 to 500 msecs may give lower CPU usage.
        /// </summary>
        public int ThrottleUpdateRate
        {
            get { return throttleUpdateRate; }
            set
            {
                throttleUpdateRate = value;
                if (throttleUpdateRate == 0 && updateTimer != null)
                {
                    updateTimer.Stop();
                    updateTimer.Tick -= new EventHandler(updateTimer_Tick);
                    updateTimer_Tick(null, EventArgs.Empty);//process any pending updates...
                    updateTimer = null;
                }
            }
        }

        /// <summary>
        /// Unsubscribe to certain events that were subscribed to when this object was created.
        /// </summary>
        public void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                if (updateTimer != null)
                {
                    updateTimer.Stop();
                    updateTimer.Tick -= new EventHandler(updateTimer_Tick);
                    updateTimer = null;
                }
                
                UnwireEvents();
            }
        }

        #endregion

        #region private members

        private int throttleUpdateRate;
        private DispatcherTimer updateTimer;
        private List<RowColumnIndex> pendingUpdates;
        private ChangingValueInfo changingValue;
        bool notFound;
        bool inWorker;
        delegate void UpdateCellDelegate(int row, int col);
        delegate void InsertRowDelegate(int row, object value);

        /// <summary>
        /// To Wire the required events in the PivotGrid
        /// </summary>
        private void WireEvents()
        {
            if (this.engine != null && this.engine.DataSourceList != null && this.engine.DataSourceList is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)this.engine.DataSourceList).CollectionChanged -= new NotifyCollectionChangedEventHandler(UpdateRefreshHelper_CollectionChanged);
                ((INotifyCollectionChanged)this.engine.DataSourceList).CollectionChanged += new NotifyCollectionChangedEventHandler(UpdateRefreshHelper_CollectionChanged);
            }
            else if (this.engine != null && this.engine.DataSourceList != null && this.engine.DataSourceList is IBindingList)
            {
                ((IBindingList)this.engine.DataSourceList).ListChanged -= new ListChangedEventHandler(UpdateRefreshHelper_ListChanged);
                ((IBindingList)this.engine.DataSourceList).ListChanged += new ListChangedEventHandler(UpdateRefreshHelper_ListChanged);
                if (this.engine.DataSourceList is DataView)
                {
                    ((DataView)this.engine.DataSourceList).Table.RowDeleting -= new DataRowChangeEventHandler(Table_RowDeleting);
                    ((DataView)this.engine.DataSourceList).Table.RowDeleting += new DataRowChangeEventHandler(Table_RowDeleting);
                }
            }

            if (gridBase != null && this.engine.DataSourceList != null)
            {
                IEnumerable list = this.engine.DataSourceList;
                if (list != null)
                {
                    object o = null;
                    foreach (object item in list)
                    {
                        o = item;
                        break;
                    }
                    if (o != null)
                    {
                        if (o is INotifyPropertyChanged)
                        {
                            foreach (INotifyPropertyChanged item in list)
                            {
                                item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
                            }
                        }

                        if (o is INotifyPropertyChanging)
                        {
                            foreach (INotifyPropertyChanging item in list)
                            {
                                item.PropertyChanging += new PropertyChangingEventHandler(item_PropertyChanging);
                            }
                        }
                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging += new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// To UnWire the events from the PivotGrid
        /// </summary>
        private void UnwireEvents()
        {
            if (gridBase != null && this.engine.DataSourceList != null && this.engine.DataSourceList is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)this.engine.DataSourceList).CollectionChanged -= new NotifyCollectionChangedEventHandler(UpdateRefreshHelper_CollectionChanged);
            }
            else if (this.engine != null && this.engine.DataSourceList != null && this.engine.DataSourceList is IBindingList)
            {
                ((IBindingList)this.engine.DataSourceList).ListChanged -= new ListChangedEventHandler(UpdateRefreshHelper_ListChanged);
            }

            if (gridBase != null && this.engine.DataSourceList != null)
            {
                IEnumerable list = this.engine.DataSourceList;
                if (list != null)
                {
                    object o = null;
                    foreach (object item in list)
                    {
                        o = item;
                        break;
                    }
                    if (o != null)
                    {
                        if (o is INotifyPropertyChanged)
                        {
                            foreach (INotifyPropertyChanged item in list)
                            {
                                item.PropertyChanged -= new PropertyChangedEventHandler(Item_PropertyChanged);
                            }
                        }
                        if (o is INotifyPropertyChanging)
                        {
                            foreach (INotifyPropertyChanging item in list)
                            {
                                item.PropertyChanging -= new PropertyChangingEventHandler(item_PropertyChanging);
                            }
                        }
                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging -= new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when a value in column is changed
        /// </summary>
        private void Table_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            changingValue = new ChangingValueInfo()
            {
                ColumnIndex = FindColumn(e.Row, e.Column.ColumnName),
                RowIndex = FindRow(e.Row, e.Column.ColumnName),
                OriginalValue = e.Row[e.Column.ColumnName]
            };
        }

        /// <summary>
        /// Occurs when a row is deleted from the table
        /// </summary>
        private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (e.Action == DataRowAction.Delete)
            {

                int col = FindColumn(e.Row, null);
                int row = FindRow(e.Row, null);
                foreach (var cal in engine.PivotCalculations)
                {
                    object o1 = e.Row[cal.FieldName];
                    ChangeValue(o1, null, e.Row, cal.FieldName, row, col, false, true);
                    if (engine.ShowCalculationsAsColumns)
                        col++;
                    else
                        row++;
                }
            }
        }

        /// <summary>
        /// Updates the cell with the new value passed
        /// </summary>
        private void ChangeRowColumnPivotValue(object oldValue, object newValue, object item, string propertyName)
        {
            var v = engine.PivotColumns.Where(c => c.FieldMappingName == propertyName);

            bool found = !(v == null || v.Count() == 0);

            if (!found)
            {
                v = engine.PivotRows.Where(c => c.FieldMappingName == propertyName);
                found = !(v == null || v.Count() == 0);
            }
            if (found)
            {
                PivotItem pi = v.First();

                if (pi != null)
                {
                    if (pi.Format != null && pi.Format.Length > 0)
                    {
                        oldValue = string.Format(pi.Format, oldValue);
                        newValue = string.Format(pi.Format, newValue);
                    }
                    else
                    {
                        oldValue = oldValue.ToString();
                        newValue = newValue.ToString();
                    }
                }

                //remove the old one...

                int col1 = FindColumn(item, null, oldValue, v.First());
                int row1 = FindRow(item, null, oldValue, v.First());



                foreach (var cal in engine.PivotCalculations)
                {
                    object o1 = engine.ItemProperties[cal.FieldName].GetValue(item);
                    ChangeValue(o1, null, item, cal.FieldName, row1, col1, false, true);

                    if (engine.ShowCalculationsAsColumns)
                        col1++;
                    else
                        row1++;

                }

                //add the new one
                col1 = FindColumn(item, null, newValue, v.First());
                row1 = FindRow(item, null, newValue, v.First());


                foreach (var cal in engine.PivotCalculations)
                {
                    object o1 = engine.ItemProperties[cal.FieldName].GetValue(item);
                    ChangeValue(null, o1, item, cal.FieldName, row1, col1, true, false);
                    if (engine.ShowCalculationsAsColumns)
                        col1++;
                    else
                        row1++;

                }
            }
        }

        /// <summary>
        /// To check the index of the cell to be updated
        /// </summary>
        private void CheckUpdateCell(int row, int col)
        {
            if (throttleUpdateRate > 0)
            {
                if (pendingUpdates == null)
                {
                    pendingUpdates = new List<RowColumnIndex>();
                }
                pendingUpdates.Add(new RowColumnIndex(row, col));
                if (!inWorker)
                {
                    inWorker = true;
                    ThreadPool.QueueUserWorkItem(
                    arg =>
                    {
                        while (throttleUpdateRate > 0)
                        {
                            Thread.Sleep(throttleUpdateRate);
                            gridBase.Invoke(new EventHandler(updateTimer_Tick), new object[] { null, EventArgs.Empty });
                        }
                        inWorker = false;
                    });
                }
            }
            else
            {
                UpdateCell(row, col);
            }
        }

        /// <summary>
        /// To update the cell in the given index
        /// </summary>
        private void UpdateCell(int row, int col)
        {
            if (gridBase != null)
            {
                if (gridBase.InvokeRequired)
                {
                    gridBase.BeginInvoke(new UpdateCellDelegate(UpdateCell), new object[] { row, col });
                    return;
                }
            }
            gridBase.RefreshRange(GridRangeInfo.Cell(row, col));
        }

        /// <summary>
        /// To insert a new row in the provided index
        /// </summary>
        private void InsertRow(int row, object value)
        {
            if (gridBase != null)
            {
                if (gridBase.InvokeRequired)
                {
                    gridBase.BeginInvoke(new InsertRowDelegate(InsertRow), new object[] { row, value });
                    return;
                }
            }
            gridBase.Model.RowCount++;

            List<GridRangeInfo> newRanges = new List<GridRangeInfo>();
            foreach (GridRangeInfo r in gridBase.Model.CoveredRanges)
            {
                if (r.Left >= engine.PivotRows.Count)
                {
                    newRanges.Add(r);
                    continue; //skip ranges from column pivots
                }
                if (r.Top >= row)
                {
                    newRanges.Add(GridRangeInfo.Cells(r.Top + 1, r.Left, r.Bottom + 1, r.Right));
                }
                else if (r.Bottom + 1 >= row)
                {
                    newRanges.Add(GridRangeInfo.Cells(r.Top, r.Left, r.Bottom + 1, r.Right));
                }
                else
                {
                    newRanges.Add(r);
                }
            }
            gridBase.Model.CoveredRanges.Clear();
            foreach (GridRangeInfo r in newRanges)
            {
                gridBase.Model.CoveredRanges.Add(r);
            }
            gridBase.Refresh();

            for (int col = 0; col <= gridBase.Model.ColCount; ++col)
            {
                gridBase.Model[row - 1, col] = gridBase.Model[row, col];
                if (col < engine.PivotRows.Count)
                {
                    gridBase.Model[row, col].CellValue = engine.ItemProperties[engine.PivotRows[col].FieldMappingName].GetValue(value);
                }
                else
                {
                    gridBase.Model[row, col].CellValue = null;
                }
            }

        }

        /// <summary>
        /// Occurs when the timer tick changed
        /// </summary>
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < pendingUpdates.Count; i++)
            {
                RowColumnIndex rci = pendingUpdates[i];
                pendingUpdates.Clear();
                gridBase.RefreshRange(GridRangeInfo.Table());
            }
        }

        /// <summary>
        /// To get the index of next summarycell for the provided column index
        /// </summary>
        private int GetNextSummaryOverColIndex(int row1, int col)
        {
            int sumCol = -1;
            PivotCellInfo pci = engine[row1, col];
            while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && col > 0)
            {
                col--;
                pci = engine[row1, col];
            }
            sumCol = col + ((engine[row1, col].CellRange != null) ? engine[row1, col].CellRange.Right - engine[row1, col].CellRange.Left + 1 : 1);
            return sumCol;
        }

        /// <summary>
        /// To get the index of next summarycell for the provided row index
        /// </summary>
        private int GetNextSummaryDownRowIndex(int row, int col1)
        {
            int sumRow = -1;
            PivotCellInfo pci = engine[row, col1];

            while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && row > 0)
            {
                row--;
                pci = engine[row, col1];
            }
            sumRow = row + ((engine[row, col1].CellRange != null) ? engine[row, col1].CellRange.Bottom - engine[row, col1].CellRange.Top + 1 : 1);
            return sumRow;
        }

        /// <summary>
        /// To get the index for a column corresponding to the provided property name
        /// </summary>
        //if propertyName is null, then not searching for calculation....
        private int FindColumn(object item, string propertyName)
        {
            return FindColumn(item, propertyName, null, null);
        }

        /// <summary>
        /// To get the index for a column corresponding to the provided property name and DataRow
        /// </summary>
        private int FindColumn(DataRow item, string propertyName)
        {
            return FindColumn(item, propertyName, null, null);
        }

        /// <summary>
        /// To get the index for a column corresponding to the provided property name and DataRow
        /// </summary>
        private int FindColumn(DataRow item, string propertyName, object oldValue, PivotItem changedColumn)
        {
            int col = engine.PivotRows.Count + ((!engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? 1 : 0);
            int row = 0;
            foreach (PivotItem pi in engine.PivotColumns)
            {
                object val = oldValue != null && pi == changedColumn ? oldValue : item[pi.FieldMappingName].ToString();

                while (col < engine.ColumnCount)
                {
                    if (engine[row, col] == null || val.Equals(engine[row, col].Value))
                    {
                        break;
                    }
                    col += (engine[row, col].CellRange != null) ? engine[row, col].CellRange.Right - engine[row, col].CellRange.Left + 1 : 1;
                }
                row++;
            }
            if (propertyName != null && engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1)
            {
                foreach (PivotComputationInfo ci in engine.PivotCalculations)
                {

                    if (col >= engine.ColumnCount || ci.FieldName == propertyName)
                    {
                        break;
                    }
                    col++;
                }
            }

            return col;
        }

        /// <summary>
        /// To get the index for a column corresponding to the provided property name
        /// </summary>
        private int FindColumn(object item, string propertyName, object oldValue, PivotItem changedColumn)
        {
            int col = engine.PivotRows.Count + ((!engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? 1 : 0);
            int row = 0;

            foreach (PivotItem pi in engine.PivotColumns)
            {
                object val = oldValue != null && pi == changedColumn ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item).ToString();


                while (col < engine.ColumnCount)
                {
                    if (engine[row, col] == null || val.Equals(engine[row, col].Value))
                    {
                        break;
                    }
                    col += (engine[row, col].CellRange != null) ? engine[row, col].CellRange.Right - engine[row, col].CellRange.Left + 1 : 1;
                }
                row++;
            }
            if (propertyName != null && engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1)
            {
                foreach (PivotComputationInfo ci in engine.PivotCalculations)
                {

                    if (col >= engine.ColumnCount || ci.FieldName == propertyName)
                    {
                        break;
                    }
                    col++;
                }
            }
            return col;
        }

        /// <summary>
        /// To get the index for a row corresponding to the provided property name
        /// </summary>
        //if propertyName is null, then not searching for calculation....
        private int FindRow(object item, string propertyName)
        {
            return FindRow(item, propertyName, null, null);
        }

        /// <summary>
        /// To get the index for a row corresponding to the provided property name and DataRow
        /// </summary>
        private int FindRow(DataRow item, string propertyName)
        {
            return FindRow(item, propertyName, null, null);
        }

        /// <summary>
        /// To get the index for a row corresponding to the provided property name 
        /// </summary>
        private int FindRow(object item, string propertyName, object oldValue, PivotItem changedRow)
        {
            int row = engine.PivotColumns.Count + ((engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? 1 : 0);
            int col = 0;

            foreach (PivotItem pi in engine.PivotRows)
            {
                object val = oldValue != null && pi == changedRow ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item).ToString();
                while (row < engine.RowCount)
                {
                    if (engine[row, col] == null)
                    {
                        break;
                    }
                    if (val.Equals(engine[row, col].Value))
                    {
                        break;
                    }
                    if (engine[row, col].Value.Equals('x'))
                    {
                        return -row + 1;
                    }
                    row += (engine[row, col].CellRange != null) ? engine[row, col].CellRange.Bottom - engine[row, col].CellRange.Top + 2 : 1;
                }
                col++;
            }
            if (propertyName != null && !engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1)
            {
                foreach (PivotComputationInfo ci in engine.PivotCalculations)
                {

                    if (row >= engine.RowCount || ci.FieldName == propertyName)
                    {
                        break;
                    }
                    row++;
                }
            }

            return row;
        }

        /// <summary>
        /// To get the index for a row corresponding to the provided property name and DataRow
        /// </summary>
        private int FindRow(DataRow item, string propertyName, object oldValue, PivotItem changedRow)
        {
            int row = engine.PivotColumns.Count + ((engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? 1 : 0);
            int col = 0;
            foreach (PivotItem pi in engine.PivotRows)
            {
                object val = oldValue != null && pi == changedRow ? oldValue : item[pi.FieldMappingName].ToString();
                while (row < engine.RowCount)
                {
                    if (engine[row, col] == null)
                    {
                        break;
                    }
                    if (val.Equals(engine[row, col].Value))
                    {
                        break;
                    }
                    if (engine[row, col].Value != null && engine[row, col].Value.Equals('x'))
                    {
                        return -row;
                    }
                    row += (engine[row, col].CellRange != null) ? engine[row, col].CellRange.Bottom - engine[row, col].CellRange.Top + 2 : 1;
                }
                col++;
            }
            if (propertyName != null && !engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1)
            {
                foreach (PivotComputationInfo ci in engine.PivotCalculations)
                {

                    if (row >= engine.RowCount || ci.FieldName == propertyName)
                    {
                        break;
                    }
                    row++;
                }
            }
            return row;
        }
        #endregion

        #region virtual change event handlers....
        /// <summary>
        /// Override this method to catch the previous values of an underlying object before a change is applied.
        /// This method is the default handler for the object's PropertyChanging event.
        /// </summary>
        /// <param name="sender">The object that is changing.</param>
        /// <param name="e">The event arguments will contain the property name of the changed value.</param>
        protected virtual void item_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            changingValue = new ChangingValueInfo()
            {
                ColumnIndex = FindColumn(sender, e.PropertyName),
                RowIndex = FindRow(sender, e.PropertyName),
                OriginalValue = engine.ItemProperties[e.PropertyName].GetValue(sender)
            };
        }

        /// <summary>
        /// Override this method to control the response to a list changed event for a list that implements IBindingList.
        /// </summary>
        /// <param name="sender">The IBindingList.</param>
        /// <param name="e">The ListChanged event arguments.</param>

        protected virtual void UpdateRefreshHelper_ListChanged(object sender, ListChangedEventArgs e)
        {
            notFound = false;
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    {
                        object o = ((IList)sender)[e.NewIndex];
                        int col = FindColumn(o, null);
                        int row = FindRow(o, null);
                        if (row < 0 || row >= this.engine.PivotValues.Count)
                        {
                            engine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.RowAdded });
                        }
                        else foreach (var cal in engine.PivotCalculations)
                            {

                                object o1 = engine.ItemProperties[cal.FieldName].GetValue(o);


                                ChangeValue(null, o1, o, cal.FieldName, row, col, true, false);
                                if (notFound)
                                {
                                    notFound = false;
                                    break;
                                }
                                if (engine.ShowCalculationsAsColumns)
                                    col++;
                                else
                                    row++;
                            }
                        if (o is INotifyPropertyChanged)
                        {
                            INotifyPropertyChanged item = o as INotifyPropertyChanged;
                            item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
                        }
                        if (o is INotifyPropertyChanging)
                        {
                            INotifyPropertyChanging item = o as INotifyPropertyChanging;
                            item.PropertyChanging += new PropertyChangingEventHandler(item_PropertyChanging);
                        }
                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging += new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
                    }
                    break;
                case ListChangedType.ItemDeleted:
                    {
                        //nop
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Override this method to control the response to a collection changed event for a list that implements ICollectionChanged.
        /// </summary>
        /// <param name="sender">The ICollectionChanged collection.</param>
        /// <param name="e">The NotifyCollectionChanges event arguments.</param>

        protected virtual void UpdateRefreshHelper_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            notFound = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (object o in e.NewItems)
                    {
                        int col = FindColumn(o, null);
                        int row = FindRow(o, null);
                        if (row < 0 || row >= engine.PivotValues.Count)
                        {
                            engine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs() { ChangeHints = SchemaChangeHints.RowAdded });
                        }
                        else
                        {

                            foreach (var cal in engine.PivotCalculations)
                            {
                                object o1 = engine.ItemProperties[cal.FieldName].GetValue(o);
                                ChangeValue(null, o1, o, cal.FieldName, row, col, true, false);


                                if (notFound)
                                {
                                    notFound = false;
                                    break;
                                }
                                if (engine.ShowCalculationsAsColumns)
                                    col++;
                                else
                                    row++;
                            }
                        }
                        if (o is INotifyPropertyChanged)
                        {
                            INotifyPropertyChanged item = o as INotifyPropertyChanged;
                            item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
                        }
                        if (o is INotifyPropertyChanging)
                        {
                            INotifyPropertyChanging item = o as INotifyPropertyChanging;
                            item.PropertyChanging += new PropertyChangingEventHandler(item_PropertyChanging);
                        }

                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging += new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:

                    foreach (object o in e.OldItems)
                    {
                        int col = FindColumn(o, null);
                        int row = FindRow(o, null);

                        foreach (var cal in engine.PivotCalculations)
                        {
                            object o1 = engine.ItemProperties[cal.FieldName].GetValue(o);
                            ChangeValue(o1, null, o, cal.FieldName, row, col, false, true);


                            if (engine.ShowCalculationsAsColumns)
                                col++;
                            else
                                row++;
                        }
                        if (o is INotifyPropertyChanged)
                        {
                            INotifyPropertyChanged item = o as INotifyPropertyChanged;
                            item.PropertyChanged -= new PropertyChangedEventHandler(Item_PropertyChanged);
                        }
                        if (o is INotifyPropertyChanging)
                        {
                            INotifyPropertyChanging item = o as INotifyPropertyChanging;
                            item.PropertyChanging -= new PropertyChangingEventHandler(item_PropertyChanging);
                        }

                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging -= new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Override this method to catch the previous values of an underlying object after a change is applied.
        /// This method is the default handler for the object's PropertyChange event. The base implementation
        /// calls the <see cref="ChangeValue"/> method to respond to the changed value.
        /// </summary>
        /// <param name="sender">The object that is changing.</param>
        /// <param name="e">The event arguments will contain the property name of the changed value.</param>
        protected virtual void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pivotCalculations = engine.PivotCalculations.Where(i => i.FieldName == e.PropertyName);
            int col = 0;
            int row = 0;
            IEnumerable<PivotComputationInfo> pivotComputationInfos = pivotCalculations as PivotComputationInfo[] ?? pivotCalculations.ToArray();
            for (int i = 0; i < pivotComputationInfos.Count(); i++)
            {
                if (changingValue == null)
                {
                    changingValue = new ChangingValueInfo()
                    {
                        ColumnIndex = FindColumn(sender, e.PropertyName),
                        RowIndex = FindRow(sender, e.PropertyName),
                        OriginalValue = null
                    };
                }
                col = changingValue.ColumnIndex;
                row = changingValue.RowIndex;

                //Gets column and row of the next Calculation with same filed name
                if (i > 0)
                {
                    if (engine.ShowCalculationsAsColumns)
                        col += engine.PivotCalculations.IndexOf(pivotComputationInfos.ElementAt(i));
                    else
                        row += engine.PivotCalculations.IndexOf(pivotComputationInfos.ElementAt(i));
                }

                ChangeValue(changingValue.OriginalValue, engine.ItemProperties[e.PropertyName].GetValue(sender), sender, e.PropertyName, row, col, true, true);

            }
        }

        /// <summary>
        /// Override this method to control how the changes affect the pivot display. The base implementation calls IAdjustable methods on the summaries to accomplish the change.
        /// </summary>
        /// <param name="oldValue">The previous value of the changed property.</param>
        /// <param name="newValue">The new value of teh changed property.</param>
        /// <param name="item">The object that holds the changed proeprty.</param>
        /// <param name="propertyName">The property name that was changed.</param>
        /// <param name="row">The row index in the PivotGridControl that is affected by the change.</param>
        /// <param name="col">The column index in the PivotGridControl that is affected by the change.</param>
        /// <param name="adjustAdd">If true, the IAdjustable.AdjustForNewContribution on the Summary for this row/column is called to make the adjustment.</param>
        /// <param name="adjustRemove">If true, the IAdjustable.AdjustForOldContribution on the Summary for this row/column is called to make the adjustment.</param>
        protected virtual void ChangeValue(object oldValue, object newValue, object item, string propertyName, int row, int col, bool adjustAdd, bool adjustRemove)
        {
            var v = engine.PivotCalculations.Where(c => c.FieldName == propertyName);
            if (v == null || v.Count() == 0)
            {
                ChangeRowColumnPivotValue(oldValue, newValue, item, propertyName);
                return;
            }
            int saveRow = row;
            int saveCol = col;

            var pivotCalculations = engine.PivotCalculations.Where(c => c.FieldName == propertyName);

            foreach (var calc in pivotCalculations)
            {
                int calOffSet = engine.PivotCalculations.IndexOf(calc);
                int colOffSet = (engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? calOffSet : 0;
                int rowOffSet = (!engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? calOffSet : 0;
                PivotCellInfo pci = engine[row, col];
                BinaryList changingRows = new BinaryList();
                BinaryList changingCols = new BinaryList();

                if (pci != null)
                {
                    if (pci.Summary == null)
                    {
                        pci.Summary = calc.Summary.GetInstance();
                    }

                    if (pci.Summary is IAdjustable)
                    {
                        //process from cell downward, adjusting the summaries

                        int col1 = engine.PivotRows.Count - 1;
                        int row1 = engine.PivotColumns.Count - 1;
                        bool notDone = true;
                        while (notDone && col1 > -1)
                        {
                            col1--;
                            changingRows.AddIfUnique(row);
                            if (col1 > -1)
                            {
                                row = GetNextSummaryDownRowIndex(row, col1) + rowOffSet;
                                if (row == -1)
                                {
                                    notDone = false;
                                }
                            }
                        }
                        //handle grand total at bottom
                        row = engine.RowCount - 2 - rowOffSet;
                        changingRows.AddIfUnique(row);

                        //process from right of cell over to the right
                        col = saveCol;
                        row = saveRow;
                        notDone = true;
                        while (notDone && row1 > -1)
                        {
                            row1--;
                            changingCols.AddIfUnique(col);
                            if (row1 > -1)
                            {
                                col = GetNextSummaryOverColIndex(row1, col) + colOffSet;
                                if (col == -1)
                                {
                                    notDone = false;
                                }
                            }
                        }
                        //handle grand total on right
                        col = engine.ColumnCount - 2 - (engine.PivotCalculations.Count - colOffSet - 1);
                        changingCols.AddIfUnique(col);

                        //adjust all necessary summaries to the right and below (and including) of the changed summary
                        foreach (int r in changingRows)
                        {
                            foreach (int c in changingCols)
                            {
                                pci = engine[r, c];
                                if (pci != null && pci.Summary != null)
                                {
                                    if (pci.Summary is IAdjustable)
                                    {
                                        if (adjustRemove)
                                        {
                                            if (oldValue != null)
                                                ((IAdjustable)pci.Summary).AdjustForOldContribution(oldValue);
                                        }
                                        if (adjustAdd)
                                        {
                                            ((IAdjustable)pci.Summary).AdjustForNewContribution(newValue);
                                        }
                                    }
                                    engine[r, c].Value = pci.Summary.GetResult();
                                    engine[r, c].FormattedText = string.Format("{0:" + engine.PivotCalculations[col % engine.PivotCalculations.Count].Format + "}", engine[r, c].Value);
                                    CheckUpdateCell(r + 1, c + 1);
                                }
                            }
                        }

                    }
                }
                else
                {
                    notFound = true;
                    //new value...
                    engine.Populate();
                }
            }
        }

        #endregion
    }

    //class used to track the changing value information
    class ChangingValueInfo
    {
        public object OriginalValue { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
    }

    /// <summary>
    /// A collection with row and column coordinates
    /// </summary>
    public struct RowColumnIndex
    {
        // Fields
        int _rowIndex;
        int _columnIndex;

        // Constructors

        /// <summary>
        /// Initializes a new <see cref="RowColumnIndex"/> with row and column coordinates.
        /// </summary>
        /// <param name="r">The row index.</param>
        /// <param name="c">The column index.</param>
        public RowColumnIndex(int r, int c)
        {
            this._rowIndex = r;
            this._columnIndex = c;
        }

        /// <summary>
        /// Gets the empty instance with RowIndex and ColumnIndex set to int.MinValue
        /// </summary>
        /// <value>The empty.</value>
        public static RowColumnIndex Empty
        {
            get
            {
                return new RowColumnIndex(int.MinValue, int.MinValue);
            }
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            RowColumnIndex other = (RowColumnIndex)obj;

            return other.RowIndex == RowIndex && other.ColumnIndex == ColumnIndex;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get
            {
                return _rowIndex == int.MinValue;
            }
        }


        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return unchecked((int)(this._rowIndex * 2654435761u + this._columnIndex));
        }

        /// <summary>
        /// Returns the type name with state of this instance.
        /// </summary>
        /// <returns>
        /// </returns>
        public override string ToString()
        {
            return String.Concat(
                new string[] 
				{
					"RowColumnPosition { RowIndex = ",
					this.RowIndex.ToString(),
					", ColumnIndex = ",
					this.ColumnIndex.ToString(),
					"}"
				}
                );
        }

        /// <summary>
        /// The column index.
        /// </summary>
        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
            set
            {
                this._columnIndex = value;
            }
        }

        /// <summary>
        /// The row index.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return this._rowIndex;
            }
            set
            {
                this._rowIndex = value;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="r1">The r1.</param>
        /// <param name="r2">The r2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(RowColumnIndex r1, RowColumnIndex r2)
        {
            return r1.Equals(r2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="r1">The r1.</param>
        /// <param name="r2">The r2.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(RowColumnIndex r1, RowColumnIndex r2)
        {
            return !r1.Equals(r2);
        }
    }
}
