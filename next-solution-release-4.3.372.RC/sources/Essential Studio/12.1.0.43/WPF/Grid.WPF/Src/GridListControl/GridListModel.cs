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
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using Syncfusion.Windows.Collections;
    using Syncfusion.Windows.GridCommon;
    using Syncfusion.Windows.Controls.Cells;
    using System.Windows.Controls;
    using Syncfusion.Linq;
	using System.Collections.Specialized;
    
    public class GridListModel : GridModel
    {
        #region Fields
        IEnumerable itemsSource;
        IList _sourceList;
        ICollectionView _collectionView;
        //ItemCollection items;
        string displayMember = string.Empty;
        string valueMember = string.Empty;
        PropertyDescriptorCollection _pdc;
        double defaultColumnWidth = 60;
        bool showRowHeaders = true;
        bool showColumnHeaders = true;
        bool autoPopulateDropDownColumns = true;
        GridControlLengthUnitType dropDownColumnSizer = GridControlLengthUnitType.Auto;
        GridListColumnsCollection dropDownVisibleColumns;
        GridListColumns columns;
        PropertyDescriptor displayPd;
        PropertyDescriptor valuePd;
        string name;
        #endregion

        #region Ctor
        public GridListModel()
        {
            columns = new GridListColumns(this);

            ColumnWidths = columns;

            Options.ShowCurrentCell = false;
            Options.ExcelLikeCurrentCell = false;
            Options.ExcelLikeSelectionFrame = false;
            Options.ListBoxSelectionMode = GridSelectionMode.One;
            Options.AllowSelection = GridSelectionFlags.Row | GridSelectionFlags.Multiple | GridSelectionFlags.Keyboard | GridSelectionFlags.Shift;
            TableStyle.CellType = "Static";
            this.HeaderColumns = 0;
        }
        #endregion

        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        GridSelectionMode SelectionMode
        {
            get
            {
                return Options.ListBoxSelectionMode;
            }
            set
            {
                Options.ListBoxSelectionMode = value;
            }
        }

        public double DefaultColumnWidth
        {
            get { return defaultColumnWidth; }
            set { defaultColumnWidth = value; }
        }

        public bool ShowRowHeaders
        {
            get { return showRowHeaders; }
            set { showRowHeaders = value; }
        }

        public bool AutoPopulateDropDownColumns
        {
            get { return autoPopulateDropDownColumns; }
            set { autoPopulateDropDownColumns = value; }
        }
        public GridControlLengthUnitType DropDownColumnSizer
        {
            get { return dropDownColumnSizer; }
            set { dropDownColumnSizer = value; }
        }
        public GridListColumnsCollection DropDownVisibleColumns
        {
            get { return dropDownVisibleColumns; }
            set { dropDownVisibleColumns = value; }
        }
        public bool ShowColumnHeaders
        {
            get { return showColumnHeaders; }
            set { showColumnHeaders = value; }
        }

        #endregion

        #region Columns, ItemsSource

        /// <summary>
        /// Gets or sets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public GridListColumns Columns
        {
            get { return columns; }
            set
            {
                if (columns != value)
                {
                    columns = new GridListColumns(this);
                    if (value != null)
                    {
                        foreach (GridListColumn column in value)
                            columns.Add(column);
                    }
                }
            }
        }



        // Implement Listbox-like derived control with ItemSource, DisplayMember and ValueMember
        [BindableAttribute(true)]
        public IEnumerable ItemsSource
        {
            get { return itemsSource; }
            set
            {
                if (itemsSource != value)
                {
                    if (itemsSource != null)
                        UnwireItemsSource();
                    itemsSource = value;
                    if (itemsSource != null)
                        WireItemsSource();
                    OnItemsSourceChanged();
                    //if (value != null)
                    //    items = null;
                    //else if (items == null)
                    //    items = new ItemCollection();
                }
            }
        }

        public event EventHandler ItemsSourceChanged;

        private void OnItemsSourceChanged()
        {
            if (ItemsSourceChanged != null)
                ItemsSourceChanged(this, EventArgs.Empty);
        }

        protected virtual void WireItemsSource()
        {
            if (itemsSource is IList)
                SourceList = (IList)itemsSource;
            else
            {
                EnumarableWrapperList list = new EnumarableWrapperList(itemsSource);
                SourceList = list;
            }

            if (itemsSource is ICollectionView)
            {
                _collectionView = ((ICollectionView)itemsSource);
                _collectionView.CurrentChanging += new CurrentChangingEventHandler(_collectionView_CurrentChanging);
                _collectionView.CurrentChanged += new EventHandler(_collectionView_CurrentChanged);
                _collectionView.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_collectionView_CollectionChanged);
            }
        }

        void _collectionView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SelectedRanges.Clear();
            foreach (GridControlBase view in Views)
            {
                view.CurrentCell.Deactivate(true);
            }

            IEnumerable t = ItemsSource;
            ItemsSource = null;
            ItemsSource = t;
            VolatileCellStyles.Clear();
            foreach (GridControlBase view in Views)
            {
                view.InvalidateCell(GridRangeInfo.Table());
                view.InvalidateVisual();
            }
            if (_collectionView != null)
                CurrentIndex = _collectionView.CurrentPosition;
        }

        void _collectionView_CurrentChanged(object sender, EventArgs e)
        {
            CurrentIndex = _collectionView.CurrentPosition;
        }

        void _collectionView_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
        }

        protected virtual void UnwireItemsSource()
        {
            SourceList = null;
            if (_collectionView != null)
            {
                _collectionView.CurrentChanging -= new CurrentChangingEventHandler(_collectionView_CurrentChanging);
                _collectionView.CurrentChanged -= new EventHandler(_collectionView_CurrentChanged);
                _collectionView.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_collectionView_CollectionChanged);
                _collectionView = null;
            }
        }
        #endregion

        #region SourceList, Wire/Unwire, Get Record Count
        public IList SourceList
        {
            get { return _sourceList; }
            set
            {
                if (!Object.ReferenceEquals(_sourceList, value))
                {
                    if (_sourceList != null)
                        UnwireSourceList();
                    _sourceList = value;
                    if (_sourceList != null)
                        WireSourceList();
                    OnSourceListChanged();
                }
            }
        }

        protected virtual void OnSourceListChanged()
        {
        }

        protected virtual void WireSourceList()
        {
            IBindingList bindingList = SourceList as IBindingList;
            if (bindingList != null)
                bindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);

            RowCount = SourceList.Count + 1 + FooterRows;

            _pdc = ListUtil.GetItemProperties(SourceList);
            //ColumnCount = _pdc.Count + 1;
            if (Columns.Count == 0 && AutoPopulateDropDownColumns)
            {
                foreach (PropertyDescriptor pd in _pdc)
                {
                    GridListColumn column = new GridListColumn();
                    column.MappingName = pd.Name;
                    column.Width = 60;
#if SyncfusionFramework4_0
                    var canAddColumn = !ListUtil.IsComplexType(pd) || typeof(byte[]).IsAssignableFrom(pd.PropertyType);
                    System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                    bool hasDisplayAttribute = false;
                    this.GetDisplayAttribute(pd, ref canAddColumn, ref hasDisplayAttribute, out displayAttribute);
                    if (canAddColumn)
#endif
                        Columns.Add(column);
                }
            }
            else if (!AutoPopulateDropDownColumns)
            {

                if (this.DropDownVisibleColumns == null ||(this.DropDownVisibleColumns!=null && this.DropDownVisibleColumns.Count==0))
                {
                    throw new InvalidOperationException();
                }
                foreach (PropertyDescriptor pd in _pdc)
                {
                    GridListColumn column = new GridListColumn();
                    foreach (var items in this.DropDownVisibleColumns)
                    {
                        if (pd.Name == items.MappingName)
                        {
                            column.MappingName = items.MappingName;
                            column.Width = (double.IsNaN(items.Width)) ? 60 : items.Width;
                            column.IsHidden = items.IsHidden;
                            column.CellStyle = items.CellStyle;
                            column.HeaderStyle = items.HeaderStyle;
                            var isHidden = !this.DropDownVisibleColumns.Contains(new GridListColumn() { MappingName = column.MappingName });
                            if (isHidden)
                            {
#if SyncfusionFramework4_0
                                var canAddColumn = !ListUtil.IsComplexType(pd) || typeof(byte[]).IsAssignableFrom(pd.PropertyType);
                                System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute = null;
                                bool hasDisplayAttribute = false;
                                this.GetDisplayAttribute(pd, ref canAddColumn, ref hasDisplayAttribute, out displayAttribute);
                                if (canAddColumn)
                                {
#endif
                                    Columns.Add(column);
                                    break;
#if SyncfusionFramework4_0
                                }
#endif
                            }
                        }
                    }
                }
            }
            // Check if it is a DataTable.
            //_dt = GetDataTable(this.SourceList);
            //WireDataTable(_dt);

            //this._dataSourceRaisesTwoItemAddedEvents = _dt != null;
        }

