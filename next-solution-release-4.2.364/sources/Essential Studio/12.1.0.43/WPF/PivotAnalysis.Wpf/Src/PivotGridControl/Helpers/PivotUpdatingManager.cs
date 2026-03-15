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
using Syncfusion.Windows.Controls.Grid;
using System.Windows;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using Syncfusion.Windows.Controls.Cells;
#if !SILVERLIGHT
using System.Data;
#endif
#if SILVERLIGHT
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.Silverlight.Controls.PivotGrid;
using System.Reflection;
#endif

namespace Syncfusion.Windows.Controls.PivotGrid
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
    ///     B) an IList&lt;T&gt; where T implements both INotifyPropertyChanging and INotifyPropertyChanged. Additionally, the IList must also
    ///     implement INotifyCollectionChanged or IBindingList."
    /// </remarks>
    public class PivotUpdatingManager : IDisposable
    {
        GridControlBase gridBase = null;
        PivotEngine engine = null;
        bool isDisposed = false;
        bool saveStatePersistenceEnabled = false;
        PivotGridControl grid = null;

        /// <summary>
        /// Initializes the <see cref="PivotUpdatingManager"/> class.
        /// </summary>
        /// <param name="grid">The PivotGidControl that should be responding to updates.</param>
        public PivotUpdatingManager(PivotGridControl grid)
        {
            this.grid = grid;
#if Silverlight5
            if (this.grid.EnableUpdating)
            {
                InitializeUpdatingManager(grid);
            }
#endif

#if !SILVERLIGHT
            if (grid.IsLoaded)
            {
                InitializeUpdatingManager(grid);
            }
            else
            {
#endif
            grid.Loaded += (s, e) =>
                {
                    if (grid.InternalGrid == null)
                    {
                        grid.ApplyTemplate();
                    }
                    InitializeUpdatingManager(grid);
                };
            }
#if !SILVERLIGHT
        }
#endif

        internal void InitializeUpdatingManager(PivotGridControl grid)
        {
            this.gridBase = grid.InternalGrid;// gridBase;
            this.engine = grid.PivotEngine;// engine;
            saveStatePersistenceEnabled = grid.StatePersistenceEnabled;
            grid.StatePersistenceEnabled = true;
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
                    //updateTimer_Tick(null, EventArgs.Empty);//process any pending updates...
                    updateTimer = null;
                }
                this.grid.StatePersistenceEnabled = saveStatePersistenceEnabled;
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


        internal void WireEvents()
        {
            if (this.engine != null && this.engine.DataSourceList != null && this.engine.DataSourceList is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)this.engine.DataSourceList).CollectionChanged -= new NotifyCollectionChangedEventHandler(UpdateRefreshHelper_CollectionChanged);
                ((INotifyCollectionChanged)this.engine.DataSourceList).CollectionChanged += new NotifyCollectionChangedEventHandler(UpdateRefreshHelper_CollectionChanged);
            }
#if !SILVERLIGHT
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
#endif

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
                                item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
                            }
                        }
#if !SILVERLIGHT || Silverlight5
                        if (o is INotifyPropertyChanging)
                        {
                            foreach (INotifyPropertyChanging item in list)
                            {
                                item.PropertyChanging -= new PropertyChangingEventHandler(item_PropertyChanging);
                                item.PropertyChanging += new PropertyChangingEventHandler(item_PropertyChanging);
                            }
                        }
#endif
#if !SILVERLIGHT
                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging -= new DataColumnChangeEventHandler(Table_ColumnChanging);
                            drv.DataView.Table.ColumnChanging += new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
#endif
                    }
                }
            }
        }

        internal void UnwireEvents()
        {
            if (gridBase != null && this.engine.DataSourceList != null && this.engine.DataSourceList is INotifyCollectionChanged)
            {
                ((INotifyCollectionChanged)this.engine.DataSourceList).CollectionChanged -= new NotifyCollectionChangedEventHandler(UpdateRefreshHelper_CollectionChanged);
            }
#if !SILVERLIGHT
            else if (this.engine != null && this.engine.DataSourceList != null && this.engine.DataSourceList is IBindingList)
            {
                ((IBindingList)this.engine.DataSourceList).ListChanged -= new ListChangedEventHandler(UpdateRefreshHelper_ListChanged);
            }
#endif

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
#if !SILVERLIGHT || Silverlight5
                        if (o is INotifyPropertyChanging)
                        {
                            foreach (INotifyPropertyChanging item in list)
                            {
                                item.PropertyChanging -= new PropertyChangingEventHandler(item_PropertyChanging);
                            }
                        }
#endif
#if !SILVERLIGHT
                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging -= new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
#endif
                    }
                }
            }
        }
