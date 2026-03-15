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
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.ComponentModel;
using System.Reflection;
using System.Collections;
using Syncfusion.Windows.Shared.Controls;
using Syncfusion.PivotAnalysis.Base.Silverlight;
using System.Windows.Shapes;
using System.Diagnostics;
using Syncfusion.Silverlight.Controls.PivotSchemaDesigner;

namespace Syncfusion.Silverlight.Controls.PivotGrid
{
    public partial class PivotGridGroupingBar
    {
        private Dictionary<PivotItem, IComparer> _savedComparers;
        #region [ Events ]

        internal event EventHandler OnFilterItemChanged;

        void InternalGrid_ResizingColumns(object sender, Windows.Controls.Grid.GridResizingColumnsEventArgs args)
        {
            if (args.AllowResize)
            {
                if (this.RowHeaderArea != null)
                {
                    for (int i = 0; i < this.Engine.PivotRows.Count; i++)
                    {
                        ListBoxItem item = this.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                        item.Width = (this.GridControl.InternalGrid.Model.ColumnWidths[i] >= 4) ? this.GridControl.InternalGrid.Model.ColumnWidths[i] - 4 : 0;
                    }
                }
                this.CalculateComputationInfoWidth();
            }
        }

        void InternalGrid_ResizingRows(object sender, Windows.Controls.Grid.GridResizingRowsEventArgs args)
        {
            if (args.AllowResize)
            {
                this.PositionPopup();
            }
        }

        void PivotColumns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.ColumnHeaderArea.Items.Count > 0 && this.ColumnHeaderArea.Items[0] is TextBlock)
            {
                this.ColumnHeaderArea.Items.Clear();
                //this.ColumnHeaderArea.Items.Clear();
                this.ColumnHeaderArea.DataContext = this.GridControl.PivotColumns;
                this.ColumnHeaderArea.ItemsSource = this.GridControl.PivotColumns;

                ResourceDictionary resource = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };

