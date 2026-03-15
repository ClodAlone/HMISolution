#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using Syncfusion.Windows.Forms.Grid.Grouping;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Grouping;
using Syncfusion.Windows.Forms;
using System.Globalization;
using System.Collections;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Collections.BinaryTree;
using System.Reflection;
using System.ComponentModel;
using Syncfusion.Diagnostics;

namespace Syncfusion.GridHelperClasses
{
    #region Pager class
    /// <summary>
    /// A Paging helper that can be wired with GridGroupingControl to enable data paging with IEnumerable type(DataTable) data source.
    /// </summary>
    public class Pager
    {
        int _pageCount;
        int _maxRec;
        int _pageSize = 1000;
        int _currentPage;
        int _recNo;
        private DataTable _dt;
        private readonly Label _myLabel = new Label();
        private Engine _engine;
        private GridTableFilterBarGridListCellModelExt _model;
        private GridGroupingControl _grid;
#if !SyncfusionFramework2_0 && !SyncfusionFramework3_5
        private Dictionary<string, Type> types = new Dictionary<string, Type>();
        List<DynamicPropertyDescriptor> props = new List<DynamicPropertyDescriptor>();
#endif

        /// <summary>
        /// Gets or sets the page size(record count to display in each page).
        /// </summary>
        /// <value>An integer value.</value>
        /// <remarks>1000, by default.</remarks>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        /// <summary>
        /// Wires the paging helper with GridGroupingControl.
        /// </summary>
        /// <param name="groupingGrid">GridGroupingControl</param>
        /// <param name="table">DataTable</param>
        public void Wire(GridGroupingControl groupingGrid, DataTable table)
        {
            _grid = groupingGrid;
            _dt = table;
            InitializePager();
        }

        /// <summary>
        /// Wires the paging helper with GridGroupingControl.
        /// </summary>
        /// <param name="groupingGrid">GridGroupingControl</param>
        public void Wire(GridGroupingControl groupingGrid)
        {
            _grid = groupingGrid;
            _dt = GetSourceListBase(groupingGrid.DataSource);
            InitializePager();
        }

        /// <summary>
        /// Unhook the GridGroupingControl from the paging.
        /// </summary>
        /// <param name="groupingGrid">GridGroupingControl</param>
        public void Unwire(GridGroupingControl groupingGrid)
        {
            if (groupingGrid != null)
            {
                groupingGrid.BeginUpdate();
                groupingGrid.RecordNavigationBar.CurrentRecordChanging -= RecordNavigationBar_CurrentRecordChanging;
                groupingGrid.Engine.SourceListChanged -= new EventHandler(_engine_SourceListChanged);
                groupingGrid.RecordNavigationBar.ArrowButtonClicked -= RecordNavigationBar_ArrowButtonClicked;
                groupingGrid.FilterBarSelectedItemChanging -= _grid_FilterBarSelectedItemChanging; groupingGrid.EndUpdate(true);
                groupingGrid.Table.RecordValueChanging += new RecordValueChangingEventHandler(Table_RecordValueChanging);
                groupingGrid.Refresh();
                groupingGrid = null;
            }
        }