#if !SILVERLIGHT
        private void Table_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            changingValue = new ChangingValueInfo()
            {
                ColumnIndex = FindColumn(e.Row, e.Column.ColumnName),
                RowIndex = FindRow(e.Row, e.Column.ColumnName),
                OriginalValue = e.Row[e.Column.ColumnName]
            };
        }
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

#endif
#if SILVERLIGHT
        private object[] ProcessList(List<PivotItem> pivotItems)
        {
            int count = pivotItems.Count;
            object[] pds = new object[count];

            for (int i = 0; i < count; i++)
            {

                if (grid.PivotEngine.ItemProperties != null && grid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] is PropertyInfo)
                    pds[i] = grid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] as PropertyInfo;
                else if (grid.PivotEngine.ItemProperties != null && grid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] is ExpressionPropertyDescriptor)
                    pds[i] = grid.PivotEngine.ItemProperties[pivotItems[i].FieldMappingName] as ExpressionPropertyDescriptor;

            }

            return pds;
        }
#endif
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


#if !SILVERLIGHT

                foreach (var cal in engine.PivotCalculations)
                {
                    object o1 = engine.ItemProperties[cal.FieldName].GetValue(item);
                    ChangeValue(o1, null, item, cal.FieldName, row1, col1, false, true);


#else
                    object[] calPDs = GetCalcValuesPDs();
                    for (int k = 0; k < grid.PivotCalculations.Count; k++)
                    {
                        if (calPDs[k] is PropertyInfo)
                        {
                            PropertyInfo info = calPDs[k] as PropertyInfo;

                            object o1 = info.GetValue(item, null) as IComparable;
                            ChangeValue(o1,null, item, info.Name, row1, col1,false,true);
                        }
                        else if (calPDs[k] is ExpressionPropertyDescriptor)
                        {
                            ExpressionPropertyDescriptor expDesc = calPDs[k] as ExpressionPropertyDescriptor;
                            object o1 = expDesc.GetValue(item) as IComparable;
                            ChangeValue(o1,null, item, expDesc.Name, row1, col1,false,true);
                        }

                   
#endif

                    if (engine.ShowCalculationsAsColumns)
                        col1++;
                    else
                        row1++;

                }

                //add the new one
                col1 = FindColumn(item, null, newValue, v.First());
                row1 = FindRow(item, null, newValue, v.First());


#if !SILVERLIGHT
                foreach (var cal in engine.PivotCalculations)
                {
                    object o1 = engine.ItemProperties[cal.FieldName].GetValue(item);
                    ChangeValue(null, o1, item, cal.FieldName, row1, col1, true, false);
#else


                for (int k = 0; k < grid.PivotCalculations.Count; k++)
                {
                    if (calPDs[k] is PropertyInfo)
                    {
                        PropertyInfo info = calPDs[k] as PropertyInfo;

                        object o1 = info.GetValue(item, null) as IComparable;
                        ChangeValue(null, o1, item, info.Name, row1, col1,true,false);
                    }
                    else if (calPDs[k] is ExpressionPropertyDescriptor)
                    {
                        ExpressionPropertyDescriptor expDesc = calPDs[k] as ExpressionPropertyDescriptor;
                        object o1 = expDesc.GetValue(item) as IComparable;
                        ChangeValue(null, o1, item, expDesc.Name, row1, col1,true,false);
                    }

                   
#endif

                    if (engine.ShowCalculationsAsColumns)
                        col1++;
                    else
                        row1++;

                }
            }
        }

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
#if !SILVERLIGHT
                            gridBase.Dispatcher.Invoke(new EventHandler(updateTimer_Tick), new object[] { null, EventArgs.Empty });
#else
                            gridBase.Dispatcher.BeginInvoke(new EventHandler(updateTimer_Tick), new object[] { null, EventArgs.Empty });