#if SyncfusionFramework4_0
#if !SILVERLIGHT
        private void GetDisplayAttribute(PropertyDescriptor pd, ref bool canAddColumn, ref bool hasDisplayAttribute, out System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute)
        {
            var attr = pd.Attributes.ToList<Attribute>().FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute));
#else
        private void GetDisplayAttribute(PropertyInfo pd, ref bool canAddColumn, ref bool hasDisplayAttribute, out System.ComponentModel.DataAnnotations.DisplayAttribute displayAttribute)
        {
            var attr = (System.ComponentModel.DataAnnotations.DisplayAttribute)pd.GetCustomAttributes(false).FirstOrDefault(a => a.GetType() == typeof(System.ComponentModel.DataAnnotations.DisplayAttribute)); //xPd.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false);
#endif

            displayAttribute = null;
            if (attr != null)
            {
                displayAttribute = attr as System.ComponentModel.DataAnnotations.DisplayAttribute;
                if (!hasDisplayAttribute)
                {
                    hasDisplayAttribute = true;
                }

                if (displayAttribute.GetAutoGenerateField() != null)
                {
                    canAddColumn = displayAttribute.AutoGenerateField;
                }
            }

        }
#endif

        private int _lastAddNewIndex = -1;
        void bindingList_ListChanged(object sender, ListChangedEventArgs e)
        {
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemChanged:
                    // DataView raises a ItemAdded with subsequent ItemChanged and ItemMoved event when data view is sorted
                    // Skip the ItemChanged notification in such case.
                    if (_lastAddNewIndex == -1 || _lastAddNewIndex != e.NewIndex)
                        OnSourceListItemChanged(e);
                    _lastAddNewIndex = -1;
                    break;

                case ListChangedType.ItemAdded:
                    this._lastAddNewIndex = -1;
                    OnSourceListItemAdded(e);

                    // DataView raises a ItemAdded with subsequent ItemMoved event when data view is sorted.
                    // The lastAddNewIndex will be checked in the OnSourceListItemChanged and ItemMoved
                    // handler so that the subsequents events can be ignored.
                    this._lastAddNewIndex = e.NewIndex;
                    break;

                case ListChangedType.ItemDeleted:
                    this._lastAddNewIndex = -1;
                    OnSourceListItemDeleted(e);
                    break;

                case ListChangedType.ItemMoved:
                    // DataView raises a ItemAdded with subsequent ItemChanged and ItemMoved event when data view is sorted
                    // In the ItemAdded event the e.NewIndex already had the correct index and was
                    // inserted at the right position. Doing it here again would mess up 
                    // order. Therefore skip the ItemMoved notification.
                    if (_lastAddNewIndex != e.NewIndex)
                        OnSourceListItemMoved(e);
                    _lastAddNewIndex = -1;
                    break;

                case ListChangedType.Reset:
                    OnSourceListReset(e);
                    break;
            }
        }

        private void OnSourceListReset(ListChangedEventArgs e)
        {
            // Force getting new PropertDescriptorCollection and refresh everything
            IEnumerable list = this.ItemsSource;
            this.ItemsSource = null;
            this.ItemsSource = list;
            InvalidateVisual(true);
            this._lastAddNewIndex = -1;
        }

        private void OnSourceListItemMoved(ListChangedEventArgs e)
        {
            this.MoveRows(e.OldIndex + 1, 1, e.NewIndex + 1);
            InvalidateVisual(true);
            OnSourceListItemChanged(e);
        }

        private void OnSourceListItemDeleted(ListChangedEventArgs e)
        {
            int rowIndex = e.NewIndex + 1;
            this.RemoveRows(rowIndex, 1);
            InvalidateVisual(true);
        }

        private void OnSourceListItemAdded(ListChangedEventArgs e)
        {
            int rowIndex = e.NewIndex + 1;
            this.InsertRows(rowIndex, 1);
            object item = SourceList[e.NewIndex];
            InvalidateVisual(true);
        }

        private void OnSourceListItemChanged(ListChangedEventArgs e)
        {
            int rowIndex = e.NewIndex + 1;
            this.InvalidateCell(GridRangeInfo.Row(rowIndex));
        }

        protected virtual void UnwireSourceList()
        {
            IBindingList bindingList = SourceList as IBindingList;
            if (bindingList != null)
                bindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
            //UnwireDataTable(_dt);
            RowCount = 1;
            ColumnCount = 1;
        }

        /// <summary>
        /// Gets or sets the collection of PropertyDescriptors
        /// of the underlying data source. <see cref="TreeColumn.MappingName"/> identifies properties
        /// in this collection so that cells can display values from the <see cref="TreeNode.Data"/>
        /// of a <see cref="TreeNode"/>.
        /// </summary>
        /// <value>The node item properties.</value>
        public PropertyDescriptorCollection ItemProperties
        {
            get { return _pdc; }
            set { _pdc = value; }
        }

        #endregion

        #region ColumnChanging Event - Not implemented