        /// <summary>
        /// A method that returns DataTable from the dataSource of GridGroupingControl
        /// </summary>
        /// <param name="dataSource">DataSource of GridGroupingControl</param>
        /// <returns>returns the GridGroupingControl datasource as dataTable</returns> 
        private DataTable GetSourceListBase(object dataSource)
        {
            this.CheckValidDataSource(dataSource);
            if ((dataSource is IPassThroughGroupingResult))
            {
                if (dataSource == null)
                {
                    return null;
                }
                return ConvertDataTableFromIEnumerable((IList)ResolveBindingSource(dataSource));
            }
            else
            {
                if (dataSource != null)
                {
                    object value = ResolveBindingSource(dataSource);

                    IEnumerable list = null;
                    if (value is System.Data.DataView)
                    {
                        list = new DataTableList(((System.Data.DataView)value).Table);
                    }
                    else if (value is System.Data.DataTable)
                    {
                        list = new DataTableList((System.Data.DataTable)value);
                    }
#if !SyncfusionFramework2_0 && !SyncfusionFramework3_5
                        else if (this._grid.Table.Engine.IsDynamicData)
                        {
                            if (this._grid.Table.Engine.IsDynamicData)
                            {
                                PropertyDescriptorCollection itemProperties = null;
                                IDictionary<string, object> dynamicObject = null;

                                IEnumerable source = this._grid.Table.Engine.GetSourceList();
                                if (source != null)
                                {
                                    foreach (object o in source)
                                    {
                                        dynamicObject = o as IDictionary<string, object>;
                                        if (dynamicObject == null)
                                            continue;
                                        break;
                                    }
                                    if (dynamicObject != null)
                                    {
                                        types.Clear();
                                        props.Clear();
                                        PopulateDynamicPropertiesandTypes(dynamicObject);
                                        itemProperties = new PropertyDescriptorCollection(props.ToArray());
                                        DataTable table = new DataTable();
                                        foreach (PropertyDescriptor prop in itemProperties)
                                        {
                                            // HERE IS WHERE THE ERROR IS THROWN FOR NULLABLE TYPES
                                            table.Columns.Add(prop.Name, prop.PropertyType);
                                        }
                                        foreach (object o in source)
                                        {
                                            DataRow row = table.NewRow();
                                            foreach (PropertyDescriptor prop in itemProperties)
                                            {
                                                row[prop.Name] = prop.GetValue(o);
                                            }

                                            table.Rows.Add(row);
                                        }
                                        return table;
                                    }
                                }
                            }
                        }
#endif
                    if (list == null)
                    {
                        if (value is IEnumerable)
                        {
                            list = (IEnumerable)value;
                        }
                        else if (value is IListSource)
                        {
                            list = ((IListSource)value).GetList();
                        }
                    }

                    if (list != null)
                    {
                        return ConvertDataTableFromIEnumerable(list);
                    }
                }
            }

            return null;
        }

#if !SyncfusionFramework2_0 && !SyncfusionFramework3_5
        /// <summary>
        /// Retrieves the dynamic properties and its design time types for internal usage.
        /// </summary>
        /// <param name="obj">dynamic item</param>
        private void PopulateDynamicPropertiesandTypes(IDictionary<string, object> obj)
        {
            foreach (string key in obj.Keys)
            {
                if (obj[key] is IDictionary<string, object>)
                {
                    PopulateDynamicPropertiesandTypes(obj[key] as IDictionary<string, object>);
                }
                else if (!types.ContainsKey(key))
                {
                    props.Add(new DynamicPropertyDescriptor(key, null));
                    if (obj[key] != null)
                        types.Add(key, obj[key].GetType());
                    else
                        types.Add(key, typeof(object));
                }
            }
        }
#endif

        /// <summary>
        /// A method used to get the resolved binding source
        /// </summary>
        /// <param name="value">DataSource as object</param>
        /// <returns>DataSource</returns>
        private object ResolveBindingSource(object value)
        {
#if !ASPNET && SyncfusionFramework2_0
                // Resolve BindingSource to real list.
                System.Windows.Forms.BindingSource bs = value as System.Windows.Forms.BindingSource;
                if (bs != null && bs.List is System.Data.DataSet)
                {
                    value = bs.List;
                }
                //// look only for a dataset - if it is a regular list, e.g. custom collection 
                //// bind directly to BindingSource.
                IListSource ls = value as System.Data.DataSet;
                while (ls != null && ls.ContainsListCollection)
                {
                    value = ls.GetList();
                    ls = value as IListSource;
                }
#endif
            return value;
        }

