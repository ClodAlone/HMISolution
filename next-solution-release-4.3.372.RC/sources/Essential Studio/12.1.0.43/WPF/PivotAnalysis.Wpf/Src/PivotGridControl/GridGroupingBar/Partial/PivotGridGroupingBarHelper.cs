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
using System.Windows.Controls;
using Syncfusion.PivotAnalysis.Base;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections;
using System.Windows.Documents;

#if SILVERLIGHT
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Globalization;
using Syncfusion.Silverlight.Controls.PivotGrid.Resources;
using Syncfusion.Silverlight.Controls.PivotSchemaDesigner;
using System.Windows.Data;
namespace Syncfusion.Silverlight.Controls.PivotGrid
#else

using System.Data;
using Syncfusion.Windows.Controls.PivotSchemaDesigner;
using Syncfusion.Windows.Controls.PivotGrid.Resources;
using System.Windows.Data;

namespace Syncfusion.Windows.Controls.PivotGrid
#endif
{
    public partial class PivotGridGroupingBar
    {
        #region [ Private Methods ]
#if!SILVERLIGHT
        ResourceWrapperKeys rsKeys = new ResourceWrapperKeys();
#endif

        /// <summary>
        /// Wires the grid events.
        /// </summary>
        private void WireGridEvents()
        {
            if (this.GridControl != null && this.GridControl.InternalGrid != null)
            {
#if SILVERLIGHT
                this.GridControl.InternalGrid.ResizingColumns += new Windows.Controls.Grid.GridResizingColumnsEventHandler(InternalGrid_ResizingColumns);
                this.GridControl.InternalGrid.ResizingRows += new Windows.Controls.Grid.GridResizingRowsEventHandler(InternalGrid_ResizingRows);
                this.GridControl.DataRefreshed +=new DataRefreshed(GridControl_DataRefreshed);
#else
                this.GridControl.InternalGrid.ResizingColumns += new Grid.GridResizingColumnsEventHandler(InternalGrid_ResizingColumns);
#endif
            }
        }

        /// <summary>
        /// Updates the width of the pivot row items.
        /// </summary>
        private void UpdatePivotRowItemsWidth()
        {
            if (this.RowHeaderArea != null)
            {
                for (int i = 0; i < this.Engine.PivotRows.Count; i++)
                {
                    ListBoxItem item = this.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                        item.Width = this.GridControl.InternalGrid.Model.ColumnWidths[i];
                }
            }
        }

        /// <summary>
        /// Wires the PivotGroupingRowItemsControl Events.
        /// </summary>
        private void WirePivotRowListEvents()
        {
#if !SILVERLIGHT
            if (this.RowHeaderArea != null)
            {
                this.RowHeaderArea.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(PivotItemList_PreviewMouseMove);
                this.RowHeaderArea.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(PivotItemList_PreviewMouseLeftButtonDown);
                this.RowHeaderArea.PreviewDragOver += new DragEventHandler(PivotItemList_PreviewDragOver);
                this.RowHeaderArea.PreviewDrop += new DragEventHandler(PivotItemList_PreviewDrop);
                this.RowHeaderArea.PreviewDragLeave += new DragEventHandler(PivotItemList_PreviewDragLeave);
                this.RowHeaderArea.PreviewMouseRightButtonDown += new MouseButtonEventHandler(PivotItemList_PreviewMouseRightButtonDown);
            }
#else
            if (this.RowHeaderArea != null)
            {
                this.RowHeaderArea.CommandExecute += new CommandExecuteChanged(PivotItem_CommandExecute);
                this.RowHeaderArea.ArrangeOverrideExecute += new ArrangeOverrideExecuted(PivotItem_ArrangeOverrideExecute);
                this.RowHeaderArea.LayoutUpdated += new EventHandler(RowHeaderArea_LayoutUpdated);
            }
#endif
        }

        /// <summary>
        /// Wires the PivotGroupingItemsControl Events.
        /// </summary>
        private void WirePivotItemControlEvents()
        {
#if !SILVERLIGHT
            if (this.FilterHeaderArea != null)
            {
                this.FilterHeaderArea.PreviewMouseMove += new MouseEventHandler(PivotItemList_PreviewMouseMove);
                this.FilterHeaderArea.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(PivotItemList_PreviewMouseLeftButtonDown);
                this.FilterHeaderArea.PreviewDragOver += new DragEventHandler(PivotItemList_PreviewDragOver);
                this.FilterHeaderArea.PreviewDrop += new DragEventHandler(FilterBar_PreviewDrop);
                this.FilterHeaderArea.PreviewDragLeave += new DragEventHandler(PivotItemList_PreviewDragLeave);
            }

            if (this.DataHeaderArea != null)
            {
                this.DataHeaderArea.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(PivotItemList_PreviewMouseMove);
                this.DataHeaderArea.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(PivotItemList_PreviewMouseLeftButtonDown);
                this.DataHeaderArea.PreviewDragOver += new DragEventHandler(PivotItemList_PreviewDragOver);
                this.DataHeaderArea.PreviewDrop += new DragEventHandler(PivotItemList_PreviewDrop);
                this.DataHeaderArea.PreviewDragLeave += new DragEventHandler(PivotItemList_PreviewDragLeave);
                this.DataHeaderArea.PreviewMouseRightButtonDown += new MouseButtonEventHandler(PivotItemList_PreviewMouseRightButtonDown);
            }

            if (this.ColumnHeaderArea != null)
            {
                this.ColumnHeaderArea.PreviewMouseMove += new System.Windows.Input.MouseEventHandler(PivotItemList_PreviewMouseMove);
                this.ColumnHeaderArea.PreviewMouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(PivotItemList_PreviewMouseLeftButtonDown);
                this.ColumnHeaderArea.PreviewDragOver += new DragEventHandler(PivotItemList_PreviewDragOver);
                this.ColumnHeaderArea.PreviewDrop += new DragEventHandler(PivotItemList_PreviewDrop);
                this.ColumnHeaderArea.PreviewDragLeave += new DragEventHandler(PivotItemList_PreviewDragLeave);
                this.ColumnHeaderArea.PreviewMouseRightButtonDown += new MouseButtonEventHandler(PivotItemList_PreviewMouseRightButtonDown);
            }
#else
            if (this.FilterHeaderArea != null)
            {
                this.FilterHeaderArea.CommandExecute += new CommandExecuteChanged(PivotItem_CommandExecute);
                this.FilterHeaderArea.ArrangeOverrideExecute += new ArrangeOverrideExecuted(PivotItem_ArrangeOverrideExecute);
                this.FilterHeaderArea.LayoutUpdated += new EventHandler(FilterHeaderArea_LayoutUpdated);
            }

            if (this.DataHeaderArea != null)
            {
                this.DataHeaderArea.ArrangeOverrideExecute += new ArrangeOverrideExecuted(PivotItem_ArrangeOverrideExecute);
                this.DataHeaderArea.LayoutUpdated += new EventHandler(DataHeaderArea_LayoutUpdated);
            }

            if (this.ColumnHeaderArea != null)
            {
                this.ColumnHeaderArea.CommandExecute += new CommandExecuteChanged(PivotItem_CommandExecute);
                this.ColumnHeaderArea.ArrangeOverrideExecute += new ArrangeOverrideExecuted(PivotItem_ArrangeOverrideExecute);
                this.ColumnHeaderArea.LayoutUpdated +=new EventHandler(ColumnHeaderArea_LayoutUpdated);
            }

            if (this.FilterPopup != null)
            {
                this.FilterPopup.Opened += new EventHandler(FilterPopup_Opened);
            }
#endif

            this.GridControl.Filters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(GridControl_Filters_CollectionChanged);
            this.GridControl.PivotColumns.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotColumns_CollectionChanged);
#if SILVERLIGHT

            this.RowHeaderArea.ContainerPrepared += new ContainerPrepared(RowHeaderArea_ContainerPrepared);
            this.ColumnHeaderArea.ContainerPrepared += new ContainerPrepared(ColumnHeaderArea_ContainerPrepared);
            this.FilterHeaderArea.ContainerPrepared += new ContainerPrepared(FilterHeaderArea_ContainerPrepared);
            this.DataHeaderArea.ContainerPrepared += new ContainerPrepared(DataHeaderArea_ContainerPrepared);
            //this.GridControl.DataRefreshed += new DataRefreshed(GridControl_DataRefreshed);  

            this.GridControl.PivotRows.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PivotRows_CollectionChanged);
#endif
        }

        internal bool IsFilterHeaderArea(List<GridFilter> list)
        {
            foreach (var item in list)
            {
                if (item.IsFilterHeaderArea)
                {
                    return true;
                }
                return false;
            }

            return false;
        }

#if SILVERLIGHT

        internal void FormatItems(PivotGroupingItemsControl ctrl)
        {
            if (ctrl.ItemContainerGenerator != null)
            {
                for (int i = 0; i < ctrl.Items.Count; i++)
                {
                    ListBoxItem item = ctrl.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        this.ApplyBorderColor(item);
                    }
                }
            }
        }