#if later
        // Allow this table to listen to DataTable.ColumnChanging events. When
        // a ColumnChanging event is handled the table will automatically add ChangedFieldInfo
        // objects with information about the new and old value of the column. The
        // ChangedFieldInfo objects will then be checked in the ListChanged event handler.

        DataTable _dt;
        bool _dataSourceRaisesTwoItemAddedEvents;

        void WireDataTable(DataTable dt)
        {
            if (dt == null)
                return;

            dt.ColumnChanging += new DataColumnChangeEventHandler(dt_ColumnChanging);
            dt.RowDeleting += new DataRowChangeEventHandler(dt_RowDeleting);
        }

        void UnwireDataTable(DataTable dt)
        {
            if (dt == null)
                return;

            dt.ColumnChanging -= new DataColumnChangeEventHandler(dt_ColumnChanging);
            dt.RowDeleting -= new DataRowChangeEventHandler(dt_RowDeleting);
        }

        DataTable GetDataTable(object datasource)
        {
            if (datasource is DataTable)
                return (DataTable)datasource;

            else if (datasource is DataView)
                return ((DataView)datasource).Table;

            return null;
        }

        private void dt_ColumnChanging(object sender, DataColumnChangeEventArgs e)
        {
            string name = e.Column.ColumnName;
            ChangedFieldInfo ci = new ChangedFieldInfo(_pdc, name, e.Row[e.Column], e.ProposedValue);
            AddChangedField(ci);
            //Console.WriteLine("{0} : {1} -> {2} ({3})", ci.Name, ci.OldValue, ci.NewValue, e.Row[0]);
        }
        protected virtual void dt_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            PrepareRemoving(e.Row);
        }

        void PrepareRemoving(object row)
        {
            OnPrepareRemoving(row);
        }

        /// <summary>
        /// This virtual method gets called before a row is removed from
        /// the underlying datasource.
        /// </summary>
        /// <param name="row"></param>
        protected virtual void OnPrepareRemoving(object row)
        {
            for (int n = 0; n < _pdc.Count; n++)
            {
                PropertyDescriptor pd = _pdc[n];
                if (pd.PropertyType == typeof(double))
                {
                    object value = GetValue(row, pd);
                    ChangedFieldInfo ci = new ChangedFieldInfo(_pdc, pd.Name, value, null);
                    this.AddChangedField(ci);
                }
            }
        }

        void PrepareItemAdded(object row)
        {
            OnPrepareItemAdded(row);
        }

        /// <summary>
        /// This virtual method is called when a row was added in the underlying datasource.
        /// </summary>
        /// <param name="row"></param>
        protected virtual void OnPrepareItemAdded(object row)
        {
            //// Get new values for which delta information is needed
        }

        /// <summary>
        /// A helper method that calls pd.GetValue(row) or gets the value directly
        /// from a DataRow using its name.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="pd"></param>
        /// <returns></returns>
        public static object GetValue(object row, PropertyDescriptor pd)
        {
            if (row is DataRow)
            {
                // PropertyDescriptor is for the DataRowView - but here
                // we only have access to the DataRow.
                return ((DataRow)row)[pd.Name];
            }
            else
            {
                return pd.GetValue(row);
            }
        }