        /// <summary>
        /// A method to check the the DataSource is valid or not
        /// </summary>
        /// <param name="value">DataSource</param>
        void CheckValidDataSource(object value)
        {
            if (!(value is DBNull) && value != null && !(value is IEnumerable) && !(value is IListSource) && !(value is System.Data.DataTable))
            {
                throw new Exception("BadDataSourceForComplexBinding");
            }
        }
       
        /// <summary>
        /// A method that initializes page settings and loads the default page.
        /// </summary>
        private void InitializePager()
        {
            // Set the start and max records. 
            _maxRec = _dt.Rows.Count;
            _pageCount = _maxRec / _pageSize;

            //Adjust the page number if the last page contains a partial page.
            if ((_maxRec % _pageSize) > 0)
            {
                _pageCount++;
            }

            // Initial seeings
            _currentPage = 1;
            _recNo = 0;

            _myLabel.Location =
                new Point(
                    _grid.RecordNavigationControl.NavigationBar.ButtonBarChild.Buttons[2].Bounds.X, 0);
            _myLabel.Width =
                _grid.RecordNavigationControl.NavigationBar.ButtonBarChild.Buttons[5].Bounds.X -
                _myLabel.Location.X;
            _myLabel.TextAlign = ContentAlignment.MiddleCenter;
            _grid.RecordNavigationControl.NavigationBar.Controls.Clear();
            _grid.RecordNavigationControl.NavigationBar.Controls.Add(_myLabel);

            _engine = new Engine();
            _engine.SetSourceList(_dt.DefaultView);
            _engine.SourceListChanged += new EventHandler(_engine_SourceListChanged);
            _model = new GridTableFilterBarGridListCellModelExt(_grid.TableModel, _engine);
            _grid.TableModel.CellModels["FilterBarCell"] = _model;
            _grid.ShowNavigationBar = true;
            _grid.RecordNavigationBar.AllowAddNew = false;
            _grid.RecordNavigationBar.AllowStepIncrease = false;
            _grid.RecordNavigationBar.ButtonLook = ButtonLook.Flat;
            _grid.RecordNavigationBar.DisplayArrowButtons = DisplayArrowButtons.All;
            _grid.RecordNavigationBar.CurrentRecordChanging += RecordNavigationBar_CurrentRecordChanging;
            _grid.RecordNavigationBar.ArrowButtonClicked += RecordNavigationBar_ArrowButtonClicked;
            _grid.FilterBarSelectedItemChanging += _grid_FilterBarSelectedItemChanging;
            FillPage(_dt);
        }

        /// <summary>
        /// It occurs when source list of engine is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">EventArgs of source list changed</param>
        void _engine_SourceListChanged(object sender, EventArgs e)
        {
            _grid.Table.TableDirty = true;
        }

        /// <summary>
        /// A method that fills the temporary table with paged data to display in grid.
        /// </summary>
        /// <param name="table">DataTable with latest view applied.</param>
        private void FillPage(DataTable table)
        {
            int i;
            int end;
            DataTable temp = table.Clone();
            _pageSize = this.PageSize;
            if (_currentPage == _pageCount)
                end = _maxRec;
            else
                end = _pageSize * _currentPage;

            int start = _recNo;

            //Copy rows from the source table to fill the temporary table.
            for (i = start; i < end; i++)
            {
                temp.ImportRow(table.Rows[i]);
                _recNo++;
            }
            _grid.DataSource = temp;
            if (_grid.Table != null && _grid.Table.Records != null)
                _grid.Table.Records[0].SetCurrent(table.Columns[0].ColumnName);
            _grid.Table.RecordValueChanging += new RecordValueChangingEventHandler(Table_RecordValueChanging);
            DisplayPageInfo();
        }

        /// <summary>
        /// An event to set the currentCell to record
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An RecordValueChangingEventArgs which is contains the event data.</param>
        void Table_RecordValueChanging(object sender, RecordValueChangingEventArgs e)
        {
            GridTextBoxCellRenderer rend = this._grid.TableControl.CurrentCell.Renderer as GridTextBoxCellRenderer;
            e.NewValue=rend.ControlText;
        }
        