#endif
        /// <summary>
        /// Shows the placement indicator popup.
        /// </summary>
        private void ShowPlacementIndicatorPopup()
        {
#if !SILVERLIGHT
            if (!this.IndicatorPopup.IsOpen)
            {
                if (shouldShowBouncingArrows)
                {
                    UpAnimatedGrid.Begin();
                    DownAnimatedGrid.Begin();
                    CrossAnimatedGrid.Stop();
                    DownAnimatedGrid.Visibility = UpAnimatedGrid.Visibility = System.Windows.Visibility.Visible;
                    CrossAnimatedGrid.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    CrossAnimatedGrid.Begin();
                    UpAnimatedGrid.Stop();
                    DownAnimatedGrid.Stop();
                    DownAnimatedGrid.Visibility = UpAnimatedGrid.Visibility = System.Windows.Visibility.Hidden;
                    CrossAnimatedGrid.Visibility = System.Windows.Visibility.Visible;
                }
                this.IndicatorPopup.IsOpen = true;
            }
#endif
        }

        /// <summary>
        /// Closes the placement indicator popup.
        /// </summary>
        private void ClosePlacementIndicatorPopup()
        {
#if !SILVERLIGHT
            UpAnimatedGrid.Stop();
            DownAnimatedGrid.Stop();
            CrossAnimatedGrid.Stop();
            this.IndicatorPopup.IsOpen = false;
#endif
        }


#if !SILVERLIGHT
        /// <summary>
        /// Gets the indicator popup.
        /// </summary>
        /// <returns></returns>
        private Popup GetIndicatorPopup()
        {

            System.Windows.Controls.Grid gridPanel = new System.Windows.Controls.Grid();
            gridPanel.RowDefinitions.Add(new RowDefinition());
            gridPanel.RowDefinitions.Add(new RowDefinition());
            gridPanel.Children.Add(DownAnimatedGrid);
            gridPanel.Children.Add(UpAnimatedGrid);
            System.Windows.Controls.Grid.SetRow(DownAnimatedGrid, 0);
            System.Windows.Controls.Grid.SetRow(UpAnimatedGrid, 1);
            gridPanel.Children.Add(CrossAnimatedGrid);
            System.Windows.Controls.Grid.SetRow(CrossAnimatedGrid, 0);
            return new Popup()
            {
                AllowsTransparency = true,
                Child = gridPanel,
                Height = 50,
                StaysOpen = false,
            };
        }

#endif

        internal void ApplyTemplateToGroupingControl(ListBox dropTarget, object data)
        {
            if (dropTarget.Name == "PART_ColumnList")
            {

#if SILVERLIGHT
                if (this.ColumnHeaderArea.Items.Count > 0)
                {
                    if (this.ColumnHeaderArea.Items[0] is TextBlock)
                    {
                        this.ColumnHeaderArea.Items.Clear();
                    }
                }

#endif

                this.ColumnHeaderArea.ItemsSource = null;
#if !SILVERLIGHT
                this.ColumnHeaderArea.Items.Clear();
#endif
#if SILVERLIGHT
                this.GridControl.PivotColumns.Insert(this.ItemIndex, data as PivotItem);
#else
                this.GridControl.PivotColumns.Add(data as PivotItem);
#endif
                this.ColumnHeaderArea.DataContext = this.GridControl.PivotColumns;
                this.ColumnHeaderArea.ItemsSource = this.GridControl.PivotColumns;

                ResourceDictionary resource = new ResourceDictionary()
                {
#if !SILVERLIGHT
                    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                    Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                };

                this.ColumnHeaderArea.ItemTemplate = resource["PivotColumnItemTemplate"] as DataTemplate;

#if SILVERLIGHT
                this.GridControl.IgnoreRefesh = false;
                this.GridControl.RefreshFromGroupingBar = true;
                this.GridControl.InternalRefresh();
#endif
            }

            else if (dropTarget.Name == "PART_RowList")
            {
#if SILVERLIGHT
                if (this.RowHeaderArea.Items.Count > 0)
                {
                    if (this.RowHeaderArea.Items[0] is TextBlock)
                    {
                        this.RowHeaderArea.Items.Clear();
                    }
                }

#endif

                this.RowHeaderArea.ItemsSource = null;
#if !SILVERLIGHT
                this.RowHeaderArea.Items.Clear();
#endif

#if SILVERLIGHT
                this.GridControl.PivotRows.Insert(this.ItemIndex, data as PivotItem);
#else
                this.GridControl.PivotRows.Add(data as PivotItem);
#endif
                this.RowHeaderArea.DataContext = this.GridControl.PivotRows;
                this.RowHeaderArea.ItemsSource = this.GridControl.PivotRows;

                ResourceDictionary resource = new ResourceDictionary()
                {
#if !SILVERLIGHT
                    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                    Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                };

                this.RowHeaderArea.ItemTemplate = resource["PivotRowItemTemplate"] as DataTemplate;

#if SILVERLIGHT
                this.GridControl.IgnoreRefesh = false;
                this.GridControl.RefreshFromGroupingBar = true;
                this.GridControl.InternalRefresh();
#endif
            }

            else if (dropTarget.Name == "PART_FilterList")
            {

#if SILVERLIGHT
                if (this.FilterHeaderArea.Items.Count > 0)
                {
                    if (this.FilterHeaderArea.Items[0] is TextBlock)
                    {
                        this.FilterHeaderArea.Items.Clear();
                    }
                }

#endif

                this.FilterHeaderArea.ItemsSource = null;
#if !SILVERLIGHT
                this.FilterHeaderArea.Items.Clear();
#endif
                this.FilterHeaderArea.ItemsSource = this.Filters;

                ResourceDictionary resource = new ResourceDictionary()
                {
#if !SILVERLIGHT
                    Source = new Uri("/Syncfusion.PivotAnalysis.Wpf;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#else
                    Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
#endif
                };

                this.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;
            }
        }

#if !SILVERLIGHT

        /// <summary>
        /// Breaks the function if dropTarget and dropSource is Equal if the itemIndex is Changed
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="data">The data.</param>
        /// <param name="m_DragSource">The m_ drag source.</param>
        /// <param name="dropTarget">The drop target.</param>
        /// <returns></returns>
        private bool BreakFunction(ToggleButton button, object data, ListBox m_DragSource, ListBox dropTarget)
        {

            if (m_DragSource == dropTarget)
            {
                if (button != null)
                {
                    if (data == button.Tag)
                    {
                        return true;
                    }

                    int itemIndex = ((IList)m_DragSource.ItemsSource).IndexOf(data);
                    int siblingIndex = ((IList)m_DragSource.ItemsSource).IndexOf(button.Tag);

                    if (siblingIndex + 1 == itemIndex)
                    {
                        if (this.IndicatorPopup.Placement == PlacementMode.Left)
                            return false;
                    }
                }
            }
            return false;
        }

#endif

#if !SILVERLIGHT
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
            if (fieldType == typeof(int) || fieldType == typeof(float) || fieldType == typeof(double) || fieldType == typeof(decimal))
                return "#.##"; // default format for computational info objects.
            else
                return null; //default format for pivot item objects
        }

        /// <summary>
        /// Creates a filterItem based on PropertyDesciptor supplied
        /// </summary>
        /// <param name="propertyDescriptor"></param>
        /// <returns></returns>
        private FilterItemsCollection GetFilterItem(PropertyDescriptor propertyDescriptor)
        {
            return GetFilterItem(propertyDescriptor, null);
        }

        private FilterItemsCollection GetFilterItem(PropertyDescriptor propertyDescriptor, string format)
        {
            string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
            FilterItemsCollection filterItemsCollection = new FilterItemsCollection();
            IEnumerable itemSource = null;
            if (this.GridControl.ItemSource is System.Data.DataTable)
                itemSource = (this.GridControl.ItemSource as System.Data.DataTable).DefaultView;
            else
                itemSource = this.GridControl.ItemSource as IEnumerable;
            if (itemSource != null && !(itemSource is DataView))
            {
                foreach (var item in itemSource)
                {
                    filterItemsCollection.FilterProperty = propertyDescriptor;
                    if (propertyDescriptor.PropertyType == typeof(int) ||
                        propertyDescriptor.PropertyType == typeof(double) ||
                        propertyDescriptor.PropertyType == typeof(float) ||
                        propertyDescriptor.PropertyType == typeof(decimal) ||
                        propertyDescriptor.PropertyType == typeof(short) ||
                        propertyDescriptor.PropertyType == typeof(DateTime) ||
                        propertyDescriptor.PropertyType == typeof(long))
                    {
                        bool allow = this.GridControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < this.GridControl.PivotEngine.Filters.Count; i++)
                        {
                            allow = bool.Parse(this.GridControl.PivotEngine.Filters[i].Evaluator.DynamicInvoke(item).ToString());
                        }
                        if (allow)
                        {
                            if (format == null)
                            {
                                filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });
                            }
                            else
                            {
                                filterItemsCollection.AddIfUnique(new FilterItemElement { Key = string.Format(fmt, propertyDescriptor.GetValue(item)) });
                            }
                        }
                    }

                    else
                    {
                        bool allow = this.GridControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < this.GridControl.PivotEngine.Filters.Count; i++)
                        {
                            if (this.GridControl.PivotEngine.Filters[i].Evaluator != null)
                                allow = bool.Parse(this.GridControl.PivotEngine.Filters[i].Evaluator.DynamicInvoke(item).ToString());
                            else
                                allow = true;
                        }
                        if (allow)
                        {
                            if (propertyDescriptor is ExpressionPropertyDescriptor)
                            {
                                string s = propertyDescriptor.GetValue(item).ToString();
                                filterItemsCollection.AddIfUnique(new FilterItemElement { Key = s });
                            }
                            else
                                filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item) as string });
                        }
                    }
                }
                filterItemsCollection.DisplayHeader = GetPivotTableFieldHeader(propertyDescriptor.Name);
                filterItemsCollection.Format = format;
            }
            else
            {
                if (this.GridControl.ItemSource is DataTable || this.GridControl.ItemSource is DataView)
                {
                    DataView source = this.GridControl.ItemSource is DataView ? ((DataView)this.GridControl.ItemSource) : ((DataTable)this.GridControl.ItemSource).DefaultView;

                    string s = "";
                    if (!string.IsNullOrEmpty(source.RowFilter))
                        s = source.RowFilter;
                    foreach (FilterExpression exp in this.GridControl.PivotEngine.Filters)
                    {
                        if (s.Length > 0)
                        {
                            s += " AND " + "(" + exp.Expression + ")";
                        }
                        else
                        {
                            s = "(" + exp.Expression + ")";
                        }
                    }
                    using (DataView dv = new DataView(source.Table, s, "", DataViewRowState.CurrentRows))
                    {
                        foreach (var item in dv)
                        {
                            filterItemsCollection.FilterProperty = propertyDescriptor;
                            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });

                        }
                    }

                    ////IEnumerable source = this.GridControl.ItemSource is DataView ? ((DataView) this.GridControl.ItemSource) : ((DataTable)this.GridControl.ItemSource).DefaultView as IEnumerable;

                    ////foreach (var item in source)
                    ////{
                    ////    filterItemsCollection.FilterProperty = propertyDescriptor;
                    ////    if (propertyDescriptor.PropertyType == typeof(int) ||
                    ////        propertyDescriptor.PropertyType == typeof(double) ||
                    ////        propertyDescriptor.PropertyType == typeof(float) ||
                    ////        propertyDescriptor.PropertyType == typeof(decimal))
                    ////    {
                    ////        bool allow = this.GridControl.PivotEngine.Filters.Count == 0 ? true : false;
                    ////        for (int i = 0; i < this.GridControl.PivotEngine.Filters.Count; i++)
                    ////        {
                    ////            allow = bool.Parse(this.GridControl.PivotEngine.Filters[i].Evaluator.DynamicInvoke(item).ToString());
                    ////        }
                    ////        if (allow)
                    ////            filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item).ToString() });
                    ////    }
                    ////    else
                    ////    {
                    ////        bool allow = this.GridControl.PivotEngine.Filters.Count == 0 ? true : false;
                    ////        for (int i = 0; i < this.GridControl.PivotEngine.Filters.Count; i++)
                    ////        {
                    ////            allow = bool.Parse(this.GridControl.PivotEngine.Filters[i].Evaluator.DynamicInvoke(item).ToString());
                    ////        }
                    ////        if (allow)
                    ////        filterItemsCollection.AddIfUnique(new FilterItemElement { Key = propertyDescriptor.GetValue(item) as string });
                    ////    }
                    ////}
                }

            }
            if (filterItemsCollection != null && filterItemsCollection.Count > 0 && filterItemsCollection[0].Key != "(All)")
            {
                  filterItemsCollection.Reverse();
            }
                  
           return filterItemsCollection;
            
        }
        private FilterItemsCollection GetFilterItemsForDateTime(PropertyDescriptor descriptor, string format)
        {
            string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
            FilterItemsCollection filterItemsCollection = new FilterItemsCollection();
            IEnumerable itemSource = null;
            if (this.GridControl.ItemSource is System.Data.DataTable)
                itemSource = (this.GridControl.ItemSource as System.Data.DataTable).DefaultView;
            else
                itemSource = this.GridControl.ItemSource as IEnumerable;
            if (itemSource != null && !(itemSource is DataView))
            {
                foreach (var item in itemSource)
                {
                    filterItemsCollection.FilterProperty = descriptor;
                    if (descriptor.PropertyType == typeof(DateTime))
                    {
                         filterItemsCollection.AddIfUnique(new FilterItemElement { Key = descriptor.GetValue(item).ToString() });
                    }
                }
                filterItemsCollection.DisplayHeader = GetPivotTableFieldHeader(descriptor.Name);
                filterItemsCollection.Format = format;
            }
            if (filterItemsCollection != null && filterItemsCollection.Count > 0 && filterItemsCollection[0].Key != "(All)")
            {
                filterItemsCollection.Reverse();
            }
            return filterItemsCollection; 
        }