#endif
        #endregion

        #region Changed Fields- Not implemented
#if later
        Hashtable _changedFields = new Hashtable();
        ChangedFieldInfoCollection _changedFieldsArray = new ChangedFieldInfoCollection();

        /// <summary>
        /// The Collection with detected changes in the datasource when a ListChanged event
        /// is handled.
        /// </summary>
        public ChangedFieldInfoCollection ChangedFieldsArray
        {
            get
            {
                return _changedFieldsArray;
            }
        }

        public void ClearChangedFields()
        {
            _changedFields.Clear();
            _changedFieldsArray.Clear();
        }

        /// <summary>
        /// Call this method to add ChangedFieldInfo
        /// objects with information about the new and old value of the column. The
        /// ChangedFieldInfo objects will then be checked in the LIstChanged event handler.
        /// </summary>
        public void AddChangedField(ChangedFieldInfo ci)
        {
            if (!_changedFields.Contains(ci.FieldIndex))
            {
                _changedFields[ci.FieldIndex] = ci;
                _changedFieldsArray.Add(ci);
                //foreach (int dependantIndex in GetDependantFields(ci.FieldIndex))
                //{
                //    if (!changedFields.Contains(dependantIndex))
                //        AddChangedField(new ChangedFieldInfo(properties, properties[dependantIndex].Name));
                //}
            }
            else
            {
                // Combine ChangeField
                ChangedFieldInfo ci0 = (ChangedFieldInfo)_changedFields[ci.FieldIndex];
                ci0.NewValue = ci.NewValue;
            }
        }

        /// <summary>
        /// The Collection with detected changes in the datasource when a ListChanged event
        /// is handled.
        /// </summary>
        /// <returns></returns>
        public ChangedFieldInfoCollection GetChangedFields()
        {
            return _changedFieldsArray;
        }
