#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.Collections;
using Syncfusion.Silverlight.Controls.PivotSchemaDesigner;
using System.Collections.ObjectModel;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using Syncfusion.Windows.Shared.Controls;
using System.Collections.Generic;

namespace Syncfusion.Silverlight.Controls.PivotGrid
{
#if SyncfusionFramework4_0
    [System.ComponentModel.DesignTimeVisible(false)]
#endif
    public partial class PivotFieldListWindow : Syncfusion.Windows.Tools.Controls.WindowControl
    {
        #region Private Variable

        private bool showAllFields = true;

        private bool showSelectedFields;

        public bool ShowSelectedFields
        {
            get { return showSelectedFields; }
            set 
            { 
                showSelectedFields = value;
                this.PivotTableFields.Clear();
                InitilizePivotTableList();
            }
        }


        private bool resumeProcessing = false;

        #endregion

        #region Internal Property
        internal PivotGridControl GridControl
        {
            get { return (PivotGridControl)GetValue(GridControlProperty); }
            set { SetValue(GridControlProperty, value); }
        }

        public ObservableCollection<PivotTableField> PivotTableFields { get; set; }

        public bool ShowAllFields
        {
            get { return showAllFields; }
            set
            {
                showAllFields = value;
                if (!value)
                {
                    Part_PivotTableFields.ItemsSource = this.PivotTableFields.Where(i => i.IsSelected == false).Select(i => i);
                }
                else
                    Part_PivotTableFields.ItemsSource = this.PivotTableFields;
            }
        }

        #endregion

        #region Dependency property decleration

        // Using a DependencyProperty as the backing store for PivotControl.  This enables animation, styling, binding, etc...
        internal static readonly DependencyProperty GridControlProperty =
            DependencyProperty.Register("GridControl", typeof(PivotGridControl), typeof(PivotFieldListWindow), new PropertyMetadata(null));
        #endregion

        #region Constructor
        public PivotFieldListWindow()
        {
            InitializeComponent();
        }

        public PivotFieldListWindow(PivotGridControl gridControl)
        {
            InitializeComponent();
            this.IconSize = new Size(0, 0);
            this.GridControl = gridControl;
            this.PivotTableFields = new ObservableCollection<PivotTableField>();
            this.DataContext = this;
            MoveDown.IsEnabled = MoveUp.IsEnabled = this.Part_PivotTableFields.SelectedItem == null ? false : true;
            //InitilizePivotTableList();
            WireEvents();
        }
        #endregion

        #region Methods