#else
        /// <summary>
        /// Creates a filterItem based on PropertyDescriptor supplied
        /// </summary>
        /// <param name="propertyInfo">An object of type PropertyInfo or ExpressionPropertyDescriptor.</param>
        /// <returns>A collection of filter items of respective property.</returns>
        private FilterItemsCollection GetFilterItem(object propertyInfo)
        {
            return GetFilterItem(propertyInfo, null);
        }

        /// <summary>
        /// Creates a filterItem based on PropertyDescriptor supplied
        /// </summary>
        /// <param name="propertyInfo">An object of type PropertyInfo or ExpressionPropertyDescriptor.</param>
        /// <param name="format">formatstring</param>
        /// <returns>A collection of filter items of respective property.</returns>
        private FilterItemsCollection GetFilterItem(object propertyInfo, string format)
        {
            string fmt = format != null && format.Length > 0 ? string.Format("{{0:{0}}}", format) : null;
            FilterItemsCollection filterList = new FilterItemsCollection();
            IEnumerable itemSource = this.GridControl.ItemSource as IEnumerable;
            if (itemSource != null)
            {
                foreach (var item in itemSource)
                {
                    filterList.FilterProperty = propertyInfo;
                    Type type = typeof(object);
                    if (propertyInfo is PropertyInfo)
                    {
                        type = (propertyInfo as PropertyInfo).PropertyType;
                    }
                    else if (propertyInfo is ExpressionPropertyDescriptor)
                    {
                        type = (propertyInfo as ExpressionPropertyDescriptor).PropertyType;
                    }
                    if (type == typeof(int) ||
                        type == typeof(double) ||
                        type == typeof(float) ||
                        type == typeof(decimal))
                    {
                        bool allow = this.GridControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < this.GridControl.PivotEngine.Filters.Count; i++)
                        {
                            Delegate evaluator = this.GridControl.PivotEngine.Filters[i].Evaluator;
                            if (evaluator != null)
                                allow = bool.Parse(evaluator.DynamicInvoke(item).ToString());
                        }
                        if (allow)
                        {
                            if (propertyInfo is PropertyInfo)
                            {
                                if (format == null)
                                {
                                    filterList.AddIfUnique(new FilterItemElement { Key = (propertyInfo as PropertyInfo).GetValue(item, null).ToString() });
                                }
                                else
                                {
                                    filterList.AddIfUnique(new FilterItemElement { Key = string.Format(fmt, (propertyInfo as PropertyInfo).GetValue(item, null)) });
                                }
                            }
                        }
                    }
                    else if (type == typeof(DateTime))
                    {
                        bool allow = this.GridControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < this.GridControl.PivotEngine.Filters.Count; i++)
                        {
                            if (((PivotItem)CurrentItemForFilter).Format != this.GridControl.PivotEngine.Filters[i].Format)
                                allow = true;
                            else
                            {
                                Delegate evaluator = this.GridControl.PivotEngine.Filters[i].Evaluator;
                                if (evaluator != null)
                                    allow = bool.Parse(evaluator.DynamicInvoke(item).ToString());
                            }
                        }
                        if (allow)
                        {
                            if (propertyInfo is PropertyInfo)
                            {
                                if (format == null)
                                {
                                    filterList.AddIfUnique(new FilterItemElement { Key = (propertyInfo as PropertyInfo).GetValue(item, null).ToString() });
                                }
                                else
                                {
                                    filterList.AddIfUnique(new FilterItemElement { Key = string.Format(fmt, (propertyInfo as PropertyInfo).GetValue(item, null)) });
                                }
                            }
                        }
                    }
                    else
                    {
                        bool allow = this.GridControl.PivotEngine.Filters.Count == 0 ? true : false;
                        for (int i = 0; i < this.GridControl.PivotEngine.Filters.Count; i++)
                        {
                            Delegate evaluator = this.GridControl.PivotEngine.Filters[i].Evaluator;
                            if (evaluator != null)
                                allow = bool.TryParse(evaluator.DynamicInvoke(item).ToString(), out allow);
                        }
                        if (allow)
                        {

                            if (this.GridControl.PivotEngine.Filters.Count() > 0)
                            {
                                if (CurrentItemForFilter is PivotItem && this.GridControl.PivotRows.Any(x => x.FieldMappingName == ((PivotItem)CurrentItemForFilter).FieldMappingName))
                                {
                                    int rowLevel = this.GridControl.PivotRows.IndexOf(this.GridControl.PivotRows.FirstOrDefault(x => x.FieldMappingName == ((PivotItem)CurrentItemForFilter).FieldMappingName));
                                    int startRow = (this.GridControl.PivotEngine.PivotColumns.Count != 0 ? this.GridControl.PivotEngine.PivotColumns.Count : 0) + (this.GridControl.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);
                                    int rowCount = this.GridControl.PivotEngine.ShowGrandTotals ? this.GridControl.PivotEngine.RowCount - 1 : this.GridControl.PivotEngine.RowCount;
                                    for (int i = startRow; i < rowCount; i++)
                                    {
                                        PivotCellInfo pci = this.GridControl.PivotEngine[i, rowLevel];
                                        if (pci.FormattedText != null && pci.FormattedText != "" && pci.FormattedText != "x" && !pci.FormattedText.Contains(this.GridControl.PivotEngine.GrandString) && pci.CellType != (PivotCellType.RowHeaderCell | PivotCellType.TotalCell))
                                        {
                                            filterList.AddIfUnique(new FilterItemElement { Key = pci.FormattedText });
                                        }
                                    }
                                }
                                else if (CurrentItemForFilter is PivotItem && this.GridControl.PivotColumns.Any(x => x.FieldMappingName == ((PivotItem)CurrentItemForFilter).FieldMappingName))
                                {
                                    int colLevel = this.GridControl.PivotColumns.IndexOf(this.GridControl.PivotColumns.FirstOrDefault(x => x.FieldMappingName == ((PivotItem)CurrentItemForFilter).FieldMappingName));
                                    int startColumn = (this.GridControl.PivotEngine.PivotRows.Count != 0 ? this.GridControl.PivotEngine.PivotRows.Count : 0) + (this.GridControl.PivotEngine.PivotCalculations.Count > 1 ? 1 : 0);
                                    int columnCount = this.GridControl.PivotEngine.ShowGrandTotals ? this.GridControl.PivotEngine.ColumnCount - 1 : this.GridControl.PivotEngine.ColumnCount;
                                    for (int i = startColumn; i < columnCount; i++)
                                    {
                                        PivotCellInfo pci = this.GridControl.PivotEngine[colLevel, i];
                                        if (pci.FormattedText != null && pci.FormattedText != "" && pci.FormattedText != "x" && !pci.FormattedText.Contains(this.GridControl.PivotEngine.GrandString) && pci.CellType != (PivotCellType.ColumnHeaderCell | PivotCellType.TotalCell))
                                        {
                                            filterList.AddIfUnique(new FilterItemElement { Key = pci.FormattedText });
                                        }
                                    }
                                }
                            }
                            else if (propertyInfo is PropertyInfo)
                                filterList.AddIfUnique(new FilterItemElement { Key = (propertyInfo as PropertyInfo).GetValue(item, null) as string });
                            else if (propertyInfo is ExpressionPropertyDescriptor)
                            {
                                string s = (propertyInfo as ExpressionPropertyDescriptor).GetValue(item).ToString();
                                filterList.AddIfUnique(new FilterItemElement { Key = s });
                            }
                        }
                    }
                }
            }
            filterList.Reverse();
            if (filterList[0].Key != "(All)" && filterList.Any(x => x.Key == "(All)"))
            {
                FilterItemElement item = filterList.FirstOrDefault(x => x.Key == "(All)");
                if (item != null)
                {
                    filterList.Remove(item);
                    filterList.Insert(0, item);
                }
            }
            return filterList;
        }