        /// <summary>
        /// A method to display the page information in pager control at bottom.
        /// </summary>
        private void DisplayPageInfo()
        {
            _myLabel.Text = "Page " + _currentPage.ToString(CultureInfo.InvariantCulture) + " of " + _pageCount.ToString(CultureInfo.InvariantCulture);
            ArrowType arrowType = ArrowType.None;
            if (_currentPage != 1)
                arrowType |= ArrowType.First;
            if (_currentPage > 1)
                arrowType |= ArrowType.Previous;
            if (_currentPage < _pageCount)
                arrowType |= ArrowType.Next;
            if (_currentPage != _pageCount)
                arrowType |= ArrowType.Last;
            _grid.RecordNavigationBar.EnableButtonFlags = arrowType;
        }

        /// <summary>
        /// A method that get triggered on current record change in grid.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Syncfusion.Windows.Forms.CurrentRecordEventArgs">CurrentRecordEventArgs</see> that contains the event data.</param>
        static void RecordNavigationBar_CurrentRecordChanging(object sender, CurrentRecordEventArgs e)
        {
            e.Cancel = true;
        }

        /// <summary>
        /// A method that get invoked when filter is applied through the filter bar.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Syncfusion.Windows.Forms.Grid.Grouping.FilterBarSelectedItemChangingEventArgs">FilterBarSelectedItemChangingEventArgs</see> that contains the event data.</param>
        /// <remarks>Filter is applied to the Engine from which the filter view is set to the temporary data table.</remarks>
        void _grid_FilterBarSelectedItemChanging(object sender, FilterBarSelectedItemChangingEventArgs e)
        {
            if (e.SelectedIndex == 0)
            {
                e.Cancel = true;
                _model.SelectAll();
            }
            else if (e.SelectedIndex > 1)
            {
                e.Cancel = true;
                _model.SelectItem(e.SelectedIndex - 2);
            }
            _model.Apply();

            DataView dv = _dt.DefaultView;
            int count = _engine.TableDescriptor.RecordFilters.Count;
            string filterExpression = string.Empty;
            foreach (RecordFilterDescriptor desc in _engine.TableDescriptor.RecordFilters)
            {
                int condCount = desc.Conditions.Count;
                filterExpression += desc.FieldDescriptor.MappingName + "=";
                foreach (FilterCondition condition in desc.Conditions)
                {
                    filterExpression += "'" + condition.CompareText + "'";
                    condCount--;
                    if (condCount != 0)
                        filterExpression += condition.CompareOperator.ToString() + desc.FieldDescriptor.MappingName + "=";
                }
                count--;
                if (count != 0)
                    filterExpression += " and ";
            }
            if (dv != null)
            {
                dv.RowFilter = filterExpression;
                _currentPage = 1;
                _recNo = 0;
                _maxRec = _engine.Table.FilteredRecords.Count;
                _pageCount = _maxRec / _pageSize;

                //Adjust the page number if the last page contains a partial page.
                if ((_maxRec % _pageSize) > 0)
                {
                    _pageCount++;
                }
                FillPage(dv.ToTable());
            }
            Console.WriteLine("Filtered Records: " + _engine.Table.FilteredRecords.Count.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// A method that was invoked on clicking arrow buttons in pager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="T:Syncfusion.Windows.Forms.ArrowButtonEventArgs">ArrowButtonEventArgs</see> that contains the event data.</param>
        void RecordNavigationBar_ArrowButtonClicked(object sender, ArrowButtonEventArgs e)
        {
            switch (e.Arrow)
            {
                case ArrowType.First:
                    _currentPage = 1;
                    _recNo = 0;
                    FillPage(_dt.DefaultView.ToTable());
                    break;
                case ArrowType.Previous:
                    if (_currentPage == _pageCount)
                    {
                        _recNo = _pageSize * (_currentPage - 2);
                    }
                    _currentPage--;
                    _recNo = _pageSize * (_currentPage - 1);
                    FillPage(_dt.DefaultView.ToTable());
                    break;
                case ArrowType.Next:
                    _currentPage++;
                    if (_currentPage > _pageCount)
                    {
                        _currentPage = _pageCount;
                    }
                    FillPage(_dt.DefaultView.ToTable());
                    break;
                case ArrowType.Last:
                    _currentPage = _pageCount;
                    _recNo = _pageSize * (_currentPage - 1);
                    FillPage(_dt.DefaultView.ToTable());
                    break;
            }
        }

        /// <summary>
        /// used to convert the IEnumerable object to DataTable
        /// </summary>
        /// <param name="ien">IEnumerable DataSource</param>
        /// <returns>returns DataTable</returns>
        private DataTable ConvertDataTableFromIEnumerable(IEnumerable ien)
        {
            DataTable dt = new DataTable();
            foreach (object obj in ien)
            {
                Type t = null;
#if !SyncfusionFramework2_0 && !SyncfusionFramework3_5
                if (this._grid.Table.Engine.IsDynamicData)
                {
                    IDictionary<string, object> item = obj as IDictionary<string, object>;
                    if (item != null && item.Count == 0)
                    {
                        foreach (DynamicPropertyDescriptor info in this._grid.Table.TableDescriptor.ItemProperties)
                        {
                            item.Add(info.Name, string.Empty);
                        }
                    }
                    t = item.GetType();
                }
#else
                t = obj.GetType();
#endif
                PropertyInfo[] pis = t.GetProperties();
                if (dt.Columns.Count == 0)
                {
                    foreach (PropertyInfo pi in pis)
                    {
                        dt.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo pi in pis)
                {
                    object value = "";
                    value = pi.GetValue(obj, null);
                    dr[pi.Name] = value;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
    }
    #endregion

    #region Filter Model class - customized for paged grid filtering.
    /// <summary>
    /// A Filter model that applies filter to the Engine.
    /// </summary>
    /// <remarks>This helps to apply filtering across all records in every page.</remarks>
    public class GridTableFilterBarGridListCellModelExt : GridTableFilterBarGridListCellModel
    {
        readonly Engine _engine;
        RecordFilterDescriptorCollection _recordFilters;
        TableDescriptor _tableDescriptor;
        GridTableCellStyleInfoIdentity _identity;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:FilterBar.GridTableFilterBarGridListCellModelExt">GridTableFilterBarGridListCellModelExt</see> class. 
        /// </summary>
        /// <param name="grid">The Grid Model</param>
        /// <param name="engine">The Engine</param>
        public GridTableFilterBarGridListCellModelExt(GridModel grid, Engine engine)
            : base(grid)
        {
            _engine = engine;
        }

        /// <summary>
        /// A method that returns the filter values.
        /// </summary>
        /// <param name="column">Column in which filter is being applied.</param>
        /// <returns>Set of filter values.</returns>
        private object[] GetFilterValues(GridColumnDescriptor column)
        {
             SummaryDescriptor sd = new SummaryDescriptor(
                                column.Name + "FilterBarChoices",
                                column.MappingName,
                                FilterBarChoicesSummary.CreateSummaryMethod);
             sd.IgnoreRecordFilterCriteria = true;
            if (!_engine.Table.TableDescriptor.Summaries.Contains(sd.Name))
                _engine.Table.TableDescriptor.Summaries.Add(sd);
            ITreeTableSummary[] summaries = _engine.Table.GetSummaries();
            int summaryIndex = _engine.Table.TableDescriptor.Summaries.IndexOf(sd);
            FilterBarChoicesSummary filtersummary = (FilterBarChoicesSummary)summaries[summaryIndex];
            object[] values = filtersummary.Values;
            _engine.Table.TableDescriptor.Summaries.Remove(sd);
            _engine.Table.SummariesDirty = true;
            return values;
        }

        /// <summary>
        /// An overridden method that gets called when the filter dropdown is opened.
        /// </summary>
        /// <param name="tableCellIdentity">StyleInfoIdentity</param>
        /// <returns>Set of filter items.</returns>
        public override object[] GetFilterBarChoices(GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            _identity = tableCellIdentity;
            return GetFilterValues(_identity.Column);
        }

        /// <summary>
        /// A method that returns filter collection applied so far to the respective column.
        /// </summary>
        /// <param name="recordFilters">Filtercollection</param>
        /// <param name="tableCellIdentity">StyleInfoIdentity</param>
        /// <returns>Collection of filters.</returns>
        private IEnumerable<RecordFilterDescriptor> GetFilters(RecordFilterDescriptorCollection recordFilters, GridTableCellStyleInfoIdentity tableCellIdentity)
        {
            RecordFilterDescriptor[] rfdc = recordFilters.GetRecordFilters(tableCellIdentity.Column.MappingName);
            if (rfdc != null)
            {
                object[] uniqueId = tableCellIdentity.DisplayElement.ParentGroup.UniqueGroupId;
                ArrayList rfdList = new ArrayList();
                foreach (RecordFilterDescriptor rfd in rfdc)
                {
                    if (rfd.CompareUniqueId(uniqueId))
                    {
                        rfdList.Add(rfd);
                    }
                }

                if (rfdList.Count > 0)
                {
                    return (RecordFilterDescriptor[])rfdList.ToArray(typeof(RecordFilterDescriptor));
                }
            }
            return null;
        }

        /// <summary>
        /// A method that applies (All) type filtering to the column.
        /// </summary>
        /// <remarks>Resets the filter applied to that column.</remarks>
        public void SelectAll()
        {
            _tableDescriptor = _engine.TableDescriptor;//identity.Table.TableDescriptor;
            _recordFilters = new RecordFilterDescriptorCollection();
            _recordFilters.InitializeFrom(_tableDescriptor.RecordFilters);
            IEnumerable<RecordFilterDescriptor> removefdc = GetFilters(_recordFilters, _identity);
            if (removefdc == null) return;
            foreach (RecordFilterDescriptor rfd in removefdc)
            {
                _recordFilters.Remove(rfd);
            }
        }

        /// <summary>
        /// A method that applies specific item filtering to the column.
        /// </summary>
        /// <param name="index">selected index</param>
        public void SelectItem(int index)
        {
            string filterName = GetUniqueColumnGroupId(_identity);

            _tableDescriptor = _engine.TableDescriptor;//identity.Table.TableDescriptor;
            _recordFilters = new RecordFilterDescriptorCollection();
            _recordFilters.InitializeFrom(_tableDescriptor.RecordFilters);

            object[] items = GetFilterBarChoices(_identity);

            ////check to see if list was shortened after index was set
            if (index >= items.GetLength(0))
            {
                return;
            }

            object value = items[index];

            IEnumerable<RecordFilterDescriptor> removefdc = GetFilters(_recordFilters, _identity);
            if (removefdc != null)
            {
                foreach (RecordFilterDescriptor rfd in removefdc)
                {
                    _recordFilters.Remove(rfd);
                }
            }

            RecordFilterDescriptor newFilter = new RecordFilterDescriptor();
            newFilter.Name = filterName;
            newFilter.MappingName = _identity.Column.MappingName;
            newFilter.UniqueGroupId = this.GetUniqueGroupId(_identity);

            newFilter.Conditions.Add(new FilterCondition(FilterCompareOperator.Equals, value));
            _recordFilters.Add(newFilter);
        }

        /// <summary>
        /// Applies the filter to the Engine.
        /// </summary>
        public void Apply()
        {
            if (_tableDescriptor != null && _recordFilters != null)
            {
                _tableDescriptor.RecordFilters.InitializeFrom(_recordFilters);
            }
            _recordFilters = null;
        }
    }
#endregion
}