        private void WireEvents()
        {
            this.Loaded += new RoutedEventHandler(PivotFieldListWindow_Loaded);
            this.Closed += new Windows.Tools.Controls.ClosedEventHandler(PivotFieldListWindow_Closed);
            this.GridControl.PivotRows.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItems_CollectionChanged);
            this.GridControl.PivotColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotItems_CollectionChanged);
            this.GridControl.PivotCalculations.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotCalculations_CollectionChanged);
            this.GridControl.GroupingBar.Filters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Filters_CollectionChanged);
            DragAndDropManager.Drop += new DragDropEventHandler(DragAndDropManager_Drop);
        }


        void DragAndDropManager_Drop(object sender, DragDropEventArgs args)
        {
            if (this.GridControl.GroupingBar.DropTarget == null)
            {
                if ((args.DragSource as FrameworkElement).Name == "Part_PivotTableFields")
                {
                    if (args.DropTarget is ListBoxItem)
                    {
                        PivotTableField pivotTableField = (args.DragSource as ListBox).SelectedItem as PivotTableField;
                        this.PivotTableFields.Remove(pivotTableField);
                        int index = this.PivotTableFields.IndexOf((args.DropTarget as ListBoxItem).Content as PivotTableField);
                        this.PivotTableFields.Insert(index + 1, pivotTableField);
                    }
                }
            }
        }

        void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                if (e.OldItems != null)
                {
                    foreach (FilterItemsCollection item in e.OldItems)
                    {
                        UpdatePivotTableFieldItemSelection(item.Name);
                    }
                }
            }
            else
            {
                foreach (FilterItemsCollection item in e.NewItems)
                {
                    UpdatePivotTableFieldItemSelection(item.Name);
                }
            }
        }

        private bool FilterContains(string p)
        {
            foreach (var item in this.GridControl.GroupingBar.Filters)
            {
                if (item.Name == p)
                    return true;
            }
            return false;
        }

        void PivotCalculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                if (e.OldItems != null)
                {
                    foreach (PivotComputationInfo item in e.OldItems)
                    {
                        UpdatePivotTableFieldItemSelection(item.FieldName);
                    }
                }
            }
            else
            {
                foreach (PivotComputationInfo item in e.NewItems)
                {
                    UpdatePivotTableFieldItemSelection(item.FieldName);
                }
            }
        }

        void PivotItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
            {
                if (e.OldItems != null)
                {
                    foreach (PivotItem item in e.OldItems)
                    {
                        UpdatePivotTableFieldItemSelection(item.FieldMappingName);
                    }
                }
            }
            else
            {
                foreach (PivotItem item in e.NewItems)
                {
                    UpdatePivotTableFieldItemSelection(item.FieldMappingName);
                }
            }
        }

        void PivotFieldListWindow_Closed(object sender, Windows.Tools.Controls.ClosedEventArgs e)
        {
            this.GridControl.ShowFieldList = false;
            this.Dispose();
        }

        void PivotFieldListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.GridControl.ShowFieldList = true;
        }

        /// <summary>
        /// Initilizes the pivot table list.
        /// </summary>
        void InitilizePivotTableList()
        {
            MoveDown.IsEnabled = MoveUp.IsEnabled = false;
            if (GridControl != null)
            {
                PropertyInfo[] propertyinfoCollection = null;
                IEnumerable source = null;
                source = GridControl.ItemSource as IEnumerable;
                object firstItem = null;
                PropertyInfo propInfo_firstItem = null;
                if (source != null)
                {

                    foreach (object item in source)
                    {
                        firstItem = item;
                        propertyinfoCollection = item.GetType().GetProperties();
                        break;
                    }
                    if (firstItem != null)
                    {
                        propInfo_firstItem = firstItem.GetType().GetProperty(firstItem.ToString());
                    }
                }
                PivotTableField sp;
                if (firstItem != null)
                {
                    if (showSelectedFields && this.GridControl.SelectedFields != null)
                    {
                        foreach (string item in this.GridControl.SelectedFields)
                        {
                            foreach (PropertyInfo pi in propertyinfoCollection)
                            {
                                if (pi.Name == item)
                                {
                                    var pivotComputationInfo = this.GridControl.PivotCalculations.Where(p => p.FieldName == pi.Name).FirstOrDefault();
                                    this.PivotTableFields.Add(sp = new PivotTableField
                                    {
                                        FieldHeader = GetPivotTableFieldHeader(pi.Name),
                                        Format = GetPivotTableFormat(pi.Name, pi.PropertyType),
                                        FieldPropertyDescriptor = pi,
                                        IsSelected = GetPivotTableFieldIsSelected(pi.Name),
                                        SummaryType = pivotComputationInfo is PivotComputationInfo ? pivotComputationInfo.SummaryType : GetPivotTableSummaryType(pi.PropertyType),
                                        TotalHeader = GetPivotTableFieldTotalHeader(pi.Name),
                                        AllowRunTimeGroupByField = GetPivotTableAllowGrouping(pi.Name)
                                    });
                                    InvokePivotTableFieldSettings(sp);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (PropertyInfo pi in propertyinfoCollection)
                        {
                            var pivotComputationInfo = this.GridControl.PivotCalculations.Where(p => p.FieldName == pi.Name).FirstOrDefault();
                            this.PivotTableFields.Add(sp = new PivotTableField
                            {
                                FieldHeader = GetPivotTableFieldHeader(pi.Name),
                                Format = GetPivotTableFormat(pi.Name, pi.PropertyType),
                                FieldPropertyDescriptor = pi,
                                IsSelected = GetPivotTableFieldIsSelected(pi.Name),
                                SummaryType = pivotComputationInfo is PivotComputationInfo ? pivotComputationInfo.SummaryType : GetPivotTableSummaryType(pi.PropertyType),
                                TotalHeader = GetPivotTableFieldTotalHeader(pi.Name),
                                AllowRunTimeGroupByField = GetPivotTableAllowGrouping(pi.Name)
                            });

                            InvokePivotTableFieldSettings(sp);
                        }
                    }
                }
            }
        }

        private void InvokePivotTableFieldSettings(PivotTableField sp)
        {
            sp.PropertyChanged += (pSender, pargs) =>
            {
                if (pargs.PropertyName.Equals("IsSelected"))
                {
                    if (!resumeProcessing)
                    {
                        PivotTableField pivotTableField = (PivotTableField)pSender;
                        if (pivotTableField.IsSelected)
                        {
#if SILVERLIGHT
                            object propInfo = this.GridControl.GroupingBar.GetPropertyDescriptor(pivotTableField.FieldName);
                            var dataType = (propInfo as PropertyInfo).PropertyType;
                            if (dataType == typeof(int) ||
                                dataType == typeof(float) ||
                                dataType == typeof(double) ||
                                dataType == typeof(decimal) ||
                                dataType == typeof(short) ||
                                dataType == typeof(long))
#else
                            if (pivotTableField.DataType == typeof(int) ||
                                pivotTableField.DataType == typeof(float) ||
                                pivotTableField.DataType == typeof(double) ||
                                pivotTableField.DataType == typeof(decimal) ||
                                pivotTableField.DataType == typeof(short) ||
                                pivotTableField.DataType == typeof(long))
#endif
                            {
                                if (this.PivotTableFields != null)
                                {
                                    PivotTableField _field = this.PivotTableFields.Where(i => i.FieldName == pivotTableField.FieldName).FirstOrDefault();
                                    this.GridControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = _field.FieldHeader, FieldName = _field.FieldName, Format = _field.Format, SummaryType = _field.SummaryType, AllowRunTimeGroupByField = _field.AllowRunTimeGroupByField });
                                }
                                else
                                {
                                    this.GridControl.PivotCalculations.Add(new PivotComputationInfo { FieldHeader = pivotTableField.FieldHeader, FieldName = pivotTableField.FieldName, SummaryType = SummaryType.DoubleTotalSum, AllowRunTimeGroupByField = pivotTableField.AllowRunTimeGroupByField });
                                }
                            }
                            else
                                this.GridControl.PivotRows.Add(GetPivotItem(pivotTableField));
                            PivotItem pivotitem = this.GridControl.PivotFields.Where(p => p.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                            if (pivotitem != null)
                            {
                                this.GridControl.PivotFields.Remove(pivotitem);
                            }
                        }
                        else
                        {
                            RemoveItem(pivotTableField);
                            PivotItem pivotitem = this.GridControl.PivotFields.Where(p => p.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                            if (pivotitem == null)
                            {
                                this.GridControl.PivotFields.Add(GetPivotItem(pivotTableField));
                            }
                        }
                        if ((this.GridControl as PivotGridControl).GroupingBar != null)
                        {
                            (this.GridControl as PivotGridControl).GroupingBar.ApplyEmptyTemplate();
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Update the PivotTableField selection status
        /// </summary>
        /// <param name="fieldName"></param>
        private void UpdatePivotTableFieldItemSelection(string fieldName)
        {
            if (this.Part_PivotTableFields == null) return;
            IEnumerable<PivotTableField> sourceFields = this.Part_PivotTableFields.ItemsSource as IEnumerable<PivotTableField>;
            if (sourceFields != null)
            {
                var field = sourceFields.Where(i => i.FieldName == fieldName).FirstOrDefault();
                if (field != null)
                {
                    resumeProcessing = true;
                    field.IsSelected = GetPivotTableFieldIsSelected(fieldName);
                    resumeProcessing = false;
                }
            }
        }

        /// <summary>
        /// Checks is PivotTableField exist in any of the PivotInfo collection(FilterList,
        /// ColumnPivot, RowPivot or in Calculation
        /// </summary>
        /// <param name="fieldName">ItemSource fieldname</param>
        /// <returns>bool</returns>
        private bool GetPivotTableFieldIsSelected(string fieldName)
        {
            bool isSelected = this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null;
            if (!isSelected)
                isSelected = this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null;
            if (!isSelected)
                isSelected = this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null;
            if (!isSelected)
                isSelected = this.GridControl.GroupingBar.Filters.Where(f => f.Name == fieldName).FirstOrDefault() != null;
            if (!isSelected)
                isSelected = (this.GridControl as PivotGridControl).FilterItems.Where(f => f.Name == fieldName).FirstOrDefault() != null;

            return isSelected;
        }

        /// <summary>
        /// Checks for the FieldHeader name
        /// </summary>
        /// <param name="fieldName">ItemSource fieldname</param>
        /// <returns>string</returns>
        private string GetPivotTableFieldHeader(string fieldName)
        {
            if (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).FieldHeader;
            if (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).FieldHeader;
            if (this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotComputationInfo).FieldHeader;
            if (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).FieldHeader;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).FieldHeader;
            return fieldName;
        }

        /// <summary>
        /// Checks for the FieldHeader name
        /// </summary>
        /// <param name="fieldName">ItemSource fieldname</param>
        /// <returns>string</returns>
        private string GetPivotTableFieldTotalHeader(string fieldName)
        {
            if (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).TotalHeader;
            if (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).TotalHeader;
            if (this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return PivotGridConstants.TotalString;
            if (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).TotalHeader;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).TotalHeader;
            return fieldName;
        }

        private string GetPivotTableFormat(string fieldName, Type fieldType)
        {
            if (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).Format;
            if (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).Format;
            if (this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotComputationInfo).Format;
            if (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).Format;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).Format;
            if (fieldType == typeof(int) || fieldType == typeof(float) || fieldType == typeof(double) || fieldType == typeof(decimal))
                return "#.##"; // default format for computational info objects.
            else
                return null; //default format for pivot item objects
        }

        private bool GetPivotTableAllowGrouping(string fieldName)
        {
            if (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotColumns.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).AllowRunTimeGroupByField;
            if (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotRows.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).AllowRunTimeGroupByField;
            if (this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotCalculations.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotComputationInfo).AllowRunTimeGroupByField;
            if (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() != null)
                return (this.GridControl.PivotFields.Where(p => p.FieldMappingName == fieldName).FirstOrDefault() as PivotItem).AllowRunTimeGroupByField;
            if (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() != null)
                return (this.PivotTableFields.Where(p => p.FieldName == fieldName).FirstOrDefault() as PivotTableField).AllowRunTimeGroupByField;
            return true;
        }

        private SummaryType GetPivotTableSummaryType(Type type)
        {
            if (type == typeof(int))
            {
                return SummaryType.IntTotalSum;
            }
            if (type == typeof(decimal))
            {
                return SummaryType.DecimalTotalSum;
            }
            if (type == typeof(double) || type == typeof(float))
            {
                return SummaryType.DoubleTotalSum;
            }
            return SummaryType.Count;
        }

        /// <summary>
        /// Generates a PivotItem based on supplies PivotTableField 
        /// </summary>
        /// <param name="pivotTableField">PivotItemField</param>
        /// <returns>Pivotitem</returns>
        private PivotItem GetPivotItem(PivotTableField pivotTableField)
        {
            return new PivotItem { FieldHeader = pivotTableField.FieldHeader, FieldMappingName = pivotTableField.FieldName, TotalHeader = pivotTableField.TotalHeader, Format = pivotTableField.Format, AllowRunTimeGroupByField = pivotTableField.AllowRunTimeGroupByField };
        }

        /// <summary>
        /// Removes item PivotInfo based on the PivotTable 
        /// </summary>
        /// <param name="pivotTableField"></param>
        private void RemoveItem(PivotTableField pivotTableField)
        {
            //// Remeving item if exist in PivotRows
            PivotItem pivotItem = this.GridControl.PivotRows.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
            if (pivotItem != null)
                this.GridControl.PivotRows.Remove(pivotItem);

            //// Removing item if exist in PivotColumns
            pivotItem = this.GridControl.PivotColumns.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
            if (pivotItem != null)
                this.GridControl.PivotColumns.Remove(pivotItem);

            //// Removing item if exist in PivotValues
            int value = this.GridControl.PivotCalculations.Where(k => k.FieldName == pivotTableField.FieldName).Count();
            for (int i = 0; i < value; i++)
            {
                PivotComputationInfo calcInfo = this.GridControl.PivotCalculations.Where(j => j.FieldName == pivotTableField.FieldName).FirstOrDefault();
                if (calcInfo != null)
                    this.GridControl.PivotCalculations.Remove(calcInfo);
            }

            //// Removing item if exist in Filters
            FilterItemsCollection filterItem = this.GridControl.GroupingBar.Filters.Where(f => f.Name == pivotTableField.FieldName).FirstOrDefault();
            if (filterItem != null)
                this.GridControl.GroupingBar.Filters.Remove(filterItem);
            FilterExpression filterExpression = this.GridControl.Filters.Where(f => f.Name == pivotTableField.FieldName).FirstOrDefault();
            if (filterExpression != null)
                this.GridControl.Filters.Remove(filterExpression);
            PivotGridGroupingBar groupingBar = this.GridControl.GroupingBar as PivotGridGroupingBar;
            groupingBar.CalculatePivotRowItemWidth();

        }

        private void UpDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.Part_PivotTableFields.SelectedItem != null)
            {
                PivotTableField pivotTableField = (Part_PivotTableFields.SelectedItem as PivotTableField);

                int index = this.PivotTableFields.IndexOf(pivotTableField);
                this.PivotTableFields.Remove(pivotTableField);
                if ((sender as Button).Name.Equals("MoveUp", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.PivotTableFields.Insert(index == 0 ?index:index - 1, pivotTableField);
                }
                else if ((sender as Button).Name.Equals("MoveDown", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.PivotTableFields.Insert(index + 1, pivotTableField);
                }
                this.Part_PivotTableFields.SelectedItem = pivotTableField; 
            }
        }

        private void Part_PivotTableFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Part_PivotTableFields.Items.Count == 1)
            {
                this.MoveDown.IsEnabled = false;
                this.MoveUp.IsEnabled = false;
            }
            else
            {
                if ((sender as ListBox).SelectedIndex == (this.Part_PivotTableFields.Items.Count - 1))
                {
                    this.MoveDown.IsEnabled = false;
                    this.MoveUp.IsEnabled = true;
                }
                else if ((sender as ListBox).SelectedIndex == 0)
                {
                    this.MoveUp.IsEnabled = false;
                    this.MoveDown.IsEnabled = true;
                }
                else
                {
                    this.MoveUp.IsEnabled = true;
                    this.MoveDown.IsEnabled = true;
                }
            }
        }

        #endregion
    }
}