#endif
        #endregion

        #region DisplayMember, ValueMember, Search and Index methods.
        //[BindableAttribute(true)]
        //public ItemCollection Items 
        //{
        //    get { return items; }
        //}

        [BindableAttribute(true)]
        public string DisplayMember
        {
            get { return displayMember; }
            set
            {
                displayMember = value;
                displayPd = null;
            }
        }

        [BindableAttribute(true)]
        public string ValueMember
        {
            get { return valueMember; }
            set
            {
                valueMember = value;
                valuePd = null;
            }
        }


        PropertyDescriptor DisplayPropertyDescriptor
        {
            get
            {
                if (displayPd == null)
                {
                    if (displayMember != "")
                        displayPd = ItemProperties[displayMember];
                }
                return displayPd;
            }
        }

        PropertyDescriptor ValuePropertyDescriptor
        {
            get
            {
                if (valuePd == null)
                {
                    if (valueMember != "")
                        valuePd = ItemProperties[valueMember];
                }
                return valuePd;
            }
        }



        // Text for current selected element. If no DisplayMember
        // is set then this is item.ToString().
        public virtual string CurrentDisplayText
        {
            get
            {
                if (CurrentIndex == -1)
                    return "";
                return GetDisplayText(CurrentItem);
            }
        }

        // UIElement for current selected element (could be composite element).
        // If no DisplayMember is set then this is null.
        public virtual DependencyObject CurrentDisplayElement
        {
            get
            {
                if (CurrentIndex == -1)
                    return null;
                return GetDisplayElement(CurrentItem);
            }
        }

        // The key for the current value if DisplayMember is set
        // or the item itsself if DisplayMember is empty.
        public virtual object CurrentValue
        {
            get
            {
                if (CurrentIndex == -1)
                    return null;
                return GetValue(CurrentItem);
            }
        }

        public virtual string GetDisplayText(int index)
        {
            if (index == -1 || index > this.SourceList.Count)
            {
                return string.Empty;
            }
            if (this.DisplayMember != string.Empty)
            {
                return this.GetDisplayText(this.GetItem(index));
            }
            // sometimes the value can be null in the list
            return this.SourceList[index] != null ? this.SourceList[index].ToString() : string.Empty;
        }

        public virtual string GetDisplayText(object item)
        {
            if (item == null)
                return string.Empty;

            var shouldGetValue=true;

            if(item.GetType() == typeof(System.DBNull))
            {
                shouldGetValue = false;
            }

            if (shouldGetValue && DisplayMember != string.Empty && DisplayPropertyDescriptor != null && DisplayPropertyDescriptor.GetValue(item) != null)
            {
                return DisplayPropertyDescriptor.GetValue(item).ToString();
            }

            return item != null ? item.ToString() : string.Empty;
        }

        public virtual DependencyObject GetDisplayElement(object item)
        {
            return null;
        }

        public virtual object GetValue(object item)
        {
            if (item == null)
                return null;

            if (ValueMember != "")
                return ValuePropertyDescriptor.GetValue(item);

            if (DisplayMember != string.Empty)
            {
                return DisplayPropertyDescriptor.GetValue(item);
            }

            return item;
        }

        public object GetDisplayValue(object item)
        {
            if (item == null)
                return null;

            if (DisplayMember != string.Empty)
            {
                return DisplayPropertyDescriptor.GetValue(item);
            }

            return item;
        }

        public virtual int IndexOfItemByDisplayText(string text)
        {
            if (SourceList == null)
                return -1;

            for (int n = 0; n < SourceList.Count; n++)
            {
                if (text == GetDisplayText(SourceList[n]))
                    return n;
            }

            return -1;
        }

        public virtual int IndexOfItemByDisplayTextMatch(string text, bool ignoreCase, int startSearchAtIndex)
        {
            if (SourceList == null)
                return -1;

            if (ignoreCase)
                text = text.ToUpper();

            startSearchAtIndex = Math.Max(0, startSearchAtIndex);

            for (int n = 0; n < SourceList.Count; n++)
            {
                int index = startSearchAtIndex + n % SourceList.Count;
                string displayText = GetDisplayText(SourceList[index]);
                if (ignoreCase)
                    displayText = displayText.ToUpper();

                if (displayText.StartsWith(text))
                    return index;
            }

            return -1;
        }

        public virtual int IndexOfItemByValue(object searchValue)
        {
            if (SourceList == null)
                return -1;

            for (int n = 0; n < SourceList.Count; n++)
            {
                object itemValue = GetValue(GetItem(n));
                if (Object.ReferenceEquals(searchValue, itemValue)
                    || itemValue != null && itemValue.Equals(searchValue)
                    ||itemValue != null && searchValue != null && itemValue.ToString().Equals(searchValue.ToString()))
                    return n;
            }

            return -1;
        }

        private int IndexOfItem(object searchItem)
        {
            if (SourceList == null)
                return -1;

            for (int n = 0; n < SourceList.Count; n++)
            {
                object item = GetItem(n);
                if (Object.ReferenceEquals(searchItem, item))
                    return n;
            }

            return -1;
        }

        public virtual object GetItem(int index)
        {
            return index < this.SourceList.Count ? SourceList[index] : null;
        }

        public virtual object CurrentItem
        {
            get
            {
                int index = CurrentIndex;
                if (index == -1)
                    return null;
                else
                    return GetItem(index);
            }
            set
            {
                if (value != CurrentItem)
                {
                    int index = IndexOfItem(value);
                    CurrentIndex = index;
                }
            }
        }

        private int currentIndex = -1;
        public virtual int CurrentIndex
        {
            get
            {
                //if (CurrentCellState.IsEmpty)
                //{
                //    return this.currentIndex;
                //}
                //else if (!CurrentCellState.IsEmpty && this.Views.GetEnumerator().MoveNext())
                //{
                //    return CurrentCellState.RowIndex - 1;
                //}
                //return -1;
                return this.currentIndex;
            }
            set
            {
                if (value != this.currentIndex)
                {
                    foreach (GridControlBase grid in Views)
                    {
                        if (value == -1)
                            grid.CurrentCell.Deactivate();
                        else
                            grid.CurrentCell.MoveTo(value + 1, Math.Max(0, grid.CurrentCell.ColumnIndex));
                    }
                    this.currentIndex = value;
                    if (this.CurrentIndexChanged != null)
                    {
                        this.CurrentIndexChanged(this, EventArgs.Empty);
                    }
                }
                if (_collectionView != null)
                    _collectionView.MoveCurrentToPosition(value);
            }
        }

        public event EventHandler CurrentIndexChanged;

        #endregion

        #region Selection

        public virtual bool IsSelected(int index)
        {
            return !CurrentCellState.IsEmpty && CurrentCellState.RowIndex == index + 1; ;
        }

        public virtual void SetSelected(int index, bool selectState)
        {
            if (selectState)
            {
                foreach (GridControlBase grid in Views)
                {
                    grid.CurrentCell.MoveTo(index + 1, grid.CurrentCell.ColumnIndex);
                }
            }
            else if (IsSelected(index))
            {
                foreach (GridControlBase grid in Views)
                {
                    grid.CurrentCell.Deactivate();
                }
            }
        }

        public virtual void SetSelected(int from, int to, bool selectState)
        {
            SetSelected(from, selectState);
        }

        // Keep things in sync with SelectedRanges so that GridSelectCellsMouseController can
        // be reused. But modify/override RenderSelectedCells to
        // loop through visible rows and draw them instead of looping through Selected Ranges.
        // This way a selected row is only rendered once.

        public IList<int> GetSelectedList()
        {
            IList<int> selectedList = new List<int>();
            if (CurrentIndex != -1)
                selectedList.Add(CurrentIndex);
            return selectedList;
        }


        #endregion

        #region Bind field values from List to Cells

        // very simple binding to display data from list in cells.
        protected override void OnQueryCellInfo(GridQueryCellInfoEventArgs e)
        {
            if (e.Handled) return;

            GridListColumn column = Columns[e.Cell.ColumnIndex];

            if (e.Cell.RowIndex == 0)
            {
                e.Style.ModifyStyle(column.HeaderStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
            }
            else if (SourceList != null && e.Cell.RowIndex - 1 < SourceList.Count)
            {
                e.Style.ModifyStyle(column.CellStyle, Syncfusion.Windows.Styles.StyleModifyType.Override);
            }

            PropertyDescriptor pd = null;
            if (column.MappingName != null && ItemProperties != null)
                pd = ItemProperties[column.MappingName];

            if (pd != null)
            {
                if (e.Cell.RowIndex == 0)
                {
                    e.Style.CellValue = pd.DisplayName;
                }
                else if (e.Cell.RowIndex - 1 < SourceList.Count)
                {
                    object item = SourceList[e.Cell.RowIndex - 1];
                    object value = pd.GetValue(item);
                    e.Style.CellValueType = pd.PropertyType;
                    e.Style.CellValue = value;
                }
            }
        }

        protected override void OnCommitCellInfo(GridCommitCellInfoEventArgs e)
        {
            if (e.Handled) return;

            GridListColumn column = Columns[e.Cell.ColumnIndex];

            PropertyDescriptor pd = null;
            if (column.MappingName != null && ItemProperties != null)
                pd = ItemProperties[column.MappingName];

            if (pd != null)
            {
                if (e.Cell.RowIndex > 0)
                {
                    if (e.Style.Store.IsValueModified(GridStyleInfoStore.CellValueProperty)
                        || e.Sip == GridStyleInfoStore.CellValueProperty)
                    {
                        object item = SourceList[e.Cell.RowIndex - 1];
                        object value = e.Style.CellValue;
                        pd.SetValue(item, value);
                    }
                }
            }
        }

        #endregion

        public void MoveDown()
        {
            foreach (var grid in this.Views)
            {
                if (grid.CurrentCell.RowIndex < 0 && grid.CurrentCell.ColumnIndex < 0)
                {
                    grid.CurrentCell.MoveTo(1, 0);
                }
                else
                {
                    grid.CurrentCell.MoveDown();
                }
            }
        }

        public void MoveUp()
        {
            foreach (var grid in this.Views)
            {
                if (grid.CurrentCell.RowIndex < 0 && grid.CurrentCell.ColumnIndex < 0)
                {
                    var lastIndex = this.RowCount - this.HeaderRows;
                    grid.CurrentCell.MoveTo(lastIndex, 0);
                }
                else
                {
                    grid.CurrentCell.MoveUp();
                }
            }
        }

        public void MoveToTop()
        {
            foreach (var grid in this.Views)
            {
                if (grid.CurrentCell.RowIndex < 0 && grid.CurrentCell.ColumnIndex < 0)
                {
                    grid.CurrentCell.MoveTo(1, 0);
                }
                else
                {
                    grid.CurrentCell.MoveToTop();
                }
            }
        }

        public void MoveToBottom()
        {
            foreach (var grid in this.Views)
            {
                if (grid.CurrentCell.RowIndex < 0 && grid.CurrentCell.ColumnIndex < 0)
                {
                    var lastIndex = this.RowCount - this.HeaderRows;
                    grid.CurrentCell.MoveTo(lastIndex, 0);
                }
                else
                {
                    grid.CurrentCell.MoveToBottom();
                }
            }
        }

        public void PageUp()
        {
            foreach (var grid in this.Views)
            {
                grid.CurrentCell.PageUp();
            }
        }

        public void PageDown()
        {
            foreach (var grid in this.Views)
            {
                grid.CurrentCell.PageDown();
            }
        }
    }

    #region ChangedFieldInfoCollection - not implemented.