                this.ColumnHeaderArea.ItemTemplate = resource["PivotColumnItemTemplate"] as DataTemplate;
            }

            
        }
        
        void PivotRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.RowHeaderArea.Items.Count > 0 && this.RowHeaderArea.Items[0] is TextBlock)
            {
                this.RowHeaderArea.Items.Clear();
                //this.ColumnHeaderArea.Items.Clear();
                this.RowHeaderArea.DataContext = this.GridControl.PivotRows;
                this.RowHeaderArea.ItemsSource = this.GridControl.PivotRows;

                ResourceDictionary resource = new ResourceDictionary()
                {
                    Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                };

                this.RowHeaderArea.ItemTemplate = resource["PivotRowItemTemplate"] as DataTemplate;
            }
        }       

        void GridControl_Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.FilterHeaderArea.Items.Count > 0 && this.FilterHeaderArea.Items[0] is TextBlock)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems.Count > 0)
                    {
                        System.Collections.ObjectModel.ObservableCollection<FilterItemsCollection> filtersCollection = new System.Collections.ObjectModel.ObservableCollection<FilterItemsCollection>();
                        foreach (var item in e.NewItems)
                        {
                            if ((item is FilterExpression) && (item as FilterExpression).DimensionHeader != null && (item as FilterExpression).Tag is FilterItemsCollection)
                            {
                                filtersCollection.Add((FilterItemsCollection)(item as FilterExpression).Tag);
                            }
                        }
                        this.Filters = filtersCollection;
                    }

                }
                if (this.Filters.Count > 0)
                {
                    this.FilterHeaderArea.Items.Clear();
                    this.FilterHeaderArea.DataContext = this.Filters;
                    this.FilterHeaderArea.ItemsSource = this.Filters;

                    ResourceDictionary resource = new ResourceDictionary()
                    {
                        Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
                    };

                    this.FilterHeaderArea.ItemTemplate = resource["PivotFilterItemTemplate"] as DataTemplate;
                }
            }
            
        }

        
        internal bool shouldShowBouncingArrows = true;
        internal void DragAndDropManager_DragStarted(object sender, DragDropEventArgs args)
        {
            args.DropDescription = null;
            if (sender is ListBox) return;
            object source = ((ListBoxItem)sender).DataContext;
            if (source is PivotComputationInfo)
            {
                PivotComputationInfo item = source as PivotComputationInfo;
                if (!item.AllowRunTimeGroupByField)
                {
                    shouldShowBouncingArrows = false ;
                }
            }
            else if (source is PivotItem)
            {
                PivotItem item = source as PivotItem;
                if (!item.AllowRunTimeGroupByField)
                {
                    shouldShowBouncingArrows = false ;
                }
            }
            args.PayLoad = source;
            this.DraggedItem = source;
            this.ItemsControlName = ((FrameworkElement)args.DragSource).Name;
            // args.DragIcon = new ToggleButton() { Content= ((PivotItem)source).FieldHeader,Opacity = 0.5 };

            ResourceDictionary resource = new ResourceDictionary()
            {
                Source = new Uri("/Syncfusion.PivotAnalysis.Silverlight;component/PivotGridControl/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            };

            DragIndicatorButton = new ToggleButton();
            DragIndicatorButton.DataContext = source;
            DragIndicatorButton.Background = this.ItemsBackground;
            DragIndicatorButton.BorderBrush = this.ItemsBorderBrush;
            DragIndicatorButton.Style = resource["DragIndicatorStyle"] as Style;
            DragIndicatorButton.LayoutUpdated += new EventHandler(DragIndicatorButton_LayoutUpdated);
            DragIndicatorButton.Opacity = 0.5;
            args.DragIcon = DragIndicatorButton;
        }

        void DragIndicatorButton_LayoutUpdated(object sender, EventArgs e)
        {
            Button filterBtn = Common.FindVisualChildWithName<Button>(this.DragIndicatorButton, "filterBtn");
            Path sortPath = Common.FindVisualChildWithName<Path>(this.DragIndicatorButton,"sortPath");

            if (this.DragIndicatorButton.DataContext is PivotComputationInfo)
            {
                return;
            }

            if (filterBtn != null)
            {
                if (this.AllowFiltering)
                    filterBtn.Visibility = System.Windows.Visibility.Visible;
                else
                    filterBtn.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (sortPath != null)
            {
                if (this.AllowSorting)
                    sortPath.Visibility = System.Windows.Visibility.Visible;
                else
                    sortPath.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        internal void DragAndDropManager_Drag(object sender, DragDropEventArgs args)
        {
            if (this.Indicator == null) return;
            if (args.DragSource != null && args.DragSource is PivotGroupingItemsControl &&
               ((PivotGroupingItemsControl)args.DragSource).Name == "PART_ComputationInfoList" &&
                ((PivotGroupingItemsControl)args.DragSource).Items.Count == 1)
            {
                this.Indicator.IsOpen = false;
                this.ItemIndex = -2;
                return;
            }

            args.DropDescription = null;
            Point dragPoint = args.MouseEventArgs.GetPosition(Application.Current.RootVisual as UIElement);
              var items = VisualTreeHelper.FindElementsInHostCoordinates(dragPoint, Application.Current.RootVisual as UIElement);
              if (items.Count() == 0)
              {
                  var popupCollection = VisualTreeHelper.GetOpenPopups();
                  if (popupCollection != null)
                  {
                      foreach (var popups in popupCollection)
                      {
                          if (popups.Child is ChildWindow)
                          {
                              items = VisualTreeHelper.FindElementsInHostCoordinates(dragPoint, Application.Current.RootVisual as ChildWindow);
                          }
                      }
                  }
              }
            if (items.Count() > 0)
            {
                ToggleButton item = items.Where(i => i.GetType() == typeof(ToggleButton)).FirstOrDefault() as ToggleButton;
                if (item != null && item.Tag != args.PayLoad)
                {
                    Point buttonRelativePosition = args.MouseEventArgs.GetPosition(item);

                    int currentItemIndex = -5;
                    PivotGroupingItemsControl ctrl = items.Where(j => j.GetType() == typeof(PivotGroupingItemsControl)).FirstOrDefault() as PivotGroupingItemsControl;
                    this.ItemIndex = GetItemIndex(item, ctrl, args.PayLoad, out currentItemIndex);
                    if (ctrl != null)
                    {
                        if (ctrl.Name == this.FilterHeaderArea.Name && args.PayLoad is FilterItemsCollection)
                        {
                            this.Indicator.IsOpen = false;
                            this.ItemIndex = -2;
                            return;
                        }

                        if (item.ActualWidth > 0 && (item.ActualWidth / 2) > buttonRelativePosition.X)
                        {
                            GeneralTransform transform = item.TransformToVisual(this as UIElement);

                            this.Indicator.IsOpen = true;
                            this.Indicator.HorizontalOffset = transform.Transform(new Point(0, 0)).X - 6;
                            this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y - 10;
                            if (ctrl != null)
                                this.ItemIndex = ctrl.Items.IndexOf(item.Tag);
                        }
                        else
                        {
                            if (ItemIndex + 1 == currentItemIndex)
                            {
                                this.Indicator.IsOpen = false;
                                this.ItemIndex = -2;
                                return;
                            }

                            GeneralTransform transform = item.TransformToVisual(this as UIElement);

                            this.Indicator.IsOpen = true;
                            this.Indicator.HorizontalOffset = (transform.Transform(new Point(0, 0)).X + item.ActualWidth) - 5; ;
                            this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y - 5;// -11;

                            if (ctrl != null)
                                this.ItemIndex = ctrl.Items.IndexOf(item.Tag) + 1;
                        }
                    }

                    this.DropTarget = ctrl;
                }
                else
                {
                    if (item == null)
                    {
                        PivotGroupingItemsControl ctrl = items.Where(i => i.GetType() == typeof(PivotGroupingItemsControl)).FirstOrDefault() as PivotGroupingItemsControl;
                        if (ctrl != null)
                        {
                            ListBoxItem lstItem = ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1) as ListBoxItem;

                            if (ctrl != null && ctrl.Name == this.FilterHeaderArea.Name && args.PayLoad is FilterItemsCollection)
                            {
                                this.Indicator.IsOpen = false;
                                this.ItemIndex = -2;
                                return;
                            }

                            double width = 0;

                            if (lstItem != null && (lstItem.Content is PivotItem))
                                width = ((ListBoxItem)(ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1))).
                                            TransformToVisual(Application.Current.RootVisual as UIElement).Transform(new Point(0, 0)).X + lstItem.ActualWidth;
                            else if ((ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1)) != null)
                                width = ((ListBoxItem)(ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1))).
                                            TransformToVisual(Application.Current.RootVisual as UIElement).Transform(new Point(0, 0)).X;

                            if ((this.FlowDirection == System.Windows.FlowDirection.RightToLeft ? (dragPoint.X < width) : (dragPoint.X > width)) && lstItem != null)
                            {
                                if (lstItem.Content is TextBlock)
                                {
                                    if (((TextBlock)lstItem.Content).Text.Contains("Row"))
                                    {
                                        GeneralTransform transform = lstItem.TransformToVisual(this as UIElement);
                                        this.Indicator.IsOpen = true;
                                        this.Indicator.HorizontalOffset = (transform.Transform(new Point(0, 0)).X) - 2; ;
                                        this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y - 12;// -11;
                                    }
                                    else
                                    {
                                        GeneralTransform transform = lstItem.TransformToVisual(this as UIElement);
                                        this.Indicator.IsOpen = true;
                                        this.Indicator.HorizontalOffset = (transform.Transform(new Point(0, 0)).X) - 2; ;
                                        this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y - 5;// -11;
                                    }

                                    this.ItemIndex = 0;
                                }
                                else
                                {
                                    if (args.PayLoad != lstItem.Content)
                                    {
                                        GeneralTransform transform = lstItem.TransformToVisual(this as UIElement);
                                        this.Indicator.IsOpen = true;
                                        this.Indicator.HorizontalOffset = (transform.Transform(new Point(0, 0)).X + lstItem.ActualWidth) - 5; ;
                                        this.Indicator.VerticalOffset = transform.Transform(new Point(0, 0)).Y - 5;// -11;

                                        this.ItemIndex = ctrl.Items.Count;
                                    }
                                }
                            }
                            this.DropTarget = ctrl;
                        }
                        else
                        {
                            this.DropTarget = null;
                            this.ItemIndex = -2;
                            this.Indicator.IsOpen = false;
                        }
                    }
                    else
                    {
                        this.DropTarget = null;
                        this.ItemIndex = -2;
                        this.Indicator.IsOpen = false;
                    }
                }
            }
            else
            {
                this.ItemIndex = -2;
                this.Indicator.IsOpen = false;
            }
        }

       internal void DragAndDropManager_Drop(object sender, DragDropEventArgs args)
       {
           if (this.Indicator == null) return;
           if (!shouldShowBouncingArrows)
           {
               shouldShowBouncingArrows = true;
               this.Indicator.IsOpen = false;
               return;
           }
           //To perform drag and drop an item from any area of grouping bar to another area and from ComputationListPanel(PivotGridComputationListWindow) to GroupingArea.
           //This condition has been checked since while perform drag operation, calling VisualTreeHelper.FindElementsInHostCoordinates does not contains the new window(PivotGridComputationListWindow) opened as a child window in its returned object.         
           if (this.ItemIndex == -2 && this.DropTarget != null && (sender is ListBoxItem) && (this.DropTarget.Name != this.ColumnHeaderArea.Name || this.DropTarget.Name != this.RowHeaderArea.Name || this.DropTarget.Name != this.FilterHeaderArea.Name || this.DropTarget.Name != this.DataHeaderArea.Name))
           {
               this.ItemIndex = this.GridControl.PivotCalculations.Count;
               object DraggedItem = ((ListBoxItem)sender).DataContext;
               string draggedItemName = ((FrameworkElement)args.DragSource).Name;             
               ProcessDataItemsControl();
           }
               //To handle drag and drop in popup. Since row list is placed inside the popup.
           else if (this.ItemIndex == -2 && this.DropTarget == null && (sender is ListBoxItem))
           {
               Point dragPoint = args.MouseEventArgs.GetPosition(Application.Current.RootVisual as UIElement);
               var items = VisualTreeHelper.FindElementsInHostCoordinates(dragPoint, Application.Current.RootVisual as UIElement);
               ListBoxItem dropTarget = null;
               DependencyObject depObject = (DependencyObject)args.DropTarget;
               while (depObject != null)
               {
                   dropTarget = depObject as ListBoxItem;
                   if (dropTarget != null)
                   {
                       break;
                   }
                   depObject = VisualTreeHelper.GetParent(depObject);
               }
                PivotGroupingItemsControl ctrl = depObject as PivotGroupingItemsControl;
                if (ctrl != null)
                {
                    ListBoxItem lstItem = ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1) as ListBoxItem;
                    double width = 0;
                    if (lstItem != null && (lstItem.Content is PivotItem))
                        width = ((ListBoxItem)(ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1))).
                                    TransformToVisual(Application.Current.RootVisual as UIElement).Transform(new Point(0, 0)).X + lstItem.ActualWidth;
                    else if ((ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1)) != null)
                        width = ((ListBoxItem)(ctrl.ItemContainerGenerator.ContainerFromIndex(ctrl.Items.Count - 1))).
                                    TransformToVisual(Application.Current.RootVisual as UIElement).Transform(new Point(0, 0)).X;
                    if ((this.FlowDirection == System.Windows.FlowDirection.LeftToRight ? (dragPoint.X < width) : (dragPoint.X > width)) && lstItem != null)
                    {
                        if ((width - dragPoint.X) > dragPoint.X && this.GridControl.PivotRows.Count == 1)
                        {
                            this.ItemIndex = 0;
                        }
                        else
                        {
                            ToggleButton item = items.Where(i => i.GetType() == typeof(ToggleButton)).FirstOrDefault() as ToggleButton;
                            if (item != null)
                            {
                                Point buttonRelativePosition = args.MouseEventArgs.GetPosition(item);
                                int currentItemIndex = -5;
                                int index = GetItemIndex(item, ctrl, args.PayLoad, out currentItemIndex);
                                if (item.ActualWidth > 0 && (item.ActualWidth / 2) > buttonRelativePosition.X)
                                {
                                    this.ItemIndex = index;
                                }
                                else
                                    this.ItemIndex = index + 1;
                            }
                        }
                    }
                }
               object draggedItem = (sender as ListBoxItem).DataContext;
               if (dropTarget.Name == "PART_RowList")
                   ProcessRowItemsControl(dropTarget.Name);
           }
           //To perform drag and drop an item from grouping bar area to ComputationListPanel(PivotGridComputationListWindow)
           else
           {
               if (this.ItemIndex != -2 && this.DropTarget != null)
               {
                   if (this.DropTarget.Name == this.ColumnHeaderArea.Name)
                   {
                       ProcessColumnItemsControl(((FrameworkElement)args.DragSource).Name);
                   }

                   else if (this.DropTarget.Name == this.RowHeaderArea.Name)
                   {
                       ProcessRowItemsControl(((FrameworkElement)args.DragSource).Name);
                   }

                   else if (this.DropTarget.Name == this.DataHeaderArea.Name)
                   {
                       ProcessDataItemsControl();
                   }

                   else if (this.DropTarget.Name == this.FilterHeaderArea.Name)
                   {
                       ProcessFilterItemsControl();
                   }

               }

               if (this.Indicator.IsOpen)
                   this.Indicator.IsOpen = false;
               if (this.GridControl.AllowRowHeaderAreaAutoSizing && (this.RowHeaderArea.ItemsSource == null || this.RowHeaderArea.Items.Count == 0) && this.GridControl.GroupingBar.DataHeaderArea.ItemContainerGenerator.ContainerFromIndex(0) != null && this.GridControl.GroupingBar.ComputationButtonWidth != null && Double.IsNaN((this.GridControl.GroupingBar.DataHeaderArea.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem).Width) != true)
               {
                   this.GridControl.InternalGrid.ColumnWidths[0] = (2 * (this.GridControl.GroupingBar.DataHeaderArea.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem).Width) + this.GridControl.GroupingBar.ComputationButtonWidth.Width.Value;
               }
           }
           if (OnFilterItemChanged != null)
                  OnFilterItemChanged(this, args);
         
       }

      
       
        private PivotComputationInfo GetPivotItem(object item)
       {
            if (item is PivotItem)
            {
                PivotItem pivotitem = item as PivotItem;
                PivotComputationInfo compInfo = new PivotComputationInfo() { FieldName = pivotitem.FieldMappingName, Format = pivotitem.Format, AllowRunTimeGroupByField = pivotitem.AllowRunTimeGroupByField };
                return compInfo;
            }
            else if (item is FilterItemsCollection)
            {
                FilterItemsCollection filterItem = item as FilterItemsCollection;
                PivotItem pivotitem = new PivotItem { FieldHeader = filterItem.DisplayHeader, FieldMappingName = filterItem.Name, Format = filterItem.Format, TotalHeader = PivotGridConstants.TotalString, AllowRunTimeGroupByField = filterItem.AllowRunTimeGroupByField };
                PivotComputationInfo compInfo = new PivotComputationInfo() { FieldName = pivotitem.FieldMappingName, Format = pivotitem.Format, AllowRunTimeGroupByField = pivotitem.AllowRunTimeGroupByField };
                return compInfo;
            }
            else if (item is PivotComputationInfo)
            {
                return item as PivotComputationInfo;
            }
            return null;
        }

        private void ProcessColumnItemsControl(string ctrlName)
        {
            if (DraggedItem is PivotItem)
            {
                //if (!this.GridControl.PivotColumns.Contains(this.DraggedItem as PivotItem))
                {
                    this.GridControl.IgnoreRefesh = true;

                    if (ctrlName == this.RowHeaderArea.Name)
                    {
                        this.GridControl.PivotRows.Remove(this.DraggedItem as PivotItem);
                    }
                    else if (ctrlName == this.ColumnHeaderArea.Name)
                    {
                        if (this.ItemIndex == this.GridControl.PivotColumns.Count && this.ItemIndex >0)
                        {
                            this.ItemIndex -= 1;
                        }

                        this.GridControl.PivotColumns.Remove(this.DraggedItem as PivotItem);
                        
                        //this.ItemIndex -= 1;
                    }               

                    //this.GridControl.IgnoreRefesh = false;
                    if (this.GridControl.PivotColumns.Count == 0)

                        this.ApplyTemplateToGroupingControl(this.ColumnHeaderArea, this.DraggedItem);
                    else
                        this.GridControl.PivotColumns.Insert(this.ItemIndex, this.DraggedItem as PivotItem);

                    this.GridControl.IgnoreRefesh = false;
                    this.GridControl.RefreshFromGroupingBar = true;
                    this.GridControl.InternalRefresh();
                    this.GridControl.GroupingBar.ColumnHeaderArea.InvalidateArrange();
                }
            }

            else if (DraggedItem is PivotTableField)
            {
                PivotTableField pivotTableField = DraggedItem as PivotTableField;
                this.GridControl.IgnoreRefesh = true;
                PivotItem pivotItem = this.GridControl.PivotRows.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                if (pivotItem != null)
                {
                    this.GridControl.PivotRows.Remove(pivotItem);
                }
                var calculations = this.GridControl.PivotCalculations.Where(i => i.FieldName == pivotTableField.FieldName).FirstOrDefault();
                if (calculations != null)
                {
                    this.GridControl.PivotCalculations.Remove(calculations);
                }
                var filterItem = this.Filters.Where(i => i.Name == pivotTableField.FieldName).FirstOrDefault();
                if (filterItem != null)
                    RemoveFilterItem(filterItem);

                pivotItem = GetPivotItem(pivotTableField);
                if (this.GridControl.PivotColumns.Count == 0)
                    this.ApplyTemplateToGroupingControl(this.ColumnHeaderArea, pivotItem);
                else if (this.GridControl.PivotColumns.Where(i=>i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault() == null)
                    this.GridControl.PivotColumns.Insert(this.ItemIndex, pivotItem);

                this.GridControl.IgnoreRefesh = false;
                this.GridControl.RefreshFromGroupingBar = true;
                this.GridControl.InternalRefresh();
                this.GridControl.GroupingBar.ColumnHeaderArea.InvalidateArrange();
            }

            else if (DraggedItem is PivotComputationInfo)
            {
                if (this.GridControl.PivotCalculations.Count > 1)
                {
                    this.GridControl.IgnoreRefesh = true;
                    this.GridControl.PivotCalculations.Remove(this.DraggedItem as PivotComputationInfo);
                    PivotItem item = new PivotItem()
                    {
                        FieldHeader = ((PivotComputationInfo)DraggedItem).FieldHeader,
                        FieldMappingName = ((PivotComputationInfo)DraggedItem).FieldName,
                        Format = ((PivotComputationInfo)DraggedItem).Format,
                        TotalHeader = "Total"
                    };
                    //this.GridControl.IgnoreRefesh = false;
                    this.ApplyTemplateToGroupingControl(this.ColumnHeaderArea, item);
                }
            }

            else if (DraggedItem is FilterItemsCollection)
            {
                this.AddFilterItem(this.ColumnHeaderArea);
            }

            this.ApplyEmptyTemplate();
        }

        private void ProcessRowItemsControl(string ctrlName)
        {
            if (DraggedItem is PivotItem)
            {
               // if (!this.GridControl.PivotRows.Contains(this.DraggedItem as PivotItem))
                {
                    this.GridControl.IgnoreRefesh = true;
                    if (ctrlName == this.RowHeaderArea.Name)
                    {
                        if (this.ItemIndex == this.GridControl.PivotRows.Count && this.ItemIndex > 0)
                        {
                            this.ItemIndex -= 1;
                        }

                        this.GridControl.PivotRows.Remove(this.DraggedItem as PivotItem);
                        //this.ItemIndex -= 1;
                    }
                    else if (ctrlName == this.ColumnHeaderArea.Name)
                    {
                        this.GridControl.PivotColumns.Remove(this.DraggedItem as PivotItem);
                    }

                    //this.GridControl.IgnoreRefesh = false;
                    if (this.GridControl.PivotRows.Count == 0)
                        this.ApplyTemplateToGroupingControl(this.RowHeaderArea, this.DraggedItem);
                    else
                        this.GridControl.PivotRows.Insert(this.ItemIndex,this.DraggedItem as PivotItem);

                    this.GridControl.IgnoreRefesh = false;
                    this.GridControl.RefreshFromGroupingBar = true;
                    this.GridControl.InternalRefresh();
                    this.GridControl.GroupingBar.RowHeaderArea.InvalidateArrange();
                } 
            }
            else if (DraggedItem is PivotTableField)
            {
                PivotTableField pivotTableField = (DraggedItem as PivotTableField);
                PivotItem pivotItem = this.GridControl.PivotRows.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                if (pivotItem == null)
                {
                    this.GridControl.IgnoreRefesh = true;
                    pivotItem = this.GridControl.PivotColumns.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                    if (pivotItem != null)
                        this.GridControl.PivotColumns.Remove(pivotItem);
                    PivotComputationInfo pivotCalculation = this.GridControl.PivotCalculations.Where(i => i.FieldName == pivotTableField.FieldName).FirstOrDefault();
                    if (pivotCalculation != null)
                        this.GridControl.PivotCalculations.Remove(pivotCalculation);

                    var filterItem = this.Filters.Where(i => i.Name == pivotTableField.FieldName).FirstOrDefault();
                    if (filterItem != null)
                        RemoveFilterItem(filterItem);

                    pivotItem = GetPivotItem(pivotTableField);
                    if (this.GridControl.PivotRows.Count == 0)
                        this.ApplyTemplateToGroupingControl(this.RowHeaderArea, pivotItem);
                    else if (this.GridControl.PivotRows.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault() == null)
                        this.GridControl.PivotRows.Insert(this.ItemIndex, pivotItem);

                    this.GridControl.IgnoreRefesh = false;
                    this.GridControl.RefreshFromGroupingBar = true;
                    this.GridControl.InternalRefresh();
                    this.GridControl.GroupingBar.RowHeaderArea.InvalidateArrange();
                }
            }
            else if (DraggedItem is PivotComputationInfo)
            {
                if (this.GridControl.PivotCalculations.Count > 1)
                {
                    this.GridControl.IgnoreRefesh = true;
                    this.GridControl.PivotCalculations.Remove(this.DraggedItem as PivotComputationInfo);
                    PivotItem item = new PivotItem()
                    {
                        FieldHeader = ((PivotComputationInfo)DraggedItem).FieldHeader,
                        FieldMappingName = ((PivotComputationInfo)DraggedItem).FieldName,
                        Format = ((PivotComputationInfo)DraggedItem).Format,
                        TotalHeader = PivotGridConstants.TotalString
                    };
                    //this.GridControl.IgnoreRefesh = false;
                    this.ApplyTemplateToGroupingControl(this.RowHeaderArea, item);
                }
            }

            else if (DraggedItem is FilterItemsCollection)
            {
                this.AddFilterItem(this.RowHeaderArea);
            }

            this.ApplyEmptyTemplate();
        }

        private void ProcessDataItemsControl()
        {
            if (DraggedItem is PivotItem)
            {
                switch (this.ItemsControlName)
                {
                    case "PART_ColumnList":
                        this.GridControl.IgnoreRefesh = true;
                        this.GridControl.PivotColumns.Remove(DraggedItem as PivotItem);
                        this.AddComputationInfo(DraggedItem as PivotItem);
                        break;
                    case "PART_RowList":
                        this.GridControl.IgnoreRefesh = true;
                        this.GridControl.PivotRows.Remove(DraggedItem as PivotItem);
                        this.AddComputationInfo(DraggedItem as PivotItem);
                        break;
                }
            }
            else if (DraggedItem is PivotTableField)
            {
                this.GridControl.IgnoreRefesh = true;
                PivotTableField pivotTableField = DraggedItem as PivotTableField;
                PivotComputationInfo pivotCalculation = this.GridControl.PivotCalculations.Where(i => i.FieldName == pivotTableField.FieldName).FirstOrDefault();
                if (pivotCalculation == null)
                {
                    PivotItem pivotItem = this.GridControl.PivotColumns.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                    if (pivotItem != null)
                        this.GridControl.PivotColumns.Remove(pivotItem);

                    pivotItem = this.GridControl.PivotRows.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                    if (pivotItem != null)
                        this.GridControl.PivotRows.Remove(pivotItem);

                    var filterItem = this.Filters.Where(i => i.Name == pivotTableField.FieldName).FirstOrDefault();
                    if (filterItem != null)
                        RemoveFilterItem(filterItem);

                    this.AddComputationInfo(GetPivotItem(pivotTableField));
                }
            }
            else if (DraggedItem is FilterItemsCollection)
            {
                this.AddFilterItem(this.DataHeaderArea);
            }

            else if (DraggedItem is PivotComputationInfo)
            {
                this.GridControl.IgnoreRefesh = true;

                if (this.ItemIndex == this.GridControl.PivotCalculations.Count && this.ItemIndex > 0)
                {
                    this.ItemIndex -= 1;
                }

                this.GridControl.PivotCalculations.Remove(DraggedItem as PivotComputationInfo);

                this.GridControl.PivotCalculations.Insert(this.ItemIndex, DraggedItem as PivotComputationInfo);

                this.GridControl.IgnoreRefesh = false;
                this.GridControl.RefreshFromGroupingBar = true;
                this.GridControl.InternalRefresh();
            }

            this.ApplyEmptyTemplate();
        }

        private void ProcessFilterItemsControl()
        {
            if (this.DraggedItem is PivotItem)
            {
                PivotItem pivotItem = (PivotItem)DraggedItem;

                if (this.Filters.Where(f => f.Name == pivotItem.FieldHeader).FirstOrDefault() == null)
                {
                    object descriptor = GetPropertyDescriptor(pivotItem.FieldMappingName);
                    if (descriptor != null)
                    {
                        switch (this.ItemsControlName)
                        {
                            case "PART_ColumnList":
                                this.GridControl.IgnoreRefesh = true;
                                //this.GridControl.RefreshFromGroupingBar = true;
                                this.GridControl.PivotColumns.Remove(pivotItem);
                                break;
                            case "PART_RowList":
                                this.GridControl.IgnoreRefesh = true;
                                this.GridControl.PivotRows.Remove(pivotItem);
                                break;
                        }

                    

                        //this.GridControl.IgnoreRefesh = false;
                        FilterItemsCollection filterList = null;
                        FilterExpression exp = null;
                        if (descriptor is PropertyInfo)
                        {
                            exp = this.GridControl.Filters.SingleOrDefault(x => x.Name == (descriptor as PropertyInfo).Name);
                            if (exp != null)
                                filterList = exp.Tag as FilterItemsCollection;
                            else
                                filterList = GetFilterItem(descriptor as PropertyInfo);
                        }
                        else if (descriptor is ExpressionPropertyDescriptor)
                        {
                            exp = this.GridControl.Filters.SingleOrDefault(x => x.Name == (descriptor as ExpressionPropertyDescriptor).Name);
                            if (exp != null)
                                filterList = exp.Tag as FilterItemsCollection;
                            else
                                filterList = GetFilterItem(descriptor as ExpressionPropertyDescriptor);

                        }
                        filterList.DisplayHeader = pivotItem.FieldHeader;
                        filterList.Format = pivotItem.Format;
                        filterList.ShowSubTotal = pivotItem.ShowSubTotal;
                        this.Filters.Insert(this.ItemIndex,filterList);

                        this.GridControl.IgnoreRefesh = false;
                        this.GridControl.RefreshFromGroupingBar = true;
                        this.GridControl.InternalRefresh();
                    }
                }
            }
            else if (DraggedItem is PivotTableField)
            {
                PivotTableField pivotTableField = DraggedItem as PivotTableField;
                if (this.Filters.Where(i => i.Name == pivotTableField.FieldName).FirstOrDefault() == null)
                {
                    this.GridControl.IgnoreRefesh = true;
                    PivotItem pivotItem = this.GridControl.PivotRows.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                    if (pivotItem != null)
                        this.GridControl.PivotRows.Remove(pivotItem);

                    pivotItem = this.GridControl.PivotColumns.Where(i => i.FieldMappingName == pivotTableField.FieldName).FirstOrDefault();
                    if (pivotItem != null)
                        this.GridControl.PivotColumns.Remove(pivotItem);

                    PivotComputationInfo pivotCalculation = this.GridControl.PivotCalculations.Where(i => i.FieldName == pivotTableField.FieldName).FirstOrDefault();
                    if (pivotCalculation != null)
                        this.GridControl.PivotCalculations.Remove(pivotCalculation);

                    object descriptor = GetPropertyDescriptor(pivotTableField.FieldName);
                    if (descriptor != null)
                    {
                        FilterItemsCollection filterlist = null;
                        FilterExpression exp = null;
                        if (descriptor is PropertyInfo)
                        {
                            exp = this.GridControl.Filters.SingleOrDefault(x => x.Name == (descriptor as PropertyInfo).Name);
                            if (exp != null)
                                filterlist = exp.Tag as FilterItemsCollection;
                            else
                                filterlist = GetFilterItem(descriptor as PropertyInfo);
                        }
                        else if (descriptor is ExpressionPropertyDescriptor)
                        {
                            exp = this.GridControl.Filters.SingleOrDefault(x => x.Name == (descriptor as ExpressionPropertyDescriptor).Name);
                            if (exp != null)
                                filterlist = exp.Tag as FilterItemsCollection;
                            else
                                filterlist = GetFilterItem(descriptor as ExpressionPropertyDescriptor);
                        }
                        filterlist.DisplayHeader = pivotTableField.FieldHeader;
                        filterlist.Format = pivotTableField.Format;
                        this.Filters.Insert(this.ItemIndex, filterlist);
                        this.GridControl.IgnoreRefesh = false;
                        this.GridControl.RefreshFromGroupingBar = true;
                        this.GridControl.InternalRefresh();
                    }
                }
            }

            else if (this.DraggedItem is PivotComputationInfo)
            {
                if (this.GridControl.PivotCalculations.Count > 1)
                {
                    PivotComputationInfo pivotComputationInfo = (PivotComputationInfo)this.DraggedItem;

                    if (this.Filters.Where(f => f.Name == pivotComputationInfo.FieldName).FirstOrDefault() == null)
                    {
#if !SILVERLIGHT
                        PropertyInfo descriptor = GetPropertyDescriptor(pivotComputationInfo.FieldName);
#else
                        object descriptor = GetPropertyDescriptor(pivotComputationInfo.FieldName);
#endif
                        if (descriptor != null)
                        {
                            if (this.GridControl.PivotCalculations.Count > 1)
                            {
                                this.GridControl.IgnoreRefesh = true;
                                this.GridControl.PivotCalculations.Remove(this.DraggedItem as PivotComputationInfo);

#if !SILVERLIGHT
                                FilterItemsCollection filterList = GetFilterItem(descriptor);
#else
                                FilterItemsCollection filterList = null;
                                FilterExpression exp = null;
                                if (descriptor is PropertyInfo)
                                {
                                    exp =this.GridControl.Filters.SingleOrDefault(x=>x.Name == (descriptor as PropertyInfo).Name);
                                    if (exp != null)
                                        filterList = exp.Tag as FilterItemsCollection;
                                    else
                                        filterList = GetFilterItem(descriptor as PropertyInfo);
                                }
                                else if (descriptor is ExpressionPropertyDescriptor)
                                {
                                    exp = this.GridControl.Filters.SingleOrDefault(x => x.Name == (descriptor as ExpressionPropertyDescriptor).Name);
                                    if (exp != null)
                                        filterList = exp.Tag as FilterItemsCollection;
                                    else
                                        filterList = GetFilterItem(descriptor as ExpressionPropertyDescriptor);
                                }
#endif
                                filterList.DisplayHeader = pivotComputationInfo.FieldHeader;
                                filterList.Format = pivotComputationInfo.Format;
                                this.Filters.Insert(this.ItemIndex, filterList);

                                this.GridControl.IgnoreRefesh = false;
                                this.GridControl.RefreshFromGroupingBar = true;
                                this.GridControl.InternalRefresh();
                            }
                        }
                    }
                }
            }

            this.ApplyTemplateToGroupingControl(this.FilterHeaderArea, null);
            this.ApplyEmptyTemplate();
        }    

        void ColumnHeaderArea_LayoutUpdated(object sender, EventArgs e)
        {
            this.FormatItems(this.ColumnHeaderArea);
            this.ContextMenuManipulation(this.ColumnHeaderArea);
        }

        void RowHeaderArea_LayoutUpdated(object sender, EventArgs e)
        {
            this.FormatItems(this.RowHeaderArea);
            this.ContextMenuManipulation(this.RowHeaderArea);
        }

        void DataHeaderArea_LayoutUpdated(object sender, EventArgs e)
        {
            this.FormatItems(this.DataHeaderArea);
            this.ContextMenuManipulation(this.DataHeaderArea);
        }

        /// <summary>
        /// ContextMenu manipulation.
        /// </summary>
        /// <param name="pivotGroupingItems">The pivot grouping items.</param>
        internal void ContextMenuManipulation(PivotGroupingItemsControl pivotGroupingItems)
        {
            bool isSingleItem = pivotGroupingItems.Items.Count == 1 ? true : false;
            if (pivotGroupingItems.Items.Count > 0)
            {
                for (int i = 0; i < pivotGroupingItems.Items.Count; i++)
                {
                    ListBoxItem item = pivotGroupingItems.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (item != null && Common.FindVisualChild<Border>(item) != null)
                    {
                        Syncfusion.Windows.Shared.ContextMenuAdv contextMenu = Syncfusion.Windows.Shared.ContextMenuAdvService.GetContextMenuAdv(Common.FindVisualChild<Border>(item));
                        if (contextMenu != null)
                        {
                            contextMenu.Opened -= new RoutedEventHandler(ContextMenuAdv_Opened);
                            if (isSingleItem)
                                contextMenu.Visibility = System.Windows.Visibility.Collapsed;
                            else
                            {
                                contextMenu.Visibility = System.Windows.Visibility.Visible;
                                contextMenu.Opened += new RoutedEventHandler(ContextMenuAdv_Opened);
                            }
                        }
                    }
                }
            }
        }

        void FilterHeaderArea_LayoutUpdated(object sender, EventArgs e)
        {
            this.FormatItems(this.FilterHeaderArea);
        }
     
       internal void PivotItem_ArrangeOverrideExecute(object sender, ArrangeOverrideEventArgs e)
        {
            //e.ListBoxItem.Width += 10;
            this.ApplyBorderColor(e.ListBoxItem);
            this.PositionPopup();
        }
        
        void AllFilterItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FilterItemElement element = sender as FilterItemElement;
            if (element.IsSelected != null && !element.IsSelected.Value)
            {
                this.OKButton.IsEnabled = false;
            }
            else
                this.OKButton.IsEnabled = true;
        }

        void FilterPopup_Opened(object sender, EventArgs e)
        {
            Border brdItem = m_FilterPopup.Child as Border;
            if (brdItem != null)
            {
                brdItem.Background = this.ItemsBackground;
                brdItem.BorderBrush = this.ItemsBorderBrush;
            }

            PopupScrollViewer = Common.FindVisualChild<ScrollViewer>(((Popup)sender).Child);
         
            FilterListBox = PopupScrollViewer.Content as ListBox;
            FilterListBox.ItemsSource = null;

            this.OKButton = Common.FindVisualChildWithName<Button>(((Popup)sender).Child, "PART_btnOK");
            this.CancelButton = Common.FindVisualChildWithName<Button>(((Popup)sender).Child, "PART_btnCancel");
            FilterExpression fItem = null;
            if (CurrentItemForFilter is PivotItem)
            {
                object descriptor = GetPropertyDescriptor(((PivotItem)CurrentItemForFilter).FieldMappingName);
                if (((PivotItem)CurrentItemForFilter).Format != null)
                    fItem = this.GridControl.Filters.Where(f => (f.DimensionHeader == ((PivotItem)CurrentItemForFilter).FieldMappingName) && f.Format == ((PivotItem)CurrentItemForFilter).Format).FirstOrDefault();
                else
                    fItem = this.GridControl.Filters.Where(f => (f.DimensionHeader == ((PivotItem)CurrentItemForFilter).FieldMappingName)).FirstOrDefault();
                if (fItem == null)
                {
                    if (((PivotItem)CurrentItemForFilter).Format != null)
                        fItem = this.GridControl.Filters.Where(f => f.Name == ((PivotItem)CurrentItemForFilter).FieldMappingName && f.Format == ((PivotItem)CurrentItemForFilter).Format).FirstOrDefault();
                    else
                        fItem = this.GridControl.Filters.Where(f => f.Name == ((PivotItem)CurrentItemForFilter).FieldMappingName).FirstOrDefault();
                }
                if (fItem != null && fItem.Tag != null)
                {
                    FilterItems = fItem.Tag as FilterItemsCollection;                        
                    if (FilterItems.FilteredValues.Count > 0)
                    {
                        foreach (var item in FilterItems)
                        {
                            //filterList.Select(i => i).Where(j => j.Key == item).FirstOrDefault().IsSelected = true;
                            if (FilterItems.FilteredValues.Contains(item.Key))
                            {
                                item.IsSelected = true;
                            }
                            else
                                item.IsSelected = false;
                        }
                    }


                    FilterListBox.DataContext = FilterItems;
                    FilterListBox.ItemsSource = FilterItems;
                }
                else
                {
                    if (fItem == null || (fItem != null && fItem.Tag == null))
                    {
                        FilterItems = GetFilterItem(descriptor, (CurrentItemForFilter as PivotItem).Format);
                        if(fItem != null)
                        fItem.Tag = FilterItems;
                    }

                    if ((CurrentItemForFilter as PivotItem).Comparer != null)
                    {
                        var temp = FilterItems[0];// remove (ALL) before sorting...
                        FilterItems.RemoveAt(0);
                        FilterItems.Sort(new PivotFilterElementComparer((CurrentItemForFilter as PivotItem).Comparer));
                        FilterItems.Insert(0, temp); //Insert (ALL) back...
                    }
                    FilterListBox.DataContext = FilterItems;// GetFilterItem(descriptor);
                    FilterListBox.ItemsSource = FilterItems;// GetFilterItem(descriptor);
                    if (fItem!= null && fItem.Expression != null)
                    {
                        foreach (var item in FilterItems)
                        {
                            if (fItem.Expression.ToString().Contains(item.ToString()))
                            {
                                item.IsSelected = true;
                            }
                            else
                                item.IsSelected = false;
                        }
                    }
                }
            }

            else if (CurrentItemForFilter is FilterItemsCollection)
            {
                FilterItemsCollection filterList = CurrentItemForFilter as FilterItemsCollection;
                if (filterList[0].Key != "(All)" && filterList.Any(x => x.Key == "(All)"))
                {
                    FilterItemElement item = filterList.FirstOrDefault(x => x.Key == "(All)");
                    filterList.Remove(item);
                    filterList.Insert(0, item);
                }

                if (filterList.FilteredValues.Count > 0)
                {
                    foreach (var item in filterList)
                    {
                        //filterList.Select(i => i).Where(j => j.Key == item).FirstOrDefault().IsSelected = true;
                        if (filterList.FilteredValues.Contains(item.Key))
                        {
                            item.IsSelected = true;
                        }
                        else
                            item.IsSelected = false;
                    }
                }

                FilterItems = filterList;
                this.FilterListBox.DataContext = filterList;
                this.FilterListBox.ItemsSource = filterList;
                this.FilterPopup.IsOpen = true;
            }
        }

        void m_CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.FilterItems.RejectChanges();
            this.FilterPopup.IsOpen = false;
        }

        void m_OKButton_Click(object sender, RoutedEventArgs e)
        {
              if (this.GridControl != null && ((this.FilterItems as FilterItemsCollection)[0].IsSelected == true && (this.GridControl.Filters.Any(x => x.DimensionName == this.FilterItems.Name) || this.GridControl.Filters.Any(x => x.Name == this.FilterItems.Name))) || ((this.FilterItems as FilterItemsCollection)[0].IsSelected != true))
            {
                this.FilterItems.AcceptChanges();
                this.FilterItems.FilteredValues = new List<string>();
                FilterExpression filterExpressionExp = null;
                object propinfo =this.CurrentItemForFilter is PivotItem? GetPropertyDescriptor((this.CurrentItemForFilter as PivotItem).FieldMappingName) : null;
                FilterExpression filterExpression = this.GridControl.Filters.Where(x => x.DimensionName == this.FilterItems.Name).FirstOrDefault();
                if (propinfo != null && (propinfo as PropertyInfo).PropertyType == typeof(DateTime) && (this.CurrentItemForFilter as PivotItem).Format != null)
                {
                    this.FilterItemsForDateTime = GetFilterItemForDateTime(propinfo);
                    filterExpressionExp = this.GridControl.Filters.Where(x => (x.Name == this.FilterItems.Name) && (x.Format == (this.CurrentItemForFilter as PivotItem).Format)).FirstOrDefault();
                }
                else
                    filterExpressionExp = this.GridControl.Filters.Where(x => x.Name == this.FilterItems.Name).FirstOrDefault();
                if (filterExpression == null && filterExpressionExp==null)
                {
                    this.GridControl.UpdateGridLayout = true;
                    if (this.GridControl.ItemSource is IEnumerable)
                    {
                        if (propinfo != null && (propinfo as PropertyInfo).PropertyType == typeof(DateTime) && (this.CurrentItemForFilter as PivotItem).Format != null)
                        {
                            this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterItems.Name, Format = (this.CurrentItemForFilter as PivotItem).Format, Expression = this.FilterItems.GetFilterExpression(true, this.FilterItemsForDateTime,(this.CurrentItemForFilter as PivotItem).Format), Tag = this.FilterItems });
                        }
                        else
                            this.GridControl.Filters.Add(new FilterExpression { Name = this.FilterItems.Name, Expression = this.FilterItems.GetFilterExpression(true), Tag = this.FilterItems });
                    }
                }
                else
                {
                    this.GridControl.UpdateGridLayout = true;
                    if (this.GridControl.ItemSource is IEnumerable)
                    {
                        if (filterExpressionExp != null)
                        {
                            if (propinfo != null && (propinfo as PropertyInfo).PropertyType == typeof(DateTime) && (this.CurrentItemForFilter as PivotItem).Format != null)
                                filterExpressionExp.Expression = this.FilterItems.GetFilterExpression(true, this.FilterItemsForDateTime, (this.CurrentItemForFilter as PivotItem).Format);
                            else
                                filterExpressionExp.Expression = this.FilterItems.GetFilterExpression(true);
                        }
                        else
                        {
                            if (filterExpression.GetType() == typeof(DateTime) && (this.CurrentItemForFilter as PivotItem).Format != null)
                                filterExpression.Expression = this.FilterItems.GetFilterExpression(true, this.FilterItemsForDateTime, (this.CurrentItemForFilter as PivotItem).Format);
                            else
                                filterExpression.Expression = this.FilterItems.GetFilterExpression(true);
                        }
                    }
                    ////On FilterExpression change raise PivotSchemaDesigner changed event.
                    this.GridControl.PivotEngine.RaisePivotSchemaChangedEvent(new PivotSchemaChangedArgs());
                }
            }
            this.FilterPopup.IsOpen = false;
        }

        private FilterItemsCollection GetFilterItemForDateTime(object propertyInfo)
        {
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
                    if (type == typeof(DateTime))
                    {
                        if (propertyInfo is PropertyInfo)
                        {
                            filterList.AddIfUnique(new FilterItemElement { Key = (propertyInfo as PropertyInfo).GetValue(item, null).ToString() });
                        }
                    }
                }
            }
            return filterList;
        }

        void GridControl_DataRefreshed(object sender, DataRefreshedArgs e)
        {
            if (this.GridControl.ShowGroupingBar)
            {
                if (this.GridControl.LoadInBackground)
                    this.PositionPopup();
                this.CalculatePivotRowItemWidth();
            }
        }

        void RowHeaderPopup_Opened(object sender, EventArgs e)
        {
            this.PositionPopup();
            this.CalculatePivotRowItemWidth();
        }

        void FilterHeaderArea_ContainerPrepared(object sender, ItemContainerPrepared e)
        {
            this.PositionPopup();
            //if (e.ListBoxItem.Content is FilterItemsCollection)
            //{
            //    this.CalculateComputationInfoWidth();
            //    this.CalculateComputationInfoItemsWidth();
            //}
            this.EnsureFilteringAndSorting(e.ListBoxItem);
        }

        internal void DataHeaderArea_ContainerPrepared(object sender, ItemContainerPrepared e)
        {
            this.EnsureFilteringAndSorting(e.ListBoxItem);
        }

        void ColumnHeaderArea_ContainerPrepared(object sender, ItemContainerPrepared e)
        {
            if (e.ListBoxItem.Content is PivotItem)
            {
                this.ApplyBorderColor(e.ListBoxItem);

                //PivotItem pivotItem = e.ListBoxItem.Content as PivotItem;// ((PivotItem)((ListBoxItem)sender).Content);

                //int index = this.GridControl.PivotColumns.IndexOf(pivotItem);
                //if (index == this.GridControl.PivotColumns.Count - 1)
                //{
                //    this.CalculateComputationInfoWidth();
                   
                //    this.CalculateComputationInfoItemsWidth();

                //    if (this.GridControl.PivotRows.Count > 0)
                //        this.GridControl.InternalGrid.Model.ColumnWidths[this.GridControl.PivotRows.Count - 1] += 1;
                  
                //}

                if (this.GridControl.PivotColumns.Count > 0 && this.GridControl.InternalGrid.Model.RowHeights.LineCount>0)
                { 
                   this.GridControl.InternalGrid.Model.RowHeights[0] = this.GridControl.InternalGrid.Model.RowHeights.DefaultLineSize;
                }
            }

            this.EnsureFilteringAndSorting(e.ListBoxItem);

            this.PositionPopup();
        }

        void RowHeaderArea_ContainerPrepared(object sender, ItemContainerPrepared e)
        {
            if (this.RowHeaderArea.Items.Count == 1)
            {
                if (this.RowHeaderArea.Items[0] is TextBlock)
                {
                    return;
                }
                this.CalculatePivotRowItemWidth();
            }
            else
            {
                if (this.ContainerEmpty)
                {
                    int lastIndex = this.GridControl.PivotRows.IndexOf(e.ListBoxItem.Content as PivotItem);
                    if (lastIndex == (this.RowHeaderArea.Items.Count - 1))
                    {
                        this.CalculatePivotRowItemWidth();
                    }
                }
            }
            this.CalculatePivotRowItemWidth();
            this.EnsureFilteringAndSorting(e.ListBoxItem);
            this.PositionPopup();
        }

        #endregion

        #region [ Command Event ]

        internal void PivotItem_CommandExecute(object sender, CommandEventArgs e)
        {
            if (e.IsSort)
            {
                if (this.AllowSorting)
                {
                    PivotItem item = e.Item as PivotItem;
                    if (item.Comparer == null)
                    {
                        UpdateDescendingIcon(sender, e);

                        item.Comparer = new ReverseOrderComparer();
                        this.GridControl.UpdateGridLayout = true;
                        this.GridControl.InternalRefresh();                        
                    }

                     else if(item.Comparer is ReverseOrderComparer)
                    {
                        item.Comparer = null;

                        UpdateAscendingIcon(sender, e);

                        this.GridControl.UpdateGridLayout = true;
                        this.GridControl.InternalRefresh();
                    }

                    else
                    {
                        if (_savedComparers == null)
                        {
                            _savedComparers = new Dictionary<PivotItem, IComparer>();
                        }
                        if (_savedComparers.ContainsKey(item))
                        {
                            item.Comparer = _savedComparers[item];
                            _savedComparers.Remove(item);
                            UpdateAscendingIcon(sender, e);
                            this.GridControl.UpdateGridLayout = true;
                            this.GridControl.InternalRefresh();
                        }
                        else
                        {
                            _savedComparers.Add(item, item.Comparer);
                            UpdateDescendingIcon(sender, e);
                            item.Comparer = new ReverseCustomComparer(item.Comparer);
                            this.GridControl.UpdateGridLayout = true;
                            this.GridControl.InternalRefresh();
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < this.ColumnHeaderArea.Items.Count; i++)
                    {
                        //Path sortPath = Common.FindVisualChildWithName<Path>(this.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i), "sortPath");
                        //if (asortPath != null)
                        //{
                        //    asortPath.Visibility = System.Windows.Visibility.Collapsed;
                        //}

                        //Path bsortPath = Common.FindVisualChildWithName<Path>(this.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i), "bsortPath");
                        //if (bsortPath != null)
                        //{
                        //    bsortPath.Visibility = System.Windows.Visibility.Collapsed;
                        //}
                    }
                }
            }
            else
            {
                //Debugger.Break();
                if (this.FilterPopup.IsOpen)
                    this.FilterPopup.IsOpen = false;

                if (e.Item is PivotItem)
                {
                    PivotItem item = e.Item as PivotItem;
                    if (this.GridControl.PivotColumns.Contains(item))
                    {
                        Point pnt = ((Button)e.Tag).TransformToVisual(this as UIElement).Transform(new Point(0, 0));
                        this.FilterPopup.IsOpen = true;
                        this.FilterPopup.HorizontalOffset = pnt.X;
                        this.FilterPopup.VerticalOffset = pnt.Y;
                        CurrentItemForFilter = item;
                    }

                    else if (this.GridControl.PivotRows.Contains(item))
                    {
                        Point pnt = ((Button)e.Tag).TransformToVisual(this.GridControl as UIElement).Transform(new Point(0, 0));
                        this.FilterPopup.IsOpen = true;
                        this.FilterPopup.HorizontalOffset = pnt.X;
                        this.FilterPopup.VerticalOffset = pnt.Y;// +GetRowHeights();
                        CurrentItemForFilter = item;
                    }
                }

                else if (e.Item is FilterItemsCollection)
                {
                    Point pnt = ((Button)e.Tag).TransformToVisual(this as UIElement).Transform(new Point(0, 0));
                    this.FilterPopup.IsOpen = true;
                    this.FilterPopup.HorizontalOffset = pnt.X;
                    this.FilterPopup.VerticalOffset = pnt.Y;
                    CurrentItemForFilter = e.Item;
                }
            }
        }

        private void UpdateDescendingIcon(object sender, CommandEventArgs e)
        {
            if (((FrameworkElement)sender).Name == "PART_ColumnList")
            {
                for (int i = 0; i < this.ColumnHeaderArea.Items.Count; i++)
                {
                    ListBoxItem lstitem = this.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (lstitem.Content == e.Item)
                    {
                        Path path = Common.FindVisualChildWithName<Path>(lstitem, "colSortPath");
                        if (path != null)
                        {
                            string data = " F1 M 174.248,174.201L 174.248,174.201C 174.119,174.201 174.014,174.403 174.014,174.654C 174.014,174.774 174.039,174.891 174.082,174.972L 177.906,180.759C 177.956,180.83 178.018,180.873 178.084,180.873C 178.151,180.873 178.213,180.83 178.263,180.759L 179.156,179.411L 182.092,174.976C 182.135,174.896 182.161,174.778 182.161,174.659C 182.161,174.407 182.057,174.205 181.929,174.205L 180.434,174.204L 175.744,174.202L 174.248,174.201 Z";
                            if (path != null)
                                path.Data = Common.GetPathGeometry(data);
                        }
                    }
                }
            }

            else if (((FrameworkElement)sender).Name == "PART_RowList")
            {
                for (int i = 0; i < this.RowHeaderArea.Items.Count; i++)
                {
                    ListBoxItem lstitem = this.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (lstitem.Content == e.Item)
                    {
                        Path path = Common.FindVisualChildWithName<Path>(lstitem, "rowSortPath");
                        if (path != null)
                        {

                            string data = " F1 M 174.248,174.201L 174.248,174.201C 174.119,174.201 174.014,174.403 174.014,174.654C 174.014,174.774 174.039,174.891 174.082,174.972L 177.906,180.759C 177.956,180.83 178.018,180.873 178.084,180.873C 178.151,180.873 178.213,180.83 178.263,180.759L 179.156,179.411L 182.092,174.976C 182.135,174.896 182.161,174.778 182.161,174.659C 182.161,174.407 182.057,174.205 181.929,174.205L 180.434,174.204L 175.744,174.202L 174.248,174.201 Z";
                            if (path != null)
                                path.Data = Common.GetPathGeometry(data);
                        }
                    }
                }
            }
        }

        private void UpdateAscendingIcon(object sender, CommandEventArgs e)
        {
            if (((FrameworkElement)sender).Name == "PART_ColumnList")
            {
                for (int i = 0; i < this.ColumnHeaderArea.Items.Count; i++)
                {
                    ListBoxItem lstitem = this.ColumnHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (lstitem.Content == e.Item)
                    {
                        Path path = Common.FindVisualChildWithName<Path>(lstitem, "colSortPath");
                        if (path != null)
                        {

                            string data = " F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z";
                            if (path != null)
                                path.Data = Common.GetPathGeometry(data);
                        }
                    }
                }
            }

            else if (((FrameworkElement)sender).Name == "PART_RowList")
            {
                for (int i = 0; i < this.RowHeaderArea.Items.Count; i++)
                {
                    ListBoxItem lstitem = this.RowHeaderArea.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    if (lstitem.Content == e.Item)
                    {
                        Path path = Common.FindVisualChildWithName<Path>(lstitem, "rowSortPath");
                        if (path != null)
                        {

                            string data = " F1 M 181.926,180.872L 181.926,180.872C 182.055,180.872 182.16,180.671 182.16,180.419C 182.16,180.299 182.134,180.182 182.091,180.101L 178.264,174.316C 178.215,174.246 178.152,174.203 178.086,174.203C 178.019,174.203 177.957,174.246 177.908,174.316L 177.015,175.665L 174.081,180.101C 174.038,180.182 174.012,180.299 174.012,180.419C 174.012,180.671 174.116,180.872 174.245,180.872L 175.74,180.872L 180.429,180.872L 181.926,180.872 Z";
                            if (path != null)
                                path.Data = Common.GetPathGeometry(data);
                        }
                    }
                }
            }
        }

        #endregion
    }

    class PivotFilterElementComparer : IComparer<FilterItemElement>
    {
        IComparer comparer = null;
        public PivotFilterElementComparer(IComparer comparer)
        {
            this.comparer = comparer;
        }
        public int Compare(FilterItemElement x, FilterItemElement y)
        {
            return comparer.Compare(x.Key, y.Key);
        }
    }
}