#endif

        /// <summary>
        /// Creates a pivot calculation and add's to the calculation list
        /// </summary>
        /// <param name="index">position at which to insert pivot item</param>
        /// <param name="pivotItem">pivot item to insert</param>
        private void InsertCalculationInfo(int index, PivotItem pivotItem)
        {
#if !SILVERLIGHT
            PivotComputationInfo compInfo = new PivotComputationInfo();
            compInfo.FieldName = pivotItem.FieldMappingName;
            compInfo.FieldHeader = pivotItem.FieldHeader;
            compInfo.Format = pivotItem.Format;
            compInfo.AllowRunTimeGroupByField = pivotItem.AllowRunTimeGroupByField;
            PropertyDescriptor propDescriptor = GetPropertyDescriptor(pivotItem.FieldMappingName);

            if (pivotItem.SummaryType == SummaryType.Custom)
            {
                compInfo.Format = pivotItem.Format;
                compInfo.Summary = pivotItem.Summary;
                compInfo.SummaryType = SummaryType.Custom;
            }

            else
            {
                if (propDescriptor != null)
                {
                    if (propDescriptor.PropertyType == typeof(int))
                        compInfo.SummaryType = SummaryType.IntTotalSum;
                    else if (propDescriptor.PropertyType == typeof(double) || propDescriptor.PropertyType == typeof(float))
                        compInfo.SummaryType = SummaryType.DoubleTotalSum;
                    else if (propDescriptor.PropertyType == typeof(decimal))
                        compInfo.SummaryType = SummaryType.DecimalTotalSum;

                    else
                    {
                        compInfo.SummaryType = SummaryType.Count;
                    }
                }
            }
            this.GridControl.IgnoreRefesh = false;
            this.GridControl.PivotCalculations.Insert(index, compInfo);

#endif
        }

        /// <summary>
        /// Creates a pivot calculation and add's to the calculation list
        /// </summary>
        /// <param name="pivotItem"></param>
        private void AddComputationInfo(PivotItem pivotItem)
        {
            PivotComputationInfo compInfo = new PivotComputationInfo();
            compInfo.FieldName = pivotItem.FieldMappingName;
            compInfo.FieldHeader = pivotItem.FieldHeader;
            compInfo.Format = pivotItem.Format;
            compInfo.AllowRunTimeGroupByField = pivotItem.AllowRunTimeGroupByField;

#if SILVERLIGHT
            object propInfo = GetPropertyDescriptor(pivotItem.FieldMappingName);
            if (propInfo != null)
            {
                Type type = typeof(object);
                if (propInfo is PropertyInfo)
                {
                    PropertyInfo info = propInfo as PropertyInfo;
                    type = info.PropertyType;
                }
                else if (propInfo is ExpressionPropertyDescriptor)
                {
                    ExpressionPropertyDescriptor expDesc = propInfo as ExpressionPropertyDescriptor;
                    type = expDesc.PropertyType;
                }
                if (type == typeof(int))
                    compInfo.SummaryType = SummaryType.IntTotalSum;
                else if (type == typeof(double) || type == typeof(float))
                    compInfo.SummaryType = SummaryType.DoubleTotalSum;
                else if (type == typeof(decimal))
                    compInfo.SummaryType = SummaryType.DecimalTotalSum;
                else
                {
                    compInfo.SummaryType = SummaryType.Count;
                }

                //this.GridControl.IgnoreRefesh = false;
                this.GridControl.PivotCalculations.Insert(this.ItemIndex, compInfo);
                this.GridControl.IgnoreRefesh = false;
                this.GridControl.RefreshFromGroupingBar = true;
                this.GridControl.InternalRefresh();
            }

#else
            PropertyDescriptor propDescriptor = GetPropertyDescriptor(pivotItem.FieldMappingName);
            if (propDescriptor != null)
            {

                if (propDescriptor.PropertyType == typeof(int))
                    compInfo.SummaryType = SummaryType.IntTotalSum;
                else if (propDescriptor.PropertyType == typeof(double) || propDescriptor.PropertyType == typeof(float))
                    compInfo.SummaryType = SummaryType.DoubleTotalSum;
                else if (propDescriptor.PropertyType == typeof(decimal))
                    compInfo.SummaryType = SummaryType.DecimalTotalSum;
                else
                {
                    compInfo.SummaryType = SummaryType.Count;
                }

                this.GridControl.IgnoreRefesh = false;
                this.GridControl.PivotCalculations.Add(compInfo);

            }
#endif
        }

#if !SILVERLIGHT

        /// <summary>
        /// Gets the property descriptor based on FieldName
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        internal PropertyDescriptor GetPropertyDescriptor(string fieldName)
        {

            if (this.GridControl.ItemSource != null && this.GridControl.PivotEngine != null && this.GridControl.PivotEngine.ItemProperties != null)
            {
                return this.GridControl.PivotEngine.ItemProperties[fieldName];
            }
            else
                return null;

            //IEnumerable source = this.GridControl.ItemSource as IEnumerable;
            //if (source != null)
            //{
            //    object firstItem = null;
            //    foreach (object item in source)
            //    {
            //        firstItem = item;
            //        break;
            //    }
            //    if (firstItem != null)
            //    {
            //        var pdCollection = TypeDescriptor.GetProperties(firstItem);
            //        return pdCollection.Cast<PropertyDescriptor>().Where(p => p.Name == fieldName).FirstOrDefault();
            //    }
            //}
            //return null;
        }