#endif
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

        private void UpdateCell(int row, int col)
        {
            if (gridBase.Dispatcher != null)
            {
#if !SILVERLIGHT
                if (gridBase.Dispatcher.Thread != Thread.CurrentThread)
#else 
                    if (!gridBase.Dispatcher.CheckAccess())
#endif
                {
                    gridBase.Dispatcher.BeginInvoke(new UpdateCellDelegate(UpdateCell), new object[] { row, col });
                    return;
                }
            }

#if SILVERLIGHT
            gridBase.InvalidateCells();
#else
            if (grid.ValueCellStyle.Style != null)
            {
                if (gridBase.CurrentCell.HasCurrentCell && gridBase.CurrentCell.IsEditing && gridBase.CurrentCell.RowIndex == row && gridBase.CurrentCell.ColumnIndex == col)
                    gridBase.CurrentCell.Deactivate();
                gridBase.InvalidateCells();
            }
            else
                gridBase.InvalidateCell(GridRangeInfo.Cell(row, col));
#endif
        }

        private void InsertRow(int row, object value)
        {
            if (gridBase.Dispatcher != null)
            {
#if !SILVERLIGHT
                if (gridBase.Dispatcher.Thread != Thread.CurrentThread)
#else
                if (!gridBase.Dispatcher.CheckAccess())
#endif
                {
                    gridBase.Dispatcher.BeginInvoke(new InsertRowDelegate(InsertRow), new object[] { row, value });
                    return;
                }
            }
            gridBase.Model.RowCount++;

            List<CoveredCellInfo> newRanges = new List<CoveredCellInfo>();
            foreach (CoveredCellInfo r in gridBase.Model.CoveredCells)
            {
                if (r.Left >= engine.PivotRows.Count)
                {
                    newRanges.Add(r);
                    continue; //skip ranges from column pivots
                }
                if (r.Top >= row)
                {
                    newRanges.Add(new CoveredCellInfo(r.Top + 1, r.Left, r.Bottom + 1, r.Right));
                }
                else if (r.Bottom + 1 >= row)
                {
                    newRanges.Add(new CoveredCellInfo(r.Top, r.Left, r.Bottom + 1, r.Right));
                }
                else
                {
                    newRanges.Add(r);
                }
            }
            gridBase.Model.CoveredCells.Clear();
            foreach (CoveredCellInfo r in newRanges)
            {
                gridBase.Model.CoveredCells.Add(r);
            }
            gridBase.InvalidateCells();

#if !SILVERLIGHT
            for (int col = 0; col <= gridBase.Model.ColumnCount; ++col)
            {
                gridBase.Model[row - 1, col] = gridBase.Model[row, col];
                if (col < engine.PivotRows.Count)
                {
                    gridBase.Model[row, col].CellValue = engine.ItemProperties[engine.PivotRows[col].FieldMappingName].GetValue(value);
                }
#else
                     object[] colPDs = ProcessList(grid.PivotEngine.PivotColumns);
                     
                 for (int col = 0; col <= gridBase.Model.ColumnCount; ++col)
                 {
                     gridBase.Model[row - 1, col] = gridBase.Model[row, col];
                     if (colPDs[col] is PropertyInfo && col < engine.PivotRows.Count)
                     {
                           PropertyInfo info = colPDs[col] as PropertyInfo;
                           gridBase.Model[row, col].CellValue = info.GetValue(value,null);
                     }
                     else if (colPDs[col] is ExpressionPropertyDescriptor)
                     {
                         ExpressionPropertyDescriptor expDesc = colPDs[col] as ExpressionPropertyDescriptor;
                         gridBase.Model[row, col].CellValue = expDesc.GetValue(value);
                     }
               
#endif

                else
                {
                    gridBase.Model[row, col].CellValue = null;
                }
            }

        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (grid.ValueCellStyle.Style != null)
            {
                gridBase.InvalidateCells();
            }
            else
            {
                foreach (RowColumnIndex rci in pendingUpdates)
                {
                    gridBase.InvalidateCell(GridRangeInfo.Cell(rci.RowIndex, rci.ColumnIndex));
                }
            }          

            pendingUpdates.Clear();
        }

        private int GetNextSummaryOverColIndex(int row1, int col)
        {
            int sumCol = -1, column = col;
            PivotCellInfo pci = engine[row1, col];
            while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && col > 0)
            {
                col--;
                pci = engine[row1, col];
            }
            //sumCol = ((column - engine.PivotRows.Count) % engine.PivotCalculations.Count)+ ((engine[row1, col].CellRange != null) ? engine[row1, col].CellRange.Right - engine[row1, col].CellRange.Left + 1 : 1);
            if (engine.ShowCalculationsAsColumns)
                sumCol = col + ((column - engine.PivotRows.Count) % engine.PivotCalculations.Count) + ((engine[row1, col].CellRange != null) ? engine[row1, col].CellRange.Right - engine[row1, col].CellRange.Left + 1 : 1);
            else
                sumCol = col + engine[row1, col].CellRange.Right - engine[row1, col].CellRange.Left + 1;
            return sumCol;
        }

        private int GetNextSummaryDownRowIndex(int row, int col1)
        {
            int sumRow = -1, tempRow = row;
            PivotCellInfo pci = engine[row, col1];

            while (pci != null && 0 == (pci.CellType & PivotCellType.ExpanderCell) && row > 0)
            {
                row--;
                pci = engine[row, col1];
            }
            if (engine.ShowCalculationsAsColumns)
            {
                sumRow = row + ((engine[row, col1].CellRange != null) ? engine[row, col1].CellRange.Bottom - engine[row, col1].CellRange.Top + 1 : 1);
            }
            else
            {
                sumRow = row + ((tempRow - engine.PivotColumns.Count) % engine.PivotCalculations.Count) + ((engine[row, col1].CellRange != null) ? engine[row, col1].CellRange.Bottom - engine[row, col1].CellRange.Top + 1 : 1);
            }
            return sumRow;
        }

        //if propertyName is null, then not searching for calculation....
        private int FindColumn(object item, string propertyName)
        {
            return FindColumn(item, propertyName, null, null);
        }