#if Later
    /// <summary>
    /// The Collection with detected changes in the datasource when a ListChanged event
    /// is handled.
    /// </summary>
    /// <returns></returns>
    public class ChangedFieldInfoCollection : List<ChangedFieldInfo>
    {
    }


    /// <summary>
    /// Provides details about the changes made to a column at the time
    /// the ListChanged event is handled in the engine. 
    /// </summary>
    public class ChangedFieldInfo
    {
        PropertyDescriptorCollection pdc;
        string name;
        object oldValue;
        object newValue;
        int fieldIndex = -1;
        bool hasValue = false;
        double delta;
        bool hasDelta = false;

        public ChangedFieldInfo(PropertyDescriptorCollection pdc, string name)
        {
            this.name = name;
            this.pdc = pdc;
            this.hasValue = false;
        }

        public ChangedFieldInfo(PropertyDescriptorCollection pdc, string name, object oldValue, object newValue)
        {
            this.name = name;
            this.pdc = pdc;
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.hasValue = true;
        }

        public PropertyDescriptorCollection Properties
        {
            get
            {
                return pdc;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public object OldValue
        {
            get
            {
                return oldValue;
            }
            set
            {
                oldValue = value;
                hasValue = true;
            }
        }

        public object NewValue
        {
            get
            {
                return newValue;
            }
            set
            {
                newValue = value;
                hasValue = true;
            }
        }

        public bool HasValue
        {
            get
            {
                return hasValue;
            }
        }

        public void SetValues(object oldValue, object newValue)
        {
            this.oldValue = oldValue;
            this.newValue = newValue;
            this.hasValue = true;
        }

        public double Delta
        {
            get
            {
                if (!hasDelta)
                {
                    object _newValue = newValue;
                    object _oldValue = oldValue;
                    if (newValue == null || newValue is DBNull)
                        _newValue = 0;
                    if (oldValue == null || oldValue is DBNull)
                        _oldValue = 0;
                    delta = Convert.ToDouble(_newValue) - Convert.ToDouble(_oldValue);
                    hasDelta = true;
                }
                return delta;
            }
        }

        public int FieldIndex
        {
            get
            {
                if (fieldIndex == -1)
                    fieldIndex = Properties.IndexOf(Properties[Name]);
                return fieldIndex;
            }
        }
    }
#endif
    #endregion

    public class GridListColumnsCollection : List<GridListColumn>
    {
    }

}