# else
        internal object GetPropertyDescriptor(string fieldName)
        {
            if (this.GridControl.PivotEngine != null && this.GridControl.PivotEngine.ItemProperties != null)
            {
                return this.GridControl.PivotEngine.ItemProperties[fieldName];
            }
            else
                return null;
        }
#endif

        private int[] MoveItem(string parameter, IEnumerable source, object Tag)
        {
            int index = 0;
            switch (parameter)
            {
                case "Move to Beginning":
                    index = ((IList)source).IndexOf(Tag);
                    return new int[] { index, 0 };
                case "Move to Left":
                    index = ((IList)source).IndexOf(Tag);
                    return new int[] { index, index - 1 };
                case "Move to Right":
                    index = ((IList)source).IndexOf(Tag);
                    return new int[] { index, index + 1 };
                case "Move to End":
                    index = ((IList)source).IndexOf(Tag);
                    return new int[] { index, ((IList)source).Count - 1 };
            }

            return new int[] { 0, 0 };
        }

#if SILVERLIGHT

        private int GetItemIndex(ToggleButton item, PivotGroupingItemsControl pivotGroupingItemsControl, object currItem, out int currentItemIndex)
        {
            if (pivotGroupingItemsControl != null)
            {
                currentItemIndex = ((IList)pivotGroupingItemsControl.ItemsSource).IndexOf(currItem);
                return ((IList)pivotGroupingItemsControl.ItemsSource).IndexOf(item.Tag);
            }
            else
                currentItemIndex = -3;

            return -1;
        }

        private void ApplyBorderColor(ListBoxItem item)
        {
            Border brdItem = Common.FindVisualChildWithName<Border>(item, "brdItem");
            TextBlock text = Common.FindVisualChildWithName<TextBlock>(item, "PART_contentTxt");
            TextBlock text1 = Common.FindVisualChildWithName<TextBlock>(item, "contentTxt");
            TextBlock text2 = Common.FindVisualChildWithName<TextBlock>(item, "filtercontentTxt");

            if (brdItem != null)
            {
                brdItem.Background = this.GridControl.GroupingBarItemBackground;
                brdItem.BorderBrush = this.GridControl.GroupingBarItemBorderBrush;

            }

            if (text != null)
            {
                text.Foreground = this.GridControl.GroupingBarItemForeground;
            }
            if (text1 != null)
            {
                text1.Foreground = this.GridControl.GroupingBarItemForeground;
            }
            if (text2 != null)
            {
                text2.Foreground = this.GridControl.GroupingBarItemForeground;
            }

            if (brdItem != null)
            {
                brdItem.MouseEnter += new MouseEventHandler(brdItem_MouseEnter);
                brdItem.MouseLeave += new MouseEventHandler(brdItem_MouseLeave);
                brdItem.MouseLeftButtonDown += new MouseButtonEventHandler(brdItem_MouseLeftButtonDown);
                brdItem.MouseLeftButtonUp += new MouseButtonEventHandler(brdItem_MouseLeftButtonUp);
            }
           
            
            this.EnsureFilteringAndSorting(item);
        }

        void brdItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            TextBlock text = Common.FindVisualChildWithName<TextBlock>(border, "PART_contentTxt");
            TextBlock text1 = Common.FindVisualChildWithName<TextBlock>(border, "contentTxt");
            TextBlock text2 = Common.FindVisualChildWithName<TextBlock>(border, "filtercontentTxt");
            Button filterBtn = Common.FindVisualChildWithName<Button>(border, "filterBtn");
            Path colSortPath = Common.FindVisualChildWithName<Path>(border, "colSortPath");
            Path rowSortPath = Common.FindVisualChildWithName<Path>(border, "rowSortPath");

            if (border != null)
            {
                border.Background = this.GridControl.GroupingBarItemBackground;
                border.BorderBrush = this.GridControl.GroupingBarItemBorderBrush;
            }
            if (text != null)
            {
                text.Foreground = this.GridControl.GroupingBarItemForeground;
            }
            if (text1 != null)
            {
                text1.Foreground = this.GridControl.GroupingBarItemForeground;
            }
            if (text2 != null)
            {
                text2.Foreground = this.GridControl.GroupingBarItemForeground;
            }
            if (filterBtn != null)
            {
                filterBtn.Foreground = this.GridControl.GroupingBarItemForeground;
            }
            if (colSortPath != null)
            {
                colSortPath.Fill = this.GridControl.GroupingBarItemForeground;
            }
            if (rowSortPath != null)
            {
                rowSortPath.Fill = this.GridControl.GroupingBarItemForeground;
            }
            //throw new NotImplementedException();
        }

        void brdItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {            
            Border border = sender as Border;
            TextBlock text = Common.FindVisualChildWithName<TextBlock>(border, "PART_contentTxt");
            TextBlock text1 = Common.FindVisualChildWithName<TextBlock>(border, "contentTxt");
            TextBlock text2 = Common.FindVisualChildWithName<TextBlock>(border, "filtercontentTxt");
            Button filterBtn = Common.FindVisualChildWithName<Button>(border, "filterBtn");
            Path colSortPath = Common.FindVisualChildWithName<Path>(border, "colSortPath");
            Path rowSortPath = Common.FindVisualChildWithName<Path>(border, "rowSortPath");

            if (border != null)
            {
                border.Background = this.GridControl.GroupingButtonCheckedBackground;               
            }
            if (text != null)
            {
                text.Foreground = this.GridControl.GroupingBarItemForeground;
            }
            if (text1 != null)
            {
                text1.Foreground = this.GridControl.GroupingButtonCheckedForeground;
            }
            if (text2 != null)
            {
                text2.Foreground = this.GridControl.GroupingButtonCheckedForeground;
            }
            if (filterBtn != null)
            {
                filterBtn.Foreground = this.GridControl.GroupingButtonCheckedForeground;
            }
            if (colSortPath != null)
            {
                colSortPath.Fill = this.GridControl.GroupingButtonCheckedForeground;
            }
            if (rowSortPath != null)
            {
                rowSortPath.Fill = this.GridControl.GroupingButtonCheckedForeground;
            }

        }

        void brdItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Border bd = sender as Border;
            TextBlock text1 = Common.FindVisualChildWithName<TextBlock>(bd, "contentTxt");
            bd.Background = this.GridControl.GroupingBarItemBackground;
            bd.BorderBrush = this.GridControl.GroupingBarItemBorderBrush;
            if(text1 != null)
            {
                 text1.Foreground = this.GridControl.GroupingBarItemForeground;
            }
        }

        void brdItem_MouseEnter(object sender, MouseEventArgs e)
        {
            Border bd = sender as Border;
            bd.Background = this.GridControl.GroupingButtonHoverBackgroundBrush;
            bd.BorderBrush = this.GridControl.GroupingButtonHoverBorderBrush;
        }

        private void EnsureFilteringAndSorting(ListBoxItem item)
        {
            Button filterBtn = Common.FindVisualChildWithName<Button>(item, "filterBtn");
            Path colSortPath = Common.FindVisualChildWithName<Path>(item, "colSortPath");
            Path rowSortPath = Common.FindVisualChildWithName<Path>(item, "rowSortPath");
            this.FilterHeaderArea.BorderBrush = this.GridControl.GridOuterBorderBrush;
          
            if (rowSortPath != null)
                rowSortPath.Fill = this.GridControl.GroupingBarItemForeground;
            if (colSortPath != null)
                colSortPath.Fill = this.GridControl.GroupingBarItemForeground;
            if (filterBtn != null)
            {
                
                if (this.AllowFiltering)
                    filterBtn.Visibility = System.Windows.Visibility.Visible;
                else
                {
                    filterBtn.Visibility = System.Windows.Visibility.Collapsed;

                    PivotGroupingItemsControl filterItemsControl = Common.GetParentElement<PivotGroupingItemsControl>(item);
                    if (filterItemsControl.Name == "PART_FilterList")
                    {
                        TextBlock txtBlock = Common.FindVisualChildWithName<TextBlock>(item, "PART_contentTxt");
                        if (txtBlock != null)
                            txtBlock.Margin = new Thickness(2, 3, 2, 3);
                    }                    
                }
            }

            if (colSortPath != null)
            {
                if (this.AllowSorting)
                    colSortPath.Visibility = System.Windows.Visibility.Visible;
                else
                {
                    colSortPath.Visibility = System.Windows.Visibility.Collapsed;

                    TextBlock txtBlock = Common.FindVisualChildWithName<TextBlock>(item, "PART_contentTxt");
                    if (txtBlock != null)
                        txtBlock.Margin = new Thickness(2, 3, 2, 3);
                }
            }

            if (rowSortPath != null)
            {
                if (this.AllowSorting)
                    rowSortPath.Visibility = System.Windows.Visibility.Visible;
                else
                    rowSortPath.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void AddFilterItem(PivotGroupingItemsControl pivotGroupingItemsControl)
        {
            FilterItemsCollection filterItem = DraggedItem as FilterItemsCollection;
            //// Removing the filter item entry from the filters collection
            RemoveFilterItem(filterItem);
            //// Generating a PivotItem based on FilterItemsCollection
            PivotItem pivotItem = GetPivotItem(filterItem);
            pivotItem.ShowSubTotal = filterItem.ShowSubTotal;
            if (pivotGroupingItemsControl.Name == this.ColumnHeaderArea.Name)
            {
                if (this.GridControl.PivotColumns.Count == 0)
                {
                    this.ApplyTemplateToGroupingControl(this.ColumnHeaderArea, pivotItem);
                }
                else
                {
                    this.GridControl.PivotColumns.Insert(this.ItemIndex, pivotItem);
                    this.GridControl.IgnoreRefesh = false;
                    this.GridControl.RefreshFromGroupingBar = true;               
                    this.GridControl.InternalRefresh();
                }
            }
            else if (pivotGroupingItemsControl.Name == this.RowHeaderArea.Name)
            {
                if (this.GridControl.PivotRows.Count == 0)
                {
                    this.ApplyTemplateToGroupingControl(this.RowHeaderArea, pivotItem);
                }
                else
                {
                    this.GridControl.PivotRows.Insert(this.ItemIndex, pivotItem);
                    this.GridControl.IgnoreRefesh = false;
                    this.GridControl.RefreshFromGroupingBar = true;
                    this.GridControl.InternalRefresh();
                }
            }
            else if (pivotGroupingItemsControl.Name == this.DataHeaderArea.Name)
            {
                this.AddComputationInfo(pivotItem);
            }
        }
#endif
        internal void ApplyEmptyTemplate()
        {
            if (this.FilterHeaderArea.Items.Count == 0)
            {
                FilterHeaderArea.ItemsSource = null;
                FilterHeaderArea.ItemTemplate = null;
#if SILVERLIGHT
                string filterFieldText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropFilterFields");
#endif
                TextBlock txtBlock = new TextBlock()
                {
#if!SILVERLIGHT
                    Text = rsKeys.pivotGridHeaderDropFilterFileds,
                    Margin = new Thickness(3, 0, 0, 0),
#else
                    Text = filterFieldText,
                    Margin = new Thickness(4, 0, 0, 0),
#endif
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12
                };
                txtBlock.SetBinding(TextBlock.ForegroundProperty, new Binding("GroupingBarItemForeground")
                {
                    RelativeSource = new RelativeSource()
                    {
#if !(SyncfusionFramework4_0 && Silverlight4)
                        AncestorType = typeof(PivotGridControl),
                        Mode = RelativeSourceMode.FindAncestor
#endif
                    },
                });
                this.FilterHeaderArea.Items.Add(txtBlock);
            }

            if (this.ColumnHeaderArea.Items.Count == 0 )
            {
                ColumnHeaderArea.ItemsSource = null;
                ColumnHeaderArea.ItemTemplate = null;
#if SILVERLIGHT
                string columnText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropColumnFields");
#endif
                this.ColumnHeaderArea.Items.Add(new TextBlock()
                {
#if!SILVERLIGHT
                    Text = rsKeys.pivotGridDropColumnFields,
                    Margin = new Thickness(3, 0, 0, 0),
#else
                    Text = columnText,
                    Margin = new Thickness(4, 0, 0, 0),
#endif
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12

                });
            }
            if (this.RowHeaderArea != null && this.RowHeaderArea.Items.Count == 0)
                   {
                RowHeaderArea.ItemsSource = null;
                RowHeaderArea.ItemTemplate = null;
#if SILVERLIGHT
                string rowText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropRowFields");
#endif
                this.RowHeaderArea.Items.Add(new TextBlock()
                {
#if!SILVERLIGHT
                    Text = rsKeys.pivotGridDropRowFields,
                    Margin = new Thickness(3, 0, 0, 0),
#else
                    Text = rowText,
                    Margin = new Thickness(4, 0, 0, 0),
#endif
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 12

                });
            }
        }

#if SILVERLIGHT
        private void PositionPopup()
        {
            this.RowHeaderPopup.VerticalOffset = this.GridControl.m_actualVerticalOffset = GetRowHeaderHeight() - 3;

            if (this.GridControl.PivotRows.Count == 0)
            {
                this.RowHeaderPopup.VerticalOffset += 12;
                this.RowHeaderPopup.Margin = new Thickness(2, 0, 0, 0);
            }
            else
                this.RowHeaderPopup.Margin = new Thickness(2, 0, 0, 0);
        }

        private double GetRowHeaderHeight()
        {
            double width = 0;

            if (this.GridControl.InternalGrid.Model.HeaderRows == 0)
            {
                this.GridControl.InternalGrid.Model.HeaderRows += 1;
            }


            for (int i = 0; i < this.GridControl.InternalGrid.Model.HeaderRows ; i++)
            {
                width += this.GridControl.InternalGrid.Model.RowHeights[i];
            }

            return width == 0.0 ? 13.0 : width += 3.5;
        }   
   
#endif

        #endregion

        #region [ Internal Methods ]

        /// <summary>
        /// Calculates the width of the ComputationInfoPanel.
        /// </summary>
        internal void CalculateComputationInfoWidth()
        {
#if !SILVERLIGHT
            this.ButtonComputation.Background = this.GridControl.GroupingBarItemBackground;
#endif
            this.ComputationInfoWidth = GetTemplateChild("ComputationInfoWidth") as ColumnDefinition;
            this.ComputationButtonWidth = GetTemplateChild("ComputationButtonWidth") as ColumnDefinition;

            if (this.ComputationInfoWidth != null)
            {
                double width = GetColumnHeaderWidth();

                if (this.DataHeaderArea != null)
                {
                    if (!this.GridControl.AllowRowHeaderAreaAutoSizing)
                    {
                        if (this.RowHeaderArea != null && this.RowHeaderArea.Items.Count > 1)
                            this.ComputationButtonWidth.Width = new GridLength(width - (this.RowHeaderArea.Items.Count - 1));
                        else
                            this.ComputationButtonWidth.Width = new GridLength(width);
                        this.ComputationInfoWidth.Width = new GridLength(0);
                        this.ButtonComputation.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.ComputationButtonWidth.Width = new GridLength(0);
                        if (width < DataHeaderArea.Items.Count * 40)
                        {
                            width = DataHeaderArea.Items.Count * 40;

                            if (this.RowHeaderArea != null)
                            {
                                ItemContainerGenerator containerGenerator = this.RowHeaderArea.ItemContainerGenerator;

                                for (int i = 0; i < this.GridControl.PivotRows.Count; i++)
                                {
                                    if (this.GridControl.InternalGrid.Model.ColumnWidths.LineCount > 0)
                                        this.GridControl.InternalGrid.ColumnWidths[i] = width / this.GridControl.PivotRows.Count;
                                    if (containerGenerator != null)
                                    {
                                        ListBoxItem item = containerGenerator.ContainerFromIndex(i) as ListBoxItem;
                                        if (item != null)
                                        {
#if SILVERLIGHT
                                        if (i < (this.GridControl.PivotRows.Count - 1))
                                        {
                                            item.Width = (width / this.GridControl.PivotRows.Count);
                                            if (i == 0)
                                            {
                                                item.Width -= 4;
                                                item.Margin = new Thickness(1, 0, 0, 0);
                                            }
                                        }
                                        else
                                            item.Width = (width / this.GridControl.PivotRows.Count) - 6;
#else
                                            if (i < (this.GridControl.PivotRows.Count - 1))
                                            {
                                                item.Width = (width / this.GridControl.PivotRows.Count);
                                                if (i == 0)
                                                {
                                                    item.Width -= 1;
                                                    item.Margin = new Thickness(1, 0, 0, 0);
                                                }
                                            }
                                            else
                                                item.Width = (width / this.GridControl.PivotRows.Count) - 2;

#endif
                                        }
                                    }
                                }
                            }

                            ItemsPanelTemplate template = this.DataHeaderArea.ItemsPanel;
                        }

                        if (width > 0)
                        {
                            this.ComputationInfoWidth.Width = new GridLength(width);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the width of items in ComputationInfoPanel.
        /// </summary>
        internal void CalculateComputationInfoItemsWidth()
        {
            ItemContainerGenerator generator = this.DataHeaderArea.ItemContainerGenerator;

            int count = this.m_PivotEngine.PivotCalculations.Count;
            double availableSize = this.ComputationInfoWidth.Width.Value;// this.DataHeaderArea.ActualWidth;
            double desiredWidth = 0d;
            for (int i = 0; i < this.m_PivotEngine.PivotCalculations.Count; i++)
            {
                ListBoxItem item = generator.ContainerFromIndex(i) as ListBoxItem;
                if (item != null)
                {
                    PivotComputationInfo compInfo = item.Content as PivotComputationInfo;
                    if (compInfo != null)
                    {
                        double width = Common.GetTextSize(compInfo.FieldName).Width + 25;
                        desiredWidth += width;
                        item.Width = width;
                    }
                }
            }
            if (desiredWidth > availableSize)
            {
                for (int i = 0; i < this.m_PivotEngine.PivotCalculations.Count; i++)
                {
                    ListBoxItem item = generator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null)
                    {
                        ListBoxItem lstItem = item as ListBoxItem;
                        if (lstItem != null)
                        {
                            if (((availableSize / count)) >= 2)
                            {
                                if (this.GridControl.AllowRowHeaderAreaAutoSizing)
                                    lstItem.Width = (availableSize / count) - 2;
                            }
                        }
                    }
                }
            }
        }

        private void CalculateWidth()
        {
            ItemContainerGenerator generator = this.DataHeaderArea.ItemContainerGenerator;

            int count = this.m_PivotEngine.PivotCalculations.Count;
            double availableSize = this.ComputationInfoWidth.Width.Value;// this.DataHeaderArea.ActualWidth;
            double desiredWidth = 0d;
            for (int i = 0; i < this.m_PivotEngine.PivotCalculations.Count; i++)
            {
                ListBoxItem item = generator.ContainerFromIndex(i) as ListBoxItem;
                PivotComputationInfo compInfo = item.Content as PivotComputationInfo;
                if (compInfo != null)
                {
                    double width = Common.GetTextSize(compInfo.FieldName).Width + 15;
                    desiredWidth += width;
                    item.Width = width - 2;
                }
            }
            if (desiredWidth > availableSize)
            {
                for (int i = 0; i < this.m_PivotEngine.PivotCalculations.Count; i++)
                {
                    ListBoxItem item = generator.ContainerFromIndex(i) as ListBoxItem;
                    ListBoxItem lstItem = item as ListBoxItem;
                    if (lstItem != null)
                    {
                        if (((availableSize / count)) >= 0)
                        {
                            lstItem.Width = (availableSize / count) - 2;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the width of the column header.
        /// </summary>
        /// <returns></returns>
        internal double GetColumnHeaderWidth()
        {
            if (this.GridControl != null)
            {
                PivotGridControlBase gridControlBase = this.GridControl.InternalGrid;
                if (gridControlBase != null)
                {
                    double size = 0;
                    int headerCount = this.GridControl.PivotRows.Count;

                    if (this.GridControl.PivotCalculations.Count > 1)
                    {
                        if (!this.GridControl.PivotEngine.ShowCalculationsAsColumns)
                            headerCount++;
                    }

                    if (headerCount > 0)
                    {
                        for (int i = 0; i < headerCount; i++)
                        {
                            size += gridControlBase.Model.ColumnWidths[i];
                        }
                    }
                    else
                    {
                        return gridControlBase.Model.ColumnWidths[0];
                    }

                    return size;
                }
            }
            return 20;
        }


#if !SILVERLIGHT
        internal static ToggleButton GetButtonElement(PivotGroupingItemsControl control, ListBox dropTarget, Point hitPoint)
        {

            HitTestResult result;
            ToggleButton button = null;
            DataTemplate itemTemplate;

            hitPoint.X += 3;
            result = VisualTreeHelper.HitTest(dropTarget, hitPoint);

            if (result != null && result.VisualHit != null)
            {
                button = Common.GetParentElement<ToggleButton>(result.VisualHit);
            }

            if (button == null)
            {
                hitPoint.X -= 3;

                if (hitPoint.X >= 0)
                {
                    result = VisualTreeHelper.HitTest(dropTarget, hitPoint);
                }

                if (result != null && result.VisualHit != null)
                {
                    button = Common.GetParentElement<ToggleButton>(result.VisualHit);
                }

                if (button == null)
                {
                    if (control.Items.Count > 0)
                    {
                        ListBoxItem listBoxItem = (ListBoxItem)(control.ItemContainerGenerator.ContainerFromItem(control.Items[control.Items.Count - 1]));
                        ContentPresenter myContentPresenter = Common.FindVisualChild<ContentPresenter>(listBoxItem);
                        itemTemplate = myContentPresenter.ContentTemplate;
                        if (itemTemplate != null)
                        {
                            button = (ToggleButton)itemTemplate.FindName("toggleButtonItem", myContentPresenter);
                        }
                    }
                }
            }

            return button;
        }

        /// <summary>
        /// Gets the list item based on co-ordinates
        /// </summary>
        /// <param name="source">Source list box</param>
        /// <param name="point">co-ordinates</param>
        /// <returns>object on co-ordinate</returns>
        internal static object GetDataFromListBox(ListBox source, Point point)
        {

            if (source != null)
            {
                UIElement element = source.InputHitTest(point) as UIElement;
                if (element != null)
                {
                    object data = DependencyProperty.UnsetValue;
                    while (data == DependencyProperty.UnsetValue)
                    {
                        data = source.ItemContainerGenerator.ItemFromContainer(element);
                        if (data == DependencyProperty.UnsetValue)
                        {
                            element = VisualTreeHelper.GetParent(element) as UIElement;
                        }
                        if (element == source)
                        {
                            return null;
                        }
                    }
                    if (data != DependencyProperty.UnsetValue)
                    {
                        return data;
                    }
                }
            }
            return null;
        }

#endif

        /// <summary>
        /// Adds the Textblock to the Specified GroupingControl
        /// </summary>
        /// <param name="m_DragSource">The m_ drag source.</param>
        internal void AddEmptyItem(ListBox m_DragSource)
        {
            if (m_DragSource.Name == "PART_ColumnList")
            {
                if (((IList)m_DragSource.ItemsSource).Count == 0)
                {
                    this.ColumnHeaderArea.ItemsSource = null;
                    this.ColumnHeaderArea.ItemTemplate = null;

#if !SILVERLIGHT
                    this.ColumnHeaderArea.Items.Add(new TextBlock()
                    {
                        Text = rsKeys.pivotGridDropColumnFields,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(3, 0, 0, 0),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 12

                    });
#else 
                    string columnText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropColumnFields");
                        this.ColumnHeaderArea.Items.Add(new TextBlock()
                        {
                            Text =columnText,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                            Margin = new Thickness(3, 0, 0, 0),
                            FontFamily = new FontFamily("Segoe UI"),
                            FontSize = 12

                        });
                    
#endif
                }
            }
            else if (m_DragSource.Name == "PART_PivotColumnList")
            {
                if (((IList)m_DragSource.ItemsSource).Count == 0)
                {
                    this.ColumnHeaderArea.ItemsSource = null;
                    this.ColumnHeaderArea.ItemTemplate = null;
#if SILVERLIGHT
                    string columnText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropColumnFields");
#endif
                    this.ColumnHeaderArea.Items.Add(new TextBlock()
                    {
#if !SILVERLIGHT
                        Text = rsKeys.pivotGridDropColumnFields,
#else
                        Text = columnText,
#endif
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(3, 0, 0, 0),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 12
                    });
                }
            }
            else if (m_DragSource.Name == "PART_RowList")
            {
                if (((IList)m_DragSource.ItemsSource).Count == 0)
                {
                    this.RowHeaderArea.ItemsSource = null;
                    this.RowHeaderArea.ItemTemplate = null;

#if !SILVERLIGHT
                    this.RowHeaderArea.Items.Add(new TextBlock()
                    {
                        Text = rsKeys.pivotGridDropRowFields,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(3, 0, 0, 0),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 12

                    });
#else
                    string rowText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropRowFields");
                    this.RowHeaderArea.Items.Add(new TextBlock()
                    {
                        Text = rowText,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(3, 0, 0, 0),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 12

                    });      
#endif
                }
            }
            else if (m_DragSource.Name == "PART_FilterList")
            {
                if (((IList)m_DragSource.ItemsSource).Count == 0)
                {
                    FilterHeaderArea.ItemsSource = null;
                    FilterHeaderArea.ItemTemplate = null;

#if !SILVERLIGHT
                    this.FilterHeaderArea.Items.Add(new TextBlock()
                    {
                        Text = rsKeys.pivotGridHeaderDropFilterFileds,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Margin = new Thickness(3, 0, 0, 0),
                        FontFamily = new FontFamily("Segoe UI"),
                        FontSize = 12

                    });
#else
                    string filterFieldText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropFilterFields");
                        this.FilterHeaderArea.Items.Add(new TextBlock()
                        {
                            Text = filterFieldText,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                            Margin = new Thickness(3, 0, 0, 0),
                            FontFamily = new FontFamily("Segoe UI"),
                            FontSize = 12

                        });
                    
#endif
                }
            }
            else if (m_DragSource.Name == "PivotItemPanel")
            {
                if (((IList)m_DragSource.ItemsSource).Count == 0)
                {
                    this.DragSource.ItemsSource = null;
                    this.DragSource.ItemTemplate = null;
                }
                else
                {
#if !SILVERLIGHT
                    this.FieldList.PivotItemPanel.ItemsSource = null;//.Clear();
                    this.FieldList.PivotItemPanel.DataContext = this.FieldList.PivotFields;
                    this.FieldList.PivotItemPanel.ItemsSource = this.FieldList.PivotFields;
#endif
                }
            }
        }

        /// <summary>
        /// Removes the filter item.
        /// </summary>
        /// <param name="filterItemsCollection">The filter item.</param>
        internal void RemoveFilterItem(FilterItemsCollection filterItemsCollection)
        {
            if (filterItemsCollection != null)
                this.Filters.Remove(filterItemsCollection);
        }

        internal void CalculatePivotRowItemWidth()
        {
            if (this.GridControl.PivotEngine.ColumnCount == 0 && this.GridControl.PivotEngine.RowCount == 0)
            {
                return;
            }

            if (this.RowHeaderArea != null && this.GridControl.InternalGrid != null && (this.GridControl.InternalGrid.ItemSource as IList) != null && (this.GridControl.InternalGrid.ItemSource as IList).Count > 0 && this.GridControl.InternalGrid.Model.ColumnWidths.LineCount > 0)
            {
                ItemContainerGenerator containerGenerator = this.RowHeaderArea.ItemContainerGenerator;

                for (int i = 0; i < this.GridControl.PivotRows.Count; i++)
                {
                    this.GridControl.InternalGrid.Model.ColumnWidths[i] = this.GridControl.InternalGrid.Model.ColumnWidths.DefaultLineSize;
                }

                this.GridControl.InternalGrid.Model.ResizeColumnsToFit(Syncfusion.Windows.Controls.Grid.GridRangeInfo.Cells(0, 0, this.GridControl.InternalGrid.ScrollRows.LastBodyVisibleLineIndex, this.GridControl.InternalGrid.ScrollColumns.LastBodyVisibleLineIndex), Windows.Controls.Grid.GridResizeToFitOptions.None);

                if (containerGenerator != null)
                {
                    if (this.GridControl.PivotEngine.PivotRows.Count > 0)
                    {
                        for (int i = 0; i < this.GridControl.PivotEngine.PivotRows.Count; i++)
                        {
                            PivotItem pivotItem = this.GridControl.PivotEngine.PivotRows[i];
                            Size size = Common.GetTextSize(pivotItem.FieldHeader);
                            double columnWidth = this.GridControl.InternalGrid.Model.ColumnWidths[i];
                            double requiredWidth = 70 + size.Width;

                            if (requiredWidth > columnWidth)
                            {
                                this.GridControl.InternalGrid.Model.ColumnWidths[i] = requiredWidth;
                            }

                            else if (requiredWidth == columnWidth)
                            {
                                this.GridControl.InternalGrid.Model.ColumnWidths[i] += 1;
                            }

                            ListBoxItem item = containerGenerator.ContainerFromIndex(i) as ListBoxItem;

                            if (item != null)
                            {

                                if (i == 0)
                                {
                                    ////if(this.GridControl.PivotRows.Count ==1)

                                    ////item.Width = this.GridControl.InternalGrid.Model.ColumnWidths[i] - 7;
                                    ////else
                                    item.Width = this.GridControl.InternalGrid.Model.ColumnWidths[i] - 6 > 0 ? this.GridControl.InternalGrid.Model.ColumnWidths[i] - 6 : 0;
                                }
                                else
                                {
                                    item.Width = this.GridControl.InternalGrid.Model.ColumnWidths[i] - 2 > 0 ? this.GridControl.InternalGrid.Model.ColumnWidths[i] - 2 : 0;
                                    item.Margin = new Thickness(1, 0, 0, 0);
                                }

                                if (this.GridControl.InternalGrid.Model.HeaderRows > 1)
                                {
                                    item.Height = 31;
                                }
                                else
                                {
                                    if (this.GridControl.InternalGrid.RowHeights.LineCount > 0)
                                        this.GridControl.InternalGrid.RowHeights[0] = 33;
                                    item.Height = 31;
                                }

                            }
#if SILVERLIGHT
                            else
                            {
                                this.ContainerEmpty = true;
                            }
#endif
                        }


                        this.CalculateComputationInfoWidth();
                        this.CalculateComputationInfoItemsWidth();
                        //if (this.GridControl.InternalGrid.Model.ColumnWidths.LineCount > (this.GridControl.PivotRows.Count - 1))
                        //    this.GridControl.InternalGrid.Model.ColumnWidths[this.GridControl.PivotRows.Count - 1] += 2;

                        //if (this.ComputationInfoWidth != null)
                        //{
                        //    //this.ComputationInfoWidth.Width = new GridLength(this.ComputationInfoWidth.Width.Value + 1);
                        //}

                        //this.GridControl.InternalGrid.Model.ColumnWidths[this.GridControl.PivotRows.Count - 1] += 30;

                    }
                    else
                    {
#if !SILVERLIGHT
                        Size size = Common.GetTextSize(rsKeys.pivotGridDropRowFields);
#else
                        string rowText = SR.GetString(CultureInfo.CurrentUICulture, "PivotGrid_GroupingBar_DropRowFields");
                        Size size = Common.GetTextSize(rowText);
#endif
                        if (this.GridControl.InternalGrid.Model.ColumnWidths.LineCount > 0)
                        {
                            this.GridControl.InternalGrid.Model.ColumnWidths[0] = size.Width + 35;
                            if (size.Width < this.GridControl.PivotCalculations.Count * 40)
                            {
                                this.GridControl.InternalGrid.ColumnWidths[0] = this.GridControl.PivotCalculations.Count * 40;
                            }

                            if (this.GridControl.GroupingBar.ComputationInfoWidth != null)
                                this.GridControl.GroupingBar.ComputationInfoWidth.Width = new GridLength(this.GridControl.InternalGrid.ColumnWidths[0]);
                        }
                    }
                }
                if (!this.GridControl.AllowRowHeaderAreaAutoSizing)
                    this.CalculateComputationInfoWidth();
            }
            else
            {
                this.CalculateComputationInfoWidth();
            }
        }

#if !SILVERLIGHT
        internal void RemoveAdorners()
        {
            AdornerLayer layer = AdornerLayer.GetAdornerLayer(this.GridControl);
            Adorner[] adorners = layer.GetAdorners(this.GridControl);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    layer.Remove(item);
                }
            }
        }

        internal void RemoveAdorners(out AdornerLayer layer)
        {
            layer = AdornerLayer.GetAdornerLayer(this.GridControl);
            Adorner[] adorners = layer.GetAdorners(this.GridControl);
            if (adorners != null)
            {
                foreach (var item in adorners)
                {
                    layer.Remove(item);
                }
            }
        }

        internal void AddAdorners(Point point, string FormatText, object DraggedItem)
        {
            AdornerLayer layer = null;
            this.RemoveAdorners(out layer);
            layer.Opacity = 0.6;
            layer.Add(new DragIndicatorAdorner(this.GridControl, point.X + 5, point.Y + 5, FormatText, DraggedItem));
        }

#endif

        #endregion

        #region [ Public Methods ]

        /// <summary>
        /// Gets the pivot item.
        /// </summary>
        /// <param name="filterItem">The filter item.</param>
        /// <returns></returns>
        public PivotItem GetPivotItem(FilterItemsCollection filterItem)
        {
            return GetPivotItem(filterItem.Name, filterItem.DisplayHeader, filterItem.Format, filterItem.AllowRunTimeGroupByField);
        }

        /// <summary>
        /// Generates a PivotItem based on given FieldName
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns>Pivotitem</returns>
        public PivotItem GetPivotItem(string fieldName)
        {
            return new PivotItem { FieldHeader = fieldName, FieldMappingName = fieldName, TotalHeader = PivotGridConstants.TotalString };
        }

        /// <summary>
        /// Gets the pivot item.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldHeader">The field header.</param>
        /// <returns></returns>
        public PivotItem GetPivotItem(string fieldName, string fieldHeader)
        {
            return new PivotItem { FieldHeader = fieldHeader, FieldMappingName = fieldName, TotalHeader = PivotGridConstants.TotalString };
        }

        /// <summary>
        /// Gets the pivot item.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="fieldHeader">The field header.</param>
        /// <param name="fieldFormat">The field format.</param>
        /// <param name="fieldAllowGroup">To enable or disable group by field at runtime</param>
        /// <returns>Pivot Item</returns>
        public PivotItem GetPivotItem(string fieldName, string fieldHeader, string fieldFormat, bool fieldAllowGroup)
        {
            return new PivotItem { FieldHeader = fieldHeader, FieldMappingName = fieldName, Format = fieldFormat, TotalHeader = PivotGridConstants.TotalString, AllowRunTimeGroupByField = fieldAllowGroup };
        }

        /// <summary>
        /// Gets the pivot item from pivot table field.
        /// </summary>
        /// <param name="pivotTableField"></param>
        /// <returns></returns>
        public PivotItem GetPivotItem(PivotTableField pivotTableField)
        {
            return new PivotItem
            {
                FieldHeader = pivotTableField.FieldHeader,
                AllowRunTimeGroupByField = pivotTableField.AllowRunTimeGroupByField,
                FieldMappingName = pivotTableField.FieldName,
                Format = pivotTableField.Format,
                TotalHeader = pivotTableField.TotalHeader
            };

        }
        #endregion
    }
}