#if !SILVERLIGHT
        private int FindColumn(DataRow item, string propertyName)
        {
            return FindColumn(item, propertyName, null, null);
        }

        private int FindColumn(DataRow item, string propertyName, object oldValue, PivotItem changedColumn)
        {
            int col = engine.PivotRows.Count + ((!engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? 1 : 0);
            int row = 0;
            foreach (PivotItem pi in engine.PivotColumns)
            {
                // object val = engine.ItemProperties[pi.FieldMappingName].GetValue(item);
                // object val = oldValue != null ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item);
                object val = oldValue != null && pi == changedColumn ? oldValue : item[pi.FieldMappingName].ToString();

                while (col < engine.ColumnCount)
                {
                    if (engine[row, col] == null || engine[row, col].Value == " " || val.Equals(engine[row, col].Value))
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
#endif

        private int FindColumn(object item, string propertyName, object oldValue, PivotItem changedColumn)
        {
            int col = engine.PivotRows.Count + ((!engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? 1 : 0);
            int row = 0;

            // object val = engine.ItemProperties[pi.FieldMappingName].GetValue(item);
            // object val = oldValue != null ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item);
#if !SILVERLIGHT
            foreach (PivotItem pi in engine.PivotColumns)
            {
                object val = oldValue != null && pi == changedColumn ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item);

#else
                object[] colPDs = ProcessList(grid.PivotEngine.PivotColumns);
                
                object val = null;
                for (int k = 0; k < grid.PivotColumns.Count; k++)
                {
                    if (colPDs[k] is PropertyInfo)
                    {
                        PropertyInfo info = colPDs[k] as PropertyInfo;
                        val = info.GetValue(item, null);
                    }
                    else if (colPDs[k] is ExpressionPropertyDescriptor)
                    {
                        ExpressionPropertyDescriptor expDesc = colPDs[k] as ExpressionPropertyDescriptor;
                        val = expDesc.GetValue(item);
                    }
                

#endif

                while (col < engine.ColumnCount)
                {
                    if (engine[row, col] == null || engine[row, col].Value == " " || val == engine[row, col].Value)
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

        //if propertyName is null, then not searching for calculation....
        private int FindRow(object item, string propertyName)
        {
            return FindRow(item, propertyName, null, null);
        }
#if !SILVERLIGHT
        private int FindRow(DataRow item, string propertyName)
        {
            return FindRow(item, propertyName, null, null);
        }
#endif
        private int FindRow(object item, string propertyName, object oldValue, PivotItem changedRow)
        {
            int row = engine.PivotColumns.Count + (engine.ShowCalculationsAsColumns ? 1 : 0);
            int col = 0;

            // object val = engine.ItemProperties[pi.FieldMappingName].GetValue(item).ToString();
            // object val = oldValue != null ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item).ToString();
#if !SILVERLIGHT
            foreach (PivotItem pi in engine.PivotRows)
            {
                object val = oldValue != null && pi == changedRow ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item).ToString();
#else
                 
                 object[] rowPDs = ProcessList(grid.PivotEngine.PivotRows);
                 object val = null;
                 for (int k = 0; k < grid.PivotRows.Count; k++)
                 {
                     if (rowPDs[k] is PropertyInfo)
                     {
                         PropertyInfo info = rowPDs[k] as PropertyInfo;
                          val =info.GetValue(item, null).ToString();
                     }
                     else if (rowPDs[k] is ExpressionPropertyDescriptor)
                     {
                         ExpressionPropertyDescriptor expDesc = rowPDs[k] as ExpressionPropertyDescriptor;
                         val = expDesc.GetValue(item).ToString();
                     }
                 

#endif
                while (row < engine.RowCount)
                {
                    if (engine[row, col] == null || engine[row, col].Value == " ")
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
                    if (engine.ShowCalculationsAsColumns)
                        row += (engine[row, col].CellRange != null) ? engine[row, col].CellRange.Bottom - engine[row, col].CellRange.Top + 2 : 1;
                    else
                        row += (engine[row, col].CellRange != null) ? engine[row, col].CellRange.Bottom - engine[row, col].CellRange.Top + 1 : 1;

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
#if !SILVERLIGHT
        private int FindRow(DataRow item, string propertyName, object oldValue, PivotItem changedRow)
        {
            int row = engine.PivotColumns.Count + ((engine.ShowCalculationsAsColumns && engine.PivotCalculations.Count > 1) ? 1 : 0);
            int col = 0;
            foreach (PivotItem pi in engine.PivotRows)
            {
                // object val = engine.ItemProperties[pi.FieldMappingName].GetValue(item).ToString();
                // object val = oldValue != null ? oldValue : engine.ItemProperties[pi.FieldMappingName].GetValue(item).ToString();
                object val = oldValue != null && pi == changedRow ? oldValue : item[pi.FieldMappingName].ToString();
                while (row < engine.RowCount)
                {
                    if (engine[row, col] == null || engine[row, col].Value == " ")
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
                    // row += (engine[row, col].CellRange != null) ? engine[row, col].CellRange.Bottom - engine[row, col].CellRange.Top + 1 : 1;
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
#endif
        #endregion

        #region virtual change event handlers....
#if !SILVERLIGHT || Silverlight5
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
#if !SILVERLIGHT
                     OriginalValue = engine.ItemProperties[e.PropertyName].GetValue(sender)
#else
                     OriginalValue = (engine.ItemProperties[e.PropertyName] as PropertyInfo).GetValue(sender, null) as IComparable
#endif
                 };
        }
#endif
#if !SILVERLIGHT
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
#endif
        /// <summary>
        /// Override this method to control the response to a collection changed event for a list that implements ICollectionChanged.
        /// </summary>
        /// <param name="sender">The ICollectionChanged collection.</param>
        /// <param name="e">The NotifyCollectionChanges event arguments.</param>
#if SILVERLIGHT
        private object[] GetCalcValuesPDs()
        {
            object[] pds = new object[grid.PivotEngine.PivotCalculations.Count];

            for (int i = 0; i < grid.PivotEngine.PivotCalculations.Count; i++)
            {
                if (grid.PivotEngine.ItemProperties != null && grid.PivotEngine.ItemProperties[grid.PivotEngine.PivotCalculations[i].FieldName] is PropertyInfo)
                    pds[i] = grid.PivotEngine.ItemProperties[grid.PivotEngine.PivotCalculations[i].FieldName] as PropertyInfo;
                else if (grid.PivotEngine.ItemProperties != null && grid.PivotEngine.ItemProperties[grid.PivotEngine.PivotCalculations[i].FieldName] is ExpressionPropertyDescriptor)
                    pds[i] = grid.PivotEngine.ItemProperties[grid.PivotEngine.PivotCalculations[i].FieldName] as ExpressionPropertyDescriptor;

            }

            return pds;
        }
#endif
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

#if !SILVERLIGHT
                            foreach (var cal in engine.PivotCalculations)
                            {
                                object o1 = engine.ItemProperties[cal.FieldName].GetValue(o);
                                ChangeValue(null, o1, o, cal.FieldName, row, col, true, false);
#else
                            
                           
                    object[] calPDs = GetCalcValuesPDs();         
                    for (int k = 0; k < grid.PivotCalculations.Count; k++)
                    {
                        if (calPDs[k] is PropertyInfo)
                        {
                            PropertyInfo info = calPDs[k] as PropertyInfo;

                            object o1 = info.GetValue(o, null) as IComparable;
                            ChangeValue(null,o1, o, info.Name, row, col,true,false);
                        }
                        else if (calPDs[k] is ExpressionPropertyDescriptor)
                        {
                            ExpressionPropertyDescriptor expDesc = calPDs[k] as ExpressionPropertyDescriptor;
                            object o1 = expDesc.GetValue(o) as IComparable;
                            ChangeValue(null,o1, o, expDesc.Name, row, col, true,false);
                        }

#endif

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
                            item.PropertyChanged -= new PropertyChangedEventHandler(Item_PropertyChanged);
                            item.PropertyChanged += new PropertyChangedEventHandler(Item_PropertyChanged);
                        }
#if!SILVERLIGHT||Silverlight5
                        if (o is INotifyPropertyChanging)
                        {
                            INotifyPropertyChanging item = o as INotifyPropertyChanging;
                            item.PropertyChanging -= new PropertyChangingEventHandler(item_PropertyChanging);
                            item.PropertyChanging += new PropertyChangingEventHandler(item_PropertyChanging);
                        }
#endif
#if !SILVERLIGHT
                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging += new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
#endif
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:

                    foreach (object o in e.OldItems)
                    {
                        int col = FindColumn(o, null);
                        int row = FindRow(o, null);

#if !SILVERLIGHT
                        foreach (var cal in engine.PivotCalculations)
                        {
                            object o1 = engine.ItemProperties[cal.FieldName].GetValue(o);
                            ChangeValue(o1, null, o, cal.FieldName, row, col, false, true);
#else
                       
                        object[] calPDs = GetCalcValuesPDs();

                        for (int k = 0; k < grid.PivotCalculations.Count; k++)
                        {
                            if (calPDs[k] is PropertyInfo)
                            {
                                PropertyInfo info = calPDs[k] as PropertyInfo;

                                object o1 = info.GetValue(o, null) as IComparable;
                                ChangeValue(o1,null, o, info.Name, row, col, false, true);
                            }
                            else if (calPDs[k] is ExpressionPropertyDescriptor)
                            {
                                ExpressionPropertyDescriptor expDesc = calPDs[k] as ExpressionPropertyDescriptor;
                                object o1 = expDesc.GetValue(o) as IComparable;
                                ChangeValue(o1,null, o, expDesc.Name, row, col, false, true);
                            }

                            
#endif

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
#if !SILVERLIGHT || Silverlight5
                        if (o is INotifyPropertyChanging)
                        {
                            INotifyPropertyChanging item = o as INotifyPropertyChanging;
                            item.PropertyChanging -= new PropertyChangingEventHandler(item_PropertyChanging);
                        }
#endif
#if !SILVERLIGHT
                        else if (o is DataRowView)
                        {
                            DataRowView drv = o as DataRowView;
                            drv.DataView.Table.ColumnChanging -= new DataColumnChangeEventHandler(Table_ColumnChanging);
                        }
#endif
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    break;
#if !SILVERLIGHT
                case NotifyCollectionChangedAction.Move:
                    break;
#endif
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
            if (engine.PivotCalculations.Any(x => x.FieldName == e.PropertyName))
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
#if !SILVERLIGHT
                            OriginalValue = null
#else
                            OriginalValue = 0.0
#endif
                        };
                }
                if (i == 0 && engine.ShowCalculationsAsColumns)
                {
                    col = changingValue.ColumnIndex;
                    row = changingValue.RowIndex;
                }
                else if (!engine.ShowCalculationsAsColumns)
                {
                    col = changingValue.ColumnIndex;
                    row = FindRow(sender, e.PropertyName);
                }

                //Gets column and row of the next Calculation with same filed name
                if(i > 0)
                {
                    if (engine.ShowCalculationsAsColumns)
                        col += (engine.PivotCalculations.IndexOf(pivotComputationInfos.ElementAt(i))) - (engine.PivotCalculations.IndexOf(pivotComputationInfos.ElementAt(i - 1)));
                    else
                        row += engine.PivotCalculations.IndexOf(pivotComputationInfos.ElementAt(i));
                }

#if !SILVERLIGHT
                ChangeValue(changingValue.OriginalValue, engine.ItemProperties[e.PropertyName].GetValue(sender), sender, e.PropertyName, row, col, true, true);
#else
            object[] calPDs = GetCalcValuesPDs();
            for (int k = 0; k < grid.PivotCalculations.Count; k++)
            {
                if (calPDs[k] is PropertyInfo)
                {
                    PropertyInfo info = calPDs[k] as PropertyInfo;
                    if (info.Name==e.PropertyName)
                    {
                        object o1 = info.GetValue(sender, null) as IComparable;
                        ChangeValue(changingValue.OriginalValue, o1, sender, info.Name, row, col, true, true);
                        break;
                    }
                }
                else if (calPDs[k] is ExpressionPropertyDescriptor)
                {
                    ExpressionPropertyDescriptor expDesc = calPDs[k] as ExpressionPropertyDescriptor;
                    object o1 = expDesc.GetValue(sender) as IComparable;
                    ChangeValue(changingValue.OriginalValue, o1, sender, expDesc.Name, row, col, true, true);
                }
          }
   
#endif
                }
            }
            else if(engine.PivotRows.Any(x => x.FieldMappingName == e.PropertyName) || engine.PivotColumns.Any(x => x.FieldMappingName == e.PropertyName))
            {
                this.engine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
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
            //int calOffSet = engine.PivotCalculations.IndexOf(engine.PivotCalculations.Where(c => c.FieldName == propertyName).First() as PivotComputationInfo);
            var v = engine.PivotCalculations.Where(c => c.FieldName == propertyName);
            if (v == null || v.Count() == 0)
            {
                ChangeRowColumnPivotValue(oldValue, newValue, item, propertyName);
                return;
            }
            int saveRow = row;
            int saveCol = col;
            PivotComputationInfo calc = engine.PivotCalculations[(col - (engine.PivotRows.Count + (engine.ShowCalculationsAsColumns ? 0 : 1))) % engine.PivotCalculations.Count];

            if (calc != null)
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
                                row = GetNextSummaryDownRowIndex(row, col1) + (engine.ShowCalculationsAsColumns ? rowOffSet : (engine.PivotCalculations.Count - rowOffSet - 1));
                                if (row == -1)
                                {
                                    notDone = false;
                                }
                            }
                        }
                        //handle grand total at bottom
                        if (engine.ShowCalculationsAsColumns)
                            row = engine.RowCount - 2 - rowOffSet;
                        else
                            row = (engine.RowCount - 1) - (engine.PivotCalculations.Count - ((row - engine.PivotColumns.Count) % engine.PivotCalculations.Count));
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
                                col = GetNextSummaryOverColIndex(row1, col);// +(this.engine.PivotRows.Count);
                                if (col == -1)
                                {
                                    notDone = false;
                                }
                            }
                        }
                        //handle grand total on right
                        if (engine.ShowCalculationsAsColumns)
                            col = (engine.ColumnCount - 1) - (engine.PivotCalculations.Count - ((col - engine.PivotRows.Count) % engine.PivotCalculations.Count));
                        else ////Last column only grand total here.
                            col = engine.ColumnCount - 2 - colOffSet;
                        //Column count returns without grand total column adjustment when Show Grand Total is false. Hence we added the below if statement to compensate.
                        if (!engine.ShowGrandTotals)
                            col += engine.PivotCalculations.Count;
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
                                    if (engine.ShowCalculationsAsColumns)
                                    {
                                        engine[r, c].FormattedText = string.Format("{0:" + engine.PivotCalculations[(c - engine.PivotRows.Count) % engine.PivotCalculations.Count].Format + "}", engine[r, c].Value);
                                    }
                                    else
                                    {
                                        engine[r, c].FormattedText = string.Format("{0:" + engine.PivotCalculations[(r - engine.PivotColumns.Count) % engine.PivotCalculations.Count].Format + "}", engine[r, c].Value);

                                    }
                                    //gridBase.InvalidateCell(GridRangeInfo.Cell(r + 1, c + 1));
                                    //while (inCopy) { };
                                    // CheckUpdateCell(r + 1, c + 1);
                                    if (engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType == CalculationType.NoCalculation)
                                    {
                                        CheckUpdateCell(r, c);
                                    }
                                    // gridBase.Dispatcher.Invoke(new UpdateCellDelegate(UpdateCell), new object[] { r + 1, c + 1 });
                                }
                            }
                        }

                        if (engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType != CalculationType.NoCalculation)
                        {
                            col = saveCol;
                            if (engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType == CalculationType.PercentageOfParentRowTotal || engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType == CalculationType.PercentageOfColumnTotal)
                            {
                                AddChangingRowIndexes(rowOffSet, changingRows);
                            }
                            else if (engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType == CalculationType.PercentageOfParentColumnTotal || engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType == CalculationType.PercentageOfRowTotal)
                            {
                                AddChangingColumnIndexes(col, colOffSet, changingCols);
                            }
                            else if (engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType == CalculationType.PercentageOfParentTotal)
                            {
                                PivotComputationInfo compInfo = this.engine.PivotCalculations[(col - engine.PivotRows.Count) % this.engine.PivotCalculations.Count];
                                if (compInfo.BaseField != null)
                                {
#if !SILVERLIGHT
                                    int index = this.engine.PivotRows.FindIndex(i => i.FieldMappingName == compInfo.BaseField);
#else
                                    int index = -1;
                                    for (int i = 0; i < this.engine.PivotRows.Count; i++)
                                    {
                                        if (this.engine.PivotRows[i].FieldMappingName == compInfo.BaseField)
                                        {
                                            index = i;
                                            break;
                                        }
                                    }
#endif
                                    if (index != -1) //Selected base field is in Pivot Rows. Then calculation should be based on Row values.
                                        AddChangingRowIndexes(rowOffSet, changingRows);
                                    else
                                    {
#if !SILVERLIGHT
                                        index = this.engine.PivotColumns.FindIndex(i => i.FieldMappingName == compInfo.BaseField);
#else
                                        for (int i = 0; i < this.engine.PivotColumns.Count; i++)
                                        {
                                            if (this.engine.PivotColumns[i].FieldMappingName == compInfo.BaseField)
                                            {
                                                index = i;
                                                break;
                                            }
                                        }
#endif
                                        if (index != -1)
                                            AddChangingColumnIndexes(col, colOffSet, changingCols);
                                    }
                                }
                            }
                            else if (engine.PivotCalculations[(col - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType == CalculationType.PercentageOfGrandTotal)
                            {
                                AddChangingRowIndexes(rowOffSet, changingRows);
                                AddChangingColumnIndexes(col, colOffSet, changingCols);
                            }

                            foreach (int changerow in changingRows)
                            {
                                foreach (int changecolumn in changingCols)
                                {
                                    engine[changerow, changecolumn].FormattedText = engine.UpdateCalculatedValue(changerow, changecolumn, engine[changerow, changecolumn].Summary.GetResult(), engine.PivotCalculations.ToArray().FirstOrDefault().FieldName);
                                    CheckUpdateCell(changerow, changecolumn);
                                }
                            }
                        }
                    }
                }
                else
                {
                    notFound = true;
                    //new value...
                    //engine.Populate();
                   grid.InternalGrid.Populate(engine);
                }
            }
        }

        private void AddChangingColumnIndexes(int col, int colOffSet, BinaryList changingCols)
        {
            int startCol = 0, endCol = 0;
            startCol = this.engine.ShowCalculationsAsColumns ? this.engine.PivotRows.Count : (this.engine.PivotCalculations.Count > 1 ? 1 : 0);

            if (engine.ShowCalculationsAsColumns)
                endCol = (engine.ColumnCount - 1) - (engine.PivotCalculations.Count - ((col - engine.PivotRows.Count) % engine.PivotCalculations.Count));
            else
                endCol = engine.ColumnCount - 2 - colOffSet;
            for (int i = startCol; i < endCol; i++)
            {
                if (engine.PivotCalculations[(i - engine.PivotRows.Count) % engine.PivotCalculations.Count].CalculationType != CalculationType.NoCalculation)
                    changingCols.AddIfUnique(i);
            }
        }

        private void AddChangingRowIndexes(int rowOffSet, BinaryList changingRows)
        {
            int startRow = 0, endRow = 0;

            startRow = this.engine.PivotColumns.Count + (this.engine.ShowCalculationsAsColumns && this.engine.PivotCalculations.Count > 1 ? 1 : 0);
            if (engine.ShowCalculationsAsColumns)
                endRow = engine.RowCount - 2 - rowOffSet;
            else
                endRow = engine.RowCount - 2 - (engine.PivotCalculations.Count - rowOffSet - 1);
            for (int i = startRow; i < endRow; i++)
            {
                changingRows.AddIfUnique(i);
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

    
}
