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
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using Syncfusion.PivotAnalysis.Base;

using System.Diagnostics;

#if !SILVERLIGHT
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using System.Windows;
using System.ComponentModel;
using System.Collections;
namespace Syncfusion.Windows.Controls.PivotGrid
#else
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#endif
{
    /// <summary>
    /// This class is used to serialize some important properties in PivotGrid control
    /// </summary>
#if !SILVERLIGHT
    [Serializable]    
#else
#endif
    public class PivotGridSerializer
    {
        #region [ Initialize/Finalize ]

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridSerializer"/> class.
        /// </summary>
        public PivotGridSerializer()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PivotGridSerializer"/> class.
        /// </summary>
        /// <param name="ctrl">The CTRL.</param>
        public PivotGridSerializer(PivotGridControl ctrl)
        {
            this.GridControl = ctrl;
            if (this.GridControl != null)
            {
                this.PivotRows = ctrl.PivotRows;
                this.PivotColumns = ctrl.PivotColumns;
                this.PivotCalculations = ctrl.PivotCalculations;
                this.PivotFields = ctrl.PivotFields;
                this.Filters = ctrl.Filters;
                this.ConditionalFormats = ctrl.ConditionalFormats.ToList();

                this.GridFilterCollection = new List<GridFilter>();
                this.GridSortCollection = new List<Sort>();
                this.HiddenColumnGroups = new List<HiddenGroup>();
                this.HiddenRowGroups = new List<HiddenGroup>();
            }
        }

        #endregion

        #region [ Public Properties ]
        /// <summary>
        /// Gets or sets the PivotGridControl instance
        /// </summary>
        [XmlIgnore]
        public PivotGridControl GridControl { get; set; }
        /// <summary>
        /// Gets or sets the PivotCalculations
        /// </summary>
        public ObservableCollection<PivotComputationInfo> PivotCalculations { get; set; }
        /// <summary>
        /// Gets or sets the filters
        /// </summary>
        public ObservableCollection<FilterExpression> Filters { get; set; }
        /// <summary>
        /// Gets or sets the PivotColumns
        /// </summary>
        public ObservableCollection<PivotItem> PivotColumns { get; set; }
        /// <summary>
        /// Gets or sets the PivotRows
        /// </summary>
        public ObservableCollection<PivotItem> PivotRows { get; set; }
        /// <summary>
        /// Gets or sets the PivotFields
        /// </summary>
        public ObservableCollection<PivotItem> PivotFields { get; set; }
        /// <summary>
        /// Gets or sets the Filter Collection of PivotGrid
        /// </summary>
        public List<GridFilter> GridFilterCollection { get; set; }
        /// <summary>
        /// Gets or sets the Sort Collection of PivotGrid
        /// </summary>
        public List<Sort> GridSortCollection { get; set; }
        /// <summary>
        /// Gets or sets the value indicating the number of columns to be resized. 
        /// </summary>
        public int AutoSizeColumnCount { get; set; }
        /// <summary>
        /// Gets or sets whether GroupingBar need to be visible or not
        /// </summary>
        public bool ShowGroupingBar { get; set; }
        /// <summary>
        /// Gets or sets whether Fieldlist need to be visible or not
        /// </summary>
        public bool ShowFieldList { get; set; }
        /// <summary>
        /// Gets or sets the value indicating the number of rows to be resized. 
        /// </summary>
        public int AutoSizeRowCount { get; set; }
        /// <summary>
        /// Gets or sets the Auto size option based on which Grid rows will be resized.
        /// </summary>
        /// <value>The auto size option.</value>
        public GridAutoSizeOption AutoSizeOption { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the supplied Item Source is dynamic
        /// </summary>
        public bool IsDynamicData { get; set; }
        /// <summary>
        /// Gets or sets whether the calculations should appear as rows or columns. THe default behavior is 
        /// for the calculations to appear as columns.
        /// </summary>
        public bool ShowCalculationsAsColumns { get; set; }

        /// <summary>
        /// Gets or sets whether the column, row headers should freeze or not
        /// </summary>
        public bool FreezeHeaders { get; set; }
        /// <summary>
        /// Gets or sets whether the layout should be updated immediately after the 
        /// pivoting info update or it should wait for a Refresh() call.
        /// </summary>
        public bool DeferLayoutUpdate { get; set; }
        /// <summary>
        /// Gets or sets whether grand total calculations should be computed by the engine.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool ShowGrandTotals { get; set; }
        /// <summary>
        /// Gets or sets value indicating whether sub total calculations should be shown or hidden.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool ShowSubTotals { get; set; }
        /// <summary>
        /// Gets or sets whether the PivotGrid resize to its changed size
        /// </summary>
        public bool ResizePivotGridToFit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to Allow Selection of Cells as Like in Excel
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        public bool AllowSelection { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to [allow resize columns].
        /// </summary>
        /// <value><c>true</c> if [allow resize columns]; otherwise, <c>false</c>.</value>
        public bool AllowResizeColumns { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether [allow resize rows].
        /// </summary>
        /// <value><c>true</c> if [allow resize rows]; otherwise, <c>false</c>.</value>
        public bool AllowResizeRows { get; set; }
        /// <summary>
        /// Gets or sets whether filtering on GroupingBar is enabled
        /// </summary>
        public bool GroupingBarFiltering { get; set; }
        /// <summary>
        /// Gets or sets whether sorting on GroupingBar is enabled
        /// </summary>
        public bool GroupingBarSorting { get; set; }
        /// <summary>
        /// Enable/Disable Background color for the grouping disabled fields. Default false
        /// </summary>
        /// <value>The selected items.</value>
        public bool ShowDisabledGroupBackground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore expand/collapse state of cells on serialization.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [ignore expand collapse on serialization]; otherwise, <c>false</c>.
        /// </value>
        [XmlIgnore]
        public bool IgnoreExpandCollapseOnSerialization { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether to maintain/show collapsed cells when pivot schema getting changed.
        /// </summary> 
        public bool StatePersistenceEnabled { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether show empty value cell if summary value has null value.
        /// By default true.
        /// </summary> 
        public bool ShowEmptyCells { get; set; }
        /// <summary>
        /// Gets or sets the list of hidden rows
        /// </summary>
        public List<HiddenGroup> HiddenRowGroups { get; set; }
        /// <summary>
        /// Gets or sets the list of hidden columns
        /// </summary>
        public List<HiddenGroup> HiddenColumnGroups { get; set; }
        /// <summary>
        /// Gets or sets the list of Conditional formats
        /// </summary>
        public List<PivotGridDataConditionalFormat> ConditionalFormats { get; set; }

        #endregion

        #region [ Helper Methods ]

        /// <summary>
        /// Wraps some of the properties in this instance to PivotGrid control
        /// </summary>
        internal void Wrap()
        {
            if (this.GridControl.GroupingBar != null && this.GridControl.GroupingBar.FilterHeaderArea != null
                && this.GridControl.GroupingBar.FilterHeaderArea.ItemsSource != null)
            {
                foreach (FilterItemsCollection item in this.GridControl.GroupingBar.FilterHeaderArea.ItemsSource)
                {
                    this.GridFilterCollection.Add(new GridFilter()
                    {
                        FilterItems = item,
                        FilterProperty = item.FilterProperty,
                        Name = item.Name,
                        IsFilterHeaderArea = true,
                        FilteredValues = item.FilteredValues,
                        Header = item.DisplayHeader
                    });
                }
            }

            if (this.Filters.Count > 0)
            {
                foreach (var item in this.Filters)
                {
                    FilterItemsCollection collection = item.Tag as FilterItemsCollection;

                    if (this.GridControl.GroupingBar != null)
                    {
                        if (!this.GridControl.GroupingBar.Filters.Contains(collection))
                        {
                            AddFilterCollection(collection);
                        }
                    }
                    else
                    {
                        AddFilterCollection(collection);
                    }
                }
            }

            foreach (var item in this.PivotColumns)
            {
                this.GridSortCollection.Add(new Sort(item));
            }

            foreach (var item in this.PivotRows)
            {
                this.GridSortCollection.Add(new Sort(item));
            }

            this.AllowResizeColumns = this.GridControl.AllowResizeColumns;
            this.AllowResizeRows = this.GridControl.AllowResizeRows;
            this.AllowSelection = this.GridControl.AllowSelection;
            this.AutoSizeColumnCount = this.GridControl.AutoSizeColumnCount;
            this.AutoSizeOption = this.GridControl.AutoSizeOption;
#if !SILVERLIGHT
            this.AutoSizeRowCount = this.GridControl.AutoSizeRowCount;
#endif
            this.DeferLayoutUpdate = this.GridControl.DeferLayoutUpdate;
            this.FreezeHeaders = this.GridControl.FreezeHeaders;
            this.IsDynamicData = this.GridControl.IsDynamicData;
            this.ShowCalculationsAsColumns = this.GridControl.ShowCalculationsAsColumns;
#if !SILVERLIGHT
            this.ShowFieldList = this.GridControl.ShowFieldList;
#endif
            this.ShowGrandTotals = this.GridControl.ShowGrandTotals;
            this.ShowGroupingBar = this.GridControl.ShowGroupingBar;
            this.ShowEmptyCells = this.GridControl.ShowEmptyCells;
            this.StatePersistenceEnabled = this.GridControl.StatePersistenceEnabled;
            this.ShowSubTotals = this.GridControl.ShowSubTotals;
            this.ResizePivotGridToFit = this.GridControl.ResizePivotGridToFit;
            this.ShowDisabledGroupBackground = this.GridControl.ShowDisabledGroupBackground;

            if (this.GridControl.GroupingBar != null)
            {
                this.GroupingBarFiltering = this.GridControl.GroupingBar.AllowFiltering;
                this.GroupingBarSorting = this.GridControl.GroupingBar.AllowSorting;
            }

            this.IgnoreExpandCollapseOnSerialization = this.GridControl.IgnoreExpandCollapseOnSerialization;

            if (!this.IgnoreExpandCollapseOnSerialization)
            {
                foreach (var item in this.GridControl.InternalGrid.HiddenColumnGroups)
                {
                    this.HiddenColumnGroups.Add(item);
                }

                foreach (var item in this.GridControl.InternalGrid.HiddenRowGroups)
                {
                    this.HiddenRowGroups.Add(item);
                }
            }
        }

        private void AddFilterCollection(FilterItemsCollection collection)
        {
            this.GridFilterCollection.Add(new GridFilter()
            {
                FilterItems = collection,
                FilterProperty = collection.FilterProperty,
                Name = collection.Name,
                IsFilterHeaderArea = false,
                FilteredValues = collection.FilteredValues,
                Header = collection.DisplayHeader
            });
        }

        /// <summary>
        /// Gets the pivot grid control.
        /// </summary>
        /// <returns></returns>
        internal void WrapGridControl(PivotGridControl ctrl)
        {
            ctrl.IgnoreRefesh = true;
            this.GridControl = ctrl;
            ctrl.ExpandAllGroup();
            for (int count = ctrl.PivotRows.Count-1; count >= 0; count--)
            {
                ctrl.PivotRows.RemoveAt(count);
            }

            for (int count = ctrl.PivotColumns.Count-1; count >= 0; count--)
            {
                ctrl.PivotColumns.RemoveAt(count);
            }

            for (int count = ctrl.PivotCalculations.Count-1; count >= 0; count--)
            {
                ctrl.PivotCalculations.RemoveAt(count);
            }

            for (int count = ctrl.Filters.Count-1; count >= 0; count--)
            {
                ctrl.Filters.RemoveAt(count);
            }

#if !SILVERLIGHT
            for (int count = ctrl.PivotFields.Count - 1; count >= 0; count--)
            {
                ctrl.PivotFields.RemoveAt(count);
            }
#endif

            if (ctrl.GroupingBar != null && ctrl.GroupingBar.Filters != null)
            {
                for (int count = ctrl.GroupingBar.Filters.Count - 1; count >= 0; count--)
                {
                    ctrl.GroupingBar.Filters.RemoveAt(count);
                }
            }

            ctrl.IgnoreRefesh = false;

            foreach (var item in this.PivotCalculations)
            {
                ctrl.PivotCalculations.Add(item);
            }

            foreach (var item in this.PivotColumns)
            {
                var sortItem=this.GridSortCollection.Where(i => i.FieldMappingName == item.FieldMappingName).FirstOrDefault();
                if (sortItem!=null && sortItem.IsDescSort)
                {
                    item.Comparer = new ReverseOrderComparer();
                }
                ctrl.PivotColumns.Add(item);
            }

            foreach (var item in this.PivotRows)
            {
                var sortItem = this.GridSortCollection.Where(i => i.FieldMappingName == item.FieldMappingName).FirstOrDefault();
                if (sortItem != null && sortItem.IsDescSort)
                {
                    item.Comparer = new ReverseOrderComparer();
                }
                ctrl.PivotRows.Add(item);
            }

            foreach (var item in this.PivotFields)
            {
                ctrl.PivotFields.Add(item);
            }

            ctrl.AllowResizeColumns = this.AllowResizeColumns;
            ctrl.AllowResizeRows = this.AllowResizeRows;
            ctrl.AllowSelection = this.AllowSelection;
            ctrl.AutoSizeColumnCount = this.AutoSizeColumnCount;
            ctrl.AutoSizeOption = this.AutoSizeOption;
#if !SILVERLIGHT
            ctrl.AutoSizeRowCount = this.AutoSizeRowCount;
#endif
            ctrl.DeferLayoutUpdate = this.DeferLayoutUpdate;
            ctrl.FreezeHeaders = this.FreezeHeaders;
            ctrl.IsDynamicData = this.IsDynamicData;
            ctrl.ShowCalculationsAsColumns = this.ShowCalculationsAsColumns;
#if !SILVERLIGHT
            ctrl.ShowFieldList = this.ShowFieldList;
#endif
            ctrl.ShowGrandTotals = this.ShowGrandTotals;
            ctrl.ShowGroupingBar = this.ShowGroupingBar;
            ctrl.ShowEmptyCells = this.ShowEmptyCells;
            ctrl.StatePersistenceEnabled = this.StatePersistenceEnabled;

            ctrl.ShowSubTotals = this.ShowSubTotals;
            ctrl.ResizePivotGridToFit = this.ResizePivotGridToFit;
            ctrl.ShowDisabledGroupBackground = this.ShowDisabledGroupBackground;

            ctrl.GridSerializer = this;

            if (ctrl.ShowGroupingBar)
            {
                if (ctrl.GroupingBar != null && ctrl.GroupingBar.ColumnHeaderArea == null)
                    ctrl.GroupingBar.ApplyTemplate();
            }
            //foreach (var item in this.Filters)

            if (this.Filters.Count > 0)
            {
                for (int i = 0; i < this.GridFilterCollection.Count; i++)
                {
                    FilterItemsCollection collection = new FilterItemsCollection();
                    GridFilter filter = this.GridFilterCollection[i];

                    collection.FilteredValues = filter.FilteredValues;
                    collection.Name = filter.Name;
                    collection.DisplayHeader = filter.Header;
                    if (ctrl.GroupingBar != null)
                    {
                        collection.FilterProperty = ctrl.GroupingBar.GetPropertyDescriptor(filter.Name);
                    }
                    else
                    {
                        collection.FilterProperty = GetPropertyDescriptor(filter.Name);
                    }
                    collection.FilteredValues = filter.FilteredValues;

                    foreach (var fitem in filter.FilterItems)
                    {
                        if (fitem.Key != PivotGridConstants.AllString)
                        {
                            collection.AddWireEvent(fitem);
                        }
                        else
                            collection[0].IsSelected = fitem.SelectedState;
                    }
                    FilterExpression item = this.Filters.Where(f => f.Name == this.GridFilterCollection[i].Name).FirstOrDefault();
                    if (item != null)
                    {
                        item.Tag = collection;
                        ctrl.Filters.Add(item);
                    }
                }
            }

#if SILVERLIGHT

            this.GridControl.LayoutUpdated += new EventHandler(GridControl_LayoutUpdated);
#endif
#if !SILVERLIGHT
            if (ctrl.ShowGroupingBar)
                this.ConfigureColumnHeaderArea(ctrl);
#endif
            this.ConfigureFilter(ctrl);

            if (ctrl.GroupingBar != null)
            {
                ctrl.GroupingBar.AllowFiltering = this.GroupingBarFiltering;
                ctrl.GroupingBar.AllowSorting = this.GroupingBarSorting;
                ctrl.GroupingBar.ApplyEmptyTemplate();
            }
#if !SILVERLIGHT
            if (this.GridControl.ItemSource == null || ((this.GridControl.ItemSource is System.Data.DataTable) && (this.GridControl.ItemSource as System.Data.DataTable).DefaultView.Count == 0) || ((this.GridControl.ItemSource as IList).Count == 0))
            {
#endif
#if SILVERLIGHT
            if (this.GridControl.ItemSource == null )
            {
#endif
                this.IgnoreExpandCollapseOnSerialization = true;
            }




            if (!this.IgnoreExpandCollapseOnSerialization)
            {
                foreach (var item in this.HiddenRowGroups)
                {
                    ctrl.InternalGrid.CollapseGroup(ctrl.PivotEngine[item.From, item.Level], ctrl.InternalGrid.Model[item.From, item.Level]);
                }

                foreach (var item in this.HiddenColumnGroups)
                {
                    ctrl.InternalGrid.CollapseGroup(ctrl.PivotEngine[item.Level, item.From], ctrl.InternalGrid.Model[item.Level, item.From]);
                }
                ctrl.InvalidateCells();
            }

            if (this.ConditionalFormats != null && this.ConditionalFormats.Count > 0)
            {
#if !SILVERLIGHT
                var cfs = new FreezableCollection<PivotGridDataConditionalFormat>(this.ConditionalFormats);
#else
                var cfs = new ObservableCollection<PivotGridDataConditionalFormat>(this.ConditionalFormats);
#endif
                ctrl.ConditionalFormats = cfs;
                ctrl.InvalidateCells();
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Gets the property descriptor.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>property descriptor.</returns>
        internal PropertyDescriptor GetPropertyDescriptor(string fieldName)
        {

            if (this.GridControl.PivotEngine != null && this.GridControl.PivotEngine.ItemProperties != null)
            {
                return this.GridControl.PivotEngine.ItemProperties[fieldName];
            }
            else
                return null;
        }

# else
        /// <summary>
        /// Gets the property descriptor.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>property descriptor.</returns>
        internal object GetPropertyDescriptor(string fieldName)
        {
            if (this.GridControl.PivotEngine != null && this.GridControl.PivotEngine.ItemProperties != null)
            {
                if(this.GridControl.PivotEngine.ItemProperties[fieldName] is System.Reflection.PropertyInfo)
                    return this.GridControl.PivotEngine.ItemProperties[fieldName] as System.Reflection.PropertyInfo;
                if(this.GridControl.PivotEngine.ItemProperties[fieldName] is Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor)
                    return this.GridControl.PivotEngine.ItemProperties[fieldName] as Syncfusion.PivotAnalysis.Base.Silverlight.ExpressionPropertyDescriptor;
            }
            return null;
        }
#endif
        /// <summary>
        /// Method used to configure filter in PivotGridControl while serialize it to xml
        /// </summary>
        /// <param name="ctrl">The PivotGrid Control</param>
        public void ConfigureFilter(PivotGridControl ctrl)
        {
            if (ctrl.GroupingBar != null) {
                if (ctrl.GroupingBar.Filters.Count > 0 || (ctrl.GridSerializer != null && ctrl.GridSerializer.GridFilterCollection != null))
                {
                    if (this.Filters.Count > 0 || (this.GridControl.GridSerializer != null &&
                        this.GridControl.GridSerializer.GridFilterCollection.Count > 0 &&
                        ctrl.GroupingBar.IsFilterHeaderArea(this.GridControl.GridSerializer.GridFilterCollection)))
                    {
                        if (ctrl.GroupingBar.FilterHeaderArea.ItemsSource == null)
                        {
                            ctrl.GroupingBar.FilterHeaderArea.Items.Clear();
                        }
                        else
                        {
                            ctrl.GroupingBar.FilterHeaderArea.ItemsSource = null;
                            ctrl.GroupingBar.FilterHeaderArea.Items.Clear();
                        }
                    }


                    if (ctrl.GridSerializer.GridFilterCollection != null)
                    {
                        for (int i = 0; i < ctrl.GridSerializer.GridFilterCollection.Count; i++)
                        {
                            GridFilter filter = ctrl.GridSerializer.GridFilterCollection[i];
                            if (filter.IsFilterHeaderArea)
                            {
                                FilterItemsCollection collection = new FilterItemsCollection();
                                collection.Name = filter.Name;
                                collection.DisplayHeader = filter.Header;
                                collection.FilterProperty = ctrl.GroupingBar.GetPropertyDescriptor(filter.Name);

                                foreach (var item in filter.FilterItems)
                                {
                                    if (item.Key != PivotGridConstants.AllString)
                                    {
                                        //item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                                        collection.AddWireEvent(item);
                                    }
                                    else
                                        collection[0].IsSelected = item.SelectedState;
                                }

                                ctrl.GroupingBar.Filters.Add(collection);
                            }
                        }
                    }

                    if (ctrl.GroupingBar.Filters.Count > 0)
                    {
                        if (ctrl.GroupingBar.FilterHeaderArea.ItemTemplate == null)
                        {
                            ResourceDictionary resource = new ResourceDictionary()
                            {
#if !SILVERLIGHT
                                Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                            Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                            };
                            ctrl.GroupingBar.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;
                        }

                        ctrl.GroupingBar.FilterHeaderArea.DataContext = ctrl.GroupingBar.Filters;// this.Filters;
                        ctrl.GroupingBar.FilterHeaderArea.ItemsSource = ctrl.GroupingBar.Filters;
                    }
                    else
                        if (ctrl.GroupingBar.FilterHeaderArea != null)
                            ctrl.GroupingBar.FilterHeaderArea.ItemTemplate = null;
                }
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// Method used to configure the column header area data while serilize the pivot grid to xml
        /// </summary>
        /// <param name="ctrl">The PivotGridControl</param>
        public void ConfigureColumnHeaderArea(PivotGridControl ctrl)
        {
            if (ctrl.PivotColumns.Count > 0 && ctrl.GroupingBar != null)
            {  
                if (ctrl.GroupingBar.ColumnHeaderArea != null)
                {
                    ResourceDictionary resource = new ResourceDictionary()
                    {
                        Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                    };
                    ctrl.GroupingBar.ColumnHeaderArea.ItemTemplate = resource["PivotColumnItemTemplate"] as DataTemplate;
                    ctrl.GroupingBar.ColumnHeaderArea.DataContext = ctrl.PivotColumns;
                    ctrl.GroupingBar.ColumnHeaderArea.ItemsSource = ctrl.PivotColumns;
                }
            }
        }
#endif

        #region [ Events ]

#if SILVERLIGHT
        void GridControl_LayoutUpdated(object sender, EventArgs e)
        {
            if (this.GridControl.GroupingBar != null && this.GridControl.GroupingBar.ColumnHeaderArea != null &&
               this.GridControl.GroupingBar.ColumnHeaderArea.ItemContainerGenerator != null && this.GridControl.ShowGroupingBar)
            {
                for (int i = 0; i < this.PivotColumns.Count; i++)
                {
                    ListBoxItem item = this.GridControl.GroupingBar.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        Path colSortPath = Common.FindVisualChildWithName<Path>(item, "colSortPath");
                        if (this.GridControl.PivotColumns[i].Comparer == null)
                        {
                            string data = " F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z";
                            colSortPath.Data = Common.GetPathGeometry(data);
                        }
                        else
                        {
                            string data = " F1 M 174.248,174.201L 174.248,174.201C 174.119,174.201 174.014,174.403 174.014,174.654C 174.014,174.774 174.039,174.891 174.082,174.972L 177.906,180.759C 177.956,180.83 178.018,180.873 178.084,180.873C 178.151,180.873 178.213,180.83 178.263,180.759L 179.156,179.411L 182.092,174.976C 182.135,174.896 182.161,174.778 182.161,174.659C 182.161,174.407 182.057,174.205 181.929,174.205L 180.434,174.204L 175.744,174.202L 174.248,174.201 Z";
                            colSortPath.Data = Common.GetPathGeometry(data);
                        }
                    }

                    // item.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                    // this.GridControl.GroupingBar.ColumnHeaderArea.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;

                    //if (item.Content != null)
                    //{
                    //    ToggleButton tgbtn = Common.FindVisualChild<ToggleButton>(item);
                    //    if (tgbtn != null)
                    //    {
                    //        tgbtn.MouseEnter += new MouseEventHandler(tgbtn_MouseEnter);
                    //        tgbtn.Command = new SortPivotRowItemCommand();
                    //        tgbtn.Command.CanExecuteChanged += new EventHandler(Command_CanExecuteChangedForSort);
                    //        Button filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");

                    //        if (filterBtn != null)
                    //        {
                    //            //// for Cell Renderer
                    //            // PivotGridControlBase controlBase = Common.GetParentElement<PivotGridControlBase>(this);

                    //            //// for Popup
                    //            PivotGridControlBase controlBase = VisualTreeHelper.FindElementsInHostCoordinates(new Point(double.MaxValue, double.MinValue),
                    //                Application.Current.RootVisual as UIElement).Where(j => j.GetType() == typeof(PivotGridControl)).FirstOrDefault() as PivotGridControlBase;

                    //            if (controlBase != null)
                    //            {
                    //                bool AllowFiltering = controlBase.GridControl.GroupingBar.AllowFiltering;
                    //                if (!AllowFiltering)
                    //                {
                    //                    filterBtn.Visibility = System.Windows.Visibility.Collapsed;
                    //                }
                    //                else
                    //                    filterBtn.Visibility = System.Windows.Visibility.Visible;
                    //            }
                    //        }
                    //    }

                    //    Button btn = Common.FindVisualChild<Button>(item);
                    //    if (btn != null)
                    //    {
                    //        btn.Click += new RoutedEventHandler(btn_Click);
                    //        if (btn.Command != null)
                    //        {
                    //            btn.Command = new FilterPivotItemCommand();
                    //            btn.Command.CanExecuteChanged += new EventHandler(Command_CanExecuteChangedForFilter);
                    //        }
                    //    }

                }
            }

            if (this.GridControl.GroupingBar != null && this.GridControl.GroupingBar.RowHeaderArea != null &&
              this.GridControl.GroupingBar.RowHeaderArea.ItemContainerGenerator != null && this.GridControl.ShowGroupingBar)
            {
                for (int i = 0; i < this.PivotRows.Count; i++)
                {
                    ListBoxItem item = this.GridControl.GroupingBar.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        Path rowSortPath = Common.FindVisualChildWithName<Path>(item, "rowSortPath");
                        if (this.GridControl.PivotRows[i].Comparer == null)
                        {
                            string data = " F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z";
                            rowSortPath.Data = Common.GetPathGeometry(data);
                        }
                        else
                        {
                            string data = " F1 M 174.248,174.201L 174.248,174.201C 174.119,174.201 174.014,174.403 174.014,174.654C 174.014,174.774 174.039,174.891 174.082,174.972L 177.906,180.759C 177.956,180.83 178.018,180.873 178.084,180.873C 178.151,180.873 178.213,180.83 178.263,180.759L 179.156,179.411L 182.092,174.976C 182.135,174.896 182.161,174.778 182.161,174.659C 182.161,174.407 182.057,174.205 181.929,174.205L 180.434,174.204L 175.744,174.202L 174.248,174.201 Z";
                            rowSortPath.Data = Common.GetPathGeometry(data);
                        }
                    }
                }
                this.GridControl.GroupingBar.RowHeaderArea.InvalidateArrange();
                this.GridControl.GroupingBar.ColumnHeaderArea.InvalidateArrange();
                this.GridControl.GroupingBar.DataHeaderArea.InvalidateArrange();
                this.GridControl.GroupingBar.FilterHeaderArea.InvalidateArrange();
            }

            this.GridControl.LayoutUpdated -= new EventHandler(GridControl_LayoutUpdated);
        }

        //void tgbtn_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    VisualStateManager.GoToState(sender as Control, "MouseOver", false);
        //}

#endif

        #endregion

        #endregion
    }
    /// <summary>
    /// Class that holds the properties which helps to do sort
    /// </summary>
#if !SILVERLIGHT
    [Serializable]    
#else
#endif
    public class Sort
    {
        /// <summary>
        /// Initializes the <see cref="Sort"/> class.
        /// </summary>
        public Sort()
        {

        }
        /// <summary>
        /// Initializes the <see cref="Sort"/> class.
        /// </summary>
        /// <param name="item"> Pivot item</param>
        public Sort(PivotItem item)
        {
            this.PivotItem = item;
            this.IsDescSort = this.PivotItem.Comparer == null ? false : true;
            this.FieldMappingName = this.PivotItem.FieldMappingName;
        }
        /// <summary>
        /// Gets or Sets the Pivot item
        /// </summary>
        [XmlIgnore]
        public PivotItem PivotItem { get; set; }
        /// <summary>
        /// Gets or sets whether the sort order is descending or ascending
        /// </summary>
        public bool IsDescSort
        {
            get;
            set;
        }
        /// <summary>
        /// Gets or sets the field mapping name of the PivotItems
        /// </summary>
        public string FieldMappingName
        {
            get;
            set;
        }
    }
}
